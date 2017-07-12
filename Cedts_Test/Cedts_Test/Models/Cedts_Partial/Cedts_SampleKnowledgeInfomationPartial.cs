using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models
{
    [MetadataType(typeof(CEDTS_SampleKnowledgeInfomationMetadata))]
    public partial class Cedts_SampleKnowledgeInfomationPartial
    {
        private class CEDTS_SampleKnowledgeInfomationMetadata
        {
            [DisplayName("样本知识点统计表ID")]
            public int SKII_ID { get; set; }

            [DisplayName("答卷ID")]
            public Guid TestID { get; set; }

            [DisplayName("知识点ID")]
            public Guid KnowledgePointID { get; set; }

            [DisplayName("知识点正确率")]
            public float CorrectRate { get; set; }

            [DisplayName("平均时间")]
            public DateTime AverageTime { get; set; }

            [DisplayName("知识点掌握率")]
            public float KP_MasterRate { get; set; }

            [DisplayName("参加统计人数")]
            public int TotalCount { get; set; }
        }
    }
}