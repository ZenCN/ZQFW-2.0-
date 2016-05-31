using System;
using System.Web.Mvc;
using LogicProcessingClass.ReportOperate;
using LogicProcessingClass.AuxiliaryClass;

namespace ZQFW.Controllers.Index
{
    public class UrgeReportController : Controller
    {
        UrgeAndReadReport uarr = new UrgeAndReadReport();
        CommonFunction cf = new CommonFunction();
        //
        // GET: /UrgeReport/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获得下级单位列表
        /// </summary>
        /// <returns></returns>
        /// GET: /UrgeReport/GetLowerUnits
        public ActionResult GetLowerUnits()
        {
            JsonResult jsr = new JsonResult();
            string temp = "";
            string unitCode = Request["unitCode"];//登陆单位代码
            temp = cf.GetLowerUnitList(unitCode);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 增加催报
        /// </summary>
        /// <returns>成功：返回“1”；失败：返回错误信息</returns>
        /// POST: /UrgeReport/AddUrgeReport
        public ActionResult AddUrgeReport()
        {
            JsonResult jsr = new JsonResult();
            string temp = "1";
            int limit = Convert.ToInt32(Request["limit"]);//登陆单位级别
            string receiveUnitCode = Request["receiveUnitCode"];//接收单位代码
            string content = Request["content"];//催报内容
            string urgeReportPerson = Request["urgeReportPerson"];//催报人
            string urgeReportUnit = Request["urgeReportUnit"];//催报单位
            int msgType = Request["msgType"] == null ? 0 : int.Parse(Request["msgType"]);
            string detials = Request["detials"];
            string pagenos = Request["pagenos"];
            try
            {
                uarr.UrgeReport(limit, receiveUnitCode, content, urgeReportPerson, urgeReportUnit, msgType, detials, pagenos);
            }
            catch (Exception ex)
            {
                temp = ex.Message;
            }
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 获得催报列表
        /// </summary>
        /// <returns></returns>
        /// GET: /UrgeReport/GetUrgeReportList
        public ActionResult GetUrgeReportList()
        {
            JsonResult jsr = new JsonResult();
            string temp = "";
            int limit = Convert.ToInt32(Request["limit"]);//登陆单位级别
            string unitCode = Request["unitcode"];//登陆单位代码
            temp = uarr.GetUrgeReportList(limit, unitCode);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 阅读催报
        /// </summary>
        /// <returns></returns>
        /// GET: /UrgeReport/GetUrgeReportList
        public ActionResult ReadUrgeReport()
        {
            JsonResult jsr = new JsonResult();
            string temp = "";
            int tbNO = Convert.ToInt32(Request["TBNO"]);//催报编号
            int limit = Convert.ToInt32(Request["limit"]);//登陆单位级别
            temp = uarr.ReadUrgeReport(limit, tbNO);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }
    }
}
