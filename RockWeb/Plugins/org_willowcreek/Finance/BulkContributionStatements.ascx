<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BulkContributionStatements.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Finance.BulkContributionStatements" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <Triggers>
        <asp:PostBackTrigger ControlID="btnMerge" />
    </Triggers>
    <ContentTemplate>
        <Rock:NotificationBox ID="nbWarningMessage" runat="server" NotificationBoxType="Warning" />

        <asp:Panel ID="pnlEntry" runat="server" CssClass="panel panel-block">

            <asp:HiddenField ID="hfEntitySetId" runat="server" />

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-files-o"></i>
                    <asp:Literal ID="lActionTitle" runat="server" Text="Generate Year-End Statements" /></h1>
            </div>
            <div class="panel-body">
                <Rock:NotificationBox ID="nbNotification" runat="server" NotificationBoxType="Success" Text="Merge document submitted for processing. You will receive an email once the statements have been generated." Visible="false" />
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                <div class="row">
                    <div class="col-md-6">
                        <Rock:YearPicker ID="ypYear" runat="server" Label="Year" Required="true" />
                        <div class="row">
                            <div class="col-md-6">
                                <Rock:NumberBox ID="nbChapterSize" runat="server" Label="Chapter Size" Help="Number of records in a chapter" NumberType="Integer" MinimumValue="1" MaximumValue="1000" Text="300" />
                            </div>

                        </div>
                    </div>
                </div>

                <Rock:NotificationBox ID="nbMergeError" runat="server" NotificationBoxType="Warning" Visible="false" CssClass="js-merge-error" />

                <div class="actions">
                    <asp:LinkButton ID="btnMerge" runat="server" Text="Generate" CssClass="btn btn-primary" OnClientClick="$('.js-merge-error').hide()" OnClick="btnMerge_Click" />
                </div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
