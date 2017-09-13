<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReferenceForm_Spanish.ascx.cs" Inherits="Plugins_org_willowcreek_ProtectionApp_ReferenceForm_es" %>
<link href="/Styles/WillowCreek/ProtectionApp.css" type="text/css" rel="stylesheet" />

<asp:Panel ID="pnlAppDone" runat="server" ClientIDMode="Static">
    <p>Muchas gracias por su pronta respuesta y la voluntad de llenar esta solicitud de referencia! Hemos enviado tres solicitudes de referencia y para este proceso solo necesitamos tener respuesta de dos. Una vez que hemos recibido dos referencias, el enlace a la tercera solicitud de referencia expira, como es el caso aquí. Muchas gracias por estar dispuesto a llenar la solicitud  de referenica, a menos de que tenga alguna duda o preocupación, él o ella tiene todo listo.</p>
    
    <p>Si tiene alguna inquietud o duda, comuníquese con el Departamento de Protección por teléfono al: 224-512-1920, o por correo electrónico a través de la siguiente dirección: <a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a>.</p>
</asp:Panel>

<asp:Panel ID="pnlReference" runat="server">
    <div class="frm-pane-wrapper" <%--style="max-width: 570px;"--%>>
        <fieldset class="frm-pane" runat="server" visible="true">
            <div class="row form-group">
                  <div class="col-md-6">
                       <h3>Referencia para el Voluntario</h3>
                  </div>
                <div class="col-md-6">
                   <b><i><a id="translateToEnglish" runat="server">Click here to see this page in English.</a></i></b>
                </div>
            </div>
            <hr />
            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="Today">Fecha de Hoy</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="Today" name="Today" class="form-control" value="<%=DateTime.Now %>" disabled />
                </div>
            </div>
            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="FirstName">Primer Nombre</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="FirstName" name="FirstName" value="<%=FirstName %>" class="form-control" placeholder="Primer Nombre" required data-validation-message="Se requiere el Primer Nombre" />
                </div>
            </div>
            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="MiddleName">Segundo Nombre</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="MiddleName" name="MiddleName" value="<%=MiddleName %>" class="form-control" placeholder="Segundo Nombre" />
                </div>
            </div>
            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="LastName">Apellido(s)</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="LastName" name="LastName" value="<%=LastName %>" class="form-control" placeholder="Apellido(s)" required data-validation-message="Se requieren el Apellido(s)" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="">¿Conoce al solicitante por más de un año?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="KnownMoreThanOneYear_Yes" name="KnownMoreThanOneYear" value="true" required data-validation-message="¿Conoce al solicitante por más de un año?" />
                        Si
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="KnownMoreThanOneYear_No" name="KnownMoreThanOneYear"  value="false" required data-validation-message="¿Conoce al solicitante por más de un año?" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="">¿Tiene 18 años o más?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="IsReference18_Yes" name="IsReference18" value="true" required data-validation-message="¿Tiene 18 años o más?" />
                        Si
                    </label>               
                    <label class="radiolabel">
                        <input type="radio" id="IsReference18_No" name="IsReference18" value="false" required data-validation-message="¿Tiene 18 años o más?" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="NatureOfRelationship">¿Cuál es la naturaleza de su relación con el solicitante?</label>
                </div>
                <div class="col-md-6">
                    <select id="NatureOfRelationship" name="NatureOfRelationship" class="form-control">
                        <option value="Friend">Amigo</option>
                        <option value="Colleague">Colega de trabajo</option>
                        <option value="Supervisor">Supervisor Previo o Actual</option>
                        <option value="mentor">Mentor Previo o Actual</option>
                        <option value="Family">Familiar</option>
                    </select>
                </div>
            </div>

        </fieldset>

        <fieldset class="frm-pane">
            <legend>Preguntas</legend>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Es el solicitante capaz de mantener relaciones interpersonales significativas (sin incluir relaciones familiares)?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="MaintainRelationships_Yes" name="MaintainRelationships" value="true" required data-validation-message="¿Es el solicitante capaz de mantener relaciones interpersonales significativas (sin incluir relaciones familiares)?" />
                        Si
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="MaintainRelationships_No" name="MaintainRelationships" value="false" required data-validation-message="¿Es el solicitante capaz de mantener relaciones interpersonales significativas (sin incluir relaciones familiares)?" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="MaintainRelationships" data-dependent-value="false">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="MaintainRelationships_Explain" class="form-control" required data-validation-message="Se requiere explicación sobre si el solicitante es capaz de mantener relaciones interpersonales significativas (sin incluir familiares)" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Alguna vez se ha preocupado por la capacidad del solicitante para respetar los límites relacionales saludables?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="RespectHealthyRelationalBoundaries_Yes" name="RespectHealthyRelationalBoundaries" value="true" required data-validation-message="¿Alguna vez se ha preocupado por la capacidad del solicitante para respetar los límites relacionales saludables?" />
                        Si
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="RespectHealthyRelationalBoundaries_No" name="RespectHealthyRelationalBoundaries" value="false" required data-validation-message="¿Alguna vez se ha preocupado por la capacidad del solicitante para respetar los límites relacionales saludables?" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="RespectHealthyRelationalBoundaries" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="RespectHealthyRelationalBoundaries_Explain" class="form-control" required data-validation-message="Se requiere explicación sobre si alguna vez se ha preocupado por la capacidad del solicitante para respetar los límites relacionales saludables." data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Conoce si el solicitante ha cometido algún tipo de delito?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="CriminalOffenses_Yes" name="CriminalOffenses" value="true" required data-validation-message="¿Conoce si el solicitante ha cometido algún tipo de delito?" />
                        Si
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="CriminalOffenses_No" name="CriminalOffenses" value="false" required data-validation-message="¿Conoce si el solicitante ha cometido algún tipo de delito?" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="CriminalOffenses" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="CriminalOffenses_Explain" class="form-control" required data-validation-message="Se requiere explicación sobre sobre si tiene conocimiento de que el solicitante haya cometido algun tipo de delito" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Tiene el solicitante algún patrón de conducta que se perciba como engañoso o manipulador?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="ManipulativeBehavior_Yes" name="ManipulativeBehavior" value="true" required data-validation-message="¿Tiene el solicitante algún patrón de conducta que se perciba como engañoso o manipulador?" />
                        Si
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="ManipulativeBehavior_No" name="ManipulativeBehavior" value="false" required data-validation-message="¿Tiene el solicitante algún patrón de conducta que se perciba como engañoso o manipulador?" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="ManipulativeBehavior" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="ManipulativeBehavior_Explain" class="form-control" required data-validation-message="Se require explicación sobre si el aplicante tiene algún patrón de conducta que se perciba como engañoso o manipulador" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Está usted al tanto de cualquier acto del solicitante que involucre el daño físico, sexual o emocionall de otra persona (incluyendo, pero no limitado a abuso sexual, orden de protección, negligencia, explotación, agreción, investigación por parte del DCFS, Department of Child and Family Service (Departamento de Servicios para Niños y Familias), etc.)?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="InflictedEmotionalHarm_Yes" name="InflictedEmotionalHarm" value="true" required data-validation-message="¿Está usted al tanto de cualquier acto del solicitante que involucre el daño físico, sexual o emocionall de otra persona (incluyendo, pero no limitado a abuso sexual, orden de protección, negligencia, explotación, agreción, investigación por parte del DCFS, Department of Child and Family Service (Departamento de Servicios para Niños y Familias), etc.)?" />
                        Si
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="InflictedEmotionalHarm_No" name="InflictedEmotionalHarm" value="false" required data-validation-message="¿Está usted al tanto de cualquier acto del solicitante que involucre el daño físico, sexual o emocionall de otra persona (incluyendo, pero no limitado a abuso sexual, orden de protección, negligencia, explotación, agreción, investigación por parte del DCFS, Department of Child and Family Service (Departamento de Servicios para Niños y Familias), etc.)?" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="InflictedEmotionalHarm" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="InflictedEmotionalHarm_Explain" class="form-control" required data-validation-message="Se requiere explicación sobre si usted esta al tanto de cualquier acto del solicitante que involucre el daño físico, sexual o emocionall de otra persona (incluyendo, pero no limitado a abuso sexual, orden de protección, negligencia, explotación, agreción, investigación por parte del DCFS (Department of Children & Family Services), etc.)" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>Si usted tiene o tendría hijos, ¿confiaría en esta persona para cuidarlos?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="TrustInChildCare_Yes" name="TrustInChildCare" value="true" required data-validation-message="Si usted tiene o tuviera hijos, ¿confiaría en esta persona para cuidarlos?" />
                        Si
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="TrustInChildCare_No" name="TrustInChildCare" value="false" required data-validation-message="Si usted tiene o tuviera hijos, ¿confiaría en esta persona para cuidarlos?" />
                        No
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="TrustInChildCare" data-dependent-value="false">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="TrustInChildCare_Explain" class="form-control" required data-validation-message="Se requiere explicación sobre si usted confiaría (si tiene) sus hijos al cuidado de esta persona" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Recomendaría al solicitante como voluntario?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="WouldRecommend_Yes" name="WouldRecommend" value="1" required data-validation-message="¿Recomendaría al solicitante como voluntario?" />
                        Si
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="WouldRecommend_No" name="WouldRecommend" value="0" required data-validation-message="¿Recomendaría al solicitante como voluntario?" />
                        No
                    </label>
                    <label class="radiolabel">
                        <input type="radio" id="WouldRecommend_Depends" name="WouldRecommend" value="2" required data-validation-message="¿Recomendaría al solicitante como voluntario?" />
                        Depende
                    </label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="WouldRecommend" data-dependent-value="0,2">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="WouldRecommend_Explain" class="form-control" required data-validation-message="Se require explicación sobre si usted recomendaría al solicitante como voluntario" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>

        </fieldset>

        <fieldset class="frm-pane">
            <legend>Signature</legend>
            <div class="row form-group reference-invalid">
                <div class="col-md-12">
                    Gracias por su disposición a proporcionar una referencia para este solicitante. Desafortunadamente, al menos una respuesta en la página anterior nos hace incapaces de utilizarlo como referencia. Nos disculpamos por cualquier inconveniente. Si tiene alguna inquietud acerca de que el solicitante sirva en Willow Creek Community Church, por favor envíenos un correo electrónico directamente a <a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a>.
               
                </div>
            </div>
            <div class="row form-group reference-valid">
                <div class="col-md-12">
                    Gracias por tomarse el tiempo para completar esta referencia. Su participación nos ayuda a crear un ambiente acogedor y seguro en Willow Creek. Para completar su referencia, por favor proporcione su firma electrónica a continuación.
               
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                   Al escribir mi nombre y apellido abajo, afirmo que la información en esta referencia es exacta y veraz, según mi leal saber y entender.
               
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-4">
                    <label>Firma de la Referencia</label>
                </div>
                <div class="col-md-8">
                    <input type="text" id="ApplicantSignature" data-role="sign" class="form-control" required data-validation-message="Se requiere la firma de la referencia" />

                    <input type="text" id="ApplicantSignatureDate" class="form-control" value="<%=DateTime.Now.ToString("M/d/yyyy") %>" disabled style="width: 100px" />
                </div>
            </div>
        </fieldset>

        <fieldset class="frm-pane">
            <legend>Gracias por su participación!</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    <p>Gracias por tomarse el tiempo para completar esta referencia. La información que nos proporcionó nos ayuda a crear un ambiente acogedor y seguro en Willow Creek!</p>
                    <p>Si desea explicar con mayor profundidad cualquier información que haya proporcionado, no dude en ponerse en contacto con el Departamento de Protección.</p>
                    <p><a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a></p>
                    <p>224-512-1920</p>
                </div>
            </div>
        </fieldset>

        <div>
            <button id="btnBack" class="btn" style="display: none;">Atrás</button>
            <a type="button" id="btnContinue" class="btn btn-primary" data-loading-text="<i class='fa fa-refresh fa-spin'></i> Por Favor Espere">Continuar</a>
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
                        return ' La fecha no puede ser en el futuro';

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
                if (!valid) return ' Fecha Inválida';
                else return '';
            }
            var validateEmail = function (val) {
                var valid = val.match(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][\d]\.|1[\d]{2}\.|[\d]{1,2}\.))((25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\.){2}(25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\]?$)/i) != null;
                if (!valid) return ' Correo Electrónico Inválido';
                else return '';
            }
            var validatePhone = function (val) {
                var valid = val.match(/^[\\(]{0,1}([0-9]){3}[\\)]{0,1}[ ]?([^0-1]){1}([0-9]){2}[ ]?[-]?[ ]?([0-9]){4}[ ]*((x){0,1}([0-9]){1,5}){0,1}$/i) != null;
                if (!valid) return ' Teléfono Inválido';
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
                                addMsg = " Por favor ingrese su primer nombre y apellido.";
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
                            addMsg = " Debe coincidir";
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
                    $cntr.text('Por favor explique a detalle, tiene ' + (min - len) + ' caracteres por teclear');
                } else {
                    $cntr.text(len + ' de ' + max + ' caracteres disponibles');
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
                    url: '/api/reference_Spanish',
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
