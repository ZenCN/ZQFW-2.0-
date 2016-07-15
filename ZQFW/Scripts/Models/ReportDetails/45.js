App.Models = App.Models || {};
App.Models.HL = App.Models.HL || {
    HL01: {}
};
App.Models.HL.HL01.ReportDetials = App.Models.HL.HL01.ReportDetials || {};

App.Models.HL.HL01.ReportDetials["45"] = function (rpt, field) {
    var szdq = "", ctyCount = 0, simpleName;
    $.each(rpt.HL011, function(i) {
        if (i > 0 && !this.DW.Contains("本级") && (Number(this.SZFWX) > 0 || Number(this.SZFWZ) > 0)) {
            szdq += this.DW + "、";

            if ($.cookie("limit") == "2") {
                ctyCount++;
            }
        }
    });
    szdq = szdq.slice(0, szdq.length - 1) + "等";
    var szfwx = rpt.HL011[0].SZFWX ? rpt.HL011[0].SZFWX : 0, szfwz = rpt.HL011[0].SZFWZ ? rpt.HL011[0].SZFWZ : 0;
    switch (Number($.cookie("limit"))) {
        case 2:
            simpleName = "自治区";
            szdq += ctyCount + "个市、" + szfwx + "个县(市、区)、" + szfwz + "个乡(镇、街道)";
            break;
        case 3:
            simpleName = "市";
            szdq += szfwx + "个县(市、区)、" + szfwz + "个乡(镇、街道)";
            break;
        case 4:
            simpleName = "县";
            szdq += szfwz + "个乡(镇、街道)";
            break;
    }

    var fn = function (val, fixed) {
        if (Number(val) > 0) {
            fixed = fixed == undefined ? 2 : fixed;
            var result = parseFloat(val).toFixed(fixed);
            result = Number(result) > 0 ? result : 0;
        } else {
            result = 0;
        }

        return result;
    };

    return {
        //----------------ReportTitle--------------------
        UnitName: $.cookie("realname") || $.cookie("fullname"),
        TimeNow: App.Tools.Date.GetToday("yyyy年MM月dd日"),
        StartDateTime: App.Tools.Date.GetToday("yyyy年MM月dd日", rpt.ReportTitle.StartDateTime),  //顺序要在EndDateTime之前
        EndDateTime: App.Tools.Date.GetToday("yyyy年MM月dd日", rpt.ReportTitle.EndDateTime),
        //-------------------HL011-----------------------
        SimpleName: simpleName,
        SZDQ: szdq,  // 未写
        SZRK: fn(rpt.HL011[0].SZRK),
        SZRK_Unit: field.SZRK ? field.SZRK.MeasureName : "（人）",
        SWRK: fn(rpt.HL011[0].SWRK, 0),
        SWRK_Unit: field.SWRK ? field.SWRK.MeasureName : "（人）",
        SZRKR: fn(rpt.HL011[0].SZRKR, 0),
        //SZRKR_Unit: field.SZRKR ? field.SZRKR.MeasureName : "（人）",
        DTFW: fn(rpt.HL011[0].DTFW),
        DTFW_Unit: field.DTFW ? field.DTFW.MeasureName : "（间）",
        SHMJXJ: fn(rpt.HL011[0].SHMJXJ),
        SHMJXJ_Unit: field.SHMJXJ ? field.SHMJXJ.MeasureName : "（千公顷）",
        CZMJXJ: fn(rpt.HL011[0].CZMJXJ),
        CZMJXJ_Unit: field.SHMJXJ ? field.CZMJXJ.MeasureName : "（千公顷）",
        JSMJXJ: fn(rpt.HL011[0].JSMJXJ),
        JSMJXJ_Unit: field.JSMJXJ ? field.JSMJXJ.MeasureName : "（千公顷）",
        YZJCLS: fn(rpt.HL011[0].YZJCLS),
        YZJCLS_Unit: field.YZJCLS ? field.YZJCLS.MeasureName : "（万吨）",
        JJZWSS: fn(rpt.HL011[0].JJZWSS),
        JJZWSS_Unit: field.JJZWSS ? field.JJZWSS.MeasureName : "（万元）",
        SWDSC: fn(rpt.HL011[0].SWDSC),
        SWDSC_Unit: field.SWDSC ? field.SWDSC.MeasureName : "（万头）",
        SCYZSL: fn(rpt.HL011[0].SCYZSL),
        SCYZSL_Unit: field.SCYZSL ? field.SCYZSL.MeasureName : "（万吨）",
        TCGKQY: rpt.HL011[0].TCGKQY || 0,
        GLZD: rpt.HL011[0].GLZD || 0,
        GDZD: rpt.HL011[0].GDZD || 0,
        TXZD: rpt.HL011[0].TXZD || 0,
        SHDFCS: rpt.HL011[0].SHDFCS || 0,
        SHDFCD: fn(rpt.HL011[0].SHDFCD),
        SHHAC: rpt.HL011[0].SHHAC || 0,
        SHSZ: rpt.HL011[0].SHSZ || 0,
        SHJDJ: rpt.HL011[0].SHJDJ || 0,
        SHJDBZ: rpt.HL011[0].SHJDBZ || 0,
        SHSWCZ: rpt.HL011[0].SHSWCZ || 0,
        ZJJJZSS: fn(rpt.HL011[0].ZJJJZSS),
        ZJJJZSS_Unit: field.ZJJJZSS ? field.ZJJJZSS.MeasureName : "（亿元）",
        NLMYZJJJSS: fn(rpt.HL011[0].NLMYZJJJSS),
        NLMYZJJJSS_Unit: field.NLMYZJJJSS ? field.NLMYZJJJSS.MeasureName : "（亿元）",
        GJYSZJJJSS: fn(rpt.HL011[0].GJYSZJJJSS),
        GJYSZJJJSS_Unit: field.GJYSZJJJSS ? field.GJYSZJJJSS.MeasureName : "（亿元）",
        SLSSZJJJSS: fn(rpt.HL011[0].SLSSZJJJSS),
        SLSSZJJJSS_Unit: field.SLSSZJJJSS ? field.SLSSZJJJSS.MeasureName : "（亿元）",
        //-------------------HL013-----------------------
        GCHSWKRK: fn(rpt.HL013[0].GCHSWKRK),
        GCHSWKRK_Unit: field.GCHSWKRK ? field.GCHSWKRK.MeasureName : "（人）",
        GCJJZYRK: fn(rpt.HL013[0].GCJJZYRK),
        GCJJZYRK_Unit: field.GCJJZYRK ? field.GCJJZYRK.MeasureName : "（人）",
        //-------------------HL014-----------------------
        QXHJ: rpt.HL014[0].QXHJ || 0,
        SBJX: rpt.HL014[0].SBJX || 0,
        WZBZD: fn(rpt.HL014[0].WZBZD),
        WZBZD_Unit: field.WZBZD ? field.WZBZD.MeasureName : "（万条）",
        WZBZB: fn(rpt.HL014[0].WZBZB),
        WZBZB_Unit: field.WZBZB ? field.WZBZB.MeasureName : "（万平方米）",
        WZSSL: fn(rpt.HL014[0].WZSSL),
        WZSSL_Unit: field.WZSSL ? field.WZSSL.MeasureName : "（万立方米）",
        WZMC: fn(rpt.HL014[0].WZMC),
        WZMC_Unit: field.WZMC ? field.WZMC.MeasureName : "（万立方米）",
        WZGC: fn(rpt.HL014[0].WZGC),
        WZGC_Unit: field.WZGC ? field.WZGC.MeasureName : "（吨）",
        WZY: fn(rpt.HL014[0].WZY),
        WZY_Unit: field.WZY ? field.WZY.MeasureName : "（吨）",
        WZD: fn(rpt.HL014[0].WZD),
        WZD_Unit: field.WZD ? field.WZD.MeasureName : "（万度）",
        WZZXH: fn(rpt.HL014[0].WZZXH),
        WZZXH_Unit: field.WZZXH ? field.WZZXH.MeasureName : "（万元）",
        XYJYGD: fn(rpt.HL014[0].XYJYGD),
        XYJYGD_Unit: field.XYJYGD ? field.XYJYGD.MeasureName : "（千公顷）",
        XYJMLSJS: fn(rpt.HL014[0].XYJMLSJS),
        XYJMLSJS_Unit: field.XYJMLSJS ? field.XYJMLSJS.MeasureName : "（万吨）",
        XYJSSZRK: fn(rpt.HL014[0].XYJSSZRK),
        XYJSSZRK_Unit: field.XYJSSZRK ? field.XYJSSZRK.MeasureName : "（人）",
        XYJJQZ: fn(rpt.HL014[0].XYJJQZ),
        XYJJQZ_Unit: field.XYJJQZ ? field.XYJJQZ.MeasureName : "（人）",
        XYJZJJXY: fn(rpt.HL014[0].XYJZJJXY),
        XYJZJJXY_Unit: field.XYJZJJXY ? field.XYJZJJXY.MeasureName : "（亿元）"
    }
};