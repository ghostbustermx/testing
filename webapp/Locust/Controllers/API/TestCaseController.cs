using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class TestCaseController : ApiController
    {
        private readonly ITestCaseService _testCaseService;

        public TestCaseController(ITestCaseService testCaseService)
        {
            _testCaseService = testCaseService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<TestCase> GetAll()
        {
            return _testCaseService.GetAll();
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public TestCase Get(int id)
        {
            return _testCaseService.Get(id);
        }
        
        [System.Web.Http.ActionName("GetLastOne")]
        [System.Web.Http.HttpGet]
        public TestCase GetLastOne(int idReq, string creator, string date)
        {
            return _testCaseService.GetLastOne(idReq, creator, date);
        }

        [System.Web.Http.ActionName("GetLastTestCase")]
        [System.Web.Http.HttpGet]
        public TestCase GetLastTestCase(string creator, string date)
        {
            return _testCaseService.GetLastTestCase(creator, date);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public TestCase Save(TestCase testCase)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testCaseService.Save(testCase, user);
        }

        [System.Web.Http.ActionName("Restore")]
        [System.Web.Http.HttpPost]
        public ChangeLog Restore(ChangeLog change_log)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testCaseService.Restore(change_log, user);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public TestCase Update(TestCase testCase)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testCaseService.Update(testCase, user);
        }

        [System.Web.Http.ActionName("TestCaseChangeLogs")]
        [System.Web.Http.HttpGet]
        public List<ChangeLog> TestCaseChangeLogs(int id)
        {
            return _testCaseService.TestCaseChangeLogs(id);
        }

        [System.Web.Http.ActionName("GetProject")]
        [System.Web.Http.HttpGet]
        public Project GetProject(int idtc)
        {
            return _testCaseService.GetProject(idtc);
        }

        [System.Web.Http.ActionName("GetProjectRequirement")]
        [System.Web.Http.HttpGet]
        public Project GetProjectRequirement(int idreq)
        {
            return _testCaseService.GetProjectRequirement(idreq);
        }

        [System.Web.Http.ActionName("GetRequirement")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirement(int reqId)
        {
            return _testCaseService.GetRequirement(reqId);
        }

        [System.Web.Http.ActionName("GetRequirementForTc")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirementForTc(int tc)
        {
            return _testCaseService.GetRequirementForTc(tc);
        }

        [System.Web.Http.ActionName("AddChangeLog")]
        [System.Web.Http.HttpGet]
        public ChangeLog AddChangeLog(int id)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testCaseService.AddChangeLog(id, user);
        }

        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public TestCase Delete(int id)
        {
            return _testCaseService.Delete(id);
        }

        [System.Web.Http.ActionName("Enable")]
        [System.Web.Http.HttpGet]
        public TestCase Enable(int id)
        {
            return _testCaseService.Enable(id);
        }

        [System.Web.Http.ActionName("UpdateNumber")]
        [System.Web.Http.HttpPut]
        public TestCase UpdateNumber(TestCase testCase, int idReq)
        {
            return _testCaseService.UpdateNumber(testCase, idReq);
        }
    }
}
