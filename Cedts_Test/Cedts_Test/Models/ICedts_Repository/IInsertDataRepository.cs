using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Cedts_Test.Models
{
    public interface IInsertDataRepository
    {
        /// <summary>
        /// 根据QuestionID查找KPID
        /// </summary>
        /// <param name="ID">QuestionID</param>
        /// <returns></returns>
        string KpID(Guid ID);

        /// <summary>
        /// 获取试题统计表（视图）
        /// </summary>
        /// <returns>获取视图中的表格</returns>
        DataTable GetTable();

        /// <summary>
        /// 根据KnowledgeID,TestID查询知识点时间   
        /// </summary>
        /// <param name="KpID">知识点ID</param>
        /// <param name="TestID">答卷ID</param>
        /// <returns>知识点时间</returns>
        double selectQuestionKp(Guid KpID, Guid TestID);

        List<Excel1> selectTestAnswer();
    }
}
