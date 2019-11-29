using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using Locust.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class ScriptsController : ApiController
    {
        private readonly IScriptsService _scriptsService;


        public ScriptsController(IScriptsService scriptsService)
        {
            _scriptsService = scriptsService;
        }

        [System.Web.Http.ActionName("GetAllScripts")]
        [System.Web.Http.HttpGet]
        public List<Scripts> GetAllScripts(int ScriptGroupID)
        {
            return _scriptsService.GetAllScripts(ScriptGroupID);
        }       
        
        [System.Web.Http.ActionName("CompressAllScripts")]
        [System.Web.Http.HttpGet]
        public bool CompressAllScripts(int projectId, string Name, Scripts[] scriptsRelated)
        {
            return _scriptsService.CompressAllScripts(projectId,  Name,scriptsRelated);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public Scripts Save(int groupId, int projectId)
        {
            return _scriptsService.Save(groupId, projectId);
        }

        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public Scripts Delete(int scriptId)
        {
            return _scriptsService.Delete(scriptId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("DownLoad")]
        public HttpResponseMessage DownLoad(int scriptId, string name)
        {

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.Content = new ByteArrayContent(_scriptsService.Download(scriptId));
            httpResponseMessage.Content.Headers.Add("x-filename", name);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = name;
            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            return httpResponseMessage;

        }

    }
}
