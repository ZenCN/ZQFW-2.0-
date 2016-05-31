using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    /// <summary>
    /// 单位代码和流域代码以及流域比例类,相当于一个单位代码对应一个流域代码和流域比例集合
    /// </summary>
    public class RiverInfo
    {
        private string unitcode;
        private IDictionary<string, double> riverrate;

        public RiverInfo()
        {
        }

        public string UnitCode
        {
            get { return this.unitcode; }
            set { this.unitcode = value; }
        }

        public IDictionary<string, double> DRiverRate
        {
            get
            {
                if (this.riverrate == null)
                {
                    this.riverrate = new Dictionary<string, double>();
                    return riverrate;
                }
                else
                {
                    return this.riverrate;
                }
            }
            set { this.riverrate = value; }
        }
    }
}
