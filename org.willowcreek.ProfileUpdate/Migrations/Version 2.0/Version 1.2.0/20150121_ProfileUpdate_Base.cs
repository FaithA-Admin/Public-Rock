using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Plugin;

namespace org.willowcreek.ProfileUpdate.Migrations.Version_2._0.Version_1._2._0
{
    [MigrationNumber(1, "1.2")]
    public class _20150121_ProfileUpdate_Base : Migration
    {
        public override void Up()
        {
            #region Profile Update Page
            RockMigrationHelper.AddPage("C0854F84-2E8B-479C-A3FB-6B47BE89B795", "5FEAF34C-7FB6-4A11-8A1E-C452EC7849BD", "Profile Update", "", "4D3AE84F-A8F0-489A-8EB3-FA2AA419FD33", "fa fa-person");
            RockMigrationHelper.AddPageRoute("4D3AE84F-A8F0-489A-8EB3-FA2AA419FD33", "MyAccount/ProfileUpdate/{rckipid}");
            RockMigrationHelper.AddLayout("F3F82256-2D66-432B-9D67-3552CD2F4C2B", "Blank", "Blank", "", "023D30A4-1AA9-45FD-B1F6-0162A243BBB6"); // Site:External Website
            RockMigrationHelper.UpdateBlockType("Send Profile Request", "Block for selecting criteria to build a list of people who should receive a Profile request.", "~/Plugins/org_willowcreek/Profile/ProfileSendRequest.ascx", "org_willowcreek > Profile", "2C3E3C91-F7A9-4AAC-B989-81572A5A66EE");
            RockMigrationHelper.UpdateBlockType("Verify Profile", "Allows profile edits to be verified.", "~/Plugins/org_willowcreek/Profile/ProfileVerify.ascx", "org_willowcreek > Profile", "7635A576-DF20-4E4E-AC31-FA7983F49727");
            RockMigrationHelper.UpdateBlockType("Profile Update", "Allows a person to edit their profile information.", "~/Plugins/org_willowcreek/ProfileUpdate/ProfileUpdate.ascx", "org_willowcreek > Profile Update", "A2BAC700-0B40-496C-87E7-3617555D1590");

            // Add Block to Page: Profile Update, Site: External Website
            RockMigrationHelper.AddBlock("4D3AE84F-A8F0-489A-8EB3-FA2AA419FD33", "", "A2BAC700-0B40-496C-87E7-3617555D1590", "Profile Update", "Feature", "", "", 0, "4E28B62B-89E0-43D8-9F83-434DE1242230");
            
            // Attrib for BlockType: Attribute Values:Attribute Order
            RockMigrationHelper.AddBlockTypeAttribute("D70A59DC-16BE-43BE-9880-59598FA7A94C", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Attribute Order", "AttributeOrder", "", "The order to use for displaying attributes.  Note: this value is set through the block's UI and does not need to be set here.", 1, @"", "B7EB7168-DEAD-4BD0-A854-B94BC5BDE06E");

            // Add/Update PageContext for Page:Workflow Configuration, Entity: Rock.Model.WorkflowType, Parameter: WorkflowTypeId
            RockMigrationHelper.UpdatePageContext("DCB18A76-6DFF-48A5-A66E-2CAA10D2CA1A", "Rock.Model.WorkflowType", "WorkflowTypeId", "E904932A-4551-4A5A-B6BF-EF60AD8E90E6");

            // Update the Auth type for the page
            RockMigrationHelper.AddSecurityAuthForPage("4D3AE84F-A8F0-489A-8EB3-FA2AA419FD33", 0, "View", true, null, 2, "2DFD792B-8844-49C5-8861-2A4B52F546F2");
            RockMigrationHelper.AddSecurityAuthForPage("4D3AE84F-A8F0-489A-8EB3-FA2AA419FD33", 1, "View", false, null, 1, "72050F5B-59F1-4E35-80DF-15A348887E49");
            #endregion

            #region Send Profile Updates Page
            // Page: Send Profile Updates
            RockMigrationHelper.AddPage("B0F4B33D-DD11-4CCC-B79D-9342831B8701", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Profile Updates", "", "FB7E3B44-975F-45A8-83B7-5E8F745AA65D", ""); // Site:Rock RMS
            RockMigrationHelper.UpdateBlockType("Profile Update Batch Request", "Block for selecting criteria to build a list of people who should receive a profile update request.", "~/Plugins/org_willowcreek/ProfileUpdate/ProfileUpdateBatchRequest.ascx", "org_willowcreek > Profile Update", "2C7774DF-BA0E-4726-BF81-895519327325");
            RockMigrationHelper.AddBlock("FB7E3B44-975F-45A8-83B7-5E8F745AA65D", "", "2C7774DF-BA0E-4726-BF81-895519327325", "Send Profile Update Request", "Main", "", "", 0, "5A491DA0-7EFC-4458-ACA9-F877CC4C4595");
            RockMigrationHelper.AddBlockTypeAttribute("2C7774DF-BA0E-4726-BF81-895519327325", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Maximum Recipients", "MaximumRecipients", "", "The maximum number of recipients allowed before communication will need to be approved", 0, @"300", "C66726CF-DBFC-4723-BEFA-90FEA3F06B2C");
            RockMigrationHelper.AddBlockTypeAttribute("2C7774DF-BA0E-4726-BF81-895519327325", "C3B37465-DCAF-4C8C-930C-9A9B5D066CA9", "Photo Request Template", "PhotoRequestTemplate", "", "The template to use with this block to send requests.", 0, @"B9A0489C-A823-4C5C-A9F9-14A206EC3B88", "75B3F944-3584-46A7-BD3E-1BFF2F6FABDA");
            #endregion

            #region Defined Types
            //RockMigrationHelper.AddDefinedType("Person","School Grades","For WC School grades","7FE643F5-C790-4FD7-A1B4-19BE9220139D",@"");
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "Infant 0-12 months", "", "DBF87C3C-A8D5-492A-A1C4-BDCBDABDDD71", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "Toddler 13-18 Months", "", "0C862086-9911-47FD-ACC3-1AA63DE80686", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "Two Years Old", "", "42F6A264-3B37-425E-999C-58307FECBCE8", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "Three Years Old", "", "741FBF51-81B7-486D-BC6F-5B64112368E6", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "Four Years Old", "", "62129766-7A8E-462C-87E0-9A192FEE6ADE", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "Five Years Old", "", "CD303961-3FE8-4796-90DA-717F3BCA7732", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "Kindergarten", "", "501F5D97-04AA-412F-AC16-7AD4F916174E", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "1", "", "79AF94B8-4792-47CD-A8BF-1B87DAD8FC60", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D","2","","B4555705-DF02-41C5-AF62-288C37082C5E",false);	
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D","3","","E027A12D-F7A8-404E-A937-E1078509958F",false);	
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D","4","","2D0C183C-D767-43DA-95A1-B2F7C8D44E28",false);	
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D","5","","8F2E5EC2-E2C9-40DA-8EC9-3E7C87995ED0",false);	
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D","6","","78119C1D-E9E2-4405-AF61-4A72CBD2EB90",false);	
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D","7","","8AC1B8FC-05DD-4508-99D5-C803B9942F7D",false);	
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D","8","","8D2C5C5C-8611-42CF-903E-D53CE464819E",false);	
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D","9","","C2314362-88C4-4AD5-AF79-5437EA3E8DC5",false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "10", "", "B95A08D4-B77E-4480-B44E-68F37F791499", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "11", "", "F048DE3B-007C-4137-8979-278C6D976BE0", false);
            //RockMigrationHelper.AddDefinedValue("7FE643F5-C790-4FD7-A1B4-19BE9220139D", "12", "", "10BE2461-6AD0-44AF-A1BD-0BC200029B7F", false);
            //RockMigrationHelper.AddDefinedType("Global", "Section Community", "This is a section community type as described by the section in the South Barrington Willow Creek campus auditorium.", "0E7BBEA9-0804-4481-846C-1DC62A76390D", @"");
            //RockMigrationHelper.AddDefinedValue("0E7BBEA9-0804-4481-846C-1DC62A76390D", "101", "", "D3ACA359-DC87-44D4-8DD1-780C655065B1", false);	
            //RockMigrationHelper.AddDefinedValue("0E7BBEA9-0804-4481-846C-1DC62A76390D","102F","","436CAA54-3A30-4C34-8E4C-995EEC7EDF80",false);	
            //RockMigrationHelper.AddDefinedValue("0E7BBEA9-0804-4481-846C-1DC62A76390D","103F","","A0E9089A-0F3B-4E09-A9FA-60253783274F",false);	
            //RockMigrationHelper.AddDefinedValue("0E7BBEA9-0804-4481-846C-1DC62A76390D","104F","","5467B69F-07F5-4750-BD32-D3CFDFAA9623",false);	
            //RockMigrationHelper.AddDefinedValue("0E7BBEA9-0804-4481-846C-1DC62A76390D","105F","","4CF2647A-C06C-45CB-B0BA-0A482CDA6851",false);	
            //RockMigrationHelper.AddDefinedValue("0E7BBEA9-0804-4481-846C-1DC62A76390D","106F","","0D0EDB72-DC72-4A5D-8242-C910F90C5C7B",false);	
            //RockMigrationHelper.AddDefinedValue("0E7BBEA9-0804-4481-846C-1DC62A76390D","107","","DAE70C22-7AD3-420F-97E6-210FA8639204",false);	
            #endregion

            #region New Person Attributes
            //RockMigrationHelper.UpdatePersonAttribute("9C204CD0-1233-41C5-818A-C5DA439445AA", "E919E722-F895-44A4-B86D-38DB8FBA1844", "Health Note", "HealthNote", "fa fa-person", "For a child record, this may be collected at the time of registration.", 32, "", "DFBDEFD8-6A90-4DD7-9D50-68BA77F47815");
            //RockMigrationHelper.UpdatePersonAttribute("59D5A94C-94A0-4630-B80A-BB25697D74C7", "E919E722-F895-44A4-B86D-38DB8FBA1844", "Primary Section", "PrimarySection", "fa fa-person", "", 30, "", "ECF1AD4F-0BBA-44D9-847D-D5EE61D9B10C");
            // Make sure the attribute that points to a defined type has the qualifier record to tell the system precisely which defined type we mean...
            //RockMigrationHelper.AddAttributeQualifier("ECF1AD4F-0BBA-44D9-847D-D5EE61D9B10C", "definedtype", "0", "085A9A5D-DCFB-4375-AC15-7DDDCB3F33AC");
            //Sql
            //(
            //    @" DECLARE @DefinedTypeId int = (SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '0E7BBEA9-0804-4481-846C-1DC62A76390D')
            //       DECLARE @DefinedTypeAsString nvarchar(max) = (SELECT CONVERT(nvarchar(max), @DefinedTypeId))                   
            //       UPDATE [AttributeQualifier]  SET [Value] = @DefinedTypeAsString WHERE [Guid] = '085A9A5D-DCFB-4375-AC15-7DDDCB3F33AC'
            //");
            //RockMigrationHelper.UpdatePersonAttribute("59D5A94C-94A0-4630-B80A-BB25697D74C7", "752DC692-836E-4A3E-B670-4325CD7724BF", "Child Grade", "ChildGrade", "fa fa-person", "", 33, "", "3F133F28-0ACF-4F85-AFFD-7A21656EF7EE");
            // Make sure the attribute that points to a defined type has the qualifier record to tell the system precisely which defined type we mean...
            //RockMigrationHelper.AddAttributeQualifier("3F133F28-0ACF-4F85-AFFD-7A21656EF7EE", "definedtype", "0", "1F3B4EC3-1758-4816-B83F-B09D21D47E42");
            //Sql
            //(
            //    @" DECLARE @DefinedTypeId int = (SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '7FE643F5-C790-4FD7-A1B4-19BE9220139D')
            //       DECLARE @DefinedTypeAsString nvarchar(max) = (SELECT CONVERT(nvarchar(max), @DefinedTypeId))                   
            //       UPDATE [AttributeQualifier]  SET [Value] = @DefinedTypeAsString WHERE [Guid] = '1F3B4EC3-1758-4816-B83F-B09D21D47E42'
            //");
            //RockMigrationHelper.UpdatePersonAttribute("2F8F5EC4-57FA-4F6C-AB15-9D6616994580", "E919E722-F895-44A4-B86D-38DB8FBA1844", "Primary Service", "PrimaryService", "fa fa-person", "A time field for the primary service which the person attends.  To be used alongside the Primary Section.", 31, "", "69B3A7AC-DFA1-4F09-9713-0640FADBB469");
            #endregion

            #region Profile Update Workflow
            RockMigrationHelper.UpdateEntityType("Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true);
            RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true);
            RockMigrationHelper.UpdateEntityType("org.willowcreek.Workflow.Action.Elapse", "1FC3F882-9EFD-4959-AB24-3DDE96F837BC", false, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.CompleteActivity", "0D5E33A5-8700-4168-A42E-74D78B62D717", false, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.RunSQL", "A41216D6-6FB0-4019-B222-2C29B4519CF4", false, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SendEmail", "66197B01-D1F0-4924-A315-47AD54E030DE", false, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeToCurrentPerson", "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", false, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeFromEntity", "972F19B9-598B-474B-97A4-50E56E7B59D2", false, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeValue", "C789E457-0783-44B3-9D8F-2EBAB5F11110", false, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetPersonAttribute", "320622DA-52E0-41AE-AF90-2BF78B488552", false, true);
            RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetStatus", "96D371A7-A291-4F8F-8B38-B8F72CE5407E", false, true);
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("0D5E33A5-8700-4168-A42E-74D78B62D717", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "3463643B-2601-4116-B47C-A0D7D21D4BF4"); // Rock.Workflow.Action.CompleteActivity:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("0D5E33A5-8700-4168-A42E-74D78B62D717", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "9A2EB793-E272-440F-B7CB-3BFBFE037A0E"); // Rock.Workflow.Action.CompleteActivity:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "D5A4C63A-37CB-4003-9E85-77DD96EF6093"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Destination Elapsed Attribute", "DestinationElapsedAttribute", "The attribute that will contain the value of the elapsed interval.", 1, @"", "5369480E-FE99-4578-A82A-1FF5F078EF43"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Destination Elapsed Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Initial Date Attribute", "InitialDateAttribute", "The attribute that contains the start of the interval for which elapsed time will be calculated.  This attribute must exist on the Workflow.", 0, @"", "20CD0B82-F75E-43EF-8EC1-26F9F846053A"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Initial Date Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Interval Type", "IntervalType", "Select an interval with which to measure elapsed time.", 2, @"Days", "D7F80FBD-9880-458D-A2BE-12F77480DC4B"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Interval Type
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "8DB7265C-177E-4924-BAF3-E7D6D2250A7B"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "DE9CB292-4785-4EA3-976D-3826F91E9E98"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person Attribute", "PersonAttribute", "The attribute to set to the currently logged in person.", 0, @"", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Person Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("320622DA-52E0-41AE-AF90-2BF78B488552", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184"); // Rock.Workflow.Action.SetPersonAttribute:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("320622DA-52E0-41AE-AF90-2BF78B488552", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person", "Person", "Workflow attribute that contains the person to update.", 0, @"", "E456FB6F-05DB-4826-A612-5B704BC4EA13"); // Rock.Workflow.Action.SetPersonAttribute:Person
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("320622DA-52E0-41AE-AF90-2BF78B488552", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Value|Attribute Value", "Value", "The value or attribute value to set the person attribute to. <span class='tip tip-lava'></span>", 2, @"", "94689BDE-493E-4869-A614-2D54822D747C"); // Rock.Workflow.Action.SetPersonAttribute:Value|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("320622DA-52E0-41AE-AF90-2BF78B488552", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "Person Attribute", "PersonAttribute", "The person attribute that should be updated with the provided value.", 1, @"", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762"); // Rock.Workflow.Action.SetPersonAttribute:Person Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("320622DA-52E0-41AE-AF90-2BF78B488552", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B"); // Rock.Workflow.Action.SetPersonAttribute:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Body", "Body", "The body of the email that should be sent. <span class='tip tip-lava'></span> <span class='tip tip-html'></span>", 3, @"", "4D245B9E-6B03-46E7-8482-A51FBA190E4D"); // Rock.Workflow.Action.SendEmail:Body
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "36197160-7D3D-490D-AB42-7E29105AFE91"); // Rock.Workflow.Action.SendEmail:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "From Email Address|Attribute Value", "From", "The email address or an attribute that contains the person or email address that email should be sent from (will default to organization email). <span class='tip tip-lava'></span>", 0, @"", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC"); // Rock.Workflow.Action.SendEmail:From Email Address|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Send To Email Address|Attribute Value", "To", "The email address or an attribute that contains the person or email address that email should be sent to. <span class='tip tip-lava'></span>", 1, @"", "0C4C13B8-7076-4872-925A-F950886B5E16"); // Rock.Workflow.Action.SendEmail:Send To Email Address|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Subject", "Subject", "The subject that should be used when sending email. <span class='tip tip-lava'></span>", 2, @"", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386"); // Rock.Workflow.Action.SendEmail:Subject
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "D1269254-C15A-40BD-B784-ADCC231D3950"); // Rock.Workflow.Action.SendEmail:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("96D371A7-A291-4F8F-8B38-B8F72CE5407E", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "36CE41F4-4C87-4096-B0C6-8269163BCC0A"); // Rock.Workflow.Action.SetStatus:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("96D371A7-A291-4F8F-8B38-B8F72CE5407E", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Status", "Status", "The status to set workflow to. <span class='tip tip-lava'></span>", 0, @"", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34"); // Rock.Workflow.Action.SetStatus:Status
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("96D371A7-A291-4F8F-8B38-B8F72CE5407E", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "AE8C180C-E370-414A-B10D-97891B95D105"); // Rock.Workflow.Action.SetStatus:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1"); // Rock.Workflow.Action.SetAttributeToEntity:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Use Id instead of Guid", "UseId", "Most entity attribute field types expect the Guid of the entity (which is used by default). Select this option if the entity's Id should be used instead (should be rare).", 1, @"False", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B"); // Rock.Workflow.Action.SetAttributeToEntity:Use Id instead of Guid
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Attribute", "Attribute", "The attribute to set the value of.", 0, @"", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7"); // Rock.Workflow.Action.SetAttributeToEntity:Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("972F19B9-598B-474B-97A4-50E56E7B59D2", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB"); // Rock.Workflow.Action.SetAttributeToEntity:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "SQLQuery", "SQLQuery", "The SQL query to run. <span class='tip tip-lava'></span>", 0, @"", "F3B9908B-096F-460B-8320-122CF046D1F9"); // Rock.Workflow.Action.RunSQL:SQLQuery
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "A18C3143-0586-4565-9F36-E603BC674B4E"); // Rock.Workflow.Action.RunSQL:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Result Attribute", "ResultAttribute", "An optional attribute to set to the scaler result of SQL query.", 1, @"", "56997192-2545-4EA1-B5B2-313B04588984"); // Rock.Workflow.Action.RunSQL:Result Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("A41216D6-6FB0-4019-B222-2C29B4519CF4", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "FA7C685D-8636-41EF-9998-90FFF3998F76"); // Rock.Workflow.Action.RunSQL:Order
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("C789E457-0783-44B3-9D8F-2EBAB5F11110", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "D7EAA859-F500-4521-9523-488B12EAA7D2"); // Rock.Workflow.Action.SetAttributeValue:Active
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("C789E457-0783-44B3-9D8F-2EBAB5F11110", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Attribute", "Attribute", "The attribute to set the value of.", 0, @"", "44A0B977-4730-4519-8FF6-B0A01A95B212"); // Rock.Workflow.Action.SetAttributeValue:Attribute
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("C789E457-0783-44B3-9D8F-2EBAB5F11110", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Text Value|Attribute Value", "Value", "The text or attribute to set the value from. <span class='tip tip-lava'></span>", 1, @"", "E5272B11-A2B8-49DC-860D-8D574E2BC15C"); // Rock.Workflow.Action.SetAttributeValue:Text Value|Attribute Value
            RockMigrationHelper.UpdateWorkflowActionEntityAttribute("C789E457-0783-44B3-9D8F-2EBAB5F11110", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "57093B41-50ED-48E5-B72B-8829E62704C8"); // Rock.Workflow.Action.SetAttributeValue:Order
            RockMigrationHelper.UpdateWorkflowType(false, true, "Profile Update", "This workflow creates an email request containing a link with which a member can update their profile.  The landing page is a custom block with profile and family data.", "BBAE05FD-8192-4616-A71E-903A927E0D90", "Request", "fa fa-user", 0, true, 3, "A84EA226-1CB2-453B-87B6-81F5360BAD3D"); // Profile Update
            RockMigrationHelper.UpdateWorkflowTypeAttribute("A84EA226-1CB2-453B-87B6-81F5360BAD3D", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Data Status - Profile Completed", "DataStatus-Profile", "This is an attribute holding the value that will be assigned to the Person in a later action.", 5, @"42f4844a-0749-4bae-ab61-ec1d181648e0", "A994E2E5-2C1A-4D7E-8006-11A4098D09EF"); // Profile Update:Data Status - Profile Completed
            RockMigrationHelper.UpdateWorkflowTypeAttribute("A84EA226-1CB2-453B-87B6-81F5360BAD3D", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Data Status - Profile Emailed", "DataStatus-ProfileEmailed", "This is a particular value stored so that it can be later assigned to the Person object in an action.  At time of writing, defined types cannot be assigned to attributes in a workflow action.", 4, @"65e27ef2-905e-42ed-ae1a-7b128c9e46fd", "2181E64C-1E87-4EB4-9369-5290C7E46371"); // Profile Update:Data Status - Profile Emailed
            RockMigrationHelper.UpdateWorkflowTypeAttribute("A84EA226-1CB2-453B-87B6-81F5360BAD3D", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "Data Status - Profile Expired", "DataStatus-ProfileExpired", "This an attribute that holds the defined value that will be assigned to the Person in a later action.", 6, @"9eb99c51-e637-4b9e-b4d5-9c04437d50c4", "16DA3FED-2A24-482A-B0F7-54B3FF467082"); // Profile Update:Data Status - Profile Expired
            RockMigrationHelper.UpdateWorkflowTypeAttribute("A84EA226-1CB2-453B-87B6-81F5360BAD3D", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "The person to which the request is being sent.", 0, @"", "E276DD8B-CEC2-4E87-A085-BC95EC23762A"); // Profile Update:Person
            Sql /* Make sure the above attribute shows up in the Workflow Grid column */
            (
                @"UPDATE [Attribute] SET [IsGridColumn] = 1 WHERE [Guid] = 'E276DD8B-CEC2-4E87-A085-BC95EC23762A'
            ");
            RockMigrationHelper.UpdateWorkflowTypeAttribute("A84EA226-1CB2-453B-87B6-81F5360BAD3D", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Recipient Email", "RecipientEmail", "This attribute stores the recipient email address so it can be displayed in the Workflow Manage grid.", 7, @"", "3E9A915D-9A62-429E-9EF8-3A7252D18322"); // Profile Update:Recipient Email
            Sql /* Make sure the above attribute shows up in the Workflow Grid column */
            (
                @"UPDATE [Attribute] SET [IsGridColumn] = 1 WHERE [Guid] = '3E9A915D-9A62-429E-9EF8-3A7252D18322'
            ");
            RockMigrationHelper.UpdateWorkflowTypeAttribute("A84EA226-1CB2-453B-87B6-81F5360BAD3D", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Reentry IP", "ReentryIP", "When a user clicks the link in the email to update their profile, their IP address will be collected and stored on the workflow instance.", 3, @"", "93564B56-0A40-412A-AF3C-AF5AB513B250"); // Profile Update:Reentry IP
            Sql /* Make sure the above attribute shows up in the Workflow Grid column */
            (
                @"UPDATE [Attribute] SET [IsGridColumn] = 1 WHERE [Guid] = '93564B56-0A40-412A-AF3C-AF5AB513B250'
            ");
            RockMigrationHelper.UpdateWorkflowTypeAttribute("A84EA226-1CB2-453B-87B6-81F5360BAD3D", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Sender", "Sender", "The person sending the request.", 1, @"", "D0F92801-EEF1-43F1-BBBA-A4AC8E39E39D"); // Profile Update:Sender
            RockMigrationHelper.UpdateWorkflowTypeAttribute("A84EA226-1CB2-453B-87B6-81F5360BAD3D", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Workflow Start Time", "WorkflowStartTime", "This will be the stored value when the workflow sent the email and officially started the expiration period.", 2, @"", "8D0CB636-8A56-4793-83A8-9A83AF7B9425"); // Profile Update:Workflow Start Time
            RockMigrationHelper.AddAttributeQualifier("A994E2E5-2C1A-4D7E-8006-11A4098D09EF", "allowmultiple", @"False", "D4797A4B-2E27-4CF9-AE44-2AC3B184F092"); // Profile Update:Data Status - Profile Completed:allowmultiple
            RockMigrationHelper.AddAttributeQualifier("A994E2E5-2C1A-4D7E-8006-11A4098D09EF", "definedtype", @"60", "61FC51D8-0B61-4189-B072-73981B7EE246"); // Profile Update:Data Status - Profile Completed:definedtype
            RockMigrationHelper.AddAttributeQualifier("2181E64C-1E87-4EB4-9369-5290C7E46371", "allowmultiple", @"False", "9C8DBC4B-15C6-453F-9D60-26ABEC50F93D"); // Profile Update:Data Status - Profile Emailed:allowmultiple
            RockMigrationHelper.AddAttributeQualifier("2181E64C-1E87-4EB4-9369-5290C7E46371", "definedtype", @"60", "F52E30BD-AC70-413E-82A9-E9FDE9506E48"); // Profile Update:Data Status - Profile Emailed:definedtype
            RockMigrationHelper.AddAttributeQualifier("16DA3FED-2A24-482A-B0F7-54B3FF467082", "allowmultiple", @"False", "41A8A2DD-EE65-480D-BFCC-227AEBA03059"); // Profile Update:Data Status - Profile Expired:allowmultiple
            RockMigrationHelper.AddAttributeQualifier("16DA3FED-2A24-482A-B0F7-54B3FF467082", "definedtype", @"60", "999EB6EB-4C14-4CE8-B583-EF6E6A6A1B19"); // Profile Update:Data Status - Profile Expired:definedtype
            RockMigrationHelper.AddAttributeQualifier("3E9A915D-9A62-429E-9EF8-3A7252D18322", "ispassword", @"False", "1777EA80-9881-41B5-9646-58E12BFE0A6D"); // Profile Update:Recipient Email:ispassword
            RockMigrationHelper.AddAttributeQualifier("93564B56-0A40-412A-AF3C-AF5AB513B250", "ispassword", @"False", "06A277D8-E3DD-47EC-973D-A75B05A50A25"); // Profile Update:Reentry IP:ispassword
            RockMigrationHelper.AddAttributeQualifier("8D0CB636-8A56-4793-83A8-9A83AF7B9425", "ispassword", @"False", "826F8A76-8E85-49BB-B329-3AA4980CD365"); // Profile Update:Workflow Start Time:ispassword
            RockMigrationHelper.UpdateWorkflowActivityType("A84EA226-1CB2-453B-87B6-81F5360BAD3D", true, "Launch Request", "The first activity is responsible for collecting Person and/or Sender information and generating the Profile Data Request", true, 0, "02832F61-63ED-4268-8887-D0798791FC4C"); // Profile Update:Launch Request
            RockMigrationHelper.UpdateWorkflowActivityType("A84EA226-1CB2-453B-87B6-81F5360BAD3D", true, "Receive Profile Data Update", "The second activity handles the user's clicking of the link in the email and arriving at the WorkflowEntry block for accepting the updated Profile data.  This activity will be set to active when the email is sent, so that the WorkflowEntry block will pick up where the workflow left off.", false, 1, "990AA978-9393-4585-A797-C995EF666571"); // Profile Update:Receive Profile Data Update
            RockMigrationHelper.UpdateWorkflowActivityType("A84EA226-1CB2-453B-87B6-81F5360BAD3D", true, "Evaluate Expiration Date", "The third activity runs as often as the workflow is active to check to see if the status of the workflow is still \"Email Sent\" after a certain period of time.  In order to make sure the expiration check is always run, that action is first, and will mark the activity completed when it runs.  The second action, setting the elapsed time attribute will then run, but not complete itself, as it will be run again the next time the activity is processed.  It will never complete.  The activity will complete if the expiration is reached.", true, 2, "CC8A91EA-B209-4877-9B25-A56A23890808"); // Profile Update:Evaluate Expiration Date
            RockMigrationHelper.UpdateWorkflowActivityTypeAttribute("CC8A91EA-B209-4877-9B25-A56A23890808", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Days Since Activation", "DaysSinceActivation", "This activity variable will hold the result of the Elapse custom action that determines how many days gave passed since the Email was sent t o the recipient for which we issued this Profile Update request.", 0, @"", "F0E1C0D4-657A-42D0-8156-D7AC65935AAE"); // Profile Update:Evaluate Expiration Date:Days Since Activation
            RockMigrationHelper.UpdateWorkflowActionType("02832F61-63ED-4268-8887-D0798791FC4C", "Send Profile Data Request", 4, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "808D4A54-50AA-4357-A2F7-23D57A7ECDBF"); // Profile Update:Launch Request:Send Profile Data Request
            RockMigrationHelper.UpdateWorkflowActionType("02832F61-63ED-4268-8887-D0798791FC4C", "Set Workflow Status - Email Sent", 5, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "AB94D1EB-4258-4471-AA91-CD25F6B65282"); // Profile Update:Launch Request:Set Workflow Status - Email Sent
            RockMigrationHelper.UpdateWorkflowActionType("990AA978-9393-4585-A797-C995EF666571", "Set Workflow Status - Received Profile Update", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "8E4C614A-6CC8-4AD1-80C8-2B1608110495"); // Profile Update:Receive Profile Data Update:Set Workflow Status - Received Profile Update
            RockMigrationHelper.UpdateWorkflowActionType("02832F61-63ED-4268-8887-D0798791FC4C", "Set Workflow Recipient Email Attribute", 3, "A41216D6-6FB0-4019-B222-2C29B4519CF4", true, false, "", "", 1, "", "0A6190E7-0BEA-4C72-AC25-8D01E22246A5"); // Profile Update:Launch Request:Set Workflow Recipient Email Attribute
            RockMigrationHelper.UpdateWorkflowActionType("02832F61-63ED-4268-8887-D0798791FC4C", "Set Sender", 2, "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", true, false, "", "", 1, "", "69FC7C31-74FC-425E-9253-247238F3D2E5"); // Profile Update:Launch Request:Set Sender
            RockMigrationHelper.UpdateWorkflowActionType("02832F61-63ED-4268-8887-D0798791FC4C", "Set Person", 1, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "E7AE2A36-D060-451C-824F-2EBBC80EDEE3"); // Profile Update:Launch Request:Set Person
            RockMigrationHelper.UpdateWorkflowActionType("02832F61-63ED-4268-8887-D0798791FC4C", "Set Workflow Start Time", 0, "C789E457-0783-44B3-9D8F-2EBAB5F11110", true, false, "", "", 1, "", "6B401B5B-C70B-43D6-9A8C-8FE3D039ACC5"); // Profile Update:Launch Request:Set Workflow Start Time
            RockMigrationHelper.UpdateWorkflowActionType("CC8A91EA-B209-4877-9B25-A56A23890808", "Cease Expiration Check", 2, "0D5E33A5-8700-4168-A42E-74D78B62D717", true, true, "", "93564B56-0A40-412A-AF3C-AF5AB513B250", 64, "", "E68FCC5A-D33B-4FAB-86A8-A9444091D4DB"); // Profile Update:Evaluate Expiration Date:Cease Expiration Check
            RockMigrationHelper.UpdateWorkflowActionType("CC8A91EA-B209-4877-9B25-A56A23890808", "Set Person Data Status - Profile Expired", 0, "320622DA-52E0-41AE-AF90-2BF78B488552", true, true, "", "F0E1C0D4-657A-42D0-8156-D7AC65935AAE", 256, "30", "9FA01606-7419-4B88-BE44-E9649BC644E5"); // Profile Update:Evaluate Expiration Date:Set Person Data Status - Profile Expired
            RockMigrationHelper.UpdateWorkflowActionType("02832F61-63ED-4268-8887-D0798791FC4C", "Set Person Profile Sent Date", 6, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "A0C503C7-1F3B-43BF-8401-4CD587B7BFB2"); // Profile Update:Launch Request:Set Person Profile Sent Date
            RockMigrationHelper.UpdateWorkflowActionType("02832F61-63ED-4268-8887-D0798791FC4C", "Set Person Data Status - Profile Emailed", 7, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "35CFE160-CF9F-4EC0-8F42-63168D074AF4"); // Profile Update:Launch Request:Set Person Data Status - Profile Emailed
            RockMigrationHelper.UpdateWorkflowActionType("990AA978-9393-4585-A797-C995EF666571", "Set Person Data Status - Profile Completed", 1, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "450CD5CB-DB25-40D2-A460-F3F258BA0478"); // Profile Update:Receive Profile Data Update:Set Person Data Status - Profile Completed
            RockMigrationHelper.UpdateWorkflowActionType("990AA978-9393-4585-A797-C995EF666571", "Set Person Profile Completed Date", 2, "320622DA-52E0-41AE-AF90-2BF78B488552", true, false, "", "", 1, "", "28C4977F-608D-464F-A126-F456EBE1ABE8"); // Profile Update:Receive Profile Data Update:Set Person Profile Completed Date
            RockMigrationHelper.UpdateWorkflowActionType("CC8A91EA-B209-4877-9B25-A56A23890808", "Calculate Days Since Activation", 1, "1FC3F882-9EFD-4959-AB24-3DDE96F837BC", false, false, "", "", 1, "", "E8757EC8-458C-41E8-832E-4294862AB39B"); // Profile Update:Evaluate Expiration Date:Calculate Days Since Activation
            RockMigrationHelper.AddActionTypeAttributeValue("6B401B5B-C70B-43D6-9A8C-8FE3D039ACC5", "D7EAA859-F500-4521-9523-488B12EAA7D2", @"False"); // Profile Update:Launch Request:Set Workflow Start Time:Active
            RockMigrationHelper.AddActionTypeAttributeValue("6B401B5B-C70B-43D6-9A8C-8FE3D039ACC5", "44A0B977-4730-4519-8FF6-B0A01A95B212", @"8d0cb636-8a56-4793-83a8-9a83af7b9425"); // Profile Update:Launch Request:Set Workflow Start Time:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("6B401B5B-C70B-43D6-9A8C-8FE3D039ACC5", "57093B41-50ED-48E5-B72B-8829E62704C8", @""); // Profile Update:Launch Request:Set Workflow Start Time:Order
            RockMigrationHelper.AddActionTypeAttributeValue("6B401B5B-C70B-43D6-9A8C-8FE3D039ACC5", "E5272B11-A2B8-49DC-860D-8D574E2BC15C", @"{{ 'Now' | Date:'yyyy-MM-ddThh:mm:ss' }}"); // Profile Update:Launch Request:Set Workflow Start Time:Text Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("E7AE2A36-D060-451C-824F-2EBBC80EDEE3", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False"); // Profile Update:Launch Request:Set Person:Active
            RockMigrationHelper.AddActionTypeAttributeValue("E7AE2A36-D060-451C-824F-2EBBC80EDEE3", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"e276dd8b-cec2-4e87-a085-bc95ec23762a"); // Profile Update:Launch Request:Set Person:Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("E7AE2A36-D060-451C-824F-2EBBC80EDEE3", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @""); // Profile Update:Launch Request:Set Person:Order
            RockMigrationHelper.AddActionTypeAttributeValue("E7AE2A36-D060-451C-824F-2EBBC80EDEE3", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False"); // Profile Update:Launch Request:Set Person:Use Id instead of Guid
            RockMigrationHelper.AddActionTypeAttributeValue("69FC7C31-74FC-425E-9253-247238F3D2E5", "DE9CB292-4785-4EA3-976D-3826F91E9E98", @"False"); // Profile Update:Launch Request:Set Sender:Active
            RockMigrationHelper.AddActionTypeAttributeValue("69FC7C31-74FC-425E-9253-247238F3D2E5", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112", @"d0f92801-eef1-43f1-bbba-a4ac8e39e39d"); // Profile Update:Launch Request:Set Sender:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("69FC7C31-74FC-425E-9253-247238F3D2E5", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8", @""); // Profile Update:Launch Request:Set Sender:Order
            RockMigrationHelper.AddActionTypeAttributeValue("0A6190E7-0BEA-4C72-AC25-8D01E22246A5", "F3B9908B-096F-460B-8320-122CF046D1F9", @"DECLARE @PersonAliasGuid uniqueidentifier = '{{ Workflow.Person_unformatted }}'
SELECT [Email] 
FROM [Person] p
INNER JOIN [PersonAlias] pa ON pa.PersonId = p.Id
WHERE pa.[Guid] = @PersonAliasGuid"); // Profile Update:Launch Request:Set Workflow Recipient Email Attribute:SQLQuery
            RockMigrationHelper.AddActionTypeAttributeValue("0A6190E7-0BEA-4C72-AC25-8D01E22246A5", "A18C3143-0586-4565-9F36-E603BC674B4E", @"False"); // Profile Update:Launch Request:Set Workflow Recipient Email Attribute:Active
            RockMigrationHelper.AddActionTypeAttributeValue("0A6190E7-0BEA-4C72-AC25-8D01E22246A5", "FA7C685D-8636-41EF-9998-90FFF3998F76", @""); // Profile Update:Launch Request:Set Workflow Recipient Email Attribute:Order
            RockMigrationHelper.AddActionTypeAttributeValue("0A6190E7-0BEA-4C72-AC25-8D01E22246A5", "56997192-2545-4EA1-B5B2-313B04588984", @"3e9a915d-9a62-429e-9ef8-3a7252d18322"); // Profile Update:Launch Request:Set Workflow Recipient Email Attribute:Result Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("808D4A54-50AA-4357-A2F7-23D57A7ECDBF", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False"); // Profile Update:Launch Request:Send Profile Data Request:Active
            RockMigrationHelper.AddActionTypeAttributeValue("808D4A54-50AA-4357-A2F7-23D57A7ECDBF", "D1269254-C15A-40BD-B784-ADCC231D3950", @""); // Profile Update:Launch Request:Send Profile Data Request:Order
            RockMigrationHelper.AddActionTypeAttributeValue("808D4A54-50AA-4357-A2F7-23D57A7ECDBF", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @"d0f92801-eef1-43f1-bbba-a4ac8e39e39d"); // Profile Update:Launch Request:Send Profile Data Request:From Email Address|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("808D4A54-50AA-4357-A2F7-23D57A7ECDBF", "0C4C13B8-7076-4872-925A-F950886B5E16", @"e276dd8b-cec2-4e87-a085-bc95ec23762a"); // Profile Update:Launch Request:Send Profile Data Request:Send To Email Address|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("808D4A54-50AA-4357-A2F7-23D57A7ECDBF", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"Profile Update Request from {{ Workflow.Sender }}"); // Profile Update:Launch Request:Send Profile Data Request:Subject
            RockMigrationHelper.AddActionTypeAttributeValue("808D4A54-50AA-4357-A2F7-23D57A7ECDBF", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ GlobalAttribute.EmailStyles }}
            {{ GlobalAttribute.EmailHeader }}
            <p>{{ Person.NickName }},</p>

            <p>{{ Workflow.CustomMessage | NewlineToBr }}</p>

            <p>Please click the link to <a href=""{{ GlobalAttribute.PublicApplicationRoot }}MyAccount/ProfileUpdate/{{ Person.UrlEncodedKey }}?WorkflowId={{ Workflow.Guid }}"">update your profile</a>.</p>

            <p><a href=""{{ GlobalAttribute.PublicApplicationRoot }}Unsubscribe/{{ Person.UrlEncodedKey }}"">I&#39;m no longer involved with {{ GlobalAttribute.OrganizationName }}. Please remove me from all future communications.</a></p>

            <p>-{{ Workflow.Sender }}</p>

            {{ GlobalAttribute.EmailFooter }}

            "); // Profile Update:Launch Request:Send Profile Data Request:Body
            RockMigrationHelper.AddActionTypeAttributeValue("AB94D1EB-4258-4471-AA91-CD25F6B65282", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Profile Update:Launch Request:Set Workflow Status - Email Sent:Active
            RockMigrationHelper.AddActionTypeAttributeValue("AB94D1EB-4258-4471-AA91-CD25F6B65282", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Email Sent"); // Profile Update:Launch Request:Set Workflow Status - Email Sent:Status
            RockMigrationHelper.AddActionTypeAttributeValue("AB94D1EB-4258-4471-AA91-CD25F6B65282", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Profile Update:Launch Request:Set Workflow Status - Email Sent:Order
            RockMigrationHelper.AddActionTypeAttributeValue("A0C503C7-1F3B-43BF-8401-4CD587B7BFB2", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Profile Update:Launch Request:Set Person Profile Sent Date:Active
            RockMigrationHelper.AddActionTypeAttributeValue("A0C503C7-1F3B-43BF-8401-4CD587B7BFB2", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"e276dd8b-cec2-4e87-a085-bc95ec23762a"); // Profile Update:Launch Request:Set Person Profile Sent Date:Person
            RockMigrationHelper.AddActionTypeAttributeValue("A0C503C7-1F3B-43BF-8401-4CD587B7BFB2", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Profile Update:Launch Request:Set Person Profile Sent Date:Order
            RockMigrationHelper.AddActionTypeAttributeValue("A0C503C7-1F3B-43BF-8401-4CD587B7BFB2", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"3307d1dc-f5bc-45d2-96b8-41ff0a048dcf"); // Profile Update:Launch Request:Set Person Profile Sent Date:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("A0C503C7-1F3B-43BF-8401-4CD587B7BFB2", "94689BDE-493E-4869-A614-2D54822D747C", @"{{ 'Now' | Date:'yyyy-MM-ddThh:mm:ss' }}"); // Profile Update:Launch Request:Set Person Profile Sent Date:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("35CFE160-CF9F-4EC0-8F42-63168D074AF4", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Profile Update:Launch Request:Set Person Data Status - Profile Emailed:Active
            RockMigrationHelper.AddActionTypeAttributeValue("35CFE160-CF9F-4EC0-8F42-63168D074AF4", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"e276dd8b-cec2-4e87-a085-bc95ec23762a"); // Profile Update:Launch Request:Set Person Data Status - Profile Emailed:Person
            RockMigrationHelper.AddActionTypeAttributeValue("35CFE160-CF9F-4EC0-8F42-63168D074AF4", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Profile Update:Launch Request:Set Person Data Status - Profile Emailed:Order
            RockMigrationHelper.AddActionTypeAttributeValue("35CFE160-CF9F-4EC0-8F42-63168D074AF4", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"68f7f4ab-26d3-463e-bd5a-d18da051fa1d"); // Profile Update:Launch Request:Set Person Data Status - Profile Emailed:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("35CFE160-CF9F-4EC0-8F42-63168D074AF4", "94689BDE-493E-4869-A614-2D54822D747C", @"2181e64c-1e87-4eb4-9369-5290c7e46371"); // Profile Update:Launch Request:Set Person Data Status - Profile Emailed:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("8E4C614A-6CC8-4AD1-80C8-2B1608110495", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Profile Update:Receive Profile Data Update:Set Workflow Status - Received Profile Update:Active
            RockMigrationHelper.AddActionTypeAttributeValue("8E4C614A-6CC8-4AD1-80C8-2B1608110495", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Received Profile Update"); // Profile Update:Receive Profile Data Update:Set Workflow Status - Received Profile Update:Status
            RockMigrationHelper.AddActionTypeAttributeValue("8E4C614A-6CC8-4AD1-80C8-2B1608110495", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Profile Update:Receive Profile Data Update:Set Workflow Status - Received Profile Update:Order
            RockMigrationHelper.AddActionTypeAttributeValue("450CD5CB-DB25-40D2-A460-F3F258BA0478", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Profile Update:Receive Profile Data Update:Set Person Data Status - Profile Completed:Active
            RockMigrationHelper.AddActionTypeAttributeValue("450CD5CB-DB25-40D2-A460-F3F258BA0478", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"e276dd8b-cec2-4e87-a085-bc95ec23762a"); // Profile Update:Receive Profile Data Update:Set Person Data Status - Profile Completed:Person
            RockMigrationHelper.AddActionTypeAttributeValue("450CD5CB-DB25-40D2-A460-F3F258BA0478", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Profile Update:Receive Profile Data Update:Set Person Data Status - Profile Completed:Order
            RockMigrationHelper.AddActionTypeAttributeValue("450CD5CB-DB25-40D2-A460-F3F258BA0478", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"68f7f4ab-26d3-463e-bd5a-d18da051fa1d"); // Profile Update:Receive Profile Data Update:Set Person Data Status - Profile Completed:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("450CD5CB-DB25-40D2-A460-F3F258BA0478", "94689BDE-493E-4869-A614-2D54822D747C", @"a994e2e5-2c1a-4d7e-8006-11a4098d09ef"); // Profile Update:Receive Profile Data Update:Set Person Data Status - Profile Completed:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("28C4977F-608D-464F-A126-F456EBE1ABE8", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Profile Update:Receive Profile Data Update:Set Person Profile Completed Date:Active
            RockMigrationHelper.AddActionTypeAttributeValue("28C4977F-608D-464F-A126-F456EBE1ABE8", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"e276dd8b-cec2-4e87-a085-bc95ec23762a"); // Profile Update:Receive Profile Data Update:Set Person Profile Completed Date:Person
            RockMigrationHelper.AddActionTypeAttributeValue("28C4977F-608D-464F-A126-F456EBE1ABE8", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Profile Update:Receive Profile Data Update:Set Person Profile Completed Date:Order
            RockMigrationHelper.AddActionTypeAttributeValue("28C4977F-608D-464F-A126-F456EBE1ABE8", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"e862ba6a-8046-4e19-b7c6-51869bf2ba1e"); // Profile Update:Receive Profile Data Update:Set Person Profile Completed Date:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("28C4977F-608D-464F-A126-F456EBE1ABE8", "94689BDE-493E-4869-A614-2D54822D747C", @"{{ 'Now' | Date:'yyyy-MM-ddThh:mm:ss' }}"); // Profile Update:Receive Profile Data Update:Set Person Profile Completed Date:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("9FA01606-7419-4B88-BE44-E9649BC644E5", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Profile Update:Evaluate Expiration Date:Set Person Data Status - Profile Expired:Order
            RockMigrationHelper.AddActionTypeAttributeValue("9FA01606-7419-4B88-BE44-E9649BC644E5", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Profile Update:Evaluate Expiration Date:Set Person Data Status - Profile Expired:Active
            RockMigrationHelper.AddActionTypeAttributeValue("9FA01606-7419-4B88-BE44-E9649BC644E5", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"e276dd8b-cec2-4e87-a085-bc95ec23762a"); // Profile Update:Evaluate Expiration Date:Set Person Data Status - Profile Expired:Person
            RockMigrationHelper.AddActionTypeAttributeValue("9FA01606-7419-4B88-BE44-E9649BC644E5", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"68f7f4ab-26d3-463e-bd5a-d18da051fa1d"); // Profile Update:Evaluate Expiration Date:Set Person Data Status - Profile Expired:Person Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("9FA01606-7419-4B88-BE44-E9649BC644E5", "94689BDE-493E-4869-A614-2D54822D747C", @"16da3fed-2a24-482a-b0f7-54b3ff467082"); // Profile Update:Evaluate Expiration Date:Set Person Data Status - Profile Expired:Value|Attribute Value
            RockMigrationHelper.AddActionTypeAttributeValue("E8757EC8-458C-41E8-832E-4294862AB39B", "8DB7265C-177E-4924-BAF3-E7D6D2250A7B", @""); // Profile Update:Evaluate Expiration Date:Calculate Days Since Activation:Order
            RockMigrationHelper.AddActionTypeAttributeValue("E8757EC8-458C-41E8-832E-4294862AB39B", "20CD0B82-F75E-43EF-8EC1-26F9F846053A", @"8d0cb636-8a56-4793-83a8-9a83af7b9425"); // Profile Update:Evaluate Expiration Date:Calculate Days Since Activation:Initial Date Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("E8757EC8-458C-41E8-832E-4294862AB39B", "D5A4C63A-37CB-4003-9E85-77DD96EF6093", @"False"); // Profile Update:Evaluate Expiration Date:Calculate Days Since Activation:Active
            RockMigrationHelper.AddActionTypeAttributeValue("E8757EC8-458C-41E8-832E-4294862AB39B", "5369480E-FE99-4578-A82A-1FF5F078EF43", @"f0e1c0d4-657a-42d0-8156-d7ac65935aae"); // Profile Update:Evaluate Expiration Date:Calculate Days Since Activation:Destination Elapsed Attribute
            RockMigrationHelper.AddActionTypeAttributeValue("E8757EC8-458C-41E8-832E-4294862AB39B", "D7F80FBD-9880-458D-A2BE-12F77480DC4B", @"Days"); // Profile Update:Evaluate Expiration Date:Calculate Days Since Activation:Interval Type
            RockMigrationHelper.AddActionTypeAttributeValue("E68FCC5A-D33B-4FAB-86A8-A9444091D4DB", "3463643B-2601-4116-B47C-A0D7D21D4BF4", @"False"); // Profile Update:Evaluate Expiration Date:Cease Expiration Check:Active
            RockMigrationHelper.AddActionTypeAttributeValue("E68FCC5A-D33B-4FAB-86A8-A9444091D4DB", "9A2EB793-E272-440F-B7CB-3BFBFE037A0E", @""); // Profile Update:Evaluate Expiration Date:Cease Expiration Check:Order
            #endregion

            #region Bio block update for Profile Update workflow
            // Update the Bio block's WorkflowAction attribute value to include this new ProfileUpdate
            RockMigrationHelper.AddBlockAttributeValue("B5C1FDB6-0224-43E4-8E26-6B2EAF86253A", "7197A0FB-B330-43C4-8E62-F3C14F649813", "A84EA226-1CB2-453B-87B6-81F5360BAD3D", appendToExisting: true);
            #endregion

            #region Workflow Navigation block adding Data Integrity workflow category to list
            RockMigrationHelper.AddBlockAttributeValue("2D20CEC4-328E-4C2B-8059-78DFC49D8E35", "FB420F14-3D9D-4304-878F-124902E2CEAB", "BBAE05FD-8192-4616-A71E-903A927E0D90", appendToExisting: true);
            #endregion

            #region Modify Edit My Account to use Profile Update block
            RockMigrationHelper.DeleteBlock("539A5E9C-DE5F-4991-BCC4-9C4AC3151C7B");
            RockMigrationHelper.UpdateBlockType("Profile Update", "Allows a person to edit their profile information.", "~/Plugins/org_willowcreek/ProfileUpdate/ProfileUpdate.ascx", "org_willowcreek > Profile Update", "A2BAC700-0B40-496C-87E7-3617555D1590");
            RockMigrationHelper.AddBlock("4A4655D1-BDD9-4ECE-A3F6-B655F0BDF9F5", "", "A2BAC700-0B40-496C-87E7-3617555D1590", "Profile Update", "Main", "", "", 0, "0859D9D3-9BCA-4CC8-8EBA-2980EF7C408D");
            #endregion
        }

        public override void Down()
        {
            #region Profile Update Page
            // Attrib for BlockType: Attribute Values:Attribute Order
            RockMigrationHelper.DeleteAttribute("B7EB7168-DEAD-4BD0-A854-B94BC5BDE06E");
            // Remove Block: Profile Update, from Page: Profile Update, Site: External Website
            RockMigrationHelper.DeleteBlock("4E28B62B-89E0-43D8-9F83-434DE1242230");
            RockMigrationHelper.DeleteBlockType("A2BAC700-0B40-496C-87E7-3617555D1590"); // Profile Update
            RockMigrationHelper.DeleteBlockType("7635A576-DF20-4E4E-AC31-FA7983F49727"); // Verify Profile
            RockMigrationHelper.DeleteBlockType("2C3E3C91-F7A9-4AAC-B989-81572A5A66EE"); // Send Profile Request
            RockMigrationHelper.DeletePage("4D3AE84F-A8F0-489A-8EB3-FA2AA419FD33"); //  Page: Profile Update, Layout: FullWidth, Site: External Website
            RockMigrationHelper.DeleteLayout("023D30A4-1AA9-45FD-B1F6-0162A243BBB6"); //  Layout: Blank, Site: External Website
            // Delete PageContext for Page:Workflow Configuration, Entity: Rock.Model.WorkflowType, Parameter: WorkflowTypeId
            RockMigrationHelper.DeletePageContext("E904932A-4551-4A5A-B6BF-EF60AD8E90E6");
            #endregion

            #region Send Profile Updates Page
            RockMigrationHelper.DeleteAttribute("75B3F944-3584-46A7-BD3E-1BFF2F6FABDA");
            RockMigrationHelper.DeleteAttribute("C66726CF-DBFC-4723-BEFA-90FEA3F06B2C");
            RockMigrationHelper.DeleteBlock("5A491DA0-7EFC-4458-ACA9-F877CC4C4595");
            RockMigrationHelper.DeleteBlockType("2C7774DF-BA0E-4726-BF81-895519327325");
            RockMigrationHelper.DeletePage("FB7E3B44-975F-45A8-83B7-5E8F745AA65D"); //  Page: Send Profile Updates            
            #endregion

            #region Defined Types
            RockMigrationHelper.DeleteDefinedValue("B4555705-DF02-41C5-AF62-288C37082C5E");
            RockMigrationHelper.DeleteDefinedValue("B95A08D4-B77E-4480-B44E-68F37F791499");
            RockMigrationHelper.DeleteDefinedValue("C2314362-88C4-4AD5-AF79-5437EA3E8DC5");
            RockMigrationHelper.DeleteDefinedValue("CD303961-3FE8-4796-90DA-717F3BCA7732");
            RockMigrationHelper.DeleteDefinedValue("E027A12D-F7A8-404E-A937-E1078509958F");
            RockMigrationHelper.DeleteDefinedValue("F048DE3B-007C-4137-8979-278C6D976BE0");
            RockMigrationHelper.DeleteDefinedValue("8D2C5C5C-8611-42CF-903E-D53CE464819E");
            RockMigrationHelper.DeleteDefinedValue("8AC1B8FC-05DD-4508-99D5-C803B9942F7D");
            RockMigrationHelper.DeleteDefinedValue("79AF94B8-4792-47CD-A8BF-1B87DAD8FC60");
            RockMigrationHelper.DeleteDefinedValue("78119C1D-E9E2-4405-AF61-4A72CBD2EB90");
            RockMigrationHelper.DeleteDefinedValue("741FBF51-81B7-486D-BC6F-5B64112368E6");
            RockMigrationHelper.DeleteDefinedValue("62129766-7A8E-462C-87E0-9A192FEE6ADE");	
            RockMigrationHelper.DeleteDefinedValue("501F5D97-04AA-412F-AC16-7AD4F916174E");
            RockMigrationHelper.DeleteDefinedValue("2D0C183C-D767-43DA-95A1-B2F7C8D44E28");
            RockMigrationHelper.DeleteDefinedValue("10BE2461-6AD0-44AF-A1BD-0BC200029B7F");
            RockMigrationHelper.DeleteDefinedValue("0C862086-9911-47FD-ACC3-1AA63DE80686");
            RockMigrationHelper.DeleteDefinedValue("0D0EDB72-DC72-4A5D-8242-C910F90C5C7B");
            RockMigrationHelper.DeleteDefinedValue("42F6A264-3B37-425E-999C-58307FECBCE8");
            RockMigrationHelper.DeleteDefinedValue("436CAA54-3A30-4C34-8E4C-995EEC7EDF80");
            RockMigrationHelper.DeleteDefinedValue("4CF2647A-C06C-45CB-B0BA-0A482CDA6851");
            RockMigrationHelper.DeleteDefinedValue("5467B69F-07F5-4750-BD32-D3CFDFAA9623");
            RockMigrationHelper.DeleteDefinedValue("8F2E5EC2-E2C9-40DA-8EC9-3E7C87995ED0");
            RockMigrationHelper.DeleteDefinedValue("A0E9089A-0F3B-4E09-A9FA-60253783274F");
            RockMigrationHelper.DeleteDefinedValue("D3ACA359-DC87-44D4-8DD1-780C655065B1");
            RockMigrationHelper.DeleteDefinedValue("DAE70C22-7AD3-420F-97E6-210FA8639204");
            RockMigrationHelper.DeleteDefinedValue("DBF87C3C-A8D5-492A-A1C4-BDCBDABDDD71");
            RockMigrationHelper.DeleteDefinedType("0E7BBEA9-0804-4481-846C-1DC62A76390D");
            RockMigrationHelper.DeleteDefinedType("7FE643F5-C790-4FD7-A1B4-19BE9220139D");
            #endregion

            #region New Person Attributes
            RockMigrationHelper.DeleteAttribute("DFBDEFD8-6A90-4DD7-9D50-68BA77F47815");
            RockMigrationHelper.DeleteAttribute("ECF1AD4F-0BBA-44D9-847D-D5EE61D9B10C");
            RockMigrationHelper.DeleteAttribute("3F133F28-0ACF-4F85-AFFD-7A21656EF7EE");
            RockMigrationHelper.DeleteAttribute("69B3A7AC-DFA1-4F09-9713-0640FADBB469");
            #endregion

            #region Profile Update Workflow
            #endregion

            #region Bio block attribute removal
            // As far as I can tell, there is no single item removal delete for a multi-value attribute
            #endregion
        }
    }
}
