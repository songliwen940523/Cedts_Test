using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class TeacherInfo
    {
        /// <summary>
        /// 教师ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 教师帐号
        /// </summary>
        public string UserAccount { get; set; }

        /// <summary>
        /// 教师邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 教师性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 教师角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        public string UserNickname { get; set; }

        /// <summary>
        /// 教师电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 所属单位
        /// </summary>
        public string Partner { get; set; }

        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        public string Class { get; set; }

    }
}