using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_ItemXMLRepository
    {
        /// <summary>
        /// 获取听力对象
        /// </summary>
        /// <param name="text">试题xml内容</param>
        /// <param name="type">itemtypeid</param>
        /// <returns>听力对象</returns>
        Listen Getlisten(string text,string type);

        /// <summary>
        /// 获取快速阅读对象
        /// </summary>
        /// <param name="text">试题XML内容</param>
        /// <param name="type">itemtypeid</param>
        /// <returns>快速阅读对象</returns>
        SkimmingScanningPartCompletion GetSspc(string text, string type);

        /// <summary>
        /// 获取完形填空对象
        /// </summary>
        /// <param name="text">试题XML内容</param>
        /// <param name="type">itemtypeid</param>
        /// <returns>完型填空对象</returns>
        ClozePart GetCp(string text, string type);

        /// <summary>
        /// 获取阅读理解选词填空对象
        /// </summary>
        /// <param name="text">试题XML内容</param>
        /// <param name="type">itemtypeid</param>
        /// <returns>阅读理解选词填空对象</returns>
        ReadingPartCompletion GetRpc(string text, string type);

        /// <summary>
        /// 获取阅读理解选择题型对象
        /// </summary>
        /// <param name="text">试题XML内容</param>
        /// <param name="type">itemtypeid</param>
        /// <returns>阅读理解选择题型对象</returns>
        ReadingPartOption GetRpo(string text, string type);

        /// <summary>
        /// 获取知识点ID
        /// </summary>
        /// <returns>int(试题类型ID)</returns>
        List<Guid> GetPointID();

        /// <summary>
        /// 获取试题库表中的数据
        /// </summary>
        /// <returns>返回一个List（ItemBank）</returns>
        List<ItemBank> GetOldItem();
        
    }
}
