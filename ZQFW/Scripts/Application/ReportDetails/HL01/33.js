window.App = angular.module('SimpleReport', []).controller("MainCtrl", function ($scope) {
    $scope.Field = window.opener.$scope.BaseData.Field;
    angular.forEach($scope.Field, function (val, key) {
        if ($scope.Field[key].MeasureName != "") {
            $scope.Field[key].MeasureName = $scope.Field[key].MeasureName.replace("（", "").replace("）", "");
        }
    });
    $scope.Unit = window.opener.$scope.BaseData.Unit.Local;
    var hl011 = window.opener.$scope.Open.Report.Current.HL011;
    var hl013 = window.opener.$scope.Open.Report.Current.HL013;
    var hl014 = window.opener.$scope.Open.Report.Current.HL014;

    $scope.Report = {};
    var hl014FieldArr = ["QXHJ", "SBJX", "WZBZD", "WZBZB", "WZSSL", "WZMC", "WZGC", "WZY", "WZD", "WZZXH", "XYJYGD", "XYJMLSJS", "XYJSSZRK", "XYJJQZ", "XYJZJJXY"];
    var hl013FieldArr = ["CQZJJJSS", "JZWSYDX", "JZWSYFW", "SMXJT", "SMXGQ", "SMXGD", "SMXGS", "ZYZJZDSS", "GCJJZYRK", "GCHSWKRK", "GCYMLS", "YMFWBL", "YMFWMJ", "SYCS"];
    angular.forEach(App.DisasterReviewModel, function (val, key) {
        if (hl013FieldArr.In_Array(key) && Number(hl013[0][key]) > 0) {
            $scope.Report[key] = hl013[0][key];
        } else if (hl014FieldArr.In_Array(key) && Number(hl014[0][key]) > 0) {
            $scope.Report[key] = hl014[0][key];
        } else if (Number(hl011[0][key]) > 0) {
            $scope.Report[key] = hl011[0][key];
        } else {
            $scope.Report[key] = val;
        }
    });

    var startDateTime = window.opener.$scope.Open.Report.Current.ReportTitle.StartDateTime;
    var endDateTime = window.opener.$scope.Open.Report.Current.ReportTitle.EndDateTime;
    var writeDateTime = window.opener.$scope.Open.Report.Current.ReportTitle.WriterTime;
    $scope.Report.QSRQ = startDateTime.split("-")[0] + "年" + startDateTime.split("-")[1] + "月" + startDateTime.split("-")[2] + "日";
    $scope.Report.JSRQ = endDateTime.split("-")[0] + "年" + endDateTime.split("-")[1] + "月" + endDateTime.split("-")[2] + "日";
    $scope.Report.TBRQ = writeDateTime.split("-")[0] + "年" + writeDateTime.split("-")[1] + "月" + writeDateTime.split("-")[2].split(" ")[0] + "日";
    $scope.Report.Writer = window.opener.$scope.Open.Report.Current.ReportTitle.UnitName;
    $scope.Report.Limit = $scope.Unit.Limit;
    $scope.Report.UnitName = $scope.Unit.UnitName;
    switch (Number($scope.Unit.Limit)) {
        case 2:
            var strSZFWS = "";
            var SZFWS = 0;
            var SZFWX = 0;
            angular.forEach(hl011, function (obj) {
                if (obj.DW == "合计") {
                    if (Number(obj.SZFWX) > 0) {
                        SZFWX = Number(obj.SZFWX);
                    }
                } else if (Number(obj.SZFWX) > 0) {
                    SZFWS++;
                    strSZFWS += obj.DW + "、";
                }
            });
            if (SZFWS > 0) {
                strSZFWS = strSZFWS.substr(0, strSZFWS.length - 1);
                $scope.Report.SZFW = "全省共有" + strSZFWS + "等" + SZFWS + "个市、" + SZFWX + "个县（区）受灾。";
            } else {
                $scope.Report.SZFW = "全省";
            }
            break;
        case 3:
            var strSZFWX = "";
            var SZFWX = 0;
            angular.forEach(hl011, function (obj) {
                if (obj.DW != "合计" && Number(obj.SZFWX) > 0) {
                    SZFWX++;
                    strSZFWX += obj.DW + "、";
                }
            });
            if (SZFWX > 0) {
                strSZFWX = strSZFWX.substr(0, strSZFWX.length - 1);
                $scope.Report.SZFW = "全市共有" + strSZFWX + "等" + SZFWX + "个县（区）受灾。";
            } else {
                $scope.Report.SZFW = "全市";
            }
            break;
        case 4:
            $scope.Report.SZFW = "全县（区）";
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

            if (typeof $scope[arr[0]][arr[1]] == "string" && $scope[arr[0]][arr[1]] > len) {
                $scope[arr[0]][arr[1]] = $scope[arr[0]][arr[1]].substr(0, len - 1);
            }
        }
    };

    window.$scope = $scope;
});