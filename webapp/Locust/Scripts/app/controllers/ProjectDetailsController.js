(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('ProjectDetailsController', ['$scope', '$state', 'ProjectService', 'UserService', 'AccessService', 'TestSuplementalService', 'RequirementService', 'ModalConfirmService', 'ngToast', '$http', 'DashboardService', '$timeout', 'ExecutionGroupService', 'TestExecutionService', 'ScriptsGroupService', 'DateParse', 'TestEnvironmentService', function ($scope, $state, ProjectService, UserService, AccessService, TestSuplementalService, RequirementService, ModalConfirmService, ngToast, $http, DashboardService, $timeout, ExecutionGroupService, TestExecutionService, ScriptsGroupService, DateParse, TestEnvironmentService) {
        $scope.action = $state.current.data.action;
        var projectId = $state.params.id;
        HasAccess();
        $scope.currentPageREQ = 1;
        $scope.currentPageSTP = 1;
        $scope.currentPageScript = 1;
        $scope.pageSizeReq = 10;
        $scope.pageSizeSupp = 10;
        $scope.pageSizeScript = 10;
        $scope.download = false;
        localStorage.removeItem('steps');DateParse
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.requirements = [];
        $scope.dashboard = [];
        $scope.users = [];
        $scope.CurrentUserRole = "";
        $scope.optionSelected = 'Empty Report';
        $scope.Options2 = ['Empty Report', 'Last Execution'];
        $scope.Options = [];
        var tries = 0;
        

        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelectedReq = $scope.Options[0];
                $scope.RowsSelectedSupp = $scope.Options[0];

            }, function (error) {
            });
        }

        $scope.changeValueReq = function (value) {
            $scope.pageSizeReq = value;
        }
        $scope.changeValueSupp = function (value) {
            $scope.pageSizeSupp = value;
        }

        function HasAccess() {
            AccessService.HasAccess('Project', projectId, 0, 0, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    GetPaginationElements();
                    GetProject();
                    GetRole();
                    $scope.GetUsers(projectId);
                    $scope.GetTestEnvironment();
                }
            }, function (error) {
            });
        };

        function GetRole() {
            AccessService.GetRole(function (data) {
                $scope.CurrentUserRole = data.Role;
            }, function (error) {
                $scope.CurrentUserRole = null;
            });
        }

        function GetProject() {
            ProjectService.Get(projectId, function (data) {
                $scope.name = data.Name;
                $scope.id = projectId;
                if (data.Status == true) {
                    $scope.status = 'Active';
                    $scope.class = 'label-success';
                } else {
                    $scope.status = 'Inactive';
                    $scope.class = 'label-danger';
                }
                $scope.description = data.Description;
                GetRequirements(projectId);
                GetTestSuplementals(projectId)
                GetScriptGroups(projectId);
            }, function (error) {
            });
            ProjectService.GetDashboard(projectId, function (data) {
                $scope.dashboard.totalRequirements = data.totalRequirements;
                $scope.dashboard.requirementsMissingTestEvidence = data.requirementsMissingTestEvidence;
                $scope.dashboard.totalstp = data.totalstp;
                $scope.dashboard.totalTestCases = data.totalTestCases;
                $scope.dashboard.totalTestProcedures = data.totalTestProcedures;
                $scope.dashboard.totalTestScenarios = data.totalTestScenarios;
            }, function (error) {
            });
        }


        $scope.showModalToDelete = function (requirement) {
            RequirementService.Delete(requirement.Id, function (data) {
                GetProject();
            });
            ngToast.success({
                content: 'The requirement has been disabled successfully.'

            });
        }


        $scope.GetTestEnvironment = function () {
            TestEnvironmentService.GetActives(projectId, function (data) {
                $scope.TestEnv = data.length;
            }, function (error) {
            });
        }


        $scope.reqSelected = function (req) {
            $scope.rselected = req;
        }

        $scope.GetUsers = function (id) {
            UserService.GetUsersByProject(id, function (data) {
                $scope.users = data;
            });
        }

        $scope.GetGroups = function () {
            ExecutionGroupService.GetByProjectActives(projectId, function (data) {
                if (data.length == 0) {
                    $scope.Options2 = ['Empty Report'];
                }
                $("#Download").modal().show();
            }, function (errr) {

            });
        }

        $scope.rowSelected = function (row, projectId, type) {
            var url = "";
            switch (type) {
                case "STP":
                    url = $state.href('suplementalAddCopy', { id: row.Test_Suplemental_Id, projectId: projectId });
                    break;
                default:
            }
            window.open(url, '_parent', { focus: true });
        }



        $scope.downloadFileExcel = function () {

            $("#loadingModalReport").modal({
                backdrop: "static",
                keyboard: false,
                show: true
            });
            $scope.message = 'In queue to generate the excel report';

            ProjectService.RequestExcel(function (status) {
                if (status.assigned == true) {
                    $scope.message = 'Generating Project Report';
                    $scope.download = true;
                    var date = new Date();
                    date = date.toISOString();
                    date = date.replace(/:/g, "-");
                    date = date.replace(/\./g, "-");
                    $http({
                        method: 'GET',
                        url: '/api/Project/DownloadExcelEmpty',
                        responseType: 'arraybuffer',
                        params: {
                            id: projectId,
                            name: $scope.name,
                            type: $scope.optionSelected
                        },
                        dataType: "binary",
                        processData: false,
                    }).then(function (response) {
                        var filename = $scope.name + date;
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

                            $("#loadingModalReport").modal("hide");

                        } catch (ex) {
                            $scope.download = false;
                            ngToast.danger({
                                content: "Error Server busy, please try again in a few minutes"
                            });
                            $("#loadingModalReport").modal("hide");
                        }
                    }, function (error) {
                        if (tries < 11) {
                            $timeout(function () {
                                tries = tries + 1;
                                $scope.downloadFileExcel();

                            }, 5000);
                        }
                        else {
                            $scope.download = false;
                            ngToast.danger({
                                content: "Error Server busy, please try again in a few minutes"
                            });
                            tries = 0
                            $("#loadingModalReport").modal("hide");
                        }

                    });
                } else {
                    if (tries <= 10) {
                        $timeout(function () {
                            tries = tries + 1;
                            $scope.downloadFileExcel();

                        }, 5000);
                    }
                    else if (tries > 10) {
                        $scope.download = false;
                        ngToast.danger({
                            content: "Report Server busy, please try again in a few minutes"
                        });
                        tries = 0
                        $("#loadingModalReport").modal("hide");
                    }


                }
            }, function (error) {

                if (tries <= 10) {
                    $timeout(function () {
                        tries = tries + 1;
                        $scope.downloadFileExcel();

                    }, 5000);
                }
                else if (tries > 10) {
                    $scope.download = false;
                    ngToast.danger({
                        content: "Report Server busy, please try again in a few minutes"
                    });
                    tries = 0
                    $("#loadingModalReport").modal("hide");
                }
            });



        }



        $scope.supSelected = function (sup) {
            $scope.suplementalSelected = sup;
            TestSuplementalService.GetProcedures(sup.Test_Suplemental_Id, function (res) {
                $scope.Procedures = res;

                TestSuplementalService.GetScenarios(sup.Test_Suplemental_Id, function (res) {
                    $scope.Scenarios = res;
                });
            });
        }

        $scope.showModalToDeleteSuplemental = function (suplemental) {
            TestSuplementalService.Delete(suplemental.Test_Suplemental_Id, function (data) {
                GetProject();
                TestSuplementalService.AddChangeLog(suplemental.Test_Suplemental_Id, function (data) {
                });

            });
            ngToast.success({
                content: 'The Test Supplemental was disabled successfully'
            });
        }

        $scope.traceabilityDetails = function (id) {
            $state.go('traceabilityFinding', { projectId: id });
        }

        function GetRequirements(id) {
            RequirementService.GetProject(id, function (data) {
                $scope.requirements = data;
            }, function (error) {

            });
        }

        function GetScriptGroups(id) {
            ScriptsGroupService.GetAllScriptsGroup(id, function (data) {
                $scope.ScriptGroups = data;

                $scope.ScriptGroups.forEach(r => r.Creation_Date = DateParse.GetDate(r.Creation_Date));


            }, function (error) {
            });
        }

        function GetTestSuplementals(id) {
            TestSuplementalService.GetForProject(id, function (data) {
                $scope.suplementals = data;
            });
        }

        $scope.requirementDetails = function (id) {
            $state.go('requirementDetails', { projectId: projectId, id: id });
        }

        $scope.suplementalDetails = function (id) {
            $state.go('suplementalDetails', { projectId: projectId, id: id });
        }

        $scope.redirectToList = function () {
            $state.go('Projects');
        };

        $scope.NavigateSupplementalTestAdd = function () {
            $state.go('suplementalAdd', { projectId: projectId });
        };

        $scope.navigateActivateTestSupplemental = function () {
            $state.go('testSuplementalActivate', { projectId: projectId });
        };

        $scope.goToExecutionGroups = function (id) {
            $state.go('executionGroups', { projectId: id });
        }

        $scope.goToProjectExecution = function (id) {
            $scope.Options2 = [];
            TestExecutionService.GetByProject(projectId, groups => {
                $scope.Options2 = groups;
                $scope.ExecutionSelected = $scope.Options2[0];
                if (typeof ($scope.ExecutionSelected) == 'undefined') {
                    ngToast.danger({
                        content: 'The project selected has not execution'
                    });
                }
                $state.go('projectExecutions', { projectId: id, executionId: $scope.ExecutionSelected.Test_Execution_Id });
            }, error => {
            });

        }

        $scope.NavigateAddScriptGroup = function () {
            $state.go('ScriptGroupAdd', { projectId: projectId });
        }


    }])
})();