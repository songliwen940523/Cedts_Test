using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_AdminRepository
    {
        /// <summary>
        /// 查询所有后台用户信息
        /// </summary>
        /// <returns></returns>
        PagedList<CEDTS_User> SelectUser(int? id);

        /// <summary>
        /// 查询当前点击的用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CEDTS_User SelectEditUser(int id);
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="user"></param>
        void CreateUser(CEDTS_User user);

        /// <summary>
        /// 编辑更新用户
        /// </summary>
        /// <param name="user"></param>
        void EditUser(CEDTS_User user);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int DeleteUser(string[] name);

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
        /// 密码加密
        /// </summary>
        /// <param name="str">原始密码字符串</param>
        /// <returns>加密后的字符串</returns>
        string HashPassword(string str);

    }
}