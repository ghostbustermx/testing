using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("TestResult")]
   public partial class TestResult
    {
        [Key]
        public int Test_Result_Id { get; set; }

        public int  Test_Execution_Id { get; set; }

        public string Execution_Result { get; set; }

        public DateTime? Execution_Date { get; set; }

        public string Status { get; set; }

        public string Tester { get; set; }

        public string CurrentHolder { get; set; }

        public int? Test_Case_Id { get; set; }

        public int? Test_Scenario_Id { get; set; }

        public int? Test_Procedure_Id { get; set; }

        public string Identifier_number { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        [StringLength(500)]
        public string PhotoUrl { get; set; }

        public bool IsTaken { get; set; }

        public string Evidence { get; set; }

    }
}

