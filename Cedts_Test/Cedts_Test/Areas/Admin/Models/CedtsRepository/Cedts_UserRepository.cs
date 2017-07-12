using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_UserRepository : ICedts_UserRepository
    {
        Cedts_Entities db;
        public Cedts_UserRepository()
        {
            db = new Cedts_Entities();
        }

        int defaultPageSize = 10;
        PagedList<CEDTS_User> ICedts_UserRepository.SelectUser(int? id, Guid ClassID)
        {
            IQueryable<CEDTS_User> userList = db.CEDTS_User.Where(p => p.ClassID == ClassID && p.Role == "普通用户").Where(p => p.State == true || p.State == null).OrderBy(p => p.State);

            return userList.ToPagedList(id ?? 1, defaultPageSize);
        }

        int ICedts_UserRepository.SelectAllUser(Guid ClassID)
        {
            return db.CEDTS_User.Where(p => p.ClassID == ClassID).Where(p => p.State == true || p.State == null).Count();
        }

        int ICedts_UserRepository.SelectPassUser(Guid ClassID)
        {
            return db.CEDTS_User.Where(p => p.ClassID == ClassID && p.State == null).Count();
        }

        List<CEDTS_User> ICedts_UserRepository.SelectUserByClassID(Guid ClassID)
        {
            return db.CEDTS_User.Where(p => p.ClassID == ClassID && p.Role == "普通用户").Where(p => p.State == true || p.State == null).OrderBy(p => p.StudentNumber).ToList(); 
        }

        CEDTS_User ICedts_UserRepository.SelectUserByAccout(string Account)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == Account).FirstOrDefault();
        }

        List<CEDTS_User> ICedts_UserRepository.SelectUserByPartner(Guid PartnerID)
        {
            return db.CEDTS_User.Where(p => p.PartnerID == PartnerID).ToList();
        }

        void ICedts_UserRepository.SaveUser(CEDTS_User User)
        {
            db.AddToCEDTS_User(User);
            db.SaveChanges();
        }

        void ICedts_UserRepository.ChangeUserClass(int UserID, Guid ClassID)
        {
            var user = db.CEDTS_User.Where(p => p.UserID == UserID).FirstOrDefault();
            var user2 = user;
            user2.ClassID = ClassID;
            db.ApplyCurrentValues(user.EntityKey.EntitySetName, user2);
            db.SaveChanges();
        }

        void ICedts_UserRepository.DeleteUser(int id)
        {
            /*var user = db.CEDTS_User.Where(p => p.UserID == id).FirstOrDefault();
            var user2 = user;
            user2.State = false;
            db.ApplyCurrentValues(user.EntityKey.EntitySetName, user2);
            db.SaveChanges();*/
            var user = db.CEDTS_User.Where(p => p.UserID == id).FirstOrDefault();
            var user2 = user;
            user2.UserAccount = user2.UserID.ToString();
            user2.UserPassword = null;
            user2.Email = null;
            user2.Role = null;
            user2.RegisterTime = null;
            user2.Sex = null;
            user2.UserNickname = null;
            user2.Phone = null;
            user2.Partner = null;
            user2.StudentNumber = null;
            user2.MajorID = null;
            user2.GradeID = null;
            user2.ClassID = null;
            user2.State = false;
            user2.PartnerID = null;
            user2.BindCard = null;
            db.SaveChanges();
        }

        void ICedts_UserRepository.AuditUser(int id, bool State)
        {
            var user = db.CEDTS_User.Where(p => p.UserID == id).FirstOrDefault();
            var user2 = user;
            user2.State = State;
            db.ApplyCurrentValues(user.EntityKey.EntitySetName, user2);
            db.SaveChanges();
        }

        CEDTS_Partner ICedts_UserRepository.GetPartnerbyID(Guid PartnerID)
        {
            return db.CEDTS_Partner.Where(p => p.PartnerID == PartnerID).FirstOrDefault();
        }

        CEDTS_Major ICedts_UserRepository.GetMajorbyID(Guid MajorID)
        {
            return db.CEDTS_Major.Where(p => p.MajorID == MajorID).FirstOrDefault();
        }

        CEDTS_Grade ICedts_UserRepository.GetGradebyID(Guid GradeID)
        {
            return db.CEDTS_Grade.Where(p => p.GradeID == GradeID).FirstOrDefault();
        }

        CEDTS_Class ICedts_UserRepository.GetClassbyID(Guid ClassID)
        {
            return db.CEDTS_Class.Where(p => p.ClassID == ClassID).FirstOrDefault();
        }

        CEDTS_User ICedts_UserRepository.SelectUserByEmail(string Email)
        {
            return db.CEDTS_User.Where(p => p.Email == Email).FirstOrDefault();
        }

        List<CEDTS_Class> ICedts_UserRepository.GetClassbyUserID(int UserID)
        {
            return db.CEDTS_Class.Where(p => p.UserID == UserID).OrderBy(p=>p.CreateTime).ToList();
        }

        List<Class> ICedts_UserRepository.SelectClassList(int id)
        {
            var classinfo = (from m in db.CEDTS_Class where m.UserID == id select m).ToList();
            List<Class> classList = new List<Class>();
            foreach (var c in classinfo)
            {
                Class class1 = new Class();
                class1.ClassID = c.ClassID;
                class1.ClassName = c.ClassName;
                class1.GradeName = (from m in db.CEDTS_Grade where m.GradeID == c.GradeID select m.GradeName).FirstOrDefault();
                class1.StudentNum = (from m in db.CEDTS_User where m.ClassID == c.ClassID && m.State == true select m).Count();
                classList.Add(class1);
            }
            return classList;
        }
        #region 密码加密
        string ICedts_UserRepository.HashPassword(string str)
        {
            string rethash = string.Empty;
            System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create();
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            byte[] combined = encoder.GetBytes(str);
            hash.ComputeHash(combined);
            rethash = Convert.ToBase64String(hash.Hash);
            return rethash;
        }
        #endregion
    }
}