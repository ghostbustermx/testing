(function (angular, undefined) {
    'use strict';

    var app = angular.module(appName);

    app.service('ExecutionGroupService', ['$resource', function ($resource) {

        var noop = angular.noop;

        var resource = $resource('/api/ExecutionGroup/:action/:id',
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
                Enable: {
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
                GetByProjectActives: {
                    method: 'GET',
                    isArray: true
                }, GetByProjectInactives: {
                    method: 'GET',
                    isArray: true
                }
            });

        self = this;


        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };

        self.GetByProjectActives = function (projectId, success, error) {
            return resource.GetAll({ action: 'GetByProjectActives', projectId: projectId }, success || noop, error || noop);
        };

        self.GetByProjectInactives = function (projectId, success, error) {
            return resource.GetAll({ action: 'GetByProjectInactives', projectId: projectId }, success || noop, error || noop);
        };


        self.Get = function (id, success, error) {
            return resource.Get({ action: 'Get', executionId: id }, success || noop, error || noop);
        };

        self.Delete = function (id, success, error) {
            return resource.Delete({ action: 'Delete', id: id }, success || noop, error || noop);
        };

        self.Enable = function (id, success, error) {
            return resource.Enable({ action: 'Enable', id: id }, success || noop, error || noop);
        };

        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };
        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };

    }])

})(angular);