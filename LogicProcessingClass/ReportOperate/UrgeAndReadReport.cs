using System;
using System.Linq;
using System.Web;
using DBHelper;
using EntityModel;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：UrgeAndReadReport.cs
// 文件功能描述：催报模块相关操作
// 创建标识：胡汗 2013年12月2日
// 修改标识：

// 修改描述：
//-------------------------------------------------------------*/

namespace LogicProcessingClass.ReportOperate
{
    public class UrgeAndReadReport
    {
        Tools tool = new Tools();
        Entities entities = new Entities();

        /// <summary>
        /// 给单位催报
        /// </summary>
        /// <param name="limit">登陆单位级别</param>
        /// <param name="recieveUnitCodes">接收单位代码</param>
        /// <param name="content">催报内容</param>
        /// <param name="urgeReportPerson">催报人</param>
        /// <param name="urgeReportUnit">催报单位</param>
        public void UrgeReport(int limit, string recieveUnitCodes, string content, string urgeReportPerson, string urgeReportUnit, int msgType, string detials, string pagenos)
        {
            string[] unitCodes = recieveUnitCodes.Split(',');
            string[] detials_arr = null;
            string[] pagenos_arr = null;
            string text = "";
            string personOrPageno = "";
            string unitname = "";
            if (msgType == 2)
            {
                detials_arr = detials.Split(new char[] {','});
                pagenos_arr = pagenos.Split(new char[] { ',' });
                unitname = HttpContext.Current.Request["unitname"];
                if (unitname != null)
                {
                    unitname = HttpUtility.UrlDecode(HttpContext.Current.Request["unitname"]);
                }
                else
                {
                    FXDICTEntities dicEntity = (FXDICTEntities)entities.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
                    unitname = (from tb07 in dicEntity.TB07_District where tb07.DistrictCode == urgeReportUnit select tb07.DistrictName).First();
                }
            }
            BusinessEntities busEntityCTY = (BusinessEntities)entities.GetPersistenceEntityByLevel(3);
            BusinessEntities busEntityCNT = (BusinessEntities)entities.GetPersistenceEntityByLevel(4);
            BusinessEntities busEntityTWN = (BusinessEntities)entities.GetPersistenceEntityByLevel(5);
            #region 登陆的单位是省级
            if (limit == 2)
            {
                for (int i = 0; i < unitCodes.Length; i++)
                {
                    if (msgType == 2)
                    {
                        text = content + "&&" + pagenos_arr[i] + "&&" + detials_arr[i];
                        LogicProcessingClass.ReportOperate.Message.Record(unitCodes[i],
                            tool.GetLevelByUnitCode(unitCodes[i]), unitname, "2", text);
                        personOrPageno = pagenos_arr[i];
                        continue;
                    }
                    else
                    {
                        text = content;
                        personOrPageno = urgeReportPerson;
                    }

                    if (tool.GetLevelByUnitCode(unitCodes[i],"1") == 3)//接收单位是市级
                    {
                        AddUrgeReport(busEntityCTY, unitCodes[i], text, personOrPageno, urgeReportUnit, msgType);
                    }
                    else if (tool.GetLevelByUnitCode(unitCodes[i],"1") == 4)//接收单位是县级
                    {
                        AddUrgeReport(busEntityCNT, unitCodes[i], text, personOrPageno, urgeReportUnit, msgType);
                    }
                    else//接收单位是镇级
                    {
                        AddUrgeReport(busEntityTWN, unitCodes[i], text, personOrPageno, urgeReportUnit, msgType);
                    }
                }
                busEntityCNT.SaveChanges();
                busEntityCTY.SaveChanges();
                busEntityTWN.SaveChanges();
            }
            #endregion
            #region 登陆的单位是市级
            else if (limit == 3)
            {
                for (int i = 0; i < unitCodes.Length; i++)
                {
                    if (msgType == 2)
                    {
                        text = content + "&&" + pagenos_arr[i] + "&&" + detials_arr[i];
                        LogicProcessingClass.ReportOperate.Message.Record(unitCodes[i], tool.GetLevelByUnitCode(unitCodes[i]), unitname, "2", text);
                        personOrPageno = pagenos_arr[i];
                        continue;
                    }
                    else
                    {
                        text = content;
                        personOrPageno = urgeReportPerson;
                    }

                    if (tool.GetLevelByUnitCode(unitCodes[i], "1") == 4)//接收单位是县级
                    {
                        AddUrgeReport(busEntityCNT, unitCodes[i], text, personOrPageno, urgeReportUnit, msgType);
                    }
                    else//接收单位是镇级
                    {
                        AddUrgeReport(busEntityTWN, unitCodes[i], text, personOrPageno, urgeReportUnit, msgType);
                    }
                }
                busEntityCNT.SaveChanges();
                busEntityTWN.SaveChanges();
            }
            #endregion
            #region 登陆的单位是县级
            else
            {
                for (int i = 0; i < unitCodes.Length; i++)
                {
                    if (msgType == 2)
                    {
                        text = content + "&&" + pagenos_arr[i] + "&&" + detials_arr[i];
                        LogicProcessingClass.ReportOperate.Message.Record(unitCodes[i], tool.GetLevelByUnitCode(unitCodes[i]), unitname, "2", text);
                        personOrPageno = pagenos_arr[i];
                        continue;
                    }
                    else
                    {
                        text = content;
                        personOrPageno = urgeReportPerson;
                    }

                    AddUrgeReport(busEntityTWN, unitCodes[i], text, personOrPageno, urgeReportUnit, msgType);
                }
                busEntityTWN.SaveChanges();
            }
            #endregion
        }

        /// <summary>
        /// 阅读上级发送的催报
        /// </summary>
        /// <param name="limit">登陆单位的级别</param>
        /// <param name="tbNO">编号</param>
        public string ReadUrgeReport(int limit, int tbNO)
        {
            string strJson = "";
            BusinessEntities busEntity = (BusinessEntities)entities.GetPersistenceEntityByLevel(limit);
            var urgeReport = busEntity.UrgeReport.Where(urgeRpt => urgeRpt.TBNO == tbNO).First();
            strJson += "{TBNO:'" + urgeReport.TBNO + "',SendUnitName:'" + urgeReport.SendUnitName + "',UrgeRptContent:'" +
                    urgeReport.UrgeRptContent + "',UrgeRptDateTime:'" + urgeReport.UrgeRptDateTime + "',UrgeRptPersonName:'" +
                    urgeReport.UrgeRptPersonName + "'}";
            urgeReport.IsRead = 1;
            busEntity.SaveChanges();
            return strJson;
        }

        /// <summary>
        /// 获得催报列表
        /// </summary>
        /// <param name="limit">登陆单位级别</param>
        /// <param name="unitCode">登陆单位代码</param>
        /// <returns></returns>
        public string GetUrgeReportList(int limit, string unitCode)
        {
            string strJson = "";
            try
            {
                string urgeStr = "urgeReport:[";
                FXDICTEntities dicEntity =
                    (FXDICTEntities)
                        entities.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
                var ucs =
                    (from tb07 in dicEntity.TB07_District where tb07.pDistrictCode == unitCode select tb07.DistrictCode)
                        .ToList();
                BusinessEntities lowerBusEntity = (BusinessEntities) entities.GetPersistenceEntityByLevel(limit);
                var rpts =
                    lowerBusEntity.ReportTitle.Where(rpt => ucs.Contains(rpt.UnitCode))
                        .Where(rpt => rpt.ReceiveState == 0 && rpt.State == 3);
                BusinessEntities busEntity = (BusinessEntities) entities.GetPersistenceEntityByLevel(limit);
                var urgeReports =
                    busEntity.UrgeReport.Where(
                        urgeReport =>
                            urgeReport.ReceiveUnitCode == unitCode && urgeReport.IsRead == 0 && urgeReport.MsgType == 0);
                if (urgeReports.Any())
                {
                    foreach (var urgeReport in urgeReports)
                    {
                        urgeStr += "{TBNO:'" + urgeReport.TBNO + "',SendUnitName:'" + urgeReport.SendUnitName +
                                   "',UrgeRptContent:'" +
                                   urgeReport.UrgeRptContent + "',UrgeRptDateTime:'" + urgeReport.UrgeRptDateTime +
                                   "',UrgeRptPersonName:'" +
                                   urgeReport.UrgeRptPersonName + "'},";
                    }
                    urgeStr = urgeStr.Remove(urgeStr.Length - 1);
                }
                strJson = urgeStr + "]";
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return strJson;
        }

        /// <summary>
        /// 添加单个催报
        /// </summary>
        /// <param name="busEntity"></param>
        /// <param name="receiveUnitCodes">接收单位代码</param>
        /// <param name="content">催报内容</param>
        /// <param name="urgeReportPerson">催报人</param>
        /// <param name="urgeReportUnit">催报单位</param>
        private void AddUrgeReport(BusinessEntities busEntity, string receiveUnitCodes, string content, string urgeReportPerson, string urgeReportUnit, int msgType)
        {
            FXDICTEntities dicEntity = (FXDICTEntities)entities.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
            string unitName = (from tb07 in dicEntity.TB07_District where tb07.DistrictCode == urgeReportUnit select tb07.DistrictName).First();
            UrgeReport urgeReport = busEntity.UrgeReport.CreateObject();
            urgeReport.ReceiveUnitCode = receiveUnitCodes;
            urgeReport.SendUnitCode = urgeReportUnit;
            urgeReport.SendUnitName = unitName;
            urgeReport.UrgeRptContent = content;
            urgeReport.UrgeRptDateTime = DateTime.Now;
            urgeReport.IsRead = 0;
            urgeReport.MsgType = msgType;
            urgeReport.UrgeRptPersonName = urgeReportPerson;
            busEntity.AddToUrgeReport(urgeReport);
        }
    }
}
