using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_TemporaryQuestionKnowledgeRepository : ICedts_TemporaryQuestionKnowledgeRepository
    {

        Cedts_Entities db;
        public Cedts_TemporaryQuestionKnowledgeRepository()
        {
            db = new Cedts_Entities();
        }

        /// <summary>
        /// 实现向数据库CEDTS_QuestionKnowledge表添加一条数据
        /// </summary>
        /// <param name="qk">CEDTS_QuestionKnowledge对象</param>
        void ICedts_TemporaryQuestionKnowledgeRepository.Create(CEDTS_TemporaryQuestionKnowledge tqk)
        {

            db.AddToCEDTS_TemporaryQuestionKnowledge(tqk);
            db.SaveChanges();
        }
        void ICedts_TemporaryQuestionKnowledgeRepository.Clear(Guid GID)
        {
            db.clear(GID);
            db.SaveChanges();
        }
    }
}