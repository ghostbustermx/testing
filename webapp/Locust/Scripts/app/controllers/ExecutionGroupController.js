(function () {

    'use strict';

    var app = angular.module(appName);
    app.controller('ExecutionGroupController', ['$scope', '$state', '$window', 'ExecutionGroupService', 'AccessService', 'ngToast', '$http', 'ProjectService', 'TestExecutionService', '$timeout', function ($scope, $state, $window, ExecutionGroupService, AccessService, ngToast, $http, ProjectService, TestExecutionService, $timeout) {
        $scope.currentPage = 1;
        var projectId = $state.params.projectId;
        $scope.ExecutionGroupsArray = "";
        $scope.Project = "";
        $scope.pageSize = 10;
        $scope.Options = [];
        $scope.Options2 = [];
        $scope.download = false;
        $scope.optionSelected = null;
        var tries = 0;
        $scope.TypesOfExecution = ["Manual", "Automated"];
        $scope.TypeSelected = "Manual";
        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelected = $scope.Options[0];
            }, function (error) {
            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };


        $scope.changeValue = function (value) {
            $scope.pageSize = value;
        }

        $scope.GetTestExecutions = function (groupId) {
            $scope.Options2 = [];
            TestExecutionService.GetForGroup(groupId, groups => {
                if (groups.length == 0) {
                    ngToast.danger({
                        content: 'The selected group doest have any execution'
                    });

                    return;
                }

                groups.forEach(r => {
                    $scope.Options2.push(r);
                });
                $("#Download").modal().show();
            }, error => {
            });

        }

        $scope.hasPermissionProject = false;
        HasAccess();

        $scope.ExecutionGroupsArray = [];

        function GetExecutionGroups() {
            ExecutionGroupService.GetByProjectActives(projectId, function (data) {

                $scope.ExecutionGroupsArray = data;
            }, function (errr) {

            });
        };

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
                    GetExecutionGroups();
                    GetProject();
                    GetRole();


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
            ProjectService.Get(projectId, r => {
                $scope.Name = r.Name;
            }, error => {

            });
        }

        $scope.AddExecutionGroup = function () {

            var p = new Promise((resolve, reject) => {
                $("#SelectFiltered").modal("hide");
                resolve("success");
            });

            var timer = $timeout(function () {
                p.then((message) => {
                    if ($scope.TypeSelected == 'Automated') {

                        $state.go('executionGroupsAddAutomated', { projectId: projectId});
                    } else {

                        $state.go('executionGroupsAddManual', { projectId: projectId});
                    }
                });
            }, 250);







        }

        $scope.goToEditGroup = function (id, IsAutomated) {
            if (IsAutomated) {
                $state.go('executionGroupsEditAutomated', { projectId: projectId, executionId: id})
            } else {
                $state.go('executionGroupsEditManual', { projectId: projectId, executionId: id})
            }

        }

        $scope.goToInactiveGroups = function () {
            $state.go('executionGroupInactives', { projectId: projectId });
        }

        $scope.SelectName = "";
        $scope.selectGroup = function (group) {
            $scope.EnabledId = group.Execution_Group_Id;
            $scope.SelectName = group.Name;
        }

        $scope.PlayGroup = function (executionId) {
            $state.go('executionGroupPlay', { projectId: projectId, executionId: executionId })
        }

        $scope.goToDetails = function (executionId, isAutomated) {
            if (isAutomated == 0) {
                $state.go('executionGroupDetailsManual', { projectId: projectId, executionId: executionId });

            } else {
                $state.go('executionGroupDetailsAutomated', { projectId: projectId, executionId: executionId });
            }
        }

        $scope.downloadFileExcel = function () {
            if (tries == 0) {
                if ($scope.optionSelected == null) {
                    ngToast.danger({
                        content: 'Select a test execution'
                    });
                    return;
                }
            }

            $("#loadingModalProjectReport").modal({
                backdrop: "static",
                keyboard: false,
                show: true
            });
            $scope.message = 'In queue to generate the excel report';
            ProjectService.RequestExcel(function (status) {


                if (status.assigned == true) {
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
                            executionId: $scope.optionSelected
                        },
                        dataType: "binary",
                        processData: false,
                    }).then(function (response) {
                        var filename = $scope.Name + date;
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

                            $("#loadingModalProjectReport").modal("hide");
                            $scope.optionSelected = null;
                        } catch (ex) {
                            $scope.download = false;
                            ngToast.danger({
                                content: "Error Server busy, please try again in a few minutes"
                            });
                            $("#loadingModalProjectReport").modal("hide");
                            $scope.optionSelected = null;
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
                            $("#loadingModalProjectReport").modal("hide");
                            $scope.optionSelected = null;
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
                        $("#loadingModalProjectReport").modal("hide");
                        $scope.optionSelected = null;
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
                    $("#loadingModalProjectReport").modal("hide");
                    $scope.optionSelected = null;
                }
            });



            //$scope.optionSelected = null;
        }

        $scope.DisableGroup = function () {
            ExecutionGroupService.Delete($scope.EnabledId, function (succees) {
                GetExecutionGroups();
                ngToast.success({
                    content: 'Execution Group was disabled successfully'
                });
            }, function (err) {
                ngToast.danger({
                    content: 'Execution Group could not be disabled'
                });
            });
        }
    }])
})();