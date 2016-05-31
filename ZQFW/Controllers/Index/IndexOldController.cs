using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LogicProcessingClass.AuxiliaryClass;
using LogicProcessingClass.ReportOperate;
using DBHelper;

namespace ZQFW.Controllers
{
    public class IndexOldController : Controller
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            return View("Frame");
        }

        public string GetInitData() 
        {
            int limit = Convert.ToInt32(Request["limit"]);//单位级别
            string unitCode = Request["unitcode"].ToString();//单位代码
            TableFieldBaseData tab = new TableFieldBaseData();
            CommonFunction comm = new CommonFunction();
            string DistributeRiver = "DistributeRiver:" + (new RiverDistribute().GetRiverRPTypeInfo(unitCode).DRiverRPType.Count > 1 ? "true" : "false");//获取流域标识
            string RiverCode = comm.GetRiverCodeList();//获得所有流域对照表 格式：RiverCode:{}
            string DeathReason = comm.GetDeathReasonList();//获取所有死亡原因 格式：DeathReason:{DeathReasons:[{}]}
            string LocalCheck = "Local: " + comm.GetProvenceData();//获取校核数据 格式：LocalCheck:[{}]
            string UndersCheck = "Unders:[" + tab.QueryCheckBaseData(limit, unitCode) + "]";//获取字段校核基础数据（已按系数进行转换） 格式：UndersCheck:[{}]
            string FieldExplain = "Explain:" + tab.GetTableExplain();//填表说明 格式：tableExplain:[{}]
            string FieldUnit = "Unit:[" + tab.GetFieldMeasureName(limit) + "]"; ;//获取计量单位(中文)如xx：千公顷 格式：fieldUnit[{}]
            string MeasureValue = "MeasureValue:" + tab.GetMeasureValue(limit);//获取计量单位(整数) 格式：measureValue:[{}]
            string RecentReportInfo = new ViewReportForm().ViewReportTitleInfo(unitCode, limit);//获取最近一次的填表信息 格式：RecentReportInfo:[{}]
            string RptClass = comm.GetRptClass(); //表类型 RptClass:[]
            string CycType = comm.GetCycType();//周期类型 CycType:[]
            string UnderUnits = ""; //下级单位
            Persistence per = new Persistence();
            Dictionary<string, District> dic = per.GetLowerUnits(unitCode);
            if (dic.Count > 0)
            {
                UnderUnits += "Unit:{Unders:[";
                foreach (District dis in dic.Values)
                {
                    UnderUnits += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "'},";
                }
                UnderUnits = UnderUnits.Remove(UnderUnits.Length - 1) + "]}";
            }
            string result = "{" + UnderUnits + "," + DistributeRiver + "," + RiverCode + "," + DeathReason + "," + "RelationCheck:{" + LocalCheck + "," + UndersCheck + "}," + "Field:{" + FieldExplain + "," + FieldUnit + "," + MeasureValue + "}," + RecentReportInfo + "," + RptClass + "," + CycType + "}";

            return result;
        }
        

        // GET: /Index/New
        public ActionResult New()
        {
            return View("New");
        }

        // GET: /Index/Open
        public ActionResult Open()
        {
            return View("Open");
        }

        // GET: /Index/Inbox
        public ActionResult Inbox()
        {
            return View("Inbox");
        }

        // GET: /Index/RecycleBin
        public ActionResult RecycleBin()
        {
            return View("RecycleBin");
        }

        // GET: /Index/Secretary
        public ActionResult WorkSecretary()
        {
            return View("WorkSecretary");
        }
    }
}
