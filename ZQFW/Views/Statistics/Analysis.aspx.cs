using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace ZQFW.Views
{
    public partial class Analysis : System.Web.Mvc.ViewPage
    {
        public string UnitCode = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            UnitCode = Request.Cookies["unitcode"].Value;
        }
    }
}