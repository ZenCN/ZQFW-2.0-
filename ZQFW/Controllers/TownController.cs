using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LogicProcessingClass.ReportOperate;
using LogicProcessingClass.AuxiliaryClass;
using EntityModel;
using LogicProcessingClass;
using DBHelper;
using System.Web.Script.Serialization;
using System.Collections;
using LogicProcessingClass.XMMZH;

namespace ZQFW.Controllers
{
    public class TownController : Controller
    {
        //
        // GET: /Town/

        public ActionResult Index()
        {
            if (Session["SESSION_USER"] == null)
            {
                return Redirect("/Login");
            }

            string startTime = DateTime.Now.Ticks.ToString();
            int limit = int.Parse(Request["limit"]);
            string unitCode = Request["unitcode"];
            string initData = "";
            if (unitCode != null)
            {
                DateTime searchStartDate = Convert.ToDateTime(Request["startDate"]);
                DateTime searchEndDate = Convert.ToDateTime(Request["endDate"]);

                TownReport vtr = new TownReport();
                //string reportList = vtr.GetTownReportList(unitCode, searchStartDate, searchEndDate); //报表的列表
                LogicProcessingClass.AuxiliaryClass.CommonFunction cf = new LogicProcessingClass.AuxiliaryClass.CommonFunction();
                string deathReson = cf.GetDeathReasonList(); //死亡原因列表
                TableFieldBaseData check = new TableFieldBaseData();
                CommonFunction comm = new CommonFunction();

                string UndersCheck = "Formula:" + comm.GetProvenceData(); //获取校核数据 格式：LocalCheck:[{}]
                string LocalCheck = "Constant: " + check.QueryCheckBaseData(limit, unitCode); //基础数据

                //string checkData = check.QueryTownBaseData(unitCode); ; //基础数据
                string fieldDefineAndMeasureName = vtr.GetFieldDefineAndMeasureName(5, unitCode); //流域代码、字段说明、字段单位
                initData = "{RelationCheck:{" + UndersCheck + "," + LocalCheck + "}," + deathReson + "," + fieldDefineAndMeasureName + "" + "},RiverCode:'" +
                         new CommonFunction().GetRiverCodeByUnitCode(unitCode) + "'}";
            }
            ViewData["InitData"] = initData;

            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/Town.cshtml");
            }
            else
            {
                return View("~/Views/Release/Town.cshtml");
            }
        }

        public FileResult GetTemplateResult(string type)
        {
            switch (type)
            {
                case "BaseData":
                    return File("~/Scripts/Templates/BaseData/BaseData.htm", "text/html");
                case "InferiorManager":
                    return File("~/Scripts/Templates/BaseData/InferiorManager.htm", "text/html");
                case "Reservoir":
                    return File("~/Scripts/Templates/BaseData/Reservoir.htm", "text/html");
                default:
                    return null;
            }
        }

        /// <summary>
        /// 根据时间单位，查询该时间区间的所有报表
        /// </summary>
        /// <returns></returns>
        /// GET: /Town/SerachReportList
        public JsonResult SerachReportList()
        {
            JsonResult jsr = new JsonResult();

            string result = "";
            if (Request["unitcode"] != null)
            {
                string unitCode = Request["unitcode"];
                DateTime searchStartDate = Convert.ToDateTime(Request["startDate"]);
                DateTime searchEndDate = Convert.ToDateTime(Request["endDate"]);
                TownReport vtr = new TownReport();
                string reportList = vtr.GetTownReportList(unitCode, searchStartDate, searchEndDate);//报表的列表
                result = "{" + reportList + "}";
            }
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 删除报表，放入回收站中
        /// </summary>
        /// <returns></returns>
        /// GET: /Town/DeleteReport
        public JsonResult DeleteReport()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            TownReport town = new TownReport();
            int pageNO = Convert.ToInt32(Request["pageno"]);
            result = town.DeleteReport(pageNO);
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 打开单张报表
        /// </summary>
        /// <returns></returns>
        /// GET: /Town/OpenReport
        public JsonResult OpenReport()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            //TownReport town = new TownReport();
            int pageNO = Convert.ToInt32(Request["pageno"]);
            string rptType = "HL01";
            ViewReportForm viewRpt = new ViewReportForm();
            result = "{" + viewRpt.ViewReportFormInfo(5, pageNO, rptType) + "}";
            //result = town.GetRptInfo(pageNO);
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 发送报表
        /// </summary>
        /// <returns></returns>
        /// GET: /Town/SendReport
        public JsonResult SendReport()
        {
            JsonResult jsr = new JsonResult();
            DeleteOrSendReport send = new DeleteOrSendReport(5);
            string result = "";
            int pageNO = Convert.ToInt32(Request["pageno"]);
            string unitCode = Request["unitcode"];
            int sendType = 0;
            string unitName = HttpUtility.UrlDecode(Request["unitname"]);
            result = send.SendReports(5, pageNO, sendType, unitCode, unitName);
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 保存或修改报表
        /// </summary>
        /// <returns>成功：返回新建或修改的报表的页号，否则返回“错误消息：”</returns>
        /// GET：/Town/SaveOrUpdateRpt
        public JsonResult SaveOrUpdateRpt()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            JsonResult jsr = new JsonResult();
            TownReport town = new TownReport();

            string dataStr = Request["report"] == null ? "" : Request["report"];
            Hashtable report = serializer.Deserialize<Hashtable>(dataStr);
            string reportTitle = serializer.Serialize(report["ReportTitle"]);
            ReportTitle title = serializer.Deserialize<ReportTitle>(reportTitle);
            title.UnitCode = Request.Cookies["unitcode"].Value.ToString(); //单位代码
            title.UnitName = HttpUtility.UrlDecode(Request.Cookies["unitname"].Value.ToString(), System.Text.Encoding.GetEncoding("utf-8")); ; //单位名称

            string result = "";
            LogicProcessingClass.Tools tool = new LogicProcessingClass.Tools();
            string riverCode = new CommonFunction().GetRiverCodeByUnitCode(title.UnitCode);
            ReportTitle rpt = null;

            BusinessEntities busEntitiy = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(5);
            decimal state = 0;
            if (title.State == null || title.State == 0)
            {
                state = 0; //报表状态  0新建，1待审核，2已审核，3已发送，4已报批，5已签批，6已发送（发布）
            }
            else
            {
                state = Convert.ToDecimal(title.State);
            }
            string pageNO = null;
            if (title.PageNO != 0)
            {
                pageNO = title.PageNO.ToString();
                int id = Convert.ToInt32(pageNO);
                if (state.ToString() == "3")//已经报送的，那么新建一个新表，且把主表CopyPageNO值修改为新表的PageNO
                {
                    rpt = new ReportTitle();
                    rpt.PageNO = new ReportHelpClass().FindMaxPageNO(5) + 1;
                    ReportTitle zhurpt = busEntitiy.ReportTitle.Where(t => t.PageNO == id).SingleOrDefault();
                    zhurpt.CopyPageNO = rpt.PageNO;
                    rpt.State = 0;
                }
                else
                {
                    rpt = busEntitiy.ReportTitle.Where(t => t.PageNO == id).SingleOrDefault();
                    if (!town.DeleteTownRpt(id))
                    {
                        result = "错误消息：";
                        jsr = Json(result);
                        return jsr;
                    }
                }
            }
            else
            {
                rpt = new ReportTitle();
                rpt.State = 0;
                rpt.PageNO = new ReportHelpClass().FindMaxPageNO(5) + 1;
            }

            //新建修改HL011-HL014，HP011-HP012的表
            string DBname = "";
            XMMZHClass xmm = new XMMZHClass();
            List<Affix> aList = new List<Affix>();
            #region 插入HL011-HL012，HP011-HP012的数据
            foreach (DictionaryEntry de in report)
            {
                DBname = de.Key.ToString();
                switch (DBname)//根据DBname不同执行不同的操作
                {
                    case "HL011"://往HL011表中插入数据
                        object[] str011 = serializer.ConvertToType<object[]>(de.Value);//反序列化HL011，将里面的数据放入一个对象数组中去
                        for (int i = 0; i < str011.Length; i++)//循环这个对象数组
                        {
                            HL011 hl011 = serializer.ConvertToType<HL011>(str011[i]);//HL011对象接收，数组里的对象
                            hl011.DataOrder = i;
                            hl011.PageNO = rpt.PageNO;//新加的
                            hl011.ReportTitle = rpt;//不确定是否需要
                            hl011 = xmm.ToSetHL<HL011>(hl011, 5);//数量级转换
                            hl011.RiverCode = riverCode;
                            hl011.UnitCode = title.UnitCode;
                            hl011.DW ="合计";
                            rpt.HL011.Add(hl011);//将HL011数据放到reporttitle对象的HL011中去

                            if (hl011.ZYRK > 0) //有转移人口，往 HL014表 插入数据
                            {
                                HL014 hl014 = new HL014();
                                hl014 = tool.SetZeroToObject(hl014);
                                hl014.UnitCode = title.UnitCode;
                                hl014.RiverCode = riverCode;
                                hl014.DW = "合计";
                                hl014.PageNO = rpt.PageNO;
                                hl014.XYZYQT = hl011.ZYRK;
                                rpt.HL014.Add(hl014);
                            }
                        }
                        break;
                    case "HL012":
                        object[] str012 = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < str012.Length; i++)
                        {
                            HL012 hl012 =new HL012();
                            hl012 = tool.SetZeroToObject(hl012);
                            hl012 =  serializer.ConvertToType<HL012>(str012[i]);
                            hl012.UnitCode = title.UnitCode;
                            hl012.DW = title.UnitName;

                            hl012.DataOrder = i;
                            hl012.PageNO = rpt.PageNO;//新加的
                            hl012.ReportTitle = rpt;
                            hl012.RiverCode = riverCode;
                            rpt.HL012.Add(hl012);
                        }
                        break;
                    case "DeletedAffix":
                        object[] delAffs = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < delAffs.Length; i++)
                        {
                            Affix aff = serializer.ConvertToType<Affix>(delAffs[i]);
                            aff.PageNO = rpt.PageNO;
                            aList.Add(aff);
                        }
                        break;
                }
            }
            #endregion

            rpt.ORD_Code = "HL01";
            rpt.RPTType_Code = "XZ0";
            rpt.UnitCode = title.UnitCode; //单位代码
            rpt.UnitName = title.UnitName;
            rpt.StartDateTime = title.StartDateTime; //起始时间 
            rpt.EndDateTime = title.EndDateTime; //结束时间  
            rpt.Remark = title.Remark; //备注
            rpt.ReceiveState = -1; //接收状态  0下载表箱，1拒收表箱，2已装入，3已删除
            rpt.CopyPageNO = 0; //源表页号    新建表没有源表，此项值为0
            rpt.Del = 0;
            string date = DateTime.Now.ToShortDateString();
            rpt.WriterTime = Convert.ToDateTime(date); //填报日期  只有年月日
            rpt.LastUpdateTime = Convert.ToDateTime(date); //最后修改日期  年月日时分秒
            rpt.SendTime = DateTime.Now; 
            rpt.ReceiveTime = Convert.ToDateTime(date); //接收日期 年月日时分秒
            rpt.StatisticalCycType = 0;
            rpt.SourceType = 0;
            rpt.AssociatedPageNO = 0;
            rpt.CSPageNO = 0;//为了保证数据的完整性,不为null

            try
            {
                if (title.PageNO != 0)
                {
                    if (state != 3)
                    {
                        busEntitiy.SaveChanges();
                    }
                    else
                    {
                        busEntitiy.ReportTitle.AddObject(rpt);
                        busEntitiy.SaveChanges();
                    }
                    if (aList.Count>0)
                    {
                        town.DeleteAffixs(state, aList, rpt.PageNO, Convert.ToInt32(pageNO));
                    }
                }
                else
                {
                    busEntitiy.ReportTitle.AddObject(rpt);
                    busEntitiy.SaveChanges();
                }
                result = rpt.PageNO.ToString();
            }
            catch (Exception ex)
            {
                result = "错误消息：" + ex.Message+ex.InnerException;
            }
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 获取某报表死亡人员信息
        /// </summary>
        /// <returns>格式：{deathInfo:[....]}</returns>
        /// GET：/Town/GetDeathInfoForm
        public JsonResult GetDeathInfoForm()
        {
            JsonResult jsr = new JsonResult();
            TownReport town = new TownReport();
            string result = "";
            int pageNO = Convert.ToInt32(Request["pageno"]);
            result = town.GetDeathReasonForm(pageNO);
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Upload(HttpPostedFileBase fileData)
        {
            if (fileData != null)
            {
                try
                {
                    // 文件上传后的保存路径
                    string filePath = Server.MapPath("~/Html/" + fileData.FileName);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                    string fileExtension = Path.GetExtension(fileName); // 文件扩展名
                    string saveName = Guid.NewGuid().ToString() + fileExtension; // 保存文件名称

                    fileData.SaveAs(filePath + saveName);

                    return Json(new { Success = true, FileName = fileName, SaveName = saveName });
                }
                catch (Exception ex)
                {
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
