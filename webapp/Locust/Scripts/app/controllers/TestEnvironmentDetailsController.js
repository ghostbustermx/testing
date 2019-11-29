(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName)
    app.controller('TestEnvironmentDetailsController', ['$scope', '$window', '$state', 'TestEnvironmentService', 'AccessService', 'ngToast', 'ProjectService', function ($scope, $window, $state, TestEnvironmentService, AccessService, ngToast, ProjectService) {
        $scope.action = $state.current.data.action;
        var projectId = $state.params.projectId;
        var id = $state.params.id;
        HasAccess();

        function HasAccess() {
            AccessService.HasAccess('Project', projectId, 0, 0, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    AccessService.HasAccess('TestEnvironment', projectId, id, 0, 0, 0, 0, function (data) {
                        if (!data.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            GetProject();
                            GetTE(id);
                        }
                    }, function (error) {
                    });
                }
            }, function (error) {

            });
        };

        function GetProject() {
            ProjectService.Get(projectId, r => {
                $scope.NameProject = r.Name;
            }, error => {

            });
        }

        function GetTE(id) {
            TestEnvironmentService.Get(id, function (data) {
                $scope.Id = id;
                $scope.Name = data.Name;
                $scope.Server = data.Server;
                $scope.Processor = data.Processor;
                $scope.RAM = data.RAM;
                $scope.HardDisk = data.HardDisk;
                $scope.os = data.OS;
                $scope.ServerSoftwareDevs = data.ServerSoftwareDevs;
                $scope.ServerSoftwareTest = data.ServerSoftwareTest;
                $scope.Database = data.Database;
                $scope.URL = data.URL;
                $scope.type = data.SiteType;
                $scope.Notes = data.Notes;
                $scope.Creator = data.Creator;
                $scope.Last_Editor = data.Last_Editor;
                $scope.IsActive = data.IsActive;
            }, function (error) {

            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };

        $scope.testEnvironment = function () {
            $state.go('testEnvironment', { projectId: projectId });
        };

        $scope.redirectToList = function () {
            $state.go('testEnvironment', { projectId: projectId });
        };
    }])
})();