// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock;
using Rock.Security;

namespace org.willowcreek.Security.BackgroundCheck
{
    /// <summary>
    /// Protect My Ministry Background Check 
    /// Note: This component requires 
    /// </summary>
    [Description("Protect My Ministry Background Check (Willow)")]
    [Export(typeof(BackgroundCheckComponent))]
    [ExportMetadata("ComponentName", "Protect My Ministry")]
    public class ProtectMyMinistry : BackgroundCheckComponent
    {
        private HttpStatusCode _HTTPStatusCode;
        private RockContext rockContext;
        private string _username ="";
        private string _encryptedPassword = "";
        private bool _testmode = true;
        private string _returnURL ="";
        private string _requestURL = "";
        private string _batchRequestURL = "";
        public ProtectMyMinistry()
        {
            rockContext = new RockContext();
            var pmmEntityType = EntityTypeCache.Read(typeof(Rock.Security.BackgroundCheck.ProtectMyMinistry));
            if(pmmEntityType != null)
            {
                var pmmAttributes = new AttributeService(rockContext).Queryable().Where(a=> a.EntityTypeId == pmmEntityType.Id); 
                var attributeValues = new AttributeValueService(rockContext).Queryable();
                var pmmAttributeValues = from av in attributeValues
                                    join pmma in pmmAttributes on av.AttributeId equals pmma.Id
                                    select new
                                    {
                                        Key = pmma.Key,
                                        Value = av.Value
                                    };               
                _username = pmmAttributeValues.Where(av => av.Key == "UserName").FirstOrDefault().Value;
                _encryptedPassword = pmmAttributeValues.Where(av => av.Key == "Password").FirstOrDefault().Value;
                _testmode = pmmAttributeValues.Where(av => av.Key == "TestMode").FirstOrDefault().Value.AsBoolean();                
                _returnURL = pmmAttributeValues.Where(av => av.Key == "ReturnURL").FirstOrDefault().Value;
                _requestURL = pmmAttributeValues.Where(av => av.Key == "RequestURL").FirstOrDefault().Value;
                _batchRequestURL = new AttributeService(rockContext).Queryable().Where(a => a.EntityTypeId.HasValue == false && a.Key == "RequestURLForBatchProcessing").Select(a => a.DefaultValue).FirstOrDefault();
            }             
        }

        /// <summary>
        /// Sends a background request to Protect My Ministry
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="workflow">The Workflow initiating the request.</param>
        /// <param name="personAttribute">The person attribute.</param>
        /// <param name="ssnAttribute">The SSN attribute.</param>
        /// <param name="requestTypeAttribute">The request type attribute.</param>
        /// <param name="billingCodeAttribute">The billing code attribute.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns>
        /// True/False value of whether the request was successfully sent or not
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// Note: If the associated workflow type does not have attributes with the following keys, they
        /// will automatically be added to the workflow type configuration in order to store the results
        /// of the PMM background check request
        ///     RequestStatus:          The request status returned by PMM request
        ///     RequestMessage:         Any error messages returned by PMM request
        ///     ReportStatus:           The report status returned by PMM
        ///     ReportLink:             The location of the background report on PMM server
        ///     ReportRecommendation:   PMM's recomendataion
        ///     Report (BinaryFile):    The downloaded background report
        /// </remarks>
        public override bool SendRequest(RockContext rockContext, Rock.Model.Workflow workflow,
            AttributeCache personAttribute, AttributeCache ssnAttribute, AttributeCache requestTypeAttribute,
            AttributeCache billingCodeAttribute, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                // Check to make sure workflow is not null
                if (workflow == null)
                {
                    errorMessages.Add("The 'Protect My Ministry' background check provider requires a valid workflow.");
                    return false;
                }

                // Get the person that the request is for
                Person person = null;
                if (personAttribute != null)
                {
                    Guid? personAliasGuid = workflow.GetAttributeValue(personAttribute.Key).AsGuidOrNull();
                    if (personAliasGuid.HasValue)
                    {
                        person = new PersonAliasService(rockContext).Queryable()
                            .Where(p => p.Guid.Equals(personAliasGuid.Value))
                            .Select(p => p.Person)
                            .FirstOrDefault();
                        person.LoadAttributes(rockContext);
                    }
                }

                if (person == null)
                {
                    errorMessages.Add("The 'Protect My Ministry' background check provider requires the workflow to have a 'Person' attribute that contains the person who the background check is for.");
                    return false;
                }

                string password = Encryption.DecryptString(_encryptedPassword);

                XElement rootElement = new XElement("OrderXML",
                    new XElement("Method", "SEND ORDER"),
                    new XElement("Authentication",
                        new XElement("Username", _username),
                        new XElement("Password", password)
                    )
                );

                if (_testmode)
                {
                    rootElement.Add(new XElement("TestMode", "YES"));
                }


                //Orders element to hold all orders
                XElement ordersElement = new XElement("Orders");
                rootElement.Add(ordersElement);

                XElement orderElement = new XElement("Order");
                ordersElement.Add(orderElement);

                orderElement.Add(new XElement("ReturnResultURL", _returnURL));

                XElement subjectElement = new XElement("Subject",
                    new XElement("FirstName", person.FirstName),
                    new XElement("LastName", person.LastName)
                );
                orderElement.Add(subjectElement);

                orderElement.Add(new XElement("ReferenceNumber",_returnURL));

                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), rootElement);
                var requestDateTime = RockDateTime.Now;

                XDocument xResult = PostToWebService(xdoc, _requestURL);
                var responseDateTime = RockDateTime.Now;

                int? personAliasId = person.PrimaryAliasId;
                if (personAliasId.HasValue)
                {
                    // Create a background check file
                    using (var newRockContext = new RockContext())
                    {
                        var backgroundCheckService = new BackgroundCheckService(newRockContext);
                        var backgroundCheck = backgroundCheckService.Queryable()
                            .Where(c =>
                               c.WorkflowId.HasValue &&
                               c.WorkflowId.Value == workflow.Id)
                            .FirstOrDefault();

                        if (backgroundCheck == null)
                        {
                            backgroundCheck = new Rock.Model.BackgroundCheck();
                            backgroundCheck.PersonAliasId = personAliasId.Value;
                            backgroundCheck.WorkflowId = workflow.Id;
                            backgroundCheckService.Add(backgroundCheck);
                        }

                        backgroundCheck.RequestDate = RockDateTime.Now;

                        // Clear any SSN nodes before saving XML to record
                        foreach (var xSSNElement in xdoc.Descendants("SSN"))
                        {
                            xSSNElement.Value = "XXX-XX-XXXX";
                        }
                        foreach (var xSSNElement in xResult.Descendants("SSN"))
                        {
                            xSSNElement.Value = "XXX-XX-XXXX";
                        }

                        backgroundCheck.ResponseXml = string.Format(@"
Request XML ({0}): 
------------------------ 
{1}

Response XML ({2}): 
------------------------ 
{3}

", requestDateTime, xdoc.ToString(), responseDateTime, xResult.ToString());
                        newRockContext.SaveChanges();
                    }
                }

                if (_HTTPStatusCode == HttpStatusCode.OK)
                {
                    using (var newRockContext = new RockContext())
                    {
                        bool createdNewAttribute = false;
                        var xOrderXML = xResult.Elements("OrderXML").FirstOrDefault();
                        if (xOrderXML != null)
                        {
                            var xStatus = xOrderXML.Elements("Status").FirstOrDefault();
                            if (xStatus != null)
                            {
                                if (SaveAttributeValue(workflow, "RequestStatus", xStatus.Value,
                                    FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), newRockContext, null))
                                {
                                    createdNewAttribute = true;
                                }
                            }

                            var xErrors = xOrderXML.Elements("Errors").FirstOrDefault();
                            if (xErrors != null)
                            {
                                string errorMsg = xErrors.Elements("Message").Select(x => x.Value).ToList().AsDelimited(Environment.NewLine);
                                if (SaveAttributeValue(workflow, "RequestMessage", errorMsg,
                                    FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), newRockContext, null))
                                {
                                    createdNewAttribute = true;
                                }
                            }

                            if (xResult.Root.Descendants().Count() > 0)
                            {
                                SaveResults(xResult, workflow, rockContext, false);
                            }
                        }
                        else
                        {
                            if (SaveAttributeValue(workflow, "RequestMessage", errorMessages.AsDelimited(Environment.NewLine),
                                FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), newRockContext, null))
                            {
                                createdNewAttribute = true;
                            }

                        }

                        newRockContext.SaveChanges();

                        if (createdNewAttribute)
                        {
                            AttributeCache.FlushEntityAttributes();
                        }
                    }
                    return true;
                }
                else
                {
                    errorMessages.Add("Invalid HttpStatusCode: " + _HTTPStatusCode.ToString());
                    return false;
                }
            }

            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, null);
                errorMessages.Add(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// RerunRequest
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="workflow"></param>
        /// <param name="personAttribute"></param>
        /// <param name="oldReportIdAttribute"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        public bool RerunRequest(RockContext rockContext, Rock.Model.Workflow workflow,
    AttributeCache personAttribute, AttributeCache oldReportIdAttribute, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                // Check to make sure workflow is not null
                if (workflow == null)
                {
                    errorMessages.Add("The 'Protect My Ministry' background check provider requires a valid workflow.");
                    return false;
                }

                // Get the person that the request is for
                Person person = null;
                if (personAttribute != null)
                {
                    Guid? personAliasGuid = workflow.GetAttributeValue(personAttribute.Key).AsGuidOrNull();
                    if (personAliasGuid.HasValue)
                    {
                        person = new PersonAliasService(rockContext).Queryable()
                            .Where(p => p.Guid.Equals(personAliasGuid.Value))
                            .Select(p => p.Person)
                            .FirstOrDefault();
                        person.LoadAttributes(rockContext);
                    }
                }

                if (person == null)
                {
                    errorMessages.Add("The 'Protect My Ministry' background check provider requires the workflow to have a 'Person' attribute that contains the person who the background check is for.");
                    return false;
                }

                string password = Encryption.DecryptString(_encryptedPassword);

                XElement rootElement = new XElement("OrderXML",
                    new XElement("Method", "SEND ORDER"),
                    new XElement("Authentication",
                        new XElement("Username", _username),
                        new XElement("Password", password)
                    )
                );

                if (_testmode)
                {
                    rootElement.Add(new XElement("TestMode", "YES"));
                }

                //Orders element to hold all orders
                XElement ordersElement = new XElement("Orders");
                rootElement.Add(ordersElement);

                XElement orderElement = new XElement("Order");
                ordersElement.Add(orderElement);

                orderElement.Add(new XElement("ReturnResultURL", _returnURL));
                orderElement.Add(new XElement("OrderId", workflow.Id));

                XElement subjectElement = new XElement("Subject",
                    new XElement("FirstName", person.FirstName),
                    new XElement("LastName", person.LastName)
                );
                orderElement.Add(subjectElement);

                orderElement.Add(new XElement("ReferenceNumber", workflow.GetAttributeValue(oldReportIdAttribute.Key)));

                XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), rootElement);
                var requestDateTime = RockDateTime.Now;

                XDocument xResult = PostToWebService(xdoc, _batchRequestURL);
                var responseDateTime = RockDateTime.Now;

                int? personAliasId = person.PrimaryAliasId;
                if (personAliasId.HasValue)
                {
                    // Create a background check file
                    using (var newRockContext = new RockContext())
                    {
                        var backgroundCheckService = new BackgroundCheckService(newRockContext);
                        var backgroundCheck = backgroundCheckService.Queryable()
                            .Where(c =>
                               c.WorkflowId.HasValue &&
                               c.WorkflowId.Value == workflow.Id)
                            .FirstOrDefault();

                        if (backgroundCheck == null)
                        {
                            backgroundCheck = new Rock.Model.BackgroundCheck();
                            backgroundCheck.PersonAliasId = personAliasId.Value;
                            backgroundCheck.WorkflowId = workflow.Id;
                            backgroundCheckService.Add(backgroundCheck);
                        }

                        backgroundCheck.RequestDate = RockDateTime.Now;

                        // Clear any SSN nodes before saving XML to record
                        foreach (var xSSNElement in xdoc.Descendants("SSN"))
                        {
                            xSSNElement.Value = "XXX-XX-XXXX";
                        }
                        foreach (var xSSNElement in xResult.Descendants("SSN"))
                        {
                            xSSNElement.Value = "XXX-XX-XXXX";
                        }

                        backgroundCheck.ResponseXml = string.Format(@"
Request XML ({0}): 
------------------------ 
{1}

Response XML ({2}): 
------------------------ 
{3}

", requestDateTime, xdoc.ToString(), responseDateTime, xResult.ToString());
                        newRockContext.SaveChanges();
                    }
                }

                if (_HTTPStatusCode == HttpStatusCode.OK)
                {
                    using (var newRockContext = new RockContext())
                    {
                        bool createdNewAttribute = false;
                        var xOrderXML = xResult.Elements("OrderXML").Elements("Orders").Elements("Order").FirstOrDefault();
                        if (xOrderXML != null)
                        {
                            var xStatus = xOrderXML.Elements("Status").FirstOrDefault();
                            if (xStatus != null)
                            {
                                if (SaveAttributeValue(workflow, "RequestStatus", xStatus.Value,
                                    FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), newRockContext, null))
                                {
                                    createdNewAttribute = true;
                                }

                                if(xStatus.Value.ToLower().Contains("error"))
                                {
                                    string errorMsg = xOrderXML.Elements("Message").Select(x => x.Value).ToList().AsDelimited(Environment.NewLine);
                                    if (SaveAttributeValue(workflow, "RequestMessage", errorMsg,
                                        FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), newRockContext, null))
                                    {
                                        createdNewAttribute = true;
                                    }
                                }
                            }
                            if (xResult.Root.Descendants().Count() > 0)
                            {
                                SaveResults(xResult, workflow, rockContext, false);
                            }
                        }
                        else
                        {
                            if (SaveAttributeValue(workflow, "RequestMessage", errorMessages.AsDelimited(Environment.NewLine),
                                FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), newRockContext, null))
                            {
                                createdNewAttribute = true;
                            }

                        }

                        newRockContext.SaveChanges();

                        if (createdNewAttribute)
                        {
                            AttributeCache.FlushEntityAttributes();
                        }
                    }
                    return true;
                }
                else
                {
                    errorMessages.Add("Invalid HttpStatusCode: " + _HTTPStatusCode.ToString());
                    return false;
                }
            }

            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, null);
                errorMessages.Add(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Posts to web service.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private XDocument PostToWebService(XDocument data, string requestUrl)
        {
            string stringData = "REQUEST=" + data.Declaration.ToString() + data.ToString(SaveOptions.DisableFormatting);
            byte[] postData = ASCIIEncoding.ASCII.GetBytes(stringData);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.ContentLength = postData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return GetResponse(response.GetResponseStream(), response.ContentType, response.StatusCode);
            }
            catch (WebException webException)
            {
                string message = GetResponseMessage(webException.Response.GetResponseStream());
                throw new Exception(webException.Message + " - " + message);
            }
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="responseStream">The response stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns></returns>
        private XDocument GetResponse(Stream responseStream, string contentType, HttpStatusCode statusCode)
        {
            _HTTPStatusCode = statusCode;

            Stream receiveStream = responseStream;
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);

            StringBuilder sb = new StringBuilder();
            Char[] read = new Char[8192];
            int count = 0;
            do
            {
                count = readStream.Read(read, 0, 8192);
                String str = new String(read, 0, count);
                sb.Append(str);
            }
            while (count > 0);

            string HTMLResponse = sb.ToString();

            if (HTMLResponse.Trim().Length > 0 && HTMLResponse.Contains("<?xml"))
                return XDocument.Parse(HTMLResponse);
            else
                return null;
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <param name="responseStream">The response stream.</param>
        /// <returns></returns>
        private string GetResponseMessage(Stream responseStream)
        {
            Stream receiveStream = responseStream;
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);

            StringBuilder sb = new StringBuilder();
            Char[] read = new Char[8192];
            int count = 0;
            do
            {
                count = readStream.Read(read, 0, 8192);
                String str = new String(read, 0, count);
                sb.Append(str);
            }
            while (count > 0);

            return sb.ToString();
        }

        /// <summary>
        /// Saves the results.
        /// </summary>
        /// <param name="xResult">The x result.</param>
        /// <param name="workflow">The workflow.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="saveResponse">if set to <c>true</c> [save response].</param>
        public static void SaveResults(XDocument xResult, Rock.Model.Workflow workflow, RockContext rockContext, bool saveResponse = true)
        {
            bool createdNewAttribute = false;

            var newRockContext = new RockContext();
            var service = new BackgroundCheckService(newRockContext);
            var backgroundCheck = service.Queryable()
                .Where(c =>
                   c.WorkflowId.HasValue &&
                   c.WorkflowId.Value == workflow.Id)
                .FirstOrDefault();

            if (backgroundCheck != null && saveResponse)
            {
                // Clear any SSN nodes before saving XML to record
                foreach (var xSSNElement in xResult.Descendants("SSN"))
                {
                    xSSNElement.Value = "XXX-XX-XXXX";
                }

                backgroundCheck.ResponseXml = backgroundCheck.ResponseXml + string.Format(@"
Response XML ({0}): 
------------------------ 
{1}

", RockDateTime.Now.ToString(), xResult.ToString());
            }

            var xOrderXML = xResult.Elements("OrderXML").FirstOrDefault();
            if (xOrderXML != null)
            {
                var xOrder = xOrderXML.Elements("Order").FirstOrDefault();
                if (xOrder != null)
                {
                    bool resultFound = false;

                    // Find any order details with a status element
                    string reportStatus = "Pass";
                    foreach (var xOrderDetail in xOrder.Elements("OrderDetail"))
                    {
                        var xStatus = xOrderDetail.Elements("Status").FirstOrDefault();
                        if (xStatus != null)
                        {
                            resultFound = true;
                            if (xStatus.Value != "NO RECORD")
                            {
                                reportStatus = "Review";
                                break;
                            }
                        }
                    }

                    if (resultFound)
                    {
                        // If no records found, still double-check for any alerts
                        if (reportStatus != "Review")
                        {
                            var xAlerts = xOrder.Elements("Alerts").FirstOrDefault();
                            if (xAlerts != null)
                            {
                                if (xAlerts.Elements("OrderId").Any())
                                {
                                    reportStatus = "Review";
                                }
                            }
                        }

                        // Save the recommendation 
                        string recommendation = (from o in xOrder.Elements("Recommendation") select o.Value).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(recommendation))
                        {
                            if (SaveAttributeValue(workflow, "ReportRecommendation", recommendation,
                                FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), rockContext,
                                new Dictionary<string, string> { { "ispassword", "false" } }))
                            {
                                createdNewAttribute = true;
                            }
                        }

                        // Save the report link 
                        Guid? binaryFileGuid = null;
                        string reportLink = (from o in xOrder.Elements("ReportLink") select o.Value).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(reportLink))
                        {
                            if (SaveAttributeValue(workflow, "ReportLink", reportLink,
                                FieldTypeCache.Read(Rock.SystemGuid.FieldType.URL_LINK.AsGuid()), rockContext))
                            {
                                createdNewAttribute = true;
                            }

                            // Save the report
                            binaryFileGuid = SaveFile(workflow.Attributes["Report"], reportLink, workflow.Id.ToString() + ".pdf");
                            if (binaryFileGuid.HasValue)
                            {
                                if (SaveAttributeValue(workflow, "Report", binaryFileGuid.Value.ToString(),
                                    FieldTypeCache.Read(Rock.SystemGuid.FieldType.BINARY_FILE.AsGuid()), rockContext,
                                    new Dictionary<string, string> { { "binaryFileType", "" } }))
                                {
                                    createdNewAttribute = true;
                                }
                            }
                        }

                        // Save the status
                        if (SaveAttributeValue(workflow, "ReportStatus", reportStatus,
                            FieldTypeCache.Read(Rock.SystemGuid.FieldType.SINGLE_SELECT.AsGuid()), rockContext,
                            new Dictionary<string, string> { { "fieldtype", "ddl" }, { "values", "Pass,Fail,Review" } }))
                        {
                            createdNewAttribute = true;
                        }

                        // Save the report id 
                        string reportId = (from o in xOrder.Elements("ReportID") select o.Value).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(reportId))
                        {
                            if (SaveAttributeValue(workflow, "ReportID", reportId,
                                FieldTypeCache.Read(Rock.SystemGuid.FieldType.TEXT.AsGuid()), rockContext,
                                new Dictionary<string, string> { { "ispassword", "false" } }))
                            {
                                createdNewAttribute = true;
                            }
                        }

                        // Update the background check file
                        if (backgroundCheck != null)
                        {
                            backgroundCheck.ResponseDate = RockDateTime.Now;
                            backgroundCheck.RecordFound = reportStatus == "Review";

                            if (binaryFileGuid.HasValue)
                            {
                                var binaryFile = new BinaryFileService(newRockContext).Get(binaryFileGuid.Value);
                                if (binaryFile != null)
                                {
                                    backgroundCheck.ResponseDocumentId = binaryFile.Id;
                                }
                            }
                        }
                    }
                }
            }

            newRockContext.SaveChanges();

            if (createdNewAttribute)
            {
                AttributeCache.FlushEntityAttributes();
            }

        }

        /// <summary>
        /// Saves the attribute value.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="qualifiers">The qualifiers.</param>
        /// <returns></returns>
        private static bool SaveAttributeValue(Rock.Model.Workflow workflow, string key, string value,
            FieldTypeCache fieldType, RockContext rockContext, Dictionary<string, string> qualifiers = null)
        {
            bool createdNewAttribute = false;

            if (workflow.Attributes.ContainsKey(key))
            {
                workflow.SetAttributeValue(key, value);
            }
            else
            {
                // Read the attribute
                var attributeService = new AttributeService(rockContext);
                var attribute = attributeService
                    .Get(workflow.TypeId, "WorkflowTypeId", workflow.WorkflowTypeId.ToString())
                    .Where(a => a.Key == key)
                    .FirstOrDefault();

                // If workflow attribute doesn't exist, create it 
                // ( should only happen first time a background check is processed for given workflow type)
                if (attribute == null)
                {
                    attribute = new Rock.Model.Attribute();
                    attribute.EntityTypeId = workflow.TypeId;
                    attribute.EntityTypeQualifierColumn = "WorkflowTypeId";
                    attribute.EntityTypeQualifierValue = workflow.WorkflowTypeId.ToString();
                    attribute.Name = key.SplitCase();
                    attribute.Key = key;
                    attribute.FieldTypeId = fieldType.Id;
                    attributeService.Add(attribute);

                    if (qualifiers != null)
                    {
                        foreach (var keyVal in qualifiers)
                        {
                            var qualifier = new Rock.Model.AttributeQualifier();
                            qualifier.Key = keyVal.Key;
                            qualifier.Value = keyVal.Value;
                            attribute.AttributeQualifiers.Add(qualifier);
                        }
                    }

                    createdNewAttribute = true;
                }

                // Set the value for this action's instance to the current time
                var attributeValue = new Rock.Model.AttributeValue();
                attributeValue.Attribute = attribute;
                attributeValue.EntityId = workflow.Id;
                attributeValue.Value = value;
                new AttributeValueService(rockContext).Add(attributeValue);
            }

            return createdNewAttribute;
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="binaryFileAttribute">The binary file attribute.</param>
        /// <param name="url">The URL.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private static Guid? SaveFile(AttributeCache binaryFileAttribute, string url, string fileName)
        {
            // get BinaryFileType info
            if (binaryFileAttribute != null &&
                binaryFileAttribute.QualifierValues != null &&
                binaryFileAttribute.QualifierValues.ContainsKey("binaryFileType"))
            {
                Guid? fileTypeGuid = binaryFileAttribute.QualifierValues["binaryFileType"].Value.AsGuidOrNull();
                if (fileTypeGuid.HasValue)
                {
                    RockContext rockContext = new RockContext();
                    BinaryFileType binaryFileType = new BinaryFileTypeService(rockContext).Get(fileTypeGuid.Value);

                    if (binaryFileType != null)
                    {
                        byte[] data = null;

                        using (WebClient wc = new WebClient())
                        {
                            data = wc.DownloadData(url);
                        }

                        BinaryFile binaryFile = new BinaryFile();
                        binaryFile.Guid = Guid.NewGuid();
                        binaryFile.IsTemporary = true;
                        binaryFile.BinaryFileTypeId = binaryFileType.Id;
                        binaryFile.MimeType = "application/pdf";
                        binaryFile.FileName = fileName;
                        binaryFile.ContentStream = new MemoryStream(data);

                        var binaryFileService = new BinaryFileService(rockContext);
                        binaryFileService.Add(binaryFile);

                        rockContext.SaveChanges();

                        return binaryFile.Guid;
                    }
                }
            }

            return null;
        }
    }
}