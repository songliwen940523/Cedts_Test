<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ViewPage1
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $("#Btn").click(function () {
            window.history.go(-1);
        })
    })
</script>
    <div>
        <h4>
            该题知识点已经赋值过，不能赋值第二次</h4>
        <input type="button" id="Btn" value="返回" />
    </div>
</asp:Content>
