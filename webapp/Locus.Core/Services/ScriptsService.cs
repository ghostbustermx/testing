using Locus.Core.DTO;
using Locus.Core.Helpers;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;

namespace Locus.Core.Services
{

    public interface IScriptsService
    {
        List<Scripts> GetAllScripts(int ScriptsGroupId);
        Scripts Save(int groupId, int projectId);
        Scripts Delete(int scriptId);
        byte[] Download(int scriptId);
        bool CompressAllScripts(int projectId,  string name, Scripts[] scriptsRelated);
    }

    public class ScriptsService : IScriptsService
    {
        private readonly IScriptsRepository _scriptsRepository;

        public ScriptsService(IScriptsRepository scriptsRepository)
        {
            _scriptsRepository = scriptsRepository;
        }

        public bool CompressAllScripts(int projectId, string name, Scripts[] scriptsRelated)
        {
            var ServerPath = System.Web.HttpContext.Current.Server.MapPath("~\\" +"Files\\"+ projectId.ToString() + "\\ZipFiles");
            return ZipFileCreatorHelper.CreateZipFile(ServerPath, name, scriptsRelated);
        }

        public Scripts Delete(int scriptId)
        {
            return _scriptsRepository.Delete(scriptId);
        }

        public byte[] Download(int scriptId)
        {
            return _scriptsRepository.Download(scriptId);
        }

        public List<Scripts> GetAllScripts(int ScriptsGroupId)
        {
            return _scriptsRepository.GetAllScripts(ScriptsGroupId);
        }

        public Scripts Save(int groupId, int projectId)
        {
            return _scriptsRepository.Save(groupId, projectId);
        }
    }
}
