var myInfoWindowVisible = true;  //信息窗口可见性，在右键菜单弹出时修改为false

function getMediaWindowDisplay() {
    if ($(".zoomoutmenu").css("display") == "none") 
    {
        return false;
    }
    else 
    {
        return true;
    }
}

//隐藏信息窗
function hideWindow()
{
    $(".mapinfo").hide();
}

//创建信息窗
function createInfoWindow() {
    $(".mapinfo").hide();
    if ($(".mapinfo").html() == "") {
        var strInnerHtml = '<span><ul>';
        strInnerHtml += '<li><a></a></li>';
        for (type in window.parent.window.mapParams.disasterInfoTypes) {
            strInnerHtml += '<li><a></a></li>';
        }
        strInnerHtml += '</ul></span>';
        $(".mapinfo").append(strInnerHtml);
        setInfoWindowCSS(obtainHidden("disasterInfoType"))
    }
}

//设置信息窗样式
function setInfoWindowCSS(disasterInfoType) {
    $('.mapinfo li:even').css({ 'color': '#069', 'font-weight': 'normal' })
    $('.mapinfo li:odd').css({ 'color': '#060', 'font-weight': 'normal' });
    $('.mapinfo li:eq(0)').css({ 'font': '14px 宋体', 'font-weight': 'bolder', 'margin-top': '10px', 'border-bottom': '2px white solid' })
    var mapParams = window.parent.window.mapParams;
    if (mapParams.mapType != 3) {
        $('.mapinfo li:eq(' + mapParams.disasterInfoTypes[disasterInfoType].seq + ')').css({ 'color': '#d00', 'font-weight': 'bold' })
    }
}

//在鼠标所在坐标单位的信息详情
function showInfo(evt) {
    if (myInfoWindowVisible) {
        // 取消上次延时未执行的方法
        clearTimeout(TimeFnOnMouseMove);
        var coord = getCoordinate(evt, true);
        //    //单击事件执行延时
        //   TimeFnOnClick = setTimeout("show(" + coord_X + "," + coord_Y + ")", 10);

        //    highlightSelectedUnit(coord_X, coord_Y);
        locateWindow(".mapinfo", "input#map.map", coord.X, coord.Y, 5);
        //    showInfoWindow(coord_X, coord_Y);
        TimeFnOnMouseMove = setTimeout("showInfoWindow(" + coord.X + "," + coord.Y + ")", 20);
    }
}

//获取鼠标指针位置相对于触发事件对象的坐标
function getCoordinate(evt, stopDefaultEvt) {
    evt = window.event || evt;
    coord_X = getOffsetX(evt);    //获取鼠标指针位置相对于触发事件的对象的X坐标
    coord_Y = getOffsetY(evt);   //获取鼠标指针位置相对于触发事件的对象的Y坐标
    //将x、y坐标转为json格式
    Coord = {
        X: coord_X,
        Y: coord_Y
    };

    if (stopDefaultEvt)
        stopDefault(evt);  //阻止浏览器默认行为
    return Coord;
}

//高亮选中单位
function highlightSelectedUnit(coord_X, coord_Y) {
    var url = "map.asp?state=" + encodeURIComponent(obtainHidden("state")) + "&command=" + "" + "&mode=" + "info" + "&x=" + coord_X + "&y=" + coord_Y
    + "&boxSizeX=" + encodeURIComponent(obtainHidden("boxSizeX")) + "&boxSizeY=" + encodeURIComponent(obtainHidden("boxSizeY"))
     + "&mapType=" + encodeURIComponent(obtainHidden("mapType"));
    locateElement("map").src = url;
}

//定位窗口位置(元素选择依据、父元素选择器、x坐标、y坐标、偏移量)
function locateWindow(selector, parentSelector, coord_X, coord_Y, offset) {
    var xOffset = parseFloat(coord_X) + offset, yOffset = parseFloat(coord_Y) + offset;
    var mapWidth = $(parentSelector).width();
    var mapHeight = $(parentSelector).height();
    var windowWidth = $(selector).width();
    var windowHeight = $(selector).height();

    if (xOffset > mapWidth - windowWidth) xOffset -= windowWidth + 2 * offset;
    if (yOffset > mapHeight - windowHeight) yOffset -= windowHeight + 2 * offset;
    $(selector).css({ 'left': xOffset + 'px', 'top': yOffset + 'px' });
}

//显示信息窗
function showInfoWindow(coord_X, coord_Y) {
    var selectedUnitCode = getSelectedUnitCode(coord_X, coord_Y);
    if (selectedUnitCode == "") {
        $(".mapinfo").hide();
    }
    else {
        $(".mapinfo").show();
        var infoArr = new Array();
        var disasterInfo = window.parent.getGlobalDisasterInfo();
        var singleUnitDisasterInfo = getSingleUnitDisasterInfo(disasterInfo, selectedUnitCode);
        var mapParams = window.parent.window.mapParams;
        if (!$.isEmptyObject(singleUnitDisasterInfo)) {
            infoArr.push('地区：' + singleUnitDisasterInfo["UnitName"]);
            for (type in mapParams.disasterInfoTypes) {
                infoArr.push(mapParams.disasterInfoTypes[type].name + '：' +
                singleUnitDisasterInfo[type] + mapParams.disasterInfoTypes[type].unit);
            }
        }
        else {
            infoArr.push("该地区无灾情数据");
            for (type in mapParams.disasterInfoTypes) {
                infoArr.push(mapParams.disasterInfoTypes[type].name + '：  无');
            }
        }

        for (var i = 0; i < infoArr.length; i++) {
            $('.mapinfo li:eq(' + i + ') a').html(infoArr[i]);
        }
    }
}

//从灾情信息中获取某单位全部灾情数据
//disasterInfo 灾情信息
//unitCode 单位代码
function getSingleUnitDisasterInfo(disasterInfoObj, unitCode) {
    var singleUnitDisasterInfo = {};
    if (!$.isEmptyObject(disasterInfoObj)) {
        if (disasterInfoObj[unitCode]) {
            singleUnitDisasterInfo = disasterInfoObj[unitCode];
        }
    }
    return singleUnitDisasterInfo;
}

//获取x、y坐标所在单位
function getSelectedUnitCode(coord_X, coord_Y) {
    var selectedUnitCode = "";
    var strUrl = 'handlingMap.asp';
    strUrl += '?state=' + encodeURIComponent(obtainHidden("state"))
    strUrl += "&level=" + encodeURIComponent(obtainHidden("level"))
    strUrl += "&regionCode=" + encodeURIComponent(obtainHidden("regionCode"))
    strUrl += "&x=" + encodeURIComponent(coord_X) + "&y=" + encodeURIComponent(coord_Y)
    strUrl += "&command=" + encodeURIComponent("select");
    strUrl += "&boxSizeX=" + encodeURIComponent(obtainHidden("boxSizeX"));
    strUrl += "&boxSizeY=" + encodeURIComponent(obtainHidden("boxSizeY"));
    strUrl += "&mapType=" + encodeURIComponent(obtainHidden("mapType"));
    $.ajax({
        type: 'get',
        url: strUrl,
        success: function(str) {
            selectedUnitCode = str;
        },
        error: function() {
            //        alert('获取行政单位代码出错！');
        }, async: false
    });
    return selectedUnitCode;
}

//对map元素绑定此右键菜单
function bindingMapContextmenu() {

    $(".map_picture").html("");
    $('input#map').contextMenu('mapMenu', {
        //菜单项样式
        itemStyle: {
            fontFamily: 'verdana',
            border: 'none',
            padding: '1px',
            fontSize: '16px',
            paddingLeft: '10px'
        },

        //重写菜单绑定事件
        bindings:
          {
              'unitName': function(t) {

              },
              'detailInfo': function(t) {
                  alert('Trigger was ' + t.id + '\nAction was Open');
              },
              'picture': function(t) {
                  showMedia("picture");
              },
              'video': function(t) {
                  showMedia("video");
              }
          },

        //重写onContextMenu事件
        onContextMenu: function(e) {
            var coord = getCoordinate(e, true); //获取鼠标指针位置相对于触发事件对象的坐标
            var selectedUnitCode = getSelectedUnitCode(coord.X, coord.Y); //获取x、y坐标所在单位
            if (selectedUnitCode == "") {
                return false;
            }
            else {
                var disasterInfo = window.parent.getGlobalDisasterInfo();
                var singleUnitDisasterInfo = getSingleUnitDisasterInfo(disasterInfo, selectedUnitCode);
                if (!$.isEmptyObject(singleUnitDisasterInfo)) {
                    hideWindow();
                    clearTimeout(TimeFnOnMouseMove);  // 取消上次延时未执行的方法
                    myInfoWindowVisible = false;
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        //        ,
        //        //重写onShowMenu事件
        //        onShowMenu: function(e, menu) {
        //            return menu;
        //        }
    });
}

function showMedia(type) {
    calChartPos(type);
    if (type == "picture") {
        $("div#divChart").append('<iframe class="iframeinfo" frameborder="0"  src="MMControl/Picture.aspx" style="overflow:hidden ;width:100%; height:100%;padding:0px; margin:0px; border:0px;z-index: 12"></iframe> ')
    }
    else if (type == "video")
    {
        $("div#divChart").append('<iframe class="iframeinfo" frameborder="0"   src="MMControl/Movie.aspx" style="overflow:hidden ;width:100%; height:100%;padding:0px; margin:0px; border:0px;z-index: 12"></iframe> ')
                  //                newWin("http://localhost/flv/8.6下午(刚刚)长沙突降暴雨后我家门口的景象.flv", 480, 320, "灾情视频"); //摄像0015.3gp    0001.音悦台-ECHO.mp4 0001.音悦台-哥哥就是我的Style.mp4
    }
}

function newWin(videoPath, width, height, title) {
    var url = "disasterVideo.aspx";
    url += '?videoPath=' + encodeURIComponent(videoPath);
    url += '&width=' + encodeURIComponent(width);
    url += '&height=' + encodeURIComponent(height);
    url += '&title=' + encodeURIComponent(title);
    var windowWidth = width + 30;
    var windowHeight = height + 30;
    var param = 'height=' + windowHeight + ', width=' + windowWidth +
     ', top=300,left=350, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no,z-look=yes'
    window.open(url, '', param)
}

function shieldMapClickEvent() {
    $("input#map").click(function(e) {
        // 取消上次延时未执行的方法
        clearTimeout(TimeFnOnMouseMove);
        clearTimeout(TimeFnOnClick);
        
//        executeClickEvent(e);      //执行单击事件
        stopDefault(e);
    });
}

var zoomoutmenuFirst = true;
function executeClickEvent(evt) {
    if (getMediaWindowDisplay() == false) {
        hideWindow();
        myInfoWindowVisible = false;
        TimeFnOnClick = setTimeout('$(".zoomoutmenu").show();', 200);
        if (zoomoutmenuFirst == true) {
            zoomoutmenuFirst = false;
            var iframeWidth = parseInt($("input#Map").width() * 0.5);
            var iframeHeight = parseInt($("input#Map").height() * 0.5);
            var iframeMovieURL = "MMControl/Movie.aspx?width=" + iframeWidth + "&height=" + iframeHeight;
            var iframePieChartURL = "MMControl/PieChart.aspx?width=" + iframeWidth + "&height=" + iframeHeight;
            var iframeBarChartURL = "MMControl/BarChart.aspx?width=" + iframeWidth + "&height=" + iframeHeight;
            $("iframe#iframeMovie").attr("src", iframeMovieURL);
            $("iframe#iframePieChart").attr("src", iframePieChartURL);
            $("iframe#iframeBarChart").attr("src", iframeBarChartURL);
        }
    }
    else {
        myInfoWindowVisible = true;
        $(".zoomoutmenu").hide();
    }
}
