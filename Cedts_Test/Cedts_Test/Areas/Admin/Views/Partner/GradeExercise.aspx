<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.ScoreInfo>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	年级正确率情况
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">



    <p class="tableinfo">时间段：<%:ViewData["StartDate"] %>至<%:ViewData["EndDate"] %></p>

    <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">
                    年级
                </th>
                <th class="pname">
                    做题数
                </th>
                <th class="pname">
                    做对题数
                </th>
                <th class="pname">
                    正确率
                </th>            
                <th class="saction">
                    操作
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
    ShowGoButton = false,
    PageIndexBoxWrapperFormatString = " 转到{0}",
    SeparatorHtml = ""
})%>
                </td>
            </tr>
        </tfoot>
        <tbody>
            <% foreach (var item in Model)
               { %>
            <tr>
                <td class="pname">
                    <%: item.StudName%>
                </td>
                <td class="pname">
                    <%: item.DoneNum%>
                </td>
                <td class="pname">
                    <%: item.CorrectNum%>
                </td>
                <td class="pname">
                    <%: item.AveScore.ToString("0.0")%>%
                </td>                              
                <td class="saction">
                      <%: Html.ActionLink("查看详情", "ClassExercise", new { id = item.StudNum })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>            


</asp:Content>
