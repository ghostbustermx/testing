using Locus.Core.Context;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Repositories
{
    public interface ITestExecutionRepository
    {
        TestExecution Save(TestExecution testExecution);

        TestExecution Update(TestExecution testExecution);

        TestExecution ChangeState(int id,string state);

        TestExecution Get(int id);

        List<TestExecution> GetByProject(int projectId);

        bool ChangeExecutionsStatus(int groupId);

        List<TestExecution> GetForGroup(int executionId);

        TestExecution GetLastExecution(int groupId, int projectId);

        ExecutionGroup GetByTestExecution(int groupId); 
}

    }

    class TestExecutionRepository : ITestExecutionRepository
    {
        LocustDBContext context = new LocustDBContext();

        public TestExecution ChangeState(int id,string state)
        {
            var testExecution = (from execution in context.TestExecutions
                                 where execution.Test_Execution_Id == id
                                 select execution).FirstOrDefault();
            testExecution.State = state;

            context.Entry(testExecution).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return testExecution;
        }

        public TestExecution Get(int id)
        {
            return context.TestExecutions.Find(id);
        }

   

        public TestExecution Save(TestExecution testExecution)
        {
            try
            {

                testExecution.CreationDate = DateTime.UtcNow;
                
                context.TestExecutions.Add(testExecution);
                context.SaveChanges();
            }
            catch (Exception)
            {

                return null;
            }

            return testExecution;
            
        }

        public TestExecution Update(TestExecution testExecution)
        {

            context.Entry(testExecution).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return testExecution;
        }


        public List<TestExecution> GetForGroup(int executionId)
        {
            try
            {
                var listOfExecutions = (from executions in context.TestExecutions
                                        where executions.Execution_Group_Id == executionId
                                        select executions).ToList();

               
                return listOfExecutions;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public TestExecution GetLastExecution(int groupId, int projectId)
        {
            

            var test = (from ex in context.TestExecutions
                        where ex.Execution_Group_Id == groupId
                        select ex
                        ).OrderByDescending(d => d.CreationDate).FirstOrDefault();
            return test;
        }




    public ExecutionGroup GetByTestExecution(int groupId)
    {
        var executionGroup = (from groups in context.ExecutionGroups
                              join execution in context.TestExecutions
                              on groups.Execution_Group_Id equals execution.Execution_Group_Id
                              where execution.Test_Execution_Id == groupId
                              select groups
                              ).FirstOrDefault();
        return executionGroup;
    }

    public bool ChangeExecutionsStatus(int groupId)
    {
        context.TestExecutions.Where(x=> x.Execution_Group_Id == groupId).ToList().ForEach(r=>{
            if(r.State.Equals("Created")|| r.State.Equals("In progress"))
            {
                r.State = "Changed";
            }

        });

        context.SaveChanges();

        return true;
    }

    public List<TestExecution> GetByProject(int projectId)
    {

        var TestExecutionList = (from execution in context.TestExecutions
                                 join groups in context.ExecutionGroups
                                 on execution.Execution_Group_Id equals groups.Execution_Group_Id
                                 where groups.ProjectId == projectId
                                 select execution
                                 ).ToList();
        return TestExecutionList;
    }
}

