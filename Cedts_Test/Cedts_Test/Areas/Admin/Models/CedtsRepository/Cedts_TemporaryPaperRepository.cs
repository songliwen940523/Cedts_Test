using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_TemporaryPaperRepository : ICedts_TemporaryPaperRepository
    {

        #region 实例化Cedts_Entities
        Cedts_Entities db;
        public Cedts_TemporaryPaperRepository()
        {
            db = new Cedts_Entities();
        }
        #endregion

  

        #region 保存到TemporaryPaperAssessment临时表
        void ICedts_TemporaryPaperRepository.SavePaperAssessment(CEDTS_TemporaryPaperAssessment tpa)
        {
            db.AddToCEDTS_TemporaryPaperAssessment(tpa);
            db.SaveChanges();
        }
        #endregion

        #region 查询编辑试卷信息
        CEDTS_Paper ICedts_TemporaryPaperRepository.SelectEditPaper(Guid id)
        {
            var PaperInfo = (from m in db.CEDTS_Paper where m.PaperID == id select m).First();
            return PaperInfo;
        }
        #endregion

        #region 查询试卷扩展表信息 
        CEDTS_PaperExpansion ICedts_TemporaryPaperRepository.SelectEditPaperExpansion(Guid id)
        {
            var Expansion = (from m in db.CEDTS_PaperExpansion where m.PaperID == id select m).FirstOrDefault();
            return Expansion;
        }
        #endregion

        #region 查询时页面跳转
        int ICedts_TemporaryPaperRepository.Judge(Guid id)
        {
          
            var s = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 2 select m).ToList().Count();
            var s1 = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 3 select m).ToList().Count();
            var s2 = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 4 select m).ToList().Count();
            var s3 = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 5 select m).ToList().Count();
            var s4 = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 6 select m).ToList().Count();
            var s5 = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 7 select m).ToList().Count();
            var s6 = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 12 select m).ToList().Count();
            var s7 = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 8 select m).ToList().Count();

            var s8 = (from a in db.CEDTS_PaperExpansion where a.PaperID == id select a).FirstOrDefault();
            if (s == 0 && s8.ShortListenNum != 0)
            {
                return 1;
            }
            if (s1 == 0 && s8.LongListenNum != 0)
            {
                return 2;
            }
            if (s2 == 0 && s8.ComprehensionListenNum != 0)
            {
                return 3;
            }
            if (s3 == 0 && s8.ComplexListenNum != 0)
            {
                return 4;
            }
            if (s4 == 0 && s8.BankedClozeNum != 0)
            {
                return 5;
            }
            if (s5 == 0 && s8.MultipleChoiceNum != 0)
            {
                return 6;
            }
            if (s6 == 0 && s8.InfoMatchingNum != 0)
            {
                return 7;
            }
            if (s7 == 0 && s8.ClozeNum != 0)
            {
                return 8;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 编辑时查询快速阅读信息
        List<SkimmingScanningPartCompletion> ICedts_TemporaryPaperRepository.SelectEditSspc(Guid id, int UserID)
        {
            List<SkimmingScanningPartCompletion> SspcList = new List<SkimmingScanningPartCompletion>();

            var AssmentIDList = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 1 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssmentID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssmentID orderby m.Order ascending select m).ToList();
                var ChoiceNum = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssmentID && m.ChooseA != "" && m.QuestionContent != null orderby m.Order select m).ToList();
                var TermNum = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssmentID && m.ChooseA == "" orderby m.Order select m).ToList();
                var Assess = (from m in db.CEDTS_TemporaryAssessmentItem where m.AssessmentItemID == AssmentID select m).FirstOrDefault();
                SkimmingScanningPartCompletion Sspc = new SkimmingScanningPartCompletion();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = AssmentID.Value;
                Sspc.Choices = new List<string>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.DifficultQuestion = new List<double>();
                info.Problem = new List<string>();
                info.KnowledgeID = new List<string>();
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.Knowledge = new List<string>();


                info.Count = Assess.Count.Value;
                info.ItemType = "1";
                info.ItemType_CN = "快速阅读";
                info.PartType = "1";
                info.QuestionCount = Assess.QuestionCount.Value;
                info.ReplyTime = Assess.Duration.Value;
                info.Diffcult = Assess.Difficult.Value;
                info.Score = Assess.Score.Value;
                Sspc.ChoiceNum = ChoiceNum.Count;
                Sspc.TermNum = TermNum.Count;
                Sspc.Content = Assess.Original;

                string QkID = null;
                if (UserID == 1)
                {
                    foreach (var qu in question)
                    {
                        var QkList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                        foreach (var q in QkList)
                        {
                            QkID += q + ",";
                        }
                    }
                    QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                }
                info.QuestionKnowledgeID = QkID;

                foreach (var choic in ChoiceNum)
                {
                    info.QuestionID.Add(choic.QuestionID);
                    info.Problem.Add(choic.QuestionContent);
                    info.ScoreQuestion.Add(choic.Score.Value);
                    info.TimeQuestion.Add(choic.Duration.Value);
                    info.DifficultQuestion.Add(choic.Difficult.Value);
                    Sspc.Choices.Add(choic.ChooseA);
                    Sspc.Choices.Add(choic.ChooseB);
                    Sspc.Choices.Add(choic.ChooseC);
                    Sspc.Choices.Add(choic.ChooseD);
                    info.AnswerValue.Add(choic.Answer);
                    info.Tip.Add(choic.Analyze);
                }

                foreach (var term in TermNum)
                {
                    info.QuestionID.Add(term.QuestionID);
                    info.Problem.Add(term.QuestionContent);
                    info.ScoreQuestion.Add(term.Score.Value);
                    info.TimeQuestion.Add(term.Duration.Value);
                    info.DifficultQuestion.Add(term.Difficult.Value);
                    info.AnswerValue.Add(term.Answer);
                    info.Tip.Add(term.Analyze);
                }

                if (UserID == 1)
                {
                    foreach (var list in question)
                    {
                        string name = string.Empty;
                        string Qkid = string.Empty;

                        var KnowledgeList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                        foreach (var Knowledge in KnowledgeList)
                        {
                            var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).FirstOrDefault();
                            Qkid += KnowledgeName.KnowledgePointID + ",";
                            name += KnowledgeName.Title + ",";
                        }
                        name = name.Substring(0, name.LastIndexOf(","));
                        Qkid = Qkid.Substring(0, Qkid.LastIndexOf(','));
                        info.KnowledgeID.Add(Qkid);
                        info.Knowledge.Add(name);
                    }
                }
                Sspc.Info = info;
                SspcList.Add(Sspc);
            }
            return SspcList;
        }
        #endregion

        #region 编辑是查询短对话信息
        List<Listen> ICedts_TemporaryPaperRepository.SelectEditSlpo(Guid id, int UserID)
        {
            List<Listen> SlpoList = new List<Listen>();
            var AssmentIDList = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 2 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var ChoiceNum = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID && m.ChooseA != null && m.QuestionContent != null orderby m.Order select m).ToList();
                var TermNum = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID && m.ChooseA == null orderby m.Order select m).ToList();
                var Assess = (from m in db.CEDTS_TemporaryAssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
                Listen Slpo = new Listen();
                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                Slpo.Choices = new List<string>();
                info.TimeQuestion = new List<double>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.QustionInterval = Assess.Interval.ToString();
                info.ItemID = AssessID.Value;
                info.Count = Assess.Count.Value;
                info.ItemType = "2";
                info.ItemType_CN = "短对话听力";
                info.PartType = "2";
                info.QuestionCount = Assess.QuestionCount.Value;
                info.ReplyTime = Assess.Duration.Value;
                info.Diffcult = Assess.Difficult.Value;
                info.Score = Assess.Score.Value;

                Slpo.Script = Assess.Original;
                Slpo.SoundFile = AssessID.ToString();

                string QkID = null;
                if (UserID == 1)
                {
                    foreach (var qu in question)
                    {
                        var QkList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                        foreach (var q in QkList)
                        {
                            QkID += q + ",";
                        }
                    }
                    QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                }
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.Problem.Add(list.QuestionContent);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    info.timeInterval.Add(list.Interval.Value);
                    info.questionSound.Add(list.Sound);
                    Slpo.Choices.Add(list.ChooseA);
                    Slpo.Choices.Add(list.ChooseB);
                    Slpo.Choices.Add(list.ChooseC);
                    Slpo.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);

                    if (UserID == 1)
                    {
                        string name = string.Empty;
                        string Qkid = string.Empty;
                        var KnowledgeList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                        foreach (var Knowledge in KnowledgeList)
                        {
                            var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                            Qkid += KnowledgeName.KnowledgePointID + ",";
                            name += KnowledgeName.Title + ",";
                        }
                        name = name.Substring(0, name.LastIndexOf(","));
                        Qkid = Qkid.Substring(0, Qkid.LastIndexOf(','));
                        info.KnowledgeID.Add(Qkid);
                        info.Knowledge.Add(name);
                    }
                }
                Slpo.Info = info;
                SlpoList.Add(Slpo);
            }
            return SlpoList;
        }
        #endregion

        #region 编辑时查询长对话信息
        List<Listen> ICedts_TemporaryPaperRepository.SelectEditLlpo(Guid id, int UserID)
        {
            List<Listen> LlpoList = new List<Listen>();
            var AssmentIDList = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 3 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var Assess = (from m in db.CEDTS_TemporaryAssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
                Listen Llpo = new Listen();
                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                Llpo.Choices = new List<string>();
                info.TimeQuestion = new List<double>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.QustionInterval = Assess.Interval.ToString();
                info.ItemID = AssessID.Value;
                info.Count = Assess.Count.Value;
                info.ItemType = "3";
                info.ItemType_CN = "长对话听力";
                info.PartType = "2";
                info.QuestionCount = Assess.QuestionCount.Value;
                info.ReplyTime = Assess.Duration.Value;
                info.Diffcult = Assess.Difficult.Value;
                info.Score = Assess.Score.Value;

                Llpo.Script = Assess.Original;
                Llpo.SoundFile = AssessID.ToString();

                string QkID = null;
                if (UserID == 1)
                {
                    foreach (var qu in question)
                    {
                        var QkList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                        foreach (var q in QkList)
                        {
                            QkID += q + ",";
                        }
                    }
                    QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                }
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.Problem.Add(list.QuestionContent);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    info.questionSound.Add(list.Sound);
                    info.timeInterval.Add(list.Interval.Value);
                    Llpo.Choices.Add(list.ChooseA);
                    Llpo.Choices.Add(list.ChooseB);
                    Llpo.Choices.Add(list.ChooseC);
                    Llpo.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);

                    if (UserID == 1)
                    {
                        string name = string.Empty;
                        string Qkid = string.Empty;
                        var KnowledgeList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                        foreach (var Knowledge in KnowledgeList)
                        {
                            var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                            Qkid += KnowledgeName.KnowledgePointID + ",";
                            name += KnowledgeName.Title + ",";
                        }
                        name = name.Substring(0, name.LastIndexOf(","));
                        Qkid = Qkid.Substring(0, Qkid.LastIndexOf(','));
                        info.KnowledgeID.Add(Qkid);
                        info.Knowledge.Add(name);
                    }
                }
                Llpo.Info = info;
                LlpoList.Add(Llpo);
            }
            return LlpoList;
        }
        #endregion

        #region 编辑时查询听力短文理解信息
        List<Listen> ICedts_TemporaryPaperRepository.SelectEditRlpo(Guid id, int UserID)
        {
            List<Listen> RlpoList = new List<Listen>();
            var AssmentIDList = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 4 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var Assess = (from m in db.CEDTS_TemporaryAssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
                Listen Rlpo = new Listen();
                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                Rlpo.Choices = new List<string>();
                info.TimeQuestion = new List<double>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.QustionInterval = Assess.Interval.ToString();
                info.ItemID = AssessID.Value;
                info.Count = Assess.Count.Value;
                info.ItemType = "4";
                info.ItemType_CN = "短文听力理解";
                info.PartType = "2";
                info.QuestionCount = Assess.QuestionCount.Value;
                info.ReplyTime = Assess.Duration.Value;
                info.Diffcult = Assess.Difficult.Value;
                info.Score = Assess.Score.Value;

                Rlpo.Script = Assess.Original;
                Rlpo.SoundFile = AssessID.ToString();

                string QkID = null;
                if (UserID == 1)
                {
                    foreach (var qu in question)
                    {
                        var QkList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                        foreach (var q in QkList)
                        {
                            QkID += q + ",";
                        }
                    }
                    QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                }
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.Problem.Add(list.QuestionContent);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    info.timeInterval.Add(list.Interval.Value);
                    info.questionSound.Add(list.Sound);
                    Rlpo.Choices.Add(list.ChooseA);
                    Rlpo.Choices.Add(list.ChooseB);
                    Rlpo.Choices.Add(list.ChooseC);
                    Rlpo.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);

                    if (UserID == 1)
                    {
                        string name = string.Empty;
                        string Qkid = string.Empty;
                        var KnowledgeList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                        foreach (var Knowledge in KnowledgeList)
                        {
                            var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                            Qkid += KnowledgeName.KnowledgePointID + ",";
                            name += KnowledgeName.Title + ",";
                        }
                        name = name.Substring(0, name.LastIndexOf(","));
                        Qkid = Qkid.Substring(0, Qkid.LastIndexOf(','));
                        info.KnowledgeID.Add(Qkid);
                        info.Knowledge.Add(name);
                    }
                }
                Rlpo.Info = info;
                RlpoList.Add(Rlpo);
            }
            return RlpoList;
        }
        #endregion

        #region 编辑时查询复合型听力信息
        List<Listen> ICedts_TemporaryPaperRepository.SelectEditLpc(Guid id, int UserID)
        {
            List<Listen> LpcList = new List<Listen>();
            var AssmentIDList = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 5 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var Assess = (from m in db.CEDTS_TemporaryAssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
                Listen Lpc = new Listen();
                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();

                info.TimeQuestion = new List<double>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.QustionInterval = Assess.Interval.ToString();
                info.ItemID = AssessID.Value;
                info.Count = Assess.Count.Value;
                info.ItemType = "5";
                info.ItemType_CN = "复合型听力";
                info.PartType = "2";
                info.QuestionCount = Assess.QuestionCount.Value;
                info.ReplyTime = Assess.Duration.Value;
                info.Diffcult = Assess.Difficult.Value;
                info.Score = Assess.Score.Value;

                Lpc.Script = Assess.Original;
                Lpc.SoundFile = AssessID.ToString();

                string QkID = null;
                if (UserID == 1)
                {
                    foreach (var qu in question)
                    {
                        var QkList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                        foreach (var q in QkList)
                        {
                            QkID += q + ",";
                        }
                    }
                    QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                }
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.Problem.Add(list.QuestionContent);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    info.AnswerValue.Add(list.Answer);
                    info.questionSound.Add(list.Sound);
                    info.timeInterval.Add(list.Interval.Value);
                    info.Tip.Add(list.Analyze);

                    if (UserID == 1)
                    {
                        string name = string.Empty;
                        string Qkid = string.Empty;
                        var KnowledgeList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                        foreach (var Knowledge in KnowledgeList)
                        {
                            var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                            Qkid += KnowledgeName.KnowledgePointID + ",";
                            name += KnowledgeName.Title + ",";
                        }
                        name = name.Substring(0, name.LastIndexOf(","));
                        Qkid = Qkid.Substring(0, Qkid.LastIndexOf(','));
                        info.KnowledgeID.Add(Qkid);
                        info.Knowledge.Add(name);
                    }
                }
                Lpc.Info = info;
                LpcList.Add(Lpc);
            }
            return LpcList;
        }
        #endregion

        #region 编辑时查询阅读理解-选词填空信息
        List<ReadingPartCompletion> ICedts_TemporaryPaperRepository.SelectEditRpc(Guid id, int UserID)
        {
            List<ReadingPartCompletion> RpcList = new List<ReadingPartCompletion>();
            var AssmentIDList = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 6 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_TemporaryAssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
                var Expansion = (from m in db.CEDTS_TemporaryExpansion where m.AssessmentItemID == AssessID select m).FirstOrDefault();
                ReadingPartCompletion Rpc = new ReadingPartCompletion();
                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();

                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.TimeQuestion = new List<double>();
                Rpc.WordList = new List<string>();
                info.ItemID = AssessID.Value;
                info.Count = assess.Count.Value;
                info.ItemType = "6";
                info.ItemType_CN = "阅读选词填空";
                info.PartType = "3";
                info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
                info.ReplyTime = Convert.ToInt32(assess.Duration);
                info.Diffcult = Convert.ToDouble(assess.Difficult);
                info.Score = Convert.ToInt32(assess.Score);

                Rpc.Content = assess.Original;

                Rpc.ExpansionID = Expansion.ExpansionID;
                Rpc.WordList.Add(Expansion.ChoiceA);
                Rpc.WordList.Add(Expansion.ChoiceB);
                Rpc.WordList.Add(Expansion.ChoiceC);
                Rpc.WordList.Add(Expansion.ChoiceD);
                Rpc.WordList.Add(Expansion.ChoiceE);
                Rpc.WordList.Add(Expansion.ChoiceF);
                Rpc.WordList.Add(Expansion.ChoiceG);
                Rpc.WordList.Add(Expansion.ChoiceH);
                Rpc.WordList.Add(Expansion.ChoiceI);
                Rpc.WordList.Add(Expansion.ChoiceJ);
                Rpc.WordList.Add(Expansion.ChoiceK);
                Rpc.WordList.Add(Expansion.ChoiceL);
                Rpc.WordList.Add(Expansion.ChoiceM);
                Rpc.WordList.Add(Expansion.ChoiceN);
                Rpc.WordList.Add(Expansion.ChoiceO);


                string QkID = null;
                if (UserID == 1)
                {
                    foreach (var qu in question)
                    {
                        var QkList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                        foreach (var q in QkList)
                        {
                            QkID += q + ",";
                        }
                    }
                    QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                }
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);

                    if (UserID == 1)
                    {
                        string name = string.Empty;
                        string Qkid = string.Empty;
                        var KnowledgeList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                        foreach (var Knowledge in KnowledgeList)
                        {
                            var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                            Qkid += KnowledgeName.KnowledgePointID + ",";
                            name += KnowledgeName.Title + ",";
                        }
                        name = name.Substring(0, name.LastIndexOf(","));
                        Qkid = Qkid.Substring(0, Qkid.LastIndexOf(','));
                        info.KnowledgeID.Add(Qkid);
                        info.Knowledge.Add(name);
                    }
                }
                Rpc.Info = info;
                RpcList.Add(Rpc);
            }
            return RpcList;
        }
        #endregion

        #region 编辑时查询阅读理解-选择题型信息
        List<ReadingPartOption> ICedts_TemporaryPaperRepository.SlelectEditRpo(Guid id, int UserID)
        {
            List<ReadingPartOption> RpoList = new List<ReadingPartOption>();
            var AssmentIDList = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 7 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_TemporaryAssessmentItem where m.AssessmentItemID == AssessID select m).First();

                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                ReadingPartOption Rpo = new ReadingPartOption();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                Rpo.Choices = new List<string>();
                info.TimeQuestion = new List<double>();
                info.ItemID = AssessID.Value;
                info.Count = assess.Count.Value;
                info.ItemType = "7";
                info.ItemType_CN = "阅读选择题型";
                info.PartType = "4";
                info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
                info.ReplyTime = Convert.ToInt32(assess.Duration);
                info.Diffcult = Convert.ToDouble(assess.Difficult);
                info.Score = Convert.ToInt32(assess.Score);

                Rpo.Content = assess.Original;

                string QkID = null;
                if (UserID == 1)
                {
                    foreach (var qu in question)
                    {
                        var QkList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                        foreach (var q in QkList)
                        {
                            QkID += q + ",";
                        }
                    }
                    QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                }
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.Problem.Add(list.QuestionContent);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    Rpo.Choices.Add(list.ChooseA);
                    Rpo.Choices.Add(list.ChooseB);
                    Rpo.Choices.Add(list.ChooseC);
                    Rpo.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);

                    if (UserID == 1)
                    {
                        string name = string.Empty;
                        string Qkid = string.Empty;
                        var KnowledgeList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                        foreach (var Knowledge in KnowledgeList)
                        {
                            var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                            Qkid += KnowledgeName.KnowledgePointID + ",";
                            name += KnowledgeName.Title + ",";
                        }
                        name = name.Substring(0, name.LastIndexOf(","));
                        Qkid = Qkid.Substring(0, Qkid.LastIndexOf(','));
                        info.KnowledgeID.Add(Qkid);
                        info.Knowledge.Add(name);
                    }
                }
                Rpo.Info = info;
                RpoList.Add(Rpo);
            }
            return RpoList;
        }
        #endregion

        #region 编辑时查询阅读理解-信息匹配
        List<InfoMatchingCompletion> ICedts_TemporaryPaperRepository.SlelectEditInfoMat(Guid id, int UserID)
        {
            List<InfoMatchingCompletion> InfoMatList = new List<InfoMatchingCompletion>();
            var AssmentIDList = (from m in db.CEDTS_TemporaryPaperAssessment where m.PaperID == id && m.ItemTypeID == 12 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_TemporaryQuestion where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_TemporaryAssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
                var Expansion = (from m in db.CEDTS_TemporaryExpansion where m.AssessmentItemID == AssessID select m).FirstOrDefault();
                InfoMatchingCompletion InfoMat = new InfoMatchingCompletion();
                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();

                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.TimeQuestion = new List<double>();
                InfoMat.WordList = new List<string>();
                info.ItemID = AssessID.Value;
                info.Count = assess.Count.Value;
                info.ItemType = "12";
                info.ItemType_CN = "信息匹配";
                info.PartType = "3";
                info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
                info.ReplyTime = Convert.ToInt32(assess.Duration);
                info.Diffcult = Convert.ToDouble(assess.Difficult);
                info.Score = Convert.ToInt32(assess.Score);

                InfoMat.Content = assess.Original;

                InfoMat.ExpansionID = Expansion.ExpansionID;
                InfoMat.WordList.Add(Expansion.ChoiceA);
                InfoMat.WordList.Add(Expansion.ChoiceB);
                InfoMat.WordList.Add(Expansion.ChoiceC);
                InfoMat.WordList.Add(Expansion.ChoiceD);
                InfoMat.WordList.Add(Expansion.ChoiceE);
                InfoMat.WordList.Add(Expansion.ChoiceF);
                InfoMat.WordList.Add(Expansion.ChoiceG);
                InfoMat.WordList.Add(Expansion.ChoiceH);
                InfoMat.WordList.Add(Expansion.ChoiceI);
                InfoMat.WordList.Add(Expansion.ChoiceJ);
                InfoMat.WordList.Add(Expansion.ChoiceK);
                InfoMat.WordList.Add(Expansion.ChoiceL);
                InfoMat.WordList.Add(Expansion.ChoiceM);
                InfoMat.WordList.Add(Expansion.ChoiceN);
                InfoMat.WordList.Add(Expansion.ChoiceO);


                string QkID = null;
                if (UserID == 1)
                {
                    foreach (var qu in question)
                    {
                        var QkList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                        foreach (var q in QkList)
                        {
                            QkID += q + ",";
                        }
                    }
                    QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                }
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);

                    if (UserID == 1)
                    {
                        string name = string.Empty;
                        string Qkid = string.Empty;
                        var KnowledgeList = (from m in db.CEDTS_TemporaryQuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                        foreach (var Knowledge in KnowledgeList)
                        {
                            var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                            Qkid += KnowledgeName.KnowledgePointID + ",";
                            name += KnowledgeName.Title + ",";
                        }
                        name = name.Substring(0, name.LastIndexOf(","));
                        Qkid = Qkid.Substring(0, Qkid.LastIndexOf(','));
                        info.KnowledgeID.Add(Qkid);
                        info.Knowledge.Add(name);
                    }
                }
                InfoMat.Info = info;
                InfoMatList.Add(InfoMat);
            }
            return InfoMatList;
        }
        #endregion
    }
}