using System;
using System.Web.Mvc;
using LogicProcessingClass.ReportOperate;
using LogicProcessingClass.AuxiliaryClass;

namespace ZQFW.Controllers
{
    public class InboxAndRecycleController : Controller
    {
        InBox inbox = new InBox();
        CommonFunction cf = new CommonFunction();
        //
        // GET: /InboxAndRecycle/
         
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 接收和回收站模块页面公共初始化
        /// </summary>
        /// <param name="startTime">时间段的起始时间</param>
        /// <param name="endtime">时间段的结束时间</param>
        /// <param name="initType">初始化的类型。0：初始化接收页面，3：初始化回收站页面</param>
        /// <returns></returns>
        public ActionResult BaseInit(DateTime startTime, DateTime endtime, int initType)
        {
            JsonResult jsr = new JsonResult();
            string temp = ""; 
            int limit = Convert.ToInt32(Request["limit"]);//登录单位的级别
            string unitCode = Request["unitCode"];//登录单位代码
            string type = Request["type"];
            int searchUnitLimit = Convert.ToInt32(Request["searchUnitLimit"]);//查询的单位级别，0：本级单位，1：全部下级单位，2：某个下级单位
            temp = "{" + cf.GetLowerUnitList(unitCode) + "," + cf.GetRptClass();
            string report = inbox.SearchReport(limit, unitCode, searchUnitLimit, startTime, endtime, "0", -1, initType,type);
            if (report != "")
            {
                temp = temp + "," + report;
            }
            temp = temp + "}";
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 接收页面的初始化
        /// </summary>
        /// <returns></returns>
        /// GET: /InboxAndRecycle/InboxInit
        public ActionResult InboxInit()
        {
            int initType = 0;//接收状态是0，表示接收箱里
            DateTime startTime = DateTime.Now.Subtract(TimeSpan.FromDays(7));//发送日期时间段的起始值
            DateTime endTime = DateTime.Now;//发送日期时间段的结束值
            return BaseInit(startTime, endTime, initType);
        }
        
        /// <summary>
        /// 回收站页面的初始化
        /// </summary>
        /// <returns></returns>
        /// GET: /InboxAndRecycle/RecycleInit
        public ActionResult RecycleInit()
        {
            int initType = 3;//接收状态是3，表示回收站里
            DateTime startTime = DateTime.Today.Subtract(TimeSpan.FromDays(7));//报表结束日期时间段的起始值
            DateTime endTime = DateTime.Today;//报表结束日期时间段的结束值
            return BaseInit(startTime, endTime, initType);
        }

        /// <summary>
        /// 浏览报表明细
        /// </summary>
        /// <returns></returns>
        /// GET: /InboxAndRecycle/SearchReportByPageNo
        public JsonResult ViewReport()
        {
            JsonResult jsr = new JsonResult();
            string temp = "";      
            int limit = Convert.ToInt32(Request["limit"]);//登录单位的级别
            int pageNO = Convert.ToInt32(Request["pageno"]);//报表页号
            int searchUnitLimit = Convert.ToInt32(Request["unitType"]);//查询的单位级别，0：本级单位，1：全部下级单位，2：某个下级单位
            temp = inbox.SearchReportByPageNo(limit, pageNO, searchUnitLimit, Request["unitCode"], Request["rptType"]);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 删除回收站里的报表
        /// </summary>
        /// <returns>成功：返回删除成功；失败：返回错误信息</returns>
        /// GET: /InboxAndRecycle/DeleteReportAtRecycle
        public JsonResult DeleteReportAtRecycle()
        {
            JsonResult jsr = new JsonResult();
            string temp = "";
            int limit = Convert.ToInt32(Request["limit"]);//登录单位的级别
            string pageNOs = Request["pageNO"];//报表页号
            int searchUnitLimit = Convert.ToInt32(Request["searchUnitLimit"]);//查询的单位级别，0：本级单位，1：全部下级单位，2：某个下级单位
            try
            {
                inbox.DeleteReport(limit, pageNOs, searchUnitLimit);
                temp = "1";
            }
            catch(Exception ex)
            {
                temp = "错误消息" + ex.Message;
            }
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }
    }
}
