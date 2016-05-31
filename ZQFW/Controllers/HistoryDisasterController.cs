using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBHelper;
using LogicProcessingClass.ReportOperate;
using LogicProcessingClass.AuxiliaryClass;

namespace ZQFW.Controllers
{
    public class HistoryDisasterController : Controller
    {
        //
        // GET: /HistoryDisaster/

        public ActionResult Index()
        {
            if (Session["SESSION_USER"] == null)
            {
                return Redirect("/Login");
            }

            int limit = Convert.ToInt32(Request["limit"]); //单位级别
            string unitCode = Request["unitcode"].ToString(); //单位代码
            UrgeAndReadReport urgeReport = new UrgeAndReadReport();
            TableFieldBaseData tab = new TableFieldBaseData();
            CommonFunction comm = new CommonFunction();
            GetHP01Const hp01Const = new GetHP01Const();
            RiverDistribute river = new RiverDistribute();
            /*string DistributeRiver = "DistributeRiver:" +
                                     (new RiverDistribute().GetRiverRPTypeInfo(unitCode).DRiverRPType.Count > 1
                                         ? "true"
                                         : "false"); //获取流域标识*/
            string RiverCode = comm.GetRiverCodeList(); //获得所有流域对照表 格式：RiverCode:{}
            string DeathReason = comm.GetDeathReasonList(); //获取所有死亡原因 格式：DeathReason:{DeathReasons:[{}]}
            string UndersCheck = "Formula:" + comm.GetProvenceData(); //获取校核数据 格式：LocalCheck:[{}]
            string LocalCheck = "Constant: " + tab.QueryCheckBaseData(limit, unitCode);
            string underReservoirCode = hp01Const.GetUnderReservoirCodeByUnitCode(unitCode, limit);

            string urgeReportList = urgeReport.GetUrgeReportList(limit, unitCode);
            string HPDate = "[" + hp01Const.GetHPDate() + "]";
            //获取字段校核基础数据（已按系数进行转换） 格式：UndersCheck:[{}]
            //string FieldExplain = "Explain:" + tab.GetTableExplain(); //填表说明 格式：tableExplain:[{}]
            //string FieldUnit = "Unit:[" + tab.GetFieldMeasureName(limit) + "]";
            //; //获取计量单位(中文)如xx：千公顷 格式：fieldUnit[{}]
            //string MeasureValue = "MeasureValue:" + tab.GetMeasureValue(limit); //获取计量单位(整数) 格式：measureValue:[{}]
            string underRiver = river.GetUnderUnitRiverDataByCode(unitCode);
            string distributeCode = "";
            if (underRiver != "")
            {
                distributeCode = "DistributeRiver:" + river.GetUnderUnitRiverDataByCode(unitCode);
            }
            else
            {
                distributeCode = "DistributeRiver:[]";
            }
            string reservoirs = "";
            if (unitCode.StartsWith("15"))
            {
                string unitName = HttpUtility.UrlDecode(Request["unitname"].ToString());
                reservoirs = "Reservoir:[" + hp01Const.GetNMReservoirCodeByUnitCode(unitCode, limit, unitName) + "]";
            }
            else
            {
                reservoirs = "Reservoir:{" + hp01Const.GetHPConst(unitCode, DateTime.Now.Month) + ",RSC_UC:{" + underReservoirCode + "}}";
            }
            string fields = tab.GetTB55Fields(limit); //tb55表的数据
            string RecentReportInfo = new ViewReportForm().ViewReportTitleInfo(unitCode, limit);
            //获取最近一次的填表信息 格式：RecentReportInfo:[{}]
            string RptClass = comm.GetRptClass(); //表类型 RptClass:[]
            string CycType = comm.GetCycType(); //周期类型 CycType:[]
            string Units = ""; //下级单位
            Persistence per = new Persistence();
            Dictionary<string, District> dic = per.GetLowerUnits(unitCode);
            if (dic.Count > 0)
            {
                foreach (District dis in dic.Values)
                {
                    Units += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "',RiverCode:'" +
                             dis.RiverCode + "'},";
                }
                Units = Units.Remove(Units.Length - 1);
            }
            Units = "Unit:{RiverCode:'" + comm.GetRiverCodeByUnitCode(unitCode) + "',Unders:[" + Units + "]}";
            //if (dic.Count > 0)
            //{
            //    Units += "Unit:{RiverCode:'" + comm.GetRiverCodeByUnitCode(unitCode) + "',Unders:[";
            //    foreach (District dis in dic.Values)
            //    {
            //        Units += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "',RiverCode:'" +
            //                 dis.RiverCode + "'},";
            //    }
            //    Units = Units.Remove(Units.Length - 1) + "]}";
            //}
            //string result = "{" + UnderUnits + "," + DistributeRiver + "," + RiverCode + "," + DeathReason + "," +
            //                "RelationCheck:{" + LocalCheck + "," + UndersCheck + "}," + "Field:{" + FieldExplain + "," +
            //                FieldUnit + "," + MeasureValue + "}," + RecentReportInfo + ",Select:{" + RptClass + "," +
            //                CycType + "},zTree:{" + GetTreeNode.GetCreateTreeData() + "}}";
            string result = "{" + Units + "," + urgeReportList + "," + RiverCode + "," + DeathReason + "," + reservoirs +  //+ DistributeRiver + ","
                            "," + "HPDate:" + HPDate + "," + distributeCode + "," +
                            "RelationCheck:{" + LocalCheck + "," + UndersCheck + "}," + "Field:{" + fields + "}," +
                            RecentReportInfo + ",Select:{" + RptClass + "," + CycType + "},zTree:{" +
                            GetTreeNode.GetCreateTreeData() + "}}";
            ViewData["InitData"] = result;

            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/HistoryDisaster.cshtml");
            }
            else
            {
                return View("~/Views/Release/HistoryDisaster.cshtml");
            }
        }

    }
}