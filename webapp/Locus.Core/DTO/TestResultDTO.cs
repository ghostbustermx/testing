using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
  public class TestResultDTO
    {
        public int Test_Result_Id { get; set; }

        public string Execution_Result { get; set; }

        public DateTime Execution_Date { get; set; }

        public string photoUrl { get; set; }

        public string TestType { get; set; }

        public string Status { get; set; }

        public string Tester { get; set; }

        public int TestId { get; set; }

        public int Test_Execution_Id { get; set; }

        public bool IsTaken { get; set; }
    }
}
