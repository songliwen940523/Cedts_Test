using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Models
{
    public interface ICedts_HomeRepository
    {
        /// <summary>
        /// 统计分数
        /// </summary>
        /// <returns></returns>
        StatisticsScore TotalScore();
        /// <summary>
        /// 统计知识点掌握率
        /// </summary>
        /// <returns></returns>
        List<Knowledge> KPMaster();

        /// <summary>
        /// 题型正确率
        /// </summary>
        /// <returns></returns>
        List<double> ItemInfo();

        /// <summary>
        /// 获取当前用户最近10次做题分数
        /// </summary>
        /// <param name="UserName">当前用户名</param>
        /// <returns></returns>
        StatisticsScore UserScore(string UserName);

        /// <summary>
        /// 获取当前用户知识点掌握率
        /// </summary>
        /// <param name="UserName">当前用户名</param>
        /// <returns></returns>
        List<Knowledge> UserKpMaster(string UserName);

        /// <summary>
        /// 获取当前用户题型正确率
        /// </summary>
        /// <param name="UserName">当前用户名</param>
        /// <returns></returns>
        ItemScale UserItemInfo(string UserName);

        UserKnowledgeInfo UserKnowledgeInfo(string UserName);

        ItemInfo UserItemList(string UserName);

        /// <summary>
        /// 首页概况
        /// </summary>
        /// <returns></returns>
        Front FrontInfo();
        /// <summary>
        /// 练习我们
        /// </summary>
        /// <returns></returns>
        ContactUsInfo ContactUsInfo();

        /// <summary>
        /// 特色功能
        /// </summary>
        /// <returns></returns>
        List<CEDTS_CoreFeatures> CoreFeatures();
        /// <summary>
        /// 意见反馈
        /// </summary>
        /// <param name="feed"></param>
        void Feedback(CEDTS_Feedback feed);

        /// <summary>
        /// 获取用户动态信息
        /// </summary>
        /// <returns>返回用户动态信息</returns>
        List<CEDTS_Testing> GetTestingInfo();

        /// <summary>
        /// 获取系统概况信息
        /// </summary>
        /// <returns></returns>
        string GetSys();

        /// <summary>
        /// 获取使用说明信息
        /// </summary>
        /// <returns></returns>
        string GetInstructions();
    }
}