using Locus.Core.Context;
using Locus.Core.DTO;
using Locus.Core.Models;
using Newtonsoft.Json;
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
    public interface ITestProcedureRepository
    {
        TestProcedure Save(TestProcedure testProcedure, string user);

        TestProcedure Update(TestProcedure testProcedure, string user);

        TestProcedure Delete(int idTestProcedure);

        TestProcedure Enable(int idTestProcedure);

        List<TestProcedure> GetAll();

        TestProcedure Get(int idTestProcedure);

        TestProcedure GetLastOne(int idReq, string creator, string date);

        TestProcedure GetLastTestProcedure(string creator, string date);

        TestProcedure UpdateNumber(TestProcedure testProcedure, int idReq);

        Project GetProject(int idtp);

        Requirement GetRequirement(int idtp);

        Project GetProjectRequirement(int idreq);

        ChangeLog AddChangeLog(int testprocedureId, string user);

        ChangeLog Restore(ChangeLog change_log, string user);

        List<ChangeLog> TestProcedureChangeLogs(int id);

        TestProcedure GetTestProcedureTc(int id);

        bool UpdateFromTC(TestCase tc);

        bool IsAssigned(int? tpid, int? tcid);

        void DeleteRelation(int? tcid);

        Requirement GetRequirementForTp(int tpId);

        List<TestProcedure> CreateTestProcedureFromTestCase(int projectId, List<TestDTO> testDTOs, string User);
    }

    public class TestProcedureRepository : ITestProcedureRepository
    {
        //Instance of Database Context
        LocustDBContext context = new LocustDBContext();

        public TestProcedure Delete(int idTestProcedure)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.TestProcedures.Find(idTestProcedure);
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

        public TestProcedure Enable(int idTestProcedure)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.TestProcedures.Find(idTestProcedure);
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

        public TestProcedure Get(int idTestProcedure)
        {
            try
            {
                return context.TestProcedures.Find(idTestProcedure);
            }
            catch
            {
                return null;
            }
        }

        public List<TestProcedure> GetAll()
        {
            try
            {
                return context.TestProcedures.ToList();
            }
            catch
            {
                return null;
            }
        }

        public TestProcedure GetLastOne(int idReq, string creator, string date)
        {
            try
            {
                TestProcedure aux1 = context.TestProcedures
                .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == idReq).Count() > 0)
                .Where(r => r.Test_Procedure_Creator == creator)
                .OrderByDescending(r => r.Test_Case_Id)
                .FirstOrDefault();

                return aux1;
            }
            catch
            {
                return null;
            }
        }

        public TestProcedure GetLastTestProcedure(string creator, string date)
        {
            try
            {
                TestProcedure aux1 = context.TestProcedures.Where(r => r.Test_Procedure_Creator == creator).OrderByDescending(O => O.Test_Procedure_Id).FirstOrDefault();

                return aux1;
            }
            catch
            {
                return null;
            }
        }

        public Project GetProject(int idtp)
        {
            try
            {
                var projectId = (from req in context.Requirements
                                 join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                                 where rt.Test_Procedure_Id == idtp
                                 select req.Project_Id).FirstOrDefault();

                return context.Projects.Find(projectId);

            }
            catch
            {
                return null;
            }
        }

        public TestProcedure GetTestProcedureTc(int idtc)
        {
            try
            {
                var tpId = (from tp in context.TestProcedures
                            where tp.Test_Case_Id == idtc
                            select tp.Test_Procedure_Id).FirstOrDefault();

                return context.TestProcedures.Find(tpId);

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

        public Requirement GetRequirement(int reqId)
        {
            try
            {
                /*var reqId = (from req in context.Requirements
                             join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                             where rt.Test_Procedure_Id == idtp
                             select req.Id).FirstOrDefault();
                             */
                return context.Requirements.Find(reqId);

            }
            catch
            {
                return null;
            }
        }

        public TestProcedure Save(TestProcedure testProcedure, string user)
        {
            testProcedure.Status = true;
            testProcedure.Creation_Date = DateTime.UtcNow;
            testProcedure.Test_Procedure_Creator = user;
            testProcedure.Last_Editor = user;
            context.TestProcedures.Add(testProcedure);
            context.SaveChanges();
            return GetLastTestProcedure(testProcedure.Test_Procedure_Creator, "today");
        }

        public TestProcedure Update(TestProcedure testProcedure, string user)
        {
            testProcedure.Last_Editor = user;
            try
            {
                var newContext = new LocustDBContext();
                newContext.Entry(testProcedure).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                return testProcedure;
            }
            catch
            {
                return null;
            }
        }

        public TestProcedure UpdateNumber(TestProcedure testProcedure, int idReq)
        {
            var newContext = new LocustDBContext();

            try
            {
                var projectId = (from pr in context.Projects
                                 join requ in context.Requirements on pr.Id equals requ.Project_Id
                                 where requ.Id == idReq
                                 select pr.Id).FirstOrDefault();

                int id = Convert.ToInt32(projectId);

                var tests =
                 (from tc in context.TestProcedures
                  join rt in context.RequirementsTests on tc.Test_Procedure_Id equals rt.Test_Procedure_Id
                  join r in context.Requirements on rt.Requirement_Id equals r.Id
                  join p in context.Projects on r.Project_Id equals p.Id
                  where p.Id == id
                  select tc).OrderBy(x => x.Test_Procedure_Id).ToList();

                int tcnumber;

                if (tests.Count + 1 == 1)
                {
                    tcnumber = 0;
                }
                else
                {
                    var OneBeforeLast = tests.ElementAtOrDefault((tests.Count + 1) - 2);
                    tcnumber = Convert.ToInt32(OneBeforeLast.tp_number.Substring(3));
                }


                testProcedure.tp_number = "TP_" + Convert.ToString(tcnumber + 1);
                newContext.Entry(testProcedure).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                return testProcedure;
            }
            catch
            {
                return null;
            }
        }

        public ChangeLog AddChangeLog(int testprocedureId, string user)
        {
            System.Threading.Thread.Sleep(2000);
            var active = (from ch in context.ChangeLogs
                          join tcl in context.Test_ChangeLogs on ch.Id equals tcl.Change_Log_Id
                          where tcl.Test_Procedure_Id == testprocedureId && ch.Active == true
                          select ch).FirstOrDefault();
            if (active != null)
            {
                active.Active = false;
                context.Entry(active).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            TestProcedure tp = context.TestProcedures.Find(testprocedureId);

            /*Get steps*/
            List<Step> listSteps = context.Steps.Where(x => x.Test_Procedure_Id == testprocedureId).ToList();

            /*Get Tags*/
            var listTags = (from tags in context.Tags
                            join testTags in context.test_tags on tags.id equals testTags.Tag_Id
                            where testTags.Test_Procedure_Id == testprocedureId
                            select tags).ToList();

            /*List relations with requirements*/
            var listRelations = (from req in context.Requirements
                                 join testReq in context.RequirementsTests on req.Id equals testReq.Requirement_Id
                                 where testReq.Test_Procedure_Id == testprocedureId
                                 select req).ToList();

            /*List Relations Suplemental Test Procedures*/
            var listStpRelations = context.test_procedure_test_suplemental.Where(x => x.Test_Procedure_Id == testprocedureId).ToList();

            String stepstr = JsonConvert.SerializeObject(listSteps);
            String tcstr = JsonConvert.SerializeObject(tp);
            String tagstr = JsonConvert.SerializeObject(listTags);
            String reqstr = JsonConvert.SerializeObject(listRelations);
            String stpstr = JsonConvert.SerializeObject(listStpRelations);

            String content = "!&&&@@!TestProcedure:!&&&@@!" + tcstr + "!&&&@@!Steps:!&&&@@!" + stepstr + "!&&&@@!Tags:!&&&@@!" + tagstr + "!&&&@@!Req:!&&&@@!" + reqstr + "!&&&@@!Stp:!&&&@@!" + stpstr;
            ChangeLog cl = new ChangeLog();
            cl.Content = content;
            cl.User = user;
            cl.Date = DateTime.UtcNow;
            cl.Active = true;
            var last = context.Test_ChangeLogs.Where(x => x.Test_Procedure_Id == testprocedureId).OrderByDescending(y => y.Change_Log_Id).FirstOrDefault();
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

        private ChangeLog AddTestChangeLog(ChangeLog changeLog, TestProcedure tp)
        {
            var cl = context.ChangeLogs.Where(x => x.Content == changeLog.Content &&
                                          x.Date == changeLog.Date &&
                                          x.User == changeLog.User &&
                                          x.Version == changeLog.Version).FirstOrDefault();
            Test_ChangeLog tcl = new Test_ChangeLog();
            tcl.Test_Procedure_Id = tp.Test_Procedure_Id;
            tcl.Change_Log_Id = changeLog.Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return changeLog;

        }

        public ChangeLog Restore(ChangeLog change_log, string user)
        {
            var content = change_log.Content;

            /*Deserialize all the content from the change log*/
            var tc_idx = content.IndexOf("!&&&@@!TestProcedure:!&&&@@!");
            var st_idx = content.IndexOf("!&&&@@!Steps:!&&&@@!");
            var tag_idx = content.IndexOf("!&&&@@!Tags:!&&&@@!");
            var req_idx = content.IndexOf("!&&&@@!Req:!&&&@@!");
            var stp_idx = content.IndexOf("!&&&@@!Stp:!&&&@@!");
            //Test case
            var tp_values = content.Substring(tc_idx + 28, (st_idx - (tc_idx + 28)));
            TestProcedure tp_obj = JsonConvert.DeserializeObject<TestProcedure>(tp_values);
            tp_obj.Last_Editor = user;
            //Steps
            var steps_values = content.Substring(st_idx + 20, (tag_idx - (st_idx + 20)));
            List<Step> steps_obj = JsonConvert.DeserializeObject<List<Step>>(steps_values);


            //Tags
            var tags_values = content.Substring(tag_idx + 19, (req_idx - (tag_idx + 19)));
            List<Tag> tags_obj = JsonConvert.DeserializeObject<List<Tag>>(tags_values);
            //   Console.WriteLine(tags_obj);


            //Requirements
            var req_values = content.Substring(req_idx + 18, (stp_idx - (req_idx + 18)));
            List<Requirement> req_obj = JsonConvert.DeserializeObject<List<Requirement>>(req_values);

            //Suplementals 
            var sup_values = content.Substring(stp_idx + 18);
            List<Test_Procedure_Test_Suplemental> tpts_obj = JsonConvert.DeserializeObject<List<Test_Procedure_Test_Suplemental>>(sup_values);

            /* Find all the change logs and set Active to false */
            var changeLogs = (from ttcl in context.Test_ChangeLogs
                              join cl in context.ChangeLogs on ttcl.Change_Log_Id equals cl.Id
                              where ttcl.Test_Procedure_Id == tp_obj.Test_Procedure_Id
                              select cl).ToList();

            foreach (ChangeLog cl in changeLogs)
            {
                cl.Active = false;
                context.Entry(cl).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            /* Delete all relations and data from the actual test case */

            //Delete steps
            var steps = context.Steps.Where(x => x.Test_Procedure_Id == tp_obj.Test_Procedure_Id).ToList();
            foreach (Step st in steps)
            {
                context.Steps.Remove(st);
                context.SaveChanges();
            }

            //Delete tags Relationship
            /*
            var tags = context.test_tags.Where(x => x.Test_Procedure_Id == tp_obj.Test_Procedure_Id).ToList();
            foreach (Test_Tags tag in tags)
            {
                context.test_tags.Remove(tag);
                context.SaveChanges();
            }*/
            this.ExecuteQuery(String.Format("DELETE FROM TestTags WHERE Test_Procedure_Id = {0}", tp_obj.Test_Procedure_Id));
            /*
            //Delete Requirement Relationship.
            var reqs = context.RequirementsTests.Where(x => x.Test_Procedure_Id == tp_obj.Test_Procedure_Id).ToList();
            foreach (RequirementsTest rt in reqs)
            {
                context.RequirementsTests.Remove(rt);
                context.SaveChanges();
            }
            */
            this.ExecuteQuery(String.Format("DELETE FROM RequirementsTest WHERE Test_Procedure_Id = {0}", tp_obj.Test_Procedure_Id));


            //Delete Stp Relationships

            this.ExecuteQuery(String.Format("DELETE FROM Test_Procedure_Test_Suplemental WHERE Test_Procedure_Id = {0}", tp_obj.Test_Procedure_Id));


            if (tp_obj.Type == null)
            {
                tp_obj.Type = "Functional";
            }

            /* Save Test Case with the new values */
            //Test Case
            context.Entry(tp_obj).State = System.Data.Entity.EntityState.Modified;
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
                tt.Test_Procedure_Id = tp_obj.Test_Procedure_Id;
                context.test_tags.Add(tt);
                context.SaveChanges();
            }

            //Add Requirements.
            foreach (Requirement r in req_obj)
            {
                RequirementsTest rt = new RequirementsTest();
                rt.Requirement_Id = r.Id;
                rt.Test_Procedure_Id = tp_obj.Test_Procedure_Id;
                context.RequirementsTests.Add(rt);
                context.SaveChanges();
            }

            //Add Relations between STP
            foreach (Test_Procedure_Test_Suplemental tps1 in tpts_obj)
            {
                //context.test_procedure_test_suplemental.Add(tps1);
                // context.SaveChanges();
                //Test_Suplemental_Id
                this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Procedure_Id) VALUES ({0}, {1}) ", tps1.Test_Suplemental_Id, tps1.Test_Procedure_Id));

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
            tcl.Test_Procedure_Id = tp_obj.Test_Procedure_Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return change_log;
        }

        public List<ChangeLog> TestProcedureChangeLogs(int id)
        {
            try
            {
                return (from cl in context.ChangeLogs
                        join tcl in context.Test_ChangeLogs on cl.Id equals tcl.Change_Log_Id
                        where tcl.Test_Procedure_Id == id
                        select cl).OrderByDescending(x => x.Version).ToList();
            }
            catch
            {
                return null;
            }
        }

        public void DeleteRelation(int? tcid)
        {
            try
            {
                var tp = (from a in context.TestProcedures
                          where a.Test_Case_Id == tcid
                          select a).FirstOrDefault();
                tp.Test_Case_Id = null;
                context.Entry(tp).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

            }
            catch
            {
            }

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


        public bool IsAssigned(int? tpid, int? tcid)
        {
            var id = (from tp in context.TestProcedures
                      where tp.Test_Case_Id == tcid
                      select tp.Test_Procedure_Id
                        ).FirstOrDefault();

            if (id == tpid)
            {
                return false;
            }
            else
            if (id != 0)
            {
                return true;
            }
            else
            {
                return false;
            }



        }

        public bool UpdateFromTC(TestCase tc)
        {
            var test = (from tp in context.TestProcedures
                        where tp.Test_Case_Id == tc.Test_Case_Id
                        select tp).FirstOrDefault();


            if (test != null)
            {
                test.Test_Priority = tc.Test_Priority;
                test.Type = tc.Type;
                context.Entry(test).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public Requirement GetRequirementForTp(int tpId)
        {
            try
            {
                var reqId = (from req in context.Requirements
                             join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                             where rt.Test_Procedure_Id == tpId
                             select req.Id).FirstOrDefault();

                return context.Requirements.Find(reqId);

            }
            catch
            {
                return null;
            }
        }

        public List<TestProcedure> CreateTestProcedureFromTestCase(int projectId, List<TestDTO> testDTOs, string user)
        {
            try
            {


                List<TestProcedure> testProcedures = new List<TestProcedure>();
                foreach (var item in testDTOs)
                {
                    var TC = context.TestCases.Find(item.Test_Id);

                    if (TC != null)
                    {
                        var TP = (from TpRepository in context.TestProcedures
                                  where TpRepository.Test_Case_Id == TC.Test_Case_Id
                                  select TpRepository).FirstOrDefault();
                        if (TP != null)
                        {
                            //Setting The information on the Test Procedure (Update)
                            TP.Title = TC.Title;
                            TP.Type = "Automated";
                            TP.Test_Priority = TC.Test_Priority;
                            TP.Description = TC.Description;
                            TP.Test_Case_Id = TC.Test_Case_Id;
                            TP.Script_Id = item.ScriptId;
                            TP.Expected_Result = TC.Expected_Result;
                            //Update The test Procedure Info if exists
                            TP = Update(TP, user);
                            testProcedures.Add(TP);
                        }
                        else
                        {
                            //Setting The information on the Test Procedure (Create)
                            var TestProcedure = new TestProcedure();
                            TestProcedure.Title = TC.Title;
                            TestProcedure.Type = "Automated";
                            TestProcedure.Test_Priority = TC.Test_Priority;
                            TestProcedure.Description = TC.Description;
                            TestProcedure.Test_Case_Id = TC.Test_Case_Id;
                            TestProcedure.Script_Id = item.ScriptId;
                            TestProcedure.Expected_Result = TC.Expected_Result;
                            //Saving Test Procedure
                            TestProcedure = Save(TestProcedure, user);
                            //Updating de TP Number
                            TestProcedure = UpdateNumber(TestProcedure, item.reqId);
                            //Adding The relation with the Requirement
                            RequirementsTest requirementsTest = new RequirementsTest { Requirement_Id = item.reqId, Test_Procedure_Id = TestProcedure.Test_Procedure_Id };
                            context.RequirementsTests.Add(requirementsTest);
                            context.SaveChanges();
                            //Getting the Tags from the TP
                            var TagsFromTCRelation = (from tg in context.test_tags
                                                      where tg.Test_Case_Id == TC.Test_Case_Id
                                                      select tg).ToList();
                            foreach (var relatedTag in TagsFromTCRelation)
                            {

                                relatedTag.Test_Procedure_Id = TestProcedure.Test_Procedure_Id;
                                context.Entry(relatedTag).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                            }
                            testProcedures.Add(TestProcedure);
                        }

                    }
                }

                return testProcedures;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return null;
            }

        }

    }
}
