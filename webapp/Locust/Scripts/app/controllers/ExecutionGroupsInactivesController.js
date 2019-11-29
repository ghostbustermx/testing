(function () {
    'use strict';

    var app = angular.module(appName);

    app.controller('ExecutionGroupsInactivesController', ['$scope', '$state', '$window', 'ExecutionGroupService', 'AccessService', 'ngToast', 'ProjectService', function ($scope, $state, $window, ExecutionGroupService, AccessService, ngToast, ProjectService) {

        $scope.currentPage = 1;
        var projectId = $state.params.projectId;
        $scope.ExecutionGroupsArray = "";
        $scope.Project = "";
        $scope.pageSize = 10;
        $scope.Options = [];
        $scope.hasPermissionProject = false;
        HasAccess();
        $scope.ExecutionGroupsArray = [];
        function GetProject() {
            ProjectService.Get(projectId, r => {
                $scope.Name = r.Name;
            }, error => {

            });
        }
        $scope.GroupsDetails = function () {
            $state.go('executionGroups', { projectId: projectId });
        }

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
                    GetInactiveGroups();
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




        $scope.EnableGroup = function () {
            ExecutionGroupService.Enable($scope.EnabledId, result => {
                ngToast.success({
                    content: 'Execution Group enabled successfully'
                });


                GetInactiveGroups();
            }, error => {
                ngToast.danger({
                    content: 'Execution Group could not be enabled'
                });
            });
        }
        $scope.SelectName = "";
        $scope.selectGroup = function (group) {
            $scope.EnabledId = group.Execution_Group_Id;
            $scope.SelectName = group.Name;
        }

        function GetInactiveGroups() {
            ExecutionGroupService.GetByProjectInactives(projectId, result => {
                $scope.ExecutionGroupsArray = result;
            }, error => {

            });
        }
    }]);

})();