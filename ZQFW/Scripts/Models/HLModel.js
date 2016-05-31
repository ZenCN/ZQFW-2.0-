App.Models = App.Models || {};
App.Models.HL = App.Models.HL || {
    HL01: {}
};
App.Models.HL.HL01.HL011 = {};
App.Models.HL.HL01.HL014 = {};

App.Models.HL.HL01.Report = function(needDistinctValue) {

    return {
        ReportTitle: App.Models.ReportTitle(),
        HL011: App.Models.HL.HL01.HL011.Array(false, needDistinctValue),
        HL012: [],
        HL013: [$.extend(App.Models.HL.HL01.HL013(), { DW: "合计" })],
        HL014: App.Models.HL.HL01.HL014.Array(),
        Affix: [],
        Attr: {
            DelAffixTBNO: [],
            DelAffixURL: [],
            TableIndex: 0,
            SSB: false,
            HB: false,
            ReportState:"Created",
            ReportFooter: "ReportTitle",
            CheckErrors: {},
            AggAcc: {
                Content: [],
                Selected: undefined
            }
        }
    };
};

App.Models.HL.HL01.HL011.Array = function ($scope,needDistinctValue) {
    $scope = $scope ? $scope : window.$scope;
    var hl011 = [];
    var baseData = $scope.BaseData;
    var units = [];
    needDistinctValue = false;  //累计剔除重复值，暂时不做
    if (needDistinctValue == undefined) {
        if ($scope.BaseData.Unit.Local.Limit == 2 && $scope.Open.Report.Current.ReportTitle.SourceType == "2") {
            needDistinctValue = true;
        } else {
            needDistinctValue = false;
        }
    }
    units.push(angular.copy(baseData.Unit.Local));
    if (baseData.Unit.Unders.length > 0) {
        units[0].UnitName = '合计';
    }
    units = units.concat(baseData.Unit.Unders).RemoveBy("UnitName","全区/县");

    for (var i = 0; i < units.length; i++) {
        hl011.push(App.Models.HL.HL01.HL011.Object(units[i].UnitCode, units[i].UnitName,needDistinctValue));
    }

    return hl011;
};

App.Models.HL.HL01.HL011.Object = function(unitcode,unitname,distinctOrScope, options) {
    options = options ? options : {};
    var obj = $.extend({
        UnitCode: unitcode,
        RiverCode: undefined,
        DataOrder: undefined,
        DistributeRate: undefined,
        DW: unitname
    }, App.Config.Field.Fn.GetModel("HL01." + window.$scope.SysUserCode + ".HL011"), options);

    if (typeof distinctOrScope == "boolean" && distinctOrScope) {
        obj.SZFWX = {
            Fake: undefined,
            Real: undefined,
            Details: []
        };
    }

    return obj;
};

App.Models.HL.HL01.HL012 = function($scope) {
    $scope = $scope ? $scope : window.$scope;
    var riverSelect = angular.copy($scope.BaseData.Select.River);

    return $.extend(App.Config.Field.Fn.GetModel("HL01." + window.$scope.SysUserCode + ".HL012"), {
        Checked: false, //自定义属性
        UnitCode: $.cookie("unitcode"),
        RiverSelect: riverSelect,
        RiverCode: riverSelect.length ? riverSelect[0].Code : undefined,
        DW: "—————",
        SWHJ: $.cookie("fullname"),
        SWSJ: App.Tools.Date.GetToday(),
        SWXM: "未知",
        SWXB: "未知",
        SWNL: undefined,
        SWDD: "未知",
        DataType: "死亡",
        DeathReason: "降雨-其它",
        DeathReasonCode: "JY10",
        SourcePageNo: undefined,
        DistributeRate: undefined
    });
};

App.Models.HL.HL01.HL013 = function ($scope) {
    $scope = $scope ? $scope : window.$scope;
    var riverSelect = angular.copy($scope.BaseData.Select.River);

    return $.extend(App.Config.Field.Fn.GetModel("HL01." + window.$scope.SysUserCode + ".HL013"), {
        Checked: false,  //自定义属性
        UnitCode: $.cookie("unitcode"),
        RiverSelect: riverSelect,
        RiverCode: riverSelect.length ? riverSelect[0].Code : undefined,
        DW: "—————",
        SourcePageNo: undefined,
        DistributeRate: undefined,
        GCJSSJ: App.Tools.Date.GetToday("yyyy-M-dd HH")
    });
};

App.Models.HL.HL01.HL014.Array = function ($scope) {
    $scope = $scope ? $scope : window.$scope;
    var hl014 = [];
    var baseData = $scope.BaseData;
    var units = [];
    units.push(angular.copy(baseData.Unit.Local));
    if (baseData.Unit.Unders.length > 0) {
        units[0].UnitName = '合计';
    }
    units = units.concat(baseData.Unit.Unders).RemoveBy("UnitName","全区/县");

    for (var i = 0; i < units.length; i++) {
        hl014.push(App.Models.HL.HL01.HL014.Object(units[i].UnitCode, units[i].UnitName));
    }

    return hl014;
};

App.Models.HL.HL01.HL014.Object = function(unitcode, unitname, distinctOrScope, options) {
    options = options ? options : {};
    return $.extend({
        UnitCode: unitcode,
        DW: unitname,
        DistributeRate: undefined,
    }, App.Config.Field.Fn.GetModel("HL01." + window.$scope.SysUserCode + ".HL014"), options);
};