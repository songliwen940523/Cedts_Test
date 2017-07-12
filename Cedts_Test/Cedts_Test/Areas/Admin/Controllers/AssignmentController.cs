using System;
using System.Linq;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using Webdiyer.WebControls.Mvc;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using System.Configuration;

namespace Cedts_Test.Areas.Admin.Controllers
{
    public class AssignmentController : Controller
    {
        ICedts_ItemRepository _item;
        ICedts_QuestionKnowledgeRepository _QuestionKnowledge;
        public AssignmentController(ICedts_ItemRepository i, ICedts_QuestionKnowledgeRepository q)
        {
            _item = i;
            _QuestionKnowledge = q;
        }
        Cedts_Entities db = new Models.Cedts_Entities();
        int defaultPageSize = 10;
        public static int? ID = null;
        // GET: /Admin/Assignment/
        [Authorize(Roles = "测试")]
        public ActionResult Index(int? id)
        {
            //var UserID = (from m in db.CEDTS_User where m.UserAccount == User.Identity.Name select m.UserID).FirstOrDefault();
            ID = id;
            if (id == null)
            {
                id = ID;
            }




            int UserID = 0;
            if (User.Identity.Name == "ceshi4")
            {
                UserID = int.Parse(ConfigurationManager.AppSettings["UserID1"]);
            }
            if (User.Identity.Name == "ceshi5")
            {
                UserID = int.Parse(ConfigurationManager.AppSettings["UserID2"]);
            }
            if (User.Identity.Name == "ceshi6")
            {
                UserID = int.Parse(ConfigurationManager.AppSettings["UserID3"]);
            }
            IQueryable<ExaminationItem> Item1 = from m in db.CEDTS_AssessmentItem
                                                from s in db.CEDTS_User
                                                from n in db.CEDTS_ItemType
                                                orderby m.ItemTypeID, m.SaveTime
                                                where m.ItemTypeID == n.ItemTypeID && m.UpdateUserID == s.UserID && m.UpdateUserID == UserID
                                                select new ExaminationItem
                                                {
                                                    AssessmentItemID = m.AssessmentItemID,
                                                    ItemName = n.TypeName_CN,
                                                    SaveTime = m.SaveTime.Value,
                                                    UpdateTime = m.UpdateTime.Value,
                                                    Username = s.UserNickname,
                                                    ItemID = m.ItemTypeID.Value
                                                };
            int Take = int.Parse(ConfigurationManager.AppSettings["Take"]);
            switch (User.Identity.Name)
            {
                //case "ceshi1":
                //    int Skip1 = int.Parse(ConfigurationManager.AppSettings["Skip1"]);
                //    Item1 = Item1.Skip(Skip1).Take(Take);
                //    PagedList<ExaminationItem> Items1 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                //    return View(Items1);

                //case "ceshi2":
                //    int Skip2 = int.Parse(ConfigurationManager.AppSettings["Skip2"]);
                //    Item1 = Item1.Skip(Skip2).Take(Take);
                //    PagedList<ExaminationItem> Items2 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                //    return View(Items2);
                //case "ceshi3":
                //    int Skip3 = int.Parse(ConfigurationManager.AppSettings["Skip3"]);
                //    Item1 = Item1.Skip(Skip3).Take(Take);
                //    PagedList<ExaminationItem> Items3 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                //    return View(Items3);
                case "ceshi4":
                    int Skip4 = int.Parse(ConfigurationManager.AppSettings["Skip4"]);
                    Item1 = Item1.Skip(Skip4).Take(Take);
                    PagedList<ExaminationItem> Items4 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                    return View(Items4);
                case "ceshi5":
                    int Skip5 = int.Parse(ConfigurationManager.AppSettings["Skip4"]);
                    Item1 = Item1.Skip(Skip5).Take(Take);
                    PagedList<ExaminationItem> Items5 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                    return View(Items5);
                case "ceshi6":
                    Item1 = Item1.Skip(1000).Take(200);
                    PagedList<ExaminationItem> Items6 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                    return View(Items6);
                //case "ceshi7":
                //    Item1 = Item1.Skip(1200).Take(200);
                //    PagedList<ExaminationItem> Items7 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                //    return View(Items7);
                //case "ceshi8":
                //    Item1 = Item1.Skip(1400).Take(200);
                //    PagedList<ExaminationItem> Items8 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                //    return View(Items8);
                //case "ceshi9":
                //    Item1 = Item1.Skip(1600).Take(200);
                //    PagedList<ExaminationItem> Items9 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                //    return View(Items9);
                //case "ceshi10":
                //    Item1 = Item1.Skip(1800).Take(200);
                //    PagedList<ExaminationItem> Items10 = Item1.ToPagedList(id ?? 1, defaultPageSize);
                //    return View(Items10);
                default: return null;
            }

        }

        public ActionResult EditListen(Guid id)
        {

            var UserID = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == id select m.UpdateUserID).FirstOrDefault();
            var Info = (from m in db.CEDTS_AssignAssessment where m.AssessmentItemID == id select m.UpdateUserID).FirstOrDefault();
            int UserID1 = _item.SelectUserID(User.Identity.Name);
            if (UserID == UserID1 || Info == UserID1)
            {
                return View("ViewPage1");
            }
            else
            {
                ViewData["ID"] = id;
                int? ItemID = _item.GetEditItemID(id);
                if (ItemID == 1)
                {
                    return View("EditSAS");
                }
                if (ItemID == 5)
                {
                    return View("EditComplex");
                }

                if (ItemID == 6)
                {
                    return View("EditBankedCloze");
                }
                if (ItemID == 7)
                {
                    return View("EditMultipleChoice");
                }
                if (ItemID == 8)
                {
                    return View("EditCloze");
                }
                else
                {
                    return View("EditListen");
                }
            }
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditListen(FormCollection form)
        {
            ExcelInfo ExcelInfo = new Models.ExcelInfo();
            ExcelInfo.KPName = new List<string>();
            ExcelInfo.OldKPName = new List<string>();
            ExcelInfo.QuestionID = new List<string>();
            ExcelInfo.KnowledgeID = new List<string>();

            ExcelInfo ExcelInfo1 = new Models.ExcelInfo();
            ExcelInfo1.KPName = new List<string>();
            ExcelInfo1.KnowledgeID = new List<string>();
            ExcelInfo1.QuestionID = new List<string>();
            ExcelInfo1.OldKPName = new List<string>();

            ExcelInfo.ItemName = form["ItemType_CN"];
            ExcelInfo.UserName = User.Identity.Name;
            ExcelInfo.ItemID = form["AssessID"];

            ExcelInfo1.ItemName = form["ItemType_CN"];
            ExcelInfo1.UserName = User.Identity.Name;
            ExcelInfo1.ItemID = form["AssessID"];

            Guid AssessmentID = Guid.Parse(ExcelInfo.ItemID);
            var UpdateUserID = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssessmentID select m.UpdateUserID).FirstOrDefault();
            var UpdateUserName = (from m in db.CEDTS_User where m.UserID == UpdateUserID select m.UserAccount).FirstOrDefault();


            ExcelInfo.UpdateUserName = UpdateUserName;
            ExcelInfo1.UpdateUserName = UpdateUserName;
            int Count = int.Parse(form["Count"]);

            int QuestionCount = int.Parse(form["QuestionCount"]);
            string ques = form["QuestionID"];
            string[] QuestionID = ques.Split(',');


            for (int i = 0; i < QuestionCount; i++)
            {
                string KpID = form["hidden" + (i + 1)];
                string OldKPID = form["OldKPID" + (i + 1)];
                string OldQuestionID = form["OldQuestionID" + (i + 1)];
                string[] list = KpID.Split(',');
                string[] list1 = OldKPID.Split(',');
                int Length = 0;
                if (list.Length == list1.Length)
                {
                    for (int s = 0; s < list.Length; s++)
                    {
                        if (list.Contains(list1[s]))
                        {
                            Length += 1;
                        }
                    }
                }

                if (KpID != OldKPID)
                {
                    if (Length != list.Length)
                    {

                        ExcelInfo.KnowledgeID.Add(KpID);
                        string name = string.Empty;
                        string[] Kps = KpID.Split(',');
                        for (int b = 0; b < Kps.Length; b++)
                        {
                            Guid Kp = Guid.Parse(Kps[b]);
                            var Kpname = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Kp select m.Title).FirstOrDefault();
                            if (b == Kps.Length - 1)
                            {
                                name += Kpname;
                            }
                            else
                            {
                                name += Kpname + ",";
                            }
                        }
                        string Oldname = string.Empty;
                        string[] OldKps = OldKPID.Split(',');
                        for (int b = 0; b < OldKps.Length; b++)
                        {
                            Guid Kp = Guid.Parse(OldKps[b]);
                            var Kpname = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Kp select m.Title).FirstOrDefault();
                            if (b == OldKps.Length - 1)
                            {
                                Oldname += Kpname;
                            }
                            else
                            {
                                Oldname += Kpname + ",";
                            }
                        }

                        ExcelInfo.OldKPName.Add(Oldname);
                        ExcelInfo.KPName.Add(name);
                        ExcelInfo.QuestionID.Add(OldQuestionID);
                        ExcelInfo.Time = DateTime.Now;
                    }
                    else
                    {

                    }
                }
                else
                {
                    
                    string SameName = string.Empty;
                    string[] Kps = KpID.Split(',');
                    for (int b = 0; b < Kps.Length; b++)
                    {
                        Guid Kp = Guid.Parse(Kps[b]);
                        var Kpname = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Kp select m.Title).FirstOrDefault();
                        if (b == Kps.Length - 1)
                        {
                            SameName += Kpname;
                        }
                        else
                        {
                            SameName += Kpname + ",";
                        }
                    }
                    string Oldname = string.Empty;
                    string[] OldKps = OldKPID.Split(',');
                    for (int b = 0; b < OldKps.Length; b++)
                    {
                        Guid Kp = Guid.Parse(OldKps[b]);
                        var Kpname = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Kp select m.Title).FirstOrDefault();
                        if (b == OldKps.Length - 1)
                        {
                            Oldname += Kpname;
                        }
                        else
                        {
                            Oldname += Kpname + ",";
                        }
                    }
                    ExcelInfo1.KnowledgeID.Add(KpID);
                    ExcelInfo1.OldKPName.Add(Oldname);
                    ExcelInfo1.KPName.Add(SameName);
                    ExcelInfo1.QuestionID.Add(OldQuestionID);
                    ExcelInfo1.Time = DateTime.Now;
                }


            }
            try
            {
                using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    if (ExcelInfo.QuestionID.Count != 0)
                    {
                        if (UpdateUserID != 1)
                        {
                            CEDTS_AssignAssessment AssignItem = new CEDTS_AssignAssessment();
                            Guid ItemID = Guid.Parse(ExcelInfo.ItemID);
                            var ItemInfo = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == ItemID select m).FirstOrDefault();
                            AssignItem.AssessmentItemID = ItemInfo.AssessmentItemID;
                            AssignItem.Content = ItemInfo.Content;
                            AssignItem.Count = ItemInfo.Count;
                            AssignItem.Course = ItemInfo.Course;
                            AssignItem.Description = ItemInfo.Description;
                            AssignItem.Difficult = ItemInfo.Difficult;
                            AssignItem.Duration = ItemInfo.Duration;
                            AssignItem.Interval = ItemInfo.Interval;
                            AssignItem.ItemTypeID = ItemInfo.ItemTypeID;
                            AssignItem.Original = ItemInfo.Original;
                            AssignItem.QuestionCount = ItemInfo.QuestionCount;
                            AssignItem.SaveTime = ItemInfo.SaveTime;
                            AssignItem.Score = ItemInfo.Score;
                            AssignItem.Unit = ItemInfo.Unit;
                            AssignItem.UpdateTime = DateTime.Now;
                            AssignItem.UpdateUserID = _item.SelectUserID(User.Identity.Name);
                            AssignItem.UserID = ItemInfo.UserID;
                            AssignItem.State = true;
                            db.AddToCEDTS_AssignAssessment(AssignItem);
                            db.SaveChanges();

                            for (int i = 0; i < ExcelInfo.QuestionID.Count; i++)
                            {
                                Guid QuestionID1 = Guid.Parse(ExcelInfo.QuestionID[i]);

                                string[] KpID = ExcelInfo.KnowledgeID[i].Split(',');
                                for (int j = 0; j < KpID.Length; j++)
                                {
                                    Guid Kp = Guid.Parse(KpID[j]);
                                    CEDTS_AssignQuestionKnowledge AssignKp = new CEDTS_AssignQuestionKnowledge();
                                    AssignKp.KnowledgePointID = Kp;
                                    AssignKp.QuestionID = QuestionID1;
                                    db.AddToCEDTS_AssignQuestionKnowledge(AssignKp);
                                    db.SaveChanges();
                                }
                            }
                            Guid ItemID1 = Guid.Parse(ExcelInfo.ItemID);
                            int UserID = _item.SelectUserID(User.Identity.Name);
                            db.Update(ItemID1, UserID, DateTime.Now);
                        }
                        else
                        {
                            Guid ItemID = Guid.Parse(ExcelInfo.ItemID);
                            int UserID = _item.SelectUserID(User.Identity.Name);
                            db.Update(ItemID, UserID, DateTime.Now);

                            //删除QuestionKnowledge中数据
                            string qkIDs = form["QkID"];
                            string[] qkID = qkIDs.Split(',');
                            List<int> q = new List<int>();
                            for (int m = 0; m < qkID.Length; m++)
                            {
                                q.Add(Int32.Parse(qkID[m]));
                            }
                            _QuestionKnowledge.Delete(q);
                            //新增QuestionKnowledge中数据
                            for (int s = 0; s < QuestionID.Length; s++)
                            {
                                string[] Knowledge = form["hidden" + (s + 1)].ToString().Split(',');
                                for (int k = 0; k < Knowledge.Length; k++)
                                {
                                    CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                    qk.QuestionID = Guid.Parse(QuestionID[s]);
                                    qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                    qk.Weight = k + 1;
                                    _QuestionKnowledge.Create(qk);
                                }
                            }
                        }
                        string QID = string.Empty;
                        for (int j = 0; j < ExcelInfo.QuestionID.Count; j++)
                        {
                            if (j == ExcelInfo.QuestionID.Count - 1)
                            {
                                QID += ExcelInfo.QuestionID[j];
                            }
                            else
                            {
                                QID += ExcelInfo.QuestionID[j] + ",";
                            }
                        }
                        CEDTS_Assign Assign = new CEDTS_Assign();
                        Assign.AssessmentItemID = Guid.Parse(ExcelInfo.ItemID);
                        Assign.Time = DateTime.Now;
                        Assign.QuestionID = QID;

                        db.AddToCEDTS_Assign(Assign);
                        db.SaveChanges();
                    }
                    else
                    {

                        Guid ItemID2 = Guid.Parse(ExcelInfo.ItemID);
                        int UserID2 = _item.SelectUserID(User.Identity.Name);
                        db.Update(ItemID2, UserID2, DateTime.Now);
                        db.SaveChanges();
                    }
                    tran.Complete();
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
            #region Excel

            //Microsoft.Office.Interop.Excel.Application ExcelApp = new Application();
            //string FileName = ExcelInfo.UserName+".xls";
            //string file = "E://Web//" + FileName;
            //if (System.IO.File.Exists(file))
            //{
            //    Microsoft.Office.Interop.Excel.Workbook xBook = ExcelApp.Workbooks.Open(Server.MapPath("/"+FileName), Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);//

            //    int s = ExcelApp.ActiveSheet.UsedRange.Rows.Count;

            //    for (int l = 1; l < (ExcelInfo.QuestionID.Count+1); l++)
            //    {
            //        ExcelApp.Cells[(s + l), 1] = ExcelInfo.UserName;
            //        ExcelApp.Cells[(s + l), 2] = ExcelInfo.Time;
            //        ExcelApp.Cells[(s + l), 3] = ExcelInfo.ItemName;
            //        ExcelApp.Cells[(s + l), 4] = ExcelInfo.ItemID;
            //        ExcelApp.Cells[(s + l), 5] = ExcelInfo.QuestionID[(l-1)];
            //        ExcelApp.Cells[(s + l), 6] = ExcelInfo.OldKPName[(l-1)];
            //        ExcelApp.Cells[(s + l), 7] = ExcelInfo.KPName[(l-1)];
            //    }
            //    xBook.SaveAs(Server.MapPath("/" + FileName));//保存
            //    xBook.Close(false, Missing.Value, Missing.Value);//关闭
            //}
            //else
            //{
            //    ExcelApp.Visible = false;
            //    Microsoft.Office.Interop.Excel.Workbook Wb = ExcelApp.Application.Workbooks.Add(true);
            //    ExcelApp.Cells[1, 1] = "用户名";
            //    ExcelApp.Cells[1, 2] = "更改时间";
            //    ExcelApp.Cells[1, 3] = "试题类型";
            //    ExcelApp.Cells[1, 4] = "ItemID";
            //    ExcelApp.Cells[1, 5] = "QuestionID";
            //    ExcelApp.Cells[1, 6] = "更改前的知识点";
            //    ExcelApp.Cells[1, 7] = "更改后的知识点";
            //    for (int p = 2; p < (ExcelInfo.QuestionID.Count + 2); p++)
            //    {
            //        ExcelApp.Cells[p, 1] = ExcelInfo.UserName;
            //        ExcelApp.Cells[p, 2] = ExcelInfo.Time;
            //        ExcelApp.Cells[p, 3] = ExcelInfo.ItemName;
            //        ExcelApp.Cells[p, 4] = ExcelInfo.ItemID;
            //        ExcelApp.Cells[p, 5] = ExcelInfo.QuestionID[(p-2)];
            //        ExcelApp.Cells[p, 6] = ExcelInfo.OldKPName[(p-2)];
            //        ExcelApp.Cells[p, 7] = ExcelInfo.KPName[(p-2)];
            //    }

            //    Wb.SaveCopyAs(Server.MapPath("/" + FileName));//另存
            //    Wb.Close(false, Missing.Value, Missing.Value);//关闭

            //}
            #endregion

            if (ExcelInfo.KPName.Count > 0)
            {
                string FileName = "D://Cedts//Excel//" + User.Identity.Name + "赋值不同.txt";

                if (System.IO.File.Exists(FileName))
                {
                    StreamReader Sr = new StreamReader(FileName);
                    int Num = 0;
                    string Str = Sr.ReadToEnd();
                    Sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    string StrLine = Sr.ReadLine();
                    while (StrLine != null)
                    {
                        if (StrLine != "")
                        {
                            Num++;
                        }
                        StrLine = Sr.ReadLine();
                    }


                    
                    Sr.Close();
                    StreamWriter Sw = new StreamWriter(FileName);
                    Sw.WriteLine(Str);
                    for (int a = 0; a < ExcelInfo.QuestionID.Count; a++)
                    {
                        string Content = string.Empty;
                        Content = Num + "            " + ExcelInfo.UserName + "                   " + ExcelInfo.UpdateUserName + "                  " + ExcelInfo.Time + "             " + ExcelInfo.ItemName + "           " + ExcelInfo.ItemID + "                " + ExcelInfo.QuestionID[a] + "                  " + ExcelInfo.OldKPName[a] + "                                " + ExcelInfo.KPName[a];
                        Sw.WriteLine(Content);
                    }
                    Sw.Close();
                    Sw.Close();
                }
                else
                {
                    FileStream myFs = new FileStream(FileName, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    string Title = "编号" + "          " + "用户名" + "              " + "上次更新用户" + "                     " + "更改时间" + "                " + "试题类型" + "                         " + "ItemID" + "                                      " + "QuestionID" + "                                  " + "更改前知识点名称" + "                             " + "更改后知识点";
                    mySw.WriteLine(Title);

                    for (int a = 0; a < ExcelInfo.QuestionID.Count; a++)
                    {
                        string Content = string.Empty;
                        Content = (a + 1) + "            " + ExcelInfo.UserName + "                   " + ExcelInfo.UpdateUserName + "                  " + ExcelInfo.Time + "             " + ExcelInfo.ItemName + "           " + ExcelInfo.ItemID + "                " + ExcelInfo.QuestionID[a] + "                   " + ExcelInfo.OldKPName[a] + "                                 " + ExcelInfo.KPName[a];
                        mySw.WriteLine(Content);
                    }
                    mySw.Close();
                    myFs.Close();
                }
            }
            if (ExcelInfo1.KPName.Count > 0)
            {
                //赋值相同
                string FileName1 = "D://Cedts//Excel//" + User.Identity.Name + "赋值相同.txt";
                if (System.IO.File.Exists(FileName1))
                {
                    StreamReader Sr = new StreamReader(FileName1);
                    int Num = 0;
                    string Str = Sr.ReadToEnd();
                    Sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    string StrLine = Sr.ReadLine();
                    while (StrLine != null)
                    {
                        if (StrLine != "")
                        {
                            Num++;
                        }
                        StrLine = Sr.ReadLine();
                    }
                    
                    Sr.Close();
                    StreamWriter Sw = new StreamWriter(FileName1);
                    Sw.WriteLine(Str);
                    for (int a = 0; a < ExcelInfo1.QuestionID.Count; a++)
                    {
                        string Content = string.Empty;
                        Content = Num+ "            " + ExcelInfo1.UserName + "                   " + ExcelInfo1.UpdateUserName + "                  " + ExcelInfo1.Time + "             " + ExcelInfo1.ItemName + "           " + ExcelInfo1.ItemID + "                " + ExcelInfo1.QuestionID[a] + "                  " + ExcelInfo1.OldKPName[a] + "                                " + ExcelInfo1.KPName[a];
                        Sw.WriteLine(Content);
                    }
                    Sw.Close();
                    Sw.Close();
                }
                else
                {
                    FileStream myFs = new FileStream(FileName1, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    string Title = "编号" + "          " + "用户名" + "              " + "上次更新用户" + "                     " + "更改时间" + "                " + "试题类型" + "                         " + "ItemID" + "                                      " + "QuestionID" + "                                  " + "更改前知识点名称" + "                             " + "更改后知识点";
                    mySw.WriteLine(Title);

                    for (int a = 0; a < ExcelInfo1.QuestionID.Count; a++)
                    {
                        string Content = string.Empty;
                        Content = (a + 1) + "            " + ExcelInfo1.UserName + "                   " + ExcelInfo1.UpdateUserName + "                  " + ExcelInfo1.Time + "             " + ExcelInfo1.ItemName + "           " + ExcelInfo1.ItemID + "                " + ExcelInfo1.QuestionID[a] + "                   " + ExcelInfo1.OldKPName[a] + "                                 " + ExcelInfo1.KPName[a];
                        mySw.WriteLine(Content);
                    }
                    mySw.Close();
                    myFs.Close();
                }
            }
            return RedirectToAction("index");
        }

        public ActionResult EditGetPoint()
        {
            return Content(_item.GetPoint());
        }
        public JsonResult GetListen(Guid id)
        {
            return Json(_item.SelectAll(id));
        }
    }
}
