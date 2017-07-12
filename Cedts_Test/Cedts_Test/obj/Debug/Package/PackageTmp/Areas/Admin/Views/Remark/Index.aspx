<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.Cedts_UserTest>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    教师评审首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <script type="text/javascript">
     $(function () {
         $("#s1").change(function () {
             var num = $(this).get(0).selectedIndex;
             window.location.href = "/Admin/Remark/Index?num=" + num;
         })
     })
    </script>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("教师评审首页","Index") %>
    </div>
    班级名称:<%:Html.DropDownList("classIDList",null,new{id="s1"})%>
    <table class="tbRecords" cellspacing="0" >
        <tr>
            <th class="pname">
                用户名称
            </th>
            <th class="pname">
                试卷标题
            </th>
            <th class="pname">
                用户得分
            </th>
            <th class="pname">
                评审状态
            </th>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td >
                <%: item.UserName %>
            </td>
            <td >
                <%: item.PaperTitle %>
            </td>
            <td >
                <%: String.Format("{0:F}", item.TotalScore) %>
            </td>
            <td >
                <%if (item.IsChecked == 0)
                  { %>
                <span style="color: Red">未评审</span>
                <%} %>
                <%else
                  { %>
                <span>已评审</span>
                <%} %>
            </td>
            <td>
                <%if (item.IsChecked == 0)
                  { %>
                <%:Html.ActionLink("评阅", "Remark", new { TestID = item.TestID, PaperID = item.PaperID, type=0 })%>
                <%}
                  else
                  { %>
                  <%:Html.ActionLink("查看评阅", "Remark", new { TestID = item.TestID, PaperID = item.PaperID, type=1 })%>
                <%}%>
            </td>
        </tr>
        <% } %>
    </table>
    <%if (Model.Count == 0)
          { %>
           <center>当前没有可评阅的试卷！</center>
        <%} %>
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
</asp:Content>
