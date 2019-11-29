namespace Locus.Core.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Runtime.Serialization;

    [Table("TestCase")]
    public partial class TestCase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TestCase()
        {
            Steps = new HashSet<Step>();
            RequirementsTests = new HashSet<RequirementsTest>();
            test_tags = new HashSet<Test_Tags>();
            TestProcedures = new HashSet<TestProcedure>();
        }

        [StringLength(20)]
        public string tc_number { get; set; }

        [Key]
        public int Test_Case_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Test_Priority { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Preconditions { get; set; }

        [Required]
        [StringLength(50)]
        public string Test_Case_Creator { get; set; }

        [StringLength(50)]
        public string Last_Editor { get; set; }

        [Required]
        public string Expected_Result { get; set; }

        [Required]
        public string Type { get; set; }

        public DateTime? Creation_Date { get; set; }

        public bool Status { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Step> Steps { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequirementsTest> RequirementsTests { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Test_Tags> test_tags { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestProcedure> TestProcedures { get; set; }
    }
}
