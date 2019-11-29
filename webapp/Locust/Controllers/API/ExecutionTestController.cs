using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class ExecutionTestController : ApiController
    {
        private readonly IExecutionTestService _executionTestService;

        public ExecutionTestController(IExecutionTestService executionTestService)
        {
            _executionTestService = executionTestService;

        }

        [HttpPost]
        [System.Web.Http.ActionName("Save")]
        public bool Save(ExecutionTestEvidence[] data, int executionid)
        {
            return _executionTestService.Save(data, executionid);
            
        }

        [HttpGet]
        [System.Web.Http.ActionName("DeleteData")]
        public bool DeleteData(int executionId)
        {
            return _executionTestService.Delete(executionId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetTestCases")]
        public List<EvidenceDTO> GetTestCases(int executionId)
        {
            return _executionTestService.GetTCForExecutionGroup(executionId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetTestProcedures")]
        public List<EvidenceDTO> GetTestProcedures(int executionId)
        {
            return _executionTestService.GetTPForExecutionGroup(executionId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetTestScenarios")]
        public List<EvidenceDTO> GetTestScenarios(int executionId)
        {
            return _executionTestService.GetTSForExecutionGroup(executionId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetCombinedEvidence")]
        public List<EvidenceDTO> GetCombinedEvidence(int executionId)
        {
            var arrayCombined = _executionTestService.GetEvidenceForExecutionGroup(executionId);
            return arrayCombined;
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetAutomatedTestForExecutionGroup")]
        public List<EvidenceDTO> GetAutomatedTestForExecutionGroup(int executionId)
        {
            var automated = _executionTestService.GetAutomatedTestForExecutionGroup(executionId);
            return automated;
        }



    }
}