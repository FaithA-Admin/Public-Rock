using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using System.Data.Entity;
using System.IO;

namespace org.willowcreek.Financial
{
    public static class PledgeStatusLetters
    {
        /// <summary>
        /// Generate a contribution statement for an individual
        /// </summary>
        /// <param name="binaryFileType">The File Type to save the statement as</param>
        /// <param name="year">The year to report on</param>
        /// <param name="person">The person whose statement to generate</param>
        /// <returns></returns>
        public static BinaryFile GenerateOne( int binaryFileTypeId, bool convertToPdf, int accountId, DateTime endDate, string title, string addressLine, Person person )
        {
            return GenerateOne( binaryFileTypeId, convertToPdf, accountId, endDate, title, addressLine, person.GivingId );
        }

        /// <summary>
        /// Generate a contribution statement for an individual
        /// </summary>
        /// <param name="binaryFileType">The File Type to save the statement as</param>
        /// <param name="year">The year to report on</param>
        /// <param name="givingId">The GivingID of the group whose statement to generate</param>
        /// <returns></returns>
        public static BinaryFile GenerateOne( int binaryFileTypeId, bool convertToPdf, int accountId, DateTime endDate, string title, string addressLine, string givingId )
        {
            using ( var rockContext = new RockContext() )
            {
                // Get the data for a single person's statement
                var mergeObjects = GetStatementData( accountId, endDate, true, givingId );

                // Generate the statement file and save to the database
                var mergeTemplate = GetMergeTemplate( rockContext );
                var mergeTemplateType = mergeTemplate.GetMergeTemplateType();
                var mergeFields = GetGlobalMergeFields( endDate, title, addressLine );
                var binaryFile = mergeTemplateType.CreateDocument( mergeTemplate, mergeObjects.Select( x => x.Obj as object ).ToList(), mergeFields );

                var fileName = $"{title} - {mergeObjects[0].Name}";
                if ( convertToPdf )
                {
                    binaryFile = Files.ConvertWordToPDF( binaryFile, fileName, binaryFileTypeId );
                }
                else
                {
                    // Get the BinaryFile in the current context
                    var binaryFileService = new BinaryFileService( rockContext );
                    binaryFile = binaryFileService.Get( binaryFile.Id );

                    // Set the necessary properties
                    binaryFile.BinaryFileTypeId = binaryFileTypeId;
                    binaryFile.FileName = $"{fileName}.docx";
                    binaryFile.IsTemporary = true;
                    rockContext.SaveChanges();
                }

                return binaryFile;
            }
        }

        /// <summary>
        /// Generate all contribution statements for a single year
        /// </summary>
        /// <param name="binaryFileType">The File Type to save the statements as</param>
        /// <param name="year">The year to report on</param>
        /// <param name="chapterSize">The maximum number of statements per file</param>
        /// <returns></returns>
        public static void GenerateAll( int binaryFileTypeId, int accountId, DateTime endDate, bool includeContributorsWithNoPledge, string title, string addressLine, int chapterSize )
        {
            using ( var rockContext = new RockContext() )
            {
                var mergeTemplate = GetMergeTemplate( rockContext );
                var mergeTemplateType = mergeTemplate.GetMergeTemplateType();

                var mergeObjects = GetStatementData( accountId, endDate, includeContributorsWithNoPledge );

                var internationalStatements = mergeObjects.Where( x => x.Country != string.Empty ).ToList();
                var largeStatements = mergeObjects.Where( x => x.Pages > 2 && x.Country == string.Empty ).ToList();
                var twoPageStatements = mergeObjects.Where( x => x.Pages == 2 && x.Country == string.Empty ).OrderByDescending( x => x.Transactions ).ToList();
                var onePageStatements = mergeObjects.Where( x => x.Pages == 1 && x.Country == string.Empty ).OrderByDescending( x => x.Transactions ).ToList();

                var twoPageStatementFiles = ( twoPageStatements.Count() / chapterSize ) + ( twoPageStatements.Count() % chapterSize > 0 ? 1 : 0 );
                var onePageStatementFiles = ( onePageStatements.Count() / chapterSize ) + ( onePageStatements.Count() % chapterSize > 0 ? 1 : 0 );

                var fileCount = ( internationalStatements.Any() ? 1 : 0 )
                                + largeStatements.Count()
                                + twoPageStatementFiles
                                + onePageStatementFiles;
                var fileCounter = 0;

                var mergeFields = GetGlobalMergeFields( endDate, title, addressLine );
                var binaryFileType = new BinaryFileTypeService( rockContext ).Get( binaryFileTypeId );
                var binaryFileService = new BinaryFileService( rockContext );
                var templateBinaryFile = binaryFileService.Get( mergeTemplate.TemplateBinaryFileId );
                if ( internationalStatements.Any() )
                {
                    GenerateStatements( title, rockContext, binaryFileService, binaryFileType, mergeFields, templateBinaryFile, internationalStatements, fileCount, ref fileCounter, "International" );
                }

                foreach ( var largeStatement in largeStatements )
                {
                    GenerateStatements( title, rockContext, binaryFileService, binaryFileType, mergeFields, templateBinaryFile, new List<MergeObject> { largeStatement }, fileCount, ref fileCounter, largeStatement.Name );
                }

                for ( int i = 0; i < twoPageStatementFiles; i++ )
                {
                    GenerateStatements( title, rockContext, binaryFileService, binaryFileType, mergeFields, templateBinaryFile, twoPageStatements.Skip( i * chapterSize ).Take( chapterSize ).ToList(), fileCount, ref fileCounter, "Double-Sided" );
                }

                for ( int i = 0; i < onePageStatementFiles; i++ )
                {
                    GenerateStatements( title, rockContext, binaryFileService, binaryFileType, mergeFields, templateBinaryFile, onePageStatements.Skip( i * chapterSize ).Take( chapterSize ).ToList(), fileCount, ref fileCounter, "Single-Sided" );
                }
            }
        }

        /// <summary>
        /// Get the data for all the requested contribution statements
        /// </summary>
        /// <param name="year">The year to pull the statements for</param>
        /// <returns></returns>
        private static List<MergeObject> GetStatementData( int accountId, DateTime endDate, bool includeContributorsWithNoPledge, string givingId = null )
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add( "AccountID", accountId );
            parameters.Add( "EndDate", endDate );
            parameters.Add( "IncludeContributorsWithNoPledge", includeContributorsWithNoPledge );

            if ( givingId != null)
            {
                parameters.Add( "GivingID", givingId );
            }
            var contributors = DbService.GetDataSet( "wcRpt_PledgeStatusLetter", CommandType.StoredProcedure, parameters );

            // Create the base merge object plus all the additional fields necessary to separate them into groups
            var statementData = contributors.Tables[0].AsEnumerable()
                        .Select( row => new MergeObject
                        {
                            GivingID = row.Field<string>( "GivingID" ),
                            Name = row.Field<string>( "Name" ),
                            Pages = row.Field<int>( "Pages" ),
                            Transactions = row.Field<int>("Transactions"),
                            Country = row.Field<string>( "Country" ),
                            Obj = contributors.Tables[0].Columns.Cast<DataColumn>().ToDictionary( col => col.ColumnName, col => row[col] )
                        } ).ToList();

            // Get all the header line items and index them by GivingID
            var headerLines = contributors.Tables[1].AsEnumerable()
                        .Select( row => new HeaderLine
                        {
                            GivingID = row.Field<string>( "GivingID" ),
                            Amount = row.Field<string>( "Amount" ),
                            Year = row.Field<int>( "Year" )
                        } ).ToLookup( a => a.GivingID );

            // Get all the line items and index them by GivingID
            var lineItems = contributors.Tables[2].AsEnumerable()
                        .Select( row => new StatementLine
                        {
                            GivingID = row.Field<string>( "GivingID" ),
                            Date = row.Field<DateTime>( "TransactionDateTime" ).ToString( "MM/dd/yyyy" ),
                            Num = row.Field<string>( "CheckDesc" ),
                            Amt = row.Field<string>( "Amount" ),
                            Acct = row.Field<string>( "Description" ),
                            Desc = row.Field<string>( "Desc2" ),
                            Row = row.Field<long>( "Row" ),
                            Even = row.Field<string>( "Even" )
                        } ).ToLookup( a => a.GivingID );

            // Add the ordered line item list to each merge object
            foreach ( var data in statementData )
            {
                data.Obj.Add( "Header", headerLines[data.GivingID].OrderBy( x => x.Year ).ToList() );
                data.Obj.Add( "TransactionDetails", lineItems[data.GivingID].OrderBy( x => x.Row ).ToList() );
            }

            return statementData;
        }

        private static Dictionary<string, object> GetGlobalMergeFields( DateTime endDate, string title, string addressLine )
        {
            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, null );
            mergeFields.Add( "Title", title );
            mergeFields.Add( "AddressLine", addressLine );
            mergeFields.Add( "EndDate", endDate );
            return mergeFields;
        }

        /// <summary>
        /// Gets the Contribution Statement Merge Template defined in the Global Attributes
        /// </summary>
        /// <returns></returns>
        public static MergeTemplate GetMergeTemplate( RockContext rockContext )
        {
            var globalAttributes = Rock.Web.Cache.GlobalAttributesCache.Read();
            var templateGuid = globalAttributes.GetValue( "PledgeStatusLetterTemplate" ).AsGuid();
            if (templateGuid == Guid.Empty)
            {
                throw new Exception( "The PledgeStatusLetterTemplate global attribute is not defined." );
            }

            return new MergeTemplateService( rockContext ).Get( templateGuid );
        }

        private static void GenerateStatements(
                string title,
                RockContext rockContext,
                BinaryFileService binaryFileService,
                BinaryFileType binaryFileType,
                Dictionary<string, object> mergeFields,
                BinaryFile templateBinaryFile,
                List<MergeObject> mergeObjectsList,
                int fileCount,
                ref int fileCounter,
                string fileSuffix )
        {

            fileCounter++;
            var fileName = $"{title} Part {fileCounter:000} of {fileCount:000} - {fileSuffix}";

            var mergeTemplate = GetMergeTemplate( rockContext );
            var mergeTemplateType = mergeTemplate.GetMergeTemplateType();
            var binaryFile = mergeTemplateType.CreateDocument( mergeTemplate, mergeObjectsList.Select( x => x.Obj as object ).ToList(), mergeFields );

            Files.SaveBinaryFileToDisk( binaryFile, fileName, Path.GetExtension( templateBinaryFile.FileName ) );

            // Delete the file that's stored in the database
            binaryFile = binaryFileService.Get( binaryFile.Id );
            binaryFileService.Delete( binaryFile );
            rockContext.SaveChanges();
        }



        [DotLiquid.LiquidType( "GivingId", "Date", "Num", "Amt", "Acct", "Desc", "Row", "Even" )]
        private class StatementLine
        {
            public string GivingID { get; set; }
            public string Date { get; set; } // Transaction Date
            public string Num { get; set; } // Check/Desc field
            public string Amt { get; set; } // Gift Amount
            public string Acct { get; set; } // Account Description field
            public string Desc { get; set; } // Description of the non-cash gift, if any
            public long Row { get; set; } // Row Number
            public string Even { get; set; } // Displays Even when the row number is even
        }

        private class MergeObject
        {
            public string GivingID;
            public string Name;
            public int Pages;
            public int Transactions;
            public string Country;
            public Dictionary<string, object> Obj;
        }
        [DotLiquid.LiquidType( "GivingId", "Year", "Amount" )]
        private class HeaderLine
        {
            public string GivingID { get; set; }
            public int Year { get; set; }
            public string Amount { get; set; }
        }
    }
}
