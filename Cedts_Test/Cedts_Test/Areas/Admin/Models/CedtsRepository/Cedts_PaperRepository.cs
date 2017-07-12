using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cedts_Test.Areas.Admin.Models;
using System.Text;
using Webdiyer.WebControls.Mvc;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_PaperRepository : ICedts_PaperRepository
    {
        #region 实例化Cedts_Entities
        Cedts_Entities db;
        public Cedts_PaperRepository()
        {
            db = new Cedts_Entities();
        }
        #endregion

        PagedList<CEDTS_Paper> ICedts_PaperRepository.SelectPaper(int? id, int userid)
        {
            int defaultPageSize = 10;
            IQueryable<CEDTS_Paper> paperList;
            if (userid == 1)
            {
                paperList = (from m in db.CEDTS_Paper where m.UserID == 1 && m.State == 1 orderby m.CreateTime descending select m);
            }
            else
            {
                paperList = (from m in db.CEDTS_Paper where m.UserID == userid && (m.State == 9 || m.State == 2 ) orderby m.CreateTime descending select m);
            }
            
            PagedList<CEDTS_Paper> Paper = paperList.ToPagedList(id ?? 1, defaultPageSize);
            return Paper;
        }

        #region 根据试卷ID获取试卷
        CEDTS_Paper ICedts_PaperRepository.GetPaperByID(Guid PaperID)
        {
            return db.CEDTS_Paper.Where(m => m.PaperID == PaperID).FirstOrDefault();
        }
        #endregion

        #region 根据试卷ID获取测试对象
        CEDTS_Test ICedts_PaperRepository.GetTestByPaperID(Guid PaperID, int UserID)
        {
            return db.CEDTS_Test.Where(m => m.PaperID == PaperID && m.UserID == UserID).OrderBy(m => m.FinishDate).FirstOrDefault();
        }
        #endregion

        #region 作业分数列表分页
        PagedList<ScoreInfo> ICedts_PaperRepository.ScoreInfoPaged(int? id, List<ScoreInfo> ScoreInfo)
        {
            IQueryable<ScoreInfo> IScoreInfo = ScoreInfo.AsQueryable();
            PagedList<ScoreInfo> Score = IScoreInfo.ToPagedList(id ?? 1, 10);
            return Score;
        }
        #endregion

        #region 作业分数列表分页
        PagedList<SingleScoreInfo> ICedts_PaperRepository.SingleScoreInfoPaged(int? id, List<SingleScoreInfo> SingleScoreInfo)
        {
            IQueryable<SingleScoreInfo> ISingleScoreInfo = SingleScoreInfo.AsQueryable();
            PagedList<SingleScoreInfo> Score = ISingleScoreInfo.ToPagedList(id ?? 1, 10);
            return Score;
        }
        #endregion

        #region 自主出题选题列表分页
        PagedList<ExaminationItem> ICedts_PaperRepository.AssessmentItemPaged(int? id, List<ExaminationItem> AssessmentItemList)
        {
            IQueryable<ExaminationItem> IAssessmentItemList = AssessmentItemList.AsQueryable();
            return IAssessmentItemList.ToPagedList(id ?? 1, 10);
        }
        #endregion

        #region 根据时间和用户获取试卷列表
        List<Guid?> ICedts_PaperRepository.SelectPaperByPeriod(string StartTime, string EndTime, int UserID, Guid ClassID)
        {
            DateTime bdate = Convert.ToDateTime(StartTime);
            DateTime edate = Convert.ToDateTime(EndTime).AddDays(1);
            return db.CEDTS_PaperAssignClass.Where(m => m.ClassID == ClassID && m.UserID == UserID && m.CreateTime >= bdate && m.CreateTime <= edate).OrderByDescending(m => m.CreateTime).Select(m => m.PaperID).ToList();
        }
        #endregion

        #region 根据时间和用户获取测试列表
        List<CEDTS_Test> ICedts_PaperRepository.SelectTestByPeriod(string StartTime, string EndTime, int UserID)
        {
            DateTime bdate = Convert.ToDateTime(StartTime);
            DateTime edate = Convert.ToDateTime(EndTime).AddDays(1);
            return db.CEDTS_Test.Where(m => m.UserID == UserID && m.IsFinished == true && m.FinishDate >= bdate && m.FinishDate <= edate).ToList();
        }
        #endregion

        #region 根据时间和用户获取测试数量
        int ICedts_PaperRepository.CountTestByPeriod(string StartTime, string EndTime, int UserID)
        {
            DateTime bdate = Convert.ToDateTime(StartTime);
            DateTime edate = Convert.ToDateTime(EndTime).AddDays(1);
            return db.CEDTS_Test.Where(m => m.UserID == UserID && m.IsFinished == true && m.FinishDate >= bdate && m.FinishDate <= edate).Count();
        }
        #endregion

        #region 根据时间、用户和用户获取测试知识点列表
        List<CEDTS_TestAnswerKnowledgePoint> ICedts_PaperRepository.SelectTAKByPeriod(string StartTime, string EndTime, int UserID, Guid KnowledgeID)
        {
            DateTime bdate = Convert.ToDateTime(StartTime);
            DateTime edate = Convert.ToDateTime(EndTime).AddDays(1);
            return db.CEDTS_TestAnswerKnowledgePoint.Where(m => m.UserID == UserID && m.Time >= bdate && m.Time <= edate && m.KnowledgePointID == KnowledgeID).ToList();
        }
        #endregion

        #region 根据时间和知识点获取测试知识点列表
        List<CEDTS_TestAnswerKnowledgePoint> ICedts_PaperRepository.SelectATAKByPeriod(string StartTime, string EndTime, Guid KnowledgeID)
        {
            DateTime bdate = Convert.ToDateTime(StartTime);
            DateTime edate = Convert.ToDateTime(EndTime).AddDays(1);
            return db.CEDTS_TestAnswerKnowledgePoint.Where(m => m.Time >= bdate && m.Time <= edate && m.KnowledgePointID == KnowledgeID).ToList();
        }
        #endregion

        #region 根据时间、用户和题型获取测试题型列表
        List<CEDTS_TestAnswerTypeInfo> ICedts_PaperRepository.SelectTATByPeriod(string StartTime, string EndTime, int UserID, int TypeID)
        {
            DateTime bdate = Convert.ToDateTime(StartTime);
            DateTime edate = Convert.ToDateTime(EndTime).AddDays(1);
            return db.CEDTS_TestAnswerTypeInfo.Where(m => m.UserID == UserID && m.Time >= bdate && m.Time <= edate && m.ItemTypeID == TypeID).ToList();
        }
        #endregion

        #region 根据时间和题型获取测试题型列表
        List<CEDTS_TestAnswerTypeInfo> ICedts_PaperRepository.SelectATATByPeriod(string StartTime, string EndTime, int TypeID)
        {
            DateTime bdate = Convert.ToDateTime(StartTime);
            DateTime edate = Convert.ToDateTime(EndTime).AddDays(1);
            return db.CEDTS_TestAnswerTypeInfo.Where(m => m.Time >= bdate && m.Time <= edate && m.ItemTypeID == TypeID).ToList();
        }
        #endregion

        #region 获取所有老师自主出题列表
        List<ExaminationItem> ICedts_PaperRepository.GetExaminationItemsByTeacher()
        {
            IQueryable<ExaminationItem> p = from m in db.CEDTS_Paper
                                            from n in db.CEDTS_PaperAssessment
                                            from l in db.CEDTS_AssessmentItem
                                            from o in db.CEDTS_User
                                            where m.State == 9 && n.PaperID == m.PaperID && l.AssessmentItemID == n.AssessmentItemID && l.UserID == o.UserID
                                            select new ExaminationItem
                                            {
                                                AssessmentItemID = l.AssessmentItemID,
                                                PaperName = m.Title,
                                                Username = o.UserNickname,
                                                Difficult = l.Difficult.Value,
                                                Duration = l.Duration.Value,
                                                Score = l.Score.Value,
                                                SaveTime = l.SaveTime.Value,
                                                ItemID = l.ItemTypeID.Value
                                            };
            return p.ToList();
        }
        #endregion

        #region 根据TestID获取小题数
        int ICedts_PaperRepository.SelectQNByTestID(Guid TestID)
        {
            return db.CEDTS_TestAnswer.Where(m => m.TestID == TestID).Count();
        }
        #endregion

        #region 根据TestID获取正确小题数
        int ICedts_PaperRepository.SelectCQNByTestID(Guid TestID)
        {
            return db.CEDTS_TestAnswer.Where(m => m.TestID == TestID && m.IsRight == true).Count();
        }
        #endregion

        #region 查询选词填空选项
        string ICedts_PaperRepository.AnswerValue(Guid id, string value)
        {
            var ValueInfo = (from m in db.CEDTS_Expansion where m.AssessmentItemID == id select m).FirstOrDefault();
            string ValueData = string.Empty;
            switch (value)
            {
                case "A":
                    ValueData = ValueInfo.ChoiceA;
                    break;
                case "B":
                    ValueData = ValueInfo.ChoiceB;
                    break;
                case "C":
                    ValueData = ValueInfo.ChoiceC;
                    break;
                case "D":
                    ValueData = ValueInfo.ChoiceD;
                    break;
                case "E":
                    ValueData = ValueInfo.ChoiceE;
                    break;
                case "F":
                    ValueData = ValueInfo.ChoiceF;
                    break;
                case "G":
                    ValueData = ValueInfo.ChoiceG;
                    break;
                case "H":
                    ValueData = ValueInfo.ChoiceH;
                    break;
                case "I":
                    ValueData = ValueInfo.ChoiceI;
                    break;
                case "J":
                    ValueData = ValueInfo.ChoiceJ;
                    break;
                case "K":
                    ValueData = ValueInfo.ChoiceK;
                    break;
                case "L":
                    ValueData = ValueInfo.ChoiceL;
                    break;
                case "M":
                    ValueData = ValueInfo.ChoiceM;
                    break;
                case "N":
                    ValueData = ValueInfo.ChoiceN;
                    break;
                case "O":
                    ValueData = ValueInfo.ChoiceO;
                    break;
                default:
                    break;
            }
            return ValueData;
        }
        #endregion

        List<CEDTS_PartType> ICedts_PaperRepository.SelectPartType()
        {
            return db.CEDTS_PartType.ToList();
        }

        List<CEDTS_ItemType> ICedts_PaperRepository.SelectItemType()
        {
            return db.CEDTS_ItemType.ToList();
        }

        List<CEDTS_ItemType> ICedts_PaperRepository.SelectItemTypeByPartTypeID(int PartTypeID)
        {
            return db.CEDTS_ItemType.Where(p => p.PartTypeID == PartTypeID).ToList();
        }

        #region 根据AssessmentID获取题目数
        int ICedts_PaperRepository.SelectQNByAssessmentID(Guid AssessmentID)
        {
            return db.CEDTS_Question.Where(p => p.AssessmentItemID == AssessmentID).Count();
        }
        #endregion

        #region 获取试卷中所有的试题信息
        List<CEDTS_AssessmentItem> ICedts_PaperRepository.SelectAssessmentItems(List<string> itemList, int userID)
        {
            List<Guid> totalList = new List<Guid>();
            

            for (int k = 0; k < itemList.Count; k++)
            {
                bool checkMeet = false;//判断是否选够了用户选择的题目数量
                if (int.Parse(itemList[k]) != 0)
                {
                    //降序保存做过的题目id
                    List<Guid> guidList = (from m in db.CEDTS_UserAssessmentCount
                                           from n in db.CEDTS_AssessmentItem
                                           where m.UserID == userID && m.AssessmentItemID == n.AssessmentItemID && n.ItemTypeID == k + 1
                                           orderby m.Count
                                           select m.AssessmentItemID).ToList();
                    List<Guid> list = new List<Guid>();
                    //k+1对应itemtype ，因为typeid从0开始
                    //找出所有这个itemtype下的itemid
                    //List<Guid> guidList1 = db.CEDTS_AssessmentItem.Where(p => (p.ItemTypeID == (k + 1))&&(p.UserID == 1)).Select(p => p.AssessmentItemID).ToList();
                    List<Guid> guidList1 = (from m in db.CEDTS_AssessmentItem where m.ItemTypeID == (k + 1) && m.UserID == 1 select m.AssessmentItemID).ToList();
                    for (int i = 0; i < guidList1.Count; i++)
                    {
                        //不在以做过的题目里面，就保存
                        if (!guidList.Contains(guidList1[i]))
                        {
                            list.Add(guidList1[i]);
                            if (list.Count == int.Parse(itemList[k]))
                            {
                                checkMeet = true;
                                break;
                            }
                        }
                    }

                    if (!checkMeet)                   
                    {
                        var tempList = guidList;
                        tempList.RemoveRange(0, list.Count);
                        list.AddRange(tempList);
                    }
                    totalList.AddRange(list);
                }
            }

            List<CEDTS_AssessmentItem> AssessmentItemList = new List<CEDTS_AssessmentItem>();
            for (int m = 0; m < totalList.Count; m++)
            {
                var ttemp = totalList[m];
                AssessmentItemList.Add(db.CEDTS_AssessmentItem.Where(p => p.AssessmentItemID == ttemp).FirstOrDefault());
            }
            return AssessmentItemList;
        }
        #endregion

        #region 获取试卷中所有的试题信息（根据知识点）
        List<CEDTS_AssessmentItem> ICedts_PaperRepository.SelectAssessmentItems2(List<string> knowList, List<string> itemList, int userID)
        {
            List<Guid> knowIDList = new List<Guid>();
            List<int> countList = new List<int>();
            for (int i = 0; i < knowList.Count; i++)
            {
                knowIDList.Add(Guid.Parse(knowList[i]));
                countList.Add(int.Parse(itemList[i]));
            }

            #region 选取用户出过的小题
            List<Guid> uaList = db.CEDTS_UserAssessmentCount.Where(p => p.UserID == userID).Select(p => p.AssessmentItemID).Distinct().ToList();
            List<Guid> uqList = new List<Guid>();
            List<Guid> alluqList = new List<Guid>();
            foreach (var ua in uaList)
            {
                List<Guid> tempqList = db.CEDTS_Question.Where(p => p.AssessmentItemID == ua).Select(p => p.QuestionID).ToList();
                foreach (var tempq in tempqList)
                {
                    if (!uqList.Contains(tempq))
                    {
                        uqList.Add(tempq);
                    }
                    alluqList.Add(tempq);
                }
            }
            #endregion

            #region 选出所有包含所选知识点的待选小题Q
            List<CEDTS_Question> allaqList = new List<CEDTS_Question>();
            List<Guid> allaqIDList = new List<Guid>();
            for (int i = 0; i < knowList.Count; i++)
            {
                Guid knowID = Guid.Parse(knowList[i]);
                List<Guid> aqList = db.CEDTS_QuestionKnowledge.Where(p => p.KnowledgePointID == knowID).Select(p => p.QuestionID).Distinct().ToList();
                foreach (var q in aqList)
                {
                    var tempq = db.CEDTS_Question.Where(p => p.QuestionID == q).FirstOrDefault();
                    if (!uqList.Contains(q) && !allaqList.Contains(tempq))
                    {
                        allaqList.Add(tempq);
                    }
                    allaqIDList.Add(q);
                }
            }
            #endregion

            #region 排序Q
            List<Guid> allaList = new List<Guid>();
            foreach (Guid id in allaqIDList)
            {
                var item = db.CEDTS_Question.Where(p => p.QuestionID == id).Select(p => p.AssessmentItemID).FirstOrDefault();
                allaList.Add(item);
            }
            var sortaList = from x in allaList group x by x into y select new { y.Key, count = y.Count() };
            sortaList = sortaList.OrderByDescending(p => p.count).ToList();
            List<Guid> sortaIDList = sortaList.Select(p => p.Key).ToList();
            List<CEDTS_Question> sortqList = new List<CEDTS_Question>();
            foreach (var id in sortaIDList)
            {
                sortqList.AddRange(allaqList.Where(p => p.AssessmentItemID == id).ToList());
            }
            #endregion

            bool enough = false;//判断是否选满了用户设定的数量

            #region 抽取Q
            List<CEDTS_Question> QList = new List<CEDTS_Question>();
            foreach (var Q in sortqList)
            {

                Guid id = Q.QuestionID;
                List<Guid> kList = db.CEDTS_QuestionKnowledge.Where(p => p.QuestionID == id).Select(p => p.KnowledgePointID).ToList();
                foreach (var k in kList)
                {
                    for (int i = 0; i < knowIDList.Count; i++)
                    {
                        if (k == knowIDList[i])
                        {
                            if (countList[i] > 0)
                            {
                                countList[i]--;
                                QList.Add(Q);
                                break;
                            }
                            else
                                continue;
                        }
                    }
                }
                int index = 0;
                foreach (var c in countList)
                {
                    index += c;
                }
                if (index == 0)
                {
                    enough = true;
                    break;
                }
            }
            if (!enough)
            {
                var sortuqList = from x in alluqList group x by x into y select new { y.Key, count = y.Count() };
                sortuqList = sortaList.OrderBy(p => p.count).ToList();
                var uqList2 = sortuqList.Select(p => p.Key).ToList();
                List<Guid> realUqList = new List<Guid>();
                foreach (var uq in uqList2)
                {
                    if (allaqIDList.Contains(uq))
                    {
                        realUqList.Add(uq);
                    }
                }
                foreach (var Q in realUqList)
                {
                    var tempQ = db.CEDTS_Question.Where(p => p.QuestionID == Q).FirstOrDefault();
                    if (QList.Contains(tempQ))
                    {
                        continue;
                    }

                    List<Guid> kList = db.CEDTS_QuestionKnowledge.Where(p => p.QuestionID == Q).Select(p => p.KnowledgePointID).ToList();
                    foreach (var k in kList)
                    {
                        for (int i = 0; i < knowIDList.Count; i++)
                        {
                            if (k == knowIDList[i])
                            {
                                if (countList[i] > 0)
                                {
                                    countList[i]--;
                                    QList.Add(tempQ);
                                    break;
                                }
                                else
                                    continue;
                            }
                        }
                    }
                    int index = 0;
                    foreach (var c in countList)
                    {
                        index += c;
                    }
                    if (index == 0)
                    {
                        break;
                    }
                }
            }
            #endregion

            #region 获取List<CEDTS_AssessmentItem>
            List<CEDTS_AssessmentItem> AList = new List<CEDTS_AssessmentItem>();
            var tempAIDList = QList.Select(p => p.AssessmentItemID).Distinct().ToList();
            foreach (var aid in tempAIDList)
            {
                AList.Add(db.CEDTS_AssessmentItem.Where(p => p.AssessmentItemID == aid).FirstOrDefault());
            }
            #endregion

            return AList;
        }
        #endregion

        #region 根据等级获取知识点
        List<CEDTS_KnowledgePoints> ICedts_PaperRepository.SelectKnowledges(int level, Guid? uperID)
        {
            if (uperID == null)

                return db.CEDTS_KnowledgePoints.Where(p => p.Level == level).OrderBy(p => p.Title).ToList();
            else
                return db.CEDTS_KnowledgePoints.Where(p => p.Level == level && p.UperKnowledgeID == uperID).OrderBy(p => p.Title).ToList();
        }
        #endregion

        #region 根据知识点id号获取知识点名
        string ICedts_PaperRepository.SelectKnowledgeName(Guid KnowledgeID)
        {
            string name = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == KnowledgeID select m.Title).FirstOrDefault();
            Regex regex = new Regex(@"[.\d]");
            string KpName = regex.Replace(name, "");
            return KpName;
        }
        #endregion

        #region 获取所有知识点
        List<CEDTS_KnowledgePoints> ICedts_PaperRepository.GetAllKnowledges()
        {
            return db.CEDTS_KnowledgePoints.OrderBy(m => m.Title).ToList();
        }
        #endregion

        #region 根据知识点获取小问题

        List<CEDTS_Question> ICedts_PaperRepository.SelectQuestionByknowledge(Guid id, List<Guid> knowledge)
        {
            var allQuestion = db.CEDTS_Question.Where(p => p.AssessmentItemID == id).OrderBy(p => p.Order).ToList();
            List<CEDTS_Question> tempList = new List<CEDTS_Question>();
            for (int i = 0; i < allQuestion.Count; i++)
            {
                Guid tempQID = allQuestion[i].QuestionID;
                List<Guid> tempKID = db.CEDTS_QuestionKnowledge.Where(p => p.QuestionID == tempQID).Select(p => p.KnowledgePointID).ToList();
                bool istrue = false;
                for (int j = 0; j < tempKID.Count; j++)
                {
                    if (knowledge.Contains(tempKID[j]))
                    {
                        istrue = true;
                    }
                }
                if (istrue)
                {
                    tempList.Add(allQuestion[i]);
                }
            }
            return tempList;
        }
        #endregion

        void ICedts_PaperRepository.SavePaper(CEDTS_Paper paper)
        {
            db.AddToCEDTS_Paper(paper);
            db.SaveChanges();
        }

        void ICedts_PaperRepository.SavePaperAssignClass(CEDTS_PaperAssignClass assignrecord)
        {
            db.AddToCEDTS_PaperAssignClass(assignrecord);
            db.SaveChanges();

        }

        void ICedts_PaperRepository.SavePaperAssessment(CEDTS_PaperAssessment pa)
        {
            db.AddToCEDTS_PaperAssessment(pa);
            db.SaveChanges();
        }

        void ICedts_PaperRepository.SavePaperExpansion(CEDTS_PaperExpansion PaperExpansion)
        {
            db.AddToCEDTS_PaperExpansion(PaperExpansion);
            db.SaveChanges();
        }

        CEDTS_Paper ICedts_PaperRepository.SelectEditPaper(Guid id)
        {
            var PaperInfo = (from m in db.CEDTS_Paper where m.PaperID == id select m).First();
            return PaperInfo;
        }

        CEDTS_PaperExpansion ICedts_PaperRepository.SelectEditPaperExpansion(Guid id)
        {
            var Expansion = (from m in db.CEDTS_PaperExpansion where m.PaperID == id select m).FirstOrDefault();
            return Expansion;
        }

        #region 用户题型正确率
        ItemByTeacherScale ICedts_PaperRepository.UserItemInfo(int UserID, Guid TestID)
        {
            ItemByTeacherScale ItemByTeacher = new ItemByTeacherScale();
            var ItemList = (from m in db.CEDTS_ItemType orderby m.ItemTypeID select m);
            Guid PaperID = (from m in db.CEDTS_Test where m.TestID == TestID && m.UserID == UserID select m.PaperID).FirstOrDefault();
            double TotalScore = (from m in db.CEDTS_Paper where m.PaperID == PaperID select m.Score.Value).FirstOrDefault();
            var AssessmentList = (from m in db.CEDTS_PaperAssessment where m.PaperID == PaperID orderby m.ItemTypeID select m);
            List<double> ItemScore = new List<double>();
            List<string> ItemName = new List<string>();
            List<double> UScale = new List<double>();
            foreach (var item in ItemList)
            {
                string name = item.TypeName_CN;
                ItemName.Add(name);
                ItemScore.Add(0);
            }
            foreach (var Assessment in AssessmentList)
            {
                var AssessmentItem = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == Assessment.AssessmentItemID select m).FirstOrDefault();
                ItemScore[AssessmentItem.ItemTypeID.Value - 1] += AssessmentItem.Score.Value;
            }
            
            var UItem = (from m in db.CEDTS_TestAnswerTypeInfo where m.UserID == UserID && m.TestID == TestID orderby m.ItemTypeID select m);

            foreach (var u in UItem)
            {
                double u1 = 0.0;
                switch (u.ItemTypeID)
                {
                    case 1:
                        u1 = u.CorrectRate.Value * 1 * ItemScore[0] / TotalScore * 100;
                        break;
                    case 2:
                        u1 = u.CorrectRate.Value * 1 * ItemScore[1] / TotalScore * 100;
                        break;
                    case 3:
                        u1 = u.CorrectRate.Value * 1 * ItemScore[2] / TotalScore * 100;
                        break;
                    case 4:
                        u1 = u.CorrectRate.Value * 1 * ItemScore[3] / TotalScore * 100;
                        break;
                    case 5:
                        u1 = u.CorrectRate.Value * 1 * ItemScore[4] / TotalScore * 100;
                        break;
                    case 6:
                        u1 = u.CorrectRate.Value * 1 * ItemScore[5] / TotalScore * 100;
                        break;
                    case 7:
                        u1 = u.CorrectRate.Value * 1.5 * ItemScore[6] / TotalScore * 100;
                        break;
                    case 8:
                        u1 = u.CorrectRate.Value * 0.5 * ItemScore[7] / TotalScore * 100;
                        break;
                    case 9:
                        u1 = u.CorrectRate.Value * 1 * ItemScore[8] / TotalScore * 100;
                        break;
                    default:
                        break;
                }
                string uu = u1 + "";
                if (uu.Length > 4)
                {
                    uu = uu.Substring(0, uu.IndexOf('.') + 3);
                }             
                UScale.Add(double.Parse(uu));               
            }
            for (int i = 0; i < ItemScore.Count; i++)
            {
                if (ItemScore[i] == 0)
                {
                    ItemScore.RemoveAt(i);
                    ItemName.RemoveAt(i);
                }
                else
                {
                    ItemScore[i] = ItemScore[i] / TotalScore * 100;
                    string temp = ItemScore[i] + "";
                    if (temp.Length > 4)
                    {
                        temp = temp.Substring(0, temp.IndexOf('.') + 3);
                    }
                    ItemScore[i] = double.Parse(temp);
                }
            }
            ItemByTeacher.UScale = UScale;
            ItemByTeacher.ItemName = ItemName;
            ItemByTeacher.Fortysix = ItemScore;
            return ItemByTeacher;
        }
        #endregion

        #region 题型列表
        ItemInfo ICedts_PaperRepository.ItemList(int UserID, Guid TestID)
        {
            ItemInfo Item = new ItemInfo();
            Item.CorrectRate = new List<double>();
            Item.ItemName = new List<string>();
            Item.Num = new List<int>();
            var info = from m in db.CEDTS_TestAnswerTypeInfo
                       where m.UserID == UserID && m.TestID == TestID
                       from n in db.CEDTS_ItemType
                       where n.ItemTypeID == m.ItemTypeID
                       select new
                       {
                           Name = n.TypeName_CN,
                           Num = m.TotalItemNumber,
                           CorrectRate = m.CorrectItemNumber
                       };
            foreach (var s in info)
            {
                Item.ItemName.Add(s.Name);
                Item.CorrectRate.Add(s.CorrectRate.Value);
                Item.Num.Add(s.Num.Value);
            }
            return Item;
        }
        #endregion

        #region 题型中文列表
        List<CEDTS_ItemType> ICedts_PaperRepository.ItemCNList()
        {
            return db.CEDTS_ItemType.ToList(); ;
        }
        #endregion

        #region 根据用户和题型号获取题型数
        int ICedts_PaperRepository.ItemTestCount(int UserID, Guid TestID,int ItemTypeID)
        {
            return db.CEDTS_TestAnswer.Where(p => p.UserID == UserID && p.TestID == TestID&& p.ItemTypeID == ItemTypeID).Count();
        }
        #endregion

        #region 根据用户和题型号获取正确题型数
        int ICedts_PaperRepository.ItemTestCorCount(int UserID, Guid TestID, int ItemTypeID)
        {
            return db.CEDTS_TestAnswer.Where(p => p.UserID == UserID && p.TestID == TestID && p.ItemTypeID == ItemTypeID && p.IsRight == true).Count();
        }
        #endregion

        #region 根据用户和题型号获取知识点数
        int ICedts_PaperRepository.KnowledgeTestCount(int UserID, Guid TestID, Guid KnowledgePointID)
        {
            var answerlist = db.CEDTS_TestAnswer.Where(p => p.UserID == UserID && p.TestID == TestID).ToList();
            int count = 0;
            foreach (var item in answerlist)
            {
                count += db.CEDTS_QuestionKnowledge.Where(p => p.QuestionID == item.QuestionID && p.KnowledgePointID == KnowledgePointID).Count();

            }
            return count;
        }
        #endregion
        
        #region 根据用户和题型号获取正确知识点数
        int ICedts_PaperRepository.KnowledgeTestCorCount(int UserID, Guid TestID, Guid KnowledgePointID)
        {
            var answerlist = db.CEDTS_TestAnswer.Where(p => p.UserID == UserID && p.TestID == TestID && p.IsRight==true).ToList();
            int count = 0;
            foreach(var item in answerlist)
            {
                count += db.CEDTS_QuestionKnowledge.Where(p => p.QuestionID == item.QuestionID && p.KnowledgePointID == KnowledgePointID).Count();

            }
            return count;
        }
        #endregion


        #region 用户知识点掌握率
        List<Knowledge> ICedts_PaperRepository.UserKpMaster(int UserID, Guid TestID)
        {
            List<Knowledge> KnowList = new List<Knowledge>();
            var KpName = (from m in db.CEDTS_KnowledgePoints orderby m.Title select m);
            foreach (var kp in KpName)
            {
                string Name = string.Empty;
                Knowledge kown = new Knowledge();
                Regex regex = new Regex(@"[.\d]");
                Name = regex.Replace(kp.Title, "");
                var SMiInfo = (from m in db.CEDTS_TestAnswerKnowledgePoint where m.KnowledgePointID == kp.KnowledgePointID && m.UserID == UserID && m.TestID == TestID select m.KP_MasterRate).FirstOrDefault();
                var UMiInfo = (from m in db.CEDTS_UserKnowledgeInfomation where m.UserID == UserID && m.KnowledgePointID == kp.KnowledgePointID select m.KP_MasterRate).FirstOrDefault();
                if (SMiInfo != null)
                {
                    double SMi1 = SMiInfo.Value * 100;
                    double UMi1 = UMiInfo.Value * 100;
                    string SMi = SMi1 + "";
                    string UMi = UMi1 + "";
                    if (SMi.Length > 4)
                    {
                        SMi = SMi.Substring(0, SMi.IndexOf('.') + 2);
                    }
                    if (UMi.Length > 4)
                    {
                        UMi = UMi.Substring(0, UMi.IndexOf('.') + 2);
                    }
                    kown.KPName = Name;
                    kown.SMi = double.Parse(SMi);
                    kown.UMi = double.Parse(UMi);
                    KnowList.Add(kown);
                }
            }
            return KnowList;
        }
        #endregion

        #region 知识点列表
        UserKnowledgeInfo ICedts_PaperRepository.KpList(int UserID, Guid TestID)
        {
            UserKnowledgeInfo UserKpInfo = new UserKnowledgeInfo();
            UserKpInfo.CorrectRate = new List<double>();
            UserKpInfo.KPMaster = new List<double>();
            UserKpInfo.KpName = new List<string>();
            UserKpInfo.Num = new List<int>();
            UserKpInfo.Time = new List<int>();

            var KpInfo = from m in db.CEDTS_TestAnswerKnowledgePoint
                         where m.TestID == TestID && m.UserID == UserID
                         from n in db.CEDTS_KnowledgePoints
                         where m.KnowledgePointID == n.KnowledgePointID
                         orderby m.KP_MasterRate
                         select new
                         {
                             KpName = n.Title,
                             Num = m.TotalItemNumber,
                             CorrectRate = m.CorrectRate,
                             KpMaster = m.KP_MasterRate
                         };
            foreach (var s in KpInfo)
            {
                Regex regex = new Regex(@"[.\d]");
                string KpName = regex.Replace(s.KpName, "");
                UserKpInfo.KpName.Add(KpName);
                string CorrectRate = s.CorrectRate * 100 + "";
                if (CorrectRate.Length > 4)
                {
                    CorrectRate = CorrectRate.Substring(0, CorrectRate.IndexOf('.') + 1);
                }
                UserKpInfo.CorrectRate.Add(double.Parse(CorrectRate));
                string KpMaster = s.KpMaster * 100 + "";
                if (KpMaster.Length > 4)
                {
                    KpMaster = KpMaster.Substring(0, KpMaster.IndexOf('.') + 1);
                }
                UserKpInfo.KPMaster.Add(double.Parse(KpMaster));
                UserKpInfo.Num.Add(s.Num.Value);

            }
            return UserKpInfo;
        }
        #endregion

        #region 根据PaperID StudIDList获取TestAnswerTypeInfo列表
        List<WrongItemInfo> ICedts_PaperRepository.SelectWIIByPUList(Guid PaperID, List<int> StudIDList)
        {
            List<WrongItemInfo> WrongItemList = new List<WrongItemInfo>();
            var TATIList = from m in db.CEDTS_TestAnswerTypeInfo
                           where m.PaperID == PaperID && StudIDList.Contains(m.UserID.Value)
                           group m by m.ItemTypeID into n
                           select new
                           {
                               ItemTypeNum = n.Key,
                               AveCorrectRate = n.Average(m => m.CorrectRate),
                               TotalItemNum = n.Average(m => m.TotalItemNumber)
                           };
            foreach (var tati in TATIList)
            {
                WrongItemInfo WrongItem = new WrongItemInfo();
                WrongItem.ItemTypeNum = tati.ItemTypeNum.Value;
                WrongItem.CorrectRate = tati.AveCorrectRate.Value;
                WrongItem.TotalNum = (int)tati.TotalItemNum.Value;
                WrongItemList.Add(WrongItem);
            }
            return WrongItemList;
        }
        #endregion

        #region 根据PaperIDList StudIDList获取TestAnswerTypeInfo列表
        List<WrongItemInfo> ICedts_PaperRepository.SelectWIIByPUList(List<Guid> PaperIDList, List<int> StudIDList)
        {
            List<WrongItemInfo> WrongItemList = new List<WrongItemInfo>();
            var TATIList = from m in db.CEDTS_TestAnswerTypeInfo
                           where PaperIDList.Contains(m.PaperID) && StudIDList.Contains(m.UserID.Value)
                           group m by m.ItemTypeID into n
                           select new
                           {
                               ItemTypeNum = n.Key,
                               AveCorrectRate = n.Average(m => m.CorrectRate),
                           };
            foreach (var tati in TATIList)
            {
                WrongItemInfo WrongItem = new WrongItemInfo();
                WrongItem.ItemTypeNum = tati.ItemTypeNum.Value;
                WrongItem.ItemName = (from m in db.CEDTS_ItemType where m.ItemTypeID == tati.ItemTypeNum select m.TypeName_CN).FirstOrDefault();
                WrongItem.CorrectRate = tati.AveCorrectRate.Value;
                WrongItemList.Add(WrongItem);
            }
            foreach (var item in WrongItemList)
            {
                item.TotalNum = (from m in db.CEDTS_PaperAssessment
                                 where PaperIDList.Contains(m.PaperID.Value) && m.ItemTypeID == item.ItemTypeNum
                                 from n in db.CEDTS_Question
                                 where n.AssessmentItemID == m.AssessmentItemID
                                 select n).Count();
            }
            return WrongItemList;
        }
        #endregion

        #region 根据PaperID StudIDList获取WrongKnowledgeInfo列表
        List<WrongKnowledgeInfo> ICedts_PaperRepository.SelectWKIByPUList(Guid PaperID, List<int> StudIDList)
        {
            List<WrongKnowledgeInfo> WrongKnowledgeList = new List<WrongKnowledgeInfo>();
            var WKIList = from m in db.CEDTS_TestAnswerKnowledgePoint
                           where m.PaperID == PaperID && StudIDList.Contains(m.UserID.Value)
                           group m by m.KnowledgePointID into n
                           select new
                           {
                               KnowledgeID = n.Key,
                               AveCorrectRate = n.Average(m => m.CorrectRate),
                               TotalItemNum = n.Average(m => m.TotalItemNumber)
                           };
            WKIList = from m in WKIList orderby m.AveCorrectRate select m;
            foreach (var wki in WKIList)
            {
                WrongKnowledgeInfo WrongKnowledge = new WrongKnowledgeInfo();
                WrongKnowledge.KnowledgeID = wki.KnowledgeID;
                WrongKnowledge.CorrectRate = wki.AveCorrectRate.Value;
                WrongKnowledge.TotalNum = (int)wki.TotalItemNum.Value;
                WrongKnowledgeList.Add(WrongKnowledge);
            }
            return WrongKnowledgeList;
        }
        #endregion

        #region 根据PaperIDList StudIDList获取WrongKnowledgeInfo列表
        List<WrongKnowledgeInfo> ICedts_PaperRepository.SelectWKIByPUList(List<Guid> PaperIDList, List<int> StudIDList)
        {
            List<WrongKnowledgeInfo> WrongKnowledgeList = new List<WrongKnowledgeInfo>();
            var WKIList = from m in db.CEDTS_TestAnswerKnowledgePoint
                          where PaperIDList.Contains(m.PaperID) && StudIDList.Contains(m.UserID.Value)
                          group m by m.KnowledgePointID into n
                          select new
                          {
                              KnowledgeID = n.Key,
                              AveCorrectRate = n.Average(m => m.CorrectRate),
                          };
            WKIList = from m in WKIList orderby m.AveCorrectRate select m;
            foreach (var wki in WKIList)
            {
                WrongKnowledgeInfo WrongKnowledge = new WrongKnowledgeInfo();
                WrongKnowledge.KnowledgeID = wki.KnowledgeID;
                WrongKnowledge.CorrectRate = wki.AveCorrectRate.Value;
                WrongKnowledgeList.Add(WrongKnowledge);
            }
            
            foreach (var item in WrongKnowledgeList)
            {
                var questionlist = from m in db.CEDTS_PaperAssessment
                                   where PaperIDList.Contains(m.PaperID.Value)
                                   from n in db.CEDTS_Question
                                   where n.AssessmentItemID == m.AssessmentItemID
                                   select n.QuestionID;
                item.TotalNum = (from m in db.CEDTS_QuestionKnowledge
                                where questionlist.Contains(m.QuestionID) && m.KnowledgePointID == item.KnowledgeID
                                select m).Count();
            }
            return WrongKnowledgeList;
        }
        #endregion

        #region 根据PaperID获取TestID列表
        List<Guid> ICedts_PaperRepository.SelectTestIDListByPaperID(Guid PaperID)
        {
            return db.CEDTS_Test.Where(m => m.PaperID == PaperID).Select(m => m.TestID).ToList();
        }
        #endregion

        #region 根据TestIDList StudIDList获取小题正确数列表
        List<QuestionDoneInfo> ICedts_PaperRepository.SelectQDIByTUList(List<Guid> TestIDList, List<int> StudIDList, int ItemTypeID)
        {
            var qinfo = from m in db.CEDTS_TestAnswer
                                           where m.ItemTypeID == ItemTypeID && TestIDList.Contains(m.TestID) && StudIDList.Contains(m.UserID.Value)
                                           group m by m.QuestionID into n
                                           select new
                                           {
                                               WrongNum = n.Count(m => m.IsRight.Value == false),
                                               QuestionNum = n.Average(m => m.Number)
                                           };
            var questioninfo = from m in qinfo orderby m.WrongNum descending select m;
            List<QuestionDoneInfo> QDIList = new List<QuestionDoneInfo>();
            foreach (var q in questioninfo)
            {
                QuestionDoneInfo qdi = new QuestionDoneInfo();
                qdi.QuestionNum = (int)q.QuestionNum;
                qdi.WrongNum = q.WrongNum;
                QDIList.Add(qdi);
            }
            return QDIList;
        }
        #endregion

        #region 更新试卷信息
        void ICedts_PaperRepository.UpadatePaper(CEDTS_Paper paper)
        {
            var PaperInfo = (from m in db.CEDTS_Paper where m.PaperID == paper.PaperID select m).FirstOrDefault();
            db.ApplyCurrentValues(PaperInfo.EntityKey.EntitySetName, paper);
            db.SaveChanges();
        }
        #endregion

        #region 更新暂存信息
        void ICedts_PaperRepository.UpdataPaperExpansion(CEDTS_PaperExpansion PaperExpansion)
        {
            var PaperId = (from m in db.CEDTS_PaperExpansion where m.PaperID == PaperExpansion.PaperID select m.PaperExpansionID).FirstOrDefault();
            var PaperExpansionInfo = (from m in db.CEDTS_PaperExpansion where m.PaperExpansionID == PaperId select m).FirstOrDefault();
            PaperExpansion.PaperExpansionID = PaperId;
            db.ApplyCurrentValues(PaperExpansionInfo.EntityKey.EntitySetName, PaperExpansion);
            db.SaveChanges();
        }
        #endregion

        #region 试卷名称验证
        bool ICedts_PaperRepository.CheckName(string name)
        {
            var PaperID = (from m in db.CEDTS_Paper where m.Title == name select m.PaperID).FirstOrDefault();
            return db.CEDTS_Paper.Where(p => p.Title == name && p.PaperID != PaperID).FirstOrDefault() != null;
        }
        #endregion

        #region 试卷删除
        int ICedts_PaperRepository.DeletePaper(Guid id)
        {
            CEDTS_Paper paper = new CEDTS_Paper();
            var list = (from m in db.CEDTS_Paper where m.PaperID == id select m).First();
            paper.PaperID = list.PaperID;
            paper.Title = list.Title;
            paper.Type = list.Type;
            paper.PaperContent = list.PaperContent;
            paper.Duration = list.Duration;
            paper.Difficult = list.Difficult;
            paper.Score = list.Score;
            paper.Description = list.Description;
            paper.CreateTime = list.CreateTime;
            paper.UserID = list.UserID;
            paper.UpdateUserID = list.UpdateUserID;
            paper.UpdateTime = list.UpdateTime;
            paper.State = 3;
            db.ApplyCurrentValues(list.EntityKey.EntitySetName, paper);
            db.SaveChanges();
            return 1;
        }
        #endregion

        #region 查询时页面跳转
        int ICedts_PaperRepository.Judge(Guid id)
        {
            var s = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 2 select m).Count();
            var s1 = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 3 select m).Count();
            var s2 = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 4 select m).Count();
            var s3 = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 5 select m).Count();
            var s4 = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 6 select m).Count();
            var s5 = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 7 select m).Count();
            var s6 = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 8 select m).Count();

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
            if (s4 == 0 && s8.MultipleChoiceNum != 0)
            {
                return 5;
            }
            if (s5 == 0 && s8.BankedClozeNum != 0)
            {
                return 6;
            }
            if (s6 == 0 && s8.ClozeNum != 0)
            {
                return 7;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 查询试卷类型
        string ICedts_PaperRepository.SelectName(Guid id)
        {
            var name = (from m in db.CEDTS_Paper where m.PaperID == id select m.Type).FirstOrDefault();
            return name;
        }
        #endregion

        #region 查询PartType
        List<CEDTS_PartType> ICedts_PaperRepository.SelectEditPartType()
        {
            return db.CEDTS_PartType.ToList();
        }
        #endregion

        #region 查询PartType对下的ItemType
        List<CEDTS_ItemType> ICedts_PaperRepository.SelectEditItemTypeByPartTypeID(int PartTypeID)
        {
            return db.CEDTS_ItemType.Where(p => p.PartTypeID == PartTypeID).ToList();
        }
        #endregion

        #region 根据UserID，AssessmentItemID查询CEDTS_UserAssessmentCount是否存在记录
        int ICedts_PaperRepository.SelectUA(int UserID, Guid AssessmentID)
        {
            var UA = (from m in db.CEDTS_UserAssessmentCount where m.UserID == UserID && m.AssessmentItemID == AssessmentID select m).FirstOrDefault();
            if (UA == null)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        #endregion

        #region 根据UserID，AssessmentItemID查询CEDTS_UserAssessmentCount表信息
        CEDTS_UserAssessmentCount ICedts_PaperRepository.SelectUAC(int UserID, Guid AssessmentID)
        {
            var UAC = (from m in db.CEDTS_UserAssessmentCount where m.UserID == UserID && m.AssessmentItemID == AssessmentID select m).FirstOrDefault();
            return UAC;
        }
        #endregion

        #region 更新CEDTS_UserAssessmentCount信息
        void ICedts_PaperRepository.UpdataUAC(CEDTS_UserAssessmentCount UAC)
        {
            var UACInfo = (from m in db.CEDTS_UserAssessmentCount where m.UserAssessmentCountID == UAC.UserAssessmentCountID select m).FirstOrDefault();
            db.ApplyCurrentValues(UACInfo.EntityKey.EntitySetName, UAC);
            db.SaveChanges();
        }
        #endregion

        #region 新增CEDTS_UserAssessmentCount表信息
        void ICedts_PaperRepository.CreateUAC(CEDTS_UserAssessmentCount UAC)
        {
            db.AddToCEDTS_UserAssessmentCount(UAC);
            db.SaveChanges();
        }
        #endregion

        #region 编辑时查询快速阅读信息
        List<SkimmingScanningPartCompletion> ICedts_PaperRepository.SelectEditSspc(Guid id)
        {
            List<SkimmingScanningPartCompletion> SspcList = new List<SkimmingScanningPartCompletion>();

            var AssmentIDList = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 1 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssmentID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == AssmentID orderby m.Order ascending select m).ToList();
                var ChoiceNum = (from m in db.CEDTS_Question where m.AssessmentItemID == AssmentID && m.ChooseA != null && m.QuestionContent != null orderby m.Order select m).ToList();
                var TermNum = (from m in db.CEDTS_Question where m.AssessmentItemID == AssmentID && m.ChooseA == null orderby m.Order select m).ToList();
                var Assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssmentID select m).FirstOrDefault();
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
                    string Qkid = string.Empty;

                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
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
                Sspc.Info = info;
                SspcList.Add(Sspc);
            }
            return SspcList;
        }
        #endregion

        #region 编辑是查询短对话信息
        List<Listen> ICedts_PaperRepository.SelectEditSlpo(Guid id)
        {
            List<Listen> SlpoList = new List<Listen>();
            var AssmentIDList = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 2 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var ChoiceNum = (from m in db.CEDTS_Question where m.AssessmentItemID == AssessID && m.ChooseA != null && m.QuestionContent != null orderby m.Order select m).ToList();
                var TermNum = (from m in db.CEDTS_Question where m.AssessmentItemID == AssessID && m.ChooseA == null orderby m.Order select m).ToList();
                var Assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
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
                info.timeInterval = new List<int>();
                info.questionSound = new List<string>();
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

                Slpo.SoundFile = Assess.SoundFile;
                Slpo.Script = Assess.Original;
                Slpo.SoundFile = AssessID.ToString();

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
                    info.questionSound.Add(list.Sound);
                    info.timeInterval.Add(list.Interval.Value);
                    Slpo.Choices.Add(list.ChooseA);
                    Slpo.Choices.Add(list.ChooseB);
                    Slpo.Choices.Add(list.ChooseC);
                    Slpo.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);
                    string name = string.Empty;
                    string Qkid = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
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
                Slpo.Info = info;
                SlpoList.Add(Slpo);
            }
            return SlpoList;
        }
        #endregion

        #region 编辑时查询长对话信息
        List<Listen> ICedts_PaperRepository.SelectEditLlpo(Guid id)
        {
            List<Listen> LlpoList = new List<Listen>();
            var AssmentIDList = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 3 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var Assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
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
                info.timeInterval = new List<int>();
                info.questionSound = new List<string>();
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

                Llpo.SoundFile = Assess.SoundFile;
                Llpo.Script = Assess.Original;
                Llpo.SoundFile = AssessID.ToString();

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
                    info.timeInterval.Add(list.Interval.Value);
                    info.questionSound.Add(list.Sound);
                    Llpo.Choices.Add(list.ChooseA);
                    Llpo.Choices.Add(list.ChooseB);
                    Llpo.Choices.Add(list.ChooseC);
                    Llpo.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);
                    string name = string.Empty;
                    string Qkid = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
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
                Llpo.Info = info;
                LlpoList.Add(Llpo);
            }
            return LlpoList;
        }
        #endregion

        #region 编辑时查询听力短文理解信息
        List<Listen> ICedts_PaperRepository.SelectEditRlpo(Guid id)
        {
            List<Listen> RlpoList = new List<Listen>();
            var AssmentIDList = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 4 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var Assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
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

                Rlpo.SoundFile = Assess.SoundFile;
                Rlpo.Script = Assess.Original;
                Rlpo.SoundFile = AssessID.ToString();

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
                    info.questionSound.Add(list.Sound);
                    info.timeInterval.Add(list.Interval.Value);
                    Rlpo.Choices.Add(list.ChooseA);
                    Rlpo.Choices.Add(list.ChooseB);
                    Rlpo.Choices.Add(list.ChooseC);
                    Rlpo.Choices.Add(list.ChooseD);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);
                    string name = string.Empty;
                    string Qkid = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
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
                Rlpo.Info = info;
                RlpoList.Add(Rlpo);
            }
            return RlpoList;
        }
        #endregion

        #region 编辑时查询复合型听力信息
        List<Listen> ICedts_PaperRepository.SelectEditLpc(Guid id)
        {
            List<Listen> LpcList = new List<Listen>();
            var AssmentIDList = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 5 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var Assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssessID select m).FirstOrDefault();
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
                info.timeInterval = new List<int>();
                info.questionSound = new List<string>();
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

                Lpc.SoundFile = Assess.SoundFile;
                Lpc.Script = Assess.Original;
                Lpc.SoundFile = AssessID.ToString();

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
                    info.questionSound.Add(list.Sound);
                    info.timeInterval.Add(list.Interval.Value);
                    info.AnswerValue.Add(list.Answer);
                    info.Tip.Add(list.Analyze);
                    string name = string.Empty;
                    string Qkid = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
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
                Lpc.Info = info;
                LpcList.Add(Lpc);
            }
            return null;
        }
        #endregion

        #region 编辑时查询阅读理解-选词填空信息
        List<ReadingPartCompletion> ICedts_PaperRepository.SelectEditRpc(Guid id)
        {
            List<ReadingPartCompletion> RpcList = new List<ReadingPartCompletion>();
            var AssmentIDList = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 6 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssessID select m).First();
                var Expansion = (from m in db.CEDTS_Expansion where m.AssessmentItemID == AssessID select m).First();
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
                    string Qkid = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
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
                Rpc.Info = info;
                RpcList.Add(Rpc);
            }
            return RpcList;
        }
        #endregion

        #region 编辑时查询阅读理解-选择题型信息
        List<ReadingPartOption> ICedts_PaperRepository.SlelectEditRpo(Guid id)
        {
            List<ReadingPartOption> RpoList = new List<ReadingPartOption>();
            var AssmentIDList = (from m in db.CEDTS_PaperAssessment where m.PaperID == id && m.ItemTypeID == 6 orderby m.Weight ascending select m.AssessmentItemID).ToList();
            foreach (var AssessID in AssmentIDList)
            {
                var question = (from m in db.CEDTS_Question where m.AssessmentItemID == AssessID orderby m.Order ascending select m).ToList();
                var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssessID select m).First();

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
                    string Qkid = string.Empty;


                    var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
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
                Rpo.Info = info;
                RpoList.Add(Rpo);
            }
            return RpoList;
        }
        #endregion

        #region 创建试卷XML
        string ICedts_PaperRepository.CreatePaperXML(PaperTotal pt)
        {
            XmlDocument doc;
            XmlNode node;
            XmlElement elem;

            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);

            //加入一个根元素
            elem = doc.CreateElement("", "Paper", "");
            elem.SetAttribute("UserID", pt.UserID.ToString());
            elem.SetAttribute("UpdateUserID", pt.UpdateUserID.ToString());
            elem.SetAttribute("PaperID", pt.PaperID.ToString());
            elem.SetAttribute("totalScore", pt.Score.ToString());
            elem.SetAttribute("totalTime", pt.Duration.ToString());
            elem.SetAttribute("name", pt.Title);
            elem.SetAttribute("resultTime", "0");
            elem.SetAttribute("bShowAns", "1");
            elem.SetAttribute("Diffcult", pt.Difficult.ToString());
            elem.SetAttribute("Type", pt.Type);
            doc.AppendChild(elem);

            XmlNode Paper = doc.SelectSingleNode("Paper");//查找<Paper> 

            #region 快速阅读部分
            if (pt.SspcList.Count > 0)
            {
                XmlElement SpeedRead = doc.CreateElement("Part");//创建一个<Part>节点
                SpeedRead.SetAttribute("Type", "SpeedRead");
                Paper.AppendChild(SpeedRead);
                double Score = 0.0;
                double Time = 0.0;
                for (int i = 0; i < pt.SspcList.Count; i++)
                {
                    Score += pt.SspcList[i].Info.Score;
                    Time += pt.SspcList[i].Info.ReplyTime;
                }
                XmlElement SRsection = doc.CreateElement("Section");
                SRsection.SetAttribute("type", pt.SspcList[0].Info.ItemType);
                SRsection.SetAttribute("typecn", pt.SspcList[0].Info.ItemType_CN);
                SRsection.SetAttribute("questioninterval", "0");
                SRsection.SetAttribute("Score", Score.ToString());
                SRsection.SetAttribute("Time", Time.ToString());
                SpeedRead.AppendChild(SRsection);

                for (int j = 0; j < pt.SspcList.Count; j++)
                {
                    XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                    AssessmentItem.SetAttribute("ItemID", pt.SspcList[j].Info.ItemID.ToString());
                    AssessmentItem.SetAttribute("PartType", pt.SspcList[j].Info.PartType);
                    AssessmentItem.SetAttribute("ItemType", pt.SspcList[j].Info.ItemType);
                    AssessmentItem.SetAttribute("ItemType_CN", pt.SspcList[j].Info.ItemType_CN);
                    AssessmentItem.SetAttribute("Score", pt.SspcList[j].Info.Score.ToString());
                    AssessmentItem.SetAttribute("ReplyTime", pt.SspcList[j].Info.ReplyTime.ToString());
                    AssessmentItem.SetAttribute("Diffcult", pt.SspcList[j].Info.Diffcult.ToString());
                    AssessmentItem.SetAttribute("QustionInterval", pt.SspcList[j].Info.QustionInterval);
                    SRsection.AppendChild(AssessmentItem);

                    XmlNode AssessmentItemNode = SRsection.SelectSingleNode("AssessmentItem");
                    XmlElement Content = doc.CreateElement("Content");
                    Content.InnerText = pt.SspcList[j].Content;
                    AssessmentItemNode.AppendChild(Content);

                    XmlElement Questions = doc.CreateElement("Questions");
                    Questions.SetAttribute("Count", pt.SspcList[j].Info.QuestionCount.ToString());
                    AssessmentItemNode.AppendChild(Questions);

                    for (int m = 0; m < pt.SspcList[j].ChoiceNum; m++)
                    {
                        XmlElement question = doc.CreateElement("question");
                        question.SetAttribute("id", m.ToString());
                        Questions.AppendChild(question);

                        XmlElement prompt = doc.CreateElement("prompt");
                        prompt.InnerText = pt.SspcList[j].Info.Problem[m];
                        question.AppendChild(prompt);

                        XmlElement choices = doc.CreateElement("choices");
                        question.AppendChild(choices);

                        XmlElement choice1 = doc.CreateElement("choice");
                        choice1.SetAttribute("id", "A");
                        choice1.InnerText = pt.SspcList[j].Choices[(0 + m * 4)];
                        choices.AppendChild(choice1);


                        XmlElement choice2 = doc.CreateElement("choice");
                        choice2.SetAttribute("id", "B");
                        choice2.InnerText = pt.SspcList[j].Choices[(1 + m * 4)];
                        choices.AppendChild(choice2);

                        XmlElement choice3 = doc.CreateElement("choice");
                        choice3.SetAttribute("id", "C");
                        choice3.InnerText = pt.SspcList[j].Choices[(2 + m * 4)];
                        choices.AppendChild(choice3);

                        XmlElement choice4 = doc.CreateElement("choice");
                        choice4.SetAttribute("id", "D");
                        choice4.InnerText = pt.SspcList[j].Choices[(3 + m * 4)];
                        choices.AppendChild(choice4);

                        XmlElement Answer = doc.CreateElement("Answer");
                        question.AppendChild(Answer);

                        XmlElement value = doc.CreateElement("Value");
                        value.InnerText = pt.SspcList[j].Info.AnswerValue[m];
                        Answer.AppendChild(value);

                        XmlElement response = doc.CreateElement("Response");
                        response.InnerText = pt.SspcList[j].Info.AnswerResposn;
                        Answer.AppendChild(response);

                        XmlElement tip = doc.CreateElement("Tip");
                        tip.InnerText = pt.SspcList[j].Info.Tip[m];
                        question.AppendChild(tip);
                    }

                    for (int n = 0; n < pt.SspcList[j].TermNum; n++)
                    {
                        XmlElement question1 = doc.CreateElement("question");
                        question1.SetAttribute("id", (n + pt.SspcList[j].ChoiceNum).ToString());
                        Questions.AppendChild(question1);

                        XmlElement prompt1 = doc.CreateElement("Prompt");
                        prompt1.InnerText = pt.SspcList[j].Info.Problem[n + pt.SspcList[j].ChoiceNum];
                        question1.AppendChild(prompt1);

                        XmlElement Answer1 = doc.CreateElement("Answer");
                        question1.AppendChild(Answer1);

                        XmlElement value1 = doc.CreateElement("Value");
                        value1.InnerText = pt.SspcList[j].Info.AnswerValue[n + pt.SspcList[j].ChoiceNum];
                        Answer1.AppendChild(value1);

                        XmlElement response1 = doc.CreateElement("Response");
                        response1.InnerText = pt.SspcList[j].Info.AnswerResposn;
                        Answer1.AppendChild(response1);

                        XmlElement tip1 = doc.CreateElement("Tip");
                        tip1.InnerText = pt.SspcList[j].Info.Tip[n + pt.SspcList[j].ChoiceNum];
                        question1.AppendChild(tip1);
                    }
                }
            }
            #endregion

            #region 听力部分
            if ((pt.SlpoList.Count + pt.LlpoList.Count + pt.LpcList.Count + pt.RlpoList.Count) > 0)
            {
                XmlElement Listen = doc.CreateElement("Part");//创建一个<Part>节点
                Listen.SetAttribute("Type", "Listen");
                Paper.AppendChild(Listen);

                #region 短对话听力部分
                if (pt.SlpoList.Count > 0)
                {
                    double Score = 0.0;
                    double Time = 0;
                    for (int i = 0; i < pt.SlpoList.Count; i++)
                    {
                        Score += pt.SlpoList[i].Info.ScoreQuestion[0];
                        Time += pt.SlpoList[i].Info.TimeQuestion[0];
                    }
                    XmlElement SLsection = doc.CreateElement("Section");
                    SLsection.SetAttribute("type", pt.SlpoList[0].Info.ItemType);
                    SLsection.SetAttribute("typecn", pt.SlpoList[0].Info.ItemType_CN);
                    SLsection.SetAttribute("questioninterval", "0");
                    SLsection.SetAttribute("Score", Score.ToString());
                    SLsection.SetAttribute("Time", Time.ToString());
                    Listen.AppendChild(SLsection);
                    for (int j = 0; j < pt.SlpoList.Count; j++)
                    {
                        XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                        AssessmentItem.SetAttribute("ItemID", pt.SlpoList[j].Info.ItemID.ToString());
                        AssessmentItem.SetAttribute("PartType", pt.SlpoList[j].Info.PartType);
                        AssessmentItem.SetAttribute("ItemType", pt.SlpoList[j].Info.ItemType);
                        AssessmentItem.SetAttribute("ItemType_CN", pt.SlpoList[j].Info.ItemType_CN);
                        AssessmentItem.SetAttribute("Score", pt.SlpoList[j].Info.Score.ToString());
                        AssessmentItem.SetAttribute("ReplyTime", pt.SlpoList[j].Info.ReplyTime.ToString());
                        AssessmentItem.SetAttribute("Diffcult", pt.SlpoList[j].Info.Diffcult.ToString());
                        AssessmentItem.SetAttribute("QustionInterval", pt.SlpoList[j].Info.QustionInterval);
                        AssessmentItem.SetAttribute("Course", pt.SlpoList[j].Info.Course);
                        AssessmentItem.SetAttribute("Unit", pt.SlpoList[j].Info.Unit);
                        SLsection.AppendChild(AssessmentItem);

                        XmlNode AssessmentItemNode = SLsection.SelectSingleNode("AssessmentItem");

                        XmlElement soundFile = doc.CreateElement("soundFile");//创建一个<soundFile>节点
                        soundFile.InnerText = pt.SlpoList[j].SoundFile;
                        AssessmentItemNode.AppendChild(soundFile);//添加到<AssessmentItem>节点中
                        XmlElement Questions = doc.CreateElement("Questions");
                        Questions.SetAttribute("Count", pt.SlpoList[j].Info.QuestionCount.ToString());
                        AssessmentItemNode.AppendChild(Questions);

                        for (int i = 0; i < pt.SlpoList[j].Info.QuestionCount; i++)
                        {
                            XmlElement question = doc.CreateElement("question");
                            question.SetAttribute("id", i.ToString());
                            Questions.AppendChild(question);

                            XmlElement choices = doc.CreateElement("choices");
                            question.AppendChild(choices);

                            XmlElement choice1 = doc.CreateElement("choice");
                            choice1.SetAttribute("id", "A");
                            choice1.InnerText = pt.SlpoList[j].Choices[(0 + i * 4)];
                            choices.AppendChild(choice1);


                            XmlElement choice2 = doc.CreateElement("choice");
                            choice2.SetAttribute("id", "B");
                            choice2.InnerText = pt.SlpoList[j].Choices[(1 + i * 4)];
                            choices.AppendChild(choice2);

                            XmlElement choice3 = doc.CreateElement("choice");
                            choice3.SetAttribute("id", "C");
                            choice3.InnerText = pt.SlpoList[j].Choices[(2 + i * 4)];
                            choices.AppendChild(choice3);

                            XmlElement choice4 = doc.CreateElement("choice");
                            choice4.SetAttribute("id", "D");
                            choice4.InnerText = pt.SlpoList[j].Choices[(3 + i * 4)];
                            choices.AppendChild(choice4);

                            XmlElement Answer = doc.CreateElement("Answer");
                            question.AppendChild(Answer);

                            XmlElement value = doc.CreateElement("Value");
                            value.InnerText = pt.SlpoList[j].Info.AnswerValue[i];
                            Answer.AppendChild(value);

                            XmlElement response = doc.CreateElement("Response");
                            response.InnerText = pt.SlpoList[j].Info.AnswerResposn;
                            Answer.AppendChild(response);

                            XmlElement tip = doc.CreateElement("Tip");
                            tip.InnerText = pt.SlpoList[j].Info.Tip[i];
                            question.AppendChild(tip);

                            XmlElement problem = doc.CreateElement("Script");
                            problem.InnerText = pt.SlpoList[j].Info.Problem[i];
                            question.AppendChild(problem);

                            XmlElement sound = doc.CreateElement("Sound");
                            sound.InnerText = pt.SlpoList[j].Info.questionSound[i];
                            sound.SetAttribute("duration", pt.SlpoList[j].Info.timeInterval[i].ToString());
                            question.AppendChild(sound);
                        }

                        XmlElement script = doc.CreateElement("Script");
                        script.InnerText = pt.SlpoList[j].Script;
                        AssessmentItemNode.AppendChild(script);
                    }
                }
                #endregion

                #region 长对话听力部分
                if (pt.LlpoList.Count > 0)
                {
                    double Score = 0.0;
                    double Time = 0;
                    for (int i = 0; i < pt.LlpoList.Count; i++)
                    {
                        Score += pt.LlpoList[i].Info.Score;
                        Time += pt.LlpoList[i].Info.ReplyTime;
                    }
                    XmlElement LLsection = doc.CreateElement("Section");
                    LLsection.SetAttribute("type", pt.LlpoList[0].Info.ItemType);
                    LLsection.SetAttribute("typecn", pt.LlpoList[0].Info.ItemType_CN);
                    LLsection.SetAttribute("questioninterval", "0");
                    LLsection.SetAttribute("Score", Score.ToString());
                    LLsection.SetAttribute("Time", Time.ToString());
                    Listen.AppendChild(LLsection);
                    for (int j = 0; j < pt.LlpoList.Count; j++)
                    {
                        XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                        AssessmentItem.SetAttribute("ItemID", pt.LlpoList[j].Info.ItemID.ToString());
                        AssessmentItem.SetAttribute("PartType", pt.LlpoList[j].Info.PartType);
                        AssessmentItem.SetAttribute("ItemType", pt.LlpoList[j].Info.ItemType);
                        AssessmentItem.SetAttribute("ItemType_CN", pt.LlpoList[j].Info.ItemType_CN);
                        AssessmentItem.SetAttribute("Score", pt.LlpoList[j].Info.Score.ToString());
                        AssessmentItem.SetAttribute("ReplyTime", pt.LlpoList[j].Info.ReplyTime.ToString());
                        AssessmentItem.SetAttribute("Diffcult", pt.LlpoList[j].Info.Diffcult.ToString());
                        AssessmentItem.SetAttribute("QustionInterval", pt.LlpoList[j].Info.QustionInterval);
                        AssessmentItem.SetAttribute("Course", pt.LlpoList[j].Info.Course);
                        AssessmentItem.SetAttribute("Unit", pt.LlpoList[j].Info.Unit);
                        LLsection.AppendChild(AssessmentItem);

                        XmlNode AssessmentItemNode = LLsection.SelectSingleNode("AssessmentItem");

                        XmlElement soundFile = doc.CreateElement("soundFile");//创建一个<soundFile>节点
                        soundFile.InnerText = pt.LlpoList[j].SoundFile;
                        AssessmentItemNode.AppendChild(soundFile);//添加到<AssessmentItem>节点中
                        XmlElement Questions = doc.CreateElement("Questions");
                        Questions.SetAttribute("Count", pt.LlpoList[j].Info.QuestionCount.ToString());
                        AssessmentItemNode.AppendChild(Questions);

                        for (int m = 0; m < pt.LlpoList[j].Info.QuestionCount; m++)
                        {
                            XmlElement question = doc.CreateElement("question");
                            question.SetAttribute("id", m.ToString());
                            Questions.AppendChild(question);

                            XmlElement choices = doc.CreateElement("choices");
                            question.AppendChild(choices);

                            XmlElement choice1 = doc.CreateElement("choice");
                            choice1.SetAttribute("id", "A");
                            choice1.InnerText = pt.LlpoList[j].Choices[(0 + m * 4)];
                            choices.AppendChild(choice1);


                            XmlElement choice2 = doc.CreateElement("choice");
                            choice2.SetAttribute("id", "B");
                            choice2.InnerText = pt.LlpoList[j].Choices[(1 + m * 4)];
                            choices.AppendChild(choice2);

                            XmlElement choice3 = doc.CreateElement("choice");
                            choice3.SetAttribute("id", "C");
                            choice3.InnerText = pt.LlpoList[j].Choices[(2 + m * 4)];
                            choices.AppendChild(choice3);

                            XmlElement choice4 = doc.CreateElement("choice");
                            choice4.SetAttribute("id", "D");
                            choice4.InnerText = pt.LlpoList[j].Choices[(3 + m * 4)];
                            choices.AppendChild(choice4);

                            XmlElement Answer = doc.CreateElement("Answer");
                            question.AppendChild(Answer);

                            XmlElement value = doc.CreateElement("Value");
                            value.InnerText = pt.LlpoList[j].Info.AnswerValue[m];
                            Answer.AppendChild(value);

                            XmlElement response = doc.CreateElement("Response");
                            response.InnerText = pt.LlpoList[j].Info.AnswerResposn;
                            Answer.AppendChild(response);

                            XmlElement tip = doc.CreateElement("Tip");
                            tip.InnerText = pt.LlpoList[j].Info.Tip[m];
                            question.AppendChild(tip);

                            XmlElement problem = doc.CreateElement("Script");
                            problem.InnerText = pt.LlpoList[j].Info.Problem[m];
                            question.AppendChild(problem);

                            XmlElement sound = doc.CreateElement("Sound");
                            sound.InnerText = pt.LlpoList[j].Info.questionSound[m];
                            sound.SetAttribute("duration", pt.LlpoList[j].Info.timeInterval[m].ToString());
                            question.AppendChild(sound);
                        }

                        XmlElement script = doc.CreateElement("Script");
                        script.InnerText = pt.LlpoList[j].Script;
                        AssessmentItemNode.AppendChild(script);
                    }
                }
                #endregion

                #region 短文理解听力部分
                if (pt.RlpoList.Count > 0)
                {
                    double Score = 0.0;
                    double Time = 0;
                    for (int i = 0; i < pt.RlpoList.Count; i++)
                    {
                        Score += pt.RlpoList[i].Info.Score;
                        Time += pt.RlpoList[i].Info.ReplyTime;
                    }
                    XmlElement RLsection = doc.CreateElement("Section");
                    RLsection.SetAttribute("type", pt.RlpoList[0].Info.ItemType);
                    RLsection.SetAttribute("typecn", pt.RlpoList[0].Info.ItemType_CN);
                    RLsection.SetAttribute("questioninterval", "0");
                    RLsection.SetAttribute("Score", Score.ToString());
                    RLsection.SetAttribute("Time", Time.ToString());
                    Listen.AppendChild(RLsection);
                    for (int j = 0; j < pt.RlpoList.Count; j++)
                    {
                        XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                        AssessmentItem.SetAttribute("ItemID", pt.RlpoList[j].Info.ItemID.ToString());
                        AssessmentItem.SetAttribute("PartType", pt.RlpoList[j].Info.PartType);
                        AssessmentItem.SetAttribute("ItemType", pt.RlpoList[j].Info.ItemType);
                        AssessmentItem.SetAttribute("ItemType_CN", pt.RlpoList[j].Info.ItemType_CN);
                        AssessmentItem.SetAttribute("Score", pt.RlpoList[j].Info.Score.ToString());
                        AssessmentItem.SetAttribute("ReplyTime", pt.RlpoList[j].Info.ReplyTime.ToString());
                        AssessmentItem.SetAttribute("Diffcult", pt.RlpoList[j].Info.Diffcult.ToString());
                        AssessmentItem.SetAttribute("QustionInterval", pt.RlpoList[j].Info.QustionInterval);
                        AssessmentItem.SetAttribute("Course", pt.RlpoList[j].Info.Course);
                        AssessmentItem.SetAttribute("Unit", pt.RlpoList[j].Info.Unit);
                        RLsection.AppendChild(AssessmentItem);

                        XmlNode AssessmentItemNode = RLsection.SelectSingleNode("AssessmentItem");

                        XmlElement soundFile = doc.CreateElement("soundFile");//创建一个<soundFile>节点
                        soundFile.InnerText = pt.RlpoList[j].SoundFile;
                        AssessmentItemNode.AppendChild(soundFile);//添加到<AssessmentItem>节点中
                        XmlElement Questions = doc.CreateElement("Questions");
                        Questions.SetAttribute("Count", pt.RlpoList[j].Info.QuestionCount.ToString());
                        AssessmentItemNode.AppendChild(Questions);

                        for (int m = 0; m < pt.RlpoList[j].Info.QuestionCount; m++)
                        {
                            XmlElement question = doc.CreateElement("question");
                            question.SetAttribute("id", m.ToString());
                            Questions.AppendChild(question);

                            XmlElement choices = doc.CreateElement("choices");
                            question.AppendChild(choices);

                            XmlElement choice1 = doc.CreateElement("choice");
                            choice1.SetAttribute("id", "A");
                            choice1.InnerText = pt.RlpoList[j].Choices[(0 + m * 4)];
                            choices.AppendChild(choice1);


                            XmlElement choice2 = doc.CreateElement("choice");
                            choice2.SetAttribute("id", "B");
                            choice2.InnerText = pt.RlpoList[j].Choices[(1 + m * 4)];
                            choices.AppendChild(choice2);

                            XmlElement choice3 = doc.CreateElement("choice");
                            choice3.SetAttribute("id", "C");
                            choice3.InnerText = pt.RlpoList[j].Choices[(2 + m * 4)];
                            choices.AppendChild(choice3);

                            XmlElement choice4 = doc.CreateElement("choice");
                            choice4.SetAttribute("id", "D");
                            choice4.InnerText = pt.RlpoList[j].Choices[(3 + m * 4)];
                            choices.AppendChild(choice4);

                            XmlElement Answer = doc.CreateElement("Answer");
                            question.AppendChild(Answer);

                            XmlElement value = doc.CreateElement("Value");
                            value.InnerText = pt.RlpoList[j].Info.AnswerValue[m];
                            Answer.AppendChild(value);

                            XmlElement response = doc.CreateElement("Response");
                            response.InnerText = pt.RlpoList[j].Info.AnswerResposn;
                            Answer.AppendChild(response);

                            XmlElement tip = doc.CreateElement("Tip");
                            tip.InnerText = pt.RlpoList[j].Info.Tip[m];
                            question.AppendChild(tip);

                            XmlElement problem = doc.CreateElement("Script");
                            problem.InnerText = pt.RlpoList[j].Info.Problem[m];
                            question.AppendChild(problem);

                            XmlElement sound = doc.CreateElement("Sound");
                            sound.InnerText = pt.RlpoList[j].Info.questionSound[m];
                            sound.SetAttribute("duration", pt.RlpoList[j].Info.timeInterval[m].ToString());
                            question.AppendChild(sound);
                        }

                        XmlElement script = doc.CreateElement("Script");
                        script.InnerText = pt.RlpoList[j].Script;
                        AssessmentItemNode.AppendChild(script);
                    }
                }
                #endregion

                #region 复合型听力部分
                if (pt.LpcList.Count > 0)
                {
                    double Score = 0.0;
                    double Time = 0;
                    for (int i = 0; i < pt.LpcList.Count; i++)
                    {
                        Score += pt.LpcList[i].Info.Score;
                        Time += pt.LpcList[i].Info.ReplyTime;
                    }
                    XmlElement LLsection = doc.CreateElement("Section");
                    LLsection.SetAttribute("type", pt.LpcList[0].Info.ItemType);
                    LLsection.SetAttribute("typecn", pt.LpcList[0].Info.ItemType_CN);
                    LLsection.SetAttribute("questioninterval", "0");
                    LLsection.SetAttribute("Score", Score.ToString());
                    LLsection.SetAttribute("Time", Time.ToString());
                    Listen.AppendChild(LLsection);
                    for (int j = 0; j < pt.LpcList.Count; j++)
                    {
                        XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                        AssessmentItem.SetAttribute("ItemID", pt.LpcList[j].Info.ItemID.ToString());
                        AssessmentItem.SetAttribute("PartType", pt.LpcList[j].Info.PartType);
                        AssessmentItem.SetAttribute("ItemType", pt.LpcList[j].Info.ItemType);
                        AssessmentItem.SetAttribute("ItemType_CN", pt.LpcList[j].Info.ItemType_CN);
                        AssessmentItem.SetAttribute("Score", pt.LpcList[j].Info.Score.ToString());
                        AssessmentItem.SetAttribute("ReplyTime", pt.LpcList[j].Info.ReplyTime.ToString());
                        AssessmentItem.SetAttribute("Diffcult", pt.LpcList[j].Info.Diffcult.ToString());
                        AssessmentItem.SetAttribute("QustionInterval", pt.LpcList[j].Info.QustionInterval);
                        AssessmentItem.SetAttribute("Course", pt.LpcList[j].Info.Course);
                        AssessmentItem.SetAttribute("Unit", pt.LpcList[j].Info.Unit);
                        LLsection.AppendChild(AssessmentItem);

                        XmlNode AssessmentItemNode = LLsection.SelectSingleNode("AssessmentItem");

                        XmlElement script = doc.CreateElement("Content");
                        script.InnerText = pt.LpcList[j].Script;
                        AssessmentItemNode.AppendChild(script);
                        XmlElement soundFile = doc.CreateElement("SoundFile");//创建一个<soundFile>节点
                        soundFile.InnerText = pt.LpcList[j].SoundFile;
                        AssessmentItemNode.AppendChild(soundFile);//添加到<AssessmentItem>节点中
                        XmlElement Questions = doc.CreateElement("Questions");
                        Questions.SetAttribute("Count", pt.LpcList[j].Info.QuestionCount.ToString());
                        AssessmentItemNode.AppendChild(Questions);

                        for (int m = 0; m < pt.LpcList[j].Info.QuestionCount; m++)
                        {
                            XmlElement question = doc.CreateElement("question");
                            question.SetAttribute("id", m.ToString());
                            Questions.AppendChild(question);
                            XmlElement Answer = doc.CreateElement("Answer");
                            question.AppendChild(Answer);

                            XmlElement value = doc.CreateElement("Value");
                            value.InnerText = pt.LpcList[j].Info.AnswerValue[m];
                            Answer.AppendChild(value);

                            XmlElement response = doc.CreateElement("Response");
                            response.InnerText = pt.LpcList[j].Info.AnswerResposn;
                            Answer.AppendChild(response);

                            XmlElement tip = doc.CreateElement("Tip");
                            tip.InnerText = pt.LpcList[j].Info.Tip[m];
                            question.AppendChild(tip);

                            XmlElement sound = doc.CreateElement("Sound");
                            sound.InnerText = pt.LpcList[j].Info.questionSound[m];
                            sound.SetAttribute("duration", pt.LpcList[j].Info.timeInterval[m].ToString());
                            question.AppendChild(sound);
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region 阅读理解部分
            if ((pt.RpcList.Count + pt.RpoList.Count + pt.InfMatList.Count) > 0)
            {
                XmlElement ReadingComprehension = doc.CreateElement("Part");//创建一个<Part>节点
                ReadingComprehension.SetAttribute("Type", "ReadingComprehension");
                Paper.AppendChild(ReadingComprehension);

                #region 阅读理解选词填空部分
                if (pt.RpcList.Count > 0)
                {
                    double Score = 0.0;
                    double Time = 0;
                    for (int i = 0; i < pt.RpcList.Count; i++)
                    {
                        Score += pt.RpcList[i].Info.Score;
                        Time += pt.RpcList[i].Info.ReplyTime;
                    }
                    XmlElement CRsection = doc.CreateElement("Section");
                    CRsection.SetAttribute("type", pt.RpcList[0].Info.ItemType);
                    CRsection.SetAttribute("typecn", pt.RpcList[0].Info.ItemType_CN);
                    CRsection.SetAttribute("questioninterval", "0");
                    CRsection.SetAttribute("Score", Score.ToString());
                    CRsection.SetAttribute("Time", Time.ToString());
                    ReadingComprehension.AppendChild(CRsection);

                    for (int j = 0; j < pt.RpcList.Count; j++)
                    {
                        XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                        AssessmentItem.SetAttribute("ItemID", pt.RpcList[j].Info.ItemID.ToString());
                        AssessmentItem.SetAttribute("PartType", pt.RpcList[j].Info.PartType);
                        AssessmentItem.SetAttribute("ItemType", pt.RpcList[j].Info.ItemType);
                        AssessmentItem.SetAttribute("ItemType_CN", pt.RpcList[j].Info.ItemType_CN);
                        AssessmentItem.SetAttribute("Score", pt.RpcList[j].Info.Score.ToString());
                        AssessmentItem.SetAttribute("ReplyTime", pt.RpcList[j].Info.ReplyTime.ToString());
                        AssessmentItem.SetAttribute("Diffcult", pt.RpcList[j].Info.Diffcult.ToString());
                        AssessmentItem.SetAttribute("QustionInterval", pt.RpcList[j].Info.QustionInterval);
                        CRsection.AppendChild(AssessmentItem);

                        XmlNode AssessmentItemNode = CRsection.SelectSingleNode("AssessmentItem");

                        XmlElement Content = doc.CreateElement("Content");
                        Content.InnerText = pt.RpcList[j].Content;
                        AssessmentItemNode.AppendChild(Content);

                        XmlElement WordList = doc.CreateElement("WordList");
                        AssessmentItemNode.AppendChild(WordList);

                        XmlElement WordA = doc.CreateElement("Word");
                        WordA.SetAttribute("id", "A");
                        WordA.InnerText = pt.RpcList[j].WordList[0];
                        WordList.AppendChild(WordA);

                        XmlElement WordB = doc.CreateElement("Word");
                        WordB.SetAttribute("id", "B");
                        WordB.InnerText = pt.RpcList[j].WordList[1];
                        WordList.AppendChild(WordB);

                        XmlElement WordC = doc.CreateElement("Word");
                        WordC.SetAttribute("id", "C");
                        WordC.InnerText = pt.RpcList[j].WordList[2];
                        WordList.AppendChild(WordC);

                        XmlElement WordD = doc.CreateElement("Word");
                        WordA.SetAttribute("id", "D");
                        WordA.InnerText = pt.RpcList[j].WordList[3];
                        WordList.AppendChild(WordA);

                        XmlElement WordE = doc.CreateElement("Word");
                        WordE.SetAttribute("id", "E");
                        WordE.InnerText = pt.RpcList[j].WordList[4];
                        WordList.AppendChild(WordE);

                        XmlElement WordF = doc.CreateElement("Word");
                        WordF.SetAttribute("id", "F");
                        WordF.InnerText = pt.RpcList[j].WordList[5];
                        WordList.AppendChild(WordF);

                        XmlElement WordG = doc.CreateElement("Word");
                        WordG.SetAttribute("id", "G");
                        WordG.InnerText = pt.RpcList[j].WordList[6];
                        WordList.AppendChild(WordG);

                        XmlElement WordH = doc.CreateElement("Word");
                        WordH.SetAttribute("id", "H");
                        WordH.InnerText = pt.RpcList[j].WordList[7];
                        WordList.AppendChild(WordH);

                        XmlElement WordI = doc.CreateElement("Word");
                        WordI.SetAttribute("id", "I");
                        WordI.InnerText = pt.RpcList[j].WordList[8];
                        WordList.AppendChild(WordI);

                        XmlElement WordJ = doc.CreateElement("Word");
                        WordJ.SetAttribute("id", "J");
                        WordJ.InnerText = pt.RpcList[j].WordList[9];
                        WordList.AppendChild(WordJ);

                        XmlElement WordK = doc.CreateElement("Word");
                        WordK.SetAttribute("id", "K");
                        WordK.InnerText = pt.RpcList[j].WordList[10];
                        WordList.AppendChild(WordK);

                        XmlElement WordL = doc.CreateElement("Word");
                        WordL.SetAttribute("id", "L");
                        WordL.InnerText = pt.RpcList[j].WordList[11];
                        WordList.AppendChild(WordL);

                        XmlElement WordM = doc.CreateElement("Word");
                        WordM.SetAttribute("id", "M");
                        WordM.InnerText = pt.RpcList[j].WordList[12];
                        WordList.AppendChild(WordM);

                        XmlElement WordN = doc.CreateElement("Word");
                        WordN.SetAttribute("id", "N");
                        WordN.InnerText = pt.RpcList[j].WordList[13];
                        WordList.AppendChild(WordN);

                        XmlElement WordO = doc.CreateElement("Word");
                        WordO.SetAttribute("id", "O");
                        WordO.InnerText = pt.RpcList[j].WordList[14];
                        WordList.AppendChild(WordO);

                        XmlElement Questions = doc.CreateElement("Questions");
                        Questions.SetAttribute("Count", pt.RpcList[j].Info.QuestionCount.ToString());
                        AssessmentItemNode.AppendChild(Questions);

                        for (int m = 0; m < pt.RpcList[j].Info.QuestionCount; m++)
                        {
                            XmlElement question = doc.CreateElement("question");
                            question.SetAttribute("id", m.ToString());
                            Questions.AppendChild(question);

                            XmlElement Answer = doc.CreateElement("Answer");
                            question.AppendChild(Answer);

                            XmlElement value = doc.CreateElement("Value");
                            value.InnerText = pt.RpcList[j].Info.AnswerValue[m];
                            Answer.AppendChild(value);

                            XmlElement response = doc.CreateElement("Response");
                            response.InnerText = pt.RpcList[j].Info.AnswerResposn;
                            Answer.AppendChild(response);

                            XmlElement tip = doc.CreateElement("Tip");
                            tip.InnerText = pt.RpcList[j].Info.Tip[m];
                            question.AppendChild(tip);
                        }
                    }
                }
                #endregion

                #region 阅读理解选择部分
                if (pt.RpoList.Count > 0)
                {
                    double Score = 0.0;
                    double Time = 0;
                    for (int i = 0; i < pt.RpoList.Count; i++)
                    {
                        Score += pt.RpoList[i].Info.Score;
                        Time += pt.RpoList[i].Info.ReplyTime;
                    }
                    XmlElement ORsection = doc.CreateElement("Section");
                    ORsection.SetAttribute("type", pt.RpoList[0].Info.ItemType);
                    ORsection.SetAttribute("typecn", pt.RpoList[0].Info.ItemType_CN);
                    ORsection.SetAttribute("questioninterval", "0");
                    ORsection.SetAttribute("Score", Score.ToString());
                    ORsection.SetAttribute("Time", Time.ToString());
                    ReadingComprehension.AppendChild(ORsection);

                    for (int j = 0; j < pt.RpoList.Count; j++)
                    {
                        XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                        AssessmentItem.SetAttribute("ItemID", pt.RpoList[j].Info.ItemID.ToString());
                        AssessmentItem.SetAttribute("PartType", pt.RpoList[j].Info.PartType);
                        AssessmentItem.SetAttribute("ItemType", pt.RpoList[j].Info.ItemType);
                        AssessmentItem.SetAttribute("ItemType_CN", pt.RpoList[j].Info.ItemType_CN);
                        AssessmentItem.SetAttribute("Score", pt.RpoList[j].Info.Score.ToString());
                        AssessmentItem.SetAttribute("ReplyTime", pt.RpoList[j].Info.ReplyTime.ToString());
                        AssessmentItem.SetAttribute("Diffcult", pt.RpoList[j].Info.Diffcult.ToString());
                        AssessmentItem.SetAttribute("QustionInterval", pt.RpoList[j].Info.QustionInterval);
                        ORsection.AppendChild(AssessmentItem);

                        XmlNode AssessmentItemNode = ORsection.SelectSingleNode("AssessmentItem");

                        XmlElement Content = doc.CreateElement("Content");
                        Content.InnerText = pt.RpoList[j].Content;
                        AssessmentItemNode.AppendChild(Content);

                        XmlElement Questions = doc.CreateElement("Questions");
                        Questions.SetAttribute("Count", pt.RpoList[j].Info.QuestionCount.ToString());
                        AssessmentItemNode.AppendChild(Questions);

                        for (int i = 0; i < pt.RpoList[j].Info.QuestionCount; i++)
                        {
                            XmlElement question = doc.CreateElement("question");
                            question.SetAttribute("id", i.ToString());
                            Questions.AppendChild(question);

                            XmlElement prompt = doc.CreateElement("prompt");
                            prompt.InnerText = pt.RpoList[j].Info.Problem[i];
                            question.AppendChild(prompt);

                            XmlElement choices = doc.CreateElement("choices");
                            question.AppendChild(choices);

                            XmlElement choice1 = doc.CreateElement("choice");
                            choice1.SetAttribute("id", "A");
                            choice1.InnerText = pt.RpoList[j].Choices[(0 + i * 4)];
                            choices.AppendChild(choice1);


                            XmlElement choice2 = doc.CreateElement("choice");
                            choice2.SetAttribute("id", "B");
                            choice2.InnerText = pt.RpoList[j].Choices[(1 + i * 4)];
                            choices.AppendChild(choice2);

                            XmlElement choice3 = doc.CreateElement("choice");
                            choice3.SetAttribute("id", "C");
                            choice3.InnerText = pt.RpoList[j].Choices[(2 + i * 4)];
                            choices.AppendChild(choice3);

                            XmlElement choice4 = doc.CreateElement("choice");
                            choice4.SetAttribute("id", "D");
                            choice4.InnerText = pt.RpoList[j].Choices[(3 + i * 4)];
                            choices.AppendChild(choice4);

                            XmlElement Answer = doc.CreateElement("Answer");
                            question.AppendChild(Answer);

                            XmlElement value = doc.CreateElement("Value");
                            value.InnerText = pt.RpoList[j].Info.AnswerValue[i];
                            Answer.AppendChild(value);

                            XmlElement response = doc.CreateElement("Response");
                            response.InnerText = pt.RpoList[j].Info.AnswerResposn;
                            Answer.AppendChild(response);

                            XmlElement tip = doc.CreateElement("Tip");
                            tip.InnerText = pt.RpoList[j].Info.Tip[i];
                            question.AppendChild(tip);
                        }
                    }
                }
                #endregion

                #region 信息匹配部分
                if (pt.InfMatList.Count > 0)
                {
                    double Score = 0.0;
                    double Time = 0;
                    for (int i = 0; i < pt.InfMatList.Count; i++)
                    {
                        Score += pt.InfMatList[i].Info.Score;
                        Time += pt.InfMatList[i].Info.ReplyTime;
                    }
                    XmlElement CRsection = doc.CreateElement("Section");
                    CRsection.SetAttribute("type", pt.InfMatList[0].Info.ItemType);
                    CRsection.SetAttribute("typecn", pt.InfMatList[0].Info.ItemType_CN);
                    CRsection.SetAttribute("questioninterval", "0");
                    CRsection.SetAttribute("Score", Score.ToString());
                    CRsection.SetAttribute("Time", Time.ToString());
                    ReadingComprehension.AppendChild(CRsection);

                    for (int j = 0; j < pt.InfMatList.Count; j++)
                    {
                        XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                        AssessmentItem.SetAttribute("ItemID", pt.InfMatList[j].Info.ItemID.ToString());
                        AssessmentItem.SetAttribute("PartType", pt.InfMatList[j].Info.PartType);
                        AssessmentItem.SetAttribute("ItemType", pt.InfMatList[j].Info.ItemType);
                        AssessmentItem.SetAttribute("ItemType_CN", pt.InfMatList[j].Info.ItemType_CN);
                        AssessmentItem.SetAttribute("Score", pt.InfMatList[j].Info.Score.ToString());
                        AssessmentItem.SetAttribute("ReplyTime", pt.InfMatList[j].Info.ReplyTime.ToString());
                        AssessmentItem.SetAttribute("Diffcult", pt.InfMatList[j].Info.Diffcult.ToString());
                        AssessmentItem.SetAttribute("QustionInterval", pt.InfMatList[j].Info.QustionInterval);
                        CRsection.AppendChild(AssessmentItem);

                        XmlNode AssessmentItemNode = CRsection.SelectSingleNode("AssessmentItem");

                        XmlElement Content = doc.CreateElement("Content");
                        Content.InnerText = pt.InfMatList[j].Content;
                        AssessmentItemNode.AppendChild(Content);

                        XmlElement WordList = doc.CreateElement("WordList");
                        AssessmentItemNode.AppendChild(WordList);

                        for (int k = 0; k < pt.InfMatList[j].Info.QuestionCount; k++)
                        {
                            XmlElement Word = doc.CreateElement("Word");
                            int Num = (int)('A')+ k;
                            char Numm = (char)Num;
                            Word.SetAttribute("id", Numm.ToString());
                            Word.InnerText = pt.InfMatList[j].WordList[k];
                            WordList.AppendChild(Word);
                        }

                        /*XmlElement WordA = doc.CreateElement("Word");
                        WordA.SetAttribute("id", "A");
                        WordA.InnerText = pt.InfMatList[j].WordList[0];
                        WordList.AppendChild(WordA);

                        XmlElement WordB = doc.CreateElement("Word");
                        WordB.SetAttribute("id", "B");
                        WordB.InnerText = pt.InfMatList[j].WordList[1];
                        WordList.AppendChild(WordB);

                        XmlElement WordC = doc.CreateElement("Word");
                        WordC.SetAttribute("id", "C");
                        WordC.InnerText = pt.InfMatList[j].WordList[2];
                        WordList.AppendChild(WordC);

                        XmlElement WordD = doc.CreateElement("Word");
                        WordA.SetAttribute("id", "D");
                        WordA.InnerText = pt.InfMatList[j].WordList[3];
                        WordList.AppendChild(WordA);

                        XmlElement WordE = doc.CreateElement("Word");
                        WordE.SetAttribute("id", "E");
                        WordE.InnerText = pt.InfMatList[j].WordList[4];
                        WordList.AppendChild(WordE);

                        XmlElement WordF = doc.CreateElement("Word");
                        WordF.SetAttribute("id", "F");
                        WordF.InnerText = pt.InfMatList[j].WordList[5];
                        WordList.AppendChild(WordF);

                        XmlElement WordG = doc.CreateElement("Word");
                        WordG.SetAttribute("id", "G");
                        WordG.InnerText = pt.InfMatList[j].WordList[6];
                        WordList.AppendChild(WordG);

                        XmlElement WordH = doc.CreateElement("Word");
                        WordH.SetAttribute("id", "H");
                        WordH.InnerText = pt.InfMatList[j].WordList[7];
                        WordList.AppendChild(WordH);

                        XmlElement WordI = doc.CreateElement("Word");
                        WordI.SetAttribute("id", "I");
                        WordI.InnerText = pt.InfMatList[j].WordList[8];
                        WordList.AppendChild(WordI);

                        XmlElement WordJ = doc.CreateElement("Word");
                        WordJ.SetAttribute("id", "J");
                        WordJ.InnerText = pt.InfMatList[j].WordList[9];
                        WordList.AppendChild(WordJ);*/



                        XmlElement Questions = doc.CreateElement("Questions");
                        Questions.SetAttribute("Count", pt.InfMatList[j].Info.QuestionCount.ToString());
                        AssessmentItemNode.AppendChild(Questions);

                        for (int m = 0; m < pt.InfMatList[j].Info.QuestionCount; m++)
                        {
                            XmlElement question = doc.CreateElement("question");
                            question.SetAttribute("id", m.ToString());
                            Questions.AppendChild(question);

                            XmlElement Answer = doc.CreateElement("Answer");
                            question.AppendChild(Answer);

                            XmlElement value = doc.CreateElement("Value");
                            value.InnerText = pt.InfMatList[j].Info.AnswerValue[m];
                            Answer.AppendChild(value);

                            XmlElement response = doc.CreateElement("Response");
                            response.InnerText = pt.InfMatList[j].Info.AnswerResposn;
                            Answer.AppendChild(response);

                            XmlElement tip = doc.CreateElement("Tip");
                            tip.InnerText = pt.InfMatList[j].Info.Tip[m];
                            question.AppendChild(tip);
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region 完型填空部分
            if (pt.CpList.Count > 0)
            {
                XmlElement Cloze = doc.CreateElement("Part");//创建一个<Part>节点
                Cloze.SetAttribute("Type", "Cloze ");
                Paper.AppendChild(Cloze);

                double Score = 0.0;
                double Time = 0;
                for (int i = 0; i < pt.CpList.Count; i++)
                {
                    Score += pt.CpList[i].Info.Score;
                    Time += pt.CpList[i].Info.ReplyTime;
                }
                XmlElement Csection = doc.CreateElement("Section");
                Csection.SetAttribute("type", pt.CpList[0].Info.ItemType);
                Csection.SetAttribute("typecn", pt.CpList[0].Info.ItemType_CN);
                Csection.SetAttribute("questioninterval", "0");
                Csection.SetAttribute("Score", Score.ToString());
                Csection.SetAttribute("Time", Time.ToString());
                Cloze.AppendChild(Csection);

                for (int j = 0; j < pt.CpList.Count; j++)
                {
                    XmlElement AssessmentItem = doc.CreateElement("AssessmentItem");
                    AssessmentItem.SetAttribute("ItemID", pt.CpList[j].Info.ItemID.ToString());
                    AssessmentItem.SetAttribute("PartType", pt.CpList[j].Info.PartType);
                    AssessmentItem.SetAttribute("ItemType", pt.CpList[j].Info.ItemType);
                    AssessmentItem.SetAttribute("ItemType_CN", pt.CpList[j].Info.ItemType_CN);
                    AssessmentItem.SetAttribute("Score", pt.CpList[j].Info.Score.ToString());
                    AssessmentItem.SetAttribute("ReplyTime", pt.CpList[j].Info.ReplyTime.ToString());
                    AssessmentItem.SetAttribute("Diffcult", pt.CpList[j].Info.Diffcult.ToString());
                    AssessmentItem.SetAttribute("QustionInterval", pt.CpList[j].Info.QustionInterval);
                    Csection.AppendChild(AssessmentItem);

                    XmlNode AssessmentItemNode = Csection.SelectSingleNode("AssessmentItem");

                    XmlElement Content = doc.CreateElement("Content");
                    Content.InnerText = pt.CpList[j].Content;
                    AssessmentItemNode.AppendChild(Content);

                    XmlElement Questions = doc.CreateElement("Questions");
                    Questions.SetAttribute("Count", pt.CpList[j].Info.QuestionCount.ToString());
                    AssessmentItemNode.AppendChild(Questions);

                    for (int m = 0; m < pt.CpList[j].Info.QuestionCount; m++)
                    {
                        XmlElement question = doc.CreateElement("question");
                        question.SetAttribute("id", m.ToString());
                        Questions.AppendChild(question);

                        XmlElement choices = doc.CreateElement("choices");
                        question.AppendChild(choices);

                        XmlElement choice1 = doc.CreateElement("choice");
                        choice1.SetAttribute("id", "A");
                        choice1.InnerText = pt.CpList[j].Choices[(0 + m * 4)];
                        choices.AppendChild(choice1);


                        XmlElement choice2 = doc.CreateElement("choice");
                        choice2.SetAttribute("id", "B");
                        choice2.InnerText = pt.CpList[j].Choices[(1 + m * 4)];
                        choices.AppendChild(choice2);

                        XmlElement choice3 = doc.CreateElement("choice");
                        choice3.SetAttribute("id", "C");
                        choice3.InnerText = pt.CpList[j].Choices[(2 + m * 4)];
                        choices.AppendChild(choice3);

                        XmlElement choice4 = doc.CreateElement("choice");
                        choice4.SetAttribute("id", "D");
                        choice4.InnerText = pt.CpList[j].Choices[(3 + m * 4)];
                        choices.AppendChild(choice4);

                        XmlElement Answer = doc.CreateElement("Answer");
                        question.AppendChild(Answer);

                        XmlElement value = doc.CreateElement("Value");
                        value.InnerText = pt.CpList[j].Info.AnswerValue[m];
                        Answer.AppendChild(value);

                        XmlElement response = doc.CreateElement("Response");
                        response.InnerText = pt.CpList[j].Info.AnswerResposn;
                        Answer.AppendChild(response);

                        XmlElement tip = doc.CreateElement("Tip");
                        tip.InnerText = pt.CpList[j].Info.Tip[m];
                        question.AppendChild(tip);
                    }
                }
            }
            #endregion

            XmlNodeList questionList = Paper.SelectNodes("question");
            for (int k = 0; k < questionList.Count; k++)
            {
                XmlElement question = (XmlElement)questionList[k];
                question.SetAttribute("Numerical", (k + 1).ToString());
            }

            //项目根目录的物理路径
            string serverPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationPaperLibrary\\";
            //文件名称
            var filename = pt.PaperID.ToString() + ".xml";
            //文件及物理路径
            var filepath = serverPath + filename;
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }
            doc.Save(filepath);

            XDocument dd = XDocument.Load(filepath);
            return dd.ToString();
        }
        #endregion

        #region 获取试卷和试题关系表的ID（List）
        List<CEDTS_PaperAssessment> ICedts_PaperRepository.SelectPaperAssessmentItem(Guid id)
        {
            return db.CEDTS_PaperAssessment.Where(p => p.PaperID == id).ToList();
        }
        #endregion

        #region 获取一个试卷对象
        CEDTS_Paper ICedts_PaperRepository.SelectPaper(Guid id)
        {
            return db.CEDTS_Paper.Where(p => p.PaperID == id).FirstOrDefault();
        }
        #endregion

        #region 获取试题对象
        CEDTS_AssessmentItem ICedts_PaperRepository.SelectAssessmentItem(Guid id)
        {
            return db.CEDTS_AssessmentItem.Where(p => p.AssessmentItemID == id).FirstOrDefault();
        }
        #endregion

        #region 获取试题类型对象
        CEDTS_ItemType ICedts_PaperRepository.SelectItemType(int id)
        {
            return db.CEDTS_ItemType.Where(p => p.ItemTypeID == id).FirstOrDefault();
        }
        #endregion

        #region 获取试题中所有的小问题
        List<CEDTS_Question> ICedts_PaperRepository.SelectQuestion(Guid id)
        {
            return db.CEDTS_Question.Where(p => p.AssessmentItemID == id).OrderBy(p => p.Order).ToList();
        }
        #endregion

        #region 查询选词填空答案内容
        string ICedts_PaperRepository.SelectAnswerValue(Guid id, string value)
        {
            var item = db.CEDTS_Expansion.Where(p => p.AssessmentItemID == id).FirstOrDefault();
            string v = string.Empty;
            switch (value)
            {
                case "A": v = item.ChoiceA; break;
                case "B": v = item.ChoiceB; break;
                case "C": v = item.ChoiceC; break;
                case "D": v = item.ChoiceD; break;
                case "E": v = item.ChoiceE; break;
                case "F": v = item.ChoiceF; break;
                case "G": v = item.ChoiceG; break;
                case "H": v = item.ChoiceH; break;
                case "I": v = item.ChoiceI; break;
                case "J": v = item.ChoiceJ; break;
                case "K": v = item.ChoiceK; break;
                case "L": v = item.ChoiceL; break;
                case "M": v = item.ChoiceM; break;
                case "N": v = item.ChoiceN; break;
                case "O": v = item.ChoiceO; break;
            }
            return v;
        }
        #endregion

        #region 获取试题中的选词内容
        CEDTS_Expansion ICedts_PaperRepository.SelectExpansion(Guid id)
        {
            return db.CEDTS_Expansion.Where(p => p.AssessmentItemID == id).FirstOrDefault();
        }
        #endregion
    }
}