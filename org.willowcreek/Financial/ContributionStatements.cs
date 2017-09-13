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
    public static class ContributionStatements
    {
        /// <summary>
        /// Generate a contribution statement for an individual
        /// </summary>
        /// <param name="binaryFileType">The File Type to save the statement as</param>
        /// <param name="year">The year to report on</param>
        /// <param name="person">The person whose statement to generate</param>
        /// <returns></returns>
        public static BinaryFile GenerateOne( int binaryFileTypeId, bool convertToPdf, int year, Person person )
        {
            using ( var rockContext = new RockContext() )
            {
                // Get the data for a single person's statement
                var mergeObjects = GetStatementData( year, new[] { person.GivingId } );

                // Generate the statement file and save to the database
                var mergeTemplate = GetMergeTemplate( rockContext );
                var mergeTemplateType = mergeTemplate.GetMergeTemplateType();
                var mergeFields = GetGlobalMergeFields( year );
                var binaryFile = mergeTemplateType.CreateDocument( mergeTemplate, mergeObjects.Select( x => x.Obj as object ).ToList(), mergeFields );

                var fileName = $"{year} Contribution Statement - {mergeObjects[0].Name}";
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
        public static void GenerateAll( int binaryFileTypeId, int year, int chapterSize )
        {
            using ( var rockContext = new RockContext() )
            {
                var mergeObjects = GetStatementData( year );

                var internationalStatements = mergeObjects.Where( x => x.Country != string.Empty ).ToList();
                var largeStatements = mergeObjects.Where( x => x.Pages > 2 && x.Country == string.Empty ).ToList();
                var twoPageStatements = mergeObjects.Where( x => x.Pages == 2 && x.Country == string.Empty ).OrderBy( x => x.PostalCode ).ToList();
                var onePageStatements = mergeObjects.Where( x => x.Pages == 1 && x.Country == string.Empty ).OrderBy( x => x.PostalCode ).ToList();

                var twoPageStatementFiles = ( twoPageStatements.Count() / chapterSize ) + ( twoPageStatements.Count() % chapterSize > 0 ? 1 : 0 );
                var onePageStatementFiles = ( onePageStatements.Count() / chapterSize ) + ( onePageStatements.Count() % chapterSize > 0 ? 1 : 0 );

                var fileCount = ( internationalStatements.Any() ? 1 : 0 )
                                + largeStatements.Count()
                                + twoPageStatementFiles
                                + onePageStatementFiles;
                var fileCounter = 0;

                var mergeTemplate = GetMergeTemplate( rockContext );
                var mergeTemplateType = mergeTemplate.GetMergeTemplateType();
                var mergeFields = GetGlobalMergeFields( year );
                var binaryFileType = new BinaryFileTypeService( rockContext ).Get( binaryFileTypeId );
                var binaryFileService = new BinaryFileService( rockContext );
                var templateBinaryFile = binaryFileService.Get( mergeTemplate.TemplateBinaryFileId );
                if ( internationalStatements.Any() )
                {
                    GenerateStatements( year, rockContext, binaryFileService, binaryFileType, mergeFields, templateBinaryFile, internationalStatements, fileCount, ref fileCounter, "International" );
                }

                foreach ( var largeStatement in largeStatements )
                {
                    GenerateStatements( year, rockContext, binaryFileService, binaryFileType, mergeFields, templateBinaryFile, new List<MergeObject> { largeStatement }, fileCount, ref fileCounter, largeStatement.Name );
                }

                for ( int i = 0; i < twoPageStatementFiles; i++ )
                {
                    GenerateStatements( year, rockContext, binaryFileService, binaryFileType, mergeFields, templateBinaryFile, twoPageStatements.Skip( i * chapterSize ).Take( chapterSize ).ToList(), fileCount, ref fileCounter, "Double-Sided" );
                }

                for ( int i = 0; i < onePageStatementFiles; i++ )
                {
                    GenerateStatements( year, rockContext, binaryFileService, binaryFileType, mergeFields, templateBinaryFile, onePageStatements.Skip( i * chapterSize ).Take( chapterSize ).ToList(), fileCount, ref fileCounter, "Single-Sided" );
                }
            }
        }

        public static IEnumerable<int> StatementsAvailable( Person person, string accountGuids )
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add( "PersonID", person.Id );
            if (!string.IsNullOrWhiteSpace(accountGuids))
            {
                parameters.Add( "AccountGuids", accountGuids );
            }
            var years = DbService.GetDataTable( "wcRpt_ContributionStatementsAvailable", CommandType.StoredProcedure, parameters ).AsEnumerable().Select( x => x.Field<int>( "Year" ) ).OrderByDescending(x => x);
            return years;
        }

        /// <summary>
        /// Get the data for all the requested contribution statements
        /// </summary>
        /// <param name="year">The year to pull the statements for</param>
        /// <param name="givingIds">(Optional) A list of GivingIDs to limit the query to. Without this parameter, data for all statements will be returned</param>
        /// <returns></returns>
        private static List<MergeObject> GetStatementData( int year, IEnumerable<string> givingIds = null )
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add( "Year", year );

            // If exactly one GivingID was passed in, only run the query for that person
            if ( givingIds != null && givingIds.Count() == 1 )
            {
                parameters.Add( "GivingID", givingIds.Single() );
            }
            var contributors = DbService.GetDataSet( "wcRpt_ContributorsByYear", CommandType.StoredProcedure, parameters );

            // Create the base merge object plus all the additional fields necessary to separate them into groups
            var statementData = contributors.Tables[0].AsEnumerable()
                        .Select( row => new MergeObject
                        {
                            GivingID = row.Field<string>( "GivingID" ),
                            Name = row.Field<string>( "Name" ),
                            Pages = row.Field<int>( "Pages" ),
                            PostalCode = row.Field<string>( "PostalCode" ),
                            Country = row.Field<string>( "Country" ),
                            Obj = contributors.Tables[0].Columns.Cast<DataColumn>().ToDictionary( col => col.ColumnName, col => row[col] )
                        } ).ToList();

            // Get all the line items and index them by GivingID
            var lineItems = contributors.Tables[1].AsEnumerable()
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

            // If more than one GivingID was passed in, limit the set of statement data to those giving groups
            if ( givingIds != null && givingIds.Count() > 1 )
            {
                statementData = statementData.Where( x => givingIds.Contains( x.GivingID ) ).ToList();
            }

            // Add the ordered line item list to each merge object
            foreach ( var data in statementData )
            {
                data.Obj.Add( "TransactionDetails", lineItems[data.GivingID].OrderBy( x => x.Row ).ToList() );
            }

            return statementData;
        }

        private static Dictionary<string, object> GetGlobalMergeFields( int year )
        {
            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null, null );
            mergeFields.Add( "Year", year );
            var endOfYear = new DateTime( year, 12, 31 );
            mergeFields.Add( "EndDate", DateTime.Today < endOfYear ? DateTime.Today : endOfYear );
            return mergeFields;
        }

        /// <summary>
        /// Gets the Contribution Statement Merge Template defined in the Global Attributes
        /// </summary>
        /// <returns></returns>
        public static MergeTemplate GetMergeTemplate( RockContext rockContext )
        {
            var globalAttributes = Rock.Web.Cache.GlobalAttributesCache.Read();
            var templateGuid = globalAttributes.GetValue( "ContributionStatementTemplate" ).AsGuid();
            return new MergeTemplateService( rockContext ).Get( templateGuid );
        }

        private static void GenerateStatements(
                int year,
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
            var fileName = $"{year} Contribution Statements Part {fileCounter:000} of {fileCount:000} - {fileSuffix}";

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
            public string PostalCode;
            public string Country;
            public Dictionary<string, object> Obj;
        }
    }
}
