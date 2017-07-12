using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Major
    {
        /// <summary>
        /// 专业ID
        /// </summary>
        public Guid MajorID { get; set; }

        /// <summary>
        /// 专业名称
        /// </summary>
        public string MajorName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string MajorMark { get; set; }

        /// <summary>
        /// 上级专业名称
        /// </summary>
        public string UpMajorName { get; set; }
    }
}