using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using Locus.Core.Services;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
//using OfficeOpenXml.Utils;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Helpers
{
    public class ExcelCreator
    {

        private static readonly IProjectRepository _projectRepository = new ProjectRepository();
        private static readonly ITestSuplementalRepository _testSuplementalRepository = new TestSuplementalRepository();
        private static readonly IStepRepository _stepRepository = new StepRepository();
        private static readonly IRequirementRepository _requirementRepository = new RequirementRepository();
        private static readonly ITestCaseRepository _testCaseRepository = new TestCaseRepository();
        private static readonly ITestProcedureRepository _testProcedureRepository = new TestProcedureRepository();
        private static readonly IExecutionGroupRepository _executionGroupRepository = new ExecutionGroupRepository();
        private static readonly ITestResultRepository _testResultRepository = new TestResultRepository();
        private static readonly ITestExecutionRepository _testExecutionRepository = new TestExecutionRepository();
        private static readonly ITestEnvironmentRepository _testEnviromentRepository = new TestEnvironmentRepository();
        private static readonly IRunnersRepository _runnerRepository = new RunnerRepository();

        private readonly IProjectService _projectService = new ProjectService(_projectRepository);
        private readonly ITestSuplementalService _testSuplementalService = new TestSuplementalService(_testSuplementalRepository);
        private readonly IStepService _stepService = new StepService(_stepRepository);
        private readonly IRequirementService _requirementService = new RequirementService(_requirementRepository, _testSuplementalRepository);
        private readonly ITestCaseService _testCaseService = new TestCaseService(_testCaseRepository);
        private readonly ITestProcedureService _testProcedureService = new TestProcedureService(_testProcedureRepository);
        private readonly IExecutionGroupService _executionGroupService = new ExecutionGroupService(_executionGroupRepository);
        private readonly ITestResultService _testResultService = new TestResultService(_testResultRepository);
        private readonly ITestExecutionService _testExecutionService = new TestExecutionService(_testExecutionRepository);
        private readonly ITestEnvironmentService _testEnviromentService = new TestEnvironmentService(_testEnviromentRepository);
        private readonly IRunnersService _runnerService = new RunnerService(_runnerRepository);

        Color backGround = Color.FromArgb(31, 78, 120);
        //List on traceability
        List<LinkCellDTO> reqLinkList = new List<LinkCellDTO>();
        List<LinkCellDTO> reqTcLinkList = new List<LinkCellDTO>();
        List<LinkCellDTO> reqTsLinkList = new List<LinkCellDTO>();
        List<LinkCellDTO> reqTpLinkList = new List<LinkCellDTO>();

        //LINkS ON TEST CASES
        List<LinkCellDTO> testCaseLinkList = new List<LinkCellDTO>();
        List<LinkCellDTO> testCaseTpLinkList = new List<LinkCellDTO>();

        //LINKS ON TEST PROCEDURES
        List<LinkCellDTO> testProceduresLinkList = new List<LinkCellDTO>();
        List<LinkCellDTO> testProcedurestcLinkList = new List<LinkCellDTO>();

        //LINKS TO STP
        List<LinkCellDTO> tpToStpLinkList = new List<LinkCellDTO>();
        List<LinkCellDTO> tsToStpLinkList = new List<LinkCellDTO>();

        //STP LIST DTO
        List<LinkCellDTO> stpLinkList = new List<LinkCellDTO>();

        //LINKS ON TEST SCENARIOS
        List<LinkCellDTO> tsLinkList = new List<LinkCellDTO>();


        //LINK ON REQUERIMENTS SHEET
        List<LinkCellDTO> reqSheetList = new List<LinkCellDTO>();

        //List To Add Test Cases by Id, linked to the Test Procedure
        List<LinkCellDTO> testCasesId = new List<LinkCellDTO>();


        //List from Test Procedures to Test Cases
        List<LinkCellDTO> TpToTc = new List<LinkCellDTO>();

        //List from Of Test Cases id and names , to be inserted at the test procedures table
        List<LinkCellDTO> testCaseInfo = new List<LinkCellDTO>();

        //List from test cases to test procedures
        List<LinkCellDTO> TcToTp = new List<LinkCellDTO>();

        //List of Test Procedures id and names, to be insertet at the test cases tables
        List<LinkCellDTO> testProceduresInfo = new List<LinkCellDTO>();

        //List of all test evidence which will be included at the Traceability matrix.
        List<LinkCellDTO> testExecutedList = new List<LinkCellDTO>();

        //Test Evidence to Traceability (Its saves requirement number)
        List<LinkCellDTO> testCaseToTraceability = new List<LinkCellDTO>();
        List<LinkCellDTO> testProcedureToTraceability = new List<LinkCellDTO>();
        List<LinkCellDTO> testScenarioToTraceability = new List<LinkCellDTO>();




        int tcIndexValue = 2;
        public bool ExcelRequestor()
        {

            string path = System.Web.HttpContext.Current.Server.MapPath("~\\Files\\IsAvailable.txt");
            string readMeText = null;


            using (StreamReader readtext = new StreamReader(path))
            {
                readMeText = readtext.ReadLine();
                readtext.Close();
            }
            int aux = Int32.Parse(readMeText);
            if (aux == 1)
            {
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    writetext.WriteLine("0");
                    writetext.Close();
                }
                return true;

            }

            return false;
        }


        public byte[] GenerateExcel(Project project, string date, int executionId)
        {
            ExcelCreator ex = new ExcelCreator();


            try
            {
                List<TestSuplemental> suplementalList = ex._testSuplementalService.GetForProject(project.Id);
                List<Requirement> requirementList = ex._requirementService.GetProject(project.Id);
                DashboardDTO dashboard = ex._requirementService.GetDashboard(project.Id);

                ExecutionGroup projectGroup = null;
                List<TestDTO> testCases = ex._requirementService.GetTestCases(project.Id);
                List<TestDTO> testProcedures = ex._requirementService.GetTestProcedures(project.Id);
                List<TestDTO> testScenarios = ex._requirementService.GetTestScenarios(project.Id);

                if (executionId != -1)
                {
                    projectGroup = _executionGroupService.GetLastByProject(project.Id);
                    TestExecution lastTestExecution = _testExecutionService.GetLastExecution(projectGroup.Execution_Group_Id, project.Id);
                    if (lastTestExecution != null)
                    {
                        executionId = lastTestExecution.Test_Execution_Id;
                        requirementList = ex.AddInactiveRequirementExecuted(requirementList, executionId);
                    }
                    else
                    {
                        executionId = -1;
                    }

                }



                using (MemoryStream ms = new MemoryStream())
                {

                    var backupFolder = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
                    string projectNameFile = project.Name + "-" + date + ".xlsx";
                    string path = (Path.Combine(backupFolder, projectNameFile));
                    var fi = new FileInfo(path);
                    using (var package = new ExcelPackage())
                    {
                        try { 
                        addTraceability(requirementList, package, project, ex, dashboard, projectGroup, executionId);
                        addRequirements(requirementList, package, project, ex);
                        addTestCases(package, project, ex, executionId, testCases, requirementList);
                        addTestProcedures(package, project, ex, executionId, testProcedures, requirementList);
                        addTestScenarios(package, project, ex, executionId, testScenarios, requirementList);
                        addSuplemental(suplementalList, package, project, ex);
                        }catch(Exception e)
                        {
                            Debug.WriteLine(e.StackTrace);
                        }
                        addLinks(package);
                        package.SaveAs(fi);
                    }
                    FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    file.Close();
                    ms.Close();
                    string pathTxt = System.Web.HttpContext.Current.Server.MapPath("~\\Files\\IsAvailable.txt");
                    using (StreamWriter writetext = new StreamWriter(pathTxt))
                    {
                        writetext.WriteLine("1");
                        writetext.Close();
                    }
                    return bytes;
                }
            }
            catch (Exception e)
            {
                string path = System.Web.HttpContext.Current.Server.MapPath("~\\Files\\IsAvailable.txt");
                Debug.WriteLine(e.Message);
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    writetext.WriteLine("1");
                    writetext.Close();
                }
                return null;
            }

        }

        public bool GetIfRequirementWasExecuted(string req_number, List<Requirement> requirementList)
        {
            var req = requirementList.Find(r => r.req_number.Equals(req_number));

            if (req != null)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        public List<Requirement> AddInactiveRequirementExecuted(List<Requirement> requirementList, int executionId)
        {
            var SublistReqTc = _requirementService.GetNonActivesExecutedForTC(executionId);
            var SublistReqTs = _requirementService.GetNonActivesExecutedForTS(executionId);
            var SublistReqTp = _requirementService.GetNonActivesExecutedForTP(executionId);

            SublistReqTc = SublistReqTc.Concat(SublistReqTs).ToList();
            SublistReqTc = SublistReqTc.Concat(SublistReqTp).ToList();

            var PendingReqs = SublistReqTc.Distinct().ToList();


            requirementList = requirementList.Concat(PendingReqs).ToList();

            requirementList = requirementList.OrderBy(req => req.Id).ToList();
            return requirementList;


        }

        private void addTestScenarios(ExcelPackage package, Project project, ExcelCreator ex, int executionId, List<TestDTO> testScenarios, List<Requirement> requirementList)
        {
            var tsSheet = package.Workbook.Worksheets.Add("Test Scenarios");
            int rowValue = 2;
            tsSheet.Column(2).Width = 20;
            tsSheet.Column(5).Width = 15;
            tsSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            var first = 0;

            if (testScenarios != null)
            {
                foreach (var tc in testScenarios)
                {

                    bool print = true;
                    if (tc.reqStatus == false)
                    {
                        print = false;
                    }
                    first = 1;
                    TestResult result = null;
                    if (executionId != -1)
                    {
                        result = _testResultService.getForTestScenario(tc.Test_Id, executionId);
                        print = GetIfRequirementWasExecuted(tc.reqNumber, requirementList);
                    }
                    if (print)
                    {
                        first = rowValue;
                        LinkCellDTO cell = new LinkCellDTO();
                        //Header Project Name
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = project.Name;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rowValue++;
                        //TC Title
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Scenario";
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                        rowValue++;
                        //ID TITLE AND REQ NUMBER HEADERS
                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Value = " Test Scenario ID:";
                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Style.Font.Bold = true;
                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Value = "Priority";
                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Style.Font.Bold = true;
                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Requirement ID";
                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rowValue++;
                        //ID AND REQ NUMBER VALUES

                        LinkCellDTO link = new LinkCellDTO();
                        link.name = tc.IdentifiedNumber;
                        link.cell = "#'Test Scenarios'!$B$" + rowValue.ToString();
                        link.coor = "B" + rowValue.ToString() + ":C" + rowValue.ToString();
                        link.auxId = tc.Test_Id;
                        link.ReqNumber = tc.reqNumber;
                        tsLinkList.Add(link);

                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Value = tc.IdentifiedNumber;
                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Value = tc.Priority;
                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tc.reqNumber;
                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);

                        var linkCell = (from s in reqLinkList
                                        where s.name == tc.reqNumber
                                        select s).FirstOrDefault();


                        if (linkCell != null)
                            tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        rowValue++;
                        //TITLE
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Title: " + tc.Title;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                        rowValue++;
                        //Type
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Type: " + tc.Type;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                        rowValue++;

                        //DESCRIPTION
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Description: " + tc.Description;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        tsSheet.Row(rowValue).Height = 60;
                        rowValue++;
                        //PRECONDITION
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Preconditions: " + tc.Preconditions;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        tsSheet.Row(rowValue).Height = 60;
                        rowValue++;
                        //NOTES
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Notes: " + tc.Note;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        tsSheet.Row(rowValue).Height = 60;
                        tsSheet.Row(rowValue).Style.WrapText = true;
                        rowValue++;
                        //STEPS HEADERS
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "#";
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Actions";
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rowValue++;

                        int stepCounter = 1;
                        // List<StepDTO> stepsList = ex._stepService.GetForTestScenarioSTP(name.Id,tc.Test_Scenario_Id);
                        List<Step> stepsList = ex._stepService.GetForTestScenarioOrder(tc.Test_Id);
                        //STEPS NUMBER AND ACTIONS
                        foreach (var step in stepsList)
                        {
                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;

                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps;
                            tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                            tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;

                            string myaction = step.action;
                            int subaction = myaction.IndexOf("STP_");
                            if (subaction != -1)
                            {
                                string firstText = myaction.Substring(0, subaction); //Primera parte del string
                                string stp = myaction.Substring(subaction, myaction.Length - subaction); // Numero del STP
                                int indexStp = stp.IndexOf(" ");
                                if (indexStp != -1)
                                {
                                    string lastText = stp.Substring(indexStp, stp.Length - indexStp);
                                    stp = stp.Substring(0, indexStp);
                                    LinkCellDTO toStpLink = new LinkCellDTO();
                                    toStpLink.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                    toStpLink.name = stp;
                                    tsToStpLinkList.Add(toStpLink);
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;

                                }
                                else
                                {
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                                    LinkCellDTO toStpLink = new LinkCellDTO();
                                    toStpLink.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                    toStpLink.name = stp;
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                    tsToStpLinkList.Add(toStpLink);
                                }

                            }
                            else
                            {
                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                            }
                            if (step.type != null)
                            {
                                //int t = 0;

                                if (step.type.Equals("Expected Result"))
                                {
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps + "\n Expected Result";
                                    //tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                    tsSheet.Row(rowValue).Height = 55;
                                }
                                else
                                {
                                    stepCounter++;
                                }
                            }
                            else
                            {
                                stepCounter++;
                            }


                            rowValue++;
                        }


                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Date";
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                        if (result != null)
                        {

                            if (!result.Status.Equals("TBE"))
                            {
                                if (result.Execution_Date != null)
                                {
                                    DateTime time = DateTime.SpecifyKind(DateTime.Parse(result.Execution_Date.ToString()), DateTimeKind.Utc);
                                    DateTime time2 = time.ToLocalTime();
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = time2.ToString();
                                }

                            }
                        }
                        rowValue++;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Result";
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                        if (result != null)
                        {
                            if (result.Status.Equals("Fail"))
                            {
                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Result";
                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";

                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Execution_Result;
                            }
                            else
                            {
                                rowValue--;
                            }

                        }
                        else
                        {
                        }
                        rowValue++;

                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Result";
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                        if (result != null)
                        {

                            if (!result.Status.Equals("TBE"))
                            {
                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Status;
                            }

                        }
                        rowValue++;

                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Tester";
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                        if (result != null)
                        {
                            tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Tester;
                        }

                        ExcelRange range = tsSheet.Cells["B" + first.ToString() + ":G" + rowValue.ToString()];
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        range.Style.WrapText = true;
                        rowValue++;
                        rowValue++;
                    }


                }
            }



            if (first == 0)
            {
                package.Workbook.Worksheets.Delete(tsSheet);
            }



        }

        private void addTestProcedures(ExcelPackage package, Project project, ExcelCreator ex, int executionId, List<TestDTO> testProceduresList, List<Requirement> requirementList)
        {

            var tpSheet = package.Workbook.Worksheets.Add("Test Procedures");
            int rowValue = 2;
            tpSheet.Column(2).Width = 20;
            tpSheet.Column(5).Width = 15;
            tpSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            int first = 0;
            if (testProceduresList != null)
            {
                foreach (var tp in testProceduresList)
                {
                    try
                    {

                        bool print = true;
                        if (tp.reqStatus == false)
                        {
                            print = false;
                        }

                        first = 1;
                        TestResult result = null;
                        if (executionId != -1)
                        {
                            result = _testResultService.getForTestProcedure(tp.Test_Id, executionId);
                            print = GetIfRequirementWasExecuted(tp.reqNumber, requirementList);
                        }

                        if (print)
                        {



                            var linkCell = (from t in testCaseLinkList
                                            where t.auxId == tp.Related_Test_Id
                                            select t.name).FirstOrDefault();


                            first = rowValue;
                            LinkCellDTO cell = new LinkCellDTO();
                            //Header Project Name
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = project.Name;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rowValue++;
                            //TP Title
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Procedure";
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                            rowValue++;
                            //ID TITLE AND REQ NUMBER HEADERS
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = " Test Procedure ID:";
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            if (linkCell != null)
                            {
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Case ID";
                            }
                            else
                            {
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Requirement";
                            }


                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rowValue++;
                            //ID AND REQ NUMBER VALUES
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = tp.IdentifiedNumber;
                            LinkCellDTO link = new LinkCellDTO();
                            link.cell = "#'Test Procedures'!$B$" + rowValue.ToString();
                            link.name = tp.IdentifiedNumber;
                            link.ReqNumber = tp.reqNumber;
                            testProceduresLinkList.Add(link);
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            // AQUI!!!

                            if (linkCell != null)
                            {
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = linkCell;
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                LinkCellDTO test = new LinkCellDTO();
                                test.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                test.name = linkCell;
                                testProcedurestcLinkList.Add(test);

                            }
                            else
                            {
                                var reqLink = (from s in reqLinkList
                                               where s.name == tp.reqNumber
                                               select s).FirstOrDefault();


                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Hyperlink = new Uri(reqLink.cell + "", UriKind.Relative);
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tp.reqNumber;
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                            }

                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rowValue++;
                            //PRIORITY AND PROCEDURE Headers
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Procedure Designed: ";
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tp.Creator;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rowValue++;

                            //
                            //Title
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Title: " + tp.Title;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                            rowValue++;
                            //Type
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Type: " + tp.Type;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                            rowValue++;
                            //Description
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Description: " + tp.Description;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Row(rowValue).Height = 65;
                            rowValue++;
                            //STEPS HEADERS
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "#";
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Actions";
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                            tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            rowValue++;

                            //List<StepDTO> stepsList = ex._stepService.GetForTestProcedureSTP(name.Id,tp.Test_Procedure_Id);
                            List<Step> stepsList = ex._stepService.GetForTestProcedureOrder(tp.Test_Id);
                            //STEPS NUMBER AND ACTIONS
                            foreach (var step in stepsList)
                            {
                                tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps;
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;

                                string myaction = step.action;
                                int subaction = myaction.IndexOf("STP_");
                                if (subaction != -1)
                                {
                                    string firstText = myaction.Substring(0, subaction); //Primera parte del string
                                    string stp = myaction.Substring(subaction, myaction.Length - subaction); // Numero del STP
                                    int indexStp = stp.IndexOf(" ");
                                    if (indexStp != -1)
                                    {
                                        string lastText = stp.Substring(indexStp, stp.Length - indexStp);
                                        stp = stp.Substring(0, indexStp);
                                        LinkCellDTO toStpLink = new LinkCellDTO();
                                        toStpLink.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                        toStpLink.name = stp;
                                        tpToStpLinkList.Add(toStpLink);
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;

                                    }
                                    else
                                    {
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                                        LinkCellDTO toStpLink = new LinkCellDTO();
                                        toStpLink.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                        toStpLink.name = stp;
                                        tpToStpLinkList.Add(toStpLink);
                                    }

                                }
                                else
                                {
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                                }

                                rowValue++;
                            }

                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Expected Result";
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tp.ExpectedResult;
                            tpSheet.Row(rowValue).Height = 45;
                            rowValue++;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Date";
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                            if (result != null)
                            {
                                if (!result.Status.Equals("TBE"))
                                {

                                    DateTime time = DateTime.SpecifyKind(DateTime.Parse(result.Execution_Date.ToString()), DateTimeKind.Utc);
                                    DateTime time2 = time.ToLocalTime();


                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = time2.ToString();
                                }
                            }

                            rowValue++;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Result";
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                            if (result != null)
                            {
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Execution_Result;
                            }
                            rowValue++;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Result";
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                            if (result != null)
                            {
                                if (!result.Status.Equals("TBE"))
                                {
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Status;
                                }

                            }
                            rowValue++;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Tester";
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                            tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                            if (result != null)
                            {
                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Tester;
                            }

                            ExcelRange range = tpSheet.Cells["B" + first.ToString() + ":G" + rowValue.ToString()];
                            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            range.Style.WrapText = true;


                            rowValue++;
                            rowValue++;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }
                }
            }

            if (first == 0)
            {
                package.Workbook.Worksheets.Delete(tpSheet);
            }


        }


        public void addSuplemental(List<TestSuplemental> testSuplementals, ExcelPackage package, Project project, ExcelCreator ex)
        {
            var stpSheet = package.Workbook.Worksheets.Add("Supplemental Test");
            int rowValue = 3; //We start writting the excel at row 3
            stpSheet.Column(2).Width = 10;
            stpSheet.Column(5).Width = 15;
            stpSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            int first = 0;
            if (testSuplementals.Count > 0)
            {


                foreach (var stp in testSuplementals)
                {
                    first = rowValue;
                    LinkCellDTO cell = new LinkCellDTO();
                    //Header Project Name
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = project.Name;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowValue++;
                    //STP Title
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Supplemental Test Procedure";
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                    rowValue++;
                    //ID TITLE AND NUMBER
                    stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Supplemental Test Procedure ID:";
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                    stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = stp.stp_number;

                    cell.cell = "#'Supplemental Test'!$E$" + rowValue.ToString();
                    cell.name = stp.stp_number;
                    stpLinkList.Add(cell);
                    stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                    stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowValue++;

                    //Title
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Title: " + stp.Title;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    rowValue++;

                    //DESCRIPTION
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Description: " + stp.Description;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    stpSheet.Row(rowValue).Height = 65;
                    rowValue++;
                    //DESIGNED
                    stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Procedure Designed:";
                    stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = stp.Test_Procedure_Creator;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                    rowValue++;
                    //STEPS HEADERS
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                    stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "#";
                    stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Actions";
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                    stpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowValue++;

                    List<Step> stepsList = ex._stepService.GetForTestSuplementalOrder(stp.Test_Suplemental_Id);
                    //STEPS NUMBER AND ACTIONS
                    foreach (var step in stepsList)
                    {
                        stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                        stpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps;
                        stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                        stpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;



                        rowValue++;
                    }
                    ExcelRange range = stpSheet.Cells["B" + first.ToString() + ":G" + (rowValue - 1).ToString()];
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.WrapText = true;

                    rowValue++;

                }
            }
            else
            {

                package.Workbook.Worksheets.Delete(stpSheet);
            }
        }

        public void addTraceability(List<Requirement> requirements, ExcelPackage package, Project project, ExcelCreator ex, DashboardDTO dashboard, ExecutionGroup group, int executionId)
        {
            var tracSheet = package.Workbook.Worksheets.Add("Traceability");
            int rowValue = 5;
            //wIDTHS BY COLUMNS
            tracSheet.Column(2).Width = 30;
            tracSheet.Column(3).Width = 60;
            tracSheet.Column(4).Width = 30;
            tracSheet.Column(6).Width = 40;
            tracSheet.Column(7).Width = 30;

            //HEADERS SETUP
            tracSheet.Cells["B3:D3"].Merge = true;
            tracSheet.Cells["B3:D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["B3:D3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["B3:D3"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["B3:D3"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["B3:D3"].Style.Font.Bold = true;
            tracSheet.Cells["B3:D3"].Value = project.Name;
            //ID
            tracSheet.Cells["B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["B4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["B4"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["B4"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["B4"].Style.Font.Bold = true;
            tracSheet.Cells["B4"].Value = "Requirements ID";
            //Section
            tracSheet.Cells["C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["C4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["C4"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["C4"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["C4"].Style.Font.Bold = true;
            tracSheet.Cells["C4"].Value = "Section";
            //Test Cases
            tracSheet.Cells["D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["D4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["D4"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["D4"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["D4"].Style.Font.Bold = true;
            tracSheet.Cells["D4"].Value = "Test Evidence";

            foreach (var req in requirements)
            {
                bool aux = false;
                int cellsToMerge = 1;
                List<TestCase> auxList = _requirementService.GetAllTestCase(req.Id);
                List<TestProcedure> listProc = _requirementService.GetAllTestProcedureNoTc(req.Id);
                List<TestScenario> listScen = _requirementService.GetAllTestScenario(req.Id);

                if (auxList.Count != 0 || listProc.Count != 0 || listScen.Count != 0)
                {
                    cellsToMerge = (auxList.Count + listScen.Count + listProc.Count + rowValue) - 1;
                }
                else
                {
                    aux = true;
                    cellsToMerge = rowValue;
                }

                LinkCellDTO cell = new LinkCellDTO();
                tracSheet.Cells["B" + rowValue.ToString()].Value = req.req_number;
                tracSheet.Cells["B" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                tracSheet.Cells["B" + rowValue.ToString()].Style.Font.UnderLine = true;
                tracSheet.Cells["C" + rowValue.ToString()].Value = req.Name;

                cell.cell = "#'Traceability'!$B$" + rowValue.ToString();
                cell.name = req.req_number;
                cell.coor = "B" + rowValue.ToString();
                cell.auxId = req.Id;

                reqLinkList.Add(cell);

                tracSheet.Cells["B" + rowValue.ToString() + ":B" + cellsToMerge.ToString()].Merge = true;
                tracSheet.Cells["C" + rowValue.ToString() + ":C" + cellsToMerge.ToString()].Merge = true;

                tracSheet.Cells["B" + rowValue.ToString() + ":B" + cellsToMerge.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["C" + rowValue.ToString() + ":C" + cellsToMerge.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                tracSheet.Cells["B" + rowValue.ToString() + ":B" + cellsToMerge.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                tracSheet.Cells["C" + rowValue.ToString() + ":C" + cellsToMerge.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                if (auxList.Count != 0)
                {
                    foreach (var tc in auxList)
                    {
                        if (tc.Status == true)
                        {


                            LinkCellDTO cell2 = new LinkCellDTO();
                            cell2.coor = "D" + rowValue.ToString();
                            cell2.name = tc.tc_number;
                            cell2.ReqNumber = req.req_number;
                            reqTcLinkList.Add(cell2);
                            tracSheet.Cells["D" + rowValue.ToString()].Value = tc.tc_number;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.Font.UnderLine = true;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);

                            rowValue++;
                        }
                    }
                }


                if (listProc.Count != 0)
                {
                    foreach (var tp in listProc)
                    {
                        if (tp.Status == true)
                        {

                            LinkCellDTO cell2 = new LinkCellDTO();
                            cell2.coor = "D" + rowValue.ToString();
                            cell2.name = tp.tp_number;
                            cell2.ReqNumber = req.req_number;
                            reqTpLinkList.Add(cell2);
                            tracSheet.Cells["D" + rowValue.ToString()].Value = tp.tp_number;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.Font.UnderLine = true;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                            rowValue++;
                        }
                    }
                }


                if (listScen.Count != 0)
                {
                    foreach (var ts in listScen)
                    {
                        if (ts.Status == true)
                        {
                            LinkCellDTO cell2 = new LinkCellDTO();
                            cell2.coor = "D" + rowValue.ToString();
                            cell2.name = ts.ts_number;
                            cell2.ReqNumber = req.req_number;
                            reqTsLinkList.Add(cell2);
                            tracSheet.Cells["D" + rowValue.ToString()].Value = ts.ts_number;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.Font.UnderLine = true;
                            tracSheet.Cells["D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                            rowValue++;
                        }
                    }
                }

                if (aux)
                {
                    rowValue++;
                }


            }



            //SUMMARY
            tracSheet.Cells["F3:G4"].Merge = true;
            tracSheet.Cells["F3:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["F3:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            tracSheet.Cells["F3:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["F3:G4"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["F3:G4"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["F3:G4"].Style.Font.Bold = true;
            tracSheet.Cells["F3:G4"].Value = "PROJECT SUMMARY";

            tracSheet.Cells["F5:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["G5:G10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["F5"].Value = "Total Requirements";
            tracSheet.Cells["F6"].Value = "Total Test Cases";
            tracSheet.Cells["F7"].Value = "Total Test Procedures";
            tracSheet.Cells["F8"].Value = "Total Test Scenarios";
            tracSheet.Cells["F9"].Value = "Total Supplemental Test Procedures";
            tracSheet.Cells["F10"].Value = "Requirements Missing Test Evidence";

            tracSheet.Cells["G5"].Value = dashboard.totalRequirements;
            tracSheet.Cells["G6"].Value = dashboard.totalTestCases;
            tracSheet.Cells["G7"].Value = dashboard.totalTestProcedures;
            tracSheet.Cells["G8"].Value = dashboard.totalTestScenarios;
            tracSheet.Cells["G9"].Value = dashboard.totalstp;
            tracSheet.Cells["G10"].Value = dashboard.requirementsMissingTestEvidence;

            //Execution Summary 

            if (executionId != -1)
            {
                tracSheet.Cells["F12:G13"].Merge = true;
                tracSheet.Cells["F12:G13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["F12:G13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                tracSheet.Cells["F12:G13"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                tracSheet.Cells["F12:G13"].Style.Fill.BackgroundColor.SetColor(backGround);
                tracSheet.Cells["F12:G13"].Style.Font.Color.SetColor(Color.White);
                tracSheet.Cells["F12:G13"].Style.Font.Bold = true;
                tracSheet.Cells["F12:G13"].Value = "Test Execution Summary";

                tracSheet.Cells["F14:F22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["G14:G22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["F14"].Value = "Description";
                tracSheet.Cells["F15"].Value = "Total Evidence";
                tracSheet.Cells["F16"].Value = "Test Strategy";
                tracSheet.Cells["F17"].Value = "Total Test Passed";
                tracSheet.Cells["F18"].Value = "Total Test Failed";
                tracSheet.Cells["F19"].Value = "Total Test To Be Executed";
                tracSheet.Cells["F20"].Value = "Percentaje Pass";
                tracSheet.Cells["F21"].Value = "Percentaje Fail";
                tracSheet.Cells["F22"].Value = "Percentaje TBE";


                tracSheet.Cells["G14"].Value = "This report contain the test results of execution group '" + group.Name + "'" + "\n" + " which only has the test results of the test evidence executed on the group";
                tracSheet.Cells["G14"].Style.WrapText = true;



                List<TestResult> results = _testResultService.getForExecutionGroup(executionId);

                int TotalEvidence = results.Count();
                decimal TP = results.Where(r => r.Evidence.Equals("TP")).ToList().Count;
                decimal TS = results.Where(r => r.Evidence.Equals("TS")).ToList().Count;
                decimal TC = results.Where(r => r.Evidence.Equals("TC")).ToList().Count;

                decimal TotalPass = results.Where(r => r.Status.Equals("Pass")).ToList().Count;
                decimal TotalFail = results.Where(r => r.Status.Equals("Fail")).ToList().Count;
                decimal TotalTBE = results.Where(r => r.Status.Equals("TBE")).ToList().Count;


                decimal PercentagePass = (TotalPass / results.Count) * 100m;

                decimal PercentageFail = (TotalFail / results.Count) * 100m;

                decimal PercentageTBE = (TotalTBE / results.Count) * 100m;

                string Strategy = null;

                if (TP >= 1 && TC == 0 && TS == 0)
                {
                    Strategy = "Only TP";
                }

                if (TC >= 1 && TP == 0 && TS == 0)
                {
                    Strategy = "Only TC";
                }

                if (TS >= 1 && TP == 0 && TC == 0)
                {
                    Strategy = "Only TS";
                }

                if (TS >= 1 && TP >= 1 || TC >= 1)
                {
                    Strategy = "Hybrid";
                }



                tracSheet.Cells["G15"].Value = TotalEvidence;
                tracSheet.Cells["G16"].Value = Strategy;
                tracSheet.Cells["G17"].Value = TotalPass;
                tracSheet.Cells["G18"].Value = TotalFail;
                tracSheet.Cells["G19"].Value = TotalTBE;
                tracSheet.Cells["G20"].Value = Math.Round(PercentagePass,3).ToString() + "%";
                tracSheet.Cells["G21"].Value = Math.Round(PercentageFail,3).ToString()+"%";
                tracSheet.Cells["G22"].Value = Math.Round(PercentageTBE,3).ToString()+"%";


                try { 
                //Test Environment 
                TestEnvironment testEnvironment = _testEnviromentService.Get(group.TestEnvironmentId.Value);
                tracSheet.Cells["F24:G25"].Merge = true;
                tracSheet.Cells["F24:G25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["F24:G25"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                tracSheet.Cells["F24:G25"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                tracSheet.Cells["F24:G25"].Style.Fill.BackgroundColor.SetColor(backGround);
                tracSheet.Cells["F24:G25"].Style.Font.Color.SetColor(Color.White);
                tracSheet.Cells["F24:G25"].Style.Font.Bold = true;
                tracSheet.Cells["F24:G25"].Value = "Test Enviroment";

                tracSheet.Cells["F26"].Value = "Name";
                tracSheet.Cells["F27"].Value = "Server";
                tracSheet.Cells["F28"].Value = "Processor";
                tracSheet.Cells["F29"].Value = "RAM";
                tracSheet.Cells["F30"].Value = "HardDisk";
                tracSheet.Cells["F31"].Value = "OS";
                tracSheet.Cells["F32"].Value = "Dev Server";
                tracSheet.Cells["F33"].Value = "Test Server";
                tracSheet.Cells["F34"].Value = "Database";
                tracSheet.Cells["F35"].Value = "URL";
                tracSheet.Cells["F36"].Value = "Site Type";
                tracSheet.Cells["F37"].Value = "Creator";
                tracSheet.Cells["F38"].Value = "Last Editor";
                tracSheet.Cells["F39"].Value = "Notes";

                tracSheet.Cells["G26"].Value = testEnvironment.Name;
                tracSheet.Cells["G27"].Value = testEnvironment.Server;
                tracSheet.Cells["G28"].Value = testEnvironment.Processor;
                tracSheet.Cells["G29"].Value = testEnvironment.RAM;
                tracSheet.Cells["G30"].Value = testEnvironment.HardDisk;
                tracSheet.Cells["G31"].Value = testEnvironment.OS;
                tracSheet.Cells["G32"].Value = testEnvironment.ServerSoftwareDevs;
                tracSheet.Cells["G33"].Value = testEnvironment.ServerSoftwareTest;
                tracSheet.Cells["G34"].Value = testEnvironment.Database;
                tracSheet.Cells["G35"].Value = testEnvironment.URL;
                tracSheet.Cells["G36"].Value = testEnvironment.SiteType;
                tracSheet.Cells["G37"].Value = testEnvironment.Creator;
                tracSheet.Cells["G38"].Value = testEnvironment.Last_Editor;
                tracSheet.Cells["G39"].Value = testEnvironment.Notes;

                tracSheet.Cells["F26:F39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["G26:G39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }catch(Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }


            }




            if (executionId == -1)
            {

                if (group != null)
                {
                    //TestEnviroment
                    TestEnvironment testEnvironment = _testEnviromentService.Get(group.TestEnvironmentId.Value);
                    tracSheet.Cells["F13:G14"].Merge = true;
                    tracSheet.Cells["F13:G14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tracSheet.Cells["F13:G14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    tracSheet.Cells["F13:G14"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    tracSheet.Cells["F13:G14"].Style.Fill.BackgroundColor.SetColor(backGround);
                    tracSheet.Cells["F13:G14"].Style.Font.Color.SetColor(Color.White);
                    tracSheet.Cells["F13:G14"].Style.Font.Bold = true;
                    tracSheet.Cells["F13:G14"].Value = "Test Enviroment";

                    tracSheet.Cells["F15"].Value = "Name";
                    tracSheet.Cells["F16"].Value = "Server";
                    tracSheet.Cells["F17"].Value = "Processor";
                    tracSheet.Cells["F18"].Value = "RAM";
                    tracSheet.Cells["F19"].Value = "HardDisk";
                    tracSheet.Cells["F20"].Value = "OS";
                    tracSheet.Cells["F21"].Value = "Dev Server";
                    tracSheet.Cells["F22"].Value = "Test Server";
                    tracSheet.Cells["F23"].Value = "Database";
                    tracSheet.Cells["F24"].Value = "URL";
                    tracSheet.Cells["F25"].Value = "Site Type";
                    tracSheet.Cells["F26"].Value = "Creator";
                    tracSheet.Cells["F27"].Value = "Last Editor";
                    tracSheet.Cells["F28"].Value = "Notes";

                    tracSheet.Cells["G15"].Value = testEnvironment.Name;
                    tracSheet.Cells["G16"].Value = testEnvironment.Server;
                    tracSheet.Cells["G17"].Value = testEnvironment.Processor;
                    tracSheet.Cells["G18"].Value = testEnvironment.RAM;
                    tracSheet.Cells["G19"].Value = testEnvironment.HardDisk;
                    tracSheet.Cells["G20"].Value = testEnvironment.OS;
                    tracSheet.Cells["G21"].Value = testEnvironment.ServerSoftwareDevs;
                    tracSheet.Cells["G22"].Value = testEnvironment.ServerSoftwareTest;
                    tracSheet.Cells["G23"].Value = testEnvironment.Database;
                    tracSheet.Cells["G24"].Value = testEnvironment.URL;
                    tracSheet.Cells["G25"].Value = testEnvironment.SiteType;
                    tracSheet.Cells["G26"].Value = testEnvironment.Creator;
                    tracSheet.Cells["G27"].Value = testEnvironment.Last_Editor;
                    tracSheet.Cells["G28"].Value = testEnvironment.Notes;

                    tracSheet.Cells["F15:F28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tracSheet.Cells["G15:G28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }





        }

        public void addTestCases(ExcelPackage package, Project project, ExcelCreator ex, int executionId, List<TestDTO> testCasesDTO, List<Requirement> requirementList)
        {
            var tcSheet = package.Workbook.Worksheets.Add("Test Cases");
            int rowValue = 2;
            tcSheet.Column(2).Width = 20;
            tcSheet.Column(5).Width = 15;
            tcSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            int first = 0;

            foreach (var tc in testCasesDTO)
            {



                first = 1;
                TestResult result = null;

                first = rowValue;
                LinkCellDTO cell = new LinkCellDTO();
                //Header Project Name
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = project.Name;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;
                //TC Title
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Case";
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                rowValue++;
                //ID TITLE AND REQ NUMBER HEADERS
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = " Test Case ID:";
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Requirement";
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;
                //ID AND REQ NUMBER VALUES
                LinkCellDTO link = new LinkCellDTO();
                link.name = tc.IdentifiedNumber;
                link.cell = "#'Test Cases'!$B$" + rowValue.ToString();
                link.coor = "B" + rowValue.ToString() + ":G" + rowValue.ToString();
                link.auxId = tc.Test_Id;
                link.ReqNumber = tc.reqNumber;
                testCaseLinkList.Add(link);
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = tc.IdentifiedNumber;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tc.reqNumber;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);

                var linkCell = (from s in reqLinkList
                                where s.name == tc.reqNumber
                                select s).FirstOrDefault();


                if (linkCell != null)
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;
                //PRIORITY AND PROCEDURE Headers
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Priority: ";
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Procedure";
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;
                //Priorirty And TC
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = tc.Priority;
                var tp = _testProcedureService.GetTestProcedure(tc.Test_Id);
                if (tp != null)
                {
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tp.tp_number;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                    LinkCellDTO linkDTO = new LinkCellDTO();
                    linkDTO.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                    linkDTO.name = tp.tp_number;

                    testCaseTpLinkList.Add(linkDTO);
                }
                else
                {
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "N/A";
                    //Getting Test Result for Test CASE
                    if (executionId != -1)
                    {
                        result = _testResultService.getForTestCase(tc.Test_Id, executionId);
                    }

                }


                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                rowValue++;
                //TITLE
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Title: " + tc.Title;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                rowValue++;
                //Type
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Type: " + tc.Type;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                rowValue++;
                //DESCRIPTION
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Description: " + tc.Description;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                tcSheet.Row(rowValue).Height = 65;
                rowValue++;
                //PRECONDITION
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Preconditions: " + tc.Preconditions;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                tcSheet.Row(rowValue).Height = 65;
                rowValue++;
                //STEPS HEADERS
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "#";
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Actions";
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;

                List<Step> stepsList = ex._stepService.GetForTestCaseOrder(tc.Test_Id);
                //STEPS NUMBER AND ACTIONS
                foreach (var step in stepsList)
                {
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                    rowValue++;
                }

                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Expected Result";
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tc.ExpectedResult;
                tcSheet.Row(rowValue).Height = 45;
                rowValue++;
                if (tp == null)
                {

                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Date";
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                    if (result != null)
                    {
                        if (!result.Status.Equals("TBE"))
                        {

                            DateTime time = DateTime.SpecifyKind(DateTime.Parse(result.Execution_Date.ToString()), DateTimeKind.Utc);
                            DateTime time2 = time.ToLocalTime();


                            tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = time2.ToString();
                        }

                    }

                    rowValue++;


                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Result";
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                    if (result != null)
                    {
                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Execution_Result;
                    }

                    rowValue++;


                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Result";
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                    if (result != null)
                    {
                        if (!result.Status.Equals("TBE"))
                        {
                            tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Status;
                        }
                    }

                    rowValue++;



                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Tester";
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                    if (result != null)
                    {
                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Tester;
                    }

                    rowValue++;


                }

                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Case Designed";
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tc.Creator;
                ExcelRange range = tcSheet.Cells["B" + first.ToString() + ":G" + rowValue.ToString()];
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.WrapText = true;
                rowValue++;
                rowValue++;

            }


            if (first == 0)
            {
                package.Workbook.Worksheets.Delete(tcSheet);
            }
        }

        public void addRequirements(List<Requirement> requirements, ExcelPackage package, Project project, ExcelCreator ex)
        {
            var reqSheet = package.Workbook.Worksheets.Add("Requirements");
            int rowValue = 3; //We start writting the excel at row 3
            reqSheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            reqSheet.Cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
            reqSheet.Cells["B1:I2"].Merge = true;
            reqSheet.Cells["B1:I2"].Value = project.Name;
            reqSheet.Cells["B1:I2"].Style.Font.Size = 20;
            reqSheet.Cells["B1:I2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            reqSheet.Cells["B1:I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;




            reqSheet.Cells["B" + rowValue.ToString()].Value = "# REQ";
            reqSheet.Cells["C" + rowValue.ToString()].Value = "AXOSOFT ID";
            reqSheet.Cells["D" + rowValue.ToString()].Value = "REQUIREMENT";
            reqSheet.Cells["E" + rowValue.ToString()].Value = "DESCRIPTION";
            reqSheet.Cells["F" + rowValue.ToString()].Value = "ACEPTANCE CRITERIA";
            reqSheet.Cells["G" + rowValue.ToString()].Value = "DEVELOPER";
            reqSheet.Cells["H" + rowValue.ToString()].Value = "TESTER";
            reqSheet.Cells["I" + rowValue.ToString()].Value = "RELEASE";
            reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
            reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(89, 89, 89));
            reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Font.Size = 9;
            reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Font.Color.SetColor(Color.FromArgb(255, 255, 255));
            reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Font.Bold = true;

            rowValue++;

            var flag = false;

            foreach (var req in requirements)
            {
                reqSheet.Row(rowValue).Height = 25;
                reqSheet.Cells["B" + rowValue.ToString()].Value = req.req_number;

                var linkCell = (from s in reqLinkList
                                where s.name == req.req_number
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    reqSheet.Cells["B" + rowValue.ToString()].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                    reqSheet.Cells["B" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                    reqSheet.Cells["B" + rowValue.ToString()].Style.Font.UnderLine = true;
                }





                reqSheet.Cells["C" + rowValue.ToString()].Value = req.Axosoft_Task_Id;
                reqSheet.Cells["D" + rowValue.ToString()].Value = req.Name;
                reqSheet.Cells["E" + rowValue.ToString()].Value = req.Description;
                reqSheet.Cells["F" + rowValue.ToString()].Value = req.Acceptance_Criteria;
                reqSheet.Cells["G" + rowValue.ToString()].Value = req.Developer_Assigned;
                reqSheet.Cells["H" + rowValue.ToString()].Value = req.Tester_Assigned;
                reqSheet.Cells["I" + rowValue.ToString()].Value = req.Release;

                //Add Req Link

                LinkCellDTO link = new LinkCellDTO();
                link.name = req.req_number;
                link.cell = "#'Requirements'!$B$" + rowValue.ToString();
                link.coor = "B" + rowValue.ToString();
                reqSheetList.Add(link);


                reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Font.Size = 10;
                reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                if (flag)
                {
                    reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                    flag = false;
                }
                else
                {
                    reqSheet.Cells["B" + rowValue.ToString() + ":I" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));
                    flag = true;
                }
                rowValue++;
            }
            reqSheet.Column(1).Width = 2;
            reqSheet.Column(5).Width = 50;
            reqSheet.Column(6).Width = 50;
            reqSheet.Column(5).Style.WrapText = true;
            reqSheet.Column(6).Style.WrapText = true;
            reqSheet.Cells["B:D"].AutoFitColumns();
            reqSheet.Cells["G:I"].AutoFitColumns();

            reqSheet.Cells[$"B{3}:I{rowValue}"].AutoFilter = true;

        }

        private void addLinks(ExcelPackage package)
        {
            var tracSheet = package.Workbook.Worksheets["Traceability"];
            var testCaseSheet = package.Workbook.Worksheets["Test Cases"];
            var testProcedureSheet = package.Workbook.Worksheets["Test Procedures"];
            var testScenariosSheet = package.Workbook.Worksheets["Test Scenarios"];
            var requirementsSheet = package.Workbook.Worksheets["Requirements"];
            foreach (var tcinreq in reqTcLinkList)
            {
                var linkCell = (from s in testCaseLinkList
                                where s.name == tcinreq.name
                                && s.ReqNumber == tcinreq.ReqNumber
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    tracSheet.Cells[tcinreq.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }
            foreach (var reqtoTrac in reqLinkList)
            {
                var linkCell = (from s in reqSheetList
                                where s.name == reqtoTrac.name
                                select new { s.cell }).First();

                if (linkCell != null)
                {
                    tracSheet.Cells[reqtoTrac.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }


            foreach (var tcinreq in reqTsLinkList)
            {
                var linkCell = (from s in tsLinkList
                                where s.name == tcinreq.name
                                && s.ReqNumber == tcinreq.ReqNumber
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    tracSheet.Cells[tcinreq.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }

            foreach (var tcinreq in reqTpLinkList)
            {
                var linkCell = (from s in testProceduresLinkList
                                where s.name == tcinreq.name
                                && s.ReqNumber == tcinreq.ReqNumber
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    tracSheet.Cells[tcinreq.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }

            foreach (var tpintc in testCaseTpLinkList)
            {
                var linkCell = (from s in testProceduresLinkList
                                where s.name == tpintc.name
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    testCaseSheet.Cells[tpintc.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }

            foreach (var tcintp in testProcedurestcLinkList)
            {
                var linkCell = (from s in testCaseLinkList
                                where s.name == tcintp.name
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    testProcedureSheet.Cells[tcintp.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }

            foreach (var tcintp in tpToStpLinkList)
            {
                var linkCell = (from s in stpLinkList
                                where s.name == tcintp.name
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    testProcedureSheet.Cells[tcintp.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }

            foreach (var tcintp in tsToStpLinkList)
            {
                var linkCell = (from s in stpLinkList
                                where s.name == tcintp.name
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    testScenariosSheet.Cells[tcintp.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }
        }

        public byte[] GenerateExecutionReport(Project project, string date, int executionId)
        {



            ExcelCreator ex = new ExcelCreator();
            List<TestSuplemental> suplementalList = ex._testSuplementalService.GetForProject(project.Id);
            List<Requirement> requirementList = ex._requirementService.GetProject(project.Id);
            DashboardDTO dashboard = ex._requirementService.GetDashboard(project.Id);

            ExecutionGroup projectGroup = _testExecutionService.GetByTestExecution(executionId);
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {

                    var backupFolder = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\");
                    string projectNameFile = project.Name + "-" + date + ".xlsx";
                    string path = (Path.Combine(backupFolder, projectNameFile));
                    var fi = new FileInfo(path);
                    using (var package = new ExcelPackage())
                    {

                        //  addRequirements(requirementList, package, project.Name, ex);
                        var TestCasesSheet = AddTestCasesExecuted(requirementList, package, project, ex, executionId);
                        addTestProceduresExecuted(requirementList, package, project, ex, executionId, TestCasesSheet);
                        addTestScenariosExecuted(requirementList, package, project, ex, executionId);
                        addSuplemental(suplementalList, package, project, ex);
                        ExecutionTraceability(requirementList, package, project.Name, ex, dashboard, projectGroup, executionId);
                        addRequirements(requirementList, package, project, ex);
                        AddExecutionLinks(package);
                        package.SaveAs(fi);
                    }
                    FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    file.Close();
                    ms.Close();
                    string pathTxt = System.Web.HttpContext.Current.Server.MapPath("~\\Files\\IsAvailable.txt");
                    using (StreamWriter writetext = new StreamWriter(pathTxt))
                    {
                        writetext.WriteLine("1");
                        writetext.Close();
                    }
                    return bytes;
                }
            }
            catch (Exception e)
            {
                string path = System.Web.HttpContext.Current.Server.MapPath("~\\Files\\IsAvailable.txt");
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    writetext.WriteLine("1");
                    writetext.Close();
                }
                return null;
            }

        }

        private void addTestScenariosExecuted(List<Requirement> requirementList, ExcelPackage package, Project project, ExcelCreator ex, int executionId)
        {

            var tsSheet = package.Workbook.Worksheets.Add("Test Scenarios");
            int rowValue = 2;
            tsSheet.Column(2).Width = 20;
            tsSheet.Column(5).Width = 15;
            tsSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            var first = 0;

            foreach (var req in requirementList)
            {
                List<TestScenario> testScenarios = _requirementService.GetAllTestScenario(req.Id);
                if (testScenarios.Count > 0)
                {
                    foreach (var tc in testScenarios)
                    {
                        TestResult result = _testResultService.getForTestScenario(tc.Test_Scenario_Id, executionId);
                        if (result != null)
                        {
                            if (result.Status != null)
                            {

                                if (!result.Status.Equals("TBE"))
                                {

                                    try
                                    {



                                        first = rowValue;
                                        LinkCellDTO cell = new LinkCellDTO();
                                        //Header Project Name
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = project.Name;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        rowValue++;
                                        //TC Title
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Scenario";
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                        rowValue++;
                                        //ID TITLE AND REQ NUMBER HEADERS
                                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Value = " Test Scenario ID:";
                                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Style.Font.Bold = true;
                                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Value = "Priority";
                                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Style.Font.Bold = true;
                                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Requirement ID";
                                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        rowValue++;
                                        //ID AND REQ NUMBER VALUES

                                        LinkCellDTO link = new LinkCellDTO();
                                        link.name = tc.ts_number;
                                        link.cell = "#'Test Scenarios'!$B$" + rowValue.ToString();
                                        link.coor = "B" + rowValue.ToString() + ":C" + rowValue.ToString();
                                        link.auxId = tc.Test_Scenario_Id;
                                        tsLinkList.Add(link);
                                        link.ReqNumber = req.req_number;
                                        link.reqName = req.Name;
                                        testExecutedList.Add(link);
                                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":C" + rowValue.ToString()].Value = tc.ts_number;
                                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["D" + rowValue.ToString() + ":E" + rowValue.ToString()].Value = tc.Test_Priority;
                                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = req.req_number;
                                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                        tsSheet.Cells["F" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                        LinkCellDTO tsToTrac = new LinkCellDTO();
                                        tsToTrac.name = req.req_number;
                                        tsToTrac.coor = "F" + rowValue.ToString() + ":G" + rowValue.ToString();
                                        testScenarioToTraceability.Add(tsToTrac);
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                        rowValue++;
                                        //TITLE
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Title: " + tc.Title;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                        rowValue++;
                                        //Type
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Type: " + tc.Type;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                        rowValue++;

                                        //DESCRIPTION
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Description: " + tc.Description;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        tsSheet.Row(rowValue).Height = 60;
                                        rowValue++;
                                        //PRECONDITION
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Preconditions: " + tc.Preconditions;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        tsSheet.Row(rowValue).Height = 60;
                                        rowValue++;
                                        //NOTES
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Notes: " + tc.Note;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        tsSheet.Row(rowValue).Height = 60;
                                        tsSheet.Row(rowValue).Style.WrapText = true;
                                        rowValue++;
                                        //STEPS HEADERS
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "#";
                                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Actions";
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                        tsSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        rowValue++;

                                    }
                                    catch (Exception)
                                    {

                                        throw;
                                    }
                                    int stepCounter = 1;
                                    List<Step> stepsList = ex._stepService.GetForTestScenarioOrder(tc.Test_Scenario_Id);
                                    //STEPS NUMBER AND ACTIONS
                                    foreach (var step in stepsList)
                                    {
                                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                        tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                        if (step.type == null || step.type.Equals("Step"))
                                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps;
                                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;

                                        string myaction = step.action;
                                        int subaction = myaction.IndexOf("STP_");
                                        if (subaction != -1)
                                        {
                                            string firstText = myaction.Substring(0, subaction); //Primera parte del string
                                            string stp = myaction.Substring(subaction, myaction.Length - subaction); // Numero del STP
                                            int indexStp = stp.IndexOf(" ");
                                            if (indexStp != -1)
                                            {
                                                string lastText = stp.Substring(indexStp, stp.Length - indexStp);
                                                stp = stp.Substring(0, indexStp);
                                                LinkCellDTO toStpLink = new LinkCellDTO();
                                                toStpLink.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                                toStpLink.name = stp;
                                                tsToStpLinkList.Add(toStpLink);
                                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;

                                            }
                                            else
                                            {
                                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                                                LinkCellDTO toStpLink = new LinkCellDTO();
                                                toStpLink.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                                toStpLink.name = stp;
                                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                                tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                                tsToStpLinkList.Add(toStpLink);
                                            }

                                        }
                                        else
                                        {
                                            tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                                        }
                                        if (step.type != null)
                                        {
                                            //int t = 0;

                                            if (step.type.Equals("Expected Result"))
                                            {
                                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps + "\n Expected Result";
                                                //tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                                tsSheet.Row(rowValue).Height = 55;
                                            }
                                            else
                                            {
                                                stepCounter++;
                                            }
                                        }
                                        else
                                        {
                                            stepCounter++;
                                        }


                                        rowValue++;
                                    }





                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Date";
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                    if (result != null)
                                    {

                                        if (result.Execution_Date != null)
                                        {
                                            DateTime time = DateTime.SpecifyKind(DateTime.Parse(result.Execution_Date.ToString()), DateTimeKind.Utc);
                                            DateTime time2 = time.ToLocalTime();
                                            tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = time2.ToString();
                                        }



                                    }
                                    rowValue++;

                                    if (result != null)
                                    {
                                        if (result.Status.Equals("Fail"))
                                        {
                                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Result";
                                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                            tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                            tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                            tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Execution_Result;
                                            rowValue++;
                                        }

                                    }


                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Result";
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                    if (result != null)
                                    {
                                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Status;
                                    }
                                    rowValue++;

                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Tester";
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tsSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                    if (result != null)
                                    {
                                        tsSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Tester;
                                    }

                                    ExcelRange range = tsSheet.Cells["B" + first.ToString() + ":G" + rowValue.ToString()];
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Style.WrapText = true;
                                    rowValue++;
                                    rowValue++;


                                }
                            }



                        }


                    }
                }

            }

        }

        private void addTestProceduresExecuted(List<Requirement> requirementList, ExcelPackage package, Project project, ExcelCreator ex, int executionId, ExcelWorksheet testCasesSheet)
        {
            var tpSheet = package.Workbook.Worksheets.Add("Test Procedures");
            int rowValue = 2;
            tpSheet.Column(2).Width = 20;
            tpSheet.Column(5).Width = 15;
            tpSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

            int first = 0;

            foreach (var req in requirementList)
            {
                List<TestProcedure> testProcedures = _requirementService.GetAllTestProcedure(req.Id);
                if (testProcedures.Count > 0)
                {

                    foreach (var tp in testProcedures)
                    {
                        TestResult result = null;
                        result = _testResultService.getForTestProcedure(tp.Test_Procedure_Id, executionId);
                        if (result != null)
                        {
                            if (result.Status != null)
                            {
                                if (!result.Status.Equals("TBE"))
                                {
                                    if (tp.Test_Case_Id != null)
                                    {
                                        LinkCellDTO dto = new LinkCellDTO();
                                        dto.auxId = tp.Test_Case_Id.GetValueOrDefault();
                                        dto.ReqNumber = req.req_number;
                                        testCasesId.Add(dto);

                                        var linkCell = (from t in testCaseLinkList
                                                        where t.auxId == tp.Test_Case_Id
                                                        select t.name).FirstOrDefault();
                                    }


                                    first = rowValue;
                                    LinkCellDTO cell = new LinkCellDTO();
                                    //Header Project Name
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = project.Name;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;
                                    //TP Title
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Procedure";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    rowValue++;
                                    //ID TITLE AND REQ NUMBER HEADERS
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = " Test Procedure ID:";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    if (tp.Test_Case_Id != null)
                                    {
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Case ID";
                                    }
                                    else
                                    {
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Requirement";
                                    }


                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;
                                    //ID AND REQ NUMBER VALUES
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = tp.tp_number;
                                    LinkCellDTO link = new LinkCellDTO();
                                    link.cell = "#'Test Procedures'!$B$" + rowValue.ToString();
                                    link.name = tp.tp_number;
                                    link.coor = "B" + rowValue.ToString() + ":D" + rowValue.ToString();
                                    link.auxId = tp.Test_Procedure_Id;
                                    testProceduresInfo.Add(link);
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    // AQUI!!!

                                    if (tp.Test_Case_Id != null)
                                    {
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                        LinkCellDTO test = new LinkCellDTO();
                                        test.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                        test.name = "";
                                        test.auxId = tp.Test_Case_Id.GetValueOrDefault();
                                        TpToTc.Add(test);

                                    }
                                    else
                                    {

                                        //ESTO LO TENGO QUE AGREGAR A UNA LISTA
                                        link.ReqNumber = req.req_number;
                                        link.reqName = req.Name;
                                        testExecutedList.Add(link);
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = req.req_number;
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;


                                        LinkCellDTO tpToTrac = new LinkCellDTO();
                                        tpToTrac.name = req.req_number;
                                        tpToTrac.coor = "F" + rowValue.ToString() + ":G" + rowValue.ToString();
                                        testProcedureToTraceability.Add(tpToTrac);

                                    }

                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;
                                    //PRIORITY AND PROCEDURE Headers
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Procedure Designed: ";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tp.Test_Procedure_Creator;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;

                                    //
                                    //Title
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Title: " + tp.Title;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    rowValue++;
                                    //Type
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Type: " + tp.Type;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    rowValue++;
                                    //Description
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Description: " + tp.Description;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Row(rowValue).Height = 65;
                                    rowValue++;
                                    //STEPS HEADERS
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "#";
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Actions";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;

                                    List<Step> stepsList = ex._stepService.GetForTestProcedureOrder(tp.Test_Procedure_Id);
                                    //STEPS NUMBER AND ACTIONS
                                    foreach (var step in stepsList)
                                    {
                                        tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                        tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps;
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;

                                        string myaction = step.action;
                                        int subaction = myaction.IndexOf("STP_");
                                        if (subaction != -1)
                                        {
                                            string firstText = myaction.Substring(0, subaction); //Primera parte del string
                                            string stp = myaction.Substring(subaction, myaction.Length - subaction); // Numero del STP
                                            int indexStp = stp.IndexOf(" ");
                                            if (indexStp != -1)
                                            {
                                                string lastText = stp.Substring(indexStp, stp.Length - indexStp);
                                                stp = stp.Substring(0, indexStp);
                                                LinkCellDTO toStpLink = new LinkCellDTO();
                                                toStpLink.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                                toStpLink.name = stp;
                                                tpToStpLinkList.Add(toStpLink);
                                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;

                                            }
                                            else
                                            {
                                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                                tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                                                LinkCellDTO toStpLink = new LinkCellDTO();
                                                toStpLink.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                                toStpLink.name = stp;
                                                tpToStpLinkList.Add(toStpLink);
                                            }

                                        }
                                        else
                                        {
                                            tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                                        }

                                        rowValue++;
                                    }

                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Expected Result";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tp.Expected_Result;
                                    tpSheet.Row(rowValue).Height = 45;
                                    rowValue++;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Date";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                    if (result != null)
                                    {
                                        DateTime time = DateTime.SpecifyKind(DateTime.Parse(result.Execution_Date.ToString()), DateTimeKind.Utc);
                                        DateTime time2 = time.ToLocalTime();
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = time2.ToString();
                                    }

                                    rowValue++;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Result";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                    if (result != null)
                                    {
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Execution_Result;
                                    }
                                    rowValue++;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Result";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                    if (result != null)
                                    {
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Status;
                                    }
                                    rowValue++;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Tester";
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tpSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                    if (result != null)
                                    {
                                        tpSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Tester;
                                    }

                                    ExcelRange range = tpSheet.Cells["B" + first.ToString() + ":G" + rowValue.ToString()];
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Style.WrapText = true;


                                    rowValue++;
                                    rowValue++;
                                }
                            }
                        }


                    }

                }

            }
            AddTestCasesWithOutResults(testCasesId, project, package, testCasesSheet);

        }

        public ExcelWorksheet AddTestCasesExecuted(List<Requirement> requirements, ExcelPackage package, Project project, ExcelCreator ex, int executionId)
        {

            var tcSheet = package.Workbook.Worksheets.Add("Test Cases");
            int rowValue = 2;

            tcSheet.Column(2).Width = 20;
            tcSheet.Column(5).Width = 15;
            tcSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            int first = 0;
            foreach (var req in requirements)
            {
                List<TestCase> testCases = _requirementService.GetAllTestCase(req.Id);

                if (testCases.Count > 0)
                {
                    foreach (var tc in testCases)
                    {

                        TestResult result = null;
                        result = _testResultService.getForTestCase(tc.Test_Case_Id, executionId);
                        if (result != null)
                        {
                            if (result.Status != null)
                            {
                                if (!result.Status.Equals("TBE"))
                                {
                                    first = rowValue;
                                    LinkCellDTO cell = new LinkCellDTO();
                                    //Header Project Name
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = project.Name;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;
                                    //TC Title
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Case";
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    rowValue++;
                                    //ID TITLE AND REQ NUMBER HEADERS
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = " Test Case ID:";
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Requirement";
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;
                                    //ID AND REQ NUMBER VALUES
                                    LinkCellDTO link = new LinkCellDTO();
                                    link.name = tc.tc_number;
                                    link.cell = "#'Test Cases'!$B$" + rowValue.ToString();
                                    link.coor = "B" + rowValue.ToString() + ":G" + rowValue.ToString();
                                    link.auxId = tc.Test_Case_Id;
                                    link.ReqNumber = req.req_number;
                                    link.reqName = req.Name;
                                    testExecutedList.Add(link);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = tc.tc_number;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = req.req_number;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                    LinkCellDTO tcToTrac = new LinkCellDTO();
                                    tcToTrac.name = req.req_number;
                                    tcToTrac.coor = "F" + rowValue.ToString() + ":G" + rowValue.ToString();
                                    testCaseToTraceability.Add(tcToTrac);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;
                                    //PRIORITY AND PROCEDURE Headers
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Priority: ";
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Procedure";
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;
                                    //Priorirty And TC
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = tc.Test_Priority;
                                    var tp = _testProcedureService.GetTestProcedure(tc.Test_Case_Id);
                                    if (tp != null)
                                    {
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tp.tp_number;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                                        LinkCellDTO linkDTO = new LinkCellDTO();
                                        linkDTO.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                                        linkDTO.name = tp.tp_number;
                                        linkDTO.auxId = tp.Test_Procedure_Id;
                                        TcToTp.Add(linkDTO);
                                    }
                                    else
                                    {
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "N/A";
                                        //Getting Test Result for Test CASE

                                        result = _testResultService.getForTestCase(tc.Test_Case_Id, executionId);
                                    }


                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    rowValue++;
                                    //TITLE
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Title: " + tc.Title;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    rowValue++;
                                    //Type
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Type: " + tc.Type;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    rowValue++;
                                    //DESCRIPTION
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Description: " + tc.Description;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    tcSheet.Row(rowValue).Height = 65;
                                    rowValue++;
                                    //PRECONDITION
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Preconditions: " + tc.Preconditions;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    tcSheet.Row(rowValue).Height = 65;
                                    rowValue++;
                                    //STEPS HEADERS
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "#";
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Actions";
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    rowValue++;

                                    List<Step> stepsList = ex._stepService.GetForTestCaseOrder(tc.Test_Case_Id);
                                    //STEPS NUMBER AND ACTIONS
                                    foreach (var step in stepsList)
                                    {
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                                        rowValue++;
                                    }

                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Expected Result";
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tc.Expected_Result;
                                    tcSheet.Row(rowValue).Height = 45;
                                    rowValue++;
                                    if (tp == null)
                                    {

                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Date";
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                        if (result != null)
                                        {
                                            DateTime time = DateTime.SpecifyKind(DateTime.Parse(result.Execution_Date.ToString()), DateTimeKind.Utc);
                                            DateTime time2 = time.ToLocalTime();
                                            tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = time2.ToString();
                                        }

                                        rowValue++;


                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Execution Result";
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                        if (result != null)
                                        {
                                            tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Execution_Result;
                                        }

                                        rowValue++;


                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Result";
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                        if (result != null)
                                        {
                                            tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Status;
                                        }

                                        rowValue++;



                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Tester";
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                        tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "";
                                        if (result != null)
                                        {
                                            tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = result.Tester;
                                        }

                                        rowValue++;


                                    }

                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Case Designed";
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                                    tcSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                                    tcSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tc.Test_Case_Creator;
                                    ExcelRange range = tcSheet.Cells["B" + first.ToString() + ":G" + rowValue.ToString()];
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Style.WrapText = true;
                                    rowValue++;
                                    rowValue++;
                                }
                            }

                        }

                    }
                }





            }
            tcIndexValue = rowValue;
            return tcSheet;
        }

        public void AddTestCasesWithOutResults(List<LinkCellDTO> testIds, Project project, ExcelPackage package, ExcelWorksheet testSheet)
        {
            int rowValue = tcIndexValue;
            int first = 0;
            foreach (var celllinkDTO in testIds)
            {
                int safeId = celllinkDTO.auxId;
                TestCase tc = _testCaseService.Get(safeId);
                Requirement req = _requirementService.GetByReqNumber(project.Id, celllinkDTO.ReqNumber);
                first = rowValue;
                LinkCellDTO cell = new LinkCellDTO();
                //Header Project Name
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = project.Name;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;
                //TC Title
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Case";
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                rowValue++;
                //ID TITLE AND REQ NUMBER HEADERS
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = " Test Case ID:";
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Requirement";
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;
                //ID AND REQ NUMBER VALUES
                LinkCellDTO link = new LinkCellDTO();
                link.name = tc.tc_number;
                link.cell = "#'Test Cases'!$B$" + rowValue.ToString();
                link.coor = "B" + rowValue.ToString() + ":G" + rowValue.ToString();
                link.auxId = tc.Test_Case_Id;
                testCaseInfo.Add(link);
                link.ReqNumber = req.req_number;
                link.reqName = req.Name;
                testExecutedList.Add(link);
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = tc.tc_number;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = req.req_number;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);

                LinkCellDTO tcToTrac = new LinkCellDTO();
                tcToTrac.name = req.req_number;
                tcToTrac.coor = "F" + rowValue.ToString() + ":G" + rowValue.ToString();
                testCaseToTraceability.Add(tcToTrac);
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;
                //PRIORITY AND PROCEDURE Headers
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Priority: ";
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Test Procedure";
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;
                //Priorirty And TC
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = tc.Test_Priority;
                var tp = _testProcedureService.GetTestProcedure(tc.Test_Case_Id);
                if (tp != null)
                {
                    testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tp.tp_number;
                    testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.UnderLine = true;
                    testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                    LinkCellDTO linkDTO = new LinkCellDTO();
                    linkDTO.coor = "E" + rowValue.ToString() + ":G" + rowValue.ToString();
                    linkDTO.name = tp.tp_number;
                    linkDTO.auxId = tp.Test_Procedure_Id;
                    TcToTp.Add(linkDTO);
                }
                else
                {
                    testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "N/A";

                }


                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                rowValue++;
                //TITLE
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Title: " + tc.Title;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                rowValue++;
                //Type
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Type: " + tc.Type;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                rowValue++;
                //DESCRIPTION
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Description: " + tc.Description;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                testSheet.Row(rowValue).Height = 65;
                rowValue++;
                //PRECONDITION
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Preconditions: " + tc.Preconditions;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                testSheet.Row(rowValue).Height = 65;
                rowValue++;
                //STEPS HEADERS
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Bold = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "#";
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = "Actions";
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                testSheet.Cells["B" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rowValue++;

                List<Step> stepsList = _stepService.GetForTestCaseOrder(tc.Test_Case_Id);
                //STEPS NUMBER AND ACTIONS
                foreach (var step in stepsList)
                {
                    testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                    testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = step.number_steps;
                    testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                    testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = step.action;
                    rowValue++;
                }

                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Expected Result";
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tc.Expected_Result;
                testSheet.Row(rowValue).Height = 45;
                rowValue++;


                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Bold = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Merge = true;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Value = "Test Case Designed";
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.PatternType = ExcelFillStyle.Solid;
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Fill.BackgroundColor.SetColor(backGround);
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.White);
                testSheet.Cells["B" + rowValue.ToString() + ":D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Merge = true;
                testSheet.Cells["E" + rowValue.ToString() + ":G" + rowValue.ToString()].Value = tc.Test_Case_Creator;
                ExcelRange range = testSheet.Cells["B" + first.ToString() + ":G" + rowValue.ToString()];
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.WrapText = true;
                rowValue++;
                rowValue++;
            }
        }

        public void AddExecutionLinks(ExcelPackage package)
        {
            var tracSheet = package.Workbook.Worksheets["Traceability"];
            var testCaseSheet = package.Workbook.Worksheets["Test Cases"];
            var testProcedureSheet = package.Workbook.Worksheets["Test Procedures"];
            var testScenariosSheet = package.Workbook.Worksheets["Test Scenarios"];
            var requirementsSheet = package.Workbook.Worksheets["Requirements"];
            foreach (var tpCell in TpToTc)
            {
                var linkCell = (from cell in testCaseInfo
                                where cell.auxId == tpCell.auxId
                                select cell).FirstOrDefault();
                if (linkCell != null)
                {
                    testProcedureSheet.Cells[tpCell.coor].Value = linkCell.name;
                    testProcedureSheet.Cells[tpCell.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }

            }


            foreach (var tcCell in TcToTp)
            {
                var linkCell = (from cell in testProceduresInfo
                                where cell.auxId == tcCell.auxId
                                select cell).FirstOrDefault();
                if (linkCell != null)
                {
                    testCaseSheet.Cells[tcCell.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);

                }


            }

            foreach (var tracToReq in reqLinkList)
            {
                var linkCell = (from s in reqSheetList
                                where s.name == tracToReq.name
                                select s).FirstOrDefault();

                if (linkCell != null)
                {
                    tracSheet.Cells[tracToReq.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }

            foreach (var tcintp in tpToStpLinkList)
            {
                var linkCell = (from s in stpLinkList
                                where s.name == tcintp.name
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    testProcedureSheet.Cells[tcintp.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }

            foreach (var tcintp in tsToStpLinkList)
            {
                var linkCell = (from s in stpLinkList
                                where s.name == tcintp.name
                                select new { s.cell }).FirstOrDefault();

                if (linkCell != null)
                {
                    testScenariosSheet.Cells[tcintp.coor].Hyperlink = new Uri(linkCell.cell + "", UriKind.Relative);
                }
            }

            foreach (var ts in testScenarioToTraceability)
            {
                var linkcell = (from f in reqLinkList
                                where f.name == ts.name
                                select f).FirstOrDefault();

                if (linkcell != null)
                {
                    testScenariosSheet.Cells[ts.coor].Hyperlink = new Uri(linkcell.cell + "", UriKind.Relative);
                }
            }


            foreach (var tp in testProcedureToTraceability)
            {
                var linkcell = (from f in reqLinkList
                                where f.name == tp.name
                                select f).FirstOrDefault();

                if (linkcell != null)
                {
                    testProcedureSheet.Cells[tp.coor].Hyperlink = new Uri(linkcell.cell + "", UriKind.Relative);
                }
            }

            foreach (var tc in testCaseToTraceability)
            {
                var linkcell = (from f in reqLinkList
                                where f.name == tc.name
                                select f).FirstOrDefault();

                if (linkcell != null)
                {
                    testCaseSheet.Cells[tc.coor].Hyperlink = new Uri(linkcell.cell + "", UriKind.Relative);
                }
            }
            package.Workbook.Worksheets.MoveToStart(5);
        }

        public void ExecutionTraceability(List<Requirement> requirements, ExcelPackage package, string name, ExcelCreator ex, DashboardDTO dashboard, ExecutionGroup group, int executionId)
        {


            var tracSheet = package.Workbook.Worksheets.Add("Traceability");
            int rowValue = 5;
            //wIDTHS BY COLUMNS
            tracSheet.Column(2).Width = 30;
            tracSheet.Column(3).Width = 60;
            tracSheet.Column(4).Width = 30;
            tracSheet.Column(6).Width = 40;
            tracSheet.Column(7).Width = 30;

            //HEADERS SETUP
            tracSheet.Cells["B3:D3"].Merge = true;
            tracSheet.Cells["B3:D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["B3:D3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["B3:D3"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["B3:D3"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["B3:D3"].Style.Font.Bold = true;
            tracSheet.Cells["B3:D3"].Value = name;
            //ID
            tracSheet.Cells["B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["B4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["B4"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["B4"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["B4"].Style.Font.Bold = true;
            tracSheet.Cells["B4"].Value = "Requirements ID";
            //Section
            tracSheet.Cells["C4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["C4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["C4"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["C4"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["C4"].Style.Font.Bold = true;
            tracSheet.Cells["C4"].Value = "Section";
            //Test Cases
            tracSheet.Cells["D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["D4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["D4"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["D4"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["D4"].Style.Font.Bold = true;
            tracSheet.Cells["D4"].Value = "Test Evidence";

            foreach (var req in testExecutedList)
            {
                bool flag = true;
                int cellsToMerge = 1;
                var listOfExecutions = (from subList in testExecutedList
                                        where subList.ReqNumber == req.ReqNumber
                                        select subList).ToList();

                if (listOfExecutions.Count != 0)
                {
                    cellsToMerge = (listOfExecutions.Count + rowValue) - 1;
                }
                else
                {

                    cellsToMerge = rowValue;
                }



                if (listOfExecutions.Count != 0)
                {
                    foreach (var testEvidence in listOfExecutions)
                    {


                        if (flag)
                        {

                            LinkCellDTO cell = new LinkCellDTO();
                            tracSheet.Cells["B" + rowValue.ToString()].Value = req.ReqNumber;
                            tracSheet.Cells["B" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);
                            tracSheet.Cells["B" + rowValue.ToString()].Style.Font.UnderLine = true;
                            tracSheet.Cells["C" + rowValue.ToString()].Value = req.reqName;

                            cell.cell = "#'Traceability'!$B$" + rowValue.ToString();
                            cell.name = req.ReqNumber;
                            cell.coor = "B" + rowValue.ToString();
                            reqLinkList.Add(cell);

                            tracSheet.Cells["B" + rowValue.ToString() + ":B" + cellsToMerge.ToString()].Merge = true;
                            tracSheet.Cells["C" + rowValue.ToString() + ":C" + cellsToMerge.ToString()].Merge = true;

                            tracSheet.Cells["B" + rowValue.ToString() + ":B" + cellsToMerge.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            tracSheet.Cells["C" + rowValue.ToString() + ":C" + cellsToMerge.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            tracSheet.Cells["B" + rowValue.ToString() + ":B" + cellsToMerge.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            tracSheet.Cells["C" + rowValue.ToString() + ":C" + cellsToMerge.ToString()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            flag = false;
                        }




                        tracSheet.Cells["D" + rowValue.ToString()].Value = testEvidence.name;
                        tracSheet.Cells["D" + rowValue.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        tracSheet.Cells["D" + rowValue.ToString()].Hyperlink = new Uri(testEvidence.cell + "", UriKind.Relative);
                        tracSheet.Cells["D" + rowValue.ToString()].Style.Font.UnderLine = true;
                        tracSheet.Cells["D" + rowValue.ToString()].Style.Font.Color.SetColor(Color.Blue);

                        rowValue++;

                    }
                }


                testExecutedList = (from newList in testExecutedList
                                    where newList.ReqNumber != req.ReqNumber
                                    select newList).ToList();
            }



            //SUMMARY
            tracSheet.Cells["F3:G4"].Merge = true;
            tracSheet.Cells["F3:G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["F3:G4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            tracSheet.Cells["F3:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            tracSheet.Cells["F3:G4"].Style.Fill.BackgroundColor.SetColor(backGround);
            tracSheet.Cells["F3:G4"].Style.Font.Color.SetColor(Color.White);
            tracSheet.Cells["F3:G4"].Style.Font.Bold = true;
            tracSheet.Cells["F3:G4"].Value = "SUMMARY";

            tracSheet.Cells["F5:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["G5:G10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tracSheet.Cells["F5"].Value = "Total Requirements";
            tracSheet.Cells["F6"].Value = "Total Test Cases";
            tracSheet.Cells["F7"].Value = "Total Test Procedures";
            tracSheet.Cells["F8"].Value = "Total Test Scenarios";
            tracSheet.Cells["F9"].Value = "Total Supplemental Test Procedures";
            tracSheet.Cells["F10"].Value = "Requirements Missing Test Evidence";

            tracSheet.Cells["G5"].Value = dashboard.totalRequirements;
            tracSheet.Cells["G6"].Value = dashboard.totalTestCases;
            tracSheet.Cells["G7"].Value = dashboard.totalTestProcedures;
            tracSheet.Cells["G8"].Value = dashboard.totalTestScenarios;
            tracSheet.Cells["G9"].Value = dashboard.totalstp;
            tracSheet.Cells["G10"].Value = dashboard.requirementsMissingTestEvidence;


            if (executionId != -1)
            {
                tracSheet.Cells["F12:G13"].Merge = true;
                tracSheet.Cells["F12:G13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["F12:G13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                tracSheet.Cells["F12:G13"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                tracSheet.Cells["F12:G13"].Style.Fill.BackgroundColor.SetColor(backGround);
                tracSheet.Cells["F12:G13"].Style.Font.Color.SetColor(Color.White);
                tracSheet.Cells["F12:G13"].Style.Font.Bold = true;
                tracSheet.Cells["F12:G13"].Value = "Test Execution Summary";

                tracSheet.Cells["F14:F22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["G14:G22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                tracSheet.Cells["F14"].Value = "Description";
                tracSheet.Cells["F15"].Value = "Total Evidence";
                tracSheet.Cells["F16"].Value = "Test Strategy";
                tracSheet.Cells["F17"].Value = "Total Test Passed";
                tracSheet.Cells["F18"].Value = "Total Test Failed";
                tracSheet.Cells["F19"].Value = "Total Test To Be Executed";
                tracSheet.Cells["F20"].Value = "Percentaje Pass";
                tracSheet.Cells["F21"].Value = "Percentaje Fail";
                tracSheet.Cells["F22"].Value = "Percentaje TBE";


                tracSheet.Cells["G14"].Value = "This report contain the test results of execution group '" + group.Name + "'" + "\n" + " which only has the test results of the test evidence executed on the group";

                tracSheet.Cells["G14"].Style.WrapText = true;


                List<TestResult> results = _testResultService.getForExecutionGroup(executionId);

                int TotalEvidence = results.Count();
                decimal TP = results.Where(r => r.Evidence.Equals("TP")).ToList().Count;
                decimal TS = results.Where(r => r.Evidence.Equals("TS")).ToList().Count;
                decimal TC = results.Where(r => r.Evidence.Equals("TC")).ToList().Count;

                decimal TotalPass = results.Where(r => r.Status.Equals("Pass")).ToList().Count;
                decimal TotalFail = results.Where(r => r.Status.Equals("Fail")).ToList().Count;
                decimal TotalTBE = results.Where(r => r.Status.Equals("TBE")).ToList().Count;


                decimal PercentagePass = (TotalPass / results.Count) * 100m;

                decimal PercentageFail = (TotalFail / results.Count) * 100m;

                decimal PercentageTBE = (TotalTBE / results.Count) * 100m;

                string Strategy = null;

                if (TP >= 1 && TC == 0 && TS == 0)
                {
                    Strategy = "Only TP";
                }

                if (TC >= 1 && TP == 0 && TS == 0)
                {
                    Strategy = "Only TC";
                }

                if (TS >= 1 && TP == 0 && TC == 0)
                {
                    Strategy = "Only TS";
                }

                if (TS >= 1 && TP >= 1 || TC >= 1)
                {
                    Strategy = "Hybrid";
                }



                tracSheet.Cells["G15"].Value = TotalEvidence;
                tracSheet.Cells["G16"].Value = Strategy;
                tracSheet.Cells["G17"].Value = TotalPass;
                tracSheet.Cells["G18"].Value = TotalFail;
                tracSheet.Cells["G19"].Value = TotalTBE;
                tracSheet.Cells["G20"].Value = Math.Round(PercentagePass, 3).ToString() +"%";
                tracSheet.Cells["G21"].Value = Math.Round(PercentageFail, 3).ToString()+"%";
                tracSheet.Cells["G22"].Value = Math.Round(PercentageTBE, 3).ToString() +"%";


                try
                {
                    //Test Environment 
                    TestEnvironment testEnvironment = _testEnviromentService.Get(group.TestEnvironmentId.Value);
                    tracSheet.Cells["F24:G25"].Merge = true;
                    tracSheet.Cells["F24:G25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tracSheet.Cells["F24:G25"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    tracSheet.Cells["F24:G25"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    tracSheet.Cells["F24:G25"].Style.Fill.BackgroundColor.SetColor(backGround);
                    tracSheet.Cells["F24:G25"].Style.Font.Color.SetColor(Color.White);
                    tracSheet.Cells["F24:G25"].Style.Font.Bold = true;
                    tracSheet.Cells["F24:G25"].Value = "Test Enviroment";

                    tracSheet.Cells["F26"].Value = "Name";
                    tracSheet.Cells["F27"].Value = "Server";
                    tracSheet.Cells["F28"].Value = "Processor";
                    tracSheet.Cells["F29"].Value = "RAM";
                    tracSheet.Cells["F30"].Value = "HardDisk";
                    tracSheet.Cells["F31"].Value = "OS";
                    tracSheet.Cells["F32"].Value = "Dev Server";
                    tracSheet.Cells["F33"].Value = "Test Server";
                    tracSheet.Cells["F34"].Value = "Database";
                    tracSheet.Cells["F35"].Value = "URL";
                    tracSheet.Cells["F36"].Value = "Site Type";
                    tracSheet.Cells["F37"].Value = "Creator";
                    tracSheet.Cells["F38"].Value = "Last Editor";
                    tracSheet.Cells["F39"].Value = "Notes";

                    tracSheet.Cells["G26"].Value = testEnvironment.Name;
                    tracSheet.Cells["G27"].Value = testEnvironment.Server;
                    tracSheet.Cells["G28"].Value = testEnvironment.Processor;
                    tracSheet.Cells["G29"].Value = testEnvironment.RAM;
                    tracSheet.Cells["G30"].Value = testEnvironment.HardDisk;
                    tracSheet.Cells["G31"].Value = testEnvironment.OS;
                    tracSheet.Cells["G32"].Value = testEnvironment.ServerSoftwareDevs;
                    tracSheet.Cells["G33"].Value = testEnvironment.ServerSoftwareTest;
                    tracSheet.Cells["G34"].Value = testEnvironment.Database;
                    tracSheet.Cells["G35"].Value = testEnvironment.URL;
                    tracSheet.Cells["G36"].Value = testEnvironment.SiteType;
                    tracSheet.Cells["G37"].Value = testEnvironment.Creator;
                    tracSheet.Cells["G38"].Value = testEnvironment.Last_Editor;
                    tracSheet.Cells["G39"].Value = testEnvironment.Notes;

                    tracSheet.Cells["F26:F39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tracSheet.Cells["G26:G39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }


            }




            if (executionId == -1)
            {

                if (group != null)
                {
                    //TestEnviroment
                    TestEnvironment testEnvironment = _testEnviromentService.Get(group.TestEnvironmentId.Value);
                    tracSheet.Cells["F13:G14"].Merge = true;
                    tracSheet.Cells["F13:G14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tracSheet.Cells["F13:G14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    tracSheet.Cells["F13:G14"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    tracSheet.Cells["F13:G14"].Style.Fill.BackgroundColor.SetColor(backGround);
                    tracSheet.Cells["F13:G14"].Style.Font.Color.SetColor(Color.White);
                    tracSheet.Cells["F13:G14"].Style.Font.Bold = true;
                    tracSheet.Cells["F13:G14"].Value = "Test Enviroment";

                    tracSheet.Cells["F15"].Value = "Name";
                    tracSheet.Cells["F16"].Value = "Server";
                    tracSheet.Cells["F17"].Value = "Processor";
                    tracSheet.Cells["F18"].Value = "RAM";
                    tracSheet.Cells["F19"].Value = "HardDisk";
                    tracSheet.Cells["F20"].Value = "OS";
                    tracSheet.Cells["F21"].Value = "Dev Server";
                    tracSheet.Cells["F22"].Value = "Test Server";
                    tracSheet.Cells["F23"].Value = "Database";
                    tracSheet.Cells["F24"].Value = "URL";
                    tracSheet.Cells["F25"].Value = "Site Type";
                    tracSheet.Cells["F26"].Value = "Creator";
                    tracSheet.Cells["F27"].Value = "Last Editor";
                    tracSheet.Cells["F28"].Value = "Notes";

                    tracSheet.Cells["G15"].Value = testEnvironment.Name;
                    tracSheet.Cells["G16"].Value = testEnvironment.Server;
                    tracSheet.Cells["G17"].Value = testEnvironment.Processor;
                    tracSheet.Cells["G18"].Value = testEnvironment.RAM;
                    tracSheet.Cells["G19"].Value = testEnvironment.HardDisk;
                    tracSheet.Cells["G20"].Value = testEnvironment.OS;
                    tracSheet.Cells["G21"].Value = testEnvironment.ServerSoftwareDevs;
                    tracSheet.Cells["G22"].Value = testEnvironment.ServerSoftwareTest;
                    tracSheet.Cells["G23"].Value = testEnvironment.Database;
                    tracSheet.Cells["G24"].Value = testEnvironment.URL;
                    tracSheet.Cells["G25"].Value = testEnvironment.SiteType;
                    tracSheet.Cells["G26"].Value = testEnvironment.Creator;
                    tracSheet.Cells["G27"].Value = testEnvironment.Last_Editor;
                    tracSheet.Cells["G28"].Value = testEnvironment.Notes;

                    tracSheet.Cells["F15:F28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    tracSheet.Cells["G15:G28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }



        }

    }

}

