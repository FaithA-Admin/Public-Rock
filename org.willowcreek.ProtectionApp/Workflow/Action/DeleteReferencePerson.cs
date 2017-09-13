using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;
using org.willowcreek.Model.Extensions;
using org.willowcreek.ProtectionApp.Model;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.FileManagement;
using org.willowcreek.FileManagement.Model;
using org.willowcreek.FileManagement.Data;

namespace org.willowcreek.ProtectionApp.Workflow.Action
{
    [ActionCategory("Protection")]
    [Description("Delete a person used created just as a reference.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Delete Reference Person")]

    //IN
    [WorkflowAttribute("Reference", "(IN) Reference who should be deleted.", true, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" })]

    class DeleteReferencePerson : ActionComponent
    {
        /// <summary>
        /// Delete the reference created by this workflow
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="action"></param>
        /// <param name="entity"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();
            try
            {
                PersonAliasService paService = new PersonAliasService(rockContext);

                //Get Reference Alias
                var referenceGuidString = this.GetAttributeValue(action, "Reference", rockContext);
                Guid referenceGuid = Guid.Parse(referenceGuidString);
                PersonAlias referenceAlias = paService.Get(Guid.Parse(referenceGuid.ToString()));

                return PersonAliasServiceExtension.DeletePersonAlias(rockContext, referenceAlias, ref errorMessages);
            }
            catch(Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                errorMessages.Add(ex.Message);
                return false;
            }
        }
    }
}
