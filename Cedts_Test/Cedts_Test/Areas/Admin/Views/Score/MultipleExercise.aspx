<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/Admin/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SelectListItem>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	多次练习
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="form1" runat="server">

    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true)%>
         <div class="centralcontent">    
            <fieldset>
            <legend>成绩查看条件选择:</legend>
                <br />
                <%if (Model.Count() > 0)
                  { %>
                <table border="0" cellspacing="0" cellpadding="0" style="margin-left: 100px;">
                	<tr>
                		<td valign="top">请选择练习名称： &nbsp;&nbsp;</td>
                        <td>
                        <%foreach (var item in Model)
                          {%>
<%--                          <%:Html.CheckBox("PaperName", false, new { value = item.Value})%>  <%:item.Text %><br />--%>
                            <input type="checkbox" name="PaperName" value="<%:item.Value %>" />  <%:item.Text%><br />
                        <%} %>
                        </td>
                	</tr>
                </table>           
                <p class="needspace"><input type="button" value="全选" id = "selectall"/>&nbsp;&nbsp;<input type="button" value="清除" id = "selectnone"/>&nbsp;&nbsp;<input type="submit" value="确定" id = "submit"/>
                </p><br />
                <%} %>
                <%else
                  { %>
                    <p class="needspace">当前时间段没有练习！</p><br />
                    <p class="needspace"><input type="button" value="返回" id = "goback"/></p>
                <%} %>
            </fieldset>
            <%} %>
        </div>
    </form>
    <script type="text/javascript">
        $(function () {
            $("form").validate({});
            $("#selectall").click(function () {
                $(":checkbox").each(function () {
                    $(this).attr("checked", true);
                })
            })
            $("#selectnone").click(function () {
                $(":checkbox").each(function () {
                    $(this).attr("checked", false);
                })
            })
            $("#goback").click(function () {
                window.location.href = "/Admin/Score/Single";
            })
            $("#submit").click(function () {
                var flag = false;
                $(":checkbox").each(function () {
                    if ($(this).attr("checked") == true) {
                        flag = true;
                    }
                })
                if (!flag) {
                    alert("请至少选择一个试卷名称！");
                    return false;
                }
            });
        });
    </script>
    
</asp:Content>
