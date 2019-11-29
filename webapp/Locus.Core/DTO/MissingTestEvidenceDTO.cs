using Locus.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
    public class MissingTestEvidenceDTO
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Requirement> Requirements { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<TestCase> TC { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<TestProcedure> TP { get; set; }


    }
}
