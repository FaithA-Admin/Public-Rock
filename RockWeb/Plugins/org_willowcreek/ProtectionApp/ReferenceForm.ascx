<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReferenceForm.ascx.cs" Inherits="Plugins_org_willowcreek_ProtectionApp_ReferenceForm" %>
<link href="/Styles/WillowCreek/ProtectionApp.css" type="text/css" rel="stylesheet" />

<asp:Panel ID="pnlAppDone" runat="server" ClientIDMode="Static">
    <p>Thanks so much for your quick response and willingness to fill out a reference! We send out three references and only need two responses. Once we receive two references then the third reference link expires, as is the case here. Thanks so much for being willing to fill out the reference, but unless you have any concerns then he/she is all set.</p>
    <p>If you do have any concerns, please contact the Protection Department by phone, 224-512-1920, or by email, <a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a>.</p>
</asp:Panel>

<asp:Panel ID="pnlReference" runat="server">
    <div class="frm-pane-wrapper" <%--style="max-width: 570px;"--%>>
        <fieldset class="frm-pane" runat="server" visible="true">
             <div class="row form-group">
                  <div class="col-md-6">
                       <h3>Volunteer Reference</h3>
                  </div>
                <div class="col-md-6">
                    <b><i><a id="translateToSpanish" runat="server">Haga click aquí para ver esta página en Español.</a></i></b>
                </div>
            </div>
            
            <hr />
            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="Today">Todays Date</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="Today" name="Today" class="form-control" value="<%=DateTime.Now %>" disabled />
                </div>
            </div>
            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="FirstName">First Name</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="FirstName" name="FirstName" value="<%=FirstName %>" class="form-control" placeholder="First Name" required data-validation-message="Legal First Name is required" />
                </div>
            </div>
            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="MiddleName">Middle Name</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="MiddleName" name="MiddleName" value="<%=MiddleName %>" class="form-control" placeholder="Middle Name" />
                </div>
            </div>
            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="LastName">Last Name</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="LastName" name="LastName" value="<%=LastName %>" class="form-control" placeholder="Last Name" required data-validation-message="Last Name is required" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="">Have you known the applicant more than one year?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="KnownMoreThanOneYear_Yes" name="KnownMoreThanOneYear" value="true" required data-validation-message="Have you known the applicant more than one year is required" />
                        Yes
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="KnownMoreThanOneYear_No" name="KnownMoreThanOneYear"  value="false" required data-validation-message="Have you known the applicant more than one year is required" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="">Are you 18 years or older?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="IsReference18_Yes" name="IsReference18" value="true" required data-validation-message="Are you 18 years or older is required" />
                        Yes
                    </label>               
                    <label class="radiolabel">
                        <input type="radio" id="IsReference18_No" name="IsReference18" value="false" required data-validation-message="Are you 18 years or older is required" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="NatureOfRelationship">What is the nature of your relationship with the applicant?</label>
                </div>
                <div class="col-md-6">
                    <select id="NatureOfRelationship" name="NatureOfRelationship" class="form-control">
                        <option value="Friend">Friend</option>
                        <option value="Colleague">Colleague</option>
                        <option value="Supervisor">Past or Current Supervisor</option>
                        <option value="mentor">Past or present mentor</option>
                        <option value="Family">Family</option>
                    </select>
                </div>
            </div>

        </fieldset>

        <fieldset class="frm-pane">
            <legend>Questions</legend>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Is the applicant able to maintain meaningful peer relationships (not including family)?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="MaintainRelationships_Yes" name="MaintainRelationships" value="true" required data-validation-message="Is the applicant able to maintain meaningful peer relationships is required" />
                        Yes
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="MaintainRelationships_No" name="MaintainRelationships" value="false" required data-validation-message="Is the applicant able to maintain meaningful peer relationships is required" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="MaintainRelationships" data-dependent-value="false">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="MaintainRelationships_Explain" class="form-control" required data-validation-message="Explanation of Is the applicant able to maintain meaningful peer relationships is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Have you ever been concerned with the applicant's ability to respect healthy relational boundaries?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="RespectHealthyRelationalBoundaries_Yes" name="RespectHealthyRelationalBoundaries" value="true" required data-validation-message="Have you ever been concerned with the applicant's ability to respect healthy relational boundaries is required" />
                        Yes
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="RespectHealthyRelationalBoundaries_No" name="RespectHealthyRelationalBoundaries" value="false" required data-validation-message="Have you ever been concerned with the applicant's ability to respect healthy relational boundaries is required" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="RespectHealthyRelationalBoundaries" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="RespectHealthyRelationalBoundaries_Explain" class="form-control" required data-validation-message="Explanation of Have you ever been concerned with the applicant's ability to respect healthy relational boundaries is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Does the applicant have any criminal offenses that you are aware of?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="CriminalOffenses_Yes" name="CriminalOffenses" value="true" required data-validation-message="Does the applicant have any criminal offenses that you are aware of is required" />
                        Yes
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="CriminalOffenses_No" name="CriminalOffenses" value="false" required data-validation-message="Does the applicant have any criminal offenses that you are aware of is required" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="CriminalOffenses" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="CriminalOffenses_Explain" class="form-control" required data-validation-message="Explanation of Does the applicant have any criminal offenses that you are aware of is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Does the applicant have any patterns of behavior that you perceive as deceitful or manipulative?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="ManipulativeBehavior_Yes" name="ManipulativeBehavior" value="true" required data-validation-message="Does the applicant have any patterns of behavior that you perceive as deceitful or manipulative is required" />
                        Yes
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="ManipulativeBehavior_No" name="ManipulativeBehavior" value="false" required data-validation-message="Does the applicant have any patterns of behavior that you perceive as deceitful or manipulative is required" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="ManipulativeBehavior" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="ManipulativeBehavior_Explain" class="form-control" required data-validation-message="Does the applicant have any patterns of behavior that you perceive as deceitful or manipulative is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Are you aware of any act by the applicant involving the physical, sexual, or emotional harm of another person (including, but not limited to, sexual abuse, Order of Protection, neglect, exploitation, battery, DCFS investigation etc.)?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="InflictedEmotionalHarm_Yes" name="InflictedEmotionalHarm" value="true" required data-validation-message="Are you aware of any act by the applicant involving the physical, sexual, or emotional harm of another person (including, but not limited to, sexual abuse, Order of Protection, neglect, exploitation, battery, DCFS investigation etc.) is required" />
                        Yes
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="InflictedEmotionalHarm_No" name="InflictedEmotionalHarm" value="false" required data-validation-message="Are you aware of any act by the applicant involving the physical, sexual, or emotional harm of another person (including, but not limited to, sexual abuse, Order of Protection, neglect, exploitation, battery, DCFS investigation etc.) is required" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="InflictedEmotionalHarm" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="InflictedEmotionalHarm_Explain" class="form-control" required data-validation-message="Expanation of Are you aware of any act by the applicant involving the physical, sexual, or emotional harm of another person (including, but not limited to, sexual abuse, Order of Protection, neglect, exploitation, battery, DCFS investigation etc.) is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>If you have or were to have children, would  you trust this person to care for them?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="TrustInChildCare_Yes" name="TrustInChildCare" value="true" required data-validation-message="If you have or were to have children, would  you trust this person to care for them is required" />
                        Yes
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="TrustInChildCare_No" name="TrustInChildCare" value="false" required data-validation-message="If you have or were to have children, would  you trust this person to care for them is required" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="TrustInChildCare" data-dependent-value="false">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="TrustInChildCare_Explain" class="form-control" required data-validation-message="Explanation of If you have or were to have children, would  you trust this person to care for them is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Would you recommend the applicant as a volunteer?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="WouldRecommend_Yes" name="WouldRecommend" value="1" required data-validation-message="Would you recommend the applicant as a volunteer is required" />
                        Yes
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="WouldRecommend_No" name="WouldRecommend" value="0" required data-validation-message="Would you recommend the applicant as a volunteer is required" />
                        No
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="WouldRecommend_Depends" name="WouldRecommend" value="2" required data-validation-message="Would you recommend the applicant as a volunteer is required" />
                        Depends
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="WouldRecommend" data-dependent-value="0,2">
                <div class="col-md-6">
                    <label>Please Explain</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="WouldRecommend_Explain" class="form-control" required data-validation-message="Explanation of Would you recommend the applicant as a volunteer is required" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 of 2000 chars allowed</span>
                </div>
            </div>

        </fieldset>

        <fieldset class="frm-pane">
            <legend>Signature</legend>
            <div class="row form-group reference-invalid">
                <div class="col-md-12">
                    Thank you for your willingness to provide a reference for this applicant. Unfortunately, at least one response on the previous page makes us unable to use you as a reference. We apologize for any inconvenience. If you have concerns about the applicant serving at Willow Creek Community Church, please email us directly at <a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a>.
               
                </div>
            </div>
            <div class="row form-group reference-valid">
                <div class="col-md-12">
                    Thank you for taking the time to complete this reference. Your input helps us create a welcoming and safe atmosphere at Willow Creek. To complete your reference, please provide your electronic signature below.
               
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    By typing my first and last name below, I affirm that the information in this reference is accurate and truthful, to the best of my knowledge.
               
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-4">
                    <label>Reference's Signature</label>
                </div>
                <div class="col-md-8">
                    <input type="text" id="ApplicantSignature" data-role="sign" class="form-control" required data-validation-message="Reference's signature is required" />

                    <input type="text" id="ApplicantSignatureDate" class="form-control" value="<%=DateTime.Now.ToString("M/d/yyyy") %>" disabled style="width: 100px" />
                </div>
            </div>
        </fieldset>

        <fieldset class="frm-pane">
            <legend>Thank you for your submission!</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    <p>Thank you for taking the time to complete this reference. Your input helps us create a welcoming and safe environment at Willow Creek!</p>
                    <p>If you would like to explain any information you provided in more depth, feel free to contact the Protection Department.</p>
                    <p><a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a></p>
                    <p>224-512-1920</p>
                </div>
            </div>
        </fieldset>

        <div>
            <button id="btnBack" class="btn" style="display: none;">Back</button>
            <a type="button" id="btnContinue" class="btn btn-primary" data-loading-text="<i class='fa fa-refresh fa-spin'></i> Please Wait">Continue</a>
            <div id="validation-message"></div>
        </div>
    </div>
    <script>
        var references = new Array();
        var loadPostData = function () {
            var recommend = $('[name=WouldRecommend]:checked').val();
            var wouldRecommend = false;
            switch (recommend) {
                case 1:
                case "1":
                    wouldRecommend = true;
                    break;
                default:
                    wouldRecommend = false;
            }
            return {
                SubmissionDate: new Date(),
                QuestionaireId: null,
                Id: '<%=ReferenceId %>',
                ReferencePersonAliasGuid: '<%=ReferencePersonAliasGuid %>',
                WorkflowId: '<%=PageParameter("WorkflowId")%>',
                FirstName: $('#FirstName').val(),
                MiddleName: $('#MiddleName').val(),
                LastName: $('#LastName').val(),
                KnownMoreThanOneYear: $('[name=KnownMoreThanOneYear]:checked').val(),
                IsReference18: $('[name=IsReference18]:checked').val(),
                NatureOfRelationship: $('#NatureOfRelationship').val(),
                MaintainRelationships: $('[name=MaintainRelationships]:checked').val(),
                MaintainRelationshipsExplain: $('#MaintainRelationships_Explain').val(),
                RespectHealthyRelationalBoundaries: $('[name=RespectHealthyRelationalBoundaries]:checked').val(),
                RespectHealthyRelationalBoundariesExplain: $('#RespectHealthyRelationalBoundaries_Explain').val(),
                CriminalOffenses: $('[name=CriminalOffenses]:checked').val(),
                CriminalOffensesExplain: $('#CriminalOffenses_Explain').val(),
                ManipulativeBehavior: $('[name=ManipulativeBehavior]:checked').val(),
                ManipulativeBehaviorExplain: $('#ManipulativeBehavior_Explain').val(),
                InflictedEmotionalHarm: $('[name=InflictedEmotionalHarm]:checked').val(),
                InflictedEmotionalHarmExplain: $('#InflictedEmotionalHarm_Explain').val(),
                TrustInChildCare: $('[name=TrustInChildCare]:checked').val(),
                TrustInChildCareExplain: $('#TrustInChildCare_Explain').val(),
                //WouldRecommend: wouldRecommend,
                WouldRecommend: $('[name=WouldRecommend]:checked').val(),
                WouldRecommendExplain: $('#WouldRecommend_Explain').val(),
                Signature: $('#ApplicantSignature').val(),
                SignatureDate: $('#ApplicantSignatureDate').val()
            };
        }
        $(document).ready(function () {
            var $validInput = $('<span class="validated"><i class="fa fa-check-circle valid"></i></span>'),
                $invalidInput = $('<span class="validated"><i class="fa fa-times-circle in-valid"></i></span>');
            var $panes = $('.frm-pane'),
                $currPane = $($panes.get(0)),
                $dependents = $('[data-dependent-on]'),
                $dependentDict = new Array(),
                $maxLengthFields = $('[maxlength]'),
                $requiredFields = $('[required]'),
                $btnBack = $('#btnBack'),
                $btnContinue = $('#btnContinue'),
                $validationMsg = $('#validation-message'),
                    validReference = true;

            //setup the ui
            //-->enable validation // should we use jquery.validate() instead?
            $validationMsg.hide();
            var validateDate = function (val) {
                var valid = val.match(/^\d{1,2}\/\d{1,2}\/\d{2,4}$/i) != null;
                if (!valid)//date doesn't match 01/01/2015 lets check 2015-01-01
                    valid = val.match(/^\d{2,4}(\/)|(-)\d{1,2}(\/)|(-)\d{1,2}$/i) != null;
                if (valid)//check to make sure that it is an actual valid date
                    valid = isValidDate(val);
                if (valid) {
                    //make sure the date is not in the future
                    var date = new Date(val);
                    var now = new Date();
                    now.setHours(0, 0, 0, 0);

                    if (date > now)
                        return ' Date cannot be in the future';

                    //var yearDiff = (now.getYear() - date.getYear());
                    //var monthDiff = (date.getMonth() - now.getMonth());
                    //var dayDiff = (date.getDate() - now.getDate());
                    //var minAge = 13;
                    //if (yearDiff < minAge ||
                    //    (yearDiff == minAge && monthDiff > 0) ||
                    //    (yearDiff == minAge && monthDiff == 0 && dayDiff > 0)) {
                    //    return ' You must be older than ' + minAge + ' to continue';
                    //}
                }
                if (!valid) return ' Invalid Date';
                else return '';
            }
            var validateEmail = function (val) {
                var valid = val.match(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][\d]\.|1[\d]{2}\.|[\d]{1,2}\.))((25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\.){2}(25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\]?$)/i) != null;
                if (!valid) return ' Invalid Email';
                else return '';
            }
            var validatePhone = function (val) {
                var valid = val.match(/^[\\(]{0,1}([0-9]){3}[\\)]{0,1}[ ]?([^0-1]){1}([0-9]){2}[ ]?[-]?[ ]?([0-9]){4}[ ]*((x){0,1}([0-9]){1,5}){0,1}$/i) != null;
                if (!valid) return ' Invalid Phone';
                else return '';
            }
            var specialFields = [];
            var validateInputField = function ($ctrl, val) {
                var valid = val && val.length > 0;
                var crole = $ctrl.attr('data-role');
                var ctype = $ctrl.attr('type');
                var addMsg = '';
                if (ctype == 'radio') {
                    var $rdo = $('[name=' + $ctrl.attr('name') + ']');
                    valid = $rdo.is(':checked');
                    $parent = $rdo.last().parent();
                } else if (ctype == 'date') {
                    addMsg = validateDate(val);
                    valid = addMsg.length == 0;
                    //alert('validating ' + val + ' as date is ' + valid);
                } else if (ctype == 'email') {
                    addMsg = validateEmail(val);
                    valid = addMsg.length == 0;
                    //alert('validating ' + val + ' as email is ' + valid);
                } else if (ctype == 'tel') {
                    addMsg = validatePhone(val);
                    valid = addMsg.length == 0;
                    //alert('validating ' + val + ' as phone is ' + valid);
                } else if (crole != undefined) {
                    switch (crole) {
                        case 'date':
                            addMsg = validateDate(val);
                            valid = addMsg.length == 0;
                            //alert('validating ' + val + ' as date is ' + valid);
                            break;
                        case 'email':
                            addMsg = validateEmail(val);
                            valid = addMsg.length == 0;
                            //alert('validating ' + val + ' as email is ' + valid);
                            break;
                        case 'phone':
                            addMsg = validatePhone(val);
                            valid = addMsg.length == 0;
                            //alert('validating ' + val + ' as phone is ' + valid);
                            break;
                        case 'sign':
                            var longEnough = val.length >= 5;
                            var hasSpace = val.indexOf(" ") !== -1;
                            if (!longEnough || !hasSpace) {
                                addMsg = " Please enter first and last name.";
                                valid = false;
                            }
                            break;
                    }
                }
                return { valid: valid, msg: addMsg };
            }
            var validateInput = function () {
                var $ctrl = $(this);
                var $parent = $ctrl.parent();
                var ctrlId = this.id;
                var val = $ctrl.val();

                if ($ctrl.is(':visible') == true) {
                    var isSpecial = false;
                    var valid = true;
                    var addMsg = '';

                    for (var s in specialFields) {
                        if (specialFields[s] == ctrlId) {
                            isSpecial = true;
                            break;
                        }
                    }

                    var vobj = validateInputField($ctrl, val);
                    valid = vobj.valid;
                    addMsg = vobj.msg;
                    //                alert(isSpecial);
                    //alert(ctrlId)
                    var matchKey = $ctrl.attr('data-validation-match');
                    if (isSpecial) {
                        switch (ctrlId) {
                            default:
                                break;
                        }
                    } else if (matchKey) {
                        var match = $('#' + matchKey);
                        var isMatch = match.val() == val;
                        if (!isMatch) {
                            valid = false;
                            addMsg = " Must match";
                        }
                    }


                    $parent.children('.validated').remove();
                    var msg;
                    if (valid) {
                        msg = $validInput.clone();
                    }
                    else {
                        msg = $invalidInput.clone().append(addMsg);
                    }
                    $parent.append(msg);
                    msg.css({ right: -(msg.width() - 3) + 'px' })
                    return valid;
                } else {
                    return true;
                }
            };
            $requiredFields.blur(validateInput);
            $requiredFields.change(validateInput);
            $maxLengthFields.keydown(function (e) {
                if ((48 <= e.keyCode && e.keyCode <= 90) || (96 <= e.keyCode && e.keyCode <= 105)) {
                    var $inp = $(this);
                    var max = $inp.attr('maxlength');
                    var val = $inp.val().replace(/_/g, '');
                    if (val.length == max) {
                        e.preventDefault();
                        return false;
                    }
                }
            });
            $maxLengthFields.keyup(function (e) {
                var $inp = $(this);
                var $cntr = $inp.parent().find('.counter');
                var min = $inp.attr('data-minlength');
                var max = $inp.attr('maxlength');
                var len = $inp.val().length;
                if (len < min) {
                    $cntr.text('Please explain further, you\'ve only got ' + (min - len) + ' chars more to type');
                } else {
                    $cntr.text(len + ' of ' + max + ' chars allowed');
                }
            });

            //-->hide all dependent questions
            $dependents.hide();
            $dependents.each(function () {
                var depOn = $(this).attr('data-dependent-on');
                var depVal = $(this).attr('data-dependent-value');
                var isClassDep = depOn.indexOf('.') == 0;
                var $trigger = isClassDep ? $(depOn) : $('#' + depOn);
                if ($trigger[0] == undefined)
                    $trigger = $('[name=' + depOn + ']');

                if ($dependentDict[depOn] == undefined) {
                    $dependentDict[depOn] = new Array();
                }
                if ($dependentDict[depOn][depVal] == undefined) {
                    var toggleDependencies = function () {
                        var $trig = $(this);
                        var dep = $trig.attr('id');
                        var val = $trig.val();
                        if ($trig.attr('type') == 'checkbox')
                            val = '' + $trig.is(':checked');

                        var $deps = $dependentDict[dep];
                        if (!$deps) {
                            dep = $trig.attr('name');
                            $deps = $dependentDict[dep];
                        }
                        if (!$deps) {
                            dep = $trig.attr('data-dependent-trigger');
                            $deps = $dependentDict[dep];
                        }
                        var ctrls;
                        var show;

                        //alert(val);
                        for (var p in $deps) {
                            ctrls = $deps[p];
                            var cval = ctrls.value;
                            var deps = ctrls.dependents;

                            if (cval.indexOf(',') == -1)
                                show = val == cval;
                            else {
                                var cvals = cval.split(',');
                                for (var v in cvals) {
                                    show = val == cvals[v];
                                    if (show)
                                        break;
                                }
                            }
                            if (show) {
                                deps.show();
                            } else {
                                deps.hide();
                            }
                        }

                        ctrls = $deps[val];
                        if (ctrls != undefined) {
                            $deps = ctrls.dependents;
                            show = val == ctrls.value;
                            $deps.toggle(show);
                        }
                    }
                    $trigger.change(toggleDependencies);

                    $dependentDict[depOn][depVal] = {
                        trigger: $trigger,
                        dependents: $('[data-dependent-on="' + depOn + '"][data-dependent-value="' + depVal + '"]'),
                        value: depVal
                    };
                }
            });
            //-->hide all panes except the first
            $panes.hide().first().show();
            //ui events
            $btnBack.click(function (e) {
                e.preventDefault();

                var $cpane = $currPane,//$($panes.get(currIdx)),
                    $ppane = $currPane.prev('.frm-pane');//$($panes.get(currIdx - 1));
                if ($ppane[0]) {
                    //previous pane
                    $cpane.slideUp(400, function () {
                    });
                    $ppane.slideDown();

                    $btnContinue.show();
                    $currPane = $ppane;
                }
                if ($ppane.prev('.frm-pane')[0] == undefined) {
                    //first
                    //alert('first');
                    $btnBack.hide();
                }
                return false;
            });
            $btnContinue.click(function (e) {
                e.preventDefault();

                var $cpane = $currPane,//$($panes.get(currIdx)),
                    $npane = $currPane.next('.frm-pane');//$($panes.get(currIdx - 1));
                var $frmRequired = $cpane.find('[required]');
                $validationMsg.hide();
                $validationMsg.empty();

                //validate all required fields
                var isValid = true;
                $frmRequired.each(function () {
                    var $ctrl = $(this);
                    var valid = validateInput.call(this);
                    if (valid == false) {
                        var msg = $ctrl.attr('data-validation-message');
                        $ctrl.focus();
                        appendError(msg);
                        isValid = false;
                        return false;
                    }
                });
                if (isValid) {
                    if ($npane[0]) {

                        validReference = (validReference && ($('[name=KnownMoreThanOneYear]:checked').val() != 'false'));
                        validReference = (validReference && ($('[name=IsReference18]:checked').val() != 'false'));
                        validReference = (validReference && ($('#NatureOfRelationship').val() != 'Family'));

                        if ($('#ApplicantSignature').is(':visible')) {
                            //next pane
                            $btnBack.addClass('disabled');
                            $(this).button('loading');
                            //$btnContinue.addClass('disabled');
                            submitForm();
                        } else {

                            if (validReference) {
                                $btnBack.show();
                                $('.reference-invalid').hide();
                            } else {
                                //skip the questions
                                $npane = $npane.next();
                                $('.reference-invalid').show();
                                $('.reference-valid').hide();
                            }
                            $cpane.slideUp();
                            $npane.slideDown();
                            $npane.scrollTop();
                            $currPane = $npane;
                        }
                    } else {
                        //complete
                    }
                }
                return false;
            });
            var appendError = function (msg) {
                $validationMsg.append('<i class="fa fa-times-circle in-valid"></i> <span>' + msg + '</span>');
                $validationMsg.fadeIn(400);
            }
            var submitForm = function () {
                var data = loadPostData();
                $.ajax({
                    url: '/api/reference',
                    accepts: 'JSON',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    data: JSON.stringify(data),
                    headers: {
                        "Cache-Control": "no-cache, no-store, must-revalidate",
                        "Pragma": "no-cache",
                        "Expires": "0"
                    },
                    type: 'POST',
                    success: function (e) {
                        //success
                        if (e.success) {
                            $btnBack.hide();
                            $btnContinue.hide();
                            //alert('Success');
                            var $cpane = $currPane,
                                $npane = $currPane.next('.frm-pane');

                            $cpane.slideUp();
                            $npane.slideDown();
                            $npane.scrollTop();
                            $currPane = $npane;
                        } else if (e.errors.length > 0) {
                            for (var err in e.errors)
                                alert(e.errors[err]);
                            $btnBack.show();
                            $btnContinue.show();
                        }
                    },
                    error: function (e) {
                        var error = e.error();
                        alert('Error: ' + error.statusText);
                        $btnBack.show();
                        $btnContinue.show();
                    }
                });
            }
            var isValidDate = function (value, userFormat) {
                userFormat = userFormat || 'mm/dd/yyyy', // default format

                delimiter = /[^mdy]/.exec(userFormat)[0],
                theFormat = userFormat.split(delimiter),
                theDate = value.split(delimiter),

                isDate = function (date, format) {
                    var m, d, y
                    for (var i = 0, len = format.length; i < len; i++) {
                        if (/m/.test(format[i])) m = date[i]
                        if (/d/.test(format[i])) d = date[i]
                        if (/y/.test(format[i])) y = date[i]
                    }
                    return (
                      m > 0 && m < 13 &&
                      y && y.length === 4 &&
                      d > 0 && d <= (new Date(y, m, 0)).getDate()
                    )
                }

                return isDate(theDate, theFormat)

            }
        });
    </script>
</asp:Panel>
