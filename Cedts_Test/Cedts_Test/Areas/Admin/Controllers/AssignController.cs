using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.Transactions;
using System.IO;

namespace Cedts_Test.Areas.Admin.Controllers
{

    public class AssignController : Controller
    {
        ICedts_AssignRepository _Ag;
        ICedts_ItemRepository _item;
        ICedts_QuestionKnowledgeRepository _QuestionKnowledge;
        public AssignController(ICedts_AssignRepository a, ICedts_ItemRepository i, ICedts_QuestionKnowledgeRepository q)
        {
            _Ag = a;
            _item = i;
            _QuestionKnowledge = q;
        }
        Cedts_Entities db = new Models.Cedts_Entities();
        // GET: /Admin/Assign/
        public static string condition = string.Empty;
        public static string txt = string.Empty;
        [Authorize(Roles = "赋值")]
        public ActionResult Index(int? id)
        {

            return View(_Ag.SelectItemsByCondition(id, condition, txt));

        }
        [HttpPost]
        public ActionResult Index(int? id, FormCollection form)
        {

            //id = 1;
            condition = form["condition"];
            txt = form["txtSearch"];

            return View(_Ag.SelectItemsByCondition(id, condition, txt));
        }

        public ActionResult Listen(Guid id)
        {
            ViewData["ID"] = id;
            int? ItemID = _item.GetEditItemID(id);
            if (ItemID == 1)
            {
                return View("SAS");
            }
            if (ItemID == 5)
            {
                return View("Complex");
            }

            if (ItemID == 6)
            {
                return View("BankedCloze");
            }
            if (ItemID == 7)
            {
                return View("MultipleChoice");
            }
            if (ItemID == 8)
            {
                return View("Cloze");
            }
            else
            {
                return View("Listen");
            }
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult Listen(FormCollection form)
        {
            ExcelInfo ExcelInfo = new Models.ExcelInfo();
            ExcelInfo.KPName = new List<string>();
            ExcelInfo.OldKPName = new List<string>();
            ExcelInfo.QuestionID = new List<string>();
            ExcelInfo.KnowledgeID = new List<string>();


            ExcelInfo.ItemName = form["ItemType_CN"];
            ExcelInfo.UserName = User.Identity.Name;
            ExcelInfo.ItemID = form["AssessID"];


            ExcelInfo ExcelInfo1 = new Models.ExcelInfo();
            ExcelInfo1.KPName = new List<string>();
            ExcelInfo1.OldKPName = new List<string>();
            ExcelInfo1.QuestionID = new List<string>();
            ExcelInfo1.KnowledgeID = new List<string>();
            ExcelInfo1.FirstName = new List<string>();

            ExcelInfo1.ItemName = form["ItemType_CN"];
            ExcelInfo1.UserName = User.Identity.Name;
            ExcelInfo1.ItemID = form["AssessID"];
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
                Guid qid = Guid.Parse(OldQuestionID);//第二次赋值QuestionID
                var first = (from m in db.CEDTS_AssignQuestionKnowledge where m.QuestionID == qid select m.KnowledgePointID).ToList();
                int Length = 0;
                int Length1 = 0;
                if (list.Length <= list1.Length || list.Length <= first.Count)//第三次赋值知识点个数小于第一个和第二次个数
                {
                    if (list.Length <= list1.Length)//第二次赋值与第三次赋值知识点个数是否相同
                    {
                        for (int s = 0; s < list1.Length; s++)
                        {
                            if (list.Contains(list1[s]))
                            {
                                Length += 1;
                            }
                        }
                    }

                    if (list.Length <= first.Count)//第三次和第一次赋值比较
                    {
                        for (int s = 0; s < first.Count; s++)
                        {
                            if (list.Contains(first[s].ToString()))
                            {
                                Length1 += 1;
                            }
                        }
                    }
                }

                if (Length == list.Length || Length1 == list.Length)
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
                    string TwoName = string.Empty;
                    Guid FQID = Guid.Parse(OldQuestionID);
                    var FirstKPIDList = (from m in db.CEDTS_AssignQuestionKnowledge where m.QuestionID == FQID select m.KnowledgePointID);
                    foreach (var FirstKPID in FirstKPIDList)
                    {
                        var Name = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == FirstKPID select m.Title).FirstOrDefault();
                        TwoName += Name + ",";
                    }
                    ExcelInfo1.KnowledgeID.Add(KpID);
                    ExcelInfo1.OldKPName.Add(Oldname);
                    ExcelInfo1.KPName.Add(name);
                    ExcelInfo1.QuestionID.Add(OldQuestionID);
                    ExcelInfo1.FirstName.Add(TwoName);
                    ExcelInfo1.Time = DateTime.Now;
                }


            }
            try
            {
                using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {
                    if (ExcelInfo1.QuestionID.Count > 0)
                    {
                        if (ExcelInfo.QuestionID.Count > 0)
                        {
                            for (int i = 0; i < ExcelInfo.QuestionID.Count; i++)
                            {

                                Guid QID = Guid.Parse(ExcelInfo.QuestionID[i]);
                                var QkInfos = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == QID select m).ToList();
                                foreach (var qkinfo in QkInfos)
                                {
                                    db.DeleteObject(qkinfo);
                                }
                                string[] kps = ExcelInfo.KnowledgeID[i].Split(',');
                                for (int j = 0; j < kps.Length; j++)
                                {
                                    CEDTS_QuestionKnowledge Qk = new CEDTS_QuestionKnowledge();
                                    Qk.QuestionID = QID;
                                    Qk.KnowledgePointID = Guid.Parse(kps[j]);
                                    Qk.Weight = (j + 1);
                                    db.AddToCEDTS_QuestionKnowledge(Qk);
                                    db.SaveChanges();
                                }

                            }
                        }
                        string QID1 = string.Empty;
                        for (int j = 0; j < ExcelInfo1.QuestionID.Count; j++)
                        {
                            if (j == ExcelInfo1.QuestionID.Count - 1)
                            {
                                QID1 += ExcelInfo.QuestionID[j];
                            }
                            else
                            {
                                QID1 += ExcelInfo.QuestionID[j] + ",";
                            }
                        }
                        CEDTS_Assign Assign = new CEDTS_Assign();
                        Assign.AssessmentItemID = Guid.Parse(ExcelInfo1.ItemID);
                        Assign.Time = DateTime.Now;
                        Assign.QuestionID = QID1;

                        db.AddToCEDTS_Assign(Assign);
                        db.SaveChanges();

                        Guid ID = Guid.Parse(ExcelInfo.ItemID);

                        db.UpdateState(ID);
                        db.SaveChanges();

                    }
                    else
                    {

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
                        Guid ID = Guid.Parse(ExcelInfo.ItemID);
                        var AssignItem = (from m in db.CEDTS_AssignAssessment where m.AssessmentItemID == ID select m).FirstOrDefault();
                        db.DeleteObject(AssignItem);
                        db.SaveChanges();
                    }
                    tran.Complete();
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }

            if (ExcelInfo1.KPName.Count > 0)
            {
                string FileName = "D://Cedts//Excel//" + User.Identity.Name + "第三次赋值不同.txt";

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
                    for (int a = 0; a < ExcelInfo1.QuestionID.Count; a++)
                    {
                        string Content = string.Empty;
                        Content = Num + "            " + ExcelInfo1.UserName + "                   " + ExcelInfo1.UpdateUserName + "                  " + ExcelInfo1.Time + "             " + ExcelInfo1.ItemName + "           " + ExcelInfo1.ItemID + "                " + ExcelInfo1.QuestionID[a] + "                   " + ExcelInfo1.OldKPName[a] + "                                 " + ExcelInfo1.FirstName[a] + "                                 " + ExcelInfo1.KPName[a];
                        Sw.WriteLine(Content);
                    }
                    Sw.Close();
                    Sw.Close();
                }
                else
                {
                    FileStream myFs = new FileStream(FileName, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    string Title = "编号" + "          " + "用户名" + "              " + "上次更新用户" + "                     " + "更改时间" + "                " + "试题类型" + "                         " + "ItemID" + "                                      " + "QuestionID" + "                                  " + "第一次知识点名称" + "                             " + "第二次知识点名称" + "                             " + "第三次知识点";
                    mySw.WriteLine(Title);

                    for (int a = 0; a < ExcelInfo1.QuestionID.Count; a++)
                    {
                        string Content = string.Empty;
                        Content = (a + 1) + "            " + ExcelInfo1.UserName + "                   " + ExcelInfo1.UpdateUserName + "                  " + ExcelInfo1.Time + "             " + ExcelInfo1.ItemName + "           " + ExcelInfo1.ItemID + "                " + ExcelInfo1.QuestionID[a] + "                   " + ExcelInfo1.OldKPName[a] + "                                 " + ExcelInfo1.FirstName[a] + "                                 " + ExcelInfo1.KPName[a];
                        mySw.WriteLine(Content);
                    }
                    mySw.Close();
                    myFs.Close();
                }
            }
            return RedirectToAction("index");
        }
        public JsonResult GetListen(Guid id)
        {
            Listen s = _Ag.SelectListen(id);
            return Json(s);
        }
        /// <summary>
        /// 编辑时获取复合型听力信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>Json</returns>
        public JsonResult GetComplex(Guid id)
        {
            Listen s = _Ag.SelectComplex(id);

            return Json(s);
        }

        /// <summary>
        /// 编辑时获取快速阅读信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>json</returns>
        public JsonResult GetSspc(Guid id)
        {
            return Json(_Ag.SelectSspc(id));
        }

        /// <summary>
        /// 编辑时获取当前点击的完型填空信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>JSON</returns>
        public JsonResult GetCloze(Guid id)
        {
            return Json(_Ag.SelectCloze(id));
        }

        /// <summary>
        /// 编辑是获取当前点击的阅读理解-选择题型
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>json</returns>
        public JsonResult GetRpo(Guid id)
        {
            return Json(_Ag.SelectRpo(id));
        }

        /// <summary>
        /// 编辑时获取当前点击的阅读理解-选词填空
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>Json</returns>
        public JsonResult GetRpc(Guid id)
        {
            return Json(_Ag.SelectRpc(id));
        }
    }
}
