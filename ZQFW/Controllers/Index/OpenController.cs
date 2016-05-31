using System;
using System.Web.Mvc;
using LogicProcessingClass.ReportOperate;
using LogicProcessingClass;

namespace ZQFW.Controllers.Index
{
    public class OpenController : Controller
    {

        public void GetReportTemmplate(string unitcode, string rptType)
        {
            Response.ContentType = "text/plain";
            switch (unitcode.Trim())
            {
                default:
                    Response.WriteFile("~/JS/Template/" + rptType + "/Common.htm");
                    break;
            }
        }

        /// <summary>
        /// 查看报表（优先使用单位级别，否则需要传入单位代码。两者不能同时为空）
        /// 传入UnitCode适用与打开多级报表
        /// </summary>
        /// <returns></returns>
        /// GET: Open/OpenReport
        public JsonResult OpenReport()
        {
            JsonResult jsr = new JsonResult();
            #region 旧方法
            /*
            //ReportTitle rpt = new ReportTitle();
            string result = "";
            string arr = "";
            string limit = "";
            int sourceType = Convert.ToInt32(Request["SourceType"]);//来源类型
            if (Request["level"] == null || Request["level"] == "")
            {
                string unitCode = Request["unitcode"];
                if (unitCode != null || unitCode != "")
                {
                    limit = new Tools().GetLevelByUnitCode(unitCode,"").ToString();
                }
            }
            else
            {
                limit = Request["level"];
            }
            if (limit == "")
            {
                result = "错误消息：单位级别或单位代码不能同时为空！";
            }
            else
            {
                int pageNO = Convert.ToInt32(Request["pageno"]);
                ViewReportForm viewRpt = new ViewReportForm();
                if (sourceType == 0)
                {
                    result = viewRpt.ViewReportFormInfo(Convert.ToInt32(limit), pageNO);
                }
                else if (sourceType == 1 || sourceType == 2)
                {
                    result = viewRpt.ViewReportFormInfo(Convert.ToInt32(limit), pageNO);
                    //LogicProcessingClass.OY.ViewSourceReport vsr = new LogicProcessingClass.OY.ViewSourceReport(limit);
                    //arr = vsr.GetSourceReportList(pageNO, limit, (sourceType == 2 ? 1 : 0), UnitName);// "1"表示本级库
                    result = result + "," + arr;
                }

                
            }
            jsr = Json("{"+result+"}");
             * */
            #endregion
            string result = "";
            string arr = "";
            string rptType = Request["rptType"];
            int sourceType = Convert.ToInt32(Request["sourceType"]);//来源类型
            int limit = Convert.ToInt32(Request["limit"]);//单位级别
            string UnitName = new Tools().GetUnitNameByUnitCode(Request.Cookies["unitcode"].Value);

            int pageNO = Convert.ToInt32(Request["pageno"]);
            ViewReportForm viewRpt = new ViewReportForm();
            if (sourceType == 0)
            {
                result = viewRpt.ViewReportFormInfo(limit, pageNO, rptType);
            }
            else if (sourceType == 1 || sourceType == 2)
            {
                result = viewRpt.ViewReportFormInfo(limit, pageNO, rptType);
                arr = viewRpt.GetSourceReportList(pageNO, limit, (sourceType == 2 ? 1 : 0), UnitName, Request.Cookies["unitcode"].Value);// "1"表示本级库
                result = result + "," + arr;
            }

            jsr = Json("{" + result + "}");
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }
    }
}
