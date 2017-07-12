<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/DetailsSite.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	系统概况
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $.post("/Account/sys", function (data) {
                $("#Sys").append(data);
            })
        })
    </script>
    <div id="Sys"></div>
    <div id="pp">
    </div>
</asp:Content>
