using Locus.Core.Models;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Locus.Core.Repositories;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Locust.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Locus.Core.DTO;

namespace Locust.Controllers.API
{

   

    public class ProjectController : ApiController
    {
        private readonly IProjectService _projectService;
        private readonly IRequirementService _requirementService;

        
        public enum Report
        {
            EmptyReport = -1,
            LastExecution = 0
        }

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public ProjectController(IProjectService projectService, IRequirementService requirementService)
        {
            _projectService = projectService;
            _requirementService = requirementService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<Project> GetAll()
        {
            return _projectService.GetAll();
        }

        [System.Web.Http.ActionName("GetForUsersList")]
        [System.Web.Http.HttpGet]
        public List<ProjectDTO> GetForUsersList()
        {
            return _projectService.GetProjectDTO();
        }

        [System.Web.Http.ActionName("Restore")]
        [System.Web.Http.HttpPost]
        public ChangeLog Restore(Project project, int version)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _projectService.Restore(project, version, user);
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public Project Get(int id)
        {
            return _projectService.Get(id);
        }

        [System.Web.Http.ActionName("GetActives")]
        [System.Web.Http.HttpGet]
        public List<Project> GetActives()
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            bool isAdmin = UserHelper.IsAdmin(User.Identity.Name);
            return _projectService.GetActives(user, isAdmin);
        }

        [System.Web.Http.ActionName("ProjectChangeLogs")]
        [System.Web.Http.HttpGet]
        public List<ChangeLog> ProjectChangeLogs(int id)
        {
            return _projectService.ProjectChangeLogs(id);
        }

        [System.Web.Http.ActionName("GetInactives")]
        [System.Web.Http.HttpGet]
        public List<Project> GetInactives()
        {
            return _projectService.GetInactives();
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public Project Save(Project project)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _projectService.Save(project, user);
        }

        [System.Web.Http.ActionName("uploadFile")]
        [System.Web.Http.HttpPost]
        public void uploadFile()
        {
            _projectService.uploadFile();
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public Project Update(Project project)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _projectService.Update(project, user);
        }

        [System.Web.Http.ActionName("Enable")]
        [System.Web.Http.HttpGet]
        public Project Enable(int id)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _projectService.Enable(id, user);
        }


        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public Project Delete(int id)
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _projectService.Delete(id, user);
        }

        [System.Web.Http.ActionName("GetDashboard")]
        [System.Web.Http.HttpGet]
        public DashboardDTO GetDashboard(int projectid)
        {
            return _requirementService.GetDashboard(projectid);
        }

        [System.Web.Http.ActionName("RequestExcel")]
        [System.Web.Http.HttpGet]
        public AssignedStatusDTO Requestor()
        {
            return _projectService.ExcelRequestor();
        }

        [System.Web.Http.ActionName("DownloadExcel")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage DownloadProjectExcel(int id, string name,int executionId)
        {
            string date = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-ms");
            try
            {

                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new ByteArrayContent(_projectService.GenerateProjectReportExcel(id, date, executionId));
                httpResponseMessage.Content.Headers.Add("x-filename", "Project");
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = name + "-" + date;
                httpResponseMessage.StatusCode = HttpStatusCode.OK;


                return httpResponseMessage;



            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

            }
        }


        [System.Web.Http.ActionName("DownloadExcelEmpty")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage DownloadProjectExcelEmpty(int id, string name, string type)
        {
            string date = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-ms");
            try
            {

                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                if (type.Equals("Empty Report"))
                {
                    httpResponseMessage.Content = new ByteArrayContent(_projectService.GenerateProjectReportExcel(id, date, (int)Report.EmptyReport));
                }
                else
                {
                    httpResponseMessage.Content = new ByteArrayContent(_projectService.GenerateProjectReportExcel(id, date, (int)Report.LastExecution));
                }
                
                httpResponseMessage.Content.Headers.Add("x-filename", "Project");
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = name + "-" + date;
                httpResponseMessage.StatusCode = HttpStatusCode.OK;


                return httpResponseMessage;



            }
            catch (Exception ex)
            {
                return null;

            }
        }
        
        [System.Web.Http.ActionName("RestoreSteps")]
        [System.Web.Http.HttpGet]
        public void RestoreSteps()
        {
            _projectService.RestoreSteps();
        }
        


        [System.Web.Http.ActionName("DownloadExecutionReport")]
        [System.Web.Http.HttpGet]
        public HttpResponseMessage DownloadExecutionReport(int id, string name, int executionId)
        {
            string date = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-ms");
            try
            {

                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new ByteArrayContent(_projectService.DownloadExecutionReport(id, date, executionId));
                httpResponseMessage.Content.Headers.Add("x-filename", "Project");
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = name + "-" + date;
                httpResponseMessage.StatusCode = HttpStatusCode.OK;


                return httpResponseMessage;



            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);

            }
        }

    }
}