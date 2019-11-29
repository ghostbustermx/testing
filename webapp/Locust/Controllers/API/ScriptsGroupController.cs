using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using Locust.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class ScriptsGroupController : ApiController
    {
        private readonly IScriptsGroupService _scriptsGroupService;

        public ScriptsGroupController(IScriptsGroupService scriptsGroupService)
        {
            _scriptsGroupService = scriptsGroupService;
        }

        [System.Web.Http.ActionName("GetAllScriptsGroup")]
        [System.Web.Http.HttpGet]
        public List<ScriptsGroup> GetAllScriptsGroup(int projectId)
        {
            return _scriptsGroupService.GetAllScriptsGroup(projectId);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public ScriptsGroup Save(ScriptsGroup scriptsGroup)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _scriptsGroupService.Save(scriptsGroup, user);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public ScriptsGroup Update(ScriptsGroup scriptsGroup)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _scriptsGroupService.Update(scriptsGroup, user);
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public ScriptsGroup Get(int scriptGroupId)
        {
            return _scriptsGroupService.Get(scriptGroupId);
        }
    }
}
