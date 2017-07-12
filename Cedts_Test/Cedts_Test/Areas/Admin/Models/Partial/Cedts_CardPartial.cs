using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_CardMetadata))]
    public partial class CEDTS_Card
    {
        private class CEDTS_CardMetadata
        {
            [DisplayName("卡ID")]
            public int ID { get; set; }

            [DisplayName("序列号")]
            public string SerialNumber { get; set; }

            [DisplayName("密码")]
            public string PassWord { get; set; }

            [DisplayName("卡类别")]//0--实体卡，1--虚拟卡
            public int CardKind { get; set; }

            [DisplayName("卡类型")]//0--年卡，1--月卡，2--次数卡
            public int CardType { get; set; }

            [DisplayName("创建时间")]
            public DateTime CreateTime { get; set; }

            [DisplayName("激活时间")]
            public DateTime ActivationTime { get; set; }

            [DisplayName("激活状态")]//0--待激活，1--已激活，2--已过期
            public int ActivationState { get; set; }

            [DisplayName("有效时间")]
            public string EffectiveTime { get; set; }//一年，一月，30次

            [DisplayName("到期时间")]
            public DateTime OverdueTime { get; set; }

            [DisplayName("创建用户")]
            public int CreateUser { get; set; }

            [DisplayName("激活用户")]
            public int ActivationUser { get; set; }

            [DisplayName("折扣")]
            public double Discount { get; set; }

            [DisplayName("院校ID")]
            public Guid PartnerID { get; set; }

            [DisplayName("原价金额")]
            public double Money { get; set; }
        }
    }
}