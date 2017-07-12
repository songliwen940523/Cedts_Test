using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using Cedts_Test.Models;
using System.Web.Routing;
using System.Text;
using System.Threading;
using System.Web.Security;
using System.Net;
using System.Transactions;
using System.Configuration;


namespace Cedts_Test.Controllers
{
    public class AccountController : Controller
    {
        //定义接口对象
        ICedts_UserRepository _userrepository;
        ICedts_HomeRepository _homerepostory;
        ICedts_PaperRepository _paper;
        Cedts_Test.Areas.Admin.Models.ICedts_CardRepository _card;
        Cedts_Test.Areas.Admin.Models.ICedts_LogRepository _log;
        Cedts_Test.Areas.Admin.Models.ICedts_PartnerRepository _partner;
        public AccountController(ICedts_UserRepository r, Cedts_Test.Areas.Admin.Models.ICedts_LogRepository l, Cedts_Test.Areas.Admin.Models.ICedts_PartnerRepository p, Cedts_Test.Areas.Admin.Models.ICedts_CardRepository c, ICedts_HomeRepository h, ICedts_PaperRepository paper)
        {
            _partner = p;
            _userrepository = r;
            _log = l;
            _card = c;
            _homerepostory = h;
            _paper = paper;
        }

        public static int a = 0;

        #region 用户注册

        public ActionResult Register()
        {
            var PartnerList = _userrepository.GetPartner().OrderBy(p => p.CreateTime).ToList();

            if (PartnerList.Count > 0)
            {
                CEDTS_Partner partner = new CEDTS_Partner();
                partner.PartnerID = Guid.Empty;
                partner.PartnerName = "请选择";
                PartnerList.Insert(0, partner);
                ViewData["PartnerID"] = new SelectList(PartnerList, "PartnerID", "PartnerName", Guid.Empty);
                var major1 = _partner.GetMajorbyLevel(1, PartnerList[0].PartnerID);
                if (major1.Count > 0)
                {
                    Cedts_Test.Areas.Admin.Models.CEDTS_Major major = new Cedts_Test.Areas.Admin.Models.CEDTS_Major();
                    major.MajorID = Guid.Empty;
                    major.MajorName = "请选择";
                    major1.Insert(0, major);
                    ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName", Guid.Empty);
                    var major2 = _partner.GetMajorbyUpID(major1[0].MajorID);
                    if (major2.Count > 0)
                    {
                        major2.Insert(0, major);
                        ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName");
                        var major3 = _partner.GetMajorbyUpID(major2[0].MajorID);
                        if (major3.Count > 0)
                        {
                            major3.Insert(0, major);
                            ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName");
                            var GradeList = _partner.GetGradeByMID(major3[0].MajorID);
                            if (GradeList.Count > 0)
                            {
                                Cedts_Test.Areas.Admin.Models.CEDTS_Grade grade = new Areas.Admin.Models.CEDTS_Grade();
                                grade.GradeID = Guid.Empty;
                                grade.GradeName = "请选择";
                                GradeList.Insert(0, grade);
                                ViewData["GradeID"] = new SelectList(GradeList, "GradeID", "GradeName", Guid.Empty);
                                var ClassList = _partner.GetClassByGID(GradeList[0].GradeID);
                                if (ClassList.Count > 0)
                                {
                                    Cedts_Test.Areas.Admin.Models.CEDTS_Class Class = new Areas.Admin.Models.CEDTS_Class();
                                    Class.ClassID = Guid.Empty;
                                    Class.ClassName = "请选择";
                                    ClassList.Insert(0, Class);
                                    ViewData["ClassID"] = new SelectList(ClassList, "ClassID", "ClassName", Guid.Empty);
                                }
                                else
                                {
                                    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                                }
                            }
                            else
                            {
                                ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                                ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                            }
                        }
                        else
                        {
                            ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                            ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                            ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                        }
                    }
                    else
                    {
                        ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                        ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                        ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                    }
                }
                else
                {
                    ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                }
            }
            else
            {
                ViewData["PartnerID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            }
            TempData["PartnerID"] = ViewData["PartnerID"];
            TempData["MajorID1"] = ViewData["MajorID1"];
            TempData["MajorID2"] = ViewData["MajorID2"];
            TempData["MajorID"] = ViewData["MajorID"];
            TempData["GradeID"] = ViewData["GradeID"];
            TempData["ClassID"] = ViewData["ClassID"];
            return View();
        }

        public JsonResult GetMajorByPartnerID(Guid PartnerID)
        {
            var majorList = _partner.GetMajorByPID(PartnerID).Where(p => p.MajorLevel == 1).ToList();
            List<Cedts_Test.Areas.Admin.Models.Major> mList = new List<Cedts_Test.Areas.Admin.Models.Major>();
            for (int i = 0; i < majorList.Count; i++)
            {
                Cedts_Test.Areas.Admin.Models.Major m = new Cedts_Test.Areas.Admin.Models.Major();
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
            List<Cedts_Test.Areas.Admin.Models.Major> mList = new List<Cedts_Test.Areas.Admin.Models.Major>();
            for (int i = 0; i < majorList.Count; i++)
            {
                Cedts_Test.Areas.Admin.Models.Major m = new Cedts_Test.Areas.Admin.Models.Major();
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
                List<Cedts_Test.Areas.Admin.Models.Grade> gList = new List<Cedts_Test.Areas.Admin.Models.Grade>();
                for (int i = 0; i < gradeList.Count; i++)
                {
                    Cedts_Test.Areas.Admin.Models.Grade g = new Cedts_Test.Areas.Admin.Models.Grade();
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
                List<Cedts_Test.Areas.Admin.Models.Class> cList = new List<Cedts_Test.Areas.Admin.Models.Class>();
                for (int i = 0; i < classList.Count; i++)
                {
                    Cedts_Test.Areas.Admin.Models.Class c = new Cedts_Test.Areas.Admin.Models.Class();
                    c.ClassID = classList[i].ClassID;
                    c.ClassName = classList[i].ClassName;
                    cList.Add(c);
                }
                return Json(cList);
            }
        }

        [HttpPost]
        public ActionResult Register([Bind(Exclude = "UserID,Role,RegisterTime")]CEDTS_User user, FormCollection form)
        {
            ViewData["PartnerID"] = TempData["PartnerID"];
            ViewData["MajorID1"] = TempData["MajorID1"];
            ViewData["MajorID2"] = TempData["MajorID2"];
            ViewData["MajorID"] = TempData["MajorID"];
            ViewData["GradeID"] = TempData["GradeID"];
            ViewData["ClassID"] = TempData["ClassID"];
            string images = form["txtimages"];
            TempData["PartnerID"] = ViewData["PartnerID"];
            TempData["MajorID1"] = ViewData["MajorID1"];
            TempData["MajorID2"] = ViewData["MajorID2"];
            TempData["MajorID"] = ViewData["MajorID"];
            TempData["GradeID"] = ViewData["GradeID"];
            TempData["ClassID"] = ViewData["ClassID"];
            if (images != (string)Session["ValidateCode"])
            {
                ModelState.AddModelError("txtimages", "验证码不正确");
                return View(user);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (_userrepository.AjaxCheckAccount(user.UserAccount, 0))
                    {
                        ModelState.AddModelError("Account", "您输入的帐号已存在，请更换帐号");
                        return View(user);
                    }
                    else
                    {
                        if (_userrepository.AjaxCheckEmail(user.Email, 0))
                        {
                            ModelState.AddModelError("Email", "您输入的邮箱地址已存在，请更换邮箱地址");
                            return View(user);
                        }
                        else
                        {
                            user.UserPassword = _userrepository.HashPassword(user.UserPassword);
                            _userrepository.Create(user);

                            string Action = "Account.Register";
                            string ClientIP = HttpContext.Request.UserHostAddress;
                            string Content = "用户注册";
                            int UserID = user.UserID;
                            _log.SaveLog(Action, ClientIP, Content, UserID);
                            a = 2;
                            #region 注册成功跳转
                            if (_userrepository.LogOn(user.UserAccount, user.UserPassword))
                            {

                                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                    1,
                                    user.UserAccount,
                                    DateTime.Now,
                                    DateTime.Now.AddDays(1),
                                    false,
                                    _userrepository.GetRoleByAccount(user.UserAccount)
                                    );

                                string encTicket = FormsAuthentication.Encrypt(authTicket);
                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                                cookie.Expires = DateTime.Now.AddDays(1);
                                this.Response.Cookies.Add(cookie);
                                Action = "Account.LogOn";
                                ClientIP = HttpContext.Request.UserHostAddress;
                                Content = "用户登录";
                                UserID = _log.GetUserIDByUserAccount(user.UserAccount);
                                _log.SaveLog(Action, ClientIP, Content, UserID);

                                string role = _userrepository.GetRoleByAccount(user.UserAccount);
                                if (role == "超级管理员")
                                {
                                    return RedirectToAction("Index", "Partner", new { area = "admin" });
                                }
                                if (role == "管理员")
                                {
                                    user = _userrepository.GetUserByAccount(user.UserAccount);
                                    if (user.PartnerID == null)
                                    {
                                        ModelState.AddModelError("UserAccount", "当前管理员帐号还没有被分配到院校，请联系超级管理员！");
                                        return PartialView();
                                    }
                                    else
                                        return RedirectToAction("Teacher", "Partner", new { area = "admin" });
                                }
                                if (role == "教师")
                                {
                                    return RedirectToAction("Index", "User", new { area = "admin" });
                                }
                                if (role == "测试")
                                {
                                    return RedirectToAction("Index", "Assignment", new { area = "admin" });
                                }
                                if (role == "赋值")
                                {
                                    return RedirectToAction("Index", "Assign", new { area = "admin" });
                                }
                                else
                                {
                                    return RedirectToAction("UserTestInfo", "PaPerShow");
                                }
                            }
                            else
                            {
                                return null;
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    return View(user);
                }
            }
        }

        #endregion

        #region 用户登录

        public ActionResult LogOn()
        {
            Front FrontInfo = new Front();
            FrontInfo = _homerepostory.FrontInfo();
            ViewData["SayContent"] = FrontInfo.SayContent;
            ViewData["SayNote"] = FrontInfo.SayNote;
            ViewData["SystemOverview"] = FrontInfo.SystemOverview;
            ViewData["CoreFeaturesOne"] = FrontInfo.CoreFeatures[0];
            ViewData["CoreFeaturesTwo"] = FrontInfo.CoreFeatures[1];
            ViewData["CoreFeaturesThree"] = FrontInfo.CoreFeatures[2];

            string UserName = User.Identity.Name;
            if (UserName != "")
            {
                ViewData["Name"] = UserName;
                ViewData["Role"] = _userrepository.SelectRole(UserName);
            }
            if (_userrepository.SelectRole(UserName) == "体验用户")
            {
                ViewData["Name"] = "游客";
            }
            var testingList = _homerepostory.GetTestingInfo();
            if (testingList.Count == 0)
            {
                var paperList = _paper.GetPapers();
                var userList = _userrepository.GetUserInfo();
                int index = paperList.Count;

                if (userList.Count > 5)
                {
                    for (int i = 0; i < index; i++)
                    {
                        CEDTS_Testing t = new CEDTS_Testing();
                        t.UserID = userList[i].UserID;
                        t.UserAccount = userList[i].UserAccount;
                        t.PaperID = paperList[i].PaperID;
                        t.PaperTitle = paperList[i].Title;
                        t.PaperType = paperList[i].State.Value;
                        t.Time = DateTime.Now;
                        testingList.Add(t);
                    }
                }
                else
                {
                    int tempnum = userList.Count > index ? index : userList.Count;
                    for (int j = 0; j < tempnum; j++)
                    {
                        CEDTS_Testing t = new CEDTS_Testing();
                        t.UserID = userList[j].UserID;
                        t.UserAccount = userList[j].UserAccount;
                        t.PaperID = paperList[j].PaperID;
                        t.PaperTitle = paperList[j].Title;
                        t.PaperType = paperList[j].State.Value;
                        t.Time = DateTime.Now;
                        testingList.Add(t);
                    }
                }

            }
            Response.Buffer = true;
            Response.ExpiresAbsolute = System.DateTime.Now.AddSeconds(-1);
            Response.Expires = 0;
            Response.CacheControl = "no-cache"; 
            ViewData["Testing"] = testingList;
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(string UserAccount, string UserPassword)
        {
            UserPassword = _userrepository.HashPassword(UserPassword);
            if (_userrepository.LogOn(UserAccount, UserPassword))
            {

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    UserAccount,
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    true,
                    _userrepository.GetRoleByAccount(UserAccount)
                    );
                string encTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                cookie.Expires = DateTime.Now.AddDays(1);
                this.Response.Cookies.Add(cookie);

                string Action = "Account.LogOn";
                string ClientIP = HttpContext.Request.UserHostAddress;
                string Content = "用户登录";
                int UserID = _log.GetUserIDByUserAccount(UserAccount);
                _log.SaveLog(Action, ClientIP, Content, UserID);
                 
                string role = _userrepository.GetRoleByAccount(UserAccount);
                if (role == "超级管理员")
                {
                    return RedirectToAction("Index", "Partner", new { area = "admin" });
                }
                if (role == "管理员")
                {
                    var user = _userrepository.GetUserByAccount(UserAccount);
                    if (user.PartnerID == null)
                    {
                    }
                    else
                        return RedirectToAction("Teacher", "Partner", new { area = "admin" });
                }
                if (role == "教师")
                {
                    return RedirectToAction("ClassList", "User", new { area = "admin" });
                }
                if (role == "测试")
                {
                    return RedirectToAction("Index", "Assignment", new { area = "admin" });
                }
                if (role == "赋值")
                {
                    return RedirectToAction("Index", "Assign", new { area = "admin" });
                }
                else
                {
                    return RedirectToAction("UserTestInfo", "PaPerShow");
                }

            }
            else
            {
                //ModelState.AddModelError("UserAccount", "您输入的帐号或密码有误，请重新登录");
                return RedirectToAction("LogOn");
            }
        }

        #endregion

        #region 密码找回

        public ActionResult RetrievePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RetrievePassword(FormCollection form)
        {
            string images = form["txtimages"];
            if (images != (string)Session["ValidateCode"])
            {
                ModelState.AddModelError("txtimages", "验证码不正确");
                return View();
            }
            else
            {
                if (_userrepository.AjaxCheckEmail(form["Email"], 0))
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        var user = _userrepository.GetUser(form["Email"]);
                        string pw = _userrepository.RandNum();
                        user.UserPassword = _userrepository.HashPassword(pw);
                        TryUpdateModel<CEDTS_User>(user, new string[] { "UserPassword" }, form.ToValueProvider());
                        _userrepository.Save();
                        string err = SendEmail(user, pw);
                        if (err != "true")
                        {
                            ViewData["Message"] = err;
                            return View("Error");
                        }
                        else
                        {
                            string Action = "Account.RetrievePassword";
                            string ClientIP = HttpContext.Request.UserHostAddress;
                            string Content = "用户找回密码";
                            int UserID = user.UserID;
                            _log.SaveLog(Action, ClientIP, Content, UserID);

                            tran.Complete();
                            a = 1;
                            return RedirectToAction("LogOn", "Account");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "您输入的邮箱不存在！");
                    return View();
                }
            }
        }

        #endregion


        #region 新密码找回

        public ActionResult NewRetrievePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewRetrievePassword(FormCollection form)
        {
            if (_userrepository.AjaxCheckEmail(form["Email"], 0))
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    var user = _userrepository.GetUser(form["Email"]);
                    if (user.UserAccount != form["Account"])
                    {
                        ModelState.AddModelError("Account", "您输入的学号不存在！");
                        return View();

                    }
                    else
                    {
                        int b = 123123;
                        string pw = Convert.ToString(b);
                        user.UserPassword = _userrepository.HashPassword(pw);
                        TryUpdateModel<CEDTS_User>(user, new string[] { "UserPassword" }, form.ToValueProvider());
                        _userrepository.Save();
                        string Action = "Account.NewRetrievePassword";
                        string ClientIP = HttpContext.Request.UserHostAddress;
                        string Content = "用户找回密码";
                        int UserID = user.UserID;
                        _log.SaveLog(Action, ClientIP, Content, UserID);
                        tran.Complete();
                        a = 1;
                        return RedirectToAction("LogOn", "Account");

                    }
                }
            }
            else
            {
                ModelState.AddModelError("Email", "您输入的邮箱不存在！");
                return View();
            }
            
        }

        #endregion
        #region 用户注销
        [Filter.LogFilter(Description = "用户注销")]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            this.Request.Cookies.Clear();
            return RedirectToAction("LogOn", "Account");
        }

        #endregion

        #region Ajax验证

        public bool AjaxAccount(string Account, int UserID)
        {
            return _userrepository.AjaxCheckAccount(Account, UserID);
        }

        public bool AjaxEmail(string Email, int UserID)
        {
            return _userrepository.AjaxCheckEmail(Email, UserID);
        }
        public bool AjaxCheck(string Account, string Password)
        {
            Password = _userrepository.HashPassword(Password);
            return _userrepository.LogOn(Account, Password);
        }

        public bool CheckAccount(string Account)
        {
            return _userrepository.AjaxCheckAccount(Account, 0);
        }

        #endregion

        #region 制作验证码
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
        public class ValidateCode
        {
            public ValidateCode()
            {
            }
            /// <summary>
            /// 验证码的最大长度
            /// </summary>
            public int MaxLength
            {
                get { return 10; }
            }
            /// <summary>
            /// 验证码的最小长度
            /// </summary>
            public int MinLength
            {
                get { return 1; }
            }
            /// <summary>
            /// 生成验证码
            /// </summary>
            /// <param name="length">指定验证码的长度</param>
            /// <returns></returns>
            public string CreateValidateCode(int length)
            {
                int[] randMembers = new int[length];
                int[] validateNums = new int[length];
                string validateNumberStr = "";
                //生成起始序列值
                int seekSeek = unchecked((int)DateTime.Now.Ticks);
                Random seekRand = new Random(seekSeek);
                int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
                int[] seeks = new int[length];
                for (int i = 0; i < length; i++)
                {
                    beginSeek += 10000;
                    seeks[i] = beginSeek;
                }
                //生成随机数字
                for (int i = 0; i < length; i++)
                {
                    Random rand = new Random(seeks[i]);
                    int pownum = 1 * (int)Math.Pow(10, length);
                    randMembers[i] = rand.Next(pownum, Int32.MaxValue);
                }
                //抽取随机数字
                for (int i = 0; i < length; i++)
                {
                    string numStr = randMembers[i].ToString();
                    int numLength = numStr.Length;
                    Random rand = new Random();
                    int numPosition = rand.Next(0, numLength - 1);
                    validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
                }
                //生成验证码
                for (int i = 0; i < length; i++)
                {
                    validateNumberStr += validateNums[i].ToString();
                }
                return validateNumberStr;
            }
            /// <summary>
            /// 创建验证码的图片
            /// </summary>
            /// <param name="containsPage">要输出到的page对象</param>
            /// <param name="validateNum">验证码</param>
            public byte[] CreateValidateGraphic(string validateCode)
            {
                Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 22);
                Graphics g = Graphics.FromImage(image);
                try
                {
                    //生成随机生成器
                    Random random = new Random();
                    //清空图片背景色
                    g.Clear(Color.White);
                    //画图片的干扰线
                    for (int i = 0; i < 25; i++)
                    {
                        int x1 = random.Next(image.Width);
                        int x2 = random.Next(image.Width);
                        int y1 = random.Next(image.Height);
                        int y2 = random.Next(image.Height);
                        g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                    }
                    Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                     Color.Blue, Color.DarkRed, 1.2f, true);
                    g.DrawString(validateCode, font, brush, 3, 2);
                    //画图片的前景干扰点
                    for (int i = 0; i < 100; i++)
                    {
                        int x = random.Next(image.Width);
                        int y = random.Next(image.Height);
                        image.SetPixel(x, y, Color.FromArgb(random.Next()));
                    }
                    //画图片的边框线
                    g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                    //保存图片数据
                    MemoryStream stream = new MemoryStream();
                    image.Save(stream, ImageFormat.Jpeg);
                    //输出图片流
                    return stream.ToArray();
                }
                finally
                {
                    g.Dispose();
                    image.Dispose();
                }
            }
            /// <summary>
            /// 得到验证码图片的长度
            /// </summary>
            /// <param name="validateNumLength">验证码的长度</param>
            /// <returns></returns>
            public static int GetImageWidth(int validateNumLength)
            {
                return (int)(validateNumLength * 12.0);
            }
            /// <summary>
            /// 得到验证码的高度
            /// </summary>
            /// <returns></returns>
            public static double GetImageHeight()
            {
                return 22.5;
            }
        }
        #endregion

        #region 邮件发送
        public string SendEmail(CEDTS_User user, string Password)
        {
            try
            {
                string verify_url = Request.Url.ToString();
                string Backpath = verify_url.Substring(0, verify_url.LastIndexOf('/') + 1);
                Backpath += "LogOn";
                MailModel model = new MailModel();
                model.Sendername = "CEDTS管理员";
                model.Senderemail = "linkinthinkmusic@sina.com";
                model.Receivername = user.UserAccount;
                model.Receiveremail = user.Email;
                string path = AppDomain.CurrentDomain.BaseDirectory + "BackUp.htm";
                string mailBody = System.IO.File.ReadAllText(path, System.Text.Encoding.GetEncoding("UTF-8"));
                StringBuilder strs = new StringBuilder();
                strs.Append("<table>");
                strs.AppendFormat("<tr><td colspan='2'>{0}，你好：</td></tr>", user.UserAccount);
                strs.AppendFormat("<tr><td  style='width: 80px'></td><td>温馨提示：您在({0})申请了密码找回。</td></tr>", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                strs.AppendFormat(" <tr><td style='width: 80px'></td><td>现密码重置为（{0}）</td></tr>", Password);
                strs.AppendFormat(" <tr><td style='width: 80px'></td><td>请点击<a href='{0}'>进入系统</a>处理</td></tr>", Backpath);
                strs.Append("</table>");
                model.Sendcontent = mailBody.Replace("@MailTable@", strs.ToString());
                Thread myThread = new Thread(new ParameterizedThreadStart(new MailHelper().SendMail));
                myThread.IsBackground = true;
                myThread.Start(model);
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        #endregion

        public JsonResult State()
        {
            int temp = 0;
            temp = a;
            a = 0;
            return Json(temp);
        }

        public ActionResult ErrorTips()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserInfo()
        {
            string account = User.Identity.Name;
            var user = _userrepository.GetUserByAccount(account);
            if (user.PartnerID != null)
                ViewData["Partner"] = _partner.GetPartnerbyID(user.PartnerID.Value);
            else
                ViewData["Partner"] = null;
            if (user.MajorID != null)
                ViewData["Major"] = _partner.GetMajorbyID(user.MajorID.Value);
            else
                ViewData["Major"] = null;
            if (user.GradeID != null)
                ViewData["Grade"] = _partner.GetGradebyID(user.GradeID.Value);
            else
                ViewData["Grade"] = null;
            if (user.ClassID != null)
                ViewData["Class"] = _partner.GetClassbyID(user.ClassID.Value);
            else
                ViewData["Class"] = null;


            var PartnerList = _userrepository.GetPartner().OrderBy(p => p.CreateTime).ToList();

            if (PartnerList.Count > 0)
            {
                CEDTS_Partner partner = new CEDTS_Partner();
                partner.PartnerID = Guid.Empty;
                partner.PartnerName = "请选择";
                PartnerList.Insert(0, partner);
                ViewData["PartnerID"] = new SelectList(PartnerList, "PartnerID", "PartnerName", Guid.Empty);
                var major1 = _partner.GetMajorbyLevel(1, PartnerList[0].PartnerID);
                if (major1.Count > 0)
                {
                    Cedts_Test.Areas.Admin.Models.CEDTS_Major major = new Cedts_Test.Areas.Admin.Models.CEDTS_Major();
                    major.MajorID = Guid.Empty;
                    major.MajorName = "请选择";
                    major1.Insert(0, major);
                    ViewData["MajorID1"] = new SelectList(major1, "MajorID", "MajorName", Guid.Empty);
                    var major2 = _partner.GetMajorbyUpID(major1[0].MajorID);
                    if (major2.Count > 0)
                    {
                        major2.Insert(0, major);
                        ViewData["MajorID2"] = new SelectList(major2, "MajorID", "MajorName");
                        var major3 = _partner.GetMajorbyUpID(major2[0].MajorID);
                        if (major3.Count > 0)
                        {
                            major3.Insert(0, major);
                            ViewData["MajorID"] = new SelectList(major3, "MajorID", "MajorName");
                            var GradeList = _partner.GetGradeByMID(major3[0].MajorID);
                            if (GradeList.Count > 0)
                            {
                                Cedts_Test.Areas.Admin.Models.CEDTS_Grade grade = new Areas.Admin.Models.CEDTS_Grade();
                                grade.GradeID = Guid.Empty;
                                grade.GradeName = "请选择";
                                GradeList.Insert(0, grade);
                                ViewData["GradeID"] = new SelectList(GradeList, "GradeID", "GradeName", Guid.Empty);
                                var ClassList = _partner.GetClassByGID(GradeList[0].GradeID);
                                if (ClassList.Count > 0)
                                {
                                    Cedts_Test.Areas.Admin.Models.CEDTS_Class Class = new Areas.Admin.Models.CEDTS_Class();
                                    Class.ClassID = Guid.Empty;
                                    Class.ClassName = "请选择";
                                    ClassList.Insert(0, Class);
                                    ViewData["ClassID"] = new SelectList(ClassList, "ClassID", "ClassName", Guid.Empty);
                                }
                                else
                                {
                                    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                                }
                            }
                            else
                            {
                                ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                                ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                            }
                        }
                        else
                        {
                            ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                            ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                            ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                        }
                    }
                    else
                    {
                        ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                        ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                        ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                        ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                    }
                }
                else
                {
                    ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                    ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                }
            }
            else
            {
                ViewData["PartnerID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID1"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID2"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["MajorID"] = new List<SelectListItem>() { new SelectListItem { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["GradeID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
                ViewData["ClassID"] = new List<SelectListItem> { new SelectListItem() { Text = "请选择", Value = Guid.Empty.ToString() } };
            }

            if (user.State == null)
            {
                ViewData["state"] = "待审核";
            }
            else
            {
                ViewData["state"] = "已审核";
            }

            if (user.BindCard == null)
            {
                ViewData["Card"] = "无";
            }
            else
            {
                var bindCard = _card.SelectCardByID(user.BindCard.Value);
                ViewData["Card"] = bindCard.SerialNumber;
                ViewData["OverdueTime"] = bindCard.OverdueTime;
            }

            ViewData["ischange"] = TempData["ischange"];
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UserInfo(CEDTS_User user)
        {
            string account = User.Identity.Name;
            var olduser = _userrepository.GetUserByAccount(account);
            olduser.UserAccount = user.UserAccount;
            if (user.UserPassword != null)
            {
                olduser.UserPassword = _userrepository.HashPassword(user.UserPassword);
            }
            olduser.UserNickname = user.UserNickname;
            olduser.Sex = user.Sex;
            olduser.Email = user.Email;
            olduser.StudentNumber = user.StudentNumber;
            olduser.Phone = user.Phone;
            _userrepository.ChangeUser(olduser);
            TempData["ischange"] = 1;
            return RedirectToAction("UserInfo");
        }

        [Authorize]
        public JsonResult EditUserInfo(Guid PartnerID, Guid MajorID, Guid GradeID, Guid ClassID)
        {
            string account = User.Identity.Name;
            var user = _userrepository.GetUserByAccount(account);
            user.PartnerID = PartnerID;
            user.MajorID = MajorID;
            user.GradeID = GradeID;
            user.ClassID = ClassID;
            user.State = null;
            _userrepository.ChangeUser(user);
            return Json("");
        }

        [Authorize]
        public JsonResult OutClass()
        {
            string account = User.Identity.Name;
            var user = _userrepository.GetUserByAccount(account);
            user.PartnerID = null;
            user.MajorID = null;
            user.GradeID = null;
            user.ClassID = null;
            user.State = null;
            _userrepository.ChangeUser(user);
            return Json("1");
        }

        [Authorize]
        public ActionResult MyCard(int? id)
        {
            string UserAccount = User.Identity.Name;
            int UserID = _userrepository.GetUserByAccount(UserAccount).UserID;
            ViewData["time"] = TempData["time"];
            return View(_card.GetCardByUserID(UserID, id));
        }

        [Authorize]
        public ActionResult Activate(int ID)
        {
            string UserAccount = User.Identity.Name;
            var user = _userrepository.GetUserByAccount(UserAccount);
            if (user.BindCard != null)
            {
                var oleCard = _card.SelectCardByID(user.BindCard.Value);
                if (oleCard.OverdueTime.Value.CompareTo(DateTime.Now) <= 0)
                {
                    TempData["time"] = "1";//表示当前用户已激活的卡还没有到期
                    return RedirectToAction("MyCard");
                }
            }
            var card = _card.SelectCardByID(ID);
            card.ActivationState = 1;
            card.ActivationTime = DateTime.Now;

            card.ActivationUser = user.UserID;
            switch (card.EffectiveTime)
            {
                case "一年": card.OverdueTime = card.ActivationTime.Value.AddYears(1); break;
                case "一月": card.OverdueTime = card.ActivationTime.Value.AddMonths(1); break;
                case "30次": card.OverdueTime = card.ActivationTime.Value; break;
                default: break;
            }
            _card.UpdateCard(card);
            user.BindCard = ID;
            _userrepository.ChangeUser(user);
            return RedirectToAction("MyCard");
        }

        [Authorize]
        public ActionResult BuyCard()
        {
            ViewData["Year"] = ConfigurationManager.AppSettings["Year"];//当前年卡金额
            ViewData["Month"] = ConfigurationManager.AppSettings["Month"];//当前月卡金额
            ViewData["Times"] = ConfigurationManager.AppSettings["Times"];//当前次数卡金额
            ViewData["YTime"] = ConfigurationManager.AppSettings["YTime"];//当前年卡时效
            ViewData["MTime"] = ConfigurationManager.AppSettings["MTime"];//当前月卡时效
            ViewData["TTime"] = ConfigurationManager.AppSettings["TTime"];//当前次数卡时效
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult BuyCard(string txt, string pwd, string image)
        {
            if (image != (string)Session["ValidateCode"])
            {
                return Json("3");
            }
            else
            {
                string SerialNumber = txt;
                string PassWord = _card.HashPassword(pwd);
                var card = _card.CheckCard(SerialNumber, PassWord);
                if (card != null)
                {
                    if (card.ActivationState != 0 || card.ActivationTime != null || card.ActivationUser != null)
                    {
                        return Json("2");
                    }
                    else
                    {
                        _card.CreatUserCard(card.ActivationUser.Value, card.ID);
                        return Json("1");
                    }

                }
                else
                    return Json("0");
            }
        }

        #region 联系我们
        public ActionResult ContactUs()
        {
            ContactUsInfo ContactUsInfo = _homerepostory.ContactUsInfo();
            ViewData["Name"] = ContactUsInfo.Name;
            ViewData["Address"] = ContactUsInfo.Address;
            ViewData["Tel"] = ContactUsInfo.Tel;
            ViewData["Url"] = ContactUsInfo.Url;
            ViewData["ZipCode"] = ContactUsInfo.ZipCode;
            return View();
        }
        #endregion

        #region 系统概况
        public ActionResult SystemOverview()
        {
            return View();
        }
        public ActionResult sys()
        {

            return Json(_homerepostory.GetSys());
        }
        #endregion

        #region 特色功能
        public ActionResult CoreFeatures()
        {
            return View(_homerepostory.CoreFeatures());
        }
        #endregion

        #region 使用说明
        public ActionResult Instructions()
        {
            return View();
        }
        public ActionResult instr()
        {
            return Json(_homerepostory.GetInstructions());
        }
        #endregion

        #region 意见反馈
        public ActionResult Feedback()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Feedback(FormCollection form)
        {
            CEDTS_Feedback Feedback = new CEDTS_Feedback();
            Feedback.Title = form["title"];
            Feedback.Content = form["content"];
            Feedback.Tel = form["tel"];
            Feedback.Email = form["email"];
            Feedback.Time = DateTime.Now;
            _homerepostory.Feedback(Feedback);
            return View();
        }
        #endregion

        #region 登陆
        [HttpPost]
        public ActionResult LogOn1(string name, string pass)
        {
            pass = _userrepository.HashPassword(pass);
            if (_userrepository.LogOn(name, pass))
            {

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    name,
                    DateTime.Now,
                    DateTime.Now.AddDays(1),
                    false,
                    _userrepository.GetRoleByAccount(name)
                    );

                string encTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                cookie.Expires = DateTime.Now.AddDays(1);
                this.Response.Cookies.Add(cookie);

                string Action = "Account.LogOn";
                string ClientIP = HttpContext.Request.UserHostAddress;
                string Content = "用户登录";
                int UserID = _log.GetUserIDByUserAccount(name);
                _log.SaveLog(Action, ClientIP, Content, UserID);
                return Json(1);
            }
            else
            {
                return Json("");
            }
        }
        #endregion

        #region 页面跳转
        public ActionResult Goto(string path)
        {
            string s = User.Identity.Name;
            return RedirectToAction("LogOn");
        }
        #endregion

        #region 游客注册登陆
        public ActionResult VisitorRegister()
        {
            CEDTS_User user = new CEDTS_User();
            user.UserAccount = Guid.NewGuid().ToString();
            user.UserPassword = "123123";
            user.UserPassword = _userrepository.HashPassword(user.UserPassword);
            _userrepository.CreateUser(user);


            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                    1,
                                    user.UserAccount,
                                    DateTime.Now,
                                    DateTime.Now.AddDays(1),
                                    false,
                                    _userrepository.GetRoleByAccount(user.UserAccount)
                                    );

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.Expires = DateTime.Now.AddDays(1);
            this.Response.Cookies.Add(cookie);


            return Json("");
        }
        #endregion
    }
}
