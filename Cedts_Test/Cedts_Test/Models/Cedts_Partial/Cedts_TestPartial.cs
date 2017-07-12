using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models
{
    [MetadataType(typeof(CEDTS_TestMetadata))]
    public partial class CEDTS_Test
    {
        private class CEDTS_TestMetadata
        {
            [DisplayName("答卷ID")]
            public Guid TestID { get; set; }

            [DisplayName("试卷ID")]
            public Guid PaperID{get;set;}

            [DisplayName("用户ID")]
            public int UserID{get;set;}

            [DisplayName("开始答卷时间ID")]
            public DateTime StartTime{get;set;}

            [DisplayName("结束答卷ID")]
            public DateTime FinishedTime{get;set;}

            [DisplayName("是否完成答卷")]
            public bool IsFinished{get;set;}

            [DisplayName("答卷总分")]
            public float TotalScore{get;set;}

            [DisplayName("答卷总时间")]
            public float TotalTime { get; set; }
        }
    }
}