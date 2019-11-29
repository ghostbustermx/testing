using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Service for Requirement operations.
namespace Locus.Core.Services
{
    //Interface which contains methods for each CRUD operation
    public interface IRequirementService
    {
        Requirement Save(Requirement requirement, string user);

        Requirement Update(Requirement requirement, string user);

        Requirement Delete(int idRequirement, string user);

        List<Requirement> GetAll();

        Requirement Get(int idRequirement);

        List<TestCase> GetAllTestCase(int idReq);

        List<TestCase> GetAllTestCaseInactives(int idReq);

        List<Requirement> GetProject(int Project_Id);

        List<Requirement> GetProjectInactives(int Project_Id);

        List<TestScenario> GetAllTestScenario(int id);

        List<TestScenario> GetAllTestScenarioInactives(int id);

        List<TestProcedure> GetAllTestProcedure(int id);

        List<TestProcedure> GetAllTestProcedureInactives(int id);

        List<TestProcedure> GetAllTestProcedureNoTc(int id);

        Requirement Enable(int id, string user);

        List<ChangeLog> RequirementChangeLogs(int id);

        ChangeLog Restore(Requirement requirement, int version, string user);

        DashboardDTO GetDashboard(int projectId);

        Requirement GetRequirementbyTCId(int tcId);

        Requirement GetRequirementbyTPId(int tpId);

        List<TestCase> GetAllTCWithoutTP(int projectId);

        List<TestCase> GetAllTCWithoutTPByReq(int reqId);

        List<EvidenceDTO> GetEvidenceFromReq(int[] requirements);

        List<TestDTO> GetTestCases(int projectId);

        List<TestProcedure> GetAllTestProcedureByArray(RequirementDTO[] data);

        List<Requirement> GetRequirementsByTestEvidence(int id, string type);

        List<TestDTO> GetTestProcedures(int projectId);

        List<TestDTO> GetTestScenarios(int projectId);

        Requirement GetByReqNumber(int projectId, string reqnumber);

        List<string> GetSprints(int projectId);

        List<Requirement> GetNonActivesExecutedForTP(int executionId);
        List<Requirement> GetNonActivesExecutedForTC(int executionId);
        List<Requirement> GetNonActivesExecutedForTS(int executionId);

        List<TestCase> GetUniqueTestCases(int ProjectId);
        List<EvidenceDTO> GetAutomatedEvidenceFromReq(int[] ids);
        List<TestDTO> GetTestCasesByNumber(string[] TcNumbers, int projectId, int scriptId);
    }
    //Class which implements IRequirementService's methods and instance to IRequirementRepository
    public class RequirementService : IRequirementService
    {
        //Instance of IRequirementRepository
        private readonly IRequirementRepository _requirementRepository;
        private readonly ITestSuplementalRepository _testSuplementalRepository;
        

        //Constructor of RequirementService and initialize requirementRepository
        public RequirementService(IRequirementRepository requirementRepository)
        {
            _requirementRepository = requirementRepository;
        }

      
        public RequirementService(IRequirementRepository requirementRepository, ITestSuplementalRepository procedureSuplementalRepository)
        {
            _requirementRepository = requirementRepository;
            _testSuplementalRepository = procedureSuplementalRepository;
        }

        //Delete method calls to Delete Method From requirementRepository.
        public Requirement Delete(int idRequirement, string user)
        {
            return _requirementRepository.Delete(idRequirement, user);
        }

        public Requirement Enable(int id, string user)
        {
            return _requirementRepository.Enable(id, user);
        }

        //Get method calls to Get Method From requirementRepository.
        public Requirement Get(int idRequirement)
        {
            return _requirementRepository.Get(idRequirement);
        }
        //GetAll method calls to GetAll Method From requirementRepository.
        public List<Requirement> GetAll()
        {
            return _requirementRepository.GetAll();
        }

        public List<TestCase> GetAllTestCase(int idReq)
        {
            return _requirementRepository.GetAllTestCase(idReq);
        }

        public List<TestCase> GetAllTestCaseInactives(int idReq)
        {
            return _requirementRepository.GetAllTestCaseInactives(idReq);
        }

        public List<TestProcedure> GetAllTestProcedure(int id)
        {
            return _requirementRepository.GetAllTestProcedure(id);
        }

        public List<TestProcedure> GetAllTestProcedureInactives(int id)
        {
            return _requirementRepository.GetAllTestProcedureInactives(id);
        }

        public List<TestProcedure> GetAllTestProcedureNoTc(int id)
        {
            return _requirementRepository.GetAllTestProcedureNoTc(id);
        }

        public List<TestScenario> GetAllTestScenario(int id)
        {
            return _requirementRepository.GetAllTestScenario(id);
        }

        public List<TestScenario> GetAllTestScenarioInactives(int id)
        {
            return _requirementRepository.GetAllTestScenarioInactives(id);
        }

        public DashboardDTO GetDashboard(int projectId)
        {
            DashboardDTO dashboard = new DashboardDTO();
            List<Requirement> reqList = _requirementRepository.GetProject(projectId);
            List<TestSuplemental> suplementals = _testSuplementalRepository.GetForProject(projectId);
            dashboard.totalRequirements = reqList.Count;
            int totaltc = 0;
            int totaltp = 0;
            int totalts = 0;
            int missingtc = 0;
            List<Requirement> reqListMissingEvidence = _requirementRepository.GetAllReqWithoutEvidence(projectId);
            List<TestCase> testCases = _requirementRepository.GetUniqueTestCases(projectId);
            totaltc = testCases.Count;

            List<TestProcedure> testProcedures = _requirementRepository.GetUniqueTestProcedures(projectId);
            totaltp = testProcedures.Count;

            List<TestScenario> testScenarios = _requirementRepository.GetUniqueTestScenarios(projectId);
            totalts = testScenarios.Count;

            missingtc = reqListMissingEvidence.Count;

            dashboard.requirementsMissingTestEvidence = missingtc;
            dashboard.totalstp = _testSuplementalRepository.GetForProject(projectId).Count; 
            dashboard.totalTestCases = totaltc;
            dashboard.totalTestProcedures = totaltp;
            dashboard.totalTestScenarios = totalts;

            return dashboard;


        }

        public List<Requirement> GetProject(int Project_Id)
        {
            return _requirementRepository.GetProject(Project_Id);
        }

        public List<Requirement> GetProjectInactives(int Project_Id)
        {
            return _requirementRepository.GetProjectInactives(Project_Id);
        }

        public List<ChangeLog> RequirementChangeLogs(int id)
        {
            return _requirementRepository.RequirementChangeLogs(id);
        }

        public ChangeLog Restore(Requirement requirement, int version, string user)
        {
            return _requirementRepository.Restore(requirement, version, user);
        }

        //Save method calls to Save Method From requirementRepository.
        public Requirement Save(Requirement requirement, string user)
        {
            return _requirementRepository.Save(requirement, user);
        }
        //Update method calls to Update Method From requirementRepository.
        public Requirement Update(Requirement requirement, string user)
        {
            return _requirementRepository.Update(requirement, user);
        }

        public Requirement GetRequirementbyTCId(int tcId)
        {
            var requirement = _requirementRepository.GetRequirementbyTCId(tcId);
            return requirement;
        }

        public Requirement GetRequirementbyTPId(int tpId)
        {
            var requirement = _requirementRepository.GetRequirementbyTPId(tpId);
            return requirement;
        }

        public List<TestCase> GetAllTCWithoutTP(int projectId)
        {
            return _requirementRepository.GetAllTCWithoutTP(projectId);
        }

        public List<TestCase> GetAllTCWithoutTPByReq(int reqId)
        {
            return _requirementRepository.GetAllTCWithoutTPByReq(reqId);
        }

        public List<TestProcedure> GetAllTestProcedureByArray(RequirementDTO[] data)
        {
            return _requirementRepository.GetAllTestProcedureByArray(data);
        }

        public List<EvidenceDTO> GetEvidenceFromReq(int[] requirements)
        {
            return _requirementRepository.GetEvidenceFromReq(requirements);

        }

        public List<Requirement> GetRequirementsByTestEvidence(int id, string type)
        {
            return _requirementRepository.GetRequirementsByTestEvidence(id, type);
        }

        public List<TestDTO> GetTestCases(int projectId)
        {
            return _requirementRepository.GetTestCases(projectId);
        }

        public List<TestDTO> GetTestProcedures(int projectId)
        {
            return _requirementRepository.GetTestProcedures(projectId);
        }

        public List<TestDTO> GetTestScenarios(int projectId)
        {
            return _requirementRepository.GetTestScenarios(projectId);
        }

        public Requirement GetByReqNumber(int projectId, string reqnumber)
        {
            return _requirementRepository.GetByReqNumber(projectId, reqnumber);
        }
        public List<EvidenceDTO> GetAutomatedEvidenceFromReq(int[] ids)
        {
            return _requirementRepository.GetAutomatedEvidenceFromReq(ids);
        }

        public List<string> GetSprints(int projectId)
        {
            return _requirementRepository.GetSprints(projectId);
        }

        public List<Requirement> GetNonActivesExecutedForTP(int executionId)
        {
            return _requirementRepository.GetNonActivesExecutedForTP(executionId);
        }


        public List<TestCase> GetUniqueTestCases(int ProjectId)
        {
            return _requirementRepository.GetUniqueTestCases(ProjectId);
        }

        public List<Requirement> GetNonActivesExecutedForTC(int executionId)
        {
            return _requirementRepository.GetNonActivesExecutedForTC(executionId);
        }

        public List<Requirement> GetNonActivesExecutedForTS(int executionId)
        {
            return _requirementRepository.GetNonActivesExecutedForTS(executionId);
        }

        public List<TestDTO> GetTestCasesByNumber(string[] TcNumbers, int projectId, int scriptId)
        {
            return _requirementRepository.GetTestCasesByNumber(TcNumbers, projectId, scriptId);
        }
    }
}
