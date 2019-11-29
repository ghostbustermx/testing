using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("ChangeLog")]
    public partial class ChangeLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content{ get; set; }

        public int Version { get; set; }
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(50)]
        public string User { get; set; }

        [Required]
        public bool Active { get; set; }


    }
}
