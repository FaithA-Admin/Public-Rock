using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Rock.Model;
using Rock.Data;
using System.Data.SqlClient;
using Rock.Web.Cache;
using Rock.Security;
using Rock;
using System.IO;
using Rock.Web.UI.Controls;

namespace org.willowcreek.Financial
{
    public static class Teller
    {
        public class ImportBatch
        {
            public FinancialBatch Batch;
            public IEnumerable<ImportTransaction> Transactions;
        }

        public class ImportTransaction
        {
            public DocType DocType;
            public DateTime GiftDate;
            public string ImageFrontLocation;
            public string ImageBackLocation;
            public string RoutingNo;
            public string CheckAcctNo;
            public string CheckNo;
            public int SequenceNo;
            public IEnumerable<ImportPurpose> Purposes;
        }

        public class ImportPurpose
        {
            public string GLAccount;
            public decimal Amount;
        }

        public enum DocType
        {
            Unknown = 0,
            Check = 1,
            Document = 2,
            Form = 3,
            Image = 4,
            BatchSeperator = 5,
            DepositTicket = 6,
            BatchHeader = 7,
            BatchTrailer = 8,
            EndOfRun = 9
        }

        private static int _transactionTypeContributionId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION.AsGuid() ).Id;
        private static int _currencyTypeCheck = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.CURRENCY_TYPE_CHECK.AsGuid() ).Id;

        public class ImportBatchesResult
        {
            public int Batches = 0;
            public int Transactions = 0;
        }
        public static ImportBatchesResult ImportBatches( DateTime startDate, string serverName, string databaseNames, string imagesPhysicalPath, string imagesServerPath, bool importBackOfCheck,
            string batchNamePrefix, string source,
            IEnumerable<Guid> batchIds = null,
            Action<decimal> updateProgress = null )
        {
            var batchCount = 0;
            var transactionCount = 0;

            var databaseBatches = new List<IEnumerable<ImportBatch>>();
            decimal totalTransactions = 0;
            foreach ( var database in databaseNames.Split( ',' ) )
            {
                var batches = GetBatches( serverName, database.ToString().Trim(), imagesPhysicalPath, imagesServerPath, startDate, batchIds );
                totalTransactions += batches.Sum( x => x.Transactions.Count() - 1 ); // Don't count the deposit ticket
                databaseBatches.Add( batches );
            }
            
            foreach ( var batch in databaseBatches.SelectMany( b => b ).OrderBy( b => b.Batch.BatchStartDateTime ) )
            {
                // Create the batch
                using ( var rockContext = new RockContext() )
                {
                    var batchService = new FinancialBatchService( rockContext );

                    // If there is only one account code used on all the transactions in this batch, populate it in the Accounting Code field
                    var accounts = batch.Transactions.SelectMany( x => x.Purposes.Select( p => p.GLAccount ) ).Distinct();
                    if ( accounts.Count() == 1 )
                    {
                        batch.Batch.AccountingSystemCode = accounts.SingleOrDefault().ToString();

                        // Get the Campus from the chosen Account
                        var financialAccountService = new FinancialAccountService( rockContext );
                        batch.Batch.CampusId = financialAccountService.Queryable().Where( a => a.GlCode == batch.Batch.AccountingSystemCode && a.IsActive == true ).Select( a => a.CampusId ).Distinct().SingleOrDefault();
                    }

                    // Get the control amount from the deposit ticket transaction
                    batch.Batch.ControlAmount = batch.Transactions.Where( t => t.DocType == Teller.DocType.DepositTicket ).Select( t => t.Purposes.First().Amount ).First();

                    batchService.Add( batch.Batch );
                    rockContext.SaveChanges();
                    batch.Batch.Name = $"{batchNamePrefix} Batch {batch.Batch.Id}".Trim();
                    rockContext.SaveChanges();
                    batchCount++;

                    foreach ( var tx in batch.Transactions.Where( t => t.DocType == Teller.DocType.Check ) )
                    {
                        using ( var txContext = new RockContext() )
                        {
                            // Create the transaction
                            var rockTx = new FinancialTransaction
                            {
                                BatchId = batch.Batch.Id,
                                TransactionTypeValueId = _transactionTypeContributionId,
                                TransactionDateTime = tx.GiftDate,
                                Summary = $"{batch.Batch.Name}:{tx.SequenceNo}",
                                TransactionCode = tx.CheckNo,
                                SourceTypeValueId = DefinedValueCache.Read( source ).Id,
                                FinancialPaymentDetail = new FinancialPaymentDetail
                                {
                                    CurrencyTypeValueId = _currencyTypeCheck
                                }
                            };

                            if ( !string.IsNullOrWhiteSpace( tx.RoutingNo ) && !string.IsNullOrWhiteSpace( tx.CheckAcctNo ) )
                            {
                                string checkMicrHashed = FinancialPersonBankAccount.EncodeAccountNumber( tx.RoutingNo, tx.CheckAcctNo );

                                if ( !string.IsNullOrWhiteSpace( checkMicrHashed ) )
                                {
                                    var matchedPersonIds = new FinancialPersonBankAccountService( txContext )
                                        .Queryable()
                                        .Where( a => a.AccountNumberSecured == checkMicrHashed )
                                        .Select( a => a.PersonAlias.PersonId )
                                        .Distinct()
                                        .ToList();

                                    if ( matchedPersonIds.Count() == 1 )
                                    {
                                        rockTx.AuthorizedPersonAliasId = new PersonAliasService( txContext ).GetPrimaryAliasId( matchedPersonIds.Single() );
                                    }
                                }

                                rockTx.MICRStatus = MICRStatus.Success;
                                rockTx.CheckMicrParts = Encryption.EncryptString( $"{tx.RoutingNo}_{tx.CheckAcctNo}_{tx.CheckNo}" );
                            }

                            // Add check images
                            var binaryFileTypeId = GetContributionImageTypeID();
                            if ( binaryFileTypeId.HasValue )
                            {
                                AddImage( rockTx, binaryFileTypeId.Value, tx.ImageFrontLocation, txContext, 1 );
                                if ( importBackOfCheck )
                                {
                                    AddImage( rockTx, binaryFileTypeId.Value, tx.ImageBackLocation, txContext, 2 );
                                }
                            }

                            // Distribute funds to accounts
                            foreach ( var purpose in tx.Purposes )
                            {
                                FinancialTransactionDetail txnDetail = null;

                                var account = new FinancialAccountService( txContext ).Queryable().Where( x => x.GlCode == purpose.GLAccount ).FirstOrDefault();

                                // TODO: If account is null, create one

                                if ( account != null )
                                {
                                    txnDetail = new FinancialTransactionDetail();
                                    txnDetail.AccountId = account.Id;
                                    txnDetail.Amount = purpose.Amount;
                                    rockTx.TransactionDetails.Add( txnDetail );
                                }
                            }

                            new FinancialTransactionService( txContext ).Add( rockTx );
                            transactionCount++;
                            updateProgress?.Invoke( Convert.ToDecimal( transactionCount ) * 100 / totalTransactions );

                            txContext.SaveChanges();
                        }
                    }

                    batch.Batch.Status = BatchStatus.Open;
                    rockContext.SaveChanges();
                }
            }

            return new ImportBatchesResult { Batches = batchCount, Transactions = transactionCount };
        }

        public static IEnumerable<ImportBatch> GetBatches( string serverName, string databaseName, string imagesPhyisicalPath, string imagesServerPath, DateTime startDate, IEnumerable<Guid> batchIds = null )
        {
            // OPENQUERY is used here because joining to FinancialBatch directly is very slow when they are on different servers
            // GL_Acct is custom to Willow
            string query = $@"select * from openquery({serverName}, 'select GiftDate = B.ActualDate
                    , ImageFrontLocation = REPLACE(IMGF.ImagePath, ''{imagesPhyisicalPath}'', ''{imagesServerPath}'')
                    , ImageBackLocation = REPLACE(IMGB.ImagePath, ''{imagesPhyisicalPath}'', ''{imagesServerPath}'')
                    , RoutingNo = RTRIM(LTRIM(RoutingNo))
                    , CheckAcctNo = RTRIM(LTRIM(CheckAcctNo))
                    , CheckNo = RTRIM(LTRIM(CheckNo))
                    , BatchNo = CAST(LTRIM(RTRIM(B.BatchNo)) AS INT)
                    , SequenceNo = D.SequenceNo - 1
                    , GL_Acct = SUBSTRING(GL_Acct, 5, 6) + '' '' + SUBSTRING(GL_Acct, 1, 4) + SUBSTRING(GL_Acct, 12, 1000) 
                    , AppliedAmount
                    , B.BatchID
                    , D.DocType
                from {databaseName}.dbo.Docs D
                join {databaseName}.dbo.Batch B on B.RunId = D.RunId AND B.BatchId = D.BatchId AND D.Status = 200
                left join {databaseName}.dbo.Images IMGF on IMGF.DocID = D.DocID and IMGF.Flag = ''F''
                left join {databaseName}.dbo.Images IMGB on IMGB.DocID = D.DocID and IMGB.Flag = ''B''
                where B.Deposited = ''T''
                and B.ActualDate >= ''{startDate.Month}/{startDate.Day}/{startDate.Year}''";
            if ( batchIds != null && batchIds.Any() )
            {
                var ids = string.Join( ",", batchIds.Select( batchId => $"''{batchId.ToString()}''" ) );
                query += $" and B.BatchID IN ({ids})";
            }
            // Exclude any batches that have already been imported
            // Exclude any accounts that have the Import Scanned Checks attribute set to No
            query += @"') S
                where not exists (select 1 from FinancialBatch FB where ForeignGuid = S.BatchID)
                and not exists (select 1 from FinancialAccount FA join AttributeValue AV on AV.AttributeId = 29392 and AV.EntityId = FA.Id and ValueAsBoolean = 0 where FA.GlCode = S.GL_Acct)
                order by GiftDate, BatchNo, SequenceNo";

            DataTable data;
            try
            {
                data = DbService.GetDataTable( query, CommandType.Text, null );
            }
            catch ( SqlException ex ) when ( ex.Number == 8180 )
            {
                throw new Exception( $"{serverName}.{databaseName} does not appear to be a valid Shelby Teller database.", ex );
            }

            // Transform the data into object collections
            var batches = data.AsEnumerable()
                .GroupBy( x => new
                {
                    BatchNo = x.Field<int>( "BatchNo" ),
                    GiftDate = x.Field<DateTime>( "GiftDate" ),
                    BatchId = x.Field<string>( "BatchId" )
                } )
                .OrderBy( x => x.Key.GiftDate )
                .ThenBy( x => x.Key.BatchNo )
                .Select( b => new ImportBatch
                {
                    Batch = new FinancialBatch
                    {
                        Guid = Guid.NewGuid(),
                        Name = b.Key.BatchId,
                        Status = BatchStatus.Pending,
                        BatchStartDateTime = b.Key.GiftDate,
                        BatchEndDateTime = b.Key.GiftDate,
                        ForeignGuid = Guid.Parse( b.Key.BatchId ),
                    },
                    Transactions = b.AsEnumerable()
                         .GroupBy( t => new
                         {
                             ImageFrontLocation = t.Field<string>( "ImageFrontLocation" ),
                             ImageBackLocation = t.Field<string>( "ImageBackLocation" ),
                             RoutingNo = t.Field<string>( "RoutingNo" ),
                             CheckAcctNo = t.Field<string>( "CheckAcctNo" ),
                             CheckNo = t.Field<string>( "CheckNo" ),
                             SequenceNo = t.Field<int>( "SequenceNo" ),
                             DocType = t.Field<int>( "DocType" )
                         } )
                         .OrderBy( t => t.Key.SequenceNo )
                         .Select( t => new ImportTransaction
                         {
                             DocType = ( DocType ) t.Key.DocType,
                             GiftDate = b.Key.GiftDate,
                             ImageFrontLocation = t.Key.ImageFrontLocation,
                             ImageBackLocation = t.Key.ImageBackLocation,
                             RoutingNo = t.Key.RoutingNo,
                             CheckAcctNo = t.Key.CheckAcctNo,
                             CheckNo = t.Key.CheckNo,
                             SequenceNo = t.Key.SequenceNo,
                             Purposes = t.Select( p => new ImportPurpose
                             {
                                 GLAccount = p.Field<string>( "GL_Acct" ),
                                 Amount = p.Field<decimal>( "AppliedAmount" )
                             } )
                         } )
                } );

            return batches;
        }

        private static int? GetContributionImageTypeID()
        {
            // Get the binary file type for contribution images
            using ( var rockContext = new RockContext() )
            {
                var binaryFileType = new BinaryFileTypeService( rockContext ).Get( Rock.SystemGuid.BinaryFiletype.CONTRIBUTION_IMAGE.AsGuid() );
                if ( binaryFileType != null )
                {
                    return binaryFileType.Id;
                }
            }
            return null;
        }

        private static void AddImage( FinancialTransaction rockTx, int binaryFileTypeId, string location, RockContext rockContext, int order )
        {
            var binaryFile = new BinaryFile()
            {
                Guid = Guid.NewGuid(),
                IsTemporary = true,
                BinaryFileTypeId = binaryFileTypeId,
                MimeType = "image/tiff",
                FileName = location
            };

            try
            {
                binaryFile.ContentStream = new MemoryStream( File.ReadAllBytes( location ) );
            }
            catch ( DirectoryNotFoundException ex )
            {
                throw new Exception( $"Could not find the images for {rockTx.Summary}. Please confirm that both of your image path settings are correct.", ex );
            }

            var binaryFileService = new BinaryFileService( rockContext );
            binaryFileService.Add( binaryFile );
            rockContext.SaveChanges();

            var transactionImage = new FinancialTransactionImage
            {
                BinaryFileId = binaryFile.Id,
                Order = order
            };
            rockTx.Images.Add( transactionImage );
        }
    }
}
