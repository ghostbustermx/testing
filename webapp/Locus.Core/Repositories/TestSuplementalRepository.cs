using Locus.Core.Context;
using Locus.Core.DTO;
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
    public interface ITestSuplementalRepository
    {
        TestSuplemental Save(TestSuplemental testSuplemental, string user);

        TestSuplemental Update(TestSuplemental testSuplemental, string user);

        TestSuplemental Delete(int idTestSuplemental);

        TestSuplemental Enable(int idTestSuplemental);

        List<TestSuplemental> GetAll();

        TestSuplemental Get(int idTestSuplemental);

        TestSuplemental GetByNumber(int projectId, string number);

        TestSuplemental GetLastOne(int idTestProcedure, int idTestScenario, string creator, string date);

        TestSuplemental GetLastTestSuplemental(string creator, string date);

        TestSuplemental UpdateNumber(TestSuplemental testSuplemental);

        List<SupplementalTestProcedureDTO> GetAllSTPByProject(int idProject);

        Project GetProject(int idtp, int idts);

        Requirement GetRequirement(int idtp, int idts);

        List<TestSuplemental> GetForProject(int idProject);

        List<TestSuplemental> GetForProjectInactives(int idProject);

        List<TestSuplemental> GetForTestProcedure(int tpId);

        List<TestSuplemental> GetForTestScenario(int idTs);

        List<TestProcedure> GetProcedures(int idstp);

        List<TestScenario> GetScenarios(int idstp);

        ChangeLog AddChangeLog(int testsuplementalId, string user);

        List<ChangeLog> TestSuplementalChangeLogs(int id);

        ChangeLog Restore(ChangeLog change_log, string user);
    }

    public class TestSuplementalRepository : ITestSuplementalRepository
    {
        //Instance of Database Context
        LocustDBContext context = new LocustDBContext();
        public TestSuplemental Delete(int idTestSuplemental)
        {

            try
            {
                ReorderStepsTP(idTestSuplemental);
                ReorderStepsTS(idTestSuplemental);
                DeleteRelation(idTestSuplemental);
                // DeleteSteps(idTestSuplemental);
                var newContext = new LocustDBContext();
                var req = newContext.TestSuplementals.Find(idTestSuplemental);
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

        private void ReorderStepsTP(int idTestSuplemental)
        {
            try
            {
                TestSuplemental stp = context.TestSuplementals.Find(idTestSuplemental);
                var tpList = (from tpts in context.test_procedure_test_suplemental
                              join tp in context.TestProcedures on tpts.Test_Procedure_Id equals tp.Test_Procedure_Id
                              where tpts.Test_Suplemental_Id == idTestSuplemental
                              select tp.Test_Procedure_Id).ToList();
                foreach (int idtp in tpList)
                {
                    var stepsList = (from steps in context.Steps
                                     where steps.Test_Procedure_Id == idtp
                                     select steps).ToList();

                    foreach (Step step in stepsList)
                    {
                        if (step.action == stp.Title)
                        {
                            context.Steps.Remove(step);
                            context.SaveChanges();
                            OrderSteps(idtp, 0);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }


        private void ReorderStepsTS(int idTestSuplemental)
        {
            try
            {
                TestSuplemental stp = context.TestSuplementals.Find(idTestSuplemental);
                var tpList = (from tpts in context.test_procedure_test_suplemental
                              join tp in context.TestScenarios on tpts.Test_Scenario_Id equals tp.Test_Scenario_Id
                              where tpts.Test_Suplemental_Id == idTestSuplemental
                              select tp.Test_Scenario_Id).ToList();
                foreach (int idtp in tpList)
                {
                    var stepsList = (from steps in context.Steps
                                     where steps.Test_Scenario_Id == idtp
                                     select steps).ToList();

                    foreach (Step step in stepsList)
                    {
                        if (step.action == stp.Title)
                        {
                            context.Steps.Remove(step);
                            context.SaveChanges();
                            OrderSteps(0, idtp);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }


        private void OrderSteps(int idtp, int idts)
        {
            List<Step> stepsList;
            if (idtp != 0)
            {
                stepsList = (from steps in context.Steps
                             where steps.Test_Procedure_Id == idtp
                             select steps).OrderBy(e => e.number_steps).ToList();

            }
            else if (idts != 0)
            {
                stepsList = (from steps in context.Steps
                             where steps.Test_Procedure_Id == idtp
                             select steps).OrderBy(e => e.number_steps).ToList();
            }
            else
            {
                stepsList = null;
            }

            int cont = 0;
            foreach (Step step in stepsList)
            {
                step.number_steps = cont + 1;
                cont = cont + 1;
                context.Entry(step).State = System.Data.Entity.EntityState.Modified;

            }
            context.SaveChanges();

        }

        private void DeleteSteps(int idTestSuplemental)
        {
            try
            {
                var stepList = (from steps in context.Steps
                                where steps.Test_Suplemental_Id == idTestSuplemental
                                select steps).ToList();
                foreach (Step step in stepList)
                {
                    context.Steps.Remove(step);
                }
                context.SaveChanges();
                return;
            }
            catch
            {
                return;
            }
        }

        private void DeleteRelation(int idTestSuplemental)
        {
            try
            {
                var tptsList = (from tpts in context.test_procedure_test_suplemental
                                where tpts.Test_Suplemental_Id == idTestSuplemental
                                select tpts).ToList();
                foreach (Test_Procedure_Test_Suplemental tp in tptsList)
                {
                    context.test_procedure_test_suplemental.Remove(tp);
                }
                context.SaveChanges();
                return;
            }
            catch
            {
                return;
            }
        }

        public TestSuplemental Get(int idTestSuplemental)
        {
            try
            {
                return context.TestSuplementals.Find(idTestSuplemental);
            }
            catch
            {
                return null;
            }
        }

        public List<TestSuplemental> GetAll()
        {
            try
            {
                return context.TestSuplementals.ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<TestSuplemental> GetForProject(int idProject)
        {
            try
            {
                return context.TestSuplementals.Where(x => x.Project_Id == idProject).Where(x => x.Status == true).ToList();
            }
            catch
            {
                return null;
            }
        }

        public TestSuplemental GetLastOne(int idTestProcedure, int idTestScenario, string creator, string date)
        {
            try
            {
                if (idTestProcedure != 0)
                {
                    TestSuplemental lastOne = context.TestSuplementals
                        .Where(r => r.test_procedure_test_suplemental.Test_Procedure_Id == idTestProcedure)
                        .Where(x => x.Test_Procedure_Creator == creator)
                        .OrderByDescending(y => y.Test_Suplemental_Id)
                        .FirstOrDefault();
                    return lastOne;
                }
                else if (idTestScenario != 0)
                {
                    TestSuplemental lastOne = context.TestSuplementals
                                            .Where(r => r.test_procedure_test_suplemental.Test_Scenario_Id == idTestScenario)
                                            .Where(x => x.Test_Procedure_Creator == creator)
                                            .OrderByDescending(y => y.Test_Suplemental_Id)
                                            .FirstOrDefault();
                    return lastOne;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public TestSuplemental GetLastTestSuplemental(string creator, string date)
        {
            try
            {
                TestSuplemental aux1 = context.TestSuplementals.Where(r => r.Test_Procedure_Creator == creator).OrderByDescending(O => O.Test_Suplemental_Id).FirstOrDefault();

                return aux1;
            }
            catch
            {
                return null;
            }
        }

        public List<TestProcedure> GetProcedures(int idstp)
        {
            try
            {
                return (from tp in context.TestProcedures
                        join tpts in context.test_procedure_test_suplemental on tp.Test_Procedure_Id equals tpts.Test_Procedure_Id
                        where tpts.Test_Suplemental_Id == idstp && tpts.Status == true
                        select tp).ToList();
            }
            catch
            {
                return null;
            }
        }

        public Project GetProject(int idtp, int idts)
        {
            try
            {
                var projectId = 0;
                if (idtp != 0)
                {
                    projectId = (from req in context.Requirements
                                 join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                                 where rt.Test_Procedure_Id == idtp
                                 select req.Project_Id).FirstOrDefault();


                }
                else if (idts != 0)
                {
                    projectId = (from req in context.Requirements
                                 join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                                 where rt.Test_Scenario_Id == idts
                                 select req.Project_Id).FirstOrDefault();

                }
                return context.Projects.Find(projectId);
            }
            catch
            {
                return null;
            }
        }

        public Requirement GetRequirement(int idtp, int idts)
        {
            try
            {
                var reqId = 0;
                if (idtp != 0)
                {
                    reqId = (from req in context.Requirements
                             join rt in context.RequirementsTests on reqId equals rt.Requirement_Id
                             where rt.Test_Procedure_Id == idtp
                             select req.Id).FirstOrDefault();

                }
                else if (idts != 0)
                {
                    reqId = (from req in context.Requirements
                             join rt in context.RequirementsTests on reqId equals rt.Requirement_Id
                             where rt.Test_Scenario_Id == idts
                             select req.Id).FirstOrDefault();
                }
                return context.Requirements.Find(reqId);
            }
            catch
            {
                return null;
            }
        }

        public List<TestScenario> GetScenarios(int idstp)
        {
            try
            {
                List<TestScenario> scenariosList = (from ts in context.TestScenarios
                                                    join tpts in context.test_procedure_test_suplemental on ts.Test_Scenario_Id equals tpts.Test_Scenario_Id
                                                    where tpts.Test_Suplemental_Id == idstp && tpts.Status == true
                                                    select ts).ToList();

                return scenariosList;
            }
            catch
            {
                return null;
            }
        }

        public TestSuplemental Save(TestSuplemental testSuplemental, string user)
        {
            var newContext = new LocustDBContext();
            try
            {
                var laststp = (from stps in context.TestSuplementals
                               where stps.Project_Id == testSuplemental.Project_Id
                               select stps).OrderByDescending(x => x.Test_Suplemental_Id).FirstOrDefault();

                var stpnumber = 0;
                if (laststp != null)
                {
                    stpnumber = Convert.ToInt32(laststp.stp_number.Substring(4));
                }

                testSuplemental.stp_number = "STP_" + Convert.ToString(stpnumber + 1);
                testSuplemental.Status = true;
                testSuplemental.Test_Procedure_Creator = user;
                testSuplemental.Last_Editor = user;
                testSuplemental.Creation_Date = DateTime.UtcNow;
                context.TestSuplementals.Add(testSuplemental);
                context.SaveChanges();
                return testSuplemental;
            }
            catch
            {
                return null;
            }
        }

        public TestSuplemental Update(TestSuplemental testSuplemental, string user)
        {
            testSuplemental.Last_Editor = user;
            try
            {
                context.Entry(testSuplemental).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return testSuplemental;
            }
            catch
            {
                return null;
            }
        }

        public TestSuplemental UpdateNumber(TestSuplemental testSuplemental)
        {
            var newContext = new LocustDBContext();
            try
            {
                var laststp = (from stps in context.TestSuplementals
                               where stps.Project_Id == testSuplemental.Project_Id
                               select stps).OrderBy(x => x.Test_Suplemental_Id).ToList();

                int stpnumber;
                if (laststp.Count + 1 == 1)
                {
                    stpnumber = 0;
                }
                else
                {
                    var OneBeforeLast = laststp.ElementAtOrDefault((laststp.Count + 1) - 2);
                    stpnumber = Convert.ToInt32(OneBeforeLast.stp_number.Substring(4));
                }

                testSuplemental.stp_number = "STP_" + Convert.ToString(stpnumber + 1);
                newContext.Entry(testSuplemental).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                context.SaveChanges();
                return testSuplemental;
            }
            catch
            {
                return null;
            }
        }

        public TestSuplemental Enable(int idTestSuplemental)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.TestSuplementals.Find(idTestSuplemental);
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

        public List<TestSuplemental> GetForProjectInactives(int idProject)
        {
            try
            {
                return context.TestSuplementals.Where(x => x.Project_Id == idProject).Where(x => x.Status == false).ToList();
            }
            catch
            {
                return null;
            }
        }

        public ChangeLog AddChangeLog(int testsuplementalId, string user)
        {
            System.Threading.Thread.Sleep(2000);
            var active = (from ch in context.ChangeLogs
                          join tcl in context.Test_ChangeLogs on ch.Id equals tcl.Change_Log_Id
                          where tcl.Test_Suplemental_Id == testsuplementalId && ch.Active == true
                          select ch).FirstOrDefault();
            if (active != null)
            {
                active.Active = false;
                context.Entry(active).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            TestSuplemental tc = context.TestSuplementals.Find(testsuplementalId);
            /*Get steps*/
            List<Step> listSteps = context.Steps.Where(x => x.Test_Suplemental_Id == testsuplementalId).ToList();
            /*Get Tags*/
            var listTags = (from tags in context.Tags
                            join testTags in context.test_tags on tags.id equals testTags.Tag_Id
                            where testTags.Test_Suplemental_Id == testsuplementalId
                            select tags).ToList();

            String stepstr = JsonConvert.SerializeObject(listSteps);
            String tcstr = JsonConvert.SerializeObject(tc);

            String tagstr = JsonConvert.SerializeObject(listTags);

            String content = "!&&&@@!TestSuplemental:!&&&@@!" + tcstr + "!&&&@@!Steps:!&&&@@!" + stepstr + "!&&&@@!Tags:!&&&@@!" + tagstr;
            ChangeLog cl = new ChangeLog();
            cl.Content = content;
            cl.User = user;
            cl.Date = DateTime.UtcNow;
            cl.Active = true;
            var last = context.Test_ChangeLogs.Where(x => x.Test_Suplemental_Id == testsuplementalId).OrderByDescending(y => y.Change_Log_Id).FirstOrDefault();
            var lastVersion = 0;
            if (last != null)
            {
                lastVersion = context.ChangeLogs.Find(last.Change_Log_Id).Version;
            }
            cl.Version = lastVersion + 1;
            context.ChangeLogs.Add(cl);
            context.SaveChanges();
            return AddTestChangeLog(cl, tc);
        }

        private ChangeLog AddTestChangeLog(ChangeLog changeLog, TestSuplemental tc)
        {
            var cl = context.ChangeLogs.Where(x => x.Content == changeLog.Content &&
                                          x.Date == changeLog.Date &&
                                          x.User == changeLog.User &&
                                          x.Version == changeLog.Version).FirstOrDefault();
            Test_ChangeLog tcl = new Test_ChangeLog();
            tcl.Test_Suplemental_Id = tc.Test_Suplemental_Id;
            tcl.Change_Log_Id = changeLog.Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return changeLog;
        }

        public List<ChangeLog> TestSuplementalChangeLogs(int id)
        {
            try
            {
                return (from cl in context.ChangeLogs
                        join tcl in context.Test_ChangeLogs on cl.Id equals tcl.Change_Log_Id
                        where tcl.Test_Suplemental_Id == id
                        select cl).OrderByDescending(x => x.Version).ToList();
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
            var tc_idx = content.IndexOf("!&&&@@!TestSuplemental:!&&&@@!");
            var st_idx = content.IndexOf("!&&&@@!Steps:!&&&@@!");
            var tag_idx = content.IndexOf("!&&&@@!Tags:!&&&@@!");

            //Test case
            var tc_values = content.Substring(tc_idx + 30, (st_idx - (tc_idx + 30)));
            TestSuplemental tc_obj = JsonConvert.DeserializeObject<TestSuplemental>(tc_values);
            tc_obj.Last_Editor = user;
            //Steps
            var steps_values = content.Substring(st_idx + 20, (tag_idx - (st_idx + 20)));
            List<Step> steps_obj = JsonConvert.DeserializeObject<List<Step>>(steps_values);


            //Tags
            var tags_values = content.Substring(tag_idx + 19);
            List<Tag> tags_obj = JsonConvert.DeserializeObject<List<Tag>>(tags_values);
            Console.WriteLine(tags_obj);


            /* Find all the change logs and set Active to false */
            var changeLogs = (from ttcl in context.Test_ChangeLogs
                              join cl in context.ChangeLogs on ttcl.Change_Log_Id equals cl.Id
                              where ttcl.Test_Suplemental_Id == tc_obj.Test_Suplemental_Id
                              select cl).ToList();

            foreach (ChangeLog cl in changeLogs)
            {
                cl.Active = false;
                context.Entry(cl).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            /* Delete all relations and data from the actual test case */

            //Delete steps
            var steps = context.Steps.Where(x => x.Test_Suplemental_Id == tc_obj.Test_Suplemental_Id).ToList();
            foreach (Step st in steps)
            {
                context.Steps.Remove(st);
                context.SaveChanges();
            }

            //Delete tags Relationship
            /*
            var tags = context.test_tags.Where(x => x.Test_Suplemental_Id == tc_obj.Test_Suplemental_Id).ToList();
            foreach (Test_Tags tag in tags)
            {
                context.test_tags.Remove(tag);
                context.SaveChanges();
            }*/
            this.ExecuteQuery(String.Format("DELETE FROM TestTags WHERE Test_Suplemental_Id = {0}", tc_obj.Test_Suplemental_Id));
            /* Save Test Case with the new values */
            //Test Case
            context.Entry(tc_obj).State = System.Data.Entity.EntityState.Modified;
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
                tt.Test_Suplemental_Id = tc_obj.Test_Suplemental_Id;
                context.test_tags.Add(tt);
                context.SaveChanges();
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
            tcl.Test_Suplemental_Id = tc_obj.Test_Suplemental_Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return change_log;
        }

        public List<SupplementalTestProcedureDTO> GetAllSTPByProject(int idProject)
        {
            IStepRepository Steps = new StepRepository();
            ITagRepository Tag = new TagRepository();
            List<SupplementalTestProcedureDTO> list = new List<SupplementalTestProcedureDTO>();
            var stp = this.GetForProject(idProject);
            foreach (var row in stp)
            {
                var steps = Steps.GetForTestSuplemental(row.Test_Suplemental_Id);
                var tags = Tag.GetTestSuplementalTags(row.Test_Suplemental_Id);


                SupplementalTestProcedureDTO dto = new SupplementalTestProcedureDTO()
                {
                    Title = row.Title,
                    Description = row.Description,
                    Steps = steps,
                    Tags = tags,
                    Project_Id = row.Project_Id,
                    Status = row.Status,
                    stp_number = row.stp_number,
                    Test_Suplemental_Id = row.Test_Suplemental_Id,
                    Creation_Date = row.Creation_Date,
                    Test_Procedure_Creator = row.Test_Procedure_Creator
                };

                list.Add(dto);
            }
            return list;
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

        public List<TestSuplemental> GetForTestProcedure(int tpId)
        {
            List<TestSuplemental> list = (from stps in context.TestSuplementals
                                          join tpstp in context.test_procedure_test_suplemental
                                          on stps.Test_Suplemental_Id equals tpstp.Test_Suplemental_Id
                                          where tpstp.Test_Procedure_Id == tpId
                                          select stps
                                          ).ToList();

            return list;
        }

        public List<TestSuplemental> GetForTestScenario(int idTs)
        {
            List<TestSuplemental> list = (from stps in context.TestSuplementals
                                          join tpstp in context.test_procedure_test_suplemental
                                          on stps.Test_Suplemental_Id equals tpstp.Test_Suplemental_Id
                                          where tpstp.Test_Scenario_Id == idTs
                                          select stps
                                          ).ToList();

            return list;
        }

        public TestSuplemental GetByNumber(int projectId, string number)
        {
            var stp = (from sup in context.TestSuplementals
                       where sup.stp_number == number &&
                       sup.Project_Id == projectId
                       select sup).FirstOrDefault();


            return stp;
        }
    }
}
