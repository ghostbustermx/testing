using Locus.Core.Context;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface ITestTagsRepository
    {
        Test_Tags Save(Test_Tags test_tags);

        Test_Tags Update(Test_Tags test_tags);

        Test_Tags Delete(int idtt);

        List<Test_Tags> GetAll();

        Test_Tags Get(int idtt);

    }
    public class TestTagsRepository : ITestTagsRepository
    {
        LocustDBContext context = new LocustDBContext();

        public Test_Tags Delete(int idtt)
        {
            try
            {
                var tt = context.test_tags.Find(idtt);
                context.test_tags.Remove(tt);
                context.SaveChanges();
                return tt;

            }
            catch
            {
                return null;
            }
        }

        public Test_Tags Get(int idtt)
        {
            try
            {
                return context.test_tags.Find(idtt);
            }
            catch
            {
                return null;
            }
        }

        public List<Test_Tags> GetAll()
        {
            try
            {
                return context.test_tags.ToList();
            }
            catch
            {
                return null;
            }
        }

        public Test_Tags Save(Test_Tags test_tags)
        {
            try
            {
                context.test_tags.Add(test_tags);
                context.SaveChanges();
                return test_tags;
            }
            catch
            {
                return null;
            }
        }

        public Test_Tags Update(Test_Tags test_tags)
        {
            try
            {
                context.Entry(test_tags).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return test_tags;
            }
            catch
            {
                return null;
            }
        }
    }
}
