using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Models;

namespace Cedts_Test.Filter
{
    public class LogFilter : ActionFilterAttribute
    {
        public string Description { get; set; }

        public LogFilter()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity.Name != "")
            {
                using (CedtsEntities db = new CedtsEntities())
                {
                    CEDTS_Log log = new CEDTS_Log()
                    {
                        UserID = db.CEDTS_User.Where(p => p.UserAccount == filterContext.HttpContext.User.Identity.Name).Select(p => p.UserID).FirstOrDefault(),
                        Action = filterContext.RouteData.Values["controller"] + "." + filterContext.RouteData.Values["action"],
                        ClientIP = filterContext.HttpContext.Request.UserHostAddress,
                        LogTime = DateTime.Now,
                        Content = this.Description
                    };

                    db.AddToCEDTS_Log(log);
                    db.SaveChanges();
                }
            }
        }
    }
}