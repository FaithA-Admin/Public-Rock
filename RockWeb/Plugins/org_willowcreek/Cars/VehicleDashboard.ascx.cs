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
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using org.willowcreek.Cars.Model;
using Rock;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_willowcreek.Cars
{
    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Vehicle Dashboard" )]
    [Category( "org_willowcreek > Cars" )]
    [Description( "Dashboard to manage all the Donated Vehicle." )]

    [LinkedPage( "Vehicle Detail Page", "The page for editing a Donated Vehicle", true, "", "", 0 )]
    public partial class VehicleDashboard : Rock.Web.UI.RockBlock
    {
        #region Properties

        /// <summary>
        /// Gets or sets the status type for donation.
        /// </summary>
        /// <value>
        /// The Status Type.
        /// </value>
        protected StatusType _selectedStatus { get; set; }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            string status = ViewState["Selectedtatus"] as string ?? string.Empty;
            _selectedStatus = status.ConvertToEnum<StatusType>( StatusType.Pending );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            fVehicleDashboard.ApplyFilterClick += fVehicleDashboard_ApplyFilterClick;
            fVehicleDashboard.ClearFilterClick += fVehicleDashboard_ClearFilterClick;
            gVehicleDashboard.DataKeyNames = new string[] { "Id" };
            gVehicleDashboard.RowDataBound += gVehicleDashboard_RowDataBound;
            gVehicleDashboard.GridRebind += gVehicleDashboard_GridRebind;
            gVehicleDashboard.ShowConfirmDeleteDialog = true;
            gVehicleDashboard.Actions.ShowAdd = true;
            gVehicleDashboard.Actions.AddClick += Actions_AddClick;


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
                int? tab = PageParameter( "Tab" ).AsIntegerOrNull();
                _selectedStatus = tab.HasValue ? (StatusType)tab.Value : fVehicleDashboard.GetUserPreference( "Status" ).ConvertToEnum<StatusType>( StatusType.Pending );

                BindFilter();
                ShowTab();
            }

        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            ViewState["Selectedtatus"] = _selectedStatus.ConvertToString();

            return base.SaveViewState();
        }


        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            BindGrid();
        }

        #endregion

        #region Events

        #region Main Form Events

        /// <summary>
        /// Handles the Click event of the lbTab control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbTab_Click( object sender, EventArgs e )
        {
            LinkButton lb = sender as LinkButton;
            if ( lb != null )
            {
                _selectedStatus = lb.Text.ConvertToEnum<StatusType>();
                ShowTab();
            }
        }

        #endregion

        #region Status Type Tab Events

        /// <summary>
        /// Handles the ApplyFilterClick event of the fVehicleDashboard_ control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void fVehicleDashboard_ApplyFilterClick( object sender, EventArgs e )
        {
            fVehicleDashboard.SaveUserPreference( "Date Entered", drpVehicleEntered.DelimitedValues );
            fVehicleDashboard.SaveUserPreference( "Date In Inventory", drpVehicleInventory.DelimitedValues );
            fVehicleDashboard.SaveUserPreference( "Date Completed", drpVehicleCompleted.DelimitedValues );
            fVehicleDashboard.SaveUserPreference( "Disposition Type", ddlDispositionType.SelectedValue != All.Id.ToString() ? ddlDispositionType.SelectedValue : string.Empty );
            fVehicleDashboard.SaveUserPreference( "Donor", ppDonor.SelectedValue.ToString() );
            fVehicleDashboard.SaveUserPreference( "Vehicle Year", ypYear.SelectedYear.HasValue ? ypYear.SelectedYear.Value.ToString() : string.Empty );
            fVehicleDashboard.SaveUserPreference( "Vehicle Make", ddlMake.SelectedValue != All.Id.ToString() ? ddlMake.SelectedValue : string.Empty );
            fVehicleDashboard.SaveUserPreference( "Vehicle Model", ddlModel.SelectedValue != All.Id.ToString() ? ddlModel.SelectedValue : string.Empty );
            fVehicleDashboard.SaveUserPreference( "Sub-Status", ddlSubStatus.SelectedValue != All.Id.ToString() ? ddlSubStatus.SelectedValue : string.Empty );
            BindGrid();
        }

        /// <summary>
        /// Handles the ClearFilterClick event of the fVehicleDashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void fVehicleDashboard_ClearFilterClick( object sender, EventArgs e )
        {
            fVehicleDashboard.DeleteUserPreferences();
            BindFilter();
        }

        /// <summary>
        /// Fs the fVehicleDashboard display filter value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        protected void fVehicleDashboard_DisplayFilterValue( object sender, GridFilter.DisplayFilterValueArgs e )
        {
            switch ( e.Key )
            {
                case "Date Entered":
                case "Date In Inventory":
                case "Date Completed":
                    {
                        e.Value = DateRangePicker.FormatDelimitedValues( e.Value );
                        break;
                    }
                case "Disposition Type":
                case "Vehicle Make":
                case "Vehicle Model":
                case "Sub-Status":
                    var dv = DefinedValueCache.Read( e.Value.AsInteger() );
                    e.Value = dv != null ? dv.Value : string.Empty;
                    break;
                case "Donor":
                    var person = new PersonService( new RockContext() ).Get( e.Value.AsInteger() );
                    e.Value = ( person != null ) ? person.FullName : string.Empty;
                    break;
                case "Vehicle Year":
                    {
                        break;
                    }
                default:
                    {
                        e.Value = string.Empty;
                        break;
                    }
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlMake control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ddlMake_SelectedIndexChanged( object sender, EventArgs e )
        {
            BindModel();
        }

        /// <summary>
        /// Handles the GridRebind event of the gVehicleDashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gVehicleDashboard_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        /// <summary>
        /// Handles the RowDataBound event of the gVehicleDashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void gVehicleDashboard_RowDataBound( object sender, GridViewRowEventArgs e )
        {
            var vehicle = e.Row.DataItem as Vehicle;
            if ( vehicle != null )
            {
                var lMakeModel = e.Row.FindControl( "lMakeModel" ) as Literal;
                if ( vehicle.MakeValueId.HasValue && vehicle.ModelValueId.HasValue )
                {
                    var makeValue = DefinedValueCache.Read( vehicle.MakeValueId.Value );
                    var modelValue = DefinedValueCache.Read( vehicle.ModelValueId.Value );
                    lMakeModel.Text = makeValue != null ? makeValue.Value : string.Empty;
                    lMakeModel.Text = string.Format( "{0} {1}", lMakeModel.Text, ( modelValue != null ? modelValue.Value : string.Empty ) );
                }
            }
        }

        private void Actions_AddClick( object sender, EventArgs e )
        {
            NavigateToLinkedPage( "VehicleDetailPage", "VehicleId", 0 );
        }

        /// <summary>
        /// Handles the Delete event of the gVehicleDashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gVehicleDashboard_Delete( object sender, RowEventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                var vehicleService = new VehicleService( rockContext );
                var vehicle = vehicleService.Get( e.RowKeyId );
                if ( vehicle != null )
                {
                    if ( !UserCanEdit )
                    {
                        mdDeleteWarning.Show( "You are not authorized to delete this vehicle.", ModalAlertType.Information );
                        return;
                    }

                    string errorMessage;
                    if ( !vehicleService.CanDelete( vehicle, out errorMessage ) )
                    {
                        mdRegistrationsGridWarning.Show( errorMessage, ModalAlertType.Information );
                        return;
                    }

                    vehicleService.Delete( vehicle );
                    rockContext.SaveChanges();

                }
            }

            BindGrid();
        }

        /// <summary>
        /// Handles the RowSelected event of the gVehicleDashboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gVehicleDashboard_RowSelected( object sender, RowEventArgs e )
        {
            NavigateToLinkedPage( "VehicleDetailPage", "VehicleId", e.RowKeyId );
        }

        #endregion

        #endregion

        #region Methods

        #region Main Form Methods

        /// <summary>
        /// Shows the tab.
        /// </summary>
        private void ShowTab()
        {
            liPending.RemoveCssClass( "active" );
            liInvenory.RemoveCssClass( "active" );
            liComplete.RemoveCssClass( "active" );

            switch ( _selectedStatus )
            {
                case StatusType.Pending:
                    {
                        liPending.AddCssClass( "active" );
                        break;
                    }

                case StatusType.Inventory:
                    {
                        liInvenory.AddCssClass( "active" );
                        break;
                    }

                case StatusType.Complete:
                    {
                        liComplete.AddCssClass( "active" );
                        break;
                    }
            }

            fVehicleDashboard.SaveUserPreference( "Status", _selectedStatus.ConvertToString() );

            BindGrid();
        }

        #endregion

        #region Vehicle Tab

        /// <summary>
        /// Binds the vehicles filter.
        /// </summary>
        private void BindFilter()
        {
            drpVehicleEntered.DelimitedValues = fVehicleDashboard.GetUserPreference( "Date Entered" );
            drpVehicleInventory.DelimitedValues = fVehicleDashboard.GetUserPreference( "Date In Inventory" );
            drpVehicleCompleted.DelimitedValues = fVehicleDashboard.GetUserPreference( "Date Completed" );
            BindDefinedTypeDropdown( ddlDispositionType, new Guid( org.willowcreek.Cars.SystemGuid.DefinedType.DISPOSITION_TYPE ), "Disposition Type" );
            ypYear.SelectedYear = fVehicleDashboard.GetUserPreference( "Vehicle Year" ).AsIntegerOrNull();
            BindDefinedTypeDropdown( ddlMake, new Guid( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_MAKE ), "Vehicle Make" );

            BindModel();
            ddlModel.SetValue( fVehicleDashboard.GetUserPreference( "Vehicle Model" ).AsIntegerOrNull() );

            BindDefinedTypeDropdown( ddlSubStatus, new Guid( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_SUB_STATUS ), "Sub-Status" );

            var personId = fVehicleDashboard.GetUserPreference( "Donor" ).AsIntegerOrNull();
            if ( personId.HasValue )
            {
                var person = new PersonService( new RockContext() ).Get( personId.Value );
                ppDonor.SetValue( person );
            }
            else
            {
                ppDonor.SetValue( null );
            }

        }

        private void BindModel()
        {
            ddlModel.Items.Clear();
            ddlModel.Items.Add( new ListItem() );

            int? makeId = ddlMake.SelectedValueAsInt();
            if ( makeId.HasValue )
            {
                var dvMake = DefinedValueCache.Read( makeId.Value );
                var dtModel = DefinedTypeCache.Read( org.willowcreek.Cars.SystemGuid.DefinedType.VEHICLE_MODEL.AsGuid() );
                if ( dvMake != null && dtModel != null )
                {
                    foreach ( var dvModel in dtModel.DefinedValues )
                    {
                        if ( dvModel.GetAttributeValue( "VehicleMake" ).AsGuid().Equals( dvMake.Guid ) )
                        {
                            ddlModel.Items.Add( new ListItem( dvModel.Value, dvModel.Id.ToString() ) );
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Binds the vehicles grid.
        /// </summary>
        private void BindGrid()
        {

            using ( var rockContext = new RockContext() )
            {
                VehicleService vehicleService = new VehicleService( rockContext );
                var query = vehicleService.Queryable();
                query = query.Where( p => p.Status == _selectedStatus );

                DateRangePicker enteredRangePicker = new DateRangePicker();
                enteredRangePicker.DelimitedValues = fVehicleDashboard.GetUserPreference( "Date Entered" );
                if ( enteredRangePicker.LowerValue.HasValue )
                {
                    query = query.Where( r =>
                        r.DateEntered >= enteredRangePicker.LowerValue.Value );
                }
                if ( enteredRangePicker.UpperValue.HasValue )
                {
                    query = query.Where( r =>
                        r.DateEntered <= enteredRangePicker.UpperValue.Value );
                }

                DateRangePicker inventoryRangePicker = new DateRangePicker();
                inventoryRangePicker.DelimitedValues = fVehicleDashboard.GetUserPreference( "Date In Inventory" );
                if ( inventoryRangePicker.LowerValue.HasValue )
                {
                    query = query.Where( r =>
                        r.DateInInventory.HasValue && r.DateInInventory >= inventoryRangePicker.LowerValue.Value );
                }
                if ( inventoryRangePicker.UpperValue.HasValue )
                {
                    query = query.Where( r =>
                        r.DateInInventory.HasValue && r.DateInInventory >= inventoryRangePicker.UpperValue.Value );
                }

                DateRangePicker completedRangePicker = new DateRangePicker();
                completedRangePicker.DelimitedValues = fVehicleDashboard.GetUserPreference( "Date Completed" );
                if ( completedRangePicker.LowerValue.HasValue )
                {
                    query = query.Where( r =>
                        r.DateCompleted.HasValue && r.DateCompleted >= completedRangePicker.LowerValue.Value );
                }
                if ( completedRangePicker.UpperValue.HasValue )
                {
                    query = query.Where( r =>
                        r.DateCompleted.HasValue && r.DateCompleted >= completedRangePicker.UpperValue.Value );
                }

                int? dispositionTypeId = fVehicleDashboard.GetUserPreference( "Disposition Type" ).AsIntegerOrNull();
                if ( dispositionTypeId.HasValue )
                {
                    query = query.Where( t => t.DispositionTypeId == dispositionTypeId.Value );
                }

                int? carYear = fVehicleDashboard.GetUserPreference( "Vehicle Year" ).AsIntegerOrNull();
                if ( carYear.HasValue )
                {
                    query = query.Where( t => t.Year == carYear.Value );
                }

                int? vehicleMakeId = fVehicleDashboard.GetUserPreference( "Vehicle Make" ).AsIntegerOrNull();
                if ( vehicleMakeId.HasValue )
                {
                    query = query.Where( t => t.MakeValueId == vehicleMakeId.Value );
                }

                int? vehicleModelId = fVehicleDashboard.GetUserPreference( "Vehicle Model" ).AsIntegerOrNull();
                if ( vehicleModelId.HasValue )
                {
                    query = query.Where( t => t.ModelValueId == vehicleModelId.Value );
                }

                int? subStatus = fVehicleDashboard.GetUserPreference( "Sub-Status" ).AsIntegerOrNull();
                if ( subStatus.HasValue )
                {
                    query = query.Where( t => t.SubStatusValueId == subStatus.Value );
                }

                var filterPersonId = fVehicleDashboard.GetUserPreference( "Donor" ).AsIntegerOrNull();
                if ( filterPersonId.HasValue )
                {
                    query = query.Where( t => t.DonorPersonAlias != null && t.DonorPersonAlias.PersonId == filterPersonId.Value );
                }

                SortProperty sortProperty = gVehicleDashboard.SortProperty;
                if ( sortProperty != null )
                {
                    query = query.Sort( sortProperty ).AsQueryable();
                }
                else
                {
                    query = query.OrderByDescending( acc => acc.DateEntered );
                }

                gVehicleDashboard.SetLinqDataSource( query.AsNoTracking() );
                gVehicleDashboard.DataBind();
            }
        }


        /// <summary>
        /// Binds the defined type dropdown.
        /// </summary>
        /// <param name="ListControl">The list control.</param>
        /// <param name="definedTypeGuid">The defined type GUID.</param>
        /// <param name="userPreferenceKey">The user preference key.</param>
        private void BindDefinedTypeDropdown( ListControl listControl, Guid definedTypeGuid, string userPreferenceKey )
        {
            listControl.BindToDefinedType( DefinedTypeCache.Read( definedTypeGuid ) );
            listControl.Items.Insert( 0, new ListItem( string.Empty, string.Empty ) );

            if ( !string.IsNullOrWhiteSpace( fVehicleDashboard.GetUserPreference( userPreferenceKey ) ) )
            {
                listControl.SelectedValue = fVehicleDashboard.GetUserPreference( userPreferenceKey );
            }
        }

        #endregion

        #endregion

    }
}