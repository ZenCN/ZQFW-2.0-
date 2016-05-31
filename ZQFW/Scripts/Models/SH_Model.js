App.Models = App.Models || {};
App.Models.SH = App.Models.SH || {};

App.Models.SH.SH011 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH011.Object(), this, {
            DataOrder: (i + 1),
            NDZJYS: $scope.BaseData.Plan[$scope.BaseData.Unit.Local.UnitCode + "-" + this.UnitCode + "-SH011-NDZJYS"] || 0
        }));
    });

    return models;
}

App.Models.SH.SH011.Object = function() {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        JSNR: undefined,
        NDZJYS: undefined,
        NDZJZY: undefined,
        NDZJDF: undefined,
        DFZJSJ: undefined,
        DFZJSX: undefined,
        ZJZF: undefined,
        //以下为新增字段
        DFZJX: undefined,
        JDWCZY: undefined,
        JDZYBL: undefined,
        JDWCSJ: undefined,
        JDSJBL: undefined,
        JDWCSHIJ: undefined,
        JDSHIJBL: undefined,
        JDWCXJ: undefined,
        JDXJBL: undefined,
        ZFWCZY: undefined,
        ZFZYBL: undefined,
        ZFWCSJ: undefined,
        ZFSJBL: undefined,
        ZFWCSHIJ: undefined,
        ZFSHIJBL: undefined,
        ZFWCXJ: undefined,
        ZFXJBL: undefined
    }
};

App.Models.SH.SH021 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH021.Object(), this, {
            DataOrder: (i + 1),
            JHDF: $scope.BaseData.Plan[this.UnitCode + '-SH021-JHDF'] || 0,
            JHHA: $scope.BaseData.Plan[this.UnitCode + '-SH021-JHHA'] || 0,
            JHQY: $scope.BaseData.Plan[this.UnitCode + '-SH021-JHQY'] || 0,
            JHJHG: $scope.BaseData.Plan[this.UnitCode + '-SH021-JHJHG'] || 0,
            JHQT: $scope.BaseData.Plan[this.UnitCode + '-SH021-JHQT'] || 0,
            JHTF: $scope.BaseData.Plan[this.UnitCode + '-SH021-JHTF'] || 0,
            JHSF: $scope.BaseData.Plan[this.UnitCode + '-SH021-JHSF'] || 0,
            JHHNT: $scope.BaseData.Plan[this.UnitCode + '-SH021-JHHNT'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH021.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        GDMC: undefined,
        SZX: undefined,
        CBSJBG: undefined,
        SCTG: undefined,
        ZBQK: undefined,
        SFKG: undefined,
        JHDF: undefined,
        JHHA: undefined,
        JHQY: undefined,
        JHJHG: undefined,
        JHQT: undefined,
        JHTF: undefined,
        JHSF: undefined,
        JHHNT: undefined,
        JD: undefined,
        JGYS: undefined
    }
};

App.Models.SH.SH031 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH031.Object(), this, {
            DataOrder: (i + 1),
            YLZJH: $scope.BaseData.Plan[this.UnitCode + '-SH031-YLZJH'] || 0,
            SWZJH: $scope.BaseData.Plan[this.UnitCode + '-SH031-SWZJH'] || 0,
            WXTDJH: $scope.BaseData.Plan[this.UnitCode + '-SH031-WXTDJH'] || 0,
            WXJCJH: $scope.BaseData.Plan[this.UnitCode + '-SH031-WXJCJH'] || 0,
            TXSPJCZJH: $scope.BaseData.Plan[this.UnitCode + '-SH031-TXSPJCZJH'] || 0,
            TXSPJCZCJH: $scope.BaseData.Plan[this.UnitCode + '-SH031-TXSPJCZCJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH031.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        SSX: undefined,
        YLZJH: undefined,
        YLZWC: undefined,
        SWZJH: undefined,
        SWZWC: undefined,
        WXTDJH: undefined,
        WXTHWC: undefined,
        WXJCJH: undefined,
        WXJSWC: undefined,
        TXSPJCZJH: undefined,
        TXSPJCZWC: undefined,
        TXSPJCZCJH: undefined,
        TXSPJCZCWC: undefined
    }
};

App.Models.SH.SH032 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH032.Object(), this, {
            DataOrder: (i + 1),
            WXGBJH: $scope.BaseData.Plan[this.UnitCode + '-SH032-WXGBJH'] || 0,
            YLBJJH: $scope.BaseData.Plan[this.UnitCode + '-SH032-YLBJJH'] || 0,
            SWZJH: $scope.BaseData.Plan[this.UnitCode + '-SH032-SWZJH'] || 0,
            BJQJH: $scope.BaseData.Plan[this.UnitCode + '-SH032-BJQJH'] || 0,
            LGHJH: $scope.BaseData.Plan[this.UnitCode + '-SH032-LGHJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH032.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        SSX: undefined,
        WXGBJH: undefined,
        WXGBWC: undefined,
        YLBJJH: undefined,
        YLBJWC: undefined,
        SWZJH: undefined,
        SWZWC: undefined,
        BJQJH: undefined,
        BJQWC: undefined,
        LGHJH: undefined,
        LGHWC: undefined
    }
};

App.Models.SH.SH033 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH033.Object(), this, {
            DataOrder: (i + 1),
            XJPTYSJH: $scope.BaseData.Plan[this.UnitCode + '-SH033-XJPTYSJH'] || 0,
            YDSBJH: $scope.BaseData.Plan[this.UnitCode + '-SH033-YDSBJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH033.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        SSX: undefined,
        XJHS: undefined,
        XJYJ: undefined,
        XJPT: undefined,
        XJPTYSJH: undefined,
        XJPTYSWC: undefined,
        YDSBJH: undefined,
        YDSBWC: undefined
    }
};

App.Models.SH.SH034 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH034.Object(), this, {
            DataOrder: (i + 1)
        }));
    });

    return models;
}

App.Models.SH.SH034.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        XZQMC: undefined,
        YJZB: undefined,
        YJGZ: undefined,
        RJZB: undefined,
        RJGZ: undefined,
        XTJC: undefined,
        HTYS: undefined
    }
};

App.Models.SH.SH035 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH035.Object(), this, {
            DataOrder: (i + 1)
        }));
    });

    return models;
}

App.Models.SH.SH035.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        XZQMC: undefined,
        SWXXGX: undefined,
        QXXXGX: undefined,
        GTXXGX: undefined,
        YJXXGX: undefined
    }
};

App.Models.SH.SH036 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH036.Object(), this, {
            DataOrder: (i + 1),
            XYAJH: $scope.BaseData.Plan[this.UnitCode + '-SH036-XYAJH'] || 0,
            XZYAJH: $scope.BaseData.Plan[this.UnitCode + '-SH036-XZYAJH'] || 0,
            CYAJH: $scope.BaseData.Plan[this.UnitCode + '-SH036-CYAJH'] || 0,
            QTYAJH: $scope.BaseData.Plan[this.UnitCode + '-SH036-QTYAJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH036.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        XYAJH: undefined,
        XYAWC: undefined,
        XZYAJH: undefined,
        XZYAWC: undefined,
        CYAJH: undefined,
        CYAWC: undefined,
        QTYAJH: undefined,
        QTYAWC: undefined
    }
};

App.Models.SH.SH037 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH037.Object(), this, {
            DataOrder: (i + 1),
            XCLJH: $scope.BaseData.Plan[this.UnitCode + '-SH037-XCLJH'] || 0,
            JSPJH: $scope.BaseData.Plan[this.UnitCode + '-SH037-JSPJH'] || 0,
            MBKJH: $scope.BaseData.Plan[this.UnitCode + '-SH037-MBKJH'] || 0,
            GPJH: $scope.BaseData.Plan[this.UnitCode + '-SH037-GPJH'] || 0,
            SCJH: $scope.BaseData.Plan[this.UnitCode + '-SH037-SCJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH037.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        SSX: undefined,
        XCLJH: undefined,
        XCLWC: undefined,
        JSPJH: undefined,
        JSPWC: undefined,
        MBKJH: undefined,
        MBKWC: undefined,
        GPJH: undefined,
        GPWC: undefined,
        SCJH: undefined,
        SCWC: undefined
    }
};

App.Models.SH.SH038 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH038.Object(), this, {
            DataOrder: (i + 1),
            PXCCJH: $scope.BaseData.Plan[this.UnitCode + '-SH038-PXCCJH'] || 0,
            PXRSJH: $scope.BaseData.Plan[this.UnitCode + '-SH038-PXRSJH'] || 0,
            YLCCJH: $scope.BaseData.Plan[this.UnitCode + '-SH038-YLCCJH'] || 0,
            YLRCJH: $scope.BaseData.Plan[this.UnitCode + '-SH038-YLRCJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH038.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        SSX: undefined,
        PXCCJH: undefined,
        PXCCWC: undefined,
        PXRSJH: undefined,
        PXRSWC: undefined,
        YLCCJH: undefined,
        YLCCWC: undefined,
        YLRCJH: undefined,
        YLRCWC: undefined
    }
};

App.Models.SH.SH041 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH041.Object(), this, {
            DataOrder: (i + 1)
        }));
    });

    return models;
}

App.Models.SH.SH041.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        SSX: undefined,
        ZDYJZB: undefined,
        ZDYJCG: undefined,
        ZDYJCGSL: undefined,
        SJFWSQD: undefined,
        SJFWSMC: undefined,
        PX: undefined
    }
};

App.Models.SH.SH042 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH042.Object(), this, {
            DataOrder: (i + 1),
            SHJJDCJH: $scope.BaseData.Plan[this.UnitCode + '-SH042-SHJJDCJH'] || 0,
            SHWXQYJH: $scope.BaseData.Plan[this.UnitCode + '-SH042-SHWXQYJH'] || 0,
            YHCCLJH: $scope.BaseData.Plan[this.UnitCode + '-SH042-YHCCLJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH042.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        SSX: undefined,
        DCDX: undefined,
        SWJC: undefined,
        DXLS: undefined,
        LSSH: undefined,
        XLYJCXX: undefined,
        SHJJDCJH: undefined,
        SHJJDCWC: undefined,
        SHWXQYJH: undefined,
        SHWXQYWC: undefined,
        SSGC: undefined,
        YHCCLJH: undefined,
        YHCCLWC: undefined
    }
};

App.Models.SH.SH043 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH043.Object(), this, {
            DataOrder: (i + 1),
            YHCPJJH: $scope.BaseData.Plan[this.UnitCode + '-SH043-YHCPJJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH043.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        SSX: undefined,
        XLYFX: undefined,
        YHCPJJH: undefined,
        YHCPJWC: undefined,
        DCBG: undefined
    }
};

App.Models.SH.SH044 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH044.Object(), this, {
            DataOrder: (i + 1)
        }));
    });

    return models;
}

App.Models.SH.SH044.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        XZQMC: undefined,
        XJWC: undefined,
        S1JWC: undefined,
        S2JWC: undefined
    }
};

App.Models.SH.SH045 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH045.Object(), this, {
            DataOrder: (i + 1),
            PXCSJH: $scope.BaseData.Plan[this.UnitCode + '-SH045-PXCSJH'] || 0,
            PXRSJH: $scope.BaseData.Plan[this.UnitCode + '-SH045-PXRSJH'] || 0
        }));
    });

    return models;
}

App.Models.SH.SH045.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        XZQMC: undefined,
        PXCSJH: undefined,
        PXCSWC: undefined,
        PXRSJH: undefined,
        PXRSWC: undefined
    }
};

App.Models.SH.SH046 = function (units) {
    var models = [];
    $.each(units, function (i) {
        models.push($.extend(App.Models.SH.SH046.Object(), this, {
            DataOrder: (i + 1),
            UnitCode: this.UnitCode,
            DW: this.UnitName
        }));
    });

    return models;
};

App.Models.SH.SH046.Object = function () {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        DW: undefined,
        CGJYXS: undefined,
        XZXS: undefined,
        BZ: undefined,
        ZBJD: undefined,
        SFKG: undefined,
        JSJD: undefined
    };
};

App.Models.SH.SH051 = function (units) {
    var models = [];
    $.each(units, function(i) {
        models.push($.extend(App.Models.SH.SH051.Object(), this, {
            DataOrder: (i + 1),
            UnitCode: this.UnitCode,
            DW: this.UnitName
        }));
    });

    return models;
};

App.Models.SH.SH051.Object = function() {
    return {
        TBNO: undefined,
        PageNO: undefined,
        DataOrder: undefined,
        UnitCode: undefined,
        DW: undefined,
        YJFBGX: undefined,
        YJXJFBCS: undefined,
        YJDXTS: undefined,
        YJZRRS: undefined,
        QDGBZ: undefined,
        ZYRS: undefined,
        BMSW: undefined,
        DTFW: undefined,
        SSSH: undefined,
        BZ: undefined
    };
};

App.Models.Page = {
    SH01: {
        "1": "资金情况表"
    },
    SH02: {
        "1": "山洪沟治理进度统计表"
    },
    SH03: {
        "1": "检测系统建设表",
        "2": "预警系统建设表",
        "3": "县级平台完善表",
        "4": "信息管理表",
        "5": "信息共享表",
        "6": "预案编制完善表",
        "7": "措施宣传表",
        "8": "培训演练表"
    },
    SH04: {
        "1": "调查评价表"
        /*"1": "调查工作准备",
        "2": "现场调查",
        "3": "分析评价",
        "4": "审核汇集",
        "5": "调查评价培训"*/
    },
    SH05: {
        "1": "效益分析表"
    }
};

if (window.initData) {
    window.initData.BaseData.Page = App.Models.Page;
}else if (window.init_data) {
    window.init_data.BaseData.Page = App.Models.Page;
}