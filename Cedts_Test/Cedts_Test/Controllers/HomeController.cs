using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Models;
using System.Web.Security;

namespace Cedts_Test.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        //定义接口对象
        ICedts_UserRepository _userrepository;
        ICedts_HomeRepository _home;
        public HomeController(ICedts_UserRepository r, ICedts_HomeRepository h)
        {
            _userrepository = r;
            _home = h;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            return PartialView();
        }
        #region 系统成绩统计查询
        public JsonResult TotalSocreView()
        {
            return Json(_home.TotalScore());
        }
        #endregion

        #region 系统知识点统计查询
        public JsonResult KonwledgeView()
        {
            return Json(_home.KPMaster());
        }
        #endregion

        #region 题型统计查询
        public JsonResult ItemInfo()
        {
            return Json(_home.ItemInfo());
        }
        #endregion
        [Authorize]
        public ActionResult UserStatistics()
        {
            return PartialView();
        }
        [Authorize]
        #region 个人最近十次成绩统计
        public ActionResult UserScore()
        {

            return Json(_home.UserScore(User.Identity.Name));
        }
        #endregion
        [Authorize]
        #region 用户知识点统计查询
        public ActionResult UserKPInfo()
        {
            return Json(_home.UserKpMaster(User.Identity.Name));
        }
        #endregion
        [Authorize]
        #region 用户题型统计
        public ActionResult UserItemInfo()
        {
            if (User.Identity.Name == "游客")
            {
                return Json("");
            }
            else
            {
                return Json(_home.UserItemInfo(User.Identity.Name));
            }
        }
        #endregion

        public ActionResult UserKnowledgeInfo()
        {
            return Json(_home.UserKnowledgeInfo(User.Identity.Name));
        }

        public ActionResult ItemInfoList()
        {
            return Json(_home.UserItemList(User.Identity.Name));
        }
    }
}
