<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PushpayBatchMerge.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Finance.PushpayBatchMerge" %>

<asp:UpdatePanel ID="upnlDateUpdate" runat="server">
    <ContentTemplate>
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-compress"></i> Merge Open Batches</h1>
            </div>
            <div class="panel-body">
                <Rock:DateRangePicker ID="drpDateRange" runat="server" Label="Batch Date" Required="true" RequiredErrorMessage="You must select a date range" />
                <asp:Button ID="btnMerge" runat="server" Text="Merge" CssClass="btn btn-primary input-group" OnClick="btnMerge_Click" />
                <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
                    <div class="alert alert-success" style="margin-top: 15px; margin-bottom: 0px;">
                        <strong>Success!</strong> Pushpay batches were merged.
                    </div>
                </asp:Panel>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
