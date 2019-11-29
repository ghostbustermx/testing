(function () {
    'use strict';

    var app = angular.module(appName);
    app.controller('RequirementAddEditController', ['$scope', '$state', 'RequirementService', 'AccessService', 'ProjectService', 'EmployeeInfoService', 'ngToast', '$http', 'AttachmentService', function ($scope, $state, RequirementService, AccessService, ProjectService, EmployeeInfoService, ngToast, $http, AttachmentService) {
        $scope.action = $state.current.data.action;
        var requirementId = $state.params.id;
        var projectId = $state.params.projectId;
        HasAccess();
        $scope.projects = [];
        $scope.employees = [];
        $scope.FilesSelected = [];
        $scope.selectedProject;
        $scope.empltyEmployee = {
            Id: 0,
            UserName: 'Empty',
            FullName: 'Not Assigned',
            FirstName: 'Not',
            LastName: 'Assigned',
            PhotoUrl: null
        };
        $scope.btnSave = false;
        $scope.developerAssigned = { selectedItems: $scope.empltyEmployee };
        $scope.testerAssigned = { selectedItems: $scope.empltyEmployee };
        $scope.tester = "";
        $scope.developer = "";
        $scope.axosoftId = "";
        $scope.FilestoDisplay = [];
        $scope.DuplicateWarning = false;
        $scope.ShowAttachmentSelect = true;

        $scope.showAutomated = false;

        var index = 1;
        function GetEmployees() {
            EmployeeInfoService.GetEmployees(function (data) {
                $scope.employees = data;
                $scope.employees.Items.unshift($scope.empltyEmployee);
            }, function (error) {
            });
        }

        $scope.setValue = function () {
            if ($scope.axosoftId == undefined) {
                $scope.axosoftId = 0;
            }
        }

        function GetRequirement() {
            if ($scope.action == 'Edit') {
                RequirementService.Get(requirementId, function (data) {
                    $scope.name = data.Name;
                    $scope.req_number = data.req_number;
                    $scope.description = data.Description;

                    $scope.testerAssigned.selectedItems = { UserName: data.Tester_Assigned };
                    $scope.axosoftId = data.Axosoft_Task_Id;
                    $scope.acceptance = data.Acceptance_Criteria;
                    $scope.release = data.Release;
                    $scope.developerAssigned.selectedItems = { UserName: data.Developer_Assigned };
                    AttachmentService.GetAttachment(1, requirementId, function (data) {

                        $scope.FilestoDisplay = data;

                        $scope.FilestoDisplay.forEach(file => {
                            file.flag = 'Original';
                        });
                        index = $scope.FilestoDisplay.length + 1;
                        $scope.CheckNames();
                    }, function (err) {
                    });

                    GetProject(projectId);


                }, function (error) {
                });
            } else {
                GetProject(projectId);
                $scope.CheckNames();
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

        $scope.onfileSelected = function (event) {
            var filteredArray = $scope.FilestoDisplay.filter(f => f.flag != 'Removed');
            if (filteredArray.length >= 5) {
                ngToast.danger({
                    content: 'Limit of attachments reached'
                });
                $scope.$apply();
                return;
            } 

            $scope.selectedFiles = event.target.files;
            var SizeInMb = $scope.selectedFiles[0].size / 1024;
            SizeInMb = SizeInMb / 1024;
            if (SizeInMb> 10) {
                ngToast.danger({
                    content: 'File Exceed 10MB'
                });
                $scope.$apply();
                return;
            }

            $scope.nameFile = event.target.files[0].name;

            $scope.Fileobj = {
                Id: index++,
                Name: event.target.files[0].name,
                file: $scope.selectedFiles,
                flag: 'New'
            };
            if ($scope.CheckIfAlreadyAdded($scope.Fileobj)) {
                $scope.FilestoDisplay.push($scope.Fileobj);
            }
            
            
            
            $scope.CheckNames();
            $scope.$apply();
            

        };

        $scope.CheckIfAlreadyAdded = function (file) {
            let indx = $scope.FilestoDisplay.findIndex(f => f.Name == file.Name);

            if (indx != -1) {
                
                if ($scope.FilestoDisplay[indx].flag == 'New') {
                    $scope.FilestoDisplay[indx] = file;
                    return false;
                } else if ($scope.FilestoDisplay[indx].flag == 'Original') {
                    $scope.FilestoDisplay[indx].flag = 'Removed';
                    return true;
                }
                
            } else {
                return true;
            }

            
        }
     

        $scope.RemoveFile = function (id) {
            var indx = $scope.FilestoDisplay.findIndex(file => file.Id == id);

            if ($scope.FilestoDisplay[indx].flag == 'New') {
                var aux = $scope.FilestoDisplay.filter(file => file.Id != id);
                $scope.FilestoDisplay = aux;

            } else {
                $scope.FilestoDisplay[indx].flag = 'Removed';
            }
 
            $scope.CheckNames();
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

        $scope.CheckNames = function () {
            $scope.DuplicateWarning = false;
            $scope.FilestoDisplay.forEach(function (f) {

                $scope.FilestoDisplay.forEach(function (file) {
                    if (f.Id != file.Id) {
                        if (f.Name == file.Name) {
                            $scope.DuplicateWarning = true;
                        }
                    }
                });

            });
            if ($scope.DuplicateWarning) {
                document.getElementById("NameWarning").style.display = "block";
            } else {
                document.getElementById("NameWarning").style.display = "none";
            }
            
        };

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