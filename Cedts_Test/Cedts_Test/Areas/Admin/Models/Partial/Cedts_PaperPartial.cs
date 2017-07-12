using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Areas.Admin.Models
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
        /// <summary>
        /// 试卷ID
        /// </summary>
        public Guid PaperID { get; set; }

        /// <summary>
        /// 试卷标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 试卷类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 试卷估时
        /// </summary>
        public double Duration { get; set; }

        //试卷难度
        public double Difficult { get; set; }

        /// <summary>
        /// 试卷总分
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 试卷说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 组卷人
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 最近更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 最近更新人
        /// </summary>
        public int UpdateUserID { get; set; }

        /// <summary>
        /// 试卷内容
        /// </summary>
        public string PaperContent { get; set; }

        /// <summary>
        /// 快速阅读题型集合
        /// </summary>
        public List<SkimmingScanningPartCompletion> SspcList { get; set; }

        /// <summary>
        /// 短对话听力题型集合
        /// </summary>
        public List<Listen> SlpoList { get; set; }

        /// <summary>
        /// 长对话听力题型集合
        /// </summary>
        public List<Listen> LlpoList { get; set; }

        /// <summary>
        /// 听力理解题型集合
        /// </summary>
        public List<Listen> RlpoList { get; set; }

        /// <summary>
        /// 复合型听力题型集合
        /// </summary>
        public List<Listen> LpcList { get; set; }

        /// <summary>
        /// 阅读理解选词填空题型集合
        /// </summary>
        public List<ReadingPartCompletion> RpcList { get; set; }

        /// <summary>
        /// 阅读理解选择题型集合
        /// </summary>
        public List<ReadingPartOption> RpoList { get; set; }

        /// <summary>
        /// 信息匹配题型集合
        /// </summary>
        public List<InfoMatchingCompletion> InfMatList { get; set; }

        /// <summary>
        /// 完型填空题型集合
        /// </summary>
        public List<ClozePart> CpList { get; set; }
    }

    public class AssessmentType
    {
        public int ItemType { get; set; }
        public SkimmingScanningPartCompletion Sspc { get; set; }
        public Listen Slpo { get; set; }
        public Listen Llpo { get; set; }
        public Listen Rlpo { get; set; }
        public Listen Lpc { get; set; }
        public ReadingPartCompletion Rpc { get; set; }
        public ReadingPartOption Rpo { set; get; }
        public InfoMatchingCompletion InfMat { get; set; }
        public ClozePart Cp { get; set; }
    }

    public class TestInfo
    {
        public PaperTotal pt { get; set; }

        public List<Cedts_TestAnswer> taList { get; set; }
    }
}