using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_AssignRepository : ICedts_AssignRepository
    {
        Cedts_Entities db;
        public Cedts_AssignRepository()
        {
            db = new Cedts_Entities();
        }
        public static List<ExaminationItem> examitem = new List<ExaminationItem>();

        PagedList<ExaminationItem> ICedts_AssignRepository.SelectItemsByCondition(int? id, string condition, string txt)
        {
            int defaultPageSize = 10;
            if (txt != "")
            {                
                Guid ID = Guid.Parse(txt);
                IQueryable<ExaminationItem> q = examitem.Where(p => p.AssessmentItemID == ID).ToList().AsQueryable<ExaminationItem>();
                PagedList<ExaminationItem> ss = q.ToPagedList(id ?? 1, defaultPageSize);
                return ss;
            }
            else
            {
                IQueryable<ExaminationItem> q = from m in db.CEDTS_AssignAssessment
                                                from s in db.CEDTS_ItemType                                            
                                                orderby m.SaveTime descending
                                                where m.ItemTypeID == s.ItemTypeID && m.State==true                                        
                                                select new ExaminationItem
                                                {
                                                    AssessmentItemID = m.AssessmentItemID,
                                                    ItemName = s.TypeName_CN,                                                    
                                                };
                examitem = q.ToList();
                IQueryable<ExaminationItem> d = q.AsQueryable<ExaminationItem>();
                PagedList<ExaminationItem> ss = d.ToPagedList(id ?? 1, defaultPageSize);
                return ss;
            }
        }
        Listen ICedts_AssignRepository.SelectListen(Guid item)
        {
            var Assigns = (from As in db.CEDTS_Assign where As.AssessmentItemID == item orderby As.Time descending select As.QuestionID).FirstOrDefault();
            if (Assigns == null)
            {
                return null;
            }
            else
            {
                string[] Assign = Assigns.Split(',');
                var question1 = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
                List<Guid> QID = new List<Guid>();
                int Count = 0;
                for (int s = 0; s < Assign.Length; s++)
                {
                    QID.Add(Guid.Parse(Assign[s]));
                }
               
                    foreach (var q in question1)
                    {
                        if (!QID.Contains(q.QuestionID))
                        {
                            question.Remove(q);
                            if (Count == 0)
                            {
                                Count = assess.QuestionCount.Value - 1;
                            }
                            else
                            {
                                Count = Count - 1;
                            }
                        }                        
                    }
                    if (Count == 0)
                    {
                        Count = assess.QuestionCount.Value;
                    }
                var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
                var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();



                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                Listen listen = new Listen();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                listen.Choices = new List<string>();
                info.TimeQuestion = new List<double>();
                info.ItemID = item;
                info.Count = Convert.ToInt32(assess.Count);
                info.ItemType = Itemtype.TypeName;
                info.ItemType_CN = Itemtype.TypeName_CN;
                info.PartType = PartType.TypeName;
                info.QuestionCount = Convert.ToInt32(question.Count);
                info.ReplyTime = Convert.ToInt32(assess.Duration);
                info.Diffcult = Convert.ToDouble(assess.Difficult);
                info.Score = Convert.ToInt32(assess.Score);

                listen.Script = assess.Original;
                listen.SoundFile = item.ToString();

                string QkID = null;
                foreach (var qu in question)
                {
                    var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                    foreach (var q in QkList)
                    {
                        QkID += q + ",";
                    }
                }
                QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.Problem.Add(list.QuestionContent);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    listen.Choices.Add(list.ChooseA);
                    listen.Choices.Add(list.ChooseB);
                    listen.Choices.Add(list.ChooseC);
                    listen.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);
                    string name = string.Empty;
                    string id = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                    foreach (var Knowledge in KnowledgeList)
                    {
                        var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                        id += KnowledgeName.KnowledgePointID + ",";
                        name += KnowledgeName.Title + ",";
                    }
                    name = name.Substring(0, name.LastIndexOf(","));
                    id = id.Substring(0, id.LastIndexOf(','));
                    info.KnowledgeID.Add(id);
                    info.Knowledge.Add(name);
                }
                listen.Info = info;
                return listen;
            }
        }
        /// <summary>
        /// 查询当前点击的复合型听力信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的复合型听力信息</returns>
        Listen ICedts_AssignRepository.SelectComplex(Guid item)
        {
            var Assigns = (from As in db.CEDTS_Assign where As.AssessmentItemID == item orderby As.Time descending select As.QuestionID).FirstOrDefault();
            if (Assigns == null)
            {
                return null;
            }
            else
            {
                string[] Assign = Assigns.Split(',');
                var question1 = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
                List<Guid> QID = new List<Guid>();
                int Count = 0;
                for (int s = 0; s < Assign.Length; s++)
                {
                    QID.Add(Guid.Parse(Assign[s]));
                }

                foreach (var q in question1)
                {
                    if (!QID.Contains(q.QuestionID))
                    {
                        question.Remove(q);
                        if (Count == 0)
                        {
                            Count = assess.QuestionCount.Value - 1;
                        }
                        else
                        {
                            Count = Count - 1;
                        }
                    }
                }
                if (Count == 0)
                {
                    Count = assess.QuestionCount.Value;
                }
                var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
                var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();

                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();

                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                Listen Complex = new Listen();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.TimeQuestion = new List<double>();
                info.ItemID = item;
                info.Count = Convert.ToInt32(assess.Count);
                info.ItemType = Itemtype.TypeName;
                info.ItemType_CN = Itemtype.TypeName_CN;
                info.PartType = PartType.TypeName;
                info.QuestionCount = Convert.ToInt32(question.Count);
                info.ReplyTime = Convert.ToInt32(assess.Duration);
                info.Diffcult = Convert.ToDouble(assess.Difficult);
                info.Score = Convert.ToInt32(assess.Score);

                Complex.Script = assess.Original;
                Complex.SoundFile = item.ToString();

                string QkID = null;
                foreach (var qu in question)
                {
                    var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                    foreach (var q in QkList)
                    {
                        QkID += q + ",";
                    }
                }
                QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);
                    string name = string.Empty;
                    string id = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                    foreach (var Knowledge in KnowledgeList)
                    {
                        var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                        id += KnowledgeName.KnowledgePointID + ",";
                        name += KnowledgeName.Title + ",";
                    }
                    name = name.Substring(0, name.LastIndexOf(","));
                    id = id.Substring(0, id.LastIndexOf(','));
                    info.KnowledgeID.Add(id);
                    info.Knowledge.Add(name);
                }
                Complex.Info = info;
                return Complex;
            }
        }

        /// <summary>
        /// 查询当前点击的快速阅读信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的快速阅读信息</returns>
        SkimmingScanningPartCompletion ICedts_AssignRepository.SelectSspc(Guid item)
        {
            var Assigns = (from As in db.CEDTS_Assign where As.AssessmentItemID == item orderby As.Time descending select As.QuestionID).FirstOrDefault();
            if (Assigns == null)
            {
                return null;
            }
            else
            {
                string[] Assign = Assigns.Split(',');
                var question1 = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
                List<Guid> QID = new List<Guid>();
                int Count = 0;
                for (int s = 0; s < Assign.Length; s++)
                {
                    QID.Add(Guid.Parse(Assign[s]));
                }

                foreach (var q in question1)
                {
                    if (!QID.Contains(q.QuestionID))
                    {
                        question.Remove(q);
                        if (Count == 0)
                        {
                            Count = assess.QuestionCount.Value - 1;
                        }
                        else
                        {
                            Count = Count - 1;
                        }
                    }
                }
                if (Count == 0)
                {
                    Count = assess.QuestionCount.Value;
                }
                var ChoiceNum = (from m in db.CEDTS_Question where m.AssessmentItemID == item && m.ChooseA != "" orderby m.Order select m).ToList();
                var ChoiceNum1 = (from m in db.CEDTS_Question where m.AssessmentItemID == item && m.ChooseA != "" orderby m.Order select m).ToList();
                foreach (var c in ChoiceNum1)
                {
                    if (!QID.Contains(c.QuestionID))
                    {
                        ChoiceNum.Remove(c);
                    }
                }
                var TermNum = (from m in db.CEDTS_Question where m.AssessmentItemID == item && m.ChooseA == "" orderby m.Order select m).ToList();
                var TermNum1 = (from m in db.CEDTS_Question where m.AssessmentItemID == item && m.ChooseA == "" orderby m.Order select m).ToList();
                foreach (var t in TermNum1)
                {
                    if (!QID.Contains(t.QuestionID))
                    {
                        TermNum.Remove(t);
                    }
                 }
                var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
                var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();


                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                SkimmingScanningPartCompletion Sspc = new SkimmingScanningPartCompletion();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                Sspc.Choices = new List<string>();
                info.TimeQuestion = new List<double>();
                info.ItemID = item;
                info.Count = Convert.ToInt32(assess.Count);
                info.ItemType = Itemtype.TypeName;
                info.ItemType_CN = Itemtype.TypeName_CN;
                info.PartType = PartType.TypeName;
                info.QuestionCount = Convert.ToInt32(question.Count);
                info.ReplyTime = Convert.ToInt32(assess.Duration);
                info.Diffcult = Convert.ToDouble(assess.Difficult);
                info.Score = Convert.ToInt32(assess.Score);
                Sspc.ChoiceNum = ChoiceNum.Count;
                Sspc.TermNum = TermNum.Count;
                Sspc.Content = assess.Original;

                string QkID = null;
                foreach (var qu in question)
                {
                    var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                    foreach (var q in QkList)
                    {
                        QkID += q + ",";
                    }
                }
                QkID = QkID.Substring(0, QkID.LastIndexOf(","));
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

                foreach (var list in question)
                {

                    string name = string.Empty;
                    string id = string.Empty;

                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                    foreach (var Knowledge in KnowledgeList)
                    {
                        var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                        id += KnowledgeName.KnowledgePointID + ",";
                        name += KnowledgeName.Title + ",";
                    }
                    name = name.Substring(0, name.LastIndexOf(","));
                    id = id.Substring(0, id.LastIndexOf(','));
                    info.KnowledgeID.Add(id);
                    info.Knowledge.Add(name);
                }
                Sspc.Info = info;
                return Sspc;
            }
        }
        /// <summary>
        /// 查询当前点击的完型填空信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的完型填空信息</returns>
        ClozePart ICedts_AssignRepository.SelectCloze(Guid item)
        {
            var Assigns = (from As in db.CEDTS_Assign where As.AssessmentItemID == item orderby As.Time descending select As.QuestionID).FirstOrDefault();
            if (Assigns == null)
            {
                return null;
            }
            else
            {
                string[] Assign = Assigns.Split(',');
                var question1 = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
                List<Guid> QID = new List<Guid>();
                int Count = 0;
                for (int s = 0; s < Assign.Length; s++)
                {
                    QID.Add(Guid.Parse(Assign[s]));
                }

                foreach (var q in question1)
                {
                    if (!QID.Contains(q.QuestionID))
                    {
                        question.Remove(q);
                        if (Count == 0)
                        {
                            Count = assess.QuestionCount.Value - 1;
                        }
                        else
                        {
                            Count = Count - 1;
                        }
                    }
                }
                if (Count == 0)
                {
                    Count = assess.QuestionCount.Value;
                }
                var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
                var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();

                ItemBassInfo info = new ItemBassInfo();
                ClozePart Cloze = new ClozePart();
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();

                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                Cloze.Choices = new List<string>();
                info.TimeQuestion = new List<double>();
                info.ItemID = item;
                info.Count = Convert.ToInt32(assess.Count);
                info.ItemType = Itemtype.TypeName;
                info.ItemType_CN = Itemtype.TypeName_CN;
                info.PartType = PartType.TypeName;
                info.QuestionCount = Convert.ToInt32(question.Count);
                info.ReplyTime = Convert.ToInt32(assess.Duration);
                info.Diffcult = Convert.ToDouble(assess.Difficult);
                info.Score = Convert.ToInt32(assess.Score);

                Cloze.Content = assess.Original;

                string QkID = null;
                foreach (var qu in question)
                {
                    var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                    foreach (var q in QkList)
                    {
                        QkID += q + ",";
                    }
                }
                QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    Cloze.Choices.Add(list.ChooseA);
                    Cloze.Choices.Add(list.ChooseB);
                    Cloze.Choices.Add(list.ChooseC);
                    Cloze.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);
                    string name = string.Empty;
                    string id = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                    foreach (var Knowledge in KnowledgeList)
                    {
                        var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).FirstOrDefault();
                        id += KnowledgeName.KnowledgePointID + ",";
                        name += KnowledgeName.Title + ",";
                    }
                    name = name.Substring(0, name.LastIndexOf(","));
                    id = id.Substring(0, id.LastIndexOf(','));
                    info.KnowledgeID.Add(id);
                    info.Knowledge.Add(name);
                }
                Cloze.Info = info;
                return Cloze;
            }
        }
        /// <summary>
        /// 查询当前点的阅读理解-选择题型信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的阅读理解-选择题型信息</returns>
        ReadingPartOption ICedts_AssignRepository.SelectRpo(Guid item)
        {
            var Assigns = (from As in db.CEDTS_Assign where As.AssessmentItemID == item orderby As.Time descending select As.QuestionID).FirstOrDefault();
            if (Assigns == null)
            {
                return null;
            }
            else
            {
                string[] Assign = Assigns.Split(',');
                var question1 = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
                List<Guid> QID = new List<Guid>();
                int Count = 0;
                for (int s = 0; s < Assign.Length; s++)
                {
                    QID.Add(Guid.Parse(Assign[s]));
                }

                foreach (var q in question1)
                {
                    if (!QID.Contains(q.QuestionID))
                    {
                        question.Remove(q);
                        if (Count == 0)
                        {
                            Count = assess.QuestionCount.Value - 1;
                        }
                        else
                        {
                            Count = Count - 1;
                        }
                    }
                }
                if (Count == 0)
                {
                    Count = assess.QuestionCount.Value;
                }
                var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
                var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();



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
                info.ItemID = item;
                info.Count = Convert.ToInt32(assess.Count);
                info.ItemType = Itemtype.TypeName;
                info.ItemType_CN = Itemtype.TypeName_CN;
                info.PartType = PartType.TypeName;
                info.QuestionCount = Convert.ToInt32(question.Count);
                info.ReplyTime = Convert.ToInt32(assess.Duration);
                info.Diffcult = Convert.ToDouble(assess.Difficult);
                info.Score = Convert.ToInt32(assess.Score);

                Rpo.Content = assess.Original;

                string QkID = null;
                foreach (var qu in question)
                {
                    var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                    foreach (var q in QkList)
                    {
                        QkID += q + ",";
                    }
                }
                QkID = QkID.Substring(0, QkID.LastIndexOf(","));
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
                    string name = string.Empty;
                    string id = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                    foreach (var Knowledge in KnowledgeList)
                    {
                        var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                        id += KnowledgeName.KnowledgePointID + ",";
                        name += KnowledgeName.Title + ",";
                    }
                    name = name.Substring(0, name.LastIndexOf(","));
                    id = id.Substring(0, id.LastIndexOf(','));
                    info.KnowledgeID.Add(id);
                    info.Knowledge.Add(name);
                }
                Rpo.Info = info;
                return Rpo;
            }
        }
        /// <summary>
        /// 查询当前点击的阅读理解-选词填空信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的阅读理解-选词填空信息</returns>
        ReadingPartCompletion ICedts_AssignRepository.SelectRpc(Guid item)
        {
            var Assigns = (from As in db.CEDTS_Assign where As.AssessmentItemID == item orderby As.Time descending select As.QuestionID).FirstOrDefault();
            if (Assigns == null)
            {
                return null;
            }
            else
            {
                string[] Assign = Assigns.Split(',');
                var question1 = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
                List<Guid> QID = new List<Guid>();
                int Count = 0;
                for (int s = 0; s < Assign.Length; s++)
                {
                    QID.Add(Guid.Parse(Assign[s]));
                }

                foreach (var q in question1)
                {
                    if (!QID.Contains(q.QuestionID))
                    {
                        question.Remove(q);
                        if (Count == 0)
                        {
                            Count = assess.QuestionCount.Value - 1;
                        }
                        else
                        {
                            Count = Count - 1;
                        }
                    }
                }
                if (Count == 0)
                {
                    Count = assess.QuestionCount.Value;
                }
                var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
                var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();
                var Expansion = (from m in db.CEDTS_Expansion where m.AssessmentItemID == item select m).First();


                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.Problem = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                ReadingPartCompletion Rpc = new ReadingPartCompletion();
                info.Knowledge = new List<string>();
                info.KnowledgeID = new List<string>();
                info.TimeQuestion = new List<double>();
                Rpc.WordList = new List<string>();
                info.ItemID = item;
                info.Count = Convert.ToInt32(assess.Count);
                info.ItemType = Itemtype.TypeName;
                info.ItemType_CN = Itemtype.TypeName_CN;
                info.PartType = PartType.TypeName;
                info.QuestionCount = Convert.ToInt32(question.Count);
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
                foreach (var qu in question)
                {
                    var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                    foreach (var q in QkList)
                    {
                        QkID += q + ",";
                    }
                }
                QkID = QkID.Substring(0, QkID.LastIndexOf(","));
                info.QuestionKnowledgeID = QkID;

                foreach (var list in question)
                {
                    info.QuestionID.Add(list.QuestionID);
                    info.ScoreQuestion.Add(list.Score.Value);
                    info.TimeQuestion.Add(list.Duration.Value);
                    info.DifficultQuestion.Add(list.Difficult.Value);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);
                    string name = string.Empty;
                    string id = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                    foreach (var Knowledge in KnowledgeList)
                    {
                        var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                        id += KnowledgeName.KnowledgePointID + ",";
                        name += KnowledgeName.Title + ",";
                    }
                    name = name.Substring(0, name.LastIndexOf(","));
                    id = id.Substring(0, id.LastIndexOf(','));
                    info.KnowledgeID.Add(id);
                    info.Knowledge.Add(name);
                }
                Rpc.Info = info;
                return Rpc;
            }
        }
    }
}