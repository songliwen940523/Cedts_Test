using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_TemporaryQuestionKnowledgeRepository
    {
        /// <summary>
        /// 向CEDTS_QuestionKnowledge表中插入一条数据
        /// </summary>
        /// <param name="qk">CEDTS_QuestionKnowledge对象</param>
        void Create(CEDTS_TemporaryQuestionKnowledge tqk);

        void Clear(Guid GID);
    }
}