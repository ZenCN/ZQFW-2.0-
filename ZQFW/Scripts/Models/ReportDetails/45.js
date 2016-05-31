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

    return {
        //----------------ReportTitle--------------------
        UnitName: $.cookie("realname") || $.cookie("fullname"),
        TimeNow: App.Tools.Date.GetToday("yyyy年MM月dd日"),
        StartDateTime: App.Tools.Date.GetToday("yyyy年MM月dd日", rpt.ReportTitle.StartDateTime),  //顺序要在EndDateTime之前
        EndDateTime: App.Tools.Date.GetToday("yyyy年MM月dd日", rpt.ReportTitle.EndDateTime),
        //-------------------HL011-----------------------
        SimpleName: simpleName,
        SZDQ: szdq,  // 未写
        SZRK: rpt.HL011[0].SZRK || 0,
        SZRK_Unit: field.SZRK ? field.SZRK.MeasureName : "（人）",
        SWRK: rpt.HL011[0].SWRK || 0,
        SWRK_Unit: field.SWRK ? field.SWRK.MeasureName : "（人）",
        DTFW: rpt.HL011[0].DTFW || 0,
        DTFW_Unit: field.DTFW ? field.DTFW.MeasureName : "（间）",
        SHMJXJ: rpt.HL011[0].SHMJXJ || 0,
        SHMJXJ_Unit: field.SHMJXJ ? field.SHMJXJ.MeasureName : "（千公顷）",
        CZMJXJ: rpt.HL011[0].CZMJXJ || 0,
        CZMJXJ_Unit: field.SHMJXJ ? field.CZMJXJ.MeasureName : "（千公顷）",
        JSMJXJ: rpt.HL011[0].JSMJXJ || 0,
        JSMJXJ_Unit: field.JSMJXJ ? field.JSMJXJ.MeasureName : "（千公顷）",
        YZJCLS: rpt.HL011[0].YZJCLS || 0,
        YZJCLS_Unit: field.YZJCLS ? field.YZJCLS.MeasureName : "（万吨）",
        JJZWSS: rpt.HL011[0].JJZWSS || 0,
        JJZWSS_Unit: field.JJZWSS ? field.JJZWSS.MeasureName : "（万元）",
        SWDSC: rpt.HL011[0].SWDSC || 0,
        SWDSC_Unit: field.SWDSC ? field.SWDSC.MeasureName : "（万头）",
        SCYZSL: rpt.HL011[0].SCYZSL || 0,
        SCYZSL_Unit: field.SCYZSL ? field.SCYZSL.MeasureName : "（万吨）",
        TCGKQY: rpt.HL011[0].TCGKQY || 0,
        GLZD: rpt.HL011[0].GLZD || 0,
        GDZD: rpt.HL011[0].GDZD || 0,
        TXZD: rpt.HL011[0].TXZD || 0,
        SHDFCS: rpt.HL011[0].SHDFCS || 0,
        SHDFCD: rpt.HL011[0].SHDFCD || 0,
        SHHAC: rpt.HL011[0].SHHAC || 0,
        SHSZ: rpt.HL011[0].SHSZ || 0,
        SHJDJ: rpt.HL011[0].SHJDJ || 0,
        SHJDBZ: rpt.HL011[0].SHJDBZ || 0,
        SHSWCZ: rpt.HL011[0].SHSWCZ || 0,
        ZJJJZSS: rpt.HL011[0].ZJJJZSS || 0,
        ZJJJZSS_Unit: field.ZJJJZSS ? field.ZJJJZSS.MeasureName : "（亿元）",
        NLMYZJJJSS: rpt.HL011[0].NLMYZJJJSS || 0,
        NLMYZJJJSS_Unit: field.NLMYZJJJSS ? field.NLMYZJJJSS.MeasureName : "（亿元）",
        GJYSZJJJSS: rpt.HL011[0].GJYSZJJJSS || 0,
        GJYSZJJJSS_Unit: field.GJYSZJJJSS ? field.GJYSZJJJSS.MeasureName : "（亿元）",
        SLSSZJJJSS: rpt.HL011[0].SLSSZJJJSS || 0,
        SLSSZJJJSS_Unit: field.SLSSZJJJSS ? field.SLSSZJJJSS.MeasureName : "（亿元）",
        //-------------------HL013-----------------------
        GCHSWKRK: rpt.HL013[0].GCHSWKRK || 0,
        GCHSWKRK_Unit: field.GCHSWKRK ? field.GCHSWKRK.MeasureName : "（人）",
        GCJJZYRK: rpt.HL013[0].GCJJZYRK || 0,
        GCJJZYRK_Unit: field.GCJJZYRK ? field.GCJJZYRK.MeasureName : "（人）",
        //-------------------HL014-----------------------
        QXHJ: rpt.HL014[0].QXHJ || 0,
        SBJX: rpt.HL014[0].SBJX || 0,
        WZBZD: rpt.HL014[0].WZBZD || 0,
        WZBZD_Unit: field.WZBZD ? field.WZBZD.MeasureName : "（万条）",
        WZBZB: rpt.HL014[0].WZBZB || 0,
        WZBZB_Unit: field.WZBZB ? field.WZBZB.MeasureName : "（万平方米）",
        WZSSL: rpt.HL014[0].WZSSL || 0,
        WZSSL_Unit: field.WZSSL ? field.WZSSL.MeasureName : "（万立方米）",
        WZMC: rpt.HL014[0].WZMC || 0,
        WZMC_Unit: field.WZMC ? field.WZMC.MeasureName : "（万立方米）",
        WZGC: rpt.HL014[0].WZGC || 0,
        WZGC_Unit: field.WZGC ? field.WZGC.MeasureName : "（吨）",
        WZY: rpt.HL014[0].WZY || 0,
        WZY_Unit: field.WZY ? field.WZY.MeasureName : "（吨）",
        WZD: rpt.HL014[0].WZD || 0,
        WZD_Unit: field.WZD ? field.WZD.MeasureName : "（万度）",
        WZZXH: rpt.HL014[0].WZZXH || 0,
        WZZXH_Unit: field.WZZXH ? field.WZZXH.MeasureName : "（万元）",
        XYJYGD: rpt.HL014[0].XYJYGD || 0,
        XYJYGD_Unit: field.XYJYGD ? field.XYJYGD.MeasureName : "（千公顷）",
        XYJMLSJS: rpt.HL014[0].XYJMLSJS || 0,
        XYJMLSJS_Unit: field.XYJMLSJS ? field.XYJMLSJS.MeasureName : "（万吨）",
        XYJSSZRK: rpt.HL014[0].XYJSSZRK || 0,
        XYJSSZRK_Unit: field.XYJSSZRK ? field.XYJSSZRK.MeasureName : "（人）",
        XYJJQZ: rpt.HL014[0].XYJJQZ || 0,
        XYJJQZ_Unit: field.XYJJQZ ? field.XYJJQZ.MeasureName : "（人）",
        XYJZJJXY: rpt.HL014[0].XYJZJJXY || 0,
        XYJZJJXY_Unit: field.XYJZJJXY ? field.XYJZJJXY.MeasureName : "（亿元）"
    }
};