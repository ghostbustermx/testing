namespace Locus.Core.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Runtime.Serialization;

    [Table("TestProcedure")]
    public partial class TestProcedure
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TestProcedure()
        {
            Steps = new HashSet<Step>();
            RequirementsTests = new HashSet<RequirementsTest>();
            test_procedure_test_suplemental = new HashSet<Test_Procedure_Test_Suplemental>();
            test_tags = new HashSet<Test_Tags>();
        }

        [Key]
        public int Test_Procedure_Id { get; set; }

        [StringLength(20)]
        public string tp_number { get; set; }

        public int? Test_Case_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Test_Priority { get; set; }

        [StringLength(150)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Test_Procedure_Creator { get; set; }

        public string Expected_Result { get; set; }

        public DateTime? Creation_Date { get; set; }

        public int? Script_Id { get; set; }

        [StringLength(50)]
        public string Last_Editor { get; set; }

        public bool Status { get; set; }

        [Required]
        public string Type { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Step> Steps { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual TestCase TestCase { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequirementsTest> RequirementsTests { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Test_Procedure_Test_Suplemental> test_procedure_test_suplemental { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Test_Tags> test_tags { get; set; }
    }
}
