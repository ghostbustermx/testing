using Locus.Core.Context;
using Locus.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Repositories
{
    public interface ITestScenarioRepository
    {
        TestScenario Save(TestScenario testScenario, string user);

        TestScenario Update(TestScenario testScenario, string user);

        TestScenario Delete(int idTestScenario);

        TestScenario Enable(int idTestScenario);

        List<TestScenario> GetAll();

        TestScenario Get(int idTestScenario);

        TestScenario GetLastOne(int idReq, string creator, string date);

        TestScenario GetLastTestScenario(string creator, string date);

        TestScenario UpdateNumber(TestScenario testScenario,int idReq);

        Project GetProject(int idts);

        Requirement GetRequirement(int idts);

        Project GetProjectRequirement(int idreq);

        ChangeLog AddChangeLog(int testscenaruiId, string user);

        ChangeLog Restore(ChangeLog change_log, string user);

        List<ChangeLog> TestScenarioChangeLogs(int id);

        Requirement GetRequirementForTs(int tsId);
    }

    public class TestScenarioRepository : ITestScenarioRepository
    {
        //Instance of Database Context
        LocustDBContext context = new LocustDBContext();

        public ChangeLog AddChangeLog(int testscenarioId, string user)
        {
            System.Threading.Thread.Sleep(2000);
            var active = (from ch in context.ChangeLogs
                          join tcl in context.Test_ChangeLogs on ch.Id equals tcl.Change_Log_Id
                          where tcl.Test_Scenario_Id == testscenarioId && ch.Active == true
                          select ch).FirstOrDefault();
            if (active != null)
            {
                active.Active = false;
                context.Entry(active).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            TestScenario tp = context.TestScenarios.Find(testscenarioId);

            /*Get steps*/
            List<Step> listSteps = context.Steps.Where(x => x.Test_Scenario_Id == testscenarioId).ToList();

            /*Get Tags*/
            var listTags = (from tags in context.Tags
                            join testTags in context.test_tags on tags.id equals testTags.Tag_Id
                            where testTags.Test_Scenario_Id == testscenarioId
                            select tags).ToList();

            /*List relations with requirements*/
            var listRelations = (from req in context.Requirements
                                 join testReq in context.RequirementsTests on req.Id equals testReq.Requirement_Id
                                 where testReq.Test_Scenario_Id == testscenarioId
                                 select req).ToList();

            /*List Relations Suplemental Test Procedures*/
            var listStpRelations = context.test_procedure_test_suplemental.Where(x => x.Test_Scenario_Id == testscenarioId).ToList();

            String stepstr = JsonConvert.SerializeObject(listSteps);
            String tcstr = JsonConvert.SerializeObject(tp);
            String tagstr = JsonConvert.SerializeObject(listTags);
            String reqstr = JsonConvert.SerializeObject(listRelations);
            String stpstr = JsonConvert.SerializeObject(listStpRelations);

            String content = "!&&&@@!TestScenario:!&&&@@!" + tcstr + "!&&&@@!Steps:!&&&@@!" + stepstr + "!&&&@@!Tags:!&&&@@!" + tagstr + "!&&&@@!Req:!&&&@@!" + reqstr + "!&&&@@!Stp:!&&&@@!" + stpstr;
            ChangeLog cl = new ChangeLog();
            cl.Content = content;
            cl.User = user;
            cl.Date = DateTime.UtcNow;
            cl.Active = true;
            var last = context.Test_ChangeLogs.Where(x => x.Test_Scenario_Id == testscenarioId).OrderByDescending(y => y.Change_Log_Id).FirstOrDefault();
            var lastVersion = 0;
            if (last != null)
            {
                lastVersion = context.ChangeLogs.Find(last.Change_Log_Id).Version;
            }
            cl.Version = lastVersion + 1;
            context.ChangeLogs.Add(cl);
            context.SaveChanges();
            return AddTestChangeLog(cl, tp);
        }

        private ChangeLog AddTestChangeLog(ChangeLog changeLog, TestScenario tp)
        {
            var cl = context.ChangeLogs.Where(x => x.Content == changeLog.Content &&
                                          x.Date == changeLog.Date &&
                                          x.User == changeLog.User &&
                                          x.Version == changeLog.Version).FirstOrDefault();
            Test_ChangeLog tcl = new Test_ChangeLog();
            tcl.Test_Scenario_Id = tp.Test_Scenario_Id;
            tcl.Change_Log_Id = changeLog.Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return changeLog;

        }
        
        public TestScenario Delete(int idTestScenario)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.TestScenarios.Find(idTestScenario);
                req.Status = false;
                newContext.Entry(req).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                return req;
            }
            catch
            {
                return null;
            }
        }

        public TestScenario Enable(int idTestScenario)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.TestScenarios.Find(idTestScenario);
                req.Status = true;
                newContext.Entry(req).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                return req;
            }
            catch
            {
                return null;
            }
        }


        public TestScenario Get(int idTestScenario)
        {
            try
            {
                return context.TestScenarios.Find(idTestScenario);
            }
            catch
            {
                return null;
            }
        }

        public List<TestScenario> GetAll()
        {
            try
            {
                return context.TestScenarios.ToList();
            }
            catch
            {
                return null;
            }
        }

        public TestScenario GetLastOne(int idReq, string creator, string date)
        {
            try
            {
                TestScenario aux1 = context.TestScenarios
                .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == idReq).Count() > 0)
                .Where(r => r.Test_Scenario_Creator == creator)
                .OrderByDescending(r => r.Test_Scenario_Id)
                .FirstOrDefault();

                return aux1;
            }
            catch
            {
                return null;
            }
        }

        public TestScenario GetLastTestScenario(string creator, string date)
        {
            try
            {
                TestScenario aux1 = context.TestScenarios.Where(r => r.Test_Scenario_Creator == creator).OrderByDescending(O => O.Test_Scenario_Id).FirstOrDefault();

                return aux1;
            }
            catch
            {
                return null;
            }
        }

        public Project GetProject(int idts)
        {
            try
            {
                var projectId = (from req in context.Requirements
                                 join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                                 where rt.Test_Scenario_Id == idts
                                 select req.Project_Id).FirstOrDefault();

                return context.Projects.Find(projectId);

            }
            catch
            {
                return null;
            }
        }

        public Project GetProjectRequirement(int idreq)
        {
            try
            {
                var projectId = (from req in context.Requirements
                                 where req.Id == idreq
                                 select req.Project_Id).FirstOrDefault();

                return context.Projects.Find(projectId);

            }
            catch
            {
                return null;
            }
        }

        public Requirement GetRequirement(int reqid)
        {
            try
            {
                
                return context.Requirements.Find(reqid);

            }
            catch
            {
                return null;
            }
        }

        public ChangeLog Restore(ChangeLog change_log, string user)
        {
            var content = change_log.Content;

            /*Deserialize all the content from the change log*/
            var tc_idx = content.IndexOf("!&&&@@!TestScenario:!&&&@@!");
            var st_idx = content.IndexOf("!&&&@@!Steps:!&&&@@!");
            var tag_idx = content.IndexOf("!&&&@@!Tags:!&&&@@!");
            var req_idx = content.IndexOf("!&&&@@!Req:!&&&@@!");
            var stp_idx = content.IndexOf("!&&&@@!Stp:!&&&@@!");
            //Test case
            var ts_values = content.Substring(tc_idx + 27, (st_idx - (tc_idx + 27)));
            TestScenario ts_obj = JsonConvert.DeserializeObject<TestScenario>(ts_values);
            ts_obj.Last_Editor = user;
            //Steps
            var steps_values = content.Substring(st_idx + 20, (tag_idx - (st_idx + 20)));
            List<Step> steps_obj = JsonConvert.DeserializeObject<List<Step>>(steps_values);


            //Tags
            var tags_values = content.Substring(tag_idx + 19, (req_idx - (tag_idx + 19)));
            List<Tag> tags_obj = JsonConvert.DeserializeObject<List<Tag>>(tags_values);
            Console.WriteLine(tags_obj);


            //Requirements
            var req_values = content.Substring(req_idx + 18, (stp_idx - (req_idx + 18)));
            List<Requirement> req_obj = JsonConvert.DeserializeObject<List<Requirement>>(req_values);

            //Suplementals 
            var sup_values = content.Substring(stp_idx + 18);
            List<Test_Procedure_Test_Suplemental> tpts_obj = JsonConvert.DeserializeObject<List<Test_Procedure_Test_Suplemental>>(sup_values);

            /* Find all the change logs and set Active to false */
            var changeLogs = (from ttcl in context.Test_ChangeLogs
                              join cl in context.ChangeLogs on ttcl.Change_Log_Id equals cl.Id
                              where ttcl.Test_Scenario_Id == ts_obj.Test_Scenario_Id
                              select cl).ToList();

            foreach (ChangeLog cl in changeLogs)
            {
                cl.Active = false;
                context.Entry(cl).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            /* Delete all relations and data from the actual test case */

            //Delete steps
            var steps = context.Steps.Where(x => x.Test_Scenario_Id == ts_obj.Test_Scenario_Id).ToList();
            foreach (Step st in steps)
            {
                context.Steps.Remove(st);
                context.SaveChanges();
            }

            //Delete tags Relationship
            /*var tags = context.test_tags.Where(x => x.Test_Scenario_Id == ts_obj.Test_Scenario_Id).ToList();
            foreach (Test_Tags tag in tags)
            {
                context.test_tags.Remove(tag);
                context.SaveChanges();
            }*/

            this.ExecuteQuery(String.Format("DELETE FROM TestTags WHERE Test_Scenario_Id = {0}", ts_obj.Test_Scenario_Id));

            //Delete Requirement Relationship.
            /*
            var reqs = context.RequirementsTests.Where(x => x.Test_Scenario_Id == ts_obj.Test_Scenario_Id).ToList();
            foreach (RequirementsTest rt in reqs)
            {
                context.RequirementsTests.Remove(rt);
                context.SaveChanges();
            }*/

            this.ExecuteQuery(String.Format("DELETE FROM RequirementsTest WHERE Test_Scenario_Id = {0}", ts_obj.Test_Scenario_Id));
            //Delete Stp Relationships

            this.ExecuteQuery(String.Format("DELETE FROM Test_Procedure_Test_Suplemental WHERE Test_Scenario_Id = {0}", ts_obj.Test_Scenario_Id));

            if (ts_obj.Type == null)
            {
                ts_obj.Type = "Functional";
            }

            /* Save Test Case with the new values */
            //Test Case
            context.Entry(ts_obj).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            //Add Steps.
            foreach (Step st in steps_obj)
            {
                context.Steps.Add(st);
                context.SaveChanges();
            }
            //Add Tags.
            foreach (Tag t in tags_obj)
            {
                Test_Tags tt = new Test_Tags();
                tt.Tag_Id = t.id;
                tt.Test_Scenario_Id = ts_obj.Test_Scenario_Id;
                context.test_tags.Add(tt);
                context.SaveChanges();
            }

            //Add Requirements.
            foreach (Requirement r in req_obj)
            {
                RequirementsTest rt = new RequirementsTest();
                rt.Requirement_Id = r.Id;
                rt.Test_Scenario_Id = ts_obj.Test_Scenario_Id;
                context.RequirementsTests.Add(rt);
                context.SaveChanges();
            }

            //Add Relations between STP
            foreach (Test_Procedure_Test_Suplemental tps1 in tpts_obj)
            {
                this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Scenario_Id) VALUES ({0}, {1}) ", tps1.Test_Suplemental_Id, tps1.Test_Scenario_Id));
            }

            /*Create new change log*/
            change_log.Version = changeLogs.Count + 1;
            change_log.User = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(16);
            change_log.Date = DateTime.UtcNow;
            change_log.Active = true;
            context.ChangeLogs.Add(change_log);
            context.SaveChanges();

            /*Save new test_changelog*/
            var id_cl = (from cl in context.ChangeLogs
                         where cl.Content == change_log.Content
                         && cl.Version == change_log.Version
                         select cl).FirstOrDefault();
            Test_ChangeLog tcl = new Test_ChangeLog();
            tcl.Change_Log_Id = id_cl.Id;
            tcl.Test_Scenario_Id = ts_obj.Test_Scenario_Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return change_log;
        }

        private bool ExecuteQuery(string query)
        {
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
                            if (reader.Read())
                            {

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public TestScenario Save(TestScenario testScenario, string user)
        {
            try
            {
                testScenario.Status = true;
                testScenario.Creation_Date = DateTime.UtcNow;
                testScenario.Test_Scenario_Creator = user;
                testScenario.Last_Editor = user;
                context.TestScenarios.Add(testScenario);
                context.SaveChanges();
                return GetLastTestScenario(testScenario.Test_Scenario_Creator, "today");
            }
            catch
            {
                return null;
            }
            
        }

        public List<ChangeLog> TestScenarioChangeLogs(int id)
        {
            try
            {
                return (from cl in context.ChangeLogs
                        join tcl in context.Test_ChangeLogs on cl.Id equals tcl.Change_Log_Id
                        where tcl.Test_Scenario_Id == id
                        select cl).OrderByDescending(x => x.Version).ToList();
            }
            catch
            {
                return null;
            }
        }

        public TestScenario Update(TestScenario testScenario, string user)
        {
            testScenario.Last_Editor = user;
            try
            {
                var newContext = new LocustDBContext();
                newContext.Entry(testScenario).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                return testScenario;
            }
            catch
            {
                return null;
            }
        }

        public TestScenario UpdateNumber(TestScenario testScenario, int idReq)
        {
            var newContext = new LocustDBContext();
            try
            {
                var projectId = (from pr in context.Projects
                                 join requ in context.Requirements on pr.Id equals requ.Project_Id
                                 where requ.Id == idReq
                                 select pr.Id).FirstOrDefault();

                int id = Convert.ToInt32(projectId);

                var testScenarios =
                 (from ts in context.TestScenarios
                  join rt in context.RequirementsTests on ts.Test_Scenario_Id equals rt.Test_Scenario_Id
                  join r in context.Requirements on rt.Requirement_Id equals r.Id
                  join p in context.Projects on r.Project_Id equals p.Id
                  where p.Id == id
                  select ts).OrderByDescending(x => x.Test_Scenario_Id).FirstOrDefault();


                var tests =
                 (from ts in context.TestScenarios
                  join rt in context.RequirementsTests on ts.Test_Scenario_Id equals rt.Test_Scenario_Id
                  join r in context.Requirements on rt.Requirement_Id equals r.Id
                  join p in context.Projects on r.Project_Id equals p.Id
                  where p.Id == id
                  select ts).OrderBy(x => x.Test_Scenario_Id).ToList();

                int tsnumber;

                if (tests.Count + 1 == 1)
                {
                    tsnumber = 0;
                }
                else
                {
                    var OneBeforeLast = tests.ElementAtOrDefault((tests.Count + 1) - 2);
                    tsnumber = Convert.ToInt32(OneBeforeLast.ts_number.Substring(3));
                }


                testScenario.ts_number = "TS_" + Convert.ToString(tsnumber + 1);
                newContext.Entry(testScenario).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                context.SaveChanges();
                return testScenario;
            }
            catch
            {
                return null;
            }
        }

        public Requirement GetRequirementForTs(int tsId)
        {
            try
            {
                var reqId = (from req in context.Requirements
                             join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                             where rt.Test_Scenario_Id == tsId
                             select req.Id).FirstOrDefault();

                return context.Requirements.Find(reqId);

            }
            catch
            {
                return null;
            }
        }


    }
}
