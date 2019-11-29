using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System.Collections.Generic;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class JanusController : ApiController
    {




        private readonly IJanusService _janusService;
        private readonly IUserService _userService;


        public JanusController(IJanusService janusService, IUserService userService)
        {
            _janusService = janusService;
            _userService = userService;
        }

        [System.Web.Http.ActionName("GetEmployeeInfo")]
        [System.Web.Http.HttpGet]
        public User GetUserInfo()
        {
            string user = UserHelper.GetCurrentUser(User.Identity.Name);
            User u = _userService.GetByUsername(user);
            if (u != null)
            {
                if (UserHelper.IsAdminWithoutParse(u.UserName))
                {
                    u.Role = "ADMIN";
                }
                return u;
            }
            else
            {

                EmployeeInfoDTO employee = _janusService.GetEmployeeInfoDTO(user);

                User users = new User()
                {
                    Email = employee.ValueObject.Email,
                    FirstName = employee.ValueObject.FirstName,
                    IsActive = employee.ValueObject.IsActive,
                    LastName = employee.ValueObject.LastName,
                    PhotoUrl = employee.ValueObject.PhotoUrl,
                    UserName = employee.ValueObject.UserName
                };
                if (UserHelper.IsAdminWithoutParse(users.UserName))
                {
                    users.Role = "ADMIN";
                }
                return users;
            }


        }

        [System.Web.Http.ActionName("GetEmployees")]
        [System.Web.Http.HttpGet]
        public EmployeesDTO GetEmployees()
        {
            return _janusService.GetEmployees();
        }
    }
}
