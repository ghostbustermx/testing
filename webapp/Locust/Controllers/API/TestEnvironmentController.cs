using Locus.Core.Models;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Locus.Core.Repositories;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Locust.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Locus.Core.DTO;

namespace Locust.Controllers.API
{

    public class TestEnvironmentController : ApiController
    {
        private readonly ITestEnvironmentService _testEnvironmentService;

        public TestEnvironmentController(ITestEnvironmentService testEnvironmentService)
        {
            _testEnvironmentService = testEnvironmentService;
        }

        [System.Web.Http.ActionName("GetInactives")]
        [System.Web.Http.HttpGet]
        public List<TestEnvironment> GetInactives(int id)
        {
            return _testEnvironmentService.GetInactives(id);
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public TestEnvironment Get(int id)
        {
            return _testEnvironmentService.Get(id);
        }

        [System.Web.Http.ActionName("GetActives")]
        [System.Web.Http.HttpGet]
        public List<TestEnvironment> GetActives(int id)
        {
            return _testEnvironmentService.GetActives(id);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public TestEnvironment Save(TestEnvironment te)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            te.Creator = user;
            te.Last_Editor = user;
            return _testEnvironmentService.Save(te);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public TestEnvironment Update(TestEnvironment te)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            te.Last_Editor = user;
            return _testEnvironmentService.Update(te);
        }

        [System.Web.Http.ActionName("GetTETypes")]
        [System.Web.Http.HttpGet]
        public string[] GetTETypes()
        {
            string[] values = SplitterHelper.splitStringFromKey("TypesEnv");
            return values;
        }

        [System.Web.Http.ActionName("GetOS")]
        [System.Web.Http.HttpGet]
        public string[] GetOS()
        {
            string[] values = SplitterHelper.splitStringFromKey("OS");
            return values;
        }

        [System.Web.Http.ActionName("HasRelationships")]
        [System.Web.Http.HttpGet]
        public List<ExecutionGroup> HasRelationships(int id)
        {
            return _testEnvironmentService.HasRelationships(id);
        }
    }
}