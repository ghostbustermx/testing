using Locus.Core.Models;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Locust.Controllers.API
{
    public class TagController : ApiController
    {
        private readonly ITagService _tagService;


        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [System.Web.Http.ActionName("GetAll")]
        [System.Web.Http.HttpGet]
        public List<Tag> GetAll()
        {
            return _tagService.GetAll();
        }

        [System.Web.Http.ActionName("Get")]
        [System.Web.Http.HttpGet]
        public Tag Get(int id)
        {
            return _tagService.Get(id);
        }

        [System.Web.Http.ActionName("Save")]
        [System.Web.Http.HttpPost]
        public Tag Save(Tag tag, int idtc, int idts, int idtp, int idstp)
        {
            return _tagService.Save(tag, idtc, idts, idtp, idstp);
        }

        [System.Web.Http.ActionName("Update")]
        [System.Web.Http.HttpPut]
        public Tag Update(Tag tag)
        {
            return _tagService.Update(tag);
        }

        [System.Web.Http.ActionName("GetTestCaseTags")]
        [System.Web.Http.HttpGet]
        public List<Tag> GetTestCaseTags(int idtc)
        {
            return _tagService.GetTestCaseTags(idtc);
        }

        [System.Web.Http.ActionName("GetTestScenarioTags")]
        [System.Web.Http.HttpGet]
        public List<Tag> GetTestScenarioTags(int idts)
        {
            return _tagService.GetTestScenarioTags(idts);
        }


        [System.Web.Http.ActionName("GetTestProcedureTags")]
        [System.Web.Http.HttpGet]
        public List<Tag> GetTestProcedureTags(int idtp)
        {
            return _tagService.GetTestProcedureTags(idtp);
        }

        [System.Web.Http.ActionName("GetTestSuplementalTags")]
        [System.Web.Http.HttpGet]
        public List<Tag> GetTestSuplementalTags(int idstp)
        {
            return _tagService.GetTestSuplementalTags(idstp);
        }


        [System.Web.Http.ActionName("Delete")]
        [System.Web.Http.HttpDelete]
        public Tag Delete(int idTag, int idtc, int idts, int idtp, int idstp)
        {
            return _tagService.Delete(idTag, idtc, idts, idtp, idstp);
        }
    }
}
