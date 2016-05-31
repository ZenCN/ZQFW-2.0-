using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;

namespace ZQFW.Views
{
    public partial class EvaluateTable : System.Web.Mvc.ViewPage
    {
        public string UnitCode;   //使用该系统的行政单位代码
        protected void Page_Load(object sender, EventArgs e)
        {
            UnitCode = Request.Cookies["unitcode"].Value;  //使用该系统的行政单位代码
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
        //导出地图
        protected void exportMap_Click(object sender, EventArgs e)
        {
            ////if (Request.Cookies["imgUrl"] == null)
            //if (hidImgUrl.Value == "")
            //{
            //    Response.Write("<script>alert('导出地图失败！')</script>");
            //    return;
            //}

            //string imgPath = "";
            //try
            //{
            //    CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
            //    string filePath = "HTML/StatisticAlanalysis/assessment/evaluatetable.aspx";
            //    string absolutePath = Request.Url.AbsoluteUri;
            //    string virtualPath = absolutePath.Substring(0, Compare.IndexOf(absolutePath, filePath, CompareOptions.IgnoreCase));  //获取虚拟目录(截取字符串时忽略大小写)
            //    string url = virtualPath + "HTML/StatisticAlanalysis/printMap.aspx";

            //    //InternetSetCookie(url, "imgName", hidImgName.Value);
            //    //InternetSetCookie(url, "imgUrl", hidImgUrl.Value);
            //    //InternetSetCookie(url, "mapHeight", hidMapHeight.Value);
            //    //InternetSetCookie(url, "mapWidth", hidMapWidth.Value);
            //    //InternetSetCookie(url, "legend", hidLegend.Value);

            //    //string encodedImgName = "%E6%B1%9F%E8%A5%BF%E7%9C%812013%E5%B9%B410%E6%9C%8810%E6%97%A5-10%E6%9C%8810%E6%97%A5%E5%8F%97%E7%81%BE%E4%BA%BA%E5%8F%A3%E5%88%86%E5%B8%83%E5%9B%BE";
            //    //string encodedImgUrl = "http%3A%2F%2Flocalhost%2FBJHL%2FMapServer%2Fmap.asp%3Fstate%3Dlayers%253D0%252C1%2524viewport%253D9%252E4356284653742562e%252D003%252C9%252E4356284653742562e%252D003%252C1%252E1602871529578806e%252B002%252C2%252E7285590789921116e%252B001%2524%26command%3D%26mode%3D%26query%3D%26queryPars%3D%26what%3D%26where%3D%26x%3D%26y%3D%26boxSizeX%3D1190%26boxSizeY%3D600%26mapIndex%3D1";
            //    //string encodedMapHeight = "600";
            //    //string encodedMapWidth = "1192";
            //    //string encodedLegend = "%3Cdiv%20id%3D%22legend%22%3E%3Cspan%20id%3D%22zoomer%22%20class%3D%22notprint%22%3E%3C%2Fspan%3E%3Clabel%3E%E5%8F%97%E7%81%BE%E4%BA%BA%E5%8F%A3(%E4%B8%87%E4%BA%BA)%3C%2Flabel%3E%3Cul%3E%3Cli%3E%3Cdiv%20class%3D%22colorblocks%22%20style%3D%22background%3A%239ABA18%22%3E%3Csvg%20version%3D%221.1%22%3E%3Crect%20style%3D%22fill%3A%239ABA18%3Bfill-opacity%3A0.7%3B%22%20width%3D%22100%25%22%20height%3D%22100%25%22%3E%3C%2Frect%3E%3C%2Fsvg%3E%3C%2Fdiv%3E%3Cp%3E%E5%B0%8F%E4%BA%8E20%3C%2Fp%3E%3C%2Fli%3E%3Cli%3E%3Cdiv%20class%3D%22colorblocks%22%20style%3D%22background%3A%23BAA404%22%3E%3Csvg%20version%3D%221.1%22%3E%3Crect%20style%3D%22fill%3A%23BAA404%3Bfill-opacity%3A0.7%3B%22%20width%3D%22100%25%22%20height%3D%22100%25%22%3E%3C%2Frect%3E%3C%2Fsvg%3E%3C%2Fdiv%3E%3Cp%3E20-40%3C%2Fp%3E%3C%2Fli%3E%3Cli%3E%3Cdiv%20class%3D%22colorblocks%22%20style%3D%22background%3A%23DA7804%22%3E%3Csvg%20version%3D%221.1%22%3E%3Crect%20style%3D%22fill%3A%23DA7804%3Bfill-opacity%3A0.7%3B%22%20width%3D%22100%25%22%20height%3D%22100%25%22%3E%3C%2Frect%3E%3C%2Fsvg%3E%3C%2Fdiv%3E%3Cp%3E40-60%3C%2Fp%3E%3C%2Fli%3E%3Cli%3E%3Cdiv%20class%3D%22colorblocks%22%20style%3D%22background%3A%23F04000%22%3E%3Csvg%20version%3D%221.1%22%3E%3Crect%20style%3D%22fill%3A%23F04000%3Bfill-opacity%3A0.7%3B%22%20width%3D%22100%25%22%20height%3D%22100%25%22%3E%3C%2Frect%3E%3C%2Fsvg%3E%3C%2Fdiv%3E%3Cp%3E60-120%3C%2Fp%3E%3C%2Fli%3E%3Cli%3E%3Cdiv%20class%3D%22colorblocks%22%20style%3D%22background%3A%23FF0000%22%3E%3Csvg%20version%3D%221.1%22%3E%3Crect%20style%3D%22fill%3A%23FF0000%3Bfill-opacity%3A0.7%3B%22%20width%3D%22100%25%22%20height%3D%22100%25%22%3E%3C%2Frect%3E%3C%2Fsvg%3E%3C%2Fdiv%3E%3Cp%3E%E5%A4%A7%E4%BA%8E120%3C%2Fp%3E%3C%2Fli%3E%3C%2Ful%3E%3C%2Fdiv%3E";

            //    //int imgHeight = Convert.ToInt32(Server.UrlDecode(encodedMapHeight)) + 42;
            //    //int imgWidth = Convert.ToInt32(Server.UrlDecode(encodedMapWidth)) + 2;
            //    string imgName = Server.UrlDecode(hidImgName.Value);
            //    string imgUrl = Server.UrlDecode(hidImgUrl.Value);
            //    int mapHeight = Convert.ToInt32(Server.UrlDecode(hidMapHeight.Value));
            //    int mapWidth = Convert.ToInt32(Server.UrlDecode(hidMapWidth.Value));
            //    string legend = Server.UrlDecode(hidLegend.Value);

            //    GetImage thumb = new GetImage();
            //    System.Drawing.Bitmap bitmap = thumb.GetBitmap(url, imgName, imgUrl, mapWidth, mapHeight, legend);

            //    System.Random random = new Random(DateTime.Now.Millisecond);
            //    int RandKey = random.Next(100);
            //    string randStr = imgName + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()
            //        + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + RandKey.ToString();
            //    imgPath = Server.MapPath("~/") + imgName + randStr + ".png";
            //    bitmap.Save(imgPath, System.Drawing.Imaging.ImageFormat.Png); //保存

            //    System.IO.FileInfo DownloadFile = new System.IO.FileInfo(imgPath);
            //    if (DownloadFile.Exists)
            //    {
            //        Response.Clear();
            //        Response.ClearHeaders();
            //        Response.Buffer = false;
            //        Response.ContentType = "application/octet-stream";
            //        Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(imgName + ".png", System.Text.Encoding.UTF8));
            //        Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
            //        Response.WriteFile(DownloadFile.FullName);
            //        Response.Flush();
            //        Response.End();
            //    }
            //    else
            //    {
            //        //文件不存在 
            //        Response.Write("<script>alert('导出地图失败！')</script>");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string a = ex.Message;
            //    //意外出错
            //    Response.Write("<script>alert('导出地图失败！')</script>");
            //}
            //finally
            //{
            //    if (File.Exists(imgPath))
            //        File.Delete(imgPath);
            //}
        }
    }
}