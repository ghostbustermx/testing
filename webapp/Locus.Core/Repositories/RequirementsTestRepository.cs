using Locus.Core.Context;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface IRequirementsTestRepository
    {
        RequirementsTest Save(RequirementsTest rq);

        RequirementsTest Update(RequirementsTest rq);

        RequirementsTest Delete(int rq);

        List<RequirementsTest> GetAll();

        RequirementsTest Get(int rq);
        List<RequirementsTest> GetTestCaseRelations(int id);

        RequirementsTest DeleteTestCase(int reqId, int tcId);

        RequirementsTest DeleteTestProcedure(int reqId, int tpId);

        RequirementsTest DeleteTestScenario(int reqId, int tsId);

        List<RequirementsTest> GetTestScenarioRelations(int id);

        List<RequirementsTest> GetTestProcedureRelations(int id);
    }
    public class RequirementsTestRepository : IRequirementsTestRepository
    {
        private LocustDBContext context = new LocustDBContext();

        public RequirementsTest Delete(int rq)
        {
            try
            {
                var rt = context.RequirementsTests.Find(rq);
                context.RequirementsTests.Remove(rt);
                context.SaveChanges();
                return rt;
            }
            catch
            {
                return null;
            }
        }

        public RequirementsTest DeleteTestCase(int reqId, int tcId)
        {
            // read connectionstring from config file
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            var rel = context.RequirementsTests.Where(x => x.Requirement_Id == reqId && x.Test_Case_Id == tcId).First();

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("DELETE FROM RequirementsTest WHERE Requirement_Id = {0}  AND Test_Case_Id = {1}",
                        reqId, tcId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                //DeleteDisable();
                return rel;
            }
            catch
            {
                return null;
            }
        }

        public RequirementsTest DeleteTestProcedure(int reqId, int tpId)
        {
            // read connectionstring from config file
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            var rel = context.RequirementsTests.Where(x => x.Requirement_Id == reqId && x.Test_Procedure_Id == tpId).FirstOrDefault();
            Debug.WriteLine("req id" + reqId);
            Debug.WriteLine("tc id" + tpId);
            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("DELETE FROM RequirementsTest WHERE Requirement_Id = {0}  AND Test_Procedure_Id = {1}",
                        reqId, tpId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                //DeleteDisable();
                return rel;
            }
            catch
            {
                return null;
            }
        }

        public RequirementsTest DeleteTestScenario(int reqId, int tsId)
        {
            // read connectionstring from config file
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            var rel = context.RequirementsTests.Where(x => x.Requirement_Id == reqId && x.Test_Scenario_Id == tsId).First();

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("DELETE FROM RequirementsTest WHERE Requirement_Id = {0}  AND Test_Scenario_Id = {1}",
                        reqId, tsId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                //DeleteDisable();
                return rel;
            }
            catch
            {
                return null;
            }
        }

        public RequirementsTest Get(int rq)
        {
            try
            {
                return context.RequirementsTests.Find(rq);
            }
            catch
            {
                return null;
            }
        }

        public List<RequirementsTest> GetAll()
        {
            try
            {
                return context.RequirementsTests.ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<RequirementsTest> GetTestCaseRelations(int id)
        {
            try
            {
                return context.RequirementsTests.Where(x => x.Test_Case_Id == id).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<RequirementsTest> GetTestProcedureRelations(int id)
        {
            try
            {
                return context.RequirementsTests.Where(x => x.Test_Procedure_Id == id).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<RequirementsTest> GetTestScenarioRelations(int id)
        {
            try
            {
                return context.RequirementsTests.Where(x => x.Test_Scenario_Id == id).ToList();
            }
            catch
            {
                return null;
            }
        }

        public RequirementsTest Save(RequirementsTest rq)
        {
            try
            {
                context.RequirementsTests.Add(rq);
                context.SaveChanges();
                return rq;
            }
            catch
            {
                return null;
            }
        }

        

        public RequirementsTest Update(RequirementsTest rq)
        {
            try
            {
                context.Entry(rq).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return rq;
            }
            catch
            {
                return null;
            }
        }
    }
}
