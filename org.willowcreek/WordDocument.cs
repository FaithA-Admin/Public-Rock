using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace org.willowcreek
{
    public class WordDocument : IDisposable
    {
        public readonly Application WordApp = null;
        public readonly Document WordDoc = null;

        public WordDocument(string filePath)
        {
            WordApp = new Application();
            WordDoc = WordApp.Documents.Open( filePath, ReadOnly: false );

            WordApp.Options.SaveNormalPrompt = false;
            WordApp.Options.SavePropertiesPrompt = false;
        } 

        public void Save( string filePath = null )
        {
            if ( filePath == null )
            {
                WordDoc.Save();
            }
            else
            {
                WordDoc.SaveAs2( filePath );
            }
        }

        public void SaveAsPDF(string filePath)
        {
            WordDoc.ExportAsFixedFormat( filePath, WdExportFormat.wdExportFormatPDF );
        }

        public void Quit()
        {
            WordDoc.Close( false );
            WordApp.Quit();
        }

        public void Dispose()
        {
            Quit();
        }
    }
}
