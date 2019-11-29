using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Services
{
    //Interface which contains methods for each CRUD operation
    public interface ITagService
    {

        Tag Save(Tag tag, int id, int idts, int idtp, int idstp);

        Tag Update(Tag tag);

        Tag Delete(int idtag, int idtc, int idts, int idtp, int idstp);

        List<Tag> GetAll();

        Tag Get(int idtag);

        List<Tag> GetTestCaseTags(int idtc);

        List<Tag> GetTestScenarioTags(int idts);

        List<Tag> GetTestProcedureTags(int idtp);

        List<Tag> GetTestSuplementalTags(int idstp);
    }

    public class TagService : ITagService
    {
        //Instance of IProjectRepository
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Tag Delete(int idtag, int idtc, int idts, int idtp, int idstp )
        {
            return _tagRepository.Delete(idtag, idtc, idts, idtp, idstp);
        }

        public Tag Get(int idtag)
        {
            return _tagRepository.Get(idtag);
        }

        public List<Tag> GetAll()
        {
            return _tagRepository.GetAll();
        }

        public List<Tag> GetTestCaseTags(int idtc)
        {
            return _tagRepository.GetTestCaseTags(idtc);
        }

        public List<Tag> GetTestProcedureTags(int idtp)
        {
            return _tagRepository.GetTestProcedureTags(idtp);
        }

        public List<Tag> GetTestScenarioTags(int idts)
        {
            return _tagRepository.GetTestScenarioTags(idts);
        }

        public List<Tag> GetTestSuplementalTags(int idstp)
        {
            return _tagRepository.GetTestSuplementalTags(idstp);
        }

        public Tag Save(Tag tag, int idtc, int idts, int idtp, int idstp)
        {
            return _tagRepository.Save(tag, idtc, idts, idtp, idstp);
        }

        public Tag Update(Tag tag)
        {
            return _tagRepository.Update(tag);
        }
    }
}
