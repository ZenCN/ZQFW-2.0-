using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Collections;
using LogicProcessingClass.ReportOperate;
using EntityModel;
using LogicProcessingClass.AuxiliaryClass;
using LogicProcessingClass.ReportOperate;
using System.IO;
using LogicProcessingClass;

namespace ZQFW.Controllers
{
    public class ReportOperateController : Controller
    {
        //
        // GET: /ReportOperate/

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 删除报表
        /// </summary>
        /// <returns>成功1，否则返回"错误消息：..."</returns>
        // GET: /ReportOperate/DeleteReport
        /*public JsonResult DeleteReport()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            DeleteOrSendReport del = null;
            if (Request["type"] == "0")    //删除报表
            {
                int limit = Convert.ToInt32(Request["level"]);
                del = new DeleteOrSendReport(limit);
                result = del.Delete(Convert.ToInt32(Request["pageno"]),limit);
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
        }*/




        /// <summary>
        /// 获取当前报表的流域信息（每个流域对应一个超链接）
        /// </summary>
        /// <returns></returns>
        /// GET: /ReportOperate/GetRiverReportData
        public JsonResult GetRiverReportData()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            int pageNo = Convert.ToInt32(Request["pageNo"]);//不需要单位级别，只有省级才有
            ViewReportForm view = new ViewReportForm();
            result = view.RiverData(pageNo, Request["unitCode"]);
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// GET: /ReportOperate/InitData
        public JsonResult InitData()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            int limit = Convert.ToInt32(Request["limit"]);//单位级别
            string unitCode = Request["unitCode"].ToString();//单位代码
            TableFieldBaseData tab = new TableFieldBaseData();
            CommonFunction comm = new CommonFunction();
            string river = "{riverDistribute:[{river:'" + (new RiverDistribute().GetRiverRPTypeInfo(unitCode).DRiverRPType.Count > 1 ? "true" : "false") + "'}]}";//获取流域标识
            string riverCodes = "{" + comm.GetRiverCodeList() + "}";//获得所有流域对照表 格式：riverCodeList:[{}]
            string deathReasons = "{" + comm.GetDeathReasonList() + "}";//获取所有死亡原因 格式：DeathReason:{DeathReasons:[{}]}
            string rationalityCheck = "{ProvenceData:" + comm.GetProvenceData() + "}";//获取校核数据 格式：ProvenceData:[{}]
            string checkBaseData = "{baseData:[" + tab.QueryCheckBaseData(limit, unitCode) + "]}";//获取字段校核基础数据（已按系数进行转换） 格式：baseData:[{}]
            string tableExplain = "{tableExplain:" + tab.GetTableExplain() + "}";//填表说明 格式：tableExplain:[{}]
            string fieldUnit = "{fieldUnit:[" + tab.GetFieldMeasureName(limit) + "]}"; ;//获取计量单位(中文)如xx：千公顷 格式：fieldUnit[{}]
            string measureValue = "{measureValue:" + tab.GetMeasureValue(limit) + "}";//获取计量单位(整数) 格式：measureValue:[{}]
            string recentReportInfo = "{" + new ViewReportForm().ViewReportTitleInfo(unitCode, limit) + "}";//获取最近一次的填表信息 格式：RecentReportInfo:[{}]

            result = "[" + river + "," + riverCodes + "," + deathReasons + "," + rationalityCheck + "," + checkBaseData + "," + tableExplain + "," + fieldUnit + "," + measureValue + "," + recentReportInfo + "]";


            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        /// <summary>导出Excel
        /// </summary>
        /// <returns></returns>
        /// GET: /ReportOperate/ExportExcel
        public JsonResult ExportExcel()
        {
            JsonResult jsr = new JsonResult();
            int limit = Convert.ToInt32(Request["limit"]);
            int pageNO = Convert.ToInt32(Request["pageno"]);
            string unitCode = Request["unitcode"];
            string rptType = Request["rpttype"];//导出报表的类型（洪涝：HP01，蓄水：HP01，国统：GT）
            string ord_name = Request["ord_name"] == null ? "行政" : Request["ord_name"];
            string startDateTime = Request["sTime"];
            string endDateTime = Request["eTime"];
            string result = "";
            int serchLimt = limit;
            if (rptType=="NP01")
            {
                serchLimt = 2;
            }
            ExcelOperate excel = new ExcelOperate(serchLimt);
            result = excel.ExportExcel(limit, pageNO, unitCode, rptType, ord_name, startDateTime, endDateTime);
            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }
        /// <summary>导入Excel
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult ImportExcel(HttpPostedFileBase fileData)
        public JsonResult ImportExcel()
        {
            string unitCode = Request["unitcode"];
            string rptType = Request["rptType"];//洪涝：HL01，蓄水：HP01
            int limit = Convert.ToInt32(Request["limit"]);
            string result = "";
            string fileName = ""; //原始文件名
            string filePath = Server.MapPath("../ExcelFile");
            string name = "";
            if (Request.Files[0] != null)
            {
                //fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                fileName = Path.GetFileName(Request.Files[0].FileName);// 原始文件名称
                try
                {
                    string[] hzms = fileName.Split('.');
                    string hzm = hzms[hzms.Length - 1];
                    if (hzm.ToUpper() == "XLS") //2003版本的Excel
                    {
                        int fileSize = Request.Files[0].ContentLength;
                        if (fileSize > 0)
                        {
                            if (fileSize / 1024 <= 2048)
                            {
                                name = unitCode + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "." + hzm;
                                Request.Files[0].SaveAs(filePath + "\\" + name);
                                //fileData.SaveAs(filePath + "\\" + name);
                                ExcelOperate importFunc = new ExcelOperate(limit);
                                ArrayList al = importFunc.ImportExcelData(filePath + "\\" + name, rptType, unitCode,limit);
                                importFunc.deleteExcel("../ExcelFile/" + name);
                                return Json(new { data = al }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                result = "导入失败，" + fileName + "文件太大，请确保文件大小在2M以内！";
                            }
                        }
                    }
                    else
                    {
                        result = "您导入的 " + fileName + " 文件有误，请导入2003版本的Excel文档！";
                    }
                    return Json(new { Message = result }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(filePath + name))//发生异常时，判断文件是否已经上传到服务器，如果已经上传则删除
                    {
                        System.IO.File.Delete(filePath + name);
                    }
                    return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "请选择要导入的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }

        #region 有单独的控制器AffixOperater进行上传下载
        ///// <summary>上传附件（前提是每个上传控件的id是不同的，否则不能用此方法）
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult UploadFiles()
        //{
        //    JsonResult jsr = new JsonResult();
        //    int pageNO = Convert.ToInt32(Request["PageNO"]);
        //    int limit = Convert.ToInt32(Request["Limit"]);

        //    string result = "-1";
        //    CommonFunction commonFun = new CommonFunction();
        //    //附件集合
        //    List<Affix> aList = new List<Affix>();
        //    bool flag = true;
        //    //文件名
        //    string fileName = "";
        //    //路径
        //    string downloadURL = "../Affix/";
        //    try
        //    {
        //        string filepath = Server.MapPath("../Affix");
        //        foreach (string upload in Request.Files)
        //        {
        //            //    string mimeType = Request.Files[upload].ContentType;
        //            //    //Stream fileStream = Request.Files[upload].InputStream;
        //            //    string fileName = Path.GetFileName(Request.Files[upload].FileName);
        //            //    name = name + fileName;
        //            //    int fileLength = Request.Files[upload].ContentLength;
        //            //    Request.Files[upload].SaveAs(filepath + "\\" + name);

        //            string name = Request["unitCode"] + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "HL01_0.mil";
        //            int fileSize = Request.Files[upload].ContentLength;
        //            if (fileSize > 0)
        //            {
        //                if (fileSize / 1024 <= 2048)
        //                {
        //                    //判断该文件在文件夹中是否已存在，若存在则在后面 +1；
        //                    name = commonFun.GetNewFileName(name);
        //                    Request.Files[upload].SaveAs(filepath + "\\" + name);
        //                    Affix af = new Affix();
        //                    //string fileName = Path.GetFileName(Request.Files[upload].FileName);
        //                    af.FileName = Request.Files[upload].FileName;
        //                    af.DownloadURL = downloadURL + name;
        //                    af.FileSize = fileSize;
        //                    aList.Add(af);
        //                    flag = true;
        //                }
        //                else
        //                {
        //                    flag = false;
        //                    result = "fileLoadError:保存失败，" + Request.Files[upload].FileName + "文件太大，请确保文件大小在2M以内！";
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                flag = false;
        //                result = "fileLoadError:保存失败，" + Request.Files[upload].FileName + "文件大小为0，请确保文件大小大于0！";
        //                break;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        flag = false;
        //        result = "fileLoadError:保存失败，上传文件 " + fileName + " 出错！";
        //    }
        //    if (flag == true && aList.Count > 0)
        //    {
        //        SaveOrUpdateReport rpt = new SaveOrUpdateReport();
        //        result = rpt.DBUploadFileAffix(pageNO, limit, aList);
        //    }

        //    jsr = Json(result);
        //    jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    return jsr;
        //}

        public JsonResult DownFile()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            try
            {
                //解密文件路径
                string filePath = Server.MapPath("../" + new Tools().EncryptOrDecrypt(1, Request["url"].ToString(), "JXHLZQBS"));

                //还原文件名
                string fileName = filePath.Substring(0, filePath.LastIndexOf("\\") + 1) + Request["name"];
                if (!System.IO.Directory.Exists(filePath))  //判断文件是否存在
                //File.Exists(fileName) && File.Exists(filePath)
                {
                    //File.Move(filePath, fileName);  //通过File.Move方法在当前目录下复制一个副本，并还原文件名
                    Response.Clear();  //清除缓冲
                    Response.Charset = "ISO-8859-1";  //提供下载的文件，不编码的话文件名会乱码 
                    Response.ContentEncoding = System.Text.Encoding.UTF8;

                    // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Request["name"]);
                    Response.ContentType = "Application/octet-stream";

                    // 把文件流发送到客户端 
                    Response.WriteFile(fileName);
                    Response.Flush();
                    Response.Close();
                    //File.Move(fileName, filePath);  //删除原来创建的副本
                }
                else
                {
                    result = "文件不存在";
                }
            }
            catch (Exception)
            {
                result = "<script>window.alert('错误消息：该附件不存在，下载失败');window.history.go(-1);</script>";
            }

            jsr = Json(result);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }
        #endregion 

    }
}
