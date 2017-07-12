<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_KnowledgePoints>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    修改知识点能力
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#Sort").change(function () {
                $("#Side").empty();
                var selindex = $("#Sort").val();
                var url = "/Admin/Knowledge/GetSide/?type=" + selindex;
                $.post(url, { type: selindex }, function (data) {
                    $.each(data, function (i, item) {
                        $("<option value=" + item.Value + ">" + item.Text + "</option>").appendTo("#Side");
                    });
                }, "json");
            });
            $("form").validate({});
            $("#Title").rules("add", {
                required: true,
                messages: { required: "必填项，不能为空。" }
            });

            $("#Describe").rules("add", {
                required: true,
                messages: { required: "必填项，不能为空。" }
            });
        });
    </script>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("知识点管理首页","Index") %>
        =>
        <%:Html.ActionLink("修改知识点能力","EditPoint") %>
    </div>
    <fieldset>
        <legend>修改知识点能力：</legend>
        <div style="display: none">
            <%:Html.TextBoxFor(model=>model.Level) %>
            <%:Html.TextBoxFor(model=>model.KnowledgePointID) %>
        </div>
        <div class="editor-label">
            名称：
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Title) %>
            <%: Html.ValidationMessageFor(model => model.Title) %>
        </div>
        <div class="editor-label">
            描述：
        </div>
        <div class="editor-field">
            <%: Html.TextAreaFor(model => model.Describe) %>
            <%: Html.ValidationMessageFor(model => model.Describe) %>
        </div>
        <div class="editor-label">
            知识能力面：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("Sort", ViewData["Sort"] as SelectList, new { id="Sort"})%>
        </div>
        <div class="editor-label">
            类型名称：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("Side", ViewData["Side"] as SelectList, new { id="Side"})%>
        </div>
        <p class="actionbar">
            <input type="submit" value="保存" /> <%: Html.ActionLink("返回", "Index") %>
        </p>
    </fieldset>
    <% } %>
</asp:Content>
