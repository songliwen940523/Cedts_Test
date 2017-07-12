<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    密码找回
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% Html.EnableClientValidation(); %>
    <% using (Html.BeginForm(null, null, FormMethod.Post, new { @class = "form1" }))
       {%>
    <%: Html.ValidationSummary(true)%>
    <fieldset>
        <legend>密码找回入口：</legend>
        <div class="editor-label">
            <%: Html.Label("注册邮箱")%>: <span class="editor-field">
                <%: Html.TextBox("Email")%>
                <%: Html.ValidationMessage("Email")%>
            </span>
        </div>
        <script type="text/javascript">
            function flush() {
                var verify = document.getElementById('valiCode');
                verify.setAttribute('src', '../../Account/GetValidateCode?' + Math.random());
            }  
        </script>
        <div class="editor-label">
            <%:Html.Label("验证码：")%>
            <%=Html.TextBox("txtimages", "", new { style = "width:70px" })%>
            <img id="valiCode" style="cursor: pointer;" src="../../Account/GetValidateCode" alt="点击刷新"
                onclick="flush();" />
            <%=Html.ValidationMessage("txtimages")%>
        </div>
        <p class="buttons">
            <input type="submit" value="提交" />&nbsp;&nbsp;&nbsp;<input type="button" value="返回"
                onclick="window.location.reload('/Home/Index');" />
        </p>
    </fieldset>
    <% } %>
    <%--客户端验证--%>
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.validate.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $().ready(function () {
            $("form").validate({});
            $("#Email").rules("add", {
                required: true,
                email: true,
                rangelength: [1, 50],
                remote: {
                    type: "post",
                    url: "/Account/AjaxEmail",
                    data: { Email: function () { return $("#Email").val(); }, UserID: function () { return 0; } },
                    dataType: "html",
                    dataFilter: function (data, type) { if (data=="True") return true; else return false; }
                },
                messages: { required: "邮箱地址不能为空。", email: "邮箱格式不正确。", rangelength: "邮箱地址不能超过50个字符。", remote: "邮箱地址不存在。" }
            });
            jQuery.validator.addMethod("rangelength", function (value, element, param) {
                var length = value.length;
                for (var i = 0; i < value.length; i++) {
                    if (value.charCodeAt(i) > 127) {
                        length++;
                    }
                }
                return this.optional(element) || (length >= param[0] && length <= param[1]);
            }, "输入的值在3-15个字节之间。");
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
</asp:Content>
