window.Report = window.opener.$scope[window.opener.$scope.Attr.NameSpace].Report.Current;

function ConvertToStr(value) {
    if (typeof value == "string") {
        return value.trim();
    } else {
        return "";
    }
}

$(function() {
    printClick();
});

function printClick() {
    var bodyPage = undefined;
    bodyPage = $(window.opener.document).find("#Report table");
    var tableIndex = bodyPage.attr("ng-switch-when");//当前表序号，从0开始
    var ordCode = Report.ReportTitle.ORD_Code;
    var rptClassCode = ordCode.substr(0, 2);
    if (rptClassCode == "HP" && bodyPage.length > 1) {//湖南蓄水表2,3,4
        printHPReport(bodyPage);
    } else {//其他表类(洪涝等)
        
        bodyPage = bodyPage.clone();
        if (rptClassCode == "HL" && (tableIndex == 4 || tableIndex == 5)) {//洪涝表4，5去掉第一列
            bodyPage.find("thead tr").find("th").eq(0).hide();
            var trs = bodyPage.find("tbody tr");
            $.each(trs, function(i, n) {
                $(trs[i]).find("th").eq(0).hide();
            });
        }
        AdjustReportTitle(window.opener.$scope.BaseData.Unit.Local.UnitCode, bodyPage); //调整打印页面表头尾  window.opener.ReportTitle.UnitCode
        setTDValue(bodyPage);
        bodyPage.find("input").remove();
        setTDValue2(bodyPage);
        bodyPage.find("select").remove();
        setTDValue3(bodyPage);
        bodyPage.find("textarea").remove();
        if (tableIndex == 5) {  //表六
            var tds = bodyPage.find("td div.Stretch").parent();
            if (tds.length > 0) {
                $.each(tds, function (i, n) {
                    var s = $(tds[i]).find("div.Stretch span").html();
                    $(tds[i]).html(s);
                });
                bodyPage.find("td div.Stretch").remove();
            }
        }
        $("table tbody").html(bodyPage);
        window.print();
    }
    
}

//将input中的数据复制到td
function setTDValue(bodyPage) {
    //每个包含input的td
    var tds = bodyPage.find("td input[type='text']").parent();
    $.each(tds, function(i, n) {
        var s = $(tds[i]).children("input").val();
        $(tds[i]).html(s);
    });

    if (window.Report.ReportTitle.ORD_Code == 'SH04') {
        var tds = bodyPage.find("td input[type='radio']").parent();
        $.each(tds, function (i, n) {
            var s = $(tds[i]).children("input").val();
            if (!$(tds[i]).children("input")[0].checked) {
                $(tds[i]).children("input").parent().remove();
            }
        });
    }
}

//将select中的数据复制到td
function setTDValue2(bodyPage) {
    var tds = bodyPage.find("td select").parent();
    $.each(tds, function (i, n) {
        var s = $(tds[i]).children("select").find(':selected').text();
        $(tds[i]).html(s);
    })
}

//将select中的数据复制到td
function setTDValue3(bodyPage) {
    var tds = bodyPage.find("td textarea").parent();
    $.each(tds, function(i, n) {
        var s = $(tds[i]).children("textarea").html();
        $(tds[i]).html(s);
    });
}

//调整打印页面表头尾
function AdjustReportTitle(unitcode, sourceBodyPage) {
    //报表标题
    var title = sourceBodyPage.find("caption").html();
    sourceBodyPage.find("caption").remove();
    $("table caption h2").html(title);

    //表号
    var sheetIndex = parseInt(sourceBodyPage.attr("ng-switch-when")) + 1;
    var sheet = "表&nbsp&nbsp&nbsp&nbsp号：国汛统" + (sheetIndex) + "表";
    if (unitcode.substr(0, 2) == "11") { //如果是北京
        $(".comSubTitle").remove();
        $("#right").append('<p class="comSubTitle">批准机关：北京市人民政府防汛抗旱指挥部</p>');
        //调整subTitle
        $("#right").css("width", 250);
        $("#leftPar").css("padding", "6px 0 0 2px");
        $("#title").css("height", "0px");
    }
    var tbdw = ConvertToStr(Report.ReportTitle.UnitName);
    var qzrq = Report.ReportTitle.StartDateTime + "～" + Report.ReportTitle.EndDateTime;
    var dwfzr = ConvertToStr(Report.ReportTitle.UnitPrincipal);
    var tjfzr = ConvertToStr(Report.ReportTitle.StatisticsPrincipal);
    var tbr = ConvertToStr(Report.ReportTitle.WriterName);
    var tbrq = ConvertToStr(Report.ReportTitle.WriterTime);
    $(".tagContent").append("<div style='margin-top: 2px;'> <div id='dwfzr' class='tail'>单位负责人：</div> <div id='tjfzr' class='tail'>统计负责人：</div>  <div id='tbr' class='tail'>填报人：</div> <div id='tbrq' class='tail'>填报日期：2013-6-20</div>  </div>");
    $("#sheet").html(sheet);
    $("#left").html("填报单位：" + tbdw);
    $("#middle").html("起止日期：" + qzrq);
    $("#dwfzr").html("单位负责人：" + dwfzr);
    $("#tjfzr").html("统计负责人：" + tjfzr);
    $("#tbr").html("填报人：" + tbr);
    $("#tbrq").html("填报日期：" + tbrq);
}

//打印蓄水表2，3，4(sourceBodyPage的个数大于1)
function printHPReport(aSourceBodyPage) {
    var subBodyPage = undefined;
    var title = aSourceBodyPage.eq(1).find("caption").html(); //表2
    if (title == undefined) {//表3，4
        title = aSourceBodyPage.eq(1).parent().find("span").html();
    }

    $("table").remove();

    $(".tagContent").append("<div id='rptTitle' style='margin-top: 2px;'>" + title + "</div>");
    for (var i = 1; i < aSourceBodyPage.length; i++) {
        subBodyPage = $(aSourceBodyPage[i]).clone();
        setTDValue(subBodyPage);

        $(".tagContent").append(subBodyPage);
    }

    $("table").css("float", "left");
    $("table").css("width", "500px");


    var tbdw = ConvertToStr(Report.ReportTitle.UnitName);
    var qzrq = Report.ReportTitle.StartDateTime + "～" + Report.ReportTitle.EndDateTime;
    var dwfzr = ConvertToStr(Report.ReportTitle.UnitPrincipal);
    var tjfzr = ConvertToStr(Report.ReportTitle.StatisticsPrincipal);
    var tbr = ConvertToStr(Report.ReportTitle.WriterName);
    var tbrq = ConvertToStr(Report.ReportTitle.WriterTime);
    $(".tagContent").append("<div style='margin-top: 2px;'> <div id='dwfzr' class='tail'>单位负责人：</div> <div id='tjfzr' class='tail'>统计负责人：</div>  <div id='tbr' class='tail'>填报人：</div> <div id='tbrq' class='tail'>填报日期：2013-6-20</div>  </div>");
    $("#left").html("填报单位：" + tbdw);
    $("#middle").html("起止日期：" + qzrq);
    $("#dwfzr").html("单位负责人：" + dwfzr);
    $("#tjfzr").html("统计负责人：" + tjfzr);
    $("#tbr").html("填报人：" + tbr);
    $("#tbrq").html("填报日期：" + tbrq);
    window.print();
}