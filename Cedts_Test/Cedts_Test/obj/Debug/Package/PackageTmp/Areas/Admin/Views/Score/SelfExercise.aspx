<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.ScoreInfo>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	自主练习
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p class="tableinfo">时间段：<%:ViewData["StartDate"] %>至<%:ViewData["EndDate"] %>&nbsp;&nbsp;&nbsp;&nbsp;班级名称：<%:ViewData["ClassName"] %></p>
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
                    自主做题数
                </th>
                <th class="pname">
                    正确题数
                </th>
                <th class="pname">
                    自主正确率
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
                <td class="pname">
                    <%: item.DoneNum%>
                </td>
                <td class="pname">
                    <%: item.CorrectNum%>
                </td>
                <%if (item.DoneNum == 0)
                  { %>
                    <td class="pname">
                        --
                    </td>
                <%} %>
                <%else
                  { %>
                    <td class="pname">
                        <%: item.DoneScore.ToString("0.0")%>%
                    </td>
                <%} %> 
            </tr>
            <% } %>
        </tbody>
    </table>
    <%} %>
</asp:Content>
