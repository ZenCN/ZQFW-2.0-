//***********************全局参数******************************
var ChartSettings = //全局参数
{
ReportType: { Type: "1", Name: "洪涝" },
BeginYear: '',
EndYear: '',
Period: {},  //灾害起始时间
CycTypes: [],
PageNos: [],
Fields: { Field: "SZRK", FieldName: "受灾人口", PerField: undefined },
Units: [],
Chart: null
}

//***********************模块原型类******************************

//年份原型
function getYears(cycTypes) {
    $.ajax({
        type: 'post',
        data:{cycTypes:cycTypes},
        url: '/Statistics/GetYears',
        async: false,
        success: function (beginYear) {
             qYear = beginYear;          
        }
     })
     return qYear;
}
 //载入时，日期 ，以及首次获取报表参数
    function dateinit(){     
        var datenow = mynewdate(99);
        document.getElementById('mxstartdate').value=datenow.mxsttime;
        document.getElementById('mxenddate').value=datenow.mxedtime;        
        var stime = new Date($('#mxstartdate').val());
        var edtime = new Date($('#mxenddate').val());
        ChartSettings.Period = { BeginMonth: stime.getMonth()+1, BeginDay: stime.getDate(), BDateRange: 0, EndMonth: edtime.getMonth()+1, EndDay: edtime.getDate(), EDateRange: 0 }; 
        ChartSettings.BeginYear =stime.getFullYear();
        ChartSettings.EndYear=edtime.getFullYear();     
    }
    //开始时间改变
    function stdatechange(){  
        var datenow =  $dp.cal.newdate;
        var stdate = new Date("'"+datenow.y+"-"+datenow.M+"-"+datenow.d+"'");
        var eddate = new Date($('#mxenddate').val());
        ChartSettings.PageNos = [];
        ChartSettings.Period = { BeginMonth: datenow.M, BeginDay: datenow.d, BDateRange: 0, EndMonth: eddate.getMonth()+1, EndDay: eddate.getDate(), EDateRange: 0 }; 
        ChartSettings.BeginYear = datenow.y;
        ChartSettings.EndYear=eddate.getFullYear();
        if(stdate>eddate){
            ChartSettings.Period = { BeginMonth: datenow.M, BeginDay: datenow.d, BDateRange: 0, EndMonth: datenow.M, EndDay: datenow.d, EDateRange: 0 }; 
            ChartSettings.BeginYear = datenow.y;
            ChartSettings.EndYear=datenow.y;
            document.getElementById('mxenddate').value=datenow.y+"-"+datenow.M+"-"+datenow.d;
        }
        GetReportData();
    }
    //结束时间改变
    function eddatechange(){  
        var datenow =  $dp.cal.date;
        var eddate = new Date("'"+datenow.y+"-"+datenow.M+"-"+datenow.d+"'");
        var stdate = new Date($('#mxstartdate').val());
        ChartSettings.PageNos = [];
        ChartSettings.Period = { BeginMonth: stdate.getMonth()+1, BeginDay: stdate.getDate(), BDateRange: 0, EndMonth: datenow.M, EndDay: datenow.d, EDateRange: 0 }; 
        ChartSettings.BeginYear =stdate.getFullYear();
        ChartSettings.EndYear=datenow.y;
        if(eddate<stdate){
            ChartSettings.Period = { BeginMonth: datenow.M, BeginDay: datenow.d, BDateRange: 0, EndMonth: datenow.M, EndDay: datenow.d, EDateRange: 0 }; 
            ChartSettings.BeginYear = datenow.y;
            ChartSettings.EndYear=datenow.y;
            document.getElementById('mxstartdate').value=datenow.y+"-"+datenow.M+"-"+datenow.d;
        }
        GetReportData();
    }
    //获取当前月份的天数
function mynewdate (ca){    
     var cycTypes = getAllCycTypes();
     var Year = getYears(cycTypes);
     var mdate = new Date();     
     var styear =0;
     var edyear =0;
     var stmonth =0;
     var edmonth =0;
     var stday =0;
     var edday =0;
     if(Year.length==0){
        styear =mdate.getFullYear();
        edyear =mdate.getFullYear();
        stmonth=1;
        edmonth=12;
        stday=1;
        edday=31;
     }else{
        styear = Math.min.apply(null,Year);
        edyear = Math.max.apply(null,Year);
        stmonth=1;
        edmonth=12;
        stday=1;
        edday=31;
     }
     if(ca==99){
        styear =edyear;
        stmonth=mdate.getMonth()+1;
        edmonth=mdate.getMonth()+1;
        stday=1;        
        edday=getDays(edmonth);
     }
     var mxdate = {
     mxsttime:styear+'-'+stmonth+'-'+stday ,
     mxedtime:edyear+'-'+edmonth+'-'+edday    
     }
     return mxdate;
}
//var YearMachine = function (cycTypes, ACallBack) {
//    $('.mxselclasss #beginyear').empty();
//    $('.mxselclasss #endyear').empty();
//    var Year = getYears(cycTypes);
//    if (Year.length == 0) {
//        var myDate = new Date();
//        Year[0] = myDate.getFullYear();
//    }
//    for (var i = 0; i < Year.length; i++) {
//        var options = '<option value=' + Year[i] + '>' + Year[i] + '年</option>';
//        $('.mxselclasss #beginyear').append(options);
//        $('.mxselclasss #endyear').append(options);
//    }
//    if ($.isFunction(ACallBack)) {
//        var BeginDate = Year[0].toString();
//        var EndData = Year[0].toString();
//        $('.mxselclasss #beginyear').change(function () {
//            var Year = $(this).find('option:selected').attr('value');
//            var YearName = $(this).find('option:selected').text();
//            var edyear = $('.mxselclasss #endyear').find('option:selected').attr('value'); 
//            document.getElementById("starttimeshake").style.visibility="hidden";         
//            if(Year> edyear){         
//            document.getElementById("starttimeshake").style.visibility="visible"; 
//            setTimeout('document.getElementById("starttimeshake").style.visibility="hidden"',800); 
//            document.getElementById('endyear').value=Year;
//            var year1 = $('.mxselclasss #endyear').find('option:selected').attr('value');
//            var year2 = $('.mxselclasss #endyear').find('option:selected').text();
//            ACallBack(year1, year2, "EndData");
//            $("#endmonth").val($(this).val()).change();             
//            }
//            ACallBack(Year, YearName, "BeginDate");
//        })
//        $('.mxselclasss #endyear').change(function () {
//            var Year = $(this).find('option:selected').attr('value');
//            var YearName = $(this).find('option:selected').text();
//             var star_year=$('.mxselclasss #beginyear').find('option:selected').attr('value');
//             document.getElementById("endtimeshake").style.visibility="hidden";  
//            if(Year < star_year){
//            document.getElementById("endtimeshake").style.visibility="visible";  
//            setTimeout('document.getElementById("endtimeshake").style.visibility="hidden"',800); 
//            document.getElementById('beginyear').value=Year;
//            var year1 = $('.mxselclasss #beginyear').find('option:selected').attr('value');
//            var year2 = $('.mxselclasss #beginyear').find('option:selected').text();
//            ACallBack(year1, year2, "BeginDate");
//            }
//            ACallBack(Year, YearName, "EndData");
//        })
//        return { BeginYear: BeginDate, EndYear: EndData };
//    }
//    
//}

////日期原型
//var DateMaker = function (BeginYear, EndYear, ACallBack) {
//    for (var i = 1; i <= 12; i++) {
//        var option = '<option value=' + i + '>' + i + '月</option>';
//        $("#beginmonth").append(option);
//        $("#endmonth").append(option);
//    }

//    for (var i = 1; i <= 31; i++) {
//        var option = '<option value=' + i + '>' + i + '日</option>';
//        $("#endday").append(option);
//        $("#beginday").append(option);
//    }
////     var options = '<option value=0 selected>0</option>'
////                    + '<option value=0>前后一天</option>'
////                    + '<option value=3 selected>前后三天</option>'
////                    + '<option value=7>前后一周</option>';
////    var options = '<option value=0 selected>0</option>';
////    $(".daterange").append(options);

//    if ($.isFunction(ACallBack)) {
//        $(".mxselclasss .select").change(function () {
//            //如果改变的是开始月份
//            if (this.id == "beginmonth") {
//                var days = getDays($(this).val());
//                var day = parseInt($("#beginday").val());
//                $("#beginday").html("");
//                alert($(".mxselclasss #beginmonth").val());
//                for (var i = 1; i <= days; i++) {
//                    var option = '<option value=' + i + '>' + i + '日</option>';
//                    $("#beginday").append(option);
//                }
//                if (day > days) {
//                    day = days;
//                }
//                var bgmonth = parseInt($(".mxselclasss #beginmonth").val());
//                var edmonth = parseInt($(".mxselclasss #endmonth").val());
//                var bgyear = parseInt($('.mxselclasss #beginyear').find('option:selected').attr('value'));
//                var edyear = parseInt($('.mxselclasss #endyear').find('option:selected').attr('value'));
//                if(bgyear==edyear&&(bgmonth>edmonth))
//                {                    
//                    $("#endmonth").val($(this).val()).change();
//                }
//            }
//            //如果改变的是结束月份
//            if (this.id == "endmonth") {
//                var days = getDays($(this).val());
//                var day = parseInt($("#endday").val());
//                $("#endday").html("");
//                for (var i = 1; i <= days; i++) {
//                    var option = '<option value=' + i + '>' + i + '日</option>';
//                    $("#endday").append(option);
//                }
//                if (day > days) {
//                    day = days;
//                }
//                if(($('.mxselclasss #beginyear').find('option:selected').attr('value')==$('.mxselclasss #endyear').find('option:selected').attr('value'))&&($(".mxselclasss #beginmonth").val()>$(".mxselclasss #endmonth").val()))
//                {      
//                     document.getElementById('beginmonth').value=parseInt(document.getElementById('endmonth').value);      
//                    $("#beginmonth").val($(this).val()).change();
//                }
//            }
//            var beginMonth = $("#beginmonth").val();    //起始月份
//            var beginDay = $("#beginday").val();        //起始日期
//            var bDateRange =0;    //起始时间范围 var bDateRange = $("#bdaterange").val(); 
//            var endMonth = $("#endmonth").val();        //结束月份
//            var endDay = $("#endday").val();            //结束日期
//            var eDaterRange = 0;    //结束时间范围   var eDaterRange = $("#edaterange").val();   
//            ACallBack(beginMonth, beginDay, bDateRange, endMonth, endDay, eDaterRange);
//        });
//    }

//    var max_op = document.getElementById("endday").options.length - 1; //减掉1才能获得最后一个选项,因为序号是从0开始的
//    document.getElementById("endday").options[max_op].selected = true;

//    return {
//        BeginMonth: $("#beginmonth").val(),    //起始月份
//        BeginDay: $("#beginday").val(),        //起始日期
//        BDateRange: 0,    //起始时间范围 $("#bdaterange").val()
//        EndMonth: $("#endmonth").val(),        //结束月份
//        EndDay: $("#endday").val(),            //结束日期
//        EDateRange: 0    //结束时间范围 $("#edaterange").val() 
//    };  //灾害起始时间
//}

//获取该月天数
function getDays(month) {
    month = parseInt(month);
    switch (month) {
        case 1:
        case 3:
        case 5:
        case 7:
        case 8:
        case 10:
        case 12:
            var days = 31;
            break;
        case 4:
        case 6:
        case 9:
        case 11:
            var days = 30;
            break;
        case 2:
            var days = 28;
            break;
        default:
            var days = 30;
            break;
    }
    return days;
}

//报表类型原型
var ReportType = function (ASettings) {
    var pSettings =
    {
        "Type": "1",
        "Name": "洪涝",
        "CycTypes": [{ "type": 3, "typeName": "月报"},
                    { "type": 4, "typeName": "累计报"},
                    { "type": 5, "typeName": "年终报"},
                    { "type": 6, "typeName": "过程报"}],         //月报3,累计报4,年终报5,过程报6
        "CallBack": null
    };
    $.extend(pSettings, ASettings);

    var cycTypes = [];
    var isDouble = false;
    for (var i = 0; i < pSettings.CycTypes.length; i++) {
        $("#cyctypelist").append("<li class='" + (isDouble ? "double" : "") + "'><input class='radio_select' id='" + pSettings.CycTypes[i].type + "' type='checkbox' checked=true /><span class='radio_title'>"
        + pSettings.CycTypes[i].typeName + "</span></li>");  //checked='checked'
        cycTypes.push(pSettings.CycTypes[i].type);
        isDouble = !isDouble;
    }

    if ($.isFunction(pSettings.CallBack)) {
        $("#cyctypelist :checkbox").change(function () {

            pSettings.CallBack(this.id, this.checked);
        });
    }
    return {
        ReportType: { Type: pSettings.Type, Name: pSettings.Name },
        CycTypes: cycTypes
    };
}

//行政单位原型
var UnitMaker = function (Units, ACallBack) {
    var isDouble = false;
    var SelectedUnits = [];
    Units = eval('(' + Units + ')');
    $('#chooseunit #unitslist').empty();
    for (var i = 0; i < Units.length; i++) {
        $('#chooseunit #unitslist').append("<li class='" + (isDouble ? "double" : "") + "'><input class='radio_select' unitcode='" + Units[i].DistrictCode + "' type='checkbox' name='type'/><span class='radio_title'>" + Units[i].DistrictName + "</span></li>");
        isDouble = !isDouble;
    }
    if ($.isFunction(ACallBack)) {
        $("#chooseunit #unitslist li input").change(function () {
            var UnitCode = $(this).attr("unitcode");
            var UnitName = $(this).find(".radio_title").text();
            ACallBack(UnitCode, UnitName, this.checked);
        });
    }
    return SelectedUnits;
}

//灾情数据类型
var FieldMaker = function (Fields, ACallBack) {
    var PerField = {
        symj: "symj",
        shmjxj: "shmjxj",
        czmjxj: "czmjxj",
        jsmjxj: "jsmjxj"
    };
    var isDouble = false;
    var SelectedField =
    {
        Field: Fields[0].value,
        FieldName: Fields[0].name,
        PerField: undefined
    };
    $('#choosetypes #typelist').empty();
    Fields = eval('(' + Fields + ')');
    for (var i = 0; i < Fields.length; i++) {

        $('#choosetypes #typelist').append("<li class='" + (isDouble ? "double" : "") + "'><input class='radio_select' typename='" + Fields[i].value + "' typecname='" + Fields[i].name + "' type='radio' name='type'" + (i < 0 ? "checked='checked'" : "") + "/><span class='radio_title'>" + Fields[i].name + "</span></li>");
        isDouble = !isDouble;
    }
    if ($.isFunction(ACallBack)) {
        $("#choosetypes #typelist li input").not(".perswitch input").change(function () {
            var Field = $(this).attr("typename");
            var FieldName = $(this).attr("typecname");
            var PerField = "qgq"; //$(this).parent().find(".perswitch input:checked").attr("typecname");
            ACallBack(Field, FieldName, PerField, this.checked);
        });
    }
    return SelectedField;
}

//报表原型
var ReportMaker = function (Reports, ACallBack) {
    $("#choosereport #reportlist").empty();
    for (var i = 0; i < Reports.length; i++) {
        var rt = eval('(' + Reports[i] + ')');
        for (var j = 0; j < rt.length; j++) {
            $("#choosereport #reportlist").append("<li cycType='" + rt[j].CycType + "'><input class='radio_select' pageno='" + rt[j].PageNO + "' type='checkbox' name='report'/><span class='radio_title'>  " + rt[j].CycTypeName + "  " + rt[j].Starttime + "  至  " + rt[j].Endtime + "</span></li>");
            // " + ChartSettings.CycType.CycTypeName + "
        }
    }
    upDateRptLists();   //根据时段类型显示报表清单
    if ($.isFunction(ACallBack)) {
        $("#choosereport #reportlist li input").change(function () {
            var PageNo = $(this).attr("pageno");
            ACallBack(PageNo, this.checked);
        });
    }
}
//var ChartList = function(Charts,ADelCallBack,AOpenCallBack)
//{
//    for(var i = 0;i < Charts.length ; i++)
//    {
//        $("#left_side ul").append("<li><img src='image/chart/left/chart_line.png'/><div class='_info'><span class='_title' title='" + title + "'>" + title + "</span><span class='_operation'><a class='open_' id='" + index + "' tid='" + tid + "'>查看</a></span><span class='_operation'><a class='del_' id='" + index + "' tid='" + tid + "' >删除</a></span></div></li>")
//    }
//    if($.isFunction(ADelCallBack))
//    {
//        $("#left_side ul li .del_").click(function(){
//            var Tid = $(this).attr("tid");
//            ADelCallBack(Tid);
//        })
//    }
//    if($.isFunction(AOpenCallBack))
//    {
//        $("#left_side ul li .open_").click(function(){
//            var Tid = $(this).attr("tid");
//            AOpenCallBack(Tid);
//        })
//    }
//}

//***********************事件类******************************
function CycTypeChange(CycType, isChecked)//时段类型改变事件
{
    if (isChecked) {
        ChartSettings.CycTypes.push(CycType);
    }
    else {
        ChartSettings.CycTypes = $.grep(ChartSettings.CycTypes, function (n) {
            return n != CycType;
        })
    }
    upDateRptLists();
//    ChartSettings.PageNos = [];
//    GetReportData();
}
//function YearChange(Year, YearName, YearType)//年份选择改变事件
//{
//    switch (YearType) {
//        case "BeginDate":
//            ChartSettings.BeginYear = Year;
//            break;
//        case "EndData":
//            ChartSettings.EndYear = Year;
//            break;
//    }
//    //ChartSettings.Period = new DateMaker(ChartSettings.BeginYear, ChartSettings.EndYear, DateChange);
//    ChartSettings.PageNos = [];
//    GetReportData();
//}

//日期选择事件
//function DateChange(beginMonth, beginDay, bDateRange, endMonth, endDay, eDateRange) {
//    ChartSettings.Period = { BeginMonth: beginMonth, BeginDay: beginDay, BDateRange: 0, EndMonth: endMonth, EndDay: endDay, EDateRange: 0 }; //BDateRange: bDateRange  EDateRange: eDateRange
//    ChartSettings.PageNos = [];
//    GetReportData();
//}
function UnitChange(UnitCode, UnitName, isChecked)//单位选择事件
{
    if (UnitCode == sUnitCode && isChecked) {
        $('#chooseunit #unitslist .radio_select').not("[unitcode=" + sUnitCode + "]").attr("checked", false).change();
    }
    else if (UnitCode != sUnitCode && isChecked) {
        $('#chooseunit #unitslist [unitcode=' + sUnitCode + ']').attr("checked", false).change();
    }
    if (isChecked) {
        if ($.inArray(UnitCode, ChartSettings.Units) < 0)
            ChartSettings.Units.push(UnitCode);
    }
    else {
        ChartSettings.Units = $.grep(ChartSettings.Units, function (n) {
            return n != UnitCode;
        })
    }
}
function FieldChange(Field,FieldName,PerField,isChecked)
{
    ChartSettings.Fields.Field = Field;
    ChartSettings.Fields.FieldName = FieldName;
    ChartSettings.Fields.PerField = PerField;   
}
function ReportChange(PageNO, isChecked) {
    if (isChecked) {
        if ($.inArray(PageNO, ChartSettings.PageNos) < 0)
            ChartSettings.PageNos.push(PageNO);
    }
    else {
        ChartSettings.PageNos = $.grep(ChartSettings.PageNos, function (n) {
            return n != PageNO;
        })
    }
}
function OpenChartClick(Tid)
{
    alert(Tid);
}
function DelChartClick(Tid)
{
    alert(Tid);
}
function SelectedAll(P)
{
    P.attr("checked",true).change();
}
function UnSelectedAll(P)
{
    P.attr("checked",false).change();
}
//***********************数据获取类******************************
function ArrayToQuery(Array)
{
    var QueryString = "";
    for(var i = 0;i < Array.length;i++){
        QueryString += Array[i];
        if(i < Array.length - 1){
            QueryString += ",";
        }
    }
    return QueryString;
}

function getAllCycTypes() {
    var cycTypes = "";
    var checkBoxs = $("#cyctypelist :checkbox")
    for (var i = 0; i < checkBoxs.length; i++) {
        if (i > 0) {
            cycTypes += ",";
        }
        cycTypes += checkBoxs[i].id;
    }
    return cycTypes;
}

var GetInitData = function () {
    rptType = new ReportType({
        Type: "1",
        Name: "洪涝",
        CallBack: CycTypeChange
    });
    ChartSettings.ReportType = rptType.ReportType;
    ChartSettings.CycTypes = rptType.CycTypes;
    var cycTypes = getAllCycTypes();
    dateinit();
//    var Years = new YearMachine(cycTypes,YearChange);
//    ChartSettings.BeginYear = Years.BeginYear;
//    ChartSettings.EndYear = Years.EndYear;
//    ChartSettings.Period = new DateMaker(ChartSettings.BeginYear, ChartSettings.EndYear, DateChange);

    $.ajax({
        url: "FirstDisasterAnalysis",
        type: "post",
        data: { beginYear: ChartSettings.BeginYear, endYear: ChartSettings.EndYear, beginMonth: ChartSettings.Period.BeginMonth, beginDay: ChartSettings.Period.BeginDay, bDateRange: 0, endMonth: ChartSettings.Period.EndMonth, endDay: ChartSettings.Period.EndDay, eDateRange:0, cycTypes: cycTypes, pageNumDataList: 0, pageLineNumDataList: 100, pageNumTrueNode: 0, pageLineNumTrueNode: 100 },
        // data: { beginYear: ChartSettings.BeginYear, endYear: ChartSettings.EndYear, beginMonth: ChartSettings.Period.BeginMonth, beginDay: ChartSettings.Period.BeginDay, bDateRange: ChartSettings.Period.BDateRange, endMonth: ChartSettings.Period.EndMonth, endDay: ChartSettings.Period.EndDay, eDateRange: ChartSettings.Period.EDateRange, cycTypes: cycTypes, pageNumDataList: 0, pageLineNumDataList: 100, pageNumTrueNode: 0, pageLineNumTrueNode: 100 },
        success: function (data) {
            //            data = eval("(" + data + ")");

            new ReportMaker(data.dataListJson, ReportChange);
//            new ChartList(data.trueNodeJson, DelChartClick, OpenChartClick);
            ChartSettings.Units = new UnitMaker(data.unitNameJson, UnitChange);
            ChartSettings.Fields = new FieldMaker(data.disasterTypeJson, FieldChange);
        },
        error: function () {
        }
    })
}

var GetReportData = function () {
    var cycTypes = getAllCycTypes();
    var tesaasd = ChartSettings;
    $.ajax({
        url: "DisasterAnalysisDataList",
        type: "post",
        data:{beginYear: ChartSettings.BeginYear, endYear: ChartSettings.EndYear, beginMonth: ChartSettings.Period.BeginMonth, beginDay: ChartSettings.Period.BeginDay, bDateRange: 0, endMonth: ChartSettings.Period.EndMonth, endDay: ChartSettings.Period.EndDay, eDateRange: 0, cycTypes: cycTypes, pageNum: 0, pageLineNum: 100},
        // data:{beginYear: ChartSettings.BeginYear, endYear: ChartSettings.EndYear, beginMonth: ChartSettings.Period.BeginMonth, beginDay: ChartSettings.Period.BeginDay, bDateRange: ChartSettings.Period.BDateRange, endMonth: ChartSettings.Period.EndMonth, endDay: ChartSettings.Period.EndDay, eDateRange: ChartSettings.Period.EDateRange, cycTypes: cycTypes, pageNum: 0, pageLineNum: 100},
        success: function (data) {
//            data = eval("(" + data + ")");
            if (data.length == 1) {
                ChartSettings.ReportLoaded = true;
            }
            new ReportMaker(data, ReportChange);
        },
        error: function () {

        }
    })
}
//更新报表清单
function upDateRptLists() {
    var $reportList_li = $("#choosereport #reportlist li")
    $reportList_li.removeClass("double");
    $reportList_li.removeClass("visible")
    $reportList_li.hide();
    for (var i = 0; i < ChartSettings.CycTypes.length; i++) {
        var visible_li = $("#choosereport #reportlist li[cycType=" + ChartSettings.CycTypes[i] + "]")
        visible_li.addClass("visible");
        visible_li.show();
    }
    $("#choosereport #reportlist li.visible:odd").addClass("double");

    var chkdPageNos = $("#choosereport #reportlist li.visible :checkbox:checked");
    var PageNos = [];
    for (var i = 0; i < chkdPageNos.length; i++) {
        PageNos.push(chkdPageNos.eq(i).attr("pageno"));
    }
    ChartSettings.PageNos = PageNos;
}

$(document).ready(function () {
    GetInitData();
    $("#settings_button").click(function () {
        if (ChartSettings.PageNos.length < 1) {
            alert("请选择报表！");
            return;
        }
        if (ChartSettings.Fields.PerField == undefined) {
            alert("请选择灾情指标！");
            return;
        }
        if (ChartSettings.Units.length == 0) {
            alert("请选择行政单位！")
            return;
        }
        $('#step_shower').hide();
        $('#chart_shower').show();
        GetChartData();
    })
    $('#back').click(function () {
        $('#chart_shower').hide();
        $('#step_shower').show();
        //$('#save').show();
        ChartSettings.Chart = null;
    });
    $("#choosecyctype .selectall").click(function () {
        $("#cyctypelist :checkbox").attr("checked", true);
        ChartSettings.CycTypes = [3, 4, 5, 6];
        upDateRptLists();
        //        GetReportData();
    });
    $("#choosecyctype .clearall").click(function () {
        $("#cyctypelist :checkbox").attr("checked", false);
        ChartSettings.CycTypes = [];
        upDateRptLists();
        //        ChartSettings.PageNos = [];
        //        GetReportData();
    });
    $("#choosereport .selectall").click(function () {
        SelectedAll($("#reportlist li.visible input"));
    });
    $("#choosereport .clearall").click(function () {
        UnSelectedAll($("#reportlist li.visible input"));
    });
    $("#chooseunit .selectall").click(function () {
        SelectedAll($("#unitslist li input").not("[unitcode=" + sUnitCode + "]"));
    });
    $("#chooseunit .clearall").click(function () {
        UnSelectedAll($("#unitslist li input").not("[unitcode=" + sUnitCode + "]"));
    });

    /**************************************************************************************************************************************/
    /*月份筛选*/
//    $("#choosemonth #beginmonth").change(function () {

//        ChartSettings.Months = [];
//        var isDouble = false;
//        var beginmonth = $(this).find('option:selected').attr('value');
//        var endmonth = $("#choosemonth #endmonth").find('option:selected').attr('value');
//        if (beginmonth <= endmonth) {

//            $("#choosemonth #monthlist").empty();
//            for (var monthobj = beginmonth; monthobj <= endmonth; monthobj++) {
//                $("#choosemonth #monthlist").append("<li class='" + (isDouble ? "double" : "") + "'><input class='radio_select' month='" + monthobj + "' type='checkbox' name='mouth'  checked='checked'/><span class='radio_title'>" + monthobj + "月</span></li>");
//                ChartSettings.Months.push(parseInt(monthobj));
//                isDouble = !isDouble;
//            }
//        }
//    })
})

//***********************报表生成***************************
var GetChartData = function () {
    $.ajax({
        //        url:"../../GeneralProcedure/LL/DisasterAnalysisChart.ashx?pageno=" + ArrayToQuery(ChartSettings.PageNos) + "&disasterType=" + ChartSettings.Fields.Field + "&dw=" + ArrayToQuery(ChartSettings.Units),
        url: "DisasterAnalysisChart",
        data: { pageNOs: ArrayToQuery(ChartSettings.PageNos), disasterType: ChartSettings.Fields.Field, unitCodes: ArrayToQuery(ChartSettings.Units) },
        type: "post",
        success: function (data) {
            if ($.isEmptyObject(data)) {
                alert("获取数据失败！");
            }
            else {
                ChartSettings.Chart = CreateChart(data);
            }
        },
        error: function () {
            alert("获取数据失败！");
        }
    })
}
var CreateChart = function(Data) {
    var Chart = new Highcharts.Chart({
        credits: {
            enabled: false
        },
        chart: {
            renderTo: 'chart',
            defaultSeriesType:  "column",  //ChartSettings.PageNos.length > 1 ? "line" :
            marginRight: 80
        },
        title: {
            text: Data.Title,
            x: -20
        },
        subtitle: {
            text: Data.SubTitle, //新写的时间获取            style: {
              fontFamily: '宋体',
              fontSize: '13px',
            },
            x: -20
        },
        xAxis: {
            categories: Data.Categories
        },
        yAxis: {
            title: {
                text: ChartSettings.Fields.FieldName,
                style:{fontSize: '13px'}
            },
            plotLines: [{
                value: 0,
                width: 1,
                color: '#808080'
            }]
            },
            tooltip: {
                formatter: function() {
                    return '<b>' + this.series.name + '</b><br/>' + this.x + '<br/>数量：' + this.y + '';
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'top',
                x: 0,
                y: 70,
                borderWidth: 0
            },
            series: Data.Series
        });
        return Chart;
    }
    
