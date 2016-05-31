
//刷新地图
function refreshMap(pageNO, disasterInfoType, level, regionCode, disasterInfoValues, breakValues, newMode) {
    modifyHidden("pageNO", pageNO);
    modifyHidden("disasterInfoType", disasterInfoType);
    modifyHidden("breakValues", breakValues);
    modifyHidden("disasterInfoValues", disasterInfoValues);
    var state = obtainHidden("state");
    var mode = newMode || obtainHidden("mode");
    var colors = obtainHidden("colors");
    var url = "map.asp"
    url += "?state=" + encodeURIComponent(state)
    url += "&mode=" + encodeURIComponent(mode)
    url += "&pageNO=" + encodeURIComponent(pageNO)
    url += "&disasterInfoType=" + encodeURIComponent(disasterInfoType)
    url += "&colors=" + encodeURIComponent(colors)
    url += "&level=" + encodeURIComponent(level)
    url += "&regionCode=" + encodeURIComponent(regionCode)
    url += "&disasterInfoValues=" + encodeURIComponent(disasterInfoValues)
    url += "&breakValues=" + encodeURIComponent(breakValues);
    url += "&command=" + encodeURIComponent("renderMap");
    url += "&boxSizeX=" + encodeURIComponent(obtainHidden("boxSizeX"));
    url += "&boxSizeY=" + encodeURIComponent(obtainHidden("boxSizeY"));
    url += "&mapType=" + encodeURIComponent(obtainHidden("mapType"));
    locateElement("map").src = url;
    setInfoWindowCSS(disasterInfoType);
    updataLegend(disasterInfoType,level);
    updateInfoDetail();
}

//地图上下级跳转
function changeMapLevel(evt) {
    var maxLevel = window.parent.window.mapParams.maxLevel;
    if (0 == maxLevel) {
        stopDefault(evt);
    }
    else {
        window.parent.timer1 = new Date();
        // 取消上次延时未执行的方法
        clearTimeout(TimeFnOnMouseMove);
        clearTimeout(TimeFnOnClick);
        var evt = window.event || evt,
                  coord_X,
                  coord_Y;

        coord_X = (evt.offsetX == undefined) ? getOffset(evt).X : evt.offsetX;   //获取鼠标指针位置相对于触发事件的对象的X坐标 
        coord_Y = (evt.offsetY == undefined) ? getOffset(evt).Y : evt.offsetY;   //获取鼠标指针位置相对于触发事件的对象的Y坐标
        stopDefault(evt);

        var strUrl = 'handlingMap.asp';
        strUrl += '?state=' + encodeURIComponent(obtainHidden("state"))
        strUrl += "&level=" + encodeURIComponent(obtainHidden("level"))
        strUrl += "&regionCode=" + encodeURIComponent(obtainHidden("regionCode"))
        strUrl += "&x=" + encodeURIComponent(coord_X) + "&y=" + encodeURIComponent(coord_Y)
        strUrl += "&command=" + encodeURIComponent("changeRegion");
        strUrl += "&boxSizeX=" + encodeURIComponent(obtainHidden("boxSizeX"));
        strUrl += "&boxSizeY=" + encodeURIComponent(obtainHidden("boxSizeY"));
        strUrl += "&mapType=" + encodeURIComponent(obtainHidden("mapType"));

        $.ajax({
            type: 'get',
            url: strUrl,
            success: function (str) {
                if (str != "") {
                    var param = eval("(" + str + ")");
                    var newLevel = param.level;
                    var newRegionCode = param.regionCode;
                    var pageNO = obtainHidden("pageNO");
                    var disasterInfoType = obtainHidden("disasterInfoType");
                    var mapType = parseInt(obtainHidden("mapType"));
                    window.parent.changeMapLevel(pageNO, disasterInfoType, newLevel, newRegionCode, mapType); ;
                }
            },
            error: function () {
                alert('获取点击单位出错！');
            }, async: false
        });
    }
}

function changeView(level, regionCode, breakValues, disasterInfoValues) {
    modifyHidden("level", level);
    modifyHidden("regionCode", regionCode);
    modifyHidden("breakValues", breakValues);
    modifyHidden("disasterInfoValues", disasterInfoValues);
    modifyHidden("command", "renderMap");
    modifyHidden("mode", "changeRegion");
    //    refreshMap(pageNO, disasterInfoType, level, regionCode, disasterInfoValues, breakValues,"changeRegion")
    reload();
}

//改变地图大小
function changeMapSize() {
    var oldWidth = $("input#map").width()
    var oldHeight = $("input#map").height()
    var cxmap = window.parent.window.getMapWidth();
    var cymap = window.parent.window.getMapHeight() + 20;    //加20像素覆盖掉地图下面的图标
    //    $("div#aboutMap").width(cxmap)
    //    $("div#aboutMap").height(cymap)
    //    var scaleIncX = cxmap / oldWidth;
    //    var scaleIncY = cymap / oldHeight;
    //    var scaleInc = Math.max(scaleIncX, scaleIncY)
    modifyHidden("boxSizeX", cxmap);
    modifyHidden("boxSizeY", cymap);
    //    modifyHidden("command", "changeMapSize");
    //    reload();
    refreshImage(cxmap, cymap);
}

//刷新图像大小
function refreshImage(cxmap, cymap) {
  $("input#map").hide();
    var level = parseInt(obtainHidden("level"));
    var regionCode = obtainHidden("regionCode")
    var strUrl = "handlingMap.asp"
    strUrl += "?level=" + encodeURIComponent(level)
    strUrl += "&regionCode=" + encodeURIComponent(regionCode);
    strUrl += "&boxSizeX=" + encodeURIComponent(cxmap);
    strUrl += "&boxSizeY=" + encodeURIComponent(cymap);
    strUrl += "&command=" + encodeURIComponent("changeMapSize");
    strUrl += "&mapType=" + encodeURIComponent(obtainHidden("mapType"));
    $.ajax({
        type: 'get',
        url: strUrl,
        success: function(str) {
            if (str != "") {
                modifyHidden("state", str);
                var timestamp = (new Date()).getTime();
                locateElement("map").src = "map.asp?state=" + encodeURIComponent(str) + "&boxSizeX=" + encodeURIComponent(obtainHidden("boxSizeX")) + "&boxSizeY=" + encodeURIComponent(obtainHidden("boxSizeY")) + "&mapType=" + encodeURIComponent(obtainHidden("mapType"))
                 + "&timestamp=" + timestamp; 
            }
        },
        error: function() {
            //            alert('获取点击单位出错！');
        }, async: false
    });
     $("input#map").animate({ width: cxmap, height: cymap }, "fast");
     $("input#map").show();
}


//更新多媒体数据
var infoDetail =""
function updateInfoDetail() {
    if (infoDetail != "") {
        if (infoDetail == "pie" || infoDetail == "bar") {
            createChart(infoDetail);
        }
        else {
            chartHide();
        }
    }
}

// reload page
function reload() {
    document.forms[0].submit();
}

//保存cookie
function saveCookie() {
//    var disasterName = window.parent.window.mapParams.disasterInfoTypes[obtainHidden("disasterInfoType")].name;
//    var imgName = obtainHidden("regionName") + window.parent.window.getPeriod() + disasterName + "分布图";
//    addPublicCookie("imgName", imgName);    //保存地图名称
//    addPublicCookie("imgUrl", locateElement("map").src);    //保存地图图片路径
//    addPublicCookie("mapHeight", obtainHidden("boxSizeY"));
//    addPublicCookie("mapWidth",obtainHidden("boxSizeX"));
//    addPublicCookie("legend", document.getElementById('legend').outerHTML);
    //        addCookie("pageNO", obtainHidden("pageNO"), 720);
    //        addCookie("disasterInfoType", obtainHidden("disasterInfoType"), 720);
    //        addCookie("level", obtainHidden("level"), 720);
    //        addCookie("regionCode", obtainHidden("regionCode"), 720);
    //        addCookie("state", obtainHidden("state"), 720);
    //        addCookie("breakValues", obtainHidden("breakValues"), 720);
    //        addCookie("colors", obtainHidden("colors"), 720);
}

//获取当前页号
function getCurPageNO() {
    return obtainHidden("pageNO");
}

//获取地图级别
function getMapLevel() {
    return parseInt(obtainHidden("level"));
}

//获取单位代码
function getRegionCode() {
    return obtainHidden("regionCode");
}

//获取单位名称
function getRegionName() {
    return obtainHidden("regionName");
}

//获取灾情信息类型
function getDisasterInfoType() {
    return obtainHidden("disasterInfoType")
}

//获取图像路径
function getImgUrl() {
    return locateElement("map").src;
}

//获取图例元素
function getLegend() {
    return locateElement("legend").outerHTML;
}