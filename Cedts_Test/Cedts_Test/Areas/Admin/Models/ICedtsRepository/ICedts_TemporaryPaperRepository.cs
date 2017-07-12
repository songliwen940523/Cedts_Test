using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_TemporaryPaperRepository
    {
 

        /// <summary>
        /// 查询暂存表题目信息
        /// </summary>
        /// <param name="id">试卷ID</param>
        /// <returns></returns>
        CEDTS_PaperExpansion SelectEditPaperExpansion(Guid id);


        /// <summary>
        /// 保存试卷和试题的关系表
        /// </summary>
        /// <param name="pa">试卷和试题的关系表对象</param>
        void SavePaperAssessment(CEDTS_TemporaryPaperAssessment tpa);


        /// <summary>
        /// 查询暂存试卷信息
        /// </summary>
        /// <param name="id">暂存试卷ID</param>
        CEDTS_Paper SelectEditPaper(Guid id);



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
        List<SkimmingScanningPartCompletion> SelectEditSspc(Guid id, int UserID);

        /// <summary>
        /// 查询短对话听力信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>短对话听力信息</returns>
        List<Listen> SelectEditSlpo(Guid id, int UserID);

        /// <summary>
        /// 查询长对话听力信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>长对话信息</returns>
        List<Listen> SelectEditLlpo(Guid id, int UserID);

        /// <summary>
        /// 查询短文听力理解信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>短文听力理解信息</returns>
        List<Listen> SelectEditRlpo(Guid id, int UserID);

        /// <summary>
        /// 查询复合型听力信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>复合型听力信息</returns>
        List<Listen> SelectEditLpc(Guid id, int UserID);

        /// <summary>
        /// 查询阅读理解-选词填空信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>阅读理解-选词填空信息</returns>
        List<ReadingPartCompletion> SelectEditRpc(Guid id, int UserID);

        /// <summary>
        /// 查询阅读理解-选择题型信息
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>阅读理解-选择题型信息</returns>
        List<ReadingPartOption> SlelectEditRpo(Guid id, int UserID);

        /// <summary>
        /// 查询阅读理解-信息匹配
        /// </summary>
        /// <param name="id">PaperID</param>
        /// <returns>阅读理解-信息匹配</returns>
        List<InfoMatchingCompletion> SlelectEditInfoMat(Guid id, int UserID);
    }
}