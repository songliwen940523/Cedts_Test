using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_ItemTypeMetadata))]
    public partial class CEDTS_ItemType
    {
        private class CEDTS_ItemTypeMetadata
        {
            [DisplayName("类型ID")]
            public int ItemTypeID { get; set; }

            [DisplayName("类型名称")]
            public string TypeName{get;set;}

            [DisplayName("中文名称")]
            public string TypeName_CN{get;set;}

            [DisplayName("类型描述")]
            public string Describle{get;set;}

            [DisplayName("所属类型ID")]
            public int PartTypeID { get; set; }

        }
    }
}