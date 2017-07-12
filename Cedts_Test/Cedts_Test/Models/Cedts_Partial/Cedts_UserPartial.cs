using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models
{
    [MetadataType(typeof(CEDTS_UserMetadata))]
    public partial class CEDTS_User
    {
        partial void OnUserAccountChanged()
        {
            this._UserAccount = this._UserAccount.ToLower();
        }

        private class CEDTS_UserMetadata
        {
            [DisplayName("用户编号")]
            public int UserID { get; set; }


            [DisplayName("用户帐号")]
            [Required(ErrorMessage = "请输入帐号")]
            [StringLength(20, ErrorMessage = "请不要超过20个字")]
            public string UserAccount { get; set; }


            [DisplayName("用户姓名")]
            [Required(ErrorMessage = "请输入姓名")]
            [StringLength(20, ErrorMessage = "请不要超过20个字")]
            public string UserNickname { get; set; }


            [DisplayName("用户密码")]
            [DataType(DataType.Password)]
            [Required(ErrorMessage = "请输入密码")]
            [StringLength(20, ErrorMessage = "请不要超过20个字")]
            public string UserPassword { get; set; }


            [DisplayName("用户邮箱")]
            [Required(ErrorMessage = "请输入邮箱")]
            [StringLength(255, ErrorMessage = "请不要超过255个字")]
            [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4})$", ErrorMessage = "请输入正确的邮箱格式")]
            public string Email { get; set; }


            [DisplayName("用户性别")]
            [Required(ErrorMessage = "请选择性别")]
            public string Sex { get; set; }


            [DisplayName("用户角色")]
            public string Role { get; set; }


            [DisplayName("注册时间")]
            public DateTime RegisterTime { get; set; }

            [RegularExpression(@"^0{1}[1-9]{1}[0-9]{1}\-[1-9]{1}[0-9]{7}|0{1}[1-9]{1}[0-9]{2}\-[1-9]{1}[0-9]{6}|^\(0{1}[1-9]{1}[0-9]{1}\)[1-9]{1}[0-9]{7}|^\(0{1}[1-9]{1}[0-9]{2}\)[1-9]{1}[0-9]{6}|^0{1}[1-9]{1}[0-9]{1}[1-9]{1}[0-9]{7}|^0{1}[1-9]{1}[0-9]{2}[1-9]{1}[0-9]{6}|^13[0-9]{1}[0-9]{8}|^15[^4]{1}[0-9]{8}|^18[0256789]{1}[0-9]{8}|^14[7]{1}[0-9]{8}", ErrorMessage = "请输入正确的电话格式")]
            [DisplayName("用户手机")]
            public string Phone { get; set; }


            [DisplayName("学号/编号")]
            public string StudentNumber { get; set; }


            [DisplayName("学校/机构")]
            public Guid PartnerID { get; set; }


            [DisplayName("专业")]
            public Guid MajorID { get; set; }


            [DisplayName("年级")]
            public Guid GradeID { get; set; }


            [DisplayName("班级")]
            public Guid ClassID { get; set; }


            [DisplayName("审核状态")]
            public bool State { get; set; }
        }
    }
}