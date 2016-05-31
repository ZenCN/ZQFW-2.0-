using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aspose.Words;
using DBHelper;
using EntityModel;

namespace LogicProcessingClass.ReportOperate
{
    public class DocOper
    {
        private BusinessEntities business = null;


        public DocOper(int limit)
        {
            Entities getEntity = new Entities();
            business = (BusinessEntities)getEntity.GetPersistenceEntityByLevel(limit);
        }

        public string BenefitReport(int tbno)
        {
            string[] fieldNames = new string[12] { "BArea", "BDate", "DQJJ", "ZQYQ", "ZQSQ", "ZQZQ", "YJJC", "YJYQ", "YJYZ", "YJXY", "XYXJ", "ZHZJ" };
            string[] fieldValues = new string[12];
            string fileName = "";

            Benefit benefit = business.Benefit.Where(t => t.TBNO == tbno).SingleOrDefault();
            if (benefit != null)
            {
                fieldValues[0] = benefit.BArea;
                fieldValues[1] = Convert.ToDateTime(benefit.BDate).ToString("yyyy年MM月dd日");
                fieldValues[2] = benefit.DQJJ;
                fieldValues[3] = benefit.ZQYQ;
                fieldValues[4] = benefit.ZQSQ;
                fieldValues[5] = benefit.ZQZQ;
                fieldValues[6] = benefit.YJJC;
                fieldValues[7] = benefit.YJYQ;
                fieldValues[8] = benefit.YJYZ;
                fieldValues[9] = benefit.YJXY;
                fieldValues[10] = benefit.XYXJ;
                fieldValues[11] = benefit.ZHZJ;

                fileName = fieldValues[0] + "-" + fieldValues[1] + "-效益报表";
            }

            string tempPath = System.Web.HttpContext.Current.Server.MapPath("~/DocModel/Benefit.doc");
            string outputPath = System.Web.HttpContext.Current.Server.MapPath("~/ExcelFile/" + fileName + ".doc");

            try
            {
                var doc = new Aspose.Words.Document(tempPath);  //载入模板
                doc.MailMerge.Execute(fieldNames, fieldValues);  //合并模版，相当于页面的渲染
                doc.Save(outputPath);  //保存合并后的文档\
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return outputPath;
        }
    }
}
