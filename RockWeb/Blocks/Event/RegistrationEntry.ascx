<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RegistrationEntry.ascx.cs" Inherits="RockWeb.Blocks.Event.RegistrationEntry" %>

<style>
    iframe {
        width: 100%;
        height: 800px;
        overflow: hidden;
        border-style: none;
    }
</style>
<asp:UpdatePanel ID="upnlContent" runat="server">
<ContentTemplate>

    <asp:HiddenField ID="signatureFormRequired" ClientIDMode="Static" runat="server" value="" />
    <asp:HiddenField ID="hfTriggerScroll" runat="server" Value="" />
    <asp:HiddenField ID="hfAllowNavigate" runat="server" Value="" />

    <asp:ValidationSummary ID="vsSummary" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />
    <Rock:NotificationBox ID="nbPaymentValidation" runat="server" NotificationBoxType="Danger" Visible="false" />

    <Rock:NotificationBox ID="nbMain" runat="server" Visible="false"></Rock:NotificationBox>
    <Rock:NotificationBox ID="nbWaitingList" runat="server" Visible="false" NotificationBoxType="Warning" />

    <asp:Panel ID="pnlHowMany" runat="server" Visible="false" CssClass="registrationentry-intro">

        <h1>How many <asp:Literal ID="lRegistrantTerm" runat="server" /> will you be registering?</h1>
        <Rock:NumberUpDown ID="numHowMany"  runat="server" CssClass="input-lg" OnNumberUpdated="numHowMany_NumberUpdated"  />

        <div class="actions">
            <Rock:BootstrapButton ID="lbHowManyNext" runat="server" AccessKey="n" ToolTip="Alt+n" Text="Next" DataLoadingText="Next" CssClass="btn btn-primary pull-right" CausesValidation="true" OnClick="lbHowManyNext_Click" />
        </div>

    </asp:Panel>

    <asp:Panel ID="pnlRegistrant" runat="server" Visible="false" CssClass="registrationentry-registrant">

        
        <h1>
            <asp:Literal ID="lRegistrantTitle" runat="server" />
        </h1>        

        <Rock:NotificationBox ID="nbType" runat="server" NotificationBoxType="Warning"  />

        <asp:Panel ID="pnlRegistrantProgressBar" runat="server" CssClass="clearfix">
            <div class="progress">
                <div class="progress-bar" role="progressbar" aria-valuenow="<%=this.PercentComplete%>" aria-valuemin="0" aria-valuemax="100" style="width: <%=this.PercentComplete%>%;">
                    <span class="sr-only"><%=this.PercentComplete%>% Complete</span>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel id="pnlRegistrantFields" runat="server" >
                       
            <asp:Panel ID="pnlFamilyOptions" runat="server" CssClass="well js-registration-same-family">
                <Rock:RockRadioButtonList ID="rblFamilyOptions" runat="server" Label="Individual is in the same immediate family as" RepeatDirection="Vertical" Required="true" RequiredErrorMessage="Answer to which family is required." DataTextField="Value" DataValueField="Key" />
            </asp:Panel>
        
            <asp:Panel ID="pnlFamilyMembers" runat="server" Visible="false" CssClass="row" >
                <div class="col-md-6">
                    <Rock:RockDropDownList ID="ddlFamilyMembers" runat="server" Label="Family Member" AutoPostBack="true" OnSelectedIndexChanged="ddlFamilyMembers_SelectedIndexChanged" />
                </div>
            </asp:Panel>

            <asp:PlaceHolder ID="phRegistrantControls" runat="server" />
        
            <div id="divFees" runat="server" class="well registration-additional-options">
                <h4><asp:Literal ID="lRegistrantFeeCaption" runat="server" /></h4>
                <asp:PlaceHolder ID="phFees" runat="server" />
            </div>

        </asp:Panel>

        <asp:Panel id="pnlDigitalSignature" runat="server" visible="false">
            <Rock:NotificationBox ID="nbDigitalSignature" runat="server" NotificationBoxType="Info"></Rock:NotificationBox>
            <asp:HiddenField ID="hfRequiredDocumentLinkUrl" runat="server" />
            <asp:HiddenField ID="hfRequiredDocumentQueryString" runat="server" />
            <asp:HiddenField ID="hfRegistrantGuid" runat="server" ClientIDMode="Static" />
            <iframe id="iframeRequiredDocument" frameborder="0" runat="server" ClientIDMode="Static"></iframe>
            <asp:Button id="btnRequiredDocument" runat="server" Visible="false" OnClientClick="var win = window.open($('input[id$=hfRequiredDocumentLinkUrl]').val(), '_blank');win.focus(); return false;" Text="Open Document" CssClass="btn btn-default pull-right"></asp:Button>
            <span style="display:none" >
                <asp:LinkButton ID="lbRequiredDocumentNext" runat="server" Text="Required Document Return" OnClick="lbRequiredDocumentNext_Click" CausesValidation="false" ></asp:LinkButton>
            </span>

        </asp:Panel>

        <div class="actions">
            <asp:LinkButton ID="lbRegistrantPrev" runat="server" AccessKey="p" ToolTip="Alt+p" Text="Previous" CssClass="btn btn-default" CausesValidation="false" OnClick="lbRegistrantPrev_Click"  />
            <Rock:BootstrapButton ID="lbRegistrantNext" runat="server" AccessKey="n" ToolTip="Alt+n" Text="Next" DataLoadingText="Next" CssClass="btn btn-primary pull-right" CausesValidation="true" OnClick="lbRegistrantNext_Click" />
        </div>

    </asp:Panel>

    <asp:Panel ID="pnlSummaryAndPayment" runat="server" Visible="false" CssClass="registrationentry-summary">
        
        <h1><asp:Literal ID="lSummaryAndPaymentTitle" runat="server" /></h1>

        <asp:Panel ID="pnlSummaryAndPaymentProgressBar" runat="server">
            <div class="progress">
                <div class="progress-bar" role="progressbar" aria-valuenow="<%=this.PercentComplete%>" aria-valuemin="0" aria-valuemax="100" style="width: <%=this.PercentComplete%>%;">
                    <span class="sr-only"><%=this.PercentComplete%>% Complete</span>
                </div>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlRegistrarInfo" runat="server" CssClass="well">
          
            <asp:Literal ID="lSummaryText" runat="server" />
            <asp:Literal ID="lSummaryTextDebug" runat="server" Visible="false" />
                        
            <h4>This <asp:Literal id="lRegistrationTerm" runat="server" /> Was <asp:Literal ID="lRegistrationCompletedByTerm" runat="server" /> By</h4>
            <div class="row">
                 <div class="col-md-6">
                    <asp:Panel ID="pnlRegistrarDonorIsBusiness" runat="server" Visible="false">
                        <Rock:RockCheckBox ID="cbRegistrarDonorIsBusiness" runat="server" Label="Is this donation being made on behalf of a Business?" Text="Yes" Help="Check this box if you will be filling the form on behalf of a business, otherwise leave blank." AutoPostBack="true" Required="false" RequiredErrorMessage="Answer to filling this form on behalf of a business is required." OnCheckedChanged="cbRegistrarDonorIsBusiness_CheckChanged"/>
                    </asp:Panel>
                 </div>
            </div>           
            <div class="row">
                <div class="col-md-6">
                    <Rock:RockTextBox ID="tbYourFirstName" runat="server" Label="First Name" CssClass="js-your-first-name" Required="true" />
                </div>
                <div class="col-md-6">
                    <Rock:RockTextBox ID="tbYourLastName" runat="server" Label="Last Name" Required="true" />
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <asp:Panel ID="pnlRegistrarDateOfBirth" runat="server" Visible="false">
                        <Rock:BirthdayPicker ID="bpDOB" runat="server" Label="Date of Birth" Required="false" />
                    </asp:Panel>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <asp:Panel ID="pnlRegistrarDonorAddress" runat="server" Visible="false">
                        <Rock:AddressControl ID="acYourAddress" runat="server" Label="Address" Required="true" />
                    </asp:Panel>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <Rock:EmailBox ID="tbConfirmationEmail" runat="server" Label="Please Enter Your Email Address" Required="true" />
                    <Rock:RockCheckBox ID="cbUpdateEmail" runat="server" Text="Should Your Account Be Updated To Use This Email Address?" Visible="false" Checked="true" />
                    <asp:Literal ID="lUpdateEmailWarning" runat="server" Text="Note: Your account will automatically be updated with this email address." Visible="false" />
                </div>
                <div class="col-md-6">
                    <asp:Panel ID="pnlRegistrarFamilyOptions" runat="server" CssClass="js-registration-same-family">
                        <Rock:RockRadioButtonList ID="rblRegistrarFamilyOptions" runat="server" Label="You are in the same immediate family as" RepeatDirection="Horizontal" Required="true" DataTextField="Value" DataValueField="Key" RequiredErrorMessage="Answer to which family is required." />
                    </asp:Panel>
                </div>
            </div>

        </asp:Panel>
        
        <asp:Panel ID="pnlRegistrantsReview" CssClass="margin-b-md" runat="server" Visible="false">
            <asp:Literal ID="lRegistrantsReview" runat="server" />
            <ul>
                <asp:Repeater ID="rptrRegistrantsReview" runat="server">
                    <ItemTemplate>
                        <li><strong> <%# Eval("RegistrantName")  %></strong></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </asp:Panel>
        <asp:Panel ID="pnlWaitingListReview" CssClass="margin-b-md" runat="server" Visible="false">
            <asp:Literal ID="lWaitingListReview" runat="server" />
            <ul>
                <asp:Repeater ID="rptrWaitingListReview" runat="server">
                    <ItemTemplate>
                        <li><strong> <%# Eval("RegistrantName")  %></strong></li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </asp:Panel>     
        
        <asp:Panel ID="pnlCostAndFees" runat="server">

            <h4>Payment Summary</h4>
                
            <Rock:NotificationBox ID="nbDiscountCode" runat="server" Visible="false" NotificationBoxType="Warning"></Rock:NotificationBox>
                
            <div class="clearfix">
                <div id="divDiscountCode" runat="server" class="form-group pull-right">
                    <label class="control-label"><asp:Literal ID="lDiscountCodeLabel" runat="server" /></label>
                    <div class="input-group">
                        <asp:TextBox ID="tbDiscountCode" runat="server" CssClass="form-control input-width-md input-sm"></asp:TextBox>
                        <asp:LinkButton ID="lbDiscountApply" runat="server" CssClass="btn btn-default btn-sm margin-l-sm" Text="Apply" OnClick="lbDiscountApply_Click" CausesValidation="false"></asp:LinkButton>
                    </div>
                </div>
            </div>

            <div class="fee-table">
                <asp:Repeater ID="rptFeeSummary" runat="server">
                    <HeaderTemplate>
                        <div class="row hidden-xs fee-header">
                            <div class="col-sm-6">
                                <strong>Description</strong>
                            </div>
                                
                            <div runat="server" class="col-sm-3 fee-value" visible='<%# (RegistrationState.DiscountPercentage > 0.0m) %>'>
                                <strong>Discounted Amount</strong>
                            </div>

                            <div class="col-sm-3 fee-value">
                                <strong>Amount</strong>
                            </div>
                                
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="row fee-row-<%# Eval("Type").ToString().ToLower() %>">
                            <div class="col-sm-6 fee-caption">
                                <%# Eval("Description") %>
                            </div>
                                
                            <div runat="server" class="col-sm-3 fee-value" visible='<%# (RegistrationState.DiscountPercentage > 0.0m) %>'>
                                <span class="visible-xs-inline">Discounted Amount:</span> <%# Rock.Web.Cache.GlobalAttributesCache.Value( "CurrencySymbol" )%> <%# string.Format("{0:N}", Eval("DiscountedCost")) %> 
                            </div>

                            <div class="col-sm-3 fee-value">
                                <span class="visible-xs-inline">Amount:</span> <%# Rock.Web.Cache.GlobalAttributesCache.Value( "CurrencySymbol" )%> <%# string.Format("{0:N}", Eval("Cost")) %> 
                            </div>
                                    
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="row fee-totals">
                <div class="col-sm-offset-8 col-sm-4 fee-totals-options">
                    <asp:HiddenField ID="hfTotalCost" runat="server" />
                    <Rock:RockLiteral ID="lTotalCost" runat="server" Label="Total Cost" />

                    <asp:HiddenField ID="hfPreviouslyPaid" runat="server" />
                    <Rock:RockLiteral ID="lPreviouslyPaid" runat="server" Label="Previously Paid" />
                    
                    <%-- For Partial Payments... --%>

                    <asp:HiddenField ID="hfMinimumDue" runat="server" />
                    <Rock:RockLiteral ID="lMinimumDue" runat="server" Label="Minimum Due Today" />
                    
                    <div class="form-right">
                        <Rock:CurrencyBox ID="nbAmountPaid" runat="server" CssClass="input-width-md amount-to-pay" NumberType="Currency" Label="Amount To Pay Today" Required="true" />
                    </div>
                                 
                    <Rock:RockLiteral ID="lRemainingDue" runat="server" Label="Amount Remaining" />


                    <%-- For Payoff --%>
                    
                    <Rock:RockLiteral ID="lAmountDue" runat="server" Label="Amount Due" />
                </div>
            </div>
                
        </asp:Panel>

        <asp:Panel ID="pnlPaymentInfo" runat="server" CssClass="well">

            <asp:Literal ID="lPaymentInfoTitle" runat="server" />

            <Rock:RockRadioButtonList ID="rblSavedCC" runat="server" CssClass="radio-list margin-b-lg" RepeatDirection="Vertical" DataValueField="Id" DataTextField="Name" />
            
            <div id="divNewCard" runat="server" class="radio-content">
                <Rock:RockTextBox ID="txtCardFirstName" runat="server" Label="First Name on Card" Visible="false" Required="true" ></Rock:RockTextBox>
                <Rock:RockTextBox ID="txtCardLastName" runat="server" Label="Last Name on Card" Visible="false" Required="true" ></Rock:RockTextBox>
                <Rock:RockTextBox ID="txtCardName" runat="server" Label="Name on Card" Visible="false" Required="true" ></Rock:RockTextBox>
                <Rock:RockTextBox ID="txtCreditCard" runat="server" Label="Credit Card #" MaxLength="19" CssClass="credit-card" Required="true" />
                <ul class="card-logos list-unstyled">
                    <li class="card-visa"></li>
                    <li class="card-mastercard"></li>
                    <li class="card-discover"></li>
                    <li class="card-amex"></li>
                </ul>
                <div class="row">
                    <div class="col-sm-6">
                        <Rock:MonthYearPicker ID="mypExpiration" runat="server" Label="Expiration Date" Required="true" />
                    </div>
                    <div class="col-sm-6">
                        <Rock:RockTextBox ID="txtCVV" Label="Card Security Code" CssClass="input-width-xs" runat="server" MaxLength="4" Required="true" />
                    </div>
                </div>
                <Rock:AddressControl ID="acBillingAddress" runat="server" Label="Billing Address" UseStateAbbreviation="true" UseCountryAbbreviation="false" ShowAddressLine2="false" Required="true" />
                <Rock:RockLiteral ID="lAmountPaidConfirmation" runat="server" Label="This is the amount that will be charged to your credit card" Visible="false" />
                <div id="myWellLogo" runat="server" visible="false" style="text-align: left;">
                    <p>
                        Credit Card processing powered by My Well
                        <br />
                        <a href="http://mywell.org" target="_blank">
                            <img src="/Assets/Images/MyWellLogo.png" alt="My Well logo" style="width: 140px">
                        </a>
                    </p>
                </div>
            </div>

        </asp:Panel>

        <div class="actions">
            <asp:LinkButton ID="lbSummaryPrev" runat="server" AccessKey="p" ToolTip="Alt+p" Text="Previous" CssClass="btn btn-default" CausesValidation="false" OnClick="lbSummaryPrev_Click" />
            <Rock:BootstrapButton ID="lbSummaryNext" runat="server" AccessKey="n" ToolTip="Alt+n" Text="Finish" DataLoadingText="Next" CssClass="btn btn-primary pull-right" CausesValidation="true" OnClick="lbSummaryNext_Click" />
            <asp:LinkButton ID="lbPaymentPrev" runat="server" AccessKey="p" ToolTip="Alt+p" Text="Previous" CssClass="btn btn-default" CausesValidation="false" OnClick="lbPaymentPrev_Click" />
            <asp:Label ID="aStep2Submit" runat="server" ClientIDMode="Static" CssClass="btn btn-primary pull-right" Text="Finish" />
        </div>

        <iframe id="iframeStep2" src="<%=this.Step2IFrameUrl%>" style="display:none"></iframe>

        <asp:HiddenField ID="hfStep2AutoSubmit" runat="server" Value="false" />
        <asp:HiddenField ID="hfStep2Url" runat="server" />
        <asp:HiddenField ID="hfStep2ReturnQueryString" runat="server" />
        <span style="display:none" >
            <asp:LinkButton ID="lbStep2Return" runat="server" Text="Step 2 Return" OnClick="lbStep2Return_Click" CausesValidation="false" ></asp:LinkButton>
        </span>

    </asp:Panel>

    <asp:Panel ID="pnlSuccess" runat="server" Visible="false" >
        
        <h1><asp:Literal ID="lSuccessTitle" runat="server" /></h1>

        <asp:Panel ID="pnlSuccessProgressBar" runat="server">
            <div class="progress">
                <div class="progress-bar" role="progressbar" aria-valuenow="<%=this.PercentComplete%>" aria-valuemin="0" aria-valuemax="100" style="width: <%=this.PercentComplete%>%;">
                    <span class="sr-only"><%=this.PercentComplete%>% Complete</span>
                </div>
            </div>
        </asp:Panel>

        <asp:Literal ID="lSuccess" runat="server" />
        <asp:Literal ID="lSuccessDebug" runat="server" Visible="false" />

        <asp:Panel ID="pnlSaveAccount" runat="server" Visible="false">
            <div class="well">
                <legend>Make Payments Even Easier</legend>
                <fieldset>
                    <Rock:RockCheckBox ID="cbSaveAccount" runat="server" Text="Save account information for future transactions" CssClass="toggle-input" />
                    <div id="divSaveAccount" runat="server" class="toggle-content">
                        <Rock:RockTextBox ID="txtSaveAccount" runat="server" Label="Name for this account" CssClass="input-large"></Rock:RockTextBox>

                        <asp:PlaceHolder ID="phCreateLogin" runat="server" Visible="false">

                            <div class="control-group">
                                <div class="controls">
                                    <div class="alert alert-info">
                                        <b>Note:</b> For security purposes you will need to login to use your saved account information.  To create
	    			                a login account please provide a user name and password below. You will be sent an email with the account 
	    			                information above as a reminder.
                                    </div>
                                </div>
                            </div>

                            <Rock:RockTextBox ID="txtUserName" runat="server" Label="Username" CssClass="input-medium" />
                            <Rock:RockTextBox ID="txtPassword" runat="server" Label="Password" CssClass="input-medium" TextMode="Password" />
                            <Rock:RockTextBox ID="txtPasswordConfirm" runat="server" Label="Confirm Password" CssClass="input-medium" TextMode="Password" />

                        </asp:PlaceHolder>

                        <Rock:NotificationBox ID="nbSaveAccount" runat="server" Visible="false" NotificationBoxType="Danger"></Rock:NotificationBox>

                        <div id="divSaveActions" runat="server" class="actions">
                            <asp:LinkButton ID="lbSaveAccount" runat="server" Text="Save Account" CssClass="btn btn-primary" OnClick="lbSaveAccount_Click" />
                        </div>
                    </div>
                </fieldset>                    
            </div>
        </asp:Panel>

    </asp:Panel>
     <script>        
            $(document).ready(function () {
                var safariBrowser = (navigator.userAgent.indexOf('Safari') != -1 && navigator.userAgent.indexOf('Chrome') == -1);
                if (safariBrowser && ($('#signatureFormRequired').val() == "True")) {
                    if (!(getParameterByName('redirect') == "no")) {
                        window.location.replace("https://mw.signnow.com/setcookie?redirect_uri=" + encodeURIComponent(window.location.href + "&redirect=no"));
                    }
                }

                function getParameterByName(name, url) {
                    if (!url) {
                        url = window.location.href;
                    }
                    name = name.replace(/[\[\]]/g, "\\$&");
                    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                        results = regex.exec(url);
                    if (!results) return null;
                    if (!results[2]) return '';
                    return decodeURIComponent(results[2].replace(/\+/g, " "));
                }
            });
        </script>   
</ContentTemplate>
</asp:UpdatePanel>
