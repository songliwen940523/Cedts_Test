using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_ItemMetadata))]
    public partial class CEDTS_AssessmentItem
    {
        private class CEDTS_ItemMetadata
        {
            [DisplayName("题目编号")]
            public Guid AssessmentItemID { get; set; }

            [DisplayName("题目类型编号")]
            public int ItemTypeID { get; set; }

            [DisplayName("题目描述")]
            public string Description { get; set; }

            [DisplayName("题目上传时间")]
            public DateTime SaveTime { get; set; }

            [DisplayName("题目估时")]
            public int Duration { get; set; }

            [DisplayName("题目难度")]
            public double Difficult { get; set; }

            [DisplayName("题目分数")]
            public double Score { get; set; }

            [DisplayName("题目被练习次数")]
            public int Count { get; set; }

            [DisplayName("更新时间")]
            public DateTime UpdataTime { get; set; }

            [DisplayName("题目原文")]
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