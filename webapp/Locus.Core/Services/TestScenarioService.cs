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
    public interface ITestScenarioService
    {
        TestScenario Save(TestScenario testScenario, string user);

        TestScenario Update(TestScenario testScenario, string user);

        TestScenario Delete(int idTestScenario);

        TestScenario Enable(int idTestScenario);

        List<TestScenario> GetAll();

        TestScenario Get(int idTestScenario);

        TestScenario GetLastOne(int idReq, string creator, string date);

        TestScenario GetLastTestScenario(string creator, string date);

        TestScenario UpdateNumber(TestScenario testScenario, int idReq);

        Project GetProject(int idts);

        Requirement GetRequirement(int idts);

        Project GetProjectRequirement(int idreq);

        ChangeLog AddChangeLog(int id, string user);

        List<ChangeLog> TestScenarioChangeLogs(int id);

        ChangeLog Restore(ChangeLog change_log, string user);

        Requirement GetRequirementForTs(int tsId);
    }

    public class TestScenarioService : ITestScenarioService
    {
        private readonly ITestScenarioRepository _testScenarioRepository;
        private readonly ITestResultRepository _testResultRepository;

        //Constructor of TestCaseService and initialize testCaseRepository
        public TestScenarioService(ITestScenarioRepository testScenarioRepository)
        {
            _testScenarioRepository = testScenarioRepository;
        }

        public TestScenarioService(ITestScenarioRepository testScenarioRepository, ITestResultRepository testResultRepository)
        {
            _testScenarioRepository = testScenarioRepository;
            _testResultRepository= testResultRepository;
        }

        public ChangeLog AddChangeLog(int id, string user)
        {
            return _testScenarioRepository.AddChangeLog(id, user);
        }

        public TestScenario Delete(int idTestScenario)
        {
            return _testScenarioRepository.Delete(idTestScenario);
        }

        public TestScenario Enable(int idTestScenario)
        {
            return _testScenarioRepository.Enable(idTestScenario);
        }

        public TestScenario Get(int idTestScenario)
        {
            return _testScenarioRepository.Get(idTestScenario);
        }

        public List<TestScenario> GetAll()
        {
            return _testScenarioRepository.GetAll();
        }

        public TestScenario GetLastOne(int idReq, string creator, string date)
        {
            return _testScenarioRepository.GetLastOne(idReq, creator, date);
        }

        public TestScenario GetLastTestScenario(string creator, string date)
        {
            return _testScenarioRepository.GetLastTestScenario(creator, date);
        }

        public Project GetProject(int idts)
        {
            return _testScenarioRepository.GetProject(idts);
        }

        public Project GetProjectRequirement(int idreq)
        {
            return _testScenarioRepository.GetProjectRequirement(idreq);
        }

        public Requirement GetRequirement(int idts)
        {
            return _testScenarioRepository.GetRequirement(idts);
        }

        public Requirement GetRequirementForTs(int tsId)
        {
            return _testScenarioRepository.GetRequirementForTs(tsId);
        }

        public ChangeLog Restore(ChangeLog change_log, string user)
        {
            return _testScenarioRepository.Restore(change_log, user);
        }

        public TestScenario Save(TestScenario testScenario, string user)
        {
            return _testScenarioRepository.Save(testScenario, user);
        }

        public List<ChangeLog> TestScenarioChangeLogs(int id)
        {
            return _testScenarioRepository.TestScenarioChangeLogs(id);
        }

        public TestScenario Update(TestScenario testScenario, string user)
        {
            _testResultRepository.UpdateTitles(testScenario.Test_Scenario_Id, testScenario.Title, "TS");
            return _testScenarioRepository.Update(testScenario, user);
        }

        public TestScenario UpdateNumber(TestScenario testScenario, int idReq)
        {
            return _testScenarioRepository.UpdateNumber(testScenario, idReq);
        }
    }
}
