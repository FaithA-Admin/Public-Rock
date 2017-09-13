using System;
using System.Threading;
using System.IO;
using Rock;
using Rock.Model;
using Rock.Data;

namespace org.willowcreek
{
    public static class Files
    {
        public static BinaryFile ConvertWordToPDF(BinaryFile binaryFile, string fileName, int binaryFileTypeId, bool isTemporary = true)
        {
            using ( var rockContext = new RockContext() )
            {
                // Get the BinaryFile in the current context
                var binaryFileService = new BinaryFileService( rockContext );
                binaryFile = binaryFileService.Get( binaryFile.Id );

                // Save the file to disk so it can be opened in Word
                var filePath = SaveBinaryFileToDisk( binaryFile, $"{fileName} {Guid.NewGuid()}", ".docx" );

                var newFilePath = filePath.Replace( ".docx", ".pdf" );
                using ( var wordDoc = new WordDocument( filePath ) )
                {
                    wordDoc.SaveAsPDF( newFilePath );
                }

                binaryFile.BinaryFileTypeId = binaryFileTypeId;
                binaryFile.ContentStream = new FileStream( newFilePath, FileMode.Open );
                binaryFile.FileName = $"{fileName}.pdf";
                binaryFile.MimeType = "application/pdf";
                binaryFile.IsTemporary = isTemporary;
                rockContext.SaveChanges();

                DeleteFileWhenReady( newFilePath );
                DeleteFileWhenReady( filePath );

                return binaryFile;
            }
        }
        
        // Save the files directly to the disk
        public static string SaveBinaryFileToDisk( BinaryFile binaryFile, string fileName, string fileNameExtension )
        {
            var filePath = $"{GetFilePath()}{fileName}{fileNameExtension}";
            var fileContents = binaryFile.ContentStream;
            using ( var fileStream = File.Create( filePath ) )
            {
                fileContents.Seek( 0, SeekOrigin.Begin );
                fileContents.CopyTo( fileStream );
            }
            return filePath;
        }

        public static string GetFilePath()
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory.EnsureTrailingBackslash()}App_Data\\Files\\";
        }

        public static void DeleteFileWhenReady( string sFilename )
        {
            new Thread( () =>
            {
                Thread.CurrentThread.IsBackground = true;

                var ready = false;
                while ( ready == false )
                {
                    // If the file can be opened for exclusive access it means that the file is no longer locked by another process.
                    try
                    {
                        using ( FileStream inputStream = File.Open( sFilename, FileMode.Open, FileAccess.Read, FileShare.None ) )
                        {
                            ready = inputStream.Length > 0;
                        }
                    }
                    catch ( Exception )
                    {
                        ready = false;
                    }
                }
                File.Delete( sFilename );
            } ).Start();
        }
    }
}
