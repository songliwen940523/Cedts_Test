using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_TestAnswerMetadata))]
    public partial class CEDTS_TestAnswer
    {
        private class CEDTS_TestAnswerMetadata
        {
            [DisplayName("用户答案表主键")]
            public int TestAnswerID { get; set; }

            [DisplayName("小问题ID")]
            public Guid QuestionID { get; set; }

            [DisplayName("用户ID")]
            public int UserID { get; set; }

            [DisplayName("测试ID")]
            public Guid TestID { get; set; }

            [DisplayName("标准答案")]
            public string Answer { get; set; }

            [DisplayName("用户提交的答案")]
            public string UserAnswer { get; set; }

            [DisplayName("标准答案内容")]
            public string AnswerContent { get; set; }

            [DisplayName("用户是否正确")]
            public bool IsRight { get; set; }

            [DisplayName("试题ID")]
            public Guid AssessmentItemID { get; set; }

            [DisplayName("试题类型")]
            public int ItemTypeID { get; set; }

            [DisplayName("完成试题话费的时间")]
            public double ItemAnswerTime { get; set; }
        }
    }

    public class Cedts_TestAnswer
    {
        public int TestAnswerID { get; set; }

        public Guid QuestionID { get; set; }

        public Guid TestID { get; set; }

        public string Answer { get; set; }

        public string UserAnswer { get; set; }

        public string AnswerContent { get; set; }

        public bool IsRight { get; set; }

        public Guid AssessmentItemID { get; set; }
    }
}