<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BatchDateUpdate.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Finance.BatchDateUpdate" %>

<asp:UpdatePanel ID="upnlDateUpdate" runat="server">
    <ContentTemplate>
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-calendar"></i> Update Selected Batches</h1>
            </div>
            <div class="panel-body">
                <%--<div class="form-control-group">--%>
                <table>
                    <tr style="vertical-align: bottom">
                        <td>
                            <Rock:DatePicker ID="dpBatchDate" Label="New Batch Date" runat="server" Required="true" CssClass="input-group" Help="Update the date on all the Open/Pending batches that are checked above."></Rock:DatePicker>
                        </td>
                        <td>
                            <asp:Button ID="btnUpdate" runat="server" Text="Update Dates" CssClass="btn btn-primary input-group" Style="margin: 15px;" OnClick="btnUpdate_Click" />
                        </td>
                    </tr>
                </table>
                <%--</div>--%>
            </div>
        </div>
    </ContentTemplate>
<%--    <Triggers>
        <asp:PostBackTrigger ControlID="btnUpdate" /> 
    </Triggers>--%>
</asp:UpdatePanel>
