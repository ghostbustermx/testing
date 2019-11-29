using Locus.Core.Context;
using Locus.Core.DTO;
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

    public interface ITestResultRepository
    {
        TestResult Save(TestResult testResult, string user);

        TestResult Update(TestResult testResult, string user, TestResultDTO testResultDTO);

        TestResult Delete(int idTestResult);

        TestResult getForTestCase(int? idTestCase, int executionId);

        TestResult getForTestProcedure(int? idTestProcedure, int executionId);

        TestResult getForTestScenario(int? idTestScenario, int executionId);

        List<TestResult> getForExecutionGroup(int idExecutionGroup);

        TestResult SetStatus(TestResult testResult);

        AssignedStatusDTO GetCurrentHolder(int executionId, int testId, string type, string user);

        List<TestResult> CreateTestResults(int groupId, int executionId);

        List<TestResult> UpdateTestResults(int groupId, int executionId);

        List<TestResult> GetForUser(int testExecutionId, string user);

        TestResult Get(int idTestResult);

        TestResult GetToExecute(int testExecutionId, string user, string PhotoUrl);

        TestResult ReassignTestResult(TestResult testUpdated, TestResult testAssigned);

        bool RemoveUserFromExecution(int testExecutionId, string user);

        bool UpdateTitles(int testId, string newTitle, string EvidenceType);

        bool PassAll(int executionId, string tester, string action);

        bool FailAll(int executionId, string tester, string action);

        bool RemoveUsersFromExecution(int executionId, User[] userNames);
    }

    class TestResultRepository : ITestResultRepository
    {
        LocustDBContext context = new LocustDBContext();

        public TestResult Delete(int idTestResult)
        {
            throw new NotImplementedException();
        }

        public List<TestResult> getForExecutionGroup(int idExecutionGroup)
        {
            var listOfResults = (from r in context.TestResult
                                 where r.Test_Execution_Id == idExecutionGroup
                                 select r).ToList();

            return listOfResults;

        }

        public TestResult getForTestCase(int? idTestCase, int executionId)
        {
            int safeInt = idTestCase.GetValueOrDefault();
            var result = (from r in context.TestResult
                          where r.Test_Case_Id == safeInt &&
                          r.Test_Execution_Id == executionId
                          select r).FirstOrDefault();

            return result;
        }

        public TestResult getForTestProcedure(int? idTestProcedure, int executionId)
        {
            int safeInt = idTestProcedure.GetValueOrDefault();
            var result = (from r in context.TestResult
                          where r.Test_Procedure_Id == safeInt &&
                          r.Test_Execution_Id == executionId
                          select r).FirstOrDefault();

            return result;
        }

        public TestResult getForTestScenario(int? idTestScenario, int executionId)
        {
            int safeInt = idTestScenario.GetValueOrDefault();
            var result = (from r in context.TestResult
                          where r.Test_Scenario_Id == safeInt &&
                          r.Test_Execution_Id == executionId
                          select r).FirstOrDefault();
            return result;
        }

        public TestResult Save(TestResult testResult, string user)
        {

            TestResult result = Get(testResult.Test_Result_Id);


            result.CurrentHolder = testResult.CurrentHolder;
            result.Status = testResult.Status;
            result.Execution_Date = DateTime.UtcNow;
            result.Execution_Result = testResult.Execution_Result;
            result.Status = testResult.Status;
            result.PhotoUrl = testResult.PhotoUrl;
            result.Tester = user;
            result.IsTaken = testResult.IsTaken;


            if (!result.Status.Equals("Fail"))
            {


                if (result.Evidence.Equals("TS"))
                {
                    var ts = context.TestScenarios.Find(result.Test_Scenario_Id);

                    var steps = (from s in context.Steps
                                 where s.Test_Scenario_Id == ts.Test_Scenario_Id
                                 select s
                                         ).ToList();


                    var execution = "";

                    foreach (var st in steps)
                    {
                        if (st.type != null)
                        {
                            if (st.type.Equals("Expected Result"))
                            {
                                execution = execution + st.action + Environment.NewLine;
                            }
                        }

                    }
                    result.Execution_Result = execution;
                }

            }
            context.Entry(result).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return result;


        }

        public TestResult SetStatus(TestResult testResult)
        {

            var result = (from r in context.TestResult
                          where r.Test_Result_Id == testResult.Test_Result_Id
                          select r).FirstOrDefault();

            result.CurrentHolder = testResult.CurrentHolder;
            result.PhotoUrl = testResult.PhotoUrl;
            result.IsTaken = testResult.IsTaken;

            context.Entry(result).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return result;
        }

        public AssignedStatusDTO GetCurrentHolder(int executionId, int testId, string type, string user)
        {
            AssignedStatusDTO assignedStatus = new AssignedStatusDTO();
            TestResult testResult = null;
            assignedStatus.assigned = false;
            assignedStatus.message = "NoMatch";
            if (type.Equals("TC"))
            {
                testResult = getForTestCase(testId, executionId);
                if (testResult.CurrentHolder.Equals(user))
                {
                    assignedStatus.assigned = true;
                    assignedStatus.message = testResult.CurrentHolder;
                    return assignedStatus;
                }
                else { return assignedStatus; }
            }
            if (type.Equals("TP"))
            {
                testResult = getForTestProcedure(testId, executionId);
                if (testResult.CurrentHolder.Equals(user))
                {
                    assignedStatus.assigned = true;
                    assignedStatus.message = testResult.CurrentHolder;
                    return assignedStatus;
                }
                else { return assignedStatus; }
            }
            if (type.Equals("TS"))
            {
                testResult = getForTestScenario(testId, executionId);
                if (testResult.CurrentHolder.Equals(user))
                {
                    assignedStatus.assigned = true;
                    assignedStatus.message = testResult.CurrentHolder;
                    return assignedStatus;
                }
                else { return assignedStatus; }
            }

            return assignedStatus;
        }

        public TestResult Update(TestResult testResult, string user, TestResultDTO testResultDTO)
        {

            var result = context.TestResult.Find(testResult.Test_Result_Id);

            if (user.Equals(result.CurrentHolder))
            {



                result.Execution_Date = DateTime.UtcNow;
                result.Tester = user;
                result.Status = testResultDTO.Status;
                result.Execution_Result = testResultDTO.Execution_Result;
                result.IsTaken = false;
                result.PhotoUrl = null;
                result.CurrentHolder = null;
                context.Entry(result).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return result;
            }
            else
            {
                return null;
            }

        }

        public List<TestResult> CreateTestResults(int groupId, int executionId)
        {

            var TestCasesList = (from groupRel in context.ExecutionTest
                                 where groupRel.Execution_Group_Id == groupId
                                 && groupRel.Tc_Id != null
                                 select groupRel
                                 ).ToList();

            var TestProcedureList = (from groupRel in context.ExecutionTest
                                     where groupRel.Execution_Group_Id == groupId
                                     && groupRel.Tp_Id != null
                                     select groupRel
                                 ).ToList();

            var TestScenariosList = (from groupRel in context.ExecutionTest
                                     where groupRel.Execution_Group_Id == groupId
                                     && groupRel.Ts_Id != null
                                     select groupRel
                                 ).ToList();

            var TestAutomatedList = (from groupRel in context.ExecutionTest
                                     where groupRel.Execution_Group_Id == groupId
                                     && groupRel.Ta_Id != null
                                     select groupRel
                            ).ToList();


            List<TestResult> testResults = new List<TestResult>();


            foreach (var testCase in TestCasesList)
            {
                TestResult testResult = new TestResult();
                var test = (from tc in context.TestCases
                            where tc.Test_Case_Id == testCase.Tc_Id
                            select tc
                            ).FirstOrDefault();
                testResult.Test_Execution_Id = executionId;
                testResult.Test_Case_Id = testCase.Tc_Id;
                testResult.IsTaken = false;
                testResult.Status = "TBE";
                testResult.Execution_Date = DateTime.UtcNow;
                testResult.Title = test.Title;
                testResult.Identifier_number = test.tc_number;
                testResult.Evidence = "TC";
                testResults.Add(testResult);
            }

            foreach (var testProcedure in TestProcedureList)
            {
                TestResult testResult = new TestResult();
                var test = (from tp in context.TestProcedures
                            where tp.Test_Procedure_Id == testProcedure.Tp_Id
                            select tp
                            ).FirstOrDefault();
                testResult.Test_Execution_Id = executionId;
                testResult.Test_Procedure_Id = testProcedure.Tp_Id;
                testResult.IsTaken = false;
                testResult.Status = "TBE";
                testResult.Execution_Date = DateTime.UtcNow;
                testResult.Title = test.Title;
                testResult.Identifier_number = test.tp_number;
                testResult.Evidence = "TP";
                testResults.Add(testResult);
            }


            foreach (var testAutomated in TestAutomatedList)
            {
                TestResult testResult = new TestResult();
                var test = (from s in context.Scripts
                            where s.Id == testAutomated.Ta_Id
                            select s
                           ).FirstOrDefault();
                testResult.Test_Execution_Id = executionId;
                testResult.Test_Procedure_Id = testAutomated.Ta_Id;
                testResult.IsTaken = false;
                testResult.Status = "TBE";
                testResult.Execution_Date = DateTime.UtcNow;
                testResult.Title = test.Name;
                testResult.Identifier_number = "TA_" + test.Id;
                testResult.Evidence = "TA";
                testResults.Add(testResult);
            }




            context.TestResult.AddRange(testResults);
            context.SaveChanges();

            return testResults;
        }

        public TestResult Get(int idTestResult)
        {
            var testResult = (from result in context.TestResult
                              where idTestResult == result.Test_Result_Id
                              select result).FirstOrDefault();

            return testResult;
        }

        public TestResult GetToExecute(int testExecutionId, string user, string PhotoUrl)
        {
            var testResult = (from result in context.TestResult
                              where result.IsTaken != true && result.Status == "TBE"
                              && result.Test_Execution_Id == testExecutionId
                              select result).OrderBy(x => x.Test_Result_Id).FirstOrDefault();
            if (testResult != null)
            {
                testResult.PhotoUrl = PhotoUrl;
                testResult.IsTaken = true;
                testResult.CurrentHolder = user;
                context.Entry(testResult).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }


            /*   TestResult aresult = context.TestResult.Find(testResult.Test_Result_Id);*/
            return testResult;

        }

        public TestResult ReassignTestResult(TestResult testUpdated, TestResult testAssigned)
        {
            var testResultR = (from r1 in context.TestResult
                               where r1.Test_Result_Id == testUpdated.Test_Result_Id
                               select r1).FirstOrDefault();
            testResultR.PhotoUrl = testUpdated.PhotoUrl;
            testResultR.CurrentHolder = testUpdated.CurrentHolder;
            testResultR.IsTaken = false;
            context.Entry(testResultR).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            context.TestResult.Where(x => x.CurrentHolder.Equals(testUpdated.CurrentHolder)
            && x.Test_Execution_Id == testUpdated.Test_Execution_Id)
            .ToList()
            .ForEach(r =>
            {
                r.CurrentHolder = null;
                r.IsTaken = false;
                r.PhotoUrl = null;
            });
            context.SaveChanges();

            var testAssignedN = (from r2 in context.TestResult
                                 where r2.Test_Result_Id == testAssigned.Test_Result_Id
                                 select r2).FirstOrDefault();
            testAssignedN.PhotoUrl = testAssigned.PhotoUrl;
            testAssignedN.IsTaken = true;
            testAssignedN.CurrentHolder = testAssigned.CurrentHolder;
            context.Entry(testAssignedN).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();


            return testAssignedN;

        }

        public List<TestResult> GetForUser(int testExecutionId, string user)
        {
            var results = (from r in context.TestResult
                           where r.Test_Execution_Id == testExecutionId && r.CurrentHolder == user
                           select r).ToList();
            return results;
        }

        public bool RemoveUserFromExecution(int testExecutionId, string user)
        {
            context.TestResult.Where(x => x.CurrentHolder.Equals(user)
          && x.Test_Execution_Id == testExecutionId)
          .ToList()
          .ForEach(r =>
          {
              r.CurrentHolder = null;
              r.IsTaken = false;
              r.PhotoUrl = null;
          });
            context.SaveChanges();

            return true;
        }

        public List<TestResult> UpdateTestResults(int groupId, int executionId)
        {

            //Actual List of Results
            List<TestResult> testResults = new List<TestResult>();
            var TpResultsToBeRemoved = (from results in context.TestResult
                                        where results.Test_Execution_Id == executionId
                                        && !(from groups in context.ExecutionTest
                                             where groups.Execution_Group_Id == groupId
                                             select groups.Tp_Id).Contains(results.Test_Procedure_Id)
                                        select results
                                            ).ToList();


            var TcResultsToBeRemoved = (from results in context.TestResult
                                        where results.Test_Execution_Id == executionId
                                        && !(from groups in context.ExecutionTest
                                             where groups.Execution_Group_Id == groupId
                                             select groups.Tc_Id).Contains(results.Test_Case_Id)
                                        select results
                                            ).ToList();

            var TsResultsToBeRemoved = (from results in context.TestResult
                                        where results.Test_Execution_Id == executionId
                                        && !(from groups in context.ExecutionTest
                                             where groups.Execution_Group_Id == groupId
                                             select groups.Ts_Id).Contains(results.Test_Scenario_Id)
                                        select results
                                            ).ToList();




            if (TpResultsToBeRemoved != null)
                context.TestResult.RemoveRange(TpResultsToBeRemoved);
            if (TcResultsToBeRemoved != null)
                context.TestResult.RemoveRange(TcResultsToBeRemoved);
            if (TsResultsToBeRemoved != null)
                context.TestResult.RemoveRange(TsResultsToBeRemoved);
            context.SaveChanges();


            var TestProcedureRelations = (from groupRel in context.ExecutionTest
                                          where groupRel.Execution_Group_Id == groupId
                                          && groupRel.Tp_Id != null
                                         && !(from results in context.TestResult
                                              where results.Test_Execution_Id == executionId
                                              select results.Test_Procedure_Id).Contains(groupRel.Tp_Id)
                                          select groupRel
                                  ).ToList();

            var TestCaseRelations = (from groupRel in context.ExecutionTest
                                     where groupRel.Execution_Group_Id == groupId
                                     && groupRel.Tc_Id != null
                                    && !(from results in context.TestResult
                                         where results.Test_Execution_Id == executionId
                                         select results.Test_Case_Id).Contains(groupRel.Tc_Id)
                                     select groupRel
                                  ).ToList();

            var TestScenarioRelations = (from groupRel in context.ExecutionTest
                                         where groupRel.Execution_Group_Id == groupId
                                         && groupRel.Ts_Id != null
                                        && !(from results in context.TestResult
                                             where results.Test_Execution_Id == executionId
                                             select results.Test_Scenario_Id).Contains(groupRel.Ts_Id)
                                         select groupRel
                                  ).ToList();


            foreach (var testProcedure in TestProcedureRelations)
            {
                TestResult testResult = new TestResult();
                var test = (from tp in context.TestProcedures
                            where tp.Test_Procedure_Id == testProcedure.Tp_Id
                            select tp
                            ).FirstOrDefault();
                testResult.Test_Execution_Id = executionId;
                testResult.Test_Procedure_Id = testProcedure.Tp_Id;
                testResult.IsTaken = false;
                testResult.Status = "TBE";
                testResult.Execution_Date = DateTime.UtcNow;
                testResult.Title = test.Title;
                testResult.Identifier_number = test.tp_number;
                testResult.Evidence = "TP";
                testResults.Add(testResult);
            }

            foreach (var testCase in TestCaseRelations)
            {
                TestResult testResult = new TestResult();
                var test = (from tc in context.TestCases
                            where tc.Test_Case_Id == testCase.Tc_Id
                            select tc
                            ).FirstOrDefault();
                testResult.Test_Execution_Id = executionId;
                testResult.Test_Case_Id = testCase.Tc_Id;
                testResult.IsTaken = false;
                testResult.Status = "TBE";
                testResult.Execution_Date = DateTime.UtcNow;
                testResult.Title = test.Title;
                testResult.Identifier_number = test.tc_number;
                testResult.Evidence = "TC";
                testResults.Add(testResult);
            }

            foreach (var testScenario in TestScenarioRelations)
            {
                TestResult testResult = new TestResult();
                var test = (from ts in context.TestScenarios
                            where ts.Test_Scenario_Id == testScenario.Ts_Id
                            select ts
                           ).FirstOrDefault();
                testResult.Test_Execution_Id = executionId;
                testResult.Test_Scenario_Id = testScenario.Ts_Id;
                testResult.IsTaken = false;
                testResult.Status = "TBE";
                testResult.Execution_Date = DateTime.UtcNow;
                testResult.Title = test.Title;
                testResult.Identifier_number = test.ts_number;
                testResult.Evidence = "TS";
                testResults.Add(testResult);
            }

            context.TestResult.AddRange(testResults);
            context.SaveChanges();


            return testResults;
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

        public bool UpdateTitles(int testId, string newTitle, string EvidenceType)
        {
            switch (EvidenceType)
            {
                case "TC":
                    context.TestResult.Where(test => test.Test_Case_Id == testId).ToList().ForEach(t => { t.Title = newTitle; });
                    break;
                case "TP":
                    context.TestResult.Where(test => test.Test_Procedure_Id == testId).ToList().ForEach(t => { t.Title = newTitle; });
                    break;
                case "TS":
                    context.TestResult.Where(test => test.Test_Scenario_Id == testId).ToList().ForEach(t => { t.Title = newTitle; });
                    break;
            }

            context.SaveChanges();
            return true;

        }

        public bool PassAll(int executionId, string tester, string action)
        {
            if (action.Equals("All"))
            {

                context.TestResult.Where(results => results.Test_Execution_Id == executionId && results.Status != "Pass").ToList().ForEach(r =>
                  {
                      r.Status = "Pass";
                      r.PhotoUrl = null;
                      r.Tester = tester;
                      r.Execution_Date = DateTime.UtcNow;
                      r.IsTaken = false;
                      r.CurrentHolder = null;
                      switch (r.Evidence)
                      {
                          case "TC":
                              r.Execution_Result = (from tc in context.TestCases
                                                    where r.Test_Case_Id == tc.Test_Case_Id
                                                    select tc.Expected_Result).FirstOrDefault();
                              break;
                          case "TP":
                              r.Execution_Result = (from tp in context.TestProcedures
                                                    where r.Test_Procedure_Id == tp.Test_Procedure_Id
                                                    select tp.Expected_Result).FirstOrDefault();
                              break;
                          case "TS":
                              r.Execution_Result = null;
                              break;
                      }
                  });
            }

            else
            {
                context.TestResult.Where(results => results.Test_Execution_Id == executionId && results.Status == "TBE").ToList().ForEach(r =>
                {
                    r.Status = "Pass";
                    r.PhotoUrl = null;
                    r.Tester = tester;
                    r.Execution_Date = DateTime.UtcNow;
                    r.IsTaken = false;
                    r.CurrentHolder = null;
                    switch (r.Evidence)
                    {
                        case "TC":
                            r.Execution_Result = (from tc in context.TestCases
                                                  where r.Test_Case_Id == tc.Test_Case_Id
                                                  select tc.Expected_Result).FirstOrDefault();
                            break;
                        case "TP":
                            r.Execution_Result = (from tp in context.TestProcedures
                                                  where r.Test_Procedure_Id == tp.Test_Procedure_Id
                                                  select tp.Expected_Result).FirstOrDefault();
                            break;
                        case "TS":
                            r.Execution_Result = null;
                            break;
                    }
                });
            }
            context.SaveChanges();
            return true;
        }

        public bool FailAll(int executionId, string tester, string action)
        {
            if (action.Equals("All"))
            {
                context.TestResult.Where(results => results.Test_Execution_Id == executionId && results.Status != "Fail").ToList().ForEach(r =>
                  {
                      r.Status = "Fail";
                      r.PhotoUrl = null;
                      r.Tester = tester;
                      r.Execution_Date = DateTime.UtcNow;
                      r.IsTaken = false;
                      r.CurrentHolder = null;
                      r.Execution_Result = "The Execution Result didn't match the expected result";
                  });
            }
            else
            {
                context.TestResult.Where(results => results.Test_Execution_Id == executionId && results.Status == "TBE").ToList().ForEach(r =>
                {
                    r.Status = "Fail";
                    r.PhotoUrl = null;
                    r.Tester = tester;
                    r.Execution_Date = DateTime.UtcNow;
                    r.IsTaken = false;
                    r.CurrentHolder = null;
                    r.Execution_Result = "The Execution Result didn't match the expected result";
                });
            }

            context.SaveChanges();
            return true;
        }

        public bool RemoveUsersFromExecution(int executionId, User[] userNames)
        {
            if (userNames.Length >= 1)
            {
                foreach (var item in userNames)
                {
                    var testResult = (from result in context.TestResult
                                      where result.Test_Execution_Id == executionId && item.UserName.Equals(result.CurrentHolder)
                                      select result).FirstOrDefault();
                    if (testResult != null)
                    {
                        testResult.CurrentHolder = null;
                        testResult.PhotoUrl = null;
                        testResult.IsTaken = false;

                        context.Entry(testResult).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }


                }
            }
            return true;

        }
    }
}
