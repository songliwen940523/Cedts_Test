using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_TestRepository
    {
        /// <summary>
        /// 获取Test表信息
        /// </summary>
        /// <returns>返回Test对象集合</returns>
        List<CEDTS_Test> SelectTest();

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <returns>用户名</returns>
        string GetUserName(int UserID);

        /// <summary>
        /// 获取试卷名称
        /// </summary>
        /// <param name="PaperID">试卷ID</param>
        /// <returns>试卷名称</returns>
        string GetPaperTitle(Guid PaperID);

        /// <summary>
        /// 根据TestID获取用户答案
        /// </summary>
        /// <param name="TestID">测试ID</param>
        /// <returns>CEDTS_TestAnswer对象集合</returns>
        List<CEDTS_TestAnswer> GetTestAnswer(Guid TestID);

        /// <summary>
        /// 获取test表对象
        /// </summary>
        /// <param name="testID">测试ID</param>
        /// <returns>test对象</returns>
        CEDTS_Test GetTest(Guid testID);

        /// <summary>
        /// 想Test表中添加评阅
        /// </summary>
        /// <param name="test">test对象</param>
        void UpdateTest(CEDTS_Test test);
    }
}