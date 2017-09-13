<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuestionnaireForm.ascx.cs" Inherits="Plugins_org_willowcreek_ProtectionApp_QuestionnaireForm" %>
<link href="/Styles/WillowCreek/ProtectionApp.css" type="text/css" rel="stylesheet" />
<script src="/scripts/WillowRockProtectionApp.js" type="text/javascript"></script>
<script src="/scripts/jquery.maskedinput.min.js" type="text/javascript"></script>
<Rock:HelpBlock runat="server"  /> <!-- This makes the little help links work -->
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
    <div id="willow_pa_applicant_form" class="frm-pane-wrapper" style="max-width: 600px;">
        <fieldset class="frm-pane" runat="server" visible="true">
            <p>
                Before beginning the protection application, please verify your identity.
            </p>
        </fieldset>

        <fieldset class="frm-pane" runat="server" visible="true">
            <div class="row form-group">
                <div class="col-md-12">
                    Please do not exit the application process until you've completed the following components, which should take 5-10 minutes:
                    <ul>
                        <li>Application</li>
                        <li>Personal References</li>
                        <li>Policy Acknowledgement</li>
                    </ul>
                </div>
            </div>

            <legend>General Information</legend>

            <div style="display: none">
                <input type="text" id="ApplicantPersonAliasGuid" name="ApplicantPersonAliasGuid" />
            </div>


            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="Today">Todays Date</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="Today" name="Today" class="form-control" value="<%=DateTime.Now %>" disabled />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="LegalFirstName">Legal First Name</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="LegalFirstName" name="LegalFirstName" class="form-control" placeholder="Legal First Name" required data-validation-message="Legal First Name is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="MiddleName">Middle Name</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="MiddleName" name="LegalName" class="form-control" placeholder="Middle Name" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="LastName">Last Name</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="LastName" name="LastName" class="form-control" placeholder="Last Name" required data-validation-message="Last Name is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Suffix">Suffix</label>
                </div>
                <div class="col-md-6">
                    <select id="Suffix" name="Suffix" class="form-control">
                        <option value=""></option>
                        <option value="188">Jr.</option>
                        <option value="189">Sr.</option>
                        <option value="195">Ph.D.</option>
                        <option value="190">II</option>
                        <option value="191">III</option>
                        <option value="192">IV</option>
                        <option value="193">V</option>
                        <option value="194">VI</option>
                    </select>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12 notation-simple">
                    Please verify your full legal name below, this will be required for your signature at the end of the application.
                </div>
            </div>
            <div class="row form-group notation-simple">
                <div class="col-md-6">
                    <label for="FullLegalName">Full Legal Name</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="FullLegalName" name="FullLegalName" class="form-control" placeholder="Full Legal Name" disabled />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Nickname">Nickname</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="Nickname" name="Nickname" class="form-control" placeholder="Nickname" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="PreviousName">If applicable: Previous names (Including Maiden)</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousName" name="PreviousName" class="form-control" placeholder="PreviousName" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="CurrentAddressStreet">Street Address</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="CurrentAddressStreet" name="CurrentAddressStreet" class="form-control" placeholder="Street Address" required data-validation-message="Street Address is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="CurrentAddressCity">City</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="CurrentAddressCity" name="CurrentAddressCity" class="form-control" placeholder="City" required data-validation-message="City is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="CurrentAddressState">State</label>
                    <span class="hint">Two letter abbreviation (e.g. IL)</span>
                </div>
                <div class="col-md-6">
                    <input type="text" id="CurrentAddressState" name="CurrentAddressState" class="form-control" placeholder="State" required data-validation-message="State is required" maxlength="2" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="CurrentAddressZip">Zip</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="CurrentAddressZip" name="CurrentAddressZip" class="form-control" placeholder="Zip" required data-validation-message="Zip is required" maxlength="5" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="">Have you lived at your current address for at least 12 months?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="livedCurrentMoreThan12_Yes" name="livedCurrentMoreThan12" value="1" required data-validation-message="Have you lived at your current address for at least 12 months is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="livedCurrentMoreThan12_No" name="livedCurrentMoreThan12" value="0" required data-validation-message="Have you lived at your current address for at least 12 months is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="livedCurrentMoreThan12" data-dependent-value="0">
                <div class="col-md-6">
                    <label for="PreviousStreetAddress">Previous Address</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousStreetAddress" class="form-control" name="PreviousStreetAddress" placeholder="Street Address" required data-validation-message="Previous Street Address is required" />
                </div>
            </div>
            <div class="row form-group" data-dependent-on="livedCurrentMoreThan12" data-dependent-value="0">
                <div class="col-md-6">
                    <label for="PreviousAddressCity">City</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousAddressCity" name="PreviousAddressCity" class="form-control" placeholder="City" required data-validation-message="Previous City is required" />
                </div>
            </div>
            <div class="row form-group" data-dependent-on="livedCurrentMoreThan12" data-dependent-value="0">
                <div class="col-md-6">
                    <label for="PreviousAddressState">State</label>
                    <span class="hint">Two letter abbreviation (e.g. IL)</span>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousAddressState" name="PreviousAddressState" class="form-control" placeholder="State" required data-validation-message="Previous State is required" maxlength="2" />
                </div>
            </div>
            <div class="row form-group" data-dependent-on="livedCurrentMoreThan12" data-dependent-value="0">
                <div class="col-md-6">
                    <label for="PreviousAddressZip">Zip</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousAddressZip" name="PreviousAddressZip" class="form-control" placeholder="Zip" required data-validation-message="Previous Zip is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="MobilePhone">Mobile Phone</label>
                </div>
                <div class="col-md-6">
                    <input type="tel" id="MobilePhone" name="MobilePhone" class="form-control" data-role="phone" placeholder="(123) 456-7890" data-required-if="!#HomePhone" data-validation-message="Either a Home or Mobile Phone is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="HomePhone">Home Phone</label>
                </div>
                <div class="col-md-6">
                    <input type="tel" id="HomePhone" name="HomePhone" class="form-control" data-role="phone" placeholder="(123) 456-7890" required data-validation-message="Home Phone is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="EmailAddress">Email</label>
                </div>
                <div class="col-md-6">
                    <input type="email" id="EmailAddress" name="EmailAddress" class="form-control" placeholder="email@domain.com" required data-validation-message="Email is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Gender_Male">Gender</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="Gender_Male" name="Gender" value="1" required data-validation-message="Gender is required" />
                        Male</label>
                    <label class="radiolabel">
                        <input type="radio" id="Gender_Female" name="Gender" value="2" required data-validation-message="Gender is required" />
                        Female</label>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="DateOfBirth">Date of Birth</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="DateOfBirth" name="DateOfBirth" class="form-control" placeholder="mm/dd/yyyy" data-role="date" required data-validation-message="Date of Birth is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Marital_Status">Marital Status</label>
                </div>
                <div class="col-md-6">
                    <select id="Marital_Status" name="Marital" class="form-control" required data-validation-message="Marital Status is required">
                        <option value="144">Single</option>
                        <option value="143">Married</option>
                        <option value="895">Separated</option>
                        <option value="896">Divorced</option>
                        <option value="897">Widowed</option>
                    </select>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Children_Yes">Do you have children?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="Children_Yes" name="Children" value="true" required data-validation-message="Do you have children is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="Children_No" name="Children" value="false" required data-validation-message="Do you have children is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="Children" data-dependent-value="true">
                <div class="col-md-6"></div>
                <div class="col-md-3">
                    <label for="ChildrenCount">How many?</label>
                </div>
                <div class="col-md-3">
                    <input type="text" id="ChildrenCount" name="ChildrenCount" class="form-control" placeholder="0" required data-validation-message="How many children is required" />
                </div>
            </div>
            <div class="row form-group" data-dependent-on="Children" data-dependent-value="true">
                <div class="col-md-6"></div>
                <div class="col-md-3">
                    <label for="Ages">Age(s)</label>
                </div>
                <div class="col-md-3">
                    <input type="text" id="Ages" name="Ages" class="form-control" placeholder="3,5,9" required data-validation-message="Your childrens ages are required" />
                </div>
            </div>
            <div id="guardianDiv" class="row form-group">
                <div class="col-md-6">
                    <label for="Guardian_No">Do you have a legal guardian?</label>
                    <a class="help" href="#" tabindex="-1"><i class="fa fa-question-circle"></i></a>
                    <div class="alert alert-info help-message" style="display: none;">
                        <small>A person who has the legal authority to care for the personal and property interests of another person, called a ward.
                        </small>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="Guardian_Yes" name="Guardian" value="true" required data-validation-message="Do you have a legal guardian, other than yourself is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="Guardian_No" name="Guardian" value="false" required data-validation-message="Do you have a legal guardian, other than yourself is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="WCCC_Date">Do you attend Willow Creek Community Church?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="WCCC_Yes" name="WCCC" value="true" required data-validation-message="Do you attend WCCC is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="WCCC_No" name="WCCC" value="false" required data-validation-message="Do you attend WCCC is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="WCCC" data-dependent-value="true">
                <div class="col-md-6">
                    <label for="WCCC_Date">When did you begin attending Willow Creek Community Church?</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="WCCC_Date" name="WCCC_Date" class="form-control" placeholder="mm/dd/yyyy" data-role="date" required data-validation-message="When did you begin attending WCCC is required" />
                </div>
            </div>

        </fieldset>

        <fieldset class="frm-pane" runat="server" visible="true">
            <legend>Personal Questions</legend>

            <div class="row form-group">
                <div class="col-md-12">
                    <label>
                        Please indicate if you have any history of addiction to the following:<br />
                        (Select all that apply)</label><br />
                    <span class="note">("Recent" = within the past 12 months)</span>
                </div>
            </div>
            <div class="row form-group indented-block">
                <div class="col-md-12">
                    <div class="row form-group">
                        <div class="col-md-6">
                            <label>Pornography:</label>
                        </div>
                        <div class="col-md-6">
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Porno" id="Porno_Past" value="1" required data-validation-message="Pornography is required" />
                                Past</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Porno" id="Porno_Recent" value="2" required data-validation-message="Pornography is required" />
                                Recent</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Porno" id="Porno_None" value="0" required data-validation-message="Pornography is required" />
                                None</label>
                        </div>
                    </div>
                    <div class="row form-group">
                        <div class="col-md-6">
                            <label>Alcohol:</label>
                        </div>
                        <div class="col-md-6">
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Alcohol" id="Alcohol_Past" value="1" required data-validation-message="Alcohol is required" />
                                Past</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Alcohol" id="Alcohol_Recent" value="2" required data-validation-message="Alcohol is required" />
                                Recent</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Alcohol" id="Alcohol_None" value="0" required data-validation-message="Alcohol is required" />
                                None</label>
                        </div>
                    </div>
                    <div class="row form-group">
                        <div class="col-md-6">
                            <label>Drugs (illegal or prescription):</label>
                        </div>
                        <div class="col-md-6">
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Drugs" id="Drugs_Past" value="1" required data-validation-message="Drugs (illegal or prescription) is required" />
                                Past</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Drugs" id="Drugs_Recent" value="2" required data-validation-message="Drugs (illegal or prescription) is required" />
                                Recent</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Drugs" id="Drugs_None" value="0" required data-validation-message="Drugs (illegal or prescription) is required" />
                                None</label>
                        </div>
                    </div>
                    <div class="row form-group" data-dependent-on=".recent" data-dependent-value="2">
                        <div class="col-md-6">
                            <label>Please Explain</label>
                        </div>
                        <div class="col-md-6">
                            <textarea rows="4" id="Addiction_Explain" class="form-control" required data-validation-message="Explanation of your recent history with one of the above addictions is required" data-minlength="75" maxlength="2000"></textarea>
                            <span class="counter">75 of 2000 chars allowed</span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <div style="font-weight: bold">
                        Have you ever looked at pornography featuring minors?
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="lookedAtPorn_Yes" name="lookedAtPorn" value="true" required data-validation-message="Have you ever looked at pornography featuring minors is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="lookedAtPorn_No" name="lookedAtPorn" value="false" required data-validation-message="Have you ever looked at pornography featuring minors is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="lookedAtPorn" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="lookedAtPorn_Explain" class="form-control" required data-validation-message="Explanation of why you looked at pornography featuring minors is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <div style="font-weight: bold">
                        Have you ever been the subject of a <a class="help" href="#" style="color: inherit; border-bottom: 1px dotted #86b8cc" tabindex="-1">DCFS</a> investigation that was indicated (evidence was found to indicate abuse/neglect)?                
                <div class="alert alert-info help-message" style="display: none; font-weight: normal;">
                    <small>Department of Child and Family Services 
                    </small>
                </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="DCFS_Yes" name="DCFS" value="true" required data-validation-message="Have you ever been the subject of a DCFS investigation is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="DCFS_No" name="DCFS" value="false" required data-validation-message="Have you ever been the subject of a DCFS investigation is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="DCFS" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="DCFS_Explain" class="form-control" required data-validation-message="Explanation of why you have been the subject of a DCFS investigation is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Have you ever had an order of protection filed against you?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="OOP_Yes" name="OOP" value="true" required data-validation-message="Have you ever had an order of protection filed against you is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="OOP_No" name="OOP" value="false" required data-validation-message="Have you ever had an order of protection filed against you is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="OOP" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="OOP_Explain" class="form-control" required data-validation-message="Explanation of why you had an order of protection filed against you is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Have you ever committed or been accused of any act involving the physical, sexual, OR emotional harm of another person (Examples include battery, rape, neglect, etc.)?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="ComittedOrAccused_Yes" name="ComittedOrAccused" value="true" required data-validation-message="Have you ever committed or been accused of any act involving the physical, sexual, OR emotional harm of another person is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="ComittedOrAccused_No" name="ComittedOrAccused" value="false" required data-validation-message="Have you ever committed or been accused of any act involving the physical, sexual, OR emotional harm of another person is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="ComittedOrAccused" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="ComittedOrAccused_Explain" class="form-control" required data-validation-message="Explanation of why you were committed or accused of any act involving the physical, sexual, OR emotional harm of another person is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <div style="font-weight: bold">
                        Have you ever had any kind of a relationship with a minor or<a class="help" href="#" style="color: inherit; border-bottom: 1px dotted #86b8cc" tabindex="-1">vulnerable adult</a> that has brought sexual gratification to yourself?                    
                <div class="alert alert-info help-message" style="display: none; font-weight: normal">
                    <small>A person age 18 or older who lacks the physical and/or mental capacity to provide for his or her daily needs, is unable to protect him or herself against significant harm or exploitation, and is at risk in the community. This includes, but is not limited to, anyone who has been assigned a legal guardian.
                    </small>
                </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="RelWithVuln_Yes" name="RelWithVuln" value="true" required data-validation-message="Have you ever had any kind of a relationship with a minor or vulnerable adult that has brought sexual gratification to yourself is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="RelWithVuln_No" name="RelWithVuln" value="false" required data-validation-message="Have you ever had any kind of a relationship with a minor or vulnerable adult that has brought sexual gratification to yourself is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="RelWithVuln" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="RelWithVuln_Explain" class="form-control" required data-validation-message="Expanation of why you had any kind of a relationship with a minor or vulnerable adult that has brought sexual gratification to yourself is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Have you ever left or been asked to leave a role within an organization due to a concern regarding inappropriate conduct?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="Misconduct_Yes" name="Misconduct" value="true" required data-validation-message="Have you ever left or been asked to leave a role within an organization due to a concern regarding inappropriate conduct is required" />
                        Yes</label>
                    <label class="radiolabel">
                        <input type="radio" id="Misconduct_No" name="Misconduct" value="false" required data-validation-message="Have you ever left or been asked to leave a role within an organization due to a concern regarding inappropriate conduct is required" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="Misconduct" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="Misconduct_Explain" class="form-control" required data-validation-message="Explanation of why you left or was asked to leave a role within an organization due to a concern regarding inappropriate conduct is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>

        </fieldset>

        <fieldset class="frm-pane" runat="server" visible="true">
            <legend>Release of Information</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    Thank you for taking the time to fill out this application and for helping us to create a safe atmosphere at Willow Creek. Please read and affirm the following statements:
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    <input id="chkCorrectInfo" name="chkCorrectInfo" type="checkbox" />
                    The information contained in this application is correct to the best of my knowledge.
                </div>
                <div class="col-md-12">
                    <input id="chkAuthReleaseInfo" name="chkAuthReleaseInfo" type="checkbox" />
                    I authorize the release of the information contained in this application, on a confidential, need-to-know basis, to any ministry at Willow Creek Community Church in which I seek a position (volunteer or for compensation).
                </div>
            </div>

            <h4>Background Check Authorization</h4>
            <div class="row form-group note">
                <div class="col-md-12" style="border: solid 1px #666; margin: 0 0 0 15px; padding: 5px; font: normal 9pt verdana; font-style: italic; overflow: auto; height: 85px;">
                    <p>
                        By typing my name below, I authorize Willow Creek Community Church and its authorized agents to obtain/ prepare consumer reports or investigative consumer reports about me.  I acknowledge receipt of a copy of A Summary of Your Rights under the Fair Credit Reporting Act (below) and verify that I have read it.
                    </p>
                    <p style="text-align: center;">
                        Summary of Your Rights under the Fair Credit Reporting Act                    
                    </p>
                    <p>
                        In connection with my Protection application for Willow Creek Community Church, I understand that a "consumer report" and/or 
                "investigative consumer report", as defined by the Fair Credit Reporting Act, will be requested by Willow Creek for employment 
                or volunteer purposes, whichever is applicable, from Protect My Ministry, Inc., ("Protect My Ministry"), a consumer reporting 
                agency as defined by the Fair Credit Reporting Act.  These reports may include information as to my character, general reputation, 
                personal characteristics or mode of living, whichever are applicable. They may involve interviews with sources such as my neighbors, 
                friends or associates. The report may also contain information about me relating to my criminal history, credit history, driving 
                and/or motor vehicle records, social security number verification, verification of education or employment history, worker's 
                compensation (only after a conditional job offer) or other background checks. Such reports may be obtained at any time after 
                receipt of this Disclosure and Authorization and if I am hired or serve as a volunteer, whichever is applicable, throughout the 
                course of my employment or volunteer service, as permitted by law and unless revoked by me in writing.  I understand that I have 
                the right, upon written request made within a reasonable amount time after the receipt of this notice, to request disclosure of the 
                nature and scope of any investigative consumer report to Protect My Ministry, Inc., 14499 N. Dale Mabry Hwy., Suite 201 South, 
                Tampa, FL 33618 or 1-800-319-5581. 
                    </p>
                </div>
                <div class="col-md-12" style="font: italic 9pt verdana; padding: 15px;">
                    For information about Protect My Ministry's privacy practices, 
                see <a href="http://www.protectmyministry.com" target="_blank">www.protectmyministry.com</a>.
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Applicant's Social Security Number:</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="ApplicantSsn" class="form-control" placeholder="999-99-9999" required data-validation-message="Applicant's social security number is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Verify Applicant's Social Security Number:</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="ApplicantSsnVerify" class="form-control" placeholder="999-99-9999" required data-validation-message="Applicant's social security number is required" data-validation-match="ApplicantSsn" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-4">
                    <label>Applicant's Signature</label>
                </div>
                <div class="col-md-8">
                    <input type="text" id="ApplicantSignature" class="form-control" placeholder="Please type full legal name" required data-validation-message="Applicant's signature is required" />

                    <input type="text" id="ApplicantSignatureDate" class="form-control" value="<%=DateTime.Now.ToString("M/d/yyyy") %>" disabled style="width: 100px" />
                </div>
            </div>
            <div class="row form-group" Id ="guardianSignatureDiv">
                <div class="col-md-4">
                    <label>Parent/Guardian Signature*</label>
                </div>
                <div class="col-md-8">
                    <input type="text" id="GuardianSignature" class="form-control" required data-validation-message="Guardian's signature is required" />

                    <input type="text" id="GuardianSignatureDate" class="form-control" value="<%=DateTime.Now.ToString("M/d/yyyy") %>" disabled style="width: 100px" />
                </div>
            </div>
            <div class="row form-group note" data-dependent-on="Guardian" data-dependent-value="true">
                <div class="col-md-12">*If applicant is a minor</div>
            </div>
        </fieldset>

        <%--        <app:References runat="server" />--%>

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
        <div id="buttonContainer">
            <div id="willow_pa_applicant_buttons">
                <button id="btnYesIm" class="btn btn-primary">
                    I am
                    <asp:Literal ID="ltlApplicantName" runat="server"></asp:Literal>
                </button>
                <button id="btnNoIm" class="btn btn-link">
                    I'm not
                <asp:Literal ID="ltlApplicantName2" runat="server"></asp:Literal></button>
                <button id="btnApplicantBack" class="btn" style="display: none;">Back</button>
                <a type="button" id="btnApplicantContinue" style="display: none;" class="btn btn-primary" data-loading-text="<i class='fa fa-refresh fa-spin'></i> Please Wait">Continue</a>
                <div id="applicant-validation-message"></div>
            </div>
        </div>
        <div id="Prepopulate" class="hidden">
            <input type="text" runat="server" id="GenderType" />
            <input type="text" runat="server" id="Nickname_server" />
            <input type="text" runat="server" id="Street_server" />
            <input type="text" runat="server" id="City_server" />
            <input type="text" runat="server" id="State_server" />
            <input type="text" runat="server" id="Zip_server" />
            <input type="text" runat="server" id="MobilePhone_server" />
            <input type="text" runat="server" id="HomePhone_server" />
            <input type="text" runat="server" id="Email_server" />
            <input type="text" runat="server" id="DateOfBirth_server" />
            <input type="text" runat="server" id="MaritalStatus_server" />
            <input type="text" runat="server" id="ApplicantPersonAliasGuid_server" />
        </div>
    </div>
</asp:Panel>

<script>
    $(document).ready(function () {
        WillowRock.init({
            personId: '<%= CurrentPersonId %>',
            workflowId: '<%=PageParameter("WorkflowId")%>'
        });

        //setup the ui
        switch ($('#<%=GenderType.ClientID %>').val()) {
            case "1":
                $("#Gender_Male").prop('checked', true);
                break;
            case "2":
                $("#Gender_Female").prop('checked', true);
                break;
            default:
                $("#Gender_Male").prop('checked', false);
                $("#Gender_Female").prop('checked', false);
                break;
        }
        $("#ApplicantPersonAliasGuid").val($("#<%=ApplicantPersonAliasGuid_server.ClientID%>").val());
        $("#Nickname").val($("#<%=Nickname_server.ClientID%>").val());
        $("#CurrentAddressStreet").val($("#<%=Street_server.ClientID%>").val());
        $("#CurrentAddressCity").val($("#<%=City_server.ClientID%>").val());
        $("#CurrentAddressState").val($("#<%=State_server.ClientID%>").val());
        $("#CurrentAddressZip").val($("#<%=Zip_server.ClientID%>").val());
        $("#MobilePhone").val($("#<%=MobilePhone_server.ClientID%>").val());
        $("#HomePhone").val($("#<%=HomePhone_server.ClientID%>").val());
        $("#EmailAddress").val($("#<%=Email_server.ClientID%>").val());
        $("#DateOfBirth").val($("#<%=DateOfBirth_server.ClientID%>").val());
        $("#Marital_Status").val($("#<%=MaritalStatus_server.ClientID%>").val());
        //-->enable masked inputs
        $('#State').mask('aa');
        $('#Zip').mask('99999');
        $('#PreviousState').mask('aa');
        $('#PreviousZip').mask('99999');
        $('#HomePhone').mask('(999) 999-9999');
        $('#MobilePhone').mask('(999) 999-9999');
        $('#DateOfBirth').mask('99/99/9999', { placeholder: "mm/dd/yyyy" });
        $('#NumChildren').mask('99');
        $('#WCCC_Date').mask('99/99/9999', { placeholder: "mm/dd/yyyy" });
        $('#ApplicantSsn').mask('999-99-9999');
        $('#ApplicantSsnVerify').mask('999-99-9999');

        var $firstName = $('#LegalFirstName');
        var $middleName = $('#MiddleName');
        var $lastName = $('#LastName');
        var $suffix = $('#Suffix');
        $firstName.keyup(WillowRock.protectionApp.Applicant.setLegalName);
        $middleName.keyup(WillowRock.protectionApp.Applicant.setLegalName);
        $lastName.keyup(WillowRock.protectionApp.Applicant.setLegalName);
        $suffix.change(WillowRock.protectionApp.Applicant.setLegalName);
    });
</script>
