<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Cedts_Test.Areas.Admin.Models.CEDTS_SystemOverview>" %>
<script type="text/javascript">
    $(function () {

        var editor = CKEDITOR.replace('textarea');

        $("#btnSave").click(function () {
            var content = editor.getData();
            var intro = $("#txtSystem").val();
            $.post("/Admin/Notice/System", { content: content, intro: intro }, function (data) {
                if (data == "1") {
                    alert("修改成功！");
                }
            }, "json");
        })
    })
</script>
<% using (Html.BeginForm())
   {%>
<%: Html.ValidationSummary(true) %>
<fieldset>
    <legend>系统概况</legend>
    <div class="editor-field">
        <div>
            <span>简介：<input type="text" id="txtSystem" value="<%:Model.Intro %>" style="width: 80%" /></span>
        </div>
        <div id="editor">
            <textarea id='textarea' name='textarea' cols='40' rows='8'><%:Model.Content %></textarea>
        </div>
    </div>
    <p class="actionbar">
        <input type="button" id="btnSave" value="保存" />
    </p>
</fieldset>
<% } %>
