using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_AdminRepository : ICedts_AdminRepository
    {
        #region 实例化Cedts_Entities
        Cedts_Entities db;
        public Cedts_AdminRepository()
        {
            db = new Cedts_Entities();
        }
        #endregion

        #region 后台用户查询展示
        int defaultPageSize = 10;
        PagedList<CEDTS_User> ICedts_AdminRepository.SelectUser(int? id)
        {
            IQueryable<CEDTS_User> user = (from m in db.CEDTS_User where m.Role == "管理员" orderby m.RegisterTime descending select m);

            PagedList<CEDTS_User> Users = user.ToPagedList(id ?? 1, defaultPageSize);
            return Users;
        }
        #endregion

        #region 查询当前点击的用户信息
        CEDTS_User ICedts_AdminRepository.SelectEditUser(int id)
        {
            var user = (from m in db.CEDTS_User where m.UserID == id select m).First();
            return user;
        }
        #endregion

        #region 后台用户新增
        void ICedts_AdminRepository.CreateUser(CEDTS_User user)
        {
            user.RegisterTime = DateTime.Now;
            db.AddToCEDTS_User(user);
            db.SaveChanges();
        }
        #endregion

        #region 后台用户编辑
        void ICedts_AdminRepository.EditUser(CEDTS_User user)
        {
            var name = (from m in db.CEDTS_User where m.UserID == user.UserID select m).First();
            if (user.UserPassword == null || user.UserPassword == "")
            {
                user.UserPassword = name.UserPassword;
            }
            user.Role = name.Role;
            db.ApplyCurrentValues(name.EntityKey.EntitySetName, user);
            db.SaveChanges();
        }
        #endregion

        #region 后台用户删除
        int ICedts_AdminRepository.DeleteUser(string[] name)
        {
            int count = DeleteUser(name);
            return count;
        }

        public int DeleteUser(string[] ids)
        {
            int num = 0;

            string strids = string.Join(",", ids.Cast<string>().ToArray());
            var sideid = db.CEDTS_User.Where("it.UserID in {" + strids + "}");
            foreach (CEDTS_User item in sideid.ToList())
            {
                if (item != null)
                {
                    var sortid = db.CEDTS_User.Where("it.UserID in {" + strids + "}");
                    foreach (CEDTS_User items in sortid.ToList())
                    {
                        if (items.PartnerID != null)
                        {
                            Guid PartnerID = items.PartnerID.Value;
                            var partner = db.CEDTS_Partner.Where(p => p.PartnerID == PartnerID).FirstOrDefault();
                            var partner2 = partner;
                            partner2.AdminAccount = string.Empty;
                            db.ApplyCurrentValues(partner.EntityKey.EntitySetName, partner2);
                        }
                        var userid = items.UserID;
                        var log = db.CEDTS_Log.Where(p => p.UserID == userid).ToList();
                        foreach (var l in log)
                        {
                            db.DeleteObject(l);
                        }
                        db.DeleteObject(items);
                        num = 1;
                    }
                }
                else
                {
                    return num;
                }
            }
            db.SaveChanges();
            return num;
        }
        #endregion

        #region 验证帐号
        bool ICedts_AdminRepository.AjaxCheckAccount(string Account, int UserID)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == Account && p.UserID != UserID).FirstOrDefault() != null;
        }
        #endregion

        #region 验证邮箱
        bool ICedts_AdminRepository.AjaxCheckEmail(string Email, int UserID)
        {
            return db.CEDTS_User.Where(p => p.Email == Email && p.UserID != UserID).FirstOrDefault() != null;
        }
        #endregion

        #region 密码加密
        string ICedts_AdminRepository.HashPassword(string str)
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