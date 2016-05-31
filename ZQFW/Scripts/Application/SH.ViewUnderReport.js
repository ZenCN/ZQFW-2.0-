window.App = angular.module('ViewUnderReport', []);

App.controller("MainCtrl", function ($scope) {
    window.$scope = $scope;
    $scope.Open = {
        Report: {
            Current: window.report
        }
    }

    $scope.Open.Report.Current.Attr = {};

    if ($scope.Open.Report.Current.ReportTitle.ORD_Code == 'SH03') {
        $scope.Open.Report.Current.Attr.TableIndex = 5;
    } else {
        $scope.Open.Report.Current.Attr.TableIndex = 0;
    }

    $scope.BaseData = {
        Page: App.Models.Page
    };
});

