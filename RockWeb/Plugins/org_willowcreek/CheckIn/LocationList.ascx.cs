using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Web.Cache;
using org.willowcreek.SystemGuid;
using System.Data.Entity;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

using Rock.Security;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using org.willowcreek;
using System.Linq.Dynamic;
using org.willowcreek.CheckIn;
using System.Data;

namespace RockWeb.Plugins.org_willowcreek.CheckIn
{
    [DisplayName( "Location List" )]
    [Category( "org_willowcreek > Check-in" )]
    [Description( "Lists all the locations with their current check-in numbers" )]
    public partial class LocationList : Rock.Web.UI.RockBlock
    {
        private int? _checkInTypeId;

        protected void Page_Load( object sender, EventArgs e )
        {
            _checkInTypeId = PageParameter( "CheckinTypeId" ).AsIntegerOrNull();
            btnEvac.PostBackUrl = "/Evac/" + PageParameter( "CheckinTypeId" );
            if ( _checkInTypeId.HasValue )
            {
                var schedule = ActiveSchedule.GetScheduleParameters(_checkInTypeId.Value, PageParameter( "Now" ) );
                if ( schedule != null )
                {
                    var parameters = new Dictionary<string, object>();
                    parameters.Add( "CheckInTypeId", _checkInTypeId );
                    parameters.Add( "ScheduleId", schedule.Item1 );
                    parameters.Add( "StartDateTime", schedule.Item2 );
                    var dataSource = DbService.GetDataTable( "wcRpt_CurrentCheckInGroupList", System.Data.CommandType.StoredProcedure, parameters );

                    gLocations.DataSource = dataSource;
                    gLocations.DataBind();
                }
            }
        }

        protected void gLocations_RowSelected( object sender, RowEventArgs e )
        {
            if ( e.RowKeyId > 0 )
            {
                var url = "/Room/" + _checkInTypeId + "/" + e.RowKeyId;
                var nowString = PageParameter( "now" );
                if ( !string.IsNullOrEmpty( nowString ) )
                {
                    url += "?now=" + nowString;
                }
                Response.Redirect( url, false );
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}