(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TestCaseAddEditController', ['$scope', '$state', 'TestCaseService', 'StepService', 'AccessService', 'RequirementsTestService', 'RequirementService', 'TagService', 'ngToast', 'TypesOfTestService', 'AttachmentService', '$http', 'ValidationService', function ($scope, $state, TestCaseService, StepService, AccessService, RequirementsTestService, RequirementService, TagService, ngToast, TypesOfTestService, AttachmentService, $http, ValidationService) {

        $scope.action = $state.current.data.action;
        var testprocedureId = $state.params.id;
        var reqId = $state.params.reqId;
        var tcCopy = $state.current.data.isCopy;
        var projectId = $state.params.projectId;

        HasAccess();
        $scope.currentPageREQ1 = 1;
        $scope.pageSizeREQ = 5;
        $scope.currentPageREQ = 1;
        $scope.pageSize = 10;
        $scope.tagsAux = [];
        $scope.stepsaux = [];
        $scope.typesOfTest = [];
        $scope.FilesSelected = [];
        $scope.FilestoDisplay = [];
        $scope.priorityOptions = ['Low', 'Normal', 'Medium', 'High', 'Urgent'];

        $scope.CurrentUserRole = "";
        var index = 1;
        var count = 1;
        $scope.customStatus = ['', '', '', ''];
        $scope.customStatusLine = ['normal', 'normal', 'normal', 'normal'];
        $scope.isVisible = [false, false, false, false];
        $scope.from = 0;
        $scope.saveBtn = false;
        Stages();


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
                    $scope.isVisible[3] = false;

                    $scope.customStatus[1] = 'normal';
                    $scope.customStatusLine[1] = 'normal';

                    $scope.customStatus[2] = 'normal';
                    $scope.customStatusLine[2] = 'normal';

                    $scope.customStatus[3] = 'normal';
                    $scope.customStatusLine[3] = 'normal';
                    step1();
                    $scope.from = 0;
                }
                if (to == 1) {
                    if ($scope.from > to || $scope.from == 0) {
                        if (formValidation($scope.form)) {
                            $scope.isVisible[0] = false;
                            $scope.isVisible[1] = true;
                            $scope.isVisible[2] = false;
                            $scope.isVisible[3] = false;

                            $scope.customStatus[0] = 'done';
                            $scope.customStatusLine[0] = 'done';

                            $scope.customStatus[1] = 'active';
                            $scope.customStatusLine[1] = 'active';

                            $scope.customStatus[2] = 'normal';
                            $scope.customStatusLine[2] = 'normal';

                            $scope.customStatus[3] = 'normal';
                            $scope.customStatusLine[3] = 'normal';
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
                            $scope.isVisible[3] = false;


                            $scope.customStatus[0] = 'done';
                            $scope.customStatusLine[0] = 'done';

                            $scope.customStatus[1] = 'done';
                            $scope.customStatusLine[1] = 'done';

                            $scope.customStatus[2] = 'active';
                            $scope.customStatusLine[2] = 'active';


                            $scope.customStatus[3] = 'normal';
                            $scope.customStatusLine[3] = 'normal';
                            $scope.from = 2;

                        } else {
                            ngToast.info({
                                content: 'Please complete all fields correctly.'
                            });
                            step1();
                        }
                    }
                }
                if (to == 3) {
                    if ($scope.from > to || $scope.from == 2) {
                        $scope.isVisible[0] = false;
                        $scope.isVisible[1] = false;
                        $scope.isVisible[2] = false;
                        $scope.isVisible[3] = true;

                        $scope.customStatus[0] = 'done';
                        $scope.customStatusLine[0] = 'done';

                        $scope.customStatus[1] = 'done';
                        $scope.customStatusLine[1] = 'done';

                        $scope.customStatus[2] = 'done';
                        $scope.customStatusLine[2] = 'done';

                        $scope.customStatus[3] = 'active';
                        $scope.customStatusLine[3] = 'active';
                        $scope.from = 3;
                        $scope.saveBtn = true;
                    }
                    if ($scope.from == 3) {
                        $scope.saveBtn = true;

                    }
                }

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
                $scope.isVisible[3] = false;
                $scope.isVisible[to] = true;



                $scope.customStatus = ['done', 'done', 'done', 'done'];
                $scope.customStatus[to] = 'active';

                $scope.customStatusLine = ['done', 'done', 'done', 'done'];
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

        $scope.Options = [];

        function HasAccess() {
            if ($scope.action == 'Add') {
                AccessService.HasAccess('Requirement', projectId, reqId, 0, 0, 0, 0, function (data) {
                    if (!data.HasPermission) {
                        $state.go('Projects').then(function () {
                            ngToast.danger({
                                content: 'You do not have access to the requested page.'
                            });
                        });
                    } else {
                        GetPaginationElements();
                        GetTypes();
                        if (tcCopy) {
                            GetTCCopy();
                        }
                        GetRole();
                        GetTestCase();
                        GetValidations();
                    }
                }, function (error) {
                });
            } else {
                AccessService.HasAccess('TC', projectId, reqId, testprocedureId, 0, 0, 0, function (data) {
                    if (!data.HasPermission) {
                        $state.go('Projects').then(function () {
                            ngToast.danger({
                                content: 'You do not have access to the requested page.'
                            });
                        });
                    } else {
                        GetPaginationElements();
                        GetTypes();
                        GetRole();
                        GetTestCase();
                        GetValidations();
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


        function GetValidations() {
            ValidationService.GetFilesValidation(function (data) {
                $scope.Validations = data;
            }, function (error) {

            })
        };

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


        function GetTCCopy() {
            if (tcCopy != null) {
                if (tcCopy) {
                    TestCaseService.Get(testprocedureId, function (data) {
                        $scope.title = "[COPY] " + data.Title;
                        $scope.description = data.Description;
                        $scope.preconditions = data.Preconditions;
                        $scope.expectedResult = data.Expected_Result;
                        $scope.selectedPriority = data.Test_Priority;
                        $scope.selectedType = data.Type;
                        $scope.status = data.Status;

                        GetAllSteps(testprocedureId);
                        GetAllTags(testprocedureId);
                        GetAllRelations(testprocedureId);

                        GetProject(reqId);
                        GetRequirement(reqId);

                    }, function (error) {

                    });

                }
            }
        }


        function GetTypes() {
            TypesOfTestService.Get(function (data) {
                $scope.typesOfTest = data;
            },
                function (err) {

                });
        }


        function GetTestCase() {
            var value = localStorage.getItem("steps");
            $scope.steps = angular.fromJson(value);
            var vData = localStorage.getItem("data");
            var data1 = angular.fromJson(vData);
            var rData = localStorage.getItem("rt");
            $scope.requirementsAssigned = angular.fromJson(rData);

            if ($scope.action == 'Edit') {
                TestCaseService.Get(testprocedureId, function (data) {
                    if (data1 == null) {
                        $scope.tc = data.tc_number;
                        $scope.title = data.Title;
                        $scope.description = data.Description;
                        $scope.preconditions = data.Preconditions;
                        $scope.expectedResult = data.Expected_Result;
                        $scope.selectedType = data.Type;
                        $scope.selectedPriority = data.Test_Priority;
                        $scope.test_case_creator = data.Test_Case_Creator;
                        $scope.date = data.Creation_Date;
                        $scope.status = data.Status;
                    } else {
                        $scope.tc = data1.tc;
                        $scope.title = data1.title;
                        $scope.description = data1.description;
                        $scope.preconditions = data1.preconditions;
                        $scope.selectedType = data1.Type;
                        $scope.test_case_creator = data1.test_case_creator;
                        $scope.expectedResult = data1.expectedResult;
                        $scope.selectedPriority = data1.selectedPriority;
                        $scope.date = data1.date;
                        $scope.status = data1.status;


                    }

                    if ($scope.steps == null) {
                        GetAllSteps(testprocedureId);
                    }
                    if ($scope.tagsBackup == null) {
                        GetAllTags(testprocedureId);
                    }
                    if ($scope.requirementsAssigned == null) {
                        $scope.requirementsAssigned = [];
                        GetAllRelations(testprocedureId);
                    }

                    AttachmentService.GetAttachment(3, testprocedureId, function (data) {

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
                GetProject(reqId);
                GetRequirement(reqId);
            } else if ($scope.action == 'Add') {
                if (data1 != null) {
                    $scope.tc = data1.tc;
                    $scope.title = data1.title;
                    $scope.description = data1.description;
                    $scope.preconditions = data1.preconditions;
                    $scope.expectedResult = data1.expectedResult;
                    $scope.selectedPriority = data1.selectedPriority;
                    $scope.date = data1.date;
                } else {

                }
                if ($scope.steps == null) {
                    $scope.steps = [];
                }
                if ($scope.tagsBackup == null) {
                    $scope.tagsBackup = [];
                    $scope.tags = [];
                }
                if ($scope.requirementsAssigned == null) {
                    $scope.requirementsAssigned = [];
                }
                GetProject(reqId);
                GetRequirement(reqId);
            }
        }
        $scope.selectReq = function (item) {
            document.getElementById("dropdownMenu1").innerHTML = item.Axosoft_Task_Id + '-' + item.Name + '<span class="caret"></span>';
            $scope.reqSelect = item;
        }

        $scope.deleteReq = function (req) {
            var atleast = 0;
            if ($scope.requirementsAssigned.length > 1) {
                $scope.requirementsAssigned.forEach(function (req1) {
                    if (req1.Axosoft_Task_Id != req.Axosoft_Task_Id && req1.flag != "Removed") {
                        atleast = atleast + 1;
                    }
                });
                if (atleast > 0) {
                    var idx = $scope.requirementsAssigned.indexOf(req);
                    var req = $scope.requirementsAssigned[idx];
                    if (req.flag != "Added") {
                        req.flag = "Removed";
                    } else {
                        $scope.requirementsAssigned.splice(parseInt(idx), 1);
                    }
                    localStorage.setItem("rt", JSON.stringify($scope.requirementsAssigned));
                } else {
                    ngToast.danger({
                        content: 'Need to have at least one requirement assigned'
                    });
                }
            } else {
                ngToast.danger({
                    content: 'Need to have at least one requirement assigned'
                });
            }

        }

        $scope.addReq = function () {
            var duplicated = 0;
            $scope.requirementsAssigned.forEach(function (req) {
                if (req.Axosoft_Task_Id == $scope.reqSelect.Axosoft_Task_Id) {
                    duplicated = 1;
                }
            });
            if (duplicated == 0) {
                $scope.reqSelect.flag = "Added";
                $scope.requirementsAssigned.push($scope.reqSelect);
            } else {
                ngToast.danger({
                    content: 'Requirement already selected'
                });
            }

        };

        function GetAllTags(testprocedureId) {
            TagService.GetTestCaseTags(testprocedureId, function (tags) {
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
            $state.go('requirementDetails', { projectId: projectId, id: reqId });

        };

        $scope.AddUpdate = function (form) {
            var cont = 0;
            $scope.saved = true;

            if (form.$valid) {
                $scope.steps.forEach(function (step) {
                    if (step.flag != "Removed") {
                        cont = cont + 1;
                    }
                })
                if ($scope.steps.length > 0 && cont > 0) {
                    if ($scope.tags.length > 0) {
                        if ($scope.action == "Add") {
                            document.getElementById("save").disabled = true;
                            createTestCase();
                        } else {
                            document.getElementById("save").disabled = true;
                            updateTestCase();
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

        $scope.RemoveAllSteps = function () {
            $scope.steps = [];
        }

        function createTestCase() {
            $scope.TestCase = {
                Test_Case_Id: parseInt(testprocedureId),
                Test_Priority: $scope.selectedPriority,
                Type: $scope.selectedType,
                Title: $scope.title,
                Description: $scope.description,
                Preconditions: $scope.preconditions,
                Expected_Result: $scope.expectedResult,
                Creation_Date: $scope.date
            };

            TestCaseService.Save($scope.TestCase, function (testcase) {
                $scope.testcase = testcase;
                $scope.idLast = testcase.Test_Case_Id;
                createRequirementsTest(testcase.Test_Case_Id, testcase);
                $scope.manageTags(testcase.Test_Case_Id);
                ngToast.success({
                    content: 'Test Case Added Successfully'
                });

            }, function (error) {
                ngToast.danger({
                    content: 'Error Adding'
                });
            });
        }

        function createRequirementsTest(id, testcase) {
            updateNumber(testcase, id);
        }

        function updateNumber(testcase, id) {
            if (tcCopy) {
                testprocedureId = reqId;
            }

            if ($scope.action == "Add") {
                testprocedureId = reqId;
            }


            TestCaseService.UpdateNumber(testcase, testprocedureId, function (data) {
                //When the tc_number was updated, then its going to insert the steps.

                $scope.steps.forEach(function (step) {
                    step.Test_Case_Id = id;
                });
                $scope.reorderSteps();
                var NormalSave = 1;
                StepService.SaveArray($scope.steps, projectId, NormalSave, function (data) {
                    $scope.redirectToList();
                }, function (error) { });

                $scope.requirementsAssigned.forEach(function (req) {
                    if (req.flag == "Added") {
                        $scope.RequirementsTest = {
                            Id: 1,
                            Requirement_Id: req.Id,
                            Test_Case_Id: id
                        };
                        RequirementsTestService.Save($scope.RequirementsTest, function (data) {
                        }, function (response) {
                            ngToast.danger({
                                content: 'Error Requirements Test'
                            });
                        });
                    }
                });

                TestCaseService.AddChangeLog(id, function (data) {

                });

                $scope.ManageAttachments(testcase);

            }, function (response) {
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

        function GetAllSteps(id) {
            StepService.GetForTestCase(id, function (data) {

                $scope.steps = data;
                $scope.steps.forEach(function (step) {
                    step.flag = "Original";
                });
                localStorage.setItem("steps", JSON.stringify($scope.steps));
            }, function (error) {
            });


        }

        function GetAllRelations(id) {
            var exists = false;
            $scope.requirementsAssigned = [];
            RequirementsTestService.GetTestCaseRelations(id, function (data) {
                data.forEach(function (relation) {
                    RequirementService.Get(relation.Requirement_Id, function (data1) {
                        if (tcCopy) {
                            data1.flag = "Added";
                        } else {
                            data1.flag = "Original";
                        }
                        var elements = $scope.requirementsAssigned;
                        elements.forEach(function (req) {
                            if (req.Requirement_Id == data1.Id) {
                                exists = true;
                            }
                        });
                        if (!exists) {
                            $scope.requirementsAssigned.push(data1);
                        }
                    });
                });
            });
        }

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

        function updateTestCase() {
            $scope.TestCase = {
                $id: count++,
                tc_number: $scope.tc,
                Test_Case_Id: parseInt(testprocedureId),
                Test_Priority: $scope.selectedPriority,
                Title: $scope.title,
                Description: $scope.description,
                Type: $scope.selectedType,
                Preconditions: $scope.preconditions,
                Test_Case_Creator: $scope.test_case_creator,
                Expected_Result: $scope.expectedResult,
                Creation_Date: $scope.date,
                Status: $scope.status
            };
            TestCaseService.Update($scope.TestCase, function (testcase) {

                $scope.reorderSteps();
                var NormalSave = 1;
                var TypeOfEvidence = 2;
                StepService.UpdateArray($scope.steps, projectId, NormalSave, $scope.TestCase.Test_Case_Id, TypeOfEvidence, function (data) { }, function (error) { });

                $scope.manageTags($scope.TestCase.Test_Case_Id);
                $scope.requirementsAssigned.forEach(function (req) {
                    if (req.flag == "Added") {
                        $scope.RequirementsTest = {
                            Id: 1,
                            Requirement_Id: req.Id,
                            Test_Case_Id: testprocedureId
                        };
                        RequirementsTestService.Save($scope.RequirementsTest, function (data) {
                        }, function (response) {
                            ngToast.danger({
                                content: 'Error Requirements Test'
                            });
                        });
                    } else if (req.flag == "Removed") {

                        RequirementsTestService.DeleteTestCase(req.Id, testprocedureId, function (data) {
                        }, function (response) {
                            ngToast.danger({
                                content: 'Error Requirements Test'
                            });
                        });

                    }
                });
                if (testprocedureId != null && testprocedureId != 0) {
                    TestCaseService.AddChangeLog(testprocedureId, function (data) {
                    });
                }

                ngToast.success({
                    content: 'Test Case Update Successfully'
                });

                $scope.ManageAttachments($scope.TestCase);

                localStorage.removeItem('steps');
                localStorage.removeItem('rt');
                localStorage.removeItem('tags');
                localStorage.removeItem('data');
                localStorage.removeItem('tpstps');
                $scope.redirectToList();
            }, function (error) { });

        }
        function GetProject(reqId) {
            TestCaseService.GetProjectRequirement(reqId, function (data) {
                $scope.projectName = data.Name;
                $scope.projectId = data.Id;
                RequirementService.GetProject(data.Id, function (data) {
                    $scope.requirements = data;
                });
            });

        }

        function GetRequirement(reqId) {
            RequirementService.Get(reqId, function (data) {
                $scope.requirementName = data.req_number;
                $scope.requirementId = data.Id;
                if (!tcCopy) {
                    if ($scope.action == 'Add' && $scope.requirementsAssigned.length == 0) {
                        data.flag = "Added";
                        $scope.requirementsAssigned.push(data);
                    }
                }
            });
        }

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: $scope.projectId });
        }

        $scope.requirementDetails = function () {
            $state.go('requirementDetails', { projecId: projectId, reqId: $scope.requirementId });
        }

        $scope.manageTags = function (testCaseId) {
            $scope.tagsAux.forEach(function (tag) {
                if (tcCopy) {
                    if (tag.flag == "Original") {
                        TagService.Save(tag, testCaseId, 0, 0, 0, function (data) { }, function (error) { });
                    }
                }
                if (tag.flag == "Added") {
                    tag.Project_Id = $scope.projectId;
                    TagService.Save(tag, testCaseId, 0, 0, 0, function (data) { }, function (error) { });
                } else if (tag.flag == "Removed") {
                    TagService.Delete(tag.id, testCaseId, 0, 0, 0, function (data) { }, function (error) { });
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
                                EntityAction: 3,
                                EntityId: data.Test_Case_Id,
                                projectId: projectId,
                                EntityNumber: data.Test_Case_Id,
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

    }])
})();