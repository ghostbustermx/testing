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

    public interface IExecutionGroupRepository
    {
        ExecutionGroup Save(ExecutionGroup executionGroup, string user);

        ExecutionGroup Update(ExecutionGroup executionGroup, string user);

        ExecutionGroup Delete(int ExecutionId, string user);

        ExecutionGroup Enable(int ExecutionId, string user);

        ExecutionGroup Get(int ExecutionId);

        List<ExecutionGroup> GetAll();

        List<ExecutionGroup> GetByProjectActives(int projectId);

        List<ExecutionGroup> GetByProjectInactives(int projectId);

        ExecutionGroup GetLastByProject(int projectId);

    }

    class ExecutionGroupRepository : IExecutionGroupRepository
    {
        LocustDBContext context = new LocustDBContext();

        public ExecutionGroup Delete(int ExecutionId, string user)
        {

            var group = context.ExecutionGroups.Find(ExecutionId);
            group.isActive = false;
            group.lastEditDate = DateTime.UtcNow;
            group.LastEditor = user;
            context.Entry(group).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return group;
        }

        public ExecutionGroup Enable(int ExecutionId, string user)
        {
            var group = context.ExecutionGroups.Find(ExecutionId);
            group.isActive = true;
            group.lastEditDate = DateTime.UtcNow;
            group.LastEditor = user;
            context.Entry(group).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return group;
        }

        public ExecutionGroup Get(int ExecutionId)
        {
            return context.ExecutionGroups.Find(ExecutionId);
        }

        public List<ExecutionGroup> GetAll()
        {
            return context.ExecutionGroups.ToList();
        }

        public List<ExecutionGroup> GetByProjectActives(int projectId)
        {
            return context.ExecutionGroups.Where(x => x.isActive == true && x.ProjectId == projectId).ToList();
        }

        public List<ExecutionGroup> GetByProjectInactives(int projectId)
        {
            return context.ExecutionGroups.Where(x => x.isActive == false && x.ProjectId == projectId).ToList();
        }



        public ExecutionGroup Save(ExecutionGroup executionGroup, string user)
        {
            executionGroup.Creator = user;
            executionGroup.Creation_Date = DateTime.UtcNow;
            context.Entry(executionGroup).State = System.Data.Entity.EntityState.Added;
            context.SaveChanges();
            return executionGroup;


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

        public ExecutionGroup Update(ExecutionGroup executionGroup, string user)
        {
            executionGroup.LastEditor = user;
            executionGroup.lastEditDate = DateTime.UtcNow;
            context.Entry(executionGroup).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
            return executionGroup;
        }

        public ExecutionGroup GetLastByProject(int projectId)
        {
            var groupId = (from gr in context.ExecutionGroups
                           where gr.ProjectId == projectId
                           select gr).OrderByDescending(d => d.Creation_Date).FirstOrDefault();

            return groupId;
        }
    }

}
