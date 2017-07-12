<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/DetailsSite.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Models.CEDTS_Feedback>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    意见反馈
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>
            <h2>
                意见反馈</h2>
        </legend>
        <div class="editor-label">
            反馈标题：
        </div>
        <div class="editor-field">
            <input type="text" name="title" id="title" style="width: 500px" />
            <span id="titlePrompt" style="color: Red;"></span>
        </div>
        <div class="editor-label">
            反馈意见或者建议：
        </div>
        <div class="editor-field">
            <textarea name="content" id="content" rows="8" cols="60"></textarea>
            <span id="contentPrompt" style="color: Red;"></span>
        </div>
        <div class="editor-label">
            手机号码：
        </div>
        <div class="editor-field">
            <input type="text" name="tel" id="tel" style="width: 500px" />
            <span id="telPrompt" style="color: Red;"></span>
        </div>
        <div class="editor-label">
            邮箱地址：
            <div class="editor-field">
                <input type="text" name="email" id="email" style="width: 500px" />
                <span id="emailPrompt" style="color: Red;"></span>
            </div>
        </div>
        <p>
            <input type="submit" value="提交" onclick="return Check();" />
        </p>
    </fieldset>
    <div id="pp">
    </div>
    <% } %>
    <script type="text/javascript">
        function Check() {
            $("#titlePrompt").html("");
            $("#contentPrompt").html("");
            $("#telPrompt").html("");
            $("#emailPrompt").html("");
            if ($("#title").val() == "") {
                $("#titlePrompt").html("标题不能为空！");
                return false;
            }
            else {
                if ($("#content").val() == "") {
                    $("#contentPrompt").html("意见不能为空！");
                    return false;
                }
                else {
                    if ($("#tel").val() == "") {
                        $("#telPrompt").html("联系方式不能为空！");
                        return false;
                    } else {
                        var mobile = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
                        if (!mobile.test($("#tel").val())) {
                            $("#telPrompt").html("手机号码格式不正确！");
                            return false;
                        }
                        else {
                            if ($("#email").val() == "") {
                                $("#emailPrompt").html("邮箱不能为空！");
                                return false;
                            } else {
                                var myreg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
                                if (!myreg.test($("#email").val())) {
                                    $("#emailPrompt").html("邮箱格式不正确！");
                                    return false;
                                }
                                else {
                                    alert("提交成功！");
                                }
                            }
                        }
                    }
                }


            }
        }
    </script>
</asp:Content>
