using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Locus.Core.Services
{
    public interface ITraceabilityFindingController
    {
        MissingTestEvidenceDTO GetTestEvidenceMissing(int id);

    }

    public class TraceabilityFindingService : ITraceabilityFindingController
    {
        private readonly IRequirementRepository _reqRepository;


        public TraceabilityFindingService(IRequirementRepository reqRepository)
        {
            _reqRepository = reqRepository;
        }



        public MissingTestEvidenceDTO GetTestEvidenceMissing(int id)
        {
            List<Requirement> requirements = _reqRepository.GetAllReqWithoutEvidence(id);
            List<Requirement> requirementsComplement = GetReqWithEvidenceDisable(id);
            List<Requirement> listTotal = requirements.Concat(requirementsComplement).ToList();


            var testCases = _reqRepository.GetAllTCWithoutTP(id);
            var testProcedures = _reqRepository.GetAllTPWithoutTC(id);

            MissingTestEvidenceDTO testEvidence = new MissingTestEvidenceDTO
            {
                Requirements = listTotal,
                TC = testCases,
                TP = testProcedures
            };
            return testEvidence;
        }
        private List<Requirement> CombineLists(List<Requirement> list1, List<Requirement> list2)
        {
            if (list1 == null)
            {
                return list2;
            }
            if (list2 == null)
            {
                return list1;
            }
            return list1.Concat(list2).ToList();
        }


        private List<Requirement> GetReqWithEvidenceDisable(int projectId)
        {
            List<Requirement> req_tc = _reqRepository.GetTCDisables(projectId);
            List<Requirement> req_tp = _reqRepository.GetTPDisables(projectId);
            List<Requirement> req_ts = _reqRepository.GetTSDisables(projectId);

            List<Requirement> tc_tp_ts= this.CombineLists(this.CombineLists(req_tc, req_tp), req_ts);



            List<Requirement> requierements = new List<Requirement>();

            foreach (var evidence in tc_tp_ts)
            {
                var total_TC = _reqRepository.GetCountTCTotal(evidence.Id);
                var TC = _reqRepository.GetCountTC(evidence.Id);

                var total_TP = _reqRepository.GetCountTPTotal(evidence.Id);
                var TP = _reqRepository.GetCountTP(evidence.Id);

                var total_TS = _reqRepository.GetCountTSTotal(evidence.Id);
                var TS = _reqRepository.GetCountTS(evidence.Id);

                if (total_TC.Equals(TC) && total_TP.Equals(TP) && total_TS.Equals(TS))
                {
                    var req = requierements.Where(x => x.Id == evidence.Id).FirstOrDefault();
                    if (req == null)
                    {
                        requierements.Add(evidence);
                    }
                }
            }
            return requierements;
        }
    }
}
