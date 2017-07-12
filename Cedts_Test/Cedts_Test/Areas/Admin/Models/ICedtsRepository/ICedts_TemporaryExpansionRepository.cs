using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_TemporaryExpansionRepository
    {
        /// <summary>
        /// 向Expansion表添加选词选项
        /// </summary>
        /// <param name="Expansion">选词填空选项</param>
        void CreateExpansion(CEDTS_TemporaryExpansion Expansion);
    }
}