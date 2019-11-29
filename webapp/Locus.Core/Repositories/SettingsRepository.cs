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
    public interface ISettingsRepository
    {
        Setting Save(Setting settings);
        Setting GetByName(Setting settings);
        Setting Update(Setting settings);
    }


    public class SettingsRepository : ISettingsRepository
    {
        private LocustDBContext context = new LocustDBContext();

        public Setting GetByName(Setting setting)
        {
            try
            {
                var settings = context.Settings.Find(setting.UserName);

                if (settings == null)
                {
                    Setting settingDefault = new Setting()
                    {
                        UIMode = "Blue",
                        UserName = setting.UserName
                    };
                    context.Settings.Add(settingDefault);
                    context.SaveChanges();
                    return settingDefault;
                }
                return context.Settings.Find(setting.UserName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Setting Save(Setting setting)
        {
            try
            {
                context.Settings.Add(setting);
                context.SaveChanges();
            }
            catch
            {
                return null;
            }
            return setting;
        }

        public Setting Update(Setting setting)
        {
            try
            {
                context.Entry(setting).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            catch
            {
                return null;
            }
            return setting;
        }
    }
}
