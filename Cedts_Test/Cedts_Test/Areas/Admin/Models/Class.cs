using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Class
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        public Guid ClassID { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 所属年级
        /// </summary>
        public string GradeName { get; set; }

        /// <summary>
        /// 班级人数
        /// </summary>
        public int StudentNum { get; set; }
    }
}