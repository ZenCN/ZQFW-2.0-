using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using System.Collections;
using System.Data.Linq.SqlClient;
using System.Data.Linq;
using System.Web.Script.Serialization;
using DBHelper;
using System.Web;
using EntityModel.ReportAuxiliaryModel;
/*----------------------------------------------------------------
    // 版本说明：

    // 版本号：
    // 文件名：TableFieldBaseData.cs
    // 文件功能描述：获取表中各字段的基本数据信息(包含单位字段基础数据、字段校核数据，字段数量级小数位等数据)
    // 创建标识：
    // 修改标识：
    // 修改描述：
//--------------------------------------------------------------*/
namespace LogicProcessingClass.AuxiliaryClass
{
    public class TableFieldBaseData
    {
        FXDICTEntities fxdict = (FXDICTEntities)new Persistence().GetPersistenceEntity(EntitiesConnection.entityName.FXDICTEntities);

        /// <summary>
        /// 获得所有字段单位，每条数据为TB55_FieldDefine对象,并存储到Application[fieldUnit]中
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <returns></returns>
        public IList GetAllFieldUnitIList(int limit)
        {
            IList list = null;
            HttpApplicationState httpapplication = HttpContext.Current.Application;
            if (httpapplication["fieldUnit" + limit.ToString()] != null)
            {
                list = (IList)httpapplication["fieldUnit" + limit];
            }
            else
            {
                list = GetReportFormList();
                httpapplication["fieldUnit" + limit.ToString()] = list;
            }
            return list;
        }

        public string GetRiverUnits(string river_code)
        {
            if (river_code.Length == 0)
            {
                return "";
            }

            string result = "";
            string unitcode = HttpContext.Current.Request.Cookies["unitcode"].Value.ToString().Substring(0, 2) +
                              "000000";

            if (river_code.Trim() == "AAB00006,AB000000") //
            {
                if (unitcode == "15000000")
                {
                    result =
                        "[{UnitCode:'15150000',UnitName:'呼伦贝尔市'},{UnitCode:'15230000',UnitName:'兴安盟'},{UnitCode:'15170000',UnitName:'通辽市'},{UnitCode:'15140000',UnitName:'赤峰市'}]";
                }
                else if (unitcode == "22000000")
                {
                    result = "window.opener.$scope.BaseData.Unit.Unders";
                }

                return result;
            }
            else
            {
                var units =
                    fxdict.TB07_District.Where(
                        unit =>
                            unit.DistrictClass == 3 && unit.pDistrictCode == unitcode &&
                            unit.RD_RiverCode1.Contains(river_code))
                        .OrderBy(unit => unit.Uorder).ToList();
                foreach (var unit in units)
                {
                    result += "{UnitCode:'" + unit.DistrictCode + "',UnitName:'" + unit.DistrictName + "'},";
                }
            }

            if (result.Length > 0)
            {
                result = "[" + result.Remove(result.Length - 1) + "]";
            }

            return result;
        }


        /// <summary>
        /// 根据给出的字段获取基础数据（该校核数据是没有经过数量级处理的）
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="fields">字段名(字段之间用“，”分隔)</param>
        /// <returns></returns>
        public string FiledsBaseData(string unitCode, string fields)
        {
            var tb04s = from tb04 in fxdict.TB04_CheckBase
                        from tb20 in fxdict.TB20_FieldDefine
                        where tb04.District_Code == unitCode &&
                        fields.Contains(tb20.FieldCode) && // linq to entities 中不能用contains 来实现in
                        tb04.FieldDefine_NO == tb20.TBNO
                        select new
                        {
                            tb20.FieldCode,
                            tb04.BaseData
                        };
            string returnStr = "BaseData:{";
            string temp = "";
            foreach (var obj in tb04s)
            {
                if (obj.FieldCode != null || obj.FieldCode.Equals(""))
                {
                    temp += obj.FieldCode + ":" + obj.BaseData + ",";
                }
            }
            if (temp.Length > 0)
            {
                temp = temp.Substring(0, temp.Length - 1);
                returnStr += temp + "}";
                return returnStr;
            }
            else
            {
                return "BaseData:{}";
            }
        }

        /// <summary>
        /// 获得某个单位所有下级单位的基础校核数据（所有省、市、县基础数据校核）
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <param name="unitCode">单位编号</param>
        /// <param name="iSessionFXDICT">数据库连接</param>
        /// <returns></returns>
        public string QueryCheckBaseData(int limit, string unitCode)
        {
            string jsonStr = "";
            char[] unitCodeCharArr = unitCode.ToCharArray();
            string startUnitCode = "";
            string endUnitCode = "";
            switch (limit)
            {
                case 2://省级
                    //startUnitCode = unitCodeCharArr[0].ToString() + unitCodeCharArr[1].ToString();  //????为什么要拆成字符数组？可以直接采用下列方法
                    startUnitCode = unitCode.Substring(0, 2);
                    endUnitCode = "0000";
                    break;
                case 3://市级
                    //startUnitCode = unitCodeCharArr[0].ToString() + unitCodeCharArr[1].ToString() + unitCodeCharArr[2].ToString()+ unitCodeCharArr[3].ToString();
                    startUnitCode = unitCode.Substring(0, 4);
                    endUnitCode = "00";
                    break;
                case 4://县级
                    //startUnitCode = unitCodeCharArr[0].ToString() + unitCodeCharArr[1].ToString() + unitCodeCharArr[2].ToString()+ unitCodeCharArr[3].ToString() + unitCodeCharArr[4].ToString() + unitCodeCharArr[5];
                    startUnitCode = unitCode.Substring(0, 6);
                    endUnitCode = "";
                    break;
                default://乡镇  乡镇的基础数据比较少，并且是获取自己单位的数据，非下级所有单位的，直接写死，是特殊的。
                    jsonStr = QueryTownBaseData(unitCode);
                    return jsonStr;
            }
            var tb04_tb20s = (from tb20 in fxdict.TB20_FieldDefine
                              from tb04 in fxdict.TB04_CheckBase
                              where tb04.FieldDefine_NO == tb20.TBNO &&
                              tb04.District_Code.EndsWith(endUnitCode) &&
                              tb04.District_Code.StartsWith(startUnitCode)
                              orderby tb04.TBNO descending
                              //SqlMethods.Like(tb04.District_Code, likeUnitCode)//该方法可以与T-SQL中like使用方法一样
                              select new
                              {
                                  tb20.FieldCode,
                                  tb20.FieldCNName,
                                  tb04.BaseData,
                                  tb20.TD_TabCode,
                                  tb04.District_Code
                              }).Distinct();

            //var baseDataList = tb04_tb20s.ToList();
            ArrayList baseDataList = new ArrayList();
            object[] temp = { };
            foreach (var obj in tb04_tb20s)//因为是匿名类型，在其他方法里面不能很好的取值或直接转换成object数组 手动转换成object数组存入list中
            {
                temp = new object[5];
                temp[0] = obj.FieldCode;
                temp[1] = obj.FieldCNName;
                temp[2] = obj.BaseData.ToString();
                temp[3] = obj.TD_TabCode;
                temp[4] = obj.District_Code;
                baseDataList.Add(temp);
            }

            //GetAllFieldUnit gafu = new GetAllFieldUnit();
            IList list = GetAllFieldUnitIList(limit);
            jsonStr = transformBaseData(baseDataList, list, limit);
            return jsonStr;
            //显示linq生成的sql
            //DataContext l = new DataContext(;
            //string sss= l.Log.ToString();
            //using (NorthwindDataContext context = new NorthwindDataContext())
            //{
            //    context.Log = Console.Out;
            //}
        }

        /// <summary>
        /// 获取乡镇的基础数据（已进行数量级转换）
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <returns></returns>
        public string QueryTownBaseData(string unitCode)
        {
            var objs = (from tb20 in fxdict.TB20_FieldDefine
                        from tb04 in fxdict.TB04_CheckBase
                        where tb04.FieldDefine_NO == tb20.TBNO &&
                        tb04.District_Code == unitCode
                        orderby tb04.TBNO descending
                        select new
                        {
                            tb20.FieldCode,
                            tb20.FieldCNName,
                            tb04.BaseData,
                            tb20.TD_TabCode,
                            tb04.District_Code
                        }).Distinct();
            string str = "{";
            string temp = "";
            string flag = "";
            foreach (var obj in objs)
            {
                if (obj.FieldCode != null || obj.FieldCode.Equals(""))
                {
                    if (obj.FieldCode.Equals("SHMJXJ"))
                    {
                        flag = String.Format("{0:N4}", Convert.ToDouble(obj.BaseData) / 666.6667);
                        flag = flag.Replace(",", "");

                    }
                    else if (obj.FieldCode.Equals("SZRK"))
                    {
                        flag = Convert.ToString(Convert.ToDouble(obj.BaseData));
                    }
                    else if (obj.FieldCode.Equals("ZJJJZSS"))
                    {
                        flag = Convert.ToString(Convert.ToDouble(obj.BaseData) / 10000);
                    }
                    else if (obj.FieldCode.Equals("SLSSZJJJSS"))
                    {
                        flag = Convert.ToString(Convert.ToDouble(obj.BaseData) / 10000);
                    }
                    if (flag != "" && Convert.ToDouble(flag) > 0)
                    {
                        temp += obj.FieldCode + ":" + flag + ",";
                    }
                }
                else
                {
                    return temp;
                }
            }
            if (temp.Length > 0)
            {
                temp = temp.Substring(0, temp.Length - 1);
                str = str + temp + "}";
            }
            else
            {
                str = "{}";
            }
            return str;
        }

        /// <summary>
        /// 转换基础数据，根据tb04中的基数，以及tb55中字段系数，小数位等信息把基础数据进行转换， 生成指定json格式的数据
        /// </summary>
        /// <param name="baseDataList">校核数据list</param>
        /// <param name="fieldUnitLists">tb55中的字段单位数据的list</param>
        /// <param name="limit">单位级别</param>
        /// <returns></returns>
        public string transformBaseData(IList baseDataList, IList fieldUnitLists, int limit)
        {
            StringBuilder sb = new StringBuilder();
            TransformJSON tjson = new TransformJSON();
            Dictionary<string, string> xh = new Dictionary<string, string>();
            for (int i = 0; i < baseDataList.Count; i++)
            {
                object[] objarr = (object[])baseDataList[i];
                for (int j = 0; j < fieldUnitLists.Count; j++)
                {
                    TB55_FieldDefine tb55 = (TB55_FieldDefine)fieldUnitLists[j];
                    if (objarr[0].Equals(tb55.FieldCode) && objarr[3].Equals(tb55.TD_TabCode) && limit.ToString().Equals(tb55.UnitCls.ToString()))
                    {
                        int decimalNumber = Convert.ToInt32(tb55.DecimalCount);
                        double numberUnit = Convert.ToDouble(tb55.MeasureValue);
                        double numberBaseData = Convert.ToDouble(objarr[2] == "" ? "0" : objarr[2]);
                        if (numberUnit == 0)
                        {
                            numberUnit = 1;
                        }
                        numberBaseData = numberBaseData / numberUnit;
                        string temp = Convert.ToString(numberBaseData).Replace(",", "");
                        //string basedata = String.Format("{0:N" + tb55.DecimalCount + "}", Convert.ToString(numberBaseData).Replace(",", ""));
                        string basedata = Math.Round(Convert.ToDecimal(temp), Convert.ToInt32(tb55.DecimalCount)).ToString();
                        Dictionary<string, string> dh = new Dictionary<string, string>();
                        dh.Add("value", basedata);
                        dh.Add("name", objarr[1].ToString().Split('（')[0]);
                        string sssss = tjson.CreateObjJSON(dh);
                        string key = objarr[4].ToString() + "-" + objarr[3].ToString() + "-" + objarr[0].ToString();
                        if (xh.ContainsKey(key))//如果存在该key那么不添加了  添加的基础数据以最新的那条为准（tb04中按照tbno降序排列最大的值是最新的）
                        {
                            break;
                        }
                        else
                        {
                            xh.Add(key, tjson.CreateObjJSON(dh));
                            break;
                        }
                        //t20.FieldCode,t20.FieldcnName,tb04.BaseData,t20.TdTabCode,tb04.District_Code
                        //FieldCode,TDTabCode,MeasureValue,DecimalCount,MeasureName,InputRemark,UnitCls
                    }
                }
            }
            sb.Append(tjson.CreateObjJSON(xh).Replace("\"{", "{").Replace("}\"", "}").Replace("\\\"", "\""));
            return sb.ToString();
        }

        /// <summary>
        /// tb20中获取填表说明
        /// </summary>
        /// <returns></returns>
        public string GetTableExplain()
        {
            var tb20s = from fieldDefine in fxdict.TB20_FieldDefine
                        where fieldDefine.TD_TabCode != "ReportTittle" &&
                        fieldDefine.InputRemark != ""
                        select new
                        {
                            fieldDefine.FieldCode,
                            fieldDefine.InputRemark
                        };
            TransformJSON trnJson = new TransformJSON();
            string explain = trnJson.CreateJSON(tb20s.ToList());
            return explain;
        }

        /// <summary>
        /// 获取某一级别的字段名，数量级名称(中文的单位：千公顷等）
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <param name="unitCode">单位代码</param>
        /// <returns>字段名称和数量级名称</returns>
        //public string QueryFieldUnit(int limit, string unitCode)
        public string GetFieldMeasureName(int limit)
        {
            StringBuilder sb = new StringBuilder();
            //GetAllFieldUnit gafu = new GetAllFieldUnit();
            IList list = GetAllFieldUnitIList(limit);
            if (list.Count > 0)
            {
                sb.Append("{");
                for (int i = 0; i < list.Count; i++)
                {
                    TB55_FieldDefine tb55 = (TB55_FieldDefine)list[i];
                    if (tb55.MeasureName != null && tb55.MeasureName != "" && Convert.ToInt32(tb55.UnitCls) == limit)
                    {
                        sb.Append(tb55.FieldCode + ":'" + tb55.MeasureName + "',");
                    }

                    //FieldCode,TDTabCode,MeasureValue,DecimalCount,MeasureName,InputRemark,UnitCls
                    //if (obj[4] != null && obj[4].ToString() != "" && Convert.ToInt32(obj[6]) == limit)
                    //{
                    //    sb.Append(obj[0] + ":'" + obj[4] + "',");
                    //}
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("}");
            }
            else
            {
                return "{}";
            }
            return sb.ToString();
        }


        /// <summary>
        /// 获取计量单位(整数)
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <returns></returns>
        public string GetMeasureValue(int limit)
        {
            var tb55s = from fieldDefine in fxdict.TB55_FieldDefine
                        where fieldDefine.MeasureValue > 1 &&
                        fieldDefine.UnitCls == limit
                        select new
                        {
                            fieldDefine.FieldCode,
                            fieldDefine.MeasureValue
                        };
            string result = new TransformJSON().CreateJSON(tb55s.ToList());
            return result;
        }


        /// <summary>
        /// 根据字段名,从Application里面查询出对应字段换算级和保留小数位数

        /// 返回string[]数组(fieldArr[0] = 数量级单位 , fieldArr[1] = 保留小数位数)
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="limit">单位级别</param>
        /// <returns>string[]数组(fieldArr[0] = 数量级系数 , fieldArr[1] = 保留小数位数)</returns>
        public string[] GetFieldUnitArr(string fieldName, int limit)
        {
            IList list = null;
            HttpApplicationState httpapplication = HttpContext.Current.Application;
            if (httpapplication["fieldUnit" + limit.ToString()] != null)//不同级别的字段数据
            {
                list = (IList)httpapplication["fieldUnit" + limit.ToString()];
            }
            else
            {
                list = GetReportFormList();
                httpapplication["fieldUnit" + limit.ToString()] = list;
            }
            string[] fieldArr = new string[2];
            for (int i = 0; i < list.Count; i++)
            {
                TB55_FieldDefine tb55 = (TB55_FieldDefine)list[i];
                if (fieldName.Equals(tb55.FieldCode.ToString()))
                {
                    if (limit.ToString().Equals(tb55.UnitCls.ToString()))
                    {
                        fieldArr[0] = tb55.MeasureValue.ToString();//数量级系数
                        fieldArr[1] = tb55.DecimalCount.ToString();//保留小数位
                        break;
                    }
                }
            }
            return fieldArr;
        }



        /// <summary>
        /// 查询tb55的数据（可用于表1到表9对应的字段数量级和小数位）
        /// </summary>
        /// <returns></returns>
        public IList GetReportFormList()
        {
            IList list = null;
            var tb55 = from fieldDefine in fxdict.TB55_FieldDefine
                       select fieldDefine;
            //select new
            //{
            //    fieldDefine.FieldCode,
            //    fieldDefine.TD_TabCode,
            //    fieldDefine.MeasureValue,
            //    fieldDefine.DecimalCount,
            //    fieldDefine.MeasureName,
            //    fieldDefine.InputRemark,
            //    fieldDefine.UnitCls
            //};
            //list = fxdict.TB55_FieldDefine.ToList();
            list = tb55.ToList();
            return list;
        }
        /// <summary>获取tb55表中字段的数据
        /// </summary>
        /// <returns></returns>
        public string GetTB55Fields(int limit)
        {
            var tb55s = from fieldDefine in fxdict.TB55_FieldDefine
                          where fieldDefine.UnitCls == limit 
                          //&& fieldDefine.MeasureName != null&& fieldDefine.MeasureName != "" && fieldDefine.MeasureValue > 1
                          select new
                          {
                              fieldDefine.FieldCode,
                              fieldDefine.MeasureValue,
                              fieldDefine.MeasureName,
                              fieldDefine.InputRemark,
                              fieldDefine.DecimalCount
                          };
            string json = "";
            foreach (var obj in tb55s)
            {
                json += "'" + obj.FieldCode + "':{'MeasureValue':" + Convert.ToInt32(obj.MeasureValue) + ",'MeasureName':'" + obj.MeasureName + "','InputRemark':'" + obj.InputRemark +"','DecimalCount':"+Convert.ToInt32(obj.DecimalCount)+ "},";
            }
            if (json != "")
            {
                json = json.Remove(json.Length - 1);
            }
            return json;
        }

        /// <summary>
        /// 根据单位代码获取该单位的流域代码
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <returns>流域代码</returns>
        public string GetRiverCode(string unitCode)
        {
            var tb07s = from fieldDefine in fxdict.TB07_District
                        select fieldDefine.RD_RiverCode1;
            string result = tb07s.FirstOrDefault();
            return result;
        }

    }
}
