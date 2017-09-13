using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using org.willowcreek.CheckIn;
using Rock;
using Rock.Data;
using Rock.Model;

namespace RockWeb.Plugins.org_willowcreek.CheckIn
{
    /// <summary>
    /// Displays the calendars that user is authorized to view.
    /// </summary>
    [DisplayName( "Evacuation Report" )]
    [Category( "org_willowcreek > Check-in" )]
    [Description( "Displays all the attendance by grade" )]

    public partial class EvacuationReport : Rock.Web.UI.RockBlock
    {
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            var checkInTypeId = PageParameter( "CheckinTypeId" ).AsIntegerOrNull();

            if (!IsPostBack)
            {
                dpDate.SelectedDate = DateTime.Today;
                PopulateSchedules();
                var schedule = ActiveSchedule.GetScheduleParameters( checkInTypeId.Value, PageParameter( "Now" ) );
                if ( schedule != null )
                {
                    ddlConfig.SelectedValue = schedule.Item1.ToString();
                }
                DisplayReport();
            }
        }

        private void DisplayReport()
        {
            var ds = DbService.GetDataSet( "wcRpt_EvacuationReport", System.Data.CommandType.StoredProcedure,
                new Dictionary<string, object> {
                    { "CheckInTypeId", PageParameter( "CheckinTypeId" ).AsInteger() },
                    { "ScheduleId", ddlConfig.SelectedValue },
                    { "StartDateTime", dpDate.SelectedDate }
                } );

            string html = "";
            bool first = true;
            string prevGrade = "";
            int gradeTotal = 0;
            int Total = 0;
            string prevReportName = "";

            if ( ds != null && ds.Tables.Count > 0 )
            {
                first = true;
                gradeTotal = 0;
                Total = 0;

                //SUMMARY
                DataTable dtSummary = ds.Tables[0];
                if ( dtSummary.Rows.Count > 0 )
                {
                    int grades = 0;

                    html += "<div class='evacTitle'>Evac: Summary" + "</div>";

                    html += "<div class='evacSummary'>";

                    foreach ( DataRow dr in dtSummary.Rows )
                    {

                        if ( dr["Grade"].ToString() != prevGrade )
                        {
                            if ( first )
                            {
                                first = false;
                            }
                            else
                            {
                                html += "<tr><th>&nbsp;</th><th>" + gradeTotal.ToString() + "</th></tr>";
                                html += "</table></div>";
                                gradeTotal = 0;
                                grades++;
                            }
                            if ( grades >= 12 )
                            {
                                html += "<div class='pg'></div>";
                                grades = 0;
                            }

                            html += "<div style='float:left;width:300px;height:110px;margin-bottom:20px'>";
                            html += "<table>";
                            html += "<tr>";
                            html += "<th style='width:230px'>" + dr["Grade"].ToString() + "</th>";
                            html += "<th>&nbsp;</th>";
                            html += "</tr>";
                        }

                        html += "<tr>";
                        html += "<td>" + dr["ReportName"].ToString() + "</td>";
                        html += "<td>" + dr["Total"].ToString() + "</td>";
                        html += "</tr>";
                        gradeTotal += Convert.ToInt32( dr["Total"] );
                        Total += Convert.ToInt32( dr["Total"] );

                        prevGrade = dr["Grade"].ToString();
                    }

                    html += "<tr><th>&nbsp;</th><th>" + gradeTotal.ToString() + "</th></tr>";
                    html += "</table></div>";
                    html += "<div style='clear:both;font-size: 20px;font-weight: bold;'>Report Total: " + Total.ToString() + "<br/><br/><br/>" + "</div>";
                    html += "</div>";

                    html += "<div class='pg'></div>";
                }

                //DETAILS
                DataTable dt = ds.Tables[1];
                if ( dt.Rows.Count > 0 )
                {
                    first = true;
                    prevGrade = "";
                    string prevSmallGroup = "";
                    gradeTotal = 0;
                    int groupTotal = 0;

                    prevReportName = "";

                    foreach ( DataRow dr in dt.Rows )
                    {
                        string grade = dr["grade"].ToString();
                        if ( prevGrade != grade )
                        {
                            if ( first )
                                first = false;
                            else
                                html += "<div class='pg'></div>";

                            html = html.Replace( "*gradetotal*", gradeTotal.ToString() );
                            html += "<div class='evacGrade'>" + dr["grade"].ToString() + "<span class='evacTotal'>: *gradetotal* kid(s)</span></div>";
                            prevGrade = grade;
                            gradeTotal = 0;
                        }


                        string smallGroup = dr["nodename"].ToString();
                        if ( prevSmallGroup != smallGroup )
                        {
                            html = html.Replace( "*grouptotal*", groupTotal.ToString() );
                            //html += "<div class='evacSmallGroup'>" + dr["nodename"].ToString() + "<span class='evacTotal'>: *grouptotal* kid(s)</span></div>";
                            //html += "<div style='float:left;font-size:14px;font-weight:bold;width:100px;margin-left:75px;'>" + dr["ReportName"].ToString() + "</div>";

                            html += "<table class='evacSmallGroup'>";
                            html += "<tr>";
                            html += "<td style='margin-left:25px;'>" + dr["nodename"].ToString() + "<span class='evacTotal'>: *grouptotal* kid(s)</span></td>";
                            html += "</tr>";

                            html += "<tr>";
                            html += "<td style='float:left;font-size:14px;font-weight:bold;width:100px;margin-left:25px;margin-bottom:-10px;'>" + dr["ReportName"].ToString() + "</td>";
                            html += "</tr>";
                            html += "</table>";

                            prevSmallGroup = smallGroup;
                            prevReportName = dr["ReportName"].ToString();
                            groupTotal = 0;
                        }


                        //string ReportName = dr["ReportName"].ToString();
                        //if (prevReportName != ReportName)
                        if ( prevReportName != dr["ReportName"].ToString() )
                        {
                            html += "<table class='evacSmallGroup'>";
                            html += "<tr>";
                            html += "<td style='float:left;font-size:14px;font-weight:bold;width:100px;margin-left:25px;margin-bottom:-10px;'>" + dr["ReportName"].ToString() + "</td>";
                            html += "</tr>";
                            html += "</table>";

                            prevReportName = dr["ReportName"].ToString();
                        }


                        html += "<table class='evacChild'>";
                        html += "<td class='evacId'>" + dr["phouseholdid"].ToString() + "</td>";
                        html += "<td class='evacName'>" + dr["lastname"].ToString() + ", " + dr["firstname"].ToString() + "</td>";
                        html += "<td class='evacName'>" + dr["ParentLocation"].ToString() + "</td>";
                        html += "</table>";
                        gradeTotal++;
                        groupTotal++;
                    }
                    html = html.Replace( "*gradetotal*", gradeTotal.ToString() );
                    html = html.Replace( "*grouptotal*", groupTotal.ToString() );
                }
                //html += "<div class='pg'></div>";
            }

            lHtml.Text = html;
        }


        protected void dbDate_TextChanged( object sender, EventArgs e )
        {
            PopulateSchedules();
            DisplayReport();
        }

        protected void ddlConfig_SelectedIndexChanged( object sender, EventArgs e )
        {
            DisplayReport();
        }

        private void PopulateSchedules()
        {
            var schedules = ActiveSchedule.GetSchedules( new RockContext(), PageParameter( "CheckinTypeId" ).AsInteger(), dpDate.SelectedDate.Value );
            ddlConfig.DataValueField = "ID";
            ddlConfig.DataTextField = "Name";
            ddlConfig.DataSource = schedules;
            ddlConfig.DataBind();
        }

        protected void dpDate_SelectedBirthdayChanged( object sender, EventArgs e )
        {
            PopulateSchedules();
            DisplayReport();
        }
    }
}