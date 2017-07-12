<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_Major>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    专业信息修改
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("专业管理首页","Major") %>
        =>
        <%:Html.ActionLink("专业信息修改","EditMajor") %>
    </div>
    <fieldset>
        <legend>专业信息修改：</legend>
        <%:Html.HiddenFor(model => model.MajorID)%>
        <div class="editor-label">
            专业名称：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.MajorName) %>
            <%: Html.ValidationMessageFor(model => model.MajorName) %>
        </div>
        <div class="editor-label">
            备 注：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.MajorMark) %>
            <%: Html.ValidationMessageFor(model => model.MajorMark) %>
        </div>
        <div class="editor-label">
            上级专业：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("UpMajorID")%>
        </div>
        <p>
            <input type="submit" value="提    交" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("返    回", "Major") %>
    </div>
    <script type="text/javascript">
        $(function () {
            $("form").validate({});
            $("#MajorName").rules("add", {
                required: true,
                rangelength: [1, 20],
                remote: {
                    type: "post",
                    url: "/../Partner/AjaxMajorName",
                    data: { MajorName: function () { return $("#MajorName").val(); }, MajorID: function () { return $("#MajorID").val(); } },
                    dataType: "html",
                    dataFilter: function (data, type) { if (data == "false") return true; else return false; }
                },
                messages: { required: "专业名称不能为空。", rangelength: "名字不能超过20个字符。", remote: "名字已经存在，请更换名字。" }
            });
        })


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
