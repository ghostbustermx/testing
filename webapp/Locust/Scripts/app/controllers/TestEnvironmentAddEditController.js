(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName)
    app.controller('TestEnvironmentAddEditController', ['$scope', '$window', '$state', 'TestEnvironmentService', 'AccessService', 'ngToast', 'ProjectService', function ($scope, $window, $state, TestEnvironmentService, AccessService, ngToast, ProjectService) {
        $scope.action = $state.current.data.action;
        $scope.btnSave = false;
        var projectId = $state.params.projectId;
        var id = $state.params.id;
        $scope.OS = [];
        $scope.TETypes = [];
        HasAccess();
        $scope.os = { selectedItems: '' };
        $scope.type = { selectedItems: '' };

        function GetOS() {
            TestEnvironmentService.GetOS(function (data) {
                $scope.OS = data;
            }, function (error) {
            });
        }

        function GetTETypes() {
            TestEnvironmentService.GetTETypes(function (data) {
                $scope.TETypes = data;
            }, function (error) {
            });
        }

        function HasAccess() {
            if ($scope.action == 'Add') {
                AccessService.HasAccess('Project', projectId, 0, 0, 0, 0, 0, function (data) {
                    if (!data.HasPermission) {
                        $state.go('Projects').then(function () {
                            ngToast.danger({
                                content: 'You do not have access to the requested page.'
                            });
                        });
                    } else {
                        GetTE(id);
                        GetOS();
                        GetTETypes();
                        GetProject();
                        GetRole();
                    }
                }, function (error) {
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
                        GetTE(id);
                        GetOS();
                        GetTETypes();
                        GetProject();
                        GetRole();
                    }
                }, function (error) {
                });
            }
        };

        function GetRole() {
            AccessService.GetRole(function (data) {
                $scope.CurrentUserRole = data.Role;
                if (data.Role == 'VBP' || data.Role == 'BA') {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                }
            }, function (error) {

            });
        }

        function GetProject() {
            ProjectService.Get(projectId, r => {
                $scope.NameProject = r.Name;
            }, error => {

            });
        }

        function GetTE(id) {
            if ($scope.action == 'Edit') {
                TestEnvironmentService.Get(id, function (data) {
                    $scope.Id = id;
                    $scope.Name = data.Name;
                    $scope.Server = data.Server;
                    $scope.Processor = data.Processor;
                    $scope.RAM = data.RAM;
                    $scope.HardDisk = data.HardDisk;
                    $scope.os.selectedItems = data.OS;
                    $scope.ServerSoftwareDevs = data.ServerSoftwareDevs;
                    $scope.ServerSoftwareTest = data.ServerSoftwareTest;
                    $scope.Database = data.Database;
                    $scope.URL = data.URL;
                    $scope.type.selectedItems = data.SiteType;
                    $scope.Notes = data.Notes;
                    $scope.IsActive = data.IsActive;
                    $scope.Creator = data.Creator;
                }, function (error) {

                });
            }
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };

        $scope.testEnvironment = function () {
            $state.go('testEnvironment', { projectId: projectId });
        };
        $scope.validateUrl = function () {
            var re = new RegExp("^https?://");
            if ($scope.URL != "" && typeof ($scope.URL) != 'undefined') {
                if (!$scope.URL.match(re)) {
                   return false;
                }
            }
            return true;
        }
        $scope.AddUpdate = function (form) {
            if (!$scope.validateUrl()) {
                ngToast.danger({
                    content: 'Please Specify the http or https protocol'
                });
                return;
            } else{
            if (form.$valid) {
                $scope.btnSave = true;
                var te = {
                    Id: $scope.Id,
                    Name: $scope.Name,
                    Server: $scope.Server,
                    Processor: $scope.Processor,
                    RAM: $scope.RAM,
                    HardDisk: $scope.HardDisk,
                    OS: $scope.os.selectedItems,
                    SiteType: $scope.type.selectedItems,
                    ServerSoftwareDevs: $scope.ServerSoftwareDevs,
                    ServerSoftwareTest: $scope.ServerSoftwareTest,
                    Database: $scope.Database,
                    URL: $scope.URL,
                    Notes: $scope.Notes,
                    ProjectId: projectId,
                    IsActive: true,
                    Creator: $scope.Creator
                };
                var action;
                if ($scope.action == 'Add') {
                    action = 'added'
                } else {
                    action = 'edited'
                }

                var method = ($scope.action == 'Add') ? TestEnvironmentService.Save : TestEnvironmentService.Update;
                method(te, function (data) {
                    $state.go('testEnvironment', { projectId: projectId }).then(function () {
                        ngToast.success({
                            content: 'The test environment has been ' + action + ' successfully.'
                        });
                    });
                }, function (response) {
                    $scope.btnSave = false;

                    ngToast.danger({
                        content: 'The test environment has not been ' + action + '.'
                    });
                });

            } else {
                $scope.btnSave = false;
                ngToast.info({
                    content: 'Please complete all fields correctly.'
                });
            }
        }

        $scope.redirectToList = function () {
            $state.go('testEnvironment', { projectId: projectId });
        }
        };

    }])
})();