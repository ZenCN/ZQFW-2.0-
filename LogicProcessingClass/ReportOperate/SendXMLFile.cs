using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using EntityModel;
using DBHelper;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：SendXMLFile.cs
// 文件功能描述：省级将生成的XML文件发送给国家防总等单位
// 修改标识：// 修改描述：

//-------------------------------------------------------------*/
namespace LogicProcessingClass.ReportOperate
{
    public class SendXMLFile
    {
        /// <summary>
        /// 发送XML文件(测试期间，已经是发送给.8和.32数据库)
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="sendUniCode">发送单位的单位代码</param>
        /// <param name="reciveunitcode">接收单位的单位代码</param>
        /// <param name="limit">单位级别</param>
        /// <returns>返回0或1，确认是否成功</returns>
        public int SendReportByXML(int pageNO, string sendUniCode, int limit)
        {
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            string rptTypeCode = FindRppttypecode(pageNO, limit);
            if (rptTypeCode == null || rptTypeCode == "")//如果上报类型为空，则返回0上报失败
            {
                return 0;
            }
            string reciveUnitCode = FindReciveUnitcode(rptTypeCode);//如果接收单位为空，则返回0上报失败
            if (reciveUnitCode == null || reciveUnitCode == "")
            {
                return 0;
            }
            CreateXML crexml = new CreateXML();
            ArrayList array = crexml.CreateHLXML(pageNO, limit);
            string XMLName = "";
            XMLName = array[0].ToString().Replace("\\\\", "\\");
            array.RemoveAt(0);
            try
            {
                int csPageNO = crexml.SendFile(@XMLName, sendUniCode, reciveUnitCode);////BS的WebService返回的是存入CS库中的页号
                int count = 0;
                //if (csPageNO > 0)//bs版本
                //{
                    //循环提交附件
                    for (int i = 0; i < array.Count; i++)
                    {
                        string fileName = (string)array[i];
                        count += crexml.SendAffix(sendUniCode, fileName);
                    }
                    if ( count == array.Count)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                   
                //}
                //else
                //{
                //    return 0;
                //}
                //if (count>0)
                //{
                //    return 1;
                //}
                //else
                //{
                //    return 0;
                //}
            }
            catch (Exception ex)
            {
                var rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
                rpt.State = 0;
                busEntity.SaveChanges();//报送不成功则把状态State修改成0，未报送
                return 0;
            }
        }

        /// <summary>
        /// 根据pageNO找出上报类别代码
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="limit">单位级别</param>
        /// <returns>rppttypecode返回上报类别代码</returns>
        public string FindRppttypecode(int pageNO, int limit)
        {
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            string rptTypeCode = "";
            var rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
            rptTypeCode = rpt.RPTType_Code;
            busEntity.Dispose();

            return rptTypeCode;
        }

        /// <summary>
        /// 根据上报类别代码找出接收单位代码
        /// </summary>
        /// <param name="rptTypeCode">上报类型代码</param>
        /// <returns>unitcode返回接收文件的单位代码</returns>
        public string FindReciveUnitcode(string rptTypeCode)
        {
            FXDICTEntities fxdict = Persistence.GetDbEntities();
            string unitCode = "";
            var tb11 = fxdict.TB11_RptType.Where(t => t.RptTypeCode == rptTypeCode).SingleOrDefault();
            unitCode = tb11.UnitCode;
            fxdict.Dispose();

            return unitCode;
        }
    }
}
