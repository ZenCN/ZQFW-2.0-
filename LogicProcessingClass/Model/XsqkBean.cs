using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicProcessingClass.Model
{
    public class XsqkBean
    {
        public string Time;
        public string UnitName;
        public string SimpleUnitName;
        public string XSLMeasureName;
        public string OtherUnitName;
        public string Month ;//统计的是口那月的蓄水情况
        public string TJ_MONTH ;//（统计时间）月
        public string TJ_DAY ;//（统计时间）日
        public string XSCSZJ;//总共蓄水工程（处）
        public string XXSLZJ;//总共蓄水量（亿立方米）
        public string XZYBFB;//总共蓄水量占计划的百分比（%）
        public string GGGSXSL ;//灌溉、供水蓄水量（亿立方米）
        public string GGGSXSBFB ;//灌溉、供水蓄水量占计划的百分比（%）
        public string SQ_MONTH ;//上期月
        public string SQ_DAY ;//上期日
        public string SQ_DHS ;//比上期多或少
        public string TQ_DHS ;//比同期多或少
        public string BSQXS ;//比上期多（少）蓄水（亿立方米）
        public string BTQPJXS ;//比历年同期平均多（少）蓄水（亿立方米）
        public string BTQPJXS_Percent;//比历年同期平均多（少）蓄水百分之几（%）
        public string DZKXXSL;//大型水库蓄水量
        public string DZKXXSL_Percent;//大型水库占总蓄水量的百分之几（%）
        public string ZZKXXSL;//中型水库蓄水量
        public string ZZKXXSL_Percent;//中型水库占总蓄水量的百分之几（%）
        public string XZKXXSL;//小型水库蓄水量
        public string XZKXXSL_Percent;//小型水库占总蓄水量的百分之几（%）
        public string SPTXXS;//山坪塘蓄水量
        public string SPTXXS_Percent;//山坪塘占总蓄水量的百分之几（%）
        public string XZKYSL;//可用水量（亿立方米）
        /*public string KYSLDJW ;//自1973年来同期占第几位*/
        public string MaxXSLCities;//蓄水量较多的四个城市之一
        public string MaxXSLCitiesXSBFB;//这个城市蓄水百分比
        public string MinXSLCities;//蓄水量较少的四个城市之一
        public string MinXSLCitiesXSBFB;//这个城市蓄水百分比
        public string QTCSXSBFBMIN ;//其他城市蓄水百分比的最小值
        public string QTCSXSBFBMAX ;//其他城市蓄水百分比的最大值
        public string BarChartPath;  //柱状图路径
        public string PieChartPath;  //饼状图路径
        public string MapFileName;  //地图图片名称
        public string MapPath;  //地图图片路径
    }
}
