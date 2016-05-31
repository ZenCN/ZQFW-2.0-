var chartpie = function (pie_data, type) {
    mapParams = getMapParams(3);
    var title = mapParams.disasterInfoTypes[type].name + "统计图";
    var unit = mapParams.disasterInfoTypes[type].unit;
    var fixed = mapParams.disasterInfoTypes[type].fixed;    //小数点位数
    chart_pie = new Highcharts.Chart({
        credits: {
            enabled: false
        },
        chart: {
            renderTo: 'chartContiner',
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false
        },
        title: {
            text: title
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
                        return this.point.name + ': ' + Highcharts.numberFormat(this.y, fixed) + unit;
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
    });
}

    function GetChartData(chartdata, type) {
        var strdate2 = chartdata.split("!");
        var data = new Array();
        for (var j = 0; j < strdate2.length; j++) {
            data[j] = strdate2[j].split(",");
            data[j][1] = parseFloat(data[j][1]);
        }
        var pie_data = data;
        chartpie(pie_data, type);
    }



function SetChartValues(pageNO, type) {
    $.ajax({
        type: 'post',
        data: { pageNO: pageNO, type: type },
        url: '/Statistics/GetPieChartData',
        success: function (data) {
            if (data.length > 0) {
                chartpie(data, type);
            }
        }
    });
}