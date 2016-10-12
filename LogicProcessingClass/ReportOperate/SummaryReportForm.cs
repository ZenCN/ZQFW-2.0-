using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBHelper;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：SummaryReportForm.cs
// 文件功能描述：汇总累计
// 修改标识：// 修改描述：

//-------------------------------------------------------------*/
using EntityModel;
using LogicProcessingClass.AuxiliaryClass;
using EntityModel.RepeatModel;
using System.Reflection;
using NPOI.SS.Formula.Functions;

namespace LogicProcessingClass.ReportOperate
{
    public class SummaryReportForm
    {
        /// <summary> 汇总数据
        /// </summary>
        /// <param name="pageNoList">页号集合</param>
        /// <param name="limit">当前单位级别</param>
        /// <param name="typeLimit">查看本级(0)表或者下级(1)表</param>
        /// <param name="operateType">操作名称，加表：sum,减表sub</param>
        /// <returns></returns>
        public string GetSummaryReportFormData(string pageNoList, int limit, int typeLimit, string operateType,string unitCode)
        {
            string jsonStr = "{";

            TableFieldBaseData table = new TableFieldBaseData();
            IList list = new List<object[]>();
            IList tempList = table.GetAllFieldUnitIList(limit);
            for (int i = 0; i < tempList.Count; i++)
            {
                TB55_FieldDefine tb55 = (TB55_FieldDefine)tempList[i];
                if (limit == tb55.UnitCls)
                {
                    //FieldCode,TDTabCode,MeasureValue,DecimalCount,MeasureName,InputRemark,UnitCls
                    object[] obj = new object[7];
                    obj[0] = tb55.FieldCode;
                    obj[1] = tb55.TD_TabCode;
                    obj[2] = tb55.MeasureValue;
                    obj[3] = tb55.DecimalCount;
                    obj[4] = tb55.MeasureName;
                    obj[5] = tb55.InputRemark;
                    obj[6] = tb55.UnitCls;
                    list.Add(obj);
                }
            }
            jsonStr += GetReportFormList(pageNoList, list, typeLimit, limit,operateType,unitCode);
            jsonStr += "}";
            return jsonStr;
        }

        /// <summary>
        /// 根据页号集合，查询对应字段数据集合
        /// </summary>
        /// <param name="pageNoList">页号集合</param>
        /// <param name="IList">字段集合,</param>
        /// <param name="dataBaseName">数据库名</param>
        /// <param name="typelimit">本级库或下级库，用于区别省级的汇总和市级的累计</param>
        /// <returns></returns>
        /*public string GetReportFormList(string pageNoList, IList list, EntitiesConnection.entityName entityName, int typelimit,  int limit,string operateType,string unitCode)
        {
            StringBuilder sbJSON = new StringBuilder();
            ArrayList filedList = QueryTableFiled(list);
            IList alist = new ArrayList();
            IList clist = new ArrayList();
            ArrayList arry = new ArrayList();
            string sql = "";
            string result = "";
            string[] pageNos = new string[] { "", "load", "" };
            SortedList slist = null;
            bool repeat = false;
            bool InQuiry = true;
            BusinessEntities CTYbusEntities = null;
            
            for (int i = 0; i < filedList.Count; i++)
            {
                string tableName = "";
                switch (i)
                {
                    case 0:
                        tableName = "HL011";
                        break;
                    case 1:
                        tableName = "HL012";
                        break;
                    case 2:
                        tableName = "HL013";
                        break;
                    default:
                        tableName = "HL014";
                        break;
                }

                # region 仅用于省级累计

                if ("FXPRVEntities".Equals(entityName.ToString()) && tableName == "HL011" && limit != 0)  //使用FXPRV库说明，当前表为累计表
                {
                    pageNos = pageNoList.Split(new string[] { ";" }, StringSplitOptions.None);  //分隔不同类型的表，“录入表；汇总表；累计表”
                    pageNos[2] = (pageNos[1] == "" ? (pageNos[2] == "" ? "" : pageNos[2]) : (pageNos[2] == "" ? pageNos[1] : pageNos[1] + "," + pageNos[2]));  //将汇总表、累计表页号放在一起，方便一起查询本机库中的AggAcc表
                    pageNoList = (pageNos[0] == "" ? (pageNos[2] == "" ? "" : pageNos[2]) : (pageNos[2] == "" ? pageNos[0] : pageNos[0] + "," + pageNos[2]));  //将三种不同来源类型的表放在一起
                    pageNos[1] = "";  //置空汇总表页号集合

                    if (pageNos[2] == "") //如果加了录入表，则设置InQuiry的值，防止其到下级库中查找源表
                    {
                        InQuiry = false;
                    }

                    int j = 1, k = 0;  //计数器，j = 1方便循环第一次查累计表或汇总表的源表，第二次查源表的来源类型
                    while (pageNos[2] != "") //是否存在累计表、汇总表
                    {
                        alist.Clear();
                        if (j % 2 != 0) //表示非“源表”，即是；累计表或汇总表
                        {
                            //hsql = "select OperateType,SPageNO from AggAccRecord where PageNo in(" + pageNos[2] + ")"; 
                            //在FXPRV库中查累计表、汇总表的源表
                            var aggs = busEntities.AggAccRecord.Where("it.PageNo in{" + pageNos[2] + "}").Select(t => new { t.OperateType, t.SPageNO }).ToList();
                            foreach (var obj in aggs)
                            {
                                object[] objArr = new object[2];
                                objArr[0] = obj.OperateType;
                                objArr[1] = obj.SPageNO;
                                alist.Add(objArr);
                            }
                        }
                        else
                        {
                            //hsql = "select SourceType,Id from ReportTitle where PageNo in(" + pageNos[2] + ")";
                            //在FXPRV库中查源表的来源类型
                            var rpts = busEntities.ReportTitle.Where("it.PageNO in{" + pageNos[2] + "}").Select(t => new { t.SourceType, t.PageNO }).ToList();
                            foreach (var obj in rpts)
                            {
                                object[] objArr = new object[2];
                                objArr[0] = obj.SourceType;
                                objArr[1] = obj.PageNO;
                                alist.Add(objArr);
                            }
                        }

                        pageNos[2] = "";  //置空非“源表”的集合，方便下次查询
                        for (int m = 0; m < alist.Count; m++)
                        {
                            object[] obj = (object[])alist[m];
                            switch (Convert.ToInt32(obj[0]))  //obj[0]为OperateType(1,2)或SourceType(0,1,2)的值
                            {
                                case 0:  //为“0”只有在ReportTitle中才能查得到，并且说明当前表为本级的录入表
                                    k = 0;
                                    break;
                                case 1:  //“obj”为汇总表
                                    if (j % 2 != 0)  //在AggAccRecord表中查出OperateType=1，把PageNO放到pageNos[1]中方便在下级库中查询这些页号
                                    {
                                        k = 1;
                                    }
                                    else  //在ReportTitle表中查出源表的来源类型为1，即当前源表为汇总表，把PageNO放到pageNos[2]中方便二次查询AggAccRecord中的OperateType
                                    {
                                        k = 2;
                                    }
                                    break;
                                case 2:  //累计
                                    k = 2;  //在AggAccRecord或ReportTitle表中查出当前表为累计表，把PageNO放到pageNos[2]中方便二次查询AggAccRecord中的OperateType
                                    break;
                            }
                            if (pageNos[k] != "")
                            {
                                if (!repeat && pageNos[k].IndexOf(obj[1].ToString()) > 0)  //判断查出来的页号是否已存在pageNos[k]中，并记录是否有重复的页号
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
                    if (pageNos[0] != "")  //当前累计表中有本级的录入表
                    {
                        //query = iSession.CreateQuery("select UnitCode,sum(SZFWX) from HL011 where PageNO in(" + pageNos[0] + ") and dw <>'合计' group by UnitCode");  //查询当前累计表中“所有的的录入表！”的受灾范围县“不重复的”所有的页号
                        //clist = query.List();
                        var hl01s = busEntities.HL011.Where("it.PageNO in {" + pageNos[0] + "}").AsQueryable();
                        var hl01temp = (from temp in hl01s
                                        where temp.DW != "合计"
                                        group temp by temp.UnitCode into h
                                        select new
                                        {
                                            UnitCode = h.Key,
                                            SZFWXCount = h.Sum(t => t.SZFWX)
                                        });
                        clist.Clear();
                        foreach (var obj in hl01temp)
                        {
                            object[] objArr = new object[2];
                            objArr[0] = obj.UnitCode;
                            objArr[1] = obj.SZFWXCount;
                            clist.Add(objArr);
                        }
                    }
                }

                #endregion

                sql = CreateHSQL(pageNoList, filedList[i].ToString(), tableName, list, limit);  //创建SQL语句  limit

                //国家防总累计
                if ("FXCLDEntities".Equals(entityName.ToString()))
                {
                    sql = sql.Replace("dw='合计'", "dw<>'合计'");
                }

                #region 张建军添加的代码,用于省级（汇总、累计）、市级（汇总、累计）、县级（汇总）生成的SQL语句

                if (tableName == "HL011" && limit != 0)
                {
                    if ("FXCNTEntities".Equals(entityName.ToString()))  //市级汇总或县级累计
                    {
                        if (typelimit == 0)  //县级累计
                        {
                            sql = sql.Replace("dw='合计'", "dw<>'合计'");
                        }
                        else if (typelimit == 1)  //市级汇总
                        {
                            sql = sql.Replace("sum(SZFWZ)", "sum(distinct(SZFWZ))");
                        }
                    }
                    else if (("FXCTYEntities".Equals(entityName.ToString()) || "FXPRVEntities".Equals(entityName.ToString())) && typelimit == 0) //省级累计、汇总或者市级累计要剔除重复值，必须查“合计行”的“元数据”
                    {
                        sql = sql.Replace("dw='合计'", "dw<>'合计'");
                    }
                }
                else if (limit != 0 && tableName == "HL014" && typelimit == 0 && (("FXCTYEntities".Equals(entityName.ToString()) || "FXCNTEntities".Equals(entityName.ToString()) && pageNos[1] == "load") || "FXPRVEntities".Equals(entityName.ToString())))  //省级累计、市级累计
                {
                    sql = sql.Replace("dw='合计'", "dw<>'合计'");
                }

                //sql = sql.Replace("sum(", "sum(isnull(").Replace(")/", ",0))/");//不进行为空处理，未知结果影响
                #endregion
                switch (tableName)
                {
                    case "HL011":
                        alist = busEntities.ExecuteStoreQuery<ReportHL011>(sql).ToList();
                        break;
                    case "HL012":
                        alist = busEntities.ExecuteStoreQuery<ReportHL012>(sql).ToList();
                        break;
                    case "HL013":
                        alist = busEntities.ExecuteStoreQuery<ReportHL013>(sql).ToList();
                        break;
                    default:
                        alist = busEntities.ExecuteStoreQuery<ReportHL014>(sql).ToList();
                        break;
                }
                //for (i = 0; i < alist.Count; i++)
                //{
                //    PropertyInfo[] propertys = alist[i].GetType().GetProperties();
                //    foreach (var propertyInfo in propertys)
                //    {
                //        if (propertyInfo.GetValue(alist[i], null) != null)
                //        {
                //            string s = propertyInfo.GetValue(alist[i], null).ToString();
                //            if (s == "0.000000")
                //            {
                //                propertyInfo.SetValue(alist[i], null, null);
                //            }
                //        }
                //    }
                //}
                //query = iSession.CreateQuery(hsql);
                //alist = query.List();

                //if (tableName == "HL012" || tableName == "HL013")
                //{
                //    sbJSON.Append(tableName).Append(":[").Append(RetainNumber(alist, list, filedList[i].ToString(), tableName));
                //    sbJSON.Append("],");
                //}
                //else
                //{
                sbJSON.Append(tableName).Append(":[").Append(RetainNumber(alist, list, filedList[i].ToString(), tableName,operateType));//修改有}改为]

                #region 仅仅用于省级汇总、累计

                if (limit != 0 && tableName == "HL011" && ("FXCTYEntities".Equals(entityName.ToString()) && typelimit != 0 || "FXPRVEntities".Equals(entityName.ToString())))  //省级汇总(FXCTYEntities)或者累计(FXPRVEntities、FXCTYEntities!)
                {
                    string pageNo = ((pageNos[1] != "" && pageNos[1] != "load") ? pageNos[1] : pageNoList);  //判断当前表是否为累计表，如果是累计表，返回要在级库中查询下的页号，如不是，即当前表为汇总表，则返回前台传过来的pageNoList
                    if (clist != null)  //当前累计表的源表中查询过录入表，即查询累计表中的源表中属于省级的中的数据(只有录入类型的报表)，（clist变量用过）
                    {
                        if (clist.Count > 0)  //如果存在录入表的数据
                        {
                            string tmp = sbJSON.ToString();
                            sbJSON = sbJSON.Remove(0, sbJSON.Length);
                            sbJSON.Append(ReplaceSZFWX(clist, tmp));

                            object[] arr = null;
                            for (var j = 0; j < clist.Count; j++)
                            {
                                arr = (object[])clist[j];
                                if (arr[0].ToString().Substring(4) == "0000")
                                {
                                    sbJSON.Append(",{" + arr[0].ToString() + ":" + Convert.ToInt32(arr[1]) + "}");
                                }
                            }
                        }
                    }
                    else if ("FXPRVEntities".Equals(entityName.ToString()))  //当前累计表中的源表中不存在录入表
                    {
                        string tmp = sbJSON.ToString();
                        sbJSON = sbJSON.Remove(0, sbJSON.Length);
                        sbJSON.Append(ReplaceSZFWX(tmp));
                    }

                    if (pageNo != "" && InQuiry)  //用省级汇总表的源表的页号、省级累计表中所有汇总类型的源表的源表的页号！（源表(汇总)的源表）到市级库中查询相关的数据
                    {
                        if ("FXPRVEntities".Equals(entityName.ToString()))  //如果当前表为省级累计表
                        {
                            //iSession.Close();  //关闭iSession，即断开与省级库的连接
                            //iSessionFactory.Close();
                            busEntities = getBusinessEntities(3, 0);  //连接市级库，方便查询数据

                            if (repeat)  //如果存在重复的页号（这个地方，可能会存在重复的页号，因为这些页号都是真实存在于市级库中的，即省级的累计表可能累计了市级库的同一个页号，例如，省级库中累计1表包含汇总1表（包含市级库中录入1表、汇总2表，累计3表）、汇总2表（包含市级库中录入1表）
                            {
                                slist = new CommonFunction().RecordStrRepeatTimes(pageNo);  //记录重复页号的重复的次数，！！！！！！可能会使累计受灾范围县结果偏大！！
                            }
                        }

                        if (slist == null)  //表示省级汇总（slist变量没用过）
                        {
                            slist = new SortedList();
                            slist.Add(1, pageNo);  //记录页号重复的次数
                        }
                        //该位置还没做**************************************************************************************
                        for (int n = 0; n < slist.Count; n++)
                        {
                            //hsql = "select UnitCode,sum(SZFWX)*" + slist.GetKey(n) + " from HL011 where PageNo in(" + slist[slist.GetKey(n)] + ") and dw<>'合计' group by UnitCode ";  //slist.GetKey(n)表示页号重复的次数
                            var hl01s = busEntities.HL011.Where("it.PageNO in {" + slist[slist.GetKey(n)] + "}").AsQueryable();
                            decimal slistCount = Convert.ToDecimal(slist.GetKey(n));
                            var hl01temp = (from temp in hl01s
                                            where temp.DW != "合计"
                                            group temp by temp.UnitCode into h
                                            select new
                                            {
                                                UnitCode = h.Key,
                                                SZFWXCount = h.Sum(t => t.SZFWX) * slistCount
                                            });
                            if (clist != null)
                            {
                                clist.Clear();
                            }
                            foreach (var obj in hl01temp)
                            {
                                object[] objArr = new object[2];
                                objArr[0] = obj.UnitCode;
                                objArr[1] = obj.SZFWXCount;
                                clist.Add(objArr);
                            }

                            //clist = query.List();  //到市级库中查询不重复的受灾范围县的数据
                            if (clist.Count > 0)
                            {
                                bool NoDataBefore = true;  //标记HL011之前是否存在数据
                                if (sbJSON.ToString() != "HL011:[")  //存在有HL011的数据
                                {
                                    //sbJSON = sbJSON.Remove(sbJSON.Length - 1, 1).Append(",");
                                    sbJSON = sbJSON.Append(",");
                                    NoDataBefore = false;
                                }
                                for (int j = 0; j < clist.Count; j++)
                                {
                                    object[] obj = (object[])clist[j];
                                    sbJSON.Append("{"+obj[0] + ":'" + Convert.ToInt32(obj[1]) + "'},");  //保存不重复的受灾范围县的数据
                                    //sbJSON.Append(obj[0] + ":{Fake:'" + Convert.ToInt32(obj[1]) + "',Real:'',Details:[]}");
                                }
                                sbJSON = sbJSON.Remove(sbJSON.Length - 1, 1);
                                //if (!NoDataBefore) //HL011之前存在数据
                                //{
                                //    sbJSON = sbJSON.Append("}");
                                //}
                            }
                        }

                        if ("FXPRVEntities".Equals(entityName.ToString()))  //如果当前表是省级累计表，则需要还原数据库配置，即关闭市级库，重新连接省级库，以方便查询其它表的信息
                        {
                            busEntities = getBusinessEntities(2, 0);
                        }
                        clist = null;
                    }
                }

                #endregion

                sbJSON.Append("],");//修改有}改为]
                //}
            }
            //if (unitCode.StartsWith("33"))
            //{
            //    string maxTemp = GetTab6Max(pageNoList);
            //    sbJSON.Append("'MAX':").Append(maxTemp);
            //    return sbJSON.ToString();
            //}

            result = sbJSON.Remove(sbJSON.Length - 1, 1).ToString();



            return result;   //.Remove(sbJSON.Length - 1, 1).ToString(); //sbJSON.ToString(); //
           
        }*/

        public string GetReportFormList(string pageNoList, IList list, int typelimit, int limit, string operateType, string unitCode)
        {
            StringBuilder sbJSON = new StringBuilder();
            ArrayList filedList = QueryTableFiled(list);
            IList alist = new ArrayList();
            IList clist = new ArrayList();
            ArrayList arry = new ArrayList();
            string sql = "";
            string result = "";
            string[] pageNos = new string[] { "", "load", "" };
            SortedList slist = null;
            bool repeat = false;
            bool InQuiry = true;
            BusinessEntities busEntities = Persistence.GetDbEntities(limit + typelimit);

            for (int i = 0; i < filedList.Count; i++)
            {
                string tableName = "";
                switch (i)
                {
                    case 0:
                        tableName = "HL011";
                        break;
                    case 1:
                        tableName = "HL012";
                        break;
                    case 2:
                        tableName = "HL013";
                        break;
                    default:
                        tableName = "HL014";
                        break;
                }

                sql = CreateHSQL(pageNoList, filedList[i].ToString(), tableName, list, limit);  //创建SQL语句  limit

                if ((tableName == "HL011" || tableName == "HL014") && typelimit == 0)
                {
                    sql = sql.Replace("dw='合计'", "dw<>'合计'");
                }

                switch (tableName)
                {
                    case "HL011":
                        alist = busEntities.ExecuteStoreQuery<ReportHL011>(sql).ToList();
                        break;
                    case "HL012":
                        alist = busEntities.ExecuteStoreQuery<ReportHL012>(sql).ToList();
                        break;
                    case "HL013":
                        alist = busEntities.ExecuteStoreQuery<ReportHL013>(sql).ToList();
                        break;
                    default:
                        alist = busEntities.ExecuteStoreQuery<ReportHL014>(sql).ToList();
                        break;
                }
                
                sbJSON.Append(tableName).Append(":[").Append(RetainNumber(alist, list, filedList[i].ToString(), tableName, operateType));//修改有}改为]
                sbJSON.Append("],");
            }
            busEntities.Dispose();
            result = sbJSON.Remove(sbJSON.Length - 1, 1).ToString();

            return result; 

        }


        /// <summary>
        /// 生成要返回的数据列
        /// </summary>
        /// <param name="list">传入数据列集合</param>
        /// <returns></returns>
        public ArrayList QueryTableFiled(IList list)
        {
            ArrayList alist = new ArrayList();
            StringBuilder sbHL011 = new StringBuilder();
            StringBuilder sbHL012 = new StringBuilder();
            StringBuilder sbHL013 = new StringBuilder();
            StringBuilder sbHL014 = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                object[] objArr = (object[])list[i];
                int number = Convert.ToInt32(Convert.ToDouble(objArr[2]));
                if ("DW".Equals(objArr[0].ToString()))
                {
                    continue;
                }
                switch (objArr[1].ToString())
                {
                    case "HL011":
                        sbHL011.Append(objArr[0]).Append(",");
                        break;
                    case "HL012":
                        sbHL012.Append(objArr[0]).Append(",");
                        break;
                    case "HL013":
                        sbHL013.Append(objArr[0]).Append(",");
                        break;
                    default://HL014
                        sbHL014.Append(objArr[0]).Append(",");
                        break;
                }
            }
            if (sbHL011.Length > 0)
            {
                sbHL011.Remove(sbHL011.Length - 1, 1);
            }
            if (sbHL012.Length > 0)
            {
                sbHL012.Remove(sbHL012.Length - 1, 1);
            }
            if (sbHL013.Length > 0)
            {
                sbHL013.Remove(sbHL013.Length - 1, 1);
            }
            if (sbHL014.Length > 0)
            {
                sbHL014.Remove(sbHL014.Length - 1, 1);
            }
            alist.Add(sbHL011);
            alist.Add(sbHL012);
            alist.Add(sbHL013);
            alist.Add(sbHL014);
            return alist;
        }


        /// <summary>生成特定HQL语句
        /// </summary>
        /// <param name="pageNoList">页号集合</param>
        /// <param name="fieldList">列集合</param>
        /// <param name="tableName">表名</param>
        /// <param name="list">tb55表的字段数据集合（系数等）</param>
        /// <param name="limit">级别</param>
        /// <returns></returns>
        public string CreateHSQL(string pageNoList, string fieldList, string tableName, IList list, int limit)
        {
            StringBuilder hsql = new StringBuilder();
            string[] fieldArr = fieldList.Split(',');
            if ("HL012".Equals(tableName) || "HL013".Equals(tableName))
            {
                hsql.Append("select PageNO,UnitCode,DW,TBNO,");  //张建军增加了PageNO字段
            }
            else
            {
                hsql.Append("select UnitCode,DW,");
            }

            if ("HL012".Equals(tableName))
            {
                hsql.Append("DataType,");
            }
            StringBuilder notSumStr = new StringBuilder();
            for (int i = 0; i < fieldArr.Length; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    object[] obj = (object[])list[j];
                    if ("DW".Equals(fieldArr[i]))
                    {
                        break;
                    }
                    if (fieldArr[i].Equals(obj[0].ToString()) && tableName.Equals(obj[1].ToString()) && limit.ToString().Equals(obj[6].ToString()))
                    {
                        int decimalNumber = Convert.ToInt32(Convert.ToDouble(obj[3]));
                        int numberUnit = Convert.ToInt32(Convert.ToDouble(obj[2]));
                        if (numberUnit == 0)
                        {
                            numberUnit = 1;
                        }
                        //numberUnit = 1;
                        if (decimalNumber == -1)
                        {
                            hsql.Append(fieldArr[i]).Append(",");
                            notSumStr.Append(fieldArr[i]).Append(",");
                            break;
                        }
                        else
                        {
                            hsql.Append("sum(").Append(fieldArr[i]).Append(")/").Append(numberUnit).Append(" as ").Append(fieldArr[i]).Append(",");
                            break;
                        }
                    }
                }
            }
            hsql.Remove(hsql.Length - 1, 1);
            hsql.Append(" from ").Append(tableName).Append(" where PageNO in (").Append(pageNoList).Append(")");
            if ("HL012".Equals(tableName) || "HL013".Equals(tableName))
            {
                if ("HL013".Equals(tableName))
                {
                    hsql.Append(" and dw<>'合计' ");
                }

                hsql.Append(" group by UnitCode,DW,TBNO,PageNO ");

                if ("HL012".Equals(tableName))
                {
                    hsql.Append(",DataType");
                }

            }
            else
            {
                hsql.Append(" and dw='合计' group by UnitCode,DW ");
            }
            if (notSumStr.ToString() != "")
            {
                notSumStr.Remove(notSumStr.Length - 1, 1);
            }
            if (notSumStr.ToString() != "")
            {
                hsql.Append(",").Append(notSumStr.ToString());
            }
            return hsql.ToString();
        }

        /// <summary>合计行SQL，对有重复值的数据，不进行累加，前台进行（暂时所以数据交给前台处理，该方法没用到）
        /// </summary>
        /// <param name="pageNoList"></param>
        /// <param name="fieldList"></param>
        /// <param name="tableName"></param>
        /// <param name="list"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public string CreateSumSQL(string pageNoList, string fieldList, string tableName, IList list, int limit)
        {
            StringBuilder sql = new StringBuilder();
            string[] fieldArr = fieldList.Split(',');
            if ("HL012".Equals(tableName))//无合计行
            {
                return "";
            }
            else if ("HL013".Equals(tableName))
            {
                sql.Append("select ");
            }

            StringBuilder notSumStr = new StringBuilder();
            for (int i = 0; i < fieldArr.Length; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    object[] obj = (object[])list[j];
                    if ("DW".Equals(fieldArr[i]))
                    {
                        break;
                    }
                    if (fieldArr[i].Equals(obj[0].ToString()) && tableName.Equals(obj[1].ToString()) && limit.ToString().Equals(obj[6].ToString()))
                    {
                        int decimalNumber = Convert.ToInt32(Convert.ToDouble(obj[3]));
                        int numberUnit = Convert.ToInt32(Convert.ToDouble(obj[2]));
                        if (numberUnit == 0)
                        {
                            numberUnit = 1;
                        }
                        //numberUnit = 1;
                        if (decimalNumber == -1)
                        {
                            sql.Append(fieldArr[i]).Append(",");
                            notSumStr.Append(fieldArr[i]).Append(",");
                            break;
                        }
                        else
                        {
                            sql.Append("sum(").Append(fieldArr[i]).Append(")/").Append(numberUnit).Append(" as ").Append(fieldArr[i]).Append(",");
                            break;
                        }
                    }
                }
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(" from ").Append(tableName).Append(" where PageNO in (").Append(pageNoList).Append(")");
            sql.Append(" and dw ='合计'");
            if (notSumStr.ToString() != "")
            {
                notSumStr.Remove(notSumStr.Length - 1, 1);
            }
            if (notSumStr.ToString() != "")
            {
                sql.Append(",").Append(notSumStr.ToString());
            }
            return sql.ToString();
        }
        /// <summary>
        /// 省级累计时将受灾乡镇的最大值插入到返回给客户端的JSON字符串中
        /// </summary>
        /// <param name="list">各市中受灾乡的最大值</param>
        /// <param name="jsonstr">给客户端的JSON字符串</param>
        /// <returns>插入后的字符串</returns>
        public string ReplaceSZFWX(IList list, string jsonstr)
        {
            int startIndex = 0;
            int endIndex = 0;
            for (int i = 0; i < list.Count; i++)
            {
                object[] obj = (object[])list[i];
                startIndex = jsonstr.IndexOf(obj[0].ToString().Trim());
                if (startIndex > 0)
                {
                    startIndex = jsonstr.IndexOf("SZFWX", startIndex);
                    if (startIndex > 0)
                    {
                        startIndex = jsonstr.IndexOf("'", startIndex);
                        if (startIndex > 0)
                        {
                            endIndex = jsonstr.IndexOf("'", startIndex + 1);
                        }
                    }
                }
                if (startIndex > 0 && endIndex > 0)
                {
                    jsonstr = jsonstr.Remove(startIndex + 1, endIndex - startIndex - 1);
                    jsonstr = jsonstr.Insert(startIndex + 1, Convert.ToInt32(obj[1]).ToString());
                }
            }
            return jsonstr;
        }

        public string ReplaceSZFWX(string jsonstr)
        {
            int startIndex = 0;
            int endIndex = 0;
            while (startIndex != -1)
            {
                startIndex = jsonstr.IndexOf("SZFWX", startIndex);
                if (startIndex > 0)
                {
                    startIndex = jsonstr.IndexOf("'", startIndex);
                    if (startIndex > 0)
                    {
                        endIndex = jsonstr.IndexOf("'", startIndex + 1);
                    }
                }
                if (startIndex > 0 && endIndex > 0)
                {
                    jsonstr = jsonstr.Remove(startIndex + 1, endIndex - startIndex - 1);
                    jsonstr = jsonstr.Insert(startIndex + 1, "0");
                }
            }
            return jsonstr;
        }

        /// <summary>
        /// 把数据保留几位
        /// </summary>
        /// <param name="IList">数据集合</param>
        /// <param name="number">要保留的几位</param>
        /// <param name="string">需要保留小数的列</param>
        /// <param name="tableName">表名</param>
        /// <param name="operateType">操作名称，加表：sum,减表sub</param>
        /// <returns></returns>
        public string RetainNumber(IList list, IList decimalList, string filedList, string tableName,string operateType)
        {
            StringBuilder sb = new StringBuilder();
            string[] strFiledArr = filedList.Split(',');
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                switch (tableName)
                {
                    case "HL011":
                        ReportHL011 rHl011 = (ReportHL011)list[i];
                        dic = ObjectDataToDic<ReportHL011>(rHl011);
                        break;
                    case "HL012":
                        ReportHL012 rHl012 = (ReportHL012)list[i];
                        dic = ObjectDataToDic<ReportHL012>(rHl012);
                        break;
                    case "HL013":
                        ReportHL013 rHl013 = (ReportHL013)list[i];
                        dic = ObjectDataToDic<ReportHL013>(rHl013);
                        break;
                    default://HL014
                        ReportHL014 rHl014 = (ReportHL014)list[i];
                        dic = ObjectDataToDic<ReportHL014>(rHl014);
                        break;
                }
                if (operateType=="sum")
                {
                    if (tableName == "HL012" || tableName == "HL013")
                    {
                        sb.Append("{").Append("PageNO:'").Append(dic["PageNO"] + "',");
                        sb.Append("UnitCode").Append(":'").Append(dic["UnitCode"]).Append("',");
                        sb.Append("SourcePageNo").Append(":'").Append(dic["PageNO"]).Append("',");
                        //sb.Append("TBNO:'").Append(dic["TBNO"]).Append("',");
                    }
                    else
                    {
                        sb.Append("{UnitCode:'" + dic["UnitCode"]).Append("',");
                    }
                    if (tableName == "HL012")
                    {
                        sb.Append("DataType:'" + dic["DataType"]).Append("',");
                    }
                    for (int j = 0; j < strFiledArr.Length; j++)
                    {
                        for (int k = 0; k < decimalList.Count; k++)
                        {
                            object[] objArr2 = (object[])decimalList[k];
                            if (strFiledArr[j].Equals(objArr2[0].ToString()))
                            {
                                int decimalNumber = Convert.ToInt32(objArr2[3]);
                                if (dic.ContainsKey(objArr2[0].ToString()))
                                {
                                    if (dic[objArr2[0].ToString()] != "0.000000") //去除值为0的字段
                                    {
                                        if (decimalNumber >= 0)
                                        {
                                            string unitData =
                                                String.Format("{0:N" + decimalNumber + "}",
                                                    Convert.ToDouble(dic[objArr2[0].ToString()])).Replace(",", "");
                                            sb.Append(strFiledArr[j]).Append(":'").Append(unitData).Append("',");
                                            break;
                                        }
                                        else
                                        {
                                            sb.Append(strFiledArr[j])
                                                .Append(":")
                                                .Append("'")
                                                .Append(dic[objArr2[0].ToString()])
                                                .Append("'")
                                                .Append(",");
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("},");
                }
                else if (operateType == "sub")
                {
                    //if (tableName == "HL012" || tableName == "HL013")
                    if (tableName == "HL012")
                    {
                        //sb.Append("{").Append("TBNO:'").Append(dic["TBNO"]).Append("',");
                        return "";
                    }
                    else
                    {
                        sb.Append("{UnitCode:'" + dic["UnitCode"]).Append("',");
                        for (int j = 0; j < strFiledArr.Length; j++)
                        {
                            for (int k = 0; k < decimalList.Count; k++)
                            {
                                object[] objArr2 = (object[]) decimalList[k];
                                if (strFiledArr[j].Equals(objArr2[0].ToString()))
                                {
                                    int decimalNumber = Convert.ToInt32(objArr2[3]);
                                    if (dic.ContainsKey(objArr2[0].ToString()))
                                    {
                                        if (dic[objArr2[0].ToString()]!="0.000000")//去除值为0的字段
                                        {
                                            if (decimalNumber >= 0)
                                            {
                                                string unitData = String.Format("{0:N" + decimalNumber + "}",
                                                    Convert.ToDouble(dic[objArr2[0].ToString()])).Replace(",", "");
                                                sb.Append(strFiledArr[j]).Append(":'").Append(unitData).Append("',");
                                                break;
                                            }
                                            else
                                            {
                                                sb.Append(strFiledArr[j])
                                                    .Append(":")
                                                    .Append("'")
                                                    .Append(dic[objArr2[0].ToString()])
                                                    .Append("'")
                                                    .Append(",");
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("},");
                }
                
            }
            if (sb.ToString() != "")
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 把对象的值存入dictionary中，key为字段名
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="hl">有数据的对象实体</param>
        /// <returns></returns>
        public Dictionary<string, string> ObjectDataToDic<T>(T hl)
        {
            PropertyInfo[] pfs = hl.GetType().GetProperties();//利用反射获得类的属性
            string temp = "";
            Dictionary<string, string> dataDic = new Dictionary<string, string>();
            for (int i = 0; i < pfs.Length; i++)
            {
                if (pfs[i].GetValue(hl, null) == null)
                {
                //    if (pfs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                //    {
                //        temp = "0.00000000";
                //    }
                //    else if (pfs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                //    {
                //        temp = "0";
                //    }
                //    else
                //    {
                //        temp = "";
                //    } 
                    dataDic.Add(pfs[i].Name.ToString(), "");
                }
                else
                {
                    temp = pfs[i].GetValue(hl, null).ToString();
                    dataDic.Add(pfs[i].Name.ToString(), temp);
                    //if (temp == "0")
                    //{
                    //    temp = "0.00000000";
                    //}
                }
                
            }
            return dataDic;
        }
    }
}
