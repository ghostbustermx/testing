using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
    public class RunnerDTO
    {
        public string Identifier { get; set; }
        public int Id { get; set; }
        public string OS { get; set; }
        public bool? IsConnected { get; set; }
        public bool? Status { get; set; }
        public string MAC { get; set; }
        public string Description { get; set; }
        public string IPAddress { get; set; }
        public string Tags { get; set; }
        public DateTime? Creation_Date { get; set; }
        public DateTime? Last_Connection_Date { get; set; }
        public String Time { get; set; }

    }
}
