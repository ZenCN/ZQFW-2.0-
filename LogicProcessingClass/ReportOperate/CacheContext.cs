using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;

namespace LogicProcessingClass.ReportOperate
{
    /// <summary>
    /// 流域分配类
    /// </summary>
    public class CacheContext
    {
        public static IDictionary<string, RiverRPTypeInfo> RiverRPTypeInfo
        {
            get
            {
                IDictionary<string, RiverRPTypeInfo> oo = CacheHelper.GetCache("RiverRPTypeInfo") as IDictionary<string, RiverRPTypeInfo>;
                if (oo == null)
                {
                    oo = new Dictionary<string, RiverRPTypeInfo>();
                    CacheHelper.SetCache("RiverRPTypeInfo", oo);
                    return oo;
                }
                else
                {
                    return oo;
                }

            }
        }

        public static IDictionary<string, RiverInfo> RiverInfoList
        {
            get
            {
                IDictionary<string, RiverInfo> oo = CacheHelper.GetCache("RiverInfoList") as IDictionary<string, RiverInfo>;
                if (oo == null)
                {
                    oo = new Dictionary<string, RiverInfo>();
                    CacheHelper.SetCache("RiverInfoList", oo);
                    return oo;
                }
                else
                {
                    return oo;
                }

            }
        }
    }
}
