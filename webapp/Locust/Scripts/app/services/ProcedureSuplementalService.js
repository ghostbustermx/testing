(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('ProcedureSuplementalService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/ProcedureSuplemental/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                Update: {
                    method: 'PUT'
                },
                DeleteTP: {
                    method: 'DELETE'
                },
                DeleteTS: {
                    method: 'DELETE'
                },
                GetTP: {
                    method: 'GET',
                    isArray: false
                },
                GetTS: {
                    method: 'GET',
                    isArray: false
                },
                GetAll: {
                    method: 'GET',
                    isArray: true
                },
                GetForTP: {
                    method: 'GET',
                    isArray: true
                },
                GetForTS: {
                    method: 'GET',
                    isArray: true
                },
                DisableForTp: {
                    method: 'GET',
                    isArray: false
                },
                DisableForTs: {
                    method: 'GET',
                    isArray: false
                },
                EnableForTp: {
                    method: 'GET',
                    isArray: false
                },
                EnableForTs: {
                    method: 'GET',
                    isArray: false
                }

            }),

            self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };

        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };

        self.DeleteTP = function (idtp, idstp, success, error) {
            return resource.DeleteTP({ action: 'DeleteTP', idtp: idtp, idstp: idstp }, success || noop, error || noop);
        };

        self.DeleteTS = function (idts, idstp, success, error) {
            return resource.DeleteTS({ action: 'DeleteTS', idts: idts, idstp: idstp }, success || noop, error || noop);
        };

        self.GetTP = function (idtp, idstp, success, error) {
            return resource.GetTP({ action: 'GetTP', idtp: idtp, idstp: idstp }, success || noop, error || noop);
        };

        self.GetTS = function (idts, idstp, success, error) {
            return resource.GetTS({ action: 'GetTS', idts: idts, idstp: idstp }, success || noop, error || noop);
        };

        self.GetForTP = function (idtp, success, error) {
            return resource.GetForTP({ action: 'GetForTP', idtp: idtp }, success || noop, error || noop);
        };

        self.DisableForTp = function (tpId, success, error) {
            return resource.DisableForTp({ action: 'DisableForTp', tpId: tpId }, success || noop, error || noop);
        };

        self.EnableForTp = function (tpId, success, error) {
            return resource.EnableForTp({ action: 'EnableForTp', tpId: tpId }, success || noop, error || noop);
        };

        self.GetForTS = function (idts, success, error) {
            return resource.GetForTS({ action: 'GetForTS', idts: idts }, success || noop, error || noop);
        };

        self.DisableForTs = function (tsId, success, error) {
            return resource.DisableForTs({ action: 'DisableForTs', tsId: tsId }, success || noop, error || noop);
        };

        self.EnableForTs = function (tsId, success, error) {
            return resource.EnableForTs({ action: 'EnableForTs', tsId: tsId }, success || noop, error || noop);
        };

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };

    }]);
})(angular);