using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;


namespace Locust.Controllers.API
{

    public class UsersController : ApiController
    {
        private readonly IUserService _userService;


        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<User> GetAll()
        {
            return _userService.GetAll();
        }


        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public User Save(UsersProjectDTO userProject)
        {
            return _userService.Save(userProject);
        }


        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public User Update(UsersProjectDTO userProject)
        {
            return _userService.Update(userProject);
        }

        [System.Web.Http.ActionName("Disable")]
        [System.Web.Http.HttpPut]
        public User Disable(User user)
        {
            return _userService.DisableUser(user);
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public User Get(User user)
        {
            return _userService.GetById(user.Id);
        }


        [System.Web.Http.ActionName("GetByUserName")]
        [System.Web.Http.HttpGet]
        public User GetByUserName(string userName)
        {

            return _userService.GetByUsername(userName);
        }


        [System.Web.Http.ActionName("GetUsersProjects")]
        [System.Web.Http.HttpGet]
        public List<UsersProjects> GetUsersProjects(int id)
        {
            return _userService.GetUsersProjects(id);
        }

        [System.Web.Http.ActionName("GetUsersByProject")]
        [System.Web.Http.HttpGet]
        public List<User> GetUsersByProject(int id)
        {
            return _userService.GetUsersByProject(id);
        }


        [System.Web.Http.ActionName("GetCurrentUser")]
        [System.Web.Http.HttpGet]
        public User GetCurrentUser()
        {
            var user = UserHelper.GetCurrentUser(User.Identity.Name);
           
            return _userService.GetByUsername(user);
        }

    }
}