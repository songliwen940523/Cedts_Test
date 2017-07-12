<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	试卷分析
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p class="tableinfo">试卷名称：<%:ViewData["PaperName"]%>&nbsp;&nbsp;&nbsp;&nbsp;班级名称：<%:ViewData["ClassName"] %></p>
    <%if (ViewData["Empty"] == "Empty")
      {%>
      <p class = "taskinfo">该班级没有学生！</p>
    <%} %>
    <%else
      { %>
    <div class="singlelist">
        <%:Html.Action("MultipleItemList", "Score")%>
        <%:Html.Action("MultipleKnowledgeList", "Score")%>
    </div>
    <%} %>

</asp:Content>
