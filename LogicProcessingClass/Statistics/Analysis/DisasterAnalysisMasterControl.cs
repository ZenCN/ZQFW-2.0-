using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicProcessingClass.AuxiliaryClass;
using EntityModel;
using DBHelper;
using System.Web.Script.Serialization;
namespace LogicProcessingClass.Statistics
{
    public class DisasterAnalysisMasterControl
    {
       /// <summary>
       /// 灾情类型
       /// </summary>
       /// <returns></returns>
        public string TypeNumberOrDisasterType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[{name:'受灾人口',value:'SZRK'},");//HL011
            sb.Append("{name:'受灾面积',value:'SHMJXJ'},");//HL011
            sb.Append("{name:'成灾面积',value:'CZMJXJ'},");//HL011
            sb.Append("{name:'绝收面积',value:'JSMJXJ'},");//HL011
            sb.Append("{name:'死亡人口',value:'SWRK'},");//HL011
            sb.Append("{name:'转移人口',value:'ZYRK'},");//HL011
            sb.Append("{name:'倒塌房屋',value:'DTFW'},");//HL011
            sb.Append("{name:'直接经济总损失',value:'ZJJJZSS'},");//HL011
            sb.Append("{name:'水利损失',value:'SLSSZJJJSS'}]");//HL011
            return sb.ToString();
        }

        /// <summary>第一次进入灾情分析模块，获取初始数据
        /// 
        /// </summary>
        /// <param name="beginYear">开始年份</param>
        /// <param name="endYear">结束年份</param>
        /// <param name="beginMonth">开始月份</param>
        /// <param name="beginDay">开始日期</param>
        /// <param name="bDateRange">开始日期范围（单位天）</param>
        /// <param name="endMonth">结束年份</param>
        /// <param name="endDay">结束日期</param>
        /// <param name="eDateRange">结束日期范围（天）</param>
        /// <param name="cycTypes">报表时段类型</param>
        /// <param name="pageNumDataList">数据的当前页码</param>
        /// <param name="pageLineNumDataList">数据的每页显示行数</param>
        /// <param name="pageNumTrueNode">树形菜单的当前页码</param>
        /// <param name="pageLineNumTrueNode">树形菜单每页显示行数</param>
        /// <param name="unitCode">行政单位代码</param>
        /// <param name="level">行政单位级别</param>
        /// <returns>初始化数据</returns>
        public object GetDisasterAnalysisData(int beginYear, int endYear, string beginMonth, string beginDay, string bDateRange,
            string endMonth, string endDay, string eDateRange, string cycTypes, int pageNumDataList, int pageLineNumDataList,
                                              int pageNumTrueNode, int pageLineNumTrueNode, string unitCode, int level)
        {
            DisasterAnalysis da = new DisasterAnalysis();
            BusinessEntities businessFXPRV = Persistence.GetDbEntities(level);
            FXDICTEntities fxdict = new FXDICTEntities();
            StringBuilder sb = new StringBuilder();
           
            string tnzhd = TypeNumberOrDisasterType();//获取洪涝的灾情指标
            

            OperateApplication oa = new OperateApplication();
            string unitNameJson = "";//单位JSON
            //string tableTypeJson = "";//报表类型JSON
            string disasterTypeJson = "";//灾害类型JSON
            if (oa.JudgeAppcalitionName("DisasterAnalysisUnitNameJson"))//如果缓存没有，就去读数据库，并把数据存入缓存
            {
                unitNameJson = oa.GetApplication("DisasterAnalysisUnitNameJson").ToString();
            }
            else
            {
                unitNameJson = GetDW(fxdict,unitCode);//获得目前使用行政单位的所有下级单位集合


                oa.SetApplication("DisasterAnalysisUnitNameJson", unitNameJson);
            }
            //if (oa.JudgeAppcalitionName("DisasterAnalysisTableTypeJson"))//如果缓存没有，就去读数据库，并把数据存入缓存
            //{
            //    tableTypeJson = oa.GetApplication("DisasterAnalysisTableTypeJson").ToString();
            //}
            //else
            //{
                
            //    oa.SetApplication("DisasterAnalysisTableTypeJson", tableTypeJson);
            //}
            if (oa.JudgeAppcalitionName("DisasterAnalysisDisasterTypeJson"))//如果缓存没有，就去读数据库，并把数据存入缓存
            {
                disasterTypeJson = oa.GetApplication("DisasterAnalysisDisasterTypeJson").ToString();
            }
            else
            {
                
                disasterTypeJson = TypeNumberOrDisasterType();
                oa.SetApplication("DisasterAnalysisDisasterTypeJson", disasterTypeJson);
            }
            List<string> dataListJson = da.DisasterAnalysisConditionQuery(beginYear, endYear, beginMonth, beginDay, bDateRange,
             endMonth, endDay, eDateRange, cycTypes, pageNumDataList, pageLineNumDataList, businessFXPRV, fxdict);//查询出默认时间段的所有数据,并分页显示
            string trueNodeJson = da.DisasterAnalysisTrueNodeData(pageNumTrueNode, pageLineNumTrueNode, true, businessFXPRV);//树形菜单数据
            var dataSet = new { unitNameJson = unitNameJson,  disasterTypeJson = disasterTypeJson, dataListJson = dataListJson, trueNodeJson = trueNodeJson };
            return dataSet;

        }
        /// <summary>
        /// 获取单位名称和代码
        /// </summary>
        /// <param name="fxdictEntities"></param>
        /// <param name="unitCode">单位代码</param>
        /// <returns>单位名称和代码序列化</returns>
        public string GetDW(FXDICTEntities fxdictEntities, string unitCode)
        {
            var list = from d in fxdictEntities.TB07_District
                       where d.pDistrictCode == unitCode || d.DistrictCode == unitCode
                       select new { d.DistrictName, d.DistrictCode };
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str = serializer.Serialize(list);
            return str;
        }
    }
}
