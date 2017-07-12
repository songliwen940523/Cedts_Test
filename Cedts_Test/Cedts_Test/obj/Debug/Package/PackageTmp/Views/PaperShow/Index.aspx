<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Models.CEDTS_Paper>>" %>

<%@ Import Namespace="Cedts_Test.Models" %>
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
                试卷标题
            </th>
            <th>
                试卷类型
            </th>
            <th>
                试卷估时
            </th>
            <th>
                试卷难度
            </th>
            <th>
                试卷总分
            </th>
            <th>
                试卷描述
            </th>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%: item.Title %>
            </td>
            <td>
                <%: item.Type %>
            </td>
            <td>
                <%: item.Duration %>
            </td>
            <td>
                <%: String.Format("{0:F}", item.Difficult) %>
            </td>
            <td>
                <%: item.Score %>
            </td>
            <td>
                <%: item.Description %>
            </td>
            <td>
                <%: Html.ActionLink("练习", "Exerciser", new {id = item.PaperID })%>
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
