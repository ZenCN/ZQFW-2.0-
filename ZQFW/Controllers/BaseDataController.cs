using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DBHelper;
using EntityModel;
using JetBrains;
using LogicProcessingClass.ReportOperate;
using LogicProcessingClass.Model;

namespace ZQFW.Controllers
{
    public class BaseDataController : Controller
    {
        BaseData bd = new BaseData();

        //
        // GET: /BaseData/
        public ActionResult Index()
        {
            if (Session["SESSION_USER"] != null)
            {
                bool debug = Request["debug"] == null ? false : true;
                if (debug)
                {
                    return File("~/Views/Debug/BaseData.html", "text/html");
                }
                else
                {
                    return File("~/Views/Release/BaseData.html", "text/html");
                }
            }
            else
            {
                return Redirect("/Login");
            }
        }

        /// <summary>
        /// 获取单位的基础数据
        /// </summary>
        /// <returns></returns>
        /// GET: /BaseData/GetBaseData
        public string GetBaseData()
        {
            string temp = "";
            string unitCode = Request["unitCode"];//单位代码
            temp = bd.GetBaseDataByUnitCode(unitCode);
            return temp;
        }

        /// <summary>
        /// 保存单位的基础数据
        /// </summary>
        /// <returns>成功返回“保存成功！”；失败返回错误信息</returns>
        /// GET: /BaseData/SaveBaseData
        [HttpPost]
        public string SaveBaseData()
        {
            string temp = "";
            string unitCode = Request["unitCode"];//单位代码
            //string fieldDefineNo = Request["fieldDefineNo"];//字段编号，多个以逗号隔开
            //string baseData = Request["baseData"];//基础数据，多个以逗号隔开，与字段编号一一对应
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string jsonStr = Request["jsonStr"];
            List<TB04_CheckBase> tb04List = serializer.Deserialize<List<TB04_CheckBase>>(jsonStr);//反序列化
            //baseData.PostBaseData(unitCode, tb04List);
            try
            {
                bd.SaveBaseData(unitCode, tb04List);
                temp = "1";
            }
            catch (Exception ex)
            {
                temp = "错误信息：" + ex.Message;
            }
            return temp;
        }

        /// <summary>
        /// 重置单位的登陆密码
        /// </summary>
        /// <returns>成功返回“重置成功！”；失败返回错误信息</returns>
        /// GET: /BaseData/ResetPassword
        public string ResetPassword()
        {
            string temp = "";
            string unitCode = Request["unitCode"]; //单位代码
            int limit = int.Parse(Request["limit"]);
            try
            {
                if (unitCode.IndexOf(",") > 0)
                {
                    string[] unitCodes = unitCode.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    bd.ResetPassword(unitCodes, limit);
                }
                else  //single
                {
                    bd.ResetPassword(unitCode, limit);
                }
                temp = "重置成功！";
            }
            catch (Exception ex)
            {
                temp = "错误信息：" + ex.Message;
            }
            return temp;
        }

        /// <summary>
        /// 增加单位
        /// </summary>
        /// <returns>成功返回“success”；失败返回失败原因</returns>
        /// GET: /BaseData/UnitOperater
        public JsonResult UnitOperater()
        {
            string temp = "";
            JsonResult jsr = new JsonResult();
            LogicProcessingClass.Tools tool = new LogicProcessingClass.Tools();
            string unitCode = Request["unitCode"];//单位代码
            string pUnitCode = Request["pUnitCode"];//上级单位代码
            string unitName = Request["unitName"];//单位名称
            int uorder = Convert.ToInt32(Request["uorder"]);//单位顺序
            string riverDictCode = Request["riverDictCode"];//流域代码
            string type = Request["type"];//操作编号1增加 2删除 修改(与增加一起为1) 4重置
            if (type == "1")
            {
                temp = bd.AddUnit(unitCode, pUnitCode, unitName, uorder, riverDictCode);
            }
            else if (type == "2")
            {
                temp = bd.DeleteUnit(unitCode);
            }
            else if (type == "4")
            {
                try
                {
                    bd.ResetPassword(unitCode, Tools.GetLimitByCode(unitCode));
                    temp = "重置成功！";
                }
                catch (Exception ex)
                {
                    temp = "错误信息：" + ex.Message;
                }
            }
            if (temp.IndexOf("成功") >= 0)
            {
                //tool.ClearApplication();
                tool.ClearApplicationByKey("Units-" + unitCode.Substring(0, 2));//只清除登录单位的缓存
                HttpApplicationState App = System.Web.HttpContext.Current.Application;
                App["InitUnitsFlag"] = "false";
            }

            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>检查新增单位的信息是否符合条件
        /// </summary>
        /// <returns></returns>
        /// GET: /BaseData/CheckUnitInfo
        public JsonResult CheckUnitInfo()
        {
            string jsonStr = "";
            JsonResult jsr = new JsonResult();
            string code = Request["code"];
            string pcode = Request["pcode"];
            decimal uorder = Convert.ToDecimal(Request["uorder"]);
            try
            {
                int unitCount = bd.CheckUnitCodeExist(code, -1);//传入-1，不添加级别搜索条件 
                int punitCount = bd.CheckUnitCodeExist(pcode, 1);//传入1，级别搜索条件不能为5
                int uorderCount = bd.CheckUorderExist(pcode, uorder);
                jsonStr += "{'code':" + unitCount + ",'pcode':" + punitCount + ",'uorder':" + uorderCount + "}";
            }
            catch (Exception ex)
            {
                jsonStr = "错误消息：单位信息检查出错！" + ex.Message;
            }
            jsr = Json(jsonStr);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>清除所有application之后，在基础数据页面由ajax初始化实体和下级单位
        /// </summary>
        /// <returns></returns>
        /// GET: /BaseData/Global
        public JsonResult Global()
        {
            string temp = "1";
            JsonResult jsr = new JsonResult();
            Persistence persistence = new Persistence();
            persistence.PersistenceUnits(Request["unitCode"].Substring(0, 2) + "000000");  //网站启动第一次把所有单位信息加载到application中
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 修改单位
        /// </summary>
        /// <returns>成功返回“success”；失败返回失败原因</returns>
        /// GET: /BaseData/UpdateUnit
        /*public JsonResult UpdateUnit()
        {
            string temp = "";
            JsonResult jsr = new JsonResult();
            string unitCode = Request["unitCode"];//单位代码
            string pUnitCode = Request["pUnitCode"];//上级单位代码
            string unitName = Request["unitName"];//单位名称
            int uorder = Convert.ToInt32(Request["uorder"]);//单位顺序
            string riverDictCode = Request["riverDictCode"];//流域代码
            temp = bd.UpdateUnit(unitCode, pUnitCode, unitName, uorder, riverDictCode);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }*/
        public string Update_Add_Units()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string result = "";
            List<Dictionary<string, string>> update_units = jss.Deserialize<List<Dictionary<string, string>>>(Request["update_units"]);
            List<Dictionary<string, string>> add_units = jss.Deserialize<List<Dictionary<string, string>>>(Request["add_units"]);
            try
            {
                foreach (var unit in update_units)
                {
                    result = bd.UpdateUnit(unit["Code"], unit["ParentCode"], unit["Name"], int.Parse(unit["Order"]),
                        unit["RiverCode"]);
                }
            }
            catch (Exception ex)
            {
                result = "更新单位信息出错，详情：" + ex.Message;
            }

            if (update_units.Count > 0 && result == "success" || update_units.Count == 0)
            {
                try
                {
                    foreach (var unit in add_units)
                    {
                        result = bd.AddUnit(unit["Code"], unit["ParentCode"], unit["Name"], int.Parse(unit["Order"]),
                            unit["RiverCode"]);
                    }

                    if (Request["unitcode"] != null)
                    {
                        Persistence persistence = new Persistence();
                        persistence.PersistenceUnits(Request["unitcode"].Substring(0, 2) + "000000 ");
                    }
                }
                catch (Exception ex)
                {
                    if (update_units.Count > 0)
                    {
                        result = "更新单位信息成功，增加单位信息失败，详情：" + ex.Message;
                    }
                    else
                    {
                        result = "增加单位信息失败，详情：" + ex.Message;
                    }

                }
            }

            return result;
        }

        /// <summary>
        /// 删除单位
        /// </summary>
        /// <returns>成功返回“success”；失败返回失败原因</returns>
        /// GET: /BaseData/DeleteUnit
        public string DeleteUnit()
        {
            string temp = "";
            string unitCode = Request["unitCode"];//单位代码
            temp = bd.DeleteUnit(unitCode);
            return temp;
        }

        /// <summary>
        /// 获取本身单位或者下级单位
        /// </summary>
        /// <returns></returns>
        /// GET: /BaseData/GetUnits
        public string GetUnits()
        {
            string temp = "";
            string unitCode = Request["unitCode"];//单位代码
            temp = bd.GetUnits(unitCode);
            return temp;
        }


        /// <summary>获取树形结构需要的数据
        /// </summary>
        /// <returns></returns>
        /// GET: /BaseData/GetTreeUnits
        public JsonResult GetTreeUnits()
        {
            string temp = "";
            JsonResult jsr = new JsonResult();
            string unitCode = Request["unitCode"];//单位代码
            //temp = bd.GetAllUnit(unitCode); 
            temp = bd.GetAllUnitObj(unitCode);
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 基础数据登陆
        /// </summary>
        /// <returns>成功返回“success”；失败返回“fail”</returns>
        /// post: /BaseData/BaseDataLogin
        [HttpPost]
        public JsonResult BaseDataLogin()
        {
            string temp = "success";
            JsonResult jsr = new JsonResult();
            string password = Request["pwd"];
            if (password != "zizo")
            {
                temp = "fail";
            }
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 导出灾情综述
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportDisasterReview(string report)
        {
            if (report == null)
            {
                return new JsonResult();
            }

            JsonResult jsr = new JsonResult();
            string result = "";
            int limit = int.Parse(Request.Cookies["limit"].Value);
            string unitcode = Request.Cookies["unitcode"].Value;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            
            ExportWord exportWord = new ExportWord();
            try
            {
                string fileName = "";
                switch (unitcode.Substring(0,2))
                {
                    case "15":
                        ZqzsBean_15 zqzsBean_i5 = serializer.Deserialize<ZqzsBean_15>(report);
                        fileName = exportWord.ExportZqzsToWord_15(zqzsBean_i5, limit);
                        break;
                    case "45":
                        fileName = exportWord.ExportZqzsToWord_45(report, limit);
                        break;
                    default:
                        ZqzsBean zqzsBean = serializer.Deserialize<ZqzsBean>(report);
                        fileName = exportWord.ExportZqzsToWord(zqzsBean, limit);
                        break;
                }

                //try
                //{
                if (System.IO.File.Exists(fileName))  //判断文件是否存在
                {
                    Response.Charset = "ISO-8859-1";  //提供下载的文件，不编码的话文件名会乱码 
                    Response.ContentEncoding = System.Text.Encoding.UTF8;

                    // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(fileName));
                    Response.ContentType = "Application/octet-stream";

                    // 把文件流发送到客户端 
                    Response.WriteFile(fileName);
                    Response.Flush();
                    Response.Close();

                    System.IO.File.Delete(fileName);
                    result = "导出成功！";
                }
                else
                {
                    throw new Exception("导出失败！");
                }

                //}
                //catch (Exception)
                //{
                //    result = "<script>window.alert('错误消息：导出失败');window.history.go(-1);</script>";
                //}
            }
            catch (Exception e)
            {
                result = "错误信息：" + e.Message;
            }
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 导出蓄水情况
        /// </summary>
        /// <returns></returns>
        public JsonResult ExportCommonReport()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            int limit = int.Parse(Request.Cookies["limit"].Value);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            XsqkBean xsqkBean = serializer.Deserialize<XsqkBean>(Request["Report"]);
            ExportWord exportWord = new ExportWord();
            try
            {
                string fileName = exportWord.ExportXsqkToWord(xsqkBean, limit);
                if (System.IO.File.Exists(fileName))  //判断文件是否存在
                {
                    Response.Clear();  //清除缓冲
                    Response.Charset = "ISO-8859-1";  //提供下载的文件，不编码的话文件名会乱码 
                    Response.ContentEncoding = System.Text.Encoding.UTF8;

                    // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(fileName));
                    Response.ContentType = "Application/octet-stream";

                    // 把文件流发送到客户端 
                    Response.WriteFile(fileName);
                    Response.Flush();
                    Response.Close();
                    System.IO.File.Delete(fileName);
                    result = "导出成功！";
                }
                else
                {
                    throw new Exception("导出失败！");
                }
            }
            catch (Exception e)
            {
                result = "错误信息：" + e.Message;
            }
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        public string GetHpDateResult()
        {
            GetHP01Const hp01Const = new GetHP01Const();
            string result = "[" + hp01Const.GetHPDate() + "]";

            return result;
        }

        public string GetHpDataResult(string time)
        {
            string result = "[]";

            if (!string.IsNullOrEmpty(time))
            {
                time = DateTime.Parse(time).ToString("yyyy-M-d");

                FXDICTEntities fxdict = new FXDICTEntities();
                var hpHisData = from d in fxdict.TB63_HunanHisHPData
                    join u in fxdict.TB07_District
                        on d.UnitCode equals u.DistrictCode
                    where DateTime.Equals(d.RptTime, time)
                    orderby d.UnitCode
                    select new
                    {
                        UnitName = u.DistrictName,
                        TBNO = d.TBNO,
                        ZJLNXSL = d.ZJLNXSL //Convert.ToDouble().ToString("0.00")
                    };

                if (hpHisData.Any())
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    result = jss.Serialize(hpHisData);
                }
            }

            return result;
        }

        public string UpdateHpData(string json)
        {
            string result = "0";

            try
            {
                FXDICTEntities fxdict = new FXDICTEntities();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<TB63_HunanHisHPData> list = jss.Deserialize<List<TB63_HunanHisHPData>>(json);
                for (int i = 0; i < list.Count; i++)
                {
                    fxdict.TB63_HunanHisHPData.Attach(list[i]);
                    var stateEntry = fxdict.ObjectStateManager.GetObjectStateEntry(list[i]);
                    stateEntry.SetModifiedProperty("ZJLNXSL");
                }
                fxdict.SaveChanges();
                result = "1";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
