
using System.IO;
using System.Net.Http;

namespace Locust.Helpers
{

    public enum Types
    {
        Backup,
        Restore,
        Image
    }

    public static class FileNameHelper
    {
        public static bool ExtensionIsValid(string path, Types type)
        {
            string[] extension=new string[] { };
            switch (type)
            {
                case Types.Backup:
                    extension = new string[] { ".bak" };

                    break;
                case Types.Restore:
                    extension = new string[] { ".bak"};

                    break;
                case Types.Image:
                    extension = new string[] { ".png", ".jpeg" };
                    break;
            }

            string extensionExpected=Path.GetExtension(path.Replace("\"", string.Empty)).ToLower();
            
            foreach(string ext in extension){
                if (ext.Equals(extensionExpected))
                {
                    return true;
                }
            }
            return false;
        }

        public static void DeleteMultipartFileData(CustomMultipartFormDataStreamProvider provider)
        {
            foreach (MultipartFileData file in provider.FileData)
            {
                if (File.Exists(file.LocalFileName))
                {
                    File.Delete(file.LocalFileName);
                }
            }
        }
    }
}