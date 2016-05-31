using System;
using System.Linq;
using System.Web.Mvc;
using EntityModel;
using DBHelper;
using LogicProcessingClass.Statistics;
using System.Drawing.PieChart;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.IO;
using System.Threading;

namespace ZQFW.Controllers
{
    public class StatisticsController : Controller
    {
        //
        // GET: /Statistics/

        //统计分析页面
        public ActionResult Index()
        {
            if (Session["SESSION_USER"] != null)
            {
                return View();
            }
            else
            {
                return Redirect("/Login");
            }
        }
        /****************************灾情分布******************************/
        //灾情分布地图
        public ActionResult Map()
        {
            return View();
        }
        //地图截图
        public ActionResult printMap()
        {
            return View();
        }
        //蓄水图地图截图
        public ActionResult printWSMap()
        {
            return View();
        }
        ////导出三维饼图
        public ActionResult ChartP3()
        {
            return View();
        }
        //统计图
        public ActionResult NewBB()
        {
            return View();
        }
        /****************************灾情分析******************************/
        //灾情分析
        public ActionResult Analysis()
        {
            return View();
        }
        /****************************灾情评估******************************/
        //灾情评估
        public ActionResult Assessment()
        {
            return View();
        }

        //洪涝灾情场次评估表
        public ActionResult EvaluateTable()
        {
            return View();
        }
        //洪涝灾情年度评估表
        public ActionResult EvaluateTableOfYear()
        {
            return View();
        }
        //洪涝灾情年度评估饼图
        public ActionResult PieChart()
        {
            return View();
        }

        /// <summary>//获取时段类型   /*吴博怀 20140222*/
        /// 
        /// </summary>
        /// <returns>时段类型</returns>
        /******************************************************************/
        /******************************************************************/
        /******************************************************************/
        /****************************灾情分布******************************/
        public JsonResult GetCycTypes()
        {
            FXDICTEntities fxdictEntity = (FXDICTEntities)new Entities().GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            var c = (from tb16 in fxdictEntity.TB16_OperateReportDefine
                     select tb16.OptionalCyc).ToList();
            int[] cArray = new int[c[0].Split(',').Length];
            for (int i = 0; i < cArray.Length; i++)
            {
                cArray[i] = Convert.ToInt16(c[0].Split(',')[i]);
            }
            var cycType = from tb14 in fxdictEntity.TB14_Cyc
                          from tb16 in cArray
                          where tb14.CycType == tb16
                          select tb14;
            return Json(cycType.ToList());
        }

        /// <summary>获取所查询的时段类型的数据包括的年份    /*吴博怀 20140222*/
        /// 
        /// </summary>
        /// <param name="cycTypes">时段类型</param>
        /// <returns>年份</returns>
        public JsonResult GetYears(string cycTypes)
        {
            string[] strCycTypeArr = cycTypes.Split(',');
            decimal[] cycTypeArr = Array.ConvertAll<string, decimal>(strCycTypeArr, s => Convert.ToDecimal(s));
            int level = int.Parse(Request.Cookies["limit"].Value);
            BusinessEntities bsnEntities = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
            var years = (from rpt in bsnEntities.ReportTitle
                         from d in cycTypeArr
                         where rpt.StatisticalCycType == d && rpt.Del == 0 && rpt.RPTType_Code == "XZ0" && rpt.ORD_Code == "HL01"
                         select new { year = rpt.EndDateTime.Value.Year }).Distinct()
                             .OrderByDescending(y => y.year).Select(y => y.year).ToArray();
            return Json(years);
        }
        /// <summary>获取sidetype表示的灾害类型（洪涝或抗旱）year年的cyc时段类型的报表数据（用于生成标尺）  /*吴博怀 20140222*/
        /// 
        /// </summary>
        /// <param name="sidetype">灾害类型（洪涝或抗旱）</param>
        /// <param name="year">年份</param>
        /// <param name="cyctype">时段类型</param>
        /// <returns>标尺数据</returns>
        public JsonResult GetScaleData(string sidetype, decimal cyctype, int? year)
        {
            if (year == null) return null;
            FXDICTEntities fxdictEntities = (FXDICTEntities)new Entities().GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            int level = Convert.ToInt32(Request.Cookies["limit"].Value);
            BusinessEntities bsnEntities = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
            var scaleData = from rpt in bsnEntities.ReportTitle.ToList()
                            from c in fxdictEntities.TB14_Cyc
                            where rpt.StatisticalCycType == c.CycType && rpt.RPTType_Code == "XZ0" && rpt.ORD_Code == "HL01"
                            && rpt.EndDateTime.Value.Year == year && rpt.StatisticalCycType == cyctype && rpt.Del == 0
                            orderby rpt.EndDateTime.Value.Month, rpt.EndDateTime.Value.Day, rpt.StartDateTime.Value.Month, rpt.StartDateTime.Value.Day
                            select new
                            {
                                Pageno = rpt.PageNO,
                                StartDate = Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy,MM,dd"),
                                EndDate = Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy,MM,dd"),
                                SorceType = cyctype,
                                SorceTypeName = c.Remark,
                                WriteTime = Convert.ToDateTime(rpt.LastUpdateTime == null ? rpt.WriterTime : rpt.LastUpdateTime).ToString("yyyy年MM月dd日 HH:mm:ss")
                            };
            return Json(scaleData);
        }

        /// <summary>获取八大类型灾情数据用户地图渲染
        /// 
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="pageNO">页号</param>
        /// <param name="unitCode">单位代码</param>
        /// <param name="mapType">地图类型</param>
        /// <returns>地图数据</returns>
        public JsonResult GetDataForMap(int level, string pageNO, string unitCode, int mapType)
        {
            MapBean mapBean = new MapBean();
            return Json(mapBean.GetDataForMap(level, pageNO, unitCode, mapType));
        }
        #region 生成、导出三维饼图
        /// <summary>生成三维饼图 /*修改 胡蔚星 20140320*/
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        //public void GetPieChart(int width, int height)
        //{
        //    string[] strValues = (Request["Values"]).Split(',');

        //    ArrayList arrayValue = new ArrayList();
        //    for (int i = 0; i < strValues.Length; i++)
        //    {
        //        arrayValue.Add(Convert.ToDecimal(strValues[i]));
        //        //arrayText.Add(strTexts[i] + ":" + strValues[i] + unit);
        //        //arrayText.Add("");
        //    }
        //    decimal[] Values = (decimal[])arrayValue.ToArray(typeof(decimal));
        //    //string[] Texts = (string[])arrayText.ToArray(typeof(string));
        //    int minMargin = 0;
        //    double scale = 2;
        //    double imgWidth = 500;
        //    double imgHeight = 200;
        //    double margin_left = minMargin;
        //    double margin_top = minMargin;
        //    if ((width - 2 * minMargin) / (height - 2 * minMargin) < scale)
        //    {
        //        imgWidth = width - 2 * minMargin;
        //        imgHeight = imgWidth / scale;
        //        margin_top = (height - imgHeight) / 2;
        //    }
        //    else
        //    {
        //        imgHeight = height - 2 * minMargin;
        //        imgWidth = imgHeight * scale;
        //        margin_left = (width - imgWidth) / 2;
        //    }


        //    Bitmap bit = new Bitmap(width, height);
        //    Graphics g = Graphics.FromImage(bit);
        //    g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);

        //    try
        //    {
        //        PieChart3D pieChart = new PieChart3D((float)margin_left, (float)margin_top, (float)imgWidth, (float)imgHeight, Values, 0.1F);
        //        pieChart.SliceRelativeHeight = 0.15F;
        //        pieChart.InitialAngle = 0F;
        //        //pieChart.Texts = Texts;
        //        pieChart.SliceRelativeDisplacement = 0.2F;
        //        pieChart.Font = new Font("宋体", 9F);
        //        pieChart.EdgeColorType = EdgeColorType.NoEdge;
        //        pieChart.Colors = new Color[] {
        //                                        Color.Red,
        //                                        Color.Green,
        //                                        Color.Blue,
        //                                        Color.Yellow,
        //                                        Color.Purple,
        //                                        Color.Olive,
        //                                        Color.Navy,
        //                                        Color.Aqua,
        //                                        Color.Lime,
        //                                        Color.Maroon,
        //                                        Color.Teal,
        //                                        Color.Fuchsia,
        //                                        Color.BlueViolet,
        //                                        Color.Brown,
        //                                        //Color.AliceBlue,
        //                                        Color.BurlyWood,
        //                                        Color.Aquamarine,
        //                                        Color.CadetBlue,
        //                                        //Color.Azure,
        //                                        Color.Chartreuse,
        //                                        //Color.Beige,
        //                                        Color.Chocolate,
        //                                        Color.Coral,
        //                                        //Color.Bisque,

        //                                        Color.DarkBlue,
        //                                        Color.DarkCyan,
        //                                        Color.DarkGoldenrod,
        //                                        Color.DarkGray,
        //                                        Color.DarkGreen,
        //                                        Color.DarkKhaki,
        //                                        Color.DarkMagenta,
        //                                        Color.DarkOliveGreen,
        //                                        Color.DarkOrange,
        //                                        Color.DarkOrchid,
        //                                        Color.DarkRed,
        //                                        Color.DarkSalmon,
        //                                        Color.DarkSeaGreen,
        //                                        Color.DarkSlateBlue,
        //                                        Color.DarkSlateGray,
        //                                        Color.DarkTurquoise,
        //                                        Color.DarkViolet,
        //                                        Color.DeepPink,
        //                                        Color.DeepSkyBlue,
        //                                        Color.DimGray,
        //                                        Color.DodgerBlue
        //                                      };
        //        pieChart.Alpha = 255;
        //        pieChart.ShadowStyle = ShadowStyle.GradualShadow;
        //        pieChart.ForeColor = Color.FromArgb(60, 60, 60);

        //        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //        pieChart.Draw(g);
        //        //pieChart.PlaceTexts(g);

        //        //string imageName = context.Request["imageName"];
        //        //string imagePath = context.Server.MapPath("~/MapServer/tempimage/") + imageName;
        //        //bit.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);   //保存到本地


        //        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //        bit.Save(ms, System.Drawing.Imaging.ImageFormat.Png);   //保存到输出流中


        //        Response.ClearContent();
        //        Response.ContentType = "Image/png";
        //        Response.BinaryWrite(ms.ToArray());
        //    }
        //    finally
        //    {
        //        g.Dispose();
        //        bit.Dispose();
        //    }
        //}
        /// <summary>启动导出饼图线程
        /// 
        /// </summary>

        //public void newP3Thread()
        //{
        //    if (Request.Form["hidTitle"] == "")
        //    {
        //        Response.Write("<script>alert('导出饼图失败！')</script>");

        //    }
        //    ThreadStart start = new ThreadStart(exportP3);
        //    //System.Threading.Thread th = new System.Threading.Thread(start);
        //    newThread = new Thread(start);
        //    newThread.SetApartmentState(ApartmentState.STA);
        //    newThread.Start();
        //    newThread.Join();
        //}
        /// <summary>导出三维饼图
        /// 
        /// </summary>
        //public void exportP3()
        //{
        //    //if (Request.Cookies["imgUrl"] == null)
        //    if (Response.IsClientConnected)
        //    {
        //        string imgPath = "";
        //        string port = Request.Url.Port.ToString();
        //        string localhost = "http://localhost:" + port;
        //        string virtualPath = Request.ServerVariables.Get("Http_Host").ToString();  //获取虚拟目录(外网根目录)
        //        try
        //        {
        //            CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
        //            //string filePath = "Statistics/newP3Thread";
        //            //string absolutePath = Request.Url.AbsoluteUri;
        //            //string virtualPath = absolutePath.Substring(0, Compare.IndexOf(absolutePath, filePath, CompareOptions.IgnoreCase));  //获取虚拟目录(截取字符串时忽略大小写)
        //            //string url = "http://" + virtualPath + "/Statistics/ChartP3";
        //            string url = localhost + "/Statistics/ChartP3";
        //            string title = Server.UrlDecode(Request.Form["hidTitle"]);
        //            string subtitle = Server.UrlDecode(Request.Form["hidSubtitle"]);
        //            string strNames = Server.UrlDecode(Request.Form["hidNames"]);
        //            string strDatas = Server.UrlDecode(Request.Form["hidDatas"]);
        //            string dataUnit = Server.UrlDecode(Request.Form["hidDataUnit"]);
        //            int width = Convert.ToInt32(Server.UrlDecode(Request.Form["hidWidth"]));
        //            //string imageName = Server.UrlDecode(hidImgName.Value);

        //            WebPageBitmap wpb = new WebPageBitmap(url, title, subtitle, strNames, strDatas, dataUnit, width);
        //            System.Drawing.Bitmap bitmap = wpb.DrawBitmap();

        //            //imgPath = Server.MapPath("~/") + title + ".png";
        //            System.Random random = new Random(DateTime.Now.Millisecond);
        //            int RandKey = random.Next(100);
        //            string randStr = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()
        //                + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + RandKey.ToString();
        //            imgPath = MapPath("") + "ExportPicture\\" + randStr + ".png";
        //            bitmap.Save(imgPath, System.Drawing.Imaging.ImageFormat.Png); //保存

        //            //File(imgPath,title);
        //            System.IO.FileInfo DownloadFile = new System.IO.FileInfo(imgPath);
        //            if (DownloadFile.Exists)
        //            {
        //                if (Response.IsClientConnected)
        //                {
        //                    Response.Clear();
        //                    Response.ClearHeaders();
        //                    Response.Buffer = false;
        //                    Response.ContentType = "application/octet-stream";
        //                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(title, System.Text.Encoding.UTF8) + ".png");
        //                    Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
        //                    Response.WriteFile(imgPath);
        //                    Response.Flush();
        //                    Response.End();
        //                }
        //            }
        //            else
        //            {
        //                //文件不存在 
        //                Response.Write("<script>alert('导出饼图失败！')</script>");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //意外出错
        //            Response.Write("<script>alert('导出饼图失败！" + ex.Message + "')</script>");
        //        }
        //        finally
        //        {
        //            if (System.IO.File.Exists(imgPath))
        //                System.IO.File.Delete(imgPath);
        //        }
        //        newThread.Abort();
        //    }
        //}
        #endregion
        #region 导出地图
        /// <summary>导出地图线程 /*修改 胡蔚星 20140320*/
        /// 
        /// </summary>
        Thread newThread;
        public void newMapThread()
        {
            if (Request.Form["hidImgUrl"] == "")
            {
                Response.Write("<script>alert('导出地图失败！')</script>");

            }
            ThreadStart start = new ThreadStart(btnexportMap);
            newThread = new Thread(start);
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
            newThread.Join();
        }

        protected void btnexportMap()
        {
            //if (Request.Cookies["imgUrl"] == null)
            //if (hidImgUrl.Value == "")
            //{
            //    Response.Write("<script>alert('导出地图失败！')</script>");
            //    return;
            //}
            if (Response.IsClientConnected)
            {
                string mapType = "";
                string imgPath = MapPath("") + "ExportPicture\\";
                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }
                //string filePath = "Statistics/newMapThread";
                //string absolutePath = Request.Url.AbsoluteUri;
                string port = Request.Url.Port.ToString();
                string localhost = "http://localhost:" + port;
                string virtualPath = Request.ServerVariables.Get("Http_Host").ToString();  //获取虚拟目录(外网根目录)
                //string virtualPath = absolutePath.Substring(0, Compare.IndexOf(absolutePath, filePath, CompareOptions.IgnoreCase));  //获取虚拟目录(截取字符串时忽略大小写)
                try
                {
                    CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
                    //string url = "http://" + virtualPath + "/Statistics/printMap";
                    mapType = Server.UrlDecode(Request.Form["mapType"]);
                    string url = localhost + "/Statistics/" + (mapType == "4" ? "printWSMap" : "printMap");
                    string imgName = Server.UrlDecode(Request.Form["hidImgName"]);
                    string imgUrl = Server.UrlDecode(Request.Form["hidImgUrl"]);
                    int HttpHostLength = imgUrl.IndexOf(virtualPath) + virtualPath.Length;
                    imgUrl = localhost + imgUrl.Substring(HttpHostLength);
                    int mapHeight = Convert.ToInt32(Server.UrlDecode(Request.Form["hidMapHeight"]));
                    int mapWidth = Convert.ToInt32(Server.UrlDecode(Request.Form["hidMapWidth"]));
                    string legend = Server.UrlDecode(Request.Form["hidLegend"]);
                    WebPageBitmap wpb = new WebPageBitmap(url, imgName, imgUrl, mapWidth, mapHeight, legend);
                    System.Drawing.Bitmap bitmap = wpb.DrawBitmap();
                    //GetImage thumb = new GetImage();
                    //System.Drawing.Bitmap bitmap = thumb.GetBitmap(url, imgName, imgUrl, mapWidth, mapHeight, legend);
                    System.Random random = new Random(DateTime.Now.Millisecond);
                    int RandKey = random.Next(100);
                    string randStr = "M" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()
                        + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + RandKey.ToString();
                    imgPath = imgPath + randStr + ".png";
                    bitmap.Save(imgPath, System.Drawing.Imaging.ImageFormat.Png); //保存
                    if (Response.IsClientConnected)
                    {
                        if (mapType == "4")
                        {
                            Response.Write(imgPath);
                        }
                        else
                        {
                            System.IO.FileInfo DownloadFile = new System.IO.FileInfo(imgPath);
                            if (DownloadFile.Exists)
                            {
                                Response.Clear();
                                Response.ClearHeaders();
                                Response.Buffer = false;
                                Response.ContentType = "application/octet-stream";
                                Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(imgName + ".png", System.Text.Encoding.UTF8));
                                Response.AppendHeader("Content-Length", DownloadFile.Length.ToString());
                                Response.WriteFile(DownloadFile.FullName);
                                Response.Flush();
                                Response.End();
                            }
                            else
                            {
                                //文件不存在 
                                Response.Write("<script>alert('导出地图失败！')</script>");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //意外出错
                    Response.Write("<script>alert('意外出错，导出地图失败！" + ex.Message + "')</script>");
                }
                finally
                {
                    if (mapType != "4" && System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);
                }
                newThread.Abort();
            }
        }
        #endregion
        /// <summary>处理导出文件的路径 /*胡蔚星 20140320*/
        /// 
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>

        public static string MapPath(string strPath)
        {
            if (System.Web.HttpContext.Current != null)
            {
                return System.Web.HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用   
            {
                strPath = strPath.Replace("/", "");
                strPath = strPath.Replace("~", "");
                //if (strPath.StartsWith("//"))  
                //{  
                //    strPath = strPath.TrimStart('//');  
                //}  
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        /******************************************************************/
        /******************************************************************/
        /******************************************************************/
        /****************************灾情分析******************************/
        /// <summary>第一次进入灾情分析页面时获取初始起始时间的报表相关信息
        /// 
        /// </summary>
        /// <param name="beginYear">开始年</param>
        /// <param name="endYear">结束年</param>
        /// <param name="beginMonth">开始月份</param>
        /// <param name="beginDay">开始日期</param>
        /// <param name="bDateRange">开始日期范围</param>
        /// <param name="endMonth">结束月份</param>
        /// <param name="endDay">结束日期</param>
        /// <param name="eDateRange">结束日期范围</param>
        /// <param name="cycTypes">报表时段类型</param>
        /// <param name="pageNumDataList">数据的当前页码</param>
        /// <param name="pageLineNumDataList">数据的每页显示行数</param>
        /// <param name="pageNumTrueNode">树形菜单的当前页码</param>
        /// <param name="pageLineNumTrueNode">树形菜单每页显示行数</param>
        /// <returns>报表相关信息</returns>
        public JsonResult FirstDisasterAnalysis(int beginYear, int endYear, string beginMonth, string beginDay, string bDateRange,
            string endMonth, string endDay, string eDateRange, string cycTypes, int pageNumDataList, int pageLineNumDataList,
            int pageNumTrueNode, int pageLineNumTrueNode)
        {
            string unitCode = Request.Cookies["unitCode"].Value;
            int level = int.Parse(Request.Cookies["limit"].Value);
            DisasterAnalysisMasterControl camc = new DisasterAnalysisMasterControl();
            var jsonStr = camc.GetDisasterAnalysisData(beginYear, endYear, beginMonth, beginDay, bDateRange,
                 endMonth, endDay, eDateRange, cycTypes, pageNumDataList, pageLineNumDataList, pageNumTrueNode, pageLineNumTrueNode, unitCode, level);
            return Json(jsonStr);
        }

        /// <summary>获取指定起始时间的报表相关信息
        /// 
        /// </summary>
        /// <param name="beginYear">开始年</param>
        /// <param name="endYear">结束年</param>
        /// <param name="beginMonth">开始月份</param>
        /// <param name="beginDay">开始日期</param>
        /// <param name="bDateRange">开始日期范围</param>
        /// <param name="endMonth">结束月份</param>
        /// <param name="endDay">结束日期</param>
        /// <param name="eDateRange">结束日期范围</param>
        /// <param name="cycTypes">报表时段类型</param>
        /// <param name="pageNum">当前页码</param>
        /// <param name="pageLineNum">每页显示行数</param>
        /// <returns>报表相关信息</returns>
        public JsonResult DisasterAnalysisDataList(int beginYear, int endYear, string beginMonth, string beginDay, string bDateRange,
            string endMonth, string endDay, string eDateRange, string cycTypes, int pageNum, int pageLineNum)
        {
            int level = int.Parse(Request.Cookies["limit"].Value);
            BusinessEntities bsn = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
            FXDICTEntities fxdict = new FXDICTEntities();
            DisasterAnalysis da = new DisasterAnalysis();
            var jsonStr = da.DisasterAnalysisConditionQuery(beginYear, endYear, beginMonth, beginDay, bDateRange,
             endMonth, endDay, eDateRange, cycTypes, pageNum, pageLineNum, bsn, fxdict);
            return Json(jsonStr);
        }



        /// <summary>获取统计数据（用于生成柱状图）
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="disasterType">灾情类型</param>
        /// <param name="unitCodes">单位代码</param>
        /// <returns></returns>
        public JsonResult DisasterAnalysisChart(string pageNOs, string disasterType, string unitCodes)
        {
            if ("".Equals(pageNOs) || "".Equals(unitCodes))
            {
                return Json(new { });
            }
            int level = int.Parse(Request.Cookies["limit"].Value);
            BusinessEntities bsnEntities = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
            DisasterAnalysis da = new DisasterAnalysis();
            string[] strPageNOArr = pageNOs.Split(',');
            int[] pageNOArr = Array.ConvertAll<string, int>(strPageNOArr, strNum => Convert.ToInt32(strNum));
            disasterType = disasterType.ToLower();
            string strTitle = da.GetQSTitle(disasterType);//标题
            string[] timeArr = da.GetDisasterAnalysisDataTime(pageNOArr, bsnEntities);//获得X轴数据
            QSDataDWBean[] qsDataDWs = da.GetDisasterAnalysisData(pageNOArr, disasterType, unitCodes, bsnEntities);//Y轴点数据
            var dataSet = new { Title = strTitle, SubTitle = "", Categories = timeArr, Series = qsDataDWs };
            return Json(dataSet);
        }

        /// <summary>根据页号查询统计表中的灾情数据
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <returns></returns>
        public string BBData()
        {
            string pageno = "";
            int level = int.Parse(Request.Cookies["limit"].Value);
            string regionCode = Request["unitCode"];
            int mapLevel = int.Parse(Request["mapLevel"]);
            if (Request["pageno"] == null || Request["pageno"] == "")
            {
                pageno = "0";
            }
            else
            {
                pageno = Request["pageno"];
            }


            GetBBData gbbd = new GetBBData();
            string json = gbbd.ShowTableTile(pageno, level, mapLevel, regionCode);

            return json;
        }
        /******************************************************************/
        /******************************************************************/
        /******************************************************************/
        /****************************灾情评估******************************/
        /// <summary>获取场次洪涝灾情评估提纲（标题、报表时段类型、时间、灾情等级）
        /// 
        /// </summary>
        /// <param name="evalType">评估类型</param>
        /// <param name="currentCount">当前已查询条数</param>
        /// <param name="queryCount">此次查询条数</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>场次洪涝灾情评估提纲（标题、报表时段类型、时间、灾情等级）</returns>
        public JsonResult GetDisasterAssessmentTitle(int evalType, int currentCount, int queryCount, string startDate, string endDate)
        {
            DateTime startDateTime = (startDate == "") ? DateTime.Now.AddYears(-100) : Convert.ToDateTime(startDate);
            DateTime endDateTime = (endDate == "") ? DateTime.Now.AddYears(1) : Convert.ToDateTime(endDate);
            string unitCode = Request.Cookies["unitcode"].Value;
            int level = Convert.ToInt32(Request.Cookies["limit"].Value);
            DisasterAssessment_title da_title = new DisasterAssessment_title(level);
            if (evalType == 0)  //如果评估类型为场次评估
            {
                var jsonData = da_title.GetSingleEvaluationTitle(currentCount, queryCount, startDateTime, endDateTime, unitCode);//获取评估标题名称、时间、级别
                return Json(jsonData);
            }
            else    //如果评估类型为年度评估
            {
                var jsonData = da_title.GetAnnualEvaluationTitle(currentCount, queryCount, startDateTime, endDateTime);//获取评估标题名称、时间、级别
                return Json(jsonData);
            }
        }

        /// <summary>获取灾情评估详细内容
        /// 
        /// </summary>
        /// <param name="evalType">评估类型</param>
        /// <param name="pageNO">页号</param>
        /// <returns></returns>
        public JsonResult GetDisasterAssessmentContent(int evalType, int pageNO)
        {
            int level = Convert.ToInt32(Request.Cookies["limit"].Value);
            DisasterAssessment_Content content = new DisasterAssessment_Content(level);
            string startEndDate = content.GetStartEndDate(pageNO);
            int unitCount = content.GetUnitCount(pageNO);
            int lowerUnitCount = content.GetLowerLevelUnitCount(pageNO, level);
            IList<object> objDetail = content.GetDisasterData(pageNO, evalType);
            var jsonData = new { period = startEndDate, countofprovince = unitCount, countofcounty = lowerUnitCount, detail = objDetail };
            return Json(jsonData);
        }
        /// <summary>获取灾情评估地图中每个单位的灾情等级
        /// 
        /// </summary>
        /// <param name="pageNO"></param>
        /// <param name="UnitCode"></param>
        /// <returns></returns>
        public JsonResult GetDWDisasterLevel(int pageNO)
        {
            int level = Convert.ToInt32(Request.Cookies["limit"].Value);
            getDisasterLevelForMap getLevel = new getDisasterLevelForMap(level);
            object DWLevelList = getLevel.getDisasterLevel(pageNO, level);
            return Json(DWLevelList);

        }
        /// <summary>获取年度灾情评估饼状图数据
        /// 
        /// </summary>
        /// <param name="pageNO"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonResult GetPieChartData(int pageNO, string type)
        {
            int level = Convert.ToInt32(Request.Cookies["limit"].Value);
            DisasterAssessment_Content content = new DisasterAssessment_Content(level);
            var pieChartData = content.GetPieChartData(pageNO, type);
            return Json(pieChartData);
        }




    }

}
