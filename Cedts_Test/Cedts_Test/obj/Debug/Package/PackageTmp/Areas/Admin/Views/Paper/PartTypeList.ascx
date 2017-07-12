<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Areas.Admin.Models.CEDTS_PartType>>" %>
<fieldset>
<legend>试卷题型数量信息：</legend>
<%int j = 0;%><%string[] arry = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" }; %>
<% foreach (var item in Model)
   { %>
<div id="parttype" style="border: 1px solid red;">
    <%:arry[j] %>、 类型英文名：<%: Html.Label(item.TypeName) %>； 类型中文名：<%: Html.Label(item.TypeName_CN) %><br />
    <br />
    <div id="itemtype">
        <% Html.RenderAction("ItemTypeList", new { PartTypeID = item.PartTypeID }); %>
    </div>
</div>
<br />
<%j++; %>
<% } %>
</fieldset>