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
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using Rock.Attribute;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Security;
using org.willowcreek.FileManagement.Data;
using Rock.Web.Cache;
using Rock.Field.Types;
using System.Web.UI.WebControls;

namespace RockWeb.Plugins.org_willowcreek.FileManagement
{
    [DisplayName( "Person Document List" )]
    [Category("org_willowcreek > File Management")]
    [Description( "Shows a list of all binary files for the Person in context." )]
    [BinaryFileTypeField]
    public partial class PersonDocumentList : PersonBlock
    {
        private BinaryFileType _binaryFileType = null;

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit(e);

            //GenerateCancelScript();

            Guid binaryFileTypeGuid = Guid.NewGuid();
            if ( Guid.TryParse( GetAttributeValue( "BinaryFileType" ), out binaryFileTypeGuid ) )
            {
                var service = new BinaryFileTypeService( new RockContext() );
                _binaryFileType = service.Get( binaryFileTypeGuid );
            }

            BindFilter();
            fBinaryFile.ApplyFilterClick += fBinaryFile_ApplyFilterClick;

            // This not only hooks up the event handler, but tells the control to display the Save button...
            modalAddDocument.SaveClick += modalAddDocument_SaveClick;
            //modalAddDocument.OnCancelScript = "CancelNewDocModal();";

            // Hook up the Image Upload event...
            fileUploader.FileUploaded += fileUploader_FileUploaded;


            gBinaryFile.DataKeyNames = new string[] { "id" };
            gBinaryFile.Actions.ShowAdd = true;
            gBinaryFile.Actions.AddClick += gBinaryFile_Add;
            gBinaryFile.GridRebind += gBinaryFile_GridRebind;
            gBinaryFile.RowItemText = _binaryFileType != null ? _binaryFileType.Name : "Binary File";
            gBinaryFile.RowDataBound += gBinaryFile_RowDataBound;

            // Block Security and special attributes (RockPage takes care of View)
            bool canAddEditDelete = IsUserAuthorized( Authorization.EDIT );
            gBinaryFile.Actions.ShowAdd = canAddEditDelete;
            gBinaryFile.IsDeleteEnabled = canAddEditDelete;
        }

        /// <summary>
        /// This method has working script that can be used as a function call from the Cancel link.
        /// I can confirm that in conjunction with the OnCancelScript set in the OnInit method, this 
        /// function is reachable.  But for the reason mentioned below in the WebMethod, the javascript 
        /// call to cancelUpload never gets back to the server.
        /// </summary>
        protected void GenerateCancelScript()
        {
            string script = @"
                function CancelNewDocModal() {
                    alert('hi!');
                    PageMethods.cancelUpload(CallSuccess, CallFailed);
                    return false;
                }

                function CallSucess(res) {
                    alert('succeeded!');
                }

                function CallFailed(res) {
                    alert('call failed!');
                }
                ";

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "modaldialog-onCancel-" + this.ClientID, script, true);

        }

        /// <summary>
        /// If we could turn on EnablePageMethods on the page's ScriptManager, we could call this method from javascript 
        /// on this control.
        /// </summary>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static string cancelUpload()
        {
            string ret = "who cares";
            
            return ret;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                BindGrid();
            }

            base.OnLoad( e );
        }

        void modalAddDocument_SaveClick(object sender, EventArgs e)
        {
            if (fileUploader.BinaryFileId.HasValue)
            {
                // Get the uploaded file and some of the data items...
                var tempFileId = Convert.ToInt32(hfUploadedFileId.Value);
                var fileName = tbNewDocName.Text;
                var fileDescription = tbNewDocDescription.Text;

                // Now let's mark the file as permanent...
                var fileService = new BinaryFileService(new RockContext());
                var file = fileService.Get(tempFileId);
                file.Description = fileDescription;
                file.FileName = fileName;
                file.IsTemporary = false;
                file.BinaryFileTypeId = _binaryFileType.Id;
                fileService.Context.SaveChanges();

                // Now let's create the new link record...
                var personDocumentService = new PersonDocumentService(new FileManagementContext());
                personDocumentService.Add(new org.willowcreek.FileManagement.Model.PersonDocument
                {
                    PersonAliasGuid = Person.PrimaryAlias.Guid,
                    Guid = Guid.NewGuid(),
                    BinaryFileId = file.Guid,
                });
                personDocumentService.Context.SaveChanges();

                // Close the dialog...should rebind the grid too, while we are at it...
                modalAddDocument.Hide();
                BindGrid();
            }
        }

        #endregion

        #region Grid Events (main grid)
        void gBinaryFile_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.DataItem != null)
            {
                if (e.Row.DataItem is BinaryFile)
                {
                    // Need the right variable...
                    var binaryFile = e.Row.DataItem as BinaryFile;

                    // Now I want the cell that contains the Literal control...
                    var literalControl = e.Row.FindControl("viewPlaceholder") as Literal;

                    if (literalControl != null)
                    {
                        // Need a View Button (this code borrowed from the Rock...BinaryFileField.FormatValue method)
                        var filePath = System.Web.VirtualPathUtility.ToAbsolute("~/GetFile.ashx");
                        var html = string.Format("<a href='{0}?guid={1}' target='_blank' title={2} class='btn btn-sm btn-default'>View</a>", filePath, binaryFile.Guid, binaryFile.FileName);
                        literalControl.Text = html;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the ApplyFilterClick event of the fBinaryFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void fBinaryFile_ApplyFilterClick( object sender, EventArgs e )
        {
            fBinaryFile.SaveUserPreference( "File Name", tbName.Text );
            fBinaryFile.SaveUserPreference( "Mime Type", tbType.Text );

            BindGrid();
        }

        /// <summary>
        /// Handles the Add event of the gBinaryFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gBinaryFile_Add( object sender, EventArgs e )
        {
            tbNewDocName.Text = String.Empty;
            tbNewDocDescription.Text = String.Empty;
            fileUploader.BinaryFileId = null;

            modalAddDocument.Show();
        }

        /// <summary>
        /// Handles the Delete event of the gBinaryFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gBinaryFile_Delete( object sender, RowEventArgs e )
        {
            var rockContext = new RockContext();
            BinaryFileService binaryFileService = new BinaryFileService( rockContext );
            BinaryFile binaryFile = binaryFileService.Get( e.RowKeyId );

            if ( binaryFile != null )
            {
                string errorMessage;
                if ( !binaryFileService.CanDelete( binaryFile, out errorMessage ) )
                {
                    mdGridWarning.Show( errorMessage, ModalAlertType.Information );
                    return;
                }

                // First, delete the person document record, then the file...
                PersonDocumentService personDocumentService = new PersonDocumentService(new FileManagementContext());
                if (personDocumentService.Queryable().Any(d => d.PersonAliasGuid == Person.PrimaryAlias.Guid && d.BinaryFileId == binaryFile.Guid))
                {
                    var personDocument = personDocumentService.Queryable().Single(d => d.PersonAliasGuid == Person.PrimaryAlias.Guid && d.BinaryFileId == binaryFile.Guid);
                    personDocumentService.Delete(personDocument);
                    personDocumentService.Context.SaveChanges();

                    // Now delete the file...
                    binaryFileService.Delete(binaryFile);
                    rockContext.SaveChanges();
                }
            }

            BindGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the gBinaryFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void gBinaryFile_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        #endregion

        #region Uploader Events
        /// <summary>
        /// Handles the FileUploaded event of the fsFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void fileUploader_FileUploaded(object sender, EventArgs e)
        {
            // All we are going to do is store the ID of the temporary file that we are uploading...
            if (fileUploader.BinaryFileId.HasValue)
            {
                var fileService = new BinaryFileService(new RockContext());
                var file = fileService.Get(fileUploader.BinaryFileId.Value);

                hfUploadedFileId.Value = fileUploader.BinaryFileId.Value.ToString();
                tbNewDocName.Text = file.FileName;
            }

            modalAddDocument.Show();
        }
        #endregion

        #region Internal Methods

        /// <summary>
        /// Binds the filter.
        /// </summary>
        private void BindFilter()
        {
            if ( !Page.IsPostBack )
            {
                tbName.Text = fBinaryFile.GetUserPreference( "File Name" );
                tbType.Text = fBinaryFile.GetUserPreference( "Mime Type" );
            }            
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void BindGrid()
        {
            // Get the person in context, and get the list of doc IDs for that person...
            Guid binaryFileTypeGuid = _binaryFileType != null ? _binaryFileType.Guid : Guid.NewGuid();

            var thePersonAliasGuids = this.Person.Aliases.Select(a => a.Guid).ToList();
            var personDocService = new PersonDocumentService(new FileManagementContext());
            var dox = personDocService.Queryable().Where(d => thePersonAliasGuids.Contains(d.PersonAliasGuid));
            var docIds = dox.Select(d => d.BinaryFileId).ToList();

            // First get all the docs based on the list for the person...
            var binaryFileService = new BinaryFileService(new RockContext());
            var queryable = binaryFileService.Queryable().Where(f => docIds.Contains(f.Guid) && f.BinaryFileType.Guid == binaryFileTypeGuid);

            if (_binaryFileType != null)
            {
                queryable = queryable.Where(f => f.BinaryFileType.Guid == _binaryFileType.Guid);
            }

            var sortProperty = gBinaryFile.SortProperty;
            string name = fBinaryFile.GetUserPreference( "File Name" );
            if ( !string.IsNullOrWhiteSpace( name ) )
            {
                queryable = queryable.Where( f => f.FileName.Contains( name ) );
            }

            string type = fBinaryFile.GetUserPreference( "Mime Type" );
            if ( !string.IsNullOrWhiteSpace( type ) )
            {
                queryable = queryable.Where( f => f.MimeType.Contains( type ) );
            }

            if ( sortProperty != null )
            {
                gBinaryFile.DataSource = queryable.Sort( sortProperty ).ToList();
            }
            else
            {
                gBinaryFile.DataSource = queryable.OrderBy( d => d.FileName ).ToList();
            }

            gBinaryFile.DataBind();
        }

        #endregion
    }
}