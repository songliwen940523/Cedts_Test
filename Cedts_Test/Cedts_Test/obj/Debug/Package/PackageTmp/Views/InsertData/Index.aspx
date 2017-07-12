<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Models.CEDTS_Paper>>" %>

<%@ Import Namespace="Cedts_Test.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var id;
        $(function () {
            $("input[name='count']").keyup(function () {
                var tmptxt = $(this).val();
                $(this).val(tmptxt.replace(/\D|^0/g, ''));
            }).bind("paste", function () {
                var tmptxt = $(this).val();
                $(this).val(tmptxt.replace(/\D|^0/g, ''));
            }).css("ime-mode", "disabled");

            $(".a").each(function () {
                $(this).click(function () {
                    id = $(this).attr("id");
                    $(".d").each(function () {
                        $(this).hide();
                        $(this).parent("td").children("a").show();
                    })
                    $(this).hide();
                    $(this).parent("td").children("div").show();
                })
            })

        })
    </script>

    <table>
        <tr>
            <th>
                试卷标题
            </th>
            <th>
                试卷类型
            </th>
            <th>
                试卷估时
            </th>
            <th>
                试卷难度
            </th>
            <th>
                试卷总分
            </th>
            <th>
                试卷描述
            </th>
            <th>
            </th>
        </tr>
        <% foreach (var item in Model)
           { %>
        <tr>
            <td>
                <%: item.Title %>
            </td>
            <td>
                <%: item.Type %>
            </td>
            <td>
                <%: item.Duration %>
            </td>
            <td>
                <%: String.Format("{0:F}", item.Difficult) %>
            </td>
            <td>
                <%: item.Score %>
            </td>
            <td>
                <%: item.Description %>
            </td>
            <td>
                <%using (Html.BeginForm("Exerciser", "InsertData"))
                  { %>
                <%:Html.Hidden("hidden",item.PaperID) %>
                <input type="submit" class="submit" value="确定" />
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
    ShowGoButton = false,PageIndexBoxWrapperFormatString=" 转到{0}",SeparatorHtml = "" })%>
</asp:Content>
