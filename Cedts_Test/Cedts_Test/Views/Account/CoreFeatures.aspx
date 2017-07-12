<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/DetailsSite.Master"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<Cedts_Test.Models.CEDTS_CoreFeatures>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    特色功能
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var Num = $("#hid").val();
            for (var i = 0; i < Num; i++) {
                var content = $("#Node" + (i + 1)).val();
                $("#Content" + (i + 1)).html(content);
            }
        })
    </script>
    <table>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <h2>
                    特色功能<%:item.Order %></h2>
                <input type="hidden" id="Node<%:item.Order %>" value="<%:item.Content %>" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;&nbsp<span id="Content<%:item.Order %>"></span>
            </td>
        </tr>
        <% } %>
    </table>
    <input id="hid" type="hidden" value="<%: Model.Count()%>" />
    <div id="pp">
    </div>
</asp:Content>
