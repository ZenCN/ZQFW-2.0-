using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBHelper;
using EntityModel;
using System.Globalization;
using System.Data.Linq.SqlClient;
using System.Data.Objects.SqlClient;
using System.Web;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：GetTreeNote.cs
// 文件功能描述：获得树形菜单的数据
// 创建标识：
// 修改标识：// 修改描述：
//-------------------------------------------------------------*/
using LogicProcessingClass.Model;

namespace LogicProcessingClass.ReportOperate
{
    public class GetTreeNode
    {
        private BusinessEntities busEntity = null;
        private Dictionary<string, string> units = new Dictionary<string, string>();
        private HttpApplicationState App = HttpContext.Current.Application;
        public GetTreeNode(int limit, int typeLimit)
        {
            if (typeLimit == 1)
            {
                limit = limit + 1;
            }
            Entities getEntity = new Entities();
            busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
        }


        /// <summary>获得当年树形菜单的数据(打开页面中)
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <param name="unitCode">当前单位编号</param>
        /// <param name="typeLimit">查看本级(0)表或者下级(1)表</param>
        /// <param name="tableName">表名HL01等</param>
        /// <param name="cycDate">时段类型(实时、过程报等)</param>
        /// <param name="nextUnitCode">下级单位编号</param>
        /// <returns></returns>
        public string GetCurYearTreeNodeData(int limit, string unitCode, int typeLimit, string tableName, string cycDate, string nextUnitCode, string typeTable,bool queryOneUnit)
        {
            //var entity = new FXDICTEntities();
            if (queryOneUnit)
            {
                var entity = new FXDICTEntities();
                var district = (TB07_District)entity.TB07_District.Where(t => t.DistrictCode == unitCode).SingleOrDefault();

                if (district != null)
                {
                    units.Add(district.DistrictCode, district.DistrictName);
                }
            }
            else
            {
                Persistence per = new Persistence();
                Dictionary<string, District> units_dic = per.GetLowerUnits(unitCode);
                if (units_dic.Count > 0)
                {
                    foreach (District dis in units_dic.Values)
                    {
                        units.Add(dis.UnitCode, dis.UnitName);
                    }
                }
            }

            string jsonStr = "[";
            int year = DateTime.Now.Year;
            DateTime dt = DateTime.Now.AddDays(-1);//取最近两天的时间,包含当天
            string dateStr = dt.ToString();
            int year1 = dt.Year;
            IList list1 = GetReceivedReportForm(typeLimit, unitCode, limit, year1, dateStr, tableName, cycDate, nextUnitCode, typeTable, queryOneUnit);//获得最近两天数据

            string jsonData = GetMonthData(list1, typeLimit);
            if (typeLimit == 1)
            {
                jsonStr += "{name:'昨天今天装入的报表',open:'true',nocheck:'true',iconSkin:'ico_close ',title:'',children:[" + jsonData + "]},";
            }
            else
            {
                jsonStr += "{name:'昨天今天修改保存的报表',open:'true',nocheck:'true',iconSkin:'ico_close ',title:'',children:[" + jsonData + "]},";
            }
            for (int i = year; i >= year; i--)
            {
                IList list = GetReceivedReportForm(typeLimit, unitCode, limit, i, "", tableName, cycDate, nextUnitCode, typeTable, queryOneUnit);
                jsonStr += "{name:'" + i + "年（总共" + list.Count + "套报表）',nocheck:'true'" + (year == i ? ",open:'true'" : "");
                jsonStr += ",title:'',children: [";
                jsonStr += GetMonthData(list, 3, typeLimit);
                jsonStr += "]},";

                if (list.Count > 0)
                {
                    ArrayList pageNoArray = GetPageNO(list, limit, typeLimit, 0, 6);
                    jsonStr = ReplaceLockedID("id", jsonStr, pageNoArray);
                }
            }
            if (jsonStr != "")
            {
                jsonStr = jsonStr.Remove(jsonStr.Length - 1);
            }

            jsonStr += "]";
            return jsonStr;
        }
        /// <summary>获得往年树形菜单的数据(打开页面中)
        /// </summary>
        /// <param name="limit">单位级别</param>
        /// <param name="unitCode">当前单位编号</param>
        /// <param name="typelimit">查看本级(0)表或者下级(1)表</param>
        /// <param name="tableName">表名HL01等</param>
        /// <param name="cycDate">时段类型(实时、过程报等)</param>
        /// <param name="nextUnitCode">下级单位编号</param>
        /// <returns></returns>
        public string GetFormerYearsTreeNodeData(int limit, string unitCode, int typelimit, string tableName, string cycDate, string nextUnitCode, string typeTable, bool queryOneUnit)
        {

            string jsonStr = "";
            //if (App[unitCode + "TreeUnits"] != null)
            //{
            //    units = (Dictionary<string, string>)App[unitCode + "TreeUnits"];
            //}
            //else
            //{
            /*var tb07s = new FXDICTEntities().TB07_District.Where(t => t.pDistrictCode == unitCode).ToList();
            foreach (var tb07District in tb07s)
            {
                units.Add(tb07District.DistrictCode, tb07District.DistrictName);
            }*/
            var entity = new FXDICTEntities();
            if (queryOneUnit)
            {
                foreach (
                    var tb07District in
                        entity.TB07_District.Where(t => t.DistrictCode == unitCode).ToList())
                {
                    units.Add(tb07District.DistrictCode, tb07District.DistrictName);
                }
            }
            else
            {
                foreach (
                    var tb07District in
                        entity.TB07_District.Where(t => t.pDistrictCode == unitCode).ToList())
                {
                    units.Add(tb07District.DistrictCode, tb07District.DistrictName);
                }
            }
            //    App[unitCode + "TreeUnits"] = units;
            //}
            //if (App["FormerYearsTreeNodeData" + unitCode + typelimit] != null)//判断是否在缓存中
            //{
            //    jsonStr = App["FormerYearsTreeNodeData" + unitCode + typelimit].ToString();
            //}
            //else
            //{
            jsonStr = "[";
            int year = DateTime.Now.Year;
            var rptDate = busEntity.ReportTitle.Select(p => p.ReceiveTime).Min();
            int minYear = year - 1;
            if (rptDate != null)
            {
                minYear = Convert.ToDateTime(rptDate).Year; //找出有数据的最小年份
            }
            for (int i = year - 1; i >= (minYear == year ? year - 1 : minYear); i--)
            {
                IList list = GetReceivedReportForm(typelimit, unitCode, limit, i, "", tableName, cycDate, nextUnitCode, typeTable, queryOneUnit);
                if (list.Count == 0)
                {
                    continue;
                }
                jsonStr += "{name:'" + i + "年（总共" + list.Count + "套报表）',nocheck:'true'";
                jsonStr += ",title:'',children: [";
                jsonStr += GetMonthData(list, 3, typelimit);
                jsonStr += "]},";

                //张建军  获取锁定表页号  省级、市级   备注：下级表的删除在接收表箱操作
                if (list.Count > 0)  //typelimit == 0 && 
                {
                    ArrayList pageNoArray = GetPageNO(list, limit, typelimit, 0, 6);  //((object[])(list1[0])).Length - 1
                    jsonStr = ReplaceLockedID("id", jsonStr, pageNoArray);
                }
            }
            if (jsonStr != "[")
            {
                jsonStr = jsonStr.Remove(jsonStr.Length - 1);
                jsonStr += "]";
            }
            else
            {
                jsonStr = "";
            }
            //jsonStr += "]";
            //if (jsonStr != "")
            //{
            //    jsonStr = jsonStr.Remove(jsonStr.Length - 1);
            //}

            //    App["FormerYearsTreeNodeData" + unitCode + typelimit] = jsonStr;
            //}
            return jsonStr;
        }

        public string GetFormerYearsTreeNodeData(int limit, string unitCode, int typelimit, string tableName, string cycDate, string nextUnitCode, string typeTable, bool queryOneUnit, int minYear)
        {

            string jsonStr = "";
            //if (App[unitCode + "TreeUnits"] != null)
            //{
            //    units = (Dictionary<string, string>)App[unitCode + "TreeUnits"];
            //}
            //else
            //{
            /*var tb07s = new FXDICTEntities().TB07_District.Where(t => t.pDistrictCode == unitCode).ToList();
            foreach (var tb07District in tb07s)
            {
                units.Add(tb07District.DistrictCode, tb07District.DistrictName);
            }*/
            var entity = new FXDICTEntities();
            if (queryOneUnit)
            {
                foreach (
                    var tb07District in
                        entity.TB07_District.Where(t => t.DistrictCode == unitCode).ToList())
                {
                    units.Add(tb07District.DistrictCode, tb07District.DistrictName);
                }
            }
            else
            {
                foreach (
                    var tb07District in
                        entity.TB07_District.Where(t => t.pDistrictCode == unitCode).ToList())
                {
                    units.Add(tb07District.DistrictCode, tb07District.DistrictName);
                }
            }
            //    App[unitCode + "TreeUnits"] = units;
            //}
            //if (App["FormerYearsTreeNodeData" + unitCode + typelimit] != null)//判断是否在缓存中
            //{
            //    jsonStr = App["FormerYearsTreeNodeData" + unitCode + typelimit].ToString();
            //}
            //else
            //{
            jsonStr = "[";
            int startYear = DateTime.Now.Year;
            bool isHistory = false;
            if (minYear == -1)
            {
                var rptDate = busEntity.ReportTitle.Where(t => t.ORD_Code.StartsWith("SH") && t.Del != 1).Select(p => p.EndDateTime).Min();
                startYear = startYear - 2;
                if (rptDate != null && Convert.ToDateTime(rptDate).Year <= startYear)
                {
                    minYear = Convert.ToDateTime(rptDate).Year; //找出有数据的最小年份
                    isHistory = true;
                }
                else  //数据库中没有前年及更早的数据
                {
                    return "[]";
                }
            }
            //int year = DateTime.Now.Year;
            //var rptDate = busEntity.ReportTitle.Select(p => p.ReceiveTime).Min();
            //int minYear = year - 1;
            /*if (rptDate != null)
            {
                minYear = Convert.ToDateTime(rptDate).Year; //找出有数据的最小年份
            }*/
            for (int i = startYear; i >= minYear; i--)
            {
                IList list = GetReceivedReportForm(typelimit, unitCode, limit, i, "", tableName, cycDate, nextUnitCode, typeTable, queryOneUnit);
                if (list.Count == 0 && isHistory)
                {
                    continue;
                }
                jsonStr += "{name:'" + i + "年（总共" + list.Count + "套报表）',nocheck:'true'";
                if (i == DateTime.Now.Year)
                {
                    jsonStr += ",open:'true'";
                }
                jsonStr += ",title:'',children: [";
                jsonStr += GetMonthData(list, 3, typelimit);
                jsonStr += "]},";

                //张建军  获取锁定表页号  省级、市级   备注：下级表的删除在接收表箱操作
                if (list.Count > 0)  //typelimit == 0 && 
                {
                    ArrayList pageNoArray = GetPageNO(list, limit, typelimit, 0, 6);  //((object[])(list1[0])).Length - 1
                    jsonStr = ReplaceLockedID("id", jsonStr, pageNoArray);
                }
            }
            if (jsonStr != "[")
            {
                jsonStr = jsonStr.Remove(jsonStr.Length - 1);
                jsonStr += "]";
            }
            else
            {
                jsonStr = "";
            }
            //jsonStr += "]";
            //if (jsonStr != "")
            //{
            //    jsonStr = jsonStr.Remove(jsonStr.Length - 1);
            //}

            //    App["FormerYearsTreeNodeData" + unitCode + typelimit] = jsonStr;
            //}
            jsonStr = jsonStr.Replace("[汇总]", "").Replace("[录入]", "").Replace("[初报]", "[已报送]");

            return jsonStr;
        }

        /// <summary>获取新建页面树形菜单数据（如洪涝表、蓄水表）
        /// </summary>
        /// <returns></returns>
        public static string GetCreateTreeData()
        {
            FXDICTEntities fxdict = (FXDICTEntities)new Entities().GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            string str = "RptClass:[";
            var tb15s = fxdict.TB15_RptClass.AsQueryable();
            foreach (var tb15 in tb15s)
            {
                string childs = "";
                str += "{name:'" + tb15.RptClassShortName + "',open:'true',children:[";
                foreach (var tb16 in tb15.TB16_OperateReportDefine)
                {
                    childs += "{name:'" + tb16.OperateReportName + "',id:'" + tb16.OperateReportCode + "'},";
                }
                if (childs != "")
                {
                    childs = childs.Remove(childs.Length - 1);
                }
                str = str + childs + "]},";
            }
            if (str != "")
            {
                str = str.Remove(str.Length - 1) + "]";
            }
            return str;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list">数据集合</param>
        /// <param name="numberField">字段编号</param>
        /// <param name="typelimit">查看本级(0)表或者下级(1)表</param>
        /// <returns></returns>
        public string GetMonthData(IList list, int numberField, int typelimit)
        {
            string jsonStr = "";
            string month1 = "";
            string month2 = "";
            string month3 = "";
            string month4 = "";
            string month5 = "";
            string month6 = "";
            string month7 = "";
            string month8 = "";
            string month9 = "";
            string month10 = "";
            string month11 = "";
            string month12 = "";
            for (int i = 0; i < list.Count; i++)
            {
                Object[] objData = (Object[])list[i];
                DateTime dt = Convert.ToDateTime(objData[3]);
                int month = dt.Month;
                switch (month)
                {
                    case 1://1月
                        month1 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 2://2月
                        month2 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 3://3月
                        month3 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 4://4月
                        month4 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 5://5月
                        month5 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 6://6月
                        month6 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 7://7月
                        month7 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 8://8月
                        month8 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 9://9月
                        month9 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 10://10月
                        month10 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 11://11月
                        month11 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    case 12://12月
                        month12 += "{" + GetMonthData(objData, typelimit) + "},";
                        break;
                    default:
                        break;
                }
            }
            if (month1 != "")
            {
                month1 = month1.Remove(month1.Length - 1);
                jsonStr += "{name:'1月',nocheck:'true',children:[" + month1 + "]},";
            }
            else
            {
                jsonStr += "{name:'1月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month2 != "")
            {
                month2 = month2.Remove(month2.Length - 1);
                jsonStr += "{name:'2月',nocheck:'true',children:[" + month2 + "]},";
            }
            else
            {
                jsonStr += "{name:'2月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }

            if (month3 != "")
            {
                month3 = month3.Remove(month3.Length - 1);
                jsonStr += "{name:'3月',nocheck:'true',children:[" + month3 + "]},";
            }
            else
            {
                jsonStr += "{name:'3月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }

            if (month4 != "")
            {
                month4 = month4.Remove(month4.Length - 1);
                jsonStr += "{name:'4月',nocheck:'true',children:[" + month4 + "]},";
            }
            else
            {
                jsonStr += "{name:'4月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month5 != "")
            {
                month5 = month5.Remove(month5.Length - 1);
                jsonStr += "{name:'5月',nocheck:'true',children:[" + month5 + "]},";
            }
            else
            {
                jsonStr += "{name:'5月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month6 != "")
            {
                month6 = month6.Remove(month6.Length - 1);
                jsonStr += "{name:'6月',nocheck:'true',children:[" + month6 + "]},";
            }
            else
            {
                jsonStr += "{name:'6月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month7 != "")
            {
                month7 = month7.Remove(month7.Length - 1);
                jsonStr += "{name:'7月',nocheck:'true',children:[" + month7 + "]},";
            }
            else
            {
                jsonStr += "{name:'7月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month8 != "")
            {
                month8 = month8.Remove(month8.Length - 1);
                jsonStr += "{name:'8月',nocheck:'true',children:[" + month8 + "]},";
            }
            else
            {
                jsonStr += "{name:'8月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month9 != "")
            {
                month9 = month9.Remove(month9.Length - 1);
                jsonStr += "{name:'9月',nocheck:'true',children:[" + month9 + "]},";
            }
            else
            {
                jsonStr += "{name:'9月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month10 != "")
            {
                month10 = month10.Remove(month10.Length - 1);
                jsonStr += "{name:'10月',nocheck:'true',children:[" + month10 + "]},";
            }
            else
            {
                jsonStr += "{name:'10月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month11 != "")
            {
                month11 = month11.Remove(month11.Length - 1);
                jsonStr += "{name:'11月',nocheck:'true',children:[" + month11 + "]},";
            }
            else
            {
                jsonStr += "{name:'11月',nocheck:'true',title:'',iconSkin:'ico_close '},";
            }
            if (month12 != "")
            {
                month12 = month12.Remove(month12.Length - 1);
                jsonStr += "{name:'12月',nocheck:'true',children:[" + month12 + "]},";
            }
            else
            {
                jsonStr += "{name:'12月',nocheck:'true',title:'',iconSkin:'ico_close '}";
            }
            return jsonStr;
        }
        public IList GetReceivedReportForm(int typeLimit, string unitCode, int limit, int year, string lastDate, string tableName, string typeDate, string nextUnitCode, string typeTable, bool queryOneUnit)
        {
            //string fieldArr = "select Id,UnitCode,StartDateTime,EndDateTime,StatisticalCycType,SourceType,State,SendOperType ";//" Id,UnitCode,StartDateTime,EndDateTime";


            string sourceType = "0,1,2";
            var rpts = busEntity.ReportTitle.Where("it.SourceType in {" + sourceType + "}").AsQueryable();
            rpts = rpts.Where(t => t.Del == 0 && t.RPTType_Code == "XZ0");//
            if (queryOneUnit)
            {
                rpts = (from rpt in rpts
                        where rpt.UnitCode == unitCode
                        select rpt).AsQueryable();
            }
            else
            {
                if (limit == 0) //国家防总,查询省级数据
                {
                    rpts = (from rpt in rpts
                            where rpt.UnitCode.EndsWith("000000")
                            select rpt).AsQueryable();
                }
                else if (limit == 2) //省级,查询市级数据
                {
                    rpts = (from rpt in rpts
                            where rpt.UnitCode.StartsWith(unitCode.Substring(0, 2)) &&
                                  rpt.UnitCode.EndsWith("0000")
                            select rpt).AsQueryable();
                }
                else if (limit == 3) //市级，查询县级数据
                {
                    rpts = (from rpt in rpts
                            where rpt.UnitCode.StartsWith(unitCode.Substring(0, 4)) &&
                                  rpt.UnitCode.EndsWith("00")
                            select rpt).AsQueryable();
                }
                else if (limit == 4) //县级，查询乡级数据
                {
                    rpts = (from rpt in rpts
                            where rpt.UnitCode.StartsWith(unitCode.Substring(0, 6))
                            select rpt).AsQueryable();
                }
                else // 北京乡级查询自己
                {
                    rpts = (from rpt in rpts
                            where rpt.UnitCode == unitCode
                            select rpt).AsQueryable();
                }
            }
            DateTime yearStartDate = Convert.ToDateTime(year.ToString() + "-01-01 00:00:00");//转换成完整的日期格式
            DateTime yearEndDate = Convert.ToDateTime(year.ToString() + "-12-31 23:59:59");//yearStartDate与yearEndDate组成一年的一个区间，查找出这一年的数据（因为暂时没有解决like的问题）
            rpts = rpts.Where(t => t.StartDateTime >= yearStartDate && t.EndDateTime <= yearEndDate);

            if (typeLimit == 0)//本级库
            {
                rpts = rpts.Where(t => t.State == 0 || t.State == 3 || t.State == 4);
                if (!unitCode.StartsWith("33"))//浙江，对报送的报表复制之后，依旧显示已经报送的报表
                {
                    rpts = rpts.Where(t => t.CopyPageNO == 0);
                }

            }
            else //下级库
            {
                rpts = rpts.Where(t => t.ReceiveState == 2 && t.State == 3);

            }
            if (nextUnitCode != null && nextUnitCode != "")
            {
                rpts = rpts.Where(t => t.UnitCode == nextUnitCode);

            }
            if (typeTable != "" && typeTable != null)
            {
                rpts = rpts.Where(t => t.ORD_Code == typeTable);

            }
            if (tableName != "" && tableName != null)
            {
                rpts = rpts.Where(t => t.RPTType_Code == tableName);

            }
            if (typeDate != "" && typeDate != null)
            {
                decimal cycType = Convert.ToDecimal(typeDate);
                rpts = rpts.Where(t => t.StatisticalCycType == cycType);

            }

            if (lastDate != "")
            {
                DateTime recTime = Convert.ToDateTime(lastDate);
                rpts = rpts.Where(t => t.ReceiveTime >= recTime);

            }

            var rptList = from rpt in rpts
                          orderby rpt.SendTime descending
                          select new
                           {
                               rpt.PageNO,
                               rpt.UnitCode,
                               rpt.StartDateTime,
                               rpt.EndDateTime,
                               rpt.StatisticalCycType,
                               rpt.SourceType,
                               rpt.State,
                               rpt.SendOperType,
                               rpt.LastUpdateTime,
                               rpt.SendTime,
                               rpt.WriterTime
                           };
            ArrayList arrList = new ArrayList();
            IList<int?> localAggList = null;
            IList<int?> upperAggList = null;


            Entities getEntity = new Entities();
            BusinessEntities upEntities = null;
            if (typeLimit == 0)  //本级
            {
                /*if (limit > 2)
                {
                    upEntities = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit - 1);
                    huiZongAggList = upEntities.AggAccRecord.Where(t => t.OperateType == 1).Select(t => t.SPageNO).Distinct().ToList();//汇总
                }
                else
                {
                    huiZongAggList = new List<int?>();
                }
                leiJiAggList = busEntity.AggAccRecord.Where(t => t.OperateType == 2).Select(t => t.SPageNO).Distinct().ToList();//累计*/
                upEntities = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);  //不管是否报送先从本级库中查SPageNO是否被累计过
                localAggList = busEntity.AggAccRecord.Where(t => t.OperateType == 2).Select(t => t.SPageNO).Distinct().ToList();
                if (limit > 2)  //省级无法查看国家防总的AggAcc表
                {
                    upEntities = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(limit - 1);  //查看已报送的表是否被上级汇总过
                    upperAggList = busEntity.AggAccRecord.Where(t => t.OperateType == 1).Select(t => t.SPageNO).Distinct().ToList();
                }
            }
            else  //下级
            {
                upEntities = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit); //下级表是否被本级汇总过
                upperAggList = upEntities.AggAccRecord.Where(t => t.OperateType == 1).Select(t => t.SPageNO).Distinct().ToList();//汇总
            }
            object[] temp = { };
            foreach (var obj in rptList)
            {
                temp = new object[12];
                temp[0] = obj.PageNO;
                temp[1] = obj.UnitCode;
                temp[2] = obj.StartDateTime.ToString();
                temp[3] = obj.EndDateTime.ToString();
                temp[4] = obj.StatisticalCycType;
                temp[5] = obj.SourceType;
                temp[6] = obj.State;
                temp[7] = obj.SendOperType;
                temp[8] = obj.LastUpdateTime;
                temp[9] = obj.SendTime;
                //报表是否被自己本级累计过（不管是否已报送，特别注意：我的报表已使用过，那表示一定是因为被累计而纯在AggAcc表的SPageno里面），
                //如果已报送的话，要看上级库中Aggacc表中看该表是否被汇总过（注意：下级表只能被上级汇总才会出现在上级的AggAcc表里面）
                if (localAggList != null && localAggList.Contains(obj.PageNO) || Convert.ToInt32(obj.State) == 3 && upperAggList != null && upperAggList.Contains(obj.PageNO))
                {
                    temp[10] = "true";
                }
                else
                {
                    temp[10] = "false";
                }
                temp[11] = Convert.ToDateTime(obj.WriterTime).ToString("M月d日 HH:mm");
                arrList.Add(temp);
            }
            return arrList;
        }

        /// <summary>
        /// 封装月份值标签栏及下级标签栏
        /// </summary>
        /// <param name="list">数据集合</param>
        /// <param name="typelimit">查看本级(0)表或者下级(1)表</param>
        /// <returns></returns>
        public string GetMonthData(IList list, int typelimit)
        {
            string jsonStr = "";
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Object[] objData = (Object[])list[i];
                    jsonStr += "{" + GetMonthData(objData, typelimit) + "},";
                }
                jsonStr = jsonStr.Remove(jsonStr.Length - 1);
            }

            return jsonStr;
        }
        /// <summary>
        /// 生成底层标签栏
        /// </summary>
        /// <param name="objData">数据data</param>
        /// <param name="typeLimit">查看本级(0)表或者下级(1)表</param>
        /// <returns></returns>
        public string GetMonthData(object[] objData, int typeLimit)
        {
            StringBuilder sb = new StringBuilder();
            string typeName = GetSourceType(Convert.ToInt32(objData[5]));
            DateTime dt1 = Convert.ToDateTime(objData[2]);
            DateTime dt2 = Convert.ToDateTime(objData[3]);
            string stateName = "";//是否上报
            string operTypeName = "";//上报类型

            if (objData.Length != 0 && typeLimit != 1)
            {
                if (Convert.ToInt32(objData[6]) == 3)//如果已经报送，判断报送类型
                {
                    //operTypeName = GetOperType(Convert.ToInt32(objData[7]));
                    operTypeName = "已报送";
                }
                else
                {
                    stateName = GetStateName(Convert.ToInt32(objData[6]));
                }
            }
            if (typeLimit == 1)//增加下级表的报表单位
            {
                string unitName = "未知";
                if (objData[1] != null)
                {
                    unitName = GetUnitNameByUnitCode(objData[1].ToString());
                }
                sb.Append("name:'[").Append(unitName).Append("][").Append(typeName).Append("]");
            }
            else
            {
                sb.Append("name:'[").Append(typeName).Append("]");
            }
            //sb.Append("name:'[").Append(typeName).Append("]");

            if (operTypeName != "")
            {
                sb.Append("[").Append(operTypeName).Append("]");
            }
            if (stateName != "")
            {
                sb.Append("[").Append(stateName).Append("]");
            }

            /*UrgeReport refuse = (UrgeReport)busEntity.UrgeReport.Where(t => t.UrgeRptPersonName == objData[0]);
            if (refuse.Count() > 0)
            {
                sb.Append("[拒收]");
            }*/
            string unitcode = HttpContext.Current.Request["unitcode"];
            string ord_code = HttpContext.Current.Request["ord_code"];
            if (unitcode != null && unitcode.Contains("15") && ord_code != null && ord_code.ToLower() == "sh01") //内蒙古山洪报表数只显示填报时期
            {
                sb.Append(objData[11].ToString());
            }
            else
            {
                sb.Append(dt1.Month).Append("月");
                sb.Append(dt1.Day).Append("日");
                sb.Append("-");
                if (dt1.Month == dt2.Month)
                {
                    sb.Append(dt2.Day).Append("日");
                }
                else
                {
                    sb.Append(dt2.Month).Append("月");
                    sb.Append(dt2.Day).Append("日");
                }
            }
            sb.Append("',");
            //sb.Append("nocheck:'true',");
            sb.Append("id:'").Append(objData[0]).Append("',");
            /*if (refuse.Count() > 0)
            {
                sb.Append("refuse:'").Append(refuse.UrgeRptContent.Split(new string[] { "&&" }, StringSplitOptions.None)[0]).Append("',");
            }*/
            sb.Append("UnitCode:'").Append(objData[1]).Append("',");
            sb.Append("StartDate:'").Append(Convert.ToDateTime(objData[2]).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo)).Append("',");
            sb.Append("EndDate:'").Append(Convert.ToDateTime(objData[3]).ToString("yyyy年MM月dd日", DateTimeFormatInfo.InvariantInfo)).Append("',");
            sb.Append("iconSkin:'c").Append(Convert.ToInt32(objData[4]).ToString()).Append("',");
            sb.Append("SourceType:'").Append(Convert.ToInt32(objData[5])).Append("',");
            try
            {
                sb.Append("State:'").Append(Convert.ToInt32(objData[6])).Append("',");
            }
            catch (Exception)
            {
                sb.Append("State:'").Append(objData[6]).Append("',");
            }
            sb.Append("SendOperType:'").Append(Convert.ToInt32(objData[7])).Append("',");
            if (typeLimit == 1)
            {
                sb.Append("title:'报送时间：").Append(objData[9]).Append("',");
            }
            else
            {
                sb.Append("title:'最后更新时间：").Append(objData[8]).Append("',");
            }
            sb.Append("is_used:").Append(objData[10]).Append(",");

            return sb.ToString();
        }
        /// <summary>
        /// 根据SourceType数字，判断报表的来源类型
        /// </summary>
        /// <param name="TypeNumber">来源数字</param>
        /// <returns>来源类型具体的名称</returns>
        public string GetSourceType(int TypeNumber)
        {
            string typeName = "";
            switch (TypeNumber)
            {
                case 0://录入
                    typeName = "录入";
                    break;
                case 1://汇总
                    typeName = "汇总";
                    break;
                default://累计
                    typeName = "累计";
                    break;
            }
            return typeName;
        }

        /// <summary>
        /// 根据SendOperType值判断上报类型为初报、续报还是核报
        /// </summary>
        /// <param name="SendOperType">上报类型值</param>
        /// <returns></returns>
        public string GetOperType(int SendOperType)
        {
            string OperTypeName = "";
            if (SendOperType == 0)
            {
                OperTypeName = "初报";
            }
            else if (SendOperType == 1)
            {
                OperTypeName = "续报";
            }
            else if (SendOperType == 2)
            {
                OperTypeName = "核报";
            }
            else
            {
                OperTypeName = "未知报送";
            }
            return OperTypeName;
        }


        /// <summary> 根据单位代码，获取单位名称
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <returns></returns>
        public string GetUnitNameByUnitCode(string unitCode)
        {
            string unitName = "";
            if (units.ContainsKey(unitCode))
            {
                unitName = units[unitCode].ToString();
            }
            else
            {
                unitName = "未知";
            }
            return unitName;
        }

        /// <summary>
        /// 根据状态值判断是否已经报送
        /// </summary>
        /// <param name="stateNum">State值</param>
        /// <returns></returns>
        public string GetStateName(int stateNum)
        {
            string stateName = "";
            if (System.Web.HttpContext.Current.Request["rptClass"] == "NP01")
            {
                stateName = "";
            }
            else
            {
                if (stateNum == 3 || stateNum == 4)  //State = 4，显示未报送，报表不包送
                {
                    stateName = "已报送";
                }
                else
                {
                    stateName = "未报送";
                }
            }
            return stateName;
        }

        /// <summary>
        /// 获取锁定表页号
        /// </summary>
        /// <param name="list"></param>
        /// <param name="limit"></param>
        /// <param name="typeLimit"></param>
        /// <param name="pageNOIndex"></param>
        /// <param name="stateIndex"></param>
        /// <returns></returns>
        public ArrayList GetPageNO(IList list, int limit, int typeLimit, int pageNOIndex, int stateIndex)
        {

            ArrayList FoundPageNos = new ArrayList();
            int state = typeLimit == 0 ? 0 : 3;
            for (int i = 0; i < list.Count; i++)
            {
                object[] obj = (object[])list[i];
                if (Convert.ToInt32(obj[stateIndex]) == state)
                {
                    FoundPageNos.Add(obj[pageNOIndex]);
                }
            }

            if (FoundPageNos.Count > 0)
            {
                string pagenoList = "";
                for (int i = 0; i < FoundPageNos.Count; i++)
                {
                    if (i == 0)
                    {
                        pagenoList += FoundPageNos[i].ToString();
                    }
                    else
                    {
                        pagenoList += "," + FoundPageNos[i].ToString();
                    }
                }
                var aggAccs = busEntity.AggAccRecord.Where("it.SPageNO in {" + pagenoList + "}");
                var aggs = (from agg in aggAccs
                            select new { agg.SPageNO }).Distinct();

                //IQuery query = iSession.CreateQuery("select distinct(SPageNO) from AggAccRecord where SPageNO in(" + pagenoList + ")");
                //list = query.List();

                FoundPageNos.Clear();
                foreach (var obj in aggs)
                {
                    FoundPageNos.Add((int)obj.SPageNO);
                }
                //for (int i = 0; i < list.Count; i++)
                //{
                //    FoundPageNos.Add((int)list[i]);
                //}
            }

            return FoundPageNos.Count > 0 ? FoundPageNos : null;
        }

        /// <summary>
        /// 替换锁定的ID
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonStr"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public string ReplaceLockedID(string key, string jsonStr, ArrayList arr)
        {
            if (arr != null)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    jsonStr = jsonStr.Replace(key + ":'" + arr[i] + "'", key + ":'" + arr[i] + "',isLocked:true");
                }
            }
            return jsonStr;
        }
    }
}
