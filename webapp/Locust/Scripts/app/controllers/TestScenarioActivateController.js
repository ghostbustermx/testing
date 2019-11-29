(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TestScenarioActivateController', ['$scope', '$state', '$window', 'AccessService', 'RequirementService', 'TestScenarioService', 'ModalConfirmService', 'ngToast', function ($scope, $state, $window, AccessService, RequirementService, TestScenarioService, ModalConfirmService, ngToast) {
        $scope.currentPageTS = 1;
        $scope.pageSize = 10;
        var reqId = $state.params.reqId;
        var projectId = $state.params.projectId;
        $scope.CurrentUserRole = "";
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.requirements = [];
        $scope.id = reqId;

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
                    GetAllTestScenario();
                    GetRole();
                    GetProjectName(reqId);
                    GetRequirement(reqId);
                }
            }, function (error) {
            });
        };


        function GetAllTestScenario() {
            RequirementService.GetAllTestScenarioInactives(reqId, function (data) {
                $scope.testScenarios = data;
            }, function (error) {
            });
        }

        $scope.reqSelected = function (req) {
            $scope.rselected = req;
        }

        $scope.projectDetails = function (id) {
            $state.go('projectDetails', { id: projectId });
        }

        $scope.requirementDetails = function (id) {
            $state.go('requirementDetails', { projectId: projectId, id: reqId });
        }

        $scope.NavigateTestScenarioEdit = function (idts) {
            $state.go('testscenarioEdit', { projectId: projectId, reqId: reqId, id: idts });

        }
        $scope.testscenarioDetails = function (tsid) {
            $state.go('testscenarioDetails', { projectId: projectId, id: tsid, reqId: reqId });
        };

        $scope.testscenarioSelected = function (ts) {
            $scope.tsselected = ts;
        }

        $scope.showModalToEnable = function (ts) {
            TestScenarioService.Enable(ts.Test_Scenario_Id, function (data) {
                GetAllTestScenario();
                ngToast.success({
                    content: 'The test scenario has been enabled successfully'
                });
            });
        }

        function GetProjectName(reqId) {
            TestScenarioService.GetProjectRequirement(reqId, function (data) {
                $scope.projectName = data.Name;
            });
        }

        function GetRequirement(reqId) {
            TestScenarioService.GetRequirement(reqId, function (data) {
                $scope.requirementId = data.req_number;
            });
        }
    }])
})();