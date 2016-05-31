using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using EntityModel;
using System.Web;
/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：
    // 文件名：Tools.cs
    // 文件功能描述：提供一些辅助方法
    // 创建标识：
    // 修改标识：
    // 修改描述：
//-------------------------------------------------------------*/

namespace LogicProcessingClass
{
    public class Tools
    {
        private FXDICTEntities fxdict = new FXDICTEntities();



        /// <summary>
        /// 加密（解密）字符串
        /// </summary>
        /// <param name="commandCode">命令代码：0表示加密；1表示解密</param>
        /// <param name="enOrDeStr">要加密（解密）的字符串</param>
        /// <param name="key">要加密（解密）的密钥，要求为8位</param>
        /// <returns>解密（加密）成功返回解密（加密）后的字符串，失败返源串</returns>
        public string EncryptOrDecrypt(int commandCode, string enOrDeStr, string key)
        {
            //默认密钥向量
            byte[] keys = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};

            try
            {
                if (commandCode == 0) //加密
                {
                    byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8)); //转换为字节

                    byte[] rgbIV = keys;
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(enOrDeStr);
                    DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider(); //实例化数据加密标准
                    MemoryStream mStream = new MemoryStream(); //实例化内存流

                    //将数据流链接到加密转换的流
                    CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV),
                        CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cStream.FlushFinalBlock();

                    return Convert.ToBase64String(mStream.ToArray());
                }
                else if (commandCode == 1)
                {
                    byte[] rgbKey = Encoding.UTF8.GetBytes(key);
                    byte[] rgbIV = keys;
                    byte[] inputByteArray = Convert.FromBase64String(enOrDeStr.Replace(" ", "+"));
                    DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                    MemoryStream mStream = new MemoryStream();
                    CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV),
                        CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cStream.FlushFinalBlock();

                    return Encoding.UTF8.GetString(mStream.ToArray());
                }
                else
                {
                    return enOrDeStr;
                }
            }
            catch (Exception)
            {
                return enOrDeStr;
            }
        }

        /// <summary>
        /// 根据单位代码获得使用单位编号
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <returns></returns>
        public string GetUnitNameByUnitCode(string unitCode)
        {
            string UnitName = "";
            switch (unitCode.Substring(0, 2))
            {
                case "11":
                    UnitName = "BJ";
                    break;
                case "36":
                    UnitName = "JX";
                    break;
                case "43":
                    UnitName = "HN";
                    break;
            }
            return UnitName;
        }

        /// <summary>根据单位代码获取该单位的中文名称
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <returns></returns>
        public string GetNameByUnitCode(string unitCode)
        {
            string unitName = "";
            var units = fxdict.TB07_District.Where(t => t.DistrictCode == unitCode).ToList();
            if (units.Count > 0)
            {
                unitName = units.First().DistrictName;
            }
            return unitName;
        }

        /// <summary>
        /// 根据上级代码得到下级代码的区间
        /// </summary>
        /// <param name="unitcode">上级代码</param>
        /// <returns>返回下级代码的区间及上级代码</returns>
        public int[] GetLowerUnitCodeZone(string unitcode)
        {
            int[] result = new int[3];
            if (unitcode.Substring(2) == "000000") //省:
            {
                result[0] = int.Parse(unitcode.Substring(0, 2))*1000000;
                result[1] = (int.Parse(unitcode.Substring(0, 2))*100 + 99)*10000;
                result[2] = 0;
                return result;
            }
            else
            {
                if (unitcode.Substring(4) == "0000") //市:
                {
                    result[0] = int.Parse(unitcode.Substring(0, 4))*10000;
                    result[1] = (int.Parse(unitcode.Substring(0, 4))*100 + 99)*100;
                    result[2] = 1;
                    return result;
                }
                else
                {
                    if (unitcode.Substring(6) == "00") //县
                    {
                        result[0] = int.Parse(unitcode.Substring(0, 6))*100;
                        result[1] = result[0] + 99;
                        result[2] = 2;
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 根据单位编号从字典库中获取该单位级别
        /// </summary>
        /// <param name="unitcode">单位编号</param>
        /// <returns>该单位级别</returns>
        public int GetLevelByUnitCode(string unitcode)
        {
            int level = GetLevelByUnitCode(unitcode,unitcode);

            /*var tb07 = from district in fxdict.TB07_District
                where district.DistrictCode == unitcode
                select district;

            if (tb07.Count() >= 1)
            {
                string[] ss = tb07.First().DistrictClass.ToString().Split('.');
                level = int.Parse(ss[0].ToString());
            }*/
            return level;
        }

        /// <summary>
        /// 根据单位编号计算出该单位的级别（不是从数据库，unitname任意，紧紧是为了重载）
        /// </summary>
        /// <param name="unitcode">单位代码</param>
        /// <param name="unitname">单位名（任意值）</param>
        /// <returns></returns>
        public int GetLevelByUnitCode(string unitcode, string unitname)
        {
            int level = -1;
            //if (unitcode.Substring(0, 2).ToString() == "11")//北京的级别规则才是这个   张建军 江西也有催报功能

            //{
            if (unitcode.Substring(2).ToString() == "000000")
            {
                level = 2;
            }
            else if (unitcode.Substring(4).ToString() == "0000")
            {
                level = 3;
            }
            else if (unitcode.Substring(6).ToString() == "00")
            {
                level = 4;
            }
            else
            {
                level = 5;
            }
            //}
            //if (unitcode.IndexOf("000000") >= 0)
            //    level = 2;
            //else if (unitcode.IndexOf("0000") >= 0)
            //    level = 3;
            //else if (unitcode.IndexOf("00") >= 0)
            //    level = 4;
            //else
            //    level = 5;
            return level;
        }

        public List<Dictionary<string, string>> GetUnderUnitInfoByCode(string UnitCode)
        {
            List<Dictionary<string, string>> unitList = new List<Dictionary<string, string>>();

            var tb07 = from district in fxdict.TB07_District
                where district.pDistrictCode == UnitCode
                orderby district.Uorder
                select district;
            foreach (var obj in tb07)
            {
                Dictionary<string, string> pUnitList = new Dictionary<string, string>();
                pUnitList.Add("DistrictCode", obj.DistrictCode.ToString());
                pUnitList.Add("DistrictName", obj.DistrictName.ToString());
                pUnitList.Add("DistrictClass", Convert.ToInt32(obj.DistrictClass).ToString());
                unitList.Add(pUnitList);
            }
            if (tb07.Count() == 0)
            {
                unitList = null;
            }
            return unitList;
        }

        /// <summary>
        /// 清除当前application所有对象
        /// </summary>
        public void ClearApplication()
        {
            HttpApplicationState App = HttpContext.Current.Application;
            App.Clear();
        }

        /// <summary>
        /// 通过对象名清除当前application指定对象
        /// </summary>
        /// <param name="key">对象名</param>
        public void ClearApplicationByKey(string key)
        {
            HttpApplicationState App = HttpContext.Current.Application;
            if (App.AllKeys.Contains(key))
            {
                App.Remove(key);
            }
        }

        public string ClearApplicationKey(string key)
        {
            string result = "";
            HttpApplicationState App = HttpContext.Current.Application;
            if (App.AllKeys.Contains(key))
            {
                App.Remove(key);
                result = "清除" + key + "的缓存成功！";
            }
            else
            {
                result = "不存在该缓存";
            }
            return result;
        }

        /// <summary>把新建类的各数字类型属性的值设置为0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T SetZeroToObject<T>(T obj)
        {
            PropertyInfo[] pfs = obj.GetType().GetProperties(); //利用反射获得类的属性
            for (int i = 0; i < pfs.Length; i++)
            {

                if (pfs[i].PropertyType.FullName.IndexOf("System.Decimal") != -1) // == "System.Decimal"
                {
                    pfs[i].SetValue(obj,Convert.ToDecimal(0), null);
                }
                else if (pfs[i].PropertyType.FullName.IndexOf("System.Int32") != -1)
                {
                    pfs[i].SetValue(obj, 0, null);
                }
            }
            return obj;
        }
    }
}
