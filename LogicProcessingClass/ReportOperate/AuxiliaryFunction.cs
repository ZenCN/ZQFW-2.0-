using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using DBHelper;
using EntityModel;

namespace LogicProcessingClass.ReportOperate
{
    public class AuxiliaryFunction
    {
        /// <summary>
        /// 把报送的未读信息存入application中
        /// </summary>
        public static void ReadMsgFillInApplicaion()
        {
            System.Web.HttpApplicationState HAS = HttpContext.Current.Application;
            Hashtable Table = new Hashtable();
            foreach (string limit in (new string[] { "2", "3", "4" }))
            {
                Dictionary<string, Dictionary<string, int>> MsgDic = new Dictionary<string, Dictionary<string, int>>();
                Dictionary<string, int> SendMsgDetails = new Dictionary<string, int>(); ;
                BusinessEntities busEntity = Persistence.GetDbEntities(int.Parse(limit));
                var urgeRpts = from urgeRpt in busEntity.UrgeReport
                               where urgeRpt.MsgType == 1
                               select new
                               {
                                   urgeRpt.ReceiveUnitCode,
                                   urgeRpt.SendUnitName
                               };
                string ReceiveUnitCode = "";
                string SendUnitName = "";
                foreach(var obj in urgeRpts)
                {
                    ReceiveUnitCode = obj.ReceiveUnitCode.ToString().Trim();
                    SendUnitName = obj.SendUnitName.ToString().Trim();
                    if (MsgDic.ContainsKey(ReceiveUnitCode))
                    {
                        SendMsgDetails = (Dictionary<string, int>)MsgDic[ReceiveUnitCode];
                        if (SendMsgDetails.ContainsKey(SendUnitName))
                        {
                            SendMsgDetails[SendUnitName] = SendMsgDetails[SendUnitName] + 1;
                        }
                        else
                        {
                            SendMsgDetails[SendUnitName] = 1;
                        }
                    }
                    else
                    {
                        SendMsgDetails[SendUnitName] = 1;
                        MsgDic[ReceiveUnitCode] = SendMsgDetails;
                    }
                }
                Table[limit] = MsgDic;
                busEntity.Dispose();
            }
            HAS["SendMsg"] = Table;
        }

        /// <summary>
        /// 从数据库中删除已读且MsgType = 1的记录
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static string RemoveFormDB(string unitCode, int limit)
        {
            string Result = "";
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            var urgeRpts = busEntity.UrgeReport.Where(t => t.ReceiveUnitCode == unitCode && t.MsgType == 1);
            try
            {
                foreach (var obj in urgeRpts)
                {
                    busEntity.UrgeReport.DeleteObject(obj);
                }
                busEntity.SaveChanges();
                Result = "1";                
            }
            catch (Exception ex)
            {
                Result ="错误消息："+ ex.Message;
               
            }
            return Result;
        }
    }
}
