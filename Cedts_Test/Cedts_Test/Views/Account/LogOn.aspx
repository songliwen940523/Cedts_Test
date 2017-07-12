<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />

    <title>首页</title>
    <link rel="Shortcut Icon" href="/favicon.ico" type="image/x-icon" />
    <link href="../../Content/default.css" rel="stylesheet" type="text/css" />
    <link href="../../Content/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Content/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/slides.min.jquery.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.validate.min.js" type="text/javascript"></script>
    <script src="../../Scripts/default.js" type="text/javascript"></script>
</head>
<body class="index">

    <div class="header wrp">
        <h1>
            <a href="#link" class="logo">英语诊断性练习系统</a></h1>
        <div id="loginForm" class="loginForm">
            <h4>
                登录</h4>
            <% Html.EnableClientValidation(); %>
            <% using (Html.BeginForm())
               {%>
            <%: Html.ValidationSummary(true) %>
            <p>
                <label for="UserAccount">
                    用户帐号</label><input type="text" value="" name="UserAccount" id="UserAccount" class="txt">
                <%: Html.ValidationMessage("UserAccount") %>
            </p>
            <p>
                <label for="UserPassword">
                    用户密码</label><input type="password" value="" name="UserPassword" id="UserPassword"
                        class="txt">
                <%: Html.ValidationMessage("UserAccount") %>
            </p>
            <p class="submit">
                <a class="btnLogon" id="btnLogon">登录</a> <a href="#link" class="Forget">忘记密码？</a></p>
            <div style="display: none;">
                <input type="submit" id="submit" /></div>
            <% } %>
        </div>
    </div>
    <div class="sitePromotion">
        <div class="slides">
            <div class="slides_container">
                <div class="slide">
                    <img src="../../Images/slide1.gif" alt="" /></div>
                <div class="slide">
                    <img src="../../Images/slide2.gif" alt="" /></div>
                <div class="slide">
                    <img src="../../Images/slide3.gif" alt="" /></div>
            </div>
        </div>
        <div class="textOnScreen">
            <ol>
                <li>我们拥有海量的题库</li>
                <li>我们能专业的分析出你需要改进的地方，并为你制定具有高效的学习方案</li>
                <li>我们用图形形象的描述出你的成长点滴</li>
            </ol>
        </div>
        <div class="quotes">
            <blockquote>
                <%:ViewData["SayContent"]%><cite>——<%:ViewData["SayNote"]%></cite></blockquote>
        </div>
        <div class="quickButtons">
            <div id="before">
                <a href="#link" class="btnQuickExp" style="display:none">快速体验</a> <a href="#link" class="btnRegNow">免费注册</a>
                <a href="#link" class="btnQuickLogin">登录系统</a>
            </div>
            <div id="after" style="display: none;">
                <span id="ViewName">
                    <%:ViewData["Name"] %></span><br />
                <span>使用诊断练习系统！</span><br />
                <h2>
                     <%if (ViewData["Role"] != null)
                  { %>
                <%if (ViewData["Role"].ToString() == "教师")
                  {%>
                      <%:Html.ActionLink("进入个人中心", "ClassList", "User", new { area = "admin" }, null)%>
                <%} %>
                <%else if (ViewData["Role"].ToString() == "管理员")
                  { %>
                      <%:Html.ActionLink("进入个人中心", "Teacher", "Partner", new { area = "admin" }, null)%>
                <%} %>
                <% else if (ViewData["Role"].ToString() == "超级管理员")
                  { %>
                     <%:Html.ActionLink("进入个人中心", "Index", "Partner", new { area = "admin" }, null)%>
                <%} %>
                <% else
                  { %>
                      <%:Html.ActionLink("进入个人中心", "UserTestInfo", "PaperShow")%>
                <%} %>
                <%} %></h2>
                <%: Html.ActionLink("退出系统", "LogOff", "Account")%>
            </div>
        </div>
    </div>
    <div class="body wrp">
        <div class="siteHighlights">
            <div class="overview highlight">
                <h2>
                    系统概览 <span class="en">System Overview</span></h2>
                <p><%:ViewData["SystemOverview"]%><br />
                    <a href="/Account/SystemOverview" target="_blank">详细»
                        </a></p>
            </div>
            <div class="features highlight">
                <h2>
                    特色功能 <span class="en">Core Features</span></h2>
                <ul class="featureList">
                    <li>
                        <h3>
                            特色功能一</h3>
                        <p>
                            <a href="/Account/CoreFeatures" target="_blank">
                                <%:ViewData["CoreFeaturesOne"] %></a></p>
                    </li>
                    <li>
                        <h3>
                            特色功能二</h3>
                        <p>
                            <a href="/Account/CoreFeatures" target="_blank">
                                <%:ViewData["CoreFeaturesTwo"] %></a></p>
                    </li>
                    <li>
                        <h3>
                            特色功能三</h3>
                        <p>
                            <a href="/Account/CoreFeatures" target="_blank">
                                <%:ViewData["CoreFeaturesThree"]%></a></p>
                    </li>
                </ul>
            </div>
            <div class="activities highlight">
                <h2>
                    用户动态 <span class="en">User Activities</span></h2>
                <ul class="actList">
                    <% var testingList = ViewData["Testing"] as List<Cedts_Test.Models.CEDTS_Testing>;
                       foreach (var t in testingList)
                       { %>
                    <li><span id="disPaperID" style="display: none">
                        <%:t.PaperID %></span> <span><a href="#link" id="Practicing" class="Practic" name="<%:t.PaperID %>">
                            <%:t.UserAccount %>用户正在练习<%:t.PaperTitle %></a></span> </li>
                    <%  }
                    %>
                </ul>
            </div>
        </div>
    </div>
    <div class="foot wrp">
        <p>
            ©2009-2012 All Rights Reserved 科大讯飞 中国科大 上海外语教育出版社</p>
        <p>
            <a href="/Account/Instructions" target="_blank">使用说明</a> | <a href="/Account/ContactUs"
                target="_blank">联系方式</a> | <a href="/Account/Feedback" target="_blank">意见反馈</a></p>
        <p>
            ICP备案号：皖ICP备05002528号-1</p>
    </div>
    <div id="dd" icon="icon-save"
        style="padding: 5px; width: 335px; height: 156px;">
        <br />
        用户名称：<input type="text" id="Name" style="width: 200px;" /><br />
        <br />
        用户密码：<input type="password" id="Pass" style="width: 200px;" />
    </div>
    
</body>
</html>
<%--客户端验证--%>
<script type="text/javascript">
    var attr;
    $('#dd').dialog({
        title: '登陆',
        modal: true,
        closed: true,
        buttons: [{
            text: '确认',
            iconCls: 'icon-ok',
            handler: function () {
                var name = $("#Name").val();
                var pass = $("#Pass").val();
                $.post("/Account/LogOn1", { Name: name, Pass: pass }, function (data) {
                    if (data == 1) {
                        var href = "/PaperShow/Exerciser/" + attr;
                        window.location.href = href;
                    }
                })
            }
        }, {
            text: '取消',
            handler: function () {
                $('#dd').dialog('close');
            }
        }]
    });
    function open1() {
        $('#dd').dialog('open');
    }
    function close1() {
        $('#dd').dialog('close');
    }
    $().ready(function () {
        $(".Practic").click(function () {
            attr = $(this).attr("name");
            if ($("#after").css("display") != "none") {
                var href = "/PaperShow/Exerciser/" + attr;
                window.location.href = href;
            }
            else {
                open1();
            }
        })
        $(".btnQuickExp").click(function () {
            $.post("/Account/VisitorRegister", null, function (data) {
                window.location.href = '../PaperShow/UserTestInfo';
            })
        })
        $("#Register").click(function () {
            window.location.href = '../Account/Register';
        })
        $.post("/account/State", null, function (data) {
            if (data == 1) {
                alert("您好！你的密码已经重置为123123，请登录后修改密码。");
            }
        }, "json");
    })
</script>
<%--禁止输入空格字符--%>
<script type="text/javascript">
    document.body.onload = function resets() {
        var controls = document.getElementsByTagName('input');
        for (var i = 0; i < controls.length; i++) {
            if (controls[i].type == 'text' || controls[i].type == 'password') {
                controls[i].onkeydown = function () {
                    if (event.keyCode == 32)
                        return false;
                };
            }
        }
    }
</script>
