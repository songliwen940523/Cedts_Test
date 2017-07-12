using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;


namespace Cedts_Test.Areas.Admin.Controllers
{
    public class LogController : Controller
    {
        ICedts_LogRepository _log;
        public LogController(ICedts_LogRepository l)
        {
            _log = l;
        }

        [Filter.LogFilter(Description="查看日志")]
        public ActionResult Index(int? id)
        {
            return View(_log.SelectLog(id));
        }

        [Authorize(Roles = "超级管理员")]
        [HttpPost]
        [Filter.LogFilter(Description = "删除日志")]
        public JsonResult Delete(string ids)
        {
            try
            {
                if (_log.Delete(ids))
                {
                    return Json("数据已删除");
                }
                else
                {
                    return Json("数据不存在，删除失败");
                }

            }
            catch (Exception ex)
            {
                return Json("系统发生错误: " + ex.Message);
            }
        }

    }
}
