using Locus.Core.Context;
using Locus.Core.DTO;
using Locus.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;


//Repository for Requirement operations.
namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface IRequirementRepository
    {
        Requirement Save(Requirement requirement, string user);

        Requirement Update(Requirement requirement, string user);

        Requirement Delete(int idRequirement, string user);

        List<Requirement> GetAll();

        List<TestCase> GetAllTestCase(int idReq);

        List<TestCase> GetAllTestCaseInactives(int idReq);

        Requirement Get(int idRequirement);

        List<Requirement> GetProject(int Project_Id);

        List<Requirement> GetProjectInactives(int Project_Id);

        List<TestScenario> GetAllTestScenario(int idReq);

        List<TestScenario> GetAllTestScenarioInactives(int idReq);

        List<TestProcedure> GetAllTestProcedure(int id);

        List<TestProcedure> GetAllTestProcedureInactives(int id);

        List<TestProcedure> GetAllTestProcedureAutomated(int id);

        List<TestProcedure> GetAllTestProcedureInactivesAutomated(int id);

        Requirement GetByReqNumber(int projectId, string reqnumber);

        List<TestProcedure> GetAllTestProcedureByArray(RequirementDTO[] data);

        List<TestDTO> GetTestCases(int projectId);

        List<TestDTO> GetTestProcedures(int projectId);

        List<TestDTO> GetTestScenarios(int projectId);

        Requirement Enable(int id, string user);

        List<ChangeLog> RequirementChangeLogs(int id);

        ChangeLog Restore(Requirement requirement, int version, string user);

        List<TestProcedure> GetAllTestProcedureNoTc(int id);

        List<Requirement> GetAllReqWithoutEvidence(int projectId);

        List<TestCase> GetAllTCWithoutTP(int projectId);

        List<TestProcedure> GetAllTPWithoutTC(int projectId);

        Requirement GetRequirementbyTCId(int tcId);

        Requirement GetRequirementbyTPId(int tpId);

        List<TestCase> GetAllTCWithoutTPByReq(int reqId);

        List<EvidenceDTO> GetEvidenceFromReq(int[] requirements);

        List<Requirement> GetRequirementsByTestEvidence(int id, string type);

        List<Requirement> GetTCDisables(int id);
        List<Requirement> GetTPDisables(int id);
        List<Requirement> GetTSDisables(int id);

        int GetCountTCTotal(int reqId);
        int GetCountTC(int reqId);

        int GetCountTPTotal(int reqId);
        int GetCountTP(int reqId);

        int GetCountTSTotal(int reqId);
        int GetCountTS(int reqId);
        List<EvidenceDTO> GetAutomatedEvidenceFromReq(int[] ids);

        List<string> GetSprints(int projectId);
        List<Requirement> GetNonActivesExecutedForTP(int executionId);
        List<Requirement> GetNonActivesExecutedForTC(int executionId);
        List<Requirement> GetNonActivesExecutedForTS(int executionId);
        List<TestCase> GetUniqueTestCases(int ProjectId);
        List<TestProcedure> GetUniqueTestProcedures(int ProjectId);
        List<TestScenario> GetUniqueTestScenarios(int ProjectId);
        List<TestDTO> GetTestCasesByNumber(string[] TcNumbers, int projectId, int ScriptId);
        List<TestDTO> GetUniqueTestCasesDTO(int projectId);
    }


    //Class which implements IRequirementRepository's methods and use DBContext for apply operations.
    public class RequirementRepository : IRequirementRepository
    {
        //Instance of Database Context
        LocustDBContext context = new LocustDBContext();

        //Method to delete a requirement from the list of requirements in database.
        public Requirement Delete(int idRequirement, string user)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.Requirements.Find(idRequirement);
                req.Status = false;
                newContext.Entry(req).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                var changeLog = AddChangeLog(req, user);
                AddTestChangeLog(changeLog, req);
                return req;
            }
            catch
            {
                return null;
            }
        }

        private ChangeLog AddChangeLog(Requirement requirement, string user)
        {
            var req = context.Requirements.Where(x => x.Name == requirement.Name &&
                                          x.Status == requirement.Status &&
                                          x.Project_Id == requirement.Project_Id &&
                                          x.Description == requirement.Description &&
                                          x.Release == requirement.Release &&
                                          x.req_number == requirement.req_number &&
                                          x.Acceptance_Criteria == requirement.Acceptance_Criteria &&
                                          x.Axosoft_Task_Id == requirement.Axosoft_Task_Id &&
                                          x.Developer_Assigned == requirement.Developer_Assigned &&
                                          x.Tester_Assigned == requirement.Tester_Assigned).FirstOrDefault();

            var active = (from ch in context.ChangeLogs
                          join tcl in context.Test_ChangeLogs on ch.Id equals tcl.Change_Log_Id
                          where tcl.Requirement_Id == req.Id && ch.Active == true
                          select ch).FirstOrDefault();
            if (active != null)
            {
                active.Active = false;
                context.Entry(active).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            String reqString = JsonConvert.SerializeObject(req);
            ChangeLog cl = new ChangeLog();
            cl.Content = reqString;
            cl.User = user;

            cl.Date = DateTime.UtcNow;
            cl.Active = true;
            var last = context.Test_ChangeLogs.Where(x => x.Requirement_Id == req.Id).OrderByDescending(y => y.Change_Log_Id).FirstOrDefault();
            var lastVersion = 0;
            if (last != null)
            {
                lastVersion = context.ChangeLogs.Find(last.Change_Log_Id).Version;
            }
            cl.Version = lastVersion + 1;
            context.ChangeLogs.Add(cl);
            context.SaveChanges();
            return cl;
        }

        private Test_ChangeLog AddTestChangeLog(ChangeLog changeLog, Requirement req)
        {
            var cl = context.ChangeLogs.Where(x => x.Content == changeLog.Content &&
                                          x.Date == changeLog.Date &&
                                          x.User == changeLog.User &&
                                          x.Version == changeLog.Version).FirstOrDefault();

            Test_ChangeLog tcl = new Test_ChangeLog();
            tcl.Requirement_Id = req.Id;
            tcl.Change_Log_Id = changeLog.Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return tcl;

        }

        public Requirement Enable(int id, string user)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.Requirements.Find(id);
                req.Status = true;
                newContext.Entry(req).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                var changeLog = AddChangeLog(req, user);
                AddTestChangeLog(changeLog, req);


                return req;
            }
            catch
            {
                return null;
            }
        }

        //Method to get a requirement from the database which id coincide with the parameter
        public Requirement Get(int idRequirement)
        {
            try
            {
                return context.Requirements.Find(idRequirement);
            }
            catch
            {
                return null;
            }

        }

        //Method to obtain all of the requirements from database.
        public List<Requirement> GetAll()
        {
            try
            {
                return context.Requirements.Where(x => x.Status == true).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<TestCase> GetAllTestCase(int idReq)
        {
            try
            {
                var testCases = context.TestCases
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == idReq).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == true)
                    .ToList();
                return testCases;
            }
            catch
            {
                return null;
            }
        }

        public List<TestCase> GetAllTestCaseInactives(int idReq)
        {
            try
            {
                var testCases = context.TestCases
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == idReq).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == false)
                    .ToList();
                return testCases;
            }
            catch
            {
                return null;
            }
        }

        public List<TestProcedure> GetAllTestProcedure(int id)
        {
            try
            {
                var testProcedures = context.TestProcedures
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == id).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == true && x.Script_Id == null)
                    .ToList();
                return testProcedures;
            }
            catch
            {
                return null;
            }
        }



        public List<TestProcedure> GetAllTestProcedureNoTc(int id)
        {
            try
            {
                var testProcedures = context.TestProcedures
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == id).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == true && x.Test_Case_Id == null)
                    .ToList();
                return testProcedures;
            }
            catch
            {
                return null;
            }
        }

        public List<TestProcedure> GetAllTPWithoutTC(int projectId)
        {
            List<TestProcedure> testprocedures = new List<TestProcedure>();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT * FROM TestProcedure AS tp LEFT JOIN RequirementsTest AS rt ON tp.Test_Procedure_Id=rt.Test_Procedure_Id LEFT JOIN requirement AS req ON req.Id=rt.Requirement_Id WHERE tp.Test_Case_Id IS NULL AND req.Status=1 AND req.Project_Id={0} AND tp.Status=1"
                        , projectId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TestProcedure tp = new TestProcedure()
                                {
                                    Test_Procedure_Id = reader.GetInt32(0),
                                    tp_number = reader.GetString(1),
                                    Test_Priority = reader.GetString(3),
                                    Title = reader.GetString(4),
                                    Description = reader.GetString(5),
                                    Test_Procedure_Creator = reader.GetString(6),
                                    Expected_Result = reader.GetString(7),
                                    Creation_Date = reader.GetDateTime(8),
                                    Status = reader.GetBoolean(9),
                                };
                                testprocedures.Add(tp);
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return testprocedures;
        }


        private List<Requirement> ExecuteQuery(string query)
        {
            List<Requirement> requirements = new List<Requirement>();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Requirement requirement = new Requirement()
                                {
                                    Id = reader.GetInt32(0),
                                    Project_Id = reader.GetInt32(1),
                                    Name = reader.GetString(2),
                                    Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Developer_Assigned = reader.GetString(4),
                                    Tester_Assigned = reader.GetString(5),
                                    Axosoft_Task_Id = reader.GetInt32(6),
                                    Status = reader.GetBoolean(7),
                                    req_number = reader.GetString(8),
                                    Acceptance_Criteria = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                    Release = reader.GetString(10),
                                };
                                requirements.Add(requirement);
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return requirements;
        }


        private int ExecuteQueryTotal(string query)
        {
            int total = 0;
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            total = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch
            {
                return 0;
            }
            return total;
        }

        public List<Requirement> GetAllReqWithoutEvidence(int projectId)
        {
            var query = String.Format("SELECT * FROM requirement AS req LEFT JOIN RequirementsTest AS rt ON req.Id=rt.Requirement_Id WHERE req.Project_Id={0} AND rt.Requirement_Id IS NULL AND Status=1", projectId);
            return this.ExecuteQuery(query);
        }

        public List<Requirement> GetTCDisables(int projectId)
        {
            var query = String.Format("SELECT * FROM requirement AS req JOIN RequirementsTest AS rt  ON req.Id=rt.Requirement_Id  JOIN TestCase AS tc ON tc.Test_Case_Id=rt.Test_Case_Id AND tc.Status=0 where req.Project_Id={0} AND req.[Status]=1", projectId);
            return this.ExecuteQuery(query);
        }

        public List<Requirement> GetTPDisables(int projectId)
        {
            var query = String.Format("SELECT * FROM requirement AS req JOIN RequirementsTest AS rt  ON req.Id=rt.Requirement_Id  JOIN TestProcedure AS tp ON tp.Test_Procedure_Id=rt.Test_Procedure_Id AND tp.Status=0 where req.Project_Id={0} AND req.[Status]=1", projectId);
            return this.ExecuteQuery(query);
        }

        public List<Requirement> GetTSDisables(int projectId)
        {
            var query = String.Format("SELECT * FROM requirement AS req JOIN RequirementsTest AS rt  ON req.Id=rt.Requirement_Id  JOIN TestScenario AS ts ON ts.Test_Scenario_Id=rt.Test_Scenario_Id AND ts.Status=0 where req.Project_Id={0} AND req.[Status]=1", projectId);
            return this.ExecuteQuery(query);
        }


        public int GetCountTCTotal(int reqId)
        {
            var query = String.Format("SELECT COUNT(rt.Test_Case_Id) as total FROM RequirementsTest as rt where rt.Requirement_Id={0} and rt.Test_Case_Id is not null;", reqId);
            return this.ExecuteQueryTotal(query);
        }
        public int GetCountTC(int reqId)
        {
            var query = String.Format("SELECT COUNT(tc.Test_Case_Id) as total FROM RequirementsTest as rt  join TestCase as tc on rt.Test_Case_Id=tc.Test_Case_Id AND tc.Status=0  where rt.Requirement_Id={0} and rt.Test_Case_Id is not null;", reqId);
            return this.ExecuteQueryTotal(query);
        }

        public int GetCountTPTotal(int reqId)
        {
            var query = String.Format("SELECT COUNT(rt.Test_Procedure_Id) as total FROM RequirementsTest as rt where rt.Requirement_Id={0} and rt.Test_Procedure_Id is not null;", reqId);
            return this.ExecuteQueryTotal(query);
        }

        public int GetCountTP(int reqId)
        {
            var query = String.Format("SELECT COUNT(tp.Test_Procedure_Id) as total FROM RequirementsTest as rt  join TestProcedure as tp on rt.Test_Procedure_Id=tp.Test_Procedure_Id AND tp.Status=0  where rt.Requirement_Id={0} and rt.Test_Procedure_Id is not null;", reqId);
            return this.ExecuteQueryTotal(query);
        }

        public int GetCountTSTotal(int reqId)
        {
            var query = String.Format("SELECT COUNT(rt.Test_Scenario_Id) as total FROM RequirementsTest as rt where rt.Requirement_Id={0} and rt.Test_Scenario_Id is not null;", reqId);
            return this.ExecuteQueryTotal(query);
        }

        public int GetCountTS(int reqId)
        {
            var query = String.Format("SELECT COUNT(ts.Test_Scenario_Id) as total FROM RequirementsTest as rt  join TestScenario as ts on rt.Test_Scenario_Id=ts.Test_Scenario_Id AND ts.Status=0  where rt.Requirement_Id={0} and rt.Test_Scenario_Id is not null;", reqId);
            return this.ExecuteQueryTotal(query);
        }


        public List<TestCase> GetAllTCWithoutTP(int projectId)
        {
            List<TestCase> testcases = new List<TestCase>();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {

                    var query = String.Format("SELECT * FROM TestCase AS tc LEFT JOIN  TestProcedure as tp ON tc.Test_Case_Id=tp.Test_Case_Id LEFT JOIN RequirementsTest AS rt ON tc.Test_Case_Id=rt.Test_Case_Id  LEFT JOIN requirement as req ON req.Id=rt.Requirement_Id WHERE tp.Test_Case_Id IS NULL AND req.Status=1 AND req.Project_Id={0} AND tc.Status=1",
                    projectId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TestCase tc = new TestCase()
                                {
                                    Test_Case_Id = reader.GetInt32(0),
                                    tc_number = reader.GetString(1),
                                    Test_Priority = reader.GetString(2),
                                    Title = reader.GetString(3),
                                    Description = reader.GetString(4),
                                    Preconditions = reader.GetString(5),
                                    Test_Case_Creator = reader.GetString(6),
                                    Expected_Result = reader.GetString(7),
                                    Creation_Date = reader.GetDateTime(8),
                                    Status = reader.GetBoolean(9),
                                };
                                testcases.Add(tc);
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return testcases;
        }


        public List<TestCase> GetAllTCWithoutTPByReq(int reqId)
        {
            List<TestCase> testcases = new List<TestCase>();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {

                    var query = String.Format("SELECT * FROM TestCase AS tc LEFT JOIN  TestProcedure as tp ON tc.Test_Case_Id=tp.Test_Case_Id LEFT JOIN RequirementsTest AS rt ON tc.Test_Case_Id=rt.Test_Case_Id  LEFT JOIN requirement as req ON req.Id=rt.Requirement_Id WHERE tp.Test_Case_Id IS NULL AND req.Status=1 AND req.Id={0} AND tc.Status=1",
                    reqId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TestCase tc = new TestCase()
                                {
                                    Test_Case_Id = reader.GetInt32(0),
                                    tc_number = reader.GetString(1),
                                    Test_Priority = reader.GetString(2),
                                    Title = reader.GetString(3),
                                    Description = reader.GetString(4),
                                    Preconditions = reader.GetString(5),
                                    Test_Case_Creator = reader.GetString(6),
                                    Expected_Result = reader.GetString(7),
                                    Creation_Date = reader.GetDateTime(8),
                                    Status = reader.GetBoolean(9),
                                };
                                testcases.Add(tc);
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return testcases;
        }





        public List<TestProcedure> GetAllTestProcedureInactives(int id)
        {
            try
            {
                var testProcedures = context.TestProcedures
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == id).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == false)
                    .ToList();
                return testProcedures;
            }
            catch
            {
                return null;
            }
        }

        public List<TestScenario> GetAllTestScenario(int idReq)
        {
            try
            {
                var testScenarios = context.TestScenarios
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == idReq).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == true)
                    .ToList();
                return testScenarios;
            }
            catch
            {
                return null;
            }
        }

        public List<TestScenario> GetAllTestScenarioInactives(int idReq)
        {
            try
            {
                var testScenarios = context.TestScenarios
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == idReq).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == false)
                    .ToList();
                return testScenarios;
            }
            catch
            {
                return null;
            }
        }

        public List<Requirement> GetProject(int Project_Id)
        {
            return context.Requirements.Where(d => d.Project_Id == Project_Id).Where(x => x.Status == true).ToList();
        }

        public List<Requirement> GetProjectInactives(int Project_Id)
        {
            return context.Requirements.Where(d => d.Project_Id == Project_Id).Where(x => x.Status == false).ToList();
        }
        //Method to save a new requirement in database.
        public Requirement Save(Requirement requirement, string user)
        {
            LocustDBContext newContext = new LocustDBContext();
            try
            {
                if (requirement.Axosoft_Task_Id == 0)
                {
                    var lastReq =
                      (from req in newContext.Requirements
                       where req.Project_Id == requirement.Project_Id
                       select req
                       ).OrderByDescending(x => x.Id).FirstOrDefault();

                    var reqnumber = 0;
                    if (lastReq == null)
                    {
                        reqnumber = 0;
                    }
                    else
                    {
                        reqnumber = Convert.ToInt32(lastReq.req_number.Substring(3));
                    }

                    requirement.req_number = "RQ_" + Convert.ToString(reqnumber + 1);
                    requirement.Status = true;
                    context.Requirements.Add(requirement);
                    context.SaveChanges();

                    var req1 = context.Requirements.Where(x => x.Name == requirement.Name &&
                                                  x.Status == requirement.Status &&
                                                  x.Project_Id == requirement.Project_Id &&
                                                  x.Description == requirement.Description &&
                                                  x.Release == requirement.Release &&
                                                  x.req_number == requirement.req_number &&
                                                  x.Acceptance_Criteria == requirement.Acceptance_Criteria &&
                                                  x.Axosoft_Task_Id == requirement.Axosoft_Task_Id &&
                                                  x.Developer_Assigned == requirement.Developer_Assigned &&
                                                  x.Tester_Assigned == requirement.Tester_Assigned).FirstOrDefault();

                    var changeLog = AddChangeLog(req1, user);
                    AddTestChangeLog(changeLog, req1);
                    return requirement;
                }
                else
                {
                    if (ValidateAxosoftId(requirement))
                    {
                        var lastReq =
                     (from req in newContext.Requirements
                      where req.Project_Id == requirement.Project_Id
                      select req
                      ).OrderByDescending(x => x.Id).FirstOrDefault();

                        var reqnumber = 0;
                        if (lastReq == null)
                        {
                            reqnumber = 0;
                        }
                        else
                        {
                            reqnumber = Convert.ToInt32(lastReq.req_number.Substring(3));
                        }

                        requirement.req_number = "RQ_" + Convert.ToString(reqnumber + 1);
                        requirement.Status = true;
                        context.Requirements.Add(requirement);
                        context.SaveChanges();

                        var req1 = context.Requirements.Where(x => x.Name == requirement.Name &&
                                                      x.Status == requirement.Status &&
                                                      x.Project_Id == requirement.Project_Id &&
                                                      x.Description == requirement.Description &&
                                                      x.Release == requirement.Release &&
                                                      x.req_number == requirement.req_number &&
                                                      x.Acceptance_Criteria == requirement.Acceptance_Criteria &&
                                                      x.Axosoft_Task_Id == requirement.Axosoft_Task_Id &&
                                                      x.Developer_Assigned == requirement.Developer_Assigned &&
                                                      x.Tester_Assigned == requirement.Tester_Assigned).FirstOrDefault();

                        var changeLog = AddChangeLog(req1, user);
                        AddTestChangeLog(changeLog, req1);
                        return requirement;
                    }
                    else
                    {
                        requirement.Name = "repeated" + requirement.Axosoft_Task_Id;
                        return requirement;
                    }
                }





            }
            catch
            {
                return null;
            }

        }
        //Method to update a requirement from the database.

        public bool ValidateAxosoftId(Requirement requirement)
        {
            var repeatAxosoftId = context.Requirements.Where(x => x.Axosoft_Task_Id == requirement.Axosoft_Task_Id).FirstOrDefault();
            if (repeatAxosoftId != null)
            {
                requirement.Name = "repeated" + requirement.Axosoft_Task_Id;
                return false;
            }
            else
            {
                return true;
            }
        }


        public Requirement Update(Requirement requirement, string user)
        {
            try
            {
                var repeatAxosoftId = context.Requirements.Where(x => x.Axosoft_Task_Id == requirement.Axosoft_Task_Id && x.Id != requirement.Id).FirstOrDefault();
                if (repeatAxosoftId != null)
                {
                    requirement.Name = "repeated" + requirement.Axosoft_Task_Id;
                    return requirement;
                }
                else
                {
                    var newContext = new LocustDBContext();
                    requirement.Status = true;
                    newContext.Entry(requirement).State = System.Data.Entity.EntityState.Modified;
                    newContext.SaveChanges();

                    var changeLog = AddChangeLog(requirement, user);
                    AddTestChangeLog(changeLog, requirement);

                    return requirement;
                }

            }
            catch
            {
                return null;
            }
        }

        public List<ChangeLog> RequirementChangeLogs(int id)
        {
            try
            {
                return (from cl in context.ChangeLogs
                        join tcl in context.Test_ChangeLogs on cl.Id equals tcl.Change_Log_Id
                        where tcl.Requirement_Id == id
                        select cl).OrderByDescending(x => x.Version).ToList();
            }
            catch
            {
                return null;
            }
        }

        public ChangeLog Restore(Requirement requirement, int version, string user)
        {
            try
            {
                context.Entry(requirement).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                var cl = (from changeLog in context.ChangeLogs
                          join testCL in context.Test_ChangeLogs on changeLog.Id equals testCL.Change_Log_Id
                          where testCL.Requirement_Id == requirement.Id && changeLog.Active == true
                          select changeLog).FirstOrDefault();
                cl.Active = false;
                context.Entry(cl).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();


                var change = AddChangeLog(requirement, user);
                AddTestChangeLog(change, requirement);
                return change;

            }
            catch
            {
                return null;
            }
        }

        public Requirement GetRequirementbyTCId(int tcId)
        {
            Requirement requirement = new Requirement();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT * FROM requirement AS req LEFT JOIN RequirementsTest AS rt ON req.Id=rt.Requirement_Id WHERE rt.Test_Case_Id ={0} AND Status=1",
                    tcId);
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                requirement = new Requirement()
                                {
                                    Id = reader.GetInt32(0),
                                    Project_Id = reader.GetInt32(1),
                                    Name = reader.GetString(2),
                                    Description = reader.GetString(3),
                                    Developer_Assigned = reader.GetString(4),
                                    Tester_Assigned = reader.GetString(5),
                                    Axosoft_Task_Id = reader.GetInt32(6),
                                    Status = reader.GetBoolean(7),
                                    req_number = reader.GetString(8),
                                    Acceptance_Criteria = reader.GetString(9),
                                    Release = reader.GetString(10),
                                };

                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return requirement;
        }

        public Requirement GetRequirementbyTPId(int tpId)
        {
            Requirement requirement = new Requirement();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT * FROM requirement AS req LEFT JOIN RequirementsTest AS rt ON req.Id=rt.Requirement_Id WHERE rt.Test_Procedure_Id ={0} AND Status=1",
                    tpId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                requirement = new Requirement()
                                {
                                    Id = reader.GetInt32(0),
                                    Project_Id = reader.GetInt32(1),
                                    Name = reader.GetString(2),
                                    Description = reader.GetString(3),
                                    Developer_Assigned = reader.GetString(4),
                                    Tester_Assigned = reader.GetString(5),
                                    Axosoft_Task_Id = reader.GetInt32(6),
                                    Status = reader.GetBoolean(7),
                                    req_number = reader.GetString(8),
                                    Acceptance_Criteria = reader.GetString(9),
                                    Release = reader.GetString(10),
                                };

                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return requirement;
        }

        public List<TestProcedure> GetAllTestProcedureByArray(RequirementDTO[] data)
        {
            List<TestProcedure> testProceduresList = new List<TestProcedure>();
            foreach (var item in data)
            {
                try
                {
                    var testProcedures = context.TestProcedures
                        .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == item.Id).Count() > 0)
                        .Include(r => r.RequirementsTests)
                        .Where(x => x.Status == true)
                        .ToList();


                    testProceduresList.AddRange(testProcedures);
                }
                catch
                {
                    return null;
                }



            }
            return testProceduresList;
        }

        public List<EvidenceDTO> GetEvidenceFromReq(int[] requirements)
        {
            List<EvidenceDTO> evidence = new List<EvidenceDTO>();
            foreach (var req in requirements)
            {
                List<TestCase> testcases = GetAllTCWithoutTPByReq(req);
                foreach (var tc in testcases)
                {
                    evidence.Add(new EvidenceDTO() { ReqId = req, Element_id = tc.Test_Case_Id, Identifier_number = tc.tc_number, Title = tc.Title, Description = tc.Description, Priority = tc.Test_Priority, Type = tc.Type, Evidence = "TC" });
                }
            }

            foreach (var req in requirements)
            {
                List<TestProcedure> testProcedures = GetAllTestProcedure(req);
                foreach (var tp in testProcedures)
                {

                    evidence.Add(new EvidenceDTO() { ReqId = req, Element_id = tp.Test_Procedure_Id, Identifier_number = tp.tp_number, Title = tp.Title, Description = tp.Description, Priority = tp.Test_Priority, Type = tp.Type, Evidence = "TP" });

                }
            }

            foreach (var req in requirements)
            {
                List<TestScenario> testScenario = GetAllTestScenario(req);
                foreach (var ts in testScenario)
                {
                    evidence.Add(new EvidenceDTO() { ReqId = req, Element_id = ts.Test_Scenario_Id, Identifier_number = ts.ts_number, Title = ts.Title, Description = ts.Description, Priority = ts.Test_Priority, Type = ts.Type, Evidence = "TS" });
                }
            }


            return evidence;
        }
        public List<Requirement> GetRequirementsByTestEvidence(int id, string type)
        {
            List<Requirement> list = new List<Requirement>();
            List<RequirementsTest> requirements = null;
            switch (type)
            {
                case "TC":
                    {
                        requirements = context.RequirementsTests.Where(x => x.Test_Case_Id == id).ToList();
                        break;
                    }
                case "TP":
                    {
                        requirements = context.RequirementsTests.Where(x => x.Test_Procedure_Id == id).ToList();
                        break;
                    }
                case "TS":
                    {
                        requirements = context.RequirementsTests.Where(x => x.Test_Scenario_Id == id).ToList();
                        break;
                    }
            }
            foreach (var r in requirements)
            {
                var req = context.Requirements.Find(r.Requirement_Id);
                if (req != null)
                {
                    list.Add(req);
                }
            }
            return list;
        }

        public List<TestDTO> GetTestCases(int projectId)
        {
            TestDTO testDTO = new TestDTO();
            List<TestDTO> list = new List<TestDTO>();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT tc.*, tcreq.Requirement_Id, reqnumber =(Select req.req_number FROM Requirement as req WHERE req.Project_Id ={0} AND Requirement_Id = req.Id), reqStatus = (Select req.Status FROM Requirement as req WHERE req.Project_Id ={0} AND Requirement_Id = req.Id ) FROM TestCase as tc left join RequirementsTest as tcreq on tc.Test_Case_Id = tcreq.Test_Case_Id where tcreq.Requirement_Id IN (Select req.Id FROM Requirement as req WHERE req.Project_Id = {0}) AND tc.Status = 1",
                    projectId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                testDTO = new TestDTO();

                                testDTO.Test_Id = reader.GetInt32(0);
                                testDTO.IdentifiedNumber = reader.GetString(1);
                                testDTO.Priority = reader.GetString(2);
                                testDTO.Title = reader.GetString(3);
                                testDTO.Description = reader.GetString(4);
                                testDTO.Preconditions = reader.GetString(5);
                                testDTO.Creator = reader.GetString(6);
                                testDTO.ExpectedResult = reader.GetString(7);
                                testDTO.CreationDate = reader.GetDateTime(8);
                                testDTO.Status = reader.GetBoolean(9);
                                testDTO.LastEditor = reader.GetString(10);
                                testDTO.Type = reader.GetString(11);
                                testDTO.reqId = reader.GetInt32(12);
                                testDTO.reqNumber = reader.GetString(13);
                                testDTO.TestType = "TC";
                                testDTO.reqStatus = reader.GetBoolean(14);
                                list.Add(testDTO);

                            }
                        }
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        public List<TestDTO> GetTestProcedures(int projectId)
        {
            TestDTO testDTO = new TestDTO();
            List<TestDTO> list = new List<TestDTO>();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT tp.*, tcreq.Requirement_Id, reqnumber =(Select req.req_number FROM Requirement as req WHERE req.Project_Id = {0} AND Requirement_Id = req.Id) , reqStatus = (Select req.Status FROM Requirement as req WHERE req.Project_Id ={0} AND Requirement_Id = req.Id )FROM TestProcedure as tp left join RequirementsTest as tcreq on tp.Test_Procedure_Id = tcreq.Test_Procedure_Id where tcreq.Requirement_Id IN(Select req.Id  FROM Requirement as req WHERE req.Project_Id = {0} ) AND tp.status = 1",
                    projectId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                testDTO = new TestDTO();

                                testDTO.Test_Id = reader.GetInt32(0);
                                testDTO.IdentifiedNumber = reader.GetString(1);
                                testDTO.Related_Test_Id = reader.IsDBNull(2) ? 0 : (int)reader.GetInt32(2);
                                testDTO.Priority = reader.GetString(3);
                                testDTO.Title = reader.GetString(4);
                                testDTO.Description = reader.GetString(5);
                                testDTO.Creator = reader.GetString(6);
                                testDTO.ExpectedResult = reader.GetString(7);
                                testDTO.CreationDate = reader.GetDateTime(8);
                                testDTO.Status = reader.GetBoolean(9);
                                testDTO.LastEditor = reader.GetString(10);
                                testDTO.Type = reader.GetString(11);
                                testDTO.reqId = reader.GetInt32(12);
                                testDTO.reqNumber = reader.GetString(13);
                                testDTO.reqStatus = reader.GetBoolean(14);
                                testDTO.TestType = "TP";
                                list.Add(testDTO);

                            }
                        }
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        public List<TestDTO> GetTestScenarios(int projectId)
        {
            TestDTO testDTO = new TestDTO();
            List<TestDTO> list = new List<TestDTO>();
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT ts.*, tcreq.Requirement_Id, reqnumber =(Select req.req_number FROM Requirement as req  WHERE req.Project_Id = {0} AND Requirement_Id = req.Id), reqStatus = (Select req.Status FROM Requirement as req WHERE req.Project_Id ={0} AND Requirement_Id = req.Id ) FROM TestScenario as ts  left join RequirementsTest as tcreq on ts.Test_Scenario_Id = tcreq.Test_Scenario_Id  where tcreq.Requirement_Id IN(Select req.Id FROM Requirement as req WHERE req.Project_Id = {0} ) and status = 1",
                    projectId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                testDTO = new TestDTO();

                                testDTO.Test_Id = reader.GetInt32(0);
                                testDTO.IdentifiedNumber = reader.GetString(1);
                                testDTO.Priority = reader.GetString(2);
                                testDTO.Title = reader.GetString(3);
                                testDTO.Description = reader.IsDBNull(4) ? null : reader.GetString(4);
                                testDTO.Preconditions = reader.IsDBNull(5) ? null : reader.GetString(5);
                                testDTO.Note = reader.IsDBNull(6) ? null : reader.GetString(6);
                                testDTO.Creator = reader.GetString(7);
                                testDTO.CreationDate = reader.GetDateTime(8);
                                testDTO.Status = reader.GetBoolean(9);
                                testDTO.LastEditor = reader.GetString(10);
                                testDTO.Type = reader.GetString(11);
                                testDTO.reqId = reader.GetInt32(12);
                                testDTO.reqNumber = reader.GetString(13);
                                testDTO.TestType = "TS";
                                testDTO.reqStatus = reader.GetBoolean(14);
                                list.Add(testDTO);

                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                return null;
            }
        }

        public Requirement GetByReqNumber(int projectId, string reqnumber)
        {
            var req = (from requirements in context.Requirements
                       where requirements.Project_Id == projectId
                       && requirements.req_number.Equals(reqnumber)
                       select requirements).FirstOrDefault();

            return req;
        }

        public List<TestProcedure> GetAllTestProcedureAutomated(int id)
        {
            try
            {
                var testProcedures = context.TestProcedures
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == id).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == true && x.Type.Equals("Automated"))
                    .ToList();
                return testProcedures;
            }
            catch
            {
                return null;
            }
        }

        public List<TestProcedure> GetAllTestProcedureInactivesAutomated(int id)
        {
            try
            {
                var testProcedures = context.TestProcedures
                    .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == id).Count() > 0)
                    .Include(r => r.RequirementsTests)
                    .Where(x => x.Status == false && x.Type.Equals("Automated"))
                    .ToList();
                return testProcedures;
            }
            catch
            {
                return null;
            }
        }

        public List<EvidenceDTO> GetAutomatedEvidenceFromReq(int[] ids)
        {
            List<EvidenceDTO> evidence = new List<EvidenceDTO>();
            foreach (var req in ids)
            {
                var listOfTc = (from tp in context.TestProcedures
                                join rt in context.RequirementsTests
                                on tp.Test_Procedure_Id equals rt.Test_Procedure_Id
                                where rt.Requirement_Id == req && tp.Script_Id != null
                                select new { id = tp.Test_Procedure_Id, title = tp.Title, number = tp.tp_number, description = tp.Description, priority = tp.Test_Priority, type = tp.Type }).ToList();

                foreach (var item in listOfTc)
                {
                    EvidenceDTO ev = new EvidenceDTO() { ReqId = req, Element_id = item.id, Identifier_number = item.number, Title = item.title, Description = item.description, Priority = item.priority, Type = item.type, Evidence = "TP" };
                    evidence.Add(ev);
                }
            }
            return evidence;
        }

        public List<string> GetSprints(int projectId)
        {
            var List = (from req in context.Requirements
                        where req.Project_Id == projectId
                        select req.Release
                             ).Distinct().ToList();
            return List;

        }


        public List<Requirement> GetNonActivesExecutedForTP(int executionId)
        {
            List<Requirement> ReqList = new List<Requirement>();
            Requirement req = null;
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT req.Id,  req.Project_Id, req.Name, req.Description, req.Developer_Assigned, req.Tester_Assigned, req.Axosoft_Task_Id, req.Status, req.req_number,req.Acceptance_Criteria, req.Release FROM Requirement as req left join RequirementsTest as tcreq on req.Id = tcreq.Requirement_Id WHERE tcreq.Test_Procedure_Id IN(Select testResult.Test_Procedure_Id FROM TestResult WHERE Test_Execution_Id ={0}) AND req.Status = 0 Group by req.Id, req.Project_Id, req.Name, req.Description, req.Developer_Assigned, req.Tester_Assigned, req.Axosoft_Task_Id, req.Status, req.req_number, req.Acceptance_Criteria, req.Release", executionId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                req = new Requirement();

                                req.Id = reader.GetInt32(0);
                                req.Project_Id = reader.GetInt32(1);
                                req.Name = reader.GetString(2);
                                req.Description = reader.IsDBNull(3) ? null : reader.GetString(3);
                                req.Developer_Assigned = reader.IsDBNull(4) ? null : reader.GetString(4);
                                req.Tester_Assigned = reader.IsDBNull(5) ? null : reader.GetString(5);
                                req.Axosoft_Task_Id = reader.GetInt32(6);
                                req.Status = reader.GetBoolean(7);
                                req.req_number = reader.GetString(8);
                                req.Acceptance_Criteria = reader.IsDBNull(9) ? null : reader.GetString(9);
                                req.Release = reader.IsDBNull(10) ? null : reader.GetString(10);
                                ReqList.Add(req);

                            }
                        }
                    }
                }
                return ReqList;
            }
            catch
            {
                return null;
            }
        }

        public List<Requirement> GetNonActivesExecutedForTC(int executionId)
        {
            List<Requirement> ReqList = new List<Requirement>();
            Requirement req = null;
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT req.Id,  req.Project_Id, req.Name, req.Description, req.Developer_Assigned, req.Tester_Assigned, req.Axosoft_Task_Id, req.Status, req.req_number,req.Acceptance_Criteria, req.Release FROM Requirement as req left join RequirementsTest as tcreq on req.Id = tcreq.Requirement_Id WHERE tcreq.Test_Case_Id IN(Select testResult.Test_Case_Id FROM TestResult WHERE Test_Execution_Id ={0}) AND req.Status = 0 Group by req.Id, req.Project_Id, req.Name, req.Description, req.Developer_Assigned, req.Tester_Assigned, req.Axosoft_Task_Id, req.Status, req.req_number, req.Acceptance_Criteria, req.Release", executionId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                req = new Requirement();

                                req.Id = reader.GetInt32(0);
                                req.Project_Id = reader.GetInt32(1);
                                req.Name = reader.GetString(2);
                                req.Description = reader.IsDBNull(3) ? null : reader.GetString(3);
                                req.Developer_Assigned = reader.IsDBNull(4) ? null : reader.GetString(4);
                                req.Tester_Assigned = reader.IsDBNull(5) ? null : reader.GetString(5);
                                req.Axosoft_Task_Id = reader.GetInt32(6);
                                req.Status = reader.GetBoolean(7);
                                req.req_number = reader.GetString(8);
                                req.Acceptance_Criteria = reader.IsDBNull(9) ? null : reader.GetString(9);
                                req.Release = reader.IsDBNull(10) ? null : reader.GetString(10);
                                ReqList.Add(req);

                            }
                        }
                    }
                }
                return ReqList;
            }
            catch
            {
                return null;
            }
        }

        public List<Requirement> GetNonActivesExecutedForTS(int executionId)
        {
            List<Requirement> ReqList = new List<Requirement>();
            Requirement req = null;
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    var query = String.Format("SELECT req.Id,  req.Project_Id, req.Name, req.Description, req.Developer_Assigned, req.Tester_Assigned, req.Axosoft_Task_Id, req.Status, req.req_number,req.Acceptance_Criteria, req.Release FROM Requirement as req left join RequirementsTest as tcreq on req.Id = tcreq.Requirement_Id WHERE tcreq.Test_Scenario_Id IN(Select testResult.Test_Scenario_Id FROM TestResult WHERE Test_Execution_Id ={0}) AND req.Status = 0 Group by req.Id, req.Project_Id, req.Name, req.Description, req.Developer_Assigned, req.Tester_Assigned, req.Axosoft_Task_Id, req.Status, req.req_number, req.Acceptance_Criteria, req.Release", executionId);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                req = new Requirement();

                                req.Id = reader.GetInt32(0);
                                req.Project_Id = reader.GetInt32(1);
                                req.Name = reader.GetString(2);
                                req.Description = reader.IsDBNull(3) ? null : reader.GetString(3);
                                req.Developer_Assigned = reader.IsDBNull(4) ? null : reader.GetString(4);
                                req.Tester_Assigned = reader.IsDBNull(5) ? null : reader.GetString(5);
                                req.Axosoft_Task_Id = reader.GetInt32(6);
                                req.Status = reader.GetBoolean(7);
                                req.req_number = reader.GetString(8);
                                req.Acceptance_Criteria = reader.IsDBNull(9) ? null : reader.GetString(9);
                                req.Release = reader.IsDBNull(10) ? null : reader.GetString(10);
                                ReqList.Add(req);

                            }
                        }
                    }
                }
                return ReqList;
            }
            catch
            {
                return null;
            }
        }



        public List<TestCase> GetUniqueTestCases(int ProjectId)
        {
            List<TestCase> TestCasesList = new List<TestCase>();
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("Select DISTINCT tc.* FROM TestCase as tc JOIN RequirementsTest as reqtc ON tc.Test_Case_Id = reqtc.Test_Case_Id WHERE reqtc.Requirement_Id = (Select Id from Requirement WHERE Project_Id = {0} AND Id = reqtc.Requirement_Id)", ProjectId);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        TestCase test = null;
                        while (reader.Read())
                        {
                            test = new TestCase();
                            test.Test_Case_Id = reader.GetInt32(0);
                            test.tc_number = reader.GetString(1);
                            test.Test_Priority = reader.GetString(2);
                            test.Title = reader.GetString(3);
                            test.Description = reader.GetString(4);
                            test.Preconditions = reader.GetString(5);
                            test.Test_Case_Creator = reader.GetString(6);
                            test.Expected_Result = reader.GetString(7);
                            test.Creation_Date = reader.GetDateTime(8);
                            test.Status = reader.GetBoolean(9);
                            test.Last_Editor = reader.GetString(10);
                            test.Type = reader.GetString(11);
                            TestCasesList.Add(test);
                        }
                    }
                }



            }
            return TestCasesList;
        }

        public List<TestProcedure> GetUniqueTestProcedures(int ProjectId)
        {
            List<TestProcedure> TestProceduresList = new List<TestProcedure>();
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("Select DISTINCT tc.* FROM TestProcedure as tc JOIN RequirementsTest as reqtc ON tc.Test_Procedure_Id = reqtc.Test_Procedure_Id WHERE reqtc.Requirement_Id = (Select Id from Requirement WHERE Project_Id = {0} AND Id = reqtc.Requirement_Id)", ProjectId);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        TestProcedure testp = null;
                        while (reader.Read())
                        {
                            testp = new TestProcedure();
                            testp.Test_Procedure_Id = reader.GetInt32(0);
                            testp.tp_number = reader.GetString(1);
                            testp.Test_Case_Id = reader.IsDBNull(2) ? 0 : (int)reader.GetInt32(2);
                            testp.Test_Priority = reader.GetString(3);
                            testp.Title = reader.GetString(4);
                            testp.Description = reader.GetString(5);
                            testp.Test_Procedure_Creator = reader.GetString(6);
                            testp.Expected_Result = reader.GetString(7);
                            testp.Creation_Date = reader.GetDateTime(8);
                            testp.Status = reader.GetBoolean(9);
                            testp.Last_Editor = reader.GetString(10);
                            testp.Type = reader.GetString(11);
                            TestProceduresList.Add(testp);
                        }
                    }
                }



            }
            return TestProceduresList;
        }

        public List<TestScenario> GetUniqueTestScenarios(int ProjectId)
        {
            List<TestScenario> TestScenariosList = new List<TestScenario>();
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("Select DISTINCT tc.* FROM TestScenario as tc JOIN RequirementsTest as reqtc ON tc.Test_Scenario_Id = reqtc.Test_Scenario_Id WHERE reqtc.Requirement_Id = (Select Id from Requirement WHERE Project_Id = {0} AND Id = reqtc.Requirement_Id)", ProjectId);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        TestScenario testSc = null;
                        while (reader.Read())
                        {
                            testSc = new TestScenario();
                            testSc.Test_Scenario_Id = reader.GetInt32(0);
                            testSc.ts_number = reader.GetString(1);
                            testSc.Test_Priority = reader.GetString(2);
                            testSc.Title = reader.GetString(3);
                            testSc.Description = reader.GetString(4);
                            testSc.Preconditions = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            testSc.Note = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            testSc.Test_Scenario_Creator = reader.GetString(7);
                            testSc.Creation_Date = reader.GetDateTime(8);
                            testSc.Status = reader.GetBoolean(9);
                            testSc.Last_Editor = reader.GetString(10);
                            testSc.Type = reader.GetString(11);
                            TestScenariosList.Add(testSc);
                        }
                    }
                }



            }
            return TestScenariosList;
        }

        public List<TestDTO> GetTestCasesByNumber(string[] TcNumbers, int projectId, int scriptId)
        {
            string[] DistincNumbers = TcNumbers.Distinct().ToArray();
            List<TestDTO> TestCasesList = GetUniqueTestCasesDTO(projectId);
            List<TestDTO> TestDTOList = new List<TestDTO>();
            foreach (var number in DistincNumbers)
            {
                var TC = (from testCase in TestCasesList
                          where testCase.IdentifiedNumber.Equals(number)
                          select testCase
                          ).FirstOrDefault();
                if (TC != null)
                {
                    TestDTO testDTO = new TestDTO
                    {
                        IdentifiedNumber = TC.IdentifiedNumber,
                        Test_Id = TC.Test_Id,
                        Priority = TC.Priority,
                        Title = TC.Title,
                        ScriptId = scriptId,
                        reqId= TC.reqId
                        
                    };
                    TestDTOList.Add(testDTO);
                }
                
            }
        //    var TestDTOListFinal = TestDTOList.Distinct().ToList();

            return TestDTOList;
        }

        public List<TestDTO> GetUniqueTestCasesDTO(int projectId)
        {
            List<TestDTO> TestCasesList = new List<TestDTO>();
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("Select DISTINCT tc.*, reqtc.Requirement_Id FROM TestCase as tc JOIN RequirementsTest as reqtc ON tc.Test_Case_Id = reqtc.Test_Case_Id WHERE reqtc.Requirement_Id = (Select Id from Requirement WHERE Project_Id = {0} AND Id = reqtc.Requirement_Id)", projectId);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        TestDTO test = null;
                        while (reader.Read())
                        {
                            test = new TestDTO();
                            test.Test_Id = reader.GetInt32(0);
                            test.IdentifiedNumber = reader.GetString(1);
                            test.Priority = reader.GetString(2);
                            test.Title = reader.GetString(3);
                            test.Description = reader.GetString(4);
                            test.Preconditions = reader.GetString(5);
                            test.Creator = reader.GetString(6);
                            test.ExpectedResult = reader.GetString(7);
                            test.CreationDate = reader.GetDateTime(8);
                            test.Status = reader.GetBoolean(9);
                            test.LastEditor = reader.GetString(10);
                            test.Type = reader.GetString(11);
                            test.reqId = reader.GetInt32(12);
                            TestCasesList.Add(test);
                        }
                    }
                }



            }
            return TestCasesList;
        }

    }
}

