/*设置从数据库获取数据时灾情类型对应的索引（灾情类型索引从1开始，0是单位名称的索引）、单位、名称、分类间隔值；
分类间隔值breakValues对象是一个多维数组，可以分别为省市县乡设置不同分类间断值，数组维数和颜色数组维数对应与地图级别对应*/
var DisasterInfoTypes1 = {
    "SZRK": { "seq": 1, "index": 1, "name": "受灾人口", "unit": "万人", "fixed": 4, "breakValues": [[20, 40, 60, 120], [20, 40, 60, 120], [20, 40, 60, 120]] },
    "SWRK": { "seq": 2, "index": 3, "name": "死亡人口", "unit": "人", "fixed": 0, "breakValues": [[1, 5, 10, 50], [1, 5, 10, 50], [1, 5, 10, 50]] },
    "SZRKR": { "seq": 3, "index": 7, "name": "失踪人口", "unit": "人", "fixed": 0, "breakValues": [[1, 5, 10, 50], [1, 5, 10, 50], [1, 5, 10, 50]] },
    "ZYRK": { "seq": 4, "index": 8, "name": "转移人口", "unit": "万人", "fixed": 4, "breakValues": [[0.01, 0.1, 1, 10], [0.01, 0.1, 1, 10], [0.01, 0.1, 1, 10]] },
    "SHMJXJ": { "seq": 5, "index": 2, "name": "受灾面积", "unit": "千公顷", "fixed": 4, "breakValues": [[20, 100, 200, 600], [20, 100, 200, 600], [20, 100, 200, 600]] },
    "DTFW": { "seq": 6, "index": 4, "name": "倒塌房屋", "unit": "万间", "fixed": 4, "breakValues": [[0.01, 0.05, 0.1, 1], [0.01, 0.05, 0.1, 1], [0.01, 0.05, 0.1, 1]] },
    "ZJJJZSS": { "seq": 7, "index": 5, "name": "直接经济损失", "unit": "亿元", "fixed": 4, "breakValues": [[0.1, 0.5, 1, 10], [0.1, 0.5, 1, 10], [0.1, 0.5, 1, 10]] },
    "SLSSZJJJSS": { "seq": 8, "index": 6, "name": "水利经济损失", "unit": "亿元", "fixed": 4, "breakValues": [[0.5, 1, 5, 10], [0.5, 1, 5, 10], [0.5, 1, 5, 10]] }
}

var DisasterInfoTypes2 = {
    "SZRK": { "seq": 1, "index": 1, "name": "受灾人口", "unit": "万人", "fixed": 4, "breakValues": [[20, 40, 60, 120], [20, 40, 60, 120], [20, 40, 60, 120]] },
    "SHMJXJ": { "seq": 2, "index": 2, "name": "受灾面积", "unit": "千公顷", "fixed": 4, "breakValues": [[20, 100, 200, 600], [20, 100, 200, 600], [20, 100, 200, 600]] },
    "SWRK": { "seq": 3, "index": 3, "name": "死亡人口", "unit": "人", "fixed": 0, "breakValues": [[1, 5, 10, 50], [1, 5, 10, 50], [1, 5, 10, 50]] },
    "DTFW": { "seq": 4, "index": 4, "name": "倒塌房屋", "unit": "万间", "fixed": 4, "breakValues": [[0.01, 0.05, 0.1, 1], [0.01, 0.05, 0.1, 1], [0.01, 0.05, 0.1, 1]] },
    "ZJJJZSS": { "seq": 5, "index": 5, "name": "经济损失", "unit": "亿元", "fixed": 4, "breakValues": [[0.1, 0.5, 1, 10], [0.1, 0.5, 1, 10], [0.1, 0.5, 1, 10]] },
    "SMXJT": { "seq": 6, "index": 6, "name": "骨干交通中断历时", "unit": "天", "fixed": 2, "breakValues": [[0.5, 1, 10, 20], [0.5, 1, 10, 20], [0.5, 1, 10, 20]] },
    "GCYMLS": { "seq": 7, "index": 7, "name": "城市区淹没历时", "unit": "天", "fixed": 2, "breakValues": [[0.5, 1, 5, 50], [0.5, 1, 5, 50], [0.5, 1, 5, 50]] }
}

/*颜色对象是一个多维数组，可以分别为省市县乡设置不同渲染颜色，与地图级别对应。前5个颜色用于渲染4个分类间隔值划分出的5类单位，
倒数第2个颜色用于渲染无数据单位，倒数第一个颜色用于渲染非查看区域单位*/
var Colors1 = [["#9ABA18", "#BAA404", "#DA7804", "#F04000", "#FF0000", "#C2B8AE", "#EEEEEE"],
["#9ABA18", "#BAA404", "#DA7804", "#F04000", "#FF0000", "#C2B8AE", "#EEEEEE"],
["#9ABA18", "#BAA404", "#DA7804", "#F04000", "#FF0000", "#C2B8AE", "#EEEEEE"]];

//灾情分布地图配置
mapParams0 = {
    "mapContainerName": "map",   //地图容器Iframe的id和name
    "infoTableContainer": "infoTable",    //地图渲染数据表div容器名称
    "adminRanks": [{ "rank": 2 }, { "rank": 3}],     //地图图层级别，全国地图级别为1，省级地图为2，市级地图为3, { "rank": 3}

    //"dataAshxSrc": "../../MapServer/GeneralProcedure/getvirtualData.ashx",  //获取灾情数据的一般处理程序路径
    //"dataAshxSrc": "../../GeneralProcedure/WBH/getDataForMap.ashx",  //获取灾情数据的一般处理程序路径
    "mapControlSrc": "../../MapServer/mapControl.asp",
    "mapType": 0
}
mapParams0.disasterInfoTypes = DisasterInfoTypes1;
mapParams0.colors = Colors1;

//灾情分布地图配置
mapParams1 = {
    "mapContainerName": "map",   //地图容器Iframe的id和name
    "infoTableContainer": "infoTable",    //地图渲染数据表div容器名称
    "adminRanks": [{ "rank": 2 }, { "rank": 3}],     //地图图层级别，全国地图级别为1，省级地图为2，市级地图为3, { "rank": 3}
    "mapControlSrc": "../../MapServer/mapControl.asp",
    "mapType": 1
}
mapParams1.disasterInfoTypes = DisasterInfoTypes1;
mapParams1.colors = Colors1;

//灾情分布-流域地图配置
mapParams2 = {
    "mapContainerName": "map",   //地图容器Iframe的id和name
    "infoTableContainer": "infoTable",    //地图渲染数据表div容器名称
    "adminRanks": [{ "rank": 2 }, { "rank": 3}],     //地图图层级别，全国地图级别为1，省级地图为2，市级地图为3, { "rank": 3}
    //"dataAshxSrc": "../../MapServer/GeneralProcedure/getvirtualData.ashx",  //获取灾情数据的一般处理程序路径
    "mapControlSrc": "../../MapServer/mapControl.asp",
    "mapType": 2
}
mapParams2.disasterInfoTypes = DisasterInfoTypes1;
mapParams2.colors = Colors1;

//灾情评估地图配置
mapParams3 = {
    "mapContainerName": "QGDC_MapFrame",   //地图容器Iframe的id和name
    "infoTableContainer": "",    //地图渲染数据表div容器名称（为空不显示信息表）
    "adminRanks": [{ "rank": 2 }, { "rank": 3}],     //地图图层级别，全国地图级别为1，省级地图为2，市级地图为3
    //"dataAshxSrc": "../../../GeneralProcedure/FM/GetDisasterInfo.ashx",  //获取灾情数据的一般处理程序路径

    "mapControlSrc": "../../MapServer/mapControl1.asp",
    "mapType": 3
}
mapParams3.disasterInfoTypes = DisasterInfoTypes2;
mapParams3.colors = Colors1;

mapParamsArr = [mapParams0, mapParams1, mapParams2, mapParams3];
//获取地图参数
function getMapParams1(mapType) {
    var mapParams = mapParamsArr[mapType];
    return mapParams;
}

////灾情分布-流域地图配置(老版本)
//mapParams2 = { 
//    "mapContainerName": "map",   //地图容器Iframe的id和name
//    "infoTableContainer": "infoTable",    //地图渲染数据表div容器名称
//    "adminRanks": [{ "rank": 2 }, { "rank": 3}],     //地图图层级别，全国地图级别为1，省级地图为2，市级地图为3
//    "topRegion": { "code": "2", "name": "长江流域" },          //最上一级区域的单位名称和代码

//    "dataAshxSrc": "../../MapServer/GeneralProcedure/getvirtualData.ashx",  //获取灾情数据的一般处理程序路径
//    "mapType": 2
//}
//mapParams2.disasterInfoTypes = DisasterInfoTypes1;
//mapParams2.colors = Colors1;