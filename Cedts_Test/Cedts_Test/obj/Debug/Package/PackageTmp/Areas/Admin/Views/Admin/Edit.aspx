<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_User>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    编辑院校管理员信息
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%--    <script src="../../../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery.validate.min.js" type="text/javascript"></script>--%>
    <script type="text/javascript">
        $().ready(function () {
            $("form").validate({});
            $("#UserAccount").rules("add", {
                required: true,
                rangelength: [1, 20],
                remote: {
                    type: "post",
                    url: "/Account/AjaxAccount",
                    data:
                    {
                        Account: function () { return $("#UserAccount").val(); },
                        UserID: function () { return $("#UserID").val(); }
                    },
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
                    data: {
                        Email: function () { return $("#Email").val(); },
                        UserID: function () { return $("#UserID").val(); }
                    },
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
                rangelength: [6, 20],
                messages: { rangelength: "密码必须为6-20位字符之间。" }
            });
            $("#UserPassword2").rules("add", {
                equalTo: "#UserPassword",
                rangelength: [6, 20],
                messages: { equalTo: "两次密码不相同。", rangelength: "密码必须为6-20位字符之间。" }
            });
            $("#Phone").rules("add", {
                isPhone: true,
                messages: { isPhone: "联系电话格式不正确。" }
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
                var mobile = /^13[0-9]{1}[0-9]{8}$|15[0-9]{1}[0-9]{8}$|18[0-9]{9}$/;
                var tel = /^[0-9]{3,4}\-[0-9]{7,8}$/;
                return this.optional(element) || (tel.test(value) || mobile.test(value));

            }, "请正确填写您的联系电话");
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
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("院校管理员管理首页", "Index")%>
        =>
        <%:Html.ActionLink("修改院校管理员", "Edit")%>
    </div>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>修改管理员：</legend>
        <div style="display: none">
            <%:Html.HiddenFor(model=>model.UserID) %>
        </div>
        <div class="editor-label">
            真实姓名：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.UserNickname, new {onkeyup="value=value.replace(/[^\\a-\\z\\A-\\Z0-9\\u4E00-\\u9FA5\\_\\-]/g,'')" })%>
            <%: Html.ValidationMessageFor(model => model.UserNickname) %>
        </div>
        <div class="editor-label">
            用户帐号：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.UserAccount)%>
            <%: Html.ValidationMessageFor(model => model.UserAccount) %>
        </div>
        <div class="editor-label">
            用户密码：
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(model => model.UserPassword, new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
            <%: Html.ValidationMessageFor(model => model.UserPassword) %>
        </div>
        <div class="editor-label">
            <%: Html.Label("确认密码") %>:
        </div>
        <div class="editor-field">
            <%: Html.Password("UserPassword2", "", new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
            <span id="sp1" style="color: Red"></span>
        </div>
        <div class="editor-label">
            性别：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("Sex", ViewData["Sex"] as SelectList, new { id="Sex"})%>
        </div>
        <div class="editor-label">
            用户邮箱：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Email) %>
            <%: Html.ValidationMessageFor(model => model.Email) %>
        </div>
        <div class="editor-label">
            联系电话：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Phone) %>
            <%: Html.ValidationMessageFor(model => model.Phone) %>
        </div>
        <div class="editor-field">
            <%: Html.HiddenFor(model => model.PartnerID)%>
        </div>
        <div class="editor-field" style="display: none">
            <%: Html.TextBoxFor(model => model.RegisterTime, String.Format("{0:g}", Model.RegisterTime)) %>
        </div>
        <p>
            <input type="submit" value="保存" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("返回", "Index") %>
    </div>
</asp:Content>
