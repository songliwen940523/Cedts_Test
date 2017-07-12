using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_UserRepository
    {
        /// <summary>
        /// 获取班级中的学生对象
        /// </summary>
        /// <param name="id">分页ID</param>
        /// <param name="ClassID">班级ID</param>
        /// <returns>分页后的学生对象</returns>
        PagedList<CEDTS_User> SelectUser(int? id,Guid ClassID);

        /// <summary>
        /// 获取班级中的学生个数
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        /// <returns>学生个数</returns>
        int SelectAllUser(Guid ClassID);

        /// <summary>
        /// 获取班级中的通过审查的学生个数
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        /// <returns>学生个数</returns>
        int SelectPassUser(Guid ClassID);

        /// <summary>
        /// 获取班级中的学生对象
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        /// <returns>学生对象</returns>
        List<CEDTS_User> SelectUserByClassID(Guid ClassID);

        /// <summary>
        /// 根据登录账户的帐号获取登录账户的信息
        /// </summary>
        /// <param name="Account">用户帐号</param>
        /// <returns>User对象</returns>
        CEDTS_User SelectUserByAccout(string Account);

        /// <summary>
        /// 根据院校管理员获取学生列表
        /// </summary>
        /// <param name="PartnerID">院校管理员</param>
        /// <returns>User对象</returns>
        List<CEDTS_User> SelectUserByPartner(Guid PartnerID);

        /// <summary>
        /// 根据邮箱地址获取帐号信息
        /// </summary>
        /// <param name="Email">邮箱地址</param>
        /// <returns>User对象</returns>
        CEDTS_User SelectUserByEmail(string Email);

        /// <summary>
        /// 保存学生信息
        /// </summary>
        /// <param name="User">学生对象</param>
        void SaveUser(CEDTS_User User);

        /// <summary>
        /// 更换学生班级
        /// </summary>
        /// <param name="StudID">学生ID</param>
        /// <param name="ClassID">班级ID</param>
        void ChangeUserClass(int UserID, Guid ClassID);

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="id">学生ID</param>
        void DeleteUser(int id);

        /// <summary>
        /// 审核学生信息
        /// </summary>
        /// <param name="id">学生ID</param>
        /// <param name="State">审核状态</param>
        void AuditUser(int id, bool State);

        /// <summary>
        /// 获取院校对象
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>返回院校对象</returns>
        CEDTS_Partner GetPartnerbyID(Guid PartnerID);

        /// <summary>
        /// 根据ID获取专业对象
        /// </summary>
        /// <param name="MajorID">专业ID</param>
        /// <returns>专业对象</returns>
        CEDTS_Major GetMajorbyID(Guid MajorID);

        /// <summary>
        /// 根据ID获取年级对象
        /// </summary>
        /// <param name="GradeID">年级ID</param>
        /// <returns>年级对象</returns>
        CEDTS_Grade GetGradebyID(Guid GradeID);

        /// <summary>
        /// 根据ID获取班级对象
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        /// <returns>班级对象</returns>
        CEDTS_Class GetClassbyID(Guid ClassID);

        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="str">原始密码字符串</param>
        /// <returns>加密后的字符串</returns>
        string HashPassword(string str);

        /// <summary>
        /// 根据教师ID获取班级对象
        /// </summary>
        /// <param name="UserID">教师ID</param>
        /// <returns>班级对象</returns>
        List<CEDTS_Class> GetClassbyUserID(int UserID);

        /// <summary>
        /// 根據教師ID查詢班級列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<Class> SelectClassList(int id);
    }
}