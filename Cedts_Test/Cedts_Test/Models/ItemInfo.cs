using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Models
{
    /// <summary>
    /// 试题基本信息
    /// </summary>
    public class ItemBassInfo
    {
        //试题ID-数据库
        public Guid ItemID { get; set; }

        //题目大类型-数据库
        public string PartType { get; set; }

        //题目小类型-数据库
        public string ItemType { get; set; }

        //题目小类型中文名称-数据库
        public string ItemType_CN { get; set; }

        //题目分数-数据库
        public double Score { get; set; }

        //题目估时-数据库
        public double ReplyTime { get; set; }

        //题目难度-数据库
        public double Diffcult { get; set; }

        //问题间隔
        public string QustionInterval { get; set; }

        //Qustion个数
        public int QuestionCount { get; set; }

        //Qustion答案
        public List<string> AnswerValue { get; set; }

        //Qustion位置符号
        public string AnswerResposn { get; set; }

        //使用次数
        public int Count { get; set; }

        //答案解析
        public List<string> Tip { get; set; }

        //问题内容
        public List<string> Problem { get; set; }

        //问题ID集合
        public List<Guid> QuestionID { get; set; }

        //知识点
        public List<string> Knowledge { get; set; }

        //小问题难度
        public List<double> DifficultQuestion { get; set; }

        //小问题分数
        public List<double> ScoreQuestion { get; set; }

        //小问题时间
        public List<double> TimeQuestion { get; set; }

        //知识点与问题关系表ID
        public string QuestionKnowledgeID { get; set; }

        //知识点ID
        public List<string> KnowledgeID { get; set; }

        //所属书籍
        public string Course { get; set; }

        //所属单元
        public string Unit { get; set; }

        //问题声音文件
        public List<string> questionSound { get; set; }

        //听力文件时间间隔
        public List<int> timeInterval { get; set; }

        //用户提交的答案
        public List<string> UserAnswer { get; set; }

        public ItemBassInfo() { }
        public ItemBassInfo
            (Guid _ItemID, string _PartType,
            string _ItemType, string _ItemType_CN,
            double _Score, int _ReplyTime,
            double _Diffcult, string _QustionInterval,
            int _QuestionCount, List<string> _AnswerValue,
            string _AnswerResposn, List<string> _Tip,
            List<Guid> _QuestionID,
            List<string> _Knowledge,
            List<double> _DifficultQuestion,
            List<double> _ScoreQuestion,
            string _QuestionKnowledgeID,
            List<string> _KnowledgeID,
            List<double> _TimeQuestion,
            List<string> _questionSound,
            string _Course,
            string _Unit,
            int _Count,
            List<int> _timeInterval
            )
        {
            this.ItemID = _ItemID;
            this.PartType = _PartType;
            this.ItemType = _ItemType;
            this.ItemType_CN = _ItemType_CN;
            this.Score = _Score;
            this.ReplyTime = _ReplyTime;
            this.Diffcult = _Diffcult;
            this.QustionInterval = _QustionInterval;
            this.QuestionCount = _QuestionCount;
            this.AnswerValue = _AnswerValue;
            this.AnswerResposn = _AnswerResposn;
            this.Tip = _Tip;
            this.QuestionID = _QuestionID;
            this.Knowledge = _Knowledge;
            this.ScoreQuestion = _ScoreQuestion;
            this.DifficultQuestion = _DifficultQuestion;
            this.QuestionKnowledgeID = _QuestionKnowledgeID;
            this.KnowledgeID = _KnowledgeID;
            this.TimeQuestion = _TimeQuestion;
            this.questionSound = _questionSound;
            this.Course = _Course;
            this.Unit = _Unit;
            this.Count = _Count;
            this.timeInterval = _timeInterval;
        }
    }

    /// <summary>
    /// 选择听力类型信息
    /// </summary>

    /// <summary>
    /// 选择听力类型信息
    /// </summary>
    public class Listen
    {
        //题目基本信息
        public ItemBassInfo Info { get; set; }

        //音频文件路径
        public string SoundFile { get; set; }

        //题目选项内容
        public List<string> Choices { get; set; }

        //听力原文
        public string Script { get; set; }
    }


    /// <summary>
    /// 阅读理解填词类型信息
    /// </summary>
    public class ReadingPartCompletion
    {
        //题目基本信息
        public ItemBassInfo Info { get; set; }

        //阅读理解原文
        public string Content { get; set; }

        //阅读理解选词列表
        public List<string> WordList { get; set; }

        //选词ExpansionID
        public int ExpansionID { get; set; }
    }

    /// <summary>
    /// 阅读理解选择类型信息
    /// </summary>
    public class ReadingPartOption
    {
        //题目基本信息
        public ItemBassInfo Info { get; set; }

        //阅读理解原文
        public string Content { get; set; }

        //题目选项内容
        public List<string> Choices { get; set; }
    }

    /// <summary>
    /// 信息匹配类型信息
    /// </summary>
    public class InfoMatchingCompletion
    {
        //题目基本信息
        public ItemBassInfo Info { get; set; }

        //阅读理解原文
        public string Content { get; set; }

        //阅读理解选词列表
        public List<string> WordList { get; set; }

        //选词ExpansionID
        public int ExpansionID { get; set; }
    }

    /// <summary>
    /// 完形填空类型信息
    /// </summary>
    public class ClozePart
    {
        //题目基本信息
        public ItemBassInfo Info { get; set; }

        //完型填空原文
        public string Content { get; set; }

        //题目选项内容
        public List<string> Choices { get; set; }
    }

    /// <summary>
    /// 快速阅读-类型信息
    /// </summary>
    public class SkimmingScanningPartCompletion
    {
        //题目基本信息
        public ItemBassInfo Info { get; set; }

        //快速阅读原文
        public string Content { get; set; }

        //题目选项内容
        public List<string> Choices { get; set; }

        //快速阅读选择题数
        public int ChoiceNum { get; set; }

        //快速阅读填空题数
        public int TermNum { get; set; }
    }

}