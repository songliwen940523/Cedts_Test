<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.TeacherInfo>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    教师管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("教师管理首页","Teacher") %>
    </div>
    <table class="tbRecords" cellspacing="0" >
        <tr>
            <th class="pname">
                教师帐号
            </th>
            <th class="pname">
                教师邮箱
            </th>
            <th class="pname">
                教师性别
            </th>
            <th class="pname">
                教师角色
            </th>
            <th class="pname">
                教师姓名
            </th>
            <th class="pname">
                教师电话
            </th>
            <th class="pname">
                所属单位
            </th>
           <%-- <th class="pname">
                专业
            </th>
            <th class="pname">
                年级
            </th>
            <th class="pname">
                班级
            </th>--%>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%: item.UserAccount %>
            </td>
            <td>
                <%: item.Email %>
            </td>
            <td>
                <%: item.Sex %>
            </td>
            <td>
                <%: item.Role %>
            </td>
            <td>
                <%: item.UserNickname %>
            </td>
            <td>
                <%: item.Phone %>
            </td>
            <td>
                <%: item.Partner %>
            </td>
           <%-- <td>
                <%: item.Major %>
            </td>
            <td>
                <%: item.Grade %>
            </td>
            <td>
                <%: item.Class %>
            </td>--%>
            <td>
                <%: Html.ActionLink("编辑", "EditTeacher", new {  id=item.UserID}) %>
                
               <a href="#" id="<%:item.UserID %>" class="a">删除</a>
            </td>
        </tr>
        <% } %>
    </table>
    <%=Html.Pager(Model, new PagerOptions  
{  
    PageIndexParameterName = "id",  
    CssClass = "pages", 
    FirstPageText = "首页", 
    LastPageText = "末页",  
    PrevPageText = "上一页",  
    NextPageText = "下一页",  
    CurrentPagerItemWrapperFormatString = "<span class=\"cpb\">{0}</span>",  
    ShowPageIndexBox = true, 
    NumericPagerItemWrapperFormatString = "<span class=\"item\">{0}</span>",  
    PageIndexBoxType = PageIndexBoxType.DropDownList,   
    ShowGoButton = false,PageIndexBoxWrapperFormatString=" 转到{0}",SeparatorHtml = "" })%>
    <p>
        <%: Html.ActionLink("新增教师", "CreateTeacher") %>
    </p>
    <script type="text/javascript">
        $(function () {
            $.messager.defaults = { ok: "确   定", cancel: "取   消" };
            $(".a").click(function () {
                var id = $(this).attr("id");
                $.messager.confirm('提    示', '<b>你确认要删除该教师?<br>如果删除，该班的教师将需要您重新分配！</b>', function (r) {
                    if (r) {
                        location.href = "/Admin/Partner/DeleteTeacher?id=" + id;
                    }
                });
            });
        });
    </script>
</asp:Content>
