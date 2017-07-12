using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Models
{
    public class SingleScoreInfo
    {
        public int UserID { get; set; }
        public string StudNum { get; set; }
        public string StudName { get; set; }
        public double Score { get; set; }
        public double TotalScore { get; set; }
        public double Percent { get; set; }
        public bool Done { get; set; }
    }
    public class SingleStatistics
    {
        public double AvePercent { get; set; }
        public double AveScore { get; set; }
        public double Variance { get; set; }
        public double StandardDeviation { get; set; }
        public double HighestScore { get; set; }
        public string HighestStudName { get; set; }
        public double LowestScore { get; set; }
        public string LowestStudName { get; set; }
        public int DoNum { get; set; }
        public int UndoNum { get; set; }
    }
}