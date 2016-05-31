<script language ="javascript"  runat="server">
    //地图相关参数
    function getMapParams(mapType) {
        //var mapIndex = parameterCookies("mapIndex", "")
        if (mapType == 0 | mapType == 1 | mapType == 2 | mapType == 3) {
            var fileName = (mapType == 3) ? "HeilongjiangMap1.map" : "HeilongjiangMap.map"
            return {
                "adminRanks": [
                //{ "rank": 1, "labels": "Province Labels", "drawing": "Province Drawing", "table": "Province Table",
                //"line": "Province_Line Drawing","lineTable":"Province_Line Table",
                //  "fields": { "code": "CODE", "name": "NAME", "superiorCode": "SUPERIOR_CODE", "adminRank": "ADMIN_RANK", "id": "FID" }
                //}
                //,
                     {"rank": 2, "labels": "City Labels", "drawing": "City Drawing", "table": "City Table",
                     "line": "City_Line Drawing", "lineTable": "City_Line Table",
                     "fields": { "code": "CODE", "name": "NAME", "superiorCode": "SUPERIOR_CODE", "adminRank": "ADMIN_RANK", "id": "FID" }
                 }
                    ,
                     { "rank": 3, "labels": "County Labels", "drawing": "County Drawing", "table": "County Table",
                         "fields": { "code": "CODE", "name": "NAME", "superiorCode": "SUPERIOR_CODE", "adminRank": "ADMIN_RANK", "id": "FID" }
                     }
                ],     //地图图层级别，全国地图级别为1，省级地图为2，市级地图为3
                "topRegion": { "code": "43000000", "name": "湖南省" },
                "file": fileName,
                "component": "Map",
                "riverBasinLabels": "RiverBasin Labels",
                "riverBasinLine": "RiverBasin_Line Drawing"
            };
        }
        else if (mapType == 2) {
            return {
                "adminRanks": [
                    { "rank": 2, "labels": "Liuyu2 Labels", "drawing": "Liuyu2 Drawing", "table": "Liuyu2 Table",
                        "fields": { "code": "CODE", "name": "NAME", "superiorCode": "SUPERIOR_CODE", "adminRank": "ADMIN_RANK", "id": "FID" }
                    },
                    { "rank": 3, "labels": "Liuyu3 Labels", "drawing": "Liuyu3 Drawing", "table": "Liuyu3 Table",
                        "fields": { "code": "CODE", "name": "NAME", "superiorCode": "SUPERIOR_CODE", "adminRank": "ADMIN_RANK", "id": "FID" }
                    }
                ],     //地图图层级别，全国地图级别为1，省级地图为2，市级地图为3
                "topRegion": { "code": "2", "name": "长江流域" },
                "file": "HeilongjiangMap.map",
                "component": "Map 2"
            }
        }
    }
</script>

