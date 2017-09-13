using System;
using System.Linq;
using Rock.Plugin;

namespace org.willowcreek.ProtectionApp.Migrations
{
    [MigrationNumber(4, "1.2")]
    public class CreateReference : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            //build the pages
            BuildPages();
            #region Reference Check Workflow
            #endregion            
        }
 
        /// <summary>
        /// Builds the pages.
        /// </summary>
        private void BuildPages()
        {
            #region Reference Check Page
            string refCheckPageID = "C37B36D3-89E9-45E4-85F6-0B6702C19865";
            
            RockMigrationHelper.AddPage("5D612B47-4C1F-49CB-B71B-1BCC9AB2446E", "5FEAF34C-7FB6-4A11-8A1E-C452EC7849BD", "Protection App - Reference Check", "", refCheckPageID); // Site:Rock RMS
            RockMigrationHelper.AddPageRoute(refCheckPageID, "MyAccount/ProtectionApp/ReferenceCheck/{rckipid}");
            RockMigrationHelper.AddLayout("F3F82256-2D66-432B-9D67-3552CD2F4C2B", "Blank", "Blank", "", "E5F193D3-FDC4-4320-8A76-078995A247F9"); // Site:External Website
            RockMigrationHelper.UpdateBlockType("Protection App Reference Form", "Displays the reference form of the Protection App.", "~/Plugins/org_willowcreek/ProtectionApp/ReferenceForm.ascx", "org_willowcreek > Protection App", "419E73CC-4219-411E-A76A-05AA92BF2352");
            
            // Add/Update PageContext for Page:Workflow Configuration, Entity: Rock.Model.WorkflowType, Parameter: WorkflowTypeId
            RockMigrationHelper.UpdatePageContext(refCheckPageID, "Rock.Model.WorkflowType", "WorkflowTypeId", "DC6AA203-82B0-4459-ADD7-C3C237AD11BD");
            
            // Add Block to Page: Reference, Site: External Website
            RockMigrationHelper.AddBlock(refCheckPageID, "", "419E73CC-4219-411E-A76A-05AA92BF2352", "Protection App Reference Form", "Feature", "", "", 0, "9DC358E2-F4CF-488F-A66D-DE4DA1A0F722");
            
            ////External site
            //protectionAppPageID = Guid.NewGuid().ToString();
            //RockMigrationHelper.AddPage("5D612B47-4C1F-49CB-B71B-1BCC9AB2446E", "5FEAF34C-7FB6-4A11-8A1E-C452EC7849BD", "Protection Application", "", protectionAppPageID, "fa fa-check-square-o"); // Site:External Site
            //RockMigrationHelper.AddPageRoute(protectionAppPageID, "Reference");
            
            //// Add Block to Page: Reference, Site: External Website
            //RockMigrationHelper.AddBlock(protectionAppPageID, "", "A2BAC700-0B40-496C-87E7-3617555D1590", "Reference", "Feature", "", "", 0, "9DC358E2-F4CF-488F-A66D-DE4DA1A0F722");
            
            #endregion

            #region Send Protection Apps Page
            // Page: Send Protection Apps
            //RockMigrationHelper.AddPage("B0F4B33D-DD11-4CCC-B79D-9342831B8701", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Send Protection Apps", "", "FF82C946-8275-498B-8505-09EF0ECCA123", ""); // Site:Rock RMS
            //RockMigrationHelper.UpdateBlockType("Send Reference Request", "Block for selecting criteria to build a list of people who should receive a Reference request.", "~/Plugins/org_willowcreek/ProtectionApp/ProtectionAppSendRequest.ascx", "CRM > ProtectionAppRequest",  "A9213F2A-BF82-4FAA-9FC9-A34BDB12551B");
            //RockMigrationHelper.AddBlock("FF82C946-8275-498B-8505-09EF0ECCA123", "", "A9213F2A-BF82-4FAA-9FC9-A34BDB12551B", "Send Reference Request", "Main", "", "", 0, "BF0A584D-728F-44DB-9580-303CE1367BDB");
            //RockMigrationHelper.AddBlockTypeAttribute("A9213F2A-BF82-4FAA-9FC9-A34BDB12551B", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Maximum Recipients", "MaximumRecipients", "", "The maximum number of recipients allowed before communication will need to be approved", 0, @"300", "DAEADB4E-487B-4BAD-98D5-20C49750E811");
            #endregion
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            #region Reference Page
            // Remove Block: Reference, from Page: Reference, Site: External Website
            RockMigrationHelper.DeleteBlock("9DC358E2-F4CF-488F-A66D-DE4DA1A0F722");
            //RockMigrationHelper.DeleteBlockType("E28D32A8-74EB-4604-96F2-03434316BB64"); // Send Reference
            RockMigrationHelper.DeleteBlockType("419E73CC-4219-411E-A76A-05AA92BF2352"); // Reference
            // Delete PageContext for Page:Workflow Configuration, Entity: Rock.Model.WorkflowType, Parameter: WorkflowTypeId
            RockMigrationHelper.DeletePageContext("DC6AA203-82B0-4459-ADD7-C3C237AD11BD");
            #endregion

            #region Send Protection Apps Page
            RockMigrationHelper.DeleteAttribute("DAEADB4E-487B-4BAD-98D5-20C49750E811");
            RockMigrationHelper.DeleteBlock("BF0A584D-728F-44DB-9580-303CE1367BDB");
            RockMigrationHelper.DeleteBlockType("A9213F2A-BF82-4FAA-9FC9-A34BDB12551B");
            RockMigrationHelper.DeletePage("FF82C946-8275-498B-8505-09EF0ECCA123"); //  Page: Send Protection Apps            
            #endregion

            #region New Person Attributes
            #endregion

            #region Reference Workflow
            #endregion

            #region Bio block attribute removal
            // As far as I can tell, there is no single item removal delete for a multi-value attribute
            #endregion
        }
    }
}
