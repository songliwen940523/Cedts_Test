<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<PagedList<Cedts_Test.Areas.Admin.Models.SingleScoreInfo>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<%if (Model.Count == 0)
      {%>
      <p class = "taskinfo">该班级没有学生！</p>
      <%} %>
      <%else
      { %>
    <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">
                    学号
                </th>
                <th class="pname">
                    姓名
                </th>
                <th class="pname">
                    得分率
                </th>
                <th class="pname">
                    得分
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
                    <%: item.StudNum%>
                </td>
                <td class="pname">
                    <%: item.StudName%>
                </td>
                <%if (item.Done)
                  { %>
                <td class="pname">
                    <%: item.Percent.ToString("0.0")%>%
                </td>
                <td class="pname">
                    <%: item.Score.ToString("0.0")%>/<%: item.TotalScore.ToString("0.0")%>
                </td>
                <%} %>
                <%else
                  { %>
                <td class="pname">
                    --
                </td>
                <td class="pname">
                    --
                </td>
                <%} %>                     
                <td class="saction">
                      <%: Html.ActionLink("查看详情", "SingleTest", new { id = item.UserID })%>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
    <%} %>