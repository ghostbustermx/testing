(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TestProcedureActivateController', ['$scope', '$state', '$window', 'AccessService', 'RequirementService', 'TestProcedureService', 'ModalConfirmService', 'ngToast', 'ProcedureSuplementalService', function ($scope, $state, $window, AccessService, RequirementService, TestProcedureService, ModalConfirmService, ngToast,ProcedureSuplementalService) {
        $scope.currentPageTP = 1;
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
                    GetAllTestProcedure();
                    GetRole();
                    GetProjectName(reqId);
                    GetRequirement(reqId);
                }
            }, function (error) {
            });
        };

        function GetAllTestProcedure() {
            RequirementService.GetAllTestProcedureInactives(reqId, function (data) {
                $scope.testProcedures = data;
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

        $scope.testprocedureSelected = function (tp) {
            $scope.tpselected = tp;
        }

        $scope.testprocedureDetails = function (id) {
            $state.go('testprocedureDetails', { projectId: projectId, id: id, reqId: reqId });
        }

        $scope.NavigateTestProcedureEdit = function (idtp) {
            $state.go('testprocedureEdit', { projectId: projectId, reqId: reqId, id: idtp });

        }

        $scope.showModalToEnable = function (tp) {

            TestProcedureService.Enable(tp.Test_Procedure_Id, function (data) {
                ProcedureSuplementalService.EnableForTp(tp.Test_Procedure_Id, function (success) {
                }, function (error) {
                });
                GetAllTestProcedure();
                ngToast.success({
                    content: 'The test procedure has been enabled successfully'
                });
            });

        }

        function GetProjectName(reqId) {
            TestProcedureService.GetProjectRequirement(reqId, function (data) {
                $scope.projectName = data.Name;
            });
        }

        function GetRequirement(reqId) {
            TestProcedureService.GetRequirement(reqId, function (data) {
                $scope.requirementId = data.req_number;
            });
        }

    }])
})();