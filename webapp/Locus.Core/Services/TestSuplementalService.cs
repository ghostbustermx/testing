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
    public interface ITestSuplementalService
    {
        TestSuplemental Save(TestSuplemental testSuplemental, string user);

        TestSuplemental Update(TestSuplemental testSuplemental, string user);

        TestSuplemental Delete(int idTestSuplemental);

        TestSuplemental Enable(int idTestSuplemental);

        List<TestSuplemental> GetAll();

        TestSuplemental Get(int idTestSuplemental);

        TestSuplemental GetLastOne(int idTestProcedure, int idTestScenario, string creator, string date);

        TestSuplemental GetLastTestSuplemental(string creator, string date);

        TestSuplemental UpdateNumber(TestSuplemental testSuplemental);

        List<SupplementalTestProcedureDTO> GetAllSTPByProject(int idProject);

        Project GetProject(int idtp, int idts);

        Requirement GetRequirement(int idtp, int idts);

        List<TestSuplemental> GetForProject(int idProject);

        List<TestSuplemental> GetForProjectInactives(int idProject);

        List<TestSuplemental> GetForTestProcedure(int tpId);

        List<TestSuplemental> GetForTestScenario(int idTs);

        List<TestProcedure> GetProcedures(int idstp);

        List<TestScenario> GetScenarios(int idstp);

        ChangeLog AddChangeLog(int testsuplementalId, string user);

        List<ChangeLog> TestSuplementalChangeLogs(int id);

        ChangeLog Restore(ChangeLog change_log, string user);

        TestSuplemental GetByNumber(int projectId, string number);
    }
    public class TestSuplementalService : ITestSuplementalService
    {
        private readonly ITestSuplementalRepository _testSuplementalRepository;

        //Constructor of TestCaseService and initialize testCaseRepository
        public TestSuplementalService(ITestSuplementalRepository testSuplementalRepository)
        {
            _testSuplementalRepository = testSuplementalRepository;
        }

        public ChangeLog AddChangeLog(int testsuplementalId, string user)
        {
            return _testSuplementalRepository.AddChangeLog(testsuplementalId, user);
        }

        public TestSuplemental Delete(int idTestSuplemental)
        {
            return _testSuplementalRepository.Delete(idTestSuplemental);
        }

        public TestSuplemental Enable(int idTestSuplemental)
        {
            return _testSuplementalRepository.Enable(idTestSuplemental);
        }

        public TestSuplemental Get(int idTestSuplemental)
        {
            return _testSuplementalRepository.Get(idTestSuplemental);
        }

        public List<TestSuplemental> GetAll()
        {
            return _testSuplementalRepository.GetAll();
        }

        public List<SupplementalTestProcedureDTO> GetAllSTPByProject(int idProject)
        {
            return _testSuplementalRepository.GetAllSTPByProject(idProject);
        }

        public TestSuplemental GetByNumber(int projectId, string number)
        {
            return _testSuplementalRepository.GetByNumber(projectId, number);
        }

        public List<TestSuplemental> GetForProject(int idProject)
        {
            return _testSuplementalRepository.GetForProject(idProject);
        }

        public List<TestSuplemental> GetForProjectInactives(int idProject)
        {
            return _testSuplementalRepository.GetForProjectInactives(idProject);
        }

        public List<TestSuplemental> GetForTestProcedure(int tpId)
        {
            return _testSuplementalRepository.GetForTestProcedure(tpId);
        }

        public List<TestSuplemental> GetForTestScenario(int idTs)
        {
            return _testSuplementalRepository.GetForTestScenario(idTs);
        }

        public TestSuplemental GetLastOne(int idTestProcedure, int idTestScenario, string creator, string date)
        {
            return _testSuplementalRepository.GetLastOne(idTestProcedure, idTestScenario, creator, date);
        }

        public TestSuplemental GetLastTestSuplemental(string creator, string date)
        {
            return _testSuplementalRepository.GetLastTestSuplemental(creator, date);
        }

        public List<TestProcedure> GetProcedures(int idstp)
        {
            return _testSuplementalRepository.GetProcedures(idstp);
        }

        public Project GetProject(int idtp, int idts)
        {
            return _testSuplementalRepository.GetProject(idtp, idts);
        }

        public Requirement GetRequirement(int idtp, int idts)
        {
            return _testSuplementalRepository.GetRequirement(idtp, idts);
        }

        public List<TestScenario> GetScenarios(int idstp)
        {
            return _testSuplementalRepository.GetScenarios(idstp);
        }

        public ChangeLog Restore(ChangeLog change_log, string user)
        {
            return _testSuplementalRepository.Restore(change_log, user);
        }

        public TestSuplemental Save(TestSuplemental testSuplemental, string user)
        {
            return _testSuplementalRepository.Save(testSuplemental, user);
        }

        public List<ChangeLog> TestSuplementalChangeLogs(int id)
        {
            return _testSuplementalRepository.TestSuplementalChangeLogs(id);
        }

        public TestSuplemental Update(TestSuplemental testSuplemental, string user)
        {
            return _testSuplementalRepository.Update(testSuplemental, user);
        }

        public TestSuplemental UpdateNumber(TestSuplemental testSuplemental)
        {
            return _testSuplementalRepository.UpdateNumber(testSuplemental);
        }
    }
}
