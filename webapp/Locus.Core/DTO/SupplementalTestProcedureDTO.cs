using Locus.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{

    public class SupplementalTestProcedureDTO
    {

        public int Test_Suplemental_Id { get; set; }

        public string stp_number { get; set; }


        public string Title { get; set; }


        public string Description { get; set; }


        public string Test_Procedure_Creator { get; set; }

        public DateTime? Creation_Date { get; set; }

        public bool Status { get; set; }

        public int Project_Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Step> Steps { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Tag> Tags { get; set; }


    }
}
