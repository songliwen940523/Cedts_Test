using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;

namespace Cedts_Test.Areas.Admin.Controllers
{
    public class NoticeController : Controller
    {
        ICedts_NoticeRepository _notice;
        public NoticeController(ICedts_NoticeRepository n)
        {
            _notice = n;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region 系统概况

        public PartialViewResult System()
        {
            return PartialView(_notice.GetSystemInfo());
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult System(string content, string intro)
        {
            _notice.EditSystem(content, intro);
            return Json("1");
        }

        #endregion

        #region 特色功能

        public PartialViewResult Features()
        {
            var featuresList = _notice.GetFeaturesInfo();
            TempData["List"] = featuresList;
            ViewData["content1"] = featuresList[0].Content;
            ViewData["content2"] = featuresList[1].Content;
            ViewData["content3"] = featuresList[2].Content;
            ViewData["txt1"] = featuresList[0].Intro;
            ViewData["txt2"] = featuresList[1].Intro;
            ViewData["txt3"] = featuresList[2].Intro;

            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Features(string content1, string content2, string content3, string txt1, string txt2, string txt3)
        {
            var featuresList = TempData["List"] as List<CEDTS_CoreFeatures>;
            featuresList[0].Content = content1;
            featuresList[1].Content = content2;
            featuresList[2].Content = content3;
            featuresList[0].Intro = txt1;
            featuresList[1].Intro = txt2;
            featuresList[2].Intro = txt3;
            _notice.EditFeatures(featuresList);
            return Json("1");
        }
        #endregion

        #region 使用说明

        public PartialViewResult Instructions()
        {
            return PartialView(_notice.GetInstructionsInfo());
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Instructions(string content4)
        {
            _notice.EidtInstructions(content4);
            return Json("1");
        }

        #endregion

        #region 联系方式

        public PartialViewResult Contact()
        {
            return PartialView(_notice.GetContactInfo());
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Contact(string Name, string Tel, string ZipCode, string Address, string Url)
        {
            CEDTS_Contact contact = new CEDTS_Contact();
            contact.Name = Name;
            contact.Tel = Tel;
            contact.ZipCode = ZipCode;
            contact.Address = Address;
            contact.Url = Url;
            _notice.EidtContact(contact);
            return Json("1");
        }

        #endregion

        #region 名言显示
        public PartialViewResult Saying()
        {
            return PartialView(_notice.GetSayingInfo());
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Saying(string content5, string note)
        {
            _notice.EidtSaying(content5, note);
            return Json("1");
        }

        #endregion

        #region 意见反馈
        public ActionResult FeedBack()
        {
            return View(_notice.GetFeedback());
        }

        public ActionResult DeleteFeedBack(int id)
        {
            _notice.DeleteFeedBackbyID(id);
            return RedirectToAction("FeedBack");
        }

        public ActionResult CleanFeedBack()
        {
            _notice.DeleteFeedBackAll();
            return RedirectToAction("FeedBack");
        }
        #endregion

    }
}
