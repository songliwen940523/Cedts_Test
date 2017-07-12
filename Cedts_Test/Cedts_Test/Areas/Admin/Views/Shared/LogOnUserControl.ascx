<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%
    if (Request.IsAuthenticated)
    {
%>
欢迎您，Hi,<%: Page.User.Identity.Name %>
<%: Html.ActionLink("注销", "LogOff", "Account", new { area=""},null)%>
<%
    }
    else
    {
%>
<%: Html.ActionLink("登录", "LogOn", "Account", new { area = "" }, null)%>
<%
    }
%>
