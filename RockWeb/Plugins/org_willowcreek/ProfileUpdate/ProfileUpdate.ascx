<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProfileUpdate.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.ProfileUpdate.ProfileUpdate" %>

<link rel="stylesheet" href="/Styles/WillowCreek/ProfileUpdate.css" />
<style>
    .rock-check-box-list {
        margin: 0px;
    }

        .rock-check-box-list > .control-wrapper {
            visibility: hidden;
            height: 0px;
        }

    .rock-checkbox-icon {
        display: inline;
        padding-right: 25px;
        font-size: initial;
    }
</style>
<asp:UpdatePanel ID="upEditPerson" runat="server">
    <ContentTemplate>

        <div class="row">
            <div class="col-md-9">
                <h1>
                    <asp:Literal ID="lPageTitle" runat="server" Text="Profile Update" />
                </h1>
            </div>            
        </div>
        <div class="row">
             <div class="col-md-3"></div>
             <div class="col-md-9">
                <Rock:RockDropDownList ID="ddlLanguage" runat="server"  CssClass="input-width-md" Label="Language" OnSelectedIndexChanged="ddlLanguage_SelectedIndexChanged" AutoPostBack="true" />
            </div>
        </div>
        <div class="row">
            <div class="col-md-3">
                <Rock:ImageEditor ID="imgPhoto" runat="server" Label="Photo" BinaryFileTypeGuid="03BD8476-8A9F-4078-B628-5B538F967AFC" />
            </div>

            <div class="col-md-9">
                <asp:ValidationSummary ID="valValidation" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                <fieldset>
                    <Rock:RockDropDownList ID="ddlTitle" runat="server" CssClass="input-width-md" Label="Title" />
                    <Rock:DataTextBox ID="tbFirstName" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="FirstName" Required="true" />
                    <Rock:DataTextBox ID="tbNickName" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="NickName" Label="Nickname" />
                    <Rock:DataTextBox ID="tbLastName" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="LastName" Required="true" />
                    <Rock:RockDropDownList ID="ddlSuffix" CssClass="input-width-md" runat="server" Label="Suffix" />
                    <Rock:BirthdayPicker ID="bpBirthDay" runat="server" Label="Birthday" Required="true" />
                    <Rock:RockCheckBoxList ID="cblGender" runat="server" Label="Gender" Required="true">
                        <asp:ListItem Text="Male" Value="Male" />
                        <asp:ListItem Text="Female" Value="Female" />
                    </Rock:RockCheckBoxList>
                    <nobr>
                      <Rock:RockCheckBox ID="chkMale" CssClass="checks" runat="server" Text="Male" UnSelectedIconCssClass="fa fa-fw fa-square-o" SelectedIconCssClass="fa fa-fw fa-check-square" AutoPostBack="true" OnCheckedChanged="chkMale_CheckedChanged" DisplayInline="true" Required="true" />
                      <Rock:RockCheckBox ID="chkFemale" CssClass="checks" runat="server" Text="Female" UnSelectedIconCssClass="fa fa-fw fa-square-o" SelectedIconCssClass="fa fa-fw fa-check-square" AutoPostBack="true"  OnCheckedChanged="chkFemale_CheckedChanged" DisplayInline="true" Required="true" />
                    </nobr>
                    <Rock:RockRadioButtonList ID="rblMaritalStatus" runat="server" RepeatDirection="Horizontal" Label="Marital Status" Required="true" />
                </fieldset>
            </div>
        </div>
        <div class="row">
            <div class="col-md-3">
                <legend><asp:Literal ID="lContactInfo" runat="server" Text="Contact Info" /></legend>
            </div>
            <div class="col-md-9">
                <div class="form-horizontal">
                    <asp:Repeater ID="rContactInfo" runat="server">
                        <ItemTemplate>
                            <div class="form-group">
                                <div class="control-label col-sm-2"><asp:Literal ID="lPhonetype" runat="server" Text="" /></div>
                                <div class="controls col-sm-10">
                                    <div class="row">
                                        <div class="col-sm-7">
                                            <asp:HiddenField ID="hfPhoneType" runat="server" Value='<%# Eval("NumberTypeValueId")  %>' />
                                            <Rock:PhoneNumberBox ID="pnbPhone" runat="server" CountryCode='<%# Eval("CountryCode")  %>' Number='<%# Eval("NumberFormatted")  %>' />
                                        </div>
                                        <div class="col-sm-5">
                                            <div class="row">
                                                <div class="col-xs-6">
                                                    <asp:CheckBox ID="cbSms" runat="server" Text="sms" Checked='<%# (bool)Eval("IsMessagingEnabled") %>' CssClass="js-sms-number" />
                                                </div>
                                                <div class="col-xs-6">
                                                    <asp:CheckBox ID="cbUnlisted" runat="server" Text="unlisted" Checked='<%# (bool)Eval("IsUnlisted") %>' />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <Rock:DataTextBox ID="tbEmail" PrependText="<i class='fa fa-envelope-o'></i>" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="Email" />
            </div>
        </div>
        <div class="row">
            <div runat="server" visible="false">
                <div class="col-md-3">
                    <legend>Social Media</legend>
                </div>
                <div class="col-md-9">
                    <section class="panel panel-default">
                        <div class="panel-heading clearfix">
                            <div class="pull-left">
                                <asp:Literal ID="lCategoryName" runat="server" />
                            </div>
                            <div class="actions pull-right">
                                <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-default btn-xs" ToolTip="Edit Attributes" OnClick="lbEdit_Click"><i class="fa fa-pencil"></i> Edit</asp:LinkButton>
                            </div>
                        </div>
                        <div class="panel-body">
                            <Rock:HiddenFieldWithClass ID="hfAttributeOrder" runat="server" CssClass="js-attribute-values-order" />
                            <fieldset id="fsAttributes" runat="server" class="attribute-values"></fieldset>

                        </div>
                    </section>

                </div>
            </div>

            <Rock:NotificationBox ID="nbInvalidFamily" runat="server" Visible="false" />

            <div class="col-md-3">
                <legend><asp:Literal ID="lFamilyInformation" runat="server" Text="Family Information" /></legend>
            </div>
            <div class="col-md-9">
                <div class="panel panel-block" id="pnlEditFamily" runat="server">
                    <div class="panel-heading">
                        <h1 class="panel-title"><i class="fa fa-users"></i>
                            <asp:Literal ID="lBanner" runat="server" /></h1>
                    </div>
                    <div class="panel-body">



                        <div class="row">
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:CampusPicker ID="cpCampus" runat="server" Required="true" AutoPostBack="true" OnSelectedIndexChanged="cpCampus_SelectedIndexChanged" />
                                </fieldset>
                            </div>
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:TimePicker ID="tbPrimaryService" runat="server" Label="Primary Service" />
                                </fieldset>

                            </div>
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:RockDropDownList ID="ddlSection" CssClass="input-width-md" runat="server" Label="Primary Section" />
                                </fieldset>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:RockTextBox ID="tbFamilyName" runat="server" Label="Family Name" Required="false" CssClass="input-meduim" AutoPostBack="true" OnTextChanged="tbFamilyName_TextChanged" Visible="false" />
                                </fieldset>
                            </div>
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:RockDropDownList ID="ddlRecordStatus" runat="server" Label="Record Status" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ddlRecordStatus_SelectedIndexChanged" />
                                    <Rock:RockDropDownList ID="ddlReason" runat="server" Label="Reason" Visible="false" AutoPostBack="true" OnSelectedIndexChanged="ddlReason_SelectedIndexChanged"></Rock:RockDropDownList>
                                </fieldset>
                            </div>
                        </div>

                        <div class="panel panel-default editfamily-list">
                            <div class="panel-heading clearfix">
                                <div class="pull-left"><b>Family Members</b><br />
                                    <i><asp:Literal ID="lAddFamilyMemberNote" runat="server" Text="*Please add only your spouse and/or children who are part of your household." /></i></div>
                                <div class="pull-right">
                                    <asp:LinkButton ID="lbAddPerson" runat="server" Visible="true" CssClass="btn btn-primary" OnClick="lbAddPerson_Click" CausesValidation="false"><asp:Literal ID="lAddPerson" runat="server" Text="Add Person" /></asp:LinkButton>
                                </div>
                            </div>
                            <div class="panel-body">
                                <ul class="groupmembers">
                                    <asp:ListView ID="lvMembers" runat="server">
                                        <ItemTemplate>
                                            <li class="member">
                                                <div class="person-image" id="divPersonImage" runat="server"></div>
                                                <%#(string.IsNullOrEmpty(Eval("NickName").ToString()) ? Eval("FirstName") : Eval("NickName")) %><br />
                                                <%# Eval("LastName") %><br />
                                                <asp:LinkButton ID="btnEditFamilyMember" runat="server" Visible="true" Text="Edit" CommandName="EditPerson" CommandArgument='<%# Eval("Id") %>' />
                                            </li>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </ul>
                            </div>
                        </div>

                        <div class="panel panel-default">
                            <div class="panel-heading clearfix">
                                <div class="pull-left"><asp:Literal ID="lHomeAddress" runat="server" Text="Home Addresses" /></div>
                            </div>

                            <div class="panel-body">

                                <div class="grid grid-panel">
                                    <Rock:AddressControl ID="acHomeAddress" runat="server" RequiredErrorMessage="Your Home Address is Required" Required="true" />
                                </div>

                            </div>
                        </div>
                        <asp:ValidationSummary ID="valSummaryBottom" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
                        <div class="actions">
                            <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                            <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" OnClick="btnCancel_Click" />
                            <asp:LinkButton ID="btnDelete" runat="server" Text="Delete" CssClass="btn btn-link" OnClick="btnDelete_Click" CausesValidation="false" />
                        </div>
                        <div>
                            <p style="text-align: right;"><b><asp:Literal ID="lAdditionalProfileChangesNote" runat="server" Text="If you need assistance making changes to your profile, please click the following" />&nbsp;<a href="/page/493" target="_blank">link.</a></b></p>
                        </div>
                    </div>
                </div>
            </div>

            <Rock:ConfirmPageUnload ID="confirmExit" runat="server" ConfirmationMessage="Changes have been made to this family that have not yet been saved." Enabled="false" />

            <asp:HiddenField ID="hfActiveDialog" runat="server" />

            <Rock:ModalDialog ID="modalAddPerson" runat="server" Title="Add Person" ValidationGroup="AddPerson">

                <Content>

                    <asp:HiddenField ID="hfActiveTab" runat="server" />

                    <asp:ValidationSummary ID="valSummaryAddPerson" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" ValidationGroup="AddPerson" />

                    <div id="divNewPerson" runat="server" class="tab-pane active">
                        <div class="row">
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:RockTextBox ID="tbNewPersonFirstName" runat="server" Label="First Name" ValidationGroup="AddPerson" />
                                </fieldset>
                            </div>
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:RockTextBox ID="tbNewPersonLastName" runat="server" Label="Last Name" ValidationGroup="AddPerson" />
                                </fieldset>
                            </div>
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:RockTextBox ID="tbNewPersonNickName" runat="server" Label="Nickname" ValidationGroup="AddPerson" />
                                </fieldset>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:RockCheckBoxList ID="cblNewPersonGender" runat="server" Label="Gender" Required="true">
                                        <asp:ListItem Text="Male" Value="Male" />
                                        <asp:ListItem Text="Female" Value="Female" />
                                    </Rock:RockCheckBoxList>
                                    <nobr>
                                        <Rock:RockCheckBox ID="chkNewPersonMale" CssClass="checks" runat="server" Text="Male" UnSelectedIconCssClass="fa fa-fw fa-square-o" SelectedIconCssClass="fa fa-fw fa-check-square" AutoPostBack="true" OnCheckedChanged="chkNewPersonMale_CheckedChanged" DisplayInline="true" Required="true" />
                                        <Rock:RockCheckBox ID="chkNewPersonFemale" CssClass="checks" runat="server" Text="Female" UnSelectedIconCssClass="fa fa-fw fa-square-o" SelectedIconCssClass="fa fa-fw fa-check-square" AutoPostBack="true"  OnCheckedChanged="chkNewPersonFemale_CheckedChanged" DisplayInline="true" Required="true" />
					                </nobr>
                                </fieldset>
                            </div>
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:DatePicker ID="dpNewPersonBirthDate" runat="server" Label="Birthdate" ValidationGroup="AddPerson" />
                                </fieldset>
                            </div>
                            <div class="col-md-4">
                                <fieldset>
                                    <Rock:RockRadioButtonList ID="rblNewPersonRole" runat="server" DataTextField="Name" DataValueField="Id" RepeatDirection="Horizontal" Label="Role" ValidationGroup="AddPerson" OnSelectedIndexChanged="rblNewPersonRole_SelectedIndexChanged" AutoPostBack="true" />
                                </fieldset>
                            </div>
                        </div>
                        <div class="row">
                            <div class="rule5"></div>
                        </div>

                        <div class="row">
                            <div class="col-md-4">
                                <Rock:PhoneNumberBox ID="pnbNewPersonMobile" Label="Mobile" runat="server" CountryCode='<%# Eval("CountryCode")  %>' Number='<%# Eval("NumberFormatted")  %>' />
                            </div>
                            <div class="col-md-8">
                                <Rock:DataTextBox ID="tbNewPersonEmail" PrependText="<i class='fa fa-envelope-o'></i>" runat="server" SourceTypeName="Rock.Model.Person, Rock" PropertyName="Email" />
                            </div>
                        </div>
                        <div id="childDetail" runat="server" visible="false">
                            <div class="row">
                                <div class="col-md-4">
                                    <fieldset>
                                        <Rock:GradePicker ID="ddlGradePicker" runat="server" UseGradeOffsetAsValue="true" />
                                        <div class="col-sm-3 hidden">
                                            <Rock:YearPicker ID="ypGraduation" runat="server" Label="Graduation Year" Help="High School Graduation Year." />
                                        </div>
                                    </fieldset>
                                </div>
                                <div class="col-md-8">
                                    <fieldset>
                                        <Rock:RockTextBox ID="tbChildHealthNote" runat="server" Label="Health Note" ValidationGroup="AddPerson" />
                                    </fieldset>
                                </div>
                            </div>
                        </div>
                        <div id="adultDetail" runat="server" visible="false">
                            <div class="row">
                                <div class="col-md-4">
                                    <Rock:PhoneNumberBox ID="pnbNewPersonWork" Label="Work" runat="server" CountryCode='<%# Eval("CountryCode")  %>' Number='<%# Eval("NumberFormatted")  %>' />
                                </div>
                            </div>
                        </div>
                    </div>
                </Content>
            </Rock:ModalDialog>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
