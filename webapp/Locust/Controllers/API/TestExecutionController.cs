using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class TestExecutionController: ApiController
    {
        private readonly ITestExecutionService _testExecutionService;

        public TestExecutionController(ITestExecutionService testExecutionService)
        {
            _testExecutionService = testExecutionService;
        }
        
        [HttpPost]
        [System.Web.Http.ActionName("Save")]
        public TestExecution Save(TestExecution testExecution)
        {
            testExecution.Creator = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testExecutionService.Save(testExecution);
        }


        [HttpPut]
        [System.Web.Http.ActionName("Update")]
        public TestExecution Update(TestExecution testExecution)
        {
            return _testExecutionService.Update(testExecution);
        }

        [HttpGet]
        [System.Web.Http.ActionName("Get")]
        public TestExecution Get(int id)
        {
            return _testExecutionService.Get(id);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetByProject")]
        public List<TestExecution> GetByProject(int projectId)
        {
            return _testExecutionService.GetByProject(projectId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("ChangeState")]
        public TestExecution ChangeState(int id, string state)
        {
            return _testExecutionService.ChangeState(id, state);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetForGroup")]
        public List<TestExecution> GetForGroup(int executionId)
        {
            return _testExecutionService.GetForGroup(executionId);
        }

    }
}