(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('ProjectController', ['$scope', '$state', '$window', 'ProjectService', 'AccessService', 'ModalConfirmService', 'ngToast', '$http', '$timeout', 'ExecutionGroupService', function ($scope, $state, $window, ProjectService, AccessService, ModalConfirmService, ngToast, $http, $timeout, ExecutionGroupService) {
        $scope.currentPage = 1;
        $scope.pageSize = 10;
        $scope.hasPermissionProject = false;
        HasAccessProjectSection();
        localStorage.removeItem('steps');
        localStorage.removeItem('rt');
        localStorage.removeItem('tags');
        localStorage.removeItem('data');
        localStorage.removeItem('tpstps');
        $scope.projects = [];
        $scope.Options = [];
        $scope.download = false;
        $scope.optionSelected = 'Empty Report';
        $scope.Options2 = ['Empty Report', 'Last Execution'];
        var tries = 0;
        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelected = $scope.Options[0];
            }, function (error) {
            });
        }

        $scope.changeValue = function (value) {
            $scope.pageSize = value;
        }

        $scope.SetProject = function (id, project) {
            $scope.selectedProject = {};
            $scope.selectedProject.id = id;
            $scope.selectedProject.name = project;
            GetGroups(id);
        }

        function GetGroups(projectId) {
            ExecutionGroupService.GetByProjectActives(projectId, function (data) {
                if (data.length == 0) {
                    $scope.Options2 = ['Empty Report'];
                }
                $("#Download").modal().show();
            }, function (errr) {

            });
        }

        $scope.downloadFileExcel = function () {
            $("#loadingModalProjectReport").modal({
                backdrop: "static",
                keyboard: false,
                show: true
            });
            $scope.message = 'In queue to generate the excel report';

            ProjectService.RequestExcel(function (status) {

               

                if (status.assigned == true) {
                    $scope.download = true;
                    $scope.message = 'Generating Project Report';

                    //optionSelected
                   
                    var date = new Date();
                    date = date.toISOString();
                    date = date.replace(/:/g, "-");
                    date = date.replace(/\./g, "-");
                    $http({
                        method: 'GET',
                        url: '/api/Project/DownloadExcelEmpty',
                        responseType: 'arraybuffer',
                        params: {
                            id: $scope.selectedProject.id,
                            name: $scope.selectedProject.name,
                            type: $scope.optionSelected
                        },
                        dataType: "binary",
                        processData: false,
                    }).then(function (response) {
                            var filename = $scope.selectedProject.name + date;
                            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                            try {
                                var blob = new Blob([response.data], { type: contentType });
                                var url = window.URL.createObjectURL(blob);
                                var linkElement = document.createElement('a');
                                linkElement.setAttribute('href', url);
                                linkElement.setAttribute("download", filename);
                                var clickEvent = new MouseEvent("click", {
                                    "view": window,
                                    "bubbles": true,
                                    "cancelable": false
                                });
                                linkElement.dispatchEvent(clickEvent);
                                $scope.download = false;

                                $("#loadingModalProjectReport").modal("hide");

                            } catch (ex) {
                                $scope.download = false;
                                ngToast.danger({
                                    content: "Error: " + ex
                                });
                            }
                        
                    }, function (error) {
                        $scope.download = false;
                        ngToast.danger({
                            content: "Report Server busy, please try again in a few minutes"
                        });
                        tries = 0
                        $("#loadingModalProjectReport").modal("hide");
                    });
                } else {
                    if (tries <= 10) {
                        $timeout(function () {
                            tries = tries + 1;
                            $scope.downloadFileExcel();
                            
                        }, 5000);
                    }
                    else if (tries > 10) {
                        $scope.download = false;
                        ngToast.danger({
                            content: "Report Server busy, please try again in a few minutes"
                        });
                        tries = 0
                        $("#loadingModalProjectReport").modal("hide");
                    }


                }
                }, function (error) {

                    if (tries <= 10) {
                        $timeout(function () {
                            tries = tries + 1;
                            $scope.downloadFileExcel();
                            
                        }, 5000);
                    }
                    else if (tries > 10) {
                        $scope.download = false;
                        ngToast.danger({
                            content: "Report Server busy, please try again in a few minutes"
                        });
                        tries = 0
                        $("#loadingModalProjectReport").modal("hide");
                    }
                });

        }
        GetPaginationElements();
        GetAllProjects();
        function HasAccessProjectSection() {
            AccessService.HasAccessProjectSection(function (data) {
                $scope.hasPermissionProject = data.HasPermission;
            }, function (error) {
                $scope.hasPermissionProject = false;
            });
        }

        function GetAllProjects() {
            ProjectService.GetActives(function (data) {
                $scope.projects = data;
            }, function (error) {
            });
        }

        $scope.redirectTo = function (url) {
            $window.open(url, '_blank');
        };

        $scope.projectDetails = function (id) {
            $state.go('projectDetails', { id: id });
        }

        $scope.showModalToDelete = function (project) {
            var options = {
                closeButtonText: 'Cancel',
                actionButtonText: 'Yes',
                headerText: 'Delete',
                bodyText: 'Are you sure you want to delete the project: ' + project.Name
            };
            ModalConfirmService.showModal({}, options).then(function (result) {
                ProjectService.Delete(project.Id);
                ngToast.success({
                    content: 'The project has been deleted successfull.'
                });
            });
            GetAllProjects();
        }
    }])
})();