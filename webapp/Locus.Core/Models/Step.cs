namespace Locus.Core.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Runtime.Serialization;

    public partial class Step
    {
        public int Id { get; set; }

        public int? Test_Case_Id { get; set; }

        public int? Test_Procedure_Id { get; set; }

        public int? Test_Scenario_Id { get; set; }

        public int? Test_Suplemental_Id { get; set; }

        public int number_steps { get; set; }

        public string type { get; set; }

        [Required]
        public string action { get; set; }

        public DateTime creation_date { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual TestCase TestCase { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual TestProcedure TestProcedure { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual TestScenario TestScenario { get; set; }
    }
}
