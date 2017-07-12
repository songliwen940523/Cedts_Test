<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PagedList<Cedts_Test.Areas.Admin.Models.ExaminationItem>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<div>
    <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">                    
                </th>
                <th class="pname">
                    所属试卷
                </th>
                <th class="pname">
                    出题人
                </th>
                <th class="pname">
                    难度
                </th>
                <th class="pname">
                    估时
                </th>
                <th class="pname">
                    分数
                </th>
                <th class="pname">
                    出题时间
                </th>
                <th class="pname">
                    详细
                </th>
            </tr>
        </thead>
        <tfoot>
            <tr>
                <td colspan="5" class="more">
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
    ShowGoButton = false,PageIndexBoxWrapperFormatString=" 转到{0}",SeparatorHtml = "" 
})%>
                </td>
            </tr>
        </tfoot>
        <tbody>
            <% foreach (var item in Model)
               { %>
            <tr>
                <td class="pname">
                    <input type="checkbox" name="ItemID" value="<%:item.AssessmentItemID %>" />
                </td>
                <td class="pname">
                    <%: item.PaperName %>
                </td>
                <td class="pname">
                    <%: item.Username %>
                </td>
                <td class="pname">
                    <%: item.Difficult %>
                </td>
                <td class="pname">
                    <%: item.Duration %>
                </td>
                <td class="pname">
                    <%: item.Score %>
                </td>
                <td class="pname">
                    <%: String.Format("{0:g}", item.SaveTime) %>
                </td>                
                <td class="pname">                   
                    <%:Html.ActionLink("题目详情", "DetailItem", new { ItemID = item.AssessmentItemID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
</div>


