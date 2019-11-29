
(function (angular, undefined) {
    'use strict';

    var app = angular.module(appName);

    app.service('ExecutionTestService', ['$resource', function ($resource) {
        var noop = angular.noop;

        var resource = $resource('/api/ExecutionTest/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                DeleteData: {
                    method: 'GET'
                },
                GetTestCases: {
                    method: 'GET',
                    isArray: true
                },
                GetTestProcedures: {
                    method: 'GET',
                    isArray: true
                },
                GetTestScenarios: {
                    method: 'GET',
                    isArray: true
                },
                GetCombinedEvidence: {
                    method: 'GET',
                    isArray: true
                },
                GetAutomatedTestForExecutionGroup: {
                    method: 'GET',
                    isArray: true
                },
                SaveTestProcedure: {
                    method: 'POST'
                }
            });


        self = this;


        self.Save = function (data, executionid , success, error) {
            return resource.Save({ action: 'Save', executionid: executionid }, data, success || noop, error || noop);
        };


        self.SaveTestProcedure = function (data, success, error) {
            return resource.Save({ action: 'SaveTestProcedure' }, data, success || noop, error || noop);
        };

        self.DeleteData = function (executionId, success, error) {
            return resource.DeleteData({ action: 'DeleteData', executionId: executionId }, success || noop, error || noop);
        };

        self.GetTestCases = function (id, success, error) {
            return resource.GetTestCases({ action: "GetTestCases", executionId: id }, success || noop, error || noop);
        }

        self.GetTestProcedures = function (id, success, error) {
            return resource.GetTestCases({ action: "GetTestProcedures", executionId: id }, success || noop, error || noop);
        }

        self.GetTestScenarios = function (id, success, error) {
            return resource.GetTestCases({ action: "GetTestScenarios", executionId: id }, success || noop, error || noop);
        }

        self.GetCombinedEvidence = function (id, success, error) {
            return resource.GetCombinedEvidence({ action: "GetCombinedEvidence", executionId: id }, success || noop, error || noop);
        }

        self.GetAutomatedTestForExecutionGroup = function (id, success, error) {
            return resource.GetAutomatedTestForExecutionGroup({ action: "GetAutomatedTestForExecutionGroup", executionId: id }, success || noop, error || noop);
        }
    }])

})(angular);