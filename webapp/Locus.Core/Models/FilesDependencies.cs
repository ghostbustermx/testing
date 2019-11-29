using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Models
{
    public class FilesDependencies
    {
        [Key]
        public int Id { get; set; }
        public int ExecutionId { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int? GroupId { get; set; }
    }
}
