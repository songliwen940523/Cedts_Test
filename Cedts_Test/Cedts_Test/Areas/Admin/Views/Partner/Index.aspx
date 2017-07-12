<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.CEDTS_Partner>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    院校管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("院校管理首页", "Index")%>
    </div>
    <table  class="tbRecords" cellspacing="0" >
        <tr>
            <th class="pname">
                院校名称
            </th>
            <th class="pname">
                所属省份
            </th>
            <th class="pname">
                所属区市
            </th>
            <th class="pname">
                详细地址
            </th>
            <th class="pname">
                联系人
            </th>
            <th class="pname">
                联系电话
            </th>
            <th class="pname">
                手机
            </th>
            <th class="pname">
                邮箱地址
            </th>
            <th class="pname">
                官网地址
            </th>
            <th class="pname">
                管理账户
            </th>
            <th class="pname">
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td >
                <%: item.PartnerName %>
            </td>
            <td >
                <%: item.Province %>
            </td>
            <td>
                <%: item.City %>
            </td>
            <td>
                <%: item.Address %>
            </td>
            <td>
                <%: item.Principal %>
            </td>
            <td>
                <%: item.Telphone %>
            </td>
            <td>
                <%: item.Mobilephone %>
            </td>
            <td>
                <%: item.Email %>
            </td>
            <td>
                <%: item.Src %>
            </td>
            <td>
                <%: item.AdminAccount %>
            </td>
            <td>
                <%: Html.ActionLink("编    辑", "Edit", new { id=item.PartnerID }) %>
                
                 <a href="#" id="<%:item.PartnerID %>" class="a">删 除</a>
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
    <p class="actionbar">
        <%: Html.ActionLink("新增院校", "Create") %>
    </p>
    <script type="text/javascript">
        $(function () {
            $.messager.defaults = { ok: "确   定", cancel: "取   消" };
            $(".a").click(function () {
                var id = $(this).attr("id");
                $.messager.confirm('提    示', '<b>你确认要删除该班级?<br>如果删除，与之相关的用户、班级、年级、专业信息将全部丢失！</b>', function (r) {
                    if (r) {
                        location.href = "/Admin/Partner/Delete?id=" + id;
                    }
                });
            });
        });
    </script>
</asp:Content>
