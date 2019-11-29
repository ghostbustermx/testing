using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Helpers
{
    public static class ZipFileCreatorHelper
    {

        public static bool CreateZipFile(string PathFolter, string FileName, Scripts[] files)
        {
            try
            {
                // Create and open a new ZIP file
                string dir = PathFolter;
                if (!Directory.Exists(dir))
                {
                    DirectoryInfo directory = Directory.CreateDirectory(dir);
                }
                string SavingPath = PathFolter + "\\" + FileName + DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss") + ".zip";
                var zip = ZipFile.Open(SavingPath, ZipArchiveMode.Create);
                foreach (var file in files)
                {
                    // Add the entry for each file
                    zip.CreateEntryFromFile(file.Path, Path.GetFileName(file.Path), CompressionLevel.Optimal);
                }
                // Dispose of the object when we are done
                zip.Dispose();
                return true;
            }
            catch(Exception r)
            {
                return false;
            }
            
        }
    }
}
