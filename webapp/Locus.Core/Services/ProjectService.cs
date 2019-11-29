using Locus.Core.DTO;
using Locus.Core.Helpers;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

//Service for project operations.
namespace Locus.Core.Services
{
    //Interface which contains methods for each CRUD operation
    public interface IProjectService
    {
        
        Project Save(Project project, string user);

        Project Update(Project project, string user);

        Project Delete(int idProject, string user);

        List<Project> GetAll();

        Project Get(int idProject);

        List<Project> GetActives(string username,bool isAdmin);

        List<Project> GetInactives();

        Project Enable(int id, string user);

        void uploadFile();

        List<ChangeLog> ProjectChangeLogs(int id);

        ChangeLog Restore(Project project, int version, string user);

        List<ProjectDTO> GetProjectDTO();

        byte[] GenerateProjectReportExcel(int id, string date, int executionId);

        byte[] DownloadExecutionReport(int id, string date, int executionId);

        AssignedStatusDTO ExcelRequestor();

        void RestoreSteps();
    }
    //Class which implements IProjectService's methods and instance to IProjectRepository
    public class ProjectService : IProjectService
    {
        //Instance of IProjectRepository
        private readonly IProjectRepository _projectRepository;

        

        //Constructor of ProjectService and initialize projectRepository
        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        //Delete method calls to Delete Method From projectRepository.
        public Project Delete(int idProject, string user)
        {
           return  _projectRepository.Delete(idProject, user);
        }

        public Project Enable(int id, string user)
        {
            return _projectRepository.Enable(id, user);
        }

        public byte[] GenerateProjectReportExcel(int id, string date, int executionId)
        {
            Project project = _projectRepository.Get(id);
            ExcelCreator excelCreator = new ExcelCreator();

            return excelCreator.GenerateExcel(project, date, executionId);
        }

        public byte[] DownloadExecutionReport(int id, string date, int executionId)
        {
            Project project = _projectRepository.Get(id);
            ExcelCreator excelCreator = new ExcelCreator();

            return excelCreator.GenerateExecutionReport(project, date, executionId);
        }

        //Get method calls to Get Method From projectRepository.
        public Project Get(int idProject)
        {
            return _projectRepository.Get(idProject);
        }

        public List<Project> GetActives(string username,bool isAdmin)
        {
            return _projectRepository.GetActives(username, isAdmin);
        }

        //GetAll method calls to GetAll Method From projectRepository.
        public List<Project> GetAll()
        {
            return _projectRepository.GetAll();
        }

        public List<Project> GetInactives()
        {
            return _projectRepository.GetInactives();
        }

        public List<ProjectDTO> GetProjectDTO()
        {
            return _projectRepository.GetProjectDTO();
        }

        public List<ChangeLog> ProjectChangeLogs(int id)
        {
            return _projectRepository.ProjectChangeLogs(id);
        }

        public ChangeLog Restore(Project project, int version, string user)
        {
            return _projectRepository.Restore(project, version, user);
        }

        //Save method calls to Save Method From projectRepository.
        public Project Save(Project project, string user)
        {
            return _projectRepository.Save(project, user);
        }
        //Update method calls to Update Method From projectRepository.
        public Project Update(Project project, string user)
        {
            return _projectRepository.Update(project, user);
        }

        public void uploadFile()
        {

            var httpRequest = HttpContext.Current.Request;
            string filePathImage = "";
            foreach(string file in httpRequest.Files)
            {
                var postedFile = httpRequest.Files[file];
                var path = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\img\\");
                filePathImage = file+".png";

                var filePath = path + filePathImage;
                postedFile.SaveAs(filePath);
            }
        }

        public AssignedStatusDTO ExcelRequestor()
        {
            return Helpers.ExcelRequestor.Requestor();
        }

        public void RestoreSteps()
        {
            _projectRepository.RestoreSteps();
        }
    }
}
