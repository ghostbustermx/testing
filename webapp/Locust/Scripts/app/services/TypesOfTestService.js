(function (angular, undefined) {
    'use strict';

    var app = angular.module(appName);


    app.service('TypesOfTestService', ['$resource', function ($resource) {
        var noop = angular.noop;
        var resource = $resource('/api/TestTypes/:action',
            {}, {
                Get: {
                    method: 'Get',
                    isArray: true
                },
                GetAutomated: {
                    method: 'Get',
                    isArray: true
                }

            }), self = this;

        self.Get = function (success, error) {
            resource.Get({ action: 'GetTypes' }, success || noop, error || noop);
        }

        self.GetAutomated = function (TestAutomated ,success, error) {
            resource.GetAutomated({ action: 'GetAutomated', TestAutomated: TestAutomated }, success || noop, error || noop);
        }

    }])

})(angular);