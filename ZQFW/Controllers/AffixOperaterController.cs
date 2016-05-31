using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using EntityModel;
using LogicProcessingClass.AuxiliaryClass;
using LogicProcessingClass.ReportOperate;
using LogicProcessingClass;

namespace ZQFW.Controllers
{
    public class AffixOperaterController : Controller
    {

        /// <summary>上传附件
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFiles(HttpPostedFileBase fileData)
        {
            int pageNO = Convert.ToInt32(Request["PageNo"]);
            int limit = Convert.ToInt32(Request["limit"]);
            CommonFunction commonFun = new CommonFunction();
            string result = "";
            //string unitCode = Request.Cookies["UnitCode"].Value == null ? Request["unitCode"] : Request.Cookies["UnitCode"].Value;
            string unitCode = Request["unitcode"];
            string rptType = Request["rptType"];
            if (unitCode != null && unitCode.StartsWith("15") && rptType =="NP01")//内蒙蓄水
            {
                limit = 2;
            }
            string fileName = ""; //原始文件名
            //string unitName = Request["unitCode"];
            string saveName = unitCode + DateTime.Now.ToString("yyyyMMddHHmmssssffff") + "HL01_0.mil";//存入的文件名

            string downloadURL = "../Affix/"; //下载路径
            string filePath = Server.MapPath("../Affix/");//保存的物理路径
            if (fileData != null)
            {
                fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                try
                {
                    int fileSize = fileData.ContentLength;
                    if (fileSize > 0)
                    {
                        if (fileSize / 1024 <= 5120)
                        {
                            saveName = commonFun.GetNewFileName(saveName);//判断该文件在文件夹中是否已存在，若存在则在后面 +1；
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }
                            fileData.SaveAs(filePath + saveName);
                            Affix af = new Affix();
                            af.FileName = fileName;
                            af.DownloadURL = downloadURL + saveName;
                            af.FileSize = fileSize;

                            SaveOrUpdateReport rpt = new SaveOrUpdateReport();
                            result = rpt.DBUploadFileAffix(pageNO, limit, af);
                            //string[] urlTBNO = null;
                            //if (result.IndexOf("&") != -1)
                            //{
                            //    urlTBNO = result.Split('&');
                            //}
                            return Json(new { url = result.Substring(0, result.LastIndexOf('&')), tbno = result.Substring(result.LastIndexOf('&')+1) }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Message = "文件" + fileName + "太大，请确保文件大小在5M以内！" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Message = fileName + "文件大小不能为0！" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(filePath + saveName))//发生异常时，判断文件是否已经上传到服务器，如果已经上传则删除
                    {
                        System.IO.File.Delete(filePath + saveName);
                    }
                    return Json(new { Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
            #region
            //for (int i = 0; i < Request.Files.Count; i++)
            //{
            //    fileName = Path.GetFileName(Request.Files[i].FileName);
            //    string name = Request["unitCode"] + string.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + "HL01_0.mil";
            //    int fileSize = Request.Files[i].ContentLength;
            //    if (fileSize > 0)
            //    {
            //        if (fileSize / 1024 <= 2048)
            //        {
            //            //判断该文件在文件夹中是否已存在，若存在则在后面 +1；
            //            name = commonFun.GetNewFileName(name);
            //            Request.Files[i].SaveAs(filepath + "\\" + name);
            //            Affix af = new Affix();
            //            af.FileName = fileName;
            //            af.DownloadURL = downloadURL + name;
            //            af.FileSize = fileSize;
            //            aList.Add(af);
            //            flag = true;
            //        }
            //        else
            //        {
            //            flag = false;
            //            result = "fileLoadError:保存失败，" + fileName + "文件太大，请确保文件大小在2M以内！";
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        flag = false;
            //        result = "fileLoadError:保存失败，" + fileName + "文件大小为0，请确保文件大小大于0！";
            //        break;
            //    }

            //}
            #endregion
        }

        /// <summary>下载文件
        /// 需要提供文件名称和加密的下载地址
        /// </summary>
        /// <returns></returns>
        //public JsonResult DownFiles()
        //{
        //    JsonResult jsr = new JsonResult();
        //    string result = "";
        //    try
        //    {
        //        //解密文件路径
        //        string filePath = Server.MapPath(new Tools().EncryptOrDecrypt(1, Request["url"].ToString(), "JXHLZQBS"));

        //        //还原文件名
        //        string fileName = filePath.Substring(0, filePath.LastIndexOf("\\") + 1) + Request["name"];
        //        if (!System.IO.File.Exists(fileName) && System.IO.File.Exists(filePath))  //判断文件是否存在
        //        {
        //            System.IO.File.Move(filePath, fileName);  //通过File.Move方法在还原文件名
        //            Response.Clear();  //清除缓冲
        //            Response.Charset = "ISO-8859-1";  //提供下载的文件，不编码的话文件名会乱码 
        //            Response.ContentEncoding = System.Text.Encoding.UTF8;

        //            // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
        //            Response.AddHeader("Content-Disposition", "attachment; filename=" + Request["name"]);
        //            Response.ContentType = "Application/octet-stream";

        //            //using(StreamFileFromDisk(fileName, Request["name"]);

        //            // 把文件流发送到客户端 
        //            Response.WriteFile(fileName);
        //            Response.Flush();
        //            Response.Close();
        //            System.IO.File.Move(fileName, filePath);  //通过File.Move方法在恢复为以前的文件名
        //        }
        //        else
        //        {
        //            throw new Exception("文件不存在");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = "错误消息：该附件不存在，下载失败!";
        //    }
        //    jsr = Json(result);
        //    jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    return jsr;

        //}

        public string DownloadFile(string file_name)
        {
            string filePath = Server.MapPath("~/Document/File/" + file_name);
            if (System.IO.File.Exists(filePath)) //判断文件是否存在
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        /// <summary>下载文件，发送文件流到客户端
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string DownFiles()
        {
            JsonResult jsr = new JsonResult();
            string result = "";
            string copyName = "";
            string downName=Request["name"];
            try
            {
                //解密文件路径
                string filePath = Server.MapPath(new Tools().EncryptOrDecrypt(1, Request["url"].ToString(), "JXHLZQBS"));

                //还原文件名
                string fileName = filePath.Substring(0, filePath.LastIndexOf("\\") + 1) + downName;
                if (!System.IO.File.Exists(fileName) && System.IO.File.Exists(filePath))  //判断文件是否存在
                {
                    copyName = filePath.Substring(0, filePath.LastIndexOf("\\") + 1)+DateTime.Now.ToString("HHmmssssffff")+".mil";
                    System.IO.File.Copy(filePath, copyName, true);//Copy一份用于还原文件名（使用完之后需要进行删除）
                    System.IO.File.Move(copyName, fileName);  //通过File.Move方法在还原文件名
                    Response.Clear();  //清除缓冲
                    Response.Charset = "ISO-8859-1";  //提供下载的文件，不编码的话文件名会乱码 
                    Response.ContentEncoding = System.Text.Encoding.UTF8;

                    // 添加头信息，为"文件下载/另存为"对话框指定默认文件名
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + downName);
                    Response.ContentType = "Application/octet-stream";

                    // 把文件流发送到客户端 
                    Response.WriteFile(fileName);
                    Response.Flush();
                    Response.Close();
                    //System.IO.File.Move(fileName, filePath);  //通过File.Move方法在恢复为以前的文件名
                    System.IO.File.Delete(fileName);
                    result = "下载成功！";
                }
                else
                {
                    throw new Exception("文件不存在");
                }
               
            }
            catch (Exception)
            {
                result = "<script>window.alert('错误消息：该附件不存在，下载失败');window.history.go(-1);</script>";
                //result = "错误消息：该附件不存在，下载失败!";
            }
            //jsr = Json(result);
            //jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return jsr;
            return result;
        }

        public FileStreamResult StreamFileFromDisk(string fullFileName, string fileName)
        {
            return File(new FileStream(fullFileName, FileMode.Open), "text/plain", fileName);
        }
    }
}
