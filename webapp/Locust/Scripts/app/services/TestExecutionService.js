
(function () {
    'use strict';

    var app = angular.module(appName);

    app.service('TestExecutionService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/TestExecution/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                Get: {
                    method: 'GET',
                    isArray: false
                },
                Update: {
                    method: 'PUT'
                },
                GetForGroup: {
                    method: 'GET',
                    isArray: true
                },
                ChangeState: {
                    method: 'GET',
                    isArray: false
                },
                GetByProject: {
                    method: 'GET',
                    isArray: true
                }
            });
        self = this;



        self.Save = function (entity, success, error) {
            return resource.Save({ action: "Save" }, entity, success || noop, error || noop);
        };

        self.Update = function (entity, success, error) {
            return resource.Update({ action: "Update" }, entity, success || noop, error || noop);
        };

        self.Get = function (id, success, error) {
            return resource.Get({ action: "Get", id: id }, success || noop, error || noop);
        };

        self.GetForGroup = function (executionId, success, error) {
            return resource.GetForGroup({ action: "GetForGroup", executionId: executionId }, success || noop, error || noop);
        };

        self.ChangeState = function (id,state ,success, error) {
            return resource.ChangeState({ action: "ChangeState", id: id, state: state }, success || noop, error || noop);
        };

        self.GetByProject = function (projectId,success, error) {
            return resource.GetByProject({ action: "GetByProject", projectId: projectId}, success || noop, error || noop);
        };

    }]);

})(angular);