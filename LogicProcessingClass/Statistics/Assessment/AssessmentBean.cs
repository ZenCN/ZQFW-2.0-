using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicProcessingClass.Statistics
{
    public class DisasterTypeBean
    {
        public string type { get; set; }  //灾情数据类别
        public string name { get; set; }    //类别名称
        public string measureUnit { get; set; } //测量单位
        public int measureValue { get; set; }  //测量值
        public double value { get; set; }  //值
        public int decimalDigits { get; set; } //保留小数位数
    }

    public class PieChartBean
    {
        public string unitName { get; set; }  //行政单位名称
        public double value { get; set; }  //灾情数据值
    }
}
