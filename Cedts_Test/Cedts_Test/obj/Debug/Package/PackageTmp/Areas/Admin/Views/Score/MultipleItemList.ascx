<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Cedts_Test.Areas.Admin.Models.WrongItemInfo>>" %>
<%@ Import Namespace="Cedts_Test.Areas.Admin.Models" %>
<%if (Model.Count() > 0)
  {%>
<table class="tbRecords" cellspacing="0">
    <caption>易错题型列表</caption>
    <thead>
        <tr>
            <th class="pname">
                题型名称
            </th>
            <th class="pname">
                题目数量
            </th>
            <th class="pname">
                得分率
            </th>
        </tr>
    </thead>    
    <tbody>
        <% foreach (var item in Model)
            { %>
        <tr>
            <td class="pname">
                <%: item.ItemName%>
            </td>
            <td class="pname">
                <%: item.TotalNum%>
            </td>
            <td class="pname">
                <%: item.CorrectRate.ToString("0.0")%>%
            </td>
        </tr>
        <% } %>
    </tbody>
</table>
<%} %>
<%else
  {%>
    <h3>当前没有同学进行过练习！</h3>
<% }%>
