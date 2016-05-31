using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using EntityModel;

namespace LogicProcessingClass.XMMZH
{
    public class XMMSetZHClass
    {
        public HL011 XMMSetHl011(HL011 hl011)
        {
            PropertyInfo[] pfs = hl011.GetType().GetProperties();//利用反射获得类的属性

            string[] arr = { };
            string temp = "";
            decimal shuliangji = 0;
            double xiaoshu = 0;
            decimal changetemp = 0;
            for (int i = 0; i < pfs.Length; i++)
            {
                if (arr != null)
                {
                    shuliangji = Convert.ToDecimal(arr[0]);
                    xiaoshu = Convert.ToDouble(arr[1]);
                }

                if (pfs[i].PropertyType.FullName == "System.String")
                {
                    //if (pfs[i].GetValue(hl011, null) == null)
                    //{
                    //    temp = "";
                    //}
                    //else
                    //{
                    //    temp = pfs[i].GetValue(hl011, null).ToString();
                    //}
                    //pfs[i].SetValue(hl011, temp, null);
                }
                else if (pfs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1)
                //else if (pfs[i].PropertyType.FullName == "System.Decimal")
                {
                    temp = pfs[i].GetValue(hl011, null).ToString();
                    changetemp = Convert.ToDecimal(temp);
                    if (shuliangji == 0)
                    {

                    }
                    else
                    {
                        changetemp = changetemp * shuliangji;
                    }

                    pfs[i].SetValue(hl011, changetemp, null);
                }
                else if (pfs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                //else if (pfs[i].PropertyType.FullName == "System.Int32")
                {
                    //temp = pfs[i].GetValue(hl011, null).ToString();
                    //pfs[i].SetValue(hl011, temp, null);
                }
                else if (pfs[i].PropertyType.FullName == "System.DateTime")
                {
                    //temp = Convert.ToDateTime(pfs[i].GetValue(hl011, null)).ToString("yyyy-MM-dd");
                    //pfs[i].SetValue(hl011, temp, null);
                }
                else
                {

                }
            }
            return hl011;
        }
    }
}
