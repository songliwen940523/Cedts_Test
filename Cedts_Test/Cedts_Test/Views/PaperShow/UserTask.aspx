<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Models.AssignedTask>>" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	用户作业
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<p class = "tasktitle">未完成作业列表</p>
    <div class = "tasklist">
    <%if (Model.Count == 0)
      {%>
      <p class = "taskinfo">当前没有未做的作业！</p>
      <%} %>
      <%else
      { %>
    <table class="tbRecords" cellspacing="0">
        <thead>
            <tr>
                <th class="pname">
                    作业名称
                </th>
                <th class="ptime">
                    发布时间
                </th>
                <th class="pstat">
                    完成状态
                </th>                
                <th class="paction">
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
                    <%: item.TaskName%>
                </td>
                <td class="ptime">
                    <%: String.Format("{0:g}", item.AssignTime)%>
                </td>
                <td class="pstat">
                <%if (item.IsFinished == true)
                  {%>
                    暂停中
                    <%}
                  else
                  {%>
                  <span>未练习</span>
                    <%} %>
                </td>                
                <td class="paction">
                    
                    <%if (item.IsFinished == true)
                      { %>
                      <%:Html.ActionLink("继续练习", "ContinuesPractice", new { TestID = item.TaskID })%>                     
                    <%}
                      else
                      {%>
                      <%: Html.ActionLink("开始练习", "Exerciser", new { id = item.PaperID })%>
                    <%} %>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
    <%} %>
</div>

</asp:Content>
