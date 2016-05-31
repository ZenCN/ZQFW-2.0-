using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel
{
    /// <summary>
    /// 行政代码和流域代码以及流域操作表代码关系类,相当于一个单位对码对应一个流域代码和操作表代码集合
    /// </summary>
    public class RiverRPTypeInfo
    {
        private string unitcode;
        private IDictionary<string, string> riverrtype;

        public RiverRPTypeInfo()
        {
        }

        /// <summary>
        /// 单位代码
        /// </summary>
        public string Unitcode
        {
            get { return this.unitcode; }
            set { this.unitcode = value; }
        }

        /// <summary>
        /// KEY为流域代码，VALUE为操作表代码(XZ0等)
        /// </summary>
        public IDictionary<string, string> DRiverRPType
        {
            get
            {
                if (this.riverrtype == null)
                {
                    this.riverrtype = new Dictionary<string, string>();
                    return this.riverrtype;
                }
                else
                {
                    return this.riverrtype;
                }
            }
            set { this.riverrtype = value; }
        }
    }
}
