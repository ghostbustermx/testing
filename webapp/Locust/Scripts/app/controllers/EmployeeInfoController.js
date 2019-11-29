(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName);
    app.controller('EmployeeInfoController', ['$scope', 'EmployeeInfoService', 'AccessService', 'SettingsService', '$window', function ($scope, EmployeeInfoService, AccessService, SettingsService, $window) {
        $scope.info = [];
        $scope.items = [];
        GetTemplate();
        $scope.hasPermissionUsers = false;
        $scope.hasPermissionBackup = false;
        $scope.hasPermissionRunners = false;

        $scope.states = {};
        $scope.states.activeItem = 1;
        HasAccessUsersSection();
        HasAccessBackupSection();
        HasAccessRunnersSection();

        GetEmployeeInfo();


        function GetTemplate() {
            SettingsService.Get(function (data) {
                var uimode = data.UIMode;
                switch (uimode) {
                    case "Blue":
                        $scope.style = true;
                        break;
                    case "Dark":
                        $scope.style = false;
                        break;
                }
            }, function (error) {
                ModeBlue();
            });
        }

        async function GetEmployeeInfo() {
            EmployeeInfoService.GetEmployeeInfo(function (data) {
                $scope.info = data;
            }, function (error) {
            });
        }

        async function HasAccessUsersSection() {
            AccessService.HasAccessUserSection(function (data) {
                $scope.hasPermissionUsers = data.HasPermission;
                GetMenu();
            }, function (error) {
                $scope.hasPermissionUsers = false;
            });
        }


        function HasAccessBackupSection() {
            AccessService.HasAccessBackupSection(function (data) {
                $scope.hasPermissionBackup = data.HasPermission;
                GetMenu();
            }, function (error) {
                $scope.hasPermissionBackup = false;
            });
        }

        function HasAccessRunnersSection() {
            AccessService.HasAccessRunnerSection(function (data) {
                $scope.hasPermissionRunners = data.HasPermission;
                GetMenu();
            }, function (error) {
                $scope.hasPermissionRunners = false;
            });
        }

        function GetMenu() {
            $scope.items = [{
                id: 1,
                title: 'Projects',
                icon: 'fa fa-globe',
                ref: 'Projects',
                hasPermission: true
            }, {
                id: 2,
                title: 'Users',
                icon: 'fa fa-users',
                ref: 'Users',
                hasPermission: $scope.hasPermissionUsers

            }, {
                id: 3,
                title: 'Backups',
                icon: 'fa fa-hdd-o',
                ref: 'Backups',
                hasPermission: $scope.hasPermissionBackup

            },
            {
                id: 4,
                title: 'Runners',
                icon: 'fa fa-server',
                ref: 'Runners',
                hasPermission: $scope.hasPermissionRunners
            }];
        }
        var elem = document.documentElement;
        $scope.isFullscreen = false;

        $scope.SetStyle = function () {
            if (!$scope.style) {
                $scope.mode = "Blue";
                Update()
                ModeBlue();
                $scope.style = true;
            } else {
                $scope.mode = "Dark";
                Update()
                ModeDark();
                $scope.style = false;
            }
        };

        function Update() {
            var settings = {
                UserName: "none",
                UIMode: $scope.mode
            };
            SettingsService.Update(settings, function (data) {

            }, function (error) {

            });
        }

        function ModeBlue() {
            $('head').find('link#dark').remove();
            $('head').append('<link id="blue" rel="stylesheet" type="text/css" href="../Content/css/mode1.css">');

        };
        function ModeDark() {
            $('head').find('link#blue').remove();
            $('head').append('<link id="dark" rel="stylesheet" type="text/css" href="../Content/css/mode2.css">');
        };


        $scope.FullScreen = function () {
            // ## The below if statement seems to work better ## if ((document.fullScreenElement && document.fullScreenElement !== null) || (document.msfullscreenElement && document.msfullscreenElement !== null) || (!document.mozFullScreen && !document.webkitIsFullScreen)) {
            if ((document.fullScreenElement !== undefined && document.fullScreenElement === null) || (document.msFullscreenElement !== undefined && document.msFullscreenElement === null) || (document.mozFullScreen !== undefined && !document.mozFullScreen) || (document.webkitIsFullScreen !== undefined && !document.webkitIsFullScreen)) {
                if (elem.requestFullScreen) {
                    elem.requestFullScreen();
                } else if (elem.mozRequestFullScreen) {
                    elem.mozRequestFullScreen();
                } else if (elem.webkitRequestFullScreen) {
                    elem.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
                } else if (elem.msRequestFullscreen) {
                    elem.msRequestFullscreen();
                }
            } else {
                if (document.cancelFullScreen) {
                    document.cancelFullScreen();
                } else if (document.mozCancelFullScreen) {
                    document.mozCancelFullScreen();
                } else if (document.webkitCancelFullScreen) {
                    document.webkitCancelFullScreen();
                } else if (document.msExitFullscreen) {
                    document.msExitFullscreen();
                }
            }
        }


        $scope.CopyURL = function () {
            //----< copy_Share_Button() >----
            var sURL = window.location.href;
            var sTemp = "<input id=\"copy_to_Clipboard\" value=\"" + sURL + "\" />"
            $("body").append(sTemp);
            $("#copy_to_Clipboard").select();
            document.execCommand("copy");
            $("#copy_to_Clipboard").remove();
            $window.open(sURL, "_blank", "location=yes");
            //----</ copy_Share_Button() >----
        };
    }])
})();