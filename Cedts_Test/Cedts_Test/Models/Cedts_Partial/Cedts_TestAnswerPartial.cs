using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models
{
    [MetadataType(typeof(Cedts_TestAnswerMetadata))]
    public partial class Cedts_TestAnswerPartial
    {
        private class Cedts_TestAnswerMetadata
        {
            [DisplayName("TestQuestionID")]
            public int TestQuestionID { get; set; }

            [DisplayName("答卷ID")]
            public Guid TestID{get;set;}

            [DisplayName("问题ID")]
            public Guid QuestionID{get;set;}

            [DisplayName("用户ID")]
            public int UserID{get;set;}

            [DisplayName("试题ID")]
            public Guid AssessmentItemID{get;set;}

            [DisplayName("试题类型ID")]
            public int ItemTypeID{get;set;}

            [DisplayName("正确答案")]
            public string Answer{get;set;}

            [DisplayName("用户答案")]
            public string UserAnswer{get;set;}

            [DisplayName("是否正确")]
            public bool IsTrue{get;set;}

            [DisplayName("试题回答时间")]
            public DateTime ItemAnswerTime { get; set; }
        }
    }
}