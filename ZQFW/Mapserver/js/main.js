$(document).ready(function () {
    bbAddData();
    $(function () {
        $("#exportMap").click(function () {
            $("#Form2").submit();
        })
    });
    $('#side_bar_tab li').click(function () {
        $('#side_bar_tab li').removeClass('selected');
        $('.side_list').removeClass('selected');
        $('#side_bar_lists li').removeClass('selected');
        $(this).addClass('selected');
        $('#' + $(this).attr('index')).addClass('selected');
        $('#' + $(this).attr('index') + ' .listli:first').addClass('selected');
        bbAddData();
    });
    $('.listli').click(function () {
        $('.listli').removeClass('selected');
        $(this).addClass('selected');
        if (ruler1) {
            refreshMap(ruler1.getSelected().pageno, $('.listli.selected').attr('type'), getMapType());  //刷新地图
        } else {
            refreshMap("", $('.listli.selected').attr('type'), getMapType());
        }
    });
    $("._year").change(function () {
        showmap(getSideType(), getCycType(), getYear(), getMapType());
    })
    $("#BBType").change(function () {
        fillYearSelect(getCycType())
        showmap(getSideType(), getCycType(), getYear(), getMapType())
    });
    $('a[rel*=facebox]').facebox('my-groovy-style');

    $mapTypes = $("#divMapType span[title]")
    $mapTypes.mouseover(function () {
        $(this).children().css({ 'border-color': '#83A1FF' });
        $(this).children().children().first().css({ 'background-color': '#83A1FF', 'opacity': 1 })
    })

    $mapTypes.mouseout(function () {
        $(this).children().css({ 'border-color': 'gray' });
        $(this).children().children().first().css({ 'background-color': 'gray', 'opacity': 0.5 })
    })

    $("#divMapType").mouseenter(function () {
        $(this).width(110);
        var mapType = getMapType();
        for (var i = 0; i < $mapTypes.length; i++) {
            if (i != mapType) {
                if (i < mapType) {
                    var right = i * 58;
                    $mapTypes.eq(i).animate({ "right": right });
                }
                else {
                    var right = (i - 1) * 58;
                    $mapTypes.eq(i).animate({ "right": right });
                }
            }
        }
    })

    $("#divMapType").mouseleave(function () {
        $(this).width(49);
        for (var i = 0; i < $mapTypes.length; i++) {
            $mapTypes.eq(i).animate({ "right": "0px" })
        }
    })

    $mapTypes.click(function () {
        var curMapType = getMapType();
        newMapType = $mapTypes.index(this);

        //        if (mapType > 0) {
        //            var right = 4 + (mapType - 1) * 58
        //            $mapTypes.eq(mapType).css({ "right": right });
        //        }
        $mapTypes.eq(newMapType).hide();
        $mapTypes.eq(curMapType).show();
        for (var i = 0; i < $mapTypes.length; i++) {
            if (i != newMapType) {
                if (i < newMapType) {
                    var right = i * 58;
                    $mapTypes.eq(i).css({ "right": right });
                }
                else {
                    var right = (i - 1) * 58;
                    $mapTypes.eq(i).css({ "right": right });
                }
            }
        }
        setMapType(newMapType);
        mapParams = getMapParams(newMapType);
        initializingMap(ruler1.getSelected().pageno, $('.listli.selected').attr('type'), newMapType);  //初始化地图
        //        mapInitialized = false; //设置地图是否初始化为否
        //        showmap(getSideType(), getCycType(), getYear(), newMapType);
    })
    //    $('.year_text span').html(getYear() + '年');
});

//获取当前选择的时段类型
var getCycType = function() {
    return $("#BBType").find('option:selected').val();
}
//获取当前选择的年份
var getYear = function() {
    return $("._year").val();
}
//获取当前选择灾害类型（1为洪涝,2为抗旱）
var getSideType = function() {
    return 1
    //         return  $('#side_bar_tab .selected').attr('index')
}
//获取当前地图类型
var getMapType = function () {
    return 0;
    //return $("#divMapType span[title]").index($("#divMapType span[title].select"));
}
//设置当前地图类型
function setMapType(mapType) {
    $("#divMapType span[title]").removeClass("select")
    $("#divMapType span[title]").eq(mapType).addClass("select");
}

var legend1 = null;
function bbAddData() {
    mapInitialized = false; //设置地图是否初始化为否
    var mapType = 0;   //默认地图类型为直观图
    //setMapType(mapType);
    mapParams = getMapParams(mapType);
    $.ajax({
        type:"post",
        url: 'GetCycTypes',
        beforeSend: function () {
        },
        error: function (e) {
        },
        success: function (cycTypes) {
            var bbTypeObj = $("#BBType").empty();
            for (var i = 0; i < cycTypes.length; i++) {
                var cycType = cycTypes[i];
                
                var options = "<option value='" + cycType.CycType + "'>" + cycType.Remark + "</option>";
                bbTypeObj.append(options);
            }
            $("#BBType").val("6");
            fillYearSelect(getCycType())
            showmap(getSideType(), getCycType(), getYear(), mapType);
        }
    });
};

//获取时间段
var getPeriod = function () {
    var year = getYear() + "年";
    var selectedRuler = ruler1.getSelected()
    var date = selectedRuler.startmonth + '月' + selectedRuler.startday + '日-' + selectedRuler.endmonth + '月' + selectedRuler.endday + '日';
    var period = year + date;
    return period;
}

function fillYearSelect(cycType) {
    $.ajax({
        type: 'post',
        data: { cycTypes: cycType },
        url: '/Statistics/GetYears',
        success: function (years) {
            if (years.length >0) {
                var yearSelect = $("#yearSelect");
                yearSelect.html("");
                for (var i = 0; i < years.length; i++) {
                    var options = "<option value='" + years[i] + "'>" + years[i] + "</option>";
                    yearSelect.append(options);
                }
            }
            else {
                $("#yearSelect").html("")
            }
        },
        error: function () {
            alert('获得数据出错');
        },
        async: false
    });

}

//生成统计图前检查数据
function checkData(evt, chartType) {
    var disasterInfoObj = getGlobalDisasterInfo();  //灾情数据数组
    if ($.isEmptyObject(disasterInfoObj)) {
        $(".chartbutton #leftbutton a").attr("href", "");
        stopDefault(evt);
        alert("当前无灾情数据!");
    }
    else {
        if (chartType == "table") {
            $(".chartbutton #rightbutton a").attr("href", "NewBB?type=table");
            return;
        }      
        var mapLevel = window.frames[mapParams.mapContainerName].getMapLevel();  //获取地图级别
        var disasterInfoType = $('.listli.selected').attr('type');
        var maxdata = 0;
        for (singleUnit in disasterInfoObj) {
            if (mapParams.mapType == 1 && mapLevel == 0 && getUnitDegree(singleUnit) == 1) {
                continue;
            }
            var singleUnitDisasterInfoArr = disasterInfoObj[singleUnit];
            var singleDisasterTypeData = singleUnitDisasterInfoArr[disasterInfoType];
            if (parseFloat(singleDisasterTypeData) > maxdata) {
                maxdata = parseFloat(singleDisasterTypeData)
            }
        }
        if (maxdata <= 0) {
            var dataName = mapParams.disasterInfoTypes[disasterInfoType].name;  //获取数据名称
            stopDefault(evt);
            alert("没有" + dataName + "!");
            return;
        }
        else {
            var element;
            var chartURL = "NewBB?type=" + chartType;

            if (chartType == "bar") {
                elment = $(".chartbutton #leftbutton a")
            }
            else if (chartType == "pie") {
                elment = $(".chartbutton #midl_button a")
            }
            else {
                elment = $(".chartbutton #midr_button a")
            }
            elment.attr("href", chartURL);
        }
    }
}

//显示地图渲染数据表格
function showInfoTable() {
    if ($("#infoTable").length > 0) {
        var htmlstr = '';
        if (ruler1) {
            var disasterInfoType = $('.listli.selected').attr('type');
            var disasterInfo = getGlobalDisasterInfo()  //获取灾情信息
            if (disasterInfo != "") {
                var dataName = mapParams.disasterInfoTypes[disasterInfoType].name;  //获取数据名称
                var dataUnit = mapParams.disasterInfoTypes[disasterInfoType].unit;   //获取数据单位
                var timer = ruler1.getSelected()
                var time = timer.startyear + "年" + timer.startmonth + "月" + timer.startday + "日" + "-" +
    timer.endyear + "年" + timer.endmonth + "月" + timer.endday + "日"
                var sorceType = "(" + timer.sorcetype + ")"

                var tableInfo = getInfoTableData(disasterInfo, disasterInfoType)  //获取单位名称及对应灾情数据
                htmlstr += "<table cellspacing='0'><thead><tr><td colspan='2'>" + dataName + "(" + dataUnit + ")" + "</td></tr>"
               + "<tr><td colspan='2'>" + time + "<br/>" + sorceType + "</td></tr></thead><tbody>";
                //创建灾害类型数据表
                for (var i = 0; i < tableInfo.length; i++) {
                    htmlstr += "<tr " + (i == tableInfo.length - 1 ? "class='last'" : "") + "><th><span>" + tableInfo[i][0] + "</span></th><td>" + tableInfo[i][1] + "</td></tr>";
                }
                htmlstr += "</tbody></table>";
            }

        }
        $('#infoTable').html(htmlstr);
    }
}