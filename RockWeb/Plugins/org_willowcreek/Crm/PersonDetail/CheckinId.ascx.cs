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
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

using Rock;
using Rock.Attribute;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.Cache;
using Rock.Data;
using Rock.Web.UI.Controls;
using org.willowcreek.SystemGuid;


namespace RockWeb.Plugins.org_willowcreek.Crm.PersonDetail
{
    [DisplayName("RockCheckinId")]
    [Category("org_willowcreek > CRM > Person Detail")]
    [Description("Handles displaying the Checkin Id of a family.")]    
    public partial class CheckinId : PersonBlock
    {

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);          
            if (Person != null && Person.Id != 0)
            {               
                loadBlockData();
            }
        }

        protected void btnNewCheckinCard_Click(object sender, EventArgs e)
        {
            using (var rockContext = new RockContext())
            {
                var personAliasService = new PersonAliasService(rockContext);
                var groupMemberService = new GroupMemberService(rockContext);
                var groupService = new GroupService(rockContext);
                var groupTypeService = new GroupTypeService(rockContext);
                var attributeValueService = new AttributeValueService(rockContext);
                var attributeService = new AttributeService(rockContext);
                Guid familyGroupTypeGuidAsGuid = GroupTypeGuids.FAMILY.AsGuid();
                Guid checkInCardAsGuid = AttributeGuids.CHECKINCARD.AsGuid();

                var familyGroupQry = from gm in groupMemberService.Queryable()
                                     join g in groupService.Queryable() on gm.GroupId equals g.Id
                                     join gt in groupTypeService.Queryable() on g.GroupTypeId equals gt.Id
                                     where gt.Guid == familyGroupTypeGuidAsGuid
                                            && gm.PersonId == Person.Id
                                     select new { g.Id };
                var familyGroup = familyGroupQry.FirstOrDefault().Id.ToString().AsDoubleOrNull();

                var checkinQry = from gm in groupMemberService.Queryable()
                                 join g in groupService.Queryable() on gm.GroupId equals g.Id
                                 join gt in groupTypeService.Queryable() on g.GroupTypeId equals gt.Id
                                 join av in attributeValueService.Queryable() on g.Id equals av.EntityId
                                 join a in attributeService.Queryable() on av.AttributeId equals a.Id
                                 where gt.Guid == familyGroupTypeGuidAsGuid
                                        && gm.PersonId == Person.Id
                                        && a.Guid == checkInCardAsGuid
                                 select new { av.Value };
                var checkInCardTemp = checkinQry.FirstOrDefault();
                object checkInCard = null;
                if(checkInCardTemp != null)
                {
                    checkInCard = checkInCardTemp.Value.AsDoubleOrNull().ToString();
                }else
                {
                    checkInCard = System.DBNull.Value;
                }

                var personAliasQry = from pa in personAliasService.Queryable()
                                     where pa.PersonId == CurrentUser.PersonId
                                     select new { pa.Id };
                var personAlias = personAliasQry.FirstOrDefault().Id;
                
                SqlParameter checkInCardParam = new SqlParameter("@previousCardNumber", checkInCard);
                SqlParameter familyGroupParam = new SqlParameter("@familyId", familyGroup);
                SqlParameter personAliasParam = new SqlParameter("@cardNumberRequestorPersonAliasId", personAlias);
                SqlParameter newCardNumber = new SqlParameter("@newCardNumber", SqlDbType.NVarChar,-1);
                newCardNumber.Direction = ParameterDirection.Output;


                rockContext.Database.ExecuteSqlCommand("wcCheckin_AssignCardToFamily @previousCardNumber, @familyId, @cardNumberRequestorPersonAliasId, @newCardNumber out",
                                                        checkInCardParam, familyGroupParam, personAliasParam, newCardNumber);

                var changes = new List<string>();
                if(checkInCardTemp == null)
                {
                    History.EvaluateChange(changes, "Rock Check-In Card",null, newCardNumber.Value.ToString().Insert(3, "-"));
                }
                else
                {
                    History.EvaluateChange(changes, "Rock Check-In Card", checkInCardTemp.Value.ToString().Insert(3, "-"), newCardNumber.Value.ToString().Insert(3, "-"));
                }                
                HistoryService.SaveChanges(rockContext, typeof(Person), Rock.SystemGuid.Category.HISTORY_PERSON_FAMILY_CHANGES.AsGuid(), Person.Id, changes);
                            
            }   
            Response.Redirect(this.Page.Request.CurrentExecutionFilePath);        
        }

        private void loadBlockData()
        {
            using (var rockContext = new RockContext())
            {
                fsCheckinIds.Controls.Clear();
                var personAliasService = new PersonAliasService(rockContext);
                var groupMemberService = new GroupMemberService(rockContext);
                var groupService = new GroupService(rockContext);
                var groupTypeService = new GroupTypeService(rockContext);
                var attributeValueService = new AttributeValueService(rockContext);
                var attributeService = new AttributeService(rockContext);
                Guid familyGroupTypeGuidAsGuid = GroupTypeGuids.FAMILY.AsGuid();
                Guid checkInCardAsGuid = AttributeGuids.CHECKINCARD.AsGuid();

                var checkinQry = from gm in groupMemberService.Queryable()
                                 join g in groupService.Queryable() on gm.GroupId equals g.Id
                                 join gt in groupTypeService.Queryable() on g.GroupTypeId equals gt.Id
                                 join av in attributeValueService.Queryable() on g.Id equals av.EntityId
                                 join a in attributeService.Queryable() on av.AttributeId equals a.Id
                                 where gt.Guid == familyGroupTypeGuidAsGuid
                                        && gm.PersonId == Person.Id
                                        && a.Guid == checkInCardAsGuid
                                        && av.Value.Trim() != ""
                                 select new { g.Name, av.Value };
                var checkInCard = checkinQry.FirstOrDefault();

                if (checkInCard != null  && checkInCard.Value != null && checkInCard.Name != null)
                {
                    string familyName = checkInCard.Name.First().ToString().ToUpper() + checkInCard.Name.Substring( 1 );
                    string barcode = checkInCard.Value.ToString() + "-" + Person.Id.ToString();
                    string cardNumber = checkInCard.Value.ToString().Insert( 3, "-" ) + " " + Person.FirstName[0] + Person.LastName[0];

                    var cardPage = GetAttributeValue( "ViewCardPage" ).AsGuidOrNull();
                    if(cardPage != null)
                    {
                        var pageId = new PageService( rockContext ).Queryable().Where( p => p.Guid == cardPage ).Select( p => p.Id ).First().ToString();
                        fsCheckinIds.Controls.Add( new System.Web.UI.WebControls.HyperLink {   Target="_blank", NavigateUrl = "~/Page/" + pageId + "?qrCode=" + barcode + "&cardNumber=" + cardNumber, Text = "<p style=\"text-align:left;\">Show Card <i class=\"fa fa-id-card\"></i></p>" } );

                    }
                    AddPrintCardScript(barcode, cardNumber);
                    fsCheckinIds.Controls.Add( new System.Web.UI.WebControls.HyperLink { NavigateUrl = "javascript:printCard()", Text = "<p style=\"text-align:right;\">Print Card <i class=\"fa fa-id-card\"></i></p>" } );
                    fsCheckinIds.Controls.Add(new RockLiteral { Label = "Family Name:", Text = familyName });
                    fsCheckinIds.Controls.Add(new RockLiteral { Label = "Barcode:", Text = barcode });
                    fsCheckinIds.Controls.Add(new RockLiteral { Label = "Card number:", Text = cardNumber });
                    lbNewCheckinCard.Text = "Replace Check-In ID";
                }
                else
                {
                    lbNewCheckinCard.Text = "New Check-In ID";
                }
            }            
        }

        private void AddPrintCardScript(string barcode, string cardNumber)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "function printCard() {" );           
            sb.AppendLine( "var windowContent = '<!DOCTYPE html>';" );
            sb.AppendLine( "windowContent += '<html lang=\"en\">';" );
            sb.AppendLine( "windowContent += '<head>';" );
            sb.AppendLine( "windowContent += '<meta charset=\"utf - 8\">';" );
            sb.AppendLine( "windowContent += '<style>@page { size: landscape; margin: 0mm; } body { background-color:#FFFFFF; ma  }</style>';" );
            sb.AppendLine( "windowContent += '<style scoped>.checkInCard { margin: auto; position: absolute; top:0; left:0; right:0; Text-Align:center; display: block !important; }</style>';" );            
            sb.AppendLine( "windowContent += '</head>';" );
            sb.AppendLine( "windowContent += '<body class=\"checkInCard\">';" );
            sb.AppendLine( "windowContent += '<img src=\"https://chart.googleapis.com/chart?chs=150x150&cht=qr&chl=" + barcode.EncodeHtml() + "\">';" );
            sb.AppendLine( "windowContent += '<br><b>" + cardNumber + "</b>';" );
            sb.AppendLine( "windowContent += '</body>';" );
            sb.AppendLine( "windowContent += '</html>';" );
            sb.AppendLine( "var printWin = window.open();" );
            sb.AppendLine( "printWin.document.open();" );
            sb.AppendLine( "printWin.document.write( windowContent );" );
            sb.AppendLine( "setTimeout(function(){ printWin.document.close(); printWin.focus(); printWin.print(); printWin.close(); }, 1000);" );
            sb.AppendLine( "" );
            sb.AppendLine( "}" );
            RockPage.AddScriptToHead( this.Page, sb.ToString(), true );
        }
    }
}