(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('BackupService', ['$resource', function ($resource) {
        
        var noop = angular.noop;
        var resource = $resource('/api/Backup/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },     
                Get: {
                    method: 'GET',
                    isArray: false
                },
                GetAll: {
                    method: 'GET',
                    isArray: true
                },
                Download: {
                    method: 'GET',
                    responseType: 'arraybuffer',
                }

            }),

            self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };

        self.Get = function (id, success, error) {
            return resource.Get({ action: 'Get', id: id }, success || noop, error || noop);
        };

        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };

        self.Download = function (entity, success, error) {
            return resource.Download({ action: 'Download' }, entity, success || noop, error || noop);
        };
    }]);
})(angular);