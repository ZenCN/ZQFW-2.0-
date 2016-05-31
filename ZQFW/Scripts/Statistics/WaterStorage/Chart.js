/*-------------------------------------
#Description:      蓄水柱状图
#Version:          1.0
#Author:           吴博怀
#Recent:           2014-07-09
-------------------------------------*/

$(document).ready(function() {
    createBarChart();
    createPieChart();
    createMap();
});

//将图表保存到服务器
var saveChart = function () {
    $scope.Report.MapPath = uploadMap(); 
    var barSvg = bar_chart.getSVG();
    $scope.Report.BarChartPath = uploadChart(barSvg);
    var pieSvg = pie_chart.getSVG();
    $scope.Report.PieChartPath = uploadChart(pieSvg);
}

//上传图表，以图片的方式存在服务器端
var uploadChart = function (svg) {
    var chartName = "chart";
    $.ajax({
        type: "post",
        data: { svg: svg },
        url: "/Hightcharts/saveImgFromHightcharts",
        success: function (str) {
            chartName = str;
        },
        async: false
    });
    return chartName;
}

//上传地图，以图片的方式存在服务器端
function uploadMap() {
    var frameMap = window.frames[mapParams.mapContainerName];
    var imgName = '各市州蓄水情况评价';//$scope.Report.Date + 
    var imgUrl = frameMap.getImgUrl();  //获取当前地图图片路径
    var legend = frameMap.getLegend();  //获取当前图例元素
    var mapWidth = getMapWidth();  //获取地图容器宽度
    var mapHeight = getMapHeight();  //获取地图容器高度
    var chartName = "map";
    $.ajax({
        type: "post",
        data: { hidImgName: encodeURIComponent(imgName),
            hidImgUrl: encodeURIComponent(imgUrl),
            hidLegend: encodeURIComponent(legend),
            hidMapWidth: encodeURIComponent(mapWidth),
            hidMapHeight: encodeURIComponent(mapHeight),
            mapType:4
          },
        url: "/Statistics/newMapThread",
        success: function (str) {
            chartName = str;
        },
        async: false
    });
    return chartName;
}

//创建柱状图
var createBarChart = function () {
    var hp011 = window.opener.$scope.Report.Open.Current.HP011;
    var objLNTQXSL = window.opener.$scope.BaseData.Reservoir.LNTQXSL;
    var objSQXSL = window.opener.$scope.BaseData.Reservoir.SQXSL;
    var units = [];
    var barData = [{ name: '本期蓄水量', data: [] }, { name: '历史同期蓄水量', data: [] }, { name: '上期蓄水量', data: []}];
    var maxValue = 0;
    $.each(hp011, function (i) {
        if (hp011[i].DW != "合计" && hp011[i].DW != "121") {
            units.push(hp011[i].DW);
            var unitCode = hp011[i].UnitCode;
            var BQXLL = $scope.Fn.Number(hp011[i].XXSLZJ);  //本期蓄水量
            barData[0].data.push(BQXLL);
            var LNTQXSL = $scope.Fn.Number(objLNTQXSL[unitCode]);  //历史同期蓄水量
            barData[1].data.push(LNTQXSL);
            var SQXSL = $scope.Fn.Number(objSQXSL[unitCode]);  //上期蓄水量
            barData[2].data.push(SQXSL);
            var max = Math.max(Math.max(BQXLL, LNTQXSL), SQXSL);
            maxValue = Math.max(maxValue, max);
        }
    })
    var title ='分市州工程蓄水情况'; //$scope.Report.Date + 
    //    var units = ['长沙', '株洲', '湘潭', '衡阳', '邵阳', '岳阳', '郴州', '张家界', '娄底', '湘西', '怀化', '常德'];
    //    var BQXSL = [49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4];
    //    var LNTQXSL = [83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3];
    //    var SQXSL = [48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2];
    bar_chart = new Highcharts.Chart({
        chart: {
//          backgroundColor: '#F8F8F8',
            type: 'column',
            renderTo: 'BarChart'
        },
        title: {
            text: title,
            style: {
                fontSize: '18px',
                color: '#000'
            }
        },
        subtitle: {
            text: '单位： 亿立方米',
            style: {
                fontSize: '13px',
                color: '#000'
            }
        },
        xAxis: {
            categories: units,
            labels: {
                rotation: -45,
                align: 'right',
                x: 10,
                style: {
                    fontFamily: 'Verdana, sans-serif', //'宋体'
                    color: '#000'
                    //writingMode: 'tb-rl'    //文字竖排样式
                    //	                fontWeight: 'bold'
                }
            }
        },
        yAxis: {
            min: 0,
            max: 20,
            title: {
                text: '蓄水量 (亿立方米)',
                style: {
                    fontSize: '13px',
                    color:'#000'
                }
            },
            tickInterval: 5
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.2f} 亿立方米</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        plotOptions: {
            column: {
                groupPadding: 0.1,
                //                pointPadding: 0.2,
                borderWidth: 0,
                pointWidth: 9
            }
        },
        series: barData,
        credits: {
            enabled: false // 禁用版权信息
        },
        exporting: {
            enabled: false // 禁用导出按钮
        }
    });
}

//创建饼状图
var createPieChart = function () {
    var title = '各类水利工程蓄水占比'; //$scope.Report.Date + 
    var hp011 = window.opener.$scope.Report.Open.Current.HP011[0];  //蓄水表合计行
    var DXSKXSL = $scope.Fn.Number(hp011.DZKXXSL);   //大型水库蓄水量
    var ZXSKXSL = $scope.Fn.Number(hp011.ZZKXXSL);   //中型水库蓄水量
    var XXSKXSL = $scope.Fn.Number(hp011.XYKXXS) + $scope.Fn.Number(hp011.XRKXXS);   //小型水库蓄水量
    var SPTXSL = $scope.Fn.Number(hp011.SPTXXS);   //山坪塘蓄水量
    var pieData = [{ name: '大型水库', y: DXSKXSL, sliced: true },
                    { name: '中型水库', y: ZXSKXSL, sliced: true },
                    { name: '小型水库', y: XXSKXSL, sliced: true },
                    { name: '山坪塘', y: SPTXSL, sliced: true}];
    pie_chart = new Highcharts.Chart({
        chart: {
            //            backgroundColor: '#F8F8F8',
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0
            },
            //            plotBackgroundColor: null,
            //            plotBorderWidth: null,
            //            plotShadow: false,
            renderTo: 'PieChart'

        },
        title: {
            text: title,
            style: {
                fontSize: '18px',
                color: '#000'
            }
        },
        subtitle: {
            text: '单位： 亿立方米',
            style: {
                fontSize: '13px',
                color: '#000'
            }
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.y:.2f}亿立方米</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                borderWidth: 10,
                depth: 35,
                dataLabels: {
                    enabled: true,
                    //                    connectorColor:'#555555',
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
        series: [{
            type: 'pie',
            name: '蓄水量',
            data: pieData
        }],
        credits: {
            enabled: false // 禁用版权信息
        },
        exporting: {
            enabled: false // 禁用导出按钮
        }
    });
}

//加载地图
var createMap = function () {
    var hp011 = window.opener.$scope.Report.Open.Current.HP011;
    var disasterInfoValues = [];
    $.each(hp011, function (i) {
        if (hp011[i].DW != "合计") {
            var data = [];
            var XZYBFB = $scope.Fn.Number(hp011[i].XZYBFB);  //实际蓄水量占计划比例
            data.push(hp011[i].UnitCode);
            data.push(XZYBFB);
            disasterInfoValues.push(data);
        }
    })

    mapParams = getMapParams(0);
    var breakValues = Obj2Str([-1,40, 60, 80]);
    //var colors = "[ '#FFFF00','#FF7878', '#FFAA40', '#B4E1FC', '#B3FFD9']"; // ,'#00FFFF'
    var colors = "[ '#FFFF00','#FF7878', '#ffbc75', '#95ceff', '#a9ff96']";
    var cxmap = getMapWidth();
    var cymap = getMapHeight() + 20;   //加20像素覆盖掉地图下面的图标; + 20
    //manifold地图链接
    var url = mapParams.mapControlSrc;                //."../../MapServer/mapControl.asp"
    url += "?command=" + encodeURIComponent("startup")
    url += "&disasterInfoType=" + encodeURIComponent("")
    url += "&level=" + encodeURIComponent(0)
    url += "&regionCode=" + encodeURIComponent("43000000")
    url += "&colors=" + encodeURIComponent(colors)
    url += "&state=" + encodeURIComponent("")
    url += "&disasterInfoValues=" + encodeURIComponent(Obj2Str(disasterInfoValues));
    url += "&breakValues=" + encodeURIComponent(breakValues);
    url += "&boxSizeX=" + encodeURIComponent(cxmap);
    url += "&boxSizeY=" + encodeURIComponent(cymap);
    url += "&mapType=" + encodeURIComponent(0);
    //locateElement(mapParams.mapContainerName).src = url;
    $('#map').attr('src', url);
}

//获取地图容器宽度
function getMapWidth() {
    return $('#' + mapParams.mapContainerName).width();
}

//获取地图容器高度
function getMapHeight() {
    return $('#' + mapParams.mapContainerName).height();
}
