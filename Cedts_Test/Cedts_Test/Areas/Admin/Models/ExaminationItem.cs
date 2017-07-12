using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class ExaminationItem
    {
        public Guid AssessmentItemID { get; set; }

        public string ItemName { get; set; }

        public double Difficult { get; set; }

        public double Duration { get; set; }

        public double Score { get; set; }

        public DateTime SaveTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public int Count { get; set; }

        public string PaperName { get; set; }

        public string Username { get; set; }
        
        public string UpdateName { get; set; }

        public int ItemID { get; set; }
       
    }

    public class ExcelInfo
    {
        public string UserName { get; set; }

        public string UpdateUserName { get; set; }

        public string ItemName { get; set; }

        public string ItemID { get; set; }

        public DateTime Time { get;set; }

        public List<string> QuestionID { get; set; }

        public List<string> KnowledgeID { get; set; }

        public List<string> OldKPName { get; set; }

        public List<string> FirstName { get; set; }

        public List<string> KPName { get; set; }
    }
}