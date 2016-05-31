using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;

namespace ZQFW.Controllers
{
    public class SystemMaintainController : Controller
    {
        //
        // GET: /SystemMaintain/Index

        public ActionResult Index()
        {
            if (Session["SESSION_USER"] != null)
            {
                bool debug = Request["debug"] == null ? false : true;
                if (debug)
                {
                    return File("~/Views/Debug/SystemMaintain.html", "text/html");
                }
                else
                {
                    return File("~/Views/Release/SystemMaintain.html", "text/html");
                }
            }
            else
            {
                return Redirect("/Login");
            }
        }

        /// <summary>数据库备份
        /// </summary>
        /// <returns></returns>
        /// Get:/SystemMaintain/BackupData
        public JsonResult BackupData()
        {
            string result = "";
            JsonResult jsr = new JsonResult();
            string conString = ConfigurationManager.ConnectionStrings["DataBackup"].ConnectionString;
            DateTime dt = DateTime.Now;
            string path = "D:\\DB_BackUp";
            string name = dt.Year.ToString() + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second;
            string[] dataBaseNames = { "FXPRV", "FXCTY", "FXCNT", "FXTWN", "FXDICT", "FXCLD" };
            string sqltxt = "";
            for (int i = 0; i < dataBaseNames.Length; i++)
            {
                string conStr = conString.Replace("FXCLD", dataBaseNames[i]);
                string fileName = dataBaseNames[i] + "_" + name;
                sqltxt = @"BACKUP DATABASE " + dataBaseNames[i] + " TO Disk='" + path + "\\" + fileName + ".bak" + "'";
                SqlConnection con = new SqlConnection(conString);
                con.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(sqltxt, con);
                    cmd.ExecuteNonQuery();
                    result = "1";
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }

            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

    }
}
