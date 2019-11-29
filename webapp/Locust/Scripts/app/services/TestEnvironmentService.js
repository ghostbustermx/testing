(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('TestEnvironmentService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/TestEnvironment/:action/:id',
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
                },
                GetActives: {
                    method: 'GET',
                    isArray: true
                },
                GetInactives: {
                    method: 'GET',
                    isArray: true
                },
                GetOS: {
                    method: 'GET',
                    isArray: true
                },
                GetTETypes: {
                    method: 'GET',
                    isArray: true
                },
                HasRelationships: {
                    method: 'GET',
                    isArray: true
                }


            });
        self = this;

        self.Get = function (id, success, error) {
            return resource.Get({ action: 'Get', id: id }, success || noop, error || noop);
        };

        self.GetActives = function (id, success, error) {
            return resource.GetActives({ action: 'GetActives', id: id }, success || noop, error || noop);
        };

        self.GetInactives = function (id, success, error) {
            return resource.GetInactives({ action: 'GetInactives', id: id }, success || noop, error || noop);
        };


        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };
        self.GetOS = function (entity, success, error) {
            return resource.GetOS({ action: 'GetOS' }, entity, success || noop, error || noop);
        };
        self.GetTETypes = function (entity, success, error) {
            return resource.GetTETypes({ action: 'GetTETypes' }, entity, success || noop, error || noop);
        };

        self.HasRelationships = function (id, success, error) {
            return resource.HasRelationships({ action: 'HasRelationships', id: id }, success || noop, error || noop);
        };

    }]);
})(angular);