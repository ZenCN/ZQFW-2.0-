using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using EntityModel;
using LogicProcessingClass.XMMZH;
using System.Web.Script.Serialization;
using DBHelper;
using EntityModel.ReportAuxiliaryModel;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：DifferentialReport.cs
// 文件功能描述：汇总与累计、对应差值表的保存修改
// 创建标识：
// 修改标识：// 修改描述：
//-------------------------------------------------------------*/
namespace LogicProcessingClass.ReportOperate
{
    public class DifferentialReport
    {

        /// <summary>
        /// 保存或更新因为累计产生的差值表(currentDiffRpt参数中如果有“PageNO”的整数值，是修改，否则是新建)
        /// </summary>
        /// <param name="limit">当前单位级别</param>
        /// <param name="unitCode">当前单位代码</param>
        /// <param name="currentDiffRpt">差值表数据</param>
        /// <param name="reportTitle">表头信息</param>
        /// <param name="SPageNO">差值表页号（aggAcc）</param>
        /// <param name="aggAcc">原aggacc表信息（可能在此修改里面的数据）</param>
        /// <param name="dataStr">原行政表数据（可能在此修改里面的数据）</param>
        /// <returns></returns>
        public string SaveOrUpdate(int limit, string unitCode, string currentDiffRpt, Hashtable reportTitle, int SPageNO, ref string aggAcc, ref string dataStr)
        {
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReportTitle rpt = null;

            currentDiffRpt = currentDiffRpt.Substring(1, currentDiffRpt.Length - 2);
            currentDiffRpt = currentDiffRpt.Replace("D_PageNo", "0");
            Hashtable data = serializer.Deserialize<Hashtable>(currentDiffRpt);
            string PageNO = data["PageNO"].ToString();
            int CurrentRptPageNo = -1;
            bool flag = false;//定义一个Data数据是否成功提交的标识
            bool isNew = false;//保存是否是新建true，修改：false
            string JsonStr = "";
            string DBname = "";
            int Count = 0;
            string D_HL012 = "";
            string D_HL013 = "";
            XMMZHClass xmm = new XMMZHClass();

            #region 判断是新建还是修改
            if (int.TryParse(PageNO, out CurrentRptPageNo))  //Update 操作
            {
                rpt = busEntity.ReportTitle.Where(t => t.PageNO == CurrentRptPageNo).SingleOrDefault();
                SaveOrUpdateReport cleanRpt = new SaveOrUpdateReport();
                if (cleanRpt.CleanTableData(busEntity, CurrentRptPageNo))
                {
                    flag = true;
                    isNew = false;//是修改
                }
                else
                {
                    flag = false;
                }
                //rep.Id = CurrentRptPageNo;
                //iSession.Delete(rep);  //是否需要删除该Rpt表？
            }
            else
            {
                rpt = new ReportTitle();
                rpt.PageNO = SPageNO;
                flag = true;
            }
            #endregion

            dataStr = dataStr.Replace("D_PageNo", rpt.PageNO.ToString());
            if (flag && data.Count > 1)
            {
                DateTime dt = DateTime.Now;
                rpt.ORD_Code = reportTitle["ORD_Code"].ToString();
                rpt.RPTType_Code = reportTitle["RPTType_Code"].ToString();
                rpt.UnitName = reportTitle["UnitName"].ToString();
                rpt.UnitCode = unitCode;
                rpt.StartDateTime = Convert.ToDateTime(reportTitle["StartDateTime"].ToString());
                rpt.WriterTime = Convert.ToDateTime(reportTitle["WriterTime"].ToString());
                rpt.EndDateTime = Convert.ToDateTime(reportTitle["EndDateTime"].ToString());
                rpt.SourceType = 6;
                rpt.SendTime = dt;
                rpt.ReceiveTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                rpt.LastUpdateTime = dt;
                #region 插入HL011--HL014的数据
                foreach (DictionaryEntry de in data)//data里有HL011-HL014的所有数据，遍历这些数据
                {
                    DBname = de.Key.ToString();
                    switch (DBname)//根据DBname不同执行不同的操作
                    {
                        case "HL011"://往HL011表中插入数据
                            object[] str011 = serializer.ConvertToType<object[]>(de.Value);//反序列化HL011，将里面的数据放入一个对象数组中去
                            for (int i = 0; i < str011.Length; i++)//循环这个对象数组
                            {
                                HL011 hl011 = serializer.ConvertToType<HL011>(str011[i]);//HL011对象接收，数组里的对象
                                hl011.ReportTitle = rpt;
                                hl011 = xmm.ToSetHL<HL011>(hl011, limit);//数量级转换
                                rpt.HL011.Add(hl011);
                            }
                            break;
                        case "HL012":
                            object[] str012 = serializer.ConvertToType<object[]>(de.Value);
                            for (int i = 0; i < str012.Length; i++)
                            {
                                HL012 hl012 = serializer.ConvertToType<HL012>(str012[i]);
                                hl012.ReportTitle = rpt;
                                rpt.HL012.Add(hl012);
                            }
                            Count += str012.Length;
                            break;
                        case "HL013":
                            object[] str013 = serializer.ConvertToType<object[]>(de.Value);
                            for (int i = 0; i < str013.Length; i++)
                            {
                                HL013 hl013 = serializer.ConvertToType<HL013>(str013[i]);
                                hl013.ReportTitle = rpt;
                                hl013 = xmm.ToSetHL<HL013>(hl013, limit);
                                rpt.HL013.Add(hl013);
                            }
                            Count += str013.Length;
                            break;
                        case "HL014":
                            object[] str014 = serializer.ConvertToType<object[]>(de.Value);
                            for (int i = 0; i < str014.Length; i++)
                            {
                                HL014 hl014 = serializer.ConvertToType<HL014>(str014[i]);
                                hl014.ReportTitle = rpt;
                                hl014 = xmm.ToSetHL<HL014>(hl014, limit);
                                rpt.HL014.Add(hl014);
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                try
                {
                    if (isNew)
                    {
                        busEntity.ReportTitle.AddObject(rpt);
                        busEntity.SaveChanges();
                    }
                    else
                    {
                        busEntity.SaveChanges();
                    }
                    flag = true;
                    #region  返回HL012,HL013的数据
                    if (Count > 0)
                    {
                        LogicProcessingClass.XMMZH.XMMZHClass ZH = new LogicProcessingClass.XMMZH.XMMZHClass();//数量级转换对象
                        ArrayList Container = new ArrayList();
                        //var hl012s = busEntity.HL012.Where(t => t.PageNO == rpt.PageNO);
                        //foreach (var hl012obj in hl012s)
                        foreach (var hl012obj in rpt.HL012)
                        {
                            LZHL012 hl012 = ZH.ConvertHLToXMMHL<LZHL012, HL012>(hl012obj, limit);
                            Container.Add(hl012);
                        }
                        D_HL012 = "dHL012:" + serializer.Serialize(Container);
                        Container.Clear();
                        foreach (var hl013obj in rpt.HL013)
                        {
                            LZHL013 hl013 = ZH.ConvertHLToXMMHL<LZHL013, HL013>(hl013obj, limit);
                            Container.Add(hl013);
                        }
                        D_HL013 = "dHL013:" + serializer.Serialize(Container);
                    }
                    #endregion
                }
                catch (Exception)
                {
                    flag = false;
                }

                if (flag)
                {
                    JsonStr = "sourceRpt:[{UnitCode:" + unitCode + ",id:" + SPageNO + "}]";
                    aggAcc = aggAcc.Remove(aggAcc.Length - 2);
                    if (aggAcc != "{AggAcc:[")
                    {
                        aggAcc += ",";
                    }
                    aggAcc += "{'SPageNO':" + SPageNO + ",'OperateType':'2','UnitCode':" + unitCode + "}]}";
                    if (D_HL012.Length > 0)
                    {
                        JsonStr += "," + D_HL012;
                    }
                    if (D_HL013.Length > 0)
                    {
                        JsonStr += "," + D_HL013;
                    }
                }
            }
            if (JsonStr.Trim().Length == 0)
            {
                JsonStr = "distinctedDiffRpt";
            }
            return JsonStr;
        }

        /// <summary>
        ///  保存或更新因为  汇总  产生的差值表(diffReport数组中的数据格式与累计中单个diffReport报表格式一样)
        /// </summary>
        /// <param name="limit">当前单位级别</param>
        /// <param name="diffReports">差值表集合</param>
        /// <param name="reportTitle">表头信息</param>
        /// <param name="dataStr">原行政表数据（可能在此修改里面的数据）</param>
        /// <param name="aggAcc">原aggacc表信息（可能在此修改里面的数据）</param>
        /// <param name="allDiffPageNOs">已经保存的差值表页号（可为空）</param>
        /// <returns></returns>
        public string SaveOrUpdate(int limit, ArrayList diffReports, Hashtable reportTitle, ref string dataStr, ref string aggAcc, string allDiffPageNOs)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BusinessEntities busEntity = Persistence.GetDbEntities(limit + 1);//传入的是当前单位级别，数据存放在下级单位，所以需要+1
            ReportTitle rpt = null;
            ReportHelpClass rptHelp = new ReportHelpClass();
            int maxSPageNO = rptHelp.FindMaxPageNO(limit);//找到最大的页号并加一

            bool CommitSuccess = false;
            string currentDiffRpt = "";
            string JsonStr = "";
            string DBname = "";
            XMMZHClass xmm = new XMMZHClass();
            string HL012D_PageNOs = "";
            string HL013D_PageNOs = "";
            string pageNOs = "";
            LogicProcessingClass.AuxiliaryClass.TransformJSON tran = new LogicProcessingClass.AuxiliaryClass.TransformJSON();
            #region 保存数据
            for (int j = 0; j < diffReports.Count; j++)
            {
                currentDiffRpt = diffReports[j].ToString();
                currentDiffRpt = currentDiffRpt.Substring(1, currentDiffRpt.Length - 2);
                if (currentDiffRpt.IndexOf("D_PageNo") >= 0)
                {
                    currentDiffRpt = currentDiffRpt.Replace("D_PageNo", "0");
                }

                Hashtable data = serializer.Deserialize<Hashtable>(currentDiffRpt);
                string PageNO = data["PageNO"].ToString();
                int currentRptPageNo = -1;
                bool flag = false;//定义一个Data数据是否成功提交的标识
                bool isNew = false;//保存是否是新建true，修改：false

                #region 判断是新建还是修改
                if (int.TryParse(PageNO, out currentRptPageNo))  //Update 操作
                {
                    rpt = busEntity.ReportTitle.Where(t => t.PageNO == currentRptPageNo).SingleOrDefault();
                    SaveOrUpdateReport cleanRpt = new SaveOrUpdateReport();
                    if (cleanRpt.CleanTableData(busEntity, currentRptPageNo))
                    {
                        flag = true;
                        isNew = false;//是修改
                    }
                    else
                    {
                        flag = false;
                        JsonStr = "错误消息：未知";
                    }
                }
                else
                {
                    rpt = new ReportTitle();
                    rpt.PageNO = ++maxSPageNO;
                    flag = true;
                }
                #endregion
                pageNOs += rpt.PageNO.ToString()+",";
                dataStr = dataStr.Replace("D_PageNo", rpt.PageNO.ToString());
                
                if (flag && data.Count > 1)
                {

                    rpt.ORD_Code = reportTitle["ORD_Code"].ToString();
                    rpt.RPTType_Code = reportTitle["RPTType_Code"].ToString();
                    rpt.UnitCode = currentDiffRpt.Substring(currentDiffRpt.IndexOf("UnitCode") + 11, 8);
                    rpt.WriterTime = Convert.ToDateTime(reportTitle["WriterTime"].ToString());  //所有的时间必须要,否则会出现SQlDateTime溢出错误
                    rpt.StartDateTime = Convert.ToDateTime(reportTitle["StartDateTime"].ToString());
                    rpt.EndDateTime = Convert.ToDateTime(reportTitle["EndDateTime"].ToString());
                    rpt.SourceType = 6;
                    rpt.SendTime = DateTime.Now;
                    rpt.ReceiveTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    rpt.LastUpdateTime = DateTime.Now;
                    currentDiffRpt = diffReports[j].ToString();  //重新赋值，因为“D_PageNO”已被替换成空串
                    if (currentDiffRpt.IndexOf("D_PageNo") >= 0)
                    {
                        ArrayList ObjectArr = GetHL012AndHL013ObjectStr(currentDiffRpt);
                        for (int k = 0; k < ObjectArr.Count; k++)
                        {
                            string ObjectStr = ObjectArr[k].ToString();
                            //将本级库中的HL012、HL013表中的PageNO保存在上级库中的对应的SPageNO中以方便删除
                            dataStr = dataStr.Replace(ObjectStr, ObjectStr.Replace("D_PageNo", rpt.PageNO.ToString()));
                        }
                    }
                    #region  HL011-HL014的数据
                    foreach (DictionaryEntry de in data)//data里有HL011-HL014的所有数据，遍历这些数据
                    {
                        DBname = de.Key.ToString();
                        switch (DBname)//根据DBname不同执行不同的操作
                        {
                            case "HL011"://往HL011表中插入数据
                                object[] str011 = serializer.ConvertToType<object[]>(de.Value);//反序列化HL011，将里面的数据放入一个对象数组中去
                                for (int i = 0; i < str011.Length; i++)//循环这个对象数组
                                {
                                    HL011 hl011 = serializer.ConvertToType<HL011>(str011[i]);//HL011对象接收，数组里的对象
                                    hl011.ReportTitle = rpt;
                                    hl011 = xmm.ToSetHL<HL011>(hl011, limit);//数量级转换
                                    rpt.HL011.Add(hl011);
                                }
                                break;
                            case "HL012":
                                object[] str012 = serializer.ConvertToType<object[]>(de.Value);
                                for (int i = 0; i < str012.Length; i++)
                                {
                                    HL012 hl012 = serializer.ConvertToType<HL012>(str012[i]);
                                    hl012.ReportTitle = rpt;
                                    rpt.HL012.Add(hl012);
                                    HL012D_PageNOs += HL012D_PageNOs.Length == 0 ? rpt.PageNO.ToString() : "," + rpt.PageNO.ToString();
                                }

                                break;
                            case "HL013":
                                object[] str013 = serializer.ConvertToType<object[]>(de.Value);
                                for (int i = 0; i < str013.Length; i++)
                                {
                                    HL013 hl013 = serializer.ConvertToType<HL013>(str013[i]);
                                    hl013.ReportTitle = rpt;
                                    hl013 = xmm.ToSetHL<HL013>(hl013, limit);
                                    rpt.HL013.Add(hl013);
                                    HL013D_PageNOs += HL013D_PageNOs.Length == 0 ? rpt.PageNO.ToString() : "," + rpt.PageNO.ToString();
                                }

                                break;
                            case "HL014":
                                object[] str014 = serializer.ConvertToType<object[]>(de.Value);
                                for (int i = 0; i < str014.Length; i++)
                                {
                                    HL014 hl014 = serializer.ConvertToType<HL014>(str014[i]);
                                    hl014.ReportTitle = rpt;
                                    hl014 = xmm.ToSetHL<HL014>(hl014, limit);
                                    rpt.HL014.Add(hl014);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                    try
                    {
                        if (isNew)
                        {
                            busEntity.ReportTitle.AddObject(rpt);
                            busEntity.SaveChanges();
                            CommitSuccess = true;
                        }
                        else
                        {
                            busEntity.SaveChanges();
                            CommitSuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        CommitSuccess = false;
                        JsonStr = "错误消息：" + ex.Message;
                    }
                }
            }
            #endregion
            if (CommitSuccess)
            {
                LogicProcessingClass.XMMZH.XMMZHClass ZH = new LogicProcessingClass.XMMZH.XMMZHClass();//数量级转换对象
                ArrayList Container = new ArrayList();
                if (HL012D_PageNOs.Length > 0)
                {
                    if (allDiffPageNOs.Length > 0)
                    {
                        HL012D_PageNOs += "," + allDiffPageNOs;
                    }
                    //query = iSession.CreateQuery(" from HL012 where PageNO in(" + HL012D_PageNOs + ")");
                    HL012D_PageNOs = "D_HL012:";
                    var hl012s = busEntity.HL012.Where("it.PageNO in {" + HL012D_PageNOs + "}");
                    foreach (var hl012obj in hl012s)
                    {
                        LZHL012 hl012 = ZH.ConvertHLToXMMHL<LZHL012, HL012>(hl012obj, limit);
                        Container.Add(hl012);
                    }
                    HL012D_PageNOs += serializer.Serialize(Container);
                }
                if (HL013D_PageNOs.Length > 0)
                {
                    if (allDiffPageNOs.Length > 0)
                    {
                        HL013D_PageNOs += "," + allDiffPageNOs;
                    }
                    Container.Clear();
                    var hl013s = busEntity.HL013.Where("it.PageNO in {" + HL013D_PageNOs + "}");
                    foreach (var hl013obj in hl013s)
                    {
                        LZHL013 hl013 = ZH.ConvertHLToXMMHL<LZHL013, HL013>(hl013obj, limit);
                        Container.Add(hl013);
                    }
                    HL013D_PageNOs = "D_HL013:";
                    Container.Clear();
                    HL013D_PageNOs += serializer.Serialize(Container);
                }

                string[] arrPageNO = pageNOs.Remove(pageNOs.Length-1).Split(',');//所有的页号
                for (int k = 0; k < diffReports.Count; k++)
                {
                    currentDiffRpt = diffReports[k].ToString();
                    string UnitCode = currentDiffRpt.Substring(currentDiffRpt.IndexOf("UnitCode") + 11, 8);
                    if (k == 0)
                    {
                        JsonStr = "sourceRpt:[{UnitCode:" + UnitCode + ",id:" + arrPageNO[k] + "}";
                        aggAcc = aggAcc.Remove(aggAcc.Length - 2);
                        if (aggAcc != "{AggAcc:[")
                        {
                            aggAcc += ",";
                        }
                        aggAcc += "{'SPageNO':" + arrPageNO[k] + ",'OperateType':'1','UnitCode':" + UnitCode + "}";
                    }
                    else
                    {
                        JsonStr += ",{UnitCode:" + UnitCode + ",id:" + arrPageNO[k] + "}";
                        aggAcc += ",{'SPageNO':" + arrPageNO[k] + ",'OperateType':'1','UnitCode':" + UnitCode + "}";
                    }
                }

                //if (CommitSuccess)
                //{
                    if (JsonStr.Length != 0)
                    {
                        JsonStr += "]";
                        aggAcc += "]}";
                        if (HL012D_PageNOs.Length > 0)
                        {
                            JsonStr += ",";
                            JsonStr += HL012D_PageNOs;
                        }
                        if (HL013D_PageNOs.Length > 0)
                        {
                            JsonStr += ",";
                            JsonStr += HL013D_PageNOs;
                        }
                    }
                    if (JsonStr.Length == 0)
                    {
                        JsonStr = "distinctedDiffRpt";
                    }
                //}
            }
            return JsonStr;
        }

        public ArrayList SplitDiffReportByString(string DiffRptStr)
        {
            ArrayList DiffReportArr = new ArrayList();
            int StartIndex = 0;
            int EndIndex = 0;
            string DiffReportStr = "";
            while (DiffRptStr.IndexOf("{", 0) >= 0)
            {
                StartIndex = DiffRptStr.IndexOf("{", 0);
                EndIndex = DiffRptStr.IndexOf("]}", StartIndex);
                DiffReportStr = DiffRptStr.Substring(StartIndex, EndIndex - StartIndex + 2);
                DiffRptStr = DiffRptStr.Remove(StartIndex, EndIndex - StartIndex + 3);
                DiffReportArr.Add(DiffReportStr);
            }
            return DiffReportArr;
        }

        public ArrayList GetHL012AndHL013ObjectStr(string DiffRptStr)
        {
            ArrayList ObjectArr = new ArrayList();
            int StartIndex = 0;
            int EndIndex = 0;
            string ObjectStr = "";
            StartIndex = DiffRptStr.IndexOf("HL012");
            EndIndex = DiffRptStr.IndexOf("HL014");
            DiffRptStr = DiffRptStr.Substring(StartIndex, EndIndex - StartIndex);
            while (DiffRptStr.IndexOf("{") >= 0)
            {
                StartIndex = DiffRptStr.IndexOf("{");
                EndIndex = DiffRptStr.IndexOf("}");
                ObjectStr = DiffRptStr.Substring(StartIndex, EndIndex - StartIndex + 1);
                ObjectArr.Add(ObjectStr);
                DiffRptStr = DiffRptStr.Remove(StartIndex, EndIndex - StartIndex + 2);
            }

            return ObjectArr;
        }

        //public string CreateSQL(string DiffData, string Tablename, string UpdateKey, int limit, int SourceType, string type)
        //{
        //    string SQL = "";
        //    string ColumnUpdatesqlStr = "";
        //    int StartIndex = -1;
        //    int EndIndex = -1;
        //    string DataStr = "";
        //    string[] ObjectStr = new string[] { "" };
        //    string[] KeyAndValue = null;
        //    string[] MiniDic = null;
        //    string FieldList = "";
        //    string ValueList = "";
        //    switch (Tablename)
        //    {
        //        case "HL011":
        //            if (DiffData.IndexOf("'HL011':[]") == -1)
        //            {
        //                StartIndex = DiffData.IndexOf("HL011") + 10;
        //                EndIndex = DiffData.IndexOf("HL012") - 4;
        //                DataStr = DiffData.Substring(StartIndex, EndIndex - StartIndex);
        //            }
        //            break;
        //        case "HL014":
        //            if (DiffData.IndexOf("'HL014':[]") == -1)
        //            {
        //                StartIndex = DiffData.IndexOf("HL014") + 10;
        //                EndIndex = DiffData.Length - 3;
        //                DataStr = DiffData.Substring(StartIndex, EndIndex - StartIndex);
        //            }
        //            break;

        //    }
        //    if (DataStr != "")
        //    {
        //        if (DataStr.IndexOf("},{") == -1)
        //        {
        //            ObjectStr[0] = DataStr;
        //        }
        //        ObjectStr = DataStr.Split(new string[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);
        //        ArrayList InvalidField = new ArrayList();
        //        bool IsInvalidField = false;
        //        LogicProcessingClass.AuxiliaryClass.GetAllFieldUnit gafu = new LogicProcessingClass.AuxiliaryClass.GetAllFieldUnit();
        //        string[] MeasureValue = { };
        //        decimal MeasureUnit = 0;
        //        int ReservedMedian = 0;
        //        for (int k = 0; k < ObjectStr.Length; k++)
        //        {
        //            InvalidField.Clear();
        //            InvalidField.AddRange(new string[] { "TBNO", "UNITCODE", "RIVERCODE", "PAGENO", "DATAORDER", "DISTRIBUTERATE", "DW", "ID" });
        //            KeyAndValue = ObjectStr[k].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //            string UnitCode = "";
        //            for (int i = 0; i < KeyAndValue.Length; i++)
        //            {
        //                MiniDic = KeyAndValue[i].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
        //                MiniDic[0] = MiniDic[0].Replace("\"", "").Trim().ToUpper();
        //                MiniDic[1] = MiniDic[1].Replace("\"", "").Trim();
        //                for (int j = 0; j < InvalidField.Count; j++)
        //                {
        //                    if (MiniDic[0] == "UNITCODE")
        //                    {
        //                        UnitCode = MiniDic[1];
        //                    }
        //                    if (MiniDic[0] == InvalidField[j].ToString())
        //                    {
        //                        IsInvalidField = true;
        //                        InvalidField.RemoveAt(j);
        //                        break;
        //                    }
        //                }
        //                if (!IsInvalidField)
        //                {
        //                    MeasureValue = gafu.GetFieldUnitArr(MiniDic[0], limit);
        //                    if (MeasureValue != null)
        //                    {
        //                        MeasureUnit = Convert.ToDecimal(MeasureValue[0]);
        //                        ReservedMedian = Convert.ToInt16(MeasureValue[1]);
        //                        if (ReservedMedian > 0)  //有保留位数
        //                        {
        //                            decimal PrimaryValue = Convert.ToDecimal(MiniDic[1]);
        //                            MiniDic[1] = (PrimaryValue * MeasureUnit).ToString();
        //                        }
        //                        else if (ReservedMedian == 0)
        //                        {
        //                            int PrimaryValue = Convert.ToInt32(MiniDic[1]);
        //                            MiniDic[1] = (PrimaryValue * Convert.ToInt32(MeasureUnit)).ToString();
        //                        }
        //                    }
        //                    switch (type)
        //                    {
        //                        case "Update":
        //                            ColumnUpdatesqlStr += MiniDic[0] + " = " + MiniDic[1] + ",";  //右边逗号“，”后面不要打空格，备注：UpdateSQL.Remove(UpdateSQL.Length - 1)
        //                            break;
        //                        case "Insert":
        //                            if (!FieldList.Contains("UnitCode") && UnitCode != "")
        //                            {
        //                                FieldList += "UnitCode,";
        //                                ValueList += "'" + UnitCode + "',";
        //                            }
        //                            FieldList += MiniDic[0] + ",";
        //                            ValueList += "'" + MiniDic[1] + "',";
        //                            break;
        //                    }

        //                }
        //                IsInvalidField = false;
        //            }
        //            switch (type)
        //            {
        //                case "Update":
        //                    SQL += "update " + Tablename + " set " + ColumnUpdatesqlStr.Remove(ColumnUpdatesqlStr.Length - 1) + " where " + (SourceType == 2 ? ("UnitCode=" + UnitCode + " and") : "") + " PageNO=" + UpdateKey + ";";  //ReportTitle.Id
        //                    break;
        //                case "Insert":
        //                    SQL += "insert into " + Tablename + "(" + FieldList + "PageNO)" + " values(" + ValueList + "'" + UpdateKey + "')";
        //                    break;
        //            }
        //        }
        //    }
        //    switch (type)
        //    {
        //        case "Update":
        //            if (SourceType == 1)
        //            {
        //                SQL = SQL.Remove(SQL.Length - 1);
        //            }
        //            break;
        //    }

        //    return SQL;
        //}

    }
}
