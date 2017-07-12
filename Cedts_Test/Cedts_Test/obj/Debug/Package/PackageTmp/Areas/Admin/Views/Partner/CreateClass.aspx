<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_Class>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    新增班级
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("班级管理首页","Class") %>
        =>
        <%:Html.ActionLink("班级信息添加","CreateClass") %>
    </div>
    <fieldset>
        <legend>班级创建：</legend>
        <div class="editor-label">
            班级名称：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.ClassName) %>
            <%: Html.ValidationMessageFor(model => model.ClassName) %>
        </div>
          <div class="editor-label">
            班级教师：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("UserID") %>
        </div>
          <div class="editor-label">
            所属年级：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("GradeID") %>
        </div>
        <p>
            <input type="submit" id="submit" value="提    交" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("返    回", "Class") %>
    </div>
    <script type="text/javascript">
        $(function () {
            $("form").validate({});
            $("#ClassName").rules("add", {
                required: true,
                rangelength: [1, 50],
                remote: {
                    type: "post",
                    url: "/../Partner/AjaxClass",
                    data: { ClassName: function () { return $("#ClassName").val(); }},
                    dataType: "html",
                    dataFilter: function (data, type) { if (data == "false") return true; else return false; }
                },
                messages: { required: "班级名称不能为空。", rangelength: "名字不能超过50个字符。", remote: "名字已经存在，请更换名字。" }
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
