using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public interface ICedts_PartnerRepository
    {

        /// <summary>
        /// 获取管理员集合
        /// </summary>
        /// <returns>管理员集合</returns>
        List<CEDTS_User> GetAdmin();

        /// <summary>
        /// 检验院校名称是否存在
        /// </summary>
        /// <param name="PartnerName">单位名称</param>
        /// <param name="PartnerID">单位ID</param>
        /// <returns>true||false</returns>
        bool CheckPartnerName(string PartnerName, Guid? PartnerID);

        /// <summary>
        /// 验证院校邮件地址是否存在
        /// </summary>
        /// <param name="Email">邮箱地址</param>
        /// <param name="PartnerID">单位ID</param>
        /// <returns>true||false</returns>
        bool CheckPartnerEmail(string Email, Guid? PartnerID);

        /// <summary>
        /// 保存院校信息
        /// </summary>
        /// <param name="Partner">院校对象</param>
        void SavePartner(CEDTS_Partner Partner);

        /// <summary>
        /// 获取院校对象
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>返回院校对象</returns>
        CEDTS_Partner GetPartnerbyID(Guid PartnerID);

        /// <summary>
        /// 编辑时获取以选中的管理员
        /// </summary>
        /// <param name="Account">管理员帐号</param>
        /// <returns>用户对象</returns>
        CEDTS_User GetUserbyAccount(string Account);

        /// <summary>
        /// 更新院校信息
        /// </summary>
        /// <param name="Partner">院校对象</param>
        void ChangePartner(CEDTS_Partner Partner);

        /// <summary>
        /// 根据管理员帐号获取所属单位的ＩＤ
        /// </summary>
        /// <param name="UserAccount">管理员帐号</param>
        /// <returns>院校ＩＤ</returns>
        Guid GetPartnerIDbyUserAccount(string UserAccount);

        /// <summary>
        /// 获取单位下的教师
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>教师对象集合</returns>
        List<CEDTS_User> GetTeacherbyPartnerID(Guid PartnerID);

        /// <summary>
        /// 获取系别对象集合
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>年级对象集合</returns>
        List<CEDTS_Grade> GetGrade(Guid PartnerID, Guid MajorID);

        /// <summary>
        /// 获取年级对象集合
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>年级对象集合</returns>
        List<CEDTS_Grade> GetTGrade(Guid PartnerID, Guid MajorID);

        /// <summary>
        /// 获取班级对象集合
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>班级对象集合</returns>
        List<CEDTS_Class> GetClass(Guid PartnerID);

        /// <summary>
        /// 根据ID获取专业对象
        /// </summary>
        /// <param name="MajorID">专业ID</param>
        /// <returns>专业对象</returns>
        CEDTS_Major GetMajorbyID(Guid MajorID);

        /// <summary>
        /// 根据ID获取年级对象
        /// </summary>
        /// <param name="GradeID">年级ID</param>
        /// <returns>年级对象</returns>
        CEDTS_Grade GetGradebyID(Guid GradeID);

        /// <summary>
        /// 根据ID获取班级对象
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        /// <returns>班级对象</returns>
        CEDTS_Class GetClassbyID(Guid ClassID);

        /// <summary>
        /// 获取Partner对象集合
        /// </summary>
        /// <returns>Partner对象集合</returns>
        PagedList<CEDTS_Partner> GetPartner(int? id);

        /// <summary>
        /// 获取Major对象集合
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>Major对象集合</returns>
        List<CEDTS_Major> GetMajorByPID(Guid PartnerID);

        /// <summary>
        /// 获取Grade对象集合
        /// </summary>
        /// <param name="Major">专业ID</param>
        /// <returns>Grade对象集合</returns>
        List<CEDTS_Grade> GetGradeByMID(Guid MajorID);

        /// <summary>
        /// 获取Class对象集合
        /// </summary>
        /// <param name="Grade">年级ID</param>
        /// <returns>Class对象集合</returns>
        List<CEDTS_Class> GetClassByGID(Guid GradeID);

        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="str">用户密码</param>
        /// <returns>加密后的密码</returns>
        string HashPassword(string str);

        /// <summary>
        /// 保存教师信息
        /// </summary>
        /// <param name="Teacher">教师对象</param>
        void SaveTeacher(CEDTS_User Teacher);

        /// <summary>
        /// 获取教师信息
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns>教师对象</returns>
        CEDTS_User GetTeacherbyID(int id);

        /// <summary>
        /// 更改教师信息
        /// </summary>
        /// <param name="Teacher">教师对象</param>
        void ChangeTeacher(CEDTS_User Teacher);

        /// <summary>
        /// 根据院校ID获取专业对象集合
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>专业对象集合</returns>
        List<CEDTS_Major>GetMajorbyName(string MajorName, Guid PartnerID);

        /// <summary>
        /// 根据等级和院校ID获取专业对象集合
        /// </summary>
        /// <param name="Level">等级</param>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>专业对象集合</returns>
        List<CEDTS_Major> GetMajorbyLevel(int Level, Guid PartnerID);

        /// <summary>
        /// 根据等级和院校ID获取专业对象集合
        /// </summary>
        /// <param name="Level">等级</param>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>专业对象集合</returns>
        List<CEDTS_Major> NewGetMajorbyLevel(int Level, Guid MajorID, Guid PartnerID);

        /// <summary>
        /// 根据专业ID和院校ID获取专业对象集合
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>专业对象集合</returns>
        List<CEDTS_Major> GetMajorforTGrade(Guid MajorID, Guid PartnerID);

        /// <summary>
        /// 判断专业名字是否存在
        /// </summary>
        /// <param name="MajorName">专业名称</param>
        /// <param name="MajorID">专业ID</param>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>true（false）</returns>
        bool CheckMajorName(string MajorName, Guid? MajorID, Guid PartnerID);

        /// <summary>
        /// 保存专业信息
        /// </summary>
        /// <param name="Major">专业对象</param>
        void SaveMajor(CEDTS_Major Major);

        /// <summary>
        /// 编辑专业信息
        /// </summary>
        /// <param name="Major">专业对象</param>
        void ChangeMajor(CEDTS_Major Major);

        /// <summary>
        /// 判断年级名字是否存在
        /// </summary>
        /// <param name="GradeName">年级名称</param>
        /// <param name="GradeID">年级ID</param>
        /// <param name="MajorID">专业ID</param>
        /// <returns>true(false)</returns>
        bool CheckGradeName(string GradeName, Guid? MajorID, Guid? GradeID);

        /// <summary>
        /// 根据上级ID获取专业对象集合
        /// </summary>
        /// <param name="MajorID">上级专业ID</param>
        /// <returns>专业对象集合</returns>
        List<CEDTS_Major> GetMajorbyUpID(Guid MajorID);

        /// <summary>
        /// 保存年级信息
        /// </summary>
        /// <param name="Grade">年级对象</param>
        void SaveGrade(CEDTS_Grade Grade);

        /// <summary>
        /// 编辑年级信息
        /// </summary>
        /// <param name="Grade">年级对象</param>
        void ChangeGrade(CEDTS_Grade Grade);

        /// <summary>
        /// 判断班级名称是否已存在
        /// </summary>
        /// <param name="ClassName">班级名称</param>
        /// <param name="GradeID">年级ID</param>
        /// <param name="ClassID">班级ID</param>
        /// <returns>true(flase)</returns>
        bool CheckClassName(string ClassName, Guid? ClassID);

        /// <summary>
        /// 保存班级信息
        /// </summary>
        /// <param name="Class">班级对象</param>
        void SaveClass(CEDTS_Class Class);

        /// <summary>
        /// 编辑班级信息
        /// </summary>
        /// <param name="Class">班级对象</param>
        void ChangeClass(CEDTS_Class Class);

        /// <summary>
        /// 删除班级
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        void DeleteClass(Guid ClassID);

        /// <summary>
        /// 获取该年级下的所有班级ID
        /// </summary>
        /// <param name="GradeID">年级ID</param>
        /// <returns>班级ID集合</returns>
        List<Guid> GetClassIDList(Guid GradeID);

        /// <summary>
        /// 删除年级
        /// </summary>
        /// <param name="GradeID">年级ID</param>
        void DeleteGrade(Guid GradeID);

        /// <summary>
        /// 获取该专业下的所有年级ID
        /// </summary>
        /// <param name="MajorID">专业ID</param>
        /// <returns>年级ID集合</returns>
        List<Guid> GetGradeIDList(Guid MajorID);

        /// <summary>
        /// 删除专业
        /// </summary>
        /// <param name="MajorID">专业ID</param>
        void DeleteMajor(Guid MajorID);

        /// <summary>
        /// 删除教师或院校管理员
        /// </summary>
        /// <param name="id">教师或管理员ID</param>
        void DeleteUser(int id);

        /// <summary>
        /// 获取院校下的教师和管理员ID
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>教师和管理员ID集合</returns>
        List<int> GetAdminAndTheacher(Guid PartnerID);

        /// <summary>
        /// 获取院校下所有的专业ID
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        /// <returns>专业ID集合</returns>
        List<Guid> GetMajorIDList(Guid PartnerID);

        /// <summary>
        /// 删除院校
        /// </summary>
        /// <param name="PartnerID">院校ID</param>
        void DeletePartner(Guid PartnerID);
    }
}
