(function () {
    'use strict';

    var app = angular.module(appName);
    app.controller('ScriptGroupAddEditController', ['$scope', '$state', 'AccessService', 'ProjectService', 'EmployeeInfoService', 'ngToast', '$http', 'ValidationService', 'RequirementService', 'ScriptsGroupService', 'ScriptsService','TestProcedureService' ,function ($scope, $state, AccessService, ProjectService, EmployeeInfoService, ngToast, $http, ValidationService, RequirementService, ScriptsGroupService, ScriptsService, TestProcedureService) {
        $scope.action = $state.current.data.action;
        var scriptgroupId = $state.params.id;
        var projectId = $state.params.projectId;
        GetProject(projectId);
        $scope.priorityOptions = ['Low', 'Normal', 'Medium', 'High', 'Urgent'];
        $scope.FilestoDisplay = [];
        GetValidations();
        var index = 1;
        $scope.TestCasesList = [];


        //Navigation

        $scope.RedirectToDetails = function () {
            $state.go('projectDetails', { id: projectId });
        }

        function GetProject(id) {
            ProjectService.Get(id, function (data) {
                $scope.Project_Id = data.Name;
                $scope.ProjectName = data.Name;
            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };

        //Script Group Add/Update
        $scope.AddUpdate = function () {
            if ($scope.action == 'Add') {
                $scope.ScripGroup = {
                    projectId: projectId,
                    Name: $scope.name,
                    Release: $scope.release,
                    Priority: $scope.selectedPriority
                };
                ScriptsGroupService.Save($scope.ScripGroup, function (Data) {
                    $scope.UploadFiles(Data.Id);
                }, function (error) {
                });
            } else {

            }

        }

        $scope.CreateTestProceduresAutomated = function (GroupId) {
            ScriptsService.GetAllScripts(GroupId, function (Scripts) {
                $scope.Ids = [];
                Scripts.forEach(s => {
                    $scope.Ids.push(s.Id);
                });

                TestProcedureService.CreateFromScripts(Scripts, projectId, function (TestProcedures) {

                    $scope.RedirectToDetails();
                },
                    function (error) {
                    });
            }, function (error) {

            });
        }

        //Files Section

        $scope.UploadFiles = function (GroupId) {
            if ($scope.FilestoDisplay.lenght != 0) {
                $scope.IdToDelete = [];
                $scope.FilesToDelete = $scope.FilestoDisplay.filter(file => file.flag == 'Removed');
                $scope.FilesToDelete.forEach(function (f) {
                    $scope.IdToDelete.push(f.Id);
                });



           //     AttachmentService.RemoveAttachments($scope.IdToDelete, function (success) {
                    $scope.FilesToPost = $scope.FilestoDisplay.filter(file => file.flag == 'New');
                var HasFinish = 0;
               
                    var fileIndex = 0;
                    $scope.FilesToPost.forEach(function (f) {
                        f.ScriptsGroup_Id = GroupId;
                        var formdata = new FormData();
                        formdata.append($scope.name, f.file);
                        fileIndex++;
                        var request = {
                            method: 'POST',
                            url: '/api/Scripts/Save/',
                            data: formdata,
                            params: {
                                GroupId: GroupId,
                                projectId: projectId,
                            },
                            headers: {
                                'Content-Type': undefined
                            }
                        };
                        $http(request).then(function (data) {
                            HasFinish++;
                            if (HasFinish == $scope.FilesToPost.length) {
                                $scope.CreateTestProceduresAutomated(GroupId);
                            }
                            
                            
                        });
                    });

             //   }, function (err) {
            //    });
            }

        }

        function GetValidations() {
            ValidationService.GetScriptsValidation(function (data) {
                $scope.Validations = data;
            }, function (error) {

            })
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


        $scope.onfileSelected = function (event) {
            var filteredArray = $scope.FilestoDisplay.filter(f => f.flag != 'Removed');



            $scope.selectedFiles = event.target.files;

            if ($scope.selectedFiles.length != 0) {
            if ($scope.selectedFiles.length > 1) {
                $("#loadingModalPlay").modal({
                    backdrop: "static",
                    keyboard: false,
                    show: true
                });

                $scope.NumberOfFiles = $scope.selectedFiles.length;

                for (var i = 0; i < $scope.selectedFiles.length; i++) {
                    var AddToArray = true;
                    var SizeInMb = $scope.selectedFiles[i].size / 1024;
                    SizeInMb = SizeInMb / 1024;
                    if ($scope.selectedFiles[i].size > $scope.Validations.TotalMb) {
                        var SizeToDisplay = $scope.Validations.TotalMb / 1024;
                        SizeToDisplay = SizeToDisplay / 1024;
                        ngToast.danger({
                            content: 'File Exceed ' + SizeToDisplay.toString() + " MB's"
                        });
                        $scope.$apply();
                        AddToArray = false;
                    }

                    $scope.nameFile = $scope.selectedFiles[i].name;
                    var realName = $scope.nameFile.split(/\.(?=[^\.]+$)/);
                    var tamanho = SizeInMb.toString().slice(0, 5);

                    $scope.Fileobj = {
                        Id: index++,
                        Name: event.target.files[i].name,
                        file: $scope.selectedFiles[i],
                        flag: 'New',
                        Extention: "." + realName[realName.length - 1]
                    };


                    if ($scope.Validations.FilesAllowed != '/') {
                        if (!$scope.ValidateExtention($scope.Fileobj.Extention)) {
                            ngToast.danger({
                                content: 'Non a valid format'
                            });
                            $scope.$apply();
                            return;
                        }
                    }

                    if ($scope.CheckIfAlreadyAdded($scope.Fileobj)) {
                        $scope.FilestoDisplay.push($scope.Fileobj);

                        
                    }

                }
                $scope.LoadFileText();
                $scope.selectedFiles = undefined;
            } else {
                
                var SizeInMb = $scope.selectedFiles[0].size / 1024;
                SizeInMb = SizeInMb / 1024;
                if ($scope.selectedFiles[0].size > $scope.Validations.TotalMb) {
                    var SizeToDisplay = $scope.Validations.TotalMb / 1024;
                    SizeToDisplay = SizeToDisplay / 1024;
                    ngToast.danger({
                        content: 'File Exceed ' + SizeToDisplay.toString() + " MB's"
                    });
                    $scope.$apply();
                    return;
                }

                $scope.nameFile = event.target.files[0].name;

                var tamanho = SizeInMb.toString().slice(0, 5);

                var realName = $scope.nameFile.split(/\.(?=[^\.]+$)/);

                $scope.Fileobj = {
                    Id: index++,
                    Name: event.target.files[0].name,
                    file: $scope.selectedFiles[0],
                    flag: 'New',
                    Path: '',
                    Extention: "." + realName[realName.length - 1]
                };

                if ($scope.Validations.FilesAllowed != '/') {
                    if (!$scope.ValidateExtention($scope.Fileobj.Extention)) {
                        ngToast.danger({
                            content: 'Non a valid format'
                        });
                        $scope.$apply();
                        return;
                    }
                }

                if ($scope.CheckIfAlreadyAdded($scope.Fileobj)) {
                    $scope.FilestoDisplay.push($scope.Fileobj);

                    const reader = new FileReader()
                    reader.onload = handleFileLoad;
                    reader.readAsText($scope.Fileobj.file);


                    function handleFileLoad(event) {
                        $scope.ExtractTestCasesFromFile(event.target.result);
                        RequirementService.GetTestCasesByNumber($scope.TestCasesList, projectId,0, function (response) {
                            $scope.Procedures = response;
                            $scope.selectedFiles = undefined;
                            $("#loadingModalPlay").modal("hide");
                        }, function (error) {
                                $("#loadingModalPlay").modal("hide");
                        })
                        
                    }


                } else {
                    ngToast.info({
                        content: 'Files with the same name will be overwritten'
                    });
                    $("#loadingModalPlay").modal("hide");
                }


            }

            $scope.CheckNames();
            $scope.$apply();
            }
        };

        $scope.LoadFileText = function () {
            var Finish = 0;
            $scope.TestCasesList = [];

            var aux = $scope.FilestoDisplay.filter(f => f.flag != 'Removed');

            if (aux.length != 0) {
                $scope.FilestoDisplay.forEach(function (Fileobj) {
                    var reader = new FileReader()
                    reader.onload = handleFileLoad;
                    reader.readAsText(Fileobj.file);

                    function handleFileLoad(event) {
                        File.fulltext = event.target.result;
                        Finish = $scope.ExtractTestCasesFromFile(File.fulltext, Finish);
                        if (Finish == $scope.FilestoDisplay.length) {
                            RequirementService.GetTestCasesByNumber($scope.TestCasesList, projectId,0, function (response) {
                                $scope.Procedures = response;
                                $("#loadingModalPlay").modal("hide");
                            }, function (error) {
                                $("#loadingModalPlay").modal("hide");
                            })
                        };

                    }
                })
            } else {
                $scope.Procedures = [];
                $("#loadingModalPlay").modal("hide");
            }
            
        }


        $scope.ExtractTestCasesFromFile = function (fulltext, Iteration) {

            var CroppedText = fulltext.split("\n");

            CroppedText.forEach(textLine => {
                if (textLine.includes("TC_")) {

                    var text = textLine.substring(textLine.indexOf("TC_"), textLine.length - 1);
                    text = text.trim();
                    $scope.TestCasesList.push(text);
                }

            });

            Iteration = Iteration + 1;
            return Iteration;
            //debugger;
            
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

        $scope.RemoveFile = function (id) {

            $("#loadingModalPlay").modal({
                backdrop: "static",
                keyboard: false,
                show: true
            });

            var indx = $scope.FilestoDisplay.findIndex(file => file.Id == id);

            if ($scope.FilestoDisplay[indx].flag == 'New') {
                var aux = $scope.FilestoDisplay.filter(file => file.Id != id);
                $scope.FilestoDisplay = aux;

            } else {
                $scope.FilestoDisplay[indx].flag = 'Removed';
            }

            $scope.LoadFileText();
        }

        $scope.ValidateExtention = function (Ext) {

            var extentionsArray = $scope.Validations.FilesAllowed.split(",");

            var Execute = false;

            for (var i = 0; i < extentionsArray.length; i++) {

                if (extentionsArray[i].toLowerCase() == Ext.toLowerCase()) {
                    Execute = true;
                }

            }

            return Execute;

        }

    }])
})();