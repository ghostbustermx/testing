(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('ProfileController', ['$scope', 'DateParse', 'EmployeeInfoService', 'SettingsService', function ($scope, DateParse, EmployeeInfoService, SettingsService) {
        GetEmployeeInfo();
        async function GetEmployeeInfo() {
            EmployeeInfoService.GetEmployeeInfo(function (data) {
                data.HireDate = DateParse.GetDateWIthoutTime(data.HireDate);
                $scope.info = data;
            }, function (error) {
            });
        }
    }])
})();