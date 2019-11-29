namespace Locus.Core.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Runtime.Serialization;

    [Table("TestSuplemental")]
    public partial class TestSuplemental
    {
        [Key]
        public int Test_Suplemental_Id { get; set; }

        [StringLength(50)]
        public string stp_number { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Test_Procedure_Creator { get; set; }

        public DateTime? Creation_Date { get; set; }

        [StringLength(50)]
        public string Last_Editor { get; set; }

        public bool Status { get; set; }

        public int Project_Id { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Test_Procedure_Test_Suplemental test_procedure_test_suplemental { get; set; }
    }
}
