using Locus.Core.Context;
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
    public interface ITagRepository
    {
        Tag Save(Tag tag, int idtc, int idts, int idtp, int idstp);

        Tag Update(Tag tag);

        Tag Delete(int idTag, int idtc, int idts, int idtp, int idstp);

        List<Tag> GetAll();

        Tag Get(int idProject);

        List<Tag> GetTestCaseTags(int idtc);

        List<Tag> GetTestScenarioTags(int idts);

        List<Tag> GetTestProcedureTags(int idtp);

        List<Tag> GetTestSuplementalTags(int idstp);
    }

    public class TagRepository : ITagRepository
    {
        //Instance of Database Context
        private LocustDBContext context = new LocustDBContext();

        public Tag Delete(int idTag, int idtc, int idts, int idtp, int idstp)
        {
            // read connectionstring from config file
            var connectionString = ConfigurationManager.ConnectionStrings["LocustDBContext"].ConnectionString;

            var sqlConStrBuilder = new SqlConnectionStringBuilder(connectionString);

            var tag = context.Tags.Find(idTag);

            if (idtc != 0)
            {
                try
                {
                    using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                    {
                        var query = String.Format("DELETE FROM TestTags WHERE Tag_Id = {0}  AND Test_Case_Id = {1}",
                            idTag, idtc);

                        using (var command = new SqlCommand(query, connection))
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                    
                    return tag;
                }
                catch
                {
                    return null;
                }

            }
            else if (idts != 0)
            {
                try
                {
                    using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                    {
                        var query = String.Format("DELETE FROM TestTags WHERE Tag_Id = {0}  AND Test_Scenario_Id = {1}",
                            idTag, idts);

                        using (var command = new SqlCommand(query, connection))
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                    
                }
                catch
                {
                    return null;
                }
            }
            else if (idtp != 0)
            {
                try
                {
                    using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                    {
                        var query = String.Format("DELETE FROM TestTags WHERE Tag_Id = {0}  AND Test_Procedure_Id = {1}",
                            idTag, idtp);

                        using (var command = new SqlCommand(query, connection))
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                    return tag;
                }
                catch
                {
                    return null;
                }
            }
            else if (idstp != 0)
            {
                try
                {
                    using (var connection = new SqlConnection(sqlConStrBuilder.ConnectionString))
                    {
                        var query = String.Format("DELETE FROM TestTags WHERE Tag_Id = {0}  AND Test_Suplemental_Id = {1}",
                            idTag, idstp);

                        using (var command = new SqlCommand(query, connection))
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                    return tag;
                }
                catch
                {
                    return null;
                }

            }
            return tag;
        }

        public Tag Get(int idtag)
        {
            try
            {
                return context.Tags.Find(idtag);
                
            }
            catch
            {
                return null;
            }
        }


        public List<Tag> GetAll()
        {
            try
            {
                return context.Tags.ToList();
            }
            catch
            {
                return null;
            }
        }


        public List<Tag> GetTestCaseTags(int idtc)
        {
            var projectId = (from req in context.Requirements
                             join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                             where rt.Test_Case_Id == idtc
                             select req.Project_Id).FirstOrDefault();
            try
            {
               var tags = (from tag in context.Tags
                           join tt in context.test_tags on tag.id equals tt.Tag_Id
                           join tc in context.TestCases on tt.Test_Case_Id equals tc.Test_Case_Id
                           where tc.Test_Case_Id == idtc
                           where tag.Project_Id == projectId
                           select tag).ToList();

                return tags;
            }
            catch
            {
                return null;
            }
            
        }

        public List<Tag> GetTestProcedureTags(int idtp)
        {
            var projectId = (from req in context.Requirements
                             join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                             where rt.Test_Procedure_Id == idtp
                             select req.Project_Id).FirstOrDefault();
            try
            {
                var tags = (from tag in context.Tags
                            join tt in context.test_tags on tag.id equals tt.Tag_Id
                            join tc in context.TestProcedures on tt.Test_Procedure_Id equals tc.Test_Procedure_Id
                            where tc.Test_Procedure_Id == idtp
                            where tag.Project_Id == projectId
                            select tag).ToList();

                return tags;
            }
            catch
            {
                return null;
            }
        }

        public List<Tag> GetTestScenarioTags(int idts)
        {
            var projectId = (from req in context.Requirements
                             join rt in context.RequirementsTests on req.Id equals rt.Requirement_Id
                             where rt.Test_Scenario_Id == idts
                             select req.Project_Id).FirstOrDefault();
            try
            {
                var tags = (from tag in context.Tags
                            join tt in context.test_tags on tag.id equals tt.Tag_Id
                            join tc in context.TestScenarios on tt.Test_Scenario_Id equals tc.Test_Scenario_Id
                            where tc.Test_Scenario_Id == idts
                            where tag.Project_Id == projectId
                            select tag).ToList();

                return tags;
            }
            catch
            {
                return null;
            }
        }

        public List<Tag> GetTestSuplementalTags(int idstp)
        {
            var projectId = (from stps in context.TestSuplementals
                             where stps.Test_Suplemental_Id == idstp
                             select stps.Project_Id).FirstOrDefault();
            try
            {
                var tags = (from tag in context.Tags
                            join tt in context.test_tags on tag.id equals tt.Tag_Id
                            join tc in context.TestSuplementals on tt.Test_Suplemental_Id equals tc.Test_Suplemental_Id
                            where tc.Test_Suplemental_Id == idstp
                            where tag.Project_Id == projectId
                            select tag).ToList();

                return tags;
            }
            catch
            {
                return null;
            }
        }

        public Tag Save(Tag tag, int idtc, int idts, int idtp, int idstp)
        {
            if(idtc != 0)
            {
                LocustDBContext newContext = new LocustDBContext();
                    var name = tag.name.ToLower();
                    Tag exist = context.Tags.Where(x => x.name == name).Where(y => y.Project_Id == tag.Project_Id).FirstOrDefault();
                    int id;

                    if (exist == null)
                    {
                        //tag.Project_Id = projectId;
                        newContext.Tags.Add(tag);
                        newContext.SaveChanges();
                        Tag withId = newContext.Tags.Where(x => x.name == name).Where(y => y.Project_Id == tag.Project_Id).FirstOrDefault();
                        id = withId.id;
                        exist = withId;
                    }
                    else
                    {
                        id = exist.id;
                    }
                    LocustDBContext newContext2 = new LocustDBContext();
                    Test_Tags t = new Test_Tags();
                    t.Test_Case_Id = idtc;
                    t.Tag_Id = id;
                    newContext2.test_tags.Add(t);
                    newContext2.SaveChanges();
                    return exist;


                

            }else if(idts != 0)
            {
                LocustDBContext newContext = new LocustDBContext();
                try
                {
                    var name = tag.name.ToLower();
                    Tag exist = context.Tags.Where(x => x.name == name).Where(y => y.Project_Id == tag.Project_Id).FirstOrDefault();
                    int id;

                    if (exist == null)
                    {
                        newContext.Tags.Add(tag);
                        newContext.SaveChanges();
                        Tag withId = newContext.Tags.Where(x => x.name == name).Where(y => y.Project_Id == tag.Project_Id).FirstOrDefault();
                        id = withId.id;
                        exist = withId;
                    }
                    else
                    {
                        id = exist.id;
                    }
                    LocustDBContext newContext2 = new LocustDBContext();
                    Test_Tags t = new Test_Tags();
                    t.Test_Scenario_Id = idts;
                    t.Tag_Id = id;
                    newContext2.test_tags.Add(t);
                    newContext2.SaveChanges();
                    return exist;


                }
                catch
                {
                    return null;
                }
            }else if(idtp != 0)
            {
                LocustDBContext newContext = new LocustDBContext();

                try
                {
                    var name = tag.name.ToLower();
                    Tag exist = context.Tags.Where(x => x.name == name).Where(y => y.Project_Id == tag.Project_Id).FirstOrDefault();
                    int id;

                    if (exist == null)
                    {
                        newContext.Tags.Add(tag);
                        newContext.SaveChanges();
                        Tag withId = newContext.Tags.Where(x => x.name == name).Where(y => y.Project_Id == tag.Project_Id).FirstOrDefault();
                        id = withId.id;
                        exist = withId;
                    }
                    else
                    {
                        id = exist.id;
                    }
                    LocustDBContext newContext2 = new LocustDBContext();
                    Test_Tags t = new Test_Tags();
                    t.Test_Procedure_Id = idtp;
                    t.Tag_Id = id;
                    newContext2.test_tags.Add(t);
                    newContext2.SaveChanges();
                    return exist;


                }
                catch
                {
                    return null;
                }
            }
            else if (idstp != 0)
            {
                LocustDBContext newContext = new LocustDBContext();
                var projectId = (from stps in context.TestSuplementals
                                 where stps.Test_Suplemental_Id == idstp
                                 select stps.Project_Id).FirstOrDefault();

                try
                {
                    var name = tag.name.ToLower();
                    Tag exist = context.Tags.Where(x => x.name == name).Where(y => y.Project_Id == projectId).FirstOrDefault();
                    int id;

                    if (exist == null)
                    {
                        tag.Project_Id = projectId;
                        newContext.Tags.Add(tag);
                        newContext.SaveChanges();
                        Tag withId = newContext.Tags.Where(x => x.name == name).Where(y => y.Project_Id == projectId).FirstOrDefault();
                        id = withId.id;
                        exist = withId;
                    }
                    else
                    {
                        id = exist.id;
                    }
                    LocustDBContext newContext2 = new LocustDBContext();
                    Test_Tags t = new Test_Tags();
                    t.Test_Suplemental_Id = idstp;
                    t.Tag_Id = id;
                    newContext2.test_tags.Add(t);
                    newContext2.SaveChanges();
                    return exist;


                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }

        public Tag Update(Tag tag)
        {
            try
            {
                context.Entry(tag).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return tag;
            }
            catch
            {
                return null;
            }
        }
    }
}
