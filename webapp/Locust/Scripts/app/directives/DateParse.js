
var app = angular.module(appName);
app.factory('DateParse', function () {
    return {
        GetDate: function (date) {
            var date = new Date(date);
            var date = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds()));
            return date.toLocaleString();
        }
        ,
        GetDateWIthoutTime: function (date) {

            var date = new Date(date);
            datestring = (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear()

            return datestring;
        }
    };
});
