(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TraceabilityFindingController', ['$scope', '$state', '$window', 'TraceabilityFindingService', 'AccessService', 'ProjectService', 'RequirementService', 'ModalConfirmService', 'ngToast', function ($scope, $state, $window, TraceabilityFindingService, AccessService, ProjectService, RequirementService, ModalConfirmService, ngToast) {
        $scope.currentPage = 1;
        $scope.pageSizeReq = 10;
        $scope.pageSizeTC = 10;
        $scope.pageSizeTP = 10;
        $scope.currentPageREQ = 1;
        $scope.currentPageTC = 1
        $scope.currentPageTP = 1
        var projectId = $state.params.projectId;
        $scope.CurrentUserRole = ""
        HasAccess();
        $scope.requirements = [];
        $scope.testProcedures = [];
        $scope.testCases = [];
        $scope.Options = [];


        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelectedReq = $scope.Options[0];
                $scope.RowsSelectedTC = $scope.Options[0];
                $scope.RowsSelectedTP = $scope.Options[0];

            }, function (error) {
            });
        }

        $scope.changeValueReq = function (value) {
            $scope.pageSizeReq = value;
        }
        $scope.changeValueTC = function (value) {
            $scope.pageSizeTC = value;
        }
        $scope.changeValueTP = function (value) {
            $scope.pageSizeTP = value;
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

        function GetAllTraceabilityFindings() {
            TraceabilityFindingService.GetTraceabilityFindings(projectId, function (data) {
                $scope.requirements = data.Requirements;
                $scope.testProcedures = data.TP;
                $scope.testCases = data.TC;
            }, function (error) {
            });
        }

        $scope.testcaseEdit = function (id) {
            RequirementService.GetRequirementbyTCId(id, function (data) {
                $state.go('testcaseEdit', { projectId: projectId, id: id, reqId: data.Id });
            }, function (error) {
            });
        };

        $scope.requirementDetails = function (id) {
            $state.go('requirementDetails', { projectId: projectId, id: id })
        };

        $scope.testprocedureEdit = function (id) {
            RequirementService.GetRequirementbyTPId(id, function (data) {
                $state.go('testprocedureEdit', { projectId: projectId, id: id, reqId: data.Id });
            }, function (error) {

            });
        };


        function GetProject() {
            ProjectService.Get(projectId, function (data) {
                $scope.projectName = data.Name;
                $scope.id = projectId;
                if (data.Status == true) {
                    $scope.status = 'Active';
                    $scope.class = 'label-success';
                } else {
                    $scope.status = 'Inactive';
                    $scope.class = 'label-danger';
                }
                $scope.description = data.Description;
                GetAllTraceabilityFindings(projectId);
            }, function (error) {
            });
        }

        $scope.redirectTo = function (url) {
            $window.open(url, '_blank');
        };

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        }

    }])
})();