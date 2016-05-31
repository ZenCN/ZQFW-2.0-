using System;
using System.Collections.Generic;
using System.Linq;
using LogicProcessingClass.AuxiliaryClass;
using EntityModel;
using DBHelper;

namespace LogicProcessingClass.Statistics
{
    public  class getDisasterLevelForMap
    {
        BusinessEntities m_BsnEntities;
        SingleEvaluation sEvaluation;
        int m_Level;
        public getDisasterLevelForMap(int level)
        {
            m_Level = level;
            m_BsnEntities = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
            sEvaluation = new SingleEvaluation();
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        public object getDisasterLevel(int pageNO, int level)
        {
            DisasterAssessment_title dt = new DisasterAssessment_title(level);
            var hl011 = getHL011(pageNO);
            var hl013 = from h3 in m_BsnEntities.HL013
                        where h3.PageNO == pageNO && h3.DW !="合计"
                        group h3 by h3.UnitCode into g
                        select new
                        {
                            UnitCode=g.FirstOrDefault().UnitCode,
                            PageNO = g.FirstOrDefault().PageNO,
                            SMXJT = g.Sum(h => h.SMXJT),
                            GCYMLS = g.Sum(h => h.GCYMLS),
                            SMXGS = g.Sum(h => h.SMXGS),
                            SMXGD = g.Sum(h => h.SMXGD),
                            SMXGQ = g.Sum(h => h.SMXGQ),
                        };
            var dataList = (from h1 in hl011
                            join h3 in hl013
                            on new { h1.PageNO,h1.UnitCode } equals new { h3.PageNO,h3.UnitCode } into hl
                            from h in hl.DefaultIfEmpty()
                            select new
                            {
                                U = h1.UnitCode,
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
                            }).ToList();
            DWdisasterLevel[] levelList = new DWdisasterLevel[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                var d = dataList[i];
                double pop = dt.getPopulation(d.U);
                double LandArea = dt.getLandArea(d.U);
                int disasterLevel = sEvaluation.getGrade(d.SWRK, d.SZRK, d.SHMJXJ, d.ZJJJZSS, d.SLSSZJJJSS, d.DTFW,
                            d.SMXJT, d.GCYMLS, d.SMXGS, d.SMXGD, d.SMXGQ, pop, LandArea);
                var l = new DWdisasterLevel
                {
                    unitCode = d.U,
                    dLevel = disasterLevel.ToString()
                };
                levelList[i] = l;
            }
            return levelList;
        }
        IList<HL011> getHL011(int pageNO)
        {
            var hl011 = (from h1 in m_BsnEntities.HL011.ToList()
                         where h1.PageNO==pageNO && h1.DW !="合计"
                         select h1
                         ).ToList();
            return hl011;
        }
    }
}
