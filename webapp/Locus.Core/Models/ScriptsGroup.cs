using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    public class ScriptsGroup
    {
        public int Id { get; set; }

        [Required]
        public int projectId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Release { get; set; }

        [Required]
        [StringLength(50)]
        public string Creator { get; set; }

        [StringLength(50)]
        public string Last_Editor { get; set; }

        [Required]
        public DateTime Creation_Date { get; set; }

        [Required]
        [StringLength(50)]
        public string Priority{ get; set; }
    }
}
