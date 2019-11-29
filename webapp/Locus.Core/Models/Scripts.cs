using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    public class Scripts
    {

        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [Required]
        [StringLength(10)]
        public string Extension { get; set; }
        [Required]
        public int ScriptsGroup_Id { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
