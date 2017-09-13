using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Rock.Web.UI;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using Rock;
using Rock.Data;
using Rock.Model;
using org.willowcreek.ProtectionApp.Logic;
using System.Web;

[DisplayName("Protection App Questionnaire Form")]
[Category("org_willowcreek > Protection App Spanish")]
[Description("Displays the questionnaire form of the Protection App.")]
public partial class Plugins_org_willowcreek_ProtectionApp_QuestionnaireForm_es : Rock.Web.UI.RockBlock 
{
    #region Fields

    private Questionnaire questionnaire = null;

    #endregion

    #region Base Control Methods

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
        this.BlockUpdated += Block_BlockUpdated;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var context = new RockContext();
        var workflowService = new WorkflowService(context);
        var workflowId = PageParameter("WorkflowId");

        var validationResult = ProtectionRouter.GetValidatedWorkflow(context, workflowService, CurrentPerson, workflowId, 172 );
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
        var qService = new QuestionnaireService(new ProtectionAppContext());
        if (validationResult.Workflow.CompletedDateTime.HasValue || qService.Queryable().Any(x => x.WorkflowId == workflowId))
        {
            ProtectionRouter.GoToNextIncompleteStep(context, validationResult.Workflow, Response, true);
        }

        // We want the html available to client script but we don't want to show it
        pnlAppDone.Visible = true;
        pnlWrongUser.Visible = true;
        pnlQuestionnaire.Visible = true;
        pnlAppDone.Style.Add("display", "none");
        pnlWrongUser.Style.Add("display", "none");

        ApplicantPersonAliasGuid_server.Value = CurrentPerson.PrimaryAlias.Guid.ToString();

        ltlApplicantName.Text = CurrentPerson.FirstName;
        ltlApplicantName2.Text = CurrentPerson.FirstName;

        Nickname_server.Value = CurrentPerson.NickName;
        var phones = CurrentPerson.PhoneNumbers;

        var homePhone = phones.FirstOrDefault(p => p.NumberTypeValueId == 13);
        if (homePhone != null)
            HomePhone_server.Value = homePhone.NumberFormatted;

        var mobilePhone = phones.FirstOrDefault(p => p.NumberTypeValueId == 12);
        if (mobilePhone != null)
            MobilePhone_server.Value = mobilePhone.NumberFormatted;

        Email_server.Value = CurrentPerson.Email;
        DateOfBirth_server.Value = CurrentPerson.BirthDate != null ? CurrentPerson.BirthDate.Value.ToString("MM/dd/yyyy") : null;

        if (CurrentPerson.MaritalStatusValue == null || CurrentPerson.MaritalStatusValue.Id == 163)
            MaritalStatus_server.Value = "144"; //TH - If unknown set to single
        else
            MaritalStatus_server.Value = CurrentPerson.MaritalStatusValue.Id.ToString();

        switch (CurrentPerson.Gender)
        {
            case Gender.Male:
                GenderType.Value = "1";
                break;
            case Gender.Female:
                GenderType.Value = "2";
                break;
            default:
                GenderType.Value = "0";
                break;
        }

        var family = CurrentPerson.GetFamilies().FirstOrDefault(f => f.IsActive && f.GroupTypeId == 10);
        if (family != null)
        {
            var groupLocations = family.GroupLocations.FirstOrDefault(l => l.Location.IsActive && l.GroupLocationTypeValue.Value == "Home");
            if (groupLocations != null)
            {
                var currentLocation = groupLocations.Location;

                Street_server.Value = currentLocation.Street1;
                City_server.Value = currentLocation.City;
                State_server.Value = currentLocation.State;
                Zip_server.Value = currentLocation.PostalCode;
            }
        }

        if (!Page.IsPostBack)
        {
        }
    }

    

    public override List<BreadCrumb> GetBreadCrumbs(Rock.Web.PageReference pageReference)
    {
        var breadCrumbs = new List<BreadCrumb>();

        return breadCrumbs;
    }

    #endregion

    #region Events

    /// <summary>
    /// Handles the BlockUpdated event of the control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Block_BlockUpdated(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Handles the Click event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        NavigateToParentPage();
    }

    /// <summary>
    /// Handles the Click event of the btnSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
    }

    #endregion

    #region Methods

    #endregion
}