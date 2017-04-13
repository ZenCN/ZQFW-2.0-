App.Models = App.Models || {};
App.Models.HL = App.Models.HL || {
    HL01: {}
};
App.Models.HL.HL01.ReportDetials = App.Models.HL.HL01.ReportDetials || {};

App.Models.HL.HL01.ReportDetials["51"] = function (rpt, field) {
    var szdq = "", ctyCount = 0, simpleName, sdsw = 0, topFiveName = '', topFiveJJSS = '';
    $.each(rpt.HL011, function (i) {
        if (i > 0 && !this.DW.Contains("本级") && (Number(this.SZFWX) > 0 || Number(this.SZFWZ) > 0)) {
            szdq += this.DW + "、";

            if ($.cookie("limit") == "2") {
                ctyCount++;
            }
        }
    });
    szdq = szdq.slice(0, szdq.length - 1) + "共";
    var szfwx = rpt.HL011[0].SZFWX ? rpt.HL011[0].SZFWX : 0, szfwz = rpt.HL011[0].SZFWZ ? rpt.HL011[0].SZFWZ : 0;
        switch (Number($.cookie("limit"))) {
        case 2:
            simpleName = "省";
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

    $.each(rpt.HL012, function() {
        if (this.DeathReason.Contains('降雨-山洪灾害')) {
            sdsw++;
        }
    });

    $.each(rpt.HL011.BubbleSort('ZJJJZSS'), function (i) {
        if (i > 0) {
            topFiveName += this.DW + '、';
            topFiveJJSS += this.ZJJJZSS + '、';
            if (i == 5) {
                topFiveName = topFiveName.slice(0, topFiveName.length - 1);
                topFiveJJSS = topFiveJJSS.slice(0, topFiveJJSS.length - 1);
                return false;
            }
        }
    });


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
        Name: simpleName,
        SZDQ: szdq,
        SZRK: fn(rpt.HL011[0].SZRK),
        SZRK_Unit: field.SZRK ? field.SZRK.MeasureName : "（人）",
        ZYRK: fn(rpt.HL011[0].ZYRK),
        ZYRK_Unit: field.ZYRK ? field.ZYRK.MeasureName : "（人）",
        DTFW: fn(rpt.HL011[0].DTFW),
        DTFW_Unit: field.DTFW ? field.DTFW.MeasureName : "（间）",
        SWRK: fn(rpt.HL011[0].SWRK, 0),
        SDSW: sdsw,  //其中山地灾害死亡人数
        SZRKR: fn(rpt.HL011[0].SZRKR, 0),
        SHMJXJ: fn(rpt.HL011[0].SHMJXJ),
        SHMJXJ_Unit: field.SHMJXJ ? field.SHMJXJ.MeasureName : "（千公顷）",
        CZMJXJ: fn(rpt.HL011[0].CZMJXJ),
        CZMJXJ_Unit: field.SHMJXJ ? field.CZMJXJ.MeasureName : "（千公顷）",
        JSMJXJ: fn(rpt.HL011[0].JSMJXJ),
        JSMJXJ_Unit: field.JSMJXJ ? field.JSMJXJ.MeasureName : "（千公顷）",
        YZJCLS: fn(rpt.HL011[0].YZJCLS),
        YZJCLS_Unit: field.YZJCLS ? field.YZJCLS.MeasureName : "（万吨）",
        GLZD: rpt.HL011[0].GLZD || 0,
        GDZD: rpt.HL011[0].GDZD || 0,
        TXZD: rpt.HL011[0].TXZD || 0,
        SHSK: undefined,  //????  顺坏水库
        SHDFCS: rpt.HL011[0].SHDFCS || 0,
        SHDFCD: fn(rpt.HL011[0].SHDFCD),
        ZJJJZSS: fn(rpt.HL011[0].ZJJJZSS),
        SLSSZJJJSS: fn(rpt.HL011[0].SLSSZJJJSS),
        Money_Unit: field.ZJJJZSS ? field.ZJJJZSS.MeasureName : "（亿元）",
        Top_Five_Name: topFiveName,
        Top_Five_JJSS: topFiveJJSS
    }
};