using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models
{
    [MetadataType(typeof(CEDTS_TestAnswerKnowledgePointMetadata))]
    public partial class Cedts_TestAnswerKnowledgePointPartial
    {
        private class CEDTS_TestAnswerKnowledgePointMetadata
        {
            [DisplayName("知识点统计表ID")]
            public int TKPID { get; set; }

            [DisplayName("答卷ID")]
            public Guid TestID { get; set; }

            [DisplayName("用户ID")]
            public int UserID { get; set; }

            [DisplayName("知识点ID")]
            public Guid KnowledgePointID { get; set; }

            [DisplayName("正确试题数目")]
            public int CorrectItemNumber { get; set; }

            [DisplayName("总计试题数目")]
            public int TotalItemNumber { get; set; }

            [DisplayName("正确率")]
            public float CorrectRate { get; set; }

            [DisplayName("平均时间")]
            public DateTime AverageTime { get; set; }

            [DisplayName("知识点掌握率")]
            public float KP_MasterRate { get; set; }
        }
    }
}