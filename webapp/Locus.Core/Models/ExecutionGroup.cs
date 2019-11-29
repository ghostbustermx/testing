using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("ExecutionGroup")]
    public partial class ExecutionGroup
    {
        [Key]
        public int Execution_Group_Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        
        public int? TestEnvironmentId { get; set; }

        public int? RunnerId { get; set; }

        public DateTime? Creation_Date { get; set; }

        [Required]
        public string Creator{ get; set; }

        public string LastEditor { get; set; }

        public DateTime? lastEditDate { get; set; }

        public bool isActive { get; set; }

        public bool IsReadyToExecute { get; set; }
        public bool isAutomated { get; set; }


       /* public virtual ExecutionTestEvidence _Requirements { get; set; }*/

        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExecutionTestEvidence> ExecutionTestEvidence { get; set; }
        
    }
}
