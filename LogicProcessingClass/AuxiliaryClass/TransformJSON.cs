using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using EntityModel;
using System.Collections;
/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：
    // 文件名：TransformJSON.cs
    // 文件功能描述：转换JSON字符串,序列号对象
    // 创建标识：
    // 修改标识：
    // 修改描述：
//-------------------------------------------------------------*/
namespace LogicProcessingClass.AuxiliaryClass
{
    public class TransformJSON
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        /// <summary>
        /// 序列化ReportTitle对象
        /// </summary>
        /// <param name="rjson">要序列化的ReportTitle对象</param>
        /// <returns>被序列化的JSON数据</returns>
        public string CreateReportTitleJSON(ReportTitle report)
        {
            string jsonstr = jss.Serialize(report);     //序列化report对象
            return jsonstr;
        }
        /// <summary>
        /// 把obj数据转换成JSON格式字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public string CreateObjJSON(object obj)
        {
            return jss.Serialize(obj);
        }

        /// <summary>
        /// 反序列化ReportTitle对象的JSON数据
        /// </summary>
        /// <param name="reportJSON">要反序列化的JSON数据</param>
        /// <returns>被反序列化的ReportTitle对象</returns>
        public ReportTitle GetReportTitleObject(string reportJSON)
        {
            ReportTitle report = (ReportTitle)jss.DeserializeObject(reportJSON);
            return report;
        }

        /// <summary>
        /// 序列化HL011Bean对象
        /// </summary>
        /// <param name="hl011">要序列化的HL011Bean对象</param>
        /// <returns>被序列化的JSON数据</returns>
        public string CreateHL011JSONBean(HL011 hl011)
        {
            string jsonstr = jss.Serialize(hl011);
            return jsonstr;
        }

        /// <summary>
        /// 反序列化HL011Bean对象的JSON数据
        /// </summary>
        /// <param name="hl011JSON">要反序列化的JSON数据</param>
        /// <returns>被反序列化的HL011Bean对象</returns>
        public HL011 GetHL011Object(string hl011JSON)
        {
            HL011 hl011 = (HL011)jss.DeserializeObject(hl011JSON);
            return hl011;
        }

        /// <summary>
        /// 序列化HL012Bean对象
        /// </summary>
        /// <param name="hl012">要序列化的HL012Bean对象</param>
        /// <returns>被序列化的JSON数据</returns>
        public string CreateHL012JSONBean(HL012 hl012)
        {
            string jsonstr = jss.Serialize(hl012);
            return jsonstr;
        }

        /// <summary>
        /// 反序列化HL012Bean对象的JSON数据
        /// </summary>
        /// <param name="hl012JSON">要反序列化的JSON数据</param>
        /// <returns>被反序列化的HL011Bean对象</returns>
        public HL012 Gethl012Object(string hl012JSON)
        {
            HL012 hl012 = (HL012)jss.DeserializeObject(hl012JSON);
            return hl012;
        }

        /// <summary>
        /// 序列化HL013Bean对象
        /// </summary>
        /// <param name="hl013">要序列化的HL013Bean对象</param>
        /// <returns>被序列化的JSON数据</returns>
        public string CreateHL013JSONBean(HL013 hl013)
        {
            string jsonstr = jss.Serialize(hl013);
            return jsonstr;
        }

        /// <summary>
        /// 反序列化HL013Bean对象的JSON数据
        /// </summary>
        /// <param name="hl013JSON">要反序列化的JSON数据</param>
        /// <returns>被反序列化的HL013Bean对象</returns>
        public HL013 GetHL013Object(string hl013JSON)
        {
            HL013 hl013 = (HL013)jss.DeserializeObject(hl013JSON);
            return hl013;
        }

        /// <summary>
        /// 序列化HL014Bean对象
        /// </summary>
        /// <param name="hl014">要序列化的HL014Bean对象</param>
        /// <returns>被序列化的JSON数据</returns>
        public string CreateHL014JSONBean(HL014 hl014)
        {
            string jsonstr = jss.Serialize(hl014);
            return jsonstr;
        }

        /// <summary>
        /// 反序列化HL014Bean对象的JSON数据
        /// </summary>
        /// <param name="hl014JSON">要反序列化的JSON数据</param>
        /// <returns>被反序列化的HL014Bean对象</returns>
        public HL014 GetHL014Object(string hl014JSON)
        {
            HL014 hl014 = (HL014)jss.DeserializeObject(hl014JSON);
            return hl014;
        }

        /// <summary>
        /// 序列化HP011Bean对象
        /// </summary>
        /// <param name="hl014">要序列化的HP011Bean对象</param>
        /// <returns>被序列化的JSON数据</returns>
        public string CreateHP011JSONBean(HP011 hp011)
        {
            string jsonstr = jss.Serialize(hp011);
            return jsonstr;
        }

        /// <summary>
        /// 反序列化HP011Bean对象的JSON数据
        /// </summary>
        /// <param name="hp011JSON">要反序列化的JSON数据</param>
        /// <returns>被反序列化的HL014Bean对象</returns>
        public HP011 GetHP011Object(string hp011JSON)
        {
            HP011 hp011 = (HP011)jss.DeserializeObject(hp011JSON);
            return hp011;
        }



        /// <summary>
        /// 序列化HP012Bean对象
        /// </summary>
        /// <param name="HP012">要序列化的HP012Bean对象</param>
        /// <returns>被序列化的JSON数据</returns>
        public string CreateHP012JSONBean(HP012 hl014)
        {
            string jsonstr = jss.Serialize(hl014);
            return jsonstr;
        }

        /// <summary>
        /// 反序列化HP012Bean对象的JSON数据
        /// </summary>
        /// <param name="hp012JSON">要反序列化的JSON数据</param>
        /// <returns>被反序列化的HL014Bean对象</returns>
        public HP012 GetHP012Object(string hp012JSON)
        {
            HP012 hp012 = (HP012)jss.DeserializeObject(hp012JSON);
            return hp012;
        }

        /// <summary>
        /// 把数组转换成一个Json对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
/*        public string JSON_ArrayToObject(IList list)
        {
            string json = jss.Serialize(list).Replace("\"", "'");
            json = json.Replace(",", ":").Replace("]:", ",").Replace("[", "");
            json = json.Remove(json.Length - 2);
            json = "{" + json + "}";
            return json;
        }*/

        /// <summary>
        /// List转换成Json对象
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string CreateJSON(IList list)
        {
            //return JSON_ArrayToObject(jss.Serialize(list));
            return jss.Serialize(list).Replace("\"", "'");//把所有的双引号替换成单引号
        }
    }
}
