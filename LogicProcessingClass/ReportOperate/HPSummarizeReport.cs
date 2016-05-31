using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using DBHelper;
using EntityModel;
using EntityModel.RepeatModel;
using EntityModel.ReportAuxiliaryModel;
using LogicProcessingClass.AuxiliaryClass;
using NPOI.SS.Formula.Functions;

namespace LogicProcessingClass.ReportOperate
{
    public class HPSummarizeReport
    {
        private BusinessEntities busEntities = null;

        /// <summary>初始化
        /// </summary>
        /// <param name="limit">当前单位级别</param>
        public HPSummarizeReport(int limit)
        {
            Entities getEntities = new Entities();
            busEntities = (BusinessEntities)getEntities.GetPersistenceEntityByLevel(limit+1);//查看的是下级的
        }

        /// <summary> 汇总数据
        /// </summary>
        /// <param name="pageNoList">页号集合</param>
        /// <param name="limit">当前单位级别</param>
        /// <returns></returns>
        public string GetSummaryReportFormData(string pageNoList, int limit)
        {
            string jsonStr = "{";

            TableFieldBaseData table = new TableFieldBaseData();
            IList fieldList = new List<object[]>();
            IList tempList = table.GetAllFieldUnitIList(limit);
            for (int i = 0; i < tempList.Count; i++)
            {
                TB55_FieldDefine tb55 = (TB55_FieldDefine)tempList[i];
                if (limit == tb55.UnitCls)
                {
                    object[] obj = new object[7];
                    obj[0] = tb55.FieldCode;
                    obj[1] = tb55.TD_TabCode;
                    obj[2] = tb55.MeasureValue;
                    obj[3] = tb55.DecimalCount;
                    obj[4] = tb55.MeasureName;
                    obj[5] = tb55.InputRemark;
                    obj[6] = tb55.UnitCls;
                    fieldList.Add(obj);
                }
            }
            jsonStr += GetReportFormList(pageNoList, fieldList, limit);
            jsonStr += "}";
            return jsonStr;
        }

        /// <summary>
        /// 根据页号集合，查询对应字段数据集合
        /// </summary>
        /// <param name="pageNoList">页号集合</param>
        /// <param name="fieldLists">字段集合,</param>
        /// <param name="dataBaseName">数据库名</param>
        /// <param name="limit">单位级别</param>
        /// <returns></returns>
        public string GetReportFormList(string pageNoList, IList fieldLists, int limit)
        {
            StringBuilder sbJSON = new StringBuilder();
            ArrayList filedList = QueryTableFiled(fieldLists);
            IList dataList = null;
            ArrayList arry = new ArrayList();
            string sql = "";
            for (int i = 0; i < filedList.Count; i++)
            {
                string tableName = "";
                switch (i)
                {
                    case 0:
                        tableName = "HP011";
                        break;
                    case 1:
                        tableName = "HP012";
                        break;
                }
                sql = CreateSQL(pageNoList, filedList[i].ToString(), tableName, fieldLists, limit);  //创建SQL语句  limit
                switch (tableName)
                {
                    case "HP011":
                        dataList = busEntities.ExecuteStoreQuery<ReportHP011>(sql).ToList();
                        break;
                    case "HP012":
                        dataList = busEntities.ExecuteStoreQuery<ReportHP012>(sql).ToList();
                        break;
                }
                if (tableName == "HP011")
                {
                    sbJSON.Append(tableName)
                        .Append(":[")
                        .Append(RetainNumber(dataList, fieldLists, filedList[i].ToString(), tableName))
                        .Append("],");
                }
                else if (tableName == "HP012")
                {
                    ArrayList larDataList= new ArrayList();
                    ArrayList midDataList = new ArrayList();
                    foreach (var data in dataList)
                    {
                        ReportHP012 rHp012 = (ReportHP012)data;
                        if (rHp012.DISTRIBUTERATE=="1")
                        {
                            larDataList.Add(data);
                        }
                        else if (rHp012.DISTRIBUTERATE == "2")
                        {
                            midDataList.Add(data);
                        }
                    }
                    //strHP012 = "HP012:{Real:{Large:[],Middle:[]}}";
                    sbJSON.Append(tableName)
                        .Append(":{Real:{Large:[")
                        .Append(RetainNumber(larDataList, fieldLists, filedList[i].ToString(), tableName))
                        .Append("],Middle:[")
                        .Append(RetainNumber(midDataList, fieldLists, filedList[i].ToString(), tableName))
                        .Append("]}},");
                }
            }
            return sbJSON.Remove(sbJSON.Length - 1, 1).ToString(); 
        }


        /// <summary>
        /// 生成要返回的数据列
        /// </summary>
        /// <param name="list">传入数据列集合</param>
        /// <returns></returns>
        public ArrayList QueryTableFiled(IList list)
        {
            ArrayList alist = new ArrayList();
            StringBuilder sbHP011 = new StringBuilder();
            StringBuilder sbHP012 = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                object[] objArr = (object[])list[i];
                int number = Convert.ToInt32(Convert.ToDouble(objArr[2]));
                if ("DW".Equals(objArr[0].ToString()) || "DXSKMC".Equals(objArr[0].ToString()))
                {
                    continue;
                }
                switch (objArr[1].ToString())
                {
                    case "HP011":
                        sbHP011.Append(objArr[0]).Append(",");
                        break;
                    case "HP012":
                        sbHP012.Append(objArr[0]).Append(",");
                        break;
                }
            }
            if (sbHP011.Length > 0)
            {
                sbHP011.Remove(sbHP011.Length - 1, 1);
            }
            if (sbHP012.Length > 0)
            {
                sbHP012.Remove(sbHP012.Length - 1, 1);
            }
            alist.Add(sbHP011);
            alist.Add(sbHP012);
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
        public string CreateSQL(string pageNoList, string fieldList, string tableName, IList list, int limit)
        {
            StringBuilder hsql = new StringBuilder();
            string[] fieldArr = fieldList.Split(',');
            if ("HP011".Equals(tableName))
            {
                hsql.Append("select UnitCode,DW,");
            }
            else
            {
                hsql.Append("select UnitCode,DXSKMC,DISTRIBUTERATE,");
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
            //if ("HL012".Equals(tableName) || "HL013".Equals(tableName))
            //{
            //    if ("HL013".Equals(tableName))
            //    {
            //        hsql.Append(" and dw <> '合计' ");
            //    }

            //    hsql.Append(" group by UnitCode,DW,TBNO,PageNO ");
            //}
            //else
            //{
            if ("HP011".Equals(tableName))
            {
                hsql.Append(" and dw='合计' group by UnitCode,DW order by UnitCode");
            }
            else
            {
                hsql.Append(" and DXSKMC !='合计' group by UnitCode,DXSKMC,DISTRIBUTERATE order by DISTRIBUTERATE ");
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

        /// <summary>
        /// 把数据保留几位
        /// </summary>
        /// <param name="IList">数据集合</param>
        /// <param name="number">要保留的几位</param>
        /// <param name="string">需要保留小数的列</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public string RetainNumber(IList list, IList decimalList, string filedList, string tableName)
        {
            string needFiles = "XXSLZJ,XZKYSL,DZKXXSL,DXKKYSL,ZZKXXSL,ZXKKYSL,XYKXXS,XYKKYS,XRKXXS,XRKKYS,SPTXXS,SPTKYS,DXKXXSL,DXKKYS";
            StringBuilder sb = new StringBuilder();
            string[] strFiledArr = filedList.Split(',');
            for (int i = 0; i < list.Count; i++)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                switch (tableName)
                {
                    case "HP011":
                        ReportHP011 rHp011 = (ReportHP011)list[i];
                        dic = ObjectDataToDic<ReportHP011>(rHp011);
                        break;
                    case "HP012":
                        ReportHP012 rHp012 = (ReportHP012)list[i];
                        dic = ObjectDataToDic<ReportHP012>(rHp012);
                        break;
                }

                sb.Append("{UnitCode:'" + dic["UNITCODE"]).Append("',");

                for (int j = 0; j < strFiledArr.Length; j++)
                {
                    for (int k = 0; k < decimalList.Count; k++)
                    {
                        object[] objArr2 = (object[])decimalList[k];
                        if (strFiledArr[j].Equals(objArr2[0].ToString()) && needFiles.Contains(strFiledArr[j]))
                        {
                            int decimalNumber = Convert.ToInt32(objArr2[3]);
                            if (dic.ContainsKey(objArr2[0].ToString()))
                            {
                                if (decimalNumber >= 0)
                                {
                                    string unitData = String.Format("{0:N" + decimalNumber + "}", Convert.ToDouble(dic[objArr2[0].ToString()])).Replace(",","");
                                    sb.Append(strFiledArr[j]).Append(":'").Append(unitData).Append("',");
                                    break;
                                }
                                else
                                {
                                    sb.Append(strFiledArr[j]).Append(":").Append("'").Append(dic[objArr2[0].ToString()]).Append("'").Append(",");
                                    break;
                                }
                            }
                        }
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("},");
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
                    if (pfs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                    {
                        temp = "0.00000000";
                    }
                    else
                    {
                        temp = "0";
                    }
                    //temp = "0";
                }
                else
                {
                    temp = pfs[i].GetValue(hl, null).ToString();
                    if (temp == "0")
                    {
                        temp = "0.00000000";
                    }
                }
                dataDic.Add(pfs[i].Name.ToString(), temp);
            }
            return dataDic;
        }
    }
}
