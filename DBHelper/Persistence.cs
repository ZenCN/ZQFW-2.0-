using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using DBHelper;
using EntityModel;

namespace DBHelper
{
    /// <summary>
    /// 持久化静态类，只负责持久化数据或取出持久化数据。
    /// </summary>
    public class Persistence
    {
        HttpApplicationState App = HttpContext.Current.Application;  //全局变量大写，名字可用缩写。

        public static FXDICTEntities GetDbEntities()
        {
            return new FXDICTEntities(ConfigurationManager.ConnectionStrings["FXDICTEntities"].ConnectionString);
        }

        public static BusinessEntities GetDbEntities(int level)
        {
            switch (level)
            {
                case 0:
                    return new BusinessEntities(ConfigurationManager.ConnectionStrings["FXCLDEntities"].ConnectionString);
                case 2:
                    return new BusinessEntities(ConfigurationManager.ConnectionStrings["FXPRVEntities"].ConnectionString);
                case 3:
                    return new BusinessEntities(ConfigurationManager.ConnectionStrings["FXCTYEntities"].ConnectionString);
                case 4:
                    return new BusinessEntities(ConfigurationManager.ConnectionStrings["FXCNTEntities"].ConnectionString);
                case 5:
                    return new BusinessEntities(ConfigurationManager.ConnectionStrings["FXTWNEntities"].ConnectionString);
                default:
                    throw new Exception("GetDbEntities参数level，单位级别不正确");
            }
        }

        public void PersistenceUnits(string rootCode)
        {
            App["InitUnitsFlag"] = "false";

            using (FXDICTEntities fxdict = new FXDICTEntities())
            {
                string code = rootCode.Substring(0, 2) + "000000";
                var result = (from district in fxdict.TB07_District
                    where district.DistrictCode == code
                    select district).OrderBy(t => t.Uorder);
                var allLowerUnits = result.ToList().Select(t => new District
                {
                    UnitCode = t.DistrictCode,
                    UnitName = t.DistrictName,
                    RiverCode = t.RD_RiverCode1,
                    UnitLevel = t.DistrictClass.ToString(),
                    Del = t.Del.ToString(),
                    LowerUnits = GetAllLowerUnits(t.DistrictCode.ToString(), fxdict.TB07_District.ToList())
                }).ToList();
                App["Units-" + rootCode.Substring(0, 2)] = allLowerUnits[0];
                App["InitUnitsFlag"] = "true";
            }
        }

        private Dictionary<string, District> GetAllLowerUnits(string rootCode, IList<TB07_District> tb07s)
        {
            if (rootCode.Trim().Substring(6, 2) == "00")
            {
                using (FXDICTEntities fxdict = new FXDICTEntities())
                {
                    var result = (from district in tb07s
                        where district.pDistrictCode == rootCode
                        select district).OrderBy(t => t.Uorder);
                    Dictionary<string, District> upperUnit = new Dictionary<string, District>();
                    if (result.Any())
                    {
                        var units = result.ToList().Select(t => new District
                        {
                            UnitCode = t.DistrictCode.ToString(),
                            UnitName = t.DistrictName.ToString(),
                            RiverCode = t.RD_RiverCode1.ToString(),
                            UnitLevel = t.DistrictClass.ToString(),
                            Del = t.Del.ToString(),
                            LowerUnits = GetAllLowerUnits(t.DistrictCode.ToString(), tb07s)
                        });
                        foreach (var district in units)
                        {
                            if (!upperUnit.ContainsKey(rootCode + "-" + district.UnitCode))
                            {
                                upperUnit.Add(rootCode + "-" + district.UnitCode, district);
                            }
                        }
                    }
                    return upperUnit;
                }
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, District> GetLowerUnits(string UnitCode)
        {
            District district = null;

            var units = App["Units-" + UnitCode.Substring(0, 2)];
            while (units == null)
            {
                PersistenceUnits(UnitCode);
                units = App["Units-" + UnitCode.Substring(0, 2)];
            }
            district = (District)units;
            switch (Tools.GetLimitByCode(UnitCode))
            {
                case 2:
                    return district.LowerUnits;
                case 3:
                    return district.LowerUnits[UnitCode.Substring(0, 2) + "000000-" + UnitCode].LowerUnits;
                case 4:
                    var tmp = district.LowerUnits[UnitCode.Substring(0, 2) + "000000-" + UnitCode.Substring(0, 4) + "0000"];
                    return tmp.LowerUnits[UnitCode.Substring(0, 4) + "0000-" + UnitCode].LowerUnits;
                default:

                    var temp = district.LowerUnits[UnitCode.Substring(0, 2) + "000000-" + UnitCode.Substring(0, 4) + "0000"];
                    var temp1 = temp.LowerUnits[UnitCode.Substring(0, 4) + "0000-" + UnitCode.Substring(0, 6) + "00"];
                    return temp1.LowerUnits[UnitCode.Substring(0, 6) + "00-" + UnitCode].LowerUnits;
            }
        }
    }
}
