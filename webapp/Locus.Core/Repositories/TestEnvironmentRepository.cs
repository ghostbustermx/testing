using Locus.Core.Context;
using Locus.Core.DTO;
using Locus.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//Repository for project operations.
namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface ITestEnvironmentRepository
    {
        TestEnvironment Save(TestEnvironment project);

        TestEnvironment Update(TestEnvironment project);

        TestEnvironment Get(int id);

        List<TestEnvironment> GetActives(int id);

        List<TestEnvironment> GetInactives(int id);

        List<ExecutionGroup> HasRelationships(int id);


    }
    //Class which implements IProjectRepository's methods and use DBContext for apply operations.
    public class TestEnvironmentRepository : ITestEnvironmentRepository
    {
        private LocustDBContext context = new LocustDBContext();

        public TestEnvironment Get(int id)
        {
            try
            {
                return context.TestEnvironment.Find(id);
            }
            catch
            {
                return null;
            }

        }

        public List<TestEnvironment> GetActives(int id)
        {
            return context.TestEnvironment.Where(x => x.IsActive == true && x.ProjectId == id).ToList();
        }

        public List<TestEnvironment> GetInactives(int id)
        {
            return context.TestEnvironment.Where(x => x.IsActive == false && x.ProjectId == id).ToList();
        }



        //Method to save a new project in database.
        public TestEnvironment Save(TestEnvironment te)
        {
            context.TestEnvironment.Add(te);
            context.SaveChanges();
            return te;
        }

        public List<ExecutionGroup> HasRelationships(int id)
        {
            var ExecutionGroups = context.ExecutionGroups.Where(x => x.TestEnvironmentId == id).ToList();
            return ExecutionGroups;
        }

        public TestEnvironment Update(TestEnvironment te)
        {
            try
            {
                context.Entry(te).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return te;
            }
            catch
            {
                return null;
            }
        }



    }
}
