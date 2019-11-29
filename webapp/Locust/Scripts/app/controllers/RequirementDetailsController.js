(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('RequirementDetailsController', ['$scope', '$state', 'RequirementService', 'AccessService', 'TestCaseService', 'TestScenarioService', 'TestProcedureService', 'ProjectService', 'ModalConfirmService', 'ngToast', 'ProcedureSuplementalService', 'AttachmentService','$http' ,function ($scope, $state, RequirementService, AccessService, TestCaseService, TestScenarioService, TestProcedureService, ProjectService, ModalConfirmService, ngToast, ProcedureSuplementalService, AttachmentService,$http) {
        $scope.action = $state.current.data.action;
        var requirementId = $state.params.id;
        var projectId = $state.params.projectId;
        HasAccess();
        $scope.currentPageTC = 1;
        $scope.currentPageTP = 1;
        $scope.currentPageTS = 1;
        $scope.pageSizeTC = 10;
        $scope.pageSizeTP = 10;
        $scope.pageSizeTS = 10;
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.testCases = [];

        $scope.project_id;
        $scope.CurrentUserRole = "";
        $scope.FilestoDisplay = [];

        $scope.Options = [];

        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelectedTC = $scope.Options[0];
                $scope.RowsSelectedTP = $scope.Options[0];
                $scope.RowsSelectedTS = $scope.Options[0];

            }, function (error) {
            });
        }

        $scope.changeValueTC = function (value) {
            $scope.pageSizeTC = value;
        }
        $scope.changeValueTP = function (value) {
            $scope.pageSizeTP = value;
        }
        $scope.changeValueTS = function (value) {
            $scope.pageSizeTS = value;
        }



        function HasAccess() {
            AccessService.HasAccess('Requirement', projectId, requirementId, 0, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    GetPaginationElements();
                    GetRequirement();
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

        function GetRequirement() {
            RequirementService.Get(requirementId, function (data) {
                $scope.project_id = data.Project_Id;
                $scope.axosoft_Task_Id = data.Axosoft_Task_Id;
                $scope.name = data.Name;
                $scope.id = requirementId;
                $scope.description = data.Description;
                $scope.acceptanceCriteria = data.Acceptance_Criteria;
                $scope.req_number = data.req_number;
                $scope.developerAssigned = data.Developer_Assigned;
                $scope.testerAssigned = data.Tester_Assigned;
                AttachmentService.GetAttachment(1,requirementId, function (data) {
                    $scope.FilestoDisplay = data;
                }, function (error) {

                });
                if (data.Status == true) {
                    $scope.status = 'Active';
                    $scope.class = 'label-success';
                } else {
                    $scope.status = 'Inactive';
                    $scope.class = 'label-danger';
                }
                GetAllTestCase(requirementId);
                GetAllTestScenario(requirementId);
                GetAllTestProcedure(requirementId);
                GetProject(data.Project_Id);
            }, function (error) {

            });
        }

        function GetProject(id) {
            ProjectService.Get(id, function (data) {
                $scope.Project_Id = data.Name;
            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: $scope.project_id });
        };
        //TC
        $scope.NavigateTestCaseAdd = function () {
            $state.go('testcaseAdd', { projectId: projectId, reqId: requirementId });
        }

        $scope.NavigateTestCaseEdit = function (idtc) {
            $state.go('testcaseEdit', { projectId: projectId, reqId: requirementId, id: idtc });

        }

        $scope.NavigateTestCaseActivate = function () {
            $state.go('testCaseActivate', { projectId: projectId, reqId: requirementId });
        }
        //TS
        $scope.NavigateTestScenarioAdd = function () {
            $state.go('testscenarioAdd', { projectId: projectId, reqId: requirementId });
        }

        $scope.NavigateTestScenarioEdit = function (idts) {
            $state.go('testscenarioEdit', { projectId: projectId, reqId: requirementId, id: idts });

        }

        $scope.NavigateTestScenarioActivate = function () {
            $state.go('testScenarioActivate', { projectId: projectId, reqId: requirementId });
        }
        //TP
        $scope.NavigateTestProcedureAdd = function () {
            $state.go('testprocedureAdd', { projectId: projectId, reqId: requirementId });
        }

        $scope.NavigateTestProcedureEdit = function (idtp) {
            $state.go('testprocedureEdit', { projectId: projectId, reqId: requirementId, id: idtp });

        }

        $scope.NavigateTestProcedureActivate = function () {
            $state.go('testProcedureActivate', { projectId: projectId, reqId: requirementId });
        }



        //TA
        $scope.NavigateTestAutomatedAdd = function () {
            $state.go('testAutomatedAdd', { projectId: projectId, reqId: requirementId });
        }

        $scope.NavigateTestAutomatedEdit = function (idtp) {
            $state.go('testAutomatedEdit', { projectId: projectId, reqId: requirementId, id: idtp });

        }

        $scope.NavigateTestProcedureActivate = function () {
            $state.go('testProcedureActivate', { projectId: projectId, reqId: requirementId });
        }

        $scope.testscenarioSelected = function (ts) {
            $scope.tsselected = ts;
        }

        $scope.testcaseSelected = function (tc) {
            $scope.tcselected = tc;
        }

        $scope.rowSelected = function (row, reqId, type) {
            var url = "";
            switch (type) {
                case "TC":
                    url = $state.href('testcaseAddCopy', { projectId: projectId, id: row.Test_Case_Id, reqId: reqId });
                    break;
                case "TP":
                    url = $state.href('testprocedureAddCopy', { projectId: projectId, id: row.Test_Procedure_Id, reqId: reqId });
                    break;
                case "TS":
                    url = $state.href('testscenarioAddCopy', { projectId: projectId, id: row.Test_Scenario_Id, reqId: reqId });
                    break;
                case "TA":
                    url = $state.href('testAutomatedAddCopy', { projectId: projectId, id: row.Test_Procedure_Id, reqId: reqId });
                    break;
                default:
            }
            window.open(url, '_parent', { focus: true });
        }

        $scope.testprocedureSelected = function (tp) {
            $scope.tpselected = tp;
        }

        $scope.showModalToDelete = function (testCase) {
            TestCaseService.Delete(testCase.Test_Case_Id, function (data) {
                GetRequirement();
            });
            TestCaseService.AddChangeLog(testCase.Test_Case_Id, function (data) {
            });
            ngToast.success({
                content: 'The testcase has been disabled successfully.'
            });
        }

        $scope.showModalToDeleteScenario = function (testScenario) {
            TestScenarioService.Delete(testScenario.Test_Scenario_Id, function (data) {
                ProcedureSuplementalService.DisableForTs(testScenario.Test_Scenario_Id, function (data) {
                }, function (error) {
                });
                GetRequirement();
                ngToast.success({
                    content: 'The test scenario has been disabled successfully'
                })
                TestScenarioService.AddChangeLog(testScenario.Test_Procedure_Id, function (data) {
                });

               
            });
           ;
        }

        $scope.showModalToDeleteProcedure = function (testProcedure) {
            TestProcedureService.Delete(testProcedure.Test_Procedure_Id, function (data) {
                ProcedureSuplementalService.DisableForTp(testProcedure.Test_Procedure_Id, function (data) {
                }, function (error) {
                });
                GetRequirement();
            });
            TestProcedureService.AddChangeLog(testProcedure.Test_Procedure_Id, function (data) {
            });
            ngToast.success({
                content: 'The test procedure has been disabled successfully.'
            });
        }

        $scope.testcaseDetails = function (tcid, reqid) {
            $state.go('testcaseDetails', { projectId: projectId, id: tcid, reqId: reqid });
        };

        $scope.testscenarioDetails = function (tsid, reqid) {
            $state.go('testscenarioDetails', { projectId: projectId, id: tsid, reqId: reqid });
        };

        $scope.testprocedureDetails = function (tpid, reqid) {
            $state.go('testprocedureDetails', { projectId: projectId, id: tpid, reqId: reqid });
        };

        $scope.requirementChangelog = function () {
            $state.go('requirementChangeLog', { id: requirementId, projectId: projectId });
        };

        function GetAllTestCase(id) {
            RequirementService.GetAllTestCase(id, function (data) {

                $scope.testCases = data;

            }, function (error) {
            });
        }

        function GetAllTestScenario(id) {
            RequirementService.GetAllTestScenario(id, function (data) {
                $scope.testScenarios = data;
            }, function (error) {
            });
        }

        function GetAllTestProcedure(id) {
            RequirementService.GetAllTestProcedure(id, function (data) {
                $scope.testProcedures = data.filter(tp => tp.Type != "Automated");
                $scope.testProceduresAutomated = data.filter(tp => tp.Type == "Automated");
            }, function (error) {
                });


        }
        
        $scope.redirectToList = function () {
            $state.go('Projects');
        };

        $scope.DownloadFile = function (id) {
            var fileIndx = $scope.FilestoDisplay.findIndex(file => file.Id == id);
            var filename = $scope.FilestoDisplay[fileIndx].Name;
            $http({
                method: 'GET',
                url: '/api/Attachment/DownLoad',
                params: { EntityId: id, name: filename },
                responseType: 'arraybuffer'
            }).then(function (response) {
                var filename = $scope.FilestoDisplay[fileIndx].Name;
                var contentType = "application/octet-stream";
                var linkElement = document.createElement('a');
                try {
                    var blob = new Blob([response.data], { type: contentType });
                    var url = window.URL.createObjectURL(blob);

                    linkElement.setAttribute('href', url);
                    linkElement.setAttribute("download", filename);

                    var clickEvent = new MouseEvent("click", {
                        "view": window,
                        "bubbles": true,
                        "cancelable": false
                    });
                    linkElement.dispatchEvent(clickEvent);
                } catch (err) {

                }

            }, function (err) {

            });

        }

    }])
})();