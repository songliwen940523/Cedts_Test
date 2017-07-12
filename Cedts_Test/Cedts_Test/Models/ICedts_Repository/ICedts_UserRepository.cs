using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cedts_Test.Models
{
    public interface ICedts_UserRepository
    {
        /// <summary>
        /// 检查用户是否已经存在
        /// </summary>
        /// <param name="Account">用户帐号</param>
        /// <returns>bool</returns>
        bool AjaxCheckAccount(string Account, int UserID);

        /// <summary>
        /// 检查用户邮箱是否已经存在
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        bool AjaxCheckEmail(string Email, int UserID);

        /// <summary>
        /// 把用户数据添加到数据库
        /// </summary>
        /// <param name="user">一个用户</param>
        void Create(CEDTS_User user);

        /// <summary>
        /// 登录验证帐号密码是否匹配
        /// </summary>
        /// <param name="Account">帐号</param>
        /// <param name="Password">密码</param>
        /// <returns></returns>
        bool LogOn(string Account, string Password);

        /// <summary>
        /// 通过帐号获取角色
        /// </summary>
        /// <param name="Account">帐号</param>
        /// <returns>角色</returns>
        string GetRoleByAccount(string Account);

        /// <summary>
        /// 根据Email获取用户信息
        /// </summary>
        /// <param name="Email">邮箱地址</param>
        /// <returns>user</returns>
        CEDTS_User GetUser(string Email);

        /// <summary>
        /// 生成一个随机数
        /// </summary>
        /// <returns>string</returns>
        string RandNum();

        /// <summary>
        /// 保存到数据库
        /// </summary>
        void Save();

        /// <summary>
        /// 单向加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的密码（string）</returns>
        string HashPassword(string str);

        /// <summary>
        /// 获取Partner对象集合
        /// </summary>
        /// <returns>Partner对象集合</returns>
        List<CEDTS_Partner> GetPartner();

        /// <summary>
        /// 获取Major对象集合
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>Major对象集合</returns>
        List<CEDTS_Major> GetMajor(Guid PartnerID);

        /// <summary>
        /// 获取Grade对象集合
        /// </summary>
        /// <param name="Major">专业ID</param>
        /// <returns>Grade对象集合</returns>
        List<CEDTS_Grade> GetGrade(Guid MajorID);

        /// <summary>
        /// 获取Class对象集合
        /// </summary>
        /// <param name="Grade">年级ID</param>
        /// <returns>Class对象集合</returns>
        List<CEDTS_Class> GetClass(Guid GradeID);

        /// <summary>
        /// 根据用户帐号获取用户信息
        /// </summary>
        /// <param name="Account">用户帐号</param>
        /// <returns>用户对象</returns>
        CEDTS_User GetUserByAccount(string Account);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user">用户对象</param>
        void ChangeUser(CEDTS_User user);

        /// <summary>
        /// 获取普通用户信息集合
        /// </summary>
        /// <returns>普通用户信息集合</returns>
        List<CEDTS_User> GetUserInfo();

        /// <summary>
        /// 创建游客
        /// </summary>
        /// <param name="user"></param>
        void CreateUser(CEDTS_User user);

        /// <summary>
        /// 查询当前用户角色
        /// </summary>
        /// <param name="name">用户名</param>
        /// <returns>用户角色</returns>
        string SelectRole(string name);
    }
}
