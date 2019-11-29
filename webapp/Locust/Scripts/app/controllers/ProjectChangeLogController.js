(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('ProjectChangeLogController', ['$scope', '$state', 'ProjectService', 'AccessService', 'ModalConfirmService', 'ngToast', function ($scope, $state, ProjectService, AccessService, ModalConfirmService, ngToast) {
        $scope.action = $state.current.data.action;
        var projectId = $state.params.id;
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.requirements = [];
        $scope.changeLogs = [];
        $scope.CurrentUserRole = "";

        function HasAccess() {
            AccessService.HasAccess('Project', projectId, 0, 0, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    GetVersions();
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

        function GetVersions() {
            ProjectService.Get(projectId, function (data) {
                $scope.name = data.Name;
                $scope.id = projectId;
                if (data.Status == false) {
                    $scope.status = 'Inactive';
                } else {
                    $scope.status = 'Active';
                }
                $scope.description = data.Description;

                ProjectService.ProjectChangeLogs(projectId, function (data) {
                    $scope.changeLogs = [];
                    var arrayChangeLogs = data;
                    var cont = 0;
                    arrayChangeLogs.forEach(function (cl) {
                        if (cl.Active != true) {
                            var content = angular.fromJson(cl.Content);
                            content.User = cl.User;
                            content.Version = cl.Version;
                            if (content.Status == true) {
                                content.Status = 'Active'
                            } else {
                                content.Status = 'Inactive'
                            }
                            $scope.changeLogs.push(content);
                        }
                        cont = cont + 1;
                    });
                });
            }, function (error) {
            });
        }

        $scope.restore = function (changeLog) {
            var status = true;
            if (changeLog.Status == "Active") {
                status = true;
            } else {
                status = false;
            }
            var Project = {
                Id: changeLog.Id,
                Name: changeLog.Name,
                Status: status,
                Description: changeLog.Description,
                Image: changeLog.Image
            }
            ProjectService.Restore(Project, changeLog.Version, function (data) {
                ngToast.success({
                    content: 'Project restored successfully'
                })
                GetVersions();
            });
        }

        $scope.clSelected = function (cl) {
            $scope.clselected = cl;
        }

        $scope.redirectToList = function () {
            $state.go('Projects');
        };

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };
    }])
})();