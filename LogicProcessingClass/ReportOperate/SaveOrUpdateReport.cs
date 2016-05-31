using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Collections;
using EntityModel;
using System.Web.Script.Serialization;
using LogicProcessingClass.XMMZH;
using DBHelper;
using System.Data.Common;
using System.Transactions;
using LogicProcessingClass.AuxiliaryClass;
using System.Web;
using System.Data;
using System.Threading;

/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：SaveOrUpdateReport.cs
// 文件功能描述：保存或修改报表
// 创建标识：
// 修改标识：// 修改描述：
//-------------------------------------------------------------*/
namespace LogicProcessingClass.ReportOperate
{
    public class SaveOrUpdateReport
    {

        Entities getEntity = new Entities();
        private static object SaveLock = new object();
        //public static byte userCount = 0;
        HttpApplicationState App = System.Web.HttpContext.Current.Application;

        public string DelAffix(int pageno, string tbnos, string urls)
        {
            string result = "0";
            try
            {
                BusinessEntities entity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(int.Parse(HttpContext.Current.Request.Cookies["limit"].Value));
                ReportTitle rpt = new ReportTitle();
                rpt = entity.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault();
                string[] tbno = tbnos.Split(new char[] { ',' });
                string[] url = urls.Split(new char[] { ',' });
                for (int l = 0; l < url.Length; l++)
                {
                    int NO = int.Parse(tbno[l].ToString());

                    if (rpt.Affix.Where(t => t.TBNO == NO).Count() > 0)
                    {
                        var aff = rpt.Affix.Where(t => t.TBNO == NO).SingleOrDefault();
                        rpt.Affix.Remove(aff);
                        entity.SaveChanges();
                        aff.FileName =
                            System.Web.HttpContext.Current.Server.MapPath(
                                new LogicProcessingClass.Tools().EncryptOrDecrypt(1, url[l], "JXHLZQBS"));
                        if (System.IO.File.Exists(aff.FileName))
                        {
                            System.IO.File.Delete(aff.FileName);
                        }
                    }
                }
                result = "1";
            }
            catch (Exception ex)
            {
                result = "删除附件失败，" + ex.Message;
            }

            return result;
        }

        public string Save(int limit, string unitCode, string diffData, int isrivercode, string dataStr, string diffPageNOs)
        {
            string temp = "";
            int userCount = 0;
            int waitCount = 0;
            while (true)
            {
                if (App["userCount"] != null)
                {
                    userCount = Convert.ToInt32(App["userCount"]);
                }
                if (userCount == 0)
                {
                    userCount++;
                    App["userCount"] = userCount;
                    if (userCount == 1) //只有第一个保存
                    {
                        try
                        {
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            Hashtable report = serializer.Deserialize<Hashtable>(dataStr);
                            string reportTitle = serializer.Serialize(report["ReportTitle"]);
                            Hashtable ReportTitle = serializer.Deserialize<Hashtable>(reportTitle); //表头信息
                            Hashtable data = serializer.Deserialize<Hashtable>(dataStr);
                            string affix = null;
                            if (report["DelAffixURL"].ToString() != "" && report["DelAffixTBNO"].ToString() != "")
                            {
                                affix = report["DelAffixURL"].ToString().Replace("..", "~") + ";" +
                                        report["DelAffixTBNO"].ToString();
                            }
                            if (unitCode.StartsWith("15") && ReportTitle["ORD_Code"].ToString() == "NP01") //内蒙古蓄水表
                            {
                                temp = NMNPSaveUpdateReport(int.Parse(ReportTitle["PageNO"].ToString()), data, affix,
                                    limit, unitCode);
                            }
                            else //非内蒙古蓄水
                            {
                                #region

                                ReportHelpClass rptHelp = new ReportHelpClass();
                                string aggAcc = serializer.Serialize(report["SourceReport"]);
                                int pageNO = rptHelp.FindMaxPageNO(limit) + 1; //找到最大的页号并加一
                                if (dataStr == "")
                                {
                                    temp = "错误消息："; //不成功
                                }
                                else
                                {
                                    string isNew = ReportTitle["PageNO"].ToString() == "0" ? "true" : "false";
                                    if (isNew == "false" && ReportTitle["State"] != null &&
                                        ReportTitle["State"].ToString() != "3")
                                        //如果是修改，那么加1不需要，
                                    {
                                        //HttpApplicationState App = System.Web.HttpContext.Current.Application;
                                        App[limit.ToString() + "_maxPageNO"] =
                                            Convert.ToInt32(App[limit.ToString() + "_maxPageNO"]) - 1;
                                    }
                                    ///差值表diffData中如果有正确的PageNO参数，则是修改，否则是新建
                                    if (ReportTitle["PageNO"].ToString() == "0")
                                    {
                                        ReportTitle["State"] = 0;
                                    }

                                    #region----保存差值表----差值数据需要在前台传入数据之后进行调试，现阶段没有数据进行测试调试------------------

                                    string sourceRptResult = "";
                                    if (diffData.Trim().Length != 0) //不存在差值数据
                                    {
                                        DifferentialReport DiffReport = new DifferentialReport();
                                        if (Convert.ToInt32(ReportTitle["SourceType"].ToString()) == 2) //如果是累计差值表
                                        {
                                            int sPageNO = 0;
                                            sPageNO = pageNO; //新建时用到，如果diffData中正确的传入差值表PageNO，则不会使用sPageNO
                                            sourceRptResult = DiffReport.SaveOrUpdate(limit, unitCode, diffData,
                                                ReportTitle,
                                                sPageNO,
                                                ref aggAcc, ref dataStr);
                                        }
                                        else if (Convert.ToInt32(ReportTitle["SourceType"].ToString()) == 1) //汇总差值表
                                        {
                                            ArrayList diffRptList = DiffReport.SplitDiffReportByString(diffData);
                                            string allDiffPageNOs = null;
                                            if (diffPageNOs != null)
                                            {
                                                allDiffPageNOs = diffPageNOs;
                                            }
                                            sourceRptResult = DiffReport.SaveOrUpdate(limit, diffRptList, ReportTitle,
                                                ref dataStr,
                                                ref aggAcc, allDiffPageNOs);
                                        }
                                    }
                                    //-------------------------------------------------------------------------------------------------------

                                    #endregion

                                    //Hashtable data = serializer.Deserialize<Hashtable>(dataStr);
                                    //把HL011-HL014数据序列化成Hashtable类型
                                    Hashtable SReport =
                                        serializer.Deserialize<Hashtable>("{SourceReport:" + aggAcc + "}");
                                    //汇总的下级表的页号 

                                    #region 前台传入的流域数据处理

                                    ///********************************************多流域保存（如：湖南）*****/
                                    //string rates = report["RiverRates"] == null ? "" : report["RiverRates"].ToString();
                                    List<RiverInfo> rifs = new List<RiverInfo>();
                                    if (report["RiverRates"] != null)
                                    {
                                        //Hashtable hdata = serializer.Deserialize<Hashtable>(rates);
                                        Dictionary<string, object> dic =
                                            (Dictionary<string, object>) report["RiverRates"];
                                        foreach (string key in dic.Keys)
                                        {
                                            RiverInfo rif = new RiverInfo();
                                            rif.UnitCode = key.ToString();
                                            object[] str1 = serializer.ConvertToType<object[]>(dic[key]);
                                            //将里面的数据放入一个对象数组中去
                                            for (int i = 0; i < str1.Length; i++)
                                            {
                                                RiverDataInfo r = serializer.ConvertToType<RiverDataInfo>(str1[i]);
                                                //RiverDataInfo对象接收，数组里的对象
                                                rif.DRiverRate.Add(r.RiverCode.ToString(),
                                                    Convert.ToDouble(r.RiverData.ToString()));
                                            }
                                            rifs.Add(rif);
                                        }
                                        isrivercode = 1;
                                    }

                                    #endregion

                                    //string affix = null;
                                    //if (report["DelAffixURL"].ToString() != "" && report["DelAffixTBNO"].ToString() != "")
                                    //{
                                    //    affix = report["DelAffixURL"].ToString().Replace("..", "~") + ";" +
                                    //            report["DelAffixTBNO"].ToString();
                                    //}

                                    //pageNO = rptHelp.FindMaxPageNO(limit) + 1; //找到最大的页号并加一(如果差值表新建保存成功，那么PageNO需要重新取)
                                    if (isNew == "false")
                                    {
                                        pageNO = Convert.ToInt32(ReportTitle["PageNO"].ToString());
                                    }
                                    string rptType = ReportTitle["ORD_Code"].ToString();

                                    temp = SaveUpdateReport(limit, unitCode, pageNO, data, ReportTitle, SReport,
                                        isrivercode,
                                        isNew, rifs, affix, rptType);
                                    //temp += sourceRptResult;
                                    int errorIndex = temp.IndexOf("&"); //错误信息的位置索引,只搜索前面11个字符"saveFalse&&"
                                    int sourceRptErrorIndex = sourceRptResult.IndexOf("错误消息");
                                    //差值表错误信息的位置索引,只搜索前面11个字符"saveFalse&&"
                                    if (sourceRptErrorIndex != -1)
                                    {
                                        temp = "错误消息：差值表报表保存失败!" + "{" + sourceRptResult + "}";
                                    }
                                    if (errorIndex != -1)
                                    {
                                        temp = "错误消息：报表保存失败!" + temp;
                                    }
                                }
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            temp = ex.Message;
                        }
                        finally
                        {
                            userCount--;
                            App["userCount"] = userCount;
                        }
                        break;
                    }
                    else //其他的退出，但不 break,再循环到外层
                    {
                        userCount--;
                        App["userCount"] = userCount;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    if (waitCount == 10)//等待20秒之后还有操作在进行的话，强制结束
                    {
                        temp = "当前服务器访问人数过多，请稍后保存！";
                        App["userCount"] = null;
                        App["userCount"] = 0;
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                    waitCount++;

                }
            }

            return temp;
            //}
        }


        /// <summary>
        /// 保存或更新报表数据
        /// </summary>
        /// <param name="limit">级别</param>
        /// <param name="unitCode">单位代码</param>
        /// <param name="data">HL011-HL014，HP011-HP012的数据</param>
        /// <param name="reportTitle">reporttitle数据</param>
        /// <param name="SReport">汇总下级表的各表pageno（aggAcc）</param>
        /// <param name="isRiverCode">是否进行流域分配</param>
        /// <param name="isNew">是否新建true或false</param>
        /// <param name="rInfos">流域比例信息</param>
        /// <returns>修改后的表信息</returns>
        public string SaveUpdateReport(int limit, string unitCode, int pageNO, Hashtable data, Hashtable reportTitle, Hashtable SReport, int isRiverCode, string isNew, List<RiverInfo> rInfos, string affix, string rptType)
        {
            BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
            ViewReportForm viewReport = new ViewReportForm();
            ReportHelpClass rptHelp = new ReportHelpClass();
            string saveFlag = "";//保存失败，传出saveFalse&&,传出一个保存失败的标识
            ReportTitle rpt = null;
            bool CopyReport = false;

            int csOldPageNO = 0;//国家防总保存的CS数据库中的页号，如果为0，那么是新建

            if (isNew.ToUpper() == "FALSE" && reportTitle["State"].ToString() != "3")//isNew是新建还是修改还是Copy
            {
                rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
                bool cflag = CleanTableData(busEntity, pageNO);

                /****************************国家防总BS/CS两版本数据同步  start********************************************/
                if (limit == 0 && Convert.IsDBNull(rpt.CSPageNO))
                {
                    csOldPageNO = Convert.ToInt32(rpt.CSPageNO);//保存的CS版本报表对应的页号
                }
                /********************防总  end*************/

                if (!cflag)//清理失败 
                {
                    saveFlag = "清除旧数据失败！";
                    return saveFlag;
                }
            }
            else
            {
                rpt = new ReportTitle();
                rpt.CopyPageNO = 0;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            #region 对已经报送的报表进行复制
            if (reportTitle["State"] != null && reportTitle["State"].ToString() == "3")//已经报送，那么进行Copy
            {
                int maxPageNO = rptHelp.FindMaxPageNO(limit) + 1;
                ReportTitle zhubiaoRpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();//已经报送的主表
                zhubiaoRpt.CopyPageNO = maxPageNO;
                pageNO = maxPageNO;
                string[] tbno = null;
                if (affix != null)
                {
                    tbno = affix.Split(new char[] { ';' })[1].Split(new char[] { ',' });
                }
                foreach (var copyAff in zhubiaoRpt.Affix)
                {
                    Affix newAff = new Affix();
                    bool copy = true;
                    if (tbno != null)
                    {
                        for (int j = 0; j < tbno.Length; j++)
                        {
                            if (tbno[j] == copyAff.TBNO.ToString())   //判断需要删除的附件的TBNO是否等于当前的要复制的TBNO
                            {
                                copy = false;
                                break;
                            }
                        }
                    }
                    if (copy)
                    {
                        newAff = rptHelp.CloneEF<Affix>(copyAff);
                        newAff.PageNO = maxPageNO;
                        rpt.Affix.Add(newAff);
                    }
                    else
                    {
                        continue;
                    }
                }
                CopyReport = true;
            }
            #endregion

            //新建ReportTitle表
            DateTime dt = DateTime.Now;
            string staticUnitName = HttpContext.Current.Request.Cookies["unitname"].Value;
            rpt.Remark = reportTitle["Remark"] == null ? "" : reportTitle["Remark"].ToString();
            rpt.StatisticsPrincipal = reportTitle["StatisticsPrincipal"] == null ? "" : reportTitle["StatisticsPrincipal"].ToString();
            rpt.UnitPrincipal = reportTitle["UnitPrincipal"] == null ? "" : reportTitle["UnitPrincipal"].ToString();
            rpt.WriterName = reportTitle["WriterName"] == null ? "" : reportTitle["WriterName"].ToString();
            rpt.StatisticalCycType = Convert.ToInt32(reportTitle["StatisticalCycType"]);
            rpt.ORD_Code = reportTitle["ORD_Code"] == null ? "" : reportTitle["ORD_Code"].ToString();
            rpt.RPTType_Code = "XZ0";
            if (reportTitle["RPTType_Code"] != null && reportTitle["RPTType_Code"] != "")
            {
                rpt.RPTType_Code = reportTitle["RPTType_Code"].ToString();
            }
            rpt.UnitName = (reportTitle["UnitName"] == null || reportTitle["UnitName"].ToString() == "") ? HttpUtility.UrlDecode(staticUnitName, Encoding.GetEncoding("utf-8")) : reportTitle["UnitName"].ToString();
            rpt.UnitCode = unitCode;
            rpt.StartDateTime = Convert.ToDateTime(reportTitle["StartDateTime"].ToString());
            rpt.WriterTime = Convert.ToDateTime(reportTitle["WriterTime"].ToString());
            rpt.EndDateTime = Convert.ToDateTime(reportTitle["EndDateTime"].ToString());
            rpt.Del = 0;
            rpt.ReceiveState = 0;
            if (CopyReport && rpt.UnitCode.StartsWith("22")) //State = 4, 修改已报送，显示已保送，报表不保送
            {
                rpt.State = 4;
            }
            else {
                rpt.State = 0;
            }
            rpt.CloudPageNO = 0;
            rpt.SendOperType = 0;
            rpt.SourceType = reportTitle["SourceType"] == null ? 0 : Convert.ToInt32(reportTitle["SourceType"].ToString());
            rpt.SendTime = dt;
            rpt.ReceiveTime = dt;
            rpt.LastUpdateTime = dt;
            rpt.PageNO = pageNO;



            //新建修改HL011-HL014，HP011-HP012的表
            string DBname = "";
            XMMZHClass xmm = new XMMZHClass();
            #region 插入HL011-HL012，HP011-HP012的数据
            foreach (DictionaryEntry de in data)//data里有HL011-HL014，HP011-HP012的所有数据，遍历这些数据
            {
                DBname = de.Key.ToString();
                switch (DBname)//根据DBname不同执行不同的操作
                {
                    case "HL011"://往HL011表中插入数据
                        object[] str011 = serializer.ConvertToType<object[]>(de.Value);//反序列化HL011，将里面的数据放入一个对象数组中去
                        for (int i = 0; i < str011.Length; i++)//循环这个对象数组
                        {
                            HL011 hl011 = serializer.ConvertToType<HL011>(str011[i]);//HL011对象接收，数组里的对象
                            hl011.DataOrder = i;
                            hl011.PageNO = rpt.PageNO;//新加的
                            hl011.ReportTitle = rpt;//不确定是否需要
                            hl011 = xmm.ToSetHL<HL011>(hl011, limit);//数量级转换
                            rpt.HL011.Add(hl011);//将HL011数据放到reporttitle对象的HL011中去
                        }
                        break;
                    case "HL012":
                        object[] str012 = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < str012.Length; i++)
                        {
                            HL012 hl012 = serializer.ConvertToType<HL012>(str012[i]);
                            hl012.DataOrder = i;
                            hl012.PageNO = rpt.PageNO;//新加的
                            hl012.ReportTitle = rpt;
                            rpt.HL012.Add(hl012);
                        }
                        break;
                    case "HL013":
                        object[] str013 = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < str013.Length; i++)
                        {
                            HL013 hl013 = serializer.ConvertToType<HL013>(str013[i]);
                            hl013.DataOrder = i;
                            hl013.PageNO = rpt.PageNO;//新加的
                            hl013.ReportTitle = rpt;
                            hl013 = xmm.ToSetHL<HL013>(hl013, limit);
                            rpt.HL013.Add(hl013);
                        }
                        break;
                    case "HL014":
                        object[] str014 = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < str014.Length; i++)
                        {
                            HL014 hl014 = serializer.ConvertToType<HL014>(str014[i]);
                            hl014.DataOrder = i;
                            hl014.PageNO = rpt.PageNO;//新加的
                            hl014.ReportTitle = rpt;
                            hl014 = xmm.ToSetHL<HL014>(hl014, limit);
                            rpt.HL014.Add(hl014);
                        }
                        break;
                    case "HP011":
                        object[] strhp011 = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < strhp011.Length; i++)
                        {
                            HP011 hp011 = serializer.ConvertToType<HP011>(strhp011[i]);
                            hp011.DATAORDER = i;
                            hp011.PAGENO = rpt.PageNO;//新加的
                            hp011.ReportTitle = rpt;
                            hp011 = xmm.ToSetHL<HP011>(hp011, limit);
                            rpt.HP011.Add(hp011);
                        }
                        break;
                    case "HP012":
                        object[] strhp012 = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < strhp012.Length; i++)
                        {
                            HP012 hp012 = serializer.ConvertToType<HP012>(strhp012[i]);
                            hp012.DATAORDER = i;
                            hp012.PAGENO = rpt.PageNO;//新加的
                            hp012.ReportTitle = rpt;
                            hp012 = xmm.ToSetHL<HP012>(hp012, limit);
                            rpt.HP012.Add(hp012);
                        }
                        break;
                    /*case "ReportTitle":
                        Dictionary<string, string> rptTitle = serializer.ConvertToType< Dictionary<string, string>>(de.Value);
                        /*for (int i = 0; i < ; i++)
                        {
                            if(kvPair.Key)
                        }#1#
                        if (rptTitle["RPTType_Code"] != null && rptTitle["RPTType_Code"] != "")
                        {
                            rpt.RPTType_Code = rptTitle["RPTType_Code"];
                        }
                        break;*/
                    default:
                        break;
                }
            }
            #endregion
            //新建AggAccRecord表
            //if (rpt.SourceType == 1 || rpt.SourceType == 2)
            if ((rpt.SourceType == 1 || rpt.SourceType == 2) && SReport["SourceReport"] != null)
            {
                object[] AggAccs = serializer.ConvertToType<object[]>(SReport["SourceReport"]);
                for (int i = 0; i < AggAccs.Length; i++)
                {
                    //Dictionary<string, object> sourceReport = serializer.ConvertToType<Dictionary<string, object>>(AggAccs[i]);
                    Hashtable AggAcc = serializer.Deserialize<Hashtable>(serializer.Serialize(AggAccs[i]));
                    AggAccRecord Agg = new AggAccRecord();
                    Agg.ORD_Code = reportTitle["ORD_Code"].ToString();
                    //Agg.SPageNO = Convert.ToInt32(sourceReport.First().Value);
                    Agg.SPageNO = Convert.ToInt32(AggAcc["id"]);
                    Agg.PageNo = pageNO;
                    //Agg.UnitCode = Convert.ToString(sourceReport.Last().Value);
                    Agg.UnitCode = AggAcc["UnitCode"].ToString();
                    Agg.DBType = 0;
                    Agg.OperateType = rpt.SourceType;
                    rpt.AggAccRecord.Add(Agg);
                }
            }
            bool successRiver = false;//定义一个分配流域是否成功的变量
            bool flag = false;//定义一个Data数据是否成功提交的标识
            try
            {
                //using (TransactionScope scope = new TransactionScope())//事务scope.Complete();
                //{
                /****************************国家防总BS/CS两版本数据同步  start********************************************/
                int csNewPageNO = 0;
                if (limit == 0)//只有国家防总才会有该功能
                {
                    CSReportOperate csRptOperate = new CSReportOperate();
                    csNewPageNO = csRptOperate.SaveRpt(limit, unitCode, csOldPageNO, data, reportTitle, SReport, affix);//返回CS数据库中的页号，保存在BS中
                }
                rpt.CSPageNO = csNewPageNO;//把CS版本中的页号保存到BS版本中

                /*************************************国家防总   end******************************************/

                if (isNew.ToUpper() == "FALSE" && reportTitle["State"].ToString() != "3")
                {
                    busEntity.SaveChanges();
                }
                else
                {
                    busEntity.ReportTitle.AddObject(rpt);
                    busEntity.SaveChanges();
                }
                //scope.Complete();
                flag = true;
                //}
            }
            //catch (OptimisticConcurrencyException ex)
            //{
            //    //以数据库为准
            //    busEntity.Refresh(RefreshMode.StoreWins, rpt);
            //    //最后再次更新数据库
            //    busEntity.SaveChanges();

            //}
            catch (Exception ex)
            {
                rpt = null;
                saveFlag = "saveFalse&&" + ex.Message + "&&" + ex.InnerException;
                App[limit.ToString() + "_maxPageNO"] = Convert.ToInt32(App[limit.ToString() + "_maxPageNO"]) - 1;
                return saveFlag;//保存行政表失败，那么肯定不要进行流域分配，
            }
            RiverDistribute riverDis = new RiverDistribute();
            if (isRiverCode == 1 && flag) //如果选择流域分配，并且之前的HL011-HL014,HP011-HP012数据提交成功
            {
                if (unitCode.StartsWith("15"))
                {
                    successRiver = riverDis.NMSaveRiverDistribute(pageNO, rInfos);
                }
                else if (unitCode.StartsWith("22"))
                {
                    if (rpt.HL011.Count > 0 || rpt.HL012.Count > 0 || rpt.HL013.Count > 0 || rpt.HL014.Count > 0) {

                        RiverDistribute rd = new RiverDistribute();
                        //删除之前的流域表
                        if (rd.IsRiverDistribute(rpt.PageNO))//是否已经进行流域分配了
                        {
                            rd.DeleteRiverReport(rpt.PageNO);
                        }
                        
                        successRiver = riverDis.SaveSingleRiverDistribute(rpt, "AA2");
                        successRiver = riverDis.SaveSingleRiverDistribute(rpt, "BB2");
                        successRiver = riverDis.SaveSingleRiverDistribute(rpt, "AB1");
                    }
                }
                else
                {
                    successRiver = riverDis.SaveRiverDistribute(pageNO, rInfos);
                }
            }

            //物理删除附件
            if (affix != null)
            {
                if (reportTitle["State"] != null && reportTitle["State"].ToString() == "3")//!= "3"改为==3 已经报送的报表不能删除附件
                {
                }
                else
                {
                    string[] url = affix.Split(new char[] { ';' })[0].Split(new char[] { ',' });
                    string[] tbno = affix.Split(new char[] { ';' })[1].Split(new char[] { ',' });
                    //using (TransactionScope scope = new TransactionScope())//事务scope.Complete();
                    //{
                    for (int l = 0; l < url.Length; l++)
                    {
                        int NO = int.Parse(tbno[l].ToString());
                        if (busEntity.Affix.Where(t => t.TBNO == NO).Count() > 0)
                        {
                            var aff = busEntity.Affix.Where(t => t.TBNO == NO).SingleOrDefault();
                            busEntity.Affix.DeleteObject(aff);
                            busEntity.SaveChanges();
                            aff.FileName = System.Web.HttpContext.Current.Server.MapPath(new Tools().EncryptOrDecrypt(1, url[l], "JXHLZQBS"));
                            if (System.IO.File.Exists(aff.FileName))
                            {
                                System.IO.File.Delete(aff.FileName);
                            }
                        }
                    }
                    //    scope.Complete();
                    //}
                }
            }

            if (isRiverCode == 1 && !successRiver)//如果选择流域分配，且保存流域信息失败
            {
                CleanTableData(busEntity, pageNO);
                saveFlag = "saveFalse&&流域分配错误！";
            }

            return saveFlag + pageNO;
        }

        /// <summary>
        /// 清空ReportTitle表对应的HL011-HL014,HP011-HP012,AggAccRecord的数据全部清理
        /// </summary>
        /// <param name="busEntity">对应级别的实体</param>
        /// <param name="pageNO">需要更新的报表编号</param>
        /// <returns>bool值</returns>
        public bool CleanTableData(BusinessEntities busEntity, int pageNO)
        {
            bool cleanFlag = false;
            //using (TransactionScope scope = new TransactionScope())
            //{
            var hl011s = busEntity.HL011.Where(t => t.PageNO == pageNO);
            var hl012s = busEntity.HL012.Where(t => t.PageNO == pageNO);
            var hl013s = busEntity.HL013.Where(t => t.PageNO == pageNO);
            var hl014s = busEntity.HL014.Where(t => t.PageNO == pageNO);
            var aggs = busEntity.AggAccRecord.Where(t => t.PageNo == pageNO);
            foreach (var hl011 in hl011s)
            {
                busEntity.HL011.DeleteObject(hl011);
            }
            foreach (var hl012 in hl012s)
            {
                busEntity.HL012.DeleteObject(hl012);
            }
            foreach (var hl013 in hl013s)
            {
                busEntity.HL013.DeleteObject(hl013);
            }
            foreach (var hl014 in hl014s)
            {
                busEntity.HL014.DeleteObject(hl014);
            }

            string code = System.Web.HttpContext.Current.Request.Cookies["unitcode"].Value.Substring(0, 2);
            if (code == "43")
            {
                var hp011s = busEntity.HP011.Where(t => t.PAGENO == pageNO);
                var hp012s = busEntity.HP012.Where(t => t.PAGENO == pageNO);
                foreach (var hp011 in hp011s)
                {
                    busEntity.HP011.DeleteObject(hp011);
                }
                foreach (var hp012 in hp012s)
                {
                    busEntity.HP012.DeleteObject(hp012);
                }
            }

            foreach (var agg in aggs)
            {
                busEntity.AggAccRecord.DeleteObject(agg);
            }
            try
            {
                busEntity.SaveChanges();
                //scope.Complete();
                cleanFlag = true;
            }
            catch (Exception ex)
            {

            }
            //}
            return cleanFlag;
        }

        /// <summary>
        /// 上传附件（每次只上传一个文件）
        /// </summary>
        /// <param name="pageNO">附件所在报表页号</param>
        /// <param name="limit">单位级别</param>
        /// <param name="aList">附件集合</param>
        /// <returns>返回刚上传文件的页号和加密下载路径以“&”分隔隔开</returns>
        public string DBUploadFileAffix(int pageNO, int limit, Affix affix)
        {
            string jsonStr = "0";
            BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
            ReportTitle rpt = new ReportTitle();

            try
            {
                rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
                Affix af = affix;
                af.ReportTitle = rpt;
                af.PageNO = rpt.PageNO;
                rpt.Affix.Add(af);
                busEntity.SaveChanges();
                var affs = (from aff in busEntity.Affix
                            where aff.PageNO == rpt.PageNO
                            orderby aff.TBNO descending
                            select aff).FirstOrDefault();
                Tools tool = new Tools();
                jsonStr = tool.EncryptOrDecrypt(0, affs.DownloadURL, "JXHLZQBS") + "&" + affs.TBNO;
            }

            catch (Exception ex)
            {
                jsonStr = "错误消息：" + ex.Message;
            }
            return jsonStr;
        }



        /// <summary>内蒙蓄水表保存
        /// </summary>
        /// <param name="pageNO"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string NMNPSaveUpdateReport(int pageNO, Hashtable data, string affix,int limit,string unitCode)
        {
            BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(2);
            string saveFlag = "";//保存失败，传出saveFalse&&,传出一个保存失败的标识
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string DBname = "";
            busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault().LastUpdateTime = DateTime.Now;
            #region 插入NP011的数据
            foreach (DictionaryEntry de in data)
            {
                DBname = de.Key.ToString();
                switch (DBname)
                {
                    case "NP011":
                        object[] str011 = serializer.ConvertToType<object[]>(de.Value);
                        for (int i = 0; i < str011.Length; i++)
                        {
                            NP011 np011 = serializer.ConvertToType<NP011>(str011[i]);
                            if (np011.TBNO == 0)
                            {
                                //np011.DataOrder = i;
                                np011.PageNO = pageNO;
                                //np011.ReportTitle = rpt;
                                busEntity.NP011.AddObject(np011);
                            }
                            else
                            {
                                NP011 np011Temp = busEntity.NP011.Where(t => t.TBNO == np011.TBNO).Single();
                                np011Temp.RSCode = np011.RSCode;
                                np011Temp.RSName = np011.RSName;
                                np011Temp.UnitName = np011.UnitName;
                                /*np011Temp.ZKR = np011.ZKR;
                                np011Temp.XXSW = np011.XXSW;
                                np011Temp.ZCXSW = np011.ZCXSW;
                                np011Temp.ZCXSWXYKR = np011.ZCXSWXYKR;*/
                                np011Temp.DQSW = np011.DQSW;
                                np011Temp.DQXSL = np011.DQXSL;
                                np011Temp.SFCXXSW = np011.SFCXXSW;
                                np011Temp.DataOrder = np011.DataOrder;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //物理删除附件
            if (affix != null)
            {
                string[] url = affix.Split(new char[] { ';' })[0].Split(new char[] { ',' });
                string[] tbno = affix.Split(new char[] { ';' })[1].Split(new char[] { ',' });

                for (int l = 0; l < url.Length; l++)
                {
                    int NO = int.Parse(tbno[l].ToString());
                    if (busEntity.Affix.Where(t => t.TBNO == NO).Count() > 0)
                    {
                        var aff = busEntity.Affix.Where(t => t.TBNO == NO).SingleOrDefault();
                        busEntity.Affix.DeleteObject(aff);
                        //busEntity.SaveChanges();
                        string fileName = System.Web.HttpContext.Current.Server.MapPath(new Tools().EncryptOrDecrypt(1, url[l], "JXHLZQBS"));
                        if (System.IO.File.Exists(fileName))
                        {
                            System.IO.File.Delete(fileName);
                        }
                    }
                }
            }
            #endregion
            try
            {
                busEntity.SaveChanges();
                if (limit>2)
                {
                    int[] limitSub = { 2, 4, 6 };
                    string tempCode = unitCode.Substring(0, limitSub[limit - 2]);
                    var np01s = busEntity.NP011.Where(t => t.RSCode.StartsWith(tempCode) && t.PageNO == pageNO).ToList();
                    foreach (var np011 in np01s)
                    {
                        saveFlag += "'" + np011.RSCode + "':'" + np011.TBNO + "',";
                    }
                    if (saveFlag != "")
                    {
                        saveFlag = "{'PageNO':"+pageNO+",'TBNO':{"+saveFlag.Remove(saveFlag.Length - 1)+"}}";
                    }
                }
            }
            catch (Exception ex)
            {
                //rpt = null;
                saveFlag = "saveFalse&&" + ex.Message + "&&" + ex.InnerException;
                return saveFlag;//保存行政表失败，那么肯定不要进行流域分配，
            }
            return saveFlag;
        }

        /// <summary>内蒙古蓄水表判断是否有对应周期的表，没有则新建
        /// </summary>
        /// <returns></returns>
        public string NMNPCreateReport(DateTime endTime)
        {
            DateTime startTime = GetNPRptStartDateTime(endTime);
            //DateTime startTime = GetNPRptStartDateTime();
            //DateTime endTime = GetNPRptEndDateTime(startTime);
            BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(2);//省级
            var rpts =
                busEntity.ReportTitle.Where(t => t.StartDateTime == startTime && t.EndDateTime == endTime && t.Del == 0).ToList();
            if (rpts.Count > 0)
            {
                return rpts.FirstOrDefault().PageNO.ToString();
            }
            else
            {
                ReportHelpClass rptHelp = new ReportHelpClass();
                int pageNO = rptHelp.FindMaxPageNO(2) + 1; //找到最大的页号并加一
                string saveFlag = ""; //保存失败，传出saveFalse&&,传出一个保存失败的标识
                ReportTitle rpt = new ReportTitle();
                DateTime dt = DateTime.Now;
                rpt.CopyPageNO = 0;
                rpt.StatisticalCycType = 2; //旬报
                rpt.ORD_Code = "NP01";
                rpt.RPTType_Code = "XZ0";
                rpt.UnitName = "内蒙古自治区";
                rpt.UnitCode = "15000000";
                rpt.StartDateTime = startTime;
                rpt.WriterTime = dt;
                rpt.EndDateTime = endTime;
                rpt.Del = 0;
                rpt.ReceiveState = 0;
                rpt.State = 0;
                rpt.CloudPageNO = 0;
                rpt.SendOperType = 0;
                rpt.SourceType = 0; //录入
                rpt.SendTime = dt;
                rpt.ReceiveTime = dt;
                rpt.LastUpdateTime = dt;
                rpt.PageNO = pageNO;

                try
                {
                    busEntity.ReportTitle.AddObject(rpt);
                    busEntity.SaveChanges();
                }
                catch (Exception ex)
                {
                    saveFlag = "saveFalse&&" + ex.Message + "&&" + ex.InnerException;
                    App["2_maxPageNO"] = Convert.ToInt32(App["2_maxPageNO"]) - 1; //省级
                    return saveFlag;
                }
                return saveFlag + pageNO;
            }
        }

        public DateTime GetNPRptStartDateTime( DateTime endTime)
        {
            //DateTime dt = DateTime.Now;
            //DateTime startTime = new DateTime();
            //int curMonth = dt.Month;
            //int curDay = dt.Day;
            //if (curMonth >= 4 && curMonth < 10) //汛期
            //{
            //    if (curDay > 15)
            //    {
            //        startTime = Convert.ToDateTime(dt.Year + "-" + dt.Month + "-16");
            //    }
            //    else
            //    {
            //        startTime = Convert.ToDateTime(dt.Year + "-" + dt.Month + "-1");
            //    }
            //}
            //else
            //{
            //    startTime = Convert.ToDateTime(dt.Year + "-" + dt.Month + "-1");
            //}

            DateTime startTime = new DateTime();
            int curYear = endTime.Year;
            int curMonth = endTime.Month;
            int curDay = endTime.Day;
            if (curMonth >= 6 && curMonth < 10) //汛期
            {
                if (curDay > 15)
                {
                    startTime = Convert.ToDateTime(curYear + "-" + curMonth + "-16");
                }
                else
                {
                    startTime = Convert.ToDateTime(curYear + "-" + curMonth + "-1");
                }
            }
            else
            {
                startTime = Convert.ToDateTime(curYear + "-" + curMonth + "-1");
            }
            return startTime;
        }


        public DateTime GetNPRptEndDateTime(DateTime startTime)
        {
            DateTime endTime = new DateTime();
            int curMonth = startTime.Month;
            int curDay = startTime.Day;
            DateTime d1 = new DateTime(startTime.Year, startTime.Month, 1);
            if (curMonth >= 6 && curMonth < 10) //汛期
            {
                if (curDay > 15)
                {
                    endTime = d1.AddMonths(1).AddDays(-1);//当月最后一天
                }
                else
                {
                    endTime = startTime.AddDays(14);
                }
            }
            else
            {
                if (curMonth == 2)//2月只取28号
                {
                    endTime = new DateTime(startTime.Year, startTime.Month, 28);
                }
                else
                {
                    endTime = d1.AddMonths(1).AddDays(-1);
                }
            }
            return endTime;
        }
    }
}
