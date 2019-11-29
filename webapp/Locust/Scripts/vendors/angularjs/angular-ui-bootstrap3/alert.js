angular.module('ui.bootstrap.alert', [])

.controller('AlertController', ['$scope', '$attrs', function ($scope, $attrs) {
  $scope.closeable = 'close' in $attrs;
}])

.directive('alert', function () {
  return {
    restrict:'EA',
    controller:'AlertController',
    templateUrl:virtualPath + 'Content/templates3/alert/alert.html',
    transclude:true,
    replace:true,
    scope: {
      type: '@',
      close: '&'
    }
  };
});
