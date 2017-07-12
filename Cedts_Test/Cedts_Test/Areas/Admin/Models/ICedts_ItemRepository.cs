using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_ItemRepository
    {
        /// <summary>
        /// 添加听力题目
        /// </summary>
        /// <param name="item">一个听力选择题题目实体</param>
        void AddItem(Listen listen);

        /// <summary>
        /// 添加复合型听力
        /// </summary>
        /// <param name="complex">复合型听力信息</param>
        void AddComplexItem(Listen complex);

        /// <summary>
        /// 快速阅读
        /// </summary>
        /// <param name="Sspc">快速阅读题型信息</param>
        void AddSspcItem(SkimmingScanningPartCompletion Sspc);

        /// <summary>
        /// 添加快速阅读-选择题型
        /// </summary>
        /// <param name="Rpo">选择题型信息</param>
        void AddRpoItem(ReadingPartOption Rpo);

        ///// <summary>
        ///// 添加快速阅读-简答题型Short Answer Questions
        ///// </summary>
        ///// <param name="Saq">简答题型信息</param>
        //void AddSaqItem(SkimmingScanningPartBool Saq);

        /// <summary>
        /// 添加完形填空
        /// </summary>
        /// <param name="Cloze">完形填空信息</param>
        void AddClozeItem(ClozePart Cloze);

        /// <summary>
        /// 添加阅读理解-精读信息
        /// </summary>
        /// <param name="Rpo">阅读理解-精读信息</param>
        void AddIntensiveRead(ReadingPartOption Rpo);

        /// <summary>
        /// 添加阅读理解-选词填空
        /// </summary>
        /// <param name="Rpc">阅读理解-选词填空信息</param>
        void AddRpcItem(ReadingPartCompletion Rpc);

        /// <summary>
        /// 保存题目信息到数据库
        /// </summary>
        /// <param name="item">数据库试题实体</param>
        void Create(CEDTS_AssessmentItem item);

        /// <summary>
        /// 获取听力部分信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>ListenPartOption类对象信息</returns>
        Listen SelectAll(Guid id);

        /// <summary>
        /// 获取复合听力信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>ListenPartCompletion类对象信息</returns>
        Listen SelectComplex(Guid id);

        /// <summary>
        /// 获取快速阅读信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>SkimmingScanningPartBool信息</returns>
        SkimmingScanningPartCompletion SelectSspc(Guid id);

        /// <summary>
        /// 获取完型填空信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>ClozePart信息</returns>
        ClozePart SelectCloze(Guid id);

        /// <summary>
        /// 获取阅读理解-选择题型信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>ReadingPartOption信息</returns>
        ReadingPartOption SelectRpo(Guid id);

        /// <summary>
        /// 获取阅读理解-选词填空信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>ReadingPartCompletion信息</returns>
        ReadingPartCompletion SlelectRpc(Guid id);

        /// <summary>
        /// 编辑听力
        /// </summary>
        /// <param name="listen"></param>
        void EditItem(Listen listen);

        /// <summary>
        /// 编辑复合型听力信息
        /// </summary>
        /// <param name="Complex"></param>
        void EditComplex(Listen Complex);

        /// <summary>
        /// 编辑快速阅读信息
        /// </summary>
        /// <param name="Sspc"></param>
        void EditSspc(SkimmingScanningPartCompletion Sspc);

        /// <summary>
        /// 编辑完型填空信息
        /// </summary>
        /// <param name="Cloze"></param>
        void EditCloze(ClozePart Cloze);

        /// <summary>
        /// 编辑阅读理解-选择题型信息
        /// </summary>
        /// <param name="Rpo">选择题型信息</param>
        void EditRpo(ReadingPartOption Rpo);

        /// <summary>
        /// 编辑阅读理解-选词填空信息
        /// </summary>
        /// <param name="Rpc">选词填空信息</param>
        void EditRpc(ReadingPartCompletion Rpc);

        /// <summary>
        /// 编辑时获取ItemID
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>ItemID</returns>
        int? GetEditItemID(Guid id);

        /// <summary>
        /// 更改题目
        /// </summary>
        /// <param name="item">一个题目实体</param>
        void UpdateItem(CEDTS_AssessmentItem item);

        /// <summary>
        /// 删除题目
        /// </summary>
        /// <param name="item">一个题目实体</param>
        int DeleteItem(Guid? id);

        /// <summary>
        /// 根据查询条件获取试题列表
        /// </summary>
        /// <param name="id">分页ID</param>
        /// <param name="condition">条件分类名</param>
        /// <param name="txt">条件内容</param>
        /// <returns>试题列表</returns>
        PagedList<ExaminationItem> SelectItemsByCondition(int? id, string condition, string txt);

        /// <summary>
        /// 新增后返回当前选中的
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<SelectListItem> GetPart(int id);

        /// <summary>
        /// 新增后返回当前选中的Item
        /// </summary>
        /// <param name="partid">PartID</param>
        /// <param name="id">ItemID</param>
        /// <returns></returns>
        List<SelectListItem> GetItem(int partid,int id);

        /// <summary>
        /// 首次加载是获取Item
        /// </summary>
        /// <param name="id">PartID</param>
        /// <returns></returns>
        List<CEDTS_ItemType> FirstItem(int? id);

        /// <summary>
        /// 获取知识点
        /// </summary>
        /// <param name="PartType">PartTypeID</param>
        /// <returns>string（Content）</returns>
        string GetPoint();

        ///// <summary>
        ///// 编辑时获取知识点
        ///// </summary>
        ///// <param name="PartType">PartName</param>
        ///// <returns>string（Content）</returns>
        //string EditGetPoint();

        /// <summary>
        /// l联动获取Item值
        /// </summary>
        /// <param name="id">PartID</param>
        /// <returns>集合</returns>
        List<SelectListItem> GetItems(int? id);

        /// <summary>
        /// 获取所有PartType信息
        /// </summary>
        /// <returns>list</returns>
        IQueryable<CEDTS_PartType> GetPartType();

        /// <summary>
        /// 获取Item英文名称
        /// </summary>
        /// <param name="id">ItemID</param>
        /// <returns>返回Item英文名称</returns>
        string ItemName(int id);
        /// <summary>
        /// 获取Item中文名称
        /// </summary>
        /// <param name="id">ItemID</param>
        /// <returns>Item中文名称</returns>
        string ItemName_CN(int id);
        /// <summary>
        /// 获取Part英文名称
        /// </summary>
        /// <param name="id">PartID</param>
        /// <returns>Part英文名称</returns>
        string PartName(int id);

        /// <summary>
        /// 根据ItemName获取ItemID
        /// </summary>
        /// <param name="name">ItemName</param>
        /// <returns>ItemID</returns>
        int GetItemID(string name);

        /// <summary>
        /// 根据UserAccount查询UserID
        /// </summary>
        /// <param name="UserAccount">用户帐号</param>
        /// <returns>用户ID</returns>
        int SelectUserID(string UserAccount);
    }
}
