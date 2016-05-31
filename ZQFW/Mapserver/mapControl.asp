<%@ enablesessionstate=false language=javascript CODEPAGE="65001" %>  
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<!-- #include file="public.asp" -->
<%
//page aspcompat=true
    //function parameter(name, startup) {
       // return (Request.Form(name).Count > 0) ? parameterForm(name, startup) : parameterCookies(name, startup);
    //}
   
    function parameter(name,startup)
    {
    	return (Request.QueryString(name).Count > 0)? parameterQueryString(name,startup):parameterForm(name,startup);
 }
// grow list of values
function growList(org, value) {
    if (org == null || org == "")
        return value;
    return org + "$" + value;
}
// ensure the page is not cached
Response.Expires = 0;

// create export parameters
var findV = false;
var hyperlinksNewV = false;
var infoV = false;
var layersV = false;
var legendV = false;
var locationV = false;
var scaleV = false;
var viewsV = false;

// create parameters
var boxBandX = parameterForm("boxBandX", "");
var boxBandY = parameterForm("boxBandY", "");
var boxStopX = parameterForm("boxStopX", "");
var boxStopY = parameterForm("boxStopY", "");
var count = parameterForm("count", "0");
var findP = parameterForm("findP", "y");
var layersP = parameterForm("layersP", "y");
var legendP = parameterForm("legendP", "y");
var mode = parameterForm("mode", "");
var view = parameterForm("view", "");
var viewsP = parameterForm("viewsP", "y");
var what = parameterForm("what", "");
var where = parameterForm("where", "");
var queriesP = parameterForm("queriesP", "y");
var query = parameterForm("query", "");
var queryPars = parameterForm("queryPars", "");
var x = parameterForm("x", "");
var y = parameterForm("y", "");
var command = parameter("command", "");
var boxSizeX = parameter("boxSizeX", "805");
var boxSizeY = parameter("boxSizeY", "600");
var state = parameter("state", "");
var pageNO = parameter("pageNO","");
var disasterInfoType = parameter("disasterInfoType","");
var colors = parameter("colors", "['#9ABA18','#BAA404','#DA7804','#F05000','#FF0000','#C2B8AE','#EEEEEE']");
var level = parseInt(parameter("level",""));
var regionCode = parameter("regionCode", "");
var regionName = parameter("regionName", "");
var disasterInfoValues = parameter("disasterInfoValues", "");
var breakValues = parameter("breakValues", "[20,40,60,120]");
var mapType = parseInt(parameter("mapType", ""));
var mapParams = getMapParams(mapType);
var configOpts = getConfigOpts(mapParams, boxSizeX, boxSizeY);

// handle mapserver commands that change state string
if (command == "startup") {
    //var configOpts = getConfigOptsFromFile(mapParams);
    //configOpts = resetMapSize(configOpts, boxSizeX, boxSizeY);
    //Response.Cookies("configOpts") = configOpts;
}
else if (command == "changeMapSize") {
        //calStateByOpts(configOpts);
//configOpts = resetMapSize(configOpts,boxSizeX,boxSizeY);
//Response.Cookies("configOpts") = configOpts;
}

// create mapserver object
var mapserver;
mapserver = Server.CreateObject("Manifold.MapServer");
mapserver.CreateWithOpts(configOpts, state, Server); 

var index = 0;
// handle mapserver commands that do not change state string
if(command == "startup") {
    renderMap(mapParams, mapType, disasterInfoValues, breakValues, colors, level, regionCode);
    setLayersVisibility(mapParams, mapType, level);
    command = ""
    regionName = getNameByCode(mapserver,mapParams,level - 1, regionCode);
}
else if (command == "renderMap") {
    if(mode == "changeRegion")
    {
        updateState(mapParams, level, regionCode);
        setLayersVisibility(mapParams, mapType, level);
        mode = "";
    }
    renderMap(mapParams, mapType, disasterInfoValues, breakValues, colors, level, regionCode);
    command = ""
    regionName = getNameByCode(mapserver,mapParams, level - 1, regionCode);
} 
else if (command == "changeMapSize") {
updateState(mapParams, level, regionCode);
        command = ""
}

   
// create results
var copyright = mapserver.Copyright;
var cxhdr = mapserver.CX + 180;
var cxmap = mapserver.CX;
var cymap = mapserver.CY;
var fields = mapserver.Fields;
var layers = mapserver.Layers;
var location = "";
if (locationV)
	location = mapserver.Location;
var scaling = "";
if (scaleV)
	scaling = mapserver.Scale;
state = mapserver.State;
var subtitle = mapserver.Subtitle;
var target = mapserver.Target;
var title = mapserver.Title;
var views = mapserver.Views;
var queries = mapserver.Queries;


// release mapserver object
mapserver = null;

// redirect response if necessary
if (target != "" && !hyperlinksNewV)
	Response.Redirect(target);

// compose image URL
var image = "map.asp?state=" + Server.URLEncode(state) + "&command=" + Server.URLEncode(command) + "&mode=" + Server.URLEncode(mode) + "&query=" + Server.URLEncode(query) + "&queryPars=" + Server.URLEncode(queryPars) + "&what=" + Server.URLEncode(what) + "&where=" + Server.URLEncode(where) + "&x=" + Server.URLEncode(x) + "&y=" + Server.URLEncode(y)
    + "&boxSizeX=" + Server.URLEncode(boxSizeX) + "&boxSizeY=" + Server.URLEncode(boxSizeY) + "&mapType=" + Server.URLEncode(mapType);
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head><title><%= encode(title) %></title>
    <link href="css/mapControl/button.css" rel="stylesheet" type="text/css" />
    <link href="css/mapControl/mapinfo.css" rel="stylesheet" type="text/css" />
    <link href="css/mapControl/mapControl.css" rel="stylesheet" type="text/css" />
    <link href="css/mapControl/legend.css" rel="stylesheet" type="text/css" />
    <link href="css/mapControl/legendEditor.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/Library/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="JQuery/jquery.contextmenu.r2.packed.js" type="text/javascript"></script>
    
    <script src="js/public.js" type="text/javascript"></script>
    <script src="js/mapControl/refreshMap.js" type="text/javascript"></script>
    <script src="js/mapControl/getOffset.js" type="text/javascript"></script>
    <script src="js/mapControl/showInfo.js" type="text/javascript"></script>
    <script src="js/mapControl/legend.js" type="text/javascript"></script>
    <script src="js/mapControl/legendEditor.js" type="text/javascript"></script>
<!--    <script src="js/mapControl/chart.js" type="text/javascript"></script>-->
<!-- content scripting -->
<script type="text/javascript" language="javascript">

    var opera = (navigator.userAgent.indexOf("Opera") >= 0)? true: false;
    var ie = (document.all && !opera)? true: false;
    var nn4 = (document.layers)? true: false;
    var bandH = <%= cymap %>;
    var bandW = <%= cxmap %>;
    var bandX = -1;
    var bandXOffset = 0;
    var bandY = -1;
    var bandYOffset = 0;
    var TimeFnOnMouseMove = null;     //为mousemove事件定义setTimeout执行方法
    var TimeFnOnClick = null;     //为click事件定义setTimeout执行方法

    // start page
    function startup() {
    var level = parseInt(obtainHidden("level"));
    var disasterInfoType = obtainHidden("disasterInfoType")
    updataLegend(disasterInfoType, level);  //更新图例
        //    if (window.parent.timer1 != null) {
        //        window.parent.timer2 = new Date();
        //        alert(window.parent.timer2 - window.parent.timer1);
        //    }

    window.onresize = function() { changeMapSize(); }   //窗口大小改变事件
    shieldMapClickEvent();          //绑定单击事件
    //shieldDefContextmenu("map");   //屏蔽map元素的浏览器默认上下文菜单
    //bindingMapContextmenu();        //对map元素绑定此右键菜单
    window.parent.hideLoading();      //隐藏正在加载
}

</script>
</head>

<body link="black" vlink="black" alink="black" onload="startup();" style=" margin:0px;">
<!--<div id="top">
    <div id="buttons" class="inline_table">
        <ul class="chartbutton">
            <li id="leftbutton" ><div  onclick = "createChart('bar')" >
                <img id="chartbar" src="images/button/chart_bar.gif" /></div></li>
            <li id="mid_button" ><div  onclick = "createChart('pie')" >
                <img id="chartpie"src="images/button/chart_pie.gif" /></div></li>
            <li id="rightbutton"   ><div onclick = "createChart('table')">
                <img id="maptable"src="images/button/table.gif" /></div></li>
        </ul>
    </div>   
</div>-->
<!-- content language="javascript" onload="ResizeImg(this,<% = cxmap%>,<%= cymap %>)"  ondblclick='changeMapLevel(event)' -->
<form action="mapControl.asp" method="post" id="formMap">
    <div id="aboutMap" class="aboutMap" style=" position:absolute">    
		<input id="map" class="map" style="margin:0" type="image" src="<%=image %>" width="<%= cxmap %>" height="<%= cymap %>" border="0" 
				onmouseover='createInfoWindow()' onmouseout='hideWindow()' onmousemove='showInfo(event)'  ondblclick='changeMapLevel(event)'/>
	   <!-- <img id="dd" class="map" style="margin:0;" width="<%=cxmap %>" height="<%=cymap %>" src="../Image/江西省2013年10月10日-10月10日受灾人口分布图.png" />-->
		<input id="boxBandX" name="boxBandX" type="hidden" value="<%= boxBandX %>"/>
		<input id="boxBandY" name="boxBandY" type="hidden" value="<%= boxBandY %>"/>
		<input id="boxSizeX" name="boxSizeX" type="hidden" value="<%= cxmap %>"/>
		<input id="boxSizeY" name="boxSizeY" type="hidden" value="<%= cymap %>"/>
		<input id="boxStopX" name="boxStopX" type="hidden" value="<%= boxStopX %>"/>
		<input id="boxStopY" name="boxStopY" type="hidden" value="<%= boxStopY %>"/>
		<input id="command" name="command" type="hidden" value=""/>
		<input id="findP" name="findP" type="hidden" value="<%= findP %>"/>
		<input id="layersP" name="layersP" type="hidden" value="<%= layersP %>"/>
		<input id="legendP" name="legendP" type="hidden" value="<%= legendP %>"/>
		<input id="mode" name="mode" type= "hidden" value="<%= mode %>"/>
		<input id="state" name="state" type="hidden" value="<%= state %>"/>
		<input id="view" name="view" type="hidden" value=""/>
		<input id="viewsP" name="viewsP" type="hidden" value="<%= viewsP %>"/>
		<input id="queriesP" name="queriesP" type="hidden" value="<%= queriesP %>"/>
		<input id="query" name="query" type="hidden" value=""/>
		<input id="queryPars" name="queryPars" type="hidden" value=""/>
		<input id="mapType" name="mapType" type="hidden" value="<%= mapType %>"/>
        <input id="pageNO" name="pageNO" type="hidden" value="<%= pageNO %>"/>
        <input id="disasterInfoType" name="disasterInfoType" type="hidden" value="<%= disasterInfoType %>"/>
        <input id="colors" name="colors" type="hidden" value="<%= colors%>"/>
        <input id="level" name="level" type="hidden" value="<%= level%>"/>
        <input id="regionCode" name="regionCode" type="hidden" value="<%= regionCode%>"/>
        <input id="regionName" name="regionName" type="hidden" value="<%= regionName%>"/>
        <input id="breakValues" name="breakValues" type="hidden" value="<%= breakValues %>"/>
        <input id="disasterInfoValues" name="disasterInfoValues" type="hidden" value=""/>
<!--        <div class="mapBottom"></div>-->
        <div id="divDisasterInfo" class="mapinfo" style=" display:none"></div>
        <div class="contextMenu" id="mapMenu" style=" display:none">
             <ul>
              <!--<li id="detailInfo">详细信息</li>-->
              <li id="picture">图片</li>
              <li id="video">视频</li>
            </ul>
        </div>
        <div id="divShield" class="shield"></div>
        <div id="divChart" class="divChart"></div>
    </div>
    <div id="legend" style="left:10px;"></div>
    <div id="legendEditor"></div>
    <!--<div id="nhzd" class="nhzd"></div>-->
</form>
<script type="text/javascript">
    $(document).ready(function() {
        $(".shield").click(function() {
            chartHide();
        })
    })
</script>


</body>
</html>
