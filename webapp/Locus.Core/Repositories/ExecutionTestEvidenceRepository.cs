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
    public interface IExecutionTestEvidence
    {
        bool Save(ExecutionTestEvidence[] data, int executionid);

        ExecutionTestEvidence Update(int idExecutionGroup, int idRequirement);

        bool Delete(int executionId);

        List<ExecutionTestEvidence> GetAll();

        List<Requirement> GetForExecutionGroup(int executionGroup);

        List<EvidenceDTO> GetTCForExecutionGroup(int executionId);

        List<EvidenceDTO> GetTPForExecutionGroup(int executionId);

        List<EvidenceDTO> GetTSForExecutionGroup(int executionId);

        List<EvidenceDTO> GetAutomatedTestForExecutionGroup(int executionId);

        List<Scripts> GetScriptsByExecutionGroup(int executionId);

    }

    class ExecutionTestEvidenceRepository : IExecutionTestEvidence
    {
        private LocustDBContext context = new LocustDBContext();

        public bool Delete(int id)
        {
            this.ExecuteQuery(String.Format("DELETE FROM ExecutionTestEvidence WHERE Execution_Group_Id = {0}", id));
            return true;
        }

        public List<ExecutionTestEvidence> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Requirement> GetForExecutionGroup(int executionGroup)
        {/*
            var test = (from req in context.Requirements
                       join reqEx in context.ExecutionTest
                       on req.Id equals reqEx.Requirement_Id
                       where req.Id == reqEx.Requirement_Id
                       select req).ToList();
                       */
            return null;

        }

        public List<EvidenceDTO> GetTCForExecutionGroup(int executionId)
        {
            List<EvidenceDTO> testCasesDTO = new List<EvidenceDTO>();
            var listOfTc = (from tc in context.TestCases
                            join groups in context.ExecutionTest
                            on tc.Test_Case_Id equals groups.Tc_Id
                            where groups.Execution_Group_Id == executionId
                            select new { id = tc.Test_Case_Id, title = tc.Title, number = tc.tc_number, description = tc.Description, priority = tc.Test_Priority, type = tc.Type }).ToList();

            foreach (var item in listOfTc)
            {

                var reqId = (from req in context.Requirements
                             join rel in context.RequirementsTests
                             on req.Id equals rel.Requirement_Id
                             where rel.Test_Case_Id == item.id
                             select req.Id).FirstOrDefault();

                EvidenceDTO ev = new EvidenceDTO() { ReqId = reqId, Element_id = item.id, Identifier_number = item.number, Title = item.title, Description = item.description, Priority = item.priority, Evidence = "TC" };

                testCasesDTO.Add(ev);
            }



            return testCasesDTO;
        }

        public List<EvidenceDTO> GetAutomatedTestForExecutionGroup(int executionId)
        {
            List<EvidenceDTO> testProceduresDTO = new List<EvidenceDTO>();
            var listOfTc = (from tp in context.TestProcedures
                            join groups in context.ExecutionTest
                            on tp.Test_Procedure_Id equals groups.Tp_Id
                            where groups.Execution_Group_Id == executionId && tp.Script_Id != null
                            select new { id = tp.Test_Procedure_Id, title = tp.Title, number = tp.tp_number, description = tp.Description, priority = tp.Test_Priority, type = tp.Type }).ToList();

            foreach (var item in listOfTc)
            {

                var reqId = (from req in context.Requirements
                             join rel in context.RequirementsTests
                             on req.Id equals rel.Requirement_Id
                             where rel.Test_Procedure_Id == item.id
                             select req.Id).FirstOrDefault();


                EvidenceDTO ev = new EvidenceDTO() { ReqId = reqId, Element_id = item.id, Identifier_number = item.number, Title = item.title, Description = item.description, Priority = item.priority, Type = item.type, Evidence = "TP" };


                testProceduresDTO.Add(ev);
            }
            return testProceduresDTO;
            //foreach (var item in listOfTc)
            //{

            //    var reqId = (from req in context.Requirements
            //                 join rel in context.RequirementsTests
            //                 on req.Id equals rel.Requirement_Id
            //                 where rel.Test_Case_Id == item.id
            //                 select req.Id).FirstOrDefault();

            //    EvidenceDTO ev = new EvidenceDTO() { ReqId = reqId, Element_id = item.id, Identifier_number = item.number, Title = item.title, Description = item.description, Priority = item.priority, Evidence = "TC" };

            //    scripts.Add(ev);
            //}



            //return testCasesDTO;
        }


        public List<Scripts> GetScriptsByExecutionGroup(int executionId)
        {
            List<EvidenceDTO> testProceduresDTO = new List<EvidenceDTO>();
            var listOfTP = (from tp in context.TestProcedures
                            join groups in context.ExecutionTest
                            on tp.Test_Procedure_Id equals groups.Tp_Id
                            where groups.Execution_Group_Id == executionId && tp.Script_Id != null
                            select new TestProcedure { Script_Id = tp.Script_Id }).ToList();

            IEnumerable<int> ids = listOfTP.Select(tp => tp.Script_Id.Value).Distinct();
            List<Scripts> listScripts = new List<Scripts>();
            foreach (int id in ids)
            {
                Scripts script = context.Scripts.Where(s => s.Id == id).FirstOrDefault();
                if (script != null)
                {
                    listScripts.Add(script);
                }
            }
            return listScripts;
        }

        public List<EvidenceDTO> GetTPForExecutionGroup(int executionId)
        {
            List<EvidenceDTO> testProceduresDTO = new List<EvidenceDTO>();
            var listOfTc = (from tp in context.TestProcedures
                            join groups in context.ExecutionTest
                            on tp.Test_Procedure_Id equals groups.Tp_Id
                            where groups.Execution_Group_Id == executionId && tp.Script_Id == null
                            select new { id = tp.Test_Procedure_Id, title = tp.Title, number = tp.tp_number, description = tp.Description, priority = tp.Test_Priority, type = tp.Type }).ToList();

            foreach (var item in listOfTc)
            {

                var reqId = (from req in context.Requirements
                             join rel in context.RequirementsTests
                             on req.Id equals rel.Requirement_Id
                             where rel.Test_Procedure_Id == item.id
                             select req.Id).FirstOrDefault();


                EvidenceDTO ev = new EvidenceDTO() { ReqId = reqId, Element_id = item.id, Identifier_number = item.number, Title = item.title, Description = item.description, Priority = item.priority, Type = item.type, Evidence = "TP" };


                testProceduresDTO.Add(ev);
            }



            return testProceduresDTO;
        }

        public List<EvidenceDTO> GetTSForExecutionGroup(int executionId)
        {
            List<EvidenceDTO> testScenariosDTO = new List<EvidenceDTO>();
            var listOfTc = (from ts in context.TestScenarios
                            join groups in context.ExecutionTest
                            on ts.Test_Scenario_Id equals groups.Ts_Id
                            where groups.Execution_Group_Id == executionId
                            select new { id = ts.Test_Scenario_Id, title = ts.Title, number = ts.ts_number, description = ts.Description, priority = ts.Test_Priority, type = ts.Type }).ToList();





            foreach (var item in listOfTc)
            {
                var reqId = (from req in context.Requirements
                             join rel in context.RequirementsTests
                             on req.Id equals rel.Requirement_Id
                             where rel.Test_Scenario_Id == item.id
                             select req.Id).FirstOrDefault();

                //TestDTO test = new TestDTO();
                //test.TestId = item.id;
                //test.TestType = "TS";
                //test.Title = item.title;
                //test.Number = item.number;
                //test.reqId = reqId;

                EvidenceDTO ev = new EvidenceDTO() { ReqId = reqId, Element_id = item.id, Identifier_number = item.number, Title = item.title, Description = item.description, Priority = item.priority, Type = item.type, Evidence = "TS" };


                testScenariosDTO.Add(ev);
            }



            return testScenariosDTO;
        }

        public bool Save(ExecutionTestEvidence[] data, int executionid)
        {
            foreach (var item in data)
            {
                context.ExecutionTest.Add(item);
                context.SaveChanges();
            }
            var ex = context.ExecutionGroups.Find(executionid);
            ex.IsReadyToExecute = true;
            context.Entry(ex).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();


            return true;

        }

        public ExecutionTestEvidence Update(int idExecutionGroup, int idRequirement)
        {
            throw new NotImplementedException();
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
