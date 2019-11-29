using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
   public class RequirementDTO
    {
       public int Id { get; set; }

       public string Name { get; set; }

       public string req_number { get; set; }

        public bool isSelected { get; set; }

        public bool testId { get; set; }

    }
}
