using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Aspose.Words.Tables;
using DBHelper;
using EntityModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace LogicProcessingClass.ReportOperate
{
    public class ExcelOper
    {
        BusinessEntities business = null;
        FXDICTEntities fxdict = null;
        ISheet sheet = null;

        public ExcelOper(int limit)
        {
            Entities getEntity = new Entities();
            business = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
            fxdict = (FXDICTEntities)getEntity.GetPersistenceEntityByEntityName(EntitiesConnection.entityName.FXDICTEntities);
        }

        public string To_SH01(int pageno)  //, string ord_code
        {
            string response = "";
            string fileUrl = "ExcelModel/sh/sh01/";
            bool isCurYear = true;
            HSSFWorkbook hssfworkbook;
            ReportTitle rptTitle = business.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault();

            if (rptTitle.EndDateTime.Value.Year == DateTime.Now.Year) //是否为今年的数据
            {
                fileUrl += "cur_year.xls";
                isCurYear = true;
            }
            else
            {
                fileUrl += "over_year.xls";
                isCurYear = false;
            }

            using (FileStream file =
                new FileStream(AppDomain.CurrentDomain.BaseDirectory + fileUrl,
                    FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            ISheet sheet_zjqk = hssfworkbook.GetSheet("资金情况表");
            
            WriteReportTitleInfo(rptTitle, sheet_zjqk);

            List<SH011> sh011S = business.SH011.Where(t => t.PageNO == pageno).OrderBy(t => t.DataOrder).ToList();
            int rowIndex = 0;
            foreach (SH011 sh011 in sh011S)
            {
                sheet_zjqk.GetRow(7 + rowIndex).GetCell(1).SetCellValue(Console(sh011.NDZJYS, 2));
                sheet_zjqk.GetRow(7 + rowIndex).GetCell(2).SetCellValue(Console(sh011.NDZJZY, 2));
                sheet_zjqk.GetRow(7 + rowIndex).GetCell(3).SetCellValue(Console(sh011.NDZJDF, 2));
                sheet_zjqk.GetRow(7 + rowIndex).GetCell(4).SetCellValue(Console(sh011.DFZJSJ, 2));
                sheet_zjqk.GetRow(7 + rowIndex).GetCell(5).SetCellValue(Console(sh011.DFZJSX, 2));
                sheet_zjqk.GetRow(7 + rowIndex).GetCell(6).SetCellValue(Console(sh011.ZJZF, 2));
                //以下为新增的字段
                if (isCurYear)
                {
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(6).SetCellValue(Console(sh011.DFZJX, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(7).SetCellValue(Console(sh011.JDWCZY, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(8).SetCellValue(Console(sh011.JDZYBL, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(9).SetCellValue(Console(sh011.JDWCSJ, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(10).SetCellValue(Console(sh011.JDSJBL, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(11).SetCellValue(Console(sh011.JDWCSHIJ, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(12).SetCellValue(Console(sh011.JDSHIJBL, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(13).SetCellValue(Console(sh011.JDWCXJ, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(14).SetCellValue(Console(sh011.JDXJBL, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(15).SetCellValue(Console(sh011.ZFWCZY, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(16).SetCellValue(Console(sh011.ZFZYBL, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(17).SetCellValue(Console(sh011.ZFWCSJ, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(18).SetCellValue(Console(sh011.ZFSJBL, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(19).SetCellValue(Console(sh011.ZFWCSHIJ, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(20).SetCellValue(Console(sh011.ZFSHIJBL, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(21).SetCellValue(Console(sh011.ZFWCXJ, 2));
                    sheet_zjqk.GetRow(7 + rowIndex).GetCell(22).SetCellValue(Console(sh011.ZFXJBL, 2));
                }
                rowIndex++;
            }

            response = ResponseExcel(hssfworkbook, rptTitle.UnitName + "资金情况表");

            return response;
        }

        public string To_SH02(int pageno)
        {
            string response = "";
            HSSFWorkbook hssfworkbook;
            using (FileStream file =
                new FileStream(AppDomain.CurrentDomain.BaseDirectory + "ExcelModel/sh/sh02/cur_year.xls",
                    FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            ReportTitle rptTitle = business.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault();
            sheet = hssfworkbook.GetSheet("山洪沟治理进度统计表");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH021> sh021S = business.SH021.Where(t => t.PageNO == pageno).ToList();
            sheet = CreateCell(sheet, sh021S.Count, 17, 6);
            int index = 0;
            foreach (SH021 sh021 in sh021S)
            {
                sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh021.GDMC);
                sheet.GetRow(6 + index).GetCell(2).SetCellValue(sh021.SZX);
                sheet.GetRow(6 + index).GetCell(3).SetCellValue(sh021.CBSJBG);
                sheet.GetRow(6 + index).GetCell(4).SetCellValue(sh021.SCTG);
                sheet.GetRow(6 + index).GetCell(5).SetCellValue(sh021.ZBQK);
                sheet.GetRow(6 + index).GetCell(6).SetCellValue(sh021.SFKG);
                sheet.GetRow(6 + index).GetCell(7).SetCellValue(Console(sh021.JHDF, 0));
                sheet.GetRow(6 + index).GetCell(8).SetCellValue(Console(sh021.JHHA, 0));
                sheet.GetRow(6 + index).GetCell(9).SetCellValue(Console(sh021.JHQY, 0));
                sheet.GetRow(6 + index).GetCell(10).SetCellValue(Console(sh021.JHJHG, 0));
                sheet.GetRow(6 + index).GetCell(11).SetCellValue(Console(sh021.JHQT, 0));
                sheet.GetRow(6 + index).GetCell(12).SetCellValue(Console(sh021.JHTF, 0));
                sheet.GetRow(6 + index).GetCell(13).SetCellValue(Console(sh021.JHSF, 0));
                sheet.GetRow(6 + index).GetCell(14).SetCellValue(Console(sh021.JHHNT, 0));
                sheet.GetRow(6 + index).GetCell(15).SetCellValue(Console(sh021.JD, 0));
                sheet.GetRow(6 + index).GetCell(16).SetCellValue(Console(sh021.JGYS, 0));
                index++;
            }
            CellStyle(sh021S.Count, 17, 6);

            response = ResponseExcel(hssfworkbook, rptTitle.UnitName + "山洪沟治理进度统计表");
            sheet = null;
            return response;
        }

        public string To_SH05(int pageno)
        {
            string response = "";
            HSSFWorkbook hssfworkbook;
            using (FileStream file =
                new FileStream(AppDomain.CurrentDomain.BaseDirectory + "ExcelModel/sh/sh05/cur_year.xls",
                    FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            var time_str = business.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault().WriterTime.ToString();
            DateTime WriterTime = DateTime.Parse(time_str);

            sheet = hssfworkbook.GetSheet("效益发挥统计表");
            sheet.GetRow(1).GetCell(0).SetCellValue("统计时段：（" + WriterTime.ToString("yyyy") + "-01-01 至 " + WriterTime.ToString("yyyy-MM-dd") + "）");

            List<SH051> sh051S = business.SH051.Where(t => t.PageNO == pageno).ToList();
            int start_index = 7;
            sheet = CreateCell(sheet, sh051S.Count, 11, start_index);
            int index = 0;
            foreach (SH051 sh051 in sh051S){
                sheet.GetRow(start_index + index).GetCell(0).SetCellValue(sh051.DW);
                sheet.GetRow(start_index + index).GetCell(1).SetCellValue(sh051.YJFBGX == null ? "" : Convert.ToInt32(sh051.YJFBGX.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(2).SetCellValue(sh051.YJXJFBCS == null ? "" : Convert.ToInt32(sh051.YJXJFBCS.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(3).SetCellValue(sh051.YJDXTS == null ? "" : Convert.ToInt32(sh051.YJDXTS.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(4).SetCellValue(sh051.YJZRRS == null ? "" : Convert.ToInt32(sh051.YJZRRS.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(5).SetCellValue(sh051.QDGBZ == null ? "" : Convert.ToInt32(sh051.QDGBZ.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(6).SetCellValue(sh051.ZYRS == null ? "" : Convert.ToInt32(sh051.ZYRS.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(7).SetCellValue(sh051.BMSW == null ? "" : Convert.ToInt32(sh051.BMSW.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(8).SetCellValue(sh051.DTFW == null ? "" : Convert.ToInt32(sh051.DTFW.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(9).SetCellValue(sh051.SSSH == null ? "" : Convert.ToInt32(sh051.SSSH.Value).ToString());
                sheet.GetRow(start_index + index).GetCell(10).SetCellValue(sh051.BZ);
                index++;
            }
            CellStyle(sh051S.Count, 11, start_index);

            string UnitName = HttpContext.Current.Request.Cookies["unitname"].Value;
            response = ResponseExcel(hssfworkbook, HttpUtility.UrlDecode(UnitName) + "效益发挥统计表");
            sheet = null;
            return response;
        }

        public string To_SH03(int pageno)
        {
            string response = "";
            HSSFWorkbook hssfworkbook;
            using (FileStream file =
                new FileStream(AppDomain.CurrentDomain.BaseDirectory + "ExcelModel/sh/sh03/cur_year.xls",
                    FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            int index = 0;
            ReportTitle rptTitle = business.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault();

            //表1
            /*sheet = hssfworkbook.GetSheet("1、监测系统建设表");
            WriteReportTitleInfo(rptTitle, sheet);
            
            List<SH031> sh031S = business.SH031.Where(t => t.PageNO == pageno).ToList();
            if (sh031S.Any())
            {
                sheet = CreateCell(sheet, sh031S.Count, 14, 6);
                foreach (SH031 sh031 in sh031S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh031.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(Console(sh031.YLZJH, 0));
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(Console(sh031.YLZWC, 0));
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(Console(sh031.SWZJH, 0));
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(Console(sh031.SWZWC, 0));
                    sheet.GetRow(6 + index).GetCell(6).SetCellValue(Console(sh031.WXTDJH, 0));
                    sheet.GetRow(6 + index).GetCell(7).SetCellValue(Console(sh031.WXTHWC, 0));
                    sheet.GetRow(6 + index).GetCell(8).SetCellValue(Console(sh031.WXJCJH, 0));
                    sheet.GetRow(6 + index).GetCell(9).SetCellValue(Console(sh031.WXJSWC, 0));
                    sheet.GetRow(6 + index).GetCell(10).SetCellValue(Console(sh031.TXSPJCZJH, 0));
                    sheet.GetRow(6 + index).GetCell(11).SetCellValue(Console(sh031.TXSPJCZWC, 0));
                    sheet.GetRow(6 + index).GetCell(12).SetCellValue(Console(sh031.TXSPJCZCJH, 0));
                    sheet.GetRow(6 + index).GetCell(13).SetCellValue(Console(sh031.TXSPJCZCWC, 0));
                    index++;
                };
                CellStyle(sh031S.Count, 14, 6);
            }

            //表2
            sheet = hssfworkbook.GetSheet("2、预警系统建设表");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH032> sh032S = business.SH032.Where(t => t.PageNO == pageno).ToList();
            if (sh032S.Any())
            {
                sheet = CreateCell(sheet, sh032S.Count, 12, 6);
                index = 0;
                foreach (SH032 sh032 in sh032S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh032.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(Console(sh032.WXGBJH, 0));
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(Console(sh032.WXGBWC, 0));
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(Console(sh032.YLBJJH, 0));
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(Console(sh032.YLBJWC, 0));
                    sheet.GetRow(6 + index).GetCell(6).SetCellValue(Console(sh032.SWZJH, 0));
                    sheet.GetRow(6 + index).GetCell(7).SetCellValue(Console(sh032.SWZWC, 0));
                    sheet.GetRow(6 + index).GetCell(8).SetCellValue(Console(sh032.BJQJH, 0));
                    sheet.GetRow(6 + index).GetCell(9).SetCellValue(Console(sh032.BJQWC, 0));
                    sheet.GetRow(6 + index).GetCell(10).SetCellValue(Console(sh032.LGHJH, 0));
                    sheet.GetRow(6 + index).GetCell(11).SetCellValue(Console(sh032.LGHWC, 0));
                    index++;
                }
                CellStyle(sh032S.Count, 12, 6);
            }

            //表3
            sheet = hssfworkbook.GetSheet("3、县级平台完善表");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH033> sh033S = business.SH033.Where(t => t.PageNO == pageno).ToList();
            if (sh033S.Any())
            {
                sheet = CreateCell(sheet, sh033S.Count, 9, 6);
                index = 0;
                foreach (SH033 sh033 in sh033S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh033.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(sh033.XJHS);
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(sh033.XJYJ);
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(sh033.XJPT);
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(Console(sh033.XJPTYSJH, 0));
                    sheet.GetRow(6 + index).GetCell(6).SetCellValue(Console(sh033.XJPTYSWC, 0));
                    sheet.GetRow(6 + index).GetCell(7).SetCellValue(Console(sh033.YDSBJH, 0));
                    sheet.GetRow(6 + index).GetCell(8).SetCellValue(Console(sh033.YDSBWC, 0));
                    index++;
                }
                CellStyle(sh033S.Count, 9, 6);
            }

            //表4
            sheet = hssfworkbook.GetSheet("4、信息管理表");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH034> sh034S = business.SH034.Where(t => t.PageNO == pageno).ToList();
            if (sh034S.Any())
            {
                sheet = CreateCell(sheet, sh034S.Count, 8, 6);
                index = 0;
                foreach (SH034 sh034 in sh034S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh034.XZQMC);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(sh034.YJZB);
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(sh034.YJGZ);
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(sh034.RJZB);
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(sh034.RJGZ);
                    sheet.GetRow(6 + index).GetCell(6).SetCellValue(sh034.XTJC);
                    sheet.GetRow(6 + index).GetCell(7).SetCellValue(sh034.HTYS);
                    index++;
                }
                CellStyle(sh034S.Count, 8, 6);
            }

            //表5
            sheet = hssfworkbook.GetSheet("5、信息共享表");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH035> sh035S = business.SH035.Where(t => t.PageNO == pageno).ToList();
            if (sh035S.Any())
            {
                sheet = CreateCell(sheet, sh035S.Count, 6, 5);
                index = 0;
                foreach (SH035 sh035 in sh035S)
                {
                    sheet.GetRow(5 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(5 + index).GetCell(1).SetCellValue(sh035.XZQMC);
                    sheet.GetRow(5 + index).GetCell(2).SetCellValue(sh035.SWXXGX);
                    sheet.GetRow(5 + index).GetCell(3).SetCellValue(sh035.QXXXGX);
                    sheet.GetRow(5 + index).GetCell(4).SetCellValue(sh035.GTXXGX);
                    sheet.GetRow(5 + index).GetCell(5).SetCellValue(sh035.YJXXGX);
                    index++;
                }
                CellStyle(sh035S.Count, 6, 5);
            }*/

            //表6
            sheet = hssfworkbook.GetSheet("预案编制完善表");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH036> sh036S = business.SH036.Where(t => t.PageNO == pageno).ToList();
            if (sh036S.Any())
            {
                sheet = CreateCell(sheet, sh036S.Count, 10, 6);
                index = 0;
                foreach (SH036 sh036 in sh036S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh036.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(Console(sh036.XYAJH, 0));
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(Console(sh036.XYAWC, 0));
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(Console(sh036.XZYAJH, 0));
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(Console(sh036.XZYAWC, 0));
                    sheet.GetRow(6 + index).GetCell(6).SetCellValue(Console(sh036.CYAJH, 0));
                    sheet.GetRow(6 + index).GetCell(7).SetCellValue(Console(sh036.CYAWC, 0));
                    sheet.GetRow(6 + index).GetCell(8).SetCellValue(Console(sh036.QTYAJH, 0));
                    sheet.GetRow(6 + index).GetCell(9).SetCellValue(Console(sh036.QTYAWC, 0));
                    index++;
                }
                CellStyle(sh036S.Count, 10, 6);
            }

            //表7
            sheet = hssfworkbook.GetSheet("措施宣传表");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH037> sh037S = business.SH037.Where(t => t.PageNO == pageno).ToList();
            if (sh037S.Any())
            {
                sheet = CreateCell(sheet, sh037S.Count, 12, 6);
                index = 0;
                foreach (SH037 sh037 in sh037S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh037.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(Console(sh037.XCLJH, 0));
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(Console(sh037.XCLWC, 0));
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(Console(sh037.JSPJH, 0));
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(Console(sh037.JSPWC, 0));
                    sheet.GetRow(6 + index).GetCell(6).SetCellValue(Console(sh037.MBKJH, 0));
                    sheet.GetRow(6 + index).GetCell(7).SetCellValue(Console(sh037.MBKWC, 0));
                    sheet.GetRow(6 + index).GetCell(8).SetCellValue(Console(sh037.GPJH, 0));
                    sheet.GetRow(6 + index).GetCell(9).SetCellValue(Console(sh037.GPWC, 0));
                    sheet.GetRow(6 + index).GetCell(10).SetCellValue(Console(sh037.SCJH, 0));
                    sheet.GetRow(6 + index).GetCell(11).SetCellValue(Console(sh037.SCWC, 0));
                    index++;
                }
                CellStyle(sh037S.Count, 12, 6);
            }

            //表8
            sheet = hssfworkbook.GetSheet("培训演练表");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH038> sh038S = business.SH038.Where(t => t.PageNO == pageno).ToList();
            if (sh038S.Any())
            {
                sheet = CreateCell(sheet, sh038S.Count, 10, 6);
                index = 0;
                foreach (SH038 sh038 in sh038S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh038.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(Console(sh038.PXCCJH, 0));
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(Console(sh038.PXCCWC, 0));
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(Console(sh038.PXRSJH, 0));
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(Console(sh038.PXRSWC, 0));
                    sheet.GetRow(6 + index).GetCell(6).SetCellValue(Console(sh038.YLCCJH, 0));
                    sheet.GetRow(6 + index).GetCell(7).SetCellValue(Console(sh038.YLCCWC, 0));
                    sheet.GetRow(6 + index).GetCell(8).SetCellValue(Console(sh038.YLRCJH, 0));
                    sheet.GetRow(6 + index).GetCell(9).SetCellValue(Console(sh038.YLRCWC, 0));
                    index++;
                }
                CellStyle(sh038S.Count, 10, 6);
            }

            response = ResponseExcel(hssfworkbook, rptTitle.UnitName + "非工程措施补充完善表");
            sheet = null;

            return response;
        }

        public string To_SH04(int pageno)
        {
            string response = "";
            string fileUrl = "ExcelModel/sh/sh04/";
            bool isCurYear = true;
            HSSFWorkbook hssfworkbook;

            int index = 0;
            ReportTitle rptTitle = business.ReportTitle.Where(t => t.PageNO == pageno).SingleOrDefault();

            if (rptTitle.EndDateTime.Value.Year == DateTime.Now.Year) //是否为今年的数据
            {
                fileUrl += "cur_year.xls";
                isCurYear = true;
            }
            else
            {
                fileUrl += "over_year.xls";
                isCurYear = false;
            }

            using (FileStream file =
                new FileStream(AppDomain.CurrentDomain.BaseDirectory + fileUrl,
                    FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            //表1
            sheet = hssfworkbook.GetSheet("1、调查工作准备");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH041> sh041S = business.SH041.Where(t => t.PageNO == pageno).ToList();
            if (sh041S.Any())
            {
                sheet = CreateCell(sheet, sh041S.Count, 8, 6);
                foreach (SH041 sh041 in sh041S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh041.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(sh041.ZDYJZB);
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(sh041.ZDYJCG);
                    if (isCurYear)
                    {
                        sheet.GetRow(6 + index).GetCell(4).SetCellValue(sh041.PX);
                    }
                    else
                    {
                        sheet.GetRow(6 + index).GetCell(4).SetCellValue(Console(sh041.ZDYJCGSL, 0));
                        sheet.GetRow(6 + index).GetCell(5).SetCellValue(sh041.SJFWSQD);
                        sheet.GetRow(6 + index).GetCell(6).SetCellValue(sh041.SJFWSMC);
                        sheet.GetRow(6 + index).GetCell(7).SetCellValue(sh041.PX);
                    }
                    index++;
                }
                CellStyle(sh041S.Count, 5, 6);
            }

            //表2
            sheet = hssfworkbook.GetSheet("2、现场调查");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH042> sh042S = business.SH042.Where(t => t.PageNO == pageno).ToList();
            if (sh042S.Any())
            {
                sheet = CreateCell(sheet, sh042S.Count, 14, 6);
                index = 0;
                foreach (SH042 sh042 in sh042S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh042.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(sh042.DCDX);
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(sh042.SWJC);
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(sh042.DXLS);
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(sh042.LSSH);
                    sheet.GetRow(6 + index).GetCell(6).SetCellValue(Console(sh042.XLYJCXX, 0));
                    sheet.GetRow(6 + index).GetCell(7).SetCellValue(Console(sh042.SHJJDCJH, 0));
                    sheet.GetRow(6 + index).GetCell(8).SetCellValue(Console(sh042.SHJJDCWC, 0));
                    sheet.GetRow(6 + index).GetCell(9).SetCellValue(Console(sh042.SHWXQYJH, 0));
                    sheet.GetRow(6 + index).GetCell(10).SetCellValue(Console(sh042.SHWXQYWC, 0));
                    sheet.GetRow(6 + index).GetCell(11).SetCellValue(sh042.SSGC);
                    sheet.GetRow(6 + index).GetCell(12).SetCellValue(Console(sh042.YHCCLJH, 0));
                    sheet.GetRow(6 + index).GetCell(13).SetCellValue(Console(sh042.YHCCLWC, 0));
                    index++;
                }
                CellStyle(sh042S.Count, 14, 6);
            }

            //表3
            sheet = hssfworkbook.GetSheet("3、分析评价");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH043> sh043S = business.SH043.Where(t => t.PageNO == pageno).ToList();
            if (sh043S.Any())
            {
                sheet = CreateCell(sheet, sh043S.Count, 6, 6);
                index = 0;
                foreach (SH043 sh043 in sh043S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh043.SSX);
                    sheet.GetRow(6 + index).GetCell(2).SetCellValue(Console(sh043.XLYFX, 0));
                    sheet.GetRow(6 + index).GetCell(3).SetCellValue(Console(sh043.YHCPJJH, 0));
                    sheet.GetRow(6 + index).GetCell(4).SetCellValue(Console(sh043.YHCPJWC, 0));
                    sheet.GetRow(6 + index).GetCell(5).SetCellValue(sh043.DCBG);
                    index++;
                }
                CellStyle(sh043S.Count, 6, 6);
            }

            //表4
            sheet = hssfworkbook.GetSheet("4、审核汇集");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH044> sh044S = business.SH044.Where(t => t.PageNO == pageno).ToList();
            if (sh044S.Any())
            {
                sheet = CreateCell(sheet, sh044S.Count, 5, 5);
                index = 0;
                foreach (SH044 sh044 in sh044S)
                {
                    sheet.GetRow(5 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(5 + index).GetCell(1).SetCellValue(sh044.XZQMC);
                    sheet.GetRow(5 + index).GetCell(2).SetCellValue(sh044.XJWC);
                    sheet.GetRow(5 + index).GetCell(3).SetCellValue(sh044.S1JWC);
                    sheet.GetRow(5 + index).GetCell(4).SetCellValue(sh044.S2JWC);
                    index++;
                }
                CellStyle(sh044S.Count, 5, 5);
            }

            //表5
            sheet = hssfworkbook.GetSheet("5、调查评价培训");
            WriteReportTitleInfo(rptTitle, sheet);
            List<SH045> sh045S = business.SH045.Where(t => t.PageNO == pageno).ToList();
            if (sh045S.Any())
            {
                sheet = CreateCell(sheet, sh045S.Count, 4, 6);
                index = 0;
                foreach (SH045 sh045 in sh045S)
                {
                    sheet.GetRow(6 + index).GetCell(0).SetCellValue(index + 1);
                    sheet.GetRow(6 + index).GetCell(1).SetCellValue(sh045.XZQMC);
                    if (isCurYear)
                    {
                        sheet.GetRow(6 + index).GetCell(2).SetCellValue(Console(sh045.PXCSWC, 0));
                        sheet.GetRow(6 + index).GetCell(3).SetCellValue(Console(sh045.PXRSWC, 0));
                    }
                    else
                    {
                        sheet.GetRow(6 + index).GetCell(2).SetCellValue(Console(sh045.PXCSJH, 0));
                        sheet.GetRow(6 + index).GetCell(3).SetCellValue(Console(sh045.PXCSWC, 0));
                        sheet.GetRow(6 + index).GetCell(4).SetCellValue(Console(sh045.PXRSJH, 0));
                        sheet.GetRow(6 + index).GetCell(5).SetCellValue(Console(sh045.PXRSWC, 0));
                    }
                    index++;
                }
                CellStyle(sh045S.Count, 4, 6);
            }

            response = ResponseExcel(hssfworkbook, rptTitle.UnitName + "调查评价表");
            sheet = null;
            return response;
        }

        private string Console(object obj, int _fixed)
        {
            try
            {
                if (obj == null)
                {
                    return "";
                }

                string str = "";
                switch (_fixed)
                {
                    case 2:
                        str = "0.00";
                        break;
                    case 3:
                        str = "0.000";
                        break;
                    case 4:
                        str = "0.0000";
                        break;
                }

                double db = Convert.ToDouble(obj);
                if (db > 0)
                {
                    return db.ToString(str);
                }
                else if (db == 0)
                {
                    return "0";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void WriteReportTitleInfo(ReportTitle rptTitle, ISheet sheet)
        {
            sheet.GetRow(2).GetCell(2).SetCellValue(rptTitle.UnitName);
            sheet.GetRow(3).GetCell(2).SetCellValue(Convert.ToDateTime(rptTitle.EndDateTime).ToString("yyyy年MM月dd日"));
        }

        public ISheet CreateCell(ISheet sheet, int rows, int cols, int row_start)
        {
            for (int i = 0; i <= rows; i++)//多添加一列用于表尾信息的填写
            {
                IRow row = null;
                if (sheet.GetRow(row_start + i) == null)
                {
                    row = sheet.CreateRow(row_start + i);
                }
                else
                {
                    row = sheet.GetRow(row_start + i);
                }
                for (int j = 0; j < cols; j++)
                {
                    if (row.GetCell(j) == null)
                    {
                        row.CreateCell(j);
                    }
                }
            }

            return sheet;
        }

        public string ResponseExcel(HSSFWorkbook hssfworkbook, string name)
        {
            string result = "";
            // 设置响应头（文件名和文件格式）
            //设置响应的类型为Excel
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //设置下载的Excel文件名
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", name + ".xls"));
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

        private void CellStyle(int rowsCount, int colsCount, int startRowIndex)
        {
            ICellStyle cellStyle = sheet.Workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center; //水平对齐居中
            cellStyle.BorderBottom = BorderStyle.Thin; //边框是黑色的
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;

            for (int i = startRowIndex; i < startRowIndex + rowsCount; i++)//对所有有数据的单元格边框进行设置成黑色边框
            {
                for (int j = 0; j < colsCount; j++)
                {
                    sheet.GetRow(i).GetCell(j).CellStyle = cellStyle;
                }
            }
        }
    }
}
