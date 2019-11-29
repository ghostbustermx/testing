using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Locust.Helpers
{
    public class DownloadFile : IHttpActionResult
    {

        MemoryStream fileStuff;
        string filePath;
        HttpRequestMessage httpRequestMessage;
        HttpResponseMessage httpResponseMessage;


        public DownloadFile (HttpRequestMessage request, string file)
        {
            var dataBytes = File.ReadAllBytes(file);
            var dataStream = new MemoryStream(dataBytes);
            fileStuff = dataStream;
            httpRequestMessage = request;
            filePath = file;
        }

        public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(fileStuff);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = filePath;
            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            return System.Threading.Tasks.Task.FromResult(httpResponseMessage);
        }
    }
}