<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VehicleDonationAdd.ascx.cs" Inherits="RockWeb.Plugins.org_willowcreek.Cars.VehicleDonationAdd" %>
<asp:UpdatePanel ID="upAddVehicle" runat="server">
    <ContentTemplate>

        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-car"></i> Make a Donation</h1>
            </div>
            <div class="panel-body">

                <asp:Panel ID="pnlVehicle" runat="server">

                    <asp:ValidationSummary ID="valSummaryTop" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

                    <Rock:RockRadioButtonList ID="rblDonationType" runat="server" RepeatDirection="Horizontal" Label="Donation Type" AutoPostBack="true" OnSelectedIndexChanged="rblDonationType_SelectedIndexChanged" />

                    <asp:Panel ID="pnlBusinessData" runat="server" CssClass="panel panel-block">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-building"></i> Business Information</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:RockTextBox ID="tbBusinessName" runat="server" Label="Business Name" Required="true" />
                                    <Rock:EmailBox ID="tbEmail" runat="server" Label="Business Email" />
                                    <Rock:PhoneNumberBox ID="pnWork" runat="server" Label="Work Phone Number" Required="true" />
                                </div>
                                <div class="col-md-6">
                                    <Rock:AddressControl ID="acAddress" runat="server" Label="Business Address" UseStateAbbreviation="true" UseCountryAbbreviation="false" Required="true" />
                                    <Rock:RockTextBox ID="tbEIN" runat="server" Label="EIN" />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <div class="panel panel-block">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-user"></i> Contact Information</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:RockTextBox ID="tbFirstName" runat="server" Label="First Name" Required="true" />
                                </div>
                                <div class="col-md-6">
                                    <Rock:RockTextBox ID="tbLastName" runat="server" Label="Last Name" Required="true" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:EmailBox ID="tbPersonalEmail" runat="server" Label="Email" Required="true" />
                                    <Rock:PhoneNumberBox ID="pnHome" runat="server" Label="Home Phone Number" Required="true" />
                                    <Rock:DatePicker ID="dpBirthdate" runat="server" Label="Birthdate" Required="true" />
                                </div>
                                <div class="col-md-6">
                                    <Rock:AddressControl ID="acPersonalAddress" runat="server" Label="Mailing Address" Required="true" UseStateAbbreviation="true" UseCountryAbbreviation="false" />
                                    <Rock:SSNBox ID="tbSSN" runat="server" Label="Social Security Number" Help="If you are using the vehicle as a tax write off, we will need your SSN in order to provide the appropriate tax documents." />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-block">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-car"></i> Donation Details</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:YearPicker ID="ypYear" runat="server" Label="Vehicle Year" Required="true" />
                                    <Rock:RockDropDownList ID="ddlMake" runat="server" Label="Make" Required="true" AutoPostBack="true" OnSelectedIndexChanged="ddlMake_SelectedIndexChanged" />
                                    <Rock:RockDropDownList ID="ddlModel" runat="server" Label="Model" Required="true" />
                                    <Rock:RockTextBox ID="dtvin" runat="server" Label="VIN" />
                                    <Rock:RockTextBox ID="tbMileage" runat="server" Label="Mileage" />
                                </div>
                                <div class="col-md-6">
                                    <Rock:RockDropDownList ID="ddlBodyStyle" runat="server" Label="Body Style" />
                                    <Rock:RockDropDownList ID="ddlColor" runat="server" Label="Color" />
                                    <Rock:ImageUploader ID="fuPhoto" runat="server" Label="Photo" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <Rock:RockTextBox ID="tbNote" runat="server" Label="Any Additional Notes" TextMode="MultiLine" Rows="3" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-block">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-truck"></i> Pickup Information</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-md-6">
                                    <Rock:RockRadioButtonList ID="rblPickup" runat="server" RepeatDirection="Horizontal" Label="Pickup Location" AutoPostBack="true" OnSelectedIndexChanged="rblPickupSameAsDonor_SelectedIndexChanged" DataValueField="Key" DataTextField="Value" />
                                </div>
                                <div class="col-md-6">
                                    <Rock:AddressControl ID="acPickupLocation" runat="server" Label="Pickup Address" UseStateAbbreviation="true" UseCountryAbbreviation="false" Visible="false" />
                                </div>
                            </div>
                        </div>
                    </div
                        
                    <div class="actions">
                        <asp:Button ID="btnFinish" runat="server" Text="Finish" CssClass="btn btn-primary" OnClick="btnFinish_Click" />
                    </div>

                    <Rock:NotificationBox ID="nbError" runat="server" Visible="false" NotificationBoxType="Danger" Heading="Oops..." />
                </asp:Panel>

                <asp:Panel ID="pnlResult" runat="server" Visible="false">
                    <asp:Literal ID="lResultMessage" runat="server" />
                </asp:Panel>

            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
