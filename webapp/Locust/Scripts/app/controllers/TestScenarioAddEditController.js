(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('TestScenarioAddEditController', ['$scope', '$state', 'TestScenarioService', 'AccessService', 'StepService', 'RequirementsTestService', 'RequirementService', 'TagService', 'TestSuplementalService', 'ProcedureSuplementalService', 'ngToast', 'TypesOfTestService', 'AttachmentService', '$http', 'ValidationService', function ($scope, $state, TestScenarioService, AccessService, StepService, RequirementsTestService, RequirementService, TagService, TestSuplementalService, ProcedureSuplementalService, ngToast, TypesOfTestService, AttachmentService, $http, ValidationService) {

        $scope.action = $state.current.data.action;
        $scope.currentPageREQ1 = 1;
        $scope.pageSizeREQ = 5;
        $scope.currentPageREQ = 1;
        $scope.pageSize = 10;
        var testScenarioId = $state.params.id;
        var reqId = $state.params.reqId;
        var tsCopy = $state.current.data.isCopy;
        var projectId = $state.params.projectId;
        $scope.isExpectedResult = false;
        HasAccess();
        $scope.StepToEdit = {};
        $scope.StepToEdit.action = "";
        $scope.typesOfTest = [];
        var lastStep = {};
        $scope.saved = false;
        $scope.tagsAux = [];
        $scope.stepsaux = [];
        $scope.FilesSelected = [];
        $scope.FilestoDisplay = [];
        $scope.priorityOptions = ['Low', 'Normal', 'Medium', 'High', 'Urgent'];
        $scope.tagsSupList = [];
        var count = 0;
        $scope.customStatus = ['', '', '', ''];
        $scope.customStatusLine = ['normal', 'normal', 'normal', 'normal'];
        $scope.isVisible = [false, false, false, false];
        $scope.from = 0;
        $scope.saveBtn = false;
        Stages();
        var index = 1;
        

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

        function step1() {
            $scope.isVisible[0] = true;
            $scope.customStatus[0] = 'active';
            $scope.customStatusLine[0] = 'active';
        }

        function scrollToBottom() {
            var elmnt = document.getElementById("stepsTable");
            elmnt.scrollTop = elmnt.scrollHeight;
        }


        $scope.RemoveAllSteps = function () {
            $scope.steps = [];
        }

        $scope.download = function () {
            var fileName = "Steps.txt";
            var text = ""
            var stepsOrder = $scope.steps.sort((a, b) => Number(a.number_steps) - Number(b.number_steps));
            stepsOrder.forEach(function (element) {
                if (element.type == 'Expected Result') {
                    text = text + "▼" + element.action + "▲\n";
                } else {
                    text = text + element.action + "▲\n";
                }
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

        $scope.CurrentUserRole = "";
        function GetTypes() {
            TypesOfTestService.Get(function (data) {
                $scope.typesOfTest = data;
            },
                function (err) {
                });
        }

        $scope.Options = [];
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

        $scope.changeOrder = function () {
            var table = document.getElementById("stepsTable");
            for (var i = 0, row; row = table.rows[i]; i++) {
                if (i != 0) {
                    for (var j = 0; j < $scope.stepsaux.length; j++) {

                        if ($scope.stepsaux[j].Id == row.cells[1].innerText) {
                            if ($scope.stepsaux[j].flag == "Original") {
                                if ($scope.stepsaux[j].number_steps != parseInt(row.cells[0].innerText)) {
                                    $scope.stepsaux[j].flag = "Updated";
                                }
                            }
                            $scope.stepsaux[j].number_steps = parseInt(row.cells[0].innerText);
                        }
                        if ($scope.stepsaux[j].flag == "Removed") {
                            $scope.stepsaux[j].number_steps = 0;
                        }
                    }
                }
            }
        }


        $scope.reorderSteps = function () {
            var table = document.getElementById("stepsTable");
            for (var i = 0, row; row = table.rows[i]; i++) {
                if (i != 0) {
                    $scope.steps.forEach(function (step) {
                        if (step.Id == row.cells[1].innerText) {
                            step.number_steps = parseInt(row.cells[0].innerText);
                        }
                    });

                }
            }
        }

        $scope.getLastStep = function () {
            for (var i = 0; i < $scope.steps.length; i++) {
                if (i > 0) {
                    if ($scope.steps[i - 1].number_steps > $scope.steps[i].number_steps) {
                        lastStep = $scope.steps[i];
                    }
                }
            }
        }

        function GetValidations() {
            ValidationService.GetFilesValidation(function (data) {
                $scope.Validations = data;
            }, function (error) {

            })
        };

        $scope.goStp = function (step) {
            var string = step.action.substring(step.action.indexOf("STP_"), step.action.length);
            $scope.GetSuplementals();
            $scope.suplementals.forEach(function (sup) {
                if (sup.stp_number == string.trim()) {
                    var url = $state.href('suplementalDetails', { projectId: projectId, id: sup.Test_Suplemental_Id });
                    window.open(url, '_blank', { focus: true });
                }
            });
        }

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
                        if (tsCopy) {
                            GetTSCopy();
                        }
                        GetTestScenario();
                        GetRole();
                        GetValidations();
                    }
                }, function (error) {
                });
            } else {
                AccessService.HasAccess('TS', projectId, reqId, 0, 0, 0, testScenarioId, function (data) {
                    if (!data.HasPermission) {
                        $state.go('Projects').then(function () {
                            ngToast.danger({
                                content: 'You do not have access to the requested page.'
                            });
                        });
                    } else {
                        GetPaginationElements();
                        GetRole();
                        GetTypes();

                        GetTestScenario();
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
            }, function (error) {

            });
        }

        function GetTSCopy() {
            if (tsCopy != null) {
                if (tsCopy) {
                    TestScenarioService.Get(testScenarioId, function (data) {
                        $scope.title = "[COPY] " + data.Title;
                        $scope.description = data.Description;
                        $scope.preconditions = data.Preconditions;
                        $scope.selectedPriority = data.Test_Priority;
                        $scope.date = data.Creation_Date;
                        $scope.selectedType = data.Type;
                        $scope.status = data.Status;
                        $scope.note = data.Note;
                        $scope.test_scenario_creator = data.Test_Scenario_Creator;
                        GetAllSteps(testScenarioId);
                        GetAllTags(testScenarioId);
                        GetRelations(testScenarioId);
                        GetAllRelations(testScenarioId);
                        $scope.suplementals = [];
                        $scope.GetSuplementals();
                        GetProject(reqId);
                        GetRequirement(reqId);
                    }, function (error) {

                    });

                }
            }
        }

        function GetTestScenario() {
            var value = localStorage.getItem("steps");
            $scope.steps = angular.fromJson(value);
            var vData = localStorage.getItem("data");
            var data1 = angular.fromJson(vData);
            var stpData = localStorage.getItem("tpstps");
            $scope.tpstp = angular.fromJson(stpData);
            var rData = localStorage.getItem("rt");
            $scope.requirementsAssigned = angular.fromJson(rData);
            if ($scope.action == 'Edit') {
                TestScenarioService.Get(testScenarioId, function (data) {
                    if (data1 == null) {
                        $scope.ts = data.ts_number;
                        $scope.title = data.Title;
                        $scope.description = data.Description;
                        $scope.preconditions = data.Preconditions;
                        $scope.selectedPriority = data.Test_Priority;
                        $scope.selectedType = data.Type;
                        $scope.date = data.Creation_Date;
                        $scope.status = data.Status;
                        $scope.note = data.Note;
                        $scope.test_scenario_creator = data.Test_Scenario_Creator;
                    } else {
                        $scope.ts = data1.ts;
                        $scope.title = data1.title;
                        $scope.description = data1.description;
                        $scope.preconditions = data1.preconditions;
                        $scope.selectedPriority = data1.selectedPriority;
                        $scope.selectedType = data1.Type;
                        $scope.test_scenario_creator = data1.test_scenario_creator;
                        $scope.date = data1.date;
                        $scope.status = data1.status;
                        $scope.note = data1.note;
                    }

                    if ($scope.steps == null) {
                        GetAllSteps(testScenarioId);

                    }
                    if ($scope.tagsBackup == null) {
                        GetAllTags(testScenarioId);
                    }

                    if ($scope.tpstp == null) {
                        GetRelations(testScenarioId);
                    }

                    if ($scope.requirementsAssigned == null) {
                        $scope.requirementsAssigned = [];
                        GetAllRelations(testScenarioId);
                    }

                    if ($scope.suplementals == null) {
                        $scope.suplementals = [];
                        $scope.GetSuplementals();
                    }

                }, function (error) {
                });
                GetProject(reqId);
                GetRequirement(reqId);

                AttachmentService.GetAttachment(4, testScenarioId, function (data) {

                    $scope.FilestoDisplay = data;

                    $scope.FilestoDisplay.forEach(file => {
                        file.flag = 'Original';
                        file.show = $scope.ShowImage(file.Extention);
                    });
                    index = $scope.FilestoDisplay.length + 1;
                    $scope.CheckNames();
                }, function (err) {
                });


            } else if ($scope.action == 'Add') {
                if (data1 != null) {
                    $scope.ts = data1.ts;
                    $scope.title = data1.title;
                    $scope.description = data1.description;
                    $scope.preconditions = data1.preconditions;
                    $scope.selectedType = data1.Type;
                    $scope.note = data1.note;
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
                if ($scope.tpstp == null) {
                    $scope.tpstp = [];
                }
                if ($scope.requirementsAssigned == null) {
                    $scope.requirementsAssigned = [];
                }
                if ($scope.suplementals == null) {
                    $scope.suplementals = [];

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

        function GetAllTags(testScenarioId) {
            TagService.GetTestScenarioTags(testScenarioId, function (tags) {
                $scope.tagsBackup = tags;
                $scope.tags = tags;
                $scope.tagsBackup.forEach(function (tag) {
                    tag.flag = "Original";
                });
                $scope.tags.forEach(function (tag) {
                    tag.flag = "Original";
                });
                localStorage.setItem("tags", JSON.stringify($scope.tagsBackup));

                for (var i = 0; i < $scope.tags.length; i++) {
                    $scope.tagsAux[i] = $scope.tags[i];
                }
            }, function (error) {

            });

        }

        $scope.GetSuplementals = function () {
            $scope.supDescription = null;
            $scope.selectedSuplemental = null;
            $scope.filter = "";
            $scope.stepsSuplementals = [];
            $scope.tagsSupList = [];

            TestScenarioService.GetProjectRequirement(reqId, function (data) {
                $scope.projectId = data.Id;

                TestSuplementalService.GetAllSTPByProject($scope.projectId, function (res) {
                    $scope.suplementals = res;
                });

            });
        }

        $scope.selectSTP = function () {

            if ($scope.selectedSuplemental != null && $scope.supDescription != null) {
                var desc = $scope.supDescription + " " + $scope.selectedSuplemental.stp_number;
                var repeat = false;
                $scope.tpstp.forEach(function (relation) {
                    if (relation.suplemental == $scope.selectedSuplemental.Test_Suplemental_Id) {
                        if (relation.flag != "Removed") {
                            repeat = true;
                        }

                    }
                });
                if (repeat == false) {
                    var tpts = {
                        suplemental: $scope.selectedSuplemental.Test_Suplemental_Id,
                        flag: 'Added'
                    }
                    $scope.tpstp.push(tpts);
                }
                $scope.supDescription = "";
                var high = 0;
                $scope.steps.forEach(function (step) {
                    if (step.flag != "Removed") {
                        if (step.number_steps > high) {
                            high = step.number_steps;
                        }
                    }
                });
                var Step = {
                    Id: count++,
                    Test_Procedure_Id: '',
                    type: 'Step',
                    number_steps: high + 1,
                    action: desc,
                    creation_date: "",
                    flag: "Added"
                }

                $scope.steps.push(Step);
                localStorage.setItem("rt", JSON.stringify($scope.requirementsAssigned));
            } else {
                ngToast.danger({
                    content: 'Step description is empty or supplemental test not selected'
                });
            }
        };

        function GetAllRelations(id) {
            var exists = false;
            $scope.requirementsAssigned = [];
            RequirementsTestService.GetTestScenarioRelations(id, function (data) {
                data.forEach(function (relation) {
                    RequirementService.Get(relation.Requirement_Id, function (data1) {
                        if (tsCopy) {
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

        $scope.suplementalChange = function (suplemental) {
            $scope.selectedSuplemental = suplemental;
        };

        $scope.cancel = function () {
            $scope.redirectToList();
        };

        $scope.redirectToList = function () {
            $state.go('requirementDetails', { projectId: projectId, id: reqId });

        };

        $scope.AddUpdate = function (form) {
            $scope.saved = true;

            let validSteps = $scope.validateSteps();

            if (validSteps != "") {
                ngToast.danger({
                    content: validSteps
                });
                $scope.saved = false
                return;
            }

            var cont = 0;
            if (form.$valid) {
                $scope.steps.forEach(function (step) {
                    if (step.flag != "Removed") {
                        cont = cont + 1;
                    }
                })

                if ($scope.steps.length > 0 && cont > 0) {
                    if ($scope.tags.length > 0) {
                        if ($scope.action == "Add") {
                            createTestCase();
                        } else {
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

        function createTestCase() {
            $scope.TestScenario = {
                Test_Scenario_Id: parseInt(testScenarioId),
                Test_Priority: $scope.selectedPriority,
                Title: $scope.title,
                Description: $scope.description,
                Note: $scope.note,
                Type: $scope.selectedType,
                Preconditions: $scope.preconditions,
                Creation_Date: $scope.date
            };

            TestScenarioService.Save($scope.TestScenario, function (testscenario) {
                $scope.testscenario = testscenario;
                $scope.idLast = testscenario.Test_Scenario_Id;
                createRequirementsTest(testscenario.Test_Scenario_Id, testscenario);

            }, function (error) {
                ngToast.danger({
                    content: 'Error Adding'
                });
            });
            $scope.saved = false

        }

        function createRequirementsTest(id, testscenario) {
            updateNumber(testscenario, id);

        }

        function updateNumber(testscenario, id) {
            if (tsCopy) {
                testScenarioId = reqId;
            }
            if ($scope.action == "Add") {
                testScenarioId = reqId;
            }


            TestScenarioService.UpdateNumber(testscenario, testScenarioId, function (data) {


                $scope.manageTags(id);


                $scope.steps.forEach(function (step) {
                    step.Test_Scenario_Id = id;
                });
                $scope.reorderSteps();

                var SaveWithRelationWithTS = 3;
                StepService.SaveArray($scope.steps, projectId, SaveWithRelationWithTS, function (data) {
                    $scope.redirectToList();
                }, function (error) { });



                $scope.requirementsAssigned.forEach(function (req) {
                    if (req.flag == "Added") {
                        $scope.RequirementsTest = {
                            Id: 1,
                            Requirement_Id: req.Id,
                            Test_Scenario_Id: id
                        };
                        RequirementsTestService.Save($scope.RequirementsTest, function (data) {

                        }, function (response) {
                            ngToast.danger({
                                content: 'Error Requirements Test'
                            });
                        });
                    }
                });

                TestScenarioService.AddChangeLog(id, function (data) {

                });
                $scope.ManageAttachments($scope.testscenario);
                ngToast.success({
                    content: 'Test Scenario Added Successfully'
                });


            }, function (response) {
            });
        }

        $scope.updateStep = function (number, step) {
            $scope.StepToEdit = step;
            $scope.originalAction = step.action;
            $scope.stepDescription = $scope.StepToEdit.action;
            if (step.type == 'Expected Result') {

                document.getElementById("btnExpectedResult").style.display = "none";
                document.getElementById("addStep").style.display = "none";
                document.getElementById("btnEditExpected").style.display = "block";

            } else {
                document.getElementById("addStep").style.display = "none";
                document.getElementById("editStep").style.display = "block";
                document.getElementById("btnExpectedResult").style.display = "none";
            }

            /*   $scope.steps.forEach(function (stepp) {
                   if (stepp.number_steps == number) {
                       $scope.stepEdit = stepp;
                       $scope.stepDescription = $scope.stepEdit.action;
                   }
               });*/

        }

        $scope.RestoreStepButtons = function () {
            document.getElementById("btnExpectedResult").style.display = "block";
            document.getElementById("addStep").style.display = "block";
            document.getElementById("editStep").style.display = "none";
            document.getElementById("btnEditExpected").style.display = "none";
        }

        $scope.editSteps = function () {

            if ($scope.stepDescription != '' && typeof $scope.stepDescription != 'undefined') {

                $scope.StepToEdit.action = $scope.stepDescription;


                var aux2 = ($scope.stepDescription.split(new RegExp("STP_", "gi")).length - 1);
                if (aux2 > 1) {
                    ngToast.danger({
                        content: 'The Key Word "STP_" just can be one time per step'
                    });
                    $scope.stepDescription = '';
                    return;
                }


                var aux = $scope.stepDescription.indexOf("STP_");
                if (aux != -1) {
                    var string = $scope.stepDescription.substring($scope.stepDescription.indexOf("STP_"), $scope.stepDescription.length);
                    var aux2 = string.indexOf(" ");
                    if (aux2 != -1) {
                        ngToast.danger({
                            content: 'The STP number must be at the end of the step description'
                        });
                        $scope.stepDescription = '';
                        return;
                    }

                    var sts = string.split('STP_');
                    if (typeof (sts[1]) != 'undefined') {
                        if (!sts[1].match(/^\d+$/)) {
                            ngToast.danger({
                                content: 'The STP number must be at the end of the step description'
                            });
                            $scope.stepDescription = '';
                            return;
                        }
                    }

                }
                var change = true;

                $scope.steps.forEach(function (step) {
                    if (step.Id == $scope.StepToEdit.Id) {

                        if (step.type == "Expected Result" && aux != -1) {
                            ngToast.danger({
                                content: 'An STP Cannot be added as Expected Result'
                            });
                            change = false;
                        }

                        if (change) {
                            step.action = $scope.stepDescription;
                            $scope.StepToEdit = {};
                            $scope.StepToEdit.action = "";

                        } else {
                            step.action = $scope.originalAction;
                        }



                        if (step.type == 'Expected Result') {
                            $scope.RestoreStepButtons();
                        } else {
                            $scope.RestoreStepButtons();
                        }
                        
                    }

                });
                $scope.stepDescription = '';
                $scope.RestoreStepButtons();
            } else {
                ngToast.danger({
                    content: 'Step description is empty'
                });
                $scope.RestoreStepButtons();
            }

        }

        function findSTP(action) {

            var aux = action.indexOf("STP_");
            let supId;
            if (aux != -1) {
                var string = action.substring(action.indexOf("STP_"), action.length);

                TestSuplementalService.GetByNumber(projectId, string, r => {
                    supId = r.Test_Suplemental_Id;
                    RemoveSTP(supId);
                }, err => {
                });
            }

        }

        function RemoveSTP(supId) {
            for (var i = $scope.tpstp.length - 1; i >= 0; i--) {
                if ($scope.tpstp[i].suplemental == supId && $scope.tpstp[i].flag == 'Added') {
                    $scope.tpstp.splice(i, 1);
                } else {
                    $scope.tpstp[i].flag = "Removed";
                }

            }
        }

        $scope.addStep = function (expected) {
            if (expected && $scope.steps.length == 0) {
                ngToast.danger({
                    content: 'You should add a step first'
                });
                return false;
            }

            if (expected && ($scope.stepDescription.includes("▼") || $scope.stepDescription.includes("▲"))) {
                ngToast.danger({
                    content: 'To add multiple steps please click on add step'
                });
                return false;
            }

            if ($scope.stepDescription != '' && typeof $scope.stepDescription != 'undefined') {

                if ($scope.stepDescription.includes("▲")) {
                    var Steps = $scope.stepDescription.split("▲");
                    var execute = true;
                    Steps.forEach(function (st) {
                        if (execute) {
                            if (st.trim() != "") {
                                if (st.includes("▼")) {
                                    var substr = st.split("▼");
                                    if (substr.length > 2) {
                                        ngToast.danger({
                                            content: 'Please remove multiple expected result simbols between steps'
                                        });
                                        execute = false;
                                    } else {
                                        execute = $scope.AddStepToArray(substr[1].trimLeft(), true);
                                    }

                                } else {
                                    execute = $scope.AddStepToArray(st.trimLeft(), false);
                                }

                            }
                        }


                    });
                    $scope.stepDescription = '';
                } else {
                    var execute = $scope.AddStepToArray($scope.stepDescription, expected);
                    $scope.stepDescription = '';

                }
                scrollToBottom();

                $scope.stepDescription = '';
            } else {
                ngToast.danger({
                    content: 'Step description is empty'
                });
            }

        };

        $scope.AddStepToArray = function (Description, expected) {
            var aux2 = (Description.split(new RegExp("STP_", "gi")).length - 1);
            if (aux2 > 1) {
                ngToast.danger({
                    content: 'The Key Word "STP_" just can be one time per step'
                });
                return false;
            }

            var aux = Description.indexOf("STP_");
            if (aux != -1) {
                var string = Description.substring(Description.indexOf("STP_"), Description.length);
                var aux2 = string.indexOf(" ");
                if (aux2 != -1) {
                    ngToast.danger({
                        content: 'The STP number must be at the end of the step description'
                    });
                    return false;
                }


                var sts = string.split('STP_');
                if (typeof (sts[1]) != 'undefined') {
                    if (!sts[1].toString().match(/\d+/g)) {
                        ngToast.danger({
                            content: 'The STP number must be at the end of the step description'
                        });
                        return false;
                    }
                }

                if (expected) {
                    ngToast.danger({
                        content: "You can't add an STP as Expected Result"
                    });
                    return false;
                }


            }

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

            if (expected) {
                Step.type = 'Expected Result'
            }

            if (aux != -1) {
                Step.type = 'STP';
            }

            $scope.steps.push(Step);
            return true;

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

                        if (step.Id == row.cells[1].innerText) {
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

        function GetRelations(id) {
            $scope.tpstp = [];
            ProcedureSuplementalService.GetForTS(id, function (rel) {
                var relations = rel;

                relations.forEach(function (stp) {
                    var aux = {
                        flag: "Original",
                        suplemental: stp.Test_Suplemental_Id
                    }
                    $scope.tpstp.push(aux);

                });
                localStorage.setItem("tpstps", JSON.stringify($scope.tpstp));
            });
        }

        function GetAllSteps(id) {
            StepService.GetForTestScenario(id, function (data) {

                $scope.steps = data;
                $scope.steps.forEach(function (step) {
                    step.flag = "Original";

                    if (step.type == null) {
                        step.type = "Step";
                    }
                });
                localStorage.setItem("steps", JSON.stringify($scope.steps));
            }, function (error) {
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
                    localStorage.setItem("tags", JSON.stringify($scope.tagsBackup));
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
            $scope.TestScenario = {
                $id: count++,
                ts_number: $scope.ts,
                Test_Scenario_Id: parseInt(testScenarioId),
                Test_Priority: $scope.selectedPriority,
                Note: $scope.note,
                Title: $scope.title,
                Type: $scope.selectedType,
                Description: $scope.description,
                Preconditions: $scope.preconditions,
                Test_Scenario_Creator: $scope.test_scenario_creator,
                Creation_Date: $scope.date,
                Status: $scope.status
            };
            TestScenarioService.Update($scope.TestScenario, function (testscenario) {

                $scope.reorderSteps();
                var SaveWithRelationWithTP = 3;
                var TypeOfEvidence = 3;
                StepService.UpdateArray($scope.steps, projectId, SaveWithRelationWithTP, $scope.TestScenario.Test_Scenario_Id, TypeOfEvidence, function (data) { }, function (error) { });

                $scope.manageTags(testScenarioId);


                $scope.requirementsAssigned.forEach(function (req) {
                    if (req.flag == "Added") {
                        $scope.RequirementsTest = {
                            Id: 1,
                            Requirement_Id: req.Id,
                            Test_Scenario_Id: testScenarioId
                        };
                        RequirementsTestService.Save($scope.RequirementsTest, function (data) {

                        }, function (response) {
                            ngToast.danger({
                                content: 'Error Requirements Test'
                            });
                        });
                    } else if (req.flag == "Removed") {

                        RequirementsTestService.DeleteTestScenario(req.Id, testScenarioId, function (data) {
                        }, function (response) {
                            ngToast.danger({
                                content: 'Error Requirements Test'
                            });
                        });

                    }
                });

                TestScenarioService.AddChangeLog(testScenarioId, function (data) {
                });

                ngToast.success({
                    content: 'Test Scenario Update Successfully'
                });

                localStorage.removeItem('steps');
                localStorage.removeItem('rt');
                localStorage.removeItem('tags');
                localStorage.removeItem('data');
                localStorage.removeItem('tpstps');
                $scope.saved = false

                $scope.redirectToList();
                $scope.ManageAttachments($scope.TestScenario);
            }, function (error) { });

        }

        function GetProject(reqId) {
            TestScenarioService.GetProjectRequirement(reqId, function (data) {
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
                if (!tsCopy) {
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
            $state.go('requirementDetails', { projectId: projectId, id: reqId });
        }

        $scope.manageTags = function (testScenarioId) {
            $scope.tagsAux.forEach(function (tag) {
                tag.Project_Id = $scope.projectId;
                if (tsCopy) {
                    if (tag.flag == "Original") {
                        TagService.Save(tag, 0, testScenarioId, 0, 0, function (data) { }, function (error) { });
                    }
                }
                if (tag.flag == "Added") {
                    TagService.Save(tag, 0, testScenarioId, 0, 0, function (data) { }, function (error) { });
                } else if (tag.flag == "Removed") {
                    TagService.Delete(tag.id, 0, testScenarioId, 0, 0, function (data) { }, function (error) { });
                }
            });
        }

        $scope.validateSteps = function () {
            $scope.reorderSteps();
            let isValid = "";
            let firstStep;

            var table = document.getElementById("stepsTable");
            let index = table.rows.length - 1;

            $scope.steps.forEach(function (step) {
                if (step.number_steps == index) {
                    lastStep = step;
                }
            });
            $scope.steps.sort(function (a, b) {
                var keyA = a.number_steps,
                    keyB = b.number_steps;
                return keyA - keyB;
            });


            for (var i = 0; i < $scope.steps.length; i++) {
                if ($scope.steps[i].number_steps == 1) {
                    firstStep = $scope.steps[i];
                    break;
                }

            }

            for (var i = 0; i < $scope.steps.length; i++) {
                if (i != 0) {
                    if ($scope.steps[i].type == $scope.steps[i - 1].type && $scope.steps[i].type == "Expected Result") {
                        isValid = "Please add a Step before an Expected Result";
                    }
                }
            }

            if (lastStep.type != 'Expected Result') {
                isValid = "The last element must be a Expected Result!";
            }

            if (firstStep.type == 'Expected Result') {
                isValid = "Please add a Step before an Expected Result";
            }
            return isValid;
        }

        /*Attachments*/
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
                                EntityAction: 4,
                                EntityId: data.Test_Scenario_Id,
                                projectId: projectId,
                                EntityNumber: data.Test_Scenario_Id,
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