<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_User>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    新增教师
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



            $("#submit").click(function () {
                return CheckMGC();
            })

            $("#MajorID1").change(function () {
                $.post("/../Partner/GetMajor", { MajorID: $(this).val() }, function (data) {
                    var vlist = "";
                    if (data != "") {
                        jQuery.each(data, function (i, n) {
                            vlist += "<option value=" + n.MajorID + ">" + n.MajorName + "</option>";
                        });
                        $("#MajorID2").empty();
                        $("#MajorID2").append(vlist);
                        $("#MajorID2").change();
                    }
                    else {
                        $("#MajorID2").empty();
                        $("#MajorID").empty();
                        $("#GradeID").empty();
                        $("#ClassID").empty();
                        vlist = "<option value='00000000-0000-0000-0000-000000000000'>请选择</option>";
                        $("#MajorID2").append(vlist);
                        $("#MajorID").append(vlist);
                        $("#GradeID").append(vlist);
                        $("#ClassID").append(vlist);
                    }

                }, "json");
            })

            $("#MajorID2").change(function () {
                $.post("/../Partner/GetMajor", { MajorID: $(this).val() }, function (data) {
                    var vlist = "";
                    if (data != "") {
                        jQuery.each(data, function (i, n) {
                            vlist += "<option value=" + n.MajorID + ">" + n.MajorName + "</option>";
                        });
                        $("#MajorID").empty();
                        $("#MajorID").append(vlist);
                        $("#MajorID").change();
                    }
                    else {
                        $("#MajorID").empty();
                        $("#GradeID").empty();
                        $("#ClassID").empty();
                        vlist = "<option value='00000000-0000-0000-0000-000000000000'>请选择</option>";
                        $("#MajorID").append(vlist);
                        $("#GradeID").append(vlist);
                        $("#ClassID").append(vlist);
                    }
                }, "json");
            })

            $("#MajorID").change(function () {
                $.post("/../Partner/GetGrade", { MajorID: $(this).val() }, function (data) {
                    var vlist = "";
                    if (data != "") {
                        jQuery.each(data, function (i, n) {
                            vlist += "<option value=" + n.GradeID + ">" + n.GradeName + "</option>";
                        });
                        $("#GradeID").empty();
                        $("#GradeID").append(vlist);
                        $("#GradeID").change();
                    }
                    else {
                        $("#GradeID").empty();
                        $("#ClassID").empty();
                        vlist = "<option value='00000000-0000-0000-0000-000000000000'>请选择</option>";
                        $("#GradeID").append(vlist);
                        $("#ClassID").append(vlist);
                    }
                }, "json");
            })

            $("#GradeID").change(function () {
                $.post("/../Partner/GetClass", { GradeID: $(this).val() }, function (data) {
                    var vlist = "";
                    if (data != "") {
                        jQuery.each(data, function (i, n) {
                            vlist += "<option value=" + n.ClassID + ">" + n.ClassName + "</option>";
                        });
                        $("#ClassID").empty();
                        $("#ClassID").append(vlist);
                    }
                    else {
                        $("#ClassID").empty();
                        vlist = "<option value='00000000-0000-0000-0000-000000000000'>请选择</option>";
                        $("#ClassID").append(vlist);
                    }
                }, "json");
            })
        })

        function CheckMGC() {
            if ($("#MajorID").val() == "00000000-0000-0000-0000-000000000000") {
                alert("当前没有可选专业，请先添加专业！");
                return false;
            }
            if ($("#GradeID").val() == "00000000-0000-0000-0000-000000000000") {
                alert("当前没有可选年级，请先添加年级！");
                return false;
            }
            if ($("#ClassID").val() == "00000000-0000-0000-0000-000000000000") {
                alert("当前没有可选班级，请先添加班级！");
                return false;
            }
        }

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
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("教师管理首页","Teacher") %>
        =>
        <%:Html.ActionLink("教师信息添加","CreateTeacher") %>
    </div>
    <fieldset>
        <legend>教师创建：</legend>
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
            <%: Html.TextBoxFor(model => model.UserAccount, new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
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
            确认密码:
        </div>
        <div class="editor-field">
            <%: Html.Password("UserPassword2", "", new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
            <span id="sp1" style="color: Red"></span>
        </div>
        <div class="editor-label">
            性别：<%:Html.DropDownList("Sex", ViewData["Sex"] as SelectList, new { id="Sex"})%>
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
        <%--<div class="editor-label">
            专业： 学科门类：<%:Html.DropDownList("MajorID1") %>&nbsp;一级学科：<%:Html.DropDownList("MajorID2") %>&nbsp;二级学科：<%:Html.DropDownList("MajorID") %>
        </div>
        <div class="editor-label">
            年级：
            <%:Html.DropDownList("GradeID", ViewData["GradeID"] as SelectList, new { id="GradeID"})%>
        </div>
        <div class="editor-label">
            班级：<%:Html.DropDownList("ClassID", ViewData["ClassID"] as SelectList, new { id="ClassID"})%>
        </div>--%>
        <p>
            <input type="submit" id="submit" value="提  交" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("返  回", "Teacher") %>
    </div>
</asp:Content>
