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
var disasterInfoType = parameter("disasterInfoType","");
var colors = parameter("colors", "");
var level = parseInt(parameter("level",""));
var regionCode = parameter("regionCode", "");
var regionName = parameter("regionName", "");
var disasterInfoValues = parameter("disasterInfoValues","");
var breakValues = parameter("breakValues", "");
var mapType = parseInt(parameter("mapType", ""));
var mapParams = getMapParams(mapType);
var configOpts = getConfigOpts(mapParams, boxSizeX, boxSizeY);

// handle mapserver commands that change state string

// create mapserver object
var mapserver;
mapserver = Server.CreateObject("Manifold.MapServer");
mapserver.CreateWithOpts(configOpts, state, Server); 

var index = 0;
// handle mapserver commands that do not change state string
if(command == "startup") {
    renderMap(mapParams,disasterInfoValues, breakValues, colors,regionCode);
    setLayersVisibility(mapParams);
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
    <link href="legend.css" rel="stylesheet" type="text/css" />
    <script src="../../Library/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../../Library/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../../../Mapserver/JQuery/jquery.contextmenu.r2.packed.js" type="text/javascript"></script>
    <script src="../../../Mapserver/js/public.js" type="text/javascript"></script>
    <script src="../../../Mapserver/js/mapControl/refreshMap.js" type="text/javascript"></script>
    <script src="../../../Mapserver/js/mapControl/getOffset.js" type="text/javascript"></script>
    <script src="legend.js" type="text/javascript"></script>
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
//    var TimeFnOnMouseMove = null;     //为mousemove事件定义setTimeout执行方法
//    var TimeFnOnClick = null;     //为click事件定义setTimeout执行方法

    // start page
    function startup() {
    var level = parseInt(obtainHidden("level"));
    var disasterInfoType = obtainHidden("");
    updataLegend();  //更新图例
        //    if (window.parent.timer1 != null) {
        //        window.parent.timer2 = new Date();
        //        alert(window.parent.timer2 - window.parent.timer1);
        //    }

    window.onresize = function() { changeMapSize(); }   //窗口大小改变事件
   
    $("input#map").click(function(e) {
        stopDefault(e);
    });
         //绑定单击事件
    //shieldDefContextmenu("map");   //屏蔽map元素的浏览器默认上下文菜单
    //bindingMapContextmenu();        //对map元素绑定此右键菜单
//    window.parent.hideLoading();      //隐藏正在加载
}

</script>
</head>

<body link="black" vlink="black" alink="black" onload="startup();" style=" margin:0px;">
<form action="mapControl.asp" method="post" id="formMap">
    <div id="aboutMap" class="aboutMap" style=" position:absolute">    
		<input id="map" class="map" style="margin:0" type="image" src="<%=image %>" width="<%= cxmap %>" height="<%= cymap %>" border="0"/>
    </div>
    <div id="legend" style="left:10px;width: 140px;z-index: 100;position: absolute;bottom: 10px;">
<!--    <input type="image" src="Map/images/waterLegend.png" />-->
    </div>
    <!--<div id="nhzd" class="nhzd"></div>-->
</form>
</body>
</html>
