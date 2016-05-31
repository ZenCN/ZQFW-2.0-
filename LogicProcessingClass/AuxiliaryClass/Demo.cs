using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using LogicProcessingClass.Model;

namespace LogicProcessingClass.AuxiliaryClass
{
    public class Demo
    {
        public static FXDICTEntities fxdict = new FXDICTEntities();
        public static UserInfo GetUserInfo(string unitCode)
        {
            UserInfo outPutUserInfo = new UserInfo();
            var localUnit = fxdict.TB07_District.SingleOrDefault(t => t.DistrictCode == unitCode);
            var underUnits = fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode).OrderBy(t=>t.Uorder);
            if (localUnit!=null)
            {
                outPutUserInfo.LocalUnit = new Unit
                {
                    UnitCode = localUnit.DistrictCode,
                    UnitName = localUnit.DistrictName
                };
                outPutUserInfo.UnderUnits = underUnits.Select(t => new Unit
                {
                    UnitCode = t.DistrictCode,
                    UnitName = t.DistrictName
                }).ToList();
            }
            return outPutUserInfo;
        }
    }
}
