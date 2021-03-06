﻿<!-- #include file="mapParams/mapParams.asp" -->
<script language ="javascript"  runat="server">
    // return "&#160;" for empty text and use server encoder otherwise
    function encode(text) {
        return (text != "") ? Server.HTMLEncode(text) : "&#160;";
    }

    // locate parameter in form
    function parameterForm(name, startup) {
        return (Request.Form(name).Count > 0) ? Request.Form(name)(1) : startup;
    }

    // locate parameter in query string
    function parameterQueryString(name, startup) {
        return (Request.QueryString(name).Count > 0) ? Request.QueryString(name)(1) : startup;
    }

    // locate parameter in cookies
    function parameterCookies(name, startup) {
        return (Request.Cookies(name).item != "") ? Request.Cookies(name).item : startup;
    }
    
     function trim(str) { //删除左右两端的空格
        return str.replace(/(^\s*)|(\s*$)/g, "");
    }

    function ltrim(str) { //删除左边的空格
        return str.replace(/(^\s*)/g, "");
    }

    function rtrim(str) { //删除右边的空格
        return str.replace(/(\s*$)/g, "");
    }


    function trimEnd(str) { //去掉字符串结束符\0
        return str.replace(/\0/g, "");
    }

    // obtain a value from XML document
    function xmlValue(doc, xpath) {
        var node = doc.selectSingleNode(xpath);
        if (node != null)
            return node.text;
        return null;
    }
    
    function jsontoxml() {
        var xmlDoc = Server.CreateObject("Msxml2.DOMDocument.3.0");
        xmlDoc.async = false;
        xmlDoc.load(Server.MapPath("mapParams.xml"));
        var aaaa = xmlToJson(xmlDoc)
        var ss = aaaa.mapParams.hunan.adminRanks[0].rank
    }
    
    //xml转Json
        function xmlToJson(xml) {
        // Create the return object 
        var obj = {};

        if (xml.nodeType == 1) { // element 
            // do attributes 
            if (xml.attributes.length > 0) {
                for (var j = 0; j < xml.attributes.length; j++) {
                    var attribute = xml.attributes.item(j);
                    obj[attribute.nodeName] = attribute.nodeValue;
                }
            }
        } else if (xml.nodeType == 3) { // text 
            obj = xml.nodeValue;
        }

        // do children 
        if (xml.hasChildNodes()) {
            for (var i = 0; i < xml.childNodes.length; i++) {
                var item = xml.childNodes.item(i);
                var nodeName = item.nodeName;
                if (typeof (obj[nodeName]) == "undefined") {
                    obj[nodeName] = xmlToJson(item);
                } else {
                    if (typeof (obj[nodeName].length) == "undefined") {
                        var old = obj[nodeName];
                        obj[nodeName] = [];
                        obj[nodeName].push(old);
                    }
                    obj[nodeName].push(xmlToJson(item));
                }
            }
        }
        return obj;
    }; 
    

 // extract parameter from the state string
    function extractStatePar(state, name) {
        var stateItems = state.split("$");
        if (stateItems == null)
            return "";

        // locate parameter
        for (var stateIndex = 0; stateIndex < stateItems.length; stateIndex++) {
            var stateItem = stateItems[stateIndex];
            if (stateItem != null && stateItem.indexOf(name + "=") == 0)
                return stateItem.substr(name.length + 1, stateItem.length - name.length - 1);
        }
        return "";
    }

    // replace parameter within the state string
    function replaceStatePar(state, name, value) {
        var stateItems = state.split("$");
        if (stateItems == null)
            return name + "=" + value + "$";
        var found = false;

        // locate parameter
        for (var stateIndex = 0; stateIndex < stateItems.length; stateIndex++) {
            var stateItem = stateItems[stateIndex];
            if (stateItem != null && stateItem.indexOf(name + "=") == 0) {
                stateItems[stateIndex] = name + "=" + value;
                found = true;
            }
        }
        return stateItems.join("$") + "$" + (found ? "" : (name + "=" + value + "$"));
    }


    function getConfigOpts(mapParams, mapWidth, mapHeight) {
        var strConfigOpts = getConfigOptsFromFile(mapParams);
        strConfigOpts = replaceConfigOpt(strConfigOpts, "cx", mapWidth);
        strConfigOpts = replaceConfigOpt(strConfigOpts, "cy", mapHeight);
        return strConfigOpts;
        //return (Request.Cookies("configOpts").item != "") ? Request.Cookies("configOpts").item : getConfigOptsFromFile();
    }


    //从地图服务配置文件获取配置参数字符串
    function getConfigOptsFromFile(mapParams) {
        var fso = new ActiveXObject("Scripting.FileSystemObject");
        var configFilePath = Server.MapPath("config.txt")
        var openFile = fso.OpenTextFile(configFilePath, 1, true);
        var configOpts = openFile.ReadAll();
        var component = mapParams.component
        var file = Server.MapPath(mapParams.mapFile);
        configOpts = replaceConfigOpt(configOpts, "component", component); //设置地图服务名称
        configOpts = replaceConfigOpt(configOpts, "file", file);  //设置地图文件路径
        return configOpts;
    }

    // extract parameter from the configOpts string
    function extractConfigOpt(configOpts, name) {
        var configOptArr = configOpts.split("\n");
        if (configOptArr == null)
            return "";
        // locate parameter
        for (var i = 0; i < configOptArr.length; i++) {
            var configOpt = configOptArr[i];
            var optKey = trim(configOpt.split("=")[0]);
            if (optKey == name) {
                var optValue = trim(configOpt.split("=")[1]);
                return optValue;
            }
        }
        return "";
    }

    // replace parameter within the configOpts string
    function replaceConfigOpt(configOpts, name, value) {
        var configOptArr = configOpts.split("\n");
        if (configOptArr == null)
            return name + "=" + value;
        var found = false;

        // locate parameter
        for (var i = 0; i < configOptArr.length; i++) {
            var configOpt = configOptArr[i];
            var optKey = trim(configOpt.split("=")[0]);
            if (optKey == name) {
                configOptArr[i] = name + "=" + value;
                found = true;
            }
        }
        return configOptArr.join("\n") + (found ? "" : ("\n" + name + "=" + value));
    }

    function resetMapSize(strConfigOpts,mapWidth,mapHeight) {
        strConfigOpts = replaceConfigOpt(strConfigOpts, "cx", mapWidth);
        strConfigOpts = replaceConfigOpt(strConfigOpts, "cy", mapHeight);
        return strConfigOpts;
    }
    
    //调整图像质量
    //adjustPar 调整参数
    function adjustImageQuality(adjustPar) {
        mapserver.CX = mapserver.CX * adjustPar;
        mapserver.CY = mapserver.CY * adjustPar; ;
        mapserver.ViewScaleX = mapserver.ViewScaleX / adjustPar;
        mapserver.ViewScaleY = mapserver.ViewScaleY / adjustPar
    }
    
    function calStateByOpts(configOpts)
    {
        var orgMapWidth = extractConfigOpt(configOpts, "cx");
        var orgMapHeight = extractConfigOpt(configOpts, "cy");
        if (orgMapWidth != boxSizeX | orgMapHeight != boxSizeY) {
            var viewport = extractStatePar(state, "viewport");
            if (viewport != null && viewport != "") {

                // split viewport into pars
                var viewportPars = viewport.split(",");
                if (viewportPars != null && viewportPars.length == 4) {
                    var scaleX = parseFloat(viewportPars[0]);
                    var scaleY = parseFloat(viewportPars[1]);

                    // replace scaleX and scaleY with scale imposed by box
                    var scaleIncX = orgMapWidth / boxSizeX;
                    var scaleIncY = orgMapHeight / boxSizeY;
                    var scaleInc = Math.max(scaleIncX, scaleIncY)
                    scaleX = scaleX * scaleInc;
                    scaleY = scaleY * scaleInc;

                    viewportPars[0] = scaleX;
                    viewportPars[1] = scaleY;
                    viewport = viewportPars.join(",");

                    // replace viewport parameter within state string
                    state = replaceStatePar(state, "viewport", viewport);
                }
            }
        }
    }

    function removeQuery(msvr,sql) {
        var components = msvr.Document.ComponentSet;
        nQuery = components.ItemByID(sql.id);
        if (nQuery >= 0)
            components.Remove(nQuery);
        sql = null;
    }


    //根据坐标获取地图单位信息
    function getAreaByCoord(msvr,mapParams, mapLevel, x, y) {
        var mapX = msvr.MapX(x); var mapY = msvr.MapY(y); //由获取地图坐标
        var layer = mapParams.adminRanks[mapLevel];
        var code = layer.fields["code"];
        var superiorCode = layer.fields["superiorCode"];
        var drawing = layer.drawing;
        var component = mapParams.component;
        var sql = msvr.Document.NewQuery("");
        var mapName = '"' + component + '"'
        var query = "SELECT [ID],[" + code + "],[" + superiorCode + "] FROM [" + drawing +
    "] WHERE Contains(Geom([ID]), AssignCoordSys(NewPoint(" + mapX + "," + mapY + "),CoordSys(" + mapName + " AS COMPONENT)))";
        sql.Text = query;
        if (sql.QueryType == 9)	// QueryTypeSelect
        {
            sql.Run();
            var table = sql.Table;
            var rows = table.RecordSet;
            if (rows.Count < 1)
                return null;
            else {
                var row = rows(0);
                var id = row.Data("ID");
                var code = row.Data(layer.fields["code"]);
                var superiorCode = row.Data(layer.fields["superiorCode"]);
                //var name = trimEnd(row.Data(layer.fields["name"])) //+ '","Name":"' + name
                var jsonStr = '{"Code":"' + code + '","ID":' + id + ',"SuperiorCode":"' + superiorCode + '"}';
                removeQuery(msvr, sql);  //移除sql文件
                return eval("(" + jsonStr + ")");
            }
        }
        else {
            return null;
        }
    }

    //根据地图单位代码获取单位信息
    function getValueByCode(msvr,mapParams, mapLevel, unitCode, valueName) {
        var layer = mapParams.adminRanks[mapLevel];   //获取行政区域相关参数
        var code = layer.fields["code"];
        var name = layer.fields["name"];
        var superiorCode = layer.fields["superiorCode"];
        var table = layer.table;
        var query = "SELECT [" + name + "],[" + superiorCode + "] FROM [" + table + "] WHERE [" + code + "]=" + '"' + unitCode + '"';
        var sql = msvr.Document.NewQuery("");
        sql.Text = query;
        if (sql.QueryType == 9)	// QueryTypeSelect
        {
            sql.Run();
            var table = sql.Table;
            var rows = table.RecordSet;
            if (rows.Count < 1)
                return "";
            else {
                var row = rows(0);
                var value = row.Data(layer.fields[valueName]);
                removeQuery(msvr, sql);  //移除sql文件
                return value;
            }
        }
        else {
            return "";
        }
    }

    //根据地图单位代码获取单位名称
    function getNameByCode(msvr,mapParams, mapLevel, unitCode) {
        if (mapLevel < 0) {
            return mapParams.topRegion.name;
        }
        else {
            return getValueByCode(msvr, mapParams,mapLevel, unitCode, "name");      //根据地图单位代码获取单位名称
        }
    }

    //获取根据单位代码获取上级单位代码
    function getSuperiorCode(msvr,mapParams, mapLevel, unitCode) {
        if (mapLevel < 1) {
            return mapParams.topRegion.code;
        }
          else {
            return getValueByCode(msvr,mapParams, mapLevel, unitCode, "superiorCode");      //根据地图单位代码获取上级单位代码
        }
    }

    //根据地图单位代码将地图缩放至区域范围
    function zoomToArea(msvr,mapParams, mapLevel, unitCode) {
            //var query = mapParams.adminRanks[mapLevel - 1].queries["zoomToArea"]
            //mapserver.Query(query, regionCode);
            
        if (mapLevel < 0) {
            return msvr.zoomToFit();
        }
        else {
            var layer = mapParams.adminRanks[mapLevel];   //获取行政区域相关参数
            var code = layer.fields["code"];
            var drawing = layer.drawing;
            var query = "SELECT [ID] FROM [" + drawing + "] WHERE [" + code + "]=" + '"' + unitCode + '"';
            var sql = mapserver.Document.NewQuery("temp");
            sql.Text = query;
            msvr.Query("temp", "")
            removeQuery(msvr, sql);  //移除sql文件
        }
    }

    
        //更新地图状态
    function updateState(mapParams,mapLevel, regionCode) {
        if (mapLevel <= 0) {
            mapserver.ZoomToFit();
        }
        else {
            zoomToArea(mapserver,mapParams, mapLevel-1, regionCode);
            //var mapParams = getMapParams();      //获取地图参数
            //var query = mapParams.adminRanks[mapLevel - 1].queries["zoomToArea"]
            //mapserver.Query(query, regionCode);-->
        }
    }


    //设置图层可见性
    //mapParams:地图相关参数
    //mapType:地图类别（0为直观图，1为市级图，2为流域图）
    //level:地图层次
    function setLayersVisibility(mapParams, mapType, level) {
        var layers = mapserver.Layers;
        var layerArr = layers.split("\n");
        for (var i = 0; i < layerArr.length - 1; i++) {
            mapserver.TurnLayer(layerArr[i], false);
            if (mapType == 1 && layerArr[i] == mapParams.adminRanks[0].drawing) {  //如果地图类别为市级图，地图行政级别为0，且图层名为省级面图层
                mapserver.Component.LayerSet.Item(i).Opacity = 1;   //设置图层透明度为1
            }
            else {
                mapserver.Component.LayerSet.Item(i).Opacity = 100;
            }
        }

        if (mapType == 0) {
            mapserver.TurnLayer(mapParams.adminRanks[level].labels, true);
            mapserver.TurnLayer(mapParams.adminRanks[level].drawing, true);
            if (level > 0) {
                mapserver.TurnLayer(mapParams.adminRanks[level - 1].labels, true)
            }
        }
        else if (mapType == 1) {
            if (level == 0) {
                mapserver.TurnLayer(mapParams.adminRanks[level].line, true);
                mapserver.TurnLayer(mapParams.adminRanks[level].labels, true);
                mapserver.TurnLayer(mapParams.adminRanks[level].drawing, true);
                //mapserver.TurnLayer(mapParams.adminRanks[level + 1].labels, true);
                mapserver.TurnLayer(mapParams.adminRanks[level + 1].drawing, true);
            }
            else {
                mapserver.TurnLayer(mapParams.adminRanks[level].labels, true);
                mapserver.TurnLayer(mapParams.adminRanks[level].drawing, true)
                mapserver.TurnLayer(mapParams.adminRanks[level - 1].labels, true)
            }
        }
        else if (mapType == 2) {
            mapserver.TurnLayer(mapParams.riverBasinLabels, true);
            mapserver.TurnLayer(mapParams.riverBasinLine, true);
            mapserver.TurnLayer(mapParams.adminRanks[level].line, true);
            mapserver.TurnLayer(mapParams.adminRanks[level + 1].drawing, true);
            mapserver.TurnLayer(mapParams.adminRanks[level].labels, true);
        }
        else {
            mapserver.TurnLayer(mapParams.adminRanks[level].labels, true);
            mapserver.TurnLayer(mapParams.adminRanks[level].drawing, true);
        }
    }


    //根据灾情信息渲染地图
    function renderMap(mapParams, mapType, disasterInfoValues, breakValues, colors, level, regionCode) {
        var disasterValueArr = eval("(" + disasterInfoValues + ")");
        var breakValueArr = eval("(" + breakValues + ")");
        var colorArr = eval("(" + colors + ")");

        if ((mapType == 1 || mapType == 2) && level == 0) {
            var layer = mapParams.adminRanks[level + 1]       //获取行政单位参数
        }
        else {
            var layer = mapParams.adminRanks[level]       //获取行政单位参数
        }
        var component = mapserver.Document.ComponentSet(layer.drawing);  //获取当前行政区域面图层
        var table = component.OwnedTable;
        var rows = table.RecordSet;

        component.AreaBackground.SetUniqueValues('FID');   //使用单位代码对行政单位进行唯一值渲染
        var xmlDoc = Server.CreateObject("Msxml2.DOMDocument.3.0");
        xmlDoc.async = false;
        var strXML = component.AreaBackground.ToXML();   //将背景色进行唯一值渲染后的结果导出为xml
        xmlDoc.loadXML(strXML);


        //根据数据库灾情数据重新定义面的背景色
        if (xmlDoc.parseError.errorcode == null) {
            //var codeList = xmlDoc.selectSingleNode("//values").childNodes;
            var colorList = xmlDoc.selectSingleNode("//colors").childNodes;
            for (var i = 0; i < rows.Count; i++) {
                var row = rows(i)
                var superiorCode = row.Data(layer.fields["superiorCode"]);
                if (superiorCode == regionCode || ((mapType == 1 || mapType == 2) && level == 0)) {
                    colorList[i].text = colorArr[colorArr.length - 2];
                    for (var j = 0; j < disasterValueArr.length; j++) {
                        var code = row.Data(layer.fields["code"]);
                        if (code == disasterValueArr[j][0]) {
                            var disasterValue = disasterValueArr[j][1];
                            if (disasterValue < breakValueArr[0]) {
                                colorList[i].text = colorArr[0];
                            }
                            else if (disasterValue < breakValueArr[1]) {
                                colorList[i].text = colorArr[1];
                            }
                            else if (disasterValue < breakValueArr[2]) {
                                colorList[i].text = colorArr[2];
                            }
                            else if (disasterValue < breakValueArr[3]) {
                                colorList[i].text = colorArr[3];
                            }
                            else {
                                colorList[i].text = colorArr[4];
                            }
                            break;
                        }
                    }
                }
                else {
                    colorList[i].text = colorArr[colorArr.length - 1];
                }
            }
        }
        component.AreaBackground.LoadFrom(xmlDoc.xml, true);  //根据修改后的xml渲染行政单位背景色
    }
</script>

