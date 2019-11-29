using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;




//Service for project operations.
namespace Locus.Core.Services
{
    //Interface which contains methods for each CRUD operation
    public interface IBackupService
    {
        Backup Save(Backup project);

        List<Backup> GetAll();

        Backup Get(int idBackup);

        Backup Get(String backupName);

        //API exposed
        Backup GenerateBackups();

        RestoreStatusDTO RestoreBackupByFileAttached(string path);

        RestoreStatusDTO RestoreBackupByName(Backup backup);

        void Delete();

    }
    //Class which implements IProjectService's methods and instance to IProjectRepository
    public class BackupService : IBackupService
    {
        //Instance of IProjectRepository
        private readonly IBackupRepository _backupRepository;

        //Constructor of ProjectService and initialize projectRepository
        public BackupService(IBackupRepository backupRepository)
        {
            _backupRepository = backupRepository;
        }

        //Get method calls to Get Method From projectRepository.
        public Backup Get(int idBackup)
        {
            return _backupRepository.Get(idBackup);
        }
        //GetAll method calls to GetAll Method From projectRepository.
        public List<Backup> GetAll()
        {
            return _backupRepository.GetAll();
        }
        //Save method calls to Save Method From projectRepository.
        public Backup Save(Backup backup)
        {
            return this.CreateBackup(backup);
        }



        private Backup CreateBackup(Backup backup)
        {
            // read connectionstring from config file
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            // read backup folder from config file ("C:/temp/")
            var backupFolder = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            // set backupfilename (you will get something like: "C:/temp/MyDatabase-2013-12-07.bak")
            var dateUTC = DateTime.UtcNow;
            var dateString = dateUTC.ToString("yyyy-MM-dd-HH-mm-ss");

            var backupFileName = String.Format("{0}-{1}.bak",
                 sqlConStrBuilder.InitialCatalog,
                 dateString
              );
            var onlyName = backupFileName;
            backupFileName = String.Format("{0}{1}", backupFolder, backupFileName);
            var status = true;
            var message = "The Backup has been created!!!";

            try
            {
            using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
            {
                var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}'",
                    sqlConStrBuilder.InitialCatalog, backupFileName);

                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            }
            catch (Exception e)
            {
                status = false;
                message =""+e.Message;


            }
            var bak = new Backup
            {
                Name = onlyName,
                Message = message,
                Description = backup.Description,
                GeneratedBy = backup.GeneratedBy,
                Status = status,
                Creation_Date = dateUTC,
            };

            return _backupRepository.Save(bak);
        }

        public Backup GenerateBackups()
        {
            Backup backup = new Backup
            {
                Description = "Backup Generated via API.",
                GeneratedBy = "API"
            };
            Backup backupGenerated=this.CreateBackup(backup);

            if (backupGenerated==null)
            {
                return null;
            }
            else
            {
                return backupGenerated;
            } 
        }

        public RestoreStatusDTO RestoreBackupByFileAttached(string path)
        {
          return this.Restore(path);  
        }

        public RestoreStatusDTO RestoreBackupByName(Backup backup)
        {
            var backupFolder = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
            var path = backupFolder + backup.Name;
          
            return this.Restore(path);
        }

        public Backup Get(string backupName)
        {
            return _backupRepository.Get(backupName);
        }

        private RestoreStatusDTO Restore(string path)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            var master = String.Format("USE {0} ", "Master");
            var singleUser = String.Format("ALTER DATABASE {0} SET Single_User  WITH Rollback Immediate", sqlConStrBuilder.InitialCatalog);
            var restore = String.Format("RESTORE DATABASE {0} FROM DISK = '{1}' WITH REPLACE", sqlConStrBuilder.InitialCatalog, @path);
            var multiUser = String.Format("ALTER DATABASE {0} SET Multi_User", sqlConStrBuilder.InitialCatalog);
            RestoreStatusDTO restoreStatus = new RestoreStatusDTO()
            {
                Message = String.Format("The database {0} has been restored successfully", sqlConStrBuilder.InitialCatalog),
                Status = true
            };
            try
            {

            
            this.ExecuteQuery(master);
            this.ExecuteQuery(singleUser);
            this.ExecuteQuery(restore);
            
            }
            catch (Exception e)
            {
                restoreStatus.Message = String.Format("The database {0} has not been restored. Exception: {1}", sqlConStrBuilder.InitialCatalog, e.Message);
                restoreStatus.Status = false;
            }
            finally
            {
                this.ExecuteQuery(multiUser);
            }
           
             return restoreStatus;
            

        }
        private bool ExecuteQuery(string query)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
            sqlConStrBuilder.InitialCatalog = "Master";

            try
            {
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {
                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Delete()
        {
            string backupName = _backupRepository.Delete();
            var backupFolder = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
            File.Delete(backupFolder + backupName);
        }
    }
}
