<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuestionnaireReferences.ascx.cs" Inherits="Plugins_org_willowcreek_ProtectionApp_QuestionnaireReferences" %>
<link href="/Styles/WillowCreek/ProtectionApp.css" type="text/css" rel="stylesheet" />
<script src="/scripts/WillowRockReferenceList.js" type="text/javascript"></script>
<script src="/scripts/jquery.maskedinput.min.js" type="text/javascript"></script>

<noscript>
    <style type="text/css">
        #noscript-warning {
            font-family: sans-serif;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            z-index: 101;
            text-align: center;
            font-weight: bold;
            font-size: 120%;
            color: #fff;
            background-color: #ae0000;
            padding: 5px 0 5px 0;
        }

        .frm-pane-wrapper {
            display: none;
        }
    </style>
    <div id="noscript-warning">Protection Application works best with JavaScript enabled</div>
    <div class="noscriptmsg">
        <h4>Browser Compatability Issue</h4>
        Unfortunately you don't have JavaScript enabled in this browser, our Protection Application requires JavaScript to be enabled.
        <br />
        <br />
        Please either enable JavaScript or use a different browser, we suggest <a href="https://www.google.com/chrome/browser/desktop/">Google Chrome</a>
    </div>
</noscript>

<asp:Panel ID="pnlAppDone" runat="server" ClientIDMode="Static">
    <p>This application has already been completed or the link has expired.</p>
    <p>Please contact <a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a> with any questions regarding this process.</p>
</asp:Panel>

<asp:Panel ID="pnlWrongUser" runat="server" ClientIDMode="Static">
    <p>The application links are unique to an individual.</p>
    <p>Please contact <a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a> with any questions regarding this process.</p>
</asp:Panel>

<asp:Panel ID="pnlQuestionnaire" runat="server">
    <div id="willow_pa_applicant_form" class="frm-pane-wrapper" style="width: 90%;">
        <fieldset id="willow_pa_reference_form" class="frm-pane">
            <legend>References</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    <span class="note">Please supply three (3) references that:</span><br />
                    <span class="note" style="text-indent: each-line">- are over 18</span><br />
                    <span class="note" style="text-indent: each-line">- must be a non-relative (cannot be spouse, fiancé, in-laws or step family)</span><br />
                    <span class="note" style="text-indent: each-line">- have known you for over a year</span><br />
                    <span class="note" style="text-indent: each-line">- have a definite knowledge of your character</span><br />
                    <span class="note" style="text-indent: each-line"><b>Please follow up with your three references to ensure that they have received the reference email and complete your reference request.</b></span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    <div id="referenceList">
                        <div class="row">
                            <div class="col-md-3">
                                <label>Name</label>
                            </div>
                            <div class="col-md-2">
                                <label>Association</label>
                            </div>
                            <div class="col-md-4">
                                <label>Contact</label>
                            </div>
                            <div class="col-md-2">
                                <label>Status</label>
                            </div>
                            <div class="col-md-1"></div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="refPane" style="width: 60%">
                <div class="row form-group">
                    <div class="col-md-6">
                        <label>Name:</label>
                    </div>
                    <div class="col-md-6">
                        <input type="text" id="RefName" name="RefName" class="form-control" required data-validation-message="Reference Name is required" />
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-6">
                        <label>Nature of Association:</label>
                    </div>
                    <div class="col-md-6">
                        <select id="RefAssociation" name="RefAssociation" class="form-control" required data-validation-message="Reference Nature of Association is required">
                            <option value="">Select...</option>
                            <option value="Friend">Friend</option>
                            <option value="Colleague">Colleague</option>
                            <option value="Supervisor">Past or Current Supervisor</option>
                            <option value="mentor">Past or Present Mentor</option>
                            <option value="Family">Family</option>
                        </select>
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-6">
                        <label>Email (required):</label>
                    </div>
                    <div class="col-md-6">
                        <input type="email" id="RefEmail" name="RefEmail" class="form-control" data-role="email" placeholder="email@domain.com" required data-validation-message="Reference Email is required" />
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-12">
                        If you do not know your reference's email address, please contact them to retrieve this information before moving forward. If your reference does not have an email address, please use another reference. Using an incorrect email address will delay the process.
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-12" style="text-align: right;">
                        <span style="display: none;"><span class="refCount">0</span> of 3 provided</span>
                        <button id="btnAddRef" class="btn btn-primary">Add</button>
                    </div>
                </div>
            </div>
        </fieldset>

        <fieldset class="frm-pane" style="display:none; width: 60%">
            <legend>Release of Information</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    <input id="chkAuthReleaseRef" name="chkAuthReleaseRef" type="checkbox" />
                    I authorize any references on the previous page to give you any information (including opinions) that they may have regarding my character and fitness for working with children, students or vulnerable adults.
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-4">
                    <label>Applicant's Signature</label>
                </div>
                <div class="col-md-8">
                    <input type="text" id="ApplicantSignature" class="form-control" required data-validation-message="Applicant's signature is required" placeholder="Please type full legal name" />
                    <input type="text" id="FullLegalName" name="FullLegalName" class="form-control" value="<%=FullLegalName %>" placeholder="Full Legal Name" disabled style="display:none" />

                    <input type="text" id="ApplicantSignatureDate" class="form-control" value="<%=DateTime.Now.ToString("M/d/yyyy") %>" disabled style="width: 100px" />
                </div>
            </div>
        </fieldset>
        <fieldset class="frm-pane">
            <legend>Thank you for your submission!</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    <p>Thank you for taking the time to fill out this form! Your ministry should be in contact with you about any next steps/follow-up.</p>
                    <p>If you would like to explain any information you provided in more depth or if you have any questions, feel free to contact the Protection Department directly.</p>
                    <p><a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a></p>
                    <p>224-512-1920</p>
                </div>
            </div>
        </fieldset>

        <div class="row form-group ResendText" runat="server" id="ResendText">
            <div class="col-md-12">
                If the above references are correct please click 'Continue' to resend the reference request emails.
            </div>
        </div>
        <div id="willow_pa_reference_buttons">
            <button id="btnReferenceBack" class="btn" style="display: none;">Back</button>
            <%--<button id="btnReferenceContinue" class="btn btn-primary" data-loading-text="<i class='fa fa-refresh fa-spin'></i> Please Wait">Continue</button>--%>
            <a type="button" id="btnReferenceContinue" class="btn btn-primary" data-loading-text="<i class='fa fa-refresh fa-spin'></i> Please Wait">Continue</a>
            
            <div id="reference-validation-message"></div>
        </div>
    </div>
</asp:Panel>

<script>
    $(document).ready(function () {
        var refList = [
            <% if (HasReference1)
               {%>
            { Name: '<%=Reference1Name%>', Association: '<%=Reference1Association%>', Contact: '<%=Reference1Email%>', Complete: '<%=Reference1Complete%>', Status: '<%=Reference1Status%>', ReferencePersonAliasGuid: '<%=Reference1PersonAliasGuid%>' },
            <%}%>
            <% if (HasReference2)
               {%>
            { Name: '<%=Reference2Name%>', Association: '<%=Reference2Association%>', Contact: '<%=Reference2Email%>', Complete: '<%=Reference2Complete%>', Status: '<%=Reference2Status%>', ReferencePersonAliasGuid: '<%=Reference2PersonAliasGuid%>' },
            <%}%>
            <% if (HasReference3)
               {%>
            { Name: '<%=Reference3Name%>', Association: '<%=Reference3Association%>', Contact: '<%=Reference3Email%>', Complete: '<%=Reference3Complete%>', Status: '<%=Reference3Status%>', ReferencePersonAliasGuid: '<%=Reference3PersonAliasGuid%>' }
            <%}%>
        ];
        WillowRock.init({
            ApplicantPersonAliasGuid: '<%= CurrentPersonAlias.Guid %>',
            workflowId: '<%=PageParameter("WorkflowId")%>',
            referenceList: refList
        });
    });
</script>