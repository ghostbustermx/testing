using Locus.Core.DTO;
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
    public class TestProcedureController : ApiController
    {
        private readonly ITestProcedureService _testProcedureService;

        public TestProcedureController(ITestProcedureService testProcedureService)
        {
            _testProcedureService = testProcedureService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<TestProcedure> GetAll()
        {
            return _testProcedureService.GetAll();
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public TestProcedure Get(int id)
        {
            return _testProcedureService.Get(id);
        }

        [System.Web.Http.ActionName("GetLastOne")]
        [System.Web.Http.HttpGet]
        public TestProcedure GetLastOne(int idReq, string creator, string date)
        {
            return _testProcedureService.GetLastOne(idReq, creator, date);
        }

        [System.Web.Http.ActionName("GetLastTestProcedure")]
        [System.Web.Http.HttpGet]
        public TestProcedure GetLastTestProcedure(string creator, string date)
        {
            return _testProcedureService.GetLastTestProcedure(creator, date);
        }

        [System.Web.Http.ActionName("TestProcedureChangeLogs")]
        [System.Web.Http.HttpGet]
        public List<ChangeLog> TestScenarioChangeLogs(int id)
        {
            return _testProcedureService.TestProcedureChangeLogs(id);
        }

        [System.Web.Http.ActionName("Restore")]
        [System.Web.Http.HttpPost]
        public ChangeLog Restore(ChangeLog change_log)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testProcedureService.Restore(change_log, user);
        }

        [System.Web.Http.ActionName("Enable")]
        [System.Web.Http.HttpGet]
        public TestProcedure Enable(int id)
        {
            return _testProcedureService.Enable(id);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public TestProcedure Save(TestProcedure testProcedure)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testProcedureService.Save(testProcedure, user);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public TestProcedure Update(TestProcedure testProcedure)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testProcedureService.Update(testProcedure, user);
        }

        [System.Web.Http.ActionName("AddChangeLog")]
        [System.Web.Http.HttpGet]
        public ChangeLog AddChangeLog(int id)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testProcedureService.AddChangeLog(id, user);
        }

        [System.Web.Http.ActionName("GetProject")]
        [System.Web.Http.HttpGet]
        public Project GetProject(int idtp)
        {
            return _testProcedureService.GetProject(idtp);
        }

        [System.Web.Http.ActionName("GetProjectRequirement")]
        [System.Web.Http.HttpGet]
        public Project GetProjectRequirement(int idreq)
        {
            return _testProcedureService.GetProjectRequirement(idreq);
        }

        [System.Web.Http.ActionName("GetRequirement")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirement(int reqId)
        {
            return _testProcedureService.GetRequirement(reqId);
        }
        
        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public TestProcedure Delete(int id)
        {
            return _testProcedureService.Delete(id);
        }

        [System.Web.Http.ActionName("UpdateNumber")]
        [System.Web.Http.HttpPut]
        public TestProcedure UpdateNumber(TestProcedure testProcedure, int idReq)
        {
            return _testProcedureService.UpdateNumber(testProcedure, idReq);
        }

        [System.Web.Http.ActionName("IsAssigned")]
        [System.Web.Http.HttpGet]
        public AssignedStatusDTO IsAssgined(int? tpid,int? tcid)
        {
            return _testProcedureService.IsAssigned(tpid,tcid);
        }


          [System.Web.Http.ActionName("GetRequirementForTp")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirementForTp(int tpid)
        {
            return _testProcedureService.GetRequirementForTp(tpid);
        }

        [System.Web.Http.ActionName("CreateFromScripts")]
        [System.Web.Http.HttpPost]
        public List<TestProcedure> CreateFromScripts(Scripts[] scripts, int projectId)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testProcedureService.CreateFromScripts(scripts, projectId, user);
        }

    }
}
