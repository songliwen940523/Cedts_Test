<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_User>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    导入学生信息
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $().ready(function () {
            $("form").validate({});
            $("#UserAccount").rules("add", {
                required: true,
                rangelength: [1, 20],
                remote: {
                    type: "post",
                    url: "/Account/AjaxAccount",
                    data: { Account: function () { return $("#UserAccount").val(); }, UserID: function () { return 0; } },
                    dataType: "html",
                    dataFilter: function (data, type) { if (data == "False") return true; else return false; }
                },
                messages: { required: "帐号不能为空。", rangelength: "帐号不能超过20个字符。", remote: "帐号已经存在，请更换帐号。" }
            });
            $("#Email").rules("add", {
                required: true,
                email: true,
                rangelength: [1, 50],
                remote: {
                    type: "post",
                    url: "/Account/AjaxEmail",
                    data: { Email: function () { return $("#Email").val(); }, UserID: function () { return 0; } },
                    dataType: "html",
                    dataFilter: function (data, type) { if (data == "False") return true; else return false; }
                },
                messages: { required: "邮箱地址不能为空。", email: "邮箱格式不正确。", rangelength: "邮箱地址不能超过50个字符。", remote: "邮箱地址已经存在，请更换邮箱地址。" }
            });
            $("#UserNickname").rules("add", {
                required: true,
                rangelength: [1, 12],
                messages: { required: "用户姓名不能为空！", rangelength: "姓名不能超过12个字符。" }
            });
            $("#UserPassword").rules("add", {
                required: true,
                rangelength: [6, 20],
                messages: { required: "密码不能空。", rangelength: "密码必须为6-20位字符之间。" }
            });
            $("#UserPassword2").rules("add", {
                required: true,
                equalTo: "#UserPassword",
                rangelength: [6, 20],
                messages: { required: "密码确认不能为空。", equalTo: "两次密码不相同。", rangelength: "密码必须为6-20位字符之间。" }
            });
            $("#Phone").rules("add", {
                isPhone: true,
                messages: { isPhone: "联系电话格式不正确。" }
            });
            $("#StudentNumber").rules("add", {
                required: true,
                messages: { required: "学生编号不能为空。" }
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
            // 联系电话(手机/电话皆可)验证   
            jQuery.validator.addMethod("isPhone", function (value, element) {
                var length = value.length;
                var mobile = /^(((13[0-9]{1})|(15[0-9]{1}))+\d{8})$/;
                var tel = /^\d{3,4}-?\d{7,9}$/;
                return this.optional(element) || (tel.test(value) || mobile.test(value));

            }, "请正确填写您的联系电话");
        })        
    </script>
    <%-- 禁止输入空格字符--%>
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
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>学生信息导入：</legend>
        <div class="editor-label">
            学生帐号：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.UserAccount) %>
            <%: Html.ValidationMessageFor(model => model.UserAccount) %>
        </div>
        <div class="editor-label">
            学生密码：
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(model => model.UserPassword, new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
            <%: Html.ValidationMessageFor(model => model.UserPassword) %>
        </div>
        <div class="editor-label">
            确认密码：
        </div>
        <div class="editor-field">
            <%: Html.Password("UserPassword2", "", new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
            <span id="sp1" style="color: Red"></span>
        </div>
        <div class="editor-label">
            学生性别：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("Sex", ViewData["Sex"] as SelectList, new { id="Sex"})%>
        </div>
        <div class="editor-label">
            学生邮箱：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Email) %>
            <%: Html.ValidationMessageFor(model => model.Email) %>
        </div>
        <div class="editor-label">
            学生姓名：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.UserNickname) %>
            <%: Html.ValidationMessageFor(model => model.UserNickname) %>
        </div>
        <div class="editor-label">
            联系电话：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Phone) %>
            <%: Html.ValidationMessageFor(model => model.Phone) %>
        </div>
        <div class="editor-label">
            学生编号：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.StudentNumber) %>
            <%: Html.ValidationMessageFor(model => model.StudentNumber) %>
        </div>
        <p>
            <input type="submit" id="submit" value="提    交" />
        </p>
    </fieldset>
    <% } %>
</asp:Content>
