using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using Locus.Core.Services;
using System.Collections.Generic;
using System.Configuration;
using static Locust.Controllers.API.AccessController;

namespace Locust.Helpers
{
    public class UserHelper
    {

        public static string GetCurrentUser(string user)
        {
            var username = user.Split('\\');
            return username[1];
        }


        public static UsersPermissionsDTO HasAccessToAddUsers(string user)
        {
            UsersPermissionsDTO usersPermissions = new UsersPermissionsDTO();

            if (IsAdmin(user))
            {
                usersPermissions.HasPermission = true;
                return usersPermissions;
            }
            else
            {
                var service = PrepareService();
                var users = service.GetByUsername(GetCurrentUser(user));
                usersPermissions.HasPermission = false;

                if (users != null && users.Role.Equals("TFA"))
                {
                    usersPermissions.HasPermission = true;

                }
                return usersPermissions;
            }
        }

        private static UsersPermissionsDTO AdminAccess(string user)
        {
            UsersPermissionsDTO usersPermissions = new UsersPermissionsDTO();
            if (IsAdmin(user))
            {
                usersPermissions.HasPermission = true;
                return usersPermissions;
            }
            else
            {
                usersPermissions.HasPermission = false;
                return usersPermissions;
            }
        }

        public static UsersPermissionsDTO HasAccessToAddBackup(string user)
        {
            return AdminAccess(user);
        }

        public static UsersPermissionsDTO HasAccessRunnerSection(string user)
        {
            return AdminAccess(user);
        }



        public static UsersPermissionsDTO HasAccessToProject(string user)
        {
            return AdminAccess(user);
        }


        private static IUserService PrepareService()
        {
            IUserRepository userRepository = new UserRepository();
            IUserProjectRepository userProjectRepository = new UserProjectRepository();
            IUserService userService = new UserService(userRepository, userProjectRepository);
            return userService;
        }


        public static UsersPermissionsDTO HasSomeAccessSite(string user)
        {

            UsersPermissionsDTO usersPermissions = new UsersPermissionsDTO();
            var service = PrepareService();
            var currentUser = GetCurrentUser(user);
            var users = service.GetByUsername(currentUser);

            if (IsAdmin(user))
            {
                if (users == null)
                {
                    IJanusService janusUser = new JanusService();
                    var u = janusUser.GetEmployeeInfoDTO(currentUser);
                    User userToBeSetted = new User
                    {
                        Role = "ADMIN",
                        Id = 0,
                        FirstName = u.ValueObject.FirstName,
                        LastName = u.ValueObject.LastName,
                        Alias = u.ValueObject.Alias,
                        Department = u.ValueObject.Department,
                        Division = u.ValueObject.Division,
                        Email = u.ValueObject.Email,
                        Gender = u.ValueObject.Gender,
                        HireDate = u.ValueObject.HireDate,
                        IsActive = u.ValueObject.IsActive,
                        JobTitle = u.ValueObject.JobTitle,
                        PhotoUrl = u.ValueObject.PhotoUrl,
                        UserName = currentUser
                    };

                    //Case to test the admin Role
                    if (currentUser.Equals("admin"))
                    {
                        userToBeSetted = new User
                        {
                            Role = "ADMIN",
                            Id = 0,
                            FirstName = "Locust",
                            LastName = "Copechi",
                            Alias = "RTS",
                            Department = "Testing",
                            Division = "IT",
                            Email = "LocustAdmin@pinnacleaerospace.com",
                            Gender = "N/A",
                            HireDate = new System.DateTime(2008, 6, 1, 7, 47, 0),
                            IsActive = true,
                            JobTitle = "Tool",
                            PhotoUrl = "https://indsamachar.com/wp-content/uploads/2019/06/remote-4.jpg",
                            UserName = currentUser
                        };
                    }

                    UsersProjectDTO userToSave = new UsersProjectDTO
                    {
                        User = userToBeSetted,
                        Projects = new List<UsersProjects>()
                    };

                    service.Save(userToSave);
                }
                else
                {
                    if (!users.Role.Equals("ADMIN"))
                    {
                        UsersProjectDTO userToUpdate = new UsersProjectDTO
                        {
                            User = users,
                            Projects = new List<UsersProjects>()
                        };

                        users.Role = "ADMIN";
                        service.Update(userToUpdate);
                    }
                }


                usersPermissions.HasPermission = true;
                return usersPermissions;
            }
            else
            {
                usersPermissions.HasPermission = false;

                if (users == null)
                {
                    return usersPermissions;
                }

                if (users.Role == "ADMIN")
                {
                    usersPermissions.HasPermission = false;
                    service.Delete(users);
                    return usersPermissions;

                }

                if (users != null && users.IsActive)
                {

                    usersPermissions.HasPermission = true;

                }
                return usersPermissions;
            }
        }


        public static User GetRole(string user)
        {
            var username = GetCurrentUser(user).ToLower();
            var service = PrepareService();
            var result = service.GetByUsername(username);
            return result;
        }


        public static bool IsAdmin(string user)
        {
            var username = GetCurrentUser(user).ToLower();

            var array = ConfigurationManager.AppSettings["Administrators"];

            var administrators = array.Split(',');

            foreach (var admin in administrators)
            {
                if (admin.ToLower().Equals(username))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsAdminWithoutParse(string user)
        {
            var array = ConfigurationManager.AppSettings["Administrators"];

            var administrators = array.Split(',');

            foreach (var admin in administrators)
            {
                if (admin.ToLower().Equals(user))
                {
                    return true;
                }
            }
            return false;
        }


        public static UsersPermissionsDTO HasPermission(CustomPermissions custom, int projectID, int? reqId, int? STPId, int? TCId, int? TPId, int? TSId, string user)
        {
            UsersPermissionsDTO usersPermissions = new UsersPermissionsDTO
            {
                HasPermission = false
            };

            //SA
            if (IsAdmin(user))
            {
                usersPermissions.HasPermission = true;
                return usersPermissions;
            }

            //FTA
            var service = PrepareService();
            var users = service.GetByUsername(GetCurrentUser(user));

            if (!users.IsActive)
            {
                usersPermissions.HasPermission = false;
                return usersPermissions;
            }

            if (users != null && users.Role.Equals("TFA") && !custom.Equals("Backup"))
            {
                usersPermissions.HasPermission = true;
                return usersPermissions;
            }

            //Tester 
            if (users != null && users.Role.Equals("TESTER") && !custom.Equals("Backup") && !custom.Equals("Users"))
            {
                usersPermissions.HasPermission = true;
                return usersPermissions;
            }

            //TBP
            if (users != null && users.Role.Equals("TBP") && !custom.Equals("Backup") && !custom.Equals("Users"))
            {
                //usersPermissions.HasPermission = HasAccessToProject(projectID, users);
                //return usersPermissions;
                switch (custom)
                {
                    case CustomPermissions.Requirement:
                        {
                            usersPermissions.HasPermission = HasAccessToRequirement(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.Project:
                        {
                            usersPermissions.HasPermission = HasAccessToProject(projectID, users);
                            break;
                        }
                    case CustomPermissions.STP:
                        {
                            usersPermissions.HasPermission = HasAccessToSTP(projectID, STPId, users);
                            break;
                        }
                    case CustomPermissions.TC:
                        {
                            usersPermissions.HasPermission = HasAccessToTC(projectID, reqId, TCId, users);
                            break;
                        }
                    case CustomPermissions.TP:
                        {
                            usersPermissions.HasPermission = HasAccessToTP(projectID, reqId, TPId, users);
                            break;
                        }
                    case CustomPermissions.TS:
                        {
                            usersPermissions.HasPermission = HasAccessToTS(projectID, reqId, TSId, users);
                            break;
                        }
                    case CustomPermissions.TC_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTC(projectID, reqId, TCId, users);
                            break;
                        }
                    case CustomPermissions.TP_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTP(projectID, reqId, TPId, users);
                            break;
                        }
                    case CustomPermissions.TS_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTS(projectID, reqId, TSId, users);
                            break;
                        }
                    case CustomPermissions.STP_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToSTP(projectID, STPId, users);
                            break;
                        }
                    case CustomPermissions.ExecutionGroup:
                        {
                            usersPermissions.HasPermission = HasAccessToExecutionGroup(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.TestEnvironment:
                        {
                            usersPermissions.HasPermission = HasAccessToTestEnvironment(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.PlayExecution:
                        {
                            usersPermissions.HasPermission = HasAccessToPlayExecution(projectID, reqId, TCId, users);
                            break;
                        }
                }
            }


            //VBP
            if (users != null && users.Role.Equals("VBP") && !custom.Equals("Backup") && !custom.Equals("Users"))
            {
                switch (custom)
                {
                    case CustomPermissions.Requirement:
                        {
                            usersPermissions.HasPermission = HasAccessToRequirement(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.Project:
                        {
                            usersPermissions.HasPermission = HasAccessToProject(projectID, users);
                            break;
                        }
                    case CustomPermissions.TC:
                        {
                            usersPermissions.HasPermission = HasAccessToTC(projectID, reqId, TCId, users);
                            break;
                        }
                    case CustomPermissions.TP:
                        {
                            usersPermissions.HasPermission = HasAccessToTP(projectID, reqId, TPId, users);
                            break;
                        }
                    case CustomPermissions.TS:
                        {
                            usersPermissions.HasPermission = HasAccessToTS(projectID, reqId, TSId, users);
                            break;
                        }
                    case CustomPermissions.TC_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTC(projectID, reqId, TCId, users);
                            break;
                        }
                    case CustomPermissions.TP_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTP(projectID, reqId, TPId, users);
                            break;
                        }
                    case CustomPermissions.TS_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTS(projectID, reqId, TSId, users);
                            break;
                        }
                    case CustomPermissions.STP_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToSTP(projectID, STPId, users);
                            break;
                        }
                    case CustomPermissions.ExecutionGroup:
                        {
                            usersPermissions.HasPermission = HasAccessToExecutionGroup(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.TestEnvironment:
                        {
                            usersPermissions.HasPermission = HasAccessToTestEnvironment(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.PlayExecution:
                        {
                            usersPermissions.HasPermission = HasAccessToPlayExecution(projectID, reqId, TCId, users);
                            break;
                        }
                }

            }

            if (users != null && users.Role.Equals("BA") && !custom.Equals("Backup") && !custom.Equals("Users"))
            {
                switch (custom)
                {
                    case CustomPermissions.Requirement:
                        {
                            usersPermissions.HasPermission = HasAccessToRequirement(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.Project:
                        {
                            usersPermissions.HasPermission = HasAccessToProject(projectID, users);
                            break;
                        }
                    case CustomPermissions.TC_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTC(projectID, reqId, TCId, users);
                            break;
                        }
                    case CustomPermissions.TP_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTP(projectID, reqId, TPId, users);
                            break;
                        }
                    case CustomPermissions.TS_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToTS(projectID, reqId, TSId, users);
                            break;
                        }
                    case CustomPermissions.STP_Details:
                        {
                            usersPermissions.HasPermission = HasAccessToSTP(projectID, STPId, users);
                            break;
                        }
                    case CustomPermissions.ExecutionGroup:
                        {
                            usersPermissions.HasPermission = HasAccessToExecutionGroup(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.TestEnvironment:
                        {
                            usersPermissions.HasPermission = HasAccessToTestEnvironment(projectID, reqId, users);
                            break;
                        }
                    case CustomPermissions.PlayExecution:
                        {
                            usersPermissions.HasPermission = HasAccessToPlayExecution(projectID, reqId, TCId, users);
                            break;
                        }


                }
            }
            return usersPermissions;
        }

        private static bool HasAccessToRequirement(int projectId, int? reqId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToRequirement(projectId, reqId, user);
            return result;
        }

        private static bool HasAccessToTestEnvironment(int projectId, int? reqId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToTestEnvironment(projectId, reqId, user);
            return result;
        }
        private static bool HasAccessToExecutionGroup(int projectId, int? reqId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToExecutionGroup(projectId, reqId, user);
            return result;
        }

        private static bool HasAccessToPlayExecution(int projectId, int? executionId, int? playId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToPlayExecution(projectId, executionId, playId, user);
            return result;
        }



        private static bool HasAccessToProject(int projectId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToProject(projectId, user);
            return result;
        }

        private static bool HasAccessToSTP(int projectId, int? STPId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToSTP(projectId, STPId, user);
            return result;
        }


        private static bool HasAccessToTC(int projectId, int? reqID, int? TCId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToTC(projectId, reqID, TCId, user);
            return result;
        }


        private static bool HasAccessToTP(int projectId, int? reqID, int? TPId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToTP(projectId, reqID, TPId, user);
            return result;
        }

        private static bool HasAccessToTS(int projectId, int? reqID, int? TSId, User user)
        {
            var service = PrepareService();
            var result = service.HasAccessToTS(projectId, reqID, TSId, user);
            return result;
        }
    }
}