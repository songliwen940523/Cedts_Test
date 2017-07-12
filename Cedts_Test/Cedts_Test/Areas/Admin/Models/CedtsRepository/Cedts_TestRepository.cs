using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_TestRepository : ICedts_TestRepository
    {
        #region 实例化Cedts_Entities
        Cedts_Entities db;
        public Cedts_TestRepository()
        {
            db = new Cedts_Entities();
        }
        #endregion

        List<CEDTS_Test> ICedts_TestRepository.SelectTest()
        {
            return db.CEDTS_Test.Where(p => p.IsFinished == true).OrderBy(p=>p.IsChecked).ToList();
        }

        string ICedts_TestRepository.GetUserName(int UserID)
        {
            return db.CEDTS_User.Where(p => p.UserID == UserID).Select(p => p.UserAccount).FirstOrDefault();
        }

        string ICedts_TestRepository.GetPaperTitle(Guid PaperID)
        {
            return db.CEDTS_Paper.Where(p => p.PaperID == PaperID).Select(p => p.Title).FirstOrDefault();
        }

        List<CEDTS_TestAnswer> ICedts_TestRepository.GetTestAnswer(Guid TestID)
        {
            return db.CEDTS_TestAnswer.Where(p => p.TestID == TestID).ToList();
        }

        CEDTS_Test ICedts_TestRepository.GetTest(Guid testID)
        {
            return db.CEDTS_Test.Where(p => p.TestID == testID).FirstOrDefault();
        }

        void ICedts_TestRepository.UpdateTest(CEDTS_Test test)
        {
            var oldtest = db.CEDTS_Test.Where(p => p.TestID == test.TestID).FirstOrDefault();

            db.ApplyCurrentValues<CEDTS_Test>(oldtest.EntityKey.EntitySetName, test);

            db.SaveChanges();
        }
    }
}