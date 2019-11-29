(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('UserService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Users/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                Update: {
                    method: 'PUT'
                },
                Disable: {
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
                GetUsersProjects: {
                    method: 'GET',
                    isArray: true
                },
                GetUsersByProject: {
                    method: 'GET',
                    isArray: true
                },
                GetCurrentUser: {
                method: 'GET',
                isArray: false
            }
            });



        
        self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };

        self.GetUsersProjects = function (id, success, error) {
            return resource.GetUsersProjects({ action: 'GetUsersProjects', id: id }, success || noop, error || noop);
        };

        self.GetUsersByProject = function (id, success, error) {
            return resource.GetUsersByProject({ action: 'GetUsersByProject', id: id }, success || noop, error || noop);
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

        self.Disable = function (entity, success, error) {
            return resource.Update({ action: 'Disable' }, entity, success || noop, error || noop);
        }

        self.GetCurrentUser = function (success, error) {
            return resource.GetCurrentUser({ action: 'GetCurrentUser' }, success || noop, error || noop);
        };
    }]);
})(angular);