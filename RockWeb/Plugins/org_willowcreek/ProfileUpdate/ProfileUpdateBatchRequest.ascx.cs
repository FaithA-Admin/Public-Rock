// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using org.willowcreek.ProfileUpdate.Model;

namespace RockWeb.Plugins.org_willowcreek.ProfileUpdate
{
    /// <summary>
    /// Block to build a list of people who should receive a photo request.
    /// </summary>
    [DisplayName( "Profile Update Batch Request" )]
    [Category("org_willowcreek > Profile Update")]
    [Description( "Block for selecting criteria to build a list of people who should receive a profile update request." )]
    [CommunicationTemplateField( "Photo Request Template", "The template to use with this block to send requests.", true, "B9A0489C-A823-4C5C-A9F9-14A206EC3B88" )]
    [IntegerField( "Maximum Recipients", "The maximum number of recipients allowed before communication will need to be approved", false, 75 )]
    public partial class ProfileUpdateBatchRequest : Rock.Web.UI.RockBlock
    {
        #region Fields

        // used for private variables

        #endregion

        #region Properties

        Dictionary<string, string> MediumData = new Dictionary<string, string>();

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                BindCheckBoxLists();
            }
        }

        #endregion

        #region Events

        // handlers called by the controls on your block

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {

        }

        /// <summary>
        /// Handles the Click event of the btnSend control and displays the "confirm" button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSend_Click( object sender, EventArgs e )
        {
            nbTestResult.Visible = false;
            var people = GetMatchingPeople().ToList();
            CheckApprovalRequired( people.Count );
            if ( people.Count > 0 )
            {
                SetActionButtons( false );
                nbConfirmMessage.Title = "Please Confirm";
                nbConfirmMessage.NotificationBoxType = NotificationBoxType.Info;
                nbConfirmMessage.Text = string.Format( "This will send an email to {0:#,###,###,##0} recipients. Press confirm to continue.", people.Count );
            }
            else
            {
                SetActionButtons( true );
                nbConfirmMessage.Visible = true;
                nbConfirmMessage.NotificationBoxType = NotificationBoxType.Warning;
                nbConfirmMessage.Title = "Warning";
                nbConfirmMessage.Text = string.Format( "Hmm, that didn't match anyone. Try adjusting your criteria.", people.Count );
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control and resets the form back to the pre-confirm send state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            SetActionButtons( resetToSend: true );
        }

        /// <summary>
        /// Handles the Click event of the btnSendConfirmed control which sends the actual communication.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSendConfirmed_Click( object sender, EventArgs e )
        {
            if ( Page.IsValid && CurrentPerson != null )
            {
                SetActionButtons( resetToSend: true );

                var people = GetMatchingPeople();

                var rockContext = new RockContext();
                var workflowTypeService = new WorkflowTypeService(rockContext);
                var workflowType = workflowTypeService.Get(ProfileUpdateWorkflowHelper.PROFILE_UPDATE_WORKFLOW_TYPE);

                if (workflowType != null)
                {
                    foreach (var person in people)
                    {
                        var workflow = Workflow.Activate(workflowType, "Profile Update");
                        if (workflow != null)
                        {
                            List<string> workflowErrors;

                            var workflowService = new Rock.Model.WorkflowService(rockContext);
                            //if (workflow.Process(rockContext, person, out workflowErrors))
                            if (workflowService.Process(workflow, person, out workflowErrors))
                            {
                                if (workflow.IsPersisted || workflowType.IsPersisted)
                                {
                                    //var workflowService = new Rock.Model.WorkflowService(rockContext);
                                    workflowService.Add(workflow);

                                    rockContext.WrapTransaction(() =>
                                    {
                                        rockContext.SaveChanges();
                                        workflow.SaveAttributeValues(rockContext);
                                        foreach (var activity in workflow.Activities)
                                        {
                                            activity.SaveAttributeValues(rockContext);
                                        }
                                    });

                                    string message = "Profile Update workflows have been activated.";

                                    pnlSuccess.Visible = true;
                                    pnlForm.Visible = false;
                                    nbSuccess.Text = message;
                                }
                            }
                            else
                            {
                                nbError.Title = "Workflow Processing Error(s):";
                                nbError.Text = workflowErrors.AsDelimited("<br/>");
                            }
                        }
                        else
                        {
                            nbError.Text = "Could not activate workflow.";
                        }
                    }
                }
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Determines whether approval is required, and sets the submit button text appropriately
        /// </summary>
        /// <param name="communication">The communication.</param>
        /// <returns>
        ///   <c>true</c> if approval is required for the communication; otherwise, <c>false</c>.
        /// </returns>
        private bool CheckApprovalRequired( int numberOfRecipients )
        {
            int maxRecipients = int.MaxValue;
            int.TryParse( GetAttributeValue( "MaximumRecipients" ), out maxRecipients );
            bool approvalRequired = numberOfRecipients > maxRecipients;

            btnSendConfirmed.Text = ( approvalRequired && !IsUserAuthorized( "Approve" ) ? "Confirm and Submit" : "Confirm and Send" ) + " Communication";

            return approvalRequired;
        }

        /// <summary>
        /// Sets the action buttons to the reset state (pre confirm) or the confirm state.
        /// </summary>
        /// <param name="resetToSend">if set to <c>true</c> [reset to send].</param>
        private void SetActionButtons( bool resetToSend )
        {
            btnSendConfirmed.Visible = !resetToSend;
            nbConfirmMessage.Visible = !resetToSend;
            btnCancel.Visible = !resetToSend;
            //btnSend.Visible = resetToSend;
            if ( resetToSend )
            {
                btnSend.RemoveCssClass( "hidden" );
            }
            else
            {
                btnSend.AddCssClass( "hidden" );
            }

            nbTestResult.Visible = false;
        }

        /// <summary>
        /// Gets the matching people for the criteria on the form.
        /// </summary>
        /// <returns>a queryable containing the matching people</returns>
        private List<Person> GetMatchingPeople()
        {
            PersonService personService = new PersonService( new RockContext() );

            var familyGroupType = GroupTypeCache.GetFamilyGroupType();
            List<int> selectedRoleIds = cblRoles.SelectedValuesAsInt;

            var ageBirthDate = RockDateTime.Now.AddYears( -nbAge.Text.AsInteger() );
            var createdDate = RockDateTime.Now.AddMonths(-nbCreatedLessThan.Text.AsInteger());
            var profileSent = RockDateTime.Now.AddMonths(-nbProfileSentThan.Text.AsInteger());
            var profileCompleted = RockDateTime.Now.AddMonths(-nbProfileCompletedThan.Text.AsInteger());
            bool excludeRestricted = clExcludeRestricted.Checked;

            var people = personService.Queryable("Members", false, false);

            // people who have emails addresses
            people = people.Where( p => ! ( p.Email == null || p.Email.Trim() == string.Empty ) );

            // people who match the Connection Status critera
            people = people.Where( p => cblConnectionStatus.SelectedValuesAsInt.Contains( p.ConnectionStatusValueId ?? -1 ) );

            // people who match the Record Status critera
            people = people.Where( p => cblRecordStatus.SelectedValuesAsInt.Contains( p.RecordStatusValueId ?? -1 ) );

            // people who are old enough
            people = people.Where( p => p.BirthDate <= ageBirthDate );

            // people who are in the matching role for a family group
            people = people.Where( p => p.Members.Where( gm => gm.Group.GroupTypeId == familyGroupType.Id
                && selectedRoleIds.Contains( gm.GroupRoleId ) ).Any() );

            // people who are of the correct email status...
            people = people.Where(p => p.EmailPreference == EmailPreference.EmailAllowed);

            // people not created more than X months ago.
            people = people.Where(p => p.CreatedDateTime >= createdDate );

            // Placeholder for Restricted

            // people except those already sent an email within X months
            // people except those who have completed a profile within X months
            return RefinePeopleByAttributeValue(people, profileSent, profileCompleted);
        }

        /// <summary>
        /// The purpose of this method is to do what can't be done with LINQ in the finding of valid recipients for Profile Update requests.
        /// Instead, the LINQ query will return a list of people, from there, we can peek at each one to see if one of their attributes would
        /// exclude them from yet another profile update mailing.  Basically, you just loop through the people, check an attribute's existence,
        /// and perform a quick date check, removing them from the candidate list if necessary.
        /// </summary>
        /// <param name="people">The original list</param>
        /// <param name="profileSent">The date in which a person's request needed to be sent to disqualify them from another</param>
        /// <param name="profileCompleted">The date in which a person's request needed to be completed to disqualify them from another request</param>
        /// <returns></returns>
        private List<Person> RefinePeopleByAttributeValue(IQueryable<Person> people, DateTime profileSent, DateTime profileCompleted)
        {
            var attributeService = new AttributeService(new RockContext());
            var attributeValueService = new AttributeValueService(new RockContext());

            var candidates = people.ToList();
            var removals = new List<Person>();

            foreach (var candidate in candidates)
            {
                // The IDs for these attributes come from the DataIntegrity migration...
                var sentAttributeObject = attributeService.Get(Guid.Parse("3307D1DC-F5BC-45D2-96B8-41FF0A048DCF"));
                var sentAttributeValueObject = attributeValueService.GetByAttributeIdAndEntityId(sentAttributeObject.Id, candidate.Id);
                var completedAttributeObject = attributeService.Get(Guid.Parse("E862BA6A-8046-4E19-B7C6-51869BF2BA1E"));
                var completedAttributeValueObject = attributeValueService.GetByAttributeIdAndEntityId(completedAttributeObject.Id, candidate.Id);

                bool removed = false;
                DateTime sentDate = DateTime.MinValue;
                DateTime completedDate = DateTime.MinValue;

                if (sentAttributeValueObject != null)
                {
                    if (DateTime.TryParse(sentAttributeValueObject.Value, out sentDate))
                    {
                        if (sentDate >= profileSent)
                        {
                            removals.Add(candidate);
                            removed = true;
                        }
                    }
                }

                if (!removed && completedAttributeValueObject != null)
                {
                    if(DateTime.TryParse(completedAttributeValueObject.Value, out completedDate))
                    {
                        if (completedDate >= profileCompleted)
                        {
                            removals.Add(candidate);
                            removed = true;
                        }
                    }
                }
            }

            foreach(var removal in removals)
            {
                candidates.Remove(removal);
            }

            return candidates;
        }

        /// <summary>
        /// Binds the checkbox lists.
        /// </summary>
        private void BindCheckBoxLists()
        {
            // roles...
            var familyGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid() );
            cblRoles.DataSource = familyGroupType.Roles;
            cblRoles.DataBind();

            // Set the selected value in the control to ADULT...
            cblRoles.SelectedValue = "3";

            // otherwise we can just bind like this for now
            cblConnectionStatus.BindToDefinedType( DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.PERSON_CONNECTION_STATUS.AsGuid() ) );
            foreach (var item in cblConnectionStatus.Items)
            {
                var listItem = item as ListItem;
                listItem.Selected = true;
            }

            // Bind to active record status type and preselect...
            cblRecordStatus.BindToDefinedType(DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.PERSON_RECORD_STATUS.AsGuid()));
            cblRecordStatus.SelectedValue = "3";
        }

        #endregion

}
}
