using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Models
{
    public class Cedts_UserRepository : ICedts_UserRepository
    {
        CedtsEntities db;
        public Cedts_UserRepository()
        {
            db = new CedtsEntities();
        }

        #region 成员

        bool ICedts_UserRepository.AjaxCheckAccount(string Account, int UserID)
        {
            if (UserID == 0)
                return db.CEDTS_User.Where(p => p.UserAccount == Account).FirstOrDefault() != null;
            else
            {
                if (db.CEDTS_User.Where(p => p.UserAccount == Account).FirstOrDefault() == null)
                {
                    return false;
                }
                if (db.CEDTS_User.Where(p => p.UserAccount == Account).Count() == 1 && db.CEDTS_User.Where(p => p.UserAccount == Account).Select(p => p.UserID).FirstOrDefault() == UserID)
                {
                    return false;
                }
                else
                    return true;
            }
        }

        bool ICedts_UserRepository.AjaxCheckEmail(string Email, int UserID)
        {
            if (UserID == 0)
                return db.CEDTS_User.Where(p => p.Email == Email).FirstOrDefault() != null;
            else
            {
                if (db.CEDTS_User.Where(p => p.Email == Email).FirstOrDefault() == null)
                {
                    return false;
                }
                if (db.CEDTS_User.Where(p => p.Email == Email).Count() == 1 && db.CEDTS_User.Where(p => p.Email == Email).Select(p => p.UserID).FirstOrDefault() == UserID)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        void ICedts_UserRepository.Create(CEDTS_User user)
        {
            user.Role = "普通用户";
            user.RegisterTime = DateTime.Now;
            if (user.ClassID == Guid.Empty)
            {
                user.ClassID = null;
                user.GradeID = null;
                user.MajorID = null;
                user.PartnerID = null;
            }

            db.AddToCEDTS_User(user);
            db.SaveChanges();
        }

        bool ICedts_UserRepository.LogOn(string Account, string Password)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == Account && p.UserPassword == Password).FirstOrDefault() != null;
        }
        string ICedts_UserRepository.GetRoleByAccount(string Account)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == Account).FirstOrDefault().Role;
        }

        CEDTS_User ICedts_UserRepository.GetUser(string Email)
        {
            return db.CEDTS_User.Where(p => p.Email == Email).FirstOrDefault();
        }

        void ICedts_UserRepository.Save()
        {
            db.SaveChanges();
        }

        string ICedts_UserRepository.RandNum()
        {
            Random rand = new Random();
            int n1 = rand.Next(0, 9);
            int n2 = rand.Next(0, 9);
            int n3 = rand.Next(0, 9);
            int n4 = rand.Next(0, 9);
            int n5 = rand.Next(0, 9);
            int n6 = rand.Next(0, 9);
            string num = n1.ToString() + n2.ToString() + n3.ToString() + n4.ToString() + n5.ToString() + n6.ToString();
            return num;
        }

        List<CEDTS_Partner> ICedts_UserRepository.GetPartner()
        {
            return db.CEDTS_Partner.ToList();
        }

        List<CEDTS_Major> ICedts_UserRepository.GetMajor(Guid PartnerID)
        {
            return db.CEDTS_Major.Where(p => p.PartnerID == PartnerID).OrderByDescending(p => p.CreateTime).ToList();
        }

        List<CEDTS_Grade> ICedts_UserRepository.GetGrade(Guid MajorID)
        {
            return db.CEDTS_Grade.Where(p => p.MajorID == MajorID).OrderByDescending(p => p.CreateTime).ToList();
        }

        List<CEDTS_Class> ICedts_UserRepository.GetClass(Guid GradeID)
        {
            return db.CEDTS_Class.Where(p => p.GradeID == GradeID).OrderByDescending(p => p.CreateTime).ToList();
        }

        CEDTS_User ICedts_UserRepository.GetUserByAccount(string Account)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == Account).FirstOrDefault();
        }

        void ICedts_UserRepository.ChangeUser(CEDTS_User user)
        {
            var id = user.UserID;
            var olduser = db.CEDTS_User.Where(p => p.UserID == id).FirstOrDefault();
            db.ApplyCurrentValues(olduser.EntityKey.EntitySetName, user);
            db.SaveChanges();
        }

        List<CEDTS_User> ICedts_UserRepository.GetUserInfo()
        {
            return db.CEDTS_User.Where(p => p.Role == "普通用户").ToList();
        }

        void ICedts_UserRepository.CreateUser(CEDTS_User user)
        {
            user.Role = "体验用户";
            user.RegisterTime = DateTime.Now;
            if (user.ClassID == Guid.Empty)
            {
                user.ClassID = null;
                user.GradeID = null;
                user.MajorID = null;
                user.PartnerID = null;
            }

            db.AddToCEDTS_User(user);
            db.SaveChanges();
        }

        #endregion

        #region 查询当前用户角色
        string ICedts_UserRepository.SelectRole(string name)
        {
            var Role = (from m in db.CEDTS_User where m.UserAccount == name select m.Role).FirstOrDefault();
            return Role;
        }
        #endregion

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