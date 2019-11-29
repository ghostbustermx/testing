using Locus.Core.DTO;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class TraceabilityFindingController : ApiController
    {

        private readonly ITraceabilityFindingController _testEvidenceService;

        public TraceabilityFindingController(ITraceabilityFindingController testEvidenceService)
        {
            _testEvidenceService = testEvidenceService;

        }
        [System.Web.Http.ActionName("GetTraceabilityFindings")]
        [System.Web.Http.HttpGet]
        public MissingTestEvidenceDTO GetTestEvidenceMissing(int id)
        {
            return _testEvidenceService.GetTestEvidenceMissing(id);
        }
    }
}
