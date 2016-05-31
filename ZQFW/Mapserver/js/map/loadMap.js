//初始化地图
function initializingMap(pageNO, disasterInfoType, mapType) {
    var level = 0;   //设置地图初始级别
    //    var limit = $.cookie("limit")
    //    if ($.cookie("logintype") == "loginfz" & limit == "2") {
    //        level = 1;
    //    }
    //    regionCode = $.cookie("unitcode");    //使用该系统的行政单位代码
    var regionCode = sUnitCode;    //使用该系统的行政单位代码

    var state = "";
    var colors = Obj2Str(mapParams.colors[level]);   //获取分类渲染的颜色
    var disasterInfo = getDisasterInfoFromDb(pageNO, mapType, level, regionCode);
    setGlobalDisasterInfo(disasterInfo);
    if (mapType == 3) {
        var disasterInfoValues = getClassifyData(pageNO, disasterInfo);
        var breakValues = "[0, 1.1, 2.1, 3.1]"
    }
    else {
        var disasterInfoValues = getSingleDisasterTypeData(disasterInfo, disasterInfoType);
        var breakValues = Obj2Str(mapParams.disasterInfoTypes[disasterInfoType].breakValues[level]);   //获取分类间断值
        showRenderData();  //显示地图渲染数据表格
    }
    linkingMap(mapType, pageNO, disasterInfoType, level, regionCode, colors, state, disasterInfoValues, breakValues);
}

//加载manifold地图
function linkingMap(mapType, pageNO, disasterInfoType, level, regionCode, colors, state, disasterInfoValues, breakValues) {
    var cxmap = getMapWidth();
    var cymap = getMapHeight() + 20;   //加20像素覆盖掉地图下面的图标; + 20
    //北京市民政局
    if (regionCode == "11034000") {
        regionCode = "11000000";
    }
    //manifold地图链接
    var url = mapParams.mapControlSrc;                //."../../MapServer/mapControl.asp"
    url += "?command=" + encodeURIComponent("startup")
    url += "&pageNO=" + encodeURIComponent(pageNO)
    url += "&disasterInfoType=" + encodeURIComponent(disasterInfoType)
    url += "&level=" + encodeURIComponent(level)
    url += "&regionCode=" + encodeURIComponent(regionCode)
    url += "&colors=" + encodeURIComponent(colors)
    url += "&state=" + encodeURIComponent(state)
    url += "&disasterInfoValues=" + encodeURIComponent(disasterInfoValues)
    url += "&breakValues=" + encodeURIComponent(breakValues);
    url += "&boxSizeX=" + encodeURIComponent(cxmap);
    url += "&boxSizeY=" + encodeURIComponent(cymap);
    url += "&mapType=" + encodeURIComponent(mapType);
    //locateElement(mapParams.mapContainerName).src = url;
    $('#' + mapParams.mapContainerName).attr('src', url);
}

function refreshMap(pageNO, disasterInfoType, mapType) {
    var level = window.frames[mapParams.mapContainerName].window.getMapLevel();
    var regionCode = window.frames[mapParams.mapContainerName].window.getRegionCode();
    var curPageNO = window.frames[mapParams.mapContainerName].window.getCurPageNO();
    var disasterInfo = getGlobalDisasterInfo();
    if (curPageNO != pageNO) {
        disasterInfo = getDisasterInfoFromDb(pageNO, mapType, level, regionCode);
        setGlobalDisasterInfo(disasterInfo);
    }

    var breakValues = Obj2Str(mapParams.disasterInfoTypes[disasterInfoType].breakValues[level]);   //获取分类间断值

    if (mapType == 3) {
        disasterInfoValues = getClassifyData(disasterInfo);
        breakValues = "[-1, 1.1, 2.1, 3.1]"
    }
    else {
        disasterInfoValues = getSingleDisasterTypeData(disasterInfo, disasterInfoType);
        showRenderData();  //显示地图渲染数据表格
    }

    window.frames[mapParams.mapContainerName].window.refreshMap(pageNO, disasterInfoType, level, regionCode, disasterInfoValues, breakValues)
}


function changeMapLevel(pageNO, disasterInfoType, level, regionCode, mapType) {
    var disasterInfo = getDisasterInfoFromDb(pageNO, mapType, level, regionCode);
    setGlobalDisasterInfo(disasterInfo);
    var breakValues = Obj2Str(mapParams.disasterInfoTypes[disasterInfoType].breakValues[level]);   //获取分类间断值

    if (mapType == 3) {
        disasterInfoValues = getClassifyData(disasterInfo);
        breakValues = "[-1, 1.1, 2.1, 3.1]"
    }
    else {
        disasterInfoValues = getSingleDisasterTypeData(disasterInfo, disasterInfoType);
        showRenderData();  //显示地图渲染数据表格
    }
    window.frames[mapParams.mapContainerName].window.changeView(level, regionCode, breakValues, disasterInfoValues);
}


//获取地图容器宽度
function getMapWidth() {
    return $('#' + mapParams.mapContainerName).width();
}

//获取地图容器高度
function getMapHeight() {
    return $('#' + mapParams.mapContainerName).height();
}

//隐藏缓冲图片
function hideLoading() {
    $("#loading").hide();
}

//在地图右侧表格显示渲染数据
function showRenderData() {
    try {
        if (typeof (eval(showInfoTable)) == "function") {
            showInfoTable();
        }
    } catch (e) {
        //alert("not function");
    }
}