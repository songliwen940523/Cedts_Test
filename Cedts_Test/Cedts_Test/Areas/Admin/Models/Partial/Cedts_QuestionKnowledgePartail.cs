using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Cedts_Test.Areas.Admin.Models
{

    [MetadataType(typeof(Cedts_QuestionKnowledgeMetadata))]
    public partial class CEDTS_QuestionKnowledge
    {

        private class Cedts_QuestionKnowledgeMetadata
        {
            //知识点和小问题关系表ID
            public int Cedts_QuestionKnowledgeID { get; set; }

            //小问题ID
            public Guid QuestionID { get; set; }

            //知识点ID
            public Guid KnowledgePointID { get; set; }

            //权重
            public int Weight { get; set; }
        }
    }
}