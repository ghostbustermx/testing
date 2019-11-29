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
    public class RunnerController : ApiController
    {
        private readonly IRunnersService _runnerService;
        private readonly ITestExecutionService _testExecutionService;
        private readonly IExecutionGroupService _testGroupService;
        private readonly IExecutionTestService _executionTestService;

        public enum DownloadType
        {
            TEMPLATE,
            SCRIPT
        }

        public RunnerController(IRunnersService runnerService, ITestExecutionService testExecutionService, IExecutionGroupService testGroupService, IExecutionTestService executionTestService)
        {
            _runnerService = runnerService;
            _testExecutionService = testExecutionService;
            _testGroupService = testGroupService;
            _executionTestService = executionTestService;
        }

        [ActionName("Register")]
        [HttpPost]
        [ValidateActionParameters]
        public IHttpActionResult RegisterRunner(Runner runner)
        {
            if (!VerifyToken.IsTokenValid(Request))
            {
                return Content(HttpStatusCode.Unauthorized, Resource.ResourceManager.GetString("TokenInvalid"));
            }
            try
            {
                Runner r = _runnerService.GetRunner(runner);
                if (r != null)
                {
                    return Content(HttpStatusCode.InternalServerError, String.Format(Resource.ResourceManager.GetString("RunnerExists"), runner.Identifier));
                }
                runner.IsConnected = true;
                runner.Status = true;
                Runner runners = _runnerService.Register(runner);
                return Ok(runners);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.Message));
            }
        }

        [ActionName("Update")]
        [HttpPut]
        [ValidateActionParameters]
        public IHttpActionResult UpdateRunner(Runner runner)
        {
            if (!VerifyToken.IsTokenValid(Request))
            {
                return Content(HttpStatusCode.Unauthorized, Resource.ResourceManager.GetString("TokenInvalid"));
            }
            try
            {
                Runner r = _runnerService.GetRunner(runner);
                if (r == null)
                {
                    return Content(HttpStatusCode.InternalServerError, String.Format(Resource.ResourceManager.GetString("RunnerDoesNotExists"), runner.Identifier));
                }
                r.Tags = runner.Tags;
                r.OS = runner.OS;
                r.IPAddress = runner.IPAddress;
                r.MAC = runner.MAC;
                r.Description = runner.Description;
                Runner runners = _runnerService.Update(r);
                return Ok(runners);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.Message));
            }
        }

        [ActionName("Delete")]
        [HttpDelete]
        [ValidateActionParameters]
        public IHttpActionResult DeleteRunner(string Identifier)
        {
            if (!VerifyToken.IsTokenValid(Request))
            {
                return Content(HttpStatusCode.Unauthorized, Resource.ResourceManager.GetString("TokenInvalid"));
            }
            try
            {
                Runner r = _runnerService.GetRunner(new Runner() { Identifier = Identifier });
                if (r == null)
                {
                    return Content(HttpStatusCode.InternalServerError, String.Format(Resource.ResourceManager.GetString("RunnerDoesNotExists"), Identifier));
                }
                Runner runners = _runnerService.Delete(r);
                return Ok(String.Format(Resource.ResourceManager.GetString("RunnerDeleted"), Identifier));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.Message));
            }
        }


        [ActionName("Connect")]
        [HttpPut]
        [ValidateActionParameters]
        public IHttpActionResult ConnectRunner(string Identifier)
        {
            if (!VerifyToken.IsTokenValid(Request))
            {
                return Content(HttpStatusCode.Unauthorized, Resources.ResourceManager.GetString("TokenInvalid"));
            }
            try
            {
                Runner r = _runnerService.GetRunner(new Runner() { Identifier = Identifier });
                if (r == null)
                {
                    return Content(HttpStatusCode.InternalServerError, String.Format(Resource.ResourceManager.GetString("RunnerDoesNotExists"), Identifier));

                }
                if (!r.Status)
                {
                    return Content(HttpStatusCode.InternalServerError, String.Format(Resource.ResourceManager.GetString("RunnerNotActivated"), Identifier));
                }
                r.IsConnected = true;
                r.Last_Connection_Date = DateTime.UtcNow;
                _runnerService.Update(r);
                return Ok(String.Format(Resource.ResourceManager.GetString("RunnerUpdated"), Identifier));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.Message));
            }
        }


        [ActionName("DownloadTestFiles")]
        [HttpPut]
        [ValidateActionParameters]
        public IHttpActionResult DownloadTestFiles(string Identifier, int ExecutionID, DownloadType type)
        {
            Runner r = _runnerService.GetRunner(new Runner() { Identifier = Identifier });

            if (r == null)
            {
                return Content(HttpStatusCode.InternalServerError, Resources.ResourceManager.GetString("RunnerIdentifierInvalid"));
            }

            TestExecution testExecution = _testExecutionService.Get(ExecutionID);
            if (testExecution == null)
            {
                return Content(HttpStatusCode.InternalServerError, Resources.ResourceManager.GetString("ExecutionIdInvalid"));
            }

            ExecutionGroup group = _testGroupService.Get(testExecution.Execution_Group_Id);
            if (!group.isAutomated)
            {
                return Content(HttpStatusCode.InternalServerError, Resources.ResourceManager.GetString("IsNotAutomated"));
            }

            try
            {
                //logic to get the path acording to the type (Scripts,Template).
                string path = "";
                switch (type)
                {
                    case DownloadType.TEMPLATE:
                        path = "example";
                        break;
                    case DownloadType.SCRIPT:
                        path = "example";
                        break;

                    default:
                        return Content(HttpStatusCode.InternalServerError, Resources.ResourceManager.GetString("DownloadType"));
                }

                DownloadFile download = new DownloadFile(Request, path);
                return download;
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, String.Format("Error: {0}", e.Message));
            }
        }
    }
}
