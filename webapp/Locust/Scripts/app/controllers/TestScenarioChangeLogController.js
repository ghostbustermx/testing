(function () {
    'use strict';
    var app = angular.module(appName);
    app.controller('TestScenarioChangeLogController', ['$scope', 'DateParse', '$state', 'AccessService', 'TestScenarioService', 'StepService', 'TagService', 'RequirementsTestService', 'RequirementService', 'ModalConfirmService', 'ngToast', function ($scope, DateParse, $state, AccessService, TestScenarioService, StepService, TagService, RequirementsTestService, RequirementService, ModalConfirmService, ngToast) {
        $scope.action = $state.current.data.action;
        var testScenarioId = $state.params.id;
        var reqId = $state.params.reqId;
        var projectId = $state.params.projectId;
        $scope.CurrentUserRole = "";
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.steps = [];
        $scope.tags = [];
        $scope.projectId = 0;
        $scope.tagsAux = [];
        $scope.requirementId = 0;


        function HasAccess() {
            AccessService.HasAccess('TS', projectId, reqId, 0, 0, 0, testScenarioId, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    AccessService.HasAccess('TS_Details', projectId, reqId, 0, 0, 0, testScenarioId, function (data2) {
                        if (!data2.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            GetTestScenario();
                            GetRole();
                        }
                    }, function (error) {
                    });
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

        function GetTestScenario() {
            TestScenarioService.Get(testScenarioId, function (data) {
                $scope.id = testScenarioId;
                $scope.tsNumber = data.ts_number;
                $scope.testPriority = data.Test_Priority;
                $scope.title = data.Title;
                $scope.description = data.Description;
                $scope.preconditions = data.Preconditions;
                $scope.creator = data.Test_Scenario_Creator;
                $scope.creation_date = DateParse.GetDate(data.Creation_Date);
                $scope.Type = data.Type;
                $scope.status = data.Status;
                $scope.lastEditor = data.Last_Editor;
                if ($scope.lastEditor == "") {
                    $scope.lastEditor = null;
                }
                if ($scope.status == true) {
                    $scope.class = 'label-success'
                    $scope.active = 'Active'
                } else {
                    $scope.class = 'label-danger'
                    $scope.active = 'Inactive'
                }
                GetAllSteps(testScenarioId);
                GetProject(testScenarioId);
                GetRequirement(reqId);

                GetVersions(testScenarioId);
                GetAllRelations(testScenarioId);
            }, function (error) {

            });
        }

        $scope.tsDetails = function () {
            $state.go('testscenarioDetails', { projectId: projectId, id: testScenarioId, reqId: reqId });
        }

        function GetVersions(id) {
            TestScenarioService.TestScenarioChangeLogs(id, function (data) {
                $scope.cl = [];
                $scope.stepVersion = [];
                $scope.tagVersion = [];
                $scope.reqVersion = [];
                $scope.changeLogs = data;

                $scope.changeLogs.forEach(function (cl) {
                    if (cl.Active == false) {
                        var content = cl.Content;
                        var test_procedure = angular.fromJson(content.substring(content.indexOf("!&&&@@!TestScenario:!&&&@@!") + 27, content.indexOf("!&&&@@!Steps:!&&&@@!")));
                        test_procedure.Creation_Date = DateParse.GetDate(test_procedure.Creation_Date);
                        var steps = angular.fromJson(content.substring(content.indexOf("!&&&@@!Steps:!&&&@@!") + 20, content.indexOf("!&&&@@!Tags:!&&&@@!")));
                        var tags = angular.fromJson(content.substring(content.indexOf("!&&&@@!Tags:!&&&@@!") + 19, content.indexOf("!&&&@@!Req:!&&&@@!")));
                        var req = angular.fromJson(content.substring(content.indexOf("!&&&@@!Req:!&&&@@!") + 18, content.indexOf("!&&&@@!Stp:!&&&@@!")));
                        test_procedure.User = cl.User;
                        test_procedure.Version = cl.Version;
                        if (test_procedure.Status == true) {
                            test_procedure.class = 'label-success'
                            test_procedure.active = 'Active'
                        } else {
                            test_procedure.class = 'label-danger'
                            test_procedure.active = 'Inactive'
                        }
                        $scope.cl.push(test_procedure);

                        steps.forEach(function (st) {
                            st.Version = cl.Version;
                            $scope.stepVersion.push(st);
                        });

                        tags.forEach(function (t) {
                            t.Version = cl.Version;
                            $scope.tagVersion.push(t);
                        });

                        req.forEach(function (r) {
                            r.Version = cl.Version;
                            $scope.reqVersion.push(r);
                        });
                    }
                });
            });
        }

        function GetAllSteps(id) {
            StepService.GetForTestScenario(id, function (data) {
                $scope.steps = data;
                TagService.GetTestScenarioTags(id, function (tags) {
                    $scope.tags = tags;
                }, function (error) {
                });
            }, function (error) {

            });
        }


        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: $scope.projectId });
        }

        $scope.requirementDetails = function () {
            $state.go('requirementDetails', { projectId: projectId, id: $scope.requirementId });
        }

        $scope.redirectToList = function () {

            $state.go('requirementDetails', { projectId: projectId, id: $scope.requirementId });
        }

        function GetProject(testScenarioId) {
            TestScenarioService.GetProject(testScenarioId, function (data) {
                $scope.projectName = data.Name;
                $scope.projectId = data.Id;
            });
        }

        function GetRequirement(reqId) {
            TestScenarioService.GetRequirement(reqId, function (data) {
                $scope.requirementName = data.req_number;
                $scope.requirementId = data.Id;
            });
        }

        $scope.clSelected = function (cl, tagsArr) {
            $scope.clselected = cl;
            $scope.tagsAux = [];
            tagsArr.forEach(function (tags) {
                if (tags.Version == $scope.clselected.Version) {
                    $scope.tagsAux.push(tags)
                }
            });
        }

        function GetAllRelations(id) {
            $scope.requirementsAssigned = [];
            RequirementsTestService.GetTestScenarioRelations(id, function (data) {
                data.forEach(function (relation) {
                    RequirementService.Get(relation.Requirement_Id, function (data1) {
                        $scope.requirementsAssigned.push(data1);
                    });
                });
            });
        }

        $scope.restore = function (change_log) {
            var selected;
            $scope.changeLogs.forEach(function (cl) {
                if (cl.Version == change_log.Version) {
                    selected = cl;
                }
            });
            TestScenarioService.Restore(selected, function (res) {
                ngToast.success({
                    content: 'Test Scenario restored successfully'
                })
                GetTestScenario();
            });

        }
    }])
})();