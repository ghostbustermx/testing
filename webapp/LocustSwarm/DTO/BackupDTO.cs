using System;

namespace LocustSwarm.DTO
{
    class BackupDTO
    {
        public int Id { get; set; }


        public string Name { get; set; }


        public bool Status { get; set; }


        public string Description { get; set; }


        public string Message { get; set; }


        public string GeneratedBy { get; set; }

        public DateTime Creation_Date { get; set; }

        public string EndpointToDownload { get; set; }
    }
}