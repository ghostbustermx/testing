(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('RequirementChangeLogController', ['$scope', '$state', 'RequirementService', 'AccessService', 'ProjectService', 'ModalConfirmService', 'ngToast', function ($scope, $state, RequirementService, AccessService, ProjectService, ModalConfirmService, ngToast) {
        $scope.action = $state.current.data.action;
        var requirementId = $state.params.id;
        var projectId = $state.params.projectId;
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
            AccessService.HasAccess('Requirement', projectId, requirementId, 0, 0, 0, 0, function (data) {
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
            RequirementService.Get(requirementId, function (data) {
                $scope.project_id = data.Project_Id;
                $scope.axosoft_Task_Id = data.Axosoft_Task_Id;
                $scope.name = data.Name;
                $scope.id = requirementId;
                $scope.description = data.Description;
                $scope.req_number = data.req_number;
                $scope.developerAssigned = data.Developer_Assigned;
                $scope.testerAssigned = data.Tester_Assigned;
                $scope.status = data.Status;
                GetProject($scope.project_id);
                RequirementService.RequirementChangeLogs(requirementId, function (data) {
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
            var Requirement = {
                Id: changeLog.Id,
                Project_Id: changeLog.Project_Id,
                Name: changeLog.Name,
                Description: changeLog.Description,
                Developer_Assigned: changeLog.Developer_Assigned,
                Tester_Assigned: changeLog.Tester_Assigned,
                Axosoft_Task_Id: changeLog.Axosoft_Task_Id,
                Status: status,
                req_number: changeLog.req_number,
                Acceptance_Criteria: changeLog.Acceptance_Criteria,
                Release: changeLog.Release
            }
            RequirementService.Restore(Requirement, changeLog.Version, function (data) {
                ngToast.success({
                    content:'Requirement restored successfully'
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

        $scope.requirementDetails = function () {
            $state.go('requirementDetails', { projectId: projectId, id: requirementId });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: $scope.project_id });
        };

        function GetProject(id) {
            ProjectService.Get(id, function (data) {
                $scope.Project_Id = data.Name;
            });
        }
    }])
})();