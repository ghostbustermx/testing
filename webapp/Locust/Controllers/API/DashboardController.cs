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
    public class DashboardController : ApiController
    {

        private readonly IRequirementService _requirementService;

        public DashboardController(IRequirementService requirementService)
        {
            _requirementService = requirementService;

        }

        [ActionName("GetDashboard")]
        [HttpGet]
        public DashboardDTO GetDashboard(int projectid)
        {
            return _requirementService.GetDashboard(projectid);
        }


    }

}

