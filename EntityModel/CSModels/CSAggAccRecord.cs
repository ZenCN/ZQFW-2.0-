using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GJFZWebService.Models.CSModels
{
    public class CSAggAccRecord
    {
        public int TBNO
        {
            get;
            set;
        }
        public string ORD_Code
        {
            get;
            set;
        }
        public decimal OperateType
        {
            get;
            set;
        }
        public int SPageNO
        {
            get;
            set;
        }
        public int PageNo
        {
            get;
            set;
        }
        public decimal DBType
        {
            get;
            set;
        }
        public string UnitCode
        {
            get;
            set;
        }
    }
}