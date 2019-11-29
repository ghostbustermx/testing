using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
    public class UsersProjectDTO
    {
        public User User { get; set; }
        public List<UsersProjects> Projects { get; set; }
    }
}
