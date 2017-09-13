using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;
using System.Data.Entity;
using Rock.Web.UI;
using System.Data;
using Financial = org.willowcreek.Financial;

namespace RockWeb.Plugins.org_willowcreek.Finance
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Contribution Statement List" )]
    [Category( "org_willowcreek > Finance" )]
    [Description( "Block for displaying a listing of years where contribution statements are available." )]
    [IntegerField("Max Years To Display", "The maximum number of years to display (including the current year).", true, 7, order:1)]
    [BinaryFileTypeField( "File Type", "The file type used to save the contribution statements.", true, "FC7218EE-EA28-4EA4-8C3D-F30750A2FE59", order:3 )]
    [BooleanField("Convert To PDF", "Saving as PDF requires that the version of Microsoft Word used to create the template be installed on the server.", order: 4 )]
    [BooleanField("Use Person Context", "Determines if the person context should be used instead of the CurrentPerson.", true, order: 5)]
    [ContextAware]
    public partial class ContributionStatementList : RockBlock
    {
        
        #region Properties

        /// <summary>
        /// Gets the target person.
        /// </summary>
        /// <value>
        /// The target person.
        /// </value>
        protected Person TargetPerson { get; private set; }

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            if ( GetAttributeValue( "UsePersonContext" ).AsBoolean() )
            {
                TargetPerson = ContextEntity<Person>();
            }
            else
            {
                TargetPerson = CurrentPerson;
            }

            var numberOfYears = GetAttributeValue( "MaxYearsToDisplay" ).AsInteger();

            RockContext rockContext = new RockContext();

            FinancialTransactionDetailService financialTransactionDetailService = new FinancialTransactionDetailService( rockContext );

            if ( TargetPerson != null )
            {
            //    // fetch all the possible PersonAliasIds that have this GivingID to help optimize the SQL
            //    var personAliasIds = new PersonAliasService( rockContext ).Queryable().Where( a => a.Person.GivingId == TargetPerson.GivingId ).Select( a => a.Id ).ToList();

            //    // get the transactions for the person or all the members in the person's giving group (Family)
            //    var qry = financialTransactionDetailService.Queryable().AsNoTracking()
            //                .Where( t => t.Transaction.AuthorizedPersonAliasId.HasValue && personAliasIds.Contains( t.Transaction.AuthorizedPersonAliasId.Value ) );

            //    if ( string.IsNullOrWhiteSpace( GetAttributeValue( "Accounts" ) ) )
            //    {
            //        qry = qry.Where( t => t.Account.IsTaxDeductible );
            //    }
            //    else
            //    {
            //        var accountGuids = GetAttributeValue( "Accounts" ).Split( ',' ).Select( Guid.Parse ).ToList();
            //        qry = qry.Where( t => accountGuids.Contains( t.Account.Guid ) );
            //    }

            //    // Materialize the data
            //    var details = qry.ToList();

            //    // Load Payment Detail Attributes
            //    foreach ( var detail in details )
            //    {
            //        detail.Transaction.FinancialPaymentDetail.LoadAttributes();
            //    }

            //    // Exclude transactions that contain a value for the Non-Tax Exempt Conduit attribute
            //    var conduitKey = "ConduitNon-TaxExempt";
            //    var yearQry = details.Where( d => !d.Transaction.FinancialPaymentDetail.Attributes.ContainsKey( conduitKey ) || d.Transaction.FinancialPaymentDetail.AttributeValues[conduitKey].Value == string.Empty )
            //                        .GroupBy( t => t.Transaction.TransactionDateTime.Value.Year )
            //                        .Select( g => g.Key )
            //                        .OrderByDescending( y => y );

                var years = Financial.ContributionStatements.StatementsAvailable( TargetPerson, GetAttributeValue( "Accounts" ) );

                if ( years.Any() )
                {
                    var html = new Literal();
                    html.Text = "<hr /><p class=\"margin-t-md\"><strong>< i class='fa fa-file-text-o'></i>Available Contribution Statements</strong></p>";
                    foreach ( var year in years.Take( numberOfYears ) )
                    {
                        var button = new LinkButton();
                        button.Text = year.ToString() + ( year == DateTime.Now.Year ? " <small>YTD</small>" : "" );
                        button.ID = "btnStatement" + year.ToString();
                        button.CssClass = "btn btn-sm btn-default";
                        button.Style.Add( "margin-right", "5px" );
                        phStatementButtons.Controls.Add( button );
                        button.Click += new EventHandler( this.btnMerge_Click );
                        ScriptManager.GetCurrent( this.Page ).RegisterPostBackControl( button );
                    }
                }
            }

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        #endregion

        #region Events

        // handlers called by the controls on your block

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {

        }

        protected void btnMerge_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                var button = sender as LinkButton;
                var year = button.ID.Replace( "btnStatement", "" ).AsInteger();

                var binaryFileTypeId = new BinaryFileTypeService( rockContext ).Get( GetAttributeValue( "FileType" ).AsGuid() ).Id;
                var outputBinaryFileDoc = Financial.ContributionStatements.GenerateOne( binaryFileTypeId, GetAttributeValue( "ConvertToPDF" ).AsBoolean(), year, TargetPerson );
                var uri = new UriBuilder( outputBinaryFileDoc.Url );
                var qry = System.Web.HttpUtility.ParseQueryString( uri.Query );
                qry["attachment"] = true.ToTrueFalse();
                uri.Query = qry.ToString();
                Response.Redirect( uri.ToString(), false );
                Context.ApplicationInstance.CompleteRequest();
            }
        }
        #endregion
    }
}