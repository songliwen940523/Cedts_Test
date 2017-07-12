<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Cedts_Test.Areas.Admin.Models.CEDTS_CoreFeatures>" %>
<script type="text/javascript">
    $(function () {

        var editor1 = CKEDITOR.replace('textarea1');
        var editor2 = CKEDITOR.replace('textarea2');
        var editor3 = CKEDITOR.replace('textarea3');

        $("#btnSave2").click(function () {

            var content1 = editor1.getData();
            var content2 = editor2.getData();
            var content3 = editor3.getData();
            var txt1 = $("#txtOne").val();
            var txt2 = $("#txtTwo").val();
            var txt3 = $("#txtThree").val();

            $.post("/Admin/Notice/Features",
             {
                 content1: content1,
                 content2: content2,
                 content3: content3,
                 txt1: txt1,
                 txt2: txt2,
                 txt3: txt3
             }, function (data) {
                 if (data == "1") {
                     alert("修改成功！");
                 }
             }, "json");
        })
    })
</script>
<fieldset>
    <legend>系统特色</legend>
    <div>
        <h3>
            <span>特色一：</span>
        </h3>
        <span>简介：<input type="text" id="txtOne" value="<%:ViewData["txt1"]%>" style="width: 80%" /></span>
        <span>
            <textarea id='textarea1' name='textarea1' cols='40' rows='8'> <%:ViewData["content1"]%></textarea>
        </span>
    </div>
    <hr />
    <div>
        <h3>
            <span>特色二：</span>
        </h3>
        <span>简介：<input type="text" id="txtTwo" value="<%:ViewData["txt2"]%>" style="width: 80%" /></span> <span>
            <textarea id='textarea2' name='textarea2' cols='40' rows='8'><%:ViewData["content2"]%></textarea>
        </span>
    </div>
    <hr />
    <div>
        <h3>
            <span>特色三：</span>
        </h3>
        <span>简介：<input type="text" id="txtThree" value="<%:ViewData["txt3"]%>" style="width: 80%" /></span>
        <span>
            <textarea id='textarea3' name='textarea3' cols='40' rows='8'><%:ViewData["content3"]%></textarea>
        </span>
    </div>
    <hr />
</fieldset>
<p class="actionbar">
    <input type="button" id="btnSave2" value="保存" />
</p>
