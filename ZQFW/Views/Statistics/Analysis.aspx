<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Analysis.aspx.cs" Inherits="ZQFW.Views.Analysis" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta content="webkit" name="renderer"/>
    <title>灾情分析</title>
    <link href="../../CSS/Public/Body.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/Statistics/Analysis/chart_left_side.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/Statistics/Analysis/right_step_shower.css" rel="stylesheet"
        type="text/css" />
    <link href="../../CSS/Statistics/Analysis/chart_shower.css" rel="stylesheet" type="text/css" />
    <link href="../../CSS/Statistics/Analysis/TableCss.css" rel="stylesheet" type="text/css" />
    <%--    <link href="../../CSS/Plugins/zTree/zTreeStyle.css" rel="stylesheet" type="text/css" />--%>
    <script type="text/javascript">
        var sUnitCode = "<%=UnitCode %>";  //使用该系统的行政单位代码
    </script>
    .0
    <script src="../../Scripts/Statistics/Analysis/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/Plugins/highchart/highcharts.js" type="text/javascript"></script>
    <script src="../../Scripts/Library/Plugins/highchart/modules/exporting.js" type="text/javascript"></script>
    <%--    <script src="../../Scripts/Library/Plugins/jquery.ztree.all-3.5.min.js" type="text/javascript"></script>--%>
    <script src="../../Scripts/Statistics/Analysis/TableDate.js" type="text/javascript"></script>
    <script src="../../Scripts/Statistics/Analysis/TableTitle_JS.js" type="text/javascript"></script>
    <script src="../../Scripts/Statistics/Analysis/Analysis1.js" type="text/javascript"></script>
</head>
<body class="DarkGray">
    <div id="confirm-dialog" style="display: none;" title="保存趋势图">
        <p>
            趋势图命名：<input type="text" value="" id="name_tendency" /></p>
    </div>
    <div id="left_side">
        <label class="info_title">
            我的分析图</label>
        <ul>
        </ul>
    </div>
    <div id="step_shower">
        <div class="fakebody">
            <label id="settings_title">
                分析参数设置</label>
            <div id="settings">
                <div id="steps">
                    <div id="timeandtype" class="step" style="width: 25%;">
                        <div class="groupes">
                            <div id="chooseyear" class="groupe mxselclasss">
                                <label class="title">
                                    结束时间范围选择</label>
                                <div class="groupe_select" style="margin-top: 10px;">
                                    <div class="selects">
                                    <label for="mxstartdate" style="font-family:微软雅黑">
                                        由</label>
                                        <%--  <label for="mxstartdate" style="font-size: larger">
                                            起始时间:</label>           
                                    <select id="beginyear" class="select">
                                        </select>
                                        <select id="beginmonth" class="select">
                                        </select>
                                        <select id="beginday" class="select">
                                        </select>--%>
                                        <%--<label for="mxstartdate" style="font-size: larger; font-family:微软雅黑">
                                            起始时间:</label> --%>
                                        <input id="mxstartdate" style="width: 35%" class="Wdate" type="text"  onfocus="var mxdate = mynewdate(); WdatePicker({minDate:mxdate.mxsttime,maxDate:mxdate.mxedtime,onpicked:stdatechange})">
                                        <label for="mxenddate" style="font-family:微软雅黑">
                                        至</label>
                                        <input id="mxenddate" style="width:35%" class="Wdate" type="text"  onfocus="var mxdate = mynewdate();WdatePicker({minDate:mxdate.mxsttime ,maxDate:mxdate.mxedtime,onpicked:eddatechange})">
                                    </div>
                                   <%-- <label for="endyear" style=" margin-left:40%; font-family:微软雅黑"">
                                        至</label>
                                    <div class="selects">
                                    <label for="mxenddate" style="font-size: larger; font-family:微软雅黑">
                                            起始时间:</label> 
                                        <input id="mxenddate1" style="width: 48%" class="Wdate" type="text" onfocus="var mxdate = mynewdate();WdatePicker({minDate:mxdate.mxsttime ,maxDate:mxdate.mxedtime,dchanged:eddatechange})">,dchanged:stdatechange
                                    </div>--%>
                                    <%--,ychanged:stdatechange,Mchanged:stdatechange--%>
                                    <%-- <span id="starttimeshake" style="margin-left:50px;color:Red; visibility:hidden">开始时间不得大于结束时间</span>--%>
                                </div>
                            </div>
                            <%-- <div id="choosetime" style="margin-top: 10px;" class="groupe mxselclasss">
                                <label class="title top">
                                    结束时间选择</label>
                                <div class="groupe_select" style="margin-top: 10px;">
                                    <div class="selects">
                                        <label for="mxenddate" style="font-size: larger">
                                            结束时间:</label>
                                       <select id="endyear" class="select">
                                        </select>
                                        <select id="endmonth" class="select">
                                        </select>
                                        <select id="endday" class="select">
                                        </select>                                        
                                             <select id="edaterange" class="select daterange">
                                        </select>
                                        <input  id="mxenddate1" style="width: 50%"  class="Wdate" type="text" onfocus="var mxdate = mynewdate();WdatePicker({minDate:mxdate.mxsttime ,maxDate:mxdate.mxedtime,dchanged:eddatechange})">
                                    </div>                                    
                                   ,ychanged:eddatechange,Mchanged:eddatechange
                                   <span id="endtimeshake" style="margin-left:50px;color:Red;visibility:hidden">结束时间不得小于开始时间</span>
                                </div>
                            </div>--%>
                            <%-------------备份--------------%>
                            <%--  <div id="Div1" class="groupe">
                                <label class="title">
                                    年份选择</label>
                                <div class="groupe_select">
                                 <div class="selects">
                                        <label for="beginyear">起始年份:</label>
                                        <select id="Select1" class="select">
                                        </select>
                                        <label for="endyear">至</label>
                                        <select id="Select2" class="select">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div id="Div2" class="groupe">
                                <label class="title top">
                                    时间选择</label>
                                <div class="groupe_select">
                                    <div class="selects">
                                        <label for="beginmonth">起始时间:</label>
                                        <select id="Select3" class="select">
                                        </select>
                                        <select id="Select4" class="select"></select>
                                        <select id="bdaterange" class="select daterange" >
                                        </select>
                                    </div>
                                     <div class="selects">
                                        <label for="endmonth">结束时间:</label>
                                        <select id="Select5" class="select">
                                        </select>
                                        <select id="Select6" class="select"></select>
                                        <select id="edaterange" class="select daterange">
                                        </select>
                                    </div>
                                </div>
                            </div>--%>
                            <%-------------备份--------------%>
                            <div id="choosecyctype" class="groupe scollCon" style="top: 20%">
                                <%--                                <p style="height: 20px;line-height: 20px;margin: 0px;">--%>
                                <div class="title top">
                                    <span style="float: left">报表类型选择</span> <a class="selectall" href="javascript:void(0);"
                                        taget="cyctypelist">全选</a> <a class="clearall" href="javascript:void(0);" taget="cyctypelist">
                                            清除</a>
                                </div>
                                <%--                                </p>--%>
                                <div class="scoll">
                                    <ul id="cyctypelist" class="groupe_list">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="reportin" class="step" style="width: 30%; left: 25%">
                        <div class="groupes">
                            <div id="choosereport" class="groupe scollCon">
                                <div class="title">
                                    <span style="float: left">报表选择</span> <a class="selectall" href="javascript:void(0);"
                                        taget="reportlist">全选</a> <a class="clearall" href="javascript:void(0);" taget="reportlist">
                                            清除</a>
                                </div>
                                <div id="reportscoll" class="scoll">
                                    <ul id="reportlist" class="groupe_list">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="types" class="step" style="width: 25%; left: 55%">
                        <div class="groupes">
                            <div id="choosetypes" class="groupe scollCon">
                                <label class="title">
                                    指标选择</label>
                                <div class="scoll">
                                    <ul id="typelist" class="groupe_list">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="units" class="step" style="width: 20%; left: 80%;">
                        <div class="groupes last">
                            <div id="chooseunit" class="groupe scollCon">
                                <div class="title">
                                    <span style="float: left">单位选择</span> <a class="selectall" href="javascript:void(0);"
                                        taget="unitslist">全选</a> <a class="clearall" href="javascript:void(0);" taget="unitslist">
                                            清除</a>
                                </div>
                                <div class="scoll">
                                    <ul id="unitslist" class="groupe_list">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <span id="settings_button"><span>生成</span></span>
        </div>
    </div>
    <div id="chart_shower" style="display: none;">
        <div class="fakebody">
            <div id="chart">
            </div>
            <%--            
            <div id="table">                
            </div>
            --%>
            <span id="back" class="buttonBack"><span>返回</span></span> <span id="save" class="buttonBack"
                style="display: none;"><span>保存</span></span>
        </div>
    </div>
</body>
</html>
