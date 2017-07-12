using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Models;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using System.Transactions;
using System.IO;
using System.Configuration;

namespace Cedts_Test.Controllers
{
    [Authorize]
    public class PaperShowController : Controller
    {
        public static PaperTotal pp = new PaperTotal();
        public static PaperTotalContinue ppc = new PaperTotalContinue();
        public static double ValueA = double.Parse(ConfigurationManager.AppSettings["ValueA"]);
        public static string CeType5Title = string.Empty;
        public static string Continues = string.Empty;
        ICedts_PaperRepository _paper;
        public PaperShowController(ICedts_PaperRepository r)
        {
            _paper = r;
        }

        [Filter.LogFilter(Description = "查看试卷列表")]
        public ActionResult CeType1(int? id)
        {
            return View(_paper.SelectPapers(id));
        }

        [Authorize(Roles = "普通用户")]
        public ActionResult SelectHowtoCreat()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SelectHowtoCreat(FormCollection form)
        {
            TempData["Title"] = form["Title"];
            int action = int.Parse(form["action"]);
            switch (action)
            {
                case 1:
                    {
                        return RedirectToAction("Index");
                    }
                case 2:
                    {
                        return RedirectToAction("ShowPaperByItemType");
                    }
                case 3:
                    {
                        return RedirectToAction("ShowPaperByRand");
                    }
                case 4:
                    {
                        return RedirectToAction("ShowPaperByKnowledge");
                    }
                case 5:
                    {
                        return RedirectToAction("ShowPaperByBadKnowledge");
                    }
                case 6:
                    {
                        return RedirectToAction("ShowPaperByKnowledgeCover");
                    }
                default:
                    {
                        return View();
                    }
            }
        }

        [Filter.LogFilter(Description = "试卷练习")]
        public ActionResult Exerciser(Guid id)
        {
            Guid PID = Guid.Empty;
            if (_paper.SelectPaperCount(id) == Guid.Empty)
            {
                PID = id;
            }
            else
            {
                PID = _paper.SelectPaperCount(id);
            }
            PaperTotal pt = new PaperTotal();
            List<SkimmingScanningPartCompletion> sspcList = new List<SkimmingScanningPartCompletion>();//临时存储快速阅读题型
            List<Listen> slpoList = new List<Listen>();//临时存储短对话听力题型
            List<Listen> llpoList = new List<Listen>();//临时存储长对话听力题型
            List<Listen> rlpoList = new List<Listen>();//临时存储听力理解题型
            List<Listen> lpcList = new List<Listen>();//临时存储复合型听力题型
            List<ReadingPartCompletion> rpcList = new List<ReadingPartCompletion>();//临时存储阅读理解选词填空题型
            List<ReadingPartOption> rpoList = new List<ReadingPartOption>();//临时存储阅读理解选择题型
            List<InfoMatchingCompletion> infoMatList = new List<InfoMatchingCompletion>();
            List<ClozePart> cpList = new List<ClozePart>();//临时存储完型填空题型

            List<CEDTS_PaperAssessment> paList = _paper.SelectPaperAssessmentItem(PID);
            CEDTS_Paper paper = _paper.SelectPaper(PID);

            foreach (var pa in paList)
            {
                Guid ItemID = Guid.Parse(pa.AssessmentItemID.ToString());//试题ID

                #region 快速阅读理解赋值
                if (pa.ItemTypeID == 1)
                {
                    SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
                    sspc.TermNum = TermNum;
                    sspc.ChoiceNum = info.QuestionCount - TermNum;
                    sspc.Info = info;
                    sspcList.Add(sspc);
                }
                #endregion

                #region 短对话听力赋值
                if (pa.ItemTypeID == 2)
                {
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                        info.QuestionID.Add(question.QuestionID);
                        //info.timeInterval.Add(question.Interval.Value);
                        //info.questionSound.Add(question.Sound);
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
                    }
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    slpoList.Add(listen);
                }
                #endregion

                #region 长对话听力赋值
                if (pa.ItemTypeID == 3)
                {
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    llpoList.Add(listen);
                }
                #endregion

                #region 短文理解听力赋值
                if (pa.ItemTypeID == 4)
                {
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    rlpoList.Add(listen);
                }
                #endregion

                #region 复合型听力赋值
                if (pa.ItemTypeID == 5)
                {
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                        info.QuestionID.Add(question.QuestionID);
                        //info.timeInterval.Add(question.Interval.Value);
                        //info.questionSound.Add(question.Sound);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                    }
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    lpcList.Add(listen);
                }
                #endregion

                #region 阅读理解选词填空赋值
                if (pa.ItemTypeID == 6)
                {
                    ReadingPartCompletion rpc = new ReadingPartCompletion();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
                    CEDTS_Expansion es = _paper.SelectExpansion(ItemID);
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
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
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
                    rpc.Info = info;
                    rpcList.Add(rpc);
                }

                #endregion

                #region 阅读理解选择题型赋值
                if (pa.ItemTypeID == 7)
                {
                    ReadingPartOption rpo = new ReadingPartOption();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
                    rpo.Info = info;
                    rpoList.Add(rpo);
                }
                #endregion

                #region 信息匹配赋值
                if (pa.ItemTypeID == 9)
                {
                    InfoMatchingCompletion infoMat = new InfoMatchingCompletion();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
                    CEDTS_Expansion es = _paper.SelectExpansion(ItemID);
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

                    infoMat.Content = ai.Original;
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
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
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
                    infoMat.Info = info;
                    infoMatList.Add(infoMat);
                }

                #endregion

                #region 完型填空赋值
                if (pa.ItemTypeID == 8)
                {
                    ClozePart cp = new ClozePart();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
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
            pt.InfMatList = infoMatList;
            pt.CpList = cpList;
            pp = pt;
            return RedirectToAction("PaperShow");
        }

        [Filter.LogFilter(Description = "随机组卷练习")]
        public ActionResult ShowPaperByRand()
        {
            string[] Counts = { "1", "8", "2", "3", "1", "1", "2", "1", "1" };
            List<string> CountList = Counts.ToList();
            int userID = _paper.SelectUserID(User.Identity.Name);
            pp = MakePaper(_paper.SelectAssessmentItems(CountList, userID), 3, null);
            return RedirectToAction("PaperShow");
        }


        public ActionResult ShowPaperByItemType()
        {
            return View(_paper.SelectPartType());
        }

        [Filter.LogFilter(Description = "题型组卷练习")]
        [HttpPost]
        public ActionResult ShowPaperByItemType(int Sspc, int Slpo, int Llpo, int Rlpo, int Lpc, int Rpo, int Rpc, int InfoMat, int Cp, string Title)
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
            Number.Add(InfoMat.ToString());
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
            TempData["CeType2Title"] = Title;
            int userID = _paper.SelectUserID(User.Identity.Name);
            pp = MakePaper(_paper.SelectAssessmentItems(CountList, userID), 1, null);
            return Json("");
        }

        /// <summary>
        /// 题型数量设置，根据PartTypeID获取ItemType信息
        /// </summary>
        /// <param name="PartTypeID">题目大类型ID</param>
        /// <returns>返回ItemType表的局部视图</returns>
        [ChildActionOnly]
        public ActionResult ItemType(string PartTypeID)
        {
            return PartialView(_paper.SelectItemTypeByPartTypeID(int.Parse(PartTypeID)));
        }

        public ActionResult ShowPaperByKnowledge()
        {
            return View();
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

        [Filter.LogFilter(Description = "按照知识点组卷练习")]
        [HttpPost]
        public string ShowPaperByKnowledge(string select2, string select3, string Title3)
        {
            TempData["CeType3Title"] = Title3;
            List<string> KnowledgeIDList = new List<string>();
            List<string> CountList = new List<string>();
            string[] s2 = select2.Split(',');
            string[] s3 = select3.Split(',');
            for (int i = 0; i < s2.Length - 1; i++)
            {
                KnowledgeIDList.Add(s2[i]);
                CountList.Add(s3[i]);
            }

            int userID = _paper.SelectUserID(User.Identity.Name);
            try
            {
                pp = MakePaper(_paper.SelectAssessmentItems2(KnowledgeIDList, CountList, userID), 2, KnowledgeIDList);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }


        public ActionResult ShowPaperByBadKnowledge()
        {
            return View();
        }

        [Filter.LogFilter(Description = "按知识点弱项组卷")]
        [HttpPost]
        public ActionResult ShowPaperByBadKnowledge(string Num, string Title)
        {
            TempData["CeType4Title"] = Title;
            int num = int.Parse(Num);
            List<string> numList = new List<string>();
            if (num > 0 && num <= 3)
            {
                numList.Add(num.ToString());
            }
            if (num > 3 && num <= 15)
            {
                double tempNum = num / 3;
                int Count = (int)Math.Floor(tempNum);
                for (int i = 0; i < Count; i++)
                {
                    numList.Add("3");
                }
                numList.Add((num - Count * 3).ToString());
            }
            if (num > 15)
            {
                if (num % 5 == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        numList.Add((num / 5).ToString());
                    }
                }
                else
                {
                    int temp = num % 5;
                    for (int i = 0; i < 5; i++)
                    {
                        numList.Add(((num - temp) / 5).ToString());
                    }
                    for (int j = 0; j < temp; j++)
                    {
                        numList[j] = (int.Parse(numList[j]) + 1).ToString();
                    }
                }
            }
            int userID = _paper.SelectUserID(User.Identity.Name);
            List<string> guidList = _paper.SelectBadKnowledge(userID, numList.Count);

            if (guidList.Count == 0)
            {
                numList.Clear();
                int temp = num / 8;
                if (temp < 1)
                {
                    for (int i = 0; i < num; i++)
                    {
                        numList.Add("1");
                    }
                }
                else
                {
                    int tempNum = num % 8;
                    for (int i = 0; i < 8; i++)
                    {
                        if (i < tempNum)
                            numList.Add((num / 8 + 1).ToString());
                        else
                            numList.Add((num / 8).ToString());
                    }
                }
                pp = MakePaper(_paper.SelectAssessmentItems(numList, userID), 4, null);
            }
            else
            {
                if (numList.Count != guidList.Count)
                {
                    numList.Clear();
                    if (num % guidList.Count == 0)
                    {
                        for (int i = 0; i < guidList.Count; i++)
                        {
                            numList.Add((num / guidList.Count).ToString());
                        }
                    }
                    else
                    {
                        int temp = num % guidList.Count;
                        for (int i = 0; i < guidList.Count; i++)
                        {
                            numList.Add(((num - temp) / guidList.Count).ToString());
                        }
                        for (int j = 0; j < temp; j++)
                        {
                            numList[j] = (int.Parse(numList[j]) + 1).ToString();
                        }
                    }
                }
                pp = MakePaper(_paper.SelectAssessmentItems3(guidList, numList, userID), 4, guidList);
            }
            return Json("");
        }

        /*[Filter.LogFilter(Description = "按得分最大化组卷")]
        public ActionResult ShowPaperByKnowledgeCover()
        {
            int num = 10;
            double P1 = 0.1567;
            double P2 = 0.3114;
            double P3 = 0.0758;
            double P4 = 0.2665;
            double P5 = 0.0669;
            double p1 = P1 / (P1 + P2 + P3 + P4 + P5);
            double p2 = P2 / (P1 + P2 + P3 + P4 + P5);
            double p3 = P3 / (P1 + P2 + P3 + P4 + P5);
            double p4 = P4 / (P1 + P2 + P3 + P4 + P5);
            double p5 = P5 / (P1 + P2 + P3 + P4 + P5);
            string k1 = "信息明示";
            string k2 = "同义转述";
            string k3 = "隐含意义推断";
            string k4 = "词义理解";
            string k5 = "词类判断";
            List<string> kk = new List<string>();
            string know1 = _paper.SelectKnowledgeIDs(k1);
            kk.Add(know1);
            string know2 = _paper.SelectKnowledgeIDs(k2);
            kk.Add(know2);
            string know3 = _paper.SelectKnowledgeIDs(k3);
            kk.Add(know3);
            string know4 = _paper.SelectKnowledgeIDs(k4);
            kk.Add(know4);
            string know5 = _paper.SelectKnowledgeIDs(k5);
            kk.Add(know5);
            int userID = _paper.SelectUserID(User.Identity.Name);
            double M1 = _paper.SelectKP_MasterRate(k1, userID);
            double M2 = _paper.SelectKP_MasterRate(k2, userID);
            double M3 = _paper.SelectKP_MasterRate(k3, userID);
            double M4 = _paper.SelectKP_MasterRate(k4, userID);
            double M5 = _paper.SelectKP_MasterRate(k5, userID);
            if (double.IsNaN(M1))
            {
                M1 = 0.0;
            }
            if (double.IsNaN(M2))
            {
                M2 = 0.0;
            }
            if (double.IsNaN(M3))
            {
                M3 = 0.0;
            }
            if (double.IsNaN(M4))
            {
                M4 = 0.0;
            }
            if (double.IsNaN(M5))
            {
                M5 = 0.0;
            }
            int n1 = (int)Math.Floor(p1 * num * (1 - M1) * 2);
            int n2 = (int)Math.Floor(p2 * num * (1 - M2) * 2);
            int n3 = (int)Math.Floor(p3 * num * (1 - M3) * 2);
            int n4 = (int)Math.Floor(p4 * num * (1 - M4) * 2);
            int n5 = (int)Math.Floor(p5 * num * (1 - M5) * 2);
            int n = n1 + n2 + n3 + n4 + n5;
            List<string> countList = new List<string>();

            #region 第一次求出的题目数量刚好
            if (n == num)
            {
                countList.Add(n1.ToString());
                countList.Add(n2.ToString());
                countList.Add(n3.ToString());
                countList.Add(n4.ToString());
                countList.Add(n5.ToString());
                pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                return RedirectToAction("PaperShow");
            }
            #endregion

            #region 第一次求出的题目数量少了
            if (n < num)
            {
                int tempnum = num - n;
                int n11 = (int)Math.Floor(p1 * tempnum * (1 - M1) * 2);
                int n22 = (int)Math.Floor(p2 * tempnum * (1 - M2) * 2);
                int n33 = (int)Math.Floor(p3 * tempnum * (1 - M3) * 2);
                int n44 = (int)Math.Floor(p4 * tempnum * (1 - M4) * 2);
                int n55 = (int)Math.Floor(p5 * tempnum * (1 - M5) * 2);
                int nn = n11 + n22 + n33 + n44 + n55;

                #region 第二次求出的题目数量刚好
                if (tempnum == nn)
                {
                    countList.Add((n1 + n11).ToString());
                    countList.Add((n2 + n22).ToString());
                    countList.Add((n3 + n33).ToString());
                    countList.Add((n4 + n44).ToString());
                    countList.Add((n5 + n55).ToString());
                    pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                    return RedirectToAction("PaperShow");
                }
                #endregion

                #region 第二次求出的题目数量少了
                if (tempnum > nn)
                {
                    int temp = tempnum - nn;
                    if (temp % 5 == 0)
                    {
                        countList.Add((n1 + n11 + temp / 5).ToString());
                        countList.Add((n2 + n22 + temp / 5).ToString());
                        countList.Add((n3 + n33 + temp / 5).ToString());
                        countList.Add((n4 + n44 + temp / 5).ToString());
                        countList.Add((n5 + n55 + temp / 5).ToString());

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                        return RedirectToAction("PaperShow");
                    }
                    else
                    {
                        int c = temp % 5;
                        int d = (temp - c) / 5;
                        countList.Add((n1 + n11 + d).ToString());
                        countList.Add((n2 + n22 + d).ToString());
                        countList.Add((n3 + n33 + d).ToString());
                        countList.Add((n4 + n44 + d).ToString());
                        countList.Add((n5 + n55 + d).ToString());
                        for (int i = 0; i < c; i++)
                        {
                            countList[i] = (int.Parse(countList[i]) + 1).ToString();
                        }

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                        return RedirectToAction("PaperShow");
                    }
                }
                #endregion

                #region 第二次求出的题目数量多了
                else
                {
                    int temp = nn - tempnum;
                    if (temp % 5 == 0)
                    {
                        countList.Add((n1 + n11 - temp / 5).ToString());
                        countList.Add((n2 + n22 - temp / 5).ToString());
                        countList.Add((n3 + n33 - temp / 5).ToString());
                        countList.Add((n4 + n44 - temp / 5).ToString());
                        countList.Add((n5 + n55 - temp / 5).ToString());

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                        return RedirectToAction("PaperShow");
                    }
                    else
                    {
                        int c = temp % 5;
                        int d = (temp - c) / 5;
                        countList.Add((n1 + n11 - d).ToString());
                        countList.Add((n2 + n22 - d).ToString());
                        countList.Add((n3 + n33 - d).ToString());
                        countList.Add((n4 + n44 - d).ToString());
                        countList.Add((n5 + n55 - d).ToString());
                        for (int i = 0; i < c; i++)
                        {
                            countList[i] = (int.Parse(countList[i]) - 1).ToString();
                        }

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                        return RedirectToAction("PaperShow");
                    }
                }
                #endregion
            }
            #endregion

            #region 第一次求出的题目数量多了
            else
            {
                int tempnum = n - num;
                int n11 = (int)Math.Floor(p1 * tempnum * (1 - M1) * 2);
                int n22 = (int)Math.Floor(p2 * tempnum * (1 - M2) * 2);
                int n33 = (int)Math.Floor(p3 * tempnum * (1 - M3) * 2);
                int n44 = (int)Math.Floor(p4 * tempnum * (1 - M4) * 2);
                int n55 = (int)Math.Floor(p5 * tempnum * (1 - M5) * 2);
                int nn = n11 + n22 + n33 + n44 + n55;

                #region 第二次求出的题目数量刚好
                if (tempnum == nn)
                {
                    countList.Add((n1 - n11).ToString());
                    countList.Add((n2 - n22).ToString());
                    countList.Add((n3 - n33).ToString());
                    countList.Add((n4 - n44).ToString());
                    countList.Add((n5 - n55).ToString());

                    pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                    return RedirectToAction("PaperShow");
                }
                #endregion

                #region 第二次求出的题目数量多了
                if (tempnum > nn)
                {
                    int temp = tempnum - nn;
                    if (temp % 5 == 0)
                    {
                        countList.Add((n1 - n11 - temp / 5).ToString());
                        countList.Add((n2 - n22 - temp / 5).ToString());
                        countList.Add((n3 - n33 - temp / 5).ToString());
                        countList.Add((n4 - n44 - temp / 5).ToString());
                        countList.Add((n5 - n55 - temp / 5).ToString());

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                        return RedirectToAction("PaperShow");
                    }
                    else
                    {
                        int c = temp % 5;
                        int d = (temp - c) / 5;
                        countList.Add((n1 - n11 - d).ToString());
                        countList.Add((n2 - n22 - d).ToString());
                        countList.Add((n3 - n33 - d).ToString());
                        countList.Add((n4 - n44 - d).ToString());
                        countList.Add((n5 - n55 - d).ToString());
                        for (int i = 0; i < c; i++)
                        {
                            countList[i] = (int.Parse(countList[i]) - 1).ToString();
                        }

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                        return RedirectToAction("PaperShow");
                    }
                }
                #endregion

                #region 第二次求出的题目数量少了
                else
                {
                    int temp = nn - tempnum;
                    if (temp % 5 == 0)
                    {
                        countList.Add((n1 - n11 + temp / 5).ToString());
                        countList.Add((n2 - n22 + temp / 5).ToString());
                        countList.Add((n3 - n33 + temp / 5).ToString());
                        countList.Add((n4 - n44 + temp / 5).ToString());
                        countList.Add((n5 - n55 + temp / 5).ToString());

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                        return RedirectToAction("PaperShow");
                    }
                    else
                    {
                        int c = temp % 5;
                        int d = (temp - c) / 5;
                        countList.Add((n1 - n11 + d).ToString());
                        countList.Add((n2 - n22 + d).ToString());
                        countList.Add((n3 - n33 + d).ToString());
                        countList.Add((n4 - n44 + d).ToString());
                        countList.Add((n5 - n55 + d).ToString());
                        for (int i = 0; i < c; i++)
                        {
                            countList[i] = (int.Parse(countList[i]) + 1).ToString();
                        }

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                        return RedirectToAction("PaperShow");
                    }
                }
                #endregion
            }
            #endregion
        }

        [HttpPost]
        public ActionResult ShowPaperByKnowledgeCover(string title)
        {
            TempData["CeType5Title"] = title;
            return null;
        }*/
        [Filter.LogFilter(Description = "按得分最大化组卷")]
        [HttpPost]
        public ActionResult ShowPaperByKnowledgeCover(string title)
        {
            TempData["CeType5Title"] = title;
            int num = 10;
            double P1 = 0.1567;//四级考试中的频次
            double P2 = 0.3114;
            double P3 = 0.0758;
            double P4 = 0.2665;
            double P5 = 0.0669;
            double p1 = P1 / (P1 + P2 + P3 + P4 + P5);//所占百分比
            double p2 = P2 / (P1 + P2 + P3 + P4 + P5);
            double p3 = P3 / (P1 + P2 + P3 + P4 + P5);
            double p4 = P4 / (P1 + P2 + P3 + P4 + P5);
            double p5 = P5 / (P1 + P2 + P3 + P4 + P5);
            string k1 = "信息明示";//考察最频繁的前5项知识点
            string k2 = "同义转述";
            string k3 = "隐含意义推断";
            string k4 = "词义理解";
            string k5 = "词类判断";
            List<string> kk = new List<string>();
            string know1 = _paper.SelectKnowledgeIDs(k1);//根据知识点名得到知识点id号
            kk.Add(know1);
            string know2 = _paper.SelectKnowledgeIDs(k2);
            kk.Add(know2);
            string know3 = _paper.SelectKnowledgeIDs(k3);
            kk.Add(know3);
            string know4 = _paper.SelectKnowledgeIDs(k4);
            kk.Add(know4);
            string know5 = _paper.SelectKnowledgeIDs(k5);
            kk.Add(know5);
            int userID = _paper.SelectUserID(User.Identity.Name);
            double M1 = _paper.SelectKP_MasterRate(k1, userID);//用户知识点掌握率
            double M2 = _paper.SelectKP_MasterRate(k2, userID);
            double M3 = _paper.SelectKP_MasterRate(k3, userID);
            double M4 = _paper.SelectKP_MasterRate(k4, userID);
            double M5 = _paper.SelectKP_MasterRate(k5, userID);
            if (double.IsNaN(M1))
            {
                M1 = 0.0;
            }
            if (double.IsNaN(M2))
            {
                M2 = 0.0;
            }
            if (double.IsNaN(M3))
            {
                M3 = 0.0;
            }
            if (double.IsNaN(M4))
            {
                M4 = 0.0;
            }
            if (double.IsNaN(M5))
            {
                M5 = 0.0;
            }
            int n1 = (int)Math.Floor(p1 * num * (1 - M1) * 2);
            int n2 = (int)Math.Floor(p2 * num * (1 - M2) * 2);
            int n3 = (int)Math.Floor(p3 * num * (1 - M3) * 2);
            int n4 = (int)Math.Floor(p4 * num * (1 - M4) * 2);
            int n5 = (int)Math.Floor(p5 * num * (1 - M5) * 2);
            int n = n1 + n2 + n3 + n4 + n5;
            List<string> countList = new List<string>();//存放5类知识点的题数

            #region 第一次求出的题目数量刚好
            if (n == num)
            {
                countList.Add(n1.ToString());
                countList.Add(n2.ToString());
                countList.Add(n3.ToString());
                countList.Add(n4.ToString());
                countList.Add(n5.ToString());
                pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);//kk是知识点id号
                return Json("");

            }
            #endregion

            #region 第一次求出的题目数量少了
            if (n < num)
            {
                int tempnum = num - n;
                int n11 = (int)Math.Floor(p1 * tempnum * (1 - M1) * 2);
                int n22 = (int)Math.Floor(p2 * tempnum * (1 - M2) * 2);
                int n33 = (int)Math.Floor(p3 * tempnum * (1 - M3) * 2);
                int n44 = (int)Math.Floor(p4 * tempnum * (1 - M4) * 2);
                int n55 = (int)Math.Floor(p5 * tempnum * (1 - M5) * 2);
                int nn = n11 + n22 + n33 + n44 + n55;

                #region 第二次求出的题目数量刚好
                if (tempnum == nn)
                {
                    countList.Add((n1 + n11).ToString());
                    countList.Add((n2 + n22).ToString());
                    countList.Add((n3 + n33).ToString());
                    countList.Add((n4 + n44).ToString());
                    countList.Add((n5 + n55).ToString());
                    pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                    return Json("");

                }
                #endregion

                #region 第二次求出的题目数量少了
                if (tempnum > nn)
                {
                    int temp = tempnum - nn;
                    if (temp % 5 == 0)
                    {
                        countList.Add((n1 + n11 + temp / 5).ToString());
                        countList.Add((n2 + n22 + temp / 5).ToString());
                        countList.Add((n3 + n33 + temp / 5).ToString());
                        countList.Add((n4 + n44 + temp / 5).ToString());
                        countList.Add((n5 + n55 + temp / 5).ToString());

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                        return Json("");
                    }
                    else
                    {
                        int c = temp % 5;
                        int d = (temp - c) / 5;
                        countList.Add((n1 + n11 + d).ToString());
                        countList.Add((n2 + n22 + d).ToString());
                        countList.Add((n3 + n33 + d).ToString());
                        countList.Add((n4 + n44 + d).ToString());
                        countList.Add((n5 + n55 + d).ToString());
                        for (int i = 0; i < c; i++)
                        {
                            countList[i] = (int.Parse(countList[i]) + 1).ToString();
                        }

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                        return Json("");

                    }
                }
                #endregion

                #region 第二次求出的题目数量多了
                else
                {
                    int temp = nn - tempnum;
                    if (temp % 5 == 0)
                    {
                        countList.Add((n1 + n11 - temp / 5).ToString());
                        countList.Add((n2 + n22 - temp / 5).ToString());
                        countList.Add((n3 + n33 - temp / 5).ToString());
                        countList.Add((n4 + n44 - temp / 5).ToString());
                        countList.Add((n5 + n55 - temp / 5).ToString());

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                        return Json("");

                    }
                    else
                    {
                        int c = temp % 5;
                        int d = (temp - c) / 5;
                        countList.Add((n1 + n11 - d).ToString());
                        countList.Add((n2 + n22 - d).ToString());
                        countList.Add((n3 + n33 - d).ToString());
                        countList.Add((n4 + n44 - d).ToString());
                        countList.Add((n5 + n55 - d).ToString());
                        for (int i = 0; i < c; i++)
                        {
                            countList[i] = (int.Parse(countList[i]) - 1).ToString();
                        }

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                        return Json("");

                    }
                }
                #endregion
            }
            #endregion

            #region 第一次求出的题目数量多了
            else
            {
                int tempnum = n - num;
                int n11 = (int)Math.Floor(p1 * tempnum * (1 - M1) * 2);
                int n22 = (int)Math.Floor(p2 * tempnum * (1 - M2) * 2);
                int n33 = (int)Math.Floor(p3 * tempnum * (1 - M3) * 2);
                int n44 = (int)Math.Floor(p4 * tempnum * (1 - M4) * 2);
                int n55 = (int)Math.Floor(p5 * tempnum * (1 - M5) * 2);
                int nn = n11 + n22 + n33 + n44 + n55;

                #region 第二次求出的题目数量刚好
                if (tempnum == nn)
                {
                    countList.Add((n1 - n11).ToString());
                    countList.Add((n2 - n22).ToString());
                    countList.Add((n3 - n33).ToString());
                    countList.Add((n4 - n44).ToString());
                    countList.Add((n5 - n55).ToString());

                    pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                    return Json("");

                }
                #endregion

                #region 第二次求出的题目数量多了
                if (tempnum > nn)
                {
                    int temp = tempnum - nn;
                    if (temp % 5 == 0)
                    {
                        countList.Add((n1 - n11 - temp / 5).ToString());
                        countList.Add((n2 - n22 - temp / 5).ToString());
                        countList.Add((n3 - n33 - temp / 5).ToString());
                        countList.Add((n4 - n44 - temp / 5).ToString());
                        countList.Add((n5 - n55 - temp / 5).ToString());

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                        return Json("");

                    }
                    else
                    {
                        int c = temp % 5;
                        int d = (temp - c) / 5;
                        countList.Add((n1 - n11 - d).ToString());
                        countList.Add((n2 - n22 - d).ToString());
                        countList.Add((n3 - n33 - d).ToString());
                        countList.Add((n4 - n44 - d).ToString());
                        countList.Add((n5 - n55 - d).ToString());
                        for (int i = 0; i < c; i++)
                        {
                            countList[i] = (int.Parse(countList[i]) - 1).ToString();
                        }

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);

                        return Json("");
                    }
                }
                #endregion

                #region 第二次求出的题目数量少了
                else
                {
                    int temp = nn - tempnum;
                    if (temp % 5 == 0)
                    {
                        countList.Add((n1 - n11 + temp / 5).ToString());
                        countList.Add((n2 - n22 + temp / 5).ToString());
                        countList.Add((n3 - n33 + temp / 5).ToString());
                        countList.Add((n4 - n44 + temp / 5).ToString());
                        countList.Add((n5 - n55 + temp / 5).ToString());

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                        return Json("");
                    }
                    else
                    {
                        int c = temp % 5;
                        int d = (temp - c) / 5;
                        countList.Add((n1 - n11 + d).ToString());
                        countList.Add((n2 - n22 + d).ToString());
                        countList.Add((n3 - n33 + d).ToString());
                        countList.Add((n4 - n44 + d).ToString());
                        countList.Add((n5 - n55 + d).ToString());
                        for (int i = 0; i < c; i++)
                        {
                            countList[i] = (int.Parse(countList[i]) + 1).ToString();
                        }

                        pp = MakePaper(_paper.SelectAssessmentItems3(kk, countList, userID), 5, kk);
                        return Json("");
                    }
                }
                #endregion
            }
            #endregion
        }

        [Authorize(Roles = "普通用户,体验用户")]
        public ActionResult PaperShow()//从数据库中读取事件封装成对象给view
        {
            if (Continues == "1") //1表示暂存的试卷
            {
                ViewData["title"] = ppc.paperTotal.Title;
                ViewData["Temp"] = "1";
            }
            else
            {
                ViewData["title"] = pp.Title;
                ViewData["Temp"] = "2";
                pp.timeList = new List<int>();
                //pp.timeList.Add(1);
            }
            return View();
        }

        [Authorize(Roles = "普通用户,体验用户")]
        [HttpPost]  //保存试卷
        public ActionResult PaperShow(FormCollection form)
        {
            try
            {
                using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(1, 5, 0)))
                {
                    int PaperState = _paper.SelectPS(Guid.Parse(form["PaperID"]));
                    string temp = form["Temp"];
                    #region 完整试卷保存
                    if (temp == "1") //完整试卷保存
                    {
                        double TotalScore = 0;
                        List<CEDTS_TestAnswer> TestAnswerList = new List<CEDTS_TestAnswer>();
                        List<CEDTS_TestAnswerKnowledgePoint> TestAKPList = new List<CEDTS_TestAnswerKnowledgePoint>();
                        string id = form["Continues"]; //其实是testID 即答卷id
                        //保存到答卷表
                        CEDTS_Test Test = new CEDTS_Test();
                        if (id == "")
                        {
                            Test.TestID = Guid.NewGuid();
                            Test.StartDate = DateTime.Parse(form["StartTime"]);
                            Test.UserID = _paper.SelectUserID(User.Identity.Name);
                            Test.IsFinished = true;
                            Test.IsChecked = 0;
                            Test.PaperID = Guid.Parse(form["PaperID"]);
                            Test.FinishDate = DateTime.Now;
                            TimeSpan ts = Test.FinishDate.Value - Test.StartDate.Value;
                            Test.TotalTime = ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;

                            _paper.CreateTest(Test);
                        }
                        else
                        {
                            TimeSpan ts = DateTime.Now - DateTime.Parse(form["StartTime"]);

                            int t1 = form["temptime1"] == null || form["temptime1"] == "" ? 0 : int.Parse(form["temptime1"]);
                            int t2 = form["temptime2"] == null || form["temptime2"] == "" ? 0 : int.Parse(form["temptime2"]);
                            int t3 = form["temptime3"] == null || form["temptime3"] == "" ? 0 : int.Parse(form["temptime3"]);
                            int t4 = form["temptime4"] == null || form["temptime4"] == "" ? 0 : int.Parse(form["temptime4"]);
                            int t5 = form["temptime5"] == null || form["temptime5"] == "" ? 0 : int.Parse(form["temptime5"]);
                            int t6 = form["temptime6"] == null || form["temptime6"] == "" ? 0 : int.Parse(form["temptime6"]);
                            int t7 = form["temptime7"] == null || form["temptime7"] == "" ? 0 : int.Parse(form["temptime7"]);
                            int t8 = form["temptime8"] == null || form["temptime8"] == "" ? 0 : int.Parse(form["temptime8"]);
                            int t9 = form["temptime9"] == null || form["temptime9"] == "" ? 0 : int.Parse(form["temptime9"]);

                            int TotalTime = ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds + t1 + t2 + t3 + t4 + t5 + t6 + t7 + t8 + t9;
                            Guid TestID = Guid.Parse(id);

                            Test.TestID = TestID;
                            Test.UserID = _paper.SelectUserID(User.Identity.Name);
                            Test.PaperID = Guid.Parse(form["PaperID"]);
                            Test.IsFinished = true;
                            Test.IsChecked = 0;
                            Test.PaperID = Guid.Parse(form["PaperID"]);
                            Test.StartDate = DateTime.Parse(form["StartTime"]);
                            Test.FinishDate = DateTime.Now;
                            Test.TotalTime = TotalTime;
                            _paper.UpdataTest(Test);
                            _paper.DeleteTempTime(TestID); //暂存试卷下删除暂存试卷零时表中的time表中的时间
                            _paper.DeleteTempTest(TestID);//同上，改为test表
                        }
                        //判断试卷中是否有快速阅读题型
                        int Sspc = 0; int Slpo = 0; int Llpo = 0; int Rlpo = 0; int Lpc = 0; int Rpc = 0; int Rpo = 0; int InfoMat = 0; int Cp = 0;
                        string SSspc = form["Sspc"];
                        if (SSspc != "")
                        {
                            Sspc = int.Parse(SSspc);
                        }
                        string SSlpo = form["Slpo"];
                        if (SSlpo != "")
                        {
                            Slpo = int.Parse(SSlpo);
                        }
                        string SLlpo = form["Llpo"];
                        if (SLlpo != "")
                        {
                            Llpo = int.Parse(SLlpo);
                        }
                        string SRlpo = form["Rlpo"];
                        if (SRlpo != "")
                        {
                            Rlpo = int.Parse(SRlpo);
                        }
                        string SLpc = form["Lpc"];
                        if (SLpc != "")
                        {
                            Lpc = int.Parse(SLpc);
                        }
                        string SRpc = form["Rpc"];
                        if (SRpc != "")
                        {
                            Rpc = int.Parse(SRpc);
                        }
                        string SRpo = form["Rpo"];
                        if (SRpo != "")
                        {
                            Rpo = int.Parse(SRpo);
                        }
                        string SInfoMat = form["InfoMat"];
                        if (SInfoMat != "")
                        {
                            InfoMat = int.Parse(SInfoMat);
                        }
                        string SCP = form["CP"];
                        if (SCP != "")
                        {
                            Cp = int.Parse(SCP);
                        }
                        KnowledgePointInfo KpInfo = new KnowledgePointInfo();
                        KpInfo.KnowledgePointID = new List<Guid>();
                        KpInfo.TotalCount = new List<int>();
                        KpInfo.AverageTime = new List<double>();
                        KpInfo.CorrectCount = new List<int>();
                        int Number = 1;

                        #region 快速阅读
                        if (Sspc > 0)
                        {
                            int SspCorrectNum = 0;
                            int SspTotalNum = int.Parse(form["SspcTotalNum"]);
                            string Time1 = form["time1"];
                            int tempTime1 = form["temptime1"] == "" || form["temptime1"] == null ? 0 : int.Parse(form["temptime1"]);
                            int SspTime = 0;
                            if (Time1 != "")
                            {
                                SspTime = (int.Parse(Time1) + tempTime1) / SspTotalNum;
                            }
                            else
                            {
                                SspTime = 0;
                            }


                            for (int i = 0; i < Sspc; i++)
                            {
                                int ChoiceNum = int.Parse(form["ChoiceNum" + i]);
                                int TermNum = int.Parse(form["TermNum" + i]);
                                Guid AssessmentItemID = Guid.Parse(form["SspAssessmentItemID" + i]);
                                
                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;
                                    if (_paper.SelectUA(Test.UserID, AssessmentItemID) == 2)
                                    {
                                        UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, AssessmentItemID).AssessmentItemID;
                                        UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, AssessmentItemID).UserAssessmentCountID;
                                        UAC.Count = _paper.SelectUAC(Test.UserID, AssessmentItemID).Count + 1;
                                        _paper.UpdataUAC(UAC);
                                    }
                                    else
                                    {                                    
                                        UAC.AssessmentItemID = AssessmentItemID;
                                        UAC.Count = 1;
                                        _paper.CreateUAC(UAC);
                                    }
                                }                                
                                for (int i1 = 0; i1 < ChoiceNum; i1++)
                                {
                                    int Rigth = 0;
                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();

                                    Guid SspQuestionID = Guid.Parse(form["SspQuestionID" + i + "_" + i1]);
                                    string SspUserAnswer = form["SspRadio" + i + "_" + i1];
                                    string SspAnswerValue = form["SspAnswerValue" + i + "_" + i1];

                                    if (SspAnswerValue == SspUserAnswer)
                                    {
                                        TotalScore += double.Parse(form["SspScoreQuestion" + i + "_" + i1]);
                                        TestAnswer.IsRight = true;
                                        SspCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    TestAnswer.Answer = SspAnswerValue;
                                    TestAnswer.AnswerContent = _paper.AnswerContent(SspQuestionID, SspAnswerValue);
                                    TestAnswer.UserAnswer = SspUserAnswer;
                                    TestAnswer.QuestionID = SspQuestionID;
                                    TestAnswer.AssessmentItemID = AssessmentItemID;
                                    TestAnswer.ItemTypeID = 1;
                                    TestAnswer.ItemAnswerTime = SspTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    //
                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(SspQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (!KpInfo.KnowledgePointID.Contains(KpID))
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpID == KpInfo.KnowledgePointID[j])
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                                for (int i2 = 0; i2 < TermNum; i2++)
                                {
                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                    Guid SspQuestionID = Guid.Parse(form["SspQuestionID" + i + "_" + (ChoiceNum + i2)]);
                                    string SspUserAnswer = form["SspAnswer" + i + "_" + i2] == null ? string.Empty : form["SspAnswer" + i + "_" + i2];
                                    string SspUserAnswer1 = SspUserAnswer.Replace(" ", "");
                                    SspUserAnswer1 = SspUserAnswer1.ToUpper();

                                    string SspAnswerValue = form["SspAnswerValue" + i + "_" + (ChoiceNum + i2)] == null ? string.Empty : form["SspAnswerValue" + i + "_" + (ChoiceNum + i2)];
                                    string SspAnswerValue1 = SspAnswerValue.Replace(" ", "");
                                    SspAnswerValue1 = SspAnswerValue1.ToUpper();

                                    int Rigth = 0;
                                    if (SspAnswerValue1 == SspUserAnswer1)
                                    {
                                        TotalScore += double.Parse(form["SspScoreQuestion" + i + "_" + (ChoiceNum + i2)]);
                                        TestAnswer.IsRight = true;
                                        SspCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    if (SspUserAnswer == null)
                                    {
                                        SspUserAnswer = "";
                                    }
                                    TestAnswer.Answer = SspAnswerValue;
                                    TestAnswer.AnswerContent = SspAnswerValue;
                                    TestAnswer.UserAnswer = SspUserAnswer;
                                    TestAnswer.AssessmentItemID = AssessmentItemID;
                                    TestAnswer.QuestionID = SspQuestionID;
                                    TestAnswer.ItemTypeID = 1;
                                    TestAnswer.ItemAnswerTime = SspTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(SspQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (KpInfo.KnowledgePointID.Contains(KpID) == false)
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime / SspTotalNum));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpInfo.KnowledgePointID[j] == KpID)
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                        KpInfo.AverageTime[j] = (KpInfo.AverageTime[j] + Convert.ToDouble(TestAnswer.ItemAnswerTime / SspTotalNum)) / KpInfo.TotalCount[j];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }

                            }

                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.ItemTypeID = 1;
                            SspTPI.CorrectItemNumber = SspCorrectNum;
                            SspTPI.TotalItemNumber = SspTotalNum;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);
                            SspTPI.Time = DateTime.Now;
                            _paper.CreateTATI(SspTPI);

                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }

                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;

                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                            
                        }

                        #endregion

                        #region 短对话
                        if (Slpo > 0)
                        {
                            int SlpoCorrectNum = 0;
                            string Time1 = form["time2"];
                            int tempTime1 = form["temptime2"] == "" || form["temptime2"] == null ? 0 : int.Parse(form["temptime2"]);
                            int SlpoTime = 0;
                            if (Time1 != "")
                            {
                                SlpoTime = ((int.Parse(Time1) + tempTime1) / Slpo);
                            }
                            else
                            {
                                SlpoTime = 0;
                            }
                            for (int i = 0; i < Slpo; i++)
                            {
                                Guid SlpoAssessmentItemID = Guid.Parse(form["SlpAssessmentItemID" + i]);
                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;
                                    if (_paper.SelectAU(SlpoAssessmentItemID) == 1)
                                    {
                                        if (_paper.SelectUA(Test.UserID, SlpoAssessmentItemID) == 2)
                                        {
                                            UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, SlpoAssessmentItemID).AssessmentItemID;
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, SlpoAssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(Test.UserID, SlpoAssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.AssessmentItemID = SlpoAssessmentItemID;
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }
                                    }
                                }
                                                              
                                int Rigth = 0;
                                CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                Guid SlpoQuestionID = Guid.Parse(form["SlpQuestionID" + i]);
                                string SlpoUserAnswer = form["SlpRadio" + i];
                                string SlpoAnswerValue = form["SlpAnswerValue" + i];

                                if (SlpoAnswerValue == SlpoUserAnswer)
                                {
                                    TotalScore += double.Parse(form["SlpScoreQuestion" + i]);
                                    TestAnswer.IsRight = true;
                                    SlpoCorrectNum += 1;
                                    Rigth = 1;
                                }
                                else
                                {
                                    TestAnswer.IsRight = false;
                                }
                                //添加信息到TestAnswer表
                                TestAnswer.TestID = Test.TestID;
                                TestAnswer.UserID = Test.UserID;
                                TestAnswer.Answer = SlpoAnswerValue;
                                TestAnswer.AnswerContent = _paper.AnswerContent(SlpoQuestionID, SlpoAnswerValue);
                                TestAnswer.UserAnswer = SlpoUserAnswer;
                                TestAnswer.QuestionID = SlpoQuestionID;
                                TestAnswer.AssessmentItemID = SlpoAssessmentItemID;
                                TestAnswer.ItemTypeID = 2;
                                TestAnswer.ItemAnswerTime = SlpoTime;
                                TestAnswer.Number = Number;
                                Number += 1;
                                _paper.CreateTestAnswer(TestAnswer);

                                //
                                if (PaperState != 9 && PaperState != 10)
                                {
                                    List<Guid> KpIDList = new List<Guid>();
                                    KpIDList = _paper.SelectKnowledge(SlpoQuestionID);

                                    foreach (Guid KpID in KpIDList)
                                    {

                                        if (!KpInfo.KnowledgePointID.Contains(KpID))
                                        {
                                            if (Rigth == 1)
                                            {
                                                KpInfo.CorrectCount.Add(1);
                                            }
                                            else
                                            {
                                                KpInfo.CorrectCount.Add(0);
                                            }
                                            KpInfo.KnowledgePointID.Add(KpID);
                                            KpInfo.TotalCount.Add(1);
                                            KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                        }
                                        else
                                        {
                                            for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                            {
                                                if (KpID == KpInfo.KnowledgePointID[j])
                                                {
                                                    if (Rigth == 1)
                                                    {
                                                        KpInfo.CorrectCount[j] += 1;
                                                    }
                                                    KpInfo.TotalCount[j] += 1;
                                                }
                                            }
                                        }
                                    }
                                }                             

                            }

                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.ItemTypeID = 2;
                            SspTPI.CorrectItemNumber = SlpoCorrectNum;
                            SspTPI.TotalItemNumber = Slpo;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);
                            SspTPI.Time = DateTime.Now;
                            _paper.CreateTATI(SspTPI);

                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }

                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;
                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                            
                        }
                        #endregion

                        #region 长对话
                        if (Llpo > 0)
                        {
                            int LlpoTotalNum = int.Parse(form["LlpoTotalNum"]);
                            string Time1 = form["time3"];
                            int tempTime1 = form["temptime3"] == "" || form["temptime3"] == null ? 0 : int.Parse(form["temptime3"]);
                            int LlpoTime = 0;
                            if (Time1 != "")
                            {
                                LlpoTime = (int.Parse(Time1) + tempTime1) / LlpoTotalNum;
                            }
                            else
                            {
                                LlpoTime = 0;
                            }
                            int LlpoCorrectNum = 0;
                            for (int i = 0; i < Llpo; i++)
                            {
                                Guid LlpoAssessmentItemID = Guid.Parse(form["LlpAssessmentItemID" + i]);
                                int LlpoNum = int.Parse(form["LlpNum" + i]);

                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;

                                    if (_paper.SelectUA(Test.UserID, LlpoAssessmentItemID) == 2)
                                    {
                                        UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, LlpoAssessmentItemID).AssessmentItemID;
                                        UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, LlpoAssessmentItemID).UserAssessmentCountID;
                                        UAC.Count = _paper.SelectUAC(Test.UserID, LlpoAssessmentItemID).Count + 1;
                                        _paper.UpdataUAC(UAC);
                                    }
                                    else
                                    {
                                        UAC.AssessmentItemID = LlpoAssessmentItemID;
                                        UAC.Count = 1;
                                        _paper.CreateUAC(UAC);
                                    }
                                }
                                

                                for (int i1 = 0; i1 < LlpoNum; i1++)
                                {
                                    int Rigth = 0;

                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();

                                    Guid LlpoQuestionID = Guid.Parse(form["LlpQuestionID" + i + "_" + i1]);
                                    string LlpoUserAnswer = form["LlpRadio" + i + "_" + i1];
                                    string LlpoAnswerValue = form["LlpAnswerValue" + i + "_" + i1];

                                    if (LlpoAnswerValue == LlpoUserAnswer)
                                    {
                                        TotalScore += double.Parse(form["LlpScoreQuestion" + i + "_" + i1]);
                                        TestAnswer.IsRight = true;
                                        LlpoCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    TestAnswer.Answer = LlpoAnswerValue;
                                    TestAnswer.AnswerContent = _paper.AnswerContent(LlpoQuestionID, LlpoAnswerValue);
                                    TestAnswer.UserAnswer = LlpoUserAnswer;
                                    TestAnswer.QuestionID = LlpoQuestionID;
                                    TestAnswer.AssessmentItemID = LlpoAssessmentItemID;
                                    TestAnswer.ItemTypeID = 3;
                                    TestAnswer.ItemAnswerTime = LlpoTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    //
                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(LlpoQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (!KpInfo.KnowledgePointID.Contains(KpID))
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpID == KpInfo.KnowledgePointID[j])
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.Time = DateTime.Now;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.ItemTypeID = 3;
                            SspTPI.CorrectItemNumber = LlpoCorrectNum;
                            SspTPI.TotalItemNumber = LlpoTotalNum;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);

                            _paper.CreateTATI(SspTPI);

                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }

                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;
                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                            
                        }
                        #endregion

                        #region 短文听力理解
                        if (Rlpo > 0)
                        {
                            int RlpoTotalNum = int.Parse(form["RlpoTotalNum"]);
                            int RlpoCorrectNum = 0;
                            string Time1 = form["time4"];
                            int tempTime1 = form["temptime4"] == "" || form["temptime4"] == null ? 0 : int.Parse(form["temptime4"]);
                            int RlpoTime = 0;
                            if (Time1 != "")
                            {
                                RlpoTime = (int.Parse(Time1) + tempTime1) / RlpoTotalNum;
                            }
                            else
                            {
                                RlpoTime = 0;
                            }
                            for (int i = 0; i < Rlpo; i++)
                            {
                                Guid RlpoAssessmentItemID = Guid.Parse(form["RlpAssessmentItemID" + i]);
                                int LlpoNum = int.Parse(form["RlpNum" + i]);

                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;
                                    if (_paper.SelectUA(Test.UserID, RlpoAssessmentItemID) == 2)
                                    {
                                        UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, RlpoAssessmentItemID).AssessmentItemID;
                                        UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, RlpoAssessmentItemID).UserAssessmentCountID;
                                        UAC.Count = _paper.SelectUAC(Test.UserID, RlpoAssessmentItemID).Count + 1;
                                        _paper.UpdataUAC(UAC);
                                    }
                                    else
                                    {
                                        UAC.AssessmentItemID = RlpoAssessmentItemID;
                                        UAC.Count = 1;
                                        _paper.CreateUAC(UAC);
                                    }
                                }
                                

                                for (int i1 = 0; i1 < LlpoNum; i1++)
                                {
                                    int Rigth = 0;
                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();

                                    Guid RlpoQuestionID = Guid.Parse(form["RlpQuestionID" + i + "_" + i1]);
                                    string RlpoUserAnswer = form["RlpoRadio" + i + "_" + i1];
                                    string RlpoAnswerValue = form["RlpAnswerValue" + i + "_" + i1];

                                    if (RlpoAnswerValue == RlpoUserAnswer)
                                    {
                                        TotalScore += double.Parse(form["RlpScoreQuestion" + i + "_" + i1]);
                                        TestAnswer.IsRight = true;
                                        RlpoCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    TestAnswer.Answer = RlpoAnswerValue;
                                    TestAnswer.AnswerContent = _paper.AnswerContent(RlpoQuestionID, RlpoAnswerValue);
                                    TestAnswer.UserAnswer = RlpoUserAnswer;
                                    TestAnswer.QuestionID = RlpoQuestionID;
                                    TestAnswer.AssessmentItemID = RlpoAssessmentItemID;
                                    TestAnswer.ItemTypeID = 4;
                                    TestAnswer.ItemAnswerTime = RlpoTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    //
                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(RlpoQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (!KpInfo.KnowledgePointID.Contains(KpID))
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpID == KpInfo.KnowledgePointID[j])
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.Time = DateTime.Now;
                            SspTPI.ItemTypeID = 4;
                            SspTPI.CorrectItemNumber = RlpoCorrectNum;
                            SspTPI.TotalItemNumber = RlpoTotalNum;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);

                            _paper.CreateTATI(SspTPI);

                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }

                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;
                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                            
                        }
                        #endregion

                        #region 复合型听力
                        if (Lpc > 0)
                        {
                            int LpcTotalNum = int.Parse(form["LpcTotalNum"]);
                            string Time1 = form["time5"];
                            int tempTime1 = form["temptime5"] == "" || form["temptime5"] == null ? 0 : int.Parse(form["temptime5"]);
                            int LpcTime = 0;
                            if (Time1 != "")
                            {
                                LpcTime = (int.Parse(Time1) + tempTime1) / LpcTotalNum;
                            }
                            else
                            {
                                LpcTime = 0;
                            }
                            int LpcCorrectNum = 0;
                            for (int i = 0; i < Lpc; i++)
                            {
                                Guid LpcAssessmentItemID = Guid.Parse(form["LpcAssessmentItemID" + i]);
                                int LpcNum = int.Parse(form["LpcNum" + i]);

                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;
                                    if (_paper.SelectUA(Test.UserID, LpcAssessmentItemID) == 2)
                                    {
                                        UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, LpcAssessmentItemID).AssessmentItemID;
                                        UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, LpcAssessmentItemID).UserAssessmentCountID;
                                        UAC.Count = _paper.SelectUAC(Test.UserID, LpcAssessmentItemID).Count + 1;
                                        _paper.UpdataUAC(UAC);
                                    }
                                    else
                                    {
                                        UAC.AssessmentItemID = LpcAssessmentItemID;
                                        UAC.Count = 1;
                                        _paper.CreateUAC(UAC);
                                    }
                                }
                                

                                for (int i1 = 0; i1 < LpcNum; i1++)
                                {
                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                    Guid LpcQuestionID = Guid.Parse(form["LpcQuestionID" + i + "_" + i1]);
                                    string LpcUserAnswer = form["LpcAnswer" + i + "_" + i1] == null ? string.Empty : form["LpcAnswer" + i + "_" + i1];
                                    string LpcUserAnswer1 = LpcUserAnswer.Replace(" ", "");
                                    LpcUserAnswer1 = LpcUserAnswer1.ToUpper();

                                    string LpcAnswerValue = form["LpcAnswerValue" + i + "_" + i1] == null ? string.Empty : form["LpcAnswerValue" + i + "_" + i1];
                                    string LpcAnswerValue1 = LpcAnswerValue.Replace(" ", "");
                                    LpcAnswerValue1 = LpcAnswerValue1.ToUpper();

                                    int Rigth = 0;
                                    if (LpcAnswerValue1 == LpcUserAnswer1)
                                    {
                                        TotalScore += double.Parse(form["LpcScoreQuestion" + i + "_" + i1]);
                                        TestAnswer.IsRight = true;
                                        LpcCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    if (LpcUserAnswer == null)
                                    {
                                        LpcUserAnswer = "";
                                    }
                                    TestAnswer.Answer = LpcAnswerValue;
                                    TestAnswer.AnswerContent = LpcAnswerValue;
                                    TestAnswer.UserAnswer = LpcUserAnswer;
                                    TestAnswer.QuestionID = LpcQuestionID;
                                    TestAnswer.AssessmentItemID = LpcAssessmentItemID;
                                    TestAnswer.ItemTypeID = 5;
                                    TestAnswer.ItemAnswerTime = LpcTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    //
                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(LpcQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (!KpInfo.KnowledgePointID.Contains(KpID))
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpID == KpInfo.KnowledgePointID[j])
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.Time = DateTime.Now;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.ItemTypeID = 5;
                            SspTPI.CorrectItemNumber = LpcCorrectNum;
                            SspTPI.TotalItemNumber = LpcTotalNum;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);

                            _paper.CreateTATI(SspTPI);

                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }

                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;
                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                            
                        }
                        #endregion

                        #region 阅读理解-选词填空
                        if (Rpc > 0)
                        {

                            int RpcTotalNum = int.Parse(form["RpcTotalNum"]);
                            string Time1 = form["time6"];
                            int tempTime1 = form["temptime6"] == "" || form["temptime6"] == null ? 0 : int.Parse(form["temptime6"]);
                            int RpcTime = 0;
                            if (Time1 != "")
                            {
                                RpcTime = (int.Parse(Time1) + tempTime1) / RpcTotalNum;
                            }
                            else
                            {
                                RpcTime = 0;
                            }
                            int RpcCorrectNum = 0;
                            for (int i = 0; i < Rpc; i++)
                            {
                                Guid RpcAssessmentItemID = Guid.Parse(form["RpcAssessmentItemID" + i]);
                                int RpcNum = int.Parse(form["RpcNum" + i]);

                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;

                                    if (_paper.SelectUA(Test.UserID, RpcAssessmentItemID) == 2)
                                    {
                                        UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, RpcAssessmentItemID).AssessmentItemID;
                                        UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, RpcAssessmentItemID).UserAssessmentCountID;
                                        UAC.Count = _paper.SelectUAC(Test.UserID, RpcAssessmentItemID).Count + 1;
                                        _paper.UpdataUAC(UAC);
                                    }
                                    else
                                    {
                                        UAC.AssessmentItemID = RpcAssessmentItemID;
                                        UAC.Count = 1;
                                        _paper.CreateUAC(UAC);
                                    }
                                }
                                

                                for (int i1 = 0; i1 < RpcNum; i1++)
                                {
                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                    Guid RpcQuestionID = Guid.Parse(form["RpcQuestionID" + i + "_" + i1]);
                                    string RpcUserAnswer = form["RpcAnswer" + i + "_" + i1] == null ? string.Empty : form["RpcAnswer" + i + "_" + i1];
                                    string RpcUserAnswer1 = RpcUserAnswer.Replace(" ", "");
                                    RpcUserAnswer1 = RpcUserAnswer1.ToUpper();

                                    string RpcAnswerValue = form["RpcAnswerValue" + i + "_" + i1] == null ? string.Empty : form["RpcAnswerValue" + i + "_" + i1];
                                    string RpcAnswerValue1 = RpcAnswerValue.Replace(" ", "");
                                    RpcAnswerValue1 = RpcAnswerValue1.ToUpper();

                                    int Rigth = 0;
                                    if (RpcAnswerValue1 == RpcUserAnswer1)
                                    {
                                        TotalScore += double.Parse(form["RpcScoreQuestion" + i + "_" + i1]);
                                        TestAnswer.IsRight = true;
                                        RpcCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    TestAnswer.Answer = RpcAnswerValue;
                                    TestAnswer.AnswerContent = _paper.AnswerContent(RpcQuestionID, RpcAnswerValue);
                                    TestAnswer.UserAnswer = RpcUserAnswer;
                                    TestAnswer.QuestionID = RpcQuestionID;
                                    TestAnswer.AssessmentItemID = RpcAssessmentItemID;
                                    TestAnswer.ItemTypeID = 6;
                                    TestAnswer.ItemAnswerTime = RpcTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    //
                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(RpcQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (!KpInfo.KnowledgePointID.Contains(KpID))
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpID == KpInfo.KnowledgePointID[j])
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.Time = DateTime.Now;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.ItemTypeID = 6;
                            SspTPI.CorrectItemNumber = RpcCorrectNum;
                            SspTPI.TotalItemNumber = RpcTotalNum;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);

                            _paper.CreateTATI(SspTPI);

                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }

                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;
                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                            
                        }
                        #endregion

                        #region 阅读理解-选择题型
                        if (Rpo > 0)
                        {
                            int RpoTotalNum = int.Parse(form["RpoTotalNum"]);
                            int RpoCorrectNum = 0;
                            int LpoTime = 0;
                            string Time1 = form["time7"];
                            int tempTime1 = form["temptime7"] == "" || form["temptime7"] == null ? 0 : int.Parse(form["temptime7"]);
                            if (Time1 != "")
                            {
                                LpoTime = (int.Parse(Time1) + tempTime1) / RpoTotalNum;
                            }
                            else
                            {
                                LpoTime = 0;
                            }
                            for (int i = 0; i < Rpo; i++)
                            {
                                Guid RpoAssessmentItemID = Guid.Parse(form["RpoAssessmentItemID" + i]);
                                int LpoNum = int.Parse(form["RpoNum" + i]);

                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;

                                    if (_paper.SelectUA(Test.UserID, RpoAssessmentItemID) == 2)
                                    {
                                        UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, RpoAssessmentItemID).AssessmentItemID;
                                        UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, RpoAssessmentItemID).UserAssessmentCountID;
                                        UAC.Count = _paper.SelectUAC(Test.UserID, RpoAssessmentItemID).Count + 1;
                                        _paper.UpdataUAC(UAC);
                                    }
                                    else
                                    {
                                        UAC.AssessmentItemID = RpoAssessmentItemID;
                                        UAC.Count = 1;
                                        _paper.CreateUAC(UAC);
                                    }
                                }
                                

                                for (int i1 = 0; i1 < LpoNum; i1++)
                                {
                                    int Rigth = 0;
                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();

                                    Guid RpoQuestionID = Guid.Parse(form["RpoQuestionID" + i + "_" + i1]);
                                    string RpoUserAnswer = form["RpoRadio" + i + "_" + i1];
                                    string RpoAnswerValue = form["RpoAnswerValue" + i + "_" + i1];

                                    if (RpoAnswerValue == RpoUserAnswer)
                                    {
                                        TotalScore += double.Parse(form["RpoScoreQuestion" + i + "_" + i1]);
                                        TestAnswer.IsRight = true;
                                        RpoCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    TestAnswer.Answer = RpoAnswerValue;
                                    TestAnswer.AnswerContent = _paper.AnswerContent(RpoQuestionID, RpoAnswerValue);
                                    TestAnswer.UserAnswer = RpoUserAnswer;
                                    TestAnswer.QuestionID = RpoQuestionID;
                                    TestAnswer.AssessmentItemID = RpoAssessmentItemID;
                                    TestAnswer.ItemTypeID = 7;
                                    TestAnswer.ItemAnswerTime = LpoTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    //
                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(RpoQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (!KpInfo.KnowledgePointID.Contains(KpID))
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpID == KpInfo.KnowledgePointID[j])
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }

                            }
                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.Time = DateTime.Now;
                            SspTPI.ItemTypeID = 7;
                            SspTPI.CorrectItemNumber = RpoCorrectNum;
                            SspTPI.TotalItemNumber = RpoTotalNum;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);

                            _paper.CreateTATI(SspTPI);

                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }

                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;
                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                            
                        }
                        #endregion

                        #region 阅读理解-信息匹配
                        if (InfoMat > 0)
                        {

                            int InfoMatTotalNum = int.Parse(form["InfoMatTotalNum"]);
                            string Time1 = form["time8"];
                            int tempTime1 = form["temptime8"] == "" || form["temptime8"] == null ? 0 : int.Parse(form["temptime8"]);
                            int InfoMatTime = 0;
                            if (Time1 != "")
                            {
                                InfoMatTime = (int.Parse(Time1) + tempTime1) / InfoMatTotalNum;
                            }
                            else
                            {
                                InfoMatTime = 0;
                            }
                            int InfoMatCorrectNum = 0;
                            for (int i = 0; i < InfoMat; i++)
                            {
                                Guid InfoMatAssessmentItemID = Guid.Parse(form["InfoMatAssessmentItemID" + i]);
                                int InfoMatNum = int.Parse(form["InfoMatNum" + i]);
                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;

                                    if (_paper.SelectUA(Test.UserID, InfoMatAssessmentItemID) == 2)
                                    {
                                        UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, InfoMatAssessmentItemID).AssessmentItemID;
                                        UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, InfoMatAssessmentItemID).UserAssessmentCountID;
                                        UAC.Count = _paper.SelectUAC(Test.UserID, InfoMatAssessmentItemID).Count + 1;
                                        _paper.UpdataUAC(UAC);
                                    }
                                    else
                                    {
                                        UAC.AssessmentItemID = InfoMatAssessmentItemID;
                                        UAC.Count = 1;
                                        _paper.CreateUAC(UAC);
                                    }
                                }

                                for (int i1 = 0; i1 < InfoMatNum; i1++)
                                {
                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                    Guid InfoMatQuestionID = Guid.Parse(form["InfoMatQuestionID" + i + "_" + i1]);
                                    string InfoMatUserAnswer = form["InfoMatAnswer" + i + "_" + i1] == null ? string.Empty : form["InfoMatAnswer" + i + "_" + i1];
                                    string InfoMatUserAnswer1 = InfoMatUserAnswer.Replace(" ", "");
                                    InfoMatUserAnswer1 = InfoMatUserAnswer1.ToUpper();

                                    string InfoMatAnswerValue = form["InfoMatAnswerValue" + i + "_" + i1] == null ? string.Empty : form["InfoMatAnswerValue" + i + "_" + i1];
                                    string InfoMatAnswerValue1 = InfoMatAnswerValue.Replace(" ", "");
                                    InfoMatAnswerValue1 = InfoMatAnswerValue1.ToUpper();

                                    int Rigth = 0;
                                    if (InfoMatAnswerValue1 == InfoMatUserAnswer1)
                                    {
                                        TotalScore += double.Parse(form["InfoMatScoreQuestion" + i + "_" + i1]);
                                        TestAnswer.IsRight = true;
                                        InfoMatCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    TestAnswer.Answer = InfoMatAnswerValue;
                                    TestAnswer.AnswerContent = _paper.AnswerContent(InfoMatQuestionID, InfoMatAnswerValue);
                                    TestAnswer.UserAnswer = InfoMatUserAnswer;
                                    TestAnswer.QuestionID = InfoMatQuestionID;
                                    TestAnswer.AssessmentItemID = InfoMatAssessmentItemID;
                                    TestAnswer.ItemTypeID = 9;
                                    TestAnswer.ItemAnswerTime = InfoMatTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    //
                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(InfoMatQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (!KpInfo.KnowledgePointID.Contains(KpID))
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpID == KpInfo.KnowledgePointID[j])
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.Time = DateTime.Now;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.ItemTypeID = 9;
                            SspTPI.CorrectItemNumber = InfoMatCorrectNum;
                            SspTPI.TotalItemNumber = InfoMatTotalNum;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);

                            _paper.CreateTATI(SspTPI);
                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }

                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;
                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                        }
                        #endregion

                        #region 完型填空
                        if (Cp > 0)
                        {
                            int CpTotalNum = int.Parse(form["CpTotalNum"]);
                            string Time1 = form["time9"];
                            int tempTime1 = form["temptime9"] == "" || form["temptime9"] == null ? 0 : int.Parse(form["temptime9"]);
                            int CpTime = 0;
                            if (Time1 != "")
                            {
                                CpTime = (int.Parse(Time1) + tempTime1) / CpTotalNum;
                            }
                            else
                            {
                                CpTime = 0;
                            }
                            int CpCorrectNum = 0;
                            for (int i = 0; i < Cp; i++)
                            {
                                Guid CpAssessmentItemID = Guid.Parse(form["CpAssessmentItemID" + i]);
                                int CpNum = int.Parse(form["CpNum" + i]);

                                if (PaperState != 9 && PaperState != 10)
                                {
                                    CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                    UAC.UserID = Test.UserID;
                                    if (_paper.SelectUA(Test.UserID, CpAssessmentItemID) == 2)
                                    {
                                        UAC.AssessmentItemID = _paper.SelectUAC(Test.UserID, CpAssessmentItemID).AssessmentItemID;
                                        UAC.UserAssessmentCountID = _paper.SelectUAC(Test.UserID, CpAssessmentItemID).UserAssessmentCountID;
                                        UAC.Count = _paper.SelectUAC(Test.UserID, CpAssessmentItemID).Count + 1;
                                        _paper.UpdataUAC(UAC);
                                    }
                                    else
                                    {
                                        UAC.AssessmentItemID = CpAssessmentItemID;
                                        UAC.Count = 1;
                                        _paper.CreateUAC(UAC);
                                    }
                                }
                                

                                for (int i1 = 0; i1 < CpNum; i1++)
                                {
                                    int Rigth = 0;
                                    CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();

                                    Guid CpQuestionID = Guid.Parse(form["CpQuestionID" + i + "_" + i1]);
                                    string CpUserAnswer = form["CpRadio" + i + "_" + i1];
                                    string CpAnswerValue = form["CpAnswerValue" + i + "_" + i1];
                                    if (CpAnswerValue == CpUserAnswer)
                                    {
                                        TotalScore += double.Parse(form["CpScoreQuestion" + i + "_" + i1]);
                                        TestAnswer.IsRight = true;
                                        CpCorrectNum += 1;
                                        Rigth = 1;
                                    }
                                    else
                                    {
                                        TestAnswer.IsRight = false;
                                    }
                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserID = Test.UserID;
                                    TestAnswer.Answer = CpAnswerValue;
                                    TestAnswer.AnswerContent = _paper.AnswerContent(CpQuestionID, CpAnswerValue);
                                    TestAnswer.UserAnswer = CpUserAnswer;
                                    TestAnswer.QuestionID = CpQuestionID;
                                    TestAnswer.AssessmentItemID = CpAssessmentItemID;
                                    TestAnswer.ItemTypeID = 8;
                                    TestAnswer.ItemAnswerTime = CpTime;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTestAnswer(TestAnswer);

                                    //
                                    if (PaperState != 9 && PaperState != 10)
                                    {
                                        List<Guid> KpIDList = new List<Guid>();
                                        KpIDList = _paper.SelectKnowledge(CpQuestionID);


                                        foreach (Guid KpID in KpIDList)
                                        {

                                            if (!KpInfo.KnowledgePointID.Contains(KpID))
                                            {
                                                if (Rigth == 1)
                                                {
                                                    KpInfo.CorrectCount.Add(1);
                                                }
                                                else
                                                {
                                                    KpInfo.CorrectCount.Add(0);
                                                }
                                                KpInfo.KnowledgePointID.Add(KpID);
                                                KpInfo.TotalCount.Add(1);
                                                KpInfo.AverageTime.Add(Convert.ToDouble(TestAnswer.ItemAnswerTime));
                                            }
                                            else
                                            {
                                                for (int j = 0; j < KpInfo.KnowledgePointID.Count; j++)
                                                {
                                                    if (KpID == KpInfo.KnowledgePointID[j])
                                                    {
                                                        if (Rigth == 1)
                                                        {
                                                            KpInfo.CorrectCount[j] += 1;
                                                        }
                                                        KpInfo.TotalCount[j] += 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            CEDTS_TestAnswerTypeInfo SspTPI = new CEDTS_TestAnswerTypeInfo();
                            SspTPI.TestID = Test.TestID;
                            SspTPI.UserID = Test.UserID;
                            SspTPI.Time = DateTime.Now;
                            SspTPI.PaperID = Test.PaperID;
                            SspTPI.ItemTypeID = 8;
                            SspTPI.CorrectItemNumber = CpCorrectNum;
                            SspTPI.TotalItemNumber = CpTotalNum;
                            SspTPI.CorrectRate = Convert.ToDouble(SspTPI.CorrectItemNumber) / Convert.ToDouble(SspTPI.TotalItemNumber);

                            _paper.CreateTATI(SspTPI);

                            if (PaperState != 9 && PaperState != 10)
                            {
                                CEDTS_UserAnswerInfo SspUAI = new CEDTS_UserAnswerInfo();
                                SspUAI.ItemTypeID = SspTPI.ItemTypeID;
                                SspUAI.UserID = SspTPI.UserID;

                                CEDTS_SmapleAnswerTypeInfo SspSATI = new CEDTS_SmapleAnswerTypeInfo();
                                SspSATI.ItemTypeID = SspTPI.ItemTypeID;
                                if (_paper.SelectSATI(SspSATI.ItemTypeID) == 2)
                                {
                                    if (_paper.SelectUAINum(SspUAI.UserID, SspUAI.ItemTypeID) == 2)
                                    {
                                        SspUAI.UATI_ID = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).UATI_ID;
                                        SspUAI.TotalCount = _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + 1;
                                        SspUAI.CorrectRate = (_paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).CorrectRate * _paper.SelectUAI(SspUAI.UserID, SspUAI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspUAI.TotalCount;
                                        _paper.UpdataUATI(SspUAI);
                                    }
                                    else
                                    {
                                        SspUAI.TotalCount = 1;
                                        SspUAI.CorrectRate = SspTPI.CorrectRate;
                                        _paper.CreateUATI(SspUAI);
                                    }
                                    SspSATI.SATI_ID = _paper.SelectSMAP(SspSATI.ItemTypeID).SATI_ID;
                                    SspSATI.TotalCount = _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + 1;
                                    SspSATI.CorrectRate = (_paper.SelectSMAP(SspSATI.ItemTypeID).CorrectRate * _paper.SelectSMAP(SspSATI.ItemTypeID).TotalCount + SspTPI.CorrectRate) / SspSATI.TotalCount;
                                    _paper.UpdataSATI(SspSATI);

                                }
                                else
                                {
                                    SspUAI.TotalCount = 1;
                                    SspUAI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateUATI(SspUAI);
                                    SspSATI.TotalCount = 1;
                                    SspSATI.CorrectRate = SspTPI.CorrectRate;
                                    _paper.CreateSATI(SspSATI);
                                }
                            }
                            
                        }
                        #endregion

                        #region 知识点统计
                        if (PaperState != 9 && PaperState != 10)
                        {
                            for (int k = 0; k < KpInfo.KnowledgePointID.Count; k++)
                            {
                                CEDTS_SampleKnowledgeInfomation SKI = new CEDTS_SampleKnowledgeInfomation();
                                CEDTS_TestAnswerKnowledgePoint TestAKP = new CEDTS_TestAnswerKnowledgePoint();
                                CEDTS_UserKnowledgeInfomation UKI = new CEDTS_UserKnowledgeInfomation();

                                SKI.KnowledgePointID = KpInfo.KnowledgePointID[k];

                                TestAKP.TestID = Test.TestID;
                                TestAKP.PaperID = Test.PaperID;
                                TestAKP.UserID = Test.UserID;
                                TestAKP.KnowledgePointID = KpInfo.KnowledgePointID[k];
                                TestAKP.CorrectItemNumber = KpInfo.CorrectCount[k];//知识点正确题数
                                TestAKP.TotalItemNumber = KpInfo.TotalCount[k];//知识点总题数
                                TestAKP.CorrectRate = Convert.ToDouble(TestAKP.CorrectItemNumber) / Convert.ToDouble(TestAKP.TotalItemNumber);//知识点正确率
                                TestAKP.AverageTime = KpInfo.AverageTime[k];//知识点平均时间

                                TestAKP.Time = DateTime.Now;




                                UKI.UserID = Test.UserID;
                                UKI.KnowledgePointID = KpInfo.KnowledgePointID[k];

                                if (_paper.SelectSIK(KpInfo.KnowledgePointID[k]) == 2)
                                {
                                    SKI.SKII_ID = _paper.SelectSKI(KpInfo.KnowledgePointID[k]).SKII_ID;
                                    SKI.TotalCount = _paper.SelectSKI(KpInfo.KnowledgePointID[k]).TotalCount + 1;//样本空间知识点练习次数
                                    SKI.AverageTime = (_paper.SelectSKI(KpInfo.KnowledgePointID[k]).AverageTime * _paper.SelectSKI(KpInfo.KnowledgePointID[k]).TotalCount + KpInfo.AverageTime[k]) / SKI.TotalCount;//样本空间知识点练习平均时间
                                    SKI.CorrectRate = (_paper.SelectSKI(KpInfo.KnowledgePointID[k]).CorrectRate * _paper.SelectSKI(KpInfo.KnowledgePointID[k]).TotalCount + TestAKP.CorrectRate) / SKI.TotalCount;//样本空间知识点正确率
                                    if (TestAKP.AverageTime + SKI.AverageTime * ValueA - TestAKP.AverageTime * ValueA == 0.0)
                                    {
                                        TestAKP.KP_MasterRate = 0.0;
                                    }
                                    else
                                    {
                                        TestAKP.KP_MasterRate = (ValueA * SKI.AverageTime * TestAKP.CorrectRate) / (TestAKP.AverageTime + SKI.AverageTime * ValueA - TestAKP.AverageTime * ValueA);//知识点掌握率
                                        //TestAKP.KP_MasterRate = (TestAKP.CorrectRate * SKI.AverageTime * 3.2) / (TestAKP.AverageTime + 3.2 * SKI.AverageTime);
                                    }
                                    SKI.KP_MasterRate = (_paper.SelectSKI(KpInfo.KnowledgePointID[k]).KP_MasterRate * _paper.SelectSKI(KpInfo.KnowledgePointID[k]).TotalCount + TestAKP.KP_MasterRate) / SKI.TotalCount;//样本空间知识点掌握率

                                    _paper.CreateTAKP(TestAKP);
                                    _paper.UpdataSKI(SKI);

                                    if (_paper.SelectUkINum(Test.UserID, KpInfo.KnowledgePointID[k]) == 2)
                                    {
                                        UKI.UKII_ID = _paper.SelectUKI(KpInfo.KnowledgePointID[k], Test.UserID).UKII_ID;
                                        UKI.TotalCount = _paper.SelectUKI(KpInfo.KnowledgePointID[k], Test.UserID).TotalCount + 1;
                                        UKI.AverageTime = (_paper.SelectUKI(KpInfo.KnowledgePointID[k], Test.UserID).AverageTime * _paper.SelectUKI(KpInfo.KnowledgePointID[k], Test.UserID).TotalCount + KpInfo.AverageTime[k]) / UKI.TotalCount;
                                        UKI.CorrectRate = (_paper.SelectUKI(KpInfo.KnowledgePointID[k], Test.UserID).CorrectRate * _paper.SelectUKI(KpInfo.KnowledgePointID[k], Test.UserID).TotalCount + TestAKP.CorrectRate) / UKI.TotalCount;
                                        UKI.KP_MasterRate = (_paper.SelectUKI(KpInfo.KnowledgePointID[k], Test.UserID).KP_MasterRate * _paper.SelectUKI(KpInfo.KnowledgePointID[k], Test.UserID).TotalCount + TestAKP.KP_MasterRate) / UKI.TotalCount;
                                        _paper.UpdataUKI(UKI);
                                    }
                                    else
                                    {
                                        UKI.CorrectRate = TestAKP.CorrectRate;
                                        UKI.AverageTime = TestAKP.AverageTime;
                                        UKI.TotalCount = 1;
                                        UKI.KP_MasterRate = TestAKP.KP_MasterRate;
                                        _paper.CreateUKI(UKI);
                                    }
                                }
                                else
                                {
                                    SKI.CorrectRate = TestAKP.CorrectRate;
                                    SKI.AverageTime = TestAKP.AverageTime;
                                    SKI.TotalCount = 1;

                                    if (TestAKP.AverageTime + SKI.AverageTime * ValueA - TestAKP.AverageTime * ValueA == 0.0)
                                    {
                                        TestAKP.KP_MasterRate = 0.0;
                                    }
                                    else
                                    {
                                        TestAKP.KP_MasterRate = (ValueA * SKI.AverageTime * TestAKP.CorrectRate) / (TestAKP.AverageTime + SKI.AverageTime * ValueA - TestAKP.AverageTime * ValueA);
                                        //TestAKP.KP_MasterRate = (TestAKP.CorrectRate * SKI.AverageTime * 3.2) / (TestAKP.AverageTime + 3.2 * SKI.AverageTime);
                                    }
                                    SKI.KP_MasterRate = TestAKP.KP_MasterRate;
                                    _paper.CreateTAKP(TestAKP);
                                    _paper.CreateSKI(SKI);

                                    UKI.CorrectRate = TestAKP.CorrectRate;
                                    UKI.AverageTime = TestAKP.AverageTime;
                                    UKI.TotalCount = 1;
                                    UKI.KP_MasterRate = TestAKP.KP_MasterRate;
                                    _paper.CreateUKI(UKI);
                                }
                            }
                        }
                        
                        #endregion

                        Test.TotalScore = TotalScore;
                        _paper.UpdataTest(Test);

                        ////计算学习者状态
                        //bool existsFlag = true; //判断是否在数据库的status表中有这个user
                        ////计算学习者状态过程
                        ////第一步  读取所有学生的各个知识点的掌握率组成一个二维数组
                        //List<Array> t = _paper.getAllKnowledgeRate();
                        ////进行处理得到这个学生的判定
                        //string learnerStatus="学习者状态a";

                        //if (existsFlag)
                        //{

                        //    //update数据表；
                        //}
                        //else
                        //{
                        //    CEDTS_LearnerStatus ls = new CEDTS_LearnerStatus();
                        //   ls.userId= _paper.SelectUserID(User.Identity.Name);
                        //   ls.status = learnerStatus;
                        //    _paper.insertStatusToTable(ls);
                        //}
                    }
                    #endregion

                    #region 中断试卷保存
                    if (temp == "2")
                    {
                        //保存到答卷表
                        CEDTS_Test Test = new CEDTS_Test();
                        string id = form["Continues"];
                        if (id == "")
                        {
                            Test.TestID = Guid.NewGuid();
                            Test.StartDate = DateTime.Parse(form["StartTime"]);
                            Test.UserID = _paper.SelectUserID(User.Identity.Name);
                            Test.IsFinished = false;
                            Test.IsChecked = 0;
                            Test.TotalScore = 0;
                            Test.PaperID = Guid.Parse(form["PaperID"]);
                            Test.FinishDate = DateTime.Now;
                            TimeSpan ts = Test.FinishDate.Value - Test.StartDate.Value;
                            Test.TotalTime = ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;

                            _paper.CreateTest(Test);
                        }
                        else
                        {
                            TimeSpan ts = DateTime.Now - DateTime.Parse(form["StartTime"]);
                            int t1 = form["temptime1"] == null || form["temptime1"] == "" ? 0 : int.Parse(form["temptime1"]);
                            int t2 = form["temptime2"] == null || form["temptime2"] == "" ? 0 : int.Parse(form["temptime2"]);
                            int t3 = form["temptime3"] == null || form["temptime3"] == "" ? 0 : int.Parse(form["temptime3"]);
                            int t4 = form["temptime4"] == null || form["temptime4"] == "" ? 0 : int.Parse(form["temptime4"]);
                            int t5 = form["temptime5"] == null || form["temptime5"] == "" ? 0 : int.Parse(form["temptime5"]);
                            int t6 = form["temptime6"] == null || form["temptime6"] == "" ? 0 : int.Parse(form["temptime6"]);
                            int t7 = form["temptime7"] == null || form["temptime7"] == "" ? 0 : int.Parse(form["temptime7"]);
                            int t8 = form["temptime8"] == null || form["temptime8"] == "" ? 0 : int.Parse(form["temptime8"]);
                            int t9 = form["temptime9"] == null || form["temptime9"] == "" ? 0 : int.Parse(form["temptime9"]);

                            int TotalTime = ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds + t1 + t2 + t3 + t4 + t5 + t6 + t7 + t8 + t9;
                            Guid TestID = Guid.Parse(id);

                            Test.TestID = TestID;
                            Test.UserID = _paper.SelectUserID(User.Identity.Name);
                            Test.PaperID = Guid.Parse(form["PaperID"]);
                            Test.IsFinished = false;
                            Test.IsChecked = 0;
                            Test.FinishDate = DateTime.Now;
                            Test.TotalTime = TotalTime;
                            Test.TotalScore = 0;
                            _paper.UpdataTest(Test);
                            _paper.DeleteTempTime(TestID);
                            _paper.DeleteTempTest(TestID);
                        }

                        //判断试卷中是否有快速阅读题型
                        int Sspc = 0; int Slpo = 0; int Llpo = 0; int Rlpo = 0; int Lpc = 0; int Rpc = 0; int Rpo = 0; int InfoMat = 0; int Cp = 0;
                        string SSspc = form["Sspc"];
                        if (SSspc != "")
                        {
                            Sspc = int.Parse(SSspc);
                        }
                        string SSlpo = form["Slpo"];
                        if (SSlpo != "")
                        {
                            Slpo = int.Parse(SSlpo);
                        }
                        string SLlpo = form["Llpo"];
                        if (SLlpo != "")
                        {
                            Llpo = int.Parse(SLlpo);
                        }
                        string SRlpo = form["Rlpo"];
                        if (SRlpo != "")
                        {
                            Rlpo = int.Parse(SRlpo);
                        }
                        string SLpc = form["Lpc"];
                        if (SLpc != "")
                        {
                            Lpc = int.Parse(SLpc);
                        }
                        string SRpc = form["Rpc"];
                        if (SRpc != "")
                        {
                            Rpc = int.Parse(SRpc);
                        }
                        string SRpo = form["Rpo"];
                        if (SRpo != "")
                        {
                            Rpo = int.Parse(SRpo);
                        }
                        string SInfoMat = form["InfoMat"];
                        if (SInfoMat != "")
                        {
                            InfoMat = int.Parse(SInfoMat);
                        }
                        string SCP = form["CP"];
                        if (SCP != "")
                        {
                            Cp = int.Parse(SCP);
                        }

                        int Number = 1;
                        int TimeNumber = 1;
                        #region 快速阅读
                        if (Sspc > 0)
                        {
                            int SspTotalNum = int.Parse(form["SspcTotalNum"]);
                            string Time1 = form["time1"];
                            int tempTime1 = form["temptime1"] == "" || form["temptime1"] == null ? 0 : int.Parse(form["temptime1"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }
                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;

                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;
                            for (int i = 0; i < Sspc; i++) //题型下的大题循环
                            {
                                int ChoiceNum = int.Parse(form["ChoiceNum" + i]);// 选择题总数
                                int TermNum = int.Parse(form["TermNum" + i]);// 填空题总数
                                Guid AssessmentItemID = Guid.Parse(form["SspAssessmentItemID" + i]);


                                for (int i1 = 0; i1 < ChoiceNum; i1++)
                                {
                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();

                                    Guid SspQuestionID = Guid.Parse(form["SspQuestionID" + i + "_" + i1]);
                                    string SspUserAnswer = form["SspRadio" + i + "_" + i1];
                                    string SspAnswerValue = form["SspAnswerValue" + i + "_" + i1];



                                    //添加信息到tmpTestAnswer表

                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserAnswer = SspUserAnswer;
                                    TestAnswer.QuestionID = SspQuestionID;
                                    TestAnswer.AssessmentItemID = AssessmentItemID;
                                    //TestAnswer.AssessmentItemID = 1;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);

                                }
                                for (int i2 = 0; i2 < TermNum; i2++)
                                {
                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();
                                    Guid SspQuestionID = Guid.Parse(form["SspQuestionID" + i + "_" + (ChoiceNum + i2)]);
                                    string SspUserAnswer = form["SspAnswer" + i + "_" + i2] == null ? string.Empty : form["SspAnswer" + i + "_" + i2];
                                    string SspUserAnswer1 = SspUserAnswer.Replace(" ", "");
                                    SspUserAnswer1 = SspUserAnswer1.ToUpper();




                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    if (SspUserAnswer == null)
                                    {
                                        SspUserAnswer = "";
                                    }
                                    TestAnswer.UserAnswer = SspUserAnswer;
                                    TestAnswer.AssessmentItemID = AssessmentItemID;
                                    TestAnswer.QuestionID = SspQuestionID;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);

                                }

                            }
                        }

                        #endregion

                        #region 短对话
                        if (Slpo > 0)
                        {
                            string Time1 = form["time2"];
                            int tempTime1 = form["temptime2"] == "" || form["temptime2"] == null ? 0 : int.Parse(form["temptime2"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }
                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;
                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;
                            for (int i = 0; i < Slpo; i++)
                            {
                                Guid SlpoAssessmentItemID = Guid.Parse(form["SlpAssessmentItemID" + i]);


                                CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();
                                Guid SlpoQuestionID = Guid.Parse(form["SlpQuestionID" + i]);
                                string SlpoUserAnswer = form["SlpRadio" + i];
                                string SlpoAnswerValue = form["SlpAnswerValue" + i];


                                //添加信息到TestAnswer表
                                TestAnswer.TestID = Test.TestID;
                                TestAnswer.UserAnswer = SlpoUserAnswer;
                                TestAnswer.QuestionID = SlpoQuestionID;
                                TestAnswer.AssessmentItemID = SlpoAssessmentItemID;
                                TestAnswer.Number = Number;
                                Number += 1;
                                _paper.CreateTempTestAnswer(TestAnswer);
                            }
                        }
                        #endregion

                        #region 长对话
                        if (Llpo > 0)
                        {
                            int LlpoTotalNum = int.Parse(form["LlpoTotalNum"]);
                            string Time1 = form["time3"];
                            int tempTime1 = form["temptime3"] == "" || form["temptime3"] == null ? 0 : int.Parse(form["temptime3"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }
                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;
                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;

                            for (int i = 0; i < Llpo; i++)
                            {
                                Guid LlpoAssessmentItemID = Guid.Parse(form["LlpAssessmentItemID" + i]);
                                int LlpoNum = int.Parse(form["LlpNum" + i]);



                                for (int i1 = 0; i1 < LlpoNum; i1++)
                                {
                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();

                                    Guid LlpoQuestionID = Guid.Parse(form["LlpQuestionID" + i + "_" + i1]);
                                    string LlpoUserAnswer = form["LlpRadio" + i + "_" + i1];
                                    string LlpoAnswerValue = form["LlpAnswerValue" + i + "_" + i1];



                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserAnswer = LlpoUserAnswer;
                                    TestAnswer.QuestionID = LlpoQuestionID;
                                    TestAnswer.AssessmentItemID = LlpoAssessmentItemID;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);
                                }
                            }
                        }
                        #endregion

                        #region 短文听力理解
                        if (Rlpo > 0)
                        {
                            int RlpoTotalNum = int.Parse(form["RlpoTotalNum"]);
                            string Time1 = form["time4"];
                            int tempTime1 = form["temptime4"] == "" || form["temptime4"] == null ? 0 : int.Parse(form["temptime4"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }

                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;
                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;
                            for (int i = 0; i < Rlpo; i++)
                            {
                                Guid RlpoAssessmentItemID = Guid.Parse(form["RlpAssessmentItemID" + i]);
                                int LlpoNum = int.Parse(form["RlpNum" + i]);


                                for (int i1 = 0; i1 < LlpoNum; i1++)
                                {

                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();

                                    Guid RlpoQuestionID = Guid.Parse(form["RlpQuestionID" + i + "_" + i1]);
                                    string RlpoUserAnswer = form["RlpoRadio" + i + "_" + i1];
                                    string RlpoAnswerValue = form["RlpAnswerValue" + i + "_" + i1];


                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserAnswer = RlpoUserAnswer;
                                    TestAnswer.QuestionID = RlpoQuestionID;
                                    TestAnswer.AssessmentItemID = RlpoAssessmentItemID;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);
                                }
                            }
                        }
                        #endregion

                        #region 复合型听力
                        if (Lpc > 0)
                        {
                            int LpcTotalNum = int.Parse(form["LpcTotalNum"]);
                            string Time1 = form["time5"];
                            int tempTime1 = form["temptime5"] == "" || form["temptime5"] == null ? 0 : int.Parse(form["temptime5"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }
                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;
                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;
                            for (int i = 0; i < Lpc; i++)
                            {
                                Guid LpcAssessmentItemID = Guid.Parse(form["LpcAssessmentItemID" + i]);
                                int LpcNum = int.Parse(form["LpcNum" + i]);



                                for (int i1 = 0; i1 < LpcNum; i1++)
                                {
                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();
                                    Guid LpcQuestionID = Guid.Parse(form["LpcQuestionID" + i + "_" + i1]);
                                    string LpcUserAnswer = form["LpcAnswer" + i + "_" + i1] == null ? string.Empty : form["LpcAnswer" + i + "_" + i1];
                                    string LpcUserAnswer1 = LpcUserAnswer.Replace(" ", "");
                                    LpcUserAnswer1 = LpcUserAnswer1.ToUpper();

                                    string LpcAnswerValue = form["LpcAnswerValue" + i + "_" + i1] == null ? string.Empty : form["LpcAnswerValue" + i + "_" + i1];
                                    string LpcAnswerValue1 = LpcAnswerValue.Replace(" ", "");
                                    LpcAnswerValue1 = LpcAnswerValue1.ToUpper();



                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    if (LpcUserAnswer == null)
                                    {
                                        LpcUserAnswer = "";
                                    }
                                    TestAnswer.UserAnswer = LpcUserAnswer;
                                    TestAnswer.QuestionID = LpcQuestionID;
                                    TestAnswer.AssessmentItemID = LpcAssessmentItemID;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);
                                }
                            }
                        }
                        #endregion

                        #region 阅读理解-选词填空
                        if (Rpc > 0)
                        {

                            int RpcTotalNum = int.Parse(form["RpcTotalNum"]);
                            string Time1 = form["time6"];
                            int tempTime1 = form["temptime6"] == "" || form["temptime6"] == null ? 0 : int.Parse(form["temptime6"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }
                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;
                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;
                            for (int i = 0; i < Rpc; i++)
                            {
                                Guid RpcAssessmentItemID = Guid.Parse(form["RpcAssessmentItemID" + i]);
                                int RpcNum = int.Parse(form["RpcNum" + i]);



                                for (int i1 = 0; i1 < RpcNum; i1++)
                                {
                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();
                                    Guid RpcQuestionID = Guid.Parse(form["RpcQuestionID" + i + "_" + i1]);
                                    string RpcUserAnswer = form["RpcAnswer" + i + "_" + i1] == null ? string.Empty : form["RpcAnswer" + i + "_" + i1];
                                    string RpcUserAnswer1 = RpcUserAnswer.Replace(" ", "");
                                    RpcUserAnswer1 = RpcUserAnswer1.ToUpper();

                                    string RpcAnswerValue = form["RpcAnswerValue" + i + "_" + i1] == null ? string.Empty : form["RpcAnswerValue" + i + "_" + i1];
                                    string RpcAnswerValue1 = RpcAnswerValue.Replace(" ", "");
                                    RpcAnswerValue1 = RpcAnswerValue1.ToUpper();


                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserAnswer = RpcUserAnswer;
                                    TestAnswer.QuestionID = RpcQuestionID;
                                    TestAnswer.AssessmentItemID = RpcAssessmentItemID;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);
                                }
                            }
                        }
                        #endregion

                        #region 阅读理解-选择题型
                        if (Rpo > 0)
                        {
                            int RpoTotalNum = int.Parse(form["RpoTotalNum"]);
                            string Time1 = form["time7"];
                            int tempTime1 = form["temptime7"] == "" || form["temptime7"] == null ? 0 : int.Parse(form["temptime7"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }
                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;
                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;

                            for (int i = 0; i < Rpo; i++)
                            {
                                Guid RpoAssessmentItemID = Guid.Parse(form["RpoAssessmentItemID" + i]);
                                int LpoNum = int.Parse(form["RpoNum" + i]);


                                for (int i1 = 0; i1 < LpoNum; i1++)
                                {

                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();

                                    Guid RpoQuestionID = Guid.Parse(form["RpoQuestionID" + i + "_" + i1]);
                                    string RpoUserAnswer = form["RpoRadio" + i + "_" + i1];
                                    string RpoAnswerValue = form["RpoAnswerValue" + i + "_" + i1];


                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserAnswer = RpoUserAnswer;
                                    TestAnswer.QuestionID = RpoQuestionID;
                                    TestAnswer.AssessmentItemID = RpoAssessmentItemID;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);
                                }
                            }
                        }
                        #endregion

                        #region 阅读理解-信息匹配
                        if (InfoMat > 0)
                        {

                            int InfoMatTotalNum = int.Parse(form["InfoMatTotalNum"]);
                            string Time1 = form["time8"];
                            int tempTime1 = form["temptime8"] == "" || form["temptime8"] == null ? 0 : int.Parse(form["temptime8"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }
                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;
                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;
                            for (int i = 0; i < InfoMat; i++)
                            {
                                Guid InfoMatAssessmentItemID = Guid.Parse(form["InfoMatAssessmentItemID" + i]);
                                int InfoMatNum = int.Parse(form["InfoMatNum" + i]);



                                for (int i1 = 0; i1 < InfoMatNum; i1++)
                                {
                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();
                                    Guid InfoMatQuestionID = Guid.Parse(form["InfoMatQuestionID" + i + "_" + i1]);
                                    string InfoMatUserAnswer = form["InfoMatAnswer" + i + "_" + i1] == null ? string.Empty : form["InfoMatAnswer" + i + "_" + i1];
                                    string InfoMatUserAnswer1 = InfoMatUserAnswer.Replace(" ", "");
                                    InfoMatUserAnswer1 = InfoMatUserAnswer1.ToUpper();

                                    string InfoMatAnswerValue = form["InfoMatAnswerValue" + i + "_" + i1] == null ? string.Empty : form["InfoMatAnswerValue" + i + "_" + i1];
                                    string InfoMatAnswerValue1 = InfoMatAnswerValue.Replace(" ", "");
                                    InfoMatAnswerValue1 = InfoMatAnswerValue1.ToUpper();


                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserAnswer = InfoMatUserAnswer;
                                    TestAnswer.QuestionID = InfoMatQuestionID;
                                    TestAnswer.AssessmentItemID = InfoMatAssessmentItemID;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);
                                }
                            }
                        }
                        #endregion

                        #region 完型填空
                        if (Cp > 0)
                        {
                            int CpTotalNum = int.Parse(form["CpTotalNum"]);
                            string Time1 = form["time9"];
                            int tempTime1 = form["temptime9"] == "" || form["temptime9"] == null ? 0 : int.Parse(form["temptime9"]);
                            if (Time1 == "")
                            {
                                Time1 = "0";
                            }
                            CEDTS_tempAssessmentItemTime TempTestTime = new CEDTS_tempAssessmentItemTime();
                            TempTestTime.TestID = Test.TestID;
                            TempTestTime.Time = (int.Parse(Time1) + tempTime1);
                            TempTestTime.Number = TimeNumber;
                            _paper.CreateTestTime(TempTestTime);
                            TimeNumber += 1;
                            for (int i = 0; i < Cp; i++)
                            {
                                Guid CpAssessmentItemID = Guid.Parse(form["CpAssessmentItemID" + i]);
                                int CpNum = int.Parse(form["CpNum" + i]);



                                for (int i1 = 0; i1 < CpNum; i1++)
                                {

                                    CEDTS_tempTestAnswer TestAnswer = new CEDTS_tempTestAnswer();

                                    Guid CpQuestionID = Guid.Parse(form["CpQuestionID" + i + "_" + i1]);
                                    string CpUserAnswer = form["CpRadio" + i + "_" + i1];
                                    string CpAnswerValue = form["CpAnswerValue" + i + "_" + i1];

                                    //添加信息到TestAnswer表
                                    TestAnswer.TestID = Test.TestID;
                                    TestAnswer.UserAnswer = CpUserAnswer;
                                    TestAnswer.QuestionID = CpQuestionID;
                                    TestAnswer.AssessmentItemID = CpAssessmentItemID;
                                    TestAnswer.Number = Number;
                                    Number += 1;
                                    _paper.CreateTempTestAnswer(TestAnswer);
                                }
                            }
                        }
                        #endregion

                    }
                    #endregion
                    tran.Complete();
                }

            }

            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
            return RedirectToAction("UserTestInfo");
        }

        public ActionResult GetJosnData()
        {
            if (Continues == "1")
            {
                Continues = "2";
                JsonResult i = Json(ppc);
                return Json(ppc);
            }
            else
            {
                return Json(pp);
            }

        }


        public ActionResult ContinuesPractice(Guid TestID)
        {
            ppc = MakePaperContinue(TestID);
            Continues = "1";
            return RedirectToAction("PaperShow");
        }

        #region 继续答卷
        public PaperTotalContinue MakePaperContinue(Guid testID)
        {
            PaperTotal pt = new PaperTotal();
            List<SkimmingScanningPartCompletion> sspcList = new List<SkimmingScanningPartCompletion>();//临时存储快速阅读题型
            List<Listen> slpoList = new List<Listen>();//临时存储短对话听力题型
            List<Listen> llpoList = new List<Listen>();//临时存储长对话听力题型
            List<Listen> rlpoList = new List<Listen>();//临时存储听力理解题型
            List<Listen> lpcList = new List<Listen>();//临时存储复合型听力题型
            List<ReadingPartCompletion> rpcList = new List<ReadingPartCompletion>();//临时存储阅读理解选词填空题型
            List<ReadingPartOption> rpoList = new List<ReadingPartOption>();//临时存储阅读理解选择题型
            List<InfoMatchingCompletion> infoMatList = new List<InfoMatchingCompletion>();//临时存储阅读理解信息匹配
            List<ClozePart> cpList = new List<ClozePart>();//临时存储完型填空题型
            List<Guid> assessmentidList = new List<Guid>();//大题ID
            List<Guid> questionidList = new List<Guid>();//小题ID
            List<int> timeList = new List<int>();//大题时间集合
            List<string> userDataList = new List<string>();//用户答案集合，没有填的答案为null

            List<CEDTS_tempTestAnswer> ctaList = _paper.getTempTestAnswerList(testID);
            foreach (var item in ctaList)
            {
                questionidList.Add(item.QuestionID.Value);
                userDataList.Add(item.UserAnswer);
                if (!assessmentidList.Contains(item.AssessmentItemID.Value))
                {
                    assessmentidList.Add(item.AssessmentItemID.Value);
                }
            }

            List<CEDTS_tempAssessmentItemTime> ctatList = _paper.getTempAssessmentTime(testID);
            foreach (var item in ctatList)
            {
                timeList.Add(item.Time.Value);
            }

            PaperTotalContinue ptc = new PaperTotalContinue();

            ptc.timeList = timeList;
            ptc.userData = userDataList;

            CEDTS_Test test = _paper.SelectTestInfo(testID);
            Guid paperId = test.PaperID;
            CEDTS_Paper paper = _paper.SelectPaper(paperId);
            pt.UserID = paper.UserID.Value;
            pt.UpdateUserID = paper.UpdateUserID.Value;
            pt.UpdateTime = paper.UpdateTime.Value;
            pt.Type = paper.Type;
            pt.Title = paper.Title;
            pt.Score = paper.Score.Value;
            pt.PaperID = paper.PaperID;
            pt.PaperContent = paper.PaperContent;
            pt.Duration = paper.Duration.Value;
            pt.Difficult = paper.Difficult.Value;
            pt.Description = paper.Description;
            pt.CreateTime = paper.CreateTime.Value;

            foreach (var item in assessmentidList)
            {
                var ai = _paper.SelectAssessmentItem(item);

                #region 快速阅读理解赋值
                if (ai.ItemTypeID == 1)
                {
                    SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
                        {
                            questionList.Remove(question);
                        }
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
                }
                #endregion

                #region 短对话听力赋值
                if (ai.ItemTypeID == 2)
                {
                    Listen listen = new Listen();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
                        {
                            questionList.Remove(question);
                        }
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
                }
                #endregion

                #region 长对话听力赋值
                if (ai.ItemTypeID == 3)
                {
                    Listen listen = new Listen();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
                        {
                            questionList.Remove(question);
                        }
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
                }
                #endregion

                #region 短文理解听力赋值
                if (ai.ItemTypeID == 4)
                {
                    Listen listen = new Listen();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
                        {
                            questionList.Remove(question);
                        }
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
                }
                #endregion

                #region 复合型听力赋值
                if (ai.ItemTypeID == 5)
                {
                    Listen listen = new Listen();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    listen.Script = ai.Original;
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
                        {
                            string script = "(_" + question.Order + "_)";
                            listen.Script = listen.Script.Replace(script, "(_<span style='color:Red'>" + question.Answer + "</span>_)");
                            questionList.Remove(question);
                        }
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
                }
                #endregion

                #region 阅读理解选词填空赋值
                if (ai.ItemTypeID == 6)
                {
                    ReadingPartCompletion rpc = new ReadingPartCompletion();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    rpc.Content = ai.Original;
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
                        {
                            string script = "(_" + question.Order + "_)";
                            rpc.Content = rpc.Content.Replace(script, "(_<span style='color:Red'>" + question.Answer + "</span>_)");
                            questionList.Remove(question);
                        }
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
                }

                #endregion

                #region 阅读理解选择题型赋值
                if (ai.ItemTypeID == 7)
                {
                    ReadingPartOption rpo = new ReadingPartOption();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
                        {
                            questionList.Remove(question);
                        }
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
                }
                #endregion

                #region 阅读理解信息匹配赋值
                if (ai.ItemTypeID == 9)
                {
                    InfoMatchingCompletion infoMat = new InfoMatchingCompletion();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    infoMat.Content = ai.Original;
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
                        {
                            string script = "(_" + question.Order + "_)";
                            infoMat.Content = infoMat.Content.Replace(script, "(_<span style='color:Red'>" + question.Answer + "</span>_)");
                            questionList.Remove(question);
                        }
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
                    infoMat.WordList.Add(es.ChoiceK);
                    infoMat.WordList.Add(es.ChoiceL);
                    infoMat.WordList.Add(es.ChoiceN);
                    infoMat.WordList.Add(es.ChoiceM);
                    infoMat.WordList.Add(es.ChoiceO);
                    infoMat.Info = info;
                    infoMatList.Add(infoMat);
                }

                #endregion

                #region 完型填空赋值
                if (ai.ItemTypeID == 8)
                {
                    ClozePart cp = new ClozePart();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ai.AssessmentItemID);
                    cp.Content = ai.Original;
                    foreach (var question in questionList)
                    {
                        if (!questionidList.Contains(question.QuestionID))
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
                            questionList.Remove(question);
                        }
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

                }
                #endregion
            }

            pt.SspcList = sspcList;
            pt.SlpoList = slpoList;
            pt.LlpoList = llpoList;
            pt.LpcList = lpcList;
            pt.RlpoList = rlpoList;
            pt.RpcList = rpcList;
            pt.RpoList = rpoList;
            pt.InfMatList = infoMatList;
            pt.CpList = cpList;

            ptc.paperTotal = pt;
            ptc.TestID = testID.ToString();
            return ptc;
        }
        #endregion

        /// <summary>
        /// 组卷方法
        /// </summary>
        /// <param name="aiList">试题ID集合</param>
        /// <param name="type">组卷方式</param>
        /// <returns>试卷总信息</returns>
        public PaperTotal MakePaper(List<CEDTS_AssessmentItem> aiList, int type, List<string> kk)
        {
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
            string typeTitle = string.Empty;
            switch (type)
            {
                case 1:
                    {
                        typeTitle = TempData["CeType2Title"].ToString();
                        typeName = "按题型组卷";
                        break;
                    }
                case 2:
                    {
                        typeTitle = TempData["CeType3Title"].ToString();
                        typeName = "按知识点组卷";

                        break;
                    }
                case 3:
                    {
                        typeTitle = "随机组卷" + DateTime.Now.ToString("yyyy-MM-dd");
                        typeName = "随机组卷";
                        break;
                    }
                case 4:
                    {
                        typeTitle = TempData["CeType4Title"].ToString();
                        typeName = "知识点弱项组卷";
                        break;
                    }
                default:
                    {
                        typeTitle = TempData["CeType5Title"].ToString();
                        typeName = "得分最大化组卷";
                        break;
                    }
            }

            paper.Title = typeTitle;
            paper.Type = typeName;
            paper.PaperID = Guid.NewGuid();
            paper.Score = Score;
            paper.Duration = Duration;
            paper.Difficult = ((int)((Difficult / aiList.Count) * 100)) / 100;
            paper.Description = "用户：" + User.Identity.Name + "在" + DateTime.Now + typeName;
            paper.CreateTime = DateTime.Now;
            paper.UpdateTime = DateTime.Now;
            paper.UserID = _paper.SelectUserID(User.Identity.Name);
            paper.UpdateUserID = paper.UserID;
            paper.State = 4;//4代表是用户从前台组的试卷
            _paper.SavePaper(paper);
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
                _paper.SavePaperAssessmentItem(pa);
                #endregion

                #region 快速阅读理解赋值
                if (ai.ItemTypeID == 1)
                {
                    SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = new List<CEDTS_Question>();
                    if (type == 2 || type == 4 || type == 5)
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
                    if (type == 2 || type == 4 || type == 5)
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
                    if (type == 2 || type == 4 || type == 5)
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
                    if (type == 2 || type == 4 || type == 5)
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
                    if (type == 2 || type == 4 || type == 5)
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
                    if (type == 2 || type == 4 || type == 5)
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
                    if (type == 2 || type == 4 || type == 5)
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
                    if (type == 2 || type == 4 || type == 5)
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
            _paper.UpdetPaper(paper);
            #endregion

            TimeSpan ts = DateTime.Now - dt;

            return pt;
        }

        #region 做题记录查询

        public ActionResult TestRecord(int? id)
        {
            string UserName = User.Identity.Name;
            int UserId = _paper.SelectUserID(UserName);
            if (id == null)
            {
                if (TempData["id"] != null)
                {
                    id = int.Parse(TempData["id"].ToString());
                    TempData["id"] = id;
                }
                return View(_paper.TestRecord(id, UserId));
            }
            else
            {
                TempData["id"] = id;
                return RedirectToAction("UserTestInfo");
            }
        }
        #endregion

        #region 用户做题信息
        public ActionResult UserTestInfo()
        {

            int UserID = _paper.SelectUserID(User.Identity.Name);
            ViewData["ClassID"] = _paper.SelectUserInfo(User.Identity.Name).ClassID;
            ViewData["Vocabulary"] = _paper.SelectInsufficient(UserID).Vocabulary;
            ViewData["Grammar"] = _paper.SelectInsufficient(UserID).Grammar;
            ViewData["Comprehend"] = _paper.SelectInsufficient(UserID).Comprehend;
            ViewData["UserName"] = User.Identity.Name;
            ViewData["UserRole"] = _paper.SelectRole(User.Identity.Name);
            ViewData["PaperID"] = Guid.Parse(ConfigurationManager.AppSettings["PaperID"]);
            //ViewData["status"] = _paper.selectStatus(UserID);
            //ViewData["status"] = "学习者状态a";
            return View();
        }
        #endregion

        #region 用户作业信息
        public ActionResult UserTask(int? id)
        {
            string ClassIDStr = _paper.SelectUserInfo(User.Identity.Name).ClassID.ToString();
            int UserID = _paper.SelectUserID(User.Identity.Name);
            ViewData["ClassID"] = ClassIDStr;
            Guid ClassID = Guid.Parse(ClassIDStr);
            List<CEDTS_PaperAssignClass> TaskList = _paper.SelectAssignedPaper(ClassID);
            List<CEDTS_PaperAssignClass> UndoList = new List<CEDTS_PaperAssignClass>();
            List<AssignedTask> AssignedTask = new List<AssignedTask>();
            foreach (var task in TaskList)
            {
                string taskpaperidstr = task.PaperID.ToString();
                Guid taskpaperid = Guid.Parse(taskpaperidstr);
                if (_paper.SelectTestByPaperID(taskpaperid, UserID) != 3)
                {
                    UndoList.Add(task);
                }
            }
            foreach (var undotask in UndoList)
            {
                AssignedTask eachtask = new AssignedTask();
                eachtask.PaperID = Guid.Parse(undotask.PaperID.ToString());
                eachtask.TaskName = _paper.SelectPaper(eachtask.PaperID).Title;
                eachtask.AssignTime = DateTime.Parse(undotask.CreateTime.ToString());
                if (_paper.SelectTestByPaperID(eachtask.PaperID, UserID) == 1)
                {
                    eachtask.IsFinished = false;
                    eachtask.TaskID = eachtask.PaperID;
                }
                else
                {
                    eachtask.IsFinished = true;
                    eachtask.TaskID = _paper.SelectTestIDByPaperID(eachtask.PaperID, UserID);                    
                }
                AssignedTask.Add(eachtask);
            }
            //IQueryable IAssignedTask = AssignedTask.AsQueryable();
            //PagedList<AssignedTask> Task = IAssignedTask.ToPagedList(id ?? 1, 10);
            return View(_paper.AssignedTaskPaged(id, AssignedTask));
        }
        #endregion

        #region 用户答卷详情查询
        public ActionResult AnswerInfo(Guid id)
        {
            int UserID = _paper.SelectUserID(User.Identity.Name);
            ErrorList erroe = _paper.SelectError(id, UserID);
            TempData["id"] = id;
            TempData["TestID"] = id;
            ViewData["Score"] = erroe.Score;
            double num = erroe.Time / 60;
            ViewData["Minutes"] = Math.Floor(num);
            ViewData["Second"] = erroe.Time % 60;
            ViewData["TotalScore"] = erroe.TotalScore;
            if (erroe.Proposal == null)
            {
                ViewData["Proposal"] = "暂未点评。";
            }
            else
            {
                ViewData["Proposal"] = erroe.Proposal;
            }
            return View();
        }
        #endregion

        public ActionResult ClassScore(Guid id)
        {
            Guid ClassID = _paper.SelectUserInfo(User.Identity.Name).ClassID.Value;
            Guid SinglePaperID = _paper.SelectPaperID(id);            
            List<SingleScoreInfo> SingleScoreInfoList = GetSingleScoreList(ClassID, SinglePaperID);
            SingleStatistics SingleStatisticsInfo = new SingleStatistics();
            string ClassName = _paper.GetClassbyID(ClassID).ClassName;
            CEDTS_Paper paper = _paper.SelectPaper(SinglePaperID);
            ViewData["TaskName"] = paper.Title;
            ViewData["ClassName"] = ClassName;
            ViewData["PaperID"] = SinglePaperID;
            if (SingleScoreInfoList.Count == 0)
            {
                ViewData["Empty"] = "Empty";
            }
            else
            {
                ViewData["Empty"] = "";
                bool doneflag = false;
                foreach (var singlescore in SingleScoreInfoList)
                {
                    if (singlescore.Done)
                    {
                        doneflag = true;
                        break;
                    }
                }
                if (doneflag)
                {
                    int donum = 0;
                    int undonum = SingleScoreInfoList.Count;
                    double totalstudscore = 0.0;
                    double highscore = double.MinValue;
                    double lowscore = double.MaxValue;
                    string highname = "";
                    string lowname = "";
                    foreach (var singlescore in SingleScoreInfoList)
                    {
                        if (singlescore.Done)
                        {
                            donum++;
                            undonum--;
                            totalstudscore += singlescore.Score;
                            if (singlescore.Score <= lowscore)
                            {
                                if (singlescore.Score == lowscore)
                                {
                                    lowname = lowname + "、" + singlescore.StudName;
                                }
                                else
                                {
                                    lowscore = singlescore.Score;
                                    lowname = singlescore.StudName;
                                }
                            }
                            if (singlescore.Score >= highscore)
                            {
                                if (singlescore.Score == highscore)
                                {
                                    highname = highname + "、" + singlescore.StudName;
                                }
                                else
                                {
                                    highscore = singlescore.Score;
                                    highname = singlescore.StudName;
                                }
                            }
                        }
                    }
                    SingleStatisticsInfo.AveScore = totalstudscore / donum;
                    SingleStatisticsInfo.AvePercent = SingleStatisticsInfo.AveScore / paper.Score.Value * 100;
                    SingleStatisticsInfo.DoNum = donum;
                    SingleStatisticsInfo.UndoNum = undonum;
                    SingleStatisticsInfo.HighestScore = highscore;
                    SingleStatisticsInfo.HighestStudName = highname;
                    SingleStatisticsInfo.LowestScore = lowscore;
                    SingleStatisticsInfo.LowestStudName = lowname;
                    double temp = 0.0;
                    foreach (var singlescore in SingleScoreInfoList)
                    {
                        if (singlescore.Done)
                        {
                            temp += Math.Pow((singlescore.Score - SingleStatisticsInfo.AveScore), 2);
                        }
                    }
                    SingleStatisticsInfo.Variance = temp / donum;
                    SingleStatisticsInfo.StandardDeviation = Math.Sqrt(SingleStatisticsInfo.Variance);
                }
                else
                {
                    SingleStatisticsInfo.DoNum = 0;
                    SingleStatisticsInfo.UndoNum = SingleScoreInfoList.Count;
                }
            }
            return View(SingleStatisticsInfo);
        }

        public List<SingleScoreInfo> GetSingleScoreList(Guid ClassIDNum, Guid PaperIDNum)
        {
            List<SingleScoreInfo> SingleScoreInfoList = new List<SingleScoreInfo>();
            List<CEDTS_User> userlist = _paper.SelectUserByClassID(ClassIDNum);
            CEDTS_Paper paper = _paper.SelectPaper(PaperIDNum);
            double totalscore = paper.Score.Value;
            foreach (var user in userlist)
            {
                SingleScoreInfo eachsinglescore = new SingleScoreInfo();
                eachsinglescore.UserID = user.UserID;
                eachsinglescore.StudNum = user.StudentNumber;
                eachsinglescore.StudName = user.UserNickname;
                eachsinglescore.TotalScore = totalscore;
                CEDTS_Test test = _paper.GetTestByPaperID(PaperIDNum, user.UserID);
                if (test == null || test.IsFinished == false)
                {
                    eachsinglescore.Done = false;
                    eachsinglescore.Score = 0;
                    eachsinglescore.Percent = 0;
                }
                else
                {
                    eachsinglescore.Done = true;
                    eachsinglescore.Score = test.TotalScore.Value;
                    eachsinglescore.Percent = test.TotalScore.Value / totalscore * 100;
                }
                SingleScoreInfoList.Add(eachsinglescore);
            }
            return SingleScoreInfoList;
        }

        public ActionResult SingleScoreList(int? id, Guid PaperID)
        {
            Guid ClassID = _paper.SelectUserInfo(User.Identity.Name).ClassID.Value;
            if (id == null)
            {
                if (TempData["id"] != null)
                {
                    id = int.Parse(TempData["id"].ToString());
                    TempData["id"] = id;
                }
                return View(_paper.SingleScoreInfoPaged(id, GetSingleScoreList(ClassID, PaperID)));
            }
            else
            {
                TempData["id"] = id;
                return RedirectToAction("SingleExerciseScore");
            }
        }

        #region 用户答卷答案查询
        public ActionResult Answer(Guid id)
        {
            ViewData["TestID"] = id;
            return View();
        }
        #endregion

        #region 试卷答案查询
        public ActionResult GetPaper(Guid id)
        {

            Guid PaperID = _paper.SelectPaperID(id);
            PaperTotal pt = new PaperTotal();
            List<SkimmingScanningPartCompletion> sspcList = new List<SkimmingScanningPartCompletion>();//临时存储快速阅读题型
            List<Listen> slpoList = new List<Listen>();//临时存储短对话听力题型
            List<Listen> llpoList = new List<Listen>();//临时存储长对话听力题型
            List<Listen> rlpoList = new List<Listen>();//临时存储听力理解题型
            List<Listen> lpcList = new List<Listen>();//临时存储复合型听力题型
            List<ReadingPartCompletion> rpcList = new List<ReadingPartCompletion>();//临时存储阅读理解选词填空题型
            List<ReadingPartOption> rpoList = new List<ReadingPartOption>();//临时存储阅读理解选择题型
            List<InfoMatchingCompletion> infoMatList = new List<InfoMatchingCompletion>();
            List<ClozePart> cpList = new List<ClozePart>();//临时存储完型填空题型

            List<CEDTS_PaperAssessment> paList = _paper.SelectPaperAssessmentItem(PaperID);
            CEDTS_Paper paper = _paper.SelectPaper(PaperID);

            List<CEDTS_TestAnswer> taList = _paper.GetTestAnswer(id);

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
                    listen.SoundFile = ai.SoundFile;
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
                    listen.SoundFile = ai.SoundFile;
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
                    listen.SoundFile = ai.SoundFile;
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
                    listen.SoundFile = ai.SoundFile;
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
                        string Value = _paper.AnswerValue(info.ItemID, question.Answer);
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            string Content = "(_" + question.Order + "_)";

                            rpc.Content = rpc.Content.Replace(Content, "(_<span style='color:Red'>" + Value + "</span>_)");
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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

                #region 阅读理解信息匹配赋值
                if (pa.ItemTypeID == 9)
                {
                    var tempListQuestion = new List<CEDTS_Question>();
                    InfoMatchingCompletion infoMat = new InfoMatchingCompletion();
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

                    infoMat.Content = ai.Original;
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
                        string Value = _paper.AnswerValue(info.ItemID, question.Answer);
                        if (!relQuestion.Contains(question.QuestionID))
                        {
                            string Content = "(_" + question.Order + "_)";

                            infoMat.Content = infoMat.Content.Replace(Content, "(_<span style='color:Red'>" + Value + "</span>_)");
                            continue;
                        }
                        info.QuestionID.Add(question.QuestionID);
                        info.UserAnswer.Add(taList.Where(p => p.QuestionID == question.QuestionID).Select(p => p.UserAnswer).FirstOrDefault());
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
                    infoMat.WordList.Add(es.ChoiceK);
                    infoMat.WordList.Add(es.ChoiceL);
                    infoMat.WordList.Add(es.ChoiceM);
                    infoMat.WordList.Add(es.ChoiceN);
                    infoMat.WordList.Add(es.ChoiceO);
                    info.QuestionCount = tempListQuestion.Count;
                    infoMat.Info = info;
                    infoMatList.Add(infoMat);
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
            pt.InfMatList = infoMatList;
            pt.CpList = cpList;
            return Json(pt);
        }
        #endregion

        #region 用户当前答卷图形化展示
        public ActionResult TestView()
        {
            ViewData["TestID"] = TempData["TestID"];
            ViewData["PaperName"] = _paper.SelectPaperName(Guid.Parse(ViewData["TestID"].ToString()));
            return PartialView();
        }
        #endregion

        #region 当前答卷题型统计
        public ActionResult Item(Guid id)
        {
            return Json(_paper.UserItemInfo(id, User.Identity.Name));
        }
        #endregion

        #region 当前答卷知识点统计
        public ActionResult Knowledge(Guid id)
        {
            return Json(_paper.UserKpMaster(id, User.Identity.Name));
        }
        #endregion

        #region 错题集合
        public ActionResult ErrorQuestion()
        {
            Guid id = Guid.Parse(TempData["TestID"].ToString());
            return PartialView(_paper.ErrorQuestion(id));
        }
        #endregion

        #region 快速体验试卷
        [Authorize(Roles = "普通用户,体验用户")]
        public ActionResult QuickExperience()
        {
            Guid id = Guid.Parse(ConfigurationManager.AppSettings["PaperID"]);
            PaperTotal pt = new PaperTotal();
            List<SkimmingScanningPartCompletion> sspcList = new List<SkimmingScanningPartCompletion>();//临时存储快速阅读题型
            List<Listen> slpoList = new List<Listen>();//临时存储短对话听力题型
            List<Listen> llpoList = new List<Listen>();//临时存储长对话听力题型
            List<Listen> rlpoList = new List<Listen>();//临时存储听力理解题型
            List<Listen> lpcList = new List<Listen>();//临时存储复合型听力题型
            List<ReadingPartCompletion> rpcList = new List<ReadingPartCompletion>();//临时存储阅读理解选词填空题型
            List<ReadingPartOption> rpoList = new List<ReadingPartOption>();//临时存储阅读理解选择题型
            List<ClozePart> cpList = new List<ClozePart>();//临时存储完型填空题型

            List<CEDTS_PaperAssessment> paList = _paper.SelectPaperAssessmentItem(id);
            CEDTS_Paper paper = _paper.SelectPaper(id);

            foreach (var pa in paList)
            {
                Guid ItemID = Guid.Parse(pa.AssessmentItemID.ToString());//试题ID

                #region 快速阅读理解赋值
                if (pa.ItemTypeID == 1)
                {
                    SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
                    sspc.TermNum = TermNum;
                    sspc.ChoiceNum = info.QuestionCount - TermNum;
                    sspc.Info = info;
                    sspcList.Add(sspc);
                }
                #endregion

                #region 短对话听力赋值
                if (pa.ItemTypeID == 2)
                {
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                        info.QuestionID.Add(question.QuestionID);
                        //info.timeInterval.Add(question.Interval.Value);
                        //info.questionSound.Add(question.Sound);
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
                    }
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    slpoList.Add(listen);
                }
                #endregion

                #region 长对话听力赋值
                if (pa.ItemTypeID == 3)
                {
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    llpoList.Add(listen);
                }
                #endregion

                #region 短文理解听力赋值
                if (pa.ItemTypeID == 4)
                {
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    rlpoList.Add(listen);
                }
                #endregion

                #region 复合型听力赋值
                if (pa.ItemTypeID == 5)
                {
                    Listen listen = new Listen();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                        info.QuestionID.Add(question.QuestionID);
                        //info.timeInterval.Add(question.Interval.Value);
                        //info.questionSound.Add(question.Sound);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                    }
                    listen.SoundFile = ai.SoundFile;
                    listen.Info = info;
                    lpcList.Add(listen);
                }
                #endregion

                #region 阅读理解选词填空赋值
                if (pa.ItemTypeID == 6)
                {
                    ReadingPartCompletion rpc = new ReadingPartCompletion();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
                    CEDTS_Expansion es = _paper.SelectExpansion(ItemID);
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
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
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
                    rpc.Info = info;
                    rpcList.Add(rpc);
                }

                #endregion

                #region 阅读理解选择题型赋值
                if (pa.ItemTypeID == 7)
                {
                    ReadingPartOption rpo = new ReadingPartOption();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
                    rpo.Info = info;
                    rpoList.Add(rpo);
                }
                #endregion

                #region 完型填空赋值
                if (pa.ItemTypeID == 8)
                {
                    ClozePart cp = new ClozePart();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
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
                    }
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
            pp = pt;
            return RedirectToAction("PaperShow");
        }
        #endregion

        #region 知识点详情列表
        public ActionResult KpList(Guid id)
        {
            return Json(_paper.KpList(id, User.Identity.Name));
        }
        #endregion

        #region 题型列表
        public ActionResult ItemList(Guid id)
        {
            return Json(_paper.ItemList(id, User.Identity.Name));
        }
        #endregion

        #region 同一试卷成绩分布图
        public ActionResult SamePaper(Guid id)
        {
            int UserID = _paper.SelectUserID(User.Identity.Name);
            return Json(_paper.SelectSamePaperList(id, UserID));
        }
        #endregion

        #region 查询试卷练习次数是否超过2次
        public ActionResult Validation(Guid id)
        {
            int UserID = _paper.SelectUserID(User.Identity.Name);
            return Json(_paper.SelectCount(id, UserID));
        }
        #endregion

        #region 得分率分析
        public ActionResult RateAnalysis()
        {
            if (_paper.SelectRole(User.Identity.Name) == "体验用户")
            {
                return Json("所用体验用户体验的是同一份试卷，更多试卷请先登录后使用（点击右上角登陆！）");
            }
            else
            {
                int id = _paper.SelectUserID(User.Identity.Name);
                return Json(_paper.RateAnalysis(id));
            }
        }
        #endregion

        #region 知识点分析
        public ActionResult KpAnalysis()
        {
            if (_paper.SelectRole(User.Identity.Name) == "体验用户")
            {
                return Json("图为所有体验用户的知识点掌握率，想试试个性化的诊断吗？请登录后使用（点击右上角登陆！）");
            }
            else
            {
                int id = _paper.SelectUserID(User.Identity.Name);
                return Json(_paper.KpAnalysis(id));
            }
        }
        #endregion

        #region 题型分析
        public ActionResult QuestionAnalysis()
        {
            if (_paper.SelectRole(User.Identity.Name) == "体验用户")
            {
                return Json("只针对登录用户开发，，请返回首页登录后使用（点击首页两个字返回首页）”");
            }
            else
            {
                return Json("");
            }
        }
        #endregion

        #region 学习建议
        public ActionResult Suggestions()
        {
            Guid id = Guid.Parse(TempData["id"].ToString());
            int UserID = _paper.SelectUserID(User.Identity.Name);
            ErrorList erroe = _paper.SelectError(id, UserID);
            string face = string.Empty;
            double ScoreRate = erroe.Score / erroe.TotalScore;
            if (ScoreRate >= 0.9)
            {
                face = "优秀<img src='../../Images/1.1.gif'></img>";
            }
            if (ScoreRate < 0.9 && ScoreRate >= 0.8)
            {
                face = "良好<img src='../../Images/1.2.gif'></img>";
            }
            if (ScoreRate < 0.8 && ScoreRate >= 0.7)
            {
                face = "一般<img src='../../Images/1.3.gif'></img>";
            }
            if (ScoreRate < 0.7 && ScoreRate >= 0.6)
            {
                face = "及格<img src='../../Images/1.4.gif'></img>";
            }
            if (ScoreRate < 0.6 && ScoreRate >= 0.5)
            {
                face = "不及格<img src='../../Images/1.5.gif'></img>";
            }
            if (ScoreRate < 0.5)
            {
                face = "很差<img src='../../Images/1.6.gif'></img>";
            }
            string Content = "<b>试卷总分：</b>" + erroe.TotalScore + "分, <b>实际得分：</b>" + erroe.Score + "分," + face + "。";
            return Json(Content);
        }
        #endregion

        #region 单份试卷成绩分析
        public ActionResult SingleRateAnalysis(Guid id)
        {
            if (_paper.SelectRole(User.Identity.Name) == "体验用户")
            {
                return Json("所用体验用户体验的是同一份试卷，更多试卷登录后使用（点击右上角登陆两个字）");
            }
            else
            {
                int UserID = _paper.SelectUserID(User.Identity.Name);
                return Json(_paper.SingleRateAnalysis(id, UserID));
            }
        }
        #endregion

        #region 单份试卷知识点分析
        public ActionResult SingleKpAnalysis(Guid id)
        {
            if (_paper.SelectRole(User.Identity.Name) == "体验用户")
            {
                return Json("图为所有体验用户的知识点掌握率，想试试个性化的诊断吗？请登录后使用（点击右上角两个字）");
            }
            else
            {
                int UserID = _paper.SelectUserID(User.Identity.Name);
                return Json(_paper.SingleKpAnalysis(id, UserID));
            }
        }
        #endregion

        #region 查看是否是注册用户

        public ActionResult CheckUser()
        {
            if (_paper.SelectRole(User.Identity.Name) == "体验用户")
            {
                return Json("1");
            }
            else
            {
                return Json("");
            }
        }
        #endregion

        #region 查询当前用户是否练过试卷
        public ActionResult SelectPaper(string UserName)
        {
            int id = _paper.SelectUserID(UserName);
            return Json(_paper.SelectPaper(id));
        }

        public ActionResult selectFirstpaper()
        {
            int id = _paper.SelectUserID(User.Identity.Name);
            return Json(_paper.SelectPaper(id));
        }
        #endregion


    }
}
