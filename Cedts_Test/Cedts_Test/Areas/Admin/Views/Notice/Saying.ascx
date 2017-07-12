<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Cedts_Test.Areas.Admin.Models.CEDTS_Saying>" %>
<script type="text/javascript">
    $(function () {
        $("#btnSave5").click(function () {
            var content5 = $("#Content").val();
            var note = $("#Note").val();
            $.post("/Admin/Notice/Saying", { content5: content5, note: note }, function (data) {
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
    <legend>名人名言</legend>
    <div class="editor-label">
        名言：
    </div>
    <div class="editor-field">
        <%: Html.TextBoxFor(model => model.Content, new { id = "Content" })%>
        <%: Html.ValidationMessageFor(model => model.Content) %>
    </div>
    <div class="editor-label">
        名人：
    </div>
    <div class="editor-field">
        <%: Html.TextBoxFor(model => model.Note, new { id = "Note" })%>
        <%: Html.ValidationMessageFor(model => model.Note) %>
    </div>
    <p class="actionbar">
        <input type="button" value="保存" id="btnSave5" />
    </p>
</fieldset>
<% } %>
