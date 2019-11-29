(function () {
    'use strict';
    var app = angular.module(appName);
    app.controller('TestSuplementalDetailsController', ['$scope', 'DateParse', '$state', 'AccessService', 'TestSuplementalService', 'StepService', 'TagService', 'ProjectService', 'ModalConfirmService', 'ngToast', 'AttachmentService', '$http', function ($scope, DateParse, $state, AccessService, TestSuplementalService, StepService, TagService, ProjectService, ModalConfirmService, ngToast, AttachmentService, $http) {
        $scope.action = $state.current.data.action;
        var testSuplementalId = $state.params.id;
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
        $scope.FilestoDisplay = [];

        function HasAccess() {
            AccessService.HasAccess('Project', projectId, 0, 0, 0, 0, 0, function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                }
                else {
                    AccessService.HasAccess('STP_Details', projectId, 0, 0, 0, testSuplementalId, 0, function (data2) {
                        if (!data2.HasPermission) {
                            $state.go('Projects').then(function () {
                                ngToast.danger({
                                    content: 'You do not have access to the requested page.'
                                });
                            });
                        } else {
                            GetTestSuplemental();
                        }
                    }, function (error) {
                    });
                }
            }, function (error) {
            });
        };

        function GetTestSuplemental() {
            TestSuplementalService.Get(testSuplementalId, function (data) {
                $scope.id = testSuplementalId;
                $scope.stpNumber = data.stp_number;
                $scope.title = data.Title;
                $scope.description = data.Description;
                $scope.creator = data.Test_Procedure_Creator;
                $scope.creation_date = data.Creation_Date;
                $scope.projectId = data.Project_Id;
                $scope.status = data.Status;
                $scope.lastEditor = data.Last_Editor;
                $scope.creation_date = DateParse.GetDate(data.Creation_Date);
                if ($scope.lastEditor == "") {
                    $scope.lastEditor = null;
                }
                GetAllSteps(testSuplementalId);
                GetProject($scope.projectId);

                AttachmentService.GetAttachment(2, testSuplementalId, function (data) {
                    $scope.FilestoDisplay = data;
                }, function (error) {

                });

            }, function (error) {
            });
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

        $scope.supplementalChangelog = function () {
            $state.go('testSuplementalChangeLog', { id: testSuplementalId, projectId: projectId });
        }

        function GetProject(id) {
            ProjectService.Get(id, function (data) {
                $scope.projectName = data.Name;
                $scope.projectId = data.Id;
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