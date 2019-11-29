(function () {
    "use strict";

    window.virtualPath = window.virtualPath || '/';

    // Register Main app module
    var app = angular.module(appName, ['ui.router', 'ngResource', 'ui.bootstrap', '720kb.datepicker', '720kb.tooltips', 'ngMessages', 'ngToast', 'angular-loading-bar', 'angularUtils.directives.dirPagination', 'wt.responsive', 'ngTagsInput', 'ui.select', 'angular-steps','ngAnimate']).config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
        cfpLoadingBarProvider.parentSelector = '#loading-bar-container';
        cfpLoadingBarProvider.includeSpinner = false;
    }])
        .config(['tooltipsConfProvider', function configConf(tooltipsConfProvider) {
            tooltipsConfProvider.configure({
                'smart': true,
                'size': 'medium',
                'speed': 'fast',
                'side': 'right',
                'tooltipTemplateUrlCache': true
                //etc...
            });
        }])
    app.run(["$rootScope", "$window", function ($rootScope, $window) {

        $rootScope.safeApply = function (fn) {
            var phase = this.$root.$$phase;
            if (phase === "$apply" || phase === "$digest") {
                if (typeof (fn) === "function") {
                    fn.apply(this);
                }
            } else {
                this.$apply(fn);
            }
        };


        //if (!String.prototype.format) {
        //    String.prototype.format = function () {
        //        var args = arguments;
        //        return this.replace(/{(\d+)}/g, function (match, number) {
        //            return typeof args[number] != 'undefined'
        //              ? args[number]
        //              : match
        //            ;
        //        });
        //    };
        //}
        /*
		// When using 'ngRoute'
		$rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
			if (current.$$route.title) {
				$window.document.title = current.$$route.title;
			}
		});
		*/
        // When using 'ui.route'
        $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
            if (toState.data && toState.data.title) {
                $window.document.title = toState.data.title;
            }
        });
    }]);


    app.filter('TCFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.tc_number.substring(0, item.tc_number.length))
                        || stringMatch.test(item.Test_Priority.substring(0, item.Test_Priority.length))
                        || stringMatch.test(item.Title.substring(0, item.Title.length))
                        || stringMatch.test(item.Test_Case_Creator.substring(0, item.Test_Case_Creator.length))) {
                        filtered.push(item);
                    }
                }
            }

            return filtered;
        };
    });


    app.filter('GroupsFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.Name.substring(0, item.Name.length))
                        || stringMatch.test(item.Release.substring(0, item.Release.length))
                        || stringMatch.test(item.Creator.substring(0, item.Creator.length))) {
                        filtered.push(item);
                    }
                }
            }

            return filtered;
        };
    });

    app.filter('ExecutionGroups', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.Name.substring(0, item.Name.length))
                        || stringMatch.test(item.Description.substring(0, item.Description.length))) {
                        filtered.push(item);
                    }
                }
            }

            return filtered;
        };
    });


    app.filter('TestResults', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.Title.substring(0, item.Title.length))
                        || stringMatch.test(item.Identifier_number.substring(0, item.Identifier_number.length))) {
                        filtered.push(item);
                    }
                }
            }

            return filtered;
        };
    });

    app.filter('orderObjectBy', function () {
        return function (items, field, reverse) {
            var filtered = [];
            angular.forEach(items, function (item) {
                filtered.push(item);
            });
            filtered.sort(function (a, b) {
                return (a[field] > b[field] ? 1 : -1);
            });
            if (reverse) filtered.reverse();
            return filtered;
        };
    });

    app.filter('TPFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.tp_number.substring(0, item.tp_number.length))
                        || stringMatch.test(item.Test_Priority.substring(0, item.Test_Priority.length))
                        || stringMatch.test(item.Title.substring(0, item.Title.length))
                        || stringMatch.test(item.Test_Procedure_Creator.substring(0, item.Test_Procedure_Creator.length))) {
                        filtered.push(item);
                    }
                }
            }

            return filtered;
        };
    });

    app.filter('STPFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.stp_number.substring(0, item.stp_number.length))
                        || stringMatch.test(item.Title.substring(0, item.Title.length))
                        || stringMatch.test(item.Test_Procedure_Creator.substring(0, item.Test_Procedure_Creator.length))) {
                        filtered.push(item);
                    }
                }
            }


            return filtered;
        };
    });

    app.filter('TSFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.ts_number.substring(0, item.ts_number.length))
                        || stringMatch.test(item.Test_Priority.substring(0, item.Test_Priority.length))
                        || stringMatch.test(item.Title.substring(0, item.Title.length))
                        || stringMatch.test(item.Test_Scenario_Creator.substring(0, item.Test_Scenario_Creator.length))) {
                        filtered.push(item);
                    }
                }
            }
            return filtered;
        };
    });

    app.filter('REQFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    var task = item.Axosoft_Task_Id.toString();
                    if (stringMatch.test(task.substring(0, task.length))
                        || stringMatch.test(item.req_number.substring(0, item.req_number.length))
                        || stringMatch.test(item.Name.substring(0, item.Name.length))
                        || stringMatch.test(item.Developer_Assigned.substring(0, item.Developer_Assigned.length))
                        || stringMatch.test(item.Tester_Assigned.substring(0, item.Tester_Assigned.length))
                    ) {
                        filtered.push(item);
                    }
                }
            }
            return filtered;
        };
    });

    app.filter('PROJFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.Name.substring(0, item.Name.length))

                    ) {
                        filtered.push(item);
                    }
                }
            }
            return filtered;
        };
    });

    app.filter('UserFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.UserName.substring(0, item.UserName.length))
                        || stringMatch.test(item.Role.substring(0, item.Role.length))
                        || stringMatch.test(item.Email.substring(0, item.Email.length))
                        || stringMatch.test(item.FirstName.substring(0, item.FirstName.length))
                        || stringMatch.test(item.LastName.substring(0, item.LastName.length))


                    ) {
                        filtered.push(item);
                    }
                }
            }
            return filtered;
        };
    });

    app.filter('optFilter', function () {
        return function (items, string) {
            var filtered = [];
            if (items != null) {
                var stringMatch = new RegExp(string, 'i');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    if (stringMatch.test(item.substring(0, item.length))
                    ) {
                        filtered.push(item);
                    }
                }
            }
            return filtered;
        };
    });





    app.directive('autoFocus', ['$timeout', function ($timeout) {
        return {
            restrict: 'AC',
            link: function (_scope, _element) {
                $timeout(function () {
                    _element[0].focus();
                }, 0);
            }
        };
    }]);


})();
