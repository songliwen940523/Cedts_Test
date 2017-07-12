using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Models
{
    public class QuestionStatistics
    {
        /// <summary>
        /// 小题ID
        /// </summary>
        public Guid QuestionID { get; set; }
        /// <summary>
        /// 小题对错
        /// </summary>
        public bool IsRight { get; set; }
        /// <summary>
        /// 小题用时
        /// </summary>
        public double AverageTime { get; set; }
        /// <summary>
        /// 知识点ID
        /// </summary>
        public string KnowledgePointID { get; set; }
        /// <summary>
        /// 知识点名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 小题分值
        /// </summary>
        public double Score { get; set; }
    }
}