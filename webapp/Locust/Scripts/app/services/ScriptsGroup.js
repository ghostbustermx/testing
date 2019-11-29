(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('ScriptsGroupService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/ScriptsGroup/:action/:id',
            { id: '@Id' }, {

                GetAllScriptsGroup: {
                    method: 'GET',
                    isArray: true
                }, Save: {
                    method:'POST'
                }, Update: {
                    method:'PUT'
                }, Get: {
                    method: 'GET',
                    isArray: false
                } 
            }),

            self = this;

        self.GetAllScriptsGroup = function (id, success, error) {
            return resource.GetAllScriptsGroup({ action: 'GetAllScriptsGroup', projectId: id }, success || noop, error || noop);
        };

        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save'}, entity, success || noop, error || noop);
        };

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update'}, entity, success || noop, error || noop);
        };

        self.Get = function (scriptGroupId, success, error) {
            return resource.Get({ action: 'Get', scriptGroupId: scriptGroupId }, success || noop, error || noop);
        };
    }]);
})(angular);