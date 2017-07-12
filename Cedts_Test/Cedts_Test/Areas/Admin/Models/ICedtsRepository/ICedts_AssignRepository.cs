using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_AssignRepository
    {

        /// <summary>
        /// 根据查询条件获取试题列表
        /// </summary>
        /// <param name="id">分页ID</param>
        /// <param name="condition">条件分类名</param>
        /// <param name="txt">条件内容</param>
        /// <returns>试题列表</returns>
        PagedList<ExaminationItem> SelectItemsByCondition(int? id, string condition, string txt);

        Listen SelectListen(Guid id);
        SkimmingScanningPartCompletion SelectSspc(Guid id);
        Listen SelectComplex(Guid id);
        ClozePart SelectCloze(Guid id);
        ReadingPartOption SelectRpo(Guid id);
        ReadingPartCompletion SelectRpc(Guid id);
        
    }
}