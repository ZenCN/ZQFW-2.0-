using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using EntityModel;
using DBHelper;
using System.Collections;
using System.Reflection;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：ReportHelpClass.cs
// 文件功能描述：对报表的新建、修改等操作提供一些辅助方法。比如找出某时间段里类型相同的报表
// 创建标识：
// 修改标识：// 修改描述：
//-------------------------------------------------------------*/
namespace LogicProcessingClass.ReportOperate
{
    public class ReportHelpClass
    {
        /// <summary>
        /// 找出相同时段的报表
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <param name="ordCode">报表名称(HL01)洪涝01</param>
        /// <param name="sCycType">报表类型</param>
        /// <param name="startDateTime">开始时间</param>
        /// <param name="endDateTime">结束时间</param>
        /// <param name="sourceType">来源类型</param>
        /// <param name="unitCode">单位代码</param>
        /// <returns>jsonStr，找出的相同报表信息</returns>
        public string FindSameReport(int limit, string ordCode, int sCycType, string startDateTime, string endDateTime, int sourceType, string unitCode)
        {
            if (ordCode=="NP01" && unitCode.StartsWith("15"))
            {
                limit = 2;
                unitCode = "15000000";
            }
            string jsonStr = "{reportList:[";
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            DateTime sDateTime = Convert.ToDateTime(startDateTime);
            DateTime eDateTime = Convert.ToDateTime(endDateTime);
            var sameRpts = from rpt in busEntity.ReportTitle
                           where rpt.ORD_Code == ordCode &&
                           rpt.StatisticalCycType == sCycType &&
                           rpt.RPTType_Code == "XZ0" &&
                           rpt.StartDateTime == sDateTime &&
                           rpt.EndDateTime == eDateTime &&
                           rpt.SourceType == sourceType &&
                           rpt.UnitCode == unitCode &&
                           rpt.Del == 0 &&
                           (rpt.State == 0 || rpt.State == 3) &&
                           rpt.CopyPageNO == 0
                           select new
                           {
                               rpt.PageNO,
                               rpt.StartDateTime,
                               rpt.EndDateTime,
                               rpt.WriterTime,
                               rpt.SourceType,
                               rpt.Remark
                           };
            
            foreach (var sameRpt in sameRpts)
            {
                jsonStr += "{PageNO:'" + sameRpt.PageNO + "',StartDateTime:'" + Convert.ToDateTime(sameRpt.StartDateTime).ToString("yyyy-MM-dd") + "',EndDateTime:'" + Convert.ToDateTime(sameRpt.EndDateTime).ToString("yyyy-MM-dd")
                    + "',WriterTime:'" + Convert.ToDateTime(sameRpt.WriterTime).ToString("yyyy-MM-dd HH:mm:ss ") + "',SourceType:'" + sameRpt.SourceType + "',Remark:'" + sameRpt.Remark + "'},";
            }
            if (sameRpts.Count() > 0)
            {
                jsonStr = jsonStr.Remove(jsonStr.Length - 1);
            }

            busEntity.Dispose();

            jsonStr = jsonStr + "]}";

            return jsonStr;
        }

        /// <summary>
        /// 找出最大的页号pageNO,如果没有数据则为0
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <returns>最大pageNO页号</returns>
        //public int FindMaxPageNO(int limit)
        //{
        //    BusinessEntities busEntity = Persistence.GetDbEntities(limit);
        //    var rpts = from rpt in busEntity.ReportTitle
        //               orderby rpt.PageNO descending
        //               select rpt.PageNO;
        //    //var rpts = busEntity.ReportTitle.Max(t => t.PageNO);
        //    int maxPageNO = 0;
        //    if (rpts.Count() != 0)
        //    {
        //        maxPageNO = Convert.ToInt32(rpts.First().ToString());
        //    }
        //    return maxPageNO;
        //}

        public int FindMaxPageNO(int limit)
        {
            //----------------刘志-------------------------
            //BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            //var rpts = from rpt in busEntity.ReportTitle
            //           orderby rpt.PageNO descending
            //           select rpt.PageNO;
            ////var rpts = busEntity.ReportTitle.Max(t => t.PageNO);
            //int maxPageNO = 0;
            //if (rpts.Count() != 0)
            //{
            //    maxPageNO = Convert.ToInt32(rpts.First().ToString());
            //}
            //return maxPageNO;

            //---------------张建军------------------------------
            HttpApplicationState App = System.Web.HttpContext.Current.Application;

            int maxPageNO = 0;

            if (App[limit + "_maxPageNO"] == null) //与该application相关的都是并发修改,重启iis时
            {
                BusinessEntities busEntity = Persistence.GetDbEntities(limit);
                if (busEntity.ReportTitle.Any())
                {
                    maxPageNO = busEntity.ReportTitle.Max(t => t.PageNO);
                }
                busEntity.Dispose();

                maxPageNO = maxPageNO + 1;
            }
            else
            {
                maxPageNO = Convert.ToInt32(App[limit + "_maxPageNO"]);
            }

            App[limit + "_maxPageNO"] = maxPageNO + 1;

            return maxPageNO;

            

            /*HttpApplicationState app = System.Web.HttpContext.Current.Application;

            if (app["rng"] == null)
            {
                System.Security.Cryptography.RNGCryptoServiceProvider _rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
                app["rng"] = _rng;
            }

            System.Security.Cryptography.RNGCryptoServiceProvider rng = (System.Security.Cryptography.RNGCryptoServiceProvider)app["rng"];

            byte[] bytes = new byte[4];

            rng.GetBytes(bytes);

            int pageno = BitConverter.ToInt32(bytes, 0);

            //BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            //while (busEntity.ReportTitle.Where(t => t.PageNO == pageno).Any())
            //{
                
            //}

            return pageno;*/
        }

        /// <summary>
        /// 泛型克隆方法，将h1作为参数传入，把值赋值给新对象后返回(PageNO没有进行Copy)
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="hl"></param>
        /// <returns></returns>
        public T CloneEF<T>(T hl)
            where T : new()
        {
            PropertyInfo[] pfs = hl.GetType().GetProperties();//利用反射获得类的属性
            T tempModel = new T();
            PropertyInfo[] fixmm = tempModel.GetType().GetProperties();
            string temp = "";
            for (int j = 0; j < fixmm.Length; j++)
            {
                //找出各个属性的名字
                for (int i = 0; i < pfs.Length; i++)
                {
                    if (fixmm[j].Name.ToUpper() == pfs[i].Name.ToUpper() && fixmm[j].Name.ToUpper() != "PAGENO")
                    {
                        if (pfs[i].PropertyType.FullName.IndexOf("System.String") != -1)
                        {
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = "";
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }
                            fixmm[j].SetValue(tempModel, temp, null);
                        }

                        else if (pfs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                        {
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = "0";
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }
                            fixmm[j].SetValue(tempModel, Convert.ToDecimal(temp), null);
                        }
                        else if (pfs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                        {
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = null;
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }

                            fixmm[j].SetValue(tempModel, Convert.ToInt32(temp), null);
                        }
                        else if (pfs[i].PropertyType.FullName.IndexOf("System.DateTime") != -1)
                        {
                            //temp = Convert.ToDateTime(pfs[i].GetValue(hl, null)).ToString("yyyy-MM-dd");
                            temp = pfs[i].GetValue(hl, null).ToString();
                            fixmm[j].SetValue(tempModel, Convert.ToDateTime(temp), null);
                        }
                    }
                }
            }
            return tempModel;
        }
        public T Clone<T>(T hl)
            where T:ICloneable, new()
        {
            T tempModel = new T();
            tempModel =(T) hl.Clone();
            return tempModel;
        }
    }
}
