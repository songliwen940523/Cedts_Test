using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Controllers
{
    [Authorize(Roles = "教师")]
    public class RemarkController : Controller
    {
        ICedts_TestRepository _test;
        ICedts_PaperRepository _paper;
        ICedts_UserRepository _icedts;
        public RemarkController(ICedts_TestRepository t, ICedts_PaperRepository p, ICedts_UserRepository i)
        {
            _test = t;
            _paper = p;
            _icedts = i;
        }
        public static Guid PartnerID = Guid.Empty;
        public static Guid MajorID = Guid.Empty;
        public static Guid GradeID = Guid.Empty;
        public static Guid ClassID = Guid.Empty;

        public ActionResult Index(int? id,int? num)
        {
            Guid cID = Guid.Empty;
            if (num == null)
            {
                num = 0;
            }

            List<SelectListItem> classIDList = new List<SelectListItem>();


            string UserName = User.Identity.Name;
            int UserID = _icedts.SelectUserByAccout(UserName).UserID;
            List<CEDTS_Class> ClassList = _icedts.GetClassbyUserID(UserID);
            for (int i = 0; i < ClassList.Count; i++)
            {
                classIDList.Add(new SelectListItem { Text = ClassList[i].ClassName, Value = ClassList[i].ClassID.ToString() });
            }
            //if (num != 0)
            //{
                classIDList[num.Value].Selected = true;
                cID = Guid.Parse(classIDList[num.Value].Value);
            //}

            ViewData["classIDList"] = classIDList;
            CEDTS_User Teacher = _icedts.SelectUserByAccout(User.Identity.Name);
            PartnerID = Teacher.PartnerID.Value;
           // MajorID = Teacher.MajorID.Value;
            //GradeID = Teacher.GradeID.Value;
            ClassID = cID;

            List<CEDTS_Test> testList = _test.SelectTest();

            List<Cedts_UserTest> utList = new List<Cedts_UserTest>();
            List<int> useridList = _icedts.SelectUser(null, ClassID).Where(p => p.State == true).Select(p => p.UserID).ToList(); ;

            foreach (var test in testList)
            {
                if (useridList.Contains(test.UserID.Value))
                {
                    Cedts_UserTest ut = new Cedts_UserTest();
                    ut.TestID = test.TestID;
                    ut.UserName = _test.GetUserName(test.UserID.Value);
                    ut.PaperID = test.PaperID;
                    ut.PaperTitle = _test.GetPaperTitle(test.PaperID);
                    ut.TotalScore = test.TotalScore.Value;
                    ut.Remark = test.Remark;
                    ut.IsChecked = test.IsChecked.Value;
                    utList.Add(ut);
                }
            }
            return View(utList.AsQueryable().ToPagedList(id ?? 1, 10));
        }

        public ActionResult Remark(Guid PaperID, Guid TestID, int type)
        {
            PaperTotal pt = new PaperTotal();
            List<SkimmingScanningPartCompletion> sspcList = new List<SkimmingScanningPartCompletion>();//临时存储快速阅读题型
            List<Listen> slpoList = new List<Listen>();//临时存储短对话听力题型
            List<Listen> llpoList = new List<Listen>();//临时存储长对话听力题型
            List<Listen> rlpoList = new List<Listen>();//临时存储听力理解题型
            List<Listen> lpcList = new List<Listen>();//临时存储复合型听力题型
            List<ReadingPartCompletion> rpcList = new List<ReadingPartCompletion>();//临时存储阅读理解选词填空题型
            List<ReadingPartOption> rpoList = new List<ReadingPartOption>();//临时存储阅读理解选择题型
            List<ClozePart> cpList = new List<ClozePart>();//临时存储完型填空题型

            List<CEDTS_PaperAssessment> paList = _paper.SelectPaperAssessmentItem(PaperID);
            CEDTS_Paper paper = _paper.SelectPaper(PaperID);

            List<CEDTS_TestAnswer> taList = _test.GetTestAnswer(TestID);

            foreach (var pa in paList)
            {
                Guid ItemID = Guid.Parse(pa.AssessmentItemID.ToString());//试题ID
                var relQuestion = taList.Where(p => p.AssessmentItemID == ItemID).Select(p => p.QuestionID).ToList();

                #region 快速阅读理解赋值
                if (pa.ItemTypeID == 1)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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

                    sspc.Content = ai.Original;
                    info.QuestionCount = (int)ai.QuestionCount;
                    info.Score = (double)ai.Score;
                    info.ReplyTime = (int)ai.Duration;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = (double)ai.Difficult;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    int TermNum = 0;

                    foreach (var question in questionList)
                    {
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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
                    sspcList.Add(sspc);
                }
                #endregion

                #region 短对话听力赋值
                if (pa.ItemTypeID == 2)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    info.QustionInterval = ai.Interval.ToString();
                    listen.Choices = new List<string>();

                    listen.Script = ai.Original;
                    info.QuestionCount = (int)ai.QuestionCount;
                    info.Score = (double)ai.Score;
                    info.ReplyTime = (int)ai.Duration;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = (double)ai.Difficult;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    foreach (var question in questionList)
                    {
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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
                    slpoList.Add(listen);
                }
                #endregion

                #region 长对话听力赋值
                if (pa.ItemTypeID == 3)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    info.QustionInterval = ai.Interval.ToString();
                    info.timeInterval = new List<int>();
                    info.questionSound = new List<string>();
                    listen.Choices = new List<string>();

                    listen.Script = ai.Original;
                    info.QuestionCount = (int)ai.QuestionCount;
                    info.Score = (double)ai.Score;
                    info.ReplyTime = (int)ai.Duration;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = (double)ai.Difficult;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    foreach (var question in questionList)
                    {
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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
                    llpoList.Add(listen);
                }
                #endregion

                #region 短文理解听力赋值
                if (pa.ItemTypeID == 4)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    info.QustionInterval = ai.Interval.ToString();
                    info.questionSound = new List<string>();
                    info.timeInterval = new List<int>();
                    listen.Choices = new List<string>();

                    listen.Script = ai.Original;
                    info.QuestionCount = (int)ai.QuestionCount;
                    info.Score = (double)ai.Score;
                    info.ReplyTime = (int)ai.Duration;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = (double)ai.Difficult;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    foreach (var question in questionList)
                    {
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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
                    rlpoList.Add(listen);
                }
                #endregion

                #region 复合型听力赋值
                if (pa.ItemTypeID == 5)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    info.QustionInterval = ai.Interval.ToString();
                    listen.Choices = new List<string>();

                    listen.Script = ai.Original;
                    info.QuestionCount = (int)ai.QuestionCount;
                    info.Score = (double)ai.Score;
                    info.ReplyTime = (int)ai.Duration;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = (double)ai.Difficult;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    foreach (var question in questionList)
                    {
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            string script = "(_" + question.Order + "_)";
                            listen.Script = listen.Script.Replace(script, "(_<span style='color:Red'>" + question.Answer + "</span>_)");
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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
                    lpcList.Add(listen);
                }
                #endregion

                #region 阅读理解选词填空赋值
                if (pa.ItemTypeID == 6)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    ReadingPartCompletion rpc = new ReadingPartCompletion();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
                    CEDTS_Expansion es = _paper.SelectExpansion(ItemID);
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

                    rpc.Content = ai.Original;
                    info.QuestionCount = (int)ai.QuestionCount;
                    info.Score = (double)ai.Score;
                    info.ReplyTime = (int)ai.Duration;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = (double)ai.Difficult;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    foreach (var question in questionList)
                    {
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            string Content = "(_" + question.Order + "_)";
                            rpc.Content = rpc.Content.Replace(Content, "(_<span style='color:Red'>" + question.Answer + "</span>_)");
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
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
                    rpcList.Add(rpc);
                }

                #endregion

                #region 阅读理解选择题型赋值
                if (pa.ItemTypeID == 7)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    ReadingPartOption rpo = new ReadingPartOption();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    rpo.Content = ai.Original;
                    info.QuestionCount = (int)ai.QuestionCount;
                    info.Score = (double)ai.Score;
                    info.ReplyTime = (int)ai.Duration;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = (double)ai.Difficult;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    foreach (var question in questionList)
                    {
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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
                    rpoList.Add(rpo);
                }
                #endregion

                #region 完型填空赋值
                if (pa.ItemTypeID == 8)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    ClozePart cp = new ClozePart();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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

                    cp.Content = ai.Original;
                    info.QuestionCount = (int)ai.QuestionCount;
                    info.Score = (double)ai.Score;
                    info.ReplyTime = (int)ai.Duration;
                    info.ItemID = (Guid)ai.AssessmentItemID;
                    info.Count = (int)ai.Count;
                    info.Diffcult = (double)ai.Difficult;
                    info.ItemType = it.ItemTypeID.ToString();
                    info.ItemType_CN = it.TypeName_CN;
                    info.PartType = it.PartTypeID.ToString();
                    foreach (var question in questionList)
                    {
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            string Content = "(_" + question.Order + "_)";
                            string value = string.Empty;
                            switch (question.Answer)
                            {
                                case "A": value = question.ChooseA; break;
                                case "B": value = question.ChooseB; break;
                                case "C": value = question.ChooseC; break;
                                case "D": value = question.ChooseD; break;
                                default: break;
                            }
                            cp.Content = cp.Content.Replace(Content, "(_<span style='color:Red'>" + value + "</span>_)");
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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
                    cpList.Add(cp);
                }
                #endregion
            }
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
            pt.PaperContent = paper.PaperContent;
            pt.SspcList = sspcList;
            pt.SlpoList = slpoList;
            pt.LlpoList = llpoList;
            pt.LpcList = lpcList;
            pt.RlpoList = rlpoList;
            pt.RpcList = rpcList;
            pt.RpoList = rpoList;
            pt.CpList = cpList;
            this.TempData["pt"] = pt;
            ViewData["testID"] = TestID.ToString();
            TempData["content"] = _test.GetTest(TestID).Remark;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Remark(FormCollection form)
        {
            this.ValidateRequest = false;
            Guid testID = Guid.Parse(form["testid"]);
            string content = form["myEditor"];
            content = content.Substring(3, content.LastIndexOf('p') - 5);
            CEDTS_Test test = _test.GetTest(testID);
            test.Remark = content;
            test.IsChecked = 1;
            _test.UpdateTest(test);
            return RedirectToAction("Index");
        }

        public ActionResult GetDate()
        {
            PaperTotal pt = this.TempData["pt"] as PaperTotal;
            return Json(pt);
        }


    }
}
