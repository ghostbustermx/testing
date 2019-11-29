using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    public class UsersProjects
    {

        [Required]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(70)]
        public string ProjectName { get; set; }
    }
}
