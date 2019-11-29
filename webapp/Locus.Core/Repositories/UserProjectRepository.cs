using Locus.Core.Context;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Repository for project operations.
namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface IUserProjectRepository
    {
        List<UsersProjects> GetProjectsByUser(User user);
        UsersProjects GetProjectById(int projectID, int userId);
        bool HasAccessToRequirement(int projectID, int? reqId, int userId);
        bool HasAccessToProject(int projectID, int userId);
        bool HasAccessToSTP(int projectID, int? STPId, int userId);
        bool HasAccessToTC(int projectID, int? reqId, int? TCId, int userId);
        bool HasAccessToTP(int projectID, int? reqId, int? TPId, int userId);
        bool HasAccessToTS(int projectID, int? reqId, int? TSId, int userId);
        bool HasAccessToTestEnvironment(int projectID, int? teId, int userId);
        bool HasAccessToExecutionGroup(int projectID, int? executionId, int userId);
        bool HasAccessToPlayExecution(int id, int? executionId, int? playId, int userId);


    }


    public class UserProjectRepository : IUserProjectRepository
    {
        private LocustDBContext context = new LocustDBContext();

        public UsersProjects GetProjectById(int projectID, int userId)
        {
            try
            {
                return context.UsersProjects.Where(u => u.UserId == userId && u.ProjectId == projectID).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<UsersProjects> GetProjectsByUser(User user)
        {
            try
            {
                return context.UsersProjects.Where(u => u.UserId.Equals(user.Id)).ToList();
            }
            catch
            {
                return null;
            }
        }


        public bool HasAccessToProject(int projectID, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM Project AS p JOIN UsersProjects up ON up.UserId={1} AND p.Id={0} AND up.ProjectId={0};"
                                    , projectID, userId));
        }

        public bool HasAccessToRequirement(int projectID, int? reqID, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM Project AS p JOIN Requirement AS rs ON rs.Project_Id={0} AND p.id={0} JOIN UsersProjects up on up.UserId={1} where rs.Id={2} AND up.ProjectId=p.Id;"
                          , projectID, userId, reqID));
        }


        public bool HasAccessToTestEnvironment(int projectID, int? testEnvId, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM Project AS p JOIN TestEnvironment AS te ON te.ProjectId={0} AND p.id={0} JOIN UsersProjects up on up.UserId={1} where te.Id={2} AND up.ProjectId=p.Id;"
                        , projectID, userId, testEnvId));
        }

        public bool HasAccessToExecutionGroup(int projectID, int? executionId, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM Project AS p JOIN ExecutionGroup AS ex ON ex.ProjectId={0} AND p.id={0} JOIN UsersProjects up on up.UserId={1} where ex.Execution_Group_Id={2} AND up.ProjectId=p.Id;"
                        , projectID, userId, executionId));
        }



        public bool HasAccessToSTP(int projectID, int? STPId, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM Project AS p JOIN TestSuplemental AS ts ON ts.Project_Id ={0} AND p.id ={0} JOIN UsersProjects up ON up.UserId ={1} WHERE ts.Test_Suplemental_Id ={2} AND up.ProjectId = p.Id;"
                , projectID, userId, STPId));
        }

        public bool HasAccessToTC(int projectID, int? reqId, int? TCId, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM TestCase AS tc JOIN RequirementsTest AS rt ON rt.Test_Case_Id={0} and tc.Test_Case_Id={0} JOIN Requirement r ON r.Id={1} AND rt.Requirement_Id={1} JOIN Project p ON p.Id={2} AND r.Project_Id={2} JOIN UsersProjects up ON up.ProjectId=p.Id WHERE up.UserId={3};"
                , TCId, reqId, projectID, userId));
        }

        public bool HasAccessToTP(int projectID, int? reqId, int? TPId, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM TestProcedure AS tp JOIN RequirementsTest AS rt ON rt.Test_Procedure_Id={0} and tp.Test_Procedure_Id={0} JOIN Requirement r ON r.Id={1} AND rt.Requirement_Id={1} JOIN Project p ON p.Id={2} AND r.Project_Id={2} JOIN UsersProjects up ON up.ProjectId=p.Id WHERE up.UserId={3};"
                , TPId, reqId, projectID, userId));
        }

        public bool HasAccessToTS(int projectID, int? reqId, int? TSId, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM TestScenario AS ts JOIN RequirementsTest AS rt ON rt.Test_Scenario_Id={0} AND ts.Test_Scenario_Id={0} JOIN Requirement r ON r.Id={1} AND rt.Requirement_Id={1} JOIN Project p ON p.Id={2} AND r.Project_Id={2} JOIN UsersProjects up ON up.ProjectId=p.Id WHERE up.UserId={3};"
                , TSId, reqId, projectID, userId));
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

        public bool HasAccessToPlayExecution(int id, int? executionId, int? playId, int userId)
        {
            return this.ExecuteQuery(String.Format("SELECT * FROM Project AS p JOIN ExecutionGroup AS ex ON ex.ProjectId={0} AND p.id={0} JOIN UsersProjects up on up.UserId={3} JOIN TestExecution te on te.Test_Execution_Id={2} AND te.Execution_Group_Id={1} where ex.Execution_Group_Id={1} AND up.ProjectId=p.Id;"
                       , id, executionId, playId, userId));
        }
    }
}
