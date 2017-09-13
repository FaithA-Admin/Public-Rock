<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VehicleDetail.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Cars.VehicleDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlMain" runat="server" CssClass="panel panel-block">
        
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-car"></i> <asp:Literal ID="lTitle" runat="server" /></h1>

                <div class="panel-labels">
                    <Rock:HighlightLabel ID="hlblStatus" runat="server" LabelType="Info" Text="Status" />
                    <Rock:HighlightLabel ID="hlblStockNumber" runat="server" LabelType="Type" Text="Stock Nubmer" />
                </div>
            </div>

            <Rock:PanelDrawer ID="pdAuditDetails" runat="server"></Rock:PanelDrawer>
            
            <Rock:NotificationBox ID="nbMessage" runat="server" Visible="false" />

            <asp:Panel ID="pnlView" runat="server" CssClass="panel-body">

                <div class="row">
                    <div class="col-md-6">
                        <div class="row">
                            <div class="col-sm-6">
                                <Rock:RockLiteral ID="lDonor" runat="server" Label="Donor" />
                                <Rock:RockLiteral ID="lFedId" runat="server" Label="" />
                            </div>
                            <div class="col-sm-6">
                                <Rock:RockLiteral ID="lPhoneNumber" runat="server" Label="Home Phone Number" />
                                <Rock:RockLiteral ID="lEmail" runat="server" Label="Email" />
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="row">
                            <div class="col-sm-6">
                                <Rock:RockLiteral ID="lMailingAddr" runat="server" Label="Mailing Address" />
                            </div>
                            <div class="col-sm-6">
                                <Rock:RockLiteral ID="lPickupAddr" runat="server" Label="Pickup Address" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div id="divSubStatus" runat="server" class="col-sm-3 col-xs-6">
                        <Rock:RockLiteral ID="lSubStatus" runat="server" Label="Sub-Status" />
                    </div>
                    <div class="col-sm-3 col-xs-6">
                        <Rock:RockLiteral ID="lDateEntered" runat="server" Label="Date Entered" />
                    </div>
                    <div id="divDateInInventory" runat="server" class="col-sm-3 col-xs-6">
                        <Rock:RockLiteral ID="lDateInInventory" runat="server" Label="Date In Inventory" />
                    </div>
                    <div id="divDateDonorLetterSent" runat="server" class="col-sm-3 col-xs-6">
                        <Rock:RockLiteral ID="lDateDonorLetterSent" runat="server" Label="Donor Letter Sent" />
                    </div>
                    <div id="divDateSoldLetterSent" runat="server" class="col-sm-3 col-xs-6">
                        <Rock:RockLiteral ID="lDateSoldLetterSent" runat="server" Label="Sold Letter Sent" />
                    </div>
                    <div id="divDateCompleted" runat="server" class="col-sm-3 col-xs-6">
                        <Rock:RockLiteral ID="lDateCompleted" runat="server" Label="Date Completed" />
                    </div>
                </div>

                <div class="actions clearfix">
                    <div class="pull-right">
                        <asp:LinkButton ID="lbMoveToInventory" runat="server" CssClass="btn btn-primary btn-sm" Text="Move To Inventory" CausesValidation="false" OnClick="lbMoveToInventory_Click" />
                        <asp:LinkButton ID="lbMarkComplete" runat="server" CssClass="btn btn-primary btn-sm" Text="Mark Complete" CausesValidation="false" OnClick="lbMarkComplete_Click"/>
                        <asp:LinkButton ID="lbEditDisposition" runat="server" CssClass="btn btn-link btn-sm" Text="Edit Disposition Details" CausesValidation="false" OnClick="lbMarkComplete_Click"/>
                    </div>
                </div>

                <asp:Panel ID="pnlDisposition" runat="server" CssClass="well margin-t-md" Visible="false">
    
                    <div class="row">
                        <div class="col-sm-4">
                            <Rock:RockDropDownList ID="ddlDispositionType" runat="server" Label="Disposition Type" Required="true" AutoPostBack="true" OnSelectedIndexChanged="ddlDispositionType_SelectedIndexChanged" />
                        </div>
                        <div class="col-sm-4">
                            <Rock:PersonPicker ID="ppRecipient" runat="server" Label="Recipient" IncludeBusinesses="true" />
                        </div>
                        <div class="col-sm-4">
                            <Rock:DatePicker ID="dpCompletedDate" runat="server" Label="Completed Date" Required="true" />
                        </div>
                    </div>

                    <asp:Panel ID="pnlDispositionAmount" runat="server" CssClass="row" Visible="false">
                        <div class="col-sm-4">
                            <Rock:CurrencyBox ID="cbAmountCollected" runat="server" Label="Amount Collected" />
                        </div>
                        <div class="col-sm-4">
                            <Rock:RockDropDownList ID="ddlPaymentType" runat="server" Label="Payment Type" />
                        </div>
                        <div class="col-sm-4">
                            <Rock:RockTextBox ID="tbPaymentNote" runat="server" Label="Payment Note" />
                        </div>
                    </asp:Panel>

                    <div class="row">
                        <div class="col-sm-12">
                            <Rock:RockTextBox ID="tb1098Summary" runat="server" Label="1098 Summary (30 characters)" MaxLength="30" />
                        </div>
                    </div>

                    <div class="actions">
                        <asp:LinkButton ID="lbSaveDisposition" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="lbSaveDisposition_Click" />
                        <asp:LinkButton ID="lbCancelDisposition" runat="server" CssClass="btn btn-link" Text="Cancel" CausesValidation="false" OnClick="lbCancelDisposition_Click" />
                    </div>

                </asp:Panel>                

                <div class="well margin-t-md">

                    <div class="clearfix margin-b-sm">
                        <h3 class="panel-title pull-left">Donation Details</h3>
                        <div class="panel-labels pull-right">
                            <Rock:HighlightLabel ID="hlblCondition" runat="server" LabelType="Success" Text="Condition" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-5 col-sm-4">
                            <div class="row">
                                <div class="col-md-6 margin-b-sm">
                                    <asp:Literal ID="lPhoto1" runat="server" />
                                </div>
                                <div class="col-md-6 margin-b-sm">
                                    <asp:Literal ID="lPhoto2" runat="server" />
                                </div>
                                <div class="col-md-6 margin-b-sm">
                                    <asp:Literal ID="lPhoto3" runat="server" />
                                </div>
                                <div class="col-md-6 margin-b-sm">
                                    <asp:Literal ID="lPhoto4" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 margin-b-sm">
                                    <asp:Literal ID="lTitleLink" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="col-md-7 col-sm-8">
                            <div class="row">
                                <div class="col-sm-8">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <asp:Literal ID="lVehicleDetailsLeft" runat="server" />
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:Literal ID="lVehicleDetailsRight" runat="server" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <Rock:RockLiteral ID="lNotes" runat="server" Label="Donor Notes" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <asp:Literal ID="lDispositionDetails" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="actions">
                    <asp:LinkButton ID="lbEdit" runat="server" CssClass="btn btn-primary" Text="Edit" OnClick="lbEdit_Click" />
                    <asp:LinkButton ID="lbEmailDonorLetter" runat="server" CssClass="btn btn-default" Text="Send Donor Letter" OnClick="lbEmailDonorLetter_Click" />
                    <asp:LinkButton ID="lbEmailSoldLetter" runat="server" CssClass="btn btn-default" Text="Send Sold Letter" visible="false" OnClick="lbEmailSoldLetter_Click" />
                </div>

            </asp:Panel>        

            <asp:Panel ID="pnlEdit" runat="server" CssClass="panel-body" Visible="false">

                <asp:ValidationSummary ID="valSummaryTop" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                <asp:HiddenField ID="hfVehicleId" runat="server" />

                <div class="row">
                    <div class="col-sm-4">
                        <Rock:PersonPicker ID="ppDonor" runat="server" Label="Donor" IncludeBusinesses="true" />
                    </div>
                    <div class="col-sm-8">
                        <Rock:RockRadioButtonList ID="rblStatus" runat="server" Label="Status" RepeatDirection="Horizontal" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-4">
                        <Rock:RockDropDownList ID="ddlSubStatus" runat="server" Label="Sub-Status" />
                        <Rock:DatePicker ID="dpDateEntered" runat="server" Label="Date Entered" Required="true" />
                    </div>
                    <div class="col-sm-2">
                        <Rock:RockCheckBox ID="cbIsDropOff" runat="server" Label="Drop Off" Text="Yes" AutoPostBack="true" OnCheckedChanged="cbIsDropOff_CheckedChanged" />
                    </div>
                    <div class="col-sm-6">
                        <Rock:AddressControl ID="acPickupLocation" runat="server" Label="Pickup Address" Required="true" />
                    </div>
                </div>

                <div class="well">
                    <h2 class="panel-title margin-b-md">Donation Details</h2>

                    <div class="row">
                        <div class="col-sm-3 col-md-2">
                            <Rock:YearPicker ID="ypYear" runat="server" Label="Vehicle Year" Required="true" />
                        </div>
                        <div class="col-sm-3 col-md-4">
                            <Rock:RockRadioButtonList ID="rblCondition" runat="server" Label="Condition" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1" Text="Runing" />
                                <asp:ListItem Value="0" Text="Not Running" />
                            </Rock:RockRadioButtonList>
                        </div>
                        <div class="col-sm-3 col-md-2">
                            <Rock:CurrencyBox ID="cbEstimatedValue" runat="server" Label="Estimated Value" />
                        </div>
                        <div class="col-sm-3 col-md-4">
                            <Rock:RockRadioButtonList ID="rblAssessedValue" runat="server" Label="Assessed Value" RepeatDirection="Horizontal" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-6">
                            <Rock:RockDropDownList ID="ddlMake" runat="server" Label="Make" Required="true" AutoPostBack="true" OnSelectedIndexChanged="ddlMake_SelectedIndexChanged"/>
                            <Rock:RockDropDownList ID="ddlModel" runat="server" Label="Model" Required="true" />
                            <Rock:RockTextBox ID="dtvin" runat="server" Label="VIN" />
                        </div>
                        <div class="col-sm-6">
                            <Rock:RockTextBox ID="tbMileage" runat="server" Label="Mileage" />
                            <Rock:RockDropDownList ID="ddlBodyStyle" runat="server" Label="Body Style" />
                            <Rock:RockDropDownList ID="ddlColor" runat="server" Label="Color" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12">
                            <Rock:RockTextBox ID="tbNote" runat="server" Label="Donor Notes" TextMode="MultiLine" Rows="3" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-2 col-sm-3 col-xs-6">
                            <Rock:ImageUploader ID="iuPhoto1" runat="server" Label="Photos" />
                        </div>
                        <div class="col-md-2 col-sm-3 col-xs-6">
                            <Rock:ImageUploader ID="iuPhoto2" runat="server" Label="&nbsp;" />
                        </div>
                        <div class="col-md-2 col-sm-3 col-xs-6">
                            <Rock:ImageUploader ID="iuPhoto3" runat="server" Label="&nbsp;" />
                        </div>
                        <div class="col-md-2 col-sm-3 col-xs-6">
                            <Rock:ImageUploader ID="iuPhoto4" runat="server" Label="&nbsp;" />
                        </div>
                        <div class="col-md-2 col-sm-3 col-xs-6">
                            <Rock:FileUploader ID="fuTitle" runat="server" Label="Title" />
                        </div>
                    </div>

                </div>

                <div class="actions">
                    <asp:LinkButton ID="lbSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="lbSave_Click" />
                    <asp:LinkButton ID="lbCancel" runat="server" CssClass="btn btn-link" Text="Cancel" CausesValidation="false" OnClick="lbCancel_Click" />
                </div>

            </asp:Panel>

        </asp:Panel>

        <asp:Panel ID="pnlCommunication" runat="server" Visible="false"  CssClass="panel panel-block" >

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-car"></i> <asp:Literal ID="lCommunicationHeading" runat="server" /></h1>
            </div>

            <div class="panel-body">

                <Rock:NotificationBox ID="nbCommunication" runat="server" NotificationBoxType="Info" Dismissable="true" Visible="false" />

                <asp:HiddenField ID="hfCommType" runat="server" />

                <div class="row">
                    <div class="col-sm-6">
                        <Rock:RockLiteral ID="lCommDonor" runat="server" Label="Donor" />
                    </div>
                    <div class="col-sm-6">
                        <Rock:RockLiteral ID="lCommEmail" runat="server" Label="Email Address" />
                    </div>
                </div>

                <div class="well margin-t-sm">
                    <h3 class="panel-title">Vehicle Summary</h3>
                    <div class="row margin-t-sm">
                        <div class="col-md-2 col-sm-3 col-xs-6">
                            <asp:Literal ID="lCommPhoto" runat="server" />
                        </div>
                        <div class="col-md-10 col-sm-9 col-xs-6">
                            <asp:Literal ID="lComVehicleSummary" runat="server" />
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-6">
                        <Rock:RockTextBox ID="tbFromName" runat="server" Label="From Name" Required="true" />
                        <Rock:EmailBox ID="ebFromAddress" runat="server" Label="From Address" Required="true" />
                        <Rock:RockTextBox ID="tbSubject" runat="server" Label="Subject" Required="true" />
                    </div>
                    <div class="col-sm-6">
                        <asp:HiddenField ID="hfAttachments" runat="server" />
                        <Rock:FileUploader ID="fuAttachments" runat="server" Label="Attachments" OnFileUploaded="fuAttachments_FileUploaded" />
                        <div class="attachment">
                            <ul class="attchament-content">
                                <asp:Repeater ID="rptrAttachments" runat="server" OnItemCommand="rptrAttachments_ItemCommand">
                                    <ItemTemplate>
                                        <li><a href='<%# GetAttachmentUrl( (int)Eval("Key") )%>' target="_blank"><i class="fa fa-file-text-o"></i> <%# Eval("Value") %></a>
                                            <asp:LinkButton ID="lbRemoveAttachment" runat="server" CommandName="Remove" CommandArgument='<%# Eval("Key") %>'><i class="fa fa-times"></i></asp:LinkButton>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-sm-12">
                        <Rock:HtmlEditor ID="heCommMessage" runat="server" Label="Message" StartInCodeEditorMode="false" />
                    </div>
                </div>

                <div class="actions">
                    <asp:LinkButton ID="lbSend" runat="server" CssClass="btn btn-primary" Text="Send" OnClick="lbSend_Click" />
                    <asp:LinkButton ID="lbCancelSend" runat="server" CssClass="btn btn-link" Text="Cancel" CausesValidation="false" OnClick="lbCancelSend_Click"/>
                    <asp:LinkButton ID="lbPrint" runat="server" CssClass="btn btn-primary pull-right" Text="Print" OnClick="lbPrint_Click"/>
                </div>

            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>