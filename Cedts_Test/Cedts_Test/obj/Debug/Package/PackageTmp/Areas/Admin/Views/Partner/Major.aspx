<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    专业管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("专业管理首页","Major") %>
    </div>
    <br />
    <div>
        <%:Html.Action("Major1")%>
    </div>
    <br />
    <hr />
    <br />
    <div>
        <%:Html.Action("Major2")%>
    </div>
    <br />
    <hr />
    <br />
    <div>
        <%:Html.Action("Major3")%>
    </div>
    <br />
    <div>
        <%:Html.ActionLink("新增专业", "CreateMajor")%>
    </div>
    <script type="text/javascript">
        $(function () {
            $.messager.defaults = { ok: "确   定", cancel: "取   消" };
            $(".a").click(function () {
                var id = $(this).attr("id");
                $.messager.confirm('提    示', '<b>你确认要删除该专业?<br>如果删除，与之相关的用户、班级、年级信息将全部丢失！</b>', function (r) {
                    if (r) {
                        location.href = "/Admin/Partner/DeleteMajor?id=" + id;
                    }
                });
            });
        });
    </script>
</asp:Content>
