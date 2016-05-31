window.App = angular.module('DisasterReview', []).controller("MainCtrl", function ($scope) {
    $scope.Field = window.opener.$scope.BaseData.Field;
    $scope.Unit = window.opener.$scope.BaseData.Unit.Local;
    var hl011 = window.opener.$scope.Open.Report.Current.HL011;
    var hl013 = window.opener.$scope.Open.Report.Current.HL013;
    var hl014 = window.opener.$scope.Open.Report.Current.HL014;
    $scope.Report = {};
    var fieldArr = ["CQZJJJSS", "JZWSYDX", "JZWSYFW", "SMXJT", "SMXGQ", "SMXGD", "SMXGS", "ZYZJZDSS", "GCJJZYRK", "GCHSWKRK", "GCYMLS", "YMFWBL", "YMFWMJ", "SYCS"];
    angular.forEach(App.DisasterReviewModel, function (val, key) {
        if (fieldArr.In_Array(key) && Number(hl013[0][key]) > 0) {
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
    var arr = $scope.Report.StartDateTime.split("-");
    $scope.Report.ZQKSRQ = arr[0] + "年" + arr[1] + "月" + arr[2] + "日";
    arr = $scope.Report.EndDateTime.split("-");
    $scope.Report.ZQJZRQ = arr[1] + "月" + arr[2] + "日";
    $scope.Report.StartDateTime = $scope.Report.ZQKSRQ;
    $scope.Report.EndDateTime = arr[0] + "年" + arr[1] + "月" + arr[2] + "日";

    $scope.Report.Limit = $scope.Unit.Limit;
    $scope.Report.UnitName = $scope.Unit.UnitName;
    fieldArr = ["QXHJ", "QXBDGB", "QXDFRY", "QXFXJD", "ZJXJ", "ZJZY", "ZJSJ", "ZJSJYS", "ZJQZ"];
    angular.forEach(fieldArr, function (field) {
        $scope.Report[field] = hl014[0][field];
    });

    $scope.Report.SZFWDSS = "";
    if (parseInt(window.opener.$.cookie("limit")) == 2) {
        var units = "";
        var count = 0;
        $.each(hl011, function (i) {
            if (i == 0) {
                return true;
            }

            if (Number(this.SZFWX) > 0) {
                units += this.DW + "、";
                count++;
            }
        });
        units = units.slice(0, units.length - 2);
        $scope.Report.SZFWDSS = units + count + "个盟市、";
        $("input#SZFWDSS").width((($scope.Report.SZFWDSS.length * 16) + 5) + 'px'); //一个汉字占16px左右
    }
    if ($scope.Report.SZFWDSS.length == 0) {
        $("input#SZFWDSS").hide();
    }

    if (parseInt(window.opener.$.cookie("limit")) < 4) {
        $scope.Report.SZFWX += "个旗县（市、区）、";
    } else {
        $scope.Report.SZFWX = "";
    }

    if ($scope.Report.SZFWX.length > 0) {
        $("input#SZFWX").width((($scope.Report.SZFWX.length * 16) + 5) + 'px'); //一个汉字占16px左右
    } else {
        $("input#SZFWX").hide();
    }

    fieldArr = ["QXHJ", "QXBDGB", "QXDFRY", "QXFXJD", "ZJXJ", "ZJZY", "ZJSJ", "ZJSJYS", "ZJQZ"];
    angular.forEach(fieldArr, function (field) {
        $scope.Report[field] = Number($scope.Report[field]) > 0 ? $scope.Report[field] : 0;
    });


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