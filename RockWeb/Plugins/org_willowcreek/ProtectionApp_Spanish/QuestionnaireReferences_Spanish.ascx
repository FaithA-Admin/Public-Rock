<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuestionnaireReferences_Spanish.ascx.cs" Inherits="Plugins_org_willowcreek_ProtectionApp_QuestionnaireReferences_es" %>
<link href="/Styles/WillowCreek/ProtectionApp.css" type="text/css" rel="stylesheet" />
<script src="/scripts/WillowRockReferenceList_Spanish.js" type="text/javascript"></script>
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
    <div id="willow_pa_applicant_form" class="frm-pane-wrapper" style="width: 90%;">
        <fieldset id="willow_pa_reference_form" class="frm-pane">
            <legend>Referencias</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    <span class="note">Por favor proporcione tres (3) referencias que:</span><br />
                    <span class="note" style="text-indent: each-line">- sean mayores de 18 años</span><br />
                    <span class="note" style="text-indent: each-line">- no sean parientes de usted (incluyendo cónyuge, prometido(a) y padrastro/madrastra/hermanastros)</span><br />
                    <span class="note" style="text-indent: each-line">- lo conozcan por más de un año</span><br />
                    <span class="note" style="text-indent: each-line">- tengan conociminento definitivo de su carácter</span><br />
                    <span class="note" style="text-indent: each-line"><b>Por favor dele seguimiento a sus tres referencias para asegurar que hayan recibido el correo solicitando su petición de referencia.</b></span>
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-12">
                    <div id="referenceList">
                        <div class="row">
                            <div class="col-md-3">
                                <label>Nombre</label>
                            </div>
                            <div class="col-md-2">
                                <label>Relación con la persona</label>
                            </div>
                            <div class="col-md-4">
                                <label>Contacto</label>
                            </div>
                            <div class="col-md-2">
                                <label>Estado</label>
                            </div>
                            <div class="col-md-1"></div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="refPane" style="width: 60%">
                <div class="row form-group">
                    <div class="col-md-6">
                        <label>Nombre:</label>
                    </div>
                    <div class="col-md-6">
                        <input type="text" id="RefName" name="RefName" class="form-control" required data-validation-message="Nombre de la Referencia" />
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-6">
                        <label>Relación con la persona:</label>
                    </div>
                    <div class="col-md-6">
                        <select id="RefAssociation" name="RefAssociation" class="form-control" required data-validation-message="Relación con la Persona">
                            <option value="">Selecciona...</option>
                            <option value="Friend">Amigo</option>
                            <option value="Colleague">Colega de Trabajo</option>
                            <option value="Supervisor">Previo o Actual Supervisor</option>
                            <option value="mentor">Previo o Actual Mentor</option>
                            <option value="Family">Familiar</option>
                        </select>
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-6">
                        <label>Correo electrónico (requerido):</label>
                    </div>
                    <div class="col-md-6">
                        <input type="email" id="RefEmail" name="RefEmail" class="form-control" data-role="email" placeholder="correoelectrónico@dominio.com" required data-validation-message="Correo electrónico" />
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-12">
                        Si no conoce el correo electrónico de su referencia, por favor contactela para obtener esta información antes de continuar llenando la solicitud. Si su refrencia no tiene correo electrónico, por favor utilize otra refrencia. Utilizar un correo electrónico incorrecto causará un retraso en este proceso.
                    </div>
                </div>
                <div class="row form-group">
                    <div class="col-md-12" style="text-align: right;">
                        <span style="display: none;"><span class="refCount">0</span> de 3 ingresadas</span>
                        <button id="btnAddRef" class="btn btn-primary">Aggregar</button>
                    </div>
                </div>
            </div>
        </fieldset>

        <fieldset class="frm-pane" style="display:none; width: 60%">
            <legend>Divulgación de información</legend>
            <div class="row form-group">
                <div class="col-md-12">
                    <input id="chkAuthReleaseRef" name="chkAuthReleaseRef" type="checkbox" />
                    Autorizo cualquier referencia en la página anterior para darle cualquier información (incluyendo opiniones) que puedan tener sobre mi carácter y aptitud para trabajar con niños, estudiantes o adultos vulnerables.
                </div>
            </div>
            <div class="row form-group">
                <div class="col-md-4">
                    <label>Firma del Solicitante</label>
                </div>
                <div class="col-md-8">
                    <input type="text" id="ApplicantSignature" class="form-control" required data-validation-message="Se requiere la firma del solicitante" placeholder="Por favor ingrese su nombre legal completo" />
                    <input type="text" id="FullLegalName" name="FullLegalName" class="form-control" value="<%=FullLegalName %>" placeholder="Nombre legal completo" disabled style="display:none" />

                    <input type="text" id="ApplicantSignatureDate" class="form-control" value="<%=DateTime.Now.ToString("M/d/yyyy") %>" disabled style="width: 100px" />
                </div>
            </div>
        </fieldset>
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

        <div class="row form-group ResendText" runat="server" id="ResendText">
            <div class="col-md-12">
                Si las referencias anteriores son correctas por favor haga click en "Continuar"para reenviar los correos electrónicos solicitando las referncias.
            </div>
        </div>
        <div id="willow_pa_reference_buttons">
            <button id="btnReferenceBack" class="btn" style="display: none;">Atrás</button>
            <%--<button id="btnReferenceContinue" class="btn btn-primary" data-loading-text="<i class='fa fa-refresh fa-spin'></i> Please Wait">Continue</button>--%>
            <a type="button" id="btnReferenceContinue" class="btn btn-primary" data-loading-text="<i class='fa fa-refresh fa-spin'></i> Por Favor Espere">Continuar</a>
            
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