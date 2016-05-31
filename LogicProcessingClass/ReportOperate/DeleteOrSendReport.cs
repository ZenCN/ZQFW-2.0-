using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using DBHelper;
using System.Web;
using System.Collections;
/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：V2.0
    // 文件名：DeleteReport.cs
    // 文件功能描述：删除报表
    // 创建标识：
    // 修改标识：
    // 修改描述：
//-------------------------------------------------------------*/
namespace LogicProcessingClass.ReportOperate
{
    public class DeleteOrSendReport
    {
        private BusinessEntities busEntity = null;
        Entities getEntity = new Entities();
        /// <summary>
        /// 构造函数，根据单位级别初始化对应的业务模型
        /// </summary>
        /// <param name="limit"></param>
        public DeleteOrSendReport(int limit)
        {
            busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
        }
        #region  删除操作
        /// <summary>
        /// 删除报表
        /// </summary>
        /// <param name="pageNO">要删除的报表的页号</param>
        /// <returns>系统返回的消息</returns>
        public string Delete(int pageNO, int State, int limit)
        {
            string message = "";
            string ord_code = HttpContext.Current.Request.Cookies["ord_code"].Value;

            if (ord_code != "NP01")
            {
                int upperCount = 0;
                int count = 0;
                BusinessEntities business = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(limit);

                //不管是否报送先从本级库中查SPageNO是否被累计过
                count = busEntity.AggAccRecord.Where(t => t.OperateType == 2 && t.SPageNO == pageNO).Count();
                if (count == 0 && limit > 2 && State == 3) //已报送
                {
                    business = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(limit - 1); //查看已报送的表是否被上级汇总过
                    count = busEntity.AggAccRecord.Where(t => t.OperateType == 1 && t.SPageNO == pageNO).Count();
                }

                try
                {
                    if (count == 0)
                    {
                        var rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
                        rpt.LastUpdateTime = DateTime.Now;
                        rpt.Del = 1;
                        busEntity.SaveChanges();
                        message = "1";
                    }
                    else
                    {
                        message = "-1";
                    }

                }
                catch (Exception ex)
                {
                    message = "错误消息：" + ex.Message;
                }
            }

            return message;
        }

        /// <summary>
        /// 删除死亡人员信息
        /// </summary>
        /// <param name="tbno">要删除的死亡人员记录的TBNO(主键)</param>
        /// <returns>删除是否成功</returns>
        public string DeleteDeathInfo(string tbnos)
        {

            string message = "";
            var hl012s = busEntity.HL012.Where("it.TBNO in {" + tbnos + "}");
            try
            {
                foreach (var hl012 in hl012s)
                {
                    busEntity.HL012.DeleteObject(hl012);
                }
                busEntity.SaveChanges();
                message = "1";
            }
            catch (Exception ex)
            {
                message = "错误消息：" + ex.Message;
            }
            return message;
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="tbnos"></param>
        /// <returns></returns>
        public string DeleteAffix(string tbnos)
        {
            string message = "";
            var affs = busEntity.Affix.Where("it.TBNO in {" + tbnos + "}");
            try
            {
                foreach (var aff in affs)
                {
                    busEntity.Affix.DeleteObject(aff);
                }
                busEntity.SaveChanges();
                message = "1";
            }
            catch (Exception ex)
            {
                message = "错误消息：" + ex.Message;
            }
            return message;
        }
        #endregion


        #region 报送报表操作
        /// <summary>
        /// 用户上报数据报表
        /// </summary>
        /// <param name="limit">districtClass为单位级别：0国家防总；2省级；3市级；4县级；5乡镇级；</param>
        /// <param name="pageNO">pageNo页号</param>
        /// <returns>返回判定标志flag1代表成功,否则“错误消息：”</returns>
        public string SendReports(int limit, int pageNO, int sendType, string unitCode, string unitName)
        {
            string message = "";
            string a = DateTime.Now.ToString();
            var rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
            rpt.CopyPageNO = 0;//副本字段，默认为0
            rpt.State = 3;//变更表头的状态为3，表示该套报表已经上报
            rpt.ReceiveState = 0;//接收状态字段不变，默认为0 
            rpt.SendOperType = sendType;
            rpt.SendTime = DateTime.Now;
            try
            {
                busEntity.SaveChanges();
                message = "1";

                string upperUnitCode = "00000000";

                if (limit == 3)
                {
                    upperUnitCode = unitCode.Substring(0, 2) + "000000";
                }
                else if (limit == 4)
                {
                    upperUnitCode = unitCode.Substring(0, 4) + "0000";
                }
                else if (limit == 5)
                {
                    upperUnitCode = unitCode.Substring(0, 6) + "00";
                }

                limit = limit == 2 ? 0 : (limit - 1);

                if (HttpContext.Current.Request["ord_code"] == "HL01" || limit == 5)
                {
                    LogicProcessingClass.ReportOperate.Message.Record(upperUnitCode, limit, unitName, "1", "");
                }
            }
            catch (Exception ex)
            {
                message = "错误消息：" + ex.Message;
            }
            return message;
        }
        #endregion

        #region 撤销没有参加汇总的报表
        /// <summary>
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="pageNO"></param>
        /// <returns>1：撤销报送成功，2：该报表已经参加汇总，撤销失败</returns>
        public string UndoReports(int limit, int pageNO)
        {
            string message = "";
            if (limit==2)
            {
                limit = 1;
            }
            BusinessEntities upBusEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit-1);//上级库中进行查询
            var aggs = upBusEntity.AggAccRecord.Where(t => t.SPageNO == pageNO && t.OperateType == 1).AsQueryable();//查询是否参加上级的汇总
            if (!aggs.Any())
            {
                var rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
                rpt.State = 0;//变更表头的状态为0，表示该套报表没有上报
                rpt.ReceiveState = 0;//接收状态字段不变，默认为0 
                rpt.SendOperType = 0;
                rpt.LastUpdateTime = DateTime.Now;
                try
                {
                    busEntity.SaveChanges();
                    message = "1";
                }
                catch (Exception ex)
                {
                    message = "错误消息：" + ex.Message;
                }
            }
            else
            {
                message = "2";//已经参加上级单位的汇总
            }
            
            return message;
        }
        #endregion
    }
}
