window.App = angular.module("Index",[]);

App.controller('MainCtrl', function ($scope, $http) {
    $scope.BaseData = window.initdata;

    $scope.Benefit = {
        Report: {
            Current: App.Models.Benefit(),
            Filter: {
                Condition: {
                    StartDateTime: App.Tools.Date.GetOneDay(-7),
                    EndDateTime: App.Tools.Date.GetToday()
                },
                Search: function() {
                    Alert("该功能正在开发...");
                }
            }
        },
        Fn: {
            Save: function() {
                $http.post('/sh/SaveBenefitReport?report=' + angular.toJson($scope.Benefit.Report.Current)).then(function (response) {
                    if (Number(response.data) > 0) {
                        var date = new Date($scope.Benefit.Report.Current.BDate);
                        if ($scope.Benefit.Report.Current.TBNO == 0) {
                            $scope.BaseData.Reports.push({
                                TBNO: response.data,
                                BArea: $scope.Benefit.Report.Current.BArea,
                                BDate: date.getFullYear().toString().slice(2) + "年" + (date.getMonth() + 1) + "月" + date.getDate() + "日"
                            });
                        } else {
                            var rpt = $scope.BaseData.Reports.Find("TBNO", $scope.Benefit.Report.Current.TBNO);
                            rpt.BArea = $scope.Benefit.Report.Current.BArea;
                            rpt.BDate = date.getFullYear().toString().slice(2) + "年" + (date.getMonth() + 1) + "月" + date.getDate() + "日";
                        }
                        $scope.Benefit.Report.Current.TBNO = response.data;

                        Alert("保存成功！");
                    } else {
                        Alert(response.data);
                        throw response.data;
                    }
                });
            },
            Open: function(tbno) {
                $http.get('/sh/openbenefitreport?tbno=' + tbno).then(function(response) {
                    if (typeof response.data == "object") {
                        delete response.data.EntityKey;
                        delete response.data.EntityState;

                        $scope.Benefit.Report.Current = response.data;
                        var date = new Date(Number($scope.Benefit.Report.Current.BDate.replace("/Date(", "").replace(")/", "")));
                        $scope.Benefit.Report.Current.BDate = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
                    } else {
                        Alert("打开报表出错！");
                        throw response.data;
                    }
                });
            },
            Delete: function () {
                if (confirm("确定要删除？")) {
                    $http.post('/sh/DeleteBenefitReport?tbno=' + $scope.Benefit.Report.Current.TBNO).then(function (response) {
                        if (Number(response.data) > 0) {
                            $scope.BaseData.Reports = $scope.BaseData.Reports.RemoveBy("TBNO", $scope.Benefit.Report.Current.TBNO);
                            $scope.Benefit.Fn.Clear();
                            Alert("删除成功！");
                        } else {
                            Alert(response.data);
                            throw response.data;
                        }
                    });
                }
            },
            Clear: function() {
                $scope.Benefit.Report.Current = App.Models.Benefit();
            }
        }
    };

    window.$scope = $scope;
});

App.directive('ngWdatepicker', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function ($scope, $element, $attrs, ngModelCtrl) {
            $(function () {
                $element.click(function () {
                    var setting = $(this).attr("ng-wdatepicker").trim().length > 0 ? JSON.parse($(this).attr("ng-wdatepicker")) : {};
                    if (setting.minDate == "cur_year") {
                        setting.minDate = new Date().getFullYear() + "-01-01";
                    } else {
                        setting.minDate = setting.minDate ? $parse(setting.minDate)($scope) : "1900-01-01";
                    }
                    WdatePicker({
                        dateFmt: setting.dateFmt ? setting.dateFmt : "yyyy-MM-dd",
                        minDate: setting.minDate,
                        maxDate: setting.maxDate ? $parse(setting.maxDate)($scope) : "2091-06-22",
                        onpicked: function () {
                            $element.scope().$apply(function () {
                                var M = $dp.cal.date.M, d = $dp.cal.date.d, H, m, s;
                                switch (setting.dateFmt) {
                                    case "yyyy-MM-dd HH":
                                        H = $dp.cal.date.H;
                                        ngModelCtrl.$setViewValue($dp.cal.date.y + "-" + (M < 10 ? "0" + M : M) + "-" + (d < 10 ? "0" + d : d) + " " + (H < 10 ? "0" + H : H));
                                        break;
                                    case "yyyy":
                                        ngModelCtrl.$setViewValue($dp.cal.date.y);
                                        break;
                                    case "MM":
                                        ngModelCtrl.$setViewValue(M < 10 ? "0" + M : M);
                                        break;
                                    case "dd":
                                        ngModelCtrl.$setViewValue(d < 10 ? "0" + d : d);
                                        break;
                                    case "yyyy-MM-dd HH:mm:ss":
                                        H = $dp.cal.date.H;
                                        m = $dp.cal.date.m;
                                        s = $dp.cal.date.s;
                                        ngModelCtrl.$setViewValue($dp.cal.date.y + "-" + (M < 10 ? "0" + M : M) + "-" + (d < 10 ? "0" + d : d) + " " + (H < 10 ? "0" + H : H) + ":" + (m < 10 ? "0" + m : m) + ":" + (s < 10 ? "0" + s : s));
                                        break;
                                    default:
                                        ngModelCtrl.$setViewValue($dp.cal.date.y + "-" + (M < 10 ? "0" + M : M) + "-" + (d < 10 ? "0" + d : d));
                                        break;
                                }
                                if (setting.callback) {
                                    $parse(setting.callback)($scope)();
                                }
                            });
                        }
                    });
                });
            });
        }
    };
}]);

App.Models = App.Models || {};
App.Models.Benefit = function () {
    return {
        TBNO: 0,
        UnitCode: $.cookie("unitcode"),
        BArea: $.cookie("unitname"), /*报告地区*/
        BDate: App.Tools.Date.GetToday(), /*报告时间*/
        DQJJ: undefined, /*地区简介*/
        ZQYQ: undefined, /*灾情简介-雨情*/
        ZQSQ: undefined, /*灾情简介-水情*/
        ZQZQ: undefined, /*灾情简介-灾情*/
        YJJC: undefined, /*预警情况-监测情况*/
        YJYQ: undefined, /*预警情况-雨前预警*/
        YJYZ: undefined, /*预警情况-雨中预警*/
        YJXY: undefined, /*应急响应*/
        XYXJ: undefined, /*响应细节*/
        ZHZJ: undefined  /*灾后总结*/
    };
};