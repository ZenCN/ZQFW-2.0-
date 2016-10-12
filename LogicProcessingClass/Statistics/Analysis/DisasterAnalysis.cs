using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using System.Collections;
using System.Web.Script.Serialization;
using DBHelper;
using LogicProcessingClass.AuxiliaryClass;

namespace LogicProcessingClass.Statistics
{
    public class DisasterAnalysis
    {


        #region 刘磊 根据开始时间、灾情类型、报表类型、月份查询出对应的报表集合
        ///修改 胡蔚星20140227
        ///修改 汪明星20150123
        /// <summary>根据开始时间、灾情类型、报表类型、月份查询出对应的报表集合
        /// 
        /// </summary>
        /// <param name="beginYear">开始年份</param>
        /// <param name="endYear">结束年份</param>
        /// <param name="beginMonth">开始月份</param>
        /// <param name="beginDay">开始日期</param>
        /// <param name="bDateRange">开始日期范围</param>
        /// <param name="endMonth">结束月份</param>
        /// <param name="endDay">结束日期</param>
        /// <param name="eDateRange">结束日期范围</param>
        /// <param name="cycTypes">报表类型(月报、过程报等等)</param>
        /// <param name="pageNum">当前页码</param>
        /// <param name="pageLineNum">每页显示行数</param>
        /// <returns></returns>
        public List<string> DisasterAnalysisConditionQuery(int beginYear, int endYear, string beginMonth, string beginDay, string bDateRange,
             string endMonth, string endDay, string eDateRange, string cycTypes, int pageNum, int pageLineNum, BusinessEntities businessFXPRV, FXDICTEntities fxdict)
        {
            //List<string> result = new List<string>();
            List<string> mxresult = new List<string>();
            if (beginYear > endYear || cycTypes == "")
                return mxresult;
            string[] strCycTypeArr = cycTypes.Split(',');
            int[] cycTypeArr = Array.ConvertAll<string, int>(strCycTypeArr, strCycType => Convert.ToInt32(strCycType));
            List<TB14_Cyc> remarks = fxdict.TB14_Cyc.ToList();
            //List<ReportTitle> repottitle =  businessFXPRV.ReportTitle.ToList();
            List<ReportTitle> repottitle = (from t in businessFXPRV.ReportTitle
                                            where t.Del == 0
                                            select t).ToList();
            //for (int i = endYear; i >= beginYear; i--)
            //{
            //string startTime = i.ToString() + "-" + beginMonth + "-" + ((!isLeapYear(i) && beginDay == "29") ? "28" : beginDay);
            //string endTime = i.ToString() + "-" + endMonth + "-" + ((!isLeapYear(i) && endDay == "29") ? "28" : endDay);
            string startTime = beginYear.ToString() + "-" + beginMonth + "-" + beginDay;
            string endTime = endYear.ToString() + "-" + endMonth + "-" + endDay;
            DateTime startDateTime = Convert.ToDateTime(startTime);
            DateTime endDateTime = Convert.ToDateTime(endTime);
            DateTime startDateTime1 = startDateTime.AddDays(-Convert.ToInt32(bDateRange));
            //DateTime startDateTime2 = startDateTime.AddDays(Convert.ToInt32(bDateRange) + 1);
            DateTime endDateTime1 = endDateTime.AddDays(-Convert.ToInt32(eDateRange));
            //DateTime endDateTime2 = endDateTime.AddDays(Convert.ToInt32(eDateRange) + 1);

            DateTime dt = DateTime.Now;
            var ret = new List<string>();
            var ilist = new List<dc>();
            var list = repottitle.Where(t => (t.EndDateTime >= startDateTime1) && (t.EndDateTime <= endDateTime1) && t.RPTType_Code == "XZ0" && t.CopyPageNO == 0).Join(remarks, rt => rt.StatisticalCycType, rm => rm.CycType, (rt, rm) => new dc
            {
                PageNO = rt.PageNO,
                UnitName = rt.UnitName,
                UnitCode = rt.UnitCode,
                Starttime = Convert.ToDateTime(rt.StartDateTime).ToString("yyyy/MM/dd"),
                Endtime = Convert.ToDateTime(rt.EndDateTime).ToString("yyyy/MM/dd"),
                CycType = (System.Int32?)rt.StatisticalCycType,
                CycTypeName = rm.Remark
            }).ToList();
            cycTypeArr.ToList().ForEach(s => { ilist.AddRange(list.Where(t => t.CycType == s)); });
            var ttt = ilist.OrderByDescending(t => t.Endtime).ThenByDescending(t => t.PageNO).ToList();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            mxresult.Add(serializer.Serialize(ttt));

            //var data = (from t in repottitle
            //            from c in cycTypeArr
            //            join rk in remarks on t.StatisticalCycType equals rk.CycType
            //            where ((t.EndDateTime >= startDateTime1) && (t.EndDateTime <= endDateTime1) && t.RPTType_Code == "XZ0" && t.StatisticalCycType == c && t.CopyPageNO == 0)//标记
            //            group t by new
            //            {
            //                t.PageNO,
            //                t.StartDateTime,
            //                t.UnitName,
            //                t.UnitCode,
            //                t.EndDateTime,
            //                t.StatisticalCycType,
            //                rk.Remark

            //            } into g
            //            orderby g.Key.EndDateTime descending, g.Key.PageNO descending
            //            select new
            //            {
            //                PageNO = (System.Int32?)g.Max(p => p.PageNO),
            //                g.Key.UnitName,
            //                g.Key.UnitCode,
            //                Starttime = Convert.ToDateTime(g.Key.StartDateTime).ToString("yyyy/MM/dd"),
            //                Endtime = Convert.ToDateTime(g.Key.EndDateTime).ToString("yyyy/MM/dd"),
            //                CycType = (System.Int32?)g.Key.StatisticalCycType,
            //                CycTypeName = g.Key.Remark
            //            }).ToList();
            ////JavaScriptSerializer serializer = new JavaScriptSerializer();
            //result.Add(serializer.Serialize(data));

            //  }
            return mxresult;
        }
        /// <summary>
        /// 接受DisasterAnalysisConditionQuery数据模型
        /// </summary>
        public class dc
        {
            public int PageNO { get; set; }
            public string UnitName { get; set; }
            public string UnitCode { get; set; }
            public string Starttime { get; set; }
            public string Endtime { get; set; }
            public int? CycType { get; set; }
            public string CycTypeName { get; set; }
        }

        /// <summary>
        /// 判断是否为闰年 -- 未使用
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public bool isLeapYear(int year)
        {
            if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 灾情分析模块树形菜单
        /// </summary>
        /// <param name="pageNum">当前页码</param>
        /// <param name="pageLineNum">每页显示行数</param>
        /// <param name="firstTime">是否是第一次进入系统</param>
        /// <returns>返回灾情分析树形菜单JSON</returns>
        public string DisasterAnalysisTrueNodeData(int pageNum, int pageLineNum, bool firstTime, BusinessEntities businessFXPRV)
        {
            int maxPageNum = 0;
            maxPageNum = QueryMaxPageNum(businessFXPRV);
            if (maxPageNum == 0)
            {
                return "";//表示没有数据,直接返回空给前台
            }
            int maxPage = maxPageNum / pageLineNum;
            if (maxPage == pageNum)
            {
                return "1";//表示已经是最后一页了E:\JXHLZQ\JXHLZQXXFWXT\JXHLZQXXFWXT\HTML\StatisticAlanalysis\Analysis.aspx
            }
            if (pageNum == 1)
            {
                return "2";//表示已经是首页了
            }
            IList<TENDENCYCHARTINFO> list = DisasterAnalysisData(pageNum, pageLineNum, businessFXPRV);
            return DisasterAnalysisTrueNoData(list, businessFXPRV);

        }
        /// <summary>
        /// 根据表名（一般主键字段）查询出有多少行记录

        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段</param>
        /// <param name="OpenISession.SessionEnum">枚举，查询哪个数据库</param>
        /// <returns>返回最大的页号值</returns>
        public int QueryMaxPageNum(BusinessEntities businessFXPRV)
        {
            int maxPageNum = 0;//当前最大页码值

            IList list = (from t in businessFXPRV.TENDENCYCHARTINFO select t.TPAGENO).ToList();
            maxPageNum = list.Count;
            return maxPageNum;
        }
        /// <summary>
        /// 生成灾情分析图的树形菜单数据
        /// </summary>
        /// <param name="list">分页后的数据</param>
        /// <returns></returns>
        public string DisasterAnalysisTrueNoData(IList<TENDENCYCHARTINFO> list, BusinessEntities businessFXPRV)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                TENDENCYCHARTINFO t = list[i];

                sb.Append("{tid:'").Append(t.TID).Append("',title:'");
                sb.Append(t.TTITLE).Append("'},");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        /// <summary>
        /// 得到List返回结果集
        /// </summary>
        /// <param name="pageNum">当前页码</param>
        /// <param name="pageLineNum">每页显示行数</param>
        /// <returns>得到灾情分析模块返回结果集</returns>
        //public IList<TENDENCYCHARTINFO> DisasterAnalysisData(int pageNum, int pageLineNum, BusinessEntities businessFXPRV)
        public IList<TENDENCYCHARTINFO> DisasterAnalysisData(int pageNum, int pageLineNum, BusinessEntities businessFXPRV)
        {
            return businessFXPRV.CreateObjectSet<TENDENCYCHARTINFO>().OrderBy(t => t.TDATA).Skip(pageNum * pageLineNum).Take(pageLineNum).ToList();
        }
        #endregion

        #region 刘磊,获取生成灾情分析模块趋势图的数据(吴博怀修改20140303)
        /// <summary>
        /// 根据PageNOs查询出x轴的时间段
        /// </summary>
        /// <param name="pageNOs">页号集合</param>
        /// <param name="bsnEntities">数据库实体</param>
        /// <returns>x轴时间段和页号集合</returns>
        public string[] GetDisasterAnalysisDataTime(int[] pageNOArr, BusinessEntities bsnEntities)
        {
            FXDICTEntities fxdict = new FXDICTEntities();
            string[] timeArr = (from r in bsnEntities.ReportTitle.ToList()
                                from p in pageNOArr
                                join c in fxdict.TB14_Cyc.ToList() on r.StatisticalCycType equals c.CycType
                                where r.PageNO == p
                                orderby r.EndDateTime, r.PageNO
                                select c.Remark + r.EndDateTime.Value.Year.ToString() + "年"
                                + "<br/>" + r.StartDateTime.Value.Month.ToString() + "月" + r.StartDateTime.Value.Day.ToString() + "日-"
                                + r.EndDateTime.Value.Month.ToString() + "月" + r.EndDateTime.Value.Day.ToString() + "日"
                                ).ToArray<string>();
            return timeArr;
        }

        /// <summary>
        /// 获得生成灾情分析趋势图的数据
        /// </summary>
        /// <param name="pageNOs">页号集合</param>
        /// <param name="disasterType">受灾类型</param>
        /// <param name="unitCodes">单位集合</param>
        /// <param name="bsnEntities">数据库实体</param>
        /// <returns></returns>
        public QSDataDWBean[] GetDisasterAnalysisData(int[] pageNOArr, string disasterType, string unitCodes, BusinessEntities bsnEntities)
        {
            StringBuilder jsonstr = new StringBuilder();
            int disasterTypeUnit = GetDisasterUnit(disasterType);   //根据灾情类型获取对应单位
            string[] unitCodeArr = unitCodes.Split(',');
            List<QSChartBean> list = new List<QSChartBean>();
            if (disasterType == "qxhj" || disasterType == "zjxj" || disasterType == "xyjmswr" || disasterType == "xyjzjjxy")
            {
                list = ((from hl in bsnEntities.HL014
                         from p in pageNOArr
                         from da in unitCodeArr
                         where (hl.PageNO == p && hl.UnitCode == da)
                         select new QSChartBean
                         {
                             UnitCode = hl.UnitCode,
                             PageNO = hl.PageNO,
                             value =
                             disasterType == "qxhj" ? (hl.QXHJ == null) ? 0 : hl.QXHJ / disasterTypeUnit :
                             disasterType == "zjxj" ? (hl.ZJXJ == null) ? 0 : hl.ZJXJ / disasterTypeUnit :
                             disasterType == "xyjmswr" ? (hl.XYJMSWR == null) ? 0 : hl.XYJMSWR / disasterTypeUnit :
                             (hl.XYJZJJXY == null) ? 0 : hl.XYJZJJXY / disasterTypeUnit
                         }).Distinct()).ToList<QSChartBean>();
            }
            else
            {
                list = ((from hl in bsnEntities.HL011
                         from p in pageNOArr
                         from da in unitCodeArr
                         where (hl.PageNO == p && hl.UnitCode == da)
                         select new QSChartBean
                         {
                             UnitCode = hl.UnitCode,
                             PageNO = hl.PageNO,
                             value =
                             disasterType == "sycs" ? (hl.SYCS == null) ? 0 : hl.SYCS / disasterTypeUnit :
                             disasterType == "nlmyzjjjss" ? (hl.NLMYZJJJSS == null) ? 0 : hl.NLMYZJJJSS / disasterTypeUnit :
                             disasterType == "gjyszjjjss" ? (hl.GJYSZJJJSS == null) ? 0 : hl.GJYSZJJJSS / disasterTypeUnit :
                             disasterType == "shskd" ? (hl.SHSKD == null) ? 0 : hl.SHSKD / disasterTypeUnit :
                             disasterType == "shskx" ? (hl.SHSKX == null) ? 0 : hl.SHSKX / disasterTypeUnit :
                             disasterType == "shdfcd" ? (hl.SHDFCD == null) ? 0 : hl.SHDFCD / disasterTypeUnit :
                             disasterType == "shswcz" ? (hl.SHSWCZ == null) ? 0 : hl.SHSWCZ / disasterTypeUnit :
                             disasterType == "shhtcd" ? (hl.SHHTCD == null) ? 0 : hl.SHHTCD / disasterTypeUnit :
                             disasterType == "szrk" ? (hl.SZRK == null) ? 0 : hl.SZRK / disasterTypeUnit :
                             disasterType == "shmjxj" ? (hl.SHMJXJ == null) ? 0 : hl.SHMJXJ / disasterTypeUnit :
                             disasterType == "czmjxj" ? (hl.CZMJXJ == null) ? 0 : hl.CZMJXJ / disasterTypeUnit :
                             disasterType == "jsmjxj" ? (hl.JSMJXJ == null) ? 0 : hl.JSMJXJ / disasterTypeUnit :
                             disasterType == "swrk" ? (hl.SWRK == null) ? 0 : hl.SWRK / disasterTypeUnit :
                             disasterType == "zyrk" ? (hl.ZYRK == null) ? 0 : hl.ZYRK / disasterTypeUnit :
                             disasterType == "dtfw" ? (hl.DTFW == null) ? 0 : hl.DTFW / disasterTypeUnit :
                             disasterType == "zjjjzss" ? (hl.ZJJJZSS == null) ? 0 : hl.ZJJJZSS / disasterTypeUnit :
                             (hl.SLSSZJJJSS == null) ? 0 : hl.SLSSZJJJSS / disasterTypeUnit
                         }).Distinct()).ToList<QSChartBean>();
            }
            QSDataDWBean[] qSDataDWs = new QSDataDWBean[unitCodeArr.Length];
            int pageNOCount = pageNOArr.Length;
            for (int i = 0; i < unitCodeArr.Length; i++)
            {
                QSDataDWBean cqsddw = new QSDataDWBean();
                cqsddw.name = GetUnitName(unitCodeArr[i]);
                double[] data = new double[pageNOCount];
                string unitCode = unitCodeArr[i];
                for (int j = pageNOCount - 1; j >= 0; j--)
                {
                    int pageNO = pageNOArr[j];
                    for (int k = 0; k < list.Count; k++)
                    {
                        var l = list[k];
                        if (pageNO.Equals(l.PageNO) && unitCode.Equals(l.UnitCode))
                        {
                            data[pageNOCount - 1 - j] = (double)l.value;    //按pageno从小到大赋值，与x轴时间升序排列保持一致
                            break;
                        }
                    }
                }
                cqsddw.data = data;
                qSDataDWs[i] = cqsddw;
            }
            return qSDataDWs;
        }

        /// <summary>传入行政单位代码,返回行政单位名称
        /// 
        /// </summary>
        /// <param name="unitCode">行政单位代码</param>
        /// <returns>行政单位名称</returns>
        public string GetUnitName(string unitCode)
        {
            FXDICTEntities fxdict = Persistence.GetDbEntities();;
            string unitName = fxdict.TB07_District.Single(d => d.DistrictCode == unitCode).DistrictName;
            return unitName;
        }

        /// <summary>
        /// 根据受灾类型获得单位
        /// </summary>
        /// <param name="disastertype">受灾类型</param>
        /// <returns></returns>
        public int GetDisasterUnit(string disastertype)
        {
            int numberUnit = 0;
            switch (disastertype)
            {
                case "shmjxj"://洪涝受灾面积
                    numberUnit = 10000000;
                    break;
                case "szrk"://受灾人口
                    numberUnit = 10000;
                    break;
                case "symj"://受涝面积
                    numberUnit = 10000000;
                    break;
                case "czmjxj"://成灾面积
                    numberUnit = 10000000;
                    break;
                case "jsmjxj"://绝收面积
                    numberUnit = 10000000;
                    break;
                case "swrk"://死亡人口
                    numberUnit = 1;
                    break;
                case "zyrk"://转移人口
                    numberUnit = 10000;
                    break;
                case "dtfw"://倒塌房屋
                    numberUnit = 10000;
                    break;
                case "zjxj"://洪涝,资金投入
                    numberUnit = 10000;
                    break;
                case "ysknrk"://人饮水困难人数

                    numberUnit = 10000;
                    break;
                case "SHMJHJXJ"://受旱面积
                    numberUnit = 10000000;
                    break;
                case "shmjqhxj"://轻旱面积
                    numberUnit = 10000000;
                    break;
                case "shmjzhxj"://重旱面积
                    numberUnit = 10000000;
                    break;
                case "shmjgkxj"://干枯面积
                    numberUnit = 10000000;
                    break;
                case "szmjzj"://干旱受灾面积
                    numberUnit = 10000000;
                    break;
                case "szmjcj"://成灾面积
                    numberUnit = 10000000;
                    break;
                case "szmjjs"://绝收面积
                    numberUnit = 10000000;
                    break;
                case "shyxrs"://受旱影响人数
                    numberUnit = 10000;
                    break;
                case "trkhzjhj"://干旱资金投入
                    numberUnit = 10000;
                    break;
                case "szl":
                    numberUnit = 1;
                    break;
                case "shl":
                    numberUnit = 1;
                    break;
                case "zjjjzss":
                    numberUnit = 10000;
                    break;
                case "slsszjjjss":
                    numberUnit = 10000;
                    break;
                case "shskd"://大中型水库损坏
                    numberUnit = 1;
                    break;
                case "shskx"://小型水库损坏
                    numberUnit = 1;
                    break;
                case "shdfcd"://损坏提防
                    numberUnit = 1000;
                    break;
                case "shswcz"://损坏水文测站
                    numberUnit = 1;
                    break;
                case "shhtcd"://损坏海塘
                    numberUnit = 1000;
                    break;
                case "sycs"://受淹城市
                    numberUnit = 1;
                    break;
                case "qxhj"://出动抢险人数
                    numberUnit = 1;
                    break;
                case "zjtrxj"://资金投入小计
                    numberUnit = 10000;
                    break;
                case "xyjmswr"://避免人员伤亡
                    numberUnit = 1;
                    break;
                default://洪涝:直接经济损失,水利损失。抗旱：直接经济损失，农业损失。

                    numberUnit = 100000000;
                    break;
            }
            return numberUnit;
        }


        /// <summary>
        /// 根据受灾类型获得标题
        /// </summary>
        /// <param name="disastertype">受灾类型</param>
        /// <returns>标题</returns>
        public string GetQSTitle(string disastertype)
        {
            string strtitle = "";
            switch (disastertype)
            {
                case "szrk"://受灾人口
                    strtitle = "受灾人口趋势图(万人)";
                    break;
                case "swrk"://死亡人口
                    strtitle = "死亡人口趋势图(人)";
                    break;
                case "zyrk"://转移人口
                    strtitle = "转移人口趋势图(万人)";
                    break;
                case "symj"://受涝面积
                    strtitle = "受涝面积趋势图(人)";
                    break;
                case "dtfw"://倒塌房屋
                    strtitle = "倒塌房屋趋势图(万间)";
                    break;
                case "shmjxj"://农作物受灾面积

                    strtitle = "农作物受灾面积趋势图(千公顷)";
                    break;
                case "zjjjzss"://直接经济总损失

                    strtitle = "直接经济总损失趋势图(万元)";
                    break;
                case "czmjxj"://成灾面积
                    strtitle = "成灾面积趋势图(千公顷)";
                    break;
                case "jsmjxj"://绝收面积
                    strtitle = "绝收面积趋势图(千公顷)";
                    break;
                case "zjxj"://洪涝资金投入
                    strtitle = "洪涝资金投入趋势图(万元)";
                    break;
                case "slsszjjjss"://水利损失
                    strtitle = "水利损失趋势图(万元)";
                    break;
                case "shyxrs"://受旱影响人数
                    strtitle = "受旱影响人数趋势图(万人)";
                    break;
                case "shmjhjxj"://受旱面积
                    strtitle = "受旱面积趋势图(千公顷)";
                    break;
                case "shmjqhxj"://轻旱面积
                    strtitle = "轻旱面积趋势图(千公顷)";
                    break;
                case "shmjzhxj"://重旱面积
                    strtitle = "重旱面积趋势图(千公顷)";
                    break;
                case "shmjgkxj"://干枯面积
                    strtitle = "干枯面积趋势图(千公顷)";
                    break;
                case "szmjzj"://抗旱受灾面积
                    strtitle = "受灾面积趋势图(千公顷)";
                    break;
                case "szmjcj"://成灾面积
                    strtitle = "成灾面积趋势图(千公顷)";
                    break;
                case "szmjjs"://绝收面积
                    strtitle = "绝收面积趋势图(千公顷)";
                    break;
                case "ysknrk"://饮水困难人数
                    strtitle = "饮水困难人数趋势图(万人)";
                    break;
                case "qthyyhzjjjss"://因旱直接经济总损失

                    strtitle = "因旱直接经济总损失趋势图(亿元)";
                    break;
                case "jjzssnyss"://农业损失
                    strtitle = "农业损失趋势图(亿元)";
                    break;
                case "nlmyzjjjss"://农林牧渔直接经济损失
                    strtitle = "农林牧渔直接经济损失趋势图(亿元)";
                    break;
                case "gjyszjjjss"://工业交通运输直接经济损失
                    strtitle = " 工业交通运输直接经济损失(亿元)";
                    break;
                case "shskd"://大中型水库损坏
                    strtitle = "大中型水库损坏(座)";
                    break;
                case "shskx"://小型水库损坏
                    strtitle = "小型水库损坏(座)";
                    break;
                case "shdfcd"://损坏提防
                    strtitle = "损坏提防(千米)";
                    break;
                case "shswcz"://损坏水文测站
                    strtitle = "损坏水文测站(座)";
                    break;
                case "shhtcd"://损坏海塘
                    strtitle = "损坏海塘(千米)";
                    break;
                case "sycs"://受淹城市
                    strtitle = "受淹城市(个)";
                    break;
                case "qxhj"://出动抢险人数
                    strtitle = "投入抢险人数(人次)";
                    break;
                case "zjtrxj"://资金投入小计
                    strtitle = "资金投入(万元)";
                    break;
                case "xyjmswr"://避免人员伤亡
                    strtitle = "避免人员伤亡(人次)";
                    break;
                case "xyjzjjxy"://减灾经济效益
                    strtitle = "减灾经济效益(亿元)";
                    break;
                default:
                    strtitle = "资金投入(万元)";
                    break;
            }
            return strtitle;
        }
        #endregion
    }
}
