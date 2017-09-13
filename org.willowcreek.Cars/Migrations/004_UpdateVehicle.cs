using Rock.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.willowcreek.Cars.Migrations
{
    [MigrationNumber( 4, "1.5.0" )]
    public class UpcateVehicle : Migration
    {
        public override void Up()
        {
            Sql( @"
    ALTER TABLE [_org_willowcreek_Cars_Vehicle] ADD 
        [IsDropOff] bit NULL,
        [EstimatedValue] [decimal](18, 2) NULL;
" );

        }

        public override void Down()
        {
        }

    }
}
