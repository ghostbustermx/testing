namespace Locus.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RequirementsTest")]
    public partial class RequirementsTest
    {
        
        public int Requirement_Id { get; set; }

        public int? Test_Case_Id { get; set; }

        public int? Test_Scenario_Id { get; set; }

        public int? Test_Procedure_Id { get; set; }

        public virtual Requirement Requirement { get; set; }

        public virtual TestCase TestCase { get; set; }

        public virtual TestProcedure TestProcedure { get; set; }

        public virtual TestScenario TestScenario { get; set; }
    }
}
