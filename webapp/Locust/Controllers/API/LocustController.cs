using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using Locust.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Locust.Controllers.API
{

    [AllowAnonymous]
    public class LocustController : ApiController
    {

        private readonly IBackupService _backupService;

        public LocustController(IBackupService backupService)
        {
            _backupService = backupService;
        }

        [ActionName("GetBackups")]
        [HttpGet]
        public IHttpActionResult GetBackups()
        {

            if (!VerifyToken.IsTokenValid(Request))
            {
                return Content(HttpStatusCode.Unauthorized, Resources.ResourceManager.GetString("TokenInvalid"));
            }

            try
            {
                List<Backup> backups = _backupService.GetAll();
                return Ok(backups);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.Message));
            }
        }

        [ActionName("GenerateBackup")]
        [HttpPost]
        [ValidateActionParameters]
        public IHttpActionResult GenerateBackup()
        {
            if (!VerifyToken.IsTokenValid(Request))
            {
                return Content(HttpStatusCode.Unauthorized, Resources.ResourceManager.GetString("TokenInvalid"));
            }

            try
            {

               _backupService.Delete();

                Backup backup = _backupService.GenerateBackups();
                if (backup == null)
                {
                    return Content(HttpStatusCode.InternalServerError, String.Format("The backup could not be generated"));
                }
                return Ok(backup);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.Message));
            }
        }



        [ActionName("DownloadBackup")]
        [HttpPost]
        [ValidateActionParameters]
        public HttpResponseMessage DownloadBackup([FromUri] BackupNameDTO backup)
        {
            if (!VerifyToken.IsTokenValid(Request))
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.StatusCode = HttpStatusCode.Unauthorized;
                return httpResponseMessage;
            }

            try
            {
                Backup backupFound = _backupService.Get(backup.Name);

                if (backupFound != null)
                {
                    string filePath = HttpContext.Current.Server.MapPath("~/App_Data/") + backupFound.Name;

                    using (MemoryStream ms = new System.IO.MemoryStream())
                    {
                        using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);

                            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", backupFound.Name);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = backupFound.Name;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                            return httpResponseMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");
        }

        [System.Web.Http.ActionName("RestoreBackupByFileAttached")]
        [HttpPost]
        public async Task<IHttpActionResult> RestoreBackupByFileAttached()
        {
            if (!VerifyToken.IsTokenValid(Request))
            {
                return Content(HttpStatusCode.Unauthorized, Resources.ResourceManager.GetString("TokenInvalid"));
            }
            else
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var root = HttpContext.Current.Server.MapPath("~/App_Data/Uploads/");
                var provider = new CustomMultipartFormDataStreamProvider(root);

                try
                {
                    // Read the form data.
                    await Request.Content.ReadAsMultipartAsync(provider);
                    RestoreStatusDTO restore = _backupService.RestoreBackupByFileAttached(provider.FileData[0].LocalFileName);
                    FileNameHelper.DeleteMultipartFileData(provider);
                    return Ok(restore);

                }
                catch (Exception e)
                {
                    FileNameHelper.DeleteMultipartFileData(provider);
                    return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.InnerException.Message));
                }
            }
        }



        [System.Web.Http.ActionName("RestoreBackupByName")]
        [HttpPost]
        public IHttpActionResult RestoreBackupByName([FromUri] BackupNameDTO backup)
        {
            if (!VerifyToken.IsTokenValid(Request))
            {
                return Content(HttpStatusCode.Unauthorized, Resources.ResourceManager.GetString("TokenInvalid"));
            }
            else
            {
                Backup backupFound = _backupService.Get(backup.Name);

                if (backupFound != null)
                {
                    try
                    {
                       RestoreStatusDTO restore= _backupService.RestoreBackupByName(backupFound);
                       return Ok(restore);
                    }
                    catch (Exception e)
                    {
                        return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.Message));
                    }
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, String.Format("The Backup with the name {0} does not exists.", backup.Name));
                }
            }
        }
    }
}
