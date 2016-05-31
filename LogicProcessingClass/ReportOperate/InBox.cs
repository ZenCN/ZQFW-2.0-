using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBHelper;
using EntityModel;
using LogicProcessingClass.XMMZH;
using EntityModel.ReportAuxiliaryModel;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：InBox.cs
// 文件功能描述：接收模块或者回收站模块下报表操作
// 创建标识：胡汗 2013年11月26日
// 修改标识：

// 修改描述：
//-------------------------------------------------------------*/
using NPOI.SS.Formula.Functions;

namespace LogicProcessingClass.ReportOperate
{
    public class InBox
    {
        Entities getEntity = new Entities();

        /// <summary>
        /// 根据接收模块或者回收站模块里的检索条件查询报表
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <param name="unitCode">单位代码,</param>
        /// <param name="searchUnitLimit">表示查询的单位级别，0：本级单位，1：全部下级单位，2：某个下级单位</param>
        /// <param name="startTime">时间段的起始值，注意：在回收站模块下的时间应处理成年月日的格式</param>
        /// <param name="endTime">时间段的结束值，注意：在回收站模块下的时间应处理成年月日的格式</param>
        /// <param name="rptClassCode">报表类型，"0"表示全部类型</param>
        /// <param name="cycType">时段类型，-1：全部时段</param>
        /// <param name="receiveState">接收状态，0：下载表箱，1：拒收表箱，2：已装入，3：已删除</param>
        /// <returns></returns>
        public string SearchReport(int limit, string unitCode, int searchUnitLimit,
            DateTime startTime, DateTime endTime, string rptClassCode, int cycType, int receiveState, string type)
        {
            BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(searchUnitLimit == 0 ? limit : limit + 1);
            FXDICTEntities  dicEntity = (FXDICTEntities)getEntity.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            string strJson = "";
            endTime = endTime.AddDays(1);
            //var rptTypeCode = (from tb28 in dicEntity.TB28_OprRptType where tb28.TB16_OperateReportDefine.RC_Code
            //                        == rptClassCode select tb28.RptType_Code).ToList(); //获取上报表的操作类型
            var rpts = busEntity.ReportTitle.Where("it.SourceType in {0,1,2}").AsQueryable();
            #region 查询单位条件判断

            if (searchUnitLimit == 0) //本级单位
            {
                rpts = rpts.Where(rpt => rpt.Del == 1 && rpt.UnitCode == unitCode);
            }
            else if (searchUnitLimit == 2)//某个下级单位
            {
                rpts = rpts.Where(rpt => rpt.UnitCode == unitCode).Where(rpt => rpt.ReceiveState == receiveState && rpt.State == 3);
            }
            else//所有下级单位
            {
                var ucs = (from tb07 in dicEntity.TB07_District where tb07.pDistrictCode == unitCode select tb07.DistrictCode).ToList();
                rpts = rpts.Where(rpt => ucs.Contains(rpt.UnitCode)).Where(rpt => rpt.ReceiveState == receiveState && rpt.State == 3);
            }
            #endregion
            #region 报表类型条件判断
            if (rptClassCode != "0")//报表类型不是为全部
            {
                string[] arr = null;
                if (rptClassCode.IndexOf(",") > 0)
                {
                    arr = rptClassCode.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    List<string> list = new List<string> {"SH01", "SH02", "SH03", "SH04"};
                    rpts = rpts.Where(rpt => list.Contains(rpt.ORD_Code));
                }
                else
                {
                    rpts = rpts.Where(rpt => rpt.ORD_Code == rptClassCode);
                }
            }
            #endregion
            #region 时段类型条件判断
            if (type == "Receive")//有时段类型检索条件（表示接收模块下）
            {
                if (cycType != -1)//某个时段类型
                {
                    rpts = rpts.Where(rpt => rpt.StatisticalCycType == cycType);
                }
                /*if (receiveState != 0)//收表箱中,显示所有没有接收、拒收操作的报表
                {
                    rpts = rpts.Where(rpt => rpt.SendTime >= startTime).Where(rpt => rpt.SendTime < endTime);
                }*/
                rpts = rpts.Where(rpt => rpt.SendTime >= startTime).Where(rpt => rpt.SendTime < endTime);
            }
            else//（表示回收站模块下）
            {
                if (cycType != -1)//某个时段类型
                {
                    rpts = rpts.Where(rpt => rpt.StatisticalCycType == cycType);
                }
                rpts = rpts.Where(rpt => rpt.EndDateTime >= startTime).Where(rpt => rpt.EndDateTime < endTime);
            }
            rpts = rpts.OrderByDescending(r => r.SendTime);  //张建军 接收、回收站页面按发送时间排序
            #endregion
            #region 返回数据拼接
            foreach (var pt in rpts)
            {
                strJson += "{PageNO:'" + pt.PageNO + "',UnitCode:'" + pt.UnitCode + "',UnitName:'" + pt.UnitName + "',ORD_Code:'" + pt.ORD_Code + "',StartDateTime:'" +
                    pt.StartDateTime.Value.ToShortDateString().Replace("/", "-") + "',EndDateTime:'" + pt.EndDateTime.Value.ToShortDateString().Replace("/", "-") + "',WriterTime:'" + pt.WriterTime.Value.ToShortDateString().Replace("/", "-") + "',SendTime:'" +
                    Convert.ToDateTime(pt.SendTime).ToString("yyyy-M-d HH:mm:ss") + "',Remark:'" + pt.Remark + "',StatisticalCycType:'" + Convert.ToInt32(pt.StatisticalCycType) + "',SourceType:'" +
                    Convert.ToInt32(pt.SourceType) + "',LastUpdateTime:'" + Convert.ToDateTime(pt.LastUpdateTime).ToString("yyyy-M-d HH:mm:ss") + "',SendOperType:'" + pt.SendOperType + "'},";
            }
            if (strJson.Length != 0)
            {
                strJson = "{reportTitles:[" + strJson.Remove(strJson.Length - 1) + "]}";
            }
            else
            {
                strJson = "{reportTitles:[]}";
            }
            #endregion
            return strJson;
        }

        /// <summary>
        /// 查看报表明细
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <param name="pageNO">页号</param>
        /// <param name="searchUnitLimit">表示查询的单位的级别，0：本级单位，1：全部下级单位，2：某个下级单位</param>
        /// <returns></returns>
        public string SearchReportByPageNo(int limit, int pageNO, int searchUnitLimit, string unitCode, string rptType)
        {
            string result = "";
            string hl01 = "";
            string aff = "";
            string unders = "";
            if (rptType == "HL01")
            {
                XMMZHClass ZH = new XMMZHClass();
                BusinessEntities busEntity =
                    (BusinessEntities) getEntity.GetPersistenceEntityByLevel(searchUnitLimit == 0 ? limit : limit + 1);
                var rpts = (from report in busEntity.HL011
                    where report.PageNO == pageNO
                    orderby report.DataOrder
                    select report);

                #region 返回数据拼接

                foreach (var rpt in rpts)
                {

                    //XMMHL011 hl011 = ZH.ConvertHLToXMMHL<XMMHL011, HL011>(rpt, (searchUnitLimit == 0 ? limit : limit + 1));
                    LZHL011 hl011 = ZH.ConvertHLToXMMHL<LZHL011, HL011>(rpt, limit); //该级别是当前使用单位的级别，因为是显示给当前单位看
                    hl01 += "{DW:'" + hl011.DW + "',SZFWX:'" +
                            (rpt.SZFWX == 0 ? "" : hl011.SZFWX) + "',SZFWZ:'" +
                            (rpt.SZFWZ == 0 ? "" : hl011.SZFWZ) + "',SHMJXJ:'" +
                            (rpt.SHMJXJ == 0 ? "" : hl011.SHMJXJ) + "',SZRK:'" +
                            (rpt.SZRK == 0 ? "" : hl011.SZRK) + "',SWRK:'" +
                            (rpt.SWRK == 0 ? "" : hl011.SWRK) + "',SZRKR:'" +
                            (rpt.SZRKR == 0 ? "" : hl011.SZRKR) + "',ZYRK:'" +
                            (rpt.ZYRK == 0 ? "" : hl011.ZYRK) + "',DTFW:'" +
                            (rpt.DTFW == 0 ? "" : hl011.DTFW) + "',ZJJJZSS:'" +
                            (rpt.ZJJJZSS == 0 ? "" : hl011.ZJJJZSS) + "',SLSSZJJJSS:'" +
                            (rpt.SLSSZJJJSS == 0 ? "" : hl011.SLSSZJJJSS) + "'},";
                }
                //var rt = busEntity.ReportTitle.Where(r => r.PageNO == pageNO).First();
                //var affs = busEntity.Affix.Where(a => a.PageNO == rt.PageNO);
                var affs = busEntity.Affix.Where(a => a.PageNO == pageNO);
                foreach (var affix in affs)
                {
                    aff += "{name:'" + affix.FileName + "',url:'" + affix.DownloadURL + "'},";
                }
                if (aff.Length > 0)
                {
                    aff = aff.Remove(aff.Length - 1);
                }
                if (hl01.Length != 0)
                {
                    hl01 = hl01.Remove(hl01.Length - 1);
                }
            } 
            if (unitCode != "")
            {
                Persistence per = new Persistence();
                Dictionary<string, District> dic = per.GetLowerUnits(unitCode);
                if (dic != null && dic.Count > 0)
                {
                    foreach (District dis in dic.Values)
                    {
                        unders += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "'},";
                    }
                    unders = unders.Remove(unders.Length - 1);
                }
            }
            result = "{reportDetails:[" + hl01 + "],affix:[" + aff + "],underUnits:[" + unders + "]}";
            #endregion
            return result;
        }

        /// <summary>
        /// 根据页号操作报表
        /// </summary>
        /// <param name="limit">登录单位级别</param>
        /// <param name="pageNOs">页号，页号与页号之间用逗号隔开</param>
        /// <param name="searchUnitLimit">查询单位级别</param>
        /// <param name="handleType">操作类型，-1：恢复，1：拒收，2：装入，3：删除</param>
        public string HandleReport(int limit, string pageNOs, int searchUnitLimit, int handleType,
            string refuseUnitCodes, string time)
        {
            BusinessEntities busEntity =
                (BusinessEntities) getEntity.GetPersistenceEntityByLevel(searchUnitLimit == 0 ? limit : limit + 1);
            var rpts = busEntity.ReportTitle.Where("it.PageNO in{" + pageNOs + "}").ToList();
            string temp = "1";
            string mgs = "";
            #region 国家防总

            int bsLastState = 3; //传入国家防总CS版本数据处理中需要的BS状态值，该状态值会转换成CS对应的状态值
            int csLimit = 0; //需要传入CS数据库处理的单位级别
            if (searchUnitLimit != 0) //查看的都是下级单位的报表，连接数据库时的参数（单位级别）要加1
            {
                csLimit += 1;
            }
            CSReportOperate csRpt = new CSReportOperate();
            bool csUpdateFlag = true; //国家防总CS数据库更新状态

            #endregion

            #region 恢复操作

            if (handleType == -1) //恢复操作
            {
                foreach (var rpt in rpts)
                {
                    if (rpt.ReceiveState == 1) //从拒收表箱里恢复
                    {
                        rpt.ReceiveState = 0;
                        bsLastState = 0;
                    }
                    else if (rpt.Del == 1 || rpt.ReceiveState == 3) //从回收站里恢复
                    {
                        if (searchUnitLimit == 0)
                        {
                            rpt.Del = 0;
                            bsLastState = 0;
                        }
                        else
                        {
                            if (rpt.RBType==null ||rpt.ReceiveState == rpt.RBType)
                            {
                                rpt.ReceiveState = 0;
                                rpt.RBType = 0;
                            }
                            else
                            {
                                rpt.ReceiveState = rpt.RBType;
                            }
                            bsLastState = Convert.ToInt32(rpt.RBType);
                        }
                    }

                    #region/****************国家防总CS数据库的下级报表操作*****Start********************/

                    if (limit == 0)
                    {
                        try
                        {
                            if (!csRpt.InboxOperateRpt(csLimit, rpt.PageNO, handleType, bsLastState))
                            {
                                csUpdateFlag = false;
                                throw new Exception("CS数据库更新出错！");
                            }
                        }
                        catch (Exception e)
                        {
                            csUpdateFlag = false;
                            return "错误消息：" + e.Message;
                        }
                    }

                    #endregion/**************国家防总CS数据库的下级报表操作 end************************/
                }
            }
                #endregion
                #region 其他操作

            else //除去恢复的其他操作
            {
                foreach (var rpt in rpts)
                {
                    if (handleType == 3) //删除操作
                    {
                        //汇总表或累计表是可以删除的，是参与了汇总或者累计，就不能删除 -- 刘志
                        //if (rpt.SourceType == 1 || rpt.SourceType == 2)//该报表是汇总表或者累计表
                        //{
                        //    temp = "该报表是汇总表或者累计表，不能删除。";
                        //}
                        //else
                        //{
                        BusinessEntities curBusEntity =
                (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);//当前登录单位中，判断是否参与了上级汇总
                        var curAaggAccRecords =
                           curBusEntity.AggAccRecord.Where(
                               aggAccRecord => aggAccRecord.SPageNO == rpt.PageNO && aggAccRecord.OperateType == 1);//1为汇总
                        var aggAccRecords =
                            busEntity.AggAccRecord.Where(
                                aggAccRecord => aggAccRecord.SPageNO == rpt.PageNO && aggAccRecord.OperateType == 2);//查找该报表参加自己单位的累计
                        if ((aggAccRecords.Any() && rpt.CopyPageNO == 0)||curAaggAccRecords.Any()) //参加累计并且没有副本或参与了汇总
                        {
                            //mgs += rpt.UnitName+",";
                            temp = "有报表参加过累计并且没有副本或参加了汇总，没有进行删除。";
                        }
                        else
                        {
                            rpt.RBType = rpt.ReceiveState;
                            rpt.ReceiveState = handleType;
                        }
                        //}
                    }
                    else if (handleType == 2) //装入的时候，需要记录当前的装入时间，否则在树形列表中“昨天及今天装入的报表”可能查看不到
                    {
                        rpt.ReceiveTime = DateTime.Now;
                        rpt.ReceiveState = handleType;
                    }
                    else
                    {
                        rpt.ReceiveTime = DateTime.Now;
                        rpt.ReceiveState = handleType;
                    }

                    #region/****************国家防总CS数据库的下级报表操作*****Start********************/

                    if (limit == 0)
                    {
                        try
                        {
                            if (!csRpt.InboxOperateRpt(csLimit, rpt.PageNO, handleType, bsLastState))
                            {
                                csUpdateFlag = false;
                                throw new Exception("CS数据库更新出错！");
                            }
                        }
                        catch (Exception e)
                        {
                            csUpdateFlag = false;
                            return "错误消息：" + e.Message;
                        }
                    }

                    #endregion/**************国家防总CS数据库的下级报表操作 end************************/
                }
            }

            #endregion

            if (csUpdateFlag) //国家防总CS数据库更新状态
            {
                try
                {
                    busEntity.SaveChanges();
                    //if (mgs!="")
                    //{
                    //    temp +=  mgs.Remove(mgs.Length-1) + "的报表参加过累计并且没有副本或参加了汇总，没有进行删除。";
                    //}
                }
                catch (Exception e)
                {
                    temp = "错误消息：" + e.Message+e.InnerException;
                }
            }
            else
            {
                temp = "CS数据库更新出错！";
            }

            if (refuseUnitCodes != "" && time != "") //有拒收的单位代码
            {
                var unitcodes = refuseUnitCodes.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                var timeArr = time.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                var unitname = HttpUtility.UrlDecode(HttpContext.Current.Request["unitname"]);
                limit = limit == 0 ? 2 : (limit + 1);

                /*for (int i = 0; i < unitcodes.Length; i++)
                {
                    Message.Record(unitcodes[i], limit, unitname + ":" + timeArr[i], "2");
                }*/
            }

            return temp;
        }

        /// <summary>
        /// 回收站模块下删除报表
        /// </summary>
        /// <param name="limit">登录单位级别</param>
        /// <param name="pageNOs">要删除的页号，页号与页号之间用逗号隔开</param>
        /// <param name="searchUnitLimit">表示查询的单位的级别，0：本级单位，1：全部下级单位，2：某个下级单位</param>
        /// <returns></returns>
        public string DeleteReport(int limit, string pageNOs, int searchUnitLimit)
        {
            BusinessEntities busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(searchUnitLimit == 0 ? limit : limit + 1);
            //BusinessEntities lowerBusEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(searchUnitLimit == 0 ? limit + 1 : limit + 2);
            BusinessEntities prvEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(2);//从省级库中找出CS数据库中报表的页号

            var test = busEntity.ReportTitle.Where("it.AssociatedPageNO in {" + pageNOs + "}").Select(rpt => rpt.PageNO);
            Array arr = test.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                pageNOs += "," + arr.GetValue(i).ToString();
            }
            var rpts = busEntity.ReportTitle.Where("it.PageNO in {" + pageNOs + "}");

            CSReportOperate csRpt = new CSReportOperate();
            bool csDeleteFlag = true;//国家防总CS数据库更新状态

            //#region 删除本级单位报表
            //if (searchUnitLimit == 0)//要删除的是本级单位报表
            //{
                foreach (var rpt in rpts)
                {
                    #region/****************国家防总CS数据库的下级报表操作*****Start********************/
                    if (limit == 0)
                    {
                        int csPageNO = 0;
                        try
                        {
                            csPageNO =
                               Convert.ToInt32(prvEntity.ReportTitle.Where(t => t.PageNO == rpt.PageNO).SingleOrDefault().CSPageNO);
                            if (!csRpt.DeleteCSRpt(csPageNO,0))
                            {
                                csDeleteFlag = false;
                                throw new Exception("CS数据库删除出错！");
                            }
                        }
                        catch (Exception e)
                        {
                            return "错误消息：" + e.Message;
                        }
                    }
                    #endregion/**************国家防总CS数据库的下级报表操作 end************************/

                    //#region 该报表是汇总表，判断汇总成该报表的那些报表是否是差值表，是则删除它以及其相关的记录
                    //if (rpt.SourceType == 1)//该报表是汇总表
                    //{
                    //    DeleteDifferenceReport(lowerBusEntity, rpt);
                    //    lowerBusEntity.SaveChanges();
                    //}
                    //#endregion

                    //#region 该报表是累计表，判断累计成该报表的那些报表是否是差值表，是则删除它以及其相关的记录
                    //else if (rpt.SourceType == 2)//该报表时累计表
                    //{
                    //    DeleteDifferenceReport(busEntity, rpt);
                    //}
                    //#endregion

                    #region 删除该报表以及其相关的记录
                    DeleteForeign(busEntity, rpt);
                    busEntity.DeleteObject(rpt);
                    #endregion
                //}
            }
            //#endregion
            //#region 删除下级单位报表
            //else//要删除的是下级单位报表
            //{
            //    foreach (var rpt in rpts)
            //    {
            //        //#region 该报表是汇总表，判断汇总成该报表的那些报表是否是差值表，是则删除它以及其相关的记录
            //        //if (rpt.SourceType == 1)//该报表是汇总表
            //        //{
            //        //    DeleteDifferenceReport(lowerBusEntity, rpt);
            //        //    lowerBusEntity.SaveChanges();
            //        //}
            //        //#endregion

            //        //#region 该报表是累计表，判断累计成该报表的那些报表是否是差值表，是则删除它以及其相关的记录
            //        //else if (rpt.SourceType == 2)//该报表时累计表
            //        //{
            //        //    DeleteDifferenceReport(busEntity, rpt);
            //        //}
            //        //#endregion

            //        //#region  判断该报表本身是否参加过累计，没有则删除，有则改变状态值
            //        //var aggAccRecords = busEntity.AggAccRecord.Where(aggAccRecord => aggAccRecord.SPageNO == rpt.PageNO && aggAccRecord.OperateType == 2);//查找该报表参加过累计的
            //        //if (!aggAccRecords.Any() && rpt.CopyPageNO > 0)//没有累计并且有副本
            //        //{
            //            //DeleteForeign(busEntity, rpt);
            //            //busEntity.DeleteObject(rpt);
            //        //}
            //        //else
            //        //{
            //        //    rpt.ReceiveState = 4;
            //        //}
            //        //#endregion
            //    }
            //}
            //#endregion

            string temp = "1";
            if (csDeleteFlag) //国家防总CS数据库删除报表的状态
            {
                try
                {
                    busEntity.SaveChanges();
                }
                catch (Exception e)
                {
                    temp = "错误消息：" + e.Message;
                }
            }
            else
            {
                temp = "CS数据库删除出错！";
            }
            return temp;
        }

        /// <summary>
        /// 删除外键表的记录
        /// </summary>
        /// <param name="busEntity"></param>
        /// <param name="rpt"></param>
        private void DeleteForeign(BusinessEntities busEntity, ReportTitle rpt)
        {
            DeleteObject(busEntity, rpt.HL011.ToList<object>());
            DeleteObject(busEntity, rpt.HL012.ToList<object>());
            DeleteObject(busEntity, rpt.HL013.ToList<object>());
            DeleteObject(busEntity, rpt.HL014.ToList<object>());
            DeleteObject(busEntity, rpt.HP011.ToList<object>());
            DeleteObject(busEntity, rpt.HP012.ToList<object>());
            DeleteObject(busEntity, rpt.AggAccRecord.ToList<object>());
            DeleteObject(busEntity, rpt.Affix.ToList<object>());
        }

        /// <summary>
        /// 循环删除IList里的对象
        /// </summary>
        /// <param name="busEntity"></param>
        /// <param name="lists">linq查询出来的IList集合</param>
        private void DeleteObject(BusinessEntities busEntity, IList<object> lists)
        {
            foreach (var list in lists)
            {
                busEntity.DeleteObject(list);
            }
        }

        /// <summary>
        /// 删除参加某个报表的累计或汇总并且是差值表的报表及其相关记录
        /// </summary>
        /// <param name="busEntity"></param>
        /// <param name="reportTitle">某个报表</param>
        private void DeleteDifferenceReport(BusinessEntities busEntity, ReportTitle reportTitle)
        {
            var sPageNOs = from report in busEntity.AggAccRecord where report.PageNo == reportTitle.PageNO select report.SPageNO;
            foreach (var sPageNO in sPageNOs)
            {
                var reports = busEntity.ReportTitle.Where(report => report.PageNO == sPageNO);//得到所有参该累计或汇总的报表
                foreach (var report in reports)
                {
                    if (report.SourceType == 6)//报表是差值表
                    {
                        DeleteForeign(busEntity, report);
                        busEntity.DeleteObject(report);
                    }
                }
            }
        }
    }
}
