<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.CEDTS_Paper>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    试卷管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".label").click(function () {
                if (!window.confirm("确定要删除您选择的数据项吗?")) {
                    return;
                }

                var id = $(this).attr("id").split("_")[1];
                $.ajax({
                    type: "POST",
                    url: "/Admin/Paper/DeletePaper/",
                    data: { id: function () { return id } },
                    dataType: "html",
                    success: function (data) {
                        if (data == 1) {
                            alert("删除成功!");
                        }
                        else {
                            alert("删除失败!");
                        }
                        window.location.reload();
                    }
                });
            });
        });
    </script>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("试卷管理首页","Index") %>
    </div>
    <div class="divfloat">
        <div id="divpages">
            <table class="tbRecords">
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
                        试卷分数
                    </th>
                    <th>
                        试卷描述
                    </th>
                    <th>
                        创建时间
                    </th>
                    <th>
                        创建人
                    </th>
                    <th>
                        最近更新人
                    </th>
                    <th>
                        最近更新时间
                    </th>
                    <th>
                        编辑
                    </th>
                    <th>
                        删除
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
                        <%: String.Format("{0:g}", item.CreateTime) %>
                    </td>
                    <td>
                        <%: item.UserID %>
                    </td>
                    <td>
                        <%: item.UpdateUserID %>
                    </td>
                    <td>
                        <%: String.Format("{0:g}", item.UpdateTime) %>
                    </td>
                    <td>
                        <%if (item.State == 2) %>
                        <%{ %>
                        <%: Html.ActionLink("继续编辑", "Edit", new { id = item.PaperID })%>
                        <%} %>
                    </td>
                    <td>
                    
                        <a href="#" class="label" id="Delete_<%=item.PaperID%>">
                            删除</a>
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
        </div>
    </div>
    <p class="actionbar">
        <%: Html.ActionLink("添加试卷", "Create") %>
    </p>
</asp:Content>
