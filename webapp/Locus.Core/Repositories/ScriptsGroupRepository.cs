using Locus.Core.Context;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Locus.Core.Repositories
{

    public interface IScriptsGroupRepository
    {
        List<ScriptsGroup> GetAllScriptsGroup(int projectId);
        ScriptsGroup Save(ScriptsGroup group, string user);
        ScriptsGroup Update(ScriptsGroup group, string user);
        List<ScriptsGroup> GetForProject(int projectId);
        ScriptsGroup Get(int groupId);

    }
    public class ScriptsGroupRepository : IScriptsGroupRepository
    {
        //Instance of Database Context
        private LocustDBContext context = new LocustDBContext();

        public ScriptsGroup Get(int groupId)
        {
            return context.ScriptsGroup.Find(groupId);
        }

        public List<ScriptsGroup> GetAllScriptsGroup(int projectId)
        {
            try
            {
                return context.ScriptsGroup.Where(s => s.projectId == projectId).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<ScriptsGroup> GetForProject(int projectId)
        {
            var scriptslist = (from scriptG in context.ScriptsGroup
                               where scriptG.projectId == projectId
                               select scriptG).ToList();

            return scriptslist;
        }

        public ScriptsGroup Save(ScriptsGroup group, string user)
        {
            group.Creation_Date = DateTime.UtcNow;
            group.Creator = user;
            context.ScriptsGroup.Add(group);
            context.SaveChanges();

            return group;
        }

        public ScriptsGroup Update(ScriptsGroup group, string user)
        {
            group.Last_Editor = user;
            context.Entry(group).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            return group;
        }
    }
}
