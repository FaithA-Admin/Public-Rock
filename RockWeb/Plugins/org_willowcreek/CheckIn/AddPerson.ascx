<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddPerson.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.CheckIn.AddPerson" %>

<style>
    .form-control {
        font-size: 28px;
        height: inherit;
    }

    .split-date-picker {
        font-size: 1.5em;
    }

    .split-date-picker select {
        display: inline;
        width: 28%;
    }

    select {
        -moz-appearance: none; /* hide dropdown button in firefox */
        -webkit-appearance: none; /* hide dropdown button in chrome */
    }

    select::-ms-expand {
        display: none; /* hide dropdown button in IE */
    }





    /*.datepicker {
        color: #000;
    }*/


    

    input[type=radio],
    input[type='checkbox'] {
        display: none;
    }

    input[type=radio] + label {
        display: inline;
        font-size: 28px;
        padding-right: 28px;
    }

    input[type='radio'] + label:before {
        display: inline-block;
        font-family: FontAwesome;
        font-style: normal;
        font-weight: normal;
        line-height: 1;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        padding-right: 18px;
        width: 60px;
        font-size: 60px;
        vertical-align: middle;
    }

    input[type=radio] + label:before {
        content: "\f096"; /* Checkbox Unchecked */
    }

    input[type=radio]:checked + label:before {
        content: "\f14a"; /* Checkbox Checked */
    }

    .radio label,
    .checkbox label {
        padding-left: 0;
    }



    .rock-check-box-list {
        margin:0px;
    }
    .rock-check-box-list>.control-wrapper {
        visibility:hidden;
        height:0px;
    }
    .rock-checkbox-icon {
        font-size:xx-large;
        display:inline;
        padding-right:25px;
    }
</style>

<asp:UpdatePanel ID="upContent" runat="server">
    <ContentTemplate>
        
        <asp:HiddenField ID="hfSameFamily" runat="server" />

        <asp:Panel ID="pnlParent" runat="server">
            <div class="checkin-body">
                <div class="checkin-search-actions checkin-start">
                    <div class="checkin-header">
                        <h1>Are you the parent or legal guardian of this child?</h1>
                    </div>
                    <asp:LinkButton CssClass="btn btn-primary btn-checkin" ID="lbYes" OnClick="lbYes_Click" runat="server"><span>Yes</span></asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;
                    <asp:LinkButton CssClass="btn btn-primary btn-checkin" ID="lbNo" OnClick="lbNo_Click" runat="server"><span>No</span></asp:LinkButton>
                </div>
            </div>
            <div class="checkin-footer">
                <div class="checkin-actions">
                    <asp:LinkButton CssClass="btn btn-default" ID="lbParentBack" runat="server" Text="Back" OnClick="lbParentBack_Click" CausesValidation="false" />
                    <asp:LinkButton CssClass="btn btn-default" ID="lbParentCancel" runat="server" Text="Cancel" OnClick="lbParentCancel_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlChild" runat="server" Visible="false">
            <Rock:ModalAlert ID="maWarning" runat="server" />
            <div class="checkin-body">
                <div class="checkin-header">
                    <h1>Child Information</h1>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <Rock:RockTextBox ID="tbFirstName" Label="First Name" runat="server" onClick="this.select();" />
                    </div>
                    <div class="col-md-4">
                        <Rock:RockTextBox ID="tbLastName" Label="Last Name" runat="server" onClick="this.select();" />
                    </div>
                    <div class="col-md-4">
                        <Rock:RockTextBox ID="tbAllergy" Label="Allergies / Health Notes" runat="server" Placeholder="None" onClick="this.select();" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
<%--                        <div class="form-group required">
                            <label CssClass="control-label">Gender</label>
                            <div class="control-wrapper">
                                <nobr>
                                <input type="radio" name="gender" id="male" value="male">
                                <label for="male">Male</label>

                                <input type="radio" name="gender" id="female" value="female">
                                <label for="female">Female</label>
                                </nobr>
                            </div>
                        </div>--%>
<%--                        <Rock:RockRadioButtonList ID="rblGender" runat="server" Label="Gender" Required="true">
                            <asp:ListItem Text="Male" Value="Male" />
                            <asp:ListItem Text="Female" Value="Female" />
                        </Rock:RockRadioButtonList>--%>
<%--                        <Rock:RockRadioButton runat="server" GroupName="Gender" DisplayInline="true" Text="Male" UnSelectedIconCssClass="fa fa-fw fa-square-o fa-lg"
                                        SelectedIconCssClass="fa fa-fw fa-check-square fa-lg" />
                        <Rock:RockRadioButton runat="server" GroupName="Gender" DisplayInline="true" Text="Female" UnSelectedIconCssClass="fa fa-fw fa-square-o fa-lg"
                                        SelectedIconCssClass="fa fa-fw fa-check-square fa-lg" />--%>
                        <Rock:RockCheckBoxList ID="cblGender" runat="server" Label="Gender" >
                            <asp:ListItem Text="Male" Value="Male" />
                            <asp:ListItem Text="Female" Value="Female" />
                        </Rock:RockCheckBoxList>
                        <nobr>
                        <Rock:RockCheckBox ID="chkMale" CssClass="checks" runat="server" Text="Male" UnSelectedIconCssClass="fa fa-fw fa-square-o" SelectedIconCssClass="fa fa-fw fa-check-square" AutoPostBack="true" OnCheckedChanged="chkMale_CheckedChanged" DisplayInline="true" />
                        <Rock:RockCheckBox ID="chkFemale" CssClass="checks" runat="server" Text="Female" UnSelectedIconCssClass="fa fa-fw fa-square-o" SelectedIconCssClass="fa fa-fw fa-check-square" AutoPostBack="true"  OnCheckedChanged="chkFemale_CheckedChanged" DisplayInline="true" />
                            </nobr>
                    </div>
                    <div class="col-md-4">
                        <Rock:SplitDatePicker ID="dpBirthdate" runat="server" Label="Birth Date" MaxYearOffset="0" />
                    </div>
                    <div class="col-md-4">
                        <Rock:RockDropDownList ID="ddlGrade" runat="server" Label="Grade" DataValueField="Key" DataTextField="Value" />
                    </div>
                </div>
                

                
            </div>
            <div class="checkin-footer">
                <div class="checkin-actions">
                    <asp:LinkButton CssClass="btn btn-default" ID="lbBack" runat="server" OnClick="lbBack_Click" Text="Back" CausesValidation="false" />
                    <asp:LinkButton CssClass="btn btn-default" ID="lbCancel" runat="server" OnClick="lbCancel_Click" Text="Cancel" CausesValidation="false" />
                    <asp:LinkButton CssClass="btn btn-primary pull-right" ID="lbNext" runat="server" OnClick="lbNext_Click" Text="Next" />
                </div>
            </div>
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
