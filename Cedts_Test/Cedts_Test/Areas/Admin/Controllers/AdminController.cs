using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace Cedts_Test.Areas.Admin.Controllers
{
    [Authorize(Roles = "超级管理员")]
    public class AdminController : Controller
    {
        #region 实例化 ICedts_AdminRepository
        ICedts_AdminRepository _admin;
        public AdminController(ICedts_AdminRepository r)
        {
            _admin = r;
        }
        #endregion

        #region  后台用户管理展示
        [Filter.LogFilter(Description = "查看后台用户列表")]
        public ActionResult Index(int? id)
        {
            return View(_admin.SelectUser(id));
        }
        #endregion

        #region 后台用户新增
        public ActionResult Create()
        {
            List<SelectListItem> Sex = new List<SelectListItem>();
            Sex.Add(new SelectListItem { Text = "男", Value = "男" });
            Sex.Add(new SelectListItem { Text = "女", Value = "女" });
            ViewData["Sex"] = Sex;
            return View();
        }

        [HttpPost]
        [Filter.LogFilter(Description = "添加一个后台用户")]
        public ActionResult Create(CEDTS_User user)
        {
            try
            {
                user.UserPassword = _admin.HashPassword(user.UserPassword);
                user.Role = "管理员";
                _admin.CreateUser(user);

                return RedirectToAction("Index");
            }

            catch
            {
                return View();
            }
        }
        #endregion

        #region 后台用户编辑
        public ActionResult Edit(int id)
        {
            var Name = _admin.SelectEditUser(id);

            List<SelectListItem> Sex = new List<SelectListItem>();
            if (Name.Sex == "男")
            {
                Sex.Add(new SelectListItem { Text = "男", Value = "男", Selected = true });
                Sex.Add(new SelectListItem { Text = "女", Value = "女" });
                ViewData["Sex"] = Sex;
            }
            else
            {
                Sex.Add(new SelectListItem { Text = "男", Value = "男" });
                Sex.Add(new SelectListItem { Text = "女", Value = "女", Selected = true });
                ViewData["Sex"] = Sex;
            }

            return View(Name);
        }

        [HttpPost]
        [Filter.LogFilter(Description = "修改后台用户信息")]
        public ActionResult Edit(CEDTS_User user)
        {
            if (user.UserPassword != "" && user.UserPassword != null)
            {
                user.UserPassword = _admin.HashPassword(user.UserPassword);
            }
            _admin.EditUser(user);
            return RedirectToAction("Index");
        }
        #endregion

        #region 后台用户删除
        [Filter.LogFilter(Description = "删除后台用户")]
        public JsonResult DeleteUser(string[] name)
        {
            int num = 0;
            using (TransactionScope tran = new TransactionScope())
            {
                num = _admin.DeleteUser(name);
                tran.Complete();
            }
            return Json(num);
        }
        #endregion

        #region 备份数据库

        public string DbBackup()
        {

            SqlConnection connection = new SqlConnection("Data Source=202.141.163.28;User ID=sa;Password=123!@#qwe");
            string dbName = "Cedts";
            string dbFileName = dbName + ".bak";
            //备份数据库 

            SqlCommand command = new SqlCommand("use master;backup database @name to disk=@path;", connection);
            connection.Open();
            string path = @"D:\Cedts.bak";
            command.Parameters.AddWithValue("@name", dbName);
            command.Parameters.AddWithValue("@path", path);
            command.ExecuteNonQuery();
            connection.Close();
            return "备份成功！";
        }

        #endregion

    }
}
