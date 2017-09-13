<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonDocumentList.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.FileManagement.PersonDocumentList" %>

<asp:UpdatePanel ID="upBinaryFile" runat="server">
    <ContentTemplate>

        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-files-o"></i> File List</h1>
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">
                    <Rock:GridFilter ID="fBinaryFile" runat="server">
                        <Rock:RockTextBox ID="tbName" runat="server" Label="File Name" />
                        <Rock:RockTextBox ID="tbType" runat="server" Label="Mime Type" />
                    </Rock:GridFilter>
        
                    <Rock:Grid ID="gBinaryFile" runat="server" AllowSorting="true">
                        <Columns>
                            <asp:BoundField DataField="FileName" HeaderText="File Name" SortExpression="FileName" />
                            <asp:BoundField DataField="MimeType" HeaderText="Mime Type" SortExpression="MimeType" />
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Literal ID="viewPlaceholder" runat="server"></asp:Literal>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <Rock:DateTimeField DataField="ModifiedDateTime" HeaderText="Last Modified" SortExpression="ModifiedDateTime" />
                            <Rock:BoolField DataField="IsSystem" HeaderText="System" SortExpression="IsSystem" />
                            <Rock:DeleteField OnClick="gBinaryFile_Delete" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
        </div>

        <Rock:ModalAlert ID="mdGridWarning" runat="server" />
        
            <Rock:ModalDialog ID="modalAddDocument" runat="server" Title="Add Document" Content-Height="380" ValidationGroup="NewDocument" SaveButtonText="Save">
                <Content>

                    <asp:ValidationSummary ID="valSummaryTop" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" ValidationGroup="NewDocument" />

                    <div id="divExistingPerson" runat="server">
                        <fieldset>
                            <Rock:RockTextBox ID="tbNewDocName" runat="server" Label="File Name" Required="false" />
                            <Rock:RockTextBox ID="tbNewDocDescription" runat="server" Label="File Description" Required="false" TextMode="MultiLine" />
                            <Rock:FileUploader ID="fileUploader" runat="server" Label="Upload a new file..." ShowDeleteButton="false" />
                        </fieldset>
                    </div>

                    <asp:HiddenField ID="hfUploadedFileId" runat="server" />
                </Content>
            </Rock:ModalDialog>
        

    </ContentTemplate>
</asp:UpdatePanel>
