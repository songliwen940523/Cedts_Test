using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Models;
using System.Transactions;
using System.Data;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Configuration;

namespace Cedts_Test.Controllers
{
    public class InsertDataController : Controller
    {
        ICedts_PaperRepository _paper;
        IInsertDataRepository _insert;
        public InsertDataController(ICedts_PaperRepository r, IInsertDataRepository i)
        {
            _paper = r;
            _insert = i;
        }


        public ActionResult Index(int? id)
        {
            return View(_paper.SelectPapers(id));
        }

        [HttpPost]
        public ActionResult Exerciser(FormCollection form)
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

            Random r = new Random();//定义随机数

            Guid id = Guid.Parse(form["hidden"]);//试卷ID
            int userID = 28;//用户ID
            int totalCount = int.Parse(ConfigurationManager.AppSettings["totalCount"]);//样本总量
            int Ti = 68;//知识点平均时间

            #region 正态分布区间概率
            //xi试卷正确率，ti小题平均时间
            double c1 = 0.03;//xi在区间[0.95，1]，ti在区间（0.7Ti,1.36Ti）
            double c2 = 0.08;//xi在区间[0.85，0.95]，ti在区间（0.76Ti,1.34Ti）
            double c3 = 0.10;//xi在区间[075，0.85]，ti在区间（0.8Ti,1.3Ti）
            double c4 = 0.12;//xi在区间[0.65，0.75]，ti在区间（0.82Ti,1.26Ti）
            double c5 = 0.13;//xi在区间[0.55,0.65]，ti在区间（0.84Ti,1.25Ti）
            double c6 = 0.12;//xi在区间[0.45,0.55]，ti在区间（0.85Ti,1.4Ti）
            double c7 = 0.10;//xi在区间[0.35,0.45]，ti在区间（0.86Ti,1.9Ti）
            double c8 = 0.08;//xi在区间[0.25,0.35]，ti在区间（0.87Ti,2.3Ti）
            double c9 = 0.06;//xi在区间[0.15,0.25]，ti在区间（0.88Ti,2.6Ti）
            double c10 = 0.04;//xi在区间[0.05,0.15]，ti在区间（0.89Ti,2.8Ti）
            double c11 = 0.01;//xi在区间[0,0.05]，ti在区间（0.9Ti,3Ti）
            #endregion

            List<int> answerCountList = new List<int>();//各区间样本数量

            #region 各区间样本数量
            int t1 = (int)(c1 * totalCount);
            answerCountList.Add(t1);
            int t2 = (int)(c2 * totalCount);
            answerCountList.Add(t2);
            int t3 = (int)(c3 * totalCount);
            answerCountList.Add(t3);
            int t4 = (int)(c4 * totalCount);
            answerCountList.Add(t4);
            int t5 = (int)(c5 * totalCount);
            answerCountList.Add(t5);
            int t6 = (int)(c6 * totalCount);
            answerCountList.Add(t6);
            int t7 = (int)(c7 * totalCount);
            answerCountList.Add(t7);
            int t8 = (int)(c8 * totalCount);
            answerCountList.Add(t8);
            int t9 = (int)(c9 * totalCount);
            answerCountList.Add(t9);
            int t10 = (int)(c10 * totalCount);
            answerCountList.Add(t10);
            int t11 = (int)(c11 * totalCount);
            answerCountList.Add(t11);
            int t12 = totalCount - t1 - t2 - t3 - t4 - t5 - t6 - t7 - t8 - t9 - t10 - t11;
            answerCountList.Add(t12);
            #endregion

            #region 获取试卷信息

            List<CEDTS_PaperAssessment> paList = _paper.SelectPaperAssessmentItem(id);
            CEDTS_Paper paper = _paper.SelectPaper(id);

            List<string> answerList1 = new List<string>();
            List<string> answerList2 = new List<string>();
            List<string> answerList3 = new List<string>();

            int tempIndex = 0;
            List<int> ChangeIndexList = new List<int>();

            foreach (var pa in paList)
            {
                Guid ItemID = Guid.Parse(pa.AssessmentItemID.ToString());//试题ID

                #region 快速阅读理解赋值
                if (pa.ItemTypeID == 1)
                {
                    SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
                    CEDTS_AssessmentItem ai = _paper.SelectAssessmentItem(ItemID);
                    CEDTS_ItemType it = _paper.SelectItemType((int)ai.ItemTypeID);
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
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
                    foreach (var question in questionList)
                    {
                        info.QuestionID.Add(question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                        if (question.ChooseA != null && question.ChooseA != "")
                        {
                            tempIndex++;
                            answerList1.Add(question.Answer.ToLower());
                            sspc.Choices.Add(question.ChooseA);
                            sspc.Choices.Add(question.ChooseB);
                            sspc.Choices.Add(question.ChooseC);
                            sspc.Choices.Add(question.ChooseD);
                        }
                        else
                        {
                            ChangeIndexList.Add(tempIndex);
                            tempIndex++;
                            answerList2.Add(question.Answer.ToLower());
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
                        tempIndex++;
                        answerList1.Add(question.Answer.ToLower());
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
                    }
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
                        tempIndex++;
                        answerList1.Add(question.Answer.ToLower());
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
                        tempIndex++;
                        answerList1.Add(question.Answer.ToLower());
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
                        ChangeIndexList.Add(tempIndex);
                        tempIndex++;
                        answerList2.Add(question.Answer);
                        info.QuestionID.Add((Guid)question.QuestionID);
                        info.ScoreQuestion.Add((double)question.Score);
                        info.TimeQuestion.Add((int)question.Duration);
                        info.DifficultQuestion.Add((double)question.Difficult);
                        info.Problem.Add(question.QuestionContent);
                        info.AnswerValue.Add(question.Answer);
                        info.Tip.Add(question.Analyze);
                    }
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
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
                    foreach (var question in questionList)
                    {
                        ChangeIndexList.Add(tempIndex);
                        tempIndex++;
                        answerList3.Add(question.Answer.ToLower());
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
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
                    foreach (var question in questionList)
                    {
                        tempIndex++;
                        answerList1.Add(question.Answer.ToLower());
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
                    List<CEDTS_Question> questionList = _paper.SelectQuestion(ItemID);
                    foreach (var question in questionList)
                    {
                        tempIndex++;
                        answerList1.Add(question.Answer.ToLower());
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

            #endregion

            #region 创建DataTable

            System.Data.DataTable dt = new System.Data.DataTable();//为excel创建表格
            dt.Columns.Add("KnowledgeID", System.Type.GetType("System.String"));
            dt.Columns.Add("KnowledgeName", System.Type.GetType("System.String"));
            dt.Columns.Add("IAccuracy", System.Type.GetType("System.String"));
            dt.Columns.Add("NAccuracy", System.Type.GetType("System.String"));
            dt.Columns.Add("AAccuracy", System.Type.GetType("System.String"));
            dt.Columns.Add("ITime", System.Type.GetType("System.String"));
            dt.Columns.Add("NTime", System.Type.GetType("System.String"));
            dt.Columns.Add("ATime", System.Type.GetType("System.String"));
            dt.Columns.Add("Mrate", System.Type.GetType("System.String"));


            System.Data.DataTable dt1 = new System.Data.DataTable();//为excel创建表格
            dt1.Columns.Add("1.1Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("1.2Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("1.3Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("1.4Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("1.5Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("1.6Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("2.1Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("2.2Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("2.3Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("2.4Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("2.5Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("2.6Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("2.7Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("2.8Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("3.1Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("3.2Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("3.3Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("3.4Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("3.5Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("3.6Ti", System.Type.GetType("System.String"));
            dt1.Columns.Add("3.7Ti", System.Type.GetType("System.String"));


            System.Data.DataTable dt4 = new System.Data.DataTable();//为excel创建表格
            dt4.Columns.Add("1.1Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("1.2Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("1.3Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("1.4Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("1.5Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("1.6Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("2.1Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("2.2Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("2.3Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("2.4Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("2.5Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("2.6Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("2.7Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("2.8Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("3.1Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("3.2Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("3.3Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("3.4Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("3.5Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("3.6Mi", System.Type.GetType("System.String"));
            dt4.Columns.Add("3.7Mi", System.Type.GetType("System.String"));

            System.Data.DataTable dt3 = new System.Data.DataTable();//为excel创建表格
            dt3.Columns.Add("1.1Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("1.2Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("1.3Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("1.4Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("1.5Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("1.6Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("2.1Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("2.2Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("2.3Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("2.4Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("2.5Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("2.6Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("2.7Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("2.8Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("3.1Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("3.2Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("3.3Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("3.4Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("3.5Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("3.6Xi", System.Type.GetType("System.String"));
            dt3.Columns.Add("3.7Xi", System.Type.GetType("System.String"));

            #endregion

            //循环生成答案
            for (int num = 1; num <= 12; num++)
            {
                double Max_xi = 0.0, Min_xi = 0.0;
                double Max_ti = 0.0, Min_ti = 0.0;

                switch (num)
                {
                    case 1: Max_xi = 1; Min_xi = 0.95; Max_ti = 1.36 * Ti; Min_ti = 0.7 * Ti; break;
                    case 2: Max_xi = 0.95; Min_xi = 0.85; Max_ti = 1.34 * Ti; Min_ti = 0.76 * Ti; break;
                    case 3: Max_xi = 0.85; Min_xi = 0.75; Max_ti = 1.3 * Ti; Min_ti = 0.8 * Ti; break;
                    case 4: Max_xi = 0.75; Min_xi = 0.65; Max_ti = 1.26 * Ti; Min_ti = 0.82 * Ti; break;
                    case 5: Max_xi = 0.65; Min_xi = 0.55; Max_ti = 1.25 * Ti; Min_ti = 0.84 * Ti; break;
                    case 6: Max_xi = 0.55; Min_xi = 0.45; Max_ti = 1.4 * Ti; Min_ti = 0.85 * Ti; break;
                    case 7: Max_xi = 0.45; Min_xi = 0.35; Max_ti = 1.9 * Ti; Min_ti = 0.86 * Ti; break;
                    case 8: Max_xi = 0.35; Min_xi = 0.25; Max_ti = 2.3 * Ti; Min_ti = 0.87 * Ti; break;
                    case 9: Max_xi = 0.25; Min_xi = 0.15; Max_ti = 2.6 * Ti; Min_ti = 0.88 * Ti; break;
                    case 10: Max_xi = 0.15; Min_xi = 0.05; Max_ti = 2.8 * Ti; Min_ti = 0.89 * Ti; break;
                    case 11: Max_xi = 0.05; Min_xi = 0; Max_ti = 3 * Ti; Min_ti = 0.9 * Ti; break;
                    default: Max_xi = 1; Min_xi = 0; Max_ti = 19 * Ti; Min_ti = 0.05 * Ti; break;
                }

                #region 正确率区间最大值和最小值
                int Xi_max = (int)(100 * Max_xi);
                int Xi_min = 0;
                if (Min_xi != 0)
                {
                    Xi_min = (int)(100 * Min_xi);
                }
                #endregion

                #region 小题平均时间最大值和最小值
                int Ti_max = (int)Max_ti;
                int Ti_min = (int)Min_ti;
                #endregion

                #region num=12时，完全随机
                if (num == 12)
                {
                    Xi_min = 0;
                    Xi_max = 100;
                    Ti_min = (int)Min_ti;
                    Ti_max = (int)Max_ti;
                }
                #endregion

                int end = 0;
                while (true)
                {
                    end++;
                    if (end > answerCountList[num - 1])
                    {
                        //当生成区间数量足够就跳出循环
                        break;
                    }

                    #region 知识点--将所有KPID添加到集合中

                    List<string> KpList = new List<string>();
                    if (pt.SspcList.Count > 0)
                    {
                        for (int i = 0; i < pt.SspcList.Count; i++)
                        {
                            for (int j = 0; j < pt.SspcList[i].Info.QuestionID.Count; j++)
                            {
                                KpList.Add(_insert.KpID(pt.SspcList[i].Info.QuestionID[j]));
                            }
                        }
                    }

                    if (pt.SlpoList.Count > 0)
                    {
                        for (int i = 0; i < pt.SlpoList.Count; i++)
                        {
                            for (int j = 0; j < pt.SlpoList[i].Info.QuestionID.Count; j++)
                            {
                                KpList.Add(_insert.KpID(pt.SlpoList[i].Info.QuestionID[j]));
                            }
                        }
                    }

                    if (pt.LlpoList.Count > 0)
                    {
                        for (int i = 0; i < pt.LlpoList.Count; i++)
                        {
                            for (int j = 0; j < pt.LlpoList[i].Info.QuestionID.Count; j++)
                            {
                                KpList.Add(_insert.KpID(pt.LlpoList[i].Info.QuestionID[j]));
                            }
                        }
                    }

                    if (pt.RlpoList.Count > 0)
                    {
                        for (int i = 0; i < pt.RlpoList.Count; i++)
                        {
                            for (int j = 0; j < pt.RlpoList[i].Info.QuestionID.Count; j++)
                            {
                                KpList.Add(_insert.KpID(pt.RlpoList[i].Info.QuestionID[j]));
                            }
                        }
                    }


                    if (pt.LpcList.Count > 0)
                    {
                        for (int i = 0; i < pt.LpcList.Count; i++)
                        {
                            for (int j = 0; j < pt.LpcList[i].Info.QuestionID.Count; j++)
                            {
                                KpList.Add(_insert.KpID(pt.LpcList[i].Info.QuestionID[j]));
                            }
                        }
                    }


                    if (pt.RpcList.Count > 0)
                    {
                        for (int i = 0; i < pt.RpcList.Count; i++)
                        {
                            for (int j = 0; j < pt.RpcList[i].Info.QuestionID.Count; j++)
                            {
                                KpList.Add(_insert.KpID(pt.RpcList[i].Info.QuestionID[j]));
                            }
                        }
                    }


                    if (pt.CpList.Count > 0)
                    {
                        for (int i = 0; i < pt.CpList.Count; i++)
                        {
                            for (int j = 0; j < pt.CpList[i].Info.QuestionID.Count; j++)
                            {
                                KpList.Add(_insert.KpID(pt.CpList[i].Info.QuestionID[j]));
                            }
                        }
                    }

                    if (pt.RpoList.Count > 0)
                    {
                        for (int i = 0; i < pt.RpoList.Count; i++)
                        {
                            for (int j = 0; j < pt.RpoList[i].Info.QuestionID.Count; j++)
                            {
                                KpList.Add(_insert.KpID(pt.RpoList[i].Info.QuestionID[j]));
                            }
                        }
                    }
                    #endregion

                    int tempnum = r.Next(Xi_min, Xi_max + 1);//试卷的正确率

                    int Count = answerList1.Count + answerList2.Count + answerList3.Count;
                    int Right = 0;//正确的题目数量
                    if (tempnum != 0)
                    {
                        Right = (int)(Count * tempnum * 0.01);
                    }
                    int Wrong = Count - Right;//错误的题目数量

                    List<int> indexList = new List<int>();//记录正确或错误题的序号


                    #region indexList记录错误题序号

                    if (Right > Wrong)
                    {
                        while (true)
                        {
                            if (indexList.Count == Wrong)
                            {
                                break;
                            }
                            Random rand = new Random();
                            int x = rand.Next(Count);
                            if (indexList.Contains(x))
                            {
                                continue;
                            }
                            indexList.Add(x);
                        }

                        indexList.Sort();
                        string answers = string.Empty;

                        for (int j = 0; j < Count; j++)
                        {
                            if (indexList.Contains(j))
                            {
                                if (j < answerList1.Count)
                                {
                                    while (true)
                                    {
                                        Random rand = new Random();
                                        string[] tempanswer = { "A", "B", "C", "D" };
                                        int index = rand.Next(4);
                                        if (tempanswer[index].ToLower() != answerList1[j])
                                        {
                                            answers += tempanswer[index].ToLower() + "|";
                                            break;
                                        }
                                    }
                                }
                                if (j >= answerList1.Count && j < answerList1.Count + answerList2.Count)
                                {
                                    answers += "wrong" + "|";
                                }
                                if (j >= answerList1.Count + answerList2.Count && j < answerList1.Count + answerList2.Count + answerList3.Count)
                                {
                                    while (true)
                                    {
                                        Random rnd = new Random();
                                        // A-O  ASCII值  65-80
                                        int i = rnd.Next(65, 80);
                                        char c = (char)i;
                                        if (c.ToString().ToLower() != answerList3[j - answerList1.Count - answerList2.Count].ToLower())
                                        {
                                            answers += c.ToString().ToLower() + "|";
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (j < answerList1.Count)
                                {
                                    answers += answerList1[j].ToLower() + "|";
                                }
                                if (j >= answerList1.Count && j < answerList1.Count + answerList2.Count)
                                {
                                    answers += answerList2[j - answerList1.Count].ToLower() + "|";
                                }
                                if (j >= answerList1.Count + answerList2.Count && j < answerList1.Count + answerList2.Count + answerList3.Count)
                                {
                                    answers += answerList3[j - answerList2.Count - answerList1.Count].ToLower() + "|";
                                }
                            }
                        }
                        if (answers != string.Empty)
                        {
                            answers = answers.Substring(0, answers.LastIndexOf('|'));
                            List<string> UserAs = answers.Split('|').ToList();
                            List<string> s1 = new List<string>();
                            List<string> s2 = new List<string>();
                            List<string> s3 = new List<string>();
                            for (int i = 0; i < UserAs.Count; i++)
                            {
                                if (i < answerList1.Count)
                                {
                                    s1.Add(UserAs[i]);
                                }
                                if (i >= answerList1.Count && i < answerList1.Count + answerList2.Count)
                                {
                                    s2.Add(UserAs[i]);
                                }
                                if (i >= answerList1.Count + answerList2.Count)
                                {
                                    s3.Add(UserAs[i]);
                                }
                            }
                            for (int i = 0; i < ChangeIndexList.Count; i++)
                            {
                                if (i < s2.Count)
                                {
                                    s1.Insert(ChangeIndexList[i], s2[i]);
                                }
                                if (i >= s2.Count && i < s2.Count + s3.Count)
                                {
                                    s1.Insert(ChangeIndexList[i], s3[i - s2.Count]);
                                }
                            }
                            UserAs.Clear();
                            UserAs = s1;

                            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(10, 5, 0)))
                            {
                                KnowledgePointInfo Kp = new KnowledgePointInfo();
                                Kp.AverageTime = new List<double>();
                                Kp.CorrectCount = new List<int>();
                                Kp.KnowledgePointID = new List<Guid>();
                                Kp.TotalCount = new List<int>();
                                int SspcTotalNum = 0;
                                int SlpoTotalNum = 0;
                                int LlpoTotalNum = 0;
                                int LpcTotalNum = 0;
                                int RlpoTotalNum = 0;
                                int RpcTotalNum = 0;
                                int RpoTotalNum = 0;
                                int CpTotalNum = 0;

                                //向数据库添加答卷信息
                                CEDTS_Test Test = new CEDTS_Test();
                                Test.TestID = Guid.NewGuid();
                                DateTime NowTime = DateTime.Now;
                                Test.PaperID = pt.PaperID;
                                Test.StartDate = NowTime;
                                Test.UserID = userID;
                                Test.IsFinished = true;
                                Test.IsChecked = 0;
                                Test.FinishDate = NowTime.AddMinutes(150);
                                Test.TotalTime = 150 * 60;

                                _paper.CreateTest(Test);

                                //向TestAnswer表插入数据
                                double TotalScore = 0;

                                #region 快速阅读题型统计
                                int Number = 1;
                                if (pt.SspcList.Count > 0)
                                {
                                    int ErrorNum = 0;
                                    for (int l = 0; l < pt.SspcList.Count; l++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        SspcTotalNum += pt.SspcList[l].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.SspcList[l].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.SspcList[l].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.SspcList[l].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.SspcList[l].Info.AnswerValue[j];

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[j];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (j >= 7)
                                            {
                                                Answer = Answer.Replace(" ", "");
                                                UserAnswer = UserAnswer.Replace(" ", "");
                                                if (Answer == UserAnswer)
                                                {
                                                    TestAnswer.IsRight = true;
                                                }
                                                else
                                                {
                                                    TestAnswer.IsRight = false;
                                                }
                                                TestAnswer.AnswerContent = TestAnswer.Answer;
                                                TestAnswer.UserAnswer = UserAs[j];
                                            }
                                            else
                                            {


                                                if (Answer == UserAnswer)
                                                {
                                                    TestAnswer.IsRight = true;
                                                }
                                                else
                                                {
                                                    TestAnswer.IsRight = false;
                                                }
                                                TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);
                                                TestAnswer.UserAnswer = UserAnswer;
                                            }

                                            TestAnswer.AssessmentItemID = pt.SspcList[l].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 1;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    //题型错误统计
                                    for (int k = 0; k < indexList.Count; k++)
                                    {
                                        if (indexList[k] < SspcTotalNum)
                                        {
                                            ErrorNum += 1;
                                        }
                                    }
                                    TotalScore = (SspcTotalNum - ErrorNum) * 1;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 1;
                                    TATI.CorrectItemNumber = SspcTotalNum - ErrorNum;
                                    TATI.TotalItemNumber = SspcTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 1;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 1;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 短对话听力题型统计
                                //短对话听力题型统计
                                if (pt.SlpoList.Count > 0)
                                {
                                    int ErrorNum = 0;
                                    for (int i = 0; i < pt.SlpoList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        SlpoTotalNum += pt.SlpoList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.SlpoList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.SlpoList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.SlpoList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.SlpoList[i].Info.AnswerValue[j];

                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.SlpoList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 2;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > SspcTotalNum && indexList[j] < (SspcTotalNum + SlpoTotalNum))
                                        {
                                            ErrorNum += 1;
                                        }
                                    }
                                    TotalScore += (SlpoTotalNum - ErrorNum) * 1;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 2;
                                    TATI.CorrectItemNumber = SlpoTotalNum - ErrorNum;
                                    TATI.TotalItemNumber = SlpoTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 2;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 2;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 长对话听力题型统计
                                //长对话听力题型统计
                                if (pt.LlpoList.Count > 0)
                                {
                                    int ErrorNum = 0;
                                    for (int i = 0; i < pt.LlpoList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        LlpoTotalNum += pt.LlpoList[i].Info.QuestionCount;
                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.LlpoList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.LlpoList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.LlpoList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.LlpoList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.LlpoList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 3;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }

                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum) && indexList[j] < (SspcTotalNum + LlpoTotalNum + SlpoTotalNum))
                                        {
                                            ErrorNum += 1;
                                        }
                                    }
                                    TotalScore += (LlpoTotalNum - ErrorNum) * 1;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 3;
                                    TATI.CorrectItemNumber = LlpoTotalNum - ErrorNum;
                                    TATI.TotalItemNumber = LlpoTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 3;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 3;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 短文听力理解题型统计
                                //短文听力理解题型统计
                                if (pt.RlpoList.Count > 0)
                                {
                                    int ErrorNum = 0;
                                    for (int i = 0; i < pt.RlpoList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        RlpoTotalNum += pt.RlpoList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.RlpoList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.RlpoList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.RlpoList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.RlpoList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = TestAnswer.Answer;
                                            TestAnswer.AssessmentItemID = pt.RlpoList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 5;

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.Number = Number;
                                            Number += 1;


                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LpcTotalNum + LlpoTotalNum + RlpoTotalNum))
                                        {
                                            ErrorNum += 1;
                                        }
                                    }

                                    TotalScore += (RlpoTotalNum - ErrorNum) * 1;

                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 5;
                                    TATI.CorrectItemNumber = RlpoTotalNum - ErrorNum;
                                    TATI.TotalItemNumber = RlpoTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 5;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 5;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }

                                #endregion

                                #region 复合型听力题型统计

                                //复合型听力题型统计
                                if (pt.LpcList.Count > 0)
                                {
                                    int ErrorNum = 0;
                                    for (int i = 0; i < pt.LpcList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        LpcTotalNum += pt.LpcList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.LpcList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.LpcList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.LpcList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.LpcList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);

                                            string Answer = TestAnswer.Answer;
                                            Answer = Answer.Replace(" ", "");
                                            Answer = Answer.ToUpper();
                                            string UserAnswer = UserAs[(Number - 1)];
                                            UserAnswer = UserAnswer.Replace(" ", "");
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAs[(Number - 1)];
                                            TestAnswer.AssessmentItemID = pt.LpcList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 4;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + LlpoTotalNum + SlpoTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum))
                                        {
                                            ErrorNum += 1;
                                        }
                                    }
                                    TotalScore += (LpcTotalNum - ErrorNum) * 1;

                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 4;
                                    TATI.CorrectItemNumber = LpcTotalNum - ErrorNum;
                                    TATI.TotalItemNumber = LpcTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 4;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 4;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 阅读理解-选词填空题型统计

                                //阅读理解-选词填空题型统计
                                if (pt.RpcList.Count > 0)
                                {
                                    for (int i = 0; i < pt.RpcList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        RpcTotalNum += pt.RpcList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.RpcList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.RpcList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.RpcList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.RpcList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);
                                            TestAnswer.AssessmentItemID = pt.RpcList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 6;


                                            string Answer = TestAnswer.Answer;
                                            Answer = Answer.Replace(" ", "");
                                            Answer = Answer.ToUpper();
                                            string UserAnswer = UserAs[(Number - 1)];
                                            UserAnswer = UserAnswer.Replace(" ", "");
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    int ErrorNum = 0;
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + RlpoTotalNum + RpcTotalNum))
                                        {
                                            ErrorNum += 1;
                                        }
                                    }
                                    TotalScore += (RpcTotalNum - ErrorNum) * 1;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 6;
                                    TATI.CorrectItemNumber = RpcTotalNum - ErrorNum;
                                    TATI.TotalItemNumber = RpcTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 6;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 6;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 阅读理解-选择题型统计

                                //阅读理解-选择题型统计
                                if (pt.RpoList.Count > 0)
                                {
                                    int ErrorNum = 0;
                                    for (int i = 0; i < pt.RpoList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        RpoTotalNum += pt.RpoList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.RpoList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.RpoList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.RpoList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.RpoList[i].Info.AnswerValue[j];

                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.RpoList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 7;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum + RpcTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum + RpcTotalNum + RpoTotalNum))
                                        {
                                            ErrorNum += 1;
                                        }
                                    }
                                    TotalScore += (RpoTotalNum - ErrorNum) * 1.5;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 7;
                                    TATI.CorrectItemNumber = RpoTotalNum - ErrorNum;
                                    TATI.TotalItemNumber = RpoTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 7;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }

                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 7;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 完型填空题型统计

                                //完型填空题型统计
                                if (pt.CpList.Count > 0)
                                {
                                    int ErrorNum = 0;
                                    for (int i = 0; i < pt.CpList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        CpTotalNum += pt.CpList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.CpList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.CpList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.CpList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.CpList[i].Info.AnswerValue[j];

                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);


                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.CpList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 8;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum + RpcTotalNum + RpoTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum + RpcTotalNum + RpoTotalNum + CpTotalNum))
                                        {
                                            ErrorNum += 1;
                                        }
                                    }

                                    TotalScore += (CpTotalNum - ErrorNum) * 0.5;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 8;
                                    TATI.CorrectItemNumber = CpTotalNum - ErrorNum;
                                    TATI.TotalItemNumber = CpTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 8;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 8;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 知识点统计

                                //错误知识点统计
                                for (int m = 0; m < indexList.Count; m++)
                                {
                                    string kp1 = KpList[indexList[m]];
                                    string[] kp2 = kp1.Split('|');
                                    for (int n = 0; n < kp2.Length; n++)
                                    {
                                        for (int j = 0; j < kp2.Length; j++)
                                        {
                                            if (Kp.KnowledgePointID.Contains(Guid.Parse(kp2[j])))
                                            {
                                                for (int t = 0; t < Kp.KnowledgePointID.Count; t++)
                                                {
                                                    if (Kp.KnowledgePointID[t] == Guid.Parse(kp2[j]))
                                                    {

                                                        Kp.AverageTime[t] += _insert.selectQuestionKp(Guid.Parse(kp2[j]), Test.TestID);
                                                        Kp.TotalCount[t] += 1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Kp.KnowledgePointID.Add(Guid.Parse(kp2[j]));
                                                Kp.TotalCount.Add(1);
                                                Kp.CorrectCount.Add(0);
                                                Kp.AverageTime.Add(_insert.selectQuestionKp(Guid.Parse(kp2[j]), Test.TestID));
                                            }
                                        }
                                    }
                                }
                                //移除已统计的错误知识点
                                for (int i = indexList.Count - 1; i >= 0; i--)
                                {
                                    KpList.RemoveAt(indexList[i]);
                                }
                                //正确知识点统计
                                for (int h = 0; h < KpList.Count; h++)
                                {
                                    string kp1 = KpList[h];
                                    string[] kp2 = KpList[h].Split('|');
                                    for (int i = 0; i < kp2.Length; i++)
                                    {
                                        if (Kp.KnowledgePointID.Contains(Guid.Parse(kp2[i])))
                                        {
                                            for (int t = 0; t < Kp.KnowledgePointID.Count; t++)
                                            {
                                                if (Kp.KnowledgePointID[t] == Guid.Parse(kp2[i]))
                                                {
                                                    Kp.AverageTime[t] += _insert.selectQuestionKp(Guid.Parse(kp2[i]), Test.TestID);
                                                    Kp.TotalCount[t] += 1;
                                                    Kp.CorrectCount[t] += 1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Kp.AverageTime.Add(_insert.selectQuestionKp(Guid.Parse(kp2[i]), Test.TestID));
                                            Kp.KnowledgePointID.Add(Guid.Parse(kp2[i]));
                                            Kp.CorrectCount.Add(1);
                                            Kp.TotalCount.Add(1);
                                        }
                                    }
                                }



                                DataRow dr1 = dt1.NewRow();
                                DataRow dr3 = dt3.NewRow();
                                DataRow dr4 = dt4.NewRow();
                                for (int k = 0; k < Kp.KnowledgePointID.Count; k++)
                                {
                                    DataRow dr = dt.NewRow();


                                    CEDTS_SampleKnowledgeInfomation SKI = new CEDTS_SampleKnowledgeInfomation();
                                    CEDTS_TestAnswerKnowledgePoint TestAKP = new CEDTS_TestAnswerKnowledgePoint();
                                    CEDTS_UserKnowledgeInfomation UKI = new CEDTS_UserKnowledgeInfomation();

                                    SKI.KnowledgePointID = Kp.KnowledgePointID[k];//知识点ID

                                    TestAKP.TestID = Test.TestID; //用户当前测试ID
                                    TestAKP.PaperID = Test.PaperID;//当前试卷ID
                                    TestAKP.UserID = Test.UserID;//用户ID
                                    TestAKP.KnowledgePointID = Kp.KnowledgePointID[k];//知识点ID

                                    dr[0] = TestAKP.KnowledgePointID.ToString();
                                    dr[1] = _paper.SelectKnowledgeByID(TestAKP.KnowledgePointID).Title.ToString();

                                    TestAKP.CorrectItemNumber = Kp.CorrectCount[k];//知识点正确题数
                                    TestAKP.TotalItemNumber = Kp.TotalCount[k];//知识点总题数
                                    //知识点正确率=知识点正确题数/知识点总题数(当前测试知识点正确)
                                    TestAKP.CorrectRate = Convert.ToDouble(TestAKP.CorrectItemNumber) / Convert.ToDouble(TestAKP.TotalItemNumber);

                                    dr[3] = TestAKP.CorrectRate.ToString();

                                    TestAKP.AverageTime = (Kp.AverageTime[k] / Kp.TotalCount[k]);//当前知识点平均耗时（当前测试知识点平均时间）

                                    dr[6] = TestAKP.AverageTime.ToString();

                                    TestAKP.Time = DateTime.Now;




                                    UKI.UserID = Test.UserID;
                                    UKI.KnowledgePointID = Kp.KnowledgePointID[k];

                                    if (_paper.SelectSIK(Kp.KnowledgePointID[k]) == 2)
                                    {
                                        SKI.SKII_ID = _paper.SelectSKI(Kp.KnowledgePointID[k]).SKII_ID;
                                        //当前知识点在系统中练习次数
                                        SKI.TotalCount = _paper.SelectSKI(Kp.KnowledgePointID[k]).TotalCount + 1;
                                        //平均时间（知识点样本空间时间）
                                        SKI.AverageTime = (_paper.SelectSKI(Kp.KnowledgePointID[k]).AverageTime * _paper.SelectSKI(Kp.KnowledgePointID[k]).TotalCount + Kp.AverageTime[k] / Kp.TotalCount[k]) / SKI.TotalCount;
                                        //当前知识点在系统练习中正确率
                                        SKI.CorrectRate = (_paper.SelectSKI(Kp.KnowledgePointID[k]).CorrectRate * _paper.SelectSKI(Kp.KnowledgePointID[k]).TotalCount + TestAKP.CorrectRate) / SKI.TotalCount;
                                        //当前知识点掌握率（知识点掌握率）
                                        TestAKP.KP_MasterRate = (TestAKP.CorrectRate * _paper.SelectSKI(Kp.KnowledgePointID[k]).AverageTime * 3.2) / (TestAKP.AverageTime + 3.2 * _paper.SelectSKI(Kp.KnowledgePointID[k]).AverageTime);
                                        //当前知识点在系统练习中掌握率
                                        SKI.KP_MasterRate = (_paper.SelectSKI(Kp.KnowledgePointID[k]).KP_MasterRate * _paper.SelectSKI(Kp.KnowledgePointID[k]).TotalCount + TestAKP.KP_MasterRate) / SKI.TotalCount;
                                        dr[2] = _paper.SelectSKI(Kp.KnowledgePointID[k]).CorrectRate.ToString();
                                        dr[5] = _paper.SelectSKI(Kp.KnowledgePointID[k]).AverageTime.ToString();

                                        _paper.CreateTAKP(TestAKP);
                                        _paper.UpdataSKI(SKI);

                                        if (_paper.SelectUkINum(Test.UserID, Kp.KnowledgePointID[k]) == 2)
                                        {
                                            UKI.UKII_ID = _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).UKII_ID;
                                            //练习次数
                                            UKI.TotalCount = _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).TotalCount + 1;
                                            //平均时间（累计平均时间）
                                            UKI.AverageTime = (_paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).AverageTime * _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).TotalCount + Kp.AverageTime[k] / Kp.TotalCount[k]) / UKI.TotalCount;
                                            //正确率(个人样本空间知识点正确率 )(累计知识点正确率---查询)
                                            UKI.CorrectRate = (_paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).CorrectRate * _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).TotalCount + TestAKP.CorrectRate) / UKI.TotalCount;
                                            //掌握率
                                            UKI.KP_MasterRate = (_paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).KP_MasterRate * _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).TotalCount + TestAKP.KP_MasterRate) / UKI.TotalCount;
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

                                        dr[2] = 0;
                                        dr[5] = 0;

                                        TestAKP.KP_MasterRate = TestAKP.CorrectRate;//样本空间时间为0，当前第一次掌握率记为正确率
                                        SKI.KP_MasterRate = TestAKP.KP_MasterRate;
                                        _paper.CreateTAKP(TestAKP);
                                        _paper.CreateSKI(SKI);

                                        UKI.CorrectRate = TestAKP.CorrectRate;
                                        UKI.AverageTime = TestAKP.AverageTime;
                                        UKI.TotalCount = 1;
                                        UKI.KP_MasterRate = TestAKP.KP_MasterRate;
                                        _paper.CreateUKI(UKI);
                                    }
                                    #region C值算法伪造数据
                                    string KpID = TestAKP.KnowledgePointID.ToString();
                                    KpID = KpID.ToUpper();

                                    string tempAverageTime = TestAKP.AverageTime.ToString();
                                    string tempCorrectRate = TestAKP.CorrectRate.ToString();
                                    string tempMasterRate = TestAKP.KP_MasterRate.ToString();
                                    switch (KpID)
                                    {
                                        case "5EE3269D-0296-4C7D-B282-D5409FBE00DF":
                                            dr1[0] = tempAverageTime;
                                            dr3[0] = tempCorrectRate;
                                            dr4[0] = tempMasterRate;
                                            break;
                                        case "487B5E83-684E-4484-A965-4397D04468EE":
                                            dr1[1] = tempAverageTime;
                                            dr3[1] = tempCorrectRate;
                                            dr4[1] = tempMasterRate;
                                            break;
                                        case "876BFAD1-9EB3-47B9-B56F-C65A2E288019":
                                            dr1[2] = tempAverageTime;
                                            dr3[2] = tempCorrectRate;
                                            dr4[2] = tempMasterRate;
                                            break;
                                        case "86C01AA6-7C26-4EE9-9C71-FEFA18B7C835":
                                            dr1[3] = tempAverageTime;
                                            dr3[3] = tempCorrectRate;
                                            dr4[3] = tempMasterRate;
                                            break;
                                        case "84EB0A3E-0CB0-4DDD-81D8-1976162C4CA3":
                                            dr1[4] = tempAverageTime;
                                            dr3[4] = tempCorrectRate;
                                            dr4[4] = tempMasterRate;
                                            break;
                                        case "6CBCE252-FC2C-4B89-B8B6-E80F11068AA3":
                                            dr1[5] = tempAverageTime;
                                            dr3[5] = tempCorrectRate;
                                            dr4[5] = tempMasterRate;
                                            break;
                                        case "E6E791AD-174F-4A37-8806-5F3119DE7C79":
                                            dr1[6] = tempAverageTime;
                                            dr3[6] = tempCorrectRate;
                                            dr4[6] = tempMasterRate;
                                            break;
                                        case "E728EF68-20B3-4DE0-A8B7-FA5019DE94E0":
                                            dr1[7] = tempAverageTime;
                                            dr3[7] = tempCorrectRate;
                                            dr4[7] = tempMasterRate;
                                            break;
                                        case "09D3E61D-33DF-4F71-B84F-92AFB0A2A903":
                                            dr1[8] = tempAverageTime;
                                            dr3[8] = tempCorrectRate;
                                            dr4[8] = tempMasterRate;
                                            break;
                                        case "5240875C-9317-4857-99AC-516B14FE06E1":
                                            dr1[9] = tempAverageTime;
                                            dr3[9] = tempCorrectRate;
                                            dr4[9] = tempMasterRate;
                                            break;
                                        case "055E3537-66F0-460E-A94C-7C8FCD240F33":
                                            dr1[10] = tempAverageTime;
                                            dr3[10] = tempCorrectRate;
                                            dr4[10] = tempMasterRate;
                                            break;
                                        case "194A9727-0D8E-421C-9CF9-6D723520A9C2":
                                            dr1[11] = tempAverageTime;
                                            dr3[11] = tempCorrectRate;
                                            dr4[11] = tempMasterRate;
                                            break;
                                        case "274BB572-BC33-40DE-8705-C154FE1FD88E":
                                            dr1[12] = tempAverageTime;
                                            dr3[12] = tempCorrectRate;
                                            dr4[12] = tempMasterRate;
                                            break;
                                        case "D83DE643-1A42-4434-BDA1-7FC8B29B97CF":
                                            dr1[13] = tempAverageTime;
                                            dr3[13] = tempCorrectRate;
                                            dr4[13] = tempMasterRate;
                                            break;
                                        case "A763E476-EC3B-449E-881A-692A3B876A08":
                                            dr1[14] = tempAverageTime;
                                            dr3[14] = tempCorrectRate;
                                            dr4[14] = tempMasterRate;
                                            break;
                                        case "BC3B6C74-8BB4-43DD-83EF-A8C8C61B44F6":
                                            dr1[15] = tempAverageTime;
                                            dr3[15] = tempCorrectRate;
                                            dr4[15] = tempMasterRate;
                                            break;
                                        case "A02A23EC-D0EF-4408-9823-2A36FC3F7119":
                                            dr1[16] = tempAverageTime;
                                            dr3[16] = tempCorrectRate;
                                            dr4[16] = tempMasterRate;
                                            break;
                                        case "EF7E1646-389F-4AA3-8466-9FFE7AC81BE8":
                                            dr1[17] = tempAverageTime;
                                            dr3[17] = tempCorrectRate;
                                            dr4[17] = tempMasterRate;
                                            break;
                                        case "81DF5997-BE9C-4461-AD3D-A177FA54AA3A":
                                            dr1[18] = tempAverageTime;
                                            dr3[18] = tempCorrectRate;
                                            dr4[18] = tempMasterRate;
                                            break;
                                        case "A42EC9FE-5941-492A-8589-864522ADDC3A":
                                            dr1[19] = tempAverageTime;
                                            dr3[19] = tempCorrectRate;
                                            dr4[19] = tempMasterRate;
                                            break;
                                        case "96B46208-7B7A-45E1-B2FF-15EAA9595CE9":
                                            dr1[20] = tempAverageTime;
                                            dr3[20] = tempCorrectRate;
                                            dr4[20] = tempMasterRate;
                                            break;
                                        default:
                                            break;
                                    }

                                    #endregion


                                    dr[4] = SKI.CorrectRate;

                                    dr[7] = SKI.AverageTime.ToString();
                                    dr[8] = TestAKP.KP_MasterRate.ToString();
                                    dt.Rows.Add(dr);
                                }
                                #endregion
                                dt1.Rows.Add(dr1);
                                dt3.Rows.Add(dr3);
                                dt4.Rows.Add(dr4);

                                DataRow dr_ave = dt.NewRow();
                                dr_ave[0] = Test.TestID.ToString();
                                dr_ave[1] = ((double)Right / (double)Count).ToString();
                                dt.Rows.Add(dr_ave);
                                dt.Rows.Add(dt.NewRow());

                                Test.TotalScore = TotalScore;
                                _paper.UpdataTest(Test);
                                tran.Complete();
                            }
                        }
                    }
                    #endregion

                    #region indexList记录正确的序号

                    else
                    {
                        while (true)
                        {
                            if (indexList.Count == Right)
                            {
                                break;
                            }
                            Random rand = new Random();
                            int x = rand.Next(Count);
                            if (indexList.Contains(x))
                            {
                                continue;
                            }
                            indexList.Add(x);
                        }

                        indexList.Sort();
                        string answers = string.Empty;

                        for (int j = 0; j < Count; j++)
                        {
                            if (!indexList.Contains(j))
                            {

                                if (j < answerList1.Count)
                                {
                                    while (true)
                                    {
                                        Random rand = new Random();
                                        string[] tempanswer = { "A", "B", "C", "D" };
                                        int index = rand.Next(4);
                                        if (tempanswer[index].ToLower() != answerList1[j])
                                        {
                                            answers += tempanswer[index].ToLower() + "|";
                                            break;
                                        }
                                    }
                                }
                                if (j >= answerList1.Count && j < answerList1.Count + answerList2.Count)
                                {
                                    answers += "wrong" + "|";
                                }
                                if (j >= answerList1.Count + answerList2.Count && j < answerList1.Count + answerList2.Count + answerList3.Count)
                                {
                                    while (true)
                                    {
                                        Random rnd = new Random();
                                        // A-O  ASCII值  65-80
                                        int i = rnd.Next(65, 80);
                                        char c = (char)i;
                                        if (c.ToString().ToLower() != answerList3[j - answerList1.Count - answerList2.Count].ToLower())
                                        {
                                            answers += c.ToString().ToLower() + "|";
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (j < answerList1.Count)
                                {
                                    answers += answerList1[j].ToLower() + "|";
                                }
                                if (j >= answerList1.Count && j < answerList1.Count + answerList2.Count)
                                {
                                    answers += answerList2[j - answerList1.Count].ToLower() + "|";
                                }
                                if (j >= answerList1.Count + answerList2.Count && j < answerList1.Count + answerList2.Count + answerList3.Count)
                                {
                                    answers += answerList3[j - answerList2.Count - answerList1.Count].ToLower() + "|";
                                }
                            }
                        }
                        if (answers != string.Empty)
                        {
                            answers = answers.Substring(0, answers.LastIndexOf('|'));
                            List<string> UserAs = answers.Split('|').ToList();
                            List<string> s1 = new List<string>();
                            List<string> s2 = new List<string>();
                            List<string> s3 = new List<string>();
                            for (int i = 0; i < UserAs.Count; i++)
                            {
                                if (i < answerList1.Count)
                                {
                                    s1.Add(UserAs[i]);
                                }
                                if (i >= answerList1.Count && i < answerList1.Count + answerList2.Count)
                                {
                                    s2.Add(UserAs[i]);
                                }
                                if (i >= answerList1.Count + answerList2.Count)
                                {
                                    s3.Add(UserAs[i]);
                                }
                            }
                            for (int i = 0; i < ChangeIndexList.Count; i++)
                            {
                                if (i < s2.Count)
                                {
                                    s1.Insert(ChangeIndexList[i], s2[i]);
                                }
                                if (i >= s2.Count && i < s2.Count + s3.Count)
                                {
                                    s1.Insert(ChangeIndexList[i], s3[i - s2.Count]);
                                }
                            }
                            UserAs.Clear();
                            UserAs = s1;

                            using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(10, 5, 0)))
                            {
                                KnowledgePointInfo Kp = new KnowledgePointInfo();
                                Kp.AverageTime = new List<double>();
                                Kp.CorrectCount = new List<int>();
                                Kp.KnowledgePointID = new List<Guid>();
                                Kp.TotalCount = new List<int>();
                                int SspcTotalNum = 0;
                                int SlpoTotalNum = 0;
                                int LlpoTotalNum = 0;
                                int LpcTotalNum = 0;
                                int RlpoTotalNum = 0;
                                int RpcTotalNum = 0;
                                int RpoTotalNum = 0;
                                int CpTotalNum = 0;
                                int Number = 1;
                                #region 向数据库添加答卷信息

                                CEDTS_Test Test = new CEDTS_Test();
                                Test.TestID = Guid.NewGuid();
                                DateTime NowTime = DateTime.Now;
                                Test.PaperID = pt.PaperID;
                                Test.StartDate = NowTime;
                                Test.UserID = userID;
                                Test.IsFinished = true;
                                Test.IsChecked = 0;
                                Test.FinishDate = NowTime.AddMinutes(150);
                                Test.TotalTime = 150 * 60;
                                _paper.CreateTest(Test);
                                #endregion

                                #region 向TestAnswer表插入数据

                                double TotalScore = 0;

                                #region 快速阅读题型统计

                                if (pt.SspcList.Count > 0)
                                {
                                    int CorrectNum = 0;
                                    for (int l = 0; l < pt.SspcList.Count; l++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        SspcTotalNum += pt.SspcList[l].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.SspcList[l].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.SspcList[l].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.SspcList[l].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.SspcList[l].Info.AnswerValue[j];

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[j];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (j >= 7)
                                            {
                                                TestAnswer.AnswerContent = TestAnswer.Answer;
                                                Answer = Answer.Replace(" ", "");
                                                UserAnswer = UserAnswer.Replace(" ", "");
                                                if (Answer == UserAnswer)
                                                {
                                                    TestAnswer.IsRight = true;
                                                }
                                                else
                                                {
                                                    TestAnswer.IsRight = false;
                                                }
                                                TestAnswer.UserAnswer = UserAs[j];
                                            }
                                            else
                                            {
                                                TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);
                                                if (Answer == UserAnswer)
                                                {
                                                    TestAnswer.IsRight = true;
                                                }
                                                else
                                                {
                                                    TestAnswer.IsRight = false;
                                                }
                                                TestAnswer.Answer = UserAnswer;
                                            }
                                            TestAnswer.AssessmentItemID = pt.SspcList[l].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 1;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    //题型正确统计
                                    for (int k = 0; k < indexList.Count; k++)
                                    {
                                        if (indexList[k] < SspcTotalNum)
                                        {
                                            CorrectNum += 1;
                                        }
                                    }
                                    TotalScore = CorrectNum * 1;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 1;
                                    TATI.CorrectItemNumber = CorrectNum;
                                    TATI.TotalItemNumber = SspcTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 1;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 1;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 短对话听力题型统计
                                //短对话听力题型统计
                                if (pt.SlpoList.Count > 0)
                                {
                                    int CorrectNum = 0;
                                    for (int i = 0; i < pt.SlpoList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        SlpoTotalNum += pt.SlpoList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.SlpoList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.SlpoList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.SlpoList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.SlpoList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.SlpoList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 2;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > SspcTotalNum && indexList[j] < (SspcTotalNum + SlpoTotalNum))
                                        {
                                            CorrectNum += 1;
                                        }
                                    }
                                    TotalScore += CorrectNum * 1;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 2;
                                    TATI.CorrectItemNumber = CorrectNum;
                                    TATI.TotalItemNumber = SlpoTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 2;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 2;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 长对话听力题型统计
                                //长对话听力题型统计
                                if (pt.LlpoList.Count > 0)
                                {
                                    int CorrectNum = 0;
                                    for (int i = 0; i < pt.LlpoList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        LlpoTotalNum += pt.LlpoList[i].Info.QuestionCount;
                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.LlpoList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.LlpoList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.LlpoList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.LlpoList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);


                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.LlpoList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 3;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }

                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum) && indexList[j] < (SspcTotalNum + LlpoTotalNum + SlpoTotalNum))
                                        {
                                            CorrectNum += 1;
                                        }
                                    }
                                    TotalScore += CorrectNum * 1;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 3;
                                    TATI.CorrectItemNumber = CorrectNum;
                                    TATI.TotalItemNumber = LlpoTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 3;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 3;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 短文听力理解题型统计
                                //短文听力理解题型统计
                                if (pt.RlpoList.Count > 0)
                                {
                                    int CorrectNum = 0;
                                    for (int i = 0; i < pt.RlpoList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        RlpoTotalNum += pt.RlpoList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.RlpoList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.RlpoList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.RlpoList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.RlpoList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = TestAnswer.Answer;

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.RlpoList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 5;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LpcTotalNum + LlpoTotalNum + RlpoTotalNum))
                                        {
                                            CorrectNum += 1;
                                        }
                                    }

                                    TotalScore += CorrectNum * 1;

                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 5;
                                    TATI.CorrectItemNumber = CorrectNum;
                                    TATI.TotalItemNumber = RlpoTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 5;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 5;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 复合型听力题型统计

                                //复合型听力题型统计
                                if (pt.LpcList.Count > 0)
                                {
                                    int CorrectNum = 0;
                                    for (int i = 0; i < pt.LpcList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        LpcTotalNum += pt.LpcList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.LpcList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.LpcList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.LpcList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.LpcList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);

                                            string Answer = TestAnswer.Answer;
                                            Answer = Answer.Replace(" ", "");
                                            Answer = Answer.ToUpper();
                                            string UserAnswer = UserAs[(Number - 1)];
                                            UserAnswer = UserAnswer.Replace(" ", "");
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }


                                            TestAnswer.UserAnswer = UserAs[(Number - 1)];
                                            TestAnswer.AssessmentItemID = pt.LpcList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 4;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + LlpoTotalNum + SlpoTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum))
                                        {
                                            CorrectNum += 1;
                                        }
                                    }
                                    TotalScore += CorrectNum * 1;

                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 4;
                                    TATI.CorrectItemNumber = CorrectNum;
                                    TATI.TotalItemNumber = LpcTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 4;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 4;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }



                                #endregion

                                #region 阅读理解-选词填空题型统计

                                //阅读理解-选词填空题型统计
                                if (pt.RpcList.Count > 0)
                                {
                                    for (int i = 0; i < pt.RpcList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        RpcTotalNum += pt.RpcList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.RpcList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.RpcList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.RpcList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.RpcList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);
                                            TestAnswer.AssessmentItemID = pt.RpcList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 6;
                                            TestAnswer.Number = Number;
                                            Number += 1;

                                            string Answer = TestAnswer.Answer;
                                            Answer = Answer.Replace(" ", "");
                                            Answer = Answer.ToUpper();
                                            string UserAnswer = UserAs[(Number - 1)];
                                            UserAnswer = UserAnswer.Replace(" ", "");
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    int CorrectNum = 0;
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + RlpoTotalNum + RpcTotalNum))
                                        {
                                            CorrectNum += 1;
                                        }
                                    }
                                    TotalScore += CorrectNum * 1;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 6;
                                    TATI.CorrectItemNumber = CorrectNum;
                                    TATI.TotalItemNumber = RpcTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 6;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 6;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 阅读理解-选择题型统计

                                //阅读理解-选择题型统计
                                if (pt.RpoList.Count > 0)
                                {
                                    int CorrectNum = 0;
                                    for (int i = 0; i < pt.RpoList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        RpoTotalNum += pt.RpoList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.RpoList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.RpoList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.RpoList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.RpoList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);

                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.RpoList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 7;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum + RpcTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum + RpcTotalNum + RpoTotalNum))
                                        {
                                            CorrectNum += 1;
                                        }
                                    }
                                    TotalScore += CorrectNum * 1.5;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 7;
                                    TATI.CorrectItemNumber = CorrectNum;
                                    TATI.TotalItemNumber = RpoTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 7;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }

                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 7;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #region 完型填空题型统计

                                //完型填空题型统计
                                if (pt.CpList.Count > 0)
                                {
                                    int CorrectNum = 0;
                                    for (int i = 0; i < pt.CpList.Count; i++)
                                    {
                                        int time = r.Next(Ti_min, Ti_max + 1);//小题平均时间

                                        CpTotalNum += pt.CpList[i].Info.QuestionCount;

                                        //UserAssessmentCount表
                                        CEDTS_UserAssessmentCount UAC = new CEDTS_UserAssessmentCount();
                                        UAC.UserID = userID;
                                        UAC.AssessmentItemID = pt.CpList[i].Info.ItemID;
                                        if (_paper.SelectUA(UAC.UserID, UAC.AssessmentItemID) == 2)
                                        {
                                            UAC.UserAssessmentCountID = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).UserAssessmentCountID;
                                            UAC.Count = _paper.SelectUAC(UAC.UserID, UAC.AssessmentItemID).Count + 1;
                                            _paper.UpdataUAC(UAC);
                                        }
                                        else
                                        {
                                            UAC.Count = 1;
                                            _paper.CreateUAC(UAC);
                                        }


                                        //用户答卷信息
                                        for (int j = 0; j < pt.CpList[i].Info.QuestionID.Count; j++)
                                        {
                                            CEDTS_TestAnswer TestAnswer = new CEDTS_TestAnswer();
                                            Guid AssessmentID = pt.CpList[i].Info.QuestionID[j];

                                            TestAnswer.QuestionID = AssessmentID;
                                            TestAnswer.UserID = userID;
                                            TestAnswer.TestID = Test.TestID;
                                            TestAnswer.Answer = pt.CpList[i].Info.AnswerValue[j];
                                            TestAnswer.AnswerContent = _paper.AnswerContent(AssessmentID, TestAnswer.Answer);


                                            string Answer = TestAnswer.Answer;
                                            string UserAnswer = UserAs[(Number - 1)];
                                            Answer = Answer.ToUpper();
                                            UserAnswer = UserAnswer.ToUpper();
                                            if (Answer == UserAnswer)
                                            {
                                                TestAnswer.IsRight = true;
                                            }
                                            else
                                            {
                                                TestAnswer.IsRight = false;
                                            }
                                            TestAnswer.UserAnswer = UserAnswer;
                                            TestAnswer.AssessmentItemID = pt.CpList[i].Info.ItemID;
                                            TestAnswer.ItemAnswerTime = time;
                                            TestAnswer.ItemTypeID = 8;
                                            TestAnswer.Number = Number;
                                            Number += 1;
                                            _paper.CreateTestAnswer(TestAnswer);
                                        }
                                    }
                                    for (int j = 0; j < indexList.Count; j++)
                                    {
                                        if (indexList[j] > (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum + RpcTotalNum + RpoTotalNum) && indexList[j] < (SspcTotalNum + SlpoTotalNum + LlpoTotalNum + LpcTotalNum + RlpoTotalNum + RpcTotalNum + RpoTotalNum + CpTotalNum))
                                        {
                                            CorrectNum += 1;
                                        }
                                    }

                                    TotalScore += CorrectNum * 0.5;
                                    //TestAnswerTypeInfo表---用户答卷记录
                                    CEDTS_TestAnswerTypeInfo TATI = new CEDTS_TestAnswerTypeInfo();
                                    TATI.PaperID = Test.PaperID;
                                    TATI.TestID = Test.TestID;
                                    TATI.UserID = Test.UserID;
                                    TATI.ItemTypeID = 8;
                                    TATI.CorrectItemNumber = CorrectNum;
                                    TATI.TotalItemNumber = CpTotalNum;
                                    TATI.CorrectRate = TATI.CorrectItemNumber / TATI.TotalItemNumber;
                                    TATI.Time = DateTime.Now;
                                    _paper.CreateTATI(TATI);

                                    //UserAnswerInfo表----用户做过题型
                                    CEDTS_UserAnswerInfo UAI = new CEDTS_UserAnswerInfo();
                                    UAI.UserID = Test.UserID;
                                    UAI.ItemTypeID = 8;
                                    if (_paper.SelectUAINum(UAI.UserID, UAI.ItemTypeID) == 2)
                                    {
                                        UAI.UATI_ID = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).UATI_ID;
                                        UAI.TotalCount = _paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).TotalCount + 1;
                                        UAI.CorrectRate = (_paper.SelectUAI(UAI.UserID, UAI.ItemTypeID).CorrectRate + TATI.CorrectRate) / UAI.TotalCount;
                                        _paper.UpdataUATI(UAI);
                                    }
                                    else
                                    {
                                        UAI.CorrectRate = TATI.CorrectRate;
                                        UAI.TotalCount = 1;
                                        _paper.CreateUATI(UAI);
                                    }


                                    //mapleAnswerTypeInfo表----系统所有用户做过题型统计
                                    CEDTS_SmapleAnswerTypeInfo SATI = new CEDTS_SmapleAnswerTypeInfo();
                                    SATI.ItemTypeID = 8;
                                    if (_paper.SelectSATI(SATI.ItemTypeID) == 2)
                                    {
                                        SATI.SATI_ID = _paper.SelectSMAP(SATI.ItemTypeID).SATI_ID;
                                        SATI.TotalCount = _paper.SelectSMAP(SATI.ItemTypeID).TotalCount + 1;
                                        SATI.CorrectRate = (_paper.SelectSMAP(SATI.ItemTypeID).CorrectRate + TATI.CorrectRate) / SATI.TotalCount;
                                        _paper.UpdataSATI(SATI);
                                    }
                                    else
                                    {
                                        SATI.TotalCount = 1;
                                        SATI.CorrectRate = TATI.CorrectRate;
                                        _paper.CreateSATI(SATI);
                                    }

                                }
                                #endregion

                                #endregion

                                #region 知识点统计

                                //正确知识点统计

                                for (int m = 0; m < indexList.Count; m++)
                                {
                                    string kp1 = KpList[indexList[m]];
                                    string[] kp2 = kp1.Split('|');
                                    for (int n = 0; n < kp2.Length; n++)
                                    {
                                        for (int j = 0; j < kp2.Length; j++)
                                        {
                                            if (Kp.KnowledgePointID.Contains(Guid.Parse(kp2[j])))
                                            {
                                                for (int t = 0; t < Kp.KnowledgePointID.Count; t++)
                                                {
                                                    if (Kp.KnowledgePointID[t] == Guid.Parse(kp2[j]))
                                                    {

                                                        Kp.AverageTime[t] += _insert.selectQuestionKp(Guid.Parse(kp2[j]), Test.TestID);
                                                        Kp.TotalCount[t] += 1;
                                                        Kp.CorrectCount[t] += 1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Kp.KnowledgePointID.Add(Guid.Parse(kp2[j]));
                                                Kp.TotalCount.Add(1);
                                                Kp.CorrectCount.Add(1);
                                                Kp.AverageTime.Add(_insert.selectQuestionKp(Guid.Parse(kp2[j]), Test.TestID));
                                            }
                                        }
                                    }
                                }
                                //移除已统计的正确知识点
                                for (int i = indexList.Count - 1; i >= 0; i--)
                                {
                                    KpList.RemoveAt(indexList[i]);
                                }
                                //错误知识点统计
                                for (int h = 0; h < KpList.Count; h++)
                                {
                                    string kp1 = KpList[h];
                                    string[] kp2 = KpList[h].Split('|');
                                    for (int i = 0; i < kp2.Length; i++)
                                    {
                                        if (Kp.KnowledgePointID.Contains(Guid.Parse(kp2[i])))
                                        {
                                            for (int t = 0; t < Kp.KnowledgePointID.Count; t++)
                                            {
                                                if (Kp.KnowledgePointID[t] == Guid.Parse(kp2[i]))
                                                {
                                                    Kp.AverageTime[t] += _insert.selectQuestionKp(Guid.Parse(kp2[i]), Test.TestID);
                                                    Kp.TotalCount[t] += 1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Kp.AverageTime.Add(_insert.selectQuestionKp(Guid.Parse(kp2[i]), Test.TestID));
                                            Kp.KnowledgePointID.Add(Guid.Parse(kp2[i]));
                                            Kp.CorrectCount.Add(0);
                                            Kp.TotalCount.Add(1);
                                        }
                                    }
                                }



                                DataRow dr1 = dt1.NewRow();
                                DataRow dr3 = dt3.NewRow();
                                DataRow dr4 = dt4.NewRow();
                                for (int k = 0; k < Kp.KnowledgePointID.Count; k++)
                                {
                                    DataRow dr = dt.NewRow();


                                    CEDTS_SampleKnowledgeInfomation SKI = new CEDTS_SampleKnowledgeInfomation();
                                    CEDTS_TestAnswerKnowledgePoint TestAKP = new CEDTS_TestAnswerKnowledgePoint();
                                    CEDTS_UserKnowledgeInfomation UKI = new CEDTS_UserKnowledgeInfomation();

                                    SKI.KnowledgePointID = Kp.KnowledgePointID[k];//知识点ID

                                    TestAKP.TestID = Test.TestID; //用户当前测试ID
                                    TestAKP.PaperID = Test.PaperID;//当前试卷ID
                                    TestAKP.UserID = Test.UserID;//用户ID
                                    TestAKP.KnowledgePointID = Kp.KnowledgePointID[k];//知识点ID

                                    dr[0] = TestAKP.KnowledgePointID.ToString();
                                    dr[1] = _paper.SelectKnowledgeByID(TestAKP.KnowledgePointID).Title.ToString();

                                    TestAKP.CorrectItemNumber = Kp.CorrectCount[k];//知识点正确题数
                                    TestAKP.TotalItemNumber = Kp.TotalCount[k];//知识点总题数
                                    //知识点正确率=知识点正确题数/知识点总题数(当前测试知识点正确)
                                    TestAKP.CorrectRate = Convert.ToDouble(TestAKP.CorrectItemNumber) / Convert.ToDouble(TestAKP.TotalItemNumber);

                                    dr[3] = TestAKP.CorrectRate.ToString();

                                    TestAKP.AverageTime = (Kp.AverageTime[k] / Kp.TotalCount[k]);//当前知识点平均耗时（当前测试知识点平均时间）

                                    dr[6] = TestAKP.AverageTime.ToString();

                                    TestAKP.Time = DateTime.Now;




                                    UKI.UserID = Test.UserID;
                                    UKI.KnowledgePointID = Kp.KnowledgePointID[k];

                                    if (_paper.SelectSIK(Kp.KnowledgePointID[k]) == 2)
                                    {
                                        SKI.SKII_ID = _paper.SelectSKI(Kp.KnowledgePointID[k]).SKII_ID;
                                        //当前知识点在系统中练习次数
                                        SKI.TotalCount = _paper.SelectSKI(Kp.KnowledgePointID[k]).TotalCount + 1;
                                        //平均时间（知识点样本空间时间）
                                        SKI.AverageTime = (_paper.SelectSKI(Kp.KnowledgePointID[k]).AverageTime * _paper.SelectSKI(Kp.KnowledgePointID[k]).TotalCount + Kp.AverageTime[k] / Kp.TotalCount[k]) / SKI.TotalCount;
                                        //当前知识点在系统练习中正确率
                                        SKI.CorrectRate = (_paper.SelectSKI(Kp.KnowledgePointID[k]).CorrectRate * _paper.SelectSKI(Kp.KnowledgePointID[k]).TotalCount + TestAKP.CorrectRate) / SKI.TotalCount;
                                        //当前知识点掌握率（知识点掌握率）
                                        TestAKP.KP_MasterRate = (TestAKP.CorrectRate * _paper.SelectSKI(Kp.KnowledgePointID[k]).AverageTime * 3.2) / (TestAKP.AverageTime + 3.2 * _paper.SelectSKI(Kp.KnowledgePointID[k]).AverageTime);
                                        //当前知识点在系统练习中掌握率
                                        SKI.KP_MasterRate = (_paper.SelectSKI(Kp.KnowledgePointID[k]).KP_MasterRate * _paper.SelectSKI(Kp.KnowledgePointID[k]).TotalCount + TestAKP.KP_MasterRate) / SKI.TotalCount;

                                        dr[2] = _paper.SelectSKI(Kp.KnowledgePointID[k]).CorrectRate.ToString();
                                        dr[5] = _paper.SelectSKI(Kp.KnowledgePointID[k]).AverageTime.ToString();
                                        _paper.CreateTAKP(TestAKP);
                                        _paper.UpdataSKI(SKI);

                                        if (_paper.SelectUkINum(Test.UserID, Kp.KnowledgePointID[k]) == 2)
                                        {
                                            UKI.UKII_ID = _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).UKII_ID;
                                            //练习次数
                                            UKI.TotalCount = _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).TotalCount + 1;
                                            //平均时间（累计平均时间）
                                            UKI.AverageTime = (_paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).AverageTime * _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).TotalCount + Kp.AverageTime[k] / Kp.TotalCount[k]) / UKI.TotalCount;
                                            //正确率(个人样本空间知识点正确率 )(累计知识点正确率---查询)
                                            UKI.CorrectRate = (_paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).CorrectRate * _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).TotalCount + TestAKP.CorrectRate) / UKI.TotalCount;
                                            //掌握率
                                            UKI.KP_MasterRate = (_paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).KP_MasterRate * _paper.SelectUKI(Kp.KnowledgePointID[k], Test.UserID).TotalCount + TestAKP.KP_MasterRate) / UKI.TotalCount;
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


                                        TestAKP.KP_MasterRate = TestAKP.CorrectRate;//样本空间时间为0，当前第一次掌握率记为正确率
                                        SKI.KP_MasterRate = TestAKP.KP_MasterRate;
                                        _paper.CreateTAKP(TestAKP);
                                        _paper.CreateSKI(SKI);

                                        UKI.CorrectRate = TestAKP.CorrectRate;
                                        UKI.AverageTime = TestAKP.AverageTime;
                                        UKI.TotalCount = 1;
                                        UKI.KP_MasterRate = TestAKP.KP_MasterRate;

                                        dr[2] = 0;
                                        dr[5] = 0;
                                        _paper.CreateUKI(UKI);
                                    }
                                    #region C值算法伪造数据
                                    string KpID = TestAKP.KnowledgePointID.ToString();
                                    KpID = KpID.ToUpper();
                                    string tempAverageTime = TestAKP.AverageTime.ToString();
                                    string tempCorrectRate = TestAKP.CorrectRate.ToString();
                                    string tempMasterRate = TestAKP.KP_MasterRate.ToString();
                                    switch (KpID)
                                    {
                                        case "5EE3269D-0296-4C7D-B282-D5409FBE00DF":
                                            dr1[0] = tempAverageTime;
                                            dr3[0] = tempCorrectRate;
                                            dr4[0] = tempMasterRate;
                                            break;
                                        case "487B5E83-684E-4484-A965-4397D04468EE":
                                            dr1[1] = tempAverageTime;
                                            dr3[1] = tempCorrectRate;
                                            dr4[1] = tempMasterRate;
                                            break;
                                        case "876BFAD1-9EB3-47B9-B56F-C65A2E288019":
                                            dr1[2] = tempAverageTime;
                                            dr3[2] = tempCorrectRate;
                                            dr4[2] = tempMasterRate;
                                            break;
                                        case "86C01AA6-7C26-4EE9-9C71-FEFA18B7C835":
                                            dr1[3] = tempAverageTime;
                                            dr3[3] = tempCorrectRate;
                                            dr4[3] = tempMasterRate;
                                            break;
                                        case "84EB0A3E-0CB0-4DDD-81D8-1976162C4CA3":
                                            dr1[4] = tempAverageTime;
                                            dr3[4] = tempCorrectRate;
                                            dr4[4] = tempMasterRate;
                                            break;
                                        case "6CBCE252-FC2C-4B89-B8B6-E80F11068AA3":
                                            dr1[5] = tempAverageTime;
                                            dr3[5] = tempCorrectRate;
                                            dr4[5] = tempMasterRate;
                                            break;
                                        case "E6E791AD-174F-4A37-8806-5F3119DE7C79":
                                            dr1[6] = tempAverageTime;
                                            dr3[6] = tempCorrectRate;
                                            dr4[6] = tempMasterRate;
                                            break;
                                        case "E728EF68-20B3-4DE0-A8B7-FA5019DE94E0":
                                            dr1[7] = tempAverageTime;
                                            dr3[7] = tempCorrectRate;
                                            dr4[7] = tempMasterRate;
                                            break;
                                        case "09D3E61D-33DF-4F71-B84F-92AFB0A2A903":
                                            dr1[8] = tempAverageTime;
                                            dr3[8] = tempCorrectRate;
                                            dr4[8] = tempMasterRate;
                                            break;
                                        case "5240875C-9317-4857-99AC-516B14FE06E1":
                                            dr1[9] = tempAverageTime;
                                            dr3[9] = tempCorrectRate;
                                            dr4[9] = tempMasterRate;
                                            break;
                                        case "055E3537-66F0-460E-A94C-7C8FCD240F33":
                                            dr1[10] = tempAverageTime;
                                            dr3[10] = tempCorrectRate;
                                            dr4[10] = tempMasterRate;
                                            break;
                                        case "194A9727-0D8E-421C-9CF9-6D723520A9C2":
                                            dr1[11] = tempAverageTime;
                                            dr3[11] = tempCorrectRate;
                                            dr4[11] = tempMasterRate;
                                            break;
                                        case "274BB572-BC33-40DE-8705-C154FE1FD88E":
                                            dr1[12] = tempAverageTime;
                                            dr3[12] = tempCorrectRate;
                                            dr4[12] = tempMasterRate;
                                            break;
                                        case "D83DE643-1A42-4434-BDA1-7FC8B29B97CF":
                                            dr1[13] = tempAverageTime;
                                            dr3[13] = tempCorrectRate;
                                            dr4[13] = tempMasterRate;
                                            break;
                                        case "A763E476-EC3B-449E-881A-692A3B876A08":
                                            dr1[14] = tempAverageTime;
                                            dr3[14] = tempCorrectRate;
                                            dr4[14] = tempMasterRate;
                                            break;
                                        case "BC3B6C74-8BB4-43DD-83EF-A8C8C61B44F6":
                                            dr1[15] = tempAverageTime;
                                            dr3[15] = tempCorrectRate;
                                            dr4[15] = tempMasterRate;
                                            break;
                                        case "A02A23EC-D0EF-4408-9823-2A36FC3F7119":
                                            dr1[16] = tempAverageTime;
                                            dr3[16] = tempCorrectRate;
                                            dr4[16] = tempMasterRate;
                                            break;
                                        case "EF7E1646-389F-4AA3-8466-9FFE7AC81BE8":
                                            dr1[17] = tempAverageTime;
                                            dr3[17] = tempCorrectRate;
                                            dr4[17] = tempMasterRate;
                                            break;
                                        case "81DF5997-BE9C-4461-AD3D-A177FA54AA3A":
                                            dr1[18] = tempAverageTime;
                                            dr3[18] = tempCorrectRate;
                                            dr4[18] = tempMasterRate;
                                            break;
                                        case "A42EC9FE-5941-492A-8589-864522ADDC3A":
                                            dr1[19] = tempAverageTime;
                                            dr3[19] = tempCorrectRate;
                                            dr4[19] = tempMasterRate;
                                            break;
                                        case "96B46208-7B7A-45E1-B2FF-15EAA9595CE9":
                                            dr1[20] = tempAverageTime;
                                            dr3[20] = tempCorrectRate;
                                            dr4[20] = tempMasterRate;
                                            break;
                                        default:
                                            break;
                                    }

                                    #endregion
                                    dr[4] = SKI.CorrectRate;
                                    dr[7] = SKI.AverageTime.ToString();
                                    dr[8] = TestAKP.KP_MasterRate.ToString();
                                    dt.Rows.Add(dr);
                                }

                                dt1.Rows.Add(dr1);
                                dt3.Rows.Add(dr3);
                                dt4.Rows.Add(dr4);

                                DataRow dr_ave = dt.NewRow();
                                dr_ave[0] = Test.TestID.ToString();
                                dr_ave[1] = ((double)Right / (double)Count).ToString();
                                dt.Rows.Add(dr_ave);
                                dt.Rows.Add(dt.NewRow());
                                #endregion

                                Test.TotalScore = TotalScore;
                                _paper.UpdataTest(Test);
                                tran.Complete();
                            }
                        }
                    }
                    #endregion
                }
            }

            string[] listname1 = { "1.1Ti", "1.2Ti", "1.3Ti", "1.4Ti", "1.5Ti", "1.6Ti", "2.1Ti", "2.2Ti", "2.3Ti", "2.4Ti", "2.5Ti", "2.6Ti", "2.7Ti", "2.8Ti", "3.1Ti", "3.2Ti", "3.3Ti", "3.4Ti", "3.5Ti", "3.6Ti", "3.7Ti" };
            string[] cols1 = { "1.1Ti", "1.2Ti", "1.3Ti", "1.4Ti", "1.5Ti", "1.6Ti", "2.1Ti", "2.2Ti", "2.3Ti", "2.4Ti", "2.5Ti", "2.6Ti", "2.7Ti", "2.8Ti", "3.1Ti", "3.2Ti", "3.3Ti", "3.4Ti", "3.5Ti", "3.6Ti", "3.7Ti" };
            ToExcel.tableToExcel(dt1, listname1, cols1);


            string[] listname3 = { "1.1Xi", "1.2Xi", "1.3Xi", "1.4Xi", "1.5Xi", "1.6Xi", "2.1Xi", "2.2Xi", "2.3Xi", "2.4Xi", "2.5Xi", "2.6Xi", "2.7Xi", "2.8Xi", "3.1Xi", "3.2Xi", "3.3Xi", "3.4Xi", "3.5Xi", "3.6Xi", "3.7Xi" };
            string[] cols3 = { "1.1Xi", "1.2Xi", "1.3Xi", "1.4Xi", "1.5Xi", "1.6Xi", "2.1Xi", "2.2Xi", "2.3Xi", "2.4Xi", "2.5Xi", "2.6Xi", "2.7Xi", "2.8Xi", "3.1Xi", "3.2Xi", "3.3Xi", "3.4Xi", "3.5Xi", "3.6Xi", "3.7Xi" };
            ToExcel.tableToExcel(dt3, listname3, cols3);


            string[] listname4 = { "1.1Mi", "1.2Mi", "1.3Mi", "1.4Mi", "1.5Mi", "1.6Mi", "2.1Mi", "2.2Mi", "2.3Mi", "2.4Mi", "2.5Mi", "2.6Mi", "2.7Mi", "2.8Mi", "3.1Mi", "3.2Mi", "3.3Mi", "3.4Mi", "3.5Mi", "3.6Mi", "3.7Mi" };
            string[] cols4 = { "1.1Mi", "1.2Mi", "1.3Mi", "1.4Mi", "1.5Mi", "1.6Mi", "2.1Mi", "2.2Mi", "2.3Mi", "2.4Mi", "2.5Mi", "2.6Mi", "2.7Mi", "2.8Mi", "3.1Mi", "3.2Mi", "3.3Mi", "3.4Mi", "3.5Mi", "3.6Mi", "3.7Mi" };
            ToExcel.tableToExcel(dt4, listname4, cols4);

            string[] listname = { "知识点ID", "知识点名称", "个人样本空间知识点正确率", "当前测试知识点正确率", "累计知识点正确率", "知识点样本空间时间", "当前测试知识点平均时间", "累计平均时间", "知识点掌握率" };
            string[] cols = { "KnowledgeID", "KnowledgeName", "IAccuracy", "NAccuracy", "AAccuracy", "ITime", "NTime", "ATime", "Mrate" };
            ToExcel.tableToExcel(dt, listname, cols);

            System.Data.DataTable dt2 = _insert.GetTable();
            string[] listname2 = { "小题id", "小题对错", "小题用时", "小题对应知识点id", "小题对应知识点名称", "小题分值" };
            string[] cols2 = { "QuestionID", "IsRight", "AverageTime", "KnowledgePointID", "Title", "Score" };
            ToExcel.tableToExcel(dt2, listname2, cols2);

            DoExcel();
            return View();
        }

        public ActionResult doExercise()
        {
            System.Data.DataTable dt1 = new System.Data.DataTable();//为excel创建表格
            dt1.Columns.Add("1", System.Type.GetType("System.String"));
            dt1.Columns.Add("2", System.Type.GetType("System.String"));
            dt1.Columns.Add("3", System.Type.GetType("System.String"));
            dt1.Columns.Add("4", System.Type.GetType("System.String"));
            dt1.Columns.Add("5", System.Type.GetType("System.String"));
            dt1.Columns.Add("6", System.Type.GetType("System.String"));
            dt1.Columns.Add("7", System.Type.GetType("System.String"));
            dt1.Columns.Add("8", System.Type.GetType("System.String"));
            dt1.Columns.Add("9", System.Type.GetType("System.String"));
            dt1.Columns.Add("10", System.Type.GetType("System.String"));
            dt1.Columns.Add("11", System.Type.GetType("System.String"));
            dt1.Columns.Add("12", System.Type.GetType("System.String"));
            dt1.Columns.Add("13", System.Type.GetType("System.String"));
            dt1.Columns.Add("14", System.Type.GetType("System.String"));
            dt1.Columns.Add("15", System.Type.GetType("System.String"));
            dt1.Columns.Add("16", System.Type.GetType("System.String"));
            dt1.Columns.Add("17", System.Type.GetType("System.String"));
            dt1.Columns.Add("18", System.Type.GetType("System.String"));
            dt1.Columns.Add("19", System.Type.GetType("System.String"));
            dt1.Columns.Add("20", System.Type.GetType("System.String"));
            dt1.Columns.Add("21", System.Type.GetType("System.String"));
            dt1.Columns.Add("22", System.Type.GetType("System.String"));
            dt1.Columns.Add("23", System.Type.GetType("System.String"));
            dt1.Columns.Add("24", System.Type.GetType("System.String"));
            dt1.Columns.Add("25", System.Type.GetType("System.String"));
            dt1.Columns.Add("26", System.Type.GetType("System.String"));
            dt1.Columns.Add("27", System.Type.GetType("System.String"));
            dt1.Columns.Add("28", System.Type.GetType("System.String"));
            dt1.Columns.Add("29", System.Type.GetType("System.String"));
            dt1.Columns.Add("30", System.Type.GetType("System.String"));
            dt1.Columns.Add("31", System.Type.GetType("System.String"));
            dt1.Columns.Add("32", System.Type.GetType("System.String"));
            dt1.Columns.Add("33", System.Type.GetType("System.String"));
            dt1.Columns.Add("34", System.Type.GetType("System.String"));
            dt1.Columns.Add("35", System.Type.GetType("System.String"));
            dt1.Columns.Add("36", System.Type.GetType("System.String"));
            dt1.Columns.Add("37", System.Type.GetType("System.String"));
            dt1.Columns.Add("38", System.Type.GetType("System.String"));
            dt1.Columns.Add("39", System.Type.GetType("System.String"));
            dt1.Columns.Add("40", System.Type.GetType("System.String"));
            dt1.Columns.Add("41", System.Type.GetType("System.String"));
            dt1.Columns.Add("42", System.Type.GetType("System.String"));
            dt1.Columns.Add("43", System.Type.GetType("System.String"));
            dt1.Columns.Add("44", System.Type.GetType("System.String"));
            dt1.Columns.Add("45", System.Type.GetType("System.String"));
            dt1.Columns.Add("46", System.Type.GetType("System.String"));
            dt1.Columns.Add("47", System.Type.GetType("System.String"));
            dt1.Columns.Add("48", System.Type.GetType("System.String"));
            dt1.Columns.Add("49", System.Type.GetType("System.String"));
            dt1.Columns.Add("50", System.Type.GetType("System.String"));
            dt1.Columns.Add("51", System.Type.GetType("System.String"));
            dt1.Columns.Add("52", System.Type.GetType("System.String"));
            dt1.Columns.Add("53", System.Type.GetType("System.String"));
            dt1.Columns.Add("54", System.Type.GetType("System.String"));
            dt1.Columns.Add("55", System.Type.GetType("System.String"));
            dt1.Columns.Add("56", System.Type.GetType("System.String"));
            dt1.Columns.Add("57", System.Type.GetType("System.String"));
            dt1.Columns.Add("58", System.Type.GetType("System.String"));
            dt1.Columns.Add("59", System.Type.GetType("System.String"));
            dt1.Columns.Add("60", System.Type.GetType("System.String"));
            dt1.Columns.Add("61", System.Type.GetType("System.String"));
            dt1.Columns.Add("62", System.Type.GetType("System.String"));
            dt1.Columns.Add("63", System.Type.GetType("System.String"));
            dt1.Columns.Add("64", System.Type.GetType("System.String"));
            dt1.Columns.Add("65", System.Type.GetType("System.String"));
            dt1.Columns.Add("66", System.Type.GetType("System.String"));
            dt1.Columns.Add("67", System.Type.GetType("System.String"));
            dt1.Columns.Add("68", System.Type.GetType("System.String"));
            dt1.Columns.Add("69", System.Type.GetType("System.String"));
            dt1.Columns.Add("70", System.Type.GetType("System.String"));
            dt1.Columns.Add("71", System.Type.GetType("System.String"));
            dt1.Columns.Add("72", System.Type.GetType("System.String"));
            dt1.Columns.Add("73", System.Type.GetType("System.String"));
            dt1.Columns.Add("74", System.Type.GetType("System.String"));
            dt1.Columns.Add("75", System.Type.GetType("System.String"));
            dt1.Columns.Add("76", System.Type.GetType("System.String"));
            dt1.Columns.Add("77", System.Type.GetType("System.String"));
            dt1.Columns.Add("78", System.Type.GetType("System.String"));
            dt1.Columns.Add("79", System.Type.GetType("System.String"));
            dt1.Columns.Add("80", System.Type.GetType("System.String"));
            dt1.Columns.Add("81", System.Type.GetType("System.String"));
            dt1.Columns.Add("82", System.Type.GetType("System.String"));
            dt1.Columns.Add("83", System.Type.GetType("System.String"));
            dt1.Columns.Add("84", System.Type.GetType("System.String"));
            dt1.Columns.Add("85", System.Type.GetType("System.String"));
            dt1.Columns.Add("86", System.Type.GetType("System.String"));
            dt1.Columns.Add("87", System.Type.GetType("System.String"));
            dt1.Columns.Add("88", System.Type.GetType("System.String"));

            
            List<Excel1> list = _insert.selectTestAnswer();
             DataRow dr1 = dt1.NewRow();
            for (int i = 0; i < list.Count; i++)
            {
               
                dr1[0] = list[i].id;
                for (int j = 0; j < list[i].ii.rigth.Count; j++)
                {
                    dr1[j + 1] = list[i].ii.rigth.ToString();
                }
            }
            dt1.Rows.Add(dr1);
            string[] listname1 = { "1", "2", "3", "4", "5", "6", "8", "9", "10", "11", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88" };
            string[] cols1 = { "1", "2", "3", "4", "5", "6", "8", "9", "10", "11", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "80", "81", "82", "83", "84", "85", "86", "87", "88" };
            ToExcel.tableToExcel(dt1, listname1, cols1);
            DoExcel();
            return View();
        }

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
    }
}
