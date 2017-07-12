<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.ScoreInfo>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	所有练习
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p class="tableinfo">时间段：<%:ViewData["StartDate"] %>至<%:ViewData["EndDate"] %></p>
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
                <%foreach(var testname in Model[0].TestNames)
                 { %>
                <th class="pname">
                    <%:testname %>
                </th> 
                <%} %>  
                <th class="pname">
                    自主做题数
                </th>
                <th class="pname">
                    自主得分率
                </th>
                <th class="pname">
                    平均得分率
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
                <%foreach (var score in item.TestScore)
                  {%>
                    <td class="pname">
                    <%: score.ToString("0.0") %>%
                    </td>
                <%} %>
                <td class="pname">
                    <%: item.DoneNum%>
                </td>
                <td class="pname">
                    <%: item.DoneScore.ToString("0.0")%>%
                </td>
                <td class="pname">
                    <%: item.AveScore.ToString("0.0")%>%
                </td>                              
            </tr>
            <% } %>
        </tbody>
    </table>
    <%} %>
</asp:Content>
