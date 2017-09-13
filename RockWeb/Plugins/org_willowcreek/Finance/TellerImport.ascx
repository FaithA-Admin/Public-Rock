<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TellerImport.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Finance.TellerImport" %>

<script src="/SignalR/hubs"></script>
<script type="text/javascript">

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_initializeRequest(InitializeRequest);

    function InitializeRequest(sender, args) {
        var updateProgress = $get('updateProgress');
        var postBackElement = args.get_postBackElement();
        if (postBackElement.id == '<%= btnConfirm.ClientID %>') {
            updateProgress.control._associatedUpdatePanelId = 'dummyId';
            $('#<%= btnCancelConfirm.ClientID %>').hide();
        }
        else{
            updateProgress.control._associatedUpdatePanelId = null;
        }
    }

    $(function () {

        var proxy = $.connection.rockMessageHub;

        proxy.client.receiveNotification = function (name, message) {
            if (name == '<%=signalREventName %>') {
                $("div.progress").show();
                var percentage = message + '%'
                var $progress = $("div.progress-bar");
                $progress.attr("aria-valuenow", message);
                $progress.css("width", percentage);
                $progress.html(percentage);
            }
        }

        $.connection.hub.start().done(function () {
            // hub started... 
        });
    })

</script>

<asp:UpdatePanel ID="upnlContent" runat="server">

    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-download"></i> Import Scanned Checks</h1>
            </div>

            <div class="panel-body">

                <asp:Panel ID="pnlEntry" runat="server" Visible="true" >

                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                    <Rock:NotificationBox ID="nbMessage" runat="server" />

                    <div class="actions margin-b-md">
                        <asp:LinkButton ID="btnImport" runat="server" CssClass="btn btn-primary" Text="Import" OnClick="btnImport_Click" />
                    </div>

                    <div class="panel panel-block">
                        <div class="panel-heading">
                            <h1 class="panel-title"><i class="fa fa-archive"></i> Available Batches</h1>
                        </div>
                        <div class="panel-body">
                            <div class="grid grid-panel">
                                <Rock:ModalAlert ID="maWarningDialog" runat="server" />
                                <Rock:GridFilter ID="gfBatchFilter" runat="server">
                                    <Rock:DateRangePicker ID="drpBatchDate" runat="server" Label="Date Range" />
                                    <Rock:DefinedValuesPicker ID="dvpCompany" runat="server" Label="Company"  />
                                    <Rock:AccountPicker ID="apAccount" runat="server" Label="Account" AllowMultiSelect="true"  />
                                </Rock:GridFilter>

                                <Rock:ModalAlert ID="mdGridWarning" runat="server" />
                                <Rock:Grid ID="gBatchList" runat="server" RowItemText="Batch" AllowSorting="true" CssClass="js-grid-batch-list">
                                    <Columns>
                                        <Rock:SelectField />
                                        <Rock:RockBoundField DataField="Batch" HeaderText="Batch" SortExpression="Batch" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                                        <Rock:DateField DataField="Date" HeaderText="Date" SortExpression="Date" HeaderStyle-HorizontalAlign="Right" />
                                        <Rock:RockBoundField DataField="Checks" HeaderText="Checks" HtmlEncode="false" SortExpression="Checks" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                                        <Rock:RockBoundField DataField="Company" HeaderText="Company" SortExpression="Company" />
                                        <Rock:RockBoundField DataField="Account" HeaderText="Account" SortExpression="Account" />
                                        <Rock:RockBoundField DataField="BatchId" HeaderText="BatchId" SortExpression="BatchId" Visible="false" />
                                    </Columns>
                                </Rock:Grid>
                            </div>
                        </div>
                    </div>


                </asp:Panel>

                <asp:Panel ID="pnlConfirm" runat="server" Visible="false" >

                    <h4><asp:Literal ID="lConfirm" runat="server" /></h4>
                    <br />

                    <div class="progress" style="display:none">
                        <div class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="min-width: 2em;">
                            0%
                        </div>
                    </div>

                    <div class="actions">
                        <Rock:BootstrapButton ID="btnConfirm" runat="server" CssClass="btn btn-primary" Text="Confirm" DataLoadingText="Importing..." CausesValidation="true" OnClick="btnConfirm_Click" />
                        <asp:LinkButton ID="btnCancelConfirm" runat="server" CssClass="btn btn-link" Text="Cancel" OnClick="btnCancelConfirm_Click"/>
                    </div>

                </asp:Panel>

                <asp:Panel ID="pnlResults" runat="server" Visible="false" >

                    <Rock:NotificationBox ID="nbSuccess" runat="server" NotificationBoxType="Success" Heading="Import Summary:"  />
                    <Rock:NotificationBox ID="nbErrors" runat="server" NotificationBoxType="Danger" Heading="Import Errors:"  />

                    <div class="actions">
                        <asp:LinkButton ID="btnImportMore" runat="server" CssClass="btn btn-link" Text="Import More" OnClick="btnImportMore_Click" />
                    </div>

                </asp:Panel>

                <asp:Panel ID="pnlSetup" runat="server" Visible="false" >
                    <Rock:NotificationBox ID="nbSetup" runat="server" Text="Please configure the block settings." />
                </asp:Panel>

            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
