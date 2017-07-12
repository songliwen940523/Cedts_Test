<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Models.CEDTS_User>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    注册
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.EnableClientValidation(); %>
    <% using (Html.BeginForm(null, null, FormMethod.Post, new { @class = "form1" }))
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>新用户注册入口</legend>
        <div>
            带*项为必填项<br />
            用户信息：</div>
        <div class="editor-label">
            用户帐号（*）：
            <%: Html.TextBoxFor(model => model.UserAccount, new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
            <%: Html.ValidationMessageFor(model => model.UserAccount) %>
        </div>
        <div class="editor-field">
        </div>
        <div class="editor-label">
            用户姓名（*）：
            <%: Html.TextBoxFor(model => model.UserNickname, new { onkeyup = "value=value.replace(/[^\\a-\\z\\A-\\Z0-9\\u4E00-\\u9FA5\\_\\-]/g,'')" })%>
            <%: Html.ValidationMessageFor(model => model.UserNickname) %>
        </div>
        <div class="editor-label">
            用户邮箱（*）：
            <%: Html.TextBoxFor(model => model.Email) %>
            <%: Html.ValidationMessageFor(model => model.Email) %>
        </div>
        <div class="editor-label">
            用户密码（*）：
            <%: Html.PasswordFor(model => model.UserPassword, new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
            <%: Html.ValidationMessageFor(model => model.UserPassword) %>
        </div>
        <div class="editor-label">
            确认密码（*）：
            <%: Html.Password("UserPassword2", "", new { onkeyup = "value=value.replace(/[^0-9a-zA-Z]/g,'')" })%>
            <span id="sp1" style="color: Red"></span>
        </div>
        <div class="editor-label">
            性别（*）：
            <%: Html.RadioButton("Sex","男",true) %>男&nbsp;&nbsp;&nbsp;
            <%: Html.RadioButton("Sex","女") %>女
            <%: Html.ValidationMessage("Sex") %>
        </div>
        <div class="editor-label">
            联系电话：
            <%: Html.TextBoxFor(model => model.Phone) %>
            <%: Html.ValidationMessageFor(model => model.Phone) %>
        </div>
        <%:Html.CheckBox("studentinfo", false, new {value="false" })%>学生信息
        <div id="sinfo" style="display: none">
            <div class="editor-label">
                学校/机构：
                <%: Html.DropDownList("PartnerID")%><%: Html.ValidationMessageFor(model => model.PartnerID) %>
            </div>
            <div class="editor-label">
                专业： 学科门类：<%:Html.DropDownList("MajorID1") %>&nbsp; 一级学科：<%:Html.DropDownList("MajorID2") %>&nbsp;
                二级学科：<%:Html.DropDownList("MajorID") %><%: Html.ValidationMessageFor(model => model.MajorID) %>
            </div>
            <div class="editor-label">
                年级：
                <%:Html.DropDownList("GradeID", ViewData["GradeID"] as SelectList, new { id="GradeID"})%><%: Html.ValidationMessageFor(model => model.GradeID) %>
            </div>
            <div class="editor-label">
                班级：
                <%:Html.DropDownList("ClassID", ViewData["ClassID"] as SelectList, new { id="ClassID"})%><%: Html.ValidationMessageFor(model => model.ClassID) %>
            </div>
            <div class="editor-label">
                学生编号：
                <%: Html.TextBoxFor(model => model.StudentNumber) %>
                <%: Html.ValidationMessageFor(model => model.StudentNumber) %>
            </div>
        </div>
        <script type="text/javascript">
            function flush() {
                var verify = document.getElementById('valiCode');
                verify.setAttribute('src', '../../Account/GetValidateCode?' + Math.random());
            }  
        </script>
        <div class="editor-label">
            <%:Html.Label("验证码：") %>
            <%=Html.TextBox("txtimages", "", new { style = "width:70px" })%>
            <img id="valiCode" style="cursor: pointer;" src="../../Account/GetValidateCode" alt="点击刷新"
                onclick="flush();" />
            <%=Html .ValidationMessage("txtimages") %>
        </div>
        <p class="buttons">
            <input type="submit" value="注册" onclick="return CheckMGC();" />&nbsp;&nbsp;&nbsp;<input
                type="button" value="返回" onclick="window.location.href='/Account/LogOn';" />
        </p>
    </fieldset>
    <% } %>
    <%--客户端验证--%>
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.validate.min.js" type="text/javascript"></script>
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
                var mobile = /^(((13[0-9]{1})|(15[0-9]{1}))+\d{8})$/;
                var tel = /^\d{3,4}-?\d{7,9}$/;
                return this.optional(element) || (tel.test(value) || mobile.test(value));

            }, "请正确填写您的联系电话");

            $("#PartnerID").change(function () {
                $.post("/Account/GetMajorByPartnerID", { PartnerID: $(this).val() }, function (data) {
                    var vlist = "";
                    if (data != "") {
                        jQuery.each(data, function (i, n) {
                            vlist += "<option value=" + n.MajorID + ">" + n.MajorName + "</option>";
                        });
                        $("#MajorID1").empty();
                        $("#MajorID1").append(vlist);
                        $("#MajorID1").change();
                    }
                    else {
                        $("#MajorID1").empty();
                        $("#MajorID2").empty();
                        $("#MajorID").empty();
                        $("#GradeID").empty();
                        $("#ClassID").empty();
                        vlist = "<option value='00000000-0000-0000-0000-000000000000'>请选择</option>";
                        $("#MajorID1").append(vlist);
                        $("#MajorID2").append(vlist);
                        $("#MajorID").append(vlist);
                        $("#GradeID").append(vlist);
                        $("#ClassID").append(vlist);
                    }
                }, "json");
            })

            $("#MajorID1").change(function () {
                $.post("/Account/GetMajor", { MajorID: $(this).val() }, function (data) {
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
                $.post("/Account/GetMajor", { MajorID: $(this).val() }, function (data) {
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
                $.post("/Account/GetGrade", { MajorID: $(this).val() }, function (data) {
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
                $.post("/Account/GetClass", { GradeID: $(this).val() }, function (data) {
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
            $("#studentinfo").click(function () {
                if ($(this).val() == "true") {
                    $(this).val("false");
                    $("#sinfo").hide();

                    $("#PartnerID").children("option").each(function () {
                        if ($(this).val() == "00000000-0000-0000-0000-000000000000") {
                            $(this).attr("selected", true);
                            $("#PartnerID").change();
                            return false;
                        }
                    })
                }
                else {
                    $(this).val("true");
                    $("#sinfo").show();
                }
            })
        })

        function CheckMGC() {

            if ($("#studentinfo").val() == "true") {
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
</asp:Content>
