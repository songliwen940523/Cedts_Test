using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_TestMetadata))]
    public partial class CEDTS_Test
    {
        private class CEDTS_TestMetadata
        {
            [DisplayName("测试试卷ID")]
            public Guid TestID { get; set; }

            [DisplayName("是否完成")]
            public bool IsFinished { get; set; }

            [DisplayName("完成时间")]
            public DateTime FinishDate { get; set; }

            [DisplayName("做卷人ID")]
            public int UserID { get; set; }

            [DisplayName("试卷ID")]
            public Guid PaperID { get; set; }

            [DisplayName("开始时间")]
            public DateTime StartDate { get; set; }

            [DisplayName("试卷得分")]
            public double TotalScore { get; set; }

            [DisplayName("做卷花时")]
            public double TotalTime { get; set; }

            [DisplayName("是否评审")]
            public bool IsChecked { get; set; }

            [DisplayName("教师评语")]
            public string Remark { get; set; }
        }
    }

    public class Cedts_UserTest
    {
        public Guid TestID { get; set; }

        public string UserName { get; set; }

        public Guid PaperID { get; set; }

        public string PaperTitle { get; set; }

        public double TotalScore { get; set; }

        public string Remark { get; set; }

        public int IsChecked { get; set; }
    }


}