using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class ScoreInfo
    {
        public int UserID { get; set; }
        public string StudNum { get; set; }
        public string StudName { get; set; }
        public List<string> TestNames { get; set; }
        public List<double> TestScore { get; set; }
        public int DoneNum { get; set; }
        public int CorrectNum { get; set; }
        public double DoneScore { get; set; }
        public double AveScore { get; set; }
    }
    public class SingleScoreInfo
    {
        public int UserID { get; set; }
        public string StudNum { get; set; }
        public string StudName { get; set; }
        public double Score { get; set; }
        public double TotalScore { get; set; }
        public double Percent { get; set; }
        public bool Done { get; set; }
    }
    public class SingleStatistics
    {
        public double AvePercent { get; set; }
        public double AveScore { get; set; }
        public double Variance { get; set; }
        public double StandardDeviation { get; set; }
        public double HighestScore { get; set; }
        public string HighestStudName { get; set; }
        public double LowestScore { get; set; }
        public string LowestStudName { get; set; }
        public int DoNum { get; set; }
        public int UndoNum { get; set; }
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
    public class ItemByTeacherScale
    {
        //试卷中该题型所占百分比
        public List<double> Fortysix { get; set; }
        //用户在该题型的得分率
        public List<double> UScale { get; set; }
        //题型名称
        public List<string> ItemName { get; set; }
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
    public class WrongItemInfo
    {
        public int ItemTypeNum { get; set; }
        public string ItemName { get; set; }
        public int TotalNum { get; set; }
        public double CorrectRate { get; set; }
        public List<int> WrongNum { get; set; }
    }
    public class WrongKnowledgeInfo
    {
        public Guid KnowledgeID { get; set; }
        public string KnowledgeName { get; set; }
        public int TotalNum { get; set; }
        public double CorrectRate { get; set; }
    }
    public class QuestionDoneInfo
    {
        public int QuestionNum { get; set; }
        public int WrongNum { get; set; }
    }
}