using Locus.Core.DTO;
using Locus.Core.Models;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class AccessController : ApiController
    {
        [Flags]
        public enum CustomPermissions { Backup, Project, User, Requirement, TC, TC_Details, TP, TP_Details, STP, STP_Details, TS, TS_Details, TestEnvironment, ExecutionGroup, PlayExecution };

        [System.Web.Http.ActionName("HasAccess")]
        [System.Web.Http.HttpGet]
        public UsersPermissionsDTO HasAccess(CustomPermissions permission, int projectId, int? reqId, int? TCId, int? TPId, int? STPId, int? TSId)
        {
            try
            {
                UsersPermissionsDTO access = UserHelper.HasPermission(permission, projectId, reqId, STPId, TCId, TPId, TSId, User.Identity.Name);
                return access;
            }
            catch
            {
                UsersPermissionsDTO access = new UsersPermissionsDTO
                {
                    HasPermission = false
                };
                return access;
            }
        }


        [System.Web.Http.ActionName("GetRole")]
        [System.Web.Http.HttpGet]
        public User GetRole()
        {
            if (UserHelper.IsAdmin(User.Identity.Name))
            {
                var user = UserHelper.GetRole(User.Identity.Name);
                user.Role = "ADMIN";
                return user;
            }
            return UserHelper.GetRole(User.Identity.Name);
        }


        [System.Web.Http.ActionName("HasAccessUserSection")]
        [System.Web.Http.HttpGet]
        public UsersPermissionsDTO HasAccessUserSection()
        {
            UsersPermissionsDTO access = UserHelper.HasAccessToAddUsers(User.Identity.Name);
            return access;
        }

        [System.Web.Http.ActionName("HasAccessBackupSection")]
        [System.Web.Http.HttpGet]
        public UsersPermissionsDTO HasAccessBackupSection()
        {
            UsersPermissionsDTO access = UserHelper.HasAccessToAddBackup(User.Identity.Name);
            return access;
        }

        [System.Web.Http.ActionName("HasAccessRunnerSection")]
        [System.Web.Http.HttpGet]
        public UsersPermissionsDTO HasAccessRunnerSection()
        {
            UsersPermissionsDTO access = UserHelper.HasAccessRunnerSection(User.Identity.Name);
            return access;
        }

        

        [System.Web.Http.ActionName("GetPagination")]
        [System.Web.Http.HttpGet]
        public string[] GetPagination()
        {
            return SplitterHelper.splitStringFromKey("Rows");
        }


        [System.Web.Http.ActionName("HasAccessProjectSection")]
        [System.Web.Http.HttpGet]
        public UsersPermissionsDTO HasAccessProjectSection()
        {
            UsersPermissionsDTO access = UserHelper.HasAccessToProject(User.Identity.Name);
            return access;
        }

    }
}
