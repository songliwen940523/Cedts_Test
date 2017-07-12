using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class SelectAssessmentItem
    {
        public List<ExaminationItem> sspcList { get; set; }
        public List<ExaminationItem> slpoList { get; set; }
        public List<ExaminationItem> llpoList { get; set; }
        public List<ExaminationItem> rlpoList { get; set; }
        public List<ExaminationItem> lpcList { get; set; }
        public List<ExaminationItem> rpcList { get; set; }
        public List<ExaminationItem> rpoList { get; set; }
        public List<ExaminationItem> infoMatList { get; set; }
        public List<ExaminationItem> cpList { get; set; }
    }
}