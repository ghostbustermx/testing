using Locus.Core.DTO;
using Locus.Core.Helpers;
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
    public interface ITestProcedureService
    {
        TestProcedure Save(TestProcedure testProcedure, string user);

        TestProcedure Update(TestProcedure testProcedure, string user);

        TestProcedure Delete(int idTestProcedure);

        TestProcedure Enable(int idTestProcedure);

        List<TestProcedure> GetAll();

        TestProcedure Get(int idTestProcedure);

        TestProcedure GetLastOne(int idReq, string creator, string date);

        TestProcedure GetLastTestProcedure(string creator, string date);

        TestProcedure UpdateNumber(TestProcedure testProcedure, int idReq);

        Project GetProject(int idtp);

        Requirement GetRequirement(int idtp);

        Project GetProjectRequirement(int idreq);

        ChangeLog AddChangeLog(int id, string user);

        List<ChangeLog> TestProcedureChangeLogs(int id);

        ChangeLog Restore(ChangeLog change_log, string user);

        TestProcedure GetTestProcedure(int test_Case_Id);

        AssignedStatusDTO IsAssigned(int? tpid,int? tcid);

        Requirement GetRequirementForTp(int tpId);
        List<TestProcedure> CreateFromScripts(Scripts[] scripts, int projectId, string user);
    }


    public class TestProcedureService : ITestProcedureService
    {
        private readonly ITestProcedureRepository _testProcedureRepository;
        private readonly IRequirementRepository _requirementRepository;
        private readonly ITestResultRepository _testResultRepository;

        //Constructor of TestCaseService and initialize testCaseRepository
        public TestProcedureService(ITestProcedureRepository testProcedureRepository)
        {
            _testProcedureRepository = testProcedureRepository;
        }

        public TestProcedureService(ITestProcedureRepository testProcedureRepository, ITestResultRepository testResultRepository, IRequirementRepository requirementRepository)
        {
            _testProcedureRepository = testProcedureRepository;
            _testResultRepository = testResultRepository;
            _requirementRepository = requirementRepository;
        }    

        public TestProcedure Delete(int idTestProcedure)
        {
            return _testProcedureRepository.Delete(idTestProcedure);
        }

        public TestProcedure Enable(int idTestProcedure)
        {
            return _testProcedureRepository.Enable(idTestProcedure);
        }

        public TestProcedure Get(int idTestProcedure)
        {
            return _testProcedureRepository.Get(idTestProcedure);
        }

        public List<TestProcedure> GetAll()
        {
            return _testProcedureRepository.GetAll();
        }

        public ChangeLog AddChangeLog(int testprocedureId, string user)
        {
            return _testProcedureRepository.AddChangeLog(testprocedureId, user);
        }

        public TestProcedure GetLastOne(int idReq, string creator, string date)
        {
            return _testProcedureRepository.GetLastOne(idReq, creator, date);
        }

        public TestProcedure GetLastTestProcedure(string creator, string date)
        {
            return _testProcedureRepository.GetLastTestProcedure(creator, date);
        }

        public Project GetProject(int idtp)
        {
            return _testProcedureRepository.GetProject(idtp);
        }

        public Project GetProjectRequirement(int idreq)
        {
            return _testProcedureRepository.GetProjectRequirement(idreq);
        }

        public Requirement GetRequirement(int idtp)
        {
            return _testProcedureRepository.GetRequirement(idtp);
        }

        public TestProcedure Save(TestProcedure testProcedure, string user)
        {
            return _testProcedureRepository.Save(testProcedure, user);
        }

        public TestProcedure Update(TestProcedure testProcedure, string user)
        {
            _testResultRepository.UpdateTitles(testProcedure.Test_Procedure_Id, testProcedure.Title, "TP");
            return _testProcedureRepository.Update(testProcedure, user);
        }

        public TestProcedure UpdateNumber(TestProcedure testProcedure, int idReq)
        {
            return _testProcedureRepository.UpdateNumber(testProcedure, idReq);
        }

        public List<ChangeLog> TestProcedureChangeLogs(int id)
        {
            return _testProcedureRepository.TestProcedureChangeLogs(id);
        }

        public ChangeLog Restore(ChangeLog change_log, string user)
        {
            return _testProcedureRepository.Restore(change_log, user);
        }

        public TestProcedure GetTestProcedure(int test_Case_Id)
        {
            return _testProcedureRepository.GetTestProcedureTc(test_Case_Id);
        }

        public AssignedStatusDTO IsAssigned(int? tpid ,int? tcid)
        {
            AssignedStatusDTO test = new AssignedStatusDTO();
            if (_testProcedureRepository.IsAssigned(tpid,tcid)) {
                test.assigned = true;
                test.message = "The Selected Test Case has been already been assigned to another Test Procedure";
                return test;
            }
            else
            {
                test.assigned = false;
                test.message = "";
                return test;
            }
            
        }

        public Requirement GetRequirementForTp(int tpId)
        {
            return _testProcedureRepository.GetRequirementForTp(tpId);
        }

        public List<TestProcedure> CreateFromScripts(Scripts[] scripts, int projectId, string user)
        {

            List<ScriptDTO> scriptDTOs = new List<ScriptDTO>();
            List<TestDTO> testCases = new List<TestDTO>();
            foreach (var script in scripts)
            {
                ScriptDTO scriptDTO = new ScriptDTO();
                scriptDTO.Id = script.Id;
                scriptDTO.RelatedTestCases = TextExtractorHelper.ExtractFromScript(script);
                 testCases.AddRange(_requirementRepository.GetTestCasesByNumber(scriptDTO.RelatedTestCases, projectId, script.Id));

            }

            return _testProcedureRepository.CreateTestProcedureFromTestCase(projectId, testCases, user);
        }
    }
}
