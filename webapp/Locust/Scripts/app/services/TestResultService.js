(function () {
    'use strict';

    var app = angular.module(appName);

    app.service('TestResultService', ['$resource', function ($resource) {

        var noop = angular.noop;

        var resource = $resource('/api/TestResult/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                GetForGroup: {
                    method: 'GET',
                    isArray: true,
                    timeout:10000
                },
                SetStatus: {
                    method: 'POST',
                },
                GetForTestCase: {
                    method: 'GET',
                    isArray: false
                },
                GetForTestProcedure: {
                    method: 'GET',
                    isArray: false
                },
                GetForTestScenario: {
                    method: 'GET',
                    isArray: false
                },
                GetCurrentHolder: {
                    method: 'GET',
                    isArray: false
                },
                CreateTestResults: {
                    method: 'GET',
                    isArray: true
                },
                Get: {
                    method: 'GET',
                    isArray: false
                },
                GetToExecute: {
                    method: 'GET',
                    isArray: false
                },
                ReassignTestResult: {
                    method: 'POST'
                },
                GetForUser: {
                    method: 'GET',
                    isArray: true

                },
                RemoveUser: {
                    method: 'GET',
                    isArray: false

                },
                UpdateTestResults: {
                    method: 'GET',
                    isArray: true
                },
                PassAll: {
                    method: 'GET',
                    isArray: false
                },
                FailAll: {
                    method: 'GET',
                    isArray: false
                },
                RemoveFromUsersExecution: {
                    method: 'POST',
                }
            });
            self = this;

        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        }

        self.GetForGroup = function (id, success, error) {
            return resource.GetForGroup({ action: 'GetForGroup', id: id }, success || noop, error || noop);
        }

        self.GetForUser = function (id,success, error) {
            return resource.GetForUser({ action: 'GetForUser', testExecutionId: id }, success || noop, error || noop);
        }

        self.SetStatus = function (entity, success, error) {
            return resource.SetStatus({ action: 'SetStatus'}, entity, success || noop, error || noop);
        }

        self.GetForTestCase = function (id, tcId, success, error) {
            return resource.GetForTestCase({ action: 'GetForTestCase', id: id, tcId: tcId }, success || noop, error || noop);
        }

        self.CreateTestResults = function (groupId, executionId, success, error) {
            return resource.CreateTestResults({ action: 'CreateTestResults', groupId: groupId, executionId: executionId }, success || noop, error || noop);
        }

        self.GetForTestProcedure = function (id, tpId, success, error) {
            return resource.GetForTestProcedure({ action: 'GetForTestProcedure', id: id, tpId: tpId }, success || noop, error || noop);
        }

        self.GetForTestScenario = function (id, tsId, success, error) {
            return resource.GetForTestScenario({ action: 'GetForTestScenario', id: id, tsId: tsId }, success || noop, error || noop);
        }

        self.GetCurrentHolder = function (id, testId, type, success, error) {
            return resource.GetCurrentHolder({ action: 'GetCurrentHolder', executionId: id, testId: testId, type:type}, success || noop, error || noop);
        }

        self.Get = function (id, success, error) {
            return resource.GetCurrentHolder({ action: 'Get', idTestResult: id}, success || noop, error || noop);
        }

        self.GetToExecute = function (id, success, error) {
            return resource.GetToExecute({ action: 'GetToExecute', testExecutionId: id }, success || noop, error || noop);
        }

        self.ReassignTestResult = function (entity, success, error) {
            return resource.ReassignTestResult({ action: 'ReassignTestResult' }, entity, success || noop, error || noop);
        }

        self.RemoveUser = function (id, success, error) {
            return resource.RemoveUser({ action: 'RemoveFromExecution', testExecutionId: id }, success || noop, error || noop);

        }

        self.RemoveFromUsersExecution = function (id, users, success, error) {
            return resource.RemoveFromUsersExecution({ action: 'RemoveFromUsersExecution', executionId: id}, users, success || noop, error || noop);

        }

        self.UpdateTestResults = function (groupId, executionId, success, error) {
            return resource.UpdateTestResults({ action: 'UpdateTestResults', groupId: groupId, executionId: executionId }, success || noop, error || noop);

        }
        self.PassAll = function (executionId, tester, evidence ,success, error) {
            return resource.PassAll({ action: 'PassAll', executionId: executionId, tester: tester, evidence: evidence }, success || noop, error || noop);

        }

        self.FailAll = function (executionId, tester, evidence, success, error) {
            return resource.FailAll({ action: 'FailAll', executionId: executionId, tester: tester, evidence: evidence }, success || noop, error || noop);

        }
    }]);

})(angular);