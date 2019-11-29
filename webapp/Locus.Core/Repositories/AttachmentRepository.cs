using Locus.Core.Context;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Locus.Core.Repositories
{
    public interface IAttachmentRepository
    {

        void Save(int EntityAction, int EntityId, int projectId, string EntityNumber, int number);

        List<Attachment> GetAttachment(int EntityAction, int EntityId);

        void RemoveAttachments(int[] EntityId);

        byte[] DownloadFile(int AttachmentId);
    }


    class AttachmentRepository : IAttachmentRepository
    {
        private LocustDBContext context = new LocustDBContext();

        public byte[] DownloadFile(int AttachmentId)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~\\");
            var attachment = context.Attachment.Find(AttachmentId);

            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(path+"\\" + attachment.Path, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    return bytes;
                }
            }



        }

        public List<Attachment> GetAttachment(int EntityAction, int EntityId)
        {
            switch (EntityAction)
            {
                case 1:
                    return (from att in context.Attachment
                            where att.Requirement_Id == EntityId
                            select att).ToList();
                case 2:
                    return (from att in context.Attachment
                            where att.TestSupplemental_Id == EntityId
                            select att).ToList();
                case 3:
                    return (from att in context.Attachment
                            where att.Test_Case_Id == EntityId
                            select att).ToList();
                case 4:
                    return (from att in context.Attachment
                            where att.Test_Scenario_Id == EntityId
                            select att).ToList();
                case 5:
                    return (from att in context.Attachment
                            where att.Test_Procedure_Id == EntityId
                            select att).ToList();
                case 6:
                    return (from att in context.Attachment
                            where att.Test_Result_Id == EntityId
                            select att).ToList();
                default: return null;
            }
        }

        public void RemoveAttachments(int[] EntityId)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~\\");
            foreach (var id in EntityId)
            {
                var attachment = context.Attachment.Find(id);
                context.Attachment.Remove(attachment);
                context.SaveChanges();
                File.Delete(path+"\\"+ attachment.Path);
            }

        }

        public void Save(int EntityAction, int EntityId, int projectId, string EntityNumber, int number)
        {
            var httpRequest = HttpContext.Current.Request;
            var path = System.Web.HttpContext.Current.Server.MapPath("~\\Files");
            var project = context.Projects.Find(projectId);

            var projectFolder = path + "\\" + project.Name;
            var attachmentsDirectory = path + "\\" + project.Name + "\\Attachments";
            var DirectoryName ="Files\\"+ project.Name + "\\Attachments";
            if (!Directory.Exists(projectFolder))
            {
                DirectoryInfo di = Directory.CreateDirectory(projectFolder);
                DirectoryInfo di2 = Directory.CreateDirectory(attachmentsDirectory);
            }
            
            foreach (string file in httpRequest.Files)
            {

                var postedFile = httpRequest.Files[file];

                FileInfo fileName = new FileInfo(postedFile.FileName);

                string fileExt = fileName.Extension;

                Attachment attachment = new Attachment();


                switch (EntityAction)
                {
                    case 1:
                        attachment.Requirement_Id = EntityId;
                        attachment.Extention = fileExt;
                        var RequirementDirectory = attachmentsDirectory + "\\" + EntityNumber;
                        
                        if (!Directory.Exists(RequirementDirectory))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(RequirementDirectory);
                        }


                        attachment.Name = fileName.Name;
                        attachment.Path = DirectoryName +"\\"+ EntityNumber+"\\" + attachment.Name;
                        attachment.Size = (postedFile.ContentLength / 1024);
                        attachment.Size = (attachment.Size / 1024);
                        var SaveFileReqPath = RequirementDirectory + "\\" + attachment.Name;
                        postedFile.SaveAs(SaveFileReqPath);

                        break;
                    case 2:
                        attachment.TestSupplemental_Id = EntityId;

                        var TestEvidenceDirectorySTP = attachmentsDirectory + "\\Test Evidence";

                        if (!Directory.Exists(TestEvidenceDirectorySTP))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(TestEvidenceDirectorySTP);
                        }

                        attachment.Extention = fileExt;
                        var stp = context.TestSuplementals.Find(EntityId);
                        var TestDirectorySTP = TestEvidenceDirectorySTP + "\\" + stp.stp_number;
                        DirectoryName = DirectoryName + "\\Test Evidence" + "\\" + stp.stp_number;
                        if (!Directory.Exists(TestDirectorySTP))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(TestDirectorySTP);
                        }

                        attachment.Name = fileName.Name;
                        attachment.Path = DirectoryName + "\\" + attachment.Name;
                        attachment.Size = (postedFile.ContentLength / 1024);
                        attachment.Size = (attachment.Size / 1024);

                        var SaveFilePathSTP = TestDirectorySTP + "\\" + attachment.Name;
                        postedFile.SaveAs(SaveFilePathSTP);

                        break;

                    case 3:

                        var TestEvidenceDirectory = attachmentsDirectory + "\\Test Evidence";

                        if (!Directory.Exists(TestEvidenceDirectory))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(TestEvidenceDirectory);
                        }

                        attachment.Test_Case_Id = EntityId;

                        attachment.Extention = fileExt;
                        var tc = context.TestCases.Find(EntityId);
                        var TestDirectory = TestEvidenceDirectory + "\\" + tc.tc_number;
                        DirectoryName = DirectoryName + "\\Test Evidence" + "\\" + tc.tc_number;
                        if (!Directory.Exists(TestDirectory))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(TestDirectory);
                        }

                        attachment.Name = fileName.Name;
                        attachment.Path = DirectoryName + "\\" + attachment.Name ;
                        attachment.Size = (postedFile.ContentLength / 1024);
                        attachment.Size = (attachment.Size / 1024);

                        var SaveFilePath = TestDirectory +"\\" + attachment.Name;
                        postedFile.SaveAs(SaveFilePath);
                        break;
                    case 4:
                        var TestEvidenceDirectoryTS = attachmentsDirectory + "\\Test Evidence";

                        if (!Directory.Exists(TestEvidenceDirectoryTS))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(TestEvidenceDirectoryTS);
                        }

                        attachment.Test_Scenario_Id = EntityId;

                        attachment.Extention = fileExt;
                        var ts = context.TestScenarios.Find(EntityId);
                        var TestDirectoryTS = TestEvidenceDirectoryTS + "\\" + ts.ts_number;
                        DirectoryName = DirectoryName + "\\Test Evidence" + "\\" + ts.ts_number;
                        if (!Directory.Exists(TestDirectoryTS))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(TestDirectoryTS);
                        }

                        attachment.Name = fileName.Name;
                        attachment.Path = DirectoryName + "\\" + attachment.Name;
                        attachment.Size = (postedFile.ContentLength / 1024);
                        attachment.Size = (attachment.Size / 1024);

                        var SaveFilePathTS = TestDirectoryTS + "\\" + attachment.Name;
                        postedFile.SaveAs(SaveFilePathTS);
                        break;
                        
                    case 5:
                        attachment.Test_Procedure_Id = EntityId;

                        var TestEvidenceDirectoryTP = attachmentsDirectory + "\\Test Evidence";

                        if (!Directory.Exists(TestEvidenceDirectoryTP))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(TestEvidenceDirectoryTP);
                        }

                        

                        attachment.Extention = fileExt;
                        var tp = context.TestProcedures.Find(EntityId);
                        var TestDirectoryTP = TestEvidenceDirectoryTP + "\\" + tp.tp_number;
                        DirectoryName = DirectoryName + "\\Test Evidence" + "\\" + tp.tp_number;
                        if (!Directory.Exists(TestDirectoryTP))
                        {
                            DirectoryInfo dir_req = Directory.CreateDirectory(TestDirectoryTP);
                        }

                        attachment.Name = fileName.Name;
                        attachment.Path = DirectoryName + "\\" + attachment.Name;
                        attachment.Size = (postedFile.ContentLength / 1024);
                        attachment.Size = (attachment.Size / 1024);

                        var SaveFilePathTP = TestDirectoryTP + "\\" + attachment.Name;
                        postedFile.SaveAs(SaveFilePathTP);

                        break;
                    case 6:
                        attachment.Test_Result_Id = EntityId;
                        break;
                }


                string SizeValue = attachment.Size.ToString("00.000");
                attachment.Size = Double.Parse(SizeValue);
               
                context.Attachment.Add(attachment);
                context.SaveChanges();


                
            }
        }
    }
}
