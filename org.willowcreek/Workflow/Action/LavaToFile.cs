using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.MergeTemplates;
using Rock.Model;
using Rock.Workflow;

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory( "Utility" )]
    [Description( "Creates a Merge Document using the current Workflow's attributes" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Lava to File" )]
    [CustomDropdownListField( "File Format", "The kind of file to create.", "text/html^HTML,text/csv^Comma Separated Values", order: 0 )]
    [TextField( "File Name", order: 1 )]
    [BinaryFileTypeField( "File Type", order: 2 )]
    [CodeEditorField( "Lava Template", order: 3 )]
    [WorkflowAttribute( "Result Attribute", "The attribute that will be populated with the merge document.", order: 4, fieldTypeClassNames: new string[] { "Rock.Field.Types.FileFieldType" } )]
    public class LavaToFile : ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            var code = GetAttributeValue(action, "LavaTemplate" );
            code = code.ResolveMergeFields( GetMergeFields( action ) );
            var binaryFileTypeService = new BinaryFileTypeService( rockContext );
            var binaryFileType = binaryFileTypeService.Get( GetAttributeValue(action, "FileType").AsGuid() );
            var fileName = GetAttributeValue(action, "FileName" );

            BinaryFile binaryFile;
            var mimeType = GetAttributeValue( action, "FileFormat" );
            using ( Stream s = GenerateStreamFromString( code ) )
            {
                binaryFile = new BinaryFile()
                {
                    Guid = Guid.NewGuid(),
                    IsTemporary = false,
                    BinaryFileTypeId = binaryFileType.Id,
                    MimeType = mimeType,
                    FileName = fileName,
                    ContentStream = s
                };
                var binaryFileService = new BinaryFileService( rockContext );
                binaryFileService.Add( binaryFile );
                rockContext.SaveChanges();
            }


            var attributeService = new AttributeService( rockContext );
            var resultAttribute = attributeService.Get( GetAttributeValue( action, "ResultAttribute" ).AsGuid() );

            action.Activity.Workflow.SetAttributeValue( resultAttribute.Key, binaryFile.Guid.ToString() );

            return true;
        }

        public static Stream GenerateStreamFromString( string s )
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter( stream );
            writer.Write( s );
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
