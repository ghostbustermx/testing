using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    public class Runner
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Identifier { get; set; }

        [Required]
        [StringLength(100)]
        public string OS { get; set; }

        public bool? IsConnected { get; set; }

        public bool Status { get; set; }

        [Required]
        [StringLength(25)]
        public string MAC { get; set; }


        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(20)]
        public string IPAddress { get; set; }

        [StringLength(200)]
        public string Tags { get; set; }

        public DateTime Creation_Date { get; set; }

        public DateTime Last_Connection_Date { get; set; }

    }
}
