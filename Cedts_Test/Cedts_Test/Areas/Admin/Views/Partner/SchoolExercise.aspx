<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    全校情况
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p class="tableinfo">时间段：<%:ViewData["StartDate"] %>至<%:ViewData["EndDate"] %></p>
    <p class="schoolinfo">在该段时间内，全校共做了<%:ViewData["TotalNum"] %>套卷，共做了<%:ViewData["TotalQue"] %>道题，做对<%:ViewData["CorQue"]%>道题，正确率为<%:ViewData["CorRate"] %>%。</p>
</asp:Content>
