using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_PaperRepository
    {
        /// <summary>
        /// 获取试卷列表
        /// </summary>
        /// <returns> List(CEDTS_Paper)</returns>
        PagedList<CEDTS_Paper> SelectPaper(int? id, int userid);

        /// <summary>
        /// 根据试卷ID获取试卷名称
        /// </summary>
        /// <returns> string</returns>
        CEDTS_Paper GetPaperByID(Guid PaperID);

        /// <summary>
        /// 根据试卷ID和用户ID获取试卷名称
        /// </summary>
        /// <returns> string</returns>
        CEDTS_Test GetTestByPaperID(Guid PaperID, int UserID);

        /// <summary>
        /// 分页得分列表
        /// </summary>
        /// <param name="ScoreInfo">得分列表</param>
        /// <returns>分页得分列表</returns>
        PagedList<ScoreInfo> ScoreInfoPaged(int? id, List<ScoreInfo> ScoreInfo);

        /// <summary>
        /// 分页得分列表
        /// </summary>
        /// <param name="ScoreInfo">得分列表</param>
        /// <returns>分页得分列表</returns>
        PagedList<SingleScoreInfo> SingleScoreInfoPaged(int? id, List<SingleScoreInfo> SingleScoreInfo);

        /// <summary>
        /// 自主出题选题列表分页
        /// </summary>
        /// <param name="AssessmentItemList">试题列表</param>
        /// <returns>分页试题列表</returns>
        PagedList<ExaminationItem> AssessmentItemPaged(int? id, List<ExaminationItem> AssessmentItemList);

        /// <summary>
        /// 根据时间和用户获取试卷ID列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UserID">用户ID</param>
        /// <returns> List(CEDTS_PaperAssignClass)</returns>
        List<Guid?> SelectPaperByPeriod(string StartTime, string EndTime, int UserID, Guid ClassID);

        /// <summary>
        /// 根据时间和用户获取测试数量
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UserID">用户ID</param>
        /// <returns> 试卷数</returns>
        int CountTestByPeriod(string StartTime, string EndTime, int UserID);

        /// <summary>
        /// 根据时间和用户获取测试列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UserID">用户ID</param>
        /// <returns> List(CEDTS_Test)</returns>
        List<CEDTS_Test> SelectTestByPeriod(string StartTime, string EndTime, int UserID);

        /// <summary>
        /// 根据时间和用户获取测试知识点列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="KnowledgeID">知识点ID</param>
        /// <returns> List(CEDTS_TestAnswerKnowledgePoint)</returns>
        List<CEDTS_TestAnswerKnowledgePoint> SelectTAKByPeriod(string StartTime, string EndTime, int UserID, Guid KnowledgeID);

        /// <summary>
        /// 根据时间和知识点获取测试知识点列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="KnowledgeID">知识点ID</param>
        /// <returns> List(CEDTS_TestAnswerKnowledgePoint)</returns>
        List<CEDTS_TestAnswerKnowledgePoint> SelectATAKByPeriod(string StartTime, string EndTime, Guid KnowledgeID);

        /// <summary>
        /// 根据时间和用户获取测试题型列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="TypeID">知识点ID</param>
        /// <returns> List(CEDTS_TestAnswerKnowledgePoint)</returns>
        List<CEDTS_TestAnswerTypeInfo> SelectTATByPeriod(string StartTime, string EndTime, int UserID, int TypeID);

        /// <summary>
        /// 根据时间和用户获取测试题型列表
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="TypeID">知识点ID</param>
        /// <returns> List(CEDTS_TestAnswerKnowledgePoint)</returns>
        List<CEDTS_TestAnswerTypeInfo> SelectATATByPeriod(string StartTime, string EndTime, int TypeID);

        /// <summary>
        /// 根据测试获取小题数
        /// <param name="TestID">测试ID</param>
        /// <returns>小题数</returns>
        int SelectQNByTestID(Guid TestID);

        /// <summary>
        /// 根据测试获取正确小题数
        /// <param name="TestID">测试ID</param>
        /// <returns>正确小题数</returns>
        int SelectCQNByTestID(Guid TestID);

        /// <summary>
        /// 查询选词填空选项
        /// </summary>
        /// <param name="id">AssessmentD</param>
        /// <returns>选项值</returns>
        string AnswerValue(Guid id, string value);

        /// <summary>
        /// 获取题目大类型
        /// </summary>
        /// <returns>List(CEDTS_PartType)</returns>
        List<CEDTS_PartType> SelectPartType();

        /// <summary>
        /// 获取题目小类型
        /// </summary>
        /// <returns>List(CEDTS_ItemType)</returns>
        List<CEDTS_ItemType> SelectItemType();

        /// <summary>
        /// 根据PartTypeID获取ItemType信息
        /// </summary>
        /// <param name="PartTypeID">题目大类型ID</param>
        /// <returns>List(CEDTS_ItemType)</returns>
        List<CEDTS_ItemType> SelectItemTypeByPartTypeID(int PartTypeID);

        /// <summary>
        /// 根据PaperID StudIDList获取WrongItemInfo列表
        /// </summary>
        /// <param name="PaperID">试卷ID</param>
        /// <param name="StudIDList">学生ID列表</param>
        /// <returns>List(WrongItemInfo)</returns>
        List<WrongItemInfo> SelectWIIByPUList(Guid PaperID, List<int> StudIDList);

        /// <summary>
        /// 根据PaperIDList StudIDList获取WrongItemInfo列表
        /// </summary>
        /// <param name="PaperIDList">试卷ID列表</param>
        /// <param name="StudIDList">学生ID列表</param>
        /// <returns>List(WrongItemInfo)</returns>
        List<WrongItemInfo> SelectWIIByPUList(List<Guid> PaperIDList, List<int> StudIDList);

        /// <summary>
        /// 根据PaperID StudIDList获取WrongKnowledgeInfo列表
        /// </summary>
        /// <param name="PaperID">试卷ID</param>
        /// <param name="StudIDList">学生ID列表</param>
        /// <returns>List(WrongKnowledgeInfo)</returns>
        List<WrongKnowledgeInfo> SelectWKIByPUList(Guid PaperID, List<int> StudIDList);

        /// <summary>
        /// 根据PaperIDList StudIDList获取WrongKnowledgeInfo列表
        /// </summary>
        /// <param name="PaperIDList">试卷ID列表</param>
        /// <param name="StudIDList">学生ID列表</param>
        /// <returns>List(WrongKnowledgeInfo)</returns>
        List<WrongKnowledgeInfo> SelectWKIByPUList(List<Guid> PaperIDList, List<int> StudIDList);

        /// <summary>
        /// 根据TestIDList StudIDList获取小题正确数列表
        /// </summary>
        /// <param name="TestIDList">测试ID列表</param>
        /// <param name="StudIDList">学生ID列表</param>
        /// <returns>List(QuestionDoneInfo)</returns>
        List<QuestionDoneInfo> SelectQDIByTUList(List<Guid> TestIDList, List<int> StudIDList, int ItemTypeID);

        /// <summary>
        /// 根据PaperID获取TestID列表
        /// </summary>
        /// <param name="PaperID">试卷ID</param>
        /// <returns>List(TestID)</returns>
        List<Guid> SelectTestIDListByPaperID(Guid PaperID);

        /// <summary>
        /// 获取题目集合（与知识点无关）
        /// </summary>
        /// <param name="itemList">各题型数量</param>
        /// <param name="userID">用户ID</param>
        /// <returns>试题集合</returns>
        List<CEDTS_AssessmentItem> SelectAssessmentItems(List<string> itemList, int userID);

        /// <summary>
        /// 获取试卷中所有的试题信息（根据知识点）
        /// </summary>
        /// <param name="knowList">知识点集合</param>
        /// <param name="itemList">试题集合</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        List<CEDTS_AssessmentItem> SelectAssessmentItems2(List<string> knowList, List<string> itemList, int userID);

        /// <summary>
        /// 根据等级获取知识点
        /// </summary>
        /// <param name="level">知识点等级</param>
        /// <param name="uperID">知识点上级ID</param>
        /// <returns>知识点对象集合</returns>
        List<CEDTS_KnowledgePoints> SelectKnowledges(int level, Guid? uperID);

        /// <summary>
        /// 根据知识点ID号获取知识点名称
        /// </summary>
        /// <param name="KnowledgeID">知识点ID</param>
        /// <returns>知识点名称</returns>
        string SelectKnowledgeName(Guid KnowledgeID);

        /// <summary>
        /// 获取知识点
        /// </summary>
        /// <returns>知识点对象集合</returns>
        List<CEDTS_KnowledgePoints> GetAllKnowledges();

        /// <summary>
        /// 获取知识点相关的小问题
        /// </summary>
        /// <param name="id">试题ID</param>
        /// <param name="knowledge">知识点范围集</param>
        /// <returns>小问题对象集合</returns>
        List<CEDTS_Question> SelectQuestionByknowledge(Guid id, List<Guid> knowledge);

        /// <summary>
        /// 获取所有老师自主出题列表
        /// </summary>
        /// <returns>试题对象集合</returns>
        List<ExaminationItem> GetExaminationItemsByTeacher();

        /// <summary>
        /// 查询选词填空答案内容
        /// </summary>
        /// <param name="id">AssessmentID</param>
        /// <param name="value">选项键</param>
        /// <returns>选项值</returns>
        string SelectAnswerValue(Guid id, string value);

        /// <summary>
        /// 保存试卷到数据库
        /// </summary>
        /// <param name="paper">试卷对象</param>
        void SavePaper(CEDTS_Paper paper);

        /// <summary>
        /// 保存发布作业记录到数据库
        /// </summary>
        /// <param name="paper">试卷对象</param>
        void SavePaperAssignClass(CEDTS_PaperAssignClass assignrecord);

        /// <summary>
        /// 保存试卷和试题的关系表
        /// </summary>
        /// <param name="pa">试卷和试题的关系表对象</param>
        void SavePaperAssessment(CEDTS_PaperAssessment pa);

        /// <summary>
        /// 保存暂存试卷信息
        /// </summary>
        /// <param name="PaperExpansion">暂存试卷信息</param>
        void SavePaperExpansion(CEDTS_PaperExpansion PaperExpansion);

        /// <summary>
        /// 查询暂存试卷信息
        /// </summary>
        /// <param name="id">暂存试卷ID</param>
        CEDTS_Paper SelectEditPaper(Guid id);

        /// <summary>
        /// 查询暂存表题目信息
        /// </summary>
        /// <param name="id">试卷ID</param>
        /// <returns></returns>
        CEDTS_PaperExpansion SelectEditPaperExpansion(Guid id);

        #region 试卷名称验证
        bool CheckName(string name);
        #endregion

        #region 试卷删除
        int DeletePaper(Guid id);
        #endregion

        string SelectName(Guid id);

        /// <summary>
        /// 获取题目大类型
        /// </summary>
        /// <returns>List(CEDTS_PartType)</returns>
        List<CEDTS_PartType> SelectEditPartType();

        /// <summary>
        /// 根据PartTypeID获取ItemType信息
        /// </summary>
        /// <param name="PartTypeID">题目大类型ID</param>
        /// <returns>List(CEDTS_ItemType)</returns>
        List<CEDTS_ItemType> SelectEditItemTypeByPartTypeID(int PartTypeID);

        /// <summary>
        /// 查询上次暂存该编辑跳转页面
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns></returns>
        int Judge(Guid id);

        /// <summary>
        /// 编辑时查询快速阅读信息
        /// </summary>
        /// <param name="id">试卷ID</param>
        /// <returns>快速阅读信息</returns>
        List<SkimmingScanningPartCompletion> SelectEditSspc(Guid id);

        /// <summary>
        /// 查询短对话听力信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>短对话听力信息</returns>
        List<Listen> SelectEditSlpo(Guid id);

        /// <summary>
        /// 查询长对话听力信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>长对话信息</returns>
        List<Listen> SelectEditLlpo(Guid id);

        /// <summary>
        /// 查询短文听力理解信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>短文听力理解信息</returns>
        List<Listen> SelectEditRlpo(Guid id);

        /// <summary>
        /// 查询复合型听力信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>复合型听力信息</returns>
        List<Listen> SelectEditLpc(Guid id);

        /// <summary>
        /// 查询阅读理解-选词填空信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>阅读理解-选词填空信息</returns>
        List<ReadingPartCompletion> SelectEditRpc(Guid id);

        /// <summary>
        /// 查询阅读理解-选择题型信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>阅读理解-选择题型信息</returns>
        List<ReadingPartOption> SlelectEditRpo(Guid id);

        /// <summary>
        /// 更新试卷信息
        /// </summary>
        /// <param name="paper"></param>
        void UpadatePaper(CEDTS_Paper paper);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PaperExpansion"></param>
        void UpdataPaperExpansion(CEDTS_PaperExpansion PaperExpansion);

        /// <summary>
        /// 创建试卷XML
        /// </summary>
        /// <param name="pt">试卷汇总信息对象</param>
        /// <returns>试卷内容</returns>
        string CreatePaperXML(PaperTotal pt);

        /// <summary>
        /// 获取试卷中的试题ID
        /// </summary>
        /// <param name="id">试卷ID</param>
        /// <returns>List(试题ID)</returns>
        List<CEDTS_PaperAssessment> SelectPaperAssessmentItem(Guid id);

        /// <summary>
        /// 获取试卷信息
        /// </summary>
        /// <param name="id">试卷ID</param>
        /// <returns>试卷对象</returns>
        CEDTS_Paper SelectPaper(Guid id);

        /// <summary>
        /// 获取试题信息
        /// </summary>
        /// <param name="id">试题ID</param>
        /// <returns>试题对象</returns>
        CEDTS_AssessmentItem SelectAssessmentItem(Guid id);

        /// <summary>
        /// 根据AssessmentID查询包含的题目数
        /// </summary>
        /// <param name="PaperID">AssessmentID</param>
        /// <returns>题目数</returns>
        int SelectQNByAssessmentID(Guid AssessmentID);

        /// <summary>
        /// 获取试题类型信息
        /// </summary>
        /// <param name="id">试题类型ID</param>
        /// <returns>试题类型对象</returns>
        CEDTS_ItemType SelectItemType(int id);

        /// <summary>
        /// 查询小问题的信息
        /// </summary>
        /// <param name="id">试题ID</param>
        /// <returns>小问题对象</returns>
        List<CEDTS_Question> SelectQuestion(Guid id);

        /// <summary>
        /// 获取选词填空选项内容
        /// </summary>
        /// <param name="id">试题ID</param>
        /// <returns>选项对象</returns>
        CEDTS_Expansion SelectExpansion(Guid id);

        /// <summary>
        /// 根据UserID，AssessmentItemID查询CEDTS_UserAssessmentCount是否存在记录
        /// </summary>
        /// <param name="UserID">UserID</param>
        /// <param name="AssessmentID">AssessmentID</param>
        /// <returns></returns>
        int SelectUA(int UserID, Guid AssessmentID);

        /// <summary>
        /// 根据UserID，AssessmentItemID查询CEDTS_UserAssessmentCount表信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="AssessmentID">AssessmentID</param>
        /// <returns></returns>
        CEDTS_UserAssessmentCount SelectUAC(int UserID, Guid AssessmentID);

        /// <summary>
        /// 根据UserID，TestID获取题型名称、比例、用户得分率等信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="TestID">TestID</param>
        /// <returns></returns>
        ItemByTeacherScale UserItemInfo(int UserID, Guid TestID);

        /// <summary>
        /// 根据UserID，TestID获取题型题数和用户答对题数
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="TestID">TestID</param>
        /// <returns></returns>
        ItemInfo ItemList(int UserID, Guid TestID);

        /// <summary>
        /// 获取题型中文列表
        /// </summary>
        /// <returns> List(CEDTS_ItemType)</returns>
        List<CEDTS_ItemType> ItemCNList();

        /// <summary>
        /// 根据用户和题型号获取题型数
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="TestID">TestID</param>
        /// <param name="ItemTypeID">ItemTypeID</param>
        /// <returns></returns>
        int ItemTestCount(int UserID, Guid TestID, int ItemTypeID);

        /// <summary>
        /// 根据用户和题型号获取正确题型数
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="TestID">TestID</param>
        /// <param name="ItemTypeID">ItemTypeID</param>
        /// <returns></returns>
        int ItemTestCorCount(int UserID, Guid TestID, int ItemTypeID);

        /// <summary>
        /// 根据用户和题型号获取知识点数
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="TestID">TestID</param>
        /// <param name="KnowledgePointID">KnowledgePointID</param>
        /// <returns></returns>
        int KnowledgeTestCount(int UserID, Guid TestID, Guid KnowledgePointID);

        /// <summary>
        /// 根据用户和题型号获取正确知识点数
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="TestID">TestID</param>
        /// <param name="KnowledgePointID">KnowledgePointID</param>
        /// <returns></returns>
        int KnowledgeTestCorCount(int UserID, Guid TestID, Guid KnowledgePointID);

        /// <summary>
        /// 根据UserID，TestID获取用户知识点掌握率
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="TestID">TestID</param>
        /// <returns></returns>
        List<Knowledge> UserKpMaster(int UserID, Guid TestID);

        /// <summary>
        /// 根据UserID，TestID获取用户知识点列表
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="TestID">TestID</param>
        /// <returns></returns>
        UserKnowledgeInfo KpList(int UserID, Guid TestID);

        /// <summary>
        /// 更新CEDTS_UserAssessmentCount信息
        /// </summary>
        /// <param name="UAC">CEDTS_UserAssessmentCount信息</param>
        void UpdataUAC(CEDTS_UserAssessmentCount UAC);

        /// <summary>
        /// 新增CEDTS_UserAssessmentCount表信息
        /// </summary>
        /// <param name="UAC">CEDTS_UserAssessmentCount表信息</param>
        void CreateUAC(CEDTS_UserAssessmentCount UAC);

    }
}
