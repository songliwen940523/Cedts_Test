<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/DetailsSite.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	使用说明
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $.post("/Account/instr", function (data) {
                $("#instr").append(data);
            })
        })
    </script>
    <div id="instr"></div>
    <div id="pp">
    </div>
</asp:Content>
