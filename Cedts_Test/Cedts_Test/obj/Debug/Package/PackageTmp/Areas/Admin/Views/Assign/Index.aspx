<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.ExaminationItem>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%  using (Html.BeginForm())
        { %>
    <div id="Search">
        查询条件：<select id="condition" name="condition">
            <option value="1">试题ID</option>
        </select>
        <input type="text" id="txtSearch" name="txtSearch" />
        <input type="submit" id="btnSearch" value="查 询" />
    </div>
    <%} %>
    <table class="tbRecords" cellspacing="0">
        <tr>
            <th class="pstat">
                编号
            </th>
            <th class="pstat">
                类型
            </th>
            <th class="pstat">
                编辑
            </th>
        </tr>
        <%int i = 0; %>
        <% foreach (var item in Model)
           { %>
        <%i++; %>
        <tr>
            <td class="pstat">
                <%: Html.Encode( i.ToString()) %>
            </td>
            <td class="pstat">
                <%: item.ItemName %>
            </td>
            <td class="pstat">
                <%: Html.ActionLink("编辑", "Listen", new { id=item.AssessmentItemID }) %>
            </td>
            <td>
                
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
