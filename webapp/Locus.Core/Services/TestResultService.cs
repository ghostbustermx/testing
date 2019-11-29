using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Services
{
    public interface ITestResultService
    {
        TestResult Save(TestResult testResult, string user);

        TestResult Delete(int idTestResult);

        TestResult getForTestCase(int idTestCase, int executionId);

        TestResult getForTestProcedure(int idTestProcedure, int executionId);

        TestResult getForTestScenario(int idTestScenario, int executionId);

        List<TestResult> getForExecutionGroup(int idExecutionGroup);

        TestResult SetStatus(TestResult testResult);

        AssignedStatusDTO GetCurrentHolder(int executionId, int testId, string type, string user);

        List<TestResult> CreateTestResults(int groupId, int executionId);

        TestResult Get(int idTestResult);

        TestResult GetToExecute(int testExecutionId, string user, string PhotoUrl);

        TestResult ReassignTestResult(TestResult testRemoved, TestResult testAssigned);

        List<TestResult> GetForUser(int testExecutionId, string user);

        bool RemoveUserFromExecution(int testExecutionId, string user);

        List<TestResult> UpdateTestResults(int groupId, int executionId);

        bool PassAll(int executionId, string tester, string action);

        bool FailAll(int executionId, string tester, string action);

        bool RemoveUsersFromExecution(int executionId, User[] userNames);
    }

    class TestResultService : ITestResultService
    {

        private readonly ITestResultRepository _testResultRepository;

        public TestResultService(ITestResultRepository testResultRepository)
        {
            _testResultRepository = testResultRepository;
        }

        public List<TestResult> CreateTestResults(int groupId, int executionId)
        {
            return _testResultRepository.CreateTestResults(groupId, executionId);
        }

        public TestResult Delete(int idTestResult)
        {
            throw new NotImplementedException();
        }

        public bool FailAll(int executionId, string tester, string action)
        {
            return _testResultRepository.FailAll(executionId, tester, action);
        }

        public TestResult Get(int idTestResult)
        {
            return _testResultRepository.Get(idTestResult);
        }

        public AssignedStatusDTO GetCurrentHolder(int executionId, int testId, string type, string user)
        {
            return _testResultRepository.GetCurrentHolder(executionId, testId, type, user);
        }

        public List<TestResult> getForExecutionGroup(int idExecutionGroup)
        {
            return _testResultRepository.getForExecutionGroup(idExecutionGroup);
        }

        public TestResult getForTestCase(int idTestCase, int executionId)
        {
            return _testResultRepository.getForTestCase(idTestCase, executionId);
        }

        public TestResult getForTestProcedure(int idTestProcedure, int executionId)
        {
            return _testResultRepository.getForTestProcedure(idTestProcedure, executionId);
        }

        public TestResult getForTestScenario(int idTestScenario, int executionId)
        {
            return _testResultRepository.getForTestScenario(idTestScenario, executionId);
        }

        public List<TestResult> GetForUser(int testExecutionId, string user)
        {
            return _testResultRepository.GetForUser(testExecutionId, user);
        }

        public TestResult GetToExecute(int testExecutionId, string user, string PhotoUrl)
        {
            return _testResultRepository.GetToExecute(testExecutionId, user, PhotoUrl);
        }

        public bool PassAll(int executionId, string tester, string action)
        {
            return _testResultRepository.PassAll(executionId, tester, action);
        }

        public TestResult ReassignTestResult(TestResult testUpdated, TestResult testAssigned)
        {
            return _testResultRepository.ReassignTestResult(testUpdated, testAssigned);
        }

        public bool RemoveUserFromExecution(int testExecutionId, string user)
        {
            return _testResultRepository.RemoveUserFromExecution(testExecutionId, user);
        }

        public bool RemoveUsersFromExecution(int executionId, User[] userNames)
        {
            return _testResultRepository.RemoveUsersFromExecution(executionId, userNames);
        }

        public TestResult Save(TestResult testResult, string user)
        {
            return _testResultRepository.Save(testResult, user);
        }

        public TestResult SetStatus(TestResult testResult)
        {
            return _testResultRepository.SetStatus(testResult);
        }

        public List<TestResult> UpdateTestResults(int groupId, int executionId)
        {
            return _testResultRepository.UpdateTestResults(groupId, executionId); 
        }
    }
}
