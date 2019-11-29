using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System.Collections.Generic;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class RequirementController : ApiController
    {
        private readonly IRequirementService _requirementService;

        public RequirementController(IRequirementService requirementService)
        {
            _requirementService = requirementService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<Requirement> GetAll()
        {
            return _requirementService.GetAll();
        }

        [System.Web.Http.ActionName("RequirementChangeLogs")]
        [System.Web.Http.HttpGet]
        public List<ChangeLog> RequirementChangeLogs(int id)
        {
            return _requirementService.RequirementChangeLogs(id);
        }

        [System.Web.Http.ActionName("Restore")]
        [System.Web.Http.HttpPost]
        public ChangeLog Restore(Requirement requirement, int version)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _requirementService.Restore(requirement, version, user);
        }

        [System.Web.Http.ActionName("GetAllTestScenario")]
        [System.Web.Http.HttpGet]
        public List<TestScenario> GetAllTestScenario(int id)
        {
            return _requirementService.GetAllTestScenario(id);
        }

        [System.Web.Http.ActionName("GetAllTestScenarioInactives")]
        [System.Web.Http.HttpGet]
        public List<TestScenario> GetAllTestScenarioInactives(int id)
        {
            return _requirementService.GetAllTestScenarioInactives(id);
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public Requirement Get(int id)
        {
            return _requirementService.Get(id);
        }

        [System.Web.Http.ActionName("GetAllTestCase")]
        [System.Web.Http.HttpGet]
        public List<TestCase> GetAllTestCase(int id)
        {
            return _requirementService.GetAllTestCase(id);
        }

        [System.Web.Http.ActionName("GetAllTestCaseInactives")]
        [System.Web.Http.HttpGet]
        public List<TestCase> GetAllTestCaseInactives(int id)
        {
            return _requirementService.GetAllTestCaseInactives(id);
        }


        [System.Web.Http.ActionName("GetAllTestProcedure")]
        [System.Web.Http.HttpGet]
        public List<TestProcedure> GetAllTestProcedure(int id)
        {
            return _requirementService.GetAllTestProcedure(id);
        }

        [System.Web.Http.ActionName("GetAllTestProcedureInactives")]
        [System.Web.Http.HttpGet]
        public List<TestProcedure> GetAllTestProcedureInactives(int id)
        {
            return _requirementService.GetAllTestProcedureInactives(id);
        }

        [System.Web.Http.ActionName("GetProject")]
        [System.Web.Http.HttpGet]
        public List<Requirement> GetProject(int Project_Id)
        {
            return _requirementService.GetProject(Project_Id);
        }


        [System.Web.Http.ActionName("GetProjectInactives")]
        [System.Web.Http.HttpGet]
        public List<Requirement> GetProjectInactives(int Project_Id)
        {
            return _requirementService.GetProjectInactives(Project_Id);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public Requirement Save(Requirement requirement)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _requirementService.Save(requirement, user);
        }

        [System.Web.Http.ActionName("Enable")]
        [System.Web.Http.HttpGet]
        public Requirement Enable(int id)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _requirementService.Enable(id, user);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public Requirement Update(Requirement requirement)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _requirementService.Update(requirement, user);
        }

        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public Requirement Delete(int id)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _requirementService.Delete(id, user);
        }

        [System.Web.Http.ActionName("GetRequirementbyTCId")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirementbyTCId(int id)
        {
            return _requirementService.GetRequirementbyTCId(id);
        }

        [System.Web.Http.ActionName("GetRequirementbyTPId")]
        [System.Web.Http.HttpGet]
        public Requirement GetRequirementbyTPId(int id)
        {
            return _requirementService.GetRequirementbyTPId(id);
        }

        [System.Web.Http.ActionName("GetAllTCWithoutTP")]
        [HttpGet]
        public List<TestCase> GetAllTCWithoutTP(int projectId)
        {
            return _requirementService.GetAllTCWithoutTP(projectId);
        }

        [System.Web.Http.ActionName("GetAllTCWithoutTPByReq")]
        [HttpGet]
        public List<TestCase> GetAllTCWithoutTPByReq(int reqId)
        {
            return _requirementService.GetAllTCWithoutTPByReq(reqId);
        }


        [System.Web.Http.ActionName("GetEvidenceFromReq")]
        [HttpPost]
        public List<EvidenceDTO> GetEvidenceFromReq(int[] ids)
        {
            return _requirementService.GetEvidenceFromReq(ids);
        }

        [System.Web.Http.ActionName("GetAutomatedEvidenceFromReq")]
        [HttpPost]
        public List<EvidenceDTO> GetAutomatedEvidenceFromReq(int[] ids)
        {
            return _requirementService.GetAutomatedEvidenceFromReq(ids);
        }



        [System.Web.Http.ActionName("GetRequirementsByTestEvidence")]
        [HttpGet]
        public List<Requirement> GetRequirementsByTestEvidence(int id, string type)
        {
            return _requirementService.GetRequirementsByTestEvidence(id, type);
        }

        [System.Web.Http.ActionName("GetSprints")]
        [HttpGet]
        public List<string> GetSprints(int projectId)
        {
            return _requirementService.GetSprints(projectId);
        }

        [System.Web.Http.ActionName("GetTestCasesByNumber")]
        [HttpPost]
        public List<TestDTO> GetTestCasesByNumber(string[] TcNumbers, int projectId, int scriptId)
        {
            return _requirementService.GetTestCasesByNumber(TcNumbers, projectId, scriptId);
        }


    }
}
