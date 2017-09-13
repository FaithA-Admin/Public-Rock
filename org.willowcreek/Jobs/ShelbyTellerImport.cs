using Quartz;
using Rock.Data;
using Rock.Attribute;
using System;
using Rock.Web.UI.Controls;
using org.willowcreek.Financial;

namespace org.willowcreek.Jobs
{

    [TextField( "Shelby Teller Database Server", "The Server name where the Shelby Teller databases are stored.", required: true, order: 1, key: "Server" )]
    [TextField( "Shelby Teller Database Names", "The Database name(s) containing the Shelby Teller transactions. If using more than one, separate them by commas, e.g. Teller1,Teller2,Teller3", required: true, order: 2, key: "DatabaseNames" )]
    [TextField( "Images Folder Physical Path", "The location of the Shelby Teller Images folder as stored in the database, e.g. C:\\Program Files\\ShelbyTELLER\\Server\\Images", required: true, order: 3, key: "ImagesPhysicalPath" )]
    [TextField( "Images Folder Server Path", "The shared path to the Shelby Teller Images folder, as accessible by the Rock system, e.g. \\\\ShelbyTeller\\Images", required: true, order: 4, key: "ImagesServerPath" )]
    [DateField( "Start Date", "Import batches from this date forward.", required: true, order: 5, key: "StartDate" )]
    [BooleanField( "Import Back of Check", "Check this box if you would like to import both sides of the check", order: 6, key: "ImportBackOfCheck" )]
    [TextField( "Batch Name Prefix", "Text that should begin the name of each imported batch", required: false, order: 7, key: "BatchNamePrefix" )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.FINANCIAL_SOURCE_TYPE, "Transaction Source", "The source type that should be set on each imported transaction", false, order: 8, key: "Source", defaultValue: Rock.SystemGuid.DefinedValue.FINANCIAL_SOURCE_TYPE_ONSITE_COLLECTION )]
    class ShelbyTellerImport : IJob
    {

        public void Execute( IJobExecutionContext context )
        {
            var dataMap = context.JobDetail.JobDataMap;
            var startDate = DateTime.Parse( dataMap.GetString( "StartDate" ) );
            var serverName = dataMap.GetString( "Server" );
            var databaseNames = dataMap.GetString( "DatabaseNames" );
            var imagesPhysicalPath = dataMap.GetString( "ImagesPhysicalPath" );
            var imagesServerPath = dataMap.GetString( "ImagesServerPath" );
            var batchNamePrefix = dataMap.GetString( "BatchNamePrefix" )?.Trim() ?? string.Empty;
            var source = dataMap.GetString( "Source" );
            var importBackOfCheck = dataMap.GetBoolean( "ImportBackOfCheck" );

            var result = Teller.ImportBatches( startDate, serverName, databaseNames, imagesPhysicalPath, imagesServerPath, importBackOfCheck, batchNamePrefix, source );

            using ( var rockContext = new RockContext() )
            {
                if ( result.Batches > 0 )
                {
                    context.Result = $"Successfully imported {result.Batches} batches containing {result.Transactions} transactions.";
                }
                else
                {
                    context.Result = $"No new batches to import.";
                }
                rockContext.SaveChanges();
            }
        }
    }
}
