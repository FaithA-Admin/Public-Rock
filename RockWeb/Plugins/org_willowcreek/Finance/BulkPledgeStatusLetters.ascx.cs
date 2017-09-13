using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.MergeTemplates;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_willowcreek.Finance
{
    [DisplayName( "Bulk Pledge Status Letters" )]
    [Category( "org_willowcreek > Finance" )]
    [Description( "Used for merging pledge data into output documents, such as Word, Html, using a pre-defined template." )]
    [IntegerField( "Database Timeout", "The number of seconds to wait before reporting a database timeout.", false, 180, "", 0 )]
    [BinaryFileTypeField( "File Type", "The file type used to save the contribution statements.", true, "FC7218EE-EA28-4EA4-8C3D-F30750A2FE59" )]
    public partial class BulkPledgeStatusLetters : RockBlock
    {

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );

            //// set postback timeout to whatever the DatabaseTimeout is plus an extra 5 seconds so that page doesn't timeout before the database does
            //// note: this only makes a difference on Postback, not on the initial page visit
            int databaseTimeout = GetAttributeValue( "DatabaseTimeout" ).AsIntegerOrNull() ?? 180;
            var sm = ScriptManager.GetCurrent( this.Page );
            if ( sm.AsyncPostBackTimeout < databaseTimeout + 5 )
            {
                sm.AsyncPostBackTimeout = databaseTimeout + 5;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                ShowDetail();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Handles the Click event of the btnMerge control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnMerge_Click( object sender, EventArgs e )
        {
            nbWarningMessage.Visible = false;
            nbNotification.Visible = false;

            var rockContext = new RockContext();

            BinaryFileType binaryFileType = new BinaryFileTypeService( rockContext ).Get( GetAttributeValue( "FileType" ).AsGuid() );
            if ( binaryFileType == null )
            {
                nbWarningMessage.Text = "Unable to get File Type, please configure this in the Block Settings.";
                nbWarningMessage.NotificationBoxType = NotificationBoxType.Danger;
                nbWarningMessage.Visible = true;
                return;
            }

            var globalAttributes = Rock.Web.Cache.GlobalAttributesCache.Read();
            var templateGuid = globalAttributes.GetValue( "PledgeStatusLetterTemplate" ).AsGuid();
            if ( templateGuid == Guid.Empty )
            {
                nbWarningMessage.Text = "The PledgeStatusLetterTemplate global attribute is not defined.";
                nbWarningMessage.NotificationBoxType = NotificationBoxType.Danger;
                nbWarningMessage.Visible = true;
                return;
            }

            var transaction = new org.willowcreek.Financial.GeneratePledgeStatusLetterTransaction();
            transaction.Context = Context;
            transaction.Response = Response;
            transaction.DatabaseTimeout = GetAttributeValue( "DatabaseTimeout" ).AsIntegerOrNull();

            var accountService = new FinancialAccountService( rockContext );
            var account = accountService.Get( apAccount.ItemId.AsInteger() );
            account.LoadAttributes();

            if ( !account.AttributeValues.ContainsKey( "CampaignName" ) || string.IsNullOrWhiteSpace(account.AttributeValues["CampaignName"].Value ))
            {
                nbWarningMessage.Text = "The specified Account does not have a Campaign Name defined. Please define this attribute on the Account Detail screen or select a different Account.";
                nbWarningMessage.NotificationBoxType = NotificationBoxType.Danger;
                nbWarningMessage.Visible = true;
                return;
            }

            if ( !account.AttributeValues.ContainsKey( "PledgeLinkRole" ) || string.IsNullOrWhiteSpace( account.AttributeValues["PledgeLinkRole"].Value ) )
            {
                nbWarningMessage.Text = "The specified Account does not have a Pledge Link Role defined. Please define this attribute on the Account Detail screen or select a different Account.";
                nbWarningMessage.NotificationBoxType = NotificationBoxType.Danger;
                nbWarningMessage.Visible = true;
                return;
            }

            transaction.AccountId = account.Id;
            transaction.Title = account.AttributeValues["CampaignName"].Value;
            var url = account.Campus.Url;
            if ( url.Count( s => s == '.' ) > 1 )
            {
                url = url.Substring( url.IndexOf( "." ) + 1 );
            }
            if (url.Substring(url.Length - 1) == "/")
            {
                url = url.Substring( 0, url.Length - 1 );
            }
            transaction.AddressLine = account.Campus.Location.ToString().Replace("\r\n", ", ") + "   " + account.Campus.PhoneNumber + "   " + url;
            transaction.EndDate = dpEndDate.SelectedDate.Value;
            transaction.IncludeContributorsWithNoPledge = cbIncludeNoPledge.Checked;
            transaction.BinaryFileType = binaryFileType;
            transaction.Requestor = CurrentPerson;
            transaction.ChapterSize = nbChapterSize.Text.AsInteger();

            if ( ppPerson.PersonId != null )
            {
                transaction.GivingId = new PersonService( rockContext ).Queryable().Where( p => p.Id == ppPerson.PersonId.Value ).Select( p => p.GivingId ).Single();
            }
            Rock.Transactions.RockQueue.TransactionQueue.Enqueue( transaction );

            SetBlockUserPreference( "ChapterSize", nbChapterSize.Text );
            //SetBlockUserPreference( "AccountId", apAccount.ItemId.AsInteger() );

            nbNotification.Visible = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the detail.
        /// </summary>
        private void ShowDetail()
        {
            nbNotification.Visible = false;
            var delimitedDateValues = GetBlockUserPreference( "Date Range" );

            var chapterSize = GetBlockUserPreference( "ChapterSize" );
            if ( !string.IsNullOrWhiteSpace( chapterSize ) )
            {
                nbChapterSize.Text = chapterSize;
            }

            //var year = GetBlockUserPreference( "Year" );
            //if ( !string.IsNullOrWhiteSpace( year ) )
            //{
            //    ypYear.Text = year;
            //}
        }

        #endregion        
    }
}