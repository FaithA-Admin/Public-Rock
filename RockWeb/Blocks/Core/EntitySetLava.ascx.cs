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
using System.Web.UI;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Core
{
    /// <summary>
    /// Block for displaying contents of an entity set formatted using Lava
    /// </summary>
    [DisplayName( "Entity Set Lava" )]
    [Category( "Core" )]
    [Description( "Block for displaying contents of an entity set formatted using Lava." )]

    [CodeEditorField( "Lava Template", "The Lava template to use for formatting the Entity Set.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 500, true, "", "", 0 )]
    [BooleanField( "Enable Debug", "Shows the merge fields available for the Lava", order: 1 )]
    public partial class EntitySetLava : RockBlock
    {

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

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
                DisplayResults();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            DisplayResults();
        }

        #endregion

        #region Methods

        private void DisplayResults()
        {
            int? entitySetId = PageParameter( "EntitySetId" ).AsIntegerOrNull();
            if ( entitySetId.HasValue  )
            {
                using ( var rockContext = new RockContext() )
                {
                    var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson );

                    var entitySet = new EntitySetService( rockContext ).Get( entitySetId.Value );
                    if ( entitySet != null  )
                    {
                        mergeFields.Add( "EntitySet", entitySet );
                    }

                    var template = GetAttributeValue( "LavaTemplate" );
                    lResults.Text = template.ResolveMergeFields( mergeFields );

                    // show debug info
                    if ( GetAttributeValue( "EnableDebug" ).AsBoolean() && UserCanEdit )
                    {
                        lDebug.Visible = true;
                        lDebug.Text = mergeFields.lavaDebugInfo();
                    }
                }
            }
        }

        #endregion

    }
}