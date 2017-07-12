using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Cedts_Test.Models
{
    [MetadataType(typeof(CEDTS_LogMetadata))]
    public partial class CEDTS_Log
    {
        private class CEDTS_LogMetadata
        {
            [DisplayName("日志ID")]
            public int LogID { get; set; }

            [DisplayName("操作的用户ID")]
            public int UserID { get; set; }

            [DisplayName("记录的描述")]
            public string Content { get; set; }

            [DisplayName("操作记录的时间")]
            public DateTime LogTime { get; set; }

            [DisplayName("客户端连接的IP")]
            public string ClientIP { get; set; }

            [DisplayName("记录操作的Controller和Action")]
            public int Action { get; set; }
        }
    }
}