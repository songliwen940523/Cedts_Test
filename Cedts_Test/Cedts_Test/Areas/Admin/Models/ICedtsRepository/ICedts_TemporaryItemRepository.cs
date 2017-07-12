using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_TemporaryItemRepository
    {
        /// <summary>
        /// 保存题目信息到l临时数据库
        /// </summary>
        /// <param name="item">数据库试题实体</param>
        void Create(CEDTS_TemporaryAssessmentItem item);
    }
}