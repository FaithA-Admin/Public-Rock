using System;
using System.Linq;
using Rock.Data;
using Rock.Model;
using org.willowcreek.ProtectionApp.Model;

namespace org.willowcreek.ProtectionApp
{
    /// <summary>
    /// Class Utility.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Generates the HTML document.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="replacementData">The replacement data.</param>
        /// <returns>System.String.</returns>
        public static string GenerateHtmlDocument(string path, object replacementData)
        {
            string html = string.Empty;
            try
            {
                html = System.IO.File.ReadAllText(path);
                System.Text.StringBuilder raw = new System.Text.StringBuilder(html);

                System.Text.RegularExpressions.Regex expression = new System.Text.RegularExpressions.Regex("{{[^}]+}}");
                Type entityType = replacementData.GetType();

               //if the replacement data was retrived we may be in a proxy class, lets make sure we're using the real entity
                if (entityType.BaseType != null && entityType.Namespace == "System.Data.Entity.DynamicProxies")
                    entityType = entityType.BaseType;

                System.Reflection.PropertyInfo[] pis = entityType.GetProperties();

                System.Text.RegularExpressions.MatchCollection matches = expression.Matches(html);
                while (matches.Count > 0)
                {
                    System.Text.RegularExpressions.Match match = matches[0];
                    string currMatch = match.Value;
                    string propName = currMatch.Replace("{", "").Replace(entityType.Name + ".", "").Replace("}", "");
                    System.Reflection.PropertyInfo pi = pis.Where(x => x.Name == propName).FirstOrDefault();
                    if (pi != null)
                    {
                        var upperName = propName.ToUpperInvariant();
                        var value = pi.GetValue(replacementData);
                        string val = string.Empty;
                        if (upperName == "DATEOFBIRTH")
                            val = ((DateTime)value).ToShortDateString();
                        else if (upperName == "APPLICANTSSN")
                            val = "***-**-****";
                        else if (value is LivedAtTimePeriod) if ((LivedAtTimePeriod)value == LivedAtTimePeriod.GreaterThan12Months)
                                val = "Yes";
                            else
                                val = "No";
                        else if (value != null)
                            val = value.ToString();

                        int index = match.Index;
                        int length = match.Length;

                        if (string.IsNullOrEmpty(val))
                            val = "-";//string.Format("No {0} Provided", propName);

                        if (val.ToLower() == "true")
                            val = "Yes";
                        if (val.ToLower() == "false")
                            val = "No";

                        raw.Remove(index, length);
                        if (!string.IsNullOrEmpty(val))
                            raw.Insert(index, val);
                    }
                    html = raw.ToString();
                    matches = expression.Matches(html);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
                throw;
            }
            return html;
        }

        /// <summary>
        /// Generates the html document.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="path">The path.</param>
        /// <param name="replacementData">The replacement data.</param>
        /// <param name="fileTypeGuid">The file type identifier.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileDescription">The file description.</param>
        /// <param name="attributeGuid">The attribute unique identifier.</param>
        /// <returns>System.String.</returns>
        public static string GenerateDocument(RockContext rockContext, PersonAlias alias, string path, object replacementData, Guid fileTypeGuid, string fileName,
            string fileDescription, Guid attributeGuid, System.IO.MemoryStream docStream)
        {
            string html = GenerateHtmlDocument(path, replacementData);

            //save the html to the person record
            if (rockContext != null && !string.IsNullOrEmpty(html))
                SaveFile(rockContext, fileDescription, fileName, fileTypeGuid, html, alias, attributeGuid, docStream);

            return html;
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="fileDescription">The file description.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileTypeGuid">The file type identifier.</param>
        /// <param name="html">The HTML.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="attributeGuid">The attribute unique identifier.</param>
        /// <param name="docStream">The MemoryStream used to hold the data</param>
        private static void SaveFile(RockContext rockContext, string fileDescription, string fileName, Guid fileTypeGuid, string html,
            PersonAlias alias, Guid attributeGuid, System.IO.MemoryStream docStream)
        {
            try
            {
                //save the html to the person record
                var documentService = new BinaryFileService(rockContext);
                var attributeService = new AttributeService(rockContext);
                var attributeValueService = new AttributeValueService(rockContext);
                var typeService = new BinaryFileTypeService(rockContext);

                var attribute = attributeService.Get(attributeGuid);
                var fileType = typeService.Get(fileTypeGuid);
                var htmlFile = new BinaryFile();
                htmlFile.BinaryFileType = fileType;
                htmlFile.BinaryFileTypeId = fileType.Id;
                htmlFile.Description = fileDescription;
                htmlFile.FileName = fileName;
                htmlFile.IsTemporary = false;

                //write to the stream
                using (docStream)
                {
                    using (var writer = new System.IO.BinaryWriter(docStream))
                    {
                        writer.Write(System.Text.Encoding.UTF8.GetBytes(html));
                        htmlFile.MimeType = "text/html";
                        //v3.1 adds the Content Stream directly to the Binary File, in v2.0 we need to use
                        // the BinaryFileData
                        //htmlFile.ContentStream = memory;
                        htmlFile.DatabaseData = new BinaryFileData();
                        htmlFile.ContentStream = docStream;
                        documentService.Add(htmlFile);
                        documentService.Context.SaveChanges();
                    }
                }
                //save the document to the person using the established attributes
                var attributeValue = new AttributeValue
                {
                    Attribute = attribute,
                    AttributeId = attribute.Id,
                    //TH - Was not saving actual person id, fixed
                    EntityId = alias.PersonId,
                    IsSystem = false,
                    Value = htmlFile.Guid.ToString()
                };
                //Check if attribute value exists
                //TODO Validate history is saving
                var existingAttributeValue = attributeValueService.GetByAttributeIdAndEntityId(attribute.Id, alias.PersonId);

                if (existingAttributeValue == null)
                    attributeValueService.Add(attributeValue);
                else
                    existingAttributeValue.Value = htmlFile.Guid.ToString();

                attributeValueService.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
                throw;
            }
        }

    }
}
