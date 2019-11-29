﻿using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;


namespace Locust.Controllers.API
{

    public class BackupController : ApiController
    {
        private readonly IBackupService _backupService;

        
        public BackupController(IBackupService backupService)
        {
            _backupService = backupService;
        }
        
        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<Backup> GetAll()
        {
            return _backupService.GetAll();
        }

        [System.Web.Http.ActionName("Download")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Download([FromUri] string name)
        {
            try
            {
                Backup backupFound = _backupService.Get(name);
                if (backupFound!=null)
                {
                    string filePath = HttpContext.Current.Server.MapPath("~/App_Data/") + backupFound.Name;

                    using (MemoryStream ms = new MemoryStream())
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
                return this.Request.CreateResponse(HttpStatusCode.NotFound, "File not found.");
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public Backup Get(int id)
        {
            return _backupService.Get(id);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public Backup Save(Backup backup)
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            backup.GeneratedBy = userName;
            return _backupService.Save(backup);
        }
    }
}