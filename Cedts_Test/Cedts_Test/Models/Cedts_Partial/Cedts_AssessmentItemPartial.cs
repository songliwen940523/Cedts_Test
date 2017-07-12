using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models
{
    [MetadataType(typeof(CEDTS_AssessmentItemMetadata))]
    public partial class CEDTS_AssessmentItem
    {
        private class CEDTS_AssessmentItemMetadata
        {
            [DisplayName("试题ID")]
            public Guid AssessmentItemID { get; set; }

            [DisplayName("试题类型ID")]
            public int ItemTypeID { get; set; }

            [DisplayName("试题描述")]
            public string Description { get; set; }

            [DisplayName("试题创建人ID")]
            public int UserID { get; set; }

            [DisplayName("试题更新人ID")]
            public int UpdateUserID { get; set; }

            [DisplayName("试题创建时间")]
            public DateTime SaveTime { get; set; }

            [DisplayName("试题估时")]
            public int Duration { get; set; }

            [DisplayName("试题难度")]
            public double Difficult { get; set; }

            [DisplayName("试题分数")]
            public double Score { get; set; }

            [DisplayName("试题被练习次数")]
            public int Count { get; set; }

            [DisplayName("试题信息内容")]
            public string Content { get; set; }

            [DisplayName("试题包含的小题数目")]
            public int QuestionCount { get; set; }

            [DisplayName("试题更新时间")]
            public DateTime UpdateTime { get; set; }

            [DisplayName("试题原文")]
            public string Original { get; set; }

            [DisplayName("所属书籍")]
            public string Course { get; set; }

            [DisplayName("所属单元")]
            public string Unit { get; set; }

            [DisplayName("时间间隔")]
            public int Interval { get; set; }
        }
    }
}