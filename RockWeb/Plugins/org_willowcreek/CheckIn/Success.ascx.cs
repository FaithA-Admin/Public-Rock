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
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Web.UI;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Data;
using org.willowcreek.SystemGuid;

namespace RockWeb.Plugins.org_willowcreek.CheckIn
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName( "Success" )]
    [Category( "org_willowcreek > Check-in" )]
    [Description( "Displays the details of a successful checkin." )]
    [LinkedPage( "Person Select Page", "", false, "", "", 5 )]
    [BooleanField( "Translate Block Into Spanish", "", false )]
    public partial class Success : CheckInBlock
    {
        private int _maxMillisecondsWaitTimeToConnectToCheckinPrinter;
        private int _numberofCheckinLabelPrintRetries;
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            RockPage.AddScriptLink( "~/Scripts/CheckinClient/cordova-2.4.0.js", false );
            RockPage.AddScriptLink( "~/Scripts/CheckinClient/ZebraPrint.js" );

            RockPage.AddScriptLink( "~/Scripts/iscroll.js" );
            RockPage.AddScriptLink( "~/Scripts/CheckinClient/checkin-core.js" );

            var bodyTag = this.Page.Master.FindControl( "bodyTag" ) as HtmlGenericControl;
            if ( bodyTag != null )
            {
                bodyTag.AddCssClass( "checkin-success-bg" );
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            lbDone.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Done" : "Listo";
            lbAnother.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Another Person" : "Otra Persona";
            lHeader.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Checked-in" : "Check-in Exitoso";

            if ( CurrentWorkflow == null || CurrentCheckInState == null )
            {
                NavigateToHomePage();
            }
            else
            {
                if ( !Page.IsPostBack )
                {
                    //Read check-in print global attributes directly to avoid the need of reloading the global attribute cache in case the settings are changed on the fly.
                    using ( var rockContext = new RockContext() )
                    {
                        var maxWaitTimeAttributeId = new AttributeService( rockContext ).Queryable().Where( a => a.Guid.ToString() == AttributeGuids.MAX_MILLISECONDS_WAIT_TIME_TO_CONNECT_TO_CHECKIN_PRINTER).Select(a=> a.Id).FirstOrDefault();
                        _maxMillisecondsWaitTimeToConnectToCheckinPrinter = new AttributeValueService( rockContext ).Queryable().Where( av => av.AttributeId == maxWaitTimeAttributeId ).Select( av => av.Value ).FirstOrDefault().AsInteger();
                        var numberOfPrintRetriesAttributeId = new AttributeService( rockContext ).Queryable().Where( a => a.Guid.ToString() == AttributeGuids.NUMBER_OF_CHECKIN_LABEL_PRINT_RETRIES ).Select( a => a.Id ).FirstOrDefault();
                        _numberofCheckinLabelPrintRetries = new AttributeValueService( rockContext ).Queryable().Where( av => av.AttributeId == numberOfPrintRetriesAttributeId ).Select( av => av.Value ).FirstOrDefault().AsInteger();
                    }                   

                    printCheckinLabels();
                }
            }
        }

        /// <summary>
        /// Print Checkin labels
        /// </summary>
        private void printCheckinLabels()
        {
            try
            {
                var printFromClient = new List<CheckInLabel>();
                var printFromServer = new List<CheckInLabel>();

                // Print the labels
                foreach ( var family in CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ) )
                {
                    lbAnother.Visible =
                        CurrentCheckInState.CheckInType.TypeOfCheckin == TypeOfCheckin.Individual &&
                        family.People.Count > 1;

                    foreach ( var person in family.GetPeople( true ) )
                    {
                        foreach ( var groupType in person.GetGroupTypes( true ) )
                        {
                            foreach ( var group in groupType.GetGroups( true ) )
                            {
                                foreach ( var location in group.GetLocations( true ) )
                                {
                                    foreach ( var schedule in location.GetSchedules( true ) )
                                    {
                                        string personText = person.ToString().EncodeHtml();
                                        string groupText = "<b><font size=\"+3\">" + group.ToString().EncodeHtml() + "</font></b>";
                                        string locationText = location.ToString().EncodeHtml();
                                        string scheduleText = schedule.ToString().EncodeHtml();
                                        string personSecurityCodeText = person.SecurityCode.ToString().EncodeHtml();

                                        var li = new HtmlGenericControl( "li" );
                                        li.InnerHtml = string.Format( !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "{0} was checked into {1} in {2} at {3}" : "{0} fue registrado(a) en {1} en {2} en {3}",
                                           personText, groupText, locationText, scheduleText, personSecurityCodeText );

                                        phResults.Controls.Add( li );
                                    }
                                }
                            }

                            if ( groupType.Labels != null && groupType.Labels.Any() )
                            {
                                printFromClient.AddRange( groupType.Labels.Where( l => l.PrintFrom == Rock.Model.PrintFrom.Client ) );
                                printFromServer.AddRange( groupType.Labels.Where( l => l.PrintFrom == Rock.Model.PrintFrom.Server ) );
                            }
                        }
                    }
                }

                if ( printFromClient.Any() )
                {
                    var urlRoot = string.Format( "{0}://{1}", Request.Url.Scheme, Request.Url.Authority );
                    printFromClient.OrderBy( l => l.Order ).ToList().ForEach( l => l.LabelFile = urlRoot + l.LabelFile );
                    AddLabelScript( printFromClient.ToJson() );
                }

                if ( printFromServer.Any() )
                {
                    Socket socket = null;
                    string currentIporHostname = string.Empty;

                    foreach ( var label in printFromServer.OrderBy( l => l.Order ) )
                    {
                        var labelCache = KioskLabel.Read( label.FileGuid );
                        if ( labelCache != null )
                        {
                            if ( !string.IsNullOrWhiteSpace( label.PrinterAddress ) )
                            {
                                string printContent = labelCache.FileContent;
                                foreach ( var mergeField in label.MergeFields )
                                {
                                    if ( !string.IsNullOrWhiteSpace( mergeField.Value ) )
                                    {
                                        printContent = Regex.Replace( printContent, string.Format( @"(?<=\^FD){0}(?=\^FS)", mergeField.Key ), ZebraFormatString( mergeField.Value ) );
                                    }
                                    else
                                    {
                                        // Remove the box preceding merge field
                                        printContent = Regex.Replace( printContent, string.Format( @"\^FO.*\^FS\s*(?=\^FT.*\^FD{0}\^FS)", mergeField.Key ), string.Empty );
                                        // Remove the merge field
                                        printContent = Regex.Replace( printContent, string.Format( @"\^FD{0}\^FS", mergeField.Key ), "^FD^FS" );
                                    }
                                }

                                if ( label.PrinterAddress != currentIporHostname )
                                {
                                    currentIporHostname = label.PrinterAddress;
                                }

                                bool successfulPrint = false;
                                int numberofRetries = _numberofCheckinLabelPrintRetries;
                                while ( !successfulPrint )
                                {
                                    try
                                    {
                                        connectToPrinter( ref socket, ref currentIporHostname );
                                        if ( socket.Connected )
                                        {
                                            printLabel( socket, printContent );
                                            successfulPrint = true;
                                        }
                                        else
                                        {
                                            throw new Exception( "Could not connect following printer: " + currentIporHostname );
                                        }
                                    }
                                    catch ( Exception ex )
                                    {
                                        ExceptionLogService.LogException( ex, Context, this.RockPage.PageId, this.RockPage.Site.Id );
                                        if ( numberofRetries == 0 )
                                        {
                                            phResults.Controls.Add( new LiteralControl( "<br/>NOTE: Could not connect to printer!" + (GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? " (Impresora no disponible!)" : string.Empty ) ) );
                                            break;
                                        }
                                        else
                                        {
                                            numberofRetries--;
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if ( socketConnected( ref socket ) )
                    {
                        closeSocket( ref socket );
                    }
                }

            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex, Context, this.RockPage.PageId, this.RockPage.Site.Id );
            }
        }

        /// <summary>
        /// Print label
        /// </summary>
        /// <param name="socket">The socket</param>
        /// <param name="printContent">The print content</param>
        private static void printLabel( Socket socket, string printContent )
        {
            var ns = new NetworkStream( socket );
            byte[] toSend = System.Text.Encoding.ASCII.GetBytes( printContent );
            ns.Write( toSend, 0, toSend.Length );
        }

        /// <summary>
        /// Connects to printer
        /// </summary>
        /// <param name="socket">The socket</param>
        /// <param name="currentIporHostname">The printer host name or IP address</param>
        /// <param name="label">The label to print</param>
        private void connectToPrinter( ref Socket socket, ref string currentIporHostname )
        {
            if ( socketConnected( ref socket ) )
            {
                closeSocket( ref socket );
            }


            IPAddress ipAddress;
            IPEndPoint printerIp;
            if ( IPAddress.TryParse( currentIporHostname, out ipAddress ) )
            {
                printerIp = new IPEndPoint( ipAddress, 9100 );
            }
            else
            {
                IPAddress[] ips;
                try
                {
                    ips = Dns.GetHostAddresses( currentIporHostname );
                    printerIp = new IPEndPoint( IPAddress.Parse( ips[0].ToString() ), 9100 );
                }
                catch ( Exception ex )
                {
                    throw new Exception( "Could not resolve the IP address of the following printer: " + currentIporHostname, ex );
                }
            }

            socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            IAsyncResult result = socket.BeginConnect( printerIp, null, null );
            bool success = result.AsyncWaitHandle.WaitOne( _maxMillisecondsWaitTimeToConnectToCheckinPrinter, true );
        }

        /// <summary>
        /// Checks if the socket is connected
        /// </summary>
        /// <param name="socket"></param>
        /// <returns>Returns True if socket is connected otherwise false.</returns>
        private bool socketConnected( ref Socket socket )
        {
            if ( socket != null && socket.Connected )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Closes an active socket.
        /// </summary>
        /// <param name="socket"></param>
        private void closeSocket( ref Socket socket )
        {
            socket.Shutdown( SocketShutdown.Both );
            socket.Close();
        }

        /// <summary>
        /// Handles the Click event of the lbDone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void lbDone_Click( object sender, EventArgs e )
        {
            NavigateToHomePage();
        }

        private string ZebraFormatString( string input, bool isJson = false )
        {
            if ( isJson )
            {
                return input
                    .Replace( "é", @"\\82" )
                    .Replace( "ü", @"\\81" )
                    .Replace( "á", @"\\a0" )
                    .Replace( "í", @"\\a1" )
                    .Replace( "ó", @"\\a2" )
                    .Replace( "ú", @"\\a3" )
                    .Replace( "¿", @"\\a8" )
                    .Replace( "¡", @"\\ad" )
                    .Replace( "ñ", @"\\a4").Replace( "Ñ", @"\\a5");
            }
            else
            {
                return input
                    .Replace( "é", @"\82" )
                    .Replace( "ü", @"\81" )
                    .Replace( "á", @"\a0" )
                    .Replace( "í", @"\a1" )
                    .Replace( "ó", @"\a2" )
                    .Replace( "ú", @"\a3" )
                    .Replace( "¿", @"\a8" )
                    .Replace( "¡", @"\ad" )
                    .Replace( "ñ", @"\a4" ).Replace( "Ñ", @"\a5" );
            }
        }

        /// <summary>
        /// Handles the Click event of the lbAnother control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void lbAnother_Click( object sender, EventArgs e )
        {
            if ( KioskCurrentlyActive )
            {
                foreach ( var family in CurrentCheckInState.CheckIn.Families.Where( f => f.Selected ) )
                {
                    foreach ( var person in family.People.Where( p => p.Selected ) )
                    {
                        person.Selected = false;

                        foreach ( var groupType in person.GroupTypes.Where( g => g.Selected ) )
                        {
                            groupType.Selected = false;
                        }
                    }
                }

                SaveState();
                NavigateToLinkedPage( "PersonSelectPage" );

            }
            else
            {
                NavigateToHomePage();
            }
        }

        /// <summary>
        /// Adds the label script.
        /// </summary>
        /// <param name="jsonObject">The json object.</param>
        private void AddLabelScript( string jsonObject )
        {
            string script = string.Format( @"

        // setup deviceready event to wait for cordova
	    if (navigator.userAgent.match(/(iPhone|iPod|iPad)/)) {{
            document.addEventListener('deviceready', onDeviceReady, false);
        }} else {{
            $( document ).ready(function() {{
                onDeviceReady();
            }});
        }}

	    // label data
        var labelData = {0};

		function onDeviceReady() {{
			try {{			
                printLabels();
            }} 
            catch (err) {{
                console.log('An error occurred printing labels: ' + err);
            }}
		}}
		
		function alertDismissed() {{
		    // do something
		}}
		
		function printLabels() {{
		    ZebraPrintPlugin.printTags(
            	JSON.stringify(labelData), 
            	function(result) {{ 
			        console.log('Tag printed');
			    }},
			    function(error) {{   
				    // error is an array where:
				    // error[0] is the error message
				    // error[1] determines if a re-print is possible (in the case where the JSON is good, but the printer was not connected)
			        console.log('An error occurred: ' + error[0]);
                    navigator.notification.alert(
                        'An error occurred while printing the labels.' + error[0],  // message
                        alertDismissed,         // callback
                        'Error',            // title
                        'Ok'                  // buttonName
                    );
			    }}
            );
	    }}
", ZebraFormatString( jsonObject, true ) );
            ScriptManager.RegisterStartupScript( this, this.GetType(), "addLabelScript", script, true );
        }

    }
}