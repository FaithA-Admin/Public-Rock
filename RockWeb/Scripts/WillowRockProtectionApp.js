//var WillowRock = (function (wr) {
//    wr.genericStub = (function (g) {
//        if (g == undefined) g = {};

//        return g;
//    }(wr.genericStub))

//    return wr;
//}(WillowRock));

var WillowRock = (function (wr) {
    if (wr == undefined) wr = {
        //set the defaults for Willow
    };
    wr.utils = (function (u) {
        if (u == undefined)
            u = {
                calculateAge: function (birthdate) {
                    var today = new Date()
                    var dob = new Date(birthdate);
                    var years = today.getFullYear() - dob.getFullYear();
                    var month = today.getMonth() - dob.getMonth();
                    var day = today.getDate() - dob.getDate();
                    if (month < 0)
                        years--;
                    else if (month == 0 && day < 0)
                        years--;
                    return years;
                }
            };
        return u;
    }(wr.utils));

    wr.protectionApp = (function (pa) {
        if (pa == undefined)
            pa = {
                //set the default for the protection application, shared between Applicant and Reference
                $validInput: null,
                $invalidInput: null,
                $panes: null,
                $currPane: null,
                $dependents: null,
                $dependentDict: null,
                $maxLengthFields: null,
                $requiredFields: null,
                $btnYesIm: null,
                $btnNoIm: null,
                $guardian: null,
                hasApplicantForm: false,
                hasReferenceForm: false,
                referencesVisible: false
            };

        pa.init = function (options) {
            var options = {
                personId: options.personId || null,
                workflowId: options.workflowId || null,
                appForm: $('#willow_pa_applicant_form'),
                refForm: $('#willow_pa_reference_form')
            }
            

            pa.hasApplicantForm = options.appForm[0] != undefined;
            pa.hasReferenceForm = options.refForm[0] != undefined;

            if (pa.hasReferenceForm && pa.References) pa.References.init(options, pa.Applicant);
            if (pa.hasApplicantForm && pa.Applicant) pa.Applicant.init(options, pa.References);

            pa.$validInput = $('<span class="validated"><i class="fa fa-check-circle valid"></i></span>');
            pa.$invalidInput = $('<span class="validated"><i class="fa fa-times-circle in-valid"></i></span>');
            pa.$panes = $('.frm-pane');
            pa.$currPane = $(pa.$panes.get(0));
            pa.$dob = $('#DateOfBirth');
            pa.$dependents = $('[data-dependent-on]');
            pa.$dependentDict = new Array();
            pa.$maxLengthFields = $('[maxlength]');
            pa.$requiredFields = $('[required]');
            pa.$btnYesIm = $('#btnYesIm');
            pa.$btnNoIm = $('#btnNoIm');
            pa.$guardian = $('input[type=radio][name=Guardian]');

            //-->hide all panes except the first
            pa.$panes.hide().first().show();
            //-->hide all dependent questions
            pa.$dependents.hide();
            pa.$dependents.each(function () {
                var depOn = $(this).attr('data-dependent-on');
                var depVal = $(this).attr('data-dependent-value');
                var isClassDep = depOn.indexOf('.') == 0;
                var $trigger = isClassDep ? $(depOn) : $('#' + depOn);
                if ($trigger[0] == undefined)
                    $trigger = $('[name=' + depOn + ']');

                if (pa.$dependentDict[depOn] == undefined) {
                    pa.$dependentDict[depOn] = new Array();
                }
                if (pa.$dependentDict[depOn][depVal] == undefined) {
                    var toggleDependencies = function () {
                        var $trig = $(this);
                        var dep = $trig.attr('id');
                        var val = $trig.val();
                        if ($trig.attr('type') == 'checkbox')
                            val = '' + $trig.is(':checked');

                        var $deps = pa.$dependentDict[dep];
                        if (!$deps) {
                            dep = $trig.attr('name');
                            $deps = pa.$dependentDict[dep];
                        }
                        if (!$deps) {
                            dep = $trig.attr('data-dependent-trigger');
                            $deps = pa.$dependentDict[dep];
                        }

                        for (var p in $deps) {
                            var ctrls = $deps[p];
                            var deps = ctrls.dependents;

                            var show = val == ctrls.value;
                            if (dep == '.recent') {
                                var trgs = $('.recent[value=1], .recent[value=2]');
                                var hasRecent = false;
                                trgs.each(function () {
                                    if ($(this).is(':checked')) {
                                        hasRecent = true;
                                        return false;
                                    }
                                });
                                if (hasRecent || show && !deps.is(':visible'))
                                    deps.show();
                                else if (!hasRecent)
                                    deps.hide();
                            } else {
                                deps.hide();
                            }
                        }

                        var ctrls = $deps[val];
                        if (ctrls != undefined) {
                            $deps = ctrls.dependents;
                            var show = val == ctrls.value;
                            $deps.toggle(show);
                        }
                    }
                    $trigger.change(toggleDependencies);

                    pa.$dependentDict[depOn][depVal] = {
                        trigger: $trigger,
                        dependents: $('[data-dependent-on="' + depOn + '"][data-dependent-value="' + depVal + '"]'),
                        value: depVal
                    };
                }
            });

            //-->enable validation // should we use jquery.validate() instead?
            pa.Validation = new WillowRockValidation({
                failureMsg: function () {
                    return pa.$invalidInput.clone();
                },
                specialFields: ["MobilePhone", "HomePhone", "ApplicantSignature", "ApplicantSignature2", "GuardianSignature"],
                successMsg: function () {
                    return pa.$validInput.clone();
                }
            });

            pa.$requiredFields.blur(pa.Validation.validateInput);
            pa.$requiredFields.change(pa.Validation.validateInput);
            pa.$maxLengthFields.keydown(function (e) {
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
            pa.$maxLengthFields.keyup(function (e) {
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

            pa.$dob.change(function () {
                if (WillowRock.utils.calculateAge(pa.$dob.val()) < 18) {
                    $('#Guardian_Yes').prop('checked', true);
                    $('#guardianDiv').hide();
                    $('#guardianSignatureDiv').show();
                    $('#GuardianSignature').val('');
                } else {
                    $('#Guardian_No').prop('checked', true);
                    $('#guardianDiv').show();
                    $('#guardianSignatureDiv').hide();
                    $('#GuardianSignature').val('');
                }
            });

            pa.$guardian.change(function() {
                if (this.value == 'true') {
                    $('#guardianSignatureDiv').show();
                    $('#GuardianSignature').val('');
                }
                else if (this.value == 'false') {
                    $('#guardianSignatureDiv').hide();
                    $('#GuardianSignature').val('');
                }
            });
                       
            //ui events
            pa.$btnYesIm.click(function (e) {
                e.preventDefault();
                var $cpane = pa.$currPane,//$($panes.get(currIdx)),
                    $npane = $cpane.next('.frm-pane');//$($panes.get(currIdx - 1));
                $cpane.slideUp();
                $npane.slideDown(function () {
                    window.scrollTo(0, 0);
                });
                pa.Applicant.$btnContinue.show();
                pa.$btnYesIm.hide();
                pa.$btnNoIm.hide();
                pa.$currPane = $npane;

                if (WillowRock.utils.calculateAge(pa.$dob.val()) < 18) {
                    $('#Guardian_Yes').prop('checked', true);
                    $('#guardianDiv').hide();
                    $('#guardianSignatureDiv').show();
                    $('#GuardianSignature').val('');
                } else {
                    $('#Guardian_No').prop('checked', true);
                    $('#guardianDiv').show();
                    $('#guardianSignatureDiv').hide();
                    $('#GuardianSignature').val('');
                }
            });
            pa.$btnNoIm.click(function (e) {
                e.preventDefault();
                var $cpane = pa.$currPane,
                    $npane = $('#pnlWrongUser');
                $cpane.slideUp();
                $npane.slideDown(function () {
                    window.scrollTo(0, 0);
                });
                pa.$btnYesIm.hide();
                pa.$btnNoIm.hide();
                pa.$currPane = $npane;
            });
        };

        return pa;
    }(wr.protectionApp))

    wr.init = function (options) {
        //init any children
        wr.protectionApp.init(options);
    }
    return wr;
}(WillowRock));


//for Applicant, we do in another module with same name incase we decide to split files
var WillowRock = (function (wr) {
    wr.protectionApp = (function (pa) {
        if (pa == undefined) pa = {};

        pa.Applicant = function (a) {
            if (a == undefined)
                a = {
                    $buttons: null,
                    $btnContinue: null,
                    $btnBack: null,
                    $validationMsg: null,
                    form: null
                    //,References: null
                };

            a.appendError = function (msg) {
                a.$validationMsg.append('<i class="fa fa-times-circle in-valid"></i> <span>' + msg + '</span>');
                a.$validationMsg.fadeIn(400);
            };

            a.setLegalName = function () {
                var $firstName = $('#LegalFirstName');
                var $middleName = $('#MiddleName');
                var $lastName = $('#LastName');
                var $suffix = $("#Suffix option:selected").text();

                //JR - 04/21/2016 Removed "replace" from code so that blank spaces in names are left intact. 
                //var first = $firstName.val().replace(/\s+/g, '');
                //var middle = $middleName.val().replace(/\s+/g, '');
                //var last = $lastName.val().replace(/\s+/g, '');
                var first = $firstName.val().trim();        
                var middle = $middleName.val().trim();               
                var last = $lastName.val().trim();
                var suffix = $suffix.trim();
                $('#FullLegalName').val(first + (middle.length > 0 ? (' ' + middle) : '') + ' ' + last + ' ' + suffix);
            }
            a.init = function (options, r) {
                a.personId = options.personId,
                a.workflowId = options.workflowId,
                a.ApplicantPersonAliasGuid = options.ApplicantPersonAliasGuid,
                a.form = options.appForm;
                a.$buttons = $('#willow_pa_applicant_buttons');
                a.$btnContinue = $('#btnApplicantContinue');
                a.$btnBack = $('#btnApplicantBack');
                a.$validationMsg = $('#applicant-validation-message');
                a.$validationMsg.hide();
                a.$refValidationMsg = $('#reference-validation-message');
                a.$refValidationMsg.hide();

                a.$btnBack.click(function (e) {
                    e.preventDefault();

                    var $cpane = pa.$currPane,
                        $ppane = $cpane.prev('.frm-pane');
                    if ($ppane[0]) {
                        //previous pane
                        $cpane.slideUp(400, function () {
                            if ($('#LegalFirstName').is(':visible'))
                                a.$btnBack.hide();
                        });
                        $ppane.slideDown();

                        a.$btnContinue.show();
                        pa.$currPane = $ppane;
                    } else {
                        //first
                        a.$btnBack.hide();
                    }
                    return false;
                });
                a.$btnContinue.click(function (e) {
                    e.preventDefault();
                    var $cpane = pa.$currPane,
                        $npane = $cpane.next('.frm-pane');
                    var $frmRequired = $cpane.find('[required]');
                    a.$validationMsg.hide();
                    a.$validationMsg.empty();

                    //validate all required fields
                    var isValid = true;
                    $frmRequired.each(function () {
                        var $ctrl = $(this);
                        var valid = pa.Validation.validateInput.call(this);
                        if (valid == false) {
                            var msg = $ctrl.attr('data-validation-message');
                            $ctrl.focus();
                            a.appendError(msg);
                            isValid = false;
                            return false;
                        }
                    });
                    if (isValid) {
                        if ($npane[0]) {
                            //next pane
                            if ($cpane.find('#chkCorrectInfo')[0]) {
                                if (!$('#chkCorrectInfo').is(':checked')) {
                                    a.appendError('You must acknowledge that the information provided is correct');
                                    $('#chkCorrectInfo').focus();
                                } else if (!$('#chkAuthReleaseInfo').is(':checked')) {
                                    a.appendError('You must authorize the release of your information');
                                    $('#chkAuthReleaseInfo').focus();
                                } else {
                                    $(this).button('loading');
                                    a.$btnBack.addClass('disabled');
                                    //a.$btnContinue.addClass('disabled');
                                    a.submitForm();
                                }
                            } else {
                                $cpane.slideUp();
                                $npane.slideDown(400, function () {
                                    window.scrollTo(0, 0);
                                });

                                a.setLegalName();
                                a.$btnBack.show();
                                pa.$currPane = $npane;

                                //do we have a legal guardian?
                                var $guardianSignature = $('#GuardianSignature');
                                if ($guardianSignature.is(':visible')) {
                                    if ($('#Guardian_Yes').is(':checked'))
                                        $guardianSignature.closest('div.row').show();
                                }
                            }
                        } else {
                            //complete
                            alert('complete')
                        }
                    }
                    return false;
                });
            };

            a.loadFormData = function (personId, workflowId) {
                var childCount = parseInt($('#ChildrenCount').val());
                var suffix = parseInt($('#Suffix').val());
                var data = {
                    SubmissionDate: new Date(),
                    ApplicantPersonId: personId,
                    WorkflowId: workflowId,
                    ApplicantPersonAliasGuid: $('#ApplicantPersonAliasGuid').val(),
                    ApplicantSsn: $('#ApplicantSsn').val(),
                    LegalFirstName: $('#LegalFirstName').val().trim(),
                    MiddleName: $('#MiddleName').val().trim(),
                    LastName: $('#LastName').val().trim(),
                    Suffix: suffix,
                    FullLegalName: $('#FullLegalName').val().trim(),
                    Nickname: $('#Nickname').val().trim(),
                    MaidenName: $('#PreviousName').val().trim(),
                    CurrentAddressStreet: $('#CurrentAddressStreet').val(),
                    CurrentAddressCity: $('#CurrentAddressCity').val(),
                    CurrentAddressState: $('#CurrentAddressState').val(),
                    CurrentAddressZip: $('#CurrentAddressZip').val(),
                    TimeAtCurrentAddress: $('[name=livedCurrentMoreThan12]:checked').val(),
                    PreviousAddressStreet: $('#PreviousAddressStreet').val(),
                    PreviousAddressCity: $('#PreviousAddressCity').val(),
                    PreviousAddressState: $('#PreviousAddressState').val(),
                    PreviousAddressZip: $('#PreviousAddressZip').val(),
                    HomePhone: $('#HomePhone').val(),
                    MobilePhone: $('#MobilePhone').val(),
                    EmailAddress: $('#EmailAddress').val(),
                    Gender: $('[name=Gender]:checked').val(),
                    DateOfBirth: $('#DateOfBirth').val(),
                    MaritalStatus: $('#Marital_Status').val(),
                    ChildrenCount: isNaN(childCount) ? 0 : childCount,
                    ChildrenAges: $('#Ages').val(),
                    LegalGuardian: $('[name=Guardian]:checked').val(),
                    AttendWccc: $('[name=WCCC]:checked').val(),
                    StartWccc: $('#WCCC_Date').val(),
                    PornographyAddiction: $('[name=Porno]:checked').val(),
                    AlcoholAddiction: $('[name=Alcohol]:checked').val(),
                    DrugAddiction: $('[name=Drugs]:checked').val(),
                    AddictionExplain: $('#Addiction_Explain').val(),
                    PornographyVulnerable: $('[name=lookedAtPorn]:checked').val(),
                    PornographyVulnerableExplain: $('#lookedAtPorn_Explain').val(),
                    DcfsInvestigation: $('[name=DCFS]:checked').val(),
                    DcfsInvestigationExplain: $('#DCFS_Explain').val(),
                    OrderOfProtection: $('[name=OOP]:checked').val(),
                    OrderOfProtectionExplain: $('#OOP_Explain').val(),
                    CommittedOrAccused: $('[name=ComittedOrAccused]:checked').val(),
                    CommittedOrAccusedExplain: $('#ComittedOrAccused_Explain').val(),
                    RelationshipVulnerable: $('[name=RelWithVuln]:checked').val(),
                    RelationshipVulnerableExplain: $('#RelWithVuln_Explain').val(),
                    AskedToLeave: $('[name=Misconduct]:checked').val(),
                    AskedToLeaveExplain: $('#Misconduct_Explain').val(),
                    CorrectInfo: $('#chkCorrectInfo').is(':checked'),
                    AuthorizeRelease: $('#chkAuthReleaseInfo').is(':checked'),
                    AuthorizeReference: $('#chkAuthReleaseRef').is(':checked'),
                    Signature: $('#ApplicantSignature').val(),
                    SignatureDate: $('#ApplicantSignatureDate').val(),
                    GuardianSignature: $('#GuardianSignature').val(),
                    GuardianSignatureDate: $('#GuardianSignatureDate').val(),
                    CampusId: 0
                };

                return data;
            };

            a.submitForm = function () {
                var data = a.loadFormData(a.personId, a.workflowId);
                $.ajax({
                    url: '/api/questionnaire',
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
                            if (e.redirectUrl.length > 0) {
                                window.location.href = e.redirectUrl;
                            } else {
                                var $cpane = pa.$currPane,
                                    $npane = $cpane.next('.frm-pane');

                                $cpane.slideUp();
                                $npane.slideDown(400, function() { window.scrollTo(0, 0); });
                                pa.$currPane = $npane;
                            }

                        } else if (e.errors.length > 0) {
                            var ssnError = false;
                            for (var err in e.errors) {
                                var eMsg = e.errors[err];
                                alert(eMsg);
                                if (eMsg.indexOf('Social Security Number') > 0) {
                                    ssnError = true;
                                    $('#ApplicantSsn').focus();
                                }
                            }
                            a.$buttons.show();
                        }
                    },
                    error: function (e) {
                        var error = e.error();
                        alert('Error: ' + error.statusText + " - " + error.responseText);
                        a.$buttons.show();
                    }
                });
            };

            return a;
        }(pa.Applicant);

        return pa;
    }(wr.protectionApp))

    return wr;
}(WillowRock));

//Willow's Custom Validation method, could be split into another file.
var WillowRockValidation = function (options) {
    if (options == undefined)
        options = {
            failureMsg: function () {
                return $('<div>Failed<div>');
            },
            specialFields: [],

            successMsg: function () {
                return $('<div>Success<div>');
            }
        }
    var vld = this;
    vld.failureMsg = options.failureMsg;
    vld.specialFields = options.specialFields;
    vld.successMsg = options.successMsg;
    vld.isValidDate = function (value, userFormat) {
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

    };
    vld.validateDate = function (val) {
        var valid = val.match(/^\d{1,2}\/\d{1,2}\/\d{2,4}$/i) != null;
        if (!valid)//date doesn't match 01/01/2015 lets check 2015-01-01
            valid = val.match(/^\d{2,4}(\/)|(-)\d{1,2}(\/)|(-)\d{1,2}$/i) != null;
        if (valid)//check to make sure that it is an actual valid date
            valid = vld.isValidDate(val);
        if (valid) {
            //make sure the date is not in the future
            var date = new Date(val);
            var now = new Date();
            now.setHours(0, 0, 0, 0);

            if (date > now)
                return ' Date cannot be in the future';
        }
        if (!valid) return ' Invalid Date';
        else return '';
    }
    vld.validateEmail = function (val) {
        var valid = val.match(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][\d]\.|1[\d]{2}\.|[\d]{1,2}\.))((25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\.){2}(25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\]?$)/i) != null;
        if (!valid) return ' Invalid Email';
        else return '';
    }
    vld.validatePhone = function (val) {
        var valid = val.match(/^[\\(]{0,1}([0-9]){3}[\\)]{0,1}[ ]?([^0-1]){1}([0-9]){2}[ ]?[-]?[ ]?([0-9]){4}[ ]*((x){0,1}([0-9]){1,5}){0,1}$/i) != null;
        if (!valid) return ' Invalid Phone';
        else return '';
    }
    vld.validateInputField = function ($ctrl, val) {
        var min = $ctrl.attr('data-minlength') || 0;
        var valid = val && val.length > min;
        var crole = $ctrl.attr('data-role');
        var ctype = $ctrl.attr('type');
        var addMsg = (val && val.length < min) ? 'You must enter at least ' + min + ' characters' : '';
        if (ctype == 'radio') {
            var $rdo = $('input[name=' + $ctrl.attr('name') + ']');
            valid = $rdo.is(':checked');
            //$parent = $rdo.parent().parent(); //.parent();

        } else if (ctype == 'date') {
            addMsg = vld.validateDate(val);
            valid = addMsg.length == 0;
        } else if (ctype == 'email') {
            addMsg = vld.validateEmail(val);
            valid = addMsg.length == 0;
        } else if (ctype == 'tel') {
            addMsg = vld.validatePhone(val);
            valid = addMsg.length == 0;
        } else if (crole != undefined) {
            switch (crole) {
                case 'date':
                    addMsg = vld.validateDate(val);
                    valid = addMsg.length == 0;
                    break;
                case 'email':
                    addMsg = vld.validateEmail(val);
                    valid = addMsg.length == 0;
                    break;
                case 'phone':
                    addMsg = vld.validatePhone(val);
                    valid = addMsg.length == 0;
                    break;
            }
        }
        return { valid: valid, msg: addMsg };
    }
    vld.validateInput = function () {
        var $ctrl = $(this);
        var $parent = $ctrl.parent();
        var $ctrltype = $ctrl.prop('type');
        if ($ctrltype == 'radio') {
            $parent = $parent.parent();
        }
        var ctrlId = this.id;
        var val = $ctrl.val();

        if ($ctrl.is(':visible') == true) {
            var isSpecial = false;
            var valid = true;
            var addMsg = '';

            for (var s in vld.specialFields) {
                if (vld.specialFields[s] == ctrlId) {
                    isSpecial = true;
                    break;
                }
            }

            var vobj = vld.validateInputField($ctrl, val);
            valid = vobj.valid;
            addMsg = vobj.msg;
            var matchKey = $ctrl.attr('data-validation-match');
            if (isSpecial) {
                switch (ctrlId) {
                    case "ApplicantSignature":
                    case "ApplicantSignature2":
                        var sig = $('#' + ctrlId).val().trim();
                        var name = $('#FullLegalName').val().trim();
                        var nameo = name;

                        //JR - 4/21/2016 commented the following two lines to not remove blank spaces from names.
                        //sig = sig.replace(/\s+/gi, '');
                        //name = name.replace(/\s+/gi, '');

                        if (sig != name) {
                            valid = false;
                            addMsg = " Your signature must match: " + nameo;
                        }

                     

                        break;

                    case "GuardianSignature":
                        if ($('#GuardianSignature').val().trim() == '') {
                            valid = false;
                            addMsg = "Guardian's signature is required";
                        }
                        break;
                   
                       
                    case "MobilePhone":
                    case "HomePhone":
                        var home = $('#HomePhone').val();
                        var mobile = $('#MobilePhone').val();
                        if (home != '' && mobile != '')
                            valid = valid && true;
                        else if (mobile == '' && home == '') {
                            $parent = $('#MobilePhone').parent();
                            valid = false;
                        } else if (mobile != '' && home == '') {
                            $parent = $('#MobilePhone').parent();
                            $('#HomePhone').parent().find('span.validated').remove();
                            valid = true;
                        }
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
                msg = vld.successMsg();
            }
            else {
                msg = vld.failureMsg().append(addMsg);
            }
            $parent.append(msg);
            var width = $parent.outerWidth(true);
            msg.css({ left: width + 'px' })
            return valid;
        } else {
            return true;
        }
    };
    return vld;
};