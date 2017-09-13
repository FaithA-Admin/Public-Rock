﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Web.UI.Controls;
using Rock.Web.Cache;
using System.Web.UI;
using org.secc.FamilyCheckin.Model;
using org.secc.FamilyCheckin.Exceptions;
using Rock.Data;

namespace RockWeb.Plugins.org_secc.FamilyCheckin
{
    [DisplayName( "QuickSearch" )]
    [Category( "SECC > Check-in" )]
    [Description( "QuickSearch block for helping parents find their family quickly." )]
    [IntegerField( "Minimum Phone Number Length", "Minimum length for phone number searches (defaults to 4).", false, 4 )]
    [IntegerField( "Maximum Phone Number Length", "Maximum length for phone number searches (defaults to 10).", false, 10 )]
    [IntegerField( "Refresh Interval", "How often (seconds) should page automatically query server for new Check-in data", false, 10 )]
    [TextField( "Search Regex", "Regular Expression to run the search input through before sending it to the workflow. Useful for stripping off characters.", false )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.CHECKIN_SEARCH_TYPE, "Search Type", "The type of search to use for check-in (default is phone number).", true, false, Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_PHONE_NUMBER, order: 4 )]
    [CodeEditorField( "Default Content", "Default content to display", CodeEditorMode.Html, CodeEditorTheme.Rock, 200, true, "", "", 12 )]
    public partial class QuickSearch : CheckInBlock
    {

        protected int minLength;
        protected int maxLength;
        protected KioskType KioskType;


        protected override void OnInit( EventArgs e )
        {

            base.OnInit( e );

            if ( CurrentCheckInState == null )
            {
                LogException( new CheckInStateLost( "Lost check-in state on init" ) );
                NavigateToPreviousPage();
                return;
            }

            if ( Session["KioskTypeId"] != null )
            {
                int kioskTypeId = ( int ) Session["KioskTypeId"];
                KioskType = new KioskTypeService( new RockContext() ).Get( kioskTypeId );
                if ( KioskType == null )
                {
                    NavigateToPreviousPage();
                    return;
                }
            }
            else
            {
                NavigateToPreviousPage();
                return;
            }

            RockPage.AddScriptLink( "~/scripts/jquery.plugin.min.js" );
            RockPage.AddScriptLink( "~/scripts/jquery.countdown.min.js" );

            RegisterScript();
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            minLength = int.Parse( GetAttributeValue( "MinimumPhoneNumberLength" ) );
            maxLength = int.Parse( GetAttributeValue( "MaximumPhoneNumberLength" ) );

            if ( Request["__EVENTTARGET"] == "ChooseFamily" )
            {
                ChooseFamily( Request["__EVENTARGUMENT"] );
            }

            if ( !Page.IsPostBack && CurrentCheckInState != null )
            {
                CurrentWorkflow = null;
                CurrentCheckInState.CheckIn = new CheckInStatus();
                SaveState();
                Session["BlockGuid"] = BlockCache.Guid;
                RefreshView();
                if ( Session["KioskMessage"] != null && !string.IsNullOrWhiteSpace( ( string ) Session["KioskMessage"] ) )
                {
                    ltContent.Text = ( string ) Session["KioskMessage"];
                }
                else
                {
                    ltContent.Text = GetAttributeValue( "DefaultContent" );
                }
            }
        }

        private void ChooseFamily( string familyIdAsString )
        {
            int familyId = Int32.Parse( familyIdAsString );
            CurrentCheckInState = ( CheckInState ) Session["CheckInState"];
            ClearSelection();
            CheckInFamily selectedFamily = CurrentCheckInState.CheckIn.Families.FirstOrDefault( f => f.Group.Id == familyId );
            if ( selectedFamily != null )
            {
                try
                {
                    //clear QCPeople session object and get it ready for quick checkin.
                    Session.Remove( "qcPeople" );
                }
                catch { }
                selectedFamily.Selected = true;
                SaveState();
                NavigateToNextPage();
            }
        }


        private void ClearSelection()
        {
            foreach ( var family in CurrentCheckInState.CheckIn.Families )
            {
                family.Selected = false;
                family.People = new List<CheckInPerson>();
            }
        }

        /// <summary>
        /// Handles the Click event of the lbRefresh control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbRefresh_Click( object sender, EventArgs e )
        {
            RefreshView();
        }


        /// <summary>
        /// Refreshes the view.
        /// </summary>
        private void RefreshView()
        {
            hfRefreshTimerSeconds.Value = GetAttributeValue( "RefreshInterval" );
            pnlNotActive.Visible = false;
            pnlNotActiveYet.Visible = false;
            pnlClosed.Visible = false;
            pnlActive.Visible = false;
            ManagerLoggedIn = false;
            lblActiveWhen.Text = string.Empty;

            if ( CurrentCheckInState == null )
            {
                LogException( new CheckInStateLost( "Lost check-in state on refresh view" ) );
                NavigateToPreviousPage();
                return;
            }


            if ( !KioskType.IsOpen() )
            {
                DateTime? activeAt = KioskType.GetNextOpen();
                if ( activeAt == null )
                {
                    pnlNotActive.Visible = true;
                    HideSign();
                }
                else
                {
                    lblActiveWhen.Text = ( activeAt ?? RockDateTime.Today.AddDays( 1 ) ).ToString( "o" );
                    pnlNotActiveYet.Visible = true;
                    HideSign();
                }
                ScriptManager.RegisterStartupScript( Page, Page.GetType(), "Set Kiosk Active", "kioskActive=false;", true );
            }
            else if ( CurrentCheckInState.Kiosk.FilteredGroupTypes( CurrentCheckInState.ConfiguredGroupTypes ).Count == 0 )
            {
                pnlNotActive.Visible = true;
                HideSign();
                ScriptManager.RegisterStartupScript( Page, Page.GetType(), "Set Kiosk Active", "kioskActive=false;", true );
            }
            else if ( !CurrentCheckInState.Kiosk.HasLocations( CurrentCheckInState.ConfiguredGroupTypes ) )
            {
                DateTime activeAt = CurrentCheckInState.Kiosk.FilteredGroupTypes( CurrentCheckInState.ConfiguredGroupTypes ).Select( g => g.NextActiveTime ).Min();
                lblActiveWhen.Text = activeAt.ToString( "o" );
                pnlNotActiveYet.Visible = true;
                HideSign();
                ScriptManager.RegisterStartupScript( Page, Page.GetType(), "Set Kiosk Active", "kioskActive=false;", true );
            }
            else
            {
                pnlActive.Visible = true;
                ShowWelcomeSign();
                ScriptManager.RegisterStartupScript( Page, Page.GetType(), "Set Kiosk Active", "kioskActive=true;", true );
            }
        }

        private void HideSign()
        {
            ScriptManager.RegisterStartupScript( Page, Page.GetType(), "HideSign", "hideSign();", true );
        }

        private void ShowWelcomeSign()
        {
            ScriptManager.RegisterStartupScript( Page, Page.GetType(), "ShowWelcomeSign", "showWelcome();", true );
        }

        /// <summary>
        /// Registers the script.
        /// </summary>
        private void RegisterScript()
        {
            // Note: the OnExpiry property of the countdown jquery plugin seems to add a new callback
            // everytime the setting is set which is why the clearCountdown method is used to prevent 
            // a plethora of partial postbacks occurring when the countdown expires.
            string script = string.Format( @"
var timeoutSeconds = $('.js-refresh-timer-seconds').val();
if (timeout) {{
    window.clearTimeout(timeout);
}}
var timeout = window.setTimeout(function(){{checkStatus( {1} )}}, timeoutSeconds * 1000);

var $ActiveWhen = $('.active-when');
var $CountdownTimer = $('.countdown-timer');

function refreshKiosk() {{
    window.clearTimeout(timeout);
    var input = $('input[id$= \'tbPhone\']').get(0);
    if (input) {{
        if (input.value.length<1) {{
            {0};
        }} else {{
            timeout = window.setTimeout(function(){{checkStatus( {1} )}}, timeoutSeconds * 1000)
        }}
    }} else {{
        {0};
    }}
}}

function clearCountdown() {{
    if ($ActiveWhen.text() != '')
    {{
        $ActiveWhen.text('');
        refreshKiosk();
    }}
}}

if ($ActiveWhen.text() != '')
{{
    var timeActive = new Date($ActiveWhen.text());
    timeActive.setSeconds(timeActive.getSeconds() + 15);
    $CountdownTimer.countdown({{
        until: timeActive, 
        compact:true, 
        onExpiry: clearCountdown
    }});
}}

", this.Page.ClientScript.GetPostBackEventReference( lbRefresh, "" ), KioskType.Id);
            ScriptManager.RegisterStartupScript( Page, Page.GetType(), "RefreshScript", script, true );
        }
    }
}