using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_QuestionMetadata))]
    public partial class CEDTS_Question
    {
        private class CEDTS_QuestionMetadata
        {
            //小问题ID
            public Guid QuestionID { get; set; }

            //所属题目ID
            public Guid AssessmentItemID { get; set; }

            //小题分数
            public double Score { get; set; }

            //小题问题内容
            public string QuestionContent { get; set; }

            //选项A
            public string ChooseA { get; set; }

            //选项B
            public string ChooseB { get; set; }

            //选项C
            public string ChooseC { get; set; }

            //选项D
            public string ChooseD { get; set; }

            //正确答案
            public string Answer { get; set; }

            //答案解析
            public string Analyze { get; set; }

            //排序
            public int Order { get; set; }

            //估时
            public int Duration { get; set; }

            //难度
            public double Difficult { get; set; }

            [DisplayName("音频文件")]
            public string Sound { get; set; }

            [DisplayName("时间间隔")]
            public int Interval { get; set; }
        }
    }
}