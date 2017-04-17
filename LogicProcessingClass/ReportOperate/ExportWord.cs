using System;
using System.Linq;
using System.Text;
using EntityModel;
using Aspose.Words;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using DBHelper;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：ExportWord.cs
// 文件功能描述：导出WORD
// 创建标识：胡汗 2013年12月12日
// 修改标识：

// 修改描述：
//-------------------------------------------------------------*/
using LogicProcessingClass.Model;

namespace LogicProcessingClass.ReportOperate
{
    public class ExportWord
    {
        /// <summary>
        /// 灾情综述导出word
        /// </summary>
        /// <param name="zqzsBean">灾情综述实体类</param>
        public string ExportZqzsToWord(ZqzsBean zqzsBean, int limit)
        {
            DateTime dt = DateTime.Now;
            string strTime = dt.Year.ToString() + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second;
            string fileName = strTime + "灾情综述.doc";//文件名称
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "ExcelFile\\";//文件路径
            #region 字符串拼接方式
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc); //建立DocumentBuilder物件
            StringBuilder titleStr = new StringBuilder();
            string[] contents = new string[6];
            string[] titles = new string[6];
            string unitName = System.Configuration.ConfigurationSettings.AppSettings["UnitName"];
            titleStr.Append(zqzsBean.UnitName).Append("").Append(zqzsBean.StartDateTime).Append("至").Append(zqzsBean.EndDateTime).Append("洪涝灾害情况综述");
            StringBuilder fTitleStr = new StringBuilder();
            fTitleStr.Append("（根据具体洪涝灾害过程进行调整填报）");
            string fieldArr = "szrk,zyrk,dtfw,zjjjzss,shmjxj,czmjxj,jsmjxj,yzjcls,swdsc,scyzmj,scyzsl,nlmyzjjjss,gjyszjjjss,slsszjjjss,gchswkrk,gcjjzyrk,jzwsyfw,cqzjjjss";
            string[] strFieldUnitArr = QueryNumUnit(fieldArr.Split(','), limit.ToString());
            titles[0] = "一、雨情";
            contents[0] = new StringBuilder().Append("受").Append(zqzsBean.DisasterTypeName).Append("（气象因素）影响，").Append(zqzsBean.StartDateTime).Append("至").Append(zqzsBean.EndDateTime).Append("，全").
                Append(zqzsBean.SimpleUnitName).Append("出现").Append(zqzsBean.JYQDDX).Append("（降雨强度定性）过程，降雨范围").Append(zqzsBean.JYFW).Append("，全").Append(zqzsBean.SimpleUnitName).
                Append("平均雨量").Append(zqzsBean.PJYL).Append("毫米，超过").Append(zqzsBean.CPJYL).Append("毫米有").Append(zqzsBean.CPJYLZS).Append("站，").Append("覆盖面积").
                Append(zqzsBean.FGMJ).Append("平方公里，过程降雨量最大的单站降雨量").Append(zqzsBean.GCZDDZYL).Append("毫米，日降雨量最大的单站降雨量").Append(zqzsBean.RZDDZYL).
                Append("毫米，雨强最大的单站降雨量").Append(zqzsBean.YQZDDZYL).Append("毫米。").ToString();

            titles[1] = "二、水情";
            contents[1] = new StringBuilder().Append("受强降雨影响，主要江河水位上涨情况概述，").Append(zqzsBean.JMC).Append("江、").Append(zqzsBean.HMC).Append("河水位猛涨，有").Append(zqzsBean.CLSZS).
                Append("站超历史，有").Append(zqzsBean.CBZZS).Append("站超保证，有").Append(zqzsBean.CJJZS).Append("站超警戒。主要江河控制站水文特征值（如：**江**站16日0时洪峰水位32.75米，超过警戒水位4.25米，").
                Append("例：1953年有实测记录以来第5位，洪水频率超过10年一遇，相应流量6430立方米每秒，水位流量均为1999年以来最大）。").ToString();

            titles[2] = "三、灾情综述";
            contents[2] = new StringBuilder().Append("暴雨洪水造成").Append(zqzsBean.ZQSM).Append("（山洪、滑坡、泥石流……）等洪涝灾害，据统计，截止").Append(zqzsBean.ZQJZRQ).Append("共造成全").Append(zqzsBean.SimpleUnitName).
                //Append(zqzsBean.UnitData).Append(zqzsBean.SZDSMC).Append("等")
                Append(zqzsBean.SZFWDSS).Append("个市（州）").Append(zqzsBean.SZFWX).Append("个县（市、区）").  
                Append(zqzsBean.SZFWZ).Append("个乡镇").Append(zqzsBean.SZRK).Append(strFieldUnitArr[0]).Append("受灾，因灾死亡").Append(zqzsBean.SWRK).
                Append("人（其中山洪冲淹、滑坡、泥石流死亡").Append(zqzsBean.ZYRYSWRK).Append("人），失踪").Append(zqzsBean.SZRKR).Append("人，紧急转移人口").
                Append(zqzsBean.ZYRK).Append(strFieldUnitArr[1]).Append("，倒塌房屋").Append(zqzsBean.DTFW).Append(strFieldUnitArr[2]).Append("，直接经济损失").
                Append(zqzsBean.ZJJJZSS).Append(strFieldUnitArr[3]).Append("。（详细损失）农作物受灾面积 ").Append(zqzsBean.SHMJXJ).Append(strFieldUnitArr[4]).Append("，其中成灾面积").
                Append(zqzsBean.CZMJXJ).Append(strFieldUnitArr[5]).Append("，绝收面积").Append(zqzsBean.JSMJXJ).Append(strFieldUnitArr[6]).Append("，因灾减产粮食").
                Append(zqzsBean.YZJCLS).Append(strFieldUnitArr[7]).Append("，死亡大牲畜").Append(zqzsBean.SWDSC).Append(strFieldUnitArr[8]).Append("，水产养殖损失").
                Append(zqzsBean.SCYZMJ).Append(strFieldUnitArr[9]).Append(zqzsBean.SCYZSL).Append(strFieldUnitArr[10]).Append("，农林牧渔业直接经济损失").Append(zqzsBean.NLMYZJJJSS).
                Append(strFieldUnitArr[11]).Append("；停产工矿企业").Append(zqzsBean.TCGKQY).Append("个，铁路中断").Append(zqzsBean.TLZD).Append("条次，公路中断 ").
                Append(zqzsBean.GLZD).Append("条次，机场港口关停").Append(zqzsBean.JCGKGT).Append("个次，供电中断").Append(zqzsBean.GDZD).Append("条次，通讯中断").Append(zqzsBean.TXZD).
                Append("条次，工业交通运输业直接经济损失").Append(zqzsBean.GJYSZJJJSS).Append(strFieldUnitArr[12]).Append("；损坏水库").Append(zqzsBean.SHSKD).
                Append("座（大中型），水库垮坝").Append(zqzsBean.SKKBD).Append("座（大中型），损坏堤防").Append(zqzsBean.SHDFCS).Append("处，长度").Append(zqzsBean.SHDFCD).
                Append("千米，损坏护岸").Append(zqzsBean.SHHAC).Append("处，损坏水闸").Append(zqzsBean.SHSZ).Append("座，冲毁塘坝").Append(zqzsBean.CHTB).Append("座，损坏灌溉设施").
                Append(zqzsBean.SHGGSS).Append("处，损坏水文测站").Append(zqzsBean.SHSWCZ).Append("个，损坏机电泵站").Append(zqzsBean.SHJDBZ).Append("座，损坏水电站").
                Append(zqzsBean.SHSDZ).Append("座，水利设施直接经济损失").Append(zqzsBean.SLSSZJJJSS).Append(strFieldUnitArr[13]).Append("。（城市受淹情况）受淹城市").
                Append(zqzsBean.SYCS).Append("个，受淹面积").Append(zqzsBean.YMFWMJ).Append("平方公里，占城区面积比例").Append(zqzsBean.YMFWBL).Append("%，淹没历时").
                Append(zqzsBean.GCYMLS).Append("小时，洪水围困人口").Append(zqzsBean.GCHSWKRK).Append(strFieldUnitArr[14]).Append("，紧急转移人口").Append(zqzsBean.GCJJZYRK).
                Append(strFieldUnitArr[15]).Append("，主要街道最大水深").Append(zqzsBean.ZYZJZDSS).Append("米，供水中断").Append(zqzsBean.SMXGS).Append("小时，供电中断").
                Append(zqzsBean.SMXGD).Append("小时，供气中断").Append(zqzsBean.SMXGQ).Append("小时，交通中断").Append(zqzsBean.SMXJT).Append("小时，房屋受淹").Append(zqzsBean.JZWSYFW).
                Append(strFieldUnitArr[16]).Append("，地下设施受淹").Append(zqzsBean.JZWSYDX).Append("平方米，造成城区直接经济损失").Append(zqzsBean.CQZJJJSS).
                Append(strFieldUnitArr[17]).Append("。").ToString();

            titles[3] = "四、灾情特点";
            contents[3] = zqzsBean.ZQTD;

            titles[4] = "五、灾情地区";
            contents[4] = zqzsBean.ZQDQ;

            titles[5] = "六、抗灾救灾典型";
            contents[5] = zqzsBean.KZJZDX;

            builder.ParagraphFormat.LineSpacing = 16;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//居中
            InsertTitle(builder, titleStr.ToString(),true,20);
            builder.Writeln(fTitleStr.ToString());
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;//居左
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            for (int i = 0; i < 6; i++)
            {
                InsertTitle(builder, titles[i], true, 18);
                InsertContent(builder,contents[i],false,16);
            }

            #endregion
            #region word模板方式
            //Document doc = new Document("E:\\zxfw\\ZQFW\\test\\word_export\\" + "2013121019285819306.doc");
            //string unitName = System.Configuration.ConfigurationSettings.AppSettings["UnitName"];
            //string fieldArr = "szrk,zyrk,dtfw,zjjjzss,shmjxj,czmjxj,jsmjxj,yzjcls,swdsc,scyzmj,scyzsl,nlmyzjjjss,gjyszjjjss," +
            //                  "slsszjjjss,gchswkrk,gcjjzyrk,jzwsyfw,cqzjjjss";
            //string[] strFieldUnitArr = QueryNumUnit(fieldArr.Split(','), zqzsBean.Limit);
            //Replace(doc,  "d1", zqzsBean.UnitName);
            //Replace(doc,  "d2", zqzsBean.startTime);
            //Replace(doc,  "d3", zqzsBean.endTime);
            //Replace(doc,  "d4", zqzsBean.disastertypename);
            //Replace(doc,  "d5", zqzsBean.startTime);
            //Replace(doc,  "d6", zqzsBean.endTime);
            //Replace(doc,  "d7", zqzsBean.unitData);
            //Replace(doc,  "d8", zqzsBean.jyqddx);
            //Replace(doc,  "d9", zqzsBean.jyfw);
            //Replace(doc,  "d10", zqzsBean.unitData);
            //Replace(doc,  "d11", zqzsBean.pjyl);
            //Replace(doc,  "d12", zqzsBean.cpjyl);
            //Replace(doc,  "d13", zqzsBean.cpjylzs);
            //Replace(doc,  "d14", zqzsBean.fgmj);
            //Replace(doc,  "d15", zqzsBean.gczddzyl);
            //Replace(doc,  "d16", zqzsBean.rzddzyl);
            //Replace(doc,  "d17", zqzsBean.yqzddzyl);
            //Replace(doc,  "d18", zqzsBean.jmc);
            //Replace(doc,  "d19", zqzsBean.hmc);
            //Replace(doc,  "d20", zqzsBean.clszs);
            //Replace(doc,  "d21", zqzsBean.cbzzs);
            //Replace(doc,  "d22", zqzsBean.cjjzs);
            //Replace(doc,  "d23", zqzsBean.zqsm);
            //Replace(doc,  "d24", zqzsBean.zqjzrq);
            //Replace(doc,  "d25", zqzsBean.unitData);
            //Replace(doc,  "d26", zqzsBean.szdsmc);
            //Replace(doc,  "d27", zqzsBean.szfwdss);
            //Replace(doc,  "d28", zqzsBean.szfwx);
            //Replace(doc,  "d29", zqzsBean.szfwz);
            //Replace(doc,  "d30", zqzsBean.szrk);
            //Replace(doc,  "d31", zqzsBean.swrk);
            //Replace(doc,  "d32", zqzsBean.zyrkswrk);
            //Replace(doc,  "d33", zqzsBean.szrkr);
            //Replace(doc,  "d34", zqzsBean.zyrk);
            //Replace(doc,  "d35", zqzsBean.dtfw);
            //Replace(doc,  "d36", zqzsBean.zjjjzss);
            //Replace(doc,  "d37", zqzsBean.shmjxj);
            //Replace(doc,  "d38", zqzsBean.czmjxj);
            //Replace(doc,  "d39", zqzsBean.jsmjxj);
            //Replace(doc,  "d40", zqzsBean.yzjcls);
            //Replace(doc,  "d41", zqzsBean.swdsc);
            //Replace(doc,  "d42", zqzsBean.scyzmj);
            //Replace(doc,  "d43", zqzsBean.scyzsl);
            //Replace(doc,  "d44", zqzsBean.nlmyzjjjss);
            //Replace(doc,  "d45", zqzsBean.tcgkqy);
            //Replace(doc,  "d46", zqzsBean.tlzd);
            //Replace(doc,  "d47", zqzsBean.glzd);
            //Replace(doc,  "d48", zqzsBean.jcgkgt);
            //Replace(doc,  "d49", zqzsBean.gdzd);
            //Replace(doc,  "d50", zqzsBean.txzd);
            //Replace(doc,  "d51", zqzsBean.gjyszjjjss);
            //Replace(doc,  "d52", zqzsBean.shskd);
            //Replace(doc,  "d53", zqzsBean.skkbd);
            //Replace(doc,  "d54", zqzsBean.shdfcs);
            //Replace(doc,  "d55", zqzsBean.shdfcd);
            //Replace(doc,  "d56", zqzsBean.shhac);
            //Replace(doc,  "d57", zqzsBean.shsz);
            //Replace(doc,  "d58", zqzsBean.chtb);
            //Replace(doc,  "d59", zqzsBean.shggss);
            //Replace(doc,  "d60", zqzsBean.shswcz);
            //Replace(doc,  "d61", zqzsBean.shjdbz);
            //Replace(doc,  "d62", zqzsBean.shsdz);
            //Replace(doc,  "d63", zqzsBean.slsszjjjss);
            //Replace(doc,  "d64", zqzsBean.sycss);
            //Replace(doc,  "d65", zqzsBean.ymfwmj);
            //Replace(doc,  "d66", zqzsBean.ymfwbl);
            //Replace(doc,  "d67", zqzsBean.gcymls);
            //Replace(doc,  "d68", zqzsBean.gchswkrk);
            //Replace(doc,  "d69", zqzsBean.gcjjzyrk);
            //Replace(doc,  "d70", zqzsBean.zyzjzdss);
            //Replace(doc,  "d71", zqzsBean.smxgs);
            //Replace(doc,  "d72", zqzsBean.smxgd);
            //Replace(doc,  "d73", zqzsBean.smxgq);
            //Replace(doc,  "d74", zqzsBean.smxjt);
            //Replace(doc,  "d75", zqzsBean.jzwsyfw);
            //Replace(doc,  "d76", zqzsBean.jzwsydx);
            //Replace(doc,  "d77", zqzsBean.cqzjjjss);
            //Replace(doc,  "d78", zqzsBean.zqtd);
            //Replace(doc,  "d79", zqzsBean.zqdq);
            //Replace(doc,  "d80", zqzsBean.kzjzdx);
            //for (int i = 0; i < 18; i++)
            //{
            //    Replace(doc,  "u" + i, strFieldUnitArr[i]);
            //}
            #endregion
            doc.Save(filePath + fileName);//将文件存档
            return filePath + fileName;
        }

        public string MailMergeToZQZSWord(string json, int limit, string code, string img_url)
        {
            string[] arr = json.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string[] fieldNames = arr[0].Split(new string[] { "," }, StringSplitOptions.None);
            string[] fieldValues = arr[1].Split(new string[] { "," }, StringSplitOptions.None);
            string fileName = "";
            for (int i = 0; i < fieldNames.Length; i++)
            {
                if (fieldNames[i] == "StartDateTime")
                {
                    fileName = fieldValues[i];
                }else if (fieldNames[i] == "EndDateTime")
                {
                    fileName += "～" + fieldValues[i];
                }
            }
            fileName = fileName.Replace("年", "-").Replace("月", "-").Replace("日", "");
            fileName = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request["unitname"]) + "灾情综述" + fileName;

            string tempPath = System.Web.HttpContext.Current.Server.MapPath("~/Views/Release/ReportDetails/HL01/" + code + ".doc");
            string outputPath = System.Web.HttpContext.Current.Server.MapPath("~/ExcelFile/" + fileName + ".doc");

            try
            {
                var doc = new Document(tempPath);  //载入模板
                doc.MailMerge.Execute(fieldNames, fieldValues);  //合并模版，相当于页面的渲染

                if (!string.IsNullOrEmpty(img_url))
                {
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.MoveToDocumentEnd();  //移动焦点到文档最后
                    builder.InsertBreak(BreakType.LineBreak);//换行
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//居中
                    builder.InsertImage(img_url);
                }

                doc.Save(outputPath);  //保存合并后的文档
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return outputPath;
        }

        /// <summary>
        /// 灾情综述导出word
        /// </summary>
        /// <param name="zqzsBean">灾情综述实体类</param>
        public string ExportZqzsToWord_15(ZqzsBean_15 zqzsBean, int limit)
        {
            DateTime dt = DateTime.Now;
            string strTime = dt.Year.ToString() + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second;
            string fileName = strTime + "灾情综述.doc";//文件名称
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "ExcelFile\\";//文件路径
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc); //建立DocumentBuilder物件
            StringBuilder titleStr = new StringBuilder();
            string[] contents = new string[6];
            string[] titles = new string[6];
            string unitName = System.Configuration.ConfigurationSettings.AppSettings["UnitName"];
            titleStr.Append(zqzsBean.UnitName).Append("地区").Append(zqzsBean.StartDateTime).Append("至").Append(zqzsBean.EndDateTime).Append("洪涝灾害情况综述");
            //StringBuilder fTitleStr = new StringBuilder();
            //fTitleStr.Append("（根据具体洪涝灾害过程进行调整填报）");
            string fieldArr = "szrk,zyrk,dtfw,zjjjzss,shmjxj,czmjxj,jsmjxj,yzjcls,swdsc,scyzmj,scyzsl,nlmyzjjjss,gjyszjjjss,slsszjjjss,gchswkrk,gcjjzyrk,jzwsyfw,cqzjjjss";
            string[] strFieldUnitArr = QueryNumUnit(fieldArr.Split(','), limit.ToString());
            titles[0] = "一、雨情";
            contents[0] =
                new StringBuilder().Append("    ").ToString();  /*Append(zqzsBean.StartDateTime).Append("至").Append(zqzsBean.EndDateTime)
                    .Append("，全区有").Append(zqzsBean.ZGJMS).Append("盟市出现降水过程，其中有")
                    .Append(zqzsBean.JMS).Append("盟市、").Append(zqzsBean.JQX).Append("旗县出现")
                    .Append(zqzsBean.QMJYL).Append("雨，过程降雨量最大是").Append(zqzsBean.GCJYLMax)
                    .Append("站，降雨量为").Append(zqzsBean.GCJYL).Append("毫米，日降雨量最大是")
                    .Append(zqzsBean.RJYLMax).Append("站，降雨量为").Append(zqzsBean.RJYL).Append("毫米。")*/
            titles[1] = "二、水情";
            contents[1] = new StringBuilder().Append("    ").ToString();   /*Append("受强降雨影响，主要江河水位上涨情况概述，").Append(zqzsBean.JMC).Append("江、").Append(zqzsBean.HMC).Append("河水位猛涨，有").Append(zqzsBean.CLSZS).
                Append("站超历史，有").Append(zqzsBean.CBZZS).Append("站超保证，有").Append(zqzsBean.CJJZS).Append("站超警戒。主要江河控制站水文特征值（如：**江**站16日0时洪峰水位32.75米，超过警戒水位4.25米，").
                Append("例：1953年有实测记录以来第5位，洪水频率超过10年一遇，相应流量6430立方米每秒，水位流量均为1999年以来最大）。")*/

            titles[2] = "三、灾情";
            contents[2] = new StringBuilder().Append("").Append("据统计，从").Append(zqzsBean.ZQKSRQ).Append("到").Append(zqzsBean.ZQJZRQ).Append("，全区共造成").
                Append(zqzsBean.SZFWDSS).Append(zqzsBean.SZFWX).
                Append(zqzsBean.SZFWZ).Append("个乡镇苏木，").Append(zqzsBean.SZRK).Append(strFieldUnitArr[0]).Append("受灾，因灾死亡").Append(zqzsBean.SWRK).
                Append("人（其中山洪冲淹、滑坡、泥石流死亡").Append(zqzsBean.ZYRYSWRK).Append("人），失踪").Append(zqzsBean.SZRKR).Append("人，紧急转移人口").
                Append(zqzsBean.ZYRK).Append(strFieldUnitArr[1]).Append("，倒塌房屋").Append(zqzsBean.DTFW).Append(strFieldUnitArr[2]).Append("。农作物受灾面积 ").Append(zqzsBean.SHMJXJ).Append(strFieldUnitArr[4]).Append("，成灾面积").
                Append(zqzsBean.CZMJXJ).Append(strFieldUnitArr[5]).Append("，绝收面积").Append(zqzsBean.JSMJXJ).Append(strFieldUnitArr[6]).Append("，因灾减产粮食").
                Append(zqzsBean.YZJCLS).Append(strFieldUnitArr[7]).Append("，死亡大牲畜").Append(zqzsBean.SWDSC).Append(strFieldUnitArr[8]).Append("，水产养殖损失").
                Append(zqzsBean.SCYZMJ).Append(strFieldUnitArr[9]).Append(zqzsBean.SCYZSL).Append(strFieldUnitArr[10]).Append("；停产工矿企业").Append(zqzsBean.TCGKQY).Append("个，铁路中断").Append(zqzsBean.TLZD).Append("条次，公路中断 ").
                Append(zqzsBean.GLZD).Append("条次，机场港口关停").Append(zqzsBean.JCGKGT).Append("个次，供电中断").Append(zqzsBean.GDZD).Append("条次，通讯中断").Append(zqzsBean.TXZD).
                Append("条次").Append("；损坏水库").Append(zqzsBean.SHSKD).
                Append("座（大中型），水库垮坝").Append(zqzsBean.SKKBD).Append("座（大中型），损坏堤防").Append(zqzsBean.SHDFCS).Append("处、长度").Append(zqzsBean.SHDFCD).
                Append("千米，损坏护岸").Append(zqzsBean.SHHAC).Append("处，损坏水闸").Append(zqzsBean.SHSZ).Append("座，冲毁塘坝").Append(zqzsBean.CHTB).Append("座，损坏灌溉设施").
                Append(zqzsBean.SHGGSS).Append("处，损坏水文测站").Append(zqzsBean.SHSWCZ).Append("个，损坏机电泵站").Append(zqzsBean.SHJDBZ).Append("座，损坏水电站").
                Append(zqzsBean.SHSDZ).Append("座").Append("。受淹城市").
                Append(zqzsBean.SYCS).Append("个，受淹面积").Append(zqzsBean.YMFWMJ).Append("平方公里，占城区面积比例").Append(zqzsBean.YMFWBL).Append("%，淹没历时").
                Append(zqzsBean.GCYMLS).Append("小时，洪水围困人口").Append(zqzsBean.GCHSWKRK).Append(strFieldUnitArr[14]).Append("，受淹过程中紧急转移人口").Append(zqzsBean.GCJJZYRK).
                Append(strFieldUnitArr[15]).Append("，主要街道最大水深").Append(zqzsBean.ZYZJZDSS).Append("米，供水中断").Append(zqzsBean.SMXGS).Append("小时，供电中断").
                Append(zqzsBean.SMXGD).Append("小时，供气中断").Append(zqzsBean.SMXGQ).Append("小时，交通中断").Append(zqzsBean.SMXJT).Append("小时，房屋受淹").Append(zqzsBean.JZWSYFW).
                Append(strFieldUnitArr[16]).Append("，地下设施受淹").Append(zqzsBean.JZWSYDX).Append("平方米。").Append("共造成直接经济损失").Append(zqzsBean.ZJJJZSS).Append(strFieldUnitArr[17]).
                Append("，其中农林牧渔业直接经济损失").Append(zqzsBean.NLMYZJJJSS).Append(strFieldUnitArr[11]).
                Append("、工业交通运输业直接经济损失").Append(zqzsBean.GJYSZJJJSS).Append(strFieldUnitArr[12]).
                Append("、水利设施直接经济损失").Append(zqzsBean.SLSSZJJJSS).Append(strFieldUnitArr[13]).
                Append("、城区直接经济损失").Append(zqzsBean.CQZJJJSS).Append(strFieldUnitArr[3]).ToString();


            titles[3] = "四、抢险投入";
            contents[3] = new StringBuilder().Append("").Append("总投入抢险人数").Append(zqzsBean.QXHJ).Append("人次，其中部队官兵").
                Append(zqzsBean.QXBDGB).Append("人次、地方人员").Append(zqzsBean.QXDFRY).
                Append("人次、 防汛机动抢险队员").Append(zqzsBean.QXFXJD).Append("人次。")
                .Append("总投入资金小计").Append(zqzsBean.ZJXJ).Append("万元，其中中央").Append(zqzsBean.ZJZY).
                Append("万元、省级").Append(zqzsBean.ZJSJ).Append("万元、省级以下").Append(zqzsBean.ZJSJYS).Append("万元、群众投劳折资").Append(zqzsBean.ZJQZ).Append("万元。").ToString(); //

            builder.ParagraphFormat.LineSpacing = 16;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//居中
            InsertTitle(builder, titleStr.ToString(), true, 20);
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;//居左
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            for (int i = 0; i < 4; i++)
            {
                InsertTitle(builder, titles[i], true, 18);
                InsertContent(builder, contents[i], false, 16);
            }
            /*InsertContent(builder, contents[3], false, 16);
            InsertTitle(builder, titles[3], true, 18);
            InsertContent(builder, contents[4], false, 16);*/
            
            doc.Save(filePath + fileName);//将文件存档
            return filePath + fileName;
        }

        /// <summary>
        /// 蓄水情况导出word
        /// </summary>
        /// <param name="xsqkBean">蓄水情况实体类</param>
        /// <param name="limit"></param>
        public string ExportXsqkToWord(XsqkBean xsqkBean, int limit)
        {
            DateTime dt = DateTime.Now;
            string strTime = dt.Year.ToString() + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second;
            string fileName = strTime + "蓄水情况.doc";//文件名称
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "ExcelFile\\";//文件路径System.AppDomain.CurrentDomain.BaseDirectory.ToString()
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc); //建立DocumentBuilder物件
            StringBuilder titleStr = new StringBuilder();
            string[] contents = new string[2];
            StringBuilder fTitleStr = new StringBuilder();
            dt = Convert.ToDateTime(xsqkBean.Time);
            //fTitleStr.Append(dt.Year).Append("年").Append(dt.Month).Append("月").Append(dt.Day).Append("日");
            fTitleStr.Append(xsqkBean.Time);
            titleStr.Append(xsqkBean.UnitName).Append(xsqkBean.Month).Append("月中旬蓄水情况");
            if (dt.Day > 16 || dt.Day == 1)
            {
                titleStr.Replace("月中旬", "月下旬");
            }
            contents[0] =
                new StringBuilder().Append(xsqkBean.TJ_MONTH)
                    .Append("月")
                    .Append(xsqkBean.TJ_DAY)
                    .Append("日统计，全")
                    .Append(xsqkBean.SimpleUnitName)
                    .Append(xsqkBean.XSCSZJ)
                    .Append("万处蓄水工程共蓄水")
                    .
                    Append(xsqkBean.XXSLZJ)
                    .Append(xsqkBean.XSLMeasureName)
                    .Append("，占计划蓄水量的")
                    .Append(xsqkBean.XZYBFB)
                    .Append("%，比上期")
                    .Append(xsqkBean.SQ_MONTH)
                    .Append("月")
                    .Append(xsqkBean.SQ_DAY)
                    .Append("日")
                    .
                    Append(xsqkBean.SQ_DHS)
                    .Append("蓄")
                    .Append(xsqkBean.BSQXS)
                    .Append(xsqkBean.XSLMeasureName)
                    .Append("；比历年同期平均")
                    .Append(xsqkBean.TQ_DHS)
                    .Append("蓄")
                    .
                    Append(xsqkBean.BTQPJXS)
                    .Append(xsqkBean.XSLMeasureName)
                    .Append("，偏")
                    .Append(xsqkBean.TQ_DHS)
                    .Append("；可用水量")
                    .Append(xsqkBean.XZKYSL)
                    .Append(xsqkBean.XSLMeasureName)
                    .ToString(); //.Append("；为1973年有蓄水资料记载以来同期的第").
                //Append(xsqkBean.KYSLDJW).Append("位。").ToString();
            contents[1] = new StringBuilder().Append("蓄水量相对较多的为").Append(xsqkBean.MaxXSLCities).Append("，分别占应蓄水量的").Append(xsqkBean.MaxXSLCitiesXSBFB).
                Append("。蓄水量较少的有").Append(xsqkBean.MinXSLCities).Append("，分别占计划蓄水量的").Append(xsqkBean.MinXSLCitiesXSBFB).
                Append("。其它").Append(xsqkBean.OtherUnitName).Append("蓄水占计划蓄水量的").Append(xsqkBean.QTCSXSBFBMIN).Append("%至").
                Append(xsqkBean.QTCSXSBFBMAX).Append("%。").ToString();

            if (limit != 2)
            {
                contents[0] = contents[0].Replace("亿立方米", "万立方米");
                if (limit == 3)
                {
                    contents[0] = contents[0].Replace("全省", "全市");
                    contents[1] = contents[1].Replace("四市，", "四县，").Replace("其它市州", "其它县区");
                }
                else if (limit == 4)
                {
                    contents[0] = contents[0].Replace("全省", "全县");
                    contents[1] = contents[1].Replace("四市，", "四镇，").Replace("其它市州", "其它乡镇");
                }
            }
            builder.ParagraphFormat.LineSpacing = 16;//行间距
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;//居右
            builder.Writeln(fTitleStr.ToString());
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;//居中
            InsertTitle(builder, titleStr.ToString(), true, 20);
            builder.InsertBreak(BreakType.LineBreak);//换行
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;//居左
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            for (int i = 0; i < 2; i++)
            {
                InsertContent(builder, contents[i], false, 16);
            }
            builder.InsertBreak(BreakType.LineBreak);//换行
            //builder.Underline = Underline.Single;
            builder.InsertImage(xsqkBean.BarChartPath);
            builder.InsertImage(xsqkBean.PieChartPath);
            //builder.Underline = Underline.None;
            builder.InsertImage(xsqkBean.MapPath);
            doc.Save(filePath + fileName);//将文件存档
            return filePath + fileName;
        }

        //查询出指定字段集合的数值单位
        public string[] QueryNumUnit(string[] fieldArr, string limit)
        {
            if (fieldArr.Length == 0)
            {
                return null;
            }
            string[] strUnitArr = new string[fieldArr.Length];
            IList list = GetAllFieldUnitIList();
            
            for (int i = 0; i < fieldArr.Length; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    object[] obj = (object[])list[j];
                    if (obj[4] != null && obj[4].ToString() != "" && limit.Equals(obj[6].ToString()) && 
                        fieldArr[i].ToUpper().Equals(obj[0].ToString()))//数量级名称存在且不为空，单位级别和字段代码分别匹配
                    {

                        string str = obj[4].ToString().Replace("（", "").Replace("）", "");
                        strUnitArr[i] = str;
                        break;
                    }
                }
            }
            return strUnitArr;
        }

        /// <summary>
        /// 获得所有字段单位,并存储到Application[fieldUnit]中
        /// </summary>
        /// <returns></returns>
        public IList GetAllFieldUnitIList()
        {
            IList list = null;
            HttpApplicationState httpapplication = HttpContext.Current.Application;
            if (httpapplication["fieldUnit"] != null)
            {
                list = (IList)httpapplication["fieldUnit"];
            }
            else
            {
                list = GetReportFormList();
                httpapplication["fieldUnit"] = list;
            }
            return list;
        }

        /// <summary>
        /// 从数据库获得所有字段单位
        /// </summary>
        /// <returns></returns>
        public ArrayList GetReportFormList()
        {
            ArrayList list = new ArrayList();
            FXDICTEntities dicEntry = Persistence.GetDbEntities();
            var fieldDefines = from tb55 in dicEntry.TB55_FieldDefine select tb55;
            if (fieldDefines.Any())
            {
                foreach (var fieldDefine in fieldDefines)
                {
                    object[] temp = new object[7];
                    temp[0] = fieldDefine.FieldCode;//字段代码
                    temp[1] = fieldDefine.TD_TabCode;//表名
                    temp[2] = fieldDefine.MeasureValue;//数量级系数
                    temp[3] = fieldDefine.DecimalCount;//保留小数位数
                    temp[4] = fieldDefine.MeasureName;//数量级名称
                    temp[5] = fieldDefine.InputRemark;//填表说明
                    temp[6] = fieldDefine.UnitCls;//单位级别
                    list.Add(temp);
                }
            }
            return list;
        }
        
        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="doc">文本域</param>
        /// <param name="oldText">旧字符串</param>
        /// <param name="newText">新字符串</param>
        private void Replace(Document doc, string oldText, string newText)
        {
            doc.Range.Replace(oldText, newText == null ? "" : newText, false, false);
        }

        /// <summary>
        /// 插入标题
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="title">标题</param>
        /// <param name="bold">是否加粗</param>
        /// <param name="size">字体大小</param>
        private void InsertTitle(DocumentBuilder builder, string title, bool bold, int size)
        {
            builder.InsertBreak(BreakType.LineBreak);//换行
            builder.Font.Bold = bold;//加粗与否
            builder.Font.Size = size;//字体大小
            builder.Writeln(title);
            builder.Font.ClearFormatting();//清除字体格式
        }

        /// <summary>
        /// 插入内容
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="content">内容</param>
        private void InsertContent(DocumentBuilder builder, string content, bool bold, int size)
        {
            if (content == null)
            {
                content = "";
            }
            builder.Font.Bold = bold;//加粗与否
            builder.Font.Size = size;//字体大小
            builder.Write("        ");//缩进
            builder.Writeln(content);
        }
    }
}
