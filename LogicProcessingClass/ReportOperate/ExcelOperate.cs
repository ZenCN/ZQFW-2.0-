using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel.PeerResolvers;
using System.Text;
using System.IO;
using EntityModel.RepeatModel;
using LogicProcessingClass.Model;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Web;
using EntityModel;
using EntityModel.ReportAuxiliaryModel;
using DBHelper;
using LogicProcessingClass.XMMZH;
using System.Collections;
using System.Configuration;
using NPOI.SS.Util;
using System.Data.OleDb;
using System.Data;
using System.Globalization;
using LogicProcessingClass.AuxiliaryClass;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：ExcelOperate.cs
// 文件功能描述：导出Excel
// 创建标识：
// 修改标识：
// 修改描述：
//-------------------------------------------------------------*/
namespace LogicProcessingClass.ReportOperate
{
    public class ExcelOperate
    {
        BusinessEntities busEntity = null;
        FXDICTEntities fxdict = null;
        public ExcelOperate(int limit)
        {
            Entities getEntity = new Entities();
            busEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
            fxdict = (FXDICTEntities)getEntity.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
        }
        /// <summary>把生成好的Excel发送到客户端
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <returns></returns>
        public string ResponseExcel(HSSFWorkbook hssfworkbook, string ordName, string sTime, string eTime)
        {
            string result = "";
            // 设置响应头（文件名和文件格式）
            //设置响应的类型为Excel
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //设置下载的Excel文件名
            HttpContext.Current.Response.AddHeader("Content-Disposition",
                string.Format("attachment; filename={0}",
                    System.Web.HttpUtility.UrlDecode(HttpContext.Current.Request["unitname"])
                    + "洪涝灾害" + ordName + "报表" + sTime + "～" + eTime + ".xls"));
            //Clear方法删除所有缓存中的HTML输出。但此方法只删除Response显示输入信息，不删除Response头信息。以免影响导出数据的完整性。
            HttpContext.Current.Response.Clear();

            //写入到客户端
            using (MemoryStream ms = new MemoryStream())
            {
                //将工作簿的内容放到内存流中
                hssfworkbook.Write(ms);
                //将内存流转换成字节数组发送到客户端
                HttpContext.Current.Response.BinaryWrite(ms.GetBuffer());
                //HttpContext.Current.ApplicationInstance.CompleteRequest();//为了解决Response.End()由于代码已经过优化或者本机框架位于调用堆栈之上，无法计算表达式的值 的异常
                HttpContext.Current.Response.End();
                result = "下载成功！";
            }
            return result;
        }

        /// <summary>把生成好的Excel发送到客户端
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <returns></returns>
        public string ResponseExcel(HSSFWorkbook hssfworkbook)
        {
            string result = "";
            // 设置响应头（文件名和文件格式）
            //设置响应的类型为Excel
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //设置下载的Excel文件名
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls"));
            //Clear方法删除所有缓存中的HTML输出。但此方法只删除Response显示输入信息，不删除Response头信息。以免影响导出数据的完整性。
            HttpContext.Current.Response.Clear();

            //写入到客户端
            using (MemoryStream ms = new MemoryStream())
            {
                //将工作簿的内容放到内存流中
                hssfworkbook.Write(ms);
                //将内存流转换成字节数组发送到客户端
                HttpContext.Current.Response.BinaryWrite(ms.GetBuffer());
                //HttpContext.Current.ApplicationInstance.CompleteRequest();//为了解决Response.End()由于代码已经过优化或者本机框架位于调用堆栈之上，无法计算表达式的值 的异常
                HttpContext.Current.Response.End();
                result = "下载成功！";
            }
            return result;
        }
        #region 导出Excel

        #region 导出洪涝Excel

        public string ExportExcel(int limit, int pageNO, string unitCode, string rptType, string ordName, string sTime, string eTime)
        {
            string result = "";
            long type = getRptType(pageNO);
            try
            {
                if (rptType == "HL01")
                {
                    if (type == 0)  //实时报 只需表9 与表5
                    {
                        return ExportSomeTable(limit, pageNO, unitCode, ordName, sTime, eTime);
                    }
                    else  //其它报表类型 导出全部表            
                    {
                        return ExportAllTab(limit, pageNO, unitCode, ordName, sTime, eTime);
                    }
                }
                else if (rptType == "HP01")
                {
                    return ExportXuShuiTable(limit, pageNO, unitCode, ordName, sTime, eTime);
                }
                else if (rptType == "GT")//国统表
                {
                    return ExportGTExcel(limit, pageNO, unitCode);
                }
                else if (rptType == "NP01")//国统表
                {
                    return ExportNMXuShuiTable(limit, pageNO, unitCode);
                }

            }
            catch (Exception ex)
            {
                result = "错误消息：" + ex.Message;
            }
            return result;
        }

        #region 导出数据到Excel

        /// <summary>
        /// 导出实时报（表5、表9）
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="pageNO"></param>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string ExportSomeTable(int limit, int pageNO, string unitCode, string ordName, string sTime, string eTime)
        {
            string result = "";
            Dictionary<string, string> riverDic = GetRiverData();
            string unitInfo = GetUnderUnitNames(unitCode, "HL01");
            string riverInfo = GetRiverDataByUnitCode(unitCode);
            string deathReason = getDiedReasonStr();
            IList<TB07_District> tb07List = getLowerUnits(unitCode, "HL01");
            IList<LZHL011> xmmList1 = GetLZHL01(pageNO, limit, tb07List);//表1-表4、以及表9
            IList<LZHL012> xmmList2 = GetLZHL02(pageNO, limit);//表5
            ReportTitle rpt = getRptInfo(pageNO);
            Dictionary<string, string> fieldDic = new Dictionary<string, string>();
            fieldDic = GetFieldsData(limit);
            string[] tableFieldsArr = new string[] { "DW,SZFWX,SZFWZ,SZRK,SYCS,DTFW,SWRK,SZRKR,ZYRK,ZJJJZSS", "DW,SHMJXJ,SHMJLS,CZMJXJ,CZMJLS,JSMJXJ,JSMJLS,YZJCLS,JJZWSS,SWDSC,SCYZMJ,SCYZSL,NLMYZJJJSS", "DW,TCGKQY,TLZD,GLZD,JCGKGT,GDZD,TXZD,GJYSZJJJSS", "DW,SHSKD,SHSKX,SKKBD,SKKBX1,SKKBX2,SHDFCS,SHDFCD,DFJKCS,DFJKCD,SHHAC,SHSZ,CHTB,SHGGSS,SHSWCZ,SHJDJ,SHJDBZ,SHSDZ,SLSSZJJJSS", "CSMC,YMFWMJ,YMFWBL,SZRK,SWRK,GCJSSJ,GCYMLS,GCLJJYL,GCHSWKRK,GCJJZYRK,ZYZJZDSS,SMXGS,SMXGD,SMXGQ,SMXJT,JZWSYFW,JZWSYDX,CQZJJJSS", "DW,WZBZD,WZBZB,WZDSSS,WZSSL,WZMC,WZGC,WZJSY,WZY,WZD,WZQT,WZZXH,QXHJ,QXBDGB,QXDFRY,QXFXJD,SBQXZ,SBYS,SBJX", "DW,ZJXJ,ZJZY,ZJSJ,ZJSJYS,ZJQZ,XYJYGD,XYJMLSJS,XYJSSZRK,XYJJQZ,XYJMSWC,XYJMSWR,XYZYSH,XYZYTF,XYZYQT,XYBMSY,XYJZJJXY", "DW,SZFWX,SZFWZ,SHMJXJ,SZRK,SWRK,SZRKR,ZYRK,DTFW,ZJJJZSS,SLSSZJJJSS" };//各表的字段，没有包含表5的
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "ExcelModel/someNewmoban.xls", FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet5 = hssfworkbook.GetSheet("表5");
            ISheet sheet9 = hssfworkbook.GetSheet("表9");
            sheet5 = HLTable5(sheet5, xmmList2, rpt, riverDic, unitInfo, riverInfo, deathReason);
            sheet9 = HLTable9(sheet9, xmmList1, rpt, tableFieldsArr[7], fieldDic);
            result = ResponseExcel(hssfworkbook, ordName, sTime, eTime);

            return result;
        }
        /// <summary>
        /// 导出所有报表
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="pageNO"></param>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string ExportAllTab(int limit, int pageNO, string unitCode, string ordName, string sTime, string eTime)
        {
            try
            {
                string result = "";
                Dictionary<string, string> riverDic = GetRiverData();
                string unitInfo = GetUnderUnitNames(unitCode, "HL01");
                string riverInfo = GetRiverDataByUnitCode(unitCode);
                string deathReason = getDiedReasonStr();
                IList<TB07_District> tb07List = getLowerUnits(unitCode, "HL01");
                IList<LZHL011> xmmList1 = GetLZHL01(pageNO, limit, tb07List); //表1-表4、以及表9
                IList<LZHL012> xmmList2 = GetLZHL02(pageNO, limit); //表5
                IList<LZHL013> xmmList3 = GetLZHL03(pageNO, limit); //表6
                IList<LZHL014> xmmList4 = GetLZHL04(pageNO, limit, tb07List); //表7-表8
                ReportTitle rpt = getRptInfo(pageNO);
                Dictionary<string, string> fieldDic = new Dictionary<string, string>();
                fieldDic = GetFieldsData(limit);
                string[] tableFieldsArr = new string[]
                {
                    "DW,SZFWX,SZFWZ,SZRK,SYCS,DTFW,SWRK,SZRKR,ZYRK,ZJJJZSS",
                    "DW,SHMJXJ,SHMJLS,CZMJXJ,CZMJLS,JSMJXJ,JSMJLS,YZJCLS,JJZWSS,SWDSC,SCYZMJ,SCYZSL,NLMYZJJJSS",
                    "DW,TCGKQY,TLZD,GLZD,JCGKGT,GDZD,TXZD,GJYSZJJJSS",
                    "DW,SHSKD,SHSKX,SKKBD,SKKBX1,SKKBX2,SHDFCS,SHDFCD,DFJKCS,DFJKCD,SHHAC,SHSZ,CHTB,SHGGSS,SHSWCZ,SHJDJ,SHJDBZ,SHSDZ,SLSSZJJJSS",
                    "DW,CSMC,YMFWMJ,YMFWBL,SZRK,SWRK,GCJSSJ,GCYMLS,GCLJJYL,GCHSWKRK,GCJJZYRK,ZYZJZDSS,SMXGS,SMXGD,SMXGQ,SMXJT,JZWSYFW,JZWSYDX,CQZJJJSS",
                    "DW,WZBZD,WZBZB,WZDSSS,WZSSL,WZMC,WZGC,WZJSY,WZY,WZD,WZQT,WZZXH,QXHJ,QXBDGB,QXDFRY,QXFXJD,SBQXZ,SBYS,SBJX",
                    "DW,ZJXJ,ZJZY,ZJSJ,ZJSJYS,ZJQZ,XYJYGD,XYJMLSJS,XYJSSZRK,XYJJQZ,XYJMSWC,XYJMSWR,XYZYSH,XYZYTF,XYZYQT,XYBMSY,XYJZJJXY",
                    "DW,SZFWX,SZFWZ,SHMJXJ,SZRK,SWRK,SZRKR,ZYRK,DTFW,ZJJJZSS,SLSSZJJJSS"
                }; //各表的字段，没有包含表5的

                #region 读取我们需要用到的xls模板

                //创建工作簿对象
                HSSFWorkbook hssfworkbook;
                string mobanName = "";
                if (unitCode.StartsWith("33")) //浙江
                {
                    mobanName = "ExcelModel/zjnewmoban.xls";
                    tableFieldsArr[3] =
                        "DW,SHSKD,SHSKX,SKKBD,SKKBX1,SKKBX2,SHDFCS,SHDFCD,DFJKCS,DFJKCD,SHHTCS,SHHTCD,HTJKCS,HTJKCD,SHHAC,SHSZ,CHTB,SHGGSS,SHSWCZ,SHJDJ,SHJDBZ,SHSDZ,SLSSZJJJSS";
                    //浙江的表4增加了四个字段

                }
                else
                {
                    mobanName = "ExcelModel/newmoban.xls";
                }
                using (
                    FileStream file = new FileStream(
                        System.AppDomain.CurrentDomain.BaseDirectory.ToString() + mobanName, FileMode.Open,
                        FileAccess.Read))
                {
                    //将文件流中模板加载到工作簿对象中
                    hssfworkbook = new HSSFWorkbook(file);
                }

                #endregion

                //建立一个名为Sheet1的工作表
                ISheet sheet1 = hssfworkbook.GetSheet("表1");
                ISheet sheet2 = hssfworkbook.GetSheet("表2");
                ISheet sheet3 = hssfworkbook.GetSheet("表3");
                ISheet sheet4 = hssfworkbook.GetSheet("表4");
                ISheet sheet5 = hssfworkbook.GetSheet("表5");
                ISheet sheet6 = hssfworkbook.GetSheet("表6");
                ISheet sheet7 = hssfworkbook.GetSheet("表7");
                ISheet sheet8 = hssfworkbook.GetSheet("表8");
                ISheet sheet9 = hssfworkbook.GetSheet("表9");

                sheet1 = HLTable1(sheet1, xmmList1, rpt, tableFieldsArr[0], fieldDic);
                sheet2 = HLTable2(sheet2, xmmList1, rpt, tableFieldsArr[1], fieldDic);
                sheet3 = HLTable3(sheet3, xmmList1, rpt, tableFieldsArr[2], fieldDic);
                if (unitCode.StartsWith("33")) //浙江
                {
                    sheet4 = ZJHLTable4(sheet4, xmmList1, rpt, tableFieldsArr[3], fieldDic);
                }
                else
                {
                    sheet4 = HLTable4(sheet4, xmmList1, rpt, tableFieldsArr[3], fieldDic);
                }

                sheet5 = HLTable5(sheet5, xmmList2, rpt, riverDic, unitInfo, riverInfo, deathReason);
                sheet6 = HLTable6(sheet6, xmmList3, rpt, unitInfo, riverInfo, riverDic, tableFieldsArr[4], fieldDic);
                //由于表5没有，所以是从4开始
                sheet7 = HLTable7(sheet7, xmmList4, rpt, tableFieldsArr[5], fieldDic);
                sheet8 = HLTable8(sheet8, xmmList4, rpt, tableFieldsArr[6], fieldDic);
                sheet9 = HLTable9(sheet9, xmmList1, rpt, tableFieldsArr[7], fieldDic);

                //强制Excel重新计算表中所有的公式
                result = ResponseExcel(hssfworkbook, ordName, sTime, eTime);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 导出数据到Excel

        #endregion 导出洪涝Excel

        #region 导出蓄水Excel

        public string ExportXuShuiTable(int limit, int pageNO, string unitCode, string ordName, string sTime, string eTime)
        {
            string result = "";

            string unitInfo = GetUnderUnitNames(unitCode, "HP01");

            IList<TB07_District> tb07List = getLowerUnits(unitCode, "HP01");
            IList<TB43_Reservoir> reservoirList = getReservoirs(unitCode); //该单位所有的水库
            string reservoirs = "";
            foreach (var tb43Reservoir in reservoirList)
            {
                reservoirs += tb43Reservoir.RSName + ",";
            }
            if (reservoirs != "")
            {
                reservoirs = reservoirs.Remove(reservoirs.Length - 1);
            }
            IList<LZHP011> xmmHPList1 = GetLZHP01(pageNO, limit, tb07List); //表1
            IList<LZHP012> xmmHPList2 = GetLZHP02(pageNO, limit, reservoirList); //

            IList<LZHP012> xmmBigHPList = GetBigXMMHP012(xmmHPList2);
            IList<LZHP012> xmmMiddleHPList = GetMiddleXMMHP012(xmmHPList2);
            ReportTitle rpt = getRptInfo(pageNO);
            Dictionary<int, string> hpDic = new Dictionary<int, string>();
            hpDic.Add(2, "蓄水量单位：亿立方米");
            hpDic.Add(3, "蓄水量单位：万立方米");
            hpDic.Add(4, "蓄水量单位：万立方米");
            Dictionary<string, string> fieldDic = new Dictionary<string, string>();
            fieldDic = GetFieldsData(limit);
            string[] tableFieldsArr = new string[]
            {
                "DW,XSCSZJ,YXSLZJ,XXSLZJ,XZKYSL,XZYBFB,SQZJ,LNZJ,DZXKCS,DZKYXSL,DZKXXSL,DXKKYSL,KXZYBFB,ZZXKCS,ZZKYXSL,ZZKXXSL,ZXKKYSL,ZXZYBFB",
                "DW,XYSKCS,XYKYXS,XYKXXS,XYKKYS,XKXZYBFB,XRSKCS,XRKYXS,XRKXXS,XRKKYS,XRXZYBFB,SPTHJCS,SPTYXS,SPTXXS,SPTKYS,TXZYBFB",
                "DXSKMC,DXKYXSL,DXKXXSL,DXKKYS,DXZYBFB,QNTQDXS", "DXSKMC,DXKYXSL,DXKXXSL,DXKKYS,DXZYBFB,QNTQDXS"
            };
            HSSFWorkbook hssfworkbook;
            using (
                FileStream file =
                    new FileStream(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "ExcelModel/xushui.xls",
                        FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ISheet sheet1 = hssfworkbook.GetSheet("表1");

            ISheet sheet2 = null;
            if (limit == 2)
            {
                sheet2 = hssfworkbook.GetSheet("表5");
            }
            else
            {
                sheet2 = hssfworkbook.GetSheet("表2");
            }
            ISheet sheet3 = hssfworkbook.GetSheet("表3");

            sheet1 = HPTable1(sheet1, xmmHPList1, rpt, limit, unitInfo, hpDic, tableFieldsArr[0], fieldDic);
            sheet2 = HPTable2(sheet2, xmmHPList1, xmmBigHPList, rpt, unitInfo, tableFieldsArr[1], fieldDic, limit, hpDic);

            int sheetCount = 4;
            if (limit == 2)
            {
                hssfworkbook.RemoveSheetAt(1);//删表2，表3，表4
                hssfworkbook.RemoveSheetAt(1);
                hssfworkbook.RemoveSheetAt(2);
                hssfworkbook.SetSheetName(1, "表2");//改表5为表2
                sheetCount = 3;
            }
            else
            {
                hssfworkbook.RemoveSheetAt(3);//删掉表5
                ISheet sheet4 = hssfworkbook.GetSheet("表4");
                sheet3 = HPTable3(sheet3, xmmBigHPList, rpt, tb07List.Count, reservoirs, tableFieldsArr[2], fieldDic, hpDic, limit);
                sheet4 = HPTable3(sheet4, xmmMiddleHPList, rpt, tb07List.Count, reservoirs, tableFieldsArr[3], fieldDic, hpDic, limit);
            }
            //for (int i = 1; i <= sheetCount; i++)//程序设置，只有一个sheet有效果。其他的没效果，但实际excel中已经设置了
            //{
            //    ISheet sheetPrint = hssfworkbook.GetSheet("表"+i);
            //    sheetPrint.PrintSetup.Landscape = true;
            //    sheetPrint.PrintSetup.Scale = 80;
            //}
            ordName = "蓄水";
            result = ResponseExcel(hssfworkbook, ordName, sTime, eTime);
            return result;
        }

        public string ExportNMXuShuiTable(int limit, int pageNO, string unitCode)
        {
            string result = "";

            //string unitInfo = GetUnderUnitNames(unitCode, "HP01");

            //IList<TB07_District> tb07List = getLowerUnits(unitCode, "HP01");
            //IList<TB62_NMReservoir> reservoirList = getNMReservoirs(unitCode, limit); //该单位所有的水库
            //string reservoirs = "";
            //foreach (var reservoir in reservoirList)
            //{
            //    reservoirs += reservoir.RSName + ",";
            //}
            //if (reservoirs != "")
            //{
            //    reservoirs = reservoirs.Remove(reservoirs.Length - 1);
            //}
            //IList<LZHP011> xmmHPList1 = GetLZNP01(pageNO, limit, tb07List); //表1
            IList<LZNP011> lznp011s = getNMUnitReservoirs(unitCode, limit);
            Dictionary<string, LZNP011> lznp011sDic = getNMReservoirsData(unitCode, limit, pageNO);
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "ExcelModel/nmxushui.xls",
                        FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            ReportTitle rpt = getRptInfo(pageNO);
            ISheet sheet1 = hssfworkbook.GetSheet("表1");
            sheet1 = NPTable1(sheet1, lznp011s, lznp011sDic, rpt, limit);
            result = ResponseExcel(hssfworkbook);
            return result;
        }

        #endregion 导出蓄水Excel

        #region 存入数据到Excel模板中

        #region  把数据分别存放到洪涝excel模板的9张表中
        public ISheet HLTable1(ISheet sheet1, IList<LZHL011> xmmhl011s, ReportTitle rpt, string fields, Dictionary<string, string> dic)
        {


            sheet1 = CreateCell(sheet1, xmmhl011s.Count, 10, 9);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 10; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }

            for (int i = 0; i < xmmhl011s.Count; i++)
            {
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl011s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl011s[i].SZFWX);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl011s[i].SZFWZ);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl011s[i].SZRK);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl011s[i].SYCS);

                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl011s[i].DTFW);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl011s[i].SWRK);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl011s[i].SZRKR);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl011s[i].ZYRK);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl011s[i].ZJJJZSS);

            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 0, 2);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 3, 4);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 5, 6);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 7, 9);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(5).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(5).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(7).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));

            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 10; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        public ISheet HLTable2(ISheet sheet1, IList<LZHL011> xmmhl011s, ReportTitle rpt, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhl011s.Count, 13, 9);//下表从0开始的,所以是13
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 13; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }

            for (int i = 0; i < xmmhl011s.Count; i++)
            {
                //sheet1.CreateRow(9 + i).CreateCell(0).SetCellValue(xmmhl011s[i].DW);
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl011s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl011s[i].SHMJXJ);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl011s[i].SHMJLS);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl011s[i].CZMJXJ);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl011s[i].CZMJLS);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl011s[i].JSMJXJ);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl011s[i].JSMJLS);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl011s[i].YZJCLS);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl011s[i].JJZWSS);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl011s[i].SWDSC);
                sheet1.GetRow(9 + i).GetCell(10).SetCellValue(xmmhl011s[i].SCYZMJ);
                sheet1.GetRow(9 + i).GetCell(11).SetCellValue(xmmhl011s[i].SCYZSL);
                sheet1.GetRow(9 + i).GetCell(12).SetCellValue(xmmhl011s[i].NLMYZJJJSS);
            }

            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 0, 1);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 3, 4);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 6, 7);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 10, 12);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(5).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(10).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));

            // 设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 13; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        public ISheet HLTable3(ISheet sheet1, IList<LZHL011> xmmhl011s, ReportTitle rpt, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhl011s.Count, 8, 9);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 8; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }
            for (int i = 0; i < xmmhl011s.Count; i++)
            {
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl011s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl011s[i].TCGKQY);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl011s[i].TLZD);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl011s[i].GLZD);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl011s[i].JCGKGT);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl011s[i].GDZD);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl011s[i].TXZD);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl011s[i].GJYSZJJJSS);
            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 0, 1);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 2, 3);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 4, 5);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 6, 7);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(5).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(2).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(4).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(6).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 8; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        public ISheet HLTable4(ISheet sheet1, IList<LZHL011> xmmhl011s, ReportTitle rpt, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhl011s.Count, 19, 9);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 19; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }
            for (int i = 0; i < xmmhl011s.Count; i++)
            {
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl011s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl011s[i].SHSKD);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl011s[i].SHSKX);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl011s[i].SKKBD);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl011s[i].SKKBX1);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl011s[i].SKKBX2);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl011s[i].SHDFCS);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl011s[i].SHDFCD);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl011s[i].DFJKCS);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl011s[i].DFJKCD);
                sheet1.GetRow(9 + i).GetCell(10).SetCellValue(xmmhl011s[i].SHHAC);
                sheet1.GetRow(9 + i).GetCell(11).SetCellValue(xmmhl011s[i].SHSZ);
                sheet1.GetRow(9 + i).GetCell(12).SetCellValue(xmmhl011s[i].CHTB);
                sheet1.GetRow(9 + i).GetCell(13).SetCellValue(xmmhl011s[i].SHGGSS);
                sheet1.GetRow(9 + i).GetCell(14).SetCellValue(xmmhl011s[i].SHSWCZ);
                sheet1.GetRow(9 + i).GetCell(15).SetCellValue(xmmhl011s[i].SHJDJ);
                sheet1.GetRow(9 + i).GetCell(16).SetCellValue(xmmhl011s[i].SHJDBZ);
                sheet1.GetRow(9 + i).GetCell(17).SetCellValue(xmmhl011s[i].SHSDZ);
                sheet1.GetRow(9 + i).GetCell(18).SetCellValue(xmmhl011s[i].SLSSZJJJSS);
            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 0, 3);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 5, 8);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 10, 13);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 15, 18);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(5).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(5).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(10).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(15).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 19; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        public ISheet ZJHLTable4(ISheet sheet1, IList<LZHL011> xmmhl011s, ReportTitle rpt, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhl011s.Count, 23, 9);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 23; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }
            for (int i = 0; i < xmmhl011s.Count; i++)
            {
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl011s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl011s[i].SHSKD);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl011s[i].SHSKX);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl011s[i].SKKBD);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl011s[i].SKKBX1);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl011s[i].SKKBX2);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl011s[i].SHDFCS);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl011s[i].SHDFCD);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl011s[i].DFJKCS);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl011s[i].DFJKCD);

                sheet1.GetRow(9 + i).GetCell(10).SetCellValue(xmmhl011s[i].SHHTCS);
                sheet1.GetRow(9 + i).GetCell(11).SetCellValue(xmmhl011s[i].SHHTCD);
                sheet1.GetRow(9 + i).GetCell(12).SetCellValue(xmmhl011s[i].HTJKCS);
                sheet1.GetRow(9 + i).GetCell(13).SetCellValue(xmmhl011s[i].HTJKCD);

                sheet1.GetRow(9 + i).GetCell(14).SetCellValue(xmmhl011s[i].SHHAC);
                sheet1.GetRow(9 + i).GetCell(15).SetCellValue(xmmhl011s[i].SHSZ);
                sheet1.GetRow(9 + i).GetCell(16).SetCellValue(xmmhl011s[i].CHTB);
                sheet1.GetRow(9 + i).GetCell(17).SetCellValue(xmmhl011s[i].SHGGSS);
                sheet1.GetRow(9 + i).GetCell(18).SetCellValue(xmmhl011s[i].SHSWCZ);
                sheet1.GetRow(9 + i).GetCell(19).SetCellValue(xmmhl011s[i].SHJDJ);
                sheet1.GetRow(9 + i).GetCell(20).SetCellValue(xmmhl011s[i].SHJDBZ);
                sheet1.GetRow(9 + i).GetCell(21).SetCellValue(xmmhl011s[i].SHSDZ);
                sheet1.GetRow(9 + i).GetCell(22).SetCellValue(xmmhl011s[i].SLSSZJJJSS);
            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 0, 3);//合并单元格

            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 5, 8);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 10, 13);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 15, 18);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(5).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(5).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(10).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(15).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 23; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        /// <summary>
        /// 死亡人口 特殊处理
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="xmmhl012s"></param>
        /// <param name="rpt"></param>
        /// <returns></returns>
        public ISheet HLTable5(ISheet sheet1, IList<LZHL012> xmmhl012s, ReportTitle rpt, Dictionary<string, string> riverDic, string unitInfo, string riverInfo, string deathReason)
        {
            //ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");
            sheet1 = CreateCell(sheet1, xmmhl012s.Count, 11, 9);
            //下级单位
            CellRangeAddressList regions = new CellRangeAddressList(9, 65535, 0, 0);
            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(unitInfo.Split(','));
            HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
            ((HSSFSheet)sheet1).AddValidationData(dataValidate);

            //设置类别下拉框
            regions = new CellRangeAddressList(9, 65535, 1, 1);
            constraint = DVConstraint.CreateExplicitListConstraint(new string[] { "死亡", "失踪" });
            dataValidate = new HSSFDataValidation(regions, constraint);
            ((HSSFSheet)sheet1).AddValidationData(dataValidate);

            //设置性别下拉框
            regions = new CellRangeAddressList(9, 65535, 3, 3);
            constraint = DVConstraint.CreateExplicitListConstraint(new string[] { "未知", "男", "女" });
            dataValidate = new HSSFDataValidation(regions, constraint);
            ((HSSFSheet)sheet1).AddValidationData(dataValidate);

            ////死亡原因(模板中已经设置了，并且如果超过了255个字符，不能直接这样处理)
            //regions = new CellRangeAddressList(0, 65535, 7, 7);
            //constraint = DVConstraint.CreateExplicitListConstraint(deathReason.Split(','));
            //dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);

            //流域
            regions = new CellRangeAddressList(0, 65535, 10, 10);
            constraint = DVConstraint.CreateExplicitListConstraint(riverInfo.Split(','));
            dataValidate = new HSSFDataValidation(regions, constraint);
            ((HSSFSheet)sheet1).AddValidationData(dataValidate);

            string dw = "";
            for (int i = 0; i < xmmhl012s.Count; i++)
            {
                dw = new LogicProcessingClass.Tools().GetNameByUnitCode(xmmhl012s[i].UnitCode);
                string rivercode = xmmhl012s[i].RiverCode;
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl012s[i].DW == "" ? dw : xmmhl012s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl012s[i].DataType);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl012s[i].SWXM);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl012s[i].SWXB);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl012s[i].SWNL);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl012s[i].SWHJ);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl012s[i].SWSJ);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl012s[i].SWDD);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl012s[i].DeathReason);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl012s[i].BZ.Replace("{/r-/n}", ""));

                string river = rivercode.Trim() == "" ? "" : riverDic[rivercode.Trim()];
                sheet1.GetRow(9 + i).GetCell(10).SetCellValue(river); //流域名称r

            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl012s.Count, 9 + xmmhl012s.Count, 1, 3);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl012s.Count, 9 + xmmhl012s.Count, 4, 5);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl012s.Count, 9 + xmmhl012s.Count, 6, 7);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl012s.Count, 9 + xmmhl012s.Count, 8, 9);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(5).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(6).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl012s.Count).GetCell(1).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl012s.Count).GetCell(4).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl012s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl012s.Count).GetCell(8).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl012s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 11; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        /// <summary>
        /// 受灾城市 特殊处理
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="xmmhl013s"></param>
        /// <param name="rpt"></param>
        /// <returns></returns>
        public ISheet HLTable6(ISheet sheet1, IList<LZHL013> xmmhl013s, ReportTitle rpt, string unitInfo, string riverInfo, Dictionary<string, string> riverDic, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhl013s.Count, 20, 9);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 19; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }

            //下级单位
            CellRangeAddressList regions = new CellRangeAddressList(9, 65535, 0, 0);
            DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(unitInfo.Split(','));
            HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
            ((HSSFSheet)sheet1).AddValidationData(dataValidate);


            //流域
            regions = new CellRangeAddressList(9, 65535, 19, 19);
            constraint = DVConstraint.CreateExplicitListConstraint(riverInfo.Split(','));
            dataValidate = new HSSFDataValidation(regions, constraint);
            ((HSSFSheet)sheet1).AddValidationData(dataValidate);
            for (int i = 0; i < xmmhl013s.Count; i++)
            {
                string rivercode = xmmhl013s[i].RiverCode;
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl013s[i].DW);  //有标题

                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl013s[i].CSMC);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl013s[i].YMFWMJ);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl013s[i].YMFWBL);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl013s[i].SZRK);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl013s[i].SWRK);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl013s[i].GCJSSJ);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl013s[i].GCYMLS);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl013s[i].GCLJJYL);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl013s[i].GCHSWKRK);
                sheet1.GetRow(9 + i).GetCell(10).SetCellValue(xmmhl013s[i].GCJJZYRK);
                sheet1.GetRow(9 + i).GetCell(11).SetCellValue(xmmhl013s[i].ZYZJZDSS);
                sheet1.GetRow(9 + i).GetCell(12).SetCellValue(xmmhl013s[i].SMXGS);
                sheet1.GetRow(9 + i).GetCell(13).SetCellValue(xmmhl013s[i].SMXGD);
                sheet1.GetRow(9 + i).GetCell(14).SetCellValue(xmmhl013s[i].SMXGQ);
                sheet1.GetRow(9 + i).GetCell(15).SetCellValue(xmmhl013s[i].SMXJT);
                sheet1.GetRow(9 + i).GetCell(16).SetCellValue(xmmhl013s[i].JZWSYFW);
                sheet1.GetRow(9 + i).GetCell(17).SetCellValue(xmmhl013s[i].JZWSYDX);
                sheet1.GetRow(9 + i).GetCell(18).SetCellValue(xmmhl013s[i].CQZJJJSS);
                //if (rivercode == null || rivercode.ToString().Trim().Length == 0&& i ==0)
                //{
                //    sheet1.GetRow(9 + i).GetCell(19).SetCellValue("合计");
                //}
                //else
                if (rivercode.ToString().Trim().Length > 0 && riverDic.ContainsKey(rivercode.ToString().Trim()))
                {
                    sheet1.GetRow(9 + i).GetCell(19).SetCellValue(riverDic[rivercode.ToString().Trim()]);
                }
                else
                {
                    sheet1.GetRow(9 + i).GetCell(19).SetCellValue("");
                }

            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl013s.Count, 9 + xmmhl013s.Count, 1, 4);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl013s.Count, 9 + xmmhl013s.Count, 6, 10);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl013s.Count, 9 + xmmhl013s.Count, 11, 14);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl013s.Count, 9 + xmmhl013s.Count, 15, 18);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(5).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl013s.Count).GetCell(1).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl013s.Count).GetCell(6).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl013s.Count).GetCell(11).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl013s.Count).GetCell(15).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl013s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 20; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        public ISheet HLTable7(ISheet sheet1, IList<LZHL014> xmmhl014s, ReportTitle rpt, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhl014s.Count, 19, 9);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 19; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }
            for (int i = 0; i < xmmhl014s.Count; i++)
            {
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl014s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl014s[i].WZBZD);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl014s[i].WZBZB);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl014s[i].WZDSSS);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl014s[i].WZSSL);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl014s[i].WZMC);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl014s[i].WZGC);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl014s[i].WZJSY);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl014s[i].WZY);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl014s[i].WZD);
                sheet1.GetRow(9 + i).GetCell(10).SetCellValue(xmmhl014s[i].WZQT);
                sheet1.GetRow(9 + i).GetCell(11).SetCellValue(xmmhl014s[i].WZZXH);
                sheet1.GetRow(9 + i).GetCell(12).SetCellValue(xmmhl014s[i].QXHJ);
                sheet1.GetRow(9 + i).GetCell(13).SetCellValue(xmmhl014s[i].QXBDGB);
                sheet1.GetRow(9 + i).GetCell(14).SetCellValue(xmmhl014s[i].QXDFRY);
                sheet1.GetRow(9 + i).GetCell(15).SetCellValue(xmmhl014s[i].QXFXJD);
                sheet1.GetRow(9 + i).GetCell(16).SetCellValue(xmmhl014s[i].SBQXZ);
                sheet1.GetRow(9 + i).GetCell(17).SetCellValue(xmmhl014s[i].SBYS);
                sheet1.GetRow(9 + i).GetCell(18).SetCellValue(xmmhl014s[i].SBJX);
            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl014s.Count, 9 + xmmhl014s.Count, 0, 2);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl014s.Count, 9 + xmmhl014s.Count, 4, 7);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl014s.Count, 9 + xmmhl014s.Count, 10, 13);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl014s.Count, 9 + xmmhl014s.Count, 15, 18);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(3).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(4).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl014s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl014s.Count).GetCell(4).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl014s.Count).GetCell(10).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl014s.Count).GetCell(15).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl014s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 19; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        public ISheet HLTable8(ISheet sheet1, IList<LZHL014> xmmhl014s, ReportTitle rpt, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhl014s.Count, 17, 9);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 17; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }
            for (int i = 0; i < xmmhl014s.Count; i++)
            {
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl014s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl014s[i].ZJXJ);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl014s[i].ZJZY);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl014s[i].ZJSJ);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl014s[i].ZJSJYS);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl014s[i].ZJQZ);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl014s[i].XYJYGD);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl014s[i].XYJMLSJS);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl014s[i].XYJSSZRK);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl014s[i].XYJJQZ);
                sheet1.GetRow(9 + i).GetCell(10).SetCellValue(xmmhl014s[i].XYJMSWC);
                sheet1.GetRow(9 + i).GetCell(11).SetCellValue(xmmhl014s[i].XYJMSWR);
                sheet1.GetRow(9 + i).GetCell(12).SetCellValue(xmmhl014s[i].XYZYSH);
                sheet1.GetRow(9 + i).GetCell(13).SetCellValue(xmmhl014s[i].XYZYTF);
                sheet1.GetRow(9 + i).GetCell(14).SetCellValue(xmmhl014s[i].XYZYQT);
                sheet1.GetRow(9 + i).GetCell(15).SetCellValue(xmmhl014s[i].XYBMSY);
                sheet1.GetRow(9 + i).GetCell(16).SetCellValue(xmmhl014s[i].XYJZJJXY);

            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl014s.Count, 9 + xmmhl014s.Count, 0, 2);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl014s.Count, 9 + xmmhl014s.Count, 4, 6);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl014s.Count, 9 + xmmhl014s.Count, 8, 11);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl014s.Count, 9 + xmmhl014s.Count, 13, 16);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(3).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(4).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl014s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl014s.Count).GetCell(4).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl014s.Count).GetCell(8).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl014s.Count).GetCell(13).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl014s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 17; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        public ISheet HLTable9(ISheet sheet1, IList<LZHL011> xmmhl011s, ReportTitle rpt, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhl011s.Count, 11, 9);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 11; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(8).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }
            for (int i = 0; i < xmmhl011s.Count; i++)
            {
                sheet1.GetRow(9 + i).GetCell(0).SetCellValue(xmmhl011s[i].DW);
                sheet1.GetRow(9 + i).GetCell(1).SetCellValue(xmmhl011s[i].SZFWX);
                sheet1.GetRow(9 + i).GetCell(2).SetCellValue(xmmhl011s[i].SZFWZ);
                sheet1.GetRow(9 + i).GetCell(3).SetCellValue(xmmhl011s[i].SHMJXJ);
                sheet1.GetRow(9 + i).GetCell(4).SetCellValue(xmmhl011s[i].SZRK);
                sheet1.GetRow(9 + i).GetCell(5).SetCellValue(xmmhl011s[i].SWRK);
                sheet1.GetRow(9 + i).GetCell(6).SetCellValue(xmmhl011s[i].SZRKR);
                sheet1.GetRow(9 + i).GetCell(7).SetCellValue(xmmhl011s[i].ZYRK);
                sheet1.GetRow(9 + i).GetCell(8).SetCellValue(xmmhl011s[i].DTFW);
                sheet1.GetRow(9 + i).GetCell(9).SetCellValue(xmmhl011s[i].ZJJJZSS);
                sheet1.GetRow(9 + i).GetCell(10).SetCellValue(xmmhl011s[i].SLSSZJJJSS);

            }
            //填写表头信息
            CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 0, 2);//合并单元格
            CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 3, 5);
            CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 6, 7);
            CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhl011s.Count, 9 + xmmhl011s.Count, 8, 10);
            sheet1.AddMergedRegion(cellRangeAddress);
            sheet1.AddMergedRegion(cellRangeAddress1);
            sheet1.AddMergedRegion(cellRangeAddress2);
            sheet1.AddMergedRegion(cellRangeAddress3);

            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(5).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(9 + xmmhl011s.Count).GetCell(8).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 9; x < 9 + xmmhl011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 11; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }
        #endregion 把数据分别存放到洪涝excel模板的9张表中

        #region 存入蓄水的Excel模板的4张表中
        public ISheet HPTable1(ISheet sheet1, IList<LZHP011> xmmhp011s, ReportTitle rpt, int limit, string unitInfo, Dictionary<int, string> hpDic, string fields, Dictionary<string, string> dic)
        {
            sheet1 = CreateCell(sheet1, xmmhp011s.Count, 18, 6);
            sheet1.GetRow(1).GetCell(14).SetCellValue(hpDic[limit]);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 17; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(5).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }
            ////下级单位
            //CellRangeAddressList regions = new CellRangeAddressList(5, 65535, 0, 0);
            //DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(unitInfo.Split(','));
            //HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);

            for (int i = 0; i < xmmhp011s.Count; i++)//18个cell 从第6行开始 
            {
                sheet1.GetRow(6 + i).GetCell(0).SetCellValue(xmmhp011s[i].DW);
                sheet1.GetRow(6 + i).GetCell(1).SetCellValue(xmmhp011s[i].XSCSZJ);
                sheet1.GetRow(6 + i).GetCell(2).SetCellValue(xmmhp011s[i].YXSLZJ);
                sheet1.GetRow(6 + i).GetCell(3).SetCellValue(xmmhp011s[i].XXSLZJ);
                sheet1.GetRow(6 + i).GetCell(4).SetCellValue(xmmhp011s[i].XZKYSL);
                sheet1.GetRow(6 + i).GetCell(5).SetCellValue(xmmhp011s[i].XZYBFB);
                sheet1.GetRow(6 + i).GetCell(6).SetCellValue(xmmhp011s[i].SQZJ);
                sheet1.GetRow(6 + i).GetCell(7).SetCellValue(xmmhp011s[i].LNZJ);
                sheet1.GetRow(6 + i).GetCell(8).SetCellValue(xmmhp011s[i].DZXKCS);
                sheet1.GetRow(6 + i).GetCell(9).SetCellValue(xmmhp011s[i].DZKYXSL);
                sheet1.GetRow(6 + i).GetCell(10).SetCellValue(xmmhp011s[i].DZKXXSL);
                sheet1.GetRow(6 + i).GetCell(11).SetCellValue(xmmhp011s[i].DXKKYSL);
                sheet1.GetRow(6 + i).GetCell(12).SetCellValue(xmmhp011s[i].KXZYBFB);
                sheet1.GetRow(6 + i).GetCell(13).SetCellValue(xmmhp011s[i].ZZXKCS);
                sheet1.GetRow(6 + i).GetCell(14).SetCellValue(xmmhp011s[i].ZZKYXSL);
                sheet1.GetRow(6 + i).GetCell(15).SetCellValue(xmmhp011s[i].ZZKXXSL);
                sheet1.GetRow(6 + i).GetCell(16).SetCellValue(xmmhp011s[i].ZXKKYSL);
                sheet1.GetRow(6 + i).GetCell(17).SetCellValue(xmmhp011s[i].ZXZYBFB);
            }
            //填写表头信息 
            //CellRangeAddress cellRangeAddress = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 0, 2);//合并单元格
            //CellRangeAddress cellRangeAddress1 = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 3, 7);
            //CellRangeAddress cellRangeAddress2 = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 8, 11);
            //sheet1.AddMergedRegion(cellRangeAddress);
            //sheet1.AddMergedRegion(cellRangeAddress1);
            //sheet1.AddMergedRegion(cellRangeAddress2);
            DateTime dt = Convert.ToDateTime(rpt.WriterTime);
            sheet1.GetRow(1).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName + "                 填报日期：" + dt.Year + "年" + dt.Month + "月" + dt.Day + "日");
            if (limit == 2)//省级的单位不同
            {
                sheet1.GetRow(1).GetCell(13).SetCellValue("蓄水量单位：亿立方米");
            }
            else
            {
                sheet1.GetRow(1).GetCell(13).SetCellValue("蓄水量单位：万立方米");
            }

            //sheet1.GetRow(5).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            //sheet1.GetRow(4 + xmmhp011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            //sheet1.GetRow(4 + xmmhp011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            //sheet1.GetRow(4 + xmmhp011s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            //sheet1.GetRow(4 + xmmhp011s.Count).GetCell(8).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 6; x < 6 + xmmhp011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 18; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            //sheet1.PrintSetup.Landscape = true;
            //sheet1.PrintSetup.Scale = 80;
            //PrintSetup(sheet1);
            return sheet1;
        }

        public ISheet HPTable2(ISheet sheet1, IList<LZHP011> xmmhp011s, IList<LZHP012> xmmhp012s, ReportTitle rpt, string unitInfo, string fields, Dictionary<string, string> dic, int limit, Dictionary<int, string> hpDic)
        {
            sheet1.GetRow(1).GetCell(13).SetCellValue(hpDic[limit]);
            int cellCount = 0;
            if (limit == 2)
            {
                cellCount = 22;
                xmmhp012s = xmmhp012s.Where(t => t.DXSKMC != "合计").ToList<LZHP012>();
            }
            else
            {
                cellCount = 16;
            }
            sheet1 = CreateCell(sheet1, xmmhp011s.Count, cellCount, 6);
            string[] fieldsArr = fields.Split(',');
            //for (int j = 1; j < 15; j++)
            //{
            //    if (dic.ContainsKey(fieldsArr[j]))
            //    {
            //        sheet2.GetRow(5).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
            //    }
            //}
            ////下级单位
            //CellRangeAddressList regions = new CellRangeAddressList(5, 65535, 0, 0);
            //DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(unitInfo.Split(','));
            //HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);

            for (int i = 0; i < xmmhp011s.Count; i++)//16个cell
            {
                sheet1.GetRow(6 + i).GetCell(0).SetCellValue(xmmhp011s[i].DW);
                sheet1.GetRow(6 + i).GetCell(1).SetCellValue(xmmhp011s[i].XYSKCS);
                sheet1.GetRow(6 + i).GetCell(2).SetCellValue(xmmhp011s[i].XYKYXS);
                sheet1.GetRow(6 + i).GetCell(3).SetCellValue(xmmhp011s[i].XYKXXS);
                sheet1.GetRow(6 + i).GetCell(4).SetCellValue(xmmhp011s[i].XYKKYS);
                sheet1.GetRow(6 + i).GetCell(5).SetCellValue(xmmhp011s[i].XKXZYBFB);
                sheet1.GetRow(6 + i).GetCell(6).SetCellValue(xmmhp011s[i].XRSKCS);
                sheet1.GetRow(6 + i).GetCell(7).SetCellValue(xmmhp011s[i].XRKYXS);
                sheet1.GetRow(6 + i).GetCell(8).SetCellValue(xmmhp011s[i].XRKXXS);
                sheet1.GetRow(6 + i).GetCell(9).SetCellValue(xmmhp011s[i].XRKKYS);
                sheet1.GetRow(6 + i).GetCell(10).SetCellValue(xmmhp011s[i].XRXZYBFB);
                sheet1.GetRow(6 + i).GetCell(11).SetCellValue(xmmhp011s[i].SPTHJCS);
                sheet1.GetRow(6 + i).GetCell(12).SetCellValue(xmmhp011s[i].SPTYXS);
                sheet1.GetRow(6 + i).GetCell(13).SetCellValue(xmmhp011s[i].SPTXXS);
                sheet1.GetRow(6 + i).GetCell(14).SetCellValue(xmmhp011s[i].SPTKYS);
                sheet1.GetRow(6 + i).GetCell(15).SetCellValue(xmmhp011s[i].TXZYBFB);
                if (limit == 2)
                {
                    if (i < xmmhp012s.Count)
                    {
                        sheet1.GetRow(6 + i).GetCell(16).SetCellValue(xmmhp012s[i].DXSKMC);
                        sheet1.GetRow(6 + i).GetCell(17).SetCellValue(xmmhp012s[i].DXKYXSL);
                        sheet1.GetRow(6 + i).GetCell(18).SetCellValue(xmmhp012s[i].DXKXXSL);
                        sheet1.GetRow(6 + i).GetCell(19).SetCellValue(xmmhp012s[i].DXKKYS);
                        sheet1.GetRow(6 + i).GetCell(20).SetCellValue(xmmhp012s[i].DXZYBFB);
                        sheet1.GetRow(6 + i).GetCell(21).SetCellValue(xmmhp012s[i].QNTQDXS);
                    }
                }
            }
            //填写表头信息
            //CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 0, 2);//合并单元格
            //CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 3, 5);
            //CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 6, 7);
            //CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 8, 10);
            //sheet1.AddMergedRegion(cellRangeAddress);
            //sheet1.AddMergedRegion(cellRangeAddress1);
            //sheet1.AddMergedRegion(cellRangeAddress2);
            //sheet1.AddMergedRegion(cellRangeAddress3);
            sheet1.GetRow(1).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName + "                 填报日期：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy-MM-dd"));
            //sheet1.GetRow(1).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            //sheet1.GetRow(1).GetCell(4).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(8).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 6; x < 6 + xmmhp011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < cellCount; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            //PrintSetup(sheet1);
            //sheet2.PrintSetup.Landscape = true;
            //sheet2.PrintSetup.Scale = 80;
            return sheet1;
        }

        /// <summary>导出表三大型水库数据
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="xmmhp012s">只包含中型水库数据的XMMHP012对象集合</param>
        /// <param name="rpt"></param>
        /// <param name="unitCount">当前单位的下级单位个数</param>
        /// <returns></returns>
        public ISheet HPTable3(ISheet sheet1, IList<LZHP012> xmmhp012s, ReportTitle rpt, int unitCount, string shuiku, string fields, Dictionary<string, string> dic, Dictionary<int, string> hpDic, int limit)
        {
            sheet1.GetRow(1).GetCell(13).SetCellValue(hpDic[limit]);
            sheet1 = CreateCell(sheet1, unitCount, 25, 6);
            unitCount = unitCount + 1;
            string[] fieldsArr = fields.Split(',');
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (dic.ContainsKey(fieldsArr[j]))
                    {
                        sheet1.GetRow(5).GetCell(j + i * 6).SetCellValue(dic[fieldsArr[j]]);
                    }
                }
            }

            ////水库
            //CellRangeAddressList regions = new CellRangeAddressList(5, 65535, 0, 0);
            //DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(shuiku.Split(','));
            //HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);

            //regions = new CellRangeAddressList(5, 65535, 6, 6);
            //constraint = DVConstraint.CreateExplicitListConstraint(shuiku.Split(','));
            //dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);


            //regions = new CellRangeAddressList(5, 65535, 12, 12);
            //constraint = DVConstraint.CreateExplicitListConstraint(shuiku.Split(','));
            //dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);

            for (int i = 0; i < xmmhp012s.Count; i++)//
            {
                int num = i / unitCount;
                sheet1.GetRow(6 + i - num * unitCount).GetCell(0 + num * 6).SetCellValue(xmmhp012s[i].DXSKMC);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(1 + num * 6).SetCellValue(xmmhp012s[i].DXKYXSL);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(2 + num * 6).SetCellValue(xmmhp012s[i].DXKXXSL);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(3 + num * 6).SetCellValue(xmmhp012s[i].DXKKYS);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(4 + num * 6).SetCellValue(xmmhp012s[i].DXZYBFB);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(5 + num * 6).SetCellValue(xmmhp012s[i].QNTQDXS);

            }
            //填写表头信息
            //CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 0, 2);//合并单元格
            //CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 3, 5);
            //CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 6, 7);
            //CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 8, 10);
            //sheet1.AddMergedRegion(cellRangeAddress);
            //sheet1.AddMergedRegion(cellRangeAddress1);
            //sheet1.AddMergedRegion(cellRangeAddress2);
            //sheet1.AddMergedRegion(cellRangeAddress3);
            sheet1.GetRow(1).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName + "                 填报日期：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy-MM-dd"));
            //sheet1.GetRow(1).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            //sheet1.GetRow(1).GetCell(4).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(8).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 6; x < 6 + unitCount; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 18; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            //PrintSetup(sheet1);
            //sheet1.PrintSetup.Landscape = true;
            //sheet1.PrintSetup.Scale = 80;
            return sheet1;
        }
        /// <summary>导出表四中型水库数据
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="xmmhp012s">只包含中型水库数据的XMMHP012对象集合</param>
        /// <param name="rpt"></param>
        /// <param name="unitCount">当前单位的下级单位个数</param>
        /// <returns></returns>
        public ISheet HPTable4(ISheet sheet1, IList<LZHP012> xmmhp012s, ReportTitle rpt, int unitCount, string shuiku, string fields, Dictionary<string, string> dic, Dictionary<int, string> hpDic, int limit)
        {
            sheet1.GetRow(1).GetCell(13).SetCellValue(hpDic[limit]);
            sheet1 = CreateCell(sheet1, unitCount, 25, 6);
            unitCount = unitCount + 1;
            string[] fieldsArr = fields.Split(',');
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (dic.ContainsKey(fieldsArr[j]))
                    {
                        sheet1.GetRow(5).GetCell(j + i * 6).SetCellValue(dic[fieldsArr[j]]);
                    }
                }
            }
            ////水库
            //CellRangeAddressList regions = new CellRangeAddressList(5, 65535, 0, 0);
            //DVConstraint constraint = DVConstraint.CreateExplicitListConstraint(shuiku.Split(','));
            //HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);

            //regions = new CellRangeAddressList(5, 65535, 6, 6);
            //constraint = DVConstraint.CreateExplicitListConstraint(shuiku.Split(','));
            //dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);

            //regions = new CellRangeAddressList(5, 65535, 12, 12);
            //constraint = DVConstraint.CreateExplicitListConstraint(shuiku.Split(','));
            //dataValidate = new HSSFDataValidation(regions, constraint);
            //((HSSFSheet)sheet1).AddValidationData(dataValidate);

            for (int i = 0; i < xmmhp012s.Count; i++)//13个cell
            {
                if (xmmhp012s[i].DistributeRate == "2")
                {

                }
                int num = i / unitCount;
                sheet1.GetRow(6 + i - num * unitCount).GetCell(0 + num * 6).SetCellValue(xmmhp012s[i].DXSKMC);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(1 + num * 6).SetCellValue(xmmhp012s[i].DXKYXSL);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(2 + num * 6).SetCellValue(xmmhp012s[i].DXKXXSL);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(3 + num * 6).SetCellValue(xmmhp012s[i].DXKKYS);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(4 + num * 6).SetCellValue(xmmhp012s[i].DXZYBFB);
                sheet1.GetRow(6 + i - num * unitCount).GetCell(5 + num * 6).SetCellValue(xmmhp012s[i].QNTQDXS);

            }
            //填写表头信息
            //CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 0, 2);//合并单元格
            //CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 3, 5);
            //CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 6, 7);
            //CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + xmmhp011s.Count, 9 + xmmhp011s.Count, 8, 10);
            //sheet1.AddMergedRegion(cellRangeAddress);
            //sheet1.AddMergedRegion(cellRangeAddress1);
            //sheet1.AddMergedRegion(cellRangeAddress2);
            //sheet1.AddMergedRegion(cellRangeAddress3);
            sheet1.GetRow(1).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName + "                 填报日期：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy-MM-dd"));
            //sheet1.GetRow(1).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            //sheet1.GetRow(1).GetCell(4).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            //sheet1.GetRow(9 + xmmhp011s.Count).GetCell(8).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 6; x < 6 + unitCount; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 18; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            //PrintSetup(sheet1);
            //sheet1.PrintSetup.Landscape = true;
            //sheet1.PrintSetup.Scale = 80;
            return sheet1;
        }

        #endregion 存入蓄水的Excel模板的3张表中

        #region 存入内蒙蓄水表中
        public ISheet NPTable1(ISheet sheet1, IList<LZNP011> lznp011s, Dictionary<string, LZNP011> lznp011sDic, ReportTitle rpt, int limit)
        {
            int count = lznp011s.Count;
            int i = 0;

            if (limit < 4)
            {
                count++;
                i++;
            }

            sheet1 = CreateCell(sheet1, count, 11, 7);

            BusinessEntities business = null;
            string sliceCode = null;
            double dqxsl;
            if (limit < 4)
            {
                string unitcode = System.Web.HttpContext.Current.Request["unitcode"];
                var hj = fxdict.TB62_NMReservoir.Where(t => t.UnitCode == unitcode).SingleOrDefault();
                if (hj != null)
                {
                    sheet1.GetRow(7).GetCell(1).SetCellValue(hj.RSName);
                    sheet1.GetRow(7).GetCell(2).SetCellValue(hj.UnitName);
                    sheet1.GetRow(7).GetCell(3).SetCellValue(hj.ZKR.ToString());
                    sheet1.GetRow(7).GetCell(4).SetCellValue(hj.SKR);

                    Entities getEntity = new Entities();
                    business = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(2);
                    sliceCode = limit == 3 ? unitcode.Substring(0, 4) : unitcode.Substring(0, 2);
                    dqxsl =
                        Convert.ToDouble(
                            business.NP011.Where(t => t.PageNO == rpt.PageNO && t.UnitCode.StartsWith(sliceCode))
                                .Sum(t => t.DQXSL));
                    if (dqxsl > 0)
                    {
                        sheet1.GetRow(7).GetCell(9).SetCellValue(dqxsl.ToString("0.000"));
                    }
                }
            }

            for (int j = 0; j < lznp011s.Count; j++)
            {
                if (limit == 2 && lznp011s[j].RSCode == "0")
                {
                    sliceCode = lznp011s[j].UnitCode.Substring(0, 4);
                    dqxsl =
                        Convert.ToDouble(
                            business.NP011.Where(
                                t => t.PageNO == rpt.PageNO && t.UnitCode.StartsWith(sliceCode))
                                .Sum(t => t.DQXSL));
                    if (dqxsl > 0)
                    {
                        sheet1.GetRow(7 + j + i).GetCell(9).SetCellValue(dqxsl.ToString("0.000"));
                    }
                }

                sheet1.GetRow(7 + j + i).GetCell(0).SetCellValue(lznp011s[j].TBNO);
                sheet1.GetRow(7 + j + i).GetCell(1).SetCellValue(lznp011s[j].RSName);
                sheet1.GetRow(7 + j + i).GetCell(2).SetCellValue(lznp011s[j].UnitName);
                sheet1.GetRow(7 + j + i).GetCell(3).SetCellValue(lznp011s[j].ZKR);
                sheet1.GetRow(7 + j + i).GetCell(4).SetCellValue(lznp011s[j].SKR);
                sheet1.GetRow(7 + j + i).GetCell(5).SetCellValue(lznp011s[j].SSW);
                sheet1.GetRow(7 + j + i).GetCell(6).SetCellValue(lznp011s[j].XXSW);

                sheet1.GetRow(7 + j + i).GetCell(7).SetCellValue(lznp011s[j].ZCXSW);
                if (lznp011sDic.ContainsKey(lznp011s[j].RSCode))
                {
                    sheet1.GetRow(7 + j + i).GetCell(8).SetCellValue(lznp011sDic[lznp011s[j].RSCode].DQSW);
                    sheet1.GetRow(7 + j + i).GetCell(9).SetCellValue(lznp011sDic[lznp011s[j].RSCode].DQXSL);
                    sheet1.GetRow(7 + j + i).GetCell(10).SetCellValue(lznp011sDic[lznp011s[j].RSCode].SFCXXSW);
                }
            }
            //填写表头信息
            //CellRangeAddress cellRangeAddress = new CellRangeAddress(9 + lznp011s.Count, 9 + lznp011s.Count, 0, 2);//合并单元格
            //CellRangeAddress cellRangeAddress1 = new CellRangeAddress(9 + lznp011s.Count, 9 + lznp011s.Count, 3, 4);
            //CellRangeAddress cellRangeAddress2 = new CellRangeAddress(9 + lznp011s.Count, 9 + lznp011s.Count, 5, 6);
            //CellRangeAddress cellRangeAddress3 = new CellRangeAddress(9 + lznp011s.Count, 9 + lznp011s.Count, 7, 9);
            //sheet1.AddMergedRegion(cellRangeAddress);
            //sheet1.AddMergedRegion(cellRangeAddress1);
            //sheet1.AddMergedRegion(cellRangeAddress2);
            //sheet1.AddMergedRegion(cellRangeAddress3);

            //sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            sheet1.GetRow(6).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            //sheet1.GetRow(9 + lznp011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            //sheet1.GetRow(9 + lznp011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            //sheet1.GetRow(9 + lznp011s.Count).GetCell(5).SetCellValue("填报人：" + rpt.WriterName);
            //sheet1.GetRow(9 + lznp011s.Count).GetCell(7).SetCellValue("填报时间：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));

            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 7; x < 7 + count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 11; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }
        #endregion

        #endregion 存入数据到Excel模板中

        #region 获取经过数量级转换后的HL011-HL014,HP011-HP012的数据
        /// <summary>
        /// HL011的数据（表1到表4），如果没有数据，那么只添加一个DW名称的数据
        /// </summary>
        /// <param name="pageNO">页号</param>
        /// <param name="limit">单位级别</param>
        /// <param name="lowerUnits">当前单位的所有下级单位</param>
        /// <returns></returns>
        public IList<LZHL011> GetLZHL01(int pageNO, int limit, IList<TB07_District> lowerUnits)
        {
            IList<LZHL011> listxmm = new List<LZHL011>();
            var hl011s = busEntity.HL011.Where(t => t.PageNO == pageNO);
            XMMZHClass xmm = new XMMZHClass();
            int sumFlag = 0;//是否已经添加合计行
            int flag = 0;//是否有该单位的数据
            if (limit == 5)
            {
                foreach (var obj in hl011s)
                {
                    if (sumFlag == 0 && obj.DW.Equals("合计"))
                    {
                        LZHL011 hl = xmm.ConvertHLToXMMHL<LZHL011, HL011>(obj, limit);
                        listxmm.Add(hl);
                        break;
                    }
                }
            }
            //if (hl011s.Count() > 0)
            //{
            foreach (var lower in lowerUnits)
            {
                flag = 0;
                foreach (var obj in hl011s)
                {
                    if (sumFlag == 0 && obj.DW.Equals("合计"))
                    {
                        LZHL011 hl = xmm.ConvertHLToXMMHL<LZHL011, HL011>(obj, limit);
                        listxmm.Add(hl);
                        sumFlag = 1; //合计已经添加了
                        break;
                    }
                }
                if (sumFlag == 0)//如果没得数据，那么第一条是添加的合计
                {
                    LZHL011 heji = new LZHL011();
                    heji.DW = "合计";
                    listxmm.Add(heji);
                    sumFlag = 1;
                }
                foreach (var obj in hl011s)
                {
                    if (lower.DistrictName.Equals(obj.DW))
                    {
                        LZHL011 hl = xmm.ConvertHLToXMMHL<LZHL011, HL011>(obj, limit);
                        listxmm.Add(hl);
                        flag = 1;//该单位有数据
                        break;
                    }
                }
                if (flag == 0)
                {
                    LZHL011 hl = new LZHL011();
                    hl.DW = lower.DistrictName;
                    listxmm.Add(hl);
                }
            }
            //}
            //else//如果没有数据，那么添加一个合计行
            //{
            //    XMMHL011 heji = new XMMHL011();
            //    heji.DW = "合计";
            //    listxmm.Add(heji);
            //}
            return listxmm;
        }

        public IList<LZHL012> GetLZHL02(int pageNO, int limit)
        {
            IList<LZHL012> listxmm = new List<LZHL012>();
            var hl012s = busEntity.HL012.Where(t => t.PageNO == pageNO).OrderBy(t => t.DataOrder);
            XMMZHClass xmm = new XMMZHClass();
            foreach (var obj in hl012s)
            {
                LZHL012 hl = xmm.ConvertHLToXMMHL<LZHL012, HL012>(obj, limit);
                listxmm.Add(hl);
            }
            return listxmm;
        }

        public IList<LZHL013> GetLZHL03(int pageNO, int limit)
        {
            IList<LZHL013> listxmm = new List<LZHL013>();
            var hl013s = busEntity.HL013.Where(t => t.PageNO == pageNO).OrderBy(t => t.DataOrder);
            XMMZHClass xmm = new XMMZHClass();
            foreach (var obj in hl013s)
            {
                LZHL013 hl = xmm.ConvertHLToXMMHL<LZHL013, HL013>(obj, limit);
                listxmm.Add(hl);
            }
            return listxmm;
        }

        public IList<LZHL014> GetLZHL04(int pageNO, int limit, IList<TB07_District> lowerUnits)
        {
            IList<LZHL014> listxmm = new List<LZHL014>();
            var hl014s = busEntity.HL014.Where(t => t.PageNO == pageNO);
            XMMZHClass xmm = new XMMZHClass();
            int sumFlag = 0;//是否已经添加合计行
            int flag = 0;//是否有该单位的数据

            //if (hl014s.Count() > 0)
            //{
            foreach (var lower in lowerUnits)
            {
                flag = 0;
                foreach (var obj in hl014s)
                {
                    if (sumFlag == 0 && obj.DW.Equals("合计"))
                    {
                        LZHL014 hl = xmm.ConvertHLToXMMHL<LZHL014, HL014>(obj, limit);
                        listxmm.Add(hl);
                        sumFlag = 1; //合计已经添加了
                        break;
                    }
                }
                if (sumFlag == 0)//如果没得数据，那么第一条是添加的合计
                {
                    LZHL014 heji = new LZHL014();
                    heji.DW = "合计";
                    listxmm.Add(heji);
                    sumFlag = 1;
                }
                foreach (var obj in hl014s)
                {
                    if (lower.DistrictName.Equals(obj.DW))
                    {
                        LZHL014 hl = xmm.ConvertHLToXMMHL<LZHL014, HL014>(obj, limit);
                        listxmm.Add(hl);
                        flag = 1;//该单位有数据
                        break;
                    }
                }
                if (flag == 0)
                {
                    LZHL014 hl = new LZHL014();
                    hl.DW = lower.DistrictName;
                    listxmm.Add(hl);
                }
            }
            //}
            //else//如果没有数据，那么添加一个合计行
            //{
            //    XMMHL014 heji = new XMMHL014();
            //    heji.DW = "合计";
            //    listxmm.Add(heji);
            //}
            return listxmm;
        }

        /*************************蓄水表分割线************************/

        public IList<LZHP011> GetLZHP01(int pageNO, int limit, IList<TB07_District> lowerUnits)
        {
            IList<LZHP011> listxmm = new List<LZHP011>();
            var hp011s = busEntity.HP011.Where(t => t.PAGENO == pageNO);
            XMMZHClass xmm = new XMMZHClass();
            int sumFlag = 0;//是否已经添加合计行
            int flag = 0;//是否有该单位的数据
            //if (hp011s.Count() > 0)
            //{
            foreach (var lower in lowerUnits)
            {
                flag = 0;
                foreach (var obj in hp011s)
                {
                    if (sumFlag == 0 && obj.DW.Equals("合计"))
                    {
                        LZHP011 hl = xmm.ConvertHLToXMMHL<LZHP011, HP011>(obj, limit);
                        listxmm.Add(hl);
                        sumFlag = 1; //合计已经添加了
                        break;
                    }
                }
                if (sumFlag == 0)//如果没得数据，那么第一条是添加的合计
                {
                    LZHP011 heji = new LZHP011();
                    heji.DW = "合计";
                    listxmm.Add(heji);
                    sumFlag = 1;
                }
                foreach (var obj in hp011s)
                {
                    if (lower.DistrictName.Equals(obj.DW))
                    {
                        LZHP011 hl = xmm.ConvertHLToXMMHL<LZHP011, HP011>(obj, limit);
                        listxmm.Add(hl);
                        flag = 1;//该单位有数据
                        break;
                    }
                }


                if (flag == 0)
                {
                    LZHP011 hl = new LZHP011();
                    hl.DW = lower.DistrictName;
                    listxmm.Add(hl);
                }

            }
            //}
            //else//如果没有数据，那么添加一个合计行
            //{
            //    XMMHP011 heji = new XMMHP011();
            //    heji.DW = "合计";
            //    listxmm.Add(heji);
            //}
            return listxmm;
        }

        public IList<LZHP012> GetLZHP02(int pageNO, int limit, IList<TB43_Reservoir> Reservoirs)
        {
            IList<LZHP012> listxmm = new List<LZHP012>();
            var hp012s = busEntity.HP012.Where(t => t.PAGENO == pageNO).OrderBy(t => t.DISTRIBUTERATE);
            XMMZHClass xmm = new XMMZHClass();
            int sumFlag = 0;//是否已经添加大型水库合计行
            int middleSumFlag = 0;//是否已经添加大型水库合计行
            int flag = 0;//是否有该单位的数据
            //if (hp012s.Count() > 0)
            //{
            foreach (var shuiku in Reservoirs)
            {
                flag = 0;
                if (shuiku.RSType == 1)
                {
                    foreach (var obj in hp012s)
                    {
                        if (sumFlag == 0 && obj.DXSKMC.Equals("合计") && obj.DISTRIBUTERATE == "1")
                        {
                            LZHP012 hl = xmm.ConvertHLToXMMHL<LZHP012, HP012>(obj, limit);
                            listxmm.Add(hl);
                            sumFlag = 1; //合计已经添加了
                            break;
                        }
                    }
                    if (sumFlag == 0) //大型水库合计
                    {
                        LZHP012 heji = new LZHP012();
                        heji.DXSKMC = "合计";
                        heji.DistributeRate = "1";
                        listxmm.Add(heji);
                        sumFlag = 1;
                    }
                    foreach (var obj in hp012s)
                    {
                        if (shuiku.RSName.Equals(obj.DXSKMC)) //水库名称
                        {
                            LZHP012 hl = xmm.ConvertHLToXMMHL<LZHP012, HP012>(obj, limit);
                            listxmm.Add(hl);
                            flag = 1; //该单位有数据
                            break;
                        }
                    }


                    if (flag == 0)
                    {
                        LZHP012 hl = new LZHP012();
                        hl.DXSKMC = shuiku.RSName;
                        hl.DistributeRate = "1";
                        listxmm.Add(hl);
                    }
                }
                else if (shuiku.RSType == 2)
                {
                    foreach (var obj in hp012s)
                    {
                        if (middleSumFlag == 0 && obj.DXSKMC.Equals("合计") && obj.DISTRIBUTERATE == "2")
                        {
                            LZHP012 hl = xmm.ConvertHLToXMMHL<LZHP012, HP012>(obj, limit);
                            listxmm.Add(hl);
                            middleSumFlag = 1; //合计已经添加了
                            break;
                        }
                    }
                    if (middleSumFlag == 0) //中型水库合计
                    {
                        LZHP012 heji = new LZHP012();
                        heji.DXSKMC = "合计";
                        heji.DistributeRate = "2";
                        listxmm.Add(heji);
                        middleSumFlag = 1;
                    }
                    foreach (var obj in hp012s)
                    {
                        if (shuiku.RSName.Equals(obj.DXSKMC)) //水库名称
                        {
                            LZHP012 hl = xmm.ConvertHLToXMMHL<LZHP012, HP012>(obj, limit);
                            listxmm.Add(hl);
                            flag = 1; //该单位有数据
                            break;
                        }
                    }


                    if (flag == 0)
                    {
                        LZHP012 hl = new LZHP012();
                        hl.DXSKMC = shuiku.RSName;
                        hl.DistributeRate = "2";
                        listxmm.Add(hl);
                    }
                }
            }
            //}
            //else//如果没有数据，那么添加一个合计行
            //{
            //    XMMHP012 heji = new XMMHP012();
            //    heji.DW = "合计";
            //    listxmm.Add(heji);
            //}
            return listxmm;
        }

        public IList<NP011> GetLZNP01(int pageNO, int limit, IList<TB62_NMReservoir> nmRes, string unitCode)
        {
            IList<NP011> listxmm = new List<NP011>();
            int[] limitSub = { 2, 4, 6 };
            string tempCode = unitCode.Substring(0, limitSub[limit - 2]);
            var np011s = busEntity.NP011.Where(t => t.PageNO == pageNO).Where(t => t.RSCode.StartsWith(tempCode));
            XMMZHClass xmm = new XMMZHClass();
            int flag = 0;//是否有该单位的数据
            foreach (var lower in nmRes)
            {
                flag = 0;
                foreach (var obj in np011s)
                {
                    if (lower.RSCode.Equals(obj.RSCode))
                    {
                        listxmm.Add(obj);
                        flag = 1;//该单位有数据
                        break;
                    }
                }
                if (flag == 0)
                {
                    NP011 np01 = new NP011();
                    np01.RSCode = lower.RSCode;
                    listxmm.Add(np01);
                }

            }
            return listxmm;
        }
        #endregion 获取经过数量级转换后的HL011-HL014,HP011-HP012的数据

        #endregion 导出Excel

        #region 导入导出公共辅助的方法

        public void PrintSetup(ISheet sheet1)
        {
            sheet1.PrintSetup.Landscape = true;
            sheet1.PrintSetup.Scale = 80;
            //return sheet1;
        }

        /// <summary> 在sheet中创建cell
        /// </summary>
        /// <param name="sheet1"></param>
        /// <param name="rowNum">在模板的基础上需要创建的行数</param>
        /// <param name="cowNum">需要创建的列数</param>
        /// <returns></returns>
        public ISheet CreateCell(ISheet sheet1, int rowNum, int cowNum, int startRow)
        {
            for (int i = 0; i <= rowNum; i++)//多添加一列用于表尾信息的填写
            {
                IRow row1 = null;
                if (sheet1.GetRow(startRow + i) == null)
                {
                    row1 = sheet1.CreateRow(startRow + i);
                }
                else
                {
                    row1 = sheet1.GetRow(startRow + i);
                }
                for (int j = 0; j < cowNum; j++)
                {
                    if (row1.GetCell(j) == null)
                    {
                        row1.CreateCell(j);
                    }
                }
            }
            return sheet1;
        }

        public IList<TB07_District> getUnitInfo(string unitCode)
        {
            IList<TB07_District> listInfo = fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode).ToList();
            return listInfo;
        }

        /// <summary>获取该单位所有的水库
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public IList<TB43_Reservoir> getReservoirs(string unitCode)
        {
            IList<TB43_Reservoir> list = (from tb44 in fxdict.TB44_ReservoirDistrict
                                          where tb44.UnitCode == unitCode
                                          select tb44.TB43_Reservoir).OrderBy(t => t.RSType).OrderBy(t => t.RSOrder).ToList();//先按照水库类型排序，再按照水库顺序
            return list;
        }


        /// <summary>获取单位水库个数和水库信息
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IList<LZNP011> getNMUnitReservoirs(string unitCode, int limit)
        {
            int[] limitSub = { 4, 6, 6 };
            IList<TB07_District> tb07s = new List<TB07_District>();
            IList<LZNP011> lznp011s = new List<LZNP011>();
            if (limit < 4)
            {
                tb07s = (from tb07 in fxdict.TB07_District
                         where tb07.pDistrictCode == unitCode
                         select tb07).OrderBy(t => t.Uorder).ToList();

            }
            else
            {
                tb07s = (from tb07 in fxdict.TB07_District
                         where tb07.DistrictCode == unitCode
                         select tb07).OrderBy(t => t.Uorder).ToList();
            }
            int uNO = 1;
            TB62_NMReservoir tb62Nm = null;
            foreach (var tb07 in tb07s)
            {
                string tempCode = tb07.DistrictCode.Substring(0, limitSub[limit - 2]);
                IList<TB62_NMReservoir> list = (from t in fxdict.TB62_NMReservoir
                                                where t.UnitCode.StartsWith(tempCode)
                                                select t).OrderBy(t => t.RSOrder).ToList(); //先按照单位代码排序，再按照水库顺序
                if (list.Count > 0)
                {
                    LZNP011 unitNp01 = new LZNP011(); //存放单位的，包含水库个数、编号
                    int i = 1;
                    unitNp01.RSCode = "0";
                    unitNp01.RSName = tb07.DistrictName;
                    unitNp01.TBNO = ConvertChinese(uNO.ToString());
                    unitNp01.UnitName = list.Count.ToString();
                    unitNp01.UnitCode = tb07.DistrictCode;
                    tb62Nm = list.Where(t => t.UnitCode == tb07.DistrictCode && t.RSCode == "0").SingleOrDefault();
                    if (tb62Nm != null)
                    {
                        unitNp01.ZKR = ZeroToEmpty(Convert.ToDouble(tb62Nm.ZKR), "0.000");
                        unitNp01.SKR = ZeroToEmpty(Convert.ToDouble(tb62Nm.SKR), "0.000");
                    }

                    lznp011s.Add(unitNp01);
                    uNO++;
                    list = list.Where(t => t.RSCode != "0").ToList();
                    foreach (var tb62 in list)
                    {
                        LZNP011 np01 = new LZNP011(); //存放每一个水库、编号
                        np01.RSCode = tb62.RSCode;
                        np01.RSName = tb62.RSName;
                        np01.TBNO = i.ToString();
                        np01.UnitName = tb62.UnitName;
                        np01.ZKR = String.Format("{0:N3}", tb62.ZKR).Replace(",", "");
                        np01.SKR = ZeroToEmpty(Convert.ToDouble(tb62.SKR),"0.000");
                        np01.SSW = ZeroToEmpty(Convert.ToDouble(tb62.SSW), "0.00");
                        np01.XXSW = String.Format("{0:N2}", tb62.XXSW).Replace(",", "");
                        np01.ZCXSW = String.Format("{0:N2}", tb62.ZCXSW).Replace(",", "");
                        //np01.ZCXSWXYKR = String.Format("{0:N2}", tb62.ZCXSWXYKR).Replace(",", "");
                        lznp011s.Add(np01);
                        i++;
                    }
                }
            }
            return lznp011s;
        }

        private string ZeroToEmpty(double db, string str)
        {
            if (db > 0)
            {
                return db.ToString(str);
            }
            else
            {
                return "";
            }
        }

        /// <summary>查询出该单位水库填写数据
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="limit"></param>
        /// <param name="pageNO"></param>
        /// <returns></returns>
        public Dictionary<string, LZNP011> getNMReservoirsData(string unitCode, int limit, int pageNO)
        {
            int[] limitSub = { 2, 4, 6 };
            Entities getEntity = new Entities();
            Dictionary<string, LZNP011> resData = new Dictionary<string, LZNP011>();
            BusinessEntities nmBusEntity = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(2);

            string tempCode = unitCode.Substring(0, limitSub[limit - 2]);
            var list = (from np01 in nmBusEntity.NP011
                        where np01.UnitCode.StartsWith(tempCode) && np01.PageNO == pageNO
                        select new
                        {
                            np01.DataOrder,
                            np01.UnitCode,
                            np01.RSCode,
                            np01.DQSW,
                            np01.DQXSL,
                            np01.SFCXXSW
                        }
                        ).OrderBy(t => t.DataOrder).ToList(); //先按照单位代码排序，再按照水库顺序
            foreach (var np in list)
            {
                if (!resData.ContainsKey(np.RSCode))
                {
                    LZNP011 np01 = new LZNP011();//存放每一个水库填写的数据
                    np01.DQSW = String.Format("{0:N2}", np.DQSW).Replace(",", "");
                    np01.DQXSL = String.Format("{0:N3}", np.DQXSL).Replace(",", "");
                    np01.SFCXXSW = Convert.ToString(np.SFCXXSW);
                    resData.Add(np.RSCode, np01);
                }
            }
            return resData;
        }

        #region 阿拉伯数字转换成汉语编号
        private static string[] cstr = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };   //定义数组
        private static string[] wstr = { "", "", "十" };
        public string ConvertChinese(string str)
        {
            int len = str.Length;    //获取输入文本的长度值
            string tmpstr = "";      //定义字符串
            string rstr = "";
            for (int i = 1; i <= len; i++)
            {
                tmpstr = str.Substring(len - i, 1);    //截取输入文本 再将值赋给新的字符串
                rstr = string.Concat(cstr[Int32.Parse(tmpstr)] + wstr[i], rstr);  //将两个数组拼接在一起形成新的字符串
            }
            rstr = rstr.Replace("十零", "十");      //将新的字符串替换
            rstr = rstr.Replace("一十", "十");
            return rstr;                            //返回新的字符串
        }
        #endregion
        /// <summary>找出所有大型水库的数据
        /// </summary>
        /// <param name="xmmHp012s">所有XMMHP012的数据</param>
        /// <returns></returns>
        public IList<LZHP012> GetBigXMMHP012(IList<LZHP012> xmmHp012s)
        {
            IList<LZHP012> bigList = new List<LZHP012>();
            for (int i = 0; i < xmmHp012s.Count; i++)
            {
                if (xmmHp012s[i].DistributeRate == "1")//1是大型水库
                {
                    LZHP012 bigXmmhp012 = new LZHP012();
                    bigXmmhp012 = xmmHp012s[i];
                    bigList.Add(bigXmmhp012);
                }
            }
            return bigList;
        }

        /// <summary>找出所有中型水库的数据
        /// </summary>
        /// <param name="xmmHp012s">所有XMMHP012的数据</param>
        /// <returns></returns>
        public IList<LZHP012> GetMiddleXMMHP012(IList<LZHP012> xmmHp012s)
        {
            IList<LZHP012> middleList = new List<LZHP012>();
            for (int i = 0; i < xmmHp012s.Count; i++)
            {
                if (xmmHp012s[i].DistributeRate == "2")//2是中型水库
                {
                    LZHP012 middleXmmhp012 = new LZHP012();
                    middleXmmhp012 = xmmHp012s[i];
                    middleList.Add(middleXmmhp012);
                }
            }
            return middleList;
        }

        /// <summary> 根据单位代码获取流域代码和单位名称
        /// </summary>
        /// <param name="unitcode"></param>
        /// <returns></returns>
        private IList<object[]> getRiverCodeByUnitcode(string unitcode)
        {
            IList<object[]> list = null;
            var tb07s = fxdict.TB07_District.Where(t => t.DistrictCode == unitcode);
            foreach (var tbobj in tb07s)
            {
                object[] obj = new object[2];
                obj[0] = tbobj.RD_RiverCode1;
                obj[1] = tbobj.DistrictName;
                list.Add(obj);
            }
            return list;
        }

        /// <summary>获取下级单位
        /// </summary>
        /// <param name="unitcode"></param>
        /// <returns></returns>
        private IList<TB07_District> getLowerUnits(string unitcode, string tableType)
        {
            IList<TB07_District> list = null;
            var tb07s = fxdict.TB07_District.Where(t => t.pDistrictCode == unitcode).OrderBy(t => t.Uorder).AsQueryable();
            if (tableType == "HL01")
            {
                tb07s = tb07s.Where(t => t.DistrictName != "全区/县").OrderBy(t => t.Uorder).AsQueryable();
            }
            list = tb07s.ToList<TB07_District>();
            return list;
        }

        /// <summary> 死亡原因
        /// </summary>
        /// <returns></returns>
        private string getDiedReasonStr()
        {
            string res = "冰凌_倒房,冰凌_溺水,降雨_大江大河洪水_倒房,降雨_大江大河洪水_溺水,降雨_山洪灾害_山洪冲淹_倒房,"
                     + "降雨_山洪灾害_山洪冲淹_溺水,降雨_山洪灾害_滑坡_掩埋,降雨_山洪灾害_滑坡_倒房|JY030407,降雨_山洪灾害_泥石流_掩埋|JY030506,"
                     + "降雨_山洪灾害_泥石流_倒房|JY030507,降雨_倒房|JY07,降雨_其它|JY10,融雪_山洪灾害_山洪冲淹_倒房|RX030207,融雪_山洪灾害_山洪冲淹_溺水|RX030208,"
                     + "融雪_山洪灾害_滑坡_掩埋|RX030406,融雪_山洪灾害_滑坡_倒房|RX030407,融雪_山洪灾害_泥石流_掩埋|RX030506,融雪_山洪灾害_泥石流_倒房|RX030507,"
                     + "融雪_倒房|RX07,融雪_其它|RX10,台风_海上死亡|TF09,台风_降雨_大江大河洪水_倒房|TFJY0107,台风_降雨_大江大河洪水_溺水|TFJY0108,"
                     + "台风_降雨_山洪灾害_山洪冲淹_倒房|TFJY030207,台风_降雨_山洪灾害_山洪冲淹_溺水|TFJY030208,台风_降雨_山洪灾害_滑坡_掩埋|TFJY030406,"
                     + "台风_降雨_山洪灾害_滑坡_倒房|TFJY030407,台风_降雨_山洪灾害_泥石流_掩埋|TFJY030506,台风_降雨_山洪灾害_泥石流_倒房|TFJY030507,"
                     + "台风_降雨_倒房|TFJY07,台风_降雨_其它|TFJY10";

            //string res = "冰凌|BL,冰凌-倒房|BL07,冰凌-溺水|BL08,降雨|JY,降雨-大江大河洪水|JY01,降雨-大江大河洪水-倒房|JY0107";

            return res;

        }

        /// <summary> 获取表头信息
        /// </summary>
        /// <param name="pageNO"></param>
        /// <returns></returns>
        private ReportTitle getRptInfo(int pageNO)
        {
            ReportTitle rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).FirstOrDefault();
            return rpt;
        }

        /// <summary> 设置单元格的格式
        /// </summary>
        /// <param name="sheet">需要设置单元格格式的sheet</param>
        /// <returns></returns>
        private ICellStyle SetCellStyle(ISheet sheet)
        {
            ICellStyle style = sheet.Workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.CENTER;//水平对齐居中
            style.VerticalAlignment = VerticalAlignment.CENTER;
            style.BorderBottom = BorderStyle.THIN;//边框是黑色的
            style.BorderLeft = BorderStyle.THIN;
            style.BorderRight = BorderStyle.THIN;
            style.BorderTop = BorderStyle.THIN;
            return style;
        }

        /// <summary>从字典库获取所有的流域数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetRiverData()
        {
            var tb09s = fxdict.TB09_RiverDict.AsQueryable();
            Dictionary<string, string> dicRiverInfo = new Dictionary<string, string>();
            foreach (var obj in tb09s)
            {
                dicRiverInfo.Add(obj.RiverCode, obj.RiverName);
            }
            return dicRiverInfo;
        }

        /// <summary>获取该单位的所有流域(提供下拉列表)
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string GetRiverDataByUnitCode(string unitCode)
        {
            string riverInfo = "";
            Dictionary<string, string> dicRiverInfo = GetRiverData();
            var tb07 = fxdict.TB07_District.Where(t => t.DistrictCode == unitCode).First();
            string[] riverArr = tb07.RD_RiverCode1.Split(',');
            for (int k = 0; k < riverArr.Length; k++)
            {
                if (dicRiverInfo.ContainsKey(riverArr[k]))
                {
                    riverInfo += dicRiverInfo[riverArr[k]] + ",";
                }
            }
            if (riverInfo != "")
            {
                riverInfo = riverInfo.Remove(riverInfo.Length - 1);
            }
            return riverInfo;
        }

        /// <summary>获取该单位的所有下级单位名称(提供下拉列表)
        /// </summary>
        /// <param name="unitCode"></param>
        /// <returns></returns>
        public string GetUnderUnitNames(string unitCode, string tableType)
        {
            string unitNames = "";
            IList<TB07_District> list = getLowerUnits(unitCode, tableType);
            foreach (var obj in list)
            {
                unitNames += obj.DistrictName + ",";
            }
            if (unitNames != "")
            {
                unitNames = unitNames.Remove(unitNames.Length - 1);
            }
            return unitNames;
        }

        /// <summary>根据页号获得报表类型 判断是否是实时报
        /// </summary>
        /// <param name="pageNO"></param>
        /// <returns></returns>
        public long getRptType(int pageNO)
        {
            long typecode = -1;
            ReportTitle rpt = busEntity.ReportTitle.Where(t => t.PageNO == pageNO).FirstOrDefault();
            typecode = Convert.ToInt64(rpt.StatisticalCycType);
            return typecode;
        }

        /// <summary>获取字段单位
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetFieldsData(int limit)
        {
            Dictionary<string, string> fieldDic = new Dictionary<string, string>();
            var tb55s = fxdict.TB55_FieldDefine.Where(t => t.UnitCls == limit).ToList();
            foreach (var tb55FieldDefine in tb55s)
            {
                if (tb55FieldDefine.MeasureName != null && tb55FieldDefine.MeasureName != "")
                {
                    if (!fieldDic.ContainsKey(tb55FieldDefine.FieldCode))
                    {
                        fieldDic.Add(tb55FieldDefine.FieldCode, tb55FieldDefine.MeasureName);
                    }
                }
            }
            return fieldDic;
        }

        #endregion

        #region 导入Excel（老方法）

        /// <summary>读取Excel
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="rows">表头行数</param>
        /// <param name="unitCode">单位代码（用户自己登陆的单位代码，获取下级单位、流域时用）</param>
        /// <returns>Excel表数据集</returns>
        public ArrayList ImportData(string fileName, int[] rows, string unitCode)
        {
            //获取下级单位
            //CommonFunction cf = new CommonFunction();
            IList<TB07_District> lowerUnits = getLowerUnits(unitCode, "HL01");
            string unitsStr = "";
            foreach (var obj in lowerUnits)
            {
                unitsStr += obj.DistrictName + "|" + obj.DistrictCode + ",";
            }

            if (unitsStr != "")
            {
                unitsStr = unitsStr.Remove(unitsStr.Length - 1);
            }
            string[] unit = unitsStr.Split(',');

            //死亡原因
            string deathListStr = "冰凌|BL,冰凌-倒房|BL07,冰凌-溺水|BL08,降雨|JY,降雨-大江大河洪水|JY01,降雨-大江大河洪水-倒房|JY0107,降雨-大江大河洪水-溺水|JY0108,"
                                + "降雨-山洪灾害|JY03,降雨-山洪灾害-山洪冲淹|JY0302,降雨-山洪灾害-山洪冲淹-倒房|JY030207,降雨-山洪灾害-山洪冲淹-溺水|JY030208,"
                                + "降雨-山洪灾害-滑坡|JY0304,降雨-山洪灾害-滑坡-掩埋|JY030406,降雨-山洪灾害-滑坡-倒房|JY030407,降雨-山洪灾害-泥石流|JY0305,"
                                + "降雨-山洪灾害-泥石流-掩埋|JY030506,降雨-山洪灾害-泥石流-倒房|JY030507,降雨-倒房|JY07,降雨-其它|JY10,融雪|RX,融雪-山洪灾害|RX03,"
                                + "融雪-山洪灾害-山洪冲淹|RX0302,融雪-山洪灾害-山洪冲淹-倒房|RX030207,融雪-山洪灾害-山洪冲淹-溺水|RX030208,融雪-山洪灾害-滑坡|RX0304,"
                                + "融雪-山洪灾害-滑坡-掩埋|RX030406,融雪-山洪灾害-滑坡-倒房|RX030407,融雪-山洪灾害-泥石流|RX0305,融雪-山洪灾害-泥石流-掩埋|RX030506,"
                                + "融雪-山洪灾害-泥石流-倒房|RX030507,融雪-倒房|RX07,融雪-其它|RX10,台风|TF,台风-海上死亡|TF09,台风-降雨|TFJY,台风-降雨-大江大河洪水|TFJY01,"
                                + "台风-降雨-大江大河洪水-倒房|TFJY0107,台风-降雨-大江大河洪水-溺水|TFJY0108,台风-降雨-山洪灾害|TFJY03,台风-降雨-山洪灾害-山洪冲淹|TFJY0302,"
                                + "台风-降雨-山洪灾害-山洪冲淹-倒房|TFJY030207,台风-降雨-山洪灾害-山洪冲淹-溺水|TFJY030208,台风-降雨-山洪灾害-滑坡|TFJY0304,"
                                + "台风-降雨-山洪灾害-滑坡-掩埋|TFJY030406,台风-降雨-山洪灾害-滑坡-倒房|TFJY030407,台风-降雨-山洪灾害-泥石流|TFJY0305,"
                                + "台风-降雨-山洪灾害-泥石流-掩埋|TFJY030506,台风-降雨-山洪灾害-泥石流-倒房|TFJY030507,台风-降雨-倒房|TFJY07";
            string[] deathList = deathListStr.Split(',');


            //流域代码以及名称
            CommonFunction cf = new CommonFunction();
            string[] river = cf.GetRiverCodeAndName(unitCode).Split(',');

            string[] sheetName = { "表1", "表2", "表3", "表4", "表5", "表6", "表7", "表8", "表9" };  //表名
            System.Data.DataTable dt = new System.Data.DataTable();

            ArrayList list = new ArrayList();

            int sheetCount = SheetCount(fileName); //获取导入的Excel表格个数
            int count = 9;
            for (int k = 0; k < count; k++) //循环读取9张表
            {
                ArrayList arrList = new ArrayList(); //存放第 k 张表的数据

                if (sheetCount < 9 && (k == 4 || k == 8)) //实时报，只有表5、表9，其他表以null填充
                {
                    int beginRow = rows[k] - 1; //数据起始行

                    int beginColumn = 1; //数据起始列

                    if (k == 5 - 1 || k == 6 - 1)
                    {
                        beginColumn = 0;
                    }

                    DataSet ds = new DataSet();

                    //读取第 k 张表，sheetName[k]
                    string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";" + "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                    OleDbDataAdapter da = new OleDbDataAdapter("SELECT *  FROM [" + sheetName[k] + "$]", strConn);

                    da.Fill(ds);
                    dt = ds.Tables[0];

                    int tableRows = dt.Rows.Count - 1; // 最后一行不读取
                    for (int i = beginRow; i < tableRows; i++) //循环表格行
                    {
                        DataRow dr = dt.Rows[i]; //获取行数据

                        string[] str = new string[dt.Columns.Count - beginColumn]; //存放该行每个单元格的数据
                        if ((k == 5 - 1 || k == 6 - 1) && dr[beginColumn].ToString() == "") //表5表6中如果该行第一个单元格数据为空则不读取
                        {
                            continue;
                        }
                        int n = 0;
                        for (int j = beginColumn; j < dt.Columns.Count; j++) //循环行中的每个单元格
                        {
                            string value = dr[j].ToString(); //获取单元格的值

                            if ((k == 5 - 1 || k == 6 - 1) && j == beginColumn) //设置表5的单位代码
                            {
                                for (int h = 0; h < unit.Length; h++) //遍历下级单位代码数组
                                {
                                    string[] unitInfo = unit[h].Split('|');
                                    if (value == unitInfo[0]) //判断该行数据的单位名称是否与数组中的相同
                                    {
                                        value = unit[h];
                                        break;
                                    }
                                }
                            }
                            if (k == 5 - 1 && j == 6 - 1 && value != "") //转换死亡时间格式
                            {
                                try
                                {
                                    value = Convert.ToDateTime(value).Date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                                }
                                catch
                                {
                                    value = "";
                                }
                            }
                            if (k == 5 - 1 && j == 7) //设置表5的死亡原因及死亡原因代码
                            {
                                for (int h = 0; h < deathList.Length; h++) //遍历死亡原因数组
                                {
                                    deathList[h].Split('|');
                                    if (value == deathList[h].Split('|')[0])
                                    {
                                        value = deathList[h];
                                        break;
                                    }
                                }
                            }
                            if ((k == 5 - 1 && j == 9) || (k == 6 - 1 && j == 19)) //设置表5表6的流域
                            {
                                for (int h = 0; h < river.Length; h++) //遍历流域数组
                                {
                                    if (value == river[h].Split('|')[0])
                                    {
                                        value = river[h];
                                        break;
                                    }
                                }
                            }
                            str[n] = value;
                            n++;
                        }
                        arrList.Add(str);
                    }
                }
                else if (sheetCount >= 9) //不是实时报，共有9张表
                {
                    int beginRow = rows[k] - 1; //数据起始行

                    int beginColumn = 1;
                    if (k == 5 - 1 || k == 6 - 1)
                    {
                        beginColumn = 0;
                    }

                    DataSet ds = new DataSet();

                    //读取第 k 张表，sheetName[k]
                    string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";" + "Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                    OleDbDataAdapter da = new OleDbDataAdapter("SELECT *  FROM [" + sheetName[k] + "$]", strConn);

                    da.Fill(ds);
                    dt = ds.Tables[0];

                    int tableRows = dt.Rows.Count - 1; //最后一行不读取
                    for (int i = beginRow; i < tableRows; i++)  //循环表格行
                    {
                        DataRow dr = dt.Rows[i]; //获取行数据

                        string[] str = new string[dt.Columns.Count - beginColumn];
                        if ((k == 5 - 1 || k == 6 - 1) && dr[beginColumn].ToString() == "")
                        {
                            continue;
                        }
                        int n = 0;
                        for (int j = beginColumn; j < dt.Columns.Count; j++)
                        {
                            string value = dr[j].ToString();
                            if ((k == 5 - 1 || k == 6 - 1) && j == beginColumn) //设置表5表6的单位代码
                            {
                                for (int h = 0; h < unit.Length; h++)
                                {
                                    string[] unitInfo = unit[h].Split('|');
                                    if (value == unitInfo[0])
                                    {
                                        value = unit[h];
                                        break;
                                    }
                                }
                            }
                            if (k == 5 - 1 && j == 7) //设置表5的死亡原因
                            {
                                for (int h = 0; h < deathList.Length; h++)
                                {
                                    deathList[h].Split('|');
                                    if (value == deathList[h].Split('|')[0])
                                    {
                                        value = deathList[h];
                                        break;
                                    }
                                }
                            }
                            if (k == 5 - 1 && j == 6 - 1 && value != "") //转换死亡时间格式
                            {
                                try
                                {
                                    value = Convert.ToDateTime(value).Date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                                }
                                catch
                                {
                                    value = "";
                                }
                            }
                            if ((k == 5 - 1 && j == 9) || (k == 6 - 1 && j == 19)) //设置表5表6的流域
                            {
                                for (int h = 0; h < river.Length; h++)
                                {
                                    if (value == river[h].Split('|')[0])
                                    {
                                        value = river[h];
                                        break;
                                    }
                                }
                            }
                            str[n] = value;
                            n++;
                        }
                        arrList.Add(str);
                    }
                }
                list.Add(arrList);
            }
            return list;
        }

        /// <summary>获取Excel表格数量
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns>表格数</returns>
        public int SheetCount(string fileName)
        {
            int count = 0;
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=\"Excel 8.0\";Data Source=" + fileName))
            {
                conn.Open();
                System.Data.DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                count = dt.Rows.Count;
                conn.Close();
            }
            return count;
        }
        #endregion

        #region 导入新方法
        public ArrayList ImportExcelData(string fileName, string rptType, string unitCode, int limit)
        {
            int[] hlStartRows = { 9, 9, 9, 9, 9, 9, 9, 9, 9 }; //洪涝表表头行数
            int[] hpStartRows = { 6, 6, 6, 6 }; //蓄水表表头行数
            int[] hlMaxCols = { 10, 13, 8, 19, 11, 20, 19, 17, 11 }; //洪涝表数据的最大列数
            if (unitCode.StartsWith("33"))//浙江表4多了4列数据
            {
                hlMaxCols[3] = 23;
            }
            int[] hpMaxCols = { 18, 16, 18, 18 }; //蓄水表数据的最大列数

            if (rptType == "HL01")
            {
                return ImportHLExcel(fileName, unitCode, hlStartRows, hlMaxCols, limit);
            }
            else if (rptType == "HP01")
            {
                return ImportHPExcel(fileName, unitCode, hpStartRows, hpMaxCols);
            }
            else
            {
                return new ArrayList();
            }
        }

        public ArrayList ImportHLExcel(string fileName, string unitCode, int[] hlStartRows, int[] hlMaxCols, int limit)
        {
            #region 初始化数据
            IList<TB07_District> lowerUnits = getLowerUnits(unitCode, "HL01");
            string unitsStr = "";
            foreach (var obj in lowerUnits)
            {
                unitsStr += obj.DistrictName + "|" + obj.DistrictCode + ",";
            }

            if (unitsStr != "")
            {
                unitsStr = unitsStr.Remove(unitsStr.Length - 1);
            }
            string[] units = unitsStr.Split(',');
            ArrayList list = new ArrayList();
            //死亡原因
            string deathListStr = "冰凌|BL,冰凌-倒房|BL07,冰凌-溺水|BL08,降雨|JY,降雨-大江大河洪水|JY01,降雨-大江大河洪水-倒房|JY0107,降雨-大江大河洪水-溺水|JY0108,"
                                + "降雨-山洪灾害|JY03,降雨-山洪灾害-山洪冲淹|JY0302,降雨-山洪灾害-山洪冲淹-倒房|JY030207,降雨-山洪灾害-山洪冲淹-溺水|JY030208,"
                                + "降雨-山洪灾害-滑坡|JY0304,降雨-山洪灾害-滑坡-掩埋|JY030406,降雨-山洪灾害-滑坡-倒房|JY030407,降雨-山洪灾害-泥石流|JY0305,"
                                + "降雨-山洪灾害-泥石流-掩埋|JY030506,降雨-山洪灾害-泥石流-倒房|JY030507,降雨-倒房|JY07,降雨-其它|JY10,融雪|RX,融雪-山洪灾害|RX03,"
                                + "融雪-山洪灾害-山洪冲淹|RX0302,融雪-山洪灾害-山洪冲淹-倒房|RX030207,融雪-山洪灾害-山洪冲淹-溺水|RX030208,融雪-山洪灾害-滑坡|RX0304,"
                                + "融雪-山洪灾害-滑坡-掩埋|RX030406,融雪-山洪灾害-滑坡-倒房|RX030407,融雪-山洪灾害-泥石流|RX0305,融雪-山洪灾害-泥石流-掩埋|RX030506,"
                                + "融雪-山洪灾害-泥石流-倒房|RX030507,融雪-倒房|RX07,融雪-其它|RX10,台风|TF,台风-海上死亡|TF09,台风-降雨|TFJY,台风-降雨-大江大河洪水|TFJY01,"
                                + "台风-降雨-大江大河洪水-倒房|TFJY0107,台风-降雨-大江大河洪水-溺水|TFJY0108,台风-降雨-山洪灾害|TFJY03,台风-降雨-山洪灾害-山洪冲淹|TFJY0302,"
                                + "台风-降雨-山洪灾害-山洪冲淹-倒房|TFJY030207,台风-降雨-山洪灾害-山洪冲淹-溺水|TFJY030208,台风-降雨-山洪灾害-滑坡|TFJY0304,"
                                + "台风-降雨-山洪灾害-滑坡-掩埋|TFJY030406,台风-降雨-山洪灾害-滑坡-倒房|TFJY030407,台风-降雨-山洪灾害-泥石流|TFJY0305,"
                                + "台风-降雨-山洪灾害-泥石流-掩埋|TFJY030506,台风-降雨-山洪灾害-泥石流-倒房|TFJY030507,台风-降雨-倒房|TFJY07";
            string[] deathList = deathListStr.Split(',');


            //流域代码以及名称
            CommonFunction cf = new CommonFunction();
            string[] river = cf.GetRiverCodeAndName(unitCode).Split(',');

            //string[] sheetName = { "表1", "表2", "表3", "表4", "表5", "表6", "表7", "表8", "表9" };  //表名

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            #endregion

            //ISheet sheet = hssfworkbook.GetSheetAt(0);//根据第一个表的表名判断是否是实时报。实时报第一张表是“表9”
            #region 实时报
            if (hssfworkbook.NumberOfSheets == 3 || hssfworkbook.NumberOfSheets == 2)//实时报
            {
                for (int m = 0; m < 2; m++)
                {
                    ArrayList arrList = new ArrayList(); //存放第 k 张表的数据
                    int[] ssHlCols = { 11, 11 };//表9与表5
                    int[] ssHlRows = { 9, 9 };//表9与表5
                    ISheet sheetData = hssfworkbook.GetSheetAt(m);//取第i个表
                    int colCount = ssHlCols[m];//列的最大值
                    int endRowCount = 0;
                    if (m == 0)
                    {
                        endRowCount = ssHlRows[m] + units.Length;
                    }
                    else if (m == 1)
                    {
                        endRowCount = sheetData.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1
                    }
                    for (int i = ssHlRows[m]; i <= endRowCount; i++)
                    {
                        IRow row = sheetData.GetRow(i);
                        string[] str = new string[colCount];
                        if (row != null)
                        {
                            if (row.GetCell(0).ToString() == "")//第一列的数据如果为空，那么结束该循环
                            {
                                break;
                            }
                            for (int j = 0; j < colCount; j++)//循环该行的所有cell
                            {
                                if (row.GetCell(j) != null)
                                {
                                    string value = row.GetCell(j).ToString();

                                    if (m == 1 && j == 0) //设置表5的单位代码
                                    {
                                        for (int h = 0; h < units.Length; h++) //遍历下级单位代码数组
                                        {
                                            string[] unitInfo = units[h].Split('|');
                                            if (value == unitInfo[0]) //判断该行数据的单位名称是否与数组中的相同
                                            {
                                                value = units[h];
                                                break;
                                            }
                                        }
                                    }
                                    if (m == 1 && j == 6 && value != "") //转换死亡时间格式
                                    {
                                        try
                                        {
                                            value = Convert.ToDateTime(value).Date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                                        }
                                        catch
                                        {
                                            value = "";
                                        }
                                    }
                                    if (m == 1 && j == 8) //设置表5的死亡原因及死亡原因代码
                                    {
                                        for (int h = 0; h < deathList.Length; h++) //遍历死亡原因数组
                                        {
                                            deathList[h].Split('|');
                                            if (value == deathList[h].Split('|')[0])
                                            {
                                                value = deathList[h];
                                                break;
                                            }
                                        }
                                    }
                                    if (m == 1 && j == 10) //设置表5的流域
                                    {
                                        for (int h = 0; h < river.Length; h++) //遍历流域数组
                                        {
                                            if (value == river[h].Split('|')[0])
                                            {
                                                value = river[h];
                                                break;
                                            }
                                        }
                                    }
                                    str[j] = value;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        arrList.Add(str);
                    }
                    list.Add(arrList);
                }

            }
            #endregion

            #region 非实时报
            else if (hssfworkbook.NumberOfSheets == 10 || hssfworkbook.NumberOfSheets == 9)
            {
                for (int m = 0; m < 9; m++)
                {
                    ArrayList arrList = new ArrayList(); //存放第 k 张表的数据
                    ISheet sheetData = hssfworkbook.GetSheetAt(m);//取第i个表
                    int colCount = hlMaxCols[m];//列的最大值
                    int endRowCount = 0;
                    if (m == 4 || m == 5)
                    {
                        endRowCount = sheetData.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1
                    }
                    else
                    {
                        endRowCount = hlStartRows[m] + units.Length;
                    }
                    for (int i = hlStartRows[m]; i <= endRowCount; i++)
                    {
                        IRow row = sheetData.GetRow(i);
                        string[] str = new string[colCount];
                        if (row != null)
                        {
                            if (row.GetCell(0) == null || row.GetCell(0).ToString() == "")//第一列的数据如果为空，那么结束该循环
                            {
                                break;
                            }
                            for (int j = 0; j < colCount; j++)//循环该行的所有cell
                            {
                                if (row.GetCell(j) != null)
                                {
                                    string value = row.GetCell(j).ToString();

                                    if ((m == 4 || m == 5) && j == 0) //设置表5表6的单位代码
                                    {
                                        if (value == "合计" || (limit == 4 && m == 5 && unitCode.StartsWith("33")))
                                        {
                                            value = value + "|" + unitCode;
                                        }
                                        else
                                        {
                                            for (int h = 0; h < units.Length; h++)
                                            {
                                                string[] unitInfo = units[h].Split('|');
                                                if (value == unitInfo[0])
                                                {
                                                    value = units[h];
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (m == 4 && j == 7) //设置表5的死亡原因
                                    {
                                        for (int h = 0; h < deathList.Length; h++)
                                        {
                                            deathList[h].Split('|');
                                            if (value == deathList[h].Split('|')[0])
                                            {
                                                value = deathList[h];
                                                break;
                                            }
                                        }
                                    }
                                    if (m == 4 && j == 6 && value != "") //转换死亡时间格式
                                    {
                                        try
                                        {
                                            value = Convert.ToDateTime(value).Date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                                        }
                                        catch
                                        {
                                            value = "";
                                        }
                                    }
                                    if ((m == 4 && j == 10) || (m == 5 && j == 19)) //设置表5表6的流域
                                    {
                                        if (m == 5 && i == hlStartRows[m])
                                        {
                                            value = "";
                                            break;
                                        }
                                        for (int h = 0; h < river.Length; h++)
                                        {
                                            if (value == river[h].Split('|')[0])
                                            {
                                                value = river[h];
                                                break;
                                            }
                                        }
                                    }
                                    str[j] = value;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        arrList.Add(str);
                    }
                    list.Add(arrList);
                }
            }
            #endregion

            return list;
        }

        public ArrayList ImportHPExcel(string fileName, string unitCode, int[] hpStartRows, int[] hpMaxCols)
        {
            #region 初始化数据
            IList<TB07_District> lowerUnits = getLowerUnits(unitCode, "HP01");
            string unitsStr = "";
            foreach (var obj in lowerUnits)
            {
                unitsStr += obj.DistrictName + "|" + obj.DistrictCode + ",";
            }

            if (unitsStr != "")
            {
                unitsStr = unitsStr.Remove(unitsStr.Length - 1);
            }
            string[] units = unitsStr.Split(',');
            int unitsCount = lowerUnits.Count + 1;//+1是添加合计行
            ArrayList list = new ArrayList();
            IList<TB43_Reservoir> tb43s = getReservoirs(unitCode);
            string shuikuStr = "";
            foreach (var obj in tb43s)
            {
                shuikuStr += obj.RSName + "|" + obj.RSCode + ",";
            }
            if (shuikuStr != "")
            {
                shuikuStr = shuikuStr.Remove(shuikuStr.Length - 1);
            }
            string[] reservoirs = shuikuStr.Split(',');

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            #endregion
            for (int m = 0; m < 4; m++)
            {
                ArrayList arrList = new ArrayList(); //存放第 k 张表的数据
                ISheet sheetData = hssfworkbook.GetSheetAt(m);//取第i个表
                int maxColCount = hpMaxCols[m];//列的最大值
                int endRowCount = hpStartRows[m] + units.Length;//循环中可以等于该值，固不需要加1

                #region 表1 表2
                if (m == 0 || m == 1)//表1表2
                {
                    for (int i = hpStartRows[m]; i <= endRowCount; i++)
                    {
                        IRow row = sheetData.GetRow(i);
                        string[] str = new string[maxColCount];
                        if (row != null)
                        {
                            if (row.GetCell(0) == null || row.GetCell(0).ToString() == "")//第一列的数据如果为空，那么结束该循环
                            {
                                break;
                            }
                            for (int j = 0; j < maxColCount; j++)//循环该行的所有cell
                            {
                                if (row.GetCell(j) != null)
                                {
                                    string value = row.GetCell(j).ToString();
                                    if (j == 0) //设置表1表2的单位代码
                                    {
                                        for (int h = 0; h < units.Length; h++)
                                        {
                                            string[] unitInfo = units[h].Split('|');
                                            if (value == unitInfo[0])
                                            {
                                                value = units[h];
                                                break;
                                            }
                                        }
                                    }
                                    str[j] = value;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        arrList.Add(str);
                    }
                }
                #endregion

                #region 表3 表4
                else if (m == 2 || m == 3)//表3表4
                {
                    maxColCount = 6;
                    for (int index = 0; index <= 2; index++)
                    {
                        for (int i = hpStartRows[m]; i <= endRowCount; i++)
                        {
                            IRow row = sheetData.GetRow(i);
                            string[] str = new string[maxColCount];
                            if (row != null)
                            {
                                if (row.GetCell(0 + index * 6) == null || row.GetCell(0 + index * 6).ToString() == "")//水库列的数据如果为空，那么结束该循环
                                {
                                    break;
                                }
                                for (int j = 0 + index * 6; j < maxColCount + index * 6; j++)//循环该行的所有cell
                                {
                                    if (row.GetCell(j) != null)
                                    {
                                        string value = row.GetCell(j).ToString();

                                        if (j == index * 6) //设置表3表4的水库代码
                                        {
                                            for (int h = 0; h < reservoirs.Length; h++)
                                            {
                                                string[] reservoirsInfo = reservoirs[h].Split('|');
                                                if (value == reservoirsInfo[0])
                                                {
                                                    value = reservoirs[h];
                                                    break;
                                                }
                                            }
                                        }
                                        str[j - index * 6] = value;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                            arrList.Add(str);
                        }
                    }
                }
                #endregion

                list.Add(arrList);
            }
            return list;
        }
        #endregion
        /// <summary>删除Excel文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void deleteExcel(string fileName)
        {
            try
            {
                string file = System.Web.HttpContext.Current.Server.MapPath(fileName);
                if (System.IO.File.Exists(file)) //存在文件 
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // 1 是 FileAttributes.ReadOnly 的值 , 0是正常的属性值
                    fileInfo.Attributes = 0;
                    System.IO.File.Delete(file);
                }
            }
            catch (Exception e)
            {
                return;
            }
        }

        #region 导出国统表

        public string ExportGTExcel(int limit, int pageNO, string unitCode)
        {
            string result = "";
            IList<TB07_District> tb07List = getLowerUnits(unitCode, "HP01");
            IList<LZHP011> xmmHPList1 = GetLZHP01(pageNO, limit, tb07List);//表1
            string value = "";
            for (int i = 0; i < xmmHPList1.Count; i++)
            {
                //String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp).Replace(",", "");
                value =
                    string.Format("{0:N2}",
                        Convert.ToDouble(xmmHPList1[i].SPTXXS == "" ? "0" : xmmHPList1[i].SPTXXS) +
                        Convert.ToDouble(xmmHPList1[i].XRKXXS == "" ? "0" : xmmHPList1[i].XRKXXS)).Replace(",", "");
                xmmHPList1[i].SPTXXS = value == "0.00" ? "" : value;
            }

            ReportTitle rpt = getRptInfo(pageNO);
            Dictionary<string, string> fieldDic = new Dictionary<string, string>();
            fieldDic = GetFieldsData(limit);
            string tableFields = "DW,XXSLZJ,XXSLZJ,DZXKCS,DZKXXSL,DZKXXSL,ZZXKCS,ZZKXXSL,ZZKXXSL,XYSKCS,XYKXXS,XYKXXS,SPTXXS,SPTXXS";
            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "ExcelModel/xushuizongji.xls", FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            ISheet sheet1 = hssfworkbook.GetSheet("表1");
            if (limit == 2)
            {
                var underUnits = fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode || t.DistrictCode == unitCode).Select(t => t.DistrictCode).ToList();
                string shortTime = Convert.ToDateTime(rpt.EndDateTime).AddYears(-1).Year + "-" + Convert.ToDateTime(rpt.EndDateTime).Month + "-" + Convert.ToDateTime(rpt.EndDateTime).Day;
                string str = "";
                var tb63s = (from tb63 in fxdict.TB63_HunanHisHPData
                             where underUnits.Contains(tb63.UnitCode) &&
                                   tb63.RptTime == shortTime
                             select tb63).ToList();
                sheet1 = GTTable1(sheet1, xmmHPList1, rpt, limit, tableFields, fieldDic, tb63s);
            }
            else
            {
                IList<GTHP01> WNData = GetWNWTData(Convert.ToDateTime(rpt.StartDateTime), Convert.ToDateTime(rpt.EndDateTime), limit, unitCode);
                IList<GTHP01> eightWNData = Get08YearTData(Convert.ToDateTime(rpt.StartDateTime), Convert.ToDateTime(rpt.EndDateTime), limit, unitCode);
                IList<GTHP01> newWNData = CalculateData(WNData, eightWNData, tb07List, limit);
                sheet1 = GTTable1(sheet1, xmmHPList1, rpt, limit, tableFields, fieldDic, newWNData);
            }
            result = ResponseExcel(hssfworkbook);
            return result;
        }

        public ISheet GTTable1(ISheet sheet1, IList<LZHP011> xmmhp011s, ReportTitle rpt, int limit, string fields, Dictionary<string, string> dic, IList<GTHP01> newWNData)
        {
            sheet1 = CreateCell(sheet1, xmmhp011s.Count + 1, 14, 11);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 14; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(9).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }

            for (int i = 0; i < xmmhp011s.Count; i++)//14个cell 从第11行开始 
            {
                //DW,XXSLZJ,XXSLZJ,DZXKCS,DZKXXSL,DZKXXSL,ZZXKCS,ZZKXXSL,ZZKXXSL,XYSKCS,XYKXXS,XYKXXS,SPTXXS,SPTXXS
                sheet1.GetRow(11 + i).GetCell(0).SetCellValue(xmmhp011s[i].DW);
                sheet1.GetRow(11 + i).GetCell(1).SetCellValue(xmmhp011s[i].XXSLZJ);
                sheet1.GetRow(11 + i).GetCell(2).SetCellValue(newWNData[i].XXSLZJ.ToString() == "0" ? "" : newWNData[i].XXSLZJ.ToString());

                sheet1.GetRow(11 + i).GetCell(3).SetCellValue(xmmhp011s[i].DZXKCS);
                sheet1.GetRow(11 + i).GetCell(4).SetCellValue(xmmhp011s[i].DZKXXSL);
                sheet1.GetRow(11 + i).GetCell(5).SetCellValue(newWNData[i].DZKXXSL.ToString() == "0" ? "" : newWNData[i].DZKXXSL.ToString());

                sheet1.GetRow(11 + i).GetCell(6).SetCellValue(xmmhp011s[i].ZZXKCS);
                sheet1.GetRow(11 + i).GetCell(7).SetCellValue(xmmhp011s[i].ZZKXXSL);
                sheet1.GetRow(11 + i).GetCell(8).SetCellValue(newWNData[i].ZZKXXSL.ToString() == "0" ? "" : newWNData[i].ZZKXXSL.ToString());

                sheet1.GetRow(11 + i).GetCell(9).SetCellValue(xmmhp011s[i].XYSKCS);
                sheet1.GetRow(11 + i).GetCell(10).SetCellValue(xmmhp011s[i].XYKXXS);
                sheet1.GetRow(11 + i).GetCell(12).SetCellValue(newWNData[i].XYKXXS.ToString() == "0" ? "" : newWNData[i].XYKXXS.ToString());

                sheet1.GetRow(11 + i).GetCell(12).SetCellValue(xmmhp011s[i].SPTXXS);
                sheet1.GetRow(11 + i).GetCell(13).SetCellValue(newWNData[i].SPTXXS.ToString() == "0" ? "" : newWNData[i].SPTXXS.ToString());
            }
            //填写表头信息 
            //CellRangeAddress cellRangeAddress = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 0, 2);//合并单元格
            //CellRangeAddress cellRangeAddress1 = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 3, 7);
            //CellRangeAddress cellRangeAddress2 = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 8, 11);
            //sheet1.AddMergedRegion(cellRangeAddress);
            //sheet1.AddMergedRegion(cellRangeAddress1);
            //sheet1.AddMergedRegion(cellRangeAddress2);

            sheet1.GetRow(1).GetCell(0).SetCellValue("（ " + Convert.ToDateTime(rpt.EndDateTime).Year + " 年 " + Convert.ToDateTime(rpt.EndDateTime).Month + " 月 ）");
            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            //sheet1.GetRow(11).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(11 + xmmhp011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(11 + xmmhp011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(11 + xmmhp011s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(11 + xmmhp011s.Count).GetCell(8).SetCellValue("报出日期：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 11; x < 11 + xmmhp011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 14; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        public ISheet GTTable1(ISheet sheet1, IList<LZHP011> xmmhp011s, ReportTitle rpt, int limit, string fields, Dictionary<string, string> dic, IList<TB63_HunanHisHPData> hisData)
        {
            sheet1 = CreateCell(sheet1, xmmhp011s.Count + 1, 14, 10);
            string[] fieldsArr = fields.Split(',');
            for (int j = 1; j < 14; j++)
            {
                if (dic.ContainsKey(fieldsArr[j]))
                {
                    sheet1.GetRow(9).GetCell(j).SetCellValue(dic[fieldsArr[j]]);
                }
            }

            for (int i = 0; i < xmmhp011s.Count; i++)//14个cell 从第11行开始 
            {
                TB63_HunanHisHPData tb63 = hisData.Where(t => t.UnitCode == xmmhp011s[i].UnitCode).SingleOrDefault();
                //DW,XXSLZJ,XXSLZJ,DZXKCS,DZKXXSL,DZKXXSL,ZZXKCS,ZZKXXSL,ZZKXXSL,XYSKCS,XYKXXS,XYKXXS,SPTXXS,SPTXXS
                sheet1.GetRow(10 + i).GetCell(0).SetCellValue(xmmhp011s[i].DW);
                sheet1.GetRow(10 + i).GetCell(1).SetCellValue(xmmhp011s[i].XXSLZJ);
                sheet1.GetRow(10 + i).GetCell(2).SetCellValue(tb63 == null ? "" : tb63.ZJLNXSL.ToString());

                sheet1.GetRow(10 + i).GetCell(3).SetCellValue(xmmhp011s[i].DZXKCS);
                sheet1.GetRow(10 + i).GetCell(4).SetCellValue(xmmhp011s[i].DZKXXSL);
                sheet1.GetRow(10 + i).GetCell(5).SetCellValue(tb63 == null ? "" : tb63.DXLNXSL.ToString());

                sheet1.GetRow(10 + i).GetCell(6).SetCellValue(xmmhp011s[i].ZZXKCS);
                sheet1.GetRow(10 + i).GetCell(7).SetCellValue(xmmhp011s[i].ZZKXXSL);
                sheet1.GetRow(10 + i).GetCell(8).SetCellValue(tb63 == null ? "" : tb63.ZXLNXSL.ToString());

                sheet1.GetRow(10 + i).GetCell(9).SetCellValue(xmmhp011s[i].XYSKCS);
                sheet1.GetRow(10 + i).GetCell(10).SetCellValue(xmmhp011s[i].XYKXXS);
                sheet1.GetRow(10 + i).GetCell(11).SetCellValue(tb63 == null ? "" : tb63.XXLNXSL.ToString());

                sheet1.GetRow(10 + i).GetCell(12).SetCellValue(xmmhp011s[i].SPTXXS);
                sheet1.GetRow(10 + i).GetCell(13).SetCellValue(tb63 == null ? "" : tb63.QTLNXSL.ToString());
            }
            //填写表头信息 
            //CellRangeAddress cellRangeAddress = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 0, 2);//合并单元格
            //CellRangeAddress cellRangeAddress1 = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 3, 7);
            //CellRangeAddress cellRangeAddress2 = new CellRangeAddress(4 + xmmhp011s.Count, 4 + xmmhp011s.Count, 8, 11);
            //sheet1.AddMergedRegion(cellRangeAddress);
            //sheet1.AddMergedRegion(cellRangeAddress1);
            //sheet1.AddMergedRegion(cellRangeAddress2);

            sheet1.GetRow(1).GetCell(0).SetCellValue("（ " + Convert.ToDateTime(rpt.EndDateTime).Year + " 年 " + Convert.ToDateTime(rpt.EndDateTime).Month + " 月 ）");
            sheet1.GetRow(4).GetCell(0).SetCellValue("填报单位：" + rpt.UnitName);
            //sheet1.GetRow(11).GetCell(0).SetCellValue("起止日期：" + Convert.ToDateTime(rpt.StartDateTime).ToString("yyyy/MM/dd") + "～" + Convert.ToDateTime(rpt.EndDateTime).ToString("yyyy/MM/dd"));
            sheet1.GetRow(10 + xmmhp011s.Count).GetCell(0).SetCellValue("单位负责人：" + rpt.UnitPrincipal);
            sheet1.GetRow(10 + xmmhp011s.Count).GetCell(3).SetCellValue("统计负责人：" + rpt.StatisticsPrincipal);
            sheet1.GetRow(10 + xmmhp011s.Count).GetCell(6).SetCellValue("填报人：" + rpt.WriterName);
            sheet1.GetRow(10 + xmmhp011s.Count).GetCell(8).SetCellValue("报出日期：" + Convert.ToDateTime(rpt.WriterTime).ToString("yyyy/MM/dd"));
            //设置单元格格式
            ICellStyle cellStyle = SetCellStyle(sheet1);
            for (int x = 10; x < 10 + xmmhp011s.Count; x++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int y = 0; y < 14; y++)
                {
                    sheet1.GetRow(x).GetCell(y).CellStyle = cellStyle;
                }
            }
            return sheet1;
        }

        /// <summary>获取除08以外的往年数据
        /// </summary>
        /// <param name="startTime">当前报表开始时间</param>
        /// <param name="endTime">当前报表结束时间</param>
        /// <param name="limit">当前单位级别</param>
        /// <param name="unitCode">当前单位代码</param>
        /// <returns></returns>
        public IList<GTHP01> GetWNWTData(DateTime startTime, DateTime endTime, int limit, string unitCode)
        {
            Entities entities = new Entities();
            BusinessEntities busEntity = (BusinessEntities)entities.GetPersistenceEntityByLevel(limit);
            IList<GTHP01> dataList = null;

            int j = 1;
            string sql = "";
            string startSql = "";
            string endSql = "";
            string pageNOs = "";
            for (int m = startTime.AddYears(-1).Year; m > 2008; m--)
            {
                sql = "select ISNULL ( max(PAGENO), 0)  as PAGENO  from reporttitle where StartDateTime = '" + startTime.AddYears(-j) +
                      "' and EndDateTime  = '" + endTime.AddYears(-j) + "' and UnitCode = '" + unitCode + "'";
                dataList = busEntity.ExecuteStoreQuery<GTHP01>(sql).ToList();
                if (dataList[0].PAGENO != null && dataList[0].PAGENO != 0)
                {
                    pageNOs += dataList[0].PAGENO + ",";
                }
                j++;
            }
            dataList.Clear();
            if (pageNOs != "")
            {
                pageNOs = pageNOs.Remove(pageNOs.Length - 1);
                string sqlSum = "select dw, isnull(sum(XXSLZJ),0) as XXSLZJ,isnull(sum(DZKXXSL),0) as DZKXXSL,isnull(sum(ZZKXXSL),0) as ZZKXXSL,isnull(sum(XYKXXS),0) as XYKXXS ,isnull(sum(XRKXXS + SPTXXS),0) as XRKXXS from hp011 where pageno in (" + pageNOs + ") group by dw";
                dataList = busEntity.ExecuteStoreQuery<GTHP01>(sqlSum).ToList();
            }

            return dataList;
        }

        /// <summary>获取08的数据,并已经乘以35
        /// </summary>
        /// <param name="startTime">当前报表开始时间</param>
        /// <param name="endTime">当前报表结束时间</param>
        /// <param name="limit">当前单位级别</param>
        /// <param name="unitCode">当前单位代码</param>
        /// <returns></returns>
        public IList<GTHP01> Get08YearTData(DateTime startTime, DateTime endTime, int limit, string unitCode)
        {
            Entities entities = new Entities();
            BusinessEntities busEntity = (BusinessEntities)entities.GetPersistenceEntityByLevel(limit);
            IList<GTHP01> dataList = null;

            string sql = "";
            string pageNO = "";
            for (int m = startTime.AddYears(-1).Year; m > 2008; m--)
            {
                sql = "select ISNULL (max(PAGENO),0) as PAGENO from reporttitle where StartDateTime = '" + Convert.ToDateTime("2008-" + startTime.Month + "-" + startTime.Day) + "' and EndDateTime  = '" + Convert.ToDateTime("2008-" + endTime.Month + "-" + endTime.Day) + "'    and UnitCode = '" + unitCode + "'";
                dataList = busEntity.ExecuteStoreQuery<GTHP01>(sql).ToList();
                if (dataList[0].PAGENO != null && dataList[0].PAGENO != 0)
                {
                    pageNO = dataList[0].PAGENO.ToString();
                }
            }
            dataList.Clear();
            if (pageNO != "")
            {
                string sqlSum =
                    "select dw, isnull(sum(XXSLZJ)*35,0) as XXSLZJ,isnull(sum(DZKXXSL)*35,0) as DZKXXSL,isnull(sum(ZZKXXSL)*35,0) as ZZKXXSL,isnull(sum(XYKXXS)*35,0) as XYKXXS ,isnull(sum(XRKXXS + SPTXXS)*35,0) as XRKXXS from hp011 where pageno ='" +
                    pageNO + "' group by dw";
                dataList = busEntity.ExecuteStoreQuery<GTHP01>(sqlSum).ToList();
            }
            return dataList;
        }

        /// <summary>合并计算常年同期蓄水
        /// </summary>
        /// <param name="rhp01List"></param>
        /// <param name="eightRhp01List"></param>
        /// <param name="lowerUnits"></param>
        /// <returns></returns>
        public IList<GTHP01> CalculateData(IList<GTHP01> rhp01List, IList<GTHP01> eightRhp01List,
            IList<TB07_District> lowerUnits, int limit)
        {
            IList<GTHP01> list = new List<GTHP01>();
            IList<GTHP01> newHP01List = new List<GTHP01>();
            XMMZHClass xmm = new XMMZHClass();
            #region //合并成一个IList<GTHP01>

            if (rhp01List.Count <= 0)
            {
                newHP01List = eightRhp01List;
            }
            else
            {
                for (int i = 0; i < rhp01List.Count; i++)
                {
                    for (int j = 0; j < eightRhp01List.Count; j++)
                    {
                        if (rhp01List[i].DW.Equals(eightRhp01List[j].DW))
                        {
                            rhp01List[i].XXSLZJ = rhp01List[i].XXSLZJ + eightRhp01List[j].XXSLZJ;
                            rhp01List[i].DZKXXSL = rhp01List[i].DZKXXSL + eightRhp01List[j].DZKXXSL;
                            rhp01List[i].ZZKXXSL = rhp01List[i].ZZKXXSL + eightRhp01List[j].ZZKXXSL;
                            rhp01List[i].XYKXXS = rhp01List[i].XYKXXS + eightRhp01List[j].XYKXXS;
                            rhp01List[i].XRKXXS = rhp01List[i].XRKXXS + eightRhp01List[j].XRKXXS;
                        }
                    }
                    newHP01List.Add(rhp01List[i]);
                }
            }

            #endregion

            int sumFlag = 0;//是否已经添加合计行
            int flag = 0;//是否有该单位的数据
            foreach (var lower in lowerUnits)
            {
                flag = 0;
                for (int i = 0; i < newHP01List.Count; i++)
                {
                    if (sumFlag == 0 && newHP01List[i].DW.Equals("合计"))
                    {
                        GTHP01 rHp011 = new GTHP01();
                        rHp011.DW = newHP01List[i].DW;
                        rHp011.XXSLZJ = newHP01List[i].XXSLZJ / 39;
                        rHp011.DZKXXSL = newHP01List[i].DZKXXSL / 39;
                        rHp011.ZZKXXSL = newHP01List[i].ZZKXXSL / 39;
                        rHp011.XYKXXS = newHP01List[i].XYKXXS / 39;
                        rHp011.XRKXXS = newHP01List[i].XRKXXS / 39;
                        rHp011 = ConvertHLToXMMHL<GTHP01, GTHP01>(rHp011, limit);
                        list.Add(rHp011);
                        sumFlag = 1; //合计已经添加了
                        break;
                    }
                }
                if (sumFlag == 0)//如果没得数据，那么第一条是添加的合计
                {
                    GTHP01 hejiHp011 = new GTHP01();
                    hejiHp011.DW = "合计";
                    //hejiHp011.XXSLZJ = 0;
                    list.Add(hejiHp011);
                    sumFlag = 1;
                }
                for (int i = 0; i < rhp01List.Count; i++)
                {
                    if (lower.DistrictName.Equals(newHP01List[i].DW))
                    {
                        GTHP01 rHp011 = new GTHP01();
                        rHp011.DW = lower.DistrictName;
                        rHp011.XXSLZJ = newHP01List[i].XXSLZJ / 39;
                        rHp011.DZKXXSL = newHP01List[i].DZKXXSL / 39;
                        rHp011.ZZKXXSL = newHP01List[i].ZZKXXSL / 39;
                        rHp011.XYKXXS = newHP01List[i].XYKXXS / 39;
                        rHp011.XRKXXS = newHP01List[i].XRKXXS / 39;
                        rHp011 = ConvertHLToXMMHL<GTHP01, GTHP01>(rHp011, limit);
                        list.Add(rHp011);
                        flag = 1;//该单位有数据
                        break;
                    }
                }
                if (flag == 0)
                {
                    GTHP01 rHp011null = new GTHP01();
                    rHp011null.DW = lower.DistrictName;
                    list.Add(rHp011null);
                }
            }
            return list;
        }
        public T ConvertHLToXMMHL<T, T2>(T2 hl, int limit)
            where T : new()
            where T2 : new()
        {
            PropertyInfo[] pfs = hl.GetType().GetProperties();//利用反射获得类的属性

            T xmmhl011 = new T();
            PropertyInfo[] fixmm = xmmhl011.GetType().GetProperties();
            TableFieldBaseData tbBaseData = new TableFieldBaseData();
            string[] arr = { };
            string temp = "";
            decimal shuliangji = 0;
            double xiaoshu = 0;
            decimal changetemp = 0;
            decimal tempv = 0;
            ReportTitle rep = null;
            for (int j = 0; j < fixmm.Length; j++)
            {
                arr = tbBaseData.GetFieldUnitArr(fixmm[j].Name, limit);
                if (arr != null)
                {
                    shuliangji = Convert.ToDecimal(arr[0]);
                    xiaoshu = Convert.ToDouble(arr[1]);
                }
                //找出hl011的各个属性的名字
                for (int i = 0; i < pfs.Length; i++)
                {
                    if (fixmm[j].Name.ToUpper() == pfs[i].Name.ToUpper())
                    {
                        if (pfs[i].PropertyType.FullName == "System.String")
                        {
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = "";
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }
                            fixmm[j].SetValue(xmmhl011, temp, null);
                        }

                        else if (pfs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                        {
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = "0";
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }
                            changetemp = Convert.ToDecimal(temp);
                            if (changetemp != 0)
                            {
                                if (shuliangji == 0)
                                {
                                    temp = String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp).Replace(",", "");
                                }
                                else
                                {
                                    temp = String.Format("{0:N" + Convert.ToInt32(xiaoshu) + "}", changetemp / shuliangji).Replace(",", "");
                                }
                            }
                            else
                            {
                                temp = "0";
                            }
                            fixmm[j].SetValue(xmmhl011, Convert.ToDecimal(temp), null);
                        }
                        else if (pfs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                        {
                            if (pfs[i].GetValue(hl, null) == null)
                            {
                                temp = "0";
                            }
                            else
                            {
                                temp = pfs[i].GetValue(hl, null).ToString();
                            }
                            fixmm[j].SetValue(xmmhl011, Convert.ToInt32(temp), null);
                        }
                        break;
                    }

                }
            }
            return xmmhl011;
        }
        #endregion
    }
}
