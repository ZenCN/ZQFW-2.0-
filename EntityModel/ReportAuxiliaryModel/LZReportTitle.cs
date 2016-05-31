using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：
    // 文件名：ReportTitle.cs
    // 文件功能描述：不包含导航属性等的表ReportTitle模型
    // 创建标识：
    // 修改标识：
    // 修改描述：
//-------------------------------------------------------------*/
namespace EntityModel.ReportAuxiliaryModel
{
    public class LZReportTitle
    {
        public string PageNO { get; set; }
        public string ORD_Code { get; set; }
        public string RPTType_Code { get; set; }
        public string StatisticalCycType { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string UnitPrincipal { get; set; }
        public string StatisticsPrincipal { get; set; }
        public string WriterName { get; set; }
        public string WriterTime { get; set; }
        public string SendTime { get; set; }
        public string Remark { get; set; }
        public string Del { get; set; }
        public string State { get; set; }
        public string SourceType { get; set; }
        public string ReceiveTime { get; set; }
        public string AssociatedPageNO { get; set; }
        public string OperateReportNO { get; set; }
        public string DisasterTypeName { get; set; }
        public string DisasterDescribe { get; set; }
        public string DisasterSummary { get; set; }
        public string ExceptPageNo { get; set; }
        public string RBType { get; set; }
        public string LastUpdateTime { get; set; }
        public string ReceiveState { get; set; }
        public string CloudPageNO { get; set; }
        public string CopyPageNO { get; set; }
        public string SendOperType { get; set; }

        public LZReportTitle()
        {

        }
    }
}
