using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class PaperInfo
    {
       
        public Guid PaperID { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public int Duration { get; set; }

        public double Difficult { get; set; }

        public double Score { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public int UserID { get; set; }

        public DateTime UpdateTime { get; set; }

        public int UpdateUserID { get; set; }

        public string PaperContent { get; set; }
    }
}