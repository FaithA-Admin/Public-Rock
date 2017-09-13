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
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using org.willowcreek.ProfileUpdate.Model;
using org.willowcreek.SystemGuid;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_willowcreek.ProfileUpdate
{
    /// <summary>
    /// Allows a person to edit their account information. 
    /// </summary>
    [DisplayName( "Profile Update" )]
    [Category( "org_willowcreek > Profile Update" )]
    [Description( "Allows a person to edit their profile information." )]
    [CustomDropdownListField( "Connection Status", "Connection status to set on new family members.", "65^Member, 66^Visitor, 67^Web Prospect, 146^Attendee, 203^Participant, 898^New, 899^Reference, 1449^Care Center Guest, 3248^Promiseland Family, 3243^Promiseland Guest", true, "", "" )]
    public partial class ProfileUpdate : PersonBlock
    {
        protected string basePersonUrl;

        #region Social Media Properties
        // View modes
        private readonly string VIEW_MODE_VIEW = "VIEW";
        private readonly string VIEW_MODE_EDIT = "EDIT";
        private readonly string VIEW_MODE_ORDER = "ORDER";
        private bool _canAdministrate = true;

        /// <summary>
        /// Gets or sets the attribute list.
        /// </summary>
        /// <value>
        /// The attribute list.
        /// </value>
        protected List<int> AttributeList
        {
            get
            {
                List<int> attributeList = ViewState["AttributeList"] as List<int>;
                if ( attributeList == null )
                {
                    attributeList = new List<int>();
                    ViewState["AttributeList"] = attributeList;
                }
                return attributeList;
            }
            set { ViewState["AttributeList"] = value; }
        }

        /// <summary>
        /// Gets or sets the view mode.
        /// </summary>
        /// <value>
        /// The view mode.
        /// </value>
        protected string ViewMode
        {
            get
            {
                var viewMode = ViewState["ViewMode"];
                if ( viewMode == null )
                {
                    return VIEW_MODE_VIEW;
                }
                else
                {
                    return viewMode.ToString();
                }
            }
            set
            {
                ViewState["ViewMode"] = value;
            }
        }
        #endregion

        #region Family Edit Properties
        private Group _family = null;
        private bool _canEdit = true;
        private List<DefinedValue> addressTypes = new List<DefinedValue>();
        private List<GroupTypeRole> familyRoles = new List<GroupTypeRole>();

        private string Language
        {
            get { return ViewState["Language"] as string; }
            set { ViewState["Language"] = value; }
        }

        private int? FamilyMemberToEdit
        {
            get { return ViewState["FamilyMemberToEdit"] as int?; }
            set { ViewState["FamilyMemberToEdit"] = value; }
        }

        private List<FamilyMember> FamilyMembers
        {
            get { return ViewState["FamilyMembers"] as List<FamilyMember>; }
            set { ViewState["FamilyMembers"] = value; }
        }

        #endregion

        private string DefaultState
        {
            get
            {
                string state = ViewState["DefaultState"] as string;
                if ( state == null )
                {
                    string orgLocGuid = GlobalAttributesCache.Read().GetValue( "OrganizationAddress" );
                    if ( !string.IsNullOrWhiteSpace( orgLocGuid ) )
                    {
                        Guid locGuid = Guid.Empty;
                        if ( Guid.TryParse( orgLocGuid, out locGuid ) )
                        {
                            var location = new Rock.Model.LocationService( new RockContext() ).Get( locGuid );
                            if ( location != null )
                            {
                                state = location.State;
                                ViewState["DefaultState"] = state;
                            }
                        }
                    }
                }

                return state;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            var rockContext = new RockContext();

            if ( CurrentPerson != null )
            {
                // Check to see if the workflow has expired...
                var attributeService = new AttributeService( rockContext );
                var attributeValueService = new AttributeValueService( rockContext );
                var fieldTypeService = new FieldTypeService( rockContext );
                var fieldType = fieldTypeService.GetByName( "Defined Value" );
                var attributeObject = attributeService.GetByFieldTypeId( fieldType.First().Id ).Single( a => a.Key == "DataStatus" );
                var attributeValueObject = attributeValueService.GetByAttributeIdAndEntityId( attributeObject.Id, CurrentPerson.Id );
                if ( attributeValueObject != null && !String.IsNullOrEmpty( attributeValueObject.Value ) )
                {
                    var dataStatusTypes = DefinedTypeCache.Read( new Guid( "0B4DDE85-152F-4DBE-988E-397C3EFFBC93" ) );
                    var avAsGuid = attributeValueObject.Value.AsGuid();
                    var selectedSection = dataStatusTypes.DefinedValues.Single( v => v.Guid == avAsGuid );
                    if ( selectedSection.Guid.ToString() == "9EB99C51-E637-4B9E-B4D5-9C04437D50C4" )
                    {
                        // If the defined value on the person for the DataStatus attribute is ProfileExpired, error out...
                        Response.Redirect( "~/Http404Error", false );
                    }
                }
            }

            basePersonUrl = ResolveUrl( "~/Person/" );
            btnDelete.Attributes["onclick"] = string.Format( "javascript: return Rock.dialogs.confirmDelete(event, '{0}');", "Family" );



            if ( CurrentPerson != null )
            {
                var groupTypeService = new GroupTypeService( rockContext );
                var groupService = new GroupService( rockContext );
                var groupMemberService = new GroupMemberService( rockContext );

                int familyGroupTypeId = groupTypeService.Queryable().Where( gt => gt.Guid.ToString() == "790E3215-3B10-442B-AF69-616C0DCB998E" ).Select( gt => gt.Id ).FirstOrDefault();
                var memberGroups = groupMemberService.GetByPersonId( CurrentPerson.Id );
                _family = groupService.GetByIds( memberGroups.Select( mg => mg.GroupId ).ToList<int>() ).Where( g => g.GroupTypeId == familyGroupTypeId ).FirstOrDefault();

                if ( _family != null && string.Compare( _family.GroupType.Guid.ToString(), Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY, true ) != 0 )
                {
                    nbInvalidFamily.Text = "Sorry, but the group selected is not a Family group";
                    nbInvalidFamily.NotificationBoxType = NotificationBoxType.Danger;
                    nbInvalidFamily.Visible = true;

                    _family = null;
                    pnlEditFamily.Visible = false;
                }
                else if ( _family == null )
                {
                    nbInvalidFamily.Text = "Sorry, but the specified family was not found.";
                    nbInvalidFamily.NotificationBoxType = NotificationBoxType.Danger;
                    nbInvalidFamily.Visible = true;

                    _family = null;
                    pnlEditFamily.Visible = false;
                }
                else
                {
                    familyRoles = _family.GroupType.Roles.OrderBy( r => r.Order ).ToList();
                    rblNewPersonRole.DataSource = familyRoles;
                    rblNewPersonRole.DataBind();

                    addressTypes = _family.GroupType.LocationTypes.Select( l => l.LocationTypeValue ).OrderBy( v => v.Order ).ToList();
                }
            }

            var campusi = CampusCache.All();
            cpCampus.Campuses = campusi;
            cpCampus.Visible = campusi.Any();

            rblMaritalStatus.BindToDefinedType( DefinedTypeCache.Read( new Guid( Rock.SystemGuid.DefinedType.PERSON_MARITAL_STATUS ) ) );
            ddlRecordStatus.BindToDefinedType( DefinedTypeCache.Read( new Guid( Rock.SystemGuid.DefinedType.PERSON_RECORD_STATUS ) ), true );
            ddlReason.BindToDefinedType( DefinedTypeCache.Read( new Guid( Rock.SystemGuid.DefinedType.PERSON_RECORD_STATUS_REASON ) ), true );

            lvMembers.DataKeyNames = new string[] { "Index" };
            lvMembers.ItemDataBound += lvMembers_ItemDataBound;
            lvMembers.ItemCommand += lvMembers_ItemCommand;

            modalAddPerson.SaveButtonText = "Save";
            modalAddPerson.SaveClick += modalAddPerson_SaveClick;
            modalAddPerson.OnCancelScript = string.Format( "$('#{0}').val('');", hfActiveTab.ClientID );

            btnSave.Visible = _canEdit;

            // Save and Cancel should not confirm exit
            btnSave.OnClientClick = string.Format( "javascript:$('#{0}').val('');return true;", confirmExit.ClientID );
            btnCancel.OnClientClick = string.Format( "javascript:$('#{0}').val('');return true;", confirmExit.ClientID );

            // Populate dropdowns
            List<string> languages = new List<string> { "English", "Español" };
            ddlLanguage.DataSource = languages;
            ddlLanguage.DataBind();
            ddlTitle.BindToDefinedType( DefinedTypeCache.Read( new Guid( Rock.SystemGuid.DefinedType.PERSON_TITLE ) ), true );
            ddlSuffix.BindToDefinedType( DefinedTypeCache.Read( new Guid( Rock.SystemGuid.DefinedType.PERSON_SUFFIX ) ), true );
            ddlSection.BindToDefinedType( DefinedTypeCache.Read( DefinedTypeHelper.SECTION_COMMUNITY_GUID ), true );
            ScriptManager.RegisterStartupScript( ddlGradePicker, ddlGradePicker.GetType(), "grade-selection-" + BlockId.ToString(), ddlGradePicker.GetJavascriptForYearPicker( ypGraduation ), true );

            // Gotta love script...
            string smsScript = @"
                                $('.js-sms-number').click(function () {
                                    if ($(this).is(':checked')) {
                                        $('.js-sms-number').not($(this)).prop('checked', false);
                                    }
                                });
                            ";
            ScriptManager.RegisterStartupScript( rContactInfo, rContactInfo.GetType(), "sms-number-" + BlockId.ToString(), smsScript, true );

            this.Language = "English";
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack && CurrentPerson != null )
            {
                ShowDetails();

                BindSocialMediaAttributes();

                string personKey = PageParameter( "rckipid" );

                if ( _family != null )
                {
                    tbFamilyName.Text = _family.Name;

                    // add banner text
                    if ( _family.Name.ToLower().EndsWith( " family" ) )
                    {
                        lBanner.Text = _family.Name.FormatAsHtmlTitle();
                    }
                    else
                    {
                        lBanner.Text = ( _family.Name + " Family" ).FormatAsHtmlTitle();
                    }


                    cpCampus.SelectedCampusId = _family.CampusId;

                    // If all family members have the same record status, display that value
                    if ( _family.Members.Select( m => m.Person.RecordStatusValueId ).Distinct().Count() == 1 )
                    {
                        ddlRecordStatus.SetValue( _family.Members.Select( m => m.Person.RecordStatusValueId ).FirstOrDefault() );
                    }

                    // If all family members have the same inactive reason, set that value
                    if ( _family.Members.Select( m => m.Person.RecordStatusReasonValueId ).Distinct().Count() == 1 )
                    {
                        ddlReason.SetValue( _family.Members.Select( m => m.Person.RecordStatusReasonValueId ).FirstOrDefault() );
                    }

                    FamilyMembers = new List<FamilyMember>();
                    foreach ( var familyMember in _family.Members )
                    {
                        FamilyMembers.Add( new FamilyMember( familyMember, true ) );
                    }
                    BindMembers();

                    foreach ( var groupLocation in _family.GroupLocations )
                    {
                        int homeLocationTypeId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME.AsGuid() ).Id;
                        var firstHomeAddress = _family.GroupLocations.Where( l => l.GroupLocationTypeValueId == homeLocationTypeId ).FirstOrDefault();

                        if ( firstHomeAddress != null )
                        {
                            acHomeAddress.SetValues( firstHomeAddress.Location );
                        }
                    }
                }
                ddlLanguage.SelectedIndex = 0;
                translateToEnglish();
            }
            else
            {
                if ( !string.IsNullOrWhiteSpace( hfActiveTab.Value ) )
                {
                    modalAddPerson.Show();
                }

            }
        }

        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );
            ConstructSocialMediaControls( false );
        }

        #region Events

        /// <summary>
        /// Handles the TextChanged event of the tbFamilyName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void tbFamilyName_TextChanged( object sender, EventArgs e )
        {
            confirmExit.Enabled = true;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the cpCampus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void cpCampus_SelectedIndexChanged( object sender, EventArgs e )
        {
            confirmExit.Enabled = true;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlRecordStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ddlRecordStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            ddlReason.Visible = ( ddlRecordStatus.SelectedValueAsInt() == DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_INACTIVE ) ).Id );
            confirmExit.Enabled = true;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlReason control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ddlReason_SelectedIndexChanged( object sender, EventArgs e )
        {
            confirmExit.Enabled = true;
        }

        #region Social Media List Events
        /// <summary>
        /// Handles the Click event of the lbEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbEdit_Click( object sender, EventArgs e )
        {
            ViewMode = VIEW_MODE_EDIT;
            ConstructSocialMediaControls( true );
        }

        #endregion

        #region Family Member List Events

        /// <summary>
        /// Handles the ItemDataBound event of the lvMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ListViewItemEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void lvMembers_ItemDataBound( object sender, ListViewItemEventArgs e )
        {
            if ( e.Item.ItemType == ListViewItemType.DataItem )
            {
                var familyMember = e.Item.DataItem as FamilyMember;
                if ( familyMember != null )
                {

                    // very similar code in EditFamily.ascx.cs
                    HtmlControl divPersonImage = e.Item.FindControl( "divPersonImage" ) as HtmlControl;
                    if ( divPersonImage != null )
                    {
                        divPersonImage.Style.Add( "background-image", @String.Format( @"url({0})", Person.GetPersonPhotoUrl( familyMember.Id, familyMember.PhotoId, familyMember.Age, familyMember.Gender, null, null, null ) + "&width=65" ) );
                        divPersonImage.Style.Add( "background-size", "cover" );
                        divPersonImage.Style.Add( "background-position", "50%" );
                    }

                    int members = FamilyMembers.Count();
                }
            }
        }

        /// <summary>
        /// Handles the ItemCommand event of the lvMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ListViewCommandEventArgs"/> instance containing the event data.</param>
        void lvMembers_ItemCommand( object sender, ListViewCommandEventArgs e )
        {
            int index = ( int ) lvMembers.DataKeys[e.Item.DataItemIndex]["Index"];
            var familyMember = FamilyMembers.Where( m => m.Index == index ).FirstOrDefault();
            if ( familyMember != null )
            {
                if ( e.CommandName == "EditPerson" )
                {
                    editPerson( e.CommandArgument.ToString().AsInteger() );
                }

                confirmExit.Enabled = true;

                BindMembers();
            }
        }

        protected void editPerson( int familyMemberId )
        {
            FamilyMemberToEdit = familyMemberId;
            modalAddPerson.Show();
            modalAddPerson.Title = "Edit Person";
            tbNewPersonFirstName.Required = true;
            tbNewPersonLastName.Required = true;
            hfActiveTab.Value = "Edit";

            var personToEdit = FamilyMembers.Where( m => m.Id == familyMemberId ).Single();

            tbNewPersonFirstName.Text = personToEdit.FirstName;
            tbNewPersonLastName.Text = personToEdit.LastName;
            tbNewPersonNickName.Text = personToEdit.NickName;
            switch ( personToEdit.Gender.ConvertToString() )
            {

                case "Male":
                    chkNewPersonMale.Checked = true;
                    cblNewPersonGender.SelectedValue = personToEdit.Gender.ConvertToString();
                    chkNewPersonFemale.Checked = false;
                    break;
                case "Female":
                    chkNewPersonFemale.Checked = true;
                    cblNewPersonGender.SelectedValue = personToEdit.Gender.ConvertToString();
                    chkNewPersonMale.Checked = false;
                    break;
                default:
                    chkNewPersonMale.Checked = true;
                    cblNewPersonGender.SelectedValue = "Male";
                    chkNewPersonFemale.Checked = false;
                    break;
            }
            if ( personToEdit.BirthDate.HasValue )
            {
                dpNewPersonBirthDate.SelectedDate = personToEdit.BirthDate.Value;
            }
            else
            {
                dpNewPersonBirthDate.SelectedDate = null;
            }

            if ( personToEdit.RoleName == "Adult" )
            {
                rblNewPersonRole.SelectedValue = "3";
            }
            else if ( personToEdit.RoleName == "Child" )
            {
                rblNewPersonRole.SelectedValue = "4";
            }
            foreach ( ListItem item in rblNewPersonRole.Items )
            {
                switch ( item.Value )
                {
                    case "3":
                        if(this.Language == "English" )
                        {
                            item.Text = "Adult";
                        }else if(this.Language == "Spanish" )
                        {
                            item.Text = "Adulto";
                        }                       
                        break;
                    case "4":
                        if(this.Language == "English" )
                        {
                            item.Text = "Child";
                        }else if( this.Language == "Spanish" )
                        {
                            item.Text = "Menor";
                        }                        
                        break;
                }
            }
            setAdditionalRoleFields();

            tbNewPersonEmail.Text = personToEdit.EmailAddress;
            pnbNewPersonMobile.Text = personToEdit.Mobile;
            pnbNewPersonWork.Text = personToEdit.Work;

            tbChildHealthNote.Text = personToEdit.Allergy;

            if ( personToEdit.GraduationYear.HasValue )
            {
                ypGraduation.SelectedYear = personToEdit.GraduationYear.Value;
            }
            else
            {
                ypGraduation.SelectedYear = null;
            }

            if ( !personToEdit.HasGraduated ?? false )
            {
                int gradeOffset = personToEdit.GradeOffset.Value;
                var maxGradeOffset = ddlGradePicker.MaxGradeOffset;

                // keep trying until we find a Grade that has a gradeOffset that that includes the Person's gradeOffset (for example, there might be combined grades)
                while ( !ddlGradePicker.Items.OfType<ListItem>().Any( a => a.Value.AsInteger() == gradeOffset ) && gradeOffset <= maxGradeOffset )
                {
                    gradeOffset++;
                }

                ddlGradePicker.SetValue( gradeOffset );
            }
            else
            {
                ddlGradePicker.SelectedIndex = 0;
            }

        }

        private void setAdditionalRoleFields()
        {
            if ( rblNewPersonRole.SelectedValue == "3" )
            {
                adultDetail.Visible = true;
                childDetail.Visible = false;
            }
            else if ( rblNewPersonRole.SelectedValue == "4" )
            {
                adultDetail.Visible = false;
                childDetail.Visible = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the lbAddPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbAddPerson_Click( object sender, EventArgs e )
        {
            tbNewPersonFirstName.Required = true;
            tbNewPersonLastName.Required = true;
            //ddlNewPersonConnectionStatus.Required = true;
            modalAddPerson.Title = "Add Person";
            hfActiveTab.Value = "New";

            tbNewPersonFirstName.Text = string.Empty;

            // default the last name of the new family member to the lastname of the existing adults in the family (if all the adults have the same last name)
            var lastNames = FamilyMembers.Where( a => a.RoleGuid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Select( a => a.LastName ).Distinct().ToList();
            if ( lastNames.Count == 1 )
            {
                tbNewPersonLastName.Text = lastNames[0];
            }
            else
            {
                tbNewPersonLastName.Text = string.Empty;
            }

            tbNewPersonNickName.Text = string.Empty;
            chkNewPersonMale.Checked = true;
            cblNewPersonGender.SelectedValue = "Male";
            chkNewPersonFemale.Checked = false;
            dpNewPersonBirthDate.SelectedDate = null;
            rblNewPersonRole.SelectedIndex = 0;
            tbNewPersonEmail.Text = string.Empty;
            pnbNewPersonMobile.Text = string.Empty;
            pnbNewPersonWork.Text = string.Empty;
            ddlGradePicker.SelectedIndex = 0;
            tbChildHealthNote.Text = string.Empty;
            setAdditionalRoleFields();

            modalAddPerson.Show();
        }

        /// <summary>
        /// Handles the SaveClick event of the modalAddPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void modalAddPerson_SaveClick( object sender, EventArgs e )
        {
            if ( hfActiveTab.Value == "New" )
            {
                var familyMember = new FamilyMember();
                getFormValues( familyMember );
                familyMember.ConnectionStatusValueId = GetAttributeValue( "ConnectionStatus" ).AsInteger();
                FamilyMembers.Add( familyMember );
            }
            else if ( hfActiveTab.Value == "Edit" )
            {
                if ( FamilyMemberToEdit.HasValue )
                {
                    var familyMember = FamilyMembers.Where( m => m.Id == FamilyMemberToEdit.Value ).SingleOrDefault();

                    if ( familyMember != null )
                    {
                        getFormValues( familyMember );
                    }
                }
            }

            FamilyMemberToEdit = 0;
            tbNewPersonFirstName.Required = false;
            tbNewPersonLastName.Required = false;

            confirmExit.Enabled = true;
            modalAddPerson.Hide();

            BindMembers();
            hfActiveTab.Value = string.Empty;
        }

        private void getFormValues( FamilyMember familyMember )
        {
            familyMember.FirstName = tbNewPersonFirstName.Text;
            familyMember.NickName = tbNewPersonNickName.Text;
            familyMember.LastName = tbNewPersonLastName.Text;
            familyMember.Gender = cblNewPersonGender.SelectedValue.ConvertToEnum<Gender>();
            familyMember.BirthDate = dpNewPersonBirthDate.SelectedDate;
            familyMember.EmailAddress = tbNewPersonEmail.Text;

            var role = familyRoles.Where( r => r.Id == ( rblNewPersonRole.SelectedValueAsInt() ?? 0 ) ).FirstOrDefault();
            if ( role != null )
            {
                familyMember.RoleGuid = role.Guid;
                familyMember.RoleName = role.Name;
            }

            var mobilePhoneType = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE ) );
            if ( !string.IsNullOrWhiteSpace( PhoneNumber.CleanNumber( pnbNewPersonMobile.Number ) ) )
            {
                familyMember.Mobile = pnbNewPersonMobile.Number;
            }
            else
            {
                familyMember.Mobile = string.Empty;
            }

            // Get the adult / child information and save it...
            if ( rblNewPersonRole.SelectedIndex == 0 )
            {
                var workPhoneType = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK ) );
                if ( !string.IsNullOrWhiteSpace( PhoneNumber.CleanNumber( pnbNewPersonWork.Number ) ) )
                {
                    familyMember.Work = pnbNewPersonWork.Number;
                }
                else
                {
                    familyMember.Work = string.Empty;
                }
            }
            else
            {
                // Child...
                familyMember.Allergy = tbChildHealthNote.Text;
                familyMember.GradeOffset = ddlGradePicker.SelectedValueAsInt();
            }
        }

        #endregion

        #region Action Events

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            // confirmation was disabled by btnSave on client-side.  So if returning without a redirect,
            // it should be enabled.  If returning with a redirect, the control won't be updated to reflect
            // confirmation being enabled, so it's ok to enable it here

            if ( Page.IsValid )
            {
                confirmExit.Enabled = true;

                var rockContext = new RockContext();
                rockContext.WrapTransaction( () =>
                 {
                     SaveFamilyChanges( rockContext );
                     SaveProfileChanges( rockContext );
                     SaveWorkflowChanges( rockContext );
                 } );

                SaveSocialMediaChanges();
            }
        }

        protected void SaveWorkflowChanges( RockContext rockContext )
        {
            string workflowId = PageParameter( "WorkflowId" );
            if ( !String.IsNullOrEmpty( workflowId ) )
            {
                var attributeService = new AttributeService( rockContext );
                var attributeValueService = new AttributeValueService( rockContext );
                var workflowService = new WorkflowService( rockContext );

                Guid workflowGuid = Guid.Parse( workflowId );

                var workflowInstance = workflowService.Get( workflowGuid );

                // Now process the workflow...
                List<string> errorMessages = new List<string>();
                workflowInstance.MarkComplete();
                workflowInstance.Status = "Completed";

                rockContext.SaveChanges();
            }
        }

        protected void SaveProfileChanges( RockContext rockContext )
        {
            var familyService = new GroupService( rockContext );
            var familyMemberService = new GroupMemberService( rockContext );
            var personService = new PersonService( rockContext );
            var historyService = new HistoryService( rockContext );
            var attributeService = new AttributeService( rockContext );
            var attributeValueService = new AttributeValueService( rockContext );
            var fieldTypeService = new FieldTypeService( rockContext );

            var demographicChanges = new List<string>();
            var person = personService.Get( CurrentPersonId ?? 0 );
            if ( person != null )
            {
                int? orphanedPhotoId = null;
                if ( person.PhotoId != imgPhoto.BinaryFileId )
                {
                    orphanedPhotoId = person.PhotoId;
                    person.PhotoId = imgPhoto.BinaryFileId;

                    if ( orphanedPhotoId.HasValue )
                    {
                        if ( person.PhotoId.HasValue )
                        {
                            demographicChanges.Add( "Modified the photo." );
                        }
                        else
                        {
                            demographicChanges.Add( "Deleted the photo." );
                        }
                    }
                    else if ( person.PhotoId.HasValue )
                    {
                        demographicChanges.Add( "Added a photo." );
                    }
                }

                int? newTitleId = ddlTitle.SelectedValueAsInt();
                History.EvaluateChange( demographicChanges, "Title", DefinedValueCache.GetName( person.TitleValueId ), DefinedValueCache.GetName( newTitleId ) );
                person.TitleValueId = newTitleId;

                History.EvaluateChange( demographicChanges, "First Name", person.FirstName, tbFirstName.Text );
                person.FirstName = tbFirstName.Text;

                History.EvaluateChange( demographicChanges, "Nick Name", person.NickName, tbNickName.Text );
                person.NickName = tbNickName.Text;

                History.EvaluateChange( demographicChanges, "Last Name", person.LastName, tbLastName.Text );
                person.LastName = tbLastName.Text;

                int? newSuffixId = ddlSuffix.SelectedValueAsInt();
                History.EvaluateChange( demographicChanges, "Suffix", DefinedValueCache.GetName( person.SuffixValueId ), DefinedValueCache.GetName( newSuffixId ) );
                person.SuffixValueId = newSuffixId;

                var birthMonth = person.BirthMonth;
                var birthDay = person.BirthDay;
                var birthYear = person.BirthYear;

                var birthday = bpBirthDay.SelectedDate;
                if ( birthday.HasValue )
                {
                    person.BirthMonth = birthday.Value.Month;
                    person.BirthDay = birthday.Value.Day;
                    if ( birthday.Value.Year != DateTime.MinValue.Year )
                    {
                        person.BirthYear = birthday.Value.Year;
                    }
                    else
                    {
                        person.BirthYear = null;
                    }
                }
                else
                {
                    person.BirthDay = null;
                    person.BirthMonth = null;
                    person.BirthYear = null;
                }

                History.EvaluateChange( demographicChanges, "Birth Month", birthMonth, person.BirthMonth );
                History.EvaluateChange( demographicChanges, "Birth Day", birthDay, person.BirthDay );
                History.EvaluateChange( demographicChanges, "Birth Year", birthYear, person.BirthYear );

                var newGender = cblGender.SelectedValue.ConvertToEnum<Gender>();
                History.EvaluateChange( demographicChanges, "Gender", person.Gender, newGender );
                person.Gender = newGender;

                int? newMaritalStatusId = rblMaritalStatus.SelectedValueAsInt();
                History.EvaluateChange( demographicChanges, "Marital Status", DefinedValueCache.GetName( person.MaritalStatusValueId ), DefinedValueCache.GetName( newMaritalStatusId ) );
                person.MaritalStatusValueId = newMaritalStatusId;

                var phoneNumberTypeIds = new List<int>();

                bool smsSelected = false;

                foreach ( RepeaterItem item in rContactInfo.Items )
                {
                    HiddenField hfPhoneType = item.FindControl( "hfPhoneType" ) as HiddenField;
                    PhoneNumberBox pnbPhone = item.FindControl( "pnbPhone" ) as PhoneNumberBox;
                    CheckBox cbUnlisted = item.FindControl( "cbUnlisted" ) as CheckBox;
                    CheckBox cbSms = item.FindControl( "cbSms" ) as CheckBox;

                    if ( hfPhoneType != null &&
                        pnbPhone != null &&
                        cbSms != null &&
                        cbUnlisted != null )
                    {
                        if ( !string.IsNullOrWhiteSpace( PhoneNumber.CleanNumber( pnbPhone.Number ) ) )
                        {
                            int phoneNumberTypeId;
                            if ( int.TryParse( hfPhoneType.Value, out phoneNumberTypeId ) )
                            {
                                var phoneNumber = person.PhoneNumbers.FirstOrDefault( n => n.NumberTypeValueId == phoneNumberTypeId );
                                string oldPhoneNumber = string.Empty;
                                if ( phoneNumber == null )
                                {
                                    phoneNumber = new PhoneNumber { NumberTypeValueId = phoneNumberTypeId };
                                    person.PhoneNumbers.Add( phoneNumber );
                                }
                                else
                                {
                                    oldPhoneNumber = phoneNumber.NumberFormatted;
                                }

                                phoneNumber.CountryCode = PhoneNumber.CleanNumber( pnbPhone.CountryCode );
                                phoneNumber.Number = PhoneNumber.CleanNumber( pnbPhone.Number );

                                // Only allow one number to have SMS selected
                                if ( smsSelected )
                                {
                                    phoneNumber.IsMessagingEnabled = false;
                                }
                                else
                                {
                                    phoneNumber.IsMessagingEnabled = cbSms.Checked;
                                    smsSelected = cbSms.Checked;
                                }

                                phoneNumber.IsUnlisted = cbUnlisted.Checked;
                                phoneNumberTypeIds.Add( phoneNumberTypeId );

                                History.EvaluateChange( demographicChanges, string.Format( "{0} Phone", DefinedValueCache.GetName( phoneNumberTypeId ) ), oldPhoneNumber, PhoneNumber.FormattedNumber( phoneNumber.CountryCode, phoneNumber.Number ) );
                            }
                        }
                    }
                }

                // Remove any blank numbers
                var phoneNumberService = new PhoneNumberService( rockContext );
                foreach ( var phoneNumber in person.PhoneNumbers
                    .Where( n => n.NumberTypeValueId.HasValue && !phoneNumberTypeIds.Contains( n.NumberTypeValueId.Value ) )
                    .ToList() )
                {
                    History.EvaluateChange( demographicChanges,
                        string.Format( "{0} Phone", DefinedValueCache.GetName( phoneNumber.NumberTypeValueId ) ),
                        phoneNumber.ToString(), string.Empty );

                    person.PhoneNumbers.Remove( phoneNumber );
                    phoneNumberService.Delete( phoneNumber );
                }

                History.EvaluateChange( demographicChanges, "Email", person.Email, tbEmail.Text );
                person.Email = tbEmail.Text.Trim();


                // Save the custom section attribute...

                var personTypeId = EntityTypeCache.Read( Rock.SystemGuid.EntityType.PERSON.AsGuid() ).Id;
                var fieldType = fieldTypeService.GetByName( "Defined Value" );
                var attributeObject = attributeService.GetByEntityTypeId( personTypeId ).Single( a => a.Key == "Section" );
                var attributeValueObject = attributeValueService.GetByAttributeIdAndEntityId( attributeObject.Id, CurrentPerson.Id );
                if ( attributeValueObject != null )
                {
                    if ( !String.IsNullOrEmpty( attributeValueObject.Value ) )
                    {
                        var attribValGuid = attributeValueObject.Value.AsGuid();
                        var prevSection = new DefinedValueService( rockContext ).Queryable().FirstOrDefault( d => d.Guid == attribValGuid ).Value.ToString();
                        History.EvaluateChange( demographicChanges, "Section", prevSection, ddlSection.SelectedItem.Text );
                    }
                    else
                    {
                        History.EvaluateChange( demographicChanges, "Section", null, ddlSection.SelectedItem.Text );
                    }

                    var sectionTypes = DefinedTypeCache.Read( new Guid( DefinedTypeHelper.SECTION_COMMUNITY_GUID.ToString( "D" ) ) );
                    if ( !String.IsNullOrEmpty( ddlSection.SelectedItem.Text ) )
                    {
                        var selectedSection = sectionTypes.DefinedValues.Single( v => v.Value == ddlSection.SelectedItem.Text );
                        attributeValueObject.Value = selectedSection.Guid.ToString( "D" );
                    }
                    else
                    {
                        attributeValueObject.Value = "";
                    }
                }
                else if ( attributeValueObject == null )
                {
                    // This could be the first time this is ever set...
                    attributeValueService.Add( new AttributeValue()
                    {
                        AttributeId = attributeObject.Id,
                        IsSystem = false,
                        EntityId = CurrentPerson.Id,
                        Value = ddlSection.SelectedValue
                    } );
                }

                // Save the service time attribute
                fieldType = fieldTypeService.GetByName( "Time" );
                attributeObject = attributeService.GetByEntityTypeId( personTypeId ).Single( a => a.Key == "ServiceTime" );
                attributeValueObject = attributeValueService.GetByAttributeIdAndEntityId( attributeObject.Id, CurrentPerson.Id );
                if ( attributeValueObject != null && ( tbPrimaryService.Text != attributeValueObject.Value ) )
                {
                    History.EvaluateChange( demographicChanges, "ServiceTime", attributeValueObject.Value, tbPrimaryService.SelectedTime.Value.ToString() );
                    attributeValueObject.Value = tbPrimaryService.SelectedTime.ToString();
                }
                else if ( attributeValueObject == null )
                {
                    // This could be the first time this is ever set...
                    attributeValueService.Add( new AttributeValue()
                    {
                        AttributeId = attributeObject.Id,
                        IsSystem = false,
                        EntityId = CurrentPerson.Id,
                        Value = tbPrimaryService.Text
                    } );
                }



                if ( person.IsValid )
                {
                    if ( rockContext.SaveChanges() > 0 )
                    {
                        if ( demographicChanges.Any() )
                        {
                            HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(),
                                person.Id, demographicChanges );
                        }

                        if ( orphanedPhotoId.HasValue )
                        {
                            BinaryFileService binaryFileService = new BinaryFileService( rockContext );
                            var binaryFile = binaryFileService.Get( orphanedPhotoId.Value );
                            if ( binaryFile != null )
                            {
                                // marked the old images as IsTemporary so they will get cleaned up later
                                binaryFile.IsTemporary = true;
                                rockContext.SaveChanges();
                            }
                        }
                    }

                    NavigateToParentPage();

                }
            }
        }

        protected void SaveFamilyChanges( RockContext rockContext )
        {
            var familyService = new GroupService( rockContext );
            var familyMemberService = new GroupMemberService( rockContext );
            var personService = new PersonService( rockContext );
            var historyService = new HistoryService( rockContext );
            var attributeService = new AttributeService( rockContext );
            var attributeValueService = new AttributeValueService( rockContext );
            var fieldTypeService = new FieldTypeService( rockContext );
            var definedValueService = new DefinedValueService( rockContext );

            #region Save Family
            var familyChanges = new List<string>();

            // SAVE FAMILY
            _family = familyService.Get( _family.Id );

            History.EvaluateChange( familyChanges, "Family Name", _family.Name, tbFamilyName.Text );
            _family.Name = tbFamilyName.Text;

            int? campusId = cpCampus.SelectedValueAsInt();
            if ( _family.CampusId != campusId )
            {
                History.EvaluateChange( familyChanges, "Campus",
                    _family.CampusId.HasValue ? CampusCache.Read( _family.CampusId.Value ).Name : string.Empty,
                    campusId.HasValue ? CampusCache.Read( campusId.Value ).Name : string.Empty );
                _family.CampusId = campusId;
            }

            var familyGroupTypeId = _family.GroupTypeId;

            rockContext.SaveChanges();

            // SAVE FAMILY MEMBERS
            int? recordStatusValueID = ddlRecordStatus.SelectedValueAsInt();
            int? reasonValueId = ddlReason.SelectedValueAsInt();
            var newFamilies = new List<Group>();

            foreach ( var familyMember in FamilyMembers )
            {
                var demographicChanges = new List<string>();

                var role = familyRoles.Where( r => r.Guid.Equals( familyMember.RoleGuid ) ).FirstOrDefault();
                if ( role == null )
                {
                    role = familyRoles.FirstOrDefault();
                }

                bool isChild = role != null && role.Guid.Equals( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD ) );

                // People added to family (new or from other family)
                if ( !familyMember.ExistingFamilyMember )
                {
                    var groupMember = new GroupMember();

                    if ( familyMember.Id == -1 )
                    {
                        // added new person
                        demographicChanges.Add( "Created" );

                        var person = new Person();
                        person.FirstName = familyMember.FirstName;
                        person.NickName = familyMember.NickName;
                        History.EvaluateChange( demographicChanges, "First Name", string.Empty, person.FirstName );

                        person.LastName = familyMember.LastName;
                        History.EvaluateChange( demographicChanges, "Last Name", string.Empty, person.LastName );

                        person.Gender = familyMember.Gender;
                        History.EvaluateChange( demographicChanges, "Gender", null, person.Gender );

                        //***
                        //in Rock v3.1 change to setBirthDate
                        //this is here so 2.0 works in 3.1
                        //person.BirthDate = familyMember.BirthDate;
                        if ( familyMember.BirthDate.HasValue )
                        {
                            person.BirthMonth = familyMember.BirthDate.Value.Month;
                            person.BirthDay = familyMember.BirthDate.Value.Day;
                            if ( familyMember.BirthDate.Value.Year != DateTime.MinValue.Year )
                            {
                                person.BirthYear = familyMember.BirthDate.Value.Year;
                            }
                            else
                            {
                                person.BirthYear = null;
                            }
                        }
                        else
                        {
                            person.BirthMonth = null;
                            person.BirthDay = null;
                            person.BirthYear = null;
                        }
                        //***


                        History.EvaluateChange( demographicChanges, "Birth Date", null, person.BirthDate );

                        person.ConnectionStatusValueId = familyMember.ConnectionStatusValueId;
                        History.EvaluateChange( demographicChanges, "Connection Status", string.Empty, person.ConnectionStatusValueId.HasValue ? DefinedValueCache.GetName( person.ConnectionStatusValueId ) : string.Empty );

                        person.IsEmailActive = false;
                        History.EvaluateChange( demographicChanges, "Email Active", false.ToString(), ( person.IsEmailActive ).ToString() );

                        person.RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
                        History.EvaluateChange( demographicChanges, "Record Type", string.Empty, person.RecordTypeValueId.HasValue ? DefinedValueCache.GetName( person.RecordTypeValueId.Value ) : string.Empty );

                        //person.MaritalStatusValueId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.PERSON_MARITAL_STATUS_UNKNOWN.AsGuid()).Id;
                        //History.EvaluateChange(memberChanges, "Marital Status", string.Empty, person.MaritalStatusValueId.HasValue ? DefinedValueCache.GetName(person.MaritalStatusValueId) : string.Empty);

                        if ( !String.IsNullOrEmpty( familyMember.EmailAddress ) )
                        {
                            person.Email = familyMember.EmailAddress;
                            History.EvaluateChange( demographicChanges, "Last Name", string.Empty, person.LastName );
                        }

                        if ( !String.IsNullOrEmpty( familyMember.Mobile ) )
                        {
                            var mobilePhoneType = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE ) );
                            person.PhoneNumbers.Add( new PhoneNumber { Number = familyMember.Mobile, IsMessagingEnabled = true, NumberTypeValueId = mobilePhoneType.Id } );
                            History.EvaluateChange( demographicChanges, string.Format( "{0} Phone", mobilePhoneType.TypeName ), String.Empty, familyMember.Mobile );
                        }

                        if ( !isChild )
                        {
                            person.GivingGroupId = _family.Id;
                            History.EvaluateChange( demographicChanges, "Giving Group", string.Empty, _family.Name );

                            if ( !String.IsNullOrEmpty( familyMember.Work ) )
                            {
                                var workPhoneType =
                                    DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK ) );
                                person.PhoneNumbers.Add( new PhoneNumber
                                {
                                    Number = familyMember.Work,
                                    IsMessagingEnabled = false,
                                    NumberTypeValueId = workPhoneType.Id
                                } );
                                History.EvaluateChange( demographicChanges,
                                    string.Format( "{0} Phone", workPhoneType.TypeName ), String.Empty, familyMember.Work );
                            }
                        }
                        else
                        {
                            int? graduationYear = null;
                            if ( ypGraduation.SelectedYear.HasValue )
                            {
                                graduationYear = ypGraduation.SelectedYear.Value;
                            }
                            History.EvaluateChange( demographicChanges, "Graduation Year", person.GraduationYear, graduationYear );
                            person.GraduationYear = graduationYear;
                        }

                        person.EmailPreference = EmailPreference.EmailAllowed;

                        groupMember.Person = person;
                    }
                    else
                    {
                        // added from other family
                        groupMember.Person = personService.Get( familyMember.Id );
                    }

                    if ( recordStatusValueID > 0 )
                    {
                        History.EvaluateChange( demographicChanges, "Record Status", DefinedValueCache.GetName( groupMember.Person.RecordStatusValueId ), DefinedValueCache.GetName( recordStatusValueID ) );
                        groupMember.Person.RecordStatusValueId = recordStatusValueID;

                        History.EvaluateChange( demographicChanges, "Record Status Reason", DefinedValueCache.GetName( groupMember.Person.RecordStatusReasonValueId ), DefinedValueCache.GetName( reasonValueId ) );
                        groupMember.Person.RecordStatusReasonValueId = reasonValueId;
                    }

                    groupMember.GroupId = _family.Id;
                    if ( role != null )
                    {
                        History.EvaluateChange( demographicChanges, "Role", string.Empty, role.Name );
                        groupMember.GroupRoleId = role.Id;

                        // AJZ - Added to prevent null reference exception later when adding a new child...
                        //groupMember.GroupRole = role;
                    }

                    if ( groupMember.Person != null )
                    {
                        familyMemberService.Add( groupMember );
                        rockContext.SaveChanges();

                        // Every person should have an alias record with same id.  If it's missing, create it
                        if ( !groupMember.Person.Aliases.Any( a => a.AliasPersonId == groupMember.Person.Id ) )
                        {
                            var groupMemberPerson = personService.Get( groupMember.Person.Id );
                            if ( groupMemberPerson != null )
                            {
                                groupMemberPerson.Aliases.Add( new PersonAlias { AliasPersonId = groupMemberPerson.Id, AliasPersonGuid = groupMemberPerson.Guid } );
                                rockContext.SaveChanges();
                            }
                        }

                        familyMember.Id = groupMember.Person.Id;

                        // POST SAVE SAVES -- these records can't be saved until there is a valid ID
                        if ( isChild )
                        {
                            // Save the custom section attribute Child Grade...
                            int? graduationYear = null;
                            if ( ypGraduation.SelectedYear.HasValue )
                            {
                                graduationYear = ypGraduation.SelectedYear.Value;
                            }

                            History.EvaluateChange( demographicChanges, "Graduation Year", groupMember.Person.GraduationYear, graduationYear );
                            groupMember.Person.GraduationYear = graduationYear;

                            if ( !String.IsNullOrEmpty( familyMember.Allergy ) )
                            {
                                Guid allergyGuid = AttributeGuids.ALLERGY.AsGuid();
                                var allergyAttributeId = new AttributeService( rockContext ).Queryable().Where( k => k.Guid == allergyGuid ).Select( k => k.Id ).FirstOrDefault().ToString().AsInteger();
                                attributeValueService.Add( new AttributeValue()
                                {
                                    AttributeId = allergyAttributeId,
                                    IsSystem = false,
                                    EntityId = groupMember.Person.Id,
                                    Value = familyMember.Allergy
                                } );
                            }
                        }
                    }

                }
                else
                {
                    // existing family members
                    var groupMember = familyMemberService.Queryable( "Person" ).Where( m =>
                           m.PersonId == familyMember.Id &&
                           m.Group.GroupTypeId == familyGroupTypeId &&
                           m.GroupId == _family.Id ).FirstOrDefault();

                    if ( groupMember != null )
                    {
                        var changes = new List<string>();
                        //Update person attributes
                        var person = personService.Get( familyMember.Id );
                        if ( person != null )
                        {
                            History.EvaluateChange( demographicChanges, "First Name", person.FirstName, familyMember.FirstName );
                            person.FirstName = familyMember.FirstName;

                            History.EvaluateChange( demographicChanges, "Nick Name", person.NickName, familyMember.NickName );
                            person.NickName = familyMember.NickName;

                            History.EvaluateChange( demographicChanges, "Last Name", person.LastName, familyMember.LastName );
                            person.LastName = familyMember.LastName;

                            var birthMonth = person.BirthMonth;
                            var birthDay = person.BirthDay;
                            var birthYear = person.BirthYear;

                            var birthday = familyMember.BirthDate;
                            if ( birthday.HasValue )
                            {
                                person.BirthMonth = birthday.Value.Month;
                                person.BirthDay = birthday.Value.Day;
                                if ( birthday.Value.Year != DateTime.MinValue.Year )
                                {
                                    person.BirthYear = birthday.Value.Year;
                                }
                                else
                                {
                                    person.BirthYear = null;
                                }
                            }
                            else
                            {
                                person.BirthDay = null;
                                person.BirthMonth = null;
                                person.BirthYear = null;
                            }

                            History.EvaluateChange( demographicChanges, "Birth Month", birthMonth, person.BirthMonth );
                            History.EvaluateChange( demographicChanges, "Birth Day", birthDay, person.BirthDay );
                            History.EvaluateChange( demographicChanges, "Birth Year", birthYear, person.BirthYear );
                            History.EvaluateChange( demographicChanges, "Gender", person.Gender, familyMember.Gender );
                            person.Gender = familyMember.Gender;
                            History.EvaluateChange( demographicChanges, "Email", person.Email, familyMember.EmailAddress );
                            person.Email = familyMember.EmailAddress;

                        }

                        //Update Mobile
                        var mobilePhoneType = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE ) );
                        var mobilePhone = person.PhoneNumbers.Where( p => p.NumberTypeValueId == mobilePhoneType.Id ).FirstOrDefault();
                        if ( mobilePhone != null )
                        {
                            History.EvaluateChange( demographicChanges, string.Format( "{0} Phone", DefinedValueCache.GetName( mobilePhoneType.Id ) ), mobilePhone.NumberFormatted, PhoneNumber.FormattedNumber( "", familyMember.Mobile ) );
                            mobilePhone.Number = familyMember.Mobile;
                        }
                        else
                        {
                            if ( !string.IsNullOrEmpty( familyMember.Mobile )  )
                            {
                                person.PhoneNumbers.Add( new PhoneNumber
                                {
                                    Number = familyMember.Mobile,
                                    IsMessagingEnabled = false,
                                    NumberTypeValueId = mobilePhoneType.Id
                                } );

                                History.EvaluateChange( demographicChanges, string.Format( "{0} Phone", DefinedValueCache.GetName( mobilePhoneType.Id ) ), string.Empty, PhoneNumber.FormattedNumber( "", familyMember.Mobile ) );
                            }
                        }

                        //Update based on role, work phone or check-in grade and allergy attributes.

                        if ( !isChild )
                        {
                            var workPhoneType = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_WORK ) );
                            var workPhone = person.PhoneNumbers.Where( p => p.NumberTypeValueId == workPhoneType.Id ).FirstOrDefault();
                            if ( workPhone != null )
                            {
                                History.EvaluateChange( demographicChanges, string.Format( "{0} Phone", DefinedValueCache.GetName( workPhoneType.Id ) ), workPhone.NumberFormatted, PhoneNumber.FormattedNumber( "", familyMember.Work ) );
                                workPhone.Number = familyMember.Work;
                            }
                            else
                            {
                                if ( !string.IsNullOrEmpty( familyMember.Work) )
                                {
                                    person.PhoneNumbers.Add( new PhoneNumber
                                    {
                                        Number = familyMember.Work,
                                        IsMessagingEnabled = false,
                                        NumberTypeValueId = workPhoneType.Id
                                    } );
                                    History.EvaluateChange( demographicChanges, string.Format( "{0} Phone", DefinedValueCache.GetName( workPhoneType.Id ) ), string.Empty, PhoneNumber.FormattedNumber( "", familyMember.Work ) );
                                }
                            }
                        }
                        else
                        {
                            //Update Check-In grade
                            int? graduationYear = null;
                            if ( ypGraduation.SelectedYear.HasValue )
                            {
                                graduationYear = ypGraduation.SelectedYear.Value;
                            }

                            History.EvaluateChange( demographicChanges, "Graduation Year", person.GraduationYear, graduationYear );
                            person.GraduationYear = graduationYear;

                            //Update Allergy
                            Guid allergyGuid = AttributeGuids.ALLERGY.AsGuid();
                            var allergyAttribute = new AttributeService( rockContext ).Queryable().Where( k => k.Guid == allergyGuid ).FirstOrDefault();
                            var allergyAttributeValue = new AttributeValueService( rockContext ).Queryable().Where( a => a.AttributeId == allergyAttribute.Id && a.EntityId == person.Id ).FirstOrDefault();
                            if ( allergyAttributeValue != null )
                            {
                                //Update existing attribute
                                var allergy = attributeValueService.Queryable().Where( a => a.Id == allergyAttributeValue.Id ).First();
                                History.EvaluateChange( demographicChanges, "Allergy", allergy.Value, familyMember.Allergy );
                                allergy.Value = familyMember.Allergy;
                            }
                            else
                            {
                                //Add new attribute
                                if ( !String.IsNullOrEmpty( familyMember.Allergy ) )
                                {
                                    attributeValueService.Add( new AttributeValue()
                                    {
                                        AttributeId = allergyAttribute.Id,
                                        IsSystem = false,
                                        EntityId = groupMember.Person.Id,
                                        Value = familyMember.Allergy
                                    } );
                                    History.EvaluateChange( demographicChanges, "Allergy", string.Empty, familyMember.Allergy );
                                }
                            }
                        }

                        //Update group member role
                        if ( role != null )
                        {
                            History.EvaluateChange( demographicChanges, "Role",
                                groupMember.GroupRole != null ? groupMember.GroupRole.Name : string.Empty, role.Name );
                            groupMember.GroupRoleId = role.Id;

                            if ( recordStatusValueID > 0 )
                            {
                                History.EvaluateChange( demographicChanges, "Record Status", DefinedValueCache.GetName( groupMember.Person.RecordStatusValueId ), DefinedValueCache.GetName( recordStatusValueID ) );
                                groupMember.Person.RecordStatusValueId = recordStatusValueID;

                                History.EvaluateChange( demographicChanges, "Record Status Reason", DefinedValueCache.GetName( groupMember.Person.RecordStatusReasonValueId ), DefinedValueCache.GetName( reasonValueId ) );
                                groupMember.Person.RecordStatusReasonValueId = reasonValueId;
                            }

                            rockContext.SaveChanges();
                        }
                    }
                }

                HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(),
                    familyMember.Id, demographicChanges );

                HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_FAMILY_CHANGES.AsGuid(),
                    familyMember.Id, familyChanges, _family.Name, typeof( Group ), _family.Id );
            }

            // SAVE Address
            var groupLocationService = new GroupLocationService( rockContext );
            var dvHomeAddressType = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME.AsGuid() );
            var familyAddress = groupLocationService.Queryable().Where( l => l.GroupId == _family.Id && l.GroupLocationTypeValueId == dvHomeAddressType.Id ).FirstOrDefault();
            var newFamilyAddress = new Location();
            acHomeAddress.GetValues( newFamilyAddress );
            if ( familyAddress != null && string.IsNullOrWhiteSpace( acHomeAddress.Street1 ) )
            {
                //delete the current address
                History.EvaluateChange( familyChanges, familyAddress.GroupLocationTypeValue.Value + " Location", familyAddress.Location.ToString(), string.Empty );
                groupLocationService.Delete( familyAddress );
                rockContext.SaveChanges();
            }
            else
            {
                if ( !string.IsNullOrWhiteSpace( acHomeAddress.Street1 ) )
                {
                    if ( familyAddress == null )
                    {
                        familyAddress = new GroupLocation();
                        groupLocationService.Add( familyAddress );
                        familyAddress.GroupLocationTypeValueId = dvHomeAddressType.Id;
                        familyAddress.GroupId = _family.Id;
                        familyAddress.IsMailingLocation = true;
                        familyAddress.IsMappedLocation = true;
                    }
                    else if ( familyAddress.Location.FormattedAddress != newFamilyAddress.FormattedAddress )
                    {
                        var previousAddress = new GroupLocation();
                        groupLocationService.Add( previousAddress );

                        var previousAddressValue = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_PREVIOUS.AsGuid() );
                        if ( previousAddressValue != null )
                        {
                            previousAddress.GroupLocationTypeValueId = previousAddressValue.Id;
                            previousAddress.GroupId = _family.Id;

                            Location previousAddressLocation = new Location();
                            previousAddressLocation.Street1 = familyAddress.Location.Street1;
                            previousAddressLocation.Street2 = familyAddress.Location.Street2;
                            previousAddressLocation.City = familyAddress.Location.City;
                            previousAddressLocation.State = familyAddress.Location.State;
                            previousAddressLocation.PostalCode = familyAddress.Location.PostalCode;
                            previousAddressLocation.Country = familyAddress.Location.Country;

                            previousAddress.Location = previousAddressLocation;
                        }
                    }
                    familyAddress.IsMailingLocation = true;
                    familyAddress.IsMappedLocation = true;

                    var updatedHomeAddress = new Location();
                    acHomeAddress.GetValues( updatedHomeAddress );

                    History.EvaluateChange( familyChanges, dvHomeAddressType.Value + " Location", familyAddress.Location != null ? familyAddress.Location.ToString() : string.Empty, updatedHomeAddress.ToString() );

                    familyAddress.Location = updatedHomeAddress;
                    rockContext.SaveChanges();
                }
            }

            foreach ( var fm in _family.Members )
            {
                HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_FAMILY_CHANGES.AsGuid(),
                    fm.PersonId, familyChanges, _family.Name, typeof( Group ), _family.Id );
            }

            _family = familyService.Get( _family.Id );
            if ( _family.Members.Any( m => m.PersonId == Person.Id ) )
            {
                Response.Redirect( string.Format( "~/Person/{0}", Person.Id ), false );
            }
            else
            {
                // AJZ - There used to be complex logic here, causing an exception.  Removing it for now and redirect to the current page...
                Response.Redirect( "~", false );
            }
            #endregion

        }

        protected void SaveSocialMediaChanges()
        {
            if ( ViewMode == VIEW_MODE_EDIT )
            {
                int personEntityTypeId = EntityTypeCache.Read( typeof( Person ) ).Id;

                var rockContext = new RockContext();
                rockContext.WrapTransaction( () =>
                 {
                     var changes = new List<string>();

                     foreach ( int attributeId in AttributeList )
                     {
                         var attribute = AttributeCache.Read( attributeId );

                         if ( CurrentPerson != null )
                         {
                             Control attributeControl = fsAttributes.FindControl( string.Format( "attribute_field_{0}", attribute.Id ) );
                             if ( attributeControl != null )
                             {
                                 string originalValue = CurrentPerson.GetAttributeValue( attribute.Key );
                                 string newValue = attribute.FieldType.Field.GetEditValue( attributeControl, attribute.QualifierValues );
                                 Rock.Attribute.Helper.SaveAttributeValue( CurrentPerson, attribute, newValue, rockContext );

                                 // Check for changes to write to history
                                 if ( ( originalValue ?? string.Empty ).Trim() != ( newValue ?? string.Empty ).Trim() )
                                 {
                                     string formattedOriginalValue = string.Empty;
                                     if ( !string.IsNullOrWhiteSpace( originalValue ) )
                                     {
                                         formattedOriginalValue = attribute.FieldType.Field.FormatValue( null, originalValue, attribute.QualifierValues, false );
                                     }

                                     string formattedNewValue = string.Empty;
                                     if ( !string.IsNullOrWhiteSpace( newValue ) )
                                     {
                                         formattedNewValue = attribute.FieldType.Field.FormatValue( null, newValue, attribute.QualifierValues, false );
                                     }

                                     History.EvaluateChange( changes, attribute.Name, formattedOriginalValue, formattedNewValue );
                                 }
                             }
                         }
                     }

                     if ( changes.Any() )
                     {
                         HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(),
                             CurrentPerson.Id, changes );
                     }
                 } );
            }

            ViewMode = VIEW_MODE_VIEW;
            ConstructSocialMediaControls( false );

        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            NavigateToParentPage();
        }

        /// <summary>
        /// Handles the Click event of the btnDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnDelete_Click( object sender, EventArgs e )
        {
            var familyGroupId = _family.Id;
            var rockContext = new RockContext();
            var familyMemberService = new GroupMemberService( rockContext );
            var familyMembers = familyMemberService.GetByGroupId( familyGroupId, true );

            if ( familyMembers.Count() == 1 )
            {
                var fm = familyMembers.FirstOrDefault();

                // If the person's giving group id is this family, change their giving group id to null
                if ( fm.Person.GivingGroupId == fm.GroupId )
                {
                    var personService = new PersonService( rockContext );
                    var person = personService.Get( fm.PersonId );

                    var demographicChanges = new List<string>();
                    History.EvaluateChange( demographicChanges, "Giving Group", person.GivingGroup.Name, "" );
                    person.GivingGroupId = null;

                    HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(),
                            person.Id, demographicChanges );

                    rockContext.SaveChanges();
                }

                // remove person from family
                var oldMemberChanges = new List<string>();
                History.EvaluateChange( oldMemberChanges, "Role", fm.GroupRole.Name, string.Empty );
                History.EvaluateChange( oldMemberChanges, "Family", fm.Group.Name, string.Empty );
                HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_FAMILY_CHANGES.AsGuid(),
                    fm.Person.Id, oldMemberChanges, fm.Group.Name, typeof( Group ), fm.Group.Id );

                familyMemberService.Delete( fm );
                rockContext.SaveChanges();
            }

            var familyService = new GroupService( rockContext );

            // get the family that we want to delete (if it has no members )
            var family = familyService.Queryable()
                .Where( g =>
                     g.Id == familyGroupId &&
                     !g.Members.Any() )
                .FirstOrDefault();

            if ( family != null )
            {
                familyService.Delete( family );
                rockContext.SaveChanges();
            }

            Response.Redirect( string.Format( "~/Person/{0}", Person.Id ), false );
        }

        #endregion

        #region Methods
        private void BindMembers()
        {
            int i = 0;
            FamilyMembers.ForEach( m => m.Index = i++ );

            // only show the Delete Family button if there is only one member (or less ) in the family, and that member is in at least one other family
            btnDelete.Visible = false;
            if ( FamilyMembers.Count <= 1 )
            {
                var familyMember = FamilyMembers.FirstOrDefault();
                int familyGroupTypeId = GroupTypeCache.GetFamilyGroupType().Id;
                if ( familyMember != null )
                {
                    bool isInOtherFamilies = new GroupMemberService( new RockContext() ).Queryable()
                                    .Where( m =>
                                         m.PersonId == familyMember.Id &&
                                         m.Group.GroupTypeId == familyGroupTypeId &&
                                         m.GroupId != _family.Id ).Any();
                    if ( isInOtherFamilies )
                    {
                        // person is only person in the current family, and they are also in at least one other family, so let them delete this family
                        btnDelete.Visible = true;
                    }
                }
                else
                {
                    // somehow there are no people in this family at all, so let them delete this family
                    btnDelete.Visible = true;
                }
            }

            lvMembers.DataSource = GetMembersOrdered();
            lvMembers.DataBind();
        }

        private List<FamilyMember> GetMembersOrdered()
        {
            var orderedMembers = new List<FamilyMember>();

            // Add adult males
            orderedMembers.AddRange( FamilyMembers
                .Where( m =>
                     m.RoleGuid.Equals( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT ) ) &&
                     m.Gender == Gender.Male &&
                     m.Id != CurrentPerson.Id )
                .OrderByDescending( m => m.Age ) );

            // Add adult females
            orderedMembers.AddRange( FamilyMembers
                .Where( m =>
                     m.RoleGuid.Equals( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT ) ) &&
                     m.Gender != Gender.Male &&
                     m.Id != CurrentPerson.Id )
                .OrderByDescending( m => m.Age ) );

            // Add non-adults
            orderedMembers.AddRange( FamilyMembers
                .Where( m =>
                     !m.RoleGuid.Equals( new Guid( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT ) ) &&
                      m.Id != CurrentPerson.Id )
                .OrderByDescending( m => m.Age ) );

            return orderedMembers;
        }

        protected string FormatAddressType( object addressType )
        {
            string type = addressType.ToString();
            return type.EndsWith( "Address", StringComparison.CurrentCultureIgnoreCase ) ? type : type + " Address";
        }

        protected string FormatPersonLink( string personId )
        {
            return ResolveRockUrl( string.Format( "~/Person/{0}", personId ) );
        }

        protected string FormatPersonCssClass( bool? isDeceased )
        {
            return ( isDeceased ?? false ) ? "member deceased" : "member";
        }

        protected string FormatAsHtmlTitle( string str )
        {
            return str.FormatAsHtmlTitle();
        }

        /// <summary>
        /// Shows the details of the Person.  It takes the values from the Current Person and values the page items used to display them.
        /// </summary>
        private void ShowDetails()
        {
            var person = CurrentPerson;
            if ( person != null )
            {
                imgPhoto.BinaryFileId = person.PhotoId;
                imgPhoto.NoPictureUrl = Person.GetPersonPhotoUrl( null, null, person.Age, person.Gender, null, null, null );
                ddlTitle.SelectedValue = person.TitleValueId.HasValue ? person.TitleValueId.Value.ToString() : string.Empty;
                tbFirstName.Text = person.FirstName;
                tbNickName.Text = person.NickName;
                tbLastName.Text = person.LastName;
                ddlSuffix.SelectedValue = person.SuffixValueId.HasValue ? person.SuffixValueId.Value.ToString() : string.Empty;
                bpBirthDay.SelectedDate = person.BirthDate;
                switch ( person.Gender.ConvertToString() )
                {

                    case "Male":
                        chkMale.Checked = true;
                        cblGender.SelectedValue = person.Gender.ConvertToString();
                        break;
                    case "Female":
                        chkFemale.Checked = true;
                        cblGender.SelectedValue = person.Gender.ConvertToString();
                        break;
                    default:
                        chkMale.Checked = true;
                        cblGender.SelectedValue = "Male";
                        break;
                }
                rblMaritalStatus.SelectedValue = person.MaritalStatusValueId.HasValue ? person.MaritalStatusValueId.Value.ToString() : string.Empty;
                tbEmail.Text = person.Email;

                var mobilePhoneType = DefinedValueCache.Read( new Guid( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE ) );

                var phoneNumbers = new List<PhoneNumber>();
                var phoneNumberTypes = DefinedTypeCache.Read( new Guid( Rock.SystemGuid.DefinedType.PERSON_PHONE_TYPE ) );
                if ( phoneNumberTypes.DefinedValues.Any() )
                {
                    foreach ( var phoneNumberType in phoneNumberTypes.DefinedValues )
                    {
                        var phoneNumber = person.PhoneNumbers.FirstOrDefault( n => n.NumberTypeValueId == phoneNumberType.Id );
                        if ( phoneNumber == null )
                        {
                            var numberType = new DefinedValue();
                            numberType.Id = phoneNumberType.Id;
                            numberType.Value = phoneNumberType.Value;

                            phoneNumber = new PhoneNumber { NumberTypeValueId = numberType.Id, NumberTypeValue = numberType };
                            phoneNumber.IsMessagingEnabled = mobilePhoneType != null && phoneNumberType.Id == mobilePhoneType.Id;
                        }
                        else
                        {
                            // Update number format, just in case it wasn't saved correctly
                            phoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( phoneNumber.CountryCode, phoneNumber.Number );
                        }

                        phoneNumbers.Add( phoneNumber );
                    }

                    rContactInfo.DataSource = phoneNumbers;
                    rContactInfo.DataBind();
                }

                // And now, let's grab the Section Type for the individual
                RockContext context = new RockContext();
                var attributeService = new AttributeService( context );
                var attributeValueService = new AttributeValueService( context );
                var fieldTypeService = new FieldTypeService( context );
                var personTypeId = EntityTypeCache.Read( Rock.SystemGuid.EntityType.PERSON.AsGuid() ).Id;

                // Save their primary section...
                var fieldType = fieldTypeService.GetByName( "Defined Value" );
                var attributeObject = attributeService.GetByEntityTypeId( personTypeId ).Single( a => a.Key == "Section" );
                var attributeValueObject = attributeValueService.GetByAttributeIdAndEntityId( attributeObject.Id, CurrentPerson.Id );
                if ( attributeValueObject != null && !String.IsNullOrEmpty( attributeValueObject.Value ) )
                {
                    var sectionTypes = DefinedTypeCache.Read( new Guid( DefinedTypeHelper.SECTION_COMMUNITY_GUID.ToString( "D" ) ) );
                    var avAsGuid = attributeValueObject.Value.AsGuid();
                    var selectedSection = sectionTypes.DefinedValues.Single( v => v.Guid == avAsGuid );
                    ddlSection.SelectedValue = selectedSection.Id.ToString();
                }

                // and now the service time...
                fieldType = fieldTypeService.GetByName( "Time" );
                attributeObject = attributeService.GetByEntityTypeId( personTypeId ).Single( a => a.Key == "ServiceTime" );
                attributeValueObject = attributeValueService.GetByAttributeIdAndEntityId( attributeObject.Id, CurrentPerson.Id );
                if ( attributeValueObject != null && !String.IsNullOrEmpty( attributeValueObject.Value ) )
                {
                    tbPrimaryService.Text = attributeValueObject.Value;
                }
            }
        }

        private void BindSocialMediaAttributes()
        {
            // Social media category guid...
            Guid guid = Guid.Parse( "{dd8f467d-b83c-444f-b04c-c681167046a1}" );
            var category = CategoryCache.Read( guid );
            if ( category != null )
            {
                if ( !string.IsNullOrWhiteSpace( category.IconCssClass ) )
                {
                    lCategoryName.Text = string.Format( "<i class='{0}'></i> {1}", category.IconCssClass, category.Name );
                }
                else
                {
                    lCategoryName.Text = category.Name;
                }

                var orderOverride = new List<int>();
                GetAttributeValue( "AttributeOrder" ).SplitDelimitedValues().ToList().ForEach( a => orderOverride.Add( a.AsInteger() ) );

                var orderedAttributeList = new AttributeService( new RockContext() ).GetByCategoryId( category.Id )
                    .OrderBy( a => a.Order ).ThenBy( a => a.Name ).ToList();

                foreach ( int attributeId in orderOverride )
                {
                    var attribute = orderedAttributeList.FirstOrDefault( a => a.Id == attributeId );
                    if ( attribute != null )
                    {
                        AttributeList.Add( attribute.Id );
                    }
                }

                foreach ( var attribute in orderedAttributeList.Where( a => !orderOverride.Contains( a.Id ) ) )
                {
                    AttributeList.Add( attribute.Id );
                }

                ConstructSocialMediaControls( true );
            }
        }

        private void ConstructSocialMediaControls( bool setValues )
        {
            using ( var rockContext = new RockContext() )
            {
                fsAttributes.Controls.Clear();

                hfAttributeOrder.Value = AttributeList.AsDelimited( "|" );

                foreach ( int attributeId in AttributeList )
                {
                    var attribute = AttributeCache.Read( attributeId );
                    var attributeValueService = new AttributeValueService( rockContext );
                    string attributeValue = "";
                    var attributeValueObject = attributeValueService.GetByAttributeIdAndEntityId( attributeId, CurrentPerson.Id );
                    if ( attributeValueObject != null )
                    {
                        attributeValue = attributeValueObject.Value;
                    }
                    string formattedValue = string.Empty;

                    if ( ViewMode != VIEW_MODE_EDIT )
                    {
                        if ( ViewMode == VIEW_MODE_ORDER && _canAdministrate )
                        {
                            var div = new HtmlGenericControl( "div" );
                            fsAttributes.Controls.Add( div );
                            div.Attributes.Add( "data-attribute-id", attribute.Id.ToString() );
                            div.Attributes.Add( "class", "form-group" );

                            var a = new HtmlGenericControl( "a" );
                            div.Controls.Add( a );

                            var i = new HtmlGenericControl( "i" );
                            a.Controls.Add( i );
                            i.Attributes.Add( "class", "fa fa-bars" );

                            div.Controls.Add( new LiteralControl( " " + attribute.Name ) );
                        }
                        else
                        {
                            formattedValue = attribute.FieldType.Field.FormatValue( fsAttributes, attributeValue, attribute.QualifierValues, false );
                            if ( !string.IsNullOrWhiteSpace( formattedValue ) )
                            {
                                fsAttributes.Controls.Add( new RockLiteral { Label = attribute.Name, Text = formattedValue } );
                            }
                        }
                    }
                    else
                    {
                        attribute.AddControl( fsAttributes.Controls, attributeValue, string.Empty, setValues, true );
                    }
                }
            }
        }
        #endregion

        #endregion

        protected void rblNewPersonRole_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( rblNewPersonRole.SelectedValue == "3" )
            {
                adultDetail.Visible = true;
                childDetail.Visible = false;
            }
            else if ( rblNewPersonRole.SelectedValue == "4" )
            {
                adultDetail.Visible = false;
                childDetail.Visible = true;
            }
        }

        protected void chkMale_CheckedChanged( object sender, EventArgs e )
        {
            cblGender.SelectedValue = "Male";
            chkMale.Checked = true; // Do not allow unchecking
            chkFemale.Checked = false;
        }

        protected void chkFemale_CheckedChanged( object sender, EventArgs e )
        {
            cblGender.SelectedValue = "Female";
            chkFemale.Checked = true; // Do not allow unchecking
            chkMale.Checked = false;
        }

        protected void chkNewPersonMale_CheckedChanged( object sender, EventArgs e )
        {
            cblNewPersonGender.SelectedValue = "Male";
            chkNewPersonMale.Checked = true; // Do not allow unchecking
            chkNewPersonFemale.Checked = false;
        }

        protected void chkNewPersonFemale_CheckedChanged( object sender, EventArgs e )
        {
            cblNewPersonGender.SelectedValue = "Female";
            chkNewPersonFemale.Checked = true; // Do not allow unchecking
            chkNewPersonMale.Checked = false;
        }

        protected void ddlLanguage_SelectedIndexChanged( object sender, EventArgs e )
        {
            RockDropDownList languageDropDown = ( RockDropDownList ) sender;
            switch ( languageDropDown.SelectedValue )
            {
                case "English":
                    translateToEnglish();
                    break;
                case "Español":
                    translateToSpanish();
                    break;
                default:
                    translateToEnglish();
                    break;
            }
        }

        protected void translateToEnglish()
        {
            lPageTitle.Text = "Profile Update";
            imgPhoto.Label = "Photo";
            ddlTitle.Label = "Title";
            tbFirstName.Label = "First Name";
            tbNickName.Label = "Nickname";
            tbLastName.Label = "Last Name";
            ddlSuffix.Label = "Suffix";
            bpBirthDay.Label = "Birthday";
            cblGender.Label = "Gender";
            chkMale.Text = "Male";
            chkFemale.Text = "Female";
            rblMaritalStatus.Label = "Marital Status";
            foreach ( ListItem item in rblMaritalStatus.Items )
            {
                switch ( item.Value )
                {
                    case "143":
                        item.Text = "Married";
                        break;
                    case "144":
                        item.Text = "Single";
                        break;
                    case "895":
                        item.Text = "Separated";
                        break;
                    case "3122":
                        item.Text = "Divorced";
                        break;
                    case "897":
                        item.Text = "Widowed";
                        break;
                }
            }
            lContactInfo.Text = "Contact Info";
            foreach ( RepeaterItem item in rContactInfo.Items )
            {
                HiddenField hfPhoneType = item.FindControl( "hfPhoneType" ) as HiddenField;
                Literal phoneTypeLabel = item.FindControl( "lPhonetype" ) as Literal;
                PhoneNumberBox pnbPhone = item.FindControl( "pnbPhone" ) as PhoneNumberBox;
                CheckBox cbUnlisted = item.FindControl( "cbUnlisted" ) as CheckBox;
                cbUnlisted.Text = "unlisted";
                CheckBox cbSms = item.FindControl( "cbSms" ) as CheckBox;
                cbSms.Text = "sms";
                if ( hfPhoneType != null &&
                    pnbPhone != null &&
                    cbSms != null &&
                    cbUnlisted != null )
                {

                    int phoneNumberTypeId;
                    if ( int.TryParse( hfPhoneType.Value, out phoneNumberTypeId ) )
                    {
                        switch ( phoneNumberTypeId )
                        {
                            //mobile
                            case 12:
                                phoneTypeLabel.Text = "Mobile";
                                break;
                            //home
                            case 13:
                                phoneTypeLabel.Text = "Home";
                                break;
                            //work
                            case 136:
                                phoneTypeLabel.Text = "Work";
                                break;
                        }
                    }

                }
            }

            tbEmail.Label = "Email";
            lFamilyInformation.Text = "Family Information";
            cpCampus.Label = "Campus";
            tbPrimaryService.Label = "Primary Service";
            ddlSection.Label = "Primary Section";
            tbFamilyName.Label = "Family Name";
            lAddFamilyMemberNote.Text = "* Please add only your spouse and / or children who are part of your household.";
            lAddPerson.Text = "Add Additional Family Member";


            //Edit button on member
            foreach ( ListViewItem item in lvMembers.Items )
            {
                LinkButton editFamilyMember = item.FindControl( "btnEditFamilyMember" ) as LinkButton;
                if(editFamilyMember != null )
                {
                    editFamilyMember.Text = "Edit";
                }
            }

            lHomeAddress.Text = "Home Address";
            acHomeAddress.RequiredErrorMessage = "Your Home Address is Required";
            btnSave.Text = "Save";
            btnCancel.Text = "Cancel";
            btnDelete.Text = "Delete";

            lAdditionalProfileChangesNote.Text = "If you need assistance making changes to your profile, please click the following";

            confirmExit.ConfirmationMessage = "Changes have been made to this family that have not yet been saved.";
            //Modal Add Person
            modalAddPerson.Title = "Add Person";
            tbNewPersonFirstName.Label = "First Name";
            tbNewPersonLastName.Label = "Last Name";
            tbNewPersonNickName.Label = "Nickname";
            chkNewPersonMale.Text = "Male";
            chkNewPersonFemale.Text = "Female";
            dpNewPersonBirthDate.Label = "Birthdate";
            rblNewPersonRole.Label = "Role";
            pnbNewPersonMobile.Label = "Mobile";
            tbNewPersonEmail.Label = "Email";
            ddlGradePicker.Label = "Grade";
            ypGraduation.Label = "Graduation Year";
            ypGraduation.Help = "High School Graduation Year.";
            tbChildHealthNote.Label = "Health Note";
            pnbNewPersonWork.Label = "Work";
            this.Language = "English";            
        }


        protected void translateToSpanish()
        {
            lPageTitle.Text = "Actualización del Perfil";
            imgPhoto.Label = "Foto";
            ddlTitle.Label = "Título";
            tbFirstName.Label = "Primer Nombre";
            tbNickName.Label = "Apodo";
            tbLastName.Label = "Apellido(s)";
            ddlSuffix.Label = "Sufijo";
            bpBirthDay.Label = "Fecha de Nacimiento";
            cblGender.Label = "Sexo";
            chkMale.Text = "Masculino";
            chkFemale.Text = "Femenino";
            rblMaritalStatus.Label = "Estado Civil";
           
            foreach(ListItem item in rblMaritalStatus.Items )
            {
                switch ( item.Value )
                {
                    case "143":
                        item.Text = "Casado";
                        break;
                    case "144":
                        item.Text = "Soltero";
                        break;
                    case "895":
                        item.Text = "Separado";
                        break;
                    case "3122":
                        item.Text = "Divorciado";
                        break;
                    case "897":
                        item.Text = "Viudo";
                        break;
                }
            }
            lContactInfo.Text = "Contacto";
            foreach ( RepeaterItem item in rContactInfo.Items )
            {
                HiddenField hfPhoneType = item.FindControl( "hfPhoneType" ) as HiddenField;
                Literal phoneTypeLabel = item.FindControl( "lPhonetype" ) as Literal;
                PhoneNumberBox pnbPhone = item.FindControl( "pnbPhone" ) as PhoneNumberBox;
                CheckBox cbUnlisted = item.FindControl( "cbUnlisted" ) as CheckBox;
                cbUnlisted.Text = "privado";
                CheckBox cbSms = item.FindControl( "cbSms" ) as CheckBox;
                cbSms.Text = "sms";
                if ( hfPhoneType != null &&
                    pnbPhone != null &&
                    cbSms != null &&
                    cbUnlisted != null )
                {

                    int phoneNumberTypeId;
                    if ( int.TryParse( hfPhoneType.Value, out phoneNumberTypeId ) )
                    {
                        switch ( phoneNumberTypeId )
                        {
                            //mobile
                            case 12:
                                phoneTypeLabel.Text = "Cellular";
                                break;
                            //home
                            case 13:
                                phoneTypeLabel.Text = "Casa";
                                break;
                            //work
                            case 136:
                                phoneTypeLabel.Text = "Trabajo";
                                break;
                        }
                    }

                }
            }

            tbEmail.Label = "Correo Electrónico";
            lFamilyInformation.Text = "Información de la Familia";
            cpCampus.Label = "Campus";
            tbPrimaryService.Label = "Servicio Principal";
            ddlSection.Label = "Sección Principal";
            tbFamilyName.Label = "Apellido de la Familia";
            lAddFamilyMemberNote.Text = "* Por favor solo agregue a su esposo(a) e hijos que aún forman parte de su hogar.";
            lAddPerson.Text = "Añadir Miembro de Familia";


            //Edit button on member
            foreach ( ListViewItem item in lvMembers.Items )
            {
                LinkButton editFamilyMember = item.FindControl( "btnEditFamilyMember" ) as LinkButton;
                if ( editFamilyMember != null )
                {
                    editFamilyMember.Text = "Editar";
                }
            }

            lHomeAddress.Text = "Domicilio de Casa";
            acHomeAddress.RequiredErrorMessage = "Su Domicilio de Casa es Requerido";
            btnSave.Text = "Guardar";
            btnCancel.Text = "Cancelar";
            btnDelete.Text = "Borrar";

            lAdditionalProfileChangesNote.Text = "Si necesita ayuda para hacer cambios a su perfil, por favor haga clic en el siguiente";

            confirmExit.ConfirmationMessage = "Se han realizado cambios a la familia que aún no han sido guardados.";
            //Modal Add Person
            modalAddPerson.Title = "Agregar Persona";
            tbNewPersonFirstName.Label = "Primer Nombre";
            tbNewPersonLastName.Label = "Apellido(s)";
            tbNewPersonNickName.Label = "Apodo";
            chkNewPersonMale.Text = "Masculino";
            chkNewPersonFemale.Text = "Femenino";
            dpNewPersonBirthDate.Label = "Fecha de Nacimiento";
            rblNewPersonRole.Label = "Rol";
            pnbNewPersonMobile.Label = "Cellular";
            tbNewPersonEmail.Label = "Correo Electrónico";
            ddlGradePicker.Label = "Grado Escolar";
            ypGraduation.Label = "Año de Graduación";
            ypGraduation.Help = "Año en que se gradúa de High School.";
            tbChildHealthNote.Label = "Allergias";
            pnbNewPersonWork.Label = "Trabajo";
            this.Language = "Spanish";
        }

    }
    [Serializable]
    class FamilyMember
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public bool ExistingFamilyMember { get; set; }  // Is this person part of the original family 
        public bool Removed { get; set; } // Was an existing person removed from the family (to their own family)
        public bool RemoveFromOtherFamilies { get; set; } // When adding an existing person, should they be removed from other families
        public string FirstName { get; set; }
        public string NickName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public Guid RoleGuid { get; set; }
        public string RoleName { get; set; }
        public int? PhotoId { get; set; }
        public int? ConnectionStatusValueId { get; set; }

        public string Allergy { get; set; }
        public int? GradeOffset { get; set; }

        public int? GraduationYear { get; set; }

        public bool? HasGraduated { get; set; }

        public string Mobile { get; set; }
        public string Work { get; set; }
        public string EmailAddress { get; set; }

        public int? Age
        {
            get
            {
                if ( BirthDate.HasValue )
                {
                    return BirthDate.Age();
                }

                return null;
            }
        }

        public FamilyMember( GroupMember familyMember, bool existingFamilyMember )
        {
            if ( familyMember != null )
            {
                SetValuesFromPerson( familyMember.Person );

                if ( familyMember.GroupRole != null )
                {
                    RoleGuid = familyMember.GroupRole.Guid;
                    RoleName = familyMember.GroupRole.Name;
                }
            }

            ExistingFamilyMember = existingFamilyMember;
            Removed = false;
        }

        public FamilyMember()
        {
            Id = -1;
            ExistingFamilyMember = false;
            Removed = false;
            RemoveFromOtherFamilies = false;
        }

        public void SetValuesFromPerson( Person person )
        {
            using ( var rockContext = new RockContext() )
            {
                if ( person != null )
                {
                    Id = person.Id;
                    FirstName = person.FirstName;
                    NickName = person.NickName;
                    LastName = person.LastName;
                    Gender = person.Gender;
                    BirthDate = person.BirthDate;
                    PhotoId = person.PhotoId;
                    EmailAddress = person.Email;
                    //Extract additinal attributes
                    Guid mobilePhoneGuid = org.willowcreek.SystemGuid.DefinedValueGuids.MOBILE_PHONE.AsGuid();
                    var mobilePhoneNumberDefinedValue = new DefinedValueService( rockContext ).Queryable().Where( m => m.Guid == mobilePhoneGuid ).FirstOrDefault();
                    Mobile = new PhoneNumberService( rockContext ).Queryable().Where( p => p.PersonId == person.Id && p.NumberTypeValueId == mobilePhoneNumberDefinedValue.Id ).Select( s => s.NumberFormatted ).FirstOrDefault();
                    Guid workPhoneGuid = org.willowcreek.SystemGuid.DefinedValueGuids.WORK_PHONE.AsGuid();
                    var workPhoneNumberDefinedValue = new DefinedValueService( rockContext ).Queryable().Where( m => m.Guid == workPhoneGuid ).FirstOrDefault();
                    Work = new PhoneNumberService( rockContext ).Queryable().Where( p => p.PersonId == person.Id && p.NumberTypeValueId == workPhoneNumberDefinedValue.Id ).Select( s => s.NumberFormatted ).FirstOrDefault();
                    if ( person.GradeOffset.HasValue )
                    {
                        GradeOffset = person.GradeOffset.Value;
                    }
                    if ( person.GraduationYear.HasValue )
                    {
                        GraduationYear = person.GraduationYear;
                    }
                    if ( person.HasGraduated.HasValue )
                    {
                        HasGraduated = person.HasGraduated.Value;
                    }
                    Guid allergyGuid = AttributeGuids.ALLERGY.AsGuid();
                    var allergyAttribute = new AttributeService( rockContext ).Queryable().Where( k => k.Guid == allergyGuid ).FirstOrDefault();
                    Allergy = new AttributeValueService( rockContext ).Queryable().Where( a => a.AttributeId == allergyAttribute.Id && a.EntityId == person.Id ).Select( s => s.Value ).FirstOrDefault();
                }
            }
        }
    }
}