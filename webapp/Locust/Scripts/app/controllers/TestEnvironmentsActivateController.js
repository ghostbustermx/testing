(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TestEnvironmentsActivateController', ['$scope', '$state', '$window', 'AccessService', 'ProjectService', 'ModalConfirmService', 'ngToast', 'TestEnvironmentService', function ($scope, $state, $window, AccessService, ProjectService, ModalConfirmService, ngToast, TestEnvironmentService) {
        $scope.currentPage = 1;
        $scope.pageSize = 10;
        var projectId = $state.params.projectId;
        $scope.projectId = projectId;
        $scope.projects = [];
        HasAccess();
        $scope.TE = [];
        $scope.teSelected = "";

        function GetProject() {
            ProjectService.Get(projectId, r => {
                $scope.NameProject = r.Name;
            }, error => {

            });
        }
        $scope.Options = [];

        function GetTE() {
            TestEnvironmentService.GetInactives(projectId, function (data) {
                $scope.TE = data;
            }, function (error) {
            });
        };

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
                    GetProject();
                    GetTE();
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

        $scope.EnableTE = function (value) {
            value.IsActive = true;
            TestEnvironmentService.Update(value, function (data) {
                GetTE();
                ngToast.success({
                    content: 'The Test Environment has been enabled successfully.'
                });
            }, function (error) {
                ngToast.danger({
                    content: 'The Test Environment has not been enabled.'
                });
            });
        }

        $scope.selectTE = function (value) {
            $scope.teSelected = value;
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };

        $scope.testEnvironment = function () {
            $state.go('testEnvironment', { projectId: projectId });
        };
    }])
})();