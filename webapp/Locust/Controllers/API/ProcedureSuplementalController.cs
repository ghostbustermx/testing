using Locus.Core.Models;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class ProcedureSuplementalController : ApiController
    {
        private readonly IProcedureSuplementalService _psService;

        public ProcedureSuplementalController(IProcedureSuplementalService psService)
        {
            _psService = psService;
        }


        [System.Web.Http.ActionName("DeleteTP")]
        [System.Web.Http.HttpDelete]
        public Test_Procedure_Test_Suplemental DeleteTP(int idtp, int idstp)
        {
            return _psService.DeleteTP(idtp, idstp);
        }

        [System.Web.Http.ActionName("DeleteTS")]
        [System.Web.Http.HttpDelete]
        public Test_Procedure_Test_Suplemental DeleteTS(int idts, int idstp)
        {
            return _psService.DeleteTS(idts, idstp);
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<Test_Procedure_Test_Suplemental> GetAll()
        {
            return _psService.GetAll();
        }

        [System.Web.Http.ActionName("GetForTP")]
        [System.Web.Http.HttpGet]
        public List<Test_Procedure_Test_Suplemental> GetForTP(int idtp)
        {
            return _psService.GetForTP(idtp);
        }

        [System.Web.Http.ActionName("GetForTS")]
        [System.Web.Http.HttpGet]
        public List<Test_Procedure_Test_Suplemental> GetForTS(int idts)
        {
            return _psService.GetForTS(idts);
        }

        [System.Web.Http.ActionName("GetTP")]
        [System.Web.Http.HttpGet]
        public Test_Procedure_Test_Suplemental GetTP(int idtp, int idstp)
        {
            return _psService.GetTP(idtp, idstp);
        }

        [System.Web.Http.ActionName("GetTS")]
        [System.Web.Http.HttpGet]
        public Test_Procedure_Test_Suplemental GetTS(int idts, int idstp)
        {
            return _psService.GetTP(idts, idstp);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public Test_Procedure_Test_Suplemental Save(Test_Procedure_Test_Suplemental tpts)
        {
            return _psService.Save(tpts);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public Test_Procedure_Test_Suplemental Update(Test_Procedure_Test_Suplemental tpts)
        {
            return _psService.Update(tpts);
        }


        [System.Web.Http.ActionName("DisableForTp")]
        [System.Web.Http.HttpGet]
        public bool DisableForTp(int tpId)
        {
            return _psService.DesactiveSupplementalsTp(tpId);
        }
        [System.Web.Http.ActionName("DisableForTs")]
        [System.Web.Http.HttpGet]
        public bool DisableForTs(int tsId)
        {
            return _psService.DesactiveSupplementalsTs(tsId);
        }


        [System.Web.Http.ActionName("EnableForTp")]
        [System.Web.Http.HttpGet]
        public bool EnableForTp(int tpId)
        {
            return _psService.ActivateSupplementalsTp(tpId);
        }
        [System.Web.Http.ActionName("EnableForTs")]
        [System.Web.Http.HttpGet]
        public bool EnableForTs(int tsId)
        {
            return _psService.ActivateSupplementalsTs(tsId);
        }

    }
}