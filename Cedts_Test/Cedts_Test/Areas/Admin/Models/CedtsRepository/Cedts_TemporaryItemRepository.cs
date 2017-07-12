using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_TemporaryItemRepository : ICedts_TemporaryItemRepository
    {
        Cedts_Entities db;
        public Cedts_TemporaryItemRepository()
        {
            db = new Cedts_Entities();
        }

        /// <summary>
        /// 新增AssessmentItem信息
        /// </summary>
        /// <param name="item">AssessmentItem表基本信息</param>
        void ICedts_TemporaryItemRepository.Create(CEDTS_TemporaryAssessmentItem item)
        {
            db.AddToCEDTS_TemporaryAssessmentItem(item);
            db.SaveChanges();
        }
    }
}