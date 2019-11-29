(function () {
    //'use strict';

    var app = angular.module(appName);

    app.config(["$stateProvider", "$urlRouterProvider", "$locationProvider", function ($stateProvider, $urlRouterProvider, $locationProvider) {

        var base = virtualPath + 'Home/';

        // For any unmatched url, send to default page
        $urlRouterProvider.otherwise("/");

        //Projects
        $stateProvider
            .state('Index', {
                url: '/',
                controller: "ProjectController",
                templateUrl: base + "project",
                data: {
                    action: 'List'
                }
            });
        $stateProvider
            .state('Users', {
                url: '/users',
                controller: "UserController",
                templateUrl: base + "users",
                data: {
                    action: 'List'
                }
            });

        $stateProvider
            .state('Runners', {
                url: '/Runners',
                controller: "RunnerController",
                templateUrl: base + "runners",
            });

        $stateProvider
            .state('Profile', {
                url: '/Profile',
                controller: "ProfileController",
                templateUrl: base + "profile",
                data: {
                    action: 'List'
                }
            });

        $stateProvider
            .state('Backups', {
                url: '/backups',
                controller: "BackupController",
                templateUrl: base + "backup",
                data: {
                    action: 'List'
                }
            });

        $stateProvider
            .state('Projects', {
                url: '/project',
                controller: "ProjectController",
                templateUrl: base + "project",
                data: {
                    action: 'List'
                }
            })
            .state('projectAdd', {
                url: '/project/add',
                templateUrl: base + "projectAddEdit",
                controller: "ProjectAddEditController",
                data: {
                    action: 'Add'
                }
            })
            .state('projectEdit', {
                url: '/project/edit/:id',
                templateUrl: base + "projectAddEdit",
                controller: "ProjectAddEditController",
                data: {
                    action: 'Edit'
                }
            })
            .state('projectDetails', {
                url: '/project/details/:id',
                templateUrl: base + "projectDetails",
                controller: "ProjectDetailsController",
                data: {
                    action: 'Details'
                }

            })
            .state('projectActivate', {
                url: '/project/activate',
                templateUrl: base + "projectActivate",
                controller: "ProjectActivateController",
                data: {
                    action: 'Activate'
                }
            })
            .state('projectChangeLog', {
                url: '/project/changelog/:id',
                templateUrl: base + "projectChangeLog",
                controller: "ProjectChangeLogController",
                data: {
                    action: 'ChangeLog'
                }
            }).state('projectExecutions', {
                url: '/project/executions/:projectId/:executionId',
                templateUrl: base + "projectExecution",
                controller: "ProjectExecutionController",
                data: {
                    action: 'ExecutionDetail'
                }
            });



        $stateProvider
            .state('ScriptGroupAdd', {
                url: '/project/:projectId/ScriptGroup/add/',
                templateUrl: base + "ScriptGroupAddEdit",
                controller: "ScriptGroupAddEditController",
                data: {
                    action: 'Add'
                }
            }).state('ScriptGroupEdit', {
                url: '/project/:projectId/ScriptGroup/Edit/:id',
                templateUrl: base + "ScriptGroupAddEdit",
                controller: "ScriptGroupAddEditController",
                data: {
                    action: 'Edit'
                }
            })


        $stateProvider
            .state('requirementAdd', {
                url: '/project/:projectId/requirement/add/',
                templateUrl: base + "requirementAddEdit",
                controller: "RequirementAddEditController",
                data: {
                    action: 'Add'
                }
            })

            .state('requirementEdit', {
                url: '/project/:projectId/requirement/edit/:id',
                templateUrl: base + "requirementAddEdit",
                controller: "RequirementAddEditController",
                data: {
                    action: 'Edit'
                }
            })

            .state('requirementDetails', {
                url: '/project/:projectId/requirement/:id',
                templateUrl: base + "requirementDetails",
                controller: "RequirementDetailsController",
                data: {
                    action: 'Details'
                }
            })
            .state('requirementActivate', {
                url: '/project/:projectId/requirements/activate',
                templateUrl: base + "requirementActivate",
                controller: "RequirementActivateController",
                data: {
                    action: 'Activate'
                }
            })
            .state('requirementChangeLog', {
                url: '/project/:projectId/requirement/changelog/:id',
                templateUrl: base + "requirementChangeLog",
                controller: "RequirementChangeLogController",
                data: {
                    action: 'ChangeLog'
                }
            });

        $stateProvider
            .state('executionGroups', {
                url: '/project/:projectId/executionGroups',
                templateUrl: base + 'executionGroup',
                controller: "ExecutionGroupController",
                data: {
                    action: 'List'
                }
            }).state('executionGroupDetailsManual', {
                url: '/project/:projectId/executionGroups/details/:executionId',
                templateUrl: base + 'executionGroupsDetails',
                controller: "ExecutionGroupDetailsController",
                data: {
                    action: 'Details',
                    type: 'Manual'
                }
            }).state('executionGroupDetailsAutomated', {
                url: '/project/:projectId/executionGroups/details/:executionId',
                templateUrl: base + 'executionGroupsDetails',
                controller: "ExecutionGroupDetailsController",
                data: {
                    action: 'Details',
                    type: 'Automated'
                }
            }).state('executionGroupsAddManual', {
                url: '/project/:projectId/executionGroups/add/manual',
                templateUrl: base + 'executionGroupAddEdit',
                controller: "ExecutionGroupAddEditController",
                data: {
                    action: 'Add',
                    IsAutomated: 0
                }
            }).state('executionGroupsEditManual', {
                url: '/project/:projectId/executionGroups/edit/manual/:executionId',
                templateUrl: base + 'executionGroupAddEdit',
                controller: "ExecutionGroupAddEditController",
                data: {
                    action: 'Edit',
                    IsAutomated: 0
                }
            }).state('executionGroupsAddAutomated', {
                url: '/project/:projectId/executionGroups/add/automated',
                templateUrl: base + 'executionGroupAddEdit',
                controller: "ExecutionGroupAddEditController",
                data: {
                    action: 'Add',
                    IsAutomated: 1
                }
            }).state('executionGroupsEditAutomated', {
                url: '/project/:projectId/executionGroups/edit/automated/:executionId',
                templateUrl: base + 'executionGroupAddEdit',
                controller: "ExecutionGroupAddEditController",
                data: {
                    action: 'Edit',
                    IsAutomated: 1
                }


            }).state('executionGroupInactives', {
                url: '/project/:projectId/executionGroups/inactives',
                templateUrl: base + 'executionGroupInactives',
                controller: 'ExecutionGroupsInactivesController',
                data: {
                    action: 'Activate'
                }
            }).state('executionGroupPlay', {
                url: '/project/:projectId/executionGroups/play/:executionId/:testExecutionId',
                templateUrl: base + 'executionGroupPlay',
                controller: 'ExecutionGroupsPlayController',
                data: {
                    action: 'Play'
                }
            }).state('executionGroupView', {
                url: '/project/:projectId/executionGroups/View/:executionId/:testExecutionId',
                templateUrl: base + 'ExecutionView',
                controller: 'ExecutionViewController',
                data: {
                    action: 'View'
                }
            });


        $stateProvider
            .state('testEnvironment', {
                url: '/project/:projectId/testenvironment/',
                templateUrl: base + 'testEnvironment',
                controller: "TestEnvironmentController",
                data: {
                    action: 'List'
                }
            }).state('testEnvironmentDetails', {
                url: '/project/:projectId/testenvironment/details/:id',
                templateUrl: base + 'testEnvironmentDetails',
                controller: "TestEnvironmentDetailsController",
                data: {
                    action: 'Details'
                }
            }).state('testEnvironmentAdd', {
                url: '/project/:projectId/testenvironment/add',
                templateUrl: base + 'testEnvironmentAddEdit',
                controller: "TestEnvironmentAddEditController",
                data: {
                    action: 'Add'
                }
            }).state('testEnvironmentEdit', {
                url: '/project/:projectId/testenvironment/edit/:id',
                templateUrl: base + 'testEnvironmentAddEdit',
                controller: "TestEnvironmentAddEditController",
                data: {
                    action: 'Edit'
                }
            }).state('testEnvironmentInactive', {
                url: '/project/:projectId/testenvironment/inactive/',
                templateUrl: base + "testEnvironmentActivate",
                controller: "TestEnvironmentsActivateController",
                data: {
                    action: 'Activate'
                }
            });



        $stateProvider
            .state('testcaseAdd', {
                url: '/project/:projectId/requirement/:reqId/testcase/add/',
                templateUrl: base + "testCaseAddEdit",
                controller: "TestCaseAddEditController",
                data: {
                    action: 'Add'
                }
            })

            .state('testcaseAddCopy', {
                url: '/project/:projectId/requirement/:reqId/testcase/copy/:id',
                templateUrl: base + "testCaseAddEdit",
                controller: "TestCaseAddEditController",
                data: {
                    action: 'Add',
                    isCopy: true
                }
            })

            .state('testcaseDetails', {
                url: '/project/:projectId/requirement/:reqId/testcase/details/:id',
                templateUrl: base + "testCaseDetails",
                controller: "TestCaseDetailsController",
                data: {
                    action: 'Details'
                }
            })
            .state('testcaseEdit', {
                url: '/project/:projectId/requirement/:reqId/testcase/edit/:id',
                templateUrl: base + "testCaseAddEdit",
                controller: "TestCaseAddEditController",
                data: {
                    action: 'Edit'
                }
            })
            .state('testCaseActivate', {
                url: '/project/:projectId/requirement/:reqId/testcase/activate/',
                templateUrl: base + "testCaseActivate",
                controller: "TestCaseActivateController",
                data: {
                    action: 'Activate'
                }
            })
            .state('testCaseChangeLog', {
                url: '/project/:projectId/requirement/:reqId/testcase/changelog/:id',
                templateUrl: base + "testcaseChangeLog",
                controller: "TestCaseChangeLogController",
                data: {
                    action: 'ChangeLog'
                }
            });

        $stateProvider
            .state('testscenarioAdd', {
                url: '/project/:projectId/requirement/:reqId/testscenario/add/',
                templateUrl: base + "testScenarioAddEdit",
                controller: "TestScenarioAddEditController",
                data: {
                    action: 'Add'
                }
            })

            .state('testscenarioAddCopy', {
                url: '/project/:projectId/requirement/:reqId/testscenario/copy/:id',
                templateUrl: base + "testScenarioAddEdit",
                controller: "TestScenarioAddEditController",
                data: {
                    action: 'Add',
                    isCopy: true
                }
            })


            .state('testscenarioDetails', {
                url: '/project/:projectId/requirement/:reqId/testscenario/details/:id',
                templateUrl: base + "testScenarioDetails",
                controller: "TestScenarioDetailsController",
                data: {
                    action: 'Details'
                }
            })
            .state('testscenarioEdit', {
                url: '/project/:projectId/requirement/:reqId/testscenario/edit/:id',
                templateUrl: base + "testScenarioAddEdit",
                controller: "TestScenarioAddEditController",
                data: {
                    action: 'Edit'
                }
            })
            .state('testScenarioActivate', {
                url: '/project/:projectId/requirement/:reqId/testscenario/activate/',
                templateUrl: base + "testScenarioActivate",
                controller: "TestScenarioActivateController",
                data: {
                    action: 'Activate'
                }
            })
            .state('testScenarioChangeLog', {
                url: '/project/:projectId/requirement/:reqId/testscenario/changelog/:id',
                templateUrl: base + "testscenarioChangeLog",
                controller: "TestScenarioChangeLogController",
                data: {
                    action: 'ChangeLog'
                }
            });

        $stateProvider
            .state('testprocedureAdd', {
                url: '/project/:projectId/requirement/:reqId/testprocedure/add',
                templateUrl: base + "testProcedureAddEdit",
                controller: "TestProcedureAddEditController",
                data: {
                    action: 'Add'
                }
            })
            .state('testprocedureAddCopy', {
                url: '/project/:projectId/requirement/:reqId/testprocedure/copy/:id',
                templateUrl: base + "testProcedureAddEdit",
                controller: "TestProcedureAddEditController",
                data: {
                    action: 'Add',
                    isCopy: true
                }
            })
            .state('testprocedureDetails', {
                url: '/project/:projectId/requirement/:reqId/testprocedure/details/:id',
                templateUrl: base + "testProcedureDetails",
                controller: "TestProcedureDetailsController",
                data: {
                    action: 'Details'
                }
            })
            .state('testprocedureEdit', {
                url: '/project/:projectId/requirement/:reqId/testprocedure/edit/:id',
                templateUrl: base + "testProcedureAddEdit",
                controller: "TestProcedureAddEditController",
                data: {
                    action: 'Edit'
                }
            })
            .state('testProcedureActivate', {
                url: '/project/:projectId/requirement/:reqId/testprocedure/activate',
                templateUrl: base + "testProcedureActivate",
                controller: "TestProcedureActivateController",
                data: {
                    action: 'Activate'
                }
            })
            .state('testProcedureChangeLog', {
                url: '/project/:projectId/requirement/:reqId/testprocedure/changelog/:id',
                templateUrl: base + "testprocedureChangeLog",
                controller: "TestProcedureChangeLogController",
                data: {
                    action: 'ChangeLog'
                }
            });




        $stateProvider
            .state('testAutomatedAdd', {
                url: '/project/:projectId/requirement/:reqId/testAutomated/add',
                templateUrl: base + "testAutomatedAddEdit",
                controller: "TestAutomatedAddEditController",
                data: {
                    action: 'Add'
                }
            })
            .state('testAutomatedAddCopy', {
                url: '/project/:projectId/requirement/:reqId/testAutomated/copy/:id',
                templateUrl: base + "testAutomatedAddEdit",
                controller: "TestAutomatedAddEditController",
                data: {
                    action: 'Add',
                    isCopy: true
                }
            })
            .state('testAutomatedEdit', {
                url: '/project/:projectId/requirement/:reqId/testAutomated/edit/:id',
                templateUrl: base + "testAutomatedAddEdit",
                controller: "TestAutomatedAddEditController",
                data: {
                    action: 'Edit'
                }
            });


        $stateProvider
            .state('traceabilityFinding', {
                url: '/project/:projectId/traceability/finding/',
                templateUrl: base + "traceabilityFinding",
                controller: "TraceabilityFindingController",
                data: {
                    action: 'Information'
                }
            });

        $stateProvider
            .state('suplementalDetails', {
                url: '/project/:projectId/suplemental/details/:id',
                templateUrl: base + "testsuplementalDetails",
                controller: "TestSuplementalDetailsController",
                data: {
                    action: 'Details'
                }
            })

            .state('suplementalAdd', {
                url: '/project/:projectId/suplemental/add',
                templateUrl: base + "testsuplementalAddEdit",
                controller: "TestSuplementalAddEditController",
                data: {
                    action: 'Add'
                }
            })

            .state('suplementalAddCopy', {
                url: '/project/:projectId/suplemental/copy/:id',
                templateUrl: base + "testsuplementalAddEdit",
                controller: "TestSuplementalAddEditController",
                data: {
                    action: 'Add',
                    isCopy: true
                }
            })


            .state('suplementalEdit', {
                url: '/project/:projectId/suplemental/edit/:id',
                templateUrl: base + "testsuplementalAddEdit",
                controller: "TestSuplementalAddEditController",
                data: {
                    action: 'Edit'
                }
            })
            .state('testSuplementalActivate', {
                url: '/project/:projectId/suplemental/activate/',
                templateUrl: base + "testSuplementalActivate",
                controller: "TestSuplementalActivateController",
                data: {
                    action: 'Activate'
                }
            })
            .state('testSuplementalChangeLog', {
                url: '/project/:projectId/testsuplemental/changelog/:id',
                templateUrl: base + "testsuplementalChangeLog",
                controller: "TestSuplementalChangeLogController",
                data: {
                    action: 'ChangeLog'
                }
            });
        // Specify HTML5 mode (using the History APIs) or HashBang syntax.
        $locationProvider.html5Mode(false).hashPrefix('!');
    }]);
})();