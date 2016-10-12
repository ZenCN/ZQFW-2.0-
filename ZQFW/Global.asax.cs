using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DBHelper;
using EntityModel;

namespace ZQFW
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Login", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Persistence persistence = new Persistence();
            persistence.PersistenceUnits("35000000 ");  //浙江 33000000  湖南 43000000  黑龙江 23000000  内蒙古 15000000 江西 36000000 吉林 22000000 广西 45000000 广东 44000000 福建省 35000000

            LogicProcessingClass.ReportOperate.Message.ReadMsgFillInApplicaion();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            string sessionId = Session.SessionID;
        }

        protected void Session_End(object sender, EventArgs e)
        {
            string strUserId = Session["SESSION_USER"] as string;
            Session["SESSION_ID"] = "exit";//强制变成其他的
            Dictionary<string, int> dicList = Application.Get("GLOBAL_USER_LIST") as Dictionary<string, int>;
            if (strUserId != null && dicList != null && dicList.ContainsKey(strUserId))
            {
                int count = dicList[strUserId];
                if (count<=1)
                {
                    dicList.Remove(strUserId);
                }
                else
                {
                    count--;
                    dicList[strUserId] = count;
                }
                Application.Add("GLOBAL_USER_LIST", dicList);
            }
        }
    }
}
