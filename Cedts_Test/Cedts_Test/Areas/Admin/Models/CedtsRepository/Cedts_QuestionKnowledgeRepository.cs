using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cedts_Test.Areas.Admin.Models;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_QuestionKnowledgeRepository : ICedts_QuestionKnowledgeRepository
    {
        Cedts_Entities db;
        public Cedts_QuestionKnowledgeRepository()
        {
            db = new Cedts_Entities();
        }

        /// <summary>
        /// 实现向数据库CEDTS_QuestionKnowledge表添加一条数据
        /// </summary>
        /// <param name="qk">CEDTS_QuestionKnowledge对象</param>
        void ICedts_QuestionKnowledgeRepository.Create(CEDTS_QuestionKnowledge qk)
        {           
            db.AddToCEDTS_QuestionKnowledge(qk);
            db.SaveChanges();
        }

        void ICedts_QuestionKnowledgeRepository.Update(CEDTS_QuestionKnowledge qk)
        {
            var qKonwledge = (from m in db.CEDTS_QuestionKnowledge where m.QuestionKnowledgeID == qk.QuestionKnowledgeID select m).First();
            db.ApplyCurrentValues(qKonwledge.EntityKey.EntitySetName, qk);
            db.SaveChanges();
        }
        void ICedts_QuestionKnowledgeRepository.Delete(List<int> id)
        {
            for (int i = 0; i < id.Count; i++)
            {
                var ids = id[i];
                var qk = (from m in db.CEDTS_QuestionKnowledge where m.QuestionKnowledgeID == ids select m).First();
                db.DeleteObject(qk);
                db.SaveChanges();
            }
        }
    }
}