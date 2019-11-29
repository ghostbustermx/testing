(function (angular, undefined) {
    'use strict';

    var app = angular.module(appName);


    app.service('ValidationService', ['$resource', function ($resource) {
        var noop = angular.noop;
        var resource = $resource('/api/WebValidation/:action',
            {}, {
            GetFilesValidation: {
                method: 'Get',
                isArray: false
            }, GetScriptsValidation: {
                method: 'Get',
                isArray: false
            }, GetFilesNumber: {
                method: 'Get',
                isArray: false
            }, GetFilesAllowed: {
                method: 'Get',
                isArray: false
            }

        }), self = this;

        self.GetFilesValidation = function (success, error) {
            resource.GetFilesValidation({ action: 'GetFilesValidation' }, success || noop, error || noop);
        }

        self.GetScriptsValidation = function (success, error) {
            resource.GetScriptsValidation({ action: 'GetScriptsValidation' }, success || noop, error || noop);
        }
        self.GetFilesNumber = function (key, success, error) {
            resource.GetFilesNumber({ action: 'GetFilesNumber', WebKey: key }, success || noop, error || noop);
        }

        self.GetFilesAllowed = function (key, success, error) {
            resource.GetFilesAllowed({ action: 'GetFilesAllowed', WebKey: key }, success || noop, error || noop);
        }

    }])

})(angular);