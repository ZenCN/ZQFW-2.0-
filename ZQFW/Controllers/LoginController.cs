using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using DBHelper;
using EntityModel;
using LogicProcessingClass.LoginCorrelation;
using LogicProcessingClass.AuxiliaryClass;

namespace ZQFW.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            string str = Request.Url.ToString();
            HttpCookie single_sign_on = new HttpCookie("sso");
            int start_index = str.ToLower().IndexOf("login?param=");
            bool _return = true;
            if (Request.Url != null && start_index > 0)
            {
                str = str.Substring(start_index + 12);
                str = new LogicProcessingClass.Tools().EncryptOrDecrypt(1,
                    str, DateTime.Now.ToString("yyyyMMdd"));
                string[] arr = str.Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);
                str = arr[0].Substring(arr[0].IndexOf("=") + 1);
                HttpApplicationState app = System.Web.HttpContext.Current.Application;
                FXDICTEntities dictEntities = app["FXDICTEntities"] == null
                    ? new FXDICTEntities()
                    : (FXDICTEntities)app["FXDICTEntities"];

                /*var sso = dictEntities.TB64_SSO.Where(t => t.Account == str).SingleOrDefault();

                if (sso != null)
                {
                    string code = sso.UnitCode, name = sso.UnitName;
                    if (code.Length > 0)
                    {
                        Response.Cookies.Add(new HttpCookie("unitcode", code));
                        Response.Cookies.Add(new HttpCookie("unitname", name));
                        Response.Cookies.Add(new HttpCookie("fullname", name));
                        Response.Cookies.Add(new HttpCookie("limit", DBHelper.Tools.GetLimitByCode(code).ToString()));
                        Session["SESSION_USER"] = sso.UnitCode;
                        Session["SESSION_ID"] = Session.SessionID;

                        _return = false;
                    }
                }*/
            }

            if (_return)
            {
                single_sign_on.Value = "0";
                Response.Cookies.Add(single_sign_on);

                bool debug = Request["debug"] == null ? false : true;
                if (debug)
                {
                    return File("~/Views/Debug/Login.html", "text/html");
                }
                else
                {
                    return File("~/Views/Release/Login.html", "text/html");
                }
            }
            else
            {
                single_sign_on.Value = "1"; //单点登录
                Response.Cookies.Add(single_sign_on);

                return View("~/Views/Main.cshtml");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="unitcode"></param>
        /// <param name="password"></param>
        /// <returns>0：密码错误，2：已经在其他地方登录，1和其他说明密码验证成功</returns>
        [HttpPost]
        public string Validate(string limit, string unitcode, string password, string type)
        {
            LoginSystem lgn = new LoginSystem();
            string result = "";
            int number = 0;

            if (password == "view")
            {
                result = "1";
                Response.Cookies.Add(new HttpCookie("authority", "1"));
            }
            else
            {
                result = lgn.UserLogin(unitcode, password, int.Parse(limit), type.ToUpper()).ToString();
                Response.Cookies.Add(new HttpCookie("authority", "0"));
            }

            if (int.TryParse(result, out number))
            {
                if (number > 0)
                {
                    result = number.ToString();
                    Session["SESSION_USER"] = unitcode;
                    Session["SESSION_ID"] = Session.SessionID;

                    /*int _out = 0;
                    int.TryParse(Remote_Addr("validate"), out _out);
                    if (_out > 0)
                    {
                        Response.Cookies.Add(new HttpCookie("mac", Request["unitcode"].ToString().Trim() + ":" + Request.ServerVariables["Remote_Addr"]));
                        Remote_Addr("record");
                    }*/
                }
                else
                {
                    result = "0";
                }
            }
            return result;
        }

        public string Exit()
        {
            string unitcode = Request.Cookies["unitcode"].Value;
            LoginSystem lgn = new LoginSystem();
            string result = lgn.UserExit(unitcode).ToString();
            Session["SESSION_ID"] = "exit";
            return result;
        }



        public int LoginSession()
        {
            Response.Expires = -1;
            string sessionID = Session.SessionID;
            string cookiesUnitCode = Request.Cookies["unitcode"] == null ? "" : Request.Cookies["unitcode"].Value;
            if (Session["SESSION_USER"] != null && Session["SESSION_ID"] != null && Session["SESSION_ID"].ToString() == sessionID && Session["SESSION_USER"].ToString() == cookiesUnitCode)//都符合我后台判断要求，不是非法登录
            {
                //这里不执行，会有问题
                return 1;
            }
            else
            {
                //Response.ContentType = "text/html";
                //Response.WriteFile("~/Views/Login.htm");
                return 0;
            }
        }

        public string Read(string unitcode)
        {
            string result = "";
            Persistence per = new Persistence();
            Dictionary<string, District> dic = per.GetLowerUnits(unitcode);
            if (dic.Count > 0)
            {
                result += "[";
                foreach (District dis in dic.Values)
                {
                    if (dis.UnitName != "全区/县" && !dis.UnitName.EndsWith("本级") && dis.Del != "1")//登录中隐藏全区/县或者本级
                    {
                        result += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "'},";
                    }
                }
                result = result.Remove(result.Length - 1) + "]";
            }
            else
            {
                result = "[]";
            }
            return result;
        }

        public string ReadUnder(string unitcode)
        {
            string result = "";
            Persistence per = new Persistence();
            Dictionary<string, District> dic = per.GetLowerUnits(unitcode);
            if (dic.Count > 0)
            {
                result += "[";
                foreach (District dis in dic.Values)
                {
                    result += "{UnitCode:'" + dis.UnitCode + "',UnitName:'" + dis.UnitName + "'},";
                }
                result = result.Remove(result.Length - 1) + "]";
            }
            else
            {
                result = "[]";
            }
            return result;
        }
        //此方法已被Validate方法替换
        //判断用户名和密码是否正确，如果正确把对应信息写入cookies中
        // GET: /Login/Lgn
        //public JsonResult Lgn()
        //{
        //    string unitCode = Request["UnitCode"];
        //    string password = Request["Password"];
        //    int loginLimit = Convert.ToInt32(Request["Limit"]);
        //    string unitName = Request["UnitName"];
        //    string longUnitName = Request["LongUnitName"];

        //    LoginSystem lgn = new LoginSystem();
        //    JsonResult jsr = new JsonResult();
        //    int result = lgn.UserLogin(unitCode, password, loginLimit);
        //    if (result > 0)
        //    {
        //        HttpCookie unitcode = new HttpCookie("unitcode");
        //        unitcode.Value = unitCode;
        //        Response.Cookies.Add(unitcode);

        //        HttpCookie limit = new HttpCookie("limit");
        //        limit.Value = loginLimit.ToString();
        //        Response.Cookies.Add(limit);

        //        HttpCookie RoleName = new HttpCookie("RoleName");  
        //        RoleName.Value = lgn.RoleName;
        //        Response.Cookies.Add(RoleName);
        //    }
        //    HttpCookie islogin = new HttpCookie("islogin");
        //    islogin.Value = result == 1 ? "true" : "false";
        //    Response.Cookies.Add(islogin);
        //    jsr = Json(result.ToString());
        //    jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        //    return jsr;
        //}

        //根据单位代码获取该单位下级单位列表
        // GET: /Login/GetUnitList
        public JsonResult GetUnitList(string unitCode)
        {
            CommonFunction comm = new CommonFunction();
            JsonResult jsr = new JsonResult();
            jsr = Json(comm.GetLowerUnitList(unitCode));
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        public Dictionary<string, string> GetLowerUnits(string UpperUnitCode)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            FXDICTEntities fxdict = new FXDICTEntities();
            var tb07s = from district in fxdict.TB07_District
                        where district.pDistrictCode == UpperUnitCode
                        select district;
            return null;
        }

        //修改登录密码
        // GET: /Login/ChangePWD
        public JsonResult ChangePWD(string limit, string unitCode, string newPWD)
        {
            JsonResult jsr = new JsonResult();
            Entities getEntity = new Entities();
            BusinessEntities busEntity = getEntity.GetEntityByLevel(Convert.ToInt32(limit));
            var lgn = busEntity.LGN.Where(t => t.LoginName == unitCode);
            if (lgn != null)
            {
                try
                {
                    lgn.First().PWD = newPWD;
                    busEntity.SaveChanges();
                    jsr = Json("1");
                }
                catch (Exception ex)
                {
                    jsr = Json(ex.ToString());
                }
            }
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

        //修改登录密码
        // GET: /Login/ChangePWD
        public JsonResult ChangePWD(string limit, string unitCode, string userName, string newPWD)
        {
            JsonResult jsr = new JsonResult();
            Entities getEntity = new Entities();
            BusinessEntities busEntity = getEntity.GetEntityByLevel(Convert.ToInt32(limit));
            var lgn = busEntity.LGN.Where(t => t.LoginName == unitCode && t.UserName == userName);
            if (lgn != null)
            {
                try
                {
                    lgn.First().PWD = newPWD;
                    busEntity.SaveChanges();
                    jsr = Json("1");
                }
                catch (Exception ex)
                {
                    jsr = Json(ex.ToString());
                }
            }
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }

/*        /// <summary>
        /// 版本检测
        /// </summary>
        /// <returns>如果版本有改变，返回1,否则返回0</returns>
        /// GET: /Login/CheckVersion
        public JsonResult CheckVersion()
        {
            CacheAndVersion version = new CacheAndVersion();
            bool versionFlag = version.CheckVersionChange();
            JsonResult jsr = new JsonResult();
            string temp = "0";
            if (versionFlag)
            {
                temp = "1";
            }
            jsr = Json(temp);
            jsr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsr;
        }*/

        /// <summary>
        /// 根据单位代码查询密码，或根据单位名字进行模糊查询，如：Login/Query?u=43000000 或 Login/Query?l=2&u=湖南
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string Query(string u)
        {
            string unit_info = "未能查到相关信息，请检查您提供的参数";
            if (!string.IsNullOrEmpty(u))
            {
                string code = "";
                string name = "";
                int limit = 0;
                List<LGN> list = null;
                Entities entity = new Entities();
                BusinessEntities busEntity = null;

                if (int.TryParse(u, out limit))  //只输入Code
                {
                    code = limit.ToString();
                    if (code.Substring(2).ToString() == "000000")
                    {
                        limit = 2;
                    }
                    else if (code.Substring(4).ToString() == "0000")
                    {
                        limit = 3;
                    }
                    else if (code.Substring(6).ToString() == "00")
                    {
                        limit = 4;
                    }
                    else
                    {
                        limit = 5;
                    }
                    busEntity = entity.GetEntityByLevel(limit);
                    list = busEntity.LGN.Where(l => l.LoginName == code).ToList<LGN>();
                }
                else //输入了Name和Limit
                {
                    if (int.TryParse(Request.QueryString["l"], out limit))
                    {
                        busEntity = entity.GetEntityByLevel(limit);
                        list = busEntity.LGN.Where(l => l.RealName.Contains(u)).ToList<LGN>();
                    }
                    else
                    {
                        return "请提供正确的单位级别";
                    }
                }

                if (list.Count > 0)
                {
                    unit_info = "";
                    for (int i = 0; i < list.Count; i++)
                    {
                        unit_info += "UnitCode：" + list[i].LoginName + "<br/>FullName：" + list[i].RealName + "<br/>" +
                                     "Password：***" + list[i].PWD + "***<br/>--------------------------<br/>";
                    }

                    unit_info = "<font style='font-size: x-large;font-weight: bolder;'>" + unit_info + "<font>";
                }
            }

            return unit_info;
        }
    }
}
