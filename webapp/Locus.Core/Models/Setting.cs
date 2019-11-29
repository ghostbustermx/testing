using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Locus.Core.Models
{
    [Table("Setting")]
    public class Setting
    {

        [Required]
        [StringLength(70)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string UIMode { get; set; }
    }
}
