using Locus.Core.Models;
using Locus.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Services
{
    public interface IProcedureSuplementalService
    {
        Test_Procedure_Test_Suplemental Save(Test_Procedure_Test_Suplemental tpts);

        Test_Procedure_Test_Suplemental Update(Test_Procedure_Test_Suplemental tpts);

        Test_Procedure_Test_Suplemental DeleteTP(int idtp, int idstp);

        Test_Procedure_Test_Suplemental DeleteTS(int idts, int idstp);

        List<Test_Procedure_Test_Suplemental> GetAll();

        Test_Procedure_Test_Suplemental GetTP(int idtp, int idstp);

        Test_Procedure_Test_Suplemental GetTS(int idts, int idstp);

        List<Test_Procedure_Test_Suplemental> GetForTP(int idtp);

        List<Test_Procedure_Test_Suplemental> GetForTS(int idts);

        bool DesactiveSupplementalsTp(int idtp);

        bool DesactiveSupplementalsTs(int idts);

        bool ActivateSupplementalsTp(int idtp);

        bool ActivateSupplementalsTs(int idts);

    }
    public class ProcedureSuplementalService :IProcedureSuplementalService
    {
        //Instance of IProjectRepository
        private readonly IProcedureSuplementalRepository _psRepository;

        //Constructor of ProjectService and initialize projectRepository
        public ProcedureSuplementalService(IProcedureSuplementalRepository psRepository)
        {
            _psRepository = psRepository;
        }

        public bool ActivateSupplementalsTp(int idtp)
        {
            return _psRepository.ActivateSupplementalsTp(idtp);
        }

        public bool ActivateSupplementalsTs(int idts)
        {
            return _psRepository.ActivateSupplementalsTs(idts);
        }

        public Test_Procedure_Test_Suplemental DeleteTP(int idtp, int idstp)
        {
           return _psRepository.DeleteTP(idtp, idstp);
        }

        public Test_Procedure_Test_Suplemental DeleteTS(int idts, int idstp)
        {
            return _psRepository.DeleteTS(idts, idstp);
        }

        public bool DesactiveSupplementalsTp(int idtp)
        {
            return _psRepository.DesactiveSupplementalsTp(idtp);
        }

        public bool DesactiveSupplementalsTs(int idts)
        {
            return _psRepository.DesactiveSupplementalsTs(idts);
        }

        public List<Test_Procedure_Test_Suplemental> GetAll()
        {
            return _psRepository.GetAll();
        }

        public List<Test_Procedure_Test_Suplemental> GetForTP(int idtp)
        {
            return _psRepository.GetForTP(idtp);
        }

        public List<Test_Procedure_Test_Suplemental> GetForTS(int idts)
        {
            return _psRepository.GetForTS(idts);
        }

        public Test_Procedure_Test_Suplemental GetTP(int idtp, int idstp)
        {
            return _psRepository.GetTP(idtp, idstp);
        }

        public Test_Procedure_Test_Suplemental GetTS(int idts, int idstp)
        {
            return _psRepository.GetTP(idts, idstp);
        }

        public Test_Procedure_Test_Suplemental Save(Test_Procedure_Test_Suplemental tpts)
        {
            return _psRepository.Save(tpts);
        }

        public Test_Procedure_Test_Suplemental Update(Test_Procedure_Test_Suplemental tpts)
        {
            return _psRepository.Update(tpts);
        }
    }
}
