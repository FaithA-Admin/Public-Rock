using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using System.Xml.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Security.Cryptography;
using Rock.Security;
using System.Text;

namespace RockWeb.Plugins.org_willowcreek.Finance
{
    [DisplayName( "Online Giving Import" )]
    [Category( "org_willowcreek > Finance" )]
    [Description( "Block for importing online giving files to a batch." )]

    [LinkedPage( "Batch Detail Page", "The page used to display details of a batch.", false, "", "", 1 )]
    public partial class OnlineGivingImport : Rock.Web.UI.RockBlock
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
        //private decimal _totalAmount = 0.0M;

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
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            nbMessage.Visible = false;

            if ( !Page.IsPostBack )
            {
                ShowEntry();
            }
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

        }

        /// <summary>
        /// Handles the FileUploaded event of the fuTellerFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void fuTellerFile_FileUploaded( object sender, EventArgs e )
        {
            _binaryFileId = fuTellerFile.BinaryFileId;

            List<string> errorMessages;
            var batches = ReadLockboxFile( false, out errorMessages );

            if ( errorMessages.Count == 0 )
            {
                lConfirm.Text = string.Format( "This will import <strong>{0:N0}</strong> transactions in {1} batches. Click <em>Confirm</em> to continue.", batches.Sum( x => x.Donations.Count() ), batches.Count() );
                pnlEntry.Visible = false;
                pnlConfirm.Visible = true;
                pnlResults.Visible = false;
            }
            else
            {
                nbSuccess.Visible = false;
                nbErrors.Visible = true;
                nbErrors.Text = string.Empty;
                foreach ( var message in errorMessages )
                {
                    nbErrors.Text = "<P>" + message;
                }
                ShowResults();
            }
        }

        public static string DecryptLine( string encryptedLine, string decryptionKey )
        {
            string strDataOut = "";
            for ( var lonDataPtr = 0; lonDataPtr <= ( encryptedLine.Length / 2 ) - 1; lonDataPtr++ )
            {
                var hex = encryptedLine.Substring( ( 2 * lonDataPtr ), 2 );
                var intXOrValue1 = int.Parse( hex, System.Globalization.NumberStyles.HexNumber );
                var intXOrValue2 = Convert.ToInt32( decryptionKey.Substring( lonDataPtr, 1 ) );
                strDataOut = strDataOut + ( char ) ( intXOrValue1 ^ intXOrValue2 );
            }
            return strDataOut;
        }

        private List<Lockbox> ReadLockboxFile( bool extensive, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            // Read the whole file line by line
            var encryptedLines = new List<string>();
            if ( _binaryFileId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var binaryFile = new BinaryFileService( rockContext ).Get( _binaryFileId.Value );
                    if ( binaryFile != null )
                    {
                        using ( var stream = binaryFile.ContentStream )
                        {
                            using ( var sr = new StreamReader( stream ) )
                            {
                                string line;
                                while ( ( line = sr.ReadLine() ) != null )
                                {
                                    encryptedLines.Add( line );
                                }
                            }

                        }
                    }
                }
            }

            var batches = new List<Lockbox>();
            var batch = new Lockbox();

            // Decrypt the lines
            var decryptionKey = DbService.ExecuteScaler( "wcUtil_OLGDecryptionKey", System.Data.CommandType.StoredProcedure, new Dictionary<string, object> { { "id", int.Parse( encryptedLines.Last() ) } } ).ToString();
            foreach ( var line in encryptedLines )
            {
                var decryptedLine = DecryptLine( line, decryptionKey );
                if ( decryptedLine == "File Complete" )
                    break;

                var details = decryptedLine.Split( ',' );
                if ( details[0] == "LOCKBOX TOTAL" )
                {
                    batch.Number = batch.Donations.First().LockboxNumber;
                    batch.Total = decimal.Parse( details[1] ) / 100;
                    batch.Account = int.Parse( details[2] );
                    if ( batch.Total != batch.Donations.Sum( x => x.Amount ) )
                    {
                        ShowError( "Incorrect Batch Total", "Total does not match sum of donations on Lockbox " + batch.Number.ToString() );
                    }
                    var result = DbService.ExecuteScaler( "wcUtil_OLGLockboxGLAccount", System.Data.CommandType.StoredProcedure, new Dictionary<string, object> { { "lockbox", batch.Account } } );
                    if ( result == null )
                    {
                        errorMessages.Add( "Lockbox number " + batch.Account + " is not active. Please correct this before proceeding." );
                    }
                    else
                    {
                        batch.GLAccount = result.ToString();

                        if ( extensive )
                        {
                            // Determine the Campus based on the account code
                            using ( var rockContext = new RockContext() )
                            {
                                var financialAccountService = new FinancialAccountService( rockContext );
                                batch.CampusId = financialAccountService.Queryable().Where( a => a.GlCode == batch.GLAccount && a.IsActive == true ).Select( a => a.CampusId ).Distinct().SingleOrDefault();
                            }
                        }
                    }
                    batches.Add( batch );

                    batch = new Lockbox();
                }
                else if ( details[0] == "GRAND TOTAL" )
                {
                    var grandTotal = decimal.Parse( details[1] ) / 100;
                    if ( grandTotal != batches.Sum( x => x.Total ) )
                    {
                        ShowError( "Incorrect Grand Total", "Grand Total does not match sum of batch totals" );
                    }
                }
                else
                {
                    Donation donation = null;
                    if ( extensive )
                    {
                        var userInfo = DbService.GetDataTable( "wcUtil_OLGUserInfo", System.Data.CommandType.StoredProcedure, new Dictionary<string, object> { { "@BankInfoID", int.Parse( details[20] ) } } ).Rows[0];

                        donation = new Donation
                        {
                            LockboxNumber = int.Parse( details[0] ),
                            Sequence = int.Parse( details[1] ),
                            AccountNumber = details[2].Trim(),
                            RoutingNumber = details[3].Trim(),
                            Amount = decimal.Parse( details[5] ) / 100,
                            DepositDate = DateTime.Parse( details[7] ),
                            PhoneType = details[16].Trim(),
                            BankInfoID = int.Parse( details[20] ),
                            FirstName = userInfo["FirstName"].ToString(),
                            NickName = userInfo["NickName"].ToString(),
                            MiddleName = userInfo["MiddleName"].ToString(),
                            LastName = userInfo["LastName"].ToString(),
                            Address1 = userInfo["Address1"].ToString(),
                            Address2 = userInfo["Address2"].ToString(),
                            City = userInfo["City"].ToString(),
                            State = userInfo["State"].ToString(),
                            PostalCode = userInfo["PostalCode"].ToString(),
                            CountryCode = userInfo["CountryCode"].ToString(),
                            PhoneNumber = userInfo["PhoneNumber"].ToString(),
                            Extension = userInfo["Extension"].ToString(),
                            EmailAddress = userInfo["EmailAddress"].ToString(),
                            Birthday = string.IsNullOrWhiteSpace( userInfo["Birthday"].ToString() ) ? ( DateTime? ) null : DateTime.Parse( userInfo["Birthday"].ToString() ),
                            GenderCode = userInfo["GenderCode"].ToString(),
                            MaritalCode = userInfo["MaritalCode"].ToString(),
                            Batch = batch
                        };
                    }
                    else
                    {
                        donation = new Donation
                        {
                            Amount = decimal.Parse( details[5] ) / 100
                        };
                    }

                    if ( batch.Donations == null )
                        batch.Donations = new List<Donation>();

                    batch.Donations.Add( donation );
                    if ( extensive )
                        updateProgress();
                }

            }

            return batches;
        }

        /// <summary>
        /// Handles the Click event of the btnImport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnImport_Click( object sender, EventArgs e )
        {
            List<string> errorMessages;
            var batches = ReadLockboxFile( false, out errorMessages );

            if ( errorMessages.Count == 0 )
            {
                lConfirm.Text = string.Format( "This will import <strong>{0:N0}</strong> transactions in {1} batches. Click <em>Confirm</em> to continue.", batches.Sum( x => x.Donations.Count() ), batches.Count() );
                pnlEntry.Visible = false;
                pnlConfirm.Visible = true;
                pnlResults.Visible = false;
            }
            else
            {
                nbSuccess.Visible = false;
                nbErrors.Visible = true;
                nbErrors.Text = string.Empty;
                foreach ( var message in errorMessages )
                {
                    nbErrors.Text = "<P>" + message;
                }
                ShowResults();
            }
        }



        private class Lockbox
        {
            public List<Donation> Donations;
            public int Number;
            public decimal Total;
            public int Account;
            public string GLAccount;
            public FinancialBatch Batch;
            public int? CampusId;
        }

        private class Donation
        {
            public int LockboxNumber;
            public int Sequence;
            public string AccountNumber;
            public string RoutingNumber;
            public decimal Amount;
            public DateTime DepositDate;
            public string FirstName;
            public string NickName;
            public string MiddleName;
            public string LastName;
            public string Address1;
            public string Address2;
            public string City;
            public string State;
            public string PostalCode;
            public string CountryCode;
            public string PhoneType;
            public string PhoneNumber;
            public string Extension;
            public string EmailAddress;
            public DateTime? Birthday;
            public string GenderCode;
            public string MaritalCode;
            public int BankInfoID;
            public Lockbox Batch;

            public Person SaveAsPerson()
            {
                using ( var rockContext = new RockContext() )
                {
                    var person = new Person
                    {
                        RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id,
                        FirstName = this.FirstName,
                        NickName = this.NickName,
                        MiddleName = this.MiddleName,
                        LastName = this.LastName,
                        Email = this.EmailAddress,
                        RecordStatusValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id,
                        ConnectionStatusValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_ATTENDEE.AsGuid() ).Id
                    };
                    person.SetBirthDate( Birthday );

                    if ( GenderCode == "M" )
                        person.Gender = Gender.Male;
                    else if ( GenderCode == "F" )
                        person.Gender = Gender.Female;

                    if ( !string.IsNullOrWhiteSpace( MaritalCode ) )
                    {
                        var maritalStatusType = rockContext.DefinedTypes.Where( dt => dt.Name == "Marital Status" ).Single();
                        var maritalStatuses = rockContext.DefinedValues.Where( dv => dv.DefinedTypeId == maritalStatusType.Id );
                        switch ( MaritalCode )
                        {
                            case "SING":
                                person.MaritalStatusValue = maritalStatuses.SingleOrDefault( x => x.Value == "Single" );
                                break;
                            case "MAR":
                                person.MaritalStatusValue = maritalStatuses.SingleOrDefault( x => x.Value == "Married" );
                                break;
                            case "SEP":
                                person.MaritalStatusValue = maritalStatuses.SingleOrDefault( x => x.Value == "Separated" );
                                break;
                            case "DIV":
                                person.MaritalStatusValue = maritalStatuses.SingleOrDefault( x => x.Value == "Divorced" );
                                break;
                            case "WID":
                                person.MaritalStatusValue = maritalStatuses.SingleOrDefault( x => x.Value == "Widowed" );
                                break;
                        }
                    }

                    // Add Phone Number
                    if ( !string.IsNullOrWhiteSpace( PhoneNumber ) )
                    {
                        var phone = new PhoneNumber
                        {
                            Number = this.PhoneNumber,
                            Extension = this.Extension
                        };

                        var phoneNumberType = rockContext.DefinedTypes.Where( dt => dt.Name == "Phone Type" ).Single();
                        var phoneTypes = rockContext.DefinedValues.Where( dv => dv.DefinedTypeId == phoneNumberType.Id );
                        switch ( PhoneType )
                        {
                            case "W":
                                phone.NumberTypeValue = phoneTypes.SingleOrDefault( x => x.Value == "Work" );
                                break;
                            case "C":
                                phone.NumberTypeValue = phoneTypes.SingleOrDefault( x => x.Value == "Mobile" );
                                break;
                            default:
                                phone.NumberTypeValue = phoneTypes.SingleOrDefault( x => x.Value == "Home" );
                                break;
                        }

                        person.PhoneNumbers.Add( phone );
                    }

                    var personService = new PersonService( rockContext );
                    var family = PersonService.SaveNewPerson( person, rockContext );
                    family.CampusId = this.Batch.CampusId;

                    // Set Home Address
                    var homeAddress = new Location
                    {
                        Street1 = this.Address1,
                        Street2 = this.Address2,
                        City = this.City,
                        State = this.State,
                        PostalCode = this.PostalCode,
                        Country = this.CountryCode.Replace( "USA", "US" )
                    };

                    var locationService = new LocationService( rockContext );
                    locationService.Verify( homeAddress, false );

                    var homeGroupLocation = new GroupLocation
                    {
                        GroupId = family.Id,
                        GroupLocationTypeValueId = 19, // Home Address
                        IsMailingLocation = true,
                        IsMappedLocation = true, //Set for background check to send address
                        Location = homeAddress
                    };

                    family.GroupLocations.Add( homeGroupLocation );
                    rockContext.SaveChanges();

                    return person;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnConfirm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnConfirm_Click( object sender, EventArgs e )
        {
            // Initialize the status variables
            int matchCount = 0;
            int createdCount = 0;
            int errorCount = 0;
            _progress = 0;
            _total = 0;
            StringBuilder sb = new StringBuilder();
            var allErrorMessages = new List<string>();

            // send signalR message to start progress indicator
            _hubContext.Clients.All.receiveNotification( signalREventName, "0" );

            List<string> errorMessages;
            var batches = ReadLockboxFile( false, out errorMessages ); // Get the totals again, this is redundant but I'm in a hurry and this code should only be around for a short time - CM 9/26/16
            _total = batches.Sum( x => x.Donations.Count() );
            batches = ReadLockboxFile( true, out errorMessages );

            if ( _total > 0 )
            {
                foreach ( var lockboxBatch in batches )
                {
                    matchCount = 0;
                    createdCount = 0;
                    errorCount = 0;
                    using ( var rockContext = new RockContext() )
                    {
                        var batchService = new FinancialBatchService( rockContext );
                        var newGuid = Guid.NewGuid();
                        var batch = new FinancialBatch
                        {
                            Guid = newGuid,
                            Name = newGuid.ToString(), // Temporary name
                            Status = BatchStatus.Pending, // While transactions are being loaded
                            BatchStartDateTime = lockboxBatch.Donations.Min( x => x.DepositDate ),
                            BatchEndDateTime = lockboxBatch.Donations.Max( x => x.DepositDate ),
                            ControlAmount = lockboxBatch.Total,
                            AccountingSystemCode = lockboxBatch.GLAccount,
                            CampusId = lockboxBatch.CampusId
                        };

                        batchService.Add( batch );
                        rockContext.SaveChanges();

                        batch.Name = "OLG Batch " + batch.Id.ToString();
                        rockContext.SaveChanges();

                        // Create Transactions
                        foreach ( var donation in lockboxBatch.Donations )
                        {
                            ProcessStatus status = ProcessStatus.Error;

                            using ( var txContext = new RockContext() )
                            {
                                var rockTx = new FinancialTransaction
                                {
                                    BatchId = batch.Id,
                                    TransactionTypeValueId = _transactionTypeContributionId,
                                    TransactionDateTime = donation.DepositDate,
                                    SourceTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.FINANCIAL_SOURCE_TYPE_WEBSITE.AsGuid() ).Id,
                                    FinancialPaymentDetail = new FinancialPaymentDetail
                                    {
                                        CurrencyTypeValueId = _currencyTypeACH
                                    }
                                };

                                string accountNumberSecured = FinancialPersonBankAccount.EncodeAccountNumber( donation.RoutingNumber, donation.AccountNumber );

                                if ( !string.IsNullOrWhiteSpace( accountNumberSecured ) )
                                {
                                    var matchedPersonIds = new FinancialPersonBankAccountService( txContext )
                                        .Queryable()
                                        .Where( a => a.AccountNumberSecured == accountNumberSecured )
                                        .Select( a => a.PersonAlias.PersonId )
                                        .Distinct()
                                        .ToList();

                                    if ( matchedPersonIds.Count() == 1 )
                                    {
                                        rockTx.AuthorizedPersonAliasId = new PersonAliasService( txContext ).GetPrimaryAliasId( matchedPersonIds.Single() );
                                        status = ProcessStatus.Matched;
                                    }
                                }

                                // If no matching person was found, create one and assign this transaction and bank account to it
                                if ( rockTx.AuthorizedPersonAliasId == null )
                                {
                                    var person = donation.SaveAsPerson();
                                    var personAliasId = new PersonAliasService( txContext ).GetPrimaryAliasId( person.Id );
                                    rockTx.AuthorizedPersonAliasId = personAliasId.Value;

                                    var financialPersonBankAccount = new FinancialPersonBankAccount();
                                    financialPersonBankAccount.PersonAliasId = personAliasId.Value;
                                    financialPersonBankAccount.AccountNumberSecured = accountNumberSecured;
                                    financialPersonBankAccount.AccountNumberMasked = donation.AccountNumber.Masked();

                                    var financialPersonBankAccountService = new FinancialPersonBankAccountService( txContext );
                                    financialPersonBankAccountService.Add( financialPersonBankAccount );

                                    status = ProcessStatus.Created;
                                }

                                var account = new FinancialAccountService( txContext ).Queryable().Where( x => x.GlCode == batch.AccountingSystemCode ).FirstOrDefault();

                                var txnDetail = new FinancialTransactionDetail();
                                txnDetail.AccountId = account.Id;
                                txnDetail.Amount = donation.Amount;
                                rockTx.TransactionDetails.Add( txnDetail );

                                new FinancialTransactionService( txContext ).Add( rockTx );
                                txContext.SaveChanges();

                                updateProgress();
                            }

                            switch ( status )
                            {
                                case ProcessStatus.Matched:
                                    matchCount++;
                                    break;
                                case ProcessStatus.Created:
                                    createdCount++;
                                    break;
                                case ProcessStatus.Error:
                                    errorCount++;
                                    break;
                            }
                        }

                        // Update status from Pending now that all transactions are added
                        batch.Status = BatchStatus.Open;
                        rockContext.SaveChanges();

                        lockboxBatch.Batch = batch;

                        // Output Batch Status
                        string batchLink = string.Format( "<a href='/page/163?batchId={0}'>{1}</a>", batch.Id, batch.Name );
                        int totalTransactions = matchCount + createdCount;
                        string summaryformat = totalTransactions == 1 ?
                            "<li>{0} transaction of {1} was added to {2}.</li>" :
                            "<li>{0} transactions totaling {1} were added to {2}.</li>";
                        sb.AppendFormat( summaryformat, totalTransactions.ToString( "N0" ), lockboxBatch.Total.FormatAsCurrency(), batchLink );
                        if ( matchCount > 0 )
                            sb.AppendFormat( "<li>{0:N0} {1} matched to an existing person.</li>", matchCount, ( matchCount == 1 ? "transaction was" : "transactions were" ) );
                        if ( createdCount > 0 )
                            sb.AppendFormat( "<li>{0:N0} {1} added to a new person.</li>", createdCount, ( createdCount == 1 ? "transaction was" : "transactions were" ) );
                        if ( errorCount > 0 )
                        {
                            sb.AppendFormat( "<li>{0:N0} {1} NOT imported due to error during import (see errors below).</li>", errorCount,
                                ( errorCount == 1 ? "transaction was" : "transactions were" ) );
                        }
                    }
                }
            }

            // update success message to indicate the txns that were updated
            //sb.AppendFormat("<li>{0:N0} {1} processed.</li>", total, "transaction".PluralizeIf(total > 1));
            //sb.AppendFormat("<li>{0:N0} {1} matched to an existing person.</li>", matchCount,
            //    (matchCount == 1 ? "transaction was" : "transactions were"));
            //sb.AppendFormat("<li>{0:N0} {1} added to a new person.</li>", createdCount,
            //    (createdCount == 1 ? "transaction was" : "transactions were"));
            //if (errorCount > 0)
            //{
            //    sb.AppendFormat("<li>{0:N0} {1} NOT imported due to error during import (see errors below).</li>", errorCount,
            //        (errorCount == 1 ? "transaction was" : "transactions were"));
            //}

            //using (var rockContext = new RockContext())
            //{
            //    var batch = new FinancialBatchService(rockContext).Get(batchId.Value);
            //    if (batch != null)
            //    {
            //        // update batch control amount
            //        batch.ControlAmount += _totalAmount;
            //        rockContext.SaveChanges();

            //        // Add link to batch
            //        var qryParam = new Dictionary<string, string>();
            //        qryParam.Add("batchId", batchId.ToString());
            //        string batchUrl = LinkedPageUrl("BatchDetailPage", qryParam);
            //        string batchLink = string.Format("<a href='{0}'>{1}</a>", batchUrl, batch.Name);

            //        int totalTransactions = matchCount + createdCount;
            //        string summaryformat = totalTransactions == 1 ?
            //            "<li>{0} transaction of {1} was added to the {2} batch.</li>" :
            //            "<li>{0} transactions totaling {1} were added to the {2} batch</li>";
            //        sb.AppendFormat(summaryformat, totalTransactions.ToString("N0"), _totalAmount.FormatAsCurrency(), batchLink);
            //    }
            //}

            nbSuccess.Visible = true;
            nbSuccess.Text = string.Format( "<ul>{0}</ul>", sb.ToString() );

            // Display any errors that occurred
            if ( allErrorMessages.Any() )
            {
                StringBuilder sbErrors = new StringBuilder();
                foreach ( var errorMsg in allErrorMessages )
                {
                    sbErrors.AppendFormat( "<li>{0}</li>", errorMsg );
                }

                nbErrors.Text = string.Format( "<ul>{0}</ul>", sbErrors.ToString() );
                nbErrors.Visible = true;
            }
            else
            {
                nbErrors.Visible = false;
            }

            ShowResults();

        }

        protected void updateProgress()
        {
            _progress++;
            var percentage = ( _progress * 100 ) / ( _total * 2 );
            _hubContext.Clients.All.receiveNotification( signalREventName, percentage.ToString( "N1" ) );
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
        /// Handles the Click event of the btnImportAnother control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnImportAnother_Click( object sender, EventArgs e )
        {
            fuTellerFile.BinaryFileId = null;
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
        }

        /// <summary>
        /// Shows the results.
        /// </summary>
        private void ShowResults()
        {
            pnlEntry.Visible = false;
            pnlConfirm.Visible = false;
            pnlResults.Visible = true;
        }

        /// <summary>
        /// Gets the child element value.
        /// </summary>
        /// <param name="giftElement">The gift element.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private string GetChildElementValue( XElement giftElement, string propertyName )
        {
            var propElement = giftElement.Element( propertyName );
            if ( propElement != null )
            {
                return propElement.Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="binaryfileTypeId">The binaryfile type identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="base64String">The base64 string.</param>
        /// <returns></returns>
        private void SaveImage( FinancialTransaction txn, string base64String, int binaryfileTypeId, string fileName )
        {
            if ( !string.IsNullOrWhiteSpace( base64String ) )
            {
                using ( var rockContext = new RockContext() )
                {
                    BinaryFile binaryFile = new BinaryFile();
                    binaryFile.Guid = Guid.NewGuid();
                    binaryFile.IsTemporary = true;
                    binaryFile.BinaryFileTypeId = binaryfileTypeId;
                    binaryFile.MimeType = "image/tiff";
                    binaryFile.FileName = fileName;
                    binaryFile.ContentStream = new MemoryStream( Convert.FromBase64String( base64String ) );

                    var binaryFileService = new BinaryFileService( rockContext );
                    binaryFileService.Add( binaryFile );
                    rockContext.SaveChanges();

                    var transactionImage = new FinancialTransactionImage();
                    transactionImage.BinaryFileId = binaryFile.Id;
                    txn.Images.Add( transactionImage );
                }
            }
        }

        /// <summary>
        /// Decrypts the account information.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string DecryptAccountInformation( string value )
        {
            int i;
            string str = "2FD3C881-3324-4308-BAA0";
            string str1 = "9CD98A4E-5B73-4C4A-997A";
            byte[] num = new byte[32];
            byte[] numArray = new byte[32];
            for ( i = 0; i < str.Length; i++ )
            {
                num[i] = Convert.ToByte( str[i] );
            }
            for ( i = 0; i < str1.Length; i++ )
            {
                numArray[i] = Convert.ToByte( str1[i] );
            }
            RijndaelManaged rijndaelManaged = new RijndaelManaged()
            {
                BlockSize = 256,
                KeySize = 256
            };
            MemoryStream memoryStream = new MemoryStream( Convert.FromBase64String( value ) );
            ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor( num, numArray );
            memoryStream.Position = ( long ) 0;
            return ( new StreamReader( new CryptoStream( memoryStream, cryptoTransform, CryptoStreamMode.Read ) ) ).ReadToEnd();
        }

        /// <summary>
        /// Determines whether the binary file has valid XML.
        /// </summary>
        /// <returns></returns>
        private bool IsXmlValid()
        {
            try
            {
                var xml = GetXml();
                if ( xml == null )
                {
                    ShowError( "Invalid Import File", "Could not read XML file." );
                    return false;
                }

                if ( xml.Root.Name != "Transactions" )
                {
                    ShowError( "Invalid Import File", "The import file does not appear to be a valid teller file." );
                    return false;
                }

                if ( !xml.Root.Descendants().Where( n => n.Name == "Gift" ).Any() )
                {
                    ShowError( "Warning", "The import file does not appear to contain any transactions." );
                    return false;
                }

                return true;
            }
            catch ( Exception ex )
            {
                ShowError( "Invalid Import File", ex.Message );
                return false;
            }
        }

        /// <summary>
        /// Gets the XML.
        /// </summary>
        /// <returns></returns>
        private XDocument GetXml()
        {
            XDocument xml = null;

            if ( _binaryFileId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var binaryFile = new BinaryFileService( rockContext ).Get( _binaryFileId.Value );
                    if ( binaryFile != null )
                    {
                        using ( var stream = binaryFile.ContentStream )
                        {
                            xml = XDocument.Load( stream );
                        }
                    }
                }
            }

            return xml;
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

        /// <summary>
        /// Enumeration for tracking transction status
        /// </summary>
        private enum ProcessStatus
        {
            Matched = 0,
            Created = 1,
            Error = 2
        }
    }

}
