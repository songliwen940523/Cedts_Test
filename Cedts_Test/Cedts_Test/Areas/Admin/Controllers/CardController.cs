using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;

namespace Cedts_Test.Areas.Admin.Controllers
{
    public class CardController : Controller
    {
        #region 实例化接口对象
        ICedts_CardRepository _card;
        public CardController(ICedts_CardRepository r)
        {
            _card = r;
        }
        #endregion

        public static string info = string.Empty;
        public ActionResult Index(int? id)
        {
            if (info == string.Empty)
            {
                return View(_card.SelectAllCard(id, 4, 4, 4, 0, ""));
            }
            else
            {
                if (id == null)
                {
                    info = string.Empty;
                    return View(_card.SelectAllCard(id, 4, 4, 4, 0, ""));
                }
                else
                {
                    List<string> infoList = new List<string>();
                    infoList = info.Split(',').ToList();
                    TempData["info"] = info;
                    return View(_card.SelectAllCard(id, int.Parse(infoList[0]), int.Parse(infoList[1]), int.Parse(infoList[2]), int.Parse(infoList[3]), infoList[4]));
                }
            }
        }

        [HttpPost]
        public ActionResult Index(int? id, FormCollection form)
        {
            id = 1;
            int CKind = int.Parse(form["CKind"]);
            int CType = int.Parse(form["CType"]);
            int AState = int.Parse(form["AState"]);
            int SCondition = int.Parse(form["SCondition"]);
            string TCondition = form["TCondition"];
            info = CKind + "," + CType + "," + AState + "," + SCondition + "," + TCondition;
            TempData["info"] = info;
            return View(_card.SelectAllCard(id, CKind, CType, AState, SCondition, TCondition));
        }

        public ActionResult Create()
        {
            List<SelectListItem> CardKind = new List<SelectListItem>();
            CardKind.Add(new SelectListItem { Text = "实体卡", Value = "0" });
            CardKind.Add(new SelectListItem { Text = "虚拟卡", Value = "1" });
            ViewData["CardKind"] = CardKind;

            List<SelectListItem> CardType = new List<SelectListItem>();
            CardType.Add(new SelectListItem { Text = "年卡", Value = "0" });
            CardType.Add(new SelectListItem { Text = "月卡", Value = "1" });
            CardType.Add(new SelectListItem { Text = "次数卡", Value = "2" });
            ViewData["CardType"] = CardType;

            var PartnerList = _card.SelectPartner();
            ViewData["PartnerID"] = new SelectList(PartnerList, "PartnerID", "PartnerName");
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection form, CEDTS_Card card)
        {
            System.Data.DataTable dt = new System.Data.DataTable();//为excel创建表格
            dt.Columns.Add("SerialNumber", System.Type.GetType("System.String"));
            dt.Columns.Add("PassWord", System.Type.GetType("System.String"));

            int CardCount = int.Parse(form["num"]);
            card.CreateTime = DateTime.Now;
            card.ActivationState = 0;
            switch (card.CardType)
            {
                case 0: card.EffectiveTime = "一年"; break;
                case 1: card.EffectiveTime = "一月"; break;
                case 2: card.EffectiveTime = "30次"; break;
                default: break;
            }
            card.CreateUser = _card.GetUserIDbyAccount(User.Identity.Name);
            for (int i = 0; i < CardCount; i++)
            {
                DataRow dr = dt.NewRow();

                var SerialNumberList = _card.SelectAllCard(null, 4, 4, 4, 0, "").Select(p => p.SerialNumber).ToList();
                string txt = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16).ToUpper();
                card.SerialNumber = txt;
                if (SerialNumberList.Contains(card.SerialNumber))
                {
                    i--;
                    continue;
                }
                string pwd = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8).ToUpper();
                card.PassWord = _card.HashPassword(pwd);

                CEDTS_Card c = new CEDTS_Card();
                c.ActivationState = card.ActivationState;
                c.ActivationTime = card.ActivationTime;
                c.ActivationUser = card.ActivationUser;
                c.CardKind = 1;
                c.CardType = card.CardType;
                c.CreateTime = card.CreateTime;
                c.CreateUser = card.CreateUser;
                c.Discount = 1;
                c.EffectiveTime = card.EffectiveTime;
                c.Money = card.Money;
                c.OverdueTime = card.OverdueTime;
                c.PartnerID = card.PartnerID;
                c.PassWord = card.PassWord;
                c.SerialNumber = card.SerialNumber;
                _card.CreateCard(c);

                dr[0] = txt;
                dr[1] = pwd;
                dt.Rows.Add(dr);
            }

            string[] listname = { "序列号", "密码" };
            string[] cols = { "SerialNumber", "PassWord" };

            ToExcel.tableToExcel(dt, listname, cols);
            DoExcel();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteCard(int id)
        {
            return Json(_card.DeleteCard(id));
        }

        #region 释放Excel进程

        private void DoExcel()
        {
            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            //这里释放所有引用的资源
            application.Quit();
            KillExcel(application);
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public static void KillExcel(Microsoft.Office.Interop.Excel.Application excel)
        {
            IntPtr t = new IntPtr(excel.Hwnd); //得到这个句柄，具体作用是得到这块内存入口 

            int k = 0;
            GetWindowThreadProcessId(t, out k); //得到本进程唯一标志k
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k); //得到对进程k的引用
            p.Kill(); //关闭进程k
        }

        #endregion
        
    }
}
