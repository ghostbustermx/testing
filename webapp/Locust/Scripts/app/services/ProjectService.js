(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('ProjectService', ['$resource', function ($resource) {
        
        var noop = angular.noop;
        var resource = $resource('/api/Project/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                GetActives: {
                    method: 'GET',
                    isArray: true
                },
                GetInactives: {
                    method: 'GET',
                    isArray: true
                },
                Update: {
                    method: 'PUT'
                },
                Upload: {
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
                Get: {
                    method: 'GET',
                    isArray: false
                },
                GetAll: {
                    method: 'GET',
                    isArray: true
                },
                uploadFile: {
                    method: 'POST'
                },
                ProjectChangeLogs: {
                    method: 'GET',
                    isArray: true
                },
                Restore: {
                    method: 'POST'
                },
                GetDashboard: {
                    method: 'GET',
                    isArray: false
                },
                GetForUsersList: {
                    method: 'GET',
                    isArray: false
                },
                RequestExcel: {
                    method: 'GET',
                    timeout: 5000,
                    isArray: false
                }
            });
            self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };
        self.ProjectChangeLogs = function (id, success, error) {
            return resource.ProjectChangeLogs({ action: 'ProjectChangeLogs', id: id }, success || noop, error || noop);
        };
        self.GetActives = function (success, error) {
            return resource.GetActives({ action: 'GetActives' }, success || noop, error || noop);
        };
        self.GetForUsersList = function (success, error) {
            return resource.GetActives({ action: 'GetForUsersList' }, success || noop, error || noop);
        };

        self.Restore = function (entity, version, success, error) {
            return resource.Restore({ action: 'Restore', version: version }, entity, success || noop, error || noop);
        };

        self.GetInactives = function (success, error) {
            return resource.GetInactives({ action: 'GetInactives' }, success || noop, error || noop);
        };
        self.Get = function (id, success, error) {
            return resource.Get({ action: 'Get', id: id }, success || noop, error || noop);
        };
        self.Enable = function (id, success, error) {
            return resource.Enable({ action: 'Enable', id: id }, success || noop, error || noop);
        };
        self.Delete = function (id, success, error) {
            return resource.Delete({ action: 'Delete', id: id }, success || noop, error || noop);
        };

        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };

        self.pushFile = function (file, success, error) {
            var formdata = new FormData();
            formdata.append(file.name, file);
            return resource.uploadFile({ action: 'uploadFile' }, formdata, success || noop, error || noop);
        }

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };

        self.GetDashboard = function (projectId, success, error) {
            return resource.GetDashboard({ action: 'GetDashboard', projectId: projectId }, success || noop, error || noop);
        };

        self.RequestExcel = function (success, error) {
            return resource.RequestExcel({ action: 'RequestExcel'}, success || noop, error || noop);
        };
        
    }]);
})(angular);