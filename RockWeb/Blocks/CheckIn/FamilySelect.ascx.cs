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

namespace RockWeb.Blocks.CheckIn
{
    [DisplayName("Family Select")]
    [Category("Check-in")]
    [Description( "Displays a list of families to select for checkin." )]

    [LinkedPage( "Next Page (Family Check-in)", "", false, "", "", 5, "FamilyNextPage" )]
    [BooleanField( "Translate Block Into Spanish", "", false )]
    public partial class FamilySelect : CheckInBlock
    {
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            RockPage.AddScriptLink( "~/Scripts/iscroll.js" );
            RockPage.AddScriptLink( "~/Scripts/CheckinClient/checkin-core.js" );

            lHeader.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Families" : "Familias";
            lSelectFamily.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Select Your Family" : "Selecciona Tu Familia";
            lbBack.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Back" : "Regresar";
            lbCancel.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Cancel" : "Cancelar";

            var bodyTag = this.Page.Master.FindControl( "bodyTag" ) as HtmlGenericControl;
            if ( bodyTag != null )
            {
                bodyTag.AddCssClass( "checkin-familyselect-bg" );
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
                    if ( CurrentCheckInState.CheckIn.Families.Count == 1 &&
                        !CurrentCheckInState.CheckIn.ConfirmSingleFamily )
                    {
                        if ( UserBackedUp )
                        {
                            GoBack();
                        }
                        else
                        {
                            foreach ( var family in CurrentCheckInState.CheckIn.Families )
                            {
                                family.Selected = true;
                            }

                            ProcessSelection();
                        }
                    }
                    else
                    {
                            var families = CurrentCheckInState.CheckIn.Families
                            .OrderBy( f => f.Caption )
                            .ThenBy( f => f.SubCaption )
                            .ToList();

                        if( GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() )
                        {
                            foreach ( var family in families )
                            {
                                family.Caption = "Familia " + family.Caption.ToString().Replace( "Family", "" );
                            }
                        }                       

                        rSelection.DataSource = families;



                        rSelection.DataBind();
                    }
                }
            }
        }

        /// <summary>
        /// Clear any previously selected families.
        /// </summary>
        private void ClearSelection()
        {
            foreach ( var family in CurrentCheckInState.CheckIn.Families )
            {
                family.Selected = false;
                family.People = new List<CheckInPerson>();
            }
        }

        protected void rSelection_ItemCommand( object source, RepeaterCommandEventArgs e )
        {
            if ( KioskCurrentlyActive )
            {
                int id = Int32.Parse( e.CommandArgument.ToString() );
                var family = CurrentCheckInState.CheckIn.Families.Where( f => f.Group.Id == id ).FirstOrDefault();
                if ( family != null )
                {
                    family.Selected = true;
                    ProcessSelection();
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

        /// <summary>
        /// Special handling instead of the normal, default GoBack() behavior.
        /// </summary>
        protected override void GoBack()
        {
            if ( CurrentCheckInState != null && CurrentCheckInState.CheckIn != null )
            {
                CurrentCheckInState.CheckIn.SearchType = null;
                CurrentCheckInState.CheckIn.SearchValue = string.Empty;
                CurrentCheckInState.CheckIn.Families = new List<CheckInFamily>();
            }

            SaveState();

            if ( CurrentCheckInState.CheckIn.UserEnteredSearch )
            {
                NavigateToPreviousPage();
            }
            else
            {
                NavigateToHomePage();
            }
        }

        private void ProcessSelection()
        {
            if ( !ProcessSelection( maWarning, () => 
                CurrentCheckInState.CheckIn.Families.All( f => f.People.Count == 0 ),
                !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "<p>Sorry, no one in your family is eligible to check-in at this location.</p>": "<p>Lo sentimos, nadie en su familia es elegible para registrarse en este lugar.</p>" ) )            
            {
                ClearSelection();
            }
        }

        protected override void NavigateToNextPage( Dictionary<string, string> queryParams, bool validateSelectionRequired )
        {
            string pageAttributeKey = "NextPage";
            if ( CurrentCheckInType != null &&
                CurrentCheckInType.TypeOfCheckin == TypeOfCheckin.Family &&
                !string.IsNullOrWhiteSpace( LinkedPageUrl( "FamilyNextPage" ) ) )
            {
                pageAttributeKey = "FamilyNextPage";
            }

            queryParams = CheckForOverride( queryParams );
            NavigateToLinkedPage( pageAttributeKey, queryParams );
        }
    }
}