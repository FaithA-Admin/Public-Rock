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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Model;

namespace RockWeb.Plugins.org_willowcreek.CheckIn
{
    [DisplayName("Person Select (Family Check-in) w/ Add Person")]
    [Category("org_willowcreek > Check-in")]
    [Description("Lists people who match the selected family and provides option of selecting multiple.")]
    [BooleanField( "Translate Block Into Spanish", "", false )]
    public partial class MultiPersonSelect : CheckInBlock
    {
        bool _hidePhotos = false;

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            rSelection.ItemDataBound += rSelection_ItemDataBound;

            string script = string.Format( @"
        function GetPersonSelection() {{
            var ids = '';
            $('div.checkin-person-list').find('i.fa-check-square').each( function() {{
                ids += $(this).closest('a').attr('person-id') + ',';
            }});
            if (ids == '') {{
                bootbox.alert('{0}');
                return false;
            }}
            else
            {{
                $('#{1}').button('loading')
                $('#{2}').val(ids);
                return true;
            }}
        }}

        $('a.btn-checkin-select-check').click( function() {{
            //$(this).toggleClass('btn-dimmed');
            $(this).find('i').toggleClass('fa-check-square').toggleClass('fa-square-o');
        }});

", (!GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Please select at least one person" : "Por favor seleccione al menos una persona"), lbSelect.ClientID, hfPeople.ClientID );
            ScriptManager.RegisterStartupScript( Page, Page.GetType(), "SelectPerson", script, true );
        }

        private void rSelection_ItemDataBound( object sender, RepeaterItemEventArgs e )
        {
            if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
            {
                var phCheck = e.Item.FindControl( "phCheck" ) as PlaceHolder;
                var pnlCheckAndPhoto = e.Item.FindControl( "pnlCheckAndPhoto" ) as Panel;
                if ( phCheck != null && pnlCheckAndPhoto != null )
                {
                    phCheck.Visible = _hidePhotos;
                    pnlCheckAndPhoto.Visible = !_hidePhotos;
                }
            }
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            RockPage.AddScriptLink( "~/Scripts/iscroll.js" );
            RockPage.AddScriptLink( "~/Scripts/CheckinClient/checkin-core.js" );

            lbBack.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Back" : "Regresar";
            lbCancel.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Cancel" : "Cancelar";
            lbSelect.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Next" : "Siguiente";
            lSelectPeople.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Select People" : "Seleccionar Personas";

            var bodyTag = this.Page.Master.FindControl( "bodyTag" ) as HtmlGenericControl;
            if ( bodyTag != null )
            {
                bodyTag.AddCssClass( "checkin-multipersonselect-bg" );
            }

            if ( CurrentWorkflow == null || CurrentCheckInState == null )
            {
                NavigateToHomePage(); 
            }
            else
            {
                if ( !Page.IsPostBack )
                {
                    ClearSelection();

                    var family = CurrentCheckInState.CheckIn.CurrentFamily;
                    if ( family == null )
                    {
                        GoBack();
                    }

                    lFamilyName.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? family.ToString() : "Familia " + family.ToString().Replace("Family","");

                    _hidePhotos = CurrentCheckInState.CheckInType.HidePhotos;

                    var dataSource = family.People
                        .OrderByDescending( p => p.FamilyMember )
                        .ThenBy( p => p.Person.BirthYear )
                        .ThenBy( p => p.Person.BirthMonth )
                        .ThenBy( p => p.Person.BirthDay )
                        .ToList();

                    if (dataSource.Any())
                    {
                        rSelection.DataSource = dataSource;
                        rSelection.DataBind();
                    }
                    else
                    {
                        lAddPerson.Visible = true;
                        lAddPerson.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "<p>No one in your family is eligible to check-in at this location.</p>" : "<p>Nadie en su familia es elegible para registrarse en este lugar.</p>";
                    }

                    

                }
            }
        }

        /// <summary>
        /// Clear any previously selected people.
        /// </summary>
        private void ClearSelection()
        {
            foreach ( var family in CurrentCheckInState.CheckIn.Families )
            {
                foreach ( var person in family.People )
                {
                    person.ClearFilteredExclusions();
                    person.PossibleSchedules = new List<CheckInSchedule>();
                    person.Selected = false;
                    person.Processed = false;
                }
            }
        }

        protected void lbSelect_Click( object sender, EventArgs e )
        {
            if ( KioskCurrentlyActive )
            {
                var selectedPersonIds = hfPeople.Value.SplitDelimitedValues().AsIntegerList();

                var family = CurrentCheckInState.CheckIn.CurrentFamily;
                if ( family != null )
                {
                    foreach ( var person in family.People )
                    {
                        person.Selected = selectedPersonIds.Contains( person.Person.Id );
                        person.PreSelected = person.Selected;
                    }

                    ProcessSelection( maWarning );
                }
            }
        }

        protected void lbBack_Click( object sender, EventArgs e )
        {
            GoBack();
        }

        protected void lbCancel_Click( object sender, EventArgs e )
        {
            CancelCheckin();
        }

        protected void ProcessSelection()
        {
            ProcessSelection( 
                maWarning, 
                () => CurrentCheckInState.CheckIn.CurrentFamily.GetPeople( true )
                    .SelectMany( p => p.GroupTypes.Where( t => !t.ExcludedByFilter ) ) 
                    .Count() <= 0,
                "<p>Sorry, there are currently not any available areas that the selected people can check into.</p>" );
        }

        protected string GetCheckboxClass( bool selected )
        {
            return selected ? "fa fa-check-square fa-3x" : "fa fa-square-o fa-3x";
        }

        protected string GetPersonImageTag( object dataitem )
        {
            var person = dataitem as Person;
            if ( person != null )
            {
                return Person.GetPersonPhotoUrl( person, 200, 200 );
            }
            return string.Empty;
        }
    }
}