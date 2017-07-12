using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_ExpansionRepository : ICedts_ExpansionRepository
    {
        Cedts_Entities db;
        public Cedts_ExpansionRepository()
        {
            db = new Cedts_Entities();
        }
        /// <summary>
        ///  向Expansion表添加选词选项
        /// </summary>
        /// <param name="Expansion">选词填空选项</param>
        void ICedts_ExpansionRepository.CreateExpansion(CEDTS_Expansion Expansion)
        {
            db.AddToCEDTS_Expansion(Expansion);
            db.SaveChanges();
        }

        void ICedts_ExpansionRepository.UpdateExpansion(CEDTS_Expansion Expansion)
        {
            var Expansion1 = (from m in db.CEDTS_Expansion where m.ExpansionID == Expansion.ExpansionID select m).First();
            db.ApplyCurrentValues(Expansion1.EntityKey.EntitySetName, Expansion);
            db.SaveChanges();
        }
    }
}