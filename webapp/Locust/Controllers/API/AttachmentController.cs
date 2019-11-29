using Locus.Core.Models;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class AttachmentController:ApiController
    {
        private readonly IAttachmentService _attachmentService;

        public enum ActionOnEntity
        {
            REQ = 1,
            STP = 2,
            TC= 3,
            TS=4,
            TP = 5,
            TR = 6
        }

        public AttachmentController (IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost]
        [System.Web.Http.ActionName("UploadFile")]
        public void Save(int EntityAction, int EntityId,int projectId, string EntityNumber, int number)
        {
            switch (EntityAction)
            {
                case (int)ActionOnEntity.REQ:
                    _attachmentService.Save((int)ActionOnEntity.REQ, EntityId, projectId, EntityNumber, number); break;
                case (int)ActionOnEntity.STP:
                    _attachmentService.Save((int)ActionOnEntity.STP, EntityId, projectId, EntityNumber, number); break;
                case (int)ActionOnEntity.TC:
                    _attachmentService.Save((int)ActionOnEntity.TC, EntityId, projectId, EntityNumber, number); break;
                case (int)ActionOnEntity.TS:
                    _attachmentService.Save((int)ActionOnEntity.TS, EntityId, projectId, EntityNumber, number); break;
                case (int)ActionOnEntity.TP:
                    _attachmentService.Save((int)ActionOnEntity.TP, EntityId, projectId, EntityNumber, number); break;
                case (int)ActionOnEntity.TR:
                    _attachmentService.Save((int)ActionOnEntity.TR, EntityId, projectId, EntityNumber, number); break;

                default: _attachmentService.Save((int)ActionOnEntity.REQ, EntityId, projectId, EntityNumber, number);
                    break;
            }


            
        }

        [HttpGet]
        [System.Web.Http.ActionName("DownLoad")]
        public HttpResponseMessage DownLoad(int EntityId, string name)
        {

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.Content = new ByteArrayContent(_attachmentService.DownloadFile(EntityId));
            httpResponseMessage.Content.Headers.Add("x-filename", name);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = name;
            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            return httpResponseMessage;
            
        }

        [HttpPost]
        [System.Web.Http.ActionName("RemoveAttachments")]
        public void RemoveAttachments(int[] EntityId)
        {
            _attachmentService.RemoveAttachments(EntityId);
        }

        [HttpGet]
        [System.Web.Http.ActionName("GetAttachment")]
        public List<Attachment> GetAttachment(int EntityAction, int EntityId)
        {
            return _attachmentService.GetAttachment(EntityAction, EntityId);
        }



    }
}