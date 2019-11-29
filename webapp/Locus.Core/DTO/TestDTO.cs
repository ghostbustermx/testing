using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
    public class TestDTO
    {
        internal bool reqStatus;

        public int Test_Id { get; set; }
        public int Related_Test_Id { get; set; }
        public string IdentifiedNumber { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Preconditions { get; set; }
        public string Creator { get; set; }
        public string ExpectedResult { get; set; }
        public string Note { get; set; }

        public string TestType { get; set; }
        
        public string LastEditor { get; set; }
        
        public DateTime CreationDate { get; set; }
        public bool Status { get; set; }
        public int reqId { get; set; }
        public string Type { get; set; }
        public string reqNumber { get; set; }
        public int ScriptId { get; set; }
        
    }
}
