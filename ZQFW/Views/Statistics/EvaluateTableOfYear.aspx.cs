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
    public partial class EvaluateTableOfYear : System.Web.Mvc.ViewPage
    {
        public string UnitCode;   //使用该系统的行政单位代码
        protected void Page_Load(object sender, EventArgs e)
        {
            UnitCode = Request.Cookies["unitcode"].Value;  //使用该系统的行政单位代码
        }
    }
}
