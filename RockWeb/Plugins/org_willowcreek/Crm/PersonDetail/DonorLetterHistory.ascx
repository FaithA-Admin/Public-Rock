<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DonorLetterHistory.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Crm.PersonDetail.DonorLetterHistory" %>

<script type="text/javascript">
    function clearActiveDialog() {
        $('#<%=hfActiveDialog.ClientID %>').val('');
    }
</script>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlList" CssClass="panel panel-block" runat="server">

            <div class="panel-heading grid-actions">
                <h1 class="panel-title pull-left"><i class="fa  fa-file-text-o"></i> Donor Letter History</h1>
<%--                <div class="panel-labels">
                    <Rock:HighlightLabel ID="hlDateAdded" runat="server" LabelType="Info" />
                </div>--%>
                <asp:PlaceHolder ID="phGridActions" runat="server" />
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">
                    <Rock:Grid ID="gHistory" runat="server" AllowSorting="false" RowItemText="Letter" DataKeyNames="Id" OnGridRebind="gHistory_GridRebind" AllowPaging="false">
                        <Columns>
                            <Rock:RockTemplateField HeaderText="Letter">
                                <ItemTemplate><%# FormatCaption( (int)Eval("CategoryId"), Eval( "Caption" ).ToString(), (int)Eval( "RelatedEntityId" ) ) %></ItemTemplate>
                            </Rock:RockTemplateField>
                            <Rock:DateTimeField DataField="CreatedDateTime" SortExpression="CreatedDateTime" HeaderText="Date" FormatAsElapsedTime="false" DataFormatString="{0:d}" HeaderStyle-HorizontalAlign="Right" />
                            <Rock:DeleteField OnClick="gHistory_Delete" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>

        </asp:Panel>

        <Rock:NotificationBox ID="nbMessage" runat="server" Title="Error" NotificationBoxType="Danger" Visible="false" />

        <Rock:ModalDialog id="dlgAddLetter" runat="server" Title="Add Letter" OnSaveClick="dlgAdd_SaveClick" OnCancelScript="clearActiveDialog();" ValidationGroup="AddLetter">
            <Content>
                <Rock:DefinedValuePicker ID="dvpLetter" runat="server" Label="Letter" Required="true" />
                <Rock:DatePicker ID="dpLetterDate" runat="server" Label="Date" />
            </Content>
        </Rock:ModalDialog>

        <asp:HiddenField ID="hfActiveDialog" runat="server" />

    </ContentTemplate>
</asp:UpdatePanel>
