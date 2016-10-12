using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityModel;
using LogicProcessingClass.AuxiliaryClass;
using DBHelper;
/*----------------------------------------------------------------
// 版本说明：
// 版本号：
// 文件名：BaseData.cs
// 文件功能描述：基础数据模块操作
// 创建标识：胡汗 2013年12月6日
// 修改标识：


// 修改描述：
//-------------------------------------------------------------*/
using LogicProcessingClass.Model;
using NPOI.SS.Formula.Functions;

namespace LogicProcessingClass.ReportOperate
{
    public class BaseData
    {
        Tools tool = new Tools();
        //private FXDICTEntities dicEntity = Persistence.GetDbEntities();
        string jsonStr = "[";
        IList[] iList = new IList[4];//存放4个单位级别的数据
        IList<TB07_District>[] tb07List = new IList<TB07_District>[4];
        /// <summary>
        /// 保存单位基础数据
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="fieldDefineNo">字段定义编号</param>
        /// <param name="baseData">基础数据</param>
        public void SaveBaseData(string unitCode, List<TB04_CheckBase> tb04List)
        {
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            for (int i = 0; i < tb04List.Count; i++)
            {
                if (tb04List[i].BaseData == null || tb04List[i].BaseData == 0)
                {
                    int tbno = tb04List[i].TBNO;
                    var delTb04s = dicEntity.TB04_CheckBase.Where(t => t.TBNO == tbno).ToList();
                    if (delTb04s.Count > 0)
                    {
                        dicEntity.TB04_CheckBase.DeleteObject(delTb04s[0]);
                    }

                }
                else
                {
                    TB04_CheckBase tb04 = new TB04_CheckBase();
                    tb04.District_Code = unitCode;
                    tb04.FieldDefine_NO = tb04List[i].FieldDefine_NO;
                    tb04.BaseData = tb04List[i].BaseData;
                    switch (Convert.ToInt32(tb04.FieldDefine_NO))
                    {
                        case 25: //人口万人
                            tb04.BaseData = tb04.BaseData * 10000;
                            break;
                        case 32: //面积千公顷
                            tb04.BaseData = tb04.BaseData * 10000000;
                            break;
                        case 57: //堤防总计（千米）
                            tb04.BaseData = tb04.BaseData * 1000;
                            break;
                        default:
                            tb04.BaseData = tb04.BaseData;
                            break;
                    }

                    var bd = dicEntity.TB04_CheckBase.Where(t => t.FieldDefine_NO == tb04.FieldDefine_NO &&
                                                                 t.District_Code == unitCode).ToList();
                    if (bd.Count == 0) //这项没有，在数据库中添加
                    {
                        dicEntity.TB04_CheckBase.AddObject(tb04);
                    }
                    else //这项有，修改数据
                    {
                        bd[0].BaseData = tb04.BaseData;
                    }
                }
            }
            dicEntity.SaveChanges();

        }

        public int GetLimitByUnitCode(string unitcode)
        {
            int level = -1;

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

            return level;
        }

        /// <summary>
        /// 增加单位
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="pUnitCode">上级单位代码</param>
        /// <param name="unitName">单位名称</param>
        /// <param name="uorder">单位顺序</param>
        /// <param name="riverDictCode">流域代码</param>
        /// <returns>成功返回“success”；失败返回失败原因</returns>
        public string AddUnit(string unitCode, string pUnitCode, string unitName, int uorder, string riverDictCode)
        {
            string str = "success";
            BusinessEntities busEntity = Persistence.GetDbEntities(GetLimitByUnitCode(unitCode));
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var oneUnit = dicEntity.TB07_District.Where(d => d.DistrictCode == unitCode).ToList();
            var pUnit = dicEntity.TB07_District.Where(pd => pd.DistrictCode == pUnitCode).First();
            if (!IsInSubRiver(riverDictCode, pUnit.RD_RiverCode1))//该单位的流域不在上级单位的流域范围之内
            {
                return "该单位的流域不在上级单位的流域范围之内！";
            }
            //该单位代码可用，上级单位是镇级以上且上级单位存在
            if (oneUnit.Count == 0 && tool.GetLevelByUnitCode(pUnitCode) < 5)
            {
                #region 在单位表中添加数据
                TB07_District tb07 = dicEntity.TB07_District.CreateObject();
                tb07.DistrictCode = unitCode;
                tb07.pDistrictCode = pUnitCode;
                tb07.DistrictName = unitName;
                tb07.DistrictClass = tool.GetLevelByUnitCode(unitCode);
                tb07.Uorder = uorder;
                tb07.RD_RiverCode1 = riverDictCode;
                dicEntity.AddToTB07_District(tb07);
                #endregion
                #region 在登陆表中添加数据

                LGN lgn = null;
                var lgns = busEntity.LGN.Where(t => t.LoginName == unitCode);
                if (lgns.Any())
                {
                    lgn = lgns.SingleOrDefault();
                    lgn.LoginName = unitCode;
                    lgn.PWD = "sa";
                    lgn.RealName = unitName;
                    lgn.UserName = "未知";
                    lgn.Authority = 1;
                    lgn.OperateTable = "HL,HP";
                }
                else
                {
                    lgn = new LGN();
                    lgn.LoginName = unitCode;
                    lgn.PWD = "sa";
                    lgn.RealName = unitName;
                    lgn.UserName = "未知";
                    lgn.Authority = 1;
                    lgn.OperateTable = "HL,HP";
                    busEntity.AddToLGN(lgn);
                }
                #endregion
            }
            if (oneUnit.Any())
            {
                oneUnit[0].pDistrictCode = pUnitCode;
                oneUnit[0].DistrictName = unitName;
                oneUnit[0].DistrictClass = tool.GetLevelByUnitCode(unitCode);
                oneUnit[0].Uorder = uorder;
                oneUnit[0].RD_RiverCode1 = riverDictCode;
                var lgns = busEntity.LGN.Where(t => t.LoginName == unitCode);
                if (lgns.Any())
                {
                    lgns.SingleOrDefault().RealName = unitName;
                    lgns.SingleOrDefault().PWD = "sa";
                }
                else
                {
                    LGN lgn = new LGN();
                    lgn.LoginName = unitCode;
                    lgn.PWD = "sa";
                    lgn.RealName = unitName;
                    lgn.UserName = "未知";
                    lgn.Authority = 1;
                    lgn.OperateTable = "HL,HP";
                    busEntity.AddToLGN(lgn);
                }
            }
            try
            {
                try
                {
                    dicEntity.SaveChanges();
                    busEntity.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (Exception ex)
            {
                str = "错误消息：" + ex.InnerException + ex.Message;
            }

            return str;
        }

        /// <summary>
        /// 增加一个基础数据
        /// </summary>
        /// <param name="fxdictEntity">字典库实体</param>
        /// <param name="unitCode">单位代码</param>
        /// <param name="fieldDefineNo">字段定义编号</param>
        /// <param name="baseData">基础数据</param>
        private void AddOneBaseData(FXDICTEntities fxdictEntity, string unitCode, int fieldDefineNo, decimal baseData)
        {
            TB04_CheckBase tb04 = new TB04_CheckBase();
            tb04.District_Code = unitCode;
            tb04.BaseData = baseData;
            tb04.FieldDefine_NO = fieldDefineNo;
            tb04.RiverDict_Code = new CommonFunction().GetRiverCodeByUnitCode(unitCode);
            fxdictEntity.AddToTB04_CheckBase(tb04);
        }

        /// <summary>
        /// 重置密码(建议2.0的密码都搞成sa，以免容易混淆，北京暂未修改为zizo13）
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        public void ResetPassword(string unitCode, int limit)
        {
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            var login = busEntity.LGN.Where(lgn => lgn.LoginName == unitCode);
            if (login.Count() > 0)
            {
                login.FirstOrDefault().PWD = "sa";
            }
            else
            {
                LGN lgn = new LGN();
                lgn.LoginName = unitCode;
                lgn.PWD = "sa";
                lgn.RealName = "未知";
                lgn.RoleName = "ss";
                lgn.UserName = "张三";
                lgn.Authority = 1;
                lgn.OperateTable = "HL,HP";
                busEntity.AddToLGN(lgn);
            }
            busEntity.SaveChanges();
        }

        public void ResetPassword(string[] unitCodes, int limit)
        {
            BusinessEntities busEntity = Persistence.GetDbEntities(limit);
            foreach (string unitCode in unitCodes)
            {
                var login = busEntity.LGN.Where(lgn => lgn.LoginName == unitCode);
                if (login.Count() > 0)
                {
                    login.FirstOrDefault().PWD = "sa";
                }
                else
                {
                    LGN lgn = new LGN();
                    lgn.LoginName = unitCode;
                    lgn.PWD = "sa";
                    lgn.RealName = "未知";
                    lgn.RoleName = "ss";
                    lgn.UserName = "张三";
                    lgn.Authority = 1;
                    lgn.OperateTable = "HL,HP";
                    busEntity.AddToLGN(lgn);
                }
            }
            busEntity.SaveChanges();
        }

        /// <summary>
        /// 修改单位
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="pUnitCode">上级单位代码</param>
        /// <param name="unitName">单位名称</param>
        /// <param name="uorder">单位顺序</param>
        /// <param name="riverDictCode">流域代码</param>
        /// <returns>成功返回“success”；失败返回失败原因</returns>
        public string UpdateUnit(string unitCode, string pUnitCode, string unitName, int uorder, string riverDictCode)
        {
            string str = "success";
            BusinessEntities busEntity = Persistence.GetDbEntities(2);
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var oneUnit = dicEntity.TB07_District.Where(tb07 => tb07.DistrictCode == unitCode).First();
            var pUnit = dicEntity.TB07_District.Where(tb07 => tb07.DistrictCode == pUnitCode).First();
            var checkBases = dicEntity.TB04_CheckBase.Where(tb04 => tb04.District_Code == unitCode);
            var lgns = busEntity.LGN.Where(lgn => lgn.LoginName == unitCode);
            if (!IsInSubRiver(riverDictCode, pUnit.RD_RiverCode1))
            {
                return "该单位的流域不在上级单位的流域范围之内！";
            }
            //上级单位是镇级以上且上级单位存在
            if (tool.GetLevelByUnitCode(pUnitCode) < 5)
            {
                oneUnit.pDistrictCode = pUnitCode;
                oneUnit.DistrictName = unitName;
                oneUnit.DistrictClass = tool.GetLevelByUnitCode(unitCode);
                oneUnit.Uorder = uorder;
                oneUnit.RD_RiverCode1 = riverDictCode;
                foreach (var checkBase in checkBases)
                {
                    checkBase.RiverDict_Code = riverDictCode;//修改单位基础数据表
                }
                foreach (var lgn in lgns)
                {
                    lgn.RealName = unitName;//修改单位登录表
                }
                dicEntity.SaveChanges();
                busEntity.SaveChanges();
                //tool.ClearApplication();
            }
            else
            {
                str = "上级单位是乡镇级或上级单位不存在";
            }
            return str;
        }

        /// <summary>
        /// 删除单位
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        public string DeleteUnit(string unitCode)
        {
            string temp = "";
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var lowerUnits = dicEntity.TB07_District.Where(tb07 => tb07.pDistrictCode == unitCode);//获得下级单位
            if (!lowerUnits.Any())//没有下级单位
            {
                BusinessEntities busEntity = Persistence.GetDbEntities(tool.GetLevelByUnitCode(unitCode));
                dicEntity.TB07_District.DeleteObject(dicEntity.TB07_District.Where(tb07 => tb07.DistrictCode == unitCode).SingleOrDefault());//删除单位表信息
                var lgsns = busEntity.LGN.Where(lgn => lgn.LoginName == unitCode);
                if (lgsns.Count() > 0)
                {
                    busEntity.LGN.DeleteObject(lgsns.FirstOrDefault());//删除登陆表信息
                }
                var baseDatas = dicEntity.TB04_CheckBase.Where(tb04 => tb04.District_Code == unitCode);
                foreach (var baseData in baseDatas)
                {
                    dicEntity.TB04_CheckBase.DeleteObject(baseData);//删除单位相关基本数据
                }
                dicEntity.SaveChanges();
                busEntity.SaveChanges();
                temp = "1";
            }
            else
            {
                temp = "该单位有下级单位无法删除！";
            }
            return temp;
        }

        /// <summary>
        /// 获取单位的基础数据
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <returns></returns>
        public string GetBaseDataByUnitCode(string unitCode)
        {
            string str = "";
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var baseDates = dicEntity.TB04_CheckBase.Where(tb04 => tb04.District_Code == unitCode);
            foreach (var baseData in baseDates)
            {
                double returnValus = 0;
                switch (Convert.ToInt32(baseData.FieldDefine_NO))
                {
                    case 25://人口万人
                        returnValus = Convert.ToDouble(baseData.BaseData) / 10000;
                        break;
                    case 32://面积千公顷

                        returnValus = Convert.ToDouble(baseData.BaseData) / 10000000;
                        break;
                    case 57://堤防总计（千米）
                        returnValus = Convert.ToDouble(baseData.BaseData) / 1000;
                        break;
                    default:
                        returnValus = Convert.ToDouble(baseData.BaseData);
                        break;
                }
                str += "{TBNO:'" + baseData.TBNO + "',FieldDefine_NO:'" + baseData.FieldDefine_NO + "',BaseData:'" +
                    returnValus + "'},";
            }

            dicEntity.Dispose();

            if (str.Length != 0)
            {
                str = "{DataList:[" + str.Remove(str.Length - 1) + "]}";
            }
            else
            {
                str = "{DataList:[]}";
            }

            return str;
        }

        /// <summary>
        /// 浏览单位
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <returns></returns>
        public string GetUnits(string unitCode)
        {
            string str = "";
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var lowerUnits = dicEntity.TB07_District.Where(tb07 => tb07.pDistrictCode == unitCode).OrderBy(t => t.Uorder);//查找下级单位
            if (!lowerUnits.Any())//没有下级单位
            {
                var unit = dicEntity.TB07_District.Where(tb07 => tb07.DistrictCode == unitCode).First();//获得单位本身
                str = "{lowerunits:[{Code:'" + unit.DistrictCode + "',ParentCode:'" + unit.pDistrictCode +
                    "',Name:'" + unit.DistrictName + "',RiverCode:'" + unit.RD_RiverCode1 + "',Order:'" + Convert.ToInt32(unit.Uorder) + "'}]}";
            }
            else//有下级单位
            {
                foreach (var lowerUnit in lowerUnits)
                {
                    str += "{Code:'" + lowerUnit.DistrictCode + "',ParentCode:'" + lowerUnit.pDistrictCode + "',Name:'" +
                        lowerUnit.DistrictName + "',RiverCode:'" + lowerUnit.RD_RiverCode1 + "',Order:'" + Convert.ToInt32(lowerUnit.Uorder) + "'},";
                }
                if (str.Length != 0)
                {
                    str = "{lowerunits:[" + str.Remove(str.Length - 1) + "]}";
                }
            }
            dicEntity.Dispose();

            return str;
        }

        /// <summary>
        /// 判断单位流域是否在上级单位流域之内
        /// </summary>
        /// <param name="riverCode">单位流域代码</param>
        /// <param name="subRiverCode">上级单位流域代码</param>
        /// <returns></returns>
        public bool IsInSubRiver(string riverCode, string subRiverCode)
        {
            bool flag = true;
            string[] riverCodes = riverCode.Split(',');
            for (int i = 0; i < riverCodes.Length; i++)
            {
                if (!subRiverCode.Contains(riverCodes[i]))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        /// <summary>
        /// 获得省所有单位

        /// </summary>
        /// <param name="unitCode">当前单位代码，只有前2位有效</param>
        /// <returns>返回json格式所有单位</returns>
        public string GetAllUnit(string unitCode)
        {
            jsonStr = "[";
            int degree = GetUnitDegree(unitCode);//当前单位级别(0省级，1市级，2县级，3乡镇)
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var tb07s = dicEntity.TB07_District.AsQueryable();
            //当前单位
            var curUnit = tb07s.Where(t => t.DistrictCode == unitCode).ToList();
            //下一级单位
            var underOneUnit = tb07s.Where(t => t.pDistrictCode == unitCode).ToList();
            //下二级单位
            string underTwoUnitCode = unitCode.Substring(0, degree * 2 + 2);
            var underTwoUnit = tb07s.Where(t => t.DistrictCode.StartsWith(underTwoUnitCode) && t.DistrictClass == (degree + 4)).ToList();
            //下三级单位
            string underThreeUnitCode = unitCode.Substring(0, degree * 2 + 2);
            var underThreeUnit = tb07s.Where(t => t.DistrictCode.StartsWith(underThreeUnitCode) && t.DistrictClass == (degree + 4 + 1)).ToList();

            iList[0] = ConventToObject(curUnit);
            iList[1] = ConventToObject(underOneUnit);
            iList[2] = ConventToObject(underTwoUnit);
            iList[3] = ConventToObject(underThreeUnit);

            //拼接json  i,m,k,p
            for (int i = 0; i < iList[0].Count; i++)//循环省iList[0]
            {
                object[] obj2 = (object[])iList[0][i];
                jsonStr += "{name:'" + obj2[2] + "',unitCode:'" + obj2[0] + "'";
                jsonStr = i == 0 ? jsonStr + ",open : true" : jsonStr;//open : true是给zTree用的，表示当前这个节点打开
                GetJson(1, Convert.ToInt32(obj2[0]));//递归循环得到所有下级json
                jsonStr += "}";
            }
            dicEntity.Dispose();
            jsonStr += "]";
            return jsonStr;
        }

        public string GetAllUnitObj(string unitCode)
        {
            jsonStr = "[";
            int degree = GetUnitDegree(unitCode);//当前单位级别(0省级，1市级，2县级，3乡镇)
            string tempCode = unitCode.Substring(0, 2);
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var tb07s = dicEntity.TB07_District.Where(t => t.DistrictCode.StartsWith(tempCode)).ToList();
            //当前单位
            var curUnit = tb07s.Where(t => t.DistrictCode == unitCode).ToList();
            //下一级单位
            var underOneUnit = tb07s.Where(t => t.pDistrictCode == unitCode).ToList();
            //下二级单位
            string underTwoUnitCode = unitCode.Substring(0, degree * 2 + 2);
            var underTwoUnit = tb07s.Where(t => t.DistrictCode.StartsWith(underTwoUnitCode) && t.DistrictClass == (degree + 4)).ToList();
            //下三级单位
            string underThreeUnitCode = unitCode.Substring(0, degree * 2 + 2);
            var underThreeUnit = tb07s.Where(t => t.DistrictCode.StartsWith(underThreeUnitCode) && t.DistrictClass == (degree + 4 + 1)).ToList();

            tb07List[0] = curUnit;
            tb07List[1] = underOneUnit;
            tb07List[2] = underTwoUnit;
            tb07List[3] = underThreeUnit;
            //拼接json  i,m,k,p
            for (int i = 0; i < tb07List[0].Count; i++)//循环省iList[0]
            {
                TB07_District tb07 = tb07List[0][i];
                jsonStr += "{name:'" + tb07.DistrictName + "',unitCode:'" + tb07.DistrictCode + "'";
                jsonStr = i == 0 ? jsonStr + ",open : true" : jsonStr;//open : true是给zTree用的，表示当前这个节点打开
                GetJsonObj(1, tb07.DistrictCode);//递归循环得到所有下级json
                jsonStr += "}";
            }
            dicEntity.Dispose();
            jsonStr += "]";
            return jsonStr;
        }

        /// <summary>
        /// 递归循环ilist，得到json格式
        /// </summary>
        /// <param name="arrIndex">ilist数组序号</param>
        /// <param name="curUnitCode">当前单位代码</param>        
        private void GetJson(int arrIndex, int curUnitCode)
        {
            if (arrIndex > 3 || iList[arrIndex] == null)//超出了数组长度了,出口之一
            {
                return;
            }
            int count = 0;
            for (int m = 0; m < iList[arrIndex].Count; m++)
            {
                object[] obj = (object[])iList[arrIndex][m];
                if (Convert.ToInt32(obj[1]) == curUnitCode)//有下级单位
                {
                    if (count == 0)
                        jsonStr += ",children:[";
                    jsonStr += "{name:'" + obj[2] + "',unitCode:'" + obj[0] + "'";
                    //循环下级
                    GetJson(arrIndex + 1, Convert.ToInt32(obj[0]));//递归，调用自己
                    jsonStr += "},";
                    count++;
                }
            }
            if (count > 0)
            {
                jsonStr = jsonStr.Remove(jsonStr.Length - 1);
                jsonStr += "]";
            }
            return;
        }
        private void GetJsonObj(int arrIndex, string curUnitCode)
        {
            if (arrIndex > 3 || tb07List[arrIndex] == null)//超出了数组长度了,出口之一
            {
                return;
            }
            int count = 0;
            for (int m = 0; m < tb07List[arrIndex].Count; m++)
            {
                TB07_District tb07 = tb07List[arrIndex][m];
                if (tb07.pDistrictCode == curUnitCode)//有下级单位
                {
                    if (count == 0)
                        jsonStr += ",children:[";
                    jsonStr += "{name:'" + tb07.DistrictName + "',unitCode:'" + tb07.DistrictCode + "'";
                    //循环下级
                    GetJsonObj(arrIndex + 1, tb07.DistrictCode);//递归，调用自己
                    jsonStr += "},";
                    count++;
                }
            }
            if (count > 0)
            {
                jsonStr = jsonStr.Remove(jsonStr.Length - 1);
                jsonStr += "]";
            }
            return;
        }

        /// <summary>
        /// 根据单位代码得到单位级别
        /// </summary>
        /// <param name="unitcode"></param>
        /// <returns>0省级，1市级，2县级，3乡镇，与前面定义的iList对应</returns>
        private int GetUnitDegree(string unitcode)
        {
            if (unitcode.IndexOf("000000") >= 0)
                return 0;
            else if (unitcode.IndexOf("0000") >= 0)
                return 1;
            else if (unitcode.IndexOf("00") >= 0)
                return 2;
            else
                return 3;
        }

        /// <summary>转换成Ilist
        /// </summary>
        /// <param name="tb07s"></param>
        /// <returns></returns>
        private IList ConventToObject(IList<TB07_District> tb07s)
        {
            IList temp = new ArrayList();
            for (int i = 0; i < tb07s.Count; i++)
            {
                object[] obj = new object[4];
                obj[0] = tb07s[i].DistrictCode;
                obj[1] = tb07s[i].pDistrictCode;
                obj[2] = tb07s[i].DistrictName;
                obj[3] = tb07s[i].DistrictClass;
                temp.Add(obj);
            }
            return temp;
        }

        /// <summary>检查该单位代码是否存在，返回记录条数
        /// </summary>
        /// <param name="unitCode">单位代码</param>
        /// <param name="limit">如果是检查上级代码是否存在，传入大于0的数</param>
        /// <returns></returns>
        public int CheckUnitCodeExist(string unitCode, int limit)
        {
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var tb07s = dicEntity.TB07_District.Where(t => t.DistrictCode == unitCode).AsQueryable();
            if (limit > 0)
            {
                tb07s = tb07s.Where(t => t.DistrictClass != 5);
            }
            int count = tb07s.Count();
            dicEntity.Dispose();

            return count;
        }

        /// <summary>检查该上级代码的下级单位编号是否存在，返回记录条数
        /// </summary>
        /// <param name="unitCode">上级单位代码</param>
        /// <param name="uorder">新加单位代码</param>
        /// <returns></returns>
        public int CheckUorderExist(string unitCode, decimal uorder)
        {
            FXDICTEntities dicEntity = Persistence.GetDbEntities();
            var tb07s = dicEntity.TB07_District.Where(t => t.pDistrictCode == unitCode && t.Uorder == uorder).AsQueryable();
            int count = tb07s.Count();
            dicEntity.Dispose();

            return count;
        }
    }
}
