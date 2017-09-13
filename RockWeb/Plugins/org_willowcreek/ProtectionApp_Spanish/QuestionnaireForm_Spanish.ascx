<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuestionnaireForm_Spanish.ascx.cs" Inherits="Plugins_org_willowcreek_ProtectionApp_QuestionnaireForm_es" %>
<link href="/Styles/WillowCreek/ProtectionApp.css" type="text/css" rel="stylesheet" />
<script src="/scripts/WillowRockProtectionApp_Spanish.js" type="text/javascript"></script>
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
    <div id="noscript-warning">Se requiere tener JavaScript habilitado en el navegador web para llenar la solicitud de protección</div>
    <div class="noscriptmsg">
        <h4>Hay un problema de compatibilidad con su navegador web</h4>
        Desafortunadamente no tiene JavaScript activado en su navegador web, la solicitud de protección require que la opción de JavaScript este habilitada.
        <br />
        <br />
        Por favor habilite JavaScript o utilize otro navegador web, le sugerimos utilizar <a href="https://www.google.com/chrome/browser/desktop/">Google Chrome</a>
    </div>
</noscript>

<asp:Panel ID="pnlAppDone" runat="server" ClientIDMode="Static">
    <p>La solicitud de protección ha sido previamente completada o el enlace ha expirado.</p>
    <p>Por favor contáctenos en <a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a> si tiene alguna duda sobre este proceso.</p>
</asp:Panel>

<asp:Panel ID="pnlWrongUser" runat="server" ClientIDMode="Static">
    <p>Las ligas de la solicitud de protección son únicas para el individuo en cuestión.</p>
    <p>Por favor contáctenos en <a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a> si tiene alguna duda sobre este proceso.</p>
</asp:Panel>

<asp:Panel ID="pnlQuestionnaire" runat="server">
    <div id="willow_pa_applicant_form" class="frm-pane-wrapper" style="max-width: 600px;">
        <fieldset class="frm-pane" runat="server" visible="true">
            <p>
                Antes de comenzar la solicitud de protección, por favor verifique su identidad.
            </p>
        </fieldset>

        <fieldset class="frm-pane" runat="server" visible="true">
            <div class="row form-group">
                <div class="col-md-12">
                    Por favor no cierre su navegador web sin antes haber completado lo siguiente, lo cual debe tomarle de 5 a 10 minutos:
                    <ul>
                        <li>Solicitud de información</li>
                        <li>Referencias Personales</li>
                        <li>Reconocimiento de Políticas</li>
                    </ul>
                </div>
            </div>

            <legend>Información General</legend>

            <div style="display: none">
                <input type="text" id="ApplicantPersonAliasGuid" name="ApplicantPersonAliasGuid" />
            </div>


            <div class="row form-group" style="display: none;">
                <div class="col-md-6">
                    <label for="Today">Fecha de hoy</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="Today" name="Today" class="form-control" value="<%=DateTime.Now %>" disabled />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="LegalFirstName">Primer Nombre</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="LegalFirstName" name="LegalFirstName" class="form-control" placeholder="Primer Nombre" required data-validation-message="Primer Nombre" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="MiddleName">Segundo Nombre</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="MiddleName" name="LegalName" class="form-control" placeholder="Segundo Nombre" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="LastName">Apellido(s)</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="LastName" name="LastName" class="form-control" placeholder="Apellido(s)" required data-validation-message="Apellido(s)" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Suffix">Sufijo</label>
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
                    Por favor verifique su nombre completo, este será requerido como su firma al final de esta solicitud.
                </div>
            </div>
            <div class="row form-group notation-simple">
                <div class="col-md-6">
                    <label for="FullLegalName">Nombre Completo</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="FullLegalName" name="FullLegalName" class="form-control" placeholder="Nombre Completo" disabled />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Nickname">Apodo</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="Nickname" name="Nickname" class="form-control" placeholder="Apodo" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="PreviousName">Si aplica: escriba sus Nombres Previos (incluyendo apellido de soltera(o))</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousName" name="PreviousName" class="form-control" placeholder="Nombres Previos" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="CurrentAddressStreet">Domicilio</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="CurrentAddressStreet" name="CurrentAddressStreet" class="form-control" placeholder="Domicilio" required data-validation-message="Domicilio" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="CurrentAddressCity">Ciudad</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="CurrentAddressCity" name="CurrentAddressCity" class="form-control" placeholder="Ciudad" required data-validation-message="Ciudad" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="CurrentAddressState">Estado</label>
                    <span class="hint">Abbreviado en dos caracteres (por ejemplo IL)</span>
                </div>
                <div class="col-md-6">
                    <input type="text" id="CurrentAddressState" name="CurrentAddressState" class="form-control" placeholder="Estado" required data-validation-message="Estado" maxlength="2" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="CurrentAddressZip">Código Postal</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="CurrentAddressZip" name="CurrentAddressZip" class="form-control" placeholder="Código Postal" required data-validation-message="Código Postal" maxlength="5" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="">¿Ha vivido en su domicilio actual por mas de 12 meses?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="livedCurrentMoreThan12_Yes" name="livedCurrentMoreThan12" value="1" required data-validation-message="¿Ha vivido en su domicilio actual por mas de 12 meses?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="livedCurrentMoreThan12_No" name="livedCurrentMoreThan12" value="0" required data-validation-message="¿Ha vivido en su domicilio actual por mas de 12 meses?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="livedCurrentMoreThan12" data-dependent-value="0">
                <div class="col-md-6">
                    <label for="PreviousStreetAddress">Domicilio Previo</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousStreetAddress" class="form-control" name="PreviousStreetAddress" placeholder="Dirección" required data-validation-message="Domicilio Previo" />
                </div>
            </div>
            <div class="row form-group" data-dependent-on="livedCurrentMoreThan12" data-dependent-value="0">
                <div class="col-md-6">
                    <label for="PreviousAddressCity">Ciudad</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousAddressCity" name="PreviousAddressCity" class="form-control" placeholder="Ciudad" required data-validation-message="Ciudad de Domicilio Previo" />
                </div>
            </div>
            <div class="row form-group" data-dependent-on="livedCurrentMoreThan12" data-dependent-value="0">
                <div class="col-md-6">
                    <label for="PreviousAddressState">Estado</label>
                    <span class="hint">Abbreviado (por ejemplo IL)</span>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousAddressState" name="PreviousAddressState" class="form-control" placeholder="Estado" required data-validation-message="Estado de Domicilio Previo" maxlength="2" />
                </div>
            </div>
            <div class="row form-group" data-dependent-on="livedCurrentMoreThan12" data-dependent-value="0">
                <div class="col-md-6">
                    <label for="PreviousAddressZip">Código Postal</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="PreviousAddressZip" name="PreviousAddressZip" class="form-control" placeholder="Código Postal" required data-validation-message="Código Postal de Domicilio Previo" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="MobilePhone">Teléfono Celular</label>
                </div>
                <div class="col-md-6">
                    <input type="tel" id="MobilePhone" name="MobilePhone" class="form-control" data-role="phone" placeholder="(123) 456-7890" data-required-if="!#HomePhone" data-validation-message="Teléfono celular" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="HomePhone">Teléfono de Casa</label>
                </div>
                <div class="col-md-6">
                    <input type="tel" id="HomePhone" name="HomePhone" class="form-control" data-role="phone" placeholder="(123) 456-7890" required data-validation-message="Teléfono de casa" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="EmailAddress">Correo Electrónico</label>
                </div>
                <div class="col-md-6">
                    <input type="email" id="EmailAddress" name="EmailAddress" class="form-control" placeholder="email@domain.com" required data-validation-message="Correo Electrónico" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Gender_Male">Género</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="Gender_Male" name="Gender" value="1" required data-validation-message="Género" />
                        Masculino</label>
                    <label class="radiolabel">
                        <input type="radio" id="Gender_Female" name="Gender" value="2" required data-validation-message="Género" />
                        Femenino</label>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="DateOfBirth">Fecha de Nacimiento</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="DateOfBirth" name="DateOfBirth" class="form-control" placeholder="mm/dd/yyyy" data-role="date" required data-validation-message="Fecha de Nacimiento" />
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Marital_Status">Estado Civil</label>
                </div>
                <div class="col-md-6">
                    <select id="Marital_Status" name="Marital" class="form-control" required data-validation-message="Estado Civil">
                        <option value="144">Soltero(a)</option>
                        <option value="143">Casado(a)</option>
                        <option value="895">Separado(a)</option>
                        <option value="896">Divorciado(a)</option>
                        <option value="897">Viudo(a)</option>
                    </select>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="Children_Yes">¿Tiene hijos?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="Children_Yes" name="Children" value="true" required data-validation-message="¿Tiene hijos?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="Children_No" name="Children" value="false" required data-validation-message="¿Tiene hijos?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="Children" data-dependent-value="true">
                <div class="col-md-6"></div>
                <div class="col-md-3">
                    <label for="ChildrenCount">¿Cuantos hijos tiene?</label>
                </div>
                <div class="col-md-3">
                    <input type="text" id="ChildrenCount" name="ChildrenCount" class="form-control" placeholder="0" required data-validation-message="¿Cuantos hijos tiene?" />
                </div>
            </div>
            <div class="row form-group" data-dependent-on="Children" data-dependent-value="true">
                <div class="col-md-6"></div>
                <div class="col-md-3">
                    <label for="Ages">Edad/Edades</label>
                </div>
                <div class="col-md-3">
                    <input type="text" id="Ages" name="Ages" class="form-control" placeholder="3,5,9" required data-validation-message="Se require la edad/edades de sus hijos" />
                </div>
            </div>
            <div id="guardianDiv" class="row form-group">
                <div class="col-md-6">
                    <label for="Guardian_No">¿Tiene un tutor legal?</label>
                    <a class="help" href="#" tabindex="-1"><i class="fa fa-question-circle"></i></a>
                    <div class="alert alert-info help-message" style="display: none;">
                        <small>Una persona que tiene la autoridad legal para cuidar los intereses personales y de propiedad de otra persona, conocido como tutela.
                        </small>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="Guardian_Yes" name="Guardian" value="true" required data-validation-message="¿Tiene un tutor legal?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="Guardian_No" name="Guardian" value="false" required data-validation-message="¿Tiene un tutor legal?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label for="WCCC_Date">¿Usted asiste a Willow Creek Community Church?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="WCCC_Yes" name="WCCC" value="true" required data-validation-message="¿Usted asiste a Willow Creek Community Church?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="WCCC_No" name="WCCC" value="false" required data-validation-message="¿Usted asiste a Willow Creek Community Church?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="WCCC" data-dependent-value="true">
                <div class="col-md-6">
                    <label for="WCCC_Date">¿Cuándo comenzó a asistir a Willow Creek Community Church?</label>
                </div>
                <div class="col-md-6">
                    <input type="text" id="WCCC_Date" name="WCCC_Date" class="form-control" placeholder="mm/dd/yyyy" data-role="date" required data-validation-message="¿Cuándo comenzó a asistir a Willow Creek Community Church?" />
                </div>
            </div>

        </fieldset>

        <fieldset class="frm-pane" runat="server" visible="true">
            <legend>Preguntas Personales</legend>

            <div class="row form-group">
                <div class="col-md-12">
                    <label>
                        Por favor indique si usted tiene algún historial reciente de adicción a lo siguiente:<br />
                        (Seleccione todos los que correspondan)</label><br />
                    <span class="note">("Reciente" = en los ultimos 12 meses)</span>
                </div>
            </div>
            <div class="row form-group indented-block">
                <div class="col-md-12">
                    <div class="row form-group">
                        <div class="col-md-6">
                            <label>Pornografía:</label>
                        </div>
                        <div class="col-md-6">
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Porno" id="Porno_Past" value="1" required data-validation-message="Pornografía" />
                                Pasado</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Porno" id="Porno_Recent" value="2" required data-validation-message="Pornografía" />
                                Reciente</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Porno" id="Porno_None" value="0" required data-validation-message="Pornografía" />
                                Ninguno</label>
                        </div>
                    </div>
                    <div class="row form-group">
                        <div class="col-md-6">
                            <label>Alcohol:</label>
                        </div>
                        <div class="col-md-6">
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Alcohol" id="Alcohol_Past" value="1" required data-validation-message="Alcohol" />
                                Pasado</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Alcohol" id="Alcohol_Recent" value="2" required data-validation-message="Alcohol" />
                                Reciente</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Alcohol" id="Alcohol_None" value="0" required data-validation-message="Alcohol" />
                                Ninguno</label>
                        </div>
                    </div>
                    <div class="row form-group">
                        <div class="col-md-6">
                            <label>Drogas (ilegales o de receta):</label>
                        </div>
                        <div class="col-md-6">
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Drugs" id="Drugs_Past" value="1" required data-validation-message="Drogas" />
                                Pasado</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Drugs" id="Drugs_Recent" value="2" required data-validation-message="Drogas" />
                                Reciente</label>
                            <label class="radiolabel">
                                <input type="radio" class="recent" data-dependent-trigger=".recent" name="Drugs" id="Drugs_None" value="0" required data-validation-message="Drogas" />
                                Ninguno</label>
                        </div>
                    </div>
                    <div class="row form-group" data-dependent-on=".recent" data-dependent-value="2">
                        <div class="col-md-6">
                            <label>Por Favor Explique</label>
                        </div>
                        <div class="col-md-6">
                            <textarea rows="4" id="Addiction_Explain" class="form-control" required data-validation-message="Se requiere una explicación acerca de su historial con alguna de las adicciones mencionadas" data-minlength="75" maxlength="2000"></textarea>
                            <span class="counter">75 de 2000 caracteres permitidos</span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <div style="font-weight: bold">
                        ¿Alguna vez ha contemplado pornografía presentando a menores?
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="lookedAtPorn_Yes" name="lookedAtPorn" value="true" required data-validation-message="¿Alguna vez ha contemplado pornografía presentando a menores?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="lookedAtPorn_No" name="lookedAtPorn" value="false" required data-validation-message="¿Alguna vez ha contemplado pornografía presentando a menores?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="lookedAtPorn" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="lookedAtPorn_Explain" class="form-control" required data-validation-message="Se requiere una explicación acerca del porque ha contemplado pornografía presentando a menores" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <div style="font-weight: bold">
                        ¿Alguna vez ha sido investigado por el <a class="help" href="#" style="color: inherit; border-bottom: 1px dotted #86b8cc" tabindex="-1">DCFS</a> por evidencia de abuso o negligencia?                
                <div class="alert alert-info help-message" style="display: none; font-weight: normal;">
                    <small>Department of Child and Family Service (Departamento de Servicios para Niños y Familias) 
                    </small>
                </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="DCFS_Yes" name="DCFS" value="true" required data-validation-message="¿Alguna vez ha sido investigado por el DCFS por abuso o negligencia?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="DCFS_No" name="DCFS" value="false" required data-validation-message="¿Alguna vez ha sido investigado por el DCFS por abuso o negligencia?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="DCFS" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="DCFS_Explain" class="form-control" required data-validation-message="Se require una explicación acerca del porque ha sido sujeto de investigación por parte del DCFS" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Alguna vez ha tenido una orden de protección contra usted?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="OOP_Yes" name="OOP" value="true" required data-validation-message="¿Alguna vez ha tenido una orden de protección contra usted?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="OOP_No" name="OOP" value="false" required data-validation-message="¿Alguna vez ha tenido una orden de protección contra usted?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="OOP" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="OOP_Explain" class="form-control" required data-validation-message="Se requiere una exlicación acerca del porque ha tenido una orden de protección contra usted" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres disponibles</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Alguna vez ha cometido o ha sido acusado de cualquier acto que implicó el daño físico, sexual o emocional de otra persona? (Ejemplos incluyen agresión, violación, negligencia, etc.)</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="ComittedOrAccused_Yes" name="ComittedOrAccused" value="true" required data-validation-message="¿Alguna vez ha cometido o ha sido acusado de cualquier acto que implicó el daño físico, sexual o emocional de otra persona? (Ejemplos incluyen agresión, violación, negligencia, etc.)" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="ComittedOrAccused_No" name="ComittedOrAccused" value="false" required data-validation-message="¿Alguna vez ha cometido o ha sido acusado de cualquier acto que implicó el daño físico, sexual o emocional de otra persona? (Ejemplos incluyen agresión, violación, negligencia, etc.)" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="ComittedOrAccused" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="ComittedOrAccused_Explain" class="form-control" required data-validation-message="Se requiere una explicación acerca el porque ha cometido o ha sido acusado de algún acto que implicó el daño físico, sexual o emocional de otra persona" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <div style="font-weight: bold">
                        ¿Ha tenido alguna clase de relación con un menor o con un <a class="help" href="#" style="color: inherit; border-bottom: 1px dotted #86b8cc" tabindex="-1">adulto vulnerable</a>  que le haya traído gratificación sexual a usted?                    
                <div class="alert alert-info help-message" style="display: none; font-weight: normal">
                    <small>Una persona de 18 añoso o mayor que carezca de la capacidad física y/o mental para satisfacer sus necesidades diarias, que no pueda protegerse contra daño o explotación significativa y que esta en riesgo en la comunidad; esto incluye pero no se limita a alguien a quien se le haya asignado un tutor legal.
                    </small>
                </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="RelWithVuln_Yes" name="RelWithVuln" value="true" required data-validation-message="¿Ha tenido alguna clase de relación con un menor o con un adulto vulnerable que le haya traído gratificación sexual a usted?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="RelWithVuln_No" name="RelWithVuln" value="false" required data-validation-message="¿Ha tenido alguna clase de relación con un menor o con un adulto vulnerable que le haya traído gratificación sexual a usted?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="RelWithVuln" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="RelWithVuln_Explain" class="form-control" required data-validation-message="Se requiere una explicación acerca del porque ha tenido alguna clase de relación con un menor o con un adulto vulnerable que le haya traído gratificación sexual a usted" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Ha dejado o se le ha pedido dejar un rol dentro de una organización dado a una preocupación en cuanto a conducta inapropiada?</label>
                </div>
                <div class="col-md-6">
                    <label class="radiolabel">
                        <input type="radio" id="Misconduct_Yes" name="Misconduct" value="true" required data-validation-message="¿Ha dejado o se le ha pedido dejar un rol dentro de una organización dado a una preocupación en cuanto a conducta inapropiada?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="Misconduct_No" name="Misconduct" value="false" required data-validation-message="¿Ha dejado o se le ha pedido dejar un rol dentro de una organización dado a una preocupación en cuanto a conducta inapropiada?" />
                        No</label>
                </div>
            </div>
            <div class="row form-group" data-dependent-on="Misconduct" data-dependent-value="true">
                <div class="col-md-6">
                    <label>Por Favor Explique</label>
                </div>
                <div class="col-md-6">
                    <textarea rows="4" id="Misconduct_Explain" class="form-control" required data-validation-message="Se requiere una explicación acerca del porqué ha dejado o se le ha pedido dejar un rol dentro de una organización dado una preocupación en cuanto a conducta inapropiada" data-minlength="75" maxlength="2000"></textarea>
                    <span class="counter">75 de 2000 caracteres permitidos</span>
                </div>
            </div>

        </fieldset>

        <fieldset class="frm-pane" runat="server" visible="true">
            <legend>Liberación de Información</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    Gracias por tomarse el tiempo de llenar esta solicitud y por ayudarnos a crear un ambiente seguro aquí en Willow Creek. Por favor lea y afirme estar de acuerdo con las siguientes declaraciones:
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    <input id="chkCorrectInfo" name="chkCorrectInfo" type="checkbox" />
                    La información incluida en esta solicitud es acertada a mi leal saber.
                </div>
                <div class="col-md-12">
                    <input id="chkAuthReleaseInfo" name="chkAuthReleaseInfo" type="checkbox" />
                    Autorizo la revelación de información incluida en esta solicitud, de una forma confidencial, cuando exista la necesidad de saberla, a cualquier ministerio de Willow Creek Community Church en el cual busco un puesto (de voluntario o por remuneración).
                </div>
            </div>

            <h4>Autorización De Revisión De Antecedentes</h4>
            <div class="row form-group note">
                <div class="col-md-12" style="border: solid 1px #666; margin: 0 0 0 15px; padding: 5px; font: normal 9pt verdana; font-style: italic; overflow: auto; height: 85px;">
                    <p>
                        Al teclear mi nombre a continuación, autorizo a Willow Creek Community Church y a sus agentes autorizados, el obtener/preparar informes de consumidor o informe investigativo de consumidor sobre mí. Reconozco el haber recibido una copia de Un Resumen de sus derechos bajo el Reporte del Acta de Crédito justo (abajo) y verifico que lo he leído.
                    </p>
                    <p style="text-align: center;">
                        Resumen de sus derechos bajo el Reporte del Acta de Crédito Justo               
                    </p>
                    <p>
                        Con respecto a mi solicitud de protección con Willow Creek Community Church, yo entiendo que un “informe de consumidor” y/o un “informe investigativo de consumidor,” como es definido por el Reporte del Acta de Crédito Justo, será solicitado por Willow Creek con propósito de voluntariado o empleo, donde aplique, de Protect My Ministry, Inc., (“Protect My Ministry”), 
                        una agencia de informes de consumidor como es definida por el Reporte del Acta de Crédito Justo. Estos informes pueden incluir información referente a mi carácter, reputación general, amigos o asociados. El reporte puede también contener información sobre mi en referencia a mis antecedentes penales, historial crediticio, manejo y/o registros de vehículos, 
                        verificación del número de seguro social, verificación de historial de educación o empleo, remuneración de trabajador (solo tras una oferta de trabajo condicional) o cualquier otra revisión de antecedentes. Tales informes podrán ser obtenidos en cualquier momento al haber recibido esta Declaración y Autorización y si soy contratado(a) o sirvo como voluntario(a), 
                        cualquiera sea el caso, durante mi empleo o servicio voluntario, como es permitido por la ley y si no es revocado por mí por escrito. Yo entiendo que tengo el derecho, con una solicitud escrita en un tiempo razonable después de haber recibido este aviso, de solicitar la divulgación de la naturaleza y el alcance de cualquier informe investigativo de consumidor a Protect My Ministry, Inc., 
                        14499 N. Dale Mabry Hwy., Suite 201 South, Tampa, FL 33618 o 1-800-319-5581.
                    </p>
                </div>
                <div class="col-md-12" style="font: italic 9pt verdana; padding: 15px;">
                    Para más información sobre las prácticas privadas de Protect My Ministry, vea <a href="http://www.protectmyministry.com" target="_blank">www.protectmyministry.com</a>.
                </div>
            </div>
            
            <div class="row form-group">
                <div class="col-md-6">
                    <label>¿Cuenta usted con un Número de Seguro Social válido?</label>
                </div>
                <div class="col-md-6">
                      <label class="radiolabel">
                        <input type="radio" id="validSSN_Yes" name="validSSN" value="true" required data-validation-message="¿Cuenta usted con un Número de Seguro Social válido?" />
                        Si</label>
                    <label class="radiolabel">
                        <input type="radio" id="validSSN_No" name="validSSN" value="false" required data-validation-message="¿Cuenta usted con un Número de Seguro Social válido?" />
                        No</label>
                </div>
            </div>
            <div id="validSSNDiv">
                <div class="row form-group" data-dependent-on="validSSN" data-dependent-value="true">
                    <div class="col-md-6">
                        <label>Número de Seguro Social del Solicitante:</label>
                    </div>
                    <div class="col-md-6">
                        <input type="text" id="ApplicantSsn" class="form-control" placeholder="999-99-9999" required data-validation-message="Número de Seguro Social del Solicitante" />
                    </div>
                </div>
                <div class="row form-group" data-dependent-on="validSSN" data-dependent-value="true">
                    <div class="col-md-6">
                        <label>Verifique el Número de Seguro Social del Solicitante:</label>
                    </div>
                    <div class="col-md-6">
                        <input type="text" id="ApplicantSsnVerify" class="form-control" placeholder="999-99-9999" required data-validation-message="Número de Seguro Social del Solicitante" data-validation-match="ApplicantSsn" />
                    </div>
                </div>
            </div>            
            <div class="row form-group">
                <div class="col-md-4">
                    <label>Firma del Solicitante</label>
                </div>
                <div class="col-md-8">
                    <input type="text" id="ApplicantSignature" class="form-control" placeholder="Por favor teclea tu nombre legal completo" required data-validation-message="Firma del Solicitante" />

                    <input type="text" id="ApplicantSignatureDate" class="form-control" value="<%=DateTime.Now.ToString("M/d/yyyy") %>" disabled style="width: 100px" />
                </div>
            </div>
            <div class="row form-group" Id ="guardianSignatureDiv">
                <div class="col-md-4">
                    <label>Firma del Padre o Tutor*</label>
                </div>
                <div class="col-md-8">
                    <input type="text" id="GuardianSignature" class="form-control" required data-validation-message="Firma del Padre o Tutor" />

                    <input type="text" id="GuardianSignatureDate" class="form-control" value="<%=DateTime.Now.ToString("M/d/yyyy") %>" disabled style="width: 100px" />
                </div>
            </div>
            <div class="row form-group note" data-dependent-on="Guardian" data-dependent-value="true">
                <div class="col-md-12">*Sólo si el solicitante es un menor</div>
            </div>
        </fieldset>

        <%--        <app:References runat="server" />--%>

        <fieldset class="frm-pane">
            <legend>Gracias por llenar esta solicitud</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    <p>Gracias por tomarse el tiempo de llenar esta solicitud. Tu ministerio se pondrá en contacto contigo para darte a conocer los siguientes pasos.</p>
                    <p>Si desea explicar a mayor detalle cualquier información que haya proporcionado o si tiene alguna pregunta, no dude en comunicarse directamente con el Departamento de Protección.</p>
                    <p><a href="mailto:protection@willowcreek.org">protection@willowcreek.org</a></p>
                    <p>224-512-1920</p>
                </div>
            </div>
        </fieldset>
        <div id="buttonContainer">
            <div id="willow_pa_applicant_buttons">
                <button id="btnYesIm" class="btn btn-primary">
                    Soy
                    <asp:Literal ID="ltlApplicantName" runat="server"></asp:Literal>
                </button>
                <button id="btnNoIm" class="btn btn-link">
                    No soy
                <asp:Literal ID="ltlApplicantName2" runat="server"></asp:Literal></button>
                <button id="btnApplicantBack" class="btn" style="display: none;">Atrás</button>
                <a type="button" id="btnApplicantContinue" style="display: none;" class="btn btn-primary" data-loading-text="<i class='fa fa-refresh fa-spin'></i> Por Favor Espere">Continuar</a>
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
