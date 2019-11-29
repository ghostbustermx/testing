using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("TestEnvironment")]
    public class TestEnvironment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        [StringLength(100)]
        public string Server { get; set; }
        [Required]
        [StringLength(100)]
        public string Processor { get; set; }
        [Required]
        [StringLength(100)]
        public string RAM { get; set; }
        [Required]
        [StringLength(100)]
        public string HardDisk { get; set; }
        [Required]
        [StringLength(100)]
        public string OS { get; set; }
        public string ServerSoftwareDevs { get; set; }
        public string ServerSoftwareTest { get; set; }
        [StringLength(100)]
        public string Database { get; set; }
        public string URL { get; set; }
        [Required]
        [StringLength(100)]
        public string SiteType { get; set; }

        public string Notes { get; set; }
        [Required]
        [StringLength(100)]
        public string Creator { get; set; }

        [Required]
        [StringLength(100)]
        public string Last_Editor { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
