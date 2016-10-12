using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using EntityModel;
using DBHelper;
using System.Collections;
using System.Data;
using LogicProcessingClass.XMMZH;
using EntityModel.ReportAuxiliaryModel;
using LogicProcessingClass.AuxiliaryClass;
using System.Globalization;
using System.Net.Cache;
using Newtonsoft.Json;

/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：ViewReportForm.cs
// 文件功能描述：省市县查看报表数据
// 创建标识：
// 修改标识：

// 修改描述：
//-------------------------------------------------------------*/
namespace LogicProcessingClass.ReportOperate
{
    public class ViewReportForm
    {
        BusinessEntities changeEntity = null;
        /// <summary>
        /// 返回该单位最后一次填写的表头ReportTitle信息
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="limit">单位级别</param>
        /// <returns></returns>
        public string ViewReportTitleInfo(string unitCode, int limit)
        {
            string reportInfo = "";
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                BusinessEntities busEntity = Persistence.GetDbEntities(limit);
                var rpts = from rpt in busEntity.ReportTitle
                    where rpt.UnitCode == unitCode && rpt.ORD_Code == "HL01" && rpt.RPTType_Code == "XZ0"
                          && rpt.Del != 1 && rpt.CopyPageNO == 0 && rpt.SendOperType == 0 && rpt.CSPageNO == 0
                    orderby rpt.PageNO descending
                    select rpt;
                //IList<ReportTitle> rptList = rpts.ToList<ReportTitle>();
                XMMZHClass ZH = new XMMZHClass();
                if (rpts.Count() > 0)
                {
                    LZReportTitle rt = ZH.ZHQTReportTitle(rpts.First());
                    reportInfo = "RecentReportInfo:{HL01:" + serializer.Serialize(rt).Replace("\"", "'") + "}";
                    //reportInfo = "ReportInfo:" + serializer.Serialize(rpts.First()); // rpt对象中还有其他导航属性 会导致循环引用，需要一个其他辅助类XMMReportTitle
                }
                else
                {
                    reportInfo = "RecentReportInfo:{HL01:{}}";
                }
                busEntity.Dispose();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return reportInfo;
        }

        /// <summary>
        /// 返回该单位最后一次填写的表头ReportTitle信息
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="limit">单位级别</param>
        /// <param name="rptType">洪涝表:HL01,蓄水表：HP01</param>
        /// <returns></returns>
        public string ViewReportTitleInfo(string unitCode, int limit, string rptType)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string reportInfo = "";
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            var rpts = from report in busEntity.ReportTitle
                       where report.UnitCode == unitCode &&
                        report.ORD_Code == rptType
                       orderby report.PageNO descending
                       select report;
            //IList<ReportTitle> rptList = rpts.ToList<ReportTitle>();
            XMMZHClass ZH = new XMMZHClass();
            if (rpts.Count() > 0)
            {
                LZReportTitle rt = ZH.ZHQTReportTitle(rpts.First());
                reportInfo = "RecentReportInfo:" + serializer.Serialize(rt).Replace("\"", "'");
                //reportInfo = "ReportInfo:" + serializer.Serialize(rpts.First()); // rpt对象中还有其他导航属性 会导致循环引用，需要一个其他辅助类XMMReportTitle
            }
            else
            {
                reportInfo = "RecentReportInfo:{}";
            }
            busEntity.Dispose();
            return reportInfo;
        }

        /// <summary>
        /// 打开单张表
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <param name="pageNO">报表编号</param>
        /// <param name="rptType">报表类型</param>
        /// <returns></returns>
        public string ViewReportFormInfo(int limit, int pageNO, string rptType)
        {
            string jsonStr = "";
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            XMMZHClass ZH = new XMMZHClass();
            JavaScriptSerializer serializer = new JavaScriptSerializer(); //创建一个序列化对象
            var rpts = from rpt in busEntity.ReportTitle
                       where rpt.PageNO == pageNO
                       select rpt;
            string reportInfo = "";
            string tmp = "";
            if (rpts.Count() > 0)
            {
                var rpt = rpts.First();
                reportInfo = "ReportTitle:" + serializer.Serialize(ZH.ZHQTReportTitle(rpt));
                if (rpt.Affix.Count() > 0)
                {
                    Tools tool = new Tools();
                    foreach (var aff in rpt.Affix)
                    {
                        tmp += "{url:'" + tool.EncryptOrDecrypt(0, aff.DownloadURL.ToString(), "JXHLZQBS") +
                               "',name:'" + aff.FileName + "',tbno:'" + aff.TBNO + "'},";
                    }
                    if (tmp.Length != 0)
                    {
                        tmp = ",Affix:[" + tmp.Remove(tmp.Length - 1) + "]";
                    }
                }
                else
                {
                    tmp = ",Affix:[]";
                }

                #region 洪涝表

                if (rptType == "HL01")
                {
                    string strHL011 = "";
                    string strHL012 = "";
                    string strHL013 = "";
                    string strHL014 = "";
                    ArrayList arrXMMhl011 = new ArrayList();
                    ArrayList arrXMMhl012 = new ArrayList();
                    ArrayList arrXMMhl013 = new ArrayList();
                    ArrayList arrXMMhl014 = new ArrayList();

                    if (rpt.HL011.Count() <= 0)
                    {
                        strHL011 = "HL011:[]";
                    }
                    else
                    {//OrderBy(t=>t.UnitCode)
                        foreach (var hl01 in rpt.HL011.OrderBy(hl => hl.UnitCode).ThenBy(t => t.DataOrder))
                        {
                            LZHL011 xhl011 = ZH.ConvertHLToXMMHL<LZHL011, HL011>(hl01, limit);
                            //if ((limit == 2 || limit == 3)&& rpt.SourceType == 2)
                            //{
                            //    xhl011.SZFWX = "{Fake:'" + xhl011.SZFWX + "',Real:'',Details:[]}";
                            //}
                            arrXMMhl011.Add(xhl011);
                        }
                        strHL011 = "HL011:" + serializer.Serialize(arrXMMhl011);
                    }

                    if (rpt.HL012.Count() <= 0)
                    {
                        strHL012 = "HL012:[]";
                    }
                    else
                    {
                        foreach (var hl02 in rpt.HL012.OrderBy(hl => hl.UnitCode).ThenBy(t => t.DataOrder))
                        {
                            FBLZHL012 xhl012 = ZH.ConvertHLToXMMHL<FBLZHL012, HL012>(hl02, limit);
                            xhl012.Checked = "false";
                            arrXMMhl012.Add(xhl012);
                        }
                        strHL012 = "HL012:" + serializer.Serialize(arrXMMhl012);
                    }

                    if (rpt.HL013.Count() <= 0)
                    {
                        strHL013 = "HL013:[]";
                    }
                    else
                    {
                        foreach (var hl03 in rpt.HL013.OrderBy(hl => hl.UnitCode).ThenBy(t => t.DataOrder))
                        {
                            if (hl03.DW == "合计")
                            {
                                LZHL013 xhl013 = ZH.ConvertHLToXMMHL<LZHL013, HL013>(hl03, limit);
                                arrXMMhl013.Add(xhl013);
                            }
                            else
                            {
                                FBLZHL013 xhl013 = ZH.ConvertHLToXMMHL<FBLZHL013, HL013>(hl03, limit);
                                xhl013.Checked = "false";
                                arrXMMhl013.Add(xhl013);
                            }
                        }
                        strHL013 = "HL013:" + serializer.Serialize(arrXMMhl013);
                    }

                    if (rpt.HL014.Count() <= 0)
                    {
                        strHL014 = "HL014:[]";
                    }
                    else
                    {
                        foreach (var hl04 in rpt.HL014.OrderBy(hl => hl.UnitCode).ThenBy(t => t.DataOrder))
                        {
                            LZHL014 xhl014 = ZH.ConvertHLToXMMHL<LZHL014, HL014>(hl04, limit);
                            arrXMMhl014.Add(xhl014);
                        }
                        strHL014 = "HL014:" + serializer.Serialize(arrXMMhl014);
                    }
                    jsonStr = reportInfo + "," + strHL011 + "," + strHL012 + "," + strHL013 + "," + strHL014 + tmp;
                }
                #endregion
                #region 蓄水表

                else if (rptType == "HP01")
                {
                    string strHP011 = "";
                    string strHP012 = "";
                    ArrayList arrXMMhp011 = new ArrayList();
                    ArrayList arrXMMhp012_large = new ArrayList();
                    ArrayList arrXMMhp012_middle = new ArrayList();
                    if (rpt.HP011.Count() <= 0)
                    {
                        strHP011 = "HP011:[]";
                    }
                    else
                    {
                        foreach (var hp01 in rpt.HP011.OrderBy(hl => hl.UNITCODE).ThenBy(t => t.DATAORDER))
                        {
                            LZHP011 xhp011 = ZH.ConvertHLToXMMHL<LZHP011, HP011>(hp01, limit);
                            arrXMMhp011.Add(xhp011);
                        }
                        strHP011 = "HP011:" + serializer.Serialize(arrXMMhp011);
                    }

                    if (rpt.HP012.Count() <= 0)
                    {
                        strHP012 = "HP012:{Real:{Large:[],Middle:[]}}";
                    }
                    else
                    {
                        foreach (var hp02 in rpt.HP012.OrderBy(hl => hl.UNITCODE).ThenBy(t => t.DATAORDER))
                        {
                            LZHP012 xhp012 = ZH.ConvertHLToXMMHL<LZHP012, HP012>(hp02, limit);
                            if (hp02.DISTRIBUTERATE == "1")
                            {
                                arrXMMhp012_large.Add(xhp012);
                            }
                            else if (hp02.DISTRIBUTERATE == "2")
                            {
                                arrXMMhp012_middle.Add(xhp012);
                            }
                        }
                        strHP012 = "HP012:{Real:{Large:" + serializer.Serialize(arrXMMhp012_large) + ",Middle:" + serializer.Serialize(arrXMMhp012_middle) + "}}";
                    }
                    jsonStr = reportInfo + "," + strHP012 + "," + strHP011 + tmp;
                }

                #endregion
            }
            else
            {
                if (rptType == "HL01")
                {
                    jsonStr = "ReportTitle:[],HL011:[],HL012:[],HL013:[],HL014:[],Affix:[]";
                }
                else if (rptType == "HP01")
                {
                    jsonStr = "ReportTitle:[],HP011:[],HP012:{Real:{Large:[],Middle:[]}},Affix:[]";
                }
            }
            busEntity.Dispose();
            return jsonStr;
        }

        public string ViewReportFormInfo(int limit, int pageNO, string rptType, bool is_NMG)
        {
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            XMMZHClass ZH = new XMMZHClass();
            JavaScriptSerializer serializer = new JavaScriptSerializer(); //创建一个序列化对象
            var rpts = from rpt in busEntity.ReportTitle
                       where rpt.PageNO == pageNO
                       select rpt;
            string response = "";
            string tmp = "";
            if (rpts.Count() > 0)
            {
                var rpt = rpts.First();
                response = "ReportTitle:" + serializer.Serialize(ZH.ZHQTReportTitle(rpt));

                switch (rptType)
                {
                    case "SH01":
                        response += ",SH011:" +
                                    serializer.Serialize(
                                        busEntity.SH011.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH011>());
                        break;
                    case "SH02":
                        response += ",SH021:" +
                                    serializer.Serialize(
                                        busEntity.SH021.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH021>());
                        break;
                    case "SH03":
                        response += ",SH031:" +
                                    serializer.Serialize(
                                        busEntity.SH031.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH031>());
                        response += ",SH032:" +
                                    serializer.Serialize(
                                        busEntity.SH032.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH032>());
                        response += ",SH033:" +
                                    serializer.Serialize(
                                        busEntity.SH033.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH033>());
                        response += ",SH034:" +
                                    serializer.Serialize(
                                        busEntity.SH034.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH034>());
                        response += ",SH035:" +
                                    serializer.Serialize(
                                        busEntity.SH035.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH035>());
                        response += ",SH036:" +
                                    serializer.Serialize(
                                        busEntity.SH036.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH036>());
                        response += ",SH037:" +
                                    serializer.Serialize(
                                        busEntity.SH037.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH037>());
                        response += ",SH038:" +
                                    serializer.Serialize(
                                        busEntity.SH038.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH038>());
                        break;
                    case "SH04":
                        if (DateTime.Parse(rpt.EndDateTime.ToString()) > DateTime.Parse("2016-01-01"))
                        {
                            response += ",SH046:" +
                                        serializer.Serialize(
                                            busEntity.SH046.Where(t => t.PageNO == pageNO)
                                                .OrderBy(t => t.DataOrder)
                                                .ToList<SH046>());
                        }
                        else
                        {
                            response += ",SH041:" +
                                    serializer.Serialize(
                                        busEntity.SH041.Where(t => t.PageNO == pageNO)
                                            .OrderBy(t => t.DataOrder)
                                            .ToList<SH041>());
                            response += ",SH042:" +
                                        serializer.Serialize(
                                            busEntity.SH042.Where(t => t.PageNO == pageNO)
                                                .OrderBy(t => t.DataOrder)
                                                .ToList<SH042>());
                            response += ",SH043:" +
                                        serializer.Serialize(
                                            busEntity.SH043.Where(t => t.PageNO == pageNO)
                                                .OrderBy(t => t.DataOrder)
                                                .ToList<SH043>());
                            response += ",SH044:" +
                                        serializer.Serialize(
                                            busEntity.SH044.Where(t => t.PageNO == pageNO)
                                                .OrderBy(t => t.DataOrder)
                                                .ToList<SH044>());
                            response += ",SH045:" +
                                        serializer.Serialize(
                                            busEntity.SH045.Where(t => t.PageNO == pageNO)
                                                .OrderBy(t => t.DataOrder)
                                                .ToList<SH045>());
                        }
                        break;
                    case "SH05":
                        response += ",SH051:" +
                                  serializer.Serialize(
                                      busEntity.SH051.Where(t => t.PageNO == pageNO)
                                          .OrderBy(t => t.DataOrder)
                                          .ToList<SH051>());
                        break;
                }
            }
            else
            {
                response = "{}";
            }

            busEntity.Dispose();

            return response;
        }

        /// <summary>打开内蒙蓄水
        /// </summary>
        /// <param name="pageNO"></param>
        /// <param name="unitCode"></param>
        /// <param name="curLimit">当前登录单位级别</param>
        /// <returns></returns>
        public string ViewReportFormInfo(int pageNO, string unitCode, int curLimit)
        {
            string jsonStr = "";
            BusinessEntities busEntity = Persistence.GetDbEntities(2);
            XMMZHClass ZH = new XMMZHClass();
            JavaScriptSerializer serializer = new JavaScriptSerializer(); //创建一个序列化对象
            var rpts = from rpt in busEntity.ReportTitle
                       where rpt.PageNO == pageNO
                       select rpt;
            string reportInfo = "";
            string tmp = "";
            if (rpts.Count() > 0)
            {
                var rpt = rpts.First();
                reportInfo = "ReportTitle:" + serializer.Serialize(ZH.ZHQTReportTitle(rpt));
                if (rpt.Affix.Count() > 0)
                {
                    Tools tool = new Tools();
                    foreach (var aff in rpt.Affix)
                    {
                        tmp += "{url:'" + tool.EncryptOrDecrypt(0, aff.DownloadURL.ToString(), "JXHLZQBS") +
                               "',name:'" + aff.FileName + "',tbno:'" + aff.TBNO + "'},";
                    }
                    if (tmp.Length != 0)
                    {
                        tmp = ",Affix:[" + tmp.Remove(tmp.Length - 1) + "]";
                    }
                }
                else
                {
                    tmp = ",Affix:[]";
                }
                string strNP011 = "";
                ArrayList arrLZnp011 = new ArrayList();
                if (rpt.NP011.Count() <= 0)
                {
                    strNP011 = "NP011:[]";
                }
                else
                {
                    int[] limitSub = { 4, 4, 6 };
                    if (curLimit == 2)
                    {
                        unitCode = "15110000";//呼和浩特
                    }
                    string tempCode = unitCode.Substring(0, limitSub[curLimit - 2]);
                    foreach (var hp01 in rpt.NP011.Where(t => t.RSCode != null && t.RSCode.StartsWith(tempCode)).OrderBy(t => t.UnitCode).ThenBy(hl => hl.DataOrder))
                    {
                        LZNP011 lznp011 = ZH.ConvertHLToXMMHL<LZNP011, NP011>(hp01);
                        arrLZnp011.Add(lznp011);
                    }
                    strNP011 = "NP011:" + serializer.Serialize(arrLZnp011);
                }
                jsonStr = reportInfo + "," + strNP011 + tmp;

                if (curLimit == 2 || curLimit == 3)
                {
                    FXDICTEntities fxdict = new FXDICTEntities();
                    unitCode = curLimit == 3 ? unitCode : unitCode.Substring(0, 2) + "000000";
                    var hj = fxdict.TB62_NMReservoir.Where(t => t.UnitCode == unitCode).SingleOrDefault();
                    if (hj != null)
                    {
                        unitCode = curLimit == 3 ? unitCode.Substring(0, 4) : unitCode.Substring(0, 2);
                        decimal? dqxsl = rpt.NP011.Where(t => t.UnitCode.StartsWith(unitCode)).Sum(t => t.DQXSL);
                        jsonStr += ",HJ:{ ZKR:" + (hj.ZKR == null ? 0 : hj.ZKR) + ",SKR:" +
                                   (hj.SKR == null ? "0" : hj.SKR) + ",RSName:'" +
                                   (hj.RSName == null ? "未知" : hj.RSName) + "',UnitName:" +
                                   (hj.UnitName == null ? "0" : hj.UnitName) + ",UnitCode:" +
                                   (hj.UnitCode == null ? "未知" : hj.UnitCode) + ",DQXSL:'" +
                                   (dqxsl > 0 ? Convert.ToDouble(dqxsl).ToString("0.000") : "0")
                                   + "'}";
                    }
                    fxdict.Dispose();
                }
            }
            else
            {
                jsonStr = "ReportTitle:[],NP011:[],Affix:[]";
            }

            busEntity.Dispose();

            return jsonStr;
        }



        public string ViewNMReportFormInfo(int pageNO, string unitCode)
        {
            string jsonStr = "";
            BusinessEntities busEntity = Persistence.GetDbEntities(2);
            XMMZHClass ZH = new XMMZHClass();
            JavaScriptSerializer serializer = new JavaScriptSerializer(); //创建一个序列化对象
            var rpts = from rpt in busEntity.ReportTitle
                       where rpt.PageNO == pageNO
                       select rpt;
            if (rpts.Count() > 0)
            {
                var rpt = rpts.First();
                string strNP011 = "";
                ArrayList arrLZnp011 = new ArrayList();
                if (rpt.NP011.Count() <= 0)
                {
                    strNP011 = "NP011:[]";
                }
                else
                {
                    string tempCode = unitCode.Substring(0, 4);
                    foreach (var hp01 in rpt.NP011.Where(t => t.RSCode != null && t.RSCode.StartsWith(tempCode)).OrderBy(hl => hl.DataOrder))
                    {
                        LZNP011 lznp011 = ZH.ConvertHLToXMMHL<LZNP011, NP011>(hp01);
                        arrLZnp011.Add(lznp011);
                    }
                    strNP011 = "NP011:" + serializer.Serialize(arrLZnp011);
                }
                jsonStr = strNP011;
            }
            else
            {
                jsonStr = "NP011:[]";
            }

            busEntity.Dispose();

            return jsonStr;
        }
        public string GetTab6Max(string pageNoList, BusinessEntities busEntities)
        {
            string result = "";
            var hl013s = busEntities.HL013.Where("it.PageNO in {" + pageNoList + "}").Where(t => t.DW == "合计").ToList();
            foreach (var hl013 in hl013s)
            {
                result += "{GCYMLS:" + hl013.GCYMLS + ",ZYZJZDSS:" + hl013.ZYZJZDSS + ",PageNO:" +
                          hl013.PageNO + "},";
            }
            if (result != "")
            {
                result = "[" + result.Remove(result.Length - 1) + "]";
            }
            else
            {
                result = "[]";
            }
            return result;
        }

        public string GetSHSourceReportList(int pageno, int limit)
        {
            string response = "SourceReport:";
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);

            try
            {
                
                var aggaccs = (from agg in busEntity.AggAccRecord
                               where agg.PageNo == pageno
                               select new
                               {
                                   agg.SPageNO
                               }).ToList();

                if (aggaccs.Any())
                {
                    int?[] pagenos = new int?[aggaccs.Count];
                    for (int i = 0; i < aggaccs.Count; i++)
                    {
                        pagenos[i] = aggaccs[i].SPageNO;
                    }

                    busEntity = Persistence.GetDbEntities(limit + 1);
                    var rpts = (from rpt in busEntity.ReportTitle
                                where pagenos.Contains(rpt.PageNO)
                                select new
                                {
                                    rpt.PageNO,
                                    rpt.UnitName,
                                    rpt.WriterTime,
                                    rpt.UnitCode,
                                }).ToList();

                    response += "[";
                    DateTime dt = new DateTime();
                    string str = "";
                    foreach (var rpt in rpts)
                    {
                        str = rpt.WriterTime.ToString();
                        dt = DateTime.Parse(str);
                        response += "{id:" + rpt.PageNO + ",Name:'[" + rpt.UnitName + "][已报送]" + dt.ToString("MM月dd日 HH:mm") + "',UnitCode:" + rpt.UnitCode + "},";
                    }
                    response = response.Remove(response.Length - 1) + "]";
                }
                else
                {

                    response += "[]";
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            busEntity.Dispose();

            return response;
        }

        //获取差指表数据
        public string GetDeltaReport(int pageno, int limit)
        {
            string response = "";
            BusinessEntities db_context = Persistence.GetDbEntities(limit);

            try
            {
                var spagenos = db_context.AggAccRecord.Where(t => t.PageNo == pageno).Select(t => t.SPageNO).ToList();
                db_context = Persistence.GetDbEntities(limit + 1); //下级库
                var list =
                    db_context.ReportTitle.Where(t => spagenos.Contains(t.PageNO) && t.SourceType == 6)
                        .Select(t => t.PageNO)
                        .ToList();
                string pagenos = string.Join(",", list);

                JsonSerializerSettings setting = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                var hl011_list =
                    db_context.ExecuteStoreQuery<HL011>("select * from hl011 where pageno in(" + pagenos + ")").ToList();
                response = "HL011:" + JsonConvert.SerializeObject(hl011_list, setting) + ",";

                var hl012_list =
                    db_context.ExecuteStoreQuery<HL012>("select * from hl012 where pageno in(" + pagenos + ")").ToList();
                response += "HL012:" + JsonConvert.SerializeObject(hl012_list, setting) + ",";

                var hl013_list =
                    db_context.ExecuteStoreQuery<HL013>("select * from hl013 where pageno in(" + pagenos + ")").ToList();
                response += "HL013:" + JsonConvert.SerializeObject(hl013_list, setting) + ",";

                var hl014_list =
                    db_context.ExecuteStoreQuery<HL014>("select * from hl014 where pageno in(" + pagenos + ")").ToList();
                response += "HL014:" + JsonConvert.SerializeObject(hl014_list, setting);

                response = "Delta:{" + response + "}";
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            db_context.Dispose();

            return response;
        }

        /// <summary>
        /// 获取某张表的所有
        /// </summary>
        /// <param name="pageNO"></param>
        /// <param name="limit"></param>
        /// <param name="typeLimit">1查看本级，0查看下级</param>
        /// <param name="UnitName"></param>
        /// <returns></returns>
        public string GetSourceReportList(int pageNO, int limit, int typeLimit, string UnitName, string unitCode)
        {
            string jsonStr = "";
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            var aggaccs = from agg in busEntity.AggAccRecord
                          where agg.PageNo == pageNO
                          select new
                          {
                              agg.SPageNO,
                              agg.UnitCode
                          };
            string pageNoList = "";
            foreach (var agg in aggaccs)
            {
                pageNoList += agg.SPageNO + ",";
            }
            if (pageNoList != "")
            {
                pageNoList = pageNoList.Remove(pageNoList.Length - 1);
            }
            if (limit < 5)
            {
                if (typeLimit == 0)//查看下级
                {
                    limit = limit + 1;
                }
            }
            string[] pageNos = new string[3] { "", "", "" };
            int j = 0;
            object[] obj = null;
            SortedList slist = null;
            bool repeat = false;
            string diffRptData = "";
            string diffPageNOs = "";
            string content = "";
            string unit = "";
            ArrayList Container = new ArrayList();
            changeEntity = Persistence.GetDbEntities(limit);//根据查看类型（是否本级）改变级别
            foreach (var aggobj in aggaccs)
            {
                string types = "0,1,2,6";// //SourceType in(0,1,2,6) 源表可能是差值表
                var rpts = changeEntity.ReportTitle.Where("it.SourceType in {" + types + "}").AsQueryable();
                rpts = rpts.Where(t => t.UnitCode == aggobj.UnitCode && t.PageNO == aggobj.SPageNO);
                var sRpts = from srpt in rpts
                            select new
                            {
                                srpt.PageNO,
                                srpt.StartDateTime,
                                srpt.EndDateTime,
                                srpt.SourceType,
                                srpt.UnitCode,
                                srpt.SendOperType,
                                srpt.State
                            };

                //sql = "select Id,StartDateTime,EndDateTime,SourceType,UnitCode from ReportTitle where SourceType in(0,1,2,6) and UnitCode='" + obj[1] + "' and Id=" + obj[0];  //SourceType in(0,1,2,6) 源表可能是差值表
                int i = 0;

                foreach (var objSource in sRpts)
                {
                    if (objSource.SourceType == 6)//是差值表
                    {
                        diffPageNOs += objSource.PageNO;
                    }
                    //if (i == 0)
                    //{
                    //    jsonStr += "{id:'" + objSource.PageNO + "',StartDate:'" + Convert.ToDateTime(objSource.StartDateTime).Date.ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo)
                    //            + "',EndDate:'" + Convert.ToDateTime(objSource.EndDateTime).Date.ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo) + "',SourceType:'"
                    //            + Convert.ToInt32(objSource.SourceType) + "',UnitCode:'" + objSource.UnitCode + "'}";
                    //}
                    //else
                    //{
                    content += "{id:'" + objSource.PageNO + "',StartDate:'" + Convert.ToDateTime(objSource.StartDateTime).Date.ToString("MM月dd日", DateTimeFormatInfo.InvariantInfo)
                            + "',EndDate:'" + Convert.ToDateTime(objSource.EndDateTime).Date.ToString("dd日", DateTimeFormatInfo.InvariantInfo) + "',SourceType:'" + Convert.ToInt32(objSource.SourceType) + "',UnitCode:'" + objSource.UnitCode + "',SendOperType:'" + objSource.SendOperType + "',State:'" + Convert.ToInt32(objSource.State) + "'},";
                    //}
                    i++;
                    # region  张建军添加的代码用于获取累计表的源表中的县的单位代码
                    if (limit == 3) //省级汇总，市级累计查市库
                    {
                        var hl011s = changeEntity.HL011.Where(t => t.PageNO == aggobj.SPageNO && t.DW != "合计");
                        foreach (var hl1obj in hl011s)
                        {
                            unit += "{UnitCode:'" + hl1obj.UnitCode + "'},";
                        }
                    }
                    else if (limit == 2)//省级累计
                    {
                        j = Convert.ToInt32(objSource.SourceType);
                        j = j == 6 ? 0 : j;
                        if (pageNos[j] == "")
                        {
                            pageNos[j] = objSource.PageNO.ToString();
                        }
                        else
                        {
                            pageNos[j] += "," + objSource.PageNO.ToString();
                        }
                    }
                    #endregion
                }
            }

            if (content.Length > 0)
            {
                content = content.Remove(content.Length - 1);
            }
            if (unit.Length > 0)
            {
                unit = unit.Remove(unit.Length - 1);
            }

            #region //查出差值数据***********************************************************
            if (diffPageNOs.Length > 0)
            {
                LogicProcessingClass.XMMZH.XMMZHClass ZH = new LogicProcessingClass.XMMZH.XMMZHClass();//数量级转换对象
                System.Web.Script.Serialization.JavaScriptSerializer JSS = new System.Web.Script.Serialization.JavaScriptSerializer();
                diffPageNOs = diffPageNOs.Remove(diffPageNOs.Length - 1);
                int level = 0;
                if (typeLimit != 1)  //汇总差值表
                {
                    level = limit - 1;
                }
                else  //累计差值表
                {
                    level = limit;
                }

                var hl011s = changeEntity.HL011.Where("it.PageNO in {" + diffPageNOs + "}").AsQueryable();
                if (hl011s.Count() > 0)
                {
                    Container.Clear();
                    for (int i = 0; i < hl011s.Count(); i++)
                    {
                        LZHL011 hl011 = ZH.ConvertHLToXMMHL<LZHL011, HL011>(hl011s.ToList<HL011>()[i], level);
                        Container.Add(hl011);
                    }
                    diffRptData += "HL011:" + JSS.Serialize(Container) + ",";
                }


                //var hl012s = changeEntity.HL012.Where("it.PageNO in {" + DiffPageNOs + "}").AsQueryable();
                //if (hl012s.Count() > 0)
                //{
                //    Container.Clear();
                //    for (int i = 0; i < hl012s.Count(); i++)
                //    {
                //        XMMHL012 hl012 = ZH.ConvertHLToXMMHL<XMMHL012, HL012>(hl012s.ToList<HL012>()[i], level);
                //        Container.Add(hl012);
                //    }
                //    DiffRptData += "HL012:" + JSS.Serialize(Container) + ",";
                //}


                //var hl013s = changeEntity.HL013.Where("it.PageNO in {" + DiffPageNOs + "}").AsQueryable();
                //if (hl013s.Count() > 0)
                //{
                //    Container.Clear();
                //    for (int i = 0; i < hl013s.Count(); i++)
                //    {
                //        XMMHL013 hl013 = ZH.ConvertHLToXMMHL<XMMHL013, HL013>(hl013s.ToList<HL013>()[i], level);
                //        Container.Add(hl013);
                //    }
                //    DiffRptData += "HL013:" + JSS.Serialize(Container) + ",";
                //}


                var hl014s = changeEntity.HL014.Where("it.PageNO in {" + diffPageNOs + "}").AsQueryable();
                if (hl014s.Count() > 0)
                {
                    Container.Clear();
                    for (int i = 0; i < hl014s.Count(); i++)
                    {
                        LZHL014 hl014 = ZH.ConvertHLToXMMHL<LZHL014, HL014>(hl014s.ToList<HL014>()[i], level);
                        Container.Add(hl014);
                    }
                    diffRptData += "HL014:" + JSS.Serialize(Container) + ",";
                }
                if (diffRptData.Length > 0)
                {
                    diffRptData = "DiffRptData:{" + diffRptData.Remove(diffRptData.Length - 1) + "}";
                }
            }
            #endregion

            # region

            string other = "";
            if (limit == 2 && UnitName != "BJ")  //省级累计精确的SZFW数据
            {
                j = 1;
                int k = 0;
                pageNos[2] = (pageNos[1] == "" ? (pageNos[2] == "" ? "" : pageNos[2]) : (pageNos[2] == "" ? pageNos[1] : pageNos[1] + "," + pageNos[2]));
                pageNos[1] = "";
                while (pageNos[2] != "") //累计源表:累计表
                {
                    var aggs = changeEntity.AggAccRecord.Where("it.PageNO in {" + pageNos[2] + "}").AsQueryable();
                    var newAggs = from agg in aggs
                                  select new
                                  {
                                      agg.OperateType,
                                      agg.SPageNO
                                  };
                    var rpts = changeEntity.ReportTitle.Where("it.PageNO in {" + pageNos[2] + "}").AsQueryable();
                    var newRpts = from rpt in rpts
                                  select new
                                  {
                                      rpt.SourceType,
                                      rpt.PageNO
                                  };
                    ArrayList arrList = new ArrayList();
                    object[] arrobj = { };
                    if (j % 2 != 0)  //查累计、汇总源表
                    {
                        foreach (var aobj in newAggs)
                        {
                            arrobj = new object[2];
                            arrobj[0] = aobj.OperateType;
                            arrobj[1] = aobj.SPageNO;
                            arrList.Add(arrobj);
                        }
                    }
                    else  //查源表的来源类型
                    {
                        foreach (var robj in newRpts)
                        {
                            arrobj = new object[2];
                            arrobj[0] = robj.SourceType;
                            arrobj[1] = robj.PageNO;
                            arrList.Add(arrobj);
                        }
                    }
                    pageNos[2] = "";
                    for (int i = 0; i < arrList.Count; i++)
                    {
                        obj = (object[])arrList[i];
                        switch (Convert.ToInt32(obj[0]))
                        {
                            case 0:  //录入
                                k = 0;
                                break;
                            case 1:  //汇总

                                if (j % 2 != 0)
                                {
                                    k = 1;
                                }
                                else
                                {
                                    k = 2;
                                }
                                break;
                            case 2:  //累计
                                k = 2;
                                break;
                        }

                        if (pageNos[k] != "")
                        {
                            if (!repeat && pageNos[k].IndexOf(obj[1].ToString()) > 0)
                            {
                                repeat = true;
                            }
                            pageNos[k] += "," + obj[1].ToString();
                        }
                        else
                        {
                            pageNos[k] = obj[1].ToString();
                        }
                    }
                    j++;
                }
                if (pageNos[0] != "")  //录入表
                {
                    var hl011s = changeEntity.HL011.Where("it.PageNO in {" + pageNos[0] + "}").AsQueryable();
                    var newHl011s = from hl01 in hl011s
                                    where hl01.DW != "合计"
                                    group hl01 by hl01.UnitCode into h
                                    select new
                                    {
                                        h.Key,
                                        szfwxSum = h.Sum(t => t.SZFWX)
                                    };
                    foreach (var hobj in newHl011s)
                    {
                        other += "{UnitCode:'" + hobj.Key + "',Total:'" + Convert.ToInt32(hobj.szfwxSum) + "'},";
                    }
                }

                if (pageNos[1] != "")
                {
                    ChangeDBConnection(3);
                    slist = new CommonFunction().RecordStrRepeatTimes(pageNos[1]);
                    for (int i = 0; i < slist.Count; i++)
                    {
                        var hl011s = changeEntity.HL011.Where("it.PageNO in {" + slist[slist.GetKey(i)] + "}").AsQueryable();
                        var newHl011s = from hl01 in hl011s
                                        where hl01.DW != "合计"
                                        group hl01 by hl01.UnitCode into h
                                        select new
                                        {
                                            h.Key,
                                            szfwxSum = h.Sum(t => t.SZFWX)
                                        };
                        //sql = "select UnitCode,sum(SZFWX)*" + slist.GetKey(i) + " from HL011 where PageNo in(" + slist[slist.GetKey(i)] + ") and dw<>'合计' group by UnitCode ";
                        foreach (var hobj in newHl011s)
                        {
                            other += "{UnitCode:'" + hobj.Key + "',Total:'" + Convert.ToInt32(hobj.szfwxSum * Convert.ToInt32(slist.GetKey(i))) + "'},";
                        }
                    }
                    ChangeDBConnection(2);
                }
                if (other.Length > 0)
                {
                    other = ",Other:[" + other.Remove(other.Length - 1) + "]";
                }
                else
                {
                    other = ",Other:[]";
                }
            }
            //if (limit == 2)
            //{
            //    content = "Content:[" + content + "]";
            //    unit = "Unit:[" + unit + "]";
            //    jsonStr = content + "," + unit + other;
            //    jsonStr = "SourceReport:{" + jsonStr + "}";
            //}
            //else
            //{
            jsonStr = content;
            if (content.Length > 0 && unit.Length > 0)
            {
                jsonStr += ",";
            }
            jsonStr += unit;
            jsonStr = "SourceReport:[" + jsonStr + "]";
            //}
            #endregion


            if (diffRptData.Length > 0)
            {
                jsonStr += "," + diffRptData;
            }
            if (typeLimit != 0)  //typeLimit != 0
            {
                //查看数据库里是否存在附件
                var affs = changeEntity.Affix.Where(t => t.PageNO == pageNO);
                string tmp = "";
                if (affs.Count() > 0)
                {
                    Tools tool = new Tools();
                    foreach (var aff in affs)
                    {
                        tmp += "{url:'" + tool.EncryptOrDecrypt(0, aff.DownloadURL.ToString(), "JXHLZQBS") + "',name:'" + aff.FileName + "',tbno:'" + aff.TBNO + "'},";
                    }
                    if (tmp.Length != 0)
                    {
                        tmp = ",Affix:[" + tmp.Remove(tmp.Length - 1) + "]";
                    }
                }
                else
                {
                    tmp = ",Affix:[]";
                }
                jsonStr = jsonStr + tmp;
            }
            if (unitCode.StartsWith("33"))
            {
                if (unitCode.StartsWith("33"))
                {
                    string maxTemp = "[]";
                    if (pageNoList.Trim().Length > 0)  //pageNoList可能为空
                    {
                        maxTemp = GetTab6Max(pageNoList, changeEntity);
                    }
                    jsonStr = jsonStr + ",MAX:" + maxTemp;
                }
            }

            busEntity.Dispose();
            changeEntity.Dispose();

            return jsonStr;
        }

        public void ChangeDBConnection(int limit)
        {
            changeEntity = Persistence.GetDbEntities(limit);
        }

        /// <summary>
        /// 获取某报表的流域分配表，返回每个流域表的一个超链接，连接地址是“ViewUnderData.aspx?...”
        /// </summary>
        /// <param name="pageNO">报表页号</param>
        /// <param name="unitCode">单位编号</param>
        /// <returns>返回ViewUnderData.aspx?的一个超链接，没有数据，返回空字符串""</returns>
        public string RiverData(int pageNO, string unitCode)
        {
            string jsonStr = "";
            BusinessEntities prvEntity = Persistence.GetDbEntities(2);
            FXDICTEntities fxdict = Persistence.GetDbEntities();
            var riverRpts = from rpt in prvEntity.ReportTitle
                            where rpt.AssociatedPageNO == pageNO
                            select new
                            {
                                rpt.PageNO,
                                rpt.RPTType_Code
                            };
            foreach (var obj in riverRpts)
            {
                var tb11s = from tb11 in fxdict.TB11_RptType
                            where tb11.RptTypeCode == obj.RPTType_Code
                            select new
                            {
                                tb11.RptTypeName,
                                tb11.RvCode
                            };
                foreach (var tbobj in tb11s)
                {
                    string rPageNo = obj.PageNO.ToString(); //页号
                    string riverName = tbobj.RptTypeName.ToString(); //流域名
                    string riverCode = tbobj.RvCode; //流域代码
                    jsonStr += "<a href='ViewUnderData.aspx?DistrictCode=" + unitCode + "&ViewType=River&level=2&PageNo=" + rPageNo + "&RiverCode=" + riverCode + "' target='_blank' name='" + riverCode + "'>" + riverName + "</a>";
                }
            }
            prvEntity.Dispose();
            fxdict.Dispose();

            return jsonStr;
        }
    }
}
