<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<Cedts_Test.Areas.Admin.Models.CEDTS_Paper>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    试卷添加——基本信息
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $().ready(function () {
            $("form").validate({});
            $("#Title").rules("add", {
                required: true,
                remote: {
                    type: "post",
                    url: "/Admin/Paper/AjaxTitle",
                    data: { name: function () { return $("#Title").val(); } },
                    dataType: "html",
                    dataFilter: function (data, type) { if (data == "False") return true; else return false; }
                },
                messages: { required: "标题不能为空！", remote: "标题已经存在，请更换标题。" }
            });
            $("#Duration").rules("add", {
                required: true,
                digits: true,
                messages: { required: "估时不能为空！", digits: "估时必须是数字。" }
            });
            $("#Score").rules("add", {
                required: true,
                digits: true,
                messages: { required: "分值不能为空！", digits: "分值必须是数字。" }
            });
            $("#Description").rules("add", {
                required: true,
                messages: { required: "描述不能为空！" }
            });
        })
    </script>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true)%>
    <div id="menu">
    当前位置： =>
    <%:Html.ActionLink("试卷管理首页", "Index")%>
    =>
    <%:Html.ActionLink("试卷基本信息添加", "Create")%>
    </div>
        
    <fieldset>
        <legend>试卷基本信息设置:</legend>
        <div>
            <%: Html.LabelFor(model => model.Title)%>：
            <%: Html.TextBoxFor(model => model.Title)%>
            <%: Html.ValidationMessageFor(model => model.Title)%>
        </div>
        <div>
            <%: Html.LabelFor(model => model.Type)%>：
            <%: Html.RadioButton("Type", "大学四级试题", true)%>大学四级试题&nbsp;<%: Html.RadioButton("Type", "大学六级试题", false)%>大学六级试题
            <%: Html.ValidationMessageFor(model => model.Type)%>
        </div>
        <div>
            <%: Html.LabelFor(model => model.Duration)%>：
            <%: Html.TextBoxFor(model => model.Duration)%>
            <%: Html.ValidationMessageFor(model => model.Duration)%>
        </div>
        <div>
            <%: Html.LabelFor(model => model.Difficult)%>：
            <%:Html.DropDownList("Difficult",new List<SelectListItem>{
                     (new SelectListItem() {Text = "0.1"}),
                     (new SelectListItem() {Text = "0.2"}),
                     (new SelectListItem() {Text = "0.3"}),
                     (new SelectListItem() {Text = "0.4"}),
                     (new SelectListItem() {Text = "0.5"}),
                     (new SelectListItem() {Text = "0.6"}),
                     (new SelectListItem() {Text = "0.7"}),
                     (new SelectListItem() {Text = "0.8"}),
                     (new SelectListItem() {Text = "0.9"}),
                     (new SelectListItem() {Text = "1"})
                })%>
        </div>
        <div>
            <%: Html.LabelFor(model => model.Score)%>：
            <%: Html.TextBoxFor(model => model.Score)%>
            <%: Html.ValidationMessageFor(model => model.Score)%>
        </div>
        <div>
            <%: Html.LabelFor(model => model.Description)%>：
            <%: Html.TextAreaFor(model => model.Description)%>
            <%: Html.ValidationMessageFor(model => model.Description)%>
        </div>
    </fieldset>
    <p>
        <input type="submit" value="下一步" />
    </p>
    <% } %>
    <%if (User.Identity.Name == "admin")
      { %>
    <div>
        <%: Html.ActionLink("返  回", "Index")%>
    </div>
    <%} %>
</asp:Content>
