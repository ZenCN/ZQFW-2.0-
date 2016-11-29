using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using DBHelper;
using System.Transactions;
using System.Collections;
using System.Data;
using System.Net.Configuration;
using System.Reflection;
using EntityModel.RepeatModel;
using NPOI.SS.Formula.Functions;

namespace LogicProcessingClass.ReportOperate
{
    public class RiverDistribute
    {
        private FXDICTEntities fxdict;//字典库会话

        private BusinessEntities prvBusEntity;//省级库会话
        public RiverDistribute()
        {
            fxdict = Persistence.GetDbEntities();
            prvBusEntity = Persistence.GetDbEntities(2);
        }
        /// <summary>
        /// 根据单位代码获取流域、操作表代码关系类
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public RiverRPTypeInfo GetRiverRPTypeInfo(string unitCode)
        {
            if (CacheContext.RiverRPTypeInfo.ContainsKey(unitCode))
            {
                return CacheContext.RiverRPTypeInfo[unitCode];
            }
            else
            {
                RiverInfo rif = GetRiverInfo(unitCode, null);
                List<string> rivercode = rif.DRiverRate.Select(x => x.Key).ToList();

                var tb11s = (from RptType in fxdict.TB11_RptType
                             where rivercode.Contains(RptType.RvCode)
                             select RptType).AsQueryable();
                if (tb11s.Count() == 0) //暂时只发现黑龙江是这种特殊情况
                {
                    string code = rivercode[0];
                    if ("AAB00006,AB000000".Contains(code))
                    {
                        tb11s = (from RptType in fxdict.TB11_RptType
                                 where "AAB00006,AB000000".Contains(RptType.RvCode)
                                 select RptType).AsQueryable();
                    }
                }
                //if (tb11s.Count()==0)//暂时只发现黑龙江是这种特殊情况
                //{
                //    string code = rivercode[0];
                //   var tb112s = fxdict.TB11_RptType.Where("it.RvCode in {" + code.ToString() + "}").AsQueryable();//会报错
                //}
                RiverRPTypeInfo rtf = new RiverRPTypeInfo();
                rtf.Unitcode = unitCode;
                foreach (var tb11 in tb11s)
                {
                    if (unitCode.StartsWith("23"))//黑龙江特殊
                    {
                        rtf.DRiverRPType.Add(rivercode[0], tb11.RptTypeCode);
                    }
                    else
                    {
                        rtf.DRiverRPType.Add(tb11.RvCode, tb11.RptTypeCode); //流域代码和上报类型代码（XZ0等），如：AF000000，FF1
                    }
                }
                CacheContext.RiverRPTypeInfo.Add(rtf.Unitcode, rtf);
                return rtf;
            }
        }

        /// <summary>
        /// 根据单位代码从数据库中获取流域比例关系类（湖南的从前台传入）
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="rifs">前台传入的流域比例，为null</param>
        /// <returns>流域比例关系类</returns>
        public RiverInfo GetRiverInfo(string unitCode, List<RiverInfo> rifs)
        {
            RiverInfo riverinfo = new RiverInfo();
            //if (unitCode.StartsWith("23"))//黑龙江的
            //{
            //    riverinfo = HLJGetRiverInfo(unitCode);
            //}
            //else
            //{
            //    #region 非黑龙江

            var tb07s = from district in fxdict.TB07_District
                        where district.DistrictCode == unitCode
                        select district;

            var tb10s = from district in fxdict.TB07_District
                        from riverDistribute in district.TB10_RiverDistribute
                        where district.DistrictCode == unitCode
                        select riverDistribute;
            string rootCode = unitCode.Substring(0, 2) + "000000";//省级代码
            var prvtb07 = (from district in fxdict.TB07_District
                           where district.DistrictCode == rootCode
                           select district).FirstOrDefault();

            if (unitCode.StartsWith("43")) //湖南的，从前台传入数据，由于每次的比例可能不一样，所以不存入cache中
            {
                bool getRifByPage = false; //从前台获取流域数据是否成功
                if (rifs != null)
                {
                    foreach (var rif in rifs)
                    {
                        if (unitCode == rif.UnitCode)
                        {
                            riverinfo = rif;
                            getRifByPage = true;
                        }
                    }
                }
                if (!getRifByPage)
                {
                    string[] rivercodestr = tb07s.First().RD_RiverCode1.Split(',');
                    for (int i = 0; i < rivercodestr.Length; i++)
                    {
                        riverinfo.DRiverRate.Add(rivercodestr[i], 1);
                    }
                }
            }
            else
            {
                if (CacheContext.RiverInfoList.ContainsKey(unitCode))
                {
                    riverinfo = CacheContext.RiverInfoList[unitCode];
                }
                else
                {
                    riverinfo.UnitCode = unitCode;
                    if (tb10s.Count() > 0)
                    {
                        foreach (var riverDis in tb10s)
                        {
                            TB10_RiverDistribute river = riverDis as TB10_RiverDistribute;
                            riverinfo.DRiverRate.Add(river.RD_RiverCode.ToString(),
                                Convert.ToDouble(river.RDRate.ToString()));
                        }
                    }
                    else
                    {
                        //string[] rivercodestr = tb07s.First().RD_RiverCode1.Split(',');
                        //for (int i = 0; i < rivercodestr.Length; i++)
                        //{
                        //    riverinfo.DRiverRate.Add(rivercodestr[i], 1);
                        //}
                        if (tb07s.First().RD_RiverCode1 != null || tb07s.First().RD_RiverCode1.ToString() != "")//有流域代码
                        {
                            string[] rivercodestr = tb07s.First().RD_RiverCode1.Split(',');
                            for (int i = 0; i < rivercodestr.Length; i++)
                            {
                                riverinfo.DRiverRate.Add(rivercodestr[i], 1);
                            }
                        }
                        else//不属于任何流域，那么分配的数据就是0
                        {
                            string[] prvRivercodestr = prvtb07.RD_RiverCode1.Split(',');
                            for (int i = 0; i < prvRivercodestr.Length; i++)
                            {
                                riverinfo.DRiverRate.Add(prvRivercodestr[i], 0);
                            }
                        }
                    }
                    CacheContext.RiverInfoList.Add(riverinfo.UnitCode, riverinfo);
                }
            }

            //    #endregions
            //}
            return riverinfo;
        }

        /// <summary>黑龙江设置流域
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="rifs">前台传入的流域比例，为null</param>
        /// <returns>流域比例关系类</returns>
        public RiverInfo HLJGetRiverInfo(string unitCode)
        {
            var tb07s = from district in fxdict.TB07_District
                        where district.DistrictCode == unitCode
                        select district;

            var tb10s = from district in fxdict.TB07_District
                        from riverDistribute in district.TB10_RiverDistribute
                        where district.DistrictCode == unitCode
                        select riverDistribute;
            string rootCode = unitCode.Substring(0, 2) + "000000";//省级代码
            var prvtb07 = (from district in fxdict.TB07_District
                           where district.DistrictCode == rootCode
                           select district).FirstOrDefault();

            RiverInfo riverinfo = new RiverInfo();
            if (CacheContext.RiverInfoList.ContainsKey(unitCode))
            {
                riverinfo = CacheContext.RiverInfoList[unitCode];
            }
            else
            {
                riverinfo.UnitCode = unitCode;
                if (tb10s.Count() > 0)
                {
                    foreach (var riverDis in tb10s)
                    {
                        TB10_RiverDistribute river = riverDis as TB10_RiverDistribute;
                        riverinfo.DRiverRate.Add(river.RD_RiverCode.ToString(), Convert.ToDouble(river.RDRate.ToString()));
                    }
                }
                else
                {
                    if (tb07s.First().RD_RiverCode1 != null || tb07s.First().RD_RiverCode1.ToString() != "")//有流域代码
                    {
                        string[] rivercodestr = tb07s.First().RD_RiverCode1.Split(',');
                        for (int i = 0; i < rivercodestr.Length; i++)
                        {
                            riverinfo.DRiverRate.Add(rivercodestr[i], 1);
                        }
                    }
                    else//不属于任何流域，那么分配的数据就是0
                    {
                        string[] prvRivercodestr = prvtb07.RD_RiverCode1.Split(',');
                        for (int i = 0; i < prvRivercodestr.Length; i++)
                        {
                            riverinfo.DRiverRate.Add(prvRivercodestr[i], 0);
                        }
                    }
                }
                CacheContext.RiverInfoList.Add(riverinfo.UnitCode, riverinfo);
            }

            return riverinfo;
        }

        /// <summary>
        /// 更新流域表之前，先清除已经存在的流域表(使用TransactionScope 事务的时候，可能会发生错误)
        /// </summary>
        /// <param name="pageNO">与之关联的行政表页号，非流域表页号</param>
        /// <returns>bool</returns>
        public bool DeleteRiverReport(int pageNO)
        {
            bool delFlag = false;
            //using (TransactionScope scope = new TransactionScope())
            //{
            try
            {
                var rpts = prvBusEntity.ReportTitle.Where(t => t.AssociatedPageNO == pageNO).Select(t => t.PageNO);
                string riverPageNOs = string.Join(",", rpts.ToArray());
                var hl011s = prvBusEntity.HL011.Where("it.PageNO in {" + riverPageNOs + "}");
                var hl012s = prvBusEntity.HL012.Where("it.PageNO in {" + riverPageNOs + "}");
                var hl013s = prvBusEntity.HL013.Where("it.PageNO in {" + riverPageNOs + "}");
                var hl014s = prvBusEntity.HL014.Where("it.PageNO in {" + riverPageNOs + "}");
                var hp011s = prvBusEntity.HP011.Where("it.PageNO in {" + riverPageNOs + "}");
                var hp012s = prvBusEntity.HP012.Where("it.PageNO in {" + riverPageNOs + "}");
                var delRpts = prvBusEntity.ReportTitle.Where(t => t.AssociatedPageNO == pageNO);

                foreach (var hl011 in hl011s)
                {
                    prvBusEntity.HL011.DeleteObject(hl011);
                }
                foreach (var hl012 in hl012s)
                {
                    prvBusEntity.HL012.DeleteObject(hl012);
                }
                foreach (var hl013 in hl013s)
                {
                    prvBusEntity.HL013.DeleteObject(hl013);
                }
                foreach (var hl014 in hl014s)
                {
                    prvBusEntity.HL014.DeleteObject(hl014);
                }
                foreach (var hp011 in hp011s)
                {
                    prvBusEntity.HP011.DeleteObject(hp011);
                }
                foreach (var hp012 in hp012s)
                {
                    prvBusEntity.HP012.DeleteObject(hp012);
                }

                foreach (var rpt in delRpts)
                {
                    prvBusEntity.ReportTitle.DeleteObject(rpt);
                }
                prvBusEntity.SaveChanges();
                //scope.Complete();
                delFlag = true;
                //}
            }
            catch (Exception)
            {
                delFlag = false;
            }
            return delFlag;
        }

        /// <summary>
        /// 判断该行政表是否已经进行流域分配
        /// </summary>
        /// <param name="pageNO">行政表页号</param>
        /// <returns></returns>
        public bool IsRiverDistribute(int pageNO)
        {
            bool isRiverFlag = false;
            int count = (from rpt in prvBusEntity.ReportTitle
                         where rpt.AssociatedPageNO == pageNO &&
                         rpt.RPTType_Code != "XZ0"
                         select rpt).Count();
            if (count > 0)
            {
                isRiverFlag = true;
            }
            return isRiverFlag;
        }

        /// <summary>
        /// 单流域保存
        /// </summary>
        /// <param name="xzRpt">需要进行流域分配的行政表</param>
        /// <param name="rptTypeCode">流域上报类型FF1等</param>
        /// <returns>bool</returns>
        public bool SaveSingleRiverDistribute(ReportTitle xzRpt, string rptTypeCode)
        {
            ReportTitle riverRpt = new ReportTitle();
            ReportHelpClass rptHelp = new ReportHelpClass();
            riverRpt = rptHelp.CloneEF<ReportTitle>(xzRpt);
            riverRpt.PageNO = prvBusEntity.ReportTitle.Max(t => t.PageNO) + 1;
            riverRpt.RPTType_Code = rptTypeCode;
            riverRpt.AssociatedPageNO = xzRpt.PageNO;
            IList<HL011> hl011stemp = xzRpt.HL011.ToList();
            IList<HL012> hl012stemp = xzRpt.HL012.ToList();
            IList<HL013> hl013stemp = xzRpt.HL013.ToList();
            IList<HL014> hl014stemp = xzRpt.HL014.ToList();
            /********************蓄水表*******/
            IList<HP011> hp011stemp = xzRpt.HP011.ToList();
            IList<HP012> hp012stemp = xzRpt.HP012.ToList();
            /*******************end 蓄水*****/
            if (xzRpt.HL011.Count() > 0)
            {
                foreach (var newHl011 in hl011stemp)
                {
                    //newHl011.ReportTitle = riverRpt;】

                    if (rptTypeCode == "AA2" && (newHl011.UnitCode == "22030000" || newHl011.UnitCode == "22040000"))
                    //松花江流域，不包括四平市、辽源市
                    {
                        continue;
                    }
                    else if (rptTypeCode == "BB2" && (newHl011.UnitCode != "22030000" && newHl011.UnitCode != "22040000" && newHl011.UnitCode != "22000000"))
                    {
                        continue;
                    }

                    HL011 riverHl011 = new HL011();
                    riverHl011 = rptHelp.CloneEF<HL011>(newHl011);
                    riverHl011.PageNO = riverRpt.PageNO;
                    riverRpt.HL011.Add(riverHl011);
                }

                if (riverRpt.UnitCode.StartsWith("22") && riverRpt.HL011.Count > 0)
                {
                    if (riverRpt.HL011.Count < 2)  //只有一个
                    {
                        IList<HL011> hl011s = riverRpt.HL011.ToList();
                        if (hl011s[0].UnitCode != hl011s[0].UnitCode.Substring(0, 2) + "000000")  //非合计行
                        {
                            hl011s[0].UnitCode = hl011s[0].UnitCode.Substring(0, 2) + "000000";
                            hl011s[0].DW = "合计";
                            riverRpt.HL011.Add(hl011s[0]);
                        }
                        else
                        {
                            riverRpt.HL011.Clear();
                        }
                    }
                    else
                    {
                        HL011 hl01hj = riverRpt.HL011.Cast<HL011>().Where(x => x.DW == "合计").ToList<HL011>()[0];
                        HL011 hl011temp = new HL011();
                        IList<HL011> hl011 = riverRpt.HL011.Cast<HL011>().Where(x => x.DW != "合计").ToList<HL011>();
                        foreach (var hl01 in hl011)
                        {
                            TObjectSum<HL011>(hl011temp, hl01);
                        }
                        UnionObject<HL011>(hl01hj, hl011temp);
                    }
                }
            }
            if (xzRpt.HL012.Count() > 0)
            {
                foreach (var newHl012 in hl012stemp)
                {
                    //newHl012.ReportTitle = riverRpt;
                    if (rptTypeCode == "AA2" && (newHl012.UnitCode == "22030000" || newHl012.UnitCode == "22040000"))
                    //松花江流域，不包括四平市、辽源市
                    {
                        continue;
                    }
                    else if (rptTypeCode == "BB2" && (newHl012.UnitCode != "22030000" && newHl012.UnitCode != "22040000"))
                    {
                        continue;
                    }

                    HL012 riverHl012 = new HL012();
                    riverHl012 = rptHelp.CloneEF<HL012>(newHl012);
                    riverHl012.PageNO = riverRpt.PageNO;
                    riverRpt.HL012.Add(riverHl012);
                }
            }
            if (xzRpt.HL013.Count() > 0)
            {
                foreach (var newHl013 in hl013stemp)
                {
                    //newHl013.ReportTitle = riverRpt;
                    if (rptTypeCode == "AA2" && (newHl013.UnitCode == "22030000" || newHl013.UnitCode == "22040000"))
                    //松花江流域，不包括四平市、辽源市
                    {
                        continue;
                    }
                    else if (rptTypeCode == "BB2" && (newHl013.UnitCode != "22030000" && newHl013.UnitCode != "22040000" && newHl013.UnitCode != "22000000"))
                    {
                        continue;
                    }

                    HL013 riverHl013 = new HL013();
                    riverHl013 = rptHelp.CloneEF<HL013>(newHl013);
                    riverHl013.PageNO = riverRpt.PageNO;
                    riverRpt.HL013.Add(riverHl013);
                }

                if (riverRpt.UnitCode.StartsWith("22") && riverRpt.HL013.Count > 0)
                {
                    if (riverRpt.HL013.Count < 2)  //只有一个
                    {
                        IList<HL013> hl013s = riverRpt.HL013.ToList();
                        if (hl013s[0].UnitCode != hl013s[0].UnitCode.Substring(0, 2) + "000000")  //非合计行
                        {
                            hl013s[0].UnitCode = hl013s[0].UnitCode.Substring(0, 2) + "000000";
                            hl013s[0].DW = "合计";
                            riverRpt.HL013.Add(hl013s[0]);
                        }
                        else
                        {
                            riverRpt.HL013.Clear();
                        }
                    }
                    else
                    {
                        var list = riverRpt.HL013.Cast<HL013>().Where(x => x.DW == "合计").ToList<HL013>();
                        if (list.Count > 0)
                        {
                            HL013 hl03hj = list[0];
                            HL013 hl013temp = new HL013();
                            IList<HL013> hl013 = riverRpt.HL013.Cast<HL013>().Where(x => x.DW != "合计").ToList<HL013>();
                            foreach (var hl03 in hl013)
                            {
                                TObjectSum<HL013>(hl013temp, hl03);
                            }
                            UnionObject<HL013>(hl03hj, hl013temp);
                        }
                    }
                }
            }
            if (xzRpt.HL014.Count() > 0)
            {
                foreach (var newHl014 in hl014stemp)
                {
                    //newHl014.ReportTitle = riverRpt;
                    if (rptTypeCode == "AA2" && (newHl014.UnitCode == "22030000" || newHl014.UnitCode == "22040000"))
                    //松花江流域，不包括四平市、辽源市
                    {
                        continue;
                    }
                    else if (rptTypeCode == "BB2" && (newHl014.UnitCode != "22030000" && newHl014.UnitCode != "22040000" && newHl014.UnitCode != "22000000"))
                    {
                        continue;
                    }

                    HL014 riverHl014 = new HL014();
                    riverHl014 = rptHelp.CloneEF<HL014>(newHl014);
                    riverHl014.PageNO = riverRpt.PageNO;
                    riverRpt.HL014.Add(riverHl014);
                }

                if (riverRpt.UnitCode.StartsWith("22") && riverRpt.HL014.Count > 0)
                {
                    if (riverRpt.HL014.Count < 2)  //只有一个
                    {
                        IList<HL014> hl014s = riverRpt.HL014.ToList();
                        if (hl014s[0].UnitCode != hl014s[0].UnitCode.Substring(0, 2) + "000000")  //非合计行
                        {
                            hl014s[0].UnitCode = hl014s[0].UnitCode.Substring(0, 2) + "000000";
                            hl014s[0].DW = "合计";
                            riverRpt.HL014.Add(hl014s[0]);
                        }
                        else
                        {
                            riverRpt.HL014.Clear();
                        }
                    }
                    else
                    {
                        HL014 hl04hj = riverRpt.HL014.Cast<HL014>().Where(x => x.DW == "合计").ToList<HL014>()[0];
                        HL014 hl014temp = new HL014();
                        IList<HL014> hl014 = riverRpt.HL014.Cast<HL014>().Where(x => x.DW != "合计").ToList<HL014>();
                        foreach (var hl04 in hl014)
                        {
                            TObjectSum<HL014>(hl014temp, hl04);
                        }
                        UnionObject<HL014>(hl04hj, hl014temp);
                    }
                }
            }
            if (xzRpt.HP011.Count() > 0)
            {
                foreach (var newHp011 in hp011stemp)
                {
                    //newHp011.ReportTitle = riverRpt;
                    HP011 riverHp011 = new HP011();
                    riverHp011 = rptHelp.CloneEF<HP011>(newHp011);
                    riverHp011.PAGENO = riverRpt.PageNO;
                    riverRpt.HP011.Add(riverHp011);
                }
            }
            if (xzRpt.HP012.Count() > 0)
            {
                foreach (var newHp012 in hp012stemp)
                {
                    //newHp012.ReportTitle = riverRpt;
                    HP012 riverHp012 = new HP012();
                    riverHp012 = rptHelp.CloneEF<HP012>(newHp012);
                    riverHp012.PAGENO = riverRpt.PageNO;
                    riverRpt.HP012.Add(riverHp012);
                }
            }

            prvBusEntity.ReportTitle.AddObject(riverRpt);
            bool singleRiveFlag = false;


            //using (TransactionScope scope = new TransactionScope())
            //{


            try
            {
                prvBusEntity.SaveChanges();
                //scope.Complete();
                singleRiveFlag = true;
                //}
            }
            catch (Exception)
            {
                singleRiveFlag = false;
            }
            return singleRiveFlag;
        }

        public bool SingleRiver(int pageno)
        {
            if (IsRiverDistribute(pageno) && !DeleteRiverReport(pageno)) //是否已经进行流域分配了
            {
                return false;
            }

            bool success = false;
            ReportTitle xz_rpt = prvBusEntity.ReportTitle.SingleOrDefault(t => t.PageNO == pageno);
            RiverRPTypeInfo river_type_info = GetRiverRPTypeInfo(xz_rpt.UnitCode);

            ReportTitle river_rpt = new ReportTitle();
            ReportHelpClass rptHelp = new ReportHelpClass();
            river_rpt = rptHelp.CloneEF<ReportTitle>(xz_rpt);
            river_rpt.PageNO = rptHelp.FindMaxPageNO(2);
            river_rpt.RPTType_Code = river_type_info.DRiverRPType.ElementAt(0).Value;
            river_rpt.AssociatedPageNO = xz_rpt.PageNO;
            IList<HL011> hl011stemp = xz_rpt.HL011.ToList();
            IList<HL012> hl012stemp = xz_rpt.HL012.ToList();
            IList<HL013> hl013stemp = xz_rpt.HL013.ToList();
            IList<HL014> hl014stemp = xz_rpt.HL014.ToList();

            if (xz_rpt.HL011.Count() > 0)
            {
                foreach (var newHl011 in hl011stemp)
                {

                    HL011 riverHl011 = new HL011();
                    riverHl011 = rptHelp.CloneEF<HL011>(newHl011);
                    riverHl011.PageNO = river_rpt.PageNO;
                    river_rpt.HL011.Add(riverHl011);
                }

                if (river_rpt.HL011.Count > 0)
                {
                    if (river_rpt.HL011.Count < 2)  //只有一个
                    {
                        IList<HL011> hl011s = river_rpt.HL011.ToList();
                        if (hl011s[0].UnitCode != hl011s[0].UnitCode.Substring(0, 2) + "000000")  //非合计行
                        {
                            hl011s[0].UnitCode = hl011s[0].UnitCode.Substring(0, 2) + "000000";
                            hl011s[0].DW = "合计";
                            river_rpt.HL011.Add(hl011s[0]);
                        }
                        else
                        {
                            river_rpt.HL011.Clear();
                        }
                    }
                    else
                    {
                        HL011 hl01hj = river_rpt.HL011.Cast<HL011>().Where(x => x.DW == "合计").ToList<HL011>()[0];
                        HL011 hl011temp = new HL011();
                        IList<HL011> hl011 = river_rpt.HL011.Cast<HL011>().Where(x => x.DW != "合计").ToList<HL011>();
                        foreach (var hl01 in hl011)
                        {
                            TObjectSum<HL011>(hl011temp, hl01);
                        }
                        UnionObject<HL011>(hl01hj, hl011temp);
                    }
                }
            }
            if (xz_rpt.HL012.Count() > 0)
            {
                foreach (var newHl012 in hl012stemp)
                {
                    HL012 riverHl012 = new HL012();
                    riverHl012 = rptHelp.CloneEF<HL012>(newHl012);
                    riverHl012.PageNO = river_rpt.PageNO;
                    river_rpt.HL012.Add(riverHl012);
                }
            }
            if (xz_rpt.HL013.Count() > 0)
            {
                foreach (var newHl013 in hl013stemp)
                {
                    HL013 riverHl013 = new HL013();
                    riverHl013 = rptHelp.CloneEF<HL013>(newHl013);
                    riverHl013.PageNO = river_rpt.PageNO;
                    river_rpt.HL013.Add(riverHl013);
                }

                if (river_rpt.HL013.Count > 0)
                {
                    if (river_rpt.HL013.Count < 2)  //只有一个
                    {
                        IList<HL013> hl013s = river_rpt.HL013.ToList();
                        if (hl013s[0].UnitCode != hl013s[0].UnitCode.Substring(0, 2) + "000000")  //非合计行
                        {
                            hl013s[0].UnitCode = hl013s[0].UnitCode.Substring(0, 2) + "000000";
                            hl013s[0].DW = "合计";
                            river_rpt.HL013.Add(hl013s[0]);
                        }
                        else
                        {
                            river_rpt.HL013.Clear();
                        }
                    }
                    else
                    {
                        var list = river_rpt.HL013.Cast<HL013>().Where(x => x.DW == "合计").ToList<HL013>();
                        if (list.Count > 0)
                        {
                            HL013 hl03hj = list[0];
                            HL013 hl013temp = new HL013();
                            IList<HL013> hl013 = river_rpt.HL013.Cast<HL013>().Where(x => x.DW != "合计").ToList<HL013>();
                            foreach (var hl03 in hl013)
                            {
                                TObjectSum<HL013>(hl013temp, hl03);
                            }
                            UnionObject<HL013>(hl03hj, hl013temp);
                        }
                    }
                }
            }
            if (xz_rpt.HL014.Count() > 0)
            {
                foreach (var newHl014 in hl014stemp)
                {
                    HL014 riverHl014 = new HL014();
                    riverHl014 = rptHelp.CloneEF<HL014>(newHl014);
                    riverHl014.PageNO = river_rpt.PageNO;
                    river_rpt.HL014.Add(riverHl014);
                }

                if (river_rpt.HL014.Count > 0)
                {
                    if (river_rpt.HL014.Count < 2)  //只有一个
                    {
                        IList<HL014> hl014s = river_rpt.HL014.ToList();
                        if (hl014s[0].UnitCode != hl014s[0].UnitCode.Substring(0, 2) + "000000")  //非合计行
                        {
                            hl014s[0].UnitCode = hl014s[0].UnitCode.Substring(0, 2) + "000000";
                            hl014s[0].DW = "合计";
                            river_rpt.HL014.Add(hl014s[0]);
                        }
                        else
                        {
                            river_rpt.HL014.Clear();
                        }
                    }
                    else
                    {
                        HL014 hl04hj = river_rpt.HL014.Cast<HL014>().Where(x => x.DW == "合计").ToList<HL014>()[0];
                        HL014 hl014temp = new HL014();
                        IList<HL014> hl014 = river_rpt.HL014.Cast<HL014>().Where(x => x.DW != "合计").ToList<HL014>();
                        foreach (var hl04 in hl014)
                        {
                            TObjectSum<HL014>(hl014temp, hl04);
                        }
                        UnionObject<HL014>(hl04hj, hl014temp);
                    }
                }
            }

            prvBusEntity.ReportTitle.AddObject(river_rpt);

            try
            {
                prvBusEntity.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// 流域分配（包含从前台传入流域比例的分配，当rInfos为空时，且UnitCode不是湖南时从数据库中取流域比例）
        /// </summary>
        /// <param name="pageNO">行政表页号</param>
        /// <param name="rInfos">前台传入的流域比例信息</param>
        /// <returns></returns>
        public bool SaveRiverDistribute(int pageNO, List<RiverInfo> rInfos)
        {
            bool saveFlag = false;
            ReportTitle xzRpt = prvBusEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
            RiverRPTypeInfo prvRiverTypeInfo = GetRiverRPTypeInfo(xzRpt.UnitCode);
            if (IsRiverDistribute(pageNO))//是否已经进行流域分配了
            { 
                if (!DeleteRiverReport(pageNO))
                {
                    return false;
                }
            }

            /*if (xzRpt.UnitCode.StartsWith("22"))
            {
                prvRiverTypeInfo = new RiverRPTypeInfo();
                if (!prvRiverTypeInfo.DRiverRPType.ContainsKey("AB000000"))
                {
                    prvRiverTypeInfo.DRiverRPType.Add("AB000000", "BB2");
                }

                if (!prvRiverTypeInfo.DRiverRPType.ContainsKey("AAB00006"))
                {
                    prvRiverTypeInfo.DRiverRPType.Add("AAB00006", "AA2");
                }
            }*/

            if (prvRiverTypeInfo.DRiverRPType.Count == 1 && !xzRpt.UnitCode.StartsWith("23"))//除了黑龙江（23...）的单流域保存
            {
                string rptTypeCode = prvRiverTypeInfo.DRiverRPType.ElementAt(0).Value;
                return SaveSingleRiverDistribute(xzRpt, rptTypeCode);
            }
            Dictionary<string, List<string>> dicIntField = new Dictionary<string, List<string>>();
            if (xzRpt.UnitCode.StartsWith("36"))
            {
                dicIntField = GetIntRiverField();
            }
            //初始化或获取当前应用程序缓存中指定的UnitCode所对应的RiverInfo类，查出当前单位对应的流域代码及该流域的比率
            RiverInfo prfriverinfo = GetRiverInfo(xzRpt.UnitCode, rInfos);//注意该地方取数据的不同（前后台）********************
            ReportHelpClass rptHelp = new ReportHelpClass();
            //用来存放已分配好的行政报表ReportTitle
            IDictionary<string, ReportTitle> pRptList = new Dictionary<string, ReportTitle>();
            int maxPageNO = rptHelp.FindMaxPageNO(2);
            foreach (var prti in prvRiverTypeInfo.DRiverRPType)
            {
                ReportTitle riverRpt = rptHelp.CloneEF<ReportTitle>(xzRpt);
                riverRpt.PageNO = rptHelp.FindMaxPageNO(2);
                //riverRpt.PageNO = 0;
                riverRpt.AssociatedPageNO = xzRpt.PageNO;
                riverRpt.LastUpdateTime = DateTime.Now;
                riverRpt.ReceiveTime = DateTime.Now;
                riverRpt.RPTType_Code = prti.Value;//上报类型
                pRptList.Add(prti.Key, riverRpt);
            }

            foreach (var rpt in pRptList)
            {
                //rpt.Value.Affix = null;
                //rpt.Value.HL011 = null;
                //rpt.Value.HL012 = null;
                //rpt.Value.HL013 = null;
                //rpt.Value.HL014 = null;
                //rpt.Value.HP011 = null;
                //rpt.Value.HP012 = null;
                #region HL011-HL014数据按流域分配
                if (xzRpt.HL011.Count > 0)
                {
                    foreach (var hl011 in xzRpt.HL011)
                    {
                        HL011 newHl011 = new HL011();
                        RiverInfo pRiverInfo = GetRiverInfo(hl011.UnitCode, rInfos);
                        if (pRiverInfo.DRiverRate.Keys.Contains(rpt.Key))
                        {
                            List<string> hl011fieldlist = null;
                            dicIntField.TryGetValue("HL011", out hl011fieldlist);
                            newHl011 = ObjectMultimun<HL011>(hl011, pRiverInfo.DRiverRate[rpt.Key], hl011fieldlist);
                            newHl011.PageNO = rpt.Value.PageNO;
                            rpt.Value.HL011.Add(newHl011);
                        }
                    }
                }
                else
                {
                    rpt.Value.HL011 = null;
                    rpt.Value.HL011.Clear();
                }

                if (xzRpt.HL012.Count > 0)
                {
                    foreach (var hl012 in xzRpt.HL012)
                    {
                        HL012 newHl012 = new HL012();
                        RiverInfo pRiverInfo = GetRiverInfo(hl012.UnitCode, rInfos);
                        //if (pRiverInfo.DRiverRate.Keys.Contains(rpt.Key))
                        if (hl012.RiverCode == rpt.Key)
                        {
                            newHl012 = rptHelp.CloneEF<HL012>(hl012);
                            newHl012.PageNO = rpt.Value.PageNO;
                            rpt.Value.HL012.Add(newHl012);
                        }
                    }
                }
                else
                {
                    rpt.Value.HL012 = null;
                    rpt.Value.HL012.Clear();
                }

                if (xzRpt.HL013.Count > 0)
                {
                    foreach (var hl013 in xzRpt.HL013)
                    {
                        HL013 newHl013 = new HL013();
                        //RiverInfo pRiverInfo = GetRiverInfo(hl013.UnitCode, rInfos);//合计行没有传入UnitCode
                        //if (pRiverInfo.DRiverRate.Keys.Contains(rpt.Key))
                        if (hl013.RiverCode == rpt.Key || hl013.DW == "合计")
                        {
                            newHl013 = rptHelp.CloneEF<HL013>(hl013);
                            newHl013.PageNO = rpt.Value.PageNO;
                            rpt.Value.HL013.Add(newHl013);
                        }
                    }
                }
                else
                {
                    rpt.Value.HL013 = null;
                    rpt.Value.HL013.Clear();
                }

                if (xzRpt.HL014.Count > 0)
                {
                    foreach (var hl014 in xzRpt.HL014)
                    {
                        HL014 newHl014 = new HL014();
                        RiverInfo pRiverInfo = GetRiverInfo(hl014.UnitCode, rInfos);
                        if (pRiverInfo.DRiverRate.Keys.Contains(rpt.Key))
                        {
                            List<string> hl014fieldlist = null;
                            dicIntField.TryGetValue("HL014", out hl014fieldlist);
                            newHl014 = ObjectMultimun<HL014>(hl014, pRiverInfo.DRiverRate[rpt.Key], hl014fieldlist);
                            newHl014.PageNO = rpt.Value.PageNO;
                            rpt.Value.HL014.Add(newHl014);
                        }
                    }
                }
                else
                {
                    rpt.Value.HL014 = null;
                    rpt.Value.HL014.Clear();
                }
                #endregion

                #region//处理合计
                if (rpt.Value.HL011.Count < 2)
                {
                    //rpt.Value.HL011 = null;
                    rpt.Value.HL011.Clear();
                }
                else
                {
                    HL011 hl01hj = rpt.Value.HL011.Cast<HL011>().Where(x => x.DW == "合计").ToList<HL011>()[0];
                    HL011 hl011temp = new HL011();
                    IList<HL011> hl011 = rpt.Value.HL011.Cast<HL011>().Where(x => x.DW != "合计").ToList<HL011>();
                    int val = 0;
                    foreach (var hl01 in hl011)
                    {
                        if (hl01.SZFWX != null)
                        {
                            val = Convert.ToInt16(hl01.SZFWX);
                            hl01.SZFWX = val;
                        }

                        if (hl01.SZFWZ != null)
                        {
                            val = Convert.ToInt16(hl01.SZFWZ);
                            hl01.SZFWZ = val;
                        }

                        TObjectSum<HL011>(hl011temp, hl01);
                    }
                    UnionObject<HL011>(hl01hj, hl011temp);
                }
                if (rpt.Value.HL013.Count < 2)
                {
                    //rpt.Value.HL013 = null;
                    rpt.Value.HL013.Clear();
                }
                else
                {
                    HL013 hl03hj = rpt.Value.HL013.Cast<HL013>().Where(x => x.DW == "合计").ToList<HL013>()[0];
                    HL013 hl013temp = new HL013();
                    IList<HL013> hl013 = rpt.Value.HL013.Cast<HL013>().Where(x => x.DW != "合计").ToList<HL013>();
                    foreach (var hl03 in hl013)
                    {
                        TObjectSum<HL013>(hl013temp, hl03);
                    }
                    UnionObject<HL013>(hl03hj, hl013temp);

                    if (rpt.Value.UnitCode.StartsWith("35"))
                    {
                        hl03hj.YMFWBL = null;
                        hl03hj.GCYMLS = null;
                        hl03hj.GCLJJYL = null;
                        hl03hj.SMXGD = null;
                        hl03hj.SMXGQ = null;
                        hl03hj.SMXGS = null;
                        hl03hj.SMXJT = null;
                    }
                    else
                    {
                        hl03hj.YMFWBL = rpt.Value.HL013.Where(h => h.DW != "合计").Average(h => h.YMFWBL);
                    }
                }

                if (rpt.Value.HL014.Count < 2)
                {
                    //rpt.Value.HL014 = null;
                    rpt.Value.HL014.Clear();
                }
                else
                {
                    HL014 hl04hj = rpt.Value.HL014.Cast<HL014>().Where(x => x.DW == "合计").ToList<HL014>()[0];
                    HL014 hl014temp = new HL014();
                    IList<HL014> hl014 = rpt.Value.HL014.Cast<HL014>().Where(x => x.DW != "合计").ToList<HL014>();
                    foreach (var hl04 in hl014)
                    {
                        TObjectSum<HL014>(hl014temp, hl04);

                    }
                    UnionObject<HL014>(hl04hj, hl014temp);
                }
                #endregion

                #region //处理死记人口和受淹城市  2.0添加了失踪人口
                foreach (var hl01 in rpt.Value.HL011)
                {
                    HL011 hl011temp = hl01 as HL011;
                    if (rpt.Value.HL012.Count > 0)
                    {
                        hl011temp.SWRK = rpt.Value.HL012.Cast<HL012>().Count(x =>
                        {
                            if (hl011temp.DW == "合计")
                            {
                                return (x.RiverCode == rpt.Key) && (x.DataType == "死亡");
                            }
                            else
                            {
                                return (x.UnitCode == hl011temp.UnitCode) && (x.RiverCode == rpt.Key) && (x.DataType == "死亡");
                            }
                        });
                    }
                    else
                    {
                        hl011temp.SWRK = 0;
                    }
                    if (rpt.Value.HL012.Count > 0)//失踪人口
                    {
                        hl011temp.SZRKR = rpt.Value.HL012.Cast<HL012>().Count(x =>
                        {
                            if (hl011temp.DW == "合计")
                            {
                                return (x.RiverCode == rpt.Key) && (x.DataType == "失踪");
                            }
                            else
                            {
                                return (x.UnitCode == hl011temp.UnitCode) && (x.RiverCode == rpt.Key) && (x.DataType == "失踪");
                            }
                        });
                    }
                    else
                    {
                        hl011temp.SZRKR = 0;
                    }

                    if (rpt.Value.HL013.Count > 0)
                    {

                        hl011temp.SYCS = rpt.Value.HL013.Cast<HL013>().Count(x =>
                        {
                            if (hl011temp.DW == "合计")
                            {
                                return (x.RiverCode == rpt.Key) && (x.DW != "合计");
                            }
                            else
                            {
                                return (x.UnitCode == hl011temp.UnitCode) && (x.RiverCode == rpt.Key);
                            }
                        });
                    }
                    else
                    {
                        hl011temp.SYCS = 0;
                    }
                }
                #endregion

                prvBusEntity.ReportTitle.AddObject(rpt.Value);
                if (xzRpt.UnitCode.StartsWith("23"))
                {
                    var riverRpt = rptHelp.CloneEF(rpt.Value);
                    riverRpt.RPTType_Code = "AB1";
                    riverRpt.PageNO = maxPageNO + 1;
                    HL011 newHl011 = null;
                    HL012 newHl012 = null;
                    HL013 newHl013 = null;
                    HL014 newHl014 = null;

                    foreach (var hl011 in rpt.Value.HL011)
                    {
                        newHl011 = rptHelp.CloneEF(hl011);
                        newHl011.PageNO = riverRpt.PageNO;
                        riverRpt.HL011.Add(newHl011);
                    }
                    foreach (var hl012 in rpt.Value.HL012)
                    {
                        newHl012 = rptHelp.CloneEF(hl012);
                        newHl012.PageNO = riverRpt.PageNO;
                        riverRpt.HL012.Add(newHl012);
                    }
                    foreach (var hl013 in rpt.Value.HL013)
                    {
                        newHl013 = rptHelp.CloneEF(hl013);
                        newHl013.PageNO = riverRpt.PageNO;
                        riverRpt.HL013.Add(newHl013);
                    }
                    foreach (var hl014 in rpt.Value.HL014)
                    {
                        newHl014 = rptHelp.CloneEF(hl014);
                        newHl014.PageNO = riverRpt.PageNO;
                        riverRpt.HL014.Add(newHl014);
                    }

                    if (riverRpt.HL011.Count > 0 || riverRpt.HL012.Count > 0 || riverRpt.HL013.Count > 0 ||
                    riverRpt.HL014.Count > 0)
                    {
                        prvBusEntity.ReportTitle.AddObject(riverRpt);
                    }
                }
            }
            try
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                prvBusEntity.SaveChanges();
                saveFlag = true;
                //}
            }
            catch (Exception ex)
            {
            }
            return saveFlag;
        }

        /// <summary>内蒙蓄水
        /// </summary>
        /// <param name="pageNO"></param>
        /// <param name="rInfos"></param>
        /// <returns></returns>
        public bool NMSaveRiverDistribute(int pageNO, List<RiverInfo> rInfos)
        {
            bool saveFlag = false;
            BusinessEntities ctyBusEntity = Persistence.GetDbEntities(3);
            ReportTitle xzRpt = prvBusEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
            List<AggAccRecord> aggs = new List<AggAccRecord>();
            if(xzRpt.SourceType  == 1){
                aggs = prvBusEntity.AggAccRecord.Where(t => t.PageNo == pageNO && t.OperateType == 1).ToList<AggAccRecord>();  //汇总的

            }
            else if (xzRpt.SourceType == 2) {
                var tmp = prvBusEntity.AggAccRecord.Where(t => t.PageNo == pageNO && t.OperateType == 2).Select(s => s.SPageNO).ToList();
                aggs = prvBusEntity.AggAccRecord.Where(t => tmp.Contains(t.PageNo) && t.OperateType == 1).ToList<AggAccRecord>();
            }

            RiverRPTypeInfo prvRiverTypeInfo = GetRiverRPTypeInfo(xzRpt.UnitCode);
            if (IsRiverDistribute(pageNO))//是否已经进行流域分配了
            {
                if (!DeleteRiverReport(pageNO))
                {
                    return false;
                }
            }
            Dictionary<string, List<string>> dicIntField = new Dictionary<string, List<string>>();


            //初始化或获取当前应用程序缓存中指定的UnitCode所对应的RiverInfo类，查出当前单位对应的流域代码及该流域的比率
            RiverInfo prfriverinfo = GetRiverInfo(xzRpt.UnitCode, rInfos);//注意该地方取数据的不同（前后台）********************
            ReportHelpClass rptHelp = new ReportHelpClass();
            //用来存放已分配好的行政报表ReportTitle
            IDictionary<string, ReportTitle> pRptList = new Dictionary<string, ReportTitle>();
            ArrayList slPageNoList = new ArrayList();
            bool exist_SongLiao = false;

            foreach (var prti in prvRiverTypeInfo.DRiverRPType)
            {
                ReportTitle riverRpt = rptHelp.CloneEF<ReportTitle>(xzRpt);

                riverRpt.PageNO = rptHelp.FindMaxPageNO(2);
                riverRpt.AssociatedPageNO = xzRpt.PageNO;
                riverRpt.LastUpdateTime = DateTime.Now;
                riverRpt.ReceiveTime = DateTime.Now;
                riverRpt.RPTType_Code = prti.Value;//上报类型
                pRptList.Add(prti.Key, riverRpt);

                if (prti.Value == "AA2" || prti.Value == "BB2")//记录松花江、辽河流域表的页号
                {
                    slPageNoList.Add(riverRpt.PageNO);
                }
            }

            foreach (var rpt in pRptList)
            {
                #region HL011-HL014数据按流域分配
                if (xzRpt.HL011.Count > 0)
                {
                    foreach (var hl011 in xzRpt.HL011)
                    {
                        HL011 newHl011 = new HL011();
                        RiverInfo pRiverInfo = GetRiverInfo(hl011.UnitCode, rInfos);
                        if ((hl011.UnitCode != "15180000" && hl011.UnitCode != "15190000") && pRiverInfo.DRiverRate.Keys.Contains(rpt.Key))//排除锡林郭勒盟、锡林郭勒盟
                        {
                            List<string> hl011fieldlist = null;
                            dicIntField.TryGetValue("HL011", out hl011fieldlist);
                            newHl011 = ObjectMultimun<HL011>(hl011, pRiverInfo.DRiverRate[rpt.Key], hl011fieldlist);
                            newHl011.PageNO = rpt.Value.PageNO;
                            rpt.Value.HL011.Add(newHl011);
                        }

                        if (hl011.UnitCode == "15180000" || hl011.UnitCode == "15190000")//锡林郭勒盟、锡林郭勒盟需要查询到县进行分配
                        {
                            if (aggs.Count() > 0)//有汇总的数据，才按照县级的分流域
                            {
                                var tb07s = fxdict.TB07_District.Where(t => t.pDistrictCode == hl011.UnitCode && t.RD_RiverCode1 == rpt.Key).ToList();//有流域代码的
                                if (tb07s.Count() > 0 && rpt.Key == tb07s.FirstOrDefault().RD_RiverCode1)
                                {
                                    string codes = "";
                                    string pageNOs = "";
                                    var aggAcc = aggs.Select(t => t.SPageNO).ToList();
                                    foreach (var tb07 in tb07s)
                                    {
                                        codes += tb07.DistrictCode + ",";
                                    }
                                    codes = codes.Remove(codes.Length - 1);

                                    var ctyHl011s =
                                        ctyBusEntity.HL011.Where(
                                            t => codes.Contains(t.UnitCode) && aggAcc.Contains(t.PageNO)).ToList();
                                    foreach (var ctyHl011 in ctyHl011s)
                                    {
                                        newHl011 = ObjectAdd(newHl011, ctyHl011);
                                    }
                                    newHl011.PageNO = rpt.Value.PageNO;
                                    newHl011.UnitCode = hl011.UnitCode;
                                    newHl011.DW = hl011.DW;
                                    newHl011.DataOrder = hl011.DataOrder;
                                    rpt.Value.HL011.Add(newHl011);
                                }
                            }
                            else if (pRiverInfo.DRiverRate.ContainsKey(rpt.Key))
                            {
                                
                                List<string> hl011fieldlist = null;
                                dicIntField.TryGetValue("HL011", out hl011fieldlist);
                                newHl011 = ObjectMultimun<HL011>(hl011, pRiverInfo.DRiverRate[rpt.Key], hl011fieldlist);
                                newHl011.PageNO = rpt.Value.PageNO;
                                rpt.Value.HL011.Add(newHl011);
                            }
                        }
                    }
                }
                else
                {
                    rpt.Value.HL011 = null;
                    rpt.Value.HL011.Clear();
                }

                if (xzRpt.HL012.Count > 0)
                {
                    foreach (var hl012 in xzRpt.HL012)
                    {
                        HL012 newHl012 = new HL012();
                        RiverInfo pRiverInfo = GetRiverInfo(hl012.UnitCode, rInfos);
                        //if (pRiverInfo.DRiverRate.Keys.Contains(rpt.Key))
                        if (hl012.RiverCode == rpt.Key)
                        {
                            newHl012 = rptHelp.CloneEF<HL012>(hl012);
                            newHl012.PageNO = rpt.Value.PageNO;
                            rpt.Value.HL012.Add(newHl012);
                        }
                    }
                }
                else
                {
                    rpt.Value.HL012 = null;
                    rpt.Value.HL012.Clear();
                }

                if (xzRpt.HL013.Count > 0)
                {
                    foreach (var hl013 in xzRpt.HL013)
                    {
                        HL013 newHl013 = new HL013();
                        //RiverInfo pRiverInfo = GetRiverInfo(hl013.UnitCode, rInfos);//合计行没有传入UnitCode
                        //if (pRiverInfo.DRiverRate.Keys.Contains(rpt.Key))
                        if (hl013.RiverCode == rpt.Key || hl013.DW == "合计")
                        {
                            newHl013 = rptHelp.CloneEF<HL013>(hl013);
                            newHl013.PageNO = rpt.Value.PageNO;
                            rpt.Value.HL013.Add(newHl013);
                        }
                    }
                }
                else
                {
                    rpt.Value.HL013 = null;
                    rpt.Value.HL013.Clear();
                }

                if (xzRpt.HL014.Count > 0)
                {
                    foreach (var hl014 in xzRpt.HL014)
                    {
                        HL014 newHl014 = new HL014();
                        RiverInfo pRiverInfo = GetRiverInfo(hl014.UnitCode, rInfos);
                        if ((hl014.UnitCode != "15180000" && hl014.UnitCode != "15190000") && pRiverInfo.DRiverRate.Keys.Contains(rpt.Key))
                        {
                            List<string> hl014fieldlist = null;
                            dicIntField.TryGetValue("HL014", out hl014fieldlist);
                            newHl014 = ObjectMultimun<HL014>(hl014, pRiverInfo.DRiverRate[rpt.Key], hl014fieldlist);
                            newHl014.PageNO = rpt.Value.PageNO;
                            rpt.Value.HL014.Add(newHl014);
                        }

                        if (hl014.UnitCode == "15180000" || hl014.UnitCode == "15190000")//锡林郭勒盟、锡林郭勒盟需要查询到县进行分配
                        {
                            if (aggs.Count() > 0)//有汇总的数据，才按照县级的分流域
                            {
                                var tb07s = fxdict.TB07_District.Where(t => t.pDistrictCode == hl014.UnitCode && t.RD_RiverCode1 == rpt.Key).ToList();//有流域代码的
                                if (tb07s.Count() > 0 && rpt.Key == tb07s.FirstOrDefault().RD_RiverCode1)
                                {
                                    string codes = "";
                                    string pageNOs = "";
                                    var aggAcc = aggs.Select(t => t.SPageNO).ToList();
                                    foreach (var tb07 in tb07s)
                                    {
                                        codes += tb07.DistrictCode + ",";
                                    }
                                    codes = codes.Remove(codes.Length - 1);

                                    var ctyHl014s =
                                        ctyBusEntity.HL014.Where(
                                            t => codes.Contains(t.UnitCode) && aggAcc.Contains(t.PageNO)).ToList();
                                    foreach (var ctyHl011 in ctyHl014s)
                                    {
                                        newHl014 = ObjectAdd(newHl014, ctyHl011);
                                    }
                                    newHl014.PageNO = rpt.Value.PageNO;
                                    newHl014.UnitCode = hl014.UnitCode;
                                    newHl014.DW = hl014.DW;
                                    newHl014.DataOrder = hl014.DataOrder;
                                    rpt.Value.HL014.Add(newHl014);
                                }
                            }
                            else if (pRiverInfo.DRiverRate.ContainsKey(rpt.Key))
                            {
                                List<string> hl014fieldlist = null;
                                dicIntField.TryGetValue("HL014", out hl014fieldlist);
                                newHl014 = ObjectMultimun<HL014>(hl014, pRiverInfo.DRiverRate[rpt.Key], hl014fieldlist);
                                newHl014.PageNO = rpt.Value.PageNO;
                                rpt.Value.HL014.Add(newHl014);
                            }
                        }
                    }
                }
                else
                {
                    rpt.Value.HL014 = null;
                    rpt.Value.HL014.Clear();
                }
                #endregion

                #region//处理合计
                if (rpt.Value.HL011.Count < 2)
                {
                    //rpt.Value.HL011 = null;
                    rpt.Value.HL011.Clear();
                }
                else
                {
                    HL011 hl01hj = rpt.Value.HL011.Cast<HL011>().Where(x => x.DW == "合计").ToList<HL011>()[0];
                    HL011 hl011temp = new HL011();
                    IList<HL011> hl011 = rpt.Value.HL011.Cast<HL011>().Where(x => x.DW != "合计").ToList<HL011>();
                    foreach (var hl01 in hl011)
                    {
                        TObjectSum<HL011>(hl011temp, hl01);

                    }
                    UnionObject<HL011>(hl01hj, hl011temp);


                }
                if (rpt.Value.HL013.Count < 2)
                {
                    //rpt.Value.HL013 = null;
                    rpt.Value.HL013.Clear();
                }
                else
                {
                    HL013 hl03hj = rpt.Value.HL013.Cast<HL013>().Where(x => x.DW == "合计").ToList<HL013>()[0];
                    HL013 hl013temp = new HL013();
                    IList<HL013> hl013 = rpt.Value.HL013.Cast<HL013>().Where(x => x.DW != "合计").ToList<HL013>();
                    foreach (var hl03 in hl013)
                    {
                        TObjectSum<HL013>(hl013temp, hl03);

                    }
                    UnionObject<HL013>(hl03hj, hl013temp);

                    ///处理最大历时和最大水深
                    var maxZYZJZDSS = hl013.Max(t => t.ZYZJZDSS);//水深
                    var maxGCYMLS = hl013.Max(t => t.GCYMLS);//历时
                    hl03hj.GCYMLS = maxGCYMLS;
                    hl03hj.ZYZJZDSS = maxZYZJZDSS;
                    hl03hj.YMFWBL = hl013.Average(x => x.YMFWBL); 
                }

                if (rpt.Value.HL014.Count < 2)
                {
                    //rpt.Value.HL014 = null;
                    rpt.Value.HL014.Clear();
                }
                else
                {
                    HL014 hl04hj = rpt.Value.HL014.Cast<HL014>().Where(x => x.DW == "合计").ToList<HL014>()[0];
                    HL014 hl014temp = new HL014();
                    IList<HL014> hl014 = rpt.Value.HL014.Cast<HL014>().Where(x => x.DW != "合计").ToList<HL014>();
                    foreach (var hl04 in hl014)
                    {
                        TObjectSum<HL014>(hl014temp, hl04);

                    }
                    UnionObject<HL014>(hl04hj, hl014temp);
                }
                #endregion

                #region //处理死记人口和受淹城市  2.0添加了失踪人口
                foreach (var hl01 in rpt.Value.HL011)
                {
                    HL011 hl011temp = hl01 as HL011;
                    if (rpt.Value.HL012.Count > 0)
                    {
                        hl011temp.SWRK = rpt.Value.HL012.Cast<HL012>().Count(x =>
                        {
                            if (hl011temp.DW == "合计")
                            {
                                return (x.RiverCode == rpt.Key) && (x.DataType == "死亡");
                            }
                            else
                            {
                                return (x.UnitCode == hl011temp.UnitCode) && (x.RiverCode == rpt.Key) && (x.DataType == "死亡");
                            }
                        });
                    }
                    else
                    {
                        hl011temp.SWRK = 0;
                    }
                    if (rpt.Value.HL012.Count > 0)//失踪人口
                    {
                        hl011temp.SZRKR = rpt.Value.HL012.Cast<HL012>().Count(x =>
                        {
                            if (hl011temp.DW == "合计")
                            {
                                return (x.RiverCode == rpt.Key) && (x.DataType == "失踪");
                            }
                            else
                            {
                                return (x.UnitCode == hl011temp.UnitCode) && (x.RiverCode == rpt.Key) && (x.DataType == "失踪");
                            }
                        });
                    }
                    else
                    {
                        hl011temp.SZRKR = 0;
                    }

                    if (rpt.Value.HL013.Count > 0)
                    {

                        hl011temp.SYCS = rpt.Value.HL013.Cast<HL013>().Count(x =>
                        {
                            if (hl011temp.DW == "合计")
                            {
                                return (x.RiverCode == rpt.Key) && (x.DW != "合计");
                            }
                            else
                            {
                                return (x.UnitCode == hl011temp.UnitCode) && (x.RiverCode == rpt.Key);
                            }
                        });
                    }
                    else
                    {
                        hl011temp.SYCS = 0;
                    }
                }
                #endregion

                if (rpt.Value.HL011.Count > 0 || rpt.Value.HL012.Count > 0 || rpt.Value.HL013.Count > 0 ||
                    rpt.Value.HL014.Count > 0)
                {
                    prvBusEntity.ReportTitle.AddObject(rpt.Value);

                    if (rpt.Value.RPTType_Code == "AA2" || rpt.Value.RPTType_Code == "BB2")
                    {
                        exist_SongLiao = true;
                    }
                }
            }


            try
            {
                //using (TransactionScope scope = new TransactionScope())
                //{
                prvBusEntity.SaveChanges();

                if (exist_SongLiao)  //添加松辽委报表
                {
                    SaveSLRiverRpt(slPageNoList, xzRpt, pageNO);
                    prvBusEntity.SaveChanges();
                }

                saveFlag = true;
                //}
            }
            catch (Exception ex)
            {
                saveFlag = false;
            }
            return saveFlag;
        }
        
        /// <summary>保存松辽表
        /// </summary>
        /// <param name="pageList"></param>
        /// <param name="maxPageNO"></param>
        /// <param name="rpt"></param>
        /// <param name="PageNO"></param>
        public void SaveSLRiverRpt(ArrayList pageList, ReportTitle rpt, int PageNO)
        {
            ReportHelpClass rptHelp = new ReportHelpClass();
            IList<int> list = new List<int>();
            var rptPageNOs =
                prvBusEntity.ReportTitle.Where(
                    t => t.AssociatedPageNO == PageNO && (t.RPTType_Code == "AA2" || t.RPTType_Code == "BB2")).ToList();
            string pageNOs = "";
            for (int i = 0; i < pageList.Count; i++)
            {
                pageNOs += pageList[i] + ",";
                list.Add((int)pageList[i]);
            }
            pageNOs = pageNOs.Remove(pageNOs.Length - 1);
            string[] objStrings = new[] { "HL011", "HL012", "HL013", "HL014" };
            string tableName = "";
            string sql = "";
            ReportTitle slRiverRpt = rptHelp.CloneEF<ReportTitle>(rpt);
            slRiverRpt.PageNO = rptHelp.FindMaxPageNO(2);
            slRiverRpt.AssociatedPageNO = rpt.PageNO;
            slRiverRpt.LastUpdateTime = DateTime.Now;
            slRiverRpt.ReceiveTime = DateTime.Now;
            slRiverRpt.RPTType_Code = "AB1";//上报类型 松辽流域片表
            #region SQL形式

            for (int i = 0; i < objStrings.Length; i++)
            {
                IList alist = new ArrayList();
                tableName = objStrings[i];
                switch (tableName)
                {
                    case "HL011":
                        sql = CreateSQL(pageNOs, tableName);
                        int m = 0;
                        alist = prvBusEntity.ExecuteStoreQuery<ReportHL011>(sql).ToList();
                        foreach (var obj in alist)
                        {
                            ReportHL011 addHl011 = (ReportHL011)obj;
                            HL011 newHl011 = new HL011();
                            newHl011 = SLObjectCopy(newHl011, addHl011);
                            newHl011.PageNO = slRiverRpt.PageNO;
                            newHl011.DataOrder = m;
                            m++;
                            slRiverRpt.HL011.Add(newHl011);
                        }
                        break;
                    case "HL012":
                        HL012 newHl012 = new HL012();
                        sql = CreateSQL(pageNOs, tableName);
                        alist = prvBusEntity.ExecuteStoreQuery<HL012>(sql).ToList();
                        foreach (var obj in alist)
                        {
                            HL012 addHl012 = (HL012)obj;
                            addHl012.PageNO = slRiverRpt.PageNO;
                            slRiverRpt.HL012.Add(addHl012);
                        }
                        break;
                    case "HL013":
                        /*sql = CreateSQL(pageNOs, tableName);
                        int m3 = 0;
                        alist = prvBusEntity.ExecuteStoreQuery<ReportHL013>(sql).ToList();  //sum(YMFBL)

                        if (alist.Count > 0)
                        {
                            ReportHL013 addHl013 = (ReportHL013)alist[0];
                            HL013 newHl013 = new HL013();
                            newHl013 = SLObjectCopy(newHl013, addHl013);
                            newHl013.PageNO = slRiverRpt.PageNO;
                            newHl013.DataOrder = m3;
                            m3++;
                            slRiverRpt.HL013.Add(newHl013);

                            List<HL013> hl013s =
                                prvBusEntity.HL013.Where(x => pageList.Contains(x.PageNO) && x.DW != "合计")
                                    .ToList<HL013>();

                            foreach (HL013 hl013 in hl013s)
                            {
                                newHl013 = new HL013();
                                newHl013 = SLObjectCopy(newHl013, hl013);
                                newHl013.PageNO = slRiverRpt.PageNO;
                                newHl013.DataOrder = m3;
                                m3++;
                                slRiverRpt.HL013.Add(newHl013);
                            }
                        }*/

                        var hl013s =
                            prvBusEntity.ExecuteStoreQuery<HL013>("select * from hl013 where pageno in(" + pageNOs + ")").ToList();
                        if (hl013s.Count > 0)
                        {
                            HL013 hl03hj = hl013s.Where(x => x.DW == "合计").ToList()[0];
                            HL013 hl013temp = new HL013();
                            IList<HL013> hl013 = hl013s.Where(x => x.DW != "合计").ToList();
                            foreach (var hl03 in hl013)
                            {
                                TObjectSum<HL013>(hl013temp, hl03);
                            }
                            UnionObject<HL013>(hl03hj, hl013temp);

                            ///处理最大历时和最大水深
                            var maxZYZJZDSS = hl013.Max(t => t.ZYZJZDSS); //水深
                            var maxGCYMLS = hl013.Max(t => t.GCYMLS); //历时
                            hl03hj.GCYMLS = maxGCYMLS;
                            hl03hj.ZYZJZDSS = maxZYZJZDSS;
                            hl03hj.YMFWBL = hl013.Average(x => x.YMFWBL);

                            int m3 = 0;
                            hl03hj.PageNO = slRiverRpt.PageNO;
                            hl03hj.DataOrder = m3;
                            m3++;
                            slRiverRpt.HL013.Add(hl03hj);

                            foreach (var hl03 in hl013)
                            {
                                hl03.PageNO = slRiverRpt.PageNO;
                                hl03.DataOrder = m3;
                                m3++;
                                slRiverRpt.HL013.Add(hl03);
                            }
                        }

                        break;
                    default:
                        sql = CreateSQL(pageNOs, tableName);
                        alist = prvBusEntity.ExecuteStoreQuery<ReportHL014>(sql).ToList();
                        int m4 = 0;
                        foreach (var obj in alist)
                        {
                            ReportHL014 addHl014 = (ReportHL014)obj;
                            HL014 newHl014 = new HL014();
                            newHl014 = SLObjectCopy(newHl014, addHl014);
                            newHl014.PageNO = slRiverRpt.PageNO;
                            newHl014.DataOrder = m4;
                            m4++;
                            slRiverRpt.HL014.Add(newHl014);
                        }
                        break;
                }
            }

            #endregion
            prvBusEntity.ReportTitle.AddObject(slRiverRpt);
        }

        public string CreateSQL(string pageNOs, string tabName)
        {
            string sql = "";
            if (tabName == "HL012")
            {
                sql = "select * from hl012 where pageno in (" + pageNOs + ")";
            }
            //else if (tabName == "HL013")
            //{
            //    sql = "select * from hl013 where pageno in (" + pageNOs + ")";
            //}
            else
            {
                var tb55s =
                  (from tb55 in fxdict.TB55_FieldDefine
                   where tb55.UnitCls == 2 && tb55.TD_TabCode == tabName
                   select new
                   {
                       tb55.FieldCode,
                       tb55.DecimalCount
                   }).ToList();
                StringBuilder ssql = new StringBuilder();
                string groupStr = "";
                sql = "select UnitCode,DW,sum(tbno) as TBNO,";
                for (int i = 0; i < tb55s.Count(); i++)
                {

                    if ((tb55s[i].FieldCode.ToUpper() == "DistributeRate".ToUpper()) ||
                        (tb55s[i].FieldCode.ToUpper() == "id".ToUpper()) ||
                        (tb55s[i].FieldCode.ToUpper() == "tbno".ToUpper()) ||
                        (tb55s[i].FieldCode.ToUpper() == "PageNo".ToUpper()) ||
                        (tb55s[i].FieldCode.ToUpper() == "UnitCode".ToUpper()) ||
                        (tb55s[i].FieldCode.ToUpper() == "DW".ToUpper())
                        )
                    {
                        continue;
                    }

                    if (tb55s[i].FieldCode.ToString() == "GCYMLS" || tb55s[i].FieldCode.ToString() == "ZYZJZDSS")
                    {
                        ssql.Append("max(").Append(tb55s[i].FieldCode.ToString()).Append(")").Append(" as ").Append(tb55s[i].FieldCode.ToString()).Append(",");
                    }
                    else if (tabName == "HL013" && tb55s[i].FieldCode.ToString() == "YMFWBL")
                    {
                        ssql.Append("avg(").Append(tb55s[i].FieldCode.ToString()).Append(")").Append(" as ").Append(tb55s[i].FieldCode.ToString()).Append(",");
                    }
                    else if (tb55s[i].DecimalCount != -1)
                    {
                        ssql.Append("sum(")
                            .Append(tb55s[i].FieldCode.ToString())
                            .Append(")")
                            .Append(" as ")
                            .Append(tb55s[i].FieldCode.ToString())
                            .Append(",");
                    }
                    else
                    {
                        ssql.Append(tb55s[i].FieldCode.ToString()).Append(",");
                        groupStr += tb55s[i].FieldCode.ToString() + ",";
                    }
                }
                ssql.Remove(ssql.Length - 1, 1);
                sql = sql + ssql.ToString() + " from " + tabName + " where pageno in (" + pageNOs +
                      ") group by UnitCode,DW  ";
                if (groupStr != "")
                {
                    sql = sql + "," + groupStr.Remove(groupStr.Length - 1);
                }
                sql = sql + " order by UnitCode";
            }
            return sql;
        }

        /// <summary>
        /// 获取表（HL011等）字段信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetIntRiverField()
        {
            var tb55s = from tb55 in fxdict.TB55_FieldDefine
                        where tb55.DecimalCount == 0 &&
                        tb55.UnitCls == 2 &&
                        tb55.TD_TabCode != "HL013"
                        select new
                        {
                            tb55.TD_TabCode,
                            tb55.FieldCode
                        };
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var obj in tb55s.ToList())
            {
                if (!result.ContainsKey(obj.TD_TabCode))
                {
                    result.Add(obj.TD_TabCode, new List<string>());
                }
                List<string> dicItemList = (List<string>)result[obj.TD_TabCode];
                dicItemList.Add(obj.FieldCode);
            }
            return result;
        }


        /// <summary>
        ///  将一个对象内数字型数据型值乘一个系数，（PAGENO。TBNO等特殊数据不会处理）
        /// </summary>
        /// <typeparam name="T">类型HL011等</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="num">小于等于1的系数</param>
        /// <param name="intfieldlist">表名和字段代码</param>
        /// <returns></returns>
        public T ObjectMultimun<T>(T obj, double num, List<string> intfieldlist) where T : new()
        {
            List<string> systemtype = new List<string>();
            ReportHelpClass rptHelp = new ReportHelpClass();
            systemtype.Add("System.Int32");
            systemtype.Add("System.Int64");
            systemtype.Add("System.Double");
            systemtype.Add("System.Decimal");
            //object temp = obj.Clone();//不能T : ICloneable, new() 使用Clone方法
            object temp = rptHelp.CloneEF<T>(obj);//克隆对象
            PropertyInfo[] pifs = obj.GetType().GetProperties();
            for (int i = 0; i < pifs.Length; i++)
            {
                object tempobj = pifs[i].GetValue(temp, null);
                if (tempobj != null)
                {
                    if ((pifs[i].Name.ToUpper() == "DataOrder".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "SourcePageNo".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "id".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "tbno".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "PageNo".ToUpper()))
                    {
                        continue;
                    }
                    //if (systemtype.Contains(pifs[i].PropertyType.FullName))//有可能为nullable的
                    //{
                    object oovalue = null;
                    if (pifs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                    {
                        oovalue = Convert.ToInt32(tempobj) * num;
                        pifs[i].SetValue(temp, oovalue, null);
                    }
                    else if (pifs[i].PropertyType.FullName.IndexOf("System.Int64") != -1)
                    {
                        oovalue = Convert.ToInt64(tempobj) * num;
                        pifs[i].SetValue(temp, oovalue, null);
                    }
                    else if (pifs[i].PropertyType.FullName.IndexOf("System.Double") != -1)
                    {
                        oovalue = Convert.ToDouble(tempobj) * num;
                        pifs[i].SetValue(temp, oovalue, null);
                    }
                    else if (pifs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                    {
                        oovalue = Convert.ToDecimal(num) * Convert.ToDecimal(tempobj);
                        pifs[i].SetValue(temp, oovalue, null);
                    }
                    if (intfieldlist != null)
                    {
                        if (intfieldlist.Contains(pifs[1].Name.ToUpper()))
                        {
                            if (pifs[i].PropertyType.FullName.IndexOf("System.Double") != -1)
                            {
                                oovalue = Math.Round(Convert.ToDouble(oovalue), 0);
                            }
                            else if (pifs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                            {
                                oovalue = Math.Round(Convert.ToDecimal(oovalue));
                            }
                            pifs[i].SetValue(temp, oovalue, null);
                        }
                    }
                    //}
                }
            }
            return (T)temp;
        }

        public T ObjectAdd<T>(T baseObj, T obj) where T : new()
        {
            List<string> systemtype = new List<string>();
            ReportHelpClass rptHelp = new ReportHelpClass();
            systemtype.Add("System.Int32");
            systemtype.Add("System.Int64");
            systemtype.Add("System.Double");
            systemtype.Add("System.Decimal");

            PropertyInfo[] basePifs = baseObj.GetType().GetProperties();
            PropertyInfo[] pifs = obj.GetType().GetProperties();

            for (int i = 0; i < basePifs.Length; i++)
            {
                object baseTempobj = basePifs[i].GetValue(baseObj, null);
                for (int j = 0; j < pifs.Length; j++)
                {
                    if (basePifs[i].Name.ToUpper() == pifs[j].Name.ToUpper())
                    {
                        object tempobj = basePifs[i].GetValue(obj, null);
                        if ((basePifs[i].Name.ToUpper() == "DataOrder".ToUpper()) ||
                        (basePifs[i].Name.ToUpper() == "SourcePageNo".ToUpper()) ||
                        (basePifs[i].Name.ToUpper() == "id".ToUpper()) ||
                        (basePifs[i].Name.ToUpper() == "tbno".ToUpper()) ||
                        (basePifs[i].Name.ToUpper() == "PageNo".ToUpper()))
                        {
                            break;
                        }
                        object oovalue = null;
                        if (pifs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                        {
                            oovalue = Convert.ToInt32(baseTempobj) + Convert.ToInt32(tempobj);
                            pifs[i].SetValue(baseObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Int64") != -1)
                        {
                            oovalue = Convert.ToInt64(baseTempobj) + Convert.ToInt64(tempobj);
                            pifs[i].SetValue(baseObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Double") != -1)
                        {
                            oovalue = Convert.ToDouble(baseTempobj) + Convert.ToDouble(tempobj);
                            pifs[i].SetValue(baseObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                        {
                            oovalue = Convert.ToDecimal(baseTempobj) + Convert.ToDecimal(tempobj);
                            pifs[i].SetValue(baseObj, oovalue, null);
                        }
                        break;
                    }
                }
            }
            return (T)baseObj;
        }

        public T SLObjectAdd<T>(T newObj, T baseObj) where T : new()
        {
            List<string> systemtype = new List<string>();
            ReportHelpClass rptHelp = new ReportHelpClass();

            PropertyInfo[] basePifs = newObj.GetType().GetProperties();
            PropertyInfo[] pifs = baseObj.GetType().GetProperties();

            for (int i = 0; i < basePifs.Length; i++)
            {
                object baseTempobj = basePifs[i].GetValue(newObj, null);
                for (int j = 0; j < pifs.Length; j++)
                {
                    if (pifs[i].Name.ToUpper() == "tbno".ToUpper())
                    {
                        break;
                    }
                    if (basePifs[i].Name.ToUpper() == pifs[j].Name.ToUpper())
                    {
                        object tempobj = pifs[i].GetValue(baseObj, null);
                        object oovalue = null;
                        if (pifs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                        {
                            oovalue = Convert.ToInt32(baseTempobj) + Convert.ToInt32(tempobj);
                            pifs[i].SetValue(baseObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Int64") != -1)
                        {
                            oovalue = Convert.ToInt64(baseTempobj) + Convert.ToInt64(tempobj);
                            pifs[i].SetValue(baseObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Double") != -1)
                        {
                            oovalue = Convert.ToDouble(baseTempobj) + Convert.ToDouble(tempobj);
                            pifs[i].SetValue(baseObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                        {
                            oovalue = Convert.ToDecimal(baseTempobj) + Convert.ToDecimal(tempobj);
                            pifs[i].SetValue(baseObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.String") != -1)
                        {
                            oovalue = Convert.ToString(tempobj);
                            pifs[i].SetValue(newObj, oovalue, null);
                        }
                        break;
                    }
                }
            }
            return (T)newObj;
        }
        public T SLObjectCopy<T, T1>(T newObj, T1 baseObj) where T : new()
        {
            PropertyInfo[] pifs = newObj.GetType().GetProperties();
            PropertyInfo[] basePifs = baseObj.GetType().GetProperties();

            for (int i = 0; i < pifs.Length; i++)
            {
                for (int j = 0; j < basePifs.Length; j++)
                {
                    if (pifs[i].Name.ToUpper() == "tbno".ToUpper())
                    {
                        break;
                    }
                    if (pifs[i].Name.ToUpper() == basePifs[j].Name.ToUpper())
                    {
                        object tempobj = basePifs[j].GetValue(baseObj, null);
                        object oovalue = null;
                        if (pifs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                        {
                            oovalue = Convert.ToInt32(tempobj);
                            pifs[i].SetValue(newObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Int64") != -1)
                        {
                            oovalue = Convert.ToInt64(tempobj);
                            pifs[i].SetValue(newObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Double") != -1)
                        {
                            oovalue = Convert.ToDouble(tempobj);
                            pifs[i].SetValue(newObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                        {
                            oovalue = Convert.ToDecimal(tempobj);
                            pifs[i].SetValue(newObj, oovalue, null);
                        }
                        else if (pifs[i].PropertyType.FullName.IndexOf("System.String") != -1)
                        {
                            oovalue = Convert.ToString(tempobj);
                            pifs[i].SetValue(newObj, oovalue, null);
                        }
                        break;
                    }
                }
            }
            return (T)newObj;
        }

        /// <summary>
        /// 把obj1与obj2的值进行相加赋值给obj1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj1">对象1</param>
        /// <param name="obj2">对象2</param>
        public static void TObjectSum<T>(T obj1, T obj2)
             where T : new()
        {
            object temp = obj1;
            PropertyInfo[] pifs = obj1.GetType().GetProperties();
            for (int i = 0; i < pifs.Length; i++)
            {
                object tempobj1 = pifs[i].GetValue(obj1, null);
                object tempobj2 = pifs[i].GetValue(obj2, null);
                //if ((tempobj1 != null) && (tempobj2 != null))
                if (tempobj2 != null)
                {
                    if ((pifs[i].Name.ToUpper() == "DataOrder".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "SourcePageNo".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "id".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "tbno".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "PageNo".ToUpper()))
                    {
                        continue;
                    }
                    if (pifs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                    {
                        pifs[i].SetValue(temp, Convert.ToInt32(tempobj1) + Convert.ToInt32(tempobj2), null);
                    }
                    else if (pifs[i].PropertyType.FullName.IndexOf("System.Int64") != -1)
                    {
                        pifs[i].SetValue(temp, Convert.ToInt64(tempobj1) + Convert.ToInt64(tempobj2), null);
                    }
                    else if (pifs[i].PropertyType.FullName.IndexOf("System.Double") != -1)
                    {
                        pifs[i].SetValue(temp, Convert.ToDouble(tempobj1) + Convert.ToDouble(tempobj2), null);
                    }
                    else if (pifs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                    {
                        pifs[i].SetValue(temp, Convert.ToDecimal(tempobj1) + Convert.ToDecimal(tempobj2), null);

                    }
                }
            }
        }


        /// <summary>
        /// 把obj2的中特殊类型的数据（int32，Decimal等类型）赋值给obj
        /// </summary>
        /// <typeparam name="T">HL011等类型</typeparam>
        /// <param name="obj1">被赋值对象</param>
        /// <param name="obj2">转换好的数据</param>
        public static void UnionObject<T>(T obj1, T obj2)
           where T : new()
        {
            object temp = obj1;
            PropertyInfo[] pifs = obj1.GetType().GetProperties();
            for (int i = 0; i < pifs.Length; i++)
            {
                object tempobj1 = pifs[i].GetValue(obj1, null);
                object tempobj2 = pifs[i].GetValue(obj2, null);
                if (tempobj2 != null || tempobj1 != null)
                {
                    if ((pifs[i].Name.ToUpper() == "DataOrder".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "SourcePageNo".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "id".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "tbno".ToUpper()) ||
                        (pifs[i].Name.ToUpper() == "PageNo".ToUpper()))
                    {
                        continue;
                    }

                    if ((pifs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                       || (pifs[i].PropertyType.FullName.IndexOf("System.Int64") != -1)
                       || (pifs[i].PropertyType.FullName.IndexOf("System.Double") != -1)
                       || (pifs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1))
                    {
                        pifs[i].SetValue(temp, tempobj2, null);

                    }
                }
            }
        }

        /// <summary>根据当前登录单位，获取所有下级单位的多流域数据（流域数量大于等于2的）
        /// </summary>
        /// <param name="unitCode">当前登录单位</param>
        /// <returns></returns>
        public string GetUnderUnitRiverDataByCode(string unitCode)
        {
            var tb10s = from district in fxdict.TB07_District
                        from riverDistribute in district.TB10_RiverDistribute
                        where district.pDistrictCode == unitCode
                        select riverDistribute;
            string result = "";
            foreach (var tb10 in tb10s)
            {
                result += "{UnitCode:'" + tb10.D_DistrictCode + "',RiverName:'" + tb10.TB09_RiverDict.RiverName + "',RiverCode:'" + tb10.RD_RiverCode + "',RDRate:'" + string.Format("{0:N2}", tb10.RDRate) + "'},";
            }
            if (result != "")
            {
                result = "[" + result.Remove(result.Length - 1) + "]";
            }
            return result;
        }

        /// <summary>根据页号，取报表对应的流域PageNO
        /// </summary>
        /// <param name="pageNO"></param>
        /// <returns></returns>
        public string GetRiverPageNOByPageNO(int pageNO)
        {
            var rpts = from rpt in prvBusEntity.ReportTitle
                       where rpt.AssociatedPageNO == pageNO && rpt.Del != 1
                       select new
                       {
                           rpt.PageNO,
                           rpt.RPTType_Code
                       };
            Dictionary<string, string> riverDic = new Dictionary<string, string>();
            var tb11s = fxdict.TB11_RptType.ToList();
            foreach (var tb11 in tb11s)
            {
                riverDic.Add(tb11.RptTypeCode, "RiverCode:'" + tb11.RvCode + "',RiverName:'" + tb11.RptTypeName + "'");
            }
            string result = "";
            foreach (var rpt in rpts)
            {
                result += "{" + riverDic[rpt.RPTType_Code] + ",PageNO:'" + rpt.PageNO + "'},";
            }
            if (result != "")
            {
                result = result.Remove(result.Length - 1);
            }
            return result;
        }
    }
}
