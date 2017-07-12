<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Models.CEDTS_Card>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    充值卡购买
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../Scripts/jquery-1.7.js" type="text/javascript"></script>
    <script type="text/javascript">
        var a = 1;
        $(function () {
            $("#div_1").hide();
            $("#cardType").change(function () {
                $("#s_money").html("");
                $("#s_effective").html("");
                switch ($(this).val()) {
                    case "0": $("#s_money").html($("#h1").val()); $("#s_effective").html($("#h4").val()); break;
                    case "1": $("#s_money").html($("#h2").val()); $("#s_effective").html($("#h5").val()); break;
                    case "2": $("#s_money").html($("#h3").val()); $("#s_effective").html($("#h6").val()); break;
                    default: break;
                }
            })

            $("#btnBuy").click(function () {
                $.post("", {}, function () { }, "json");

            })

            $("#btnBind").click(function () {
                if ($("#txt").val() == "" || $("#pwd").val() == "" || $("#images2").val() == "") {
                    alert("序列号、密码、验证码均不能为空！");
                    return;
                }

                $.post("/Account/BuyCard", { txt: $("#txt").val(), pwd: $("#pwd").val(), image: $("#images2").val() }, function (data) {

                    switch (data) {
                        case "0": a = 0; $("#message2").html("").html("充值卡和密码不匹配！"); $("#images2").val(""); flush2(); break;
                        case "1": $("#message2").html("").html("绑定成功！"); window.location.href = "/Account/MyCard"; break;
                        case "2": a = 2; $("#message2").html("").html("充值卡已使用，请重新选择绑定！"); $("#images2").val(""); flush2(); break;
                        case "3": a = 3; $("#message2").html("").html("验证码错误！"); $("#images2").val(""); flush2(); break;
                        default: break;
                    }
                }, "json");

            })

            $("#txt").blur(function () {
                if (a == 0 || 2) { $("#message2").html(""); }
            })
            $("#pwd").blur(function () {
                if (a == 0 || 2) { $("#message2").html(""); }
            })
            $("#images2").blur(function () {
                if (a == 3) { $("#message2").html(""); }
            })

            $(":radio").click(function () {
                if ($(this).attr("id") == "r1") {
                    $("#div_1").hide();
                    flush2();
                    $("#div_2").show();
                }
                else {
                    $("#div_2").hide();
                    flush1();
                    $("#div_1").show();
                }
            })
        })
    </script>
    <center>
        <input type="radio" name="type" id="r1" value="0" checked="checked" />实体卡绑定&nbsp
        <input type="radio" name="type" id="r2" value="1" />虚拟卡购买
        <hr />
        <div id="div_1">
            <div>
                充值卡类型：<select id="cardType" name="cardType">
                    <option value="0">年卡</option>
                    <option value="1">月卡</option>
                    <option value="2">次数卡</option>
                </select>
                <br />
                <br />
                <br />
                <input type="hidden" id="h1" value="<%:ViewData["Year"] %>" />
                <input type="hidden" id="h2" value="<%:ViewData["Month"] %>" />
                <input type="hidden" id="h3" value="<%:ViewData["Times"] %>" />
                充值卡金额： <span id="s_money" style="color: Red">
                    <%:ViewData["Year"]%></span>
                <br />
                <br />
                <br />
                <input type="hidden" id="h4" value="<%:ViewData["YTime"] %>" />
                <input type="hidden" id="h5" value="<%:ViewData["MTime"] %>" />
                <input type="hidden" id="h6" value="<%:ViewData["TTime"] %>" />
                充值卡时效： <span id="s_effective" style="color: Red">
                    <%:ViewData["YTime"] %></span>
                <br />
                <br />
                <script type="text/javascript">
                    function flush1() {
                        var verify = document.getElementById('valiCode1');
                        verify.setAttribute('src', '../../Account/GetValidateCode?' + Math.random());
                    }  
                </script>
                <div class="editor-label">
                    <%:Html.Label("验证码：") %>
                    <%=Html.TextBox("txtimages", "", new { id="images1", style = "width:70px" })%>
                    <img id="valiCode1" style="cursor: pointer;" src="../../Account/GetValidateCode"
                        alt="点击刷新" onclick="flush1();" />
                    <div id="message1" style="color: Red">
                    </div>
                </div>
            </div>
            <div>
                <input type="button" id="btnBuy" value="购买" />&nbsp;
                <%:Html.ActionLink("返回","MyCard") %>
            </div>
        </div>
        <br />
        <br />
        <div id="div_2">
            <div>
                充值卡序列号：<input type="text" id="txt" /><br />
                <br />
                <br />
                充值卡密码：&nbsp;&nbsp;&nbsp;<input type="text" id="pwd" />
                <br />
                <br />
                <script type="text/javascript">
                    function flush2() {
                        var verify = document.getElementById('valiCode2');
                        verify.setAttribute('src', '../../Account/GetValidateCode?' + Math.random());
                    }  
                </script>
                <div class="editor-label">
                    <%:Html.Label("验证码：") %>
                    <%=Html.TextBox("txtimages", "", new { id="images2", style = "width:70px" })%>
                    <img id="valiCode2" style="cursor: pointer;" src="../../Account/GetValidateCode"
                        alt="点击刷新" onclick="flush2();" />
                    <div id="message2" style="color: Red">
                    </div>
                </div>
            </div>
            <div>
                <input type="button" id="btnBind" value="绑定" />&nbsp
                <%:Html.ActionLink("返回","MyCard") %>
            </div>
        </div>
    </center>
</asp:Content>
