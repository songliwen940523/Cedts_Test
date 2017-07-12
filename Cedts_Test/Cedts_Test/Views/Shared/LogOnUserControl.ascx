<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<link href="../../Content/default.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">
    $(function () {
        $("#UbtnLogon").click(function () {
            var Account = $("#Account").val();
            var Password = $("#Password").val();
            if (Account == "" || Password == "") {
                alert("帐号密码不能为空！");
            }
            else {
                $.post("/Account/AjaxCheck", { Account: Account, Password: Password }, function (data) {
                    if (data == "True") {
                        var UserAccount = Account;
                        var UserPassword = Password;
                        $.post("/Account/LogOn", { UserAccount: UserAccount, UserPassword: UserPassword }, function (da) {
                            location.reload();
                        })
                    }
                    else {
                        alert("帐号密码错误！");
                    }
                })
            }
        })
    })
</script>
<%if (Request.IsAuthenticated)
  {%>
<%if (!Page.User.IsInRole("体验用户"))
  { %>
欢迎您，<b>Hi,<a href="../../PaperShow/UserTestInfo" style="background: none; border-style: none;"><%: Page.User.Identity.Name %></a></b>
    <%if (ViewData["ClassID"] != null) 
      {%>
      <%:Html.ActionLink("我的作业", "UserTask", "PaperShow", new { area = "" }, null)%>&nbsp;&nbsp;
      <%} %>
<%:Html.ActionLink("我的信息", "UserInfo", "Account", new { area = "" }, null)%>&nbsp;&nbsp;<%: Html.ActionLink("退出系统", "LogOff", "Account")%>
<%} %>
<% else
  { %>
  欢迎您，<b>Hi,<a href="../../PaperShow/UserTestInfo" style="background: none; border-style: none;">游客</a></b>
<div class="loginForm">
    <h4>
        登录</h4>
    <% Html.EnableClientValidation(); %>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <p>
        <label for="UserAccount">
            用户帐号</label><input type="text" value="" name="UserAccount" id="Account" class="txt" />
        <%: Html.ValidationMessage("UserAccount") %>
    </p>
    <p>
        <label for="UserPassword">
            用户密码</label><input type="password" value="" name="UserPassword" id="Password" class="txt" />
        <%: Html.ValidationMessage("UserAccount") %>
    </p>
    <p class="submit">
        <a class="btnLogon" id="UbtnLogon">登录</a> <a href="#link" class="Forget">忘记密码？</a></p>
</div>

<% } %>
<% } %>
<%}
  else
  {%>
<%}%>
