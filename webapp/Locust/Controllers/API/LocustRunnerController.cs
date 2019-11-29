using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Services;
using Locust.Helpers;
using Locust.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class LocustRunnerController : ApiController
    {
        private readonly IRunnersService _runnerService;


        public LocustRunnerController(IRunnersService runnerService)
        {
            _runnerService = runnerService;
        }

        [System.Web.Http.ActionName("GetActives")]
        [System.Web.Http.HttpGet]
        public List<RunnerDTO> GetActives()
        {
            return _runnerService.GetActives();
        }


        [System.Web.Http.ActionName("GetInactives")]
        [System.Web.Http.HttpGet]
        public List<RunnerDTO> GetInactives()
        {
            return _runnerService.GetInactives();
        }


        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public Runner Update(Runner runner)
        {
            return _runnerService.Update(runner);
        }

        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public Runner Delete(int id)
        {
            return _runnerService.Delete(_runnerService.Get(id));
        }

        [ActionName("Get")]
        [HttpGet]
        public Runner Get(int id)
        {
            return _runnerService.Get(id);
        }

        [System.Web.Http.ActionName("GetFullActives")]
        [System.Web.Http.HttpGet]
        public List <Runner> GetFullActives()
        {
            return _runnerService.GetFullActives();
        }

    }
}
