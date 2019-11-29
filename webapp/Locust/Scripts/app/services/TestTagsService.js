(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('TestTagsService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/test_tags/:action/:id',
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

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };

    }]);
})(angular);