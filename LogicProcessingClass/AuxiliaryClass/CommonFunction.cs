using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using EntityModel;
using System.Collections;
using System.Data;
using DBHelper;

/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：
    // 文件名：CommonFunction.cs
    // 文件功能描述：公共访问类，提供一些公共使用方法
    // 创建标识：
    // 修改标识：
    // 修改描述：
//-------------------------------------------------------------*/
namespace LogicProcessingClass.AuxiliaryClass
{
    public class CommonFunction
    {
        FXDICTEntities fxdict = Persistence.GetDbEntities();
        /// <summary>
        /// 判断文件名是否已存在，若存在则另取：_n
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>新的文件名</returns>
        public string GetNewFileName(string fileName)
        {
            if (fileName != "")
            {
                string[] temp = fileName.Split('.');
                string hzm = "." + temp[1]; //后缀名

                string[] file = temp[0].Split('_');
                string name = ""; //文件名

                int i = 0;
                if (file.Length > 1)
                {
                    name = temp[0].Substring(0, temp[0].Length - file[file.Length - 1].Length - 1);
                }
                else
                {
                    name = temp[0];
                    i = 1;
                }
                string s = System.Web.HttpContext.Current.Server.MapPath("../Affix\\" + fileName);
                while (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("../Affix\\" + fileName))) //存在文件 
                {
                    fileName = name + "_" + i + hzm;
                    i++;
                }
            }
            return fileName;
        }

        /// <summary>
        /// 获取表类名称及代码
        /// </summary>
        /// <returns>JSON数据</returns>
        public string GetRptClass()
        {
            string jsonStr = "";
            try
            {
                var tb16s = from operateReportDefine in fxdict.TB16_OperateReportDefine
                           select new
                           {
                               operateReportDefine.OperateReportName,
                               operateReportDefine.OperateReportCode
                           };
                foreach (var obj in tb16s)
                {
                    jsonStr += "{name:'" + obj.OperateReportName + "',code:'" + obj.OperateReportCode + "'},";
                }

                if (jsonStr != "" || jsonStr != null)
                {
                    jsonStr = "'RptClass':[" + jsonStr.Remove(jsonStr.Length - 1) + "]";
                }
                else
                {
                    jsonStr = "'RptClass':[]";
                }
            }
            catch (Exception ex)
            {
                jsonStr = "错误消息：" + ex.Message;
            }
            return jsonStr;
        }

        /// <summary>
        /// 获取填表周期类型（CycType）
        /// </summary>
        /// <returns></returns>
        public string GetCycType()
        {
            string jsonStr = "";
            
            try
            {
                var tb16s = from oper in fxdict.TB16_OperateReportDefine
                            select new { 
                                oper.OperateReportCode,
                                oper.OptionalCyc
                            };
                 
                 foreach (var operobj in tb16s)
                 {
                     jsonStr += "'" + operobj.OperateReportCode + "':[";
                     var tb14s = from cyc in fxdict.TB14_Cyc
                                 //where operobj.OptionalCyc.Contains(cyc.CycType.ToString())
                                 select cyc;
                     string temp = "";
                     string[] cycTemps = operobj.OptionalCyc.Split(',');
                     for (int i = 0; i < cycTemps.Length; i++)
                     {
                         foreach (var obj in tb14s)
                         {
                             if (cycTemps[i] == obj.CycType.ToString())
                             {
                                 string remarkDet = "";
                                 if (obj.RemarkDetail != null)
                                 {
                                     remarkDet = obj.RemarkDetail;
                                 }
                                 temp += "{value:'" + obj.CycType + "',name:'" + obj.Remark + "',content:'" + remarkDet + "'},";
                             }
                         }
                     }
                     //foreach (var obj in tb14s)
                     //{
                     //    if (operobj.OptionalCyc.Contains(obj.CycType.ToString()))
                     //    {
                     //        string remarkDet = "";
                     //        if (obj.RemarkDetail != null)
                     //        {
                     //            remarkDet = obj.RemarkDetail;
                     //        }
                     //        temp += "{value:'" + obj.CycType + "',name:'" + obj.Remark + "',content:'" + remarkDet + "'},";
                     //    }
                     //}
                     if (temp != "")
                     {
                         jsonStr += temp.Remove(temp.Length - 1) + "],";
                     }
                    
                 }
                 if (jsonStr != "")
                 {
                     jsonStr = "'CycType':{" + jsonStr.Remove(jsonStr.Length - 1) + "}";
                 }
                 else
                 {
                     jsonStr = "'CycType':{}";
                 }
            }
            catch (Exception ex)
            {
                jsonStr = "错误消息：" + ex.Message;
            }
            return jsonStr;
        }

        /// <summary>
        /// 根据上级单位代码获取下级单位列表
        /// </summary>
        /// <param name="pUnitCode">上级单位代码</param>
        /// <returns>json数据</returns>
        public string GetLowerUnitList(string pUnitCode)
        {
            string jsonStr = "";
            try
            {
                var tb07s = from district in fxdict.TB07_District
                           where district.pDistrictCode == pUnitCode
                           select district;

                foreach (var obj in tb07s)
                {
                    jsonStr += "{unitCode:'" + obj.DistrictCode + "',unitName:'" + obj.DistrictName + "',pUnitCode:'" + obj.pDistrictCode + "',districtClass:'" + obj.DistrictClass + "',RD_RiverCode1:'" + obj.RD_RiverCode1 + "',uOrder:'" + obj.Uorder + "'},";
                }
                if (jsonStr != "" || jsonStr != null)
                {
                    jsonStr = "{LowerUnitList:[" + jsonStr.Remove(jsonStr.Length - 1) + "]}";
                }
                else
                {
                    jsonStr = "{LowerUnitList:[]}";
                }
            }
            catch (Exception ex)
            {
                jsonStr = "错误消息：" + ex.Message;
            }

            return jsonStr;
        }

        /// <summary>
        /// 查TB02中的单位数据
        /// </summary>
        /// <param name="mName">单位名</param>
        /// <returns></returns>
        public double GetNumber(string mName)
        {
            double result = 1;
            var tb02s = from measure in fxdict.TB02_Measure
                       where measure.MName == mName
                       select measure.MeasureValue;

            result = Convert.ToDouble(tb02s.First().Value);
            return result;
        }

        /// <summary>
        /// 获取死亡原因
        /// 
        /// </summary>
        /// <param name="parentReasonCode">父级死亡原因编号</param>
        /// <returns></returns>
        public string GetDeathReasonList(string parentReasonCode)
        {
            string jsonStr = "";
            return jsonStr;
        }

        /// <summary>
        /// 获得死亡原因列表
        /// </summary>
        /// <returns></returns>
        public string GetDeathReasonList()
        {
            var tb47s = from deathReason in fxdict.TB47_DeathReason
                       select deathReason;
            StringBuilder sb = new StringBuilder();
            sb.Append("DeathReason:{");
            if (tb47s.Count() != 0)
            {
                sb.Append("Data:[");
            }
            int i = 0;
            foreach(var obj in tb47s)
            {
                if (i == 0)
                {
                    sb.Append("{ReasonCode:'").Append(obj.ReasonCode).Append("',PReasonCode:'");
                    sb.Append(obj.PReasonCode).Append("',ReasonName:'").Append(obj.ReasonName);
                    sb.Append("',ReasonOrder:'").Append(obj.ReasonOrder).Append("'}");
                }
                else
                {
                    sb.Append(",").Append("{ReasonCode:'").Append(obj.ReasonCode).Append("',PReasonCode:'").Append(obj.PReasonCode);
                    sb.Append("',ReasonName:'").Append(obj.ReasonName).Append("',ReasonOrder:'").Append(obj.ReasonOrder).Append("'}");
                }
                i++;
            }
            if (tb47s.Count() != 0)
            {
                sb.Append("]");
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// 获得所有流域对照表
        /// </summary>
        /// <returns></returns>
        public string GetRiverCodeList()
        {
            //StringBuilder sb = new StringBuilder();
            //var tb09s = from riverDict in fxdict.TB09_RiverDict
            //           select riverDict;
            //sb.Append("Rivers:{");
            //int i = 0;
            //foreach(var obj in tb09s)
            //{
            //    if (i == 0)
            //    {
            //        sb.Append(obj.RiverCode).Append(":'").Append(obj.RiverName).Append("'");
            //    }
            //    else
            //    {
            //        sb.Append(",").Append(obj.RiverCode).Append(":'").Append(obj.RiverName).Append("'");
            //    }
            //    i++;
            //}
            //sb.Append("}");
            //return sb.ToString();
            string str = "";
            var tb09s = from riverDict in fxdict.TB09_RiverDict
                        select riverDict;
            //str ="Rivers:{";
            foreach (var obj in tb09s)
            {
                str += obj.RiverCode + ":'" + obj.RiverName + "',";
            }
            if (str != "")
            {
                str = "RiverCode:{" + str.Remove(str.Length - 1) + "}";
            }
            else
            {
                str = "RiverCode:{}";
            }
            return str;
        }

        /// <summary>
        /// 根据单位代码获取该单位的流域代码以及流域名称
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <returns>string</returns>
        public string GetRiverCodeAndName(string unitCode)
        {
            string river = "";
            string[] riverCode = GetRiverCodeByUnitCode(unitCode).Split(',');
            if (riverCode.Length <= 0)
            {
                return river;
            }
            var tb09s = from riverDict in fxdict.TB09_RiverDict
                       where riverCode.Contains(riverDict.RiverCode)
                       select riverDict;
            
            foreach(var obj in tb09s)
            {
                river += obj.RiverName + "|" + obj.RiverCode + ",";
            }
            if (river.Length > 0)
            {
                river = river.Remove(river.Length - 1);
            }
            return river;
        }

        /// <summary>
        /// 获取某单位的流域代码
        /// </summary>
        /// <param name="unitCode">单位编号</param>
        /// <returns>流域代码</returns>
        public string GetRiverCodeByUnitCode(string unitCode)
        {
            string riverCode = "";
            var tb07s = from district in fxdict.TB07_District
                       where district.DistrictCode == unitCode
                       select district;
            riverCode = tb07s.First().RD_RiverCode1.ToString();
            return riverCode;
        }
        /// <summary>
        /// 去除重复值时记录表重复的次数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public SortedList RecordStrRepeatTimes(string str)
        {
            SortedList slist = new SortedList();
            string[] tmp = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int times = 1;
            while (tmp.Length > 1)
            {
                string val = tmp[0];
                for (int i = 1; i < tmp.Length; i++)
                {
                    if (val == tmp[i])
                    {
                        times++;
                    }
                }

                for (int j = 0; j < times; j++)
                {
                    str = str.Remove(str.IndexOf(val), val.Length);
                }

                if (slist[times] != null)
                {
                    slist[times] += "," + val;
                }
                else
                {
                    slist[times] = val;
                }

                times = 1;
                tmp = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (tmp.Length == 1)
            {
                if (slist[1] != null)
                {
                    slist[1] += "," + tmp[0];
                }
                else
                {
                    slist[1] = tmp[0];
                }
            }
            return slist;
        }

        /// <summary>
        /// 获得省市县的校核数据
        /// </summary>
        /// <returns></returns>
        public string GetProvenceData()
        {
            StringBuilder equalityStr = new StringBuilder();
            StringBuilder inequalityStr = new StringBuilder();
            StringBuilder sbJson = new StringBuilder();
            string appName = "ProvenceData:";
            OperateApplication oa = new OperateApplication();
            IList list = null;
            if (oa.JudgeAppcalitionName(appName))
            {
                list = (IList)oa.GetApplication(appName);
            }
            else
            {
                list = fxdict.TB52_Formula.ToList();
                oa.SetApplication(appName, list);
            }
            for (int i = 0; i < list.Count; i++)
            {
                TB52_Formula obj = (TB52_Formula)list[i];
                if (obj.FormulaString == "=")
                {
                    equalityStr.Append("{Left:'").Append(obj.FieldDefine_No1).Append("',Middle:'").Append(obj.FormulaString);
                    equalityStr.Append("',Right:'").Append(obj.FieldDefine_No2).Append("'},");
                }
                else
                {
                    inequalityStr.Append("{Left:'").Append(obj.FieldDefine_No1).Append("',Middle:'").Append(obj.FormulaString);
                    inequalityStr.Append("',Right:'").Append(obj.FieldDefine_No2).Append("',Message:'").Append(obj.WrongName).Append("'},");
                }
                //sbStr.Append("{firstNumber:'").Append(obj.FieldDefine_No1).Append("',operationalsign:'").Append(obj.FormulaString);
                //sbStr.Append("',second:'").Append(obj.FieldDefine_No2).Append("',errorContext:'").Append(obj.WrongName).Append("'},");
                
            }
            if (equalityStr.ToString().Length > 0)
            {
                equalityStr.Remove(equalityStr.Length - 1, 1);
            }
            if (inequalityStr.ToString().Length > 0)
            {
                inequalityStr.Remove(inequalityStr.Length - 1, 1);
            }
            sbJson.Append("{").Append("Equality:[").Append(equalityStr).Append("],Inequality:[").Append(inequalityStr).Append("]").Append("}");
            return sbJson.ToString();
        }

        /// <summary>
        /// 获取所有操作表代码、表类代码和操作表名称
        /// </summary>
        /// <returns></returns>
        //public string GetRptClass()
        //{
        //    var tb16s = from operate in fxdict.TB16_OperateReportDefine
        //                select operate;
        //    string rptClass = "";
        //    foreach (var obj in tb16s)
        //    {
        //        rptClass += "{OperateReportCode:'" + obj.OperateReportCode + "',RC_Code:'" + obj.RC_Code + "',OperateReportName:'" + obj.OperateReportName + "'},";
        //    }
        //    if (rptClass != "")
        //    {
        //        rptClass = rptClass.Remove(rptClass.Length - 1);
        //    }
        //    return rptClass;
        //}

        /// <summary>去掉来自数据库的字符串中的换行符
        /// 
        /// </summary>
        /// <param name="newStr">字符串</param>
        /// <returns></returns>
        public string cleanString(string newStr)
        {
            string tempStr = System.Text.RegularExpressions.Regex.Replace(newStr, @"[\n\r]", "");
            return tempStr;
        }

        /// <summary>去掉字符串末尾换行符
        /// 
        /// </summary>
        /// <param name="newStr">字符串</param>
        /// <returns></returns>
        public string cleanEnd(string newStr)
        {
            newStr = newStr.TrimEnd((char[])"\n\r".ToCharArray());
            return newStr;
            //string tempStr = newStr.Replace((char)13, (char)0);
            //return tempStr.Replace((char)10, (char)0);
        }
    }
}
