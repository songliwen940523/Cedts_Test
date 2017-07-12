using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_CardRepository
    {
        /// <summary>
        /// 查询所有的卡信息
        /// </summary>
        /// <param name="id">分页ID</param>
        /// <param name="CKind">卡类别</param>
        /// <param name="CType">卡类型</param>
        /// <param name="AState">激活状态</param>
        /// <param name="SCondition">查询条件</param>
        /// <param name="TCondition">条件内容</param>
        /// <returns>卡对象集合</returns>
        PagedList<CEDTS_Card> SelectAllCard(int? id, int CKind, int CType, int AState, int SCondition, string TCondition);

        /// <summary>
        /// 获取院校对象集合
        /// </summary>
        /// <returns>院校对象集合</returns>
        List<CEDTS_Partner> SelectPartner();

        /// <summary>
        /// 单向加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的密码（string）</returns>
        string HashPassword(string str);

        /// <summary>
        /// 根据用户账户获取用户ID
        /// </summary>
        /// <param name="Account">用户账户</param>
        /// <returns>用户ID</returns>
        int GetUserIDbyAccount(string Account);

        /// <summary>
        /// 保存充值卡数据
        /// </summary>
        /// <param name="card">充值卡对象</param>
        void CreateCard(CEDTS_Card card);

        /// <summary>
        /// 删除充值卡信息
        /// </summary>
        /// <param name="id">充值卡ID</param>
        /// <returns>字符串结果，为1时为删除成功</returns>
        string DeleteCard(int id);

        /// <summary>
        /// 获取登录用户绑定的充值卡
        /// </summary>
        /// <param name="UserID">登录用户ID</param>
        /// <returns>登录用户绑定的充值卡集合</returns>
        PagedList<CEDTS_Card> GetCardByUserID(int UserID, int? id);

        /// <summary>
        /// 根据充值卡ID获取充值卡信息
        /// </summary>
        /// <param name="ID">充值卡ID</param>
        /// <returns>充值卡对象</returns>
        CEDTS_Card SelectCardByID(int ID);

        /// <summary>
        /// 更新卡对象
        /// </summary>
        /// <param name="card">更新后的卡对象</param>
        void UpdateCard(CEDTS_Card card);

        /// <summary>
        /// 验证充值卡和序列号
        /// </summary>
        /// <param name="SerialNumber">充值卡序列号</param>
        /// <param name="PassWord">充值卡密码</param>
        /// <returns>返回 Card对象</returns>
        CEDTS_Card CheckCard(string SerialNumber, string PassWord);

        /// <summary>
        /// 向用户和卡的关系表中插入一条数据
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="CardID">充值卡ID</param>
        void CreatUserCard(int UserID, int CardID);
    }
}
