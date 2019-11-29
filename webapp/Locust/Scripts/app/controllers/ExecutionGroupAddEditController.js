(function () {
    'use strict';
    var app = angular.module(appName);
    app.controller('ExecutionGroupAddEditController', ['$scope', '$state', '$window', 'ExecutionGroupService', 'AccessService', 'RequirementService', 'ScriptsGroupService', 'ScriptsService', 'ngToast', 'ExecutionTestService', 'TestEnvironmentService', 'ProjectService', 'TypesOfTestService', 'RunnerService', '$timeout', function ($scope, $state, $window, ExecutionGroupService, AccessService, RequirementService, ScriptsGroupService, ScriptsService, ngToast, ExecutionTestService, TestEnvironmentService, ProjectService, TypesOfTestService, RunnerService, $timeout) {
        var projectId = $state.params.projectId;
        $scope.IsAutomated = $state.current.data.IsAutomated;
        var executionId = $state.params.executionId;
        $scope.action = $state.current.data.action;
        HasAccess();
        $scope.hasPermissionProject = false;
        $scope.requirementsArray = [];
        $scope.testCasesArray = [];
        $scope.testProceduresArray = [];
        $scope.requirementsAssigned = [];
        $scope.testScenariosArray = [];
        $scope.TE = [];
        $scope.te = { selectedItems: '' };
        $scope.ListOfReqSprint = [];
        $scope.TP = 0;
        $scope.TS = 0;
        $scope.TC = 0;
        $scope.runner = { selectedItems: '' };
        $scope.activate = false;
        $scope.filtered = false;
        $scope.Reqfiltered = false;
        $scope.filterSelected = '';
        $scope.ListOfReqSprint = [];

        $scope.projectDetails = function () {
            $state.go('projectDetails', { id: projectId });
        };

        $scope.executionGroup = function () {
            $state.go('executionGroups', { projectId: projectId });
        };

        console.log($scope.IsAutomated);
        function GetNumberOfEvidence() {
            $scope.TP = $scope.testEvidenceToSaveArray.filter(test => test.Evidence == 'TP').length;
            $scope.TC = $scope.testEvidenceToSaveArray.filter(test => test.Evidence == 'TC').length;
            $scope.TS = $scope.testEvidenceToSaveArray.filter(test => test.Evidence == 'TS').length;

            if ($scope.IsAutomated == 0) {
                if ($scope.TP == 0 && $scope.TC == 0 && $scope.TS == 0) {
                    $scope.Strategy = "TBD";
                }

                if ($scope.TP >= 1 && $scope.TC == 0 && $scope.TS == 0) {
                    $scope.Strategy = "Only TP";
                }

                if ($scope.TC >= 1 && $scope.TP == 0 && $scope.TS == 0) {
                    $scope.Strategy = "Only TC"
                }

                if ($scope.TS >= 1 && $scope.TP == 0 && $scope.TC == 0) {
                    $scope.Strategy = "Only TS"
                }

                if ($scope.TS >= 1 && $scope.TP >= 1 || $scope.TC >= 1) {
                    $scope.Strategy = "Hybrid";
                }

            } else {
                $scope.Strategy = "Automated";
            }

        };

        var loadTime = 10000, //Load the data every second
            errorCount = 0, //Counter for the server errors
            loadPromise; //Pointer to the promise created by the Angular $timout service


        function GetData() {
            GetAllRunners();
            errorCount = 0;
            nextLoad();
        }


        var cancelNextLoad = function () {
            $timeout.cancel(loadPromise);
        };


        var nextLoad = function (mill) {
            mill = mill || loadTime;

            //Always make sure the last timeout is cleared before starting a new one
            cancelNextLoad();
            loadPromise = $timeout(GetData, mill);
        };

        var cancelNextLoad = function () {
            $timeout.cancel(loadPromise);
        };

        //Always clear the timeout when the view is destroyed, otherwise it will keep polling
        $scope.$on('$destroy', function () {
            cancelNextLoad();
        });


        function GetAllRunners() {
            if ($scope.IsAutomated == 1) {
                RunnerService.GetFullActives(function (data) {
                    $scope.runners = data;
                }, function (error) {

                });
            }

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
                        $("#loadingModalGroupData").modal({
                            backdrop: "static",
                            keyboard: false,
                            show: true
                        });
                        GetProject();
                        GetTE();
                        GetRequirements();
                        GetRole();
                        GetTypeOfTest();
                        GetData();
                        GetNumberOfEvidence();
                    }
                }, function (error) {
                });
            } else {
                AccessService.HasAccess('ExecutionGroup', projectId, executionId, 0, 0, 0, 0, function (data) {
                    if (!data.HasPermission) {
                        $state.go('Projects').then(function () {
                            ngToast.danger({
                                content: 'You do not have access to the requested page.'
                            });
                        });
                    } else {
                        $("#loadingModalGroupData").modal({
                            backdrop: "static",
                            keyboard: false,
                            show: true
                        });

                        GetProject();
                        GetTE();
                        GetRequirements();
                        GetRole();
                        GetTypeOfTest();
                        GetData();
                    }
                }, function (error) {
                });
            }
        };

        function GetRole() {
            AccessService.GetRole(function (data) {
                $scope.CurrentUserRole = data.Role;
                if (data.Role == 'VBP' || data.Role == 'BA') {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                }
            }, function (error) {

            });
        }

        function GetTypeOfTest() {

            TypesOfTestService.Get(function (data) {
                $scope.Options2 = data;
                $scope.Options2.unshift("All");
                $scope.optionSelected = $scope.Options2[0];
            },
                function (err) {

                });
        }

        function GetProject() {
            ProjectService.Get(projectId, r => {
                $scope.Name = r.Name;
            }, error => {

            });
        }

        function GetRequirements() {
            $scope.RequirementOptions = [];

            RequirementService.GetSprints(projectId, function (sprints) {
                $scope.RequirementOptions = sprints;
                $scope.RequirementOptions.unshift("All");
                $scope.FilterReq = $scope.RequirementOptions[0];
            }, function (err) {
            });

            $scope.requirementsArray = [];
            //if ($scope.IsAutomated == 0) {

            RequirementService.GetProject(projectId, function (data) {
                $scope.requirementsArray = data;
                $scope.requirementsArrayStorage = $scope.requirementsArray;
                LoadEvidence();
            }, function (err) {
            });
            //} else {
            //    ScriptsGroupService.GetAllScriptsGroup(projectId, function (data) {
            //        $scope.requirementsArray = data;
            //        $scope.requirementsArrayStorage = $scope.requirementsArray;
            //        LoadEvidence();
            //    }, function (err) {
            //    });
            //}
        }

        function HasEvidence() {
            // var count = 0;
            //if ($scope.IsAutomated == 0) {
            $scope.requirementsArray.forEach(function (req) {
                var values = $scope.testEvidenceArray.filter(obj => obj.ReqId == req.Id);
                if (values.length == 0) {
                    req.HasEvidence = false;
                } else {
                    req.HasEvidence = true;
                }
                //  count++;
            });

            // $scope.requirementsCount = count;

            //} else {
            //    $scope.requirementsArray.forEach(function (req) {
            //        var values = $scope.testEvidenceArray.filter(obj => obj.ScriptsGroup_Id == req.Id);
            //        if (values.length == 0) {
            //            req.HasEvidence = false;
            //        } else {
            //            req.HasEvidence = true;
            //        }
            //    });
            //}
        }

        function GetTE() {
            TestEnvironmentService.GetActives(projectId, function (data) {
                $scope.TE = data;
            }, function (error) {
            });
        };


        //Edit Method
        function LoadExecutionInformation() {
            if ($state.current.data.action == 'Edit') {
                ExecutionGroupService.Get(executionId, function (data) {
                    $scope.name = data.Name;
                    $scope.description = data.Description;
                    $scope.creation_date = data.Creation_Date;
                    $scope.creator = data.Creator;
                    $scope.IsAutomated = data.isAutomated;
                    if ($scope.IsAutomated) {
                        RunnerService.Get(data.RunnerId, function (data2) {
                            $scope.runner.selectedItems = data2;
                        }, function (error) {

                        });
                    } else {

                        TestEnvironmentService.Get(data.TestEnvironmentId, function (data2) {
                            $scope.te.selectedItems = data2;
                        }, function (error) {

                        });
                    }
                    LoadTestCasesOnEdit();
                }, function (err) { });
            }
        }

        $scope.selectTestByReq = function (req) {
            var values = "";
            //if ($scope.IsAutomated == 0) {
            values = $scope.testEvidenceArray.filter(obj => obj.Id == req.Id);
            //} else {
            //    values = $scope.testEvidenceArray.filter(obj => obj.ScriptsGroup_Id == req.Id);
            //}
            values.forEach(function (element) {
                $scope.SelectEvidenceLocal(element);
            });


            $scope.testEvidenceArrayFilter.forEach(function (element) {
                $scope.SelectEvidenceLocal(element);
            });
        }

        $scope.unselectTestByReq = function (req) {
            //if ($scope.IsAutomated == 0) {
            var values = $scope.testEvidenceArray.filter(obj => obj.ReqId == req.Id);

            values.forEach(function (el) {
                var otherValues = $scope.testEvidenceArray.filter(obj => obj.Identifier_number == el.Identifier_number);
                otherValues.forEach(function (element) {
                    if (element.IsSelected == undefined || element.IsSelected) {
                        element.IsSelected = false;
                        element.Style = 'evidence';
                    }
                    reqIsSelected(element);
                });
            });
            //} else {
            //    var values = $scope.testEvidenceArray.filter(obj => obj.ScriptsGroup_Id == req.Id);
            //    values.forEach(function (element) {
            //        if (element.IsSelected == undefined || element.IsSelected) {
            //            element.IsSelected = false;
            //            element.Style = 'evidence';
            //        }
            //        reqIsSelected(element);
            //    });
            //}
            $scope.testEvidenceArrayFilter.forEach(function (element) {
                if (element.IsSelected == undefined || element.IsSelected) {
                    element.IsSelected = false;
                    element.Style = 'evidence';
                }
                reqIsSelected(element);
            });
        }

        $scope.deleteEvidenceByReq = function (req) {

            //if ($scope.IsAutomated == 0) {

            var values = $scope.testEvidenceArray.filter(obj => obj.ReqId == req.Id);
            values.forEach(function (el) {
                var otherValues = $scope.testEvidenceArray.filter(obj => obj.Identifier_number == el.Identifier_number);
                otherValues.forEach(function (element) {
                    if (element.Added == undefined || element.Added) {
                        element.IsSelected = false;
                        element.Added = false;
                        element.Style = 'evidence';

                        var objIndex = $scope.testEvidenceToSaveArray.findIndex(obj => obj.Element_id == element.Element_id && obj.Identifier_number == element.Identifier_number);
                        $scope.testEvidenceToSaveArray.splice(objIndex, 1);
                    }
                    reqIsSelected(element);
                });
            });



            //} else {
            //    var values = $scope.testEvidenceArray.filter(obj => obj.ScriptsGroup_Id == req.Id);
            //    values.forEach(function (element) {
            //        if (element.Added == undefined || element.Added) {
            //            element.IsSelected = false;
            //            element.Added = false;
            //            element.Style = 'evidence';
            //            var objIndex = $scope.testEvidenceToSaveArray.findIndex(obj => obj.Element_id == element.Element_id && obj.Identifier_number == element.Identifier_number);
            //            $scope.testEvidenceToSaveArray.splice(objIndex, 1);
            //        }
            //        reqIsSelected(element);
            //    });
            //}


            $scope.testEvidenceArrayFilter.forEach(function (element) {
                if (element.Added == undefined || element.Added) {
                    element.IsSelected = false;
                    element.Added = false;
                    element.Style = 'evidence';
                }
                reqIsSelected(element);
            });
        }

        $scope.unselectAll = function () {



            $scope.testEvidenceArray.forEach(function (evidence) {
                evidence.Added = false;
                evidence.IsSelected = false;
                evidence.Style = 'evidence';
            });

            $scope.testEvidenceArrayFilter.forEach(function (evidence) {
                evidence.Added = false;
                evidence.IsSelected = false;
                evidence.Style = 'evidence';
            });


            $scope.requirementsArray.forEach(function (req) {
                reqIsSelectedByReq(req);
            });

            $scope.testEvidenceToSaveArray = [];

            GetNumberOfEvidence();
        };

        $scope.SelectEvidenceLocal = function (ev) {

            //if ($scope.IsAutomated == 0) {

            //Get Coincidences
            var values = $scope.testEvidenceArray.filter(obj => obj.Identifier_number == ev.Identifier_number);
            values.forEach(function (evidence) {
                if (evidence.Added != undefined && evidence.Added) {
                    return;
                }

                if (evidence.IsSelected) {
                    return;

                }

                if (!evidence.IsSelected) {
                    if (evidence.Added != undefined || evidence.Added != true) {
                        evidence.IsSelected = true;
                        evidence.Style = 'evidenceSelected';
                    }
                } else {
                    if (evidence.Added == undefined || evidence.Added == false) {
                        evidence.IsSelected = false;
                        evidence.Style = 'evidence';
                    }
                }
                reqIsSelected(evidence);
            });
            //} else {
            //    if (ev.Added != undefined && ev.Added) {
            //        return;
            //    }

            //    if (ev.IsSelected) {
            //        return;

            //    }

            //    if (!ev.IsSelected) {
            //        if (ev.Added != undefined || ev.Added != true) {
            //            ev.IsSelected = true;
            //            ev.Style = 'evidenceSelected';
            //        }
            //    } else {
            //        if (ev.Added == undefined || ev.Added == false) {
            //            ev.IsSelected = false;
            //            ev.Style = 'evidence';
            //        }
            //    }
            //    reqIsSelected(ev);
            //}
        }

        $scope.Save = function (form) {
            if (form.$valid) {
                if ($scope.IsAutomated == 0) {
                    if ($scope.te.selectedItems.Id == undefined || $scope.name == undefined || $scope.description == undefined) {
                        ngToast.danger({
                            content: 'Please complete all fields correctly'
                        });
                        return;
                    }
                } else {
                    if ($scope.runner.selectedItems == undefined) {
                        ngToast.danger({
                            content: 'Please complete all fields correctly'
                        });
                        return;

                        if ($scope.runner.selectedItems.Id == undefined || $scope.name == undefined || $scope.description == undefined) {
                            ngToast.danger({
                                content: 'Please complete all fields correctly'
                            });
                            return;
                        }
                    }
                }

                var aux = false;
                if ($scope.IsAutomated == 1) {
                    aux = true;
                }
                if ($state.current.data.action == 'Add') {
                    var executionGroup = {
                        Name: $scope.name,
                        Description: $scope.description,
                        ProjectId: projectId,
                        isActive: 1,
                        isAutomated: aux,
                        IsReadyToExecute: 0
                    };
                    if (aux) {
                        executionGroup.RunnerId = $scope.runner.selectedItems.Id;
                    } else {
                        executionGroup.TestEnvironmentId = $scope.te.selectedItems.Id;
                    }

                    if (aux) {
                        RunnerService.Get(executionGroup.RunnerId, function (Runner) {
                            if (Runner.IsConnected == false) {
                                ngToast.danger({
                                    content: 'The runner selected is not longer active, please select another runner'
                                });
                                return;
                            } else {

                                ExecutionGroupService.Save(executionGroup, function (data) {
                                    AddExecutionRelations(data.Execution_Group_Id);
                                    ngToast.success({
                                        content: 'Execution group was created!'
                                    });
                                    $state.go('executionGroups', { projectId: projectId });
                                },
                                    function (err) {
                                        ngToast.danger({
                                            content: 'Execution Group could not be created'
                                        });
                                    });
                            }



                        }, function (error) {
                            ngToast.danger({
                                content: 'An error happend trying to get the runner'
                            });
                        });
                    } else {

                        ExecutionGroupService.Save(executionGroup, function (data) {
                            AddExecutionRelations(data.Execution_Group_Id);
                            ngToast.success({
                                content: 'Execution group was created!'
                            });
                            $state.go('executionGroups', { projectId: projectId });
                        },
                            function (err) {
                                ngToast.danger({
                                    content: 'Execution Group could not be created'
                                });
                            });
                    }



                } else if ($state.current.data.action == 'Edit') {
                    var executionGroup = {
                        Execution_Group_Id: executionId,
                        Name: $scope.name,
                        Description: $scope.description,
                        ProjectId: projectId,
                        Creator: $scope.creator,
                        Creation_Date: $scope.creation_date,
                        isActive: 1,
                        isAutomated: $scope.IsAutomated,
                        IsReadyToExecute: 0
                    };

                    if (aux) {
                        executionGroup.RunnerId = $scope.runner.selectedItems.Id;
                        RunnerService.Get(executionGroup.RunnerId, function (Runner) {
                            if (Runner.IsConnected == false) {
                                ngToast.danger({
                                    content: 'The runner selected is not longer active, please select another runner'
                                });
                                return;
                            } else {

                                ExecutionGroupService.Update(executionGroup, function (success) {
                                    ngToast.success({
                                        content: 'Execution Group has been updated'
                                    });
                                    UpdateExecutionRelations();
                                    // UpdateTestExecutions();
                                    $scope.cancel();
                                },
                                    function (err) {
                                        ngToast.danger({
                                            content: 'Execution Group could not be created'
                                        });
                                    });
                            }



                        }, function (error) {
                            ngToast.danger({
                                content: 'An error happend trying to get the runner'
                            });
                        });
                    } else {
                        executionGroup.TestEnvironmentId = $scope.te.selectedItems.Id;
                        ExecutionGroupService.Update(executionGroup, function (success) {
                            ngToast.success({
                                content: 'Execution Group has been updated'
                            });
                            UpdateExecutionRelations();
                            // UpdateTestExecutions();
                            $scope.cancel();

                        },
                            function (err) {
                                ngToast.danger({
                                    content: 'Execution Group could not be created'
                                });
                            });
                    }
                }
            }
        };

        function UpdateExecutionRelations() {
            let UpdatedRelations = [];
            let aux;
            //Remove all the relationships by executionId
            ExecutionTestService.DeleteData(executionId, r => {

            }, err => {

            });
            $scope.testEvidenceToSaveArray.forEach(function (tc) {
                if (tc.Evidence == "TC") {
                    aux = {
                        Execution_Group_Id: executionId,
                        Tc_Id: tc.Element_id
                    };

                }
                if (tc.Evidence == "TP") {
                    aux = {
                        Execution_Group_Id: executionId,
                        Tp_Id: tc.Element_id
                    };
                }
                if (tc.Evidence == "TS") {
                    aux = {
                        Execution_Group_Id: executionId,
                        Ts_Id: tc.Element_id
                    };
                }
                //if ($scope.IsAutomated == 1) {
                //    aux = {
                //        Execution_Group_Id: executionId,
                //        Ta_Id: tc.Id
                //    };
                //}
                UpdatedRelations.push(aux);
            });
            ExecutionTestService.Save(UpdatedRelations, executionId, r => {
            }, err => {
            });
        }

        function AddExecutionRelations(id) {
            var executionGroupsList = [];
            var ExecutionTest = "";
            $scope.testEvidenceToSaveArray.forEach(function (test) {
                if (test.Evidence == "TC") {
                    ExecutionTest = {
                        Execution_Group_Id: id,
                        Tc_Id: test.Element_id
                    };
                }
                if (test.Evidence == "TP") {
                    ExecutionTest = {
                        Execution_Group_Id: id,
                        Tp_Id: test.Element_id
                    };
                }
                //if ($scope.IsAutomated == 1) {
                //    ExecutionTest = {
                //        Execution_Group_Id: id,
                //        Ta_Id: test.Id
                //    };
                //}
                if (test.Evidence == "TS") {
                    ExecutionTest = {
                        Execution_Group_Id: id,
                        Ts_Id: test.Element_id
                    };
                }
                executionGroupsList.push(ExecutionTest);
            });

            ExecutionTestService.Save(executionGroupsList, id, function (success) {

            }, function (err) {
            });
        }

        $scope.selectedCount = function () {
            var arrFiltered = $scope.testEvidenceArray.filter(obj => obj.IsSelected == true);
            return arrFiltered.length;
        }

        $scope.uniqueCount = function () {
            var uniq = {};
            var arrFiltered = $scope.testEvidenceArray.filter(obj => !uniq[obj.Identifier_number] && (uniq[obj.Identifier_number] = true) && obj.IsSelected == true);
            return arrFiltered.length;
        }

        $scope.RemoveFilter = function () {
            $scope.filtered = false;
            $scope.requirementsArray = $scope.requirementsArrayStorage;
        }

        $scope.AddReq = function (filter) {
            $scope.FilterReq = filter;
            if (filter == "All") {
                $scope.ListOfReqSprint = [];
            } else {
                var IsAlreadyApply = false;
                $scope.ListOfReqSprint.forEach(s => {
                    if (s == filter) {
                        IsAlreadyApply = true;
                    }
                });
                if (!IsAlreadyApply) {
                    $scope.ListOfReqSprint.push(filter);
                }

            }
        }

        $scope.removeReq = function (filter) {
            $scope.ListOfReqSprint = $scope.ListOfReqSprint.filter(item => item != filter);
            if ($scope.ListOfReqSprint.length == 0) {
                $scope.FilterReq = "All";
            }
        }

        $scope.selectFiltered = function () {
            $scope.testEvidenceArray = [];
            var executeFilter1 = true;
            var executeFilter2 = true;
            if ($scope.FilterReq == "All" || $scope.ListOfReqSprint.length == 0) {
                $scope.requirementsArray = $scope.requirementsArrayStorage;
                executeFilter1 = false;
            }
            if ($scope.optionSelected == "All") {
                $scope.testEvidenceArray = $scope.testEvidenceArrayStorage;
                executeFilter2 = false;
            }
            if (executeFilter1) {
                $scope.requirementsArray = [];
                $scope.ListOfReqSprint.forEach(filter => {
                    $scope.requirementsArrayAux = $scope.requirementsArrayStorage.filter(req => req.Release == filter);

                    $scope.requirementsArray = $scope.requirementsArray.concat($scope.requirementsArrayAux);
                });
            }
            if (executeFilter2) {
                $scope.testEvidenceArray = [];
                var EvidenceValuesFiltered = $scope.testEvidenceArrayStorage.filter(obj => obj.Type == $scope.optionSelected);

                $(".collapse").collapse('hide');

                $scope.testEvidenceArray = EvidenceValuesFiltered;
                HasEvidence();
                $scope.preloadList($scope.reqSelected);
            }
        }

        $scope.selectAll = function () {
            if ($scope.filtered) {
                $scope.optionSelected = "All";
                $scope.selectFiltered();
            }
            $scope.filter = "";
            $scope.testEvidenceArray.forEach(function (evidence) {
                if (evidence.Added == undefined || evidence.Added == false) {
                    evidence.IsSelected = true;
                    evidence.Style = 'evidenceSelected';
                }
            });
            $scope.testEvidenceArrayFilter.forEach(function (evidence) {
                if (evidence.Added == undefined || evidence.Added == false) {

                    evidence.IsSelected = true;
                    evidence.Style = 'evidenceSelected';
                }
            });
            $scope.requirementsArray.forEach(function (req) {
                if (req.HasEvidence) {
                    req.status = true;
                    req.style = 'selected';
                }
            });
        };

        $scope.unselectAll = function () {
            $scope.testEvidenceArray.forEach(function (evidence) {
                evidence.Added = false;
                evidence.IsSelected = false;
                evidence.Style = 'evidence';
            });

            $scope.testEvidenceArrayFilter.forEach(function (evidence) {
                evidence.Added = false;
                evidence.IsSelected = false;
                evidence.Style = 'evidence';
            });


            $scope.requirementsArray.forEach(function (req) {
                reqIsSelectedByReq(req);
            });

            $scope.testEvidenceToSaveArray = [];

            GetNumberOfEvidence();

        };

        $scope.unselectPreload = function () {
            $scope.testEvidenceArray.forEach(function (evidence) {
                if (evidence.IsSelected && (evidence.Added != undefined || !evidence.Added)) {
                    evidence.Added = false;
                    evidence.IsSelected = false;
                    evidence.Style = 'evidence';
                }
            });

            $scope.testEvidenceArrayFilter.forEach(function (evidence) {
                if (evidence.IsSelected && (evidence.Added != undefined || !evidence.Added)) {
                    evidence.Added = false;
                    evidence.IsSelected = false;
                    evidence.Style = 'evidence';
                }
            });


            $scope.requirementsArray.forEach(function (req) {
                reqIsSelectedByReq(req);
            });
        }

        $scope.testEvidenceArrayFilter = [];

        $scope.preloadList = function (req) {
            $scope.reqSelected = req;
            $(".collapse").collapse('hide');
            //if ($scope.IsAutomated == 0) {
            $scope.testEvidenceArrayFilter = $scope.testEvidenceArray.filter(obj => obj.ReqId == req.Id);
            //} else {
            //    $scope.testEvidenceArrayFilter = $scope.testEvidenceArray.filter(obj => obj.ScriptsGroup_Id == req.Id);
            //}
        }

        $scope.testEvidenceToSaveArray = [];

        $scope.LoadTestCases = function () {
            var hasData = false;
            $scope.testEvidenceArray.forEach(function (te) {
                if (te.IsSelected && (te.Added == undefined || te.Added == false)) {
                    te.Added = true;
                    te.IsSelected = false;
                    te.Style = 'evidenceAdded';

                    $scope.testEvidenceToSaveArray.push(te);
                    hasData = true;
                }
            });

            var uniq = {};

            var arrFiltered = $scope.testEvidenceToSaveArray.filter(obj => !uniq[obj.Identifier_number] && (uniq[obj.Identifier_number] = true));
            $scope.testEvidenceToSaveArray = arrFiltered;




            if (hasData) {
                ngToast.success({
                    content: '<p>The test evidence was added successfully</p>'
                });
            }
            GetNumberOfEvidence();
        }

        function LoadEvidence() {
            var reqs = [];
            $scope.requirementsArray.forEach(function (element) {
                reqs.push(element.Id);
            });
            if ($scope.IsAutomated == 0) {
                RequirementService.GetEvidenceFromReq(reqs, function (data) {
                    LoadTestEvidence(data);
                    LoadExecutionInformation();
                });
            } else {
                RequirementService.GetAutomatedEvidenceFromReq(reqs, function (data) {
                    LoadTestEvidence(data);
                    LoadExecutionInformation();
                });
            }

        }

        $scope.testEvidenceArray = [];

        function LoadTestEvidence(data) {
            var tmp = data
            for (var i = 0; i < tmp.length; i++) {
                tmp[i].Number = tmp[i].Identifier_number;
                tmp[i].Style = 'evidence';
                $scope.testEvidenceArray.push(tmp[i]);

            }
            $scope.testEvidenceArrayStorage = $scope.testEvidenceArray;
            HasEvidence();
            if ($state.current.data.action == 'Add') {
                $("#loadingModalGroupData").modal("hide");
            }
        }

        $scope.SelectEvidence = function (ev) {
            //Get Coincidences
            //if ($scope.IsAutomated == 0) {

            var values = $scope.testEvidenceArray.filter(obj => obj.Identifier_number == ev.Identifier_number);
            values.forEach(function (evidence) {
                if (evidence.Added != undefined && evidence.Added) {
                    return;
                }
                if (!evidence.IsSelected) {
                    if (evidence.Added != undefined || evidence.Added != true) {
                        evidence.IsSelected = true;
                        evidence.Style = 'evidenceSelected';
                    }
                } else {
                    if (evidence.Added == undefined || evidence.Added == false) {
                        evidence.IsSelected = false;
                        evidence.Style = 'evidence';
                    }
                }
                reqIsSelected(evidence);
            });
            //} else {
            //    if (ev.Added != undefined && ev.Added) {
            //        return;
            //    }
            //    if (!ev.IsSelected) {
            //        if (ev.Added != undefined || ev.Added != true) {
            //            ev.IsSelected = true;
            //            ev.Style = 'evidenceSelected';
            //        }
            //    } else {
            //        if (ev.Added == undefined || ev.Added == false) {
            //            ev.IsSelected = false;
            //            ev.Style = 'evidence';
            //        }
            //    }
            //    reqIsSelected(ev);
            //}
        }

        function reqIsSelected(evidence) {
            var values = "";
            //if ($scope.IsAutomated == 0) {

            values = $scope.testEvidenceArray.filter(obj => obj.ReqId == evidence.ReqId);
            //} else {
            //    values = $scope.testEvidenceArray.filter(obj => obj.ScriptsGroup_Id == evidence.ScriptsGroup_Id);

            //}
            var length = values.length;
            var count = 0;
            values.forEach(function (e) {
                if (e.IsSelected || (e.Added != undefined && e.Added)) {
                    count++;
                }
            });

            //is selected
            var style = ''
            var status = false

            if (count == length) {
                style = 'selected';
                status = true;
            }
            if (count > 0 && count < length) {
                style = 'partial';
                status = true;
            }
            if (length == 0) {
                style = 'nonSelected';
            }
            var index = "";

            //if ($scope.IsAutomated == 0) {

            index = $scope.requirementsArray.findIndex(obj => obj.Id == evidence.ReqId);
            //} else {
            //    index = $scope.requirementsArray.findIndex(obj => obj.Id == evidence.ScriptsGroup_Id);
            //}

            $scope.requirementsArray[index].style = style;
            $scope.requirementsArray[index].status = status;
        }

        function reqIsSelectedByReq(evidence) {
            //  var values = "";
            //if ($scope.IsAutomated == 0) {
            var values = $scope.testEvidenceArray.filter(obj => obj.ReqId == evidence.ReqId);
            //} else {
            //    values = $scope.testEvidenceArray.filter(obj => obj.ScriptsGroup_Id == evidence.Id);
            //}
            var length = values.length;
            var count = 0;
            values.forEach(function (e) {

                if (e.IsSelected || (e.Added != undefined && e.Added)) {
                    count++;
                }
            });

            //is selected
            var style = ''
            var status = false

            if (count == length) {
                style = 'selected';
                status = true;
            }
            if (count > 0 && count < length) {
                style = 'partial';
                status = true;
            }
            if (length == 0) {
                style = 'nonSelected'
            }
            //Get Req


            var index = $scope.requirementsArray.findIndex(obj => obj.Id == evidence.Id);
            $scope.requirementsArray[index].style = style;
            $scope.requirementsArray[index].status = status;
        }

        $scope.removeEvidenceFromSaveList = function (element) {
            //if ($scope.IsAutomated == 0) {

            var objIndex = $scope.testEvidenceToSaveArray.findIndex(obj => obj.Element_id == element.Element_id && obj.Identifier_number == element.Identifier_number);
            $scope.testEvidenceToSaveArray.splice(objIndex, 1);

            //get elements
            var elements = $scope.testEvidenceArray.filter(obj => obj.Identifier_number == element.Identifier_number);

            elements.forEach(function (element) {
                var objIndex = $scope.testEvidenceArray.findIndex(obj => obj.Element_id == element.Element_id && obj.Identifier_number == element.Identifier_number && obj.Added == true);
                $scope.testEvidenceArray[objIndex].Added = false;
                $scope.testEvidenceArray[objIndex].IsSelected = false;
                $scope.testEvidenceArray[objIndex].Style = "evidence"
            });

            checkProjectsStatus();
            GetNumberOfEvidence();

            //}
        }

        function checkProjectsStatus() {
            $scope.requirementsArray.forEach(function (req) {
                var values = "";
                // if ($scope.IsAutomated == 0) {
                var values = $scope.testEvidenceArray.filter(obj => obj.ReqId == req.Id);
                //} else {
                //    values = $scope.testEvidenceArray.filter(obj => obj.ScriptsGroup_Id == req.Id);
                //}

                var length = values.length;

                var count = 0;
                values.forEach(function (e) {

                    if (e.IsSelected || (e.Added != undefined && e.Added)) {
                        count++;
                    }
                });

                //is selected
                var style = ''
                var status = false

                if (count == length) {
                    style = 'selected';
                    status = true;
                }
                if (count > 0 && count < length) {
                    style = 'partial';
                    status = true;
                }
                if (length == 0) {
                    style = 'nonSelected'
                }
                //Get Req



                req.style = style;
                req.status = status;
            });
        }

        $scope.reqFocus = {};
        $scope.selectReqFocus = function (req) {
            $scope.reqFocus = req;
        }

        function setStatusToTestEvidence(arrFiltered) {
            //if ($scope.IsAutomated == 0) {
            arrFiltered.forEach(function (element) {
                var objects = $scope.testEvidenceArray.filter(obj => obj.Element_id == element.Element_id && obj.Identifier_number == element.Identifier_number);
                objects.forEach(function (element2) {
                    var objIndex = $scope.testEvidenceArray.findIndex(obj => obj.Element_id == element2.Element_id && obj.Identifier_number == element2.Identifier_number && obj.Added == false);
                    $scope.testEvidenceArray[objIndex].IsSelected = false;
                    $scope.testEvidenceArray[objIndex].Added = true;
                    $scope.testEvidenceArray[objIndex].Style = 'evidenceAdded';
                });
            });
            //} else {
            //    arrFiltered.forEach(function (element) {
            //        $scope.testEvidenceArray.forEach(function (element2) {
            //            if (element.Name == element2.Name) {
            //                element2.IsSelected = false;
            //                element2.Added = true;
            //                element2.Style = 'evidenceAdded';
            //            }
            //        });
            //    });
            //}

        }

        function LoadTestCasesOnEdit() {
            if ($scope.IsAutomated == 0) {

                ExecutionTestService.GetCombinedEvidence(executionId, function (data) {
                    var uniq = {}
                    var arrFiltered = data.filter(obj => !uniq[obj.Identifier_number] && (uniq[obj.Identifier_number] = true));
                    $scope.testEvidenceToSaveArray = arrFiltered;
                    $scope.testEvidenceArray.forEach(function (ev) {
                        ev.IsSelected = false;
                        ev.Added = false;
                    });
                    arrFiltered.forEach(function (ev) {
                        ev.IsSelected = false;
                        ev.Added = false;
                    });

                    setStatusToTestEvidence(arrFiltered);

                    $scope.requirementsArray.forEach(function (req) {
                        reqIsSelectedByReq(req);
                    });
                    GetNumberOfEvidence();
                    checkProjectsStatus();

                    $("#loadingModalGroupData").modal("hide");


                }, function (err) {
                });
            } else {
                ExecutionTestService.GetAutomatedTestForExecutionGroup(executionId, function (data) {
                    var uniq = {}
                    var arrFiltered = data.filter(obj => !uniq[obj.Identifier_number] && (uniq[obj.Identifier_number] = true));
                    $scope.testEvidenceToSaveArray = arrFiltered;

                    $scope.testEvidenceArray.forEach(function (ev) {
                        ev.IsSelected = false;
                        ev.Added = false;
                    });
                    arrFiltered.forEach(function (ev) {
                        ev.IsSelected = false;
                        ev.Added = false;
                    });

                    setStatusToTestEvidence(arrFiltered);

                    $scope.requirementsArray.forEach(function (req) {
                        reqIsSelectedByReq(req);
                    });
                    GetNumberOfEvidence();
                    checkProjectsStatus();

                    $("#loadingModalGroupData").modal("hide");

                }, function (err) {
                });
            }

            $scope.cancel = function () {
                $state.go('executionGroups', { projectId: projectId });
            }
        }

    }])
})();