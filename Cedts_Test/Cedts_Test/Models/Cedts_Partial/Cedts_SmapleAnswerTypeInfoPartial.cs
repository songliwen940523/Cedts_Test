using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models.Cedts_Partial
{
    [MetadataType(typeof(CEDTS_SmapleAnswerTypeInfoMetadata))]
    public partial class Cedts_SmapleAnswerTypeInfoPartial
    {
        private class CEDTS_SmapleAnswerTypeInfoMetadata
        {
            [DisplayName("样本试题统计表ID")]
            public int SATI_ID { get; set; }

            [DisplayName("样本答卷ID")]
            public Guid TestID { get; set; }

            [DisplayName("样本试题ID")]
            public int ItemTypeID { get; set; }

            [DisplayName("样本试题正确率")]
            public int CorrectRate { get; set; }

            [DisplayName("样本试题总计人数")]
            public int TotalCount { get; set; }
        }
    }
}