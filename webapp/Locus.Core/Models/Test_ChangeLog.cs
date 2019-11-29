using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("Test_ChangeLog")]
    public partial class Test_ChangeLog
    {
            [ForeignKey("ChangeLog")]
            public int Change_Log_Id { get; set; }

            [ForeignKey("Project")]
            public int? Project_Id { get; set; }

            [ForeignKey("Requirement")]
            public int? Requirement_Id { get; set; }

            public int? Test_Case_Id { get; set; }

            public int? Test_Scenario_Id { get; set; }

            public int? Test_Procedure_Id { get; set; }

            public int? Test_Suplemental_Id { get; set; }

            public virtual ChangeLog ChangeLog { get; set; }

            public virtual Project Project { get; set; }

            public virtual Requirement Requirement { get; set; }

            public virtual TestCase TestCase { get; set; }

            public virtual TestProcedure TestProcedure { get; set; }

            public virtual TestScenario TestScenario { get; set; }

            public virtual TestSuplemental TestSuplemental { get; set; }
    }
}
