using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    /// <summary>
    /// 流域比例类
    /// </summary>
    public class RiverDataInfo
    {
        private string Code;
        private double Data;
        public RiverDataInfo()
        {
        }
        //流域代码
        public string RiverCode
        {
            get { return this.Code; }
            set { this.Code = value; }
        }
        //流域比例（小于1，小数）
        public double RiverData
        {
            get { return this.Data; }
            set { this.Data = value; }
        }
    }
}
