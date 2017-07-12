using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_QuestionKnowledgeRepository
    {
        /// <summary>
        /// 向CEDTS_QuestionKnowledge表中插入一条数据
        /// </summary>
        /// <param name="qk">CEDTS_QuestionKnowledge对象</param>
        void Create(CEDTS_QuestionKnowledge qk);

        void Update(CEDTS_QuestionKnowledge qk);

        void Delete(List<int> id);
    }
}