using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModel;
using DBHelper;

namespace ZQFW.Controllers.Partial
{
    public class HeadController : Controller
    {
        //
        // GET: /Head/

        public ActionResult Index()
        {
            return View();
        }

        public void GetTemplate() 
        {
            Response.ContentType = "text/plain";
            Response.WriteFile("~/JS/Template/Public/Dialog.htm");
        }

        public string ModifyPwd(string oldPwd, string newPwd)
        {
            string result = "";
            int limit = int.Parse(Request["limit"]);
            string unitcode = Request["unitcode"];
            Entities getEntity = new Entities();
            BusinessEntities busEntity = getEntity.GetEntityByLevel(Convert.ToInt32(limit));
            var lgn = busEntity.LGN.Where(t => t.LoginName == unitcode);
            if (lgn != null)
            {
                try
                {
                    result = "0";

                    if (Request["ord_code"].ToUpper() == "SH01")
                    {
                        if (lgn.First().PWD_SH == oldPwd)
                        {
                            lgn.First().PWD_SH = newPwd;
                            busEntity.SaveChanges();
                            result = "1";
                        }
                    }
                    else
                    {
                        if (lgn.First().PWD == oldPwd)
                        {
                            lgn.First().PWD = newPwd;
                            busEntity.SaveChanges();
                            result = "1";
                        }
                    }

                }
                catch (Exception ex)
                {
                    result = "Error：" + ex.Message;
                }
            }
            return result;
        }
    }
}
