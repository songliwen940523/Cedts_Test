using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Models
{
    public interface ICedts_PaperRepository
    {

        /// <summary>
        /// 创建试卷XML
        /// </summary>
        /// <param name="pt">试卷汇总信息对象</param>
        /// <returns>试卷内容</returns>
        string CreatePaperXML(PaperTotal pt);


        /// <summary>
        /// 获取试卷列表
        /// </summary>
        /// <returns> List(CEDTS_Paper)</returns>
        PagedList<CEDTS_Paper> SelectPapers(int? id);

        /// <summary>
        /// 获取试卷信息
        /// </summary>
        /// <param name="id">试卷ID</param>
        /// <returns>试卷对象</returns>
        CEDTS_Paper SelectPaper(Guid id);

        /// <summary>
        /// 获取试卷中的试题ID
        /// </summary>
        /// <param name="id">试卷ID</param>
        /// <returns>List(试题ID)</returns>
        List<CEDTS_PaperAssessment> SelectPaperAssessmentItem(Guid id);

        /// <summary>
        /// 获取试题信息
        /// </summary>
        /// <param name="id">试题ID</param>
        /// <returns>试题对象</returns>
        CEDTS_AssessmentItem SelectAssessmentItem(Guid id);

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
        /// 获取知识点相关的小问题
        /// </summary>
        /// <param name="id">试题ID</param>
        /// <param name="knowledge">知识点范围集</param>
        /// <returns>小问题对象集合</returns>
        List<CEDTS_Question> SelectQuestionByknowledge(Guid id, List<Guid> knowledge);

        /// <summary>
        /// 获取选词填空选项内容
        /// </summary>
        /// <param name="id">试题ID</param>
        /// <returns>选项对象</returns>
        CEDTS_Expansion SelectExpansion(Guid id);

        /// <summary>
        /// 获取题目集合（与知识点无关）
        /// </summary>
        /// <param name="itemList">各题型数量</param>
        /// <param name="userID">用户ID</param>
        /// <returns>试题集合</returns>
        List<CEDTS_AssessmentItem> SelectAssessmentItems(List<string> itemList, int userID);

        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <param name="userAccount">用户帐号</param>
        /// <returns>userID</returns>
        int SelectUserID(string userAccount);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userAccount">用户帐号</param>
        /// <returns>userID</returns>
        CEDTS_User SelectUserInfo(string userAccount);

        /// <summary>
        /// 获取班级信息
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        /// <returns>userID</returns>
        CEDTS_Class GetClassbyID(Guid ClassID);

        /// <summary>
        /// 获取班级中的学生对象
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        /// <returns>学生对象</returns>
        List<CEDTS_User> SelectUserByClassID(Guid ClassID);

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
        PagedList<SingleScoreInfo> SingleScoreInfoPaged(int? id, List<SingleScoreInfo> SingleScoreInfo);

        /// <summary>
        /// 保存试卷信息
        /// </summary>
        /// <param name="paper">试卷对象</param>
        void SavePaper(CEDTS_Paper paper);

        /// <summary>
        /// 保存试卷和试题的关系
        /// </summary>
        /// <param name="pa">试卷和试题关系对象</param>
        void SavePaperAssessmentItem(CEDTS_PaperAssessment pa);

        /// <summary>
        /// 更新试卷信息
        /// </summary>
        /// <param name="paper">试卷对象</param>
        void UpdetPaper(CEDTS_Paper paper);

        /// <summary>
        /// 根据QuestionID查找KnowledgeID
        /// </summary>
        /// <param name="id">QuestionID</param>
        /// <returns>KnowledgeID</returns>
        List<Guid> SelectKnowledge(Guid id);

        /// <summary>
        /// 获取知识点弱项
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="Count">需要获取的知识点数量</param>
        /// <returns>知识点集合（List(GUID)）</returns>
        List<String> SelectBadKnowledge(int userID, int Count);

        /// <summary>
        /// 根据知识点ID.答卷ID查询TestAnswerKnowledge表
        /// </summary>
        /// <param name="id">KnowledgeID，答卷ID</param>
        /// <returns>TestAnswerKnowledge表</returns>
        CEDTS_TestAnswerKnowledgePoint SelectTestAKP(Guid KPID, Guid? PaperID);

        /// <summary>
        /// 新增答卷信息
        /// </summary>
        /// <param name="test">答卷信息</param>
        void CreateTest(CEDTS_Test test);

        /// <summary>
        /// 判断SampleKnowledgeInfo
        /// </summary>
        /// <param name="KPID">知识点ID</param>
        /// <param name="TestID">答卷ID</param>
        /// <returns></returns>
        int SelectSIK(Guid KPID);

        /// <summary>
        /// 根据KPID，PaperID查询CEDTS_SampleKnowledgeInfomation表
        /// </summary>
        /// <param name="KPID">知识点ID</param>
        /// <param name="PaperID">试卷ID</param>
        /// <returns></returns>
        CEDTS_SampleKnowledgeInfomation SelectSKI(Guid KPID);

        /// <summary>
        /// 根据KPID，UserID查询CEDTS_UserKnowledgeInfomation表
        /// </summary>
        /// <param name="KPID">知识点ID</param>
        /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        CEDTS_UserKnowledgeInfomation SelectUKI(Guid KPID, int? UserID);

        /// <summary>
        /// 根据用户ID，试题类型ID查询CEDTS_SmapleAnswerTypeInfo是否存在记录
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="ItemID">试题类型ID</param>
        /// <returns></returns>
        int SelectSATI(int? ItemID);

        /// <summary>
        /// 根据UserID，ItemID查询CEDTS_UserAnswerInfo表信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="ItemID">试题类型ID</param>
        /// <returns></returns>
        CEDTS_UserAnswerInfo SelectUAI(int? UserID, int? ItemID);

        /// <summary>
        /// 据用户ID，试题类型ID查询CEDTS_SmapleAnswerTypeInfo信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="ItemID">试题类型ID</param>
        /// <returns></returns>
        CEDTS_SmapleAnswerTypeInfo SelectSMAP(int? ItemID);

        /// <summary>
        /// 根据UserID，AssessmentItemID查询CEDTS_UserAssessmentCount是否存在记录
        /// </summary>
        /// <param name="UserID">UserID</param>
        /// <param name="AssessmentID">AssessmentID</param>
        /// <returns></returns>
        int SelectUA(int? UserID, Guid AssessmentID);

        /// <summary>
        /// 根据AssessmentItemID查询CEDTS_AssessmentItem出题人ID号
        /// </summary>
        /// <param name="AssessmentItemID">AssessmentItemID</param>
        /// <returns></returns>
        int SelectAU(Guid AssessmentID);

        /// <summary>
        /// 根据PaperID查询CEDTS_Paper的状态State
        /// </summary>
        /// <param name="PaperID">PaperID</param>
        /// <returns></returns>
        int SelectPS(Guid PaperID);

        /// <summary>
        /// 根据UserID，AssessmentItemID查询CEDTS_UserAssessmentCount表信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="AssessmentID">AssessmentID</param>
        /// <returns></returns>
        CEDTS_UserAssessmentCount SelectUAC(int? UserID, Guid AssessmentID);

        /// <summary>
        /// 更新Test表信息
        /// </summary>
        /// <param name="TestID">Test表信息</param>
        void UpdataTest(CEDTS_Test test);

        /// <summary>
        /// 新增TestAnswer表信息
        /// </summary>
        /// <param name="TestAnswer">TestAnswer表信息</param>
        void CreateTestAnswer(CEDTS_TestAnswer TestAnswer);

        /// <summary>
        /// 新增CEDTS_TestAnswerKnowledgePoint表信息
        /// </summary>
        /// <param name="TAKP">CEDTS_TestAnswerKnowledgePoint表信息</param>
        void CreateTAKP(CEDTS_TestAnswerKnowledgePoint TAKP);

        /// <summary>
        /// 新增CEDTS_UserKnowledgeInfomation信息
        /// </summary>
        /// <param name="UKI">CEDTS_UserKnowledgeInfomation信息</param>
        void CreateUKI(CEDTS_UserKnowledgeInfomation UKI);

        /// <summary>
        /// 更新CEDTS_UserKnowledgeInfomation信息
        /// </summary>
        /// <param name="UKI">CEDTS_UserKnowledgeInfomation信息</param>
        void UpdataUKI(CEDTS_UserKnowledgeInfomation UKI);

        /// <summary>
        /// 新增CEDTS_SampleKnowledgeInfomation信息
        /// </summary>
        /// <param name="SKI">CEDTS_UserKnowledgeInfomation信息</param>
        void CreateSKI(CEDTS_SampleKnowledgeInfomation SKI);

        /// <summary>
        /// 更新CEDTS_SampleKnowledgeInfomation信息
        /// </summary>
        /// <param name="SKI">CEDTS_UserKnowledgeInfomation信息</param>
        void UpdataSKI(CEDTS_SampleKnowledgeInfomation SKI);

        /// <summary>
        /// 新增CEDTS_TestAnswerTypeInfo信息
        /// </summary>
        /// <param name="TAPI">CEDTS_TestAnswerTypeInfo信息</param>
        void CreateTATI(CEDTS_TestAnswerTypeInfo TATI);

        /// <summary>
        /// 新增CEDTS_UserAnswerInfo信息
        /// </summary>
        /// <param name="UATI">CEDTS_UserAnswerInfo信息</param>
        void CreateUATI(CEDTS_UserAnswerInfo UATI);

        /// <summary>
        /// 更新CEDTS_UserAnswerInfo信息
        /// </summary>
        /// <param name="UATI">CEDTS_UserAnswerInfo信息</param>
        void UpdataUATI(CEDTS_UserAnswerInfo UATI);

        /// <summary>
        /// 新增CEDTS_SmapleAnswerTypeInfo信息
        /// </summary>
        /// <param name="SATI">CEDTS_SmapleAnswerTypeInfo信息</param>
        void CreateSATI(CEDTS_SmapleAnswerTypeInfo SATI);

        /// <summary>
        /// 更新CEDTS_SmapleAnswerTypeInfo信息
        /// </summary>
        /// <param name="SATI">CEDTS_SmapleAnswerTypeInfo信息</param>
        void UpdataSATI(CEDTS_SmapleAnswerTypeInfo SATI);

        /// <summary>
        /// 新增CEDTS_UserAssessmentCount表信息
        /// </summary>
        /// <param name="UAC">CEDTS_UserAssessmentCount表信息</param>
        void CreateUAC(CEDTS_UserAssessmentCount UAC);

        /// <summary>
        /// 更新CEDTS_UserAssessmentCount信息
        /// </summary>
        /// <param name="UAC">CEDTS_UserAssessmentCount信息</param>
        void UpdataUAC(CEDTS_UserAssessmentCount UAC);

        /// <summary>
        /// 查询当前用户是否做过此类型试题
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="ItemID">试题ID</param>
        /// <returns></returns>
        int SelectUAINum(int? UserID, int? ItemID);

        /// <summary>
        /// 查询当前用户是否做过当前知识点
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <param name="KPID">知识点ID</param>
        /// <returns></returns>
        int SelectUkINum(int? UserID, Guid? KPID);

        /// <summary>
        /// 查询出知识点对应的掌握率
        /// </summary>
        /// <param name="k">知识点名称</param>
        /// <param name="userID">用户ID</param>
        /// <returns>个人对于单个知识点的掌握率</returns>
        double SelectKP_MasterRate(string k, int userID);

        /// <summary>
        /// 根据名称获取知识点
        /// </summary>
        /// <param name="k">知识点名称</param>
        /// <returns>知识点</returns>
        string SelectKnowledgeIDs(string k);

        /// <summary>
        /// 根据班级ID号获取发布的作业信息
        /// </summary>
        /// <param name="ClassID">班级ID号</param>
        /// <returns>PaperAssignClass</returns>
        List<CEDTS_PaperAssignClass> SelectAssignedPaper(Guid ClassID);

        /// <summary>
        /// 根据PaperID号获取测试信息
        /// </summary>
        /// <param name="PaperID">PaperID号</param>
        /// <returns>int</returns>
        int SelectTestByPaperID(Guid PaperID, int UserID);

        /// <summary>
        /// 根据PaperID号获取TestID
        /// </summary>
        /// <param name="PaperID">PaperID号</param>
        /// <returns>PaperAssignClass</returns>
        Guid SelectTestIDByPaperID(Guid PaperID, int UserID);

        /// <summary>
        /// 根据等级获取知识点
        /// </summary>
        /// <param name="level">知识点等级</param>
        /// <param name="uperID">知识点上级ID</param>
        /// <returns>知识点对象集合</returns>
        List<CEDTS_KnowledgePoints> SelectKnowledges(int level, Guid? uperID);

        /// <summary>
        /// 根据知识点ID获取知识点对象
        /// </summary>
        /// <param name="id">知识点ID</param>
        /// <returns>知识点对象</returns>
        CEDTS_KnowledgePoints SelectKnowledgeByID(Guid id);

        /// <summary>
        /// 获取题目大类型
        /// </summary>
        /// <returns>List(CEDTS_PartType)</returns>
        List<CEDTS_PartType> SelectPartType();

        /// <summary>
        /// 根据PartTypeID获取ItemType信息
        /// </summary>
        /// <param name="PartTypeID">题目大类型ID</param>
        /// <returns>List(CEDTS_ItemType)</returns>
        List<CEDTS_ItemType> SelectItemTypeByPartTypeID(int PartTypeID);

        /// <summary>
        /// 根据用户ID查询最近5次答卷信息
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <returns>答卷信息</returns>
        PagedList<TestRecord> TestRecord(int? id, int UserID);

        /// <summary>
        /// 根据用户班级ID查询未做的作业
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <returns>答卷信息</returns>
        PagedList<AssignedTask> AssignedTaskPaged(int? id, List<AssignedTask> AssignedTask);

        /// <summary>
        /// 学习应加强方面
        /// </summary>
        /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        Insufficient SelectInsufficient(int UserID);

        List<CEDTS_TestAnswer> GetTestAnswer(Guid TestID);

        /// <summary>
        /// 根据TestID查询PaperID
        /// </summary>
        /// <param name="TestID"></param>
        /// <returns></returns>
        Guid SelectPaperID(Guid TestID);

        List<Knowledge> UserKpMaster(Guid TestID, string UserName);

        ItemScale UserItemInfo(Guid TestID, string UserName);

        List<ErrorQuestion> ErrorQuestion(Guid TestID);

        ErrorList SelectError(Guid TestID, int UserID);

        UserKnowledgeInfo KpList(Guid id, string UserName);

        ItemInfo ItemList(Guid id, string UserName);

        /// <summary>
        /// 查询答案选项内容
        /// </summary>
        /// <param name="id">QuestionID</param>
        /// <param name="Content">选项</param>
        /// <returns>选项内容</returns>
        string AnswerContent(Guid id, string Content);

        /// <summary>
        /// 获取试卷信息集合
        /// </summary>
        /// <returns>试卷信息集合</returns>
        List<CEDTS_Paper> GetPapers();

        /// <summary>
        /// 查询选词填空选项
        /// </summary>
        /// <param name="id">AssessmentD</param>
        /// <returns>选项值</returns>
        string AnswerValue(Guid id, string value);

        /// <summary>
        ///查询
        /// </summary>
        /// <param name="id">TestID</param>
        /// <returns>PaperID</returns>
        Guid SelectPaperCount(Guid id);

        /// <summary>
        /// 根据TestID查询PaperName
        /// </summary>
        /// <param name="id">TestID</param>
        /// <returns>试卷名称</returns>
        string SelectPaperName(Guid id);

        /// <summary>
        /// 查询同一试卷成绩分布图
        /// </summary>
        /// <param name="id">TestID</param>
        /// <returns>同一试卷List集合</returns>
        SamePaper SelectSamePaperList(Guid id,int UserID);

        /// <summary>
        /// 查询试卷练习次数是否超过2次
        /// </summary>
        /// <param name="id">TestID</param>
        /// <returns></returns>
        string SelectCount(Guid id,int UserID);

        /// <summary>
        /// 得分率分析
        /// </summary>
        /// <param name="id">UserID</param>
        /// <returns></returns>
        string RateAnalysis(int id);

        /// <summary>
        /// 知识点分析
        /// </summary>
        /// <param name="id">UserID</param>
        /// <returns></returns>
        string KpAnalysis(int id);

        /// <summary>
        /// 单份试卷成绩分析
        /// </summary>
        /// <param name="id">TestID</param>
        /// <param name="UserID">UserID</param>
        /// <returns></returns>
        string SingleRateAnalysis(Guid id, int UserID);

        string SingleKpAnalysis(Guid id, int UserID);

        /// <summary>
        /// 查询选词填空答案内容
        /// </summary>
        /// <param name="id">AssessmentID</param>
        /// <param name="value">选项键</param>
        /// <returns>选项值</returns>
        string SelectAnswerValue(Guid id, string value);

        /// <summary>
        /// 查询当前用户是否练习过试卷
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        string SelectPaper(int id);

        /// <summary>
        /// 查询当前用户角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string SelectRole(string name);

        /// <summary>
        /// 获取试卷中所有的试题信息（根据知识点）
        /// </summary>
        /// <param name="knowList">知识点集合</param>
        /// <param name="itemList">试题集合</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        List<CEDTS_AssessmentItem> SelectAssessmentItems2(List<string> knowList, List<string> itemList, int userID);
        List<CEDTS_AssessmentItem> SelectAssessmentItems3(List<string> knowList, List<string> itemList, int userID);
       
        /// <summary>
        /// 根据testID获取Test对象
        /// </summary>
        /// <param name="testID">测试ID</param>
        /// <returns>test对象</returns>
        CEDTS_Test SelectTestInfo(Guid testID);

        /// <summary>
        /// 根据测试ID获取试题信息集合
        /// </summary>
        /// <param name="testID">测试ID</param>
        /// <returns>CEDTS_tempTestAnswer对象集合</returns>
        List<CEDTS_tempTestAnswer> getTempTestAnswerList(Guid testID);

        /// <summary>
        /// 根据测试ID获取答题时间表对象集合
        /// </summary>
        /// <param name="testID">测试ID</param>
        /// <returns>CEDTS_tempAssessmentItemTime对象集合</returns>
        List<CEDTS_tempAssessmentItemTime> getTempAssessmentTime(Guid testID);

        /// <summary>
        /// 試卷暫存數據
        /// </summary>
        /// <param name="TemptestAnswer"></param>
        void CreateTempTestAnswer(CEDTS_tempTestAnswer TemptestAnswer);

        /// <summary>
        /// 試卷暫存時間
        /// </summary>
        /// <param name="TestTime"></param>
        void CreateTestTime(CEDTS_tempAssessmentItemTime TempTestTime);

        ///// <summary>
        ///// 試卷暫存更新
        ///// </summary>
        ///// <param name="TestID">答卷ID</param>
        ///// <param name="IsFinish">是否完成</param>
        ///// <param name="TotalTime">答卷時間</param>
        ///// <param name="FinishDate">完成時間</param>
        //void UpdateTest(Guid TestID, bool IsFinish, float TotalTime, DateTime FinishDate);

        /// <summary>
        /// 刪除暫存試卷時間表
        /// </summary>
        /// <param name="TestID">答卷ID</param>
        void DeleteTempTime(Guid TestID);

        /// <summary>
        /// 刪除暫存試卷答題記錄
        /// </summary>
        /// <param name="TestID">答卷ID</param>
        void DeleteTempTest(Guid TestID);

        /// <summary>
        /// 读取所有学习者的知识点掌握率
        /// </summary>
        List<Array> getAllKnowledgeRate();

        void insertStatusToTable(CEDTS_LearnerStatus LearnerStatus);

        string selectStatus(int userId);
 }

}
