using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_PartTypeMetadata))]
    public partial class CEDTS_PartType
    {
        private class CEDTS_PartTypeMetadata
        { 
            [DisplayName("类型ID")]
            public int PartTypeID{get;set;}

            [DisplayName("类型名称")]
            public string TypeName{get;set;}

            [DisplayName("中文名称")]
            public string TypeName_CN { get; set; }

            [DisplayName("类型描述")]
            public string Describe { get; set; }
        }
    }
}