<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<Cedts_Test.Areas.Admin.Models.Class>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    班级列表
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="tbRecords">
        <tr>
            <th>
                班级名称
            </th>
            <th>
                班级人数
            </th>
            <th>
                编辑
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%: item.ClassName %>
            </td>
            <td>
                <%: item.StudentNum %>
            </td>
            <td>
                <%: Html.ActionLink("班级管理", "Index", new {ClassID=item.ClassID  }) %>
            </td>
        </tr>
        <% } %>
    </table>
</asp:Content>
