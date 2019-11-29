(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('RequirementActivateController', ['$scope', '$state', '$window', 'AccessService', 'RequirementService', 'ProjectService', 'ModalConfirmService', 'ngToast', function ($scope, $state, $window, AccessService, RequirementService, ProjectService, ModalConfirmService, ngToast) {
        $scope.currentPageREQ = 1;
        $scope.pageSize = 10;
        var projectId = $state.params.projectId;
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.requirements = [];
        $scope.CurrentUserRole = "";
        $scope.Options = [];

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
                    GetAllRequirements();
                    GetRole();
                    GetProjectName(projectId);
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

        function GetAllRequirements() {
            RequirementService.GetProjectInactives(projectId, function (data) {
                $scope.requirements = data;
            }, function (error) {
            });
        }

        $scope.reqSelected = function (req) {
            $scope.rselected = req;
        }

        $scope.projectDetails = function (id) {
            $state.go('projectDetails', { id: projectId });
        }

        $scope.requirementDetails = function (id) {
            $state.go('requirementDetails', { projectId: projectId, id: id });
        }

        $scope.showModalToEnable = function (req) {
            RequirementService.Enable(req.Id, function (data) {
                GetAllRequirements();
                ngToast.success({
                    content: 'The requirement has been enabled successfully.'
                });
            });

        }

        function GetProjectName(id) {
            ProjectService.Get(id, function (data) {
                $scope.projectName = data.Name;
            });
        }

    }])
})();