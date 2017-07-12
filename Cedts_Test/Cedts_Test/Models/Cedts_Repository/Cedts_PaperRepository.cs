using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Cedts_Test.Models
{
    public class Cedts_PaperRepository : ICedts_PaperRepository
    {
        #region 实例化Cedts_Entities
        CedtsEntities db;
        public Cedts_PaperRepository()
        {
            db = new CedtsEntities();
        }
        #endregion
        int defaultPageSize = 10;

        #region 获取试卷展示信息（List）
        PagedList<CEDTS_Paper> ICedts_PaperRepository.SelectPapers(int? id)
        {
            IQueryable<CEDTS_Paper> paperList = db.CEDTS_Paper.Where(p => p.State == 1).OrderByDescending(p => p.CreateTime);
            PagedList<CEDTS_Paper> Paper = paperList.ToPagedList(id ?? 1, defaultPageSize);
            return Paper;
        }
        #endregion

        #region 获取一个试卷对象
        CEDTS_Paper ICedts_PaperRepository.SelectPaper(Guid id)
        {
            return db.CEDTS_Paper.Where(p => p.PaperID == id).FirstOrDefault();
        }
        #endregion

        #region 获取一个班级对象
        CEDTS_Class ICedts_PaperRepository.GetClassbyID(Guid ClassID)
        {
            return db.CEDTS_Class.Where(p => p.ClassID == ClassID).FirstOrDefault();
        }
        #endregion

        #region 根据班级ID获取学生列表
        List<CEDTS_User> ICedts_PaperRepository.SelectUserByClassID(Guid ClassID)
        {
            return db.CEDTS_User.Where(p => p.ClassID == ClassID && p.Role == "普通用户").Where(p => p.State == true || p.State == null).OrderBy(p => p.StudentNumber).ToList();
        }
        #endregion

        #region 根据试卷ID获取测试对象
        CEDTS_Test ICedts_PaperRepository.GetTestByPaperID(Guid PaperID, int UserID)
        {
            return db.CEDTS_Test.Where(m => m.PaperID == PaperID && m.UserID == UserID).OrderBy(m => m.FinishDate).FirstOrDefault();
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

        #region 获取试卷和试题关系表的ID（List）
        List<CEDTS_PaperAssessment> ICedts_PaperRepository.SelectPaperAssessmentItem(Guid id)
        {
            return db.CEDTS_PaperAssessment.Where(p => p.PaperID == id).ToList();
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

        #region 获取试题中的选词内容
        CEDTS_Expansion ICedts_PaperRepository.SelectExpansion(Guid id)
        {
            return db.CEDTS_Expansion.Where(p => p.AssessmentItemID == id).FirstOrDefault();
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
                    //List<Guid> guidList = db.CEDTS_UserAssessmentCount.Where(p => p.UserID == userID).OrderByDescending(p => p.Count).Select(p => p.AssessmentItemID).ToList();
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
                    //{
                    //    List<int> indexList = new List<int>();
                    //    List<Guid> templist = new List<Guid>();
                    //    for (int tempnum = 0; tempnum < int.Parse(itemList[k]); tempnum++)
                    //    {
                    //        int index = 0;
                    //        while (true)
                    //        {
                    //            Random rand = new Random();
                    //            index = rand.Next(list.Count);
                    //            if (!indexList.Contains(index))
                    //            {
                    //                indexList.Add(index);
                    //                break;
                    //            }
                    //        }
                    //        templist.Add(list[index]);
                    //    }
                    //    list.Clear();
                    //    list.AddRange(templist);
                    //}
                    // else
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

            #region 选取用户做过的小题
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

        #region 获取试卷中所有的试题信息（得分最大化&&知识点弱项）
        List<CEDTS_AssessmentItem> ICedts_PaperRepository.SelectAssessmentItems3(List<string> knowList, List<string> itemList, int userID)
        {
            var Questions = db.CEDTS_Question.Where(p => 1 == 1);
            var QuestionKnowledges = db.CEDTS_QuestionKnowledge.Where(p => 1 == 1);
            List<Guid> knowIDList = new List<Guid>();
            List<int> countList = new List<int>();
            for (int i = 0; i < knowList.Count; i++)
            {
                knowIDList.Add(Guid.Parse(knowList[i]));
                countList.Add(int.Parse(itemList[i]));
            }

            #region 选取用户做过的小题
            List<Guid> uaList = db.CEDTS_UserAssessmentCount.Where(p => p.UserID == userID).Select(p => p.AssessmentItemID).Distinct().ToList();
            List<Guid> uqList = new List<Guid>();
            List<Guid> alluqList = new List<Guid>();
            foreach (var ua in uaList)
            {
                List<Guid> tempqList = Questions.Where(p => p.AssessmentItemID == ua).Select(p => p.QuestionID).ToList();
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
            List<Guid> allItems = new List<Guid>();
            for (int i = 0; i < knowIDList.Count; i++)
            {
                Guid knowID = knowIDList[i];
                List<Guid> knowItemList = new List<Guid>();
                IQueryable<Guid> aqList = QuestionKnowledges.Where(p => p.KnowledgePointID == knowID).Select(p => p.QuestionID).Distinct();
                foreach (var q in aqList)
                {
                    var tempq = Questions.Where(p => p.QuestionID == q).FirstOrDefault();
                    if (!uqList.Contains(q) && !allaqList.Contains(tempq))
                    {
                        allaqList.Add(tempq);
                        var item = tempq.AssessmentItemID;
                        if (!uaList.Contains(item) && !allItems.Contains(item))
                            knowItemList.Add(item);
                        if (knowItemList.Distinct().ToList().Count >= countList[i])
                        {
                            allItems.AddRange(knowItemList);
                            break;
                        }
                    }
                }
            }
            #endregion


            #region 获取List<CEDTS_AssessmentItem>

            if (allItems.Count < countList.Sum())
            {
                Random random = new Random();
                while (allItems.Count < countList.Sum())
                {
                    int a = random.Next(uaList.Count);
                    Guid id = uaList[a];
                    if (!allItems.Contains(id))
                    {
                        allItems.Add(id);
                    }
                }
            }

            List<CEDTS_AssessmentItem> AList = new List<CEDTS_AssessmentItem>();
            //var tempAIDList = QList.Select(p => p.AssessmentItemID).Distinct();
            foreach (var aid in allItems)
            {
                AList.Add(db.CEDTS_AssessmentItem.Where(p => p.AssessmentItemID == aid).FirstOrDefault());
            }
            #endregion

            return AList;
        }
        #endregion

        #region 获取用户ID
        int ICedts_PaperRepository.SelectUserID(string userAccount)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == userAccount).Select(p => p.UserID).FirstOrDefault();
        }
        #endregion

        #region 获取用户ID
        CEDTS_User ICedts_PaperRepository.SelectUserInfo(string userAccount)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == userAccount).FirstOrDefault();
        }
        #endregion

        #region 保存试卷到数据库
        void ICedts_PaperRepository.SavePaper(CEDTS_Paper paper)
        {
            db.AddToCEDTS_Paper(paper);
            db.SaveChanges();
        }
        #endregion

        #region 保存试卷和试题关系表
        void ICedts_PaperRepository.SavePaperAssessmentItem(CEDTS_PaperAssessment pa)
        {
            db.AddToCEDTS_PaperAssessment(pa);
            db.SaveChanges();
        }
        #endregion 

        #region 根据PaperID查询Paper的State值
        int ICedts_PaperRepository.SelectPS(Guid PaperID)
        {
            var state = (from m in db.CEDTS_Paper where m.PaperID == PaperID select m.State).FirstOrDefault();
            return Convert.ToInt32(state);
        }
        #endregion

        #region 根据QuestionID查询KnowledgeID

        List<Guid> ICedts_PaperRepository.SelectKnowledge(Guid id)
        {
            List<Guid> KnowledgeList = new List<Guid>();
            var knowledge = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == id select m).ToList();
            foreach (var K in knowledge)
            {
                Guid kp = K.KnowledgePointID;
                KnowledgeList.Add(kp);
            }
            return KnowledgeList;
        }
        #endregion

        #region 根据知识点ID获取知识点对象
        CEDTS_KnowledgePoints ICedts_PaperRepository.SelectKnowledgeByID(Guid id)
        {
            return db.CEDTS_KnowledgePoints.Where(p => p.KnowledgePointID == id).FirstOrDefault();
        }
        #endregion

        #region 根据知识点ID.答卷ID查询查询TestAnswerKnowledge表
        CEDTS_TestAnswerKnowledgePoint ICedts_PaperRepository.SelectTestAKP(Guid KPID, Guid? PaperID)
        {
            var TestAKP = (from m in db.CEDTS_TestAnswerKnowledgePoint where m.KnowledgePointID == KPID && m.PaperID == PaperID orderby m.Time descending select m).FirstOrDefault();
            return TestAKP;
        }
        #endregion

        #region 新增答卷信息
        void ICedts_PaperRepository.CreateTest(CEDTS_Test test)
        {
            db.AddToCEDTS_Test(test);
            db.SaveChanges();
        }
        #endregion

        #region 判断SampleKnowledgeInfo是否存在
        int ICedts_PaperRepository.SelectSIK(Guid KPID)
        {
            var SIK = (from m in db.CEDTS_SampleKnowledgeInfomation where m.KnowledgePointID == KPID select m).FirstOrDefault();
            if (SIK == null)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        #endregion

        #region 根据KPID，PaperID查询CEDTS_SampleKnowledgeInfomation表
        CEDTS_SampleKnowledgeInfomation ICedts_PaperRepository.SelectSKI(Guid KPID)
        {
            var SKI = (from m in db.CEDTS_SampleKnowledgeInfomation where m.KnowledgePointID == KPID select m).FirstOrDefault();
            return SKI;
        }
        #endregion

        #region 根据KPID，UserID查询CEDTS_UserKnowledgeInfomation表
        CEDTS_UserKnowledgeInfomation ICedts_PaperRepository.SelectUKI(Guid KPID, int? UserID)
        {
            var UKI = (from m in db.CEDTS_UserKnowledgeInfomation where m.KnowledgePointID == KPID && m.UserID == UserID select m).FirstOrDefault();
            return UKI;
        }
        #endregion

        #region 根据用户ID，试题类型ID查询CEDTS_SmapleAnswerTypeInfo是否存在记录
        int ICedts_PaperRepository.SelectSATI(int? ItemID)
        {
            var SATI = (from m in db.CEDTS_SmapleAnswerTypeInfo where m.ItemTypeID == ItemID select m).FirstOrDefault();
            if (SATI == null)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        #endregion

        #region 根据UserID，ItemID查询CEDTS_UserAnswerInfo表信息
        CEDTS_UserAnswerInfo ICedts_PaperRepository.SelectUAI(int? UserID, int? ItemID)
        {
            var UAI = (from m in db.CEDTS_UserAnswerInfo where m.UserID == UserID && m.ItemTypeID == ItemID select m).FirstOrDefault();
            return UAI;
        }
        #endregion

        #region 根据AssessmentItemID查询CEDTS_AssessmentItem出题人的ID号
        int ICedts_PaperRepository.SelectAU(Guid AssessmentID)
        {
            var AU = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == AssessmentID select m.UserID).FirstOrDefault();
            int UserID = Convert.ToInt32(AU);            
            return UserID;
        }
        #endregion

        #region 据用户ID，试题类型ID查询CEDTS_SmapleAnswerTypeInfo信息
        CEDTS_SmapleAnswerTypeInfo ICedts_PaperRepository.SelectSMAP(int? ItemID)
        {
            var MAP = (from m in db.CEDTS_SmapleAnswerTypeInfo where m.ItemTypeID == ItemID select m).FirstOrDefault();
            return MAP;
        }
        #endregion

        #region 根据UserID，AssessmentItemID查询CEDTS_UserAssessmentCount是否存在记录
        int ICedts_PaperRepository.SelectUA(int? UserID, Guid AssessmentID)
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
        CEDTS_UserAssessmentCount ICedts_PaperRepository.SelectUAC(int? UserID, Guid AssessmentID)
        {
            var UAC = (from m in db.CEDTS_UserAssessmentCount where m.UserID == UserID && m.AssessmentItemID == AssessmentID select m).FirstOrDefault();
            return UAC;
        }
        #endregion

        #region 更新Test表信息
        void ICedts_PaperRepository.UpdataTest(CEDTS_Test Test)
        {
            var TestInfo = (from m in db.CEDTS_Test where m.TestID == Test.TestID select m).FirstOrDefault();
            db.ApplyCurrentValues(TestInfo.EntityKey.EntitySetName, Test);
            db.SaveChanges();
        }
        #endregion

        #region 新增TestAnswer表信息
        void ICedts_PaperRepository.CreateTestAnswer(CEDTS_TestAnswer TestAnswer)
        {
            db.AddToCEDTS_TestAnswer(TestAnswer);
            db.SaveChanges();
        }
        #endregion

        #region 新增CEDTS_TestAnswerKnowledgePoint表信息
        void ICedts_PaperRepository.CreateTAKP(CEDTS_TestAnswerKnowledgePoint TAKP)
        {
            db.AddToCEDTS_TestAnswerKnowledgePoint(TAKP);
            db.SaveChanges();
        }
        #endregion

        #region 新增CEDTS_UserKnowledgeInfomation信息
        void ICedts_PaperRepository.CreateUKI(CEDTS_UserKnowledgeInfomation UKI)
        {
            db.AddToCEDTS_UserKnowledgeInfomation(UKI);
            db.SaveChanges();
        }
        #endregion

        #region  更新CEDTS_UserKnowledgeInfomation信息
        void ICedts_PaperRepository.UpdataUKI(CEDTS_UserKnowledgeInfomation UKI)
        {
            var UKIInfo = (from m in db.CEDTS_UserKnowledgeInfomation where m.UKII_ID == UKI.UKII_ID select m).FirstOrDefault();
            db.ApplyCurrentValues(UKIInfo.EntityKey.EntitySetName, UKI);
            db.SaveChanges();
        }
        #endregion

        #region 新增CEDTS_SampleKnowledgeInfomation信息
        void ICedts_PaperRepository.CreateSKI(CEDTS_SampleKnowledgeInfomation SKI)
        {
            db.AddToCEDTS_SampleKnowledgeInfomation(SKI);
            db.SaveChanges();
        }
        #endregion

        #region 新增CEDTS_SampleKnowledgeInfomation信息
        void ICedts_PaperRepository.UpdataSKI(CEDTS_SampleKnowledgeInfomation SKI)
        {
            var SKIInfo = (from m in db.CEDTS_SampleKnowledgeInfomation where m.SKII_ID == SKI.SKII_ID select m).FirstOrDefault();
            db.ApplyCurrentValues(SKIInfo.EntityKey.EntitySetName, SKI);
            db.SaveChanges();
        }
        #endregion

        #region 新增CEDTS_TestAnswerTypeInfo信息
        void ICedts_PaperRepository.CreateTATI(CEDTS_TestAnswerTypeInfo TATI)
        {
            db.AddToCEDTS_TestAnswerTypeInfo(TATI);
            db.SaveChanges();
        }
        #endregion

        #region 新增CEDTS_UserAnswerInfo信息
        void ICedts_PaperRepository.CreateUATI(CEDTS_UserAnswerInfo UATI)
        {
            db.AddToCEDTS_UserAnswerInfo(UATI);
            db.SaveChanges();
        }
        #endregion

        #region 更新CEDTS_UserAnswerInfo信息
        void ICedts_PaperRepository.UpdataUATI(CEDTS_UserAnswerInfo UATI)
        {
            var UATIInfo = (from m in db.CEDTS_UserAnswerInfo where m.UATI_ID == UATI.UATI_ID select m).FirstOrDefault();
            db.ApplyCurrentValues(UATIInfo.EntityKey.EntitySetName, UATI);
            db.SaveChanges();
        }
        #endregion

        #region 新增CEDTS_SmapleAnswerTypeInfo信息
        void ICedts_PaperRepository.CreateSATI(CEDTS_SmapleAnswerTypeInfo SATI)
        {
            db.AddToCEDTS_SmapleAnswerTypeInfo(SATI);
            db.SaveChanges();
        }
        #endregion

        #region 更新CEDTS_SmapleAnswerTypeInfo信息
        void ICedts_PaperRepository.UpdataSATI(CEDTS_SmapleAnswerTypeInfo SATI)
        {
            var SATIInfo = (from m in db.CEDTS_SmapleAnswerTypeInfo where m.SATI_ID == SATI.SATI_ID select m).FirstOrDefault();
            db.ApplyCurrentValues(SATIInfo.EntityKey.EntitySetName, SATI);
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

        #region 更新CEDTS_UserAssessmentCount信息
        void ICedts_PaperRepository.UpdataUAC(CEDTS_UserAssessmentCount UAC)
        {
            var UACInfo = (from m in db.CEDTS_UserAssessmentCount where m.UserAssessmentCountID == UAC.UserAssessmentCountID select m).FirstOrDefault();
            db.ApplyCurrentValues(UACInfo.EntityKey.EntitySetName, UAC);
            db.SaveChanges();
        }
        #endregion

        #region  查询当前用户是否做过此类型试题
        int ICedts_PaperRepository.SelectUAINum(int? UserID, int? ItemID)
        {
            var UAINum = (from m in db.CEDTS_UserAnswerInfo where m.UserID == UserID && m.ItemTypeID == ItemID select m).FirstOrDefault();
            if (UAINum == null)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        #endregion

        #region 查询当前用户是否做过当前知识点
        int ICedts_PaperRepository.SelectUkINum(int? UserID, Guid? KPID)
        {
            var UKINum = (from m in db.CEDTS_UserKnowledgeInfomation where m.UserID == UserID && m.KnowledgePointID == KPID select m).FirstOrDefault();
            if (UKINum == null)
            {
                return 1;
            }
            else
            {
                return 2;
            }
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
                double Time = 0;
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
                        soundFile.InnerText = pt.SlpoList[j].Info.ItemID.ToString();
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
                        soundFile.InnerText = pt.LlpoList[j].Info.ItemID.ToString();
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
                        soundFile.InnerText = pt.RlpoList[j].Info.ItemID.ToString();
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
                        soundFile.InnerText = pt.LpcList[j].Info.ItemID.ToString();
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
                            int Num = (int)('A') + k;
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

        #region 更新试卷信息
        void ICedts_PaperRepository.UpdetPaper(CEDTS_Paper paper)
        {
            var PaperInfo = (from m in db.CEDTS_Paper where m.PaperID == paper.PaperID select m).FirstOrDefault();
            db.ApplyCurrentValues(PaperInfo.EntityKey.EntitySetName, paper);
            db.SaveChanges();
        }
        #endregion

        #region 获取知识点弱项
        List<string> ICedts_PaperRepository.SelectBadKnowledge(int userID, int Count)
        {
            List<Guid> list = db.CEDTS_UserKnowledgeInfomation.Where(p => p.UserID == userID).OrderBy(p => p.KP_MasterRate).Select(p => p.KnowledgePointID).ToList();
            List<string> tempList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                tempList.Add(list[i].ToString());
            }
            if (list.Count >= Count)
            {
                return tempList.Take(Count).ToList();
            }
            else
                return tempList;
        }
        #endregion

        #region 获取个人的知识点掌握率
        double ICedts_PaperRepository.SelectKP_MasterRate(string k, int userID)
        {
            List<Guid> kList = db.CEDTS_KnowledgePoints.Where(p => p.Title.Contains(k)).Select(p => p.KnowledgePointID).ToList();
            double M = 0.0;
            int j = 0;
            for (int i = 0; i < kList.Count; i++)
            {
                var ktemp = kList[i];
                var m = db.CEDTS_UserKnowledgeInfomation.Where(p => p.KnowledgePointID == ktemp && p.UserID == userID).Select(p => p.KP_MasterRate).FirstOrDefault();
                if (m != null)
                {
                    j++;
                    M += (double)m;
                }
            }
            return M / j;
        }
        #endregion

        #region 根据名称获取知识点
        string ICedts_PaperRepository.SelectKnowledgeIDs(string k)
        {
            List<Guid> kList = db.CEDTS_KnowledgePoints.Where(p => p.Title.Contains(k)).Select(p => p.KnowledgePointID).ToList();
            string kid = string.Empty;
            for (int i = 0; i < kList.Count; i++)
            {
                kid += kList[i].ToString() + ",";
            }
            kid = kid.Substring(0, kid.LastIndexOf(','));
            return kid;
        }
        #endregion

        #region 获取Parttype
        List<CEDTS_PartType> ICedts_PaperRepository.SelectPartType()
        {
            return db.CEDTS_PartType.ToList();
        }
        #endregion

        #region 获取ItemType
        List<CEDTS_ItemType> ICedts_PaperRepository.SelectItemTypeByPartTypeID(int PartTypeID)
        {
            return db.CEDTS_ItemType.Where(p => p.PartTypeID == PartTypeID).ToList();
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

        #region 根据用户ID查询最近5次答卷信息
        PagedList<TestRecord> ICedts_PaperRepository.TestRecord(int? id, int UserID)
        {
            IQueryable<TestRecord> RecordInfo = from m in db.CEDTS_Test
                                                from s in db.CEDTS_Paper
                                                where m.UserID == UserID && m.PaperID == s.PaperID
                                                orderby m.FinishDate descending
                                                select new TestRecord
                             {
                                 TestID = m.TestID,
                                 PaperName = s.Title,
                                 FinishTime = m.FinishDate.Value,
                                 TotalScore = s.Score.Value,
                                 Score = m.TotalScore.Value,
                                 isFinish = m.IsFinished.Value,
                                 Mark = m.Remark,
                                 TestType = s.State.Value 
                             };
            PagedList<TestRecord> Record = RecordInfo.ToPagedList(id ?? 1, 10);
            return Record;
        }
        #endregion

        #region 根据班级ID获取发布的作业信息
        List<CEDTS_PaperAssignClass> ICedts_PaperRepository.SelectAssignedPaper(Guid ClassID)
        {
            return db.CEDTS_PaperAssignClass.Where(m => m.ClassID == ClassID).ToList();
        }
        #endregion

        #region 根据PaperID号获取测试信息
        int ICedts_PaperRepository.SelectTestByPaperID(Guid PaperID, int UserID)
        {
            var UA = (from m in db.CEDTS_Test where m.PaperID == PaperID && m.UserID == UserID select m).FirstOrDefault();
            if (UA == null)
            {
                return 1;
            }
            else
            {
                if (UA.IsFinished == false)
                {
                    return 2;
                } 
                else
                {
                    return 3;
                }
            }
        }
        #endregion

        #region 根据PaperID获取TestID
        Guid ICedts_PaperRepository.SelectTestIDByPaperID(Guid PaperID, int UserID)
        {
            return db.CEDTS_Test.Where(m => m.PaperID == PaperID && m.UserID == UserID).Select(m => m.TestID).FirstOrDefault();
        }
        #endregion

        #region 根据用户班级ID查询未做的作业
        PagedList<AssignedTask> ICedts_PaperRepository.AssignedTaskPaged(int? id, List<AssignedTask> AssignedTask)
        {
            IQueryable<AssignedTask> IAssignedTask = AssignedTask.AsQueryable();
            PagedList<AssignedTask> Task = IAssignedTask.ToPagedList(id ?? 1, 10);
            return Task;
        }
        #endregion

        #region 根据用户ID查询
        Insufficient ICedts_PaperRepository.SelectInsufficient(int UserID)
        {
            var InsufficientInfo = (from m in db.CEDTS_UserKnowledgeInfomation orderby m.CorrectRate where m.CorrectRate < 0.6 && m.UserID == UserID select m).ToList();
            //词汇能力
            string SSVocabulary = "e7607791-7c00-4643-86a3-0690f73e545c";
            Guid GSVocabulary = Guid.Parse(SSVocabulary);
            var SVocabulary = (from m in db.CEDTS_KnowledgePoints where m.UperKnowledgeID == GSVocabulary select m.KnowledgePointID).ToList();
            //语法能力
            string SSGrammar = "c910b799-a390-4374-8a35-a277be4214f0";
            Guid GSGrammar = Guid.Parse(SSGrammar);
            var SGrammar = (from m in db.CEDTS_KnowledgePoints where m.UperKnowledgeID == GSGrammar select m.KnowledgePointID).ToList();
            //理解能力
            string SSComprehend = "87dd357f-2c1d-48b0-ad2f-39b256885255";
            Guid GSComprehend = Guid.Parse(SSComprehend);
            var SComprehend = (from m in db.CEDTS_KnowledgePoints where m.UperKnowledgeID == GSComprehend select m.KnowledgePointID).ToList();
            Regex regex = new Regex(@"[.\d]");//去除数字

            //用户词汇能力不足之处
            string UVocabulary = string.Empty;

            foreach (var Insufficient in InsufficientInfo)
            {
                if (SVocabulary.Contains(Insufficient.KnowledgePointID))
                {
                    var Title = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Insufficient.KnowledgePointID select m.Title).FirstOrDefault();
                    Title = regex.Replace(Title, "");

                    UVocabulary += Title + ",";
                }
            }
            Insufficient i = new Insufficient();
            i.Vocabulary = UVocabulary;
            //用户语法能力不足之处
            string UGrammar = string.Empty;
            foreach (var Insufficient in InsufficientInfo)
            {
                if (SGrammar.Contains(Insufficient.KnowledgePointID))
                {
                    var Title = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Insufficient.KnowledgePointID select m.Title).FirstOrDefault();
                    Title = regex.Replace(Title, "");

                    UGrammar += Title + ",";
                }
            }
            i.Grammar = UGrammar;
            //用户理解能力不足之处
            string UComprehend = string.Empty;
            foreach (var Insufficient in InsufficientInfo)
            {
                if (SComprehend.Contains(Insufficient.KnowledgePointID))
                {
                    var Title = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Insufficient.KnowledgePointID select m.Title).FirstOrDefault();
                    Title = regex.Replace(Title, "");
                    UComprehend += Title + ",";
                }
            }
            i.Comprehend = UComprehend;

            return i;
        }
        #endregion

        List<CEDTS_TestAnswer> ICedts_PaperRepository.GetTestAnswer(Guid TestID)
        {
            return db.CEDTS_TestAnswer.Where(p => p.TestID == TestID).ToList();
        }

        #region 根据TestID查询PaperID
        Guid ICedts_PaperRepository.SelectPaperID(Guid TestID)
        {
            var PaperID = (from m in db.CEDTS_Test where m.TestID == TestID select m.PaperID).FirstOrDefault();
            return PaperID;
        }
        #endregion

        #region 用户知识点掌握率
        List<Knowledge> ICedts_PaperRepository.UserKpMaster(Guid TestID, string UserName)
        {
            List<Knowledge> KnowList = new List<Knowledge>();
            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
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

        #region 用户题型正确率
        ItemScale ICedts_PaperRepository.UserItemInfo(Guid TestID, string UserName)
        {
            ItemScale ItemInfo = new ItemScale();
            ItemInfo.Fortysix = new List<double>();
            ItemInfo.SScale = new List<double>();
            ItemInfo.UScale = new List<double>();

            ItemInfo.Fortysix.Add(12.4);
            ItemInfo.Fortysix.Add(9.87);
            ItemInfo.Fortysix.Add(8.64);
            ItemInfo.Fortysix.Add(12.4);
            ItemInfo.Fortysix.Add(13.6);
            ItemInfo.Fortysix.Add(12.4);
            ItemInfo.Fortysix.Add(18.5);
            ItemInfo.Fortysix.Add(12.4);
            ItemInfo.Fortysix.Add(12.4);

            ItemInfo.UScale.Add(0);
            ItemInfo.UScale.Add(0);
            ItemInfo.UScale.Add(0);
            ItemInfo.UScale.Add(0);
            ItemInfo.UScale.Add(0);
            ItemInfo.UScale.Add(0);
            ItemInfo.UScale.Add(0);
            ItemInfo.UScale.Add(0);
            ItemInfo.UScale.Add(0);

            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
            var UItem = (from m in db.CEDTS_TestAnswerTypeInfo where m.UserID == UserID && m.TestID == TestID orderby m.ItemTypeID select m);

            foreach (var u in UItem)
            {
                double u1 = 0.0;
                switch (u.ItemTypeID)
                {
                    case 1:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[0];
                        break;
                    case 2:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[1];
                        break;
                    case 3:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[2];
                        break;
                    case 4:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[3];
                        break;
                    case 5:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[4];
                        break;
                    case 6:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[5];
                        break;
                    case 7:
                        u1 = u.CorrectRate.Value * 1.5 * ItemInfo.Fortysix[6];
                        break;
                    case 8:
                        u1 = u.CorrectRate.Value * 0.5 * ItemInfo.Fortysix[7];
                        break;
                    case 9:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[8];
                        break;
                    default:
                        break;
                }
                string uu = u1 + "";
                if (uu.Length > 4)
                {
                    uu = uu.Substring(0, uu.IndexOf('.') + 3);
                }
                ItemInfo.UScale[u.ItemTypeID.Value - 1] = double.Parse(uu);
            }
            return ItemInfo;
        }
        #endregion

        #region 错题集合
        List<ErrorQuestion> ICedts_PaperRepository.ErrorQuestion(Guid TestID)
        {
            List<ErrorQuestion> Error = new List<ErrorQuestion>();
            var Info = from m in db.CEDTS_TestAnswer
                       where m.TestID == TestID && m.IsRight == false
                       from s in db.CEDTS_Question
                       where m.QuestionID == s.QuestionID
                       select new ErrorQuestion
                       {
                           QuestionNum = m.Number,
                           SAnswer = m.Answer,
                           UAnswer = m.UserAnswer,
                           Analyze = s.Analyze
                       };
            Error = Info.ToList();
            return Error;
        }
        #endregion

        ErrorList ICedts_PaperRepository.SelectError(Guid TestID, int UserID)
        {
            ErrorList Error = new ErrorList();
            var Score = (from m in db.CEDTS_Test where m.TestID == TestID && m.UserID == UserID select m.TotalScore).FirstOrDefault();
            var PaperID = (from m in db.CEDTS_Test where m.TestID == TestID select m.PaperID).FirstOrDefault();
            var TotalScore = (from m in db.CEDTS_Paper where m.PaperID == PaperID select m.Score).FirstOrDefault();
            var Proposal = (from m in db.CEDTS_Test where m.TestID == TestID && m.UserID == UserID select m.Remark).FirstOrDefault();
            var TimeInfo = (from m in db.CEDTS_Test where m.TestID == TestID && m.UserID == UserID select m).FirstOrDefault();
            int time = Convert.ToInt32((TimeInfo.FinishDate.Value - TimeInfo.StartDate.Value).TotalSeconds);
            Error.Score = Score.Value;
            Error.Proposal = Proposal;
            Error.Time = time;
            Error.TotalScore = TotalScore.Value;
            return Error;
        }

        #region 知识点列表
        UserKnowledgeInfo ICedts_PaperRepository.KpList(Guid TestID, string UserName)
        {
            UserKnowledgeInfo UserKpInfo = new UserKnowledgeInfo();
            UserKpInfo.CorrectRate = new List<double>();
            UserKpInfo.KPMaster = new List<double>();
            UserKpInfo.KpName = new List<string>();
            UserKpInfo.Num = new List<int>();
            UserKpInfo.Time = new List<int>();

            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
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

        #region 题型列表
        ItemInfo ICedts_PaperRepository.ItemList(Guid TestID, string UserName)
        {
            ItemInfo Item = new ItemInfo();
            Item.CorrectRate = new List<double>();
            Item.ItemName = new List<string>();
            Item.Num = new List<int>();
            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
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

        #region 查看答案选项内容
        string ICedts_PaperRepository.AnswerContent(Guid id, string Content)
        {
            string AnswerContent = string.Empty;
            var Choice = (from m in db.CEDTS_Question where m.QuestionID == id select m).FirstOrDefault();
            if (Choice.ChooseA == null || Choice.ChooseA == "")
            {
                var ChoiceInfo = (from n in db.CEDTS_Expansion where n.AssessmentItemID == Choice.AssessmentItemID select n).FirstOrDefault();
                if (Content == "A")
                {
                    AnswerContent = ChoiceInfo.ChoiceA;
                }
                if (Content == "B")
                {
                    AnswerContent = ChoiceInfo.ChoiceB;
                }
                if (Content == "C")
                {
                    AnswerContent = ChoiceInfo.ChoiceC;
                }
                if (Content == "D")
                {
                    AnswerContent = ChoiceInfo.ChoiceD;
                }
                if (Content == "E")
                {
                    AnswerContent = ChoiceInfo.ChoiceE;
                }
                if (Content == "F")
                {
                    AnswerContent = ChoiceInfo.ChoiceF;
                }
                if (Content == "G")
                {
                    AnswerContent = ChoiceInfo.ChoiceG;
                }
                if (Content == "H")
                {
                    AnswerContent = ChoiceInfo.ChoiceH;
                }
                if (Content == "I")
                {
                    AnswerContent = ChoiceInfo.ChoiceI;
                }
                if (Content == "J")
                {
                    AnswerContent = ChoiceInfo.ChoiceJ;
                }
                if (Content == "K")
                {
                    AnswerContent = ChoiceInfo.ChoiceK;
                }
                if (Content == "L")
                {
                    AnswerContent = ChoiceInfo.ChoiceL;
                }
                if (Content == "M")
                {
                    AnswerContent = ChoiceInfo.ChoiceM;
                }
                if (Content == "N")
                {
                    AnswerContent = ChoiceInfo.ChoiceN;
                }
                if (Content == "O")
                {
                    AnswerContent = ChoiceInfo.ChoiceO;
                }
            }
            else
            {
                if (Content == "A")
                {
                    AnswerContent = Choice.ChooseA;
                }
                if (Content == "B")
                {
                    AnswerContent = Choice.ChooseB;
                }
                if (Content == "C")
                {
                    AnswerContent = Choice.ChooseC;
                }
                if (Content == "D")
                {
                    AnswerContent = Choice.ChooseD;
                }
            }
            return AnswerContent;
        }
        #endregion

        #region 获取试卷信息集合
        List<CEDTS_Paper> ICedts_PaperRepository.GetPapers()
        {
            if (db.CEDTS_Paper.Count() < 5)
                return db.CEDTS_Paper.ToList();
            else
                return db.CEDTS_Paper.OrderByDescending(p => p.CreateTime).Take(5).ToList();
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

        #region 根据TestID查询PaperID
        Guid ICedts_PaperRepository.SelectPaperCount(Guid id)
        {
            var count = (from m in db.CEDTS_Test where m.TestID == id select m.PaperID).FirstOrDefault();
            return count;
        }
        #endregion

        #region 根据TestID查询PaperName
        string ICedts_PaperRepository.SelectPaperName(Guid id)
        {
            var PaperID = (from m in db.CEDTS_Test where m.TestID == id select m.PaperID).FirstOrDefault();
            var PaperName = (from m in db.CEDTS_Paper where m.PaperID == PaperID select m.Title).FirstOrDefault();
            return PaperName;
        }
        #endregion

        #region 查询同一试卷成绩分布情况
        SamePaper ICedts_PaperRepository.SelectSamePaperList(Guid id, int UserID)
        {
            SamePaper SP = new SamePaper();
            SP.PaperName = new List<string>();
            SP.Score = new List<double>();
            var PaperID = (from m in db.CEDTS_Test where m.TestID == id select m.PaperID).FirstOrDefault();
            var PaperList = (from s in db.CEDTS_Test where s.PaperID == PaperID && s.UserID == UserID select s.TotalScore).ToList();
            var PaperName = (from n in db.CEDTS_Paper where n.PaperID == PaperID select n.Title).FirstOrDefault();
            foreach (var score in PaperList)
            {
                SP.Score.Add(score.Value);
                SP.PaperName.Add(PaperName);
            }
            return SP;
        }
        #endregion

        #region 查询试卷练习次数是否超过两次
        string ICedts_PaperRepository.SelectCount(Guid id, int UserID)
        {
            var PaperID = (from m in db.CEDTS_Test where m.TestID == id select m.PaperID).FirstOrDefault();
            var count = (from n in db.CEDTS_Test where n.PaperID == PaperID && n.UserID == UserID select n).ToList();
            if (count.Count > 1)
            {
                return "1";
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 得分率分析
        string ICedts_PaperRepository.RateAnalysis(int id)
        {
            var count = (from m in db.CEDTS_Test where m.UserID == id orderby m.FinishDate select m).ToList();
            double TotleScore = 0;
            if (count.Count > 1)
            {
                foreach (var s in count)
                {
                    var Ts = (from m in db.CEDTS_Paper where m.PaperID == s.PaperID select m.Score).FirstOrDefault();//试卷总分
                    TotleScore += s.TotalScore.Value / Ts.Value;//得分率
                }
                double AvgScore = TotleScore / count.Count * 100;
                string Avg = AvgScore + "";
                if (Avg.Length > 4)
                {
                    AvgScore = double.Parse(Avg.Substring(0, 4));
                }
                string rate = string.Empty;
                int sol = Convert.ToInt32(AvgScore);
                if (sol >= 90)
                {
                    rate = "很高，";
                }
                if (80 <= sol && sol < 90)
                {
                    rate = "较高，";
                }
                if (65 <= sol && sol < 80)
                {
                    rate = "一般，";
                }
                if (50 <= sol && sol < 65)
                {
                    rate = "较低，";
                }
                if (sol < 50)
                {
                    rate = "很低，（不是特别理想，不要灰心，要勤加练习哦 ）<img src='../../Images/3.2.gif'></img>,";
                }
                double AvgRate = 0;
                List<double> ListRate = new List<double>();
                double wave = 0;
                int Num = 0;
                if (count.Count > 2)
                {
                    Num = 3;
                }
                else
                {
                    Num = 2;
                }
                if (count.Count > 2)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Guid Gid = count[i].PaperID;
                        var ts = (from m in db.CEDTS_Paper where m.PaperID == Gid select m.Score).FirstOrDefault();
                        AvgRate += count[i].TotalScore.Value / ts.Value;
                        ListRate.Add(AvgRate);
                    }
                    AvgRate = AvgRate / 3;

                    wave = ((ListRate[0] - AvgRate) * (ListRate[0] - AvgRate) + (ListRate[1] - AvgRate) * (ListRate[1] - AvgRate) + (ListRate[2] - AvgRate) * (ListRate[2] - AvgRate)) / 3;
                }
                else if (count.Count == 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Guid Gid = count[i].PaperID;
                        var ts = (from m in db.CEDTS_Paper where m.PaperID == Gid select m.Score).FirstOrDefault();
                        AvgRate += count[i].TotalScore.Value / ts.Value;
                        ListRate.Add(AvgRate);
                    }
                    AvgRate = AvgRate / 2;
                    wave = ((ListRate[0] - AvgRate) * (ListRate[0] - AvgRate) + (ListRate[1] - AvgRate) * (ListRate[1] - AvgRate)) / 2;
                }

                string Content1 = string.Empty;
                if (AvgRate > AvgScore)
                {
                    Content1 = "明显高于平均得分率";
                }
                else
                {
                    Content1 = "明显低于平均得分率，有所波动";
                }
                string WaveContent = string.Empty;
                double Limit1 = double.Parse("2") / double.Parse("27");

                double Limit2 = double.Parse("4") / double.Parse("27");

                if (wave > 0 && wave <= Limit1)
                {
                    WaveContent = "稳定";
                }
                if (wave > Limit1 && wave <= Limit2)
                {
                    WaveContent = "较小";
                }
                if (wave > Limit2)
                {
                    WaveContent = "较大";
                }

                string info = "&nbsp;&nbsp;<p>您" + count.Count + "次练习的平均得分率为" + AvgScore + "%，得分率" + rate + ",再接再励哦 <img src='../../Images/3.1.gif'></img>。</p><p>&nbsp;&nbsp;最近" + Num + "次的得分率" + Content1 + "，成绩波动" + WaveContent + "，勤加练习，祝您取得更好的成绩！<img src='../../Images/3.3.gif'></img></p><br/><strong>Tip：</strong>如果您想查看最近试卷列表中已完成的单份试卷的成绩走势等详细报告，点击相应试卷操作栏中的“查看详细报告”按钮！";

                return info;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region 知识点分析
        string ICedts_PaperRepository.KpAnalysis(int id)
        {
            var Kp = (from m in db.CEDTS_UserKnowledgeInfomation where m.UserID == id orderby m.KP_MasterRate select m).Take(3);
            var Weak = (from m in db.CEDTS_UserKnowledgeInfomation where m.UserID == id orderby m.KP_MasterRate select m).ToList();
            if (Weak.Count != 0)
            {
                List<Guid> GidOne = new List<Guid>();
                List<Guid> GidOne1 = new List<Guid>();
                List<Guid> GidTwo = new List<Guid>();
                List<Guid> GidThree = new List<Guid>();
                string Content = string.Empty;
                foreach (var s in Kp)
                {
                    GidOne1.Add(s.KnowledgePointID);
                    if (s.KP_MasterRate < 0.6)
                    {
                        GidOne.Add(s.KnowledgePointID);
                    }
                }

                foreach (var n in Weak)
                {
                    var Compare = (from a in db.CEDTS_SampleKnowledgeInfomation where a.KnowledgePointID == n.KnowledgePointID select a.KP_MasterRate).FirstOrDefault();
                    if (n.KP_MasterRate <= Compare)
                    {
                        GidTwo.Add(n.KnowledgePointID);
                    }
                }


                var UserAllKp = (from m in db.CEDTS_UserKnowledgeInfomation where m.UserID == id select m.KnowledgePointID).ToList();
                var SysAllKp = (from m in db.CEDTS_KnowledgePoints where m.Level == 2 select m.KnowledgePointID).ToList();

                foreach (var l in SysAllKp)
                {
                    if (!UserAllKp.Contains(l))
                    {
                        GidThree.Add(l);
                    }
                }


                if (GidOne.Count > 0)
                {
                    for (int i = 0; i < GidOne.Count; i++)
                    {
                        Guid gid = GidOne[i];
                        var KpName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == gid select m.Title).FirstOrDefault();
                        Regex regex = new Regex(@"[.\d]");//去除数字
                        KpName = regex.Replace(KpName, "");
                        if (i == GidOne.Count - 1)
                        {
                            Content += KpName + "等知识点的掌握率低于60%，不太理想哦，要努力啦！<br/>";
                        }
                        else
                        {
                            Content += KpName + "，";
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < GidOne1.Count; i++)
                    {
                        Guid gid = GidOne1[i];
                        var KpName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == gid select m.Title).FirstOrDefault();
                        Regex regex = new Regex(@"[.\d]");//去除数字
                        KpName = regex.Replace(KpName, "");
                        if (i == GidOne1.Count - 1)
                        {
                            Content += KpName + "知识点是您的薄弱项，您有待在这三方面提高哦！<br/>";
                        }
                        else
                        {
                            Content += KpName + "，";
                        }
                    }
                }

                if (GidTwo.Count > 0)
                {
                    for (int i = 0; i < GidTwo.Count; i++)
                    {
                        Guid gid = GidTwo[i];
                        var KpName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == gid select m.Title).FirstOrDefault();
                        Regex regex = new Regex(@"[.\d]");//去除数字
                        KpName = regex.Replace(KpName, "");
                        if (i == GidTwo.Count - 1)
                        {
                            Content += KpName + "等知识点低于本系统用户的平均水平，你要迎头赶上啦！<br/>";
                        }
                        else
                        {
                            Content += KpName + "，";
                        }
                    }
                }
                else
                {
                    Content += "您的所有的知识点的掌握率都高于本系统用户平均水平，再接再励！<br/>";
                }

                if (GidThree.Count > 0)
                {
                    Content += "您在";
                    for (int i = 0; i < GidThree.Count; i++)
                    {
                        Guid gid = GidThree[i];
                        var KpName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == gid select m.Title).FirstOrDefault();
                        Regex regex = new Regex(@"[.\d]");//去除数字
                        KpName = regex.Replace(KpName, "");
                        if (i == GidThree.Count - 1)
                        {
                            Content += KpName + "等几个知识点方面尚未进行练习，建议您尽快使用 “按照知识点组卷的方式”<br/>（点击自定义练习按钮进行弹出按照知识点组卷进行练习的框）”进行练习；<br/>";
                        }
                        else
                        {
                            Content += KpName + "，";
                        }
                    }
                }
                Content += "Tip：如果您想查看最近试卷列表中已完成的单份试卷的成绩走势等详细报告，点击相应试卷操作栏中的“查看详细报告”按钮！";
                return Content;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region 单份试卷得分率分析
        string ICedts_PaperRepository.SingleRateAnalysis(Guid id, int UserID)
        {
            var PaperID = (from m in db.CEDTS_Test where m.TestID == id select m.PaperID).FirstOrDefault();
            var TestList = (from n in db.CEDTS_Test where n.PaperID == PaperID orderby n.FinishDate select n).ToList();
            double TotleScoreRate = 0;
            foreach (var s in TestList)
            {
                var Score = (from m in db.CEDTS_Paper where m.PaperID == s.PaperID select m.Score).FirstOrDefault();
                TotleScoreRate += s.TotalScore.Value / Score.Value;
            }
            double AvgScoreRate = TotleScoreRate / TestList.Count;
            string AvgScore1 = AvgScoreRate + "";
            if (AvgScore1.Length > 5)
            {
                AvgScoreRate = double.Parse(AvgScore1.Substring(0, 5));
            }
            string ment = string.Empty;
            if (AvgScoreRate >= 0.9)
            {
                ment = "很高";
            }
            if (AvgScoreRate >= 0.8 && AvgScoreRate < 0.9)
            {
                ment = "较高";
            }
            if (AvgScoreRate >= 0.65 && AvgScoreRate < 0.8)
            {
                ment = "一般";
            }
            if (AvgScoreRate >= 0.5 && AvgScoreRate < 0.65)
            {
                ment = "较低";
            }
            if (AvgScoreRate < 0.50)
            {
                ment = "很低";
            }
            int Num = 0;
            double wave = 0;
            if (TestList.Count > 3)
            {
                Num = 3;
                double Avg = (TestList[0].TotalScore.Value + TestList[1].TotalScore.Value + TestList[2].TotalScore.Value) / 3;
                wave = ((TestList[0].TotalScore.Value - Avg) * (TestList[0].TotalScore.Value - Avg) + (TestList[1].TotalScore.Value - Avg) * (TestList[1].TotalScore.Value - Avg) + (TestList[2].TotalScore.Value - Avg) * (TestList[2].TotalScore.Value - Avg)) / 3;

            }
            else
            {
                Num = TestList.Count;
                double Avg = (TestList[0].TotalScore.Value + TestList[1].TotalScore.Value) / 2;
                wave = ((TestList[0].TotalScore.Value - Avg) * (TestList[0].TotalScore.Value - Avg) + (TestList[1].TotalScore.Value - Avg) * (TestList[1].TotalScore.Value - Avg)) / 2;
            }
            string WaveContent = string.Empty;
            double Limit1 = double.Parse("2") / double.Parse("27");
            double Limit2 = double.Parse("4") / double.Parse("27");
            if (wave > 0 && wave <= Limit1)
            {
                WaveContent = "稳定";
            }
            if (wave > Limit1 && wave <= Limit2)
            {
                WaveContent = "较小";
            }
            if (wave > Limit2)
            {
                WaveContent = "较大";
            }
            string Content = "您" + TestList.Count + "次练习的平均得分率为" + AvgScoreRate + "，得分" + ment + "，再接再励哦 <img src='../../Images/3.2.gif'></img>,（不是特别理想，不要灰心，要勤加练习哦 <img src='../../Images/3.1.gif'></img>）最近" + Num + "次的得分成绩波动" + wave + "，勤加练习，祝您取得更好的成绩！ <img src='../../Images/3.3.gif'></img>";
            return Content;
        }
        #endregion

        #region 单份试卷知识点分析
        string ICedts_PaperRepository.SingleKpAnalysis(Guid id, int UserID)
        {
            var Kp = (from m in db.CEDTS_UserKnowledgeInfomation where m.UserID == UserID orderby m.KP_MasterRate select m).Take(3);
            var PaperID = (from m in db.CEDTS_Test where m.TestID == id select m.PaperID).FirstOrDefault();
            var KpList = (from m in db.CEDTS_TestAnswerKnowledgePoint where m.PaperID == PaperID && m.UserID == UserID select m).ToList();
            var Weak = (from m in db.CEDTS_TestAnswerKnowledgePoint where m.PaperID == PaperID && m.UserID == UserID orderby m.KP_MasterRate select m).ToList();
            List<Guid> GidOne = new List<Guid>();
            List<Guid> GidOne1 = new List<Guid>();
            List<Guid> GidTwo = new List<Guid>();
            List<Guid> GidThree = new List<Guid>();
            string Content = string.Empty;
            foreach (var s in KpList)
            {
                if (!GidOne1.Contains(s.KnowledgePointID))
                {
                    GidOne1.Add(s.KnowledgePointID);
                    if (s.KP_MasterRate < 0.6)
                    {
                        GidOne.Add(s.KnowledgePointID);
                    }
                }
            }

            foreach (var n in Weak)
            {
                var Compare = (from a in db.CEDTS_SampleKnowledgeInfomation where a.KnowledgePointID == n.KnowledgePointID select a.KP_MasterRate).FirstOrDefault();
                if (n.KP_MasterRate < Compare)
                {
                    if (!GidTwo.Contains(n.KnowledgePointID))
                    {
                        GidTwo.Add(n.KnowledgePointID);
                    }
                }
            }


            var UserAllKp = (from m in db.CEDTS_UserKnowledgeInfomation where m.UserID == UserID select m.KnowledgePointID).ToList();
            var SysAllKp = (from m in db.CEDTS_KnowledgePoints where m.Level == 2 select m.KnowledgePointID).ToList();

            foreach (var l in SysAllKp)
            {
                if (!UserAllKp.Contains(l))
                {
                    GidThree.Add(l);
                }
            }


            if (GidOne.Count > 0)
            {
                for (int i = 0; i < GidOne.Count; i++)
                {
                    Guid gid = GidOne[i];
                    var KpName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == gid select m.Title).FirstOrDefault();
                    Regex regex = new Regex(@"[.\d]");//去除数字
                    KpName = regex.Replace(KpName, "");
                    if (i == GidOne.Count - 1)
                    {
                        Content += KpName + "等知识点的掌握率低于60%，不太理想哦，要努力啦！<br/>";
                    }
                    else
                    {
                        Content += KpName + "，";
                    }
                }
            }
            else
            {
                for (int i = 0; i < GidOne1.Count; i++)
                {
                    Guid gid = GidOne1[i];
                    var KpName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == gid select m.Title).FirstOrDefault();
                    Regex regex = new Regex(@"[.\d]");//去除数字
                    KpName = regex.Replace(KpName, "");
                    if (i == GidOne1.Count - 1)
                    {
                        Content += KpName + "知识点是您的薄弱项，您有待在这三方面提高哦！<br/>";
                    }
                    else
                    {
                        Content += KpName + "，";
                    }
                }
            }

            if (GidTwo.Count > 0)
            {
                for (int i = 0; i < GidTwo.Count; i++)
                {
                    Guid gid = GidTwo[i];
                    var KpName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == gid select m.Title).FirstOrDefault();
                    Regex regex = new Regex(@"[.\d]");//去除数字
                    KpName = regex.Replace(KpName, "");
                    if (i == GidTwo.Count - 1)
                    {
                        Content += KpName + "等知识点低于本系统用户的平均水平，你要迎头赶上啦！<br/>";
                    }
                    else
                    {
                        Content += KpName + "，";
                    }
                }
            }
            else
            {
                Content += "您的所有的知识点的掌握率都高于本系统用户平均水平，再接再励！<br/>";
            }

            if (GidThree.Count > 0)
            {
                Content += "您在";
                for (int i = 0; i < GidThree.Count; i++)
                {
                    Guid gid = GidThree[i];
                    var KpName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == gid select m.Title).FirstOrDefault();
                    Regex regex = new Regex(@"[.\d]");//去除数字
                    KpName = regex.Replace(KpName, "");
                    if (i == GidThree.Count - 1)
                    {
                        Content += KpName + "等几个知识点方面尚未进行练习，建议您尽快使用 “按照知识点组卷的方式”<br/>（点击自定义练习按钮进行弹出按照知识点组卷进行练习的框）”进行练习；<br/>";
                    }
                    else
                    {
                        Content += KpName + "，";
                    }
                }
            }
            Content += "Tip：如果您想查看单份试卷的成绩走势等详细报告，点击对应试卷详细报告即可！";
            return Content;
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

        #region 查看当前用户是否练习过试卷
        string ICedts_PaperRepository.SelectPaper(int id)
        {
            var info = (from m in db.CEDTS_Test where m.UserID == id select m).FirstOrDefault();
            if (info == null)
            {
                return "1";
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region 查询当前用户角色
        string ICedts_PaperRepository.SelectRole(string name)
        {
            var Role = (from m in db.CEDTS_User where m.UserAccount == name select m.Role).FirstOrDefault();
            return Role;
        }
        #endregion

        #region 根据testID获取test对象
        CEDTS_Test ICedts_PaperRepository.SelectTestInfo(Guid testID)
        {
            return db.CEDTS_Test.Where(p => p.TestID == testID).FirstOrDefault();
        }
        #endregion

        #region 根据测试ID获取试题信息集合
        List<CEDTS_tempTestAnswer> ICedts_PaperRepository.getTempTestAnswerList(Guid testID)
        {
            return db.CEDTS_tempTestAnswer.Where(p => p.TestID == testID).OrderBy(p => p.Number).ToList();
        }
        #endregion

        #region 根据测试ID获取答题时间表对象集合
        List<CEDTS_tempAssessmentItemTime> ICedts_PaperRepository.getTempAssessmentTime(Guid testID)
        {
            return db.CEDTS_tempAssessmentItemTime.Where(p => p.TestID == testID).OrderBy(p => p.Number).ToList();
        }
        #endregion

        #region 試卷暫存數據
        void ICedts_PaperRepository.CreateTempTestAnswer(CEDTS_tempTestAnswer tempTestAnswer)
        {
            db.AddToCEDTS_tempTestAnswer(tempTestAnswer);
            db.SaveChanges();
        }
        #endregion

        #region 試卷暫存時間
        void ICedts_PaperRepository.CreateTestTime(CEDTS_tempAssessmentItemTime TempTestTime)
        {
            db.AddToCEDTS_tempAssessmentItemTime(TempTestTime);
            db.SaveChanges();
        }
        #endregion

        //#region 試卷暫存更新
        //void ICedts_PaperRepository.UpdateTest(Guid TestID, bool IsFinish, float TotalTime, DateTime FinishDate)
        //{
        //    db.UpdataPaper(TestID, IsFinish, TotalTime, FinishDate);
        //    db.SaveChanges();
        //}
        //#endregion

        #region 刪除暫存試卷時間
        void ICedts_PaperRepository.DeleteTempTime(Guid TestID)
        {
            var TimeList = (from m in db.CEDTS_tempAssessmentItemTime where m.TestID == TestID select m).ToList();
            foreach (var t in TimeList)
            {
                db.DeleteObject(t);
            }
            db.SaveChanges();
        }
        #endregion

        void ICedts_PaperRepository.DeleteTempTest(Guid TestID)
        {
            var TestList = (from m in db.CEDTS_tempTestAnswer where m.TestID == TestID select m).ToList();
            foreach (var t in TestList)
            {
                db.DeleteObject(t);
            }
            db.SaveChanges();
        }

        List<Array> ICedts_PaperRepository.getAllKnowledgeRate()
        {
            List<Array> kList = new List<Array>();
            var o = (from m in db.CEDTS_UserKnowledgeInfomation orderby m.UserID select m).ToList();
            foreach (var o1 in o)
            {
                //  Array a = new Array<float>(2);

            }
            return kList;
        }

        void ICedts_PaperRepository.insertStatusToTable(CEDTS_LearnerStatus LearnerStatus)
        {
            db.AddToCEDTS_LearnerStatus(LearnerStatus);
            db.SaveChanges();

        }

        string ICedts_PaperRepository.selectStatus(int userId)
        {
            var selectSql = (from m in db.CEDTS_LearnerStatus where m.userId == userId select m).FirstOrDefault();
            if (selectSql == null)
                return "";
            else
            {
                return selectSql.status;
            }
        }
    }
}