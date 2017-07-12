<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_Grade>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    新增年级
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("年级管理首页","TGrade") %>
        =>
        <%:Html.ActionLink("年级信息添加","CreateTGrade") %>
    </div>
    <fieldset>
        <legend>年级创建：</legend>
        <div class="editor-label">
            年级名称：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.GradeName) %>
            <%: Html.ValidationMessageFor(model => model.GradeName) %>
        </div>
        <div class="editor-label" style="display: none">
            所属专业：
        </div>
        <div class="editor-field" style="display: none">
            学科门类：<%:Html.DropDownList("MajorID1") %>&nbsp;一级学科：<%:Html.DropDownList("MajorID2") %>&nbsp;二级学科：<%:Html.DropDownList("MajorID") %>
        </div>
        <p>
            <input type="submit" id="submit" value="提    交" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("返    回", "TGrade") %>
    </div>
    <script type="text/javascript">
        $(function () {
            $("form").validate({});
            $("#GradeName").rules("add", {
                required: true,
                rangelength: [1, 20],
                remote: {
                    type: "post",
                    url: "/../Partner/AjaxGradeName",
                    data: { GradeName: function () { return $("#GradeName").val(); }, MajorID: function () { if ($("#MajorID").val() == "00000000-0000-0000-0000-000000000000") return null; else return $("#MajorID").val(); } },
                    dataType: "html",
                    dataFilter: function (data, type) { if (data == "false") return true; else return false; }
                },
                messages: { required: "年级名称不能为空。", rangelength: "名字不能超过20个字符。", remote: "名字已经存在，请更换名字。" }
            });


            $("#MajorID1").change(function () {
                $.post("/../Partner/GetMajor", { MajorID: $(this).val() }, function (data) {
                    if (data != "") {
                        var vlist = "";
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
                        vlist = "<option value='00000000-0000-0000-0000-000000000000'>请选择</option>";
                        $("#MajorID2").append(vlist);
                        $("#MajorID").append(vlist);
                    }

                }, "json");
            })

            $("#MajorID2").change(function () {
                $.post("/../Partner/GetMajor", { MajorID: $(this).val() }, function (data) {
                    if (data != "") {
                        var vlist = "";
                        jQuery.each(data, function (i, n) {
                            vlist += "<option value=" + n.MajorID + ">" + n.MajorName + "</option>";
                        });
                        $("#MajorID").empty();
                        $("#MajorID").append(vlist);
                    }
                    else {
                        $("#MajorID").empty();
                        vlist = "<option value='00000000-0000-0000-0000-000000000000'>请选择</option>";
                        $("#MajorID").append(vlist);
                    }
                }, "json");
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
