function clickchart(chartType, disasterInfoType) {
    var disasterInfo = getGlobalDisasterInfo();         //获取灾情数据字符串
    if (disasterInfo == "") {
        alert("当前区域无灾情数据");
        return;
    }
    var dataName = mapParams.disasterInfoTypes[disasterInfoType].name;  //获取数据名称
    var dataUnit = mapParams.disasterInfoTypes[disasterInfoType].unit;   //获取数据单位
    var fixed = mapParams.disasterInfoTypes[disasterInfoType].fixed;   //获取小数位数
    var timer = ruler1.getSelected()
    var subtitle = timer.startyear + "年" + timer.startmonth + "月" + timer.startday + "日" + "—" +
    timer.endyear + "年" + timer.endmonth + "月" + timer.endday + "日" + "(" + timer.sorcetype + ")";
    var period = timer.startyear + timer.startmonth + timer.startday + "-" + timer.endyear + timer.endmonth + timer.endday;
    var disasterInfoObj = eval(disasterInfo);          //灾情数据数组

    var mapLevel = window.frames[mapParams.mapContainerName].getMapLevel();  //获取地图级别     

    if (chartType == 0) {
        var bar_name = [];
        var bar_data = [];
        var maxdata = 0;
        for (singleUnit in disasterInfoObj) {
            if (mapParams.mapType == 1 && mapLevel == 0 && getUnitDegree(singleUnit) == 1) {
                continue;
            }
            var singleUnitDisasterInfoArr = disasterInfoObj[singleUnit];
            bar_name.push(singleUnitDisasterInfoArr.UnitName);

            //向数组添加对应类型的灾害值
//            var dataIndex = mapParams.disasterInfoTypes[disasterInfoType].index;  //获取数据索引
            var singleDisasterTypeData = singleUnitDisasterInfoArr[disasterInfoType];
            if (Number(singleDisasterTypeData) > 0) {
                bar_data.push(parseFloat(singleDisasterTypeData))
            }
            if (parseFloat(singleDisasterTypeData) > maxdata) {
                maxdata = parseFloat(singleDisasterTypeData)
            }
        }

        if (maxdata <= 0) {
            alert("没有" + dataName);
            return;
        }
        else {
                //showchartbar(regionName, dataName, dataUnit, pageNO, bar_name, bar_data);
                showchartbar(dataName, dataUnit, fixed, subtitle, bar_name, bar_data);
        }
    }
    else {
        var pie_data = [];
        var maxdata = 0;
        for (singleUnit in disasterInfoObj) {
            if (mapParams.mapType == 1 && mapLevel == 0 && getUnitDegree(singleUnit) == 1) {
                continue;
            }
            var singleUnitDisasterInfoArr = disasterInfoObj[singleUnit];
            var singleDisasterTypeDataArr = [];
            singleDisasterTypeDataArr.push(singleUnitDisasterInfoArr.UnitName)

            //向数组添加对应类型的灾害值
//            var dataIndex = mapParams.disasterInfoTypes[disasterInfoType].index;  //获取数据索引
            var singleDisasterTypeData = singleUnitDisasterInfoArr[disasterInfoType];
            singleDisasterTypeDataArr.push(parseFloat(singleDisasterTypeData));
            if (parseFloat(singleDisasterTypeData) > maxdata) {
                maxdata = parseFloat(singleDisasterTypeData);
            }
            if (Number(singleDisasterTypeData) > 0) {
                pie_data.push(singleDisasterTypeDataArr)
            }
        }
        if (maxdata <= 0) {
            alert("没有" + dataName);
            return;
        }
        else {
            if (chartType == 1) {
                //showchartpie(regionName, dataName, dataUnit, pageNO, pie_data);
                showchartpie(dataName, dataUnit, fixed, subtitle, pie_data);
            }
            else {
                showchartp3(dataName, dataUnit, fixed, subtitle, pie_data);
            }
        }
    }
}

//根据单位代码得到单位级别（0国家级，1省级，2市级，3县级，4乡镇）
function getUnitDegree(unitCode) {
        if (unitCode == "00000000")
            return 0;
        else if (unitCode.substr(2, 6) == "000000")
            return 1;
        else if (unitCode.substr(4, 4) == "0000")
            return 2;
        else if (unitCode.substr(6, 2) == "00")
            return 3;
        else
            return 4;
}

var showchartbar = function (dataName, dataUnit, fixed, subtitle, bar_name, bar_data) {
    var title = dataName + '柱状图'
    var yAxisTitle = dataName + "(" + dataUnit + ")";

    chart_bar = new Highcharts.Chart({
        credits: {
            enabled: false
        },
        chart: {
            renderTo: 'floatbox',
            defaultSeriesType: 'column'
        },
        title: {
            text: title,
            style: {
                fontFamily: '宋体'
            }
            //x:-20
        },
        subtitle: {
            text: subtitle,
            style: {
              fontFamily: '宋体',
              fontSize: '13px'
            }
            //x: -20
        },
        xAxis: {
            categories: bar_name,
            labels: {
                rotation: -45,
                align: 'right',
                style: {
                    fontSize: '13px',
                    fontFamily: '宋体'
                    //                    ,
                    //                    fontFamily: 'Verdana, sans-serif'
                }
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: yAxisTitle,
                style:{fontSize: '13px'}
            }
        },
        tooltip: {
            formatter: function () {
                return this.x + ': ' + Highcharts.numberFormat(this.y, fixed) + " " + dataUnit;  // + ")"
            }
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        },
        series: [{
            name: yAxisTitle,
            data: bar_data

        }]
//,
//        exporting: {
//            buttons: {
//                exportButton: {
//                    menuItems: null,
//                    onclick: function () {
//                        //                            var cookie = $.cookie('passed');
//                        //                            if (cookie == '1') {
//                        this.options.exporting.width = this.chartWidth;
//                        this.exportChart();
//                        //                            } else {
//                        //                                alert('请先登录再下载');
//                        //                            }
//                    }
//                }
//            }
//        }
    });
};


var showchartpie = function (dataName, dataUnit, fixed, subtitle, pie_data) {
    //    alert(pie_data);
    var title = dataName + '饼状图'

    chart_pie = new Highcharts.Chart({
        credits: {
            enabled: false
        },
        chart: {
            renderTo: 'floatbox',
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false
        },
        title: {
            text: title,
            style: {
                fontFamily: '宋体'
            }
        },
        subtitle:
      {
          text: subtitle,
          style: {
              fontFamily: '宋体',
              fontSize: '13px'
          }
          //x: -20
      },
        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b>: ' + Highcharts.numberFormat(this.percentage, 2) + ' %';
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: '#000000',
                    connectorColor: '#000000',
                    formatter: function () {
                        return this.point.name + ': ' + Highcharts.numberFormat(this.y, fixed) + dataUnit;
                    },
                    style: {
                        fontFamily: '宋体'
                    }
                }
            }
        },
        series: [{
            type: 'pie',
            name: 'Browser share',
            data: pie_data

            //         [
            //            ['无锡',   45.0],
            //            ['淮安',       26.8],
            //            {
            //               name: '连云港',    
            //               y: 12.8,
            //               sliced: true,
            //               selected: true
            //            },
            //            ['南京',    8.5],
            //            ['杭州',     6.2],
            //            ['泰州',   0.7]
            //         ]
        }]
//,
//        exporting: {
//            buttons: {
//                exportButton: {
//                    menuItems: null,
//                    onclick: function () {
//                        //                        var cookie = $.cookie('passed');
//                        //                        if (cookie == '1') {
//                        this.options.exporting.width = this.chartWidth;
//                        this.exportChart();
//                        //                        } else {
//                        //                            alert('请先登录再下载');
//                        //                        }
//                    }
//                }
//            }
//        }
    });

    
}

function showchartp3(dataName, dataUnit, fixed, subtitle, pie_data) {
    var frameMap = window.frames[mapParams.mapContainerName];
    var regionName = frameMap.getRegionName();    //获取当前单位名称
    var title = regionName + dataName + '统计图';

    chart_pie = new Highcharts.Chart({
        credits: {
            enabled: false
        },
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            },
            renderTo: 'floatbox',
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false
        },
        title: {
            text: title,
            style: {
                fontFamily: '宋体'
            }
        },
        subtitle:
      {
          text: subtitle,
          style: {
              fontFamily: '宋体',
              fontSize: '13px'
          }
          //x: -20
      },
        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b>: ' + Highcharts.numberFormat(this.percentage, 2) + ' %';
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                depth: 35,
                dataLabels: {
                    enabled: true,
                    formatter: function () {
                        return this.point.name + ': ' + Highcharts.numberFormat(this.y, fixed) + dataUnit;
                    },
                    style: {
                        fontFamily: '宋体'
                    }
                }
            }
        },
        series: [{
            type: 'pie',
            name: 'Browser share',
            data: pie_data
        }]
//        ,
//        exporting: {
//            buttons: {
//                exportButton: {
//                    menuItems: null,
//                    onclick: function () {
//                        this.options.exporting.width = this.chartWidth;
//                        this.exportChart();
//                    }
//                }
//            }
//        }
    });
}
        
        //显示三维饼图
//        var chartp3 = function (property) {
//            if (property.renderTo) {
//                var sum = 0
//                for (var i = 0; i < property.datas.length; i++) {
//                    sum += parseFloat(property.datas[i]);
//                }

//                //        if (sum <= 0) {
//                //            alert("没有" + property.dataName);
//                //            return;
//                //        }

//                var title = $('<div style="font-family: 宋体;text-align:center;"></div>').append('<p style="margin:0 auto;padding-top:10px;font-size: 16px;color: #3E576F;">'
//                + property.title + '</p>').append('<p style="margin:0 auto;font-size: 12px;color: #6D869F;">' + property.subtitle + '</p>')

//                var imgWidth = "?width=" + 400;
//                var imgHeight = "&height=" + 200;
//                var Values = "&Values=" + encodeURIComponent(property.datas.join(","));
//                //                var Texts = "&Texts=" + encodeURIComponent(property.names.join(","));
//                //                var unit = "&unit=" + encodeURIComponent(property.dataUnit);
//                //                var imageName = "&imageName=" + encodeURIComponent(property.imageName)
//                var url = "GetPieChart" + imgWidth + imgHeight + Values;
//                var pie = $('<img alt="三维饼状图" style="float:left;margin:50px 40px 25px 40px" />').attr("src", url);

//                var colors = ["Red", "Green", "Blue", "Yellow", "Purple", "Olive", "Navy", "Aqua", "Lime", "Maroon", "Teal", "Fuchsia", "BlueViolet",
//         "Brown", "BurlyWood", "Aquamarine", "CadetBlue", "Chartreuse", "Chocolate", "Coral", "DarkBlue", "DarkCyan", "DarkGoldenrod", "DarkGray",
//         "DarkGreen", "DarkKhaki", "DarkMagenta", "DarkOliveGreen", "DarkOrange", "DarkOrchid", "DarkRed", "DarkSalmon",
//         "DarkSeaGreen", "DarkSlateBlue", "DarkSlateGray", "DarkTurquoise", "DarkViolet", "DeepPink", "DeepSkyBlue", "DimGray",
//         "DodgerBlue"]
//                var ulLeft = $('<ul style="	padding:0px;margin:20px 10px 0px 0px;list-style:none;height: 100%;width: 100%;"></ul>');
//                var ulBottom = $('<ul style="padding:0px;margin:0px 10px 10px 30px;list-style:none;height: 100%;width: 100%;"></ul>');
//                var margin = "3px";
//                if (!property.showTool) {
//                    margin = "-2px";
//                }
//                for (var i = 0; i < property.datas.length; i++) {
//                    if (i < 15) {
//                        if (i == 14) {
//                            margin = "3px";
//                        }
//                        var li = $('<li style="	margin-bottom:' + margin + ';display: block;width: 100%;height:14px;"></li>');
//                        var percent = parseFloat(property.datas[i]) / sum * 100
//                        var colorVal = property.names[i] + ":" + percent.toFixed(2) + "%";
//                        li.append('<div class="colorblocks" style="	float:left;width:14px;height:14px;margin-right:10px;background-color:' + colors[i] + '"></div>')
//                      .append('<span class="discription" style="font-size:12px;margin:0px;">' + colorVal + '</span>');
//                        ulLeft.append(li);
//                    }
//                    else {
//                        var li = $('<li style="float:left;margin-bottom:3px;display: inline-block;width: 150px;height:14px;"></li>');
//                        var percent = parseFloat(property.datas[i]) / sum * 100
//                        var colorVal = property.names[i] + ":" + percent.toFixed(2) + "%";
//                        li.append('<div class="colorblocks" style="	float:left;width:14px;height:14px;margin-right:10px;background-color:' + colors[i] + '"></div>')
//                      .append('<span class="discription" style="float:left;font-size:12px;margin:0px;">' + colorVal + '</span>');
//                        ulBottom.append(li);
//                    }
//                }
//                var pieLegend = $('<div class="legend" style="float:left;"></div>').append(ulLeft)  //width:200px;
//                var pieLegend1 = $('<div class="legend" style="float:left;"></div>').append(ulBottom)  //width:200px;
//                var piechart = $('<div id="p3" style="min-width:670px;min-height:300px;background-color:White;"></div>').append(pie).append(pieLegend).append(pieLegend1);  //height:300px;
//                if (property.showTool) {    //显示工具

//                    //                var svg = '<svg xmlns="http://www.w3.org/2000/svg" version="1.1" style="position: absolute;right: 0px;width: 80px;height:40px;">'
//                    //                + '<rect id="exportRect" rx="3" ry="3" fill="#f4f4f2" x="0.5" y="0.5" width="23" height="19" stroke-width="1" transform="translate(36,10)" zIndex="19" stroke="#B0B0B0"></rect>'
//                    //                + '<rect id="printRect" rx="3" ry="3" fill="#f4f4f2" x="0.5" y="0.5" width="23" height="19" stroke-width="1" transform="translate(10,10)" zIndex="19" stroke="#B0B0B0"></rect>'
//                    //                + '<path id="exportPath" d="M 6 17 L 18 17 18 14 6 14 Z M 12 14 L 9 9 11 9 11 5 13 5 13 9 15 9 Z" fill="#A8BF77" transform="translate(36,10)" stroke="#A0A0A0" stroke-width="1" zIndex="20"></path>'
//                    //                + '<path id="printPath" d="M 6 14 L 18 14 18 9 6 9 Z M 9 9 L 9 5 15 5 15 9 Z M 9 14 L 7.5 17 16.5 17 15 14 Z" fill="#B5C9DF" transform="translate(10,10)" stroke="#A0A0A0" stroke-width="1" zIndex="20"></path>'
//                    //                + '<rect rx="0" ry="0" fill="rgb(255,255,255)" x="36" y="10" width="24" height="20" stroke-width="0.000001" id="exportP3" fill-opacity="0.001" title="导出" zIndex="21" style="cursor:pointer;"><title>导出</title></rect>'
//                    //                + '<rect rx="0" ry="0" fill="rgb(255,255,255)" x="10" y="10" width="24" height="20" stroke-width="0.000001" id="printP3" fill-opacity="0.001" title="打印" zIndex="21" style="cursor:pointer;"><title>打印</title></rect>'
//                    //                + '</svg>'

//                    var svg = '<div id="p_P3" style="position: absolute;right: 0px;width: 60px;height:30px;margin:10px 20px 0px 0px;">'
//                + '<div id="exportP3" title="导出" zIndex="21" style="float:right;width:24px;height:20px;cursor:pointer;"><img src="../../MapServer/images/chart/dloadmout.png" /></div>'
//                + '<div id="printP3"  title="打印" zIndex="21" style="float:right;margin-right:5px;width:24px;height:20px;cursor:pointer;"><img src="../../MapServer/images/chart/printmout.png" /></div>'
//                + '</div>';
//                    $('#' + property.renderTo).append(svg).append(title).append(piechart);    //将谷歌API生成的图片加入指定div中

//                    $("#exportP3").click(function () {
//                        var width = $("#" + property.renderTo).width();

//                        //                        addCookie("title", property.title);    //保存地图名称
//                        //                        addCookie("subtitle", property.subtitle);    //保存地图图片路径
//                        //                        addCookie("names", property.names.join(","));
//                        //                        addCookie("datas", property.datas.join(","));
//                        //                        addCookie("dataUnit", property.dataUnit);
//                        //                        addCookie("width", width);

//                        //                        var url = 'exportP3.aspx';
//                        //                        var w = window.open(url, "灾情分布图");

//                        modifyHidden("hidTitle", encodeURIComponent(property.title));
//                        modifyHidden("hidSubtitle", encodeURIComponent(property.subtitle));
//                        modifyHidden("hidNames", encodeURIComponent(property.names.join(",")));
//                        modifyHidden("hidDatas", encodeURIComponent(property.datas.join(",")));
//                        modifyHidden("hidDataUnit", encodeURIComponent(property.dataUnit));
//                        modifyHidden("hidWidth", encodeURIComponent(width));
//                        document.getElementById('btnExportP3').click();


//                    })

//                    $("#exportP3").mouseout(function () {
//                        //                    $("#exportRect").css({ "stroke": "#b0b0b0" });
//                        //                    $("#exportPath").css({ "fill": "#A8BF77", "stroke": "#A0A0A0" });
//                        $("#exportP3 img").attr("src", "../../MapServer/images/chart/dloadmout.png");
//                    })

//                    $("#exportP3").mouseover(function () {
//                        //                    $("#exportRect").css({ "stroke": "#909090" });
//                        //                    $("#exportPath").css({ "fill": "#768f3e", "stroke": "#4572a5" });
//                        $("#exportP3 img").attr("src", "../../MapServer/images/chart/dloadmover.png");
//                    })
//                    $("#printP3").click(function () {
//                        var newWindowObi = window.open();
//                        newWindowObi.document.write(document.getElementById('floatbox').innerHTML);
//                        newWindowObi.document.getElementById("p_P3").style.display = "none";
//                        newWindowObi.window.print();
//                        setTimeout(function () {
//                            newWindowObi.window.close();
//                        }, 100) //毫秒
//                    })
//                    $("#printP3").mouseout(function () {
//                        //                    $("#printRect").css({ "stroke": "#b0b0b0" });
//                        //                    $("#printPath").css({ "fill": "#A8BF77", "stroke": "#A0A0A0" });
//                        $("#printP3 img").attr("src", "../../MapServer/images/chart/printmout.png");
//                    })

//                    $("#printP3").mouseover(function () {
//                        //                    $("#printRect").css({ "stroke": "#909090" });
//                        //                    $("#printPath").css({ "fill": "#768f3e", "stroke": "#4572a5" });
//                        $("#printP3 img").attr("src", "../../MapServer/images/chart/printmover.png");
//                    })
//                }
//                else {           //导出时不显示工具
//                    $('#' + property.renderTo).append(title).append(piechart);    //将谷歌API生成的图片加入指定div中
//                }

//                //        $.ajax({
//                //            url: url,
//                //            success: function() {
//                //                var src = "../../MapServer/tempimage/" + property.imageName
//                //                var pie = $('<img alt="三维饼状图" />').attr("src", src);
//                //                $('#' + property.renderTo).append(pie);    //将谷歌API生成的图片加入指定div中
//                //            }
//                //        })

//            }
//        }

        ////显示三维饼图
        //var chartp3 = function(property) {
        //    if (property.renderTo) {
        //        var chd = "&chd=";
        //        var chl = "&chl=";
        //        for (var i = 0; i < property.data.length; i++) {
        //            chd += property.data[i][1] + ":";
        //            chl += "'" + property.data[i][0] + "'|";
        //        }
        //        chd = chd.substr(0, chd.length - 1);
        //        chl = chl.substr(0, chl.length - 1);
        //        var src = "http://chart.apis.google.com/chart?cht=p3&chs=700x400" + chd; //+ chl
        //        if (property.color) {
        //            var color = "&";
        //            for (var i = 0; i < property.color.length; i++) {
        //                color += property.color[i];
        //            }
        //            color = color.substr(0, color.length - 1);
        //            src += color;
        //        }
        //        var pie = $('<img alt="三维饼状图" />').attr(src, src);
        //        $('#' + property.renderTo).append(pie);    //将谷歌API生成的图片加入指定div中
        //    }
        //}
