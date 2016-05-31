
var m_DisasterInfo = {};    //全局灾情信息，在修改页号和跳转地图范围时更新

function getGlobalDisasterInfo() { //获取全局灾情信息
    return m_DisasterInfo;
}

function setGlobalDisasterInfo(disasterInfo) {  //设置全局灾情信息
    m_DisasterInfo = disasterInfo;
}

//从数据库中获取某页号下某行政级别的某行政单位的下级单位全部的灾害数据
//pageNO 页号
//level 行政级别
//unitCode 单位代码
function getDisasterInfoFromDb(pageNO, mapType, level, unitCode) {
    var data = {};
    if (pageNO != "") {
        //        if (mapParams.mapType == 2) {
        //            data = getVirtualData(pageNO, level, unitCode); //获取虚拟数据
        //        }
        //        else {
        var adminRank = mapParams.adminRanks[level].rank;   //地图行政级别
        if (level == 0) {
            unitCode = "";
        }
        else if (level == 1) {   //level大于1时，行政级别减一级
            adminRank = adminRank - 1;
        }

        if (adminRank == 1) {    //地图中全国图层为1，数据库中国家防总为0
            adminRank = 0;
        }

        $.ajax({
            type: 'post',
            data: { pageNO: pageNO, level: adminRank, unitCode: unitCode, mapType: mapType },
            url: '/Statistics/GetDataForMap',
            success: function (jsonObject) {
                data = jsonObject;
            },
            error: function () {
               //alert('获得灾害类型数据出错');
            }, async: false
        });
    }
    return data;
}


//从灾情信息中获取某类型灾情数据以及相应行政单位代码
 //disasterInfo 灾情信息
 //disasterInfoType 灾情数据类型
function getSingleDisasterTypeData(disasterInfoObj, disasterInfoType) {
    var singleDisasterTypeDataArr = [];
    if (!$.isEmptyObject(disasterInfoObj)) {
        for (unitCode in disasterInfoObj) {
            var singleDisasterTypeData = [];
            singleDisasterTypeData[0] = unitCode;
            singleDisasterTypeData[1] = disasterInfoObj[unitCode][disasterInfoType]; //向数组添加灾情数据类型及对应值
            singleDisasterTypeDataArr.push(singleDisasterTypeData);
        }
    }
    return Obj2Str(singleDisasterTypeDataArr);
}

//从灾情信息中获取某类型灾情数据以及相应行政单位名称
//disasterInfo 灾情信息
//disasterInfoType 灾情数据类型
function getInfoTableData(disasterInfoObj, disasterInfoType) {
    var singleDisasterTypeDataArr = [];
    if (!$.isEmptyObject(disasterInfoObj)) {
        for (unitCode in disasterInfoObj) {
            var singleDisasterTypeData = [];
            singleDisasterTypeData[0] = disasterInfoObj[unitCode][0];  //向数组添加单位名称
            singleDisasterTypeData[1] = disasterInfoObj[unitCode][disasterInfoType]; //向数组添加灾情数据类型及对应值
            singleDisasterTypeDataArr.push(singleDisasterTypeData);
        }
    }
    return singleDisasterTypeDataArr;
}

//从灾情信息中获取灾情等级以及相应行政单位代码作为分类渲染的依据
function getClassifyData(pageNO,disasterInfoObj) {
    var classifyDataArr = [];
    if (!$.isEmptyObject(disasterInfoObj)) {
        $.ajax({
            url: '/Statistics/GetDWDisasterLevel',
            type: 'post',
            async: false,
            data: { pageNO: pageNO },
            success: function (levelList) {
                for (var i = 0; i < levelList.length; i++) {
                    classifyDataArr.push([levelList[i].unitCode, levelList[i].dLevel]);
                }
            },
            error: function (e) {
                alert(e);
            }
        })
    }
    return Obj2Str(classifyDataArr);
}
//旧评估方法
//function getClassifyData(disasterInfoObj) {
//    var classifyDataArr = [];
//    if (!$.isEmptyObject(disasterInfoObj)) {
//        for (unitCode in disasterInfoObj) {
//            var singleUnitClassifyData = [];
//            singleUnitClassifyData[0] = unitCode;
//            var value = 0;
//            for (disasterInfoType in mapParams.disasterInfoTypes) {
//                var singleDisasterTypeData = disasterInfoObj[unitCode][disasterInfoType]; //向数组添加灾情数据类型及对应值
//                var breakValues = mapParams.disasterInfoTypes[disasterInfoType].breakValues[0];
//                var weight = 0;
//                if (singleDisasterTypeData < breakValues[0]) {
//                    weight = 1;
//                }
//                else if (singleDisasterTypeData >= breakValues[0] & singleDisasterTypeData < breakValues[1]) {
//                    weight = 2;
//                }
//                else if (singleDisasterTypeData >= breakValues[1] & singleDisasterTypeData < breakValues[2]) {
//                    weight = 3;
//                }
//                else if (singleDisasterTypeData >= breakValues[2] & singleDisasterTypeData < breakValues[3]) {
//                    weight = 4;
//                }
//                else {
//                    weight = 5;
//                }
//                value += weight;
//            }
//            singleUnitClassifyData[1] = value;
//            classifyDataArr.push(singleUnitClassifyData);
//        }
//    }
//    return Obj2Str(classifyDataArr);
//}