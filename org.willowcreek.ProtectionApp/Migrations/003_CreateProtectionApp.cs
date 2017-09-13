using System;
using System.Linq;
using Rock.Plugin;

namespace org.willowcreek.ProtectionApp.Migrations
{
    [MigrationNumber(3, "1.2")]
    public class CreateProtectionApp : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            try
            {
                //build the pages
                BuildPages();
                #region Protection App Workflow
                RockMigrationHelper.UpdateEntityType("Rock.Model.Workflow", "3540E9A7-FE30-43A9-8B0A-A372B63DFC93", true, true);

                RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActivity", "2CB52ED0-CB06-4D62-9E2C-73B60AFA4C9F", true, true);

                RockMigrationHelper.UpdateEntityType("Rock.Model.WorkflowActionType", "23E3273A-B137-48A3-9AFF-C8DC832DDCA6", true, true);

                RockMigrationHelper.UpdateEntityType("org.willowcreek.Workflow.Action.Elapse", "1FC3F882-9EFD-4959-AB24-3DDE96F837BC", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.ActivateActions", "699756EF-28EB-444B-BD28-15F0A167E614", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.ActivateActivity", "38907A90-1634-4A93-8017-619326A4A582", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.AssignActivityFromAttributeValue", "F100A31F-E93A-4C7A-9E55-0FAF41A101C4", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.AssignActivityToGroup", "DB2D8C44-6E57-4B45-8973-5DE327D61554", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.CompleteActivity", "DA2E4974-8F7D-4472-AD0B-9B65820543F0", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.CompleteWorkflow", "EEDA4318-F014-4A46-9C76-4C052EF81AA1", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.DeleteWorkflow", "76B805A5-F7D4-4F0E-934D-F2FBE5A7C9F4", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.RunSQL", "A41216D6-6FB0-4019-B222-2C29B4519CF4", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SendEmail", "66197B01-D1F0-4924-A315-47AD54E030DE", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeToCurrentPerson", "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeFromEntity", "972F19B9-598B-474B-97A4-50E56E7B59D2", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetAttributeValue", "C789E457-0783-44B3-9D8F-2EBAB5F11110", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetPersonAttribute", "D12A74D4-1467-4F3A-80FA-CFA3289128E8", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetStatus", "96D371A7-A291-4F8F-8B38-B8F72CE5407E", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.SetWorkflowName", "36005473-BD5D-470B-B28D-98E6D7ED808D", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.UserEntryForm", "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", false, true);

                RockMigrationHelper.UpdateEntityType("Rock.Workflow.Action.WriteToLog", "017DC868-AF98-4E03-B3A7-75EB5CA3BE02", false, true);

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("017DC868-AF98-4E03-B3A7-75EB5CA3BE02", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "61193203-64B0-4870-847F-33E0170D9382"); // Rock.Workflow.Action.WriteToLog:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("017DC868-AF98-4E03-B3A7-75EB5CA3BE02", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "54B3175B-193E-4B9A-9136-9145E864980D"); // Rock.Workflow.Action.WriteToLog:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("017DC868-AF98-4E03-B3A7-75EB5CA3BE02", "C28C7BF3-A552-4D77-9408-DEDCF760CED0", "Message", "Message", "The message to write to the log. <span class='tip tip-lava'></span>", 0, @"", "BAB22829-E0D0-47CB-9681-06A459888842"); // Rock.Workflow.Action.WriteToLog:Message

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "D5A4C63A-37CB-4003-9E85-77DD96EF6093"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Destination Elapsed Attribute", "DestinationElapsedAttribute", "The attribute that will contain the value of the elapsed interval.", 1, @"", "5369480E-FE99-4578-A82A-1FF5F078EF43"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Destination Elapsed Attribute

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Initial Date Attribute", "InitialDateAttribute", "The attribute that contains the start of the interval for which elapsed time will be calculated.  This attribute must exist on the Workflow.", 0, @"", "20CD0B82-F75E-43EF-8EC1-26F9F846053A"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Initial Date Attribute

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Interval Type", "IntervalType", "Select an interval with which to measure elapsed time.", 2, @"Days", "D7F80FBD-9880-458D-A2BE-12F77480DC4B"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Interval Type

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("1FC3F882-9EFD-4959-AB24-3DDE96F837BC", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "8DB7265C-177E-4924-BAF3-E7D6D2250A7B"); // org.willowcreek.ProfileUpdate.Workflow.Action.Elapse:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "DE9CB292-4785-4EA3-976D-3826F91E9E98"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person Attribute", "PersonAttribute", "The attribute to set to the currently logged in person.", 0, @"", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Person Attribute

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8"); // Rock.Workflow.Action.SetAttributeToCurrentPerson:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("36005473-BD5D-470B-B28D-98E6D7ED808D", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "0A800013-51F7-4902-885A-5BE215D67D3D"); // Rock.Workflow.Action.SetWorkflowName:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("36005473-BD5D-470B-B28D-98E6D7ED808D", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Text Value|Attribute Value", "NameValue", "The value to use for the workflow's name. <span class='tip tip-lava'></span>", 1, @"", "93852244-A667-4749-961A-D47F88675BE4"); // Rock.Workflow.Action.SetWorkflowName:Text Value|Attribute Value

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("36005473-BD5D-470B-B28D-98E6D7ED808D", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "5D95C15A-CCAE-40AD-A9DD-F929DA587115"); // Rock.Workflow.Action.SetWorkflowName:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("38907A90-1634-4A93-8017-619326A4A582", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "E8ABD802-372C-47BE-82B1-96F50DB5169E"); // Rock.Workflow.Action.ActivateActivity:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("38907A90-1634-4A93-8017-619326A4A582", "739FD425-5B8C-4605-B775-7E4D9D4C11DB", "Activity", "Activity", "The activity type to activate", 0, @"", "02D5A7A5-8781-46B4-B9FC-AF816829D240"); // Rock.Workflow.Action.ActivateActivity:Activity

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("38907A90-1634-4A93-8017-619326A4A582", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "3809A78C-B773-440C-8E3F-A8E81D0DAE08"); // Rock.Workflow.Action.ActivateActivity:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE"); // Rock.Workflow.Action.UserEntryForm:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("486DC4FA-FCBC-425F-90B0-E606DA8A9F68", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "C178113D-7C86-4229-8424-C6D0CF4A7E23"); // Rock.Workflow.Action.UserEntryForm:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Body", "Body", "The body of the email that should be sent. <span class='tip tip-lava'></span> <span class='tip tip-html'></span>", 3, @"", "4D245B9E-6B03-46E7-8482-A51FBA190E4D"); // Rock.Workflow.Action.SendEmail:Body

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "36197160-7D3D-490D-AB42-7E29105AFE91"); // Rock.Workflow.Action.SendEmail:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "From Email Address|Attribute Value", "From", "The email address or an attribute that contains the person or email address that email should be sent from (will default to organization email). <span class='tip tip-lava'></span>", 0, @"", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC"); // Rock.Workflow.Action.SendEmail:From Email Address|Attribute Value

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Send To Email Address|Attribute Value", "To", "The email address or an attribute that contains the person or email address that email should be sent to. <span class='tip tip-lava'></span>", 1, @"", "0C4C13B8-7076-4872-925A-F950886B5E16"); // Rock.Workflow.Action.SendEmail:Send To Email Address|Attribute Value

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Subject", "Subject", "The subject that should be used when sending email. <span class='tip tip-lava'></span>", 2, @"", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386"); // Rock.Workflow.Action.SendEmail:Subject

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("66197B01-D1F0-4924-A315-47AD54E030DE", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "D1269254-C15A-40BD-B784-ADCC231D3950"); // Rock.Workflow.Action.SendEmail:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("699756EF-28EB-444B-BD28-15F0A167E614", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "A134F1A7-3824-43E0-9EB1-22C899B795BD"); // Rock.Workflow.Action.ActivateActions:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("699756EF-28EB-444B-BD28-15F0A167E614", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "5DA71523-E8B0-4C4D-89A4-B47945A22A0C"); // Rock.Workflow.Action.ActivateActions:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("76B805A5-F7D4-4F0E-934D-F2FBE5A7C9F4", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "361A1EC8-FFD0-4880-AF68-91DC0E0D7CDC"); // Rock.Workflow.Action.DeleteWorkflow:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("76B805A5-F7D4-4F0E-934D-F2FBE5A7C9F4", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "79D23F8B-0DC8-4B48-8A86-AEA48B396C82"); // Rock.Workflow.Action.DeleteWorkflow:Order

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

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("D12A74D4-1467-4F3A-80FA-CFA3289128E8", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184"); // Rock.Workflow.Action.SetPersonAttribute:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("D12A74D4-1467-4F3A-80FA-CFA3289128E8", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Person", "Person", "Workflow attribute that contains the person to update.", 0, @"", "E456FB6F-05DB-4826-A612-5B704BC4EA13"); // Rock.Workflow.Action.SetPersonAttribute:Person

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("D12A74D4-1467-4F3A-80FA-CFA3289128E8", "3B1D93D7-9414-48F9-80E5-6A3FC8F94C20", "Value|Attribute Value", "Value", "The value or attribute value to set the person attribute to. <span class='tip tip-lava'></span>", 2, @"", "94689BDE-493E-4869-A614-2D54822D747C"); // Rock.Workflow.Action.SetPersonAttribute:Value|Attribute Value

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("D12A74D4-1467-4F3A-80FA-CFA3289128E8", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "Person Attribute", "PersonAttribute", "The person attribute that should be updated with the provided value.", 1, @"", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762"); // Rock.Workflow.Action.SetPersonAttribute:Person Attribute

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("D12A74D4-1467-4F3A-80FA-CFA3289128E8", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B"); // Rock.Workflow.Action.SetPersonAttribute:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("DA2E4974-8F7D-4472-AD0B-9B65820543F0", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "C5190778-2BB7-45D3-B387-D9AB205412E0"); // Rock.Workflow.Action.CompleteActivity:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("DA2E4974-8F7D-4472-AD0B-9B65820543F0", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "C632E229-3688-40ED-895C-6D2BCED3BD65"); // Rock.Workflow.Action.CompleteActivity:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("DB2D8C44-6E57-4B45-8973-5DE327D61554", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "C0D75D1A-16C5-4786-A1E0-25669BEE8FE9"); // Rock.Workflow.Action.AssignActivityToGroup:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("DB2D8C44-6E57-4B45-8973-5DE327D61554", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "041B7B51-A694-4AF5-B455-64D0DE7160A2"); // Rock.Workflow.Action.AssignActivityToGroup:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("DB2D8C44-6E57-4B45-8973-5DE327D61554", "CC34CE2C-0B0E-4BB3-9549-454B2A7DF218", "Group", "Group", "Select group type, then group, to set the group to assign this activity to.", 0, @"", "BBFAD050-5968-4D11-8887-2FF877D8C8AB"); // Rock.Workflow.Action.AssignActivityToGroup:Group

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C"); // Rock.Workflow.Action.CompleteWorkflow:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("EEDA4318-F014-4A46-9C76-4C052EF81AA1", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "25CAD4BE-5A00-409D-9BAB-E32518D89956"); // Rock.Workflow.Action.CompleteWorkflow:Order

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F100A31F-E93A-4C7A-9E55-0FAF41A101C4", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Active", "Active", "Should Service be used?", 0, @"False", "E0F7AB7E-7761-4600-A099-CB14ACDBF6EF"); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Active

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F100A31F-E93A-4C7A-9E55-0FAF41A101C4", "33E6DF69-BDFA-407A-9744-C175B60643AE", "Attribute", "Attribute", "The person or group attribute value to assign this activity to.", 0, @"", "FBADD25F-D309-4512-8430-3CC8615DD60E"); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Attribute

                RockMigrationHelper.UpdateWorkflowActionEntityAttribute("F100A31F-E93A-4C7A-9E55-0FAF41A101C4", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Order", "Order", "The order that this service should be used (priority)", 0, @"", "7A6B605D-7FB1-4F48-AF35-5A0683FB1CDA"); // Rock.Workflow.Action.AssignActivityFromAttributeValue:Order

                RockMigrationHelper.UpdateWorkflowType(false, true, "Protection Application", "This workflow creates and email request containing a link with which a member can fill out a Protection Application. The landing page is a custom block with the Protection Application form.", "6F8A431C-BEBD-4D33-AAD6-1D70870329C2", "Request", "fa fa-shield", 0, true, 3, "2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6"); // Protection Application

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "1B71FEF4-201F-4D53-8C60-2DF21F1985ED", "Campus", "Campus", "If included, the campus name will be used as the Billing Reference Code for the request (optional)", 7, @"", "7F159C6C-32D5-4E5D-8C14-40E89EC7FD30"); // Protection Application:Campus

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "Checked Attribute", "CheckedAttribute", "The person attribute that indicates if person has a valid background check (passed)", 1, @"", "DEEB85E4-256A-4C9E-8B5A-315D0B75111C"); // Protection Application:Checked Attribute

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "Date Attribute", "DateAttribute", "The person attribute the stores the date background check was completed", 2, @"", "66F2D86F-87B7-4E12-9F54-8B17EF764047"); // Protection Application:Date Attribute

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Type", "PackageType", "Value should be the type of background check to request from the vendor.", 9, @"", "5CEA4F8C-A1B8-45F5-AAC1-93CB6F6C87BC"); // Protection Application:Type

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Person", "Person", "The person who the request should be initiated for", 6, @"", "C495D2E1-9FCC-477B-B8C3-C92F28993354"); // Protection Application:Person

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "B50968BD-7643-4288-9237-6E89D2065363", "Questionnaire", "Questionnaire", "Assigns a protection app's questionnaire to the workflow.", 15, @"", "6F2D49B0-CCB8-46E7-8E2E-3021A4BFA211"); // Protection Application:Questionnaire

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "C28C7BF3-A552-4D77-9408-DEDCF760CED0", "Reason", "Reason", "A brief description of the reason that a background check is being requested", 10, @"", "B3103D24-5917-4711-94D6-DCEA9557F192"); // Protection Application:Reason

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Reference 1", "Reference1", "The first reference of the protection application.", 16, @"", "ECB35891-3E6F-4B66-BBD7-5D6EB2052136"); // Protection Application:Reference 1

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Reference 1 Complete", "Reference1Complete", "", 19, @"False", "59EFEF9D-FFFB-40A4-98A9-320964117EF6"); // Protection Application:Reference 1 Complete

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Reference 2", "Reference2", "The second reference of the protection application.", 17, @"", "887A729F-615B-477E-A4E2-E407EA0C6A45"); // Protection Application:Reference 2

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Reference 2 Complete", "Reference2Complete", "", 20, @"False", "F1083251-6C9B-4F8C-80A0-E16599C6D3B7"); // Protection Application:Reference 2 Complete

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Reference 3", "Reference3", "The third reference of the protection application", 18, @"", "D7F8EC15-22B4-4769-88E1-02163379FDE2"); // Protection Application:Reference 3

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Reference 3 Complete", "Reference3Complete", "", 21, @"False", "229615BF-EBE9-4930-88D7-50D9C51B39F0"); // Protection Application:Reference 3 Complete

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "C403E219-A56B-439E-9D50-9302DFE760CF", "Report", "Report", "The downloaded background check report", 14, @"", "4D5747BB-C182-4172-8C3E-9B3C063AAC18"); // Protection Application:Report

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "C0D0D7E2-C3B0-4004-ABEA-4BBFAD10D5D2", "Report Link", "ReportLink", "The location (URL) of the background report result", 12, @"", "B42B51B3-C18C-4F79-8F82-8B77A75D8FE0"); // Protection Application:Report Link

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Report Recommendation", "ReportRecommendation", "Providers recommendation ( if any )", 13, @"", "F6D84E4B-30A2-4104-B879-C76C1822E12E"); // Protection Application:Report Recommendation

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "7525C4CB-EE6B-41D4-9B64-A08048D5A5C0", "Report Status", "ReportStatus", "The result status of the background check", 11, @"", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3"); // Protection Application:Report Status

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Requester", "Requester", "The person initiating the request", 5, @"", "E48CD3DF-B284-4DD8-87E4-35F7D120FF7F"); // Protection Application:Requester

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "Result Attribute", "ResultAttribute", "The person attribute that stores the background check document", 4, @"", "87EEC269-3BDD-47B2-A991-E13F54EC6154"); // Protection Application:Result Attribute

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "36167F3E-8CB2-44F9-9022-102F171FBC9A", "SSN", "SSN", "The SSN of the person that the request is for", 8, @"", "696DFB2D-800C-49DE-B687-D5C868FB296C"); // Protection Application:SSN

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "Status Attribute", "StatusAttribute", "The person attribute that stores the background check status", 3, @"", "122EC290-65AA-4B0D-A70F-A5AF1DBF4C74"); // Protection Application:Status Attribute

                RockMigrationHelper.UpdateWorkflowTypeAttribute("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Workflow Start Time", "WorkflowStartTime", "This will be the stored value when the workflow sent the email and officially started the expiration period.", 0, @"", "727BE54D-B06D-4E1D-AE42-83B489EFB7BC"); // Protection Application:Workflow Start Time

                RockMigrationHelper.AddAttributeQualifier("DEEB85E4-256A-4C9E-8B5A-315D0B75111C", "allowmultiple", @"False", "FB84297E-EF6A-4380-B4DB-1EB24F0044D7"); // Protection Application:Checked Attribute:allowmultiple

                RockMigrationHelper.AddAttributeQualifier("DEEB85E4-256A-4C9E-8B5A-315D0B75111C", "entitytype", @"", "E957ED88-2360-4CF0-B1C3-38B3EB459D25"); // Protection Application:Checked Attribute:entitytype

                RockMigrationHelper.AddAttributeQualifier("66F2D86F-87B7-4E12-9F54-8B17EF764047", "allowmultiple", @"False", "C06B61AF-C10C-45AD-AD2B-36D97468D027"); // Protection Application:Date Attribute:allowmultiple

                RockMigrationHelper.AddAttributeQualifier("66F2D86F-87B7-4E12-9F54-8B17EF764047", "entitytype", @"", "1C2F5195-207E-4423-B904-9A164B35860F"); // Protection Application:Date Attribute:entitytype

                RockMigrationHelper.AddAttributeQualifier("5CEA4F8C-A1B8-45F5-AAC1-93CB6F6C87BC", "fieldtype", @"ddl", "AADD0295-A513-492D-BC60-C2704529E851"); // Protection Application:Type:fieldtype

                RockMigrationHelper.AddAttributeQualifier("5CEA4F8C-A1B8-45F5-AAC1-93CB6F6C87BC", "values", @"Basic,Plus", "324A0B1F-93DA-4846-82A5-0C42B8BB3BBC"); // Protection Application:Type:values

                RockMigrationHelper.AddAttributeQualifier("6F2D49B0-CCB8-46E7-8E2E-3021A4BFA211", "entityControlHelpTextFormat", @"", "1E6B0ECD-E005-4929-938F-85A325919C0F"); // Protection Application:Questionnaire:entityControlHelpTextFormat

                RockMigrationHelper.AddAttributeQualifier("B3103D24-5917-4711-94D6-DCEA9557F192", "numberofrows", @"", "4BC569DA-4D19-446E-A59E-2F2592ED2581"); // Protection Application:Reason:numberofrows

                RockMigrationHelper.AddAttributeQualifier("59EFEF9D-FFFB-40A4-98A9-320964117EF6", "falsetext", @"No", "219D6133-D25E-4E41-91C1-52E2F323F571"); // Protection Application:Reference 1 Complete:falsetext

                RockMigrationHelper.AddAttributeQualifier("59EFEF9D-FFFB-40A4-98A9-320964117EF6", "truetext", @"Yes", "AA47D894-6F69-4414-9A9E-9D9880A09D3B"); // Protection Application:Reference 1 Complete:truetext

                RockMigrationHelper.AddAttributeQualifier("F1083251-6C9B-4F8C-80A0-E16599C6D3B7", "falsetext", @"No", "14629A1C-E381-4105-BDD7-B7D10F18BCCA"); // Protection Application:Reference 2 Complete:falsetext

                RockMigrationHelper.AddAttributeQualifier("F1083251-6C9B-4F8C-80A0-E16599C6D3B7", "truetext", @"Yes", "47D890B2-596E-46A7-A46E-D46430E519C1"); // Protection Application:Reference 2 Complete:truetext

                RockMigrationHelper.AddAttributeQualifier("229615BF-EBE9-4930-88D7-50D9C51B39F0", "falsetext", @"No", "BD510F32-A573-496E-9B90-D418A0F4F361"); // Protection Application:Reference 3 Complete:falsetext

                RockMigrationHelper.AddAttributeQualifier("229615BF-EBE9-4930-88D7-50D9C51B39F0", "truetext", @"Yes", "17442DCA-7AA9-401F-9D24-32B5404CFF95"); // Protection Application:Reference 3 Complete:truetext

                RockMigrationHelper.AddAttributeQualifier("4D5747BB-C182-4172-8C3E-9B3C063AAC18", "binaryFileType", @"", "590A6425-2D81-491C-B184-04C1CE08261A"); // Protection Application:Report:binaryFileType

                RockMigrationHelper.AddAttributeQualifier("F6D84E4B-30A2-4104-B879-C76C1822E12E", "ispassword", @"False", "701014DE-ED88-4BE8-AC01-25F97168E827"); // Protection Application:Report Recommendation:ispassword

                RockMigrationHelper.AddAttributeQualifier("A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", "fieldtype", @"ddl", "E7C33B58-06DD-4B99-9D30-A427D186D35D"); // Protection Application:Report Status:fieldtype

                RockMigrationHelper.AddAttributeQualifier("A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", "values", @"Pass,Fail,Review", "27719917-B720-416E-84CF-114C8A63934B"); // Protection Application:Report Status:values

                RockMigrationHelper.AddAttributeQualifier("727BE54D-B06D-4E1D-AE42-83B489EFB7BC", "ispassword", @"False", "2B0A8722-DE3E-4090-A827-9DBE71A934D7"); // Protection Application:Workflow Start Time:ispassword

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Launch Request", "The first activity is responsible for collecting Person and/or Sender information and generating the Protection Applicaton Request", true, 0, "571CE67D-6496-4AE3-A0B1-77DA0767EEC8"); // Protection Application:Launch Request

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Receive Protection Application", "The second activity handles the user's clicking of the link in the email and arriving at the WorkflowEntry block for accepting the submitted Protection Application.  This activity will be set to active when the email is sent, so that the WorkflowEntry block will pick up where the workflow left off.", false, 2, "08962ECE-DA0E-4F22-B764-4E35A31CF27C"); // Protection Application:Receive Protection Application

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Evaluate Expiration Date", "The third activity runs as often as the workflow is active to check to see if the status of the workflow is still \"Email Sent\" after a certain period of time.", false, 1, "6F6471C4-D066-46FC-B9A3-ED95C586A5D2"); // Protection Application:Evaluate Expiration Date

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Approve Request", "Assigns the activity to security team and waits for their approval before submitting the request.", false, 9, "7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0"); // Protection Application:Approve Request

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Review Denial", "Provides the requester a way to add additional information for the security team to approve request.", false, 10, "73D0020B-BF87-4723-ADC9-C8E69566409D"); // Protection Application:Review Denial

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Submit Request", "Submits the background request to the selected provider for processing.", false, 11, "242C7833-FAE3-44E9-97CF-0BF20E3EE03A"); // Protection Application:Submit Request

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Process Result", "Evaluates the result of the background check received from the provider", false, 12, "7AEFF3A0-F5C2-4B0F-9981-60A4F437FFE5"); // Protection Application:Process Result

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Review Result", "Allows for review of the results from provider.", false, 13, "4AEAFD25-8CAC-4CD8-8D9F-F294C5AD30D9"); // Protection Application:Review Result

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Complete Request", "Notifies requester of result and updates person's record with result", false, 14, "9DABB5AD-95CB-4F2D-AB3B-1CA3630C1F5D"); // Protection Application:Complete Request

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Cancel Request", "Cancels the request prior to submitting to provider and deletes the workflow.", false, 15, "CD7B6CA6-ED4A-41F9-8A09-FC07BC684421"); // Protection Application:Cancel Request

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Reference 1", "", false, 3, "0014C961-D3FE-4D25-9183-12B5EB6E6E8A"); // Protection Application:Reference 1

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Reference 2", "", false, 5, "A65D98A1-E6BE-4819-B7F4-5F7B30CE98D3"); // Protection Application:Reference 2

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Reference 3", "", false, 7, "E6F91770-538A-407A-A57B-29B904BF6642"); // Protection Application:Reference 3

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Receive Reference 1", "", false, 4, "55311A4F-2AB1-4CC3-9C0A-47A15055354C"); // Protection Application:Receive Reference 1

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Receive Reference 2", "", false, 6, "C6AE73DD-7E67-423D-814F-5D047594B536"); // Protection Application:Receive Reference 2

                RockMigrationHelper.UpdateWorkflowActivityType("2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", true, "Receive Reference 3", "", false, 8, "BBABE055-2B13-4291-8383-16C473CE84FA"); // Protection Application:Receive Reference 3

                RockMigrationHelper.UpdateWorkflowActivityTypeAttribute("6F6471C4-D066-46FC-B9A3-ED95C586A5D2", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Days Since Activation", "DaysSinceActivation", "This activity variable will hold the result of the Elapse custom action that determines how many days gave passed since the Email was sent to the recipient for which we issued this Protection Application request.", 0, @"", "B9A1BC4D-09E9-4894-821B-624C36753458"); // Protection Application:Evaluate Expiration Date:Days Since Activation

                RockMigrationHelper.UpdateWorkflowActivityTypeAttribute("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "E4EAB7B2-0B76-429B-AFE4-AD86D7428C70", "Approver", "Approver", "Person who approved or denied this request", 1, @"", "0C2F0547-72E2-463E-9C81-A2479578D92E"); // Protection Application:Approve Request:Approver

                RockMigrationHelper.UpdateWorkflowActivityTypeAttribute("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "C28C7BF3-A552-4D77-9408-DEDCF760CED0", "Note", "Note", "Any notes that approver wants to provide to submitter for review", 0, @"", "EEE5A1DF-14AC-47D9-BC63-F2C17F5AADAD"); // Protection Application:Approve Request:Note

                RockMigrationHelper.UpdateWorkflowActivityTypeAttribute("4AEAFD25-8CAC-4CD8-8D9F-F294C5AD30D9", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Review Result", "ReviewResult", "The result of the review (Pass,Fail)", 0, @"", "17FD1EB6-B47C-4B70-B6EE-2CCED1106F49"); // Protection Application:Review Result:Review Result

                RockMigrationHelper.AddAttributeQualifier("EEE5A1DF-14AC-47D9-BC63-F2C17F5AADAD", "numberofrows", @"", "14FFE2B9-88DF-4496-8F6D-B5CC268CFD0B"); // Protection Application:Note:numberofrows

                RockMigrationHelper.AddAttributeQualifier("17FD1EB6-B47C-4B70-B6EE-2CCED1106F49", "ispassword", @"False", "C662D1C9-8642-46D6-8BEF-C5EAAF5F8570"); // Protection Application:Review Result:ispassword

                RockMigrationHelper.UpdateWorkflowActionForm(@"<h1>Background Request Details</h1>
<div class='alert alert-info'>
    {{CurrentPerson.NickName}}, the following background request has been submitted for your review.
    If you approve the request it will be sent to the background check provider for processing. If you
    deny the request, it will be sent back to the requester. If you deny the request, please add notes
    explaining why the request was denied.
</div>", @"", "Approve^c88fef94-95b9-444a-bc93-58e983f3c047^^The request has been submitted to provider for processing.|Deny^d6b809a9-c1cc-4ebb-816e-33d8c1e53ea4^^The requester will be notified that this request has been denied (along with the reason why).|", "88C7D1CC-3478-4562-A301-AE7D4D7FFF6D", true, "", "A54BE863-EF0F-4531-A0C3-23ED6A1A24C4"); // Protection Application:Approve Request:Approve or Deny

                RockMigrationHelper.UpdateWorkflowActionForm(@"<h1>Background Request Details</h1>
<p>
    {{CurrentPerson.NickName}}, please verify the information below to start the background
    request process.
</p>
{% if Workflow.WarnOfRecent == 'Yes' %}
    <div class='alert alert-warning'>
        Notice: It's been less than a year since this person's last background check was processed.
        Please make sure you want to continue with this request!
    </div>
{% endif %}
<hr />", @"", "Submit^fdc397cd-8b4a-436e-bea1-bce2e6717c03^7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0^Your request has been submitted successfully.|Cancel^8cf6e927-4fa5-4241-991c-391038b79631^CD7B6CA6-ED4A-41F9-8A09-FC07BC684421^The request has been cancelled.|", "", false, "", "8C8210E4-BCE5-4EE4-9714-E09EB496F165"); // Protection Application:Approve Request:Verify Details

                RockMigrationHelper.UpdateWorkflowActionForm(@"<h1>Background Request Details</h1>
<p>
    {{CurrentPerson.NickName}}, this request has come back from the approval process with the following results.
</p>

<div class='well'>
    <strong>Summary of Security Notes:</strong><br />
    <table class=' table table-condensed table-light margin-b-md'>
    	{% for activity in Workflow.Activities %}
    		{% if activity.ActivityType.Name == 'Approve Request' %}
    			<tr>
    				<td width='220'>{{activity.CompletedDateTime}}</td>
    				<td width='220'>{{activity.Approver}}</td>
    				<td>{{activity.Note}}</td>
    			</tr>
    		{% endif %}
    	{% endfor %}
    </table>
    
</div>
<hr />", @"", "Submit^fdc397cd-8b4a-436e-bea1-bce2e6717c03^7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0^The request has been submitted again to the security team for approval.|Cancel Request^8cf6e927-4fa5-4241-991c-391038b79631^CD7B6CA6-ED4A-41F9-8A09-FC07BC684421^The request has been cancelled.|", "88C7D1CC-3478-4562-A301-AE7D4D7FFF6D", true, "", "697CF00F-9834-4FA6-871D-B8E80D7A2001"); // Protection Application:Review Denial:Review

                RockMigrationHelper.UpdateWorkflowActionForm(@"<h1>Background Request Details</h1>
<div class='alert alert-info'>
    {{CurrentPerson.NickName}}, the following background request was submitted and completed, but requires
    your review. Please pass or fail this request. The requester will be notified and the person's record 
    will be updated to indicate the result you select.
</div>
<hr>", @"", "Pass^fdc397cd-8b4a-436e-bea1-bce2e6717c03^^The request has been marked as passed. Requester will be notified.|Fail^d6b809a9-c1cc-4ebb-816e-33d8c1e53ea4^^The request has been marked as failed. Requester will be notified.|", "88C7D1CC-3478-4562-A301-AE7D4D7FFF6D", true, "17FD1EB6-B47C-4B70-B6EE-2CCED1106F49", "BFD21326-84DE-44D6-B2F3-0FC42603F8A1"); // Protection Application:Review Result:Review Results

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "727BE54D-B06D-4E1D-AE42-83B489EFB7BC", 0, false, true, false, "678E4A72-33DF-4B5C-B8E2-E2D54731F087"); // Protection Application:Approve Request:Approve or Deny:Workflow Start Time

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "DEEB85E4-256A-4C9E-8B5A-315D0B75111C", 1, false, true, false, "B8E66460-DB3A-418B-AF21-1D2C883379E6"); // Protection Application:Approve Request:Approve or Deny:Checked Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "66F2D86F-87B7-4E12-9F54-8B17EF764047", 2, false, true, false, "411A16AF-BDD2-40DC-BA35-D4F6EEF81A5C"); // Protection Application:Approve Request:Approve or Deny:Date Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "122EC290-65AA-4B0D-A70F-A5AF1DBF4C74", 3, false, true, false, "136A6A08-2AA6-4F7C-BC9C-76ED742ED6C4"); // Protection Application:Approve Request:Approve or Deny:Status Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "87EEC269-3BDD-47B2-A991-E13F54EC6154", 4, false, true, false, "5392200D-5E4F-456B-884C-FE7AC1859833"); // Protection Application:Approve Request:Approve or Deny:Result Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "E48CD3DF-B284-4DD8-87E4-35F7D120FF7F", 5, true, true, false, "07D29B9D-EEDA-489B-B8DC-35E5E3D09D2F"); // Protection Application:Approve Request:Approve or Deny:Requester

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "C495D2E1-9FCC-477B-B8C3-C92F28993354", 6, true, true, false, "6464A753-0C07-4C3B-8E7D-96249C54EDFE"); // Protection Application:Approve Request:Approve or Deny:Person

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "7F159C6C-32D5-4E5D-8C14-40E89EC7FD30", 7, true, true, false, "6692B85B-8A7B-4B98-8BFB-A74DBD05D12B"); // Protection Application:Approve Request:Approve or Deny:Campus

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "696DFB2D-800C-49DE-B687-D5C868FB296C", 8, false, true, false, "1493F8BA-1A0A-48C3-B4BE-FD66BBDE26B6"); // Protection Application:Approve Request:Approve or Deny:SSN

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "5CEA4F8C-A1B8-45F5-AAC1-93CB6F6C87BC", 9, true, false, true, "8D2902EC-BC14-438F-9536-525E60EC209B"); // Protection Application:Approve Request:Approve or Deny:Type

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "B3103D24-5917-4711-94D6-DCEA9557F192", 10, true, true, false, "2E866917-5529-4742-81F6-62CBE79BA0EC"); // Protection Application:Approve Request:Approve or Deny:Reason

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 11, false, true, false, "AA794353-813F-4C8B-BFD7-D85EC3F80B11"); // Protection Application:Approve Request:Approve or Deny:Report Status

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "B42B51B3-C18C-4F79-8F82-8B77A75D8FE0", 12, false, true, false, "92CD6D68-6995-467F-A627-C9A8A34D02E0"); // Protection Application:Approve Request:Approve or Deny:Report Link

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "F6D84E4B-30A2-4104-B879-C76C1822E12E", 13, false, true, false, "712A9377-8048-4B12-BF41-3B3D5EE1EA0F"); // Protection Application:Approve Request:Approve or Deny:Report Recommendation

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "4D5747BB-C182-4172-8C3E-9B3C063AAC18", 14, false, true, false, "DC09ECE9-3113-449C-92DE-B69683355A59"); // Protection Application:Approve Request:Approve or Deny:Report

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "EEE5A1DF-14AC-47D9-BC63-F2C17F5AADAD", 15, true, false, false, "3046342B-3416-45DC-AA08-ED85A5F8223F"); // Protection Application:Approve Request:Approve or Deny:Note

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "0C2F0547-72E2-463E-9C81-A2479578D92E", 16, false, true, false, "E4CBC733-4A70-4C94-ABA7-234541CDF52B"); // Protection Application:Approve Request:Approve or Deny:Approver

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "6F2D49B0-CCB8-46E7-8E2E-3021A4BFA211", 17, false, true, false, "6114526F-B3C5-4B55-87F8-C0B29F184401"); // Protection Application:Approve Request:Approve or Deny:Questionnaire

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "ECB35891-3E6F-4B66-BBD7-5D6EB2052136", 18, false, true, false, "4D81A236-1AAE-4A8A-94E1-D42FC36DA3D5"); // Protection Application:Approve Request:Approve or Deny:Reference 1

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "887A729F-615B-477E-A4E2-E407EA0C6A45", 19, false, true, false, "607F09AE-E6FA-458F-803E-E8AC7E1A76E8"); // Protection Application:Approve Request:Approve or Deny:Reference 2

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "D7F8EC15-22B4-4769-88E1-02163379FDE2", 20, false, true, false, "CE8065AF-A536-4D97-AE00-B2D3C0D0A8B7"); // Protection Application:Approve Request:Approve or Deny:Reference 3

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "59EFEF9D-FFFB-40A4-98A9-320964117EF6", 21, false, true, false, "9051FF87-5780-4C6D-9363-12794FACF6AD"); // Protection Application:Approve Request:Approve or Deny:Reference 1 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "F1083251-6C9B-4F8C-80A0-E16599C6D3B7", 22, false, true, false, "4BB261E5-F27C-4462-866D-4CA23FF99FC1"); // Protection Application:Approve Request:Approve or Deny:Reference 2 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "229615BF-EBE9-4930-88D7-50D9C51B39F0", 23, false, true, false, "ABC5951C-9EF7-4667-8097-E3B45EEFF6C9"); // Protection Application:Approve Request:Approve or Deny:Reference 3 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "727BE54D-B06D-4E1D-AE42-83B489EFB7BC", 0, false, true, false, "8BD90AEB-C30B-4D8B-8B2A-EFEC503922A3"); // Protection Application:Approve Request:Verify Details:Workflow Start Time

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "DEEB85E4-256A-4C9E-8B5A-315D0B75111C", 1, false, true, false, "71C678E7-9F7A-47CD-905B-10C4F26D1332"); // Protection Application:Approve Request:Verify Details:Checked Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "66F2D86F-87B7-4E12-9F54-8B17EF764047", 2, false, true, false, "28CB5C1B-725D-4057-986E-F3313750AB8C"); // Protection Application:Approve Request:Verify Details:Date Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "122EC290-65AA-4B0D-A70F-A5AF1DBF4C74", 3, false, true, false, "0B8A3B6E-1EB0-4245-9DB2-73DDB47291FB"); // Protection Application:Approve Request:Verify Details:Status Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "87EEC269-3BDD-47B2-A991-E13F54EC6154", 4, false, true, false, "29A43F2A-0FC6-49DD-AA4F-501C57E8B758"); // Protection Application:Approve Request:Verify Details:Result Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "E48CD3DF-B284-4DD8-87E4-35F7D120FF7F", 5, false, true, false, "F0D6D011-2061-4B65-9628-38A95EB2E952"); // Protection Application:Approve Request:Verify Details:Requester

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "C495D2E1-9FCC-477B-B8C3-C92F28993354", 6, true, true, false, "5A1B3457-B5D2-4230-B5FC-E81BA00DCFFB"); // Protection Application:Approve Request:Verify Details:Person

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "7F159C6C-32D5-4E5D-8C14-40E89EC7FD30", 7, true, false, true, "3A2FF45D-375F-48F5-BD1B-633FC12F7A98"); // Protection Application:Approve Request:Verify Details:Campus

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "696DFB2D-800C-49DE-B687-D5C868FB296C", 8, true, false, true, "BA27B639-D49B-4A4E-B064-A8810B357FE8"); // Protection Application:Approve Request:Verify Details:SSN

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "5CEA4F8C-A1B8-45F5-AAC1-93CB6F6C87BC", 9, false, true, false, "E794E81E-86DA-4018-AC1C-0727642EF915"); // Protection Application:Approve Request:Verify Details:Type

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "B3103D24-5917-4711-94D6-DCEA9557F192", 10, true, false, false, "9F4D2279-19C9-4417-AEFB-759E297327CA"); // Protection Application:Approve Request:Verify Details:Reason

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 11, false, true, false, "30185D3C-9BBD-4973-A38E-5A9A78ED533B"); // Protection Application:Approve Request:Verify Details:Report Status

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "B42B51B3-C18C-4F79-8F82-8B77A75D8FE0", 12, false, true, false, "1F9018B4-BC86-4F8A-9618-92A962741FC1"); // Protection Application:Approve Request:Verify Details:Report Link

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "F6D84E4B-30A2-4104-B879-C76C1822E12E", 13, false, true, false, "74923042-393B-4336-94CF-2A107CF399B3"); // Protection Application:Approve Request:Verify Details:Report Recommendation

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "4D5747BB-C182-4172-8C3E-9B3C063AAC18", 14, false, true, false, "155863EF-4C7F-4DBE-914B-E3C2A9A7BAB0"); // Protection Application:Approve Request:Verify Details:Report

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "6F2D49B0-CCB8-46E7-8E2E-3021A4BFA211", 15, false, true, false, "38196488-D8B8-46AA-A4E2-4A2563C0F510"); // Protection Application:Approve Request:Verify Details:Questionnaire

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "ECB35891-3E6F-4B66-BBD7-5D6EB2052136", 16, false, true, false, "1CA14B97-A687-40C5-BCC2-97C90C97DA1F"); // Protection Application:Approve Request:Verify Details:Reference 1

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "887A729F-615B-477E-A4E2-E407EA0C6A45", 17, false, true, false, "00CAF968-1FED-42B6-BC6D-2C1A5F82A31C"); // Protection Application:Approve Request:Verify Details:Reference 2

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "D7F8EC15-22B4-4769-88E1-02163379FDE2", 18, false, true, false, "4AD4066B-4168-4191-A873-03A432365A82"); // Protection Application:Approve Request:Verify Details:Reference 3

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "EEE5A1DF-14AC-47D9-BC63-F2C17F5AADAD", 19, false, true, false, "384653C7-023B-4ED5-A83C-324565FE8134"); // Protection Application:Approve Request:Verify Details:Note

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "0C2F0547-72E2-463E-9C81-A2479578D92E", 20, false, true, false, "4789E2BF-1136-493E-9272-C2ACDE7C85D5"); // Protection Application:Approve Request:Verify Details:Approver

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "59EFEF9D-FFFB-40A4-98A9-320964117EF6", 21, false, true, false, "569CF318-554A-440D-9DC6-1104D6F64EB3"); // Protection Application:Approve Request:Verify Details:Reference 1 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "F1083251-6C9B-4F8C-80A0-E16599C6D3B7", 22, false, true, false, "0A7C98A4-0471-4999-A599-A33440782E79"); // Protection Application:Approve Request:Verify Details:Reference 2 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("8C8210E4-BCE5-4EE4-9714-E09EB496F165", "229615BF-EBE9-4930-88D7-50D9C51B39F0", 23, false, true, false, "9F6C2520-F999-49AF-95B1-714B2295C312"); // Protection Application:Approve Request:Verify Details:Reference 3 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "727BE54D-B06D-4E1D-AE42-83B489EFB7BC", 0, false, true, false, "C5FB4C4D-BDAF-4690-86F5-23F23B5B0E29"); // Protection Application:Review Denial:Review:Workflow Start Time

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "DEEB85E4-256A-4C9E-8B5A-315D0B75111C", 1, false, true, false, "E78F83DD-5B38-4AD0-A9F2-776425EFCB5C"); // Protection Application:Review Denial:Review:Checked Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "66F2D86F-87B7-4E12-9F54-8B17EF764047", 2, false, true, false, "6D2153CC-6012-4BDE-8359-11F7EBA395E2"); // Protection Application:Review Denial:Review:Date Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "122EC290-65AA-4B0D-A70F-A5AF1DBF4C74", 3, false, true, false, "9BCC5DD3-386B-4DFD-AF60-A72308753BBB"); // Protection Application:Review Denial:Review:Status Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "87EEC269-3BDD-47B2-A991-E13F54EC6154", 4, false, true, false, "9A3DA7D3-7530-4357-B68C-D0FA97CE43BF"); // Protection Application:Review Denial:Review:Result Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "E48CD3DF-B284-4DD8-87E4-35F7D120FF7F", 5, false, true, false, "5FC7B841-B21C-4441-AD31-3398BE560CDA"); // Protection Application:Review Denial:Review:Requester

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "C495D2E1-9FCC-477B-B8C3-C92F28993354", 6, true, true, false, "FA6CBED1-55F2-421B-B5DE-7C6AE363EFD6"); // Protection Application:Review Denial:Review:Person

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "7F159C6C-32D5-4E5D-8C14-40E89EC7FD30", 7, true, false, true, "5F1CB776-CED2-4BF0-97E4-559DAA1FAC50"); // Protection Application:Review Denial:Review:Campus

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "696DFB2D-800C-49DE-B687-D5C868FB296C", 8, true, false, false, "86CCD00C-03DF-4E22-B756-DF7B5DBAB9BE"); // Protection Application:Review Denial:Review:SSN

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "5CEA4F8C-A1B8-45F5-AAC1-93CB6F6C87BC", 9, true, false, true, "2FAF9992-BF64-4FC4-948B-4D074688DAE8"); // Protection Application:Review Denial:Review:Type

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "B3103D24-5917-4711-94D6-DCEA9557F192", 10, true, false, false, "B13E13D9-7436-4F6B-A810-CF59CB37DF31"); // Protection Application:Review Denial:Review:Reason

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 11, false, true, false, "1ED25D6E-3F86-4062-9EB3-79A3E1717ECC"); // Protection Application:Review Denial:Review:Report Status

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "B42B51B3-C18C-4F79-8F82-8B77A75D8FE0", 12, false, true, false, "5ACC5B24-B2D5-49C5-8F26-EAC2DBEBD3DB"); // Protection Application:Review Denial:Review:Report Link

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "F6D84E4B-30A2-4104-B879-C76C1822E12E", 13, false, true, false, "AC31A704-080E-436E-943E-7CFC9E4A97DA"); // Protection Application:Review Denial:Review:Report Recommendation

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "4D5747BB-C182-4172-8C3E-9B3C063AAC18", 14, false, true, false, "FB30C1C2-7ED3-499C-9011-EBE947C86071"); // Protection Application:Review Denial:Review:Report

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "6F2D49B0-CCB8-46E7-8E2E-3021A4BFA211", 15, false, true, false, "C6428EE5-0662-4BB2-8039-E17CB378B7F2"); // Protection Application:Review Denial:Review:Questionnaire

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "ECB35891-3E6F-4B66-BBD7-5D6EB2052136", 16, false, true, false, "713D9493-A325-4C3A-B7A8-C999590DF8E2"); // Protection Application:Review Denial:Review:Reference 1

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "887A729F-615B-477E-A4E2-E407EA0C6A45", 17, false, true, false, "4C0F6EBC-C45C-425E-ACD1-86A7575C9B96"); // Protection Application:Review Denial:Review:Reference 2

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "D7F8EC15-22B4-4769-88E1-02163379FDE2", 18, false, true, false, "E8E133D5-67D9-4F28-8F0A-77C78F7B230F"); // Protection Application:Review Denial:Review:Reference 3

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "59EFEF9D-FFFB-40A4-98A9-320964117EF6", 19, false, true, false, "19B65451-CD6F-42A1-988E-D92744FB155C"); // Protection Application:Review Denial:Review:Reference 1 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "F1083251-6C9B-4F8C-80A0-E16599C6D3B7", 20, false, true, false, "5C2BF438-0620-4AC4-A7FC-557D467BFC05"); // Protection Application:Review Denial:Review:Reference 2 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("697CF00F-9834-4FA6-871D-B8E80D7A2001", "229615BF-EBE9-4930-88D7-50D9C51B39F0", 21, false, true, false, "56F98674-CB8D-4C9A-8535-8CAD168A612F"); // Protection Application:Review Denial:Review:Reference 3 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "727BE54D-B06D-4E1D-AE42-83B489EFB7BC", 0, false, true, false, "3810F876-BC49-48B4-8D1E-1C1E4E46E7AD"); // Protection Application:Review Result:Review Results:Workflow Start Time

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "DEEB85E4-256A-4C9E-8B5A-315D0B75111C", 1, false, true, false, "FDDE220D-ECFA-464D-A473-9A88F697EBFA"); // Protection Application:Review Result:Review Results:Checked Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "66F2D86F-87B7-4E12-9F54-8B17EF764047", 2, false, true, false, "3D22ECEA-D9EA-422B-843F-ED5CB59D70D5"); // Protection Application:Review Result:Review Results:Date Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "122EC290-65AA-4B0D-A70F-A5AF1DBF4C74", 3, false, true, false, "79B2BB1F-BE72-47BF-B058-21B0C0FC55A3"); // Protection Application:Review Result:Review Results:Status Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "87EEC269-3BDD-47B2-A991-E13F54EC6154", 4, false, true, false, "580E0C3F-DD43-4167-9B16-F56D60369391"); // Protection Application:Review Result:Review Results:Result Attribute

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "E48CD3DF-B284-4DD8-87E4-35F7D120FF7F", 5, true, true, false, "49868358-EB81-4963-BE75-B3EB4EA4832B"); // Protection Application:Review Result:Review Results:Requester

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "C495D2E1-9FCC-477B-B8C3-C92F28993354", 6, true, true, false, "BE3512CF-9F16-4108-8CC7-70BF18110C84"); // Protection Application:Review Result:Review Results:Person

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "7F159C6C-32D5-4E5D-8C14-40E89EC7FD30", 7, true, true, false, "23CE2E89-F32B-4F38-B0BB-73A0EC5DDE26"); // Protection Application:Review Result:Review Results:Campus

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "696DFB2D-800C-49DE-B687-D5C868FB296C", 8, false, true, false, "645641A6-2337-4AAF-987D-B200C303F00A"); // Protection Application:Review Result:Review Results:SSN

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "5CEA4F8C-A1B8-45F5-AAC1-93CB6F6C87BC", 9, false, true, false, "1CB4EE91-1ED8-40B3-8FFF-3E8E8EF2AFE2"); // Protection Application:Review Result:Review Results:Type

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "B3103D24-5917-4711-94D6-DCEA9557F192", 10, true, true, false, "2C212F54-75CD-4F0D-8B0B-EABA566F176C"); // Protection Application:Review Result:Review Results:Reason

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 11, false, true, false, "7FF87133-E07B-43C8-AEDE-83EFABE5CC6B"); // Protection Application:Review Result:Review Results:Report Status

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "B42B51B3-C18C-4F79-8F82-8B77A75D8FE0", 12, false, true, false, "0E6D32E4-44CB-4CC9-983F-74F0A091BA8D"); // Protection Application:Review Result:Review Results:Report Link

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "F6D84E4B-30A2-4104-B879-C76C1822E12E", 13, true, true, false, "1BB0249D-CDC1-4A87-BEE6-5ED9AA374477"); // Protection Application:Review Result:Review Results:Report Recommendation

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "4D5747BB-C182-4172-8C3E-9B3C063AAC18", 14, true, true, false, "22256BD1-58F8-4F5D-949A-FF65FCBEB71D"); // Protection Application:Review Result:Review Results:Report

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "17FD1EB6-B47C-4B70-B6EE-2CCED1106F49", 15, false, true, false, "94700BDB-51EC-44C7-8188-DC4967DB5A5E"); // Protection Application:Review Result:Review Results:Review Result

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "6F2D49B0-CCB8-46E7-8E2E-3021A4BFA211", 16, false, true, false, "B535A6AB-9FFE-45CC-9481-388FD4912641"); // Protection Application:Review Result:Review Results:Questionnaire

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "ECB35891-3E6F-4B66-BBD7-5D6EB2052136", 17, false, true, false, "B960C6EA-436E-4BCD-8EE8-7743F49A8ED4"); // Protection Application:Review Result:Review Results:Reference 1

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "887A729F-615B-477E-A4E2-E407EA0C6A45", 18, false, true, false, "E0CA3028-DD41-44A3-875A-9954C0AD2F1E"); // Protection Application:Review Result:Review Results:Reference 2

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "D7F8EC15-22B4-4769-88E1-02163379FDE2", 19, false, true, false, "421100C7-B5DC-4CC3-9B3B-63E6B6108C3C"); // Protection Application:Review Result:Review Results:Reference 3

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "59EFEF9D-FFFB-40A4-98A9-320964117EF6", 20, false, true, false, "6919E6B8-B83C-4DD1-9BE2-E455CBF587D5"); // Protection Application:Review Result:Review Results:Reference 1 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "F1083251-6C9B-4F8C-80A0-E16599C6D3B7", 21, false, true, false, "E0C1E7C0-AEA2-4E90-A8BD-9F888650B547"); // Protection Application:Review Result:Review Results:Reference 2 Complete

                RockMigrationHelper.UpdateWorkflowActionFormAttribute("BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "229615BF-EBE9-4930-88D7-50D9C51B39F0", 22, false, true, false, "B569DC78-7924-4E4C-B289-96756E770AD9"); // Protection Application:Review Result:Review Results:Reference 3 Complete

                RockMigrationHelper.UpdateWorkflowActionType("73D0020B-BF87-4723-ADC9-C8E69566409D", "Assign to Requester", 1, "F100A31F-E93A-4C7A-9E55-0FAF41A101C4", true, false, "", "", 1, "", "ABE7D8F5-BEFA-4C6E-8636-FE195A1E361F"); // Protection Application:Review Denial:Assign to Requester

                RockMigrationHelper.UpdateWorkflowActionType("242C7833-FAE3-44E9-97CF-0BF20E3EE03A", "Submit Request", 1, "699756EF-28EB-444B-BD28-15F0A167E614", true, false, "", "", 1, "", "14503B77-3209-47F1-BF4F-9D38A7040B9C"); // Protection Application:Submit Request:Submit Request

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Set Warning", 4, "A41216D6-6FB0-4019-B222-2C29B4519CF4", true, false, "", "", 1, "", "4D78CCC7-92E7-4ADA-B862-92E777743C59"); // Protection Application:Launch Request:Set Warning

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Set Workflow Start Time", 5, "C789E457-0783-44B3-9D8F-2EBAB5F11110", true, false, "", "", 1, "", "8AF5EEF9-5D4E-465E-BBF0-5C439248F0AA"); // Protection Application:Launch Request:Set Workflow Start Time

                RockMigrationHelper.UpdateWorkflowActionType("4AEAFD25-8CAC-4CD8-8D9F-F294C5AD30D9", "Update Result", 3, "C789E457-0783-44B3-9D8F-2EBAB5F11110", true, false, "", "", 1, "", "251BF993-F08C-4C6B-AAD8-10FCD0187189"); // Protection Application:Review Result:Update Result

                RockMigrationHelper.UpdateWorkflowActionType("6F6471C4-D066-46FC-B9A3-ED95C586A5D2", "Calculate Days Since Activation", 0, "1FC3F882-9EFD-4959-AB24-3DDE96F837BC", true, false, "", "", 1, "", "6E751023-1FDB-4AD5-B560-665B24E6F4E5"); // Protection Application:Evaluate Expiration Date:Calculate Days Since Activation

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Send Protection Application Request", 6, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "1DAB22E0-7FA3-4C84-8514-E6653A32BF21"); // Protection Application:Launch Request:Send Protection Application Request

                RockMigrationHelper.UpdateWorkflowActionType("9DABB5AD-95CB-4F2D-AB3B-1CA3630C1F5D", "Notify Requester", 3, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "E9D1E0FC-041B-4A16-933F-945F15E16F60"); // Protection Application:Complete Request:Notify Requester

                RockMigrationHelper.UpdateWorkflowActionType("0014C961-D3FE-4D25-9183-12B5EB6E6E8A", "Send Reference 1 Email", 1, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "BB444AC8-301B-4BDF-9D67-53A039BA0FB9"); // Protection Application:Reference 1:Send Reference 1 Email

                RockMigrationHelper.UpdateWorkflowActionType("A65D98A1-E6BE-4819-B7F4-5F7B30CE98D3", "Send Reference 2 Email", 1, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "21E45EA9-D444-4E14-9598-3B8FC728F137"); // Protection Application:Reference 2:Send Reference 2 Email

                RockMigrationHelper.UpdateWorkflowActionType("E6F91770-538A-407A-A57B-29B904BF6642", "Send Reference 3 Email", 1, "66197B01-D1F0-4924-A315-47AD54E030DE", true, false, "", "", 1, "", "7A1924EC-3EF4-4CA5-A02B-F1AAF0A9D86E"); // Protection Application:Reference 3:Send Reference 3 Email

                RockMigrationHelper.UpdateWorkflowActionType("9DABB5AD-95CB-4F2D-AB3B-1CA3630C1F5D", "Complete Workflow", 4, "EEDA4318-F014-4A46-9C76-4C052EF81AA1", true, false, "", "", 1, "", "0D1E4FAF-D21A-4E2D-BEAE-3684F8429FE7"); // Protection Application:Complete Request:Complete Workflow

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Set Person", 1, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "66CAFE61-19F6-4204-A3D6-9E97F97F665E"); // Protection Application:Launch Request:Set Person

                RockMigrationHelper.UpdateWorkflowActionType("08962ECE-DA0E-4F22-B764-4E35A31CF27C", "Set Questionnaire", 0, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "DC717FEB-8AD5-4E15-A022-DA0C237AB2A8"); // Protection Application:Receive Protection Application:Set Questionnaire

                RockMigrationHelper.UpdateWorkflowActionType("0014C961-D3FE-4D25-9183-12B5EB6E6E8A", "Set Reference 1", 0, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "4920E6C3-EA3A-4C1C-A2DF-F7ABD54B0277"); // Protection Application:Reference 1:Set Reference 1

                RockMigrationHelper.UpdateWorkflowActionType("A65D98A1-E6BE-4819-B7F4-5F7B30CE98D3", "Set Reference 2", 0, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "C6423A03-4FAB-41FE-8025-5C7FD947CA4E"); // Protection Application:Reference 2:Set Reference 2

                RockMigrationHelper.UpdateWorkflowActionType("E6F91770-538A-407A-A57B-29B904BF6642", "Set Reference 3", 0, "972F19B9-598B-474B-97A4-50E56E7B59D2", true, false, "", "", 1, "", "22E81E67-218A-40C2-80AE-14351A1D90AD"); // Protection Application:Reference 3:Set Reference 3

                RockMigrationHelper.UpdateWorkflowActionType("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "Assign to Security", 2, "DB2D8C44-6E57-4B45-8973-5DE327D61554", true, false, "", "", 1, "", "4A131840-09CA-4921-AA62-E048F962639A"); // Protection Application:Approve Request:Assign to Security

                RockMigrationHelper.UpdateWorkflowActionType("4AEAFD25-8CAC-4CD8-8D9F-F294C5AD30D9", "Assign Activity", 1, "DB2D8C44-6E57-4B45-8973-5DE327D61554", true, false, "", "", 1, "", "31FC7A7E-3E99-4962-AFA0-CD5D29B9EF5A"); // Protection Application:Review Result:Assign Activity

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Activate Expiration Activity", 8, "38907A90-1634-4A93-8017-619326A4A582", true, false, "", "", 1, "", "771D6FCA-94C2-4FAA-82B4-7858E44FEBF1"); // Protection Application:Launch Request:Activate Expiration Activity

                RockMigrationHelper.UpdateWorkflowActionType("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "Submit Request", 5, "38907A90-1634-4A93-8017-619326A4A582", true, false, "", "", 1, "", "852F23FB-A977-4717-8F4E-F474F79703DD"); // Protection Application:Approve Request:Submit Request

                RockMigrationHelper.UpdateWorkflowActionType("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "Deny Request", 6, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "", 1, "", "2FCD3479-1F6A-45A4-AA57-A72A56314628"); // Protection Application:Approve Request:Deny Request

                RockMigrationHelper.UpdateWorkflowActionType("242C7833-FAE3-44E9-97CF-0BF20E3EE03A", "Process Result", 2, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 64, "", "FFE34F40-8B80-4627-8DE0-51316CB3F653"); // Protection Application:Submit Request:Process Result

                RockMigrationHelper.UpdateWorkflowActionType("7AEFF3A0-F5C2-4B0F-9981-60A4F437FFE5", "Activate Review", 2, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 2, "Pass", "4F364F5B-ECAC-4683-9812-4FD7FED41BE7"); // Protection Application:Process Result:Activate Review

                RockMigrationHelper.UpdateWorkflowActionType("7AEFF3A0-F5C2-4B0F-9981-60A4F437FFE5", "Activate Complete", 3, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 1, "Pass", "0AB3E8AB-6A5A-40BF-9DEE-6DD50A694BD1"); // Protection Application:Process Result:Activate Complete

                RockMigrationHelper.UpdateWorkflowActionType("4AEAFD25-8CAC-4CD8-8D9F-F294C5AD30D9", "Activate Complete", 4, "38907A90-1634-4A93-8017-619326A4A582", true, true, "", "", 1, "", "DF341F15-3120-43FB-83FD-0DC20F43D9F4"); // Protection Application:Review Result:Activate Complete

                RockMigrationHelper.UpdateWorkflowActionType("0014C961-D3FE-4D25-9183-12B5EB6E6E8A", "Wait for Reference 1", 2, "38907A90-1634-4A93-8017-619326A4A582", true, false, "", "59EFEF9D-FFFB-40A4-98A9-320964117EF6", 1, "Yes", "78C8937D-2103-4DF6-9F33-7EA72282D31E"); // Protection Application:Reference 1:Wait for Reference 1

                RockMigrationHelper.UpdateWorkflowActionType("A65D98A1-E6BE-4819-B7F4-5F7B30CE98D3", "Wait for Reference 2", 2, "38907A90-1634-4A93-8017-619326A4A582", true, false, "", "F1083251-6C9B-4F8C-80A0-E16599C6D3B7", 1, "Yes", "FC74E683-ADFE-49EA-937D-EDA37FEA57FA"); // Protection Application:Reference 2:Wait for Reference 2

                RockMigrationHelper.UpdateWorkflowActionType("E6F91770-538A-407A-A57B-29B904BF6642", "Wait for Reference 3", 2, "38907A90-1634-4A93-8017-619326A4A582", true, false, "", "229615BF-EBE9-4930-88D7-50D9C51B39F0", 1, "Yes", "0FAE2175-C6EB-4642-B754-8D7C65CB1252"); // Protection Application:Reference 3:Wait for Reference 3

                RockMigrationHelper.UpdateWorkflowActionType("C6AE73DD-7E67-423D-814F-5D047594B536", "Activate Application Review", 0, "38907A90-1634-4A93-8017-619326A4A582", true, false, "", "59EFEF9D-FFFB-40A4-98A9-320964117EF6", 1, "Yes", "2D35C75C-20FB-459A-A05B-AA15D621A485"); // Protection Application:Receive Reference 2:Activate Application Review

                RockMigrationHelper.UpdateWorkflowActionType("BBABE055-2B13-4291-8383-16C473CE84FA", "Activate Application Review", 0, "38907A90-1634-4A93-8017-619326A4A582", true, false, "", "F1083251-6C9B-4F8C-80A0-E16599C6D3B7", 1, "Yes", "D2A321E0-00AA-4F3D-B852-6C8FB4024DFD"); // Protection Application:Receive Reference 3:Activate Application Review

                RockMigrationHelper.UpdateWorkflowActionType("08962ECE-DA0E-4F22-B764-4E35A31CF27C", "Wait for Reference 1", 2, "017DC868-AF98-4E03-B3A7-75EB5CA3BE02", true, false, "", "59EFEF9D-FFFB-40A4-98A9-320964117EF6", 1, "Yes", "BD9245CF-F84B-43FF-AB1A-AD98694A6B49"); // Protection Application:Receive Protection Application:Wait for Reference 1

                RockMigrationHelper.UpdateWorkflowActionType("08962ECE-DA0E-4F22-B764-4E35A31CF27C", "Wait for Reference 2", 3, "017DC868-AF98-4E03-B3A7-75EB5CA3BE02", true, false, "", "F1083251-6C9B-4F8C-80A0-E16599C6D3B7", 1, "Yes", "D9E27918-A076-4B27-A321-0ECB6C13BE19"); // Protection Application:Receive Protection Application:Wait for Reference 2

                RockMigrationHelper.UpdateWorkflowActionType("08962ECE-DA0E-4F22-B764-4E35A31CF27C", "Wait for Reference 3", 4, "017DC868-AF98-4E03-B3A7-75EB5CA3BE02", true, false, "", "229615BF-EBE9-4930-88D7-50D9C51B39F0", 1, "Yes", "A2AE6F63-1E0B-42C8-9D92-96A0690EF866"); // Protection Application:Receive Protection Application:Wait for Reference 3

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Set Name", 2, "36005473-BD5D-470B-B28D-98E6D7ED808D", true, false, "", "", 1, "", "C6E94386-E742-4446-A72C-0A6BB1180970"); // Protection Application:Launch Request:Set Name

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Wait for SSN", 9, "DA2E4974-8F7D-4472-AD0B-9B65820543F0", true, false, "", "696DFB2D-800C-49DE-B687-D5C868FB296C", 64, "", "5FEFD3BE-DFEF-4CE4-B016-6DE89D6D0953"); // Protection Application:Launch Request:Wait for SSN

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Set Requester", 3, "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", true, false, "", "", 1, "", "1D94B1EA-DC1F-48C5-96AA-38A4C73569D9"); // Protection Application:Launch Request:Set Requester

                RockMigrationHelper.UpdateWorkflowActionType("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "Set Approver", 4, "24B7D5E6-C30F-48F4-9D7E-AF45A342CF3A", true, false, "", "", 1, "", "653736D4-CF5E-4FE6-AFA0-21D39946E64E"); // Protection Application:Approve Request:Set Approver

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Set Workflow Status - Email Sent", 7, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "24390508-D448-4F89-9CFC-04F3C6BE68EA"); // Protection Application:Launch Request:Set Workflow Status - Email Sent

                RockMigrationHelper.UpdateWorkflowActionType("571CE67D-6496-4AE3-A0B1-77DA0767EEC8", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "6113EB99-1AEB-4907-A539-ADA405287E93"); // Protection Application:Launch Request:Set Status

                RockMigrationHelper.UpdateWorkflowActionType("08962ECE-DA0E-4F22-B764-4E35A31CF27C", "Set Workflow Status - Received Protection Application", 1, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "9BAAC7AE-6C60-44B8-9821-AD35B9560611"); // Protection Application:Receive Protection Application:Set Workflow Status - Received Protection Application

                RockMigrationHelper.UpdateWorkflowActionType("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "Set Status", 1, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "C3D1C4F6-1EC6-48A7-A57D-A5436523209D"); // Protection Application:Approve Request:Set Status

                RockMigrationHelper.UpdateWorkflowActionType("73D0020B-BF87-4723-ADC9-C8E69566409D", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "DF13D5A6-3A7C-46AB-90D3-0A4963DD7150"); // Protection Application:Review Denial:Set Status

                RockMigrationHelper.UpdateWorkflowActionType("242C7833-FAE3-44E9-97CF-0BF20E3EE03A", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "68366F1D-1B27-4E87-AF4B-3BA29E13D961"); // Protection Application:Submit Request:Set Status

                RockMigrationHelper.UpdateWorkflowActionType("4AEAFD25-8CAC-4CD8-8D9F-F294C5AD30D9", "Set Status", 0, "96D371A7-A291-4F8F-8B38-B8F72CE5407E", true, false, "", "", 1, "", "7AB3B9BF-EF79-477A-9353-0F7DDEB67A97"); // Protection Application:Review Result:Set Status

                RockMigrationHelper.UpdateWorkflowActionType("7AEFF3A0-F5C2-4B0F-9981-60A4F437FFE5", "Save Date", 0, "D12A74D4-1467-4F3A-80FA-CFA3289128E8", true, false, "", "", 1, "", "69A5C97B-83FF-4B59-8FA9-7FA672FF110A"); // Protection Application:Process Result:Save Date

                RockMigrationHelper.UpdateWorkflowActionType("7AEFF3A0-F5C2-4B0F-9981-60A4F437FFE5", "Save Report", 1, "D12A74D4-1467-4F3A-80FA-CFA3289128E8", true, false, "", "", 1, "", "333BB079-63E1-4910-B863-31F8163FCD7B"); // Protection Application:Process Result:Save Report

                RockMigrationHelper.UpdateWorkflowActionType("9DABB5AD-95CB-4F2D-AB3B-1CA3630C1F5D", "Update Attribute Status", 0, "D12A74D4-1467-4F3A-80FA-CFA3289128E8", true, false, "", "", 1, "", "8AF2B4FD-4084-4992-8300-C77C727FEB09"); // Protection Application:Complete Request:Update Attribute Status

                RockMigrationHelper.UpdateWorkflowActionType("9DABB5AD-95CB-4F2D-AB3B-1CA3630C1F5D", "Background Check Passed", 1, "D12A74D4-1467-4F3A-80FA-CFA3289128E8", true, false, "", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 1, "Pass", "32D95BD0-48CA-4CA4-BD50-64D774D02EAA"); // Protection Application:Complete Request:Background Check Passed

                RockMigrationHelper.UpdateWorkflowActionType("9DABB5AD-95CB-4F2D-AB3B-1CA3630C1F5D", "Background Check Failed", 2, "D12A74D4-1467-4F3A-80FA-CFA3289128E8", true, false, "", "A521A639-E4B7-4F90-AD05-C80A9C4D9AC3", 1, "Fail", "6BA7F1D9-0DD7-43A2-8865-7970135064E8"); // Protection Application:Complete Request:Background Check Failed

                RockMigrationHelper.UpdateWorkflowActionType("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "Approve or Deny", 3, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "A54BE863-EF0F-4531-A0C3-23ED6A1A24C4", "", 1, "", "A1715BA1-059E-4ABE-B03F-83154CFF4122"); // Protection Application:Approve Request:Approve or Deny

                RockMigrationHelper.UpdateWorkflowActionType("7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0", "Verify Details", 0, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "8C8210E4-BCE5-4EE4-9714-E09EB496F165", "", 1, "", "35521307-188C-40EF-B057-125DD5D60C86"); // Protection Application:Approve Request:Verify Details

                RockMigrationHelper.UpdateWorkflowActionType("73D0020B-BF87-4723-ADC9-C8E69566409D", "Review", 2, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "697CF00F-9834-4FA6-871D-B8E80D7A2001", "", 1, "", "AB016E20-7AC0-4E23-AAEE-9A19F4832F9C"); // Protection Application:Review Denial:Review

                RockMigrationHelper.UpdateWorkflowActionType("4AEAFD25-8CAC-4CD8-8D9F-F294C5AD30D9", "Review Results", 2, "486DC4FA-FCBC-425F-90B0-E606DA8A9F68", true, false, "BFD21326-84DE-44D6-B2F3-0FC42603F8A1", "", 1, "", "7D8267D0-A794-4507-8218-B7A8B3957E1D"); // Protection Application:Review Result:Review Results

                RockMigrationHelper.UpdateWorkflowActionType("CD7B6CA6-ED4A-41F9-8A09-FC07BC684421", "Delete Workflow", 0, "76B805A5-F7D4-4F0E-934D-F2FBE5A7C9F4", true, false, "", "", 1, "", "EE0705B7-D2BA-4BEE-85E3-0BF131D3538C"); // Protection Application:Cancel Request:Delete Workflow

                RockMigrationHelper.AddActionTypeAttributeValue("6113EB99-1AEB-4907-A539-ADA405287E93", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Protection Application:Launch Request:Set Status:Active

                RockMigrationHelper.AddActionTypeAttributeValue("6113EB99-1AEB-4907-A539-ADA405287E93", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Initial Entry"); // Protection Application:Launch Request:Set Status:Status

                RockMigrationHelper.AddActionTypeAttributeValue("6113EB99-1AEB-4907-A539-ADA405287E93", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Protection Application:Launch Request:Set Status:Order

                RockMigrationHelper.AddActionTypeAttributeValue("66CAFE61-19F6-4204-A3D6-9E97F97F665E", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False"); // Protection Application:Launch Request:Set Person:Active

                RockMigrationHelper.AddActionTypeAttributeValue("66CAFE61-19F6-4204-A3D6-9E97F97F665E", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"c495d2e1-9fcc-477b-b8c3-c92f28993354"); // Protection Application:Launch Request:Set Person:Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("66CAFE61-19F6-4204-A3D6-9E97F97F665E", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @""); // Protection Application:Launch Request:Set Person:Order

                RockMigrationHelper.AddActionTypeAttributeValue("66CAFE61-19F6-4204-A3D6-9E97F97F665E", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"True"); // Protection Application:Launch Request:Set Person:Use Id instead of Guid

                RockMigrationHelper.AddActionTypeAttributeValue("C6E94386-E742-4446-A72C-0A6BB1180970", "0A800013-51F7-4902-885A-5BE215D67D3D", @"False"); // Protection Application:Launch Request:Set Name:Active

                RockMigrationHelper.AddActionTypeAttributeValue("C6E94386-E742-4446-A72C-0A6BB1180970", "5D95C15A-CCAE-40AD-A9DD-F929DA587115", @""); // Protection Application:Launch Request:Set Name:Order

                RockMigrationHelper.AddActionTypeAttributeValue("C6E94386-E742-4446-A72C-0A6BB1180970", "93852244-A667-4749-961A-D47F88675BE4", @"c495d2e1-9fcc-477b-b8c3-c92f28993354"); // Protection Application:Launch Request:Set Name:Text Value|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("1D94B1EA-DC1F-48C5-96AA-38A4C73569D9", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8", @""); // Protection Application:Launch Request:Set Requester:Order

                RockMigrationHelper.AddActionTypeAttributeValue("1D94B1EA-DC1F-48C5-96AA-38A4C73569D9", "DE9CB292-4785-4EA3-976D-3826F91E9E98", @"False"); // Protection Application:Launch Request:Set Requester:Active

                RockMigrationHelper.AddActionTypeAttributeValue("1D94B1EA-DC1F-48C5-96AA-38A4C73569D9", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112", @"e48cd3df-b284-4dd8-87e4-35f7d120ff7f"); // Protection Application:Launch Request:Set Requester:Person Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("4D78CCC7-92E7-4ADA-B862-92E777743C59", "F3B9908B-096F-460B-8320-122CF046D1F9", @"/*
Throws exception:
Conversion failed when converting from a character string to uniqueidentifier.
*/
/*
SELECT ISNULL( (
    SELECT 
        CASE WHEN DATEADD(year, 1, AV.[ValueAsDateTime]) > GETDATE() THEN 'True' ELSE 'False' END
    FROM [AttributeValue] AV
        INNER JOIN [Attribute] A ON A.[Id] = AV.[AttributeId]
        INNER JOIN [PersonAlias] P ON P.[PersonId] = AV.[EntityId]
    WHERE AV.[ValueAsDateTime] IS NOT NULL
        AND A.[Guid] = '{{ Workflow.DateAttribute_unformatted }}'
        AND P.[Guid] = '{{ Workflow.Person_unformatted }}'
), 'False')
*/"); // Protection Application:Launch Request:Set Warning:SQLQuery

                RockMigrationHelper.AddActionTypeAttributeValue("4D78CCC7-92E7-4ADA-B862-92E777743C59", "A18C3143-0586-4565-9F36-E603BC674B4E", @"False"); // Protection Application:Launch Request:Set Warning:Active

                RockMigrationHelper.AddActionTypeAttributeValue("4D78CCC7-92E7-4ADA-B862-92E777743C59", "FA7C685D-8636-41EF-9998-90FFF3998F76", @""); // Protection Application:Launch Request:Set Warning:Order

                RockMigrationHelper.AddActionTypeAttributeValue("4D78CCC7-92E7-4ADA-B862-92E777743C59", "56997192-2545-4EA1-B5B2-313B04588984", @""); // Protection Application:Launch Request:Set Warning:Result Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("8AF5EEF9-5D4E-465E-BBF0-5C439248F0AA", "57093B41-50ED-48E5-B72B-8829E62704C8", @""); // Protection Application:Launch Request:Set Workflow Start Time:Order

                RockMigrationHelper.AddActionTypeAttributeValue("8AF5EEF9-5D4E-465E-BBF0-5C439248F0AA", "D7EAA859-F500-4521-9523-488B12EAA7D2", @"False"); // Protection Application:Launch Request:Set Workflow Start Time:Active

                RockMigrationHelper.AddActionTypeAttributeValue("8AF5EEF9-5D4E-465E-BBF0-5C439248F0AA", "44A0B977-4730-4519-8FF6-B0A01A95B212", @"727be54d-b06d-4e1d-ae42-83b489efb7bc"); // Protection Application:Launch Request:Set Workflow Start Time:Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("8AF5EEF9-5D4E-465E-BBF0-5C439248F0AA", "E5272B11-A2B8-49DC-860D-8D574E2BC15C", @"{{ 'Now' | Date:'yyyy-MM-ddThh:mm:ss' }}"); // Protection Application:Launch Request:Set Workflow Start Time:Text Value|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("1DAB22E0-7FA3-4C84-8514-E6653A32BF21", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False"); // Protection Application:Launch Request:Send Protection Application Request:Active

                RockMigrationHelper.AddActionTypeAttributeValue("1DAB22E0-7FA3-4C84-8514-E6653A32BF21", "D1269254-C15A-40BD-B784-ADCC231D3950", @""); // Protection Application:Launch Request:Send Protection Application Request:Order

                RockMigrationHelper.AddActionTypeAttributeValue("1DAB22E0-7FA3-4C84-8514-E6653A32BF21", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @"e48cd3df-b284-4dd8-87e4-35f7d120ff7f"); // Protection Application:Launch Request:Send Protection Application Request:From Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("1DAB22E0-7FA3-4C84-8514-E6653A32BF21", "0C4C13B8-7076-4872-925A-F950886B5E16", @"c495d2e1-9fcc-477b-b8c3-c92f28993354"); // Protection Application:Launch Request:Send Protection Application Request:Send To Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("1DAB22E0-7FA3-4C84-8514-E6653A32BF21", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"Protection Application Request from {{ Workflow.Requester.NickName }}"); // Protection Application:Launch Request:Send Protection Application Request:Subject

                RockMigrationHelper.AddActionTypeAttributeValue("1DAB22E0-7FA3-4C84-8514-E6653A32BF21", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ GlobalAttribute.EmailStyles }}
            {{ GlobalAttribute.EmailHeader }}
            <p>{{ Person.NickName }},</p>

            <p>{{ Workflow.CustomMessage | NewlineToBr }}</p>

            <p>Please click the link to <a href=""{{ GlobalAttribute.PublicApplicationRoot }}MyAccount/ProtectionApp/{{ Person.UrlEncodedKey }}?WorkflowId={{ Workflow.Guid }}"">begin your application</a>.</p>

            <p><a href=""{{ GlobalAttribute.PublicApplicationRoot }}Unsubscribe/{{ Person.UrlEncodedKey }}"">I&#39;m no longer involved with {{ GlobalAttribute.OrganizationName }}. Please remove me from all future communications.</a></p>

            <p>-{{ Workflow.Requester.NickName }}</p>

            {{ GlobalAttribute.EmailFooter }}
"); // Protection Application:Launch Request:Send Protection Application Request:Body

                RockMigrationHelper.AddActionTypeAttributeValue("24390508-D448-4F89-9CFC-04F3C6BE68EA", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Protection Application:Launch Request:Set Workflow Status - Email Sent:Active

                RockMigrationHelper.AddActionTypeAttributeValue("24390508-D448-4F89-9CFC-04F3C6BE68EA", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Email Sent"); // Protection Application:Launch Request:Set Workflow Status - Email Sent:Status

                RockMigrationHelper.AddActionTypeAttributeValue("24390508-D448-4F89-9CFC-04F3C6BE68EA", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Protection Application:Launch Request:Set Workflow Status - Email Sent:Order

                RockMigrationHelper.AddActionTypeAttributeValue("771D6FCA-94C2-4FAA-82B4-7858E44FEBF1", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"6F6471C4-D066-46FC-B9A3-ED95C586A5D2"); // Protection Application:Launch Request:Activate Expiration Activity:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("771D6FCA-94C2-4FAA-82B4-7858E44FEBF1", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Launch Request:Activate Expiration Activity:Active

                RockMigrationHelper.AddActionTypeAttributeValue("771D6FCA-94C2-4FAA-82B4-7858E44FEBF1", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Launch Request:Activate Expiration Activity:Order

                RockMigrationHelper.AddActionTypeAttributeValue("5FEFD3BE-DFEF-4CE4-B016-6DE89D6D0953", "C632E229-3688-40ED-895C-6D2BCED3BD65", @""); // Protection Application:Launch Request:Wait for SSN:Order

                RockMigrationHelper.AddActionTypeAttributeValue("5FEFD3BE-DFEF-4CE4-B016-6DE89D6D0953", "C5190778-2BB7-45D3-B387-D9AB205412E0", @"False"); // Protection Application:Launch Request:Wait for SSN:Active

                RockMigrationHelper.AddActionTypeAttributeValue("6E751023-1FDB-4AD5-B560-665B24E6F4E5", "D5A4C63A-37CB-4003-9E85-77DD96EF6093", @"False"); // Protection Application:Evaluate Expiration Date:Calculate Days Since Activation:Active

                RockMigrationHelper.AddActionTypeAttributeValue("6E751023-1FDB-4AD5-B560-665B24E6F4E5", "20CD0B82-F75E-43EF-8EC1-26F9F846053A", @"727be54d-b06d-4e1d-ae42-83b489efb7bc"); // Protection Application:Evaluate Expiration Date:Calculate Days Since Activation:Initial Date Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("6E751023-1FDB-4AD5-B560-665B24E6F4E5", "8DB7265C-177E-4924-BAF3-E7D6D2250A7B", @""); // Protection Application:Evaluate Expiration Date:Calculate Days Since Activation:Order

                RockMigrationHelper.AddActionTypeAttributeValue("6E751023-1FDB-4AD5-B560-665B24E6F4E5", "5369480E-FE99-4578-A82A-1FF5F078EF43", @"b9a1bc4d-09e9-4894-821b-624c36753458"); // Protection Application:Evaluate Expiration Date:Calculate Days Since Activation:Destination Elapsed Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("6E751023-1FDB-4AD5-B560-665B24E6F4E5", "D7F80FBD-9880-458D-A2BE-12F77480DC4B", @"Days"); // Protection Application:Evaluate Expiration Date:Calculate Days Since Activation:Interval Type

                RockMigrationHelper.AddActionTypeAttributeValue("DC717FEB-8AD5-4E15-A022-DA0C237AB2A8", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @""); // Protection Application:Receive Protection Application:Set Questionnaire:Order

                RockMigrationHelper.AddActionTypeAttributeValue("DC717FEB-8AD5-4E15-A022-DA0C237AB2A8", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"6f2d49b0-ccb8-46e7-8e2e-3021a4bfa211"); // Protection Application:Receive Protection Application:Set Questionnaire:Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("DC717FEB-8AD5-4E15-A022-DA0C237AB2A8", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False"); // Protection Application:Receive Protection Application:Set Questionnaire:Active

                RockMigrationHelper.AddActionTypeAttributeValue("DC717FEB-8AD5-4E15-A022-DA0C237AB2A8", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False"); // Protection Application:Receive Protection Application:Set Questionnaire:Use Id instead of Guid

                RockMigrationHelper.AddActionTypeAttributeValue("9BAAC7AE-6C60-44B8-9821-AD35B9560611", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Protection Application:Receive Protection Application:Set Workflow Status - Received Protection Application:Order

                RockMigrationHelper.AddActionTypeAttributeValue("9BAAC7AE-6C60-44B8-9821-AD35B9560611", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Received Protection Application"); // Protection Application:Receive Protection Application:Set Workflow Status - Received Protection Application:Status

                RockMigrationHelper.AddActionTypeAttributeValue("9BAAC7AE-6C60-44B8-9821-AD35B9560611", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Protection Application:Receive Protection Application:Set Workflow Status - Received Protection Application:Active

                RockMigrationHelper.AddActionTypeAttributeValue("BD9245CF-F84B-43FF-AB1A-AD98694A6B49", "BAB22829-E0D0-47CB-9681-06A459888842", @"Reference 1 Received"); // Protection Application:Receive Protection Application:Wait for Reference 1:Message

                RockMigrationHelper.AddActionTypeAttributeValue("BD9245CF-F84B-43FF-AB1A-AD98694A6B49", "54B3175B-193E-4B9A-9136-9145E864980D", @""); // Protection Application:Receive Protection Application:Wait for Reference 1:Order

                RockMigrationHelper.AddActionTypeAttributeValue("BD9245CF-F84B-43FF-AB1A-AD98694A6B49", "61193203-64B0-4870-847F-33E0170D9382", @"False"); // Protection Application:Receive Protection Application:Wait for Reference 1:Active

                RockMigrationHelper.AddActionTypeAttributeValue("D9E27918-A076-4B27-A321-0ECB6C13BE19", "61193203-64B0-4870-847F-33E0170D9382", @"False"); // Protection Application:Receive Protection Application:Wait for Reference 2:Active

                RockMigrationHelper.AddActionTypeAttributeValue("D9E27918-A076-4B27-A321-0ECB6C13BE19", "54B3175B-193E-4B9A-9136-9145E864980D", @""); // Protection Application:Receive Protection Application:Wait for Reference 2:Order

                RockMigrationHelper.AddActionTypeAttributeValue("D9E27918-A076-4B27-A321-0ECB6C13BE19", "BAB22829-E0D0-47CB-9681-06A459888842", @"Reference 2 Received"); // Protection Application:Receive Protection Application:Wait for Reference 2:Message

                RockMigrationHelper.AddActionTypeAttributeValue("A2AE6F63-1E0B-42C8-9D92-96A0690EF866", "BAB22829-E0D0-47CB-9681-06A459888842", @"Reference 3 Received"); // Protection Application:Receive Protection Application:Wait for Reference 3:Message

                RockMigrationHelper.AddActionTypeAttributeValue("A2AE6F63-1E0B-42C8-9D92-96A0690EF866", "54B3175B-193E-4B9A-9136-9145E864980D", @""); // Protection Application:Receive Protection Application:Wait for Reference 3:Order

                RockMigrationHelper.AddActionTypeAttributeValue("A2AE6F63-1E0B-42C8-9D92-96A0690EF866", "61193203-64B0-4870-847F-33E0170D9382", @"False"); // Protection Application:Receive Protection Application:Wait for Reference 3:Active

                RockMigrationHelper.AddActionTypeAttributeValue("4920E6C3-EA3A-4C1C-A2DF-F7ABD54B0277", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False"); // Protection Application:Reference 1:Set Reference 1:Active

                RockMigrationHelper.AddActionTypeAttributeValue("4920E6C3-EA3A-4C1C-A2DF-F7ABD54B0277", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"ecb35891-3e6f-4b66-bbd7-5d6eb2052136"); // Protection Application:Reference 1:Set Reference 1:Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("4920E6C3-EA3A-4C1C-A2DF-F7ABD54B0277", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @""); // Protection Application:Reference 1:Set Reference 1:Order

                RockMigrationHelper.AddActionTypeAttributeValue("4920E6C3-EA3A-4C1C-A2DF-F7ABD54B0277", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False"); // Protection Application:Reference 1:Set Reference 1:Use Id instead of Guid

                RockMigrationHelper.AddActionTypeAttributeValue("BB444AC8-301B-4BDF-9D67-53A039BA0FB9", "D1269254-C15A-40BD-B784-ADCC231D3950", @""); // Protection Application:Reference 1:Send Reference 1 Email:Order

                RockMigrationHelper.AddActionTypeAttributeValue("BB444AC8-301B-4BDF-9D67-53A039BA0FB9", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False"); // Protection Application:Reference 1:Send Reference 1 Email:Active

                RockMigrationHelper.AddActionTypeAttributeValue("BB444AC8-301B-4BDF-9D67-53A039BA0FB9", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @"e48cd3df-b284-4dd8-87e4-35f7d120ff7f"); // Protection Application:Reference 1:Send Reference 1 Email:From Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("BB444AC8-301B-4BDF-9D67-53A039BA0FB9", "0C4C13B8-7076-4872-925A-F950886B5E16", @"ecb35891-3e6f-4b66-bbd7-5d6eb2052136"); // Protection Application:Reference 1:Send Reference 1 Email:Send To Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("BB444AC8-301B-4BDF-9D67-53A039BA0FB9", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"Reference for {{ Workflow.Person }}"); // Protection Application:Reference 1:Send Reference 1 Email:Subject

                RockMigrationHelper.AddActionTypeAttributeValue("BB444AC8-301B-4BDF-9D67-53A039BA0FB9", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ GlobalAttribute.EmailStyles }}
{{ GlobalAttribute.EmailHeader }}
<p>Dear {{ Person.FirstName }},</p>

<p>You are receiving this email because {{ Workflow.Person }} has applied for a volunteer position at Willow Creek Community Church and has listed you as a reference in his/her application.  In seeking references, Willow's desire is to create a safe and welcoming atmosphere for all who choose to worship here. We are so grateful that you are willing to assist us in this important task of screening our volunteers.</p>
<br/>
<p>Please take a few minutes to answer the short questionnaire to help us determine if the applicant is a good fit for our community. By clicking the link below, you are verifying that you are {{Person.FirstName}} {{Person.LastName}}.</p> 
<br/>
<p>Please click the link to <a href=""{{ GlobalAttribute.PublicApplicationRoot }}MyAccount/ProtectionApp/ReferenceCheck/{{ Person.UrlEncodedKey }}?WorkflowId={{ Workflow.Guid }}"">begin the reference check</a>.</p>
<br/>
<p>If you have any questions, please contact Willow Creek's Protection Ministry:<br>
<a href=""mailto:Protection@willowcreek.org"">Protection@willowcreek.org</a><br>
224-512-1920
</p>

{{ GlobalAttribute.EmailFooter }}
"); // Protection Application:Reference 1:Send Reference 1 Email:Body

                RockMigrationHelper.AddActionTypeAttributeValue("78C8937D-2103-4DF6-9F33-7EA72282D31E", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"55311A4F-2AB1-4CC3-9C0A-47A15055354C"); // Protection Application:Reference 1:Wait for Reference 1:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("78C8937D-2103-4DF6-9F33-7EA72282D31E", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Reference 1:Wait for Reference 1:Order

                RockMigrationHelper.AddActionTypeAttributeValue("78C8937D-2103-4DF6-9F33-7EA72282D31E", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Reference 1:Wait for Reference 1:Active

                RockMigrationHelper.AddActionTypeAttributeValue("C6423A03-4FAB-41FE-8025-5C7FD947CA4E", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False"); // Protection Application:Reference 2:Set Reference 2:Active

                RockMigrationHelper.AddActionTypeAttributeValue("C6423A03-4FAB-41FE-8025-5C7FD947CA4E", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @""); // Protection Application:Reference 2:Set Reference 2:Order

                RockMigrationHelper.AddActionTypeAttributeValue("C6423A03-4FAB-41FE-8025-5C7FD947CA4E", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"887a729f-615b-477e-a4e2-e407ea0c6a45"); // Protection Application:Reference 2:Set Reference 2:Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("C6423A03-4FAB-41FE-8025-5C7FD947CA4E", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False"); // Protection Application:Reference 2:Set Reference 2:Use Id instead of Guid

                RockMigrationHelper.AddActionTypeAttributeValue("21E45EA9-D444-4E14-9598-3B8FC728F137", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False"); // Protection Application:Reference 2:Send Reference 2 Email:Active

                RockMigrationHelper.AddActionTypeAttributeValue("21E45EA9-D444-4E14-9598-3B8FC728F137", "D1269254-C15A-40BD-B784-ADCC231D3950", @""); // Protection Application:Reference 2:Send Reference 2 Email:Order

                RockMigrationHelper.AddActionTypeAttributeValue("21E45EA9-D444-4E14-9598-3B8FC728F137", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @"e48cd3df-b284-4dd8-87e4-35f7d120ff7f"); // Protection Application:Reference 2:Send Reference 2 Email:From Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("21E45EA9-D444-4E14-9598-3B8FC728F137", "0C4C13B8-7076-4872-925A-F950886B5E16", @"887a729f-615b-477e-a4e2-e407ea0c6a45"); // Protection Application:Reference 2:Send Reference 2 Email:Send To Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("21E45EA9-D444-4E14-9598-3B8FC728F137", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"Reference for {{ Workflow.Person }}"); // Protection Application:Reference 2:Send Reference 2 Email:Subject

                RockMigrationHelper.AddActionTypeAttributeValue("21E45EA9-D444-4E14-9598-3B8FC728F137", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ GlobalAttribute.EmailStyles }}
{{ GlobalAttribute.EmailHeader }}
<p>Dear {{ Person.FirstName }},</p>

<p>You are receiving this email because {{ Workflow.Person }} has applied for a volunteer position at Willow Creek Community Church and has listed you as a reference in his/her application.  In seeking references, Willow's desire is to create a safe and welcoming atmosphere for all who choose to worship here. We are so grateful that you are willing to assist us in this important task of screening our volunteers.</p>
<br/>
<p>Please take a few minutes to answer the short questionnaire to help us determine if the applicant is a good fit for our community. By clicking the link below, you are verifying that you are {{Person.FirstName}} {{Person.LastName}}.</p> 
<br/>
<p>Please click the link to <a href=""{{ GlobalAttribute.PublicApplicationRoot }}MyAccount/ProtectionApp/ReferenceCheck/{{ Person.UrlEncodedKey }}?WorkflowId={{ Workflow.Guid }}"">begin the reference check</a>.</p>
<br/>
<p>If you have any questions, please contact Willow Creek's Protection Ministry:<br>
<a href=""mailto:Protection@willowcreek.org"">Protection@willowcreek.org</a><br>
224-512-1920
</p>

{{ GlobalAttribute.EmailFooter }}
"); // Protection Application:Reference 2:Send Reference 2 Email:Body

                RockMigrationHelper.AddActionTypeAttributeValue("FC74E683-ADFE-49EA-937D-EDA37FEA57FA", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"C6AE73DD-7E67-423D-814F-5D047594B536"); // Protection Application:Reference 2:Wait for Reference 2:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("FC74E683-ADFE-49EA-937D-EDA37FEA57FA", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Reference 2:Wait for Reference 2:Active

                RockMigrationHelper.AddActionTypeAttributeValue("FC74E683-ADFE-49EA-937D-EDA37FEA57FA", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Reference 2:Wait for Reference 2:Order

                RockMigrationHelper.AddActionTypeAttributeValue("2D35C75C-20FB-459A-A05B-AA15D621A485", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Receive Reference 2:Activate Application Review:Order

                RockMigrationHelper.AddActionTypeAttributeValue("2D35C75C-20FB-459A-A05B-AA15D621A485", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Receive Reference 2:Activate Application Review:Active

                RockMigrationHelper.AddActionTypeAttributeValue("2D35C75C-20FB-459A-A05B-AA15D621A485", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0"); // Protection Application:Receive Reference 2:Activate Application Review:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("22E81E67-218A-40C2-80AE-14351A1D90AD", "9392E3D7-A28B-4CD8-8B03-5E147B102EF1", @"False"); // Protection Application:Reference 3:Set Reference 3:Active

                RockMigrationHelper.AddActionTypeAttributeValue("22E81E67-218A-40C2-80AE-14351A1D90AD", "61E6E1BC-E657-4F00-B2E9-769AAA25B9F7", @"d7f8ec15-22b4-4769-88e1-02163379fde2"); // Protection Application:Reference 3:Set Reference 3:Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("22E81E67-218A-40C2-80AE-14351A1D90AD", "AD4EFAC4-E687-43DF-832F-0DC3856ABABB", @""); // Protection Application:Reference 3:Set Reference 3:Order

                RockMigrationHelper.AddActionTypeAttributeValue("22E81E67-218A-40C2-80AE-14351A1D90AD", "1246C53A-FD92-4E08-ABDE-9A6C37E70C7B", @"False"); // Protection Application:Reference 3:Set Reference 3:Use Id instead of Guid

                RockMigrationHelper.AddActionTypeAttributeValue("7A1924EC-3EF4-4CA5-A02B-F1AAF0A9D86E", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @"e48cd3df-b284-4dd8-87e4-35f7d120ff7f"); // Protection Application:Reference 3:Send Reference 3 Email:From Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("7A1924EC-3EF4-4CA5-A02B-F1AAF0A9D86E", "D1269254-C15A-40BD-B784-ADCC231D3950", @""); // Protection Application:Reference 3:Send Reference 3 Email:Order

                RockMigrationHelper.AddActionTypeAttributeValue("7A1924EC-3EF4-4CA5-A02B-F1AAF0A9D86E", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False"); // Protection Application:Reference 3:Send Reference 3 Email:Active

                RockMigrationHelper.AddActionTypeAttributeValue("7A1924EC-3EF4-4CA5-A02B-F1AAF0A9D86E", "0C4C13B8-7076-4872-925A-F950886B5E16", @"d7f8ec15-22b4-4769-88e1-02163379fde2"); // Protection Application:Reference 3:Send Reference 3 Email:Send To Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("7A1924EC-3EF4-4CA5-A02B-F1AAF0A9D86E", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"Reference for {{ Workflow.Person }}"); // Protection Application:Reference 3:Send Reference 3 Email:Subject

                RockMigrationHelper.AddActionTypeAttributeValue("7A1924EC-3EF4-4CA5-A02B-F1AAF0A9D86E", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ GlobalAttribute.EmailStyles }}
{{ GlobalAttribute.EmailHeader }}
<p>Dear {{ Person.FirstName }},</p>

<p>You are receiving this email because {{ Workflow.Person }} has applied for a volunteer position at Willow Creek Community Church and has listed you as a reference in his/her application.  In seeking references, Willow's desire is to create a safe and welcoming atmosphere for all who choose to worship here. We are so grateful that you are willing to assist us in this important task of screening our volunteers.</p>
<br/>
<p>Please take a few minutes to answer the short questionnaire to help us determine if the applicant is a good fit for our community. By clicking the link below, you are verifying that you are {{Person.FirstName}} {{Person.LastName}}.</p> 
<br/>
<p>Please click the link to <a href=""{{ GlobalAttribute.PublicApplicationRoot }}MyAccount/ProtectionApp/ReferenceCheck/{{ Person.UrlEncodedKey }}?WorkflowId={{ Workflow.Guid }}"">begin the reference check</a>.</p>
<br/>
<p>If you have any questions, please contact Willow Creek's Protection Ministry:<br>
<a href=""mailto:Protection@willowcreek.org"">Protection@willowcreek.org</a><br>
224-512-1920
</p>

{{ GlobalAttribute.EmailFooter }}
"); // Protection Application:Reference 3:Send Reference 3 Email:Body

                RockMigrationHelper.AddActionTypeAttributeValue("0FAE2175-C6EB-4642-B754-8D7C65CB1252", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"BBABE055-2B13-4291-8383-16C473CE84FA"); // Protection Application:Reference 3:Wait for Reference 3:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("0FAE2175-C6EB-4642-B754-8D7C65CB1252", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Reference 3:Wait for Reference 3:Active

                RockMigrationHelper.AddActionTypeAttributeValue("0FAE2175-C6EB-4642-B754-8D7C65CB1252", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Reference 3:Wait for Reference 3:Order

                RockMigrationHelper.AddActionTypeAttributeValue("D2A321E0-00AA-4F3D-B852-6C8FB4024DFD", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Receive Reference 3:Activate Application Review:Order

                RockMigrationHelper.AddActionTypeAttributeValue("D2A321E0-00AA-4F3D-B852-6C8FB4024DFD", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Receive Reference 3:Activate Application Review:Active

                RockMigrationHelper.AddActionTypeAttributeValue("D2A321E0-00AA-4F3D-B852-6C8FB4024DFD", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"7E673D89-A8A7-4B4E-A0B3-FD2D80C724B0"); // Protection Application:Receive Reference 3:Activate Application Review:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("35521307-188C-40EF-B057-125DD5D60C86", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False"); // Protection Application:Approve Request:Verify Details:Active

                RockMigrationHelper.AddActionTypeAttributeValue("35521307-188C-40EF-B057-125DD5D60C86", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @""); // Protection Application:Approve Request:Verify Details:Order

                RockMigrationHelper.AddActionTypeAttributeValue("C3D1C4F6-1EC6-48A7-A57D-A5436523209D", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Protection Application:Approve Request:Set Status:Order

                RockMigrationHelper.AddActionTypeAttributeValue("C3D1C4F6-1EC6-48A7-A57D-A5436523209D", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Waiting for Submit Approval"); // Protection Application:Approve Request:Set Status:Status

                RockMigrationHelper.AddActionTypeAttributeValue("C3D1C4F6-1EC6-48A7-A57D-A5436523209D", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Protection Application:Approve Request:Set Status:Active

                RockMigrationHelper.AddActionTypeAttributeValue("4A131840-09CA-4921-AA62-E048F962639A", "C0D75D1A-16C5-4786-A1E0-25669BEE8FE9", @"False"); // Protection Application:Approve Request:Assign to Security:Active

                RockMigrationHelper.AddActionTypeAttributeValue("4A131840-09CA-4921-AA62-E048F962639A", "041B7B51-A694-4AF5-B455-64D0DE7160A2", @""); // Protection Application:Approve Request:Assign to Security:Order

                RockMigrationHelper.AddActionTypeAttributeValue("4A131840-09CA-4921-AA62-E048F962639A", "BBFAD050-5968-4D11-8887-2FF877D8C8AB", @"3981cf6d-7d15-4b57-aace-c0e25d28bd49|a6bcc49e-103f-46b0-8bac-84ea03ff04d5"); // Protection Application:Approve Request:Assign to Security:Group

                RockMigrationHelper.AddActionTypeAttributeValue("A1715BA1-059E-4ABE-B03F-83154CFF4122", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @""); // Protection Application:Approve Request:Approve or Deny:Order

                RockMigrationHelper.AddActionTypeAttributeValue("A1715BA1-059E-4ABE-B03F-83154CFF4122", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False"); // Protection Application:Approve Request:Approve or Deny:Active

                RockMigrationHelper.AddActionTypeAttributeValue("653736D4-CF5E-4FE6-AFA0-21D39946E64E", "BBED8A83-8BB2-4D35-BAFB-05F67DCAD112", @"0c2f0547-72e2-463e-9c81-a2479578d92e"); // Protection Application:Approve Request:Set Approver:Person Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("653736D4-CF5E-4FE6-AFA0-21D39946E64E", "DE9CB292-4785-4EA3-976D-3826F91E9E98", @"False"); // Protection Application:Approve Request:Set Approver:Active

                RockMigrationHelper.AddActionTypeAttributeValue("653736D4-CF5E-4FE6-AFA0-21D39946E64E", "89E9BCED-91AB-47B0-AD52-D78B0B7CB9E8", @""); // Protection Application:Approve Request:Set Approver:Order

                RockMigrationHelper.AddActionTypeAttributeValue("852F23FB-A977-4717-8F4E-F474F79703DD", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Approve Request:Submit Request:Active

                RockMigrationHelper.AddActionTypeAttributeValue("852F23FB-A977-4717-8F4E-F474F79703DD", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Approve Request:Submit Request:Order

                RockMigrationHelper.AddActionTypeAttributeValue("852F23FB-A977-4717-8F4E-F474F79703DD", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"242C7833-FAE3-44E9-97CF-0BF20E3EE03A"); // Protection Application:Approve Request:Submit Request:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("2FCD3479-1F6A-45A4-AA57-A72A56314628", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"73D0020B-BF87-4723-ADC9-C8E69566409D"); // Protection Application:Approve Request:Deny Request:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("2FCD3479-1F6A-45A4-AA57-A72A56314628", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Approve Request:Deny Request:Order

                RockMigrationHelper.AddActionTypeAttributeValue("2FCD3479-1F6A-45A4-AA57-A72A56314628", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Approve Request:Deny Request:Active

                RockMigrationHelper.AddActionTypeAttributeValue("DF13D5A6-3A7C-46AB-90D3-0A4963DD7150", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Protection Application:Review Denial:Set Status:Active

                RockMigrationHelper.AddActionTypeAttributeValue("DF13D5A6-3A7C-46AB-90D3-0A4963DD7150", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Waiting for More Details"); // Protection Application:Review Denial:Set Status:Status

                RockMigrationHelper.AddActionTypeAttributeValue("DF13D5A6-3A7C-46AB-90D3-0A4963DD7150", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Protection Application:Review Denial:Set Status:Order

                RockMigrationHelper.AddActionTypeAttributeValue("ABE7D8F5-BEFA-4C6E-8636-FE195A1E361F", "E0F7AB7E-7761-4600-A099-CB14ACDBF6EF", @"False"); // Protection Application:Review Denial:Assign to Requester:Active

                RockMigrationHelper.AddActionTypeAttributeValue("ABE7D8F5-BEFA-4C6E-8636-FE195A1E361F", "FBADD25F-D309-4512-8430-3CC8615DD60E", @"e48cd3df-b284-4dd8-87e4-35f7d120ff7f"); // Protection Application:Review Denial:Assign to Requester:Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("ABE7D8F5-BEFA-4C6E-8636-FE195A1E361F", "7A6B605D-7FB1-4F48-AF35-5A0683FB1CDA", @""); // Protection Application:Review Denial:Assign to Requester:Order

                RockMigrationHelper.AddActionTypeAttributeValue("AB016E20-7AC0-4E23-AAEE-9A19F4832F9C", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False"); // Protection Application:Review Denial:Review:Active

                RockMigrationHelper.AddActionTypeAttributeValue("AB016E20-7AC0-4E23-AAEE-9A19F4832F9C", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @""); // Protection Application:Review Denial:Review:Order

                RockMigrationHelper.AddActionTypeAttributeValue("68366F1D-1B27-4E87-AF4B-3BA29E13D961", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Waiting for Result"); // Protection Application:Submit Request:Set Status:Status

                RockMigrationHelper.AddActionTypeAttributeValue("68366F1D-1B27-4E87-AF4B-3BA29E13D961", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Protection Application:Submit Request:Set Status:Active

                RockMigrationHelper.AddActionTypeAttributeValue("68366F1D-1B27-4E87-AF4B-3BA29E13D961", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Protection Application:Submit Request:Set Status:Order

                RockMigrationHelper.AddActionTypeAttributeValue("14503B77-3209-47F1-BF4F-9D38A7040B9C", "A134F1A7-3824-43E0-9EB1-22C899B795BD", @"False"); // Protection Application:Submit Request:Submit Request:Active

                RockMigrationHelper.AddActionTypeAttributeValue("14503B77-3209-47F1-BF4F-9D38A7040B9C", "5DA71523-E8B0-4C4D-89A4-B47945A22A0C", @""); // Protection Application:Submit Request:Submit Request:Order

                RockMigrationHelper.AddActionTypeAttributeValue("FFE34F40-8B80-4627-8DE0-51316CB3F653", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"7AEFF3A0-F5C2-4B0F-9981-60A4F437FFE5"); // Protection Application:Submit Request:Process Result:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("FFE34F40-8B80-4627-8DE0-51316CB3F653", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Submit Request:Process Result:Active

                RockMigrationHelper.AddActionTypeAttributeValue("FFE34F40-8B80-4627-8DE0-51316CB3F653", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Submit Request:Process Result:Order

                RockMigrationHelper.AddActionTypeAttributeValue("69A5C97B-83FF-4B59-8FA9-7FA672FF110A", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Protection Application:Process Result:Save Date:Order

                RockMigrationHelper.AddActionTypeAttributeValue("69A5C97B-83FF-4B59-8FA9-7FA672FF110A", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Protection Application:Process Result:Save Date:Active

                RockMigrationHelper.AddActionTypeAttributeValue("69A5C97B-83FF-4B59-8FA9-7FA672FF110A", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"e48cd3df-b284-4dd8-87e4-35f7d120ff7f"); // Protection Application:Process Result:Save Date:Person

                RockMigrationHelper.AddActionTypeAttributeValue("69A5C97B-83FF-4B59-8FA9-7FA672FF110A", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"4abf0bf2-49ba-4363-9d85-ac48a0f7e92a"); // Protection Application:Process Result:Save Date:Person Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("69A5C97B-83FF-4B59-8FA9-7FA672FF110A", "94689BDE-493E-4869-A614-2D54822D747C", @"{{ 'Now' | Date:'yyyy-MM-dd' }}T00:00:00"); // Protection Application:Process Result:Save Date:Value|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("333BB079-63E1-4910-B863-31F8163FCD7B", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"c495d2e1-9fcc-477b-b8c3-c92f28993354"); // Protection Application:Process Result:Save Report:Person

                RockMigrationHelper.AddActionTypeAttributeValue("333BB079-63E1-4910-B863-31F8163FCD7B", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Protection Application:Process Result:Save Report:Active

                RockMigrationHelper.AddActionTypeAttributeValue("333BB079-63E1-4910-B863-31F8163FCD7B", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Protection Application:Process Result:Save Report:Order

                RockMigrationHelper.AddActionTypeAttributeValue("333BB079-63E1-4910-B863-31F8163FCD7B", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"f3931952-460d-43e0-a6e0-eb6b5b1f9167"); // Protection Application:Process Result:Save Report:Person Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("333BB079-63E1-4910-B863-31F8163FCD7B", "94689BDE-493E-4869-A614-2D54822D747C", @"4d5747bb-c182-4172-8c3e-9b3c063aac18"); // Protection Application:Process Result:Save Report:Value|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("4F364F5B-ECAC-4683-9812-4FD7FED41BE7", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Process Result:Activate Review:Order

                RockMigrationHelper.AddActionTypeAttributeValue("4F364F5B-ECAC-4683-9812-4FD7FED41BE7", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Process Result:Activate Review:Active

                RockMigrationHelper.AddActionTypeAttributeValue("4F364F5B-ECAC-4683-9812-4FD7FED41BE7", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"4AEAFD25-8CAC-4CD8-8D9F-F294C5AD30D9"); // Protection Application:Process Result:Activate Review:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("0AB3E8AB-6A5A-40BF-9DEE-6DD50A694BD1", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"9DABB5AD-95CB-4F2D-AB3B-1CA3630C1F5D"); // Protection Application:Process Result:Activate Complete:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("0AB3E8AB-6A5A-40BF-9DEE-6DD50A694BD1", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Process Result:Activate Complete:Active

                RockMigrationHelper.AddActionTypeAttributeValue("0AB3E8AB-6A5A-40BF-9DEE-6DD50A694BD1", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Process Result:Activate Complete:Order

                RockMigrationHelper.AddActionTypeAttributeValue("7AB3B9BF-EF79-477A-9353-0F7DDEB67A97", "36CE41F4-4C87-4096-B0C6-8269163BCC0A", @"False"); // Protection Application:Review Result:Set Status:Active

                RockMigrationHelper.AddActionTypeAttributeValue("7AB3B9BF-EF79-477A-9353-0F7DDEB67A97", "91A9F4BE-4A8E-430A-B466-A88DB2D33B34", @"Waiting for Review"); // Protection Application:Review Result:Set Status:Status

                RockMigrationHelper.AddActionTypeAttributeValue("7AB3B9BF-EF79-477A-9353-0F7DDEB67A97", "AE8C180C-E370-414A-B10D-97891B95D105", @""); // Protection Application:Review Result:Set Status:Order

                RockMigrationHelper.AddActionTypeAttributeValue("31FC7A7E-3E99-4962-AFA0-CD5D29B9EF5A", "BBFAD050-5968-4D11-8887-2FF877D8C8AB", @"3981cf6d-7d15-4b57-aace-c0e25d28bd49|a6bcc49e-103f-46b0-8bac-84ea03ff04d5"); // Protection Application:Review Result:Assign Activity:Group

                RockMigrationHelper.AddActionTypeAttributeValue("31FC7A7E-3E99-4962-AFA0-CD5D29B9EF5A", "041B7B51-A694-4AF5-B455-64D0DE7160A2", @""); // Protection Application:Review Result:Assign Activity:Order

                RockMigrationHelper.AddActionTypeAttributeValue("31FC7A7E-3E99-4962-AFA0-CD5D29B9EF5A", "C0D75D1A-16C5-4786-A1E0-25669BEE8FE9", @"False"); // Protection Application:Review Result:Assign Activity:Active

                RockMigrationHelper.AddActionTypeAttributeValue("7D8267D0-A794-4507-8218-B7A8B3957E1D", "234910F2-A0DB-4D7D-BAF7-83C880EF30AE", @"False"); // Protection Application:Review Result:Review Results:Active

                RockMigrationHelper.AddActionTypeAttributeValue("7D8267D0-A794-4507-8218-B7A8B3957E1D", "C178113D-7C86-4229-8424-C6D0CF4A7E23", @""); // Protection Application:Review Result:Review Results:Order

                RockMigrationHelper.AddActionTypeAttributeValue("251BF993-F08C-4C6B-AAD8-10FCD0187189", "57093B41-50ED-48E5-B72B-8829E62704C8", @""); // Protection Application:Review Result:Update Result:Order

                RockMigrationHelper.AddActionTypeAttributeValue("251BF993-F08C-4C6B-AAD8-10FCD0187189", "D7EAA859-F500-4521-9523-488B12EAA7D2", @"False"); // Protection Application:Review Result:Update Result:Active

                RockMigrationHelper.AddActionTypeAttributeValue("251BF993-F08C-4C6B-AAD8-10FCD0187189", "44A0B977-4730-4519-8FF6-B0A01A95B212", @"a521a639-e4b7-4f90-ad05-c80a9c4d9ac3"); // Protection Application:Review Result:Update Result:Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("251BF993-F08C-4C6B-AAD8-10FCD0187189", "E5272B11-A2B8-49DC-860D-8D574E2BC15C", @"17fd1eb6-b47c-4b70-b6ee-2cced1106f49"); // Protection Application:Review Result:Update Result:Text Value|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("DF341F15-3120-43FB-83FD-0DC20F43D9F4", "02D5A7A5-8781-46B4-B9FC-AF816829D240", @"9DABB5AD-95CB-4F2D-AB3B-1CA3630C1F5D"); // Protection Application:Review Result:Activate Complete:Activity

                RockMigrationHelper.AddActionTypeAttributeValue("DF341F15-3120-43FB-83FD-0DC20F43D9F4", "3809A78C-B773-440C-8E3F-A8E81D0DAE08", @""); // Protection Application:Review Result:Activate Complete:Order

                RockMigrationHelper.AddActionTypeAttributeValue("DF341F15-3120-43FB-83FD-0DC20F43D9F4", "E8ABD802-372C-47BE-82B1-96F50DB5169E", @"False"); // Protection Application:Review Result:Activate Complete:Active

                RockMigrationHelper.AddActionTypeAttributeValue("8AF2B4FD-4084-4992-8300-C77C727FEB09", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Protection Application:Complete Request:Update Attribute Status:Order

                RockMigrationHelper.AddActionTypeAttributeValue("8AF2B4FD-4084-4992-8300-C77C727FEB09", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Protection Application:Complete Request:Update Attribute Status:Active

                RockMigrationHelper.AddActionTypeAttributeValue("8AF2B4FD-4084-4992-8300-C77C727FEB09", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"c495d2e1-9fcc-477b-b8c3-c92f28993354"); // Protection Application:Complete Request:Update Attribute Status:Person

                RockMigrationHelper.AddActionTypeAttributeValue("8AF2B4FD-4084-4992-8300-C77C727FEB09", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"44490089-e02c-4e54-a456-454845abbc9d"); // Protection Application:Complete Request:Update Attribute Status:Person Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("8AF2B4FD-4084-4992-8300-C77C727FEB09", "94689BDE-493E-4869-A614-2D54822D747C", @"a521a639-e4b7-4f90-ad05-c80a9c4d9ac3"); // Protection Application:Complete Request:Update Attribute Status:Value|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("32D95BD0-48CA-4CA4-BD50-64D774D02EAA", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"c495d2e1-9fcc-477b-b8c3-c92f28993354"); // Protection Application:Complete Request:Background Check Passed:Person

                RockMigrationHelper.AddActionTypeAttributeValue("32D95BD0-48CA-4CA4-BD50-64D774D02EAA", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Protection Application:Complete Request:Background Check Passed:Active

                RockMigrationHelper.AddActionTypeAttributeValue("32D95BD0-48CA-4CA4-BD50-64D774D02EAA", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Protection Application:Complete Request:Background Check Passed:Order

                RockMigrationHelper.AddActionTypeAttributeValue("32D95BD0-48CA-4CA4-BD50-64D774D02EAA", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"daf87b87-3d1e-463d-a197-52227fe4ea28"); // Protection Application:Complete Request:Background Check Passed:Person Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("32D95BD0-48CA-4CA4-BD50-64D774D02EAA", "94689BDE-493E-4869-A614-2D54822D747C", @"True"); // Protection Application:Complete Request:Background Check Passed:Value|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("6BA7F1D9-0DD7-43A2-8865-7970135064E8", "E5BAC4A6-FF7F-4016-BA9C-72D16CB60184", @"False"); // Protection Application:Complete Request:Background Check Failed:Active

                RockMigrationHelper.AddActionTypeAttributeValue("6BA7F1D9-0DD7-43A2-8865-7970135064E8", "E456FB6F-05DB-4826-A612-5B704BC4EA13", @"c495d2e1-9fcc-477b-b8c3-c92f28993354"); // Protection Application:Complete Request:Background Check Failed:Person

                RockMigrationHelper.AddActionTypeAttributeValue("6BA7F1D9-0DD7-43A2-8865-7970135064E8", "3F3BF3E6-AD53-491E-A40F-441F2AFCBB5B", @""); // Protection Application:Complete Request:Background Check Failed:Order

                RockMigrationHelper.AddActionTypeAttributeValue("6BA7F1D9-0DD7-43A2-8865-7970135064E8", "8F4BB00F-7FA2-41AD-8E90-81F4DFE2C762", @"daf87b87-3d1e-463d-a197-52227fe4ea28"); // Protection Application:Complete Request:Background Check Failed:Person Attribute

                RockMigrationHelper.AddActionTypeAttributeValue("6BA7F1D9-0DD7-43A2-8865-7970135064E8", "94689BDE-493E-4869-A614-2D54822D747C", @"False"); // Protection Application:Complete Request:Background Check Failed:Value|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("E9D1E0FC-041B-4A16-933F-945F15E16F60", "36197160-7D3D-490D-AB42-7E29105AFE91", @"False"); // Protection Application:Complete Request:Notify Requester:Active

                RockMigrationHelper.AddActionTypeAttributeValue("E9D1E0FC-041B-4A16-933F-945F15E16F60", "D1269254-C15A-40BD-B784-ADCC231D3950", @""); // Protection Application:Complete Request:Notify Requester:Order

                RockMigrationHelper.AddActionTypeAttributeValue("E9D1E0FC-041B-4A16-933F-945F15E16F60", "9F5F7CEC-F369-4FDF-802A-99074CE7A7FC", @""); // Protection Application:Complete Request:Notify Requester:From Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("E9D1E0FC-041B-4A16-933F-945F15E16F60", "0C4C13B8-7076-4872-925A-F950886B5E16", @"e48cd3df-b284-4dd8-87e4-35f7d120ff7f"); // Protection Application:Complete Request:Notify Requester:Send To Email Address|Attribute Value

                RockMigrationHelper.AddActionTypeAttributeValue("E9D1E0FC-041B-4A16-933F-945F15E16F60", "5D9B13B6-CD96-4C7C-86FA-4512B9D28386", @"Background Check for {{ Workflow.Person }}"); // Protection Application:Complete Request:Notify Requester:Subject

                RockMigrationHelper.AddActionTypeAttributeValue("E9D1E0FC-041B-4A16-933F-945F15E16F60", "4D245B9E-6B03-46E7-8482-A51FBA190E4D", @"{{ GlobalAttribute.EmailHeader }}

<p>{{ Person.FirstName }},</p>
<p>The background check for {{ Workflow.Person }} has been completed.</p>
<p>Result: {{ Workflow.ReportStatus | Upcase }}<p/>

{{ GlobalAttribute.EmailFooter }}"); // Protection Application:Complete Request:Notify Requester:Body

                RockMigrationHelper.AddActionTypeAttributeValue("0D1E4FAF-D21A-4E2D-BEAE-3684F8429FE7", "0CA0DDEF-48EF-4ABC-9822-A05E225DE26C", @"False"); // Protection Application:Complete Request:Complete Workflow:Active

                RockMigrationHelper.AddActionTypeAttributeValue("0D1E4FAF-D21A-4E2D-BEAE-3684F8429FE7", "25CAD4BE-5A00-409D-9BAB-E32518D89956", @""); // Protection Application:Complete Request:Complete Workflow:Order

                RockMigrationHelper.AddActionTypeAttributeValue("EE0705B7-D2BA-4BEE-85E3-0BF131D3538C", "361A1EC8-FFD0-4880-AF68-91DC0E0D7CDC", @"False"); // Protection Application:Cancel Request:Delete Workflow:Active

                RockMigrationHelper.AddActionTypeAttributeValue("EE0705B7-D2BA-4BEE-85E3-0BF131D3538C", "79D23F8B-0DC8-4B48-8A86-AEA48B396C82", @""); // Protection Application:Cancel Request:Delete Workflow:Order

                #endregion
                #region Bio block update for Protection App workflow
                // Update the Bio block's WorkflowAction attribute value to include this new Protection Application
                RockMigrationHelper.AddBlockAttributeValue("B5C1FDB6-0224-43E4-8E26-6B2EAF86253A", "7197A0FB-B330-43C4-8E62-F3C14F649813", "2B2567EE-6920-4DC1-B2F4-2DE774AAD5A6", appendToExisting: true);
                #endregion
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("Cannot insert duplicate key"))
                    throw;
            }
        }

        /// <summary>
        /// Builds the pages.
        /// </summary>
        private void BuildPages()
        {
            #region Protection Application Page
            string protectionAppPageID = "5D612B47-4C1F-49CB-B71B-1BCC9AB2446E";

            RockMigrationHelper.AddPage("C0854F84-2E8B-479C-A3FB-6B47BE89B795", "5FEAF34C-7FB6-4A11-8A1E-C452EC7849BD", "Protection Application", "", protectionAppPageID); // Site:Rock RMS
            RockMigrationHelper.AddPageRoute(protectionAppPageID, "MyAccount/ProtectionApp/{rckipid}");
            RockMigrationHelper.AddLayout("F3F82256-2D66-432B-9D67-3552CD2F4C2B", "Blank", "Blank", "", "DA599675-3FEA-4951-B160-4F3A6EE656B5"); // Site:External Website
            RockMigrationHelper.UpdateBlockType("Send Protection App Request", "Block for selecting criteria to build a list of people who should receive a Protection App request.", "~/Plugins/org_willowcreek/ProtectionApp/ProtectionAppSendRequest.ascx", "org_willowcreek > Protection App", "E28D32A8-74EB-4604-96F2-03434316BB64");
            RockMigrationHelper.UpdateBlockType("Protection App Questionnaire Form", "Displays the questionnaire form of the Protection App.", "~/Plugins/org_willowcreek/ProtectionApp/QuestionnaireForm.ascx", "org_willowcreek > Protection App", "485C49D5-0D28-4A95-A101-68E93252E176");

            // Add/Update PageContext for Page:Workflow Configuration, Entity: Rock.Model.WorkflowType, Parameter: WorkflowTypeId
            RockMigrationHelper.UpdatePageContext(protectionAppPageID, "Rock.Model.WorkflowType", "WorkflowTypeId", "5CBC153D-874C-4465-B529-56323CAF67FA");

            // Add Block to Page: ProtectionApp, Site: External Website
            RockMigrationHelper.AddBlock(protectionAppPageID, "", "485C49D5-0D28-4A95-A101-68E93252E176", "Protection App Questionnaire Form", "Feature", "", "", 0, "C7E85C12-0DDF-42B3-AC9B-8B22A5D5FF91");

            ////External site
            //protectionAppPageID = Guid.NewGuid().ToString();
            //RockMigrationHelper.AddPage("85F25819-E948-4960-9DDF-00F54D32444E", "5FEAF34C-7FB6-4A11-8A1E-C452EC7849BD", "Protection Application", "", protectionAppPageID, "fa fa-check-square-o"); // Site:External Site
            //RockMigrationHelper.AddPageRoute(protectionAppPageID, "ProtectionApp");

            //// Add Block to Page: ProtectionApp, Site: External Website
            //RockMigrationHelper.AddBlock(protectionAppPageID, "", "A2BAC700-0B40-496C-87E7-3617555D1590", "Protection App", "Feature", "", "", 0, "C7E85C12-0DDF-42B3-AC9B-8B22A5D5FF91");

            #endregion

            #region Send Protection Apps Page
            // Page: Send Protection Apps
            RockMigrationHelper.AddPage("B0F4B33D-DD11-4CCC-B79D-9342831B8701", "D65F783D-87A9-4CC9-8110-E83466A0EADB", "Protection Apps", "", "FF82C946-8275-498B-8505-09EF0ECCA123", ""); // Site:Rock RMS
            RockMigrationHelper.UpdateBlockType("Send Protection App Request", "Block for selecting criteria to build a list of people who should receive a Protection App request.", "~/Plugins/org_willowcreek/ProtectionApp/ProtectionAppSendRequest.ascx", "org_willowcreek > ProtectionAppRequest", "A9213F2A-BF82-4FAA-9FC9-A34BDB12551B");
            RockMigrationHelper.AddBlock("FF82C946-8275-498B-8505-09EF0ECCA123", "", "A9213F2A-BF82-4FAA-9FC9-A34BDB12551B", "Send Protection App Request", "Main", "", "", 0, "BF0A584D-728F-44DB-9580-303CE1367BDB");
            RockMigrationHelper.AddBlockTypeAttribute("A9213F2A-BF82-4FAA-9FC9-A34BDB12551B", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Maximum Recipients", "MaximumRecipients", "", "The maximum number of recipients allowed before communication will need to be approved", 0, @"300", "DAEADB4E-487B-4BAD-98D5-20C49750E811");
            #endregion
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            #region Protection App Page
            // Remove Block: Protection App, from Page: Protection App, Site: External Website
            RockMigrationHelper.DeleteBlock("C7E85C12-0DDF-42B3-AC9B-8B22A5D5FF91");
            RockMigrationHelper.DeleteBlockType("E28D32A8-74EB-4604-96F2-03434316BB64"); // Send Protection App
            RockMigrationHelper.DeleteBlockType("485C49D5-0D28-4A95-A101-68E93252E176"); // Protection App
            // Delete PageContext for Page:Workflow Configuration, Entity: Rock.Model.WorkflowType, Parameter: WorkflowTypeId
            RockMigrationHelper.DeletePageContext("5CBC153D-874C-4465-B529-56323CAF67FA");
            #endregion

            #region Send Protection Apps Page
            RockMigrationHelper.DeleteAttribute("DAEADB4E-487B-4BAD-98D5-20C49750E811");
            RockMigrationHelper.DeleteBlock("BF0A584D-728F-44DB-9580-303CE1367BDB");
            RockMigrationHelper.DeleteBlockType("A9213F2A-BF82-4FAA-9FC9-A34BDB12551B");
            RockMigrationHelper.DeletePage("FF82C946-8275-498B-8505-09EF0ECCA123"); //  Page: Send Protection Apps            
            #endregion

            #region New Person Attributes
            #endregion

            #region Protection App Workflow
            #endregion

            #region Bio block attribute removal
            // As far as I can tell, there is no single item removal delete for a multi-value attribute
            #endregion
        }
    }
}
