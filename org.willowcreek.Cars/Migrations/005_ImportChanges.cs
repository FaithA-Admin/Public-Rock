using Rock.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.willowcreek.Cars.Migrations
{
    [MigrationNumber( 5, "1.5.0" )]
    public class ImportChanges : Migration
    {
        public override void Up()
        {
            Sql( @"
    ALTER TABLE [_org_willowcreek_Cars_Vehicle] ALTER COLUMN [DonorPersonAliasId] [int] NULL;
    ALTER TABLE [_org_willowcreek_Cars_Vehicle] ALTER COLUMN [TStockNumber] [int] NULL;
" );

        }

        public override void Down()
        {
        }

    }
}
