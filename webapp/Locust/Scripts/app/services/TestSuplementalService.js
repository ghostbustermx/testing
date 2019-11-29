(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('TestSuplementalService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/TestSuplemental/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                Update: {
                    method: 'PUT'
                },
                Restore: {
                    method: 'POST'
                },
                Delete: {
                    method: 'DELETE',
                    params: {
                        id: "@id"
                    }
                },
                Enable: {
                    method: 'GET',
                    isArray: false
                },
                GetLastTestSuplemental: {
                    method: 'GET',
                    isArray: false
                },
                Get: {
                    method: 'GET',
                    isArray: false
                },
                GetLastOne: {
                    method: 'GET',
                    isArray: false
                },

                GetAll: {
                    method: 'GET',
                    isArray: true
                },

                GetAllSTPByProject: {
                    method: 'GET',
                    isArray: true
                },
                GetForProject: {
                    method: 'GET',
                    isArray: true
                },
                GetForProjectInactives: {
                    method: 'GET',
                    isArray: true
                },
                UpdateNumber: {
                    method: 'PUT'
                },

                GetProject: {
                    method: 'GET',
                    isArray: false
                },
                GetRequirement: {
                    method: 'GET',
                    isArray: false
                },
                GetProjectRequirement: {
                    method: 'GET',
                    isArray: false
                },
                GetProcedures: {
                    method: 'GET',
                    isArray: true
                },
                GetScenarios: {
                    method: 'GET',
                    isArray: true
                },
                AddChangeLog: {
                    method: 'GET',
                    isArray: false
                },
                TestSuplementalChangeLogs: {
                    method: 'GET',
                    isArray: true
                },
                GetForTestProcedure: {
                    method: 'GET',
                    isArray: true
                },
                GetForTestScenario: {
                    method: 'GET',
                    isArray: true
                },
                GetByNumber: {
                    method: 'GET',
                    isArray: false
                }
            }),

            self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };
        self.GetAllSTPByProject = function (idProject, success, error) {
            return resource.GetAllSTPByProject({ action: 'GetAllSTPByProject', idProject: idProject }, success || noop, error || noop);
        };
        self.Get = function (idTestSuplemental, success, error) {
            return resource.Get({ action: 'Get', idTestSuplemental: idTestSuplemental }, success || noop, error || noop);
        };
        self.GetForProject = function (idProject, success, error) {
            return resource.GetForProject({ action: 'GetForProject', idProject: idProject }, success || noop, error || noop);
        };
        self.GetForProjectInactives = function (idProject, success, error) {
            return resource.GetForProjectInactives({ action: 'GetForProjectInactives', idProject: idProject }, success || noop, error || noop);
        };
        self.GetLastOne = function (idTestProcedure, idTestScenario, creator, date, success, error) {
            return resource.GetLastOne({ action: 'GetLastOne', idTestProcedure: idTestProcedure, idTestScenario: idTestScenario, creator: creator, date: date }, success || noop, error || noop);
        };
        self.TestSuplementalChangeLogs = function (id, success, error) {
            return resource.TestSuplementalChangeLogs({ action: 'TestSuplementalChangeLogs', id: id }, success || noop, error || noop);
        };
        self.AddChangeLog = function (id, success, error) {
            return resource.AddChangeLog({ action: 'AddChangeLog', id: id }, success || noop, error || noop);
        };
        self.Restore = function (entity, success, error) {
            return resource.Restore({ action: 'Restore' }, entity, success || noop, error || noop);
        };
        self.GetRequirement = function (idtp, idts, success, error) {
            return resource.GetRequirement({ action: 'GetRequirement', idtp: idtp, idts: idts }, success || noop, error || noop);
        };
        self.Delete = function (idTestSuplemental, success, error) {
            return resource.Delete({ action: 'Delete', idTestSuplemental: idTestSuplemental }, success || noop, error || noop);
        };
        self.Enable = function (id, success, error) {
            return resource.Enable({ action: 'Enable', id: id }, success || noop, error || noop);
        };
        self.GetLastTestSuplemental = function (creator, date, success, error) {
            return resource.GetLastTestCase({ action: 'GetLastTestSuplemental', creator: creator, date: date }, success || noop, error || noop)
        };
        self.GetProject = function (idtp, idts, success, error) {
            return resource.GetProject({ action: 'GetProject', idtp: idtp, idts: idts }, success || noop, error || noop)
        };
        self.GetProcedures = function (idstp, success, error) {
            return resource.GetProcedures({ action: 'GetProcedures', idstp: idstp }, success || noop, error || noop)
        };
        self.GetScenarios = function (idstp, success, error) {
            return resource.GetScenarios({ action: 'GetScenarios', idstp: idstp }, success || noop, error || noop)
        };
        self.GetForTestProcedure = function (id, success, error) {
            return resource.GetScenarios({ action: 'GetForTestProcedure', idTp: id }, success || noop, error || noop)
        };
        self.GetForTestScenario = function (id, success, error) {
            return resource.GetScenarios({ action: 'GetForTestScenario', idTs: id }, success || noop, error || noop)
        };
        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };
        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };
        self.UpdateNumber = function (entity, success, error) {
            return resource.UpdateNumber({ action: 'UpdateNumber' }, entity, success || noop, error || noop);
        };

        self.GetByNumber = function (idProject, number, success, error) {
            return resource.GetByNumber({ action: 'GetByNumber', idProject: idProject, number: number }, success || noop, error || noop);
        };
    }]);
})(angular);