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
    //Interface which contains methods for each CRUD operation
    public interface IStepService
    {
        Step Save(Step step);

        Step[] SaveArray(Step[] steps, int projectId ,int TypeOfSave);

        Step[] UpdateArray(Step[] steps, int projectId, int TypeOfSave, int TestEvidenceId, int Evidence);

        Step[] DeleteArray(Step[] steps);

        Step Update(Step step);

        Step Delete(int idStep);

        List<Step> GetAll();

        Step Get(int idStep);

        List<Step> DeleteForTC(int tcId);

        List<Step> GetForTestCase(int tcId);

        List<Step> GetForTestScenario(int tsId);

        List<Step> GetForTestProcedure(int tpId);

        List<Step> GetForTestSuplemental(int stpId);

        List<Step> GetForTestCaseOrder(int tcId);

        List<Step> GetForTestScenarioOrder(int tsId);

        List<Step> GetForTestProcedureOrder(int tpId);

        List<Step> GetForTestSuplementalOrder(int stpId);

        List<StepDTO> GetForTestProcedureSTP(int projectId, int tpId);

        List<StepDTO> GetForTestScenarioSTP(int projectId, int tpId);

    }



    public class StepService : IStepService
    {
        //Instance of ITestCaseRepository
        private readonly IStepRepository _stepRepository;

        //Constructor of TestCaseService and initialize testCaseRepository
        public StepService(IStepRepository stepRepository)
        {
            _stepRepository = stepRepository;
        }

        public Step Delete(int idStep)
        {
            return _stepRepository.Delete(idStep);
        }

        public Step[] DeleteArray(Step[] steps)
        {
            return _stepRepository.DeleteArray(steps);
        }

        public List<Step> DeleteForTC(int tcId)
        {
            return _stepRepository.DeleteForTC(tcId);
        }

        public Step Get(int idStep)
        {
            return _stepRepository.Get(idStep);
        }

        public List<Step> GetAll()
        {
            return _stepRepository.GetAll();
        }

        public List<Step> GetForTestCase(int tcId)
        {
            return _stepRepository.GetForTestCase(tcId);
        }

        public List<Step> GetForTestCaseOrder(int tcId)
        {
            return _stepRepository.GetForTestCaseOrder(tcId);
        }

        public List<Step> GetForTestProcedure(int tpId)
        {
            return _stepRepository.GetForTestProcedure(tpId);
        }

        public List<Step> GetForTestProcedureOrder(int tpId)
        {
            return _stepRepository.GetForTestProcedureOrder(tpId);
        }

        public List<StepDTO> GetForTestProcedureSTP(int projectId, int tpId)
        {
            return _stepRepository.GetForTestProcedureSTP(projectId, tpId);
        }

        public List<Step> GetForTestScenario(int tsId)
        {
            return _stepRepository.GetForTestScenario(tsId);
        }

        public List<Step> GetForTestScenarioOrder(int tsId)
        {
            return _stepRepository.GetForTestScenarioOrder(tsId);
        }

        public List<StepDTO> GetForTestScenarioSTP(int projectId, int tpId)
        {
            return _stepRepository.GetForTestScenarioSTP(projectId, tpId);
        }

        public List<Step> GetForTestSuplemental(int stpId)
        {
            return _stepRepository.GetForTestSuplemental(stpId);
        }

        public List<Step> GetForTestSuplementalOrder(int stpId)
        {
            return _stepRepository.GetForTestSuplementalOrder(stpId);
        }

        public Step Save(Step step)
        {
            if (step.number_steps != 0)
                return _stepRepository.Save(step);
            else
            {
                return null;
            }

        }

        public Step[] SaveArray(Step[] steps, int projectId , int TypeOfSave)
        {
            return _stepRepository.SaveArray(steps, projectId ,TypeOfSave);
        }

        public Step Update(Step step)
        {
            return _stepRepository.Update(step);
        }

        public Step[] UpdateArray(Step[] steps, int projectId, int TypeOfSave, int TestEvidenceId, int Evidence)
        {
            return _stepRepository.UpdateArray(steps, projectId, TypeOfSave, TestEvidenceId, Evidence);
        }
    }
}
