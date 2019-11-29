using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Services
{
    public interface ITestExecutionService
    {
        TestExecution Save(TestExecution testExecution);

        TestExecution Update(TestExecution testExecution);

        TestExecution ChangeState(int id,string state);

        TestExecution Get(int id);

        List<TestExecution> GetForGroup(int executionId);

        TestExecution GetLastExecution(int groupId, int projectId);

        ExecutionGroup GetByTestExecution(int groupId);

        List<TestExecution> GetByProject(int projectId);
    }

    public class TestExecutionService : ITestExecutionService
    {
        private readonly ITestExecutionRepository _testExecutionRepository;
        private readonly IExecutionGroupRepository _executionGroupRepository;

        public TestExecutionService(ITestExecutionRepository testExecutionRepository)
        {
            _testExecutionRepository = testExecutionRepository;
        }

        public TestExecutionService(ITestExecutionRepository testExecutionRepository, IExecutionGroupRepository executionGroupRepository)
        {
            _testExecutionRepository = testExecutionRepository;
            _executionGroupRepository = executionGroupRepository;
        }

        public TestExecution ChangeState(int id, string state)
        {
            return _testExecutionRepository.ChangeState(id, state);
        }

        public TestExecution Get(int id)
        {
            return _testExecutionRepository.Get(id);
        }

        public List<TestExecution> GetByProject(int projectId)
        {
            return _testExecutionRepository.GetByProject(projectId);
        }

        public ExecutionGroup GetByTestExecution(int groupId)
        {
            return _testExecutionRepository.GetByTestExecution(groupId);
        }

        public List<TestExecution> GetForGroup(int executionId)
        {
            return _testExecutionRepository.GetForGroup(executionId);
        }

        public TestExecution GetLastExecution(int groupId, int projectId)
        {
            return _testExecutionRepository.GetLastExecution(groupId, projectId);
        }

        public TestExecution Save(TestExecution testExecution)
        {
            return _testExecutionRepository.Save(testExecution);
        }

        public TestExecution Update(TestExecution testExecution)
        {
            return _testExecutionRepository.Update(testExecution);
        }
    }
}
