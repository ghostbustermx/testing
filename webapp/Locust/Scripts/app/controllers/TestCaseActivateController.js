(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TestCaseActivateController', ['$scope', '$state', '$window', 'RequirementService', 'AccessService', 'TestCaseService', 'ModalConfirmService', 'ngToast', function ($scope, $state, $window, RequirementService, AccessService, TestCaseService, ModalConfirmService, ngToast) {
        $scope.currentPageTC = 1;
        $scope.pageSize = 10;
        var reqId = $state.params.reqId;
        var projectId = $state.params.projectId;
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.requirements = [];
        $scope.id = reqId;
        $scope.CurrentUserRole = "";
        $scope.Options = [];

        function GetRole() {
            AccessService.GetRole(function (data) {
                $scope.CurrentUserRole = data.Role;
            }, function (error) {
                $scope.CurrentUserRole = null;
            });
        }

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
            AccessService.HasAccess('Requirement', projectId, reqId, 0, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    GetPaginationElements();
                    GetRole();
                    GetAllTestCase();
                    GetProjectName(reqId);
                    GetRequirement(reqId);
                }
            }, function (error) {
            });
        };

        function GetAllTestCase() {
            RequirementService.GetAllTestCaseInactives(reqId, function (data) {
                $scope.testCases = data;
            }, function (error) {
            });
        }

        $scope.reqSelected = function (req) {
            $scope.rselected = req;
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        }

        $scope.requirementDetails = function () {
            $state.go('requirementDetails', { projectId: projectId, id: reqId });
        }

        $scope.testcaseSelected = function (tc) {
            $scope.tcselected = tc;
        }

        $scope.testcaseDetails = function (id) {
            $state.go('testcaseDetails', { projectId: projectId, reqId: reqId, id: id })
        }

        $scope.testcaseEdit = function (id) {
            $state.go('testcaseEdit', { projectId: projectId, reqId: reqId, id: id })
        }

        $scope.showModalToEnable = function (tc) {
            TestCaseService.Enable(tc.Test_Case_Id, function (data) {
                GetAllTestCase();
                ngToast.success({
                    content: 'The test case has been enabled successfully'
                });
            });
        }

        function GetProjectName(reqId) {
            TestCaseService.GetProjectRequirement(reqId, function (data) {
                $scope.projectName = data.Name;
            });
        }

        function GetRequirement(reqId) {
            TestCaseService.GetRequirement(reqId, function (data) {
                $scope.requirementId = data.req_number;
            });
        }

    }])
})();