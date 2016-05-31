
var infoDetail = "";
var createChart = function(type) {
    var regionCode = obtainHidden("regionCode");  //单位代码
    var regionName = obtainHidden("regionName")    //单位名称
    var pageNO = obtainHidden("pageNO");  //页号
    var disasterInfoType = obtainHidden("disasterInfoType");  //灾情信息类型
    var disasterInfo = window.parent.window.getGlobalDisasterInfo();         //获取灾情数据字符串

    var dataName = window.parent.window.mapParams.disasterInfoTypes[disasterInfoType].name;  //获取数据名称
    var dataUnit = window.parent.window.mapParams.disasterInfoTypes[disasterInfoType].unit;   //获取数据单位
    if (disasterInfo == "") {
        alert("当前区域无灾情数据");
        return;
    }
    var disasterInfoObj = eval("(" + disasterInfo + ")");          //灾情数据数组

    if (type == "bar") {
        var bar_name = [];
        var bar_data = [];
        for (singleUnit in disasterInfoObj) {
            var singleUnitDisasterInfoArr = disasterInfoObj[singleUnit];
            bar_name.push(singleUnitDisasterInfoArr[window.parent.window.mapParams.unitNameIndex]);

            //向数组添加对应类型的灾害值
            var dataIndex = window.parent.window.mapParams.disasterInfoTypes[disasterInfoType].index;  //获取数据索引
            var singleDisasterTypeData = singleUnitDisasterInfoArr[dataIndex];
            bar_data.push(parseFloat(singleDisasterTypeData))
        }
        calChartPos("bar");
        showchartbar(regionName, dataName, dataUnit, pageNO, bar_name, bar_data);

    }
    else if (type == "pie") {
        var pie_data = [];
        for (singleUnit in disasterInfoObj) {
            var singleUnitDisasterInfoArr = disasterInfoObj[singleUnit];
            var singleDisasterTypeDataArr = [];
            singleDisasterTypeDataArr.push(singleUnitDisasterInfoArr[window.parent.window.mapParams.unitNameIndex])

            //向数组添加对应类型的灾害值
            var dataIndex = window.parent.window.mapParams.disasterInfoTypes[disasterInfoType].index;  //获取数据索引
            var singleDisasterTypeData = singleUnitDisasterInfoArr[dataIndex];
            singleDisasterTypeDataArr.push(parseFloat(singleDisasterTypeData));
            pie_data.push(singleDisasterTypeDataArr)
        }
        calChartPos("pie");
        showchartpie(regionName, dataName, dataUnit, pageNO, pie_data);
    }
    else if (type == "table") {
        chartHide();
    }
}

function chartHide() {
    myInfoWindowVisible = true
    $("div#divChart").fadeOut("slow").animate({ "height": "0px", "width": "0px" }).html("");
    $(".shield").fadeOut("slow").animate({ "height": "0px", "width": "0px" });
    infoDetail = "";
}

function calChartPos(type) {
    myInfoWindowVisible = false
    infoDetail = type;
    $("div#divChart").html("");
    var width = 600;
    var height = 400;
    if (type == "bar") {
        height = 300;
    }
    else if (type == "pie") {
    }
    else if (type == "picture") {
        height = 400;
    }
    else if (type == "video") {
    var width = 700;
    }
    var left = ($("input#map").width() < width) ? 0 : ($("input#map").width() - width) / 2;
    var top = ($("input#map").height() < height) ? 0 : ($("input#map").height() - height) / 2;
    if (type == "video") {
        $("div#divChart").animate({ "width": width, "height": height, "top": top, "left": left }).fadeIn("slow");
    }
    else {
        $("div#divChart").css({ "width": width, "height": height, "top": top, "left": left }).fadeIn("slow");
    }
    $(".shield").animate({ "height": $("input#map").height(), "width": $("input#map").width() }).fadeIn("slow");
}

//柱状图
var showchartbar = function(regionName, dataName, dateUnit, timer, bar_name, bar_data) {
    var chart;
    chart = new Highcharts.Chart({
        chart: {
        renderTo: 'divChart', //柱状图的容器名称
            type: 'column'
        },
        title: {
        text: regionName + dataName + '统计图' //柱状图标题 + "(页号" + timer + ")" 
        },
        xAxis: {//x轴上的单位名称
        categories: bar_name,
            labels: {
                rotation: -45,
                align: 'right',
                style: {
                fontSize: '13px'
//                    ,
//                    fontFamily: 'Verdana, sans-serif'
                }
            }
        },
        yAxis: {
            min: 0,
            title: {
            text: dataName + ' (' + dateUnit + ')'//x轴上的标题名称
            }
        },
        legend: {
            enabled: false
        },
        tooltip: {
            formatter: function() {
                return '<b>' + this.x + '</b>: ' + Highcharts.numberFormat(this.y, 1) + dateUnit;
            }
        },
        series: [{
        name: dataName,
        data: bar_data, //x轴上的单位名称所对应数据
            dataLabels: {
                enabled: true,
                rotation: -90,
                color: '#111',
                align: 'bottom',
                style: {
                fontSize: '13px'
//                    ,
//                    fontFamily: 'Verdana, sans-serif'
                }
            }
        }]
    });
};  
    


//饼状图
var showchartpie = function(regionName, dataName, dateUnit, timer, pie_data) {
    var chart;
    chart = new Highcharts.Chart({
        chart: {
            renderTo: 'divChart',
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false
        },
        title: {
            text: regionName + dataName + '统计图'  //饼状图标题 + "(页号" + timer + ")" 
        },
        tooltip: {
            formatter: function() {
                return '<b>' + this.point.name + '</b>: ' + Highcharts.numberFormat(this.percentage, 2) + ' %';
            }

            //            pointFormat: '{series.name}: <b>{point.percentage}%</b>',
            //            percentageDecimals: 1
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    color: '#000000',
                    connectorColor: '#000000',
                    formatter: function() {
                        return this.point.name + ': ' + Highcharts.numberFormat(this.y, 2) + dateUnit;
                    }
                }
            }
        },
        series: [{
            type: 'pie',
            name: dataName,
            data: pie_data
            /*[
            ['Firefox', 45.0],
            ['IE', 26.8],
            {
            name: 'Chrome',
            y: 12.8,
            sliced: true,
            selected: true
            },
            ['Safari', 8.5],
            ['Opera', 6.2],
            ['Others', 0.7]
            ]*/
}]
        });
    };
	