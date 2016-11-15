window.App = angular.module('DisasterReview', []).controller("MainCtrl", function ($scope) {
    $scope.Field = window.opener.$scope.BaseData.Field;
    $scope.Unit = window.opener.$scope.BaseData.Unit.Local;
    var hl011 = window.opener.$scope.Open.Report.Current.HL011;
    var hl013 = window.opener.$scope.Open.Report.Current.HL013;
    $scope.Report = {};
    var hl013FieldArr = ["CQZJJJSS", "JZWSYDX", "JZWSYFW", "SMXJT", "SMXGQ", "SMXGD", "SMXGS", "ZYZJZDSS", "GCJJZYRK", "GCHSWKRK", "GCYMLS", "YMFWBL", "YMFWMJ", "SYCS"];
    angular.forEach(App.DisasterReviewModel, function (val, key) {
        if (hl013FieldArr.In_Array(key) && Number(hl013[0][key]) > 0) {
            $scope.Report[key] = hl013[0][key];
        } else if (Number(hl011[0][key]) > 0) {
            $scope.Report[key] = hl011[0][key];
        } else {
            $scope.Report[key] = val;
        }
    });
    $scope.Report.StartDateTime = window.opener.$scope.Open.Report.Current.ReportTitle.StartDateTime;
    $scope.Report.EndDateTime = window.opener.$scope.Open.Report.Current.ReportTitle.EndDateTime;
    $scope.Report.YQQSRQ = $scope.Report.StartDateTime;
    $scope.Report.YQJSRQ = $scope.Report.EndDateTime;
    $scope.Report.ZQJZRQ = $scope.Report.EndDateTime;
    $scope.Report.Limit = $scope.Unit.Limit;
    $scope.Report.UnitName = $scope.Unit.UnitName;
    switch (Number($scope.Unit.Limit)) {
        case 2:
            $scope.Report.SimpleUnitName = "省";
            $scope.Report.SZFWDSS = 0;
            angular.forEach(hl011, function (obj) {
                if (Number(obj.SZFXW) > 0) {
                    $scope.Report.SZFWDSS++;
                }
            });
            break;
        case 3:
            $scope.Report.SimpleUnitName = "市";
            $scope.Report.SZFWDSS = 1;
            break;
        case 4:
            $scope.Report.SimpleUnitName = "县";
            break;
    }

    $scope.Fn = {
        GetEvt: function () {
            return $(event.target || event.srcElement);
        },
        Export: function () {
            window.open("/BaseData/ExportDisasterReview?Report=" + angular.toJson($scope.Report), "", "", "");
        },
        CheckLen: function (len) {
            var arr = this.GetEvt().attr("ng-model").split(".");

            if (typeof $scope[arr[0]][arr[1]] == "string") {
                $scope[arr[0]][arr[1]] = $scope[arr[0]][arr[1]].replaceAll("&", "").replaceAll("#", "");
                if ($scope[arr[0]][arr[1]].length > len) {
                    $scope[arr[0]][arr[1]] = $scope[arr[0]][arr[1]].substr(0, len - 1);
                }
            }
        }
    };

    window.$scope = $scope;
});