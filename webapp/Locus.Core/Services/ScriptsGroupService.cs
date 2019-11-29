using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;

namespace Locus.Core.Services
{

    public interface IScriptsGroupService
    {
        List<ScriptsGroup> GetAllScriptsGroup(int projectId);
        ScriptsGroup Save(ScriptsGroup group, string user);
        ScriptsGroup Update(ScriptsGroup group, string user);
        List<ScriptsGroup> GetForProject(int projectId);
        ScriptsGroup Get(int groupId);
    }

    public class ScriptsGroupService : IScriptsGroupService
    {
        private readonly IScriptsGroupRepository _scriptsGroupRepository;

        public ScriptsGroupService(IScriptsGroupRepository scriptsGroupRepository)
        {
            _scriptsGroupRepository = scriptsGroupRepository;
        }

        public ScriptsGroup Get(int groupId)
        {
            return _scriptsGroupRepository.Get(groupId);
        }

        public List<ScriptsGroup> GetAllScriptsGroup(int projectId)
        {
            return _scriptsGroupRepository.GetAllScriptsGroup(projectId);
        }

        public List<ScriptsGroup> GetForProject(int projectId)
        {
            return _scriptsGroupRepository.GetForProject(projectId);
        }

        public ScriptsGroup Save(ScriptsGroup group, string user)
        {
            return _scriptsGroupRepository.Save(group, user);
        }

        public ScriptsGroup Update(ScriptsGroup group, string user)
        {
            return _scriptsGroupRepository.Update(group, user);
        }
    }
}
