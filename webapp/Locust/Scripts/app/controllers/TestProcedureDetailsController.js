(function () {
    'use strict';
    var app = angular.module(appName);
    app.controller('TestProcedureDetailsController', ['$scope', 'DateParse', '$state', 'AccessService', 'TestProcedureService', 'TestCaseService', 'TestSuplementalService', 'StepService', 'TagService', 'ModalConfirmService', 'ngToast', 'AttachmentService', '$http', function ($scope, DateParse, $state, AccessService, TestProcedureService, TestCaseService, TestSuplementalService, StepService, TagService, ModalConfirmService, ngToast, AttachmentService, $http) {
        $scope.action = $state.current.data.action;
        var testProcedureId = $state.params.id;
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
        $scope.tagsSupList = [];
        $scope.suplementals = [];
        $scope.FilestoDisplay = [];
        function HasAccess() {
            AccessService.HasAccess('TP', projectId, reqId, 0, testProcedureId, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    AccessService.HasAccess('TP_Details', projectId, reqId, 0, testProcedureId, 0, 0, function (data2) {
                        if (!data2.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            GetTestProcedure();
                            GetTags();
                        }
                    }, function (error) {
                    });
                }
            }, function (error) {
            });
        };

        function GetTestProcedure() {
            TestProcedureService.Get(testProcedureId, function (data) {
                $scope.id = testProcedureId;
                $scope.tpNumber = data.tp_number;
                $scope.testPriority = data.Test_Priority;
                $scope.title = data.Title;
                $scope.description = data.Description;
                $scope.creator = data.Test_Procedure_Creator;
                $scope.expected = data.Expected_Result;
                $scope.creation_date = data.Creation_Date;
                $scope.status = data.Status;
                $scope.type = data.Type;
                $scope.lastEditor = data.Last_Editor;
                $scope.creation_date = DateParse.GetDate(data.Creation_Date);

                if ($scope.lastEditor == "") {
                    $scope.lastEditor = null;
                }

                $scope.note = data.Note;
                if (data.Test_Case_Id != null) {
                    GetTestCase(data.Test_Case_Id);
                }
                GetAllSteps(testProcedureId);
                GetProject(testProcedureId);
                GetRequirement(reqId);
                AttachmentService.GetAttachment(5, testProcedureId, function (data) {
                    $scope.FilestoDisplay = data;
                }, function (error) {

                });
            }, function (error) {

            });
        }

        function GetAllSteps(id) {
            StepService.GetForTestProcedure(id, function (data) {
                $scope.steps = data;
                TagService.GetTestProcedureTags(id, function (tags) {
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

        function GetTestCase(id) {
            TestCaseService.Get(id, function (data) {
                $scope.testcase = data.Title;
            });
        }

        $scope.testProcedureChangelog = function () {
            $state.go('testProcedureChangeLog', { projectId: projectId, id: testProcedureId, reqId: reqId });
        }

        $scope.redirectToList = function () {

            $state.go('requirementDetails', { projectId: projectId, id: $scope.requirementId });
        }

        function GetProject(testProcedureId) {
            TestProcedureService.GetProject(testProcedureId, function (data) {
                $scope.projectName = data.Name;
                $scope.projectId = data.Id;
                TestSuplementalService.GetForProject($scope.projectId, function (res) {
                    $scope.suplementals = [];
                    $scope.suplementals = res;
                });
            });
        }

        function GetRequirement(reqId) {
            TestProcedureService.GetRequirement(reqId, function (data) {
                $scope.requirementName = data.req_number;
                $scope.requirementId = data.Id;
            });
        }

        $scope.goStp = function (step) {
            var string = step.action.substring(step.action.indexOf("STP_"), step.action.length);
            $scope.suplementals.forEach(function (sup) {
                if (sup.stp_number == string.trim()) {
                    var url = $state.href('suplementalDetails', { projectId: projectId, id: sup.Test_Suplemental_Id });
                    window.open(url, '_blank', { focus: true });
                }
            });
        }

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

        function GetTags() {
            TagService.GetAll(function (data) {
                $scope.tags = data;
            }, function (error) {
            });
        };

    }])
})();