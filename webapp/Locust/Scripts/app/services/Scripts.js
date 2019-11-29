(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('ScriptsService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Scripts/:action/:id',
            { id: '@Id' }, {

                GetAllScripts: {
                    method: 'GET',
                    isArray: true
                },
                Save:{
                    method:'POST'
                 },
                Delete:{
                    method:'DELETE'
                }
            }),

            self = this;

        self.GetAllScripts = function (id, success, error) {
            return resource.GetAllScripts({ action: 'GetAllScripts', ScriptGroupID: id }, success || noop, error || noop);
        };


        self.Save = function (projectId, entity, success, error) {
            return resource.Save({ action: 'Save', projectId: projectId }, entity, success || noop, error || noop);
        };

        self.Delete = function (scriptId, success, error) {
            return resource.Delete({ action: 'Delete', scriptId: scriptId }, success || noop, error || noop);
        };
    }]);
})(angular);