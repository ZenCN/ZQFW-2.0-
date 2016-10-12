using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using DBHelper;

namespace LogicProcessingClass.Statistics
{
    public class MapBean
    {
        public IDictionary<string, DisasterBean> GetDataForMap(int level, string pageNO, string unitCode, int mapType)
        {
            BusinessEntities bsnEntities = Persistence.GetDbEntities(level);
            IDictionary<string, DisasterBean> disasterData = new Dictionary<string, DisasterBean>();
            if (mapType == 3)
            {
                disasterData = getAssessmentMapData(Convert.ToInt32(pageNO), bsnEntities);
                return disasterData;
            }
            if (unitCode == "")
            {
                IDictionary<string, DisasterBean> disasterData1 = new Dictionary<string, DisasterBean>();
                IDictionary<string, DisasterBean> disasterData2 = new Dictionary<string, DisasterBean>();
                if (mapType == 0)
                {
                    disasterData1 = GetDisasterData(pageNO, bsnEntities);
                }

                if (mapType == 1 || mapType == 2)
                {
                    disasterData2 = GetLowerDisasterData(pageNO, "", bsnEntities, level);
                }
                disasterData = disasterData1.Union(disasterData2).ToDictionary(k => k.Key, v => v.Value);
            }
            else
            {
                disasterData = GetLowerDisasterData(pageNO, unitCode, bsnEntities, level);
            }
            return disasterData;
        }

        /// <summary>查询对应页号灾情数据
        /// 
        /// </summary>
        /// <param name="pageNOs">页号</param>
        /// <param name="bsnEntities">实体库</param>
        /// <returns>灾情数据</returns>
        IDictionary<string, DisasterBean> GetDisasterData(string pageNOs, BusinessEntities bsnEntities)
        {
            string[] strPageNOArr = pageNOs.Split(',');
            int[] pageNOArr = Array.ConvertAll<string, int>(strPageNOArr, pageNO => Convert.ToInt32(pageNO));
            IDictionary<string, DisasterBean> disasterData = new Dictionary<string, DisasterBean>();
            disasterData = (from h in bsnEntities.HL011

                            from p in pageNOArr
                            where h.PageNO == p && h.DW != "合计"
                            select h).ToDictionary(k => k.UnitCode, v => new DisasterBean
                            {
                                UnitName = v.DW,
                                SZRK =(v.SZRK==null)?0: Math.Round((double)v.SZRK / 10000, 4),
                                SHMJXJ =(v.SHMJXJ==null)?0: Math.Round((double)v.SHMJXJ / 10000000, 4),
                                SWRK = (v.SWRK==null)?0:(double)v.SWRK,
                                DTFW =(v.DTFW==null)?0: Math.Round((double)v.DTFW / 10000, 4),
                                ZJJJZSS =(v.ZJJJZSS==null)?0: Math.Round((double)v.ZJJJZSS / 100000000, 4),
                                SLSSZJJJSS = (v.SLSSZJJJSS==null)?0:Math.Round((double)v.SLSSZJJJSS / 100000000, 4),
                                SZRKR = (v.SZRKR==null)?0:(double)v.SZRKR,
                                ZYRK = (v.ZYRK==null)?0:Math.Round((double)v.ZYRK / 10000, 4),

                            });
            return disasterData;
        }

        /// <summary>
        /// 查询下级数据
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="unitCode">行政单位代码</param>
        /// <param name="bsnEntities">本级数据库</param>
        /// <param name="level">当前级别</param>
        /// <returns></returns>
        IDictionary<string, DisasterBean> GetLowerDisasterData(string pageNO, string unitCode,
            BusinessEntities bsnEntities, int level)
        {
            int lowerLevel = (level == 0) ? 2 : level + 1;
            BusinessEntities lowerLevelEntities = Persistence.GetDbEntities(lowerLevel);
            string pagenoNext = GetLowerPageNOs(Convert.ToInt32(pageNO), unitCode, bsnEntities);//得到下级pageno
            IDictionary<string, DisasterBean> disasterData = new Dictionary<string, DisasterBean>();
            if (pagenoNext != "")
            {
                disasterData = GetDisasterData(pagenoNext, lowerLevelEntities);
            }
            return disasterData;
        }
        /// <summary>
        /// 查询下级页号
        /// </summary>
        /// <param name="pageNO">本级页号</param>
        /// <param name="unitCode">行政单位代码</param>
        /// <returns>下级页号</returns>
        string GetLowerPageNOs(int pageNO, string unitCode, BusinessEntities bsnEntities)
        {
            int?[] pageNOArr;
            if (unitCode != "")
            {
                pageNOArr = (from a in bsnEntities.AggAccRecord
                             where a.PageNo == pageNO && a.UnitCode == unitCode
                             select a.SPageNO).ToArray();
            }
            else
            {
                pageNOArr = (from a in bsnEntities.AggAccRecord
                             where a.PageNo == pageNO
                             select a.SPageNO).ToArray();
            }
            string pageNOs = "";
            if (pageNOArr.Length > 0)
            {
                for (int i = 0; i < pageNOArr.Length; i++)
                {
                    pageNOs += Convert.ToString(pageNOArr[i]) + ",";
                }
                pageNOs = pageNOs.Remove(pageNOs.Length - 1, 1);
            }
            return pageNOs;
        }

        /// <summary>获取场次评估地图数据
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="bsnEntities">数据库实体类</param>
        /// <returns></returns>
        IDictionary<string, DisasterBean> getAssessmentMapData(int pageNO, BusinessEntities bsnEntities)
        {
            var hl013 = from h3 in bsnEntities.HL013
                        where h3.DW != "合计" && h3.PageNO == pageNO
                        group h3 by h3.DW into g
                        from gp in g
                        where gp.TBNO == g.Max(h1 => h1.TBNO)
                        select gp;
            var hl011 = bsnEntities.HL011.Where(h => h.PageNO == pageNO && h.DW != "合计");
            var data = from h1 in hl011
                       join h3 in hl013
                        on h1.UnitCode equals h3.UnitCode into HL
                       from h in HL.DefaultIfEmpty()
                       // where h.PageNO == pageNO && h1.DW != "合计"
                       select new
                       {
                           h1.UnitCode,
                           h1.DW,
                           h1.SWRK,
                           h1.SZRK,
                           h1.SHMJXJ,
                           h1.DTFW,
                           h1.ZJJJZSS,
                           SMXJT = (h.SMXJT==null)?0:h.SMXJT,
                           GCYMLS = (h.GCYMLS==null)?0:h.GCYMLS
                       };
            var dictionary = data.ToDictionary(k => k.UnitCode, v => new DisasterBean
            {
                UnitName = v.DW,
                SWRK = (v.SWRK==null)?0:(double)v.SWRK,
                SZRK = (v.SZRK==null)?0:Math.Round((double)v.SZRK / 10000, 4),
                SHMJXJ = (v.SHMJXJ==null)?0:Math.Round((double)v.SHMJXJ / 10000000, 4),
                DTFW = (v.DTFW==null)?0:Math.Round((double)v.DTFW / 10000, 4),
                ZJJJZSS = (v.ZJJJZSS==null)?0:Math.Round((double)v.ZJJJZSS / 100000000, 4),
                SMXJT = (v.SMXJT==null)?0:Math.Round((double)v.SMXJT / 24, 2),
                GCYMLS = (v.GCYMLS==null)?0:Math.Round((double)v.GCYMLS / 24, 2),
            });

            return dictionary;
        }
    }

    /// <summary>
    ///ScaleDataBean 的摘要说明

    /// </summary>
    public class ScaleDataBean
    {
        public ScaleDataBean()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        //页号
        private string pageno;

        public string Pageno
        {
            get { return pageno; }
            set { pageno = value; }
        }
        //开始时间

        private string startDate;

        public string StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }


        //结束时间
        private string endDate;

        public string EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }


        //报表类型
        private string sorceType;

        public string SorceType
        {
            get { return sorceType; }
            set { sorceType = value; }
        }
        //报表类型名字
        private string sorceTypeName;

        public string SorceTypeName
        {
            get
            {
                switch (sorceType)
                {
                    case "0":
                        return "实时报";
                    case "1":
                        return "日报";
                    case "2":
                        return "旬报";
                    case "3":
                        return "月报";
                    case "4":
                        return "累计报";
                    case "5":
                        return "年终报";
                    case "6":
                        return "过程报";
                    case "7":
                        return "年初报";
                    default:
                        return "实时报";
                }
            }
        }

    }

    /// <summary>//灾情数据
    /// 
    /// </summary>
    public class DisasterBean
    {
        public string UnitName { get; set; }//受灾行政单位名称
        public double SZRK { get; set; }//受灾人口 szrk
        public double SHMJXJ { get; set; }//农作物受灾面积 shmjxj
        public double SWRK { get; set; }//死亡人口  swrk
        public double DTFW { get; set; }//倒塌房屋 dtfw
        public double ZJJJZSS { get; set; }//直接经济损失 zjjjzss
        public double SLSSZJJJSS { get; set; } //水利损失 slsszjjjss
        public double SZRKR { get; set; }//失踪人口 SZRKR
        public double ZYRK { get; set; }//转移人口 zyrk
        public double SMXJT { get; set; } //骨干交通中断历时
        public double GCYMLS { get; set; }//城市淹没历时
    }

    public class LatestHL013
    {
        public string UnitCode { get; set; }
        public double SMXJT { get; set; }
        public double GCYMLS { get; set; }
    }
}
