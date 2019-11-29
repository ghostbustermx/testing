using Locus.Core.Context;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Locus.Core.Repositories
{

    public interface IScriptsRepository
    {
        List<Scripts> GetAllScripts(int ScriptsGroupId);
        Scripts Save(int groupId, int projectId);
        Scripts Delete(int scriptId);
        byte[] Download(int scriptId);
    }
    public class ScriptsRepository : IScriptsRepository
    {
        //Instance of Database Context
        private LocustDBContext context = new LocustDBContext();

        public Scripts Delete(int scriptId)
        {
            try
            {
                var script = context.Scripts.Find(scriptId);

                context.Scripts.Remove(script);
                context.SaveChanges();

                return script;

            }
            catch
            {

                return null;
            }
        }

        public byte[] Download(int scriptId)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~\\");
            var script = context.Scripts.Find(scriptId);

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(path + "\\" + script.Path, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    return bytes;
                }
            }

        }

        public List<Scripts> GetAllScripts(int ScriptsGroupId)
        {
            try
            {
                return context.Scripts.Where(s => s.ScriptsGroup_Id == ScriptsGroupId).ToList();
            }
            catch
            {
                return null;
            }



        }

        public Scripts Save(int groupId, int projectId)
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var path = System.Web.HttpContext.Current.Server.MapPath("~\\Files");

                var projectFolder = path + "\\" + projectId.ToString();
                var ScriptGroupFolder = projectFolder + "\\" + "ScriptGroup";
                var ScriptGroupFolderScripts =  ScriptGroupFolder+"\\" + groupId.ToString();
                if (!Directory.Exists(projectFolder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(projectFolder);
                    


                }
                if (!Directory.Exists(ScriptGroupFolder))
                {
                    DirectoryInfo di2 = Directory.CreateDirectory(ScriptGroupFolder);
                }
                
                if (!Directory.Exists(ScriptGroupFolderScripts))
                {
                    DirectoryInfo di3 = Directory.CreateDirectory(ScriptGroupFolderScripts);
                }


                Scripts s = new Scripts();
                foreach (string file in httpRequest.Files)
                {

                    

                    var postedFile = httpRequest.Files[file];

                    FileInfo fileName = new FileInfo(postedFile.FileName);

                    s.Name = fileName.Name;
                    s.Extension = fileName.Extension;
                    s.ScriptsGroup_Id = groupId;
                    var SaveFilePath = ScriptGroupFolderScripts + "\\" + s.Name;
                    s.Path = SaveFilePath;
                    postedFile.SaveAs(SaveFilePath);

                }


                context.Scripts.Add(s);
                context.SaveChanges();

                return s;

            }
            catch
            {
                
                return null;
            }
        }
    }

    }

