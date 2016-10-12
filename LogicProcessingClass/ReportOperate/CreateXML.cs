using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBHelper;
using System.Collections;
using EntityModel;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Web;
/*----------------------------------------------------------------
   // 版本说明：
   // 版本号：
   // 文件名：CreateXML.cs
   // 文件功能描述：省级生成报表XML文件
   // 修改标识：
   // 修改描述：
   //-------------------------------------------------------------*/
using LogicProcessingClass.WS_BS_CS_XML;

namespace LogicProcessingClass.ReportOperate
{
    public class CreateXML
    {

        /// <summary>
        /// 创建XML文件(暂时没有添加蓄水表的)
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="limit">单位等级</param>
        /// <returns>XMLName文件名</returns>
        public ArrayList CreateHLXML(int pageNO, int limit)
        {
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            var rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).SingleOrDefault();
            rpt.State = 3;
            rpt.ReceiveState = 0;
            rpt.CopyPageNO = 0;
            rpt.SendTime = DateTime.Now;
            busEntity.SaveChanges();

            string a = DateTime.Now.ToString();
            var rpts = busEntity.ReportTitle.Where(t => t.PageNO == pageNO);

            //创建根节点元素
            XmlDocument xdoc = new XmlDocument();
            XmlElement xer = xdoc.CreateElement("FXBB2011-TransmitDataList");
            xdoc.AppendChild(xer);

            //创建父节点元素
            XmlElement reptitlist = xdoc.CreateElement("ReportTitle-List");
            xer.AppendChild(reptitlist);
            XmlElement affixlist = xdoc.CreateElement("Affix-List");
            xer.AppendChild(affixlist);
            XmlElement accagglist = xdoc.CreateElement("AggAccRecord-List");
            xer.AppendChild(accagglist);
            XmlElement hl01list = xdoc.CreateElement("HL011-List");
            xer.AppendChild(hl01list);
            XmlElement hl02list = xdoc.CreateElement("HL012-List");
            xer.AppendChild(hl02list);
            XmlElement hl03list = xdoc.CreateElement("HL013-List");
            xer.AppendChild(hl03list);
            XmlElement hl04list = xdoc.CreateElement("HL014-List");
            xer.AppendChild(hl04list);

            //各父节点对应的子节点名称
            string str011 = "HL011";//HL011数据
            string str012 = "HL012";//HL012数据
            string str013 = "HL013";//HL013数据
            string str014 = "HL014";//HL014数据
            string straffix = "Affix";//附件数据
            string straccagg = "AggAccRecord";//汇总下级表数据
            string strreptit = "ReportTitle";//表头数据

            //开始生成XML节点
            foreach (var rptobj in rpts)
            {

                XmlElement reptit = MakeXML(rptobj, xdoc, strreptit, 2);//调用处理XML文件格式和数据的方法
                reptitlist.AppendChild(reptit);//将生成的XML添加到ReportTitle-List这个节点

                foreach (var affobj in rptobj.Affix)
                {
                    XmlElement xeaffix = MakeXML(affobj, xdoc, straffix, 2);
                    affixlist.AppendChild(xeaffix);
                }
                foreach (var aggobj in rptobj.AggAccRecord)
                {
                    XmlElement xeagg = MakeXML(aggobj, xdoc, straccagg, 2);
                    accagglist.AppendChild(xeagg);
                }

                foreach (var hl01obj in rptobj.HL011)
                {
                    XmlElement xe01 = MakeXML(hl01obj, xdoc, str011, 2);
                    hl01list.AppendChild(xe01);
                }

                foreach (var hl02obj in rptobj.HL012)
                {
                    XmlElement xe02 = MakeXML(hl02obj, xdoc, str012, 2);
                    hl02list.AppendChild(xe02);
                }
                foreach (var hl03obj in rptobj.HL013)
                {
                    XmlElement xe03 = MakeXML(hl03obj, xdoc, str013, 2);
                    hl03list.AppendChild(xe03);
                }
                foreach (var hl04obj in rptobj.HL014)
                {
                    XmlElement xe04 = MakeXML(hl04obj, xdoc, str014, 2);
                    hl04list.AppendChild(xe04);
                }
                #region 添加汇总的表（市级）
                if (rptobj.SourceType == 1)//只添加汇总表的下级表
                {
                    string sPageNOs = "";
                    foreach (var aggobj in rptobj.AggAccRecord)
                    {
                        sPageNOs += aggobj.SPageNO + ",";
                    }
                    if (sPageNOs != "")
                    {
                        sPageNOs = sPageNOs.Remove(sPageNOs.Length - 1);
                        //BusinessEntities lowBusEntity = Persistence.GetDbEntities(3);//市级
                        var lowRpts = busEntity.ReportTitle.Where("it.PageNO in {" + sPageNOs + "}").AsQueryable();
                        foreach (var lowRptObj in lowRpts)
                        {

                            XmlElement lowRpt = MakeXML(lowRptObj, xdoc, strreptit, 3);
                            reptitlist.AppendChild(lowRpt);

                            //foreach (var affobj in lowRptObj.Affix)
                            //{
                            //    XmlElement xeaffix = MakeXML(affobj, xdoc, straffix, 3);
                            //    affixlist.AppendChild(xeaffix);
                            //}
                            //foreach (var aggobj in lowRptObj.AggAccRecord)
                            //{
                            //    XmlElement xeagg = MakeXML(aggobj, xdoc, straccagg, 3);
                            //    affixlist.AppendChild(xeagg);
                            //}

                            foreach (var hl01obj in lowRptObj.HL011)
                            {
                                XmlElement xe01 = MakeXML(hl01obj, xdoc, str011, 3);
                                hl01list.AppendChild(xe01);
                            }

                            foreach (var hl02obj in lowRptObj.HL012)
                            {
                                XmlElement xe02 = MakeXML(hl02obj, xdoc, str012, 3);
                                hl02list.AppendChild(xe02);
                            }
                            foreach (var hl03obj in lowRptObj.HL013)
                            {
                                XmlElement xe03 = MakeXML(hl03obj, xdoc, str013, 3);
                                hl03list.AppendChild(xe03);
                            }
                            foreach (var hl04obj in lowRptObj.HL014)
                            {
                                XmlElement xe04 = MakeXML(hl04obj, xdoc, str014, 3);
                                hl04list.AppendChild(xe04);
                            }
                        }
                        //busEntity.Dispose();
                        //lowBusEntity.Dispose();
                    }
                }
                #endregion
            }


            //handerXML(xdoc);//对以上生成好的XML文件进行需要性修改（新的webService对xml格式不一样，不需要进行修改）


            string repinfo = "";
            if (rpts.Count() > 0)
            {
                ReportTitle rep = rpts.First();
                string time = Convert.ToDateTime(rep.SendTime).ToString("yyyyMMddHHmmss");
                int sta = Convert.ToInt32(rep.StatisticalCycType);
                string stati = "";
                if (sta.ToString().Length == 1)
                {
                    stati = "0" + sta.ToString();
                }
                else
                {
                    stati = sta.ToString();
                }
                time = time.Replace("/", "");
                time = time.Replace(":", "");
                time = time.Replace(" ", "");
                //按格式生成XML 文件名：bbyj+2位数报表类型+8位数单位代码+发送时间的年月日时分秒+ORDCode
                //如：bbyj064312000020120611115852hl01.XML
                repinfo = "bbyj" + stati + rep.UnitCode + time + rep.ORD_Code;
                repinfo = repinfo.ToLower();//全部转成小写
            }

            string url = AppDomain.CurrentDomain.BaseDirectory + "XML\\" + repinfo + ".xml";
            xdoc.Save(url);//保存
            ArrayList array = new ArrayList();
            array.Add(url);
            XmlNode affixNode = xer.SelectSingleNode("Affix-List");
            if (affixNode != null)
            {
                foreach (XmlNode node in affixNode.ChildNodes)
                {
                    string fileName = "";
                    foreach (XmlNode file in node.ChildNodes)
                    {
                        if (file.Name == "DownloadURL")
                        {
                            fileName = file.InnerText.ToString().Remove(0, 3);
                        }
                    }
                    array.Add(fileName);
                }
            }
            return array;
        }

        /// <summary>
        /// 传入一个XML，处理XML元素(某些元素不要显示，某些节点要换名称)，再传出
        /// </summary>
        /// <param name="obj">要生成XML元素的对象（HL011-HL014、ReportTitle等）</param>
        /// <param name="doc">XML元素</param>
        /// <param name="docname">元素节点名称</param>
        /// <returns>xer返回处理好的XML对象</returns>
        public XmlElement MakeXML(object obj, XmlDocument doc, string docname, int limit)
        {
            XmlElement xer = doc.CreateElement(docname);//创建一个节点
            XmlElement xe = null;
            PropertyInfo[] pfs = obj.GetType().GetProperties();//利用反射获得类的属性
            bool isAddLimit = false;
            string fields = "associatedpageno,statisticalcyctype,state,sourcetype,operatetype,dbtype";
            for (int i = 0; i < pfs.Length; i++)
            {
                if (pfs[i].GetValue(obj, null) == null)//如果属性值为空则跳出本次循环
                {
                    continue;
                }
                string nametype = pfs[i].PropertyType.FullName;
                if (!isAddLimit)//添加级别
                {
                    xe = doc.CreateElement("Limit");
                    xe.InnerXml = limit.ToString();
                    xer.AppendChild(xe);
                    isAddLimit = true;
                }

                //如果节点的类型不是这几种类型，则跳出循环
                if (!((nametype.IndexOf("System.Int32") != -1)
                       || (nametype.IndexOf("System.String") != -1)
                       || (nametype.IndexOf("System.Double") != -1)
                       || (nametype.IndexOf("System.Decimal") != -1)
                       || (nametype.IndexOf("System.DateTime") != -1)))
                {
                    continue;
                }

                else//否则则根据类的属性名创建元素节点，并赋给相应的值
                {
                    xe = doc.CreateElement(pfs[i].Name);
                    //if (pfs[i].Name.ToLower() != "operatetype")//强制转换成整形
                    if (!fields.Contains(pfs[i].Name.ToLower()))//强制转换成整形
                    {
                        xe.InnerXml = pfs[i].GetValue(obj, null).ToString();
                    }
                    else
                    {
                        xe.InnerXml = pfs[i].GetValue(obj, null).ToString().Split('.')[0].ToString();
                    }
                    
                    xer.AppendChild(xe);
                }

            }
            return xer;//将处理好的XML返回
        }

        /// <summary>
        /// 对生成的XML文件根据个人需要进行修改，为了生成的XML文件主键是从2开始排序

        /// </summary>
        /// <param name="doc">XML对象</param>
        /// <returns>doc返回一个XML文件</returns>
        public XmlDocument handerXML(XmlDocument doc)
        {
            //XML文件的节点路径
            string pathaffix = "/FXBB2011-TransmitDataList/Affix-List/Affix/PageNO";
            string path01 = "/FXBB2011-TransmitDataList/HL011-List/HL011/PageNO";
            string path02 = "/FXBB2011-TransmitDataList/HL012-List/HL012/PageNO";
            string path03 = "/FXBB2011-TransmitDataList/HL013-List/HL013/PageNO";
            string path04 = "/FXBB2011-TransmitDataList/HL014-List/HL014/PageNO";

            //XML文件的TBNO路径
            string TBNOpathaffix = "/FXBB2011-TransmitDataList/Affix-List/Affix/TBNO";
            string TBNOpath01 = "/FXBB2011-TransmitDataList/HL011-List/HL011/TBNO";
            string TBNOpath02 = "/FXBB2011-TransmitDataList/HL012-List/HL012/TBNO";
            string TBNOpath03 = "/FXBB2011-TransmitDataList/HL013-List/HL013/TBNO";
            string TBNOpath04 = "/FXBB2011-TransmitDataList/HL014-List/HL014/TBNO";

            //XML  ReportTitle  里PageNO节点的集合
            XmlNodeList nodeList = doc.SelectNodes("/FXBB2011-TransmitDataList/ReportTitle-List/ReportTitle/PageNO");

            //遍历PageNO节点集合
            for (int i = 0; i < nodeList.Count; i++)
            {
                string newPageNO = (i + 2).ToString();//重新创建的新的PageNO节点值
                nodeList[i].InnerText = newPageNO;//将新节点值赋给节点
                XmlAttribute xa = doc.CreateAttribute("PageNO");//创建一个节点的属性
                xa.Value = newPageNO;//将新节点值赋给这个节点属性
                nodeList[i].ParentNode.Attributes.Append(xa);//找到该节点的父节点，将这个属性添加到父节点

                //将（Affix、hl011-hl014）的TBNO、PageNO的值换成新节点值
                make(doc, pathaffix, newPageNO, TBNOpathaffix);
                make(doc, path01, newPageNO, TBNOpath01);
                make(doc, path02, newPageNO, TBNOpath02);
                make(doc, path03, newPageNO, TBNOpath03);
                make(doc, path04, newPageNO, TBNOpath04);
            }
            return doc;
        }

        /// <summary>
        /// 子节点元素数新值替换原始值,为了生成的XML文件主键是从2开始排序
        /// </summary>
        /// <param name="doc">XML文件</param>
        /// <param name="path">节点路径</param>
        /// <param name="newPageNO">新节点元素的值</param>
        public void make(XmlDocument doc, string path, string newPageNO, string TBNOpath)
        {
            XmlNodeList nodelist = doc.SelectNodes(path);//节点路径
            XmlNodeList TBNOnodelist = doc.SelectNodes(TBNOpath);//TBNO节点路径
            for (int i = 0; i < nodelist.Count; i++)
            {
                XmlAttribute xa = doc.CreateAttribute("TBNO");//创建一个节点的属性
                xa.Value = (2 + i).ToString();//将新节点值赋给这个节点属性
                nodelist[i].ParentNode.Attributes.Append(xa);//找到该节点的父节点，将这个属性添加到父节点
                nodelist[i].InnerText = newPageNO;
                TBNOnodelist[i].InnerText = (2 + i).ToString();//将当前原始值替换为新节点值
            }
        }

        /// <summary>
        /// 发送报表
        /// </summary>
        /// <param name="xmlFile">报表文件（xml格式，但不是xml文件）</param>
        /// <param name="curUnitCode">当前单位的代码</param>
        /// <param name="receiveUnitCode">接收单位代码</param>
        /// <returns>大于0，成功</returns>
        public int SendFile(string xmlFile, string curUnitCode, string receiveUnitCode)
        {
            int count = 0;
           
            using (FileStream fs = File.OpenRead(xmlFile))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                string file = Path.GetFileName(xmlFile);

                if (HttpContext.Current.Request.Cookies["unitcode"].Value.StartsWith("22")) //吉林
                {
                    WS_Deliver.DeliverSoapClient deliver = new WS_Deliver.DeliverSoapClient();
                    int.TryParse(deliver.DeliverXml(data, file, curUnitCode, receiveUnitCode), out count);
                }
                else
                {
                    WS_XML.wsRptTransSoapClient sendRpt = new WS_XML.wsRptTransSoapClient();//旧的发送方法
                    count = sendRpt.UploadReport(data, file, curUnitCode, receiveUnitCode);//测试时不发送给国家防总
                }
                //WS_BS_CS_XML.RptTransSoapClient sendRpt = new RptTransSoapClient();
                //count = sendRpt.UploadReport(data, file, curUnitCode, receiveUnitCode);//返回的是存入CS库中的页号
            }
            //if (count > 0)//bs版本
            //{
            DeleteXMLByFileName(xmlFile);
            //}
            return count;
        }

        /// <summary>删除生成的xml文件
        /// </summary>
        /// <param name="fileName">xml的绝对路径</param>
        public void DeleteXMLByFileName(string fileName)
        {
            if (System.IO.File.Exists(fileName))//判断文件是否存在
            {
                System.IO.File.Delete(fileName);
            }
        }

        /// <summary>
        /// 发送附件给上级单位
        /// </summary>
        /// <param name="targetCode">目标单位代码（接收单位代码）</param>
        /// <param name="fileName">文件名</param>
        /// <returns> 成功：1，失败：0</returns>
        public int SendAffix(string targetCode, string fileName)
        {
            FileStream fs = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + fileName.Replace("/", "\\"));
            byte[] fileByte = new byte[fs.Length];
            fs.Read(fileByte, 0, fileByte.Length);
            fileName = fileName.Remove(0, 6);
            string tmp = "";

            if (HttpContext.Current.Request.Cookies["unitcode"].Value.StartsWith("22")) //吉林
            {
                WS_Deliver.DeliverSoapClient deliver = new WS_Deliver.DeliverSoapClient();
                tmp = deliver.DeliverAffix(0, targetCode, fileName, fileByte);
            }
            else
            {
                WS_Affix.ServiceSoapClient sendAffix = new WS_Affix.ServiceSoapClient();
                tmp = sendAffix.SaveFileToServer_V1(0, targetCode, fileName, fileByte);//测试时不发送给国家防总
            }
            
            if (tmp == "发送完成!")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
