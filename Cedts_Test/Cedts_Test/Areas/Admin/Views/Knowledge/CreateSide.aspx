<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_KnowledgePoints>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    新增知识点能里面
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("form").validate({});
            $("#Title").rules("add", {
                required: true,
                remote:
                 {
                     type: "post",
                     url: "/Admin/Knowledge/AjaxPointTitle",
                     data: { Title: function () { return $("#Title").val(); }, type: function () { return 2; } },
                     dataType: "html",
                     dataFilter: function (data) { if (data == "False") return true; else return false; }
                 },
                messages: { required: "必填项，不能为空。", remote: "知识点名称已经存在，请更换名称。" }
            });

            $("#Describe").rules("add", {
                required: true,
                messages: { required: "必填项，不能为空。" }
            });
        })

    </script>

    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("知识点管理首页","Index") %>
        =>
        <%:Html.ActionLink("新增知识点能力面","CreateSide") %>
    </div>
    <fieldset>
        <legend>新增知识点能力面：</legend>
        <div class="editor-label">
            类型名称：
        </div>
        <div class="editor-field">
            <%:Html.DropDownList("Part", ViewData["PartName"] as SelectList, new { id="Part"})%>
        </div>
        <div class="editor-label">
            知识能力面名称
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Title) %>
            <%: Html.ValidationMessageFor(model => model.Title) %>
        </div>
        <div class="editor-label">
            知识能力面描述
        </div>
        <div class="editor-field">
            <%: Html.TextAreaFor(model => model.Describe) %>
            <%: Html.ValidationMessageFor(model => model.Describe) %>
        </div>
        <p class="actionbar">
            <input type="submit" value="新增" /> <%: Html.ActionLink("返回", "Index") %>
        </p>
    </fieldset>
    <% } %>
</asp:Content>
