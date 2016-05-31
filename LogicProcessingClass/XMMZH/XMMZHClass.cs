using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.ReportAuxiliaryModel;
using EntityModel;
using System.Reflection;
using System.Web;
using LogicProcessingClass.AuxiliaryClass;

namespace LogicProcessingClass.XMMZH
{
    public class XMMZHClass
    {
        /// <summary>
        /// 转换ReportTitle
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public LZReportTitle ZHQTReportTitle(ReportTitle rt)
        {
            LZReportTitle xmmrt = new LZReportTitle();
            xmmrt.PageNO = rt.PageNO.ToString();
            xmmrt.ORD_Code = rt.ORD_Code;
            xmmrt.RPTType_Code = rt.RPTType_Code;
            xmmrt.StatisticalCycType = String.Format("{0:N0}", rt.StatisticalCycType).Replace(",", "");
            xmmrt.UnitName = rt.UnitName;
            xmmrt.UnitCode = rt.UnitCode;
            xmmrt.StartDateTime = rt.StartDateTime.Value.ToShortDateString().Replace("/","-");//不能用 ToString("yyyy-MM-dd")格式化日期
            xmmrt.EndDateTime = rt.EndDateTime.Value.ToShortDateString().Replace("/", "-");//
            xmmrt.UnitPrincipal = rt.UnitPrincipal;
            xmmrt.StatisticsPrincipal = rt.StatisticsPrincipal;
            xmmrt.WriterName = rt.WriterName;
            xmmrt.WriterTime = rt.WriterTime.Value.ToString().Replace("/", "-");//
            xmmrt.SendTime = rt.SendTime.ToString().Replace("/", "-");//
            xmmrt.Remark =rt.Remark==null?rt.Remark: rt.Remark.Replace("/\n","<br>");
            xmmrt.Del = rt.Del.ToString();
            xmmrt.State = String.Format("{0:N0}", rt.State);
            xmmrt.SourceType = String.Format("{0:N0}", rt.SourceType);
            xmmrt.ReceiveTime = rt.ReceiveTime.ToString().Replace("/", "-");//
            xmmrt.AssociatedPageNO = String.Format("{0:N0}", rt.AssociatedPageNO);
            xmmrt.OperateReportNO = rt.OperateReportNO;
            xmmrt.DisasterTypeName = rt.DisasterTypeName;
            xmmrt.DisasterDescribe = rt.DisasterDescribe;
            xmmrt.DisasterSummary = rt.DisasterSummary;
            xmmrt.ExceptPageNo = rt.ExceptPageNo;
            xmmrt.RBType = rt.RBType.ToString();
            xmmrt.LastUpdateTime = rt.LastUpdateTime.ToString().Replace("/", "-");//
            xmmrt.ReceiveState = rt.ReceiveState.ToString();
            xmmrt.CloudPageNO = rt.CloudPageNO.ToString();
            xmmrt.CopyPageNO = rt.CopyPageNO.ToString();
            xmmrt.SendOperType = rt.SendOperType.ToString();
            return xmmrt;
        }

        /// <summary>
        /// 泛型方法，将T2(HL011--HL014,HP011-HP012)作为参数传入，转换成T(XMMHL011--XMMHL014,XMMHP011-XMMHP012)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="hl"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public T ConvertHLToXMMHL<T, T2>(T2 hl, int limit)
            where T : new()
            where T2 : new()
        {
            PropertyInfo[] pfs = hl.GetType().GetProperties();//利用反射获得类的属性

            T xmmhl011 = new T();
            //MemberInfo[] fixmm = xmmhl011.GetType().GetMembers();
            PropertyInfo[] fixmm = xmmhl011.GetType().GetProperties();
            TableFieldBaseData tbBaseData = new TableFieldBaseData();
            string[] arr = { };
            string temp = "";
            decimal shuliangji = 0;
            double xiaoshu = 0;
            decimal changetemp = 0;
            decimal tempv = 0;
            ReportTitle rep = null;
            for (int j = 0; j < fixmm.Length; j++)
            {
                arr = tbBaseData.GetFieldUnitArr(fixmm[j].Name, limit);
                if (arr != null)
                {
                    shuliangji = Convert.ToDecimal(arr[0]);
                    xiaoshu = Convert.ToDouble(arr[1]);
                }
                //找出hl011的各个属性的名字
                for (int i = 0; i < pfs.Length; i++)
                {
                    if (fixmm[j].Name.ToUpper() == pfs[i].Name.ToUpper())
                    {
                        //if (i == 53 && j == 53)
                        //{
                        //    string s = "";
                        //}
                        if (pfs[i].PropertyType.FullName == "System.String")
                        {
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = "";
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }
                            fixmm[j].SetValue(xmmhl011, temp, null);
                        }

                        else if (pfs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                        {
                            //temp = pfs[i].GetValue(hl, null).ToString();
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = "0";
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }
                            changetemp = Convert.ToDecimal(temp);
                            if (changetemp != 0)
                            {//TXZYBFB
                                if (fixmm[j].Name.ToUpper() == "TXZYBFB")//山平塘实际占计划
                                {
                                    xiaoshu = 2;
                                    shuliangji = 1;
                                }
                                if (shuliangji == 0)
                                {
                                    temp = String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp).Replace(",", "");
                                }
                                else
                                {
                                    temp = String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp / shuliangji).Replace(",", "");
                                }
                            }
                            else
                            {
                                temp = "";
                            }
                            fixmm[j].SetValue(xmmhl011, temp, null);
                        }
                        //else if (pfs[i].PropertyType.FullName == "System.Int32") .IndexOf( "System.Int32") != -1)
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
                            //temp = pfs[i].GetValue(hl, null).ToString();
                            fixmm[j].SetValue(xmmhl011, temp, null);
                        }
                        else if (pfs[i].PropertyType.FullName.IndexOf("System.DateTime") != -1)
                        {
                            temp = Convert.ToDateTime(pfs[i].GetValue(hl, null)).ToString("yyyy-MM-dd");
                            fixmm[j].SetValue(xmmhl011, temp, null);
                        }
                    }
                    if (pfs[i].Name.Equals("ReportTitle") && fixmm[j].Name.Equals("PageNO"))
                    {
                        rep = pfs[i].GetValue(hl, null) as ReportTitle;
                        fixmm[j].SetValue(xmmhl011, rep.PageNO.ToString(), null);
                    }
                }

                //fixmm[fixmm.Length - 1].SetValue(xmmhl011, pfs[pfs.Length-1].GetValue(hl, null).ToString(),null);
            }
            //xmmhl011.PageNO = hl.ReportTitle.Id.ToString();
            return xmmhl011;
        }


        public T ConvertHLToXMMHL<T, T2>(T2 hl)
            where T : new()
            where T2 : new()
        {
            PropertyInfo[] pfs = hl.GetType().GetProperties();//利用反射获得类的属性

            T xmmhl011 = new T();
            PropertyInfo[] fixmm = xmmhl011.GetType().GetProperties();
            string temp = "";
            decimal changetemp = 0;

            string code = HttpContext.Current.Request["unitcode"].ToString().Substring(0, 2);
            for (int j = 0; j < fixmm.Length; j++)
            {
                for (int i = 0; i < pfs.Length; i++)
                {
                    if (fixmm[j].Name.ToUpper() == pfs[i].Name.ToUpper())
                    {
                        if (pfs[i].PropertyType.FullName == "System.String")
                        {
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = "";
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }
                            fixmm[j].SetValue(xmmhl011, temp, null);
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

                                if (code == "15" && (pfs[i].Name == "DQSW" || pfs[i].Name == "DQXSL"))  
                                {
                                    changetemp = Convert.ToDecimal(temp);  
                                    if (changetemp == 0)  //内蒙古市级填蓄水数据时可以填0
                                    {
                                        fixmm[j].SetValue(xmmhl011, "0", null);
                                    }
                                    else if (changetemp > 0)
                                    {
                                        if (pfs[i].Name == "DQXSL")
                                        {
                                            temp = String.Format("{0:N3}", changetemp).Replace(",", ""); //保留三位小数
                                        }
                                        else
                                        {
                                            temp = String.Format("{0:N2}", changetemp).Replace(",", "");
                                        }
                                        fixmm[j].SetValue(xmmhl011, temp, null);
                                    }
                                    continue;
                                }
                            }
                            changetemp = Convert.ToDecimal(temp);
                            if (changetemp != 0)
                            {
                                temp = String.Format("{0:N2}", changetemp).Replace(",", "");//全部两位小数
                                }
                            else
                            {
                                temp = "";
                            }
                            
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
                            fixmm[j].SetValue(xmmhl011, temp, null);
                        }
                        else if (pfs[i].PropertyType.FullName.IndexOf("System.DateTime") != -1)
                        {
                            temp = Convert.ToDateTime(pfs[i].GetValue(hl, null)).ToString("yyyy-MM-dd");
                            fixmm[j].SetValue(xmmhl011, temp, null);
                        }
                    }
                }
                }
            return xmmhl011;
        }

        /// <summary>
        /// 泛型方法,将T obj（HL011--HL014,HP011-HP012）传入，再将转换好的T obj（HL011--HL014M,HP011-HP012）传出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public T ToSetHL<T>(T obj, int limit)
        {
            PropertyInfo[] pfs = obj.GetType().GetProperties();//利用反射获得类的属性
            TableFieldBaseData tbBaseData = new TableFieldBaseData();
            string[] arr = { };
            string temp = "";
            decimal shuliangji = 0;
            double xiaoshu = 0;
            decimal changetemp = 0;
            for (int i = 0; i < pfs.Length; i++)
            {
                arr = tbBaseData.GetFieldUnitArr(pfs[i].Name, limit);
                if (arr != null)
                {
                    shuliangji = Convert.ToDecimal(arr[0]);
                    xiaoshu = Convert.ToDouble(arr[1]);
                }
                //if (pfs[i].PropertyType.FullName == "System.String")
                //{

                //}
                if (pfs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)// == "System.Decimal"
                {
                    if (pfs[i].GetValue(obj, null) == null)
                    {
                        temp = "0";
                    }
                    else
                    {
                        temp = pfs[i].GetValue(obj, null).ToString();
                    }
                    //temp = pfs[i].GetValue(obj, null).ToString();
                    changetemp = Convert.ToDecimal(temp);
                    if (shuliangji == 0)
                    {

                    }
                    else
                    {
                        changetemp = changetemp * shuliangji;
                    }

                    pfs[i].SetValue(obj, changetemp, null);
                }
                //else if (pfs[i].PropertyType.FullName == "System.Int32")
                //{

                //}
                //else if (pfs[i].PropertyType.FullName == "System.DateTime")
                //{

                //}
                //else
                //{

                //}
            }
            return obj;
        }
    }
}
