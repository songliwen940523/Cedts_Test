using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Models
{
    public class KnowledgePointInfo
    {
        //知识点ID
        public List<Guid> KnowledgePointID { get; set; }
        //正确题数
        public List<int> CorrectCount { get; set; }
        //总计题数
        public List<int> TotalCount { get; set; }
        //回答知识点平均时间
        public List<double> AverageTime { get; set; }
    }

    public class StatisticsScore
    {
        //分数集合
        public List<double> Score { get; set; }
        //分数对应记录
        public List<int> Count { get; set; }
        //试卷名称
        public List<string> TypeName { get; set; }
    }

    public class Knowledge
    {

        //用户知识点掌握率
        public double UMi { get; set; }
        //系统知识点掌握率
        public double SMi { get; set; }
        //知识点名称
        public string KPName { get; set; }
    }
    public class ItemScale
    {
        //四级考试中各题型所占百分比
        public List<double> Fortysix { get; set; }
        //全体用户在各个题型的得分率
        public List<double> SScale { get; set; }
        //该学生在这个题型的得分率
        public List<double> UScale { get; set; }
    }
    public class KnowledgeInfo
    {
        public Guid KnowledgeID { get; set; }//ID
        public string Title { get; set; }//名称
    }

    public class TestRecord
    {
        public Guid TestID { get; set; }

        public DateTime FinishTime { get; set; }

        public string PaperName { get; set; }

        public double TotalScore { get; set; }

        public double Score { get; set; }

        public bool isFinish { get; set; }

        public int TestType { get; set; }

        public string Mark { get; set; }
    }

    public class AssignedTask
    {
        public Guid TaskID { get; set; }
        public Guid PaperID { get; set; }
        public string TaskName { get; set; }
        public DateTime AssignTime { get; set; }
        public bool IsFinished { get; set; }
    }

    public class Insufficient
    {

        //词汇能力
        public string Vocabulary { get; set; }
        //语法能力
        public string Grammar { get; set; }
        //理解能力
        public string Comprehend { get; set; }
    }

    public class ErrorQuestion
    {
        //题号
        public int? QuestionNum { get; set; }
        //标准答案
        public string SAnswer { get; set; }
        //用户答案
        public string UAnswer { get; set; }
        //解析
        public string Analyze { get; set; }
    }

    public class ErrorList
    {
        //得分
        public double Score { get; set; }
        //总分
        public double TotalScore { get; set; }
        //完成所用时间
        public int Time { get; set; }
        //评语
        public string Proposal { get; set; }
    }

    public class UserKnowledgeInfo
    {
        //知识点名称
        public List<string> KpName { get; set; }
        //题数
        public List<int> Num { get; set; }
        //耗时
        public List<int> Time { get; set; }
        //正确率
        public List<double> CorrectRate { get; set; }
        //掌握率
        public List<double> KPMaster { get; set; }
    }

    public class ItemInfo
    {
        //题型名称
        public List<string> ItemName { get; set; }
        //次数
        public List<int> Num { get; set; }
        //正确率
        public List<double> CorrectRate { get; set; }
    }
    public class SamePaper
    {
        //试卷名称
        public List<string> PaperName { get; set; }
        //试卷分数
        public List<double> Score { get; set; }
    }
}