using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Quartz;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Attribute = Rock.Model.Attribute;

namespace org.willowcreek.Duplicates.Jobs
{
    public class LinkBackgroundChecks : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            PerformLinkBackgroundChecks();
        }

        public void PerformLinkBackgroundChecks()
        {
            var rockContext = new RockContext();
            var attributeValueService = new AttributeValueService(rockContext);
            var attributeService = new AttributeService(rockContext);
            var personAliasService = new PersonAliasService(rockContext);
            var linkCount = 0;

            // Get the list of workflows without a report from the database...
            var workflows = GetWorkflowsFromDatabase(rockContext);

            //Foreach Workflow without a report
            foreach (var workflow in workflows)
            {
                workflow.LoadAttributes();

                //Get the report link for that workflow
                var workflowReportLink = attributeValueService.Queryable().FirstOrDefault(a => a.AttributeId == 1967 && a.EntityId == workflow.Id);
                if (workflowReportLink != null && workflowReportLink.Value != "")
                {
                    var reportLink = workflowReportLink.Value;

                    Guid? binaryFileGuid = SaveFile(workflow.Attributes["Report"], reportLink, workflow.Id + ".pdf");
                    if (binaryFileGuid.HasValue)
                    {
                        // Save the report to the workflow
                        var reportAttribute = attributeService.Queryable().FirstOrDefault(a => a.Id == 1966);
                        SaveAttributeValue(workflow.Id, reportAttribute, binaryFileGuid.ToString(), rockContext);

                        //Save the report to the person
                        var workflowApplicant = attributeValueService.Queryable().FirstOrDefault(a => a.EntityId == workflow.Id && a.AttributeId == 1987);
                        if (workflowApplicant != null)
                        {
                            var personAliasGuid = workflowApplicant.Value;
                            var personAlias = personAliasService.Queryable().FirstOrDefault(p => p.Guid.ToString() == personAliasGuid);
                            if (personAlias != null)
                            {
                                var personId = personAlias.PersonId;
                                var backgroundCheckAttribute = attributeService.Queryable().FirstOrDefault(a => a.Id == 1300);
                                SaveAttributeValue(personId, backgroundCheckAttribute, binaryFileGuid.ToString(), rockContext);
                            }
                        }
                        
                        linkCount++;
                    }

                }

            }

            ExceptionLogService.LogException(new Exception("Successfully Linked " + linkCount + " missing background checks."), null);
        }

        public List<Workflow> GetWorkflowsFromDatabase(RockContext rockContext)
        {
            var workflowService = new WorkflowService(rockContext);
            var attributeValueService = new AttributeValueService(rockContext);

            var reportLinkList = attributeValueService.Queryable()
                .Where(w => w.AttributeId == 1967 && w.Value != null && w.Value != "").Select(w => w.EntityId).ToList();  //Get workflows with a report link

            var missingReportList = attributeValueService.Queryable()
                .Where(w => w.AttributeId == 1966 && String.IsNullOrEmpty(w.Value) && reportLinkList.Contains(w.EntityId)).Select(w => w.EntityId).ToList(); //Get Workflows with no Report

            var workflowList = missingReportList.Select(workflowId => workflowService.Queryable().FirstOrDefault(w => w.Id == workflowId)).Where(workflow => workflow != null).ToList();

            return workflowList;
        }

        #region Rock Code

        private static Guid? SaveFile(AttributeCache binaryFileAttribute, string url, string fileName)
        {
            // get BinaryFileType info
            if (binaryFileAttribute != null &&
                binaryFileAttribute.QualifierValues != null &&
                binaryFileAttribute.QualifierValues.ContainsKey("binaryFileType"))
            {
                Guid? fileTypeGuid = binaryFileAttribute.QualifierValues["binaryFileType"].Value.AsGuidOrNull();
                if (fileTypeGuid.HasValue)
                {
                    RockContext rockContext = new RockContext();
                    BinaryFileType binaryFileType = new BinaryFileTypeService(rockContext).Get(fileTypeGuid.Value);

                    if (binaryFileType != null)
                    {
                        byte[] data = null;

                        using (WebClient wc = new WebClient())
                        {
                            data = wc.DownloadData(url);
                        }

                        BinaryFile binaryFile = new BinaryFile();
                        binaryFile.Guid = Guid.NewGuid();
                        binaryFile.IsTemporary = true;
                        binaryFile.BinaryFileTypeId = binaryFileType.Id;
                        binaryFile.MimeType = "application/pdf";
                        binaryFile.FileName = fileName;
                        binaryFile.ContentStream = new MemoryStream(data);

                        var binaryFileService = new BinaryFileService(rockContext);
                        binaryFileService.Add(binaryFile);

                        rockContext.SaveChanges();

                        return binaryFile.Guid;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Saves the attribute value.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <remarks>
        /// If a rockContext value is included, this method will save any previous changes made to the context
        /// </remarks>
        public static void SaveAttributeValue(int entityId, Attribute attribute, string newValue, RockContext rockContext = null)
        {
            if (attribute != null)
            {
                rockContext = rockContext ?? new RockContext();
                var attributeValueService = new AttributeValueService(rockContext);

                var attributeValue = attributeValueService.GetByAttributeIdAndEntityId(attribute.Id, entityId);
                if (attributeValue == null)
                {
                    if (newValue == null)
                    {
                        return;
                    }

                    attributeValue = new AttributeValue();
                    attributeValue.AttributeId = attribute.Id;
                    attributeValue.EntityId = entityId;
                    attributeValueService.Add(attributeValue);
                }

                attributeValue.Value = newValue;

                rockContext.SaveChanges();
            }
        }        

        #endregion
    }
}
