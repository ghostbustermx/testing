using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    [Table("TestExecution")]
    public class TestExecution
    {
        [Key]
        public int Test_Execution_Id { get; set; }

        public string Description { get; set; }

        [Required]
        public string Version { get; set; }

        public string Creator { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public DateTime CreationDate { get; set; }

        public int Execution_Group_Id { get; set; }

        public bool HasResultsCreated { get; set; }

    }
}
