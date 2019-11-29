using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Service for TestCase operations.
namespace Locus.Core.Services
{
    //Interface which contains methods for each CRUD operation
    public interface ITestCaseService
    {
        TestCase Save(TestCase testCase, string user);

        TestCase Update(TestCase testCase, string user);

        TestCase Delete(int idTestCase);

        TestCase Enable(int idTestCase);
        
        List<TestCase> GetAll();

        TestCase Get(int idTestCase);

        TestCase GetLastOne(int idReq, string creator, string date);

        TestCase GetLastTestCase(string creator, string date);

        TestCase UpdateNumber(TestCase testCase, int idReq);

        Project GetProject(int idtc);

        Requirement GetRequirement(int idtc);

        Requirement GetRequirementForTc(int tcId);

        Project GetProjectRequirement(int idreq);

        ChangeLog AddChangeLog(int testprocedureId, string user);

        List<ChangeLog> TestCaseChangeLogs(int id);

        ChangeLog Restore(ChangeLog change_log, string user);
    }

    //Class which implements ITestCaseService's methods and instance to ITestCaseRepository
    public class TestCaseService : ITestCaseService
    {
        //Instance of ITestCaseRepository
        private readonly ITestCaseRepository _testCaseRepository;
        private readonly ITestProcedureRepository _procedureRepository;
        private readonly ITestResultRepository _testResultRepository;

        //Constructor of TestCaseService and initialize testCaseRepository
        public TestCaseService(ITestCaseRepository testCaseRepository)
        {
            _testCaseRepository = testCaseRepository;
        }
        public TestCaseService(ITestCaseRepository testCaseRepository, ITestProcedureRepository procedureRepository, ITestResultRepository testResultRepository)
        {
            _testCaseRepository = testCaseRepository;
            _procedureRepository = procedureRepository;
            _testResultRepository = testResultRepository;
        }

        public ChangeLog AddChangeLog(int testprocedureId, string user)
        {
            return _testCaseRepository.AddChangeLog(testprocedureId, user);
        }

        //Delete method calls to Delete Method From testCaseRepository.
        public TestCase Delete(int idTestCase)
        {
            _procedureRepository.DeleteRelation(idTestCase);
            return _testCaseRepository.Delete(idTestCase);
        }

        public TestCase Enable(int idTestCase)
        {
            return _testCaseRepository.Enable(idTestCase);
        }

        //Get method calls to Get Method From testCaseRepository.
        public TestCase Get(int idTestCase)
        {
            return _testCaseRepository.Get(idTestCase);
        }
        //GetAll method calls to GetAll Method From testCaseRepository.
        public List<TestCase> GetAll()
        {
            return _testCaseRepository.GetAll();
        }

        public TestCase GetLastOne(int idReq, string creator, string date)
        {
            return _testCaseRepository.GetLastOne(idReq, creator, date);
        }

        public TestCase GetLastTestCase(string creator, string date)
        {
            return _testCaseRepository.GetLastTestCase(creator, date);
        }

        public Project GetProject(int idtc)
        {
            return _testCaseRepository.GetProject(idtc);
        }

        public Project GetProjectRequirement(int idreq)
        {
            return _testCaseRepository.GetProjectRequirement(idreq);
        }

        public Requirement GetRequirement(int idtc)
        {
            return _testCaseRepository.GetRequirement(idtc);
        }

        public Requirement GetRequirementForTc(int tcId)
        {
            return _testCaseRepository.GetRequirementForTc(tcId);
        }

        public ChangeLog Restore(ChangeLog change_log, string user)
        {
            return _testCaseRepository.Restore(change_log, user);
        }

        //Save method calls to Save Method From testCaseRepository.
        public TestCase Save(TestCase testCase, string user)
        {
            return _testCaseRepository.Save(testCase, user);
        }

        public List<ChangeLog> TestCaseChangeLogs(int id)
        {
            return _testCaseRepository.TestCaseChangeLogs(id);
        }

        //Update method calls to Update Method From testCaseRepository.
        public TestCase Update(TestCase testCase, string user)
        {
            _procedureRepository.UpdateFromTC(testCase);
            _testResultRepository.UpdateTitles(testCase.Test_Case_Id, testCase.Title, "TC");
            return _testCaseRepository.Update(testCase, user);
        }

        public TestCase UpdateNumber(TestCase testCase, int idReq)
        {
            return _testCaseRepository.UpdateNumber(testCase, idReq);
        }
    }
}
