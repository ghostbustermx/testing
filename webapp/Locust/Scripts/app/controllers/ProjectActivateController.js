(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('ProjectActivateController', ['$scope', '$state', '$window', 'AccessService', 'ProjectService', 'ModalConfirmService', 'ngToast', function ($scope, $state, $window, AccessService, ProjectService, ModalConfirmService, ngToast) {
        $scope.currentPage = 1;
        $scope.pageSize = 10;
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.projects = [];
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
            AccessService.HasAccessProjectSection(function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    GetPaginationElements();
                    GetAllProjects();
                }
            }, function (error) {
            });
        };

        function GetAllProjects() {
            ProjectService.GetInactives(function (data) {
                $scope.projects = data;
            }, function (error) {
            });
        }

        $scope.projSelected = function (proj) {
            $scope.pselected = proj;
        }

        $scope.redirectTo = function (url) {
            $state.go('Projects');
        };

        $scope.projectDetails = function (id) {
            $state.go('projectDetails', { id: id });
        }

        function GetActives() {
            $state.go('Projects');
        };
        $scope.showModalToEnable = function (project) {

            ProjectService.Enable(project.Id, function (data) {
                GetAllProjects();

                ngToast.success({
                    content: 'The project has been enabled successfully.'
                });
            });
        }
    }])
})();