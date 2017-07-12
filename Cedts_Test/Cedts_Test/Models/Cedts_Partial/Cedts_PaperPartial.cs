using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models
{
    [MetadataType(typeof(CEDTS_PaperMetadata))]
    public partial class CEDTS_Paper
    {
        private class CEDTS_PaperMetadata
        {
            [DisplayName("试卷编号")]
            public Guid PaperID { get; set; }

            [DisplayName("试卷标题")]
            public string Title { get; set; }

            [DisplayName("试卷类型")]
            public string Type { get; set; }

            [DisplayName("试卷估时")]
            public int Duration { get; set; }

            [DisplayName("试卷难度")]
            public double Difficult { get; set; }

            [DisplayName("试卷总分")]
            public double Score { get; set; }

            [DisplayName("试卷说明")]
            public string Description { get; set; }

            [DisplayName("创建时间")]
            public DateTime CreateTime { get; set; }

            [DisplayName("组卷人")]
            public int UserID { get; set; }

            [DisplayName("最近更新时间")]
            public DateTime UpdateTime { get; set; }

            [DisplayName("最近更新人")]
            public int UpdateUserID { get; set; }

            [DisplayName("试卷内容")]
            public string PaperContent { get; set; }
        }
    }
    /// <summary>
    ///试卷总信息
    /// </summary>
    public class PaperTotal
    {
        //大题的答题时间集合
        public List<int> timeList { get; set; }

        //试卷ID
        public Guid PaperID { get; set; }

        //试卷标题
        public string Title { get; set; }

        //试卷类型
        public string Type { get; set; }

        //试卷估时
        public double Duration { get; set; }

        //试卷难度
        public double Difficult { get; set; }

        //试卷总分
        public double Score { get; set; }

        //试卷说明
        public string Description { get; set; }

        //创建时间
        public DateTime CreateTime { get; set; }

        //组卷人
        public int UserID { get; set; }

        //最近更新时间
        public DateTime UpdateTime { get; set; }

        //最近更新人
        public int UpdateUserID { get; set; }

        //试卷内容
        public string PaperContent { get; set; }

        //快速阅读题型集合
        public List<SkimmingScanningPartCompletion> SspcList { get; set; }

        //短对话听力题型集合
        public List<Listen> SlpoList { get; set; }

        //长对话听力题型集合
        public List<Listen> LlpoList { get; set; }

        //听力理解题型集合
        public List<Listen> RlpoList { get; set; }

        //复合型听力题型集合
        public List<Listen> LpcList { get; set; }

        //阅读理解选词填空题型集合
        public List<ReadingPartCompletion> RpcList { get; set; }

        //阅读理解选择题型集合
        public List<ReadingPartOption> RpoList { get; set; }

        /// <summary>
        /// 信息匹配题型集合
        /// </summary>
        public List<InfoMatchingCompletion> InfMatList { get; set; }

        //完型填空题型集合
        public List<ClozePart> CpList { get; set; }

    }

    public class PaperTotalContinue
    {
        //大题的答题时间集合
        public List<int> timeList { get; set; }

        //用户做过的答案，没做的为null.
        public List<string> userData { get; set; }

        public string TestID { get; set; }

        public PaperTotal paperTotal { get; set; }
    }
}