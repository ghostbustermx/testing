(function () {
    'use strict';

    var app = angular.module(appName);

    app.controller('ProjectExecutionController', ['$scope', '$state', '$window', 'RequirementsTestService', 'AccessService', 'ngToast', '$http', 'ProjectService', 'TestExecutionService', 'TestResultService', 'DateParse', function ($scope, $state, $window, RequirementsTestService, AccessService, ngToast, $http, ProjectService, TestExecutionService, TestResultService, DateParse) {
        var projectId = $state.params.projectId;
        var executionId = $state.params.executionId;
        $scope.projectId = $state.params.projectId;
        $scope.Options2 = [];
        $scope.Options3 = ["All", "Pass", "Fail", "To Be Executed"];
        $scope.FilterSelected = "All"
        $scope.ExecutionSelected = {};
        $scope.GlobalArray = [];
        GetProject();
        GetPaginationElements();
        GetTestResults(executionId);
        GetTestExecutions();
        $scope.isFiltered = false;
        $scope.pageSize = 10;
        $scope.currentPage = 1;
        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelected = $scope.Options[0];
            }, function (error) {
            });
        }


        $scope.changeValue = function (value) {
            $scope.pageSize = value;
        }


        $scope.ChangeExecution = function (value) {
            $scope.SelectedExecution = value;
            GetTestResults($scope.SelectedExecution.Test_Execution_Id);
        }

        function GetTestExecutions() {
            $scope.Options2 = [];
            TestExecutionService.GetByProject(projectId, groups => {
                $scope.Options2 = groups;
                $scope.ExecutionSelected = $scope.Options2[0];

            }, error => {
            });

        }

        function GetTestResults(executionId) {
            TestResultService.GetForGroup(executionId, function (ExecutedList) {
                $scope.ResultsList = ExecutedList;
                $scope.ResultsList.forEach(test => {
                    test.Execution_Date = DateParse.GetDate(test.Execution_Date);
                    test.viewExpected = true;
                });

                $scope.GlobalArray = $scope.ResultsList;
                $scope.Dashboard = $scope.ResultsList;

                $scope.GetTotal();
                $scope.GetPassed();
                $scope.GetFailed();
                $scope.GetTBE();
                $scope.GetStrategy();
                $scope.GetPorcentage();

                $scope.applyFilter();
            }, function (err) {
            });
        };

        function GetProject() {
            ProjectService.Get(projectId, function (data) {
                $scope.Name = data.Name;
            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };


        $scope.GoToEvidenceDetails = function (test) {
            if (test.Evidence == 'TP') {
                RequirementsTestService.GetTestProcedureRelations(test.Test_Procedure_Id, function (data) {
                    $state.go('testprocedureDetails', { projectId: projectId, reqId: data[0].Requirement_Id, id: test.Test_Procedure_Id });
                }, function (err) {
                });

            }
            if (test.Evidence == 'TC') {
                RequirementsTestService.GetTestCaseRelations(test.Test_Case_Id, function (data) {
                    $state.go('testcaseDetails', { projectId: projectId, reqId: data[0].Requirement_Id, id: test.Test_Case_Id });
                }, function (err) {
                });

            }
            if (test.Evidence == 'TS') {
                RequirementsTestService.GetTestScenarioRelations(test.Test_Scenario_Id, function (data) {
                    $state.go('testscenarioDetails', { projectId: projectId, reqId: data[0].Requirement_Id, id: test.Test_Scenario_Id });
                }, function (err) {
                });
            }
        };




        $scope.ViewExpected = function (result, status) {
            result.viewExpected = status;
        }




        $scope.applyFilter = function () {
            var aux;
            if ($scope.FilterSelected == 'Fail') {
                if ($scope.isFiltered) {
                    $scope.ResultsList = $scope.GlobalArray
                }
                aux = $scope.ResultsList.filter(test => test.Status == 'Fail')
                $scope.ResultsList = aux;

                $scope.isFiltered = true;
            } else
                if ($scope.FilterSelected == 'Pass') {
                    if ($scope.isFiltered) {
                        $scope.ResultsList = $scope.GlobalArray
                    }
                    aux = $scope.ResultsList.filter(test => test.Status == 'Pass')
                    $scope.ResultsList = aux;
                    $scope.isFiltered = true;
                } else
                    if ($scope.FilterSelected == 'To Be Executed') {
                        if ($scope.isFiltered) {
                            $scope.ResultsList = $scope.GlobalArray
                        }
                        aux = $scope.ResultsList.filter(test => test.Status == 'TBE')
                        $scope.ResultsList = aux;
                        $scope.isFiltered = true;
                    } else {
                        $scope.ResultsList = $scope.GlobalArray
                        $scope.isFiltered = false;

                    }
        }

        $scope.GetTotal = function () {
            $scope.Total = $scope.Dashboard.length;
        };

        $scope.GetPassed = function () {
            $scope.Passed = $scope.Dashboard.filter(test => test.Status == 'Pass').length;
        };

        $scope.GetFailed = function () {
            $scope.Failed = $scope.Dashboard.filter(test => test.Status == 'Fail').length;
        };

        $scope.GetTBE = function () {
            $scope.TBE = $scope.Dashboard.filter(test => test.Status == 'TBE').length;
        };

        $scope.GetPorcentage = function () {
            var total = $scope.Dashboard.length;
            $scope.PercentageTBE = Calculate(total, $scope.TBE);
            $scope.PercentagePassed = Calculate(total, $scope.Passed);
            $scope.PercentageFailed = Calculate(total, $scope.Failed);
        };



        function Calculate(total, value) {
            if (value == 0) {
                return value;
            }
            var perc = "";
            if (isNaN(value) || isNaN(total)) {
                perc = " ";
            } else {
                perc = ((value / total) * 100).toFixed(3);
            }
            return perc;
        }


        $scope.GetStrategy = function () {
            var TP = $scope.Dashboard.filter(test => test.Evidence == 'TP').length;
            var TC = $scope.Dashboard.filter(test => test.Evidence == 'TC').length;
            var TS = $scope.Dashboard.filter(test => test.Evidence == 'TS').length;

            if (TP >= 1 && TC == 0 && TS == 0) {
                $scope.Strategy = "Only TP";
            }

            if (TC >= 1 && TP == 0 && TS == 0) {
                $scope.Strategy = "Only TC"
            }

            if (TS >= 1 && TP == 0 && TC == 0) {
                $scope.Strategy = "Only TS"
            }

            if (TS >= 1 && TP >= 1 || TC >= 1) {
                $scope.Strategy = "Hybrid";
            }
        };
    }])
})();