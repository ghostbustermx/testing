(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName)
    app.controller('TestEnvironmentController', ['$scope', '$window', '$state', 'TestEnvironmentService', 'AccessService', 'ngToast', 'ProjectService', function ($scope, $window, $state, TestEnvironmentService, AccessService, ngToast, ProjectService) {
        $scope.currentPage = 1;
        $scope.pageSize = 10;
        var projectId = $state.params.projectId;
        HasAccess();
        $scope.TE = [];
        $scope.projectId = $state.params.projectId;
        $scope.teSelected = '';
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
        $scope.selectTE = function (value) {
            $scope.teSelected = value;
        }

        $scope.DisableTE = function (value) {
            TestEnvironmentService.HasRelationships(value.Id, function (hasRelationships) {
                if (hasRelationships.length == 0) {
                    value.IsActive = false;
                    TestEnvironmentService.Update(value, function (data) {
                        GetTE();
                        ngToast.success({
                            content: 'The Test Environment has been disabled successfully.'
                        });
                    }, function (error) {
                        ngToast.danger({
                            content: 'The Test Environment has not been disabled.'
                        });
                    });
                } else {
                    var groups = [];
                    hasRelationships.forEach(function (element) {
                        groups.push(element.Name);
                    });
                    ngToast.danger({
                        content: 'The Test Environment "' + value.Name + '" cannot be disabled because has relationships with the groups: [' + groups + "]."
                    });
                }
            }, function (error) {
                ngToast.danger({
                    content: 'The Test Environment has not been disabled.'
                });
            });
        }

        function GetProject() {
            ProjectService.Get(projectId, r => {
                $scope.NameProject = r.Name;
            }, error => {

            });
        }

        $scope.OpenURL = function (value) {
            window.open(value, '_blank', { focus: true });
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
                    GetTE();
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

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };

        function GetTE() {
            TestEnvironmentService.GetActives(projectId, function (data) {
                $scope.TE = data;
            }, function (error) {
            });
        };
    }])
})();