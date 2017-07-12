<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    公告管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#btnSystem").click(function () {
                $("#system").show();
                $("#features").hide();
                $("#instructions").hide();
                $("#contact").hide();
                $("#saying").hide();
            })
            $("#btnFeatures").click(function () {
                $("#system").hide();
                $("#features").show();
                $("#instructions").hide();
                $("#contact").hide();
                $("#saying").hide();
            })
            $("#btnInstructions").click(function () {
                $("#system").hide();
                $("#features").hide();
                $("#instructions").show();
                $("#contact").hide();
                $("#saying").hide();
            })
            $("#btnContact").click(function () {
                $("#system").hide();
                $("#features").hide();
                $("#instructions").hide();
                $("#contact").show();
                $("#saying").hide();
            })
            $("#btnSaying").click(function () {
                $("#system").hide();
                $("#features").hide();
                $("#instructions").hide();
                $("#contact").hide();
                $("#saying").show();
            })
        })
    </script>
    <center>
        <h2>公告管理</h2>
        <p class="actionbar">
        <input type="button" id="btnSystem" value="系统概念" />&nbsp;
        <input type="button" id="btnFeatures" value="特色功能" />&nbsp;
        <input type="button" id="btnInstructions" value="使用说明" />&nbsp;
        <input type="button" id="btnContact" value="联系方式" />&nbsp;
        <input type="button" id="btnSaying" value="名人名言" />
        </p>
    </center>
    <div id="system">
        <%: Html.Action("System") %>
    </div>
    <div id="features" style="display: none">
        <%: Html.Action("Features")%>
    </div>
    <div id="instructions" style="display: none">
        <%: Html.Action("Instructions")%>
    </div>
    <div id="contact" style="display: none">
        <%:Html.Action("Contact")%>
    </div>
    <div id="saying" style="display: none">
        <%:Html.Action("Saying") %>
    </div>
</asp:Content>
