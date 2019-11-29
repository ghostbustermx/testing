using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
    public class LinkCellDTO
    {
        public int auxId { get; set; }
        public string cell { get; set; }
        public string coor { get; set; }
        public string name { get; set; }
        public string ReqNumber { get; set; }
        public string reqName { get; set; }
        public bool HasTestCase { get; set; }

    }
}
