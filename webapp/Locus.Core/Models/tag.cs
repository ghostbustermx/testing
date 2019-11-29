namespace Locus.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tag")]
    public partial class Tag
    {
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public int? Project_Id { get; set; }

        public virtual Project Project { get; set; }

        public virtual Test_Tags test_tags { get; set; }
    }
}
