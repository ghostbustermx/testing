(function () {
    'use strict';
    var app = angular.module(appName);
    app.controller('TestSuplementalChangeLogController', ['$scope', 'DateParse', '$state', 'AccessService', 'TestSuplementalService', 'StepService', 'TagService', 'ProjectService', 'ModalConfirmService', 'ngToast', function ($scope, DateParse, $state, AccessService, TestSuplementalService, StepService, TagService, ProjectService, ModalConfirmService, ngToast) {
        $scope.action = $state.current.data.action;
        var testsuplementalId = $state.params.id;
        var projectId = $state.params.projectId;
        HasAccess();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.CurrentUserRole = "";
        $scope.steps = [];
        $scope.tags = [];
        $scope.projectId = 0;
        $scope.tagsAux = [];

        function HasAccess() {
            AccessService.HasAccess('STP', projectId, 0, 0, 0, testsuplementalId, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    AccessService.HasAccess('STP_details', projectId, 0, 0, 0, testsuplementalId, 0, function (data2) {
                        if (!data2.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            GetTestSuplemental();
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

        function GetTestSuplemental() {
            TestSuplementalService.Get(testsuplementalId, function (data) {
                $scope.projectId = data.Project_Id;
                $scope.stpNumber = data.stp_number;
                $scope.title = data.Title;
                $scope.description = data.Description;
                $scope.creator = data.Test_Procedure_Creator;
                $scope.creation_date = DateParse.GetDate(data.Creation_Date);

                $scope.status = data.Status;
                $scope.lastEditor = data.Last_Editor;
                if ($scope.status == true) {
                    $scope.class = 'label-success'
                    $scope.active = 'Active'
                } else {
                    $scope.class = 'label-danger'
                    $scope.active = 'Inactive'
                }
                GetAllSteps(testsuplementalId);
                GetProject(testsuplementalId);

                GetVersions(testsuplementalId);
            }, function (error) {

            });
        }


        function GetVersions(id) {
            TestSuplementalService.TestSuplementalChangeLogs(id, function (data) {
                $scope.cl = [];
                $scope.stepVersion = [];
                $scope.tagVersion = [];
                $scope.reqVersion = [];
                $scope.changeLogs = data;

                $scope.changeLogs.forEach(function (cl) {
                    if (cl.Active == false) {
                        var content = cl.Content;
                        var test_suplemental = angular.fromJson(content.substring(content.indexOf("!&&&@@!TestSuplemental:!&&&@@!") + 30, content.indexOf("!&&&@@!Steps:!&&&@@!")));
                        test_suplemental.Creation_Date = DateParse.GetDate(test_suplemental.Creation_Date);

                        var steps = angular.fromJson(content.substring(content.indexOf("!&&&@@!Steps:!&&&@@!") + 20, content.indexOf("!&&&@@!Tags:!&&&@@!")));
                        var tags = angular.fromJson(content.substring(content.indexOf("!&&&@@!Tags:!&&&@@!") + 19));
                        test_suplemental.User = cl.User;
                        test_suplemental.Version = cl.Version;
                        if (test_suplemental.Status == true) {
                            test_suplemental.class = 'label-success'
                            test_suplemental.active = 'Active'
                        } else {
                            test_suplemental.class = 'label-danger'
                            test_suplemental.active = 'Inactive'
                        }
                        $scope.cl.push(test_suplemental);

                        steps.forEach(function (st) {
                            st.Version = cl.Version;
                            $scope.stepVersion.push(st);
                        });

                        tags.forEach(function (t) {
                            t.Version = cl.Version;
                            $scope.tagVersion.push(t);
                        });

                    }
                });
            });
        }

        $scope.stpDetails = function () {
            $state.go('suplementalDetails', { projectId: projectId, id: testsuplementalId });
        }

        function GetAllSteps(id) {
            StepService.GetForTestSuplemental(id, function (data) {
                $scope.steps = data;
                TagService.GetTestSuplementalTags(id, function (tags) {
                    $scope.tags = tags;
                }, function (error) {
                });
            }, function (error) {

            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: $scope.projectId });
        }

        $scope.redirectToList = function () {
            $state.go('projectDetails', { id: $scope.projectId });
        }

        function GetProject() {
            ProjectService.Get($scope.projectId, function (data) {
                $scope.projectName = data.Name;
                $scope.projectId = data.Id;
            });
        }

        $scope.clSelected = function (cl, tagsArr) {
            $scope.clselected = cl;
            $scope.tagsAux = [];
            tagsArr.forEach(function (tags) {
                if (tags.Version == $scope.clselected.Version) {
                    $scope.tagsAux.push(tags)
                }
            })
        }

        $scope.restore = function (change_log) {
            var selected;
            $scope.changeLogs.forEach(function (cl) {
                if (cl.Version == change_log.Version) {
                    selected = cl;
                }
            });
            TestSuplementalService.Restore(selected, function (res) {
                ngToast.success({
                    content: 'Test supplemental restored successfully'
                })
                GetTestSuplemental();
            });
        }
    }])
})();