(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('AttachmentService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Attachment/:action/:id',
            { id: '@Id' }, {
                RemoveAttachments: {
                    method: 'POST'
                    
                },
                GetAttachment: {
                    method: 'GET', 
                    isArray: true
                }
            }),

            self = this;

        self.RemoveAttachments = function (EntityId, success, error) {
            return resource.RemoveAttachments({ action: 'RemoveAttachments'},EntityId, success || noop, error || noop);
        };

        self.GetAttachment = function (EntityAction, EntityId,success, error) {
            return resource.GetAttachment({ action: 'GetAttachment', EntityAction: EntityAction, EntityId: EntityId  }, success || noop, error || noop);
        };


    }]);
})(angular);