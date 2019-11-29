using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class TestScenarioController : ApiController
    {
        private readonly ITestScenarioService _testScenarioService;

        public TestScenarioController(ITestScenarioService testScenarioService)
        {
            _testScenarioService = testScenarioService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<TestScenario> GetAll()
        {
            return _testScenarioService.GetAll();
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public TestScenario Get(int id)
        {
            return _testScenarioService.Get(id);
        }

        [System.Web.Http.ActionName("TestScenarioChangeLogs")]
        [System.Web.Http.HttpGet]
        public List<ChangeLog> TestScenarioChangeLogs(int id)
        {
            return _testScenarioService.TestScenarioChangeLogs(id);
        }

        [System.Web.Http.ActionName("Restore")]
        [System.Web.Http.HttpPost]
        public ChangeLog Restore(ChangeLog change_log)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testScenarioService.Restore(change_log, user);
        }

        [System.Web.Http.ActionName("AddChangeLog")]
        [System.Web.Http.HttpGet]
        public ChangeLog AddChangeLog(int id)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testScenarioService.AddChangeLog(id, user);
        }

        [System.Web.Http.ActionName("GetLastOne")]
        [System.Web.Http.HttpGet]
        public TestScenario GetLastOne(int idReq, string creator, string date)
        {
            return _testScenarioService.GetLastOne(idReq, creator, date);
        }

        [System.Web.Http.ActionName("GetLastTestScenario")]
        [System.Web.Http.HttpGet]
        public TestScenario GetLastTestScenario(string creator, string date)
        {
            return _testScenarioService.GetLastTestScenario(creator, date);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public TestScenario Save(TestScenario testScenario)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testScenarioService.Save(testScenario, user);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public TestScenario Update(TestScenario testScenario)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testScenarioService.Update(testScenario, user);
        }

        [System.Web.Http.ActionName("GetProject")]
        [System.Web.Http.HttpGet]
        public Project GetProject(int idts)
        {
            return _testScenarioService.GetProject(idts);
        }

        [System.Web.Http.ActionName("Enable")]
        [System.Web.Http.HttpGet]
        public TestScenario Enable(int id)
        {
            return _testScenarioService.Enable(id);
        }


        [System.Web.Http.ActionName("GetProjectRequirement")]
        [System.Web.Http.HttpGet]
        public Project GetProjectRequirement(int idreq)
        {
            return _testScenarioService.GetProjectRequirement(idreq);
        }

        [System.Web.Http.ActionName("GetRequirement")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirement(int reqId)
        {
            return _testScenarioService.GetRequirement(reqId);
        }


        [System.Web.Http.ActionName("GetRequirementForTs")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirementForTs(int tsId)
        {
            return _testScenarioService.GetRequirementForTs(tsId);
        }
        
        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public TestScenario Delete(int id)
        {
            return _testScenarioService.Delete(id);
        }

        [System.Web.Http.ActionName("UpdateNumber")]
        [System.Web.Http.HttpPut]
        public TestScenario UpdateNumber(TestScenario testScenario, int idReq)
        {
            return _testScenarioService.UpdateNumber(testScenario, idReq);
        }
    }

}
