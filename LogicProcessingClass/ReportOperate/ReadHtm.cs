using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms.VisualStyles;
using LogicProcessingClass.Model;

namespace LogicProcessingClass.ReportOperate
{
    public class ReadHtm
    {
        HttpApplicationState App = HttpContext.Current.Application;

        /// <summary>根据单位代码，读取对应的htm文件
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="tableType">报表类型，洪涝：HL，蓄水:HP</param>
        /// <param name="operateType">报表操作类型，编辑：edit，查看:view</param>
        /// <returns></returns>
        public string ReadHtmByUnitCode(string unitCode, string tableType, string operateType)
        {
            string str = "";
            string subUnitCode = unitCode.Substring(0, 2);
            if (App[subUnitCode + tableType + operateType + "htm"] != null)
            {
                str = App[subUnitCode + tableType + operateType + "htm"].ToString();
            }
            else
            {
                string strHtml = "";
                string commUrl = "";
                string unitUrl = "";
                string tHeadUrl = "";
                string tBodyUrl = "";
                string bodyFileName = "";

                string baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Scripts/Templates/Public/";
                int maxTableCount = 9;//表的个数
                if (tableType == "HL")
                {
                    maxTableCount = 9;
                }
                else if (tableType == "HP")
                {
                    maxTableCount = 4;
                }
                for (int i = 1; i <= maxTableCount; i++)
                {
                    commUrl = baseDirectory + tableType + "/Table/" + i + "/Common";
                    unitUrl = baseDirectory + tableType + "/Table/" + i + "/" + subUnitCode;//当前登录单位htm页面所在文件夹的路径
                    if (operateType.ToLower() == "edit")
                    {
                        bodyFileName = "/TBody_Edit.htm";
                    }
                    else if (operateType.ToLower() == "view")
                    {
                        bodyFileName = "/TBody_View.htm";
                    }

                    tHeadUrl = commUrl + "/THead.htm";
                    tBodyUrl = commUrl + bodyFileName;

                    if (Directory.Exists(unitUrl))
                    {
                        tHeadUrl = unitUrl + "/THead.htm";
                        tBodyUrl = unitUrl + bodyFileName;
                        if (!System.IO.File.Exists(tHeadUrl))//可能当前登录单位的tHead文件不存在
                        {
                            tHeadUrl = commUrl + "/THead.htm";
                        }
                        if (!System.IO.File.Exists(tBodyUrl))//可能当前登录单位的tBody文件不存在
                        {
                            tBodyUrl = commUrl + bodyFileName;
                        }
                    }

                    strHtml = "<table table-fixed ng-switch-when='" + (i - 1) + "'>" + ReadHtmByUrl(tHeadUrl) + ReadHtmByUrl(tBodyUrl) + "</table>";
                    str += strHtml; 
                }

                str = "<div class='Rpt-Content' ng-switch on='Report[Attr.NameSpace].Current.Attr.TableIndex'>" + str;

                if (operateType.ToLower() == "view")
                {
                    str = str + "</div>";
                }
                else
                {
                    str = str + "<div ng-switch-when='4' ng-initdeathtree class='DeathTree'></div></div>";
                }

                str = str + ReadHtmByUrl(System.AppDomain.CurrentDomain.BaseDirectory.ToString() +
                                   "Scripts/Templates/Public/" + tableType + "/Tab.htm");
                App[unitCode.Substring(0, 2) + tableType + operateType + "htm"] = str;

                /*保存成一个htm文件
                string filePath = HttpContext.Current.Server.MapPath(htmlpath);
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (StreamWriter sw = new StreamWriter(filePath,false, Encoding.GetEncoding("GB2312")))
                {
                    sw.Write(sb);
                }
                */
                //}
            }
            return str;
        }


        /// <summary>读取不同表的htm文件内容
        /// </summary>
        /// <param name="htmUrl"></param>
        /// <returns></returns>
        public string ReadHtmByUrl(string htmUrl)
        {
            string strLine = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(htmUrl))
            {
                strLine = sr.ReadToEnd();
            }
            return strLine;
        }
    }
}
