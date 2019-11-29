using Locus.Core.Context;
using Locus.Core.DTO;
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
    public interface IUserRepository
    {
        User Save(UsersProjectDTO userProject);

        List<User> GetAll();

        User GetById(int idUser);

        List<UsersProjects> GetUsersProjects(int idUser);

        List<User> GetUsersByProject(int id);


        User GetByUsername(string userName);

        List<User> GetByRole(string Role);

        User Delete(User user);

        User Update(UsersProjectDTO userProject);

        User DisableUser(User user);

    }


    public class UserRepository : IUserRepository
    {
        private LocustDBContext context = new LocustDBContext();

        public User GetById(int idUser)
        {
            try
            {
                return context.Users.Find(idUser);

            }
            catch
            {
                return null;
            }

        }

        public User GetByUsername(string userName)
        {
            try
            {
                return context.Users.Where(b => b.UserName.Equals(userName)).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<User> GetByRole(string role)
        {
            try
            {
                return context.Users.Where(b => b.Role.Equals(role)).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<User> GetAll()
        {
            try
            {
                return context.Users.ToList();
            }
            catch
            {
                return null;
            }
        }

        public User Save(UsersProjectDTO userProject)
        {
            try
            {
                if (userProject.User.PhotoUrl == null)
                {
                    userProject.User.PhotoUrl = "/Content/img/default-user.png";
                }
                context.Users.Add(userProject.User);
                context.SaveChanges();

            }
            catch
            {
                return null;
            }

            var getUser = context.Users.Where(u => u.UserName == userProject.User.UserName).FirstOrDefault();

            if (userProject.User.Role.Equals("TBP") || userProject.User.Role.Equals("BA") || userProject.User.Role.Equals("VBP"))
            {
                foreach (UsersProjects usersProjects in userProject.Projects)
                {
                    usersProjects.UserId = getUser.Id;
                    context.UsersProjects.Add(usersProjects);
                }
                context.SaveChanges();
            }
            return userProject.User;
        }

        public User Delete(User user)
        {
            try
            {
                context.Users.Remove(user);
                context.SaveChanges();
                return user;
            }
            catch
            {
                return null;
            }

        }

        public User DisableUser(User user)
        {
            try
            {
                context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return user;
            }
            catch
            {
                return null;
            }
        }


        public User Update(UsersProjectDTO userProject)
        {
            try
            {
                if (userProject.User.PhotoUrl == null)
                {
                    userProject.User.PhotoUrl = "/Content/img/default-user.png";
                }
                context.Entry(userProject.User).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

            }
            catch
            {
                return null;
            }

            context.UsersProjects.RemoveRange(context.UsersProjects.Where(x => x.UserId == userProject.User.Id));
            context.SaveChanges();

            if (userProject.Projects != null)
            {
                foreach (UsersProjects usersProjects in userProject.Projects)
                {
                    usersProjects.UserId = userProject.User.Id;
                    context.UsersProjects.Add(usersProjects);
                }
                context.SaveChanges();
            }
            return userProject.User;
        }

        public List<UsersProjects> GetUsersProjects(int idUser)
        {
            try
            {
                return context.UsersProjects.Where(u => u.UserId == idUser).ToList();
            }
            catch 
            {
                return null;
            }


        }

        public List<User> GetUsersByProject(int id)
        {
            try
            {
                var users = context.Users.Where(u => (u.IsActive == true) && (u.Role == "TFA" || u.Role == "TESTER")).ToList();
                var TBP = ExecuteQuery(String.Format("SELECT * FROM [dbo].[User] AS u JOIN [dbo].[UsersProjects] up ON up.ProjectId={0} AND up.UserID=u.Id WHERE u.IsActive=1", id));
                return users.Concat(TBP).ToList(); ;
            }
            catch
            {
                return null;
            }
        }


        private List<User> ExecuteQuery(string query)
        {
            List<User> users = new List<User>();
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
                                User user = new User()
                                {
                                    Id = reader.GetInt32(0),
                                    UserName = reader.GetString(1),
                                    Role = reader.GetString(2),
                                    FirstName = reader.GetString(3),
                                    LastName = reader.GetString(4),
                                    Email = reader.GetString(5),
                                    PhotoUrl = reader.GetString(6),
                                    IsActive = reader.GetBoolean(7),
                                };
                                users.Add(user);
                            }
                        }
                    }
                }
                return users;
            }
            catch
            {
                return null;
            }
        }
    }
}
