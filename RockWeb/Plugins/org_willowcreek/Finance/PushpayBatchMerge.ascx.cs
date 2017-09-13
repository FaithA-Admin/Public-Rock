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
    [DisplayName( "Pushpay Batch Merge" )]
    [Category( "org_willowcreek > Finance" )]
    [Description( "Merges open Pushpay batches that have the same name" )]
    public partial class PushpayBatchMerge : Rock.Web.UI.RockBlock
    {
        protected void btnMerge_Click( object sender, EventArgs e )
        {
            var parameters = new Dictionary<string, object>
            {
                { "StartDate", drpDateRange.LowerValue },
                { "EndDate", drpDateRange.UpperValue },
                { "PersonAliasID", CurrentPersonAliasId }
            };
            DbService.ExecuteCommand( "wcUtil_MergePushpayBatches", System.Data.CommandType.StoredProcedure, parameters );
            pnlSuccess.Visible = true;
        }
    }
}