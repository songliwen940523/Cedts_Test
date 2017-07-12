<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	年级分析
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p class="tableinfo">时间段：<%:ViewData["StartDate"] %>至<%:ViewData["EndDate"] %>&nbsp;&nbsp;&nbsp;&nbsp;年级：<%:ViewData["GradeName"] %></p>

    <div class="singlelist">
        <%:Html.Action("SingleItemList", "Partner")%>
        <%:Html.Action("SingleKnowledgeList", "Partner")%>

    </div>

</asp:Content>
