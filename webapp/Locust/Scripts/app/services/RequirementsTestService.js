(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('RequirementsTestService', ['$resource', function ($resource) {
        
        var noop = angular.noop;
        var resource = $resource('/api/RequirementsTest/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                Update: {
                    method: 'PUT'
                },
                Delete: {
                    method: 'DELETE',
                    params: {
                        id: "@id"
                    }
                },
                Get: {
                    method: 'GET',
                    isArray: false
                },
                GetAll: {
                    method: 'GET',
                    isArray: true
                },
                GetTestCaseRelations: {
                    method: 'GET',
                    isArray: true
                },
                GetTestScenarioRelations: {
                    method: 'GET',
                    isArray: true
                },
                GetTestProcedureRelations: {
                    method: 'GET',
                    isArray: true
                },
                DeleteTestCase: {
                    method: 'GET',
                    isArray: false
                },
                DeleteTestProcedure: {
                    method: 'GET',
                    isArray: false
                },
                DeleteTestScenario: {
                    method: 'GET',
                    isArray: false
                }
            }),

            self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };

        self.Get = function (id, success, error) {
            return resource.Get({ action: 'Get', id: id }, success || noop, error || noop);
        };

        self.Delete = function (id, success, error) {
            return resource.Delete({ action: 'Delete', id: id }, success || noop, error || noop);
        };

        self.DeleteTestCase = function (reqId, tcId, success, error) {
            return resource.DeleteTestCase({ action: 'DeleteTestCase', reqId: reqId, tcId: tcId }, success || noop, error || noop);
        };

        self.DeleteTestProcedure = function (reqId, tpId, success, error) {
            return resource.DeleteTestProcedure({ action: 'DeleteTestProcedure', reqId: reqId, tpId: tpId }, success || noop, error || noop);
        };

        self.DeleteTestScenario = function (reqId, tsId, success, error) {
            return resource.DeleteTestScenario({ action: 'DeleteTestScenario', reqId: reqId, tsId: tsId }, success || noop, error || noop);
        };

        self.GetTestCaseRelations = function (id, success, error) {
            return resource.GetTestCaseRelations({ action: 'GetTestCaseRelations', id: id }, success || noop, error || noop)
        };
        self.GetTestProcedureRelations = function (id, success, error) {
            return resource.GetTestProcedureRelations({ action: 'GetTestProcedureRelations', id: id }, success || noop, error || noop)
        };
        self.GetTestScenarioRelations = function (id, success, error) {
            return resource.GetTestScenarioRelations({ action: 'GetTestScenarioRelations', id: id }, success || noop, error || noop)
        };
        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };
        
    }]);
})(angular);