namespace Locus.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Requirement")]
    public partial class Requirement
    {
        public int Id { get; set; }

        public int Project_Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        public string Description { get; set; }


        [Required]
        [StringLength(50)]
        public string Developer_Assigned { get; set; }

        [Required]
        [StringLength(50)]
        public string Tester_Assigned { get; set; }

        public int Axosoft_Task_Id { get; set; }
        
        public string Acceptance_Criteria { get; set; }

        public string Release { get; set; }

        public bool Status { get; set; }

        [StringLength(50)]
        public string req_number { get; set; }

        public virtual Project Project { get; set; }

        public virtual RequirementsTest RequirementsTest { get; set; }
    }
}
