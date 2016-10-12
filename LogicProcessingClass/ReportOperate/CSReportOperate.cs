using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using EntityModel;
using GJFZWebService.Models.CSModels;
using LogicProcessingClass.XMMZH;
using DBHelper;

namespace LogicProcessingClass.ReportOperate
{
    public class CSReportOperate
    {
        public int SaveRpt(int limit, string unitCode, int pageNO, Hashtable data, Hashtable reportTitle, Hashtable SReport, string affix)
        {
            ViewReportForm viewReport = new ViewReportForm();
            ReportHelpClass rptHelp = new ReportHelpClass();

            CSDBhelper db = new CSDBhelper(limit);//可以默认为0？
            string saveFlag = "";//保存失败，传出saveFalse&&,传出一个保存失败的标识
            CSReportTitle rpt = null;
            //if (isNew.ToUpper() == "FALSE" && reportTitle["State"].ToString() != "3")//isNew是新建还是修改还是Copy
            if (pageNO > 0)//等于0，表示没有在CS库中保存有数据
            {
                bool cflag = DeleteCSRpt(pageNO, limit);
                if (!cflag)//清理失败
                {
                    saveFlag = "saveFalse&&";
                    return -1;
                }
            }
            else
            {
                rpt = new CSReportTitle();
                pageNO = GetMaxID("REPORTTITLE", 0) + 1;//CS数据库中Rpt的最大页号
                //rpt.CopyPageNO = 0;
            }
            try
            {


                #region 保存Rpt
                //新建ReportTitle表
                DateTime dt = DateTime.Now;
                string staticUnitName = HttpContext.Current.Request.Cookies["unitname"].Value;
                rpt.Remark = reportTitle["Remark"] == null ? "" : reportTitle["Remark"].ToString();
                rpt.StatisticsPrincipal = reportTitle["StatisticsPrincipal"] == null ? "" : reportTitle["StatisticsPrincipal"].ToString();
                rpt.UnitPrincipal = reportTitle["UnitPrincipal"] == null ? "" : reportTitle["UnitPrincipal"].ToString();
                rpt.WriterName = reportTitle["WriterName"] == null ? "" : reportTitle["WriterName"].ToString();
                rpt.StatisticalCycType = Convert.ToInt32(reportTitle["StatisticalCycType"]);
                rpt.ORD_Code = reportTitle["ORD_Code"] == null ? "" : reportTitle["ORD_Code"].ToString();
                rpt.RPTType_Code = reportTitle["RPTType_Code"] == null ? "XZ0" : reportTitle["RPTType_Code"].ToString();
                rpt.UnitName = reportTitle["UnitName"] == null ? HttpUtility.UrlDecode(staticUnitName, Encoding.GetEncoding("utf-8")) : reportTitle["UnitName"].ToString();
                rpt.UnitCode = unitCode;
                rpt.StartDateTime = Convert.ToDateTime(reportTitle["StartDateTime"].ToString());
                rpt.WriterTime = Convert.ToDateTime(reportTitle["WriterTime"].ToString());
                rpt.EndDateTime = Convert.ToDateTime(reportTitle["EndDateTime"].ToString());
                rpt.Del = 0;
                //rpt.ReceiveState = 0;
                rpt.State = 0;
                //rpt.CloudPageNO = 0;
                //rpt.SendOperType = 0;
                rpt.SourceType = reportTitle["SourceType"] == null ? 0 : Convert.ToInt32(reportTitle["SourceType"].ToString());
                rpt.SendTime = dt;
                rpt.ReceiveTime = dt;
                rpt.LastUpdateTime = dt;
                rpt.PageNO = pageNO;

                InsertDataBySQL<CSReportTitle>("REPORTTITLE", rpt, db);
                #endregion

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                //新建修改HL011-HL014，HP011-HP012的表
                string DBname = "";
                XMMZHClass xmm = new XMMZHClass();
                #region 插入HL011-HL012，HP011-HP012的数据

                int maxTbno = 1;
                foreach (DictionaryEntry de in data)//data里有HL011-HL014，HP011-HP012的所有数据，遍历这些数据
                {
                    DBname = de.Key.ToString();
                    switch (DBname)//根据DBname不同执行不同的操作
                    {
                        case "HL011"://往HL011表中插入数据
                            maxTbno = GetMaxID("HL011", 0) + 1;
                            object[] str011 = serializer.ConvertToType<object[]>(de.Value);//反序列化CSHL011，将里面的数据放入一个对象数组中去
                            for (int i = 0; i < str011.Length; i++)//循环这个对象数组
                            {
                                CSHL011 hl011 = serializer.ConvertToType<CSHL011>(str011[i]);//HL011对象接收，数组里的对象
                                hl011.DataOrder = i;
                                hl011.PageNO = rpt.PageNO;
                                hl011 = xmm.ToSetHL<CSHL011>(hl011, limit);//数量级转换
                                hl011.TBNO = maxTbno;
                                maxTbno++;
                                InsertDataBySQL<CSHL011>("HL011", hl011, db);
                            }
                            break;
                        case "HL012":
                            maxTbno = GetMaxID("HL012", 0) + 1;
                            object[] str012 = serializer.ConvertToType<object[]>(de.Value);
                            for (int i = 0; i < str012.Length; i++)
                            {
                                CSHL012 hl012 = serializer.ConvertToType<CSHL012>(str012[i]);
                                hl012.DataOrder = i;
                                hl012.PageNO = rpt.PageNO;
                                hl012.TBNO = maxTbno;
                                maxTbno++;
                                InsertDataBySQL<CSHL012>("HL012", hl012, db);
                            }
                            break;
                        case "HL013":
                            maxTbno = GetMaxID("HL013", 0) + 1;
                            object[] str013 = serializer.ConvertToType<object[]>(de.Value);
                            for (int i = 0; i < str013.Length; i++)
                            {
                                CSHL013 hl013 = serializer.ConvertToType<CSHL013>(str013[i]);
                                hl013.DataOrder = i;
                                hl013.PageNO = rpt.PageNO;
                                hl013 = xmm.ToSetHL<CSHL013>(hl013, limit);
                                hl013.TBNO = maxTbno;
                                maxTbno++;
                                InsertDataBySQL<CSHL013>("HL013", hl013, db);
                            }
                            break;
                        case "HL014":
                            maxTbno = GetMaxID("HL014", 0) + 1;
                            object[] str014 = serializer.ConvertToType<object[]>(de.Value);
                            for (int i = 0; i < str014.Length; i++)
                            {
                                CSHL014 hl014 = serializer.ConvertToType<CSHL014>(str014[i]);
                                hl014.DataOrder = i;
                                hl014.PageNO = rpt.PageNO;
                                hl014 = xmm.ToSetHL<CSHL014>(hl014, limit);
                                hl014.TBNO = maxTbno;
                                maxTbno++;
                                InsertDataBySQL<CSHL014>("HL014", hl014, db);
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                #region AggAcc
                if ((rpt.SourceType == 1 || rpt.SourceType == 2) && SReport["SourceReport"] != null)
                {
                    maxTbno = GetMaxID("AggAccRecord", 0) + 1;
                    object[] AggAccs = serializer.ConvertToType<object[]>(SReport["SourceReport"]);
                    int oldSPageNO = 0;
                    
                    BusinessEntities busEntity = Persistence.GetDbEntities(2);//省级库
                    for (int i = 0; i < AggAccs.Length; i++)
                    {
                        Hashtable AggAcc = serializer.Deserialize<Hashtable>(serializer.Serialize(AggAccs[i]));
                        oldSPageNO = Convert.ToInt32(AggAcc["id"]);
                        var csPageNO = busEntity.ReportTitle.Where(t => t.PageNO == oldSPageNO).SingleOrDefault().CSPageNO;//找出BS中的报表在CS中保存的页号，存入CS中的AggAcc中

                        CSAggAccRecord agg = new CSAggAccRecord();
                        agg.ORD_Code = reportTitle["ORD_Code"].ToString();
                        agg.SPageNO = Convert.ToInt32(csPageNO);
                        agg.PageNo = pageNO;
                        agg.UnitCode = AggAcc["UnitCode"].ToString();
                        agg.DBType = 0;
                        agg.OperateType = rpt.SourceType;
                        agg.TBNO = maxTbno;
                        maxTbno++;
                        InsertDataBySQL<CSAggAccRecord>("AGGACCRECORD", agg, db);
                    }
                }
                #endregion

                #region
                //删除附件记录
                //if (affix != null)
                //{
                //    if (reportTitle["State"] != null && reportTitle["State"].ToString() == "3")//!= "3"改为==3 已经报送的报表不能删除附件
                //    {
                //    }
                //    else
                //    {
                //        string[] url = affix.Split(new char[] { ';' })[0].Split(new char[] { ',' });
                //        string[] tbno = affix.Split(new char[] { ';' })[1].Split(new char[] { ',' });
                //        for (int l = 0; l < url.Length; l++)
                //        {
                //            int NO = int.Parse(tbno[l].ToString());
                //            if (busEntity.Affix.Where(t => t.TBNO == NO).Count() > 0)
                //            {
                //                aff.FileName =
                //                    System.Web.HttpContext.Current.Server.MapPath(new Tools().EncryptOrDecrypt(1, url[l],
                //                        "JXHLZQBS"));
                //            }
                //        }
                //    }
                //}
                #endregion

                return pageNO;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        /// <summary>根据传入的值拼接sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int InsertDataBySQL<T>(string tableName, T obj, CSDBhelper db)
        {
            string sql = "INSERT INTO " + tableName;
            PropertyInfo[] pfs = obj.GetType().GetProperties();//利用反射获得类的属性
            string fields = "";
            string values = "";
            for (int i = 0; i < pfs.Length; i++)
            {
                if (pfs[i].GetValue(obj, null) != null && pfs[i].GetValue(obj, null).ToString() != "0001/1/1 0:00:00")
                {
                    fields += pfs[i].Name + ",";//字段拼接的字符串
                    values += "'" + pfs[i].GetValue(obj, null).ToString() + "',";
                }
            }
            if (fields != "" && values != "")
            {
                sql += " ( " + fields.Remove(fields.Length - 1) + " ) VALUES ( " + values.Remove(values.Length - 1) + " )";
            }
            else
            {
                sql = "";
            }
            DbCommand cmd = null;
            cmd = db.GetSqlStringCommond(sql);
            int count = db.ExecuteNonQuery(cmd);
            return count;
        }


        /// <summary>找出某个表中最大主键值REPORTTITLE表的为PageNo，其他的为TBNO
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="unitType">本级：0，下级：1</param>
        /// <returns>最大主键值</returns>
        public int GetMaxID(string tableName, int unitType)
        {
            int maxId = 0;
            string sql = "";
            if (tableName.ToUpper() == "REPORTTITLE")
            {
                sql = "select max(pageno) from REPORTTITLE";
            }
            else
            {
                sql = "select max(tbno) from " + tableName;
            }
            CSDBhelper db = new CSDBhelper(unitType);
            DbCommand cmd = db.GetSqlStringCommond(sql);
            if (!Convert.IsDBNull(db.ExecuteScalar(cmd)))
            {
                maxId = Convert.ToInt32(db.ExecuteScalar(cmd));
            }
            return maxId;
        }


        /// <summary>从数据库中清空某套报表
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="limit">0是本级，1是下级</param>
        /// <returns></returns>
        public bool DeleteCSRpt(int pageNO, int limit)
        {
            bool flag = false;
            int countSum = 0;
            CSDBhelper db = new CSDBhelper(limit);
            string sqlAff = " delete from AFFIX where pageno = " + pageNO;
            string sqlAgg = "delete from AGGACCRECORD where pageno = " + pageNO;
            string sql01 = "delete from HL011 where pageno = " + pageNO;
            string sql02 = "delete from HL012 where pageno = " + pageNO;
            string sql03 = "delete from HL013 where pageno = " + pageNO;
            string sql04 = "delete from HL014 where pageno = " + pageNO;
            string sqlRpt = "delete from REPORTTITLE where pageno = " + pageNO;
            string[] sqlArr = { sqlAff, sqlAgg, sql01, sql02, sql03, sql04, sqlRpt };

            DbCommand cmd = null;
            using (Trans t = new Trans(limit))
            {
                try
                {
                    for (int i = 0; i < sqlArr.Length; i++)
                    {
                        cmd = db.GetSqlStringCommond(sqlArr[i]);
                        db.ExecuteNonQuery(cmd);
                    }
                    t.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    t.RollBack();
                    return false;
                }
            }
            //return flag;
        }

        /// <summary>恢复回收站的报表
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="pageNO"></param>
        /// <returns></returns>
        public bool RecycleResumeRpt(int limit, int pageNO)
        {
            string sql = "update REPORTTITLE set del = 0 where pageno  =  " + pageNO;
            CSDBhelper db = new CSDBhelper(limit);
            DbCommand cmd = db.GetSqlStringCommond(sql);
            int count = db.ExecuteNonQuery(cmd);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>非物理删除报表
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="pageNO"></param>
        /// <returns></returns>
        public bool DelRpt(int limit, int pageNO)
        {
            string sql = "update REPORTTITLE set del = 1 where pageno  =  " + pageNO;
            CSDBhelper db = new CSDBhelper(limit);
            DbCommand cmd = db.GetSqlStringCommond(sql);
            int count = db.ExecuteNonQuery(cmd);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

 
        /// <summary>对接收表箱中报表的操作
        /// </summary>
        /// <param name="limit">0:国家防总，其他值是县市级的数据库</param>
        /// <param name="pageNO">bs中的页号</param>
        /// <param name="operateType">BS中的操作类型，-1：恢复，1：拒收，2：装入，3：删除</param>
        /// <param name="lastState">BS中保存的删除之前的状态</param>
        /// <returns></returns>
        public bool InboxOperateRpt(int limit, int pageNO, int operateType,int lastState)
        {
            int temp = 2;//CS中不同表箱的状态值，下载：-1，装入：0，拒收：-2，删除：1
            int lastStateTemp = -1;//把BS中的状态值，转换为CS中对应的状态值
            if (lastState == 1)//拒收
            {
                lastStateTemp = -2;
            }
            else if (lastState == 2)//装入
            {
                lastStateTemp = 0;
            }

            if (operateType == -1)//恢复
            {
                temp = lastStateTemp;
            }
            else if (operateType == 1)//拒收
            {
                temp = -2;
            }
            else if (operateType == 2)//装入
            {
                temp = 0;
            }
            else if (operateType == 3)//删除
            {
                temp = 1;
            }
            BusinessEntities busEntity = Persistence.GetDbEntities(2);//省级库
            var csPageNO = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault().CSPageNO;//找出BS中的报表在CS中保存的页号
            string sql = "update REPORTTITLE set del = " + temp + " where pageno  =  " + csPageNO;
            CSDBhelper db = new CSDBhelper(limit);
            DbCommand cmd = db.GetSqlStringCommond(sql);
            int count = db.ExecuteNonQuery(cmd);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
