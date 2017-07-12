<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Cedts_Test.Areas.Admin.Models.CEDTS_Contact>" %>
<script src="../../../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {

        $("#btnSave4").click(function () {
            var Name = $("#Name").val();
            var Tel = $("#Tel").val();
            var ZipCode = $("#ZipCode").val();
            var Address = $("#Address").val();
            var Url = $("#Url").val();
            $.post("/Admin/Notice/Contact", { Name: Name, Tel: Tel, ZipCode: ZipCode, Address: Address, Url: Url },
            function (data) {
                if (data == "1") {
                    alert("修改成功！");
                }
            }, "json");
        })
    })
</script>
    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        
        <fieldset>
            <legend>联系方式</legend>
            <div class="editor-label">
                公司名称：
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Name, new { id="Name"})%>
                <%: Html.ValidationMessageFor(model => model.Name) %>
            </div>
            
            <div class="editor-label">
                联系电话：
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Tel, new { id="Tel"})%>
                <%: Html.ValidationMessageFor(model => model.Tel) %>
            </div>
            
            <div class="editor-label">
                邮编：
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.ZipCode, new { id="ZipCode"})%>
                <%: Html.ValidationMessageFor(model => model.ZipCode) %>
            </div>
            
            <div class="editor-label">
                公司地址：
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Address, new { id="Address"})%>
                <%: Html.ValidationMessageFor(model => model.Address) %>
            </div>
            
            <div class="editor-label">
                公司官网：
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Url, new { id="Url"})%>
                <%: Html.ValidationMessageFor(model => model.Url) %>
            </div>
            
            <p class="actionbar">
                <input type="button" id="btnSave4" value="保存" />
            </p>
        </fieldset>

    <% } %>

