(function () {
    'use strict';
    var app = angular.module(appName);
    app.controller('TestCaseChangeLogController', ['$scope', 'DateParse', '$state', 'TestCaseService', 'AccessService', 'StepService', 'TagService', 'RequirementsTestService', 'RequirementService', 'ModalConfirmService', 'ngToast', function ($scope, DateParse, $state, TestCaseService, AccessService, StepService, TagService, RequirementsTestService, RequirementService, ModalConfirmService, ngToast) {
        $scope.action = $state.current.data.action;
        var testcaseId = $state.params.id;
        var reqId = $state.params.reqId;
        var projectId = $state.params.projectId;
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.steps = [];
        $scope.tags = [];
        $scope.projectId = 0;
        $scope.requirementId = 0;
        $scope.tagsAux = [];
        $scope.CurrentUserRole = "";

        function HasAccess() {
            AccessService.HasAccess('TC', projectId, reqId, testcaseId, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    AccessService.HasAccess('TC_Details', projectId, reqId, testcaseId, 0, 0, 0, function (data2) {
                        if (!data2.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            GetTestCase();
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




        function GetTestCase() {
            var req = $state.params.reqId;
            TestCaseService.Get(testcaseId, function (data) {
                $scope.tcNumber = data.tc_number;
                $scope.testPriority = data.Test_Priority;
                $scope.title = data.Title;
                $scope.description = data.Description;
                $scope.preconditions = data.Preconditions;
                $scope.creator = data.Test_Case_Creator;
                $scope.expected = data.Expected_Result;
                $scope.type = data.Type;
                $scope.creation_date = DateParse.GetDate(data.Creation_Date);

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
                GetAllSteps(testcaseId);
                GetProject(testcaseId);
                GetRequirement(req);
                GetVersions(testcaseId);
                GetAllRelations(testcaseId);
            }, function (error) {

            });
        }


        function GetVersions(id) {
            TestCaseService.TestCaseChangeLogs(id, function (data) {
                $scope.cl = [];
                $scope.stepVersion = [];
                $scope.tagVersion = [];
                $scope.reqVersion = [];
                $scope.changeLogs = data;

                $scope.changeLogs.forEach(function (cl) {
                    if (cl.Active == false) {
                        var content = cl.Content;
                        var test_case = angular.fromJson(content.substring(content.indexOf("!&&&@@!TestCase:!&&&@@!") + 23, content.indexOf("!&&&@@!Steps:!&&&@@!")));
                        test_case.Creation_Date = DateParse.GetDate(test_case.Creation_Date);
                        var steps = angular.fromJson(content.substring(content.indexOf("!&&&@@!Steps:!&&&@@!") + 20, content.indexOf("!&&&@@!Tags:!&&&@@!")));
                        var tags = angular.fromJson(content.substring(content.indexOf("!&&&@@!Tags:!&&&@@!") + 19, content.indexOf("!&&&@@!Req:!&&&@@!")));
                        var req = angular.fromJson(content.substring(content.indexOf("!&&&@@!Req:!&&&@@!") + 18));
                        test_case.User = cl.User;
                        test_case.Version = cl.Version;
                        if (test_case.Status == true) {
                            test_case.class = 'label-success'
                            test_case.active = 'Active'
                        } else {
                            test_case.class = 'label-danger'
                            test_case.active = 'Inactive'
                        }
                        $scope.cl.push(test_case);

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

        $scope.tcDetails = function () {
            $state.go('testcaseDetails', { projectId: projectId, id: testcaseId, reqId: reqId });
        }

        function GetAllSteps(id) {
            StepService.GetForTestCase(id, function (data) {
                $scope.steps = data;
                TagService.GetTestCaseTags(id, function (tags) {
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

        function GetProject(testcaseId) {
            TestCaseService.GetProject(testcaseId, function (data) {
                $scope.projectName = data.Name;
                $scope.projectId = data.Id;
            });
        }

        function GetRequirement(reqId) {
            TestCaseService.GetRequirement(reqId, function (data) {
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
            RequirementsTestService.GetTestCaseRelations(id, function (data) {
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
            TestCaseService.Restore(selected, function (res) {
                ngToast.success({
                    content: 'Test Case restored successfully'
                })
                GetTestCase();
            });

        }
    }])

})();