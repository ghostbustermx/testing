(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName)
    app.controller('BackupController', ['$scope', 'DateParse', '$window', '$state', 'BackupService', 'AccessService', 'ModalConfirmService', 'ngToast', '$http', function ($scope, DateParse, $window, $state, BackupService, AccessService, ModalConfirmService, ngToast, $http) {
        $scope.currentPage = 1;
        $scope.pageSize = 10;
        $scope.backups = [];
        HasAccess();
        $scope.Options = [];

        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelected = $scope.Options[0];
            }, function (error) {
            });
        }

        $scope.changeValue = function (value) {
            $scope.pageSize = value;
        }


        function GetAllBackups() {
            BackupService.GetAll(function (data) {
                data.forEach(function (element) {
                    element.Creation_Date = DateParse.GetDate(element.Creation_Date);
                });
                $scope.backups = data;
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
                    GetAllBackups();
                }
            }, function (error) {
            });
        };





        $scope.generateBackup = function (description) {

            var Backup = {
                Description: description
            };
            $scope.description = '';

            $("#loadingModalBackup").modal({
                backdrop: "static",
                keyboard: false,
                show: true
            });

            BackupService.Save(Backup, function (data) {
                GetAllBackups();
                $("#loadingModalBackup").modal("hide");
                ngToast.success({
                    content: 'The backup' + data.Name + ' has been created successfully.'
                });

            }, function (response) {
                ngToast.danger({
                    content: 'The backup' + data.Name + ' has not been created.'
                });
            });


        };

        $scope.downloadFile = function (name) {
            $http({
                method: 'GET',
                url: '/api/Backup/Download',
                params: { name: name },
                responseType: 'arraybuffer'
            }).then(function (response) {
                var filename = name;
                var contentType = "application/octet-stream";
                //var contentType = headers['content-type'];

                var linkElement = document.createElement('a');
                try {
                    var blob = new Blob([response.data], { type: contentType });
                    var url = window.URL.createObjectURL(blob);

                    linkElement.setAttribute('href', url);
                    linkElement.setAttribute("download", filename);

                    var clickEvent = new MouseEvent("click", {
                        "view": window,
                        "bubbles": true,
                        "cancelable": false
                    });
                    linkElement.dispatchEvent(clickEvent);
                } catch (ex) {
                    ngToast.danger({
                        content: "Error: " + ex
                    });
                }
            }, function (error) {
                ngToast.danger({
                    content: "Error: " + error.status + " " + error.statusText + " - The " + name + " does not exists on the server."
                });
            });
        };




        $scope.redirectTo = function (url) {
            $window.open(url, '_blank');
        };
    }])
})();