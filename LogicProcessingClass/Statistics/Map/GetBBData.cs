using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using DBHelper;

namespace LogicProcessingClass.Statistics
{
   public  class GetBBData
   {
       public GetBBData()
       {
           //
           //TODO: 在此处添加构造函数逻辑
           //
       }
      /// <summary>统计表标题和内容
      /// 
      /// </summary>
      /// <param name="pageno"></param>
      /// <returns></returns>
       public string ShowTableTile(string pageno, int level,int mapLevel,string regionCode)
       {
           StringBuilder sb = new StringBuilder();
           string context = ""; 
            string title = Get_HL_Title_Data();
            if (pageno == "")
                context = "";
            else
            {
                if (mapLevel == 1)//mapLevel默认为0，双击之后为1，获取当前区域的灾情数据
                {
                    int pageNO = int.Parse(pageno);
                    BusinessEntities bsn = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
                    var aggPage = (from agg in bsn.AggAccRecord
                                   where agg.PageNo == pageNO && agg.UnitCode == regionCode
                                   select agg.SPageNO).ToArray();
                    if (aggPage.Length > 0)
                    {
                        context = Get_HL_Data(aggPage[0].ToString(), level + 1);
                    }
                }
                else
                {
                    context = Get_HL_Data(pageno, level);
                }
            }
           sb.Append("{'hlTitle':'" + title + "','hlData':'" + context + "'}");

           return sb.ToString();
       }
       /// <summary>获得洪涝表头信息
       /// 
       /// </summary>
       /// <param name="pageno"></param>
       /// <returns></returns>
       public string Get_HL_Title_Data()
       {
           StringBuilder sb = new StringBuilder();
           sb.Append("单位|,受灾县市|(个),受灾人口|(万人),死亡人口|(人),失踪人口|(人),转移人口|(万人),")
           .Append("受灾面积|(千公顷),倒塌房屋|(万间),直接经济总损失|(亿元),水利经济损失|(亿元)");
           return sb.ToString();
       }
       /// <summary>根据页号查询灾情数据
       /// 
       /// </summary>
       /// <param name="pageno">页号</param>
       /// <returns></returns>
       public string Get_HL_Data(string pageno,int level)
       {
           StringBuilder sb = new StringBuilder();
           int pn = Convert.ToInt32(pageno);
           //sql语句
           //string sql = "select dw,szfwx,szrk/10000 as szrk,swrk,szrkr,zyrk/10000 as zyrk,shmjxj/10000000 as shmjxj, dtfw/10000 as dtfw," +
           //    "zjjjzss/100000000 as zjjjzss,slsszjjjss/100000000 as slsszjjjss from hl011 where  dw!='兵团' and  pageno =" + pageno + "order by unitcode";
           //OpenISession openISession = new OpenISession();
           //OpenISession.SessionEnum ise = OpenISession.initSession();  //获取初始连接的数据库
           //IList<object[]> list = (IList<object[]>)openISession.GetDataList(ise, sql, 0);    //打开会话
           BusinessEntities bsn = (BusinessEntities)new Entities().GetPersistenceEntityByLevel(level);
           //BusinessEntities bsn = new BusinessEntities();
           var list = (from hl in bsn.HL011
                       where hl.DW != "兵团" && hl.PageNO ==pn
                      orderby hl.UnitCode
                      select new
                      {
                          DW=hl.DW,
                          SZFWX=hl.SZFWX,
                          SZRK = hl.SZRK / 10000,
                          SWRK=hl.SWRK,
                          SZRKR=hl.SZRKR,
                          ZYRK = hl.ZYRK / 10000,
                          SHMJXJ = hl.SHMJXJ /10000000,
                          DTFW = hl.DTFW / 10000,
                          ZJJJZSS = hl.ZJJJZSS / 100000000,
                          SLSSZJJJSS = hl.SLSSZJJJSS / 100000000
                      }).ToList();
           for (int i = 0; i < list.Count; i++)
           {
               //object[] objects = list[i];
               sb.Append(list[i].DW.ToString()).Append(",");
               sb.Append(list[i].SZFWX.ToString() == "" ? "0," : Convert.ToDouble(list[i].SZFWX) + ",");
               sb.Append(list[i].SZRK.ToString() == "" ? "0," : Convert.ToDouble(String.Format("{0:f4}", Convert.ToDouble(list[i].SZRK))) + ",");
               sb.Append(list[i].SWRK.ToString() == "" ? "0," : Convert.ToDouble(list[i].SWRK) + ",");
               sb.Append(list[i].SZRKR.ToString() == "" ? "0," : Convert.ToDouble(list[i].SZRKR) + ",");
               sb.Append(list[i].ZYRK.ToString() == "" ? "0," : Convert.ToDouble(String.Format("{0:f4}", Convert.ToDouble(list[i].ZYRK))) + ",");
               sb.Append(list[i].SHMJXJ.ToString() == "" ? "0," : Convert.ToDouble(String.Format("{0:f4}", Convert.ToDouble(list[i].SHMJXJ))) + ",");
               sb.Append(list[i].DTFW.ToString() == "" ? "0," : Convert.ToDouble(String.Format("{0:f4}", Convert.ToDouble(list[i].DTFW))) + ",");
               sb.Append(list[i].ZJJJZSS.ToString() == "" ? "0," : Convert.ToDouble(String.Format("{0:f4}", Convert.ToDouble(list[i].ZJJJZSS))) + ",");
               sb.Append(list[i].SLSSZJJJSS.ToString() == "" ? "0!" : Convert.ToDouble(String.Format("{0:f4}", Convert.ToDouble(list[i].SLSSZJJJSS))) + "!");
           }

           if (sb.ToString() != "")
           {
               sb.Remove(sb.Length - 1, 1);
           }
           AuxiliaryClass.CommonFunction comFun = new AuxiliaryClass.CommonFunction();
           return comFun.cleanString(sb.ToString());
       }
    }
}
