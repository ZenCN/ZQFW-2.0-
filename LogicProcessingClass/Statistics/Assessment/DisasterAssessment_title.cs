using System;
using System.Collections.Generic;
using System.Linq;
using LogicProcessingClass.AuxiliaryClass;
using EntityModel;
using DBHelper;
using System.Linq.Expressions;

namespace LogicProcessingClass.Statistics
{
    /*----------------------------------------------------------------
        // 版本说明：
        // 版本号：
        // 文件名：DisasterAssessment_title.cs
        // 文件功能描述：获取灾情地图标题
        // 创建标识：符明 2013-03-29
        // 修改标识：
        // 修改描述：

    //-------------------------------------------------------------*/

    public class DisasterAssessment_title
    {
        BusinessEntities m_BsnEntities;
        int m_Level;
        public DisasterAssessment_title(int level)
        {
            m_Level = level;
            m_BsnEntities = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>查询单场洪涝灾情评估数据
        /// 
        /// </summary>
        /// <param name="startDate">查询起始日期</param>
        /// <param name="endDate">查询结束日期</param>
        /// <param name="currentCount">已查询数据条数</param>
        /// <param name="queryCount">本次查询数据条数</param>
        /// <returns>单场洪涝灾情评估数据</returns>
        public object GetSingleEvaluationTitle(int currentCount, int queryCount, DateTime startDateTime, DateTime endDateTime, string unitCode)
        {
            var hl011 = getHL011Data(0, startDateTime, endDateTime);
            int number = hl011.Count;
            if (number == 0)
            {
                return new { };
            }
            else
            {
                double population = getPopulation(unitCode);
                double landArea = getLandArea(unitCode);
                var hl013 = from h3 in m_BsnEntities.HL013
                            where h3.DW == "合计"
                            group h3 by h3.PageNO into g
                            from gp in g
                            where gp.TBNO == g.Max(h1 => h1.TBNO)
                            select gp;
                var dataList = (from h1 in hl011
                    join h3 in hl013
                        on new {h1.PageNO, h1.UnitCode} equals new {h3.PageNO, h3.UnitCode} into hl
                    from h in hl.DefaultIfEmpty()
                    where h1.ReportTitle.Del == 0
                    orderby h1.ReportTitle.EndDateTime descending
                    select new
                    {

                        h1.ReportTitle.Remark,
                        PageNO = h1.PageNO == null ? h.PageNO : h1.PageNO,

                        h1.ReportTitle.StartDateTime,
                        h1.ReportTitle.EndDateTime,
                        SWRK = Convert.ToDouble(h1.SWRK),
                        SZRK = Convert.ToDouble(h1.SZRK),
                        SHMJXJ = Convert.ToDouble(h1.SHMJXJ),
                        ZJJJZSS = Convert.ToDouble(h1.ZJJJZSS),
                        SLSSZJJJSS = Convert.ToDouble(h1.SLSSZJJJSS),
                        DTFW = Convert.ToDouble(h1.DTFW),
                        SMXJT = Convert.ToDouble(h == null ? 0 : h.SMXJT),
                        GCYMLS = Convert.ToDouble(h == null ? 0 : h.GCYMLS),
                        SMXGS = Convert.ToDouble(h == null ? 0 : h.SMXGS),
                        SMXGD = Convert.ToDouble(h == null ? 0 : h.SMXGD),
                        SMXGQ = Convert.ToDouble(h == null ? 0 : h.SMXGQ)
                    }).Skip(currentCount).Take(queryCount).OrderByDescending(x => x.EndDateTime).ToList();
                SingleEvaluation sEvaluation = new SingleEvaluation();
                EvaluationTitle[] titleList = new EvaluationTitle[dataList.Count];
                for (int i = 0; i < dataList.Count; i++)
                {
                    var d = dataList[i];
                    string title = d.StartDateTime.Value.ToString("yyyy年MM月dd日") + "至" +
                                   d.EndDateTime.Value.ToString("MM月dd日");
                    string subtitle = d.Remark;

                    /*********新评估方法（据洪涝灾情评估标准_报批稿.doc）*******/
                    int disasterLevel = sEvaluation.getGrade(d.SWRK, d.SZRK, d.SHMJXJ, d.ZJJJZSS, d.SLSSZJJJSS, d.DTFW,
                        d.SMXJT, d.GCYMLS, d.SMXGS, d.SMXGD, d.SMXGQ, population, landArea);
                    /***************************************************************/

                    /********旧评估方法（来源未知）***********/
                    //double[] disasterDataArr = new double[]{d.SWRK,d.SZRK,d.SHMJXJ,d.DTFW,d.ZJJJZSS,d.SMXJT,d.GCYMLS};
                    //int disasterLevel = sEvaluation.getGrade(disasterDataArr,m_Level);
                    /*****************************************/

                    int pageNO = (int)d.PageNO;
                    var a = new EvaluationTitle
                    {
                        title = title,
                        subtitle = subtitle,
                        pageno = pageNO,
                        itemlevel = disasterLevel
                    };
                    titleList[i] = a;
                }

                var jsonData = new { totalcount = number, items = titleList };
                return jsonData;
            }
        }

        /// <summary>查询年度洪涝灾情评估数据
        /// 
        /// </summary>
        /// <param name="startDate">查询起始日期</param>
        /// <param name="endDate">查询结束日期</param>
        /// <param name="currentCount">当前已查询数据条数</param>
        /// <param name="queryCount">本次查询数据条数</param>
        /// <returns>年度洪涝灾情评估数据</returns>
        public object GetAnnualEvaluationTitle(int currentCount, int queryCount, DateTime startDateTime, DateTime endDateTime)
        {
            var hl011 = getHL011Data(1, startDateTime, endDateTime);
            int number = hl011.Count;
            if (number == 0)
            {
                return new { };
            }
            else
            {
                var dataList = (from h1 in hl011
                                where h1.ReportTitle.Del == 0
                                orderby h1.ReportTitle.EndDateTime descending
                                select new
                                {
                                    h1.ReportTitle.Remark,
                                    h1.PageNO,
                                    h1.ReportTitle.StartDateTime,
                                    h1.ReportTitle.EndDateTime,
                                    h1.ReportTitle.StatisticalCycType,
                                    SWRK = Convert.ToDouble(h1.SWRK),
                                    SZRK = Convert.ToDouble(h1.SZRK),
                                    SHMJXJ = Convert.ToDouble(h1.SHMJXJ),
                                    ZJJJZSS = Convert.ToDouble(h1.ZJJJZSS),
                                    SLSSZJJJSS = Convert.ToDouble(h1.SLSSZJJJSS),
                                    DTFW = Convert.ToDouble(h1.DTFW),
                                }).Skip(currentCount).Take(queryCount).ToList();
                AnnualEvaluation aEvaluation = new AnnualEvaluation();

                EvaluationTitle[] titleList = new EvaluationTitle[dataList.Count];
                for (int i = 0; i < dataList.Count; i++)
                {
                    var d = dataList[i];
                    string title = d.EndDateTime.Value.Year.ToString();
                    switch (Convert.ToInt32(d.StatisticalCycType))
                    {
                        case 5:
                            title += "年终";
                            break;
                        case 7:
                            title += "年初";
                            break;
                    }
                    title += "灾情评估";
                    string subtitle = d.Remark;

                    /*********新评估方法（据洪涝灾情评估标准_报批稿.doc）*******/
                    int disasterLevel = aEvaluation.getGrade(d.SWRK, d.SZRK, d.SHMJXJ, d.ZJJJZSS, d.SLSSZJJJSS, d.DTFW);
                    /***************************************************************/

                    /********旧评估方法（来源未知）***********/
                    //double[] disasterDataArr = new double[] { d.SWRK, d.SZRK, d.SHMJXJ, d.DTFW, d.ZJJJZSS };
                    //int disasterLevel = aEvaluation.getGrade(disasterDataArr,m_Level);
                    /******************************************/

                    int pageNO = (int)d.PageNO;
                    var a = new EvaluationTitle
                    {
                        title = title,
                        subtitle = subtitle,
                        pageno = pageNO,
                        itemlevel = disasterLevel
                    };
                    titleList[i] = a;
                }
                var jsonData = new { totalcount = number, items = titleList };
                return jsonData;
            }
        }

        /// <summary>根据行政单位代码获取人口
        /// 
        /// </summary>
        /// <param name="unitCode">行政单位代码</param>
        /// <returns>人口</returns>
        public double getPopulation(string unitCode)
        {
            double population = getBaseData(unitCode, 25);
            return population;
        }

        /// <summary>根据行政单位代码获取耕地面积
        /// 
        /// </summary>
        /// <param name="unitCode">行政单位代码</param>
        /// <returns>耕地面积</returns>
        public double getLandArea(string unitCode)
        {
            double landArea = getBaseData(unitCode, 32);
            return landArea;
        }

        /// <summary>获取基础数据
        /// 
        /// </summary>
        /// <param name="unitCode">行政单位代码</param>
        /// <param name="fieldNO">字段编号</param>
        /// <returns>基础数据值</returns>
        double getBaseData(string unitCode, int fieldNO)
        {
            FXDICTEntities fxdict = new FXDICTEntities();
            double basedata = Convert.ToDouble(fxdict.TB04_CheckBase.Where(t => t.FieldDefine_NO == fieldNO && t.District_Code == unitCode).Max(t => t.BaseData));
            return basedata;
        }

        /// <summary>返回符合条件的HL011表的数据
        /// 
        /// </summary>
        /// <param name="evaluationType">评估类型</param>
        /// <param name="startDateTime">开始时间</param>
        /// <param name="endDateTime">结束时间</param>
        /// <returns>HL011表的数据</returns>
        IList<HL011> getHL011Data(int evaluationType, DateTime startDateTime, DateTime endDateTime)
        {
            int[] cycTypes = getCycTypes(evaluationType);
            var endday = endDateTime.AddDays(1);
            var hl011 = m_BsnEntities.HL011.Where(t => t.DW == "合计" && t.ReportTitle.RPTType_Code == "XZ0" && t.ReportTitle.ORD_Code == "HL01" && t.ReportTitle.EndDateTime >= startDateTime && t.ReportTitle.EndDateTime < endday).ToList();
            var hs = new List<HL011>();
            cycTypes.ToList().ForEach(s => hs.AddRange(hl011.FindAll(t => t.ReportTitle.StatisticalCycType == s)));
            return hs;
        }

        /// <summary>获取评估类型（单场或年度）对应报表数据时段类型
        /// 
        /// </summary>
        /// <param name="evaluationType">评估类型</param>
        /// <returns>时段类型</returns>
        int[] getCycTypes(int evaluationType)
        {
            if (evaluationType == 0)
            {
                return new int[] { 6 };
            }
            else
            {
                return new int[] { 5, 7 };
            }
        }
    }
}
