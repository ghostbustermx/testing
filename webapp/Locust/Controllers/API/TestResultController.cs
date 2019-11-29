using Locus.Core.DTO;
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
    public class TestResultController: ApiController
    {
        private readonly ITestResultService _testResultService;
        private readonly IUserService _userService;


        public TestResultController(ITestResultService testResultService)
        {
            _testResultService = testResultService;
        }

        public TestResultController(ITestResultService testResultService, IUserService userService)
        {
            _testResultService = testResultService;
            _userService = userService;
        }


        [System.Web.Http.ActionName("Save")]
        [HttpPost]
        public TestResult Save(TestResult result)
        {

            string user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testResultService.Save(result, user);
        }

        [System.Web.Http.ActionName("Get")]
        [HttpGet]
        public TestResult Get(int idTestResult)
        {
            return _testResultService.Get(idTestResult);
            
        }

        [System.Web.Http.ActionName("GetForUser")]
        [HttpGet]
        public List<TestResult> GetForUser(int testExecutionId)
        {
            string user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testResultService.GetForUser(testExecutionId, user);
            
        }

        [System.Web.Http.ActionName("RemoveFromExecution")]
        [HttpGet]
        public bool RemoveFromExecution(int testExecutionId)
        {
            string user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testResultService.RemoveUserFromExecution(testExecutionId, user);
            
        }

        [System.Web.Http.ActionName("RemoveFromUsersExecution")]
        [HttpPost]
        public bool RemoveUsersFromExecution(int executionId, User[] users)
        {
            return _testResultService.RemoveUsersFromExecution(executionId, users);
            
        }


        [System.Web.Http.ActionName("GetForGroup")]
        [HttpGet]
        public List<TestResult> getForExecutionGroup(int id)
        {
            return _testResultService.getForExecutionGroup(id);
        }

        [System.Web.Http.ActionName("SetStatus")]
        [HttpPost]
        public TestResult SetStatus(TestResult result)
        {
            return _testResultService.SetStatus(result);
        }

        [System.Web.Http.ActionName("GetForTestCase")]
        [HttpGet]
        public TestResult GetForTestCase(int id, int tcId)
        {
            return _testResultService.getForTestCase(id, tcId);
        }

        [System.Web.Http.ActionName("GetForTestProcedure")]
        [HttpGet]
        public TestResult GetForTestProcedure(int id, int tpId)
        {
            return _testResultService.getForTestProcedure(id, tpId);
        }

        [System.Web.Http.ActionName("GetForTestScenario")]
        [HttpGet]
        public TestResult GetForTestScenario(int id, int tsId)
        {
            return _testResultService.getForTestScenario(id, tsId);
        }


        [System.Web.Http.ActionName("GetCurrentHolder")]
        [HttpGet]
        public AssignedStatusDTO GetCurrentHolder(int executionId, int testId, string type)
        {
            string user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _testResultService.GetCurrentHolder(executionId, testId, type, user);
        }


        [System.Web.Http.ActionName("CreateTestResults")]
        [HttpGet]
        public List<TestResult> CreateTestResults(int groupId, int executionId)
        {
            return _testResultService.CreateTestResults(groupId, executionId);
        }


        [System.Web.Http.ActionName("GetToExecute")]
        [HttpGet]
        public TestResult GetToExecute(int testExecutionId)
        {
            string user= UserHelper.GetCurrentUser(User.Identity.Name);
            string photoUrl = _userService.GetByUsername(user).PhotoUrl;
            return _testResultService.GetToExecute(testExecutionId, user,photoUrl);
        }

        [System.Web.Http.ActionName("ReassignTestResult")]
        [HttpPost]
        public TestResult ReassignTestResult(TestResult[] testResults)
        {
            return _testResultService.ReassignTestResult(testResults[0], testResults[1]);
        }

        [System.Web.Http.ActionName("UpdateTestResults")]
        [HttpGet]
        public List<TestResult> UpdateTestResults(int groupId,int executionId)
        {
            return _testResultService.UpdateTestResults(groupId, executionId);
        }


        [System.Web.Http.ActionName("PassAll")]
        [HttpGet]
        public bool PassAll(int executionId, string tester, string evidence)
        {
            return _testResultService.PassAll(executionId, tester, evidence);
        }


        [System.Web.Http.ActionName("FailAll")]
        [HttpGet]
        public bool FailAll(int executionId, string tester, string evidence)
        {
            return _testResultService.FailAll(executionId, tester, evidence);
        }
    }
}