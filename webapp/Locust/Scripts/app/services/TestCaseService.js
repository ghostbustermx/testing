(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('TestCaseService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/TestCase/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                Restore: {
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
                Enable: {
                    method: 'GET',
                    isArray: false
                },
                GetLastTestCase: {
                    method: 'GET',
                    isArray: false
                }
                ,
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
                GetPtojectRequirement: {
                    method: 'GET',
                    isArray: false
                },
                AddChangeLog: {
                    method: 'GET',
                    isArray: false
                },
                TestCaseChangeLogs: {
                    method: 'GET',
                    isArray: true
                },
                GetRequirementForTc: {
                    method: 'GET',
                    isArray:false
                }
            }),

            self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };

        self.Get = function (id, success, error) {
            return resource.Get({ action: 'Get', id: id }, success || noop, error || noop);
        };
        self.GetLastOne = function (idReq, creator, date, success, error) {
            return resource.GetLastOne({ action: 'GetLastOne', idReq: idReq, creator: creator, date : date }, success || noop, error || noop);
        };
        self.TestCaseChangeLogs = function (id, success, error) {
            return resource.TestCaseChangeLogs({ action: 'TestCaseChangeLogs', id: id }, success || noop, error || noop);
        };
        self.GetRequirement = function (reqId, success, error) {
            return resource.GetRequirement({ action: 'GetRequirement', reqId: reqId }, success || noop, error || noop);
        };
        self.Delete = function (id, success, error) {
            return resource.Delete({ action: 'Delete', id: id }, success || noop, error || noop);
        };
        self.Enable = function (id, success, error) {
            return resource.Enable({ action: 'Enable', id: id }, success || noop, error || noop);
        };
        self.AddChangeLog = function (id, success, error) {
            return resource.AddChangeLog({ action: 'AddChangeLog', id: id }, success || noop, error || noop);
        };
        self.GetLastTestCase = function (creator, date, success, error) {
            return resource.GetLastTestCase({ action: 'GetLastTestCase', creator: creator, date: date }, success || noop, error || noop)
        };
        self.GetProject = function (idtc, success, error) {
            return resource.GetProject({ action: 'GetProject', idtc: idtc }, success || noop, error || noop)
        };

        self.GetProjectRequirement = function (idreq, success, error) {
            return resource.GetProject({ action: 'GetProjectRequirement', idreq: idreq }, success || noop, error || noop)
        };
        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };

        self.Restore = function (entity, success, error) {
            return resource.Restore({ action: 'Restore' }, entity, success || noop, error || noop);
        };
        
        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };

        self.GetRequirementForTc = function (tc, success, error) {
            return resource.GetRequirement({ action: 'GetRequirementForTc', tc: tc }, success || noop, error || noop);
        };

        self.UpdateNumber = function (entity, idReq, success, error) {
            return resource.UpdateNumber({ action: 'UpdateNumber', idReq: idReq }, entity, success || noop, error || noop);
        }
    }]);
})(angular);