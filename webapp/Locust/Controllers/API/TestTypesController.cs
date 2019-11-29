using Locust.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class TestTypesController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("GetTypes")]
        public string[] GetTestTypes()
        {
            //List<String> listOfTypes = new List<string>();

          //  SplitterHelper.splitStringFromKey("TypesOfTest");

            return SplitterHelper.splitStringFromKey("TypesOfTest");
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("GetAutomated")]
        public string[] GetAutomated(string TestAutomated)
        {

            return SplitterHelper.splitStringFromKey(TestAutomated);
        }
    }
}
