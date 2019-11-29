using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("Attachment")]
    public partial class Attachment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }
        public int Requirement_Id { get; set; }
        public int TestSupplemental_Id { get; set; }
        public int Test_Case_Id { get; set; }
        public int Test_Procedure_Id { get; set; }
        public int Test_Scenario_Id { get; set; }
        public int Test_Result_Id { get; set; }
        public string Path { get; set; }


        public double Size { get; set; }

    }
}
