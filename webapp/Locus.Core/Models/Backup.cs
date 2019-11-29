namespace Locus.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Backup")]
    public partial class Backup
    {

        public int Id { get; set; }

        [Required]
        [StringLength(70)]
        public string Name { get; set; }

        [Required]
        public bool Status { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(200)]
        public string Message { get; set; }

        [Required]
        public string GeneratedBy { get; set; }

        [Required]
        public DateTime Creation_Date { get; set; }

    }
}
