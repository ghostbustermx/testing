using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Locust.Helpers
{
    public class CustomMultipartFormDataStreamProvider : MultipartFileStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        {
        }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            return headers.ContentDisposition.FileName.Replace("\"", string.Empty);

        }

        public override Stream GetStream(HttpContent parent, System.Net.Http.Headers.HttpContentHeaders headers)
        {
            bool isValid = FileNameHelper.ExtensionIsValid(headers.ContentDisposition.FileName,Types.Restore);
          
            if (isValid)
            {
                return base.GetStream(parent, headers);
            }
            else{
                throw new Exception("The restore file extension is not valid");
            }
        }
    }
}