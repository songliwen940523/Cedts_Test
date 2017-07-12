using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_KnowledgeRepository
    {
        //查询知识能力类
        PagedList<CEDTS_KnowledgePoints> SelectSort(int? id);

        //查询知识能力面-一级知识点
        List<KnowSide> SelectSide(int? id);

        //查询知识能力点-二级知识点
        List<KnowPoint> SelectPoint(int? id);

        //查询知识能力类（大类型）名称
        List<string> SelectTypeName();

        //查询Sort重名
        int CheckSort(string name);

        //删除知识能力类
        int DeleteSort(string[] name);

        //删除知识能力面-一级知识点
        int DeleteSide(string[] name);

        //删除知识能力点-二级知识点
        int DeletePoint(string[] name);

        //新增大类型-知识能力类
        void CreateSort(CEDTS_KnowledgePoints part);

        //新增知识能力面-一级知识点
        void CreateSide(CEDTS_KnowledgePoints Knowledge);

        //新增知识能力点-二级知识点
        void CreatePoint(CEDTS_KnowledgePoints Knowledge);

       //编辑时查询当前点击的知识能力类（大类型）信息
        CEDTS_KnowledgePoints SelectEditSort(Guid id);

        //编辑更新当前点击的知识能力类（大类型）信息
        void EditSort(CEDTS_KnowledgePoints Knowledge);

        //编辑时查询当前点击的知识能力面（一级知识点）信息
        CEDTS_KnowledgePoints SelectEditSide(Guid id);

        //编辑更新当前点击的知识能力面（一级知识点）信息
        void EditSide(CEDTS_KnowledgePoints Knowledge);

        //编辑时查询当前点击的知识能力点（二级知识点）信息
        CEDTS_KnowledgePoints SelectEditPoint(Guid id);

        //编辑更新当前点击的知识能力点（二级知识点）信息
        void EditPoint(CEDTS_KnowledgePoints Knowledge);

        //修改知识能力面（一级知识点）信息时获取Sort下列表信息
        List<SelectListItem> SortList(Guid id);

        //页面首次加载时向Side下拉列表注入值
        List<SelectListItem> SideList(Guid id);

        //点击Sort下拉列表联动Side下拉列表
        List<SelectListItem> GetSide(Guid? type);

        //修改知识能力点（二级知识点）信息获取Sort下拉列表信息
        List<SelectListItem> GetPartSide();

        //修改知识能力点（二级知识点）信息获取Side下拉列表信息
        List<SelectListItem> FirstSide();

        /// <summary>
        /// 检查Point名称是否已经存在
        /// </summary>
        /// <param name="Account">知识点名称</param>
        /// <returns>bool</returns
        bool AjaxCheckPointTitle(string Title,int type);


    }
}