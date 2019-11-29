(function () {
    'use strict';
    var app = angular.module(appName);
    app.controller('TestScenarioDetailsController', ['$scope', 'DateParse', '$state', 'AccessService', 'TestScenarioService', 'TestSuplementalService', 'StepService', 'TagService', 'ModalConfirmService', 'ngToast', 'AttachmentService', '$http', function ($scope, DateParse, $state, AccessService, TestScenarioService, TestSuplementalService, StepService, TagService, ModalConfirmService, ngToast, AttachmentService, $http) {
        $scope.action = $state.current.data.action;
        var testScenarioId = $state.params.id;
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
        $scope.FilestoDisplay = [];
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
                            GetTestCase();
                        }
                    }, function (error) {
                    });
                }
            }, function (error) {
            });
        };


        function GetTestCase() {
            TestScenarioService.Get(testScenarioId, function (data) {
                $scope.id = testScenarioId;
                $scope.tsNumber = data.ts_number;
                $scope.testPriority = data.Test_Priority;
                $scope.title = data.Title;
                $scope.description = data.Description;
                $scope.preconditions = data.Preconditions;
                $scope.creator = data.Test_Scenario_Creator;
                $scope.creation_date = data.Creation_Date;
                $scope.status = data.Status;
                $scope.note = data.Note;
                $scope.lastEditor = data.Last_Editor;
                $scope.type = data.Type;
                $scope.creation_date = DateParse.GetDate(data.Creation_Date);


                if ($scope.lastEditor == "") {
                    $scope.lastEditor = null;
                }
                GetAllSteps(testScenarioId);
                GetProject(testScenarioId);
                GetRequirement(reqId);
                AttachmentService.GetAttachment(4, testScenarioId, function (data) {
                    $scope.FilestoDisplay = data;
                }, function (error) {

                });
            }, function (error) {

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

        $scope.testScenarioChangeLogs = function () {
            $state.go('testScenarioChangeLog', { projectId: projectId, id: testScenarioId, reqId: reqId });
        }

        function GetProject(testScenarioId) {
            TestScenarioService.GetProject(testScenarioId, function (data) {
                $scope.projectName = data.Name;
                $scope.projectId = data.Id;
                TestSuplementalService.GetForProject($scope.projectId, function (res) {
                    $scope.suplementals = [];
                    $scope.suplementals = res;
                });
            });
        }

        function GetRequirement(reqId) {
            TestScenarioService.GetRequirement(reqId, function (data) {
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
    }])
})();