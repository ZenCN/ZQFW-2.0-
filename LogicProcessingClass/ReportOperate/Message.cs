using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBHelper;
using EntityModel;

namespace LogicProcessingClass.ReportOperate
{
    public class Message
    {
        public static void ReadMsgFillInApplicaion()
        {
            Entities getEntity = new Entities();
            BusinessEntities busEntities = null;
            System.Web.HttpApplicationState HAS = HttpContext.Current.Application;
            Dictionary<string, Hashtable> Msg = new Dictionary<string, Hashtable>();

            foreach (int msgType in (new int[] {1, 2}))  //1表示发送类型的消息；2表示拒收类型的消息
            {
                Hashtable Table = new Hashtable();
                foreach (int limit in (new int[] {0, 2, 3, 4, 5}))
                {
                    Dictionary<string, Dictionary<string, string>> MsgDic =
                        new Dictionary<string, Dictionary<string, string>>();
                    Dictionary<string, string> SendMsgDetails = new Dictionary<string, string>();

                    busEntities = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(limit);
                    var urges = busEntities.UrgeReport.Where(t => t.MsgType == msgType && t.IsRead == 0).ToList();
                    string ReceiveUnitCode = "";
                    string SendUnitName = "";
                    for (int i = 0; i < urges.Count; i++)
                    {
                        ReceiveUnitCode = urges[i].ReceiveUnitCode.ToString().Trim();
                        SendUnitName = urges[i].SendUnitName.ToString().Trim();
                        if (MsgDic.ContainsKey(ReceiveUnitCode))
                        {
                            SendMsgDetails = (Dictionary<string, string>)MsgDic[ReceiveUnitCode];
                            if (msgType == 2)
                            {
                                if (SendMsgDetails.ContainsKey(SendUnitName))
                                {
                                    SendMsgDetails[SendUnitName] = SendMsgDetails[SendUnitName] +
                                                                   urges[i].UrgeRptContent + "||";
                                }
                                else
                                {
                                    SendMsgDetails[SendUnitName] = urges[i].UrgeRptContent + "||";
                                }

                                if (i == urges.Count - 1)
                                {
                                    SendMsgDetails[SendUnitName] =
                                        SendMsgDetails[SendUnitName].Remove(SendMsgDetails[SendUnitName].Length - 2);
                                }
                            }
                            else
                            {
                                if (SendMsgDetails.ContainsKey(SendUnitName))
                                {
                                    SendMsgDetails[SendUnitName] = (int.Parse(SendMsgDetails[SendUnitName]) + 1).ToString();
                                }
                                else
                                {
                                    SendMsgDetails[SendUnitName] = "1";
                                }   
                            }
                        }
                        else
                        {
                            if (msgType == 2)
                            {
                                SendMsgDetails[SendUnitName] = urges[i].UrgeRptContent + "||";
                            }
                            else
                            {
                                SendMsgDetails[SendUnitName] = "1";
                            }
                            MsgDic[ReceiveUnitCode] = SendMsgDetails;
                        }
                    }
                    Table[limit.ToString()] = MsgDic;
                }

                Msg.Add(msgType.ToString(), Table);
            }

            HAS["Message"] = Msg;

            /*Entities entities = new Entities();
            BusinessEntities busEntity = null;
            UrgeReport urgeReport = null;

            foreach (int limit in (new int[] { 3, 4 }))
            {
                busEntity = (BusinessEntities) entities.GetPersistenceEntityByLevel(limit);
                (UrgeReport)busEntity.UrgeReport.Where(msg => msg.MsgType == 1 || msg.MsgType == 2);
                urgeReport.IsRead = 1;
            }

            busEntity.SaveChanges();*/
        }

        public static string RemoveFormDB(string unitcode, int limit, int msgType)
        {
            string Result = "";
            Entities getEntity = new Entities();
            BusinessEntities busEntities = (BusinessEntities) getEntity.GetPersistenceEntityByLevel(limit);
            try
            {
                var urges = busEntities.UrgeReport.Where(t => t.ReceiveUnitCode == unitcode && t.MsgType == msgType).ToList();
                if (urges.Count > 0)
                {
                    for (int i = 0; i < urges.Count; i++)
                    {
                        busEntities.UrgeReport.DeleteObject(urges[i]);
                    }
                    busEntities.SaveChanges();
                    Result = "1";
                }
            }
            catch (Exception ex)
            {
                Result = ex.Message;
            }

            return Result;
        }

        public static string Clear(string unitcode, int limit, string msgType)
        {
            string result = "0";
            System.Web.HttpApplicationState HAS = HttpContext.Current.Application;
            if (HAS["Message"] != null)
            {
                var Message = ((Dictionary<string, Hashtable>) HAS["Message"])[msgType];
                if (Message != null)
                {
                    //Hashtable HSTable = (Hashtable)Message;
                    Dictionary<string, Dictionary<string, string>> MsgDic =
                        (Dictionary<string, Dictionary<string, string>>)Message[limit.ToString()];
                    if (MsgDic.ContainsKey(unitcode))
                    {
                        MsgDic.Remove(unitcode);
                        result = "1";
                    }
                }
            }
            return result;
        }

        public static string Read(string receiveUnitcode, int receiveLimit)
        {
            string Result = "";
            string tmp = "";
            int MsgCount = 0;
            bool Clear = true;
            ArrayList Keys = null;
            Hashtable Message = null;
            System.Web.HttpApplicationState HAS = HttpContext.Current.Application;
            Dictionary<string, Dictionary<string, string>> MsgDic = null;
            Dictionary<string, string> SendMsgDetails = null;

            foreach (string msgType in (new string[] {"1", "2"}))
            {
                tmp = "";
                if (HAS["Message"] != null)
                {
                    Message = ((Dictionary<string, Hashtable>) HAS["Message"])[msgType];
                    if (Message != null)
                    {
                        MsgDic = (Dictionary<string, Dictionary<string, string>>)Message[receiveLimit.ToString()];
                        if (MsgDic.ContainsKey(receiveUnitcode))
                        {
                            SendMsgDetails = (Dictionary<string, string>)MsgDic[receiveUnitcode];
                            if (SendMsgDetails.Count > 0)
                            {
                                MsgCount = 0;
                                Keys = new ArrayList();
                                if (SendMsgDetails.Count == 1 && !SendMsgDetails.ContainsKey(receiveUnitcode) ||
                                    SendMsgDetails.Count > 1)
                                {
                                    tmp = "Details:[";
                                    foreach (KeyValuePair<string, string> DE in SendMsgDetails)
                                    {
                                        if (DE.Key != receiveUnitcode)
                                        {
                                            if (msgType == "2")
                                            {
                                                tmp += "{SendUnitName:'" + DE.Key + "',Info:'" + DE.Value + "'},";
                                            }
                                            else
                                            {
                                                tmp += "{SendUnitName:'" + DE.Key + "',Count:" + DE.Value + "},";
                                                MsgCount = MsgCount + int.Parse(DE.Value);
                                            }
                                            Keys.Add(DE.Key);
                                        }
                                    }
                                    tmp = tmp.Remove(tmp.Length - 1) + "]";
                                    foreach (object Key in Keys)
                                    {
                                        SendMsgDetails.Remove(Key.ToString());
                                    }
                                }
                                if (tmp.Length > 0)
                                {
                                    tmp += ",";
                                }
                                else
                                {
                                    tmp += "Details:[],";
                                }
                                if (SendMsgDetails.ContainsKey(receiveUnitcode))
                                {
                                    tmp += "Count:" + (int.Parse(SendMsgDetails[receiveUnitcode]) + MsgCount);
                                    SendMsgDetails[receiveUnitcode] = (int.Parse(SendMsgDetails[receiveUnitcode]) + MsgCount).ToString();
                                }
                                else
                                {
                                    tmp += "Count:" + MsgCount;
                                    SendMsgDetails[receiveUnitcode] = MsgCount.ToString();
                                }
                                MsgDic[receiveUnitcode] = SendMsgDetails;
                                if (tmp.Length > 0)
                                {
                                    if (msgType == "1")
                                    {
                                        Result = "Send:{" + tmp + "}";
                                    }
                                    else
                                    {
                                        Result += ",Refuse:{" + tmp + "}";
                                    }
                                }
                            }
                        }
                    }
                }
                if (tmp == "")
                {
                    if (msgType == "1")
                    {
                        Result = "Send:{Details:[],Count:0}";
                    }
                    else
                    {
                        Result += ",Refuse:{Details:[],Count:0}";
                        Clear = false;
                    }
                }
            }

            Result = "{" + Result + "}";

            if (Clear)
            {
                LogicProcessingClass.ReportOperate.Message.Clear(receiveUnitcode, receiveLimit, "2"); // 清除拒收消息
            }

            return Result;
        }


        public static void Record(string receiveUnitCode, int receiveLimit, string sendUnitName, string msgType, string other_str)
        {
            System.Web.HttpApplicationState HAS = HttpContext.Current.Application;
            if (HAS["Message"] == null)
            {
                LogicProcessingClass.ReportOperate.Message.ReadMsgFillInApplicaion();
            }
            var Message = ((Dictionary<string, Hashtable>)HAS["Message"])[msgType];
            Dictionary<string, Dictionary<string, string>> MsgDic = null;
            Dictionary<string, string> SendMsgDetails = null;
            if (Message == null)
            {
                LogicProcessingClass.ReportOperate.Message.ReadMsgFillInApplicaion();
                Message = ((Dictionary<string, Hashtable>)HAS["Message"])[msgType];
            }
            if (Message[receiveLimit.ToString()] == null)
            {
                MsgDic = new Dictionary<string, Dictionary<string, string>>();
                SendMsgDetails = new Dictionary<string, string>();

                if (msgType == "2")  //拒收消息
                {
                    SendMsgDetails[sendUnitName] = other_str;
                }
                else
                {
                    SendMsgDetails[sendUnitName] = "1";
                }


                MsgDic[receiveUnitCode] = SendMsgDetails;
            }
            else
            {
                MsgDic = (Dictionary<string, Dictionary<string, string>>)Message[receiveLimit.ToString()];
                if (MsgDic.ContainsKey(receiveUnitCode)) //如果以前也有信息。
                {
                    SendMsgDetails = (Dictionary<string, string>)MsgDic[receiveUnitCode];

                    if (msgType == "2") //拒收消息
                    {
                        if (SendMsgDetails.ContainsKey(sendUnitName))
                        {
                            SendMsgDetails[sendUnitName] = SendMsgDetails[sendUnitName] + "||" + other_str;
                        }
                        else
                        {
                            SendMsgDetails[sendUnitName] = other_str;
                        }
                    }
                    else
                    {
                        if (SendMsgDetails.ContainsKey(sendUnitName))
                        {
                            SendMsgDetails[sendUnitName] = (int.Parse(SendMsgDetails[sendUnitName]) + 1).ToString();
                        }
                        else
                        {
                            SendMsgDetails[sendUnitName] = "1";
                        }   
                    }
                }
                else //用户点击了“接收”，或者以前没有数据
                {
                    SendMsgDetails = new Dictionary<string, string>();
                    if (msgType == "2") //拒收消息
                    {
                        SendMsgDetails[sendUnitName] = other_str;
                    }
                    else
                    {
                        SendMsgDetails[sendUnitName] = "1";   
                    }
                }
                MsgDic[receiveUnitCode] = SendMsgDetails;
            }
        }
    }
}

