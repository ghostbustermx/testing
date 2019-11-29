(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('TagService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/tag/:action/:id',
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

                GetTestCaseTags: {
                    method: 'GET',
                    isArray: true
                },
                GetTestScenarioTags: {
                    method: 'GET',
                    isArray: true
                },
                GetTestProcedureTags: {
                    method: 'GET',
                    isArray: true
                },
                GetTestSuplementalTags: {
                    method: 'GET',
                    isArray: true
                }
            }),

            self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };

        self.GetTestCaseTags = function (idtc, success, error) {
            return resource.GetTestCaseTags({ action: 'GetTestCaseTags', idtc: idtc }, success || noop, error || noop);
        };

        self.GetTestScenarioTags = function (idts, success, error) {
            return resource.GetTestCaseTags({ action: 'GetTestScenarioTags', idts: idts }, success || noop, error || noop);
        };

        self.GetTestProcedureTags = function (idtp, success, error) {
            return resource.GetTestCaseTags({ action: 'GetTestProcedureTags', idtp: idtp }, success || noop, error || noop);
        };

        self.GetTestSuplementalTags = function (idstp, success, error) {
            return resource.GetTestCaseTags({ action: 'GetTestSuplementalTags', idstp: idstp }, success || noop, error || noop);
        };

        self.Get = function (id, success, error) {
            return resource.Get({ action: 'Get', id: id }, success || noop, error || noop);
        };

        self.Delete = function (idTag, idtc, idts, idtp,idstp, success, error) {
            return resource.Delete({ action: 'Delete', idTag: idTag, idtc: idtc, idts: idts, idtp: idtp, idstp: idstp }, success || noop, error || noop);
        };

        self.Save = function (entity, idtc, idts, idtp, idstp, success, error) {
            return resource.Save({ action: 'Save', idtc: idtc, idts: idts, idtp: idtp, idstp: idstp }, entity, success || noop, error || noop);
        };

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };

    }]);
})(angular);