(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TestSuplementalAddEditController', ['$scope', '$state', 'TestSuplementalService', 'AccessService', 'StepService', 'TagService', 'ProjectService', 'ngToast', 'AttachmentService', '$http', 'ValidationService', function ($scope, $state, TestSuplementalService, AccessService, StepService, TagService, ProjectService, ngToast, AttachmentService, $http, ValidationService) {
        $scope.action = $state.current.data.action;
        var suplementalId = $state.params.id;
        var projectId = $state.params.projectId;
        var stpCopy = $state.current.data.isCopy;
        HasAccess();
        $scope.tagsAux = [];
        $scope.stepsaux = [];
        $scope.FilesSelected = [];
        $scope.FilestoDisplay = [];
        $scope.saved = false;
        $scope.priorityOptions = ['Low', 'Normal', 'Medium', 'High', 'Urgent'];
        var tsCopy = $state.current.data.isCopy;
        var count = 0;

        var index = 1;
        $scope.customStatus = ['', '', ''];
        $scope.customStatusLine = ['normal', 'normal', 'normal'];
        $scope.isVisible = [false, false, false];
        $scope.from = 0;
        $scope.saveBtn = false;
        Stages();

        $scope.tags = [];


        function scrollToBottom() {
            var elmnt = document.getElementById("stepsTable");
            elmnt.scrollTop = elmnt.scrollHeight;
        }

        function Stages(to) {
            $scope.saveBtn = false;

            if (to == undefined) {
                step1();
            }
            if ($scope.action == 'Add') {
                if (to == 0) {
                    $scope.isVisible[0] = true;
                    $scope.isVisible[1] = false;
                    $scope.isVisible[2] = false;


                    $scope.customStatus[1] = 'normal';
                    $scope.customStatusLine[1] = 'normal';

                    $scope.customStatus[2] = 'normal';
                    $scope.customStatusLine[2] = 'normal';


                    step1();
                    $scope.from = 0;
                }
                if (to == 1) {
                    if ($scope.from > to || $scope.from == 0) {
                        if (formValidation($scope.form)) {
                            $scope.isVisible[0] = false;
                            $scope.isVisible[1] = true;
                            $scope.isVisible[2] = false;

                            $scope.customStatus[0] = 'done';
                            $scope.customStatusLine[0] = 'done';

                            $scope.customStatus[1] = 'active';
                            $scope.customStatusLine[1] = 'active';

                            $scope.customStatus[2] = 'normal';
                            $scope.customStatusLine[2] = 'normal';


                            $scope.from = 1;

                        } else {
                            ngToast.info({
                                content: 'Please complete all fields correctly.'
                            });
                            step1();
                        }
                    }
                }

                if (to == 2) {
                    if ($scope.from > to || $scope.from == 1) {
                        if (formValidation($scope.form)) {
                            $scope.isVisible[0] = false;
                            $scope.isVisible[1] = false;
                            $scope.isVisible[2] = true;


                            $scope.customStatus[0] = 'done';
                            $scope.customStatusLine[0] = 'done';

                            $scope.customStatus[1] = 'done';
                            $scope.customStatusLine[1] = 'done';

                            $scope.customStatus[2] = 'active';
                            $scope.customStatusLine[2] = 'active';


                            $scope.from = 2;
                            $scope.saveBtn = true;







                        } else {
                            ngToast.info({
                                content: 'Please complete all fields correctly.'
                            });
                            step1();
                        }
                    }
                    if ($scope.from == 2) {
                        $scope.saveBtn = true;

                    }
                }
                //if (to == 3) {
                //    if ($scope.from > to || $scope.from == 2) {
                //        $scope.isVisible[0] = false;
                //        $scope.isVisible[1] = false;
                //        $scope.isVisible[2] = false;
                //        $scope.isVisible[3] = true;

                //        $scope.customStatus[0] = 'done';
                //        $scope.customStatusLine[0] = 'done';

                //        $scope.customStatus[1] = 'done';
                //        $scope.customStatusLine[1] = 'done';

                //        $scope.customStatus[2] = 'done';
                //        $scope.customStatusLine[2] = 'done';

                //        $scope.customStatus[3] = 'customStatus';
                //        $scope.customStatusLine[3] = 'active';
                //        $scope.from = 3;
                //    }
                //}

            } else {

                $scope.saveBtn = true;

                if (to == undefined) {
                    to = 0;
                }

                if (to > 0) {
                    if (!formValidation($scope.form)) {
                        ngToast.info({
                            content: 'Please complete all fields correctly.'
                        });
                        step1();
                        return;
                    }
                }

                $scope.isVisible[0] = false;
                $scope.isVisible[1] = false;
                $scope.isVisible[2] = false;
                $scope.isVisible[to] = true;



                $scope.customStatus = ['done', 'done', 'done'];
                $scope.customStatus[to] = 'active';

                $scope.customStatusLine = ['done', 'done', 'done'];
            }
        }

        $scope.gogo = function (to) {
            Stages(to);
        };

        $scope.download = function () {
            var fileName = "Steps.txt";
            var text = ""
            var stepsOrder = $scope.steps.sort((a, b) => Number(a.number_steps) - Number(b.number_steps));
            stepsOrder.forEach(function (element) {
                text = text + element.action + "▲\n";
            });

            var element = document.createElement('a');
            element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
            element.setAttribute('download', fileName);

            element.style.display = 'none';
            document.body.appendChild(element);

            element.click();

            document.body.removeChild(element);
        };

        function formValidation(form) {
            if (form.$valid) {
                return true;
            } else {
                return false;
            }
        }

        function step1() {
            $scope.isVisible[0] = true;
            $scope.customStatus[0] = 'active';
            $scope.customStatusLine[0] = 'active';
        }



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
                        if (stpCopy) {
                            GetSTPCopy()
                        }
                        GetTestSuplemental();
                        GetRole();
                        GetValidations();
                        GetTags();
                    }
                }, function (error) {
                });
            } else {
                AccessService.HasAccess('STP', projectId, 0, 0, 0, suplementalId, 0, function (data) {
                    if (!data.HasPermission) {
                        $state.go('Projects').then(function () {
                            ngToast.danger({
                                content: 'You do not have access to the requested page.'
                            });
                        });
                    } else {
                        GetTestSuplemental();
                        GetRole();
                        GetValidations();
                        GetTags();
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
                if (data.Role == 'BA') {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                }
            }, function (error) {

            });
        }

        function GetSTPCopy() {
            if (tsCopy != null) {
                if (tsCopy) {
                    TestSuplementalService.Get(suplementalId, function (data) {
                        $scope.stp = data.stp_number;
                        $scope.title = "[COPY] " + data.Title;
                        $scope.description = data.Description;
                        $scope.test_procedure_creator = data.Test_Procedure_Creator;
                        $scope.date = data.Creation_Date;
                        $scope.status = data.Status;
                        GetAllSteps(suplementalId);
                        GetAllTags(suplementalId);
                        GetProject(projectId);
                    }, function (error) {
                    });

                }
            }
        }

        function GetValidations() {
            ValidationService.GetFilesValidation(function (data) {
                $scope.Validations = data;
            }, function (error) {

            })
        };


        $scope.changeOrder = function () {
            var table = document.getElementById("stepsTable");
            for (var i = 0, row; row = table.rows[i]; i++) {
                if (i != 0) {
                    for (var j = 0; j < $scope.stepsaux.length; j++) {
                        if ($scope.stepsaux[j].Id == row.cells[2].innerText) {
                            if ($scope.stepsaux[j].flag == "Original") {
                                if ($scope.stepsaux[j].number_steps != parseInt(row.cells[0].innerText)) {
                                    $scope.stepsaux[j].flag = "Updated";
                                }
                            } $scope.stepsaux[j].number_steps = parseInt(row.cells[0].innerText);
                        } if ($scope.stepsaux[j].flag == "Removed") {
                            $scope.stepsaux[j].number_steps = 0;
                        }
                    }
                }


            }
        }
        function GetTestSuplemental() {
            var value = localStorage.getItem("steps");
            $scope.steps = angular.fromJson(value);
            var vData = localStorage.getItem("data");
            var data1 = angular.fromJson(vData);
            if ($scope.action == 'Edit') {
                TestSuplementalService.Get(suplementalId, function (data) {
                    if (data1 == null) {
                        $scope.stp = data.stp_number;
                        $scope.title = data.Title;
                        $scope.description = data.Description;
                        $scope.test_procedure_creator = data.Test_Procedure_Creator;
                        $scope.date = data.Creation_Date;
                        $scope.status = data.Status;
                    } else {
                        suplementalId = projectId;
                        $scope.stp = data1.stp;
                        $scope.title = data1.title;
                        $scope.description = data1.description;
                        $scope.date = data1.date;
                        $scope.test_procedure_creator = data1.test_procedure_creator;
                        $scope.status = data1.status;
                    }
                    if ($scope.steps == null) {
                        GetAllSteps(suplementalId);

                    }
                    if ($scope.tagsBackup == null) {
                        GetAllTags(suplementalId);
                    }


                    AttachmentService.GetAttachment(2, suplementalId, function (data) {

                        $scope.FilestoDisplay = data;

                        $scope.FilestoDisplay.forEach(file => {
                            file.flag = 'Original';
                            file.show = $scope.ShowImage(file.Extention);
                        });
                        index = $scope.FilestoDisplay.length + 1;
                        $scope.CheckNames();
                    }, function (err) {
                    });


                }, function (error) {

                });
                GetProject(projectId);
            } else if ($scope.action == 'Add') {
                if (data1 != null) {
                    $scope.stp = data1.stp;
                    $scope.title = data1.title;
                    $scope.description = data1.description;
                    $scope.date = data1.date;
                }
                if ($scope.steps == null) {
                    $scope.steps = [];
                }
                if ($scope.tagsBackup == null) {
                    $scope.tagsBackup = [];
                    $scope.tags = [];
                }
                GetProject(projectId);
            }
        }

        function GetAllTags(suplementalId) {
            TagService.GetTestSuplementalTags(suplementalId, function (tags) {
                $scope.tagsBackup = tags;
                $scope.tags = tags;
                $scope.tagsBackup.forEach(function (tag) {
                    tag.flag = "Original";
                });
                $scope.tags.forEach(function (tag) {
                    tag.flag = "Original";
                });

                for (var i = 0; i < $scope.tags.length; i++) {
                    $scope.tagsAux[i] = $scope.tags[i];
                }
            }, function (error) {

            });
        }

        $scope.cancel = function () {
            $scope.redirectToList();
        };

        $scope.redirectToList = function () {
            $state.go('projectDetails', { id: projectId });
        };

        $scope.AddUpdate = function (form) {
            $scope.saved = true;
            var cont = 0;
            if (form.$valid) {
                $scope.steps.forEach(function (step) {
                    if (step.flag != "Removed") {
                        cont = cont + 1;
                    }
                });
                if ($scope.steps.length > 0 && cont > 0) {
                    if ($scope.tags.length > 0) {
                        if ($scope.action == "Add") {
                            createTestSuplemental();
                        } else {
                            updateTestSuplemental();
                        }
                    } else {
                        ngToast.danger({
                            content: 'You need to add at least one tag'
                        });
                    }
                } else {
                    ngToast.danger({
                        content: 'You need to add at least one step'
                    });
                }
            } else {
                ngToast.info({
                    content: 'Please complete all fields correctly.'
                });
            }
            $scope.saved = false
        }

        function createTestSuplemental() {
            $scope.TestSuplemental = {
                Test_Suplemental_Id: parseInt(suplementalId),
                Title: $scope.title,
                Description: $scope.description,
                Creation_Date: $scope.date,
                Project_Id: projectId,
                Status: true
            };

            TestSuplementalService.Save($scope.TestSuplemental, function (suplemental) {
                $scope.suplemental = suplemental;
                $scope.idLast = suplemental.Test_Suplemental_Id;
                suplementalId = $scope.idLast;
                $scope.count = 0;
                $scope.manageTags($scope.idLast);

                $scope.reorderSteps();

                $scope.steps.forEach(function (step) {
                    step.Test_Suplemental_Id = $scope.idLast;
                });
                var NormalSave = 1;
                StepService.SaveArray($scope.steps, projectId, NormalSave, function (data) {
                }, function (error) { });


                addChangeLog(suplementalId);
                ngToast.success({
                    content: 'Supplemental Test Procedure Added Successfully'
                });
                $scope.redirectToList();
                $scope.ManageAttachments(suplemental);
            }, function (error) {
                ngToast.danger({
                    content: 'Error Adding'
                });
                $scope.saved = false;
            });

        }

        function addChangeLog(suplementalId) {
            TestSuplementalService.AddChangeLog(suplementalId, function (data) {
            });
        }

        $scope.updateStep = function (number) {
            document.getElementById("addStep").style.display = "none";
            document.getElementById("editStep").style.display = "block";
            $scope.steps.forEach(function (stepp) {
                if (stepp.number_steps == number) {
                    $scope.stepEdit = stepp;
                    $scope.stepDescription = $scope.stepEdit.action;
                }
            });
        }

        $scope.editSteps = function () {
            if ($scope.stepDescription != '' && typeof $scope.stepDescription != 'undefined') {
                $scope.steps.forEach(function (step) {
                    if (step.Id == $scope.stepEdit.Id) {
                        step.action = $scope.stepDescription;
                        $scope.stepDescription = '';
                    }
                });
            } else {
                ngToast.danger({
                    content: 'Step description is empty'
                });
            }
        }

        $scope.addStep = function () {
            if ($scope.stepDescription != '' && typeof $scope.stepDescription != 'undefined') {

                if ($scope.stepDescription.includes("▲")) {
                    var Steps = $scope.stepDescription.split("▲");
                    Steps.forEach(function (st) {
                        if (st.trim() != "") {
                            $scope.AddStepToArray(st.trimLeft());
                        }
                    });
                    $scope.stepDescription = '';
                } else {
                    $scope.AddStepToArray($scope.stepDescription);
                    $scope.stepDescription = '';

                }
            } else {
                ngToast.danger({
                    content: 'Step description is empty'
                });
            }
        };

        $scope.AddStepToArray = function (Description) {
            var Step = {
                Id: count++,
                action: Description,
                type: "Step"
            }

            if ($scope.steps.length == 0) {
                Step.number_steps = 1;
            } else {
                var numberToAssign = 0;
                $scope.steps.forEach(function (step) {
                    if (numberToAssign <= step.number_steps) {
                        numberToAssign = step.number_steps;
                    }
                });
                numberToAssign = numberToAssign + 1;
                Step.number_steps = numberToAssign;

            }
            $scope.steps.push(Step);
            $scope.stepDescription = '';
        }

        $scope.reorderSteps = function () {
            var table = document.getElementById("stepsTable");
            for (var i = 0, row; row = table.rows[i]; i++) {
                if (i != 0) {
                    $scope.steps.forEach(function (step) {
                        if (step.Id == row.cells[2].innerText) {
                            step.number_steps = parseInt(row.cells[0].innerText);
                        }
                    });

                }
            }
        }

        $scope.removeStep = function (step) {

            $scope.reorderSteps();

            $scope.stepsAux = $scope.steps.filter(stepp => stepp.Id != step.Id);

            $scope.steps = angular.copy($scope.stepsAux);


            $scope.steps.forEach(function (stepp) {
                if (stepp.number_steps > step.number_steps) {
                    stepp.number_steps = stepp.number_steps - 1;
                }
            });


        }
        $scope.changeOrderSteps = function () {

            var table = document.getElementById("stepsTable");
            for (var i = 0, row; row = table.rows[i]; i++) {
                if (i != 0) {
                    $scope.steps.forEach(function (step) {
                        if (step.Id == row.cells[2].innerText) {
                            if (step.flag == "Original") {
                                if (step.number_steps != parseInt(row.cells[0].innerText)) {
                                    step.flag = "Updated";
                                }
                            }
                            step.number_steps = parseInt(row.cells[0].innerText);
                            localStorage.setItem("steps", JSON.stringify($scope.steps));
                        }
                    });

                }
            }
        }

        function GetAllSteps(id) {
            StepService.GetForTestSuplemental(id, function (data) {

                $scope.steps = data;
                $scope.steps.forEach(function (step) {
                    step.flag = "Original";
                });
                localStorage.setItem("steps", JSON.stringify($scope.steps));
            }, function (error) {
            });
        }


        $scope.RemoveAllSteps = function(){
            $scope.steps = [];
        }

        //Tags
        $scope.onTagAdded = function ($tag) {
            var currentId = 0;
            if ($scope.tags.length > 1) {
                var previousTagIdx = $scope.tags.indexOf($tag) - 1;
                var previousTag = $scope.tags[previousTagIdx];
                var tagid = $scope.tags.indexOf($tag);

                var aux = $scope.tags[$scope.tags.indexOf($tag)];
                var tagadded = $scope.tags[tagid];
                tagadded.flag = "Added";
                $scope.tagsBackup = $scope.tags;
                currentId = previousTag.id + 1;
                tagadded = currentId;
                $scope.tagsAux.push(aux);

            } else if ($scope.tags.length == 1) {
                var tagadded = $scope.tags[0];
                tagadded.flag = "Added";
                $scope.tagsAux.push(tagadded);
                $tag.id = currentId;
            }
        }

        $scope.onTagRemoved = function ($tag) {
            var nameAux = '';
            var value = localStorage.getItem("tags");
            $scope.tagsBackup = $scope.tags;
            $scope.tagsBackup.forEach(function (tag) {
                if (tag.name == $tag.name) {
                    nameAux = tag.name;
                    if (tag.flag != "Added") {
                        tag.flag = "Removed";
                    } else {
                        var idx = $scope.tagsBackup.indexOf(tag);
                        $scope.tagsBackup.splice(parseInt(idx), 1);
                    }
                }
            });

            $scope.tagsAux.forEach(function (tag) {
                if (tag.name == nameAux) {
                    if (tag.flag != "Added") {
                        tag.flag = "Removed";
                    } else {
                        var idx = $scope.tagsAux.indexOf(tag);
                        $scope.tagsAux.splice(parseInt(idx), 1);
                    }
                }
            });

        }

        function updateTestSuplemental() {
            $scope.TestSuplemental = {
                $id: count++,
                stp_number: $scope.stp,
                Test_Suplemental_Id: parseInt(suplementalId),
                Title: $scope.title,
                Description: $scope.description,
                Test_Procedure_Creator: $scope.test_procedure_creator,
                Creation_Date: $scope.date,
                Project_Id: projectId,
                Status: $scope.status
            };
            TestSuplementalService.Update($scope.TestSuplemental, function (suplemental) {
                var countTag = 0;
                $scope.idLast = suplementalId;

                $scope.manageTags(suplementalId);
                // $scope.changeOrderSteps();
                $scope.reorderSteps();
                var NormalSave = 1;
                var TypeOfEvidence = 1;
                StepService.UpdateArray($scope.steps, projectId, NormalSave, $scope.TestSuplemental.Test_Suplemental_Id, TypeOfEvidence, function (data) { }, function (error) { });


                addChangeLog(suplementalId);
                ngToast.success({
                    content: 'Supplemental Test Procedure Updated Successfully'
                });
                localStorage.removeItem('steps');
                localStorage.removeItem('rt');
                localStorage.removeItem('data');
                localStorage.removeItem('tpstps');
                $scope.ManageAttachments($scope.TestSuplemental);
                $scope.redirectToList();
            }, function (error) { });
        }

        function GetProject(projectId) {
            ProjectService.Get(projectId, function (data) {
                $scope.projectName = data.Name;
                $scope.projectId = data.Id;
            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: $scope.projectId });
        }

        $scope.manageTags = function (suplementalId) {
            $scope.tagsAux.forEach(function (tag) {
                if (stpCopy) {
                    if (tag.flag == "Original") {
                        TagService.Save(tag, 0, 0, 0, suplementalId, function (data) { }, function (error) { });
                    }
                }
                if (tag.flag == "Added") {
                    TagService.Save(tag, 0, 0, 0, suplementalId, function (data) { }, function (error) { });
                } else if (tag.flag == "Removed") {
                    TagService.Delete(tag.id, 0, 0, 0, suplementalId, function (data) { }, function (error) { });
                }
            });
        }


        /*Attachments */

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

            if ($scope.selectedFiles.length > 1) {



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
                        file: $scope.selectedFiles,
                        flag: 'New',
                        Path: '',
                        Size: tamanho,
                        Extention: "." + realName[realName.length - 1],
                        show: $scope.ShowImage("." + realName[realName.length - 1])

                    };
                    if ($scope.CheckIfAlreadyAdded($scope.Fileobj)) {
                        var filteredArray = $scope.FilestoDisplay.filter(f => f.flag != 'Removed');
                        if (filteredArray.length >= $scope.Validations.NumberOfAttachments) {
                            ngToast.danger({
                                content: 'Limit of attachments reached'
                            });
                            $scope.$apply();
                            return;

                        } else {
                            if (AddToArray) {
                                if ($scope.Validations.FilesAllowed == '/') {
                                    $scope.FilestoDisplay.push($scope.Fileobj);
                                }

                            }

                        }

                    }

                }
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
                    file: $scope.selectedFiles,
                    flag: 'New',
                    Path: '',
                    Size: tamanho,
                    Extention: "." + realName[realName.length - 1],
                    show: $scope.ShowImage("." + realName[realName.length - 1])

                };

                if ($scope.Validations.FilesAllowed != '/') {
                    if (!ValidateExtention($scope.Fileobj.Extention)) {
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

        $scope.ShowImage = function (Ext) {


            if (Ext.toLowerCase() == '.jpg' || Ext.toLowerCase() == '.png' || Ext.toLowerCase() == '.jpeg' || Ext.toLowerCase() == '.gif') {
                return true;
            } else {
                return false;
            }




        }

        $scope.ManageAttachments = function (data) {
            if ($scope.FilestoDisplay.lenght != 0) {
                $scope.IdToDelete = [];
                $scope.FilesToDelete = $scope.FilestoDisplay.filter(file => file.flag == 'Removed');
                $scope.FilesToDelete.forEach(function (f) {
                    $scope.IdToDelete.push(f.Id);
                });



                AttachmentService.RemoveAttachments($scope.IdToDelete, function (success) {
                    $scope.FilesToPost = $scope.FilestoDisplay.filter(file => file.flag == 'New');


                    var fileIndex = 0;
                    $scope.FilesToPost.forEach(function (f) {

                        var formdata = new FormData();
                        formdata.append($scope.name, f.file.item(fileIndex));
                        fileIndex++;
                        var request = {
                            method: 'POST',
                            url: '/api/Attachment/UploadFile/',
                            data: formdata,
                            params: {
                                EntityAction: 2,
                                EntityId: data.Test_Suplemental_Id,
                                projectId: projectId,
                                EntityNumber: data.Test_Suplemental_Id,
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

        $scope.RemoveAllAttachments = function () {



            var filteredArray = $scope.FilestoDisplay.filter(f => f.flag == 'Original');

            $scope.FilestoDisplay = filteredArray;

            $scope.FilestoDisplay.forEach(function (file) {
                file.flag = 'Removed';
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