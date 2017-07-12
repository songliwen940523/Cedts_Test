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
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Controllers
{
    [Authorize(Roles = "管理员,超级管理员")]
    public class PartnerController : Controller
    {

        ICedts_PartnerRepository _partner;
        ICedts_UserRepository _icedts;
        ICedts_PaperRepository _paper;
        public PartnerController(ICedts_PartnerRepository p, ICedts_UserRepository r, ICedts_PaperRepository pa)
        {
            _partner = p;
            _icedts = r;
            _paper = pa;
        }

        public static Guid UserPartnerID = Guid.Empty;
        static public string BeginTime;
        static public string EndTime;
        static public string ActionName;
        static public Guid GradeID;
        static public string GradeName;

        #region 院校管理

        public ActionResult Index(int? id)
        {
            return View(_partner.GetPartner(id));
        }

        public ActionResult Create()
        {
            var pList = _partner.GetAdmin();
            if (pList.Count > 0)
                ViewData["AdminAccount"] = new SelectList(pList, "UserAccount", "UserAccount");
            else
                ViewData["AdminAccount"] = new List<SelectListItem>() { new SelectListItem { Value = "0", Text = "请选择" } };
            return View();
        }

        public JsonResult AjaxPartner(string PartnerName, Guid? PartnerID)
        {
            return Json(_partner.CheckPartnerName(PartnerName, PartnerID));
        }

        public JsonResult AjaxEmail(string Email, Guid? PartnerID)
        {
            return Json(_partner.CheckPartnerEmail(Email, PartnerID));
        }

        [HttpPost]
        public ActionResult Create(CEDTS_Partner Partner)
        {
            _partner.SavePartner(Partner);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid id)
        {
            CEDTS_Partner partner = _partner.GetPartnerbyID(id);
            var userList = _partner.GetAdmin();
            if (partner.AdminAccount != "" && !userList.Select(p => p.UserAccount).Contains(partner.AdminAccount))
            {
                userList.Add(_partner.GetUserbyAccount(partner.AdminAccount));
                ViewData["AdminAccount"] = new SelectList(userList, "UserAccount", "UserAccount", partner.AdminAccount);
            }
            else
            {
                if (userList.Count > 0)
                    ViewData["AdminAccount"] = new SelectList(userList, "UserAccount", "UserAccount");
                else
                    ViewData["AdminAccount"] = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "请选择" } };
            }
            return View(partner);
        }

        [HttpPost]
        public ActionResult Edit(CEDTS_Partner Partner)
        {
            _partner.ChangePartner(Partner);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            var userList = _partner.GetAdminAndTheacher(id);
            foreach (var user in userList)
            {
                _partner.DeleteUser(user);
            }

            var MajorIDList = _partner.GetMajorIDList(id);
            foreach (var m in MajorIDList)
            {
                var GradeIDList = _partner.GetGradeIDList(m);
                foreach (var g in GradeIDList)
                {
                    var ClassIDList = _partner.GetClassIDList(g);
                    foreach (var c in ClassIDList)
                    {
                        _partner.DeleteClass(c);
                    }
                    _partner.DeleteGrade(g);
                }
                _partner.DeleteMajor(m);
            }
            _partner.DeletePartner(id);
            return RedirectToAction("Index");
        }

        #endregion


        #region 教师管理

        public ActionResult Teacher(int? id)
        {
            int defaultPageSize = 10;
            List<TeacherInfo> tList = new List<TeacherInfo>();
            string UserAccount = User.Identity.Name;
            Guid PartnerID = _partner.GetPartnerIDbyUserAccount(UserAccount);
            UserPartnerID = PartnerID;
            if (PartnerID != Guid.Empty)
            {
                List<CEDTS_User> TeacherList = _partner.GetTeacherbyPartnerID(PartnerID);
                for (int i = 0; i < TeacherList.Count; i++)
                {
                    TeacherInfo t = new TeacherInfo();
                    t.UserID = TeacherList[i].UserID;
                    t.UserAccount = TeacherList[i].UserAccount;
                    t.UserNickname = TeacherList[i].UserNickname;
                    t.Phone = TeacherList[i].Phone;
                    t.Role = TeacherList[i].Role;
                    t.Sex = TeacherList[i].Sex;
                    t.Email = TeacherList[i].Email;
                    t.Partner = _partner.GetPartnerbyID(PartnerID).PartnerName;
                    if (TeacherList[i].MajorID == null)
                        t.Major = null;
                    else
                        t.Major = _partner.GetMajorbyID(TeacherList[i].MajorID.Value).MajorName;
                    if (TeacherList[i].GradeID == null)
                        t.Grade = null;
                    else
                        t.Grade = _partner.GetGradebyID(TeacherList[i].GradeID.Value).GradeName;
                    if (TeacherList[i].ClassID == null)
                        t.Class = null;
                    else
                        t.Class = _partner.GetClassbyID(TeacherList[i].ClassID.Value).ClassName;
                    tList.Add(t);
                }
            }
            IQueryable<TeacherInfo> teacheList = tList.AsQueryable();

            return View(teacheList.ToPagedList(id ?? 1, defaultPageSize));
        }

        public ActionResult CreateTeacher()
        {
            List<SelectListItem> Sex = new List<SelectListItem>();
            Sex.Add(new SelectListItem { Text = "男", Value = "男" });
            Sex.Add(new SelectListItem { Text = "女", Value = "女" });
            ViewData["Sex"] = Sex;

            //Guid PartnerID = UserPartnerID;

            //var major1 = _partner.GetMajorbyLevel(1, PartnerID);
            //if (major1.Count > 0)
            //{
            //    ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName");
            //    var major2 = _partner.GetMajorbyUpID(major1[0].MajorID);
            //    if (major2.Count > 0)
            //    {
            //        ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName");
            //        var major3 = _partner.GetMajorbyUpID(major2[0].MajorID);
            //        if (major3.Count > 0)
            //        {
            //            ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName");
            //            var GradeList = _partner.GetGradeByMID(major3[0].MajorID);
            //            if (GradeList.Count > 0)
            //            {
            //                ViewData["GradeID"] = new SelectList(GradeList, "GradeID", "GradeName");
            //                var ClassList = _partner.GetClassByGID(GradeList[0].GradeID);
            //                if (ClassList.Count > 0)
            //                {
            //                    ViewData["ClassID"] = new SelectList(ClassList, "ClassID", "ClassName");
            //                }
            //                else
            //                {
            //                    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //                }
            //            }
            //            else
            //            {
            //                ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //                ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            }
            //        }
            //        else
            //        {
            //            ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        }
            //    }
            //    else
            //    {
            //        ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    }
            //}
            //else
            //{
            //    ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //}
            return View();
        }

        public JsonResult GetMajorByPartnerID(Guid PartnerID)
        {
            var majorList = _partner.GetMajorByPID(PartnerID);
            List<Major> mList = new List<Major>();
            for (int i = 0; i < majorList.Count; i++)
            {
                Major m = new Major();
                m.MajorID = majorList[i].MajorID;
                m.MajorName = majorList[i].MajorName;
                mList.Add(m);
            }
            if (majorList.Count > 0)
                return Json(mList);
            else
                return Json("");
        }

        public JsonResult GetMajor(Guid MajorID)
        {
            var majorList = _partner.GetMajorbyUpID(MajorID);
            List<Major> mList = new List<Major>();
            for (int i = 0; i < majorList.Count; i++)
            {
                Major m = new Major();
                m.MajorID = majorList[i].MajorID;
                m.MajorName = majorList[i].MajorName;
                mList.Add(m);
            }
            if (majorList.Count > 0)
                return Json(mList);
            else
                return Json("");
        }

        public JsonResult GetGrade(Guid MajorID)
        {
            if (MajorID == null)
                return Json("");
            else
            {
                var gradeList = _partner.GetGradeByMID(MajorID);
                List<Grade> gList = new List<Grade>();
                for (int i = 0; i < gradeList.Count; i++)
                {
                    Grade g = new Grade();
                    g.GradeID = gradeList[i].GradeID;
                    g.GradeName = gradeList[i].GradeName;
                    gList.Add(g);
                }
                return Json(gList);
            }
        }

        public JsonResult GetClass(Guid GradeID)
        {
            if (GradeID == null)
                return Json("");
            else
            {
                var classList = _partner.GetClassByGID(GradeID);
                List<Class> cList = new List<Class>();
                for (int i = 0; i < classList.Count; i++)
                {
                    Class c = new Class();
                    c.ClassID = classList[i].ClassID;
                    c.ClassName = classList[i].ClassName;
                    cList.Add(c);
                }
                return Json(cList);
            }
        }

        [HttpPost]
        public ActionResult CreateTeacher(CEDTS_User Teacher)
        {
            Teacher.Role = "教师";
            Teacher.RegisterTime = DateTime.Now;
            string UserAccount = User.Identity.Name;
            Guid PartnerID = _partner.GetPartnerIDbyUserAccount(UserAccount);
            Teacher.PartnerID = PartnerID;
            Teacher.UserPassword = _partner.HashPassword(Teacher.UserPassword);
            _partner.SaveTeacher(Teacher);
            return RedirectToAction("Teacher");
        }

        public ActionResult EditTeacher(int id)
        {
            var Teacher = _partner.GetTeacherbyID(id);
            List<SelectListItem> Sex = new List<SelectListItem>();
            Sex.Add(new SelectListItem { Text = "男", Value = "男" });
            Sex.Add(new SelectListItem { Text = "女", Value = "女" });
            if (Teacher.Sex == "男")
                Sex[0].Selected = true;
            else
                Sex[1].Selected = true;
            ViewData["Sex"] = Sex;

            //Guid PartnerID = UserPartnerID;
            //var m1 = _partner.GetMajorbyID(Teacher.MajorID.Value).UpMajorID;
            //var m2 = _partner.GetMajorbyID(m1).UpMajorID;

            //var major1 = _partner.GetMajorbyLevel(1, PartnerID);

            //if (major1.Count > 0)
            //{
            //    ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName", m2);
            //    var major2 = _partner.GetMajorbyUpID(m2);
            //    if (major2.Count > 0)
            //    {
            //        ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName", m1);
            //        var major3 = _partner.GetMajorbyUpID(m1);
            //        if (major3.Count > 0)
            //        {
            //            ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName", Teacher.MajorID);
            //            var GradeList = _partner.GetGradeByMID(Teacher.MajorID.Value);
            //            if (GradeList.Count > 0)
            //            {
            //                ViewData["GradeID"] = new SelectList(GradeList, "GradeID", "GradeName", Teacher.GradeID);
            //                var ClassList = _partner.GetClassByGID(Teacher.GradeID.Value);
            //                if (ClassList.Count > 0)
            //                {
            //                    ViewData["ClassID"] = new SelectList(ClassList, "ClassID", "ClassName", Teacher.ClassID);
            //                }
            //                else
            //                {
            //                    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //                }
            //            }
            //            else
            //            {
            //                ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //                ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            }
            //        }
            //        else
            //        {
            //            ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        }
            //    }
            //    else
            //    {
            //        ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    }
            //}
            //else
            //{
            //    ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //}
            return View(Teacher);
        }



        [HttpPost]
        public ActionResult EditTeacher(CEDTS_User Teacher)
        {
            if (Teacher.UserPassword != null)
            {
                Teacher.UserPassword = _partner.HashPassword(Teacher.UserPassword);
            }
            _partner.ChangeTeacher(Teacher);
            return RedirectToAction("Teacher");
        }

        public ActionResult DeleteTeacher(int id)
        {
            _partner.DeleteUser(id);
            return RedirectToAction("Teacher");
        }

        #endregion


        #region 专业管理

        public ActionResult Major()
        {
            return View();
        }

        public ActionResult Major1(int? id)
        {
            if (id == null)
            {
                if (TempData["id1"] != null)
                {
                    id = int.Parse(TempData["id1"].ToString());
                    TempData["id1"] = id;
                }
                var MajorID = _partner.GetMajorbyName("年级1", UserPartnerID);
                List<Major> majorlist = new List<Major>();
                for (int i = 0; i < MajorID.Count; i++)
                {
                    Major m = new Major();
                    m.MajorID = MajorID[i].MajorID;
                    majorlist.Add(m);
                }
                return PartialView(_partner.NewGetMajorbyLevel(1, majorlist[0].MajorID, UserPartnerID).AsQueryable().ToPagedList(id ?? 1, 5));
            }
            else
            {
                TempData["id1"] = id;
                return RedirectToAction("Major");
            }
        }

        public ActionResult Major2(int? id)
        {
            if (id == null)
            {
                if (TempData["id2"] != null)
                {
                    id = int.Parse(TempData["id2"].ToString());
                    TempData["id2"] = id;
                }
                var MajorID = _partner.GetMajorbyName("年级2", UserPartnerID);
                List<Major> majorlist = new List<Major>();
                for (int i = 0; i < MajorID.Count; i++)
                {
                    Major m = new Major();
                    m.MajorID = MajorID[i].MajorID;
                    majorlist.Add(m);
                }
                var major = _partner.NewGetMajorbyLevel(2, majorlist[0].MajorID, UserPartnerID);
                List<Major> MList = new List<Major>();
                for (int i = 0; i < major.Count; i++)
                {
                    Major m = new Major();
                    m.MajorID = major[i].MajorID;
                    m.MajorMark = major[i].MajorMark;
                    m.MajorName = major[i].MajorName;
                    m.UpMajorName = _partner.GetMajorbyID(major[i].UpMajorID).MajorName;
                    MList.Add(m);
                }
                return PartialView(MList.AsQueryable().ToPagedList(id ?? 1, 5));
            }
            else
            {
                TempData["id2"] = id;
                return RedirectToAction("Major");
            }
        }

        public ActionResult Major3(int? id)
        {
            if (id == null)
            {
                if (TempData["id3"] != null)
                {
                    id = int.Parse(TempData["id3"].ToString());
                    TempData["id3"] = id;
                }
                var MajorID = _partner.GetMajorbyName("年级3", UserPartnerID);
                List<Major> majorlist = new List<Major>();
                for (int i = 0; i < MajorID.Count; i++)
                {
                    Major m = new Major();
                    m.MajorID = MajorID[i].MajorID;
                    majorlist.Add(m);
                }
                var major = _partner.NewGetMajorbyLevel(3, majorlist[0].MajorID, UserPartnerID);
                List<Major> MList = new List<Major>();
                for (int i = 0; i < major.Count; i++)
                {
                    Major m = new Major();
                    m.MajorID = major[i].MajorID;
                    m.MajorMark = major[i].MajorMark;
                    m.MajorName = major[i].MajorName;
                    m.UpMajorName = _partner.GetMajorbyID(major[i].UpMajorID).MajorName;
                    MList.Add(m);
                }
                return PartialView(MList.AsQueryable().ToPagedList(id ?? 1, 5));
            }
            else
            {
                TempData["id3"] = id;
                return RedirectToAction("Major");
            }
        }

        public ActionResult CreateMajor()
        {
            return View();
        }

        public JsonResult AjaxMajorName(string MajorName, Guid? MajorID)
        {
            return Json(_partner.CheckMajorName(MajorName, MajorID, UserPartnerID));
        }

        public JsonResult GetMajorbyLevel(int level)
        {
            level--;
            var Major = _partner.GetMajorbyLevel(level, UserPartnerID);
            List<Major> mList = new List<Major>();
            for (int i = 0; i < Major.Count; i++)
            {
                Major m = new Major();
                m.MajorID = Major[i].MajorID;
                m.MajorName = Major[i].MajorName;
                mList.Add(m);
            }
            if (Major.Count > 0)
                return Json(mList);
            else
                return Json("");
        }

        [HttpPost]
        public ActionResult CreateMajor(CEDTS_Major Major)
        {
            Major.MajorID = Guid.NewGuid();
            if (Major.MajorLevel == 1)
                Major.UpMajorID = Major.MajorID;
            Major.PartnerID = UserPartnerID;
            Major.CreateTime = DateTime.Now;
            _partner.SaveMajor(Major);
            return RedirectToAction("Major");
        }

        public ActionResult EditMajor(Guid id)
        {
            var Major = _partner.GetMajorbyID(id);
            TempData["level"] = Major.MajorLevel;
            if (Major.MajorLevel == 1)
            {
                ViewData["UpMajorID"] = new List<SelectListItem>() { new SelectListItem { Value = Major.MajorID.ToString(), Text = Major.MajorName } };
            }
            else
                ViewData["UpMajorID"] = new SelectList(_partner.GetMajorbyLevel(Major.MajorLevel - 1, UserPartnerID), "MajorID", "MajorName", Major.UpMajorID);
            return View(Major);
        }

        [HttpPost]
        public ActionResult EditMajor(CEDTS_Major Major)
        {
            Major.MajorLevel = int.Parse(TempData["level"].ToString());
            Major.PartnerID = UserPartnerID;
            _partner.ChangeMajor(Major);
            return RedirectToAction("Major");
        }

        public ActionResult DeleteMajor(Guid id)
        {
            var major = _partner.GetMajorbyID(id);
            int level = major.MajorLevel;
            if (level == 1)
            {
                var mid = major.MajorID;
                var majorList2 = _partner.GetMajorbyUpID(mid);
                foreach (var m2 in majorList2)
                {
                    var mid2 = m2.MajorID;
                    var majorList3 = _partner.GetMajorbyUpID(mid2);
                    foreach (var m3 in majorList3)
                    {
                        var mid3 = m3.MajorID;
                        var GradeIDList = _partner.GetGradeIDList(mid3);
                        foreach (var g in GradeIDList)
                        {
                            var ClassIDList = _partner.GetClassIDList(g);
                            foreach (var c in ClassIDList)
                            {
                                _partner.DeleteClass(c);
                            }
                            _partner.DeleteGrade(g);
                        }
                    }
                }
            }
            if (level == 2)
            {
                var mid2 = major.MajorID;
                var majorList3 = _partner.GetMajorbyUpID(mid2);
                foreach (var m3 in majorList3)
                {
                    var mid3 = m3.MajorID;
                    var GradeIDList = _partner.GetGradeIDList(mid3);
                    foreach (var g in GradeIDList)
                    {
                        var ClassIDList = _partner.GetClassIDList(g);
                        foreach (var c in ClassIDList)
                        {
                            _partner.DeleteClass(c);
                        }
                        _partner.DeleteGrade(g);
                    }
                }
            }
            if (level == 3)
            {
                var mid3 = major.MajorID;
                var GradeIDList = _partner.GetGradeIDList(mid3);
                foreach (var g in GradeIDList)
                {
                    var ClassIDList = _partner.GetClassIDList(g);
                    foreach (var c in ClassIDList)
                    {
                        _partner.DeleteClass(c);
                    }
                    _partner.DeleteGrade(g);
                }
            }

            _partner.DeleteMajor(id);
            return RedirectToAction("Major");
        }

        #endregion


        #region 年级管理

        public ActionResult Grade(int? id)
        {
            var MajorID = _partner.GetMajorbyName("年级3", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            for (int i = 0; i < MajorID.Count; i++)
            {
                Major m = new Major();
                m.MajorID = MajorID[i].MajorID;
                majorlist.Add(m);
            }
            var Grade = _partner.GetGrade(UserPartnerID, majorlist[0].MajorID);
            List<Grade> gradeList = new List<Grade>();
            for (int i = 0; i < Grade.Count; i++)
            {
                Grade g = new Grade();
                g.GradeID = Grade[i].GradeID;
                g.GradeName = Grade[i].GradeName;
                g.MajorName = _partner.GetMajorbyID(Grade[i].MajorID).MajorName;
                gradeList.Add(g);

            }
            return View(gradeList.AsQueryable().ToPagedList(id ?? 1, 5));
        }

        public ActionResult TGrade(int? id)
        {
            var MajorID = _partner.GetMajorbyName("年级3", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            if (MajorID.Count==0) 
            {
                List<Guid> flag = new List<Guid>();
                for (int i = 1; i < 4;i++ )
                {
                    CEDTS_Major Major = new CEDTS_Major();
                    Major.MajorID = Guid.NewGuid();
                    flag.Add(Major.MajorID);
                    if (i == 1)
                    {
                        Major.UpMajorID = Major.MajorID;
                    }
                    else
                    {
                        Major.UpMajorID = flag[i-2];
                    }
                    Major.MajorLevel = i;
                    Major.MajorName = "年级" + i;
                    Major.PartnerID = UserPartnerID;
                    Major.CreateTime = DateTime.Now;
                    _partner.SaveMajor(Major);
                }
                return RedirectToAction("TGrade");

            }
            else{
                for (int i = 0; i < MajorID.Count; i++)
                {
                    Major m = new Major();
                    m.MajorID = MajorID[i].MajorID;
                    majorlist.Add(m);
                }
                var Grade = _partner.GetTGrade(UserPartnerID, majorlist[0].MajorID);
                List<Grade> gradeList = new List<Grade>();
                for (int i = 0; i < Grade.Count; i++)
                {
                    Grade g = new Grade();
                    g.GradeID = Grade[i].GradeID;
                    g.GradeName = Grade[i].GradeName;
                    gradeList.Add(g);
                }
                return View(gradeList.AsQueryable().ToPagedList(id ?? 1, 5));
            }

        }

        public ActionResult CreateGrade()
        {
            var MajorID = _partner.GetMajorbyName("年级1", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            for (int i = 0; i < MajorID.Count; i++)
            {
                Major m = new Major();
                m.MajorID = MajorID[i].MajorID;
                majorlist.Add(m);
            }
            Guid PartnerID = UserPartnerID;
            var major1 = _partner.NewGetMajorbyLevel(1, majorlist[0].MajorID, PartnerID);
            if (major1.Count > 0)
            {
                ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName");
                var major2 = _partner.GetMajorbyUpID(major1[0].MajorID);
                if (major2.Count > 0)
                {
                    ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName");
                    var major3 = _partner.GetMajorbyUpID(major2[0].MajorID);
                    if (major3.Count > 0)
                    {
                        ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName");
                    }
                    else
                    {
                        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    }
                }
                else
                {
                    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                }
            }
            else
            {
                ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            }
            return View();
        }

        public JsonResult AjaxGradeName(string GradeName, Guid? MajorID, Guid? GradeID)
        {
            return Json(_partner.CheckGradeName(GradeName, MajorID, GradeID));
        }

        [HttpPost]
        public ActionResult CreateGrade(CEDTS_Grade Grade)
        {
            Grade.GradeID = Guid.NewGuid();
            Grade.PartnerID = UserPartnerID;
            Grade.CreateTime = DateTime.Now;
            _partner.SaveGrade(Grade);
            return RedirectToAction("Grade");
        }

        public ActionResult CreateTGrade()
        {
            Guid PartnerID = UserPartnerID;
            var MajorID = _partner.GetMajorbyName("年级1", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            for (int i = 0; i < MajorID.Count; i++)
            {
                Major m = new Major();
                m.MajorID = MajorID[i].MajorID;
                majorlist.Add(m);
            }
            var major1 = _partner.GetMajorforTGrade(majorlist[0].MajorID, PartnerID);
            if (major1.Count > 0)
            {
                ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName");
                var major2 = _partner.GetMajorbyUpID(major1[0].MajorID);
                if (major2.Count > 0)
                {
                    ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName");
                    var major3 = _partner.GetMajorbyUpID(major2[0].MajorID);
                    if (major3.Count > 0)
                    {
                        ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName");
                    }
                    else
                    {
                        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    }
                }
                else
                {
                    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                }
            }
            else
            {
                ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateTGrade(CEDTS_Grade Grade)
        {
            Grade.GradeID = Guid.NewGuid();
            Grade.PartnerID = UserPartnerID;
            Grade.CreateTime = DateTime.Now;
            _partner.SaveGrade(Grade);
            return RedirectToAction("TGrade");
        }
        public ActionResult EditGrade(Guid id)
        {
            var MajorID = _partner.GetMajorbyName("年级1", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            for (int i = 0; i < MajorID.Count; i++)
            {
                Major m = new Major();
                m.MajorID = MajorID[i].MajorID;
                majorlist.Add(m);
            }
            var Grade = _partner.GetGradebyID(id);
            var major = _partner.GetMajorbyID(Grade.MajorID);
            var upid2 = major.UpMajorID;
            var major_2 = _partner.GetMajorbyID(upid2);
            var upid1 = major_2.UpMajorID;
            var major1 = _partner.NewGetMajorbyLevel(1, majorlist[0].MajorID, UserPartnerID);
            if (major1.Count > 0)
            {
                ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName", upid1);
                var major2 = _partner.GetMajorbyUpID(upid1);
                if (major2.Count > 0)
                {
                    ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName", upid2);
                    var major3 = _partner.GetMajorbyUpID(upid2);
                    if (major3.Count > 0)
                    {
                        ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName", Grade.MajorID);
                    }
                    else
                    {
                        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    }
                }
                else
                {
                    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                }
            }
            else
            {
                ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            }
            return View(Grade);
        }

        [HttpPost]
        public ActionResult EditGrade(CEDTS_Grade Grade)
        {
            _partner.ChangeGrade(Grade);
            return RedirectToAction("Grade");
        }

        public ActionResult EditTGrade(Guid id)
        {
            var MajorID = _partner.GetMajorbyName("年级1", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            for (int i = 0; i < MajorID.Count; i++)
            {
                Major m = new Major();
                m.MajorID = MajorID[i].MajorID;
                majorlist.Add(m);
            }
            var Grade = _partner.GetGradebyID(id);
            var major = _partner.GetMajorbyID(Grade.MajorID);
            var upid2 = major.UpMajorID;
            var major_2 = _partner.GetMajorbyID(upid2);
            var upid1 = major_2.UpMajorID;
            var major1 = _partner.GetMajorforTGrade(majorlist[0].MajorID, UserPartnerID);
            if (major1.Count > 0)
            {
                ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName", upid1);
                var major2 = _partner.GetMajorbyUpID(upid1);
                if (major2.Count > 0)
                {
                    ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName", upid2);
                    var major3 = _partner.GetMajorbyUpID(upid2);
                    if (major3.Count > 0)
                    {
                        ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName", Grade.MajorID);
                    }
                    else
                    {
                        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    }
                }
                else
                {
                    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                }
            }
            else
            {
                ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            }
            return View(Grade);
        }

        [HttpPost]
        public ActionResult EditTGrade(CEDTS_Grade Grade)
        {
            _partner.ChangeGrade(Grade);
            return RedirectToAction("TGrade");
        }

        public ActionResult DeleteGrade(Guid id)
        {
            var ClassIDList = _partner.GetClassIDList(id);
            foreach (var c in ClassIDList)
            {
                _partner.DeleteClass(c);
            }
            _partner.DeleteGrade(id);
            return RedirectToAction("Grade");
        }

        public ActionResult DeleteTGrade(Guid id)
        {
            var ClassIDList = _partner.GetClassIDList(id);
            foreach (var c in ClassIDList)
            {
                _partner.DeleteClass(c);
            }
            _partner.DeleteGrade(id);
            return RedirectToAction("TGrade");
        }

        #endregion


        #region 班级管理

        public ActionResult Class(int? id)
        {
            var cList = _partner.GetClass(UserPartnerID);
            List<Class> cl = new List<Class>();
            for (int i = 0; i < cList.Count; i++)
            {
                Class c = new Class();
                c.ClassID = cList[i].ClassID;
                c.ClassName = cList[i].ClassName;
                cl.Add(c);
            }
            return View(cl.AsQueryable().ToPagedList(id ?? 1, 5));
        }

        public ActionResult CreateClass()
        {
            var teacherList = _partner.GetTeacherbyPartnerID(UserPartnerID);
            if (teacherList.Count > 0)
            {
                ViewData["UserID"] = new SelectList(teacherList, "UserID", "UserNickname");
            }
            else
            {
                ViewData["UserID"] = new List<SelectListItem>() { new SelectListItem { Value = Guid.Empty.ToString(), Text = "请选择" } };
            }
            var MajorID = _partner.GetMajorbyName("年级3", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            for (int i = 0; i < MajorID.Count; i++)
            {
                Major m = new Major();
                m.MajorID = MajorID[i].MajorID;
                majorlist.Add(m);
            }
            var TGradeList = _partner.GetTGrade(UserPartnerID, majorlist[0].MajorID);
            if (TGradeList.Count > 0)
            {
                ViewData["GradeID"] = new SelectList(TGradeList, "GradeID", "GradeName");
            }
            else
            {
                ViewData["GradeID"] = new List<SelectListItem>() { new SelectListItem { Value = Guid.Empty.ToString(), Text = "请选择" } };
            }
            return View();
        }

        public JsonResult AjaxClass(string ClassName, Guid? ClassID)
        {
            return Json(_partner.CheckClassName(ClassName, ClassID));
        }
        public ActionResult EditStudent(int id)
        {
            var Student = _partner.GetTeacherbyID(id);
            List<SelectListItem> Sex = new List<SelectListItem>();
            Sex.Add(new SelectListItem { Text = "男", Value = "男" });
            Sex.Add(new SelectListItem { Text = "女", Value = "女" });
            if (Student.Sex == "男")
                Sex[0].Selected = true;
            else
                Sex[1].Selected = true;
            ViewData["Sex"] = Sex;

            //Guid PartnerID = UserPartnerID;
            //var m1 = _partner.GetMajorbyID(Teacher.MajorID.Value).UpMajorID;
            //var m2 = _partner.GetMajorbyID(m1).UpMajorID;

            //var major1 = _partner.GetMajorbyLevel(1, PartnerID);

            //if (major1.Count > 0)
            //{
            //    ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName", m2);
            //    var major2 = _partner.GetMajorbyUpID(m2);
            //    if (major2.Count > 0)
            //    {
            //        ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName", m1);
            //        var major3 = _partner.GetMajorbyUpID(m1);
            //        if (major3.Count > 0)
            //        {
            //            ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName", Teacher.MajorID);
            //            var GradeList = _partner.GetGradeByMID(Teacher.MajorID.Value);
            //            if (GradeList.Count > 0)
            //            {
            //                ViewData["GradeID"] = new SelectList(GradeList, "GradeID", "GradeName", Teacher.GradeID);
            //                var ClassList = _partner.GetClassByGID(Teacher.GradeID.Value);
            //                if (ClassList.Count > 0)
            //                {
            //                    ViewData["ClassID"] = new SelectList(ClassList, "ClassID", "ClassName", Teacher.ClassID);
            //                }
            //                else
            //                {
            //                    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //                }
            //            }
            //            else
            //            {
            //                ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //                ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            }
            //        }
            //        else
            //        {
            //            ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //            ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        }
            //    }
            //    else
            //    {
            //        ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //        ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    }
            //}
            //else
            //{
            //    ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            //}
            return View(Student);
        }

        [HttpPost]
        public ActionResult CreateClass(CEDTS_Class Class)
        {
            Class.ClassID = Guid.NewGuid();
            Class.PartnerID = UserPartnerID;
            Class.CreateTime = DateTime.Now;
            _partner.SaveClass(Class);
            return RedirectToAction("Class");
        }

        public ActionResult StudentList(int? id, Guid classid)
        {
            CEDTS_Class class1 = _icedts.GetClassbyID(classid);            
            ViewData["Tcount"] = _icedts.SelectAllUser(classid);
            ViewData["Fcount"] = _icedts.SelectPassUser(classid);
            ViewData["ClassID"] = classid;
            ViewData["Class"] = _icedts.GetClassbyID(classid).ClassName;
            return View(_icedts.SelectUser(id, classid));
        }

        public ActionResult EditClass(Guid id)
        {
            var MajorID = _partner.GetMajorbyName("年级3", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            for (int i = 0; i < MajorID.Count; i++)
            {
                Major m = new Major();
                m.MajorID = MajorID[i].MajorID;
                majorlist.Add(m);
            }
            var Class = _partner.GetClassbyID(id);
            var gradeList = _partner.GetTGrade(UserPartnerID, majorlist[0].MajorID);
            ViewData["GradeID"] = new SelectList(gradeList, "GradeID", "GradeName", Class.GradeID);
            var teacherList=_partner.GetTeacherbyPartnerID(UserPartnerID);
            ViewData["UserID"] = new SelectList(teacherList, "UserID", "UserNickname", Class.UserID);
            return View(Class);
        }

        [HttpPost]
        public ActionResult EditClass(CEDTS_Class Class)
        {
            _partner.ChangeClass(Class);
            return RedirectToAction("Class");
        }

        public ActionResult DeleteClass(Guid id)
        {
            _partner.DeleteClass(id);
            return RedirectToAction("Class");
        }

        public ActionResult AddStudent(Guid id)
        {
            TempData["ClassID"] = id;
            List<SelectListItem> Sex = new List<SelectListItem>();
            Sex.Add(new SelectListItem { Text = "男", Value = "男" });
            Sex.Add(new SelectListItem { Text = "女", Value = "女" });
            ViewData["Sex"] = Sex;
            return View();
        }



        [HttpPost]
        public ActionResult AddStudent(CEDTS_User User)
        {
            User.UserPassword = _icedts.HashPassword(User.UserPassword);
            User.Role = "普通用户";
            User.State = true;
            User.RegisterTime = DateTime.Now;
            User.PartnerID = UserPartnerID;
            User.ClassID = Guid.Parse(TempData["ClassID"].ToString());
            _icedts.SaveUser(User);
            return RedirectToAction("Class");
        }

        [HttpPost]
        public ActionResult EditStudent(CEDTS_User Student)
        {
            if (Student.UserPassword != null)
            {
                Student.UserPassword = _partner.HashPassword(Student.UserPassword);
            }
            _partner.ChangeTeacher(Student);
            return RedirectToAction("Class");
        }

        public ActionResult DeleteStudent(int id, Guid classid)
        {
            _icedts.DeleteUser(id);
            return RedirectToAction("StudentList", new { classid = classid });
        }

        public ActionResult Excel(string filepath, string classid)
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
                user.PartnerID = UserPartnerID;
                user.ClassID = Guid.Parse(classid);
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

        #endregion

        #region 成绩管理
        public ActionResult AdminIndex(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminIndex(FormCollection form)
        {
            BeginTime = form["CalenderBegin"];
            EndTime = form["CalenderEnd"];
            switch (form["FirstLevel"])
            {
                case "全校情况":
                    ActionName = "SchoolExercise";
                    break;
                case "年级情况":
                    ActionName = "GradeExercise";
                    break;
                default:
                    ActionName = "AdminIndex";
                    break;
            }
            return RedirectToAction(ActionName);
        }

        public ActionResult SchoolExercise(int? id)
        {
            var userList = _icedts.SelectUserByPartner(UserPartnerID);
            int testnum = 0;
            int testcnum = 0;
            int usertest = 0;
            double correctrate = 0;
            foreach (var user in userList)
            {

                usertest += _paper.CountTestByPeriod(BeginTime, EndTime, user.UserID);
                var testList = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                foreach (var test in testList)
                {
                    testnum += _paper.SelectQNByTestID(test.TestID);
                    testcnum += _paper.SelectCQNByTestID(test.TestID);
                }
               
            }
            if (testnum == 0) 
            {
                correctrate = 0; 
            }
            else {
                correctrate = testcnum * 100 / testnum; 
            }
            ViewData["TotalNum"] = usertest;
            ViewData["TotalQue"] = testnum;
            ViewData["CorQue"] = testcnum;
            ViewData["CorRate"] = correctrate;
            ViewData["StartDate"] = BeginTime;
            ViewData["EndDate"] = EndTime;
            return View();
        }

        public ActionResult GradeExercise(int? id)
        {
            List<ScoreInfo> gradelist = new List<ScoreInfo>();
            var MajorID = _partner.GetMajorbyName("年级3", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            for (int i = 0; i < MajorID.Count; i++)
            {
                Major m = new Major();
                m.MajorID = MajorID[i].MajorID;
                majorlist.Add(m);
            }
            var Grade = _partner.GetTGrade(UserPartnerID, majorlist[0].MajorID);
            foreach(var g in Grade)
            {
                ViewData["UserID"] = UserPartnerID;
                ScoreInfo eachscore = new ScoreInfo();
                eachscore.StudNum = g.GradeID.ToString();
                eachscore.StudName = g.GradeName;
                eachscore.DoneNum = 0;
                eachscore.CorrectNum = 0;
                var Class = _partner.GetClassByGID(g.GradeID);
                foreach (var c in Class)
                {
                    var stuList = _icedts.SelectUserByClassID(c.ClassID);
                    foreach (var user in stuList)
                    {
                        var testList = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                        foreach (var test in testList)
                        {
                            eachscore.DoneNum += _paper.SelectQNByTestID(test.TestID);
                            eachscore.CorrectNum += _paper.SelectCQNByTestID(test.TestID);
                        }

                    }
                }
                if (eachscore.DoneNum == 0)
                {
                    eachscore.AveScore = 0;
                }
                else 
                {
                    eachscore.AveScore = eachscore.CorrectNum * 100 / eachscore.DoneNum;
                }
                gradelist.Add(eachscore);
            }
            ViewData["StartDate"] = BeginTime;
            ViewData["EndDate"] = EndTime; 
            return View(_paper.ScoreInfoPaged(id, gradelist));
        }

        public ActionResult ClassExercise(string id)
        {
            Guid gradeID = new Guid(id.ToString());
            List<ScoreInfo> classlist = new List<ScoreInfo>();
            var Class = _partner.GetClassByGID(gradeID);
            foreach (var c in Class)
            {
                ScoreInfo eachscore = new ScoreInfo();
                eachscore.StudNum = c.ClassID.ToString();
                eachscore.StudName = c.ClassName;
                eachscore.DoneNum = 0;
                eachscore.CorrectNum = 0;

                var stuList = _icedts.SelectUserByClassID(c.ClassID);
                foreach (var user in stuList)
                {
                    var testList = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                    foreach (var test in testList)
                    {
                        eachscore.DoneNum += _paper.SelectQNByTestID(test.TestID);
                        eachscore.CorrectNum += _paper.SelectCQNByTestID(test.TestID);
                    }


                }
                if (eachscore.DoneNum == 0)
                {
                    eachscore.AveScore = 0;
                }
                else
                {
                    eachscore.AveScore = eachscore.CorrectNum * 100 / eachscore.DoneNum;
                }
                classlist.Add(eachscore);
            }
            ViewData["StartDate"] = BeginTime;
            ViewData["EndDate"] = EndTime;
            int intA = 0;
            int.TryParse(id, out intA);
            return View(_paper.ScoreInfoPaged(intA, classlist));
        }

        public ActionResult DetailExercise(string id)
        {
            Guid ClassID = new Guid(id.ToString());
            List<ScoreInfo> scorelist = new List<ScoreInfo>();
            int UserID = _icedts.SelectUserByAccout(User.Identity.Name).UserID;
            List<Guid?> paclist = _paper.SelectPaperByPeriod(BeginTime, EndTime, UserID, ClassID);
            List<CEDTS_User> userlist = _icedts.SelectUserByClassID(ClassID);
            foreach (var user in userlist)
            {
                ScoreInfo eachscore = new ScoreInfo();
                eachscore.UserID = user.UserID;
                eachscore.StudNum = user.StudentNumber;
                eachscore.StudName = user.UserNickname;
                eachscore.TestNames = new List<string>();
                eachscore.TestScore = new List<double>();
                List<CEDTS_Test> usertest = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                foreach (var pac in paclist)
                {
                    CEDTS_Paper paper = _paper.GetPaperByID(Guid.Parse(pac.ToString()));
                    eachscore.TestNames.Add(paper.Title);
                    CEDTS_Test test = _paper.GetTestByPaperID(Guid.Parse(pac.ToString()), user.UserID);
                    if (test != null && test.IsFinished == true)
                        eachscore.TestScore.Add(double.Parse(test.TotalScore.ToString()) / double.Parse(paper.Score.ToString()) * 100);
                    else
                        eachscore.TestScore.Add(0);
                }

                int donenum = 0;
                double userscore = 0.0;
                double totalscore = 0.0;
                foreach (var test in usertest)
                {
                    if (!paclist.Contains(test.PaperID))
                    {
                        List<CEDTS_PaperAssessment> assessment = _paper.SelectPaperAssessmentItem(test.PaperID);
                        foreach (var ass in assessment)
                        {
                            donenum += _paper.SelectQNByAssessmentID(Guid.Parse(ass.AssessmentItemID.ToString()));
                        }
                        userscore += double.Parse(test.TotalScore.ToString());
                        totalscore += double.Parse(_paper.SelectPaper(test.PaperID).Score.ToString());
                    }
                }
                eachscore.DoneNum = donenum;
                if (totalscore == 0)
                    eachscore.DoneScore = 0.0;
                else
                    eachscore.DoneScore = userscore / totalscore * 100;
                double avescore = 0.0;
                foreach (var score in eachscore.TestScore)
                {
                    avescore += score;
                }
                eachscore.AveScore = (avescore + eachscore.DoneScore) / (eachscore.TestScore.Count + 1);
                scorelist.Add(eachscore);
            }
            ViewData["StartDate"] = BeginTime;
            ViewData["EndDate"] = EndTime;
            int intA = 0;
            int.TryParse(id, out intA);
            return View(_paper.ScoreInfoPaged(intA, scorelist));
        }

        public ActionResult Single(int? id)
        {
            var MajorID = _partner.GetMajorbyName("年级3", UserPartnerID);
            List<Major> majorlist = new List<Major>();
            if (MajorID.Count == 0)
            {
                List<Guid> flag = new List<Guid>();
                for (int i = 1; i < 4; i++)
                {
                    CEDTS_Major Major = new CEDTS_Major();
                    Major.MajorID = Guid.NewGuid();
                    flag.Add(Major.MajorID);
                    if (i == 1)
                    {
                        Major.UpMajorID = Major.MajorID;
                    }
                    else
                    {
                        Major.UpMajorID = flag[i - 2];
                    }
                    Major.MajorLevel = i;
                    Major.MajorName = "年级" + i;
                    Major.PartnerID = UserPartnerID;
                    Major.CreateTime = DateTime.Now;
                    _partner.SaveMajor(Major);
                }
                return RedirectToAction("TGrade");

            }
            else
            {
                for (int i = 0; i < MajorID.Count; i++)
                {
                    Major m = new Major();
                    m.MajorID = MajorID[i].MajorID;
                    majorlist.Add(m);
                }
                List<CEDTS_Grade> Grade = _partner.GetTGrade(UserPartnerID, majorlist[0].MajorID);
                List<SelectListItem> GradeIDList = new List<SelectListItem>();
                for (int i = 0; i < Grade.Count; i++)
                {
                    GradeIDList.Add(new SelectListItem { Text = Grade[i].GradeName, Value = Grade[i].GradeID.ToString() });
                }
                List<SelectListItem> ClassIDList = new List<SelectListItem>();
                GradeIDList[0].Selected = true;
                ViewData["GradeIDList"] = GradeIDList;
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult Single(FormCollection form)
        {
            GradeID = Guid.Parse(form["GradeIDList"]);
            GradeName = _partner.GetGradebyID(GradeID).GradeName;
            BeginTime = form["CalenderBegin"];
            EndTime = form["CalenderEnd"];
            return RedirectToAction("SingleDetail");
        }

        public ActionResult SingleDetail()
        {
            GradeName = _partner.GetGradebyID(GradeID).GradeName;
            ViewData["GradeName"] = GradeName;
            ViewData["StartDate"] = BeginTime;
            ViewData["EndDate"] = EndTime;
            return View();
        }

        public ActionResult SingleItemList()
        {
            List<WrongItemInfo> WrongItemList = new List<WrongItemInfo>();
            var ItemList = _paper.ItemCNList();
            foreach (var Item in ItemList)
            {
                WrongItemInfo eachscore = new WrongItemInfo();
                int CorrectNum = 0;
                eachscore.TotalNum = 0;
                eachscore.ItemName = Item.TypeName_CN;
                eachscore.ItemTypeNum = Item.ItemTypeID;
                var Class = _partner.GetClassByGID(GradeID);
                foreach (var c in Class)
                {
                    var stuList = _icedts.SelectUserByClassID(c.ClassID);
                    foreach (var user in stuList)
                    {
                        var testList = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                        foreach (var test in testList)
                        {
                            eachscore.TotalNum += _paper.ItemTestCount(user.UserID,test.TestID, Item.ItemTypeID);
                            CorrectNum += _paper.ItemTestCorCount(user.UserID, test.TestID, Item.ItemTypeID);
                        }

                    }
                    if (eachscore.TotalNum == 0)
                    {
                        eachscore.CorrectRate = 0;
                    }
                    else
                    {
                        eachscore.CorrectRate = CorrectNum * 100 / eachscore.TotalNum;
                    }
                    
                }
                WrongItemList.Add(eachscore);
            }

            return PartialView(WrongItemList);
        }

        public ActionResult SingleKnowledgeList()
        {
            List<WrongKnowledgeInfo> WrongKnowledgeList = new List<WrongKnowledgeInfo>();
            var KnowledgeList = _paper.SelectKnowledges(2,null);
            foreach (var Item in KnowledgeList)
            {
                WrongKnowledgeInfo eachscore = new WrongKnowledgeInfo();
                int CorrectNum = 0;
                eachscore.TotalNum = 0;
                eachscore.KnowledgeName = Item.Title;
                eachscore.KnowledgeID = Item.KnowledgePointID;
                var Class = _partner.GetClassByGID(GradeID);
                foreach (var c in Class)
                {
                    var stuList = _icedts.SelectUserByClassID(c.ClassID);
                    foreach (var user in stuList)
                    {
                        var testList = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                        foreach (var test in testList)
                        {
                            eachscore.TotalNum += _paper.KnowledgeTestCount(user.UserID, test.TestID, Item.KnowledgePointID);
                            CorrectNum += _paper.KnowledgeTestCorCount(user.UserID, test.TestID, Item.KnowledgePointID);
                        }

                    }
                    if (eachscore.TotalNum == 0)
                    {
                        eachscore.CorrectRate = 0;
                    }
                    else
                    {
                        eachscore.CorrectRate = CorrectNum * 100 / eachscore.TotalNum;
                    }

                }
                WrongKnowledgeList.Add(eachscore);
            }

            return PartialView(WrongKnowledgeList);
        }


        #endregion
    }
}
