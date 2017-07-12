using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models.Cedts_Partial
{
    [MetadataType(typeof(CEDTS_TestAnswerTypeInfoMetadata))]
    public partial class Cedts_TestAnswerTypeInfoPartial
    {
        private class CEDTS_TestAnswerTypeInfoMetadata
        {
            [DisplayName("题型统计表ID")]
            public int TATI_ID { get; set; }

            [DisplayName("答卷ID")]
            public Guid TestID { get; set; }

            [DisplayName("用户ID")]
            public int UserID { get; set; }

            [DisplayName("题型ID")]
            public int ItemTypeID { get; set; }

            [DisplayName("正确试题题数")]
            public int CorrectItemNumber { get; set; }

            [DisplayName("总计试题题数")]
            public int TotalItemNumber { get; set; }

            [DisplayName("正确率")]
            public float CorrectRate { get; set; }
        }
    }
}