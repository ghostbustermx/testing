using Locus.Core.Context;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//Repository for project operations.
namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface IBackupRepository
    {
        Backup Save(Backup backup);

        List<Backup> GetAll();

        Backup Get(int idBackup);

        Backup Get(String backupName);

        String Delete();

    }
    //Class which implements IProjectRepository's methods and use DBContext for apply operations.
    public class BackupRepository : IBackupRepository
    {
        //Instance of Database Context
        private LocustDBContext context = new LocustDBContext();

        //Method to get a project from the database which id coincide with the parameter
        public Backup Get(int idBackup)
        {
            try
            {
                return context.Backups.Find(idBackup);
              
            }
            catch
            {
                return null;
            }
            
        }

        public Backup Get(string backupName)
        {
            try
            {
                return context.Backups.Where(b => b.Name.Equals(backupName)).FirstOrDefault();

            }
            catch
            {
                return null;
            }
        }

            //Method to obtain all of the projects from database.
            public List<Backup> GetAll()
        {
            try
            {
                return context.Backups.ToList();
              
            }
            catch
            {
                return null;
            }
        }

        public String Delete()
        {
            try
            {
                if (context.Backups.ToList().Count() >= 31)
                {
                    var b = (from backs in context.Backups select backs).OrderBy(aux => aux.Id).FirstOrDefault();
                    context.Backups.Remove(b);
                    context.SaveChanges();

                    return b.Name;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        //Method to save a new project in database.
        public Backup Save(Backup backup)
        {
            try
            {
                context.Backups.Add(backup);            
                context.SaveChanges();
                return backup;
            }
            catch
            {
                return null;
            }
        }   
    }
}
