(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('TraceabilityFindingService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/TraceabilityFinding/:action/:id',
            { id: '@Id' }, {
                GetTraceabilityFindings: {
                    method: 'GET',
                    isArray: false
                }
                }),
            self = this;

        self.GetTraceabilityFindings = function (id, success, error) {
            return resource.GetTraceabilityFindings({ action: 'GetTraceabilityFindings', id: id }, success || noop, error || noop);
        };
    }]);
})(angular);