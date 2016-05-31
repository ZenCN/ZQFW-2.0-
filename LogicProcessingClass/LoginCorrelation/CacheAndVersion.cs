using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LogicProcessingClass.LoginCorrelation
{
    public class CacheAndVersion
    {
        public string GetNewVersion()
        {
            string versionStr = ((System.Reflection.Assembly)System.Web.HttpContext.Current.Application["Assembly"]).GetName().Version.ToString();
            return versionStr;
        }
        /// <summary>
        /// 检查版本是否有更新
        /// </summary>
        /// <returns>有更新（true）</returns>
        public bool CheckVersionChange()
        {
            bool versionChange = false;
            string newVersion = GetNewVersion();
            if (HttpContext.Current.Request.Cookies["cookiesVersion"] != null)//判断是否存在该cookies
            {
                string oldVersion = HttpContext.Current.Request.Cookies["cookiesVersion"].Value;
                if (!newVersion.Equals(oldVersion))
                {
                    versionChange = true;
                }
                else
                {
                    HttpContext.Current.Response.Cookies["cookiesVersion"].Value = newVersion;
                    HttpContext.Current.Response.Cookies["cookiesVersion"].Expires = DateTime.Now.AddDays(30);
                    versionChange = false;
                }
            }
            else
            {
                HttpCookie aCookie = new HttpCookie("cookiesVersion");
                aCookie.Value = newVersion;
                aCookie.Expires = DateTime.Now.AddDays(30);
                HttpContext.Current.Response.Cookies.Add(aCookie);
                versionChange = true;
            }
            return versionChange;
        }
    }
}
