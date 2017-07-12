using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cedts_Test.Areas.Admin.Models;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_QuestionRepository : ICedts_QuestionRepository
    {
        Cedts_Entities db;
        public Cedts_QuestionRepository()
        {
            db = new Cedts_Entities();
        }
        void ICedts_QuestionRepository.CreateQuestion(List<CEDTS_Question> listquestion)
        {
            for (int i = 0; i < listquestion.Count; i++)
            {
                db.AddToCEDTS_Question(listquestion[i]);
                db.SaveChanges();
            }
        }
        void ICedts_QuestionRepository.UpdateQuestion(List<CEDTS_Question> listquestion)
        {
            for (int i = 0; i < listquestion.Count; i++)
            {
                var id = listquestion[i].QuestionID;
                var originalquestion = (from m in db.CEDTS_Question where m.QuestionID == id select m).FirstOrDefault();
                listquestion[i].Sound = originalquestion.Sound;
                db.ApplyCurrentValues(originalquestion.EntityKey.EntitySetName, listquestion[i]);
                db.SaveChanges();
            }
        }

    }
}