(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('SettingsService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Settings/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                Update: {
                    method: 'PUT'
                },
                Get: {
                    method: 'GET',
                    isArray: false
                }
            });

        self = this;

        self.Get = function (success, error) {
            return resource.Get({ action: 'Get' }, success || noop, error || noop);
        };
        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };
        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };
    }]);
})(angular);