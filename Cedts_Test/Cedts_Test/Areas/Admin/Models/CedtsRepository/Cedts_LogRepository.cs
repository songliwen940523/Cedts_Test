using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;
using System.Transactions;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_LogRepository : ICedts_LogRepository
    {
        #region 实例化Cedts_Entities
        Cedts_Entities db;
        public Cedts_LogRepository()
        {
            db = new Cedts_Entities();
        }
        #endregion

        int ICedts_LogRepository.GetUserIDByUserAccount(string userAccount)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == userAccount).Select(p => p.UserID).FirstOrDefault();
        }

        void ICedts_LogRepository.SaveLog(string Action, string ClientIP, string Content, int UserID)
        {
            CEDTS_Log log = new CEDTS_Log();
            log.UserID = UserID;
            log.LogTime = DateTime.Now;
            log.Action = Action;
            log.ClientIP = ClientIP;
            log.Content = Content;
            db.AddToCEDTS_Log(log);
            db.SaveChanges();
        }

        PagedList<CEDTS_Log> ICedts_LogRepository.SelectLog(int? id)
        {
            int defaultPageSize = 10;
            IQueryable<CEDTS_Log> logList = db.CEDTS_Log.OrderByDescending(p => p.LogTime);
            PagedList<CEDTS_Log> log = logList.ToPagedList(id ?? 1, defaultPageSize);

            return log;
        }

        bool ICedts_LogRepository.Delete(string ids)
        {
            bool flag = true;
            string[] nums = ids.Split(',');
            using (TransactionScope tran = new TransactionScope())
            {
                for (int i = 0; i < nums.Length; i++)
                {
                    int id = int.Parse(nums[i]);
                    var m = db.CEDTS_Log.Where(p => p.LogID == id).FirstOrDefault();
                    if (m != null)
                    {
                        db.DeleteObject(m);
                        db.SaveChanges();
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
                tran.Complete();
            }
            return flag;
        }
    }
}