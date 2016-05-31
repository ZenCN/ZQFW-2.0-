//打印地图
function pirntMap() {
    var frameMap = window.frames[mapParams.mapContainerName]
    var regionName = frameMap.getRegionName()   //获取当前单位名称
    var imgName = ""
    if (mapParams.mapType == 3) {
        imgName = regionName + getPeriod() + "灾情评估分布图";
    }
    else {
        var disasterInfoType = frameMap.getDisasterInfoType()   //获取当前灾情信息类型
        var disasterName = mapParams.disasterInfoTypes[disasterInfoType].name;
        imgName = regionName + getPeriod() + disasterName + "分布图";
    }
    var imgUrl = frameMap.getImgUrl()  //获取当前地图图片路径
    var legend = frameMap.getLegend()  //获取当前图例元素
    var mapWidth = getMapWidth()  //获取地图容器宽度
    var mapHeight = getMapHeight()  //获取地图容器高度
    modifyHidden("hidImgName", encodeURIComponent(imgName));
    modifyHidden("hidImgUrl", encodeURIComponent(imgUrl));
    modifyHidden("hidLegend", encodeURIComponent(legend));
    modifyHidden("hidMapWidth", encodeURIComponent(mapWidth))
    modifyHidden("hidMapHeight", encodeURIComponent(mapHeight))
    document.getElementById('exportMap').click();
    //               window.location.href = "../../GeneralProcedure/WBH/exportMap.ashx";
    //               addCookie("imgName", imgName);    //保存地图名称
    //               addCookie("imgUrl", imgUrl);    //保存地图图片路径
    //               addCookie("mapHeight", mapHeight);
    //               addCookie("mapWidth", mapWidth);
    //               addCookie("legend", legend);
    //                var url = 'printMap.aspx';
    //               var w = window.open(url, "灾情分布图");
    //               w.print();
}