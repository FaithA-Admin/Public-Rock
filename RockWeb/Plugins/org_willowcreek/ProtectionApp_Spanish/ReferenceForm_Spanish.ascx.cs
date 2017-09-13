using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Rock;
using Rock.Web.Cache;
using Rock.Web.UI;
using org.willowcreek;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using Rock.Data;
using Rock.Model;
using org.willowcreek.Model.Extensions;
using System.Web;

[DisplayName("Reference Form")]
[Category("org_willowcreek > Protection App Spanish")]
[Description("Displays the reference form of the Protection App.")]
public partial class Plugins_org_willowcreek_ProtectionApp_ReferenceForm_es : Rock.Web.UI.RockBlock
{
    /// <summary>
    /// Gets the reference identifier.
    /// </summary>
    /// <value>The reference identifier.</value>
    public int ReferenceId { get; private set; }
    /// <summary>
    /// Gets or sets the referenced person id.
    /// </summary>
    /// <value>The referenced person id.</value>
    public Guid ReferencePersonAliasGuid { get; private set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    /// <value>The first name.</value>
    public string FirstName { get; private set; }

    /// <summary>
    /// Gets or sets the name of the middle.
    /// </summary>
    /// <value>The name of the middle.</value>
    public string MiddleName { get; private set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    /// <value>The last name.</value>
    public string LastName { get; private set; }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {

        string englishVersionURL = HttpContext.Current.Request.Url.AbsoluteUri.Replace( "ReferenceCheck_Spanish", "ReferenceCheck");
        translateToEnglish.HRef = englishVersionURL;
       
        try
        {
            if (CurrentPersonId.HasValue)
            {
                if (CurrentPerson == null) return;
                ReferencePersonAliasGuid = CurrentPerson.PrimaryAlias.Guid;
                FirstName = CurrentPerson.FirstName;
                MiddleName = CurrentPerson.MiddleName;
                LastName = CurrentPerson.LastName;

                var appDone = false;
                var workflowId = PageParameter("WorkflowId");
                var rService = new ReferenceService(new ProtectionAppContext());

                if (workflowId != null)
                {
                    var context = new RockContext();
                    WorkflowService workflowService = new WorkflowService(context);
                    var workflowInstance = workflowService.Get(Guid.Parse(workflowId));
                    //Check if the workflow attribute has already been supplied, if it has report to user
                    if (workflowInstance == null || workflowInstance.CompletedDateTime.HasValue)
                        appDone = true;

                    if (!appDone)
                    {
                        workflowInstance.LoadAttributes();
                        var theAliasPersonGuids = CurrentPerson.PersonAliasGuids();
                        var refNumber = Convert.ToInt32(workflowInstance.AttributeValues["ReferenceNumber"].Value);
                        var refListWorkflowId = Convert.ToInt32(workflowInstance.AttributeValues["ReferenceListWorkflowId"].Value);
                        var refListWorkflow = workflowService.Get(refListWorkflowId);
                        //Get Applicant PersonAliasGuid
                        var applicantPersonAlias = workflowInstance.AttributeValues["Applicant"].Value;
                        Guid applicantPersonAliasGuid;
                        Guid.TryParse(applicantPersonAlias, out applicantPersonAliasGuid);
                        //Check if the other two references were completed
                        var protEval = new org.willowcreek.ProtectionApp.Logic.ProtectionEvaluator();
                        if (protEval.HasTwoCompletedReferences(context, applicantPersonAliasGuid))
                            appDone = true;

                        if (!appDone)
                        {
                            //Check if the current reference for this person and questionnaire applicant has already been completed
                            //get the ref(s) for this person
                            //Can't compare workflow id since they could be from an existing workflow
                            var r = rService.Queryable().Where(x => theAliasPersonGuids.Contains(x.ReferencePersonAliasGuid) && x.RefNumber == refNumber); //&& x.WorkflowId == workflowId
                            //get the reference that is completed if any
                            var refer = r.FirstOrDefault(x => x.SubmissionDate.HasValue);
                            var completed = Convert.ToBoolean(workflowInstance.AttributeValues["ReferenceComplete"].Value);
                            // if we have a completed reference, they are done
                            if (completed)
                            {
                                appDone = true;
                            }
                            else
                            {
                                refer = r.FirstOrDefault();
                                if (refer != null)
                                    ReferenceId = refer.Id;
                            }
                        }
                        else
                        {
                            //Remove current person, so reference isn't attached to modifying the workflow
                            if (HttpContext.Current != null && HttpContext.Current.Items.Contains("CurrentPerson"))
                            {
                                HttpContext.Current.Items.Remove("CurrentPerson");
                            }
                            workflowInstance.MarkComplete();
                            workflowInstance.Status = "Not Needed";
                            context.SaveChanges();
                        }
                    }
                }
                else
                    appDone = true;

                //Don't allow reference to fill out form if already complete or reference not found
                if (appDone || ReferenceId <= 0)
                {
                    pnlReference.Visible = false;
                }
                else
                {
                    //we want the html available to client script but we don't want to show it
                    pnlAppDone.Style.Add("display", "none");
                }
            }
        }
        catch { }
    }
}