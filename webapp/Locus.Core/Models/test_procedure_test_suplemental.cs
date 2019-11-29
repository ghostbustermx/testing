namespace Locus.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Test_Procedure_Test_Suplemental
    {

        public int Test_Suplemental_Id { get; set; }

        public int? Test_Procedure_Id { get; set; }

        public int? Test_Scenario_Id { get; set; }

        public virtual TestProcedure TestProcedure { get; set; }

        public virtual TestScenario TestScenario { get; set; }

        public virtual TestSuplemental TestSuplemental { get; set; }

        public bool Status { get; set; }
    }
}
