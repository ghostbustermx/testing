(function () {
    'use strict';
    var app = angular.module(appName);
    app.controller('TestCaseDetailsController', ['$scope', 'DateParse', '$state', 'AccessService', 'TestCaseService', 'StepService', 'TagService', 'ModalConfirmService', 'ngToast', 'AttachmentService','$http', function ($scope, DateParse, $state, AccessService, TestCaseService, StepService, TagService, ModalConfirmService, ngToast, AttachmentService,$http) {
        $scope.action = $state.current.data.action;
        var testCaseId = $state.params.id;
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
            AccessService.HasAccess('TC', projectId, reqId, testCaseId, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    AccessService.HasAccess('TC_Details', projectId, reqId, testCaseId, 0, 0, 0, function (data2) {
                        if (!data2.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            GetTestCase();
                            GetTags();
                        }
                    }, function (error) {
                    });
                }
            }, function (error) {
            });
        };

        function GetTestCase() {
            TestCaseService.Get(testCaseId, function (data) {
                $scope.id = testCaseId;
                $scope.tcNumber = data.tc_number;
                $scope.testPriority = data.Test_Priority;
                $scope.title = data.Title;
                $scope.description = data.Description;
                $scope.preconditions = data.Preconditions;
                $scope.creator = data.Test_Case_Creator;
                $scope.expected = data.Expected_Result;
                $scope.creation_date = data.Creation_Date;
                $scope.status = data.Status;
                $scope.lastEditor = data.Last_Editor;
                $scope.type = data.Type;

                $scope.creation_date = DateParse.GetDate(data.Creation_Date);

                if ($scope.lastEditor == "") {
                    $scope.lastEditor = null;
                }

                GetAllSteps(testCaseId);
                GetProject(testCaseId);
                GetRequirement(reqId);
                AttachmentService.GetAttachment(3, testCaseId, function (data) {
                    $scope.FilestoDisplay = data;
                }, function (error) {

                });
            }, function (error) {

            });
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

        $scope.TestCaseChangelog = function () {
            $state.go('testCaseChangeLog', { projectId: projectId, id: testCaseId, reqId: reqId });
        }

        function GetProject(testCaseId) {
            TestCaseService.GetProject(testCaseId, function (data) {
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