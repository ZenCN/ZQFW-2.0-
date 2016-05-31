using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.ReportAuxiliaryModel
{
    public class Fields
    {
        public string FieldCode { get; set; }
        public FieldData FieldData { get; set; }
    }
    public class FieldData
    {
        public decimal MeasureValue { get; set; }
        public string MeasureName { get; set; }
        public string InputRemark { get; set; }
    }
}
