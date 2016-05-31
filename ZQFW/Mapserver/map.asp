<%@ enablesessionstate=false language=javascript %>
<!-- #include file="public.asp" -->
<%
Response.ContentType = "image/png";
Response.Expires = 0;
//Application.Lock;
var state = parameterQueryString("state", "")
var mapType = parseInt(parameterQueryString("mapType", ""));
var mapParams = getMapParams(mapType);     //获取地图参数
var configOpts = getConfigOpts(mapParams, parameterQueryString("boxSizeX", "805"), parameterQueryString("boxSizeY", "600"));

// create mapserver 
var mapserver;
mapserver = Server.CreateObject("Manifold.MapServer");
mapserver.CreateWithOpts(configOpts, state, Server);
var type = parameterQueryString("type", "");
// inject pngfile into the response
if (type == "legend")
    Response.BinaryWrite(mapserver.RenderLegend());
else {
    var command = parameterQueryString("command", "");
    var mode = parameterQueryString("mode", "");
    var query = parameterQueryString("query", "");
    var queryPars = parameterQueryString("queryPars", "");
    var what = parameterQueryString("what", "");
    var where = parameterQueryString("where", "");
    var x = parameterQueryString("x", "");
    var y = parameterQueryString("y", "");

    var disasterInfoValues = parameterQueryString("disasterInfoValues", "");
    var breakValues = parameterQueryString("breakValues", "");
    var colors = parameterQueryString("colors", "['#9ABA18','#BAA404','#DA7804','#F05000','#FF0000','#C2B8AE','#EEEEEE']");
    var level = parseInt(parameterQueryString("level", ""));
    var regionCode = parameterQueryString("regionCode", "");

    if (command == "query")
        mapserver.Query(query, queryPars);
    else if (command == "find")
        mapserver.Locate(what, where);
    else if (command == "" && mode == "info") {
        if (x != "") {
            if (mapserver.HasInfoAt(x, y))
                mapserver.Click(x, y);
        }
    }
    else if (command == "renderMap") {
        if (mode == "changeRegion") {
            updateState(mapParams, level, regionCode);
            setLayersVisibility(mapParams,mapType, level);
        }
        renderMap(mapParams,mapType, disasterInfoValues, breakValues, colors, level, regionCode);
    }

    //adjustImageQuality(0.8);  //调整图像质量（从0到1,1为最好）
    Response.BinaryWrite(mapserver.Render());
}

// release mapserver object
mapserver = null;

// unlock application
//Application.Unlock;

%>
