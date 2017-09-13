using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Microsoft.AspNet.SignalR;
using org.willowcreek.Financial;
using System.Data;
using Rock.Data;
using System.Text;
using System.Data.SqlClient;

namespace RockWeb.Plugins.org_willowcreek.Finance
{
    [DisplayName( "Teller Import" )]
    [Category( "org_willowcreek > Finance" )]
    [Description( "Block for importing online giving files to a batch." )]

    [TextField( "Shelby Teller Database Server", "The Server name where the Shelby Teller databases are stored.", required: true, order: 1, key: "Server" )]
    [TextField( "Shelby Teller Database Names", "The Database name(s) containing the Shelby Teller transactions. If using more than one, separate them by commas, e.g. Teller1,Teller2,Teller3", required: true, order: 2, key: "DatabaseNames" )]
    [TextField( "Images Folder Physical Path", "The location of the Shelby Teller Images folder as stored in the database, e.g. C:\\Program Files\\ShelbyTELLER\\Server\\Images", required: true, order: 3, key: "ImagesPhysicalPath" )]
    [TextField( "Images Folder Server Path", "The shared path to the Shelby Teller Images folder, as accessible by the Rock system, e.g. \\\\ShelbyTeller\\Images", required: true, order: 4, key: "ImagesServerPath" )]
    [DateField( "Start Date", "Import batches from this date forward.", required: true, order: 5, key: "StartDate" )]
    [BooleanField( "Import Back of Check", "Check this box if you would like to import both sides of the check", order: 6, key: "ImportBackOfCheck" )]
    [TextField( "Batch Name Prefix", "Text that should begin the name of each imported batch", required: false, order: 7, key: "BatchNamePrefix" )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.FINANCIAL_SOURCE_TYPE, "Transaction Source", "The source type that should be set on each imported transaction", false, order: 8, key: "Source", defaultValue: Rock.SystemGuid.DefinedValue.FINANCIAL_SOURCE_TYPE_ONSITE_COLLECTION )]
    public partial class TellerImport : Rock.Web.UI.RockBlock
    {
        #region Fields

        private int? _binaryFileId = null;
        private IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<RockMessageHub>();
        private decimal _progress = 0;
        private decimal _total = 0;
        private static int _transactionTypeContributionId = Rock.Web.Cache.DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION.AsGuid() ).Id;
        private static int _currencyTypeCheck = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CHECK.AsGuid() ).Id;
        private static int _currencyTypeCash = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CASH.AsGuid() ).Id;
        private static int _currencyTypeACH = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_ACH.AsGuid() ).Id;
        protected string signalREventName = "tellerImport";

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );
            _binaryFileId = ViewState["BinaryFileId"] as int?;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            signalREventName = string.Format( "tellerImport_{0}_{1}", this.BlockId, Session.SessionID );

            RockPage.AddScriptLink( "~/Scripts/jquery.signalR-2.2.0.min.js", fingerprint: false );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );

            gfBatchFilter.ApplyFilterClick += gfBatchFilter_ApplyFilterClick;
            gfBatchFilter.ClearFilterClick += gfBatchFilter_ClearFilterClick;
            gfBatchFilter.DisplayFilterValue += gfBatchFilter_DisplayFilterValue;

            gBatchList.DataKeyNames = new string[] { "BatchId" };
            gBatchList.GridRebind += gBatchList_GridRebind;

            dvpCompany.DefinedTypeId = new RockContext().DefinedTypes.Where( dt => dt.Name == "Company" ).Select( dt => dt.Id ).SingleOrDefault();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            nbMessage.Visible = false;

            DateTime startDate;
            if ( !DateTime.TryParse( GetAttributeValue( "StartDate" ), out startDate ) )
            {
                pnlSetup.Visible = true;
                pnlEntry.Visible = false;
                pnlConfirm.Visible = false;
                pnlResults.Visible = false;
            }
            else
            {
                if ( !Page.IsPostBack )
                {
                    BindGrid();
                    BindFilter();
                    ShowEntry();
                }
            }
        }

        private void BindGrid()
        {
            gBatchList.DataSource = GetBatchInfo();
            gBatchList.DataBind();
        }

        private DataTable GetBatchInfo()
        {
            var startDate = DateTime.Parse( GetAttributeValue( "StartDate" ) );
            var serverName = GetAttributeValue( "Server" );
            var databaseNames = GetAttributeValue( "DatabaseNames" );

            return GetBatchInfo( serverName, databaseNames, startDate, gfBatchFilter, gBatchList.SortProperty );
        }

        public static DataTable GetBatchInfo( string serverName, string databaseNames, DateTime startDate, GridFilter gridFilter, SortProperty sortProperty )
        {
            // Filter
            var filterClause = string.Empty;

            // Filter by date
            var dateRangeValue = gridFilter.GetUserPreference( "Date Range" );
            if ( !string.IsNullOrWhiteSpace( dateRangeValue ) )
            {
                var drp = new DateRangePicker();
                drp.DelimitedValues = dateRangeValue;
                if ( drp.LowerValue.HasValue )
                {
                    filterClause += " and ActualDate >= '" + drp.LowerValue.Value.ToShortDateString() + "'";
                }

                if ( drp.UpperValue.HasValue )
                {
                    filterClause += " and ActualDate < '" + drp.UpperValue.Value.AddDays( 1 ) + "'";
                }
            }

            // Filter by company
            var companyFilter = gridFilter.GetUserPreference( "Company" );
            if ( !string.IsNullOrWhiteSpace( companyFilter ) )
            {
                //filterClause += $" and DVC.Id IN ({string.Join(",", companyFilter.Split(',').AsIntegerList())})";
                filterClause += " and DVC.Id IN (" + companyFilter + ")";
            }

            // Filter by account
            var accountFilter = gridFilter.GetUserPreference( "Account" );
            if ( !string.IsNullOrWhiteSpace( accountFilter ) && accountFilter != "0" )
            {
                filterClause += " and FA.Id IN (" + accountFilter + ")";
            }

            // Order By
            var orderBy = "ActualDate ASC";
            if ( sortProperty != null )
            {
                orderBy = sortProperty.Property + " " + sortProperty.DirectionString;
            }

            // Generate the batch query
            var sbBatchQuery = new StringBuilder();
            foreach ( var database in databaseNames.Split( ',' ) )
            {
                if ( sbBatchQuery.Length > 0 )
                    sbBatchQuery.AppendLine( "union all" );
                sbBatchQuery.AppendLine( @"select distinct B.BatchId, B.BatchNo, B.ActualDate, B.Checks, GL_Acct 
                                from " + serverName + "." + database + @".dbo.Batch B
                                join " + serverName + "." + database + @".dbo.Docs D on B.RunId = D.RunId AND B.BatchId = D.BatchId AND D.Status = 200
                                where B.Deposited = 'T'
                                and D.SequenceNo > 1
                                and ActualDate >= '" + startDate.ToString( "MM/dd/yyyy" ) + "'" );
            }
            var batchQuery = @"select Company = DVC.Value
                 , Account = SUBSTRING(GL_Acct, 5, 6) + ' ' + SUBSTRING(GL_Acct, 1, 4) + SUBSTRING(GL_Acct, 12, 1000) 
                 , Batch = BatchNo
                 , [Date] = ActualDate
                 , Checks
                 , BatchId
                from (" + sbBatchQuery.ToString() + @") X
                left join FinancialAccount FA on FA.Name = SUBSTRING(GL_Acct, 5, 6) + ' ' + SUBSTRING(GL_Acct, 1, 4) + SUBSTRING(GL_Acct, 12, 1000)
                left join AttributeValue AVC2 on AVC2.AttributeId = (select id from Attribute where EntityTypeId = 76 /* FinancialAccount */ and [Key] = 'Company') and AVC2.EntityId = FA.Id
                left join DefinedValue DVC on AVC2.Value = CONVERT(nvarchar(max), DVC.Guid)
                where not exists (select 1 from FinancialBatch FB where ForeignGuid = X.BatchID)
                and not exists (select 1 from AttributeValue AV where AV.AttributeId = 29392 and AV.EntityId = FA.Id and ValueAsBoolean = 0)";
            batchQuery += filterClause + " order by " + orderBy;

            DataTable data;
            try
            {
                data = DbService.GetDataTable( batchQuery, CommandType.Text, null );
            }
            catch ( SqlException ex )
            {
                throw new Exception( batchQuery, ex );
            }
            return data;
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            ViewState["BinaryFileId"] = _binaryFileId;
            return base.SaveViewState();
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
            BindGrid();
        }

        /// <summary>
        /// Handles the DisplayFilterValue event of the gfBatchFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Rock.Web.UI.Controls.GridFilter.DisplayFilterValueArgs"/> instance containing the event data.</param>
        protected void gfBatchFilter_DisplayFilterValue( object sender, Rock.Web.UI.Controls.GridFilter.DisplayFilterValueArgs e )
        {
            switch ( e.Key )
            {
                case "Date Range":
                    {
                        e.Value = DateRangePicker.FormatDelimitedValues( e.Value );
                        break;
                    }
                case "Company":
                    {
                        var companyList = e.Value.Split( ',' ).ToList();
                        e.Value = new RockContext().DefinedValues.Where( x => companyList.Contains( x.Id.ToString() ) ).Select( x => x.Value ).ToList().AsDelimited( "," );
                        break;
                    }
                case "Account":
                    {
                        if ( e.Value == "0" )
                        {
                            e.Value = string.Empty;
                        }
                        else
                        {
                            var accountList = e.Value.Split( ',' ).AsIntegerList();
                            e.Value = string.Join( ", ", new FinancialAccountService( new RockContext() ).Queryable().Where( a => accountList.Contains( a.Id ) ).Select( x => x.Name ) );
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Handles the ApplyFilterClick event of the gfBatchFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gfBatchFilter_ApplyFilterClick( object sender, EventArgs e )
        {
            gfBatchFilter.SaveUserPreference( "Date Range", drpBatchDate.DelimitedValues );
            gfBatchFilter.SaveUserPreference( "Company", dvpCompany.SelectedValues.AsDelimited( "," ) );
            gfBatchFilter.SaveUserPreference( "Account", string.Join( ",", apAccount.SelectedValues ) );
            BindGrid();
        }

        /// <summary>
        /// Handles the Delete event of the gBatchList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gBatchList_Delete( object sender, RowEventArgs e )
        {
            var rockContext = new RockContext();
            var batchService = new FinancialBatchService( rockContext );
            var transactionService = new FinancialTransactionService( rockContext );
            var batch = batchService.Get( e.RowKeyId );
            if ( batch != null )
            {
                if ( UserCanEdit || batch.IsAuthorized( Rock.Security.Authorization.EDIT, CurrentPerson ) )
                {
                    string errorMessage;
                    if ( !batchService.CanDelete( batch, out errorMessage ) )
                    {
                        mdGridWarning.Show( errorMessage, ModalAlertType.Information );
                        return;
                    }

                    rockContext.WrapTransaction( () =>
                    {
                        foreach ( var txn in transactionService.Queryable()
                            .Where( t => t.BatchId == batch.Id ) )
                        {
                            transactionService.Delete( txn );
                        }
                        HistoryService.SaveChanges(
                            rockContext,
                            typeof( FinancialBatch ),
                            Rock.SystemGuid.Category.HISTORY_FINANCIAL_BATCH.AsGuid(),
                            batch.Id,
                            new List<string> { "Deleted the batch" } );

                        batchService.Delete( batch );

                        rockContext.SaveChanges();
                    } );
                }
            }

            BindGrid();
        }

        /// <summary>
        /// Handles the ClearFilterClick event of the gfBatchFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gfBatchFilter_ClearFilterClick( object sender, EventArgs e )
        {
            gfBatchFilter.DeleteUserPreferences();
            BindFilter();
        }

        /// <summary>
        /// Binds the filter
        /// </summary>
        private void BindFilter()
        {
            drpBatchDate.DelimitedValues = gfBatchFilter.GetUserPreference( "Date Range" );

            dvpCompany.ClearSelection();
            var companyFilter = gfBatchFilter.GetUserPreference( "Company" );
            if ( !string.IsNullOrWhiteSpace( companyFilter ) )
                foreach ( var company in companyFilter.Split( ',' ) )
                {
                    dvpCompany.Items.FindByValue( company ).Selected = true;
                }

            var accounts = gfBatchFilter.GetUserPreference( "Account" ).Split( ',' ).AsIntegerList();
            apAccount.SetValues( accounts );
        }

        /// <summary>
        /// Handles the GridRebind event of the gBatchList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gBatchList_GridRebind( object sender, GridRebindEventArgs e )
        {
            BindGrid();
        }

        /// <summary>
        /// Handles the Click event of the btnImport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnImport_Click( object sender, EventArgs e )
        {
            ShowConfirmation();
        }

        protected void updateProgress( decimal percentage )
        {
            //progress++;
            //var percentage = (progress * 100) / total;
            _hubContext.Clients.All.receiveNotification( signalREventName, percentage.ToString( "N1" ) );
        }

        protected void btnConfirm_Click( object sender, EventArgs e )
        {
            // send signalR message to start progress indicator
            _hubContext.Clients.All.receiveNotification( signalREventName, "0" );

            var startDate = DateTime.Parse( GetAttributeValue( "StartDate" ) );
            var serverName = GetAttributeValue( "Server" );
            var databaseNames = GetAttributeValue( "DatabaseNames" );
            var imagesPhysicalPath = GetAttributeValue( "ImagesPhysicalPath" );
            var imagesServerPath = GetAttributeValue( "ImagesServerPath" );
            var batchNamePrefix = ( GetAttributeValue( "BatchNamePrefix" ) ?? string.Empty ).Trim();
            var source = GetAttributeValue( "Source" );
            var importBackOfCheck = bool.Parse( GetAttributeValue( "ImportBackOfCheck" ) );

            var batchesSelected = new List<Guid>();
            gBatchList.SelectedKeys.ToList().ForEach( b => batchesSelected.Add( b.ToString().AsGuid() ) );

            var result = Teller.ImportBatches( startDate, serverName, databaseNames, imagesPhysicalPath, imagesServerPath, importBackOfCheck, batchNamePrefix, source, batchesSelected, updateProgress );

            var transactionTerm = result.Transactions == 1 ? "1 transaction" : result.Transactions.ToString() + " transactions";
            var batchTerm = result.Batches == 1 ? "1 batch" : result.Batches.ToString() + " batches";
            nbSuccess.Text = "Imported " + transactionTerm + " in " + batchTerm + ".";
            nbErrors.Visible = false;

            ShowResults();
        }

        /// <summary>
        /// Handles the Click event of the btnCancelConfirm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnCancelConfirm_Click( object sender, EventArgs e )
        {
            ShowEntry();
        }

        /// <summary>
        /// Handles the Click event of the btnImportMore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnImportMore_Click( object sender, EventArgs e )
        {
            BindGrid();
            ShowEntry();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the entry.
        /// </summary>
        private void ShowEntry()
        {
            pnlEntry.Visible = true;
            pnlConfirm.Visible = false;
            pnlResults.Visible = false;
            pnlSetup.Visible = false;
        }

        /// <summary>
        /// Shows the confirmation.
        /// </summary>
        private void ShowConfirmation()
        {
            if ( gBatchList.SelectedKeys.Any() )
            {
                // Format a confirmation message based on number of txns and the batch (or lack of) selected
                lConfirm.Text = "This will import the <strong>" + gBatchList.SelectedKeys.Count().ToString() + " selected batches only</strong>. Click <em>Confirm</em> to continue.";
                // lConfirm.Text = $"This will import the <strong>{gBatchList.SelectedKeys.Count():N0} selected batches only</strong>. Click <em>Confirm</em> to continue.";
            }
            else
            {
                lConfirm.Text = "This will import <strong>all available batches</strong>. Click <em>Confirm</em> to continue.";
            }

            // Show the confirm/status/result dialog 
            pnlEntry.Visible = false;
            pnlConfirm.Visible = true;
            pnlResults.Visible = false;
            pnlSetup.Visible = false;
        }

        /// <summary>
        /// Shows the results.
        /// </summary>
        private void ShowResults()
        {
            pnlEntry.Visible = false;
            pnlConfirm.Visible = false;
            pnlResults.Visible = true;
            pnlSetup.Visible = false;
        }

        #region Show Notifications

        /// <summary>
        /// Shows a warning.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        private void ShowWarning( string title, string message )
        {
            nbMessage.NotificationBoxType = NotificationBoxType.Warning;
            ShowMessage( title, message );
        }

        /// <summary>
        /// Shows a error.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        private void ShowError( string title, string message )
        {
            nbMessage.NotificationBoxType = NotificationBoxType.Danger;
            ShowMessage( title, message );
        }

        /// <summary>
        /// Shows a message.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        private void ShowMessage( string title, string message )
        {
            nbMessage.Title = title;
            nbMessage.Text = message;
            nbMessage.Visible = true;
        }

        #endregion

        #endregion


    }

}
