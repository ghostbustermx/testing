(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('DashboardService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Dashboard/:action/:id',
            { id: '@Id' }, {
                GetDashboard: {
                    method: 'GET',
                    isArray: false
                }
                }),
            self = this;

        self.GetDashboard = function (id, success, error) {
            return resource.GetDashboard({ action: 'GetDashboard', id: id }, success || noop, error || noop);
        };

        

    }]);
})(angular);