using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBHelper;
using EntityModel;
using LogicProcessingClass.Model;

namespace LogicProcessingClass.ReportOperate
{
    public class HP01SKR
    {
        FXDICTEntities fxdict = null;
        /// <summary>获取当前登录单位的下级单位死库容
        /// </summary>
        /// <param name="unitCode">当前登录单位</param>
        /// <returns></returns>
        public string GetUnitSKR(string unitCode,int limit)
        {
            string str = "";
            fxdict = Persistence.GetDbEntities();
            //var units =
            //    fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode).Select(t=>t.DistrictCode).ToList();
            var units =
              fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode).ToList();
            string unitStr = "";
            foreach (var unit in units)
            {
                foreach (var tb51 in unit.TB51_HunanDistrictConst)
                {
                    if (limit != 2)
                    {
                        str += unit.DistrictCode + ":{'DZKSKR':'" + ZeroToEmpty(Convert.ToDouble(tb51.DZKSKR),10000) +
                               "','ZXKSKR':'" + ZeroToEmpty(Convert.ToDouble(tb51.ZXKSKR),10000) + "','XYKSKR':'" +
                               ZeroToEmpty(Convert.ToDouble(tb51.XYKSKR), 10000) + "','XRKSKR':'" +
                               ZeroToEmpty(Convert.ToDouble(tb51.XRKSKR), 10000) + "','SPTSKR':'" +
                               ZeroToEmpty(Convert.ToDouble(tb51.SPTSKR), 10000) + "'},";
                    }
                    
                    else
                    {
                        str += unit.DistrictCode + ":{'DZKSKR':'" + ZeroToEmpty(Convert.ToDouble(tb51.DZKSKR), 1) +
                               "','ZXKSKR':'" + ZeroToEmpty(Convert.ToDouble(tb51.ZXKSKR), 1) + "','XYKSKR':'" +
                               ZeroToEmpty(Convert.ToDouble(tb51.XYKSKR), 1) + "','XRKSKR':'" +
                               ZeroToEmpty(Convert.ToDouble(tb51.XRKSKR), 1) + "','SPTSKR':'" +
                               ZeroToEmpty(Convert.ToDouble(tb51.SPTSKR), 1) + "'},";
                    }
                }
            }

            var underUnits = fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode).Select(t => t.DistrictCode).ToList();
            var tb44s = (from tb44 in fxdict.TB44_ReservoirDistrict
                         where underUnits.Contains(tb44.UnitCode)
                         select tb44).ToList();

            foreach (var tb44 in tb44s)
            {
                if (limit != 2)
                {
                    str += "'" + tb44.TB43_Reservoir.RSCode + "':'" + ZeroToEmpty(Convert.ToDouble(tb44.TB43_Reservoir.SKR),10000) + "',";
                }

                else
                {
                    str += "'" + tb44.TB43_Reservoir.RSCode + "':'" + ZeroToEmpty(Convert.ToDouble(tb44.TB43_Reservoir.SKR), 1) + "',";
                }
            }
            if (str != "")
            {
                str = str.Remove(str.Length - 1);
            }
            return str;
        }

        /// <summary>获取当前单位的所有水库的死库容
        /// </summary>
        /// <param name="unitCode">当前登录单位</param>
        /// <returns></returns>
        public string GetReservoirSKR(string unitCode,int limit)
        {
            fxdict = Persistence.GetDbEntities();
            var underUnits = fxdict.TB07_District.Where(t => t.pDistrictCode == unitCode).Select(t => t.DistrictCode).ToList();
            var tb44s = (from tb44 in fxdict.TB44_ReservoirDistrict
                         where underUnits.Contains(tb44.UnitCode)
                         select tb44).ToList();

            string str = "";
            foreach (var tb44 in tb44s)
            {
                if (limit != 2)
                {
                    str += "'" + tb44.TB43_Reservoir.RSCode + "':'" + ZeroToEmpty(Convert.ToDouble(tb44.TB43_Reservoir.SKR), 10000) + "',";
                }

                else
                {
                    str += "'" + tb44.TB43_Reservoir.RSCode + "':'" + ZeroToEmpty(Convert.ToDouble(tb44.TB43_Reservoir.SKR), 1) + "',";
                }
            }
            if (str != "")
            {
                str = str.Remove(str.Length - 1);
            }

            return str;
        }

        private string ZeroToEmpty(double db, int slj)
        {
            if (db > 0)
            {
                return (db * slj).ToString("0.00");
            }
            else
            {
                return "";
            }
        }
    }
}
