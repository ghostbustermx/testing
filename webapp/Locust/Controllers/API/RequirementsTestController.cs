using Locus.Core.Models;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class RequirementsTestController : ApiController
    {
        private readonly IRequirementsTestService _rtService;


        public RequirementsTestController(IRequirementsTestService rtService)
        {
            _rtService = rtService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public IEnumerable<RequirementsTest> GetAll()
        {
            return _rtService.GetAll();
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public RequirementsTest Get(int id)
        {
            return _rtService.Get(id);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public RequirementsTest Save(RequirementsTest rt)
        {
            return _rtService.Save(rt);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public RequirementsTest Update(RequirementsTest rt)
        {
            return _rtService.Update(rt);
        }

        [System.Web.Http.ActionName("DeleteTestCase")]
        [System.Web.Http.HttpGet]
        public RequirementsTest DeleteTestCase(int reqId, int tcId)
        {
            return _rtService.DeleteTestCase(reqId, tcId);
        }

        [System.Web.Http.ActionName("DeleteTestProcedure")]
        [System.Web.Http.HttpGet]
        public RequirementsTest DeleteTestProcedure(int reqId, int tpId)
        {
            return _rtService.DeleteTestProcedure(reqId, tpId);
        }

        [System.Web.Http.ActionName("DeleteTestScenario")]
        [System.Web.Http.HttpGet]
        public RequirementsTest DeleteTestScenario(int reqId, int tsId)
        {
            return _rtService.DeleteTestScenario(reqId, tsId);
        }

        [System.Web.Http.ActionName("GetTestCaseRelations")]
        [System.Web.Http.HttpGet]
        public List<RequirementsTest> GetTestCaseRelations(int id)
        {
            return _rtService.GetTestCaseRelations(id);
        }

        [System.Web.Http.ActionName("GetTestScenarioRelations")]
        [System.Web.Http.HttpGet]
        public List<RequirementsTest> GetTestScenarioRelations(int id)
        {
            return _rtService.GetTestScenarioRelations(id);
        }

        [System.Web.Http.ActionName("GetTestProcedureRelations")]
        [System.Web.Http.HttpGet]
        public List<RequirementsTest> GetTestProcedureRelations(int id)
        {
            return _rtService.GetTestProcedureRelations(id);
        }



        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public RequirementsTest Delete(int id)
        {
            return _rtService.Delete(id);
        }
    }
}
