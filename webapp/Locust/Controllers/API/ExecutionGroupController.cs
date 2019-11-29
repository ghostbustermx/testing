using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class ExecutionGroupController: ApiController
    {
        public readonly IExecutionGroupService _executionGroupService;

        public ExecutionGroupController (IExecutionGroupService executionGroupService)
        {
            _executionGroupService = executionGroupService;
        }

        [HttpPost]
        [System.Web.Http.ActionName("Save")]
        public ExecutionGroup Save(ExecutionGroup executionGroup) 
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _executionGroupService.Save(executionGroup, user);
        }

        [HttpPut]
        [System.Web.Http.ActionName("Update")]
        public ExecutionGroup Update(ExecutionGroup executionGroup) 
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _executionGroupService.Update(executionGroup, user);
        }

        [HttpDelete]
        [System.Web.Http.ActionName("Delete")]
        public ExecutionGroup Delete(int id) 
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _executionGroupService.Delete(id, user);
        }

        [HttpDelete]
        [System.Web.Http.ActionName("Enable")]
        public ExecutionGroup Enable(int id) 
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
            return _executionGroupService.Enable(id, user);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetByProjectActives")]
        public List<ExecutionGroup> GetByProjectActives(int projectId)
        {
            return _executionGroupService.GetByProjectActives(projectId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetByProjectInactives")]
        public List<ExecutionGroup> GetByProjectInactives(int projectId)
        {
            return _executionGroupService.GetByProjectInactives(projectId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetAll")]
        public List<ExecutionGroup> GetAll()
        {
            return _executionGroupService.GetAll();
        }

        [HttpGet]
        [System.Web.Http.ActionName("Get")]
        public ExecutionGroup Get(int executionId)
        {
            return _executionGroupService.Get(executionId);
        }


    }
}