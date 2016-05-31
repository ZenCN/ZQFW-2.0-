<%@ enablesessionstate=false language=javascript%>
<!-- #include file="public.asp" -->
<%
Response.ContentType = "text/plain";
Response.Expires = 0;
function goToSuperiorRegion() {
    level = level - 1;
    regionCode = getSuperiorCode(mapserver,mapParams, level, regionCode);
}

function goToTopRegion(topRegionCode) {
    level = 0;
    regionCode = topRegionCode;
}

//获取地图区域参数
function getMapRegion(mapParams) {
    var areaInfo = getAreaByCoord(mapserver,mapParams, level, parseInt(x), parseInt(y));
    if (areaInfo != null) {
        var areaCode = areaInfo.Code;
        var superiorCode = areaInfo.SuperiorCode;

        if (superiorCode == regionCode) {
            if (level < mapParams.maxLevel && level < mapParams.adminRanks.length - 1) {
                level = level + 1;
                regionCode = areaCode;
            }
            else {
                goToSuperiorRegion()
            }
        }
        else {
            if (level > 1) {
                goToSuperiorRegion()
            }
            else {
                goToTopRegion(mapParams.topRegion.code);
            }
        }
    }
    else {
        if (level > 0) {
            goToTopRegion(mapParams.topRegion.code);
        }
    }


    //response level and regionCode
    if (regionCode != parameterQueryString("regionCode", "") | level.toString() != parameterQueryString("level", "")) 
    {
        var json = "{'level':" + level + ",'regionCode':'" + regionCode + "'}";
        Response.Write(json);
    }
}
var mapType = parseInt(parameterQueryString("mapType", ""));
var mapParams = getMapParams(mapType);  //获取地图参数
var configOpts = getConfigOpts(mapParams, parameterQueryString("boxSizeX", "805"), parameterQueryString("boxSizeY", "600"));
var command = parameterQueryString("command", "");
if (command == "changeMapSize") {
    var boxSizeX = parameterQueryString("boxSizeX", "805"); ;
    var boxSizeY = parameterQueryString("boxSizeY", "600"); ;
    configOpts = resetMapSize(configOpts, boxSizeX, boxSizeY);
    //Response.Cookies("configOpts") = configOpts;
}

var state = parameterQueryString("state", "")
// create mapserver 
var mapserver = Server.CreateObject("Manifold.MapServer");
mapserver.CreateWithOpts(configOpts, state, Server); 

var x = parameterQueryString("x", "");
var y = parameterQueryString("y", "");
var regionCode = parameterQueryString("regionCode", "");
var level = parseInt(parameterQueryString("level", ""));

if (command == "changeRegion") {
    getMapRegion(mapParams);
}
else if (command == "select") {
    var selectedUnitCode = "";

    if ((mapType == 1 || mapType == 2) && level == 0) {
        //var areaInfo = getRecord(level, parseInt(x), parseInt(y));
        var areaInfo = getAreaByCoord(mapserver, mapParams, level + 1, parseInt(x), parseInt(y));
    }
    else {
        var areaInfo = getAreaByCoord(mapserver, mapParams, level, parseInt(x), parseInt(y));
    }

    if (areaInfo != null) {
        var areaCode = areaInfo.Code;
        var superiorCode = areaInfo.SuperiorCode;
        if (superiorCode == regionCode || ((mapType == 1 || mapType == 2) && level == 0)) {
            selectedUnitCode = areaCode;
        }
    }
    //response  selectedUnitCode
    Response.Write(selectedUnitCode);
}
else if (command == "changeMapSize") {
    updateState(mapParams, level, regionCode);
    setLayersVisibility(mapParams, mapType, level);

    //response state
    Response.Write(mapserver.State);
}

// release mapserver object
mapserver = null;

// unlock application
//Application.Unlock;

%>
