using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;

namespace Cedts_Test.Areas.Admin.Controllers
{
    public class AssignHomeworkController : Controller
    {
        //
        // GET: /Admin/Assignhomework/
        ICedts_UserRepository _user;
        ICedts_PaperRepository _paper;
        public static PaperTotal taskpaper = new PaperTotal();
        public static Guid ClassID = new Guid();
        public static SelectAssessmentItem SelectItem = new SelectAssessmentItem();
        public AssignHomeworkController(ICedts_UserRepository r, ICedts_PaperRepository pa)
        {
            _user = r;
            _paper = pa;
        }
        public ActionResult Index()
        {
            List<SelectListItem> ClassIDList = new List<SelectListItem>();
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            List<CEDTS_Class> ClassList = _user.GetClassbyUserID(UserID);
            for (int i = 0; i < ClassList.Count; i++)
            {
                ClassIDList.Add(new SelectListItem { Text = ClassList[i].ClassName, Value = ClassList[i].ClassID.ToString() });
            }
            ClassIDList[0].Selected = true;
            ViewData["ClassIDList"] = ClassIDList;
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            taskpaper.Title = form["TaskName"];
            ClassID = Guid.Parse(form["ClassIDList"]);
            string modename = form["FirstLevel"] + "|" + form["SecondLevel"];
            string actionname;
            switch (modename)
            {
                case "系统题库|四级真题":
                    actionname = "CreateByTypeOne";
                    break;
                case "系统题库|按题型组卷":
                    actionname = "CreateByTypeTwo";
                    break;
                case "系统题库|按知识点组卷":
                    actionname = "CreateByTypeThree";
                    break;
                case "个人题库|选择试卷":
                    actionname = "CreateByTypeFour";
                    break;
                case "个人题库|自主选题":
                    actionname = "CreateByTypeFive";
                    break;
                default:
                    actionname = "Index";
                    break;
            }
            return RedirectToAction(actionname);
        }

        public ActionResult CreateByTypeOne(int? id)
        {
            return View(_paper.SelectPaper(id, 1));
        }
        [HttpPost]
        public ActionResult CreateByTypeOne(string PaperIDNum)
        {
            Guid PaperID = Guid.Parse(PaperIDNum);
            List<CEDTS_PaperAssessment> paList = _paper.SelectPaperAssessmentItem(PaperID);
            List<CEDTS_AssessmentItem> itList = new List<CEDTS_AssessmentItem>();            
            foreach (var pa in paList)
            {
                Guid ItemID = Guid.Parse(pa.AssessmentItemID.ToString());
                CEDTS_AssessmentItem it = _paper.SelectAssessmentItem(ItemID);
                itList.Add(it);
            }
            taskpaper = MakePaper(itList, 3, null);
            if (taskpaper == null)
            {
                return Json("0");
            } 
            else
            {
                return Json("1");
            }
            
        }

        public ActionResult CreateByTypeTwo()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateByTypeTwo(int Sspc, int Slpo, int Llpo, int Rlpo, int Lpc, int Rpo, int Rpc, int Im, int Cp)
        {
            List<string> Number = new List<string>();

            Number.Add(Sspc.ToString());
            Number.Add(Slpo.ToString());
            Number.Add(Llpo.ToString());
            Number.Add(Rlpo.ToString());
            Number.Add(Lpc.ToString());
            Number.Add(Rpo.ToString());
            Number.Add(Rpc.ToString());
            Number.Add(Cp.ToString());
            Number.Add(Im.ToString());
            List<string> list = new List<string>();
            List<CEDTS_PartType> listPart = _paper.SelectPartType();
            for (int i = 0; i < listPart.Count; i++)
            {
                List<CEDTS_ItemType> listItem = _paper.SelectItemTypeByPartTypeID(listPart[i].PartTypeID);
                for (int j = 0; j < listItem.Count; j++)
                {
                    list.Add(listPart[i].PartTypeID.ToString() + "_" + (j + 1).ToString());
                }
            }
            List<string> CountList = new List<string>();
            for (int k = 0; k < list.Count; k++)
            {
                CountList.Add(Number[k]);
            }
            int userID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            taskpaper = MakePaper(_paper.SelectAssessmentItems(CountList, userID), 1, null);
            if (taskpaper == null)
            {
                return Json("0");
            }
            else
            {
                return Json("1");
            }
        }

        public ActionResult CreateByTypeThree()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateByTypeThree(string select2, string select3)
        {
            List<string> KnowledgeIDList = new List<string>();
            List<string> CountList = new List<string>();
            string[] s2 = select2.Split(',');
            string[] s3 = select3.Split(',');
            for (int i = 0; i < s2.Length - 1; i++)
            {
                KnowledgeIDList.Add(s2[i]);
                CountList.Add(s3[i]);
            }

            int userID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            taskpaper = MakePaper(_paper.SelectAssessmentItems2(KnowledgeIDList, CountList, userID), 2, KnowledgeIDList);
            if (taskpaper == null)
            {
                return Json("0");
            }
            else
            {
                return Json("1");
            }
        }

        public ActionResult CreateByTypeFour(int? id)
        {
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            return View(_paper.SelectPaper(id, UserID));
        }
        [HttpPost]
        public ActionResult CreateByTypeFour(string PaperIDNum)
        {
            Guid PaperID = Guid.Parse(PaperIDNum);
            List<CEDTS_PaperAssessment> paList = _paper.SelectPaperAssessmentItem(PaperID);
            List<CEDTS_AssessmentItem> itList = new List<CEDTS_AssessmentItem>();
            foreach (var pa in paList)
            {
                Guid ItemID = Guid.Parse(pa.AssessmentItemID.ToString());
                CEDTS_AssessmentItem it = _paper.SelectAssessmentItem(ItemID);
                itList.Add(it);
            }
            taskpaper = MakePaper(itList, 4, null);
            if (taskpaper == null)
            {
                return Json("0");
            }
            else
            {
                return Json("1");
            }
        }

        public ActionResult CreateByTypeFive()
        {
            SelectItem.sspcList = new List<ExaminationItem>();
            SelectItem.slpoList = new List<ExaminationItem>();
            SelectItem.llpoList = new List<ExaminationItem>();
            SelectItem.rlpoList = new List<ExaminationItem>();
            SelectItem.lpcList = new List<ExaminationItem>();
            SelectItem.rpcList = new List<ExaminationItem>();
            SelectItem.rpoList = new List<ExaminationItem>();
            SelectItem.infoMatList = new List<ExaminationItem>();
            SelectItem.cpList = new List<ExaminationItem>();
            List<ExaminationItem> totalAssessmentItem = _paper.GetExaminationItemsByTeacher();
            foreach (var item in totalAssessmentItem)
            {
                switch (item.ItemID)
                {
                    case 1:
                        SelectItem.sspcList.Add(item);
                        break;
                    case 2:
                        SelectItem.slpoList.Add(item);
                        break;
                    case 3:
                        SelectItem.llpoList.Add(item);
                        break;
                    case 4:
                        SelectItem.rlpoList.Add(item);
                        break;
                    case 5:
                        SelectItem.lpcList.Add(item);
                        break;
                    case 6:
                        SelectItem.rpcList.Add(item);
                        break;
                    case 7:
                        SelectItem.rpoList.Add(item);
                        break;
                    case 8:
                        SelectItem.cpList.Add(item);
                        break;
                    case 9:
                        SelectItem.infoMatList.Add(item);
                        break;
                }
            }
            return View(SelectItem);
        }

        [HttpPost]
        public ActionResult CreateByTypeFive(string AssessmentID)
        {
            string[] AssessmentIDArray = AssessmentID.Split(',');
            List<CEDTS_AssessmentItem> AssessmentItem = new List<CEDTS_AssessmentItem>();
            foreach (var id in AssessmentIDArray)
            {
                AssessmentItem.Add(_paper.SelectAssessmentItem(Guid.Parse(id)));
            }
            taskpaper = MakePaper(AssessmentItem, 5, null);
            if (taskpaper == null)
            {
                return Json("0");
            }
            else
            {
                return Json("1");
            }
        }

        public ActionResult SspcList(int? id)
        {
            if (id == null)
            {
                if (TempData["SspcList"] != null)
                {
                    id = int.Parse(TempData["SspcList"].ToString());
                    TempData["SspcList"] = id;
                }
                return View(_paper.AssessmentItemPaged(id, SelectItem.sspcList));
            } 
            else
            {
                TempData["SspcList"] = id;
                return RedirectToAction("CreateByTypeFive");
            }
            
        }
        public ActionResult SlpoList(int? id)
        {
            if (id == null)
            {
                if (TempData["SlpoList"] != null)
                {
                    id = int.Parse(TempData["SlpoList"].ToString());
                    TempData["SlpoList"] = id;
                }
                return View(_paper.AssessmentItemPaged(id, SelectItem.slpoList));
            }
            else
            {
                TempData["SlpoList"] = id;
                return RedirectToAction("CreateByTypeFive");
            }
        }

        public ActionResult LlpoList(int? id)
        {
            if (id == null)
            {
                if (TempData["LlpoList"] != null)
                {
                    id = int.Parse(TempData["LlpoList"].ToString());
                    TempData["LlpoList"] = id;
                }
                return View(_paper.AssessmentItemPaged(id, SelectItem.slpoList));
            }
            else
            {
                TempData["LlpoList"] = id;
                return RedirectToAction("CreateByTypeFive");
            }
        }

        public ActionResult RlpoList(int? id)
        {
            if (id == null)
            {
                if (TempData["RlpoList"] != null)
                {
                    id = int.Parse(TempData["RlpoList"].ToString());
                    TempData["RlpoList"] = id;
                }
                return View(_paper.AssessmentItemPaged(id, SelectItem.slpoList));
            }
            else
            {
                TempData["RlpoList"] = id;
                return RedirectToAction("CreateByTypeFive");
            }
        }

        public ActionResult LpcList(int? id)
        {
            if (id == null)
            {
                if (TempData["LpcList"] != null)
                {
                    id = int.Parse(TempData["LpcList"].ToString());
                    TempData["LpcList"] = id;
                }
                return View(_paper.AssessmentItemPaged(id, SelectItem.slpoList));
            }
            else
            {
                TempData["LpcList"] = id;
                return RedirectToAction("CreateByTypeFive");
            }
        }

        public ActionResult RpcList(int? id)
        {
            if (id == null)
            {
                if (TempData["RpcList"] != null)
                {
                    id = int.Parse(TempData["RpcList"].ToString());
                    TempData["RpcList"] = id;
                }
                return View(_paper.AssessmentItemPaged(id, SelectItem.slpoList));
            }
            else
            {
                TempData["RpcList"] = id;
                return RedirectToAction("CreateByTypeFive");
            }
        }

        public ActionResult RpoList(int? id)
        {
            if (id == null)
            {
                if (TempData["RpoList"] != null)
                {
                    id = int.Parse(TempData["RpoList"].ToString());
                    TempData["RpoList"] = id;
                }
                return View(_paper.AssessmentItemPaged(id, SelectItem.slpoList));
            }
            else
            {
                TempData["RpoList"] = id;
                return RedirectToAction("CreateByTypeFive");
            }
        }

        public ActionResult CpList(int? id)
        {
            if (id == null)
            {
                if (TempData["CpList"] != null)
                {
                    id = int.Parse(TempData["CpList"].ToString());
                    TempData["CpList"] = id;
                }
                return View(_paper.AssessmentItemPaged(id, SelectItem.slpoList));
            }
            else
            {
                TempData["CpList"] = id;
                return RedirectToAction("CreateByTypeFive");
            }
        }

        public ActionResult DetailItem(Guid ItemID)
        {
            ViewData["AssessmentItemID"] = ItemID;
            return View();
        }

        public ActionResult GetAssessmentItem(Guid id)
        {
            AssessmentType AssessmentItem = new AssessmentType();
            CEDTS_AssessmentItem Item = _paper.SelectAssessmentItem(id);
            List<CEDTS_Question> QuestionList = _paper.SelectQuestion(id);
            AssessmentItem.ItemType = Item.ItemTypeID.Value;
            #region 快速阅读理解赋值
            if (Item.ItemTypeID == 1)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                sspc.Choices = new List<string>();

                sspc.Content = Item.Original;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();
                int TermNum = 0;

                foreach (var question in QuestionList)
                {                
                    info.QuestionID.Add(question.QuestionID);                    
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(question.Answer);
                    info.Tip.Add(question.Analyze);
                    if (question.ChooseA != "")
                    {
                        sspc.Choices.Add(question.ChooseA);
                        sspc.Choices.Add(question.ChooseB);
                        sspc.Choices.Add(question.ChooseC);
                        sspc.Choices.Add(question.ChooseD);
                    }
                    else
                    {
                        TermNum++;
                    }

                    tempListQuestion.Add(question);
                }
                info.QuestionCount = tempListQuestion.Count;
                sspc.TermNum = TermNum;
                sspc.ChoiceNum = info.QuestionCount - TermNum;
                sspc.Info = info;
                AssessmentItem.Sspc = sspc;
            }
            #endregion

            #region 短对话听力赋值
            if (Item.ItemTypeID == 2)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                Listen listen = new Listen();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.QustionInterval = Item.Interval.ToString();
                listen.Choices = new List<string>();

                listen.Script = Item.Original;
                listen.SoundFile = Item.SoundFile;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();

                foreach (var question in QuestionList)
                {
                    info.QuestionID.Add(question.QuestionID);
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(question.Answer);
                    info.Tip.Add(question.Analyze);
                    listen.Choices.Add(question.ChooseA);
                    listen.Choices.Add(question.ChooseB);
                    listen.Choices.Add(question.ChooseC);
                    listen.Choices.Add(question.ChooseD);

                    tempListQuestion.Add(question);
                }
                info.QuestionCount = tempListQuestion.Count;
                listen.Info = info;
                AssessmentItem.Slpo = listen;
            }
            #endregion

            #region 长对话听力赋值
            if (Item.ItemTypeID == 3)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                Listen listen = new Listen();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.QustionInterval = Item.Interval.ToString();
                info.timeInterval = new List<int>();
                info.questionSound = new List<string>();
                listen.Choices = new List<string>();

                listen.Script = Item.Original;
                listen.SoundFile = Item.SoundFile;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();
                foreach (var question in QuestionList)
                {
                    info.QuestionID.Add(question.QuestionID);                   
                    info.questionSound.Add(question.Sound);
                    info.timeInterval.Add(question.Interval.Value);
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(question.Answer);
                    info.Tip.Add(question.Analyze);
                    listen.Choices.Add(question.ChooseA);
                    listen.Choices.Add(question.ChooseB);
                    listen.Choices.Add(question.ChooseC);
                    listen.Choices.Add(question.ChooseD);

                    tempListQuestion.Add(question);
                }
                info.QuestionCount = tempListQuestion.Count;
                listen.Info = info;
                AssessmentItem.Llpo = listen;
            }
            #endregion

            #region 短文理解听力赋值
            if (Item.ItemTypeID == 4)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                Listen listen = new Listen();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.QustionInterval = Item.Interval.ToString();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                listen.Choices = new List<string>();

                listen.Script = Item.Original;
                listen.SoundFile = Item.SoundFile;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();
                foreach (var question in QuestionList)
                {
                    info.QuestionID.Add(question.QuestionID);                    
                    info.questionSound.Add(question.Sound);
                    info.timeInterval.Add(question.Interval.Value);
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(question.Answer);
                    info.Tip.Add(question.Analyze);
                    listen.Choices.Add(question.ChooseA);
                    listen.Choices.Add(question.ChooseB);
                    listen.Choices.Add(question.ChooseC);
                    listen.Choices.Add(question.ChooseD);

                    tempListQuestion.Add(question);
                }
                info.QuestionCount = tempListQuestion.Count;
                listen.Info = info;
                AssessmentItem.Rlpo = listen;
            }
            #endregion

            #region 复合型听力赋值
            if (Item.ItemTypeID == 5)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                Listen listen = new Listen();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.QustionInterval = Item.Interval.ToString();
                listen.Choices = new List<string>();

                listen.Script = Item.Original;
                listen.SoundFile = Item.SoundFile;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();
                foreach (var question in QuestionList)
                {                   
                    info.QuestionID.Add(question.QuestionID);                    
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(question.Answer);
                    info.Tip.Add(question.Analyze);

                    tempListQuestion.Add(question);
                }
                info.QuestionCount = tempListQuestion.Count;
                listen.Info = info;
                AssessmentItem.Lpc = listen;
            }
            #endregion

            #region 阅读理解选词填空赋值
            if (Item.ItemTypeID == 6)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                ReadingPartCompletion rpc = new ReadingPartCompletion();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);
                CEDTS_Expansion es = _paper.SelectExpansion(Item.AssessmentItemID);
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();

                rpc.Content = Item.Original;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();
                foreach (var question in QuestionList)
                {
                    string Value = _paper.AnswerValue(info.ItemID, question.Answer);                    
                    info.QuestionID.Add(question.QuestionID);                    
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(Value);
                    info.Tip.Add(question.Analyze);

                    tempListQuestion.Add(question);
                }
                rpc.WordList = new List<string>();
                rpc.WordList.Add(es.ChoiceA);
                rpc.WordList.Add(es.ChoiceB);
                rpc.WordList.Add(es.ChoiceC);
                rpc.WordList.Add(es.ChoiceD);
                rpc.WordList.Add(es.ChoiceE);
                rpc.WordList.Add(es.ChoiceF);
                rpc.WordList.Add(es.ChoiceG);
                rpc.WordList.Add(es.ChoiceH);
                rpc.WordList.Add(es.ChoiceI);
                rpc.WordList.Add(es.ChoiceJ);
                rpc.WordList.Add(es.ChoiceH);
                rpc.WordList.Add(es.ChoiceK);
                rpc.WordList.Add(es.ChoiceL);
                rpc.WordList.Add(es.ChoiceN);
                rpc.WordList.Add(es.ChoiceM);
                rpc.WordList.Add(es.ChoiceO);
                info.QuestionCount = tempListQuestion.Count;
                rpc.Info = info;
                AssessmentItem.Rpc = rpc;
            }

            #endregion

            #region 阅读理解选择题型赋值
            if (Item.ItemTypeID == 7)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                ReadingPartOption rpo = new ReadingPartOption();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);                
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                rpo.Choices = new List<string>();
                rpo.Content = Item.Original;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();
                foreach (var question in QuestionList)
                {
                    info.QuestionID.Add(question.QuestionID);                    
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(question.Answer);
                    info.Tip.Add(question.Analyze);
                    rpo.Choices.Add(question.ChooseA);
                    rpo.Choices.Add(question.ChooseB);
                    rpo.Choices.Add(question.ChooseC);
                    rpo.Choices.Add(question.ChooseD);

                    tempListQuestion.Add(question);
                }
                info.QuestionCount = tempListQuestion.Count;
                rpo.Info = info;
                AssessmentItem.Rpo = rpo;
            }
            #endregion

            #region 阅读理解选词填空赋值
            if (Item.ItemTypeID == 9)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                InfoMatchingCompletion infoMat = new InfoMatchingCompletion();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);
                CEDTS_Expansion es = _paper.SelectExpansion(Item.AssessmentItemID);
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();

                infoMat.Content = Item.Original;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();
                foreach (var question in QuestionList)
                {
                    string Value = _paper.AnswerValue(info.ItemID, question.Answer);
                    info.QuestionID.Add(question.QuestionID);
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(Value);
                    info.Tip.Add(question.Analyze);

                    tempListQuestion.Add(question);
                }
                infoMat.WordList = new List<string>();
                infoMat.WordList.Add(es.ChoiceA);
                infoMat.WordList.Add(es.ChoiceB);
                infoMat.WordList.Add(es.ChoiceC);
                infoMat.WordList.Add(es.ChoiceD);
                infoMat.WordList.Add(es.ChoiceE);
                infoMat.WordList.Add(es.ChoiceF);
                infoMat.WordList.Add(es.ChoiceG);
                infoMat.WordList.Add(es.ChoiceH);
                infoMat.WordList.Add(es.ChoiceI);
                infoMat.WordList.Add(es.ChoiceJ);
                infoMat.WordList.Add(es.ChoiceH);
                infoMat.WordList.Add(es.ChoiceK);
                infoMat.WordList.Add(es.ChoiceL);
                infoMat.WordList.Add(es.ChoiceN);
                infoMat.WordList.Add(es.ChoiceM);
                infoMat.WordList.Add(es.ChoiceO);
                info.QuestionCount = tempListQuestion.Count;
                infoMat.Info = info;
                AssessmentItem.InfMat = infoMat;
            }

            #endregion

            #region 完型填空赋值
            if (Item.ItemTypeID == 8)
            {
                var tempListQuestion = new List<CEDTS_Question>();
                ClozePart cp = new ClozePart();
                CEDTS_ItemType it = _paper.SelectItemType((int)Item.ItemTypeID);
                ItemBassInfo info = new ItemBassInfo();
                info.UserAnswer = new List<string>();
                info.QuestionID = new List<Guid>();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                cp.Choices = new List<string>();

                cp.Content = Item.Original;
                info.QuestionCount = (int)Item.QuestionCount;
                info.Score = (double)Item.Score;
                info.ReplyTime = (int)Item.Duration;
                info.ItemID = (Guid)Item.AssessmentItemID;
                info.Count = (int)Item.Count;
                info.Diffcult = (double)Item.Difficult;
                info.ItemType = it.ItemTypeID.ToString();
                info.ItemType_CN = it.TypeName_CN;
                info.PartType = it.PartTypeID.ToString();
                foreach (var question in QuestionList)
                {                    
                    info.QuestionID.Add(question.QuestionID);                    
                    info.ScoreQuestion.Add((double)question.Score);
                    info.TimeQuestion.Add((int)question.Duration);
                    info.DifficultQuestion.Add((double)question.Difficult);
                    info.Problem.Add(question.QuestionContent);
                    info.AnswerValue.Add(question.Answer);
                    info.Tip.Add(question.Analyze);
                    cp.Choices.Add(question.ChooseA);
                    cp.Choices.Add(question.ChooseB);
                    cp.Choices.Add(question.ChooseC);
                    cp.Choices.Add(question.ChooseD);

                    tempListQuestion.Add(question);
                }
                info.QuestionCount = tempListQuestion.Count;
                cp.Info = info;
                AssessmentItem.Cp = cp;
            }
            #endregion
            return Json(AssessmentItem);
        }

        public ActionResult SelectKnowledge(string uperID)
        {
            List<CEDTS_KnowledgePoints> data = new List<CEDTS_KnowledgePoints>();
            if (uperID == "")
            {
                data = _paper.SelectKnowledges(1, null);
            }
            else
            {
                data = _paper.SelectKnowledges(2, Guid.Parse(uperID.ToString()));
            }
            List<KnowledgeInfo> kiList2 = new List<KnowledgeInfo>();
            for (int i = 0; i < data.Count; i++)
            {
                KnowledgeInfo ki = new KnowledgeInfo();
                ki.KnowledgeID = data[i].KnowledgePointID;
                ki.Title = data[i].Title;
                kiList2.Add(ki);
            }
            return Json(kiList2);
        }

        public PaperTotal MakePaper(List<CEDTS_AssessmentItem> aiList, int type, List<string> kk)
        {
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            DateTime dt = DateTime.Now;
            List<Guid> KnowledgeIDList = new List<Guid>();
            if (kk != null)
            {
                for (int i = 0; i < kk.Count; i++)
                {
                    string[] s = kk[i].Split(',');
                    for (int j = 0; j < s.Length; j++)
                    {
                        if (!KnowledgeIDList.Contains(Guid.Parse(s[j])))
                        {
                            KnowledgeIDList.Add(Guid.Parse(s[j]));
                        }
                    }
                }
            }

            PaperTotal pt = new PaperTotal();
            List<SkimmingScanningPartCompletion> sspcList = new List<SkimmingScanningPartCompletion>();//临时存储快速阅读题型
            List<Listen> slpoList = new List<Listen>();//临时存储短对话听力题型
            List<Listen> llpoList = new List<Listen>();//临时存储长对话听力题型
            List<Listen> rlpoList = new List<Listen>();//临时存储听力理解题型
            List<Listen> lpcList = new List<Listen>();//临时存储复合型听力题型
            List<ReadingPartCompletion> rpcList = new List<ReadingPartCompletion>();//临时存储阅读理解选词填空题型
            List<ReadingPartOption> rpoList = new List<ReadingPartOption>();//临时存储阅读理解选择题型
            List<InfoMatchingCompletion> infoMatList = new List<InfoMatchingCompletion>();//临时存储阅读理解信息匹配题型
            List<ClozePart> cpList = new List<ClozePart>();//临时存储完型填空题型


            #region 保存试卷信息
            CEDTS_Paper paper = new CEDTS_Paper();
            double Score = 0;
            int Duration = 0;
            double Difficult = 0;
            foreach (var ai in aiList)
            {
                Score += (double)ai.Score;
                Duration += (int)ai.Duration;
                Difficult += (double)ai.Difficult;
            }
            string typeName = string.Empty;
            switch (type)
            {
                case 1:
                    {
                        typeName = "按题型组卷";
                        break;
                    }
                case 2:
                    {
                        typeName = "按知识点组卷";
                        break;
                    }                
                case 3:
                    {
                        typeName = "四级真题";
                        break;
                    }
                case 4:
                    {
                        typeName = "选择试卷";
                        break;
                    }
                default:
                    {
                        typeName = "自主选题";
                        break;
                    }
            }

            
            if (taskpaper.Title == null)
            {
                paper.Title = typeName;
                taskpaper.Title = typeName;
            }
            else
            {
                paper.Title = taskpaper.Title;
            }
            paper.Type = typeName;
            paper.PaperID = Guid.NewGuid();
            paper.Score = Score;
            paper.Duration = Duration;
            paper.Difficult = ((int)((Difficult / aiList.Count) * 100)) / 100;
            paper.Description = "用户：" + User.Identity.Name + "在" + DateTime.Now + "发布作业：" + typeName;
            paper.CreateTime = DateTime.Now;
            paper.UpdateTime = DateTime.Now;
            paper.UserID = UserID;
            paper.UpdateUserID = UserID;
            if (type == 1 || type == 2 || type == 3)
            {
                paper.State = 8;
            } 
            else
            {
                paper.State = 10;
            }
            //4代表是老师布置作业来自系统题库
            _paper.SavePaper(paper);
            #endregion

            #region 保存试卷与班级的对应信息
            CEDTS_PaperAssignClass AssignRecord = new CEDTS_PaperAssignClass();
            AssignRecord.ClassID = ClassID;
            AssignRecord.PaperID = paper.PaperID;
            AssignRecord.UserID = UserID;
            AssignRecord.CreateTime = DateTime.Now;
            _paper.SavePaperAssignClass(AssignRecord);
            #endregion


            int Weight = 0;
            double totaltime = 0.0;
            double totalscore = 0.0;
            double totaldifficoult = 0.0;
            foreach (var ai in aiList)
            {
                #region 保存试题和试卷的关系
                Weight++;
                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                pa.AssessmentItemID = ai.AssessmentItemID;
                pa.PaperID = paper.PaperID;
                pa.ItemTypeID = ai.ItemTypeID;
                if (pa.ItemTypeID == 1)
                {
                    pa.PartTypeID = 1;
                }
                if (pa.ItemTypeID == 2 || pa.ItemTypeID == 3 || pa.ItemTypeID == 4 || pa.ItemTypeID == 5)
                {
                    pa.PartTypeID = 2;
                }
                if (pa.ItemTypeID == 6 || pa.ItemTypeID == 7 || pa.ItemTypeID == 9)
                {
                    pa.PartTypeID = 3;
                }
                if (pa.ItemTypeID == 8)
                {
                    pa.PartTypeID = 4;
                }
                pa.Weight = Weight;
                _paper.SavePaperAssessment(pa);
                #endregion

                #region 保存出题次数
                CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                UAC.UserID = UserID;
                if (_paper.SelectUA(UserID, ai.AssessmentItemID) == 2)
                {
                    UAC.AssessmentItemID = _paper.SelectUAC(UserID, ai.AssessmentItemID).AssessmentItemID;
                    UAC.UserAssessmentCountID = _paper.SelectUAC(UserID, ai.AssessmentItemID).UserAssessmentCountID;
                    UAC.Count = _paper.SelectUAC(UserID, ai.AssessmentItemID).Count + 1;
                    _paper.UpdataUAC(UAC);
                }
                else
                {
                    UAC.AssessmentItemID = ai.AssessmentItemID;
                    UAC.Count = 1;
                    _paper.CreateUAC(UAC);
                }
                #endregion

                #region 快速阅读理解赋值
                if (ai.ItemTypeID == 1)
                {
                    SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        if (questionList.Count == 0)
                        {
                            questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                        }
                    }
                    else
                    {
                        questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    }
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();
                    sspc.Choices = new List<string>();

                    sspc.Content = ai.Original;
                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    int TermNum = 0;
                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                        if (question.ChooseA != "" && question.ChooseA != null)
                        {
                            sspc.Choices.Add(question.ChooseA);
                            sspc.Choices.Add(question.ChooseB);
                            sspc.Choices.Add(question.ChooseC);
                            sspc.Choices.Add(question.ChooseD);
                        }
                        else
                        {
                            TermNum++;
                        }

                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    sspc.TermNum = TermNum;
                    sspc.ChoiceNum = info.QuestionCount - TermNum;
                    sspc.Info = info;
                    sspcList.Add(sspc);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }
                #endregion

                #region 短对话听力赋值
                if (ai.ItemTypeID == 2)
                {
                    Listen listen = new Listen();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        if (questionList.Count == 0)
                        {
                            questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                        }
                    }
                    else
                    {
                        questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    }
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();
                    info.questionSound = new List<string>();
                    info.timeInterval = new List<int>();
                    info.QustionInterval = ai.Interval.ToString();
                    listen.Choices = new List<string>();

                    listen.Script = ai.Original;
                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.timeInterval.Add(0);
                        info.questionSound.Add(question.Sound);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                        listen.Choices.Add(question.ChooseA);
                        listen.Choices.Add(question.ChooseB);
                        listen.Choices.Add(question.ChooseC);
                        listen.Choices.Add(question.ChooseD);

                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    slpoList.Add(listen);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }
                #endregion

                #region 长对话听力赋值
                if (ai.ItemTypeID == 3)
                {
                    Listen listen = new Listen();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        if (questionList.Count == 0)
                        {
                            questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                        }
                    }
                    else
                    {
                        questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    }
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();
                    info.questionSound = new List<string>();
                    info.timeInterval = new List<int>();
                    info.QustionInterval = ai.Interval.ToString();
                    listen.Choices = new List<string>();

                    listen.Script = ai.Original;
                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                        info.timeInterval.Add(question.Interval.Value);
                        info.questionSound.Add(question.Sound);
                        listen.Choices.Add(question.ChooseA);
                        listen.Choices.Add(question.ChooseB);
                        listen.Choices.Add(question.ChooseC);
                        listen.Choices.Add(question.ChooseD);
                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    llpoList.Add(listen);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }
                #endregion

                #region 短文理解听力赋值
                if (ai.ItemTypeID == 4)
                {
                    Listen listen = new Listen();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        if (questionList.Count == 0)
                        {
                            questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                        }
                    }
                    else
                    {
                        questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    }
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();
                    info.questionSound = new List<string>();
                    info.timeInterval = new List<int>();
                    info.QustionInterval = ai.Interval.ToString();
                    listen.Choices = new List<string>();

                    listen.Script = ai.Original;
                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                        info.timeInterval.Add(question.Interval.Value);
                        info.questionSound.Add(question.Sound);
                        listen.Choices.Add(question.ChooseA);
                        listen.Choices.Add(question.ChooseB);
                        listen.Choices.Add(question.ChooseC);
                        listen.Choices.Add(question.ChooseD);

                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    rlpoList.Add(listen);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }
                #endregion

                #region 复合型听力赋值
                if (ai.ItemTypeID == 5)
                {
                    Listen listen = new Listen();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    var allq = _paper.SelectQuestion(ai.AssessmentItemID);
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        listen.Script = ai.Original;
                        foreach (var q in allq)
                        {
                            if (questionList.Contains(q))
                            {
                                continue;
                            }
                            string script = "(_" + q.Order + "_)";
                            listen.Script = listen.Script.Replace(script, "(_<span style='color:Red'>" + q.Answer + "</span>_)");
                        }
                        if (questionList.Count == 0)
                        {
                            questionList = allq;
                            listen.Script = ai.Original;
                        }
                    }
                    else
                    {
                        questionList = allq;
                        listen.Script = ai.Original;
                    }
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();
                    info.questionSound = new List<string>();
                    info.timeInterval = new List<int>();
                    info.QustionInterval = ai.Interval.ToString();
                    listen.Choices = new List<string>();


                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();

                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.timeInterval.Add(0);
                        info.questionSound.Add(question.Sound);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);


                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    lpcList.Add(listen);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }
                #endregion

                #region 阅读理解选词填空赋值
                if (ai.ItemTypeID == 6)
                {
                    ReadingPartCompletion rpc = new ReadingPartCompletion();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    var allq = _paper.SelectQuestion(ai.AssessmentItemID);
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        rpc.Content = ai.Original;
                        foreach (var q in allq)
                        {
                            if (questionList.Contains(q))
                            {
                                continue;
                            }
                            string Content = "(_" + q.Order + "_)";
                            rpc.Content = rpc.Content.Replace(Content, "(_<span style='color:Red'>" + _paper.SelectAnswerValue(q.AssessmentItemID, q.Answer) + "</span>_)");
                        }
                        if (questionList.Count == 0)
                        {
                            questionList = allq;
                            rpc.Content = ai.Original;
                        }
                    }
                    else
                    {
                        questionList = allq;
                        rpc.Content = ai.Original;
                    }
                    CEDTS_Expansion es = _paper.SelectExpansion(ai.AssessmentItemID);
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();


                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();

                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);


                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    rpc.WordList = new List<string>();
                    rpc.WordList.Add(es.ChoiceA);
                    rpc.WordList.Add(es.ChoiceB);
                    rpc.WordList.Add(es.ChoiceC);
                    rpc.WordList.Add(es.ChoiceD);
                    rpc.WordList.Add(es.ChoiceE);
                    rpc.WordList.Add(es.ChoiceF);
                    rpc.WordList.Add(es.ChoiceG);
                    rpc.WordList.Add(es.ChoiceH);
                    rpc.WordList.Add(es.ChoiceI);
                    rpc.WordList.Add(es.ChoiceJ);
                    rpc.WordList.Add(es.ChoiceK);
                    rpc.WordList.Add(es.ChoiceL);
                    rpc.WordList.Add(es.ChoiceN);
                    rpc.WordList.Add(es.ChoiceM);
                    rpc.WordList.Add(es.ChoiceO);
                    rpc.Info = info;
                    rpcList.Add(rpc);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }

                #endregion

                #region 阅读理解选择题型赋值
                if (ai.ItemTypeID == 7)
                {
                    ReadingPartOption rpo = new ReadingPartOption();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        if (questionList.Count == 0)
                        {
                            questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                        }
                    }
                    else
                    {
                        questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    }
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();
                    rpo.Choices = new List<string>();


                    rpo.Content = ai.Original;
                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();

                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                        rpo.Choices.Add(question.ChooseA);
                        rpo.Choices.Add(question.ChooseB);
                        rpo.Choices.Add(question.ChooseC);
                        rpo.Choices.Add(question.ChooseD);

                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    rpo.Info = info;
                    rpoList.Add(rpo);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }
                #endregion

                #region 阅读理解信息匹配赋值
                if (ai.ItemTypeID == 9)
                {
                    InfoMatchingCompletion infoMat = new InfoMatchingCompletion();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    var allq = _paper.SelectQuestion(ai.AssessmentItemID);
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        infoMat.Content = ai.Original;
                        foreach (var q in allq)
                        {
                            if (questionList.Contains(q))
                            {
                                continue;
                            }
                            string Content = "(_" + q.Order + "_)";
                            infoMat.Content = infoMat.Content.Replace(Content, "(_<span style='color:Red'>" + _paper.SelectAnswerValue(q.AssessmentItemID, q.Answer) + "</span>_)");
                        }
                        if (questionList.Count == 0)
                        {
                            questionList = allq;
                            infoMat.Content = ai.Original;
                        }
                    }
                    else
                    {
                        questionList = allq;
                        infoMat.Content = ai.Original;
                    }
                    CEDTS_Expansion es = _paper.SelectExpansion(ai.AssessmentItemID);
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();


                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();

                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);


                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    infoMat.WordList = new List<string>();//阅读理解选词填空的词语
                    infoMat.WordList.Add(es.ChoiceA);
                    infoMat.WordList.Add(es.ChoiceB);
                    infoMat.WordList.Add(es.ChoiceC);
                    infoMat.WordList.Add(es.ChoiceD);
                    infoMat.WordList.Add(es.ChoiceE);
                    infoMat.WordList.Add(es.ChoiceF);
                    infoMat.WordList.Add(es.ChoiceG);
                    infoMat.WordList.Add(es.ChoiceH);
                    infoMat.WordList.Add(es.ChoiceI);
                    infoMat.WordList.Add(es.ChoiceJ);
                    infoMat.WordList.Add(es.ChoiceK);
                    infoMat.WordList.Add(es.ChoiceL);
                    infoMat.WordList.Add(es.ChoiceM);
                    infoMat.WordList.Add(es.ChoiceN);
                    infoMat.WordList.Add(es.ChoiceO);
                    infoMat.Info = info;
                    infoMatList.Add(infoMat);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }

                #endregion

                #region 完型填空赋值
                if (ai.ItemTypeID == 8)
                {
                    ClozePart cp = new ClozePart();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    var allq = _paper.SelectQuestion(ai.AssessmentItemID);
                    if (type == 2)
                    {
                        questionList = _paper.SelectQuestionByknowledge(ai.AssessmentItemID, KnowledgeIDList);
                        cp.Content = ai.Original;
                        foreach (var q in allq)
                        {
                            if (questionList.Contains(q))
                            {
                                continue;
                            }
                            string Content = "(_" + q.Order + "_)";
                            string value = string.Empty;
                            switch (q.Answer)
                            {
                                case "A": value = q.ChooseA; break;
                                case "B": value = q.ChooseB; break;
                                case "C": value = q.ChooseC; break;
                                case "D": value = q.ChooseD; break;
                                default: break;
                            }
                            cp.Content = cp.Content.Replace(Content, "(_<span style='color:Red'>" + value + "</span>_)");
                        }
                        if (questionList.Count == 0)
                        {
                            questionList = allq;
                            cp.Content = ai.Original;
                        }
                    }
                    else
                    {
                        questionList = allq;
                        cp.Content = ai.Original;
                    }
                    ItemBassInfo info = new ItemBassInfo();
                    info.QuestionID = new List<Guid>();
                    info.AnswerValue = new List<string>();
                    info.DifficultQuestion = new List<double>();
                    info.Knowledge = new List<string>();
                    info.KnowledgeID = new List<string>();
                    info.Problem = new List<string>();
                    info.ScoreQuestion = new List<double>();
                    info.TimeQuestion = new List<double>();
                    info.Tip = new List<string>();
                    cp.Choices = new List<string>();


                    info.QuestionCount = questionList.Count;
                    info.Score = 0.0;
                    info.ReplyTime = 0;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = 0.0;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();

                    int tempcount = 0;
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                        cp.Choices.Add(question.ChooseA);
                        cp.Choices.Add(question.ChooseB);
                        cp.Choices.Add(question.ChooseC);
                        cp.Choices.Add(question.ChooseD);

                        info.Diffcult += info.DifficultQuestion[tempcount];
                        info.Score += info.ScoreQuestion[tempcount];
                        info.ReplyTime += info.TimeQuestion[tempcount];
                        tempcount++;
                    }
                    info.Diffcult = (int)((info.Diffcult * 10) / info.QuestionCount) / 10;
                    cp.Info = info;
                    cpList.Add(cp);
                    totaldifficoult += info.Diffcult;
                    totalscore += info.Score;
                    totaltime += info.ReplyTime;
                }
                #endregion
            }
            paper.Score = totalscore;
            paper.Difficult = ((int)(totalscore * 10) / aiList.Count) / 10;
            paper.Duration = (int)totaltime;


            pt.PaperID = paper.PaperID;
            pt.UserID = paper.UserID.Value;
            pt.UpdateUserID = paper.UpdateUserID.Value;
            pt.Title = paper.Title;
            pt.Type = paper.Type;
            pt.Duration = paper.Duration.Value;
            pt.Difficult = paper.Difficult.Value;
            pt.Score = paper.Score.Value;
            pt.Description = paper.Description;
            pt.CreateTime = paper.CreateTime.Value;
            pt.SspcList = sspcList;
            pt.SlpoList = slpoList;
            pt.LlpoList = llpoList;
            pt.LpcList = lpcList;
            pt.RlpoList = rlpoList;
            pt.RpcList = rpcList;
            pt.RpoList = rpoList;
            pt.CpList = cpList;
            pt.InfMatList = infoMatList;

            #region 保存试卷XML到数据库
            paper.PaperContent = _paper.CreatePaperXML(pt);
            _paper.UpadatePaper(paper);
            #endregion

            TimeSpan ts = DateTime.Now - dt;

            return pt;
        }
    }
}
