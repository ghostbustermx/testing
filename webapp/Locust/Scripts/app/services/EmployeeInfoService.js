(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('EmployeeInfoService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Janus/:action/:id',
            { id: '@Id' }, {

                GetEmployeeInfo: {
                    method: 'GET',
                    isArray: false
                },
                GetEmployees: {
                    method: 'GET',
                    isArray: false
                },  
            }),

            self = this;

        self.GetEmployeeInfo = function (success, error) {
            return resource.GetEmployeeInfo({ action: 'GetEmployeeInfo' }, success || noop, error || noop);
        };
        self.GetEmployees = function (success, error) {
            return resource.GetEmployees({ action: 'GetEmployees' }, success || noop, error || noop);
        };            
    }]);
})(angular);