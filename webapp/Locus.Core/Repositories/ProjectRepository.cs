using Locus.Core.Context;
using Locus.Core.DTO;
using Locus.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//Repository for project operations.
namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface IProjectRepository
    {
        Project Save(Project project, string user);

        Project Update(Project project, string user);

        Project Delete(int idProject, string user);

        List<Project> GetAll();

        Project Get(int idProject);

        List<Project> GetActives(string username, bool isAdmin);

        List<Project> GetInactives();

        Project Enable(int id, string user);

        List<ChangeLog> ProjectChangeLogs(int id);

        List<ProjectDTO> GetProjectDTO();

        ChangeLog Restore(Project project, int version, string user);

        void RestoreSteps();
    }
    //Class which implements IProjectRepository's methods and use DBContext for apply operations.
    public class ProjectRepository : IProjectRepository
    {
        //Instance of Database Context
        private LocustDBContext context = new LocustDBContext();

        private readonly IRequirementRepository _requirementRepository;

        public ProjectRepository()
        {

        }
        public ProjectRepository(IRequirementRepository requirementRepository)
        {
            _requirementRepository = requirementRepository;
        }


        //Method to delete a project from the list of projects in database.
        public Project Delete(int idProject, string user)
        {
            try
            {
                var project = context.Projects.Find(idProject);
                context.Projects.Remove(project);
                context.SaveChanges();

                var changeLog = AddChangeLog(project, user);
                AddTestChangeLog(changeLog, project);

                return project;
            }
            catch
            {
                return null;
            }
        }

        public Project Enable(int id, string user)
        {
            try
            {
                var project = context.Projects.Find(id);
                project.Status = true;
                context.Entry(project).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                var changeLog = AddChangeLog(project, user);
                AddTestChangeLog(changeLog, project);

                return project;
            }
            catch
            {
                return null;
            }
        }

        //Method to get a project from the database which id coincide with the parameter
        public Project Get(int idProject)
        {
            try
            {
                return context.Projects.Find(idProject);
            }
            catch
            {
                return null;
            }

        }

        public List<Project> GetActives(string userName, bool isAdmin)
        {
            if (isAdmin)
            {
                return context.Projects.Where(x => x.Status == true).ToList();
            }

            User user = context.Users.Where(u => u.UserName == userName).FirstOrDefault();
            if (!user.Role.Equals("TBP") && !user.Role.Equals("VBP") && !user.Role.Equals("BA"))
            {
                return context.Projects.Where(x => x.Status == true).ToList();
            }
            else
            {
                return ExecuteQueries(string.Format("SELECT p.id,p.Name,p.Axosoft_Project_Id,p.Image,p.Description FROM [dbo].[Project] AS p JOIN [dbo].[UsersProjects] up ON up.ProjectId=p.Id WHERE up.UserId={0} AND p.Status=1;", user.Id));
            }

        }


        private List<Project> ExecuteQueries(string query)
        {
            List<Project> projects = new List<Project>();
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
                                Project project = new Project()
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Image = reader.GetString(3),
                                    Description = reader.GetString(4),
                                };
                                projects.Add(project);
                            }
                        }
                    }
                }
                return projects;
            }
            catch
            {
                return null;
            }
        }

        //Method to obtain all of the projects from database.
        public List<Project> GetAll()
        {
            try
            {
                return context.Projects.ToList();
            }
            catch
            {
                return null;
            }
        }

        //Method to obtain all of the projects from database.
        public List<ProjectDTO> GetProjectDTO()
        {
            List<Project> projectList = context.Projects.Where(x => x.Status == true).ToList();
            List<ProjectDTO> newListProjects = new List<ProjectDTO>();
            ProjectDTO myProjectDTO;
            foreach (var item in projectList)
            {
                myProjectDTO = new ProjectDTO();
                myProjectDTO.Id = item.Id;
                myProjectDTO.Name = item.Name;



                newListProjects.Add(myProjectDTO);

            }


            return newListProjects;
        }


        public List<Project> GetInactives()
        {
            return context.Projects.Where(x => x.Status == false).ToList();
        }

        private Project SetDefaultImage(Project project)
        {
            if (project.Image == null)
            {
                project.Image = "default-project.png";
            }
            return project;
        }

        //Method to save a new project in database.
        public Project Save(Project project, string user)
        {

            project = SetDefaultImage(project);
            context.Projects.Add(project);
            context.SaveChanges();

            var proj = context.Projects.Where(x => x.Name == project.Name &&
                                  x.Status == project.Status &&
                                  x.Image == project.Image &&
                                  x.Description == project.Description).FirstOrDefault();

            var changeLog = AddChangeLog(project, user);
            AddTestChangeLog(changeLog, proj);
            return project;

        }

        private ChangeLog AddChangeLog(Project project, string user)
        {
            var proj = context.Projects.Where(x => x.Name == project.Name &&
                                          x.Status == project.Status &&
                                          x.Image == project.Image &&
                                          x.Description == project.Description).FirstOrDefault();


            var active = (from ch in context.ChangeLogs
                          join tcl in context.Test_ChangeLogs on ch.Id equals tcl.Change_Log_Id
                          where tcl.Project_Id == proj.Id && ch.Active == true
                          select ch).FirstOrDefault();
            if (active != null)
            {
                active.Active = false;
                context.Entry(active).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            String projectString = JsonConvert.SerializeObject(proj);
            ChangeLog cl = new ChangeLog();
            cl.Content = projectString;
            cl.User = user;
            cl.Date = DateTime.UtcNow;
            cl.Active = true;
            var last = context.Test_ChangeLogs.Where(x => x.Project_Id == proj.Id).OrderByDescending(y => y.Change_Log_Id).FirstOrDefault();
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

        private Test_ChangeLog AddTestChangeLog(ChangeLog changeLog, Project proj)
        {
            var cl = context.ChangeLogs.Where(x => x.Content == changeLog.Content &&
                                          x.Date == changeLog.Date &&
                                          x.User == changeLog.User &&
                                          x.Version == changeLog.Version).FirstOrDefault();
            Test_ChangeLog tcl = new Test_ChangeLog();
            tcl.Project_Id = proj.Id;
            tcl.Change_Log_Id = changeLog.Id;
            context.Test_ChangeLogs.Add(tcl);
            context.SaveChanges();
            return tcl;

        }

        private static System.Drawing.Image GetImg(String url)
        {
            System.Drawing.Image tmpimg = null;
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            return tmpimg = System.Drawing.Image.FromStream(stream);
        }

        //Method to update a project from the database.
        public Project Update(Project project, string user)
        {
            try
            {
                context.Entry(project).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                var changeLog = AddChangeLog(project, user);
                AddTestChangeLog(changeLog, project);

                return project;
            }
            catch
            {
                return null;
            }
        }

        public List<ChangeLog> ProjectChangeLogs(int id)
        {
            try
            {
                return (from cl in context.ChangeLogs
                        join tcl in context.Test_ChangeLogs on cl.Id equals tcl.Change_Log_Id
                        where tcl.Project_Id == id
                        select cl).OrderByDescending(x => x.Version).ToList();
            }
            catch
            {
                return null;
            }


        }

        public ChangeLog Restore(Project project, int version, string user)
        {
            try
            {
                context.Entry(project).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                var cl = (from changeLog in context.ChangeLogs
                          join testCL in context.Test_ChangeLogs on changeLog.Id equals testCL.Change_Log_Id
                          where testCL.Project_Id == project.Id && changeLog.Active == true
                          select changeLog).FirstOrDefault();
                cl.Active = false;
                context.Entry(cl).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                var change = AddChangeLog(project, user);
                AddTestChangeLog(change, project);
                return change;
            }
            catch
            {
                return null;
            }
        }


        public void RestoreSteps()
        {
            context.test_procedure_test_suplemental.RemoveRange(context.test_procedure_test_suplemental);
            context.SaveChanges();
            try
            {


                List<Project> ProjectList = (from proj in context.Projects
                                             select proj).ToList();

                foreach (var projects in ProjectList)
                {
                    List<Requirement> requirements = _requirementRepository.GetProject(projects.Id);
                    List<TestProcedure> testProceduresExcluded = new List<TestProcedure>();
                    List<TestProcedure> testProceduresExcludedInactives = new List<TestProcedure>();
                    List<TestScenario> testScenariosExcluded = new List<TestScenario>();
                    List<TestScenario> testScenariosExcludedInactives = new List<TestScenario>();

                    

                    foreach (var req in requirements)
                    {
                        List<TestProcedure> testProcedures = _requirementRepository.GetAllTestProcedure(req.Id);
                        List<TestProcedure> testProceduresInactives = _requirementRepository.GetAllTestProcedureInactives(req.Id);
                        List<TestScenario> testScenarios = _requirementRepository.GetAllTestScenario(req.Id);
                        List<TestScenario> testScenariosInactives = _requirementRepository.GetAllTestScenarioInactives(req.Id);


                        foreach (var testprocedure in testProcedures)
                        {
                            bool IsExcluded = false;
                            foreach (var verifyTp in testProceduresExcluded)
                            {
                                if (testprocedure.tp_number == verifyTp.tp_number)
                                {
                                    IsExcluded = true;
                                }

                            }

                            if (IsExcluded == false)
                            {
                                var steps = context.Steps.Where(x => x.Test_Procedure_Id == testprocedure.Test_Procedure_Id).ToList();
                                List<string> stpExcluded = new List<string>();
                                foreach (var step in steps)
                                {
                                    int aux = step.action.IndexOf("STP_");
                                    if (aux != -1)
                                    {
                                        string stpNumber = step.action.Substring(aux, (step.action.Length - aux));
                                        bool ExcludeStep = false;
                                        foreach (var number in stpExcluded)
                                        {
                                            if (stpNumber.Equals(number))
                                            {
                                                ExcludeStep = true;
                                            }
                                        }

                                        if(ExcludeStep == false)
                                        {
                                            var stp = (from sup in context.TestSuplementals
                                                       where sup.stp_number == stpNumber &&
                                                       sup.Project_Id == projects.Id
                                                       select sup.Test_Suplemental_Id).FirstOrDefault();
                                            Test_Procedure_Test_Suplemental test = new Test_Procedure_Test_Suplemental();


                                            this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Procedure_Id, Status) VALUES ({0},{1},1)", stp, testprocedure.Test_Procedure_Id));

                                            stpExcluded.Add(stpNumber);

                                        }
                                     
                                    }

                                }

                                testProceduresExcluded.Add(testprocedure);
                            }

                        }


                        foreach (var testScenario in testScenarios)
                        {
                            bool IsExcluded = false;
                            foreach (var verifyTp in testScenariosExcluded)
                            {
                                if (testScenario.ts_number == verifyTp.ts_number)
                                {
                                    IsExcluded = true;
                                }

                            }

                            if (IsExcluded == false)
                            {
                                var steps = context.Steps.Where(x => x.Test_Scenario_Id == testScenario.Test_Scenario_Id).ToList();
                                List<string> stpExcluded = new List<string>();
                                foreach (var step in steps)
                                {
                                    int aux = step.action.IndexOf("STP_");
                                    if (aux != -1)
                                    {
                                        string stpNumber = step.action.Substring(aux, (step.action.Length - aux));
                                        bool ExcludeStep = false;
                                        foreach (var number in stpExcluded)
                                        {
                                            if (stpNumber.Equals(number))
                                            {
                                                ExcludeStep = true;
                                            }
                                        }

                                        if (ExcludeStep == false)
                                        {
                                            var stp = (from sup in context.TestSuplementals
                                                       where sup.stp_number == stpNumber &&
                                                       sup.Project_Id == projects.Id
                                                       select sup.Test_Suplemental_Id).FirstOrDefault();
                                            Test_Procedure_Test_Suplemental test = new Test_Procedure_Test_Suplemental();


                                            this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Scenario_Id, Status) VALUES ({0},{1},1)", stp, testScenario.Test_Scenario_Id));

                                            stpExcluded.Add(stpNumber);

                                        }

                                    }

                                }

                                testScenariosExcluded.Add(testScenario);
                            }

                        }



                        foreach (var testprocedure in testProceduresInactives)
                        {
                            bool IsExcluded = false;
                            foreach (var verifyTp in testProceduresExcludedInactives)
                            {
                                if (testprocedure.tp_number == verifyTp.tp_number)
                                {
                                    IsExcluded = true;
                                }

                            }

                            if (IsExcluded == false)
                            {
                                var steps = context.Steps.Where(x => x.Test_Procedure_Id == testprocedure.Test_Procedure_Id).ToList();
                                List<string> stpExcluded = new List<string>();
                                foreach (var step in steps)
                                {
                                    int aux = step.action.IndexOf("STP_");
                                    if (aux != -1)
                                    {
                                        string stpNumber = step.action.Substring(aux, (step.action.Length - aux));
                                        bool ExcludeStep = false;
                                        foreach (var number in stpExcluded)
                                        {
                                            if (stpNumber.Equals(number))
                                            {
                                                ExcludeStep = true;
                                            }
                                        }

                                        if (ExcludeStep == false)
                                        {
                                            var stp = (from sup in context.TestSuplementals
                                                       where sup.stp_number == stpNumber &&
                                                       sup.Project_Id == projects.Id
                                                       select sup.Test_Suplemental_Id).FirstOrDefault();
                                            Test_Procedure_Test_Suplemental test = new Test_Procedure_Test_Suplemental();


                                            this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Procedure_Id, Status) VALUES ({0},{1},0)", stp, testprocedure.Test_Procedure_Id));

                                            stpExcluded.Add(stpNumber);

                                        }

                                    }

                                }

                                testProceduresExcludedInactives.Add(testprocedure);
                            }

                        }


                        foreach (var testScenario in testScenariosInactives)
                        {
                            bool IsExcluded = false;
                            foreach (var verifyTp in testScenariosExcludedInactives)
                            {
                                if (testScenario.ts_number == verifyTp.ts_number)
                                {
                                    IsExcluded = true;
                                }

                            }

                            if (IsExcluded == false)
                            {
                                var steps = context.Steps.Where(x => x.Test_Scenario_Id == testScenario.Test_Scenario_Id).ToList();
                                List<string> stpExcluded = new List<string>();
                                foreach (var step in steps)
                                {
                                    int aux = step.action.IndexOf("STP_");
                                    if (aux != -1)
                                    {
                                        string stpNumber = step.action.Substring(aux, (step.action.Length - aux));
                                        bool ExcludeStep = false;
                                        foreach (var number in stpExcluded)
                                        {
                                            if (stpNumber.Equals(number))
                                            {
                                                ExcludeStep = true;
                                            }
                                        }

                                        if (ExcludeStep == false)
                                        {
                                            var stp = (from sup in context.TestSuplementals
                                                       where sup.stp_number == stpNumber &&
                                                       sup.Project_Id == projects.Id
                                                       select sup.Test_Suplemental_Id).FirstOrDefault();
                                            Test_Procedure_Test_Suplemental test = new Test_Procedure_Test_Suplemental();


                                            this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Scenario_Id, Status) VALUES ({0},{1},0)", stp, testScenario.Test_Scenario_Id));

                                            stpExcluded.Add(stpNumber);

                                        }

                                    }

                                }

                                testScenariosExcludedInactives.Add(testScenario);
                            }

                        }

                    }



               
                }
            }


                

    

    
                

            

            catch (Exception e)
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

    }
}
