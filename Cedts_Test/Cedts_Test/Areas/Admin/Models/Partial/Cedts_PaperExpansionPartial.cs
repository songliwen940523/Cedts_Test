using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_PaperExpansionMetadata))]
    public partial class Cedts_PaperExpansionPartial
    {
        private class CEDTS_PaperExpansionMetadata
        {
            [DisplayName("试卷暂存表ID")]
            public int PaperExpansionID { get; set; }

            [DisplayName("试卷编号")]
            public Guid PaperID { get; set; }

            [DisplayName("快速阅读题数")]
            public int SkimmingAndScanningNum { get; set; }

            [DisplayName("短对话题数")]
            public int ShortListenNum { get; set; }

            [DisplayName("长对话题数")]
            public int LongListenNum { get; set; }

            [DisplayName("听力短文理解题数")]
            public int ComprehensionListenNum { get; set; }

            [DisplayName("复合型听力题数")]
            public int ComplexListenNum { get; set; }

            [DisplayName("阅读理解-选词填空题数")]
            public int BankedClozeNum { get; set; }

            [DisplayName("阅读理解-选择题型题数")]
            public int MultipleChoiceNum { get; set; }

            [DisplayName("完型填空")]
            public int ClozeNum { get; set; }
        }
    }
}