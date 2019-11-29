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
    public interface IExecutionTestService
    {
        bool Save(ExecutionTestEvidence[] data, int executionid);

        bool Delete(int executionId);

        List<EvidenceDTO> GetEvidenceForExecutionGroup(int executionId);

        List<EvidenceDTO> GetTCForExecutionGroup(int executionId);

        List<EvidenceDTO> GetTPForExecutionGroup(int executionId);


        List<EvidenceDTO> GetTSForExecutionGroup(int executionId);

        List<EvidenceDTO> GetAutomatedTestForExecutionGroup(int executionId);

        List<Scripts> GetScriptsByExecutionGroup(int executionId);



    }

    class ExecutionTestService : IExecutionTestService
    {
        private readonly IExecutionTestEvidence _executionTestRepository;


        public ExecutionTestService(IExecutionTestEvidence executionGroupRepository)
        {
            _executionTestRepository = executionGroupRepository;
        }



        public bool Delete(int executionId)
        {
            return _executionTestRepository.Delete(executionId);
        }



        public List<EvidenceDTO> GetEvidenceForExecutionGroup(int executionId)
        {
            List<EvidenceDTO> tc = _executionTestRepository.GetTCForExecutionGroup(executionId);
            List<EvidenceDTO> tp = _executionTestRepository.GetTPForExecutionGroup(executionId);
            List<EvidenceDTO> ts = _executionTestRepository.GetTSForExecutionGroup(executionId);
            List<EvidenceDTO> combined = tc.Concat(tp).Concat(ts).ToList();
            return combined;
        }

        public List<EvidenceDTO> GetAutomatedTestForExecutionGroup(int executionId)
        {
            return _executionTestRepository.GetAutomatedTestForExecutionGroup(executionId);
        }

        public List<EvidenceDTO> GetTCForExecutionGroup(int executionId)
        {
            return _executionTestRepository.GetTCForExecutionGroup(executionId);
        }

        public List<EvidenceDTO> GetTPForExecutionGroup(int executionId)
        {
            return _executionTestRepository.GetTPForExecutionGroup(executionId);
        }

        public List<EvidenceDTO> GetTSForExecutionGroup(int executionId)
        {
            return _executionTestRepository.GetTSForExecutionGroup(executionId);
        }

        public bool Save(ExecutionTestEvidence[] data, int executionid)
        {
            return _executionTestRepository.Save(data, executionid);
        }
        public List<Scripts> GetScriptsByExecutionGroup(int executionId)
        {
            return _executionTestRepository.GetScriptsByExecutionGroup(executionId);

        }


    }
}
