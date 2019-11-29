(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TestSuplementalActivateController', ['$scope', '$state', '$window', 'AccessService', 'RequirementService', 'TestSuplementalService', 'ProjectService', 'ModalConfirmService', 'ngToast', function ($scope, $state, $window, AccessService, RequirementService, TestSuplementalService, ProjectService, ModalConfirmService, ngToast) {
        $scope.currentPageSTP = 1;
        $scope.pageSize = 10;
        var projectId = $state.params.projectId;
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.suplementals = [];
        $scope.id = projectId;
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
                    GetAllSuplemental();
                    GetRole();
                    GetProjectName($scope.id);
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
        function GetAllSuplemental() {
            TestSuplementalService.GetForProjectInactives(projectId, function (data) {
                $scope.suplementals = data;
            }, function (error) {
            });
        }

        $scope.supSelected = function (stp) {
            $scope.stpselected = stp;
        }

        $scope.projectDetails = function (id) {
            $state.go('projectDetails', { id: projectId });
        }

        $scope.suplementalDetails = function (id) {
            $state.go('suplementalDetails', { projectId: projectId, id: id });
        }

        $scope.showModalToEnable = function (req) {
            TestSuplementalService.Enable(req.Test_Suplemental_Id, function (data) {
                GetAllSuplemental();

                ngToast.success({
                    content: 'The Test Supplemental has been enabled successfully'
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