using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class TestSuplementalController : ApiController
    {

        private readonly ITestSuplementalService _testSuplementalService;

        public TestSuplementalController(ITestSuplementalService testSuplementalService)
        {
            _testSuplementalService = testSuplementalService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<TestSuplemental> GetAll()
        {
            return _testSuplementalService.GetAll();
        }

        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public TestSuplemental Delete(int idTestSuplemental)
        {
            return _testSuplementalService.Delete(idTestSuplemental);
        }

        [System.Web.Http.ActionName("Restore")]
        [System.Web.Http.HttpPost]
        public ChangeLog Restore(ChangeLog change_log)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testSuplementalService.Restore(change_log,user);
        }

        [System.Web.Http.ActionName("TestSuplementalChangeLogs")]
        [System.Web.Http.HttpGet]
        public List<ChangeLog> TestSuplementalChangeLogs(int id)
        {
            return _testSuplementalService.TestSuplementalChangeLogs(id);
        }

        [System.Web.Http.ActionName("Enable")]
        [System.Web.Http.HttpGet]
        public TestSuplemental Enable(int id)
        {
            return _testSuplementalService.Enable(id);
        }

        [System.Web.Http.ActionName("AddChangeLog")]
        [System.Web.Http.HttpGet]
        public ChangeLog AddChangeLog(int id)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testSuplementalService.AddChangeLog(id, user);
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public TestSuplemental Get(int idTestSuplemental)
        {
            return _testSuplementalService.Get(idTestSuplemental);
        }

        [System.Web.Http.ActionName("GetLastOne")]
        [System.Web.Http.HttpGet]
        public TestSuplemental GetLastOne(int idTestProcedure, int idTestScenario, string creator, string date)
        {
            return _testSuplementalService.GetLastOne(idTestProcedure, idTestScenario, creator, date);
        }

        [System.Web.Http.ActionName("GetForProject")]
        [System.Web.Http.HttpGet]
        public List<TestSuplemental> GetForProject(int idProject)
        {
            return _testSuplementalService.GetForProject(idProject);
        }


        [System.Web.Http.ActionName("GetByNumber")]
        [System.Web.Http.HttpGet]
        public TestSuplemental GetByNumber(int idProject, string number)
        {
            return _testSuplementalService.GetByNumber(idProject, number);
        }

        [System.Web.Http.ActionName("GetForProjectInactives")]
        [System.Web.Http.HttpGet]
        public List<TestSuplemental> GetForProjectInactives(int idProject)
        {
            return _testSuplementalService.GetForProjectInactives(idProject);
        }

        [System.Web.Http.ActionName("GetLastTestSuplemental")]
        [System.Web.Http.HttpGet]
        public TestSuplemental GetLastTestSuplemental(string creator, string date)
        {
            return _testSuplementalService.GetLastTestSuplemental(creator, date);
        }

        [System.Web.Http.ActionName("GetProcedures")]
        [System.Web.Http.HttpGet]
        public List<TestProcedure> GetProcedures(int idstp)
        {
            return _testSuplementalService.GetProcedures(idstp);
        }

        [System.Web.Http.ActionName("GetForTestProcedure")]
        [System.Web.Http.HttpGet]
        public List<TestSuplemental> GetForTestProcedure(int idTp)
        {
            return _testSuplementalService.GetForTestProcedure(idTp);
        }

        [System.Web.Http.ActionName("GetForTestScenario")]
        [System.Web.Http.HttpGet]
        public List<TestSuplemental> GetForTestScenario(int idTs)
        {
            return _testSuplementalService.GetForTestScenario(idTs);
        }

        [System.Web.Http.ActionName("GetScenarios")]
        [System.Web.Http.HttpGet]
        public List<TestScenario> GetScenarios(int idstp)
        {
            return _testSuplementalService.GetScenarios(idstp);
        }

        [System.Web.Http.ActionName("GetProject")]
        [System.Web.Http.HttpGet]
        public Project GetProject(int idtp, int idts)
        {
            return _testSuplementalService.GetProject(idtp, idts);
        }

        [System.Web.Http.ActionName("GetRequirement")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirement(int idtp, int idts)
        {
            return _testSuplementalService.GetRequirement(idtp, idts);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public TestSuplemental Save(TestSuplemental testSuplemental)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testSuplementalService.Save(testSuplemental, user);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public TestSuplemental Update(TestSuplemental testSuplemental)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testSuplementalService.Update(testSuplemental,user);
        }

        [System.Web.Http.ActionName("UpdateNumber")]
        [System.Web.Http.HttpPut]
        public TestSuplemental UpdateNumber(TestSuplemental testSuplemental)
        {
            return _testSuplementalService.UpdateNumber(testSuplemental);
        }

        [System.Web.Http.ActionName("GetAllSTPByProject")]
        [System.Web.Http.HttpGet]
        public IEnumerable<SupplementalTestProcedureDTO> GetAllSTPByProject(int idProject)
        {

            IEnumerable<SupplementalTestProcedureDTO> list = _testSuplementalService.GetAllSTPByProject(idProject);

            return list;
        }
    }
}
