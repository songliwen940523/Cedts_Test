using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cedts_Test.Areas.Admin.Models;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_KnowledgeRepository : ICedts_KnowledgeRepository
    {
        Cedts_Entities db;
        public Cedts_KnowledgeRepository()
        {
            db = new Cedts_Entities();
        }

        int defaultPageSize = 10;
        /// <summary>
        /// 知识能力类查询
        /// </summary>
        /// <returns>集合</returns>
        PagedList<CEDTS_KnowledgePoints> ICedts_KnowledgeRepository.SelectSort(int? id)
        {
            IQueryable<CEDTS_KnowledgePoints> Sort = (from m in db.CEDTS_KnowledgePoints where m.Level == 1 orderby m.Title ascending select m);

            PagedList<CEDTS_KnowledgePoints> Sorts = Sort.ToPagedList(id ?? 1, defaultPageSize);
            return Sorts;
        }

        /// <summary>
        /// 知识能力面查询
        /// </summary>
        /// <returns>集合</returns>
        List<KnowSide> ICedts_KnowledgeRepository.SelectSide(int? id)
        {
            var kp1 = from m1 in db.CEDTS_KnowledgePoints
                      from m2 in db.CEDTS_KnowledgePoints
                      where m1.Level == 2 && m1.UperKnowledgeID == m2.KnowledgePointID
                      orderby m1.Title ascending
                      select new
                      {
                          KnowID = m1.KnowledgePointID,
                          SortName = m2.Title,
                          SideName = m1.Title,
                          Describe = m1.Describe
                      };
            //PagedList<KnowSide> kp2 = kp1.ToPagedList(id ?? 1, defaultPageSize);
            //return kp2;
            List<KnowSide> list = new List<KnowSide>();

            for (int i = 0; i < kp1.ToList().Count; i++)
            {
                KnowSide ks = new KnowSide();
                ks.KnowID = kp1.ToList().ElementAtOrDefault(i).KnowID;
                ks.SortName = kp1.ToList().ElementAtOrDefault(i).SortName;
                ks.SideName = kp1.ToList().ElementAtOrDefault(i).SideName;
                ks.Describe = kp1.ToList().ElementAtOrDefault(i).Describe;
                list.Add(ks);
            }
            return list;
        }


        /// <summary>
        ///  知识能力点查询
        /// </summary>
        /// <returns>集合</returns>
        List<KnowPoint> ICedts_KnowledgeRepository.SelectPoint(int? id)
        {
            var kp2 = from m1 in db.CEDTS_KnowledgePoints
                      from m2 in db.CEDTS_KnowledgePoints
                      from m3 in db.CEDTS_KnowledgePoints
                      where m1.UperKnowledgeID == m2.KnowledgePointID && m1.Level == 3 && m2.UperKnowledgeID == m3.KnowledgePointID
                      orderby m1.Title ascending
                      select new
                      {
                          KnowID = m1.KnowledgePointID,
                          SortName = m3.Title,
                          SideName = m2.Title,
                          PointName = m1.Title,
                          Describle = m1.Describe
                      };
            //PagedList<KnowPoint> kps=kp2.ToPagedList(id??1,defaultPageSize);
            //return kps;
            List<KnowPoint> list = new List<KnowPoint>();
            for (int i = 0; i < kp2.ToList().Count; i++)
            {
                KnowPoint kp = new KnowPoint();
                kp.KnowID = kp2.ToList().ElementAtOrDefault(i).KnowID;
                kp.SortName = kp2.ToList().ElementAtOrDefault(i).SortName;
                kp.SideName = kp2.ToList().ElementAtOrDefault(i).SideName;
                kp.PointName = kp2.ToList().ElementAtOrDefault(i).PointName;
                kp.Describle = kp2.ToList().ElementAtOrDefault(i).Describle;
                list.Add(kp);
            }
            return list;
        }

        /// <summary>
        ///   查询类型名称
        /// </summary>
        /// <returns>类型名称集合</returns>
        List<string> ICedts_KnowledgeRepository.SelectTypeName()
        {
            var name = (from m in db.CEDTS_PartType select m.TypeName_CN).ToList();
            return name;
        }

        int ICedts_KnowledgeRepository.CheckSort(string name)
        {
            int num = 0;
            var Sort = (from m in db.CEDTS_PartType where m.TypeName_CN == name select m).ToList();
            if (Sort.Count > 0)
            {
                num = 0;
                return num;
            }
            else
            {
                num = 1;
                return num;
            }
        }

        /// <summary>
        ///  新增Sort知识能力类（大类型）
        /// </summary>
        /// <param name="part">新增Sort知识能力类（大类型）信息</param>
        void ICedts_KnowledgeRepository.CreateSort(CEDTS_KnowledgePoints part)
        {
            db.AddToCEDTS_KnowledgePoints(part);
            db.SaveChanges();
        }

        /// <summary>
        /// 新增SIde知识能力面（一级知识点）
        /// </summary>
        /// <param name="knowledge">新增知识能力面（一级知识点）信息</param>
        void ICedts_KnowledgeRepository.CreateSide(CEDTS_KnowledgePoints knowledge)
        {
            db.AddToCEDTS_KnowledgePoints(knowledge);
            db.SaveChanges();
        }

        /// <summary>
        /// 新增PointSort知识能力点（二级知识点）
        /// </summary>
        /// <param name="Knowledge">新增知识能力点（二级知识点）信息</param>
        void ICedts_KnowledgeRepository.CreatePoint(CEDTS_KnowledgePoints Knowledge)
        {
            db.AddToCEDTS_KnowledgePoints(Knowledge);
            db.SaveChanges();
        }

        /// <summary>
        /// 编辑知识能力类（大类型）当前点击查询知识能力类（大类型）
        /// </summary>
        /// <param name="id">知识能力类（大类型）ID</param>
        /// <returns>Sort知识能力类（大类型）信息</returns>
        CEDTS_KnowledgePoints ICedts_KnowledgeRepository.SelectEditSort(Guid id)
        {
            var Sort = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == id select m).FirstOrDefault();
            return Sort;
        }

        /// <summary>
        /// 编辑更新知识能力类（大类型）当前点击知识能力类（大类型）
        /// </summary>
        /// <param name="Knowledge">编辑更新知识能力类（大类型）当前点击知识能力类（大类型）信息</param>
        void ICedts_KnowledgeRepository.EditSort(CEDTS_KnowledgePoints Knowledge)
        {
            var sort = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge.KnowledgePointID select m).FirstOrDefault();

            Knowledge.UperKnowledgeID = sort.UperKnowledgeID;
            Knowledge.Level = sort.Level;
            Knowledge.ConstantC = sort.ConstantC;

            db.ApplyCurrentValues(sort.EntityKey.EntitySetName, Knowledge);
            db.SaveChanges();
        }

        /// <summary>
        /// 编辑更新查询Sort下拉列表类容
        /// </summary>
        /// <param name="id">当前点击的ID</param>
        /// <returns></returns>
        List<SelectListItem> ICedts_KnowledgeRepository.SortList(Guid id)
        {
            var Side = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == id select m).FirstOrDefault();
            var partlist = (from m in db.CEDTS_KnowledgePoints where m.Level == 1 select m).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var part in partlist)
            {
                if (Side.UperKnowledgeID == part.KnowledgePointID)
                {
                    list.Add(new SelectListItem { Text = part.Title, Value = part.KnowledgePointID.ToString(), Selected = true });
                }
                else
                {
                    list.Add(new SelectListItem { Text = part.Title, Value = part.KnowledgePointID.ToString() });
                }
            }
            return list;
        }

        /// <summary>
        /// 编辑查询Side下拉列表类容
        /// </summary>
        /// <param name="id">知识能力类（大类型）ID</param>
        /// <returns>List<SelectListItem></returns>
        List<SelectListItem> ICedts_KnowledgeRepository.SideList(Guid id)
        {
            List<SelectListItem> listside = new List<SelectListItem>();
            var Point = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == id select m.UperKnowledgeID).FirstOrDefault();
            var s = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Point select m).FirstOrDefault();
            var Sidelist = (from m in db.CEDTS_KnowledgePoints where m.Level == 2 && m.UperKnowledgeID == s.UperKnowledgeID orderby m.Title descending select m).ToList();

            foreach (var side in Sidelist)
            {
                if (Point == side.KnowledgePointID)
                {
                    listside.Add(new SelectListItem { Text = side.Title, Value = side.KnowledgePointID.ToString(), Selected = true });
                }
                else
                {
                    listside.Add(new SelectListItem { Text = side.Title, Value = side.KnowledgePointID.ToString() });
                }
            }
            return listside;
        }

        /// <summary>
        /// 编辑更新知识能力点（一级知识点）获取当前点击的知识能力点（一级知识点）信息
        /// </summary>
        /// <param name="id">知识能力点（一级知识点）ID</param>
        /// <returns>知识能力点（一级知识点）信息</returns>
        CEDTS_KnowledgePoints ICedts_KnowledgeRepository.SelectEditSide(Guid id)
        {
            var Side = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == id select m).First();
            return Side;
        }


        /// <summary>
        /// 修改知识能力点（二级知识点）信息获取Sort下拉列表信息
        /// </summary>
        /// <returns> List<SelectListItem></returns>
        List<SelectListItem> ICedts_KnowledgeRepository.GetPartSide()
        {
            var GetPartList = (from m in db.CEDTS_KnowledgePoints where m.Level == 1 orderby m.Title descending select m).ToList();
            List<SelectListItem> Part = new List<SelectListItem>();
            foreach (var GetPart in GetPartList)
            {
                Part.Add(new SelectListItem { Text = GetPart.Title, Value = GetPart.KnowledgePointID.ToString() });
            }
            return Part;

        }

        /// <summary>
        /// 修改知识能力点（二级知识点）信息获取Side下拉列表信息
        /// </summary>
        /// <returns>List<SelectListItem></returns>
        List<SelectListItem> ICedts_KnowledgeRepository.FirstSide()
        {
            var FirstSideList = (from m in db.CEDTS_KnowledgePoints where m.Level == 2 orderby m.Title descending select m).ToList();
            List<SelectListItem> side = new List<SelectListItem>();
            foreach (var FirstSide in FirstSideList)
            {
                side.Add(new SelectListItem { Text = FirstSide.Title, Value = FirstSide.KnowledgePointID.ToString() });
            }
            return side;
        }

        /// <summary>
        /// 点击知识能力类（大类型）下拉列表联动Side知识能力面（二级知识面）下拉列表
        /// </summary>
        /// <param name="type">知识能力类（大类型）ID</param>
        /// <returns>List<SelectListItem></returns>
        List<SelectListItem> ICedts_KnowledgeRepository.GetSide(Guid? type)
        {
            var planList = db.CEDTS_KnowledgePoints.Where(p => p.KnowledgePointID == type).ToList();
            if (planList.ToList().Count == 0)
            {
                List<SelectListItem> item = new List<SelectListItem>();
                item.Insert(0, new SelectListItem { Text = "暂无元素-请添加元素", Value = "" });
                return item;
            }
            else
            {

                var sidelist = (from m in db.CEDTS_KnowledgePoints where m.UperKnowledgeID == type && m.Level == 2 orderby m.Title select m).ToList();
                List<SelectListItem> itemtype = new List<SelectListItem>();
                foreach (var side in sidelist)
                {
                    itemtype.Add(new SelectListItem { Text = side.Title, Value = side.KnowledgePointID.ToString() });
                }
                return itemtype;
            }
        }


        /// <summary>
        /// 编辑更新知识能力面（一级知识点）当前点击知识能力面（一级知识点）
        /// </summary>
        /// <param name="Knowledge">编辑更新知识能力面（一级知识点）当前点击知识能力面（一级知识点）信息</param>
        void ICedts_KnowledgeRepository.EditSide(CEDTS_KnowledgePoints Knowledge)
        {
            var Side = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge.KnowledgePointID select m).First();
            Knowledge.KnowledgePointID = Side.KnowledgePointID;
            Knowledge.Level = Side.Level;
            Knowledge.UperKnowledgeID = Side.UperKnowledgeID;
            db.ApplyCurrentValues(Side.EntityKey.EntitySetName, Knowledge);
            db.SaveChanges();

        }

        /// <summary>
        /// 编辑更新知识能力点（二级知识点）获取当前点击的知识能力点（二级知识点）信息
        /// </summary>
        /// <param name="id">知识能力点（二级知识点）ID</param>
        /// <returns>知识能力点（二级知识点）信息</returns>
        CEDTS_KnowledgePoints ICedts_KnowledgeRepository.SelectEditPoint(Guid id)
        {
            var Point = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == id select m).First();
            return Point;
        }

        /// <summary>
        /// 编辑更新知识能力点（二级知识点）当前点击知识能力点（二级知识点）
        /// </summary>
        /// <param name="Knowledge">编辑更新知识能力点（二级知识点）当前点击知识能力点（二级知识点）信息</param>
        void ICedts_KnowledgeRepository.EditPoint(CEDTS_KnowledgePoints Knowledge)
        {
            var Point = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge.KnowledgePointID select m).First();
            db.ApplyCurrentValues(Point.EntityKey.EntitySetName, Knowledge);
            db.SaveChanges();
        }

        /// <summary>
        /// 删除知识能力类（大类型）
        /// </summary>
        /// <param name="name">选中的知识能力类（大类型）ID</param>
        /// <returns>数字</returns>
        int ICedts_KnowledgeRepository.DeleteSort(string[] name)
        {
            int count = DeleteSort1(name);
            return count;
        }

        /// <summary>
        /// 删除知识能力面（一级知识点）
        /// </summary>
        /// <param name="name">知识能力面（一级知识点）ID</param>
        /// <returns></returns>
        int ICedts_KnowledgeRepository.DeleteSide(string[] name)
        {
            int count = DeleteSort1(name);
            return count;
        }

        /// <summary>
        /// 删除Point知识能力点（二级知识点）
        /// </summary>
        /// <param name="name">Point知识能力点（二级知识点）ID</param>
        /// <returns></returns>
        int ICedts_KnowledgeRepository.DeletePoint(string[] name)
        {
            int count = DeleteSort1(name);
            return count;
        }

        /// <summary>
        /// 删除知识能力点、知识能力面时调用方法
        /// </summary>
        /// <param name="ids">知识能力集合</param>
        /// <returns></returns>
        public int DeleteSort1(string[] ids)
        {
            int num = 0;
            ids = ids[0].Split(',');
            List<string> list = new List<string>();
            for (int i = 0; i < ids.Count(); i++)
            {
                list.Add(ids[i]);
            }

            foreach (string str in list)
            {
                Guid guid = Guid.Parse(str);
                var item = db.CEDTS_KnowledgePoints.Where(p => p.UperKnowledgeID == guid && p.KnowledgePointID != guid).FirstOrDefault();
                if (item == null)
                {
                    var qkIDs = (from m in db.CEDTS_QuestionKnowledge where m.KnowledgePointID == guid select m).FirstOrDefault();

                    if (qkIDs != null)
                    {
                        return num;
                    }

                    var DelId = db.CEDTS_KnowledgePoints.Where(p => p.KnowledgePointID == guid).FirstOrDefault();
                    db.DeleteObject(DelId);
                }
                else
                {
                    return num;
                }
            }
            db.SaveChanges();
            num = 1;
            return num;
        }

        #region 验证Point
        bool ICedts_KnowledgeRepository.AjaxCheckPointTitle(string Title, int type)
        {
            if (db.CEDTS_KnowledgePoints.Where(p => p.Title == Title).FirstOrDefault() == null)
            {
                return false;
            }
            else
            {
                if (db.CEDTS_KnowledgePoints.Where(p => p.Title == Title && p.Level == type) != null)
                    return true;
                else
                    return false;
            }
        }
        #endregion

    }
}