App.Models = App.Models || {};
App.Models.NP = App.Models.NP || {
    NP01: {}
};
App.Models.NP.NP01.NP011 = {};

App.Models.NP.NP01.Report = function() {
    return {        
        ReportTitle: App.Models.ReportTitle(),
        Affix: [],
        NP011: App.Models.NP.NP01.NP011.Array(),
        Attr: {
            DelAffixTBNO: [],
            DelAffixURL: [],
            TableIndex: 0,
            SSB: false,
            HB: false,
            ReportState: "Created",
            ReportFooter: "ReportTitle",
            CheckErrors: {}
        }
    };
};

App.Models.NP.NP01.NP011.Array = function(reservoir, arr) {
    var isLoad = reservoir ? true : false;
    reservoir = reservoir ? reservoir : $scope.BaseData.Reservoir;
    var model = [];
    var obj = undefined;

    $.each(reservoir, function(i) {
        if (this.Unders.length == 0) {
            return true;
        }

        obj = App.Models.NP.NP01.NP011.Object();
        obj.RSName = this.City;
        obj.UnitName = this.Unders.length;
        if (obj.UnitName > 0) {
            obj.UnitCode = this.Unders[0].UnitCode;
            switch ($scope.BaseData.Unit.Local.Limit) {
                case 2:
                    obj.UnitCode = obj.UnitCode.substr(0, 4) + "0000";
                    break;
                case 3:
                    obj.UnitCode = obj.UnitCode.substr(0, 6) + "00";
                    break;
            }
        }
        obj.No = App.Tools.Convert.ToChineseNumber(isLoad ? $scope.BaseData.Reservoir.length : (i + 1));
        model.push(obj);

        $.each(this.Unders, function(j) {
            obj = App.Models.NP.NP01.NP011.Object();
            obj = $.extend(obj, this);
            obj.No = j + 1;
            if (arr && arr.length > 0) {
                obj = $.extend(arr.Find("RSCode", obj.RSCode, false, true), obj);
            }

            model.push(obj);
        });
    });

    return model;
};

App.Models.NP.NP01.NP011.Object = function() {
    return {
        TBNO: undefined,
        RSName: undefined,
        RSCode: undefined,
        UnitName: undefined,
        ZKR: undefined,
        SKR: undefined,
        SSW: undefined,
        XXSW: undefined,
        ZCXSW: undefined,
        /*ZCXSWXYKR: undefined,*/
        DQSW: undefined,
        DQXSL: undefined,
        SFCXXSW: undefined,
        PageNO: undefined
    };
};