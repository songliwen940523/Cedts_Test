<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Cedts_Test.Areas.Admin.Models.CEDTS_Instructions>" %>
<script type="text/javascript">
    $(function () {

        var editor4 = CKEDITOR.replace('textarea4');

        $("#btnSave3").click(function () {
            var content4 = editor4.getData();
            $.post("/Admin/Notice/Instructions", { content4: content4 }, function (data) {
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
    <legend>使用说明</legend>
    <div class="editor-field">
        <div id="editor">
            <textarea id='textarea4' name='textarea4' cols='40' rows='8'><%:Model.Content %></textarea>
        </div>
    </div>
    <p class="actionbar">
        <input type="button" id="btnSave3" value="保存" />
    </p>
</fieldset>
<% } %>
