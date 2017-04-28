App.Models = App.Models || {};
App.Models.HL = App.Models.HL || {
    HL01: {}
};
App.Models.HL.HL01.ReportDetials = App.Models.HL.HL01.ReportDetials || {};

App.Models.HL.HL01.ReportDetials["51"] = function (rpt, field) {
    var szdq = "", ctyCount = 0, simpleName, shsk = 0, sdsw = 0, topFiveName = '', topFiveJJSS = '';
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

    shsk = App.Tools.Calculator.Addition(rpt.HL011[0].SHSKD, rpt.HL011[0].SHSKX);

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

    var result = {
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
        SHSK: shsk,  //????  顺坏水库
        SHDFCS: rpt.HL011[0].SHDFCS || 0,
        SHDFCD: fn(rpt.HL011[0].SHDFCD),
        ZJJJZSS: fn(rpt.HL011[0].ZJJJZSS),
        SLSSZJJJSS: fn(rpt.HL011[0].SLSSZJJJSS),
        Money_Unit: field.ZJJJZSS ? field.ZJJJZSS.MeasureName : "（万元）",
        Top_Five_Name: topFiveName,
        Top_Five_JJSS: topFiveJJSS
    }

    function division(arg1, arg2, n) //除法
    {
        arg1 = arg1 == undefined ? 0 : arg1;
        arg2 = Number(arg2) > 0 ? arg2 : 1;
        if (arg1 == 0 || arg2 == 0) {
            return 0; //此处不能返回undefined
        } else {
            var t1 = 0,
                t2 = 0,
                r1,
                r2;
            try {
                t1 = arg1.toString().split(".")[1].length;
            } catch (e) {
            }
            try {
                t2 = arg2.toString().split(".")[1].length;
            } catch (e) {
            }
            with (Math) {
                r1 = Number(arg1.toString().replace(".", ""));
                r2 = Number(arg2.toString().replace(".", ""));
                n = n == undefined ? 4 : n;
                return parseFloat(((r1 / r2) * pow(10, t2 - t1)).toFixed(n));
            }
        }
    }

    $.ajax({
        url: '/index/getlastfewyeardata?start=' + result.StartDateTime + '&end=' + result.EndDateTime,
        async: false,
        success: function(data) {
            data = eval("(" + data + ")");
            if (angular.isObject(data)) {
                result.SZ_Last = division(result.SZRK * field.SZRK.MeasureValue, data.QNTQ.SZRK, 2);
                result.HL_Last = division(result.SHMJXJ * field.SHMJXJ.MeasureValue, data.QNTQ.SHMJXJ, 2);
                result.SW_Last = division(result.SWRK * field.SWRK.MeasureValue, data.QNTQ.SWRK, 2);
                result.SZR_Last = division(result.SZRKR * field.SZRKR.MeasureValue, data.QNTQ.SZRKR, 2);
                result.ZJSS_Last = division(result.ZJJJZSS * field.ZJJJZSS.MeasureValue, data.QNTQ.ZJJJZSS, 2);

                result.SZ_All = division(result.SZRK * field.SZRK.MeasureValue, data.LNPJ.SZRK, 2);
                result.HL_All = division(result.SHMJXJ * field.SHMJXJ.MeasureValue, data.LNPJ.SHMJXJ, 2);
                result.SW_All = division(result.SWRK * field.SWRK.MeasureValue, data.LNPJ.SWRK, 2);
                result.SZR_All = division(result.SZRKR * field.SZRKR.MeasureValue, data.LNPJ.SZRKR, 2);
                result.ZJSS_All = division(result.ZJJJZSS * field.ZJJJZSS.MeasureValue, data.LNPJ.ZJJJZSS, 2);
            }
        }
    });

    return result;
};


App.Models.HL.HL01.ReportDetials["51"].SaveSvg = function(rpt, field) {
    var title, pie_data = [], pie_chart, img_url;
    switch (Number($.cookie('limit'))) {
    case 2:
        title = '各市（州）直接经济总损失占全省损失比例图';
        break;
    case 3:
        title = '各县（区）直接经济总损失占全市损失比例图';
        break;
    case 4:
        title = '各乡（镇）直接经济总损失占全县损失比例图';
        break;
    }
    
    $.each(rpt.HL011, function(i) {
        if (i > 0 && Number(this.ZJJJZSS) > 0) {
            pie_data.push({ name: this.DW, y: Number(this.ZJJJZSS), sliced: true });
        }
    });

    pie_chart = new Highcharts.Chart({
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            },
            renderTo: 'pie_chart',
        },
        title: {
            text: title,
            style: {
                fontSize: '18px',
                color: '#000'
            }
        },
        subtitle: {
            text: '单位：' + (field.ZJJJZSS ? field.ZJJJZSS.MeasureName : '（万元）'),
            style: {
                fontSize: '14px',
                color: '#000'
            }
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y:.2f}' + (field.ZJJJZSS ? field.ZJJJZSS.MeasureName : '（万元）') + '</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                borderWidth: 10,
                depth: 35,
                dataLabels: {
                    enabled: true,
                    format: '{point.name}({point.y:.2f}，{point.percentage:.1f}%)',
                    color: '#000',
                    style: {
                        color: '#000',
                        fontSize: '12px',
                        fontFamily: '宋体'
                    }
                }
            }
        },
        series: [
            {
                type: 'pie',
                name: '总损失',
                data: pie_data
            }
        ],
        credits: {
            enabled: false //禁用版权信息
        },
        exporting: {
            enabled: false //禁用导出按钮
        }
    });

    $.ajax({
        type: "post",
        data: { svg: pie_chart.getSVG() },
        url: "/Hightcharts/saveImgFromHightcharts",
        success: function (str) {
            img_url = str;
        },
        async: false
    });

    return img_url;
};