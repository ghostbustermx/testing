using Locus.Core.DTO;
using Locus.Core.Helpers;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Locus.Core.Services
{
    public interface ITestEnvironmentService
    {
        TestEnvironment Save(TestEnvironment project);

        TestEnvironment Update(TestEnvironment project);

        TestEnvironment Get(int id);

        List<TestEnvironment> GetActives(int id);

        List<TestEnvironment> GetInactives(int id);

        List<ExecutionGroup> HasRelationships(int id);
    }

    public class TestEnvironmentService : ITestEnvironmentService
    {
        private readonly ITestEnvironmentRepository _testEnvironmentRepository;

        public TestEnvironmentService(ITestEnvironmentRepository testEnvironmentRepository)
        {
            _testEnvironmentRepository = testEnvironmentRepository;
        }

        public TestEnvironment Get(int id)
        {
            return _testEnvironmentRepository.Get(id);
        }

        public List<TestEnvironment> GetActives(int id)
        {
            return _testEnvironmentRepository.GetActives(id);
        }

        public List<TestEnvironment> GetInactives(int id)
        {
            return _testEnvironmentRepository.GetInactives(id);
        }

        public TestEnvironment Save(TestEnvironment te)
        {
            return _testEnvironmentRepository.Save(te);
        }

        public TestEnvironment Update(TestEnvironment te)
        {
            return _testEnvironmentRepository.Update(te);
        }


        public List<ExecutionGroup> HasRelationships(int id)
        {
            return _testEnvironmentRepository.HasRelationships(id);

        }
    }
}
