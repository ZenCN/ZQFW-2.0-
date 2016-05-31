using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;

namespace ZQFW.Views.Statistics
{

       public partial class Map : System.Web.Mvc.ViewPage
       {
        public string UnitCode;

        protected void Page_Load(object sender, EventArgs e)
        {
            UnitCode = Request.Cookies["UnitCode"].Value;
        }
              [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
		
	}
}