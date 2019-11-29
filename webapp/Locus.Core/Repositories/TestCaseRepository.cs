using Locus.Core.Context;
using Locus.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Repository for Test Case operations.
namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface ITestCaseRepository
    {
        TestCase Save(TestCase testCase, string user);

        TestCase Update(TestCase testCase, string user);

        TestCase Delete(int idTestCase);

        TestCase Enable(int idTestCase);

        List<TestCase> GetAll();

        TestCase Get(int idTestCase);

        TestCase GetLastOne(int idReq, string creator, string date);

        TestCase GetLastTestCase(string creator, string date);

        TestCase UpdateNumber(TestCase testcase, int idReq);

        Project GetProject(int idtc);

        Requirement GetRequirement(int idtc);

        Project GetProjectRequirement(int idreq);

        ChangeLog AddChangeLog(int testprocedureId, string user);

        List<ChangeLog> TestCaseChangeLogs(int id);

        ChangeLog Restore(ChangeLog change_log, string user);

        Requirement GetRequirementForTc(int tcId);
    }
    //Class which implements ITestCaseRepository's methods and use DBContext for apply operations.
    public class TestCaseRepository : ITestCaseRepository
    {
        //Instance of Database Context
        LocustDBContext context = new LocustDBContext();

        //Method to delete a TestCase from the list of TestCases in database.
        public TestCase Delete(int idTestCase)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.TestCases.Find(idTestCase);
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

        //Method to enable a TestCase from the list of TestCases in database.
        public TestCase Enable(int idTestCase)
        {
            try
            {
                var newContext = new LocustDBContext();
                var req = newContext.TestCases.Find(idTestCase);
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



        //Method to get a TestCase from the database which id coincide with the parameter
        public TestCase Get(int idTestCase)
        {
            try
            {
                return context.TestCases.Find(idTestCase);
            }
            catch
            {
                return null;
            }
        }

        //Method to obtain all of the TestCases from database.
        public List<TestCase> GetAll()
        {
            try
            {
                return context.TestCases.ToList();
            }
            catch
            {
                return null;
            }
        }

        public TestCase GetLastOne(int idReq, string creator, string date)
        {
            try
            {
                TestCase aux1 = context.TestCases
                .Where(r => r.RequirementsTests.Where(x => x.Requirement_Id == idReq).Count() > 0)
                .Where(r => r.Test_Case_Creator == creator)
                .OrderByDescending(r => r.Test_Case_Id)
                .FirstOrDefault();

                return aux1;
            }
            catch
            {
                return null;
            }
        }

        public TestCase GetLastTestCase(string creator, string date)
        {
            try
            {
                TestCase aux1 = context.TestCases.Where(r => r.Test_Case_Creator == creator).OrderByDescending(O => O.Test_Case_Id).FirstOrDefault();

                return aux1;
            }
            catch
            {
                return null;
            }
        }

        //Method to save a new TestCase in database.
        public TestCase Save(TestCase test, string user)
        {
            test.Status = true;
            test.Creation_Date = DateTime.UtcNow;
            test.Test_Case_Creator = user;
            test.Last_Editor = user;
            context.TestCases.Add(test);
            context.SaveChanges();
            return GetLastTestCase(test.Test_Case_Creator, "today");
        }

        public TestCase UpdateNumber(TestCase testCase, int reqId)
        {
            var newContext = new LocustDBContext();

            try
            {
                var projectId = (from pr in context.Projects
                                 join requ in context.Requirements on pr.Id equals requ.Project_Id
                                 where requ.Id == reqId
                                 select pr.Id).FirstOrDefault();

                int id = Convert.ToInt32(projectId);

                var testCases =
                 (from tc in context.TestCases
                  join rt in context.RequirementsTests on tc.Test_Case_Id equals rt.Test_Case_Id
                  join r in context.Requirements on rt.Requirement_Id equals r.Id
                  join p in context.Projects on r.Project_Id equals p.Id
                  where p.Id == id
                  select tc).OrderByDescending(x => x.Test_Case_Id).FirstOrDefault();


                var tests =
                 (from tc in newContext.TestCases
                  join rt in newContext.RequirementsTests on tc.Test_Case_Id equals rt.Test_Case_Id
                  join r in newContext.Requirements on rt.Requirement_Id equals r.Id
                  join p in newContext.Projects on r.Project_Id equals p.Id
                  where p.Id == id
                  select tc).OrderBy(x => x.Test_Case_Id).ToList();

                int tcnumber = 0;

                if (tests.Count + 1 == 1)
                {
                    tcnumber = 0;
                }
                else
                {
                    var OneBeforeLast = tests.ElementAtOrDefault((tests.Count + 1) - 2);
                    tcnumber = Convert.ToInt32(OneBeforeLast.tc_number.Substring(3));
                }

                testCase.tc_number = "TC_" + Convert.ToString(tcnumber + 1);
                newContext.Entry(testCase).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                return testCase;
            }
            catch
            {
                return null;
            }

        }

        public ChangeLog AddChangeLog(int testCaseId, string user)
        {
            System.Threading.Thread.Sleep(2000);
            var active = (from ch in context.ChangeLogs
                          join tcl in context.Test_ChangeLogs on ch.Id equals tcl.Change_Log_Id
                          where tcl.Test_Case_Id == testCaseId && ch.Active == true
                          select ch).FirstOrDefault();
            if (active != null)
            {
                active.Active = false;
                context.Entry(active).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            TestCase tc = context.TestCases.Find(testCaseId);
            /*Get steps*/
            List<Step> listSteps = context.Steps.Where(x => x.Test_Case_Id == testCaseId).ToList();
            /*Get Steps*/
            var listTags = (from tags in context.Tags
                            join testTags in context.test_tags on tags.id equals testTags.Tag_Id
                            where testTags.Test_Case_Id == testCaseId
                            select tags).ToList();

            /*List relations with requirements*/
            var listRelations = (from req in context.Requirements
                                 join testReq in context.RequirementsTests on req.Id equals testReq.Requirement_Id
                                 where testReq.Test_Case_Id == testCaseId
                                 select req).ToList();
            
            String stepstr = JsonConvert.SerializeObject(listSteps);
            String tcstr = JsonConvert.SerializeObject(tc);
            
            String tagstr = JsonConvert.SerializeObject(listTags);
            String reqstr = JsonConvert.SerializeObject(listRelations);

            String content = "!&&&@@!TestCase:!&&&@@!" + tcstr + "!&&&@@!Steps:!&&&@@!" + stepstr + "!&&&@@!Tags:!&&&@@!" + tagstr + "!&&&@@!Req:!&&&@@!" + reqstr;
            ChangeLog cl = new ChangeLog();
            cl.Content = content;
            
            cl.User = user;
            
            //cl.User = User.Identity.Name;
            cl.Date = DateTime.UtcNow;
            cl.Active = true;
            var last = context.Test_ChangeLogs.Where(x => x.Test_Case_Id == testCaseId).OrderByDescending(y => y.Change_Log_Id).FirstOrDefault();
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

        private ChangeLog AddTestChangeLog(ChangeLog changeLog, TestCase tc)
        {
            var cl = context.ChangeLogs.Where(x => x.Content == changeLog.Content &&
                                          x.Date == changeLog.Date &&
                                          x.User == changeLog.User &&
                                          x.Version == changeLog.Version).FirstOrDefault();
            Test_ChangeLog tcl = new Test_ChangeLog();
            tcl.Test_Case_Id = tc.Test_Case_Id;
            tcl.Change_Log_Id = changeLog.Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return changeLog;

        }
        //Method to update a TestCase from the database.
        public TestCase Update(TestCase testCase, string user)
        {
            testCase.Last_Editor = user;
            try
            {
                var newContext = new LocustDBContext();
                newContext.Entry(testCase).State = System.Data.Entity.EntityState.Modified;
                newContext.SaveChanges();
                return testCase;
            }
            catch
            {
                return null;
            }
        }

        public Project GetProject(int idtc)
        {
            try
            {
                var projectId = (from req in context.Requirements
                                 join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                                 where rt.Test_Case_Id == idtc
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

        public Requirement GetRequirement(int reqId)
        {
            try
            {
             /*   var reqId = (from req in context.Requirements
                             join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                             where rt.Test_Case_Id == idtc
                             select req.Id).FirstOrDefault();
*/
                return context.Requirements.Find(reqId);

            }
            catch
            {
                return null;
            }
        }
        public Requirement GetRequirementForTc(int tcId)
        {
            try
            {
                  var reqId = (from req in context.Requirements
                                join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                                where rt.Test_Case_Id == tcId
                               select req.Id).FirstOrDefault();
   
                return context.Requirements.Find(reqId);

            }
            catch
            {
                return null;
            }
        }
        public List<ChangeLog> TestCaseChangeLogs(int id)
        {
            try
            {
                return (from cl in context.ChangeLogs
                        join tcl in context.Test_ChangeLogs on cl.Id equals tcl.Change_Log_Id
                        where tcl.Test_Case_Id == id
                        select cl).OrderByDescending(x => x.Version).ToList();
            }
            catch
            {
                return null;
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

        public ChangeLog Restore(ChangeLog change_log, string user)
        {

            var content = change_log.Content;
            
            /*Deserialize all the content from the change log*/
            var tc_idx = content.IndexOf("!&&&@@!TestCase:!&&&@@!");
            var st_idx = content.IndexOf("!&&&@@!Steps:!&&&@@!");
            var tag_idx = content.IndexOf("!&&&@@!Tags:!&&&@@!");
            var req_idx = content.IndexOf("!&&&@@!Req:!&&&@@!");

            //Test case
            var tc_values = content.Substring(tc_idx + 23, (st_idx - (tc_idx + 23)));
            TestCase tc_obj = JsonConvert.DeserializeObject<TestCase>(tc_values);

            //Steps
            var steps_values = content.Substring(st_idx + 20, (tag_idx - (st_idx + 20)));
            List<Step> steps_obj = JsonConvert.DeserializeObject<List<Step>>(steps_values);


            //Tags
            var tags_values = content.Substring(tag_idx + 19, (req_idx - (tag_idx + 19)));
            List<Tag> tags_obj = JsonConvert.DeserializeObject<List<Tag>>(tags_values);
            //Console.WriteLine(tags_obj);
            tc_obj.Last_Editor = user;

            //Requirements
            var req_values = content.Substring(req_idx + 18);
            List<Requirement> req_obj = JsonConvert.DeserializeObject<List<Requirement>>(req_values);


            /* Find all the change logs and set Active to false */
            var changeLogs = (from ttcl in context.Test_ChangeLogs
                              join cl in context.ChangeLogs on ttcl.Change_Log_Id equals cl.Id
                              where ttcl.Test_Case_Id == tc_obj.Test_Case_Id
                              select cl).ToList();

            foreach (ChangeLog cl in changeLogs)
            {
                cl.Active = false;
                context.Entry(cl).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }

            /* Delete all relations and data from the actual test case */

            //Delete steps
            var steps = context.Steps.Where(x => x.Test_Case_Id == tc_obj.Test_Case_Id).ToList();
            foreach(Step st in steps)
            {
                context.Steps.Remove(st);
                context.SaveChanges();
            }

            //Delete tags Relationship
            /*var tags = context.test_tags.Where(x => x.Test_Case_Id == tc_obj.Test_Case_Id).ToList();
            foreach(Test_Tags tag in tags)
            {
                context.test_tags.Remove(tag);
                context.SaveChanges();
            }*/

            this.ExecuteQuery(String.Format("DELETE FROM TestTags WHERE Test_Case_Id = {0}", tc_obj.Test_Case_Id));

            //Delete Requirement Relationship.
            this.ExecuteQuery(String.Format("DELETE FROM RequirementsTest WHERE Test_Case_Id = {0}", tc_obj.Test_Case_Id));

 
            if (tc_obj.Type == null)
            {
                tc_obj.Type = "Functional";
            }

            /* Save Test Case with the new values */
            //Test Case
            context.Entry(tc_obj).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            //Add Steps.
            foreach(Step st in steps_obj)
            {
                context.Steps.Add(st);
                context.SaveChanges();
            }
            //Add Tags.
            foreach(Tag t in tags_obj)
            {
                Test_Tags tt = new Test_Tags();
                tt.Tag_Id = t.id;
                tt.Test_Case_Id = tc_obj.Test_Case_Id;
                context.test_tags.Add(tt);
                context.SaveChanges();
            }

            //Add Requirements.
            foreach(Requirement r in req_obj)
            {
                RequirementsTest rt = new RequirementsTest();
                rt.Requirement_Id = r.Id;
                rt.Test_Case_Id = tc_obj.Test_Case_Id;
                context.RequirementsTests.Add(rt);
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
            tcl.Test_Case_Id = tc_obj.Test_Case_Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return change_log;

        }
    }
}
