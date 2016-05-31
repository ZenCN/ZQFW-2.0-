using System;
using System.Linq;
using EntityModel;
using DBHelper;
using System.Collections.Generic;
using System.Text;

namespace LogicProcessingClass.Statistics
{

    public class DisasterAssessment_Content
    {
        BusinessEntities m_BsnEntities;
        int m_Level;
        public DisasterAssessment_Content(int level)
        {
            m_BsnEntities = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
            m_Level = level;
        }

        /// <summary>//存储灾情数量级单位
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] Get_YL_UnitData()
        {
            //灾情数据单位  死亡人口 人，受灾人口 万人，毁坏房屋 万间，直接经济损失 亿元，骨干交通中断历时(铁路、公路干线中断) 天
            //城市区淹没历时 天
            int[] measureUnitArr = new int[] { 1, 10000, 10000000, 10000, 100000000, 24, 24 };
            return measureUnitArr;
        }

        /// <summary>//计算场次等级划分
        /// 
        /// </summary>
        /// <returns></returns>
        public double[][] Get_YL_GradeData()
        {
            //死亡人口 受灾人口 受灾面积 毁坏房屋 直接经济损失 骨干交通中断历时(铁路、公路干线中断) 城市区淹没历时
            double[][] gradeArr =
                new double[][] { new double[]{ 10, 100, 300 }, new double[]{ 1, 100, 300 }, new double[]{ 1, 100, 1000 },
                new double[]{ 1, 10, 50 }, new double[]{ 1, 100, 500 },new double[] { 0.25, 3, 10 }, new double[]{ 4, 7, 12 } };
            return gradeArr;
        }

        /// <summary>//计算年份等级划分
        /// 
        /// </summary>
        /// <returns></returns>
        public double[][] Get_YL_Year_GradeData()
        {
            //死亡人口 受灾人口 受灾面积 毁坏房屋 直接经济损失
            double[][] gradeArr = new double[][] { new double[]{ 2000, 5000, 10000 }, new double[]{ 5000, 15000, 20000 }, new double[]{ 500, 1000, 2000 },
                new double[]{ 100, 200, 500 }, new double[]{ 100, 1000, 2000 }};
            return gradeArr;
        }

        /// <summary>计算灾情数据等级
        /// 
        /// </summary>
        /// <param name="disasterData">灾情数据值</param>
        /// <param name="grades">等级区间</param>
        /// <returns>灾情数据等级</returns>
        public int GetDataGrade(double disasterData, double[] grades)
        {
            if (disasterData == 0) 
            {
                return 0; 
            }
            if (disasterData < grades[0] && disasterData>0)  //1 一般灾害
            {
                return 1;
            }
            else if (disasterData >= grades[0] && disasterData < grades[1]) //2 较大灾害
            {
                return 2;
            }
            else if (disasterData >= grades[1] && disasterData < grades[2])//3 重大灾害
            {
                return 3;
            }
            else   //4 特大灾害
            {
                return 4;
            }
        }

        /// <summary>获取报表起止时间
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <returns>报表起止时间</returns>
        public string GetStartEndDate(int pageNO)
        {
            var date = (from r in m_BsnEntities.ReportTitle.ToList()
                          where r.PageNO == pageNO
                          select r.StartDateTime.Value.ToString("yyyy年MM月dd日") + "至" 
                          + r.EndDateTime.Value.ToString("yyyy年MM月dd日")).ToList();
            return date[0];
        }

        /// <summary> 查询受灾行政单位有多少个（本级为省则查询市，本级为国家则查询省）
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <returns>行政单位个数</returns>
        public int GetUnitCount(int pageNO)
        {
            int count = m_BsnEntities.HL011.Where(h => h.DW != "合计" && h.PageNO == pageNO).Count();
            return count;
        }

        /// <summary> 查询受灾下级行政单位有多少个（本级为省，则查询县，本级为国家，则查询市）
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="level">登录单位级别</param>
        /// <returns>行政单位个数</returns>
        public int GetLowerLevelUnitCount(int pageNO,int level)
        {
            BusinessEntities bsnEntities = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level + 1);
            var pageNOs = from a in bsnEntities.AggAccRecord
                        where a.PageNo == pageNO
                        select a.PageNo;
            int count = bsnEntities.HL011.Where(h=>pageNOs.Contains(h.PageNO) && h.DW!="合计").Count();
            return count;
        }

        /// <summary>//根据pageno,查询灾情数据，并生成评估结果
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="evalType">评估类型</param>
        /// <returns></returns>
        public IList<object> GetDisasterData(int pageNO, int evalType)
        {
            int[] measureUnitArr = Get_YL_UnitData();  //获得数据单位
            double[][] singleGradeArr = Get_YL_GradeData();//获得场次灾情评估等级划分
            double[][] annualGradeArr = Get_YL_Year_GradeData();//获得年度灾情评估等级划分
            var h1 = m_BsnEntities.HL011.Single(h => h.PageNO == pageNO && h.DW == "合计");
            var disasterTypeArr = new DisasterTypeBean[]{
                new DisasterTypeBean{type="SWRK",name="死亡人口",measureUnit="人",measureValue = 1, decimalDigits=0, value=(h1.SWRK == null)? 0:(double)h1.SWRK},
               new DisasterTypeBean{type="SZRK",name="受灾人口",measureUnit="万人",measureValue = 10000, decimalDigits=4,value=(h1.SZRK == null)? 0:(double)h1.SZRK},
               new DisasterTypeBean{type="SHMJXJ",name="受灾面积",measureUnit="千公顷",measureValue = 10000000, decimalDigits=4,value=(h1.SHMJXJ == null)? 0:(double)h1.SHMJXJ},
               new DisasterTypeBean{type="DTFW",name="倒塌房屋",measureUnit="万间",measureValue = 10000, decimalDigits=4,value=(h1.DTFW == null)? 0:(double)h1.DTFW},
               new DisasterTypeBean{type="ZJJJZSS",name="直接经济损失",measureUnit="亿元",measureValue = 100000000, decimalDigits=4,value=(h1.ZJJJZSS == null)? 0:(double)h1.ZJJJZSS}
            };

            IList<object> listObject = new List<object>();
            for (int i = 0; i < disasterTypeArr.Length; i++)
            {
                DisasterTypeBean disasterType = disasterTypeArr[i];
                double disasterValue = disasterType.value / measureUnitArr[i];  //经过单位换算的灾情值
                double[] gradeArr =  (evalType == 0) ? singleGradeArr[i] : annualGradeArr[i];  //灾情等级划分
                var item = new
                {
                    itemname = disasterType.name + "(" + disasterType.measureUnit + ")",
                    itemcount = Math.Round(disasterValue, disasterType.decimalDigits),
                    itemlevel = GetDataGrade(disasterValue, gradeArr),
                    remarks = GetTop3Data(pageNO, disasterType, 0)
                };
                listObject.Add(item);
            }

            if (evalType == 0)
            {
                var h3 = m_BsnEntities.HL013.Where(h => h.PageNO == pageNO && h.DW == "合计").OrderByDescending(p => p.TBNO).Take(1).ToList();

                double SMXJT = (h3.Count > 0) ? (h3[0].SMXJT == null)?0:(double)h3[0].SMXJT : 0;
                double GCYMLS = (h3.Count > 0) ? (h3[0].GCYMLS == null) ? 0 : (double)h3[0].GCYMLS : 0;

                var disasterTypeArr_h3 = new DisasterTypeBean[]{
                    new DisasterTypeBean{type="SMXJT",name="骨干交通中断历时",measureUnit="天",measureValue = 24, decimalDigits=2, value=SMXJT},
                    new DisasterTypeBean{type="GCYMLS",name="城市区淹没历时",measureUnit="天",measureValue = 24, decimalDigits=2,value=GCYMLS}
                };
                for (int i = 0; i < disasterTypeArr_h3.Length; i++)
                {
                    DisasterTypeBean disasterType = disasterTypeArr_h3[i];
                    double disasterValue = disasterType.value / measureUnitArr[5 + i];  //经过单位换算的灾情值
                    double[] gradeArr = singleGradeArr[5+i];  //灾情等级划分
                    var item = new
                    {
                        itemname = disasterType.name + "(" + disasterType.measureUnit + ")",
                        itemcount = Math.Round(disasterValue, disasterType.decimalDigits),
                        itemlevel = GetDataGrade(disasterValue, gradeArr),
                        remarks = (h3.Count > 0) ? GetTop3Data(pageNO, disasterType, 1) : "" 
                    };
                    listObject.Add(item);
                }
            }
            else
            {
                string[] itemArr = Get_Year_PageNoListData(pageNO);
                var item = new
                {
                    itemname = "级别场次灾害数量(场)",
                    itemcount = itemArr[0],
                    itemlevel = itemArr[1],
                    remarks = itemArr[2]
                };
                listObject.Add(item);
            }
            return listObject;
        }


        /// <summary> //根据受灾类型、页号、数据连接,查询出排名前三的数据
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="disasterType">灾情数据类型</param>
        /// <param name="table">要查询的表格（0代表HL011,1代表HL013）</param>
        /// <returns></returns>
        public string GetTop3Data(int pageNO, DisasterTypeBean disasterType, int table)
        {
            string type = disasterType.type;
            int measureValue = disasterType.measureValue;
            string measureUnit = disasterType.measureUnit;
            int decimalDigits = disasterType.decimalDigits;
            if (table == 0)
            {
                var data = from h in m_BsnEntities.HL011
                           where h.DW != "合计" && h.PageNO == pageNO
                           group h by h.DW into g
                           select new
                           {
                               UnitCode = g.Key,
                               Value = g.Max(p => type == "SWRK" ? (p.SWRK==null)?0:p.SWRK :
                                                       type == "SZRK" ?(p.SZRK==null)?0: p.SZRK :
                                                       type == "SHMJXJ" ? (p.SHMJXJ==null)?0:p.SHMJXJ :
                                                       type == "DTFW" ? (p.DTFW==null)?0:p.DTFW :
                                                       (p.ZJJJZSS==null)?0:p.ZJJJZSS)
                           };
                string[] top3Data = (data.ToList().OrderByDescending(d => d.Value).Take(3).Select(
                    t => t.UnitCode + "(" + Math.Round((double)t.Value / measureValue, decimalDigits).ToString() + disasterType.measureUnit + ")"
                    )).ToArray();
                return string.Join(",", top3Data);
            }
            else
            {
                var data = from h in m_BsnEntities.HL013
                           where h.DW != "合计" && h.PageNO == pageNO
                           group h by h.DW into g
                           select new
                           {
                               UnitCode = g.Key,
                               Value = g.Max(p => type == "SMXJT" ? p.SMXJT : p.GCYMLS)
                           };
                string[] top3Data = (data.ToList().OrderByDescending(d => d.Value).Take(3).Select(
                            t => t.UnitCode + "(" + Math.Round((double)t.Value / measureValue, decimalDigits).ToString() + disasterType.measureUnit + ")"
                         )).ToArray();
                return string.Join(",", top3Data);
            }
        }

        /// <summary>//根据页号和灾情数据类型，生成饼状图数据
        /// 
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="type">灾情数据类型</param>
        /// <returns>饼图数据</returns>
        public IList<object> GetPieChartData(int pageNO, string type)
        {
            Dictionary<string,DisasterTypeBean> disasterTypes = new Dictionary<string,DisasterTypeBean>();
            disasterTypes.Add("SWRK",new DisasterTypeBean{measureValue = 1,decimalDigits=0});
            disasterTypes.Add("SZRK",new DisasterTypeBean{measureValue = 10000,decimalDigits=4});
            disasterTypes.Add("SHMJXJ",new DisasterTypeBean{measureValue = 10000000,decimalDigits=4});
            disasterTypes.Add("DTFW",new DisasterTypeBean{measureValue = 10000,decimalDigits=4});
            disasterTypes.Add("ZJJJZSS",new DisasterTypeBean{measureValue = 100000000,decimalDigits=4});
            disasterTypes.Add("SMXJT",new DisasterTypeBean{measureValue = 24,decimalDigits=2});
            disasterTypes.Add("GCYMLS",new DisasterTypeBean{measureValue = 24,decimalDigits=2});
            int[] unitArr = Get_YL_UnitData();

            IList<PieChartBean> listPieChartData = new List<PieChartBean>();
            if ("SMXJT".Equals(type) || "GCYMLS".Equals(type))
            {
                listPieChartData = m_BsnEntities.HL013.Where(h => h.PageNO == pageNO && h.DW != "合计").Select(h => new PieChartBean
                {
                    unitName= h.DW,
                    value = (double)(type== "SMXJT"?h.SMXJT:h.GCYMLS)
                }).ToList();
            }
            else
            {
                listPieChartData = m_BsnEntities.HL011.Where(h => h.PageNO == pageNO && h.DW != "合计").Select(h => new PieChartBean
                {
                    unitName = h.DW,
                    value = (double)(type == "SWRK" ? (h.SWRK==null)?0:h.SWRK :type == "SZRK" ? (h.SZRK==null)?0:h.SZRK :type == "SHMJXJ" ? (h.SHMJXJ==null)?0:h.SHMJXJ :
                    type == "DTFW" ? (h.DTFW==null)?0:h.DTFW : (h.ZJJJZSS==null)?0:h.ZJJJZSS)
                }).ToList();
            }

            IList<object> pieChartData = new List<object>(); 
            for (int i = 0; i < listPieChartData.Count; i++)
            {
                var data = listPieChartData[i];
                double value = Math.Round(data.value / disasterTypes[type].measureValue, disasterTypes[type].decimalDigits);
                var obj = new object[] { data.unitName, value };
                pieChartData.Add(obj);
            }
            return pieChartData;
        }
        public string[] Get_Year_PageNoListData(int pageno) {
            string[]PNLData=new string[3];
            var reportTime =(from r in m_BsnEntities.ReportTitle.ToList()
                            where r.PageNO == pageno
                            select new 
                            {
                                StartDateTime = Convert.ToDateTime(r.StartDateTime),
                                EndDateTime = Convert.ToDateTime(r.EndDateTime).AddDays(1)
                            }).ToList();
           DateTime startTiem=reportTime[0].StartDateTime;
           DateTime endTime = reportTime[0].EndDateTime;
           var guochengReport = (from r in m_BsnEntities.ReportTitle
                                 where r.StatisticalCycType == 6 && r.StartDateTime >= startTiem && r.EndDateTime < endTime && r.RPTType_Code == "XZ0"
                                 select new { PageNO = r.PageNO, UnitCode = r.UnitCode }).ToList();

            if (guochengReport.Count() > 0)
            {
                PNLData[0] = guochengReport.Count().ToString();
                int[] level = new int[guochengReport.Count()];
                int[] pagenoArr = new int[guochengReport.Count()];
                DisasterAssessment_title dt = new DisasterAssessment_title(m_Level);
                double pop = dt.getPopulation(guochengReport[0].UnitCode);
                double land = dt.getLandArea(guochengReport[0].UnitCode);
                for (int i = 0; i < guochengReport.Count(); i++)
                {
                    pagenoArr[i] = guochengReport[i].PageNO;
                    
                }
                level = Get_Year_Level(pagenoArr, pop, land, guochengReport[0].UnitCode);
                string[] str = Get_Number_Grade_Data(level);
                PNLData[1] = str[0];
                PNLData[2] = str[1];
            }
            else { 
                PNLData[0] = "0";
                PNLData[1] = "0";
                PNLData[2] = "没有单场灾情评估数据";
            }
            return PNLData;
 
        }
        public int[] Get_Year_Level(int[] pagenoArr, double pop, double land,string unitCode)
        {
            var hl011 = getHL011(pagenoArr, unitCode);
            var hl013 = from h3 in m_BsnEntities.HL013
                        where pagenoArr.Contains(h3.PageNO.Value) && h3.DW == "合计"
                        select new
                        {
                            UnitCode = h3.UnitCode,
                            PageNO = h3.PageNO,
                            SMXJT = h3.SMXJT,
                            GCYMLS = h3.GCYMLS,
                            SMXGS = h3.SMXGS,
                            SMXGD = h3.SMXGD,
                            SMXGQ = h3.SMXGQ,
                        };
            var dataList = (from h1 in hl011
                            join h3 in hl013
                            on new { h1.PageNO, h1.UnitCode } equals new { h3.PageNO, h3.UnitCode } into hl
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
            SingleEvaluation sEvaluation = new SingleEvaluation();
            int[] disasterLevel = new int[dataList.Count];
            for (int i = 0; i < dataList.Count;i++ ){
                disasterLevel[i] = sEvaluation.getGrade(dataList[i].SWRK, dataList[i].SZRK, dataList[i].SHMJXJ, dataList[i].ZJJJZSS, dataList[i].SLSSZJJJSS, dataList[i].DTFW,
                               dataList[i].SMXJT, dataList[i].GCYMLS, dataList[i].SMXGS, dataList[i].SMXGD, dataList[i].SMXGQ, pop, land);
            }
            return disasterLevel;
        }
        IList<HL011> getHL011(int[] pageNO, string unitCode)
        {
            var hl011 = (from h1 in m_BsnEntities.HL011.ToList()
                         from p in pageNO
                         where h1.PageNO==p && h1.UnitCode == unitCode
                         select h1
                         ).ToList();
            return hl011;
        }
        //根据场次数量和等级,得到当前年度灾情评估等级
        public string[] Get_Number_Grade_Data(int[] alist)
        {
            int number1 = 0;
            int number2 = 0;
            int number3 = 0;
            int number4 = 0;
            int singleevallevel = 0;
            string[] str = new string[2];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < alist.Count(); i++)
            {
                if (1 == Convert.ToInt32(alist[i]))
                {
                    number1++;
                }
                else if (2 == Convert.ToInt32(alist[i]))
                {
                    number2++;
                }
                else if (3 == Convert.ToInt32(alist[i]))
                {
                    number3++;
                }
                else
                {
                    number4++;
                }
            }
            int morethanlevel3 = number3 + number4;
            if (morethanlevel3 >= 0)
            {
                singleevallevel = 1;
            }
            if (number4 >= 1 || morethanlevel3 >= 2)
            {
                singleevallevel = 2;
            }
            if (number4 >= 2 || morethanlevel3 >= 3)
            {
                singleevallevel = 3;
            }
            if (number4 >= 3 || morethanlevel3 >= 4)
            {
                singleevallevel = 4;
            }
            sb.Append("其中");

            if (number4 != 0)
            {
                sb.Append("特大洪涝灾害(").Append(number4).Append("场)，");
            }
            if (number3 != 0)
            {
                sb.Append("重大洪涝灾害(").Append(number3).Append("场)，");
            }
            if (number2 != 0)
            {
                sb.Append("较大洪涝灾害(").Append(number2).Append("场)，");
            }
            if (number1 != 0)
            {
                sb.Append("一般洪涝灾害(").Append(number1).Append("场)");
            }
            str[0] = singleevallevel.ToString();
            str[1] = sb.ToString();
            return str;
        }
    }
}
