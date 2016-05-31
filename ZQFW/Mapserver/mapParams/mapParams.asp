<script language ="javascript"  runat="server">
    //从xml文件中获取地图参数
    function getMapParams(mapType) {
        var xmlDoc = Server.CreateObject("Msxml2.DOMDocument.3.0");
        //var xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.async = false;
        var xmlFile = Server.MapPath("mapParams/mapParams.xml");
        xmlDoc.load(xmlFile);
        var mapParams = {};
        //读取地图配置
        if (xmlDoc.parseError.errorcode == null) {
            //地图所属行政单位（使用单位）
            var unitInfos = xmlDoc.selectSingleNode("//topRegion").childNodes;
            var topRegion = {};
            for (var i = 0; i < unitInfos.length; i++) {
                if (unitInfos[i].tagName == "level") {
                    topRegion[unitInfos[i].tagName] = parseInt(unitInfos[i].text);
                }
                else {
                    topRegion[unitInfos[i].tagName] = unitInfos[i].text;
                }
            }
            mapParams.topRegion = topRegion;

            //根据地图类型确定地图文件路径
            var mapTypes = xmlDoc.selectSingleNode("//mapTypes").childNodes;
            for (var i = 0; i < mapTypes.length; i++) {
                var type = parseInt(mapTypes[i].getAttribute("type"));
                if (type == mapType) {
                    var mapCfgNodes = mapTypes[i].childNodes;
                    for (var j = 0; j < mapCfgNodes.length; j++) {
                        if (mapCfgNodes[j].getAttribute("dataType") == "int") {
                            mapParams[mapCfgNodes[j].tagName] = parseInt(mapCfgNodes[j].text);
                        }
                        else {
                            mapParams[mapCfgNodes[j].tagName] = mapCfgNodes[j].text
                        }
                    }
                    break;
                }
            }

            //有效地图图层
            var layers = xmlDoc.getElementsByTagName("layer");
            var ranks = [];
            var startRank = 0;
            switch (mapParams.topRegion.level) {
                case 2:
                    startRank = 1;
                    break;
                case 3:
                    startRank = 2;
                    break;
                default:
                    startRank = 0;
                    break;
            }
            for (var i = startRank; i < layers.length; i++) {
                var layerInfos = layers[i].childNodes;
                var adminRank = transElemsToJson(layerInfos);
                ranks.push(adminRank);
            }
            mapParams.adminRanks = ranks;
        }
        return mapParams;
    }

    function transElemsToJson(Elements) {
        var json = {};
        for (var i = 0; i < Elements.length; i++) {
            if (Elements[i].firstChild.nodeType == 1) {
                json[Elements[i].tagName] = transElemsToJson(Elements[i].childNodes);
            }
            else {
                if (Elements[i].tagName == "rank") {
                    json[Elements[i].tagName] = parseInt(Elements[i].text);
                }
                else {
                    json[Elements[i].tagName] = Elements[i].text;
                }
            }
        }
        return json;
    }
</script>

