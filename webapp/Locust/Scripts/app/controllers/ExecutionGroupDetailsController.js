(function () {

    'use strict';

    var app = angular.module(appName);
    app.controller('ExecutionGroupDetailsController', ['$scope', '$state', '$window', 'DateParse', 'ExecutionGroupService', 'ExecutionTestService', 'AccessService', 'ngToast', '$http', 'TestExecutionService', 'ProjectService', 'TestResultService', '$timeout', function ($scope, $state, $window, DateParse, ExecutionGroupService, ExecutionTestService, AccessService, ngToast, $http, TestExecutionService, ProjectService, TestResultService, $timeout) {

        var projectId = $state.params.projectId;
        var executionId = $state.params.executionId;
        var type = $state.current.data.type;

        HasAccess();
        $scope.currentPage = 1;
        $scope.Options = [];
        $scope.pageSize = 10;
        $scope.testGroups = [];
        $scope.displayMenu = false;
        $scope.download = false;
        $scope.Name = "";
        var tries = 0;
        function GetTestExecutions() {
            TestExecutionService.GetForGroup(executionId, groups => {
                $scope.testGroups = groups;
                $scope.testGroups.forEach(test => {
                    test.CreationDate = DateParse.GetDate(test.CreationDate);
                });
            }, error => {
            });

        }

        function HasAccess() {
            console.log(type);
            debugger;
            AccessService.HasAccess('Project', projectId, 0, 0, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    AccessService.HasAccess('ExecutionGroup', projectId, executionId, 0, 0, 0, 0, function (data) {
                        if (!data.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            GetPaginationElements();
                            GetTestExecutions();
                            GetProject();
                            GetRole();
                        }
                    }, function (error) {
                    });
                }
            }, function (error) {

            });
        };
        function GetRole() {
            AccessService.GetRole(function (data) {
                $scope.CurrentUserRole = data.Role;
            }, function (error) {

            });
        }



        $scope.changeValue = function (value) {
            $scope.pageSize = value;
        }

        function GetProject() {
            ProjectService.Get(projectId, r => {
                $scope.pName = r.Name;
            }, error => {

            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };

        $scope.ExecutionList = function () {
            $state.go('executionGroups', { projectId: projectId });
        };

        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelected = $scope.Options[0];
            }, function (error) {
            });
        }

        $scope.UpdateExecution = function (form) {
            if (form.$valid) {
                $scope.myTest.Name = $scope.Name;
                $scope.myTest.Description = $scope.Description;
                $scope.myTest.Version = $scope.Version;
                TestExecutionService.Update($scope.myTest, success => {
                    GetTestExecutions();
                    ngToast.success({
                        content: 'Execution Updated!'
                    });
                    $('#modalEditForm').modal('hide');
                    $scope.CleanFields();
                }, error => {
                });
            } else {
                ngToast.danger({
                    content: 'Please fill all the required fields correctly'
                });
            }
        };

        function GetExecutionGroup() {

            ExecutionGroupService.Get(executionId, function (execution) {
            }, function (error) {

            });
        }


        $scope.createExecution = function (form) {

            ExecutionGroupService.Get(executionId, function (execution) {
                if (execution.IsReadyToExecute == false) {
                    ngToast.danger({
                        content: 'The Execution Group is not yet ready, please wait a few moments'
                    });

                    return;
                }


            if (form.$valid) {
                var values = $scope.testGroups.filter(obj => obj.State == "In progress" || obj.State == "Created");

                if (values.length > 0) {
                    ngToast.info({
                        content: 'The group "' + $scope.Name + '" cannot be created because exists others with status "In progress" or "Created", Please finish or closed them first to create others.'
                    });
                    $('#modalRegisterForm').modal('hide');
                    $scope.CleanFields();
                    return;
                }

                let TestExecution = {
                    Name: $scope.Name,
                    Description: $scope.Description,
                    Version: $scope.Version,
                    Execution_Group_Id: executionId,
                    State: "Created",
                    HasResultsCreated: true
                };

                $("#loadingModal").modal({
                    backdrop: "static",
                    keyboard: false,
                    show: true
                });

                TestExecutionService.Save(TestExecution, executionData => {
                    $('#modalRegisterForm').modal('hide');
                    var idExecution = executionData.Test_Execution_Id;
                    ngToast.success({
                        content: 'Group Created'
                    });
                    $scope.CleanFields();
                    GetTestExecutions();
                    TestResultService.CreateTestResults(executionId, idExecution, function (sucess) {
                        $("#loadingModal").modal("hide");
                    }, function (error) {


                    });


                }, error => {
                    ngToast.danger({
                        content: 'Error'
                    });

                });
            } else {
                ngToast.danger({
                    content: 'Please fill all the required fields correctly'
                });
                }

            }, function (error) {

            });
        }

        $scope.DisplaySMenu = function (test) {
            test.displayMenu = true;
        };

        $scope.HideMenu = function (test) {
            test.displayMenu = false;
        };

        $scope.SetState = function (test, State) {
            $scope.selectedState = State;
            $scope.ATest = test;
        };

        $scope.ViewGroup = function (id) {
            $state.go('executionGroupView', { projectId: projectId, executionId: executionId, testExecutionId: id });
        }


        $scope.ChangeStatus = function () {
            TestExecutionService.ChangeState($scope.ATest.Test_Execution_Id, $scope.selectedState, success => {
                ngToast.success({
                    content: 'Execution Status Changed'
                });
                GetTestExecutions();
            }, error => {

            });
        };

        $scope.PlayGroup = function (id, state) {

            ExecutionGroupService.Get(executionId, function (execution) {
                if (execution.IsReadyToExecute == false) {
                    ngToast.danger({
                        content: 'The Execution Group is not yet ready, please wait a few moments'
                    });

                    return;
                }


            TestResultService.GetForGroup(id, result => {
                debugger;
                if (state == "Created") {
                    if (result.length == 0) {
                        ngToast.info({
                            content: 'The group cannot be started without any test evidence assigned.'
                        });
                        return;
                    }
                    TestExecutionService.ChangeState(id, "In progress", success => {
                        $state.go('executionGroupPlay', { projectId: projectId, executionId: executionId, testExecutionId: id });
                    }, err => {

                    });
                } else if (state == "Changed") {
                    TestExecutionService.ChangeState(id, "In progress", success => {


                        TestResultService.UpdateTestResults(executionId, id, success => {
                            TestResultService.GetForGroup(id, result2 => {
                                if (result2.length == 0) {
                                    ngToast.info({
                                        content: 'The group cannot be started without any test evidence assigned.'
                                    });
                                    return;
                                } else {
                                    $state.go('executionGroupPlay', { projectId: projectId, executionId: executionId, testExecutionId: id });
                                }
                            }, error => {


                                });

                            
                        }, error => {

                        })
                    }, err => {
                        $state.go('executionGroupPlay', { projectId: projectId, executionId: executionId, testExecutionId: id });
                    });
                } else {
                    if (result.length == 0) {
                        ngToast.info({
                            content: 'The group cannot be started without any test evidence assigned.'
                        });
                        return;
                    }
                    $state.go('executionGroupPlay', { projectId: projectId, executionId: executionId, testExecutionId: id });
                }

            }, err => {

                });


            }, function (error) {

            });
        }

        $scope.CleanFields = function () {
            $scope.Name = "";
            $scope.Description = "";
            $scope.Version = "";
            $scope.Name = $scope.Name.trim();
            $scope.Description = $scope.Description.trim();
            $scope.Version = $scope.Version.trim();

        };

        $scope.selectGroup = function (test) {
            $scope.myTest = test;
            $scope.Name = test.Name;
            $scope.Description = test.Description;
            $scope.Version = test.Version;

            $scope.selectedState = test.State;
        };


        $scope.downloadFileExcel = function (executionId) {

            $("#loadingModalExecutionReport").modal({
                backdrop: "static",
                keyboard: false,
                show: true
            });
            $scope.message = 'In queue to generate the excel report';

            ProjectService.RequestExcel(function (status) {



                if (status.assigned == true) {
                    $scope.download = true;

                    $scope.message = 'Generating Project Execution Report';

                    $scope.download = true;


                    var date = new Date();
                    date = date.toISOString();
                    date = date.replace(/:/g, "-");
                    date = date.replace(/\./g, "-");
                    $http({
                        method: 'GET',
                        url: '/api/Project/DownloadExecutionReport',
                        responseType: 'arraybuffer',
                        params: {
                            id: projectId,
                            name: $scope.Name,
                            executionId: executionId
                        },
                        dataType: "binary",
                        processData: false,
                    }).then(function (response) {
                        var filename = $scope.pName + date;
                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


                        try {
                            var blob = new Blob([response.data], { type: contentType });
                            var url = window.URL.createObjectURL(blob);
                            var linkElement = document.createElement('a');
                            linkElement.setAttribute('href', url);
                            linkElement.setAttribute("download", filename);
                            var clickEvent = new MouseEvent("click", {
                                "view": window,
                                "bubbles": true,
                                "cancelable": false
                            });
                            linkElement.dispatchEvent(clickEvent);
                            $scope.download = false;

                            $("#loadingModalExecutionReport").modal("hide");

                        } catch (ex) {
                            $scope.download = false;
                            ngToast.danger({
                                content: "Error: " + ex
                            });
                        }
                    }, function (error) {
                        $scope.download = false;
                        ngToast.danger({
                            content: "Report Server busy, please try again in a few minutes"
                        });
                        $("#loadingModalExecutionReport").modal("hide");
                    });
                } else {
                    if (tries <= 10) {
                        $timeout(function () {
                            tries = tries + 1;
                            $scope.downloadFileExcel(executionId);

                        }, 5000);
                    }
                    else if (tries > 10) {
                        $scope.download = false;
                        ngToast.danger({
                            content: "Report Server busy, please try again in a few minutes"
                        });
                        tries = 0
                        $("#loadingModalExecutionReport").modal("hide");
                    }


                }
            }, function (error) {

                if (tries <= 10) {
                    $timeout(function () {
                        tries = tries + 1;
                        $scope.downloadFileExcel(executionId);

                    }, 5000);
                }
                else if (tries > 10) {
                    $scope.download = false;
                    ngToast.danger({
                        content: "Report Server busy, please try again in a few minutes"
                    });
                    tries = 0
                    $("#loadingModalExecutionReport").modal("hide");
                }
            });
        };

    }]);
})();