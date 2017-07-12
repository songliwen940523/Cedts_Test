<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_Major>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    新增专业
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("专业管理首页","Major") %>
        =>
        <%:Html.ActionLink("专业信息添加","CreateMajor") %>
    </div>
    <fieldset>
        <legend>专业创建：</legend>
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
            专业等级：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("MajorLevel", 
            new List<SelectListItem>() { 
                new SelectListItem { Text = "学科门类", Value = "1" },
                new SelectListItem { Text = "一级学科", Value = "2" },
                new SelectListItem { Text = "二级学科", Value = "3" }                
            })%>
        </div>
        <div id="upid" style="display: none">
            <div class="editor-label">
                上级专业：
            </div>
            <div class="editor-field">
                <%:Html.DropDownList("UpMajorID", new List<SelectListItem>() { new SelectListItem { Text = "无", Value = "00000000-0000-0000-0000-000000000000" } })%>
            </div>
        </div>
        <p>
            <input type="submit" id="submit" value="提    交" />
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
                    data: { MajorName: function () { return $("#MajorName").val(); } },
                    dataType: "html",
                    dataFilter: function (data, type) { if (data == "false") return true; else return false; }
                },
                messages: { required: "专业名称不能为空。", rangelength: "名字不能超过20个字符。", remote: "名字已经存在，请更换名字。" }
            });

            $("#MajorLevel").change(function () {

                if ($(this).val() == 1) {
                    $("#upid").hide();
                }
                else {
                    $("#upid").show();
                    $.post("/../Partner/GetMajorbyLevel", { level: $(this).val() },
                    function (data) {

                        if (data != "") {

                            var vlist = "";
                            jQuery.each(data, function (i, n) {
                                vlist += "<option value=" + n.MajorID + ">" + n.MajorName + "</option>";
                            });
                            $("#UpMajorID").empty();
                            $("#UpMajorID").append(vlist);
                        }
                        else {

                            $("#UpMajorID").empty();
                            var level = parseInt($("#MajorLevel").val());
                            if (level - 1 > 0) {
                                $("#MajorLevel").children("option").each(function () {
                                    if ($(this).val() == level - 1) {
                                        $(this).attr("selected", "selected");
                                        $("#MajorLevel").change();
                                    }
                                });
                                if (level == 2) {
                                    $("#upid").hide();
                                    alert("当前没有可选上级专业，请先添加上级专业！");
                                }
                            }
                        }
                    }, "json");
                }
            })

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
