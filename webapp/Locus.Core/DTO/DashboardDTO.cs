using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
    public class DashboardDTO
    {
        public int totalRequirements { get; set; }
        public int requirementsMissingTestEvidence { get; set; }
        public int totalstp{ get; set; }
        public int totalTestCases { get; set; }
        public int totalTestProcedures { get; set;}
        public int totalTestScenarios { get; set; }
    }
}
