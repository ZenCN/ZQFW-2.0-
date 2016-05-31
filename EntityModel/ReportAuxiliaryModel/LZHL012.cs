using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：
    // 文件名：XMMHL012.cs
    // 文件功能描述：不包含导航属性等的表HL012模型
    // 创建标识：
    // 修改标识：
    // 修改描述：
//-------------------------------------------------------------*/
namespace EntityModel.ReportAuxiliaryModel
{
    public class LZHL012
    {
        public string TBNO { get; set; }
        public string UnitCode { get; set; }
        public string RiverCode { get; set; }

        public string DataOrder { get; set; }
        public string DistributeRate { get; set; }
        public string DW { get; set; }
        public string SWXM { get; set; }
        public string SWXB { get; set; }
        public string SWNL { get; set; }
        public string SWHJ { get; set; }
        public string SWSJ { get; set; }
        public string SWDD { get; set; }
        public string YYTF { get; set; }
        public string YYJH { get; set; }
        public string YYSH { get; set; }
        public string YYHP { get; set; }
        public string YYNSL { get; set; }
        public string YYFWDT { get; set; }
        public string YYLS { get; set; }
        public string YYGKZW { get; set; }
        public string YYQT { get; set; }
        public string BZ { get; set; }
        public string SourcePageNo { get; set; }
        public string SourceDBType { get; set; }
        public string DeathReason { get; set; }
        public string DeathReasonCode { get; set; }
        public string PageNO { get; set; }


        public string DataType
        {
            get;
            set;
        }     
        public LZHL012()
        {

        }
    }
}
