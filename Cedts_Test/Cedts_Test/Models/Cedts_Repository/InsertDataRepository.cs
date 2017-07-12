using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cedts_Test.Models;
using System.Data;
using System.Reflection;

namespace Cedts_Test.Models
{
    public class InsertDataRepository : IInsertDataRepository
    {
        CedtsEntities db;
        public InsertDataRepository()
        {
            db = new CedtsEntities();
        }

        string IInsertDataRepository.KpID(Guid ID)
        {
            string SkPID = string.Empty;
            var KpIDList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == ID select m).ToList();
            foreach (var KpID in KpIDList)
            {
                SkPID = SkPID + KpID.KnowledgePointID + '|';
            }
            SkPID = SkPID.Substring(0, SkPID.LastIndexOf('|'));
            return SkPID;
        }

        DataTable IInsertDataRepository.GetTable()
        {
            List<QuestionStatistics> qsList = new List<QuestionStatistics>();
            var testIDList = db.CEDTS_Test.Select(p => p.TestID).Distinct().ToList();
            foreach (var testID in testIDList)
            {
                var testAnswerList = db.CEDTS_TestAnswer.Where(p => p.TestID == testID).OrderBy(p => p.Number).ToList();
                foreach (var testAnswer in testAnswerList)
                {
                    QuestionStatistics qs = new QuestionStatistics();
                    qs.QuestionID = testAnswer.QuestionID;
                    qs.IsRight = testAnswer.IsRight.Value;
                    qs.AverageTime = testAnswer.ItemAnswerTime.Value;
                    var questionID = qs.QuestionID;
                    var question = db.CEDTS_Question.Where(p => p.QuestionID == questionID).FirstOrDefault();
                    var knowledges = db.CEDTS_QuestionKnowledge.Where(p => p.QuestionID == questionID).Select(p => p.KnowledgePointID).ToList();
                    qs.Score = question.Score.Value;
                    foreach (var k in knowledges)
                    {
                        var knowledge = db.CEDTS_KnowledgePoints.Where(p => p.KnowledgePointID == k).FirstOrDefault();
                        qs.KnowledgePointID += knowledge.KnowledgePointID.ToString() + ",";
                        qs.Title += knowledge.Title + ",";
                    }
                    qsList.Add(qs);
                }
                qsList.Add(new QuestionStatistics());
            }
            return CollectionHelper.ConvertTo<QuestionStatistics>(qsList); 
        }

        double IInsertDataRepository.selectQuestionKp(Guid KpID, Guid TestID)
        {
            List<Guid> QuestionList = new List<Guid>();
            double Time = 0;
            int Count = 0;
            var QuestionInfo = (from m in db.CEDTS_QuestionKnowledge where m.KnowledgePointID == KpID select m);
            foreach (var QuestionID in QuestionInfo)
            {
                QuestionList.Add(QuestionID.QuestionID);
            }
            var TestQuestionInfo = (from m in db.CEDTS_TestAnswer where m.TestID == TestID select m);
            foreach (var TestQuestion in TestQuestionInfo)
            {
                if (QuestionList.Contains(TestQuestion.QuestionID))
                {
                    Time += TestQuestion.ItemAnswerTime.Value;
                    Count += 1;
                }
            }
            return Time / Count;
        }
        List<Excel1> IInsertDataRepository.selectTestAnswer()
        {
            List<Excel1> excel = new List<Excel1>();
            
            
            var user = (from m in db.CEDTS_User orderby m.UserID  select m).Skip(150).Take(10);
            
            foreach (var s in user)
            {
                Excel1 excel2 = new Excel1();
                excel2.id = s.UserID;
                excel2.ii = new Isright();
                excel2.ii.rigth = new List<bool>();
               var info = (from n in db.CEDTS_TestAnswer where n.UserID==s.UserID orderby n.Number select n).ToList();
               foreach (var a in info)
               {
                   excel2.ii.rigth.Add(a.IsRight.Value);
               }
               excel.Add(excel2);
            }

            return excel;                     
        }
    }
}