using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
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
    [DisplayName("Parent Location")]
    [Category( "org_willowcreek > Check-in" )]
    [Description( "Displays a list of locations where a parent will be" )]

    [LinkedPage( "Next Page (Family Check-in)", "", false, "", "", 5, "FamilyNextPage" )]
    [BooleanField( "Translate Block Into Spanish", "", false )]
    public partial class ParentLocation : CheckInBlock
    {
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            RockPage.AddScriptLink( "~/Scripts/iscroll.js" );
            RockPage.AddScriptLink( "~/Scripts/CheckinClient/checkin-core.js" );

            lHeader.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Families" : "Familias";
            lSelectLocation.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Parent Location During Service?" : "Ubicación Del Padre Durante El Servicio";
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

                    var locations = GetLocations();

                    if ( locations.Any() )
                    {
                        rSelection.DataSource = locations;
                        rSelection.DataBind();
                    }
                    else
                    {
                        ProcessSelection();
                    }
                }
            }
        }

        protected class LocationOption
        {
            public string DisplayLocation;
            public string LocationToStore;
        }

        private List<LocationOption> GetLocations()
        {
            var groupType = new GroupTypeService( new Rock.Data.RockContext() ).Get( CurrentCheckinTypeId.Value );
            groupType.LoadAttributes();
            var value = groupType.AttributeValues["CheckInConfigParentLocations"].Value;

            var locations = new List<LocationOption>();

            if ( !string.IsNullOrWhiteSpace( value ) )
            {
                var locationStrings = value.Split( ',' ).Select( l => l.Trim('^', ' ') ).Where( l => l.Length > 0 ).ToList();

                foreach ( var location in locationStrings )
                {
                    if ( location.Contains( "^" ) )
                    {
                        var split = location.Split( '^' );
                        locations.Add( new LocationOption { DisplayLocation = split[0], LocationToStore = split[1] } );
                    }
                    else
                    {
                        locations.Add( new LocationOption { DisplayLocation = location, LocationToStore = location } );
                    }
                }
            }

            return locations;
        }

        private void ClearSelection()
        {
        }

        protected void rSelection_ItemCommand( object source, RepeaterCommandEventArgs e )
        {
            if ( KioskCurrentlyActive )
            {
                var parentLocation = e.CommandArgument.ToString();
                CurrentCheckInState.CheckIn.CustomValues.AddOrReplace("ParentLocation", parentLocation);
                ProcessSelection();
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
        //protected override void GoBack()
        //{
        //    if ( CurrentCheckInState != null && CurrentCheckInState.CheckIn != null )
        //    {
        //        CurrentCheckInState.CheckIn.SearchType = null;
        //        CurrentCheckInState.CheckIn.SearchValue = string.Empty;
        //        CurrentCheckInState.CheckIn.Families = new List<CheckInFamily>();
        //    }

        //    SaveState();

        //    if ( CurrentCheckInState.CheckIn.UserEnteredSearch )
        //    {
        //        NavigateToPreviousPage();
        //    }
        //    else
        //    {
        //        NavigateToHomePage();
        //    }
        //}

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