<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<Cedts_Test.Areas.Admin.Models.CEDTS_Feedback>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    意见反馈
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <center>
        <h2>
            意见反馈信息
        </h2>
    </center>
    <div>
        <% int i = 1; %>
        <% foreach (var item in Model)
           { %>
        <div>
            <fieldset>
                <legend class="actionbar">第<%:i %>条&nbsp;&nbsp;&nbsp;
                    <%: Html.ActionLink("删除", "DeleteFeedBack", new { id=item.FeedbackID })%>
                </legend>
                <div>
                    <b>反馈标题</b>：<%: item.Title %></div>
                <div>
                    <b>反馈信息</b>：<%: item.Content %></div>
                <div>
                    <b>联系方式</b>：<%: item.Tel %></div>
                <div>
                    <b>邮箱地址</b>：<%: item.Email %></div>
                <div>
                    <b>反馈时间</b>：<%: String.Format("{0:g}", item.Time) %></div>
            </fieldset>
            <%i++; %>
        </div>
        <% } %>
    </div>
    <p class="actionbar">
        <%if (Model.Count() > 0)
          {%>
        <%: Html.ActionLink("清空所有", "CleanFeedBack")%>
        <%} %>
        <%else
          { %>
          当前无信息
        <%} %>
    </p>
</asp:Content>
