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
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_willowcreek.Crm.PersonDetail
{
    /// <summary>
    /// Block for displaying the history of changes to a particular user.
    /// </summary>
    [DisplayName( "Donor Letter History" )]
    [Category( "org_willowcreek > CRM > Person Detail" )]
    [Description( "Block for displaying the history of donor letters sent to a person." )]
    [CategoryField("Categories")]
    public partial class DonorLetterHistory : PersonBlock
    {

        #region Fields

        private int personEntityTypeId = int.MinValue;
        private int groupEntityTypeId = int.MinValue;
        private Dictionary<int, string> families = new Dictionary<int, string>();

        #endregion

        #region Base Control Methods


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            gHistory.IsDeleteEnabled = UserCanEdit;
            gHistory.ShowConfirmDeleteDialog = true;

            gHistory.Actions.ShowAdd = UserCanEdit;
            gHistory.Actions.AddClick += gHistory_Add;
            if ( UserCanEdit )
            {
                gHistory.Actions.CopyAddButton( phGridActions.Controls, Page );
            }

            gHistory.ShowActionRow = false;

            dvpLetter.DefinedTypeId = DefinedTypeCache.Read( new Guid( "4B4E71CA-9CF9-4D02-8C30-3EC5C1354964" ) ).Id;
        }

        protected void gHistory_Add( object sender, EventArgs e )
        {
            dlgAddLetter.Show();
        }

        protected void dlgAdd_SaveClick( object sender, EventArgs e )
        {
            if ( dvpLetter.SelectedIndex > 0 )
            {
                var rockContext = new RockContext();
                var historyService = new HistoryService( rockContext );

                var history = new History();
                history.EntityTypeId = EntityTypeCache.Read( typeof( Person ) ).Id;
                history.CategoryId = CategoryCache.Read( new Guid( "142B94E2-BCA3-4837-9697-32139DD7902A" ) ).Id;
                history.EntityId = Person.Id;
                history.Caption = dvpLetter.SelectedItem.Text;
                history.Summary = "Sent Letter";
                history.RelatedEntityTypeId = EntityTypeCache.Read( typeof( DefinedValue ) ).Id;
                history.RelatedEntityId = dvpLetter.SelectedValue.AsIntegerOrNull();
                history.CreatedDateTime = dpLetterDate.SelectedDate ?? DateTime.Now;
                history.ModifiedDateTime = DateTime.Now;

                historyService.Add( history );
                rockContext.SaveChanges();

                BindGrid();
                dlgAddLetter.Hide();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            personEntityTypeId = EntityTypeCache.Read( typeof( Person ) ).Id;
            groupEntityTypeId = EntityTypeCache.Read( typeof( Group ) ).Id;

            if ( Person != null )
            {
                new PersonService( new RockContext() ).GetFamilies( Person.Id ).ToList().ForEach( f => families.Add( f.Id, f.Name ) );

                if ( !Page.IsPostBack )
                {
                    BindGrid();

                    //if ( Person != null )
                    //{
                    //    if ( Person.CreatedDateTime.HasValue )
                    //    {
                    //        hlDateAdded.Text = String.Format( "Date Created: {0}", Person.CreatedDateTime.Value.ToShortDateString() );
                    //    }
                    //    else
                    //    {
                    //        hlDateAdded.Visible = false;
                    //    }
                    //}
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the GridRebind event of the gHistory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void gHistory_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            if (Person != null)
            {
                var familyIds = families.Select( f => f.Key ).ToList();
                var categoriesGuids = GetAttributeValue( "Categories" ).SplitDelimitedValues().AsGuidList();
                var qry = new HistoryService( new RockContext() ).Queryable( "CreatedByPersonAlias.Person" )
                    .Where( h =>
                        ( h.EntityTypeId == personEntityTypeId && h.EntityId == Person.Id ) ||
                        ( h.EntityTypeId == groupEntityTypeId && familyIds.Contains( h.EntityId ) ) )
                    .Where( h => categoriesGuids.Contains( h.Category.Guid ) );

                qry = qry.OrderByDescending( t => t.CreatedDateTime );

                var categoriesAllowed = new Dictionary<int, bool>();

                // Combine history records that were saved at the same time
                var histories = new List<History>();
                foreach(var history in qry)
                {
                    // Make sure current person is allowed to view the category for the history item.
                    if ( !categoriesAllowed.ContainsKey( history.CategoryId ) )
                    {
                        categoriesAllowed.Add( history.CategoryId, history.Category.IsAuthorized( Authorization.VIEW, CurrentPerson ) );
                    }

                    if ( categoriesAllowed[history.CategoryId] )
                    {
                        var existingHistory = histories
                            .Where( h =>
                                h.CreatedByPersonAliasId == history.CreatedByPersonAliasId &&
                                h.CreatedDateTime == history.CreatedDateTime &&
                                h.EntityTypeId == history.EntityTypeId &&
                                h.EntityId == history.EntityId &&
                                h.CategoryId == history.CategoryId &&
                                h.RelatedEntityTypeId == history.RelatedEntityTypeId &&
                                h.RelatedEntityId == history.RelatedEntityId ).FirstOrDefault();
                        if ( existingHistory != null )
                        {
                            existingHistory.Summary += "<br/>" + history.Summary;
                        }
                        else
                        {
                            histories.Add( history );
                        }
                    }
                }


                gHistory.DataSource = histories.Select( h => new
                {
                    Id = h.Id,
                    CategoryId = h.CategoryId,
                    Category = h.Category != null ? h.Category.Name : "",
                    EntityTypeId = h.EntityTypeId,
                    EntityId = h.EntityId,
                    Caption = h.Caption ?? string.Empty,
                    Summary = h.Summary,
                    RelatedEntityTypeId = h.RelatedEntityTypeId ?? 0,
                    RelatedEntityId = h.RelatedEntityId ?? 0,
                    CreatedByPersonId = h.CreatedByPersonAlias != null ? h.CreatedByPersonAlias.PersonId : 0,
                    PersonName = h.CreatedByPersonAlias != null && h.CreatedByPersonAlias.Person != null ? h.CreatedByPersonAlias.Person.NickName + " " + h.CreatedByPersonAlias.Person.LastName : "",
                    CreatedDateTime = h.CreatedDateTime
                } ).ToList();

                gHistory.EntityTypeId = EntityTypeCache.Read<History>().Id;
                gHistory.DataBind();
            }

        }

        /// <summary>
        /// Formats the summary.
        /// </summary>
        /// <param name="entityTypeId">The entity type identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="summary">The summary.</param>
        /// <returns></returns>
        protected string FormatSummary( int entityTypeId, int entityId, string summary )
        {
            if ( entityTypeId == groupEntityTypeId && families.ContainsKey( entityId ) )
            {
                return string.Format( "[{0}] {1}", families[entityId], summary );
            }

            return summary;
        }

        /// <summary>
        /// Formats the caption.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="relatedEntityTypeId">The related entity type identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        protected string FormatCaption( int categoryId, string caption, int? entityId )
        {
            var category = CategoryCache.Read( categoryId );
            if (category != null)
            {
                string urlMask = category.GetAttributeValue( "UrlMask" );
                if (!string.IsNullOrWhiteSpace(urlMask))
                {
                    if (urlMask.Contains("{0}"))
                    {
                        string id = entityId.HasValue ? entityId.Value.ToString() : "";
                        urlMask = string.Format( urlMask, id );
                    }
                    return string.Format( "<a href='{0}'>{1}</a>", ResolveRockUrl( urlMask ), caption );
                }
            }

            return caption;
        }

        #endregion

        protected void gHistory_Delete( object sender, RowEventArgs e )
        {
            var rockContext = new RockContext();
            var historyService = new HistoryService( rockContext );
            var historyId = e.RowKeyId;
            var history = historyService.Get( historyId );
            historyService.Delete( history );
            rockContext.SaveChanges();

            BindGrid();
        }
    }
}