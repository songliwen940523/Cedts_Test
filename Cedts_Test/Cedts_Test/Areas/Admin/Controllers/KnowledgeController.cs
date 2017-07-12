using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.Transactions;
namespace Cedts_Test.Areas.Admin.Controllers
{
    [Authorize]
    public class KnowledgeController : Controller
    {
        #region 实例化Cedts_Entities、ICedts_KnowledgeRepository
        Cedts_Entities Cedts = new Cedts_Entities();

        ICedts_KnowledgeRepository icedts;
        public KnowledgeController(ICedts_KnowledgeRepository r)
        {
            icedts = r;
        }
        #endregion

        #region 类型展示
        [Filter.LogFilter(Description = "知识点查看")]
        public ActionResult Index(int? id)
        {
            ViewData["type"] = TempData["type"];
            return View(icedts.SelectSort(id));
        }
        #endregion

        #region 知识点展示
        public ActionResult Side(int? id)
        {
            return PartialView("Side", icedts.SelectSide(id));
        }
        #endregion

        #region 二级知识点展示
        public ActionResult Point(int? id)
        {
            return PartialView("Point", icedts.SelectPoint(id));
        }
        #endregion

        #region 新增题目类型
        public ActionResult CreateSort()
        {
            TempData["type"] = "知识能力类";
            return View();
        }

        [HttpPost]
        [Filter.LogFilter(Description = "添加题目类型")]
        public ActionResult CreateSort(CEDTS_KnowledgePoints part)
        {
            try
            {
                part.KnowledgePointID = Guid.NewGuid();
                part.UperKnowledgeID = part.KnowledgePointID;
                part.Level = 1;
                icedts.CreateSort(part);
                TempData["type"] = "知识能力类";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region 新增知识点
        public ActionResult CreateSide()
        {
            IQueryable<CEDTS_KnowledgePoints> queryResult = null;
            queryResult = Cedts.CEDTS_KnowledgePoints.Where(p => p.Level == 1);
            var TypeList = queryResult.ToList();
            SelectList listItem = new SelectList(TypeList.AsEnumerable(), "KnowledgePointID", "Title");
            ViewData.Add("PartName", listItem);
            TempData["type"] = "知识能力面";
            return View();
        }

        [HttpPost]
        [Filter.LogFilter(Description = "添加知识点")]
        public ActionResult CreateSide(FormCollection form, CEDTS_KnowledgePoints know)
        {
            try
            {
                know.KnowledgePointID = Guid.NewGuid();
                know.Title = form["Title"].ToString();
                know.Describe = form["Describe"].ToString();
                know.Level = 2;
                know.UperKnowledgeID = Guid.Parse(form["Part"]);
                icedts.CreateSide(know);
                TempData["type"] = "知识能力面";
                return RedirectToAction("index");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region 新增二级知识点
        public ActionResult CreatePoint()
        {
            TempData["type"] = "知识能力点";
            ViewData["Sort"] = icedts.GetPartSide();
            ViewData["Side"] = icedts.FirstSide();
            return View();
        }

        #region 二级联动获取二级知识点
        public JsonResult GetSide(Guid? type)
        {

            return Json(icedts.GetSide(type));

        }
        #endregion

        [HttpPost]
        [Filter.LogFilter(Description = "添加二级知识点")]
        public ActionResult CreatePoint(FormCollection form, CEDTS_KnowledgePoints part)
        {
            try
            {
                part.KnowledgePointID = Guid.NewGuid();
                part.Title = form["Title"].ToString();
                part.Describe = form["Describe"].ToString();
                part.Level = 3;
                part.UperKnowledgeID = Guid.Parse(form["Side"]);

                icedts.CreatePoint(part);
                TempData["type"] = "知识能力点";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message.ToString();
                return View("Errmessage");
            }
        }
        #endregion

        #region 修改类型
        public ActionResult EditSort(Guid id)
        {
            TempData["type"] = "知识能力类";
            return View(icedts.SelectEditSort(id));
        }

        [HttpPost]
        [Filter.LogFilter(Description = "修改题目类型")]
        public ActionResult EditSort(CEDTS_KnowledgePoints Knowledge)
        {
            try
            {
                icedts.EditSort(Knowledge);
                TempData["type"] = "知识能力类";
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region 修改知识点
        public ActionResult EditSide(Guid id)
        {
            ViewData["Part"] = icedts.SortList(id);
            TempData["type"] = "知识能力面";
            return View(icedts.SelectEditSide(id));
        }

        [HttpPost]
        [Filter.LogFilter(Description = "修改知识点")]
        public ActionResult EditSide(CEDTS_KnowledgePoints Knowledge, FormCollection form)
        {
            try
            {
                Knowledge.KnowledgePointID = Guid.Parse(form["Part"]);
                icedts.EditSide(Knowledge);
                TempData["type"] = "知识能力面";
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region 修改二级知识点
        public ActionResult EditPoint(Guid id)
        {
            ViewData["Sort"] = icedts.SortList(id);
            ViewData["Side"] = icedts.SideList(id);
            TempData["type"] = "知识能力点";
            return View(icedts.SelectEditPoint(id));
        }

        [HttpPost]
        [Filter.LogFilter(Description = "修改二级知识点")]
        public ActionResult EditPoint(CEDTS_KnowledgePoints Knowledge, FormCollection form)
        {
            try
            {
                Knowledge.UperKnowledgeID = Guid.Parse(form["Side"]);
                icedts.EditPoint(Knowledge);
                TempData["type"] = "知识能力点";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region 删除类型
        [Filter.LogFilter(Description = "删除能力类")]
        public JsonResult DeleteSort(string[] name)
        {
            TempData["type"] = "知识能力类";
            return Json(icedts.DeleteSort(name));
        }
        #endregion

        #region 删除知识点
        [Filter.LogFilter(Description = "删除知识点")]
        public JsonResult DeleteSide(string[] name)
        {
            TempData["type"] = "知识能力面";
            int num = 0;
            using (TransactionScope tran = new TransactionScope())
            {
                num = icedts.DeleteSide(name);
                tran.Complete();
            }
            return Json(num);
        }
        #endregion

        #region 删除二级知识点
        [Filter.LogFilter(Description = "删除二级知识点")]
        public JsonResult DeletePoint(string[] name)
        {
            TempData["type"] = "知识能力点";
            return Json(icedts.DeletePoint(name));
        }
        #endregion

        #region 验证Point
        [HttpPost]
        public bool AjaxPointTitle(string Title, int type)
        {
            return icedts.AjaxCheckPointTitle(Title, type);
        }
        #endregion

    }
}
