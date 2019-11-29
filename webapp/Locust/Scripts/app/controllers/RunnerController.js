(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName)

    app.controller('RunnerController', ['$scope', 'ngToast', 'RunnerService', 'AccessService', 'DateParse', '$timeout', function ($scope, ngToast, RunnerService, AccessService, DateParse, $timeout) {
        $scope.currentPage = 1;
        $scope.pageSize = 10;
        HasAccess();
        //Start
        $scope.AllRunners = [];
        var loadTime = 10000, //Load the data every second
            errorCount = 0, //Counter for the server errors
            loadPromise; //Pointer to the promise created by the Angular $timout service


        function GetData() {
            if ($scope.RowsFilterSelected == 'Enable') {
                GetActivesRunners();

            } else {
                GetInactivesRunners();
            }
            errorCount = 0;
            nextLoad();
        }


        var cancelNextLoad = function () {
            $timeout.cancel(loadPromise);
        };


        var nextLoad = function (mill) {
            mill = mill || loadTime;

            //Always make sure the last timeout is cleared before starting a new one
            cancelNextLoad();
            loadPromise = $timeout(GetData, mill);
        };

        var cancelNextLoad = function () {
            $timeout.cancel(loadPromise);
        };

        $scope.runnerSelected = "";
        $scope.action = "";

        $scope.SelectRunner = function (runner, actions) {
            $scope.action = actions;
            $scope.runnerSelected = runner;
        };


        function GetRunnerFilter() {
            $scope.ActivesOption = ['Enable', 'Disable'];
            $scope.RowsFilterSelected = $scope.ActivesOption[0];
        };
        $scope.selection = "";
        $scope.runnerFilter = function (selection) {
            if (selection == 'Enable') {
                $scope.selection = "Enable"
                GetActivesRunners();
            }
            if (selection == 'Disable') {
                $scope.selection = "Disable"

                GetInactivesRunners();

            }
        };

        $scope.DisableEnableRunner = function (runner) {
            if ($scope.action != "Delete") {
                var status = false;
                var IsConnected = false;

                if ($scope.action == "Enable") {
                    status = true;
                }
                runner.Status = status;
                runner.IsConnected = IsConnected;

                RunnerService.Update(runner, function (data) {
                    if ($scope.action == "Enable") {
                        GetInactivesRunners();

                    } else {
                        GetActivesRunners();

                    }

                });
            } else {
                RunnerService.Delete(runner.Id, function (data) {
                    if ($scope.selection == "Enable") {
                        GetActivesRunners();
                    } else {
                        GetInactivesRunners();
                    }

                });
            }
        }

        //Always clear the timeout when the view is destroyed, otherwise it will keep polling
        $scope.$on('$destroy', function () {
            cancelNextLoad();
        });



        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelected = $scope.Options[0];
            }, function (error) {
            });
        };

        function GetActivesRunners() {
            RunnerService.GetActives(function (data) {
                data.forEach(function (element) {
                    element.Creation_Date = DateParse.GetDate(element.Creation_Date);
                });
                $scope.runners = data;
            }, function (error) {

            });
        };

        function GetInactivesRunners() {
            RunnerService.GetInactives(function (data) {
                data.forEach(function (element) {
                    element.Creation_Date = DateParse.GetDate(element.Creation_Date);
                });
                $scope.runners = data;
            }, function (error) {

            });
        };

        function HasAccess() {
            AccessService.HasAccessBackupSection(function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    GetPaginationElements();
                    GetRunnerFilter();
                    GetData();
                }
            }, function (error) {
            });
        };
    }])
})();