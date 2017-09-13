using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_willowcreek.Finance
{
    [DisplayName("Batch Date Update")]
    [Category("org_willowcreek > Finance")]
    [Description("Allows bulk updating of batch dates")]
    public partial class BatchDateUpdate : Rock.Web.UI.RockBlock
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var batchListBlock = RockPage.RockBlocks.Where(b => b is RockWeb.Blocks.Finance.BatchList).FirstOrDefault();
            //var gBatchList = batchListBlock.ControlsOfTypeRecursive<Grid>().SingleOrDefault();
            var gBatchList = Page.ControlsOfTypeRecursive<Grid>().Where(x => x.ID == "gBatchList").SingleOrDefault();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var batchIds = new List<int>();
            var gBatchList = Page.ControlsOfTypeRecursive<Grid>().Where(x => x.ID == "gBatchList").SingleOrDefault();
            if (gBatchList != null)
            {
                foreach(GridViewRow row in gBatchList.Rows)
                {
                    var checkBox = row.FindControl("cbSelect_0") as CheckBox;
                    if (checkBox != null && checkBox.Checked)
                    {
                        batchIds.Add(Convert.ToInt32(row.Cells[1].Text));
                    }
                }
                using (var rockContext = new RockContext())
                {
                    var batchService = new FinancialBatchService(rockContext);
                    var batchesToUpdate = batchService.Queryable()
                        .Where(b => batchIds.Contains(b.Id) && b.Status != BatchStatus.Closed)
                        .ToList();
                    var newDate = dpBatchDate.SelectedDate;

                    string message = string.Empty;

                    foreach (var batch in batchesToUpdate)
                    {
                        var changes = new List<string>();
                        History.EvaluateChange(changes, "BatchStartDateTime", batch.BatchStartDateTime, newDate);
                        batch.BatchStartDateTime = newDate;

                        HistoryService.SaveChanges(
                            rockContext,
                            typeof(FinancialBatch),
                            Rock.SystemGuid.Category.HISTORY_FINANCIAL_BATCH.AsGuid(),
                            batch.Id,
                            changes,
                            false);
                    }

                    rockContext.SaveChanges();

                    // Reload the page to get the grid to show updated data.  This is a hack but I couldn't figure out how to refresh the grid directly.
                    Response.Redirect(Request.RawUrl);
                }
            }
        }
    }
}