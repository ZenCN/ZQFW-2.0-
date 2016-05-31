<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Assessment.aspx.cs" Inherits="ZQFW.Views.Assessment" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="webkit" name="renderer"/>
    <title></title>
    <link href="../../CSS/Public/Body.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/Statistics/Assessment/userview.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/Plugins/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../../Scripts/Statistics/Assessment/evallist.js" type="text/javascript"></script>
</head>
<body class="DarkGray">
    <div class="left_side">
<%--    <label class="eval_box_title">评估类型</label>--%>
        <div style="text-align:center; margin-bottom:5px; height:28px;">
           <%-- <select id="eval_type">
                <option value="1">年度灾情评估</option>
                <option value="0">场次灾情评估</option>
            </select>--%>
            <ul id="eval_type">
                <li class="left_line selected"><span>年度灾情评估</span></li>
                <li><span>场次灾情评估</span></li>
            </ul>
        </div>
    </div>
    <div class="display_area">
      <div class="display_area_frame"><iframe id="displayframe" name="displayframe" style="position:absolute; height:100%;width:100%" frameborder="0"></iframe></div>
      <div class="display_area_foot"></div>
    </div>
<script type="text/javascript">
    var perPageShowCount = 6;  //每页显示数据条数
    var perQueryCount = 4 * perPageShowCount;   //每次查询数据条数
    //type表示单场或年度评估，container表示显示数据容器，showcount表示每次显示数据条数
    var year = new evalListbase({ type: 'year', container: $('.left_side'), showcount: perPageShowCount });
    var single = new evalListbase({ type: 'single', container: $('.left_side'), showcount: perPageShowCount });
    var previewstate = 'seeall';
    var firstLoad = true; //全局变量，是否第一次加载

    //获取年度洪涝灾情评估数据
    getEvalData({
        listcount: 0,        //当前数据条数
        percount: perQueryCount,         //查询数据条数
        object: year
    });

    //获取单场洪涝灾情评估数据
    getEvalData({
        listcount: 0,            //当前数据条数
        percount: perQueryCount,             //查询数据条数
        object: single
    });

    year.getPageUp().bind('click', function () {
        if (!year.getPageUp().hasClass('unable') && !year.getIsAnimating()) {
            year.up();
        }
    })

    year.getPageDown().bind('click', function () {
        if (!year.getPageDown().hasClass('unable') && !year.getIsAnimating()) {
            if (year.down()) {   //如果后面还有数据且往后2页的数据还未查询
                getEvalData({
                    listcount: year.getCount(), //当前数据条数
                    percount: perQueryCount,         //查询数据条数
                    object: year
                });
            }
        }
    })

    single.getPageUp().bind('click', function () {
        if (!single.getPageUp().hasClass('unable') && !single.getIsAnimating()) {
            single.up();
        }
    })
    single.getPageDown().bind('click', function () {
        if (!single.getPageDown().hasClass('unable') && !single.getIsAnimating()) {
            if (single.down()) {   //如果后面还有数据且往后2页的数据还未查询
                getEvalData({
                    listcount: single.getCount(), //当前数据条数
                    percount: perQueryCount,         //查询数据条数
                    object: single
                });
            }
        }
    })

    single.getQueryButton().bind('click', function () {
        $(this).attr("disabled", "disabled");
        single.ClearLists();  //清空评估数据
        getEvalData({
            listcount: 0, //当前数据条数
            percount: perQueryCount,         //查询数据条数
            object: single
        });
    })

    $('#eval_type li').click(function () {
        $('#eval_type li').removeClass('selected');
        $(this).addClass('selected');
        if ($(this).index() == 0) {
            year.show();
            single.hide();
        }
        else {
            single.show();
            year.hide();
        }
    });

//    //切换灾情评估类型
//    $("#eval_type").change(function () {
//        if ($(this).val() == "0") {
//            single.show();
//            year.hide();
//        }
//        else {
//            year.show();
//            single.hide();
//        }
//    })
</script>
</body>
</html>
