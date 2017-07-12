using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_NoticeRepository
    {
        /// <summary>
        /// 获取系统概况信息
        /// </summary>
        /// <returns>系统概况对象</returns>
        CEDTS_SystemOverview GetSystemInfo();

        /// <summary>
        /// 修改系统概况信息
        /// </summary>
        /// <param name="content">需要修改的内容</param>
        /// <param name="intro">需要修改的简介</param>
        void EditSystem(string content, string intro);

        /// <summary>
        /// 获取系统特色对象集合
        /// </summary>
        /// <returns>系统特色对象集合</returns>
        List<CEDTS_CoreFeatures> GetFeaturesInfo();

        /// <summary>
        /// 修改系统特色
        /// </summary>
        /// <param name="featuresList">系统特色集合</param>
        void EditFeatures(List<CEDTS_CoreFeatures> featuresList);

        /// <summary>
        /// 获取使用说明信息
        /// </summary>
        /// <returns>使用说明对象</returns>
        CEDTS_Instructions GetInstructionsInfo();

        /// <summary>
        /// 修改使用说明信息
        /// </summary>
        /// <param name="content">使用说明内容</param>
        void EidtInstructions(string content);

        /// <summary>
        /// 获取联系方式
        /// </summary>
        /// <returns>联系方式对象</returns>
        CEDTS_Contact GetContactInfo();

        /// <summary>
        /// 修改联系方式
        /// </summary>
        /// <param name="contact">修改后的联系方式对象</param>
        void EidtContact(CEDTS_Contact contact);

        /// <summary>
        /// 获取名人名言对象
        /// </summary>
        /// <returns>名人名言对象</returns>
        CEDTS_Saying GetSayingInfo();

        /// <summary>
        /// 修改名人名言
        /// </summary>
        /// <param name="content">名言</param>
        /// <param name="note">名人</param>
        void EidtSaying(string content, string note);

        /// <summary>
        /// 获取意见反馈信息
        /// </summary>
        /// <returns>返回意见反馈信息集合</returns>
        List<CEDTS_Feedback> GetFeedback();

        /// <summary>
        /// 删除意见反馈信息
        /// </summary>
        /// <param name="id">意见信息ID</param>
        void DeleteFeedBackbyID(int id);

        /// <summary>
        /// 清空所有意见反馈信息
        /// </summary>
        void DeleteFeedBackAll();
    }
}
