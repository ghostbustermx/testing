

(function () {
    'use strict';

    var app = angular.module(appName);

    app.controller('ExecutionViewController', ['$scope', '$state', '$window', 'RequirementService', 'ExecutionTestService', 'TestCaseService', 'TestScenarioService', 'TestProcedureService', 'StepService', 'TestResultService', 'AccessService', 'ngToast', '$timeout', 'TestExecutionService', 'ProjectService', 'UserService', function ($scope, $state, $window, RequirementService, ExecutionTestService, TestCaseService, TestScenarioService, TestProcedureService, StepService, TestResultService, AccessService, ngToast, $timeout, TestExecutionService, ProjectService, UserService) {

        var projectId = $state.params.projectId;
        var executionId = $state.params.executionId;
        var testExecutionId = $state.params.testExecutionId;
        var modalExecuted = false;
        $scope.testSelected = {};
        $scope.Failed = 0;
        $scope.Passed = 0;
        $scope.TBD = 100;
        $scope.testToDisplay = {};
        var testExecution;
        HasAccess();
        var timer;
        $scope.ActiveTest = false;
        $scope.reqs2 = [];
        $scope.isFiltered = false;
        $scope.Options = ['All', 'Passed', 'Failed', 'To be Executed'];
        $scope.RowsSelected = 'All';
        $scope.GlobalArray = [];
        var count = 1;
        //Start

        var loadTime = 10000, //Load the data every second
            errorCount = 0, //Counter for the server errors
            loadPromise; //Pointer to the promise created by the Angular $timout service

        $scope.GetTotal = function () {
            $scope.Total = $scope.GlobalArray.length;
        };

        $scope.GetStrategy = function () {
            $scope.TP_Count = $scope.GlobalArray.filter(test => test.Evidence == 'TP').length;
            $scope.TC_Count = $scope.GlobalArray.filter(test => test.Evidence == 'TC').length;
            $scope.TS_Count = $scope.GlobalArray.filter(test => test.Evidence == 'TS').length;

            if ($scope.TP_Count >= 1 && $scope.TC_Count == 0 && $scope.TS_Count == 0) {
                $scope.Strategy = "Only TP";
            }

            if ($scope.TC_Count >= 1 && $scope.TP_Count == 0 && $scope.TS_Count == 0) {
                $scope.Strategy = "Only TC"
            }

            if ($scope.TS_Count >= 1 && $scope.TP_Count == 0 && $scope.TC_Count == 0) {
                $scope.Strategy = "Only TS"
            }

            if ($scope.TS_Count >= 1 && $scope.TP_Count >= 1 || $scope.TC_Count >= 1) {
                $scope.Strategy = "Hybrid";
            }
        };


        $scope.isValid = false;
        function HasAccess() {
            AccessService.HasAccess('Project', projectId, 0, 0, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    AccessService.HasAccess('PlayExecution', projectId, executionId, testExecutionId, 0, 0, 0, function (data) {
                        if (!data.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            $scope.isValid = true;
                            GetProject();
                            GetData();
                            GetCurrentUser();
                            GetTestExecution();
                        }
                    }, function (error) {
                    });
                }
            }, function (error) {

            });
        };


        function GetData() {
            GetTestStatus();
            errorCount = 0;
            nextLoad();
        }

        function GetCurrentUser() {
            UserService.GetCurrentUser(function (data) {
                $scope.user = data;
                if ($scope.user.Role == 'VBP' || $scope.user.Role == 'BA') {
                    $scope.showEdit = false;
                } else {
                    $scope.showEdit = true;
                }
            }, function (error) {
            });
        }


        function GetProgressBarInfo() {
            var size = $scope.GlobalArray.length;
            var passed = 0;
            var failed = 0;

            passed = $scope.GlobalArray.filter(test => test.Status == 'Pass').length
            failed = $scope.GlobalArray.filter(test => test.Status == 'Fail').length

            var executed = size - (passed + failed)
            var percentageExecuted = (executed / size * 100);
            var percentagePassR = (passed / size * 100);
            var percentageFailR = (failed / size * 100);

            var percentagePass = Math.round(passed / size * 100);
            var percentageFail = Math.round(failed / size * 100);
            var tbd = 100 - (percentagePass + percentageFail);

            if (percentageExecuted < 1 && percentageExecuted > 0 && tbd < 1) {
                if (percentagePass >= 50) {
                    percentagePass--;
                } else {
                    percentageFail--;
                }
                tbd = 1;
            }

            if (percentageFailR < 1 && percentageFailR > 0 && percentageFail < 1) {
                if (percentagePass >= 50) {
                    percentagePass--;
                } else {
                    tbd--;
                }
                percentageFail = 1;
            }

            if (percentagePassR < 1 && percentagePassR > 0 && percentagePass < 1) {
                if (percentageFail >= 50) {
                    percentageFail--;
                } else {
                    tbd--;
                }
                percentagePass = 1;
            }

            $scope.Passed = percentagePass;
            $scope.Failed = percentageFail;
            $scope.TBD = tbd;
        }


        var cancelNextLoad = function () {
        $timeout.cancel(loadPromise);
    };


    var nextLoad = function (mill) {
        mill = mill || loadTime;

        //Always make sure the last timeout is cleared before starting a new one
        cancelNextLoad();
        loadPromise = $timeout(GetData, mill);
    };




    var myTime = $timeout(function () {
        $scope.BlockButtons = false;
        $scope.time = 0;
    }, 1000);

    $scope.time = 0;

    //timer callback
    var timer = function () {
        if ($scope.time < 5000) {
            $scope.time += 1000;
            $timeout(timer, 1000);
        }
    }

    //Always clear the timeout when the view is destroyed, otherwise it will keep polling
    $scope.$on('$destroy', function () {
        cancelNextLoad();
    });

    $scope.projectDetails = function () {
        $state.go('projectDetails', { id: projectId });
    };

    $scope.ChangeFilter = function (value) {
        TestResultService.GetForGroup(testExecutionId, resultsArray => {
            var aux;
            $scope.GlobalArray = resultsArray;
            $scope.testProceduresArray = resultsArray;
            if (value == 'All') {

                $scope.isFiltered = false;
            }
            if (value == 'Failed') {
                aux = $scope.testProceduresArray.filter(test => test.Status == 'Fail')
                $scope.testProceduresArray = aux;
                $scope.isFiltered = true;
            } else
                if (value == 'Passed') {
                    aux = $scope.testProceduresArray.filter(test => test.Status == 'Pass')
                    $scope.testProceduresArray = aux;
                    $scope.isFiltered = true;
                } else
                    if (value == 'To be Executed') {
                        aux = $scope.testProceduresArray.filter(test => test.Status == 'TBE')
                        $scope.testProceduresArray = aux;
                        $scope.isFiltered = true;
                    }

            LoadTestResults();
            if ($scope.testSelected.Test_Result_Id != null) {

                var index = $scope.testProceduresArray.findIndex(test => test.Test_Result_Id == $scope.testSelected.Test_Result_Id);
                if (index != -1) {
                    $scope.testProceduresArray[index].HasViewer = true;
                }

            }
        }, error => {

        });
    }

    $scope.SelectTest = function (test) {
        if ($scope.testSelected.Test_Result_Id != test.Test_Result_Id) {
            $scope.DeselectTest($scope.testSelected.Test_Result_Id);
            test.HasViewer = true;
            $scope.SetTest(test);
        }
    }

    function GetProject() {
        ProjectService.Get(projectId, r => {
            $scope.Name = r.Name;
        }, error => {

        });
    }

    $scope.ExecutionList = function () {
        $state.go('executionGroups', { projectId: projectId });
    };

    $scope.ExecutionsList = function () {
        $state.go('executionGroupDetails', { projectId: projectId, executionId: executionId });
    };



    function GetTestExecution() {

        TestExecutionService.Get(testExecutionId, data => {
            testExecution = data;

            if (testExecution.State == 'Closed') {
                ngToast.info({
                    content: "This execution has been closed"
                });
                $scope.isCancelled = true;
            } else {
                $scope.isCancelled = false;
            }

            if (testExecution.State == 'Finished') {
                ngToast.info({
                    content: "This execution has been Finished"
                });
                $scope.isOpen = false;
            } else {
                $scope.isOpen = true;
            }

        }, function (error) {

        });

    }

    $scope.applyFilter = function () {
        var aux;
        if ($scope.RowsSelected == 'Failed') {
            aux = $scope.testProceduresArray.filter(test => test.Status == 'Fail')
            $scope.testProceduresArray = aux;
            $scope.isFiltered = true;
        } else
            if ($scope.RowsSelected == 'Passed') {
                aux = $scope.testProceduresArray.filter(test => test.Status == 'Pass')
                $scope.testProceduresArray = aux;
                $scope.isFiltered = true;
            } else
                if ($scope.RowsSelected == 'To be Executed') {
                    aux = $scope.testProceduresArray.filter(test => test.Status == 'TBE')
                    $scope.testProceduresArray = aux;
                    $scope.isFiltered = true;
                } else {
                    $scope.isFiltered = false;
                }
    }

    function GetTestStatus() {

        if (!modalExecuted && $scope.isValid) {
            $("#loadingModalView").modal({
                backdrop: "static",
                keyboard: false,
                show: true
            });
            modalExecuted = true;
        }

        TestResultService.GetForGroup(testExecutionId, resultsArray => {

            $("#loadingModalView").modal("hide");

            $scope.GlobalArray = resultsArray;
            $scope.GetTotal();
            $scope.GetStrategy();
            $scope.testProceduresArray = resultsArray;
            $scope.applyFilter();

            LoadTestResults();
            if ($scope.testSelected.Test_Result_Id != null) {

                var index = $scope.testProceduresArray.findIndex(test => test.Test_Result_Id == $scope.testSelected.Test_Result_Id);
                if (index != -1) {
                    $scope.testProceduresArray[index].HasViewer = true;
                }

            }
            GetProgressBarInfo();
            selectNext();
         

        }, err => {
        });

    }

    function LoadTestResults() {
        $scope.testProceduresArray.forEach(function (test) {

            if (test.Evidence == 'TP') {
                test.Element_id = test.Test_Procedure_Id;
            } else
                if (test.Evidence == 'TC') {
                    test.Element_id = test.Test_Case_Id;
                } else
                    if (test.Evidence == 'TS') {
                        test.Element_id = test.Test_Scenario_Id;
                    }

        });




    }

    $scope.DeselectTest = function (id) {
        let auxTest = {};
        $scope.testProceduresArray.forEach(function (testAux) {
            if (testAux.Test_Result_Id == id) {
                testAux.isSelected = false;
                testAux.HasViewer = false;
                auxTest = testAux;
                return;
            }
        });

        return auxTest;
    }


    $scope.RemoveSelection = function (id) {
        let auxTest = {};

        $scope.testProceduresArray.forEach(function (testAux) {
            if (testAux.isSelected && testAux.Element_id == id) {
                testAux.isSelected = false;
                auxTest = testAux;
                return;
            }
        });

        return auxTest;
    }


    $scope.GetTestInformation = function (test) {
        GetTestInformation(test);
    }

    function GetTestInformation(test) {
        $scope.reqs2 = [];
        if (test.Evidence == 'TC') {
            TestCaseService.Get(test.Element_id, result => {
                $scope.testToDisplay = result;
                LoadSteps(test.Element_id);
                RequirementService.GetRequirementsByTestEvidence(test.Element_id, 'TC', data1 => {
                    $scope.reqs2 = data1;
                }, err => {
                });

            }, err => {
            });
        }

        if (test.Evidence == 'TP') {
            TestProcedureService.Get(test.Element_id, result => {
                $scope.testToDisplay = result;
                LoadSteps(test.Element_id);
                RequirementService.GetRequirementsByTestEvidence(test.Element_id, "TP", data2 => {
                    $scope.reqs2 = data2;
                }, err => {
                });
            }, err => {
            });
        }

        if (test.Evidence == 'TS') {
            TestScenarioService.Get(test.Element_id, result => {
                $scope.testToDisplay = result;
                LoadSteps(test.Element_id);
                RequirementService.GetRequirementsByTestEvidence(test.Element_id, "TS", data3 => {
                    $scope.reqs2 = data3;
                }, err => {
                });
            }, err => {
            });
        }
    }

    $scope.showFields = function () {
        $scope.showSummary = true;
    }

    $scope.cancelSummary = function () {
        $scope.showSummary = false;
    }

    function selectNext() {
        if (count == 1) {
            if (typeof ($scope.testProceduresArray) != 'undefined' && typeof ($scope.user) != 'undefined') {

                if ($scope.testProceduresArray.length != 0) {
                    if ($scope.testProceduresArray.length != 0) {
                        count++;
                        $scope.testProceduresArray[0].HasViewer = true;
                        $scope.testProceduresArray[0].CurrentViewer = $scope.user.UserName;
                        $scope.SetTest($scope.testProceduresArray[0]);
                    }

                }
            }
        }

    }


    $scope.SetTest = function (test) {
        $scope.testSelected = test;
        GetTestInformation(test);
    }

    $scope.goToDetails = function (executionId) {
        $state.go('executionGroupDetails', { projectId: projectId, executionId, executionId });
    }

    $scope.LoadSteps = function (id) {
        LoadSteps(id);
    }

    function LoadSteps(id) {
        if ($scope.testSelected.Evidence == 'TC') {
            StepService.GetForTestCase(id, function (data) {
                $scope.steps = data;
            }, function (error) {
            });

        }
        if ($scope.testSelected.Evidence == 'TP') {
            StepService.GetForTestProcedureSTP(projectId, id, function (data) {
                $scope.steps = data;

            }, function (error) {
            });
        }
        if ($scope.testSelected.Evidence == 'TS') {
            StepService.GetForTestScenarioSTP(projectId, id, data => {
                $scope.steps = data;
            }, function (error) {
            });

        }
    }



    $scope.ClearSelection = function () {

        $scope.DeselectTest($scope.testSelected.Test_Result_Id);

    };

    $scope.goRequirement = function (id) {
        var url = $state.href('requirementEdit', { projectId: projectId, id: id });
        var requirementWindow = window.open(url, '_blank', { focus: true });
    }

    $scope.goTestEvidence = function (test) {
        var state;
        var reqId = $scope.reqs2[0].Id;
        var testEvidence = test.Evidence;
        if (testEvidence == 'TC') {
            state = 'testcaseEdit';
        } else if (testEvidence == 'TP') {
            state = 'testprocedureEdit';
        } else if (testEvidence == 'TS') {
            state = 'testscenarioEdit';
        }
        var url = $state.href(state, { projectId: projectId, reqId: reqId, id: test.Element_id });
        var requirementWindow = window.open(url, '_blank', { focus: true });
    }

}]);

}) ();