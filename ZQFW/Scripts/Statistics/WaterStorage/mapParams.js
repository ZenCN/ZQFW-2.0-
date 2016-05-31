//获取地图参数
function getMapParams(mapType) {
    var mapParams = {} //地图相关参数
    mapParams.mapType = mapType;
    //从xml文件读取地图配置
    var src = "../../MapServer/mapParams/mapParams.xml";
    var xmlDoc = loadXMLDoc(src);
    //设置不同页面/功能地图相关配置
    mapParams.mapContainerName = "map";
    mapParams.infoTableContainer = "";   //地图渲染数据表div容器名称（为空不显示信息表）
    mapParams.mapControlSrc = "../../Scripts/Statistics/WaterStorage/mapControl.asp";
            
    

    //灾情指标配置
    var mapTypeNodes = xmlDoc.getElementsByTagName("mapType");
    var dstNames = []
    for (var i = 0; i < mapTypeNodes.length; i++) {
        if (parseInt(mapTypeNodes[i].getAttribute("type")) == mapType) {
            var strDstNames = getNodeVal(mapTypeNodes[i].getElementsByTagName("disasterInfoTypes")[0]);
            dstNames = strDstNames.split(",");
            break;
        }
    }
    var allDstTypeNodes = xmlDoc.getElementsByTagName("allDstInfoTypes")[0].getElementsByTagName("disasterInfoType");
    var disasterTypes = {};
    for (var i = 0; i < dstNames.length; i++) {
        var dstName = dstNames[i];
        var disasterType = {};
        for (var j = 0; j < allDstTypeNodes.length; j++) {
            if (allDstTypeNodes[j].getAttribute("type") == dstName) {
                var disasterTypeNode = allDstTypeNodes[j];
                disasterType["name"] = getNodeVal(disasterTypeNode.getElementsByTagName("name")[0]);
                disasterType["unit"] = getNodeVal(disasterTypeNode.getElementsByTagName("unit")[0]);
                disasterType["fixed"] = getNodeVal(disasterTypeNode.getElementsByTagName("fixed")[0], "int");
                disasterType["seq"] = i + 1;
                var breakValueNodes = disasterTypeNode.getElementsByTagName("breakValue");
                var breakValues = [];
                for (var k = 0; k < breakValueNodes.length; k++) {
                    var strBreakValues = getNodeVal(breakValueNodes[k]);
                    breakValues.push(strBreakValues.split(","));
                }
                disasterType["breakValues"] = breakValues;
                break;
            }
        }
        disasterTypes[dstName] = disasterType;
    }
    mapParams.disasterInfoTypes = disasterTypes

    //获取地图行政级别
    var level = getNodeVal(xmlDoc.getElementsByTagName("topRegion")[0].getElementsByTagName("level")[0], "int");
        mapParams.adminRanks = [{ "rank": 1 }, { "rank": 2 }, { "rank": 3}];

    //获取地图颜色配置
    var colorNodes = xmlDoc.getElementsByTagName("colors")[0].getElementsByTagName("color");
    var colors = [];
    for (var i = 0; i < colorNodes.length; i++) {
        var color = getNodeVal(colorNodes[i]);
        colors.push(color);
    }
    mapParams.colors = [];
    mapParams.colors.push(colors);
    mapParams.colors.push(colors);
    mapParams.colors.push(colors);

    return mapParams;
}

//获取节点值
function getNodeVal(element, valType) {
    var value;
    if (!window.ActiveXObject)
        value = element.textContent;
    else
        value = element.text;

    if (valType == "int") {
        value = parseInt(value);
    }
    return value;
}

function loadXMLDoc(src) {
    try {
        var xmlHttp = createXMLHttpRequest();
        xmlHttp.open("GET", src, false);
        xmlHttp.send(null);
        var stus = xmlHttp.status;
        if (stus == 200 || stus == 0 || stus == 304) {
            //            return xmlHttp.responseText;
            return xmlHttp.responseXML;
        }
    } catch (ex) {
        alert("error!");
    }
}

var progId, progIds = ["MSXML2.XMLHTTP.6.0", "Msxml2.XMLHTTP.5.0", "Msxml2.XMLHTTP.4.0", "Msxml2.XMLHTTP.3.0", "MSXML2.XMLHTTP", "Microsoft.XMLHTTP"];

var xmlhttp;
//创建xmlhttprequest代码
function createXMLHttpRequest() {
    xmlhttp = null;
    if (window.XMLHttpRequest) {// code for Firefox, Opera, IE7, etc.
        xmlhttp = new XMLHttpRequest();
        if (window.overrideMimeType) {
            xmlHttp.overrideMimeType('text/xml');
        }

    }
    else if (window.ActiveXObject) {// code for IE6, IE5
        if (progId != null) {
            xmlhttp = new ActiveXObject(progId);
        } else {
            for (var i = 0; i < progIds.length; i++) {
                try {
                    xmlhttp = new ActiveXObject(progId = progIds[i]);
                    break;
                } catch (ex) {
                    progId = null;
                }
            }
        }
    }
    return xmlhttp;
}