using Locus.Core.DTO;
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
    public class StepController : ApiController
    {
        private readonly IStepService _stepService;

        public enum TypeOfSaving
        {
            SimpleSave = 1,
            WithSTPRelationForTP = 2,
            WithSTPRelationForTS = 3
        }

        public enum TypeOfEvidence
        {
            STP = 1,
            TC = 2,
            TS = 3,
            TP = 4
        }

        public StepController(IStepService stepService)
        {
            _stepService = stepService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public IEnumerable<Step> GetAll()
        {
            return _stepService.GetAll();
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public Step Get(int id)
        {
            return _stepService.Get(id);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public Step Save(Step step)
        {
            return _stepService.Save(step);
        }

        [System.Web.Http.ActionName("SaveArray")]
        [System.Web.Http.HttpPost]
        public Step[] SaveArray(Step[] steps, int projectId, int TypeOfSave)
        {
            switch (TypeOfSave)
            {
                case 1:
                    return _stepService.SaveArray(steps, projectId ,(int)TypeOfSaving.SimpleSave);
                case 2:
                    return _stepService.SaveArray(steps, projectId,(int)TypeOfSaving.WithSTPRelationForTP);
                case 3:
                    return _stepService.SaveArray(steps, projectId,(int)TypeOfSaving.WithSTPRelationForTS); 
                default:
                    return _stepService.SaveArray(steps, projectId,(int)TypeOfSaving.SimpleSave);
            }

            
        }

        [System.Web.Http.ActionName("UpdateArray")]
        [System.Web.Http.HttpPost]
        public Step[] UpdateArray(Step[] steps, int projectId, int TypeOfSave, int TestEvidenceId, int Evidence)
        {
            int EvidenceType = 0;

            switch (Evidence)
            {
                case (int)TypeOfEvidence.STP: EvidenceType = (int)TypeOfEvidence.STP; break;
                case (int)TypeOfEvidence.TC: EvidenceType = (int)TypeOfEvidence.TC; break;
                case (int)TypeOfEvidence.TS: EvidenceType = (int)TypeOfEvidence.TS; break;
                case (int)TypeOfEvidence.TP: EvidenceType = (int)TypeOfEvidence.TP; break;
            }




            switch (TypeOfSave)
            {
                case 1:
                    return _stepService.UpdateArray(steps, projectId, (int)TypeOfSaving.SimpleSave, TestEvidenceId, EvidenceType);
                case 2:
                    return _stepService.UpdateArray(steps, projectId, (int)TypeOfSaving.WithSTPRelationForTP, TestEvidenceId, EvidenceType);
                case 3:
                    return _stepService.UpdateArray(steps, projectId, (int)TypeOfSaving.WithSTPRelationForTS, TestEvidenceId, EvidenceType);
                default:
                    return _stepService.UpdateArray(steps, projectId, (int)TypeOfSaving.SimpleSave, TestEvidenceId, EvidenceType);
            }
        }


        [System.Web.Http.ActionName("DeleteArray")]
        [System.Web.Http.HttpPost]
        public Step[] DeleteArray(Step[] steps)
        {
            return _stepService.DeleteArray(steps);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public Step Update(Step step)
        {
            return _stepService.Update(step);
        }

        [System.Web.Http.ActionName("DeleteForTC")]
        [System.Web.Http.HttpDelete]
        public List<Step> DeleteForTC(int tcId)
        {
            return _stepService.DeleteForTC(tcId);
        }

        [System.Web.Http.ActionName("GetForTestCase")]
        [System.Web.Http.HttpGet]
        public List<Step> GetForTestCase(int tcId)
        {
            return _stepService.GetForTestCase(tcId);
        }

        [System.Web.Http.ActionName("GetForTestScenario")]
        [System.Web.Http.HttpGet]
        public List<Step> GetForTestScenario(int tsId)
        {
            return _stepService.GetForTestScenario(tsId);
        }

        [System.Web.Http.ActionName("GetForTestSuplemental")]
        [System.Web.Http.HttpGet]
        public List<Step> GetForTestSuplemental(int stpId)
        {
            return _stepService.GetForTestSuplemental(stpId);
        }


        [System.Web.Http.ActionName("GetForTestProcedure")]
        [System.Web.Http.HttpGet]
        public List<Step> GetForTestProcedure(int tpId)
        {
            return _stepService.GetForTestProcedure(tpId);
        }

        [System.Web.Http.ActionName("GetForTestProcedureSTP")]
        [System.Web.Http.HttpGet]
        public List<StepDTO> GetForTestProcedureSTP(int projectId, int tpId)
        {
            return _stepService.GetForTestProcedureSTP(projectId, tpId);
        }

        [System.Web.Http.ActionName("GetForTestScenarioSTP")]
        [System.Web.Http.HttpGet]
        public List<StepDTO> GetForTestScenarioSTP(int projectId, int tpId)
        {
            return _stepService.GetForTestScenarioSTP(projectId, tpId);
        }

        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public Step Delete(int id)
        {
            return _stepService.Delete(id);
        }
    }
}
