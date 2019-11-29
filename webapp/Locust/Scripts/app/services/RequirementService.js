(function (angular, undefined) {
    'use strict';

    // Get services module
    var app = angular.module(appName);

    app.service('RequirementService', ['$resource', function ($resource) {

        var noop = angular.noop;
        var resource = $resource('/api/Requirement/:action/:id',
            { id: '@Id' }, {
                Save: {
                    method: 'POST'
                },
                Update: {
                    method: 'PUT'
                },
                Delete: {
                    method: 'DELETE',
                    params: {
                        id: "@id"
                    }
                },
                Enable: {
                    method: 'GET',
                    isArray: false
                },
                Get: {
                    method: 'GET',
                    isArray: false
                },
                GetAll: {
                    method: 'GET',
                    isArray: true
                },
                GetProject: {
                    method: 'GET',
                    isArray: true
                },
                GetProjectInactives: {
                    method: 'GET',
                    isArray: true
                },
                GetAllTestCase: {
                    method: 'GET',
                    isArray: true
                },
                GetAllTestCaseInactives: {
                    method: 'GET',
                    isArray: true
                },
                GetAllTestScenario: {
                    method: 'GET',
                    isArray: true
                },
                GetAllTestScenarioInactives: {
                    method: 'GET',
                    isArray: true
                },
                GetAllTestProcedure: {
                    method: 'GET',
                    isArray: true
                },
                GetAllTestProcedureInactives: {
                    method: 'GET',
                    isArray: true
                },
                RequirementChangeLogs: {
                    method: 'GET',
                    isArray: true
                },
                Restore: {
                    method: 'POST'
                },
                GetRequirementbyTCId: {
                    method: 'GET',
                    isArray: false
                },
                GetRequirementbyTPId: {
                    method: 'GET',
                    isArray: false
                },
                GetAllTCWithoutTP: {
                    method: 'GET',
                    isArray: true
                },
                GetAllTCWithoutTPByReq: {
                    method: 'GET',
                    isArray: true
                },
                GetEvidenceFromReq: {
                    method: 'POST'
                    , isArray: true

                },
                GetAutomatedEvidenceFromReq: {
                    method: 'POST'
                    ,isArray: true
                },
                GetRequirementsByTestEvidence: {
                    method: 'GET'
                    , isArray: true
                },
                GetSprints: {
                    method: 'GET',
                    isArray: true
                },
                GetTestCasesByNumber: {
                    method: 'POST', 
                    isArray:true
                }
            }),

            self = this;

        self.GetAll = function (success, error) {
            return resource.GetAll({ action: 'GetAll' }, success || noop, error || noop);
        };

        self.Get = function (id, success, error) {
            return resource.Get({ action: 'Get', id: id }, success || noop, error || noop);
        };

        self.Enable = function (id, success, error) {
            return resource.Enable({ action: 'Enable', id: id }, success || noop, error || noop);
        };

        self.RequirementChangeLogs = function (id, success, error) {
            return resource.RequirementChangeLogs({ action: 'RequirementChangeLogs', id: id }, success || noop, error || noop);
        };

        self.Restore = function (entity, version, success, error) {
            return resource.Restore({ action: 'Restore', version: version }, entity, success || noop, error || noop);
        };

        self.GetProject = function (Project_Id, success, error) {
            return resource.GetProject({ action: 'GetProject', Project_Id: Project_Id }, success || noop, error || noop);
        };

        self.GetSprints = function (projectId, success, error) {
            return resource.GetProject({ action: 'GetSprints', projectId: projectId }, success || noop, error || noop);
        };

        self.GetProjectInactives = function (Project_Id, success, error) {
            return resource.GetProject({ action: 'GetProjectInactives', Project_Id: Project_Id }, success || noop, error || noop);
        };

        self.GetAllTestCase = function (id, success, error) {
            return resource.GetAllTestCase({ action: 'GetAllTestCase', id: id }, success || noop, error || noop);
        };

        self.GetAllTestCaseInactives = function (id, success, error) {
            return resource.GetAllTestCaseInactives({ action: 'GetAllTestCaseInactives', id: id }, success || noop, error || noop);
        };

        self.GetAllTestScenario = function (id, success, error) {
            return resource.GetAllTestCase({ action: 'GetAllTestScenario', id: id }, success || noop, error || noop);
        };

        self.GetAllTestScenarioInactives = function (id, success, error) {
            return resource.GetAllTestScenarioInactives({ action: 'GetAllTestScenarioInactives', id: id }, success || noop, error || noop);
        };

        self.GetAllTestProcedure = function (id, success, error) {
            return resource.GetAllTestProcedure({ action: 'GetAllTestProcedure', id: id }, success || noop, error || noop);
        };

        self.GetAllTestProcedureInactives = function (id, success, error) {
            return resource.GetAllTestProcedureInactives({ action: 'GetAllTestProcedureInactives', id: id }, success || noop, error || noop);
        };

        self.Delete = function (id, success, error) {
            return resource.Delete({ action: 'Delete', id: id }, success || noop, error || noop);
        };

        self.Save = function (entity, success, error) {
            return resource.Save({ action: 'Save' }, entity, success || noop, error || noop);
        };

        self.Update = function (entity, success, error) {
            return resource.Update({ action: 'Update' }, entity, success || noop, error || noop);
        };

        self.GetRequirementbyTCId = function (id, success, error) {
            return resource.GetRequirementbyTCId({ action: 'GetRequirementbyTCId', id: id }, success || noop, error || noop);
        };

        self.GetRequirementbyTPId = function (id, success, error) {
            return resource.GetRequirementbyTPId({ action: 'GetRequirementbyTPId', id: id }, success || noop, error || noop);
        };

        self.GetAllTCWithoutTP = function (id, success, error) {
            return resource.GetAllTCWithoutTP({ action: 'GetAllTCWithoutTP', projectId: id }, success || noop, error || noop);
        };

        self.GetAllTCWithoutTPByReq = function (id, success, error) {
            return resource.GetAllTCWithoutTP({ action: 'GetAllTCWithoutTPByReq', reqId: id }, success || noop, error || noop);
        };

        self.GetEvidenceFromReq = function (id, success, error) {
            return resource.GetEvidenceFromReq({ action: 'GetEvidenceFromReq' }, id, success || noop, error || noop);
        };

        self.GetAutomatedEvidenceFromReq = function (id, success, error) {
            return resource.GetAutomatedEvidenceFromReq({ action: 'GetAutomatedEvidenceFromReq' }, id, success || noop, error || noop);
        };

        self.GetRequirementsByTestEvidence = function (id, type, success, error) {
            return resource.GetRequirementsByTestEvidence({ action: 'GetRequirementsByTestEvidence', id: id, type: type },success || noop, error || noop);
        };

        self.GetSprints = function (id, success, error) {
            return resource.GetSprints({ action: 'GetSprints', projectId: id},success || noop, error || noop);
        };

        self.GetTestCasesByNumber = function (TcNumbers, id, scriptId ,success, error) {
            return resource.GetTestCasesByNumber({ action: 'GetTestCasesByNumber', projectId: id, scriptId: scriptId}, TcNumbers,success || noop, error || noop);
        };
        
    }]);
})(angular);