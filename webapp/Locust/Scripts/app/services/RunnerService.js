(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('RunnerService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/LocustRunner/:action/:id',
            { id: '@Id' }, {
                GetActives: {
                    method: 'GET',
                    isArray: true
                }, GetFullActives: {
                    method: 'GET',
                    isArray: true
                },
                GetInactives: {
                    method: 'GET',
                    isArray: true
                },
                Get: {
                    method: 'GET',
                    isArray: false
                },
                Update: {
                    method: 'PUT'
                },
                Delete: {
                    method: 'DELETE',
                    params: {
                        id: "@id"
                    }
                }
            }),

            self = this;

        self.GetActives = function (success, error) {
            return resource.GetActives({ action: 'GetActives' }, success || noop, error || noop);
        };

        self.GetFullActives = function (success, error) {
            return resource.GetFullActives({ action: 'GetFullActives' }, success || noop, error || noop);
        };
        self.GetInactives = function (success, error) {
            return resource.GetInactives({ action: 'GetInactives' }, success || noop, error || noop);
        };

        self.Delete = function (id, success, error) {
            return resource.Delete({ action: 'Delete', id: id }, success || noop, error || noop);
        };

        self.Update = function (runner, success, error) {
            return resource.Update({ action: 'Update' }, runner, success || noop, error || noop);
        };

        self.Get = function (id, success, error) { 
            return resource.Get({ action: 'Get', id:id }, success || noop, error || noop);
        };

    }]);
})(angular);