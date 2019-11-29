using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Services
{
    public interface IRequirementsTestService
    {

        RequirementsTest Save(RequirementsTest rt);

        RequirementsTest Update(RequirementsTest rt);

        RequirementsTest Delete(int idrt);

        List<RequirementsTest> GetAll();

        RequirementsTest Get(int idrt);

        List<RequirementsTest> GetTestCaseRelations(int id);

        RequirementsTest DeleteTestCase(int reqId, int tcId);

        RequirementsTest DeleteTestProcedure(int reqId, int tpId);

        RequirementsTest DeleteTestScenario(int reqId, int tsId);
        List<RequirementsTest> GetTestProcedureRelations(int id);
        List<RequirementsTest> GetTestScenarioRelations(int id);
    }
    public class RequirementsTestService : IRequirementsTestService
    {
        private readonly IRequirementsTestRepository _rtRepository;

        public RequirementsTestService(IRequirementsTestRepository rtRepository)
        {
            _rtRepository = rtRepository;
        }

  
        public RequirementsTest Delete(int idrt)
        {
           return _rtRepository.Delete(idrt);
        }

        public RequirementsTest DeleteTestCase(int reqId, int tcId)
        {
            return _rtRepository.DeleteTestCase(reqId, tcId);
        }

        public RequirementsTest DeleteTestProcedure(int reqId, int tpId)
        {
            return _rtRepository.DeleteTestProcedure(reqId, tpId);
        }

        public RequirementsTest DeleteTestScenario(int reqId, int tsId)
        {
            return _rtRepository.DeleteTestScenario(reqId, tsId);
        }

        public RequirementsTest Get(int idrt)
        {
            return _rtRepository.Get(idrt);
        }

        public List<RequirementsTest> GetAll()
        {
            return _rtRepository.GetAll();
        }

        public List<RequirementsTest> GetTestCaseRelations(int id)
        {
            return _rtRepository.GetTestCaseRelations(id);
        }

        public List<RequirementsTest> GetTestProcedureRelations(int id)
        {
            return _rtRepository.GetTestProcedureRelations(id);
        }

        public List<RequirementsTest> GetTestScenarioRelations(int id)
        {
            return _rtRepository.GetTestScenarioRelations(id);
        }

        public RequirementsTest Save(RequirementsTest rt)
        {
            return _rtRepository.Save(rt);
        }

        public RequirementsTest Update(RequirementsTest rt)
        {
            return _rtRepository.Update(rt);
        }
    }
}
