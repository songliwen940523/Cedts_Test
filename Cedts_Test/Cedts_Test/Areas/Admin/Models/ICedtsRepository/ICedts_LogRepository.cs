using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_LogRepository
    {
        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <param name="userAccount">用户帐号</param>
        /// <returns>用户ID（int）</returns>
        int GetUserIDByUserAccount(string userAccount);

        /// <summary>
        /// 向Log表插入一条数据
        /// </summary>
        /// <param name="Action">操作的方法</param>
        /// <param name="ClientIP">客户端IP</param>
        /// <param name="Content">操作描述</param>
        /// <param name="UserID">操作人ID</param>
        void SaveLog(string Action, string ClientIP, string Content, int UserID);

        /// <summary>
        /// 查询日志记录
        /// </summary>
        /// <param name="id">分页页码</param>
        /// <returns>List(CEDTS_Log)</returns>
        PagedList<CEDTS_Log> SelectLog(int? id);

        /// <summary>
        /// 删除日志记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns>bool</returns>
        bool Delete(string ids);
    }
}