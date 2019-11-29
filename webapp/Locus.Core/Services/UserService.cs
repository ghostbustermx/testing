using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;


namespace Locus.Core.Services
{

    public interface IUserService
    {
        User Save(UsersProjectDTO userProject);

        List<User> GetAll();

        User GetById(int idUser);

        List<UsersProjects> GetUsersProjects(int id);

        List<User> GetUsersByProject(int idProject);

        User GetByUsername(string userName);

        List<User> GetByRole(string Role);

        User Delete(User user);

        User Update(UsersProjectDTO userProject);

        User DisableUser(User user);


        bool HasAccessToProject(int id, User user);

        bool HasAccessToRequirement(int id, int? reqId, User user);

        bool HasAccessToSTP(int id, int? STPId, User user);

        bool HasAccessToTC(int id, int? reqID, int? TCId, User user);

        bool HasAccessToTP(int id, int? reqID, int? TPId, User user);

        bool HasAccessToTS(int id, int? reqID, int? TSId, User user);

        bool HasAccessToTestEnvironment(int id, int? teId, User user);

        bool HasAccessToExecutionGroup(int id, int? executionId, User user);

        bool HasAccessToPlayExecution(int id, int? executionId,int? playId, User user);

        

    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProjectRepository _userProjectsRepository;


        public UserService(IUserRepository userRepository, IUserProjectRepository userProjectsRepository)
        {
            _userRepository = userRepository;
            _userProjectsRepository = userProjectsRepository;
        }

        public User Delete(User user)
        {
            return _userRepository.Delete(user);
        }

        public User DisableUser(User user)
        {
            return _userRepository.DisableUser(user);
        }

        public List<User> GetAll()
        {
            return _userRepository.GetAll();
        }


        public User GetById(int idUser)
        {
            return _userRepository.GetById(idUser);
        }

        public List<User> GetByRole(string Role)
        {
            return _userRepository.GetByRole(Role);
        }

        public User GetByUsername(string userName)
        {
            return _userRepository.GetByUsername(userName);
        }

        public List<User> GetUsersByProject(int idProject)
        {
            return _userRepository.GetUsersByProject(idProject);
        }

        public List<UsersProjects> GetUsersProjects(int id)
        {
            return _userRepository.GetUsersProjects(id);
        }



        public bool HasAccessToProject(int id, User user)
        {
            return _userProjectsRepository.HasAccessToProject(id, user.Id);
        }

        public bool HasAccessToRequirement(int id, int? reqID, User user)
        {
            return _userProjectsRepository.HasAccessToRequirement(id, reqID, user.Id);
        }

        public bool HasAccessToSTP(int id, int? STPId, User user)
        {
            return _userProjectsRepository.HasAccessToSTP(id, STPId, user.Id);
        }

        public bool HasAccessToTC(int id, int? reqID, int? TCId, User user)
        {
            return _userProjectsRepository.HasAccessToTC(id, reqID, TCId, user.Id);
        }

        public bool HasAccessToTestEnvironment(int id, int? teId, User user)
        {
            return _userProjectsRepository.HasAccessToTestEnvironment(id, teId, user.Id);
        }
        public bool HasAccessToExecutionGroup(int id, int? executionId, User user)
        {
            return _userProjectsRepository.HasAccessToExecutionGroup(id, executionId, user.Id);
        }

        public bool HasAccessToTP(int id, int? reqID, int? TPId, User user)
        {
            return _userProjectsRepository.HasAccessToTP(id, reqID, TPId, user.Id);
        }

        public bool HasAccessToTS(int id, int? reqID, int? TSId, User user)
        {
            return _userProjectsRepository.HasAccessToTS(id, reqID, TSId, user.Id);
        }

        public User Save(UsersProjectDTO userProject)
        {
            return _userRepository.Save(userProject);
        }



        public User Update(UsersProjectDTO userProject)
        {
            return _userRepository.Update(userProject);
        }


        public bool HasAccessToPlayExecution(int id, int? executionId, int? playId, User user)
        {
            return _userProjectsRepository.HasAccessToPlayExecution(id, executionId, playId, user.Id);
        }
    }
}
