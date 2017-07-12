<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master"
    Inherits="System.Web.Mvc.ViewPage<PagedList<Cedts_Test.Areas.Admin.Models.CEDTS_Log>>" %>

<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%@ Import Namespace="Webdiyer.WebControls.Mvc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    日志管理首页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            //全选或全不选 
            $("label").click(function () {//当点击全选框时 
                var flag = $("#checkall").attr("checked"); //判断全选按钮的状态 
                $(":checkbox").each(function () {//查找每一个checkbox 
                    $(this).attr("checked", !flag); //选中或者取消选中 
                });
            });
            //如果全部选中勾上全选框，全部选中状态时取消了其中一个则取消全选框的选中状态 
            $(":checkbox").each(function () {
                $(this).click(function () {
                    if ($("[name$='check']:checked").length == $("[name$='check']").length) {
                        $("#checkall").attr("checked", "checked");
                    }
                    else $("#checkall").removeAttr("checked");
                });
            });

            $("#btnDelete").click(function () {
                var nums = "";
                $("[name$='check']:checked").each(function () {
                    nums = nums + $(this).parents("tr").attr("id") + ",";
                });
                nums = nums.substring(0, nums.lastIndexOf(','));
                if (nums.length < 1)
                    alert("请选择要删除的内容");
                else
                    $.post("/Admin/Log/Delete", { ids: nums }, function (data) { alert(data); window.location.reload(); }, "json");
            })
        })
    </script>
    <div id="menu">
        当前位置： =>
        <%:Html.ActionLink("日志管理首页","Index") %>
    </div>
    <table  class="tbRecords" cellspacing="0" >
        <tr>
            <th class="pname">
                <label>
                    <input type="checkbox" id="checkall" name="checkall" />全选</label>
            </th>
            <th class="pname">
                用户ID
            </th>
            <th class="pname">
                记录描述
            </th>
            <th class="pname">
                操作时间
            </th>
            <th class="pname">
                客户端IP
            </th>
            <th class="pname">
                方法名称
            </th>
        </tr>
        <%int i = 0; %>
        <% foreach (var item in Model)
           { %>
        <%i++; %>
        <tr id='<%:item.LogID %>'>
            <td>
                <input type="checkbox" id="check<%:i %>" name="check" />
            </td>
            <td>
                <%: item.UserID %>
            </td>
            <td>
                <%: item.Content %>
            </td>
            <td>
                <%: String.Format("{0:g}", item.LogTime) %>
            </td>
            <td>
                <%: item.ClientIP %>
            </td>
            <td>
                <%: item.Action %>
            </td>
        </tr>
        <% } %>
    </table>
    <%=Html.Pager(Model, new PagerOptions  
{  
    PageIndexParameterName = "id",  
    CssClass = "pages", 
    FirstPageText = "首 页", 
    LastPageText = "末 页",  
    PrevPageText = "上一页",  
    NextPageText = "下一页",  
    CurrentPagerItemWrapperFormatString = "<span class=\"cpb\">{0}</span>",  
    ShowPageIndexBox = true, 
    NumericPagerItemWrapperFormatString = "<span class=\"item\">{0}</span>",  
    PageIndexBoxType = PageIndexBoxType.DropDownList,   
    ShowGoButton = false,PageIndexBoxWrapperFormatString=" 转到{0}",SeparatorHtml = "" })%>
    <p class="actionbar">
        <% if (User.IsInRole("超级管理员"))
           { %>
        <input type="button" id="btnDelete" value="删 除" />
        <%} %>
    </p>
</asp:Content>
