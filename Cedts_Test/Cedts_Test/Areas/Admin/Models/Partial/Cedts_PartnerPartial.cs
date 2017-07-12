using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_PartnerMetadata))]
    public partial class CEDTS_Partner
    {
        private class CEDTS_PartnerMetadata
        {
            [DisplayName("院校ID")]
            public Guid PartnerID { get; set; }

            [DisplayName("院校名称")]
            public string PartnerName { get; set; }

            [DisplayName("单位省份")]
            public string Province { get; set; }

            [DisplayName("单位城市")]
            public string City { get; set; }

            [DisplayName("详细地址")]
            public string Address { get; set; }

            [DisplayName("联系人")]
            public string Principal { get; set; }

            [DisplayName("联系电话")]
            public string Telphone { get; set; }

            [DisplayName("手机")]
            public string Mobilephone { get; set; }

            [DisplayName("邮箱地址")]
            public string Email { get; set; }

            [DisplayName("官网地址")]
            public string Src { get; set; }

            [DisplayName("管理帐号")]
            public string AdminAccount { get; set; }
        }
    }
}