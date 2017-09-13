// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
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
using System.Text;
using System.Web.UI;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using org.willowcreek.SystemGuid;

namespace RockWeb.Plugins.org_willowcreek.Crm.PersonDetail
{
    /// <summary>
    /// The main Person Profile block the main information about a peron 
    /// </summary>
    [DisplayName( "Person Bio" )]
    [Category( "org_willowcreek > CRM > Person Detail" )]
    [Description( "Person biographic/demographic information and picture (Person detail page)." )]

    [PersonBadgesField( "Badges", "The label badges to display in this block.", false, "", "", 0 )]
    [WorkflowTypeField( "Workflow Actions", "The workflows to make available as actions.", true, false, "", "", 1 )]
    [CodeEditorField( "Additional Custom Actions", @"
Additional custom actions (will be displayed after the list of workflow actions). Any instance of '{0}' will be replaced with the current person's id.
Because the contents of this setting will be rendered inside a &lt;ul&gt; element, it is recommended to use an 
&lt;li&gt; element for each available action.  Example:
<pre>
    &lt;li&gt;&lt;a href='~/WorkflowEntry/4?PersonId={0}' tabindex='0'&gt;Fourth Action&lt;/a&gt;&lt;/li&gt;
</pre>
", Rock.Web.UI.Controls.CodeEditorMode.Html, Rock.Web.UI.Controls.CodeEditorTheme.Rock, 200, false, "", "", 2, "Actions" )]
    [LinkedPage( "Business Detail Page", "The page to redirect user to if a business is is requested.", false, "", "", 3 )]
    [BooleanField( "Display Country Code", "When enabled prepends the country code to all phone numbers.", false, "", 4 )]
    [BooleanField( "Display Middle Name", "Display the middle name of the person.", false, "", 5 )]
    [CodeEditorField( "Custom Content", "Custom Content will be rendered after the person's demographic information <span class='tip tip-lava'></span>.",
        Rock.Web.UI.Controls.CodeEditorMode.Lava, Rock.Web.UI.Controls.CodeEditorTheme.Rock, 200, false, "", "", 6, "CustomContent" )]
    [BooleanField( "Allow Following", "Should people be able to follow a person by selecting the star on the person's photo?", true, "", 7 )]
    public partial class Bio : PersonBlock
    {
        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            RockPage.AddCSSLink( ResolveRockUrl( "~/Styles/fluidbox.css" ) );
            RockPage.AddScriptLink( ResolveRockUrl( "~/Scripts/imagesloaded.min.js" ) );
            RockPage.AddScriptLink( ResolveRockUrl( "~/Scripts/jquery.fluidbox.min.js" ) );

            if ( Person != null )
            {
                pnlFollow.Visible = GetAttributeValue( "AllowFollowing" ).AsBoolean();

                // Record Type - this is always "business". it will never change.
                if ( Person.RecordTypeValueId == DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_BUSINESS.AsGuid() ).Id )
                {
                    var parms = new Dictionary<string, string>();
                    parms.Add( "businessId", Person.Id.ToString() );
                    NavigateToLinkedPage( "BusinessDetailPage", parms );
                }

                if ( Person.IsDeceased )
                {
                    divBio.AddCssClass( "deceased" );
                }

                // Set the browser page title to include person's name
                RockPage.BrowserTitle = Person.FullName;

                string badgeList = GetAttributeValue( "Badges" );
                if ( !string.IsNullOrWhiteSpace( badgeList ) )
                {
                    foreach ( string badgeGuid in badgeList.SplitDelimitedValues() )
                    {
                        Guid guid = badgeGuid.AsGuid();
                        if ( guid != Guid.Empty )
                        {
                            var personBadge = PersonBadgeCache.Read( guid );
                            if ( personBadge != null )
                            {
                                blStatus.PersonBadges.Add( personBadge );
                            }
                        }
                    }
                }

                lbEditPerson.Visible = IsUserAuthorized( Rock.Security.Authorization.EDIT );
            }
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
                if ( Person != null && Person.Id != 0 )
                {
                    SetPersonName();

                    // Setup Image
                    string imgTag = Rock.Model.Person.GetPersonPhotoImageTag( Person, 200, 200 );
                    if ( Person.PhotoId.HasValue )
                    {
                        lImage.Text = string.Format( "<a href='{0}'>{1}</a>", Person.PhotoUrl, imgTag );
                    }
                    else
                    {
                        lImage.Text = imgTag;
                    }

                    if ( GetAttributeValue( "AllowFollowing" ).AsBoolean() )
                    {
                        FollowingsHelper.SetFollowing( Person.PrimaryAlias, pnlFollow, this.CurrentPerson );
                    }

                    var socialCategoryGuid = Rock.SystemGuid.Category.PERSON_ATTRIBUTES_SOCIAL.AsGuid();
                    if ( !socialCategoryGuid.IsEmpty() )
                    {
                        var attributes = Person.Attributes.Where( p => p.Value.Categories.Select( c => c.Guid ).Contains( socialCategoryGuid ) );
                        var result = attributes.Join( Person.AttributeValues, a => a.Key, v => v.Key, ( a, v ) => new { Attribute = a.Value, Value = v.Value } );

                        rptSocial.DataSource = result
                            .Where( r =>
                                r.Value != null &&
                                r.Value.Value != string.Empty )
                            .OrderBy( r => r.Attribute.Order )
                            .Select( r => new
                            {
                                url = r.Value.Value,
                                name = r.Attribute.Name,
                                icon = r.Attribute.IconCssClass
                            } )
                            .ToList();
                        rptSocial.DataBind();
                    }

                    if ( Person.BirthDate.HasValue )
                    {
                        lAge.Text = string.Format( "{0}<small>({1})</small><br/>", Person.FormatAge(), ( Person.BirthYear.HasValue && Person.BirthYear != DateTime.MinValue.Year ) ? Person.BirthDate.Value.ToShortDateString() : Person.BirthDate.Value.ToMonthDayString() );
                    }

                    lGender.Text = Person.Gender.ToString();

                    if ( Person.GraduationYear.HasValue && Person.HasGraduated.HasValue )
                    {
                        lGraduation.Text = string.Format(
                            "<small>({0} {1})</small>",
                            Person.HasGraduated.Value ? "Graduated " : "Graduates ",
                            Person.GraduationYear.Value );
                    }

                    lGrade.Text = Person.GradeFormatted;


                    //Load Jr HighSchool and HighSchool names.
                    using ( var rockContext = new RockContext() )
                    {
                        Guid jrHighSchoolAttributeAsGuid = AttributeGuids.JR_HIGHSCHOOL.AsGuid();
                        Guid highSchoolAttributeAsGuid = AttributeGuids.HIGHSCHOOL.AsGuid();
                        var attributeValueService = new AttributeValueService( rockContext );
                        var attributeService = new AttributeService( rockContext );
                        var definedValueService = new DefinedValueService( rockContext );

                        var jrHighSchoolAttributeValue = ( from av in attributeValueService.Queryable()
                                                           join a in attributeService.Queryable() on av.AttributeId equals a.Id
                                                           where a.Guid == jrHighSchoolAttributeAsGuid && av.EntityId == Person.Id
                                                           select av.Value ).FirstOrDefault().AsGuidOrNull();
                        var highSchoolAttributeValue = ( from av in attributeValueService.Queryable()
                                                         join a in attributeService.Queryable() on av.AttributeId equals a.Id
                                                         where a.Guid == highSchoolAttributeAsGuid && av.EntityId == Person.Id
                                                         select av.Value ).FirstOrDefault().AsGuidOrNull();

                        if ( jrHighSchoolAttributeValue.HasValue )
                        {
                            lSchools.Text = " <small>Jr. H.S.: " + definedValueService.Queryable().Where( dv => dv.Guid == jrHighSchoolAttributeValue.Value ).Select( s => s.Value ).FirstOrDefault() + "</small>";
                        }

                        if ( highSchoolAttributeValue.HasValue )
                        {
                            if ( lSchools.Text != "" )
                            {
                                lSchools.Text += "<small>,</small>";
                            }
                            lSchools.Text += " <small>H.S.: " + definedValueService.Queryable().Where( dv => dv.Guid == highSchoolAttributeValue.Value ).Select( s => s.Value ).FirstOrDefault() + "</small>";
                        }

                    }


                    //Get Rock family Checkin IDs.
                    using ( var rockContext = new RockContext() )
                    {
                        var groupMemberService = new GroupMemberService( rockContext );
                        var groupService = new GroupService( rockContext );
                        var groupType = new GroupTypeService( rockContext );
                        Guid familyGroupTypeGuidAsGuid = GroupTypeGuids.FAMILY.AsGuid();

                        var checkinQry = from gm in groupMemberService.Queryable()
                                         join g in groupService.Queryable() on gm.GroupId equals g.Id
                                         join gt in groupType.Queryable() on g.GroupTypeId equals gt.Id
                                         where gt.Guid == familyGroupTypeGuidAsGuid
                                                && gm.PersonId == Person.Id
                                         select g;
                        var checkinList = checkinQry.ToList();
                        string rockCheckinFamilies = "";
                        for ( int i = 0; i < checkinList.Count; i++ )
                        {
                            rockCheckinFamilies += checkinList[i].Id.ToString() + " (" + checkinList[i].Name + ")";
                            if ( i < checkinList.Count - 1 )
                            {
                                rockCheckinFamilies += ",";
                            }
                        }
                        if ( checkinList.Count == 1 )
                        {
                            fsRockCheckinIds.Controls.Add( new RockLiteral { Text = "Rock Check-In ID: " + rockCheckinFamilies } );
                        }
                        else if ( checkinList.Count > 1 )
                        {
                            fsRockCheckinIds.Controls.Add( new RockLiteral { Text = "Rock Check-In IDs: " + rockCheckinFamilies } );
                        }
                    }


                    lMaritalStatus.Text = Person.MaritalStatusValueId.DefinedValue();
                    if ( Person.AnniversaryDate.HasValue )
                    {
                        lAnniversary.Text = string.Format( "{0} yrs <small>({1})</small>", Person.AnniversaryDate.Value.Age(), Person.AnniversaryDate.Value.ToMonthDayString() );
                    }

                    if ( Person.PhoneNumbers != null )
                    {
                        rptPhones.DataSource = Person.PhoneNumbers.ToList();
                        rptPhones.DataBind();
                    }

                    lEmail.Text = Person.GetEmailTag( ResolveRockUrl( "/" ) );
                    if ( !string.IsNullOrEmpty( Person.Email ) )
                    {
                        lEmailPerson.Text = string.Format( "<a style=\"color:#fff; background-color:#4f89bd; border-color:#4f89bd; display:inline-block; margin-bottom:0; font-weight:normal; text-align:center; vertical-align:middle; touch-action:manipulation; cursor:pointer; background-image:none; border:1px solid transparent; white-space:nowrap; padding:2px 12px; font-size:12px; border-radius:4px; line-height:1.428571429; user-select:none;\" href=\"mailto:{0}\"><i class=\"fa fa-envelope\"></i> Email {1}</a>", Person.Email, Person.NickName );
                    }

                    foreach(var smsEnabledPhone in Person.PhoneNumbers.ToList() )
                    {
                        if ( smsEnabledPhone.IsMessagingEnabled )
                        {
                            lSMSPerson.Text = string.Format( "<a style=\"color:#fff; background-color:#8cc63e; border-color:#8cc63e; display:inline-block; margin-bottom:0; font-weight:normal; text-align:center; vertical-align:middle; touch-action:manipulation; cursor:pointer; background-image:none; border:1px solid transparent; white-space:nowrap; padding:2px 12px; font-size:12px; border-radius:4px; line-height:1.428571429; user-select:none;\" href=\"/SMSCommunication?person={0}\"><i class=\"fa fa-comment\"></i> SMS {1}</a>", Person.Id.ToString(), Person.NickName );
                            break;
                        }
                    }

                    taglPersonTags.EntityTypeId = Person.TypeId;
                    taglPersonTags.EntityGuid = Person.Guid;
                    taglPersonTags.GetTagValues( CurrentPersonId );

                    StringBuilder sbActions = new StringBuilder();
                    SortedDictionary<string, string> sortedWorkflowActions = new SortedDictionary<string, string>();
                    var workflowActions = GetAttributeValue( "WorkflowActions" );
                    if ( !string.IsNullOrWhiteSpace( workflowActions ) )
                    {
                        using ( var rockContext = new RockContext() )
                        {
                            var workflowTypeService = new WorkflowTypeService( rockContext );
                            foreach ( string guidValue in workflowActions.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ) )
                            {
                                Guid? guid = guidValue.AsGuidOrNull();
                                if ( guid.HasValue )
                                {
                                    var workflowType = workflowTypeService.Get( guid.Value );
                                    if ( workflowType != null && workflowType.IsAuthorized( Authorization.VIEW, CurrentPerson ) )
                                    {
                                        string url = string.Format( "~/WorkflowEntry/{0}?PersonId={1}", workflowType.Id, Person.Id );
                                        
                                        if ( !sortedWorkflowActions.ContainsKey( workflowType.Name ) )
                                        {
                                            sortedWorkflowActions.Add( workflowType.Name, string.Format( "<li><a href='{0}'><i class='fa-fw {1}'></i> {2}</a></li>", ResolveRockUrl( url ), workflowType.IconCssClass, workflowType.Name ) );
                                        }else
                                        {
                                            sortedWorkflowActions[workflowType.Name] = sortedWorkflowActions[workflowType.Name] + Environment.NewLine + string.Format( "<li><a href='{0}'><i class='fa-fw {1}'></i> {2}</a></li>", ResolveRockUrl( url ), workflowType.IconCssClass, workflowType.Name );
                                        }
                                       
                                    }
                                }
                            }
                        }
                    }

                    foreach(var action in sortedWorkflowActions )
                    {
                        sbActions.Append( action.Value );
                        sbActions.AppendLine();
                    }

                    var actions = GetAttributeValue( "Actions" );
                    if ( !string.IsNullOrWhiteSpace( actions ) )
                    {
                        string appRoot = ResolveRockUrl( "~/" );
                        string themeRoot = ResolveRockUrl( "~~/" );
                        actions = actions.Replace( "~~/", themeRoot ).Replace( "~/", appRoot );

                        if ( actions.Contains( "{0}" ) )
                        {
                            actions = string.Format( actions, Person.Id );
                        }

                        sbActions.Append( actions );
                    }

                    lActions.Text = sbActions.ToString();
                    ulActions.Visible = !string.IsNullOrWhiteSpace( lActions.Text );

                    string customContent = GetAttributeValue( "CustomContent" );
                    if ( !string.IsNullOrWhiteSpace( customContent ) )
                    {
                        var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( RockPage, CurrentPerson );
                        string resolvedContent = customContent.ResolveMergeFields( mergeFields );
                        phCustomContent.Controls.Add( new LiteralControl( resolvedContent ) );
                    }

                }
                else
                {
                    nbInvalidPerson.Visible = true;
                    pnlContent.Visible = false;
                }
            }
        }

        private void SetPersonName()
        {
            // Check if this record represents a Business.
            bool isBusiness = false;

            if ( Person.RecordTypeValueId.HasValue )
            {
                int recordTypeValueIdBusiness = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_BUSINESS.AsGuid() ).Id;

                isBusiness = ( Person.RecordTypeValueId.Value == recordTypeValueIdBusiness );
            }

            // Get the Display Name.
            string nameText;

            if ( isBusiness )
            {
                nameText = Person.LastName;
            }
            else
            {
                if ( GetAttributeValue( "DisplayMiddleName" ).AsBoolean() && !String.IsNullOrWhiteSpace( Person.MiddleName ) )
                {
                    nameText = string.Format( "<span class='first-word'>{0}</span> <span class='middlename'>{1}</span> <span class='lastname'>{2}</span>", Person.NickName, Person.MiddleName, Person.LastName );
                }
                else
                {
                    nameText = string.Format( "<span class='first-word'>{0}</span> <span class='lastname'>{1}</span>", Person.NickName, Person.LastName );
                }


                // Add First Name if different from NickName.
                if ( Person.NickName != Person.FirstName )
                {
                    if ( !string.IsNullOrWhiteSpace( Person.FirstName ) )
                    {
                        nameText += string.Format( " <span class='firstname'>({0})</span>", Person.FirstName );
                    }
                }

                // Add Suffix.
                if ( Person.SuffixValueId.HasValue )
                {
                    var suffix = DefinedValueCache.Read( Person.SuffixValueId.Value );
                    if ( suffix != null )
                    {
                        nameText += " " + suffix.Value;
                    }
                }

                // Add Previous Names. 
                using ( var rockContext = new RockContext() )
                {
                    var previousNames = Person.GetPreviousNames( rockContext ).Select( a => a.LastName );

                    if ( previousNames.Any() )
                    {
                        nameText += string.Format( Environment.NewLine + "<span class='previous-names'>(Previous Names: {0})</span>", previousNames.ToList().AsDelimited( ", " ) );
                    }
                }
            }

            lName.Text = nameText;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the lbEditPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbEditPerson_Click( object sender, EventArgs e )
        {
            if ( Person != null )
            {
                Response.Redirect( string.Format( "~/Person/{0}/Edit", Person.Id ) );
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the phone number.
        /// </summary>
        /// <param name="unlisted">if set to <c>true</c> [unlisted].</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="number">The number.</param>
        /// <param name="phoneNumberTypeId">The phone number type identifier.</param>
        /// <returns></returns>
        protected string
        FormatPhoneNumber( bool unlisted, object countryCode, object number, int phoneNumberTypeId, bool smsEnabled = false )
        {
            string formattedNumber = "Unlisted";

            string cc = countryCode as string ?? string.Empty;
            string n = number as string ?? string.Empty;

            if ( !unlisted )
            {
                if ( GetAttributeValue( "DisplayCountryCode" ).AsBoolean() )
                {
                    formattedNumber = PhoneNumber.FormattedNumber( cc, n, true );
                }
                else
                {
                    formattedNumber = PhoneNumber.FormattedNumber( cc, n );
                }
            }

            // if the page is being loaded locally then add the tel:// link
            if ( RockPage.IsMobileRequest )
            {
                formattedNumber = string.Format( "<a href=\"tel://{0}\">{1}</a>", n, formattedNumber );
            }

            var phoneType = DefinedValueCache.Read( phoneNumberTypeId );
            if ( phoneType != null )
            {
                if ( smsEnabled )
                {
                    formattedNumber = string.Format( "{0} <small>{1} <i class='fa fa-comments'></i></small>", formattedNumber, phoneType.Value );
                }
                else
                {
                    formattedNumber = string.Format( "{0} <small>{1}</small>", formattedNumber, phoneType.Value );
                }
            }

            return formattedNumber;
        }

        #endregion
    }
}