using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Grade
    {
        /// <summary>
        /// 年级ID
        /// </summary>
        public Guid GradeID { get; set; }

        /// <summary>
        /// 年级名称
        /// </summary>
        public string GradeName { get; set; }

        /// <summary>
        /// 所属专业
        /// </summary>
        public string MajorName { get; set; }
    }
}