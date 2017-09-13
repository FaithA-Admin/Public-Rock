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
                    //alert(month)
                    //alert(day);
                    if (month < 0)
                        years--;
                    else if (month == 0 && day < 0)
                        years--;
                    //alert(years)
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
                hasApplicantForm: false,
                hasReferenceForm: false,
                referencesVisible: false,
                $workflowId: null,
                $applicantPersonAliasGuid: null,
                $refList: null
            };

        pa.init = function (options) {
            var options = {
                ApplicantPersonAliasGuid: options.ApplicantPersonAliasGuid || null,
                workflowId: options.workflowId || null,
                //appForm: $('#willow_pa_applicant_form'),
                refForm: $('#willow_pa_reference_form'),
                referenceList: options.referenceList || null
            }

            pa.$refList = options.referenceList;
            pa.$workflowId = options.workflowId;
            pa.$applicantPersonAliasGuid = options.ApplicantPersonAliasGuid;

            //pa.hasApplicantForm = options.appForm[0] != undefined;
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

            //-->hide all panes except the first
            pa.$panes.hide().first().show();

            //-->enable validation // should we use jquery.validate() instead?
            pa.Validation = new WillowRockValidation({
                failureMsg: function () {
                    return pa.$invalidInput.clone();
                },
                specialFields: ["MobilePhone", "HomePhone", "ApplicantSignature", "ApplicantSignature2"],
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
                    $cntr.text('Por favor explique a detalle, le quedan ' + (min - len) + ' caracteres por teclear');
                } else {
                    $cntr.text(len + ' de ' + max + ' caracteres permitidos');
                }
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

//for references, we do in another module with same name incase we decide to split files
var WillowRock = (function (wr) {
    wr.protectionApp = (function (pa) {
        if (pa == undefined) pa = {};

        pa.References = (function (r) {
            if (r == undefined)
                //set the default for the protection application references
                r = {
                    $btnAddRef: null,
                    $referenceList: null,
                    $refPane: null,
                    $refCount: null,
                    Applicant: null,
                    form: null,
                    refCnt: 0,
                    currRefIdx: null,
                    $buttons: null,
                    $btnContinue: null,
                    $btnBack: null,
                    $validationMsg: null,
                    list: new Array(),
                    buildReferenceRow: function (row, ref)
                    {
                      var association = "";
                      switch (ref.Association)
                      {
                        case "Friend":
                          association = "Amigo";
                          break;
                        case "Colleague":
                          association = "Colega de Trabajo";
                          break;
                        case "Supervisor":
                          association = "Previo o Actual Supervisor";
                          break;
                        case "mentor":
                          association = "Previou o Actual Mentor";
                          break;
                        case "Family":
                          association = "Familiar";
                          break;

                      }

                        row.append('<div class="col-md-3">' + ref.Name + '</div>');
                        row.append('<div class="col-md-2">' + association + '</div>');
                        row.append('<div class="col-md-4">' + ref.Contact + '</div>');
                        row.append('<div class="col-md-2">' + ref.Status + '</div>');

                        if (ref.Complete == undefined || ref.Complete == 'False' || ref.Status.indexOf('Invalid') >= 0) {
                            //enable editing
                            var $cell = $('<div class="col-md-1"/>');
                            var $edit = $('<button class="btn">Editar</button>');
                            $cell.append($edit);
                            row.append($cell);
                            $edit.click(function(e) {
                                e.preventDefault();

                                var $btn = $(this);
                                var $par = $btn.parent().parent();
                                r.currRefIdx = $par.attr('data-index');
                                var ref = r.list[r.currRefIdx];
                                for (var p in ref) {
                                    var $inp = r.$refPane.find('[name=Ref' + p + ']');
                                    switch ($inp.attr('type')) {
                                    case 'checkbox':
                                    case 'radio':
                                        $inp.each(function() {
                                            var $i = $(this);
                                            if (ref[p] == $i.val()) {
                                                $i.prop('checked', true);
                                                $i.trigger('change');
                                            }
                                        });
                                        break;
                                    default:
                                        $inp.val(ref[p]);
                                        $inp.trigger('change');
                                        break;
                                    }
                                }
                                var $cpane = pa.$currPane;
                                var $RefEmail = $cpane.find('#RefEmail');
                                $RefEmail.val(ref.Contact);

                                //reduce the count for the user to update
                                if (r.refCnt > 0)
                                    r.refCnt--;
                                r.$btnAddRef.text('Terminar');
                                r.$refPane.slideDown();
                            });
                        }
                    },
                    buildReferenceList: function () {
                        for (var i in r.list) {
                            var ref = r.list[i];
                            var row = $('<div class="row form-group"></div>');
                            row.attr('data-index', i);

                            r.buildReferenceRow(row, ref);

                            r.$referenceList.append(row);
                        }
                    },
                    updateReferenceCount: function (count) {
                        r.$refCount.text(count);
                        if (count === 3) {
                            //done with references, renable screen
                            //r.$btnContinue.removeClass('ui-state-disabled');
                            //r.$btnContinue.removeAttr('disabled');
                            r.$btnContinue.show();
                            r.$refPane.slideUp();
                        } else if (count === 0) {
                            r.$referenceList.hide();
                        } else {
                            r.$btnAddRef.text('Agregar');
                        }
                    }
                };

            r.appendError = function (msg) {
                r.$validationMsg.append('<i class="fa fa-times-circle in-valid"></i> <span>' + msg + '</span>');
                r.$validationMsg.fadeIn(400);
            };
            r.reportError = function ($ctrl, msg) {
                if (msg == undefined)
                    msg = $ctrl.attr('data-validation-message');
                $ctrl.focus();
                r.appendError(msg);
            };
            r.showRefCounts = function () {
                if (r.$btnAddRef.is(':visible') ||
                    (r.$referenceList.is(':visible') && r.list.length === 3)) {
                    if (r.list.length < 3) {
                        r.$btnContinue.hide();
                        //r.$btnContinue.addClass('ui-state-disabled');
                        //r.$btnContinue.attr('disabled', '');
                    }
                    r.$refCount.parent().show();
                }
                else {
                    r.$btnContinue.show();
                    //r.$btnContinue.removeClass('ui-state-disabled');
                    //r.$btnContinue.removeAttr('disabled', '');
                    r.$refCount.parent().hide();
                }
            };
            r.init = function (options, a) {
                r.form = options.refForm;
                r.Applicant = a;
                r.$btnAddRef = $('#btnAddRef');
                r.$referenceList = $('#referenceList');
                r.list = pa.$refList;
                r.refCnt = r.list.length;
                r.$refPane = $('#refPane');
                r.$refCount = $('.refCount');
                r.$buttons = $('#willow_pa_reference_buttons');
                r.$btnContinue = $('#btnReferenceContinue');
                r.$btnBack = $('#btnReferenceBack');
                r.$resendMsg = $('.ResendText');
                r.$validationMsg = $('#reference-validation-message');

                //move the reference buttons if we have a container
                var buttonContainer = $('#buttonContainer');
                if (buttonContainer[0]) buttonContainer.append(r.$buttons);

                r.$validationMsg.hide();

                if (r.refCnt === 0)
                    r.$referenceList.hide();
                else
                    r.buildReferenceList();
                r.updateReferenceCount(r.refCnt);

                r.showRefCounts();

                r.$btnAddRef.click(function (e) {
                    e.preventDefault();

                    var $cpane = pa.$currPane;
                    var $frmRequired = $cpane.find('[required]');
                    var $RefEmail = $cpane.find('#RefEmail');
                    var $RefAssociation = $cpane.find('#RefAssociation');

                    r.$validationMsg.hide();
                    r.$validationMsg.empty();

                    //validate all required fields
                    var isValid = true;
                    var valid;
                    var reference = {};
                    var email = $RefEmail.val();

                    //validate the email and contact information
                    valid = pa.Validation.validateInput.call($RefEmail);
                    if (valid === false) {
                        r.reportError($RefEmail);
                        isValid = false;
                        return false;
                    } else {
                        for (var idx in r.list) {
                            var ref = r.list[idx];
                            if (idx !== r.currRefIdx)
                                if (email.toLowerCase() === ref.Contact.toLowerCase()) {
                                    r.reportError($RefEmail, "Las referencias deben de tener correos electrónicos distintos");
                                    isValid = false;
                                    return false;
                                }
                        }
                        reference.Contact = email;
                        reference.Status = '';
                    }

                    //make sure the reference is not a family member
                    valid = pa.Validation.validateInput.call($RefAssociation);

                    if (valid === true && $RefAssociation.val() === 'Family') {
                        r.reportError($RefAssociation, 'Miembros de familia no pueden ser puestos como referencias. Para continuar, por favor proporcionenos una nueva referencia que cumpla con los requisitos mencionados anteriormente.');
                        isValid = false;
                        return false;
                    }

                    //validate all fields
                    $frmRequired.each(function () {
                        var $ctrl = $(this);
                        valid = pa.Validation.validateInput.call(this);
                        if (valid === false) {
                            r.reportError($ctrl);
                            isValid = false;
                            return false;
                        }
                    });

                    //clear the inputs and build the reference 
                    $frmRequired.each(function () {
                        var $ctrl = $(this);
                        if ($ctrl.is(':visible')) {
                            var value = $ctrl.val();
                            switch ($ctrl.attr('type')) {
                                case 'checkbox':
                                case 'radio':
                                    if (!$ctrl.is(':checked'))
                                        value = undefined;
                                    break;
                                default:
                                    break;
                            }
                            if (value)
                                eval('reference.' + $ctrl.attr('name').replace('Ref', '') + '="' + value + '";');
                        }
                        switch ($ctrl.attr('type')) {
                            case 'checkbox':
                            case 'radio':
                                $ctrl.removeAttr('checked');
                                break;
                            default:
                                $ctrl.val('');
                                break;
                        }
                    });
                    $RefEmail.val('');

                    var row;
                    if (r.currRefIdx == null) {
                        //add the reference
                        r.list.push(reference);
                        row = $('<div class="row form-group"></div>');
                        row.attr('data-index', r.list.length - 1);
                        r.$referenceList.append(row);
                    } else {
                        //replace the reference
                        r.list.splice(r.currRefIdx, 1, reference);
                        row = $('[data-index=' + r.currRefIdx + ']');
                        row.empty();
                        r.currRefIdx = null;
                    }

                    r.buildReferenceRow(row, reference);

                    r.$referenceList.show();
                    r.$refPane.find('.validated').remove();
                    r.$refPane.find('[data-dependent-on]').hide();

                    r.refCnt++;
                    r.updateReferenceCount(r.refCnt);
                    return true;
                });

                r.$btnBack.click(function (e) {
                    e.preventDefault();

                    var $cpane = pa.$currPane,
                        $ppane = $cpane.prev('.frm-pane');
                    if ($ppane[0]) {
                        //previous pane
                        $cpane.slideUp();
                        $ppane.slideDown(function () {
                            r.showRefCounts();
                        });

                        if (r.form.is(':visible')) {
                            r.$btnBack.hide();
                            r.$btnContinue.show();
                            r.$resendMsg.show();
                        } else {
                            r.$btnBack.show();
                            r.$btnContinue.show();
                        }

                        pa.$currPane = $ppane;
                    } else {
                        //first
                        r.$btnBack.hide();
                        r.$resendMsg.show();
                    }

                    return false;
                });

                r.$btnContinue.click(function (e) {
                    e.preventDefault();
                    var $cpane = pa.$currPane;
                    var $frmRequired = $cpane.find('[required]');
                    r.$validationMsg.hide();
                    r.$validationMsg.empty();

                    //validate all required fields
                    var isValid = true;
                    if (!$('#chkAuthReleaseRef').is(':visible')) {
                        $frmRequired.each(function() {
                            var $ctrl = $(this);
                            var valid = pa.Validation.validateInput.call(this);
                            if (valid === false) {
                                var msg = $ctrl.attr('data-validation-message');
                                $ctrl.focus();
                                r.appendError(msg);
                                isValid = false;
                                return false;
                            }
                        });
                    }
                    var $npane = $cpane.next('.frm-pane');
                    if ($npane[0]) {
                        //next pane
                        if (!$('#chkAuthReleaseRef').is(':visible')) {
                            $cpane.slideUp(function () {
                                r.showRefCounts();
                            });
                            $npane.slideDown(function () {
                                r.showRefCounts();
                                window.scrollTo(0, 0);
                            });
                            pa.$currPane = $npane;
                            r.$btnBack.show();
                            r.$resendMsg.hide();
                        } else {
                            var $ctrl = $('#ApplicantSignature');
                            if (!$('#chkAuthReleaseRef').is(':checked')) {
                                r.appendError('Debe autorizar cualquier referencia en la lista para proporcionar información');
                                $('#chkAuthReleaseRef').focus();
                            }
                            else if (!pa.Validation.validateInput.call($ctrl)) {
                                var msg = $ctrl.attr('data-validation-message');
                                $ctrl.focus();
                                r.appendError(msg);
                                return false;
                            } else {
                                //r.$resendMsg.hide();
                                $(this).button('loading');
                                r.$btnBack.addClass('disabled');
                                r.submitForm();
                            }
                        }
                    }
                    return true;
                });
            };

            r.loadFormData = function () {
                return {
                    SubmissionDate: new Date(),
                    Reference1Name: r.list[0].Name,
                    Reference1Phone: '',
                    Reference1Email: r.list[0].Contact,
                    Reference1NatureOfAssociation: r.list[0].Association,
                    Reference1PersonAliasGuid: r.list[0].ReferencePersonAliasGuid,
                    Reference2Name: r.list[1].Name,
                    Reference2Phone: '',
                    Reference2Email: r.list[1].Contact,
                    Reference2NatureOfAssociation: r.list[1].Association,
                    Reference2PersonAliasGuid: r.list[1].ReferencePersonAliasGuid,
                    Reference3Name: r.list[2].Name,
                    Reference3Phone: '',
                    Reference3Email: r.list[2].Contact,
                    Reference3NatureOfAssociation: r.list[2].Association,
                    Reference3PersonAliasGuid: r.list[2].ReferencePersonAliasGuid,
                    ApplicantSsn: '999999999',
                    LegalFirstName: 'FAKE',
                    LastName: 'USER',
                    FullLegalName: 'FAKE USER',
                    WorkflowId: pa.$workflowId,
                    ApplicantPersonAliasGuid: pa.$applicantPersonAliasGuid,
                    AuthorizeReference: $('#chkAuthReleaseRef').is(':checked'),
                    Signature: $('#ApplicantSignature').val(),
                    SignatureDate: $('#ApplicantSignatureDate').val(),
                    GuardianSignature: $('#GuardianSignature').val(),
                    GuardianSignatureDate: $('#GuardianSignatureDate').val(),
                    CampusId: 0
                };
            };

            r.submitForm = function () {
                var data = r.loadFormData();
                $.ajax({
                    url: '/api/questionnaire/references_Spanish',
                    accepts: 'JSON',
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    data: JSON.stringify(data),
                    headers: {
                        "Cache-Control": "no-cache, no-store, must-revalidate",
                        "Pragma": "no-cache",
                        "Expires": "0"
                    },
                    success: function (e) {
                        //success
                        if (e.success) {
                            if (e.redirectUrl.length > 0) {
                                window.location.href = e.redirectUrl;
                            } else {
                                var $cpane = pa.$currPane,
                                    $npane = $cpane.next('.frm-pane');

                                $cpane.slideUp();
                                $npane.slideDown(400, function () { window.scrollTo(0, 0); });
                                pa.$currPane = $npane;
                                $('#pnlAppDone').hide();
                                $('#pnlWrongUser').hide();
                            }
                        } else if (e.errors.length > 0) {
                            for (var err in e.errors) {
                                var eMsg = e.errors[err];
                                alert(eMsg);
                            }
                            r.$btnBack.show();
                            r.$btnContinue.show();
                        }
                    },
                    error: function (e, status) {
                        var error = e.error();
                        alert('Error: ' + error.statusText);
                        alert(JSON.stringify(e));
                        r.$btnBack.show();
                        r.$btnContinue.show();
                    }
                });
            };

            return r;
        }(pa.References))

        return pa;
    }(wr.protectionApp))

    wr.init = function (options) {
        //init any children
        wr.protectionApp.init(options);
    }

    return wr;
}(WillowRock));


//Willow's Custom Validation method, could be split into another file.
var WillowRockValidation = function (options) {
    if (options == undefined)
        options = {
            failureMsg: function () {
                return $('<div>Error<div>');
            },
            specialFields: [],

            successMsg: function () {
                return $('<div>Éxito<div>');
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
                return ' La fecha no puede ser en el futuro';
        }
        if (!valid) return ' Fecha Inválida';
        else return '';
    }
    vld.validateEmail = function (val) {
        var valid = val.match(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][\d]\.|1[\d]{2}\.|[\d]{1,2}\.))((25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\.){2}(25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\]?$)/i) != null;
        if (!valid) return ' Correo Electrónico Invalido';
        else return '';
    }
    vld.validatePhone = function (val) {
        var valid = val.match(/^[\\(]{0,1}([0-9]){3}[\\)]{0,1}[ ]?([^0-1]){1}([0-9]){2}[ ]?[-]?[ ]?([0-9]){4}[ ]*((x){0,1}([0-9]){1,5}){0,1}$/i) != null;
        if (!valid) return ' Teléfono Inválido';
        else return '';
    }
    vld.validateInputField = function ($ctrl, val) {
        var min = $ctrl.attr('data-minlength') || 0;
        var valid = val && val.length > min;
        var crole = $ctrl.attr('data-role');
        var ctype = $ctrl.attr('type');
        var addMsg = (val && val.length < min) ? 'Debe de ingresar al menos ' + min + ' caracteres' : '';
        if (ctype == 'radio') {
            var $rdo = $('input[name=' + $ctrl.attr('name') + ']');
            valid = $rdo.is(':checked');
            $parent = $rdo.last().parent();
        } else if (ctype == 'date') {
            addMsg = vld.validateDate(val);
            valid = addMsg.length == 0;
            //alert('validating ' + val + ' as date is ' + valid);
        } else if (ctype == 'email') {
            addMsg = vld.validateEmail(val);
            valid = addMsg.length == 0;
            //alert('validating ' + val + ' as email is ' + valid);
        } else if (ctype == 'tel') {
            addMsg = vld.validatePhone(val);
            valid = addMsg.length == 0;
            //alert('validating ' + val + ' as phone is ' + valid);
        } else if (crole != undefined) {
            switch (crole) {
                case 'date':
                    addMsg = vld.validateDate(val);
                    valid = addMsg.length == 0;
                    //alert('validating ' + val + ' as date is ' + valid);
                    break;
                case 'email':
                    addMsg = vld.validateEmail(val);
                    valid = addMsg.length == 0;
                    //alert('validating ' + val + ' as email is ' + valid);
                    break;
                case 'phone':
                    addMsg = vld.validatePhone(val);
                    valid = addMsg.length == 0;
                    //alert('validating ' + val + ' as phone is ' + valid);
                    break;
            }
        }
        return { valid: valid, msg: addMsg };
    }
    vld.validateInput = function () {
        var $ctrl = $(this);
        var $parent = $ctrl.parent();
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
            //alert(isSpecial)
            //alert(ctrlId)
            var matchKey = $ctrl.attr('data-validation-match');
            if (isSpecial) {
                switch (ctrlId) {
                    case "ApplicantSignature":
                    case "ApplicantSignature2":
                        var sig = $('#' + ctrlId).val();
                        var name = $('#FullLegalName').val();
                        var nameo = name;
                        sig = sig.replace(/\s+/gi, '');
                        name = name.replace(/\s+/gi, '');

                        if (sig != name) {
                            valid = false;
                            addMsg = " Su firma debe coincidir: " + nameo;
                        }

                        break;
                    case "MobilePhone":
                    case "HomePhone":
                        var home = $('#HomePhone').val();
                        var mobile = $('#MobilePhone').val();
                        //alert(home)
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
                    addMsg = " Debe coincidir";
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
            var width = $parent.outerWidth(true);// - msg.outerWidth(true)-3;
            //if (width > 25)
            //    alert(width);
            msg.css({ left: width + 'px' })
            return valid;
        } else {
            return true;
        }
    };
    return vld;
};