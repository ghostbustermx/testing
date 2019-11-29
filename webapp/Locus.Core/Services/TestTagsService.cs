using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;

namespace Locus.Core.Services
{
    //Interface which contains methods for each CRUD operation
    public interface ITestTagsService
    {

        Test_Tags Save(Test_Tags test_tags);

        Test_Tags Update(Test_Tags test_tags);

        Test_Tags Delete(int idtt);

        List<Test_Tags> GetAll();

        Test_Tags Get(int idtt);
    }

    public class TestTagsService : ITestTagsService
    {
        
        private readonly ITestTagsRepository _ttRepository;


        public TestTagsService(ITestTagsRepository ttRepository)
        {
            _ttRepository = ttRepository;
        }

        public Test_Tags Delete(int idtt)
        {
            return _ttRepository.Delete(idtt);
        }

        public Test_Tags Get(int idtt)
        {
            return _ttRepository.Get(idtt);
        }

        public List<Test_Tags> GetAll()
        {
            return _ttRepository.GetAll();
        }

        public Test_Tags Save(Test_Tags test_tags)
        {
            return _ttRepository.Save(test_tags);
        }

        public Test_Tags Update(Test_Tags test_tags)
        {
            return _ttRepository.Update(test_tags);
        }
    }
}
