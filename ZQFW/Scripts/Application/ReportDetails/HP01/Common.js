window.App = angular.module('CommonReport', []).controller("MainCtrl", function($scope) {
    $scope.$opener = window.opener.$scope;
    $scope.Fn = {
        NumName: function(num) {
            switch (num) {
            case 0:
                return "一";
            case 1:
                return "二";
            case 2:
                return "三";
            case 3:
                return "四";
            }
        },
        Number: function(num, len) {
            if (Number(num) > 0) {
                if (len != undefined) {
                    return parseFloat(num).toFixed(len);
                } else {
                    return parseFloat(num);
                }
            } else {
                return 0;
            }
        },
        Export: function() {
            saveChart(); //上传相关图表至服务器
            window.open("/BaseData/ExportCommonReport?Report=" + angular.toJson($scope.Report), "", "", "");
        }
    };

    $scope.Report = App.CommonReportModel;
    var hp01 = angular.copy($scope.$opener.Open.Report.Current.HP011);
    var endTimeArr = $scope.$opener.Open.Report.Current.ReportTitle.EndDateTime.split("-");
    $scope.Report.Time = App.Tools.Date.GetOneDay(0, endTimeArr[0] + "年" + endTimeArr[1] + "月" + endTimeArr[2] + "日");
    $scope.Report.Date = endTimeArr[1] + "月" + endTimeArr[2] + "日";
    endTimeArr[0] = Number(endTimeArr[0]);
    endTimeArr[1] = Number(endTimeArr[1]);
    endTimeArr[2] = Number(endTimeArr[2]);
    $.extend($scope.Report, {
        UnitName: $scope.$opener.BaseData.Unit.Local.UnitName,
        Month: endTimeArr[1],
        TJ_MONTH: endTimeArr[1],
        TJ_DAY: endTimeArr[2],
        XSCSZJ: $scope.Fn.Number(hp01[0].XSCSZJ, 2),
        XXSLZJ: $scope.Fn.Number(hp01[0].XXSLZJ, 2),
        XZYBFB: $scope.Fn.Number(hp01[0].XZYBFB, 0),
        XZKYSL: $scope.Fn.Number(hp01[0].XZKYSL, 2)
    });

    switch (Number($scope.$opener.BaseData.Unit.Local.Limit)) {
    case 2:
        $scope.Report.SimpleUnitName = "省";
        $scope.Report.OtherUnitName = "市州";
        $scope.Report.XSLMeasureName = "亿立方米";
        break;
    case 3:
        $scope.Report.SimpleUnitName = "市";
        $scope.Report.OtherUnitName = "区县";
        $scope.Report.XSLMeasureName = "万立方米";
        break;
    case 4:
        $scope.Report.SimpleUnitName = "县";
        $scope.Report.OtherUnitName = "乡镇";
        $scope.Report.XSLMeasureName = "万立方米";
        break;
    }

    var tmp = window.user_data.xsl;
    $scope.Report.SQ_MONTH = tmp.SQ_MONTH;
    $scope.Report.SQ_DAY = tmp.SQ_DAY;
    $scope.Report.BSQXS = Number($scope.Report.XXSLZJ) - Number(tmp.SQXSL); //这期蓄水量减去上期蓄水量

    if ($scope.Report.BSQXS >= 0) {
        $scope.Report.SQ_DHS = "多";
    } else {
        $scope.Report.SQ_DHS = "少";
        $scope.Report.BSQXS = Math.abs($scope.Report.BSQXS);
    }
    $scope.Report.BSQXS = $scope.Fn.Number($scope.Report.BSQXS, 2);

    tmp = $scope.$opener.BaseData.Reservoir.LNTQXSL[$scope.$opener.BaseData.Unit.Local.UnitCode]; //2011年起历年同期蓄水均值
    $scope.Report.BTQPJXS = App.Tools.Calculator.Subtraction($scope.Report.XXSLZJ, tmp);

    /*$.each(tmp.ALLTQXSL.BubbleSort("XSL"), function(i) {
                                if (Number(this.Year) == endTimeArr[0]) {
                                    $scope.Report.KYSLDJW = i + 1;
                                } else {
                                    $scope.Report.BTQPJXS += this.XSL;
                                }
                            });*/
    //$scope.Report.KYSLDJW = $scope.Report.KYSLDJW != undefined ? $scope.Report.KYSLDJW : (tmp.ALLTQXSL.length + 1);  //可用水量第几位

    /*if(tmp.ALLTQXSL.length > 1){
                                $scope.Report.BTQPJXS = $scope.Report.BTQPJXS / (tmp.ALLTQXSL.length - 1);  //历年同期平均蓄水量(不包括今年的)
                            }*/

    //$scope.Report.BTQPJXS = Number($scope.Report.XXSLZJ) - Number($scope.Report.BTQPJXS.toFixed(2));
    //$scope.Report.BTQPJXS = $scope.Report.BTQPJXS.toFixed(2);

    if ($scope.Report.BTQPJXS >= 0) {
        $scope.Report.TQ_DHS = "多";
    } else {
        $scope.Report.TQ_DHS = "少";
        $scope.Report.BTQPJXS = Math.abs($scope.Report.BTQPJXS);
    }
    $scope.Report.BTQPJXS_Percent = Math.abs(($scope.Report.XXSLZJ - tmp) / tmp) * 100;
    $scope.Report.BTQPJXS_Percent = $scope.Fn.Number($scope.Report.BTQPJXS_Percent, 2);
    $scope.Report.BTQPJXS = $scope.Fn.Number($scope.Report.BTQPJXS, 2);

    $scope.Report.DZKXXSL = hp01[0].DZKXXSL;
    $scope.Report.DZKXXSL_Percent = $scope.Fn.Number(($scope.Report.DZKXXSL / $scope.Report.XXSLZJ) * 100, 2);
    $scope.Report.ZZKXXSL = hp01[0].ZZKXXSL;
    $scope.Report.ZZKXXSL_Percent = $scope.Fn.Number(($scope.Report.ZZKXXSL / $scope.Report.XXSLZJ) * 100, 2);
    $scope.Report.XZKXXSL = App.Tools.Calculator.Addition(hp01[0].XYKXXS, hp01[0].XRKXXS);
    $scope.Report.XZKXXSL_Percent = $scope.Fn.Number(($scope.Report.XZKXXSL / $scope.Report.XXSLZJ) * 100, 2);
    $scope.Report.SPTXXS = hp01[0].SPTXXS;
    $scope.Report.SPTXXS_Percent = $scope.Fn.Number(($scope.Report.SPTXXS / $scope.Report.XXSLZJ) * 100, 2);

    hp01.splice(0, 1);
    hp01.BubbleSort("XXSLZJ");
    /*hp01 = $.grep(hp01, function(obj) {
                                return obj.XXSLZJ > 0;  //过滤蓄水量为0的单位
                            });*/
    tmp = undefined;
    $.each(hp01, function(i) {
        if (i < 4) {
            $scope.Report.MaxXSLCities += this.DW + "、";
            $scope.Report.MaxXSLCitiesXSBFB += $scope.Fn.Number(this.XZYBFB, 0) + "%、";
            tmp = i;
        } else {
            return false;
        }
    });

    if (tmp != undefined) {
        $scope.Report.MaxXSLCities = $scope.Report.MaxXSLCities.substr(0, $scope.Report.MaxXSLCities.length - 1) + $scope.Fn.NumName(tmp) + "个" + $scope.Report.OtherUnitName;
        $scope.Report.MaxXSLCitiesXSBFB = $scope.Report.MaxXSLCitiesXSBFB.substr(0, $scope.Report.MaxXSLCitiesXSBFB.length - 1);
    }

    hp01.splice(0, tmp + 1); //去掉最大的4个
    if (hp01.length > 4) {
        for (var i = hp01.length - 4; i < hp01.length; i++) { //读取最小的4个
            $scope.Report.MinXSLCities += hp01[i].DW + "、";
            $scope.Report.MinXSLCitiesXSBFB += $scope.Fn.Number(hp01[i].XZYBFB, 0) + "%、";
        }
        $scope.Report.MinXSLCities = $scope.Report.MinXSLCities.substr(0, $scope.Report.MinXSLCities.length - 1) + "四个" + $scope.Report.OtherUnitName;
        $scope.Report.MinXSLCitiesXSBFB = $scope.Report.MinXSLCitiesXSBFB.substr(0, $scope.Report.MinXSLCitiesXSBFB.length - 1);
        hp01.splice(hp01.length - 4, 4); //删除最后面4个

        $scope.Report.QTCSXSBFBMAX = $scope.Fn.Number(hp01[0].XZYBFB, 0);
        if (hp01.length > 1) { //只剩一个，用作QTCSXSBFBMAX
            $scope.Report.QTCSXSBFBMIN = $scope.Fn.Number(hp01[hp01.length - 1].XZYBFB, 0);
        }
    } else if (hp01.length > 0) {
        for (tmp = 0; i < hp01.length; tmp++) { //小于或刚好4个，全部读取
            $scope.Report.MinXSLCities += hp01[tmp].DW + "、";
            $scope.Report.MinXSLCitiesXSBFB += $scope.Fn.Number(hp01[tmp].XZYBFB, 0) + "%、";
        }
        $scope.Report.MinXSLCities = $scope.Report.MinXSLCities.substr(0, $scope.Report.MinXSLCities.length - 1) + $scope.Fn.NumName(tmp - 1) + "个" + $scope.Report.OtherUnitName;
        $scope.Report.MinXSLCitiesXSBFB = $scope.Report.MinXSLCitiesXSBFB.substr(0, $scope.Report.MinXSLCitiesXSBFB.length - 1);
    }

    $scope.$watch('Report.TJ_DAY', function(to) {
        if (!isNaN(to)) {
            to = Number(to);
            if (to < 11) {
                $scope.Report.DayName = "初";
            } else if (to > 10 && to < 21) {
                $scope.Report.DayName = "中旬";
            } else {
                $scope.Report.DayName = "底";
            }
        }
    });

    window.$scope = $scope;
});