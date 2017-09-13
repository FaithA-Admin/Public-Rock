using System;
using System.ComponentModel;
using System.Linq;
using org.willowcreek;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using org.willowcreek.ProtectionApp.Rest;
using org.willowcreek.ProtectionApp.Logic;
using Rock.Model;
using Rock.Data;
using Rock;

[DisplayName("Protection App Questionnaire References")]
[Category("org_willowcreek > Protection App Spanish")]
[Description("Displays the questionnaire references form of the Protection App.")]
public partial class Plugins_org_willowcreek_ProtectionApp_QuestionnaireReferences_es : Rock.Web.UI.RockBlock 
{
    #region Reference1
    protected bool HasReference1 { get; set; }
    protected string Reference1Name { get; set; }
    protected string Reference1Association { get; set; }
    protected string Reference1Email { get; set; }
    protected bool Reference1Complete { get; set; }
    protected string Reference1Status { get; set; }
    protected Guid Reference1PersonAliasGuid { get; set; }
    #endregion
    #region Reference2
    protected bool HasReference2 { get; set; }
    protected string Reference2Name { get; set; }
    protected string Reference2Association { get; set; }
    protected string Reference2Email { get; set; }
    protected bool Reference2Complete { get; set; }
    protected string Reference2Status { get; set; }
    protected Guid Reference2PersonAliasGuid { get; set; }
    #endregion
    #region Reference3
    protected bool HasReference3 { get; set; }
    protected string Reference3Name { get; set; }
    protected string Reference3Association { get; set; }
    protected string Reference3Email { get; set; }
    protected bool Reference3Complete { get; set; }
    protected string Reference3Status { get; set; }
    protected Guid Reference3PersonAliasGuid { get; set; }
    #endregion
    protected Guid CurrentPersonAliasGuid { get; set; }
    protected string FullLegalName { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        var context = new RockContext();
        var workflowService = new WorkflowService(context);
        var workflowId = PageParameter("WorkflowId");

        // Hopefully temporary problem of the url key pointing to a PersonAlias rather than a Person due to a previous bug.
        // This should no longer be necessary by 9/1/2016.
        if (CurrentPerson == null)
        {
            string impersonatedPersonKey = PageParameter("rckipid");
            if (!string.IsNullOrEmpty(impersonatedPersonKey))
            {
                var personAliasService = new PersonAliasService(context);
                var applicantAlias = personAliasService.GetByUrlEncodedKey(impersonatedPersonKey);
                var applicant = applicantAlias.Person;
                var url = "/MyAccount/ProtectionAppReferences_Spanish/" + applicant.UrlEncodedKey + "?WorkflowId=" + workflowId;
                Response.Redirect(url);
            }
        }

        var validationResult = ProtectionRouter.GetValidatedWorkflow(context, workflowService, CurrentPerson, workflowId, 173);
        if (validationResult.Workflow == null)
        {
            pnlAppDone.Visible = true;
            pnlWrongUser.Visible = false;
            pnlQuestionnaire.Visible = false;
            return;
        }
        if (validationResult.ValidApplicant == false)
        {
            pnlAppDone.Visible = false;
            pnlWrongUser.Visible = true;
            pnlQuestionnaire.Visible = false;
            return;
        }

        // The questionnaire may have been completed already even if the workflow has not been marked completed by the job yet
        if (validationResult.Workflow.CompletedDateTime.HasValue)
        {
            ProtectionRouter.GoToNextIncompleteStep(context, validationResult.Workflow, Response, true);
        }

        pnlAppDone.Visible = false;
        pnlWrongUser.Visible = false;
        pnlQuestionnaire.Visible = true;

        var globalAttributes = Rock.Web.Cache.GlobalAttributesCache.Read();

        //Get days to go back to retrieve references
        var populateRefDays = Convert.ToInt32(globalAttributes.GetValue("ProtectionPopulateReferencesDays"));

        //Get last 3 references created for this applicant within given timeframe 
        var protectionContext = new ProtectionAppContext();
        var rService = new ReferenceService(protectionContext);
        var references = rService.Queryable().Where(x => x.ApplicantPersonAliasGuid == CurrentPersonAlias.Guid).ToList();
        references = references.Where(x => x.CreatedDateTime >= DateTime.Now.AddDays(-populateRefDays)).OrderByDescending(x => x.CreatedDateTime).ToList();

        FullLegalName = CurrentPerson.FullLegalName();

        if(references.Count > 0)
        {
            //var context = new ProtectionAppContext();
            //var refs = context.References.AsQueryable().Where(x => x.QuestionnaireId == ques.Id).OrderByDescending(x => x.CreatedDateTime).Take(3).ToList();

            var ref1 = references.FirstOrDefault(r => r.RefNumber == 1);
            var ref2 = references.FirstOrDefault(r => r.RefNumber == 2);
            var ref3 = references.FirstOrDefault(r => r.RefNumber == 3);

            var referenceController = new ReferenceController();
            var refYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionReferenceExpiration"));

            //Get Current person (applicant) to determine complete
            var applicant = CurrentPerson;
            applicant.LoadAttributes(context);

            //Validate references are still relevent
            if (ref1 != null)
            {
                HasReference1 = ref1.SubmissionDate == null || ref1.SubmissionDate > DateTime.Now.AddYears(-refYears);
                //Only add reference info if they are not negatively completed
                if (HasReference1 && (ref1.SubmissionDate == null || referenceController.IsReferenceApproved(ref1)))
                {
                    var refValid = referenceController.IsValidReference(ref1);
                    Reference1Name = (ref1.FirstName + " " + ref1.LastName).EscapeQuotes();
                    Reference1Association = ref1.NatureOfRelationshipApplicant;
                    Reference1Email = ref1.Email.EscapeQuotes();
                    //Check person record for completed date
                    var ref1Date = applicant.AttributeValues["ProtectionReference1Date"].Value;
                    Reference1Complete = !string.IsNullOrEmpty(ref1Date);
                    Reference1Status = Reference1Complete && refValid
                        ? "Complete"
                        : (!Reference1Complete ? "Incomplete" : "Invalid");
                    Reference1Status = CreateStatusButton(Reference1Status);
                    Reference1PersonAliasGuid = ref1.ReferencePersonAliasGuid;
                }
                else
                {
                    HasReference1 = false;
                }
            }
            if (ref2 != null)
            {
                HasReference2 = ref2.SubmissionDate == null || ref2.SubmissionDate > DateTime.Now.AddYears(-refYears);
                if (HasReference2 && (ref2.SubmissionDate == null || referenceController.IsReferenceApproved(ref2)))
                {
                    var refValid = referenceController.IsValidReference(ref2);
                    Reference2Name = (ref2.FirstName + " " + ref2.LastName).EscapeQuotes();
                    Reference2Association = ref2.NatureOfRelationshipApplicant;
                    Reference2Email = ref2.Email.EscapeQuotes();
                    var ref2Date = applicant.AttributeValues["ProtectionReference2Date"].Value;
                    Reference2Complete = !string.IsNullOrEmpty(ref2Date);
                    Reference2Status = Reference2Complete && refValid
                        ? "Complete"
                        : (!Reference2Complete ? "Incomplete" : "Invalid");
                    Reference2Status = CreateStatusButton(Reference2Status);
                    Reference2PersonAliasGuid = ref2.ReferencePersonAliasGuid;
                }
                else
                {
                    HasReference2 = false;
                }
            }
            if (ref3 != null)
            {
                HasReference3 = ref3.SubmissionDate == null || ref3.SubmissionDate > DateTime.Now.AddYears(-refYears);
                if (HasReference3 && (ref3.SubmissionDate == null || referenceController.IsReferenceApproved(ref3)))
                {
                    var refValid = referenceController.IsValidReference(ref3);
                    Reference3Name = (ref3.FirstName + " " + ref3.LastName).EscapeQuotes();
                    Reference3Association = ref3.NatureOfRelationshipApplicant;
                    Reference3Email = ref3.Email.EscapeQuotes();
                    var ref3Date = applicant.AttributeValues["ProtectionReference3Date"].Value;
                    Reference3Complete = !string.IsNullOrEmpty(ref3Date);
                    Reference3Status = Reference3Complete && refValid
                        ? "Complete"
                        : (!Reference3Complete ? "Incomplete" : "Invalid");
                    Reference3Status = CreateStatusButton(Reference3Status);
                    Reference3PersonAliasGuid = ref3.ReferencePersonAliasGuid;
                }
                else
                {
                    HasReference3 = false;
                }
            }
        }

        if (HasReference1 || HasReference2 || HasReference3)
            ResendText.Visible = true;
        else
            ResendText.Visible = false;

    }

    /// <summary>
    /// Method to create the status column of the reference list
    /// </summary>
    /// <param name="refStatus"></param>
    /// <returns></returns>
    private string CreateStatusButton(string refStatus)
    {
        var btnStyle = "";
        switch (refStatus)
        {
            case "Complete":
                btnStyle = "success";
                break;
            case "Invalid":
                btnStyle = "danger";
                break;
            case "Incomplete":
            default:
                btnStyle = "default";
                break;
        }
        return string.Format("<button class=\"btn btn-{0}\" disabled>{1}</button>", btnStyle, refStatus);
    }
}