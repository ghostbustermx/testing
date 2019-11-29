(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('StepService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Step/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                SaveArray: {
                    method: 'POST',
                    isArray: true
                },
                DeleteArray: {
                    method: 'POST',
                    isArray: true
                },
                UpdateArray: {
                    method: 'POST',
                    isArray: true
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
                DeleteForTC: {
                    method: 'DELETE'
                },
                GetForTestCase: {
                    method: 'GET',
                    isArray: true
                },
                GetForTestScenario: {
                    method: 'GET',
                    isArray: true
                },
                GetForTestProcedure: {
                    method: 'GET',
                    isArray: true
                },
                GetForTestSuplemental: {
                    method: 'GET',
                    isArray: true
                },
                GetForTestProcedureSTP: {
                    method: 'GET',
                    isArray: true
                },
                GetForTestScenarioSTP: {
                    method: 'GET',
                    isArray: true
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

        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };

        self.SaveArray = function (entity, projectId, TypeOfSave, success, error) {
            return resource.SaveArray({ action: 'SaveArray', projectId: projectId, TypeOfSave: TypeOfSave }, entity, success || noop, error || noop);
        };


        self.DeleteArray = function (entity, success, error) {
            return resource.DeleteArray({ action: 'DeleteArray' }, entity, success || noop, error || noop);
        };

        self.UpdateArray = function (entity, projectId, TypeOfSave, TestEvidenceId, Evidence, success, error) {
            return resource.UpdateArray({ action: 'UpdateArray', projectId: projectId, TypeOfSave: TypeOfSave, TestEvidenceId: TestEvidenceId, Evidence: Evidence }, entity, success || noop, error || noop);
        };

        self.DeleteForTC = function (tcId, success, error) {
            return resource.DeleteForTC({ action: 'DeleteForTC', tcId: tcId }, success || noop, error || noop);
        };

        self.GetForTestCase = function (tcId, success, error) {
            return resource.GetForTestCase({ action: 'GetForTestCase', tcId: tcId }, success || noop, error || noop);
        };

        self.GetForTestScenario = function (tsId, success, error) {
            return resource.GetForTestCase({ action: 'GetForTestScenario', tsId: tsId }, success || noop, error || noop);
        };

        self.GetForTestProcedure = function (tpId, success, error) {
            return resource.GetForTestProcedure({ action: 'GetForTestProcedure', tpId: tpId }, success || noop, error || noop);
        };

        self.GetForTestProcedureSTP = function (projectId, tpId, success, error) {
            return resource.GetForTestProcedureSTP({ action: 'GetForTestProcedureSTP', projectId: projectId, tpId: tpId }, success || noop, error || noop);
        };

        self.GetForTestScenarioSTP = function (projectId, tpId, success, error) {
            return resource.GetForTestScenarioSTP({ action: 'GetForTestScenarioSTP', projectId: projectId, tpId: tpId }, success || noop, error || noop);
        };

        self.GetForTestSuplemental = function (stpId, success, error) {
            return resource.GetForTestCase({ action: 'GetForTestSuplemental', stpId: stpId }, success || noop, error || noop);
        };

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };

    }]);
})(angular);