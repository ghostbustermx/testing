using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("ExecutionTestEvidence")]
    public partial class ExecutionTestEvidence
    {
        public int Execution_Group_Id { get; set; }

        public int? Tc_Id { get; set; }
        public int? Tp_Id { get; set; }
        public int? Ts_Id { get; set; }
        public int? Ta_Id { get; set; }

        public int id { get; set; }


        public virtual ExecutionGroup ExecutionGroup { get;set;}

        
    }
}
