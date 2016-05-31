window.App = window.App || {};

App.CommonReportModel = {
    Time: undefined,
    UnitName: undefined,
    Month: undefined,   //统计的是口那月的蓄水情况
    TJ_MONTH: undefined,    //（统计时间）月
    TJ_DAY: undefined,  //（统计时间）日
    XSCSZJ: undefined,  //总共蓄水工程（处）  ZGXSGC
    XXSLZJ: undefined,   //总共蓄水量（亿立方米）ZGXSL
    XZYBFB: undefined,    //总共蓄水量占计划的百分比（%） ZGXSLBFB
    GGGSXSL: undefined, //灌溉、供水蓄水量（亿立方米）
    GGGSXSBFB: undefined,   //灌溉、供水蓄水量占计划的百分比（%）
    SQ_MONTH: undefined,    //上期月
    SQ_DAY: undefined,  //上期日
    SQ_DHS: undefined,  //比上期多或少
    TQ_DHS: undefined,  //比同期多或少
    BSQXS: undefined,   //比上期多（少）蓄水（亿立方米）
    BTQPJXS: undefined,//比历年同期平均多（少）蓄水（亿立方米）
    XZKYSL: undefined, //可用水量（亿立方米） KYSL
    KYSLDJW: undefined,//自1973年来同期占第几位
/*    CityMax1 :undefined,//蓄水量较多的四个城市之一
    CityMax2 :undefined,//蓄水量较多的四个城市之一
    CityMax3 :undefined,//蓄水量较多的四个城市之一
    CityMax4 :undefined,//蓄水量较多的四个城市之一*/
    MaxXSLCities: "",
/*    XSBFB_CityMax1 :undefined,//这个城市蓄水百分比
    XSBFB_CityMax2 :undefined,
    XSBFB_CityMax3 :undefined,
    XSBFB_CityMax4 :undefined,*/
    MaxXSLCitiesXSBFB: "",
/*    CityMin1 :undefined,//蓄水量较少的四个城市之一
    CityMin2 :undefined,//蓄水量较少的四个城市之一
    CityMin3 :undefined,//蓄水量较少的四个城市之一
    CityMin4 :undefined,//蓄水量较少的四个城市之一*/
    MinXSLCities: "",
/*    XSBFB_CityMin1 :undefined,//这个城市蓄水百分比
    XSBFB_CityMin2 :undefined,
    XSBFB_CityMin3 :undefined,
    XSBFB_CityMin4 :undefined,*/
    MinXSLCitiesXSBFB: "",
    QTCSXSBFBMIN: undefined,//其他城市蓄水百分比的最小值
    QTCSXSBFBMAX: undefined, //其他城市蓄水百分比的最大值
    SimpleUnitName: undefined,
    XSLMeasureName: undefined,
    OtherUnitName: undefined
};