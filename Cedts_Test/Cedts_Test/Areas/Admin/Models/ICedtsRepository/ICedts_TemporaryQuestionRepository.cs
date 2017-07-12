using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_TemporaryQuestionRepository
    {
        /// <summary>
        /// 保存问题信息到数据库
        /// </summary>
        /// <param name="CQuestion">Question实体类</param>
        void CreateQuestion(List<CEDTS_TemporaryQuestion> listtquestion);
    }
}