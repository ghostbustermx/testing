using Locus.Core.DTO;
using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class WebValidationController : ApiController
    {

        [HttpGet]
        [System.Web.Http.ActionName("GetFilesValidation")]
        public ValidationDTO GetFilesValidation()
        {
            return ConfigurationHelper.GetFilesValidation();
        }




        [HttpGet]
        [System.Web.Http.ActionName("GetScriptsValidation")]
        public ValidationDTO GetScriptsValidation()
        {
            return ConfigurationHelper.GetScriptsValidation();
        }



    }
}