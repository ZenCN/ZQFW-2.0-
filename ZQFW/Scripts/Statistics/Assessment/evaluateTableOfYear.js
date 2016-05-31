$('document').ready(function () {
    if (window.parent.previewstate == 'preview') {
        var selectedsinglenolist = window.parent.GetSelectedTableCollection();
        var params = { evalType: 1, pagenoList: selectedsinglenolist, title: "年度评估预览", pageno: window.parent.yearPageNo };
    } else {
        var params = { evalType: 1, pageNO: window.parent.currentPageNO };
    }

    $.ajax({ type: 'post',
        url: '/Statistics/GetDisasterAssessmentContent',
        data: params,
        beforeSend: function () {
            $('#loading').show();
        },
        success: function (jsonData) {
            $('#year_eval_table_frame').show();
            eval_evaltable(jsonData);
            $('#loading').hide();
            if (window.parent.previewstate == 'preview') {
                $('#btn_publish_year').show();
            }
        },
        error: function (e) {

        }
    });
});

rulesCollection = ["一般灾害", "较大灾害", "重大灾害", "特大灾害"]; //将灾情等级由数字转换成文字，此数组为转换规则
function ShowLevel(level, rulesCollection) {
    var flag = "无";
    for (var i = 0; i < 5; i++) {

        if ((i + 1) == level) {
            flag = rulesCollection[i]
        }
    }
    return flag;
}

//获取灾情等级最高的等级
function GetHighestItemLevel(data) {
    var pos = 0;
    var max = 0;
    for (var i = 0; i < data.detail.length; i++) {
        if (data.detail[i].itemlevel > max) {
            max = data.detail[i].itemlevel;
            pos = i;
        }
    }
    return max;
}

function tableclick(pageno,type) {
    document.getElementById('chartpieFrame').contentWindow.SetChartValues(pageno, type);
}

//获取灾情等级最高的纪录
function GetHighestItemPos(data) {
    var pos = 0;
    var max = 0;
    for (var i = 0; i < data.detail.length; i++) {
        if (data.detail[i].itemlevel > max) {
            max = data.detail[i].itemlevel;
            pos = i;
        }
    }
    return pos;
}

function GetDisasterchar(disastername) {
    var result;
    if (disastername == '死亡人口(人)') {
        result = 'SWRK';
    } else if (disastername == '受灾人口(万人)') {
        result = 'SZRK';
    } else if (disastername == '受灾面积(千公顷)') {
        result = 'SHMJXJ';
    } else if (disastername == '倒塌房屋(万间)') {
        result = 'DTFW';
    } else if (disastername == '直接经济损失(亿元)') {
        result = 'ZJJJZSS';
    } else if (disastername == '骨干交通中断历时(天)') {
        result = 'SMXJT';
    } else if (disastername == '城市区淹没历时(天)') {
        result = 'GCYMLS';
    }
    return result;
}

function eval_evaltable(data) {
    var ClassName = {};
    //var HighestItemLevel = GetHighestItemLevel(data);
    var evaluationGrade = window.parent.evaluationGrade;
    ClassName["4"] = "Large";
    ClassName["3"] = "Big";
    ClassName["2"] = "Normal";
    ClassName["1"] = "Bit";
    $('#img_warn').addClass(ClassName[evaluationGrade]).show(); 
//    if (typeof(HighestItemLevel) == "string" && "1234".indexOf(HighestItemLevel)>=0) {
//        $('#img_warn').addClass(ClassName[HighestItemLevel]).show();
//    }
    
    $('#t_title').text(data.title); //设置标题

    //设置说明段、' + data.countofcounty + '个县（区）
    var adminRank1 = "省";     //行政级别
    var adminRank2 = "市";
    if (sUnitCode == "00000000")       //如果使用系统的行政单位代码为00000000（即国家防总）
    {
        adminRank1 = "国";
        adminRank2 = "省";
    }
    var gradeCollection = ["一般洪涝灾害", "较大洪涝灾害", "重大洪涝灾害", "特别重大洪涝灾害"]
    $('#t_desc_content').html('&nbsp;&nbsp;&nbsp;&nbsp;据' + data.period + '统计，全' + adminRank1 + '共有' + data.countofprovince + '个' + adminRank2 + '受灾，本年度评定为' + ShowLevel(evaluationGrade, gradeCollection) + '年,灾情如下：');

    //设置表格内容
    var tablehtml = '';
    for (var i = 0; i < data.detail.length; i++) {	//生成一行数据
        tablehtml += "<tr>";
        if (data.detail[i].itemname != '级别场次灾害数量(场)'){
            tablehtml += '<th><a class="hyperlink" ' + (window.parent.previewstate == 'preview' ? '>' : 'href="javascript:void(0)" onclick = "tableclick(' + window.parent.currentPageNO + ',\'' + GetDisasterchar(data.detail[i].itemname) + '\')">') + data.detail[i].itemname + "</a></th>";
        }else{
            tablehtml += '<th><a>' + data.detail[i].itemname + "</a></th>";
        }
        if (data.detail[i].itemlevel == 4) {//对于特大灾害的纪录用红色字警示
            tablehtml += "<td style='color:Red'>" + data.detail[i].itemcount + "</td>";
        } else {
            tablehtml += "<td>" + data.detail[i].itemcount + "</td>";
        }
        tablehtml += "<td>" + ShowLevel(data.detail[i].itemlevel, rulesCollection) + "</td>";
        tablehtml += "<td>" + data.detail[i].remarks + "</td>";
        tablehtml += "</tr>"
    }
    $('#t_table_content').html(tablehtml);

    $("#t_table_content tr:odd").addClass("double");
    if (window.parent.previewstate != 'preview') {
        var chartframe = $('<iframe id="chartpieFrame" name="chartpieFrame" frameborder="0" style="width:698px;height:440px;" src="/Statistics/PieChart"></iframe>');
        $('#t_table_content').append($('<tr></tr>').append($('<td colspan="4" style="magin:0;padding:0;background-image:none;"></td>').append(chartframe)));
    }
}
