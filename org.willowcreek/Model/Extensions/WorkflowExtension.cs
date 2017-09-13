using Rock.Data;
using Rock.Model;

using System;
using System.Collections.Generic;
using System.Linq;

namespace org.willowcreek.Model.Extensions
{

    public static class WorkflowExtension
    {
        /// <summary>
        /// Retrieves a list of workflows based on workflow type and two participating people.
        /// </summary>
        /// <param name="workflowTypeId">The workflow type of the calling workflow.</param>
        /// <param name="attributeId1">The attribute Id for the first person.</param>
        /// <param name="attributeValue1">The attribute value containing the first person alias guid.</param>
        /// <param name="attributeId2">The attribute Id for the second person.</param>
        /// <param name="attributeValue2">The attribute value containing the second person alias guid.</param>
        /// <param name="excludeId">The Id of the workflow to exclude. Usually, the new workflow Id.</param>
        /// <returns>
        ///   List<Workflow>
        /// </returns>
        public static List<Rock.Model.Workflow> Get(this WorkflowService workflowService, int workflowTypeId, int? attributeId1, Guid? attributeValue1, int? attributeId2, Guid? attributeValue2, int? excludeId)
        {
            var rockContext = (RockContext)workflowService.Context;
            return workflowService.Queryable().Join(rockContext.AttributeValues, w => w.Id, av1 => av1.EntityId, (w, av1) => new { w, av1 })
                                              .Join(rockContext.AttributeValues, wav1 => wav1.w.Id, av2 => av2.EntityId, (wav1, av2) => new { wav1, av2 })
                                              .Where(r => r.wav1.av1.AttributeId == attributeId1
                                                        && r.wav1.av1.Value == attributeValue1.ToString()
                                                        && r.av2.AttributeId == attributeId2
                                                        && r.av2.Value == attributeValue2.ToString()
                                                        && r.wav1.w.WorkflowTypeId == workflowTypeId
                                                        && r.wav1.w.CompletedDateTime == null
                                                        && r.wav1.w.Id != excludeId)
                                              .Select(r => r.wav1.w).ToList();
        }

        /// <summary>
        /// Cancels workflows and sets status.
        /// </summary>
        /// <param name="workflows">List of workflows to cancel.</param>
        /// <param name="status">Status the workflows should be set to.</param>
        /// <returns></returns>
        public static void Cancel(this WorkflowService workflowService, List<Rock.Model.Workflow> workflows, string status)
        {
            var rockContext = (RockContext)workflowService.Context;
            foreach (var workflow in workflows)
            {
                workflow.Status = status;
                workflow.CompletedDateTime = DateTime.Now;
                rockContext.SaveChanges();
            }
        }

        /// <summary>
        /// Cancels a list of workflows based on workflow type and two participating people.
        /// </summary>
        /// <param name="workflowTypeId">The workflow type of the calling workflow.</param>
        /// <param name="attributeId1">The attribute Id for the first person.</param>
        /// <param name="attributeValue1">The attribute value containing the first person alias guid.</param>
        /// <param name="attributeId2">The attribute Id for the second person.</param>
        /// <param name="attributeValue2">The attribute value containing the second person alias guid.</param>
        /// <param name="excludeId">The Id of the workflow to exclude. Usually, the new workflow Id.</param>
        /// <param name="status">Status the workflows should be set to</param>
        /// <returns>
        ///   List<Workflow>
        /// </returns>
        public static List<Rock.Model.Workflow> Cancel(this WorkflowService workflowService, int workflowTypeId, int? attributeId1, Guid? attributeValue1, int? attributeId2, Guid? attributeValue2, int? excludeId, string status)
        {
            List<Rock.Model.Workflow> workflows = Get(workflowService, workflowTypeId, attributeId1, attributeValue1, attributeId2, attributeValue2, excludeId);
            Cancel(workflowService, workflows, status);
            return workflows;
        }

        /// <summary>
        /// Get a list of workflows beyond their expiration date.
        /// </summary>
        /// <param name="days">Number of days before a workflow expires.</param>
        /// <param name="workflowTypeIds">A list of workflow type Ids to consider.</param>
        /// <returns>
        ///   List<Workflow>
        /// </returns>
        public static List<Rock.Model.Workflow> GetExpired(this WorkflowService workflowService, int days, string workflowTypeIds)
        {                        
            List<int> worklfowTypesList = new List<int>();

            foreach(var workflowType in workflowTypeIds.Split(',').ToList<String>())
            {
                int workflowTypeId=-1;
                if(int.TryParse(workflowType.Trim(), out workflowTypeId))
                {
                    worklfowTypesList.Add(workflowTypeId);
                }
            }
            
            List<Rock.Model.Workflow> expiredWorkFlows = workflowService.Queryable().Where(w => w.CompletedDateTime == null && worklfowTypesList.Contains(w.WorkflowTypeId)).ToList<Rock.Model.Workflow>()
                                                                                    .Where(w => w.CreatedDateTime.Value < DateTime.Now.AddDays(-days)).ToList<Rock.Model.Workflow>();
            
            return expiredWorkFlows;
        }

        /// <summary>
        /// Expires a list of workflows beyond their limit of days to execute.
        /// </summary>
        /// <param name="days">Number of days before a workflow expires.</param>
        /// <param name="workflowTypeIds">A list of workflow type Ids to consider.</param>
        /// <returns>
        ///   List<Workflow>
        /// </returns>
        public static List<Rock.Model.Workflow> Expire(this WorkflowService workflowService, int days, string workflowTypeIds)
        {
            List<Rock.Model.Workflow> workflows = GetExpired(workflowService, days, workflowTypeIds);
            Cancel(workflowService, workflows, "Expired");
            return workflows;
        }

    }

}
