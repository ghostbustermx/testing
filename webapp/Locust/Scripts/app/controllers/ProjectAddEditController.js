(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('ProjectAddEditController', ['$scope', '$state', '$http', '$timeout', '$window', 'AccessService', 'ProjectService', 'ngToast', function ($scope, $state, $http, $timeout, $window, AccessService, ProjectService, ngToast) {
        $scope.action = $state.current.data.action;
        var projectId = $state.params.id;
        $scope.typesAuthentication = [];
        HasAccess();
        $scope.saved = false;

        function HasAccess() {
            AccessService.HasAccessProjectSection(function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    GetProject();
                }
            }, function (error) {
            });
        };

        function GetProject() {
            if ($scope.action == 'Edit') {
                ProjectService.Get(projectId, function (data) {
                    $scope.name = data.Name;
                    $scope.status = data.Status;
                    $scope.description = data.Description;
                    $scope.image = data.Image;
                    if ($scope.status == true) {
                        document.getElementById("switch1").checked = true;
                        document.getElementById("Status").value = true;
                    } else {
                        document.getElementById("switch1").checked = false;
                        document.getElementById("Status").value = false;
                    }
                }, function (error) {
                });
            }
        }
        $scope.cancel = function () {
            $scope.redirectToList();
        };

        $scope.onfileSelected = function (event) {
            $scope.selectedFiles = event.target.files;
            $scope.nameFile = event.target.files[0].name;
        };

        $scope.redirectToList = function () {
            $state.go('Projects');
        };

        $scope.AddUpdate = function (form) {

            if ($scope.selectedFiles != null) {
                if (isFileImage($scope.selectedFiles.item(0)) == false) {
                    ngToast.danger({
                        content: 'Not a valid image format'
                    });
                    return;
                }
            }

            $scope.saved = true;

            var status = document.getElementById("Status").innerHTML;
            if (document.getElementById("switch1").checked == true) {
                status = 1;
            } else if (document.getElementById("switch1").checked == false) {
                status = 0;
            }
            if (form.$valid) {
                var Project = {
                    Id: projectId,
                    Name: $scope.name,
                    Status: parseInt(status),
                    Description: $scope.description
                };
                if ($scope.selectedFiles == null && $scope.action == 'Add') {
                    Project.Image = 'default-project.png';
                } else if ($scope.selectedFiles != null && $scope.action == 'Add') {
                    Project.Image = $scope.name + '.png';
                } else if ($scope.selectedFiles == null && $scope.action == 'Edit') {
                    Project.Image = $scope.image;
                } else if ($scope.selectedFiles != null && $scope.action == 'Edit') {
                    Project.Image = $scope.name + '.png';
                }
                if ($scope.selectedFiles != null) {
                    var formdata = new FormData();
                    formdata.append($scope.name, $scope.selectedFiles.item(0));
                    var request = {
                        method: 'POST',
                        url: '/api/Project/uploadFile/',
                        data: formdata,
                        headers: {
                            'Content-Type': undefined
                        }
                    };
                    $http(request).then(function (data) {
                    });
                }
                var method = ($scope.action == 'Add') ? ProjectService.Save : ProjectService.Update;

                method(Project, function (data) {
                    $window.open('/', '_self');
                    var action;
                    if ($scope.action == 'Add') {
                        action = 'added'
                    } else {
                        action = 'edited'
                    }
                    ngToast.success({
                        content: 'The project has been ' + action + ' successfull.'
                    });
                    $state.go('projects', {});
                }, function (response) {
                    ngToast.danger({
                        content: 'The project has not been' + action + '.'
                    });
                    $scope.saved = false;
                });
            } else {
                ngToast.info({
                    content: 'Please complete all fields correctly.'
                });
                $scope.saved = false;
            }
        }

        function isFileImage(file) {
            return file && file['type'].split('/')[0] === 'image';
        }
    }])
})();