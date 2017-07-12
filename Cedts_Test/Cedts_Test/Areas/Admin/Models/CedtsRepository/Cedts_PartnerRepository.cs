using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_PartnerRepository : ICedts_PartnerRepository
    {
        #region 实例化Cedts_Entities
        Cedts_Entities db;
        public Cedts_PartnerRepository()
        {
            db = new Cedts_Entities();
        }
        #endregion
        int defaultPageSize = 10;

        List<CEDTS_User> ICedts_PartnerRepository.GetAdmin()
        {
            return db.CEDTS_User.Where(p => p.Role == "管理员" && p.PartnerID == null).ToList();
        }

        bool ICedts_PartnerRepository.CheckPartnerName(string PartnerName, Guid? PartnerID)
        {
            if (PartnerID == null)
                return db.CEDTS_Partner.Where(p => p.PartnerName == PartnerName).Count() > 0;
            else
            {
                var partner = db.CEDTS_Partner.Where(p => p.PartnerName == PartnerName).FirstOrDefault();
                if (partner == null)
                {
                    return false;
                }
                else
                {
                    if (partner.PartnerID == PartnerID)
                    {
                        return false;
                    }
                    else
                        return true;
                }
            }
        }

        bool ICedts_PartnerRepository.CheckPartnerEmail(string Email, Guid? PartnerID)
        {
            if (PartnerID == null)
                return db.CEDTS_Partner.Where(p => p.Email == Email).Count() > 0;
            else
            {
                var partner = db.CEDTS_Partner.Where(p => p.Email == Email).FirstOrDefault();
                if (partner == null)
                {
                    return false;
                }
                else
                {
                    if (partner.PartnerID == PartnerID)
                    {
                        return false;
                    }
                    else
                        return true;
                }
            }
        }

        void ICedts_PartnerRepository.SavePartner(CEDTS_Partner Partner)
        {
            Partner.PartnerID = Guid.NewGuid();
            Partner.CreateTime = DateTime.Now;

            var account = Partner.AdminAccount;
            CEDTS_User user = db.CEDTS_User.Where(p => p.UserAccount == account).FirstOrDefault();
            var user2 = user;
            user2.PartnerID = Partner.PartnerID;
            db.ApplyCurrentValues(user.EntityKey.EntitySetName, user2);

            db.AddToCEDTS_Partner(Partner);
            db.SaveChanges();
        }

        CEDTS_Partner ICedts_PartnerRepository.GetPartnerbyID(Guid PartnerID)
        {
            return db.CEDTS_Partner.Where(p => p.PartnerID == PartnerID).FirstOrDefault();
        }

        CEDTS_User ICedts_PartnerRepository.GetUserbyAccount(string Account)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == Account).FirstOrDefault();
        }

        void ICedts_PartnerRepository.ChangePartner(CEDTS_Partner Partner)
        {
            var id = Partner.PartnerID;
            var account = Partner.AdminAccount;
            CEDTS_User user = db.CEDTS_User.Where(p => p.UserAccount == account).FirstOrDefault();
            var tempuser = user;
            tempuser.PartnerID = Partner.PartnerID;
            db.ApplyCurrentValues(user.EntityKey.EntitySetName, tempuser);
            var partner2 = db.CEDTS_Partner.Where(p => p.PartnerID == id).FirstOrDefault();
            if (Partner.AdminAccount != partner2.AdminAccount && partner2.AdminAccount != "")
            {
                var account2 = partner2.AdminAccount;
                CEDTS_User user2 = db.CEDTS_User.Where(p => p.UserAccount == account2).FirstOrDefault();
                var tempuser2 = user2;
                tempuser2.PartnerID = null;
                db.ApplyCurrentValues(user2.EntityKey.EntitySetName, tempuser2);
            }
            Partner.CreateTime = partner2.CreateTime;
            db.ApplyCurrentValues(partner2.EntityKey.EntitySetName, Partner);
            db.SaveChanges();
        }

        Guid ICedts_PartnerRepository.GetPartnerIDbyUserAccount(string UserAccount)
        {
            var PartnerID = db.CEDTS_User.Where(p => p.UserAccount == UserAccount).Select(p => p.PartnerID).FirstOrDefault();
            if (PartnerID != null)
                return PartnerID.Value;
            else
                return Guid.Empty;
        }

        List<CEDTS_User> ICedts_PartnerRepository.GetTeacherbyPartnerID(Guid PartnerID)
        {
            return db.CEDTS_User.Where(p => p.PartnerID == PartnerID && p.Role == "教师").ToList();
        }

        CEDTS_Major ICedts_PartnerRepository.GetMajorbyID(Guid MajorID)
        {
            return db.CEDTS_Major.Where(p => p.MajorID == MajorID).FirstOrDefault();
        }

        CEDTS_Grade ICedts_PartnerRepository.GetGradebyID(Guid GradeID)
        {
            return db.CEDTS_Grade.Where(p => p.GradeID == GradeID).FirstOrDefault();
        }

        CEDTS_Class ICedts_PartnerRepository.GetClassbyID(Guid ClassID)
        {
            return db.CEDTS_Class.Where(p => p.ClassID == ClassID).FirstOrDefault();
        }

        PagedList<CEDTS_Partner> ICedts_PartnerRepository.GetPartner(int? id)
        {
            IQueryable<CEDTS_Partner> Partner = db.CEDTS_Partner.OrderBy(p => p.CreateTime);
            PagedList<CEDTS_Partner> partner = Partner.ToPagedList(id ?? 1, defaultPageSize);
            return partner;
        }

        List<CEDTS_Major> ICedts_PartnerRepository.GetMajorByPID(Guid PartnerID)
        {
            return db.CEDTS_Major.Where(p => p.PartnerID == PartnerID).OrderByDescending(p => p.CreateTime).ToList();
        }

        List<CEDTS_Grade> ICedts_PartnerRepository.GetGradeByMID(Guid MajorID)
        {
            return db.CEDTS_Grade.Where(p => p.MajorID == MajorID).OrderByDescending(p => p.CreateTime).ToList();
        }

        List<CEDTS_Class> ICedts_PartnerRepository.GetClassByGID(Guid GradeID)
        {
            return db.CEDTS_Class.Where(p => p.GradeID == GradeID).OrderByDescending(p => p.CreateTime).ToList();
        }

        void ICedts_PartnerRepository.SaveTeacher(CEDTS_User Teacher)
        {
            db.AddToCEDTS_User(Teacher);
            db.SaveChanges();
        }

        CEDTS_User ICedts_PartnerRepository.GetTeacherbyID(int id)
        {
            return db.CEDTS_User.Where(p => p.UserID == id).FirstOrDefault();
        }

        void ICedts_PartnerRepository.ChangeTeacher(CEDTS_User Teacher)
        {
            var id = Teacher.UserID;
            var t = db.CEDTS_User.Where(p => p.UserID == id).FirstOrDefault();
            if (Teacher.UserPassword == null)
            {
                Teacher.UserPassword = t.UserPassword;
            }
            Teacher.Role = t.Role;
            Teacher.State = t.State;
            Teacher.RegisterTime = t.RegisterTime;
            Teacher.PartnerID = t.PartnerID;
            Teacher.StudentNumber = t.StudentNumber;
            db.ApplyCurrentValues(t.EntityKey.EntitySetName, Teacher);
            db.SaveChanges();
        }

        List<CEDTS_Major> ICedts_PartnerRepository.GetMajorbyName(string MajorName, Guid PartnerID)
        {
            return db.CEDTS_Major.Where(p => p.PartnerID == PartnerID && p.MajorName == MajorName).ToList();
        }

        List<CEDTS_Major> ICedts_PartnerRepository.GetMajorbyLevel(int Level, Guid PartnerID)
        {
            return db.CEDTS_Major.Where(p => p.MajorLevel == Level && p.PartnerID == PartnerID).OrderByDescending(p => p.CreateTime).ToList();
        }

        List<CEDTS_Major> ICedts_PartnerRepository.NewGetMajorbyLevel(int Level,Guid MajorID,Guid PartnerID)
        {
            return db.CEDTS_Major.Where(p => p.MajorLevel == Level && p.PartnerID == PartnerID && p.MajorID != MajorID).OrderByDescending(p => p.CreateTime).ToList();
        }

        List<CEDTS_Major> ICedts_PartnerRepository.GetMajorforTGrade(Guid MajorID, Guid PartnerID)
        {
            return db.CEDTS_Major.Where(p => p.MajorID == MajorID && p.PartnerID == PartnerID).OrderByDescending(p => p.CreateTime).ToList();
        }

        bool ICedts_PartnerRepository.CheckMajorName(string MajorName, Guid? MajorID, Guid PartnerID)
        {
            if (MajorID == null)
                return db.CEDTS_Major.Where(p => p.PartnerID == PartnerID).Where(p => p.MajorName == MajorName).FirstOrDefault() != null;
            else
            {
                var Major = db.CEDTS_Major.Where(p => p.PartnerID == PartnerID).Where(p => p.MajorName == MajorName).FirstOrDefault();
                if (Major == null)
                {
                    return false;
                }
                else
                {
                    if (Major.MajorID == MajorID)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        void ICedts_PartnerRepository.SaveMajor(CEDTS_Major Major)
        {
            db.AddToCEDTS_Major(Major);
            db.SaveChanges();
        }

        void ICedts_PartnerRepository.ChangeMajor(CEDTS_Major Major)
        {
            var majorid = Major.MajorID;
            var major = db.CEDTS_Major.Where(p => p.MajorID == majorid).FirstOrDefault();
            Major.CreateTime = major.CreateTime;
            db.ApplyCurrentValues(major.EntityKey.EntitySetName, Major);
            db.SaveChanges();
        }

        List<CEDTS_Grade> ICedts_PartnerRepository.GetGrade(Guid PartnerID, Guid MajorID)
        {
            return db.CEDTS_Grade.Where(p => p.PartnerID == PartnerID && p.MajorID != MajorID).OrderByDescending(p => p.CreateTime).ToList();
        }

        List<CEDTS_Grade> ICedts_PartnerRepository.GetTGrade(Guid PartnerID, Guid MajorID)
        {
            return db.CEDTS_Grade.Where(p => p.PartnerID == PartnerID && p.MajorID == MajorID).OrderByDescending(p => p.CreateTime).ToList();
        }

        List<CEDTS_Class> ICedts_PartnerRepository.GetClass(Guid PartnerID)
        {
            return db.CEDTS_Class.Where(p => p.PartnerID == PartnerID).OrderByDescending(p => p.CreateTime).ToList();
        }

        bool ICedts_PartnerRepository.CheckGradeName(string GradeName, Guid? MajorID, Guid? GradeID)
        {
            if (MajorID == null)
            {
                return false;
            }
            else
            {
                if (GradeID == null)
                {
                    return db.CEDTS_Grade.Where(p => p.GradeName == GradeName && p.MajorID == MajorID).FirstOrDefault() != null;
                }
                else
                {
                    var Grade = db.CEDTS_Grade.Where(p => p.GradeName == GradeName && p.MajorID == MajorID).FirstOrDefault();
                    if (Grade == null)
                        return false;
                    else
                    {
                        if (Grade.GradeID == GradeID)
                            return false;
                        else
                            return true;
                    }
                }
            }
        }

        List<CEDTS_Major> ICedts_PartnerRepository.GetMajorbyUpID(Guid MajorID)
        {
            return db.CEDTS_Major.Where(p => p.UpMajorID == MajorID && p.MajorID != MajorID).OrderByDescending(p => p.CreateTime).ToList();
        }

        void ICedts_PartnerRepository.SaveGrade(CEDTS_Grade Grade)
        {
            db.AddToCEDTS_Grade(Grade);
            db.SaveChanges();
        }

        void ICedts_PartnerRepository.ChangeGrade(CEDTS_Grade Grade)
        {
            var id = Grade.GradeID;
            var grade = db.CEDTS_Grade.Where(p => p.GradeID == id).FirstOrDefault();
            Grade.PartnerID = grade.PartnerID;
            Grade.CreateTime = grade.CreateTime;
            db.ApplyCurrentValues(grade.EntityKey.EntitySetName, Grade);
            db.SaveChanges();
        }

        bool ICedts_PartnerRepository.CheckClassName(string ClassName, Guid? ClassID)
        {
            if (ClassID == null)
            {
                return db.CEDTS_Class.Where(p => p.ClassName == ClassName).FirstOrDefault() != null;
            }
            else
            {
                var Class = db.CEDTS_Class.Where(p => p.ClassName == ClassName).FirstOrDefault();
                if (Class == null)
                    return false;
                else
                {
                    if (Class.ClassID == ClassID)
                        return false;
                    else
                        return true;
                }
            }
        }

        void ICedts_PartnerRepository.SaveClass(CEDTS_Class Class)
        {
            db.AddToCEDTS_Class(Class);
            db.SaveChanges();
        }

        void ICedts_PartnerRepository.ChangeClass(CEDTS_Class Class)
        {
            var id = Class.ClassID;
            var c = db.CEDTS_Class.Where(p => p.ClassID == id).FirstOrDefault();
            Class.PartnerID = c.PartnerID;
            Class.CreateTime = c.CreateTime;
            db.ApplyCurrentValues(c.EntityKey.EntitySetName, Class);
            db.SaveChanges();
        }

        void ICedts_PartnerRepository.DeleteClass(Guid ClassID)
        {
            var userList = db.CEDTS_User.Where(p => p.ClassID == ClassID).ToList();
            foreach (var user in userList)
            {
                var tempuser = user;
                tempuser.ClassID = null;
                tempuser.State = null;
                db.ApplyCurrentValues(user.EntityKey.EntitySetName, tempuser);
            }
            var c = db.CEDTS_Class.Where(p => p.ClassID == ClassID).FirstOrDefault();
            db.DeleteObject(c);
            db.SaveChanges();
        }

        List<Guid> ICedts_PartnerRepository.GetClassIDList(Guid GradeID)
        {
            return db.CEDTS_Class.Where(p => p.GradeID == GradeID).Select(p => p.ClassID).ToList();
        }

        void ICedts_PartnerRepository.DeleteGrade(Guid GradeID)
        {
            var grade = db.CEDTS_Grade.Where(p => p.GradeID == GradeID).FirstOrDefault();
            var userList = db.CEDTS_User.Where(p => p.GradeID == GradeID);
            foreach (var user in userList)
            {
                var tempuser = user;
                tempuser.GradeID = null;
                db.ApplyCurrentValues(user.EntityKey.EntitySetName, tempuser);
            }
            db.DeleteObject(grade);
            db.SaveChanges();
        }

        List<Guid> ICedts_PartnerRepository.GetGradeIDList(Guid MajorID)
        {
            return db.CEDTS_Grade.Where(p => p.MajorID == MajorID).Select(p => p.GradeID).ToList();
        }

        void ICedts_PartnerRepository.DeleteMajor(Guid MajorID)
        {
            var major = db.CEDTS_Major.Where(p => p.MajorID == MajorID).FirstOrDefault();
            if (major.MajorLevel == 1)
            {
                var majorList2 = db.CEDTS_Major.Where(p => p.UpMajorID == MajorID);
                foreach (var m2 in majorList2)
                {
                    var id = m2.MajorID;
                    var majorList3 = db.CEDTS_Major.Where(p => p.UpMajorID == id);
                    foreach (var m3 in majorList3)
                    {
                        var mid3 = m3.MajorID;
                        var userList = db.CEDTS_User.Where(p => p.MajorID == mid3);
                        foreach (var user in userList)
                        {
                            var tempuser = user;
                            tempuser.MajorID = null;
                            db.ApplyCurrentValues(user.EntityKey.EntitySetName, tempuser);
                        }
                        db.DeleteObject(m3);
                    }
                    db.DeleteObject(m2);
                }
                db.DeleteObject(major);
            }
            if (major.MajorLevel == 2)
            {
                var majorList3 = db.CEDTS_Major.Where(p => p.UpMajorID == MajorID);
                foreach (var m3 in majorList3)
                {
                    var mid3 = m3.MajorID;
                    var userList = db.CEDTS_User.Where(p => p.MajorID == mid3);
                    foreach (var user in userList)
                    {
                        var tempuser = user;
                        tempuser.MajorID = null;
                        db.ApplyCurrentValues(user.EntityKey.EntitySetName, tempuser);
                    }
                    db.DeleteObject(m3);
                }
                db.DeleteObject(major);
            }
            if (major.MajorLevel == 3)
            {
                var mid3 = major.MajorID;
                var userList = db.CEDTS_User.Where(p => p.MajorID == mid3);
                foreach (var user in userList)
                {
                    var tempuser = user;
                    tempuser.MajorID = null;
                    db.ApplyCurrentValues(user.EntityKey.EntitySetName, tempuser);
                }
                db.DeleteObject(major);
            }
            db.SaveChanges();
        }

        void ICedts_PartnerRepository.DeleteUser(int id)
        {
            var user = db.CEDTS_User.Where(p => p.UserID == id).FirstOrDefault();
            var log = db.CEDTS_Log.Where(p => p.UserID == id).ToList();
            foreach (var l in log)
            {
                db.DeleteObject(l);
            }
            db.DeleteObject(user);
            db.SaveChanges();
        }

        List<int> ICedts_PartnerRepository.GetAdminAndTheacher(Guid PartnerID)
        {
            return db.CEDTS_User.Where(p => p.PartnerID == PartnerID && p.Role != "普通用户").Select(p => p.UserID).ToList();
        }

        List<Guid> ICedts_PartnerRepository.GetMajorIDList(Guid PartnerID)
        {
            return db.CEDTS_Major.Where(p => p.PartnerID == PartnerID).OrderByDescending(p => p.MajorLevel).Select(p => p.MajorID).ToList();
        }

        void ICedts_PartnerRepository.DeletePartner(Guid PartnerID)
        {
            var partner = db.CEDTS_Partner.Where(p => p.PartnerID == PartnerID).FirstOrDefault();
            var userList = db.CEDTS_User.Where(p => p.PartnerID == PartnerID);
            foreach (var user in userList)
            {
                var tempuser = user;
                tempuser.PartnerID = null;
                db.ApplyCurrentValues(user.EntityKey.EntitySetName, tempuser);
            }
            db.DeleteObject(partner);
            db.SaveChanges();
        }

        #region 密码加密
        string ICedts_PartnerRepository.HashPassword(string str)
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