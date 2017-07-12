using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
{
    [MetadataType(typeof(CEDTS_ExpansionMetadata))]
    public partial class CEDTS_Expansion
    {
        private class CEDTS_ExpansionMetadata
        {
            [DisplayName("ExpansionID")]
            public int ExpansionID { get; set; }

            [DisplayName("AssessmentItemID")]
            public Guid AssessmentItemID { get; set; }

            [DisplayName("ChoiceA")]
            public string ChoiceA { get; set; }

            [DisplayName("ChoiceB")]
            public string ChoiceB { get; set; }

            [DisplayName("ChoiceC")]
            public string ChoiceC { get; set; }

            [DisplayName("ChoiceD")]
            public string ChoiceD { get; set; }

            [DisplayName("ChoiceE")]
            public string  ChoiceE { get; set; }

            [DisplayName("ChoiceF")]
            public string ChoiceF { get; set; }

            [DisplayName("ChoiceG")]
            public string ChoiceG { get; set; }

            [DisplayName("ChoiceH")]
            public string ChoiceH { get; set; }

            [DisplayName("ChoiceI")]
            public string ChoiceI { get; set; }

            [DisplayName("ChoiceJ")]
            public string ChoiceJ { get; set; }

            [DisplayName("ChoiceK")]
            public string ChoiceK { get; set; }

            [DisplayName("ChoiceL")]
            public string ChoiceL { get; set; }

            [DisplayName("ChoiceM")]
            public string ChoiceM { get; set; }

            [DisplayName("ChoiceN")]
            public string ChoiceN { get; set; }

            [DisplayName("ChoiceO")]
            public string ChoiceO { get; set; }
        }
    }
}