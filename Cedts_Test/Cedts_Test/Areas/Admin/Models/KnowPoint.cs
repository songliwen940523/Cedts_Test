using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class KnowPoint
    {
        public Guid KnowID { get; set; }
        public string SortName { get; set; }
        public string SideName { get; set; }
        public string PointName { get; set; }
        public string Describle { get; set; }
    }
}