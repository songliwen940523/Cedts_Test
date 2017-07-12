using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_TemporaryExpansionRepository : ICedts_TemporaryExpansionRepository
    {
        Cedts_Entities db;
        public Cedts_TemporaryExpansionRepository()
        {
            db = new Cedts_Entities();
        }
        /// <summary>
        ///  向Expansion表添加选词选项
        /// </summary>
        /// <param name="Expansion">选词填空选项</param>
        void ICedts_TemporaryExpansionRepository.CreateExpansion(CEDTS_TemporaryExpansion Expansion)
        {
            db.AddToCEDTS_TemporaryExpansion(Expansion);
            db.SaveChanges();
        }
    }
}