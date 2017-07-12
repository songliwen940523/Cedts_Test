using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models.CedtsRepository
{
    public class Cedts_TemporaryQuestionRepository : ICedts_TemporaryQuestionRepository
    {
        Cedts_Entities db;
        public Cedts_TemporaryQuestionRepository()
        {
            db = new Cedts_Entities();
        }
        void ICedts_TemporaryQuestionRepository.CreateQuestion(List<CEDTS_TemporaryQuestion> listtquestion)
        {
            for (int i = 0; i < listtquestion.Count; i++)
            {
                db.AddToCEDTS_TemporaryQuestion(listtquestion[i]);
                db.SaveChanges();
            }
        }
    }
}