<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/DetailsSite.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ContactUs
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr>
            <td>
                名称：<%:ViewData["Name"]%>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                联系电话：<%:ViewData["Tel"] %>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                邮编：<%:ViewData["ZipCode"]%>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                联系地址：<%:ViewData["Address"]%>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                官网：<%:ViewData["Url"]%>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                联系邮箱：sa515006@mail.ustc.edu.cn
            </td>
            <td>
            </td>
        </tr>
    </table>
    <div id="pp">
    </div>
</asp:Content>
