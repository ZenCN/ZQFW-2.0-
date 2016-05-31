using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using DBHelper;
using EntityModel;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using JetBrains;
using JetBrains.ReSharper.Features.SolBuilderDuo.Engine.Satellite;
using LogicProcessingClass.AuxiliaryClass;
using LogicProcessingClass.ReportOperate;
using Newtonsoft.Json;


namespace ZQFW.Controllers
{
    public class SHController : Controller
    {
        Entities getEntity = new Entities();

        public ActionResult Index()
        {
            if (Session["SESSION_USER"] == null)
            {
                return Redirect("/Login");
            }

            //-------------------------------------------<Init Page Data>------------------------------------------------------------------------
            //------------------------New Page-----------------------------------------
            FXDICTEntities fxdict = (FXDICTEntities)new Entities().GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            var treeData = from t in fxdict.TB16_OperateReportDefine
                           where t.RC_Code == "SH"
                           select new
                           {
                               t.OperateReportCode,
                               t.OperateReportName
                           };
            string initData = "{";
            //-----------TreeData------------
            initData += "New:{ Report:{ TreeData:{ open:true, name:'山洪灾害', children:";
            if (treeData.Any())
            {
                initData += "[";
                foreach (var rptType in treeData)
                {
                    initData += "{ name:'" + rptType.OperateReportName + "', id:'" + rptType.OperateReportCode + "' },";
                }
                initData = initData.Remove(initData.Length - 1) + "]},";
            }
            else
            {
                initData += "[]},";
            }
            //------------CycType------------
            initData += "CycType:";
            var cycType = from t in fxdict.TB14_Cyc
                          where t.Remark.Contains("半月")
                          select t;
            if (cycType.Any())
            {
                initData += "{ Name: '" + cycType.SingleOrDefault().Remark + "', Code:'" +
                            cycType.SingleOrDefault().CycType + "', Title:'" + cycType.SingleOrDefault().RemarkDetail +
                            "'},";
            }
            else
            {
                initData += "{ Name:'半月报', Code:8, Title:'字典库TB14_Cyc表中不存在半月报字段' },";
            }
            //-----------Date---------------
            initData += "Date: { TimeSpan:{ Array:[";
            var date = from t in fxdict.TB64_NMSHDate
                       orderby t.DateOrder
                       select new
                       {
                           t.DateSpan
                       };
            if (date.Any())
            {
                foreach (var time in date)
                {
                    initData += "'" + time.DateSpan + "',";
                }
                initData = initData.Remove(initData.Length - 1) + "]}}";
            }
            else
            {
                initData += "]}}";
            }
            //---------SameReport------------

            //---------End-----------
            initData += "}},";
            //------------------------BaseData-----------------------------------------
            initData += "BaseData:{";
            //----------Noraml Units-----------
            initData += "Unit:{Unders:[";
            Dictionary<string, District> units = new Persistence().GetLowerUnits(Request["unitcode"]);
            if (units.Count > 0)
            {
                foreach (District dis in units.Values)
                {
                    initData += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "'},";
                }
                initData = initData.Remove(initData.Length - 1);
            }
            initData += "],";
            //----------SH Unius------------
            initData += "SH:{";
            string unitcode = Request["unitcode"];
            int limit = int.Parse(Request["limit"]);
            IList<TB66_SHUnits> list = null;
            switch (limit)
            {
                case 2:
                    list =
                        fxdict.TB66_SHUnits.Where(t => t.SH_Year == DateTime.Now.Year && t.UnitCode.StartsWith("15"))
                            .OrderBy(t => t.SH_Code)
                            .ThenBy(t => t.DataOrder)
                            .ToList<TB66_SHUnits>();
                    break;
                case 3:
                    list =
                        fxdict.TB66_SHUnits.Where(
                            t => t.SH_Year == DateTime.Now.Year && t.UnitCode.StartsWith(unitcode.Substring(0, 4)))
                            .OrderBy(t => t.SH_Code).ThenBy(t => t.DataOrder).ToList<TB66_SHUnits>();
                    break;
                case 4:
                    list =
                        fxdict.TB66_SHUnits.Where(t => t.SH_Year == DateTime.Now.Year && t.UnitCode == unitcode)
                            .OrderBy(t => t.SH_Code).ThenBy(t => t.DataOrder).ToList<TB66_SHUnits>();
                    break;
            }

            //---------SH01---------
            initData += "SH01:{";
            var sh0 = fxdict.TB66_SHUnits.Where(t => t.SH_Year == 2016 && t.SH_Code.StartsWith("SH01")).ToList<TB66_SHUnits>();
            if (sh0.Any())
            {
                initData += "SH011:[";
                foreach (var unit in sh0)
                {
                    initData += "{JSNR:'" + unit.UnitName + "',UnitCode:'" + unit.UnitCode + "'},";
                }
                initData = initData.Remove(initData.Length - 1) + "]";
            }
            initData += "},";

            if (list != null && list.Any())
            {
                //---------SH02---------
                Dictionary<string, string> shUnit = new Dictionary<string, string>();
                sh0 = list.Where(t => t.SH_Code.StartsWith("SH02")).ToList<TB66_SHUnits>();
                initData += "SH02:{";
                if (sh0.Any())
                {
                    initData += "SH021:[";
                    foreach (TB66_SHUnits unit in sh0)
                    {
                        initData += "{SZX:'" + unit.UnitName + "',UnitCode:" + unit.UnitCode + ",DataOrder:" + unit.DataOrder + ",GDMC:'" +
                                    unit.GD_Name + "'},";
                    }
                    initData = initData.Remove(initData.Length - 1) + "]";
                }
                initData += "},";
                //---------SH03---------
                sh0 = list.Where(t => t.SH_Code.StartsWith("SH03")).ToList<TB66_SHUnits>();
                initData += "SH03:{";
                if (sh0.Any())
                {
                    foreach (var group in sh0.GroupBy(t => t.SH_Code))
                    {
                        foreach (var unit in group)
                        {
                            if (!shUnit.ContainsKey(unit.SH_Code))
                            {
                                shUnit.Add(unit.SH_Code.ToUpper(), "");
                            }

                            if (unit.SH_Code == "SH034" || unit.SH_Code == "SH035")
                            {
                                shUnit[unit.SH_Code] += "{XZQMC:'";
                            }
                            else
                            {
                                shUnit[unit.SH_Code] += "{SSX:'";
                            }
                            shUnit[unit.SH_Code] += unit.UnitName + "',UnitCode:" + unit.UnitCode + ",DataOrder:" + unit.DataOrder + "},";
                        }
                    }
                    foreach (KeyValuePair<string, string> pair in shUnit)
                    {
                        initData += pair.Key + ":[" + pair.Value.Remove(pair.Value.Length - 1) + "],";
                    }
                    initData = initData.Remove(initData.Length - 1);
                }
                initData += "},";
                //---------SH04---------
                sh0 = list.Where(t => t.SH_Code.StartsWith("SH04")).ToList<TB66_SHUnits>();
                initData += "SH04:{";
                if (sh0.Any())
                {
                    shUnit.Clear();
                    foreach (var group in sh0.GroupBy(t => t.SH_Code))
                    {
                        foreach (var unit in group)
                        {
                            if (!shUnit.ContainsKey(unit.SH_Code))
                            {
                                shUnit.Add(unit.SH_Code.ToUpper(), "");
                            }

                            if (unit.SH_Code == "SH044" || unit.SH_Code == "SH045")
                            {
                                shUnit[unit.SH_Code] += "{XZQMC:'";
                            }
                            else
                            {
                                shUnit[unit.SH_Code] += "{SSX:'";
                            }
                            shUnit[unit.SH_Code] += unit.UnitName + "',UnitCode:" + unit.UnitCode +
                                                    ",Limit:" + unit.UnitCls + ",DataOrder:" + unit.DataOrder + "},";
                        }
                    }
                    foreach (KeyValuePair<string, string> pair in shUnit)
                    {
                        initData += pair.Key + ":[" + pair.Value.Remove(pair.Value.Length - 1) + "],";
                    }
                    initData = initData.Remove(initData.Length - 1);
                }
                initData += "}";
            }
            else
            {
                initData += "SH02:{},SH03:{},SH04:{}";
            }
            initData += "}}";  //SH   Unit      |  New:{....},BaseData:{Unit:{...}}
            //--------------TB67_SHYearPlanData---------------
            initData += ",Plan:";
            IList<TB67_SHYearPlanData> plans = null;
            switch (limit)
            {
                case 2:
                    plans = fxdict.TB67_SHYearPlanData.Where(t => t.SHYear == DateTime.Now.Year).ToList();
                    break;
                case 3:
                    plans =
                        fxdict.TB67_SHYearPlanData.Where(
                            t => t.SHYear == DateTime.Now.Year && t.UnitCode.StartsWith(unitcode.Substring(0, 4)))
                            .ToList();
                    break;
                case 4:
                    plans =
                        fxdict.TB67_SHYearPlanData.Where(
                            t => t.SHYear == DateTime.Now.Year && t.UnitCode.StartsWith(unitcode.Substring(0, 6)))
                            .ToList();
                    break;
            }
            if (plans != null && plans.Any())
            {
                initData += "{";
                foreach (TB67_SHYearPlanData planData in plans)
                {
                    initData += "'" + planData.UnitCode + "-" + planData.SHTableName + "-" + planData.SHFieldName + "':" +
                                Convert.ToDouble(planData.SHFieldVal) + ",";
                }
                initData = initData.Remove(initData.Length - 1) + "}";   //最后一个initData不能再是 initData +=  所有的Remove一样！
            }
            else
            {
                initData += "{}";
            }

            initData += "}";  //   BaseData
            //-------------------------------------------</Init Page Data>------------------------------------------------------------------------
            initData += "}";  //最外面的“}”
            ViewData["InitData"] = initData;
            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/Index.SH.cshtml");
            }
            else
            {
                return View("~/Views/Release/Index.SH.cshtml");
            }
        }

        public ActionResult Print()
        {
            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return File("~/Views/Debug/Print.SH.html", "text/html");
            }
            else
            {
                return File("~/Views/Release/Print.SH.html", "text/html");
            }
        }

        public ActionResult Main()
        {
            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/Main.SH.cshtml");
            }
            else
            {
                return View("~/Views/Release/Main.SH.cshtml");
            }
        }

        public ActionResult HistoryDisaster()
        {
            if (Session["SESSION_USER"] == null)
            {
                return Redirect("/Login");
            }

            //-------------------------------------------<Init Page Data>------------------------------------------------------------------------
            //------------------------New Page-----------------------------------------
            FXDICTEntities fxdict = (FXDICTEntities)new Entities().GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            var treeData = from t in fxdict.TB16_OperateReportDefine
                           where t.RC_Code == "SH"
                           select new
                           {
                               t.OperateReportCode,
                               t.OperateReportName
                           };
            string initData = "{";
            //-----------TreeData------------
            initData += "New:{ Report:{ TreeData:{ open:true, name:'山洪灾害', children:";
            if (treeData.Any())
            {
                initData += "[";
                foreach (var rptType in treeData)
                {
                    initData += "{ name:'" + rptType.OperateReportName + "', id:'" + rptType.OperateReportCode + "' },";
                }
                initData = initData.Remove(initData.Length - 1) + "]},";
            }
            else
            {
                initData += "[]},";
            }
            //------------CycType------------
            initData += "CycType:";
            var cycType = from t in fxdict.TB14_Cyc
                          where t.Remark.Contains("半月")
                          select t;
            if (cycType.Any())
            {
                initData += "{ Name: '" + cycType.SingleOrDefault().Remark + "', Code:'" +
                            cycType.SingleOrDefault().CycType + "', Title:'" + cycType.SingleOrDefault().RemarkDetail +
                            "'},";
            }
            else
            {
                initData += "{ Name:'半月报', Code:8, Title:'字典库TB14_Cyc表中不存在半月报字段' },";
            }
            //-----------Date---------------
            initData += "Date: { TimeSpan:{ Array:[";
            var date = from t in fxdict.TB64_NMSHDate
                       orderby t.DateOrder
                       select new
                       {
                           t.DateSpan
                       };
            if (date.Any())
            {
                foreach (var time in date)
                {
                    initData += "'" + time.DateSpan + "',";
                }
                initData = initData.Remove(initData.Length - 1) + "]}}";
            }
            else
            {
                initData += "]}}";
            }
            //---------SameReport------------

            //---------End-----------
            initData += "}},";
            //------------------------BaseData-----------------------------------------
            initData += "BaseData:{";
            //----------Noraml Units-----------
            initData += "Unit:{Unders:[";
            Dictionary<string, District> units = new Persistence().GetLowerUnits(Request["unitcode"]);
            if (units.Count > 0)
            {
                foreach (District dis in units.Values)
                {
                    initData += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "'},";
                }
                initData = initData.Remove(initData.Length - 1);
            }
            initData += "],";
            //----------SH Unius------------
            initData += "SH:{";
            string unitcode = Request["unitcode"];
            int limit = int.Parse(Request["limit"]);
            IList<TB66_SHUnits> list = null;
            switch (limit)
            {
                case 2:
                    list = fxdict.TB66_SHUnits.Where(t => t.SH_Year == DateTime.Now.Year && t.UnitCode.StartsWith("15")).OrderBy(t => t.SH_Code).ToList<TB66_SHUnits>();
                    break;
                case 3:
                    list =
                        fxdict.TB66_SHUnits.Where(t => t.SH_Year == DateTime.Now.Year && t.UnitCode.StartsWith(unitcode.Substring(0, 4)))
                            .OrderBy(t => t.SH_Code).ToList<TB66_SHUnits>();
                    break;
                case 4:
                    list =
                        fxdict.TB66_SHUnits.Where(t => t.SH_Year == DateTime.Now.Year && t.UnitCode == unitcode)
                            .OrderBy(t => t.SH_Code).ToList<TB66_SHUnits>();
                    break;
            }

            //---------SH01---------
            initData += "SH01:{";
            var sh0 = fxdict.TB66_SHUnits.Where(t => t.SH_Year == 2016 && t.SH_Code.StartsWith("SH01")).ToList<TB66_SHUnits>();
            if (sh0.Any())
            {
                initData += "SH011:[";
                foreach (var unit in sh0)
                {
                    initData += "{JSNR:'" + unit.UnitName + "',UnitCode:'" + unit.UnitCode + "'},";
                }
                initData = initData.Remove(initData.Length - 1) + "]";
            }
            initData += "},";

            if (list != null && list.Any())
            {
                //---------SH02---------
                Dictionary<string, string> shUnit = new Dictionary<string, string>();
                sh0 = list.Where(t => t.SH_Code.StartsWith("SH02")).ToList<TB66_SHUnits>();
                initData += "SH02:{";
                if (sh0.Any())
                {
                    initData += "SH021:[";
                    foreach (TB66_SHUnits unit in sh0)
                    {
                        initData += "{SZX:'" + unit.UnitName + "',UnitCode:" + unit.UnitCode + ",GDMC:'" +
                                    unit.GD_Name + "'},";
                    }
                    initData = initData.Remove(initData.Length - 1) + "]";
                }
                initData += "},";
                //---------SH03---------
                sh0 = list.Where(t => t.SH_Code.StartsWith("SH03")).ToList<TB66_SHUnits>();
                initData += "SH03:{";
                if (sh0.Any())
                {
                    foreach (var group in sh0.GroupBy(t => t.SH_Code))
                    {
                        foreach (var unit in group)
                        {
                            if (!shUnit.ContainsKey(unit.SH_Code))
                            {
                                shUnit.Add(unit.SH_Code.ToUpper(), "");
                            }

                            if (unit.SH_Code == "SH034" || unit.SH_Code == "SH035")
                            {
                                shUnit[unit.SH_Code] += "{XZQMC:'";
                            }
                            else
                            {
                                shUnit[unit.SH_Code] += "{SSX:'";
                            }
                            shUnit[unit.SH_Code] += unit.UnitName + "',UnitCode:" + unit.UnitCode + "},";
                        }
                    }
                    foreach (KeyValuePair<string, string> pair in shUnit)
                    {
                        initData += pair.Key + ":[" + pair.Value.Remove(pair.Value.Length - 1) + "],";
                    }
                    initData = initData.Remove(initData.Length - 1);
                }
                initData += "},";
                //---------SH04---------
                sh0 = list.Where(t => t.SH_Code.StartsWith("SH04")).ToList<TB66_SHUnits>();
                initData += "SH04:{";
                if (sh0.Any())
                {
                    shUnit.Clear();
                    foreach (var group in sh0.GroupBy(t => t.SH_Code))
                    {
                        foreach (var unit in group)
                        {
                            if (!shUnit.ContainsKey(unit.SH_Code))
                            {
                                shUnit.Add(unit.SH_Code.ToUpper(), "");
                            }

                            if (unit.SH_Code == "SH044" || unit.SH_Code == "SH045")
                            {
                                shUnit[unit.SH_Code] += "{XZQMC:'";
                            }
                            else
                            {
                                shUnit[unit.SH_Code] += "{SSX:'";
                            }
                            shUnit[unit.SH_Code] += unit.UnitName + "',UnitCode:" + unit.UnitCode +
                                                    ",Limit:" + unit.UnitCls + "},";
                        }
                    }
                    foreach (KeyValuePair<string, string> pair in shUnit)
                    {
                        initData += pair.Key + ":[" + pair.Value.Remove(pair.Value.Length - 1) + "],";
                    }
                    initData = initData.Remove(initData.Length - 1);
                }
                initData += "}";
            }
            else
            {
                initData += "SH02:{},SH03:{},SH04:{}";
            }
            initData += "}}}";  //SH   Unit    BaseData      |  New:{....},BaseData:{Unit:{...}}
            //-------------------------------------------</Init Page Data>------------------------------------------------------------------------
            initData += "}";  //最外面的“}”
            ViewData["InitData"] = initData;

            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/HistoryDisaster.SH.cshtml");
            }
            else
            {
                return View("~/Views/Release/HistoryDisaster.SH.cshtml");
            }
        }

        public string GetTemplate(string rptType, string tableType, int isCurYear)
        {
            string template = "";
            tableType = tableType.ToLower() == "edit" ? "Edit" : "View";
            string name = (isCurYear > 0 ? "CurYear" : "OverYear") + "-" + rptType + "-" + tableType;
            HttpApplicationState app = System.Web.HttpContext.Current.Application;
            if (app[name] != null)
            {
                template = app[name].ToString();
            }
            else
            {
                Dictionary<string, string[]> dic = new Dictionary<string, string[]>();
                dic.Add("SH01", new string[] {"SH011"});
                dic.Add("SH02", new string[] {"SH021"});
                dic.Add("SH03", new string[] {"SH031", "SH032", "SH033", "SH034", "SH035", "SH036", "SH037", "SH038"});
                dic.Add("SH04", new string[] {"SH046"});  //"SH041", "SH042", "SH043", "SH044", "SH045"
                dic.Add("SH05", new string[] {"SH051"});
                string bathPath = AppDomain.CurrentDomain.BaseDirectory + "Scripts/Templates/Public/SH/Table/";
                string fileUrl = "";
                StreamReader sr = null;

                template = "<div class='Rpt-Content' ng-switch on='Open.Report.Current.Attr.TableIndex'>\r";
                string tabs = "<ul class='Rpt-Tags'>\r";
                for (int i = 0; i < dic[rptType.ToUpper()].Length; i++)
                {
                    fileUrl = bathPath + dic[rptType.ToUpper()][i];
                    if (dic[rptType.ToUpper()][i] == "SH011" || dic[rptType.ToUpper()][i] == "SH041" || dic[rptType.ToUpper()][i] == "SH045")
                    {
                        if (isCurYear > 0)
                        {
                            fileUrl += "/CurYear";
                        }
                        else
                        {
                            fileUrl += "/OverYear";
                        }
                    }

                    template += "<table table-fixed ng-switch-when='" + i + "'>\r";
                    sr = new StreamReader(fileUrl + "/THead.htm");
                    template += sr.ReadToEnd() + "\r";
                    sr = new StreamReader(fileUrl + "/TBody_" + tableType + ".htm");
                    template += sr.ReadToEnd() + "\r";
                    template += "</table>\r";

                    tabs += "<li ng-if='Open.Report.Current." + rptType.ToUpper() + (i + 1) + "' ng-class='{Selected:Open.Report.Current.Attr.TableIndex == " + i +
                            "}' ng-click='Open.Report.Current.Attr.TableIndex = "
                            + i + "'><a>{{BaseData.Page." + rptType.ToUpper() + "[" + (i + 1) + "]}}</a></li>\r";
                }
                tabs += "</ul>";
                template += "</div>\r" + tabs;

                app[name] = template;
            }
            Response.ContentType = "text/plain";

            return template;
        }

        public string GetTreeData(string unitCode, string unitLimit, string rptClass, string limitType, string cycType, string filterByUnitCode, int minYear)
        {
            string response = "";
            rptClass = rptClass == null ? "" : rptClass;
            cycType = cycType == null ? "" : cycType;
            filterByUnitCode = filterByUnitCode == null ? "" : filterByUnitCode;
            bool queryOneUnit = Request.Cookies["unitcode"].Value != unitCode;
            GetTreeNode treeNode = new GetTreeNode(int.Parse(unitLimit), Convert.ToInt32(limitType));
            response = treeNode.GetFormerYearsTreeNodeData(int.Parse(unitLimit), unitCode, int.Parse(limitType), "",
                cycType, filterByUnitCode, rptClass, queryOneUnit, minYear);

            if (minYear < 0)
            {
                response = response.Replace("[未报送]", "").Replace("[已报送]", "");
            }

            if (int.Parse(unitLimit) == 2)
            {
                response = response.Replace("[未报送]", "");
            }

            return response;
        }

        public string Template()
        {
            string response = "";

            try
            {

            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public string SendReport(int limit,int pageno)
        {
            string response = "";

            try
            {
                BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
                var rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault();
                if (rpt != null)
                {
                    rpt.State = 3;//变更表头的状态为3，表示该套报表已经上报
                    rpt.ReceiveState = 2;//接收状态字段不变，默认为0 
                    rpt.CopyPageNO = 0;//副本字段，默认为0
                    rpt.SendTime = DateTime.Now;
                    busEntity.SaveChanges();
                    response = "1";
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public string SaveReport(string report)
        {
            string response = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(int.Parse(Request["limit"]));
            Hashtable table = serializer.Deserialize<Hashtable>(report);

            object rptTitle = serializer.ConvertToType<object>(table["ReportTitle"]);

            Hashtable reportTitle = serializer.ConvertToType<Hashtable>(rptTitle);
            ReportTitle rpt = new ReportTitle();
            DateTime dt = DateTime.Now;
            string staticUnitName = Request.Cookies["unitname"].Value;
            rpt.Remark = reportTitle["Remark"] == null ? "" : reportTitle["Remark"].ToString();
            rpt.StatisticsPrincipal = reportTitle["StatisticsPrincipal"] == null ? "" : reportTitle["StatisticsPrincipal"].ToString();
            rpt.UnitPrincipal = reportTitle["UnitPrincipal"] == null ? "" : reportTitle["UnitPrincipal"].ToString();
            rpt.WriterName = reportTitle["WriterName"] == null ? "" : reportTitle["WriterName"].ToString();
            rpt.StatisticalCycType = Convert.ToInt32(reportTitle["StatisticalCycType"]);
            rpt.ORD_Code = reportTitle["ORD_Code"] == null ? "" : reportTitle["ORD_Code"].ToString();
            rpt.RPTType_Code = "XZ0";
            if (reportTitle["RPTType_Code"] != null && reportTitle["RPTType_Code"] != "")
            {
                rpt.RPTType_Code = reportTitle["RPTType_Code"].ToString();
            }
            rpt.UnitName = (reportTitle["UnitName"] == null || reportTitle["UnitName"].ToString() == "") ? HttpUtility.UrlDecode(staticUnitName, Encoding.GetEncoding("utf-8")) : reportTitle["UnitName"].ToString();
            rpt.UnitCode = reportTitle["UnitCode"].ToString();
            rpt.StartDateTime = Convert.ToDateTime(reportTitle["StartDateTime"].ToString());
            rpt.WriterTime = Convert.ToDateTime(reportTitle["WriterTime"].ToString());
            rpt.EndDateTime = Convert.ToDateTime(reportTitle["EndDateTime"].ToString());
            rpt.Del = 0;
            rpt.ReceiveState = 0;
            rpt.State = reportTitle["State"] == null ? 0 : int.Parse(reportTitle["State"].ToString());
            rpt.CloudPageNO = 0;
            rpt.CopyPageNO = 0;
            rpt.SendOperType = 0;
            rpt.SourceType = reportTitle["SourceType"] == null ? 0 : Convert.ToInt32(reportTitle["SourceType"].ToString());
            rpt.SendTime = dt;
            rpt.ReceiveTime = dt;
            rpt.PageNO = int.Parse(reportTitle["PageNO"].ToString());
            rpt.LastUpdateTime = dt;

            if (rpt.PageNO != 0) //更新操作
            {
                if (rpt.State == 3)  //更新已报送
                {
                    int new_pageno = new ReportHelpClass().FindMaxPageNO(int.Parse(Request["limit"])) + 1;
                    var rpt_title = busEntity.ReportTitle.Where(t => t.PageNO == rpt.PageNO).SingleOrDefault();
                    if (rpt_title != null)
                    {
                        rpt_title.CopyPageNO = new_pageno;
                    }

                    rpt.PageNO = new_pageno;
                    rpt.State = 0;
                }
                else  //更新未报送
                {
                    if (!ClearOldData(busEntity, rpt.PageNO))
                    {
                        return "清楚旧数据失败！";
                    }
                }
            }
            else
            {
                rpt.PageNO = new ReportHelpClass().FindMaxPageNO(int.Parse(Request["limit"])) + 1;
            }
            busEntity.ReportTitle.AddObject(rpt);

            #region   处理山洪表的数据
            string deKey = "";
            //int maxTBNO = 0;
            foreach (DictionaryEntry de in table)
            {
                deKey = de.Key.ToString();
                switch (deKey) //根据DBname不同执行不同的操作
                {
                    case "SH011":
                        object[] sh011s = serializer.ConvertToType<object[]>(de.Value);
                        //maxTBNO = busEntity.SH011.Any() ? busEntity.SH011.Max(t => t.TBNO) : 0;
                        for (int i = 0; i < sh011s.Length; i++)//循环这个对象数组
                        {
                            SH011 sh011 = serializer.ConvertToType<SH011>(sh011s[i]);
                            sh011.PageNO = rpt.PageNO;
                            //sh011.TBNO = 0;
                            busEntity.SH011.AddObject(sh011);
                        }
                        break;
                    case "SH021":
                        object[] sh021s = serializer.ConvertToType<object[]>(de.Value);
                        //maxTBNO = busEntity.SH021.Any() ? busEntity.SH011.Max(t => t.TBNO) : 0;
                        for (int i = 0; i < sh021s.Length; i++)//循环这个对象数组
                        {
                            SH021 sh021 = serializer.ConvertToType<SH021>(sh021s[i]);
                            sh021.PageNO = rpt.PageNO;
                            //sh021.TBNO = 0;
                            busEntity.SH021.AddObject(sh021);
                        }
                        break;

                    case "SH031":
                        object[] sh031s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh031s.Length; i++)//循环这个对象数组
                        {
                            SH031 sh031 = serializer.ConvertToType<SH031>(sh031s[i]);
                            sh031.PageNO = rpt.PageNO;
                            //sh031.TBNO = 0;
                            busEntity.SH031.AddObject(sh031);
                        }
                        break;
                    case "SH032":
                        object[] sh032s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh032s.Length; i++)//循环这个对象数组
                        {
                            SH032 sh032 = serializer.ConvertToType<SH032>(sh032s[i]);
                            sh032.PageNO = rpt.PageNO;
                            //sh032.TBNO = 0;
                            busEntity.SH032.AddObject(sh032);
                        }
                        break;
                    case "SH033":
                        object[] sh033s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh033s.Length; i++)//循环这个对象数组
                        {
                            SH033 sh033 = serializer.ConvertToType<SH033>(sh033s[i]);
                            sh033.PageNO = rpt.PageNO;
                            //sh033.TBNO = 0;
                            busEntity.SH033.AddObject(sh033);
                        }
                        break;
                    case "SH034":
                        object[] sh034s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh034s.Length; i++)//循环这个对象数组
                        {
                            SH034 sh034 = serializer.ConvertToType<SH034>(sh034s[i]);
                            sh034.PageNO = rpt.PageNO;
                            //sh034.TBNO = 0;
                            busEntity.SH034.AddObject(sh034);
                        }
                        break;
                    case "SH035":
                        object[] sh035s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh035s.Length; i++)//循环这个对象数组
                        {
                            SH035 sh035 = serializer.ConvertToType<SH035>(sh035s[i]);
                            sh035.PageNO = rpt.PageNO;
                            //sh035.TBNO = 0;
                            busEntity.SH035.AddObject(sh035);
                        }
                        break;
                    case "SH036":
                        object[] sh036s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh036s.Length; i++)//循环这个对象数组
                        {
                            SH036 sh036 = serializer.ConvertToType<SH036>(sh036s[i]);
                            sh036.PageNO = rpt.PageNO;
                            //sh036.TBNO = 0;
                            busEntity.SH036.AddObject(sh036);
                        }
                        break;
                    case "SH037":
                        object[] sh037s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh037s.Length; i++)//循环这个对象数组
                        {
                            SH037 sh037 = serializer.ConvertToType<SH037>(sh037s[i]);
                            sh037.PageNO = rpt.PageNO;
                            //sh037.TBNO = 0;
                            busEntity.SH037.AddObject(sh037);
                        }
                        break;
                    case "SH038":
                        object[] sh038s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh038s.Length; i++)//循环这个对象数组
                        {
                            SH038 sh038 = serializer.ConvertToType<SH038>(sh038s[i]);
                            sh038.PageNO = rpt.PageNO;
                            //sh038.TBNO = 0;
                            busEntity.SH038.AddObject(sh038);
                        }
                        break;

                    case "SH041":
                        object[] sh041s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh041s.Length; i++)//循环这个对象数组
                        {
                            SH041 sh041 = serializer.ConvertToType<SH041>(sh041s[i]);
                            sh041.PageNO = rpt.PageNO;
                            //sh041.TBNO = 0;
                            busEntity.SH041.AddObject(sh041);
                        }
                        break;
                    case "SH042":
                        object[] sh042s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh042s.Length; i++)//循环这个对象数组
                        {
                            SH042 sh042 = serializer.ConvertToType<SH042>(sh042s[i]);
                            sh042.PageNO = rpt.PageNO;
                            //sh042.TBNO = 0;
                            busEntity.SH042.AddObject(sh042);
                        }
                        break;
                    case "SH043":
                        object[] sh043s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh043s.Length; i++) //循环这个对象数组
                        {
                            SH043 sh043 = serializer.ConvertToType<SH043>(sh043s[i]);
                            sh043.PageNO = rpt.PageNO;
                            //sh043.TBNO = 0;
                            busEntity.SH043.AddObject(sh043);
                        }
                        break;
                    case "SH044":
                        object[] sh044s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh044s.Length; i++) //循环这个对象数组
                        {
                            SH044 sh044 = serializer.ConvertToType<SH044>(sh044s[i]);
                            sh044.PageNO = rpt.PageNO;
                            //sh044.TBNO = 0;
                            busEntity.SH044.AddObject(sh044);
                        }
                        break;
                    case "SH045":
                        object[] sh045s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh045s.Length; i++) //循环这个对象数组
                        {
                            SH045 sh045 = serializer.ConvertToType<SH045>(sh045s[i]);
                            sh045.PageNO = rpt.PageNO;
                            //sh045.TBNO = 0;
                            busEntity.SH045.AddObject(sh045);
                        }
                        break;
                    case "SH051":
                        object[] sh051s = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < sh051s.Length; i++) //循环这个对象数组
                        {
                            SH051 sh051 = serializer.ConvertToType<SH051>(sh051s[i]);
                            sh051.PageNO = rpt.PageNO;
                            busEntity.SH051.AddObject(sh051);
                        }
                        break;
                }
            }
            #endregion

            if (rpt.SourceType == 1) //汇总
            {
                string[] pagenos = reportTitle["SPageNOs"].ToString()
                    .Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < pagenos.Length; i++)
                {
                    AggAccRecord aggacc = new AggAccRecord();
                    aggacc.ORD_Code = rpt.ORD_Code;
                    aggacc.OperateType = 1;
                    aggacc.SPageNO = int.Parse(pagenos[i]);
                    aggacc.PageNo = rpt.PageNO;

                    busEntity.AggAccRecord.AddObject(aggacc);
                }
            }

            try
            {
                busEntity.SaveChanges();
                response = rpt.PageNO.ToString();
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public string GetRecentRpt(string rptType, string StartDateTime, string EndDateTime)
        {
            string response = "";
            int limit = int.Parse(Request["limit"]);
            string unitCode = Request["unitcode"];
            DateTime sTime = DateTime.Parse(StartDateTime);
            DateTime eTime = DateTime.Parse(EndDateTime);
            BusinessEntities business = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(limit);
            var rpts =
                business.ReportTitle.Where(
                    t => t.ORD_Code == rptType &&
                         t.UnitCode == unitCode &&
                         t.Del == 0 &&
                         t.StartDateTime >= sTime &&
                         t.EndDateTime <= eTime).OrderByDescending(t => t.WriterTime).ToList();
            if (rpts.Any())
            {
                response = OpenReport(rptType, 0, limit.ToString(), unitCode, rpts[0].PageNO.ToString()).Data.ToString();
            }
            else
            {
                response = "{}";
            }

            return response;
        }

        public JsonResult OpenReport(string rptType, int sourceType, string level, string unitCode, string pageno)
        {
            JsonResult jsr = new JsonResult();

            string result = "";
            string arr = "";

            unitCode = unitCode == null ? Request["unitcode"] : unitCode;
            int limit = Convert.ToInt32(level == null ? Request["limit"] : level); //单位级别 
            string unitName = new LogicProcessingClass.Tools().GetUnitNameByUnitCode(unitCode);
            int pageNO = Convert.ToInt32(pageno); //Request["pageno"]
            ViewReportForm viewRpt = new ViewReportForm();
            if (unitCode.StartsWith("15") && rptType == "NP01")//内蒙古蓄水
            {
                result = viewRpt.ViewReportFormInfo(pageNO, unitCode, limit);
                //GetHP01Const hp01Const = new GetHP01Const();
                //if (unitName.Trim() == "")
                //{
                //    unitName =HttpUtility.UrlDecode(Request.Cookies["unitname"].Value);
                //}
                //string reservoirs = "Reservoir:[" + hp01Const.GetNMReservoirCodeByUnitCode(unitCode, limit, unitName) + "]";
                //result = result + "," + reservoirs;
            }
            else
            {
                if (sourceType == 0)
                {
                    result = viewRpt.ViewReportFormInfo(limit, pageNO, rptType, true);
                }
                else if (sourceType == 1 || sourceType == 2)
                {
                    result = viewRpt.ViewReportFormInfo(limit, pageNO, rptType, true);
                    arr = viewRpt.GetSHSourceReportList(pageNO, limit);
                    result = result + "," + arr;
                }
            }
            jsr = Json("{" + result + "}");
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        public bool ClearOldData(BusinessEntities busEntity, int pageno)
        {
            bool success = false;
            var sh011s = busEntity.SH011.Where(t => t.PageNO == pageno);
            //var sh021s = busEntity.SH021.Where(t => t.PageNO == pageno);
            //var sh031s = busEntity.SH031.Where(t => t.PageNO == pageno);
            //var sh032s = busEntity.SH032.Where(t => t.PageNO == pageno);
            //var sh033s = busEntity.SH033.Where(t => t.PageNO == pageno);
            //var sh034s = busEntity.SH034.Where(t => t.PageNO == pageno);
            //var sh035s = busEntity.SH035.Where(t => t.PageNO == pageno); 
            var sh036s = busEntity.SH036.Where(t => t.PageNO == pageno);
            var sh037s = busEntity.SH037.Where(t => t.PageNO == pageno);
            var sh038s = busEntity.SH038.Where(t => t.PageNO == pageno);
            var sh041s = busEntity.SH041.Where(t => t.PageNO == pageno);
            var sh042s = busEntity.SH042.Where(t => t.PageNO == pageno);
            var sh043s = busEntity.SH043.Where(t => t.PageNO == pageno);
            var sh044s = busEntity.SH044.Where(t => t.PageNO == pageno);
            var sh045s = busEntity.SH045.Where(t => t.PageNO == pageno);
            var sh051s = busEntity.SH051.Where(t => t.PageNO == pageno);
            var aggs = busEntity.AggAccRecord.Where(t => t.PageNo == pageno);

            foreach (var sh011 in sh011s)
            {
                busEntity.SH011.DeleteObject(sh011);
            }
            /*foreach (var sh021 in sh021s)
            {
                busEntity.SH021.DeleteObject(sh021);
            }

            foreach (var sh031 in sh031s)
            {
                busEntity.SH031.DeleteObject(sh031);
            }
            foreach (var sh032 in sh032s)
            {
                busEntity.SH032.DeleteObject(sh032);
            }
            foreach (var sh033 in sh033s)
            {
                busEntity.SH033.DeleteObject(sh033);
            }
            foreach (var sh034 in sh034s)
            {
                busEntity.SH034.DeleteObject(sh034);
            }
            foreach (var sh035 in sh035s)
            {
                busEntity.SH035.DeleteObject(sh035);
            } */
            foreach (var sh036 in sh036s)
            {
                busEntity.SH036.DeleteObject(sh036);
            }
            foreach (var sh037 in sh037s)
            {
                busEntity.SH037.DeleteObject(sh037);
            }
            foreach (var sh038 in sh038s)
            {
                busEntity.SH038.DeleteObject(sh038);
            }

            foreach (var sh041 in sh041s)
            {
                busEntity.SH041.DeleteObject(sh041);
            }
            foreach (var sh042 in sh042s)
            {
                busEntity.SH042.DeleteObject(sh042);
            }
            foreach (var sh043 in sh043s)
            {
                busEntity.SH043.DeleteObject(sh043);
            }
            foreach (var sh044 in sh044s)
            {
                busEntity.SH044.DeleteObject(sh044);
            }
            foreach (var sh045 in sh045s)
            {
                busEntity.SH045.DeleteObject(sh045);
            }
            foreach (var sh051 in sh051s)
            {
                busEntity.SH051.DeleteObject(sh051);
            }

            foreach (var agg in aggs)
            {
                busEntity.AggAccRecord.DeleteObject(agg);
            }

            var reportTitle = busEntity.ReportTitle.Where(t => t.PageNO == pageno);
            if (reportTitle.Any())
            {
                busEntity.ReportTitle.DeleteObject(reportTitle.SingleOrDefault());
            }

            try
            {
                busEntity.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }

        public bool ClearOldData(BusinessEntities busEntity, int pageno, bool clearReportTitle)
        {
            bool success = false;
            var sh011s = busEntity.SH011.Where(t => t.PageNO == pageno);
            var sh021s = busEntity.SH021.Where(t => t.PageNO == pageno);
            var sh031s = busEntity.SH031.Where(t => t.PageNO == pageno);
            var sh032s = busEntity.SH032.Where(t => t.PageNO == pageno);
            var sh033s = busEntity.SH033.Where(t => t.PageNO == pageno);
            var sh034s = busEntity.SH034.Where(t => t.PageNO == pageno);
            var sh035s = busEntity.SH035.Where(t => t.PageNO == pageno);
            var sh036s = busEntity.SH036.Where(t => t.PageNO == pageno);
            var sh037s = busEntity.SH037.Where(t => t.PageNO == pageno);
            var sh038s = busEntity.SH038.Where(t => t.PageNO == pageno);
            var sh041s = busEntity.SH041.Where(t => t.PageNO == pageno);
            var sh042s = busEntity.SH042.Where(t => t.PageNO == pageno);
            var sh043s = busEntity.SH043.Where(t => t.PageNO == pageno);
            var sh044s = busEntity.SH044.Where(t => t.PageNO == pageno);
            var sh045s = busEntity.SH045.Where(t => t.PageNO == pageno);
            var aggs = busEntity.AggAccRecord.Where(t => t.PageNo == pageno);

            foreach (var sh011 in sh011s)
            {
                busEntity.SH011.DeleteObject(sh011);
            }
            foreach (var sh021 in sh021s)
            {
                busEntity.SH021.DeleteObject(sh021);
            }

            foreach (var sh031 in sh031s)
            {
                busEntity.SH031.DeleteObject(sh031);
            }
            foreach (var sh032 in sh032s)
            {
                busEntity.SH032.DeleteObject(sh032);
            }
            foreach (var sh033 in sh033s)
            {
                busEntity.SH033.DeleteObject(sh033);
            }
            foreach (var sh034 in sh034s)
            {
                busEntity.SH034.DeleteObject(sh034);
            }
            foreach (var sh035 in sh035s)
            {
                busEntity.SH035.DeleteObject(sh035);
            } foreach (var sh036 in sh036s)
            {
                busEntity.SH036.DeleteObject(sh036);
            }
            foreach (var sh037 in sh037s)
            {
                busEntity.SH037.DeleteObject(sh037);
            }
            foreach (var sh038 in sh038s)
            {
                busEntity.SH038.DeleteObject(sh038);
            }

            foreach (var sh041 in sh041s)
            {
                busEntity.SH041.DeleteObject(sh041);
            }
            foreach (var sh042 in sh042s)
            {
                busEntity.SH042.DeleteObject(sh042);
            }
            foreach (var sh043 in sh043s)
            {
                busEntity.SH043.DeleteObject(sh043);
            }
            foreach (var sh044 in sh044s)
            {
                busEntity.SH044.DeleteObject(sh044);
            }
            foreach (var sh045 in sh045s)
            {
                busEntity.SH045.DeleteObject(sh045);
            }

            foreach (var agg in aggs)
            {
                busEntity.AggAccRecord.DeleteObject(agg);
            }

            if (clearReportTitle)
            {
                var reportTitle = busEntity.ReportTitle.Where(t => t.PageNO == pageno);
                if (reportTitle.Any())
                {
                    busEntity.ReportTitle.DeleteObject(reportTitle.SingleOrDefault());
                }
            }

            try
            {
                busEntity.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }

/*        public string SendReport(int pageno, string time)
        {
            
        }*/

        /*public string SendReport(int pageno, string time)
        {
            string response = "";
            int limit = int.Parse(Request["limit"]);
            BusinessEntities local_busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
            ReportTitle rpt = local_busEntity.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault();
            rpt.State = 3;
            rpt.SendTime = DateTime.Now;
            local_busEntity.SaveChanges();

            DateTime start_time = DateTime.Parse(time + " 00:00:00");
            DateTime end_time = DateTime.Parse(time + " 23:59:59");

            BusinessEntities upper_busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit - 1);
            string unitCode = Request["unitcode"];
            if (limit == 4)
            {
                unitCode = unitCode.Substring(0, 4);  //市级
            }
            else if (limit == 3)
            {
                unitCode = unitCode.Substring(0, 2);  //省级
            }
            var reportTitle =
                upper_busEntity.ReportTitle.Where(t => t.UnitCode.StartsWith(unitCode) && t.StartDateTime >= start_time && t.StartDateTime <= end_time && t.Del != 1 && t.ORD_Code == rpt.ORD_Code);

            ReportTitle rptTitle = null;
            if (reportTitle != null && reportTitle.Any()) //之前存在过
            {
                rptTitle = reportTitle.SingleOrDefault();
                rptTitle.State = 0;
            }
            else   //之前不存在
            {
                rptTitle = new ReportTitle();
                rptTitle.PageNO = upper_busEntity.ReportTitle.Any()
                    ? upper_busEntity.ReportTitle.Max(t => t.PageNO) + 1
                    : 1;
                rptTitle.ORD_Code = rpt.ORD_Code;
                rptTitle.RPTType_Code = rpt.RPTType_Code;
                rptTitle.StatisticalCycType = rpt.StatisticalCycType;
                if (rpt.ORD_Code == "SH01")
                {
                    rptTitle.SourceType = 1;
                }
                else
                {
                    rptTitle.SourceType = 0;
                }
                if (limit == 4)
                {
                    rptTitle.UnitCode = unitCode + "0000";
                }
                else if(limit == 3)
                {
                    rptTitle.UnitCode = unitCode + "000000";
                }
                //rptTitle.UnitName = ;
                rptTitle.StartDateTime = rpt.StartDateTime;
                rptTitle.EndDateTime = rpt.EndDateTime;
                rptTitle.Del = 0;
                rptTitle.ReceiveState = 0;
                rptTitle.State = 0;
                rptTitle.WriterTime = DateTime.Now;
                rptTitle.SendOperType = 0;
                rptTitle.LastUpdateTime = DateTime.Now;
                rptTitle.CopyPageNO = 0;
                
                upper_busEntity.ReportTitle.AddObject(rptTitle);
                upper_busEntity.SaveChanges();
            }
            ReConpute(rptTitle, limit - 1, start_time, end_time);

            response = "1";

            return response;
        }*/

        public decimal? Divide(decimal? a, decimal? b, string n)
        {
            double arg1 = (a != null && a > 0) ? Convert.ToDouble(a) : 0;
            double arg2 = (b != null && b > 0) ? Convert.ToDouble(b) : 0;
            if (arg1 > 0 && arg2 > 0)
            {
                double result = (arg1/arg2)*100;
                n = string.IsNullOrEmpty(n) ? "0.00" : n;

                return Convert.ToDecimal(result.ToString(n));
            }
            else
            {
                return null;
            }
        }

        /*public bool ReConpute(ReportTitle reportTitle, int limit, DateTime start_time, DateTime end_time)
        {
            bool flag = false;
            BusinessEntities local_busEntity = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(limit);

            if (!ClearOldData(local_busEntity, reportTitle.PageNO, false)) //删除之前的数据
            {
                return false;
            }

            BusinessEntities under_busEntity = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(limit + 1);
            string unit_code = "";
            if (limit == 3)
            {
                unit_code = reportTitle.UnitCode.Substring(0, 4);
            }
            else
            {
                unit_code = reportTitle.UnitCode.Substring(0, 2);
            }

            var list = (from t in under_busEntity.ReportTitle
                where t.ORD_Code == reportTitle.ORD_Code && t.State == 3 && t.Del != 1 && t.StartDateTime >= start_time &&
                      t.StartDateTime <= end_time && t.UnitCode.StartsWith(unit_code) && t.CopyPageNO == 0
                select new
                {
                    t.PageNO,
                    t.UnitCode
                }).ToList();
            string pagenos = "";

            AggAccRecord aggacc = null;
            for (int i = 0; i < list.Count; i++)
            {
                aggacc = new AggAccRecord();
                aggacc.PageNo = reportTitle.PageNO;
                aggacc.ORD_Code = reportTitle.ORD_Code;
                aggacc.OperateType = 1;
                aggacc.SPageNO = list[i].PageNO;
                aggacc.UnitCode = list[i].UnitCode;

                local_busEntity.AggAccRecord.AddObject(aggacc);

                pagenos += list[i].PageNO + ",";
            }
            pagenos = pagenos.Remove(pagenos.Length - 1);

            string sql = "";
            int tbno = 0;
            string[] fields = null;
            switch (reportTitle.ORD_Code)
            {
                case "SH01": //合计
                    //ZJZF字段在2016年是没有的
                    fields = new[]
                    {
                        "NDZJYS", "NDZJZY", "NDZJDF", "DFZJSJ", "DFZJSX", "DFZJX", "JDWCZY", "JDZYBL", "JDWCSJ","JDSJBL",
                        "JDWCSHIJ", "JDSHIJBL", "JDWCXJ", "JDXJBL", "ZFWCZY", "ZFZYBL", "ZFWCSJ", "ZFSJBL", "ZFWCSHIJ",
                        "ZFSHIJBL", "ZFWCXJ", "ZFXJBL", "ZJZF"
                    };
                    sql =
                        "select sum(TBNO) as TBNO,UnitCode,sum(PageNO) as PageNO,DataOrder,JSNR " + CreateSumSql(fields) +
                        " from SH011 where pageno in(" +
                        pagenos + ") and UnitCode != 'HJ' group by UnitCode,DataOrder,JSNR";
                    var sh011s = under_busEntity.ExecuteStoreQuery<SH011>(sql).ToList();
                    tbno = local_busEntity.SH011.Any() ? local_busEntity.SH011.Max(t => t.TBNO) + 1 : 1;

                    if (sh011s.Any())
                    {
                        sql = "select sum(TBNO) as TBNO,UnitCode,sum(PageNO) as PageNO,DataOrder,JSNR" +
                              CreateSumSql(fields) +
                              " from SH011 where pageno in(" +
                              pagenos + ") and UnitCode = 'HJ' group by UnitCode,DataOrder,JSNR";  //计算出合计行
                        SH011 hj = under_busEntity.ExecuteStoreQuery<SH011>(sql).SingleOrDefault();
                        hj.TBNO = tbno++;
                        hj.PageNO = reportTitle.PageNO;

                        hj.JDZYBL = Divide(hj.JDWCZY, hj.NDZJZY, null);
                        hj.JDSJBL = Divide(hj.JDWCSJ, hj.DFZJSJ, null);
                        hj.JDSHIJBL = Divide(hj.JDWCSHIJ, hj.DFZJSX, null);
                        hj.JDXJBL = Divide(hj.JDWCXJ, hj.DFZJX, null);
                        hj.ZFZYBL = Divide(hj.ZFWCZY, hj.NDZJZY, null);
                        hj.ZFSJBL = Divide(hj.ZFWCSJ, hj.DFZJSJ, null);
                        hj.ZFSHIJBL = Divide(hj.ZFWCSHIJ, hj.DFZJSX, null);

                        local_busEntity.SH011.AddObject(hj);

                        foreach (SH011 sh011 in sh011s)
                        {
                            sh011.TBNO = tbno++;
                            sh011.PageNO = reportTitle.PageNO;

                            sh011.JDZYBL = Divide(sh011.JDWCZY, sh011.NDZJZY, null);
                            sh011.JDSJBL = Divide(sh011.JDWCSJ, sh011.DFZJSJ, null);
                            sh011.JDSHIJBL = Divide(sh011.JDWCSHIJ, sh011.DFZJSX, null);
                            sh011.JDXJBL = Divide(sh011.JDWCXJ, sh011.DFZJX, null);
                            sh011.ZFZYBL = Divide(sh011.ZFWCZY, sh011.NDZJZY, null);
                            sh011.ZFSJBL = Divide(sh011.ZFWCSJ, sh011.DFZJSJ, null);
                            sh011.ZFSHIJBL = Divide(sh011.ZFWCSHIJ, sh011.DFZJSX, null);

                            local_busEntity.SH011.AddObject(sh011);
                        }
                    }
                    break;
                case "SH02": //直接取数据
                    sql = "select * from SH021 where pageno in(" + pagenos + ")";
                    var sh021s = under_busEntity.ExecuteStoreQuery<SH021>(sql).ToList();
                    tbno = local_busEntity.SH021.Any() ? local_busEntity.SH021.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH021 sh021 in sh021s)
                    {
                        sh021.TBNO = tbno++;
                        sh021.PageNO = reportTitle.PageNO;
                        local_busEntity.SH021.AddObject(sh021);
                    }
                    break;
                case "SH03": //直接取数据
                    sql = "select * from SH031 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                    var sh031s = under_busEntity.ExecuteStoreQuery<SH031>(sql).ToList();
                    tbno = local_busEntity.SH031.Any() ? local_busEntity.SH031.Max(t => t.TBNO) + 1 : 1;

                    if (sh031s.Any())
                    {
                        fields = new[] //,"","","","","","","","","","","","",""
                        {
                            "YLZJH", "YLZWC", "SWZJH", "SWZWC", "WXTDJH", "WXTHWC", "WXJCJH", "WXJSWC", "TXSPJCZJH",
                            "TXSPJCZWC", "TXSPJCZCJH", "TXSPJCZCWC"
                        };
                        sql = "select sum(TBNO) as TBNO,UnitCode,sum(PageNO) as PageNO,DataOrder,SSX" +
                              CreateSumSql(fields) +
                              " from SH031 where pageno in(" +
                              pagenos + ") and UnitCode = 'HJ' group by UnitCode,DataOrder,SSX";  //计算出合计行
                        SH031 hj = under_busEntity.ExecuteStoreQuery<SH031>(sql).SingleOrDefault();
                        hj.TBNO = tbno++;
                        hj.PageNO = reportTitle.PageNO;
                        local_busEntity.SH031.AddObject(hj);
                        foreach (SH031 sh031 in sh031s)
                        {
                            sh031.TBNO = tbno++;
                            sh031.PageNO = reportTitle.PageNO;
                            local_busEntity.SH031.AddObject(sh031);
                        }
                    }

                    sql = "select * from SH032 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                    var sh032s = under_busEntity.ExecuteStoreQuery<SH032>(sql).ToList();
                    tbno = local_busEntity.SH032.Any() ? local_busEntity.SH032.Max(t => t.TBNO) + 1 : 1;

                    if (sh032s.Any())
                    {
                        fields = new[] //,"","","","","","","","","","","","",""
                        {
                            "WXGBJH","WXGBWC","YLBJJH","YLBJWC","SWZJH","SWZWC","BJQJH","BJQWC","LGHJH","LGHWC"
                        };
                        sql = "select sum(TBNO) as TBNO,UnitCode,sum(PageNO) as PageNO,DataOrder,SSX" +
                              CreateSumSql(fields) +
                              " from SH032 where pageno in(" +
                              pagenos + ") and UnitCode = 'HJ' group by UnitCode,DataOrder,SSX";  //计算出合计行
                        SH032 hj = under_busEntity.ExecuteStoreQuery<SH032>(sql).SingleOrDefault();
                        hj.TBNO = tbno++;
                        hj.PageNO = reportTitle.PageNO;
                        local_busEntity.SH032.AddObject(hj);
                        foreach (SH032 sh032 in sh032s)
                        {
                            sh032.TBNO = tbno++;
                            sh032.PageNO = reportTitle.PageNO;
                            local_busEntity.SH032.AddObject(sh032);
                        }
                    }

                    /*sql = "select * from SH033 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                    var sh033s = under_busEntity.ExecuteStoreQuery<SH033>(sql).ToList();
                    tbno = local_busEntity.SH033.Any() ? local_busEntity.SH033.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH033 sh033 in sh033s)
                    {
                        sh033.TBNO = tbno++;
                        sh033.PageNO = reportTitle.PageNO;
                        local_busEntity.SH033.AddObject(sh033);
                    }#1#

                    /*sql = "select * from SH034 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                    var sh034s = under_busEntity.ExecuteStoreQuery<SH034>(sql).ToList();
                    tbno = local_busEntity.SH034.Any() ? local_busEntity.SH034.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH034 sh034 in sh034s)
                    {
                        sh034.TBNO = tbno++;
                        sh034.PageNO = reportTitle.PageNO;
                        local_busEntity.SH034.AddObject(sh034);
                    }#1#

                    /*sql = "select * from SH035 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                    var sh035s = under_busEntity.ExecuteStoreQuery<SH035>(sql).ToList();
                    tbno = local_busEntity.SH035.Any() ? local_busEntity.SH035.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH035 sh035 in sh035s)
                    {
                        sh035.TBNO = tbno++;
                        sh035.PageNO = reportTitle.PageNO;
                        local_busEntity.SH035.AddObject(sh035);
                    }#1#

                    sql = "select * from SH036 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                    var sh036s = under_busEntity.ExecuteStoreQuery<SH036>(sql).ToList();
                    tbno = local_busEntity.SH036.Any() ? local_busEntity.SH036.Max(t => t.TBNO) + 1 : 1;
                    if (sh036s.Any())
                    {
                        fields = new[]
                        {
                            "XYAJH","XYAWC","XZYAJH","XZYAWC","CYAJH","CYAWC","QTYAJH","QTYAWC"
                        };
                        sql = "select sum(TBNO) as TBNO,UnitCode,sum(PageNO) as PageNO,DataOrder,SSX" +
                              CreateSumSql(fields) +
                              " from SH036 where pageno in(" +
                              pagenos + ") and UnitCode = 'HJ' group by UnitCode,DataOrder,SSX";  //计算出合计行
                        SH036 hj = under_busEntity.ExecuteStoreQuery<SH036>(sql).SingleOrDefault();
                        hj.TBNO = tbno++;
                        hj.PageNO = reportTitle.PageNO;
                        local_busEntity.SH036.AddObject(hj);
                        foreach (SH036 sh036 in sh036s)
                        {
                            sh036.TBNO = tbno++;
                            sh036.PageNO = reportTitle.PageNO;
                            local_busEntity.SH036.AddObject(sh036);
                        }
                    }

                    sql = "select * from SH037 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                    var sh037s = under_busEntity.ExecuteStoreQuery<SH037>(sql).ToList();
                    tbno = local_busEntity.SH037.Any() ? local_busEntity.SH037.Max(t => t.TBNO) + 1 : 1;

                    if (sh037s.Any())
                    {
                        fields = new[]
                        {
                            "XCLJH","XCLWC","JSPJH","JSPWC","MBKJH","MBKWC","GPJH","GPWC","SCJH","SCWC"
                        };
                        sql = "select sum(TBNO) as TBNO,UnitCode,sum(PageNO) as PageNO,DataOrder,SSX" +
                              CreateSumSql(fields) +
                              " from SH037 where pageno in(" +
                              pagenos + ") and UnitCode = 'HJ' group by UnitCode,DataOrder,SSX";  //计算出合计行
                        SH037 hj = under_busEntity.ExecuteStoreQuery<SH037>(sql).SingleOrDefault();
                        hj.TBNO = tbno++;
                        hj.PageNO = reportTitle.PageNO;
                        local_busEntity.SH037.AddObject(hj);
                        foreach (SH037 sh037 in sh037s)
                        {
                            sh037.TBNO = tbno++;
                            sh037.PageNO = reportTitle.PageNO;
                            local_busEntity.SH037.AddObject(sh037);
                        }
                    }

                    sql = "select * from SH038 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                    var sh038s = under_busEntity.ExecuteStoreQuery<SH038>(sql).ToList();
                    tbno = local_busEntity.SH038.Any() ? local_busEntity.SH038.Max(t => t.TBNO) + 1 : 1;

                    if (sh038s.Any())
                    {
                        fields = new[]
                        {
                            "PXCCJH","PXCCWC","PXRSJH","PXRSWC","YLCCJH","YLCCWC","YLRCJH","YLRCWC"
                        };
                        sql = "select sum(TBNO) as TBNO,UnitCode,sum(PageNO) as PageNO,DataOrder,SSX" +
                              CreateSumSql(fields) +
                              " from SH038 where pageno in(" +
                              pagenos + ") and UnitCode = 'HJ' group by UnitCode,DataOrder,SSX";  //计算出合计行
                        SH038 hj = under_busEntity.ExecuteStoreQuery<SH038>(sql).SingleOrDefault();
                        hj.TBNO = tbno++;
                        hj.PageNO = reportTitle.PageNO;
                        local_busEntity.SH038.AddObject(hj);
                        foreach (SH038 sh038 in sh038s)
                        {
                            sh038.TBNO = tbno++;
                            sh038.PageNO = reportTitle.PageNO;
                            local_busEntity.SH038.AddObject(sh038);
                        }
                    }
                    break;
                case "SH04":
                    sql = "select * from SH041 where pageno in(" + pagenos + ")";
                    var sh041s = under_busEntity.ExecuteStoreQuery<SH041>(sql).ToList();
                    tbno = local_busEntity.SH041.Any() ? local_busEntity.SH041.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH041 sh041 in sh041s)
                    {
                        sh041.TBNO = tbno++;
                        sh041.PageNO = reportTitle.PageNO;
                        local_busEntity.SH041.AddObject(sh041);
                    }

                    sql = "select * from SH042 where pageno in(" + pagenos + ")";
                    var sh042s = under_busEntity.ExecuteStoreQuery<SH042>(sql).ToList();
                    tbno = local_busEntity.SH042.Any() ? local_busEntity.SH042.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH042 sh042 in sh042s)
                    {
                        sh042.TBNO = tbno++;
                        sh042.PageNO = reportTitle.PageNO;
                        local_busEntity.SH042.AddObject(sh042);
                    }

                    sql = "select * from SH043 where pageno in(" + pagenos + ")";
                    var sh043s = under_busEntity.ExecuteStoreQuery<SH043>(sql).ToList();
                    tbno = local_busEntity.SH043.Any() ? local_busEntity.SH043.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH043 sh043 in sh043s)
                    {
                        sh043.TBNO = tbno++;
                        sh043.PageNO = reportTitle.PageNO;
                        local_busEntity.SH043.AddObject(sh043);
                    }

                    sql = "select * from SH044 where pageno in(" + pagenos + ")";
                    var sh044s = under_busEntity.ExecuteStoreQuery<SH044>(sql).ToList();
                    tbno = local_busEntity.SH044.Any() ? local_busEntity.SH044.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH044 sh044 in sh044s)
                    {
                        sh044.TBNO = tbno++;
                        sh044.PageNO = reportTitle.PageNO;
                        local_busEntity.SH044.AddObject(sh044);
                    }

                    sql = "select * from SH045 where pageno in(" + pagenos + ")";
                    var sh045s = under_busEntity.ExecuteStoreQuery<SH045>(sql).ToList();
                    tbno = local_busEntity.SH045.Any() ? local_busEntity.SH045.Max(t => t.TBNO) + 1 : 1;
                    foreach (SH045 sh045 in sh045s)
                    {
                        sh045.TBNO = tbno++;
                        sh045.PageNO = reportTitle.PageNO;
                        local_busEntity.SH045.AddObject(sh045);
                    }
                    break;
            }

            local_busEntity.SaveChanges();
            flag = true;

            return flag;
        }*/

        public string CreateSumSql(string[] arr)
        {
            string sql = "";
            for (int i = 0; i < arr.Length; i++)
            {
                sql += " ,sum(" + arr[i] + ") as " + arr[i];
            }

            return sql;
        }

        public ActionResult ViewUnderReport()
        {
            string rptType = Request["rptType"];
            string limit = Request["limit"];
            string unitcode = Request["unitcode"];
            string pageno = Request["pageno"];

            ViewData["Report"] = OpenReport(rptType, 0, limit, unitcode, pageno).Data;

            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/SH.ViewUnderReport.cshtml");
            }
            else
            {
                return View("~/Views/Release/SH.ViewUnderReport.cshtml");
            }
        }

        public ActionResult BenefitReport()
        {
            int limit = int.Parse(Request["limit"]);
            string unitcode = Request["unitcode"];
            BusinessEntities business = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
            var rpts = business.Benefit.Where(t => t.UnitCode == unitcode).ToList();

            string initdata = "{ Reports:[";
            if (rpts.Any())
            {
                foreach (Benefit rpt in rpts)
                {
                    initdata += "{ BArea:'" + rpt.BArea + "',BDate:'" + Convert.ToDateTime(rpt.BDate).ToString("yy年M月d日") + "',TBNO:" + rpt.TBNO + "},";
                }
                initdata = initdata.Remove(initdata.Length - 1);
            }
            initdata += "]}";

            ViewData["InitData"] = initdata;
            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/ReportDetails/NP01/Benefit.cshtml");
            }
            else
            {
                return View("~/Views/Release/ReportDetails/NP01/Benefit.cshtml");
            }
        }

        public void ExportToExcel(int pageno, string ord_code)
        {
            ExcelOper export = new ExcelOper(int.Parse(Request["limit"]));

            switch (ord_code.ToUpper())
            {
                case "SH01":
                    export.To_SH01(pageno);
                    break;
                case "SH02":
                    export.To_SH02(pageno);
                    break;
                case "SH03":
                    export.To_SH03(pageno);
                    break;
                case "SH04":
                    export.To_SH04(pageno);
                    break;
                case "SH05":
                    export.To_SH05(pageno);
                    break;
            }
        }

        public string GetXsqkData(int pageno, string unitcodes)
        {
            string response = "[";
            Entities getEntity = new Entities();
            BusinessEntities business = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(2);

            List<NP011> np011S = business.NP011.Where(t => t.PageNO == pageno).ToList();
            var unitsStrings = unitcodes.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            decimal? sum;
            if (np011S.Any())
            {
                for (int i = 0; i < unitsStrings.Length; i++)
                {
                    sum = np011S.Where(t => t.UnitCode.StartsWith(unitsStrings[i].Substring(0, 4))).Sum(t => t.DQXSL);
                    if (sum != null && Convert.ToDouble(sum) > 0)
                    {
                        response += sum;
                    }
                    else
                    {
                        response += "null";
                    }

                    response += ",";
                }
            }
            else
            {
                for (int i = 0; i < unitsStrings.Length; i++)
                {
                    response += "null,";
                }
            }

            response = response.Remove(response.Length - 1) + "]";

            return response;
        }

        public string SaveBenefitReport(string report)
        {
            if (Session["SESSION_USER"] == null)
            {
                Redirect("/Login");
            }

            string response = "";
            int limit = int.Parse(Request["limit"]);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BusinessEntities business = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);

            try
            {
                Benefit benefit = serializer.Deserialize<Benefit>(report);
                if (benefit.TBNO > 0)
                {
                    response = DeleteBenefitReport(benefit.TBNO);
                    if (!int.TryParse(response, out limit))
                    {
                        return response;
                    }
                }
                else
                {
                    benefit.TBNO = business.Benefit.Any() ? business.Benefit.Max(t => t.TBNO) + 1 : 1;
                }

                business.Benefit.AddObject(benefit);
                business.SaveChanges();

                response = benefit.TBNO.ToString();
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public string DeleteBenefitReport(int tbno)
        {
            string response = "";
            int limit = int.Parse(Request["limit"]);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BusinessEntities business = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);

            try
            {
                Benefit benefit = business.Benefit.Where(t => t.TBNO == tbno).SingleOrDefault();
                if (benefit != null)
                {
                    business.Benefit.DeleteObject(benefit);
                    business.SaveChanges();
                    response = "1";
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public string OpenBenefitReport(int tbno)
        {
            string response = "";
            int limit = int.Parse(Request["limit"]);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BusinessEntities business = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);

            try
            {
                Benefit benefit = business.Benefit.Where(t => t.TBNO == tbno).SingleOrDefault();
                if (benefit != null)
                {
                    response = serializer.Serialize(benefit);
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public void ExportBenefitReport(int tbno)
        {
            int limit = int.Parse(Request["limit"]);

            DocOper export = new DocOper(limit);
            string fileUrl = export.BenefitReport(tbno);
            if (System.IO.File.Exists(fileUrl))  //判断文件是否存在
            {
                Response.Charset = "ISO-8859-1";  //提供下载的文件，不编码的话文件名会乱码 
                Response.ContentEncoding = System.Text.Encoding.UTF8;

                // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(fileUrl));
                Response.ContentType = "Application/octet-stream";

                // 把文件流发送到客户端 
                Response.WriteFile(fileUrl);
                Response.Flush();
                Response.Close();

                System.IO.File.Delete(fileUrl);
            }
            else
            {
                throw new Exception("导出失败！");
            }
        }

        public string SumReport(string pagenos, string ord_code)
        {
            string response = "";
            int limit = int.Parse(Request["limit"]);
            BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit + 1);
            string sql = "";
            string[] fields = null;

            try
            {
                switch (ord_code.ToUpper())
                {
                    case "SH01":
                        fields = new[]
                        {
                            "NDZJYS", "NDZJZY", "NDZJDF", "DFZJSJ", "DFZJSX", "DFZJX", "JDWCZY", "JDZYBL", "JDWCSJ",
                            "JDSJBL","JDWCSHIJ", "JDSHIJBL", "JDWCXJ", "JDXJBL", "ZFWCZY", "ZFZYBL", "ZFWCSJ", "ZFSJBL",
                            "ZFWCSHIJ","ZFSHIJBL", "ZFWCXJ", "ZFXJBL", "ZJZF"
                        };
                        sql = "select sum(TBNO) as TBNO,UnitCode,sum(PageNO) as PageNO,DataOrder,JSNR " + CreateSumSql(fields) +
                        " from SH011 where pageno in(" + pagenos + ") and UnitCode != 'HJ' group by UnitCode,DataOrder,JSNR";
                        var sh011s = busEntity.ExecuteStoreQuery<SH011>(sql).ToList();
                        response = "SH011: ";
                        if (sh011s.Any())
                        {
                            response += JsonConvert.SerializeObject(sh011s,
                                new Newtonsoft.Json.JsonSerializerSettings
                                {
                                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                                });
                        }
                        else
                        {
                            response += "[]";
                        }
                        break;
                    case "SH03":
                        sql = "select * from SH036 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                        var sh036s = busEntity.ExecuteStoreQuery<SH036>(sql).ToList();
                        response = "SH036: ";
                        if (sh036s.Any())
                        {
                            response += JsonConvert.SerializeObject(sh036s,
                                new Newtonsoft.Json.JsonSerializerSettings
                                {
                                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                                }) + ",";
                        }
                        else
                        {
                            response += "[],";
                        }

                        sql = "select * from SH037 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                        var sh037s = busEntity.ExecuteStoreQuery<SH037>(sql).ToList();
                        response += "SH037: ";
                        if (sh037s.Any())
                        {
                            response += JsonConvert.SerializeObject(sh037s,
                                new Newtonsoft.Json.JsonSerializerSettings
                                {
                                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                                }) + ",";
                        }
                        else
                        {
                            response += "[],";
                        }

                        sql = "select * from SH038 where pageno in(" + pagenos + ") and UnitCode != 'HJ'";
                        var sh038s = busEntity.ExecuteStoreQuery<SH038>(sql).ToList();
                        response += "SH038: ";
                        if (sh038s.Any())
                        {
                            response += JsonConvert.SerializeObject(sh038s,
                                new Newtonsoft.Json.JsonSerializerSettings
                                {
                                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                                });
                        }
                        else
                        {
                            response += "[]";
                        }
                        break;
                    case "SH05":
                        sql = "select * from SH051 where pageno in(" + pagenos + ") and DW = '合计'";
                        var sh051s = busEntity.ExecuteStoreQuery<SH051>(sql).ToList();
                        response = "SH051: ";
                        if (sh051s.Any())
                        {
                            response += JsonConvert.SerializeObject(sh051s,
                                new Newtonsoft.Json.JsonSerializerSettings
                                {
                                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                                }) + ",";
                        }
                        else
                        {
                            response += "[],";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return "{" + response + "}";
        }
    }
}
