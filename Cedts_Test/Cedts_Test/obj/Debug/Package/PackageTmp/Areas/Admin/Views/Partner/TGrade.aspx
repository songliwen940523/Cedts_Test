<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.Grade>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    年级管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("年级管理首页","TGrade") %>
    </div>
    <table class="tbRecords" cellspacing="0" >
        <tr>
             <th>
            </th>
            <th class="pname">
                年级名称
            </th>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%:Html.Hidden("GradeID",item.GradeID) %>
            </td>
            <td>
                <%: item.GradeName %>
            </td>
            <td>
                <%: Html.ActionLink("编    辑", "EditTGrade", new { id=item.GradeID }) %>
                
                <a href="#" id="<%:item.GradeID %>" class="a">删 除</a>
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
        <%: Html.ActionLink("新增年级", "CreateTGrade") %>
    </p>
    <script type="text/javascript">
        $(function () {
            $.messager.defaults = { ok: "确   定", cancel: "取   消" };
            $(".a").click(function () {
                var id = $(this).attr("id");
                $.messager.confirm('提    示', '<b>你确认要删除该年级?<br>如果删除，与之相关的用户和班级信息将全部丢失！</b>', function (r) {
                    if (r) {
                        location.href = "/Admin/Partner/DeleteTGrade?id=" + id;
                    }
                });
            });
        });
    </script>
</asp:Content>
