(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('AccessService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Access/:action/:id',
            { id: '@Id' }, {
                HasAccess: {
                    method: 'GET',
                    isArray: false
                },
                GetRole: {
                    method: 'GET',
                    isArray: false
                },
                GetPagination: {
                    method: 'GET',
                    isArray: true
                },
                HasAccessUserSection: {
                    method: 'GET',
                    isArray: false
                },
                HasAccessBackupSection: {
                    method: 'GET',
                    isArray: false
                },
                HasAccessRunnerSection: {
                    method: 'GET',
                    isArray: false
                },
                HasAccessProjectSection: {
                    method: 'GET',
                    isArray: false
                },
                Logout: {
                    method: 'GET',
                    isArray: false
                },

            }),

            self = this;



        self.HasAccess = function (permission, projectId, reqId, TCId, TPId, STPId, TSId, success, error) {
            return resource.HasAccess({ action: 'HasAccess', permission: permission, projectId: projectId, reqId: reqId, TCId: TCId, TPId: TPId, STPId: STPId, TSId: TSId }, success || noop, error || noop);
        };

        self.GetPagination = function (success, error) {
            return resource.GetPagination({ action: 'GetPagination' }, success || noop, error || noop);
        };

        self.GetRole = function (success, error) {
            return resource.GetRole({ action: 'GetRole' }, success || noop, error || noop);
        };

        self.HasAccessUserSection = function (success, error) {
            return resource.HasAccessUserSection({ action: 'HasAccessUserSection' }, success || noop, error || noop);
        };
        self.HasAccessBackupSection = function (success, error) {
            return resource.HasAccessBackupSection({ action: 'HasAccessBackupSection' }, success || noop, error || noop);
        };
        self.HasAccessRunnerSection = function (success, error) {
            return resource.HasAccessRunnerSection({ action: 'HasAccessRunnerSection' }, success || noop, error || noop);
        };
        
        self.HasAccessProjectSection = function (success, error) {
            return resource.HasAccessProjectSection({ action: 'HasAccessProjectSection' }, success || noop, error || noop);
        };
        self.Logout = function (success, error) {
            return resource.Logout({ action: 'Logout' }, success || noop, error || noop);
        };
        
    }]);
})(angular);