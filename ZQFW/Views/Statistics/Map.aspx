<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Map.aspx.cs" Inherits="ZQFW.Views.Statistics.Map" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <meta content="webkit" name="renderer"/>
    <title>灾情地图</title>
    <link href="../../MapServer/css/map/side_bar.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/css/map/map.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/css/mapControl/button.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/css/map/ruler.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/css/map/infoTable.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/facebox/facebox.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/css/table/TableCss.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/fancybox/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link href="../../MapServer/css/loginDiv.css" rel="stylesheet" type="text/css" />
</head>
<body onload="loadMap()">
    <form id="form1" runat="server" >
    <div id="side_bar">
<%--           <ul id="side_bar_tab">
            <li class="left_line selected" index="1"><a href="#">行政</a></li>
            <li index="2"><a href="#">流域</a></li>
        </ul>--%>
        <div id="side_bar_lists">
            <ul id="1" class="side_list selected">
                <li id="Li1" type="SZRK" class="listli selected"><a href="#">
                    <img class="icon" src="../../MapServer/images/icons/user.gif" alt="" />受灾人口</a></li>
                <li id="Li2" type="SWRK" class="listli"><a href="#">
                    <img class="icon" src="../../MapServer/images/icons/delete-user.gif" alt="" />死亡人口</a></li>
                <li id="Li3" type="SZRKR" class="listli"><a href="#">
                    <img class="icon" src="../../MapServer/images/icons/search-user.gif" alt="" />失踪人口</a></li>
                <li id="Li4" type="ZYRK" class="listli"><a href="#">
                    <img class="icon" src="../../MapServer/images/icons/send-user.gif" alt="" />转移人口</a></li>
                <li id="Li5" type="SHMJXJ" class="listli"><a href="#">
                    <img class="icon" src="../../MapServer/images/icons/xx.gif" alt="" />受灾面积</a></li>
                <li id="Li6" type="DTFW" class="listli"><a href="#">
                    <img class="icon" src="../../MapServer/images/icons/home.gif" alt="" />倒塌房屋</a></li>
                <li id="Li7" type="ZJJJZSS" class="listli"><a href="#">
                    <img class="icon" src="../../MapServer/images/icons/money.gif" alt="" />直接经济损失</a></li>
                <li id="Li8" type="SLSSZJJJSS" class="listli "><a href="#">
                    <img class="icon" src="../../MapServer/images/icons/water.gif" alt="" />水利经济损失</a></li>
            </ul>
        </div>
    </div>
    <div id="map_frame">
        <div id="aboutmap">
            <div id="top">
                <div id="selectors" class="inline_table" style="margin:0px 10px;">
                    <select id="BBType">
                    </select>
                </div>
                <div id="rulerbox" class="inline_table wbg" style="margin:0px 10px;">
                    <div id="year_div" class="inline_table" style="margin:0px 10px;">
                        <%--                            
                            <ul class="_year">
                                <li class ="year_top"><span class="_up"></span></li>
                                <li class="year_text"><span></span></li>
                                <li class ="year_bottom"><span class="_down"></span></li>
                            </ul>
                        --%>
                        <select id = "yearSelect" class="_year">
                        </select>
                    </div>
                    <div id="ruler">
                    </div>
                </div>
                <%--http://webmap0.map.bdimg.com/image/api/mapctrls2d0.gif--%>
               <%-- <div id="divMapType" unselectable="on" style="bottom: auto; right: 10px; top: 10px; left: auto; cursor: pointer; width: 49px; height: 51px; background-image: url(http://webmap0.map.bdimg.com/image/api/blank.gif); position: absolute; z-index: 10; background-position: initial initial; background-repeat: initial initial; " class=" BMap_noprint anchorTR">
                    <div id="contour" style="position: absolute; width: 71px; height: 21px; border: 1px solid rgb(153, 153, 153); font-size: 12px; bottom: -26px; right: -1px; background-color: white; display: none;">
                        <span style="background-image: url(../../MapServer/images/map/switchMap.gif); background-color: transparent; width: 11px; height: 11px; position: absolute; float: left; top: 5px; left: 4px; background-position: -45px -179px; background-repeat: no-repeat no-repeat; "></span>
                        <span style="position: absolute; top: 4px; margin-left: 18px; ">等高线</span>
                    </div>
                    <span title="切换到直观图" style="right: 0px; top: 0px; cursor: pointer; width: 47px; height: 49px; z-index: 100; position: absolute; font-size: 12px; border: 1px solid rgb(128, 128, 128); background-color: rgb(255, 255, 255);display: none;">
                        <span style="width: 41px; height: 43px; position: absolute; margin: 2px; border: 1px solid rgb(128, 128, 128); background-image: url(../../MapServer/images/map/switchMap.gif); background-color: transparent; background-position: 0px -131px; background-repeat: no-repeat no-repeat; ">
                            <span style="position: absolute; top: 27px; width: 41px; height: 16px; background-color: rgb(128, 128, 128); opacity: 0.5; "></span>
                            <span style="position: absolute; top: 29px; width: 41px; color: white; text-align: center; line-height: 12px; ">直观图</span>
                        </span>
                    </span>
                    
                    <span title="切换到市级图" style="right: 0px; top: 0px; cursor: pointer; width: 47px; height: 49px; z-index: 99; position: absolute; font-size: 12px; border: 1px solid rgb(128, 128, 128); background-color: rgb(255, 255, 255);">
                        <span style="width: 41px; height: 43px; position: absolute; margin: 2px; border: 1px solid rgb(128, 128, 128); background-image: url(../../MapServer/images/map/switchMap.gif); background-color: transparent; background-position: 0px -177px; background-repeat: no-repeat no-repeat; ">
                            <span style="position: absolute; top: 27px; width: 41px; height: 16px; background-color: rgb(128, 128, 128); opacity: 0.5; "></span>
                            <span style="position: absolute; top: 29px; width: 41px; color: white; text-align: center; line-height: 12px; ">市级图</span>
                        </span>
                    </span>
                    <span title="切换到流域图" style="right: 0px; top: 0px; cursor: pointer; width: 47px; height: 49px; z-index: 98; position: absolute; font-size: 12px; border: 1px solid rgb(128, 128, 128); background-color: rgb(255, 255, 255);">
                        <span style="width: 41px; height: 43px; position: absolute; margin: 2px; border: 1px solid rgb(128, 128, 128); background-image: url(../../MapServer/images/map/switchMap.gif); background-color: transparent; background-position: 0px -221px; background-repeat: no-repeat no-repeat; ">
                            <span style="position: absolute; top: 27px; width: 41px; height: 16px; background-color: rgb(128, 128, 128); opacity: 0.5; "></span>
                            <span style="position: absolute; top: 29px; width: 41px; color: white; text-align: center; line-height: 12px; ">流域图</span>
                        </span>
                    </span>
                </div>--%>
            </div>
            <div id="bottom">
                <div id="buttons" class="mapTools" style="margin:0px 10px;">
                    <ul class="chartbutton">
                        <li id="leftbutton" title="柱状图"><a href="#" rel="facebox" onmousedown="checkData(event,'bar')">
                            <img alt="柱状图" id="chartbar" src="../../MapServer/images/button/chart_bar.png" /></a></li>
                        <li id="midl_button" title="饼状图"><a href="#" rel="facebox" onmousedown="checkData(event,'pie')">
                            <img alt="饼状图"  id="chartpie" src="../../MapServer/images/button/chart_pie.png" /></a></li>
                        <li id="midr_button" title="三维饼图"><a href="#" rel="facebox" onmousedown="checkData(event,'p3')">
                            <img alt="三维饼图"  id="chartp3" src="../../MapServer/images/button/chart_p3.png" /></a></li>
                        <li id="rightbutton" title="报表"><a href="#" rel="facebox" onmousedown="checkData(event,'table')">
                            <img alt="报表"  id="maptable" src="../../MapServer/images/button/table.png" /></a></li>
                    </ul>
                </div>
                 <div class="mapTools" style="margin-left:160px; width: 238px;">
                    <input id="btnExportMap" type="button" style="height: 24px;margin: 18px 0;line-height: 24px; cursor:pointer" onclick="pirntMap()" value="导出地图" />
                </div>
                <div id="legend">
                </div>
<%--                <div id="infoTable">
                    显示地图渲染数据
                </div>--%>
            </div>
        </div>
        <iframe id="map" name="map" frameborder="0" src=""  style="background-color:White"  scrolling="no"> </iframe>
        <div id="loading">
        </div>
    </div>
    <div id="loginDiv" style="display: none;">
        <center>
            登录</center>
        <ul>
            <li><span>用户名:</span><input type="text" id="userName" /></li>
            <li><span>密码:</span><input type="password" id="passWord" /></li>
            <li>
                <input id="click" type="button" value="点击" onclick="ss()" /></li>
        </ul>
    </div>
    </form>
    <form id="Form2" action="newMapThread" Method="post" style="display:none">
        <input type="submit" id="exportMap"  style="display:none" />
        <input id="hidImgName" name="hidImgName" type="hidden" runat="server" value=""/>
		<input id="hidImgUrl" name="hidImgUrl" type="hidden"  runat="server" value=""/>
		<input id="hidLegend" name="hidLegend" type="hidden"  runat="server" value=""/>
		<input id="hidMapWidth" name="hidMapWidth" type="hidden"  runat="server" value=""/>
		<input id="hidMapHeight" name="hidMapHeight" type="hidden"  runat="server" value=""/>
    </form>
    <script type="text/javascript" src="../../MapServer/js/public.js"></script>
    <script type = "text/javascript">
        var sUnitCode = "<%=UnitCode %>"  // 使用该系统的行政单位代码

        //加载地图相关项
        function loadMap() {
            loadScript("../../Scripts/Library/jquery-1.11.0.min.js", function () {
                loadScript("../../Scripts/Library/jquery-migrate-1.2.1.min.js");
                loadScript("../../Scripts/Library/Plugins/jquery.cookie.js");
                loadScript("../../MapServer/mapParams/mapParams.js");
                loadScript("../../MapServer/js/map/ruler/ruler.js");
                loadScript("../../MapServer/js/map/getData.js");
                loadScript("../../MapServer/js/map/switchMap.js");
                loadScript("../../MapServer/js/map/loadMap.js");
                loadScript("../../MapServer/js/map/autoloadmap.js", function () {
                    loadScript("../../Mapserver/js/main.js");
                });
                loadScript("../../MapServer/js/printMap.js");
                loadScript("../../Scripts/Library/Plugins/highchart/highcharts.js", function () {
                    loadScript("../../Scripts/Library/Plugins/highchart/highcharts-3d.js");
                    loadScript("../../Scripts/Library/Plugins/highchart/modules/exporting.js");
                    loadScript("../../MapServer/js/map/chart/chart.js?t=0620");
                });
                loadScript("../../MapServer/facebox/facebox.js");
                loadScript("../../MapServer/fancybox/jquery.fancybox.pack.js");
            })
        }
    </script>
</body>
</html>