using System;
using System.Collections;
using System.Collections.Generic;
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

        #region  存储持久化数据

        /// <summary>
        /// 持久化数据库模型实体
        /// </summary>
        public void PersistenceEntities()
        {
            string[] entityNames = new string[] { "FXPRVEntities", "FXCTYEntities", "FXCNTEntities", "FXTWNEntities" };
            Entities entity = new Entities();  //类名不能包含动词，应该是“一种类别”的意思。
            for (int i = 2; i <= 5; i++)
            {
                App[entityNames[i - 2]] = entity.GetEntityByLevel(i);
            }
            App["FXDICTEntities"] = new FXDICTEntities();
        }

        /// <summary>
        /// 持久化指定的数据库模型实体
        /// </summary>
        /// <param name="entityName">数据库模型实体名称</param>
        public void PersistenceEntities(EntitiesConnection.entityName entityName)
        {
            if (App[entityName.ToString()] == null) //清除app后加的
            {
                if (entityName.ToString() == "FXDICTEntities") //清除app后加的
                {
                    App[entityName.ToString()] = new FXDICTEntities();
                }
                else
                {
                    App[entityName.ToString()] = new Entities().GetEntityByConn(entityName);
                }
            }
        }


        public void PersistenceUnits(string rootCode)
        {
            App["InitUnitsFlag"] = "false";

            if (App["FXDICTEntities"] == null)  //清除app后加的
            {
                App["FXDICTEntities"] = new FXDICTEntities();
            }
            FXDICTEntities fxdict = (FXDICTEntities)App["FXDICTEntities"];
            string code = rootCode.Substring(0, 2) + "000000";//不管是哪个级别的单位代码，都初始化成省级单位代码。
            var result = (from district in fxdict.TB07_District
                          where district.DistrictCode == code   //获取该单位的所有下级单位
                          select district).OrderBy(t => t.Uorder);
             var allLowerUnits = result.ToList().Select(t => new District
            {
                UnitCode = t.DistrictCode,
                UnitName = t.DistrictName,
                RiverCode = t.RD_RiverCode1,
                UnitLevel = t.DistrictClass.ToString(),
                Del = t.Del.ToString(),
                LowerUnits = GetAllLowerUnits(t.DistrictCode.ToString(), fxdict.TB07_District.ToList())
            }).ToList<District>();
            App["Units-" + rootCode.Substring(0, 2)] = allLowerUnits[0];//2014-6-25修改Substring(6, 2)改为Substring(0, 2)
            App["InitUnitsFlag"] = "true";
        }

        private Dictionary<string, District> GetAllLowerUnits(string rootCode, IList<TB07_District> tb07s)
        {
            if (App["FXDICTEntities"] == null)//清除app后加的
            {
                App["FXDICTEntities"] = new FXDICTEntities();
            }

            if (rootCode.Trim().Substring(6, 2) == "00")
            {
                var result = (from district in tb07s
                              where district.pDistrictCode == rootCode
                              select district).OrderBy(t => t.Uorder);
                Dictionary<string, District> upperUnit = new Dictionary<string, District>(); ;
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
            else
            {
                return null;
            }
        }

        //public void PersistenceUnits(string rootCode)
        //{
        //    App["InitUnitsFlag"] = "false";
        //    if (App["FXDICTEntities"] == null)//清除app后加的
        //    {
        //        App["FXDICTEntities"] = new FXDICTEntities();
        //    }

        //    string code = rootCode.Substring(0, 2) + "000000";//不管是哪个级别的单位代码，都初始化成省级单位代码。
        //    var result = (from district in ((FXDICTEntities)App["FXDICTEntities"]).TB07_District
        //                  where district.DistrictCode == code   //获取该单位的所有下级单位
        //                select district).OrderBy(t=>t.Uorder);
        //    var allLowerUnits = result.ToList().Select(t => new District
        //                {
        //                    UnitCode = t.DistrictCode,
        //                    UnitName = t.DistrictName,
        //                    RiverCode = t.RD_RiverCode1,
        //                    UnitLevel = t.DistrictClass.ToString(),
        //                    LowerUnits = GetAllLowerUnits(t.DistrictCode.ToString())
        //                }).ToList<District>();
        //    App["Units-" + rootCode.Substring(6,2)] = allLowerUnits[0];
        //    App["InitUnitsFlag"] = "true";
        //}

        //private Dictionary<string, District> GetAllLowerUnits(string rootCode)
        //{
        //    if (App["FXDICTEntities"] == null)//清除app后加的
        //    {
        //        App["FXDICTEntities"] = new FXDICTEntities();
        //    }
        //    if (rootCode.Trim().Substring(6, 2) == "00")
        //    {
        //        var result = (from district in ((FXDICTEntities)App["FXDICTEntities"]).TB07_District
        //                     where district.pDistrictCode == rootCode
        //                     select district).OrderBy(t=>t.Uorder);
        //        var units = result.ToList().Select(t => new District
        //                {
        //                    UnitCode = t.DistrictCode.ToString(),
        //                    UnitName = t.DistrictName.ToString(),
        //                    RiverCode = t.RD_RiverCode1.ToString(),
        //                    UnitLevel = t.DistrictClass.ToString(),
        //                    LowerUnits = GetAllLowerUnits(t.DistrictCode.ToString())
        //                });
        //        Dictionary<string, District> upperUnit = new Dictionary<string, District>();
        //        foreach (var district in units)
        //        {
        //            if (!upperUnit.ContainsKey(rootCode + "-" + district.UnitCode))
        //            {
        //                upperUnit.Add(rootCode + "-" + district.UnitCode, district);
        //            }
        //        }
                
        //        return upperUnit;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        #endregion


        #region  取出（全部/部分）持久化数据

        /// <summary>
        /// 取出指定的数据库模型实体
        /// </summary>
        /// <param name="entityName">数据库模型实体名字</param>
        /// <returns>数据库模型实体</returns>
        public object GetPersistenceEntity(EntitiesConnection.entityName entityName) 
        {
            object entity = null;
            if( entityName.ToString() !="")
            {
                entity = App[entityName.ToString()];
                while (entity == null)
                {
                    PersistenceEntities(entityName);
                    entity = App[entityName.ToString()];
                }
            }
            return entity;
        }

        public Dictionary<string, District> GetLowerUnits(string UnitCode)
        {
            District district = null;
            //var units = App["Units-" + UnitCode.Substring(6,2)];  //获取该省份的所有下级单位,UnitLevel为“2”
            var units = App["Units-" + UnitCode.Substring(0, 2)];//2014-6-25修改Substring(6, 2)改为Substring(0, 2)
            while (units == null)
            {
                PersistenceUnits(UnitCode);
                //units = App["Units-" + UnitCode.Substring(6, 2)];
                units = App["Units-" + UnitCode.Substring(0, 2)];//2014-6-25修改Substring(6, 2)改为Substring(0, 2)同上
            }
            district = (District)units;
            switch (Tools.GetLimitByCode(UnitCode))
            {
                case 2:
                    return district.LowerUnits;
                case 3:
                    return district.LowerUnits[UnitCode.Substring(0, 2) + "000000-" + UnitCode].LowerUnits;
                case 4:
                    var tmp = district.LowerUnits[UnitCode.Substring(0, 2) + "000000-" + UnitCode.Substring(0, 4) + "0000"];  //获取市级单位
                    return tmp.LowerUnits[UnitCode.Substring(0, 4) + "0000-" + UnitCode].LowerUnits;
                default:

                    var temp = district.LowerUnits[UnitCode.Substring(0, 2) + "000000-" + UnitCode.Substring(0, 4) + "0000"];  //获取市级单位
                    var temp1 = temp.LowerUnits[UnitCode.Substring(0, 4) + "0000-" + UnitCode.Substring(0, 6) + "00"];
                    return temp1.LowerUnits[UnitCode.Substring(0, 6) + "00-" + UnitCode].LowerUnits;
            }
        }

        //public Dictionary<string, District> GetLowerUnits(string UnitCode)
        //{
        //    District district = null;
        //    var units = App["Units-" + UnitCode.Substring(6,2)];  //获取该省份的所有下级单位,UnitLevel为“2”
        //    while (units == null)
        //    {
        //        PersistenceUnits(UnitCode);
        //        units = App["Units-" + UnitCode.Substring(6, 2)];
        //    }
        //    district = (District)units;
        //    switch (Tools.GetLimitByCode(UnitCode))
        //    { 
        //        case 2:
        //            return district.LowerUnits;
        //        case 3:
        //            return district.LowerUnits[UnitCode.Substring(0, 2) + "000000-" + UnitCode].LowerUnits;
        //        case 4:
        //            var tmp = district.LowerUnits[UnitCode.Substring(0, 2) + "000000-" + UnitCode.Substring(0, 4) + "0000"];  //获取市级单位
        //            return tmp.LowerUnits[UnitCode.Substring(0, 4) + "0000-" + UnitCode].LowerUnits;
        //        default:
        //            return null;
        //    }
        //}

        #endregion
    }
}
