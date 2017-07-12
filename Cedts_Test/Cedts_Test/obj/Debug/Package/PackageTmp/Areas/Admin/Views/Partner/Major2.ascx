<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PagedList<Cedts_Test.Areas.Admin.Models.Major>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<table class="tbRecords" cellspacing="0" >
    <tr>
        <th>
        </th>
        <th class="pname">
            一级学科
        </th>
        <th class="pname">
            备 注
        </th>
        <th class="pname">
            所属门类
        </th>
        <th>
        </th>
    </tr>
    <% foreach (var item in Model)
       { %>
    <tr>
        <td>
            <%:Html.Hidden("MajorID",item.MajorID) %>
        </td>
        <td>
            <%: item.MajorName %>
        </td>
        <td>
            <%: item.MajorMark %>
        </td>
        <td>
            <%: item.UpMajorName %>
        </td>
        <td>
            <%: Html.ActionLink("编辑", "EditMajor", new { id = item.MajorID })%>
            
            <a href="#" id="<%:item.MajorID %>" class="a">删除</a>
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