using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZQFW.Controllers
{
    public class MainController : Controller
    {
        public ActionResult SH()
        {
            bool debug = Request["debug"] == null ? false : true;
            if (debug)
            {
                return View("~/Views/Debug/Main.SH.cshtml");
            }
            else
            {
                return View("~/Views/Release/Main.SH.cshtml");
            }
        }

        public ActionResult Index()
        {
            bool debug = Request["debug"] == null ? false : true;
            if (Session["SESSION_USER"] != null)
            {
                if (debug)
                {
                    return View("~/Views/Debug/Main.cshtml");
                }
                else
                {
                    return View("~/Views/Release/Main.cshtml");
                }
            }
            else
            {
                return Redirect("/Login");
            }
        }
    }
}
