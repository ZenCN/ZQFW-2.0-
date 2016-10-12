using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel;
using DBHelper;
using System.Globalization;
using LogicProcessingClass.AuxiliaryClass;

namespace LogicProcessingClass.ReportOperate
{
    public class TownReport
    {
        BusinessEntities townBusEntity = Persistence.GetDbEntities(5);//默认实例化乡镇对应的业务模型

        //public string SaveOrUpdateTownRpt(int pageNO,)
        //{ 

        //}



        /// <summary>
        /// 获取乡镇报表列表
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns>对象数组</returns>
        public string GetTownReportList(string unitCode, DateTime startDate, DateTime endDate)
        {
            string jsonStr = "";
            var rpts = from rpt in townBusEntity.ReportTitle
                       where rpt.UnitCode == unitCode &&
                       rpt.StartDateTime >= startDate &&
                       rpt.EndDateTime <= endDate &&
                       rpt.CopyPageNO == 0 &&
                       rpt.Del != 1
                       orderby rpt.EndDateTime descending
                       select new
                       {
                           rpt.PageNO,
                           rpt.StartDateTime,
                           rpt.EndDateTime,
                           rpt.State
                       };
            foreach (var obj in rpts)
            {
                jsonStr += "{PageNO:" + Convert.ToInt32(obj.PageNO) + ",StartDateTime:'" + Convert.ToDateTime(obj.StartDateTime).Date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "',EndDateTime:'"
                       + Convert.ToDateTime(obj.EndDateTime).Date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "',State:" + Convert.ToInt32(obj.State) + "},";
            }
            if (jsonStr != "")
            {
                jsonStr = "ReportList:[" + jsonStr.Remove(jsonStr.Length - 1) + "]";
            }
            else
            {
                jsonStr = "ReportList:[]";
            }
            return jsonStr;
        }

        /// <summary>
        /// 获取乡镇报表详细信息
        /// </summary>
        /// <param name="pageNO">报表的页号</param>
        /// <returns>Json格式</returns>
        public string GetRptInfo(int pageNO)
        {
            string jsonStr = "";
            //string sql = "select rt.StartDateTime,rt.EndDateTime,rt.Remark,h1.SHMJXJ,h1.SZRK,h1.SWRK,h1.ZYRK,h1.DTFW,h1.ZJJJZSS,h1.SLSSZJJJSS,h1.Id,h1.SZRKR" + " from ReportTitle as rt,HL011 as h1 where rt.Id=" + pageNO + " and rt.Id=h1.ReportTitle and rt.UnitCode='" + unitCode + "'";
            var townRpt = (from rpt in townBusEntity.ReportTitle
                           where rpt.PageNO == pageNO
                           select new
                           {
                               rpt.StartDateTime,
                               rpt.EndDateTime,
                               rpt.Remark,
                               //hl011.SZRK,
                               //hl011.SWRK,
                               //hl011.SHMJXJ,
                               //hl011.ZYRK,
                               //hl011.DTFW,
                               //hl011.ZJJJZSS,
                               //hl011.SLSSZJJJSS,
                               //hl011.TBNO,
                               //hl011.SZRKR
                           }).SingleOrDefault();

            var townHl011s = townBusEntity.HL011.Where(t => t.PageNO == pageNO);
            Dictionary<string, decimal?> dic = new Dictionary<string, decimal?>();
            if (townHl011s.Count() > 0)
            {

                dic.Add("SHMJXJ", townHl011s.SingleOrDefault().SHMJXJ);
                dic.Add("SZRK", townHl011s.SingleOrDefault().SZRK);
                dic.Add("SWRK", townHl011s.SingleOrDefault().SWRK);
                dic.Add("ZYRK", townHl011s.SingleOrDefault().ZYRK);
                dic.Add("DTFW", townHl011s.SingleOrDefault().DTFW);
                dic.Add("ZJJJZSS", townHl011s.SingleOrDefault().ZJJJZSS);
                dic.Add("SLSSZJJJSS", townHl011s.SingleOrDefault().SLSSZJJJSS);
                dic.Add("SZRKR", townHl011s.SingleOrDefault().SZRKR);
            }
            FXDICTEntities fxdict = Persistence.GetDbEntities();
            var tb55s = from fieldDefine in fxdict.TB55_FieldDefine
                        where fieldDefine.UnitCls == 5 && fieldDefine.TD_TabCode == "HL011"
                        select new
                        {
                            fieldDefine.FieldCode,
                            fieldDefine.MeasureValue,
                            fieldDefine.DecimalCount
                        };
            //string[] fields = new[] { "SHMJXJ", "SZRK", "SWRK", "ZYRK", "DTFW", "ZJJJZSS", "SLSSZJJJSS", "SZRKR" };
            string reportInfo = "";
            foreach (var obj in tb55s)
            {
                if (dic.Keys.Contains(obj.FieldCode.ToString()))
                {
                    reportInfo += obj.FieldCode.ToString() + ":'" + String.Format("{0:N" + obj.DecimalCount + "}", Convert.ToDouble(dic[obj.FieldCode.ToString()].Value / obj.MeasureValue)).Replace(",", "") + "',";
                }
            }
            reportInfo += "BeginDate:'" + Convert.ToDateTime(townRpt.StartDateTime).ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "',EndDate:'" + Convert.ToDateTime(townRpt.EndDateTime).ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "',Remark:'" + townRpt.Remark + "',Id:'" + pageNO + "'";
            jsonStr = "{reportInfo:{" + reportInfo + "}}";
            //查看数据库里是否存在附件
            var affs = from aff in townBusEntity.Affix
                       where aff.PageNO == pageNO
                       select new
                       {
                           aff.DownloadURL,
                           aff.FileName,
                           aff.TBNO
                       };
            string tmp = "";
            Tools tool = new Tools();
            foreach (var obj in affs)
            {
                tmp += "{url:'" + tool.EncryptOrDecrypt(0, obj.DownloadURL, "JXHLZQBS") + "',name:'" + obj.FileName + "',tbno:'" + obj.TBNO + "'},";
            }
            if (tmp.Length != 0)
            {
                jsonStr = "{reportInfo:{" + reportInfo + "},affix:[" + tmp.Remove(tmp.Length - 1) + "]}";
            }
            else
            {
                jsonStr = "{reportInfo:{" + reportInfo + "},affix:[]}";
            }
            return jsonStr;
        }

        /// <summary>
        /// 获取死亡人员信息表
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <returns></returns>
        public string GetDeathReasonForm(int pageNO)
        {
            string jsonStr = "";
            var hl012s = from hl012 in townBusEntity.HL012
                         where hl012.PageNO == pageNO
                         select new
                         {
                             hl012.TBNO,
                             hl012.SWXM,
                             hl012.SWXB,
                             hl012.SWNL,
                             hl012.SWDD,
                             hl012.SWSJ,
                             hl012.DeathReason,
                             hl012.DeathReasonCode
                         };
            string deathInfo = "";
            foreach (var obj in hl012s)
            {
                deathInfo += "{Id:'" + obj.TBNO + "',SWXM:'" + obj.SWXM + "',SWXB:'" + obj.SWXB + "',SWNL:'" + obj.SWNL + "',SWDD:'" + obj.SWDD + "',SWSJ:'" + Convert.ToDateTime(obj.SWSJ).Date.ToShortDateString() + "',DeathReason:'" + obj.DeathReason + "',DeathReasonCode:'" + obj.DeathReasonCode + "'},";
            }
            if (deathInfo != "")
            {
                deathInfo = deathInfo.Remove(deathInfo.Length - 1);
            }
            //jsonStr = "[" + deathInfo + "]";
            jsonStr = "{deathInfo:[" + deathInfo + "]}";
            return jsonStr;
        }

        /// <summary>
        /// 乡镇填表的提示等(为了简化各方法的调用而写的)
        /// </summary>
        /// <param name="limit">单位级别（只能是乡镇级别）</param>
        /// <param name="unitcode">单位代码</param>
        /// <returns>bool</returns>
        public string GetFieldDefineAndMeasureName(int limit, string unitcode)
        {
            FXDICTEntities fxdict = Persistence.GetDbEntities();
            var tb55sDefine = from tb55 in fxdict.TB55_FieldDefine
                              where tb55.UnitCls == limit
                              select new
                              {
                                  tb55.FieldCode,
                                  tb55.MeasureName,
                                  tb55.InputRemark
                              };
            string result = "{";
            foreach (var obj in tb55sDefine)
            {
                result += obj.FieldCode + ":{MeasureName:'" + obj.MeasureName + "',Title:'" + obj.InputRemark + "'},";
            }
            //IQuery query = iSession.CreateQuery("select FieldCode,InputRemark from TB55FieldDefine where FieldCode in('SHMJXJ','SZRK','SWRK','SZRKR','ZYRK','DTFW','ZJJJZSS','SLSSZJJJSS') and UnitCls=" + limit);
            //string result = "Field:" + new TransformJSON().JSON_ArrayToObject(tb55sDefine.ToList());
            /*            var tb55sUnit = from tb55u in fxdict.TB55_FieldDefine
                                          where tb55u.UnitCls == limit
                                          select new
                                          {
                                              tb55u.FieldCode,
                                              tb55.MeasureName
                                          };
                        //query = iSession.CreateQuery("select FieldCode,MeasureName from TB55FieldDefine where MeasureValue > 1 and UnitCls=" + limit);
                        result += ",FieldUnit:" + new TransformJSON().CreateJSON(tb55sUnit.ToList());*/

            if (result != "{" && unitcode != "")
            {
                result = "Field:" + result.Remove(result.Length - 1) + "";
            }
            return result;
        }

        /// <summary>
        /// 物理删除hl011与hl012中的数据(非标记到回收站)
        /// </summary>
        /// <param name="pageNO">ReportTitle表的页号</param>
        /// <returns></returns>
        public bool DeleteTownRpt(int pageNO)
        {
            var hl011s = townBusEntity.HL011.Where(t => t.PageNO == pageNO);
            var hl012s = townBusEntity.HL012.Where(t => t.PageNO == pageNO);
            var hl014s = townBusEntity.HL014.Where(t => t.PageNO == pageNO);
            foreach (var obj011 in hl011s)
            {
                townBusEntity.HL011.DeleteObject(obj011);
            }
            foreach (var obj012 in hl012s)
            {
                townBusEntity.HL012.DeleteObject(obj012);
            }
            foreach (var obj014 in hl014s)
            {
                townBusEntity.HL014.DeleteObject(obj014);
            }
            bool flag = false;
            try
            {
                townBusEntity.SaveChanges();
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 修改时，进行附件的删除
        /// </summary>
        /// <param name="state">报表的状态（是否已经报送）</param>
        /// <param name="aList">附件的List</param>
        /// <param name="pageNO">修改的报表页号</param>
        /// <param name="zhubiaoPageNO">已经报送的报表的页号</param>
        /// <returns>bool</returns>
        public bool DeleteAffixs(decimal state, IList<Affix> aList, int pageNO, int zhubiaoPageNO)
        {
            bool flag = false;
            LogicProcessingClass.Tools tool = new LogicProcessingClass.Tools();
            //将源表中附件copy到副本表中,要删除的附件不复制
            if (state == 3)
            {
                var affs = townBusEntity.Affix.Where(t => t.PageNO == zhubiaoPageNO);
                foreach (var obj in affs)
                {
                    Affix af = new Affix();
                    bool copy = true;
                    for (int j = 0; j < aList.Count; j++)
                    {
                        if (((Affix)aList[j]).TBNO == Convert.ToInt32(obj.TBNO))
                        {
                            copy = false;
                            break;
                        }
                    }
                    if (copy)
                    {
                        af.FileName = obj.FileName.ToString();
                        af.FileSize = Convert.ToDecimal(obj.FileSize);
                        af.DownloadURL = obj.DownloadURL.ToString();
                        af.PageNO = pageNO;
                        townBusEntity.Affix.AddObject(af);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else//没有报送的
            {
                //Affix 附件表 删除附件 插入数据
                for (int i = 0; i < aList.Count; i++)
                {
                    Affix af = aList[i];
                    Affix delAff = townBusEntity.Affix.Where(t => t.TBNO == af.TBNO).SingleOrDefault();
                    if (delAff!=null)
                    {
                        townBusEntity.Affix.DeleteObject(delAff);
                    }
                    string path = System.Web.HttpContext.Current.Server.MapPath(tool.EncryptOrDecrypt(1, af.DownloadURL, "JXHLZQBS").Replace("..", "~"));
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
            }
            try
            {
                townBusEntity.SaveChanges();
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 标记删除报表（放入回收站中）
        /// </summary>
        /// <param name="pageNO">需要删除的报表页号</param>
        /// <returns>成功：1，否则返回“错误消息：”</returns>
        public string DeleteReport(int pageNO)
        {
            string jsr = "";
            var rpt = townBusEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
            rpt.Del = 1;
            try
            {
                townBusEntity.SaveChanges();
                jsr = "1";
            }
            catch (Exception ex)
            {
                jsr = "错误消息：" + ex.Message;
            }
            return jsr;
        }
    }
}
