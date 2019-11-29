using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Locus.Core.Models;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Services
{

    public interface IAttachmentService
    {

        void Save(int EntityAction, int EntityId, int projectId, string ReqNumber, int number);

        List<Attachment> GetAttachment(int EntityAction, int EntityId);

        void RemoveAttachments(int[] EntityId);

        byte[] DownloadFile(int AttachmentId);

    }

    class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _AttachmentRepository;

            public AttachmentService(IAttachmentRepository attachmentRepository)
        {
            _AttachmentRepository = attachmentRepository;
        }

        public byte[] DownloadFile(int AttachmentId)
        {
            return _AttachmentRepository.DownloadFile(AttachmentId);
        }

        public List<Attachment> GetAttachment(int EntityAction, int EntityId)
        {
            return _AttachmentRepository.GetAttachment(EntityAction, EntityId);
        }

        public void RemoveAttachments(int[] EntityId)
        {
            _AttachmentRepository.RemoveAttachments(EntityId);
        }

        public void Save(int EntityAction, int EntityId, int projectId, string ReqNumber, int number)
        {
            _AttachmentRepository.Save(EntityAction, EntityId, projectId ,ReqNumber, number);
        }
    }
}
