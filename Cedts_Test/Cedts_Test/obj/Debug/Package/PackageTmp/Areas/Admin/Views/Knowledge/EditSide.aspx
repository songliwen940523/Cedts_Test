﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_KnowledgePoints>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  修改知识点能里面
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("form").validate({});
            $("#Title").rules("add", {
                required: true,
                messages: { required: "必填项，不能为空。" }
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
        <%:Html.ActionLink("修改知识点能力面","EditSide") %>
    </div>
    <fieldset>
        <legend>修改知识点能力面：</legend>
        <div class="editor-label">
            知识能力名称：
        </div>
        <div style="display:none">
            <%:Html.TextBoxFor(modle=>modle.KnowledgePointID) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Title) %>
            <%: Html.ValidationMessageFor(model => model.Title) %>
        </div>
        <div class="editor-label">
            知识能力描述：
        </div>
        <div class="editor-field">
            <%: Html.TextAreaFor(model => model.Describe) %>
            <%: Html.ValidationMessageFor(model => model.Describe) %>
        </div>
        <div class="editor-label">
            所属类型：
        </div>
        <div class="editor-field">
       
            <%:Html.DropDownList("Part", ViewData["Part"] as SelectList, new { id="Part"})%>
        </div>
        <p class="actionbar">
            <input type="submit" value="保存" /> <%: Html.ActionLink("返回", "Index") %>
        </p>
    </fieldset>
    <% } %>
</asp:Content>
