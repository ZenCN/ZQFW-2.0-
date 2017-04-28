using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DBHelper;
using EntityModel;
using LogicProcessingClass.AuxiliaryClass;
using LogicProcessingClass.Model;
using LogicProcessingClass.ReportOperate;
using Newtonsoft.Json;
using HttpUtility = System.Web.HttpUtility;

namespace ZQFW.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            if (Session["SESSION_USER"] == null)
            {
                return Redirect("/Login");
            }

            int limit = Convert.ToInt32(Request["limit"]); //单位级别
            string unitCode = Request["unitcode"].ToString(); //单位代码
            string unitName = HttpUtility.UrlDecode(Request["unitname"].ToString());
            string ord_code = Request.Cookies["ord_code"].Value;

            UrgeAndReadReport urgeReport = new UrgeAndReadReport();
            TableFieldBaseData tab = new TableFieldBaseData();
            CommonFunction comm = new CommonFunction();
            RiverDistribute river = new RiverDistribute();

            GetHP01Const hp01Const = hp01Const = new GetHP01Const();
            HP01SKR skr = new HP01SKR(); 

            Dictionary<string, string> dic = null;  //new Dictionary<string, string>();
            HttpApplicationState app = System.Web.HttpContext.Current.Application;
            if (app["BaseData"] != null)
            {
                dic = (Dictionary<string, string>) app["BaseData"];
            }
            else
            {
                dic = new Dictionary<string, string>();
            }

            string RiverCode = ""; //获得所有流域对照表 格式：RiverCode:{}
            if (dic.ContainsKey("RiverCode"))
            {
                RiverCode = dic["RiverCode"];
            }
            else
            {
                RiverCode = comm.GetRiverCodeList();
                dic.Add("RiverCode", RiverCode);
            }

            string DeathReason = ""; //获取所有死亡原因 格式：DeathReason:{DeathReasons:[{}]}
            if (dic.ContainsKey("DeathReason"))
            {
                DeathReason = dic["DeathReason"];
            }
            else
            {
                DeathReason = comm.GetDeathReasonList();
                dic.Add("DeathReason", DeathReason);
            }

            string UndersCheck = ""; //获取校核数据 格式：LocalCheck:[{}]
            if (dic.ContainsKey("UndersCheck"))
            {
                UndersCheck = dic["UndersCheck"];
            }
            else
            {
                UndersCheck = "Formula:" + comm.GetProvenceData();
                dic.Add("UndersCheck", UndersCheck);
            }

            string LocalCheck = "Constant: " + tab.QueryCheckBaseData(limit, unitCode);

            string underReservoirCode = "";
            string unitSKR = "";
            if (ord_code == "HP01")
            {
                underReservoirCode = hp01Const.GetUnderReservoirCodeByUnitCode(unitCode, limit);
                unitSKR = skr.GetUnitSKR(unitCode, limit);//单位的死库容与水库的死库容综合到同一个方法里了
            }

            string reservoirs = "Reservoir:[]";
            if (unitCode.StartsWith("15") && ord_code == "NP01")
            {
                reservoirs = "Reservoir:[" + hp01Const.GetNMReservoirCodeByUnitCode(unitCode, limit, unitName) + "]";
            }
            else if (ord_code == "HP01")
            {
                reservoirs = "Reservoir:{" + hp01Const.GetHPConst(unitCode, DateTime.Now.Month) + ",RSC_UC:{" + underReservoirCode + "},SKR:{" + unitSKR + "}}";
            }

            string HPDate = "[]";
            if (ord_code == "HP01" || ord_code == "NP01")
            {
                HPDate = "[" + hp01Const.GetHPDate() + "]";
            }

            string urgeReportList = urgeReport.GetUrgeReportList(limit, unitCode);
           
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

            string fields = ""; //tb55表的数据
            if (dic.ContainsKey(limit + "-fields"))
            {
                fields = dic[limit + "-fields"];
            }
            else
            {
                fields = tab.GetTB55Fields(limit);;
                dic.Add(limit + "-fields", fields);
            }

            string RecentReportInfo = new ViewReportForm().ViewReportTitleInfo(unitCode, limit);

            string RptClass = ""; //表类型 RptClass:[]
            if (dic.ContainsKey("RptClass"))
            {
                RptClass = dic["RptClass"];
            }
            else
            {
                RptClass = comm.GetRptClass();
                dic.Add("RptClass", RptClass);
            }

            string CycType = ""; //周期类型 CycType:[]
            if (dic.ContainsKey("CycType"))
            {
                CycType = dic["CycType"];
            }
            else
            {
                CycType = comm.GetCycType();
                dic.Add("CycType", CycType);
            }

            string zTree = "";
            if (dic.ContainsKey("zTree"))
            {
                zTree = dic["zTree"];
            }
            else
            {
                zTree = GetTreeNode.GetCreateTreeData();
                dic.Add("zTree", zTree);
            }

            if (app["BaseData"] == null)
            {
                app["BaseData"] = dic;
            }

            string Units = ""; //下级单位
            Persistence per = new Persistence();
            Dictionary<string, District> units = per.GetLowerUnits(unitCode);
            if (units.Count > 0)
            {
                foreach (District dis in units.Values)
                {
                    Units += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "',RiverCode:'" +
                             dis.RiverCode + "'},";
                }
                Units = Units.Remove(Units.Length - 1);
            }
            Units = "Unit:{RiverCode:'" + comm.GetRiverCodeByUnitCode(unitCode) + "',Unders:[" + Units + "]}";

            if (ord_code == "NP01" && (limit == 2 || limit == 3))
            {
                FXDICTEntities fxdict = new FXDICTEntities();
                unitCode = limit == 3 ? unitCode : unitCode.Substring(0, 2) + "000000";
                List<TB62_NMReservoir> hj_list = null;
                if (limit == 2)
                {
                    hj_list = fxdict.TB62_NMReservoir.Where(t => t.RSCode == "0").OrderBy(t => t.UnitCode).ToList();
                }
                else
                {
                    hj_list = fxdict.TB62_NMReservoir.Where(t => t.UnitCode == unitCode).ToList();
                }
                string hj_str = ",XSHJ:[";
                if (Enumerable.Any(hj_list))
                {
                    foreach (TB62_NMReservoir hj in hj_list)
                    {
                        hj_str += "{ ZKR:" + (hj.ZKR == null ? 0 : hj.ZKR) + ",SKR:" +
                             (hj.SKR == null ? "0" : hj.SKR) + ",RSName:'" +
                             (hj.RSName == null ? "未知" : hj.RSName) + "',UnitName:" +
                             (hj.UnitName == null ? "0" : hj.UnitName) + ",UnitCode:" +
                             (hj.UnitCode == null ? "未知" : hj.UnitCode) + "},";
                    }
                    hj_str = hj_str.Remove(hj_str.Length - 1) + "]";

                    Units = Units.Remove(Units.Length - 1) + hj_str + "}";
                }
            }

            string result = "{" + Units + "," + urgeReportList + "," + RiverCode + "," + DeathReason + "," + reservoirs + 
                            "," + "HPDate:" + HPDate + "," + distributeCode + "," +
                            "RelationCheck:{" + LocalCheck + "," + UndersCheck + "}," + "Field:{" + fields + "}," +
                            RecentReportInfo + ",Select:{" + RptClass + "," + CycType + "},zTree:{" + zTree + "}}";

            ViewData["InitData"] = result;

            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/Index.cshtml");
            }
            else
            {
                return View("~/Views/Release/Index.cshtml");
            }
        }

        public ActionResult Frame()
        {
            return View("~/Views/Frame.cshtml");
        }

        public ActionResult ViewUnderReport()
        {
            TableFieldBaseData tab = new TableFieldBaseData();
            if (Request["queryUnderUnits"] == "1")
            {
                if (Request["level"] == "5")
                {
                    ViewData["Unders"] = "[]";
                }
                else
                {
                    ViewData["Unders"] = new LoginController().ReadUnder(Request["unitcode"]);
                }
            }
            else
            {
                ViewData["Unders"] = "window.opener.UnderUnits['" + Request["rptType"] + "']['" + Request["unitcode"] + "']";
            }

            ViewData["RiverUnits"] = "[]";
            if (Request["RiverCode"] != null)
            {
                ViewData["RiverUnits"] = tab.GetRiverUnits(Request["RiverCode"]);
            }

            ViewData["Field"] = "{" + tab.GetTB55Fields(Convert.ToInt32(Request["level"])) + "}";
            ViewData["Reservoir"] = new GetHP01Const().GetReservoirByUnitCode(Request["unitcode"]).Replace("Reservoir:", "");
            ViewData["Report"] = OpenReport(Request["rptType"], int.Parse(Request["sourceType"]), Request["level"], Request["unitcode"], Request["pageno"]).Data;

            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/ViewUnderReport.cshtml");
            }
            else 
            {
                return View("~/Views/Release/ViewUnderReport.cshtml");
            }
        }

        public string SearchSameReport()
        {
            int limit = int.Parse(Request.Cookies["Limit"].Value.ToString());
            string unitcode = Request.Cookies["unitcode"].Value.ToString();
            string result = new ReportHelpClass().FindSameReport(limit, Request["ORD_Code"],
                int.Parse(Request["StatisticalCycType"]), Request["StartDateTime"], Request["EndDateTime"],
                int.Parse(Request["SourceType"]), unitcode);
            return result;
        }

        /// <summary>读取当前登录单位需要的表格模板
        /// </summary>
        /// <param name="url"></param>
        /// tableType：报表类型，洪涝：HL，蓄水:HP
        /// operateType：报表操作类型，编辑：edit，查看:view
        public void GetTemmplate(string type, string url)
        {
            Response.ContentType = "text/plain";

            if (type == "rpt")
            {
                string[] path = url.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string result = "";

                if (path[0] == "HL")
                {
                    ReadHtm r = new ReadHtm();
                    result = r.ReadHtmByUnitCode(Request["unitcode"], path[0], path[1]);
                    Response.Write(result);
                }
                else  //HP
                {
                    string str = Request.Cookies["unitcode"].Value.ToString().Substring(0, 2);
                    str = "~/Scripts/Templates/Public/XS/" + str + "/" + path[1] + ".htm";
                    if (System.IO.File.Exists(Server.MapPath(str)))
                    {
                        Response.WriteFile(str);
                    }
                    else
                    {
                        Response.WriteFile("~/Scripts/Templates/Public/HP/Common/" + path[1] + ".htm");
                    }
                }
            }
            else
            {
                Response.WriteFile("~/Scripts/Templates/" + url.Replace(".", "/") + ".htm");
            }
        }

        /// <summary>获取当年或往年的树形结构数据
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="unitLimit"></param>
        /// <param name="rptClass"></param>
        /// <param name="limitType"></param>
        /// <param name="cycType"></param>
        /// <param name="filterByUnitCode"></param>
        /// <param name="isCurYear">如果是当年的数据：true查看往年的传入：false</param>
        /// <returns></returns>
        public JsonResult GetTreeData(string unitCode, string unitLimit, string rptClass, string limitType,
            string cycType, string filterByUnitCode, string isCurYear)
        {
            if (unitCode.StartsWith("15") && rptClass == "NP01")
            {
                unitLimit = "2";  //默认省级
            }
            GetTreeNode GTN = new GetTreeNode(int.Parse(unitLimit), Convert.ToInt32(limitType));
            JsonResult JR = new JsonResult();
            rptClass = rptClass == null ? "" : rptClass;
            cycType = cycType == null ? "" : cycType;
            filterByUnitCode = filterByUnitCode == null ? "" : filterByUnitCode;
            string result = "";
            isCurYear = isCurYear == null ? "1" : isCurYear;
            bool queryOneUnit = Request.Cookies["unitcode"].Value != unitCode;
            if (isCurYear != "0")
            {
                result = GTN.GetCurYearTreeNodeData(int.Parse(unitLimit), unitCode, int.Parse(limitType), "", cycType,
                    filterByUnitCode, rptClass, queryOneUnit);
            }
            else
            {
                result = GTN.GetFormerYearsTreeNodeData(int.Parse(unitLimit), unitCode, int.Parse(limitType), "",
                    cycType, filterByUnitCode, rptClass, queryOneUnit);
            }
            JR = Json(result);
            JR.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return JR;
        }

        public string SaveDeltaReport(string Reports)
        {
            string response = "";

            //创建匿名类型集合以方便反序列化
            var report = new
            {
                ReportTitle = new ReportTitle(),
                HL011 = new List<HL011>(),
                HL012 = new List<HL012>(),
                HL013 = new List<HL013>(),
                HL014 = new List<HL014>()
            };
            var list = Enumerable.Repeat(report, 1).ToList();

            bool is_new = false;
            int pageno = 0;
            int limit = int.Parse(Request.Cookies["limit"].Value);
            ReportTitle report_title = null;
            ReportHelpClass rptHelp = new ReportHelpClass();

            BusinessEntities under_db = Persistence.GetDbEntities(limit + 1);
            try
            {
                response = "[";
                AggAccRecord agg = null;
                var rpts = JsonConvert.DeserializeAnonymousType(Reports, list);
                for (var i = 0; i < rpts.Count; i++)
                {
                    if (rpts[i].ReportTitle.PageNO > 0) //删除之前的
                    {
                        is_new = false;
                        pageno = rpts[i].ReportTitle.PageNO;
                        report_title = under_db.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault();
                        under_db.ExecuteStoreCommand("delete from hl011 where pageno=" + rpts[i].ReportTitle.PageNO + ";"
                                                     + "delete from hl012 where pageno=" + rpts[i].ReportTitle.PageNO + ";"
                                                     + "delete from hl013 where pageno=" + rpts[i].ReportTitle.PageNO + ";"
                                                     + "delete from hl014 where pageno=" + rpts[i].ReportTitle.PageNO + ";"
                                                     + "delete from aggaccrecord where pageno=" + rpts[i].ReportTitle.PageNO);
                    }
                    else
                    {
                        is_new = true;
                        report_title = rpts[i].ReportTitle;
                        report_title.PageNO = rptHelp.FindMaxPageNO(limit + 1);
                    }

                    response += "{\"UnitCode\":" + report_title.UnitCode + ",\"PageNO\":" + report_title.PageNO + "},";

                    if (rpts[i].HL011 != null)
                    {
                        for (int j = 0; j < rpts[i].HL011.Count; j++)
                        {
                            report_title.HL011.Add(rpts[i].HL011[j]);
                        }
                    }

                    if (rpts[i].HL012 != null)
                    {
                        for (int j = 0; j < rpts[i].HL012.Count; j++)
                        {
                            report_title.HL012.Add(rpts[i].HL012[j]);
                        }
                    }

                    if (rpts[i].HL013 != null)
                    {
                        for (int j = 0; j < rpts[i].HL013.Count; j++)
                        {
                            report_title.HL013.Add(rpts[i].HL013[j]);
                        }
                    }

                    if (rpts[i].HL014 != null)
                    {
                        for (int j = 0; j < rpts[i].HL014.Count; j++)
                        {
                            report_title.HL014.Add(rpts[i].HL014[j]);
                        }
                    }

                    if (is_new)
                    {
                        under_db.ReportTitle.AddObject(report_title);
                    }
                }

                //local_db.SaveChanges();
                under_db.SaveChanges();
                response = response.Remove(response.Length - 1) + "]";
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 保存或修改报表（需要通过POST请求）如果不是新建,IsNew参数必须为false，PageNO为正确的页号
        /// </summary>
        /// <returns>操作成功返回该报表的信息，失败返回"错误消息："，如果仅差值表保存失败，返回“错误消息：差值表报表保存失败!”</returns>
        [HttpPost]
        public JsonResult SaveUpdateReport()
        {
            JsonResult jsr = new JsonResult();
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReportHelpClass rptHelp = new ReportHelpClass();
            string temp = "";
            int limit = int.Parse(Request["limit"]);
            string unitCode = Request["unitcode"];
            string diffData = Request["DiffData"] == null ? "" : Request["DiffData"];
            int isrivercode = Request["IsToRiverCode"] == null ? 0 : Convert.ToInt32(Request["report"]); //是否进行流域分配
            string dataStr = Request["report"] == null ? "" : Request["report"];
            SaveOrUpdateReport saveUpdateRpt = new SaveOrUpdateReport();
            temp = saveUpdateRpt.Save(limit, unitCode, diffData, isrivercode, dataStr, Request["DiffPageNOs"]);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        public string DelAffix(int pageno, string tbnos, string urls)
        {
            SaveOrUpdateReport op = new SaveOrUpdateReport();
            string result = op.DelAffix(pageno, tbnos, urls);

            return result;
        }

        private string Save(int limit, string unitCode, string diffData, int isrivercode, string dataStr)
        {
            string temp = "";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReportHelpClass rptHelp = new ReportHelpClass();
            Hashtable report = serializer.Deserialize<Hashtable>(dataStr);
            string reportTitle = serializer.Serialize(report["ReportTitle"]);
            string aggAcc = serializer.Serialize(report["SourceReport"]);
            int pageNO = rptHelp.FindMaxPageNO(limit); //找到最大的页号并加一
            if (dataStr == "")
            {
                temp = "错误消息："; //不成功
            }
            else
            {
                Hashtable ReportTitle = serializer.Deserialize<Hashtable>(reportTitle); //表头信息
                bool isNew = ReportTitle["PageNO"].ToString() == "0" ? true : false;
                ///差值表diffData中如果有正确的PageNO参数，则是修改，否则是新建
                if (ReportTitle["PageNO"].ToString() == "0")
                {
                    ReportTitle["State"] = 0;
                }
                #region----保存差值表----差值数据需要在前台传入数据之后进行调试，现阶段没有数据进行测试调试------------------

                string sourceRptResult = "";
                if (diffData.Trim().Length != 0) //不存在差值数据
                {
                    DifferentialReport DiffReport = new DifferentialReport();
                    if (Convert.ToInt32(ReportTitle["SourceType"].ToString()) == 2) //如果是累计差值表
                    {
                        int sPageNO = 0;
                        sPageNO = pageNO; //新建时用到，如果diffData中正确的传入差值表PageNO，则不会使用sPageNO
                        sourceRptResult = DiffReport.SaveOrUpdate(limit, unitCode, diffData, ReportTitle, sPageNO,
                            ref aggAcc, ref dataStr);
                    }
                    else if (Convert.ToInt32(ReportTitle["SourceType"].ToString()) == 1) //汇总差值表
                    {
                        ArrayList diffRptList = DiffReport.SplitDiffReportByString(diffData);
                        string allDiffPageNOs = null;
                        if (Request["DiffPageNOs"] != null)
                        {
                            allDiffPageNOs = Request["DiffPageNOs"].ToString();
                        }
                        sourceRptResult = DiffReport.SaveOrUpdate(limit, diffRptList, ReportTitle, ref dataStr,
                            ref aggAcc, allDiffPageNOs);
                    }
                }
                //-------------------------------------------------------------------------------------------------------
                #endregion

                Hashtable data = serializer.Deserialize<Hashtable>(dataStr); //把HL011-HL014数据序列化成Hashtable类型
                Hashtable SReport = serializer.Deserialize<Hashtable>("{SourceReport:" + aggAcc + "}"); //汇总的下级表的页号 

                #region 前台传入的流域数据处理

                ///********************************************多流域保存（如：湖南）*****/
                //string rates = report["RiverRates"] == null ? "" : report["RiverRates"].ToString();
                List<RiverInfo> rifs = new List<RiverInfo>();
                if (report["RiverRates"] != null)
                {
                    //Hashtable hdata = serializer.Deserialize<Hashtable>(rates);
                    Dictionary<string, object> dic = (Dictionary<string, object>)report["RiverRates"];
                    foreach (string key in dic.Keys)
                    {
                        RiverInfo rif = new RiverInfo();
                        rif.UnitCode = key.ToString();
                        object[] str1 = serializer.ConvertToType<object[]>(dic[key]); //将里面的数据放入一个对象数组中去
                        for (int i = 0; i < str1.Length; i++)
                        {
                            RiverDataInfo r = serializer.ConvertToType<RiverDataInfo>(str1[i]);
                            //RiverDataInfo对象接收，数组里的对象
                            rif.DRiverRate.Add(r.RiverCode.ToString(), Convert.ToDouble(r.RiverData.ToString()));
                        }
                        rifs.Add(rif);
                    }
                    isrivercode = 1;
                }

                #endregion

                string affix = null;
                if (report["DelAffixURL"].ToString() != "" && report["DelAffixTBNO"].ToString() != "")
                {
                    affix = report["DelAffixURL"].ToString().Replace("..", "~") + ";" + report["DelAffixTBNO"].ToString();
                }

                pageNO = rptHelp.FindMaxPageNO(limit); //找到最大的页号并加一(如果差值表新建保存成功，那么PageNO需要重新取)
                if (!isNew)
                {
                    pageNO = Convert.ToInt32(ReportTitle["PageNO"].ToString());
                }
                string rptType = ReportTitle["ORD_Code"].ToString();
                SaveOrUpdateReport saveUpdateRpt = new SaveOrUpdateReport();
                temp = saveUpdateRpt.SaveUpdateReport(limit, unitCode, pageNO, data, ReportTitle, SReport, isrivercode,
                    isNew, rifs, affix, rptType);
                //temp += sourceRptResult;
                int errorIndex = temp.IndexOf("&"); //错误信息的位置索引,只搜索前面11个字符"saveFalse&&"
                int sourceRptErrorIndex = sourceRptResult.IndexOf("错误消息"); //差值表错误信息的位置索引,只搜索前面11个字符"saveFalse&&"
                if (sourceRptErrorIndex != -1)
                {
                    temp = "错误消息：差值表报表保存失败!" + "{" + sourceRptResult + "}";
                }
                if (errorIndex != -1)
                {
                    temp = "错误消息：报表保存失败!" + temp;
                }
            }
            return temp;
        }


        /// <summary>
        /// 查看报表（优先使用单位级别，否则需要传入单位代码。两者不能同时为空）
        /// 传入UnitCode适用与打开多级报表
        /// </summary>
        /// <returns></returns>
        /// GET: Open/OpenReport
        public JsonResult OpenReport(string rptType, int sourceType, string level, string unitCode, string pageno)
        {
            JsonResult jsr = new JsonResult();

            string result = "";
            string arr = "";

            unitCode = unitCode == null ? Request["unitcode"] : unitCode;
            //string rptType = Request["rptType"];
            //int sourceType = Convert.ToInt32(sourceType); //来源类型 Request["sourceType"]
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
                    result = viewRpt.ViewReportFormInfo(limit, pageNO, rptType);
                }
                else if (sourceType == 1 || sourceType == 2)
                {
                    result = viewRpt.ViewReportFormInfo(limit, pageNO, rptType);
                    arr = viewRpt.GetSourceReportList(pageNO, limit, (sourceType == 2 ? 1 : 0), unitName, unitCode); // "1"表示本级库
                    
                    //打开delta_report
                    //var delta_rpt = viewRpt.GetDeltaReport(pageNO, limit);

                    result = result + "," + arr;  //+ "," + delta_rpt
                }
            }
            jsr = Json("{" + result + "}");
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 发送报表（发送给国家防总暂没做）
        /// </summary>
        /// <returns></returns>
        // GET: /ReportOperate/SendReport
        public JsonResult SendReport()
        {
            JsonResult jsr = new JsonResult();
            ReportTitle rpt = new ReportTitle();
            int limit = Convert.ToInt32(Request["limit"]);
            int pageNO = Convert.ToInt32(Request["pageno"]);
            string unitCode = Request["unitcode"];
            int sendType = Convert.ToInt32(Request["sendType"]);
            string unitName = HttpUtility.UrlDecode(Request["unitname"]);
            string result = "";
            bool excute = true;

            DeleteOrSendReport send = new DeleteOrSendReport(limit);
            if (limit == 2)
            {
                SendXMLFile sxf = new SendXMLFile();
                /*******************************上报国家防总的已经注释理论*****************************/
                int number = sxf.SendReportByXML(pageNO, unitCode, limit); //注意：现在已经注释了上报给国家防总的代码，发布时需要取消注释
                if (number > 0)
                {
                    excute = true;
                }
                else
                {
                    excute = false;
                }
            }
            if (excute)
            {
                result = send.SendReports(limit, pageNO, sendType, unitCode, unitName);
            }
            else
            {
                result = "错误消息：未知错误!";
            }
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 删除报表
        /// </summary>
        /// <returns>成功1，否则返回"错误消息：..."</returns>
        // GET: /ReportOperate/DeleteReport
        public JsonResult DeleteReport()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            DeleteOrSendReport del = null;
            if (Request["type"] == "0")    //删除报表
            {
                int limit = Convert.ToInt32(Request["limit"]);
                del = new DeleteOrSendReport(limit);
                result = del.Delete(Convert.ToInt32(Request["pageno"]), limit, Convert.ToInt32(Request["state"]));
                    //返回"-1"表示不能删除自己已报送的报表
            }
            else if (Request["type"] == "1")  //删除死亡人员信息
            {
                del = new DeleteOrSendReport(5);
                result = del.DeleteDeathInfo(Request["id"].ToString());
            }
            else if (Request["type"] == "2")  //删除附件
            {
                del = new DeleteOrSendReport(Convert.ToInt32(Request["level"]));
                result = del.DeleteAffix(Request["tbnos"]);
            }
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 接收页面与回收站页面查询报表
        /// </summary>
        /// <returns></returns>
        /// GET: /InboxAndRecycle/ServerReport
        public JsonResult InboxRecycleSearch()
        {
            JsonResult jsr = new JsonResult();
            string temp = "";
            int limit = Convert.ToInt32(Request["limit"]);//登录单位的级别
            string unitCode = Request["unitCode"];//登录单位代码
            int searchUnitLimit = Convert.ToInt32(Request["unitType"]);//查询的单位级别，0：本级单位，1：全部下级单位，2：某个下级单位
            string rptClassCode = Request["rptType"] == "" ? Request.Cookies["ord_code"].Value : Request["rptType"];//"0"代表全部
            DateTime startTime = Convert.ToDateTime(Request["startTime"]);//在回收站模块下的时间应处理成年月日的格式
            string type = Request["type"];//接收还是回收站
            DateTime endTime = Convert.ToDateTime(Request["endTime"]);
            int receiveState = Convert.ToInt32(Request["receiveState"]);//接收状态
            int cycType = Request["cycType"] == null ? -2 : Convert.ToInt32(Request["cycType"]);//时段类型，-2：无时段条件;-1：全部时段
            InBox inbox = new InBox();
            temp = inbox.SearchReport(limit, unitCode, searchUnitLimit, startTime, endTime, rptClassCode, cycType, receiveState, type);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 接收或回收站模块下的操作报表
        /// </summary>
        /// <returns>成功：返回操作成功；失败：返回错误消息</returns>
        /// GET: /InboxAndRecycle/HandReport
        public JsonResult ReportOperate()
        {
            JsonResult jsr = new JsonResult();
            string temp = "";
            int limit = Convert.ToInt32(Request["limit"]);//登录单位的级别
            string pageNOs = Request["pagenos"];//报表页号
            int searchUnitLimit = Convert.ToInt32(Request["unitType"]);//查询的单位级别，0：本级单位，1：全部下级单位，2：某个下级单位
            int handleType = Convert.ToInt32(Request["state"]);//操作类型，-1：恢复，1：拒收，2：装入，3：删除
            InBox inbox = new InBox();

            string unitcodes = "";
            string time = "";

            if (handleType == 1)
            {
                unitcodes = Request["unitcodes"];
                time = Request["time"];
            }

            temp = inbox.HandleReport(limit, pageNOs, searchUnitLimit, handleType, unitcodes, time);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>汇总累计
        /// </summary>
        /// <returns></returns>
        public JsonResult SummaryReport()
        {
            int limit = Convert.ToInt32(Request["limit"]);//登录单位的级别
            string pageNOs = Request["pagenos"];//报表页号
            string all_pagenos = Request["all_pagenos"];
            JsonResult jsr = new JsonResult();
            string temp = "";
            string rptType = Request["rptType"];//报表类别，HL01：洪涝表，HP01：蓄水表
            if (rptType == "HL01")
            {
                int typeLimit = Convert.ToInt32(Request["typeLimit"]);//0：本级，1：下级
                string operateType = Request["operateType"];//操作名称，加表：sum,减表sub
                SummaryReportForm sumRpt = new SummaryReportForm();
                string unitCode = Request.Cookies["unitcode"].Value;
                temp = sumRpt.GetSummaryReportFormData(pageNOs, limit, typeLimit, operateType, unitCode);
            }
            else if (rptType == "HP01")
            {
                HPSummarizeReport hpSumRpt = new HPSummarizeReport(limit);
                temp = hpSumRpt.GetSummaryReportFormData(pageNOs, limit);
            }
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>打开打印页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Print()
        {
            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return File("~/Views/Debug/Print.html", "text/html");
            }
            else
            {
                return File("~/Views/Release/Print.html", "text/html");
            }
        }

        public ActionResult ReportData(string ord_code)
        {
            string user_code = Request["UserCode"];

            if (user_code == "45")  //广西直接导出到Word
            {
                //
            }

            string path = "~/Views/";
            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                path += "Debug/ReportDetails/";
            }
            else
            {
                path += "Release/ReportDetails/";
            }

            if (user_code == null)
            {
                path += ord_code + "/Common.html";
            }
            else
            {
                path += ord_code + "/" + user_code + ".html";
            }

            if (!System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
            {
                path = "~/Views/";
                if (debug)
                {
                    path += "Debug/ReportDetails/";
                }
                else
                {
                    path += "Release/ReportDetails/";
                }
                path += ord_code + "/Common.html";
            }

            if (ord_code == "HP01")
            {
                DateTime SQTime = Convert.ToDateTime(Request["SQTime"]);
                DateTime TQTime = Convert.ToDateTime(Request["TQTime"]);
                int limit = Convert.ToInt32(Request["limit"]);
                string unitCode = Request["unitcode"].ToString();
                GetHP01Const getHp01 = new GetHP01Const();
                string SQXSL = "";
                string TQXSL = "";
                string str = "";
                SQXSL = getHp01.GetSQXSL(SQTime, limit, unitCode); //上期蓄水量
                str = "{SQXSL:" + SQXSL + ",SQ_MONTH:" + SQTime.Month + ",SQ_DAY:" + SQTime.Day + ",ALLTQXSL:[" +
                      getHp01.GetAllTQXSL(TQTime, limit, unitCode) + "]}";
                ViewData["XSL"] = str;
                path = path.Replace(".html", ".cshtml");

                return View(path);
            }
            else
            {
                return File(path, "text/html");
            }
        }

        /// <summary>获取当前单位去年同期的实际蓄水量（已进行系数和保留小数位的转换）
        /// </summary>
        /// <returns></returns>
        public JsonResult GetHPLastYearData()
        {
            JsonResult jsr = new JsonResult();
            GetHP01Const getHp01 = new GetHP01Const();

            //DateTime startTime = Convert.ToDateTime("2014/2/3");
            //DateTime endTime = Convert.ToDateTime("2014/2/10");
            //int limit = 4;
            //string unitCode = "43018100";

            DateTime startTime = Convert.ToDateTime(Request["startDateTime"]);
            DateTime endTime = Convert.ToDateTime(Request["endDateTime"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string unitCode = Request["unitcode"].ToString();
            string lnxsl = "";
            if (limit == 2)
            {
                lnxsl = getHp01.GetLNTQXSAllUnits(endTime, unitCode);
            }
            else
            {
                lnxsl = getHp01.GetLNTQXSAllUnits(endTime, limit, unitCode);
            }
            string temp = "{LNTQ:" + getHp01.GetHPLastYearSHIJIData(startTime.AddYears(-1), endTime.AddYears(-1), limit, unitCode) + ",SQXSL:{" + getHp01.GetSQXSAllUnits(startTime.AddDays(-1), limit, unitCode) + "},LNTQXSL:{" + lnxsl + "}}";
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }
        /// <summary>获取报表的流域表页号
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRiverPageNOByPageNO()
        {
            JsonResult jsr = new JsonResult();
            RiverDistribute riverData = new RiverDistribute();

            //int limit = Convert.ToInt32(Request["limit"]);
            int pageNO = Convert.ToInt32(Request["pageNO"].ToString());
            string temp = "{Distribute:[" + riverData.GetRiverPageNOByPageNO(pageNO) + "]}";
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        public JsonResult GetHP01Constant()
        {
            JsonResult jsr = new JsonResult();
            GetHP01Const hp01Const = new GetHP01Const();
            int month = Convert.ToInt32(Request["month"].ToString());
            string unitCode = Request.Cookies["unitcode"].Value;
            string result = hp01Const.GetHPConst(unitCode, month);
            result = "{" + result + "}";
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>新建某个时段报表的时候，取某一个时间段的上期和历年同期平均的数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetHP01SQAndLN()
        {
            JsonResult jsr = new JsonResult();
            GetHP01Const hp01Const = new GetHP01Const();
            DateTime endTime = Convert.ToDateTime(Request["endtime"]);
            int limit = Convert.ToInt32(Request["limit"]);
            string unitCode = Request["unitcode"];
            //DateTime endTime = Convert.ToDateTime("2014-4-30");
            //int limit = 2;
            //string unitCode = "43000000";
            string result = "";
            result = "{SQXSL:{" + hp01Const.GetSQXSAllUnits(endTime.AddMonths(-1), limit, unitCode) + "},LNTQXSL:{" + hp01Const.GetLNTQXSAllUnits(endTime, limit, unitCode) + "}}";
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        public JsonResult ReadMsg()
        {
            JsonResult jsr = new JsonResult();
            string unitcode = Request["unitcode"];
            int limit = int.Parse(Request["limit"]);
            jsr = Json(Message.Read(unitcode, limit));
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return jsr;
        }

        public string ClearMsg()
        {
            return Message.Clear(Request["unitcode"], int.Parse(Request["limit"]), "1");
        }

        /// <summary>获取对应表类最后一次填写的表头尾信息
        /// </summary>
        /// <returns></returns>
        public string GetReportTitleInfo()
        {
            JsonResult jsr = new JsonResult();
            string unitcode = Request["unitcode"];
            int limit = int.Parse(Request["limit"]);
            string rptType = Request["rptType"];//洪涝表:HL01,蓄水表：HP01

            ViewReportForm viewReport = new ViewReportForm();
            string temp = "{" + viewReport.ViewReportTitleInfo(unitcode, limit, rptType) + "}";
            //jsr = Json(temp);
            //jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return temp;
        }

        /// <summary>获取市级单位的水库
        /// </summary>
        /// <returns></returns>
        public string GetNMReservoir()
        {
            JsonResult jsr = new JsonResult();
            string unitCode = Request["unitcode"];
            string unitName = HttpUtility.UrlDecode(Request["unitname"]);
            int pageNO = Convert.ToInt32(Request["pageNO"]);
            GetHP01Const getHp01 = new GetHP01Const();
            ViewReportForm viewReport = new ViewReportForm();
            string prvTemp = viewReport.ViewNMReportFormInfo(pageNO, unitCode);
            string temp = getHp01.GetNMReservoirByUnitCode(unitCode, unitName);
            string result = "{Reservoir:" + temp + "," + prvTemp + "}";
            return result;
        }

        /// <summary>省级查看下级某个市已经保存的数据
        /// </summary>
        /// <returns></returns>
        /*public string ViewNMCityReservoir()
        {
            JsonResult jsr = new JsonResult();
            string unitCode = Request["unitcode"];
            int pageNO = Convert.ToInt32(Request["pageNO"]);
            ViewReportForm viewReport = new ViewReportForm();
            string temp = viewReport.ViewNMReportFormInfo(pageNO,unitCode);
            return temp;
        }*/

        public string NP_Operate(string type)
        {
            SaveOrUpdateReport np = new SaveOrUpdateReport();
            DateTime endTime = Convert.ToDateTime(Request["EndDateTime"].ToString());
            switch (type)
            {
                case "GetNewPageNO":
                    return np.NMNPCreateReport(endTime);
                    break;
                case "Update":
                    break;
            }
            return "0";
        }

        /// <summary>浙江撤销已经报送但没有参加汇总的报表
        /// </summary>
        /// <returns></returns>
        public string UndoSend()
        {
            string result = "";
            int limit = int.Parse(Request["limit"]);
            int pageNO = Convert.ToInt32(Request["pageNO"]);
            DeleteOrSendReport rpt = new DeleteOrSendReport(limit);
            result = rpt.UndoReports(limit, pageNO);//返回值1：撤销报送成功，2：该报表已经参加汇总，撤销失败,错误消息：出现异常
            return result;
        }

        public FileResult GetFile(string filename)
        {
            return File(Server.MapPath(filename), "text/javascript");
        }

        public string GetTime(string format)
        {
            format = format == null ? "yyyy-MM-dd HH:mm:ss" : format;
            return DateTime.Now.ToString(format);
        }

        public string GetLastFewYearData(DateTime? start, DateTime? end)
        {
            int level = int.Parse(Request["limit"]);

            return new BaseData().GetLastFewYearData(start.Value.AddYears(-1), end.Value.AddYears(-1), level);
        }
    }
}
