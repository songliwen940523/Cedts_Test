using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Text.RegularExpressions;
namespace Cedts_Test.Areas.Admin.Controllers
{
    [Authorize(Roles = "教师")]
    public class UserController : Controller
    {
        ICedts_UserRepository _icedts;
        public UserController(ICedts_UserRepository r)
        {
            _icedts = r;
        }
        public static Guid PartnerID = Guid.Empty;
        public static Guid MajorID = Guid.Empty;
        public static Guid GradeID = Guid.Empty;
        public static Guid ClassID1 = Guid.Empty;


        public ActionResult ClassList()
        {
            int UserID = _icedts.SelectUserByAccout(User.Identity.Name).UserID;
            return View(_icedts.SelectClassList(UserID));
        }

        [Filter.LogFilter(Description = "查看学生列表")]
        public ActionResult Index(int? id,Guid ClassID)
        {
            ClassID1 = ClassID;
            var List = _icedts.SelectUser(id, ClassID);
            ViewData["Tcount"] = _icedts.SelectAllUser(ClassID);
            ViewData["Fcount"] = _icedts.SelectPassUser(ClassID);
            ViewData["ClassId"] = ClassID;
            ViewData["Class"] = _icedts.GetClassbyID(ClassID).ClassName;
            ViewData["UserID"] = _icedts.SelectUserByAccout(User.Identity.Name).UserID;
            return View(List);
        }

        public ActionResult GetClassInfo(int UserID)
        {
            List<Class> classlist = _icedts.SelectClassList(UserID);
            return Json(classlist);
        }

        public ActionResult Create()
        {
            List<SelectListItem> Sex = new List<SelectListItem>();
            Sex.Add(new SelectListItem { Text = "男", Value = "男" });
            Sex.Add(new SelectListItem { Text = "女", Value = "女" });
            ViewData["Sex"] = Sex;
            ViewData["Classid"] = ClassID1;
            return View();
        }

        [Filter.LogFilter(Description = "导入学生信息")]
        [HttpPost]
        public ActionResult Create(CEDTS_User User)
        {
            User.UserPassword = _icedts.HashPassword(User.UserPassword);
            User.Role = "普通用户";
            User.State = true;
            User.RegisterTime = DateTime.Now;
            User.ClassID = ClassID1;
            _icedts.SaveUser(User);
            return RedirectToAction("Index", new { ClassID = ClassID1 });
        }

        [Filter.LogFilter(Description = "删除学生信息")]
        public ActionResult Delete(int id)
        {
            _icedts.DeleteUser(id);
            return RedirectToAction("Index", new { ClassID=ClassID1});
        }

        [Filter.LogFilter(Description = "审核学生信息")]
        public ActionResult Audit(int id, bool State)
        {
            _icedts.AuditUser(id, State);
            return RedirectToAction("Index", new { ClassID = ClassID1 });
        }

        public ActionResult ChangeClass(int StudID, Guid ClassID)
        {
            _icedts.ChangeUserClass(StudID, ClassID);
            return Json("1");
        }

        public ActionResult Excel(string filepath)
        {

            string Q = string.Empty;
            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=Excel 12.0;";
            
            
            
           
            DataSet ds = ExcelDataSource(connString, ExcelSheetName(connString)[0].ToString());
            if (ds.Tables[0].Columns.Count != 5)
            {
                System.IO.Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "TempExcel", true);
                return Json(new { message = "2", url = "导入失败！您的Excel文件格式不正确！" });
            }
            string Err = string.Empty;
            int count = 0;
            
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                CEDTS_User user = new CEDTS_User();

                user.UserAccount = ds.Tables[0].Rows[i][0].ToString();
                user.UserNickname = ds.Tables[0].Rows[i][1].ToString();
                user.Sex = ds.Tables[0].Rows[i][2].ToString();
                user.Email = ds.Tables[0].Rows[i][3].ToString();
                user.Phone = ds.Tables[0].Rows[i][4].ToString();

                if (user.UserAccount == "" || user.UserAccount == null || user.Email == "" || user.Email == null || user.Sex == "" || user.Sex == null || user.UserNickname == "" || user.UserNickname == null)
                {
                    Err += (i + 2) + ",";
                    Q += "不能导入空信息" + ",";
                    continue;
                }
                if (_icedts.SelectUserByAccout(user.UserAccount) != null || _icedts.SelectUserByEmail(user.Email) != null)
                {
                    Err += (i + 2) + ",";
                    Q += "帐号或邮箱已存在" + ",";
                    continue;
                }
                if (!Regex.IsMatch(user.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    Err += (i + 2) + ",";
                    Q += "邮箱格式不正确" + ",";
                    continue;
                }
                if (user.Phone != null || user.Phone != "")
                {
                    if (!Regex.IsMatch(user.Phone, @"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)"))
                    {
                        Err += (i + 2) + ",";
                        Q += "联系电话格式不正确" + ",";
                        continue;
                    }
                }

                user.UserPassword = _icedts.HashPassword("123123");
                user.Role = "普通用户";
                user.RegisterTime = DateTime.Now;
                user.PartnerID = PartnerID;
                user.MajorID = MajorID;
                user.GradeID = GradeID;
                user.ClassID = ClassID1;
                user.StudentNumber = user.UserAccount;
                user.State = true;

                _icedts.SaveUser(user);
                count++;
            }

            System.IO.Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "TempExcel", true);
            return Json(new { message = "1", url = Err + count.ToString(), why = Q });

        }

        //获取Excel中的信息
        public DataSet ExcelDataSource(string strConn, string sheetname)
        {
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "]", strConn);
            DataSet ds = new DataSet();
            oada.Fill(ds);
            conn.Close();
            return ds;
        }

        //获得Excel中的所有sheetname。
        public ArrayList ExcelSheetName(string strConn)
        {
            ArrayList al = new ArrayList();
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataTable sheetNames = conn.GetOleDbSchemaTable
            (System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            conn.Close();
            foreach (DataRow dr in sheetNames.Rows)
            {
                al.Add(dr[2]);
            }
            return al;
        }


    }
}
