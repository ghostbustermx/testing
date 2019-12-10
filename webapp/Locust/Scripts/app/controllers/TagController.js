(function () {
    'use strict';

    var app = angular.module(appName);
    app.controller('TagController', ['$scope', '$state', 'ProjectService',  'ngToast', '$http', 'AttachmentService', function ($scope, $state, ProjectService, ngToast, $http, AttachmentService) {
        $scope.action = $state.current.data.action;
        var requirementId = $state.params.id;
        var projectId = $state.params.projectId;
        HasAccess();
        $scope.projects = [];       
        $scope.selectedProject;
        $scope.btnSave = false;
        $scope.axosoftId = "";
        $scope.DuplicateWarning = false;
       
        

        $scope.setValue = function () {
            if ($scope.axosoftId == undefined) {
                $scope.axosoftId = 0;
            }
        }




        function GetProject(id) {
            ProjectService.Get(id, function (data) {
                $scope.projectName = data.Name;
            });
        }

        $scope.cancel = function () {
            $scope.redirectToList();
            
        };

        $scope.redirectToList = function () {
            $state.go('projectDetails', { id: projectId });
        };

        function HasAccess() {
            if ($scope.action == 'Add') {
                AccessService.HasAccess('Project', projectId, 0, 0, 0, 0, 0, function (data) {
                    if (!data.HasPermission) {
                        $state.go('Projects').then(function () {
                            ngToast.danger({
                                content: 'You do not have access to the requested page.'
                            });
                        });
                    } else {
                        GetRequirement();
                        GetRole();
                        GetEmployees();

                    }
                }, function (error) {
                });
            } else {
                AccessService.HasAccess('Requirement', projectId, requirementId, 0, 0, 0, 0, function (data) {
                    if (!data.HasPermission) {
                        $state.go('Projects').then(function () {
                            ngToast.danger({
                                content: 'You do not have access to the requested page.'
                            });
                        });
                    } else {
                        GetEmployees();
                        GetRequirement();
                        GetRole();
                    }
                }, function (error) {
                });
            }
        };

        function GetRole() {
            AccessService.GetRole(function (data) {
                $scope.CurrentUserRole = data.Role;
                if (data.Role == 'VBP') {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                }
            }, function (error) {
            });
        }



    


        $scope.AddUpdate = function (form) {
        
            if ($scope.axosoftId != 0) {
                if (!$scope.axosoftId.toString().match('^((?!(0))[0-9]{1,9})$')) {
                    ngToast.danger({
                        content: 'Not valid pattern for Axosoft Id'
                    });
                    return;
                }
            }

            if (form.$valid) {
                $scope.btnSave = true;
                var Requirement = {
                    Id: requirementId,
                    Project_Id: projectId,
                    req_number: $scope.req_number,
                    Axosoft_Task_Id: $scope.axosoftId,
                    Acceptance_Criteria: $scope.acceptance,
                    Release: $scope.release,
                    Name: $scope.name,
                    Description: $scope.description,
                    Developer_Assigned: $scope.developerAssigned.selectedItems.UserName,
                    Tester_Assigned: $scope.testerAssigned.selectedItems.UserName
                };
                var method = ($scope.action == 'Add') ? RequirementService.Save : RequirementService.Update;
                method(Requirement, function (data) {
                    var rep = "repeated" + Requirement.Axosoft_Task_Id;
                    if (data.Name == rep) {
                        ngToast.danger({
                            content: 'Axosoft ID repeated.'
                        });
                        $scope.btnSave = false;
                    } else {



                        if ($scope.FilestoDisplay.lenght != 0) {
                            $scope.IdToDelete = [];
                            $scope.FilesToDelete = $scope.FilestoDisplay.filter(file => file.flag == 'Removed');
                            $scope.FilesToDelete.forEach(function (f) {
                                $scope.IdToDelete.push(f.Id);
                            });



                            AttachmentService.RemoveAttachments($scope.IdToDelete, function (success) {
                                $scope.FilesToPost = $scope.FilestoDisplay.filter(file => file.flag == 'New');



                                $scope.FilesToPost.forEach(function (f) {

                                    var formdata = new FormData();
                                    formdata.append($scope.name, f.file.item(0));
                                    var request = {
                                        method: 'POST',
                                        url: '/api/Attachment/UploadFile/',
                                        data: formdata,
                                        params: {
                                            EntityAction: 1,
                                            EntityId: data.Id,
                                            projectId: projectId,
                                            EntityNumber: data.req_number,
                                            number: f.Id
                                        },
                                        headers: {
                                            'Content-Type': undefined
                                        }
                                    };
                                    $http(request).then(function (data) {
                                    });
                                });

                            }, function (err) {
                            });
                        }
                    

                       
                            

                        
                        $scope.redirectToList();
                        var action;
                        if ($scope.action == 'Add') {
                            action = 'added'
                        } else {
                            action = 'edited'
                        }
                        ngToast.success({
                            content: 'The requirement has been ' + action + ' successfully.'
                        });
                    }
                }, function (response) {
                    $scope.btnSave = false;
                    ngToast.danger({
                        content: 'The requirement has not been ' + action + '.'
                    });
                });
            } else {
                $scope.btnSave = false;
                ngToast.info({
                    content: 'Please complete all fields correctly.'
                });
            }
        }
    }])
})();