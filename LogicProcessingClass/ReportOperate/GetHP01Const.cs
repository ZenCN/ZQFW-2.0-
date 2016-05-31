using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using EntityModel;
using DBHelper;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：GetConst.cs
// 文件功能描述：获取蓄水表的常用数据
// 创建标识：胡汗 2013年12月26日
// 修改标识：

// 修改描述：
//-------------------------------------------------------------*/
using LogicProcessingClass.Model;
using NPOI.SS.Formula.Functions;
using System.Collections;
using EntityModel.ReportAuxiliaryModel;
using LogicProcessingClass.XMMZH;
using System.Web.Script.Serialization;
using LogicProcessingClass.AuxiliaryClass;

namespace LogicProcessingClass.ReportOperate
{
    public class GetHP01Const
    {
        private Entities entities = new Entities();
        Tools tool = new Tools();

        /// <summary>
        /// 获得蓄水表的常用数据
        /// </summary>
        /// <param name="unitCode">登录单位代码</param>
        /// <param name="rptMonth">报表结束日期</param>
        /// <returns></returns>
        public string GetHPConst(string unitCode, int rptMonth)
        {
            FXDICTEntities fxdict = (FXDICTEntities)entities.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            string temp = "";
            if (tool.GetLevelByUnitCode(unitCode) != 5)
            {
                var hjsks = from tb51 in fxdict.TB51_HunanDistrictConst select tb51;
                //if (tool.GetLevelByUnitCode(unitCode) != 4)
                //{
                var lowerUnits =
                (from tb07 in fxdict.TB07_District where tb07.pDistrictCode == unitCode select tb07.DistrictCode)
                    .ToList();
                hjsks = hjsks.Where(tb51 => lowerUnits.Contains(tb51.UnitCode));
                //}
                //else
                //{
                //    hjsks = hjsks.Where(tb51 => tb51.UnitCode == unitCode);
                //}
                var rSCodes = from tb44 in fxdict.TB44_ReservoirDistrict
                              where tb44.UnitCode == unitCode
                              select tb44.RSCode; //水库代码集
                var dxsks = from tb43 in fxdict.TB43_Reservoir
                            where tb43.RSType == 1 && rSCodes.Contains(tb43.RSCode)
                            orderby tb43.RSOrder
                            select tb43; //大型水库
                var zxsks = from tb43 in fxdict.TB43_Reservoir
                            where tb43.RSType == 2 && rSCodes.Contains(tb43.RSCode)
                            orderby tb43.RSOrder
                            select tb43; //中型水库
                string dxstr = "";
                string zxstr = "";
                string hjstr = "";
                string empty = "undefined";
                int limit = tool.GetLevelByUnitCode(unitCode);
                if (rptMonth == 0)
                {
                    rptMonth = DateTime.Now.Month;
                }
                if (rptMonth >= 4 && rptMonth <= 9) //汛期
                {
                    if (limit == 2)
                    {
                        foreach (var hjsk in hjsks)
                        {
                            hjstr += "{UnitCode:'" + hjsk.UnitCode + "',DZXKCS:" +
                                     (hjsk.DZXKCS == null ? empty : "'" + hjsk.DZXKCS + "'") + ",DZKYXSL:" +
                                     (hjsk.DZKYXSL1 == null ? empty : "'" + decimal.Round(hjsk.DZKYXSL1.Value, 2) + "'") +
                                     ",ZZXKCS:" + (hjsk.ZXKCS == null ? empty : "'" + hjsk.ZXKCS + "'") +
                                     ",ZZKYXSL:" +
                                     (hjsk.ZXKYXSL1 == null ? empty : "'" + decimal.Round(hjsk.ZXKYXSL1.Value, 2) + "'") +
                                     ",XYSKCS:" + (hjsk.XYSKCS == null ? empty : "'" + hjsk.XYSKCS + "'") + ",XYKYXS:" +
                                     (hjsk.XYKYXS1 == null ? empty : "'" + decimal.Round(hjsk.XYKYXS1.Value, 2) + "'") +
                                     ",XRSKCS:" + (hjsk.XRSKCS == null ? empty : "'" + hjsk.XRSKCS + "'") +
                                     ",XRKYXS:" +
                                     (hjsk.XRKYXS1 == null ? empty : "'" + decimal.Round(hjsk.XRKYXS1.Value, 2) + "'") +
                                     ",SPTHJCS:" +
                                     (hjsk.SPTHJCS == null ? empty : "'" + decimal.Round(hjsk.SPTHJCS.Value, 2) + "'") +
                                     ",SPTYXS:" +
                                     (hjsk.SPTYXS1 == null ? empty : "'" + decimal.Round(hjsk.SPTYXS1.Value, 2) + "'") +
                                     "},"; //得到表1表2的常用数据
                        }
                        foreach (var dxsk in dxsks)
                        {
                            dxstr += "{UnitCode:'" + dxsk.RSCode + "',DXSKMC:'" + dxsk.RSName + "',DXKYXSL:'" +
                                     decimal.Round(dxsk.ZXKYXSL1.Value, 2) + "'},"; //得到表3的常用数据
                        }
                    }
                    if (limit != 2) //登陆单位不是省级
                    {
                        foreach (var hjsk in hjsks)
                        {
                            hjstr += "{UnitCode:'" + hjsk.UnitCode + "',DZXKCS:" +
                                     (hjsk.DZXKCS == null ? empty : "'" + hjsk.DZXKCS + "'") + ",DZKYXSL:" +
                                     (hjsk.DZKYXSL1 == null ? empty : "'" + decimal.Round(hjsk.DZKYXSL1.Value * 10000, 2) + "'") +
                                     ",ZZXKCS:" + (hjsk.ZXKCS == null ? empty : "'" + hjsk.ZXKCS + "'") +
                                     ",ZZKYXSL:" +
                                     (hjsk.ZXKYXSL1 == null ? empty : "'" + decimal.Round(hjsk.ZXKYXSL1.Value * 10000, 2) + "'") +
                                     ",XYSKCS:" + (hjsk.XYSKCS == null ? empty : "'" + hjsk.XYSKCS + "'") + ",XYKYXS:" +
                                     (hjsk.XYKYXS1 == null ? empty : "'" + decimal.Round(hjsk.XYKYXS1.Value * 10000, 2) + "'") +
                                     ",XRSKCS:" + (hjsk.XRSKCS == null ? empty : "'" + hjsk.XRSKCS + "'") +
                                     ",XRKYXS:" +
                                     (hjsk.XRKYXS1 == null ? empty : "'" + decimal.Round(hjsk.XRKYXS1.Value * 10000, 2) + "'") +
                                     ",SPTHJCS:" +
                                     (hjsk.SPTHJCS == null ? empty : "'" + decimal.Round(hjsk.SPTHJCS.Value, 2) + "'") +
                                     ",SPTYXS:" +
                                     (hjsk.SPTYXS1 == null ? empty : "'" + decimal.Round(hjsk.SPTYXS1.Value * 10000, 2) + "'") +
                                     "},"; //得到表1表2的常用数据
                        }
                        foreach (var dxsk in dxsks)
                        {
                            dxstr += "{UnitCode:'" + dxsk.RSCode + "',DXSKMC:'" + dxsk.RSName + "',DXKYXSL:'" +
                                     decimal.Round(dxsk.ZXKYXSL1.Value * 10000, 2) + "'},"; //得到表3的常用数据
                        }
                        foreach (var zxsk in zxsks)
                        {
                            zxstr += "{UnitCode:'" + zxsk.RSCode + "',DXSKMC:'" + zxsk.RSName + "',DXKYXSL:'" + decimal.Round(zxsk.ZXKYXSL1.Value * 10000, 2) + "'},"; //得到表2的常用数据
                        }
                    }
                }
                else //非汛期
                {
                    if (limit == 2)
                    {
                        foreach (var hjsk in hjsks)
                        {
                            hjstr += "{UnitCode:'" + hjsk.UnitCode + "',DZXKCS:" +
                                     (hjsk.DZXKCS == null ? empty : "'" + hjsk.DZXKCS + "'") + ",DZKYXSL:" +
                                     (hjsk.DZKYXSL == null ? empty : "'" + decimal.Round(hjsk.DZKYXSL.Value, 2) + "'") +
                                     ",ZZXKCS:" + (hjsk.ZXKCS == null ? empty : "'" + hjsk.ZXKCS + "'") +
                                     ",ZZKYXSL:" +
                                     (hjsk.ZXKYXSL == null ? empty : "'" + decimal.Round(hjsk.ZXKYXSL.Value, 2) + "'") +
                                     ",XYSKCS:" + (hjsk.XYSKCS == null ? empty : "'" + hjsk.XYSKCS + "'") + ",XYKYXS:" +
                                     (hjsk.XYKYXS == null ? empty : "'" + decimal.Round(hjsk.XYKYXS.Value, 2) + "'") +
                                     ",XRSKCS:" + (hjsk.XRSKCS == null ? empty : "'" + hjsk.XRSKCS + "'") +
                                     ",XRKYXS:" +
                                     (hjsk.XRKYXS == null ? empty : "'" + decimal.Round(hjsk.XRKYXS.Value, 2) + "'") +
                                     ",SPTHJCS:" +
                                     (hjsk.SPTHJCS == null ? empty : "'" + decimal.Round(hjsk.SPTHJCS.Value, 2) + "'") +
                                     ",SPTYXS:" +
                                     (hjsk.SPTYXS == null ? empty : "'" + decimal.Round(hjsk.SPTYXS.Value, 2) + "'") +
                                     "},";
                            ; //得到表1表2的常用数据
                        }
                        foreach (var dxsk in dxsks)
                        {
                            dxstr += "{UnitCode:'" + dxsk.RSCode + "',DXSKMC:'" + dxsk.RSName + "',DXKYXSL:'" +
                                     decimal.Round(dxsk.ZXKYXSL.Value, 2) + "'},"; //得到表3的常用数据
                        }
                    }
                    if (limit != 2) //登陆单位不是省级  非省级把单位从亿立方米转换成了万立方米（乘以10000）
                    {
                        foreach (var hjsk in hjsks)
                        {
                            hjstr += "{UnitCode:'" + hjsk.UnitCode + "',DZXKCS:" +
                                     (hjsk.DZXKCS == null ? empty : "'" + hjsk.DZXKCS + "'") + ",DZKYXSL:" +
                                     (hjsk.DZKYXSL == null ? empty : "'" + decimal.Round(hjsk.DZKYXSL.Value * 10000, 2) + "'") +
                                     ",ZZXKCS:" + (hjsk.ZXKCS == null ? empty : "'" + hjsk.ZXKCS + "'") +
                                     ",ZZKYXSL:" +
                                     (hjsk.ZXKYXSL == null ? empty : "'" + decimal.Round(hjsk.ZXKYXSL.Value * 10000, 2) + "'") +
                                     ",XYSKCS:" + (hjsk.XYSKCS == null ? empty : "'" + hjsk.XYSKCS + "'") + ",XYKYXS:" +
                                     (hjsk.XYKYXS == null ? empty : "'" + decimal.Round(hjsk.XYKYXS.Value * 10000, 2) + "'") +
                                     ",XRSKCS:" + (hjsk.XRSKCS == null ? empty : "'" + hjsk.XRSKCS + "'") +
                                     ",XRKYXS:" +
                                     (hjsk.XRKYXS == null ? empty : "'" + decimal.Round(hjsk.XRKYXS.Value * 10000, 2) + "'") +
                                     ",SPTHJCS:" +
                                     (hjsk.SPTHJCS == null ? empty : "'" + decimal.Round(hjsk.SPTHJCS.Value, 2) + "'") +
                                     ",SPTYXS:" +
                                     (hjsk.SPTYXS == null ? empty : "'" + decimal.Round(hjsk.SPTYXS.Value * 10000, 2) + "'") +
                                     "},";
                            ; //得到表1表2的常用数据
                        }
                        foreach (var dxsk in dxsks)
                        {
                            dxstr += "{UnitCode:'" + dxsk.RSCode + "',DXSKMC:'" + dxsk.RSName + "',DXKYXSL:'" +
                                     decimal.Round(dxsk.ZXKYXSL.Value * 10000, 2) + "'},"; //得到表3的常用数据
                        }
                        foreach (var zxsk in zxsks)
                        {
                            zxstr += "{UnitCode:'" + zxsk.RSCode + "',DXSKMC:'" + zxsk.RSName + "',DXKYXSL:'" + decimal.Round(zxsk.ZXKYXSL.Value * 10000, 2) + "'},"; //得到表2的常用数据，
                        }
                    }
                }
                if (hjstr.Length > 0)
                {
                    hjstr.Remove(hjstr.Length - 1);
                }
                if (dxstr.Length > 0)
                {
                    dxstr.Remove(dxstr.Length - 1);
                }
                if (zxstr.Length > 0)
                {
                    zxstr.Remove(zxstr.Length - 1);
                }
                dxstr = "Large:[" + dxstr + "],";
                zxstr = "Middle:[" + zxstr + "],";
                hjstr = "Units:[" + hjstr + "]";
                temp = dxstr + zxstr + hjstr;
            }
            return temp;
        }

        public string GetReservoirByUnitCode(string unitCode)
        {
            FXDICTEntities fxdict = (FXDICTEntities)entities.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            string temp = "";
            string large = "";
            string middle = "";
            var reservoirs = (from tb44 in fxdict.TB44_ReservoirDistrict
                              where tb44.UnitCode == unitCode
                              select tb44.TB43_Reservoir).OrderBy(t => t.RSOrder);
            if (reservoirs.Count() > 0)
            {
                foreach (var reservoir in reservoirs)
                {
                    if (reservoir.RSType == 1)
                    {
                        large += "{DXSKMC:'" + reservoir.RSName + "'},";
                    }
                    else if (reservoir.RSType == 2)
                    {
                        middle += "{DXSKMC:'" + reservoir.RSName + "'},";
                    }
                }
            }
            if (middle.Length > 0)
            {
                middle = middle.Remove(middle.Length - 1);
            }
            middle = "Middle:[" + middle + "]";
            if (large.Length > 0)
            {
                large = large.Remove(large.Length - 1);
            }
            large = "Large:[" + large + "]";
            temp = middle + "," + large + ",Units:[]";
            return "Reservoir:{" + temp + "}";
        }

        public string GetLowerHPReport(int limit, int pageNO)
        {
            string jsonStr = "";
            BusinessEntities busEntity = (BusinessEntities)entities.GetPersistenceEntityByLevel(limit);
            XMMZHClass ZH = new XMMZHClass();
            JavaScriptSerializer serializer = new JavaScriptSerializer(); //创建一个序列化对象
            var rpt = busEntity.ReportTitle.Where(r => r.PageNO == pageNO).First();
            string strHP011 = "";
            string strHP012 = "";
            ArrayList units = new ArrayList();
            ArrayList large = new ArrayList();
            ArrayList middle = new ArrayList();
            if (rpt.HP011.Count() <= 0)
            {
                strHP011 = "Units:[]";
            }
            else
            {
                foreach (var hp01 in rpt.HP011.OrderBy(hl => hl.DATAORDER))
                {
                    LZHP011 xhp011 = ZH.ConvertHLToXMMHL<LZHP011, HP011>(hp01, limit);
                    units.Add(xhp011);
                }
                strHP011 = "Units:" + serializer.Serialize(units);
            }

            if (rpt.HP012.Where(h => h.DISTRIBUTERATE == "1").Count() <= 0)
            {
                strHP012 = "Large:[]";
            }
            else
            {
                foreach (var hp02 in rpt.HP012.OrderBy(hl => hl.DATAORDER))
                {
                    LZHP012 xhp012 = ZH.ConvertHLToXMMHL<LZHP012, HP012>(hp02, limit);
                    large.Add(xhp012);
                }
                strHP012 = "Large:" + serializer.Serialize(large);
            }
            if (rpt.HP012.Where(h => h.DISTRIBUTERATE == "2").Count() <= 0)
            {
                strHP012 += ",Middle:[]";
            }
            else
            {
                foreach (var hp02 in rpt.HP012.OrderBy(hl => hl.DATAORDER))
                {
                    LZHP012 xhp012 = ZH.ConvertHLToXMMHL<LZHP012, HP012>(hp02, limit);
                    large.Add(xhp012);
                }
                strHP012 += ",Middle:" + serializer.Serialize(middle);
            }
            jsonStr = "Reservoir:{" + strHP012 + "," + strHP011 + "}";
            return jsonStr;
        }


        /// <summary>蓄水表获取当前去年同期的实际蓄水量
        /// </summary>
        /// <param name="startTime">报表开始时间</param>
        /// <param name="endTime">报表结束时间</param>
        /// <param name="limit">单位级别</param>
        /// <param name="unitCode">单位代码</param>
        /// <returns></returns>
        public string GetHPLastYearSHIJIData(DateTime startTime, DateTime endTime, int limit, string unitCode)
        {
            BusinessEntities busEntity = (BusinessEntities) entities.GetPersistenceEntityByLevel(limit);
            FXDICTEntities fxdict =
                (FXDICTEntities) entities.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            var tb55 =
                fxdict.TB55_FieldDefine.Where(t => t.FieldCode == "DXKXXSL" && t.UnitCls == limit).SingleOrDefault();
                //实际蓄水的转换系数与保留小数位
            int xishu = Convert.ToInt32(tb55.DecimalCount);
            var rpt =
                busEntity.ReportTitle.Where(
                    t => t.StartDateTime == startTime && t.EndDateTime == endTime && t.UnitCode == unitCode)
                    .OrderByDescending(t => t.PageNO)
                    .AsQueryable();
            string json = "";
            if (rpt.Count() > 0)
            {
                var hp012s = from hp012 in busEntity.HP012
                    where hp012.PAGENO == rpt.FirstOrDefault().PageNO
                    select new
                    {
                        hp012.UNITCODE,
                        hp012.DXKXXSL,
                        hp012.DXSKMC,
                        hp012.DISTRIBUTERATE
                    };
                foreach (var hp012 in hp012s)
                {
                    if (hp012.DXSKMC == "合计")
                    {
                        decimal temp = Convert.ToDecimal((hp012.DXKXXSL == null ? 0 : hp012.DXKXXSL)/tb55.MeasureValue);
                        if (hp012.DISTRIBUTERATE == "1")
                        {
                            //json += "{'1':'" + decimal.Round(temp,xishu) + "',";
                            json += "{'A':'" + string.Format("{0:N" + xishu + "}", temp) + "',";
                        }
                        else if (hp012.DISTRIBUTERATE == "2")
                        {
                            if (json.Trim().Length == 0)
                            {
                                json += "{";
                            }
                            json += "'B':'" + string.Format("{0:N" + xishu + "}", temp) + "',";
                        }
                    }
                }
                foreach (var hp012 in hp012s)
                {
                    if (hp012.DXSKMC != "合计")
                    {
                        decimal temp = Convert.ToDecimal((hp012.DXKXXSL == null ? 0 : hp012.DXKXXSL)/tb55.MeasureValue);
                        json += "'" + hp012.UNITCODE + "':" + "'" + string.Format("{0:N" + xishu + "}", temp) + "',";
                    }
                }
                if (json != "")
                {
                    json = json.Remove(json.Length - 1) + "}";
                }
                else
                {
                    json = "{}";
                }
                return json;
            }
            else
            {
                return "{}";
            }
        }


        public string GetHPDate()
        {
            FXDICTEntities fxdict = new FXDICTEntities();
            var tb61s = fxdict.TB61_HunanHPDate.OrderBy(t => t.DateOrder).ToList();
            string str = "";
            foreach (var tb61 in tb61s)
            {
                str += "'" + tb61.DateSpan + "',";
            }
            if (str != "")
            {
                str = str.Remove(str.Length - 1);
            }
            return str;
        }

        /// <summary>获取某一个时间的HP011中合计行实际蓄水数据（上期蓄水量，去年同期蓄水量）
        /// </summary>
        /// <param name="endTime">时间</param>
        /// <param name="limit"></param>
        /// <param name="unitCode"></param>
        /// <returns>合计的实际蓄水量</returns>
        public string GetSQXSL(DateTime endTime, int limit, string unitCode)
        {
            BusinessEntities busEntity = (BusinessEntities)entities.GetPersistenceEntityByLevel(limit);
            var rpt = busEntity.ReportTitle.Where(t => t.EndDateTime == endTime && t.UnitCode == unitCode && t.ORD_Code == "HP01" && t.Del == 0).OrderByDescending(t => t.PageNO).AsQueryable();
            TableFieldBaseData tbBaseData = new TableFieldBaseData();
            string[] arr = { };
            decimal shuliangji = 0;
            double xiaoshu = 0;
            decimal changetemp = 0;
            string str = "0";
            if (rpt.Count() > 0)
            {
                //XXSLZJ
                var hp01s = busEntity.HP011.Where(t => t.PAGENO == rpt.FirstOrDefault().PageNO && t.DW == "合计").ToList();
                if (hp01s.Count > 0)
                {
                    arr = tbBaseData.GetFieldUnitArr("XXSLZJ", limit);
                    if (arr != null)
                    {
                        shuliangji = Convert.ToDecimal(arr[0]);
                        xiaoshu = Convert.ToDouble(arr[1]);
                    }
                    changetemp = Convert.ToDecimal(hp01s.FirstOrDefault().XXSLZJ);
                    str = String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp / shuliangji).Replace(",", ""); ;
                }
            }
            return str;
        }

        /// <summary>获取历年同期的数据
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="limit"></param>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string GetAllTQXSL(DateTime endTime, int limit, string unitCode)
        {
            BusinessEntities busEntity = (BusinessEntities)entities.GetPersistenceEntityByLevel(limit);
            string shortTime = "";
            string pageNOs = "";
            string str = "";
            Dictionary<int, DateTime> pageNOTime = new Dictionary<int, DateTime>();
            TableFieldBaseData tbBaseData = new TableFieldBaseData();
            string[] arr = { };
            decimal shuliangji = 0;
            double xiaoshu = 0;
            decimal changetemp = 0;
            IList<LNXSL> dataList = new List<LNXSL>();
            IList<LNXSL> data = new List<LNXSL>();
            if (endTime.Month > 9)
            {
                shortTime += "-" + endTime.Month;
            }
            else
            {
                shortTime += "-0" + endTime.Month;
            }
            if (endTime.Day > 9)
            {
                shortTime += "-" + endTime.Day;
            }
            else
            {
                shortTime += "-0" + endTime.Day;
            }
            string sql = "select EndDateTime, max(PAGENO) as PageNO from ReportTitle where CONVERT(varchar(100), EndDateTime, 23)  like '%" + shortTime + "' and UnitCode = '" + unitCode + "'and ORD_Code ='HP01' and del = 0 group by  EndDateTime ";  //and EndDateTime !=' " + endTime + "'
            dataList = busEntity.ExecuteStoreQuery<LNXSL>(sql).ToList();
            for (int i = 0; i < dataList.Count; i++)
            {
                pageNOTime.Add(dataList[i].PageNO, dataList[i].EndDateTime);
                pageNOs += dataList[i].PageNO + ",";
            }
            if (pageNOs != "")
            {
                var hp011s = busEntity.HP011.Where("it.PAGENO in {" + pageNOs.Remove(pageNOs.Length - 1) + "}").Where(t => t.DW == "合计").OrderBy(t => t.XXSLZJ).ToList();
                foreach (var hp011 in hp011s)
                {
                    if (Convert.ToDecimal(hp011.XXSLZJ) > 0)
                    {
                        LNXSL lnxsl = new LNXSL();
                        arr = tbBaseData.GetFieldUnitArr("XXSLZJ", limit);
                        if (arr != null)
                        {
                            shuliangji = Convert.ToDecimal(arr[0]);
                            xiaoshu = Convert.ToDouble(arr[1]);
                        }
                        changetemp = Convert.ToDecimal(hp011.XXSLZJ);
                        lnxsl.PageNO = hp011.PAGENO;
                        lnxsl.XXSLZJ =
                            Convert.ToDecimal(
                                String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp / shuliangji)
                                    .Replace(",", ""));
                        data.Add(lnxsl);
                    }
                }
            }
            for (int i = 0; i < data.Count; i++)
            {
                str += "{Year:" + pageNOTime[data[i].PageNO].Year + ",XSL:" + data[i].XXSLZJ + "},";
            }
            if (str != "")
            {
                str = str.Remove(str.Length - 1);
            }
            return str;
        }

        /// <summary>获取当前单位的所有水库
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string GetUnderReservoirCodeByUnitCode(string unitCode, int limit)
        {
            FXDICTEntities fxdict = (FXDICTEntities)new Entities().GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            var underUnits = fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode).Select(t => t.DistrictCode).ToList();
            var tb44s = (from tb44 in fxdict.TB44_ReservoirDistrict
                         //where tb44.UnitCode == unitCode || underUnits.Contains(tb44.UnitCode)
                         where underUnits.Contains(tb44.UnitCode)
                         select tb44).ToList();
            if (limit == 2)
            {
                tb44s = tb44s.Where(t => t.TB43_Reservoir.RSType == 1).ToList();
            }
            string result = "";
            string large = "";
            string middle = "";
            foreach (var tb44 in tb44s)
            {
                if (tb44.TB43_Reservoir.RSType == 1)
                {
                    large += "'" + tb44.RSCode + "':'" + tb44.UnitCode + "',";
                }
                else if (tb44.TB43_Reservoir.RSType == 2)
                {
                    middle += "'" + tb44.RSCode + "':'" + tb44.UnitCode + "',";
                }
            }
            if (large != "")
            {
                large = large.Remove(large.Length - 1);
            }
            if (middle != "")
            {
                middle = middle.Remove(middle.Length - 1);
            }
            result = "Large:{" + large + "},Middle:{" + middle + "}";
            return result;
        }

        /// <summary>上期实际蓄水（包含本单位以及所有下级单位）
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="limit"></param>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string GetSQXSAllUnits(DateTime endTime, int limit, string unitCode)
        {
            BusinessEntities busEntity = (BusinessEntities)entities.GetPersistenceEntityByLevel(limit);
            string str = "";
            TableFieldBaseData tbBaseData = new TableFieldBaseData();
            string[] arr = { };
            decimal shuliangji = 0;
            double xiaoshu = 0;
            decimal changetemp = 0;
            string XXSLZJ = "";
            if (endTime.Month == 2 && endTime.Day == 29)//闰年的也要转换成28
            {
                endTime = Convert.ToDateTime(endTime.Year + "-2-28");
            }
            var rpt = busEntity.ReportTitle.Where(t => t.EndDateTime == endTime && t.UnitCode == unitCode && t.ORD_Code == "HP01" && t.Del == 0).OrderByDescending(t => t.PageNO).AsQueryable();
            if (rpt.Count() > 0)
            {
                var hp011s = from hp01 in busEntity.HP011
                             where hp01.PAGENO == rpt.FirstOrDefault().PageNO
                             select new
                             {
                                 hp01.UNITCODE,
                                 hp01.XXSLZJ,
                                 hp01.PAGENO
                             };
                arr = tbBaseData.GetFieldUnitArr("XXSLZJ", limit);//XXSLZJ
                if (arr != null)
                {
                    shuliangji = Convert.ToDecimal(arr[0]);
                    xiaoshu = Convert.ToDouble(arr[1]);
                }
                foreach (var hp011 in hp011s)
                {
                    changetemp = Convert.ToDecimal(hp011.XXSLZJ);
                    XXSLZJ = String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp / shuliangji)
                        .Replace(",", "");
                    str += hp011.UNITCODE + ":'" + XXSLZJ + "',";
                }
            }
            if (str != "")
            {
                str = str.Remove(str.Length - 1);
            }
            return str;
        }

        /// <summary>历年同期平均实际蓄水（包含本单位以及所有下级单位，如果某年某个单位数据为0，不会进行平均）
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="limit"></param>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string GetLNTQXSAllUnits(DateTime endTime, int limit, string unitCode)
        {
            BusinessEntities busEntity = (BusinessEntities)entities.GetPersistenceEntityByLevel(limit);
            //string shortTime = "";
            string pageNO = "";
            string str = "";
            TableFieldBaseData tbBaseData = new TableFieldBaseData();
            string[] arr = { };
            decimal shuliangji = 0;
            double xiaoshu = 0;
            decimal changetemp = 0;
            string XXSLZJ = "";
            IList<LNXSL> dataList = new List<LNXSL>();
            IList<LNXSL> data = new List<LNXSL>();

            /*if (endTime.Month > 9)
            {
                shortTime += "-" + endTime.Month;
            }
            else
            {
                shortTime += "-0" + endTime.Month;
            }
            if (endTime.Day > 9)
            {
                shortTime += "-" + endTime.Day;
            }
            else
            {
                shortTime += "-0" + endTime.Day;
            }
            string sql = "select max(PAGENO) as PageNO from ReportTitle where CONVERT(varchar(50), EndDateTime, 23)  like '%" + shortTime + "' and UnitCode = '" + unitCode + "'and ORD_Code ='HP01' and del = 0 and EndDateTime !='" + endTime + "' group by  EndDateTime ";*/
            string sql = "select PageNO from ReportTitle where month(EndDateTime) = " + endTime.Month + " and day(EndDateTime) = " + endTime.Day + 
                         " and ORD_Code ='HP01' and RPTType_Code = 'XZ0' and SourceType in(0,1) and del = 0 and CopyPageNO = 0 and year(EndDateTime) >= 2011 and year(EndDateTime) <"
                         + endTime.Year + " and UnitCode = '" + unitCode + "'";
/*            sql += " like '";
            switch (limit)
            {
                case 2:
                    sql += unitCode.Substring(0, 2) + "__0000'";
                    break;
                case 3:
                    sql += unitCode.Substring(0, 4) + "__00'";
                    break;
                case 4:
                    sql += unitCode.Substring(0, 6) + "__'";
                    break;
            }*/

            dataList = busEntity.ExecuteStoreQuery<LNXSL>(sql).ToList();
            for (int i = 0; i < dataList.Count; i++)
            {
                pageNO += dataList[i].PageNO + ",";
            }
            if (pageNO != "")
            {
                string avgSQl = "select avg(XXSLZJ) as XXSLZJ, UNITCODE from HP011 where pageno in (" +
                                pageNO.Remove(pageNO.Length - 1) + ") and XXSLZJ > 0 group by UNITCODE";
                data = busEntity.ExecuteStoreQuery<LNXSL>(avgSQl).ToList();
                arr = tbBaseData.GetFieldUnitArr("XXSLZJ", limit); //XXSLZJ
                if (arr != null)
                {
                    shuliangji = Convert.ToDecimal(arr[0]);
                    xiaoshu = Convert.ToDouble(arr[1]);
                }
                foreach (var lnxsl in data)
                {
                    changetemp = Convert.ToDecimal(lnxsl.XXSLZJ);
                    XXSLZJ = String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp / shuliangji)
                        .Replace(",", "");
                    str += lnxsl.UNITCODE + ":'" + XXSLZJ + "',";
                }
            }
            if (str != "")
            {
                str = str.Remove(str.Length - 1);
            }
            return str;
        }

        public string GetLNTQXSAllUnits(DateTime endTime, string unitCode)
        {
            FXDICTEntities fxdict =
                (FXDICTEntities)entities.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            var underUnits = fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode || t.DistrictCode == unitCode).Select(t => t.DistrictCode).ToList();
            //var tb44s = (from tb44 in fxdict.TB44_ReservoirDistrict
            //    where underUnits.Contains(tb44.UnitCode );
            string shortTime = endTime.AddYears(-1).Year + "-" + endTime.Month + "-" + endTime.Day;
            string str = "";
            var tb63s = from tb63 in fxdict.TB63_HunanHisHPData
                        where underUnits.Contains(tb63.UnitCode) &&
                              tb63.RptTime == shortTime
                        select tb63;

            foreach (var lnxsl in tb63s)
            {
                str += lnxsl.UnitCode + ":'" + lnxsl.ZJLNXSL + "',";
            }

            if (str != "")
            {
                str = str.Remove(str.Length - 1);
            }
            return str;
        }

        /// <summary>获取当前单位水库,如果有下级单位，则只获取第一个下级单位的水库
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string GetNMReservoirCodeByUnitCode(string unitCode, int curLimit, string unitName)
        {
            FXDICTEntities fxdict = (FXDICTEntities)new Entities().GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            string result = "";
            string city = "";

            int[] limitSub = { 4, 6, 8 };
            string tempCode = unitCode.Substring(0, limitSub[curLimit - 2]);
            var tb07s = fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode).OrderBy(t => t.DistrictCode).ToList();
            if (tb07s.Any() && curLimit != 4)
            {
                foreach (var tb07 in tb07s)
                {
                    string unders = "";
                    tempCode = tb07.DistrictCode.Substring(0, limitSub[curLimit - 2]);
                    var tb62s = fxdict.TB62_NMReservoir.Where(t => t.RSCode.StartsWith(tempCode)).OrderBy(t=>t.UnitCode).ThenBy(t=>t.RSOrder).ToList();
                    foreach (var tb62 in tb62s)
                    {
                        unders += "{RSCode:'" + tb62.RSCode + "',RSName:'" + tb62.RSName + "',DataOrder:'" + tb62.RSOrder +
                                  "',UnitCode:'" + tb62.UnitCode + "',UnitName:'" + tb62.UnitName + "',ZKR:'" + ZeroToEmpty(Convert.ToDouble(tb62.ZKR), "0.000") + "',SKR:'" + ZeroToEmpty(Convert.ToDouble(tb62.SKR), "0.000") +
                                  "',SSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.SSW)) + "',XXSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.XXSW)) + "',ZCXSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.ZCXSW)) + "'},";
                    }
                    if (tb62s.Any())
                    {
                        result += "{City:'" + tb07.DistrictName + "',Unders:[" + unders.Remove(unders.Length - 1) +
                                  "]},";
                        if (curLimit == 2)
                        {
                            break;
                        }
                    }

                }
            }
            else
            {
                tempCode = unitCode.Substring(0, limitSub[curLimit - 2]);
                var tb62s = fxdict.TB62_NMReservoir.Where(t => t.RSCode.StartsWith(tempCode)).OrderBy(t => t.UnitCode).ThenBy(t => t.RSOrder).ToList();
                string unders = "";
                foreach (var tb62 in tb62s)
                {
                    //unders += "{RSCode:'" + tb62.RSCode + "',RSName:'" + tb62.RSName + "',DataOrder:'" + tb62.RSOrder +
                    //          "',UnitCode:'" + tb62.UnitCode + "',UnitName:'" + tb62.UnitName + "',ZKR:" + tb62.ZKR +
                    //          ",XXSW:" + tb62.XXSW + ",ZCXSW:" + tb62.ZCXSW + ",ZCXSWXYKR:" + tb62.ZCXSWXYKR + "},";
                    unders += "{RSCode:'" + tb62.RSCode + "',RSName:'" + tb62.RSName + "',DataOrder:'" + tb62.RSOrder +
                                  "',UnitCode:'" + tb62.UnitCode + "',UnitName:'" + tb62.UnitName + "',ZKR:'" + ZeroToEmpty(Convert.ToDouble(tb62.ZKR), "0.000") + "',SKR:'" + ZeroToEmpty(Convert.ToDouble(tb62.SKR), "0.000") +
                                  "',SSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.SSW)) + "',XXSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.XXSW)) + "',ZCXSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.ZCXSW)) + "'},";
                }
                if (tb62s.Any())
                {
                    result += "{City:'" + unitName + "',Unders:[" + unders.Remove(unders.Length - 1) +
                              "]},";
                }
            }
            if (result != "")
            {
                result = result.Remove(result.Length - 1).Replace("\n", "").Replace("\r", "");
            }
            return result;
        }

        /// <summary>获取某个市级单位的水库
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="curLimit"></param>
        /// <param name="unitName"></param>
        /// <returns></returns>
        public string GetNMReservoirByUnitCode(string unitCode, string unitName)
        {
            FXDICTEntities fxdict = (FXDICTEntities)new Entities().GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            string result = "";
            string tempCode = unitCode.Substring(0, 4);
            string unders = "";
            var tb62s = fxdict.TB62_NMReservoir.Where(t => t.RSCode.StartsWith(tempCode)).ToList();
            foreach (var tb62 in tb62s)
            {
                unders += "{RSCode:'" + tb62.RSCode + "',RSName:'" + tb62.RSName + "',DataOrder:'" + tb62.RSOrder +
                          "',UnitCode:'" + tb62.UnitCode + "',UnitName:'" + tb62.UnitName + "',ZKR:'" + ZeroToEmpty(Convert.ToDouble(tb62.ZKR), "0.000") + "',SKR:'" + ZeroToEmpty(Convert.ToDouble(tb62.SKR), "0.000") +
                          "',SSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.SSW)) + "',XXSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.XXSW)) + "',ZCXSW:'" + ZeroToEmpty(Convert.ToDouble(tb62.ZCXSW)) + "'},";
            }
            if (tb62s.Any())
            {
                result += "{City:'" + unitName + "',Unders:[" + unders.Remove(unders.Length - 1) +
                          "]},";
            }

            if (result != "")
            {
                result = result.Remove(result.Length - 1).Replace("\n", "").Replace("\r", "");
            }
            else
            {
                result = "{City:'" + unitName + "',Unders:[]}";
            }
            return result;
        }

        private string ZeroToEmpty(double db)
        {
            if (db > 0)
            {
                return db.ToString("0.00");
            }
            else
            {
                return "";
            }
        }

        private string ZeroToEmpty(double db, string str)
        {
            if (db > 0)
            {
                return db.ToString(str);
            }
            else
            {
                return "";
            }
        }
    }
}
