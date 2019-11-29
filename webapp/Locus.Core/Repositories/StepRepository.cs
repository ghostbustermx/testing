using Locus.Core.Context;
using Locus.Core.DTO;
using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Repositories
{
    //Interface which contains methods for each CRUD operation
    public interface IStepRepository
    {
        Step Save(Step step);

        Step[] SaveArray(Step[] steps, int projectId, int TypeOfSave);

        Step[] UpdateArray(Step[] steps, int projectId, int TypeOfSave, int TestEvidenceId, int Evidence);

        Step[] DeleteArray(Step[] steps);


        Step Update(Step step);

        List<Step> DeleteForTC(int tcId);

        Step Delete(int idStep);

        List<Step> GetAll();

        Step Get(int idStep);

        List<Step> GetForTestCase(int tcId);

        List<Step> GetForTestScenario(int tsId);

        List<Step> GetForTestProcedure(int tpId);

        List<Step> GetForTestSuplemental(int stpId);

        List<Step> GetForTestSuplementalOrder(int stpId);

        List<Step> GetForTestCaseOrder(int stpId);

        List<Step> GetForTestScenarioOrder(int tsId);

        List<Step> GetForTestProcedureOrder(int tpId);

        List<StepDTO> GetForTestScenarioSTP(int projectId, int tpId);

        List<StepDTO> GetForTestProcedureSTP(int projectId, int tpId);
    }
    //Class which implements IStepRepository's methods and use DBContext for apply operations.
    public class StepRepository : IStepRepository
    {
        //Instance of Database Context
        LocustDBContext context = new LocustDBContext();

        public Step Delete(int idStep)
        {
            try
            {
                var step = context.Steps.Find(idStep);
                context.Steps.Remove(step);
                context.SaveChanges();
                return step;
            }
            catch
            {
                return null;
            }
        }

        public Step Get(int idStep)
        {
            try
            {
                return context.Steps.Find(idStep);
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetAll()
        {
            try
            {
                return context.Steps.ToList();
            }
            catch
            {
                return null;
            }
        }

        public Step Save(Step step)
        {
            try
            {
                step.creation_date = DateTime.UtcNow;
                context.Steps.Add(step);
                context.SaveChanges();
                return step;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> DeleteForTC(int tcId)
        {
            try
            {
                var step = context.Steps.Where(X => X.Test_Case_Id == tcId).ToList();
                foreach (Step step1 in step)
                {
                    context.Steps.Remove(step1);
                    context.SaveChanges();

                }
                return step;

            }
            catch
            {
                return null;
            }
        }


        public Step Update(Step step)
        {
            try
            {
                context.Entry(step).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return step;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetForTestCase(int tcId)
        {
            try
            {
                var steps = context.Steps.Where(x => x.Test_Case_Id == tcId).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetForTestScenario(int tsId)
        {
            try
            {
                var steps = context.Steps.Where(x => x.Test_Scenario_Id == tsId).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetForTestSuplemental(int stpId)
        {
            try
            {
                var steps = context.Steps.Where(x => x.Test_Suplemental_Id == stpId).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetForTestSuplementalOrder(int stpId)
        {
            try
            {
                var steps = context.Steps.Where(x => x.Test_Suplemental_Id == stpId).ToList().OrderBy(x => x.number_steps).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }



        public List<Step> GetAllOrder()
        {
            try
            {
                var steps = context.Steps.ToList().OrderBy(x => x.number_steps).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetForTestProcedure(int tpId)
        {
            try
            {
                var steps = context.Steps.Where(x => x.Test_Procedure_Id == tpId).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetForTestCaseOrder(int tcId)
        {
            try
            {
                var steps = context.Steps.Where(x => x.Test_Case_Id == tcId).ToList().OrderBy(x => x.number_steps).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetForTestScenarioOrder(int tsId)
        {
            try
            {
                var steps = context.Steps.Where(x => x.Test_Scenario_Id == tsId).ToList().OrderBy(x => x.number_steps).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        public List<Step> GetForTestProcedureOrder(int tpId)
        {
            try
            {
                var steps = context.Steps.Where(x => x.Test_Procedure_Id == tpId).ToList().OrderBy(x => x.number_steps).ToList();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        public List<StepDTO> GetForTestProcedureSTP(int projectId, int tpId)
        {
            var steps = context.Steps.Where(x => x.Test_Procedure_Id == tpId).OrderBy(x => x.number_steps).ToList();
            List<StepDTO> allStepList = new List<StepDTO>();



            foreach (var step in steps)
            {
                StepDTO stepDTO = new StepDTO();
                stepDTO.action = step.action;
                stepDTO.number_steps = step.number_steps;
                allStepList.Add(stepDTO);
                int aux = step.action.IndexOf("STP_");
                if (aux != -1)
                {
                    string stpNumber = step.action.Substring(aux, (step.action.Length - aux));

                    var stp = (from sup in context.TestSuplementals
                               where sup.stp_number == stpNumber &&
                               sup.Project_Id == projectId
                               select sup.Test_Suplemental_Id).FirstOrDefault();

                    var stpSteps = GetForTestSuplementalOrder(stp);

                    foreach (var subStep in stpSteps)
                    {
                        StepDTO subStepDTO = new StepDTO();
                        subStepDTO.action = subStep.action;
                        subStepDTO.subType = "STP";
                        if (subStep.number_steps < 10)
                        {
                            subStepDTO.number_steps = step.number_steps + (.01 * subStep.number_steps);

                        }
                        else
                        {
                            string val = step.number_steps.ToString() + "." + subStep.number_steps.ToString();
                            subStepDTO.number_steps = Convert.ToDouble(val);
                        }
                        allStepList.Add(subStepDTO);
                    }

                }



            }


            return allStepList;
        }


        public List<StepDTO> GetForTestScenarioSTP(int projectId, int tpId)
        {
            var steps = context.Steps.Where(x => x.Test_Scenario_Id == tpId).OrderBy(x => x.number_steps).ToList();
            List<StepDTO> allStepList = new List<StepDTO>();



            foreach (var step in steps)
            {
                StepDTO stepDTO = new StepDTO();
                stepDTO.action = step.action;
                stepDTO.number_steps = step.number_steps;

                if (step.type != null)
                {
                    if (step.type.Equals("Expected Result"))
                    {
                        stepDTO.subType = "Expected Result";
                    }
                }


                allStepList.Add(stepDTO);
                int aux = step.action.IndexOf("STP_");
                if (aux != -1)
                {
                    string stpNumber = step.action.Substring(aux, (step.action.Length - aux));

                    var stp = (from sup in context.TestSuplementals
                               where sup.stp_number == stpNumber &&
                               sup.Project_Id == projectId
                               select sup.Test_Suplemental_Id).FirstOrDefault();

                    var stpSteps = GetForTestSuplementalOrder(stp);

                    foreach (var subStep in stpSteps)
                    {
                        StepDTO subStepDTO = new StepDTO();
                        subStepDTO.action = subStep.action;
                        subStepDTO.subType = "STP";
                        if (subStep.number_steps < 10)
                        {
                            subStepDTO.number_steps = step.number_steps + (.01 * subStep.number_steps);

                        }
                        else
                        {
                            string val = step.number_steps.ToString() + "." + subStep.number_steps.ToString();
                            subStepDTO.number_steps = Convert.ToDouble(val);
                        }
                        allStepList.Add(subStepDTO);
                    }

                }



            }


            return allStepList;
        }

        public Step[] SaveArray(Step[] steps, int projectId, int TypeOfSave)
        {
            List<string> stpExcluded = new List<string>();

            switch (TypeOfSave)
            {
                case 1:
                    foreach (var step in steps)
                    {
                        step.creation_date = DateTime.UtcNow;

                    }

                    context.Steps.AddRange(steps);
                    context.SaveChanges();

                    break;
                case 2:

                    foreach (var step in steps)
                    {
                        step.creation_date = DateTime.UtcNow;

                    }

                    context.Steps.AddRange(steps);
                    context.SaveChanges();


                    foreach (var step in steps)
                    {
                        int aux = step.action.IndexOf("STP_");
                        if (aux != -1)
                        {
                            string stpNumber = step.action.Substring(aux, (step.action.Length - aux));

                            bool ExcludeStep = false;
                            foreach (var number in stpExcluded)
                            {
                                if (stpNumber.Equals(number))
                                {
                                    ExcludeStep = true;
                                }
                            }
                            if (ExcludeStep == false)
                            {


                                var stp = (from sup in context.TestSuplementals
                                           where sup.stp_number == stpNumber &&
                                           sup.Project_Id == projectId
                                           select sup.Test_Suplemental_Id).FirstOrDefault();
                                this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Procedure_Id, Status) VALUES ({0},{1},1)", stp, step.Test_Procedure_Id));

                                stpExcluded.Add(stpNumber);
                            }
                        }
                    }

                    break;

                case 3:
                    foreach (var step in steps)
                    {
                        step.creation_date = DateTime.UtcNow;

                    }

                    context.Steps.AddRange(steps);
                    context.SaveChanges();


                    foreach (var step in steps)
                    {
                        int aux = step.action.IndexOf("STP_");
                        if (aux != -1)
                        {
                            string stpNumber = step.action.Substring(aux, (step.action.Length - aux));

                            bool ExcludeStep = false;
                            foreach (var number in stpExcluded)
                            {
                                if (stpNumber.Equals(number))
                                {
                                    ExcludeStep = true;
                                }
                            }
                            if (ExcludeStep == false)
                            {


                                var stp = (from sup in context.TestSuplementals
                                           where sup.stp_number == stpNumber &&
                                           sup.Project_Id == projectId
                                           select sup.Test_Suplemental_Id).FirstOrDefault();
                                this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Scenario_Id, Status) VALUES ({0},{1},1)", stp, step.Test_Scenario_Id));

                                stpExcluded.Add(stpNumber);
                            }
                        }
                    }



                    break;



            }



            return steps;
        }

        public Step[] UpdateArray(Step[] steps, int projectId, int TypeOfSave, int TestEvidenceId, int Evidence)
        {

            switch (Evidence)
            {
                case 1:
                    this.ExecuteQuery(String.Format("DELETE FROM Steps WHERE Test_Suplemental_Id = {0}", TestEvidenceId));
                    foreach (var step in steps)
                    {
                        step.Test_Suplemental_Id = TestEvidenceId;
                        step.creation_date = DateTime.UtcNow;
                    }
                    break;
                case 2:
                    this.ExecuteQuery(String.Format("DELETE FROM Steps WHERE Test_Case_Id = {0}", TestEvidenceId));
                    foreach (var step in steps)
                    {
                        step.Test_Case_Id = TestEvidenceId;
                        step.creation_date = DateTime.UtcNow;
                    }
                    break;
                case 3:
                    this.ExecuteQuery(String.Format("DELETE FROM Steps WHERE Test_Scenario_Id = {0}", TestEvidenceId));

                    this.ExecuteQuery(String.Format("DELETE FROM Test_Procedure_Test_Suplemental WHERE Test_Scenario_Id = {0}", TestEvidenceId));
                    foreach (var step in steps)
                    {
                        step.Test_Scenario_Id = TestEvidenceId;
                        step.creation_date = DateTime.UtcNow;
                    }
                    break;
                case 4:
                    this.ExecuteQuery(String.Format("DELETE FROM Steps WHERE Test_Procedure_Id = {0}", TestEvidenceId));
                    this.ExecuteQuery(String.Format("DELETE FROM Test_Procedure_Test_Suplemental WHERE Test_Procedure_Id = {0}", TestEvidenceId));
                    foreach (var step in steps)
                    {
                        step.Test_Procedure_Id = TestEvidenceId;
                        step.creation_date = DateTime.UtcNow;
                    }
                    break;
            }


            List<string> stpExcluded = new List<string>();

            switch (TypeOfSave)
            {
                case 1:
                    context.Steps.AddRange(steps);
                    context.SaveChanges();

                    break;
                case 2:
                    context.Steps.AddRange(steps);
                    context.SaveChanges();


                    foreach (var step in steps)
                    {
                        int aux = step.action.IndexOf("STP_");
                        if (aux != -1)
                        {
                            string stpNumber = step.action.Substring(aux, (step.action.Length - aux));

                            bool ExcludeStep = false;
                            foreach (var number in stpExcluded)
                            {
                                if (stpNumber.Equals(number))
                                {
                                    ExcludeStep = true;
                                }
                            }
                            if (ExcludeStep == false)
                            {


                                var stp = (from sup in context.TestSuplementals
                                           where sup.stp_number == stpNumber &&
                                           sup.Project_Id == projectId
                                           select sup.Test_Suplemental_Id).FirstOrDefault();
                                this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Procedure_Id, Status) VALUES ({0},{1},1)", stp, step.Test_Procedure_Id));

                                stpExcluded.Add(stpNumber);
                            }
                        }
                    }

                    break;

                case 3:
                    foreach (var step in steps)
                    {
                        step.creation_date = DateTime.UtcNow;

                    }

                    context.Steps.AddRange(steps);
                    context.SaveChanges();


                    foreach (var step in steps)
                    {
                        int aux = step.action.IndexOf("STP_");
                        if (aux != -1)
                        {
                            string stpNumber = step.action.Substring(aux, (step.action.Length - aux));

                            bool ExcludeStep = false;
                            foreach (var number in stpExcluded)
                            {
                                if (stpNumber.Equals(number))
                                {
                                    ExcludeStep = true;
                                }
                            }
                            if (ExcludeStep == false)
                            {


                                var stp = (from sup in context.TestSuplementals
                                           where sup.stp_number == stpNumber &&
                                           sup.Project_Id == projectId
                                           select sup.Test_Suplemental_Id).FirstOrDefault();
                                this.ExecuteQuery(String.Format("INSERT INTO Test_Procedure_Test_Suplemental (Test_Suplemental_Id, Test_Scenario_Id, Status) VALUES ({0},{1},1)", stp, step.Test_Scenario_Id));

                                stpExcluded.Add(stpNumber);
                            }
                        }
                    }



                    break;



            }


            return steps;
        }

        public Step[] DeleteArray(Step[] steps)
        {
            try
            {
                foreach (var step in steps)
                {
                    var aux = context.Steps.Find(step.Id);
                    context.Steps.Remove(aux);
                }

                context.SaveChanges();
                return steps;
            }
            catch
            {
                return null;
            }
        }

        private bool ExecuteQuery(string query)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;
                var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);
                using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                {

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
