(function () {
    'use strict';

    var app = angular.module(appName);

    app.controller('ExecutionGroupsPlayController', ['$scope', '$state', '$window', 'RequirementService', 'TestCaseService', 'TestScenarioService', 'TestProcedureService', 'StepService', 'TestResultService', 'AccessService', 'ngToast', '$timeout', 'TestExecutionService', 'ProjectService', 'UserService', 'AttachmentService', '$http', function ($scope, $state, $window, RequirementService, TestCaseService, TestScenarioService, TestProcedureService, StepService, TestResultService, AccessService, ngToast, $timeout, TestExecutionService, ProjectService, UserService, AttachmentService, $http) {

        var projectId = $state.params.projectId;
        var executionId = $state.params.executionId;
        var testExecutionId = $state.params.testExecutionId;
        var modalExecuted = false;
        HasAccess();
        $scope.testSelected = {};
        $scope.Failed = 0;
        $scope.Passed = 0;
        $scope.TBD = 100;
        $scope.testToDisplay = {};
        $scope.showSummary = false;
        $scope.isOpen = false;
        $scope.wait = false;
        $scope.IsTaken = false;
        $scope.isCancelled = false;
        var testExecution;

        var timer;
        var showMessages = true;
        var showExecutionMessage = true;
        $scope.ActiveTest = false;
        $scope.reqs2 = [];
        $scope.isFiltered = false;
        $scope.Options = ['All', 'Passed', 'Failed', 'To be Executed'];
        $scope.Options2 = ['All', 'To be Executed'];
        $scope.RowsSelected = 'All';
        $scope.optionSelected = 'All';
        $scope.GlobalArray = [];
        $scope.employees = [];
        $scope.userAssigned = {};
        $scope.UsersSelected = [];
        $scope.AutomaticSelection = true;
        $scope.filter2 = '';
        $scope.filterapplied = false;
        $scope.AutomaticSelectionStoredValue = $scope.AutomaticSelection;
        GetUsers(projectId);
        var count = 1;
        $scope.FilestoDisplay = [];
        //Start

        var loadTime = 10000, //Load the data every second
            errorCount = 0, //Counter for the server errors
            loadPromise; //Pointer to the promise created by the Angular $timout service


        function getData() {
            GetTestStatus();
            errorCount = 0;
            nextLoad();
        }

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
                            GetCurrentUser();
                        }
                    }, function (error) {
                    });
                }
            }, function (error) {

            });
        };

        function GetRole() {
            $scope.CurrentUserRole = $scope.user.Role;
            if ($scope.CurrentUserRole == 'VBP' || $scope.CurrentUserRole == 'BA') {
                $state.go('Projects').then(function () {
                    ngToast.danger({
                        content: 'You do not have access to the requested page.'
                    });
                });
            } else {
                getData();
                GetTestExecution();
                GetProject();
                selectNext();

            }
        }

        function GetUsers(id) {
            UserService.GetUsersByProject(id, function (data) {
                $scope.employees = data;
            });
        }


        function GetCurrentUser() {
            UserService.GetCurrentUser(function (data) {
                $scope.user = data;
                GetRole();
            }, function (error) {
            });

        }

        $scope.ToggleSelection = function () {
            $scope.AutomaticSelection = !$scope.AutomaticSelection;

        }

        $scope.CheckFilter = function () {
            if ($scope.filter2 != '') {
                $scope.filterapplied = true;
                $scope.AutomaticSelectionStoredValue = $scope.AutomaticSelection;
                $scope.AutomaticSelection = false;
            } else {
                $scope.filterapplied = false;
                $scope.AutomaticSelection = $scope.AutomaticSelectionStoredValue;
            }
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


            if ($scope.TBD == 0 && showExecutionMessage && testExecution.State == 'In Progress') {
                showExecutionMessage = false;
                ngToast.info({
                    content: "All the test in this group have been executed, you can rerun a test by clicking on it"
                });
            }

        }

        //Start polling the data from the server


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

        function SelectElement(identifier) {
            var elmnt = document.getElementById(identifier);
            elmnt.scrollIntoView(false);
        }



      

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };
        //'Passed', 'Failed','To be Executed'];
        $scope.ChangeFilter = function (value) {
            TestResultService.GetForGroup(testExecutionId, resultsArray => {
                var aux;
                $scope.GlobalArray = resultsArray;
                $scope.testProceduresArray = resultsArray;
                if (value == 'All') {

                    $scope.isFiltered = false;
                    $scope.CheckFilter();
                }
                if (value == 'Failed') {
                    aux = $scope.testProceduresArray.filter(test => test.Status == 'Fail')
                    $scope.testProceduresArray = aux;
                    $scope.isFiltered = true;
                    $scope.AutomaticSelectionStoredValue = $scope.AutomaticSelection;
                    $scope.AutomaticSelection = false;
                    $scope.filterapplied = true;
                } else
                    if (value == 'Passed') {
                        aux = $scope.testProceduresArray.filter(test => test.Status == 'Pass')
                        $scope.testProceduresArray = aux;
                        $scope.isFiltered = true;
                        $scope.AutomaticSelectionStoredValue = $scope.AutomaticSelection;
                        $scope.AutomaticSelection = false;
                        $scope.filterapplied = true;
                    } else
                        if (value == 'To be Executed') {
                            aux = $scope.testProceduresArray.filter(test => test.Status == 'TBE')
                            $scope.testProceduresArray = aux;
                            $scope.isFiltered = true;
                            $scope.AutomaticSelectionStoredValue = $scope.AutomaticSelection;
                            $scope.AutomaticSelection = false;
                            $scope.filterapplied = true;
                        }

                LoadTestResults();
            }, error => {

            })
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

        $scope.OpenGroup = function () {
            TestExecutionService.ChangeState(testExecution.Test_Execution_Id, "In progress", success => {
                GetTestExecution();
                ngToast.success({
                    content: 'The status of the execution has changed'
                });
                $scope.isOpen = !$scope.isOpen;
            }, err => {

            });
        };


        $scope.CloseGroup = function () {
            TestExecutionService.ChangeState(testExecution.Test_Execution_Id, "Finished", success => {
                GetTestExecution();
                ngToast.success({
                    content: 'The test execution has been finished'
                });
                $scope.isOpen = !$scope.isOpen;
                //$scope.goToDetails(executionId);
            }, err => {

            });
        };

        $scope.CancelGroup = function () {
            TestExecutionService.ChangeState(testExecution.Test_Execution_Id, "Closed", success => {
                GetTestExecution();
                ngToast.success({
                    content: 'The test execution has been closed'
                });
            }, err => {

            });
        };


        $scope.applyFilter = function () {
            var aux;
            if ($scope.FilterSelected == 'Failed') {
                aux = $scope.testProceduresArray.filter(test => test.Status == 'Fail')
                $scope.testProceduresArray = aux;
                $scope.isFiltered = true;
            } else
                if ($scope.FilterSelected == 'Passed') {
                    aux = $scope.testProceduresArray.filter(test => test.Status == 'Pass')
                    $scope.testProceduresArray = aux;
                    $scope.isFiltered = true;
                } else
                    if ($scope.FilterSelected == 'To be Executed') {
                        aux = $scope.testProceduresArray.filter(test => test.Status == 'TBE')
                        $scope.testProceduresArray = aux;
                        $scope.isFiltered = true;
                    } else {
                        $scope.isFiltered = false;
                    }
        }

        function GetTestStatus() {


            if (!modalExecuted && $scope.isValid) {
                $("#loadingModalPlay").modal({
                    backdrop: "static",
                    keyboard: false,
                    show: true
                });
                modalExecuted = true;
            }

            TestResultService.GetForGroup(testExecutionId, resultsArray => {
                TestResultService.GetForUser(testExecutionId, function (resultsList) {

                    $("#loadingModalPlay").modal("hide");

                    var results = resultsList;
                    if (results.length > 1) {
                        TestResultService.RemoveUser(testExecutionId, function (sucess) {
                            $state.reload();
                            ngToast.danger({
                                content: 'To avoid execution errors, a new test has been assigned to you'
                            });

                        }, function (error) {
                        });

                    }
                    $scope.GlobalArray = resultsArray;
                    $scope.GetTotal();
                    $scope.GetStrategy();

                    $scope.testProceduresArray = resultsArray;
                    $scope.applyFilter();
                    LoadTestResults();
                    GetProgressBarInfo();
                    selectNext();
                }, function (error) {

                });

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
                    testAux.PhotoUrl = null;
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

        function BlockButtons() {
            $scope.BlockButtons = false;
        }

        $scope.CheckTestStatus = function (test) {
            if (test.Test_Result_Id != $scope.testSelected.Test_Result_Id) {
                $scope.BlockButtons = true;
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.BlockButtons = false;
                }, 5000);
                TestResultService.Get(test.Test_Result_Id, function (data) {
                    $scope.IsTaken = false;
                    if (data.IsTaken) {
                        if ($scope.user.UserName != data.CurrentHolder) {
                            $scope.IsTaken = true;
                            if ($scope.testSelected.CurrentHolder == $scope.user.UserName) {
                                $scope.DeselectTest($scope.testSelected.Test_Result_Id);
                                $scope.RemoveTestHolder($scope.testSelected);
                            }
                            $scope.testSelected = data;
                        }
                    } else {
                        $scope.IsTaken = false;
                        $scope.ActiveTest = true;
                        if ($scope.testSelected.CurrentHolder == $scope.user.UserName) {
                            $scope.DeselectTest($scope.testSelected.Test_Result_Id);
                            $scope.ReassignTestResult($scope.testSelected, test);
                            GetTestInformation(test);
                        } else {
                            $scope.SetTestHolder(test);
                        }
                        test.PhotoUrl = $scope.user.PhotoUrl;
                        test.CurrentHolder = $scope.user.UserName;
                        $scope.testSelected = test;


                    }
                }, function (error) {

                });

            } else if ($scope.ActiveTest == false) {
                $scope.BlockButtons = true;
                $timeout.cancel(timer);
                timer = $timeout(function () {
                    $scope.BlockButtons = false;
                }, 5000);
                TestResultService.Get(test.Test_Result_Id, function (data) {
                    $scope.IsTaken = false;
                    if (data.IsTaken) {
                        if ($scope.user.UserName != data.CurrentHolder) {
                            $scope.IsTaken = true;
                            if ($scope.testSelected.CurrentHolder == $scope.user.UserName) {
                                $scope.DeselectTest($scope.testSelected.Test_Result_Id);
                                $scope.RemoveTestHolder($scope.testSelected);
                            }
                            $scope.testSelected = data;



                        }
                    } else {
                        $scope.IsTaken = false;
                        $scope.ActiveTest = true;
                        if ($scope.testSelected.CurrentHolder == $scope.user.UserName) {
                            $scope.DeselectTest($scope.testSelected.Test_Result_Id);
                            $scope.ReassignTestResult($scope.testSelected, test);
                            GetTestInformation(test);
                        } else {
                            $scope.SetTestHolder(test);
                        }
                        test.PhotoUrl = $scope.user.PhotoUrl;
                        test.CurrentHolder = $scope.user.UserName;
                        $scope.testSelected = test;


                    }
                }, function (error) {

                });
            }


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
                    AttachmentService.GetAttachment(3, test.Element_id, function (data) {
                        $scope.FilestoDisplay = data;
                    }, function (error) {

                    });
                }, err => {
                });
            }

            if (test.Evidence == 'TP') {
                TestProcedureService.Get(test.Element_id, result => {
                    $scope.testToDisplay = result;
                    LoadSteps(test.Element_id);
                    AttachmentService.GetAttachment(5, test.Element_id, function (data) {
                        $scope.FilestoDisplay = data;
                    }, function (error) {

                    });

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
                    AttachmentService.GetAttachment(4, test.Element_id, function (data) {
                        $scope.FilestoDisplay = data;
                    }, function (error) {

                    });
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

        $scope.submitReport = function () {
            if (typeof ($scope.testToDisplay.Title) != 'undefined' || $scope.testToDisplay.Title != null) {
                var resultValue = document.getElementById("result").value;
                let aux = 1;
                if (resultValue === "" || typeof (resultValue) == 'undefined' || resultValue == null) {
                    ngToast.danger({
                        content: 'Please fill the Test Result summary field'
                    });
                    aux = 0;
                    return
                }

                TestExecutionService.Get(testExecutionId, function (execution) {

                    if (execution.State == 'Closed') {
                        ngToast.danger({
                            content: 'This execution Is currently closed'
                        });
                        $state.reload();
                        return;
                    }


                    if (execution.State == 'Finished') {
                        ngToast.danger({
                            content: 'This execution has already finished'
                        });
                        $state.reload();
                        return;
                    }
                    if (execution.State == 'Changed') {
                        ngToast.danger({
                            content: 'The execution Group was changed during this test execution, please reload the test execution'
                        });

                        $scope.goToDetails(executionId);
                        return;
                    }

                    if (aux == 1) {
                        $scope.BlockButtons = true;
                        $timeout.cancel(timer);
                        timer = $timeout(function () {
                            $scope.BlockButtons = false;
                        }, 5000);
                        var result = document.getElementById("result").value;
                        let TestResult = {
                            Execution_Group_Id: executionId,
                            Test_Execution_Id: testExecutionId,
                            Execution_Result: result,
                            Status: 'Fail',
                            Test_Result_Id: $scope.testSelected.Test_Result_Id,
                            Evidence: $scope.testSelected.Evidence,
                            CurrentHolder: null,
                            IsTaken: false,
                            PhotoUrl: null
                        };

                        if ($scope.AutomaticSelection != true) {
                            TestResult.CurrentHolder = $scope.user.UserName;
                            TestResult.PhotoUrl = $scope.user.PhotoUrl;
                            TestResult.IsTaken = true;
                        }

                        TestResultService.Save(TestResult, r => {
                            if (r.Test_Result_Id == null) {
                                ngToast.danger({
                                    content: 'You are not the current holder of this test'
                                });
                                if ($scope.isFiltered != true) {
                                    selectFromStatus();
                                }

                            } else {
                                // $scope.RemoveTestHolder($scope.testSelected);
                                UpdateStatusFromExecution();
                            }


                        }, error => {

                        });

                        $scope.showSummary = false;
                    }

                }, function (error) {

                });



            } else {
                ngToast.danger({
                    content: 'Please select a test to execute'
                });
                $scope.BlockButtons = false;
            }
        }

        $scope.SetTestHolder = function (test) {
            GetTestInformation(test);
            let TestResult = {
                Test_Execution_Id: testExecutionId,
                Execution_Result: '',
                Status: 'TBE',
                TestId: $scope.testSelected.Element_id,
                Evidence: $scope.testSelected.Evidence,
                IsTaken: true,
                CurrentHolder: $scope.user.UserName,
                PhotoUrl: $scope.user.PhotoUrl
            };
            test.CurrentHolder = $scope.user.UserName;
            test.IsTaken = true;
            test.PhotoUrl = $scope.user.PhotoUrl;
            TestResultService.SetStatus(test, success => {
            }, error => {

            });
        }


        $scope.RemoveTestHolder = function (test) {
            let TestResult = {
                Test_Execution_Id: testExecutionId,
                Execution_Result: '',
                Status: 'TBE',
                TestId: test.Element_id,
                Evidence: test.Evidence,
                PhotoUrl: null,
                IsTaken: false,
                CurrentHolder: null
            };

            test.PhotoUrl = null;
            test.IsTaken = false;
            test.CurrentHolder = null;
            var NoUser = 0;
            if (typeof (test.Element_id) != 'undefined') {
                TestResultService.SetStatus(test, success => {


                }, error => {

                });
            }

        }

        function UpdateStatusFromExecution() {
            GetTestStatus();
            GetProgressBarInfo();
            if ($scope.isFiltered != true) {
                selectFromStatus();
            }

        }

        $scope.cancelSummary = function () {
            $scope.showSummary = false;
        }

        function selectNext() {
            if (count == 1) {
                if (typeof ($scope.testProceduresArray) != 'undefined') {

                    if ($scope.testProceduresArray.length != 0) {

                        $scope.testProceduresArray.forEach(function (test) {
                            if (count == 1 && test.CurrentHolder == $scope.user.UserName) {
                                $scope.ActiveTest = true;
                                count++;
                                $scope.SetTest(test);
                            }
                        });

                        $scope.testProceduresArray.forEach(function (test) {

                            if (test.Status == 'TBE' || test.Status == 'undefined') {

                                if (count == 1 && test.IsTaken != true) {

                                    count++;
                                    test.PhotoUrl = $scope.user.PhotoUrl;
                                    selectFromStatus();
                                }
                            }

                        });



                    }
                }

            }

        }


        $scope.SetTest = function (test) {
            test.isSelected = true;
            $scope.testSelected = test;
            GetTestInformation(test);
        }



        function selectFromStatus() {
            if ($scope.AutomaticSelection == true) {


                TestResultService.GetForGroup(testExecutionId, resultsArray => {
                    $scope.testProceduresArray = resultsArray;
                    LoadTestResults();
                    TestResultService.GetToExecute(testExecutionId, function (data) {
                        if (data.Test_Result_Id != null) {
                            var index = $scope.testProceduresArray.findIndex((test => test.Test_Result_Id == data.Test_Result_Id));
                            if (index != -1) {
                                $scope.ActiveTest = true;
                                SelectElement($scope.testProceduresArray[index].Identifier_number);
                                $scope.testProceduresArray[index].PhotoUrl = $scope.user.PhotoUrl;
                                $scope.testProceduresArray[index].CurrentHolder = data.CurrentHolder;
                                $scope.SetTest($scope.testProceduresArray[index]);
                                $timeout(BlockButtons, 5000);
                            }
                        } else
                            if (data.Test_Result_Id == null) {
                                $scope.ActiveTest = false;
                                if ($scope.TBD == 0 && showMessages && testExecution.State == 'Finished') {
                                    showMessages = false;
                                    ngToast.info({
                                        content: "The execution has been finished"
                                    });
                                    $scope.goToDetails(executionId);
                                }

                            }
                    }, function (error) {

                    });



                }, error => {
                });


            }
        }


        $scope.goToDetails = function (executionId) {
            $state.go('executionGroupDetails', { projectId: projectId, executionId, executionId });
        }


        $scope.PassTest = function () {
            $scope.BlockButtons = true;
            $timeout.cancel(timer);
            timer = $timeout(function () {
                $scope.BlockButtons = false;
            }, 5000);

            if (typeof ($scope.testToDisplay.Title) != 'undefined' || $scope.testToDisplay.Title != null) {


                TestExecutionService.Get(testExecutionId, function (execution) {
                    if (execution.State == 'Closed') {
                        ngToast.danger({
                            content: 'This execution Is currently closed'
                        });
                        $state.reload();
                        return;
                    }


                    if (execution.State == 'Finished') {
                        ngToast.danger({
                            content: 'This execution has already finished'
                        });
                        $state.reload();
                        return;
                    }
                    if (execution.State == 'Changed') {
                        ngToast.danger({
                            content: 'The execution Group was changed during this test execution, please reload the test execution'
                        });

                        $scope.goToDetails(executionId);
                        return;
                    }

                    let TestResult = {
                        Execution_Group_Id: executionId,
                        Test_Execution_Id: testExecutionId,
                        Execution_Result: $scope.testToDisplay.Expected_Result,
                        Status: 'Pass',
                        Test_Result_Id: $scope.testSelected.Test_Result_Id,
                        Evidence: $scope.testSelected.Evidence,
                        CurrentHolder: null,
                        IsTaken: false,
                        PhotoUrl: null
                    };

                    if ($scope.AutomaticSelection != true) {
                        TestResult.CurrentHolder = $scope.user.UserName;
                        TestResult.PhotoUrl = $scope.user.PhotoUrl;
                        TestResult.IsTaken = true;
                    }


                    TestResultService.Save(TestResult, r => {
                        if (r.Test_Result_Id == null) {
                            ngToast.danger({
                                content: 'You are not the current holder of this test'
                            });
                            selectFromStatus();
                        } else {
                            // $scope.RemoveTestHolder($scope.testSelected);
                            UpdateStatusFromExecution();
                        }
                    }, error => {

                    });

                }, function (error) {

                });

            } else {
                ngToast.danger({
                    content: 'Please select a test to execute'
                });
            }
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

        $scope.ReassignTestResult = function (TestRemoved, TestSelected) {
            var TestUpdated = TestRemoved;
            var TestAssigined = TestSelected;

            TestUpdated.CurrentHolder = null;
            TestUpdated.PhotoUrl = null;
            TestUpdated.IsTaken = false;

            TestAssigined.CurrentHolder = $scope.user.UserName;
            TestAssigined.PhotoUrl = $scope.user.PhotoUrl;
            TestAssigined.IsTaken = true;
            var auxArray = [];
            auxArray.push(TestUpdated);
            auxArray.push(TestAssigined);

            TestResultService.ReassignTestResult(auxArray, function (success) {
            }, function (error) {
            });

        }

        $scope.ClearSelection = function () {

            if ($scope.testSelected.CurrentHolder == $scope.user.UserName) {
                $scope.DeselectTest($scope.testSelected.Test_Result_Id);
                $scope.RemoveTestHolder($scope.testSelected);
                $scope.ActiveTest = false;
            } else {
                $scope.ActiveTest = false;
                ngToast.danger({
                    content: 'You no are the test owner'
                });
            }
        };


        $scope.PassAll = function () {
            $("#PassAll").hide();

            if (typeof ($scope.userAssigned.selectedItems) == 'undefined') {
                ngToast.danger({
                    content: 'Please select a user'
                });
            } else {
                TestResultService.PassAll(testExecutionId, $scope.userAssigned.selectedItems.UserName, $scope.optionSelected, function (success) {
                    ngToast.success({
                        content: 'Test Evidence Status changed correctly'
                    });
                    $scope.optionSelected = 'All';
                    GetTestStatus();
                }, function (error) {
                });
            }

        }

        $scope.FailAll = function () {
            $("#FailAll").modal("hide");

            if (typeof ($scope.userAssigned.selectedItems) == 'undefined') {
                ngToast.danger({
                    content: 'Please select a user'
                });
            } else {
                TestResultService.FailAll(testExecutionId, $scope.userAssigned.selectedItems.UserName, $scope.optionSelected, function (success) {
                    ngToast.success({
                        content: 'Test Evidence Status changed correctly'
                    });
                    GetTestStatus();
                    $scope.optionSelected = 'All';
                }, function (error) {
                });
            }
        }

        $scope.addUser = function (user) {
            var index = $scope.UsersSelected.findIndex(r => r.UserName == user.UserName);
            if (index == -1) {
                $scope.UsersSelected.push(user);
            }

        }

        $scope.removeUser = function (user) {
            var myFilter = $scope.UsersSelected.filter(u => u.UserName != user.UserName);
            $scope.UsersSelected = myFilter;

        }

        $scope.CleanUsers = function () {
            $scope.UsersSelected = [];
            $scope.userAssigned.selectedItems = null;
        }

        $scope.ApplyRemove = function () {
            var UserNotPresent = false;
            var notFound = 0;
            $scope.UsersSelected.forEach(user => {
                var index = $scope.testProceduresArray.findIndex(t => t.CurrentHolder == user.UserName);
                if (index == -1) {
                    UserNotPresent = true;
                } else {
                    notFound++;
                }
            });

            if ($scope.UsersSelected.length > 0) {
                TestResultService.RemoveFromUsersExecution(testExecutionId, $scope.UsersSelected, function (success) {
                    if (UserNotPresent && notFound != 0) {
                        ngToast.info({
                            content: 'One or more selected users werent found in the execution'
                        });
                    }
                    if (notFound != 0) {
                        ngToast.success({
                            content: 'Users Removed from execution'
                        });
                    } else {
                        ngToast.info({
                            content: "The selected user(s) weren't found"
                        });
                    }

                    $scope.CleanUsers();
                    GetTestStatus();
                }, function (error) {
                });
            } else {
                ngToast.danger({
                    content: 'You must select at least one user'
                });
            }
        }

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



        $scope.DownloadFile = function (id) {
            var fileIndx = $scope.FilestoDisplay.findIndex(file => file.Id == id);
            var filename = $scope.FilestoDisplay[fileIndx].Name;
            $http({
                method: 'GET',
                url: '/api/Attachment/DownLoad',
                params: { EntityId: id, name: filename },
                responseType: 'arraybuffer'
            }).then(function (response) {
                var filename = $scope.FilestoDisplay[fileIndx].Name;
                var contentType = "application/octet-stream";
                var linkElement = document.createElement('a');
                try {
                    var blob = new Blob([response.data], { type: contentType });
                    var url = window.URL.createObjectURL(blob);

                    linkElement.setAttribute('href', url);
                    linkElement.setAttribute("download", filename);

                    var clickEvent = new MouseEvent("click", {
                        "view": window,
                        "bubbles": true,
                        "cancelable": false
                    });
                    linkElement.dispatchEvent(clickEvent);
                } catch (err) {

                }

            }, function (err) {

            });

        }

    }]);
})();