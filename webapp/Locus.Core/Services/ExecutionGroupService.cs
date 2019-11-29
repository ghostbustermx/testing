using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Services
{

    public interface IExecutionGroupService
    {
        ExecutionGroup Save(ExecutionGroup executionGroup, string user);

        ExecutionGroup Update(ExecutionGroup executionGroup, string user);

        ExecutionGroup Delete(int ExecutionId, string user);

        ExecutionGroup Enable(int ExecutionId, string user);

        ExecutionGroup Get(int ExecutionId);

        ExecutionGroup GetLastByProject(int projectId);

        List<ExecutionGroup> GetAll();

        List<ExecutionGroup> GetByProjectActives(int projectId);

        List<ExecutionGroup> GetByProjectInactives(int projectId);



    }

    class ExecutionGroupService : IExecutionGroupService
    {
        public readonly IExecutionGroupRepository executionGroupRepository;
        public readonly ITestExecutionRepository _testExecutionRepository;


        public ExecutionGroupService(IExecutionGroupRepository executionGroupRepository)
        {
            this.executionGroupRepository = executionGroupRepository;
        }

        public ExecutionGroupService(IExecutionGroupRepository executionGroupRepository, ITestExecutionRepository testExecutionRepository)
        {
            this.executionGroupRepository = executionGroupRepository;
            this._testExecutionRepository = testExecutionRepository;
        }

        public ExecutionGroup Delete(int ExecutionId, string user)
        {
            return executionGroupRepository.Delete(ExecutionId, user);
        }

        public ExecutionGroup Enable(int ExecutionId, string user)
        {
            return executionGroupRepository.Enable(ExecutionId, user);
        }

        public ExecutionGroup Get(int ExecutionId)
        {
            return executionGroupRepository.Get(ExecutionId);
        }

        public List<ExecutionGroup> GetAll()
        {
            return executionGroupRepository.GetAll();
        }

        public List<ExecutionGroup> GetByProjectActives(int projectId)
        {
            return executionGroupRepository.GetByProjectActives(projectId);
        }

        public List<ExecutionGroup> GetByProjectInactives(int projectId)
        {
            return executionGroupRepository.GetByProjectInactives(projectId);
        }

        public ExecutionGroup GetLastByProject(int projectId)
        {
            return executionGroupRepository.GetLastByProject(projectId);                    

        }

        public ExecutionGroup Save(ExecutionGroup executionGroup, string user)
        {
            return executionGroupRepository.Save(executionGroup, user);
        }

        public ExecutionGroup Update(ExecutionGroup executionGroup, string user)
        {
            _testExecutionRepository.ChangeExecutionsStatus(executionGroup.Execution_Group_Id);
            return executionGroupRepository.Update(executionGroup, user);
        }

      
    }
}
