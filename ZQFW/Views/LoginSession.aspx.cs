using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ZQFW.Views
{
    public partial class LoginSession : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            string sessionID = Session.SessionID;
            string cookiesUnitCode = Request.Cookies["unitcode"] == null ? "" : Request.Cookies["unitcode"].Value;
            if (Session["SESSION_USER"] != null && Session["SESSION_ID"] != null && Session["SESSION_ID"].ToString() == sessionID && Session["SESSION_USER"].ToString() == cookiesUnitCode)
            {

            }
            else
            {
                Response.Redirect("~/Views/Login.htm");
            }
        }
    }
}