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
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.UI.HtmlControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Web.Cache;
using org.willowcreek.SystemGuid;

namespace RockWeb.Plugins.org_willowcreek.CheckIn
{
    [DisplayName("Search")]
    [Category("org_willowcreek > Check-in")]
    [Description("Displays Willow Creek keypad for searching on phone numbers, names and barcodes.")]
    [BooleanField( "Translate Block Into Spanish", "", false)]
    public partial class Search : CheckInBlock
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RockPage.AddScriptLink("~/Scripts/iscroll.js");
            RockPage.AddScriptLink("~/Scripts/CheckinClient/checkin-core.js");

            if (!KioskCurrentlyActive)
            {
                NavigateToHomePage();
            }

            var bodyTag = this.Page.Master.FindControl("bodyTag") as HtmlGenericControl;
            if (bodyTag != null)
            {
                bodyTag.AddCssClass("checkin-search-bg");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack && CurrentCheckInState != null)
            {

                lbBack.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Back" : "Regresar";
                lbSearch.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Search" : "Buscar";
                lClickTheTextbox.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "*Click the textbox to show the keyboard." : "*Favor de hacer click en la casilla para mostrar el teclado.";
                lKeypadBack.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Back" : "Atrás";
                lKeypadClear.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Clear" : "Borrar";

                this.Page.Form.DefaultButton = lbSearch.UniqueID;
                var hybridCheckInDeviceTypeId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.DEVICE_TYPE_HYBRID_CHECKIN).Id;
                if (CurrentCheckInState.Kiosk.Device.DeviceTypeValueId == hybridCheckInDeviceTypeId)
                {
                    lPageTitle.Text =  !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Search By Name or Phone" : "Búsqueda por Nombre o Teléfono";
                    pnlPhoneKeypad.Visible = true;
                    barcodeCamera.Visible = false;
                    search.TextMode = System.Web.UI.WebControls.TextBoxMode.Search;
                    searchType.Text = "NameOrPhone";                    
                }
                else
                {
                    bool enableCheckInKeyPad, enableCheckInWebcam;
                    using (var rockContext = new RockContext())
                    {
                        Guid checkInEnableKeypadGuid = AttributeGuids.CHECKIN_ENABLE_KEYPAD.AsGuid();
                        var enableCheckInKeyPadAttributeId = new AttributeService(rockContext).Queryable().Where(k => k.Guid == checkInEnableKeypadGuid).Select(k => k.Id).FirstOrDefault().ToString().AsIntegerOrNull();
                        enableCheckInKeyPad = new AttributeValueService(rockContext).Queryable().Where(i => i.AttributeId == enableCheckInKeyPadAttributeId && i.EntityId == CurrentCheckInState.CheckinTypeId).Select(v => v.Value).FirstOrDefault().ToString().AsBoolean(false);

                        Guid checkInEnableWebcamGuid = AttributeGuids.CHECKIN_ENABLE_WEBCAM.AsGuid();
                        var enableCheckInWebcamAttributeId = new AttributeService(rockContext).Queryable().Where(w => w.Guid == checkInEnableWebcamGuid).Select(w => w.Id).FirstOrDefault().ToString().AsIntegerOrNull();
                        enableCheckInWebcam = new AttributeValueService(rockContext).Queryable().Where(i => i.AttributeId == enableCheckInWebcamAttributeId && i.EntityId == CurrentCheckInState.CheckinTypeId).Select(v => v.Value).FirstOrDefault().ToString().AsBoolean(false);
                    }

                    if ( CurrentCheckInType.SearchType.Guid == Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_NAME_AND_PHONE.AsGuid() )
                    {
                        lPageTitle.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Search By Name or Phone" : "Búsqueda por Nombre o Teléfono";
                        pnlPhoneKeypad.Visible = true && enableCheckInKeyPad;
                        barcodeCamera.Visible = false;
                        search.TextMode = System.Web.UI.WebControls.TextBoxMode.Search;
                        searchType.Text = "NameOrPhone";
                    }
                    else if ( CurrentCheckInType.SearchType.Guid == Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_PHONE_NUMBER.AsGuid() )
                    {
                        lPageTitle.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Search By Phone" : "Búsqueda por Teléfono";
                        pnlPhoneKeypad.Visible = true && enableCheckInKeyPad;
                        barcodeCamera.Visible = false;
                        search.TextMode = System.Web.UI.WebControls.TextBoxMode.Phone;
                        searchType.Text = "Phone";
                    }
                    else if (CurrentCheckInType.SearchType.Guid == Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_NAME.AsGuid())
                    {
                        lPageTitle.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Search By Name" : "Búsqueda por Nombre";
                        pnlPhoneKeypad.Visible = false;
                        barcodeCamera.Visible = false;
                        search.TextMode = System.Web.UI.WebControls.TextBoxMode.Search;
                        searchType.Text = "Name";
                    }
                    else if (CurrentCheckInType.SearchType.Guid == DefinedValueGuids.CHECKIN_SEARCH_TYPE_BARCODE.AsGuid())
                    {
                        lPageTitle.Text = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Scan Card" : "Escanee la Tarjeta";
                        barcodeCamera.Visible = enableCheckInWebcam;
                        pnlPhoneKeypad.Visible = false;
                        search.TextMode = System.Web.UI.WebControls.TextBoxMode.Search;
                        searchType.Text = "Barcode";
                    }
                }                
            }
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            if (KioskCurrentlyActive)
            {
                // check search type
                var hybridCheckInDeviceTypeId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.DEVICE_TYPE_HYBRID_CHECKIN).Id;

                if ((CurrentCheckInType.SearchType.Guid == Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_NAME_AND_PHONE.AsGuid()) || (CurrentCheckInState.Kiosk.Device.DeviceTypeValueId == hybridCheckInDeviceTypeId))
                {
                    // name and phone search (this option uses the name search panel as UI)
                    if (search.Text.Any(c => char.IsLetter(c)))
                    {
                        SearchByName();
                    }
                    else
                    {
                        search.Text = search.Text;
                        SearchByPhone();
                    }
                }
                else if (CurrentCheckInType.SearchType.Guid == Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_PHONE_NUMBER.AsGuid())
                {
                    SearchByPhone();
                }
                else if (CurrentCheckInType.SearchType.Guid == Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_NAME.AsGuid())
                {
                    SearchByName();
                }
                else if (CurrentCheckInType.SearchType.Guid == DefinedValueGuids.CHECKIN_SEARCH_TYPE_BARCODE.AsGuid())
                {
                    SearchByBarcode();
                }

            }
        }

        private void SearchByName()
        {
            if (string.IsNullOrEmpty(search.Text.Trim()))
            {
                maWarning.Show( !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Please enter your family name." : "Por favor ingrese su apellido.", Rock.Web.UI.Controls.ModalAlertType.Warning);
            }
            else
            {
                CurrentCheckInState.CheckIn.UserEnteredSearch = true;
                CurrentCheckInState.CheckIn.ConfirmSingleFamily = true;
                CurrentCheckInState.CheckIn.SearchType = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_NAME);
                CurrentCheckInState.CheckIn.SearchValue = search.Text;
                ProcessSelection();
            }
        }

        private void SearchByPhone()
        {
            var searchTextNumeric = search.Text.AsNumeric().TrimStart('0', '1');


            int minLength = CurrentCheckInType != null ? CurrentCheckInType.MinimumPhoneSearchLength : 4;
            int maxLength = CurrentCheckInType != null ? CurrentCheckInType.MaximumPhoneSearchLength : 10;
            if (searchTextNumeric.Length >= minLength && searchTextNumeric.Length <= maxLength)
            {
                string searchInput = searchTextNumeric;

                // run regex expression on input if provided
                if (CurrentCheckInType != null && !string.IsNullOrWhiteSpace(CurrentCheckInType.RegularExpressionFilter))
                {
                    Regex regex = new Regex(CurrentCheckInType.RegularExpressionFilter);
                    Match match = regex.Match(searchInput);
                    if (match.Success)
                    {
                        if (match.Groups.Count == 2)
                        {
                            searchInput = match.Groups[1].ToString();
                        }
                    }
                }

                CurrentCheckInState.CheckIn.UserEnteredSearch = true;
                CurrentCheckInState.CheckIn.ConfirmSingleFamily = true;
                CurrentCheckInState.CheckIn.SearchType = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.CHECKIN_SEARCH_TYPE_PHONE_NUMBER);
                CurrentCheckInState.CheckIn.SearchValue = searchInput;
                ProcessSelection();
            }
            else
            {
                string errorMsg = (searchTextNumeric.Length > maxLength)
                    ? string.Format( !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "<p>Please enter no more than {0} numbers</p>" : "<p>Por favor no ingrese mas the {0} dígitos</p>", maxLength)
                    : string.Format( !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "<p>Please enter at least {0} numbers</p>" : "<p>Por favor ingrese al menos {0} dígitos</p>", minLength);

                maWarning.Show(errorMsg, Rock.Web.UI.Controls.ModalAlertType.Warning);                
            }
        }

        private void SearchByBarcode()
        {
            if (string.IsNullOrEmpty(search.Text.Trim()))
            {
                string warningMessage = string.Empty;
                if(pnlPhoneKeypad.Visible)
                {
                    warningMessage = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Please type in or scan your card barcode." : "Por favor ingrese o escanee el código de su tarjeta";
                }
                else
                {
                    warningMessage = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Please scan your card barcode." : "Por favor escanee el código de su tarjeta";
                }

                maWarning.Show(warningMessage, Rock.Web.UI.Controls.ModalAlertType.Warning);
            }
            else
            {
                string warningMessage = string.Empty;
                if(pnlPhoneKeypad.Visible)
                {
                    warningMessage = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Please type in or scan your card barcode." : "Por favor ingrese o escanee el código de su tarjeta";
                }
                else
                {
                    warningMessage = !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "Please scan your card barcode." : "Por favor escanee el código de su tarjeta";
                }

                //Display invalid code warning if the card isn't an old PML card (first four characters non numeric) or new Rock check-in card (all numeric) after removing any dashes.
                if ( !( ( search.Text.Trim().GetUntil("-").Length == 4 && search.Text.Trim().GetUntil("-").AsDoubleOrNull() == null ) || ( search.Text.Trim().Contains("-") && search.Text.Trim().Replace("-","").AsDoubleOrNull() != null )) )
                {
                    maWarning.Show( !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "<p>Invalid code.</p><p>" : "<p>Código inválido.</p><p>" + warningMessage + "<p>", Rock.Web.UI.Controls.ModalAlertType.Warning);
                }
                else
                {
                    CurrentCheckInState.CheckIn.UserEnteredSearch = true;
                    CurrentCheckInState.CheckIn.ConfirmSingleFamily = true;
                    CurrentCheckInState.CheckIn.SearchType = DefinedValueCache.Read(DefinedValueGuids.CHECKIN_SEARCH_TYPE_BARCODE);
                    CurrentCheckInState.CheckIn.SearchValue = search.Text;
                    ProcessSelection();
                }
            }
        }

        protected void ProcessSelection()
        {
            ProcessSelection(maWarning, () => CurrentCheckInState.CheckIn.Families.Count <= 0, !GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? "<p>Unable to find any family using this search criteria.</p>" : "<p>No se encontro ninguna familia usando este criterio de búsqueda.</p>");
        }

        protected void lbBack_Click(object sender, EventArgs e)
        {
            CancelCheckin();
        }

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            CancelCheckin();
        }       
    }
}