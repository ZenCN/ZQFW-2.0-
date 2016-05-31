using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicProcessingClass.Statistics
{
    class AnalysisBean
    {
    }

    /// <summary>灾情分析趋势图数据
    /// 
    /// </summary>
    public class QSChartBean
    {
        public string UnitCode { get; set; }
        public int? PageNO { get; set; }
        public decimal? value { get; set; }
    }
    public class QSDataDWBean
    {
        public string name { get; set; } //单位名称
        public double[] data { get; set; } //数据
    }

    public class EvaluationTitle
    {
        public string title { get; set; } //标题（时间）
        public string subtitle {get;set;}  //子标题（报表时段类型）
        public int itemlevel{get;set;} //灾情等级
        public int pageno{get;set;} //页号
    }
    public class DWdisasterLevel
    {
        public string unitCode { get; set; }//单位代码
        public string dLevel { get; set; }//灾情等级
    }
}
