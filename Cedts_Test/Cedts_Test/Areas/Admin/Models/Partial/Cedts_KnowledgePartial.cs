using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_KnowledgeMetadata))]
    public partial class CEDTS_KnowledgePoints
    {
        private class CEDTS_KnowledgeMetadata
        {
            [DisplayName("知识点ID")]
            public Guid KnowledgePointID { get; set; }

            [DisplayName("知识点标题")]
            public string Title { get; set; }

            [DisplayName("知识点说明")]
            public string Describe { get; set; }

            [DisplayName("上级知识点ID")]
            public Guid UperKnowledgeID { get; set; }

            [DisplayName("知识点等级")]
            public int Level { get; set; }
        }
    }
}