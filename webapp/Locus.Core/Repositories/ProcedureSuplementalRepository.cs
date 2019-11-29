using Locus.Core.Context;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Repositories
{
    public interface IProcedureSuplementalRepository
    {
        Test_Procedure_Test_Suplemental Save(Test_Procedure_Test_Suplemental tpts);

        Test_Procedure_Test_Suplemental Update(Test_Procedure_Test_Suplemental tpts);

        Test_Procedure_Test_Suplemental DeleteTP(int idtp, int idstp);

        Test_Procedure_Test_Suplemental DeleteTS(int idts, int idstp);

        List<Test_Procedure_Test_Suplemental> GetAll();

        Test_Procedure_Test_Suplemental GetTP(int idtp, int idstp);

        bool DesactiveSupplementalsTp(int idtp);

        bool DesactiveSupplementalsTs(int idts);

        bool ActivateSupplementalsTp(int idtp);

        bool ActivateSupplementalsTs(int idts);

        Test_Procedure_Test_Suplemental GetTS(int idts, int idstp);

        List<Test_Procedure_Test_Suplemental> GetForTP(int idtp);

        List<Test_Procedure_Test_Suplemental> GetForTS(int idts);

    }

    public class ProcedureSuplementalRepository : IProcedureSuplementalRepository
    {
        private LocustDBContext context = new LocustDBContext();

        public bool ActivateSupplementalsTp(int idtp)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            //var rel = context.test_procedure_test_suplemental.Where(x => x.Test_Scenario_Id == idts && x.Test_Suplemental_Id == idstp).First();

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("UPDATE [Locust-Test].[dbo].[Test_Procedure_Test_Suplemental] SET Status = 1 where Test_Procedure_Id = {0}",
                        idtp);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                //DeleteDisable();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ActivateSupplementalsTs(int idts)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            //var rel = context.test_procedure_test_suplemental.Where(x => x.Test_Scenario_Id == idts && x.Test_Suplemental_Id == idstp).First();

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("UPDATE [Locust-Test].[dbo].[Test_Procedure_Test_Suplemental] SET Status = 1 where Test_Scenario_Id = {0}",
                        idts);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                //DeleteDisable();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Test_Procedure_Test_Suplemental DeleteTP(int idtp, int idstp)
        {
            // read connectionstring from config file
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            var rel = context.test_procedure_test_suplemental.Where(x => x.Test_Procedure_Id == idtp && x.Test_Suplemental_Id == idstp).First();

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("DELETE FROM Test_Procedure_Test_Suplemental WHERE Test_Suplemental_Id = {0}  AND Test_Procedure_Id = {1}",
                        idstp, idtp);

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
        
        public Test_Procedure_Test_Suplemental DeleteTS(int idts, int idstp)
        {
            // read connectionstring from config file
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            var rel = context.test_procedure_test_suplemental.Where(x => x.Test_Scenario_Id == idts && x.Test_Suplemental_Id == idstp).First();

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("DELETE FROM Test_Procedure_Test_Suplemental WHERE Test_Suplemental_Id = {0}  AND Test_Scenario_Id = {1}",
                        idstp, idts);

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

        public bool DesactiveSupplementalsTp(int idtp)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            //var rel = context.test_procedure_test_suplemental.Where(x => x.Test_Scenario_Id == idts && x.Test_Suplemental_Id == idstp).First();

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("UPDATE [Locust-Test].[dbo].[Test_Procedure_Test_Suplemental] SET Status = 0 where Test_Procedure_Id = {0}",
                        idtp);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                //DeleteDisable();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DesactiveSupplementalsTs(int idts)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            //var rel = context.test_procedure_test_suplemental.Where(x => x.Test_Scenario_Id == idts && x.Test_Suplemental_Id == idstp).First();

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("UPDATE [Locust-Test].[dbo].[Test_Procedure_Test_Suplemental] SET Status = 0 where Test_Scenario_Id = {0}",
                        idts);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                //DeleteDisable();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Test_Procedure_Test_Suplemental> GetAll()
        {
            return context.test_procedure_test_suplemental.ToList();
        }

        public List<Test_Procedure_Test_Suplemental> GetForTP(int idtp)
        {
            try
            {
                return context.test_procedure_test_suplemental.Where(x => x.Test_Procedure_Id == idtp).ToList();
            }
            catch
            {
                return null;
            }
            
        }

        public List<Test_Procedure_Test_Suplemental> GetForTS(int idts)
        {
            try
            {
                return context.test_procedure_test_suplemental.Where(x => x.Test_Scenario_Id == idts).ToList();
            }
            catch
            {
                return null;
            }
        }

        public Test_Procedure_Test_Suplemental GetTP(int idtp, int idstp)
        {
            try
            {
                var relation = (from tpts in context.test_procedure_test_suplemental
                                where tpts.Test_Procedure_Id == idtp && tpts.Test_Suplemental_Id == idstp
                                select tpts).FirstOrDefault();
                return relation;
            }
            catch
            {
                return null;
            }
        }

        public Test_Procedure_Test_Suplemental GetTS(int idts, int idstp)
        {
            try
            {
                var relation = (from tpts in context.test_procedure_test_suplemental
                                where tpts.Test_Scenario_Id == idts && tpts.Test_Suplemental_Id == idstp
                                select tpts).FirstOrDefault();
                return relation;
            }
            catch
            {
                return null;
            }
        }

        public Test_Procedure_Test_Suplemental Save(Test_Procedure_Test_Suplemental tpts)
        {
            try
            {
                tpts.Status = true;
                context.test_procedure_test_suplemental.Add(tpts);
                context.SaveChanges();
                return tpts;
            }
            catch
            {
                return null;
            }
        }

        public Test_Procedure_Test_Suplemental Update(Test_Procedure_Test_Suplemental tpts)
        {
            try
            {
                context.Entry(tpts).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return tpts;
            }
            catch
            {
                return null;
            }
        }
    }
}
