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
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using System.Data;

namespace RockWeb.Plugins.org_willowcreek.Crm
{
    public enum DonorDuplicateChoices
    {
        HideDonors,
        ShowDonorsOnly,
        ShowAll
    }

    public enum CareCenterDuplicateChoices
    {
        HideCareCenter,
        ShowCareCenterOnly,
        ShowAll
    }

    /// <summary>
    /// List of person records that have possible duplicates
    /// </summary>
    [DisplayName( "Person Duplicate List" )]
    [Category( "org_willowcreek > CRM" )]
    [Description( "List of person records that have possible duplicates" )]

    [DecimalField( "Confidence Score High", "The minimum confidence score required to be considered a likely match", true, 80.00, order: 0 )]
    [DecimalField( "Confidence Score Low", "The maximum confidence score required to be considered an unlikely match. Values lower than this will not be shown in the grid.", true, 60.00, order: 1 )]
    [DecimalField( "Highest Score To Show On Grid", "The maximum confidence score required to be displayed on the grid. Values higher than this will not be shown in the grid.", true, 100, order: 2 )]
    [BooleanField( "Include Inactive", "Set to true to also include potential matches when both records are inactive.", false, order: 3 )]
    [LinkedPage( "Detail Page", order: 4 )]
    [EnumField( "Donor Duplicates", "Should potential duplicates be shown when both people are in a family with contributions?", typeof( DonorDuplicateChoices ), true, "2", order: 5 )]
    [EnumField( "Care Center Duplicates", "Should potential duplicates be shown when at least one record is a Care Center Guest?", typeof( CareCenterDuplicateChoices ), true, "2", order: 6, key: "CareCenterDuplicates" )]
    [CustomEnhancedListField( "Campus Short Code", "Only show potential duplicates that belong to the selected campus. If none is selected this filter won't be applied.", "CDL^Casa de Luz, CHI^Chicago, CLK^Crystal Lake, HNT^Huntley, NSH^North Shore, SBR^South Barrington, SLC^South Lake, DPG^Wheaton", false, "", "", 7 )]
    [CustomEnhancedListField( "Connection Status", "Only show potential duplicates that have the selected connection status. If none is selected this filter won't be applied.", "65^Member, 66^Visitor, 67^Web Prospect, 146^Attendee, 203^Participant, 898^New, 899^Reference, 1449^Care Center Guest, 3248^Promiseland Family, 3243^Promiseland Guest", false, "", "", 8 )]

    public partial class PersonDuplicateList : RockBlock
    {
        /// <summary>
        /// Gets the Confidence Score HTML include bootstrap label
        /// </summary>
        /// <param name="confidenceScore">The confidence score.</param>
        /// <returns></returns>
        public string GetConfidenceScoreColumnHtml( double? confidenceScore )
        {
            string css;

            if ( confidenceScore >= this.GetAttributeValue( "ConfidenceScoreHigh" ).AsDoubleOrNull() )
            {
                css = "label label-success";
            }
            else if ( confidenceScore <= this.GetAttributeValue( "ConfidenceScoreLow" ).AsDoubleOrNull() )
            {
                css = "label label-default";
            }
            else
            {
                css = "label label-warning";
            }

            if ( confidenceScore.HasValue )
            {
                return string.Format( "<span class='{0}'>{1}</span>", css, ( confidenceScore.Value / 100 ).ToString( "P" ) );
            }
            else
            {
                return string.Empty;
            }
        }

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            gList.Actions.ShowAdd = false;
            gList.DataKeyNames = new string[] { "PersonId" };
            gList.GridRebind += gList_GridRebind;
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
                BindGrid();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the GridRebind event of the gList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gList_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        /// <summary>
        /// Handles the RowSelected event of the gList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gList_RowSelected( object sender, RowEventArgs e )
        {
            NavigateToLinkedPage( "DetailPage", "PersonId", e.RowKeyId );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            RockContext rockContext = new RockContext();
            var personDuplicateService = new PersonDuplicateService( rockContext );
            int recordStatusInactiveId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_INACTIVE.AsGuid() ).Id;


            // list duplicates that:
            // - aren't confirmed as NotDuplicate and aren't IgnoreUntilScoreChanges,
            // - don't have the PersonAlias and DuplicatePersonAlias records pointing to the same person ( occurs after two people have been merged but before the Calculate Person Duplicates job runs).
            // - don't include records where both the Person and Duplicate are inactive (block option)
            var dataTable = DbService.GetDataTable( "wcRpt_getPersonDuplicateList", CommandType.Text, null );
            var personDuplicateQry = dataTable.AsEnumerable().Select( d => new
            {
                PersonId = d.Field<int>( "PersonId" ),
                PersonStatusValueId = d.Field<int>( "PersonStatusValueId" ),
                DuplicatePersonId = d.Field<int>( "DuplicatePersonId" ),
                DuplicatePersonStatusValueId = d.Field<int>( "DuplicatePersonStatusValueId" ),
                ConfidenceScore = d.Field<double>( "ConfidenceScore" ),
                AccountingOnly = Convert.ToBoolean( d.Field<int>( "AccountingOnly" ) ),
                CareCenterOnly = Convert.ToBoolean( d.Field<int>( "CareCenterOnly" ) ),
                LastName = d.Field<string>( "LastName" ),
                FirstName = d.Field<string>( "FirstName" ),
                CreatedByPerson = d.Field<string>( "CreatedByPerson" ),
                PersonModifiedDateTime = d.Field<DateTime>( "PersonModifiedDateTime" ),
                Campus = d.Field<string>( "Campus" ),
                CampusShortCode = d.Field<string>( "CampusShortCode"),
                ConnectionStatus = d.Field<string>( "ConnectionStatus" ),
                ConnectionStatusValueId = d.Field<int?>( "ConnectionStatusValueId" ),
                DuplicateConnectionStatusValueId = d.Field<int?>( "DuplicateConnectionStatusValueId" )
            } );

            if ( this.GetAttributeValue( "IncludeInactive" ).AsBoolean() == false )
            {
                personDuplicateQry = personDuplicateQry.Where( a => !( a.PersonStatusValueId == recordStatusInactiveId && a.DuplicatePersonStatusValueId == recordStatusInactiveId ) );
            }

            double? confidenceScoreLow = GetAttributeValue( "ConfidenceScoreLow" ).AsDoubleOrNull();
            if ( confidenceScoreLow.HasValue )
            {
                personDuplicateQry = personDuplicateQry.Where( a => a.ConfidenceScore >= confidenceScoreLow );
            }

            double? highestScoreToShowOnGrid = this.GetAttributeValue( "HighestScoreToShowOnGrid" ).AsDoubleOrNull();
            if ( highestScoreToShowOnGrid.HasValue )
            {
                personDuplicateQry = personDuplicateQry.Where( a => a.ConfidenceScore <= highestScoreToShowOnGrid );
            }

            var donorDuplicates = this.GetAttributeValue( "DonorDuplicates" ).AsInteger();
            if ( donorDuplicates == ( int ) DonorDuplicateChoices.HideDonors )
            {
                personDuplicateQry = personDuplicateQry.Where( a => a.AccountingOnly == false );
            }
            else if ( donorDuplicates == ( int ) DonorDuplicateChoices.ShowDonorsOnly )
            {
                personDuplicateQry = personDuplicateQry.Where( a => a.AccountingOnly == true );
            }

            var careCenterDuplicates = GetAttributeValue( "CareCenterDuplicates" ).AsInteger();
            if ( careCenterDuplicates == ( int ) CareCenterDuplicateChoices.HideCareCenter )
            {
                personDuplicateQry = personDuplicateQry.Where( a => a.CareCenterOnly == false );
            }
            else if ( careCenterDuplicates == ( int ) CareCenterDuplicateChoices.ShowCareCenterOnly )
            {
                personDuplicateQry = personDuplicateQry.Where( a => a.CareCenterOnly == true );
            }

           
            if ( !GetAttributeValue( "CampusShortCode" ).IsNullOrWhiteSpace() )
            {
                var campusShortCode = GetAttributeValue( "CampusShortCode" ).Split( ',' );
                personDuplicateQry = personDuplicateQry.Where( a => campusShortCode.Contains( a.CampusShortCode ) );
            }
            
            if ( !GetAttributeValue( "ConnectionStatus" ).IsNullOrWhiteSpace() )
            {
                var connectionstatus = GetAttributeValue( "ConnectionStatus" ).Split( ',' ).AsIntegerList();
                personDuplicateQry = personDuplicateQry.Where( a =>  connectionstatus.Contains( a.DuplicateConnectionStatusValueId.Value ) );
            }

            var groupByQry = personDuplicateQry.GroupBy( a => new { a.PersonId, a.LastName, a.FirstName, a.CreatedByPerson, a.PersonModifiedDateTime } );

            var qry = groupByQry.Select( a => new
            {
                a.Key.PersonId,
                a.Key.LastName,
                a.Key.FirstName,
                AccountingOnly = Convert.ToBoolean( a.Max( x => x.AccountingOnly ? 1 : 0 ) ),
                CareCenterOnly = Convert.ToBoolean( a.Max( x => x.CareCenterOnly ? 1 : 0 ) ),
                MatchCount = a.Count(),
                MaxConfidenceScore = a.Max( s => s.ConfidenceScore ),
                CreatedByPerson = a.Key.CreatedByPerson,
                PersonModifiedDateTime = a.Key.PersonModifiedDateTime
            } );

            qry = qry.OrderByDescending( a => a.MaxConfidenceScore ).ThenBy( a => a.LastName ).ThenBy( a => a.FirstName );

            gList.SetLinqDataSource( qry.AsQueryable() );
            gList.DataBind();
        }

        #endregion
    }
}