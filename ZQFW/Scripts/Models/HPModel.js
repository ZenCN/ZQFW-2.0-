App.Models = App.Models || {};
App.Models.HP = App.Models.HP || {
    HP01: {}
};
App.Models.HP.HP01.HP011 = {};
App.Models.HP.HP01.HP012 = {
    Fake: {},
    Real: {},
    Large: {},
    Middle: {}
};

App.Models.HP.HP01.Report = function () {
    var report = {
        ReportTitle: App.Models.ReportTitle(),
        HP011: App.Models.HP.HP01.HP011.Array(0,true),
        HP012: {
            Fake: {
                Large: undefined,
                Middle: undefined
            },
            Real: {
                Large: App.Models.HP.HP01.HP012.Real.Large(),
                Middle: App.Models.HP.HP01.HP012.Real.Middle()
            }
        },
        Affix: [],
        Attr: {
            DelAffixTBNO: [],
            DelAffixURL: [],
            TableIndex: 0,
            ReportState: "Cteated",
            ReportFooter: "ReportTitle",
            AggAcc: {
                Content: [],
                Selected: undefined
            }
        }
    };

    report.HP012.Fake.Large = App.Models.HP.HP01.HP012.Fake.Large(report.HP012.Real.Large);
    report.HP012.Fake.Middle = App.Models.HP.HP01.HP012.Fake.Middle(report.HP012.Real.Middle);

    return report;
};

App.Models.HP.HP01.HP011.Array = function ($scope, sumConstant) {
    $scope = $scope ? $scope : window.$scope;
    var hp011 = [];
    var baseData = $scope.BaseData;
    var units = [];
    var obj = undefined;
    var ctf = $scope.Fn.ConvertToFloat;
    var result = undefined;
    units.push(angular.copy(baseData.Unit.Local));
    if (baseData.Unit.Unders.length > 0) {
        units[0].UnitName = '合计';
    }
    units = units.concat(baseData.Unit.Unders);

    for (var i = 0; i < units.length; i++) {
        obj = App.Models.HP.HP01.HP011.Object(units[i].UnitCode, units[i].UnitName, $scope);
        hp011.push(obj);
        if (sumConstant && i > 0) {
            result = (ctf(obj.DZXKCS) + ctf(obj.ZZXKCS) + ctf(obj.XYSKCS) + ctf(obj.XRSKCS)) / 10000 + ctf(obj.SPTHJCS);
            result = result == 0 ? undefined : result.toFixed(2);
            obj.XSCSZJ = result;
            hp011[0].XSCSZJ = App.Tools.Calculator.Addition(hp011[0].XSCSZJ, result);
            result = ctf(obj.DZKYXSL) + ctf(obj.ZZKYXSL) + ctf(obj.XYKYXS) + ctf(obj.XRKYXS) + ctf(obj.SPTYXS);
            result = result == 0 ? undefined : result.toFixed(2);
            obj.YXSLZJ = result;
            hp011[0].YXSLZJ = App.Tools.Calculator.Addition(hp011[0].YXSLZJ, result);
        }
    }

    angular.forEach(["DZXKCS", "DZKYXSL", "ZZXKCS", "ZZKYXSL", "XYSKCS", "XYKYXS", "XRSKCS", "XRKYXS", "SPTHJCS", "SPTYXS"], function(field) {
        angular.forEach(hp011, function(obj) {
            hp011[0][field] = App.Tools.Calculator.Addition(obj[field], hp011[0][field]);
        });
    });

    return hp011;
};

App.Models.HP.HP01.HP011.Object = function (unitcode, unitname, $scope) {
    $scope = $scope ? $scope : window.$scope;
    var obj = {};
    if ($scope.BaseData.Reservoir.Units.length > 0) {
        obj = $scope.BaseData.Reservoir.Units.Find("UnitCode", unitcode);
    }
    var model = angular.extend({
        DW: unitname,
        UnitCode: unitcode,
        XSCSZJ: undefined,
        YXSLZJ: undefined,
        XXSLZJ: undefined,
        XZKYSL: undefined,
        XZYBFB: undefined,
        SQZJ: undefined,
        LNZJ: undefined,
        DZXKCS: undefined,
        DZKYXSL: undefined,
        DZKXXSL: undefined,
        DXKKYSL: undefined,
        KXZYBFB: undefined,
        ZZXKCS: undefined,
        ZZKYXSL: undefined,
        ZZKXXSL: undefined,
        ZXKKYSL: undefined,
        ZXZYBFB: undefined,
        XYSKCS: undefined,
        XYKYXS: undefined,
        XYKXXS: undefined,
        XYKKYS: undefined,
        XKXZYBFB: undefined,
        XRSKCS: undefined,
        XRKYXS: undefined,
        XRKXXS: undefined,
        XRKKYS: undefined,
        XRXZYBFB: undefined,
        SPTHJCS: undefined,
        SPTYXS: undefined,
        SPTXXS: undefined,
        SPTKYS: undefined,
        TXZYBFB: undefined
    }, obj);

    return model;
};

App.Models.HP.HP01.HP012.Object = function (rsType, obj, $scope) {
    $scope = $scope ? $scope : window.$scope;

    obj = angular.extend({
        DXSKMC: undefined,
        DXKYXSL: undefined,
        DXKXXSL: undefined,
        DXKKYS: undefined,
        DXZYBFB: undefined,
        QNTQDXS: undefined,
        DistributeRate: rsType
    }, obj);

    if ($scope.BaseData.Reservoir.QNTQDXS) {  //QNTQDXS存在即为操作，否则是查看下级表
        obj.QNTQDXS = $scope.BaseData.Reservoir.QNTQDXS[obj.UnitCode];
        if (Number(obj.QNTQDXS) <= 0) { //null undefined ""
            obj.QNTQDXS = undefined;
        }
    }

    return obj;
};

App.Models.HP.HP01.HP012.Real.Large = function ($scope) {
    $scope = $scope ? $scope : window.$scope;
    var baseData = $scope.BaseData;
    var large = [];
    if (baseData.Reservoir.Large.length > 0) {
        var reservoir = [{ DXSKMC: "合计", UnitCode: "A" }];
        reservoir = reservoir.concat(baseData.Reservoir.Large);
        angular.forEach(reservoir, function (obj) {
            large.push(App.Models.HP.HP01.HP012.Object(1, obj, $scope));
        });
    }

    angular.forEach(large, function(obj) {
        large[0].DXKYXSL = App.Tools.Calculator.Addition(obj.DXKYXSL, large[0].DXKYXSL);
    });
    
    return large;
};

App.Models.HP.HP01.HP012.Fake.Large = function (reservoir, length) {
    length = length == undefined ? $(document).scope().BaseData.Unit.Unders.length : length;
    length = length + 1;
    var large = [[]];
    var j = 0;
    angular.forEach(reservoir, function (obj,i) {
        if (i % length != 0 || i == 0) {
            large[j].push(obj);
        } else {
            large.push([obj]);
            j++;
        }
    });
    if (large[j].length != length) {
        var count = length - large[j].length;
        for (var i = 0; i < count; i++) {
            large[j].push({ isEmpty: true });
        }
    }

    return large;
};

App.Models.HP.HP01.HP012.Real.Middle = function ($scope) {
    $scope = $scope ? $scope : window.$scope;
    var baseData = $scope.BaseData;
    var middle = [];
    if (baseData.Reservoir.Middle.length > 0) {
        var reservoir = [{ DXSKMC: "合计", UnitCode: "B" }];
        reservoir = reservoir.concat(baseData.Reservoir.Middle);
        angular.forEach(reservoir, function (obj) {
            middle.push(App.Models.HP.HP01.HP012.Object(2, obj, $scope));
        });
    }

    angular.forEach(middle, function (obj) {
        middle[0].DXKYXSL = App.Tools.Calculator.Addition(obj.DXKYXSL, middle[0].DXKYXSL);
    });
    
    return middle;
};

App.Models.HP.HP01.HP012.Fake.Middle = function(reservoir, length) {
    length = length == undefined ? $(document).scope().BaseData.Unit.Unders.length : length;
    length = length + 1;
    var middle = [[]];
    var j = 0;
    angular.forEach(reservoir, function(obj,i) {
        if (i % length != 0 || i == 0) {
            middle[j].push(obj);
        } else {
            middle.push([obj]);
            j++;
        }
    });
    if (middle[j].length != length) {
        var count = length - middle[j].length;
        for (var i = 0; i < count; i++) {
            middle[j].push({ isEmpty: true });
        }
    }

    return middle;
};