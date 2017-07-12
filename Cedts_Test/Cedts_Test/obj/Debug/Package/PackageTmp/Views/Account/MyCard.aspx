<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.CEDTS_Card>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    我绑定的充值卡
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../Scripts/jquery-1.7.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            if ($("#hidden").val() == "1") {
                alert("您当前激活的充值卡还没有到期，请稍后激活！");
            }
        })
    </script>
    <input type="hidden" id="hidden" value="<%:ViewData["time"] %>" />
    <table>
        <tr>
            <th>
                序列号
            </th>
            <th>
                充值卡类别
            </th>
            <th>
                充值卡类型
            </th>
            <th>
                激活时间
            </th>
            <th>
                激活状态
            </th>
            <th>
                有效期
            </th>
            <th>
                到期时间
            </th>
            <th>
                卡金额
            </th>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%: item.SerialNumber %>
            </td>
            <td>
                <% string CardKind = string.Empty;
                   switch (item.CardKind)
                   {
                       case 0: CardKind = "实体卡"; break;
                       case 1: CardKind = "虚拟卡"; break;
                       default: break;
                   }%><%:CardKind%>
            </td>
            <td>
                <% string CardType = string.Empty;
                   switch (item.CardType)
                   {
                       case 0: CardType = "年卡"; break;
                       case 1: CardType = "月卡"; break;
                       case 2: CardType = "次数卡"; break;
                       default: break;
                   } %><%:CardType%>
            </td>
            <td>
                <%: String.Format("{0:g}", item.ActivationTime) %>
            </td>
            <td>
                <% string ActivationState = string.Empty;
                   switch (item.ActivationState)
                   {
                       case 0: ActivationState = "未激活"; break;
                       case 1: ActivationState = "已激活"; break;
                       case 2: ActivationState = "已过期"; break;
                       default: break;
                   } %><%:ActivationState %>
            </td>
            <td>
                <%: item.EffectiveTime %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.OverdueTime) %>
            </td>
            <td>
                <%: String.Format("{0:F}", item.Money) %>
            </td>
            <td>
                <%if (item.ActivationState == 0)
                  { %>
                <%:Html.ActionLink("激活", "Activate", new { ID = item.ID })%>
                <%} %>
            </td>
        </tr>
        <% } %>
    </table>
    <%=Html.Pager(Model, new PagerOptions  
{  
    PageIndexParameterName = "id",  
    CssClass = "pages", 
    FirstPageText = "首页", 
    LastPageText = "末页",  
    PrevPageText = "上一页",  
    NextPageText = "下一页",  
    CurrentPagerItemWrapperFormatString = "<span class=\"cpb\">{0}</span>",  
    ShowPageIndexBox = true, 
    NumericPagerItemWrapperFormatString = "<span class=\"item\">{0}</span>",  
    PageIndexBoxType = PageIndexBoxType.DropDownList,   
    ShowGoButton = false,PageIndexBoxWrapperFormatString=" 转到{0}",SeparatorHtml = "" 
})%>
    <p>
        <%: Html.ActionLink("我要购买", "BuyCard") %>
        &nbsp<%:Html.ActionLink("返回","UserInfo") %>
    </p>
</asp:Content>
