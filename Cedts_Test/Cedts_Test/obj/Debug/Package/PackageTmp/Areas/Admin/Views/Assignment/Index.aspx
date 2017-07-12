<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.ExaminationItem>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Index</h2>
    <table>
        <tr>
            <th>
                序号
            </th>
            <th>
                类型
            </th>
            <th>
                更新用户
            </th>
            <th>
                编辑
            </th>
        </tr>
        <%int i = 0; %>
        <% foreach (var item in Model)
           { %>
        <%i++; %>
        <tr>
            <td>
                <%:Html.Encode(i.ToString()) %>
            </td>
            <td>
                <%: item.ItemName %>
            </td>
            <td>
                <%: item.Username %>
            </td>
            <td>         
                <%: Html.ActionLink("编辑", "EditListen", new { id=item.AssessmentItemID }) %>
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
</asp:Content>
